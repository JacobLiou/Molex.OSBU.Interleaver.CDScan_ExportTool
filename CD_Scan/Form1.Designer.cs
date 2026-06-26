namespace CD_Scan
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.CDScan = new System.Windows.Forms.Button();
            this.label_status = new System.Windows.Forms.Label();
            this.label_CD = new System.Windows.Forms.Label();
            this.outputresult = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox_IP = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.radioODD = new System.Windows.Forms.RadioButton();
            this.radioEVEN = new System.Windows.Forms.RadioButton();
            this.radioMux = new System.Windows.Forms.RadioButton();
            this.radioDemux = new System.Windows.Forms.RadioButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label_log = new System.Windows.Forms.Label();
            this.label_result = new System.Windows.Forms.Label();
            this.button3 = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.openFileDialog2 = new System.Windows.Forms.OpenFileDialog();
            this.button4 = new System.Windows.Forms.Button();
            this.lab_data = new System.Windows.Forms.Label();
            this.lab_ini = new System.Windows.Forms.Label();
            this.button5 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.para1 = new System.Windows.Forms.RadioButton();
            this.para2 = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.paraname2 = new System.Windows.Forms.Label();
            this.paraname3 = new System.Windows.Forms.Label();
            this.paraname4 = new System.Windows.Forms.Label();
            this.paraname5 = new System.Windows.Forms.Label();
            this.paraname1 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(213, 97);
            this.button1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(150, 78);
            this.button1.TabIndex = 0;
            this.button1.Text = "归零";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // CDScan
            // 
            this.CDScan.Location = new System.Drawing.Point(18, 377);
            this.CDScan.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.CDScan.Name = "CDScan";
            this.CDScan.Size = new System.Drawing.Size(150, 78);
            this.CDScan.TabIndex = 1;
            this.CDScan.Text = "扫描";
            this.CDScan.UseVisualStyleBackColor = true;
            this.CDScan.Click += new System.EventHandler(this.CDScan_Click);
            // 
            // label_status
            // 
            this.label_status.Location = new System.Drawing.Point(60, 352);
            this.label_status.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label_status.Name = "label_status";
            this.label_status.Size = new System.Drawing.Size(0, 20);
            this.label_status.TabIndex = 2;
            this.label_status.Text = "22223:";
            // 
            // label_CD
            // 
            this.label_CD.Location = new System.Drawing.Point(42, 31);
            this.label_CD.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label_CD.Name = "label_CD";
            this.label_CD.Size = new System.Drawing.Size(0, 20);
            this.label_CD.TabIndex = 3;
            this.label_CD.Text = "C333:";
            // 
            // outputresult
            // 
            this.outputresult.Location = new System.Drawing.Point(213, 377);
            this.outputresult.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.outputresult.Name = "outputresult";
            this.outputresult.Size = new System.Drawing.Size(150, 78);
            this.outputresult.TabIndex = 4;
            this.outputresult.Text = "导表";
            this.outputresult.UseVisualStyleBackColor = true;
            this.outputresult.Click += new System.EventHandler(this.outputresult_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("SimSun", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(18, 31);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(82, 24);
            this.label1.TabIndex = 5;
            this.label1.Text = "CD_IP:";
            // 
            // textBox_IP
            // 
            this.textBox_IP.Font = new System.Drawing.Font("SimSun", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox_IP.Location = new System.Drawing.Point(150, 25);
            this.textBox_IP.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.textBox_IP.Name = "textBox_IP";
            this.textBox_IP.Size = new System.Drawing.Size(211, 35);
            this.textBox_IP.TabIndex = 6;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(18, 97);
            this.button2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(150, 78);
            this.button2.TabIndex = 7;
            this.button2.Text = "连接设备";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // radioODD
            // 
            this.radioODD.AutoSize = true;
            this.radioODD.Font = new System.Drawing.Font("SimSun", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.radioODD.Location = new System.Drawing.Point(286, 32);
            this.radioODD.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.radioODD.Name = "radioODD";
            this.radioODD.Size = new System.Drawing.Size(71, 28);
            this.radioODD.TabIndex = 8;
            this.radioODD.TabStop = true;
            this.radioODD.Text = "ODD";
            this.radioODD.UseVisualStyleBackColor = true;
            this.radioODD.CheckedChanged += new System.EventHandler(this.radioODD_CheckedChanged);
            // 
            // radioEVEN
            // 
            this.radioEVEN.AutoSize = true;
            this.radioEVEN.Font = new System.Drawing.Font("SimSun", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.radioEVEN.Location = new System.Drawing.Point(477, 32);
            this.radioEVEN.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.radioEVEN.Name = "radioEVEN";
            this.radioEVEN.Size = new System.Drawing.Size(83, 28);
            this.radioEVEN.TabIndex = 9;
            this.radioEVEN.TabStop = true;
            this.radioEVEN.Text = "EVEN";
            this.radioEVEN.UseVisualStyleBackColor = true;
            this.radioEVEN.CheckedChanged += new System.EventHandler(this.radioEVEN_CheckedChanged);
            // 
            // radioMux
            // 
            this.radioMux.AutoSize = true;
            this.radioMux.Font = new System.Drawing.Font("SimSun", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.radioMux.Location = new System.Drawing.Point(18, 17);
            this.radioMux.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.radioMux.Name = "radioMux";
            this.radioMux.Size = new System.Drawing.Size(71, 28);
            this.radioMux.TabIndex = 10;
            this.radioMux.TabStop = true;
            this.radioMux.Text = "Mux";
            this.radioMux.UseVisualStyleBackColor = true;
            // 
            // radioDemux
            // 
            this.radioDemux.AutoSize = true;
            this.radioDemux.Font = new System.Drawing.Font("SimSun", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.radioDemux.Location = new System.Drawing.Point(208, 18);
            this.radioDemux.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.radioDemux.Name = "radioDemux";
            this.radioDemux.Size = new System.Drawing.Size(95, 28);
            this.radioDemux.TabIndex = 11;
            this.radioDemux.TabStop = true;
            this.radioDemux.Text = "Demux";
            this.radioDemux.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.radioMux);
            this.panel1.Controls.Add(this.radioDemux);
            this.panel1.Location = new System.Drawing.Point(27, 185);
            this.panel1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(336, 65);
            this.panel1.TabIndex = 12;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.radioODD);
            this.panel2.Controls.Add(this.radioEVEN);
            this.panel2.Location = new System.Drawing.Point(-242, 282);
            this.panel2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(628, 91);
            this.panel2.TabIndex = 13;
            // 
            // label_log
            // 
            this.label_log.AutoSize = true;
            this.label_log.Location = new System.Drawing.Point(26, 551);
            this.label_log.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label_log.Name = "label_log";
            this.label_log.Size = new System.Drawing.Size(43, 20);
            this.label_log.TabIndex = 14;
            this.label_log.Text = "LOG";
            // 
            // label_result
            // 
            this.label_result.AutoSize = true;
            this.label_result.Location = new System.Drawing.Point(24, 485);
            this.label_result.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label_result.Name = "label_result";
            this.label_result.Size = new System.Drawing.Size(51, 20);
            this.label_result.TabIndex = 15;
            this.label_result.Text = "label2";
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(440, 42);
            this.button3.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(112, 35);
            this.button3.TabIndex = 16;
            this.button3.Text = "原始数据";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            this.openFileDialog1.FileOk += new System.ComponentModel.CancelEventHandler(this.openFileDialog1_FileOk);
            // 
            // openFileDialog2
            // 
            this.openFileDialog2.FileName = "openFileDialog2";
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(440, 111);
            this.button4.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(112, 35);
            this.button4.TabIndex = 17;
            this.button4.Text = "导表配置文件";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // lab_data
            // 
            this.lab_data.AutoSize = true;
            this.lab_data.Location = new System.Drawing.Point(622, 45);
            this.lab_data.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lab_data.Name = "lab_data";
            this.lab_data.Size = new System.Drawing.Size(51, 20);
            this.lab_data.TabIndex = 18;
            this.lab_data.Text = "label2";
            // 
            // lab_ini
            // 
            this.lab_ini.AutoSize = true;
            this.lab_ini.Location = new System.Drawing.Point(627, 111);
            this.lab_ini.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lab_ini.Name = "lab_ini";
            this.lab_ini.Size = new System.Drawing.Size(51, 20);
            this.lab_ini.TabIndex = 19;
            this.lab_ini.Text = "label3";
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(440, 185);
            this.button5.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(112, 35);
            this.button5.TabIndex = 20;
            this.button5.Text = "独立导表";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(420, 551);
            this.button6.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(112, 35);
            this.button6.TabIndex = 21;
            this.button6.Text = " 预匹配 ";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // para1
            // 
            this.para1.AutoSize = true;
            this.para1.Location = new System.Drawing.Point(45, 46);
            this.para1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.para1.Name = "para1";
            this.para1.Size = new System.Drawing.Size(75, 24);
            this.para1.TabIndex = 22;
            this.para1.TabStop = true;
            this.para1.Text = "参数1";
            this.para1.UseVisualStyleBackColor = true;
            this.para1.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
            // 
            // para2
            // 
            this.para2.AutoSize = true;
            this.para2.Location = new System.Drawing.Point(192, 46);
            this.para2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.para2.Name = "para2";
            this.para2.Size = new System.Drawing.Size(75, 24);
            this.para2.TabIndex = 23;
            this.para2.TabStop = true;
            this.para2.Text = "参数2";
            this.para2.UseVisualStyleBackColor = true;
            this.para2.CheckedChanged += new System.EventHandler(this.para2_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(456, 351);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(0, 20);
            this.label2.TabIndex = 24;
            // 
            // paraname2
            // 
            this.paraname2.AutoSize = true;
            this.paraname2.Location = new System.Drawing.Point(627, 351);
            this.paraname2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.paraname2.Name = "paraname2";
            this.paraname2.Size = new System.Drawing.Size(51, 20);
            this.paraname2.TabIndex = 25;
            this.paraname2.Text = "label3";
            // 
            // paraname3
            // 
            this.paraname3.AutoSize = true;
            this.paraname3.Location = new System.Drawing.Point(456, 406);
            this.paraname3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.paraname3.Name = "paraname3";
            this.paraname3.Size = new System.Drawing.Size(51, 20);
            this.paraname3.TabIndex = 26;
            this.paraname3.Text = "label4";
            // 
            // paraname4
            // 
            this.paraname4.AutoSize = true;
            this.paraname4.Location = new System.Drawing.Point(627, 406);
            this.paraname4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.paraname4.Name = "paraname4";
            this.paraname4.Size = new System.Drawing.Size(51, 20);
            this.paraname4.TabIndex = 27;
            this.paraname4.Text = "label5";
            // 
            // paraname5
            // 
            this.paraname5.AutoSize = true;
            this.paraname5.Location = new System.Drawing.Point(460, 483);
            this.paraname5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.paraname5.Name = "paraname5";
            this.paraname5.Size = new System.Drawing.Size(51, 20);
            this.paraname5.TabIndex = 28;
            this.paraname5.Text = "label6";
            // 
            // paraname1
            // 
            this.paraname1.AutoSize = true;
            this.paraname1.Location = new System.Drawing.Point(459, 349);
            this.paraname1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.paraname1.Name = "paraname1";
            this.paraname1.Size = new System.Drawing.Size(51, 20);
            this.paraname1.TabIndex = 30;
            this.paraname1.Text = "label8";
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.para1);
            this.panel3.Controls.Add(this.para2);
            this.panel3.Location = new System.Drawing.Point(440, 229);
            this.panel3.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(376, 100);
            this.panel3.TabIndex = 31;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1623, 618);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.paraname1);
            this.Controls.Add(this.paraname5);
            this.Controls.Add(this.paraname4);
            this.Controls.Add(this.paraname3);
            this.Controls.Add(this.paraname2);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.lab_ini);
            this.Controls.Add(this.lab_data);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.label_result);
            this.Controls.Add(this.label_log);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.textBox_IP);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.outputresult);
            this.Controls.Add(this.label_CD);
            this.Controls.Add(this.label_status);
            this.Controls.Add(this.CDScan);
            this.Controls.Add(this.button1);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.Text = "CD_Scan_1.4.0(20190424)";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button CDScan;
        private System.Windows.Forms.Label label_status;
        private System.Windows.Forms.Label label_CD;
        private System.Windows.Forms.Button outputresult;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox_IP;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.RadioButton radioODD;
        private System.Windows.Forms.RadioButton radioEVEN;
        private System.Windows.Forms.RadioButton radioMux;
        private System.Windows.Forms.RadioButton radioDemux;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label_log;
        private System.Windows.Forms.Label label_result;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.OpenFileDialog openFileDialog2;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Label lab_data;
        private System.Windows.Forms.Label lab_ini;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.RadioButton para1;
        private System.Windows.Forms.RadioButton para2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label paraname2;
        private System.Windows.Forms.Label paraname3;
        private System.Windows.Forms.Label paraname4;
        private System.Windows.Forms.Label paraname5;
        private System.Windows.Forms.Label paraname1;
        private System.Windows.Forms.Panel panel3;
    }
}

