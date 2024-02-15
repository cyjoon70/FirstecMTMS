using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;
using System.Diagnostics;
using FarPoint.Win.Spread;
using FarPoint.Win.Spread.Model;
using FarPoint.Win.Spread.CellType;
using EDocument.Extensions.FpSpreadExtension;
using EDocument.Network;
using EDocument.Spread;

namespace SC.QA010
{
	public partial class QA010 : UIForm.FPCOMM1
	{

		#region 변수
		string NoticeSeq = string.Empty;
		string ApprId = string.Empty;
		bool GwStatus = true;
		const string docCtgCd = "SCM";  //SCM

		// 디테일 그리드 컬럼(문서 목록)
		int colDocId = -1;
		int colSvrPath = -1;
		int colSvrFnm = -1;
		int colOrgFnm = -1;
		int colDocCd = -1;
		int colDocNm = -1;
		int colDocNo = -1;
		int colRevNo = -1;
		int colRemark = -1;
		int colRegUsrId = -1;
		int colRegUsrNm = -1;

		/// <summary>문서코드별 문서번호 유무</summary>
		Dictionary<string, string> docNoReqs = null;
		/// <summary>첨부파일목록 파일버튼 관리자</summary>
		FileButtonManager buttonManager;
		/// <summary>첨부문서표시 관리자</summary>
		AttachmentManager attachmentManager;

		string[] returnVal = null;          // 2022.05.20. hma 추가
		string strSaveFlag = "";            // 2022.05.20. hma 추가
		int iDocCnt = 0;                    // 2022.05.20. hma 추가
		#endregion

		#region 생성자
		public QA010()
		{
			InitializeComponent();
		}
		#endregion

		#region Method
		/// <summary>
		/// 임시로 다운로드한 파일을 모두 삭제합니다.
		/// </summary>
		void ViewDeleteTempFiles()
		{
			foreach (FileInfo f in new DirectoryInfo(Path.GetTempPath()).GetFiles(ViewGetTempFilenamePrefix() + "*.*")) // 프리픽스파일 모두 삭제
			{
				try { f.Delete(); }
				catch { }
			}
		}

		/// <summary>
		/// 임시파일명의 프리픽스로 사용할 고정된 문자열을 반환합니다.
		/// </summary>
		/// <returns></returns>
		string ViewGetTempFilenamePrefix()
		{
			return string.Format("{0:X}", this.GetHashCode()) + "_";
		}
		#endregion

		#region Form Load
		private void QA010_Load(object sender, EventArgs e)
		{
			try
			{
				SystemBase.Validation.GroupBox_Setting(groupBox1);

				// 첨부파일 분류 콤보박스 세팅
				SystemBase.ComboMake.C1Combo(cboType, "usp_SC010 @pType='C2'", 3);

				// 업무구분 콤보박스 세팅
				SystemBase.ComboMake.C1Combo(cboAppr, "usp_SC010 @pType='C1'", 3);

				G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "문서종류")] = SystemBase.ComboMake.ComboOnGrid("usp_T_DOC_CODE @pTYPE = 'S1', @pDOC_CTG_CD = 'SCM', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 0); // 문서종류

				UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);

				SheetView sheet = fpSpread1.ActiveSheet;
				colDocId = sheet.FindHeaderColumnIndex("문서ID");
				colSvrPath = sheet.FindHeaderColumnIndex("서버경로");
				colSvrFnm = sheet.FindHeaderColumnIndex("서버파일명");
				colOrgFnm = sheet.FindHeaderColumnIndex("파일명") + 3;     // 파일선택 버튼, 미리보기 버튼, 다운로드 버튼 다음이 파일명 컬럼
				colDocCd = sheet.FindHeaderColumnIndex("문서코드");
				colDocNm = sheet.FindHeaderColumnIndex("문서종류");
				colDocNo = sheet.FindHeaderColumnIndex("문서번호");
				colRevNo = sheet.FindHeaderColumnIndex("개정번호");
				colRemark = sheet.FindHeaderColumnIndex("비고");
				colRegUsrId = sheet.FindHeaderColumnIndex("등록자ID");
				colRegUsrNm = sheet.FindHeaderColumnIndex("등록자");

				// 첨부파일목록 파일버튼 관리자 초기화
				buttonManager = new FileButtonManager(fpSpread1.ActiveSheet, FileButtonManager.ServerFileType.DocumentFile)
				{
					ServerPathColumnIndex = colSvrPath,
					ServerFilenameColumnIndex = colSvrFnm,
					FileSelectButtonColumnIndex = colOrgFnm - 3,
					FileViewButtonColumnIndex = colOrgFnm - 2,
					FileDownloadButtonColumnIndex = colOrgFnm - 1,
					FilenameColumnIndex = colOrgFnm,
					DocTypeNameColumnIndex = colDocNm,
					DocRevisionColumnIndex = colRevNo,
					DocNumberColumnIndex = colDocNo,
				};

				//picDoc.SizeMode = PictureBoxSizeMode.AutoSize;      // 2022.02.23 hma 추가

