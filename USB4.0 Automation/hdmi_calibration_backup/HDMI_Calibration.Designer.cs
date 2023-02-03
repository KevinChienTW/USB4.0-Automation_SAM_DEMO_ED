namespace USB4._0_Automation
{
    partial class HDMI_Calibration_1_4
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HDMI_Calibration_1_4));
            this.ECAL_button = new System.Windows.Forms.Button();
            this.Near_D0_button = new System.Windows.Forms.Button();
            this.Near_D1_button = new System.Windows.Forms.Button();
            this.Near_D2_button = new System.Windows.Forms.Button();
            this.Near_D3_button = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.HEAC_button = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.Far_D3_button = new System.Windows.Forms.Button();
            this.Far_D2_button = new System.Windows.Forms.Button();
            this.Far_D1_button = new System.Windows.Forms.Button();
            this.Far_D0_button = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.OPEN_button = new System.Windows.Forms.Button();
            this.serialPort1 = new System.IO.Ports.SerialPort(this.components);
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // ECAL_button
            // 
            this.ECAL_button.BackColor = System.Drawing.Color.Transparent;
            this.ECAL_button.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("ECAL_button.BackgroundImage")));
            this.ECAL_button.FlatAppearance.BorderSize = 0;
            this.ECAL_button.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.ECAL_button.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.ECAL_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ECAL_button.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ECAL_button.Location = new System.Drawing.Point(59, 327);
            this.ECAL_button.Name = "ECAL_button";
            this.ECAL_button.Size = new System.Drawing.Size(143, 35);
            this.ECAL_button.TabIndex = 1;
            this.ECAL_button.Text = "Step1. ECAL";
            this.ECAL_button.UseVisualStyleBackColor = false;
            this.ECAL_button.Click += new System.EventHandler(this.ECAL_button_Click);
            this.ECAL_button.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ECAL_button_MouseDown);
            this.ECAL_button.MouseLeave += new System.EventHandler(this.ECAL_button_MouseLeave);
            this.ECAL_button.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ECAL_button_MouseMove);
            this.ECAL_button.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ECAL_button_MouseUp);
            // 
            // Near_D0_button
            // 
            this.Near_D0_button.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.Near_D0_button.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(68)))), ((int)(((byte)(109)))));
            this.Near_D0_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Near_D0_button.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Near_D0_button.ForeColor = System.Drawing.Color.White;
            this.Near_D0_button.Location = new System.Drawing.Point(18, 35);
            this.Near_D0_button.Margin = new System.Windows.Forms.Padding(2);
            this.Near_D0_button.Name = "Near_D0_button";
            this.Near_D0_button.Size = new System.Drawing.Size(70, 30);
            this.Near_D0_button.TabIndex = 4;
            this.Near_D0_button.Text = "D0+, D0-";
            this.Near_D0_button.UseVisualStyleBackColor = true;
            this.Near_D0_button.Click += new System.EventHandler(this.Near_D0_button_Click);
            // 
            // Near_D1_button
            // 
            this.Near_D1_button.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.Near_D1_button.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(68)))), ((int)(((byte)(109)))));
            this.Near_D1_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Near_D1_button.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Near_D1_button.ForeColor = System.Drawing.Color.White;
            this.Near_D1_button.Location = new System.Drawing.Point(18, 100);
            this.Near_D1_button.Margin = new System.Windows.Forms.Padding(2);
            this.Near_D1_button.Name = "Near_D1_button";
            this.Near_D1_button.Size = new System.Drawing.Size(70, 30);
            this.Near_D1_button.TabIndex = 5;
            this.Near_D1_button.Text = "D1+, D1-";
            this.Near_D1_button.UseVisualStyleBackColor = true;
            this.Near_D1_button.Click += new System.EventHandler(this.Near_D1_button_Click);
            // 
            // Near_D2_button
            // 
            this.Near_D2_button.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.Near_D2_button.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(68)))), ((int)(((byte)(109)))));
            this.Near_D2_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Near_D2_button.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Near_D2_button.ForeColor = System.Drawing.Color.White;
            this.Near_D2_button.Location = new System.Drawing.Point(18, 165);
            this.Near_D2_button.Margin = new System.Windows.Forms.Padding(2);
            this.Near_D2_button.Name = "Near_D2_button";
            this.Near_D2_button.Size = new System.Drawing.Size(70, 30);
            this.Near_D2_button.TabIndex = 6;
            this.Near_D2_button.Text = "D2+, D2-";
            this.Near_D2_button.UseVisualStyleBackColor = true;
            this.Near_D2_button.Click += new System.EventHandler(this.Near_D2_button_Click);
            // 
            // Near_D3_button
            // 
            this.Near_D3_button.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.Near_D3_button.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(68)))), ((int)(((byte)(109)))));
            this.Near_D3_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Near_D3_button.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Near_D3_button.ForeColor = System.Drawing.Color.White;
            this.Near_D3_button.Location = new System.Drawing.Point(18, 230);
            this.Near_D3_button.Margin = new System.Windows.Forms.Padding(2);
            this.Near_D3_button.Name = "Near_D3_button";
            this.Near_D3_button.Size = new System.Drawing.Size(70, 30);
            this.Near_D3_button.TabIndex = 7;
            this.Near_D3_button.Text = "D3+, D3-";
            this.Near_D3_button.UseVisualStyleBackColor = true;
            this.Near_D3_button.Click += new System.EventHandler(this.Near_D3_button_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.Transparent;
            this.groupBox1.Controls.Add(this.Near_D3_button);
            this.groupBox1.Controls.Add(this.Near_D2_button);
            this.groupBox1.Controls.Add(this.Near_D1_button);
            this.groupBox1.Controls.Add(this.Near_D0_button);
            this.groupBox1.Font = new System.Drawing.Font("Times New Roman", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.ForeColor = System.Drawing.Color.White;
            this.groupBox1.Location = new System.Drawing.Point(14, 21);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(103, 285);
            this.groupBox1.TabIndex = 12;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Near";
            // 
            // HEAC_button
            // 
            this.HEAC_button.BackColor = System.Drawing.Color.Transparent;
            this.HEAC_button.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("HEAC_button.BackgroundImage")));
            this.HEAC_button.FlatAppearance.BorderSize = 0;
            this.HEAC_button.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.HEAC_button.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.HEAC_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.HEAC_button.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HEAC_button.Location = new System.Drawing.Point(59, 476);
            this.HEAC_button.Margin = new System.Windows.Forms.Padding(2);
            this.HEAC_button.Name = "HEAC_button";
            this.HEAC_button.Size = new System.Drawing.Size(143, 35);
            this.HEAC_button.TabIndex = 14;
            this.HEAC_button.Text = "Step3. HEAC";
            this.HEAC_button.UseVisualStyleBackColor = false;
            this.HEAC_button.Click += new System.EventHandler(this.HEAC_button_Click);
            this.HEAC_button.MouseDown += new System.Windows.Forms.MouseEventHandler(this.HEAC_button_MouseDown);
            this.HEAC_button.MouseLeave += new System.EventHandler(this.HEAC_button_MouseLeave);
            this.HEAC_button.MouseMove += new System.Windows.Forms.MouseEventHandler(this.HEAC_button_MouseMove);
            this.HEAC_button.MouseUp += new System.Windows.Forms.MouseEventHandler(this.HEAC_button_MouseUp);
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.Color.Transparent;
            this.groupBox2.Controls.Add(this.Far_D3_button);
            this.groupBox2.Controls.Add(this.Far_D2_button);
            this.groupBox2.Controls.Add(this.Far_D1_button);
            this.groupBox2.Controls.Add(this.Far_D0_button);
            this.groupBox2.Font = new System.Drawing.Font("Times New Roman", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.ForeColor = System.Drawing.Color.White;
            this.groupBox2.Location = new System.Drawing.Point(144, 21);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(106, 285);
            this.groupBox2.TabIndex = 13;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Far";
            this.groupBox2.Enter += new System.EventHandler(this.groupBox2_Enter);
            // 
            // Far_D3_button
            // 
            this.Far_D3_button.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.Far_D3_button.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(162)))), ((int)(((byte)(232)))));
            this.Far_D3_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Far_D3_button.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Far_D3_button.ForeColor = System.Drawing.Color.White;
            this.Far_D3_button.Location = new System.Drawing.Point(18, 230);
            this.Far_D3_button.Margin = new System.Windows.Forms.Padding(2);
            this.Far_D3_button.Name = "Far_D3_button";
            this.Far_D3_button.Size = new System.Drawing.Size(70, 30);
            this.Far_D3_button.TabIndex = 7;
            this.Far_D3_button.Text = "D3+, D3-";
            this.Far_D3_button.UseVisualStyleBackColor = true;
            this.Far_D3_button.Click += new System.EventHandler(this.Far_D3_button_Click);
            // 
            // Far_D2_button
            // 
            this.Far_D2_button.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.Far_D2_button.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(162)))), ((int)(((byte)(232)))));
            this.Far_D2_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Far_D2_button.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Far_D2_button.ForeColor = System.Drawing.Color.White;
            this.Far_D2_button.Location = new System.Drawing.Point(18, 165);
            this.Far_D2_button.Margin = new System.Windows.Forms.Padding(2);
            this.Far_D2_button.Name = "Far_D2_button";
            this.Far_D2_button.Size = new System.Drawing.Size(70, 30);
            this.Far_D2_button.TabIndex = 6;
            this.Far_D2_button.Text = "D2+, D2-";
            this.Far_D2_button.UseVisualStyleBackColor = true;
            this.Far_D2_button.Click += new System.EventHandler(this.Far_D2_button_Click);
            // 
            // Far_D1_button
            // 
            this.Far_D1_button.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.Far_D1_button.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(162)))), ((int)(((byte)(232)))));
            this.Far_D1_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Far_D1_button.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Far_D1_button.ForeColor = System.Drawing.Color.White;
            this.Far_D1_button.Location = new System.Drawing.Point(18, 100);
            this.Far_D1_button.Margin = new System.Windows.Forms.Padding(2);
            this.Far_D1_button.Name = "Far_D1_button";
            this.Far_D1_button.Size = new System.Drawing.Size(70, 30);
            this.Far_D1_button.TabIndex = 5;
            this.Far_D1_button.Text = "D1+, D1-";
            this.Far_D1_button.UseVisualStyleBackColor = true;
            this.Far_D1_button.Click += new System.EventHandler(this.Far_D1_button_Click);
            // 
            // Far_D0_button
            // 
            this.Far_D0_button.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.Far_D0_button.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(162)))), ((int)(((byte)(232)))));
            this.Far_D0_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Far_D0_button.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Far_D0_button.ForeColor = System.Drawing.Color.White;
            this.Far_D0_button.Location = new System.Drawing.Point(18, 35);
            this.Far_D0_button.Margin = new System.Windows.Forms.Padding(2);
            this.Far_D0_button.Name = "Far_D0_button";
            this.Far_D0_button.Size = new System.Drawing.Size(70, 30);
            this.Far_D0_button.TabIndex = 4;
            this.Far_D0_button.Text = "D0+, D0-";
            this.Far_D0_button.UseVisualStyleBackColor = true;
            this.Far_D0_button.Click += new System.EventHandler(this.Far_D0_button_Click);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 300;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // OPEN_button
            // 
            this.OPEN_button.BackColor = System.Drawing.Color.Transparent;
            this.OPEN_button.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("OPEN_button.BackgroundImage")));
            this.OPEN_button.FlatAppearance.BorderSize = 0;
            this.OPEN_button.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.OPEN_button.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.OPEN_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.OPEN_button.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OPEN_button.Location = new System.Drawing.Point(59, 379);
            this.OPEN_button.Name = "OPEN_button";
            this.OPEN_button.Size = new System.Drawing.Size(143, 35);
            this.OPEN_button.TabIndex = 15;
            this.OPEN_button.Text = "Step2. Open";
            this.OPEN_button.UseVisualStyleBackColor = false;
            this.OPEN_button.Click += new System.EventHandler(this.OPEN_button_Click);
            this.OPEN_button.MouseDown += new System.Windows.Forms.MouseEventHandler(this.UNECAL_button_MouseDown);
            this.OPEN_button.MouseLeave += new System.EventHandler(this.UNECAL_button_MouseLeave);
            this.OPEN_button.MouseMove += new System.Windows.Forms.MouseEventHandler(this.UNECAL_button_MouseMove);
            this.OPEN_button.MouseUp += new System.Windows.Forms.MouseEventHandler(this.UNECAL_button_MouseUp);
            // 
            // HDMI_Calibration
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.ClientSize = new System.Drawing.Size(262, 522);
            this.Controls.Add(this.OPEN_button);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.HEAC_button);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.ECAL_button);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "HDMI_Calibration";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Calibration";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.HDMI_Calibration_FormClosing);
            this.Load += new System.EventHandler(this.HDMI_Calibration_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button ECAL_button;
        private System.Windows.Forms.Button Near_D0_button;
        private System.Windows.Forms.Button Near_D1_button;
        private System.Windows.Forms.Button Near_D2_button;
        private System.Windows.Forms.Button Near_D3_button;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button Far_D3_button;
        private System.Windows.Forms.Button Far_D2_button;
        private System.Windows.Forms.Button Far_D1_button;
        private System.Windows.Forms.Button Far_D0_button;
        private System.Windows.Forms.Button HEAC_button;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button OPEN_button;
        private System.IO.Ports.SerialPort serialPort1;
    }
}