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
using System.Threading;
using System.Text.RegularExpressions;
using System.IO;
using System.IO.Ports;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Management;


namespace USB4._0_Automation
{
    public partial class Main : Form
    {
        //全域變數-----------------------------------------------------------------
        Thread thread_for_e5071c_selectresource;
        Thread thread_for_sw_selectresource;
        Thread thread_for_checking_e5071c_connection;
        Thread thread_for_checking_sw_connection;
        private MessageBasedSession mbSession_E5071C;
        private MessageBasedSession mbSession_E5071C_tdr;
        string E5071C_Resource = "";
        string Switch_Resource = "";
        string lastResourceString = null;

        string test_item = "";
        int test_item_i = 0;
        string DP_DN_F;

        bool E5071C_connection_check_1st = true;
        bool SW_connection_check_1st = true;

        IniManager iniManager = new IniManager(System.Environment.CurrentDirectory + "\\" + "setting.ini"); //設定ini檔路徑
        IniManager iniRecord;

        DeEmbedd DeEmbedd_form = new DeEmbedd();
        calibration calibration_form = new calibration();
        Run_setting run_setting_form = new Run_setting();
        Limit_set Limit_form = new Limit_set();


        static int USB_Mode = 1;
        static int HDMI_Mode = 2;
        //int Test_Mode = USB_Mode;
        int Test_Mode = USB_Mode;

        Form4 form_4 = new Form4();
        Thread thread_for_Measure;
        /////////////////////////////////////////////////////////////////////////////

        public Main()
        {
            InitializeComponent();
            ///////////////////檢查本機時間是否被更改/////////////////////////
            //check_time();//檢查時間
            //DataStandardTime();
            if ((Program.nettime - Program.localtime) > 1800)
            {
                MessageBox.Show("Please correct your computer time. (Is it " + Program.nowtime + " ?)");
                System.Environment.Exit(0);
            }
            else if (Program.nettime - Program.localtime < -1800)
            {
                MessageBox.Show("Please correct your computer time. (Is it " + Program.nowtime + " ?)");
                System.Environment.Exit(0);
            }
            //////////////////////////////////////////////////////////////////
            ///
            //移動主視窗
            this.MouseDown += new MouseEventHandler(this.MouseDown1);
            this.MouseMove += new MouseEventHandler(this.MouseMove1);
        }

        //移動主視窗-----------------------------------------------------------------
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        [DllImport("user32.dll")]
        public static extern bool SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);
        public const int WM_SYSCOMMAND = 0X0112;
        public const int SC_MOVE = 0XF010;
        public const int HTCAPTION = 0X0002;

        //如果滑鼠位置在小於Y軸30的時候，按下滑鼠鍵即可移動整個UI介面
        private void MouseDown1(object sender, MouseEventArgs e)
        {
            if (e.Y >= 30)
                return;

            ReleaseCapture();
            SendMessage(this.Handle, WM_SYSCOMMAND, SC_MOVE + HTCAPTION, 0);
        }

        //滑鼠移動在視窗Y軸小於30且X軸小於800的位置時，會變換Cursor的圖示 Default --> SizeAll
        private void MouseMove1(object sender, MouseEventArgs e)
        {
            if (e.Y < 30 && e.X < 810)
            {
                this.Cursor = Cursors.SizeAll;
            }
            else
            {
                this.Cursor = Cursors.Default;
            }

        }
        /////////////////////////////////////////////////////////////////////////////

        private void Main_Load(object sender, EventArgs e)
        {
            Switch_Resource = iniManager.ReadIniFile("Session", "Switch_Resource", "");
            E5071C_Resource = iniManager.ReadIniFile("Session", "E5071C_Resource", "");
            shareArea.switch_resource = Switch_Resource;
            shareArea.e5071c_resource = E5071C_Resource;

            //設定Switch serialPort參數設定

            serialPort1.BaudRate = 9600;
            serialPort1.Parity = Parity.None;
            serialPort1.DataBits = 8;
            serialPort1.StopBits = StopBits.One;
            serialPort1.Encoding = Encoding.UTF8;
            serialPort1.ReadTimeout = 100;
            serialPort1.WriteTimeout = 100;

            //讀取ini檔中，[env_custom_set]的所有環境設定參數
            string env_start = iniManager.ReadIniFile("env_custom_set", "Start", "10M");
            string env_stop = iniManager.ReadIniFile("env_custom_set", "Stop", "20G");
            string env_point = iniManager.ReadIniFile("env_custom_set", "Point", "2000");
            string env_bandwidth = iniManager.ReadIniFile("env_custom_set", "Bandwidth", "10K");
            string env_power = iniManager.ReadIniFile("env_custom_set", "Power", "-5");

            //更改環境設定中客製的參數顯示
            env_custom_value.Text = env_start + ", " + env_stop + ", " + env_point + ", " + env_bandwidth + ", " + env_power;

            thread_for_checking_e5071c_connection = new Thread(Test_E5071C_session_Click);
            thread_for_checking_e5071c_connection.Start();
            thread_for_checking_sw_connection = new Thread(Test_switch_session_Click);
            thread_for_checking_sw_connection.Start();

            Tets_type_b3.Checked = true;

        }

        private void main_form_closing_Click_1(object sender, EventArgs e)
        {
            this.Close();
            Environment.Exit(Environment.ExitCode);
        }

        //Connection-----------------------------------------------------------------

        //打開SelectResource視窗選取Switch的Resource
        private void open_session_sw_Click(object sender, EventArgs e)
        {
            //先確認thread_for_e5071c_selectresource是否為null，如果是空的話建立一個新的，如果不是且還存在的話就把thread關閉
            if (thread_for_e5071c_selectresource == null)
            {
                thread_for_e5071c_selectresource = new Thread(e5071c_selectresource);
            }
            else
            {
                if (thread_for_e5071c_selectresource.IsAlive)
                {
                    thread_for_e5071c_selectresource.Abort();
                }
            }

            //確認thread_for_sw_selectresource是否為null，如果是空的話建立一個新的，如果不是就把存在的thread關閉再重新建立
            if (thread_for_sw_selectresource != null)
            {
                if (thread_for_sw_selectresource.IsAlive)
                {
                    thread_for_sw_selectresource.Abort();
                }
                thread_for_sw_selectresource = new Thread(sw_selectresource);
            }
            else
            {
                thread_for_sw_selectresource = new Thread(sw_selectresource);
            }

            thread_for_sw_selectresource.Start();

            open_session_sw.BackgroundImage = Properties.Resources.switch_button;
            open_session_e5071c.BackgroundImage = Properties.Resources.E5071C_Transparent;
            Fixture_board_button.BackgroundImage = Properties.Resources.Fixture_board_button;

        }
        void sw_selectresource()
        {
            //開啟SelectResource視窗，選擇連線Resource
            using (SelectResource resource = new SelectResource())
            {
                if (lastResourceString != null)
                {
                    resource.ResourceName = lastResourceString;
                }
                DialogResult result = resource.ShowDialog();
                if (result == DialogResult.OK)
                {
                    Switch_Resource = resource.ResourceName;
                    Cursor.Current = Cursors.WaitCursor;
                    Switch_Resource = "COM" + Regex.Replace(Switch_Resource, "[^0-9]", "");
                    shareArea.switch_resource = Switch_Resource;
                    Test_switch_session_Click();
                }
            }
        }

        //對Switch下指令，測試是否可以成功，成功的話將Resource寫入到settting.ini檔中
        private void Test_switch_session_Click()
        {
            string switch_command;
            switch_command = "1A,2A,3A,4A,";

            try
            {
                serialPort1.PortName = Switch_Resource;
                serialPort1.Open();

                serialPort1.DiscardInBuffer();
                serialPort1.Write(switch_command);

                open_session_sw.ImageIndex = 0; //如果有成功跑完前面的程式，代表SWITCH是有成功連線的，這時選擇成功連線的符號。
                serialPort1.Dispose();
                serialPort1.Close();

                iniManager.WriteIniFile("Session", "Switch_Resource", Switch_Resource);

            }
            catch (InvalidCastException)
            {
                open_session_sw.ImageIndex = 1; //如果跳到Exception時，代表失敗選擇失敗的連線符號。
                serialPort1.Close();
                if (!SW_connection_check_1st)
                    MessageBox.Show("Resource selected must be a message-based session");
            }
            catch (Exception exp)
            {
                open_session_sw.ImageIndex = 1; //如果跳到Exception時，代表失敗選擇失敗的連線符號。
                serialPort1.Close();
                if (!SW_connection_check_1st)
                    MessageBox.Show(exp.Message);
            }

            SW_connection_check_1st = false;
        }

