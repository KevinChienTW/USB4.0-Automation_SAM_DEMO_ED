using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NationalInstruments.Visa;
using System.IO;
using System.Threading;

namespace USB4._0_Automation
{
    public partial class DeEmbedd : Form
    {
        private MessageBasedSession mbSession_E5071C;
        string str;
        string gpib_address_E5071C = "";
        string gpib_address_Switch = "";
        string read_value = "";

        string ENA_lastest_folder_location;
        string[] ENA_folder_location_history = new string[20];

        string Case_type = "";
        bool trigger_listBox1_doubleclick = false;


        int ENA_folder_location_history_i = 0;
        IniManager iniManager = new IniManager(System.Environment.CurrentDirectory + "\\" + "setting.ini"); //設定ini檔路徑
        public DeEmbedd()
        {
            InitializeComponent();
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            gpib_address_Switch = iniManager.ReadIniFile("Session", "Switch_Resource", "");
            gpib_address_E5071C = iniManager.ReadIniFile("Session", "E5071C_Resource", "");

            ENA_folder_location_history[ENA_folder_location_history_i] = "D:\\CAMS_DEEMBEDD\\";
            ENA_lastest_folder_location = ENA_folder_location_history[ENA_folder_location_history_i];
            Search_E5071C_Folder(ENA_lastest_folder_location);

            string A_TX1 = iniManager.ReadIniFile("DeEmbedd", "sideA_TX1", "");
            string A_RX1 = iniManager.ReadIniFile("DeEmbedd", "sideA_RX1", "");
            string A_TX2 = iniManager.ReadIniFile("DeEmbedd", "sideA_TX2", "");
            string A_RX2 = iniManager.ReadIniFile("DeEmbedd", "sideA_RX2", "");
            string A_DD = iniManager.ReadIniFile("DeEmbedd", "sideA_DD", "");

            string B_TX1 = iniManager.ReadIniFile("DeEmbedd", "sideB_TX1", "");
            string B_RX1 = iniManager.ReadIniFile("DeEmbedd", "sideB_RX1", "");
            string B_TX2 = iniManager.ReadIniFile("DeEmbedd", "sideB_TX2", "");
            string B_RX2 = iniManager.ReadIniFile("DeEmbedd", "sideB_RX2", "");
            string B_DD = iniManager.ReadIniFile("DeEmbedd", "sideB_DD", "");
            //.substring 取得部分字串
            //.lastindexof 報告指定的Unicode 字符或String 在此實例中的最後一個匹配項的索引位置。
            //http://a-jau.blogspot.com/2012/01/cstringindexoflastindexofsubstringsplit.html
            sideA_TX1.Items.Add(A_TX1.Substring(A_TX1.LastIndexOf("\\") + 1, A_TX1.Length - A_TX1.LastIndexOf("\\") - 1));
            sideA_RX1.Items.Add(A_RX1.Substring(A_RX1.LastIndexOf("\\") + 1, A_RX1.Length - A_RX1.LastIndexOf("\\") - 1));
            sideA_TX2.Items.Add(A_TX2.Substring(A_TX2.LastIndexOf("\\") + 1, A_TX2.Length - A_TX2.LastIndexOf("\\") - 1));
            sideA_RX2.Items.Add(A_RX2.Substring(A_RX2.LastIndexOf("\\") + 1, A_RX2.Length - A_RX2.LastIndexOf("\\") - 1));
            sideA_DD.Items.Add(A_DD.Substring(A_DD.LastIndexOf("\\") + 1, A_DD.Length - A_DD.LastIndexOf("\\") - 1));

            sideB_TX1.Items.Add(B_TX1.Substring(B_TX1.LastIndexOf("\\") + 1, B_TX1.Length - B_TX1.LastIndexOf("\\") - 1));
            sideB_RX1.Items.Add(B_RX1.Substring(B_RX1.LastIndexOf("\\") + 1, B_RX1.Length - B_RX1.LastIndexOf("\\") - 1));
            sideB_TX2.Items.Add(B_TX2.Substring(B_TX2.LastIndexOf("\\") + 1, B_TX2.Length - B_TX2.LastIndexOf("\\") - 1));
            sideB_RX2.Items.Add(B_RX2.Substring(B_RX2.LastIndexOf("\\") + 1, B_RX2.Length - B_RX2.LastIndexOf("\\") - 1));
            sideB_DD.Items.Add(B_DD.Substring(B_DD.LastIndexOf("\\") + 1, B_DD.Length - B_DD.LastIndexOf("\\") - 1));


            sideA_TX1.Text = sideA_TX1.Items[0].ToString();
            sideA_RX1.Text = sideA_RX1.Items[0].ToString();
            sideA_TX2.Text = sideA_TX2.Items[0].ToString();
            sideA_RX2.Text = sideA_RX2.Items[0].ToString();
            sideA_DD.Text = sideA_DD.Items[0].ToString();

            sideB_TX1.Text = sideB_TX1.Items[0].ToString();
            sideB_RX1.Text = sideB_RX1.Items[0].ToString();
            sideB_TX2.Text = sideB_TX2.Items[0].ToString();
            sideB_RX2.Text = sideB_RX2.Items[0].ToString();
            sideB_DD.Text = sideB_DD.Items[0].ToString();
        }

