namespace UIForm
{
    partial class FindText
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FindText));
            this.txtFindText = new C1.Win.C1Input.C1TextBox();
            this.c1Label4 = new C1.Win.C1Input.C1Label();
            this.btnNextFind = new C1.Win.C1Input.C1Button();
            this.btnFindClose = new C1.Win.C1Input.C1Button();
            ((System.ComponentModel.ISupportInitialize)(this.txtFindText)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label4)).BeginInit();
            this.SuspendLayout();
            // 
            // txtFindText
            // 
            this.txtFindText.AutoSize = false;
            this.txtFindText.BackColor = System.Drawing.Color.White;
            this.txtFindText.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.txtFindText.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtFindText.Location = new System.Drawing.Point(100, 18);
            this.txtFindText.Name = "txtFindText";
            this.txtFindText.Size = new System.Drawing.Size(160, 21);
            this.txtFindText.TabIndex = 2;
            this.txtFindText.Tag = null;
            this.txtFindText.VerticalAlign = C1.Win.C1Input.VerticalAlignEnum.Middle;
            this.txtFindText.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtFindText_KeyDown);
            // 
            // c1Label4
            // 
            this.c1Label4.BackColor = System.Drawing.Color.Transparent;
            this.c1Label4.BorderColor = System.Drawing.Color.Transparent;
            this.c1Label4.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.c1Label4.Location = new System.Drawing.Point(10, 17);
            this.c1Label4.Name = "c1Label4";
            this.c1Label4.Size = new System.Drawing.Size(82, 21);
            this.c1Label4.TabIndex = 3;
            this.c1Label4.Tag = null;
            this.c1Label4.Text = "찾을 내용";
            this.c1Label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.c1Label4.TextDetached = true;
            this.c1Label4.Value = "";
            // 
            // btnNextFind
            // 
            this.btnNextFind.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnNextFind.BackgroundImage")));
            this.btnNextFind.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnNextFind.Location = new System.Drawing.Point(275, 13);
            this.btnNextFind.Name = "btnNextFind";
            this.btnNextFind.Size = new System.Drawing.Size(86, 25);
            this.btnNextFind.TabIndex = 4;
            this.btnNextFind.Text = "다음 찾기";
            this.btnNextFind.UseVisualStyleBackColor = true;
            this.btnNextFind.Click += new System.EventHandler(this.btnNextFind_Click);
            // 
            // btnFindClose
            // 
            this.btnFindClose.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnFindClose.BackgroundImage")));
            this.btnFindClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnFindClose.Location = new System.Drawing.Point(275, 48);
            this.btnFindClose.Name = "btnFindClose";
            this.btnFindClose.Size = new System.Drawing.Size(86, 25);
            this.btnFindClose.TabIndex = 5;
            this.btnFindClose.Text = "취소";
            this.btnFindClose.UseVisualStyleBackColor = true;
            this.btnFindClose.Click += new System.EventHandler(this.btnFindClose_Click);
            // 
            // FindText
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(376, 82);
            this.Controls.Add(this.btnFindClose);
            this.Controls.Add(this.btnNextFind);
            this.Controls.Add(this.c1Label4);
            this.Controls.Add(this.txtFindText);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FindText";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "찾기";
            ((System.ComponentModel.ISupportInitialize)(this.txtFindText)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label4)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private C1.Win.C1Input.C1TextBox txtFindText;
        private C1.Win.C1Input.C1Label c1Label4;
        private C1.Win.C1Input.C1Button btnNextFind;
        private C1.Win.C1Input.C1Button btnFindClose;
    }
}