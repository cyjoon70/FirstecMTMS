#region 작성정보
/*********************************************************************/
// 단위업무명 : 개인별 작업 시간 집계표
// 작 성 자 : 조 홍 태
// 작 성 일 : 2013-08-21
// 작성내용 : 개인별 작업 시간 집계표
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
using System.Globalization;
using System.Threading;
using WNDW;

namespace CW.CWB015
{
    public partial class CWB015 : UIForm.FPCOMM1
    {
        #region 변수선언
        UIForm.ExcelWaiting Waiting_Form = null;
        Thread th;
        #endregion

        public CWB015()
        {
            InitializeComponent();
        }

        #region Form Load 시
        private void CWB015_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);
	
            //그리드 초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

            //기타 세팅		
            dtpWorkDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddYears(-1).ToString().Substring(0,7);

            txtPlantCd.Text = SystemBase.Base.gstrPLANT_CD;
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            //그리드 초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

            //기타 세팅		
            dtpWorkDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddYears(-1).ToString().Substring(0, 7);

            txtPlantCd.Text = SystemBase.Base.gstrPLANT_CD;
        }
        #endregion

        #region 조회 조건 팝업
        //공장 코드
        private void btnPlantCd_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = "usp_B_COMMON @pTYPE = 'TABLE_POP' ,@pSPEC1 = 'PLANT_CD', @pSPEC2 = 'PLANT_NM', @pSPEC3 = 'B_PLANT_INFO', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtPlantCd.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00005", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "공장코드 조회");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtPlantCd.Text = Msgs[0].ToString();
                    txtPlantNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "공장코드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);	//데이터 조회 중 오류가 발생하였습니다.

            }
        }

        //프로젝트
        private void btnProjectNo_Click(object sender, EventArgs e)
        {
            try
            {
                WNDW007 pu = new WNDW007(txtProjectNo.Text, "N");
                pu.MaximizeBox = false;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtProjectNo.Value = Msgs[3].ToString();
                    txtProjectNm.Value = Msgs[4].ToString();
                    txtProjectNo.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "프로젝트번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);	//데이터 조회 중 오류가 발생하였습니다.

            }
        }

        //납품품목
        private void btnJ_ITEM_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW005 pu = new WNDW005();
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtJ_ITEM_CD.Text = Msgs[2].ToString();
                    txtJ_ITEM_NM.Value = Msgs[3].ToString();
                    txtJ_ITEM_CD.Focus();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //부품
        private void btnITEM_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW005 pu = new WNDW005();
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtITEM_CD.Text = Msgs[2].ToString();
                    txtITEM_NM.Value = Msgs[3].ToString();
                    txtITEM_CD.Focus();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region TextChanged
        //공장코드
        private void txtPlantCd_TextChanged(object sender, EventArgs e)
        {
            txtPlantNm.Value = SystemBase.Base.CodeName("PLANT_CD", "PLANT_NM", "B_PLANT_INFO", txtPlantCd.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");
        }
        //프로젝트
        private void txtProjectNo_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (txtProjectNo.Text != "")
                {
                    txtProjectNm.Value = SystemBase.Base.CodeName("PROJECT_NO", "PROJECT_NM", "S_SO_MASTER", txtProjectNo.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");
                }
                else
                {
                    txtProjectNm.Value = "";
                }
            }
            catch
            {

            }
        }

        // 납품품목
        private void txtJ_ITEM_CD_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtJ_ITEM_CD.Text != "")
                {
                    txtJ_ITEM_NM.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtJ_ITEM_CD.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtJ_ITEM_NM.Value = "";
                }
            }
            catch
            {

            }
        }
        
        // 부품
        private void txtITEM_CD_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtITEM_CD.Text != "")
                {
                    txtITEM_NM.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtITEM_CD.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtITEM_NM.Value = "";
                }
            }
            catch
            {

            }
        }
        #endregion

        #region 조회조건 TO 날짜 고정
        private void dtpWorkDtFr_ValueChanged(object sender, EventArgs e)
        {
            dtpWorkDtTo.Value = Convert.ToDateTime(dtpWorkDtFr.Value.ToString() + "-01").AddYears(1).AddMonths(-1).ToString().Substring(0,7);
        }
        #endregion
        
        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {
                    string strDtFr = dtpWorkDtFr.Text + "-01";
                    string strDtTo = Convert.ToDateTime(dtpWorkDtTo.Text + "-01").AddMonths(1).AddDays(-1).ToShortDateString();

                    string strQuery = "usp_CWB015 @pTYPE = 'S1'";
                    strQuery += ", @pWORK_DT_FR = '" + strDtFr + "'";
                    strQuery += ", @pWORK_DT_TO = '" + strDtTo + "'";
                    strQuery += ", @pPLANT_CD = '" + txtPlantCd.Text + "'";
                    strQuery += ", @pPROJECT_NO = '" + txtProjectNo.Text + "'";
                    strQuery += ", @pMAKE_ITEM_CD = '" + txtJ_ITEM_CD.Text + "'";
                    strQuery += ", @pITEM_CD = '" + txtITEM_CD.Text + "'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0, true);

                    //조회 조건에 맞게 Head명 바꾸기
                    for (int i = SystemBase.Base.GridHeadIndex(GHIdx1, "작업자명") + 1; i < fpSpread1.Sheets[0].ColumnCount - 1; i++)
                    {
                        string strWorkDtFr = Convert.ToDateTime(dtpWorkDtFr.Value.ToString() + "-01").AddMonths(i - SystemBase.Base.GridHeadIndex(GHIdx1, "작업자명") - 1).ToString();
                        fpSpread1.Sheets[0].ColumnHeader.Cells[0, i].Text = strWorkDtFr.Substring(2, 5).Replace("-", ".");
                    }

                    fpSpread1.Sheets[0].SetColumnMerge(SystemBase.Base.GridHeadIndex(GHIdx1, "계약품목명"), FarPoint.Win.Spread.Model.MergePolicy.Always);
                    fpSpread1.Sheets[0].SetColumnMerge(SystemBase.Base.GridHeadIndex(GHIdx1, "부품번호"), FarPoint.Win.Spread.Model.MergePolicy.Always);
                    fpSpread1.Sheets[0].SetColumnMerge(SystemBase.Base.GridHeadIndex(GHIdx1, "품명"), FarPoint.Win.Spread.Model.MergePolicy.Always);
                    //fpSpread1.Sheets[0].SetColumnMerge(SystemBase.Base.GridHeadIndex(GHIdx1, "공정명"), FarPoint.Win.Spread.Model.MergePolicy.Always);
                    fpSpread1.Sheets[0].SetColumnMerge(SystemBase.Base.GridHeadIndex(GHIdx1, "납품품목"), FarPoint.Win.Spread.Model.MergePolicy.Restricted);
                    fpSpread1.Sheets[0].SetColumnMerge(SystemBase.Base.GridHeadIndex(GHIdx1, "납품품목명"), FarPoint.Win.Spread.Model.MergePolicy.Restricted);

                    int project_nm = SystemBase.Base.GridHeadIndex(GHIdx1, "계약품목명");
                    int work_item_nm = SystemBase.Base.GridHeadIndex(GHIdx1, "부품번호");
                    int make_item_nm = SystemBase.Base.GridHeadIndex(GHIdx1, "납품품목");

                    for (int j = 0; j < fpSpread1.Sheets[0].RowCount; j++)
                    {
                        if (fpSpread1.Sheets[0].Cells[j, project_nm].Text == "합계")
                        {
                            fpSpread1.Sheets[0].Cells[j, 2].ColumnSpan = 10;
                            fpSpread1.Sheets[0].Cells[j, 1, j, fpSpread1.Sheets[0].ColumnCount - 1].BackColor = SystemBase.Base.gColor1;
                        }
                        if (fpSpread1.Sheets[0].Cells[j, work_item_nm].Text.Contains("소계") == true)
                        {
                            fpSpread1.Sheets[0].Cells[j, 5].ColumnSpan = 8;
                            fpSpread1.Sheets[0].Cells[j, 1, j, fpSpread1.Sheets[0].ColumnCount - 1].BackColor = SystemBase.Base.gColor2;
                        }
                        if (fpSpread1.Sheets[0].Cells[j, make_item_nm].Text.Contains("소계") == true)
                        {
                            fpSpread1.Sheets[0].Cells[j, 3].ColumnSpan = 10;
                            fpSpread1.Sheets[0].Cells[j, 1, j, fpSpread1.Sheets[0].ColumnCount - 1].BackColor = SystemBase.Base.gColor3;
                        }
                    }

                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이타 조회 중 오류가 발생하였습니다.
            }

            this.Cursor = Cursors.Default;
        }

        #endregion
        
    }
}
