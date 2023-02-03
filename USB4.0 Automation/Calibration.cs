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
using System.IO;
using System.IO.Ports;

namespace USB4._0_Automation
{
    public partial class calibration : Form
    {
        const int SIDE_A_1B2B = 0;
        const int SIDE_A_1C2C = 10;
        const int SIDE_A_1D2D = 20;
        const int SIDE_A_1E2E = 30;
        const int SIDE_A_1F2F = 40;

        const int SIDE_B_3F4F = 50;
        const int SIDE_B_3E4E = 60;
        const int SIDE_B_3D4D = 70;
        const int SIDE_B_3C4C = 80;

        public string B2_1B3B2CX4CX_TMP;

        int[] calibration_done = new int[100];
        int calibration_done_i = 0;
        int switch_Port1_Port2 = 0;

        private MessageBasedSession mbSession_E5071C;
        private MessageBasedSession mbSession_E5071C_tdr;

        string E5071C_Resource;
        string Switch_Resource;

        string switch_port_name = "";
        string switch_port1_2_name;

        Color select_button_color;
        Thread thread_for_calibrating;
        Thread thread_for_stop;

        IniManager iniCalibration = new IniManager(System.Environment.CurrentDirectory + "\\" + "calibration_sta.ini"); //設定ini檔路

        bool double_click_event = false;
        string lock_selection_assemble = "";

        Form4 form_4 = new Form4();
        public calibration()
        {
            InitializeComponent();
        }
        private void calibration_Load(object sender, EventArgs e)
        {
            Ecal1B2B.Select();
            calibration_start.Enabled = false;
            if (Program.test_type == "B8" || Program.test_type == "B3")//20221017 Add
            {
                Ecal1D2D_onlyB2.Visible = false;
            }
            else
            {
                Ecal1D2D_onlyB2.Visible = true;
            }

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


            string value = iniCalibration.ReadIniFile("STA", "calibration_done[0]", "");
            if (value == "")
            {
                string now_path = System.Windows.Forms.Application.StartupPath + "\\";
                StreamWriter sw = new StreamWriter(now_path + "calibration_sta.ini", false);
                //Write a line of text
                sw.WriteLine("[STA]");
                //Write a second line of text
                int i = 0;
                foreach (int calibration_item in calibration_done)
                {
                    sw.WriteLine("calibration_done[" + i + "]=" + calibration_item.ToString());
                    i++;
                }
                //Close the file
                sw.Close();
            }

            for (int i = 0; i < 99; i++)
            {
                value = iniCalibration.ReadIniFile("STA", "calibration_done[" + i.ToString() + "]", "");
                calibration_done[i] = Int32.Parse(value);
            }
            Calibration_items_done();

            if (Program.test_type == "B3")
            {
                Ecal3B4B.Enabled = true;
            }
            else
            {
                Ecal3B4B.Enabled = false;
            }

        }

        private void calibration_formClosing(object sender, FormClosingEventArgs e)
        {
            stop_calibration("none");
        }

        private void Ecal1B2B_Click(object sender, EventArgs e)
        {
            if (Lock_selection.Checked == true)
            {
                //switch_port_name = switch_port1_2_name + "," + Ecal1B2B.Text; //Switch指令
                //calibration_done_i = switch_Port1_Port2 + 0;
                //Reset_button_state(); //將所有按鈕恢復到原本的顏色
                //Calibration_sub_items_done(); //針對(switch_Port1_Port2),X,X的組合(switch_Port1_Port2 為使用者在勾選Lock Selection之前，點選的按鈕名稱，如(1B,2B))，如果已經做完的就會直接上色
                //Ecal1B2B.BackColor = Color.FromArgb(0, 68, 109); 
            }
            else
            {
                switch_port1_2_name = "";
                switch_port1_2_name = Ecal1B2B.Text;
                Reset_button_state(); //將所有按鈕恢復到原本的顏色
                Calibration_items_done(); //將已經測試完的按鈕直接上色
                Ecal1B2B.FlatAppearance.BorderSize = 2;
                Ecal1B2B.FlatAppearance.BorderColor = Color.White;
                select_button_color = Ecal1B2B.BackColor; //記錄當下選擇的顏色
                switch_Port1_Port2 = SIDE_A_1B2B;
            }

            if (double_click_event && !Lock_selection.Checked)
            {
                Lock_selection.Checked = true;
                lock_selection_assemble = Ecal1B2B.Text;
            }
            else if (double_click_event && Lock_selection.Checked && Ecal1B2B.Text == lock_selection_assemble)
            {
                Lock_selection.Checked = false;
            }
            double_click_event = true;
            click_timer.Enabled = true;
        }

        private void Ecal1C2C_Click(object sender, EventArgs e)
        {
            if (Lock_selection.Checked == true)
            {
                switch_port_name = switch_port1_2_name + "," + "2Cx,4Cx";
                calibration_done_i = switch_Port1_Port2 + 1;
                Reset_button_state();
                Calibration_sub_items_done();
                Ecal1C2C.BackColor = Color.FromArgb(0, 68, 109);
            }
            else
            {
                switch_port1_2_name = "";
                switch_port1_2_name = Ecal1C2C.Text;
                Reset_button_state();
                Calibration_items_done();
                Ecal1C2C.FlatAppearance.BorderSize = 2;
                Ecal1C2C.FlatAppearance.BorderColor = Color.White;
                select_button_color = Ecal1C2C.BackColor;
                switch_Port1_Port2 = SIDE_A_1C2C;
            }

            if (double_click_event && !Lock_selection.Checked)
            {
                Lock_selection.Checked = true;
                lock_selection_assemble = Ecal1C2C.Text;
            }
            else if (double_click_event && Lock_selection.Checked && Ecal1C2C.Text == lock_selection_assemble)
            {
                Lock_selection.Checked = false;
            }
            double_click_event = true;
            click_timer.Enabled = true;
        }

