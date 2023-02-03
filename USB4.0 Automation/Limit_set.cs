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
using Excel = Microsoft.Office.Interop.Excel;
using System.Threading;

using System.Diagnostics;

namespace USB4._0_Automation
{

    public partial class Limit_set : Form
    {
        IniManager iniManager_LIMIT = new IniManager(System.Environment.CurrentDirectory + "\\ext_criteria.ini"); //設定ini檔路徑
        IniManager iniManager = new IniManager(System.Environment.CurrentDirectory + "\\" + "setting.ini"); //設定ini檔路徑
        private MessageBasedSession mbSession_E5071C;
        string gpib_address_E5071C = "";
        string ENA_lastest_folder_location;
        string[] ENA_folder_location_history = new string[20];
        string[] ENA_folder_location_record = new string[100];
        string lastResourceString = null;
        string now_time;
        string now_path = System.Windows.Forms.Application.StartupPath + "\\";
        string data_path = "";
        int ENA_folder_location_history_i = 0;
        int ENA_folder_location_record_i = 0;
        int s4p_files_num;
        int[] Executing_Config_EXCEL_PID = new int[100];
        int Executing_Config_EXCEL_PID_i = 0;
        double progressbar_complet_percent = 0;
        bool form_loading;

        string ILF_2_5G;
        string ILF_5G;
        string ILF_10G;
        string INEXT_IFEXT;
        string IDDXT;
        string IMR;
        string IRL;
        string SCD;
        string DD_50M;
        string DD_100M;
        string DD_200M;
        string DD_400M;

        string LIMIT_FILE;

        public Limit_set()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            mbSession_E5071C.RawIO.Write(":MMEM:CAT? " + "\"" + "D:\\CAMS_LIMITLINE\\"+ "\"");
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            

            ILF_2_5G = ILF_2_5G_tb.Text;
            ILF_5G = ILF_5G__tb.Text;
            ILF_10G = ILF_10G_tb.Text;
            INEXT_IFEXT = INEXT_IFEXT_tb.Text;
            IDDXT = IDDXT_tb.Text;
            IMR = IMR_tb.Text;
            IRL = IRL_tb.Text;
            SCD = SCD_tb.Text;
            DD_50M = DD_50M_tb.Text;
            DD_100M = DD_100M_tb.Text;
            DD_200M = DD_200M_tb.Text;
            DD_400M = DD_400M_tb.Text;

            iniManager_LIMIT.WriteIniFile("test_item", "ILfitatNq_db", "[-2.0, " + ILF_2_5G + ", " + ILF_5G + ", -8.5, " + ILF_10G + ", -20.0];");
            iniManager_LIMIT.WriteIniFile("test_item", "SS_NEXT_db", "[" + INEXT_IFEXT +"]");
            iniManager_LIMIT.WriteIniFile("test_item", "SS_FEXT_db", "[" + INEXT_IFEXT +"]");
            iniManager_LIMIT.WriteIniFile("test_item", "SS2DD_NEXT_db", "[" + IDDXT + "]");
            iniManager_LIMIT.WriteIniFile("test_item", "SS2DD_FEXT_db", "[" + IDDXT + "]");
            iniManager_LIMIT.WriteIniFile("test_item", "IMR_db", "[-28.936, -33.472, " + IMR + ", -39.9925, -41.41, -33.472]");
            iniManager_LIMIT.WriteIniFile("test_item", "IRL_db", "[-28.936, -33.472, " + IRL + ", -39.9925, -41.41, -33.472]");
            iniManager_LIMIT.WriteIniFile("test_item", "SS_MC_db", "["+ SCD +","+ SCD + ","+SCD+ "," + SCD + "," + SCD + "]");
            iniManager_LIMIT.WriteIniFile("test_item", "DD_IL_db", "[" + DD_50M + "," + DD_100M + "," + DD_200M + "," + DD_400M + "]");

