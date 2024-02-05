#region 작성정보
/*********************************************************************/
// 단위업무명:  수불현황조회(프로젝트변경)
// 작 성 자  :  한 미 애
// 작 성 일  :  2018-10-25
// 작성내용  :  구매입고에 대한 프로젝트를 변경한 데이터 포함한 기준으로 수불 데이터를 조회한다.
// 수 정 일  :
// 수 정 자  :
// 수정내용  :
// 비    고  :
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

namespace IT.ITR119
{
    public partial class ITR119 : UIForm.FPCOMM1
    {
        #region 변수선언
        bool form_act_chk = false;
        int SDown = 1;		// 조회 횟수
        int AddRow = 100;
        #endregion

        #region 생성자
        public ITR119()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void ITR119_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            SystemBase.ComboMake.C1Combo(cboPlantCd, "usp_B_COMMON @pType='B031', @pSPEC1 = '" + SystemBase.Base.gstrBIZCD + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//공장
            SystemBase.ComboMake.C1Combo(cboTranType, "usp_B_COMMON @pType='COMM', @pCODE = 'I001', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 3);//수불구분
            SystemBase.ComboMake.C1Combo(cboItemAcct, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'B036', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 3); //품목계정

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

            //기타 세팅
            dtpTranDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString().Substring(0,10);
            dtpTranDtTo.Text = SystemBase.Base.ServerTime("YYMMDD").Substring(0,10);
            txtSlFr.Value = "W03";
        }
        #endregion
        
        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            fpSpread1.Sheets[0].Rows.Count = 0;

            //기타 세팅
            dtpTranDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString().Substring(0,10);
            dtpTranDtTo.Text = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0,10);
            txtSlFr.Value = "W03";
        }
        #endregion

        #region SearchExec()  그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {
                    string strQuery = " usp_ITR119 'S1'";
                    strQuery += ", @pLANG_CD ='" + SystemBase.Base.gstrLangCd + "'";
                    strQuery += ", @pPLANT_CD ='" + cboPlantCd.SelectedValue.ToString() + "'";
                    strQuery += ", @pITEM_CD ='" + txtItemCd.Text.Trim() + "'";
                    strQuery += ", @pTRAN_DT_FR ='" + dtpTranDtFr.Text + "'";
                    strQuery += ", @pTRAN_DT_TO ='" + dtpTranDtTo.Text + "'";
                    strQuery += ", @pITEM_ACCT ='" + cboItemAcct.SelectedValue.ToString() + "'";
                    strQuery += ", @pSL_CD_FR ='" + txtSlFr.Text.Trim() + "'";
                    strQuery += ", @pMOVE_TYPE ='" + txtMoveType.Text.Trim() + "'";
                    strQuery += ", @pTRAN_TYPE ='" + cboTranType.SelectedValue.ToString() + "'";
                    strQuery += ", @pENT_CD ='" + txtEnt_CD.Text.Trim() + "'";
                    strQuery += ", @pPROJECT_NO ='" + txtProject_No.Text.Trim() + "'";
                    strQuery += ", @pPROJECT_SEQ_FR ='" + txtProject_Seq.Text.Trim() + "'";
                    strQuery += ", @pPROJECT_SEQ_TO ='" + txtProject_Seq1.Text.Trim() + "'";
                    strQuery += ", @pWORKORDER_NO_FR ='" + txtWorkOrderNo_FR.Text.Trim() + "'";
                    strQuery += ", @pWORKORDER_NO_TO ='" + txtWorkOrderNo_TO.Text.Trim() + "'";
                    strQuery += ", @pTOPCOUNT ='" + AddRow + "'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 2, true);
                    fpSpread1.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.Normal;