        //打開SelectResource視窗選取E5071C的Resource
        private void open_session_e5071c_Click(object sender, EventArgs e)
        {
            //先確認thread_for_sw_selectresource是否為null，如果是空的話建立一個新的，如果不是且還存在的話就把thread關閉
            if (thread_for_sw_selectresource == null)
            {
                thread_for_sw_selectresource = new Thread(sw_selectresource);
            }
            else
            {
                if (thread_for_sw_selectresource.IsAlive)
                {
                    thread_for_sw_selectresource.Abort();
                }
            }

            //確認thread_for_e5071c_selectresource是否為null，如果是空的話建立一個新的，如果不是就把存在的thread關閉再重新建立
            if (thread_for_e5071c_selectresource != null)
            {
                if (thread_for_e5071c_selectresource.IsAlive)
                {
                    thread_for_e5071c_selectresource.Abort();
                }
                thread_for_e5071c_selectresource = new Thread(e5071c_selectresource);
            }
            else
            {
                thread_for_e5071c_selectresource = new Thread(e5071c_selectresource);
            }


            thread_for_e5071c_selectresource.Start();

            open_session_sw.BackgroundImage = Properties.Resources.switch_Transparent;
            open_session_e5071c.BackgroundImage = Properties.Resources.E5071C_button;
            Fixture_board_button.BackgroundImage = Properties.Resources.Fixture_board_button;
        }
        void e5071c_selectresource()
        {
            //開啟SelectResource視窗，選擇連線Resource
            using (SelectResource resource = new SelectResource())
            {
                if (lastResourceString != null)
                {
                    resource.ResourceName = lastResourceString;
                }
                DialogResult result = resource.ShowDialog();
                if (result == DialogResult.OK)
                {
                    //if(resource.ResourceName.Contains("MY46524666"))//resource.ResourceName.Contains("MY46524666")
                    //{
                    //    E5071C_Resource = resource.ResourceName;
                    //    Cursor.Current = Cursors.WaitCursor;
                    //    shareArea.e5071c_resource = E5071C_Resource;
                    //    Test_E5071C_session_Click();
                    //}
                    //else
                    //{
                    //    MessageBox.Show("ENA serial number is wrong, Please check your ENA number");
                    //}
                    E5071C_Resource = resource.ResourceName;
                    Cursor.Current = Cursors.WaitCursor;
                    shareArea.e5071c_resource = E5071C_Resource;
                    Test_E5071C_session_Click();
                }
            }
        }
        //對E5071C下指令，測試是否可以成功，成功的話將Resource寫入到settting.ini檔中
        private void Test_E5071C_session_Click()
        {
            var rmSession = new ResourceManager();
            string file;
            string[] res = new string[1];
            string[] res2 = new string[0];
            string[] files = new string[0];
            string read_value1 = "";
            string read_value2 = "";

            bool RESULT_folder_exist = false;
            bool DEEMBEDD_folder_exist = false;
            bool STA_folder_exist = false;
            bool TDR_STA_folder_exist = false;

            //確認ENA的D槽中是否有這些資料夾
            try
            {
                mbSession_E5071C = (MessageBasedSession)rmSession.Open(E5071C_Resource);
                mbSession_E5071C.TimeoutMilliseconds = 10;
                mbSession_E5071C.RawIO.Write(":MMEM:CAT? " + "\"D:\\\"");
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

                mbSession_E5071C.RawIO.Write(":SYST:BEEP:COMP:STAT OFF");                                       //2022-10-30 kevin Add BZ Off 
                mbSession_E5071C.RawIO.Write(":SYST:BEEP:WARN:STAT OFF");                                       //2022-10-30 kevin Add BZ Off

                foreach (string folder_name in files)
                {
                    if (folder_name.Contains("CAMS_RESULT"))
                        RESULT_folder_exist = true;
                    if (folder_name.Contains("CAMS_DEEMBEDD"))
                        DEEMBEDD_folder_exist = true;
                    if (folder_name.Contains("CAMS_STA"))
                        STA_folder_exist = true;
                    if (folder_name.Contains("CAMS_TDR_STA"))
                        TDR_STA_folder_exist = true;

                }

                if (!RESULT_folder_exist)
                    //mbSession_E5071C.RawIO.Write(":MMEM:MDIR \"D:\\CAMS_RESULT\\B8\"");
                    //mbSession_E5071C.RawIO.Write(":MMEM:MDIR \"D:\\CAMS_RESULT\\B2\"");
                    mbSession_E5071C.RawIO.Write(":MMEM:MDIR \"D:\\CAMS_RESULT\\B3\"");// 20221017 Add
                if (!DEEMBEDD_folder_exist)

                    mbSession_E5071C.RawIO.Write(":MMEM:MDIR \"D:\\CAMS_DEEMBEDD\"");
                mbSession_E5071C.RawIO.Write(":MMEM:MDIR \"D:\\CAMS_DEEMBEDD\\Cable\"");
                mbSession_E5071C.RawIO.Write(":MMEM:MDIR \"D:\\CAMS_DEEMBEDD\\Connector\"");
                //mbSession_E5071C.RawIO.Write(":MMEM:MDIR \"D:\\CAMS_DEEMBEDD\\B2\"");  //20220816_Sam
                mbSession_E5071C.RawIO.Write(":MMEM:MDIR \"D:\\CAMS_DEEMBEDD\\B3\"");  //20221017 Add

                if (!STA_folder_exist)
                    mbSession_E5071C.RawIO.Write(":MMEM:MDIR \"D:\\CAMS_STA\"");
                //mbSession_E5071C.RawIO.Write(":MMEM:MDIR \"D:\\CAMS_STA\\B2\"");
                mbSession_E5071C.RawIO.Write(":MMEM:MDIR \"D:\\CAMS_STA\\B3\""); //20221017 Add
                if (!TDR_STA_folder_exist)
                    mbSession_E5071C.RawIO.Write(":MMEM:MDIR \"D:\\CAMS_TDR_STA\"");

                mbSession_E5071C.RawIO.Write("*CLS");                                         //2022-10-30 kevin Add   clear error MSG
                mbSession_E5071C.RawIO.Write(":DISP:CCL");                                    //2022-10-30 kevin Add 

                mbSession_E5071C.Dispose();

                open_session_e5071c.ImageIndex = 0; //如果有成功跑完前面的程式，代表ENA是有成功連線的，這時選擇成功連線的符號。
                iniManager.WriteIniFile("Session", "E5071C_Resource", E5071C_Resource);


            }
            catch (InvalidCastException)
            {
                open_session_e5071c.ImageIndex = 1; //如果跳到Exception時，代表失敗選擇失敗的連線符號。
                if (!E5071C_connection_check_1st)
                    MessageBox.Show("Resource selected must be a message-based session");
            }
            catch (Exception exp)
            {
                open_session_e5071c.ImageIndex = 1; //如果跳到Exception時，代表失敗選擇失敗的連線符號。
                if (!E5071C_connection_check_1st)
                    MessageBox.Show(exp.Message);
            }

            E5071C_connection_check_1st = false;
        }
        /////////////////////////////////////////////////////////////////////////////