				SearchExec();
			}
			catch (Exception f)
			{
				MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		#endregion

		#region SelectExec() 그리드 조회 로직
		private void SelectExec(bool Msg)
		{
			this.Cursor = Cursors.WaitCursor;

			try
			{
				string strQuery = "";
				strQuery = " usp_SC010 @pTYPE = 'S1' ";
				strQuery = strQuery + ", @pCOMP_CODE = '" + SystemBase.Base.gstrCOMCD + "' ";
				strQuery = strQuery + ", @pJOB_TYPE ='" + cboType.SelectedValue.ToString() + "' ";
				strQuery = strQuery + ", @pAPPR ='" + cboAppr.SelectedValue.ToString() + "' ";
				strQuery = strQuery + ", @pUP_ID ='" + SystemBase.Base.gstrUserID + "' ";

				UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);

				fpSpread1.Sheets[0].SetColumnMerge(3, FarPoint.Win.Spread.Model.MergePolicy.Always);
				fpSpread1.Sheets[0].SetColumnMerge(4, FarPoint.Win.Spread.Model.MergePolicy.Always);

				for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
				{
					if ((fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "승인")].Text == "True") ||
							(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "등록구분")].Text == "N"))
					{
						// readonly 처리
						UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "승인") + "|3");
					}
				}

				buttonManager.UpdateButtons(); // 버튼 업데이트

				SheetView sheet = fpSpread1.ActiveSheet;
				((TextCellType)sheet.Columns[colRevNo].CellType).MaxLength = 5; // 개정번호
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}

			this.Cursor = Cursors.Default;
		}
		#endregion

		#region NewExec()
		protected override void NewExec()
		{
			SystemBase.Validation.GroupBox_Reset(groupBox1);

			//그리드 초기화
			UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
		}
		#endregion

		#region SearchExec() -- 검색
		protected override void SearchExec()
		{
			SelectExec(true);
		}
		#endregion

		#region SaveExec()
		protected override void SaveExec()
		{
			string ERRCode = "ER", MSGCode = "";

			SqlConnection dbConn = SystemBase.DbOpen.DBCON();
			SqlCommand cmd = dbConn.CreateCommand();
			SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

			try
			{
				for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
				{
					string strHead = fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text;

					if (strHead.Length > 0)
					{
						string strSql = " usp_SC010 @pTYPE = 'U1'";
						strSql += ", @pCOMP_CODE = '" + SystemBase.Base.gstrCOMCD + "' ";

						if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "승인")].Text == "True")
							strSql += ", @pAPPR = 'Y' ";
						else
							strSql += ", @pAPPR = 'N' ";

						strSql += ", @pDOC_ID = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "문서ID")].Text + "' ";
						strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";

						DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
						ERRCode = ds.Tables[0].Rows[0][0].ToString();
						MSGCode = ds.Tables[0].Rows[0][1].ToString();

						if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }   // ER 코드 Return시 점프

					}
				}

				if (ERRCode == "ER")
				{
					Trans.Rollback();
					goto Exit;  // ER 코드 Return시 점프
				}
			}
			catch (Exception ex)
			{
				Trans.Rollback();
				MessageBox.Show(ex.ToString());
				MSGCode = "P0001";
				goto Exit;  // ER 코드 Return시 점프
			}
			Trans.Commit();

		Exit:
			dbConn.Close();
			MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode));
			SearchExec();

		}
		#endregion

		#region 동일 파일번호 동시 체크 처리 및 파일 보기 및 다운로드
		protected override void fpSpread1_ChangeEvent(int row, int col)
		{
			string strFileNo = string.Empty;
			string strAppr = string.Empty;

			try
			{
				if (col == SystemBase.Base.GridHeadIndex(GHIdx1, "승인"))
				{
					strFileNo = fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "파일번호")].Value.ToString();
					strAppr = fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "승인")].Value.ToString();

					for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
					{
						if (strFileNo == fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "파일번호")].Value.ToString())
						{
							if (strAppr == "True")
							{
								fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "승인")].Value = true;
								UIForm.FPMake.fpChange(fpSpread1, i);
							}

							if (strAppr == "False")
							{
								fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "승인")].Value = false;
								UIForm.FPMake.fpChange(fpSpread1, i);
							}
						}
					}
				}
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void fpSpread1_CellClick(object sender, CellClickEventArgs e)
		{
			string strFileNo = string.Empty;

			if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "승인"))
			{
				strFileNo = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "파일번호")].Value.ToString();

				for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
				{
					if (strFileNo == fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "파일번호")].Value.ToString())
					{
						if (fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "승인")].Value.ToString() == "True")
						{
							fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "승인")].Value = true;
							UIForm.FPMake.fpChange(fpSpread1, i);
						}

						if (fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "승인")].Value.ToString() == "False")
						{
							fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "승인")].Value = false;
							UIForm.FPMake.fpChange(fpSpread1, i);
						}
					}
				}
			}
		}
		#endregion
	}
}
