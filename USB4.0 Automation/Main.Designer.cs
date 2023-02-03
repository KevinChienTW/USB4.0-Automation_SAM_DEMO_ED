namespace USB4._0_Automation
{
    partial class Main
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置受控資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
        /// 這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.Run_test_button = new System.Windows.Forms.Button();
            this.open_session_sw = new System.Windows.Forms.Button();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.open_session_e5071c = new System.Windows.Forms.Button();
            this.serialPort1 = new System.IO.Ports.SerialPort(this.components);
            this.DeEmbedd_button = new System.Windows.Forms.Button();
            this.panel4 = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.Fixture_board_button = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.button3 = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.Tets_type_b3 = new System.Windows.Forms.RadioButton();
            this.label10 = new System.Windows.Forms.Label();
            this.switch_control = new System.Windows.Forms.Label();
            this.set_env = new System.Windows.Forms.Button();
            this.env_custom_set = new System.Windows.Forms.Button();
            this.bandwidth_value = new System.Windows.Forms.TextBox();
            this.env_default_value2 = new System.Windows.Forms.RadioButton();
            this.power_value = new System.Windows.Forms.TextBox();
            this.stop_value = new System.Windows.Forms.TextBox();
            this.env_custom_value = new System.Windows.Forms.RadioButton();
            this.point_value = new System.Windows.Forms.TextBox();
            this.env_default_value1 = new System.Windows.Forms.RadioButton();
            this.switch_channel_F = new System.Windows.Forms.RadioButton();
            this.label4 = new System.Windows.Forms.Label();
            this.switch_channel_E = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.switch_channel_D = new System.Windows.Forms.RadioButton();
            this.label7 = new System.Windows.Forms.Label();
            this.switch_channel_C = new System.Windows.Forms.RadioButton();
            this.start_value = new System.Windows.Forms.TextBox();
            this.switch_channel_B = new System.Windows.Forms.RadioButton();
            this.switch_channel_A = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.calibration_button = new System.Windows.Forms.Button();
            this.TDR_Cal_button = new System.Windows.Forms.Button();
            this.USB_tab = new System.Windows.Forms.Button();
            this.main_form_closing = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.label6 = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.Form_Minimized = new System.Windows.Forms.Button();
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.timer3 = new System.Windows.Forms.Timer(this.components);
            this.panel4.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // Run_test_button
            // 
            this.Run_test_button.BackColor = System.Drawing.Color.Transparent;
            this.Run_test_button.BackgroundImage = global::USB4._0_Automation.Properties.Resources.button_yellow_m;
            this.Run_test_button.FlatAppearance.BorderSize = 0;
            this.Run_test_button.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.Run_test_button.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.Run_test_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Run_test_button.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Run_test_button.ForeColor = System.Drawing.Color.White;
            this.Run_test_button.Location = new System.Drawing.Point(571, 517);
            this.Run_test_button.Name = "Run_test_button";
            this.Run_test_button.Size = new System.Drawing.Size(233, 41);
            this.Run_test_button.TabIndex = 9;
            this.Run_test_button.Text = " Run";
            this.Run_test_button.UseVisualStyleBackColor = false;
            this.Run_test_button.Visible = false;
            this.Run_test_button.Click += new System.EventHandler(this.Run_test_button_Click);
            this.Run_test_button.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Run_test_mouseDown);
            this.Run_test_button.MouseLeave += new System.EventHandler(this.Run_test_mouseLeave);
            this.Run_test_button.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Run_test_mouseMove);
            this.Run_test_button.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Run_test_mouseUp);
            // 
            // open_session_sw
            // 
            this.open_session_sw.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(63)))), ((int)(((byte)(114)))), ((int)(((byte)(145)))));
            this.open_session_sw.BackgroundImage = global::USB4._0_Automation.Properties.Resources.switch_Transparent;
            this.open_session_sw.FlatAppearance.BorderSize = 0;
            this.open_session_sw.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.open_session_sw.Font = new System.Drawing.Font("Times New Roman", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.open_session_sw.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(199)))), ((int)(((byte)(215)))), ((int)(((byte)(231)))));
            this.open_session_sw.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.open_session_sw.ImageList = this.imageList1;
            this.open_session_sw.Location = new System.Drawing.Point(9, 74);
            this.open_session_sw.Name = "open_session_sw";
            this.open_session_sw.Size = new System.Drawing.Size(159, 52);
            this.open_session_sw.TabIndex = 10;
            this.open_session_sw.UseVisualStyleBackColor = false;
            this.open_session_sw.Click += new System.EventHandler(this.open_session_sw_Click);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "check_ok_accept_apply_1582.ico");
            this.imageList1.Images.SetKeyName(1, "delete_delete_exit_1577.png");
            // 
            // open_session_e5071c
            // 
            this.open_session_e5071c.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(63)))), ((int)(((byte)(114)))), ((int)(((byte)(145)))));
            this.open_session_e5071c.BackgroundImage = global::USB4._0_Automation.Properties.Resources.E5071C_Transparent;
            this.open_session_e5071c.FlatAppearance.BorderSize = 0;
            this.open_session_e5071c.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.open_session_e5071c.Font = new System.Drawing.Font("Times New Roman", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.open_session_e5071c.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(199)))), ((int)(((byte)(215)))), ((int)(((byte)(231)))));
            this.open_session_e5071c.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.open_session_e5071c.ImageList = this.imageList1;
            this.open_session_e5071c.Location = new System.Drawing.Point(9, 124);
            this.open_session_e5071c.Name = "open_session_e5071c";
            this.open_session_e5071c.Size = new System.Drawing.Size(159, 52);
            this.open_session_e5071c.TabIndex = 11;
            this.open_session_e5071c.UseVisualStyleBackColor = false;
            this.open_session_e5071c.Click += new System.EventHandler(this.open_session_e5071c_Click);
            // 
            // DeEmbedd_button
            // 
            this.DeEmbedd_button.BackColor = System.Drawing.Color.Transparent;
            this.DeEmbedd_button.BackgroundImage = global::USB4._0_Automation.Properties.Resources.button_blue_m;
            this.DeEmbedd_button.FlatAppearance.BorderSize = 0;
            this.DeEmbedd_button.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.DeEmbedd_button.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.DeEmbedd_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.DeEmbedd_button.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DeEmbedd_button.ForeColor = System.Drawing.Color.White;
            this.DeEmbedd_button.Location = new System.Drawing.Point(228, 518);
            this.DeEmbedd_button.Name = "DeEmbedd_button";
            this.DeEmbedd_button.Size = new System.Drawing.Size(233, 41);
            this.DeEmbedd_button.TabIndex = 19;
            this.DeEmbedd_button.Text = "De-embed";
            this.DeEmbedd_button.UseVisualStyleBackColor = false;
            this.DeEmbedd_button.Click += new System.EventHandler(this.DeEmbedd_button_Click);
            this.DeEmbedd_button.MouseDown += new System.Windows.Forms.MouseEventHandler(this.DeEmbedd_button_mouseDown);
            this.DeEmbedd_button.MouseLeave += new System.EventHandler(this.DeEmbedd_button_mouseLeave);
            this.DeEmbedd_button.MouseMove += new System.Windows.Forms.MouseEventHandler(this.DeEmbedd_button_mouseMove);
            this.DeEmbedd_button.MouseUp += new System.Windows.Forms.MouseEventHandler(this.DeEmbedd_button_mouseUp);
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(92)))), ((int)(((byte)(115)))));
            this.panel4.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panel4.BackgroundImage")));
            this.panel4.Controls.Add(this.label5);
            this.panel4.Controls.Add(this.Fixture_board_button);
            this.panel4.Controls.Add(this.open_session_sw);
            this.panel4.Controls.Add(this.open_session_e5071c);
            this.panel4.Location = new System.Drawing.Point(-9, 33);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(175, 609);
            this.panel4.TabIndex = 26;
            this.panel4.Paint += new System.Windows.Forms.PaintEventHandler(this.panel4_Paint);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.Font = new System.Drawing.Font("Times New Roman", 14.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(39, 32);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(101, 22);
            this.label5.TabIndex = 51;
            this.label5.Text = "Connection";
            // 
            // Fixture_board_button
            // 
            this.Fixture_board_button.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(63)))), ((int)(((byte)(114)))), ((int)(((byte)(145)))));
            this.Fixture_board_button.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("Fixture_board_button.BackgroundImage")));
            this.Fixture_board_button.FlatAppearance.BorderSize = 0;
            this.Fixture_board_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Fixture_board_button.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(199)))), ((int)(((byte)(215)))), ((int)(((byte)(231)))));
            this.Fixture_board_button.Location = new System.Drawing.Point(9, 175);
            this.Fixture_board_button.Name = "Fixture_board_button";
            this.Fixture_board_button.Size = new System.Drawing.Size(159, 52);
            this.Fixture_board_button.TabIndex = 49;
            this.Fixture_board_button.UseVisualStyleBackColor = false;
            this.Fixture_board_button.Click += new System.EventHandler(this.Fixture_board_button_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.Color.Transparent;
            this.groupBox2.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("groupBox2.BackgroundImage")));
            this.groupBox2.Controls.Add(this.button3);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.groupBox4);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.switch_control);
            this.groupBox2.Controls.Add(this.set_env);
            this.groupBox2.Controls.Add(this.env_custom_set);
            this.groupBox2.Controls.Add(this.bandwidth_value);
            this.groupBox2.Controls.Add(this.env_default_value2);
            this.groupBox2.Controls.Add(this.power_value);
            this.groupBox2.Controls.Add(this.stop_value);
            this.groupBox2.Controls.Add(this.env_custom_value);
            this.groupBox2.Controls.Add(this.point_value);
            this.groupBox2.Controls.Add(this.env_default_value1);
            this.groupBox2.Controls.Add(this.switch_channel_F);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.switch_channel_E);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.switch_channel_D);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.switch_channel_C);
            this.groupBox2.Controls.Add(this.start_value);
            this.groupBox2.Controls.Add(this.switch_channel_B);
            this.groupBox2.Controls.Add(this.switch_channel_A);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.calibration_button);
            this.groupBox2.ForeColor = System.Drawing.Color.Transparent;
            this.groupBox2.Location = new System.Drawing.Point(201, 118);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(649, 386);
            this.groupBox2.TabIndex = 27;
            this.groupBox2.TabStop = false;
            this.groupBox2.Enter += new System.EventHandler(this.groupBox2_Enter);
            // 
            // button3
            // 
            this.button3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(199)))), ((int)(((byte)(215)))), ((int)(((byte)(231)))));
            this.button3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button3.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button3.ForeColor = System.Drawing.Color.Black;
            this.button3.Location = new System.Drawing.Point(498, 344);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 61;
            this.button3.Text = "LIMIT Set";
            this.button3.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.button3.UseVisualStyleBackColor = false;
            this.button3.Visible = false;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label3.Font = new System.Drawing.Font("Times New Roman", 12F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(393, 178);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(46, 19);
            this.label3.TabIndex = 60;
            this.label3.Text = "Point";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.Tets_type_b3);
            this.groupBox4.Location = new System.Drawing.Point(151, 14);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(79, 27);
            this.groupBox4.TabIndex = 59;
            this.groupBox4.TabStop = false;
            // 
            // Tets_type_b3
            // 
            this.Tets_type_b3.AutoSize = true;
            this.Tets_type_b3.Location = new System.Drawing.Point(5, 9);
            this.Tets_type_b3.Name = "Tets_type_b3";
            this.Tets_type_b3.Size = new System.Drawing.Size(37, 16);
            this.Tets_type_b3.TabIndex = 55;
            this.Tets_type_b3.TabStop = true;
            this.Tets_type_b3.Text = "B3";
            this.Tets_type_b3.UseVisualStyleBackColor = true;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(24, 21);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(68, 19);
            this.label10.TabIndex = 52;
            this.label10.Text = "Test Type";
            // 
            // switch_control
            // 
            this.switch_control.AutoSize = true;
            this.switch_control.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.switch_control.Location = new System.Drawing.Point(24, 58);
            this.switch_control.Name = "switch_control";
            this.switch_control.Size = new System.Drawing.Size(100, 19);
            this.switch_control.TabIndex = 50;
            this.switch_control.Text = "Switch Control";
            this.switch_control.Visible = false;
            this.switch_control.Click += new System.EventHandler(this.switch_control_Click);
            // 
            // set_env
            // 
            this.set_env.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(199)))), ((int)(((byte)(215)))), ((int)(((byte)(231)))));
            this.set_env.FlatAppearance.BorderSize = 0;
            this.set_env.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.set_env.Font = new System.Drawing.Font("Times New Roman", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.set_env.ForeColor = System.Drawing.Color.Black;
            this.set_env.Location = new System.Drawing.Point(520, 266);
            this.set_env.Name = "set_env";
            this.set_env.Size = new System.Drawing.Size(48, 28);
            this.set_env.TabIndex = 28;
            this.set_env.Text = "Save";
            this.set_env.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.set_env.UseVisualStyleBackColor = false;
            this.set_env.Visible = false;
            this.set_env.Click += new System.EventHandler(this.set_env_Click);
            // 
            // env_custom_set
            // 
            this.env_custom_set.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(199)))), ((int)(((byte)(215)))), ((int)(((byte)(231)))));
            this.env_custom_set.FlatAppearance.BorderSize = 0;
            this.env_custom_set.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.env_custom_set.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.env_custom_set.ForeColor = System.Drawing.Color.Black;
            this.env_custom_set.Location = new System.Drawing.Point(192, 136);
            this.env_custom_set.Name = "env_custom_set";
            this.env_custom_set.Size = new System.Drawing.Size(46, 23);
            this.env_custom_set.TabIndex = 49;
            this.env_custom_set.Text = "set";
            this.env_custom_set.UseVisualStyleBackColor = false;
            this.env_custom_set.Click += new System.EventHandler(this.env_custom_set_Click);
            // 
            // bandwidth_value
            // 
            this.bandwidth_value.Font = new System.Drawing.Font("Times New Roman", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bandwidth_value.Location = new System.Drawing.Point(23, 259);
            this.bandwidth_value.Multiline = true;
            this.bandwidth_value.Name = "bandwidth_value";
            this.bandwidth_value.Size = new System.Drawing.Size(176, 28);
            this.bandwidth_value.TabIndex = 23;
            // 
            // env_default_value2
            // 
            this.env_default_value2.AutoSize = true;
            this.env_default_value2.BackColor = System.Drawing.Color.Transparent;
            this.env_default_value2.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.env_default_value2.ForeColor = System.Drawing.Color.White;
            this.env_default_value2.Location = new System.Drawing.Point(28, 120);
            this.env_default_value2.Name = "env_default_value2";
            this.env_default_value2.Size = new System.Drawing.Size(152, 19);
            this.env_default_value2.TabIndex = 47;
            this.env_default_value2.TabStop = true;
            this.env_default_value2.Text = "10M, 20G, 2000, 1K, -5";
            this.env_default_value2.UseVisualStyleBackColor = false;
            this.env_default_value2.Click += new System.EventHandler(this.env_default_value2_Click);
            // 
            // power_value
            // 
            this.power_value.Font = new System.Drawing.Font("Times New Roman", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.power_value.Location = new System.Drawing.Point(212, 259);
            this.power_value.Multiline = true;
            this.power_value.Name = "power_value";
            this.power_value.Size = new System.Drawing.Size(176, 28);
            this.power_value.TabIndex = 29;
            // 
            // stop_value
            // 
            this.stop_value.Font = new System.Drawing.Font("Times New Roman", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.stop_value.Location = new System.Drawing.Point(212, 198);
            this.stop_value.Multiline = true;
            this.stop_value.Name = "stop_value";
            this.stop_value.Size = new System.Drawing.Size(176, 28);
            this.stop_value.TabIndex = 21;
            // 
            // env_custom_value
            // 
            this.env_custom_value.AutoSize = true;
            this.env_custom_value.BackColor = System.Drawing.Color.Transparent;
            this.env_custom_value.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.env_custom_value.ForeColor = System.Drawing.Color.White;
            this.env_custom_value.Location = new System.Drawing.Point(28, 150);
            this.env_custom_value.Name = "env_custom_value";
            this.env_custom_value.Size = new System.Drawing.Size(14, 13);
            this.env_custom_value.TabIndex = 48;
            this.env_custom_value.TabStop = true;
            this.env_custom_value.UseVisualStyleBackColor = false;
            this.env_custom_value.Click += new System.EventHandler(this.env_custom_value_Click);
            // 
            // point_value
            // 
            this.point_value.Font = new System.Drawing.Font("Times New Roman", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.point_value.Location = new System.Drawing.Point(397, 196);
            this.point_value.Multiline = true;
            this.point_value.Name = "point_value";
            this.point_value.Size = new System.Drawing.Size(176, 28);
            this.point_value.TabIndex = 22;
            // 
            // env_default_value1
            // 
            this.env_default_value1.AutoSize = true;
            this.env_default_value1.BackColor = System.Drawing.Color.Transparent;
            this.env_default_value1.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.env_default_value1.ForeColor = System.Drawing.Color.White;
            this.env_default_value1.Location = new System.Drawing.Point(28, 90);
            this.env_default_value1.Name = "env_default_value1";
            this.env_default_value1.Size = new System.Drawing.Size(159, 19);
            this.env_default_value1.TabIndex = 46;
            this.env_default_value1.TabStop = true;
            this.env_default_value1.Text = "10M, 20G, 2000, 20K, -5";
            this.env_default_value1.UseVisualStyleBackColor = false;
            this.env_default_value1.Click += new System.EventHandler(this.env_default_value1_Click);
            // 
            // switch_channel_F
            // 
            this.switch_channel_F.AutoSize = true;
            this.switch_channel_F.BackColor = System.Drawing.Color.Transparent;
            this.switch_channel_F.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.switch_channel_F.ForeColor = System.Drawing.Color.White;
            this.switch_channel_F.Location = new System.Drawing.Point(419, 58);
            this.switch_channel_F.Name = "switch_channel_F";
            this.switch_channel_F.Size = new System.Drawing.Size(32, 19);
            this.switch_channel_F.TabIndex = 44;
            this.switch_channel_F.TabStop = true;
            this.switch_channel_F.Text = "F";
            this.switch_channel_F.UseVisualStyleBackColor = false;
            this.switch_channel_F.Visible = false;
            this.switch_channel_F.CheckedChanged += new System.EventHandler(this.switch_channel_F_CheckedChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label4.Font = new System.Drawing.Font("Times New Roman", 12F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(26, 239);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(83, 19);
            this.label4.TabIndex = 27;
            this.label4.Text = "Bandwidth";
            // 
            // switch_channel_E
            // 
            this.switch_channel_E.AutoSize = true;
            this.switch_channel_E.BackColor = System.Drawing.Color.Transparent;
            this.switch_channel_E.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.switch_channel_E.ForeColor = System.Drawing.Color.White;
            this.switch_channel_E.Location = new System.Drawing.Point(366, 58);
            this.switch_channel_E.Name = "switch_channel_E";
            this.switch_channel_E.Size = new System.Drawing.Size(32, 19);
            this.switch_channel_E.TabIndex = 43;
            this.switch_channel_E.TabStop = true;
            this.switch_channel_E.Text = "E";
            this.switch_channel_E.UseVisualStyleBackColor = false;
            this.switch_channel_E.Visible = false;
            this.switch_channel_E.CheckedChanged += new System.EventHandler(this.switch_channel_E_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label2.Font = new System.Drawing.Font("Times New Roman", 12F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(209, 178);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(39, 19);
            this.label2.TabIndex = 25;
            this.label2.Text = "Stop";
            // 
            // switch_channel_D
            // 
            this.switch_channel_D.AutoSize = true;
            this.switch_channel_D.BackColor = System.Drawing.Color.Transparent;
            this.switch_channel_D.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.switch_channel_D.ForeColor = System.Drawing.Color.White;
            this.switch_channel_D.Location = new System.Drawing.Point(312, 58);
            this.switch_channel_D.Name = "switch_channel_D";
            this.switch_channel_D.Size = new System.Drawing.Size(34, 19);
            this.switch_channel_D.TabIndex = 42;
            this.switch_channel_D.TabStop = true;
            this.switch_channel_D.Text = "D";
            this.switch_channel_D.UseVisualStyleBackColor = false;
            this.switch_channel_D.Visible = false;
            this.switch_channel_D.CheckedChanged += new System.EventHandler(this.switch_channel_D_CheckedChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.BackColor = System.Drawing.Color.Transparent;
            this.label7.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label7.Font = new System.Drawing.Font("Times New Roman", 12F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.ForeColor = System.Drawing.Color.White;
            this.label7.Location = new System.Drawing.Point(209, 239);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(51, 19);
            this.label7.TabIndex = 30;
            this.label7.Text = "Power";
            // 
            // switch_channel_C
            // 
            this.switch_channel_C.AutoSize = true;
            this.switch_channel_C.BackColor = System.Drawing.Color.Transparent;
            this.switch_channel_C.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.switch_channel_C.ForeColor = System.Drawing.Color.White;
            this.switch_channel_C.Location = new System.Drawing.Point(258, 58);
            this.switch_channel_C.Name = "switch_channel_C";
            this.switch_channel_C.Size = new System.Drawing.Size(33, 19);
            this.switch_channel_C.TabIndex = 41;
            this.switch_channel_C.TabStop = true;
            this.switch_channel_C.Text = "C";
            this.switch_channel_C.UseVisualStyleBackColor = false;
            this.switch_channel_C.Visible = false;
            this.switch_channel_C.CheckedChanged += new System.EventHandler(this.switch_channel_C_CheckedChanged);
            // 
            // start_value
            // 
            this.start_value.BackColor = System.Drawing.SystemColors.Window;
            this.start_value.Font = new System.Drawing.Font("Times New Roman", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.start_value.Location = new System.Drawing.Point(24, 197);
            this.start_value.Multiline = true;
            this.start_value.Name = "start_value";
            this.start_value.Size = new System.Drawing.Size(176, 28);
            this.start_value.TabIndex = 20;
            // 
            // switch_channel_B
            // 
            this.switch_channel_B.AutoSize = true;
            this.switch_channel_B.BackColor = System.Drawing.Color.Transparent;
            this.switch_channel_B.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.switch_channel_B.ForeColor = System.Drawing.Color.White;
            this.switch_channel_B.Location = new System.Drawing.Point(205, 58);
            this.switch_channel_B.Name = "switch_channel_B";
            this.switch_channel_B.Size = new System.Drawing.Size(33, 19);
            this.switch_channel_B.TabIndex = 40;
            this.switch_channel_B.TabStop = true;
            this.switch_channel_B.Text = "B";
            this.switch_channel_B.UseVisualStyleBackColor = false;
            this.switch_channel_B.Visible = false;
            this.switch_channel_B.CheckedChanged += new System.EventHandler(this.switch_channel_B_CheckedChanged);
            // 
            // switch_channel_A
            // 
            this.switch_channel_A.AutoSize = true;
            this.switch_channel_A.BackColor = System.Drawing.Color.Transparent;
            this.switch_channel_A.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.switch_channel_A.ForeColor = System.Drawing.Color.White;
            this.switch_channel_A.Location = new System.Drawing.Point(152, 58);
            this.switch_channel_A.Name = "switch_channel_A";
            this.switch_channel_A.Size = new System.Drawing.Size(35, 19);
            this.switch_channel_A.TabIndex = 39;
            this.switch_channel_A.TabStop = true;
            this.switch_channel_A.Text = "A";
            this.switch_channel_A.UseVisualStyleBackColor = false;
            this.switch_channel_A.Visible = false;
            this.switch_channel_A.CheckedChanged += new System.EventHandler(this.switch_channel_A_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label1.Font = new System.Drawing.Font("Times New Roman", 12F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(26, 178);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(42, 19);
            this.label1.TabIndex = 24;
            this.label1.Text = "Start";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.BackColor = System.Drawing.Color.Transparent;
            this.label8.Font = new System.Drawing.Font("Times New Roman", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.ForeColor = System.Drawing.Color.White;
            this.label8.Location = new System.Drawing.Point(568, 24);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(68, 24);
            this.label8.TabIndex = 31;
            this.label8.Text = "USB 4";
            // 
            // calibration_button
            // 
            this.calibration_button.BackColor = System.Drawing.Color.Transparent;
            this.calibration_button.BackgroundImage = global::USB4._0_Automation.Properties.Resources.button_blue_s;
            this.calibration_button.FlatAppearance.BorderSize = 0;
            this.calibration_button.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.calibration_button.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.calibration_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.calibration_button.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.calibration_button.ForeColor = System.Drawing.Color.White;
            this.calibration_button.Location = new System.Drawing.Point(225, 333);
            this.calibration_button.Name = "calibration_button";
            this.calibration_button.Size = new System.Drawing.Size(175, 41);
            this.calibration_button.TabIndex = 0;
            this.calibration_button.Text = "Calibration";
            this.calibration_button.UseVisualStyleBackColor = false;
            this.calibration_button.Visible = false;
            this.calibration_button.Click += new System.EventHandler(this.calibration_button_Click);
            this.calibration_button.MouseDown += new System.Windows.Forms.MouseEventHandler(this.calibration_mouseDown);
            this.calibration_button.MouseLeave += new System.EventHandler(this.calibration_mouseLeave);
            this.calibration_button.MouseMove += new System.Windows.Forms.MouseEventHandler(this.calibration_mouseMove);
            this.calibration_button.MouseUp += new System.Windows.Forms.MouseEventHandler(this.calibration_mouseUp);
            // 
            // TDR_Cal_button
            // 
            this.TDR_Cal_button.ForeColor = System.Drawing.Color.Black;
            this.TDR_Cal_button.Location = new System.Drawing.Point(326, 518);
            this.TDR_Cal_button.Name = "TDR_Cal_button";
            this.TDR_Cal_button.Size = new System.Drawing.Size(127, 37);
            this.TDR_Cal_button.TabIndex = 51;
            this.TDR_Cal_button.Text = "TDR Cal";
            this.TDR_Cal_button.UseVisualStyleBackColor = true;
            this.TDR_Cal_button.Visible = false;
            this.TDR_Cal_button.Click += new System.EventHandler(this.TDR_Cal_button_Click);
            // 
            // USB_tab
            // 
            this.USB_tab.BackColor = System.Drawing.Color.Transparent;
            this.USB_tab.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("USB_tab.BackgroundImage")));
            this.USB_tab.FlatAppearance.BorderSize = 0;
            this.USB_tab.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.USB_tab.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.USB_tab.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.USB_tab.Font = new System.Drawing.Font("Times New Roman", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.USB_tab.ForeColor = System.Drawing.Color.White;
            this.USB_tab.Location = new System.Drawing.Point(200, 79);
            this.USB_tab.Name = "USB_tab";
            this.USB_tab.Size = new System.Drawing.Size(170, 47);
            this.USB_tab.TabIndex = 28;
            this.USB_tab.UseVisualStyleBackColor = false;
            this.USB_tab.Click += new System.EventHandler(this.USB_tab_Click);
            // 
            // main_form_closing
            // 
            this.main_form_closing.BackColor = System.Drawing.Color.Transparent;
            this.main_form_closing.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("main_form_closing.BackgroundImage")));
            this.main_form_closing.FlatAppearance.BorderSize = 0;
            this.main_form_closing.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.main_form_closing.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.main_form_closing.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.main_form_closing.Location = new System.Drawing.Point(851, 3);
            this.main_form_closing.Name = "main_form_closing";
            this.main_form_closing.Size = new System.Drawing.Size(24, 24);
            this.main_form_closing.TabIndex = 31;
            this.main_form_closing.UseVisualStyleBackColor = false;
            this.main_form_closing.Click += new System.EventHandler(this.main_form_closing_Click_1);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictureBox1.BackgroundImage")));
            this.pictureBox1.Location = new System.Drawing.Point(769, 55);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(94, 41);
            this.pictureBox1.TabIndex = 32;
            this.pictureBox1.TabStop = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox2.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictureBox2.BackgroundImage")));
            this.pictureBox2.Location = new System.Drawing.Point(426, 589);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(185, 18);
            this.pictureBox2.TabIndex = 51;
            this.pictureBox2.TabStop = false;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.BackColor = System.Drawing.Color.Transparent;
            this.label6.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.Color.White;
            this.label6.Location = new System.Drawing.Point(9, 5);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(198, 19);
            this.label6.TabIndex = 52;
            this.label6.Text = "Allion CAMS System (V1.0.3)";
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 200;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // Form_Minimized
            // 
            this.Form_Minimized.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.Form_Minimized.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("Form_Minimized.BackgroundImage")));
            this.Form_Minimized.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.Form_Minimized.Location = new System.Drawing.Point(819, 3);
            this.Form_Minimized.Name = "Form_Minimized";
            this.Form_Minimized.Size = new System.Drawing.Size(24, 24);
            this.Form_Minimized.TabIndex = 55;
            this.Form_Minimized.UseVisualStyleBackColor = false;
            this.Form_Minimized.Click += new System.EventHandler(this.Form_Minimized_Click);
            // 
            // timer2
            // 
            this.timer2.Enabled = true;
            this.timer2.Tick += new System.EventHandler(this.timer2_Tick);
            // 
            // timer3
            // 
            this.timer3.Enabled = true;
            this.timer3.Tick += new System.EventHandler(this.timer3_Tick);
            // 
            // Main
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(5)))), ((int)(((byte)(36)))), ((int)(((byte)(57)))));
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.ClientSize = new System.Drawing.Size(876, 607);
            this.Controls.Add(this.USB_tab);
            this.Controls.Add(this.Form_Minimized);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.main_form_closing);
            this.Controls.Add(this.DeEmbedd_button);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.TDR_Cal_button);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.Run_test_button);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.Main_Load);
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button Run_test_button;
        private System.Windows.Forms.Button open_session_sw;
        private System.Windows.Forms.Button open_session_e5071c;
        private System.IO.Ports.SerialPort serialPort1;
        private System.Windows.Forms.Button DeEmbedd_button;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox power_value;
        private System.Windows.Forms.TextBox start_value;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox bandwidth_value;
        private System.Windows.Forms.TextBox point_value;
        private System.Windows.Forms.TextBox stop_value;
        private System.Windows.Forms.Button set_env;
        private System.Windows.Forms.Button USB_tab;
        private System.Windows.Forms.RadioButton switch_channel_F;
        private System.Windows.Forms.RadioButton switch_channel_E;
        private System.Windows.Forms.RadioButton switch_channel_D;
        private System.Windows.Forms.RadioButton switch_channel_C;
        private System.Windows.Forms.RadioButton switch_channel_B;
        private System.Windows.Forms.RadioButton switch_channel_A;
        private System.Windows.Forms.Button main_form_closing;
        private System.Windows.Forms.RadioButton env_default_value2;
        private System.Windows.Forms.RadioButton env_default_value1;
        private System.Windows.Forms.Button Fixture_board_button;
        private System.Windows.Forms.Button env_custom_set;
        private System.Windows.Forms.RadioButton env_custom_value;
        private System.Windows.Forms.Button calibration_button;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Label switch_control;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button Form_Minimized;
        private System.Windows.Forms.Button TDR_Cal_button;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Timer timer2;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.RadioButton Tets_type_b3;
        private System.Windows.Forms.Timer timer3;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button3;
    }
}

