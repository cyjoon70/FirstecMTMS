#region 작성정보
/*********************************************************************/
// 단위업무명 : 실시간 TOUCH 집계현황
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-04-16
// 작성내용 : 실시간 TOUCH 집계현황 관리
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

namespace PC.PEA008
{
    public partial class PEA008 : UIForm.FPCOMM1
    {
        #region 생성자
        public PEA008()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load시
        private void PEA008_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            //콤보박스 세팅
            SystemBase.ComboMake.C1Combo(cboDIV, "usp_B_COMMON @pType='COMM', @pCODE = 'P062', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 3);

            //기타 세팅
            dtpWorkDtFr.Value = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0,10);
            dtpWorkDtTo.Value = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0,10);
            dtpComptDtFR.Value = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0, 10);
            dtpComptDtTO.Value = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0, 10);

            label3.Visible = false;
            cboIndirectNo.Visible = false;
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
        }
        #endregion

        #region 조회조건 팝업
        //작업자
        private void btnWorkDuty_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pTYPE = 'P054' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";				// 쿼리
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };			// 쿼리 인자값(조회조건)
                string[] strSearch = new string[] { txtWorkDuty.Text, "" };							// 쿼리 인자값에 들어갈 데이타

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00071", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "작업자 조회", false);
                pu.Width = 600;
                pu.ShowDialog();	//공통 팝업 호출
                if (pu.DialogResult == DialogResult.OK)
                {
                    string MSG = pu.ReturnVal.Replace("|", "#");
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(MSG);

                    txtWorkDuty.Value = Msgs[0].ToString();
                    txtWorkDutyNm.Value = Msgs[1].ToString();
                    txtWorkDuty.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "작업자 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //작업장
        private void btnWcCd_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pTYPE = 'P042', @pLANG_CD = 'KOR', @pETC = 'P061' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";					// 쿼리
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };					// 쿼리 인자값(조회조건)
                string[] strSearch = new string[] { txtWcCd.Text, "" };								// 쿼리 인자값에 들어갈 데이타

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00025", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "작업장 조회", false);
                pu.ShowDialog();	//공통 팝업 호출
                if (pu.DialogResult == DialogResult.OK)
                {
                    string MSG = pu.ReturnVal.Replace("|", "#");
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(MSG);

                    txtWcCd.Value = Msgs[0].ToString();
                    txtWcNm.Value = Msgs[1].ToString();
                    txtWcCd.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "부서 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnItem_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW005 pu = new WNDW005(txtItemCd.Text, "");
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

        private void btnWrokOrderNoFr_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW006 pu = new WNDW006(txtWorkOrderNoFr.Text);
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtWorkOrderNoFr.Value = Msgs[1].ToString();
                    txtWorkOrderNoFr.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "제조오더번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnWorkOrderNoTo_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW006 pu = new WNDW006(txtWorkOrderNoTo.Text);
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtWorkOrderNoTo.Value = Msgs[1].ToString();
                    txtWorkOrderNoTo.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "제조오더번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnProjectNo_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW003 pu = new WNDW003(btnProjectNo.Text, "S1", "C");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtProjectNo.Value = Msgs[3].ToString();
                    txtProjectNm.Value = Msgs[4].ToString();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnGroupCd_Click(object sender, EventArgs e)
        {
            try
            {
                WNDW005 pu = new WNDW005(txtGroupCd.Text, "");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtGroupCd.Text = Msgs[2].ToString();
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

        private void btnJob_Click(object sender, EventArgs e)
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
                    txtProjectSeqFr.Text = Msgs[0].ToString();
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
        private void btnProjectSeqTo_Click_1(object sender, EventArgs e)
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
                    txtProjectSeqFr.Text = Msgs[0].ToString();
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
        //작업자
        private void txtWorkDuty_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtWorkDuty.Text != "")
                {
                    txtWorkDutyNm.Value = SystemBase.Base.CodeName("RES_CD", "RES_DIS", "P_RESO_MANAGE", txtWorkDuty.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");
                }
                else
                {
                    txtWorkDutyNm.Value = "";
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
                    txtWcNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtWcCd.Text, " AND MAJOR_CD = 'P061'  AND COMP_CODE='" + SystemBase.Base.gstrCOMCD + "' ");
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
            }
            catch
            {

            }
        }

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
        #endregion

        #region NewExec()
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            fpSpread1.Sheets[0].Rows.Count = 0;

            //기타 세팅

            dtpWorkDtFr.Value = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0,10);
            dtpWorkDtTo.Value = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0,10);
            dtpComptDtFR.Value = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0, 10);
            dtpComptDtTO.Value = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0, 10);

            cboDIV.SelectedIndex = 0;

        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {
                string strStatus = "";
                string strType = "";
                string strCloseY = "";     // 2018.11.12. hma 추가: 마감건만 조회시

                string strDiv = Convert.ToString(cboDIV.SelectedValue);

                if (rdoAll.Checked == true) { strStatus = "A"; }
                else if (rdoING.Checked == true) { strStatus = "I"; }
                else { strStatus = "C"; }

                if (strDiv == "") { strType = "S1"; }
                else if (strDiv == "I") { strType = "S3"; }
                else { strType = "S2"; }

                string strQuery = " usp_PEA008  @pTYPE = '" + strType + "'";
                strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'";
                strQuery += ", @pH_RES_CD= '" + txtWorkDuty.Text + "'";

                if (cboIndirectNo.Visible == true)
                    strQuery += ", @pINDIRECT_NO= '" + cboIndirectNo.SelectedValue + "'";

                strQuery += ", @pWC_CD = '" + txtWcCd.Text + "'";
                strQuery += ", @pWORK_DT_FR = '" + dtpWorkDtFr.Text + "'";
                strQuery += ", @pWORK_DT_TO = '" + dtpWorkDtTo.Text + "'";
                strQuery += ", @pSTATUS = '" + strStatus + "'";
                strQuery += ", @pWORKORDER_NO_FR = '" + txtWorkOrderNoFr.Text + "'";
                strQuery += ", @pWORKORDER_NO_TO = '" + txtWorkOrderNoTo.Text + "'";
                strQuery += ", @pITEM_CD = '" + txtItemCd.Text + "'";
                strQuery += ", @pPROJECT_NO = '" + txtProjectNo.Text + "'";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                if (txtGroupCd.Text != "")
                    strQuery += ", @pGROUP_CD ='" + txtGroupCd.Text + "'";
                if (txtJobCd.Text != "")
                    strQuery += ", @pJOB_CD = '" + txtJobCd.Text + "' ";

                strQuery += ", @pPROJECT_SEQ_FR = '" + txtProjectSeqFr.Text + "' ";
                strQuery += ", @pM_RES_CD = '" + txtMResCd.Text + "'";          // 2018.07.19. hma 추가: 설비자원(배정설비)
                strQuery += ", @pCOMPT_DT_FR = '" + dtpComptDtFR.Text + "'";          // 2019.04.04. ksh 추가: 목표완료일 추가
                strQuery += ", @pCOMPT_DT_TO = '" + dtpComptDtTO.Text + "'";          // 2019.04.04. ksh 추가: 목표완료일 추가

                // 2018.11.12. hma 추가(Start): 마감건만 조회여부
                strCloseY = "N";
                if (chkCloseY.Checked == true)
                    strCloseY = "Y";
                strQuery += ", @pCLOSE_Y = '" + strCloseY + "'";
                // 2018.11.12. hma 추가(End)

                // 2020.11.11. ksh 추가(Start) : 수주납기일 조회조건 추가
                strQuery += ", @pREF_DELV_DT_FR = '" + dtpDeliveryFr.Text + "'";
                strQuery += ", @pREF_DELV_DT_TO = '" + dtpDeliveryTo.Text + "'";
                // 2020.11.11. ksh 추가(End)

                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0, true);

                // 2018.10.15. hma 추가(Start): 시간(분) 항목 시간을 합산하여 시간으로 환산한 후 상단 공수합계 항목에 표기.
                decimal dWorkHourSum = 0;
                if (fpSpread1.Sheets[0].Rows.Count > 0)
                {
                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {
                        //2019.04.01 ksh 수정 : 시간(분) -> 실동공수(분) 변경
                        //dWorkHourSum += Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "시간(분)")].Value);
                        dWorkHourSum += Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "실동공수(분)")].Value);
                        
                    }
                    txtWorkHourSum.ReadOnly = false;
                    txtWorkHourSum.Value = (dWorkHourSum / 60).ToString();
                    txtWorkHourSum.ReadOnly = true;
                }
                // 2018.10.15. hma 추가(End)
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }

            this.Cursor = Cursors.Default;
        }
        #endregion

        #region 직/간접 항목 cboDIV_SelectedIndexChanged
        private void cboDIV_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            string val = Convert.ToString(cboDIV.SelectedValue);

            if (val == "D")
            {
                label3.Visible = true;
                label3.Text = "직접항목";
                cboIndirectNo.Visible = true;
                SystemBase.ComboMake.C1Combo(cboIndirectNo, "usp_B_COMMON @pType='COMM', @pCODE = 'P063', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 3);

            }
            else if (val == "I")
            {
                label3.Visible = true;
                label3.Text = "간접항목";
                cboIndirectNo.Visible = true;
                SystemBase.ComboMake.C1Combo(cboIndirectNo, "usp_B_COMMON @pType='COMM', @pCODE = 'P025', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 3);
            }
            else
            {
                label3.Visible = false;
                cboIndirectNo.Visible = false;
            }
        }
        #endregion

        // 2018.07.19. hma 추가(Start)
        #region txtMResCd_TextChanged(): 설비자원 코드 변경시 이벤트 처리
        private void txtMResCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtMResCd.Text != "")
                {
                    txtMResNm.Value = SystemBase.Base.CodeName("RES_CD", "RES_DIS", "P_RESO_MANAGE", txtMResCd.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");
                }
                else
                {
                    txtMResNm.Value = "";
                }
            }
            catch { }
        }

        private void btnMRes_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pType='P065' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };
                string[] strSearch = new string[] { txtMResCd.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00066", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "작업자 조회");
                pu.Width = 500;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtMResCd.Text = Msgs[0].ToString();
                    txtMResNm.Value = Msgs[1].ToString();
                    txtMResCd.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "설비자원 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 2020.12.29. hma 추가(Start): 작업대기 상태 작업자에 대한 작업배정 및 TOUCH실적 조회
        #region btnWaitWorker_Click(): 작업대기 상태 작업자에 대한 작업배정 및 TOUCH실적 조회
        private void btnWaitWorker_Click(object sender, EventArgs e)
        {
            PEA008P1 myForm = new PEA008P1();
            myForm.ShowDialog();
        }
        #endregion
        // 2020.12.29. hma 추가(End)
        #endregion
        // 2018.07.19. hma 추가(End)
    }
}
