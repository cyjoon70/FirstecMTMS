#region 작성정보
/*********************************************************************/
// 단위업무명 : 제조오더현황조회
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-04-16
// 작성내용 : 제조오더현황조회 및 관리
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

namespace PC.PCC043
{
    public partial class PCC043 : UIForm.FPCOMM1
    {
        #region 생성자
        public PCC043()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load시
        private void PCC043_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            //콤보박스세팅
            SystemBase.ComboMake.C1Combo(cboPlantCd, "usp_B_COMMON @pType='TABLE', @pCODE = 'PLANT_CD', @pNAME = 'PLANT_NM', @pSPEC1 = 'B_PLANT_INFO', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//공장
            SystemBase.ComboMake.C1Combo(cboStatus, "usp_P_COMMON @pTYPE = 'P150' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 3);  //지시상태
            SystemBase.ComboMake.C1Combo(cboOrderFlag, "usp_B_COMMON @pType='COMM', @pCODE = 'P026', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 3);  //지시구분

            //그리드초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

            //기타세팅
            cboPlantCd.SelectedValue = SystemBase.Base.gstrPLANT_CD;
            cboOrderFlag.SelectedValue = "3";
            cboStatus.SelectedValue = "RL";
            dtpPlanStartDtFr.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToString().Substring(0,10);
            dtpPlanStartDtTo.Value = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0,10);
        }
        #endregion
        
        #region NewExec()
        protected override void NewExec()
        {
            //조회조건 초기화
            SystemBase.Base.GroupBoxReset(groupBox1);

            //그리드 초기화
            fpSpread1.Sheets[0].Rows.Count = 0;

            //기타세팅
            cboPlantCd.SelectedValue = SystemBase.Base.gstrPLANT_CD;
            cboOrderFlag.SelectedValue = "3";
            cboStatus.SelectedValue = "RL";
            dtpPlanStartDtFr.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToString().Substring(0,10);
            dtpPlanStartDtTo.Value = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0,10);
            rdoDivN.Checked = true;
        }
        #endregion

        #region 조회조건 팝업
        //품목코드
        private void btnItemCd_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW005 pu = new WNDW005(Convert.ToString(cboPlantCd.SelectedValue), true, txtItemCd.Text);
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

        //제품코드
        private void btnGroupCd_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW005 pu = new WNDW005(Convert.ToString(cboPlantCd.SelectedValue), true, txtGroupCd.Text);
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

        //프로젝트번호
        private void btnProjectNo_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW003 pu = new WNDW003(txtProjectNo.Text, "S1");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtProjectNo.Value = Msgs[3].ToString();
                    txtProjectNm.Value = Msgs[4].ToString();
                    txtProjectSeqFr.Value = Msgs[5].ToString();
                    txtProjectSeqTo.Value = Msgs[5].ToString();
                    txtItemCd.Value = Msgs[6].ToString();
                    txtItemNm.Value = Msgs[7].ToString();

                    txtProjectNo.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "프로젝트 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //프로젝트차수 FROM
        private void btnProjectSeqFr_Click(object sender, System.EventArgs e)
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
                    txtProjectSeqFr.Value = Msgs[0].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        //프로젝트차수 TO
        private void btnProjectSeqTo_Click(object sender, System.EventArgs e)
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
                    txtProjectSeqTo.Value = Msgs[0].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        //제조오더번호 FROM
        private void btnWorkorderNoFr_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW006 pu = new WNDW006(txtWorkorderNoFr.Text);
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtWorkorderNoFr.Value = Msgs[1].ToString();
                    txtWorkorderNoFr.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "제조오더번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //제조오더번호 TO
        private void btnWorkorderNoTo_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW006 pu = new WNDW006(txtWorkorderNoTo.Text);
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtWorkorderNoTo.Value = Msgs[1].ToString();
                    txtWorkorderNoTo.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "제조오더번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //제품오더번호 FROM
        private void btnMakeorderNoFr_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW008 pu = new WNDW008(txtMakeorderNoFr.Text, "R");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtMakeorderNoFr.Value = Msgs[1].ToString();
                    txtProjectNo.Value = Msgs[6].ToString();
                    txtProjectNm.Value = Msgs[7].ToString();
                    txtProjectSeqFr.Value = Msgs[8].ToString();
                    txtItemCd.Value = Msgs[9].ToString();
                    txtItemNm.Value = Msgs[10].ToString();
                    txtMakeorderNoFr.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "제품오더번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //제품오더번호 TO
        private void btnMakeorderNoTo_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW008 pu = new WNDW008(txtMakeorderNoTo.Text, "R");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtMakeorderNoTo.Value = Msgs[1].ToString();
                    txtMakeorderNoTo.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "제품오더번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        //생산담당자
        private void btnProdManaDuty_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON 'B010' ,@pSPEC1='" + SystemBase.Base.gstrBIZCD + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtProdManaDuty.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P04003", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "담당자 조회");	//생산관리 사용자조회
                pu.Width = 450;
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtProdManaDuty.Value = Msgs[0].ToString();
                    txtProdManaDutyNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "생산담당자 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //대표오더번호 FROM
        private void btnWorkorderNoRsFr_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW.WNDW028 pu = new WNDW.WNDW028(txtWorkorderNoRsFr.Text);
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtWorkorderNoRsFr.Value = Msgs[1].ToString();
                    txtWorkorderNoRsFr.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "대표오더정보조회 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //대표오더번호 TO
        private void btnWorkorderNoRsTo_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW.WNDW028 pu = new WNDW.WNDW028(txtWorkorderNoRsTo.Text);
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtWorkorderNoRsTo.Value = Msgs[1].ToString();
                    txtWorkorderNoRsTo.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "대표오더정보조회 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 조회조건 TextChanged
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
            if (txtProjectNm.Text == "")
            {
                txtProjectSeqFr.Value = "";
                txtProjectSeqTo.Value = "";
            }
        }