        //環境設定-----------------------------------------------------------------
        private void set_env_Click(object sender, EventArgs e)
        {

            E5071C_Resource = iniManager.ReadIniFile("Session", "E5071C_Resource", "");
            var rmSession = new ResourceManager();
            try
            {
                mbSession_E5071C = (MessageBasedSession)rmSession.Open(E5071C_Resource); //連線到E5071C
                try
                {
                    mbSession_E5071C_tdr = (MessageBasedSession)rmSession.Open("TCPIP0::127.0.0.1::inst0::INSTR");
                    try
                    {
                        thread_for_Measure = new Thread(Environment_setting);
                        thread_for_Measure.Start();

                        form_4 = new Form4();
                        form_4.ShowDialog();
                    }
                    catch (Exception exp)
                    {
                        if (mbSession_E5071C_tdr != null)
                        {
                            if (!mbSession_E5071C_tdr.IsDisposed)
                            {
                                mbSession_E5071C_tdr.Dispose();
                            }
                        }
                        MessageBox.Show(exp.Message);
                    }
                }
                catch (InvalidCastException)
                {
                    if (mbSession_E5071C != null)
                    {
                        if (!mbSession_E5071C.IsDisposed)
                        {
                            mbSession_E5071C.Dispose();
                        }
                    }
                    MessageBox.Show("Resource selected must be a message-based session");
                }
                catch (Exception exp)
                {
                    if (mbSession_E5071C != null)
                    {
                        if (!mbSession_E5071C.IsDisposed)
                        {
                            mbSession_E5071C.Dispose();
                        }
                    }
                    //MessageBox.Show(exp.Message);
                    MessageBox.Show("Please open ENA-TDR.exe or Restart");
                }
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
        /////////////////////////////////////////////////////////////////////////////
        //calibration-----------------------------------------------------------------
        private void calibration_button_Click(object sender, EventArgs e)
        {
            if (!shareArea.Test_beginning)
            {
                calibration_form.Close();
                DeEmbedd_form.Close();
                run_setting_form.Close();

                ////////////////////////////////20220816_Sam(待確認是否可以共用)
                calibration_form = new calibration();
                calibration_form.Show();
                ///////////////////////////////
            }
            else
            {
                MessageBox.Show("Test is run now, please wait");
            }
        }
        private void calibration_mouseDown(object sender, MouseEventArgs e)
        {
            calibration_button.BackgroundImage = Properties.Resources.button_blue_s_1;
        }

        private void calibration_mouseMove(object sender, MouseEventArgs e)
        {
            calibration_button.BackgroundImage = Properties.Resources.button_blue_s_1;
        }

        private void calibration_mouseLeave(object sender, EventArgs e)
        {
            calibration_button.BackgroundImage = Properties.Resources.button_blue_s;
        }

        private void calibration_mouseUp(object sender, MouseEventArgs e)
        {
            calibration_button.BackgroundImage = Properties.Resources.button_blue_s;
        }
        /////////////////////////////////////////////////////////////////////////////

        //DeEembedd-----------------------------------------------------------------
        private void DeEmbedd_button_Click(object sender, EventArgs e)
        {

            try
            {
                if (!shareArea.Test_beginning)
                {
                    DeEmbedd_form.Close();
                    calibration_form.Close();
                    run_setting_form.Close();
                    Limit_form.Close();
                    /////////////20220816_Sam(確認是否可以共同使用function)
                    DeEmbedd_form = new DeEmbedd();
                    DeEmbedd_form.Show();
                    //////////
                }
                else
                {
                    MessageBox.Show("Test is run now, please wait");
                }

            }
            catch
            {

            }
        }
        private void DeEmbedd_button_mouseDown(object sender, MouseEventArgs e)
        {
            DeEmbedd_button.BackgroundImage = Properties.Resources.button_blue_m_1;
        }
        private void DeEmbedd_button_mouseMove(object sender, MouseEventArgs e)
        {
            DeEmbedd_button.BackgroundImage = Properties.Resources.button_blue_m_1;
        }
        private void DeEmbedd_button_mouseLeave(object sender, EventArgs e)
        {
            DeEmbedd_button.BackgroundImage = Properties.Resources.button_blue_m;
        }
        private void DeEmbedd_button_mouseUp(object sender, MouseEventArgs e)
        {
            DeEmbedd_button.BackgroundImage = Properties.Resources.button_blue_m;
        }
        ////////////////////////////////////////////////////////////////////////////

        //按下Run按鍵-------------------------------------------------------------------
        private void Run_test_button_Click(object sender, EventArgs e)
        {
            if (Test_Mode == USB_Mode)
            {


                run_setting_form.Close();
                calibration_form.Close();
                DeEmbedd_form.Close();
                if (serialPort1.IsOpen)
                {
                    serialPort1.Dispose();
                    serialPort1.Close();
                }
                shareArea.Test_beginning = false;
                run_setting_form = new Run_setting();
                run_setting_form.Show();
            }


        }

        private void Run_test_mouseDown(object sender, MouseEventArgs e)
        {
            Run_test_button.BackgroundImage = Properties.Resources.button_yellow_m_1;
        }

        private void Run_test_mouseMove(object sender, MouseEventArgs e)
        {
            Run_test_button.BackgroundImage = Properties.Resources.button_yellow_m_1;
        }
        private void Run_test_mouseLeave(object sender, EventArgs e)
        {
            Run_test_button.BackgroundImage = Properties.Resources.button_yellow_m;
        }

        private void Run_test_mouseUp(object sender, MouseEventArgs e)
        {
            Run_test_button.BackgroundImage = Properties.Resources.button_yellow_m;
        }
        ///////////////////////////////////////////////////////////////////////////////////

        //測試環境設定------------------------------------------------------------------------------------------------
        public void Environment_setting()
        {
            long start_val;
            long stop_val;
            long point_val;
            long bandwidth_val;

            //mbSession_E5071C.RawIO.Write(":CALC2:FSIM:SEND:DEEM:PORT1 USER");
            //mbSession_E5071C.RawIO.Write(":CALC2:FSIM:SEND:DEEM:PORT2 USER");
            //mbSession_E5071C.RawIO.Write(":CALC2:FSIM:SEND:DEEM:PORT3 USER");
            //mbSession_E5071C.RawIO.Write(":CALC2:FSIM:SEND:DEEM:PORT4 USER");

            mbSession_E5071C.RawIO.Write(":CALC2:MARK1 ON");
            mbSession_E5071C.RawIO.Write(":CALC2:MARK1:X 2.5E9");
            mbSession_E5071C.RawIO.Write(":CALC2:MARK2 ON");
            mbSession_E5071C.RawIO.Write(":CALC2:MARK2:X 5E9");
            mbSession_E5071C.RawIO.Write(":CALC2:MARK3 ON");
            mbSession_E5071C.RawIO.Write(":CALC2:MARK3:X 1E10");
            mbSession_E5071C.RawIO.Write(":CALC2:MARK4 ON");
            mbSession_E5071C.RawIO.Write(":CALC2:MARK4:X 12.5E9");
            mbSession_E5071C.RawIO.Write(":CALC2:MARK5 ON");
            mbSession_E5071C.RawIO.Write(":CALC2:MARK5:X 15E9");

            mbSession_E5071C.RawIO.Write(":DISPlay:IMAGe INV");
            mbSession_E5071C.RawIO.Write(":INIT2:CONT ON");
            mbSession_E5071C.RawIO.Write(":TRIG:SOUR BUS");
            wait_done("*OPC?");

            //Start
            if (start_value.Text.Contains("G") || start_value.Text.Contains("g"))
            {
                start_val = Convert.ToInt32(Regex.Replace(start_value.Text, "[^0-9]", ""));
                start_val = start_val * 1000000000;
            }
            else if (start_value.Text.Contains("M") || start_value.Text.Contains("m"))
            {
                start_val = Convert.ToInt32(Regex.Replace(start_value.Text, "[^0-9]", ""));
                start_val = start_val * 1000000;
            }
            else
            {
                try
                {
                    start_val = Convert.ToInt32(Regex.Replace(start_value.Text, "[^0-9]", ""));
                }
                catch
                {
                    start_val = 10000000;
                }

            }
            /////////////////////////////////////////////////////////////////////

            //Stop
            if (stop_value.Text.Contains("G") || stop_value.Text.Contains("g"))
            {
                stop_val = Convert.ToInt32(Regex.Replace(stop_value.Text, "[^0-9]", ""));
                stop_val = stop_val * 1000000000;
            }
            else if (stop_value.Text.Contains("M") || stop_value.Text.Contains("m"))
            {
                stop_val = Convert.ToInt32(Regex.Replace(stop_value.Text, "[^0-9]", ""));
                stop_val = stop_val * 1000000;
            }
            else
            {
                try
                {
                    stop_val = Convert.ToInt32(Regex.Replace(stop_value.Text, "[^0-9]", ""));
                }
                catch
                {
                    if (Program.test_type == "B3")
                    {
                        stop_val = 15000000000;
                    }
                    else
                    {
                        stop_val = 20000000000;
                    }
                }
            }
            /////////////////////////////////////////////////////////////////////

            //Point
            if (point_value.Text.Contains("G") || point_value.Text.Contains("g"))
            {
                point_val = Convert.ToInt32(Regex.Replace(point_value.Text, "[^0-9]", ""));
                point_val = point_val * 1000000000;
            }
            else if (point_value.Text.Contains("M") || point_value.Text.Contains("m"))
            {
                point_val = Convert.ToInt32(Regex.Replace(point_value.Text, "[^0-9]", ""));
                point_val = point_val * 1000000;
            }
            else
            {
                try
                {
                    point_val = Convert.ToInt32(Regex.Replace(point_value.Text, "[^0-9]", ""));
                }
                catch
                {
                    point_val = 2000;
                    if (Program.test_type == "B3")
                    {
                        point_val = 1500;
                    }
                    else
                    {
                        point_val = 2000;
                    }
                }
            }
            /////////////////////////////////////////////////////////////////////

            //Bandwidth
            if (bandwidth_value.Text.Contains("G") || bandwidth_value.Text.Contains("g"))
            {
                bandwidth_val = Convert.ToInt32(Regex.Replace(bandwidth_value.Text, "[^0-9]", ""));
                bandwidth_val = bandwidth_val * 1000000000;
            }
            else if (bandwidth_value.Text.Contains("M") || bandwidth_value.Text.Contains("m"))
            {
                bandwidth_val = Convert.ToInt32(Regex.Replace(bandwidth_value.Text, "[^0-9]", ""));
                bandwidth_val = bandwidth_val * 1000000;
            }
            else if (bandwidth_value.Text.Contains("K") || bandwidth_value.Text.Contains("k"))
            {
                bandwidth_val = Convert.ToInt32(Regex.Replace(bandwidth_value.Text, "[^0-9]", ""));
                bandwidth_val = bandwidth_val * 1000;
            }
            else
            {
                try
                {
                    bandwidth_val = Convert.ToInt32(Regex.Replace(bandwidth_value.Text, "[^0-9]", ""));
                }
                catch
                {
                    bandwidth_val = 1000;
                }
            }

            //mbSession_E5071C.RawIO.Write("*RST");
            //wait_done("*OPC?");

            if (Program.test_type == "B3")//20221017 Add
            {
                try
                {
                    mbSession_E5071C.RawIO.Write("DIAG:PROG:TDR:STAT?");
                    String TDR_mode = mbSession_E5071C.RawIO.ReadString();
                    if (TDR_mode.Contains("1\n"))
                    {
                        TDR_ENV_SET();
                        //TDR_ENV_SET_DD();

                        mbSession_E5071C.RawIO.Write(":DISP:SPL D12");    //將畫面切割為左及右

                        mbSession_E5071C.RawIO.Write(":DISP:WIND1:ACT");  //設定左邊視窗為啟動的
                        mbSession_E5071C.RawIO.Write(":DISP:WIND1:MAX ON");  //將視窗最大化
                        wait_done("*OPC?");
                        mbSession_E5071C_tdr.RawIO.Write(":CALC1:ATR:ACT 1");  //設定左邊視窗TRACE1為啟動的
                        TDR_wait_done("*OPC?");

                        mbSession_E5071C.RawIO.Write(":DISP:WIND2:ACT");  //設定右邊視窗為啟動的
                        wait_done("*OPC?");

                        //mbSession_E5071C.RawIO.Write(":DISP:MAX ON");
                        mbSession_E5071C.RawIO.Write(":SENS2:FREQ:STAR " + start_val.ToString()); //Start   會去讀取文字檔當中的START數值
                        mbSession_E5071C.RawIO.Write(":SENS2:FREQ:STOP " + stop_val.ToString());  //Stop
                        mbSession_E5071C.RawIO.Write(":SENS2:SWE:POIN " + point_val.ToString());  //Point
                        mbSession_E5071C.RawIO.Write(":SENS2:BWID " + bandwidth_val.ToString());  //Bendwidth
                        mbSession_E5071C.RawIO.Write(":SOUR2:POW -5");                            //Power
                        wait_done("*OPC?");
                        //Trace1
                        mbSession_E5071C.RawIO.Write(":CALC2:FSIM:STAT ON");
                        mbSession_E5071C.RawIO.Write(":CALC2:FSIM:BAL:DEV BBAL");
                        mbSession_E5071C.RawIO.Write(":CALC2:FSIM:BAL:TOP:BBAL 1,3,2,4");
                        mbSession_E5071C.RawIO.Write(":CALC2:FSIM:BAL:PAR1:STAT ON");
                        mbSession_E5071C.RawIO.Write(":CALC2:FSIM:BAL:PAR1:BBAL SDD21");
                        wait_done("*OPC?");
                        //Trace2                           
                        mbSession_E5071C.RawIO.Write(":CALC2:PAR:COUN 2");
                        mbSession_E5071C.RawIO.Write(":CALC2:PAR2:SEL");
                        mbSession_E5071C.RawIO.Write(":CALC2:FSIM:STAT ON");
                        mbSession_E5071C.RawIO.Write(":CALC2:FSIM:BAL:DEV BBAL");
                        mbSession_E5071C.RawIO.Write(":CALC2:FSIM:BAL:TOP:BBAL 1,3,2,4");
                        mbSession_E5071C.RawIO.Write(":CALC2:FSIM:BAL:PAR2:STAT ON");
                        mbSession_E5071C.RawIO.Write(":CALC2:FSIM:BAL:PAR2:BBAL SDD12");
                        mbSession_E5071C.RawIO.Write(":CALC2:PAR1:SEL");
                        wait_done("*OPC?");

                        //kevin test -s
                        mbSession_E5071C.RawIO.Write(":CALC2:FSIM:EMB:STAT ON");
                        mbSession_E5071C.RawIO.Write(":CALC2:FSIM:SEND:DEEM:PORT1 USER");
                        mbSession_E5071C.RawIO.Write(":CALC2:FSIM:SEND:DEEM:PORT2 USER");
                        mbSession_E5071C.RawIO.Write(":CALC2:FSIM:SEND:DEEM:PORT3 USER");
                        mbSession_E5071C.RawIO.Write(":CALC2:FSIM:SEND:DEEM:PORT4 USER");

                        //mbSession_E5071C.RawIO.Write(":DISP:WIND2:ACT");
                        //mbSession_E5071C.RawIO.Write(":DISP:MAX ON");
                        //mbSession_E5071C.RawIO.Write(":CALC2:FSIM:SEND:DEEM:STAT ON");
                        //kevin test -e

                    }
                    else
                    {
                        MessageBox.Show("Please open TDR mode on ENA");
                    }

                    this.Invoke((MethodInvoker)delegate ()
                    {
                        mbSession_E5071C.Dispose();
                        mbSession_E5071C_tdr.Dispose();
                        form_4.Close();
                    });

                }
                catch (Exception exp)
                {
                    this.Invoke((MethodInvoker)delegate ()
                    {
                        mbSession_E5071C.Dispose();
                        mbSession_E5071C_tdr.Dispose();
                        form_4.Close();
                    });
                    MessageBox.Show(exp.Message);
                }

                //kevin test -s


                //kevin test -e
            }






        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////
        void TDR_ENV_SET()
        {

          check_DIF2_ENV();
            TDR_Query_response_value(":CALC1:ATR:MARK:COUP?", "0\n");
            TDR_Query_response_value(":CALC1:ATR:TIME:COUP?", "0\n");
            wait_done("*OPC?");
            mbSession_E5071C.RawIO.Write(":CALC1:FSIM:STAT ON");                                           //Analysis --> fixture simulator --> Fixture Simulator(ON)
            mbSession_E5071C.RawIO.Write(":CALC1:FSIM:BAL:DEV BBAL");                                      //Analysis --> fixture simulator --> Topology -> Device(Bal-Bal)
            mbSession_E5071C.RawIO.Write(":CALC1:FSIM:BAL:TOP:BBAL 1,3,2,4");                              //Analysis --> fixture simulator --> Topology --> Port1(1-3) & Port2(2-4) 
            wait_done("*OPC?");
            mbSession_E5071C.RawIO.Write(":CALC1:FSIM:BAL:PAR1:STAT ON");                                  //Analysis --> fixture simulator --> BalUn(OM)
            mbSession_E5071C.RawIO.Write(":CALC1:FSIM:BAL:PAR1:BBAL SDD21");


            wait_done("*OPC?");

            mbSession_E5071C.RawIO.Write(":CALC1:PAR:COUN 10");  //建了10個trace

            //確認Trace1和Trace5 是否為Tdd11 和 Tdd22

            //setting  Trace1 和 Trace5
            TDR_Query_response_value(":CALC1:TRAC1:PAR?", "\"Tdd11\"\n");                                   //TDR/TDT --> Parameters
            TDR_Query_response_value(":CALC1:TRAC1:FORM?", "IMP\n");                                        //TDR/TDT --> Parameters --> Format
            TDR_Query_response_value(":CALC1:TRAC5:PAR?", "\"Tdd22\"\n");                                   //TDR/TDT --> Parameters
            TDR_Query_response_value(":CALC1:TRAC5:FORM?", "IMP\n");                                        //TDR/TDT --> Parameters --> Format

            TDR_wait_done("*OPC?");

            //setting  Trace2 和 Trace6
            TDR_Query_response_value(":CALC1:TRAC2:PAR?", "\"T21\"\n");                                   //TDR/TDT --> Parameters
            TDR_Query_response_value(":CALC1:TRAC2:FORM?", "VOLT\n");                                        //TDR/TDT --> Parameters --> Format
            TDR_Query_response_value(":CALC1:TRAC6:PAR?", "\"T43\"\n");                                   //TDR/TDT --> Parameters                                      
            TDR_Query_response_value(":CALC1:TRAC6:FORM?", "VOLT\n");                                     //TDR/TDT --> Parameters --> Format

            TDR_wait_done("*OPC?");

            //setting  Trace3 和 Trace7
            TDR_Query_response_value(":CALC1:TRAC3:PAR?", "\"Tdd11\"\n");                                   //TDR/TDT --> Parameters
            TDR_Query_response_value(":CALC1:TRAC3:FORM?", "IMP\n");                                        //TDR/TDT --> Parameters --> Format
            TDR_Query_response_value(":CALC1:TRAC7:PAR?", "\"Tdd22\"\n");                                   //TDR/TDT --> Parameters
            TDR_Query_response_value(":CALC1:TRAC7:FORM?", "IMP\n");                                        //TDR/TDT --> Parameters --> Format

            TDR_wait_done("*OPC?");

            //setting  Trace4 和 Trace8
            TDR_Query_response_value(":CALC1:TRAC4:PAR?", "\"T11\"\n");                                   //TDR/TDT --> Parameters
            TDR_Query_response_value(":CALC1:TRAC4:FORM?", "IMP\n");                                        //TDR/TDT --> Parameters --> Format
            TDR_Query_response_value(":CALC1:TRAC8:PAR?", "\"T33\"\n");                                   //TDR/TDT --> Parameters
            TDR_Query_response_value(":CALC1:TRAC8:FORM?", "IMP\n");                                        //TDR/TDT --> Parameters --> Format


            wait_done("*OPC?");
            //setting  Trace9 和 Trace10
            TDR_Query_response_value(":CALC1:TRAC9:PAR?", "\"T22\"\n");                                   //TDR/TDT --> Parameters
            TDR_Query_response_value(":CALC1:TRAC9:FORM?", "IMP\n");                                        //TDR/TDT --> Parameters --> Format
            TDR_Query_response_value(":CALC1:TRAC10:PAR?", "\"T44\"\n");                                   //TDR/TDT --> Parameters
            TDR_Query_response_value(":CALC1:TRAC10:FORM?", "IMP\n");                                        //TDR/TDT --> Parameters --> Format

            wait_done("*OPC?");


            //Scal Trace1 和 Trace5
            mbSession_E5071C_tdr.RawIO.Write(":CALC1:TRAC1:TIME:STEP:RTIM:THR T1_9");
            mbSession_E5071C_tdr.RawIO.Write(":CALC1:TRAC1:TIME:STEP:RTIM:DATA 200e-12");
            mbSession_E5071C_tdr.RawIO.Write(":CALC1:TRAC5:TIME:STEP:RTIM:THR T1_9");
            mbSession_E5071C_tdr.RawIO.Write(":CALC1:TRAC5:TIME:STEP:RTIM:DATA 200e-12");
            TDR_wait_done("*OPC?");
            //Scal Trace2 和 Trace6
            mbSession_E5071C_tdr.RawIO.Write(":CALC1:TRAC2:TIME:STEP:RTIM:THR T1_9");
            mbSession_E5071C_tdr.RawIO.Write(":CALC1:TRAC2:TIME:STEP:RTIM:DATA 200e-12");
            mbSession_E5071C_tdr.RawIO.Write(":CALC1:TRAC6:TIME:STEP:RTIM:THR T1_9");
            mbSession_E5071C_tdr.RawIO.Write(":CALC1:TRAC6:TIME:STEP:RTIM:DATA 200e-12");
            TDR_wait_done("*OPC?");





            //Scal Trace3 和 Trace7
            mbSession_E5071C_tdr.RawIO.Write(":CALC1:TRAC3:TIME:STEP:RTIM:THR T2_8");
            mbSession_E5071C_tdr.RawIO.Write(":CALC1:TRAC3:TIME:STEP:RTIM:DATA 40e-12");
            mbSession_E5071C_tdr.RawIO.Write(":CALC1:TRAC7:TIME:STEP:RTIM:THR T2_8");
            mbSession_E5071C_tdr.RawIO.Write(":CALC1:TRAC7:TIME:STEP:RTIM:DATA 40e-12");
            TDR_wait_done("*OPC?");
            //Scal Trace4 和 Trace8
            mbSession_E5071C_tdr.RawIO.Write(":CALC1:TRAC4:TIME:STEP:RTIM:THR T1_9");
            mbSession_E5071C_tdr.RawIO.Write(":CALC1:TRAC4:TIME:STEP:RTIM:DATA 200e-12");
            mbSession_E5071C_tdr.RawIO.Write(":CALC1:TRAC8:TIME:STEP:RTIM:THR T1_9");
            mbSession_E5071C_tdr.RawIO.Write(":CALC1:TRAC8:TIME:STEP:RTIM:DATA 200e-12");

            TDR_wait_done("*OPC?");

            //Scal Trace9 和 Trace10
            mbSession_E5071C_tdr.RawIO.Write(":CALC1:TRAC9:TIME:STEP:RTIM:THR T1_9");
            mbSession_E5071C_tdr.RawIO.Write(":CALC1:TRAC9:TIME:STEP:RTIM:DATA 200e-12");
            mbSession_E5071C_tdr.RawIO.Write(":CALC1:TRAC10:TIME:STEP:RTIM:THR T1_9");
            mbSession_E5071C_tdr.RawIO.Write(":CALC1:TRAC10:TIME:STEP:RTIM:DATA 200e-12");
            TDR_wait_done("*OPC?");


            //Trace_1  DIV & STAR END
            mbSession_E5071C_tdr.RawIO.Write(":DISP:TRAC1:X:SCAL:PDIV 500e-12");                           //set value of x-axis scale per division
            mbSession_E5071C_tdr.RawIO.Write(":DISP:TRAC1:X:SCAL:RLEV -1e-9");                          //set value of x-axis reference line
            mbSession_E5071C_tdr.RawIO.Write(":DISP:TRAC1:Y:SCAL:PDIV 5");                                 //sets value of Y-axis scale per division
            mbSession_E5071C_tdr.RawIO.Write(":DISP:TRAC1:Y:SCAL:RLEV 65");
            TDR_wait_done("*OPC?");
            //Trace_5  DIV & STAR END
            mbSession_E5071C_tdr.RawIO.Write(":DISP:TRAC5:X:SCAL:PDIV 500e-12");                           //set value of x-axis scale per division
            mbSession_E5071C_tdr.RawIO.Write(":DISP:TRAC5:X:SCAL:RLEV -1e-9");                          //set value of x-axis reference line
            mbSession_E5071C_tdr.RawIO.Write(":DISP:TRAC5:Y:SCAL:PDIV 5");                                 //sets value of Y-axis scale per division
            mbSession_E5071C_tdr.RawIO.Write(":DISP:TRAC5:Y:SCAL:RLEV 65");
            TDR_wait_done("*OPC?");
            //Trace_2  DIV & STAR END
            mbSession_E5071C_tdr.RawIO.Write(":DISP:TRAC2:X:SCAL:PDIV 5e-9");                           //set value of x-axis scale per division
            mbSession_E5071C_tdr.RawIO.Write(":DISP:TRAC2:X:SCAL:RLEV -1e-9");                          //set value of x-axis reference line
            mbSession_E5071C_tdr.RawIO.Write(":DISP:TRAC2:Y:SCAL:PDIV 50e-3");                                 //sets value of Y-axis scale per division
            mbSession_E5071C_tdr.RawIO.Write(":DISP:TRAC2:Y:SCAL:RLEV 100e-3");
            TDR_wait_done("*OPC?");
            //Trace_6  DIV & STAR END
            mbSession_E5071C_tdr.RawIO.Write(":DISP:TRAC6:X:SCAL:PDIV 5e-9");                           //set value of x-axis scale per division
            mbSession_E5071C_tdr.RawIO.Write(":DISP:TRAC6:X:SCAL:RLEV -1e-9");                          //set value of x-axis reference line
            mbSession_E5071C_tdr.RawIO.Write(":DISP:TRAC6:Y:SCAL:PDIV 50e-3");                                 //sets value of Y-axis scale per division
            mbSession_E5071C_tdr.RawIO.Write(":DISP:TRAC6:Y:SCAL:RLEV 100e-3");
            TDR_wait_done("*OPC?");
            //Trace_3  DIV & STAR END
            mbSession_E5071C_tdr.RawIO.Write(":DISP:TRAC3:X:SCAL:PDIV 500e-12");                           //set value of x-axis scale per division
            mbSession_E5071C_tdr.RawIO.Write(":DISP:TRAC3:X:SCAL:RLEV -1e-9");                          //set value of x-axis reference line
            mbSession_E5071C_tdr.RawIO.Write(":DISP:TRAC3:Y:SCAL:PDIV 5");                                 //sets value of Y-axis scale per division
            mbSession_E5071C_tdr.RawIO.Write(":DISP:TRAC3:Y:SCAL:RLEV 65");
            TDR_wait_done("*OPC?");
            //Trace_7  DIV & STAR END
            mbSession_E5071C_tdr.RawIO.Write(":DISP:TRAC7:X:SCAL:PDIV 500e-12");                           //set value of x-axis scale per division
            mbSession_E5071C_tdr.RawIO.Write(":DISP:TRAC7:X:SCAL:RLEV -1e-9");                          //set value of x-axis reference line
            mbSession_E5071C_tdr.RawIO.Write(":DISP:TRAC7:Y:SCAL:PDIV 5");                                 //sets value of Y-axis scale per division
            mbSession_E5071C_tdr.RawIO.Write(":DISP:TRAC7:Y:SCAL:RLEV 65");

            TDR_wait_done("*OPC?");
            //Trace_4  DIV & STAR END
            mbSession_E5071C_tdr.RawIO.Write(":DISP:TRAC4:X:SCAL:PDIV 500e-12");                           //set value of x-axis scale per division
            mbSession_E5071C_tdr.RawIO.Write(":DISP:TRAC4:X:SCAL:RLEV -1e-9");                          //set value of x-axis reference line
            mbSession_E5071C_tdr.RawIO.Write(":DISP:TRAC4:Y:SCAL:PDIV 5");                                 //sets value of Y-axis scale per division
            mbSession_E5071C_tdr.RawIO.Write(":DISP:TRAC4:Y:SCAL:RLEV 20");
            TDR_wait_done("*OPC?");
            //Trace_8  DIV & STAR END
            mbSession_E5071C_tdr.RawIO.Write(":DISP:TRAC8:X:SCAL:PDIV 500e-12");                           //set value of x-axis scale per division
            mbSession_E5071C_tdr.RawIO.Write(":DISP:TRAC8:X:SCAL:RLEV -1e-9");                          //set value of x-axis reference line
            mbSession_E5071C_tdr.RawIO.Write(":DISP:TRAC8:Y:SCAL:PDIV 5");                                 //sets value of Y-axis scale per division
            mbSession_E5071C_tdr.RawIO.Write(":DISP:TRAC8:Y:SCAL:RLEV 20");

            TDR_wait_done("*OPC?");
            //Trace_9  DIV & STAR END
            mbSession_E5071C_tdr.RawIO.Write(":DISP:TRAC9:X:SCAL:PDIV 500e-12");                           //set value of x-axis scale per division
            mbSession_E5071C_tdr.RawIO.Write(":DISP:TRAC9:X:SCAL:RLEV -1e-9");                          //set value of x-axis reference line
            mbSession_E5071C_tdr.RawIO.Write(":DISP:TRAC9:Y:SCAL:PDIV 5");                                 //sets value of Y-axis scale per division
            mbSession_E5071C_tdr.RawIO.Write(":DISP:TRAC9:Y:SCAL:RLEV 20");

            TDR_wait_done("*OPC?");
            //Trace_10  DIV & STAR END
            mbSession_E5071C_tdr.RawIO.Write(":DISP:TRAC10:X:SCAL:PDIV 500e-12");                           //set value of x-axis scale per division
            mbSession_E5071C_tdr.RawIO.Write(":DISP:TRAC10:X:SCAL:RLEV -1e-9");                          //set value of x-axis reference line
            mbSession_E5071C_tdr.RawIO.Write(":DISP:TRAC10:Y:SCAL:PDIV 5");                                 //sets value of Y-axis scale per division
            mbSession_E5071C_tdr.RawIO.Write(":DISP:TRAC10:Y:SCAL:RLEV 20");

            TDR_wait_done("*OPC?");


            // Off BZ
            mbSession_E5071C.RawIO.Write(":SYST:BEEP:COMP:STAT OFF");
            mbSession_E5071C.RawIO.Write(":SYST:BEEP:WARN:STAT OFF");
            //off BZ


            //KEVIN TEST _2023_1/18_22:30  -S

            //MARK1 Setting     
            //mbSession_E5071C.RawIO.Write(":CALC1:MARK1 ON");  //ok
            //mbSession_E5071C.RawIO.Write(":CALC1:MARK1:FUNC:TYPE MAX");    //ok                               //2022/09/21 Kevin add for --->	這一行是將 MARK1 設定為 MAX
            //mbSession_E5071C.RawIO.Write(":CALC1:TRAC1:MARK1:FUNC:TRAC ON");                                     //2022/09/21 Kevin add for --->	這一行是將MARK1設定為追蹤
            //mbSession_E5071C.RawIO.Write(":CALC1:MARK:FUNC:DOM ON");                                       //2022/09/21 Kevin add for --->	這一行是設定Search Range(ON)
            //wait_done("*OPC?");

            //Trace1  maker ON AND 設定Maker
            //mbSession_E5071C.RawIO.Write(":CALC1:MARK1:FUNC:TRAC ON");
            //mbSession_E5071C_tdr.RawIO.Write(":CALC1:MARK:FUNC:DOM ON");
            //mbSession_E5071C_tdr.RawIO.Write(":CALC1:MARK:FUNC:DOM:STAR 1.5e-9");
            //mbSession_E5071C_tdr.RawIO.Write(":CALC1:MARK:FUNC:DOM:STOP 3e-9");
            //MARK2 Setting
            //mbSession_E5071C.RawIO.Write(":CALC1:MARK2 ON");  //ok
            //mbSession_E5071C.RawIO.Write(":CALC1:MARK2:FUNC:TYPE MIN");    //ok                               //2022/09/21 Kevin add for --->	這一行是將 MARK1 設定為 MAX
            //mbSession_E5071C.RawIO.Write(":CALC1:TRAC1:MARK1:FUNC:TRAC ON");                                     //2022/09/21 Kevin add for --->	這一行是將MARK1設定為追蹤
            //mbSession_E5071C.RawIO.Write(":CALC1:MARK:FUNC:DOM ON");                                       //2022/09/21 Kevin add for --->	這一行是設定Search Range(ON)

            wait_done("*OPC?");

            ////





            //Tr2、Tr6 「SKEW」
            mbSession_E5071C.RawIO.Write(":DISP:WIND1:ACT");
            mbSession_E5071C.RawIO.Write(":DISP:WIND1:MAX OFF");                                           //2022/09/21 Kevin add for ---> 這段是設定起始為 Tr2
            wait_done("*OPC?");
            mbSession_E5071C_tdr.RawIO.Write(":CALC:ATR:ACT 2");
            mbSession_E5071C_tdr.RawIO.Write(":CALC:TRAC2:DTIM:TARG 6");
            mbSession_E5071C_tdr.RawIO.Write(":CALC:TRAC2:DTIM:STAT ON");                                     //2022/09/21 Kevin add for ---> 這一段是設定 Tr2 及 Tr6 作相減

            TDR_wait_done("*OPC?");
            mbSession_E5071C_tdr.RawIO.Write(":CALC:TRAC2:DTIM:POS 50");                                        //2022/09/21 Kevin add for ---> 這一段是設定 position 為50
            mbSession_E5071C_tdr.RawIO.Write(":TRIG:SING");                                                     //2022/09/21 Kevin add for --->	
            mbSession_E5071C_tdr.RawIO.Write(":CALC:TRAC2:DTIM:DATA?");                                         //2022/09/21 Kevin add for --->	//:CALCulate:TRACe{Tr}:DTIMe:DATA  //This command gets delta time result value. You can get the result even if :CALCulate:TRACe{Tr}:DTIMe:STATe is off.
            TDR_wait_done("*OPC?");


            mbSession_E5071C.RawIO.Write(":DISP:WIND1:MAX ON");
            mbSession_E5071C_tdr.RawIO.Write(":CALC:ATR:ACT 3");
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC3:MARK1 ON");
            mbSession_E5071C.RawIO.Write(":CALC1:MARK:FUNC:DOM OFF");                                          //確認一下需要關嗎?
            TDR_wait_done("*OPC?");
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC3:MARK1:FUNC:TRAC ON");                                   //MKR/ANALYSIS --> Marker Search --> Track(on)
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC3:MARK1:FUNC:TYPE TARG");                                 //MKR/ANALYSIS --> Marker Search --> Search Target
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC3:MARK1:FUNC:TARG 200e-3");                               //MKR/ANALYSIS --> Marker Search --> Target value
            wait_done("*OPC?");

            mbSession_E5071C.RawIO.Write(":SYST:BEEP:COMP:STAT OFF");
            mbSession_E5071C.RawIO.Write(":SYST:BEEP:WARN:STAT OFF");


            mbSession_E5071C_tdr.RawIO.Write(":CALC:ATR:ACT 2");
            mbSession_E5071C_tdr.RawIO.Write(":DISP:TRAC2:X:SCAL:RLEV -3e-9");
            TDR_wait_done("*OPC?");

            //mbSession_E5071C.RawIO.Write(":DISP:WIND1:MAX ON");
            mbSession_E5071C.RawIO.Write(":CALC1:SELected:EQUation:STATE ON");
            mbSession_E5071C.RawIO.Write(":CALC1:SELected:EQUation:TEXT \"S21-S41\"");
            wait_done("*OPC?");

            mbSession_E5071C_tdr.RawIO.Write(":CALC:ATR:ACT 6");
            mbSession_E5071C_tdr.RawIO.Write(":DISP:TRAC6:X:SCAL:RLEV -3e-9");
            TDR_wait_done("*OPC?");

            mbSession_E5071C.RawIO.Write(":CALC1:SELected:EQUation:STATE ON");
            mbSession_E5071C.RawIO.Write(":CALC1:SELected:EQUation:TEXT \"S43-S23\"");
            wait_done("*OPC?");

            mbSession_E5071C_tdr.RawIO.Write(":CALC:TRAC2:DTIM:TARG 6");
            mbSession_E5071C_tdr.RawIO.Write(":CALC:TRAC2:DTIM:STAT ON");

            TDR_wait_done("*OPC?");

            mbSession_E5071C.RawIO.Write(":DISP:WIND1:TRAC1:Y:RPOS 0");                                    //Position of reference division line, Response --> Scale --> Reference Position
            mbSession_E5071C.RawIO.Write(":DISP:WIND1:TRAC5:Y:RPOS 0");                                    //Position of reference division line, Response --> Scale --> Reference Position
            wait_done("*OPC?");

            // Off BZ
            mbSession_E5071C.RawIO.Write(":SYST:BEEP:COMP:STAT OFF");
            mbSession_E5071C.RawIO.Write(":SYST:BEEP:WARN:STAT OFF");
            //off BZ


            // Tr1、Tr5
            mbSession_E5071C.RawIO.Write(":CALC1:PAR1:SEL");
            mbSession_E5071C.RawIO.Write(":DISP:WIND1:MAX ON");
            mbSession_E5071C.RawIO.Write(":MMEM:LOAD:LIM \"D:\\CAMS_Limitline\\IMP_LIMIT_SS_TR15.csv\"");                                                                                //Analysis > Limit Test > Edit Limit Line > Import from CSV File
            mbSession_E5071C.RawIO.Write(":CALC1:LIM ON");                                                                                                                       //Analysis > Limit Test > Limit Test
            mbSession_E5071C.RawIO.Write(":CALC1:LIM:DISP ON");                                                                                                                  //Analysis > Limit Test > Limit Line
            mbSession_E5071C.RawIO.Write(":DISP:FSIG OFF");                                                                                                                       //Analysis > Limit Test > Fail Sign
            wait_done("*OPC?");


            //mbSession_E5071C_tdr.RawIO.Write(":DISP: WIND1: MAX OFF");                                    //切換到視窗縮小
            //mbSession_E5071C_tdr.RawIO.Write(":CALC1: PAR1: SEL");                                        //選擇視窗1 的TRACE 5
            //mbSession_E5071C_tdr.RawIO.Write(":CALC1:MARK:FUNC:DOM ON");
            //mbSession_E5071C_tdr.RawIO.Write(":CALC1:MARK:FUNC:DOM:STAR 1.5e-9");
            //mbSession_E5071C_tdr.RawIO.Write(":CALC1:MARK:FUNC:DOM:STOP 3e-9");





            //TRACE 1 MutiSearch MAX
            mbSession_E5071C.RawIO.Write(":CALC1:PAR1:SEL");
            TDR_wait_done("*OPC?");
            mbSession_E5071C_tdr.RawIO.Write(":CALC1:MARK1:ACT");
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC1:MARK1 ON");
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC1:MARK1:FUNC:TYPE MAX");
            wait_done("*OPC?");
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC1:MARK1:FUNC:DOM:MULT ON");
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC1:MARK1:FUNC:DOM:MULT:RANG 1");
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC1:MARK1:FUNC:DOM:MULT:STAR 1,1.5e-9");
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC1:MARK1:FUNC:DOM:MULT:STOP 1,3e-9");
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC1:MARK1:FUNC:TRAC ON");
            mbSession_E5071C.RawIO.Write(":CALC1:MARK:FUNC:DOM ON");
            mbSession_E5071C.RawIO.Write(":CALC1:MARK1:FUNC:EXEC");
            wait_done("*OPC?");

            //TRACE 1 MutiSearch Min
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC1:MARK2 ON");
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC1:MARK2:FUNC:TYPE MIN");
            wait_done("*OPC?");
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC1:MARK2:FUNC:DOM:MULT ON");
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC1:MARK2:FUNC:DOM:MULT:RANG 1");
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC1:MARK2:FUNC:DOM:MULT:STAR 1,1.5e-9");
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC1:MARK2:FUNC:DOM:MULT:STOP 1,3e-9");
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC1:MARK2:FUNC:TRAC ON");
            mbSession_E5071C.RawIO.Write(":CALC1:MARK:FUNC:DOM ON");
            mbSession_E5071C.RawIO.Write(":CALC1:MARK2:FUNC:EXEC");
            wait_done("*OPC?");


            //TRACE 4 MutiSearch MAX
            mbSession_E5071C.RawIO.Write(":CALC1:PAR4:SEL");
            TDR_wait_done("*OPC?");
            mbSession_E5071C_tdr.RawIO.Write(":CALC1:MARK1:ACT");
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC4:MARK1 ON");
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC4:MARK1:FUNC:TYPE MAX");
            wait_done("*OPC?");
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC4:MARK1:FUNC:DOM:MULT ON");
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC4:MARK1:FUNC:DOM:MULT:RANG 1");
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC4:MARK1:FUNC:DOM:MULT:STAR 1,1.5e-9");
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC4:MARK1:FUNC:DOM:MULT:STOP 1,3e-9");
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC4:MARK1:FUNC:TRAC ON");
            mbSession_E5071C.RawIO.Write(":CALC1:MARK:FUNC:DOM ON");
            mbSession_E5071C.RawIO.Write(":CALC1:MARK1:FUNC:EXEC");
            wait_done("*OPC?");

            //TRACE 4 MutiSearch Min
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC4:MARK2 ON");
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC4:MARK2:FUNC:TYPE MIN");
            wait_done("*OPC?");
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC4:MARK2:FUNC:DOM:MULT ON");
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC4:MARK2:FUNC:DOM:MULT:RANG 1");
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC4:MARK2:FUNC:DOM:MULT:STAR 1,1.5e-9");
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC4:MARK2:FUNC:DOM:MULT:STOP 1,3e-9");
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC4:MARK2:FUNC:TRAC ON");
            mbSession_E5071C.RawIO.Write(":CALC1:MARK:FUNC:DOM ON");
            mbSession_E5071C.RawIO.Write(":CALC1:MARK2:FUNC:EXEC");
            wait_done("*OPC?");


            //TRACE 5 MutiSearch MAX
            mbSession_E5071C.RawIO.Write(":CALC1:PAR5:SEL");
            TDR_wait_done("*OPC?");
            mbSession_E5071C_tdr.RawIO.Write(":CALC1:MARK1:ACT");
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC5:MARK1 ON");
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC5:MARK1:FUNC:TYPE MAX");
            wait_done("*OPC?");
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC5:MARK1:FUNC:DOM:MULT ON");
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC5:MARK1:FUNC:DOM:MULT:RANG 1");
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC5:MARK1:FUNC:DOM:MULT:STAR 1,1.5e-9");
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC5:MARK1:FUNC:DOM:MULT:STOP 1,3e-9");
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC5:MARK1:FUNC:TRAC ON");
            mbSession_E5071C.RawIO.Write(":CALC1:MARK:FUNC:DOM ON");
            mbSession_E5071C.RawIO.Write(":CALC1:MARK1:FUNC:EXEC");
            wait_done("*OPC?");

            //TRACE 5 MutiSearch Min
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC5:MARK2 ON");
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC5:MARK2:FUNC:TYPE MIN");
            wait_done("*OPC?");
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC5:MARK2:FUNC:DOM:MULT ON");
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC5:MARK2:FUNC:DOM:MULT:RANG 1");
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC5:MARK2:FUNC:DOM:MULT:STAR 1,1.5e-9");
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC5:MARK2:FUNC:DOM:MULT:STOP 1,3e-9");
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC5:MARK2:FUNC:TRAC ON");
            mbSession_E5071C.RawIO.Write(":CALC1:MARK:FUNC:DOM ON");
            mbSession_E5071C.RawIO.Write(":CALC1:MARK2:FUNC:EXEC");
            wait_done("*OPC?");


            //TRACE 8 MutiSearch MAX
            mbSession_E5071C.RawIO.Write(":CALC1:PAR8:SEL");
            TDR_wait_done("*OPC?");
            mbSession_E5071C_tdr.RawIO.Write(":CALC1:MARK1:ACT");
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC8:MARK1 ON");
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC8:MARK1:FUNC:TYPE MAX");
            wait_done("*OPC?");
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC8:MARK1:FUNC:DOM:MULT ON");
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC8:MARK1:FUNC:DOM:MULT:RANG 1");
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC8:MARK1:FUNC:DOM:MULT:STAR 1,1.5e-9");
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC8:MARK1:FUNC:DOM:MULT:STOP 1,3e-9");
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC8:MARK1:FUNC:TRAC ON");
            mbSession_E5071C.RawIO.Write(":CALC1:MARK:FUNC:DOM ON");
            mbSession_E5071C.RawIO.Write(":CALC1:MARK1:FUNC:EXEC");
            wait_done("*OPC?");

            //TRACE 8 MutiSearch Min
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC8:MARK2 ON");
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC8:MARK2:FUNC:TYPE MIN");
            wait_done("*OPC?");
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC8:MARK2:FUNC:DOM:MULT ON");
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC8:MARK2:FUNC:DOM:MULT:RANG 1");
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC8:MARK2:FUNC:DOM:MULT:STAR 1,1.5e-9");
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC8:MARK2:FUNC:DOM:MULT:STOP 1,3e-9");
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC8:MARK2:FUNC:TRAC ON");
            mbSession_E5071C.RawIO.Write(":CALC1:MARK:FUNC:DOM ON");
            mbSession_E5071C.RawIO.Write(":CALC1:MARK2:FUNC:EXEC");
            wait_done("*OPC?");


            //TRACE 9 MutiSearch MAX
            mbSession_E5071C.RawIO.Write(":CALC1:PAR9:SEL");
            TDR_wait_done("*OPC?");
            mbSession_E5071C_tdr.RawIO.Write(":CALC1:MARK1:ACT");
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC9:MARK1 ON");
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC9:MARK1:FUNC:TYPE MAX");
            wait_done("*OPC?");
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC9:MARK1:FUNC:DOM:MULT ON");
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC9:MARK1:FUNC:DOM:MULT:RANG 1");
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC9:MARK1:FUNC:DOM:MULT:STAR 1,1.5e-9");
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC9:MARK1:FUNC:DOM:MULT:STOP 1,3e-9");
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC9:MARK1:FUNC:TRAC ON");
            mbSession_E5071C.RawIO.Write(":CALC1:MARK:FUNC:DOM ON");
            mbSession_E5071C.RawIO.Write(":CALC1:MARK1:FUNC:EXEC");
            wait_done("*OPC?");

            //TRACE 9 MutiSearch Min
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC9:MARK2 ON");
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC9:MARK2:FUNC:TYPE MIN");
            wait_done("*OPC?");
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC9:MARK2:FUNC:DOM:MULT ON");
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC9:MARK2:FUNC:DOM:MULT:RANG 1");
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC9:MARK2:FUNC:DOM:MULT:STAR 1,1.5e-9");
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC9:MARK2:FUNC:DOM:MULT:STOP 1,3e-9");
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC9:MARK2:FUNC:TRAC ON");
            mbSession_E5071C.RawIO.Write(":CALC1:MARK:FUNC:DOM ON");
            mbSession_E5071C.RawIO.Write(":CALC1:MARK2:FUNC:EXEC");
            wait_done("*OPC?");


            //TRACE 10 MutiSearch MAX
            mbSession_E5071C.RawIO.Write(":CALC1:PAR10:SEL");
            TDR_wait_done("*OPC?");
            mbSession_E5071C_tdr.RawIO.Write(":CALC1:MARK1:ACT");
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC10:MARK1 ON");
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC10:MARK1:FUNC:TYPE MAX");
            wait_done("*OPC?");
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC10:MARK1:FUNC:DOM:MULT ON");
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC10:MARK1:FUNC:DOM:MULT:RANG 1");
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC10:MARK1:FUNC:DOM:MULT:STAR 1,1.5e-9");
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC10:MARK1:FUNC:DOM:MULT:STOP 1,3e-9");
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC10:MARK1:FUNC:TRAC ON");
            mbSession_E5071C.RawIO.Write(":CALC1:MARK:FUNC:DOM ON");
            mbSession_E5071C.RawIO.Write(":CALC1:MARK1:FUNC:EXEC");
            wait_done("*OPC?");

            //TRACE 10 MutiSearch Min
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC10:MARK2 ON");
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC10:MARK2:FUNC:TYPE MIN");
            wait_done("*OPC?");
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC10:MARK2:FUNC:DOM:MULT ON");
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC10:MARK2:FUNC:DOM:MULT:RANG 1");
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC10:MARK2:FUNC:DOM:MULT:STAR 1,1.5e-9");
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC10:MARK2:FUNC:DOM:MULT:STOP 1,3e-9");
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC10:MARK2:FUNC:TRAC ON");
            mbSession_E5071C.RawIO.Write(":CALC1:MARK:FUNC:DOM ON");
            mbSession_E5071C.RawIO.Write(":CALC1:MARK2:FUNC:EXEC");
            wait_done("*OPC?");


            //TRACE 3 MutiSearch MAX
            mbSession_E5071C.RawIO.Write(":CALC1:PAR3:SEL");
            TDR_wait_done("*OPC?");
            mbSession_E5071C_tdr.RawIO.Write(":CALC1:MARK1:ACT");
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC3:MARK1 ON");
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC3:MARK1:FUNC:TYPE MAX");
            wait_done("*OPC?");
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC3:MARK1:FUNC:DOM:MULT ON");
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC3:MARK1:FUNC:DOM:MULT:RANG 1");
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC3:MARK1:FUNC:DOM:MULT:STAR 1,520e-12");
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC3:MARK1:FUNC:DOM:MULT:STOP 1,850e-12");
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC3:MARK1:FUNC:TRAC ON");
            mbSession_E5071C.RawIO.Write(":CALC1:MARK:FUNC:DOM ON");
            mbSession_E5071C.RawIO.Write(":CALC1:MARK1:FUNC:EXEC");
            wait_done("*OPC?");

            //TRACE 3 MutiSearch Min
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC3:MARK2 ON");
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC3:MARK2:FUNC:TYPE MIN");
            wait_done("*OPC?");
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC3:MARK2:FUNC:DOM:MULT ON");
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC3:MARK2:FUNC:DOM:MULT:RANG 1");
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC3:MARK2:FUNC:DOM:MULT:STAR 1,520e-12");
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC3:MARK2:FUNC:DOM:MULT:STOP 1,850e-12");
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC3:MARK2:FUNC:TRAC ON");
            mbSession_E5071C.RawIO.Write(":CALC1:MARK:FUNC:DOM ON");
            mbSession_E5071C.RawIO.Write(":CALC1:MARK2:FUNC:EXEC");
            wait_done("*OPC?");



            //TRACE 7 MutiSearch MAX
            mbSession_E5071C.RawIO.Write(":CALC1:PAR7:SEL");
            TDR_wait_done("*OPC?");
            mbSession_E5071C_tdr.RawIO.Write(":CALC1:MARK1:ACT");
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC7:MARK1 ON");
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC7:MARK1:FUNC:TYPE MAX");
            wait_done("*OPC?");
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC7:MARK1:FUNC:DOM:MULT ON");
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC7:MARK1:FUNC:DOM:MULT:RANG 1");
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC7:MARK1:FUNC:DOM:MULT:STAR 1,520e-12");
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC7:MARK1:FUNC:DOM:MULT:STOP 1,850e-12");
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC7:MARK1:FUNC:TRAC ON");
            mbSession_E5071C.RawIO.Write(":CALC1:MARK:FUNC:DOM ON");
            mbSession_E5071C.RawIO.Write(":CALC1:MARK1:FUNC:EXEC");
            wait_done("*OPC?");

            //TRACE 7 MutiSearch Min
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC7:MARK2 ON");
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC7:MARK2:FUNC:TYPE MIN");
            wait_done("*OPC?");
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC7:MARK2:FUNC:DOM:MULT ON");
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC7:MARK2:FUNC:DOM:MULT:RANG 1");
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC7:MARK2:FUNC:DOM:MULT:STAR 1,1.5e-9");
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC7:MARK2:FUNC:DOM:MULT:STOP 1,3e-9");
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC7:MARK2:FUNC:TRAC ON");
            mbSession_E5071C.RawIO.Write(":CALC1:MARK:FUNC:DOM ON");
            mbSession_E5071C.RawIO.Write(":CALC1:MARK2:FUNC:EXEC");
            wait_done("*OPC?");







            mbSession_E5071C.RawIO.Write(":SYST:BEEP:COMP:STAT OFF");  // BZ  Off
            mbSession_E5071C.RawIO.Write(":SYST:BEEP:WARN:STAT OFF");  // WAR OFF


            mbSession_E5071C.RawIO.Write(":CALC1:PAR5:SEL");
            mbSession_E5071C.RawIO.Write(":DISP:WIND1:MAX ON");
            mbSession_E5071C.RawIO.Write(":MMEM:LOAD:LIM \"D:\\CAMS_Limitline\\IMP_LIMIT_SS_TR15.csv\"");                                                                                     //Analysis > Limit Test > Edit Limit Line > Import from CSV File
            mbSession_E5071C.RawIO.Write(":CALC1:LIM ON");                                                                                                                       //Analysis > Limit Test > Limit Test
            mbSession_E5071C.RawIO.Write(":CALC1:LIM:DISP ON");                                                                                                                  //Analysis > Limit Test > Limit Line
            mbSession_E5071C.RawIO.Write(":DISP:FSIG OFF");
            wait_done("*OPC?");

            // Tr3、Tr7
            mbSession_E5071C.RawIO.Write(":CALC1:PAR3:SEL");
            mbSession_E5071C.RawIO.Write(":DISP:WIND1:MAX ON");
            mbSession_E5071C.RawIO.Write(":MMEM:LOAD:LIM \"D:\\CAMS_Limitline\\IMP_LIMIT_SS_TR37.csv\"");                                                                                //Analysis > Limit Test > Edit Limit Line > Import from CSV File
            mbSession_E5071C.RawIO.Write(":CALC1:LIM ON");                                                                                                                       //Analysis > Limit Test > Limit Test
            mbSession_E5071C.RawIO.Write(":CALC1:LIM:DISP ON");                                                                                                                  //Analysis > Limit Test > Limit Line
            mbSession_E5071C.RawIO.Write(":DISP:FSIG OFF");                                                                                                                       //Analysis > Limit Test > Fail Sign
            wait_done("*OPC?");

            mbSession_E5071C.RawIO.Write(":CALC1:PAR7:SEL");
            mbSession_E5071C.RawIO.Write(":DISP:WIND1:MAX ON");
            mbSession_E5071C.RawIO.Write(":MMEM:LOAD:LIM \"D:\\CAMS_Limitline\\IMP_LIMIT_SS_TR37.csv\"");                                                                                     //Analysis > Limit Test > Edit Limit Line > Import from CSV File
            mbSession_E5071C.RawIO.Write(":CALC1:LIM ON");                                                                                                                       //Analysis > Limit Test > Limit Test
            mbSession_E5071C.RawIO.Write(":CALC1:LIM:DISP ON");                                                                                                                  //Analysis > Limit Test > Limit Line
            mbSession_E5071C.RawIO.Write(":DISP:FSIG OFF");
            wait_done("*OPC?");

            // Tr4、Tr8、Tr9、Tr10
            mbSession_E5071C.RawIO.Write(":CALC1:PAR4:SEL");
            mbSession_E5071C.RawIO.Write(":DISP:WIND1:MAX ON");
            mbSession_E5071C.RawIO.Write(":MMEM:LOAD:LIM \"D:\\CAMS_Limitline\\IMP_LIMIT_SS_TR48910.csv\"");                                                                                //Analysis > Limit Test > Edit Limit Line > Import from CSV File
            mbSession_E5071C.RawIO.Write(":CALC1:LIM ON");                                                                                                                       //Analysis > Limit Test > Limit Test
            mbSession_E5071C.RawIO.Write(":CALC1:LIM:DISP ON");                                                                                                                  //Analysis > Limit Test > Limit Line
            mbSession_E5071C.RawIO.Write(":DISP:FSIG OFF");                                                                                                                       //Analysis > Limit Test > Fail Sign
            wait_done("*OPC?");

            mbSession_E5071C.RawIO.Write(":CALC1:PAR8:SEL");
            mbSession_E5071C.RawIO.Write(":DISP:WIND1:MAX ON");
            mbSession_E5071C.RawIO.Write(":MMEM:LOAD:LIM \"D:\\CAMS_Limitline\\IMP_LIMIT_SS_TR48910.csv\"");                                                                                     //Analysis > Limit Test > Edit Limit Line > Import from CSV File
            mbSession_E5071C.RawIO.Write(":CALC1:LIM ON");                                                                                                                       //Analysis > Limit Test > Limit Test
            mbSession_E5071C.RawIO.Write(":CALC1:LIM:DISP ON");                                                                                                                  //Analysis > Limit Test > Limit Line
            mbSession_E5071C.RawIO.Write(":DISP:FSIG OFF");
            wait_done("*OPC?");

            mbSession_E5071C.RawIO.Write(":CALC1:PAR9:SEL");
            mbSession_E5071C.RawIO.Write(":DISP:WIND1:MAX ON");
            mbSession_E5071C.RawIO.Write(":MMEM:LOAD:LIM \"D:\\CAMS_Limitline\\IMP_LIMIT_SS_TR48910.csv\"");                                                                                     //Analysis > Limit Test > Edit Limit Line > Import from CSV File
            mbSession_E5071C.RawIO.Write(":CALC1:LIM ON");                                                                                                                       //Analysis > Limit Test > Limit Test
            mbSession_E5071C.RawIO.Write(":CALC1:LIM:DISP ON");                                                                                                                  //Analysis > Limit Test > Limit Line
            mbSession_E5071C.RawIO.Write(":DISP:FSIG OFF");
            wait_done("*OPC?");

            mbSession_E5071C.RawIO.Write(":CALC1:PAR10:SEL");
            mbSession_E5071C.RawIO.Write(":DISP:WIND1:MAX ON");
            mbSession_E5071C.RawIO.Write(":MMEM:LOAD:LIM \"D:\\CAMS_Limitline\\IMP_LIMIT_SS_TR48910.csv\"");                                                                                     //Analysis > Limit Test > Edit Limit Line > Import from CSV File
            mbSession_E5071C.RawIO.Write(":CALC1:LIM ON");                                                                                                                       //Analysis > Limit Test > Limit Test
            mbSession_E5071C.RawIO.Write(":CALC1:LIM:DISP ON");                                                                                                                  //Analysis > Limit Test > Limit Line
            mbSession_E5071C.RawIO.Write(":DISP:FSIG OFF");
            wait_done("*OPC?");
            TDR_wait_done("*OPC?");
        }
       
      

        void check_DIF2_ENV()
        {
            string DUT_Topology;
            string Trace1_parameter;
            string Trace5_parameter;
            string Trace3_parameter;
            string Trace7_parameter;
            mbSession_E5071C_tdr.RawIO.Write(":CALC1:DEV?");
            DUT_Topology = mbSession_E5071C_tdr.RawIO.ReadString();
            mbSession_E5071C.RawIO.Write("DISP:WIND:TRAC1:LAB:PAR?");
            Trace1_parameter = mbSession_E5071C.RawIO.ReadString();
            mbSession_E5071C.RawIO.Write("DISP:WIND:TRAC5:LAB:PAR?");
            Trace5_parameter = mbSession_E5071C.RawIO.ReadString();
            mbSession_E5071C.RawIO.Write("DISP:WIND:TRAC3:LAB:PAR?");
            Trace3_parameter = mbSession_E5071C.RawIO.ReadString();
            mbSession_E5071C.RawIO.Write("DISP:WIND:TRAC7:LAB:PAR?");
            Trace7_parameter = mbSession_E5071C.RawIO.ReadString();

            if (DUT_Topology.ToUpper() != "DIF2\n")
            {
                mbSession_E5071C_tdr.RawIO.Write(":CALC1:DEV DIF2");
                TDR_wait_done("*OPC?");
            }
            else if (!(Trace1_parameter == "\"Tdd11\"\n" || Trace1_parameter == "\"Tcc11\"\n"))
            {
                mbSession_E5071C_tdr.RawIO.Write(":CALC1:DEV DIF2");
                TDR_wait_done("*OPC?");
            }
            else if (!(Trace5_parameter == "\"Tdd22\"\n" || Trace5_parameter == "\"Tcc22\"\n"))
            {
                mbSession_E5071C_tdr.RawIO.Write(":CALC1:DEV DIF2");
                TDR_wait_done("*OPC?");
            }
            else if (!(Trace3_parameter == "\"Tdd21\"\n" || Trace3_parameter == "\"T21\"\n"))
            {
                mbSession_E5071C_tdr.RawIO.Write(":CALC1:DEV DIF2");
                TDR_wait_done("*OPC?");
            }
            else if (!(Trace7_parameter == "\"Tdd12\"\n" || Trace7_parameter == "\"T43\"\n"))
            {
                mbSession_E5071C_tdr.RawIO.Write(":CALC1:DEV DIF2");
                TDR_wait_done("*OPC?");
            }

        }

        void TDR_Query_response_value(string command, string parameter)
        {
            string value;
            mbSession_E5071C_tdr.RawIO.Write(command);
            value = mbSession_E5071C_tdr.RawIO.ReadString();
            if (value != parameter)
            {
                command = command.Replace("?", " " + parameter.Replace("\n", ""));
                mbSession_E5071C_tdr.RawIO.Write(command);
                TDR_wait_done("*OPC?");
            }
        }
        void TDR_wait_done(string gpib_command)
        {
            string read_value;

            if (gpib_command != "")
            {
                try
                {
                    mbSession_E5071C_tdr.RawIO.Write(gpib_command);
                    while (true)
                    {
                        try
                        {
                            read_value = mbSession_E5071C_tdr.RawIO.ReadString();
                            break;
                        }
                        catch
                        {
                            read_value = "";
                        }
                    }
                }
                catch (Exception exp)
                {
                    MessageBox.Show("*OPC Command Error");
                }
            }
        }


        private void USB_tab_Click(object sender, EventArgs e)
        {
            Test_Mode = USB_Mode;
            groupBox2.BringToFront();
            USB_tab.BringToFront();
            USB_tab.BackgroundImage = Properties.Resources.USB_TAB;

        }

        private void env_default_value1_Click(object sender, EventArgs e)
        {
            start_value.Text = "10M";
            stop_value.Text = "20G";
            point_value.Text = "2000";
            bandwidth_value.Text = "20K";
            power_value.Text = "-5";
        }
        private void env_default_value2_Click(object sender, EventArgs e)
        {
            start_value.Text = "10M";
            stop_value.Text = "20G";
            point_value.Text = "2000";
            bandwidth_value.Text = "1K";
            power_value.Text = "-5";
        }
        private void Fixture_board_button_Click(object sender, EventArgs e)
        {
            if (thread_for_e5071c_selectresource == null)
            {
                thread_for_e5071c_selectresource = new Thread(e5071c_selectresource);
            }
            else
            {
                if (thread_for_e5071c_selectresource.IsAlive)
                    thread_for_e5071c_selectresource.Abort();
            }

            if (thread_for_sw_selectresource == null)
            {
                thread_for_sw_selectresource = new Thread(sw_selectresource);
            }
            else
            {
                if (thread_for_sw_selectresource.IsAlive)
                    thread_for_sw_selectresource.Abort();
            }

            open_session_sw.BackgroundImage = Properties.Resources.switch_Transparent;
            open_session_e5071c.BackgroundImage = Properties.Resources.E5071C_Transparent;
            Fixture_board_button.BackgroundImage = Properties.Resources.Fixture_board_button_press;

            diagram lForm = new diagram();
            lForm.Owner = this;
            lForm.ShowDialog();
        }
        private void env_custom_value_Click(object sender, EventArgs e)
        {
            //藉由讀取env_custom_value.text，分別切割出start_value、stop_value、point_value、bandwidth_value、power_value
            string[] env_values = (env_custom_value.Text).Replace(" ", "").Split(',');
            start_value.Text = env_values[0];
            stop_value.Text = env_values[1];
            point_value.Text = env_values[2];
            bandwidth_value.Text = env_values[3];
            power_value.Text = env_values[4];
        }

        private void env_custom_set_Click(object sender, EventArgs e)
        {

            //在setting.ini檔案中，寫入更新的數值
            iniManager.WriteIniFile("env_custom_set", "Start", start_value.Text);
            iniManager.WriteIniFile("env_custom_set", "Stop", stop_value.Text);
            iniManager.WriteIniFile("env_custom_set", "Point", point_value.Text);
            iniManager.WriteIniFile("env_custom_set", "Bandwidth", bandwidth_value.Text);
            iniManager.WriteIniFile("env_custom_set", "Power", power_value.Text);

            //在setting.ini檔案中，讀取更新的數值
            string env_start = iniManager.ReadIniFile("env_custom_set", "Start", "10M");
            string env_stop = iniManager.ReadIniFile("env_custom_set", "Stop", "20G");
            string env_point = iniManager.ReadIniFile("env_custom_set", "Point", "2000");
            string env_bandwidth = iniManager.ReadIniFile("env_custom_set", "Bandwidth", "10K");
            string env_power = iniManager.ReadIniFile("env_custom_set", "Power", "-5");

            //更改環境設定中客製的參數顯示
            env_custom_value.Text = env_start + ", " + env_stop + ", " + env_point + ", " + env_bandwidth + ", " + env_power;
        }

        private void switch_control_Click(object sender, EventArgs e)
        {
            switch_channel_A.Visible = true;
            switch_channel_B.Visible = true;
            switch_channel_C.Visible = true;
            switch_channel_D.Visible = true;
            switch_channel_E.Visible = true;
            switch_channel_F.Visible = true;
        }

        private void switch_channel_A_CheckedChanged(object sender, EventArgs e)
        {
            if (!serialPort1.IsOpen)
            {
                serialPort1.Open(); //連線到Switch
            }
            serialPort1.Write("1A,3A,2A,4A,");
            serialPort1.Close();
        }

        private void switch_channel_B_CheckedChanged(object sender, EventArgs e)
        {
            if (!serialPort1.IsOpen)
            {
                serialPort1.Open(); //連線到Switch
            }
            serialPort1.Write("1B,3B,2B,4B,");
            serialPort1.Close();
        }

        private void switch_channel_C_CheckedChanged(object sender, EventArgs e)
        {
            if (!serialPort1.IsOpen)
            {
                serialPort1.Open(); //連線到Switch
            }
            serialPort1.Write("1C,3C,2C,4C,");
            serialPort1.Close();
        }

        private void switch_channel_D_CheckedChanged(object sender, EventArgs e)
        {
            if (!serialPort1.IsOpen)
            {
                serialPort1.Open(); //連線到Switch
            }
            serialPort1.Write("1D,3D,2D,4D,");
            serialPort1.Close();
        }

        private void switch_channel_E_CheckedChanged(object sender, EventArgs e)
        {
            if (!serialPort1.IsOpen)
            {
                serialPort1.Open(); //連線到Switch
            }
            serialPort1.Write("1E,3E,2E,4E,");
            serialPort1.Close();
        }

        private void switch_channel_F_CheckedChanged(object sender, EventArgs e)
        {
            if (!serialPort1.IsOpen)
            {
                serialPort1.Open(); //連線到Switch
            }
            serialPort1.Write("1F,3F,2F,4F,");
            serialPort1.Close();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            
            if (open_session_sw.ImageIndex == 0 && open_session_e5071c.ImageIndex == 0)//20221215
            {
                set_env.Visible = true;
                calibration_button.Visible = true;
                Run_test_button.Visible = true;
                switch_control.Visible = true;
                button3.Visible = true;

                if (Test_Mode == HDMI_Mode)
                {
                    DeEmbedd_button.Visible = false;
                }
                else
                {
                    DeEmbedd_button.Visible = true;
                }
            }
            else
            {
                set_env.Visible = false;
                calibration_button.Visible = false;
                DeEmbedd_button.Visible = false;
                Run_test_button.Visible = false;
                switch_control.Visible = false;
                button3.Visible = false;//LIMIT SET
            }
        }

        private void Form_Minimized_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }


        void wait_done(string gpib_command)
        {
            string read_value;

            if (gpib_command != "")
            {
                try
                {
                    mbSession_E5071C.RawIO.Write(gpib_command);
                    while (true)
                    {
                        try
                        {
                            read_value = mbSession_E5071C.RawIO.ReadString();
                            break;
                        }
                        catch
                        {
                            read_value = "";
                        }
                    }
                }
                catch (Exception exp)
                {
                    MessageBox.Show("*OPC Command Error");
                }
            }
        }

        private void TDR_Cal_button_Click(object sender, EventArgs e)
        {
            //USB_TDR_calibration_form = new Form6();
            //calibration_form.Owner = this;
            //USB_TDR_calibration_form.Show();
        }


        //檢查時間
        private void check_time()
        {

            Int32 unixTimestamp = (Int32)(DateTime.Now.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            //MessageBox.Show(unixTimestamp);
            Console.WriteLine("Unix Timestamp is {0}.", unixTimestamp);

            Program.localtime = unixTimestamp;
        }


        public static DateTime DataStandardTime()//使用時，將static 關鍵字刪除，在其它位置方可使用?2010-11-24
        {//返回國際標準時間
            //只使用的TimerServer的IP地址，未使用域名
            string[,] TimerServer = new string[14, 2];
            int[] ServerTab = new int[] { 3, 2, 4, 8, 9, 6, 11, 5, 10, 0, 1, 7, 12 };

            TimerServer[0, 0] = "time-a.nist.gov";
            TimerServer[0, 1] = "129.6.15.28";
            TimerServer[1, 0] = "time-b.nist.gov";
            TimerServer[1, 1] = "129.6.15.29";
            TimerServer[2, 0] = "time-a.timefreq.bldrdoc.gov";
            TimerServer[2, 1] = "132.163.4.101";
            TimerServer[3, 0] = "time-b.timefreq.bldrdoc.gov";
            TimerServer[3, 1] = "132.163.4.102";
            TimerServer[4, 0] = "time-c.timefreq.bldrdoc.gov";
            TimerServer[4, 1] = "132.163.4.103";
            TimerServer[5, 0] = "utcnist.colorado.edu";
            TimerServer[5, 1] = "128.138.140.44";
            TimerServer[6, 0] = "time.nist.gov";
            TimerServer[6, 1] = "192.43.244.18";
            TimerServer[7, 0] = "time-nw.nist.gov";
            TimerServer[7, 1] = "131.107.1.10";
            TimerServer[8, 0] = "nist1.symmetricom.com";
            TimerServer[8, 1] = "69.25.96.13";
            TimerServer[9, 0] = "nist1-dc.glassey.com";
            TimerServer[9, 1] = "216.200.93.8";
            TimerServer[10, 0] = "nist1-ny.glassey.com";
            TimerServer[10, 1] = "208.184.49.9";
            TimerServer[11, 0] = "nist1-sj.glassey.com";
            TimerServer[11, 1] = "207.126.98.204";
            TimerServer[12, 0] = "nist1.aol-ca.truetime.com";
            TimerServer[12, 1] = "207.200.81.113";
            TimerServer[13, 0] = "nist1.aol-va.truetime.com";
            TimerServer[13, 1] = "64.236.96.53";
            int portNum = 13;
            string hostName;
            byte[] bytes = new byte[1024];
            int bytesRead = 0;
            System.Net.Sockets.TcpClient client = new System.Net.Sockets.TcpClient();
            for (int i = 0; i < 13; i++)
            {
                hostName = TimerServer[ServerTab[i], 0];

                Debug.WriteLine("hostName:" + hostName);
                try
                {
                    client.Connect(hostName, portNum);

                    System.Net.Sockets.NetworkStream ns = client.GetStream();
                    bytesRead = ns.Read(bytes, 0, bytes.Length);
                    client.Close();
                    break;
                }
                catch (System.Exception)
                {
                    Debug.WriteLine("錯誤！");
                }
            }
            char[] sp = new char[1];
            sp[0] = ' ';
            System.DateTime dt = new DateTime();
            string str1;
            str1 = System.Text.Encoding.ASCII.GetString(bytes, 0, bytesRead);
            Debug.WriteLine("ntp time:" + str1);

            string[] s;
            s = str1.Split(sp);
            if (s[0] == "")
            {
                MessageBox.Show("Please connect Internet and restart program");
                System.Environment.Exit(0);
            }
            dt = System.DateTime.Parse(s[1] + " " + s[2]);//得到標準時間
            Debug.WriteLine("get:" + dt.ToString());
            dt = dt.AddHours(8);//得到北京時間*/
            Program.nowtime = dt;
            DateTime PreviousDateTime = new DateTime(1970, 1, 1);
            var stringSecsNow = (dt - PreviousDateTime).TotalSeconds.ToString();

            //dt = dt.AddYears(-1970);
            //dt = dt.AddMonths(-1);
            //dt = dt.AddDays(-1);
            //dt = dt.TotalSeconds;
            Console.WriteLine("The Unix Timestamp is {0}.", stringSecsNow);
            Program.nettime = Convert.ToInt32(stringSecsNow);
            return dt;

        }

        private void timer2_Tick(object sender, EventArgs e)
        {

            if (Tets_type_b3.Checked)
            {
                //switch_channel_A.Enabled = true;
                //switch_channel_B.Enabled = true;
                //switch_channel_C.Enabled = true;
                //label13.BringToFront();
                //label14.BringToFront();
                //groupBox6.BringToFront();
                //groupBox7.BringToFront();
                //groupBox6.Visible = true;
                //groupBox7.Visible = true;
                //label13.Visible = true;
                //label14.Visible = true;

                //Full_Featured_rbt.Enabled = false;
                //Charging_Through_rbt.Enabled = false;
                //oneM.Enabled = false;
                //twoM.Enabled = false;
                //threeM.Enabled = false;
                Program.test_type = "B3";


            }
            else
            {


                switch_channel_A.Enabled = true;
                switch_channel_B.Enabled = true;
                switch_channel_C.Enabled = true;


                //把勾選銷

                Program.test_type = "B8";
            }


        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Full_Featured_rbt_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void timer3_Tick(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            DeEmbedd_form.Close();
            calibration_form.Close();
            run_setting_form.Close();
            Limit_form.Close();
            /////////////20220816_Sam(確認是否可以共同使用function)
            Limit_form = new Limit_set();
            Limit_form.Show();
            //////////
            //try
            //{
            //    if (!shareArea.Test_beginning)
            //    {
            //        DeEmbedd_form.Close();
            //        calibration_form.Close();
            //        run_setting_form.Close();
            //        Limit_form.Close();
            //        /////////////20220816_Sam(確認是否可以共同使用function)
            //        Limit_form = new Limit_set;
            //        Limit_form.Show();
            //        //////////
            //    }
            //    else
            //    {
            //        MessageBox.Show("Test is run now, please wait");
            //    }

            //}
            //catch
            //{

            //}
        }
    }

    //視窗共用變數-----------------------------------------------------------------
    public class shareArea
    {
        public static string switch_resource;
        public static string e5071c_resource;
        public static string Report_case;
        public static string case_name;
        public static bool Test_beginning = false;
    }
    ////////////////////////////////////////////////////////////////////////////////

    public class IniManager
    {
        private string filePath;
        private StringBuilder lpReturnedString;
        private int bufferSize;

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string lpString, string lpFileName);

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string lpDefault, StringBuilder lpReturnedString, int nSize, string lpFileName);

        public IniManager(string iniPath)
        {
            filePath = iniPath;
            bufferSize = 48500;
            lpReturnedString = new StringBuilder(bufferSize);
        }

        // read ini date depend on section and key
        public string ReadIniFile(string section, string key, string defaultValue)
        {
            lpReturnedString.Clear();
            GetPrivateProfileString(section, key, defaultValue, lpReturnedString, bufferSize, filePath);
            return lpReturnedString.ToString();
        }

        // write ini data depend on section and key
        public void WriteIniFile(string section, string key, Object value)
        {
            WritePrivateProfileString(section, key, value.ToString(), filePath);
        }


    }

}
