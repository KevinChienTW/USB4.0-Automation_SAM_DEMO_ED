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
using System.IO.Ports;
using System.IO;
using Excel = Microsoft.Office.Interop.Excel;
using System.Diagnostics;
using System.Text.RegularExpressions;








namespace USB4._0_Automation
{
    public partial class Run_setting : Form
    {
        Thread thread_for_Measure;
        Thread thread_for_stop;
        Thread thread_for_analysis;
        Process p = new Process();
        string now_path = System.Windows.Forms.Application.StartupPath + "\\";
        private MessageBasedSession mbSession_E5071C;
        private MessageBasedSession mbSession_E5071C_tdr;
        string E5071C_Resource = "";
        string Switch_Resource = "";

        string test_item = "";
        string test_previous_item = "";
        int test_item_i = 0;

        string switch_ports;

        string now_time;
        string now_time_STA;
        string now_time_STA_end;
        string Store_folder_location;

        string sideA_TX1_1;
        string sideA_RX1_1;
        string sideA_TX2_1;
        string sideA_RX2_1;
        string sideA_DD_1;

        string sideA_TX1_2;
        string sideA_RX1_2;
        string sideA_TX2_2;
        string sideA_RX2_2;
        string sideA_DD_2;

        string sideB_RX1_1;
        string sideB_TX1_1;
        string sideB_RX2_1;
        string sideB_TX2_1;
        string sideB_DD_1 ;

        string sideB_RX1_2;
        string sideB_TX1_2;
        string sideB_RX2_2;
        string sideB_TX2_2;
        string sideB_DD_2;

        string sideA_DP_1;
        string sideA_DN_1;
        string sideA_CC1_1;
        string sideA_SBU1_1;

        string sideA_SBU2_1;
        string sideA_Vbus_1;

        string sideB_DP_1;
        string sideB_DN_1;
        string sideB_CC1_1;
        string sideB_SBU1_1;

        string sideB_SBU2_1;
        string sideB_Vbus_1;

        string sideA_DP_2;
        string sideA_DN_2;
        string sideA_CC1_2;
        string sideA_SBU1_2;
        string sideA_SBU2_2;
        string sideA_Vbus_2;

        string sideB_DP_2;
        string sideB_DN_2;
        string sideB_CC1_2;
        string sideB_SBU1_2;
        string sideB_SBU2_2;
        string sideB_Vbus_2;

        int checkbox_check_total = 0;
        int checkbox_check_total_before = 0;

        string DP_DN_F;
        string GEN_SP4_F;
        string pathString; // new 4item folder 

        string File_Txrx_T;

        IniManager iniManager = new IniManager(System.Environment.CurrentDirectory + "\\" + "setting.ini"); //設定ini檔路徑

        Form4 form_4 = new Form4();
        //Form7 USB_TDR_Measure_form = new Form7();
        public Run_setting()
        {
            InitializeComponent();
        }

        private void Run_setting_Load(object sender, EventArgs e)
        {

            
            //從main中的變數，透過shareArea傳遞到calibration介面中
            Switch_Resource = shareArea.switch_resource;
            E5071C_Resource = shareArea.e5071c_resource;
            
            try
            {
                //設定Switch serialPort參數設定
                serialPort1.PortName = Switch_Resource;
                serialPort1.BaudRate = 9600;
                serialPort1.Parity = Parity.None;
                serialPort1.DataBits = 8;
                serialPort1.StopBits = StopBits.One;
                serialPort1.Encoding = Encoding.UTF8;
                serialPort1.ReadTimeout = 500;
            }
            catch
            {
                MessageBox.Show("Switch COM port can't be opened");
                this.Close();
            }

            
            download_files_check.Checked = true;

            //check_STA_files_B3();
        }

