#region 작성정보
/*********************************************************************/
// 단위업무명 : 발주집계조회
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-04-17
// 작성내용 : 발주집계조회 관리
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
using WNDW;
using FarPoint.Win.Spread.CellType;

namespace MP.MPO502
{
    public partial class MPO502 : UIForm.FPCOMM1
    {
        #region 생성자
        public MPO502()
        {
            InitializeComponent();
        }
        #endregion 

        #region Form Load 시
        private void MPO502_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            SystemBase.ComboMake.C1Combo(cboItemDiv, "usp_M_COMMON @pTYPE = 'M031'   , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");

            //콤보박스 세팅
            SystemBase.ComboMake.C1Combo(cboPriceFlag, "usp_B_COMMON @pType='COMM', @pCODE = 'S011', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 9); //단가구분

            //그리드 초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

            //기타 세팅	
            txtPlantCd.Value = SystemBase.Base.gstrPLANT_CD;
            dtpPoDtFr.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToString().Substring(0,10);
            dtpPoDtTo.Value = SystemBase.Base.ServerTime("YYMMDD").Substring(0,10);

            SystemBase.Validation.GroupBoxControlsLock(groupBox2, true);
        }
        #endregion
        
        #region NewExec()
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            SystemBase.Validation.GroupBox_Reset(groupBox2);

            //그리드 초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

            //기타 세팅	
            txtPlantCd.Value = SystemBase.Base.gstrPLANT_CD;
            dtpPoDtFr.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToString().Substring(0,10);
            dtpPoDtTo.Value = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0,10);
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

