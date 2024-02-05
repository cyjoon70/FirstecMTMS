using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WNDW;
using System.Data.SqlClient;

namespace SC.QA009
{
	public partial class QA009P1 : UIForm.FPCOMM1
	{

		#region 변수
		string gStrAuth = string.Empty;
		#endregion

		#region 생성자
		public QA009P1()
		{
			InitializeComponent();
		}
		#endregion

		#region Form Load
		private void QA009P1_Load(object sender, EventArgs e)
		{
			this.Text = "협력사 점수 부여 현황";

			GetAuth();

			UIForm.Buttons.ReButton("110000010001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);

			//GroupBo x1 초기화
			SystemBase.Validation.GroupBox_Setting(groupBox1);

			//그리드 초기화
			UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

			SetInit();

		}

		private void SetInit()
		{
			//기타 세팅	
			dtssBASIC_YEAR.Text = SystemBase.Base.ServerTime("YYMMDD").Substring(0, 4);
		}

		private void GetAuth()
		{
			DataTable dt;
			string strQuery = string.Empty;
			strQuery = "SELECT DBO.UFN_GETQMAUTH ('" + SystemBase.Base.gstrUserID + "')";

			dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

			if (dt != null) gStrAuth = dt.Rows[0][0].ToString();
		}
		#endregion

		#region SearchExec()
		protected override void SearchExec()
		{
			//조회조건 필수 체크
			if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
			{
				this.Cursor = Cursors.WaitCursor;

				try
				{
					string strQuery = "usp_SC009 @pTYPE = 'S2'";
					strQuery += ", @pCOMP_CODE	= '" + SystemBase.Base.gstrCOMCD + "' ";
					strQuery += ", @sYEAR		= '" + dtssBASIC_YEAR.Text + "'";
					strQuery += ", @sCUST_CD	= '" + txtCUST_CD.Text + "'";

					UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
				}
				catch (Exception f)
				{
					SystemBase.Loggers.Log(this.Name, f.ToString());
					MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회중 오류가 발생하였습니다.
				}

				this.Cursor = Cursors.Default;
			}

		}
		#endregion

		#region 협력업체 조회
		private void btnCust_Click(object sender, EventArgs e)
		{
			GetCustInfo(txtCUST_CD, txtCUST_NM);
		}

		private void txtCUST_CD_TextChanged(object sender, EventArgs e)
		{
			txtCUST_NM.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtCUST_CD.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
		}

		private void GetCustInfo(C1.Win.C1Input.C1TextBox id, C1.Win.C1Input.C1TextBox name)
		{
			try
			{
				WNDW002 pu = new WNDW002(id.Text, "");
				pu.MaximizeBox = false;
				pu.ShowDialog();
				if (pu.DialogResult == DialogResult.OK)
				{
					string[] Msgs = pu.ReturnVal;

					id.Value = Msgs[1].ToString();
					name.Value = Msgs[2].ToString();
				}

			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "거래처 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
			}
		}
		#endregion

		#region New
		protected override void NewExec()
		{
			SystemBase.Validation.GroupBox_Reset(groupBox1);

			fpSpread1.Sheets[0].Rows.Count = 0;

			SetInit();
		}
		#endregion

		#region SaveExec()
		protected override void SaveExec()
		{
			string ERRCode = "ER", MSGCode = "";
			decimal dQPA = 0, dQSA = 0, dADD_POINT = 0, dSUBTRACT_POINT = 0;

			SqlConnection dbConn = SystemBase.DbOpen.DBCON();
			SqlCommand cmd = dbConn.CreateCommand();
			SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

			if (SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", false))
			{
				try
				{
					for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
					{
						string strHead = fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text;
						string strGbn = "";

						if (strHead.Length > 0)
						{
							if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "QPA")].Value == null)
								dQPA = 0;
							else
								dQPA = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "QPA")].Value);

							if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "QSA")].Value == null)
								dQSA = 0;
							else
								dQSA = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "QSA")].Value);

							if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "가점")].Value == null)
								dADD_POINT = 0;
							else
								dADD_POINT = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "가점")].Value);

							if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "감점")].Value == null)
								dSUBTRACT_POINT = 0;
							else
								dSUBTRACT_POINT = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "감점")].Value);


							string strQuery = "";
							strQuery = " usp_SC009 @pTYPE = 'M1' ";
							strQuery = strQuery + ", @pCOMP_CODE		= '" + SystemBase.Base.gstrCOMCD + "' ";
							strQuery = strQuery + ", @pCUST_CD			= '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "협력사코드")].Value + "' ";	// 협력사코드
							strQuery = strQuery + ", @pBASIC_YEAR		= '" + dtssBASIC_YEAR.Text + "' ";                                                                      // 기준년도
							strQuery = strQuery + ", @pQPA				= " + dQPA + " ";                                                                                       // QPA 점수
							strQuery = strQuery + ", @pQSA				= " + dQSA + " ";                                                                                       // QSA 점수
							strQuery = strQuery + ", @pADD_POINT		= " + dADD_POINT + " ";                                                                                 // 가점
							strQuery = strQuery + ", @pSUBTRACT_POINT	= " + dSUBTRACT_POINT + " ";                                                                            // 감점
							strQuery = strQuery + ", @pIN_ID			= '" + SystemBase.Base.gstrUserID + "' ";                                                               // 등록자
							strQuery = strQuery + ", @pUP_ID			= '" + SystemBase.Base.gstrUserID + "' ";
							strQuery = strQuery + ", @pAUTH				= '" + gStrAuth + "' ";
							
							DataSet ds = SystemBase.DbOpen.TranDataSet(strQuery, dbConn, Trans);
							ERRCode = ds.Tables[0].Rows[0][0].ToString();
							MSGCode = ds.Tables[0].Rows[0][1].ToString();

							if (ERRCode == "ER")
							{
								Trans.Rollback();
								goto Exit;  // ER 코드 Return시 점프
							}
							else
							{

							}
						}
					}

					Trans.Commit();
				}
				catch (Exception ex)
				{
					Trans.Rollback();
					MessageBox.Show(ex.ToString());
					MSGCode = "P0001";
					goto Exit;  // ER 코드 Return시 점프
				}

			Exit:
				dbConn.Close();
				MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode));
				if (ERRCode == "OK")
					SearchExec();
			}

		}
		#endregion
	}
}
