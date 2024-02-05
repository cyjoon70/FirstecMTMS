#region 작성정보
/*********************************************************************/
// 단위업무명 : 수주진행별조회
// 작 성 자 : 김 현 근
// 작 성 일 : 2013-04-11
// 작성내용 : 수주진행별조회
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
namespace SO.SOB005
{
    public partial class SOB005 : UIForm.FPCOMM1
    {
        #region 생성자
        public SOB005()
        {
            InitializeComponent();

        }
        #endregion

        #region Form Load 시
        private void SOB005_Load(object sender, System.EventArgs e)
        {
            //GroupBox1 초기화
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            SystemBase.ComboMake.C1Combo(cboContractType, "usp_B_COMMON @pType='COMM', @pCODE = 'S014', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 3);//계약구분

            //그리드 세팅
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0);

            //기타 세팅	
            dtpDeliveryDtFr.Text = SystemBase.Base.ServerTime("YYMMDD");
            dtpDeliveryDtTo.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(1).ToShortDateString();
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            //그리드 세팅
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0);

            //기타 세팅	
            dtpDeliveryDtFr.Text = SystemBase.Base.ServerTime("YYMMDD");
            dtpDeliveryDtTo.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(1).ToShortDateString();

            rdoAll.Checked = true;
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            //조회조건 필수 체크
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1) == true)
            {
                string strCfmYn = "";

                try
                {
                    string strQuery = "usp_SOB005 @pTYPE = 'S1'";
                    strQuery += ", @pDELIVERY_DT_FR = '" + dtpDeliveryDtFr.Text + "'";
                    strQuery += ", @pDELIVERY_DT_TO = '" + dtpDeliveryDtTo.Text + "'";
                    strQuery += ", @pREF_DELIVERY_DT_FR = '" + dtpRefDelvDtFr.Text + "'";
                    strQuery += ", @pREF_DELIVERY_DT_TO = '" + dtpRefDelvDtTo.Text + "'";
                    strQuery += ", @pPROJECT_NO = '" + txtProjectNo.Text + "'";
                    strQuery += ", @pPROJECT_SEQ = '" + txtProjectSeq.Text + "'";
                    strQuery += ", @pENT_CD = '" + txtEntCd.Text + "'";
                    strQuery += ", @pSHIP_CUST = '" + txtShipCustCd.Text + "'";
                    strQuery += ", @pSO_NO = '" + txtSoNo.Text + "'";
                    strQuery += ", @pCONTRACT_TYPE = '" + cboContractType.SelectedValue.ToString() + "'";
                    strQuery += ", @pITEM_CD = '" + txtItemCd.Text + "'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";

                    string strCloseFlg = "";
                    if (rdoStart.Checked == true) { strCloseFlg = "Y"; }
                    else if (rdoClose.Checked == true) { strCloseFlg = "N"; }

                    strQuery += ", @pCLOSE_FLG = '" + strCloseFlg + "'";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0, true);

                    // 2019.08.13. hma 추가(Start): 합계수량 및 합계금액을 상단에 보여주도록 함.
                    double dSoQty = 0,dSoAmt = 0;
                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {
                        dSoQty = dSoQty + Convert.ToDouble(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수주량")].Value);
                        dSoAmt = dSoAmt + Convert.ToDouble(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수주금액")].Value);
                    }

                    txtSoQty.ReadOnly = false;
                    txtSoAmt.ReadOnly = false;

                    txtSoQty.Value = dSoQty;
                    txtSoAmt.Value = dSoAmt;

                    txtSoQty.ReadOnly = true;
                    txtSoAmt.ReadOnly = true;
                    // 2019.08.13. hma 추가(End)
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회중 오류가 발생하였습니다.
                }
            }
            this.Cursor = System.Windows.Forms.Cursors.Default;
        }
        #endregion

        #region 조회조건 팝업   
        //프로젝트번호 
        private void btnProjectNo_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW.WNDW007 pu = new WNDW.WNDW007(txtProjectNo.Text);
                pu.MaximizeBox = false;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtEntCd.Text = Msgs[1].ToString();
                    txtProjectNo.Text = Msgs[3].ToString();
                    txtProjectNm.Value = Msgs[4].ToString();
                    txtProjectSeq.Text = "";
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
                    txtProjectSeq.Text = Msgs[0].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        //사업코드
        private void btnEntCd_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = "usp_B_COMMON @pTYPE = 'TABLE_POP', @pSPEC1='ENT_CD', @pSPEC2 = 'ENT_NM', @pSPEC3 = 'S_ENTERPRISE_INFO', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtEntCd.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P05008", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "사업코드 조회");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtEntCd.Text = Msgs[0].ToString();
                    txtEntNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "사업코드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);	//데이터 조회 중 오류가 발생하였습니다.
            }
        }

        //수주번호
        private void btnSoNo_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW.WNDW012 pu = new WNDW.WNDW012();
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtSoNo.Value = Msgs[1].ToString();
                    txtSoNo.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "수주정보 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //품목
        private void btnItemCd_Click(object sender, EventArgs e)
        {
            try
            {
                WNDW005 pu = new WNDW005("10");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtItemCd.Text = Msgs[2].ToString();
                    txtItemNm.Value = Msgs[3].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목코드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //납품처
        private void btnShipCust_Click(object sender, EventArgs e)
        {
            try
            {
                WNDW002 pu = new WNDW002(txtShipCustCd.Text, "");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtShipCustCd.Text = Msgs[1].ToString();
                    txtShipCustNm.Value = Msgs[2].ToString();
                    txtShipCustCd.Focus();
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString());
            }
        }
        #endregion

        #region 조회조건 TextChanged      
        //프로젝트번호
        private void txtProjectNo_TextChanged(object sender, EventArgs e)
        {
            if (txtProjectNo.Text != "")
            {
                txtProjectNm.Value = SystemBase.Base.CodeName("PROJECT_NO", "PROJECT_NM", "S_SO_MASTER", txtProjectNo.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
                if (txtProjectNm.Value.ToString() == "")
                {
                    txtProjectSeq.Text = "";
                    txtEntCd.Text = "";
                }
            }
            else
            {
                txtProjectNm.Value = "";
            }
        }
        //사업코드
        private void txtEntCd_TextChanged(object sender, EventArgs e)
        {
            txtEntNm.Value = SystemBase.Base.CodeName("ENT_CD", "ENT_NM", "S_ENTERPRISE_INFO", txtEntCd.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
        }

        //품목 
        private void txtItemCd_TextChanged(object sender, EventArgs e)
        {
            try
            {
                txtItemNm.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtItemCd.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목명 가져오기"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }	
        }

        //납품처
        private void txtShipCustCd_TextChanged(object sender, EventArgs e)
        {
            txtShipCustNm.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtShipCustCd.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
        }
        #endregion

        #region 참조팝업
        //구매정보참조
        private void butReqRef_Click(object sender, EventArgs e)
        {
            if (fpSpread1.ActiveSheet.GetSelection(0) != null)
            {
                try
                {
                    string strPoNo = fpSpread1.Sheets[0].Cells[fpSpread1.ActiveSheet.GetSelection(0).Row, SystemBase.Base.GridHeadIndex(GHIdx1, "수주번호")].Text;
                    string strProjectNo = fpSpread1.Sheets[0].Cells[fpSpread1.ActiveSheet.GetSelection(0).Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text;
                    string strProjectSeq = fpSpread1.Sheets[0].Cells[fpSpread1.ActiveSheet.GetSelection(0).Row, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Text;
                    string strItemCd = fpSpread1.Sheets[0].Cells[fpSpread1.ActiveSheet.GetSelection(0).Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목")].Text;
                    string strItemNm = fpSpread1.Sheets[0].Cells[fpSpread1.ActiveSheet.GetSelection(0).Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목명")].Text;

                    SOB005P1 myForm = new SOB005P1(strPoNo, strProjectNo, strProjectSeq, strItemCd, strItemNm);
                    myForm.ShowDialog();
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "구매정보참조 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);	//데이터 조회 중 오류가 발생하였습니다.
                }
            }
        }

        //생산정보참조
        private void butMrpRef_Click(object sender, EventArgs e)
        {
            if (fpSpread1.ActiveSheet.GetSelection(0) != null)
            {
                try
                {
                    string strPoNo = fpSpread1.Sheets[0].Cells[fpSpread1.ActiveSheet.GetSelection(0).Row, SystemBase.Base.GridHeadIndex(GHIdx1, "수주번호")].Text;
                    string strProjectNo = fpSpread1.Sheets[0].Cells[fpSpread1.ActiveSheet.GetSelection(0).Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text;
                    string strProjectSeq = fpSpread1.Sheets[0].Cells[fpSpread1.ActiveSheet.GetSelection(0).Row, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Text;
                    string strItemCd = fpSpread1.Sheets[0].Cells[fpSpread1.ActiveSheet.GetSelection(0).Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목")].Text;
                    string strItemNm = fpSpread1.Sheets[0].Cells[fpSpread1.ActiveSheet.GetSelection(0).Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목명")].Text;

                    SOB005P2 myForm = new SOB005P2(strPoNo, strProjectNo, strProjectSeq, strItemCd, strItemNm);
                    myForm.ShowDialog();
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "생산정보참조 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);	//데이터 조회 중 오류가 발생하였습니다.
                }
            }
        }
        #endregion

    }
}