        //생산담당자
        private void txtProdManaDuty_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtProdManaDuty.Text != "")
                {
                    txtProdManaDutyNm.Value = SystemBase.Base.CodeName("USR_ID", "USR_NM", "B_SYS_USER", txtProdManaDuty.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");
                }
                else
                {
                    txtProdManaDutyNm.Value = "";
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
                    string cfmYn = "";
                    if (rdoDivY.Checked == true) { cfmYn = "Y"; }
                    else if (rdoDivN.Checked == true) { cfmYn = "N"; }
                    else { cfmYn = ""; }

                    string strQuery = " usp_PCC043  @pTYPE = 'S1'";
                    strQuery += ", @pPLANT_CD = '" + Convert.ToString(cboPlantCd.SelectedValue) + "' ";
                    strQuery += ", @pDELIVERY_DT_FR = '" + dtpDeliveryDtFr.Text + "' ";
                    strQuery += ", @pDELIVERY_DT_TO = '" + dtpDeliveryDtTo.Text + "' ";
                    strQuery += ", @pITEM_CD = '" + txtItemCd.Text + "' ";
                    strQuery += ", @pGROUP_CD = '" + txtGroupCd.Text + "' ";
                    strQuery += ", @pPLAN_START_DT_FR = '" + dtpPlanStartDtFr.Text + "' ";
                    strQuery += ", @pPLAN_START_DT_TO = '" + dtpPlanStartDtTo.Text + "' ";
                    strQuery += ", @pWORKORDER_NO_FR = '" + txtWorkorderNoFr.Text + "' ";
                    strQuery += ", @pWORKORDER_NO_TO = '" + txtWorkorderNoTo.Text + "' ";
                    strQuery += ", @pMAKEORDER_NO_FR = '" + txtMakeorderNoFr.Text + "' ";
                    strQuery += ", @pMAKEORDER_NO_TO = '" + txtMakeorderNoTo.Text + "' ";
                    strQuery += ", @pPROJECT_NO = '" + txtProjectNo.Text + "' ";
                    strQuery += ", @pPROD_MANA_DUTY = '" + txtProdManaDuty.Text + "' ";
                    strQuery += ", @pPROJECT_SEQ_FR = '" + txtProjectSeqFr.Text + "' ";
                    strQuery += ", @pPROJECT_SEQ_TO = '" + txtProjectSeqTo.Text + "'";
                    strQuery += ", @pDIV = '" + cfmYn + "'";
                    strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'";
                    strQuery += ", @pORDER_STATUS = '" + Convert.ToString(cboStatus.SelectedValue) + "'";
                    strQuery += ", @pPRODT_ORDER_TYPE = '" + Convert.ToString(cboOrderFlag.SelectedValue) + "'";
                    strQuery += ", @pWORKORDER_NO_RS_FR = '" + txtWorkorderNoRsFr.Text + "' ";
                    strQuery += ", @pWORKORDER_NO_RS_TO = '" + txtWorkorderNoRsTo.Text + "' ";
                    strQuery += ", @pCO_CD = '" +  SystemBase.Base.gstrCOMCD + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 1, true);

                    if (fpSpread1.Sheets[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                        {
                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "생산L/T")].ForeColor = Color.Blue;
                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품질L/T")].ForeColor = Color.Green;
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

        #region 공정진행현황
        private void btnProcInfo_Click(object sender, System.EventArgs e)
        {
            if (fpSpread1.Sheets[0].Rows.Count > 0)
            {
                int Row = fpSpread1.Sheets[0].ActiveRowIndex;

                string ProjectNo = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text;
                string ProjectSeq = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Text;
                string ItemCd = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text;
                string WoNo = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "오더번호")].Text;

                PCC043P1 myForm = new PCC043P1(ProjectNo, ProjectSeq, ItemCd, WoNo);
                myForm.ShowDialog();
            }
        }
        #endregion

        #region 부품내역
        private void btnItemSpec_Click(object sender, System.EventArgs e)
        {
            if (fpSpread1.Sheets[0].Rows.Count > 0)
            {
                int Row = fpSpread1.Sheets[0].ActiveRowIndex;

                string strWoNo = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "오더번호")].Text;
                string strProcSeq = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "공정순서")].Text;

                if (strWoNo == "")
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0061", "제조오더번호"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                PCC043P2 form = new PCC043P2(strWoNo);
                form.ShowDialog();
            }
        }
        #endregion	

    }
}