        //按下Measure按鍵-------------------------------------------------------------------
        private void measure_Click(object sender, EventArgs e)
        {
            //thread_for_Measure = new Thread(start_Run);
            //thread_for_Measure.Start();
            string dutnametext;
            string dutsntext;
            dutnametext = Dut_name.Text.Replace(" ", "");
            //dutsntext = Dut_sn.Text.Replace(" ", "");
            if (dutnametext != "")
            {
                if (measure_button.Text == "Measure")
                {
                    var rmSession = new ResourceManager();
                    try
                    {
                        test_previous_item = "";
                        test_item = "";
                        test_item_i = 0;
                        now_time = DateTime.Now.ToString("yyyyMMdd");
                        
                        Store_folder_location = ".//data//" + now_time;

                        //連線設定
                        mbSession_E5071C = (MessageBasedSession)rmSession.Open(E5071C_Resource); //連線到E5071C
                        mbSession_E5071C.TimeoutMilliseconds = 1000;
                        if (!serialPort1.IsOpen)
                            serialPort1.Open(); //連線到Switch

                        try
                        {
                            mbSession_E5071C_tdr = (MessageBasedSession)rmSession.Open("TCPIP0::127.0.0.1::inst0::INSTR");
                            
                            try
                            {
                                thread_for_Measure = new Thread(start_Run);
                                thread_for_Measure.Start();
                                shareArea.Test_beginning = true;
                                measure_button.Text = "Stop";
                                Dut_name.Enabled = false;

                                pictureBox1.BackgroundImage = Properties.Resources.RUN_Wait;
                            }
                            catch (Exception exp)
                            {
                                MessageBox.Show(exp.Message);
                            }
                        }
                        catch (InvalidCastException)
                        {
                            MessageBox.Show("Resource selected must be a message-based session");
                        }
                        catch (Exception exp)
                        {
                            //stop_run();
                            thread_for_stop = new Thread(stop_run);
                            thread_for_stop.Start();
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
                else
                {
                    thread_for_stop = new Thread(stop_run);
                    thread_for_stop.Start();
                    form_4 = new Form4();
                    form_4.ShowDialog();
                }
            }
            else
            {
                if (dutnametext == "")
                {
                    MessageBox.Show("DUT Name and DUT SN is empty");
                }
                else if (dutnametext == "")
                {
                    MessageBox.Show("DUT Name is empty");
                }
                
            }

        }
        private void measure_mouseDown(object sender, MouseEventArgs e)
        {
            measure_button.BackgroundImage = Properties.Resources.button_yellow_s_1;
        }
        private void measure_mouseMove(object sender, MouseEventArgs e)
        {
            measure_button.BackgroundImage = Properties.Resources.button_yellow_s_1;
        }

        private void measure_mouseLeave(object sender, EventArgs e)
        {
            measure_button.BackgroundImage = Properties.Resources.button_yellow_s;
        }

        private void measure_mouseUp(object sender, MouseEventArgs e)
        {
            measure_button.BackgroundImage = Properties.Resources.button_yellow_s;
        }
        ///////////////////////////////////////////////////////////////////////////////////

        //測試被中斷時的動作
        void stop_run()
        {
            //string read_value = "";
            this.Invoke((MethodInvoker)delegate ()
            {
                measure_button.Enabled = false;
            });

            if(thread_for_Measure != null)
            {
                if (thread_for_Measure.IsAlive)
                {
                    thread_for_Measure.Abort();
                }
            }

            //中斷E5071C連線
            if (mbSession_E5071C != null)
            {
                if (!mbSession_E5071C.IsDisposed)
                {
                    mbSession_E5071C.Dispose();
                }
            }
            if (mbSession_E5071C_tdr != null)
            {
                if (!mbSession_E5071C_tdr.IsDisposed)
                {
                    mbSession_E5071C_tdr.Dispose();
                }
            }

            //中斷Switch連線
            if (serialPort1.IsOpen)
            {
                serialPort1.Close();
            }

            //if (exception_message != "none" && exception_message != "執行緒已經中止。")
            //{
            //    MessageBox.Show(exception_message);
            //}

            this.Invoke((MethodInvoker)delegate ()
            {
                measure_button.Text = "Measure";
                Dut_name.Enabled = true;
                measure_button.Enabled = true;
                pictureBox1.BackgroundImage = Properties.Resources.RUN_Finish;
                form_4.Close();
                shareArea.Test_beginning = false;
            });
        }

        //開始執行測試(多執行緒)
        private void start_Run()//20220822可以加上B2路徑?
        {
            var rmSession = new ResourceManager();
            sideA_TX1_1 = iniManager.ReadIniFile("DeEmbedd", "sideA_TX1", "") + "_1.s2p";
            sideA_RX1_1 = iniManager.ReadIniFile("DeEmbedd", "sideA_RX1", "") + "_1.s2p";
            sideA_TX2_1 = iniManager.ReadIniFile("DeEmbedd", "sideA_TX2", "") + "_1.s2p";
            sideA_RX2_1 = iniManager.ReadIniFile("DeEmbedd", "sideA_RX2", "") + "_1.s2p";
            sideA_DD_1 = iniManager.ReadIniFile("DeEmbedd", "sideA_DD", "") + "_1.s2p";

            sideA_TX1_2 = iniManager.ReadIniFile("DeEmbedd", "sideA_TX1", "") + "_2.s2p";
            sideA_RX1_2 = iniManager.ReadIniFile("DeEmbedd", "sideA_RX1", "") + "_2.s2p";
            sideA_TX2_2 = iniManager.ReadIniFile("DeEmbedd", "sideA_TX2", "") + "_2.s2p";
            sideA_RX2_2 = iniManager.ReadIniFile("DeEmbedd", "sideA_RX2", "") + "_2.s2p";
            sideA_DD_2 = iniManager.ReadIniFile("DeEmbedd", "sideA_DD", "") + "_2.s2p";

            sideB_RX1_1 = iniManager.ReadIniFile("DeEmbedd", "sideB_RX1", "") + "_1.s2p";
            sideB_TX1_1 = iniManager.ReadIniFile("DeEmbedd", "sideB_TX1", "") + "_1.s2p";
            sideB_RX2_1 = iniManager.ReadIniFile("DeEmbedd", "sideB_RX2", "") + "_1.s2p";
            sideB_TX2_1 = iniManager.ReadIniFile("DeEmbedd", "sideB_TX2", "") + "_1.s2p";
            sideB_DD_1 = iniManager.ReadIniFile("DeEmbedd", "sideB_DD", "") + "_1.s2p";

            sideB_RX1_2 = iniManager.ReadIniFile("DeEmbedd", "sideB_RX1", "") + "_2.s2p";
            sideB_TX1_2 = iniManager.ReadIniFile("DeEmbedd", "sideB_TX1", "") + "_2.s2p";
            sideB_RX2_2 = iniManager.ReadIniFile("DeEmbedd", "sideB_RX2", "") + "_2.s2p";
            sideB_TX2_2 = iniManager.ReadIniFile("DeEmbedd", "sideB_TX2", "") + "_2.s2p";
            sideB_DD_2 = iniManager.ReadIniFile("DeEmbedd", "sideB_DD", "") + "_2.s2p";

            check_STA_files_B3();

            //創建儲存檔案的資料夾，如果已存在就不再創建  20221216sam removeDUTSN
            this.Invoke((MethodInvoker)delegate ()
            {
                mbSession_E5071C.RawIO.Write("*CLS");

                mbSession_E5071C.RawIO.Write(":MMEM:MDIR \"D:\\CAMS_RESULT\\B3\\" + "( " + Dut_name.Text.ToUpper() + " )" + "\"");//B3

                mbSession_E5071C.RawIO.Write(":SYST:ERR?");
                string err_message = mbSession_E5071C.RawIO.ReadString();
                if (err_message == "+104,\"Failed to create directory\"\n")
                {
                    MessageBox.Show("資料夾已存在");
                    mbSession_E5071C.RawIO.Write(":DISP:CCL");
                    var result = MessageBox.Show("資料夾: " + "( " + Dut_name.Text.ToUpper() + " 已存在，是否要繼續", "", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (result == DialogResult.No)
                    {
                        //stop_run();
                        thread_for_stop = new Thread(stop_run);
                        thread_for_stop.Start();
                        measure_button.Text = "Measure";
                        pictureBox1.BackgroundImage = Properties.Resources.RUN_Finish;
                    }
                }
            });
            if (test1.Checked || test2.Checked || test7.Checked || test8.Checked || test14.Checked || test15.Checked)//SS Pair ILfit、SCD12、SCD21、IMR、IRL
            {
                //#1     /B8#1
                test_item_i = 1;
                Switch("1B,3B,2B,4B");
                fixture_board_calibration_B3(sideA_TX1_1, sideB_RX1_2);
                single_and_save_forSS("TYPE1", "Default");
                //High_speed_test("1B,3B,2B,4B", "TXRX1");

                //#2    /B8#14
                test_item_i = 2;
                Switch("1C,3C,2C,4C");
                fixture_board_calibration_B3(sideA_RX1_1, sideB_TX1_2);
                single_and_save_forSS("TYPE1", "Default");
                //High_speed_test("1C,3C,2C,4C", "TXRX1");

                //#16     /B8#23
                test_item_i = 16;
                Switch("1D,3D,2D,4D");
                fixture_board_calibration_B3(sideA_TX2_1, sideB_RX2_2);
                single_and_save_forSS("TYPE1", "Default");
                //High_speed_test("1D,3D,2D,4D", "TXRX2");

                //#17       /B8#28
                test_item_i = 17;
                Switch("1E,3E,2E,4E");
                fixture_board_calibration_B3(sideA_RX2_1, sideB_TX2_2);  
                single_and_save_forSS("TYPE1", "Default");
                //High_speed_test("1E,3E,2E,4E", "TXRX2");

                
            }

            if (test3.Checked || test11.Checked || test12.Checked || test13.Checked)//DD Pair_Attenuation
            {

                // #3  
                test_item_i = 3;
                Switch("1F,3F,2F,4F");
                fixture_board_calibration_B3(sideA_DD_1, sideB_DD_2);
                single_and_save("TYPE2", "Default");

                //18       
                test_item_i = 18;
                Switch("1F,3F,2F,4F");
                fixture_board_calibration_B3(sideA_DD_1, sideB_DD_2);
                single_and_save("TYPE2", "Default");


                //if (test3.Checked)
                //{
                //    // #3  
                //    test_item_i = 3;
                //    Switch("1F,3F,2F,4F");
                //    fixture_board_calibration_B3(sideA_DD_1, sideB_DD_2);
                //    single_and_save("TYPE2", "Default");

                //    //18       
                //    test_item_i = 18;
                //    Switch("1F,3F,2F,4F");
                //    fixture_board_calibration_B3(sideA_DD_1, sideB_DD_2);
                //    single_and_save("TYPE2", "Default");

                //    //download_s4p_to_local();
                //}
                //else
                //{
                //    // #3  
                //    test_item_i = 3;
                //    Switch("1F,3F,2F,4F");
                //    fixture_board_calibration_B3_Limit(sideA_DD_1, sideB_DD_2);
                //    single_and_save_only_png("TYPE2", "Default");

                //    //18       
                //    test_item_i = 18;
                //    Switch("1F,3F,2F,4F");
                //    fixture_board_calibration_B3_Limit(sideA_DD_1, sideB_DD_2);
                //    single_and_save_only_png("TYPE2", "Default");
                //}
                
            }
            if (test18.Checked || test19.Checked)//INEXT and IFEXT、IDDXT_1NEXT +FEXT and, IDDXT_2NEXT
            {
                // #6  
                test_item_i = 6;
                Switch("1B,3B,2Fx,4Fx");
                fixture_board_calibration_B3(sideA_TX1_1, sideA_DD_2);
                single_and_save("TYPE2", "Default");

                // #7  
                test_item_i = 7;
                Switch("1C,3C,2Fx,4Fx");
                fixture_board_calibration_B3(sideA_RX1_1, sideA_DD_2);
                single_and_save("TYPE2", "Default");

                // #8  
                test_item_i = 8;
                Switch("1FX,3FX,2B,4B");
                fixture_board_calibration_B3(sideB_RX1_1, sideB_DD_2);
                single_and_save("TYPE2", "Default");

                // #9  
                test_item_i = 9;
                Switch("1Cx,3Cx,2F,4F");
                fixture_board_calibration_B3(sideB_TX1_1, sideB_DD_2);
                single_and_save("TYPE2", "Default");

                // #10   /B8#2
                test_item_i = 10;
                Switch("1B,3B,2C,4C");
                fixture_board_calibration_B3(sideA_TX1_1, sideB_TX1_2);
                single_and_save("TYPE2", "Default");

                //#4   /B8#5
                test_item_i = 4;//測試項
                Switch("1B,3B,2Cx,4Cx");//Switch切換組合
                fixture_board_calibration_B3(sideA_TX1_1, sideA_RX1_2);//加入所有校正檔案(包含DeEmbedd與calibration)
                single_and_save("TYPE2", "Default"); //設定畫面顯示、single、儲存檔案

                //#11   /B8#8
                test_item_i = 11;
                Switch("1C,3C,2B,4B"); //2B,4B,1C,3C
                fixture_board_calibration_B3(sideB_RX1_1, sideA_RX1_2);
                single_and_save("TYPE2", "Reverse");

                // #5     /B8#11
                test_item_i = 5;
                Switch("1Cx,3Cx,2B,4B"); //3B,4B,3C,4C
                fixture_board_calibration_B3(sideB_RX1_1, sideB_TX1_2);
                single_and_save("TYPE2", "Default");

                // #13   /B8#30
                test_item_i = 13;
                Switch("1F,3F,2B,4B");
                fixture_board_calibration_B3(sideA_DD_1, sideB_RX1_2);
                single_and_save("TYPE2", "Default");

                // #12    /B8#32
                test_item_i = 12;
                Switch("1F,3F,2C,4C");
                fixture_board_calibration_B3(sideA_DD_1, sideB_TX1_2);
                single_and_save("TYPE2", "Default");

                // #14    /B8#37
                test_item_i = 14;
                Switch("1B,3B,2F,4F"); //3F,4F,1B,2B
                fixture_board_calibration_B3(sideB_DD_1, sideA_TX1_2);
                single_and_save("TYPE2", "Reverse");

                // #15    /B8#39
                test_item_i = 15;
                Switch("1C,3C,2F,4F"); //3F,4F,1C,2C
                fixture_board_calibration_B3(sideB_DD_1, sideA_RX1_2);
                single_and_save("TYPE2", "Reverse");

                //19       
                test_item_i = 19;
                Switch("1D,3D,2Ex,4Ex");
                fixture_board_calibration_B3(sideA_TX2_1, sideA_RX2_2);
                single_and_save("TYPE2", "Default");

                //21       
                test_item_i = 21;
                Switch("1D,3D,2Fx,4Fx");
                fixture_board_calibration_B3(sideA_TX2_1, sideA_DD_2);
                single_and_save("TYPE2", "Default");

                //22       
                test_item_i = 22;
                Switch("1E,3E,2Fx,4Fx");
                fixture_board_calibration_B3(sideA_RX2_1, sideA_DD_2);
                single_and_save("TYPE2", "Default");

                //23       
                test_item_i = 23;
                Switch("1Dx,3Dx,2F,4F");
                fixture_board_calibration_B3(sideB_RX2_1, sideB_DD_2);
                single_and_save("TYPE2", "Default");

                //24       
                test_item_i = 24;
                Switch("1Ex,3Ex,2F,4F");
                fixture_board_calibration_B3(sideB_TX2_1, sideB_DD_2);
                single_and_save("TYPE2", "Default");

                //20       /B8#27                          //出現錯誤~~FILNAME NOT FOUND ??   這一個得要重新K一次
                test_item_i = 20;
                Switch("1Ex,3Ex,2D,4D"); //3D,4D,3E,4E
                fixture_board_calibration_B3(sideB_RX2_1, sideB_TX2_2);
                single_and_save("TYPE2", "Reverse");

                //#25        /B8#24
                test_item_i = 25;
                Switch("1D,3D,2E,4E");
                fixture_board_calibration_B3(sideA_TX2_1, sideB_TX2_2);
                single_and_save("TYPE2", "Default");

                //#26       /B8#26
                test_item_i = 26;
                Switch("1E,3E,2D,4D"); //3D,4D,1E,2E
                fixture_board_calibration_B3(sideB_RX2_1, sideA_RX2_2);
                single_and_save("TYPE2", "Reverse");

                //27       /B8#36
                test_item_i = 27;
                Switch("1F,3F,2E,4E");
                fixture_board_calibration_B3(sideA_DD_1, sideB_TX2_2);
                single_and_save("TYPE2", "Default");


                //#28       /B8#34
                test_item_i = 28;
                Switch("1F,3F,2D,4D");
                fixture_board_calibration_B3(sideA_DD_1, sideB_RX2_2);
                single_and_save("TYPE2", "Default");

                //29       /B8#41
                test_item_i = 29;
                Switch("1D,3D,2F,4F"); //3F,4F,1D,2D
                fixture_board_calibration_B3(sideB_DD_1, sideA_TX2_2);
                single_and_save("TYPE2", "Reverse");

                //30       /B8#43
                test_item_i = 30;
                Switch("1E,3E,2F,4F"); //3F,4F,1E,2E
                fixture_board_calibration_B3(sideB_DD_1, sideA_RX2_2);
                single_and_save("TYPE2", "Reverse");
            }

            download_s4p_to_local();
            //MessageBox.Show("FINISH");
            //DateTime Td0 = DateTime.Now;
            //download_s4p_to_local();
            //DateTime Td1 = DateTime.Now;
            //Console.WriteLine(Td1 - Td0);
            thread_for_stop = new Thread(stop_run);
            thread_for_stop.Start();

            ///////////////////////analys/////////////
            ///
            thread_for_analysis = new Thread(analysis_s4p);
            thread_for_analysis.Start();
            ///
            ///////////////////////////////////////
        }

        void analysis_s4p()
        {
            string para = "demo_code\\demo_code.exe";

            string folder_location = Store_folder_location.Replace(".//", "./");          
            folder_location = folder_location.Replace("//", "\\");

            string input_path = " -i \""+ folder_location+"\\" + "( " + Dut_name.Text + " )\"";

            string output_path = " -o \"" + folder_location + "\\" + "( " + Dut_name.Text + " )\" --getall";

            string plots;

            if (Save_plots.Checked)
            {
                plots = " --saveplots";
            }
            else
            {
                plots = "";
            }

            string parameter = para + input_path + output_path + plots;

            p.StartInfo.FileName = "cmd.exe";

            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.CreateNoWindow = true; //不跳出cmd視窗
            //p.StartInfo.Arguments = parameter;
            p.Start();
            p.StandardInput.WriteLine(parameter);
            //p.WaitForExit();
            p.Close();

            if (p != null)
            {
                //p.WaitForExit();
                p.Close();

                p.Dispose();
                p = null;
            }
        }

        //匯入所有校正所需檔案------------------------------------------------------------------------------
        void fixture_board_calibration(string FB_P1P3, string FB_P2P4)///20220822如果B2可以共用function，處新增
        {
            try
            {
                ////Load .STA
                ///
                mbSession_E5071C.RawIO.Write(":MMEM:LOAD \"D:\\CAMS_STA\\B3\\" + switch_ports + ".STA\"");


                mbSession_E5071C.RawIO.Write(":DISP:WIND2:ACT");
                mbSession_E5071C.RawIO.Write(":DISP:MAX ON");
                wait_done("*OPC?");
                ////Environment_setting();

                ////Load .s2p
                mbSession_E5071C.RawIO.Write(":CALC2:FSIM:SEND:DEEM:STAT ON");
                mbSession_E5071C.RawIO.Write(":CALC2:FSIM:SEND:DEEM:PORT1:USER:FIL " + "\"" + FB_P1P3 + "\""); //:CALC1:FSIM:SEND:DEEM:PORT1 USER
                mbSession_E5071C.RawIO.Write(":CALC2:FSIM:SEND:DEEM:PORT3:USER:FIL " + "\"" + FB_P1P3 + "\"");
                mbSession_E5071C.RawIO.Write(":CALC2:FSIM:SEND:DEEM:PORT2:USER:FIL " + "\"" + FB_P2P4 + "\"");
                mbSession_E5071C.RawIO.Write(":CALC2:FSIM:SEND:DEEM:PORT4:USER:FIL " + "\"" + FB_P2P4 + "\"");

                mbSession_E5071C.RawIO.Write(":CALC2:FSIM:SEND:DEEM:PORT1 USER");
                mbSession_E5071C.RawIO.Write(":CALC2:FSIM:SEND:DEEM:PORT2 USER");
                mbSession_E5071C.RawIO.Write(":CALC2:FSIM:SEND:DEEM:PORT3 USER");
                mbSession_E5071C.RawIO.Write(":CALC2:FSIM:SEND:DEEM:PORT4 USER");
                wait_done("*OPC?");
            }
            catch (Exception exp)
            {
                //stop_run(exp.Message);
                MessageBox.Show(exp.Message + "\n Loading STA or Deembed file");
                thread_for_stop = new Thread(stop_run);
                thread_for_stop.Start();
            }

        }
        void fixture_board_calibration_B3(string FB_P1P3, string FB_P2P4)///20220822如果B2可以共用function，處新增
        {

            try
            {
                DateTime T0 = DateTime.Now;
                ////Load .STA
                ///
                mbSession_E5071C.RawIO.Write(":MMEM:LOAD \"D:\\CAMS_STA\\B3\\" + switch_ports + ".STA\"");
                wait_done("*OPC?");
                /////    keivn test -s
                ///
                DateTime zero = DateTime.Now;
                Console.WriteLine(zero - T0);

                mbSession_E5071C.RawIO.Write(":DISP:WIND2:ACT");//0.4sec
                mbSession_E5071C.RawIO.Write(":DISP:MAX ON");


                mbSession_E5071C.RawIO.Write(":CALC1:PAR1:SEL");

                mbSession_E5071C.RawIO.Write(":DISP:WIND2:MAX ON");

                mbSession_E5071C.RawIO.Write(":CALC2:LIM ON");                                                                                                                       //Analysis > Limit Test > Limit Test
                mbSession_E5071C.RawIO.Write(":CALC2:LIM:DISP ON");

               

                //mbSession_E5071C.RawIO.Write(":MMEM:LOAD:LIM \"D:\\CAMS_Limitline\\IMP_LIMIT_1.csv\"");
                

                //wait_done("*OPC?");

                //mbSession_E5071C.RawIO.Write(":CALC1:LIM ON");                                                                                                                       //Analysis > Limit Test > Limit Test
                //mbSession_E5071C.RawIO.Write(":CALC1:LIM:DISP ON");                                                                                                                  //Analysis > Limit Test > Limit Line
                //mbSession_E5071C.RawIO.Write(":DISP:FSIG OFF");
                //wait_done("*OPC?");

                //mbSession_E5071C.RawIO.Write(":DISP:MAX Off");
                /////    kevin  test -e

                mbSession_E5071C.RawIO.Write(":DISP:WIND2:ACT");
                mbSession_E5071C.RawIO.Write(":DISP:MAX ON");
                DateTime T2 = DateTime.Now;
                //Console.WriteLine("limit = ", T2 - zero);
                
                ////Environment_setting();
                ////Load .s2p
                mbSession_E5071C.RawIO.Write(":CALC2:FSIM:SEND:DEEM:STAT ON");
                //wait_done("*OPC?");
                //Thread.Sleep(10);
                mbSession_E5071C.RawIO.Write(":CALC2:FSIM:SEND:DEEM:PORT1:USER:FIL " + "\"" + FB_P1P3 + "\""); //:CALC1:FSIM:SEND:DEEM:PORT1 USER
                //mbSession_E5071C.RawIO.Write(":CALC2:FSIM:SEND:DEEM:PORT1 USER");
                //wait_done("*OPC?");
                //Thread.Sleep(10);
                mbSession_E5071C.RawIO.Write(":CALC2:FSIM:SEND:DEEM:PORT2:USER:FIL " + "\"" + FB_P1P3 + "\"");
                //mbSession_E5071C.RawIO.Write(":CALC2:FSIM:SEND:DEEM:PORT2 USER");
                //wait_done("*OPC?");
                //////Thread.Sleep(10);
                mbSession_E5071C.RawIO.Write(":CALC2:FSIM:SEND:DEEM:PORT3:USER:FIL " + "\"" + FB_P2P4 + "\"");
                //mbSession_E5071C.RawIO.Write(":CALC2:FSIM:SEND:DEEM:PORT3 USER");
                ////wait_done("*OPC?");
                ////Thread.Sleep(10);
                mbSession_E5071C.RawIO.Write(":CALC2:FSIM:SEND:DEEM:PORT4:USER:FIL " + "\"" + FB_P2P4 + "\"");
                //mbSession_E5071C.RawIO.Write(":CALC2:FSIM:SEND:DEEM:PORT4 USER");
                ////wait_done("*OPC?");
                ////Thread.Sleep(10);

                //mbSession_E5071C.RawIO.Write(":CALC2:FSIM:SEND:DEEM:PORT1 USER");
                //////wait_done("*OPC?");
                //////Thread.Sleep(10);
                //mbSession_E5071C.RawIO.Write(":CALC2:FSIM:SEND:DEEM:PORT2 USER");
                //////wait_done("*OPC?");
                //////Thread.Sleep(10);
                //mbSession_E5071C.RawIO.Write(":CALC2:FSIM:SEND:DEEM:PORT3 USER");
                ////wait_done("*OPC?");
                //Thread.Sleep(10);
                //mbSession_E5071C.RawIO.Write(":CALC2:FSIM:SEND:DEEM:PORT4 USER");
                wait_done("*OPC?");
                //Thread.Sleep(10);
                DateTime T3 = DateTime.Now;
                Console.WriteLine(T3 - T2);
            }
            catch (Exception exp)
            {
                //stop_run(exp.Message);
                MessageBox.Show(exp.Message + "\n Loading STA or Deembed file");
                thread_for_stop = new Thread(stop_run);
                thread_for_stop.Start();
            }

        }

        
        void fixture_board_calibration_B3_Limit(string FB_P1P3, string FB_P2P4)///20220822如果B2可以共用function，處新增
        {
            try
            {
                //DateTime T0 = DateTime.Now;
                ////Load .STA
                ///
                mbSession_E5071C.RawIO.Write(":MMEM:LOAD \"D:\\CAMS_STA\\B3\\" + switch_ports + ".STA\"");
                wait_done("*OPC?");
                /////    keivn test -s
                ///
                //DateTime zero = DateTime.Now;
                //Console.WriteLine("Load STA = ", zero - T0);

                mbSession_E5071C.RawIO.Write(":DISP:WIND2:ACT");
                mbSession_E5071C.RawIO.Write(":DISP:MAX ON");


                mbSession_E5071C.RawIO.Write(":CALC1:PAR1:SEL");

                mbSession_E5071C.RawIO.Write(":DISP:WIND2:MAX ON");

                mbSession_E5071C.RawIO.Write(":CALC2:LIM ON");                                                                                                                       //Analysis > Limit Test > Limit Test
                mbSession_E5071C.RawIO.Write(":CALC2:LIM:DISP ON");



                mbSession_E5071C.RawIO.Write(":MMEM:LOAD:LIM \"D:\\CAMS_Limitline\\IMP_LIMIT_1.csv\"");


                wait_done("*OPC?");

                mbSession_E5071C.RawIO.Write(":CALC1:LIM ON");                                                                                                                       //Analysis > Limit Test > Limit Test
                mbSession_E5071C.RawIO.Write(":CALC1:LIM:DISP ON");                                                                                                                  //Analysis > Limit Test > Limit Line
                mbSession_E5071C.RawIO.Write(":DISP:FSIG OFF");
                wait_done("*OPC?");

                mbSession_E5071C.RawIO.Write(":DISP:MAX Off");
                /////    kevin  test -e

                mbSession_E5071C.RawIO.Write(":DISP:WIND2:ACT");
                mbSession_E5071C.RawIO.Write(":DISP:MAX ON");
                //DateTime T2 = DateTime.Now;
                //Console.WriteLine("limit = ", T2 - zero);

                ////Environment_setting();
                ////Load .s2p
                mbSession_E5071C.RawIO.Write(":CALC2:FSIM:SEND:DEEM:STAT ON");
                mbSession_E5071C.RawIO.Write(":CALC2:FSIM:SEND:DEEM:PORT1:USER:FIL " + "\"" + FB_P1P3 + "\""); //:CALC1:FSIM:SEND:DEEM:PORT1 USER
                mbSession_E5071C.RawIO.Write(":CALC2:FSIM:SEND:DEEM:PORT3:USER:FIL " + "\"" + FB_P1P3 + "\"");
                mbSession_E5071C.RawIO.Write(":CALC2:FSIM:SEND:DEEM:PORT2:USER:FIL " + "\"" + FB_P2P4 + "\"");
                mbSession_E5071C.RawIO.Write(":CALC2:FSIM:SEND:DEEM:PORT4:USER:FIL " + "\"" + FB_P2P4 + "\"");

                mbSession_E5071C.RawIO.Write(":CALC2:FSIM:SEND:DEEM:PORT1 USER");
                mbSession_E5071C.RawIO.Write(":CALC2:FSIM:SEND:DEEM:PORT2 USER");
                mbSession_E5071C.RawIO.Write(":CALC2:FSIM:SEND:DEEM:PORT3 USER");
                mbSession_E5071C.RawIO.Write(":CALC2:FSIM:SEND:DEEM:PORT4 USER");
                wait_done("*OPC?");
                //DateTime T3 = DateTime.Now;
                //Console.WriteLine("Environment_setting &Load .s2p = ",T3 - T2);
            }
            catch (Exception exp)
            {
                //stop_run(exp.Message);
                MessageBox.Show(exp.Message + "\n Loading STA or Deembed file");
                thread_for_stop = new Thread(stop_run);
                thread_for_stop.Start();
            }

        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////////////

        //設定測試畫面、執行Single、存檔--------------------------------------------------------------------------
        void single_and_save(string scale, string switch_type)///20220822如果B2可以共用function，處新增
        {
            //DateTime T0 = DateTime.Now;
            test_item = "ITEM" + test_item_i.ToString();

            try
            {
                if (scale == "TYPE1")
                {
                    mbSession_E5071C.RawIO.Write(":DISP:WIND2:TRAC1:Y:PDIV 5");
                    mbSession_E5071C.RawIO.Write(":DISP:WIND2:TRAC1:Y:RPOS 8");
                    mbSession_E5071C.RawIO.Write(":DISP:WIND2:TRAC1:Y:RLEV 0");
                    mbSession_E5071C.RawIO.Write(":DISP:WIND2:TRAC2:Y:PDIV 5");
                    mbSession_E5071C.RawIO.Write(":DISP:WIND2:TRAC2:Y:RPOS 8");
                    mbSession_E5071C.RawIO.Write(":DISP:WIND2:TRAC2:Y:RLEV 0");
                }
                else if (scale == "TYPE2")
                {
                    mbSession_E5071C.RawIO.Write(":DISP:WIND2:TRAC1:Y:PDIV 10");
                    mbSession_E5071C.RawIO.Write(":DISP:WIND2:TRAC1:Y:RPOS 8");
                    mbSession_E5071C.RawIO.Write(":DISP:WIND2:TRAC1:Y:RLEV -30");
                    //mbSession_E5071C.RawIO.Write(":DISP:WIND2:TRAC2:Y:PDIV 10");
                    //mbSession_E5071C.RawIO.Write(":DISP:WIND2:TRAC2:Y:RPOS 8");
                    //mbSession_E5071C.RawIO.Write(":DISP:WIND2:TRAC2:Y:RLEV -30");
                }

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




                //single
                if (switch_type == "Default")
                {
                    mbSession_E5071C.RawIO.Write(":CALC2:PAR:COUN 1");
                    mbSession_E5071C.RawIO.Write(":TRIG:SING");
                    wait_single_done("*OPC?");
                }
                else if (switch_type == "Reverse")
                {
                    mbSession_E5071C.RawIO.Write(":CALC2:PAR:COUN 2");
                    mbSession_E5071C.RawIO.Write(":TRIG:SING");
                    wait_single_done("*OPC?");
                    mbSession_E5071C.RawIO.Write(":CALC2:PAR1:SEL");
                    mbSession_E5071C.RawIO.Write(":DISP:WIND2:MAX ON");
                }
                //DateTime T1 = DateTime.Now;
                //Console.WriteLine(T1 - T0);

                //save
                //:DISP:CCL
                mbSession_E5071C.RawIO.Write("*CLS");
                mbSession_E5071C.RawIO.Write(":DISP:CCL");
                mbSession_E5071C.RawIO.Write(":MMEM:STOR:SNP:TYPE:S4P 1,2,3,4");
                wait_done("*OPC?");

                mbSession_E5071C.RawIO.Write(":MMEM:STOR:SNP \"D:\\CAMS_RESULT\\B3\\" + "( " + Dut_name.Text.ToUpper() + " )" + "\\" + test_item + ".s4p\"");
                wait_done("*OPC?");
                Thread.Sleep(50);

                mbSession_E5071C.RawIO.Write(":CALC2:PAR:COUN 1"); //必須在存檔.s4p之後才可以執行，因為.s4p存檔需要single當下的所有trace才可以存檔。
                wait_done("*OPC?");
                Thread.Sleep(50);

                //mbSession_E5071C.RawIO.Write(":MMEMory:STORe:FDATa \"D:\\CAMS_RESULT\\B3\\" + "( " + Dut_name.Text.ToUpper() +  " )" + "\\" + test_item + ".csv\"");
                //wait_done("*OPC?");
                //Thread.Sleep(100);

                ////mbSession_E5071C.RawIO.Write(":MMEM:STOR:IMAG \"D:\\CAMS_RESULT\\" + "( " + Dut_name.Text.ToUpper() + "_" + Dut_sn.Text.ToUpper() + " )" + "\\" + test_item + ".png\"");
                ////wait_done("*OPC?");
                ////Thread.Sleep(50);

                test_previous_item = test_item;
                //DateTime T2 = DateTime.Now;
                //Console.WriteLine(T2 - T1);
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message + "\n single and save");
                //stop_run(exp.Message);
                thread_for_stop = new Thread(stop_run);
                thread_for_stop.Start();
            }
        }
        void single_and_save_forSS(string scale, string switch_type)///20220822如果B2可以共用function，處新增
        {
            DateTime T0 = DateTime.Now;
            test_item = "ITEM" + test_item_i.ToString();

            try
            {
                if (scale == "TYPE1")
                {
                    mbSession_E5071C.RawIO.Write(":DISP:WIND2:TRAC1:Y:PDIV 5");
                    mbSession_E5071C.RawIO.Write(":DISP:WIND2:TRAC1:Y:RPOS 8");
                    mbSession_E5071C.RawIO.Write(":DISP:WIND2:TRAC1:Y:RLEV 0");
                    mbSession_E5071C.RawIO.Write(":DISP:WIND2:TRAC2:Y:PDIV 5");
                    mbSession_E5071C.RawIO.Write(":DISP:WIND2:TRAC2:Y:RPOS 8");
                    mbSession_E5071C.RawIO.Write(":DISP:WIND2:TRAC2:Y:RLEV 0");
                }
                else if (scale == "TYPE2")
                {
                    mbSession_E5071C.RawIO.Write(":DISP:WIND2:TRAC1:Y:PDIV 10");
                    mbSession_E5071C.RawIO.Write(":DISP:WIND2:TRAC1:Y:RPOS 8");
                    mbSession_E5071C.RawIO.Write(":DISP:WIND2:TRAC1:Y:RLEV -30");
                    //mbSession_E5071C.RawIO.Write(":DISP:WIND2:TRAC2:Y:PDIV 10");
                    //mbSession_E5071C.RawIO.Write(":DISP:WIND2:TRAC2:Y:RPOS 8");
                    //mbSession_E5071C.RawIO.Write(":DISP:WIND2:TRAC2:Y:RLEV -30");
                }

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




                //single
                if (switch_type == "Default")
                {
                    mbSession_E5071C.RawIO.Write(":CALC2:PAR:COUN 1");
                    mbSession_E5071C.RawIO.Write(":TRIG:SING");
                    wait_single_done("*OPC?");
                }
                else if (switch_type == "Reverse")
                {
                    mbSession_E5071C.RawIO.Write(":CALC2:PAR:COUN 2");
                    mbSession_E5071C.RawIO.Write(":TRIG:SING");
                    wait_single_done("*OPC?");
                    mbSession_E5071C.RawIO.Write(":CALC2:PAR1:SEL");
                    mbSession_E5071C.RawIO.Write(":DISP:WIND2:MAX ON");
                }
                DateTime T1 = DateTime.Now;
                Console.WriteLine(T1 - T0);

                //save
                //:DISP:CCL
                mbSession_E5071C.RawIO.Write("*CLS");
                mbSession_E5071C.RawIO.Write(":DISP:CCL");
                mbSession_E5071C.RawIO.Write(":MMEM:STOR:SNP:TYPE:S4P 1,2,3,4");
                wait_done("*OPC?");

                mbSession_E5071C.RawIO.Write(":MMEM:STOR:SNP \"D:\\CAMS_RESULT\\B3\\" + "( " + Dut_name.Text.ToUpper() + " )" + "\\" + test_item + ".s4p\"");
                wait_done("*OPC?");
                Thread.Sleep(50);

                mbSession_E5071C.RawIO.Write(":CALC2:PAR:COUN 1"); //必須在存檔.s4p之後才可以執行，因為.s4p存檔需要single當下的所有trace才可以存檔。
                wait_done("*OPC?");
                Thread.Sleep(50);

                mbSession_E5071C.RawIO.Write(":MMEMory:STORe:FDATa \"D:\\CAMS_RESULT\\B3\\" + "( " + Dut_name.Text.ToUpper() + " )" + "\\" + test_item + ".csv\"");
                wait_done("*OPC?");
                Thread.Sleep(100);

                

                test_previous_item = test_item;
                DateTime T2 = DateTime.Now;
                Console.WriteLine(T2 - T1);
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message + "\n single and save");
                //stop_run(exp.Message);
                thread_for_stop = new Thread(stop_run);
                thread_for_stop.Start();
            }
        }

        void single_and_save_only_png(string scale, string switch_type)///20220822如果B2可以共用function，處新增
        {
            //DateTime T0 = DateTime.Now;
            test_item = "ITEM" + test_item_i.ToString();

            try
            {
                if (scale == "TYPE1")
                {
                    mbSession_E5071C.RawIO.Write(":DISP:WIND2:TRAC1:Y:PDIV 5");
                    mbSession_E5071C.RawIO.Write(":DISP:WIND2:TRAC1:Y:RPOS 8");
                    mbSession_E5071C.RawIO.Write(":DISP:WIND2:TRAC1:Y:RLEV 0");
                    mbSession_E5071C.RawIO.Write(":DISP:WIND2:TRAC2:Y:PDIV 5");
                    mbSession_E5071C.RawIO.Write(":DISP:WIND2:TRAC2:Y:RPOS 8");
                    mbSession_E5071C.RawIO.Write(":DISP:WIND2:TRAC2:Y:RLEV 0");
                }
                else if (scale == "TYPE2")
                {
                    mbSession_E5071C.RawIO.Write(":DISP:WIND2:TRAC1:Y:PDIV 10");
                    mbSession_E5071C.RawIO.Write(":DISP:WIND2:TRAC1:Y:RPOS 8");
                    mbSession_E5071C.RawIO.Write(":DISP:WIND2:TRAC1:Y:RLEV -30");
                    //mbSession_E5071C.RawIO.Write(":DISP:WIND2:TRAC2:Y:PDIV 10");
                    //mbSession_E5071C.RawIO.Write(":DISP:WIND2:TRAC2:Y:RPOS 8");
                    //mbSession_E5071C.RawIO.Write(":DISP:WIND2:TRAC2:Y:RLEV -30");
                }

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




                //single
                if (switch_type == "Default")
                {
                    mbSession_E5071C.RawIO.Write(":CALC2:PAR:COUN 1");
                    mbSession_E5071C.RawIO.Write(":TRIG:SING");
                    wait_single_done("*OPC?");
                }
                else if (switch_type == "Reverse")
                {
                    mbSession_E5071C.RawIO.Write(":CALC2:PAR:COUN 2");
                    mbSession_E5071C.RawIO.Write(":TRIG:SING");
                    wait_single_done("*OPC?");
                    mbSession_E5071C.RawIO.Write(":CALC2:PAR1:SEL");
                    mbSession_E5071C.RawIO.Write(":DISP:WIND2:MAX ON");
                }
                //DateTime T1 = DateTime.Now;
                //Console.WriteLine(T1 - T0);

                //save
                //:DISP:CCL
                mbSession_E5071C.RawIO.Write("*CLS");
                mbSession_E5071C.RawIO.Write(":DISP:CCL");
                mbSession_E5071C.RawIO.Write(":MMEM:STOR:SNP:TYPE:S4P 1,2,3,4");
                wait_done("*OPC?");

                mbSession_E5071C.RawIO.Write(":MMEM:STOR:SNP \"D:\\CAMS_RESULT\\B3\\" + "( " + Dut_name.Text.ToUpper() + " )" + "\\" + test_item + ".s4p\"");
                wait_done("*OPC?");
                Thread.Sleep(50);

                mbSession_E5071C.RawIO.Write(":CALC2:PAR:COUN 1"); //必須在存檔.s4p之後才可以執行，因為.s4p存檔需要single當下的所有trace才可以存檔。
                wait_done("*OPC?");
                Thread.Sleep(50);

                //mbSession_E5071C.RawIO.Write(":MMEMory:STORe:FDATa \"D:\\CAMS_RESULT\\B3\\" + "( " + Dut_name.Text.ToUpper() +  " )" + "\\" + test_item + ".csv\"");
                //wait_done("*OPC?");
                //Thread.Sleep(100);

                mbSession_E5071C.RawIO.Write(":MMEM:STOR:IMAG \"D:\\CAMS_RESULT\\B3\\" + "( " + Dut_name.Text.ToUpper() +  " )" + "\\" + test_item + ".png\"");
                wait_done("*OPC?");
                Thread.Sleep(50);

                test_previous_item = test_item;
                //DateTime T2 = DateTime.Now;
                //Console.WriteLine(T2 - T1);
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message + "\n single and save");
                //stop_run(exp.Message);
                thread_for_stop = new Thread(stop_run);
                thread_for_stop.Start();
            }
        }

        void wait_done(string gpib_command)
        {
            string read_value;

            if (gpib_command != "")
            {
                try
                {
                    mbSession_E5071C.RawIO.Write(gpib_command);
                    //Thread.Sleep(1000);
                    //for (uint i = 0; i< 1000; i++) 
                    //{
                    //    for (uint j = 0; j< 30000; j++) ;
                     
                    //}
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
                        //Thread.Sleep(50);
                    }
                }
                catch (Exception exp)
                {
                    MessageBox.Show(exp.Message + "\n *OPC Command Error");
                }
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
                    //MessageBox.Show("*OPC Command Error");
                }
            }
        }