            iniManager.WriteIniFile("LIMIT", "SS_PDI_LL", SS_PDI_LL.SelectedItem.ToString());
            iniManager.WriteIniFile("LIMIT", "SS_SEI_LL", SS_SEI_LL.SelectedItem.ToString());
            iniManager.WriteIniFile("LIMIT", "CI_LL", CI_LL.SelectedItem.ToString());
            iniManager.WriteIniFile("LIMIT", "SS_SK_L", SS_SK_L.SelectedItem.ToString());
            iniManager.WriteIniFile("LIMIT", "DD_DI_LL", DD_DI_LL.SelectedItem.ToString());
            iniManager.WriteIniFile("LIMIT", "DD_DL_L", DD_DL_L.SelectedItem.ToString());
            iniManager.WriteIniFile("LIMIT", "DD_PS_L", DD_PS_L.SelectedItem.ToString());


            this.Close();


        }

        private void Limit_set_Load(object sender, EventArgs e)
        {
            ILF_2_5G_tb.Text = "-4.0";
            ILF_5G__tb.Text = "-6.0";
            ILF_10G_tb.Text = "-11.0";
            INEXT_IFEXT_tb.Text = "-40";
            IDDXT_tb.Text = "-35";
            IMR_tb.Text = "-37.0";
            IRL_tb.Text = "-20.0";
            SCD_tb.Text = "-20.0";
            DD_50M_tb.Text = "-1.52";
            DD_100M_tb.Text = "-2.03";
            DD_200M_tb.Text = "-2.91";
            DD_400M_tb.Text = "-4.35";

           

            gpib_address_E5071C = iniManager.ReadIniFile("Session", "E5071C_Resource", "");
            ENA_folder_location_history[ENA_folder_location_history_i] = "D:\\CAMS_LIMITLINE\\";
            //ENA_folder_location_history[ENA_folder_location_history_i] = "D:\\CAMS_RESULT\\B2\\";
            ENA_lastest_folder_location = ENA_folder_location_history[ENA_folder_location_history_i];
            if (shareArea.Test_beginning != true)
            {
                Search_E5071C_Folder(ENA_lastest_folder_location);
            }
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
                    int end_pos = read_value1.IndexOf(",,", i);
                    //int start_po = read_value1.LastIndexOf(",",i );
                    if (end_pos < 0)
                    {
                        break;
                    }
                    int start_pos = read_value1.LastIndexOf(",", end_pos-1)+1 ;
                    file = read_value1.Substring(start_pos, end_pos - start_pos);
                    i = end_pos + 1;
                    
                    res[0] = file;
                    files = new string[res2.Length + res.Length];
                    Array.Copy(res2, files, res2.Length);
                    Array.Copy(res, 0, files, res2.Length, res.Length);
                    res2 = files;
                }

                if (SS_PDI_LL.Items.Count == 0)
                {
                    foreach (string folder_name in files)
                    {
                        SS_PDI_LL.Items.Add(folder_name);
                    }
                }
                else
                {
                    int j = 0;
                    foreach (string items in SS_PDI_LL.Items)
                    {
                        listbox[j] = items;
                        j++;
                    }
                    foreach (string folder_name in files)
                    {
                        SS_PDI_LL.Items.Add(folder_name);
                        foreach (string items in listbox)
                        {
                            if (folder_name == items)
                            {
                                SS_PDI_LL.Items.Remove(folder_name);
                            }
                        }
                    }
                }

                if (SS_SEI_LL.Items.Count == 0)
                {
                    foreach (string folder_name in files)
                    {
                        SS_SEI_LL.Items.Add(folder_name);
                    }
                }
                else
                {
                    int j = 0;
                    foreach (string items in SS_SEI_LL.Items)
                    {
                        listbox[j] = items;
                        j++;
                    }
                    foreach (string folder_name in files)
                    {
                        SS_SEI_LL.Items.Add(folder_name);
                        foreach (string items in listbox)
                        {
                            if (folder_name == items)
                            {
                                SS_SEI_LL.Items.Remove(folder_name);
                            }
                        }
                    }
                }

