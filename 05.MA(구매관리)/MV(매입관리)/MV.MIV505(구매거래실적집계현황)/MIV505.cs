#region 작성정보
/*********************************************************************/
// 단위업무명 : 구매거래명세서조회
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-04-18
// 작성내용 : 구매거래명세서조회 관리
// 수 정 일 :
// 수 정 자 :
// 수정내용 :
// 비    고 :
/*********************************************************************/
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using WNDW;

namespace MV.MIV505 
{
    public partial class MIV505 : UIForm.FPCOMM2_2T
    {
        #region 생성자 
        public MIV505()
        {
            InitializeComponent();
        }
        #endregion
        
        #region Form Load 시
        private void MIV505_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            //그리드 초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
            UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, 0, 0, false);

            //기타 세팅	
            txtPlantCd.Value = SystemBase.Base.gstrPLANT_CD;
            dtpIvDtFr.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToString().Substring(0,10);
            dtpIvDtTo.Value = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0,10);

            c1DockingTab1.SelectedIndex = 0;
        }
        #endregion

        #region NewExec()
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            //그리드 초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
            UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, 0, 0, false);

            //기타 세팅	
            txtPlantCd.Value = SystemBase.Base.gstrPLANT_CD;
            dtpIvDtFr.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToString().Substring(0,10);
            dtpIvDtTo.Value = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0,10);

            c1DockingTab1.SelectedIndex = 0;
        }
        #endregion

        #region 조회조건 팝업
        //공장
        private void btnPlantCd_Click(object sender, System.EventArgs e)
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

                    txtPlantCd.Value = Msgs[0].ToString();
                    txtPlantNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "공장코드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);	//데이터 조회 중 오류가 발생하였습니다.

            }
        }

        //지급처
        private void btnPaymentCust_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW002 pu = new WNDW002(txtPaymentCust.Text, "");
                pu.MaximizeBox = false;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtPaymentCust.Value = Msgs[1].ToString();
                    txtPaymentCustNm.Value = Msgs[2].ToString();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "지급처 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        //매입번호
        private void btnIvNo_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW.WNDW024 pu = new WNDW.WNDW024();
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtIvNo.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "매입정보 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //매입형태
        private void btnIvType_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = "usp_B_COMMON @pTYPE = 'TABLE_POP', @pSPEC1 = 'IV_TYPE', @pSPEC2 = 'IV_TYPE_NM', @pSPEC3 = 'M_IV_TYPE', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtIvType.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00006", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "매입형태 조회");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtIvType.Value = Msgs[0].ToString();
                    txtIvTypeNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "매입형태 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);	//데이터 조회 중 오류가 발생하였습니다.

            }
        }

        //프로젝트번호
        private void btnProjectNo_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW007 pu = new WNDW007(txtProjectNo.Text);
                pu.MaximizeBox = false;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtProjectNo.Value = Msgs[3].ToString();
                    txtProjectNm.Value = Msgs[4].ToString();
                    txtProjectSeq.Value = "";
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "프로젝트번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //프로젝트차수
        private void btnProjectSeq_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON 'PROJ_SEQ', @pSPEC1 = '" + txtProjectNo.Text + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";										// 쿼리
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };			// 쿼리 인자값(조회조건)
                string[] strSearch = new string[] { "", "" };		// 쿼리 인자값에 들어갈 데이타

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P09001", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "프로젝트차수 조회", false);
                pu.Width = 400;
                pu.ShowDialog();	//공통 팝업 호출

                if (pu.DialogResult == DialogResult.OK)
                {
                    string MSG = pu.ReturnVal.Replace("|", "#");
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(MSG);
                    txtProjectSeq.Value = Msgs[0].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }
        #endregion

        #region 조회조건 TextChanged
        //공장
        private void txtPlantCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtPlantCd.Text != "")
                {
                    txtPlantNm.Value = SystemBase.Base.CodeName("PLANT_CD", "PLANT_NM", "B_PLANT_INFO", txtPlantCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtPlantNm.Value = "";
                }
            }
            catch
            {

            }
        }

        //지급처
        private void txtPaymentCust_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtPaymentCust.Text != "")
                {
                    txtPaymentCustNm.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtPaymentCust.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtPaymentCustNm.Value = "";
                }
            }
            catch
            {

            }
        }

        //매입형태
        private void txtIvType_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtIvType.Text != "")
                {
                    txtIvTypeNm.Value = SystemBase.Base.CodeName("IV_TYPE", "IV_TYPE_NM", "M_IV_TYPE", txtIvType.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtIvTypeNm.Value = "";
                }
            }
            catch
            {

            }
        }

        //프로젝트번호
        private void txtProjectNo_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtProjectNo.Text != "")
                {
                    txtProjectNm.Value = SystemBase.Base.CodeName("PROJECT_NO", "PROJECT_NM", "S_SO_MASTER", txtProjectNo.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtProjectNm.Value = "";
                }
                if (txtProjectNm.Text == "")
                    txtProjectSeq.Value = "";
            }
            catch
            {

            }
        }
        #endregion

        #region SearchExec()
        protected override void SearchExec()
        {
            //조회조건 필수 체크
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                this.Cursor = Cursors.WaitCursor;

                c1DockingTab1.SelectedIndex = 0;

                try
                {
                    string strQuery1 = "usp_MIV505 @pTYPE = 'S1'";
                    strQuery1 += ", @pPLANT_CD = '" + txtPlantCd.Text + "'";
                    strQuery1 += ", @pIV_DT_FR = '" + dtpIvDtFr.Text + "'";
                    strQuery1 += ", @pIV_DT_TO = '" + dtpIvDtTo.Text + "'";
                    strQuery1 += ", @pIV_TYPE = '" + txtIvType.Text + "'";
                    strQuery1 += ", @pPAYMENT_CUST = '" + txtPaymentCust.Text + "'";
                    strQuery1 += ", @pIV_NO = '" + txtIvNo.Text + "'";
                    strQuery1 += ", @pPROJECT_NO = '" + txtProjectNo.Text + "'";
                    strQuery1 += ", @pPROJECT_SEQ = '" + txtProjectSeq.Text + "'";
                    strQuery1 += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'";
                    strQuery1 += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    string strQuery2 = "usp_MIV505 @pTYPE = 'S2'";
                    strQuery2 += ", @pPLANT_CD = '" + txtPlantCd.Text + "'";
                    strQuery2 += ", @pIV_DT_FR = '" + dtpIvDtFr.Text + "'";
                    strQuery2 += ", @pIV_DT_TO = '" + dtpIvDtTo.Text + "'";
                    strQuery2 += ", @pIV_TYPE = '" + txtIvType.Text + "'";
                    strQuery2 += ", @pPAYMENT_CUST = '" + txtPaymentCust.Text + "'";
                    strQuery2 += ", @pIV_NO = '" + txtIvNo.Text + "'";
                    strQuery2 += ", @pPROJECT_NO = '" + txtProjectNo.Text + "'";
                    strQuery2 += ", @pPROJECT_SEQ = '" + txtProjectSeq.Text + "'";
                    strQuery2 += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'";
                    strQuery2 += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery1, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0, true);
                    UIForm.FPMake.grdCommSheet(fpSpread2, strQuery2, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, false, 0, 0, true);

                    if (fpSpread1.Sheets[0].Rows.Count > 0)
                        Set_Section(fpSpread1, 10);
                    if (fpSpread2.Sheets[0].Rows.Count > 0)
                        Set_Section(fpSpread2, 13);
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

        #region 소계 합계 그리드 재정의
        private void Set_Section(FarPoint.Win.Spread.FpSpread baseGrid, int iColumn)
        {
            int iCnt = baseGrid.Sheets[0].RowCount;

            //소계, 합계 컬럼 합치고 색 변경
            for (int i = 0; i < iCnt; i++)
            {

                if (baseGrid.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "지급처명")].Text == "")
                {
                    baseGrid.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "지급처")].ColumnSpan = iColumn;
                    baseGrid.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "지급처")].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;

                    for (int j = 1; j < baseGrid.Sheets[0].ColumnCount; j++)
                    {
                        if (baseGrid.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "지급처")].Text == "합계")
                        {
                            baseGrid.Sheets[0].Cells[i, j].BackColor = SystemBase.Base.gColor1;
                        }
                        else
                            baseGrid.Sheets[0].Cells[i, j].BackColor = SystemBase.Base.gColor2;
                    }
                }
            }
        }
        #endregion
        
    }
}
