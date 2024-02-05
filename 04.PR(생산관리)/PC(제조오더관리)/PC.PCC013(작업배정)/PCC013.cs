#region 작성정보
/*********************************************************************/
// 단위업무명 : 작업배정
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-03-25
// 작성내용 : 작업배정
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
using System.Data.SqlClient;
using WNDW;
using FarPoint.Win.Spread;

namespace PC.PCC013
{
    public partial class PCC013 : UIForm.FPCOMM2
    {
        private string strMQuery;

        string strWorkOrderNo = "", strProcSeq = "", strSheetStartDt = "", strSheetComptDt = "", strWcCd = "", strResCd = "", strJobCd = "";
        int BalQty = 0, WorkTmStd = 0;
        int Row = 0;
        string Key = "";
        int SaveRow = 0;

        public PCC013()
        {
            InitializeComponent();
        }

        #region Form Load 시
        private void PCC013_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            txtPlantCd.Text = SystemBase.Base.gstrPLANT_CD;

            dtpStartDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD").ToString()).AddYears(-1).ToShortDateString().Substring(0,10);
            dtpStartDtTo.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD").ToString()).AddYears(1).ToShortDateString().Substring(0,10);

            SystemBase.ComboMake.C1Combo(cboOrderStatus, "usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'P020' , @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 3); //지시상태
            SystemBase.ComboMake.C1Combo(cboWorkOrderType, "usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'P026' , @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 3); //지시구분
            SystemBase.ComboMake.C1Combo(cboSheetStatus, "usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'P057' , @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 3); //배정상태

            cboOrderStatus.SelectedValue = "RL";
            cboSheetStatus.SelectedValue = "0";

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
		
        }
        #endregion
        
        #region 행복사 버튼 클릭 이벤트
        protected override void RCopyExe()
        {
            UIForm.FPMake.grdReMake(fpSpread1, fpSpread1.Sheets[0].ActiveRowIndex,
                SystemBase.Base.GridHeadIndex(GHIdx1, "자원코드") + "|3"
                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "자원코드_2") + "|3"
                );
        }
        #endregion

        #region NewExec() 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);

            dtpStartDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD").ToString()).AddYears(-1).ToShortDateString().Substring(0,10);
            dtpStartDtTo.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD").ToString()).AddYears(1).ToShortDateString().Substring(0,10);

            txtPlantCd.Text = SystemBase.Base.gstrPLANT_CD.ToString();

            cboOrderStatus.SelectedValue = "RL";
            cboSheetStatus.SelectedValue = "0";
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            SaveRow = 0;
            Search("");
        }