        void Search_E5071C_Folder(string gpib_command)
        {
            var rmSession = new ResourceManager();
            string file;
            string[] res = new string[1];
            string[] res2 = new string[0];
            string[] files = new string[0];
            string[] listbox = new string[100];
            string read_value1 = "";
            string read_value2 = "";
            try
            {
                //:MMEM:CAT?  Reads out the following information on the built-in storage device of the E5070B/E5071B.
                mbSession_E5071C = (MessageBasedSession)rmSession.Open(gpib_address_E5071C);
                mbSession_E5071C.TimeoutMilliseconds = 10;
                mbSession_E5071C.RawIO.Write(":MMEM:CAT? " + "\"" + gpib_command + "\"");
                Thread.Sleep(1);
                while (true)
                {
                    try
                    {
                        read_value2 = mbSession_E5071C.RawIO.ReadString();
                        read_value1 = read_value1 + read_value2;
                        if (read_value2.Length < 1024)
                        {
                            break;
                        }
                    }
                    catch
                    {
                        if (read_value1 != "")
                            break;
                    }
                }

                int i = 0;
                while (true)
                {
                    int end_pos = read_value1.IndexOf("\\,,0", i);
                    if (end_pos < 0)
                    {
                        break;
                    }
                    int start_pos = read_value1.LastIndexOf(",", end_pos) + 1;
                    file = read_value1.Substring(start_pos, end_pos - start_pos);
                    i = end_pos + 1;

                    res[0] = file;
                    files = new string[res2.Length + res.Length];
                    Array.Copy(res2, files, res2.Length);
                    Array.Copy(res, 0, files, res2.Length, res.Length);
                    res2 = files;
                }


                foreach (string folder_name in files)
                {
                    listBox1.Items.Add(folder_name);
                }

                mbSession_E5071C.Dispose();
            }
            catch (InvalidCastException)
            {
                MessageBox.Show("Resource selected must be a message-based session");
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
            }
        }
        private void listBox1_SelectedIndexChanged(object sender, System.EventArgs e)
        {

        }