        private void Ecal1D2D_Click(object sender, EventArgs e)
        {
            if (Lock_selection.Checked == true)
            {
                switch_port_name = switch_port1_2_name + "," + "2Dx,4Dx";
                calibration_done_i = switch_Port1_Port2 + 2;
                Reset_button_state();
                Calibration_sub_items_done();
                Ecal1D2D.BackColor = Color.FromArgb(0, 68, 109);
            }
            else
            {
                switch_port1_2_name = "";
                switch_port1_2_name = Ecal1D2D.Text;
                Reset_button_state();
                Calibration_items_done();
                Ecal1D2D.FlatAppearance.BorderSize = 2;
                Ecal1D2D.FlatAppearance.BorderColor = Color.White;
                select_button_color = Ecal1D2D.BackColor;
                switch_Port1_Port2 = SIDE_A_1D2D;
            }

            if (double_click_event && !Lock_selection.Checked)
            {
                Lock_selection.Checked = true;
                lock_selection_assemble = Ecal1D2D.Text;
            }
            else if (double_click_event && Lock_selection.Checked && Ecal1D2D.Text == lock_selection_assemble)
            {
                Lock_selection.Checked = false;
            }
            double_click_event = true;
            click_timer.Enabled = true;

        }

        private void Ecal1D2D_onlyB2_Click(object sender, EventArgs e)
        {
            if (Lock_selection.Checked == true)
            {
                switch_port_name = switch_port1_2_name + "," + "2Dx,4Dx";
                calibration_done_i = switch_Port1_Port2 + 2;
                Reset_button_state();
                Calibration_sub_items_done();
                Ecal1D2D.BackColor = Color.FromArgb(0, 68, 109);
            }
            else
            {
                switch_port1_2_name = "";
                switch_port1_2_name = Ecal1D2D.Text;
                Reset_button_state();
                Calibration_items_done();
                Ecal1D2D.FlatAppearance.BorderSize = 2;
                Ecal1D2D.FlatAppearance.BorderColor = Color.White;
                select_button_color = Ecal1D2D.BackColor;
                switch_Port1_Port2 = SIDE_A_1D2D;
            }

            if (double_click_event && !Lock_selection.Checked)
            {
                Lock_selection.Checked = true;
                lock_selection_assemble = Ecal1D2D.Text;
            }
            else if (double_click_event && Lock_selection.Checked && Ecal1D2D.Text == lock_selection_assemble)
            {
                Lock_selection.Checked = false;
            }
            double_click_event = true;
            click_timer.Enabled = true;
        }

        private void Ecal1E2E_Click(object sender, EventArgs e)
        {

            if (Lock_selection.Checked == true)
            {
                switch_port_name = switch_port1_2_name + "," + "2Ex,4Ex";
                calibration_done_i = switch_Port1_Port2 + 3;
                Reset_button_state();
                Calibration_sub_items_done();
                Ecal1E2E.BackColor = Color.FromArgb(0, 68, 109);
            }
            else
            {

                switch_port1_2_name = "";
                switch_port1_2_name = Ecal1E2E.Text;
                Reset_button_state();
                Calibration_items_done();
                Ecal1E2E.FlatAppearance.BorderSize = 2;
                Ecal1E2E.FlatAppearance.BorderColor = Color.White;
                select_button_color = Ecal1E2E.BackColor;
                switch_Port1_Port2 = SIDE_A_1E2E;
            }

            if (double_click_event && !Lock_selection.Checked)
            {
                Lock_selection.Checked = true;
                lock_selection_assemble = Ecal1E2E.Text;
            }
            else if (double_click_event && Lock_selection.Checked && Ecal1E2E.Text == lock_selection_assemble)
            {
                Lock_selection.Checked = false;
            }
            double_click_event = true;
            click_timer.Enabled = true;
        }

        private void Ecal1F2F_Click(object sender, EventArgs e)
        {

            if (Lock_selection.Checked == true)
            {
                switch_port_name = switch_port1_2_name + "," + "2Fx,4Fx";
                calibration_done_i = switch_Port1_Port2 + 4;
                Reset_button_state();
                Calibration_sub_items_done();
                Ecal1F2F.BackColor = Color.FromArgb(0, 68, 109);
            }
            else
            {
                switch_port1_2_name = "";
                switch_port1_2_name = Ecal1F2F.Text;
                Reset_button_state();
                Calibration_items_done();
                Ecal1F2F.FlatAppearance.BorderSize = 2;
                Ecal1F2F.FlatAppearance.BorderColor = Color.White;
                select_button_color = Ecal1F2F.BackColor;
                switch_Port1_Port2 = SIDE_A_1F2F;
            }

            if (double_click_event && !Lock_selection.Checked)
            {
                Lock_selection.Checked = true;
                lock_selection_assemble = Ecal1F2F.Text;
            }
            else if (double_click_event && Lock_selection.Checked && Ecal1F2F.Text == lock_selection_assemble)
            {
                Lock_selection.Checked = false;
            }
            double_click_event = true;
            click_timer.Enabled = true;

        }

        private void Ecal3B4B_Click(object sender, EventArgs e)
        {

            if (Lock_selection.Checked == true)
            {
                switch_port_name = switch_port1_2_name + "," + Ecal3B4B.Text;
                calibration_done_i = switch_Port1_Port2 + 9;
                Reset_button_state();
                Calibration_sub_items_done();
                Ecal3B4B.BackColor = Color.FromArgb(0, 68, 109);

            }
            else
            {
                switch_Port1_Port2 = 90;
                switch_port1_2_name = "";
                switch_port1_2_name = Ecal3B4B.Text;
                Reset_button_state();
                Calibration_items_done();
                Ecal3B4B.FlatAppearance.BorderSize = 2;
                Ecal3B4B.FlatAppearance.BorderColor = Color.White;
                select_button_color = Ecal3B4B.BackColor;

            }
            if (Program.test_type == "B3")
            {
                if (double_click_event && !Lock_selection.Checked)
                {
                    Lock_selection.Checked = true;
                    lock_selection_assemble = Ecal3B4B.Text;
                }
                else if (double_click_event && Lock_selection.Checked && Ecal3B4B.Text == lock_selection_assemble)
                {
                    Lock_selection.Checked = false;
                }
                double_click_event = true;
                click_timer.Enabled = true;
            }


        }