        //구매담당자
        private void butPurDuty_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_M_COMMON 'M011' ,@pSPEC1='" + SystemBase.Base.gstrBIZCD + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtPurDuty.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00031", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "구매담당자 팝업");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {

                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtPurDuty.Value = Msgs[0].ToString();
                    txtPurDutyNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "구매담당자 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //공급처
        private void btnCustCd_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW002 pu = new WNDW002(txtCustCd.Text, "P");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtCustCd.Value = Msgs[1].ToString();
                    txtCustNm.Value = Msgs[2].ToString();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "공급처 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        //품목
        private void btnItemCd_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW005 pu = new WNDW005(txtPlantCd.Text, true, txtItemCd.Text);
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtItemCd.Value = Msgs[2].ToString();
                    txtItemNm.Value = Msgs[3].ToString();
                    txtItemCd.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);	//데이터 조회 중 오류가 발생하였습니다.

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

        //발주형태
        private void btnPoType_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = "usp_B_COMMON @pTYPE = 'TABLE_POP', @pSPEC1='PO_TYPE_CD', @pSPEC2 = 'PO_TYPE_NM', @pSPEC3 = 'M_PO_TYPE', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtPoType.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P01005", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "발주형태 조회");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtPoType.Value = Msgs[0].ToString();
                    txtPoTypeNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "발주형태 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);	//데이터 조회 중 오류가 발생하였습니다.

            }
        }

        //발주번호
        private void btnPoNo_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW.WNDW018 pu = new WNDW.WNDW018();
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtPoNo.Value = Msgs[1].ToString();
                    txtPoNo.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "구매발주정보 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //구매요청번호
        private void btnReqNo_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = "usp_MRQ499 @pTYPE = 'P1', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "" };
                string[] strSearch = new string[] { txtReqNo.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00085", strQuery, strWhere, strSearch, new int[] { 0 }, "구매요청번호 조회");
                pu.Width = 600;
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtReqNo.Value = Msgs[0].ToString();

                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "구매요청번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);	//데이터 조회 중 오류가 발생하였습니다.

            }
        }

        private void btnItemType_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON  @pTYPE = 'COMM_POP' ,@pSPEC1='P032', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtItemType.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00077", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "품목구분 팝업");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {

                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtItemType.Value = Msgs[0].ToString();
                    txtItemTypeNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "구매담당자 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        //구매담당자
        private void txtPurDuty_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtPurDuty.Text != "")
                {
                    txtPurDutyNm.Value = SystemBase.Base.CodeName("USR_ID", "USR_NM", "B_SYS_USER", txtPurDuty.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtPurDutyNm.Value = "";
                }
            }
            catch
            {

            }
        }

        //공급처
        private void txtCustCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtCustCd.Text != "")
                {
                    txtCustNm.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtCustCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtCustNm.Value = "";
                }
            }
            catch
            {

            }
        }

        //품목
        private void txtItemCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtItemCd.Text != "")
                {
                    txtItemNm.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtItemCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtItemNm.Value = "";
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

        //발주형태
        private void txtPoType_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtPoType.Text != "")
                {
                    txtPoTypeNm.Value = SystemBase.Base.CodeName("PO_TYPE_CD", "PO_TYPE_NM", "M_PO_TYPE", txtPoType.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtPoTypeNm.Value = "";
                }
            }
            catch
            {

            }
        }

        private void txtItemType_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtItemType.Text != "")
                {
                    txtItemTypeNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtItemType.Text, " AND MAJOR_CD = 'P032' AND COMP_CODE = '" + SystemBase.Base.gstrCOMCD + "' ");
                }
                else
                {
                    txtItemTypeNm.Value = "";
                }
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
                string strCloseYn = "";

                try
                {
                    if (rdoCloseYes.Checked == true) { strCloseYn = "Y"; }
                    else if (rdoCloseNo.Checked == true) { strCloseYn = "N"; }

                    string strQuery = "usp_MPO502 @pTYPE = 'S1'";
                    strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'";
                    strQuery += ", @pCLOSE_YN = '" + strCloseYn + "'";
                    strQuery += ", @pITEM_DIV = '" + cboItemDiv.SelectedValue + "' ";
                    strQuery += ", @pPLANT_CD = '" + txtPlantCd.Text + "'";
                    strQuery += ", @pPUR_DUTY = '" + txtPurDuty.Text + "'";
                    strQuery += ", @pCUST_CD = '" + txtCustCd.Text + "'";
                    strQuery += ", @pPO_DT_FR = '" + dtpPoDtFr.Text + "'";
                    strQuery += ", @pPO_DT_TO = '" + dtpPoDtTo.Text + "'";
                    strQuery += ", @pITEM_CD = '" + txtItemCd.Text + "'";
                    strQuery += ", @pPROJECT_NO = '" + txtProjectNo.Text + "'";
                    strQuery += ", @pPROJECT_SEQ = '" + txtProjectSeq.Text + "'";
                    strQuery += ", @pPO_TYPE = '" + txtPoType.Text + "'";
                    strQuery += ", @pPRICE_FLAG = '" + cboPriceFlag.SelectedValue.ToString() + "'";
                    strQuery += ", @pPO_NO =   '" + txtPoNo.Text + "'";
                    strQuery += ", @pREQ_NO = '" + txtReqNo.Text + "'";
                    strQuery += ", @pITEM_TYPE = '" + txtItemType.Text + "'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0, true);

                    if (fpSpread1.Sheets[0].RowCount > 0)
                    {
                        Grup_Set();
                        Set_Section();
                    }
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

        #region gorupBox2 합계금액
        private void Grup_Set()
        {
            //발주금액,	선적금액, 통관금액,	입고금액, 매입금액,	합격금액, 불량금액,	결재금액
            double dTotPoAmt = 0, dTotBlAmt = 0, dTotCcAmt = 0, dTotTempMvmtAmt = 0, dTotIvAmt = 0, dTotInspGoodAmt = 0, dTotInspDefectAmt = 0, dTotApprovalAmt = 0;

            for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
            {
                if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "공급처명")].Text != "")
                {
                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "발주금액")].Text != "")
                    {
                        dTotPoAmt += Convert.ToDouble(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "발주금액")].Value);
                    }

                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "선적금액")].Text != "")
                    {
                        dTotBlAmt += Convert.ToDouble(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "선적금액")].Value);
                    }

                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "통관금액")].Text != "")
                    {
                        dTotCcAmt += Convert.ToDouble(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "통관금액")].Value);
                    }

                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "입고금액")].Text != "")
                    {
                        dTotTempMvmtAmt += Convert.ToDouble(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "입고금액")].Value);
                    }

                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "매입금액")].Text != "")
                    {
                        dTotIvAmt += Convert.ToDouble(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "매입금액")].Value);
                    }

                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "합격금액")].Text != "")
                    {
                        dTotInspGoodAmt += Convert.ToDouble(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "합격금액")].Value);
                    }

                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "불량금액")].Text != "")
                    {
                        dTotInspDefectAmt += Convert.ToDouble(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "불량금액")].Value);
                    }

                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "결재금액")].Text != "")
                    {
                        dTotApprovalAmt += Convert.ToDouble(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "결재금액")].Value);
                    }
                }
            }

            txtTotPoAmt.Value = dTotPoAmt;
            txtTotBlAmt.Value = dTotBlAmt;
            txtTotCcAmt.Value = dTotCcAmt;
            txtTotTempMvmtAmt.Value = dTotTempMvmtAmt;
            txtTotIvAmt.Value = dTotIvAmt;
            txtTotInspGoodAmt.Value = dTotInspGoodAmt;
            txtTotInspDefectAmt.Value = dTotInspDefectAmt;
            txtTotApprovalAmt.Value = dTotApprovalAmt;
        }
        #endregion

        #region 소계 합계 그리드 재정의
        private void Set_Section()
        {
            int iCnt = fpSpread1.Sheets[0].RowCount;
            int iRow = 0;

            //소계, 합계 컬럼 합치고 색 변경
            for (int i = 0; i < iCnt; i++)
            {

                if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "공급처명")].Text == "")
                {
                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "공급처")].ColumnSpan = 10;
                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "공급처")].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;

                    for (int j = 1; j < fpSpread1.Sheets[0].ColumnCount; j++)
                    {
                        //소계 합계 색 변경
                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "공급처")].Text == "합계")
                            fpSpread1.Sheets[0].Cells[i, j].BackColor = SystemBase.Base.gColor1;
                        else fpSpread1.Sheets[0].Cells[i, j].BackColor = SystemBase.Base.gColor2;

                    }
                    fpSpread1.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx1, "공급처")].RowSpan = i - iRow;
                    fpSpread1.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx1, "공급처명")].RowSpan = i - iRow;

                    iRow = i + 1;
                }

            }

        }
        #endregion
        
    }
}