        private void listBox1_doubleclick(object sender, EventArgs e)
        {
            string selection = listBox1.SelectedItem + "";
            string ENA_privious_folder_location;

            trigger_listBox1_doubleclick = true;

            if(ENA_lastest_folder_location == "D:\\CAMS_DEEMBEDD\\")
            {
                Case_type = "";
            }
            if (selection == "")
            {

            }
            else if (selection == "CONNECTOR")
            {
                Case_type = selection;
            }
            else if (selection == "CABLE")
            {
                Case_type = selection;
            }

            if (selection != "")
            {
                if (Case_type == "CONNECTOR")
                {
                    ENA_privious_folder_location = ENA_lastest_folder_location;
                    ENA_lastest_folder_location = ENA_lastest_folder_location + listBox1.SelectedItem + "\\";

                    listBox1.Items.Clear();
                    Search_E5071C_Folder(ENA_lastest_folder_location);
                    ENA_folder_location_history_i++;
                    ENA_folder_location_history[ENA_folder_location_history_i] = ENA_lastest_folder_location;

                    check_S2P_files(ENA_lastest_folder_location, Case_type);
                }
                else
                {
                    ENA_privious_folder_location = ENA_lastest_folder_location;
                    ENA_lastest_folder_location = ENA_lastest_folder_location + listBox1.SelectedItem + "\\";

                    listBox1.Items.Clear();
                    Search_E5071C_Folder(ENA_lastest_folder_location);
                    ENA_folder_location_history_i++;
                    ENA_folder_location_history[ENA_folder_location_history_i] = ENA_lastest_folder_location;

                    check_S2P_files(ENA_lastest_folder_location, Case_type);
                }
            }
        }
        private void check_S2P_files(string S2P_files_location, string type)
        {
            var rmSession = new ResourceManager();
            string file;
            string[] res = new string[1];
            string[] res2 = new string[0];
            string[] files = new string[0];
            string read_value1 = "";
            string read_value2 = "";
            string DeEmbedd_file = "";
            try
            {
                mbSession_E5071C = (MessageBasedSession)rmSession.Open(gpib_address_E5071C);
                mbSession_E5071C.TimeoutMilliseconds = 1;
                mbSession_E5071C.RawIO.Write(":MMEM:CAT? " + "\"" + S2P_files_location + "\"");
                Thread.Sleep(1);
                while (true)
                {
                    try
                    {
                        read_value2 = mbSession_E5071C.RawIO.ReadString();
                        read_value1 = read_value1 + read_value2;
                        if (read_value2.Length < 1024)
                        {
                            break;
                        }
                    }
                    catch
                    {
                        if (read_value1 != "")
                            break;
                    }
                }
                int i = 0;
                while (true)
                {
                    int end_pos = read_value1.IndexOf(".s2p", i);
                    if (end_pos < 0)
                    {
                        break;
                    }
                    int start_pos = read_value1.LastIndexOf(",", end_pos) + 1;
                    file = read_value1.Substring(start_pos, end_pos - start_pos) + ".s2p";
                    i = end_pos + 1;

                    res[0] = file;
                    files = new string[res2.Length + res.Length];
                    Array.Copy(res2, files, res2.Length);
                    Array.Copy(res, 0, files, res2.Length, res.Length);
                    res2 = files;
                }
                sideA_TX1.Items.Clear();
                sideA_RX1.Items.Clear();
                sideA_TX2.Items.Clear();
                sideA_RX2.Items.Clear();
                sideA_DD.Items.Clear();

                sideB_DD.Items.Clear();
                sideB_TX2.Items.Clear();
                sideB_RX2.Items.Clear();
                sideB_TX1.Items.Clear();
                sideB_RX1.Items.Clear();
                foreach (string file_name in files)
                {
                    listBox1.Items.Add(file_name);

                    if(file_name.Contains("_1"))
                    {
                        DeEmbedd_file = file_name.Replace("_1.s2p", "");

                        sideA_TX1.Items.Add(DeEmbedd_file);
                        sideA_RX2.Items.Add(DeEmbedd_file);
                        sideA_TX2.Items.Add(DeEmbedd_file);
                        sideA_RX1.Items.Add(DeEmbedd_file);
                        sideA_DD.Items.Add(DeEmbedd_file);
                        sideB_DD.Items.Add(DeEmbedd_file);
                        sideB_TX1.Items.Add(DeEmbedd_file);
                        sideB_RX2.Items.Add(DeEmbedd_file);
                        sideB_TX2.Items.Add(DeEmbedd_file);
                        sideB_RX1.Items.Add(DeEmbedd_file);

                        if (DeEmbedd_file.Contains("_T"))
                        {
                            if (DeEmbedd_file.Contains("sideA"))
                            {
                                sideA_TX1.Text = DeEmbedd_file;
                                sideA_RX2.Text = DeEmbedd_file;
                                sideA_DD.Text = DeEmbedd_file;
                            }
                            else if(DeEmbedd_file.Contains("sideB"))
                            {
                                if(type == "CABLE")
                                {
                                    sideB_DD.Text = DeEmbedd_file;
                                    sideB_TX1.Text = DeEmbedd_file;
                                    sideB_RX2.Text = DeEmbedd_file;
                                }
                                else if(type == "CONNECTOR")
                                {
                                    sideB_DD.Text = DeEmbedd_file;
                                    sideB_RX1.Text = DeEmbedd_file;
                                    sideB_TX2.Text = DeEmbedd_file;
                                }
                            }
                        }
                        else if (DeEmbedd_file.Contains("_B"))
                        {
                            if (DeEmbedd_file.Contains("sideA"))
                            {
                                sideA_TX2.Text = DeEmbedd_file;
                                sideA_RX1.Text = DeEmbedd_file;
                            }
                            else if (DeEmbedd_file.Contains("sideB"))
                            {
                                
                                if (type == "CABLE")
                                {
                                    sideB_RX1.Text = DeEmbedd_file;
                                    sideB_TX2.Text = DeEmbedd_file;
                                }
                                else if (type == "CONNECTOR")
                                {
                                    sideB_TX1.Text = DeEmbedd_file;
                                    sideB_RX2.Text = DeEmbedd_file;
                                }
                            }
                        }
                    }
                    
                }
                mbSession_E5071C.Dispose();
            }
            catch (InvalidCastException)
            {
                MessageBox.Show("Resource selected must be a message-based session");
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
            }
        }

