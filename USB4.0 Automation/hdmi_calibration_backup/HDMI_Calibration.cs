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

namespace USB4._0_Automation
{
    public partial class HDMI_Calibration : Form
    {
        string E5071C_Resource = "";
        string Switch_Resource = "";
        private MessageBasedSession mbSession_E5071C;
        private MessageBasedSession mbSession_E5071C_tdr;
        string[] MARKER1_DATA = new string[3];
        IniManager iniManager = new IniManager(System.Environment.CurrentDirectory + "\\" + "TDR_setting.ini"); //設定ini檔路徑
        Form4 form_4 = new Form4();
        Thread thread_for_Calibration_ini_env_set;

        int[,] calibration_done = new int[5, 4];
        int One_dimensional = 0;
        int Two_dimensional = 0;

        Color Near_D0_color = Color.Red;
        Color Near_D1_color = Color.Red;
        Color Near_D2_color = Color.Red;
        Color Near_D3_color = Color.Red;

        bool DO_DONE = false;
        bool D1_DONE = false;
        bool D2_DONE = false;
        bool D3_DONE = false;

        string switch_port1_3;
        string switch_port2_4;
        string fixture_port1_3;
        string fixture_port2_4;
        string switch_command = "";
        string fixture_port_name = "";

        bool HF7_12_env = false;
        bool Automation = true;

        public HDMI_Calibration()
        {
            InitializeComponent();
        }

        private void HDMI_Calibration_Load(object sender, EventArgs e)
        {
            var rmSession = new ResourceManager();
            E5071C_Resource = shareArea.e5071c_resource;
            Switch_Resource = shareArea.switch_resource;

            serialPort1.PortName = Switch_Resource;
            serialPort1.BaudRate = 9600;
            serialPort1.Parity = Parity.None;
            serialPort1.DataBits = 8;
            serialPort1.StopBits = StopBits.One;
            serialPort1.Encoding = Encoding.UTF8;
            serialPort1.ReadTimeout = 500;

            try
            {
                mbSession_E5071C = (MessageBasedSession)rmSession.Open(E5071C_Resource); //連線到E5071C
                if (!serialPort1.IsOpen)
                {
                    serialPort1.Open(); //連線到Switch
                }
                try
                {
                    mbSession_E5071C_tdr = (MessageBasedSession)rmSession.Open("TCPIP0::127.0.0.1::inst0::INSTR");

                    try
                    {
                        thread_for_Calibration_ini_env_set = new Thread(Calibration_ini_env_set);
                        thread_for_Calibration_ini_env_set.Start();
                        form_4 = new Form4();
                        form_4.ShowDialog();
                    }
                    catch (Exception exp)
                    {
                        MessageBox.Show(exp.Message);
                        this.Close();
                    }

                }
                catch (InvalidCastException)
                {
                    MessageBox.Show("Resource selected must be a message-based session");
                    this.Close();
                }
                catch (Exception exp)
                {
                    MessageBox.Show("Please open ENA-TDR.exe or Restart");
                    this.Close();
                }
            }
            catch (InvalidCastException)
            {
                MessageBox.Show("Resource selected must be a message-based session");
                this.Close();
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
                this.Close();
            }
        }
        private void HDMI_Calibration_FormClosing(object sender, FormClosingEventArgs e)
        {
            stop_run();
        }
        void stop_run()
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

        }
        private void ECAL_button_Click(object sender, EventArgs e)
        {
            if (HF7_12_env) //環境已改變為HF7-12的校正環境，因此需要再次改變校正環境
            {
                HF7_12_env = false;
                Calibration_ini_env_set();
            }
            ECAL_Calibration();
            calibration_done[One_dimensional, Two_dimensional] = 1;
            Calibration_items_done();
            MessageBox.Show("Calibration finish");
        }

        private void ECAL_button_MouseDown(object sender, MouseEventArgs e)
        {
            ECAL_button.BackgroundImage = Properties.Resources.button02;
            ECAL_button.ForeColor = Color.White;
        }

        private void ECAL_button_MouseMove(object sender, MouseEventArgs e)
        {
            ECAL_button.BackgroundImage = Properties.Resources.button02;
            ECAL_button.ForeColor = Color.White;
        }

        private void ECAL_button_MouseLeave(object sender, EventArgs e)
        {
            ECAL_button.BackgroundImage = Properties.Resources.button01;
            ECAL_button.ForeColor = Color.Black;
        }

        private void ECAL_button_MouseUp(object sender, MouseEventArgs e)
        {
            ECAL_button.BackgroundImage = Properties.Resources.button01;
            ECAL_button.ForeColor = Color.Black;
        }
        
