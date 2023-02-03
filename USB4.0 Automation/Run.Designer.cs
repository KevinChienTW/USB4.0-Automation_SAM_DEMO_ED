namespace USB4._0_Automation
{
    partial class Run_setting
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Run_setting));
            this.measure_button = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.Dut_name = new System.Windows.Forms.TextBox();
            this.serialPort1 = new System.IO.Ports.SerialPort(this.components);
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.download_files_check = new System.Windows.Forms.CheckBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.fast_mode_cb = new System.Windows.Forms.CheckBox();
            this.test4 = new System.Windows.Forms.CheckBox();
            this.test1 = new System.Windows.Forms.CheckBox();
            this.test17 = new System.Windows.Forms.CheckBox();
            this.test2 = new System.Windows.Forms.CheckBox();
            this.test13 = new System.Windows.Forms.CheckBox();
            this.test19 = new System.Windows.Forms.CheckBox();
            this.test5 = new System.Windows.Forms.CheckBox();
            this.test3 = new System.Windows.Forms.CheckBox();
            this.test12 = new System.Windows.Forms.CheckBox();
            this.Save_plots = new System.Windows.Forms.CheckBox();
            this.test9 = new System.Windows.Forms.CheckBox();
            this.test18 = new System.Windows.Forms.CheckBox();
            this.test16 = new System.Windows.Forms.CheckBox();
            this.test14 = new System.Windows.Forms.CheckBox();
            this.test11 = new System.Windows.Forms.CheckBox();
            this.test15 = new System.Windows.Forms.CheckBox();
            this.test8 = new System.Windows.Forms.CheckBox();
            this.test7 = new System.Windows.Forms.CheckBox();
            this.TDR_measure_button = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.case_name = new System.Windows.Forms.Label();
            this.Project_list = new System.Windows.Forms.ListView();
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.Project_list_2 = new System.Windows.Forms.ListView();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.label_pass_num = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.panel5 = new System.Windows.Forms.Panel();
            this.label_fail_num = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel5.SuspendLayout();
            this.SuspendLayout();
            // 
            // measure_button
            // 
            this.measure_button.BackColor = System.Drawing.Color.Transparent;
            this.measure_button.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("measure_button.BackgroundImage")));
            this.measure_button.FlatAppearance.BorderSize = 0;
            this.measure_button.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.measure_button.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.measure_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.measure_button.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.measure_button.ForeColor = System.Drawing.Color.White;
            this.measure_button.Location = new System.Drawing.Point(547, 209);
            this.measure_button.Name = "measure_button";
            this.measure_button.Size = new System.Drawing.Size(175, 41);
            this.measure_button.TabIndex = 1;
            this.measure_button.Text = "Measure";
            this.measure_button.UseVisualStyleBackColor = false;
            this.measure_button.Click += new System.EventHandler(this.measure_Click);
            this.measure_button.MouseDown += new System.Windows.Forms.MouseEventHandler(this.measure_mouseDown);
            this.measure_button.MouseLeave += new System.EventHandler(this.measure_mouseLeave);
            this.measure_button.MouseMove += new System.Windows.Forms.MouseEventHandler(this.measure_mouseMove);
            this.measure_button.MouseUp += new System.Windows.Forms.MouseEventHandler(this.measure_mouseUp);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(23, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 21);
            this.label1.TabIndex = 1;
            this.label1.Text = "DUT Name:";
            // 
            // Dut_name
            // 
            this.Dut_name.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Dut_name.Location = new System.Drawing.Point(129, 12);
            this.Dut_name.Name = "Dut_name";
            this.Dut_name.Size = new System.Drawing.Size(183, 24);
            this.Dut_name.TabIndex = 0;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictureBox1.BackgroundImage")));
            this.pictureBox1.Location = new System.Drawing.Point(99, -9);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(534, 150);
            this.pictureBox1.TabIndex = 5;
            this.pictureBox1.TabStop = false;
            // 
            // download_files_check
            // 
            this.download_files_check.AutoSize = true;
            this.download_files_check.BackColor = System.Drawing.Color.Transparent;
            this.download_files_check.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.download_files_check.ForeColor = System.Drawing.Color.Black;
            this.download_files_check.Location = new System.Drawing.Point(455, 12);
            this.download_files_check.Name = "download_files_check";
            this.download_files_check.Size = new System.Drawing.Size(141, 25);
            this.download_files_check.TabIndex = 6;
            this.download_files_check.Text = "Download files";
            this.download_files_check.UseVisualStyleBackColor = false;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Transparent;
            this.panel1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panel1.BackgroundImage")));
            this.panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel1.Controls.Add(this.fast_mode_cb);
            this.panel1.Controls.Add(this.test4);
            this.panel1.Controls.Add(this.test1);
            this.panel1.Controls.Add(this.test17);
            this.panel1.Controls.Add(this.test2);
            this.panel1.Controls.Add(this.test13);
            this.panel1.Controls.Add(this.test19);
            this.panel1.Controls.Add(this.test5);
            this.panel1.Controls.Add(this.test3);
            this.panel1.Controls.Add(this.test12);
            this.panel1.Controls.Add(this.Save_plots);
            this.panel1.Controls.Add(this.test9);
            this.panel1.Controls.Add(this.test18);
            this.panel1.Controls.Add(this.test16);
            this.panel1.Controls.Add(this.measure_button);
            this.panel1.Controls.Add(this.test14);
            this.panel1.Controls.Add(this.test11);
            this.panel1.Controls.Add(this.download_files_check);
            this.panel1.Controls.Add(this.test15);
            this.panel1.Controls.Add(this.Dut_name);
            this.panel1.Controls.Add(this.test8);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.test7);
            this.panel1.Location = new System.Drawing.Point(1, 450);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(760, 273);
            this.panel1.TabIndex = 10;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // fast_mode_cb
            // 
            this.fast_mode_cb.AutoSize = true;
            this.fast_mode_cb.BackColor = System.Drawing.Color.Transparent;
            this.fast_mode_cb.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fast_mode_cb.ForeColor = System.Drawing.Color.Black;
            this.fast_mode_cb.Location = new System.Drawing.Point(599, 12);
            this.fast_mode_cb.Name = "fast_mode_cb";
            this.fast_mode_cb.Size = new System.Drawing.Size(108, 25);
            this.fast_mode_cb.TabIndex = 41;
            this.fast_mode_cb.Text = "Fast mode";
            this.fast_mode_cb.UseVisualStyleBackColor = false;
            // 
            // test4
            // 
            this.test4.AutoSize = true;
            this.test4.BackColor = System.Drawing.Color.Transparent;
            this.test4.ForeColor = System.Drawing.Color.Black;
            this.test4.Location = new System.Drawing.Point(327, 56);
            this.test4.Name = "test4";
            this.test4.Size = new System.Drawing.Size(167, 16);
            this.test4.TabIndex = 24;
            this.test4.Text = "SS Pair Differential Impedance";
            this.test4.UseVisualStyleBackColor = false;
            // 
            // test1
            // 
            this.test1.AutoSize = true;
            this.test1.BackColor = System.Drawing.Color.Transparent;
            this.test1.ForeColor = System.Drawing.Color.Black;
            this.test1.Location = new System.Drawing.Point(27, 56);
            this.test1.Name = "test1";
            this.test1.Size = new System.Drawing.Size(81, 16);
            this.test1.TabIndex = 12;
            this.test1.Text = "SS Pair ILfit";
            this.test1.UseVisualStyleBackColor = false;
            // 
            // test17
            // 
            this.test17.AutoSize = true;
            this.test17.BackColor = System.Drawing.Color.Transparent;
            this.test17.ForeColor = System.Drawing.Color.Black;
            this.test17.Location = new System.Drawing.Point(27, 208);
            this.test17.Name = "test17";
            this.test17.Size = new System.Drawing.Size(58, 16);
            this.test17.TabIndex = 38;
            this.test17.Text = "SDC21";
            this.test17.UseVisualStyleBackColor = false;
            // 
            // test2
            // 
            this.test2.AutoSize = true;
            this.test2.BackColor = System.Drawing.Color.Transparent;
            this.test2.ForeColor = System.Drawing.Color.Black;
            this.test2.Location = new System.Drawing.Point(27, 78);
            this.test2.Name = "test2";
            this.test2.Size = new System.Drawing.Size(71, 16);
            this.test2.TabIndex = 13;
            this.test2.Text = "SS Pair IL";
            this.test2.UseVisualStyleBackColor = false;
            // 
            // test13
            // 
            this.test13.AutoSize = true;
            this.test13.BackColor = System.Drawing.Color.Transparent;
            this.test13.ForeColor = System.Drawing.Color.Black;
            this.test13.Location = new System.Drawing.Point(574, 98);
            this.test13.Name = "test13";
            this.test13.Size = new System.Drawing.Size(138, 16);
            this.test13.TabIndex = 33;
            this.test13.Text = "DD Pair_Intra Pair Skew";
            this.test13.UseVisualStyleBackColor = false;
            // 
            // test19
            // 
            this.test19.AutoSize = true;
            this.test19.BackColor = System.Drawing.Color.Transparent;
            this.test19.ForeColor = System.Drawing.Color.Black;
            this.test19.Location = new System.Drawing.Point(27, 252);
            this.test19.Name = "test19";
            this.test19.Size = new System.Drawing.Size(241, 16);
            this.test19.TabIndex = 40;
            this.test19.Text = "IDDXT_1NEXT +FEXT and, IDDXT_2NEXT";
            this.test19.UseVisualStyleBackColor = false;
            // 
            // test5
            // 
            this.test5.AutoSize = true;
            this.test5.BackColor = System.Drawing.Color.Transparent;
            this.test5.ForeColor = System.Drawing.Color.Black;
            this.test5.Location = new System.Drawing.Point(327, 78);
            this.test5.Name = "test5";
            this.test5.Size = new System.Drawing.Size(206, 16);
            this.test5.TabIndex = 25;
            this.test5.Text = "SS Pair Single End Impedance(  L / R )";
            this.test5.UseVisualStyleBackColor = false;
            // 
            // test3
            // 
            this.test3.AutoSize = true;
            this.test3.BackColor = System.Drawing.Color.Transparent;
            this.test3.ForeColor = System.Drawing.Color.Black;
            this.test3.Location = new System.Drawing.Point(27, 98);
            this.test3.Name = "test3";
            this.test3.Size = new System.Drawing.Size(121, 16);
            this.test3.TabIndex = 23;
            this.test3.Text = "DD Pair_Attenuation";
            this.test3.UseVisualStyleBackColor = false;
            // 
            // test12
            // 
            this.test12.AutoSize = true;
            this.test12.BackColor = System.Drawing.Color.Transparent;
            this.test12.ForeColor = System.Drawing.Color.Black;
            this.test12.Location = new System.Drawing.Point(574, 76);
            this.test12.Name = "test12";
            this.test12.Size = new System.Drawing.Size(153, 16);
            this.test12.TabIndex = 32;
            this.test12.Text = "DD Pair_Propagation Delay";
            this.test12.UseVisualStyleBackColor = false;
            // 
            // Save_plots
            // 
            this.Save_plots.AutoSize = true;
            this.Save_plots.BackColor = System.Drawing.Color.Transparent;
            this.Save_plots.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Save_plots.ForeColor = System.Drawing.Color.Black;
            this.Save_plots.Location = new System.Drawing.Point(342, 12);
            this.Save_plots.Name = "Save_plots";
            this.Save_plots.Size = new System.Drawing.Size(107, 25);
            this.Save_plots.TabIndex = 37;
            this.Save_plots.Text = "Save plots";
            this.Save_plots.UseVisualStyleBackColor = false;
            // 
            // test9
            // 
            this.test9.AutoSize = true;
            this.test9.BackColor = System.Drawing.Color.Transparent;
            this.test9.ForeColor = System.Drawing.Color.Black;
            this.test9.Location = new System.Drawing.Point(327, 100);
            this.test9.Name = "test9";
            this.test9.Size = new System.Drawing.Size(171, 16);
            this.test9.TabIndex = 29;
            this.test9.Text = "Connector Impedance_( L / R )";
            this.test9.UseVisualStyleBackColor = false;
            // 
            // test18
            // 
            this.test18.AutoSize = true;
            this.test18.BackColor = System.Drawing.Color.Transparent;
            this.test18.ForeColor = System.Drawing.Color.Black;
            this.test18.Location = new System.Drawing.Point(27, 230);
            this.test18.Name = "test18";
            this.test18.Size = new System.Drawing.Size(113, 16);
            this.test18.TabIndex = 39;
            this.test18.Text = "INEXT and IFEXT";
            this.test18.UseVisualStyleBackColor = false;
            // 
            // test16
            // 
            this.test16.AutoSize = true;
            this.test16.BackColor = System.Drawing.Color.Transparent;
            this.test16.ForeColor = System.Drawing.Color.Black;
            this.test16.Location = new System.Drawing.Point(327, 122);
            this.test16.Name = "test16";
            this.test16.Size = new System.Drawing.Size(131, 16);
            this.test16.TabIndex = 36;
            this.test16.Text = "SS Pair Intra Pair Skew";
            this.test16.UseVisualStyleBackColor = false;
            // 
            // test14
            // 
            this.test14.AutoSize = true;
            this.test14.BackColor = System.Drawing.Color.Transparent;
            this.test14.ForeColor = System.Drawing.Color.Black;
            this.test14.Location = new System.Drawing.Point(27, 122);
            this.test14.Name = "test14";
            this.test14.Size = new System.Drawing.Size(219, 16);
            this.test14.TabIndex = 34;
            this.test14.Text = "SS Pair_Integrated Multi-reflection (IMR)";
            this.test14.UseVisualStyleBackColor = false;
            // 
            // test11
            // 
            this.test11.AutoSize = true;
            this.test11.BackColor = System.Drawing.Color.Transparent;
            this.test11.ForeColor = System.Drawing.Color.Black;
            this.test11.Location = new System.Drawing.Point(574, 56);
            this.test11.Name = "test11";
            this.test11.Size = new System.Drawing.Size(174, 16);
            this.test11.TabIndex = 31;
            this.test11.Text = "DD Pair_Differential Impedance";
            this.test11.UseVisualStyleBackColor = false;
            // 
            // test15
            // 
            this.test15.AutoSize = true;
            this.test15.BackColor = System.Drawing.Color.Transparent;
            this.test15.ForeColor = System.Drawing.Color.Black;
            this.test15.Location = new System.Drawing.Point(27, 144);
            this.test15.Name = "test15";
            this.test15.Size = new System.Drawing.Size(199, 16);
            this.test15.TabIndex = 35;
            this.test15.Text = "SS Pair_Integrated Return Loss (IRL)";
            this.test15.UseVisualStyleBackColor = false;
            // 
            // test8
            // 
            this.test8.AutoSize = true;
            this.test8.BackColor = System.Drawing.Color.Transparent;
            this.test8.ForeColor = System.Drawing.Color.Black;
            this.test8.Location = new System.Drawing.Point(27, 186);
            this.test8.Name = "test8";
            this.test8.Size = new System.Drawing.Size(58, 16);
            this.test8.TabIndex = 28;
            this.test8.Text = "SCD21";
            this.test8.UseVisualStyleBackColor = false;
            // 
            // test7
            // 
            this.test7.AutoSize = true;
            this.test7.BackColor = System.Drawing.Color.Transparent;
            this.test7.ForeColor = System.Drawing.Color.Black;
            this.test7.Location = new System.Drawing.Point(27, 164);
            this.test7.Name = "test7";
            this.test7.Size = new System.Drawing.Size(58, 16);
            this.test7.TabIndex = 27;
            this.test7.Text = "SCD12";
            this.test7.UseVisualStyleBackColor = false;
            // 
            // TDR_measure_button
            // 
            this.TDR_measure_button.Location = new System.Drawing.Point(130, 239);
            this.TDR_measure_button.Name = "TDR_measure_button";
            this.TDR_measure_button.Size = new System.Drawing.Size(129, 41);
            this.TDR_measure_button.TabIndex = 9;
            this.TDR_measure_button.Text = "TDR Measure";
            this.TDR_measure_button.UseVisualStyleBackColor = true;
            this.TDR_measure_button.Visible = false;
            this.TDR_measure_button.Click += new System.EventHandler(this.TDR_measure_button_Click);
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // case_name
            // 
            this.case_name.AutoSize = true;
            this.case_name.BackColor = System.Drawing.Color.Transparent;
            this.case_name.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.case_name.ForeColor = System.Drawing.Color.White;
            this.case_name.Location = new System.Drawing.Point(652, 325);
            this.case_name.Name = "case_name";
            this.case_name.Size = new System.Drawing.Size(10, 15);
            this.case_name.TabIndex = 33;
            this.case_name.Text = ".";
            this.case_name.Visible = false;
            // 
            // Project_list
            // 
            this.Project_list.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Project_list.GridLines = true;
            this.Project_list.HideSelection = false;
            this.Project_list.Location = new System.Drawing.Point(14, 210);
            this.Project_list.Name = "Project_list";
            this.Project_list.Size = new System.Drawing.Size(360, 228);
            this.Project_list.TabIndex = 31;
            this.Project_list.UseCompatibleStateImageBehavior = false;
            this.Project_list.SelectedIndexChanged += new System.EventHandler(this.Project_list_SelectedIndexChanged);
            // 
            // timer2
            // 
            this.timer2.Tick += new System.EventHandler(this.timer2_Tick);
            // 
            // Project_list_2
            // 
            this.Project_list_2.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Project_list_2.GridLines = true;
            this.Project_list_2.HideSelection = false;
            this.Project_list_2.Location = new System.Drawing.Point(380, 210);
            this.Project_list_2.Name = "Project_list_2";
            this.Project_list_2.Size = new System.Drawing.Size(360, 228);
            this.Project_list_2.TabIndex = 34;
            this.Project_list_2.UseCompatibleStateImageBehavior = false;
            this.Project_list_2.SelectedIndexChanged += new System.EventHandler(this.Project_list_2_SelectedIndexChanged);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(99)))), ((int)(((byte)(153)))), ((int)(((byte)(95)))));
            this.panel2.Controls.Add(this.label4);
            this.panel2.Controls.Add(this.label5);
            this.panel2.Location = new System.Drawing.Point(14, 169);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(100, 35);
            this.panel2.TabIndex = 39;
            this.panel2.Visible = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Font = new System.Drawing.Font("Times New Roman", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(59, 6);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(24, 27);
            this.label4.TabIndex = 25;
            this.label4.Text = "0";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.Font = new System.Drawing.Font("Times New Roman", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(-1, 6);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(67, 27);
            this.label5.TabIndex = 23;
            this.label5.Text = "Pass :";
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(228)))), ((int)(((byte)(0)))), ((int)(((byte)(17)))));
            this.panel3.Controls.Add(this.label6);
            this.panel3.Controls.Add(this.label7);
            this.panel3.Location = new System.Drawing.Point(133, 169);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(100, 35);
            this.panel3.TabIndex = 40;
            this.panel3.Visible = false;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.BackColor = System.Drawing.Color.Transparent;
            this.label6.Font = new System.Drawing.Font("Times New Roman", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.Color.White;
            this.label6.Location = new System.Drawing.Point(54, 6);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(24, 27);
            this.label6.TabIndex = 26;
            this.label6.Text = "0";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.BackColor = System.Drawing.Color.Transparent;
            this.label7.Font = new System.Drawing.Font("Times New Roman", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.ForeColor = System.Drawing.Color.White;
            this.label7.Location = new System.Drawing.Point(-1, 6);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(66, 27);
            this.label7.TabIndex = 24;
            this.label7.Text = "Fail : ";
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(99)))), ((int)(((byte)(153)))), ((int)(((byte)(95)))));
            this.panel4.Controls.Add(this.label_pass_num);
            this.panel4.Controls.Add(this.label2);
            this.panel4.Location = new System.Drawing.Point(380, 169);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(100, 35);
            this.panel4.TabIndex = 41;
            this.panel4.Visible = false;
            // 
            // label_pass_num
            // 
            this.label_pass_num.AutoSize = true;
            this.label_pass_num.BackColor = System.Drawing.Color.Transparent;
            this.label_pass_num.Font = new System.Drawing.Font("Times New Roman", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_pass_num.ForeColor = System.Drawing.Color.White;
            this.label_pass_num.Location = new System.Drawing.Point(59, 6);
            this.label_pass_num.Name = "label_pass_num";
            this.label_pass_num.Size = new System.Drawing.Size(24, 27);
            this.label_pass_num.TabIndex = 25;
            this.label_pass_num.Text = "0";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Times New Roman", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(-1, 6);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 27);
            this.label2.TabIndex = 23;
            this.label2.Text = "Pass :";
            // 
            // panel5
            // 
            this.panel5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(228)))), ((int)(((byte)(0)))), ((int)(((byte)(17)))));
            this.panel5.Controls.Add(this.label_fail_num);
            this.panel5.Controls.Add(this.label3);
            this.panel5.Location = new System.Drawing.Point(499, 169);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(100, 35);
            this.panel5.TabIndex = 42;
            this.panel5.Visible = false;
            // 
            // label_fail_num
            // 
            this.label_fail_num.AutoSize = true;
            this.label_fail_num.BackColor = System.Drawing.Color.Transparent;
            this.label_fail_num.Font = new System.Drawing.Font("Times New Roman", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_fail_num.ForeColor = System.Drawing.Color.White;
            this.label_fail_num.Location = new System.Drawing.Point(54, 6);
            this.label_fail_num.Name = "label_fail_num";
            this.label_fail_num.Size = new System.Drawing.Size(24, 27);
            this.label_fail_num.TabIndex = 26;
            this.label_fail_num.Text = "0";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("Times New Roman", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(-1, 6);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(66, 27);
            this.label3.TabIndex = 24;
            this.label3.Text = "Fail : ";
            // 
            // Run_setting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(5)))), ((int)(((byte)(36)))), ((int)(((byte)(57)))));
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(757, 720);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel5);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.Project_list_2);
            this.Controls.Add(this.case_name);
            this.Controls.Add(this.Project_list);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.TDR_measure_button);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.Name = "Run_setting";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Run";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Run_setting_FormClosing);
            this.Load += new System.EventHandler(this.Run_setting_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button measure_button;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox Dut_name;
        private System.IO.Ports.SerialPort serialPort1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.CheckBox download_files_check;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button TDR_measure_button;
        private System.Windows.Forms.CheckBox test1;
        private System.Windows.Forms.CheckBox test2;
        private System.Windows.Forms.CheckBox test3;
        private System.Windows.Forms.Label case_name;
        private System.Windows.Forms.ListView Project_list;
        private System.Windows.Forms.Timer timer2;
        private System.Windows.Forms.CheckBox test16;
        private System.Windows.Forms.CheckBox test15;
        private System.Windows.Forms.CheckBox test14;
        private System.Windows.Forms.CheckBox test13;
        private System.Windows.Forms.CheckBox test12;
        private System.Windows.Forms.CheckBox test11;
        private System.Windows.Forms.CheckBox test9;
        private System.Windows.Forms.CheckBox test8;
        private System.Windows.Forms.CheckBox test7;
        private System.Windows.Forms.CheckBox test5;
        private System.Windows.Forms.CheckBox test4;
        private System.Windows.Forms.CheckBox Save_plots;
        private System.Windows.Forms.CheckBox test17;
        private System.Windows.Forms.CheckBox test19;
        private System.Windows.Forms.CheckBox test18;
        private System.Windows.Forms.CheckBox fast_mode_cb;
        private System.Windows.Forms.ListView Project_list_2;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Label label_pass_num;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Label label_fail_num;
        private System.Windows.Forms.Label label3;
    }
}