                if (CI_LL.Items.Count == 0)
                {
                    foreach (string folder_name in files)
                    {
                        CI_LL.Items.Add(folder_name);
                    }
                }
                else
                {
                    int j = 0;
                    foreach (string items in CI_LL.Items)
                    {
                        listbox[j] = items;
                        j++;
                    }
                    foreach (string folder_name in files)
                    {
                        CI_LL.Items.Add(folder_name);
                        foreach (string items in listbox)
                        {
                            if (folder_name == items)
                            {
                                CI_LL.Items.Remove(folder_name);
                            }
                        }
                    }
                }

                if (SS_SK_L.Items.Count == 0)
                {
                    foreach (string folder_name in files)
                    {
                        SS_SK_L.Items.Add(folder_name);
                    }
                }
                else
                {
                    int j = 0;
                    foreach (string items in SS_SK_L.Items)
                    {
                        listbox[j] = items;
                        j++;
                    }
                    foreach (string folder_name in files)
                    {
                        SS_SK_L.Items.Add(folder_name);
                        foreach (string items in listbox)
                        {
                            if (folder_name == items)
                            {
                                SS_SK_L.Items.Remove(folder_name);
                            }
                        }
                    }
                }

                if (DD_DI_LL.Items.Count == 0)
                {
                    foreach (string folder_name in files)
                    {
                        DD_DI_LL.Items.Add(folder_name);
                    }
                }
                else
                {
                    int j = 0;
                    foreach (string items in DD_DI_LL.Items)
                    {
                        listbox[j] = items;
                        j++;
                    }
                    foreach (string folder_name in files)
                    {
                        DD_DI_LL.Items.Add(folder_name);
                        foreach (string items in listbox)
                        {
                            if (folder_name == items)
                            {
                                DD_DI_LL.Items.Remove(folder_name);
                            }
                        }
                    }
                }

                if (DD_DL_L.Items.Count == 0)
                {
                    foreach (string folder_name in files)
                    {
                        DD_DL_L.Items.Add(folder_name);
                    }
                }
                else
                {
                    int j = 0;
                    foreach (string items in DD_DL_L.Items)
                    {
                        listbox[j] = items;
                        j++;
                    }
                    foreach (string folder_name in files)
                    {
                        DD_DL_L.Items.Add(folder_name);
                        foreach (string items in listbox)
                        {
                            if (folder_name == items)
                            {
                                DD_DL_L.Items.Remove(folder_name);
                            }
                        }
                    }
                }

                if (DD_PS_L.Items.Count == 0)
                {
                    foreach (string folder_name in files)
                    {
                        DD_PS_L.Items.Add(folder_name);
                    }
                }
                else
                {
                    int j = 0;
                    foreach (string items in DD_PS_L.Items)
                    {
                        listbox[j] = items;
                        j++;
                    }
                    foreach (string folder_name in files)
                    {
                        DD_PS_L.Items.Add(folder_name);
                        foreach (string items in listbox)
                        {
                            if (folder_name == items)
                            {
                                DD_PS_L.Items.Remove(folder_name);
                            }
                        }
                    }
                }
                //if (listBox1.Items.Count == 0)
                //{
                //    foreach (string folder_name in files)
                //    {
                //        listBox1.Items.Add(folder_name);
                //    }
                //}
                //else
                //{
                //    int j = 0;
                //    foreach (string items in listBox1.Items)
                //    {
                //        listbox[j] = items;
                //        j++;
                //    }
                //    foreach (string folder_name in files)
                //    {
                //        listBox1.Items.Add(folder_name);
                //        foreach (string items in listbox)
                //        {
                //            if (folder_name == items)
                //            {
                //                listBox1.Items.Remove(folder_name);
                //            }
                //        }
                //    }
                //}


                mbSession_E5071C.Dispose();
            }
            catch (InvalidCastException)
            {
                //Download.Enabled = false;
                if (!form_loading)
                    MessageBox.Show("Resource selected must be a message-based session");
            }
            catch (Exception exp)
            {

                //Download.Enabled = false;
                if (!form_loading)
                    MessageBox.Show(exp.Message);
            }
        }

        

        
    }
}