        private void Search(string WorkOrderNo)
        {
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

                try
                {
                    strMQuery = " usp_PCC013 'S1'";
                    strMQuery += ", @pPLANT_CD = '" + txtPlantCd.Text + "' ";
                    strMQuery += ", @pPROJECT_NO ='" + txtProjectNo.Text + "'";
                    strMQuery += ", @pPROJECT_SEQ = '" + txtProjectSeq.Text + "' ";
                    strMQuery += ", @pITEM_CD ='" + txtItemCd.Text + "'";
                    strMQuery += ", @pGROUP_CD ='" + txtGroupCd.Text + "'";
                    strMQuery += ", @pRES_CD ='" + txtGResCd.Text + "'";
                    strMQuery += ", @pWC_CD ='" + txtWcCd.Text + "'";
                    strMQuery += ", @pWORKORDER_NO_FR ='" + txtWorkOrderNoFr.Text + "'";
                    strMQuery += ", @pWORKORDER_NO_To ='" + txtWorkOrderNoTo.Text + "'";
                    strMQuery += ", @pWORKORDER_NO_RS_FR ='" + txtWorkOrderNoRsFr.Text + "'";
                    strMQuery += ", @pWORKORDER_NO_RS_To ='" + txtWorkOrderNoRsTo.Text + "'";
                    strMQuery += ", @pSTART_DT_FR ='" + dtpStartDtFr.Text + "'";
                    strMQuery += ", @pSTART_DT_TO = '" + dtpStartDtTo.Text + "' ";
                    strMQuery += ", @pORDER_STATUS = '" + cboOrderStatus.SelectedValue + "' ";
                    strMQuery += ", @pORDER_TYPE = '" + cboWorkOrderType.SelectedValue + "' ";
                    strMQuery += ", @pSHEET_STATUS = '" + cboSheetStatus.SelectedValue + "' ";
                    strMQuery += ", @pJOB_CD = '" + txtJobCd.Text + "' ";
                    strMQuery += ", @pPROC_SEQ_FR = '" + txtProcSeqFr.Text + "' ";
                    strMQuery += ", @pPROC_SEQ_TO = '" + txtProcSeqTo.Text + "' ";
                    strMQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    strMQuery += ", @pDELIVERY_DT_REF_FR ='" + dtpRefDelvDtFr.Text + "'";       // 2020.02.12. hma 추가: 납기일(참조) FROM
                    strMQuery += ", @pDELIVERY_DT_REF_TO ='" + dtpRefDelvDtTo.Text + "'";       // 2020.02.12. hma 추가: 납기일(참조) TO


                    UIForm.FPMake.grdCommSheet(fpSpread2, strMQuery, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, true, 0, 6);

                    fpSpread2.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.Normal;

                    if (fpSpread2.Sheets[0].Rows.Count > 0)
                    {
                        for (int row = 0; row < fpSpread2.Sheets[0].Rows.Count; row++)
                        {
                            if (Convert.ToInt32(fpSpread2.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx2, "차이일수")].Value) < 0)
                            {
                                fpSpread2.Sheets[0].Cells[row, 0, row, fpSpread2.Sheets[0].Columns.Count - 1].ForeColor = Color.Red;
                            }
                        }

                        int x = 0, y = 0;

                        if (WorkOrderNo != "")
                        {
                            fpSpread2.Search(0, WorkOrderNo, false, false, false, false, 0, 0, ref x, ref y);

                            if (x > 0)
                            {
                                fpSpread2.Sheets[0].SetActiveCell(x, 1);
                                fpSpread2.ShowActiveCell(FarPoint.Win.Spread.VerticalPosition.Center, FarPoint.Win.Spread.HorizontalPosition.Center);
                            }
                            else
                            {
                                if (SaveRow <= fpSpread2.Sheets[0].Rows.Count)
                                {
                                    x = SaveRow;
                                    fpSpread2.Sheets[0].SetActiveCell(x, 1);
                                    fpSpread2.ShowActiveCell(FarPoint.Win.Spread.VerticalPosition.Center, FarPoint.Win.Spread.HorizontalPosition.Center);
                                }
                                else
                                {
                                    x = fpSpread2.Sheets[0].Rows.Count - 1;
                                    fpSpread2.Sheets[0].SetActiveCell(x, 1);
                                    fpSpread2.ShowActiveCell(FarPoint.Win.Spread.VerticalPosition.Center, FarPoint.Win.Spread.HorizontalPosition.Center);
                                }
                            }
                        }
                        else
                        {
                            if (SaveRow <= fpSpread2.Sheets[0].Rows.Count)
                            {
                                x = SaveRow;
                                fpSpread2.Sheets[0].SetActiveCell(x, 1);
                                fpSpread2.ShowActiveCell(FarPoint.Win.Spread.VerticalPosition.Center, FarPoint.Win.Spread.HorizontalPosition.Center);
                            }
                            else
                            {
                                x = fpSpread2.Sheets[0].Rows.Count - 1;
                                fpSpread2.Sheets[0].SetActiveCell(x, 1);
                                fpSpread2.ShowActiveCell(FarPoint.Win.Spread.VerticalPosition.Center, FarPoint.Win.Spread.HorizontalPosition.Center);
                            }
                        }

                        fpSpread2.Sheets[0].AddSelection(x, 1, 1, fpSpread2.Sheets[0].ColumnCount);

                        FpSpread2CellClick(x, 0);
                    }
                    else
                    {
                        fpSpread1.Sheets[0].Rows.Count = 0;
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                this.Cursor = System.Windows.Forms.Cursors.Default;
            }
        }
        #endregion

