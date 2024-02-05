namespace CZ.CZA090
{
    partial class CZA090
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CZA090));
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.dtpCloseMonth = new C1.Win.C1Input.C1DateEdit();
            this.c1Label1 = new C1.Win.C1Input.C1Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.rdoCloseDivY = new System.Windows.Forms.RadioButton();
            this.rdoCloseDivN = new System.Windows.Forms.RadioButton();
            this.btnExec = new C1.Win.C1Input.C1Button();
            this.c1Label4 = new C1.Win.C1Input.C1Label();
            this.panButton1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.BtnDel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.BtnHelp)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.BtnDelete)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.BtnRowIns)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.BtnRCopy)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.BtnClose)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.BtnPrint)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.BtnCancel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.BtnSearch)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.BtnInsert)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.BtnNew)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.BtnExcel)).BeginInit();
            this.panel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dtpCloseMonth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label1)).BeginInit();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label4)).BeginInit();
            this.SuspendLayout();
            // 
            // panButton1
            // 
            this.panButton1.Size = new System.Drawing.Size(686, 64);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 64);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(686, 139);
            this.panel1.TabIndex = 8;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.dtpCloseMonth);
            this.groupBox1.Controls.Add(this.c1Label1);
            this.groupBox1.Controls.Add(this.panel2);
            this.groupBox1.Controls.Add(this.btnExec);
            this.groupBox1.Controls.Add(this.c1Label4);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(686, 139);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // dtpCloseMonth
            // 
            this.dtpCloseMonth.AutoSize = false;
            this.dtpCloseMonth.BackColor = System.Drawing.Color.White;
            this.dtpCloseMonth.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            // 
            // 
            // 
            this.dtpCloseMonth.Calendar.DayNameLength = 1;
            this.dtpCloseMonth.CustomFormat = "yyyy-MM";
            this.dtpCloseMonth.FormatType = C1.Win.C1Input.FormatTypeEnum.CustomFormat;
            this.dtpCloseMonth.Location = new System.Drawing.Point(116, 17);
            this.dtpCloseMonth.Name = "dtpCloseMonth";
            this.dtpCloseMonth.Size = new System.Drawing.Size(106, 21);
            this.dtpCloseMonth.TabIndex = 25;
            this.dtpCloseMonth.Tag = ";2;;";
            this.dtpCloseMonth.VerticalAlign = C1.Win.C1Input.VerticalAlignEnum.Middle;
            this.dtpCloseMonth.VisibleButtons = C1.Win.C1Input.DropDownControlButtonFlags.DropDown;
            // 
            // c1Label1
            // 
            this.c1Label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.c1Label1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.c1Label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c1Label1.Location = new System.Drawing.Point(12, 44);
            this.c1Label1.Name = "c1Label1";
            this.c1Label1.Size = new System.Drawing.Size(104, 21);
            this.c1Label1.TabIndex = 23;
            this.c1Label1.Tag = null;
            this.c1Label1.Text = "작업구분";
            this.c1Label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.c1Label1.TextDetached = true;
            this.c1Label1.Value = "";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.rdoCloseDivY);
            this.panel2.Controls.Add(this.rdoCloseDivN);
            this.panel2.Location = new System.Drawing.Point(116, 44);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(125, 21);
            this.panel2.TabIndex = 3;
            // 
            // rdoCloseDivY
            // 
            this.rdoCloseDivY.Checked = true;
            this.rdoCloseDivY.Location = new System.Drawing.Point(10, 2);
            this.rdoCloseDivY.Name = "rdoCloseDivY";
            this.rdoCloseDivY.Size = new System.Drawing.Size(52, 18);
            this.rdoCloseDivY.TabIndex = 0;
            this.rdoCloseDivY.TabStop = true;
            this.rdoCloseDivY.Text = "마감";
            this.rdoCloseDivY.UseVisualStyleBackColor = true;
            this.rdoCloseDivY.CheckedChanged += new System.EventHandler(this.rdoCloseDivY_CheckedChanged);
            // 
            // rdoCloseDivN
            // 
            this.rdoCloseDivN.Location = new System.Drawing.Point(64, 2);
            this.rdoCloseDivN.Name = "rdoCloseDivN";
            this.rdoCloseDivN.Size = new System.Drawing.Size(50, 18);
            this.rdoCloseDivN.TabIndex = 1;
            this.rdoCloseDivN.Text = "취소";
            this.rdoCloseDivN.UseVisualStyleBackColor = true;
            this.rdoCloseDivN.CheckedChanged += new System.EventHandler(this.rdoCloseDivN_CheckedChanged);
            // 
            // btnExec
            // 
            this.btnExec.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnExec.BackgroundImage")));
            this.btnExec.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnExec.Location = new System.Drawing.Point(12, 91);
            this.btnExec.Name = "btnExec";
            this.btnExec.Size = new System.Drawing.Size(104, 25);
            this.btnExec.TabIndex = 22;
            this.btnExec.Text = "작업실행";
            this.btnExec.UseVisualStyleBackColor = true;
            this.btnExec.Click += new System.EventHandler(this.btnExec_Click);
            // 
            // c1Label4
            // 
            this.c1Label4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.c1Label4.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.c1Label4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c1Label4.Location = new System.Drawing.Point(12, 17);
            this.c1Label4.Name = "c1Label4";
            this.c1Label4.Size = new System.Drawing.Size(104, 21);
            this.c1Label4.TabIndex = 2;
            this.c1Label4.Tag = null;
            this.c1Label4.Text = "작업대상년월";
            this.c1Label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.c1Label4.TextDetached = true;
            this.c1Label4.Value = "";
            // 
            // CZA090
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(686, 203);
            this.Controls.Add(this.panel1);
            this.Name = "CZA090";
            this.Text = "원가마감작업";
            this.Load += new System.EventHandler(this.CZA090_Load);
            this.Controls.SetChildIndex(this.panButton1, 0);
            this.Controls.SetChildIndex(this.panel1, 0);
            this.panButton1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.BtnDel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.BtnHelp)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.BtnDelete)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.BtnRowIns)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.BtnRCopy)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.BtnClose)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.BtnPrint)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.BtnCancel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.BtnSearch)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.BtnInsert)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.BtnNew)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.BtnExcel)).EndInit();
            this.panel1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dtpCloseMonth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label1)).EndInit();
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.c1Label4)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private C1.Win.C1Input.C1Label c1Label4;
        private C1.Win.C1Input.C1Button btnExec;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.RadioButton rdoCloseDivY;
        private System.Windows.Forms.RadioButton rdoCloseDivN;
        private C1.Win.C1Input.C1Label c1Label1;
        private C1.Win.C1Input.C1DateEdit dtpCloseMonth;
    }
}