        private void Previous_folder_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            sideA_TX1.Items.Clear();
            sideA_RX1.Items.Clear();
            sideA_TX2.Items.Clear();
            sideA_RX2.Items.Clear();
            sideA_DD.Items.Clear();

            sideB_DD.Items.Clear();
            sideB_TX2.Items.Clear();
            sideB_RX2.Items.Clear();
            sideB_TX1.Items.Clear();
            sideB_RX1.Items.Clear();
            ENA_folder_location_history_i--;
            if (ENA_folder_location_history_i < 0)
            {
                ENA_folder_location_history_i = 0;
            }
            ENA_lastest_folder_location = ENA_folder_location_history[ENA_folder_location_history_i];
            Search_E5071C_Folder(ENA_lastest_folder_location);
            //check_S2P_files(ENA_lastest_folder_location, Case_type);
        }

        private void Confirm_button_Click(object sender, EventArgs e)
        {
            Confirm_button.BackgroundImage = Properties.Resources.confirm_button_press;
            if(trigger_listBox1_doubleclick)
            {
                //在setting.ini檔案中，更新DeEmbedd的檔案位置
                iniManager.WriteIniFile("DeEmbedd", "sideA_TX1", ENA_lastest_folder_location + sideA_TX1.Text);
                iniManager.WriteIniFile("DeEmbedd", "sideA_RX1", ENA_lastest_folder_location + sideA_RX1.Text);
                iniManager.WriteIniFile("DeEmbedd", "sideA_TX2", ENA_lastest_folder_location + sideA_TX2.Text);
                iniManager.WriteIniFile("DeEmbedd", "sideA_RX2", ENA_lastest_folder_location + sideA_RX2.Text);
                iniManager.WriteIniFile("DeEmbedd", "sideA_DD", ENA_lastest_folder_location + sideA_DD.Text);

                iniManager.WriteIniFile("DeEmbedd", "sideB_RX1", ENA_lastest_folder_location + sideB_RX1.Text);
                iniManager.WriteIniFile("DeEmbedd", "sideB_TX1", ENA_lastest_folder_location + sideB_TX1.Text);
                iniManager.WriteIniFile("DeEmbedd", "sideB_RX2", ENA_lastest_folder_location + sideB_RX2.Text);
                iniManager.WriteIniFile("DeEmbedd", "sideB_TX2", ENA_lastest_folder_location + sideB_TX2.Text);
                iniManager.WriteIniFile("DeEmbedd", "sideB_DD", ENA_lastest_folder_location + sideB_DD.Text);
            }
            this.Close();
        }

        private void Confirm_button_mouseDown(object sender, MouseEventArgs e)
        {
            Confirm_button.BackgroundImage = Properties.Resources.confirm_button_press;
        }

        private void Confirm_button_mouseLeave(object sender, EventArgs e)
        {
            Confirm_button.BackgroundImage = Properties.Resources.confirm_button;
        }

        private void Confirm_button_mouseUp(object sender, MouseEventArgs e)
        {
            Confirm_button.BackgroundImage = Properties.Resources.confirm_button;
        }
        private void Confirm_button_mouseMove(object sender, MouseEventArgs e)
        {
            Confirm_button.BackgroundImage = Properties.Resources.confirm_button_press;
        }

        private void sideA_TX1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
