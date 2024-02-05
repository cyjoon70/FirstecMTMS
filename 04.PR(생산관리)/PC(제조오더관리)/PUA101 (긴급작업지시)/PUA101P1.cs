using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Threading;
using System.Data;
using System.Data.SqlClient;

namespace PUA101
{
	/// <summary>
	/// PSA010P1�� ���� ��� �����Դϴ�.
	/// </summary>
	public class PUA101P1 : System.Windows.Forms.Form
	{

		Thread  th;
		bool    stopFlg = false;

		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.ProgressBar progressBar1;
		private System.Windows.Forms.Label label1;

		public string EMG_SCH_ID = "";
		public static string PROC_STATUS_CAL_RESO_TIME = "CT";

		/// <summary>
		/// �ʼ� �����̳� �����Դϴ�.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public PUA101P1(string SCH_ID)
		{
			
			InitializeComponent();
			EMG_SCH_ID = SCH_ID;

		}

		/// <summary>
		/// ��� ���� ��� ���ҽ��� �����մϴ�.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form �����̳ʿ��� ������ �ڵ�
		/// <summary>
		/// �����̳� ������ �ʿ��� �޼����Դϴ�.
		/// �� �޼����� ������ �ڵ� ������� �������� ���ʽÿ�.
		/// </summary>
		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(PUA101P1));
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.progressBar1 = new System.Windows.Forms.ProgressBar();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.label1 = new System.Windows.Forms.Label();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// pictureBox1
			// 
			this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
			this.pictureBox1.Location = new System.Drawing.Point(40, 26);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(400, 96);
			this.pictureBox1.TabIndex = 0;
			this.pictureBox1.TabStop = false;
			// 
			// progressBar1
			// 
			this.progressBar1.Location = new System.Drawing.Point(8, 24);
			this.progressBar1.Name = "progressBar1";
			this.progressBar1.Size = new System.Drawing.Size(464, 16);
			this.progressBar1.TabIndex = 3;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Controls.Add(this.progressBar1);
			this.groupBox1.Location = new System.Drawing.Point(8, 144);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(480, 72);
			this.groupBox1.TabIndex = 4;
			this.groupBox1.TabStop = false;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 48);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(464, 16);
			this.label1.TabIndex = 4;
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.pictureBox1);
			this.groupBox2.Location = new System.Drawing.Point(8, 0);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(480, 136);
			this.groupBox2.TabIndex = 5;
			this.groupBox2.TabStop = false;
			// 
			// PUA010P1
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
			this.BackColor = System.Drawing.Color.WhiteSmoke;
			this.ClientSize = new System.Drawing.Size(498, 223);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "PSA010P1";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "SCHEDULE ������...";
			this.Closing += new System.ComponentModel.CancelEventHandler(this.PUA010P1_Closing);
			this.Load += new System.EventHandler(this.PUA010P1_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		#region PUA010P1 Load
		private void PUA010P1_Load(object sender, System.EventArgs e)
		{
			try
			{
				th = new Thread(new ThreadStart(SchStart));
				th.Start();
			}
			catch(Exception f)
			{
				SystemBase.Loggers.Log("PUA010.SCHEDULE.Scheduld() ", f.ToString());
				MessageBox.Show(f.ToString(),SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		#endregion

		#region ������� ó��
		public void SchStart()
		{
			string ERRCode = "OK";
			string MSGCode = "P0010";

			string SchDplNm = SCH_PROG.HasDplySch();

			if(SchDplNm != null) 
			{
				SystemBase.MessageBoxComm.Show(SystemBase.Base.MessageRtn("P0038", SchDplNm));
				goto EXIT_FUNC;
			}

			SqlConnection dbConn = SystemBase.DbOpen.DBCON();
			SqlCommand cmd = dbConn.CreateCommand();
			SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);
			cmd.Transaction = Trans;
			cmd.CommandTimeout = 10000;

			try
			{
				/////////////////////// �������� ������ ���� ��� ///////////////////
				SCH_PROG.InsSchProc(PUA101.PROC_TYPE, PROC_STATUS_CAL_RESO_TIME, cmd);
				/////////////////////// �������� ������ ���� ��� ///////////////////

				/////////////////////// RESO_WORK_TIME_TEMP ����(������������)///////
				SCH_PROG.DelResoWorkTime(cmd);
				/////////////////////// RESO_WORK_TIME_TEMP ����(������������)///////

				////////////////////////////// MBOP ������ ���� ////////////////////////////////////////
				MBOPCOPY.MBOP_COPY(progressBar1, label1, EMG_SCH_ID, cmd, dbConn, Trans);
				////////////////////////////// MBOP ������ ���� ////////////////////////////////////////

				////////////////////////////// ������ ���� ////////////////////////////////////////
				BACKWARD.BACKWARD_EMEG(progressBar1, label1, EMG_SCH_ID, cmd, dbConn, Trans);
				////////////////////////////// ������ ���� ////////////////////////////////////////

				/////////////////// ��� �ݿ� (MPS ���, �۾����ü�, �������) ////////////////////
				RST_UP.SCH_RST_UP(label1, cmd, dbConn, Trans);
				/////////////////// ��� �ݿ� (MPS ���, �۾����ü�, �������) ////////////////////

				/////////////////////// �������� ������ ���� ���� ///////////////////
				SCH_PROG.DelSchProc(cmd);
				/////////////////////// �������� ������ ���� ���� ///////////////////

				progressBar1.Value = 100;

				Trans.Commit();
			}
			catch(Exception f)
			{
				Trans.Rollback();

				ERRCode = "ER";
				MSGCode = f.ToString();

				SystemBase.Loggers.Log("PUA101", f.ToString());
			}
			dbConn.Close();
			//SystemBase.MessageBoxComm.Show(SystemBase.Base.MessageRtn(MSGCode));
			if (ERRCode == "OK")
			{
				MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode),SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
			else
			{
				MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode),SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}

			EXIT_FUNC:
					 
				this.DialogResult = DialogResult.OK;
				this.Close();
		}
		#endregion

		#region �ݱ�
		private void PUA010P1_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			stopFlg = true;    // ���� ��Ų��.

			if(th.Join(3000))  // 5000�� ���� ��ٸ���.
			{
				th.Abort();
			}
		}
		#endregion
	}
}