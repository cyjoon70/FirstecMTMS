#region 작성정보
/*********************************************************************/
// 단위업무명 : 품목정보등록(멀티)
// 작 성 자 : 조 홍 태
// 작 성 일 : 2013-02-01
// 작성내용 : 품목 정보 등록 및 관리
// 수 정 일 :
// 수 정 자 :
// 수정내용 :
// 비    고 :
/*********************************************************************/
#endregion

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Data;
using System.Data.SqlClient;
using System.Data.OleDb;

namespace IF.INF010
{
    public partial class INF010 : UIForm.FPCOMM1
    {
        #region 생성자
        public INF010()
        {
            InitializeComponent();
        }
		#endregion

		#region NewExec() New 버튼 클릭 이벤트
		protected override void NewExec()
		{
			SystemBase.Validation.GroupBox_Reset(groupBox1);

			UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
		}
		#endregion

		#region Form Load 시
		private void INF010_Load(object sender, System.EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(groupBox1);//필수 적용

            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "품목계정")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE='COMM'  , @pCODE = 'B036', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'", 0);
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "품목구분")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE='COMM2' , @pCODE = 'P032', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'", 0);
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "조달구분")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE='COMM'  , @pCODE = 'B011', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'", 0);
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "재질구분")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE='COMM'  , @pCODE = 'D035', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'", 0);

            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "생산전략")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE='COMM',  @pCODE ='B041', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'", 0);
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "Lot Size")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE='COMM2', @pCODE ='B022', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'", 0);
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "출고방법")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE='COMM' , @pCODE ='B030', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'", 0);
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "출고단위")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE='COMM' , @pCODE ='Z005', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'", 0);
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "재고단위")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE='COMM' , @pCODE ='Z005', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'", 0);
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "제조오더단위")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE='COMM', @pCODE ='Z005', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'", 1);
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "구매오더단위")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE='COMM', @pCODE ='Z005', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'", 1);
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "최종검사")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'Q013', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'", 0);  

            //원가정보 추가
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "양산구분")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE='COMM',  @pCODE ='B060', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'", 1);
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "통합원가부품구분(계정)")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE='COMM', @pCODE ='B061', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'", 1);
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "방산물자지정여부")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE='COMM' , @pCODE ='B029', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'", 0);
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "구매구분")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE='COMM' , @pCODE ='B062', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'", 1);
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "시효구분")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE='COMM', @pCODE ='B029', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'", 0);
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "ESD구분")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE='COMM', @pCODE ='B029', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'", 0);
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "MSL구분")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'B029', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'", 0);
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "규격화구분")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE='COMM' , @pCODE ='B063', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'", 1);
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "국방도면종류")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE='COMM', @pCODE ='B064', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'", 1);
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "중량단위")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE='COMM', @pCODE ='Z005', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'", 1);
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "부피단위")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'Z005', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'", 1);
			
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
        }
        #endregion
		
        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = Cursors.WaitCursor;
			string rdoCfm;

			if (rdoCfmY.Checked)
				rdoCfm = "B";
			else
				rdoCfm = "A";

			if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                string strQuery = " usp_IF_INF010  'S1'";
                strQuery = strQuery + ", @pCO_CD ='" + SystemBase.Base.gstrCOMCD.ToString() + "' ";
				strQuery = strQuery + ", @pPLANT_CD ='" + SystemBase.Base.gstrPLANT_CD.ToString() + "' ";
				strQuery = strQuery + ", @pGUBUN ='" + rdoCfm + "' ";

				UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0);

                //if (fpSpread1.Sheets[0].RowCount > 0)
                //{
                //    for (int i = 0; i < fpSpread1.Sheets[0].RowCount; i++)
                //    {
                //        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "조달구분")].Text != "" && fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "조달구분")].Value.ToString() == "M")
                //            UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "제조오더단위") + "|1#" +
                //                                                  SystemBase.Base.GridHeadIndex(GHIdx1, "제조오더L/T") + "|1#" +
                //                                                  SystemBase.Base.GridHeadIndex(GHIdx1, "구매오더단위") + "|0#" +
                //                                                  SystemBase.Base.GridHeadIndex(GHIdx1, "구매L/T") + "|0#" +
                //                                                  SystemBase.Base.GridHeadIndex(GHIdx1, "구매조직") + "|0");
                //        else
                //            UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "제조오더단위") + "|0#" +
                //                                                  SystemBase.Base.GridHeadIndex(GHIdx1, "제조오더L/T") + "|0#" +
                //                                                  SystemBase.Base.GridHeadIndex(GHIdx1, "구매오더단위") + "|1#" +
                //                                                  SystemBase.Base.GridHeadIndex(GHIdx1, "구매L/T") + "|1#" +
                //                                                  SystemBase.Base.GridHeadIndex(GHIdx1, "구매조직") + "|1");

                //        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot Size")].Text != "" && fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot Size")].Value.ToString() == "P")
                //            UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "올림기간") + "|1");
                //        else
                //            UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "올림기간") + "|2");

                //        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목구분")].Text != "" && fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목구분")].Value.ToString() == "99")
                //            UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "도면번호") + "|1#" +
                //                                                  SystemBase.Base.GridHeadIndex(GHIdx1, "도면REV") + "|1"); //품목구분
                //        else
                //            UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "도면번호") + "|0#" +
                //                                                  SystemBase.Base.GridHeadIndex(GHIdx1, "도면REV") + "|0");
                //    }
                //}
            }
			if (rdoCfmN.Checked ==true)
			{
				for (int i = 0; i < fpSpread1.ActiveSheet.Rows.Count; i++)
					UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "선택") + "|3");
			}
            this.Cursor = Cursors.Default;
        }
        #endregion

        #region SaveExec() 폼에 입력된 데이타 저장 로직
        protected override void SaveExec()
        {
            this.Cursor = Cursors.WaitCursor;

            if ((SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", true) == true))// 그리드 필수항목 체크 
            {
                string ERRCode = "ER", MSGCode = "SY001";	//처리할 내용이 없습니다.
              
                    //행수만큼 처리
                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {
						SqlConnection dbConn = SystemBase.DbOpen.DBCON();
						SqlCommand cmd = dbConn.CreateCommand();
						SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

						string strHead = fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text;
                        string strGbn = "I1";
						string check = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "선택")].Value.ToString();
						string strIfId = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Interface ID")].Value.ToString();
						if (check == "True")
						{
							if (strHead.Length > 0)
							{
								string strSql = " usp_IF_INF010 '" + strGbn + "'";
								strSql = strSql + ", @pCO_CD  = '" + SystemBase.Base.gstrCOMCD + "'";
								strSql = strSql + ", @pPLANT_CD  = '" + SystemBase.Base.gstrPLANT_CD + "'";
								strSql = strSql + ", @pIF_ID  = '" + strIfId + "'";
								
								DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
								ERRCode = ds.Tables[0].Rows[0][0].ToString();
								MSGCode = ds.Tables[0].Rows[0][1].ToString();

								if (ERRCode != "OK") { Trans.Rollback(); continue; }
								else Trans.Commit();
							}
						}
						dbConn.Close();
					}

                MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
			NewExec();
			SearchExec();
			this.Cursor = Cursors.Default;
        }
		#endregion

		#region 체크선택시 수정플레그 변경
		private void ChangeChkBox(int Col, int Row)
		{
			try
			{
				if (Col == SystemBase.Base.GridHeadIndex(GHIdx1, "선택")) // 배포 버튼을 클릭했을 경우
				{
					if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "선택")].Text != "True")
					{
						fpSpread1.Sheets[0].RowHeader.Cells[Row, 0].Text = "U";
					}
					else
					{
						fpSpread1.Sheets[0].RowHeader.Cells[Row, 0].Text = "";
					}

					if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "선택")].Text != "True")
					{
						fpSpread1.Sheets[0].RowHeader.Cells[Row, 0].Text = "";
					}
					else
					{
						fpSpread1.Sheets[0].RowHeader.Cells[Row, 0].Text = "U";
					}
				}
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "수정플래그등록"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		#endregion

		#region 전체선택클릭시
		private void fpSpread1_CellClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
		{
			if (fpSpread1.Sheets[0].Rows.Count > 0)
			{
				if (fpSpread1.Sheets[0].ColumnHeader.Cells.Get(0, e.Column).CellType != null)
				{
					if (e.ColumnHeader == true)
					{
						if (fpSpread1.Sheets[0].ColumnHeader.Cells[0, e.Column].Text == "True")
						{
							fpSpread1.Sheets[0].ColumnHeader.Cells.Get(0, e.Column).Value = true;
							for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
							{
								if (fpSpread1.Sheets[0].Cells[i, e.Column].Locked == false)
								{
									fpSpread1.Sheets[0].Cells[i, e.Column].Value = true;
									ChangeChkBox(e.Column, i);
								}
							}
						}
						else
						{
							fpSpread1.Sheets[0].ColumnHeader.Cells.Get(0, e.Column).Value = false;
							for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
							{
								if (fpSpread1.Sheets[0].Cells[i, e.Column].Locked == false)
								{
									fpSpread1.Sheets[0].Cells[i, e.Column].Value = false;
									ChangeChkBox(e.Column, i);
								}
							}
						}
					}
				}
			}
		}
		#endregion

		#region 그리드상 체크박스 선택시
		private void fpSpread1_ButtonClicked(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
		{
			ChangeChkBox(e.Column, e.Row);
		}
		#endregion

		#region 확정전/확정후 버튼

		private void rdoCfmY_CheckedChanged(object sender, EventArgs e)
		{
			NewExec();
		}

		private void rdoCfmN_CheckedChanged(object sender, EventArgs e)
		{
			NewExec();
		}

		#endregion
	}
}