                    if (fpSpread1.Sheets[0].RowCount > 0)
                    {
                        if (cboTranType.SelectedValue.ToString() == "PI" || cboTranType.SelectedValue.ToString() == "OI" || cboTranType.SelectedValue.ToString() == "DI") //생산출고 또는 예외출고일때
                        {
                            Decimal dePbAmt = 0;
                            Decimal deVbAmt = 0;
                            Decimal totAmt = 0;

                            if (cboTranType.SelectedValue.ToString() == "PI")
                            {
                                for (int i = 0; i < fpSpread1.Sheets[0].RowCount; i++)
                                {
                                    //구입부품
                                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목구분")].Text == "PB" || fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목구분")].Text == "PA")
                                    {
                                        dePbAmt += Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "금액")].Value);
                                    }

                                    //수입부품
                                    else if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목구분")].Text == "VB" || fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목구분")].Text == "VA")
                                    {
                                        deVbAmt += Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "금액")].Value);
                                    }
                                }
                                dtxtAmt1.ReadOnly = false;
                                dtxtAmt2.ReadOnly = false;
                                dtxtTotalAmt.ReadOnly = false;

                                dtxtAmt1.Value = dePbAmt.ToString();
                                dtxtAmt2.Value = deVbAmt.ToString();
                                dtxtTotalAmt.Value = Convert.ToString(dePbAmt + deVbAmt);

                                dtxtAmt1.ReadOnly = true;
                                dtxtAmt2.ReadOnly = true;
                                dtxtTotalAmt.ReadOnly = true;
                            }
                            else
                            {
                                for (int i = 0; i < fpSpread1.Sheets[0].RowCount; i++)
                                {
                                    totAmt += Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "금액")].Value);
                                }
                                dtxtAmt1.ReadOnly = false;
                                dtxtAmt2.ReadOnly = false;
                                dtxtTotalAmt.ReadOnly = false;

                                dtxtAmt1.Value = "0";
                                dtxtAmt2.Value = "0";
                                dtxtTotalAmt.Value = Convert.ToString(totAmt);

                                dtxtAmt1.ReadOnly = true;
                                dtxtAmt2.ReadOnly = true;
                                dtxtTotalAmt.ReadOnly = true;
                            }
                        }
                        else
                        {
                            dtxtAmt1.ReadOnly = false;
                            dtxtAmt2.ReadOnly = false;
                            dtxtTotalAmt.ReadOnly = false;

                            dtxtAmt1.Value = "0";
                            dtxtAmt2.Value = "0";
                            dtxtTotalAmt.Value = "0";

                            dtxtAmt1.ReadOnly = true;
                            dtxtAmt2.ReadOnly = true;
                            dtxtTotalAmt.ReadOnly = true;
                        }
                    }
                    else
                    {
                        dtxtAmt1.ReadOnly = false;
                        dtxtAmt2.ReadOnly = false;
                        dtxtTotalAmt.ReadOnly = false;

                        dtxtAmt1.Value = "0";
                        dtxtAmt2.Value = "0";
                        dtxtTotalAmt.Value = "0";

                        dtxtAmt1.ReadOnly = true;
                        dtxtAmt2.ReadOnly = true;
                        dtxtTotalAmt.ReadOnly = true;
                    }
                }
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

        #region 100건씩 조회
        private void fpSpread1_TopChange(object sender, FarPoint.Win.Spread.TopChangeEventArgs e)
        {
            int FPHeight = (fpSpread1.Size.Height - 28) / 20;
            if (e.NewTop >= ((AddRow * SDown) - FPHeight))
            {
                SDown++;

                this.Cursor = Cursors.WaitCursor;

                string strQuery = " usp_ITR119 'S1'";
                strQuery += ", @pLANG_CD ='" + SystemBase.Base.gstrLangCd + "'";
                strQuery += ", @pPLANT_CD ='" + cboPlantCd.SelectedValue.ToString() + "'";
                strQuery += ", @pITEM_CD ='" + txtItemCd.Text.Trim() + "'";
                strQuery += ", @pTRAN_DT_FR ='" + dtpTranDtFr.Text + "'";
                strQuery += ", @pTRAN_DT_TO ='" + dtpTranDtTo.Text + "'";
                strQuery += ", @pITEM_ACCT ='" + cboItemAcct.SelectedValue.ToString() + "'";
                strQuery += ", @pSL_CD_FR ='" + txtSlFr.Text.Trim() + "'";
                strQuery += ", @pMOVE_TYPE ='" + txtMoveType.Text.Trim() + "'";
                strQuery += ", @pTRAN_TYPE ='" + cboTranType.SelectedValue.ToString() + "'";
                strQuery += ", @pENT_CD ='" + txtEnt_CD.Text.Trim() + "'";
                strQuery += ", @pPROJECT_NO ='" + txtProject_No.Text.Trim() + "'";
                strQuery += ", @pPROJECT_SEQ_FR ='" + txtProject_Seq.Text.Trim() + "'";
                strQuery += ", @pPROJECT_SEQ_TO ='" + txtProject_Seq1.Text.Trim() + "'";
                strQuery += ", @pWORKORDER_NO_FR ='" + txtWorkOrderNo_FR.Text.Trim() + "'";
                strQuery += ", @pWORKORDER_NO_TO ='" + txtWorkOrderNo_TO.Text.Trim() + "'";
                strQuery += ", @pTOPCOUNT ='" + AddRow * SDown + "'";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery);

                this.Cursor = Cursors.Default;
            }
        }
        #endregion

        #region 버튼 Click
        private void btnSlFr_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON 'B035', @pSPEC1 = '" + cboPlantCd.SelectedValue.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtSlFr.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00014", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "창고팝업");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtSlFr.Value = Msgs[0].ToString();
                    txtSlNmFr.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        private void btnItem_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW005 pu = new WNDW005(cboPlantCd.SelectedValue.ToString(), true, txtItemCd.Text);
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtItemCd.Value = Msgs[2].ToString();
                    txtItemNm.Value = Msgs[3].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        private void btnMoveType_Click(object sender, System.EventArgs e)
        {
            DialogResult dsMsg;
            try
            {
                if (cboTranType.SelectedValue.ToString() == "")
                {
                    dsMsg = MessageBox.Show("수불구분을 먼저 선택하세요!", "경고", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    cboTranType.Focus();
                    return;
                }
                string strQuery
                    = "usp_B_COMMON @pTYPE = 'TABLE_POP1', @pSPEC1 = 'MOVE_TYPE', @pSPEC2 = 'MOVE_TYPE_NM', @pSPEC3 = 'I_MOVE_TYPE', @pSPEC4 = 'TRAN_TYPE' , @pSPEC5 = '" + cboTranType.SelectedValue.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtMoveType.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00054", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "수불유형 조회");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtMoveType.Value = Msgs[0].ToString();
                    txtMoveTypeNm.Value = Msgs[1].ToString();

                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        // 사업
        private void btnEnt_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON @pTYPE ='TABLE_POP', @pSPEC1 = 'ENT_CD', @pSPEC2 = 'ENT_NM', @pSPEC3 = 'S_ENTERPRISE_INFO', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtEnt_CD.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00007", strQuery, strWhere, strSearch, new int[] { 0, 1 });
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtEnt_CD.Value = Msgs[0].ToString();
                    txtEnt_NM.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        // 프로젝트
        private void btnProject_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW007 pu = new WNDW007(txtProject_No.Text);
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;
                    txtProject_No.Value = Msgs[3].ToString();
                    txtProject_Nm.Value = Msgs[4].ToString();
                    if (txtProject_Seq.Text != "*") txtProject_Seq.Value = "";
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        private void btnProjectSeq_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON 'PROJ_SEQ', @pSPEC1 = '" + txtProject_No.Text + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";										// 쿼리
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
                    txtProject_Seq.Value = Msgs[0].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        private void btnProjectSeq1_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON 'PROJ_SEQ', @pSPEC1 = '" + txtProject_No.Text + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";										// 쿼리
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
                    txtProject_Seq1.Value = Msgs[0].ToString();
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

        #region TextChanged
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

        private void txtSlFr_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtSlFr.Text != "")
                {
                    txtSlNmFr.Value = SystemBase.Base.CodeName("SL_CD", "SL_NM", "B_STORAGE_INFO", txtSlFr.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtSlNmFr.Value = "";
                }
            }
            catch
            {

            }
        }

        private void txtMoveType_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtMoveType.Text != "")
                {
                    txtMoveTypeNm.Value = SystemBase.Base.CodeName("MOVE_TYPE", "MOVE_TYPE_NM", "I_MOVE_TYPE", txtMoveType.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtMoveTypeNm.Value = "";
                }
            }
            catch
            {

            }
        }

        private void txtEnt_CD_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtEnt_CD.Text != "")
                {
                    txtEnt_NM.Value = SystemBase.Base.CodeName("ENT_CD", "ENT_NM", "S_ENTERPRISE_INFO", txtEnt_CD.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtEnt_NM.Value = "";
                }
            }
            catch
            {

            }
        }

        private void txtProject_No_TextChanged(object sender, System.EventArgs e)
        {
            
            try
            {
                if (txtProject_No.Text != "")
                {
                    txtProject_Nm.Value = SystemBase.Base.CodeName("PROJECT_NO", "PROJECT_NM", "S_SO_MASTER", txtProject_No.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtProject_Nm.Value = "";
                }
                if (txtProject_Seq.Text != "*")
                { txtProject_Seq.Value = ""; txtProject_Seq1.Value = ""; }
            }
            catch
            {

            }
        }
        #endregion

        #region 폼 Activated & Deactivate
        private void ITR119_Activated(object sender, System.EventArgs e)
        {
            if (form_act_chk == false) cboPlantCd.Focus();
        }

        private void ITR119_Deactivate(object sender, System.EventArgs e)
        {
            form_act_chk = true;
        }
        #endregion
    }
}