        //等待ENA single結束
        void wait_single_done(string gpib_command)///20220822確認B2是否有需要下載.S4P檔
        {
            string read_value = "";
            download_s4p_to_local(); //下載.S4P檔
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
                //stop_run(exp.Message);
                MessageBox.Show(exp.Message + "\n Single response message Error");
                thread_for_stop = new Thread(stop_run);
                thread_for_stop.Start();
            }

        }

        void download_s4p_to_local() //下載前一個single完的.S4P檔案
        {
            try 
            {
                //下載.S4P檔到本地端
                string ENA_output_data = "";
                if (download_files_check.Checked == true)
                {
                    if (test_previous_item != "")
                    {
                        Directory.CreateDirectory(Store_folder_location + "\\" + "( " + Dut_name.Text + " )");
                        //Directory.CreateDirectory(Store_folder_location + "\\" + "( " + Dut_name.Text + " )"+"\\Result\\");
                        System.IO.StreamWriter sw = new System.IO.StreamWriter(Store_folder_location + "\\" + "( " + Dut_name.Text + " )" + "\\" + test_previous_item + ".S4P");

                        mbSession_E5071C.RawIO.Write(":MMEM:TRAN? \"D:\\CAMS_RESULT\\B3\\" + "( " + Dut_name.Text.ToUpper() + " )" + "\\" + test_previous_item + ".s4p\"");

                        Thread.Sleep(1);
                        while (true)
                        {
                            try
                            {
                                ENA_output_data = mbSession_E5071C.RawIO.ReadString();
                                ENA_output_data = ENA_output_data.Substring(8, ENA_output_data.Length - 8);
                                sw.Write(ENA_output_data);
                                break;
                            }
                            catch
                            {
                                if (ENA_output_data != "")
                                    break;
                            }
                        }


                        while (true)
                        {
                            try
                            {
                                ENA_output_data = mbSession_E5071C.RawIO.ReadString();
                                sw.Write(ENA_output_data);
                                if (ENA_output_data.Length < 1024)
                                {
                                    break;
                                }
                            }
                            catch
                            {
                                break;
                            }
                        }
                        sw.Close();
                    }
                }
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message + "\n Download " + test_previous_item + ".S4P" + " Fail");
            }
            
        }

       

        
        void check_STA_files_B3()
        {
            string read_value1 = "";
            string read_value2 = "";
            string Not_found_item = "";

            string[] switch_combination = new string[] { "1B,3B,2B,4B","1B,3B,2C,4C","1B,3B,2Cx,4Cx","1B,3B,2F,4F","1B,3B,2Fx,4Fx","1FX,3FX,2B,4B","1C,3C,2B,4B",
                                                         "1C,3C,2C,4C","1C,3C,2F,4F","1C,3C,2Fx,4Fx","1Cx,3Cx,2B,4B","1Cx,3Cx,2F,4F","1D,3D,2D,4D","1D,3D,2E,4E",
                                                         "1D,3D,2Ex,4Ex","1D,3D,2F,4F","1D,3D,2Fx,4Fx","1Dx,3Dx,2F,4F","1E,3E,2D,4D","1E,3E,2E,4E","1Ex,3Ex,2D,4D",
                                                         "1E,3E,2F,4F","1E,3E,2Fx,4Fx","1Ex,3Ex,2F,4F","1F,3F,2B,4B","1F,3F,2C,4C","1F,3F,2D,4D","1F,3F,2E,4E",
                                                         "1F,3F,2F,4F","1F,3F,2F,4F"
            };

            mbSession_E5071C.RawIO.Write(":MMEM:CAT? " + "\"" + "D:\\CAMS_STA\\B3\\" + "\"");
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

            foreach (string item in switch_combination)
            {
                if (read_value1.Contains(item.ToUpper()))
                {

                }
                else
                {
                    string[] split_item = item.Split(',');
                    string real_item_combination = "";
                    string sub_item_change = "";
                    int i = 0;
                    foreach (string sub_item in split_item)
                    {
                        if (sub_item.Contains("x"))
                        {
                            int new_num = 0;
                            int num = Convert.ToInt32(Regex.Replace(sub_item, "[^0-9]", ""));
                            sub_item_change = sub_item.Replace("x", "");


                            if (num == 1)
                            {
                                new_num = 2;
                            }
                            else if (num == 3)
                            {
                                new_num = 4;
                            }
                            else if (num == 2)
                            {
                                new_num = 1;
                            }
                            else if (num == 4)
                            {
                                new_num = 3;
                            }

                            sub_item_change = sub_item_change.Replace(num.ToString(), new_num.ToString());
                        }
                        else
                        {
                            sub_item_change = sub_item;
                        }
                        if (i < 3)
                            real_item_combination = real_item_combination + sub_item_change + ",";
                        else
                            real_item_combination = real_item_combination + sub_item_change;
                        i++;
                    }

                    Not_found_item = Not_found_item + real_item_combination + "\n";
                }
            }

            if (Not_found_item != "")
            {
                var result = MessageBox.Show("校正檔案有缺少，如果要繼續執行，請按Yes按鈕。\n\n缺少校正組合:\n" + Not_found_item, "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.No)
                {
                    this.Invoke((MethodInvoker)delegate ()
                    {
                        measure_button.Text = "Measure";
                        pictureBox1.BackgroundImage = Properties.Resources.RUN_Finish;
                    });
                    //stop_run();
                    thread_for_stop = new Thread(stop_run);
                    thread_for_stop.Start();

                }
            }
        }

        void Switch(string gpib_command)
        {
            try
            {
                //DateTime T0 = DateTime.Now;
                string serial_input_data = "";
                switch_ports = gpib_command;

                if (!serialPort1.IsOpen)
                {
                    serialPort1.Open();
                    serialPort1.DiscardInBuffer();
                    serialPort1.DiscardOutBuffer();
                }
                else
                {
                    serialPort1.DiscardInBuffer();
                    serialPort1.DiscardOutBuffer();
                }
                serialPort1.Write(gpib_command + ",");
                //DateTime T1 = DateTime.Now;

                //Console.WriteLine(T1 - T0);
                //while (true)
                //{
                //    try
                //    {
                //        serial_input_data = serialPort1.ReadLine();
                //        if (serial_input_data.Contains("OK"))
                //        {

                //            break;
                //        }

                //    }
                //    catch
                //    {
                //        break;
                //    }
                //}
                //serialPort1.Dispose();
                //serialPort1.Close();
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
                //stop_run();
                thread_for_stop = new Thread(stop_run);
                thread_for_stop.Start();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            
        }

        private void Run_setting_FormClosing(object sender, FormClosingEventArgs e)
        {
            thread_for_stop = new Thread(stop_run);
            thread_for_stop.Start();
            form_4 = new Form4();
            form_4.ShowDialog();
        }

        private void TDR_measure_button_Click(object sender, EventArgs e)
        {
            //USB_TDR_Measure_form = new Form7();
            //calibration_form.Owner = this;
            //USB_TDR_Measure_form.Show();
        }
        
        
        
        private void Project_list_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            if (test1.Checked || test2.Checked || test3.Checked || test6.Checked)
            {
                test4.Enabled = false;
                test8.Enabled = false;
                test9.Enabled = false;
                test3.Enabled = false;
            }

            if (test4.Checked || test8.Checked || test9.Checked)
            {
                test1.Enabled = false;
                test2.Enabled = false;
                test3.Enabled = false;
                test6.Enabled = false;
                test3.Enabled = false;

            }

            if (test3.Checked)
            {
                test4.Enabled = false;
                test8.Enabled = false;
                test9.Enabled = false;
                test1.Enabled = false;
                test2.Enabled = false;
                test3.Enabled = false;
                test6.Enabled = false;
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