        private void Ecal3C4C_Click(object sender, EventArgs e)
        {

            if (Lock_selection.Checked == true)
            {
                switch_port_name = switch_port1_2_name + "," + Ecal3C4C.Text;
                calibration_done_i = switch_Port1_Port2 + 8;
                Reset_button_state();
                Calibration_sub_items_done();
                Ecal3C4C.BackColor = Color.FromArgb(0, 68, 109);
            }
            else
            {
                switch_port1_2_name = "";
                switch_port1_2_name = "1Cx,3Cx";
                Reset_button_state();
                Calibration_items_done();
                Ecal3C4C.FlatAppearance.BorderSize = 2;
                Ecal3C4C.FlatAppearance.BorderColor = Color.White;
                select_button_color = Ecal3C4C.BackColor;
                switch_Port1_Port2 = SIDE_B_3C4C;
            }

            if (double_click_event && !Lock_selection.Checked)
            {
                Lock_selection.Checked = true;
                lock_selection_assemble = Ecal3C4C.Text;
            }
            else if (double_click_event && Lock_selection.Checked && Ecal3C4C.Text == lock_selection_assemble)
            {
                Lock_selection.Checked = false;
            }
            double_click_event = true;
            click_timer.Enabled = true;
        }

        private void Ecal3D4D_Click(object sender, EventArgs e)
        {
            if (Lock_selection.Checked == true)
            {
                switch_port_name = switch_port1_2_name + "," + Ecal3D4D.Text;
                calibration_done_i = switch_Port1_Port2 + 7;
                Reset_button_state();
                Calibration_sub_items_done();
                Ecal3D4D.BackColor = Color.FromArgb(0, 68, 109);
            }
            else
            {
                switch_port1_2_name = "";
                switch_port1_2_name = "1Dx,3Dx";
                Reset_button_state();
                Calibration_items_done();
                Ecal3D4D.FlatAppearance.BorderSize = 2;
                Ecal3D4D.FlatAppearance.BorderColor = Color.White;
                select_button_color = Ecal3D4D.BackColor;
                switch_Port1_Port2 = SIDE_B_3D4D;
            }

            if (double_click_event && !Lock_selection.Checked)
            {
                Lock_selection.Checked = true;
                lock_selection_assemble = Ecal3D4D.Text;
            }
            else if (double_click_event && Lock_selection.Checked && Ecal3D4D.Text == lock_selection_assemble)
            {
                Lock_selection.Checked = false;
            }
            double_click_event = true;
            click_timer.Enabled = true;
        }

        private void Ecal3E4E_Click(object sender, EventArgs e)
        {
            if (Lock_selection.Checked == true)
            {
                switch_port_name = switch_port1_2_name + "," + Ecal3E4E.Text;
                calibration_done_i = switch_Port1_Port2 + 6;
                Reset_button_state();
                Calibration_sub_items_done();
                Ecal3E4E.BackColor = Color.FromArgb(0, 68, 109);
            }
            else
            {
                switch_port1_2_name = "";
                switch_port1_2_name = "1Ex,3Ex";
                Reset_button_state();
                Calibration_items_done();
                Ecal3E4E.FlatAppearance.BorderSize = 2;
                Ecal3E4E.FlatAppearance.BorderColor = Color.White;
                select_button_color = Ecal3E4E.BackColor;
                switch_Port1_Port2 = SIDE_B_3E4E;
            }

            if (double_click_event && !Lock_selection.Checked)
            {
                Lock_selection.Checked = true;
                lock_selection_assemble = Ecal3E4E.Text;
            }
            else if (double_click_event && Lock_selection.Checked && Ecal3E4E.Text == lock_selection_assemble)
            {
                Lock_selection.Checked = false;
            }
            double_click_event = true;
            click_timer.Enabled = true;
        }

        private void Ecal3F4F_Click(object sender, EventArgs e)
        {
            if (Lock_selection.Checked == true)
            {
                switch_port_name = switch_port1_2_name + "," + Ecal3F4F.Text;
                calibration_done_i = switch_Port1_Port2 + 5;
                Reset_button_state();
                Calibration_sub_items_done();
                Ecal3F4F.BackColor = Color.FromArgb(0, 68, 109);
            }
            else
            {
                switch_port1_2_name = "";
                switch_port1_2_name = "1Fx,3Fx";
                Reset_button_state();
                Calibration_items_done();
                Ecal3F4F.FlatAppearance.BorderSize = 2;
                Ecal3F4F.FlatAppearance.BorderColor = Color.White;
                select_button_color = Ecal3F4F.BackColor;
                switch_Port1_Port2 = SIDE_B_3F4F;
            }

            if (double_click_event && !Lock_selection.Checked)
            {
                Lock_selection.Checked = true;
                lock_selection_assemble = Ecal3F4F.Text;
            }
            else if (double_click_event && Lock_selection.Checked && Ecal3F4F.Text == lock_selection_assemble)
            {
                Lock_selection.Checked = false;
            }
            double_click_event = true;
            click_timer.Enabled = true;
        }

