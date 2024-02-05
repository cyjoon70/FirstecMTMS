#region 작성정보
/*********************************************************************/
// 단위업무명 : 공정별 실적현황조회
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-04-16
// 작성내용 : 공정별 실적현황조회 관리
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

namespace PC.PSB005
{
    public partial class PSB005 : UIForm.FPCOMM1
    {
        #region 생성자
        public PSB005()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load시
        private void PSB005_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            txtPlantCd.Value = SystemBase.Base.gstrPLANT_CD;

            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "단위")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'Z005' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//단위
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "통화")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'Z003' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//통화
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "지시상태")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'P020' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//지시상태
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "지시구분")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'P026' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//작업지시구분
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "사내/외")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'P028' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//공정사내외주구분

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false, false);

            dtpDtFr.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString().Substring(0,10);
            dtpDtTo.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(1).ToShortDateString().Substring(0,10);
            dtpReportDtFr.Value = "";
            dtpReportDtTo.Value = "";
            dtpRefDelvDtFr.Value = "";
            dtpRefDelvDtTo.Value = "";
            rdoRlSt.Checked = true;
            rdoAll.Checked = true;
        }
        #endregion
        
        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            fpSpread1.Sheets[0].Rows.Count = 0;

            txtPlantCd.Value = SystemBase.Base.gstrPLANT_CD.ToString();
            dtpDtFr.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString().Substring(0,10);
            dtpDtTo.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(1).ToShortDateString().Substring(0,10);
            dtpReportDtFr.Value = "";
            dtpReportDtTo.Value = "";
            dtpRefDelvDtFr.Value = "";
            dtpRefDelvDtTo.Value = "";
            rdoRlSt.Checked = true;
            rdoAll.Checked = true;

        }
        #endregion

        #region 조회조건 팝업
        //공장
        private void btnPlant_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pTYPE = 'P013', @pBIZ_CD = '" + SystemBase.Base.gstrBIZCD + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'"; // 쿼리
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };											  // 쿼리 인자값(조회조건)
                string[] strSearch = new string[] { txtPlantCd.Text, "" };															  // 쿼리 인자값에 들어갈 데이타

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00005", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "공장 조회", false);

                pu.ShowDialog();	//공통 팝업 호출
                if (pu.DialogResult == DialogResult.OK)
                {
                    string MSG = pu.ReturnVal.Replace("|", "#");
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(MSG);

                    txtPlantCd.Value = Msgs[0].ToString();
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

        //공정작업코드
        private void btnJob_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pType='P042', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pETC = 'P001' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };
                string[] strSearch = new string[] { txtJobCd.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00025", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "공정작업코드 조회");
                pu.Width = 500;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtJobCd.Value = Msgs[0].ToString();
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

        //제조오더번호
        private void btnWorkOrderNo_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW006 pu = new WNDW006(txtWorkOrderNo.Text);
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtWorkOrderNo.Value = Msgs[1].ToString();
                    txtWorkOrderNo.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "제조오더번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnWorkOrderNo_To_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW006 pu = new WNDW006(txtWorkOrderNo_To.Text);
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtWorkOrderNo_To.Value = Msgs[1].ToString();
                    txtWorkOrderNo_To.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "제조오더번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //사업
        private void btnEnt_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON @pTYPE ='TABLE_POP', @pSPEC1 = 'ENT_CD', @pSPEC2 = 'ENT_NM', @pSPEC3 = 'S_ENTERPRISE_INFO', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtEntCd.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00007", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "사업 조회");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtEntCd.Value = Msgs[0].ToString();
                    txtEntNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "사업코드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //프로젝트번호
        private void btnProject_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW003 pu = new WNDW003(txtProjectNo.Text, "S1", "S");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtEntCd.Value = Msgs[1].ToString();
                    txtEntNm.Value = Msgs[2].ToString();
                    txtProjectNo.Value = Msgs[3].ToString();
                    txtProjectNm.Value = Msgs[4].ToString();
                    txtProjectSeq.Value = Msgs[5].ToString();
                    txtGroupCd.Value = Msgs[6].ToString();
                    txtGroupNm.Value = Msgs[7].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "프로젝트 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //품목코드
        private void btnItem_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW005 pu = new WNDW005(txtPlantCd.Text, "10", txtItemCd.Text);
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
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목코드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //작업장
        private void btnWc_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pType='P042', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pETC = 'P002' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };
                string[] strSearch = new string[] { txtWcCd.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00025", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "작업장 조회");
                pu.Width = 500;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtWcCd.Value = Msgs[0].ToString();
                    txtWcNm.Value = Msgs[1].ToString();
                    txtWcCd.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "작업장 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //제품코드
        private void btnGroup_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW005 pu = new WNDW005(txtPlantCd.Text, "10", txtGroupCd.Text);
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtGroupCd.Value = Msgs[2].ToString();
                    txtGroupNm.Value = Msgs[3].ToString();
                    txtGroupCd.Focus();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목코드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //생산담당자
        private void btnMfPlanUser_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON 'B010' ,@pSPEC1='" + SystemBase.Base.gstrBIZCD + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtMfPlanUser.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P04003", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "담당자 조회");	//생산관리 사용자조회
                pu.Width = 450;
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtMfPlanUser.Value = Msgs[0].ToString();
                    txtMfPlanUserNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "생산담당자 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 코드 입력시 코드명 자동 입력
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
        //사업
        private void txtEntCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtEntCd.Text != "")
                {
                    txtEntNm.Value = SystemBase.Base.CodeName("ENT_CD", "ENT_NM", "S_ENTERPRISE_INFO", txtEntCd.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");
                }
                else
                {
                    txtEntNm.Value = "";
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
                    txtProjectNm.Value = SystemBase.Base.CodeName("PROJECT_NO", "PROJECT_NM", "S_SO_MASTER", txtProjectNo.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");
                }
                else
                {
                    txtProjectNm.Value = "";
                }

                if (txtProjectNm.Text == "")
                {
                    txtEntCd.Value = "";
                    txtEntNm.Value = "";
                    txtProjectNm.Value = "";
                    txtProjectSeq.Value = "";
                    txtGroupCd.Value = "";
                    txtGroupNm.Value = "";
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

        //생산담당자
        private void txtMfPlanUser_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtMfPlanUser.Text != "")
                {
                    txtMfPlanUserNm.Value = SystemBase.Base.CodeName("USR_ID", "USR_NM", "B_SYS_USER", txtMfPlanUser.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");
                }
                else
                {
                    txtMfPlanUserNm.Value = "";
                }
            }
            catch
            {

            }
        }

        //공정작업코드
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

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {
                    string MilestoneFlg = "";
                    if (rdoYes.Checked == true) { MilestoneFlg = "Y"; }
                    else if (rdoNo.Checked == true) { MilestoneFlg = "N"; }

                    string OrderStatusFlag = "";
                    if (rdoRl.Checked == true) { OrderStatusFlag = "RL"; }
                    else if (rdoSt.Checked == true) { OrderStatusFlag = "ST"; }
                    else if (rdoRlSt.Checked == true) { OrderStatusFlag = "RLST"; }
                    else if (rdoCl.Checked == true) { OrderStatusFlag = "CL"; }

                    string strMQuery = "";
                    strMQuery = "   usp_PSB005 @pTYPE = 'S1'";
                    strMQuery += ",            @pDT_FR = '" + dtpDtFr.Text + "' ";
                    strMQuery += ",            @pDT_TO = '" + dtpDtTo.Text + "' ";
                    strMQuery += ",            @pREPORT_DT_FR = '" + dtpReportDtFr.Text + "' ";
                    strMQuery += ",            @pREPORT_DT_TO = '" + dtpReportDtTo.Text + "' ";
                    strMQuery += ",            @pREF_DELV_DT_FR = '" + dtpRefDelvDtFr.Text + "' ";
                    strMQuery += ",            @pREF_DELV_DT_TO = '" + dtpRefDelvDtTo.Text + "' ";
                    strMQuery += ",            @pENT_CD = '" + txtEntCd.Text + "' ";
                    strMQuery += ",            @pWORKORDER_NO = '" + txtWorkOrderNo.Text + "' ";
                    strMQuery += ",            @pWORKORDER_NO_TO = '" + txtWorkOrderNo_To.Text + "' ";
                    strMQuery += ",            @pITEM_CD = '" + txtItemCd.Text + "' ";
                    strMQuery += ",            @pWC_CD = '" + txtWcCd.Text + "' ";
                    strMQuery += ",            @pPLANT_CD = '" + txtPlantCd.Text + "' ";
                    strMQuery += ",            @pPROJECT_NO = '" + txtProjectNo.Text + "' ";
                    strMQuery += ",            @pPROJECT_SEQ = '" + txtProjectSeq.Text + "' ";
                    strMQuery += ",            @pORDER_STATUS = '" + OrderStatusFlag + "' ";
                    strMQuery += ",            @pMF_PLAN_USER = '" + txtMfPlanUser.Text + "' ";
                    strMQuery += ",            @pJOB_CD = '" + txtJobCd.Text + "' ";
                    strMQuery += ",            @pMILESTONE_FLG = '" + MilestoneFlg + "' ";
                    strMQuery += ",            @pGROUP_CD = '" + txtGroupCd.Text + "' ";
                    strMQuery += ",            @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strMQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0, true);

                    //그리드 셋팅
                    if (fpSpread1.Sheets[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                        {
                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "오더번호")].Text.Trim() == "합계"
                               || fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "오더번호")].Text.Trim() == "총합계")
                            {
                                if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "오더번호")].Text.Trim() == "합계")
                                {
                                    fpSpread1.Sheets[0].Cells[i, 1, i, fpSpread1.Sheets[0].Columns.Count - 1].BackColor = SystemBase.Base.gColor1;
                                }

                                if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "오더번호")].Text.Trim() == "총합계")
                                {
                                    fpSpread1.Sheets[0].Cells[i, 1, i, fpSpread1.Sheets[0].Columns.Count - 1].BackColor = Color.Beige;
                                }

                                for (int j = 2; j < fpSpread1.Sheets[0].Columns.Count; j++)
                                {
                                    if (fpSpread1.Sheets[0].Cells[i, j].Text.Trim() == "합계" || fpSpread1.Sheets[0].Cells[i, j].Text.Trim() == "총합계")
                                    {
                                        fpSpread1.Sheets[0].Cells[i, j].Text = "";
                                    }
                                }
                            }

                        }
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            this.Cursor = System.Windows.Forms.Cursors.Default;
        }
        #endregion
    }
}
