#region 작성정보
/*********************************************************************/
// 단위업무명 : 외주공정의뢰등록/출력
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-02-14
// 작성내용 : 외주공정의뢰등록/출력 및 관리
// 수 정 일 :
// 수 정 자 :
// 수정내용 :
// 비    고 :
/*********************************************************************/
#endregion

using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Globalization;
using System.Text.RegularExpressions;
using WNDW;

namespace IF.INF030
{
    public partial class INF030 : UIForm.FPCOMM2
    {
        string strItemCd = ""; //품목코드

        public INF030()
        {
            InitializeComponent();
        }

        #region Form Load 시
        private void INF030_Load(object sender, System.EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(groupBox1);//필수체크

            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "VAT유형")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'B040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//VAT유형
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "통화")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'Z003', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//화폐단위

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
            UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, 0, 0, false);

			strItemCd = "";

        }
		#endregion

		#region NewExec() New 버튼 클릭 이벤트
		protected override void NewExec()
		{
			SystemBase.Validation.GroupBox_Reset(groupBox1);
			fpSpread1.Sheets[0].Rows.Count = 0;
			fpSpread2.Sheets[0].Rows.Count = 0;
		}
		#endregion

		#region SearchExec() Master 그리드 조회 로직
		protected override void SearchExec()
        {
            Search("", true);
        }

        private void Search(string strReqNo, bool Msg)
        {
			this.Cursor = Cursors.WaitCursor;
			string rdoCfm;

			if (rdoCfmY.Checked)
				rdoCfm = "B";
			else rdoCfm = "A";

			if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
			{
				string strQuery = " usp_IF_INF030  'S1'";
				strQuery = strQuery + ", @pCO_CD ='" + SystemBase.Base.gstrCOMCD.ToString() + "' ";
				strQuery = strQuery + ", @pPLANT_CD ='" + SystemBase.Base.gstrPLANT_CD.ToString() + "' ";
				strQuery = strQuery + ", @pGUBUN ='" + rdoCfm + "' ";

				UIForm.FPMake.grdCommSheet(fpSpread2, strQuery, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, true, 0, 0);

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

			this.Cursor = Cursors.Default;
		}
        #endregion

        #region Master그리드 선택시 상세정보 조회
        private void fpSpread2_SelectionChanged(object sender, FarPoint.Win.Spread.SelectionChangedEventArgs e)
        {
            if (fpSpread2.Sheets[0].Rows.Count > 0)
            {
                try
                {
                    int intRow = fpSpread2.ActiveSheet.GetSelection(0).Row;
                    strItemCd = fpSpread2.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx2, "품목코드")].Text.ToString();

                    SubSearch(strItemCd);
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //데이터 조회 중 오류가 발생하였습니다.		
                }
            }
        }
        #endregion

        #region 상세정보 조회
        private void SubSearch(string strCode)
        {
            this.Cursor = Cursors.WaitCursor;
			try
			{
				fpSpread1.Sheets[0].Rows.Count = 0;

				//Detail그리드 정보.
				string strSql1 = " usp_IF_INF030  'S2' ";
				strSql1 = strSql1 + ", @pCO_CD ='" + SystemBase.Base.gstrCOMCD.ToString() + "' ";
				strSql1 = strSql1 + ", @pPLANT_CD ='" + SystemBase.Base.gstrPLANT_CD.ToString() + "' ";
				strSql1 = strSql1 + ", @pITEM_CD ='" + strCode + "' ";

				UIForm.FPMake.grdCommSheet(fpSpread1, strSql1, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
				//데이터 조회 중 오류가 발생하였습니다.
			}
            this.Cursor = Cursors.Default;
        }
        #endregion

    }
}