        #region 조회조건 팝업
        //공장코드
        private void btnPlant_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pTYPE = 'P013', @pBIZ_CD = '" + SystemBase.Base.gstrBIZCD + "' , @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'"; // 쿼리
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };											  // 쿼리 인자값(조회조건)
                string[] strSearch = new string[] { txtPlantCd.Text, "" };															  // 쿼리 인자값에 들어갈 데이타

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00005", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "공장 조회");

                pu.ShowDialog();	//공통 팝업 호출
                if (pu.DialogResult == DialogResult.OK)
                {
                    string MSG = pu.ReturnVal.Replace("|", "#");
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(MSG);

                    txtPlantCd.Text = Msgs[0].ToString();
                    txtPlantNm.Value = Msgs[1].ToString();
                    txtPlantCd.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "공장 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //프로젝트번호
        private void btnProject_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW003 pu = new WNDW003(txtProjectNo.Text, "S1", "C");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtProjectNo.Text = Msgs[3].ToString();
                    txtProjectNm.Value = Msgs[4].ToString();
                    txtProjectSeq.Text = Msgs[5].ToString();
                    txtGroupCd.Text = Msgs[6].ToString();
                    txtProjectNo.Focus();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "프로젝트 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //작업장
        private void btnWc_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pType='P610', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pETC = 'P002' , @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };
                string[] strSearch = new string[] { txtWcCd.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P05009", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "작업장 조회");
                pu.Width = 500;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtWcCd.Value = Msgs[0].ToString();
                    txtWcNm.Value = Msgs[1].ToString();
                    txtSubWcCd.Value = Msgs[2].ToString();
                    txtSubWcNm.Value = Msgs[3].ToString();
                    txtWcCd.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "작업장 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void button1_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pType='P611', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pETC = 'P002' , @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };
                string[] strSearch = new string[] { txtSubWcCd.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P05009", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "작업장 조회");
                pu.Width = 500;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtSubWcCd.Text = Msgs[2].ToString();
                    txtSubWcNm.Value = Msgs[3].ToString();
                    txtSubWcCd.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "작업장 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        // 자원그룹
        private void btnGRes_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pType='P052', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };
                string[] strSearch = new string[] { txtGResCd.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00068", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "자원 조회");
                pu.Width = 500;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtGResCd.Text = Msgs[0].ToString();
                    txtGResNm.Value = Msgs[1].ToString();
                    txtGResCd.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "자원 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        // 품목
        private void btnItem_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW005 pu = new WNDW005(txtItemCd.Text, "");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtItemCd.Text = Msgs[2].ToString();
                    txtItemNm.Value = Msgs[3].ToString();
                    txtItemCd.Focus();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목코드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        // 제조오더번호
        private void btnWorkOrderFr_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW006 pu = new WNDW006(txtWorkOrderNoFr.Text);
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtWorkOrderNoFr.Text = Msgs[1].ToString();
                    txtWorkOrderNoFr.Focus();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "제조오더번호"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnWorkOrderTo_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW006 pu = new WNDW006(txtWorkOrderNoTo.Text);
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtWorkOrderNoTo.Text = Msgs[1].ToString();
                    txtWorkOrderNoTo.Focus();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "제조오더번호"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //작업코드
        private void btnJob_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pType='P042', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pETC = 'P001' , @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };
                string[] strSearch = new string[] { txtJobCd.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00025", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "공정작업코드 조회");
                pu.Width = 500;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtJobCd.Text = Msgs[0].ToString();
                    txtJobNm.Value = Msgs[1].ToString();
                    txtJobCd.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "작업장 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 텍스트박스 코드 입력시 코드명 자동입력
        //공장
        private void txtPlantCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtPlantCd.Text != "")
                {
                    txtPlantNm.Value = SystemBase.Base.CodeName("PLANT_CD", "PLANT_NM", "B_PLANT_INFO", txtPlantCd.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");
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
        //품목코드
        private void txtItemCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtItemCd.Text != "")
                {
                    txtItemNm.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtItemCd.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");
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
        //작업장
        private void txtWcCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtWcCd.Text != "")
                {
                    txtWcNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtWcCd.Text, " AND MAJOR_CD = 'P002'  AND COMP_CODE='" + SystemBase.Base.gstrCOMCD + "' ");
                }
                else
                {
                    txtWcNm.Value = "";
                }
                if (txtWcCd.Text == "R005")
                {
                    txtSubWcCd.Tag = "보조작업장;1;;";
                    txtSubWcCd.Value = "";
                    txtSubWcCd.BackColor = SystemBase.Validation.Kind_LightCyan;

                    button1.Tag = "보조작업장;1;;";
                    button1.Enabled = true;
                }
                else
                {
                    txtSubWcCd.Tag = ";2;;";
                    txtSubWcCd.BackColor = SystemBase.Validation.Kind_Gainsboro;

                    button1.Tag = ";2;;";
                    button1.Enabled = false;

                    if (txtWcNm.Text != "")
                    {
                        txtSubWcCd.Value = txtWcCd.Text;
                        txtSubWcNm.Value = txtWcNm.Value;
                    }
                    else
                    {
                        txtSubWcCd.Value = "";
                        txtSubWcNm.Value = "";
                    }
                }
            }
            catch(Exception f)
            {
                MessageBox.Show(f.ToString());
            }
            
        }
        private void txtSubWcCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtSubWcCd.Text != "")
                {
                    txtSubWcNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtSubWcCd.Text, " AND MAJOR_CD = 'P061'  AND COMP_CODE='" + SystemBase.Base.gstrCOMCD + "' ");
                }
                else
                {
                    txtSubWcNm.Value = "";
                }
            }
            catch
            {

            }
        }
        //자원
        private void txtResCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtGResCd.Text != "")
                {
                    txtGResNm.Value = SystemBase.Base.CodeName("RES_CD", "RES_DIS", "P_RESO_MANAGE", txtGResCd.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");
                }
                else
                {
                    txtGResNm.Value = "";
                }
            }
            catch
            {

            }
        }
        //제품코드
        private void txtGroupCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtGroupCd.Text != "")
                {
                    txtGroupNm.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtGroupCd.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");
                }
                else
                {
                    txtGroupNm.Value = "";
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
                if (txtProjectNo.Text == "")
                {
                    txtProjectNm.Value = "";
                    txtProjectSeq.Text = "";
                    txtGroupCd.Text = "";
                }
                else
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
            }
            catch
            {

            }
        }
        //작업코드
        private void txtJobCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtJobCd.Text != "")
                {
                    txtJobNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtJobCd.Text, " AND MAJOR_CD = 'P001'  AND COMP_CODE='" + SystemBase.Base.gstrCOMCD + "' ");
                }
                else
                {
                    txtJobNm.Value = "";
                }
            }
            catch
            {

            }
        }
        #endregion

        #region 마스터 그리드 클릭시 이벤트
        private void fpSpread2_LeaveCell(object sender, FarPoint.Win.Spread.LeaveCellEventArgs e)
        {
            if (e.Row != e.NewRow)
            {
                Row = e.NewRow;

                FpSpread2CellClick(e.NewRow, 0);
            }
        }

        private void FpSpread2CellClick(int Row, int Col)
        {
            try
            {
                if (fpSpread2.Sheets[0].Rows.Count > 0)
                {
                    Row = Row;
                    //변수세팅
                    strWorkOrderNo = fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "제조오더번호")].Text;
                    strProcSeq = fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "공정순서")].Text;
                    strSheetStartDt = fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "계획착수일")].Text;
                    strSheetComptDt = fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "계획완료일")].Text;
                    strWcCd = fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "작업장코드")].Text;
                    BalQty = Convert.ToInt32(fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "잔여오더수량")].Value);
                    strResCd = fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "자원코드")].Text;
                    strJobCd = fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "작업코드")].Text;
                    WorkTmStd = Convert.ToInt32(fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "시수")].Value);

                    SubSearch(Row);
                }
            }
            catch { }
        }
        #endregion

        #region SubSearch()
        private void SubSearch(int Row)
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {
                    strMQuery = " usp_PCC013 'S2'";
                    strMQuery += ", @pWORKORDER_NO ='" + fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "제조오더번호")].Text + "'";
                    strMQuery += ", @pPROC_SEQ ='" + fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "공정순서")].Text + "'";
                    strMQuery += ", @pWC_CD ='" + fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "작업장코드")].Text + "'";
                    strMQuery += ", @pRES_CD ='" + fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "자원코드")].Text + "'";
                    strMQuery += ", @pPLANT_CD ='" + SystemBase.Base.gstrPLANT_CD.ToString() + "'";
                    strMQuery += ", @pSUB_WC_CD ='" + txtSubWcCd.Text + "'";
                    strMQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strMQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);

                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {
                        if (fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "그룹자원구분")].Text == "0")
                        {
                            UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "설비자원") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "설비자원_2") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "설비필요여부") + "|3"
                                );
                        }
                        else
                        {
                            UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "설비자원") + "|0"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "설비자원_2") + "|0"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "설비필요여부") + "|0"
                                );
                        }
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f);
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            this.Cursor = System.Windows.Forms.Cursors.Default;
        }
        #endregion

        #region 저장
        protected override void SaveExec()
        {
            this.Cursor = Cursors.WaitCursor;

            string ERRCode = "ER", MSGCode = "P0000"; //처리할 내용이 없습니다.

            SqlConnection dbConn = SystemBase.DbOpen.DBCON();
            SqlCommand cmd = dbConn.CreateCommand();
            SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

            try
            {
                //목표완료일 Master저장
                for (int k = 0; k < fpSpread2.Sheets[0].Rows.Count; k++)
                {
                    string strMstHead = fpSpread2.Sheets[0].RowHeader.Cells[k, 0].Text;

                    if (strMstHead.Length > 0)
                    {
                        if (strMstHead == "U")
                        {
                            string strMstSql = "";
                            strMstSql += " usp_PCC013 'U2'";
                            strMstSql += ", @pWORKORDER_NO = '" + fpSpread2.Sheets[0].Cells[k, SystemBase.Base.GridHeadIndex(GHIdx2, "제조오더번호")].Text + "'";
                            strMstSql += ", @pPROC_SEQ = '" + fpSpread2.Sheets[0].Cells[k, SystemBase.Base.GridHeadIndex(GHIdx2, "공정순서")].Text + "'";
                            strMstSql += ", @pSCH_COMPT_DT ='" + fpSpread2.Sheets[0].Cells[k, SystemBase.Base.GridHeadIndex(GHIdx2, "목표완료일")].Text + "'";
                            strMstSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                            DataSet ds1 = SystemBase.DbOpen.TranDataSet(strMstSql, dbConn, Trans);
                            ERRCode = ds1.Tables[0].Rows[0][0].ToString();
                            MSGCode = ds1.Tables[0].Rows[0][1].ToString();

                            if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
                        }
                    }
                }

                //detail 작업배정 저장
                //행수만큼 처리
                for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                {
                    string strHead = fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text;
                    string strGbn = "";

                    if (strHead == "U") { strGbn = "U1"; }
                    else if (strHead == "D") { strGbn = "D1"; }

                    if (strHead.Length > 0 && Convert.ToInt32(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "배정수량")].Value) > 0)
                    {
                        if (strHead == "U")
                        {
                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "설비필요여부")].Text == "False"
                                && fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "설비자원")].Text != "")
                            {
                                MessageBox.Show(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "작업자")].Text
                                    + " - " + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "작업자명")].Text
                                    + " : 설비필요여부가 체크되지 않았습니다. 설비필요여부를 확인해 주시기 바랍니다.", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);

                                this.Cursor = Cursors.Default;
                                Trans.Rollback();
                                return;
                            }

                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "설비필요여부")].Text == "True"
                                && fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "설비자원")].Text == "")
                            {
                                DialogResult Rtn = MessageBox.Show(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "작업자")].Text
                                    + " - " + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "작업자명")].Text
                                    + " : 설비자원이 배정되지 않았습니다. 설비미배정 상태로 됩니다. 계속 하시겠습니까?.", SystemBase.Base.MessageRtn("Z0004"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                                if (Rtn != DialogResult.Yes)
                                {
                                    this.Cursor = Cursors.Default;
                                    Trans.Rollback();
                                    return;
                                }
                            }
                        }


                        string strSql = "";
                        strSql += " usp_PCC013 '" + strGbn + "'";
                        strSql += ", @pWORKORDER_NO = '" + fpSpread2.Sheets[0].Cells[fpSpread2.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx2, "제조오더번호")].Text + "'";
                        strSql += ", @pPROC_SEQ = '" + fpSpread2.Sheets[0].Cells[fpSpread2.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx2, "공정순서")].Text + "'";
                        strSql += ", @pRES_CD = '" + fpSpread2.Sheets[0].Cells[fpSpread2.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx2, "자원코드")].Text + "'";
                        strSql += ", @pH_RES_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "작업자")].Text + "'";
                        strSql += ", @pM_RES_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "설비자원")].Text + "'";
                        strSql += ", @pSHEET_QTY = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "배정수량")].Value + "'";
                        strSql += ", @pWORK_TM_STD = '" + Convert.ToInt32(fpSpread2.Sheets[0].Cells[fpSpread2.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx2, "시수")].Value) + "' ";
                        strSql += ", @pSTART_DT = '" + fpSpread2.Sheets[0].Cells[fpSpread2.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx2, "계획착수일")].Text + "'";
                        strSql += ", @pWC_CD = '" + fpSpread2.Sheets[0].Cells[fpSpread2.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx2, "작업장코드")].Text + "'";
                        strSql += ", @pJOB_CD = '" + fpSpread2.Sheets[0].Cells[fpSpread2.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx2, "작업코드")].Text + "'";
                        strSql += ", @pRES_CAPA = '" + Convert.ToInt32(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "CAPA")].Value) + "' ";
                        strSql += ", @pRES_OT = '" + Convert.ToInt32(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "OT")].Value) + "' ";
                        strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                        strSql += ", @pSHEET_DT ='" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "배정일자")].Text + "'";
                        strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                        string strMResYn = "0";
                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "설비필요여부")].Text == "True")
                        {
                            strMResYn = "1";
                        }

                        strSql += ", @pM_RES_YN ='" + strMResYn + "'";

                        DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                        ERRCode = ds.Tables[0].Rows[0][0].ToString();
                        MSGCode = ds.Tables[0].Rows[0][1].ToString();

                        if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
                    }
                }
                Trans.Commit();
            }
            catch (Exception e)
            {
                SystemBase.Loggers.Log(this.Name, e.ToString());
                Trans.Rollback();
                ERRCode = "ER";
                MSGCode = "P0001";	//에러가 발생되어 데이터 처리가 취소되었습니다.
            }
        Exit:
            dbConn.Close();

            if (ERRCode == "OK")
            {
                AutoMessageBox mBox = new AutoMessageBox(SystemBase.Base.MessageRtn(MSGCode));
                mBox.ShowDialog();

                Key = fpSpread2.Sheets[0].Cells[fpSpread2.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx2, "제조오더번호")].Text
                    + fpSpread2.Sheets[0].Cells[fpSpread2.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx2, "공정순서")].Text;

                SaveRow = fpSpread2.Sheets[0].ActiveRowIndex;

                Search(Key);

            }
            else if (ERRCode == "ER")
            { MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); }
            else
            { MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning); }


            this.Cursor = Cursors.Default;
        }

        #endregion

        #region 자동수량배정/수량취소 버튼 이벤트
        private void btnAutoQty_Click(object sender, System.EventArgs e)
        {
            if (fpSpread2.Sheets[0].Rows.Count > 0 && fpSpread1.Sheets[0].Rows.Count > 0)
            {
                for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                {
                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "배정수량")].Text == "0")
                    {
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "배정수량")].Value = BalQty;

                        fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text = "U";
                    }
                }
            }
        }

        private void btnClearQty_Click(object sender, System.EventArgs e)
        {
            if (fpSpread2.Sheets[0].Rows.Count > 0 && fpSpread1.Sheets[0].Rows.Count > 0)
            {
                for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                {
                    if (fpSpread1.Sheets[0].RowHeader.Cells[i, 0, fpSpread1.Sheets[0].Rows.Count - 1, 0].Text == "U")
                    {
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "배정수량")].Value = 0;

                        fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text = "";
                    }
                }
            }
        }
        #endregion

        #region 그리드 버튼 클릭 이벤트
        private void fpSpread1_ButtonClicked(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "설비자원_2"))
            {
                try
                {
                    string strQuery = " usp_PCC013 @pType='C1', @pSUB_WC_CD = '" + txtSubWcCd.Text + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    string[] strWhere = new string[] { "@pRES_CD", "@pRES_DIS" };
                    string[] strSearch = new string[] { "", "" };
                    UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00096", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "설비자원 조회");
                    pu.Width = 700;
                    pu.ShowDialog();
                    if (pu.DialogResult == DialogResult.OK)
                    {
                        Regex rx1 = new Regex("#");
                        string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "설비자원")].Text = Msgs[0].ToString();
                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "설비자원명")].Text = Msgs[1].ToString();

                        if (fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "설비자원")].Text != ""
                            && fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "설비자원명")].Text != "")
                        {
                            fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "설비필요여부")].Value = 1;
                            fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "설비필요여부")].Locked = true;
                        }
                        else
                        {
                            fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "설비필요여부")].Locked = false;
                        }

                        UIForm.FPMake.fpChange(fpSpread1, e.Row);//수정플래그
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "설비자원 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "배정수량_2"))
            {
                try
                {
                    fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "배정수량")].Value = BalQty;

                    UIForm.FPMake.fpChange(fpSpread1, e.Row);//수정플래그
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "배정수량 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        #endregion

        #region 그리드 체인지 이벤트
        protected override void fpSpread1_ChangeEvent(int Row, int Column)
        {
            if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "설비자원"))
            {
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "설비자원명")].Text
                    = SystemBase.Base.CodeName("RES_CD", "RES_DIS", "P_RESO_MANAGE"
                    , fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "설비자원")].Text
                    , " And PLANT_CD = '" + SystemBase.Base.gstrPLANT_CD.ToString() + "'  AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");

                if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "설비자원")].Text != ""
                    && fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "설비자원명")].Text != "")
                {
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "설비필요여부")].Value = 1;
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "설비필요여부")].Locked = true;
                }
                else
                {
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "설비필요여부")].Locked = false;
                }
            }
        }
        #endregion

        #region 부품내역
        private void btnItemSpec_Click(object sender, System.EventArgs e)
        {
            if (strWorkOrderNo == "")
            {
                MessageBox.Show(SystemBase.Base.MessageRtn("B0061", "제조오더번호"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
            PCC013P2 form = new PCC013P2(strWorkOrderNo, strProcSeq);
            form.ShowDialog();
            this.Cursor = System.Windows.Forms.Cursors.Default;
        }
        #endregion

        #region 공정현황조회
        private void btnProcInfo_Click(object sender, System.EventArgs e)
        {
            if (fpSpread2.Sheets[0].Rows.Count > 0)
            {
                this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
                int Row = fpSpread2.Sheets[0].ActiveRowIndex;

                string ProjectNo = fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "프로젝트번호")].Text;
                string ProjectSeq = fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "차수")].Text;
                string ItemCd = fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "품목코드")].Text;
                string WoNo = fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "제조오더번호")].Text;
                                
                PCC013P1 myForm = new PCC013P1(ProjectNo, ProjectSeq, ItemCd, WoNo);
                myForm.ShowDialog();
                this.Cursor = System.Windows.Forms.Cursors.Default;
            }
        }
        #endregion

        #region 대표오더번호
        private void btnWorkOrderRsFr_Click(object sender, System.EventArgs e)
        {
            try
            {
                PCC013P3 pu = new PCC013P3(txtWorkOrderNoRsFr.Text);
                pu.Width = 900;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtWorkOrderNoRsFr.Text = Msgs[1].ToString();
                    txtWorkOrderNoRsFr.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "대표오더번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnWorkOrderRsTo_Click(object sender, System.EventArgs e)
        {
            try
            {
                PCC013P3 pu = new PCC013P3(txtWorkOrderNoRsTo.Text);
                pu.Width = 900;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtWorkOrderNoRsTo.Text = Msgs[1].ToString();
                    txtWorkOrderNoRsTo.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "대표오더번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region fpSpread2_EditChange
        private void fpSpread2_EditChange(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            UIForm.FPMake.fpChange(fpSpread2, e.Row);
        }
        #endregion

        #region fpSpread2_ChangeEvent Ctrl+V관련
        protected virtual void fpSpread2_ChangeEvent(int Row, int Col) { }
        private void fpSpread2_Change(object sender, FarPoint.Win.Spread.ChangeEventArgs e)
        {
            fpSpread1_ChangeEvent(e.Row, e.Column);
        }

        private void fpSpread2_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            try
            {
                if (e.Control && e.KeyCode == Keys.C)
                {
                    fpSpread2.Sheets[0].ClipboardCopy();
                }

                if (e.Control && e.KeyCode == Keys.V)
                {
                    fpSpread2.Sheets[0].ClipboardPaste(ClipboardPasteOptions.Values);

                    // 복사된 행의 열을 구하기 위하여 클립보드 사용.

                    IDataObject iData = Clipboard.GetDataObject();

                    string strClp = (string)iData.GetData(DataFormats.Text);

                    if (strClp != "" || strClp != null || strClp.Length > 0)
                    {
                        Regex rx1 = new Regex("\r\n");
                        string[] arrData = rx1.Split(strClp.ToString());

                        int DataCount = arrData.Length - 1;

                        if (DataCount > 0)
                        {
                            int STRow = fpSpread2.ActiveSheet.ActiveRowIndex;
                            if (STRow < 0)
                                STRow = 0;

                            int ClipRowCount = STRow + DataCount;
                            if (fpSpread2.Sheets[0].RowCount < DataCount)
                                ClipRowCount = fpSpread2.Sheets[0].RowCount - STRow;

                            for (int i = STRow; i < ClipRowCount; i++)
                            {
                                if (i < fpSpread2.Sheets[0].RowCount
                                    || fpSpread2.Sheets[0].Cells[i, fpSpread2.ActiveSheet.ActiveColumnIndex].Locked != true)
                                {
                                    if (fpSpread2.Sheets[0].RowHeader.Cells[i, 0].Text != "I")
                                        fpSpread2.Sheets[0].RowHeader.Cells[i, 0].Text = "U";

                                    fpSpread2_ChangeEvent(i, fpSpread1.ActiveSheet.ActiveColumnIndex);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "Clipboard 이벤트"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region fpSpread2_CellDoubleClick
        private void fpSpread2_CellDoubleClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            string Query = "SELECT 1 FROM P_BOP_UNITY_ORDER(NOLOCK) WHERE WORKORDER_NO_RS = '" + fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "제조오더번호")].Text + "' AND CO_CD='" + SystemBase.Base.gstrCOMCD + "'";
            DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);

            if (dt.Rows.Count > 0)
            {
                PCC013P4 form = new PCC013P4(fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "제조오더번호")].Text);
                form.ShowDialog();
            }
            else
            { MessageBox.Show("통합된 대표오더번호가 아닙니다.", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        }
        #endregion
    }
}