        void Calibration_ini_env_set()
        {
            check_DIF2_ENV();

            //設定頻域的環境
            mbSession_E5071C.RawIO.Write(":DISP:SPL D12");
            mbSession_E5071C.RawIO.Write(":DISP:WIND2:ACT");
            //mbSession_E5071C.RawIO.Write(":DISP:MAX ON");

            mbSession_E5071C.RawIO.Write(":CALC2:PAR:COUN 10");
            mbSession_E5071C.RawIO.Write(":DISP:WIND2:SPL D12_34");
            mbSession_E5071C.RawIO.Write(":SENS2:FREQ:STAR 10E6");                         //Start
            mbSession_E5071C.RawIO.Write(":SENS2:FREQ:STOP 12E9 ");                        //Stop
            mbSession_E5071C.RawIO.Write(":SENS2:SWE:POIN 1200");                          //Point
            mbSession_E5071C.RawIO.Write(":SENS2:BWID 1E3");                               //Bendwidth

            mbSession_E5071C.RawIO.Write(":CALC2:FSIM:STAT ON");                           //Analysis --> fixture simulator --> Fixture Simulator(ON)
            mbSession_E5071C.RawIO.Write(":CALC2:FSIM:BAL:DEV BBAL");                      //Analysis --> fixture simulator --> Topology -> Device(Bal-Bal)
            mbSession_E5071C.RawIO.Write(":CALC2:FSIM:BAL:TOP:BBAL 1,3,2,4");              //Analysis --> fixture simulator --> Topology --> Port1(1-3) & Port2(2-4)
            mbSession_E5071C.RawIO.Write(":CALC2:FSIM:BAL:PAR1:STAT ON");                  //Analysis --> fixture simulator --> BalUn(OM)
            mbSession_E5071C.RawIO.Write(":CALC2:FSIM:BAL:PAR5:STAT ON");                  //Analysis --> fixture simulator --> BalUn(OM)
            mbSession_E5071C.RawIO.Write(":CALC2:FSIM:BAL:PAR9:STAT ON");                  //Analysis --> fixture simulator --> BalUn(OM)
            mbSession_E5071C.RawIO.Write(":CALC2:FSIM:BAL:PAR2:STAT ON");                  //Analysis --> fixture simulator --> BalUn(OM)
            mbSession_E5071C.RawIO.Write(":CALC2:FSIM:BAL:PAR6:STAT ON");                  //Analysis --> fixture simulator --> BalUn(OM)
            mbSession_E5071C.RawIO.Write(":CALC2:FSIM:BAL:PAR10:STAT ON");                  //Analysis --> fixture simulator --> BalUn(OM)
            mbSession_E5071C.RawIO.Write(":CALC2:FSIM:BAL:PAR3:STAT ON");                  //Analysis --> fixture simulator --> BalUn(OM)
            mbSession_E5071C.RawIO.Write(":CALC2:FSIM:BAL:PAR7:STAT ON");                  //Analysis --> fixture simulator --> BalUn(OM)
            mbSession_E5071C.RawIO.Write(":CALC2:FSIM:BAL:PAR4:STAT ON");                  //Analysis --> fixture simulator --> BalUn(OM)
            mbSession_E5071C.RawIO.Write(":CALC2:FSIM:BAL:PAR8:STAT ON");                  //Analysis --> fixture simulator --> BalUn(OM)

            mbSession_E5071C.RawIO.Write(":CALC2:FSIM:BAL:PAR1:BBAL SDD21");               //Response --> Meas --> SDD21
            mbSession_E5071C.RawIO.Write(":CALC2:FSIM:BAL:PAR5:BBAL SDD21");               //Response --> Meas --> SDD21
            mbSession_E5071C.RawIO.Write(":CALC2:FSIM:BAL:PAR9:BBAL SDD21");               //Response --> Meas --> SDD21 
            mbSession_E5071C.RawIO.Write(":CALC2:FSIM:BAL:PAR2:BBAL SDD21");               //Response --> Meas --> SDD21 
            mbSession_E5071C.RawIO.Write(":CALC2:FSIM:BAL:PAR6:BBAL SDD11");               //Response --> Meas --> SDD11 
            mbSession_E5071C.RawIO.Write(":CALC2:FSIM:BAL:PAR10:BBAL SDD12");              //Response --> Meas --> SDD12
            mbSession_E5071C.RawIO.Write(":CALC2:FSIM:BAL:PAR3:BBAL SCD21");               //Response --> Meas --> SDD21 
            mbSession_E5071C.RawIO.Write(":CALC2:FSIM:BAL:PAR7:BBAL SDC21");               //Response --> Meas --> SDD21 
            mbSession_E5071C.RawIO.Write(":CALC2:FSIM:BAL:PAR4:BBAL SCD12");               //Response --> Meas --> SDD12 
            mbSession_E5071C.RawIO.Write(":CALC2:FSIM:BAL:PAR8:BBAL SDC12");               //Response --> Meas --> SDD12 
            mbSession_E5071C.RawIO.Write(":CALC2:FSIM:SEND:DEEM:STAT OFF");                //Analysis > Fixture Simulator > De-Embedding > De-Embedding(OFF)

            //mbSession_E5071C.RawIO.Write(":CALC2:TRAC1:EQUation:STATE ON");
            //mbSession_E5071C.RawIO.Write(":CALC2:TRAC1:EQUation:TEXT \"FEXT_1_Sdd21=(S31-S32-S41+S42)/2\"");
            //mbSession_E5071C.RawIO.Write(":CALC2:TRAC5:EQUation:STATE ON");
            //mbSession_E5071C.RawIO.Write(":CALC2:TRAC5:EQUation:TEXT \"FEXT_2_Sdd21=(S31-S32-S41+S42)/2\"");
            //mbSession_E5071C.RawIO.Write(":CALC2:TRAC9:EQUation:STATE ON");
            //mbSession_E5071C.RawIO.Write(":CALC2:TRAC9:EQUation:TEXT \"FEXT_3_Sdd21=(S31-S32-S41+S42)/2\"");
            //mbSession_E5071C.RawIO.Write(":CALC2:TRAC2:EQUation:STATE ON");
            //mbSession_E5071C.RawIO.Write(":CALC2:TRAC2:EQUation:TEXT \"Ins_Loss_Sdd21=(S31-S32-S41+S42)/2\"");
            //mbSession_E5071C.RawIO.Write(":CALC2:TRAC6:EQUation:STATE ON");
            //mbSession_E5071C.RawIO.Write(":CALC2:TRAC6:EQUation:TEXT \"ACR=10*log10(mag(mem(1))^2+mag(mem(5))^2+mag(mem(9))^2)-20*log10(mag(mem(2)))\"");
            mbSession_E5071C.RawIO.Write(":CALC2:TRAC1:EQUation:STATE ON");
            mbSession_E5071C.RawIO.Write(":CALC2:TRAC1:EQUation:TEXT \"FEXT_1_Sdd21=(S21-S23-S41+S43)/2\"");
            mbSession_E5071C.RawIO.Write(":CALC2:TRAC5:EQUation:STATE ON");
            mbSession_E5071C.RawIO.Write(":CALC2:TRAC5:EQUation:TEXT \"FEXT_2_Sdd21=(S21-S23-S41+S43)/2\"");
            mbSession_E5071C.RawIO.Write(":CALC2:TRAC9:EQUation:STATE ON");
            mbSession_E5071C.RawIO.Write(":CALC2:TRAC9:EQUation:TEXT \"FEXT_3_Sdd21=(S21-S23-S41+S43)/2\"");
            mbSession_E5071C.RawIO.Write(":CALC2:TRAC2:EQUation:STATE ON");
            mbSession_E5071C.RawIO.Write(":CALC2:TRAC2:EQUation:TEXT \"Ins_Loss_Sdd21=(S21-S23-S41+S43)/2\"");
            mbSession_E5071C.RawIO.Write(":CALC2:TRAC10:EQUation:STATE ON");
            mbSession_E5071C.RawIO.Write(":CALC2:TRAC10:EQUation:TEXT \"Ins_Loss_Sdd12=(S12-S32-S14+S34)/2\"");

            mbSession_E5071C.RawIO.Write(":CALC2:TRAC6:FORM REAL");
            mbSession_E5071C.RawIO.Write(":CALC2:TRAC6:EQUation:STATE ON");
            mbSession_E5071C.RawIO.Write(":CALC2:TRAC6:EQUation:TEXT \"ACR=10*log10(mag(mem(1))^2+mag(mem(5))^2+mag(mem(9))^2)-20*log10(mag(mem(2)))\"");
            wait_done("*OPC?");

            //設定時域環境
            mbSession_E5071C.RawIO.Write(":DISP:WIND1:ACT");
            //mbSession_E5071C.RawIO.Write(":DISP:MAX ON");
            mbSession_E5071C.RawIO.Write(":CALC1:FSIM:STAT ON");                           //Analysis --> fixture simulator --> Fixture Simulator(ON)
            mbSession_E5071C.RawIO.Write(":CALC1:FSIM:BAL:DEV BBAL");                      //Analysis --> fixture simulator --> Topology -> Device(Bal-Bal)
            mbSession_E5071C.RawIO.Write(":CALC1:FSIM:BAL:TOP:BBAL 1,3,2,4");              //Analysis --> fixture simulator --> Topology --> Port1(1-3) & Port2(2-4)
            mbSession_E5071C.RawIO.Write(":CALC1:FSIM:BAL:PAR1:STAT ON");                  //Analysis --> fixture simulator --> BalUn(OM)
            mbSession_E5071C.RawIO.Write(":CALC1:FSIM:BAL:PAR5:STAT ON");                  //Analysis --> fixture simulator --> BalUn(OM)
            mbSession_E5071C.RawIO.Write(":CALC1:FSIM:BAL:PAR2:STAT ON");                  //Analysis --> fixture simulator --> BalUn(OM)
            mbSession_E5071C.RawIO.Write(":CALC1:FSIM:BAL:PAR6:STAT ON");                  //Analysis --> fixture simulator --> BalUn(OM)
            mbSession_E5071C.RawIO.Write(":CALC1:FSIM:BAL:PAR3:STAT ON");                  //Analysis --> fixture simulator --> BalUn(OM)
            mbSession_E5071C.RawIO.Write(":CALC1:FSIM:BAL:PAR7:STAT ON");                  //Analysis --> fixture simulator --> BalUn(OM)
            mbSession_E5071C.RawIO.Write(":CALC1:FSIM:BAL:PAR4:STAT ON");                  //Analysis --> fixture simulator --> BalUn(OM)
            mbSession_E5071C.RawIO.Write(":CALC1:FSIM:BAL:PAR8:STAT ON");                  //Analysis --> fixture simulator --> BalUn(OM)
            wait_done("*OPC?");

            mbSession_E5071C_tdr.RawIO.Write(":CALC1:ATR:MARK:COUP OFF");
            mbSession_E5071C_tdr.RawIO.Write(":CALC1:ATR:TIME:COUP OFF");
            mbSession_E5071C_tdr.RawIO.Write("CALC1:EMB:S4P:DIFF1:DEEM OFF"); ////Setup > adv waveform > de-embedding > Enable(off)
            TDR_wait_done("*OPC?");

            TDR_Query_response_val_string(":CALC1:TRAC1:PAR?", "\"Tdd11\"\n");
            TDR_Query_response_val_string(":CALC1:TRAC1:FORM?", "IMP\n");
            TDR_Query_response_val_string(":CALC1:TRAC5:PAR?", "\"Tdd22\"\n");
            TDR_Query_response_val_string(":CALC1:TRAC5:FORM?", "IMP\n");
            TDR_Query_response_val_string(":CALC1:TRAC3:PAR?", "\"T21\"\n");
            TDR_Query_response_val_string(":CALC1:TRAC3:FORM?", "VOLT\n");
            TDR_Query_response_val_string(":CALC1:TRAC7:PAR?", "\"T43\"\n");
            TDR_Query_response_val_string(":CALC1:TRAC7:FORM?", "VOLT\n");
            TDR_Query_response_val_string(":CALC1:TRAC2:PAR?", "\"Tdd11\"\n");
            TDR_Query_response_val_string(":CALC1:TRAC2:FORM?", "IMP\n");
            TDR_Query_response_val_string(":CALC1:TRAC6:PAR?", "\"Tdd22\"\n");
            TDR_Query_response_val_string(":CALC1:TRAC6:FORM?", "IMP\n");
            TDR_Query_response_val_string(":CALC1:TRAC4:PAR?", "\"Tdd21\"\n");
            TDR_Query_response_val_string(":CALC1:TRAC4:FORM?", "VOLT\n");
            TDR_Query_response_val_string(":CALC1:TRAC8:PAR?", "\"Tdd12\"\n");
            TDR_Query_response_val_string(":CALC1:TRAC8:FORM?", "VOLT\n");

            mbSession_E5071C_tdr.RawIO.Write(":SENS:DLEN:DATA 20e-9");//TDR/TDT > DUT Length
            mbSession_E5071C_tdr.RawIO.Write(":CALC:ATR:TIME:STEP:AMPL 200e-3");//Setup > Stim. Ampl.
            TDR_wait_done("*OPC?");

            this.Invoke((MethodInvoker)delegate ()
            {
                form_4.Close();
            });
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

        void HF7_12_env_set()
        {
            string err_message = "";
            mbSession_E5071C.RawIO.Write("*RST");

            mbSession_E5071C.RawIO.Write(":SENS1:FREQ:STAR 300E3"); //Start
            mbSession_E5071C.RawIO.Write(":SENS1:FREQ:STOP 200E6"); //Stop
            mbSession_E5071C.RawIO.Write(":SENS1:SWE:POIN 1200");   //Point
            mbSession_E5071C.RawIO.Write(":SENS1:BWID 1E3");        //Bendwidth

            mbSession_E5071C.RawIO.Write(":CALC1:FSIM:STAT ON");                           //Analysis --> fixture simulator --> Fixture Simulator(ON)
            mbSession_E5071C.RawIO.Write(":CALC1:FSIM:BAL:DEV BBAL");                      //Analysis --> fixture simulator --> Topology -> Device(Bal-Bal)
            mbSession_E5071C.RawIO.Write(":CALC1:FSIM:BAL:TOP:BBAL 1,3,2,4");              //Analysis --> fixture simulator --> Topology --> Port1(1-3) & Port2(2-4)
            mbSession_E5071C.RawIO.Write(":CALC1:FSIM:BAL:PAR1:STAT ON");                  //Analysis --> fixture simulator --> BalUn(OM)
            mbSession_E5071C.RawIO.Write(":CALC1:FSIM:BAL:PAR1:BBAL SDD21");               //Response --> Meas --> SDD21 
            mbSession_E5071C.RawIO.Write(":CALC1:FSIM:SEND:DEEM:STAT OFF");                //Analysis > Fixture Simulator > De-Embedding > De-Embedding(OFF)
            wait_done("*OPC?");
            
            while(true)
            {
                mbSession_E5071C.RawIO.Write(":SENS1:CORR:COLL:ECAL:SOLT4 1,2,3,4");           //Cal > ECal > 4-Port Cal
                wait_done("*OPC?");
                mbSession_E5071C.RawIO.Write(":SYST:ERR?");
                err_message = mbSession_E5071C.RawIO.ReadString();
                if (err_message == "+32,\"ECal module not in appropriate RF path\"\n")
                {
                    var result = MessageBox.Show("ECal module not in appropriate RF path.\n If user wants to continue, please click Yes else click No. ", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (result == DialogResult.No)
                    {
                        break;
                    }
                    else
                    {
                        mbSession_E5071C.RawIO.Write(":DISP:CCL");
                    }
                }
                else
                {
                    Query_response_val_string(":CALC1:FSIM:SEND:DEEM:PORT1:USER:FIL?", "\"D:\\CAMS_DEEMBEDD\\HDMI2.1 Cal_2X_Thru_1_Point1200_10MHz_to_12GHz.s2p\"\n");
                    Query_response_val_string(":CALC1:FSIM:SEND:DEEM:PORT3:USER:FIL?", "\"D:\\CAMS_DEEMBEDD\\HDMI2.1 Cal_2X_Thru_1_Point1200_10MHz_to_12GHz.s2p\"\n");
                    Query_response_val_string(":CALC1:FSIM:SEND:DEEM:PORT2:USER:FIL?", "\"D:\\CAMS_DEEMBEDD\\HDMI2.1 Cal_2X_Thru_2_Point1200_10MHz_to_12GHz.s2p\"\n");
                    Query_response_val_string(":CALC1:FSIM:SEND:DEEM:PORT4:USER:FIL?", "\"D:\\CAMS_DEEMBEDD\\HDMI2.1 Cal_2X_Thru_2_Point1200_10MHz_to_12GHz.s2p\"\n");
                    Query_response_val_string(":CALC1:FSIM:SEND:DEEM:PORT1?", "USER\n");
                    Query_response_val_string(":CALC1:FSIM:SEND:DEEM:PORT2?", "USER\n");
                    Query_response_val_string(":CALC1:FSIM:SEND:DEEM:PORT3?", "USER\n");
                    Query_response_val_string(":CALC1:FSIM:SEND:DEEM:PORT4?", "USER\n");
                    Query_response_val_string(":CALC1:FSIM:SEND:DEEM:STAT?", "1\n");

                    mbSession_E5071C.RawIO.Write(":MMEM:STOR:STYP CDST");
                    mbSession_E5071C.RawIO.Write(":MMEM:STOR \"D:\\CAMS_TDR_STA\\HFR7-12.STA\"");
                    break;
                }
            }
        }
        string ECAL_Calibration()
        {
            Switch(switch_command);
            Thread.Sleep(100);
            mbSession_E5071C.RawIO.Write(":SENS2:CORR:COLL:ECAL:SOLT4 1,3,2,4");           //Cal > ECal > 4-Port Cal
            wait_done("*OPC?");
            mbSession_E5071C.RawIO.Write(":SYST:ERR?");
            string err_message = mbSession_E5071C.RawIO.ReadString();
            if (err_message == "+32,\"ECal module not in appropriate RF path\"\n")
            {
                MessageBox.Show("ECal module not in appropriate RF path");
            }
            else
            {
                if(Automation)
                {
                    if (switch_command == "1B,3B,2B,4B," || switch_command == "1C,3C,2C,4C," || switch_command == "1D,3D,2D,4D," || switch_command == "1E,3E,2E,4E," || switch_command == "1F,3F,2F,4F,")
                    {
                        mbSession_E5071C_tdr.RawIO.Write(":SENS:CORR:COLL:ECAL:IMM");
                        TDR_wait_done("*OPC?");
                    }
                    else
                    {
                        load_ch2_DeEmbed();
                    }
                    mbSession_E5071C.RawIO.Write(":DISP:CCL");
                    wait_done("*OPC?");
                    mbSession_E5071C.RawIO.Write(":MMEM:STOR:STYP CDST");
                    mbSession_E5071C.RawIO.Write(":MMEM:STOR \"D:\\CAMS_TDR_STA\\HDMI_ECAL_" + switch_command + "_env.STA\"");
                }
                else
                {
                    mbSession_E5071C_tdr.RawIO.Write(":SENS:CORR:COLL:ECAL:IMM");
                    TDR_wait_done("*OPC?");
                    mbSession_E5071C.RawIO.Write(":DISP:CCL");
                    wait_done("*OPC?");
                    mbSession_E5071C.RawIO.Write(":MMEM:STOR:STYP CDST");
                    mbSession_E5071C.RawIO.Write(":MMEM:STOR \"D:\\CAMS_TDR_STA\\HDMI_ECAL_1B,3B,2B,4B,_env.STA\"");
                    mbSession_E5071C.RawIO.Write(":MMEM:STOR \"D:\\CAMS_TDR_STA\\HDMI_ECAL_1C,3C,2C,4C,_env.STA\"");
                    mbSession_E5071C.RawIO.Write(":MMEM:STOR \"D:\\CAMS_TDR_STA\\HDMI_ECAL_1D,3D,2D,4D,_env.STA\"");
                    mbSession_E5071C.RawIO.Write(":MMEM:STOR \"D:\\CAMS_TDR_STA\\HDMI_ECAL_1E,3E,2E,4E,_env.STA\"");
                    mbSession_E5071C.RawIO.Write(":MMEM:STOR \"D:\\CAMS_TDR_STA\\HDMI_ECAL_1F,3F,2F,4F,_env.STA\"");
                }
            }
            return err_message;
        }

        void Calibration_TDR_HFR_7_13_to_7_15_Rise_time_1n()
        {
            ////Test ID HFR7-13
            string tr1_tdd11_rise_time_value = iniManager.ReadIniFile("TDR_Setting", "Tr1_Tdd11_Rise_time", "");
            string tr1_tcc11_rise_time_value = iniManager.ReadIniFile("TDR_Setting", "Tr1_Tcc11_Rise_time", "");
            string value;
            string rise_time;

            mbSession_E5071C_tdr.RawIO.Write(":CALC:ATR:ACT 1");
            mbSession_E5071C_tdr.RawIO.Write(":TRIG:MODE RUN");
            TDR_wait_done("*OPC?");
            for (int i = 0; i < 2; i++)
            {
                if(i == 0)
                {
                    mbSession_E5071C_tdr.RawIO.Write(":CALC:TRAC1:PAR \"Tcc11\"");
                    rise_time = tr1_tdd11_rise_time_value;
                }  
                else
                {
                    mbSession_E5071C_tdr.RawIO.Write(":CALC:TRAC1:PAR \"Tdd11\"");
                    rise_time = tr1_tcc11_rise_time_value;
                }
                    
                mbSession_E5071C_tdr.RawIO.Write(":CALC:TRAC1:FORM Volt");
                mbSession_E5071C_tdr.RawIO.Write(":DISP:TRAC1:X:SCAL:PDIV 2e-9");
                mbSession_E5071C_tdr.RawIO.Write(":DISP:TRAC1:X:SCAL:RLEV -3e-9");
                mbSession_E5071C_tdr.RawIO.Write(":DISP:TRAC1:Y:SCAL:PDIV 5e-2");
                mbSession_E5071C_tdr.RawIO.Write(":DISP:TRAC1:Y:SCAL:RLEV 0");
                mbSession_E5071C_tdr.RawIO.Write(":CALC:TRAC1:TTIM:STAT ON");
                mbSession_E5071C_tdr.RawIO.Write(":CALC:TRAC1:TTIM:THR T1_9");
                mbSession_E5071C_tdr.RawIO.Write(":CALC:TRAC1:TIME:STEP:RTIM:THR T1_9");
                mbSession_E5071C_tdr.RawIO.Write(":CALC:TRAC1:TIME:STEP:RTIM:DATA " + rise_time);
                TDR_wait_done("*OPC?");
                mbSession_E5071C.RawIO.Write(":DISP:WIND1:TRAC1:Y:RPOS 1");
                wait_done("*OPC?");

                mbSession_E5071C_tdr.RawIO.Write(":CALC:TRAC1:TTIM:DATA?");
                value = mbSession_E5071C_tdr.RawIO.ReadString();
                TDR_wait_done("*OPC?");

                while (true)
                {
                    if (Convert.ToDouble(value) > 999e-12 & Convert.ToDouble(value) < 1e-9)
                    {
                        break;
                    }
                    else if (Convert.ToDouble(value) < 999e-12)
                    {
                        rise_time = (Convert.ToDouble(rise_time) + 1e-12).ToString();
                        mbSession_E5071C_tdr.RawIO.Write(":CALC:TRAC1:TIME:STEP:RTIM:DATA " + rise_time);
                        mbSession_E5071C_tdr.RawIO.Write(":CALC:TRAC1:TTIM:DATA?");
                        value = mbSession_E5071C_tdr.RawIO.ReadString();
                    }
                    else if (Convert.ToDouble(value) > 1e-9)
                    {
                        rise_time = (Convert.ToDouble(rise_time) - 1e-12).ToString();
                        mbSession_E5071C_tdr.RawIO.Write(":CALC:TRAC1:TIME:STEP:RTIM:DATA " + rise_time);
                        mbSession_E5071C_tdr.RawIO.Write(":CALC:TRAC1:TTIM:DATA?");
                        value = mbSession_E5071C_tdr.RawIO.ReadString();
                    }
                    if(Convert.ToDouble(rise_time) > 1.25e-9) //20211217
                    {
                        rise_time = "1.25e-9";
                        break;
                    }
                }
                if (i == 0)
                    iniManager.WriteIniFile("TDR_Setting", "Tr1_Tdd11_Rise_time", rise_time.ToString());
                else
                    iniManager.WriteIniFile("TDR_Setting", "Tr1_Tcc11_Rise_time", rise_time.ToString());
                Thread.Sleep(100);
            }

            string tr5_tdd22_rise_time_value = iniManager.ReadIniFile("TDR_Setting", "Tr5_Tdd22_Rise_time", "");
            string tr5_tcc22_rise_time_value = iniManager.ReadIniFile("TDR_Setting", "Tr5_Tcc22_Rise_time", "");

            mbSession_E5071C_tdr.RawIO.Write(":CALC:ATR:ACT 5");
            for (int i = 0; i < 2; i++)
            {
                if (i == 0)
                {
                    mbSession_E5071C_tdr.RawIO.Write(":CALC:TRAC5:PAR \"Tcc22\"");
                    rise_time = tr5_tdd22_rise_time_value;
                }
                else
                {
                    mbSession_E5071C_tdr.RawIO.Write(":CALC:TRAC5:PAR \"Tdd22\"");
                    rise_time = tr5_tcc22_rise_time_value;
                }
                mbSession_E5071C_tdr.RawIO.Write(":CALC:TRAC5:FORM Volt");
                mbSession_E5071C_tdr.RawIO.Write(":DISP:TRAC5:X:SCAL:PDIV 2e-9");
                mbSession_E5071C_tdr.RawIO.Write(":DISP:TRAC5:X:SCAL:RLEV -3e-9");
                mbSession_E5071C_tdr.RawIO.Write(":DISP:TRAC5:Y:SCAL:PDIV 5e-2");
                mbSession_E5071C_tdr.RawIO.Write(":DISP:TRAC5:Y:SCAL:RLEV 0");
                mbSession_E5071C_tdr.RawIO.Write(":CALC:TRAC5:TTIM:STAT ON");
                mbSession_E5071C_tdr.RawIO.Write(":CALC:TRAC5:TTIM:THR T1_9");
                mbSession_E5071C_tdr.RawIO.Write(":CALC:TRAC5:TIME:STEP:RTIM:THR T1_9");
                mbSession_E5071C_tdr.RawIO.Write(":CALC:TRAC5:TIME:STEP:RTIM:DATA " + rise_time);
                TDR_wait_done("*OPC?");
                mbSession_E5071C.RawIO.Write(":DISP:WIND1:TRAC5:Y:RPOS 1");
                wait_done("*OPC?");

                mbSession_E5071C_tdr.RawIO.Write(":CALC:TRAC5:TTIM:DATA?");
                value = mbSession_E5071C_tdr.RawIO.ReadString();
                while (true)
                {
                    if (Convert.ToDouble(value) > 999e-12 & Convert.ToDouble(value) < 1e-9)
                    {
                        break;
                    }
                    else if (Convert.ToDouble(value) < 999e-12)
                    {
                        rise_time = (Convert.ToDouble(rise_time) + 1e-12).ToString();
                        mbSession_E5071C_tdr.RawIO.Write(":CALC:TRAC5:TIME:STEP:RTIM:DATA " + rise_time);
                        mbSession_E5071C_tdr.RawIO.Write(":CALC:TRAC5:TTIM:DATA?");
                        value = mbSession_E5071C_tdr.RawIO.ReadString();
                    }
                    else if (Convert.ToDouble(value) > 1e-9) //20211217
                    {
                        rise_time = (Convert.ToDouble(rise_time) - 1e-12).ToString();
                        mbSession_E5071C_tdr.RawIO.Write(":CALC:TRAC5:TIME:STEP:RTIM:DATA " + rise_time);
                        mbSession_E5071C_tdr.RawIO.Write(":CALC:TRAC5:TTIM:DATA?");
                        value = mbSession_E5071C_tdr.RawIO.ReadString();
                    }
                    if (Convert.ToDouble(rise_time) > 1.25e-9)
                    {
                        rise_time = "1.25e-9";
                        break;
                    }
                }

                mbSession_E5071C_tdr.RawIO.Write(":CALC:TRAC1:TTIM:STAT OFF");
                TDR_wait_done("*OPC?");

                if (i == 0)
                    iniManager.WriteIniFile("TDR_Setting", "Tr5_Tdd22_Rise_time", rise_time.ToString());
                else
                    iniManager.WriteIniFile("TDR_Setting", "Tr5_Tcc22_Rise_time", rise_time.ToString());
                //MessageBox.Show(value);
                Thread.Sleep(100);
            }

            //mbSession_E5071C.RawIO.Write(":MMEM:STOR:STYP CDST");
            //mbSession_E5071C.RawIO.Write(":MMEM:STOR \"D:\\CAMS_TDR_STA\\HFR7_13.STA\"");
        }
        void TDR_Query_response_val_string(string command, string parameter)
        {
            string value;
            mbSession_E5071C_tdr.RawIO.Write(command);
            value = mbSession_E5071C_tdr.RawIO.ReadString();
            if (value.ToUpper() != parameter.ToUpper())
            {
                command = command.Replace("?", " " + parameter.Replace("\n", ""));
                mbSession_E5071C_tdr.RawIO.Write(command);
                TDR_wait_done("*OPC?");
            }
        }

        void Query_response_val_string(string command, string parameter)
        {
            string value;
            mbSession_E5071C.RawIO.Write(command);
            value = mbSession_E5071C.RawIO.ReadString();
            if (value.ToUpper() != parameter.ToUpper())
            {
                command = command.Replace("?", " " + parameter.Replace("\n", ""));
                mbSession_E5071C.RawIO.Write(command);
                TDR_wait_done("*OPC?");
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

        void Calibration_TDR_HFR_7_3_Rise_time_75p(string Test_DATA)
        {
            ////Test ID HFR7-3

            string value;
            string rise_time;
            string used_trace;

            mbSession_E5071C_tdr.RawIO.Write(":TRIG:MODE RUN");
            TDR_wait_done("*OPC?");
            for (int i = 2; i <= 6; i = i + 4)
            {
                if(i == 2)
                {
                    rise_time = iniManager.ReadIniFile("HFR7-3", Test_DATA+"_Tdd11_Rise_time", "");
                }
                else
                {
                    rise_time = iniManager.ReadIniFile("HFR7-3", Test_DATA + "_Tdd22_Rise_time", "");
                }

                used_trace = "TRAC" + i.ToString();
                mbSession_E5071C_tdr.RawIO.Write(":CALC1:ATR:ACT " + i);
                //mbSession_E5071C_tdr.RawIO.Write(":CALC1:" + used_trace + ":PAR \"Tdd11\"");
                TDR_wait_done("*OPC?");
                

                mbSession_E5071C.RawIO.Write(":DISP:WIND1:" + used_trace + ":MEM?");
                string response_val = mbSession_E5071C.RawIO.ReadString();
                if (response_val == "1\n")
                {
                    mbSession_E5071C.RawIO.Write(":DISP:WIND1:" + used_trace + ":STAT OFF");
                    mbSession_E5071C.RawIO.Write(":DISP:WIND1:" + used_trace + ":MEM OFF");
                    mbSession_E5071C.RawIO.Write(":DISP:WIND1:" + used_trace + ":STAT ON");
                }
                wait_done("*OPC?");

                mbSession_E5071C_tdr.RawIO.Write(":CALC1:" + used_trace + ":FORM Volt");
                mbSession_E5071C_tdr.RawIO.Write(":DISP:" + used_trace + ":X:SCAL:PDIV 500e-12");
                mbSession_E5071C_tdr.RawIO.Write(":DISP:" + used_trace + ":X:SCAL:RLEV -2e-9");
                mbSession_E5071C_tdr.RawIO.Write(":DISP:" + used_trace + ":Y:SCAL:PDIV 50e-3");
                mbSession_E5071C_tdr.RawIO.Write(":DISP:" + used_trace + ":Y:SCAL:RLEV 0");
                mbSession_E5071C_tdr.RawIO.Write(":CALC1:" + used_trace + ":TTIM:STAT ON");
                mbSession_E5071C_tdr.RawIO.Write(":CALC1:" + used_trace + ":TTIM:THR T1_9");
                mbSession_E5071C_tdr.RawIO.Write(":CALC1:" + used_trace + ":TIME:STEP:RTIM:THR T1_9");
                mbSession_E5071C_tdr.RawIO.Write(":CALC1:" + used_trace + ":TIME:STEP:RTIM:DATA " + rise_time);
                TDR_wait_done("*OPC?");
                mbSession_E5071C.RawIO.Write(":DISP:WIND1:" + used_trace + ":Y:RPOS 1");
                wait_done("*OPC?");

                mbSession_E5071C_tdr.RawIO.Write(":CALC1:" + used_trace + ":TTIM:DATA?");
                value = mbSession_E5071C_tdr.RawIO.ReadString();
                TDR_wait_done("*OPC?");

                while (true)
                {
                    if (Convert.ToDouble(value) > 74.9e-12 & Convert.ToDouble(value) < 75e-12)
                    {
                        break;
                    }
                    else if (Convert.ToDouble(value) < 74.9e-12)
                    {
                        rise_time = (Convert.ToDouble(rise_time) + 1e-13).ToString();
                        mbSession_E5071C_tdr.RawIO.Write(":CALC1:" + used_trace + ":TIME:STEP:RTIM:DATA " + rise_time);
                        mbSession_E5071C_tdr.RawIO.Write(":CALC1:" + used_trace + ":TTIM:DATA?");
                        value = mbSession_E5071C_tdr.RawIO.ReadString();
                    }
                    else if (Convert.ToDouble(value) > 75e-12)
                    {
                        rise_time = (Convert.ToDouble(rise_time) - 1e-13).ToString();
                        mbSession_E5071C_tdr.RawIO.Write(":CALC1:" + used_trace + ":TIME:STEP:RTIM:DATA " + rise_time);
                        mbSession_E5071C_tdr.RawIO.Write(":CALC1:" + used_trace + ":TTIM:DATA?");
                        value = mbSession_E5071C_tdr.RawIO.ReadString();
                    }
                }
                
                if (i == 2)
                {
                    iniManager.WriteIniFile("HFR7-3", Test_DATA + "_Tdd11_Rise_time", rise_time.ToString());
                }
                else
                {
                    iniManager.WriteIniFile("HFR7-3", Test_DATA + "_Tdd22_Rise_time", rise_time.ToString());
                }
                Thread.Sleep(100);
            }
        }
        void Calibration_TDR_HFR_7_3_DATA_MEM(string Test_DATA)
        {
            string used_trace;
            switch(Test_DATA)
            {
                case "D0":
                    MessageBox.Show("請接上治具，組合:(D0+,D0-,D0+,D0-)");
                    break;
                case "D1":
                    MessageBox.Show("請接上治具，組合:(D1+,D1-,D1+,D1-)");
                    break;
                case "D2":
                    MessageBox.Show("請接上治具，組合:(D2+,D2-,D2+,D2-)");
                    break;
                case "D3":
                    MessageBox.Show("請接上治具，組合:(D3+,D3-,D3+,D3-)");
                    break;
            }

            //帶入DeEmbed檔案
            load_DeEmbed();

            while(true)
            {
                for (int i = 2; i <= 6; i = i + 4)
                {
                    used_trace = "TRAC" + i.ToString();
                    mbSession_E5071C_tdr.RawIO.Write(":CALC1:ATR:ACT " + i);
                    mbSession_E5071C_tdr.RawIO.Write(":CALC1:" + used_trace + ":FORM IMP");
                    mbSession_E5071C_tdr.RawIO.Write(":DISP:" + used_trace + ":X:SCAL:PDIV 500e-12");
                    mbSession_E5071C_tdr.RawIO.Write(":DISP:" + used_trace + ":X:SCAL:RLEV -750e-12");
                    mbSession_E5071C_tdr.RawIO.Write(":DISP:" + used_trace + ":Y:SCAL:PDIV 10");
                    mbSession_E5071C_tdr.RawIO.Write(":DISP:" + used_trace + ":Y:SCAL:RLEV 100");
                    TDR_wait_done("*OPC?");

                    mbSession_E5071C.RawIO.Write(":DISP:WIND1:" + used_trace + ":Y:RPOS 5");
                    mbSession_E5071C.RawIO.Write(":DISP:CCL");
                    mbSession_E5071C.RawIO.Write("*CLS");

                    mbSession_E5071C.RawIO.Write(":CALC1:" + used_trace + ":MARK1 OFF");
                    mbSession_E5071C.RawIO.Write(":CALC1:" + used_trace + ":MARK2 OFF");
                    mbSession_E5071C.RawIO.Write(":CALC1:" + used_trace + ":MARK3 OFF");
                    mbSession_E5071C.RawIO.Write(":CALC1:" + used_trace + ":MARK4 OFF");
                    mbSession_E5071C.RawIO.Write(":CALC1:" + used_trace + ":MARK5 OFF");
                    mbSession_E5071C.RawIO.Write(":CALC1:" + used_trace + ":MARK6 OFF");
                    mbSession_E5071C.RawIO.Write(":CALC1:" + used_trace + ":MARK7 OFF");
                    mbSession_E5071C.RawIO.Write(":CALC1:" + used_trace + ":MARK8 OFF");
                    mbSession_E5071C.RawIO.Write(":CALC1:MARK:FUNC:DOM OFF");
                    mbSession_E5071C.RawIO.Write(":CALC1:" + used_trace + ":MARK1 ON");
                    mbSession_E5071C.RawIO.Write(":CALC1:" + used_trace + ":MARK1:FUNC:TYPE PEAK");
                    mbSession_E5071C.RawIO.Write(":CALC1:" + used_trace + ":MARK1:FUNC:PPOL NEG");
                    mbSession_E5071C.RawIO.Write(":CALC1:" + used_trace + ":MARK1:FUNC:PEXC 0");
                    mbSession_E5071C.RawIO.Write(":CALC1:" + used_trace + ":MARK1:FUNC:TYPE LPE");
                    wait_done("*OPC?");
                    string val;
                    while (true)
                    {
                        mbSession_E5071C.RawIO.Write(":CALC1:MARK1:FUNC:EXEC");
                        mbSession_E5071C.RawIO.Write(":SYST:ERR?");
                        val = mbSession_E5071C.RawIO.ReadString();
                        if (val == "+41,\"Peak not found\"\n")
                        {
                            mbSession_E5071C.RawIO.Write(":DISP:CCL");
                            break;
                        }
                    }
                    mbSession_E5071C.RawIO.Write(":CALC1:MARK1:FUNC:TYPE RPE");
                    //mbSession_E5071C.RawIO.Write(":CALC1:MARK1:FUNC:EXEC");
                    mbSession_E5071C.RawIO.Write(":CALC1:" + used_trace + ":MARK1:DATA?");
                    MARKER1_DATA = mbSession_E5071C.RawIO.ReadString().Split(',');
                    double MARKER1_DATA_Before_val = Convert.ToDouble(MARKER1_DATA[0]);
                    while (true)
                    {
                        mbSession_E5071C.RawIO.Write(":CALC1:MARK1:FUNC:EXEC");
                        mbSession_E5071C.RawIO.Write(":CALC1:" + used_trace + ":MARK1:DATA?");
                        MARKER1_DATA = mbSession_E5071C.RawIO.ReadString().Split(',');
                        if (Convert.ToDouble(MARKER1_DATA[0]) - MARKER1_DATA_Before_val > 100)
                        {
                            mbSession_E5071C.RawIO.Write(":CALC1:MARK1:FUNC:TYPE LPE");
                            mbSession_E5071C.RawIO.Write(":CALC1:MARK1:FUNC:EXEC");
                            mbSession_E5071C.RawIO.Write(":CALC1:" + used_trace + ":MARK1:DATA?");
                            MARKER1_DATA = mbSession_E5071C.RawIO.ReadString().Split(',');
                            break;
                        }
                        MARKER1_DATA_Before_val = Convert.ToDouble(MARKER1_DATA[0]);
                    }

                    mbSession_E5071C.RawIO.Write(":CALC1:" + used_trace + ":MATH:MEM");
                    mbSession_E5071C.RawIO.Write(":DISP:WIND1:" + used_trace + ":STAT ON");
                    mbSession_E5071C.RawIO.Write(":DISP:WIND1:" + used_trace + ":MEM ON");
                    mbSession_E5071C.RawIO.Write(":CALC1:" + used_trace + ":MATH:MEM");
                    wait_done("*OPC?");


                    if (i == 2)
                    {
                        iniManager.WriteIniFile("HFR7-3", Test_DATA + "_Tdd11_MARK1_X_position", MARKER1_DATA[2]);
                    }
                    else
                    {
                        iniManager.WriteIniFile("HFR7-3", Test_DATA + "_Tdd22_MARK1_X_position", MARKER1_DATA[2]);
                    }
                }

                if (MessageBox.Show(Test_DATA +"校正完成，是否下一步", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    break;
                }
                else
                {
                    
                }
            }
        }
        private void Near_D0_button_Click(object sender, EventArgs e)
        {
            switch_port1_3 = "1B,3B";
            switch_port2_4 = "";
            fixture_port1_3 = Near_D0_button.Text;
            Reset_Near_button_state();
            Reset_Far_button_state();
            Near_D0_button.FlatAppearance.BorderSize = 2;
            Near_D0_button.FlatAppearance.BorderColor = Near_D0_color;
            One_dimensional = 0;
            Calibration_sub_items_done();

        }

        private void Near_D1_button_Click(object sender, EventArgs e)
        {
            switch_port1_3 = "1C,3C";
            switch_port2_4 = "";
            fixture_port1_3 = Near_D1_button.Text;
            Reset_Near_button_state();
            Reset_Far_button_state();
            Near_D1_button.FlatAppearance.BorderSize = 2;
            Near_D1_button.FlatAppearance.BorderColor = Near_D1_color;
            One_dimensional = 1;
            Calibration_sub_items_done();
        }

        private void Near_D2_button_Click(object sender, EventArgs e)
        {
            switch_port1_3 = "1D,3D";
            switch_port2_4 = "";
            fixture_port1_3 = Near_D2_button.Text;
            Reset_Near_button_state();
            Reset_Far_button_state();
            Near_D2_button.FlatAppearance.BorderSize = 2;
            Near_D2_button.FlatAppearance.BorderColor = Near_D2_color;
            One_dimensional = 2;
            Calibration_sub_items_done();
        }

        private void Near_D3_button_Click(object sender, EventArgs e)
        {
            switch_port1_3 = "1E,3E";
            switch_port2_4 = "";
            fixture_port1_3 = Near_D3_button.Text;
            Reset_Near_button_state();
            Reset_Far_button_state();
            Near_D3_button.FlatAppearance.BorderSize = 2;
            Near_D3_button.FlatAppearance.BorderColor = Near_D3_color;
            One_dimensional = 3;
            Calibration_sub_items_done();
        }
        private void HEAC_button_Click(object sender, EventArgs e)
        {
            string ECAL_Message = "";
            timer1.Enabled = false;
            ECAL_button.Enabled = false;
            OPEN_button.Enabled = false;
            HEAC_button.Enabled = false;
            if (HF7_12_env) //環境已改變為HF7-12的校正環境，因此需要再次改變校正環境
            {
                HF7_12_env = false;
                Calibration_ini_env_set();
            }
            mbSession_E5071C.RawIO.Write(":DISP:CCL");
            if (Automation)
            {
                MessageBox.Show("請插上ECAL,Near:HEAC; Far:HEAC (1F,3F,2F,4F)");
                switch_command = "1F,3F,2F,4F,";
                Switch(switch_command);
                ECAL_Message = ECAL_Calibration();
            }
            else
            {
                mbSession_E5071C.RawIO.Write(":MMEM:LOAD \"D:\\CAMS_TDR_STA\\HDMI_ECAL_" + "1F,3F,2F,4F," + "_env.STA\"");
                wait_done("*OPC?");
            }

            if (ECAL_Message != "+32,\"ECal module not in appropriate RF path\"\n")
            {
                mbSession_E5071C_tdr.RawIO.Write(":SENS:DLEN:DATA 25e-9");
                MessageBox.Show("請拔除Fixture");
                Calibration_TDR_HFR_7_13_to_7_15_Rise_time_1n();
                load_DeEmbed();
                mbSession_E5071C.RawIO.Write(":DISP:CCL");
                wait_done("*OPC?");
                mbSession_E5071C.RawIO.Write(":MMEM:STOR:STYP CDST");
                mbSession_E5071C.RawIO.Write(":MMEM:STOR \"D:\\CAMS_TDR_STA\\HDMI_HEAC_env.STA\"");
                wait_done("*OPC?");

                MessageBox.Show("請插上ECAL");
                HF7_12_env = true;
                HF7_12_env_set();
                MessageBox.Show("Finish");
            }
            //ECAL_button.Enabled = true;
            OPEN_button.Enabled = true;
            HEAC_button.Enabled = true;
            timer1.Enabled = true;
        }
        private void HEAC_button_MouseDown(object sender, MouseEventArgs e)
        {
            HEAC_button.BackgroundImage = Properties.Resources.button02;
            HEAC_button.ForeColor = Color.White;
        }
        private void HEAC_button_MouseMove(object sender, MouseEventArgs e)
        {
            HEAC_button.BackgroundImage = Properties.Resources.button02;
            HEAC_button.ForeColor = Color.White;
        }
        private void HEAC_button_MouseLeave(object sender, EventArgs e)
        {
            HEAC_button.BackgroundImage = Properties.Resources.button01;
            HEAC_button.ForeColor = Color.Black;
        }
        private void HEAC_button_MouseUp(object sender, MouseEventArgs e)
        {
            HEAC_button.BackgroundImage = Properties.Resources.button01;
            HEAC_button.ForeColor = Color.Black;
        }
        private void Far_D0_button_Click(object sender, EventArgs e)
        {
            switch_port2_4 = "2B,4B";
            fixture_port2_4 = Far_D0_button.Text;
            Reset_Far_button_state();
            Two_dimensional = 0;
            Far_D0_button.BackColor = Color.FromArgb(0, 162, 232);
            Calibration_sub_items_done();
        }

        private void Far_D1_button_Click(object sender, EventArgs e)
        {
            switch_port2_4 = "2C,4C";
            fixture_port2_4 = Far_D1_button.Text;
            Reset_Far_button_state();
            Two_dimensional = 1;
            Far_D1_button.BackColor = Color.FromArgb(0, 162, 232);
            Calibration_sub_items_done();
        }

        private void Far_D2_button_Click(object sender, EventArgs e)
        {
            switch_port2_4 = "2D,4D";
            fixture_port2_4 = Far_D2_button.Text;
            Reset_Far_button_state();
            Two_dimensional = 2;
            Far_D2_button.BackColor = Color.FromArgb(0, 162, 232);
            Calibration_sub_items_done();
        }

        private void Far_D3_button_Click(object sender, EventArgs e)
        {
            switch_port2_4 = "2E,4E";
            fixture_port2_4 = Far_D3_button.Text;
            Reset_Far_button_state();
            Two_dimensional = 3;
            Far_D3_button.BackColor = Color.FromArgb(0, 162, 232);
            Calibration_sub_items_done();
        }

        //將所有按鈕恢復到原本的顏色，如果lock selection不是打勾的狀態下會把所有的按鈕邊框變為零-----------------------------------------------------------------
        private void Reset_Near_button_state()
        {
            if(!DO_DONE)
                Near_D0_button.FlatAppearance.BorderSize = 0;
            if (!D1_DONE)
                Near_D1_button.FlatAppearance.BorderSize = 0;
            if (!D2_DONE)
                Near_D2_button.FlatAppearance.BorderSize = 0;
            if (!D3_DONE)
                Near_D3_button.FlatAppearance.BorderSize = 0;

            HEAC_button.FlatAppearance.BorderSize = 0;
        }
        private void Reset_Far_button_state()
        {
            Far_D0_button.BackColor = Color.Transparent;
            Far_D1_button.BackColor = Color.Transparent;
            Far_D2_button.BackColor = Color.Transparent;
            Far_D3_button.BackColor = Color.Transparent;

            Far_D0_button.FlatAppearance.BorderSize = 0;
            Far_D1_button.FlatAppearance.BorderSize = 0;
            Far_D2_button.FlatAppearance.BorderSize = 0;
            Far_D3_button.FlatAppearance.BorderSize = 0;
        }

        //檢查那些測試項目已經完成，已經完成的按鈕會直接上色-----------------------------------------------------------------
        private void Calibration_sub_items_done()
        {
            //for (int one_DIM = 0; one_DIM < 4; one_DIM++)
            //{
                
            //}

            for (int two_DIM = 0; two_DIM < 4; two_DIM++)
            {
                if (calibration_done[One_dimensional, two_DIM] == 1)
                {
                    switch (two_DIM)
                    {
                        case 0:
                            Far_D0_button.BackColor = Color.FromArgb(0, 162, 232);
                            break;
                        case 1:
                            Far_D1_button.BackColor = Color.FromArgb(0, 162, 232);
                            break;
                        case 2:
                            Far_D2_button.BackColor = Color.FromArgb(0, 162, 232);
                            break;
                        case 3:
                            Far_D3_button.BackColor = Color.FromArgb(0, 162, 232);
                            break;
                    }
                }
            }
        }

        private void Calibration_items_done()
        {
            for (int one_DIM = 0; one_DIM < 4; one_DIM++)
            {
                bool done = true; //如果掃秒到有0的數值時，代表該測項尚未完成，因此done的變數會為false，反之，已完成按鈕會上特定顏色代表完成
                for (int two_DIM = 0; two_DIM < 4; two_DIM++)
                {
                    if (calibration_done[one_DIM, two_DIM] != 1)
                    {
                        done = false;
                    }
                }

                if(done)
                {
                    switch (one_DIM)
                    {
                        case 0:
                            Near_D0_color = Color.Green;
                            Near_D0_button.FlatAppearance.BorderColor = Near_D0_color;
                            DO_DONE = true;
                            break;
                        case 1:
                            Near_D1_color = Color.Green;
                            Near_D1_button.FlatAppearance.BorderColor = Near_D1_color;
                            D1_DONE = true;
                            break;
                        case 2:
                            Near_D2_color = Color.Green;
                            Near_D2_button.FlatAppearance.BorderColor = Near_D2_color;
                            D2_DONE = true;
                            break;
                        case 3:
                            Near_D3_color = Color.Green;
                            Near_D3_button.FlatAppearance.BorderColor = Near_D3_color;
                            D3_DONE = true;
                            break;
                    }
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            switch_command = switch_port1_3 + "," + switch_port2_4 + ",";
            fixture_port_name = "Near: (" + fixture_port1_3 + ");"+ " Far: (" + fixture_port2_4 + ")";
            if (switch_command.Length < 12)
            {
                ECAL_button.Enabled = false;
            }
            else
            {
                ECAL_button.Enabled = true;
            }
        }

        private void OPEN_button_Click(object sender, EventArgs e)
        {
            bool Cal_Ready = false;
            Cal_Ready = check_STA_files();
            if(Cal_Ready)
            {
                if (HF7_12_env) //環境已改變為HF7-12的校正環境，因此需要再次改變校正環境
                {
                    HF7_12_env = false;
                    Calibration_ini_env_set();
                }
                Switch("1B,3B,2B,4B,");
                MessageBox.Show("請拔除Fixture");
                mbSession_E5071C.RawIO.Write(":MMEM:LOAD \"D:\\CAMS_TDR_STA\\HDMI_ECAL_" + "1B,3B,2B,4B," + "_env.STA\"");
                wait_done("*OPC?");
                Calibration_TDR_HFR_7_3_Rise_time_75p("D0");
                Calibration_TDR_HFR_7_3_DATA_MEM("D0");
                mbSession_E5071C.RawIO.Write(":DISP:CCL");
                wait_done("*OPC?");
                mbSession_E5071C.RawIO.Write(":MMEM:STOR:STYP CDST");
                mbSession_E5071C.RawIO.Write(":MMEM:STOR \"D:\\CAMS_TDR_STA\\HDMI_HF7-3_" + "1B,3B,2B,4B," + "_env.STA\"");
                wait_done("*OPC?");

                Switch("1C,3C,2C,4C,");
                if (!Automation)
                    MessageBox.Show("請拔除Fixture");
                mbSession_E5071C.RawIO.Write(":MMEM:LOAD \"D:\\CAMS_TDR_STA\\HDMI_ECAL_" + "1C,3C,2C,4C," + "_env.STA\"");
                wait_done("*OPC?");
                Calibration_TDR_HFR_7_3_Rise_time_75p("D1");
                Calibration_TDR_HFR_7_3_DATA_MEM("D1");
                mbSession_E5071C.RawIO.Write(":DISP:CCL");
                wait_done("*OPC?");
                mbSession_E5071C.RawIO.Write(":MMEM:STOR:STYP CDST");
                mbSession_E5071C.RawIO.Write(":MMEM:STOR \"D:\\CAMS_TDR_STA\\HDMI_HF7-3_" + "1C,3C,2C,4C," + "_env.STA\"");
                wait_done("*OPC?");

                Switch("1D,3D,2D,4D,");
                if (!Automation)
                    MessageBox.Show("請拔除Fixture");
                mbSession_E5071C.RawIO.Write(":MMEM:LOAD \"D:\\CAMS_TDR_STA\\HDMI_ECAL_" + "1D,3D,2D,4D," + "_env.STA\"");
                wait_done("*OPC?");
                Calibration_TDR_HFR_7_3_Rise_time_75p("D2");
                Calibration_TDR_HFR_7_3_DATA_MEM("D2");
                mbSession_E5071C.RawIO.Write(":DISP:CCL");
                wait_done("*OPC?");
                mbSession_E5071C.RawIO.Write(":MMEM:STOR:STYP CDST");
                mbSession_E5071C.RawIO.Write(":MMEM:STOR \"D:\\CAMS_TDR_STA\\HDMI_HF7-3_" + "1D,3D,2D,4D," + "_env.STA\"");
                wait_done("*OPC?");

                Switch("1E,3E,2E,4E,");
                if (!Automation)
                    MessageBox.Show("請拔除Fixture");
                mbSession_E5071C.RawIO.Write(":MMEM:LOAD \"D:\\CAMS_TDR_STA\\HDMI_ECAL_" + "1E,3E,2E,4E," + "_env.STA\"");
                wait_done("*OPC?");
                Calibration_TDR_HFR_7_3_Rise_time_75p("D3");
                Calibration_TDR_HFR_7_3_DATA_MEM("D3");
                mbSession_E5071C.RawIO.Write(":DISP:CCL");
                wait_done("*OPC?");
                mbSession_E5071C.RawIO.Write(":MMEM:STOR:STYP CDST");
                mbSession_E5071C.RawIO.Write(":MMEM:STOR \"D:\\CAMS_TDR_STA\\HDMI_HF7-3_" + "1E,3E,2E,4E," + "_env.STA\"");
                wait_done("*OPC?");

                MessageBox.Show("Finish");
            }
        }
        private void UNECAL_button_MouseDown(object sender, MouseEventArgs e)
        {
            OPEN_button.BackgroundImage = Properties.Resources.button02;
            OPEN_button.ForeColor = Color.White;
        }
        private void UNECAL_button_MouseMove(object sender, MouseEventArgs e)
        {
            OPEN_button.BackgroundImage = Properties.Resources.button02;
            OPEN_button.ForeColor = Color.White;
        }

        private void UNECAL_button_MouseLeave(object sender, EventArgs e)
        {
            OPEN_button.BackgroundImage = Properties.Resources.button01;
            OPEN_button.ForeColor = Color.Black;
        }

        private void UNECAL_button_MouseUp(object sender, MouseEventArgs e)
        {
            OPEN_button.BackgroundImage = Properties.Resources.button01;
            OPEN_button.ForeColor = Color.Black;
        }
        bool check_STA_files()
        {
            bool check_status = true;
            string read_value1 = "";
            string read_value2 = "";
            string Not_found_item = "";
            string[] Open_STA_files = new string[] { "HDMI_ECAL_1B,3B,2B,4B,_env.STA", "HDMI_ECAL_1C,3C,2C,4C,_env.STA", "HDMI_ECAL_1D,3D,2D,4D,_env.STA", "HDMI_ECAL_1E,3E,2E,4E,_env.STA" };

            mbSession_E5071C.RawIO.Write(":MMEM:CAT? " + "\"" + "D:\\CAMS_TDR_STA\\" + "\"");
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

            foreach (string item in Open_STA_files)
            {
                if (read_value1.Contains(item.ToUpper()))
                {

                }
                else
                {
                    string ccombination = "";
                    ccombination = "[" + item.Replace("HDMI_ECAL_","").Replace("_env.STA","") + "]";
                    Not_found_item = Not_found_item + ccombination + "\n";
                }
            }

            if (Not_found_item != "")
            {
                MessageBox.Show("校正檔案有缺少!!!\n缺少組合:\n" + Not_found_item, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                check_status = false;
            }
            return check_status;
        }

        private void load_DeEmbed()
        {
            Query_response_val_string(":CALC1:FSIM:SEND:DEEM:PORT1:USER:FIL?", "\"D:\\CAMS_DEEMBEDD\\HDMI2.1 Cal_2X_Thru_1_Point1200_10MHz_to_12GHz.s2p\"\n");
            Query_response_val_string(":CALC1:FSIM:SEND:DEEM:PORT3:USER:FIL?", "\"D:\\CAMS_DEEMBEDD\\HDMI2.1 Cal_2X_Thru_1_Point1200_10MHz_to_12GHz.s2p\"\n");
            Query_response_val_string(":CALC1:FSIM:SEND:DEEM:PORT2:USER:FIL?", "\"D:\\CAMS_DEEMBEDD\\HDMI2.1 Cal_2X_Thru_2_Point1200_10MHz_to_12GHz.s2p\"\n");
            Query_response_val_string(":CALC1:FSIM:SEND:DEEM:PORT4:USER:FIL?", "\"D:\\CAMS_DEEMBEDD\\HDMI2.1 Cal_2X_Thru_2_Point1200_10MHz_to_12GHz.s2p\"\n");
            Query_response_val_string(":CALC1:FSIM:SEND:DEEM:PORT1?", "USER\n");
            Query_response_val_string(":CALC1:FSIM:SEND:DEEM:PORT2?", "USER\n");
            Query_response_val_string(":CALC1:FSIM:SEND:DEEM:PORT3?", "USER\n");
            Query_response_val_string(":CALC1:FSIM:SEND:DEEM:PORT4?", "USER\n");
            Query_response_val_string(":CALC1:FSIM:SEND:DEEM:STAT?", "1\n");

            Query_response_val_string(":CALC2:FSIM:SEND:DEEM:PORT1:USER:FIL?", "\"D:\\CAMS_DEEMBEDD\\HDMI2.1 Cal_2X_Thru_1_Point1200_10MHz_to_12GHz.s2p\"\n");
            Query_response_val_string(":CALC2:FSIM:SEND:DEEM:PORT3:USER:FIL?", "\"D:\\CAMS_DEEMBEDD\\HDMI2.1 Cal_2X_Thru_1_Point1200_10MHz_to_12GHz.s2p\"\n");
            Query_response_val_string(":CALC2:FSIM:SEND:DEEM:PORT2:USER:FIL?", "\"D:\\CAMS_DEEMBEDD\\HDMI2.1 Cal_2X_Thru_2_Point1200_10MHz_to_12GHz.s2p\"\n");
            Query_response_val_string(":CALC2:FSIM:SEND:DEEM:PORT4:USER:FIL?", "\"D:\\CAMS_DEEMBEDD\\HDMI2.1 Cal_2X_Thru_2_Point1200_10MHz_to_12GHz.s2p\"\n");
            Query_response_val_string(":CALC2:FSIM:SEND:DEEM:PORT1?", "USER\n");
            Query_response_val_string(":CALC2:FSIM:SEND:DEEM:PORT2?", "USER\n");
            Query_response_val_string(":CALC2:FSIM:SEND:DEEM:PORT3?", "USER\n");
            Query_response_val_string(":CALC2:FSIM:SEND:DEEM:PORT4?", "USER\n");
            Query_response_val_string(":CALC2:FSIM:SEND:DEEM:STAT?", "1\n");
        }
        void load_ch2_DeEmbed()
        {
            Query_response_val_string(":CALC2:FSIM:SEND:DEEM:PORT1:USER:FIL?", "\"D:\\CAMS_DEEMBEDD\\HDMI2.1 Cal_2X_Thru_1_Point1200_10MHz_to_12GHz.s2p\"\n");
            Query_response_val_string(":CALC2:FSIM:SEND:DEEM:PORT3:USER:FIL?", "\"D:\\CAMS_DEEMBEDD\\HDMI2.1 Cal_2X_Thru_1_Point1200_10MHz_to_12GHz.s2p\"\n");
            Query_response_val_string(":CALC2:FSIM:SEND:DEEM:PORT2:USER:FIL?", "\"D:\\CAMS_DEEMBEDD\\HDMI2.1 Cal_2X_Thru_2_Point1200_10MHz_to_12GHz.s2p\"\n");
            Query_response_val_string(":CALC2:FSIM:SEND:DEEM:PORT4:USER:FIL?", "\"D:\\CAMS_DEEMBEDD\\HDMI2.1 Cal_2X_Thru_2_Point1200_10MHz_to_12GHz.s2p\"\n");
            Query_response_val_string(":CALC2:FSIM:SEND:DEEM:PORT1?", "USER\n");
            Query_response_val_string(":CALC2:FSIM:SEND:DEEM:PORT2?", "USER\n");
            Query_response_val_string(":CALC2:FSIM:SEND:DEEM:PORT3?", "USER\n");
            Query_response_val_string(":CALC2:FSIM:SEND:DEEM:PORT4?", "USER\n");
            Query_response_val_string(":CALC2:FSIM:SEND:DEEM:STAT?", "1\n");
        }

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
                MessageBox.Show(exp.Message);
            }
        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }
    }
}