        //按下Calibration的按鈕-----------------------------------------------------------------
        private void calibration_start_Click(object sender, EventArgs e)
        {
            if (calibration_start.Enabled == true)
            {
                calibration_start.BackgroundImage = Properties.Resources.calibrating_button_disable;
                var rmSession = new ResourceManager();

                try
                {
                    if (!serialPort1.IsOpen)
                    {
                        serialPort1.Open(); //連線到Switch
                    }
                    mbSession_E5071C = (MessageBasedSession)rmSession.Open(E5071C_Resource); //連線到E5071C
                    mbSession_E5071C.TimeoutMilliseconds = 1000;

                    try
                    {
                        mbSession_E5071C_tdr = (MessageBasedSession)rmSession.Open("TCPIP0::127.0.0.1::inst0::INSTR");

                        try
                        {
                            thread_for_calibrating = new Thread(calibating);
                            thread_for_calibrating.Start();
                            calibration_start.Enabled = false;
                            timer1.Enabled = false;
                            form_4 = new Form4();
                            form_4.ShowDialog();
                            timer1.Enabled = true;
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
                        stop_calibration("none");
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
                thread_for_stop = new Thread(stop);
                thread_for_stop.Start();
            }

        }
        private void calibration_start_mouseDown(object sender, MouseEventArgs e)
        {
            calibration_start.BackgroundImage = Properties.Resources.button02;
            calibration_start.ForeColor = Color.White;
        }
        private void calibration_start_mouseMove(object sender, MouseEventArgs e)
        {
            calibration_start.BackgroundImage = Properties.Resources.button02;
            calibration_start.ForeColor = Color.White;
        }
        private void calibration_start_mouseLeave(object sender, EventArgs e)
        {
            calibration_start.BackgroundImage = Properties.Resources.button01;
            calibration_start.ForeColor = Color.Black;
        }
        private void calibration_start_mouseaUp(object sender, MouseEventArgs e)
        {
            calibration_start.BackgroundImage = Properties.Resources.button01;
            calibration_start.ForeColor = Color.Black;
        }
        /////////////////////////////////////////////////////////////////////////////
        void stop()
        {
            stop_calibration("none");
        }

        //終止calibration時的動作-----------------------------------------------------------------
        void stop_calibration(string exception_message)
        {
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

            if (exception_message != "none")
            {
                MessageBox.Show(exception_message);
            }

            //中斷執行執行緒
            if (thread_for_calibrating != null)
            {
                if (thread_for_calibrating.IsAlive)
                {
                    thread_for_calibrating.Abort();
                }
            }
        }
        /////////////////////////////////////////////////////////////////////////////
        //執行calibration時，使用多執行緒執行-----------------------------------------------------------------
        void calibating()
        {
            try
            {
                Switch(switch_port_name + ",");

                B2_1B3B2CX4CX_TMP = switch_port_name;
                mbSession_E5071C.RawIO.Write("*CLS");
                mbSession_E5071C.RawIO.Write(":DISP:MAX OFF");
                mbSession_E5071C.RawIO.Write(":DISP:WIND2:ACT");
                mbSession_E5071C.RawIO.Write(":CALC2:FSIM:SEND:DEEM:STAT OFF");
                wait_done("*OPC?");

                mbSession_E5071C.RawIO.Write(":SENS2:CORR:COLL:ECAL:SOLT4 1,2,3,4");
                wait_done("*OPC?");
                mbSession_E5071C.RawIO.Write(":SYST:ERR?");
                string err_message = mbSession_E5071C.RawIO.ReadString();

                if (err_message == "+32,\"ECal module not in appropriate RF path\"\n")
                {
                    calibration_done[calibration_done_i] = 0;
                    this.Invoke((MethodInvoker)delegate ()
                    {
                        calibration_start.Enabled = Enabled;
                    });
                    MessageBox.Show("ECal module not in appropriate RF path");
                }
                else
                {
                    if (Program.test_type == "B8" || Program.test_type == "B3")
                    {
                        if (switch_port_name == "1B,3B,2B,4B" || switch_port_name == "1C,3C,2C,4C" || switch_port_name == "1D,3D,2D,4D" || switch_port_name == "1E,3E,2E,4E" || switch_port_name == "1F,3F,2F,4F")
                        {
                            mbSession_E5071C.RawIO.Write(":DISP:WIND1:ACT");
                            if (switch_port_name == "1F,3F,2F,4F")
                            {
                                TDR_ENV_SET_DD();
                            }
                            mbSession_E5071C_tdr.RawIO.Write(":SENS:CORR:COLL:ECAL:IMM");
                            TDR_wait_done("*OPC?");
                        }

                    }
                    else if (Program.test_type == "B2")
                    {

                        if (switch_port_name == "1B,3B,2Cx,4Cx")
                        {
                            mbSession_E5071C.RawIO.Write(":DISP:WIND1:ACT");

                            mbSession_E5071C_tdr.RawIO.Write(":SENS:CORR:COLL:ECAL:IMM");                                  //execute full calibration using the ecal module(TDR), Set up --> ECAL --> Calibrate
                            TDR_wait_done("*OPC?");
                        }

                    }

                    mbSession_E5071C.RawIO.Write(":MMEM:STOR:STYP CDST");
                    if (Program.test_type == "B8")
                    {
                        mbSession_E5071C.RawIO.Write(":MMEM:STOR \"D:\\CAMS_STA\\" + switch_port_name + ".STA\"");
                    }
                    else if (Program.test_type == "B2")
                    {
                        mbSession_E5071C.RawIO.Write(":MMEM:STOR \"D:\\CAMS_STA\\B2\\" + switch_port_name + ".STA\"");
                    }
                    else if (Program.test_type == "B3")
                    {
                        mbSession_E5071C.RawIO.Write(":MMEM:STOR \"D:\\CAMS_STA\\B3\\" + switch_port_name + ".STA\"");
                    }

                    wait_done("*OPC?");

                    calibration_done[calibration_done_i] = 1;

                    string now_path = System.Windows.Forms.Application.StartupPath + "\\";
                    StreamWriter sw = new StreamWriter(now_path + "calibration_sta.ini", false);
                    //Write a line of text
                    sw.WriteLine("[STA]");
                    //Write a second line of text
                    int i = 0;
                    foreach (int calibration_item in calibration_done)
                    {
                        sw.WriteLine("calibration_done[" + i + "]=" + calibration_item.ToString());
                        i++;
                    }
                    //Close the file
                    sw.Close();


                }

                this.Invoke((MethodInvoker)delegate ()
                {
                    mbSession_E5071C.Dispose();
                    mbSession_E5071C_tdr.Dispose();
                    calibration_start.Enabled = Enabled;
                    calibration_start.BackgroundImage = Properties.Resources.button01;
                    form_4.Close();
                });
                MessageBox.Show("finish");
            }
            catch (Exception exp)
            {
                thread_for_stop = new Thread(stop);
                thread_for_stop.Start();
            }

        }
        /////////////////////////////////////////////////////////////////////////////
        //等待儀器的動作完成-----------------------------------------------------------------
        void wait_done(string gpib_command)
        {
            string read_value;

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
                stop_calibration(exp.Message);
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

            //setting  Trace3 和 Trace7
            TDR_Query_response_value(":CALC1:TRAC3:PAR?", "\"Tdd21\"\n");                                   //TDR/TDT --> Parameters
            TDR_Query_response_value(":CALC1:TRAC3:FORM?", "VOLT\n");                                        //TDR/TDT --> Parameters --> Format


            mbSession_E5071C_tdr.RawIO.Write(":SENS:DLEN:DATA 80e-10");                                    //設定上升時間
            mbSession_E5071C_tdr.RawIO.Write(":CALC1:TRAC1:TIME:STEP:RTIM:DATA 400e-12");



            TDR_wait_done("*OPC?");
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
            TDR_wait_done("*OPC?");

            //Trace_5  DIV & STAR END
            mbSession_E5071C_tdr.RawIO.Write(":DISP:TRAC5:X:SCAL:PDIV 300e-11");                           //set value of x-axis scale per division
            mbSession_E5071C_tdr.RawIO.Write(":DISP:TRAC5:X:SCAL:RLEV -500e-11");                          //set value of x-axis reference line
            mbSession_E5071C_tdr.RawIO.Write(":DISP:TRAC5:Y:SCAL:PDIV 5");                                 //sets value of Y-axis scale per division
            mbSession_E5071C_tdr.RawIO.Write(":DISP:TRAC5:Y:SCAL:RLEV 70");
            TDR_wait_done("*OPC?");

            //Trace_2  DIV & STAR END
            mbSession_E5071C_tdr.RawIO.Write(":DISP:TRAC2:X:SCAL:PDIV 500e-11");                           //set value of x-axis scale per division
            mbSession_E5071C_tdr.RawIO.Write(":DISP:TRAC2:X:SCAL:RLEV -100e-11");                          //set value of x-axis reference line
            mbSession_E5071C_tdr.RawIO.Write(":DISP:TRAC2:Y:SCAL:PDIV 500e-4");                                 //sets value of Y-axis scale per division
            mbSession_E5071C_tdr.RawIO.Write(":DISP:TRAC2:Y:SCAL:RLEV 100e-3");
            TDR_wait_done("*OPC?");
            wait_done("*OPC?");
            //Trace_6  DIV & STAR END
            mbSession_E5071C_tdr.RawIO.Write(":DISP:TRAC6:X:SCAL:PDIV 500e-11");                           //set value of x-axis scale per division
            mbSession_E5071C_tdr.RawIO.Write(":DISP:TRAC6:X:SCAL:RLEV -100e-11");                          //set value of x-axis reference line
            mbSession_E5071C_tdr.RawIO.Write(":DISP:TRAC6:Y:SCAL:PDIV 500e-4");                                 //sets value of Y-axis scale per division
            mbSession_E5071C_tdr.RawIO.Write(":DISP:TRAC6:Y:SCAL:RLEV 100e-3");

            TDR_wait_done("*OPC?");
            //Trace_3  DIV & STAR END
            mbSession_E5071C_tdr.RawIO.Write(":DISP:TRAC3:X:SCAL:PDIV 300e-11");                           //set value of x-axis scale per division
            mbSession_E5071C_tdr.RawIO.Write(":DISP:TRAC3:X:SCAL:RLEV -500e-11");                          //set value of x-axis reference line
            mbSession_E5071C_tdr.RawIO.Write(":DISP:TRAC3:Y:SCAL:PDIV 100e-3");                                 //sets value of Y-axis scale per division
            mbSession_E5071C_tdr.RawIO.Write(":DISP:TRAC3:Y:SCAL:RLEV 0");



            TDR_wait_done("*OPC?");


            // Off BZ
            mbSession_E5071C.RawIO.Write(":SYST:BEEP:COMP:STAT OFF");
            mbSession_E5071C.RawIO.Write(":SYST:BEEP:WARN:STAT OFF");
            //off BZ
            TDR_wait_done("*OPC?");

            // Tr1、Tr5「IMPEDANCE」
            //CALC1 Tr1  Import IMP_LIMIT.csv
            mbSession_E5071C.RawIO.Write(":CALC1:PAR1:SEL");
            mbSession_E5071C.RawIO.Write(":DISP:WIND1:MAX ON");
            mbSession_E5071C.RawIO.Write(":MMEM:LOAD:LIM \"D:\\CAMS_Limitline\\IMP_LIMIT_DD_TR15.csv\"");                                                                                //Analysis > Limit Test > Edit Limit Line > Import from CSV File
            mbSession_E5071C.RawIO.Write(":CALC1:LIM ON");                                                                                                                       //Analysis > Limit Test > Limit Test
            mbSession_E5071C.RawIO.Write(":CALC1:LIM:DISP ON");                                                                                                                  //Analysis > Limit Test > Limit Line
            mbSession_E5071C.RawIO.Write(":DISP:FSIG OFF");                                                                                                                       //Analysis > Limit Test > Fail Sign
            TDR_wait_done("*OPC?");


            // CALC1 Tr5  Import IMP_LIMIT.csv
            mbSession_E5071C.RawIO.Write(":CALC1:PAR5:SEL");
            mbSession_E5071C.RawIO.Write(":DISP:WIND1:MAX ON");
            mbSession_E5071C.RawIO.Write(":MMEM:LOAD:LIM \"D:\\CAMS_Limitline\\IMP_LIMIT_DD_TR15.csv\"");                                                                                     //Analysis > Limit Test > Edit Limit Line > Import from CSV File
            mbSession_E5071C.RawIO.Write(":CALC1:LIM ON");                                                                                                                       //Analysis > Limit Test > Limit Test
            mbSession_E5071C.RawIO.Write(":CALC1:LIM:DISP ON");                                                                                                                  //Analysis > Limit Test > Limit Line
            mbSession_E5071C.RawIO.Write(":DISP:FSIG OFF");
            TDR_wait_done("*OPC?");

            //MARK1 Setting     
            mbSession_E5071C.RawIO.Write(":CALC1:MARK1 ON");  //ok
            mbSession_E5071C.RawIO.Write(":CALC1:MARK1:FUNC:TYPE MAX");    //ok                               //2022/09/21 Kevin add for --->	這一行是將 MARK1 設定為 MAX
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC1:MARK1:FUNC:TRAC ON");                                     //2022/09/21 Kevin add for --->	這一行是將MARK1設定為追蹤
            mbSession_E5071C.RawIO.Write(":CALC1:MARK:FUNC:DOM ON");                                       //2022/09/21 Kevin add for --->	這一行是設定Search Range(ON)
            TDR_wait_done("*OPC?");


            //MARK2 Setting
            mbSession_E5071C.RawIO.Write(":CALC1:MARK2 ON");  //ok
            mbSession_E5071C.RawIO.Write(":CALC1:MARK2:FUNC:TYPE MIN");    //ok                               //2022/09/21 Kevin add for --->	這一行是將 MARK1 設定為 MAX
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC2:MARK1:FUNC:TRAC ON");                                     //2022/09/21 Kevin add for --->	這一行是將MARK1設定為追蹤
            mbSession_E5071C.RawIO.Write(":CALC1:MARK:FUNC:DOM ON");                                       //2022/09/21 Kevin add for --->	這一行是設定Search Range(ON)

            TDR_wait_done("*OPC?");

            //Tr2、Tr6 「SKEW」
            mbSession_E5071C.RawIO.Write(":DISP:WIND1:ACT");
            mbSession_E5071C.RawIO.Write(":DISP:WIND1:MAX OFF");
            //mbSession_E5071C.RawIO.Write(":CALC:ATR:ACT 2");                                               //2022/09/21 Kevin add for ---> 這段是設定起始為 Tr2
            mbSession_E5071C_tdr.RawIO.Write(":CALC:ATR:ACT 2");
            mbSession_E5071C_tdr.RawIO.Write(":CALC:TRAC2:DTIM:TARG 6");
            mbSession_E5071C_tdr.RawIO.Write(":CALC:TRAC2:DTIM:STAT ON");                                     //2022/09/21 Kevin add for ---> 這一段是設定 Tr2 及 Tr6 作相減


            TDR_wait_done("*OPC?");
            mbSession_E5071C_tdr.RawIO.Write(":CALC:TRAC2:DTIM:POS 50");                                        //2022/09/21 Kevin add for ---> 這一段是設定 position 為50
            mbSession_E5071C_tdr.RawIO.Write(":TRIG:SING");                                                     //2022/09/21 Kevin add for --->	
            mbSession_E5071C_tdr.RawIO.Write(":CALC:TRAC2:DTIM:DATA?");                                         //2022/09/21 Kevin add for --->	//:CALCulate:TRACe{Tr}:DTIMe:DATA  //This command gets delta time result value. You can get the result even if :CALCulate:TRACe{Tr}:DTIMe:STATe is off.
            TDR_wait_done("*OPC?");


            mbSession_E5071C.RawIO.Write(":DISP:WIND1:MAX ON");
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC3:MARK1 ON");
            mbSession_E5071C.RawIO.Write(":CALC1:MARK:FUNC:DOM OFF");
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC3:MARK1:FUNC:TRAC ON");                                   //MKR/ANALYSIS --> Marker Search --> Track(on)
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC3:MARK1:FUNC:TYPE TARG");                                 //MKR/ANALYSIS --> Marker Search --> Search Target
            mbSession_E5071C.RawIO.Write(":CALC1:TRAC3:MARK1:FUNC:TARG 200e-3");                               //MKR/ANALYSIS --> Marker Search --> Target value

            TDR_wait_done("*OPC?");

            //以下作TR2 及TR6的公式設定
            mbSession_E5071C.RawIO.Write(":SYST:BEEP:COMP:STAT OFF");
            mbSession_E5071C.RawIO.Write(":SYST:BEEP:WARN:STAT OFF");

            mbSession_E5071C_tdr.RawIO.Write(":CALC:ATR:ACT 2");
            mbSession_E5071C_tdr.RawIO.Write(":DISP:TRAC2:X:SCAL:RLEV -3e-9");
            TDR_wait_done("*OPC?");

            mbSession_E5071C.RawIO.Write(":DISP:WIND1:MAX ON");
            mbSession_E5071C.RawIO.Write(":CALC1:SELected:EQUation:STATE ON");
            mbSession_E5071C.RawIO.Write(":CALC1:SELected:EQUation:TEXT \"S21-S41\"");
            TDR_wait_done("*OPC?");


            mbSession_E5071C_tdr.RawIO.Write(":CALC:ATR:ACT 6");
            mbSession_E5071C_tdr.RawIO.Write(":DISP:TRAC6:X:SCAL:RLEV -3e-9");
            TDR_wait_done("*OPC?");

            mbSession_E5071C.RawIO.Write(":CALC1:SELected:EQUation:STATE ON");
            mbSession_E5071C.RawIO.Write(":CALC1:SELected:EQUation:TEXT \"S43-S23\"");
            mbSession_E5071C_tdr.RawIO.Write(":CALC:TRAC2:DTIM:TARG 6");
            mbSession_E5071C_tdr.RawIO.Write(":CALC:TRAC2:DTIM:STAT ON");
            TDR_wait_done("*OPC?");

            Thread.Sleep(2000);





        }   // Kevin add for DD Use   2023/1/30 -E
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
        /////////////////////////////////////////////////////////////////////////////
        //切換Switch組合-----------------------------------------------------------------
        void Switch(string gpib_command)
        {
            try
            {
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
                serialPort1.Write(gpib_command);
            }
            catch (Exception exp)
            {
                stop_calibration(exp.Message);
            }
        }
        /////////////////////////////////////////////////////////////////////////////
        //當使用者確定需要calibration的組合時，鎖定的動作，會根據使用者點選的組合，無效一些不需要測試的按鈕選項-----------------------------------------------------------------
        private void Lock_selection_CheckedChanged(object sender, EventArgs e)
        {
            if (Lock_selection.Checked == true)
            {
                Ecal3B4B.Enabled = true;
                if (switch_Port1_Port2 == 0)
                {
                    Ecal1C2C.Enabled = true;
                    Ecal1D2D.Enabled = true;
                    Ecal1E2E.Enabled = true;
                    Ecal1F2F.Enabled = true;
                    Ecal3C4C.Enabled = true;
                    Ecal3D4D.Enabled = true;

                    Ecal1B2B.FlatAppearance.BorderSize = 2;
                }
                else if (switch_Port1_Port2 == 10)
                {
                    Ecal1B2B.Enabled = false;
                    Ecal1C2C.FlatAppearance.BorderSize = 2;
                }
                else if (switch_Port1_Port2 == 20)
                {
                    Ecal1B2B.Enabled = false;
                    Ecal1C2C.Enabled = false;

                    Ecal1D2D.FlatAppearance.BorderSize = 2;
                }
                else if (switch_Port1_Port2 == 30)
                {
                    Ecal1B2B.Enabled = false;
                    Ecal1C2C.Enabled = false;
                    Ecal1D2D.Enabled = false;

                    Ecal1E2E.FlatAppearance.BorderSize = 2;
                }
                else if (switch_Port1_Port2 == 40)
                {
                    Ecal1B2B.Enabled = false;
                    Ecal1C2C.Enabled = false;
                    Ecal1D2D.Enabled = false;
                    Ecal1E2E.Enabled = false;

                    Ecal1F2F.FlatAppearance.BorderSize = 2;
                }
                else if (switch_Port1_Port2 == 90)
                {
                    //Ecal1B2B.Enabled = false;
                    //Ecal1C2C.Enabled = false;
                    //Ecal1D2D.Enabled = false;
                    //Ecal1E2E.Enabled = false;
                    //Ecal1F2F.Enabled = false;
                    //Ecal3B4B.Enabled = false;
                    //Ecal3C4C.Enabled = false;
                    //Ecal3D4D.Enabled = false;
                    //Ecal3E4E.Enabled = false;
                    //MessageBox.Show("(3B,4B,X,X) 的組合不用Calibration");
                    //Lock_selection.Checked = false;
                    //Ecal3B4B.BackColor = Color.FromArgb(0, 68, 109);
                }
                else if (switch_Port1_Port2 == 80)
                {
                    if (Program.test_type == "B3")
                    {
                        Ecal1B2B.Enabled = false;
                        Ecal1C2C.Enabled = false;
                        Ecal1D2D.Enabled = false;
                        Ecal1E2E.Enabled = false;
                        Ecal1F2F.Enabled = false;
                        Ecal3D4D.Enabled = true;
                        Ecal3E4E.Enabled = true;
                        Ecal3F4F.Enabled = true;

                        Ecal3C4C.FlatAppearance.BorderSize = 2;
                    }
                    else
                    {
                        Ecal1B2B.Enabled = false;
                        Ecal1C2C.Enabled = false;
                        Ecal1D2D.Enabled = false;
                        Ecal1E2E.Enabled = false;
                        Ecal1F2F.Enabled = false;
                        Ecal3D4D.Enabled = false;
                        Ecal3E4E.Enabled = false;
                        Ecal3F4F.Enabled = false;

                        Ecal3C4C.FlatAppearance.BorderSize = 2;
                    }

                }
                else if (switch_Port1_Port2 == 70)
                {
                    if (Program.test_type == "B3")
                    {
                        Ecal1B2B.Enabled = false;
                        Ecal1C2C.Enabled = false;
                        Ecal1D2D.Enabled = false;
                        Ecal1E2E.Enabled = false;
                        Ecal1F2F.Enabled = false;
                        Ecal3E4E.Enabled = true;
                        Ecal3F4F.Enabled = true;

                        Ecal3D4D.FlatAppearance.BorderSize = 2;
                    }
                    else
                    {
                        Ecal1B2B.Enabled = false;
                        Ecal1C2C.Enabled = false;
                        Ecal1D2D.Enabled = false;
                        Ecal1E2E.Enabled = false;
                        Ecal1F2F.Enabled = false;
                        Ecal3E4E.Enabled = false;
                        Ecal3F4F.Enabled = false;

                        Ecal3D4D.FlatAppearance.BorderSize = 2;
                    }

                }
                else if (switch_Port1_Port2 == 60)
                {
                    if (Program.test_type == "B3")
                    {
                        Ecal1B2B.Enabled = false;
                        Ecal1C2C.Enabled = false;
                        Ecal1D2D.Enabled = false;
                        Ecal1E2E.Enabled = false;
                        Ecal1F2F.Enabled = false;
                        Ecal3F4F.Enabled = true;

                        Ecal3E4E.FlatAppearance.BorderSize = 2;
                    }
                    else
                    {
                        Ecal1B2B.Enabled = false;
                        Ecal1C2C.Enabled = false;
                        Ecal1D2D.Enabled = false;
                        Ecal1E2E.Enabled = false;
                        Ecal1F2F.Enabled = false;
                        Ecal3F4F.Enabled = false;

                        Ecal3E4E.FlatAppearance.BorderSize = 2;
                    }

                }
                else if (switch_Port1_Port2 == 50)
                {
                    Ecal1B2B.Enabled = false;
                    Ecal1C2C.Enabled = false;
                    Ecal1D2D.Enabled = false;
                    Ecal1E2E.Enabled = false;
                    Ecal1F2F.Enabled = false;

                    Ecal3F4F.FlatAppearance.BorderSize = 2;
                }

                Reset_button_state(); //將所有按鈕恢復到原本的顏色
                Calibration_sub_items_done(); //針對(switch_Port1_Port2),X,X的組合(switch_Port1_Port2 為使用者在勾選Lock Selection之前，點選的按鈕名稱，如(1B,2B))，如果已經做完的就會直接上色


            }
            else
            {
                timer1.Enabled = true;
                calibration_start.Enabled = false;
                switch_port_name = "";
                Ecal1B2B.Enabled = true;
                Ecal1C2C.Enabled = true;
                Ecal1D2D.Enabled = true;
                Ecal1E2E.Enabled = true;
                Ecal1F2F.Enabled = true;
                Ecal3C4C.Enabled = true;
                Ecal3D4D.Enabled = true;
                Ecal3E4E.Enabled = true;
                Ecal3F4F.Enabled = true;
                if (Program.test_type == "B3")
                {
                    Ecal3B4B.Enabled = true;
                }
                else
                {
                    Ecal3B4B.Enabled = false;
                }



                Reset_button_state(); //將所有按鈕恢復到原本的顏色
                Calibration_items_done(); //已經完成的按鈕在沒有Lock Selection的情況下，直接上色
            }
        }
        /////////////////////////////////////////////////////////////////////////////
        //針對(switch_Port1_Port2),X,X的組合(switch_Port1_Port2 為使用者在勾選Lock Selection之前，點選的按鈕名稱，如(1B,2B))，如果已經做完的就會直接上色-----------------------------------------------------------------
        private void Calibration_sub_items_done()
        {
            //在(switch_Port1_Port2),X,X的組合中，檢查那些測項已經完成，如果完成的直接上色
            for (int switch_Port3_Port4 = 0; switch_Port3_Port4 <= 9; switch_Port3_Port4++)
            {
                if (calibration_done[switch_Port1_Port2 + switch_Port3_Port4] == 1)
                {
                    switch (switch_Port3_Port4)
                    {
                        case 0:
                            Ecal1B2B.BackColor = Color.FromArgb(0, 68, 109);
                            break;
                        case 1:
                            Ecal1C2C.BackColor = Color.FromArgb(0, 68, 109);
                            break;
                        case 2:
                            Ecal1D2D.BackColor = Color.FromArgb(0, 68, 109);
                            break;
                        case 3:
                            Ecal1E2E.BackColor = Color.FromArgb(0, 68, 109);
                            break;
                        case 4:
                            Ecal1F2F.BackColor = Color.FromArgb(0, 68, 109);
                            break;
                        case 9:
                            Ecal3B4B.BackColor = Color.FromArgb(0, 68, 109);
                            break;
                        case 8:
                            Ecal3C4C.BackColor = Color.FromArgb(0, 68, 109);
                            break;
                        case 7:
                            Ecal3D4D.BackColor = Color.FromArgb(0, 68, 109);
                            break;
                        case 6:
                            Ecal3E4E.BackColor = Color.FromArgb(0, 68, 109);
                            break;
                        case 5:
                            Ecal3F4F.BackColor = Color.FromArgb(0, 68, 109);
                            break;
                    }
                }
            }

        }
        /////////////////////////////////////////////////////////////////////////////
        //檢查那些測試項目已經完成，已經完成的按鈕會直接上色-----------------------------------------------------------------
        private void Calibration_items_done()
        {
            //掃描Port1,Port2
            for (int sw_P1_P2 = 0; sw_P1_P2 <= 9; sw_P1_P2++)
            {
                bool done = true; //如果掃秒到有0的數值時，代表該測項尚未完成，因此done的變數會為false，反之，已完成按鈕會上特定顏色代表完成
                // 掃描Port3,Port4
                for (int i = sw_P1_P2 + 1; i <= 9; i++)
                {
                    if (calibration_done[sw_P1_P2 * 10 + i] != 1)
                    {
                        done = false;
                    }
                }

                switch (sw_P1_P2 * 10)
                {
                    case 0:
                        if (done)
                        {
                            Ecal1B2B.BackColor = Color.FromArgb(52, 170, 212);
                        }
                        break;
                    case 10:
                        if (done)
                        {
                            Ecal1C2C.BackColor = Color.FromArgb(230, 131, 75);
                        }
                        break;
                    case 20:
                        if (done)
                        {
                            Ecal1D2D.BackColor = Color.FromArgb(29, 164, 131);
                        }
                        break;
                    case 30:
                        if (done)
                        {
                            Ecal1E2E.BackColor = Color.FromArgb(210, 70, 58);
                        }
                        break;
                    case 40:
                        if (done)
                        {
                            Ecal1F2F.BackColor = Color.FromArgb(118, 120, 155);
                        }
                        break;
                    case 80:
                        if (done)
                        {
                            Ecal3C4C.BackColor = Color.FromArgb(194, 82, 156);
                        }
                        break;
                    case 70:
                        if (done)
                        {
                            Ecal3D4D.BackColor = Color.FromArgb(175, 165, 18);
                        }
                        break;
                    case 60:
                        if (done)
                        {
                            Ecal3E4E.BackColor = Color.FromArgb(124, 98, 111);
                        }
                        break;
                    case 50:
                        if (done)
                        {
                            Ecal3F4F.BackColor = Color.FromArgb(54, 214, 202);
                        }
                        break;
                }
            }

        }
        /////////////////////////////////////////////////////////////////////////////
        //用timer輪詢calibration的指令是否已經可以使用-----------------------------------------------------------------
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (switch_port_name.Length >= 11)
            {
                calibration_start.Enabled = true;
                calibration_start.BackgroundImage = Properties.Resources.button01;
                timer1.Enabled = false;
            }
            else
            {
                calibration_start.Enabled = false;
                calibration_start.BackgroundImage = Properties.Resources.button01;
            }

        }
        /////////////////////////////////////////////////////////////////////////////
        //將所有按鈕恢復到原本的顏色，如果lock selection不是打勾的狀態下會把所有的按鈕邊框變為零-----------------------------------------------------------------
        private void Reset_button_state()
        {

            Ecal1B2B.BackColor = Color.Transparent;
            Ecal1C2C.BackColor = Color.Transparent;
            Ecal1D2D.BackColor = Color.Transparent;
            Ecal1E2E.BackColor = Color.Transparent;
            Ecal1F2F.BackColor = Color.Transparent;
            Ecal3B4B.BackColor = Color.Transparent;
            Ecal3C4C.BackColor = Color.Transparent;
            Ecal3D4D.BackColor = Color.Transparent;
            Ecal3E4E.BackColor = Color.Transparent;
            Ecal3F4F.BackColor = Color.Transparent;

            if (!Lock_selection.Checked == true)
            {
                Ecal1B2B.FlatAppearance.BorderSize = 0;
                Ecal1C2C.FlatAppearance.BorderSize = 0;
                Ecal1D2D.FlatAppearance.BorderSize = 0;
                Ecal1E2E.FlatAppearance.BorderSize = 0;
                Ecal1F2F.FlatAppearance.BorderSize = 0;
                Ecal3B4B.FlatAppearance.BorderSize = 0;
                Ecal3C4C.FlatAppearance.BorderSize = 0;
                Ecal3D4D.FlatAppearance.BorderSize = 0;
                Ecal3E4E.FlatAppearance.BorderSize = 0;
                Ecal3F4F.FlatAppearance.BorderSize = 0;

            }
        }
        /////////////////////////////////////////////////////////////////////////////

        private void Clear_button_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 99; i++)
            {
                calibration_done[i] = 0;
            }
            Ecal1B2B.BackColor = Color.Transparent;
            Ecal1C2C.BackColor = Color.Transparent;
            Ecal1D2D.BackColor = Color.Transparent;
            Ecal1E2E.BackColor = Color.Transparent;
            Ecal1F2F.BackColor = Color.Transparent;
            Ecal3C4C.BackColor = Color.Transparent;
            Ecal3D4D.BackColor = Color.Transparent;
            Ecal3E4E.BackColor = Color.Transparent;
            Ecal3F4F.BackColor = Color.Transparent;
        }

        private void click_timer_Tick(object sender, EventArgs e)
        {
            double_click_event = false;
            click_timer.Enabled = false;
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }


    }
}
