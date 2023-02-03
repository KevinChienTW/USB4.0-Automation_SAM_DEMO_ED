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
using System.Collections;








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
        string sideB_DD_1;

        string sideB_RX1_2;
        string sideB_TX1_2;
        string sideB_RX2_2;
        string sideB_TX2_2;
        string sideB_DD_2;

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
            Project_listview_set();
            Project_listview_2_set();
            //check_STA_files_B3();
            //Read_Result_1();
            //Read_Result_2();
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

            Project_list.Clear();
            Project_list_2.Clear();

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
                Project_listview_set();
                Project_listview_2_set();
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

            if (thread_for_Measure != null)
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
            DateTime T0 = DateTime.Now;
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

            if (test1.Checked || test2.Checked || test7.Checked || test8.Checked || test14.Checked || test15.Checked || test4.Checked || test5.Checked || test9.Checked || test16.Checked)//SS Pair ILfit、SCD12、SCD21、IMR、IRL
            {
                //#1     /B8#1
                test_item_i = 1;
                Switch("1B,3B,2B,4B");
                fixture_board_calibration_B3_Limit(sideA_TX1_1, sideB_RX1_2);
                single_and_save_forSS("TYPE1", "Default");
                if (test1.Checked || test14.Checked || test7.Checked || test8.Checked || test15.Checked || test17.Checked)//FD
                {
                    download_s4p_to_local();
                }
                if (test2.Checked)
                {
                    download_CSV_to_local();
                    Read_CSV();
                }
                //High_speed_test("1B,3B,2B,4B", "TXRX1");

                //#2    /B8#14
                test_item_i = 2;
                Switch("1C,3C,2C,4C");
                fixture_board_calibration_B3_Limit(sideA_RX1_1, sideB_TX1_2);
                single_and_save_forSS("TYPE1", "Default");
                if (test1.Checked || test14.Checked || test7.Checked || test8.Checked || test15.Checked || test17.Checked)//FD
                {
                    download_s4p_to_local();
                }
                if (test2.Checked)
                {
                    download_CSV_to_local();
                    Read_CSV();
                }

                //#16     /B8#23
                test_item_i = 16;
                Switch("1D,3D,2D,4D");
                fixture_board_calibration_B3_Limit(sideA_TX2_1, sideB_RX2_2);
                single_and_save_forSS("TYPE1", "Default");
                if (test1.Checked || test14.Checked || test7.Checked || test8.Checked || test15.Checked || test17.Checked)//FD
                {
                    download_s4p_to_local();
                }
                if (test2.Checked)
                {
                    download_CSV_to_local();
                    Read_CSV();
                }
                //High_speed_test("1D,3D,2D,4D", "TXRX2");

                //#17       /B8#28
                test_item_i = 17;
                Switch("1E,3E,2E,4E");
                fixture_board_calibration_B3_Limit(sideA_RX2_1, sideB_TX2_2);
                single_and_save_forSS("TYPE1", "Default");
                if (test1.Checked || test14.Checked || test7.Checked || test8.Checked || test15.Checked || test17.Checked)//FD
                {
                    download_s4p_to_local();
                }
                if (test2.Checked)
                {
                    download_CSV_to_local();
                    Read_CSV();
                }
                //High_speed_test("1E,3E,2E,4E", "TXRX2");

                //if (test4.Checked || test5.Checked || test9.Checked || test16.Checked)
                //{
                //    download_PNG_to_local();
                //}
            }

            if (test3.Checked || test11.Checked || test12.Checked || test13.Checked)//DD Pair_Attenuation
            {
                //3  
                test_item_i = 3;
                Switch("1F,3F,2F,4F");
                fixture_board_calibration_B3_Limit(sideA_DD_1, sideB_DD_2);
                single_and_save_forDD("TYPE2", "Default");
                if (test3.Checked)//FD
                {
                    download_s4p_to_local();
                }
                //download_PNG_to_local();
                ////18       
                //test_item_i = 18;
                //Switch("1F,3F,2F,4F");
                //fixture_board_calibration_B3(sideA_DD_1, sideB_DD_2);
                //single_and_save("TYPE2", "Default");
            }

            if (test18.Checked || test19.Checked)//INEXT and IFEXT、IDDXT_1NEXT +FEXT and, IDDXT_2NEXT
            {
                // #6  
                test_item_i = 6;
                Switch("1B,3B,2Fx,4Fx");
                fixture_board_calibration_B3(sideA_TX1_1, sideA_DD_2);
                single_and_save("TYPE2", "Default");
                download_s4p_to_local();
                // #7  
                test_item_i = 7;
                Switch("1C,3C,2Fx,4Fx");
                fixture_board_calibration_B3(sideA_RX1_1, sideA_DD_2);
                single_and_save("TYPE2", "Default");
                download_s4p_to_local();
                // #8  
                test_item_i = 8;
                Switch("1FX,3FX,2B,4B");
                fixture_board_calibration_B3(sideB_RX1_1, sideB_DD_2);
                single_and_save("TYPE2", "Default");
                download_s4p_to_local();
                // #9  
                test_item_i = 9;
                Switch("1Cx,3Cx,2F,4F");
                fixture_board_calibration_B3(sideB_TX1_1, sideB_DD_2);
                single_and_save("TYPE2", "Default");
                download_s4p_to_local();
                // #10   /B8#2
                test_item_i = 10;
                Switch("1B,3B,2C,4C");
                fixture_board_calibration_B3(sideA_TX1_1, sideB_TX1_2);
                single_and_save("TYPE2", "Default");
                download_s4p_to_local();
                //#4   /B8#5
                test_item_i = 4;//測試項
                Switch("1B,3B,2Cx,4Cx");//Switch切換組合
                fixture_board_calibration_B3(sideA_TX1_1, sideA_RX1_2);//加入所有校正檔案(包含DeEmbedd與calibration)
                single_and_save("TYPE2", "Default"); //設定畫面顯示、single、儲存檔案
                download_s4p_to_local();
                //#11   /B8#8
                test_item_i = 11;
                Switch("1C,3C,2B,4B"); //2B,4B,1C,3C
                fixture_board_calibration_B3(sideB_RX1_1, sideA_RX1_2);
                single_and_save("TYPE2", "Default");//"Reverse"
                download_s4p_to_local();
                // #5     /B8#11
                test_item_i = 5;
                Switch("1Cx,3Cx,2B,4B"); //3B,4B,3C,4C
                fixture_board_calibration_B3(sideB_RX1_1, sideB_TX1_2);
                single_and_save("TYPE2", "Default");
                download_s4p_to_local();
                // #13   /B8#30
                test_item_i = 13;
                Switch("1F,3F,2B,4B");
                fixture_board_calibration_B3(sideA_DD_1, sideB_RX1_2);
                single_and_save("TYPE2", "Default");
                download_s4p_to_local();
                // #12    /B8#32
                test_item_i = 12;
                Switch("1F,3F,2C,4C");
                fixture_board_calibration_B3(sideA_DD_1, sideB_TX1_2);
                single_and_save("TYPE2", "Default");
                download_s4p_to_local();
                // #14    /B8#37
                test_item_i = 14;
                Switch("1B,3B,2F,4F"); //3F,4F,1B,2B
                fixture_board_calibration_B3(sideB_DD_1, sideA_TX1_2);
                single_and_save("TYPE2", "Default");//"Reverse"
                download_s4p_to_local();
                // #15    /B8#39
                test_item_i = 15;
                Switch("1C,3C,2F,4F"); //3F,4F,1C,2C
                fixture_board_calibration_B3(sideB_DD_1, sideA_RX1_2);
                single_and_save("TYPE2", "Default");//"Reverse"
                download_s4p_to_local();
                //19       
                test_item_i = 19;
                Switch("1D,3D,2Ex,4Ex");
                fixture_board_calibration_B3(sideA_TX2_1, sideA_RX2_2);
                single_and_save("TYPE2", "Default");
                download_s4p_to_local();
                //21       
                test_item_i = 21;
                Switch("1D,3D,2Fx,4Fx");
                fixture_board_calibration_B3(sideA_TX2_1, sideA_DD_2);
                single_and_save("TYPE2", "Default");
                download_s4p_to_local();
                //22       
                test_item_i = 22;
                Switch("1E,3E,2Fx,4Fx");
                fixture_board_calibration_B3(sideA_RX2_1, sideA_DD_2);
                single_and_save("TYPE2", "Default");
                download_s4p_to_local();
                //23       
                test_item_i = 23;
                Switch("1Dx,3Dx,2F,4F");
                fixture_board_calibration_B3(sideB_RX2_1, sideB_DD_2);
                single_and_save("TYPE2", "Default");
                download_s4p_to_local();
                //24       
                test_item_i = 24;
                Switch("1Ex,3Ex,2F,4F");
                fixture_board_calibration_B3(sideB_TX2_1, sideB_DD_2);
                single_and_save("TYPE2", "Default");
                download_s4p_to_local();
                //20       /B8#27                          
                test_item_i = 20;
                Switch("1Ex,3Ex,2D,4D"); //3D,4D,3E,4E
                fixture_board_calibration_B3(sideB_RX2_1, sideB_TX2_2);
                single_and_save("TYPE2", "Default");//"Reverse"
                download_s4p_to_local();
                //#25        /B8#24
                test_item_i = 25;
                Switch("1D,3D,2E,4E");
                fixture_board_calibration_B3(sideA_TX2_1, sideB_TX2_2);
                single_and_save("TYPE2", "Default");
                download_s4p_to_local();
                //#26       /B8#26
                test_item_i = 26;
                Switch("1E,3E,2D,4D"); //3D,4D,1E,2E
                fixture_board_calibration_B3(sideB_RX2_1, sideA_RX2_2);
                single_and_save("TYPE2", "Default");//"Reverse"
                download_s4p_to_local();
                //27       /B8#36
                test_item_i = 27;
                Switch("1F,3F,2E,4E");
                fixture_board_calibration_B3(sideA_DD_1, sideB_TX2_2);
                single_and_save("TYPE2", "Default");
                download_s4p_to_local();

                //#28       /B8#34
                test_item_i = 28;
                Switch("1F,3F,2D,4D");
                fixture_board_calibration_B3(sideA_DD_1, sideB_RX2_2);
                single_and_save("TYPE2", "Default");
                download_s4p_to_local();
                //29       /B8#41
                test_item_i = 29;
                Switch("1D,3D,2F,4F"); //3F,4F,1D,2D
                fixture_board_calibration_B3(sideB_DD_1, sideA_TX2_2);
                single_and_save("TYPE2", "Default");//"Reverse"
                download_s4p_to_local();
                //30       /B8#43
                test_item_i = 30;
                Switch("1E,3E,2F,4F"); //3F,4F,1E,2E
                fixture_board_calibration_B3(sideB_DD_1, sideA_RX2_2);
                single_and_save("TYPE2", "Default");//"Reverse"
                download_s4p_to_local();
            }

            //DateTime Td0 = DateTime.Now;
            //download_s4p_to_local();
            //if (Save_plots.Checked)
            //{
            //    download_PNG_to_local();
            //}

            thread_for_stop = new Thread(stop_run);
            thread_for_stop.Start();
            DateTime Td1 = DateTime.Now;
            Console.WriteLine(Td1 - T0);
            ///////////////////////analys/////////////
            ///
            thread_for_analysis = new Thread(analysis_s4p);
            thread_for_analysis.Start();
            ///
            ///////////////////////////////////////

            ///////////////////////show result/////////////
            ///
            
            ///
            ///////////////////////////////////////
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
        void TDR_ENV_SET_DD()   // Kevin add for DD Use   2023/1/30 -S
        {

            check_DIF2_ENV();
            TDR_Query_response_value(":CALC1:ATR:MARK:COUP?", "0\n");
            TDR_Query_response_value(":CALC1:ATR:TIME:COUP?", "0\n");

            mbSession_E5071C.RawIO.Write(":CALC1:FSIM:STAT ON");                                           //Analysis --> fixture simulator --> Fixture Simulator(ON)
            mbSession_E5071C.RawIO.Write(":CALC1:FSIM:BAL:DEV BBAL");                                      //Analysis --> fixture simulator --> Topology -> Device(Bal-Bal)
            mbSession_E5071C.RawIO.Write(":CALC1:FSIM:BAL:TOP:BBAL 1,3,2,4");                              //Analysis --> fixture simulator --> Topology --> Port1(1-3) & Port2(2-4) 
            mbSession_E5071C.RawIO.Write(":CALC1:FSIM:BAL:PAR1:STAT ON");                                  //Analysis --> fixture simulator --> BalUn(OM)
            mbSession_E5071C.RawIO.Write(":CALC1:FSIM:BAL:PAR1:BBAL SDD21");


            wait_done("*OPC?");

            mbSession_E5071C.RawIO.Write(":CALC1:PAR:COUN 10");

            mbSession_E5071C_tdr.RawIO.Write(":SENS:DLEN:DATA 80e-10");//TDR/TDT > DUT Length
            mbSession_E5071C_tdr.RawIO.Write(":CALC1:TRAC1:TIME:STEP:RTIM:DATA 400e-12");
            //確認Trace1和Trace5 是否為Tdd11 和 Tdd22

            //setting  Trace1 和 Trace5
            TDR_Query_response_value(":CALC1:TRAC1:PAR?", "\"Tdd11\"\n");                                   //TDR/TDT --> Parameters
            TDR_Query_response_value(":CALC1:TRAC1:FORM?", "IMP\n");                                        //TDR/TDT --> Parameters --> Format
            TDR_Query_response_value(":CALC1:TRAC5:PAR?", "\"Tdd22\"\n");                                   //TDR/TDT --> Parameters
            TDR_Query_response_value(":CALC1:TRAC5:FORM?", "IMP\n");                                        //TDR/TDT --> Parameters --> Format

            //setting  Trace2 和 Trace6
            TDR_Query_response_value(":CALC1:TRAC2:PAR?", "\"T21\"\n");                                   //TDR/TDT --> Parameters
            TDR_Query_response_value(":CALC1:TRAC2:FORM?", "VOLT\n");                                        //TDR/TDT --> Parameters --> Format
            TDR_Query_response_value(":CALC1:TRAC6:PAR?", "\"T43\"\n");                                   //TDR/TDT --> Parameters                                      
            TDR_Query_response_value(":CALC1:TRAC6:FORM?", "VOLT\n");                                     //TDR/TDT --> Parameters --> Format

            //setting  Trace3 和 Trace7
            TDR_Query_response_value(":CALC1:TRAC3:PAR?", "\"Tdd21\"\n");                                   //TDR/TDT --> Parameters
            TDR_Query_response_value(":CALC1:TRAC3:FORM?", "IMP\n");                                        //TDR/TDT --> Parameters --> Format




            TDR_wait_done("*OPC?");
            wait_done("*OPC?");


            //Scal Trace1 和 Trace5
            mbSession_E5071C_tdr.RawIO.Write(":CALC1:TRAC1:TIME:STEP:RTIM:THR T2_8");
            mbSession_E5071C_tdr.RawIO.Write(":CALC1:TRAC1:TIME:STEP:RTIM:DATA 400e-12");

            mbSession_E5071C_tdr.RawIO.Write(":CALC1:TRAC5:TIME:STEP:RTIM:THR T2_8");
            mbSession_E5071C_tdr.RawIO.Write(":CALC1:TRAC5:TIME:STEP:RTIM:DATA 400e-12");


            TDR_wait_done("*OPC?");

            //Trace_1  DIV & STAR END
            mbSession_E5071C_tdr.RawIO.Write(":DISP:TRAC1:X:SCAL:PDIV 300e-11");                           //set value of x-axis scale per division
            mbSession_E5071C_tdr.RawIO.Write(":DISP:TRAC1:X:SCAL:RLEV -500e-11");                          //set value of x-axis reference line
            mbSession_E5071C_tdr.RawIO.Write(":DISP:TRAC1:Y:SCAL:PDIV 5");                                 //sets value of Y-axis scale per division
            mbSession_E5071C_tdr.RawIO.Write(":DISP:TRAC1:Y:SCAL:RLEV 70");

            //Trace_5  DIV & STAR END
            mbSession_E5071C_tdr.RawIO.Write(":DISP:TRAC5:X:SCAL:PDIV 300e-11");                           //set value of x-axis scale per division
            mbSession_E5071C_tdr.RawIO.Write(":DISP:TRAC5:X:SCAL:RLEV -500e-11");                          //set value of x-axis reference line
            mbSession_E5071C_tdr.RawIO.Write(":DISP:TRAC5:Y:SCAL:PDIV 5");                                 //sets value of Y-axis scale per division
            mbSession_E5071C_tdr.RawIO.Write(":DISP:TRAC5:Y:SCAL:RLEV 70");


            //Trace_2  DIV & STAR END
            mbSession_E5071C_tdr.RawIO.Write(":DISP:TRAC2:X:SCAL:PDIV 500e-11");                           //set value of x-axis scale per division
            mbSession_E5071C_tdr.RawIO.Write(":DISP:TRAC2:X:SCAL:RLEV -100e-11");                          //set value of x-axis reference line
            mbSession_E5071C_tdr.RawIO.Write(":DISP:TRAC2:Y:SCAL:PDIV 500e-4");                                 //sets value of Y-axis scale per division
            mbSession_E5071C_tdr.RawIO.Write(":DISP:TRAC2:Y:SCAL:RLEV 100e-3");

            //Trace_6  DIV & STAR END
            mbSession_E5071C_tdr.RawIO.Write(":DISP:TRAC6:X:SCAL:PDIV 500e-11");                           //set value of x-axis scale per division
            mbSession_E5071C_tdr.RawIO.Write(":DISP:TRAC6:X:SCAL:RLEV -100e-11");                          //set value of x-axis reference line
            mbSession_E5071C_tdr.RawIO.Write(":DISP:TRAC6:Y:SCAL:PDIV 500e-4");                                 //sets value of Y-axis scale per division
            mbSession_E5071C_tdr.RawIO.Write(":DISP:TRAC6:Y:SCAL:RLEV 100e-3");

            //Trace_3  DIV & STAR END
            mbSession_E5071C_tdr.RawIO.Write(":DISP:TRAC3:X:SCAL:PDIV 500e-11");                           //set value of x-axis scale per division
            mbSession_E5071C_tdr.RawIO.Write(":DISP:TRAC3:X:SCAL:RLEV -100e-11");                          //set value of x-axis reference line
            mbSession_E5071C_tdr.RawIO.Write(":DISP:TRAC3:Y:SCAL:PDIV 100e-3");                                 //sets value of Y-axis scale per division
            mbSession_E5071C_tdr.RawIO.Write(":DISP:TRAC3:Y:SCAL:RLEV 0");



            TDR_wait_done("*OPC?");

            // Off BZ
            mbSession_E5071C.RawIO.Write(":SYST:BEEP:COMP:STAT OFF");
            mbSession_E5071C.RawIO.Write(":SYST:BEEP:WARN:STAT OFF");
            //off BZ

            // Tr1、Tr5「IMPEDANCE」
            //CALC1 Tr1  Import IMP_LIMIT.csv
            mbSession_E5071C.RawIO.Write(":CALC1:PAR1:SEL");
            mbSession_E5071C.RawIO.Write(":DISP:WIND1:MAX ON");
            mbSession_E5071C.RawIO.Write(":MMEM:LOAD:LIM \"D:\\CAMS_Limitline\\IMP_LIMIT_DD_TR15.csv\"");                                                                                //Analysis > Limit Test > Edit Limit Line > Import from CSV File
            mbSession_E5071C.RawIO.Write(":CALC1:LIM ON");                                                                                                                       //Analysis > Limit Test > Limit Test
            mbSession_E5071C.RawIO.Write(":CALC1:LIM:DISP ON");                                                                                                                  //Analysis > Limit Test > Limit Line
            mbSession_E5071C.RawIO.Write(":DISP:FSIG OFF");                                                                                                                       //Analysis > Limit Test > Fail Sign
            wait_done("*OPC?");

            // CALC1 Tr5  Import IMP_LIMIT.csv
            mbSession_E5071C.RawIO.Write(":CALC1:PAR5:SEL");
            mbSession_E5071C.RawIO.Write(":DISP:WIND1:MAX ON");
            mbSession_E5071C.RawIO.Write(":MMEM:LOAD:LIM \"D:\\CAMS_Limitline\\IMP_LIMIT_DD_TR15.csv\"");                                                                                     //Analysis > Limit Test > Edit Limit Line > Import from CSV File
            mbSession_E5071C.RawIO.Write(":CALC1:LIM ON");                                                                                                                       //Analysis > Limit Test > Limit Test
            mbSession_E5071C.RawIO.Write(":CALC1:LIM:DISP ON");                                                                                                                  //Analysis > Limit Test > Limit Line
            mbSession_E5071C.RawIO.Write(":DISP:FSIG OFF");
            wait_done("*OPC?");

            //MARK1 Setting
            mbSession_E5071C.RawIO.Write(":CALC1:MARK1 ON");                                               //2022/09/21 Kevin add for --->	這一行是先選擇MARK1
            mbSession_E5071C.RawIO.Write(":CALC1:MARK1: FUNC:TYPE MAX");                                   //2022/09/21 Kevin add for --->	這一行是將 MARK1 設定為 MAX
            mbSession_E5071C.RawIO.Write(":CALC1:MARK1:FUNC:TRAC ON");                                     //2022/09/21 Kevin add for --->	這一行是將MARK1設定為追蹤
            mbSession_E5071C.RawIO.Write(":CALC1:MARK:FUNC:DOM ON");                                       //2022/09/21 Kevin add for --->	這一行是設定Search Range(ON)

            //MARK2 Setting
            mbSession_E5071C.RawIO.Write(":CALC1:MARK2 ON");                                               //2022/09/21 Kevin add for --->	這一行是先選擇MARK1
            mbSession_E5071C.RawIO.Write(":CALC1:MARK2: FUNC:TYPE MIN");                                   //2022/09/21 Kevin add for --->	這一行是將 MARK1 設定為 MAX
            mbSession_E5071C.RawIO.Write(":CALC1:MARK2:FUNC:TRAC ON");                                     //2022/09/21 Kevin add for --->	這一行是將MARK1設定為追蹤
            mbSession_E5071C.RawIO.Write(":CALC1:MARK:FUNC:DOM ON");                                       //2022/09/21 Kevin add for --->	這一行是設定Search Range(ON)

            //Tr2、Tr6 「SKEW」
            mbSession_E5071C.RawIO.Write(":CALC1:ATR:ACT 2");                                               //2022/09/21 Kevin add for ---> 這段是設定起始為 Tr2
            mbSession_E5071C.RawIO.Write(":CALC:TRAC2:DTIM:STAT ON");                                       //2022/09/21 Kevin add for ---> 這段有可能是將delta time 按鈕勾選起來
            mbSession_E5071C.RawIO.Write(":CALC:TRAC2:DTIM:TARG 6");                                        //2022/09/21 Kevin add for ---> 這一段是設定 Tr2 及 Tr6 作相減
            mbSession_E5071C.RawIO.Write(":CALC:TRAC2:DTIM:POS 50");                                        //2022/09/21 Kevin add for ---> 這一段是設定 position 為50
            mbSession_E5071C.RawIO.Write(":TRIG:SING");                                                     //2022/09/21 Kevin add for --->	
            mbSession_E5071C.RawIO.Write(":CALC:TRAC2:DTIM:DATA?");                                         //2022/09/21 Kevin add for --->	//:CALCulate:TRACe{Tr}:DTIMe:DATA  //This command gets delta time result value. You can get the result even if :CALCulate:TRACe{Tr}:DTIMe:STATe is off.

            mbSession_E5071C.RawIO.Write(":DISP:WIND1:MAX ON");
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC3:MARK1 ON");
            mbSession_E5071C.RawIO.Write(":CALC1:MARK:FUNC:DOM OFF");
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC3:MARK1:FUNC:TRAC ON");                                   //MKR/ANALYSIS --> Marker Search --> Track(on)
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC3:MARK1:FUNC:TYPE TARG");                                 //MKR/ANALYSIS --> Marker Search --> Search Target
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC3:MARK1:FUNC:TARG 200e-3");                               //MKR/ANALYSIS --> Marker Search --> Target value
            wait_done("*OPC?");


            //以下作TR2 及TR6的公式設定
            mbSession_E5071C.RawIO.Write(":SYST:BEEP:COMP:STAT OFF");
            mbSession_E5071C.RawIO.Write(":SYST:BEEP:WARN:STAT OFF");

            mbSession_E5071C_tdr.RawIO.Write(":CALC:ATR:ACT 2");
            mbSession_E5071C_tdr.RawIO.Write(":DISP:TRAC2:X:SCAL:RLEV -3e-9");
            TDR_wait_done("*OPC?");

            mbSession_E5071C.RawIO.Write(":DISP:WIND1:MAX ON");
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
            Thread.Sleep(2000);




        }   // Kevin add for DD Use   2023/1/30 -E
        void analysis_s4p()
        {
            string para = "demo_code\\demo_code_t.exe";

            string folder_location = Store_folder_location.Replace(".//", "./");
            folder_location = folder_location.Replace("//", "\\");

            string input_path = " -i \"" + folder_location + "\\" + "( " + Dut_name.Text + " )\"";

            string output_path = " -o \"" + folder_location + "\\" + "( " + Dut_name.Text + " )\"";

            string plots;
            string ILF;
            string IMR;
            string IRL;
            string SCD12;
            string SCD21;
            string SDC21;
            string DD;
            string INEXT;
            string ID2;
            if (test1.Checked)
            {
                ILF = " --getilfitatnq";
            }
            else
            {
                ILF = "";
            }

            if (test14.Checked)
            {
                IMR = " --getimr";
            }
            else
            {
                IMR = "";
            }

            if (test15.Checked)
            {
                IRL = " --getirl";
            }
            else
            {
                IRL = "";
            }

            if (test7.Checked)
            {
                SCD12 = " --getscd12";
            }
            else
            {
                SCD12 = "";
            }

            if (test8.Checked)
            {
                SCD21 = " --getscd21";
            }
            else
            {
                SCD21 = "";
            }

            if (test17.Checked)
            {
                SDC21 = " --getsdc21";
            }
            else
            {
                SDC21 = "";
            }

            if (test3.Checked)
            {
                DD = " --getddil";
            }
            else
            {
                DD = "";
            }

            if (test18.Checked)
            {
                INEXT = " --getinext --getifext";
            }
            else
            {
                INEXT = "";
            }

            if (test19.Checked)
            {
                ID2 = " --getiddxt2ne --getiddxt1ne1fe";
            }
            else
            {
                ID2 = "";
            }

            if (Save_plots.Checked)
            {
                plots = " --saveplots";
            }
            else
            {
                plots = "";
            }

            string parameter = para + input_path + output_path + ILF + IMR + IRL + SCD12 + SCD21 + SDC21 + DD + INEXT + ID2 + plots;

            p.StartInfo.FileName = "cmd.exe";

            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.CreateNoWindow = true; //不跳出cmd視窗
            //p.StartInfo.Arguments = parameter;
            p.Start();
            p.StandardInput.WriteLine(parameter);
            if (test18.Checked || test19.Checked)
            {
                Thread.Sleep(10000);
            }
            else
            {
                Thread.Sleep(3500);
            }
            
            this.Invoke((MethodInvoker)delegate ()
            {
                Read_Result_1();
                Read_Result_2();
            });
            
            p.WaitForExit();

            p.Close();

            if (p != null)
            {
                p.WaitForExit();
                p.Close();

                p.Dispose();
                //p = null;
            }
            //GetRead();
            
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
                //DateTime T0 = DateTime.Now;
                ////Load .STA
                ///
                mbSession_E5071C.RawIO.Write(":MMEM:LOAD \"D:\\CAMS_STA\\B3\\" + switch_ports + ".STA\"");
                
                wait_done("*OPC?");
                //TDR_ENV_SET_DD();
                /////    keivn test -s
                ///
                //DateTime zero = DateTime.Now;
                //Console.WriteLine(zero - T0);

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
                //DateTime T2 = DateTime.Now;
                //Console.WriteLine("limit = ", T2 - zero);

                ////Environment_setting();
                ////Load .s2p
                mbSession_E5071C.RawIO.Write(":CALC2:FSIM:SEND:DEEM:STAT ON");
                mbSession_E5071C.RawIO.Write(":CALC2:FSIM:SEND:DEEM:PORT1:USER:FIL " + "\"" + FB_P1P3 + "\""); //:CALC1:FSIM:SEND:DEEM:PORT1 USER
                mbSession_E5071C.RawIO.Write(":CALC2:FSIM:SEND:DEEM:PORT2:USER:FIL " + "\"" + FB_P1P3 + "\"");
                mbSession_E5071C.RawIO.Write(":CALC2:FSIM:SEND:DEEM:PORT3:USER:FIL " + "\"" + FB_P2P4 + "\"");
                mbSession_E5071C.RawIO.Write(":CALC2:FSIM:SEND:DEEM:PORT4:USER:FIL " + "\"" + FB_P2P4 + "\"");
               
                wait_done("*OPC?");
                //DateTime T3 = DateTime.Now;
                //Console.WriteLine(T3 - T2);
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
            string LIMIT_FILE = iniManager.ReadIniFile("LIMIT", "LIMIT_FILE", "");
            string SS_PDI_LL = iniManager.ReadIniFile("LIMIT", "SS_PDI_LL", "");
            string CI_LL = iniManager.ReadIniFile("LIMIT", "CI_LL", "");
            string SS_SK_L = iniManager.ReadIniFile("LIMIT", "SS_SK_L", "");
            string DD_DI_LL = iniManager.ReadIniFile("LIMIT", "DD_DI_LL", "");
            string DD_DL_L = iniManager.ReadIniFile("LIMIT", "DD_DL_L", "");
            string DD_PS_L = iniManager.ReadIniFile("LIMIT", "DD_PS_L", "");
            string SS_SEI_LL = iniManager.ReadIniFile("LIMIT", "SS_SEI_LL", "");

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
                /////////////////FD LIMIT SETTING/////////////////
                //mbSession_E5071C.RawIO.Write(":DISP:WIND2:ACT");
                //mbSession_E5071C.RawIO.Write(":DISP:MAX ON");
                //mbSession_E5071C.RawIO.Write(":CALC1:PAR1:SEL");
                //mbSession_E5071C.RawIO.Write(":DISP:WIND2:MAX ON");

                //mbSession_E5071C.RawIO.Write(":CALC2:LIM ON");                                                                                                                       //Analysis > Limit Test > Limit Test
                //mbSession_E5071C.RawIO.Write(":CALC2:LIM:DISP ON");


                //mbSession_E5071C.RawIO.Write(":MMEM:LOAD:LIM \"D:\\CAMS_Limitline\\"+LIMIT_FILE+"\"");
                //wait_done("*OPC?");

                //mbSession_E5071C.RawIO.Write(":CALC1:LIM ON");                                                                                                                       //Analysis > Limit Test > Limit Test
                //mbSession_E5071C.RawIO.Write(":CALC1:LIM:DISP ON");                                                                                                                  //Analysis > Limit Test > Limit Line
                //mbSession_E5071C.RawIO.Write(":DISP:FSIG OFF");
                //wait_done("*OPC?");

                ///////////////////////////////////////////////////
                ///////////////TDR LIMIT SETTING/////////////////////
                if (test4.Checked)//SS Pair Differential Impedance trace1,5
                {
                    mbSession_E5071C.RawIO.Write(":DISP:WIND2:MAX OFF");
                    mbSession_E5071C.RawIO.Write(":DISP:WIND1:ACT");
                    mbSession_E5071C.RawIO.Write(":CALC1:PAR1:SEL");
                    mbSession_E5071C.RawIO.Write(":MMEM:LOAD:LIM \"D:\\CAMS_Limitline\\" + SS_PDI_LL + "\"");
                    wait_done("*OPC?");
                    mbSession_E5071C.RawIO.Write(":CALC1:LIM ON");                                                                                                                       //Analysis > Limit Test > Limit Test
                    mbSession_E5071C.RawIO.Write(":CALC1:LIM:DISP ON");                                                                                                                  //Analysis > Limit Test > Limit Line
                    mbSession_E5071C.RawIO.Write(":DISP:FSIG OFF");
                    wait_done("*OPC?");


                    mbSession_E5071C.RawIO.Write(":DISP:WIND1:MAX OFF");
                    mbSession_E5071C.RawIO.Write(":CALC1:PAR5:SEL");
                    mbSession_E5071C.RawIO.Write(":MMEM:LOAD:LIM \"D:\\CAMS_Limitline\\" + SS_PDI_LL + "\"");
                    wait_done("*OPC?");
                    mbSession_E5071C.RawIO.Write(":CALC1:LIM ON");                                                                                                                       //Analysis > Limit Test > Limit Test
                    mbSession_E5071C.RawIO.Write(":CALC1:LIM:DISP ON");                                                                                                                  //Analysis > Limit Test > Limit Line
                    mbSession_E5071C.RawIO.Write(":DISP:FSIG OFF");
                    wait_done("*OPC?");

                }
                if (test5.Checked)//SS Pair Single End Impedance(  L / R ) trace4,8,9,10
                {
                    mbSession_E5071C.RawIO.Write(":DISP:WIND2:MAX OFF");
                    mbSession_E5071C.RawIO.Write(":DISP:WIND1:ACT");
                    mbSession_E5071C.RawIO.Write(":CALC1:PAR4:SEL");
                    mbSession_E5071C.RawIO.Write(":MMEM:LOAD:LIM \"D:\\CAMS_Limitline\\" + SS_SEI_LL + "\"");
                    wait_done("*OPC?");
                    mbSession_E5071C.RawIO.Write(":CALC1:LIM ON");                                                                                                                       //Analysis > Limit Test > Limit Test
                    mbSession_E5071C.RawIO.Write(":CALC1:LIM:DISP ON");                                                                                                                  //Analysis > Limit Test > Limit Line
                    mbSession_E5071C.RawIO.Write(":DISP:FSIG OFF");
                    wait_done("*OPC?");


                    mbSession_E5071C.RawIO.Write(":DISP:WIND1:MAX OFF");
                    mbSession_E5071C.RawIO.Write(":CALC1:PAR8:SEL");
                    mbSession_E5071C.RawIO.Write(":MMEM:LOAD:LIM \"D:\\CAMS_Limitline\\" + SS_SEI_LL + "\"");
                    wait_done("*OPC?");
                    mbSession_E5071C.RawIO.Write(":CALC1:LIM ON");                                                                                                                       //Analysis > Limit Test > Limit Test
                    mbSession_E5071C.RawIO.Write(":CALC1:LIM:DISP ON");                                                                                                                  //Analysis > Limit Test > Limit Line
                    mbSession_E5071C.RawIO.Write(":DISP:FSIG OFF");
                    wait_done("*OPC?");


                    mbSession_E5071C.RawIO.Write(":DISP:WIND1:MAX OFF");
                    mbSession_E5071C.RawIO.Write(":CALC1:PAR9:SEL");
                    mbSession_E5071C.RawIO.Write(":MMEM:LOAD:LIM \"D:\\CAMS_Limitline\\" + SS_SEI_LL + "\"");
                    wait_done("*OPC?");
                    mbSession_E5071C.RawIO.Write(":CALC1:LIM ON");                                                                                                                       //Analysis > Limit Test > Limit Test
                    mbSession_E5071C.RawIO.Write(":CALC1:LIM:DISP ON");                                                                                                                  //Analysis > Limit Test > Limit Line
                    mbSession_E5071C.RawIO.Write(":DISP:FSIG OFF");
                    wait_done("*OPC?");


                    mbSession_E5071C.RawIO.Write(":DISP:WIND1:MAX OFF");
                    mbSession_E5071C.RawIO.Write(":CALC1:PAR10:SEL");
                    mbSession_E5071C.RawIO.Write(":MMEM:LOAD:LIM \"D:\\CAMS_Limitline\\" + SS_SEI_LL + "\"");
                    wait_done("*OPC?");
                    mbSession_E5071C.RawIO.Write(":CALC1:LIM ON");                                                                                                                       //Analysis > Limit Test > Limit Test
                    mbSession_E5071C.RawIO.Write(":CALC1:LIM:DISP ON");                                                                                                                  //Analysis > Limit Test > Limit Line
                    mbSession_E5071C.RawIO.Write(":DISP:FSIG OFF");
                    wait_done("*OPC?");

                }
                if (test9.Checked)//Connector Impedance_( L / R ) trace3,7
                {
                    mbSession_E5071C.RawIO.Write(":DISP:WIND2:MAX OFF");
                    mbSession_E5071C.RawIO.Write(":DISP:WIND1:ACT");
                    mbSession_E5071C.RawIO.Write(":CALC1:PAR3:SEL");
                    mbSession_E5071C.RawIO.Write(":MMEM:LOAD:LIM \"D:\\CAMS_Limitline\\" + CI_LL + "\"");
                    wait_done("*OPC?");
                    mbSession_E5071C.RawIO.Write(":CALC1:LIM ON");                                                                                                                       //Analysis > Limit Test > Limit Test
                    mbSession_E5071C.RawIO.Write(":CALC1:LIM:DISP ON");                                                                                                                  //Analysis > Limit Test > Limit Line
                    mbSession_E5071C.RawIO.Write(":DISP:FSIG OFF");
                    wait_done("*OPC?");


                    mbSession_E5071C.RawIO.Write(":DISP:WIND1:MAX OFF");
                    mbSession_E5071C.RawIO.Write(":CALC1:PAR7:SEL");
                    mbSession_E5071C.RawIO.Write(":MMEM:LOAD:LIM \"D:\\CAMS_Limitline\\" + CI_LL + "\"");
                    wait_done("*OPC?");
                    mbSession_E5071C.RawIO.Write(":CALC1:LIM ON");                                                                                                                       //Analysis > Limit Test > Limit Test
                    mbSession_E5071C.RawIO.Write(":CALC1:LIM:DISP ON");                                                                                                                  //Analysis > Limit Test > Limit Line
                    mbSession_E5071C.RawIO.Write(":DISP:FSIG OFF");
                    wait_done("*OPC?");

                }
                if (test16.Checked)//SS Pair Intra Pair Skew trace2
                {
                    mbSession_E5071C.RawIO.Write(":DISP:WIND2:MAX OFF");
                    mbSession_E5071C.RawIO.Write(":DISP:WIND1:ACT");
                    mbSession_E5071C.RawIO.Write(":CALC1:PAR2:SEL");
                    mbSession_E5071C.RawIO.Write(":MMEM:LOAD:LIM \"D:\\CAMS_Limitline\\" + SS_SK_L + "\"");
                    wait_done("*OPC?");
                    mbSession_E5071C.RawIO.Write(":CALC1:LIM ON");                                                                                                                       //Analysis > Limit Test > Limit Test
                    mbSession_E5071C.RawIO.Write(":CALC1:LIM:DISP ON");                                                                                                                  //Analysis > Limit Test > Limit Line
                    mbSession_E5071C.RawIO.Write(":DISP:FSIG OFF");
                    wait_done("*OPC?");
                }
                if (test11.Checked)//DD Pair_Differential Impedance trace1,5
                {
                    mbSession_E5071C.RawIO.Write(":DISP:WIND2:MAX OFF");
                    mbSession_E5071C.RawIO.Write(":DISP:WIND1:ACT");
                    mbSession_E5071C.RawIO.Write(":CALC1:PAR1:SEL");
                    mbSession_E5071C.RawIO.Write(":MMEM:LOAD:LIM \"D:\\CAMS_Limitline\\" + DD_DI_LL + "\"");
                    wait_done("*OPC?");
                    mbSession_E5071C.RawIO.Write(":CALC1:LIM ON");                                                                                                                       //Analysis > Limit Test > Limit Test
                    mbSession_E5071C.RawIO.Write(":CALC1:LIM:DISP ON");                                                                                                                  //Analysis > Limit Test > Limit Line
                    mbSession_E5071C.RawIO.Write(":DISP:FSIG OFF");
                    wait_done("*OPC?");

                    mbSession_E5071C.RawIO.Write(":DISP:WIND1:MAX OFF");
                    mbSession_E5071C.RawIO.Write(":CALC1:PAR5:SEL");
                    mbSession_E5071C.RawIO.Write(":MMEM:LOAD:LIM \"D:\\CAMS_Limitline\\" + DD_DI_LL + "\"");
                    wait_done("*OPC?");
                    mbSession_E5071C.RawIO.Write(":CALC1:LIM ON");                                                                                                                       //Analysis > Limit Test > Limit Test
                    mbSession_E5071C.RawIO.Write(":CALC1:LIM:DISP ON");                                                                                                                  //Analysis > Limit Test > Limit Line
                    mbSession_E5071C.RawIO.Write(":DISP:FSIG OFF");
                    wait_done("*OPC?");

                }
                if (test12.Checked)//DD Pair_Propagation Delay trace3
                {
                    mbSession_E5071C.RawIO.Write(":DISP:WIND2:MAX OFF");
                    mbSession_E5071C.RawIO.Write(":DISP:WIND1:ACT");
                    mbSession_E5071C.RawIO.Write(":CALC1:PAR3:SEL");
                    mbSession_E5071C.RawIO.Write(":MMEM:LOAD:LIM \"D:\\CAMS_Limitline\\" + DD_DL_L + "\"");
                    wait_done("*OPC?");
                    mbSession_E5071C.RawIO.Write(":CALC1:LIM ON");                                                                                                                       //Analysis > Limit Test > Limit Test
                    mbSession_E5071C.RawIO.Write(":CALC1:LIM:DISP ON");                                                                                                                  //Analysis > Limit Test > Limit Line
                    mbSession_E5071C.RawIO.Write(":DISP:FSIG OFF");
                    wait_done("*OPC?");

                }
                if (test13.Checked)//DD Pair_Intra Pair Skew trace2
                {
                    mbSession_E5071C.RawIO.Write(":DISP:WIND2:MAX OFF");
                    mbSession_E5071C.RawIO.Write(":DISP:WIND1:ACT");
                    mbSession_E5071C.RawIO.Write(":CALC1:PAR2:SEL");
                    mbSession_E5071C.RawIO.Write(":MMEM:LOAD:LIM \"D:\\CAMS_Limitline\\" + DD_PS_L + "\"");
                    wait_done("*OPC?");
                    mbSession_E5071C.RawIO.Write(":CALC1:LIM ON");                                                                                                                       //Analysis > Limit Test > Limit Test
                    mbSession_E5071C.RawIO.Write(":CALC1:LIM:DISP ON");                                                                                                                  //Analysis > Limit Test > Limit Line
                    mbSession_E5071C.RawIO.Write(":DISP:FSIG OFF");
                    wait_done("*OPC?");

                }
                /////////////////////////////////////////////
                ///

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
            //DateTime T0 = DateTime.Now;
            test_item = "ITEM" + test_item_i.ToString();
            string name;

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
                    if (test1.Checked || test14.Checked || test7.Checked || test8.Checked || test15.Checked || test17.Checked)//FD
                    {
                        mbSession_E5071C.RawIO.Write(":CALC2:PAR:COUN 1");
                        mbSession_E5071C.RawIO.Write(":TRIG:SING");
                        wait_single_done("*OPC?");
                    }
                    if (test4.Checked || test5.Checked || test9.Checked || test16.Checked)//TD
                    {
                        mbSession_E5071C.RawIO.Write(":DISP:WIND1:ACT");
                        mbSession_E5071C.RawIO.Write(":INIT1:CONT ON");
                        mbSession_E5071C.RawIO.Write(":TRIG:SING");
                        wait_single_done("*OPC?");
                    }
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

                if (test1.Checked || test14.Checked || test7.Checked || test8.Checked || test15.Checked || test17.Checked)
                {
                    mbSession_E5071C.RawIO.Write(":MMEM:STOR:SNP \"D:\\CAMS_RESULT\\B3\\" + "( " + Dut_name.Text.ToUpper() + " )" + "\\" + test_item + ".s4p\"");
                    wait_done("*OPC?");
                    Thread.Sleep(50);

                    mbSession_E5071C.RawIO.Write(":CALC2:PAR:COUN 1"); //必須在存檔.s4p之後才可以執行，因為.s4p存檔需要single當下的所有trace才可以存檔。
                    wait_done("*OPC?");
                    Thread.Sleep(50);
                }

                if (test2.Checked)
                {
                    mbSession_E5071C.RawIO.Write(":MMEMory:STORe:FDATa \"D:\\CAMS_RESULT\\B3\\" + "( " + Dut_name.Text.ToUpper() + " )" + "\\" + test_item + ".csv\"");
                    wait_done("*OPC?");
                    Thread.Sleep(50);
                }

                if (test4.Checked)//SS Pair Differential Impedance trace1,5
                {
                    mbSession_E5071C.RawIO.Write(":DISP:WIND2:MAX OFF");
                    mbSession_E5071C.RawIO.Write(":DISP:WIND1:ACT");
                    mbSession_E5071C.RawIO.Write(":CALC1:PAR1:SEL");
                    mbSession_E5071C.RawIO.Write(":DISP:WIND1:MAX ON");
                    Thread.Sleep(50);
                    mbSession_E5071C.RawIO.Write(":MMEMory:STORe:FDATa \"D:\\CAMS_RESULT\\B3\\" + "( " + Dut_name.Text.ToUpper() + " )" + "\\" + test_item + "_1.csv\"");
                    wait_done("*OPC?");
                    Thread.Sleep(50);
                    mbSession_E5071C.RawIO.Write(":MMEM:STOR:IMAG \"D:\\CAMS_RESULT\\B3\\" + "( " + Dut_name.Text.ToUpper() + " )" + "\\" + test_item + "_1.png\"");
                    wait_done("*OPC?");
                    Thread.Sleep(50);

                    //name = test_item + "_SS_Pair_Differential_Impedance_1";
                    //download_PNG_to_local(name);



                    mbSession_E5071C.RawIO.Write(":DISP:WIND1:MAX OFF");
                    mbSession_E5071C.RawIO.Write(":CALC1:PAR5:SEL");
                    mbSession_E5071C.RawIO.Write(":DISP:WIND1:MAX ON");
                    Thread.Sleep(50);
                    mbSession_E5071C.RawIO.Write(":MMEMory:STORe:FDATa \"D:\\CAMS_RESULT\\B3\\" + "( " + Dut_name.Text.ToUpper() + " )" + "\\" + test_item + "_5.csv\"");
                    wait_done("*OPC?");
                    Thread.Sleep(50);
                    mbSession_E5071C.RawIO.Write(":MMEM:STOR:IMAG \"D:\\CAMS_RESULT\\B3\\" + "( " + Dut_name.Text.ToUpper() + " )" + "\\" + test_item + "_5.png\"");
                    wait_done("*OPC?");
                    Thread.Sleep(50);

                    mbSession_E5071C.RawIO.Write(":MMEM:STOR:SCR 'R:PICTURE.GIF'");
                    

                    //download_PNG_to_local();
                }
                if (test5.Checked)//SS Pair Single End Impedance(  L / R ) trace4,8,9,10
                {
                    mbSession_E5071C.RawIO.Write(":DISP:WIND2:MAX OFF");
                    mbSession_E5071C.RawIO.Write(":DISP:WIND1:ACT");
                    mbSession_E5071C.RawIO.Write(":CALC1:PAR4:SEL");
                    mbSession_E5071C.RawIO.Write(":DISP:WIND1:MAX ON");
                    Thread.Sleep(50);
                    mbSession_E5071C.RawIO.Write(":MMEMory:STORe:FDATa \"D:\\CAMS_RESULT\\B3\\" + "( " + Dut_name.Text.ToUpper() + " )" + "\\" + test_item + "_4.csv\"");
                    wait_done("*OPC?");
                    Thread.Sleep(50);
                    mbSession_E5071C.RawIO.Write(":MMEM:STOR:IMAG \"D:\\CAMS_RESULT\\B3\\" + "( " + Dut_name.Text.ToUpper() + " )" + "\\" + test_item + "_4.png\"");
                    wait_done("*OPC?");
                    Thread.Sleep(50);
                   

                    mbSession_E5071C.RawIO.Write(":DISP:WIND1:MAX OFF");
                    mbSession_E5071C.RawIO.Write(":CALC1:PAR8:SEL");
                    mbSession_E5071C.RawIO.Write(":DISP:WIND1:MAX ON");
                    Thread.Sleep(50);
                    mbSession_E5071C.RawIO.Write(":MMEMory:STORe:FDATa \"D:\\CAMS_RESULT\\B3\\" + "( " + Dut_name.Text.ToUpper() + " )" + "\\" + test_item + "_8.csv\"");
                    wait_done("*OPC?");
                    Thread.Sleep(50);
                    mbSession_E5071C.RawIO.Write(":MMEM:STOR:IMAG \"D:\\CAMS_RESULT\\B3\\" + "( " + Dut_name.Text.ToUpper() + " )" + "\\" + test_item + "_8.png\"");
                    wait_done("*OPC?");
                    Thread.Sleep(50);
                    

                    mbSession_E5071C.RawIO.Write(":DISP:WIND1:MAX OFF");
                    mbSession_E5071C.RawIO.Write(":CALC1:PAR9:SEL");
                    mbSession_E5071C.RawIO.Write(":DISP:WIND1:MAX ON");
                    Thread.Sleep(50);
                    mbSession_E5071C.RawIO.Write(":MMEMory:STORe:FDATa \"D:\\CAMS_RESULT\\B3\\" + "( " + Dut_name.Text.ToUpper() + " )" + "\\" + test_item + "_9.csv\"");
                    wait_done("*OPC?");
                    Thread.Sleep(50);
                    mbSession_E5071C.RawIO.Write(":MMEM:STOR:IMAG \"D:\\CAMS_RESULT\\B3\\" + "( " + Dut_name.Text.ToUpper() + " )" + "\\" + test_item + "_9.png\"");
                    wait_done("*OPC?");
                    Thread.Sleep(50);
                    

                    mbSession_E5071C.RawIO.Write(":DISP:WIND1:MAX OFF");
                    mbSession_E5071C.RawIO.Write(":CALC1:PAR10:SEL");
                    mbSession_E5071C.RawIO.Write(":DISP:WIND1:MAX ON");
                    Thread.Sleep(50);
                    mbSession_E5071C.RawIO.Write(":MMEMory:STORe:FDATa \"D:\\CAMS_RESULT\\B3\\" + "( " + Dut_name.Text.ToUpper() + " )" + "\\" + test_item + "_10.csv\"");
                    wait_done("*OPC?");
                    Thread.Sleep(50);
                    mbSession_E5071C.RawIO.Write(":MMEM:STOR:IMAG \"D:\\CAMS_RESULT\\B3\\" + "( " + Dut_name.Text.ToUpper() + " )" + "\\" + test_item + "_10.png\"");
                    wait_done("*OPC?");
                    Thread.Sleep(50);
                    
                }
                if (test9.Checked)//Connector Impedance_( L / R ) trace3,7
                {
                    mbSession_E5071C.RawIO.Write(":DISP:WIND2:MAX OFF");
                    mbSession_E5071C.RawIO.Write(":DISP:WIND1:ACT");
                    mbSession_E5071C.RawIO.Write(":CALC1:PAR3:SEL");
                    mbSession_E5071C.RawIO.Write(":DISP:WIND1:MAX ON");
                    Thread.Sleep(50);
                    mbSession_E5071C.RawIO.Write(":MMEM:STOR:IMAG \"D:\\CAMS_RESULT\\B3\\" + "( " + Dut_name.Text.ToUpper() + " )" + "\\" + test_item + "_3.png\"");
                    wait_done("*OPC?");
                    Thread.Sleep(50);
                    mbSession_E5071C.RawIO.Write(":MMEMory:STORe:FDATa \"D:\\CAMS_RESULT\\B3\\" + "( " + Dut_name.Text.ToUpper() + " )" + "\\" + test_item + "_3.csv\"");
                    wait_done("*OPC?");
                    Thread.Sleep(50);

                    mbSession_E5071C.RawIO.Write(":DISP:WIND1:MAX OFF");
                    mbSession_E5071C.RawIO.Write(":CALC1:PAR7:SEL");
                    mbSession_E5071C.RawIO.Write(":DISP:WIND1:MAX ON");
                    Thread.Sleep(50);
                    mbSession_E5071C.RawIO.Write(":MMEM:STOR:IMAG \"D:\\CAMS_RESULT\\B3\\" + "( " + Dut_name.Text.ToUpper() + " )" + "\\" + test_item + "_7.png\"");
                    wait_done("*OPC?");
                    Thread.Sleep(50);
                    mbSession_E5071C.RawIO.Write(":MMEMory:STORe:FDATa \"D:\\CAMS_RESULT\\B3\\" + "( " + Dut_name.Text.ToUpper() + " )" + "\\" + test_item + "_7.csv\"");
                    wait_done("*OPC?");
                    Thread.Sleep(50);
                }
                if (test16.Checked)//SS Pair Intra Pair Skew trace2
                {
                    mbSession_E5071C.RawIO.Write(":DISP:WIND2:MAX OFF");
                    mbSession_E5071C.RawIO.Write(":DISP:WIND1:ACT");
                    mbSession_E5071C.RawIO.Write(":CALC1:PAR2:SEL");
                    mbSession_E5071C.RawIO.Write(":DISP:WIND1:MAX ON");
                    Thread.Sleep(50);
                    mbSession_E5071C.RawIO.Write(":MMEM:STOR:IMAG \"D:\\CAMS_RESULT\\B3\\" + "( " + Dut_name.Text.ToUpper() + " )" + "\\" + test_item + "_2.png\"");
                    wait_done("*OPC?");
                    Thread.Sleep(50);
                    mbSession_E5071C.RawIO.Write(":MMEMory:STORe:FDATa \"D:\\CAMS_RESULT\\B3\\" + "( " + Dut_name.Text.ToUpper() + " )" + "\\" + test_item + "_2.csv\"");
                    wait_done("*OPC?");
                    Thread.Sleep(50);
                }


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
        void single_and_save_forDD(string scale, string switch_type)///20220822如果B2可以共用function，處新增
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
                    if (test3.Checked)
                    {
                        mbSession_E5071C.RawIO.Write(":CALC2:PAR:COUN 1");
                        mbSession_E5071C.RawIO.Write(":TRIG:SING");
                        wait_single_done("*OPC?");
                    }
                    if (test11.Checked || test12.Checked || test13.Checked)
                    {
                        mbSession_E5071C.RawIO.Write(":DISP:WIND1:ACT");
                        mbSession_E5071C.RawIO.Write(":INIT1:CONT ON");
                        mbSession_E5071C.RawIO.Write(":TRIG:SING");
                        wait_single_done("*OPC?");
                    }

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

                //mbSession_E5071C.RawIO.Write(":MMEM:STOR:SNP \"D:\\CAMS_RESULT\\B3\\" + "( " + Dut_name.Text.ToUpper() + " )" + "\\" + test_item + ".s4p\"");
                //wait_done("*OPC?");
                //Thread.Sleep(50);

                //mbSession_E5071C.RawIO.Write(":CALC2:PAR:COUN 1"); //必須在存檔.s4p之後才可以執行，因為.s4p存檔需要single當下的所有trace才可以存檔。
                //wait_done("*OPC?");
                //Thread.Sleep(50);

                if (test3.Checked)
                {
                    mbSession_E5071C.RawIO.Write(":MMEM:STOR:SNP \"D:\\CAMS_RESULT\\B3\\" + "( " + Dut_name.Text.ToUpper() + " )" + "\\" + test_item + ".s4p\"");
                    wait_done("*OPC?");
                    Thread.Sleep(50);

                    mbSession_E5071C.RawIO.Write(":CALC2:PAR:COUN 1"); //必須在存檔.s4p之後才可以執行，因為.s4p存檔需要single當下的所有trace才可以存檔。
                    wait_done("*OPC?");
                    Thread.Sleep(50);
                }
                if (test11.Checked)//DD Pair_Differential Impedance trace1,5
                {
                    mbSession_E5071C.RawIO.Write(":DISP:WIND2:MAX OFF");
                    mbSession_E5071C.RawIO.Write(":DISP:WIND1:ACT");
                    mbSession_E5071C.RawIO.Write(":CALC1:PAR1:SEL");
                    mbSession_E5071C.RawIO.Write(":DISP:WIND1:MAX ON");
                    Thread.Sleep(50);
                    mbSession_E5071C.RawIO.Write(":MMEM:STOR:IMAG \"D:\\CAMS_RESULT\\B3\\" + "( " + Dut_name.Text.ToUpper() + " )" + "\\" + test_item + "_DD_1.png\"");
                    wait_done("*OPC?");
                    Thread.Sleep(50);
                    mbSession_E5071C.RawIO.Write(":MMEMory:STORe:FDATa \"D:\\CAMS_RESULT\\B3\\" + "( " + Dut_name.Text.ToUpper() + " )" + "\\" + test_item + "_DD_1.csv\"");
                    wait_done("*OPC?");
                    Thread.Sleep(50);

                    mbSession_E5071C.RawIO.Write(":DISP:WIND1:MAX OFF");
                    mbSession_E5071C.RawIO.Write(":CALC1:PAR5:SEL");
                    mbSession_E5071C.RawIO.Write(":DISP:WIND1:MAX ON");
                    Thread.Sleep(50);
                    mbSession_E5071C.RawIO.Write(":MMEM:STOR:IMAG \"D:\\CAMS_RESULT\\B3\\" + "( " + Dut_name.Text.ToUpper() + " )" + "\\" + test_item + "_DD_5.png\"");
                    wait_done("*OPC?");
                    Thread.Sleep(50);
                    mbSession_E5071C.RawIO.Write(":MMEMory:STORe:FDATa \"D:\\CAMS_RESULT\\B3\\" + "( " + Dut_name.Text.ToUpper() + " )" + "\\" + test_item + "_DD_5.csv\"");
                    wait_done("*OPC?");
                    Thread.Sleep(50);
                }
                if (test12.Checked)//DD Pair_Propagation Delay trace3
                {
                    mbSession_E5071C.RawIO.Write(":DISP:WIND2:MAX OFF");
                    mbSession_E5071C.RawIO.Write(":DISP:WIND1:ACT");
                    mbSession_E5071C.RawIO.Write(":CALC1:PAR3:SEL");
                    mbSession_E5071C.RawIO.Write(":DISP:WIND1:MAX ON");
                    Thread.Sleep(50);
                    mbSession_E5071C.RawIO.Write(":MMEM:STOR:IMAG \"D:\\CAMS_RESULT\\B3\\" + "( " + Dut_name.Text.ToUpper() + " )" + "\\" + test_item + "_DD_3.png\"");
                    wait_done("*OPC?");
                    Thread.Sleep(50);
                    mbSession_E5071C.RawIO.Write(":MMEMory:STORe:FDATa \"D:\\CAMS_RESULT\\B3\\" + "( " + Dut_name.Text.ToUpper() + " )" + "\\" + test_item + "_DD_3.csv\"");
                    wait_done("*OPC?");
                    Thread.Sleep(50);
                }
                if (test13.Checked)//DD Pair_Intra Pair Skew trace2
                {
                    mbSession_E5071C.RawIO.Write(":DISP:WIND2:MAX OFF");
                    mbSession_E5071C.RawIO.Write(":DISP:WIND1:ACT");
                    mbSession_E5071C.RawIO.Write(":CALC1:PAR2:SEL");
                    mbSession_E5071C.RawIO.Write(":DISP:WIND1:MAX ON");
                    Thread.Sleep(50);
                    mbSession_E5071C.RawIO.Write(":MMEM:STOR:IMAG \"D:\\CAMS_RESULT\\B3\\" + "( " + Dut_name.Text.ToUpper() + " )" + "\\" + test_item + "_DD_2.png\"");
                    wait_done("*OPC?");
                    Thread.Sleep(50);
                    mbSession_E5071C.RawIO.Write(":MMEMory:STORe:FDATa \"D:\\CAMS_RESULT\\B3\\" + "( " + Dut_name.Text.ToUpper() + " )" + "\\" + test_item + "_DD_2.csv\"");
                    wait_done("*OPC?");
                    Thread.Sleep(50);
                }

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

        public class Global
        {
            public static string RawData_10G;
            public static string RawData_5G;
            public static string RawData_2d5G;
        }
        void Read_CSV()
        {
            String line;
            String[] OrgData = new string[1004];
            string RawData_2d5G;
            string RawData_5G;
            string RawData_10G;
            int ReadCount = 1002;

            try
            {
                //Pass the file path and file name to the StreamReader constructor
                //StreamReader sr = new StreamReader("D:\\ty.txt");
                StreamReader sr = new StreamReader(Store_folder_location + "\\" + "( " + Dut_name.Text + " )" + "\\" + test_previous_item + ".csv");
                //Read the first line of text
                line = sr.ReadLine();
                //Continue to read until you reach end of file


                while (line != null)
                {
                    for (int row = 0; row <= ReadCount; row++)
                    {
                        //Console.WriteLine(line);
                        line = sr.ReadLine();
                        OrgData[row] = line;

                        if (row == 252)
                        {
                            RawData_2d5G = OrgData[row].Substring(20, 20);
                            Global.RawData_2d5G = RawData_2d5G.Substring(0, 10);
                        }

                        if (row == 502)
                        {
                            RawData_5G = OrgData[row].Substring(20, 20);
                            Global.RawData_5G = RawData_5G.Substring(0, 10);
                        }

                        if (row == 1000)
                        {
                            RawData_10G = OrgData[row].Substring(20, 20);
                            int test;
                            float test_2;

                            test = Convert.ToInt32(GetLastCode(RawData_10G));

                            if (test == 1)
                            {
                                //test_2 = (Convert.ToInt32(RawData_10G.Substring(0,8)))*10;
                                Global.RawData_10G = RawData_10G.Substring(0, 10);
                                //test_2 = Convert.ToInt64(RawData_10G);
                            }

                        }

                    }
                    break;
                }

                //close the file
                sr.Close();
                //Console.ReadLine();

                // write to txt

                StreamWriter sw = new StreamWriter(Store_folder_location + "\\" + "( " + Dut_name.Text + " )" + "\\" + test_previous_item + ".txt");
                sw.WriteLine(Global.RawData_10G);
                sw.WriteLine(Global.RawData_5G);
                sw.WriteLine(Global.RawData_2d5G);
                sw.Close();

                string GetLastCode(string _str)
                {
                    _str = _str.Substring(_str.Length - 1, 1);
                    return _str;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
            finally
            {
                Console.WriteLine("Executing finally block.");
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
            //download_s4p_to_local(); //下載.S4P檔
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
        void download_CSV_to_local() //下載前一個single完的.CSV檔案
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
                        System.IO.StreamWriter sw = new System.IO.StreamWriter(Store_folder_location + "\\" + "( " + Dut_name.Text + " )" + "\\" + test_previous_item + ".csv");

                        mbSession_E5071C.RawIO.Write(":MMEM:TRAN? \"D:\\CAMS_RESULT\\B3\\" + "( " + Dut_name.Text.ToUpper() + " )" + "\\" + test_previous_item + ".csv\"");

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
                MessageBox.Show(exp.Message + "\n Download " + test_previous_item + ".csv" + " Fail");
            }

        }
        void download_PNG_to_local() //下載前一個single完的.PNG檔案
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
                        System.IO.StreamWriter sw = new System.IO.StreamWriter(Store_folder_location + "\\" + "( " + Dut_name.Text + " )" + "\\" + test_item +  ".bmp");

                        mbSession_E5071C.RawIO.Write(":MMEM:TRAN? \"D:\\CAMS_RESULT\\B3\\" + "( " + Dut_name.Text.ToUpper() + " )" + "\\" + test_item + ".bmp\"");

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
                MessageBox.Show(exp.Message + "\n Download " + test_previous_item + ".png" + " Fail");
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
            if (test1.Checked || test2.Checked || test3.Checked )
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

            }
        }
        private void Project_listview_set()
        {
            //ListView顯示方式
            Project_list.View = View.Details;

            Project_list.BeginUpdate();
            #region 增加Item的標題，共有三個列
            //1、創建標題
            //Project_list.Columns.Add("Test Name");
            //Project_list.Columns.Add("Value");
            //Project_list.Columns.Add("Criteria");
            //Project_list.Columns.Add("Result");

            Project_list.Columns.Add("Tx1Rx1");
            Project_list.Columns.Add("");
            Project_list.Columns.Add("");
            Project_list.Columns.Add("");

            Project_list.GridLines = true;
            Project_list.GridLines = true;
            #endregion
            Project_list.EndUpdate();

        }
        private void Project_listview_2_set()
        {
            //ListView顯示方式
            Project_list_2.View = View.Details;

            Project_list_2.BeginUpdate();
            #region 增加Item的標題，共有三個列
            //1、創建標題
            //Project_list.Columns.Add("Test Name");
            //Project_list.Columns.Add("Value");
            //Project_list.Columns.Add("Criteria");
            //Project_list.Columns.Add("Result");

            Project_list_2.Columns.Add("Tx2Rx2");
            Project_list_2.Columns.Add("");
            Project_list_2.Columns.Add("");
            Project_list_2.Columns.Add("");

            Project_list_2.GridLines = true;
            Project_list_2.GridLines = true;
            #endregion
            Project_list_2.EndUpdate();

        }
        void Read_Result_1()//TX1RX1
        {
            int P_count = 0;
            int F_count = 0;
            try
            {
                using (StreamReader SR = new StreamReader(Store_folder_location + "\\" + "( " + Dut_name.Text + " )" + "\\summary_1.txt"))//using (StreamReader SR = new StreamReader(@"D:\USB4.0_B3_forCELink\USB4.0 Automation\USB4.0 Automation\bin\Debug\data\20230131\( limit_test )\summary_1.txt"))
                {
                    string Line;
                    while ((Line = SR.ReadLine()) != null)
                    {
                        string[] ReadLine_Array = Line.Split('\t');
                        //if (cgio)
                        //{
                        //    textBox2.Text = ReadLine_Array[0];
                        //    textBox3.Text = ReadLine_Array[1];
                        //    textBox4.Text = ReadLine_Array[2];
                        //    cgio = false;
                        //}
                        ListViewItem itm = new ListViewItem(ReadLine_Array);
                        //Project_list.Items.Add(itm);

                        if (Line.Contains("Pass"))
                        {
                            Project_list.Items.Add(itm).ForeColor = Color.Green;
                            P_count = P_count + 1;
                        }
                        else if (Line.Contains("Fail"))
                        {
                            Project_list.Items.Add(itm).ForeColor = Color.Red;
                            F_count = F_count + 1;
                        }
                        else
                        {
                            Project_list.Items.Add(itm).ForeColor = Color.Black;
                        }
                    }
                }
                Project_list.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
                Project_list.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                label4.Text = P_count.ToString();
                label6.Text = F_count.ToString();
            }
            catch (IOException)
            { 
            
            }
            catch (NullReferenceException)
            {
            
            }
            catch (FormatException)
            { 
            
            }
        }
        void Read_Result_2()//TX2RX2
        {
            int P_count = 0;
            int F_count = 0;
            try
            {
                using (StreamReader SR = new StreamReader(Store_folder_location + "\\" + "( " + Dut_name.Text + " )" + "\\summary_2.txt"))//using (StreamReader SR = new StreamReader(@"D:\USB4.0_B3_forCELink\USB4.0 Automation\USB4.0 Automation\bin\Debug\data\20230131\( limit_test )\summary_2.txt"))
                {
                    string Line;
                    while ((Line = SR.ReadLine()) != null)
                    {
                        string[] ReadLine_Array = Line.Split('\t');
                        //if (cgio)
                        //{
                        //    textBox2.Text = ReadLine_Array[0];
                        //    textBox3.Text = ReadLine_Array[1];
                        //    textBox4.Text = ReadLine_Array[2];
                        //    cgio = false;
                        //}
                        ListViewItem itm = new ListViewItem(ReadLine_Array);
                        //Project_list.Items.Add(itm);

                        if (Line.Contains("Pass"))
                        {
                            Project_list_2.Items.Add(itm).ForeColor = Color.Green;
                            P_count = P_count + 1;
                        }
                        else if (Line.Contains("Fail"))
                        {
                            Project_list_2.Items.Add(itm).ForeColor = Color.Red;
                            F_count = F_count + 1;
                        }
                        else
                        {
                            Project_list_2.Items.Add(itm).ForeColor = Color.Black;
                        }

                    }
                }
                Project_list_2.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
                Project_list_2.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                label_pass_num.Text = P_count.ToString();
                label_fail_num.Text = F_count.ToString();
            }
            catch (IOException)
            {

            }
            catch (NullReferenceException)
            {

            }
            catch (FormatException)
            {

            }
        }
        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Project_list_2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
