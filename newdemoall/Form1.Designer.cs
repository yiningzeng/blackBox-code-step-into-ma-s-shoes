namespace newdemoall
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
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tbSavePath = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.button9 = new System.Windows.Forms.Button();
            this.button11 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.fuckType = new System.Windows.Forms.ComboBox();
            this.button4 = new System.Windows.Forms.Button();
            this.btnFuckStart = new System.Windows.Forms.Button();
            this.btnFuckDetectThisPcb = new System.Windows.Forms.Button();
            this.btnDetectPath = new System.Windows.Forms.Button();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.btnFuckTest = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(12, 9);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(511, 492);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Location = new System.Drawing.Point(576, 327);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(234, 1);
            this.tableLayoutPanel2.TabIndex = 90;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Location = new System.Drawing.Point(576, 261);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(234, 1);
            this.tableLayoutPanel1.TabIndex = 89;
            // 
            // tbSavePath
            // 
            this.tbSavePath.Location = new System.Drawing.Point(655, 154);
            this.tbSavePath.Name = "tbSavePath";
            this.tbSavePath.Size = new System.Drawing.Size(155, 21);
            this.tbSavePath.TabIndex = 88;
            this.tbSavePath.Text = "图片保存地址,不填为不保存";
            this.tbSavePath.Click += new System.EventHandler(this.textBox1_Click);
            this.tbSavePath.Enter += new System.EventHandler(this.textBox1_Enter);
            this.tbSavePath.Leave += new System.EventHandler(this.textBox1_Leave);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(573, 156);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(76, 16);
            this.label1.TabIndex = 87;
            this.label1.Text = "保存图片";
            // 
            // button9
            // 
            this.button9.Location = new System.Drawing.Point(576, 278);
            this.button9.Name = "button9";
            this.button9.Size = new System.Drawing.Size(234, 34);
            this.button9.TabIndex = 86;
            this.button9.Text = "新建拍摄方案";
            this.button9.UseVisualStyleBackColor = true;
            this.button9.Click += new System.EventHandler(this.button9_Click_1);
            // 
            // button11
            // 
            this.button11.Location = new System.Drawing.Point(705, 213);
            this.button11.Name = "button11";
            this.button11.Size = new System.Drawing.Size(105, 32);
            this.button11.TabIndex = 85;
            this.button11.Text = "停止";
            this.button11.UseVisualStyleBackColor = true;
            this.button11.Click += new System.EventHandler(this.button11_Click_1);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(576, 213);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(105, 32);
            this.button5.TabIndex = 84;
            this.button5.Text = "暂停";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold);
            this.label4.Location = new System.Drawing.Point(573, 27);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(110, 16);
            this.label4.TabIndex = 83;
            this.label4.Text = "使用拍摄方案";
            // 
            // fuckType
            // 
            this.fuckType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.fuckType.FormattingEnabled = true;
            this.fuckType.Location = new System.Drawing.Point(689, 27);
            this.fuckType.Name = "fuckType";
            this.fuckType.Size = new System.Drawing.Size(121, 20);
            this.fuckType.TabIndex = 82;
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(576, 347);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(234, 34);
            this.button4.TabIndex = 81;
            this.button4.Text = "修改运行规则";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click_1);
            // 
            // btnFuckStart
            // 
            this.btnFuckStart.Location = new System.Drawing.Point(576, 83);
            this.btnFuckStart.Name = "btnFuckStart";
            this.btnFuckStart.Size = new System.Drawing.Size(234, 33);
            this.btnFuckStart.TabIndex = 80;
            this.btnFuckStart.Text = "启动拍摄方案（实时拼图检测）";
            this.btnFuckStart.UseVisualStyleBackColor = true;
            this.btnFuckStart.Click += new System.EventHandler(this.btnFuckStart_Click);
            // 
            // btnFuckDetectThisPcb
            // 
            this.btnFuckDetectThisPcb.Enabled = false;
            this.btnFuckDetectThisPcb.Location = new System.Drawing.Point(576, 426);
            this.btnFuckDetectThisPcb.Name = "btnFuckDetectThisPcb";
            this.btnFuckDetectThisPcb.Size = new System.Drawing.Size(100, 34);
            this.btnFuckDetectThisPcb.TabIndex = 91;
            this.btnFuckDetectThisPcb.Text = "检测本块板";
            this.btnFuckDetectThisPcb.UseVisualStyleBackColor = true;
            this.btnFuckDetectThisPcb.Click += new System.EventHandler(this.btnFuckDetectThisPcb_Click);
            // 
            // btnDetectPath
            // 
            this.btnDetectPath.Location = new System.Drawing.Point(710, 426);
            this.btnDetectPath.Name = "btnDetectPath";
            this.btnDetectPath.Size = new System.Drawing.Size(100, 34);
            this.btnDetectPath.TabIndex = 92;
            this.btnDetectPath.Text = "检测文件夹";
            this.btnDetectPath.UseVisualStyleBackColor = true;
            this.btnDetectPath.Click += new System.EventHandler(this.btnDetectPath_Click);
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.DecimalPlaces = 2;
            this.numericUpDown1.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.numericUpDown1.Location = new System.Drawing.Point(690, 393);
            this.numericUpDown1.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(120, 21);
            this.numericUpDown1.TabIndex = 93;
            this.numericUpDown1.Value = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(576, 395);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 94;
            this.label2.Text = "灵敏度";
            // 
            // btnFuckTest
            // 
            this.btnFuckTest.Location = new System.Drawing.Point(576, 478);
            this.btnFuckTest.Name = "btnFuckTest";
            this.btnFuckTest.Size = new System.Drawing.Size(234, 23);
            this.btnFuckTest.TabIndex = 95;
            this.btnFuckTest.Text = "测试拼图+检测（测试按钮）";
            this.btnFuckTest.UseVisualStyleBackColor = true;
            this.btnFuckTest.Visible = false;
            this.btnFuckTest.Click += new System.EventHandler(this.btnFuckTest_Click);
            // 
            // Form1
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(870, 513);
            this.Controls.Add(this.btnFuckTest);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.numericUpDown1);
            this.Controls.Add(this.btnDetectPath);
            this.Controls.Add(this.btnFuckDetectThisPcb);
            this.Controls.Add(this.tableLayoutPanel2);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.tbSavePath);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button9);
            this.Controls.Add(this.button11);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.fuckType);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.btnFuckStart);
            this.Controls.Add(this.pictureBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "pcb图像采集软件";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TextBox tbSavePath;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button9;
        private System.Windows.Forms.Button button11;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox fuckType;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button btnFuckStart;
        private System.Windows.Forms.Button btnFuckDetectThisPcb;
        private System.Windows.Forms.Button btnDetectPath;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnFuckTest;
    }
}

