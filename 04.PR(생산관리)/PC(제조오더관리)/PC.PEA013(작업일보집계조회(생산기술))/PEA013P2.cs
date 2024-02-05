#region 작성정보
/*******************************************************************************************************/
// 단위업무명:  작업일보집계조회(생산기술) > 공수상세조회 팝업
// 작 성 자  :  한 미 애
// 작 성 일  :  2019-05-29
// 작성내용  :  작업일보집계조회(생산기술) 그리드에서 더블클릭하면 해당 건에 대한 상세 데이터를 보여준다.
// 수 정 일  :
// 수 정 자  :
// 수정내용  :
// 비    고  :  
/*******************************************************************************************************/
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


namespace PC.PEA013
{
    public partial class PEA013P2 : UIForm.FPCOMM3
    {
        #region 변수선언
        string strWorkDtFr = "", strWorkDtTo = "", strWCCd = "", strWCNm = "", strWorkDuty = "", strWorkDutyNm = "",
                strItemCd = "", strItemNm = "", strProcSeq = "", strMResCd = "", strJobCd = "", strJobNm = "",
                strStatus = "", strCloseYN = "", strComptDtFr = "", strComptDtTo = "";
        string strStep = "";

        public static System.Drawing.Color Kind_LightCyan = Color.LightSkyBlue;//System.Drawing.Color.FromArgb(242, 252, 254);	// 필수 입력
        public static System.Drawing.Color Kind_Gainsboro = System.Drawing.Color.FromArgb(239, 239, 239);   // 읽기전용
        public static System.Drawing.Color Kind_White = System.Drawing.Color.White;
        public static System.Drawing.Color Kind_Linen = System.Drawing.Color.Linen;

        //public string strSelectDt = "";
        #endregion

        public PEA013P2()
        {
            InitializeComponent();
        }

        public PEA013P2(string WorkDtFr, string WorkDtTo, string WCCd, string WCNm, string WorkDuty, string WorkDutyNm,
                        string ItemCd, string ItemNm, string ProcSeq, string JobCd, string JobNm, string MResCd, 
                        string Status, string CloseYN, string ComptDtFr, string ComptDtTo)
        {
            strWorkDtFr = WorkDtFr;
            strWorkDtTo = WorkDtTo;
            strWCCd = WCCd;
            strWCNm = WCNm;
            strWorkDuty = WorkDuty;
            strWorkDutyNm = WorkDutyNm;
            strItemCd = ItemCd;
            strItemNm= ItemNm;
            strProcSeq = ProcSeq;
            strJobCd = JobCd;
            strJobNm = JobNm;
            strMResCd = MResCd;
            strStatus = Status;
            strCloseYN = CloseYN;
            strComptDtFr = ComptDtFr;
            strComptDtTo = ComptDtTo;

            InitializeComponent();
        }

        #region 폼로드 이벤트
        private void PEA013P2_Load(object sender, System.EventArgs e)
        {
            //버튼 재정의(조회권한만)
            UIForm.Buttons.ReButton("010000000001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);

            // 필수체크
            //SystemBase.Validation.GroupBox_Setting(groupBox1);

            this.Text = "작업일보집계조회(생산기술) > 공수상세조회";

            strStep = "N";

            dtpWorkDtFr.Text = strWorkDtFr;
            dtpWorkDtTo.Text = strWorkDtTo;
            txtItemCd.Text = strItemCd;
            txtItemNm.Text = strItemNm;
            txtWcCd.Text = strWCCd;
            txtWcNm.Text = strWCNm;
            txtWorkDuty.Text = strWorkDuty;
            txtWorkDutyNm.Text = strWorkDutyNm;
            txtProcSeq.Text = strProcSeq;
            txtProcJobCd.Text = strJobCd;
            txtProcJobNm.Text = strJobNm;
            txtMResCd.Text = strMResCd;

            if (strCloseYN == "Y")
                chkCloseY.Checked = true;
            else
                chkCloseY.Checked = false;

            dtpComptDtFR.Text = strComptDtFr;
            dtpComptDtTO.Text = strComptDtTo;

            rdoDirectSTHrs.Checked = true;        // 기본적으로 실동공수가 체크되도록 함.

            // 작업일자 및 품목코드 비활성화 처리
            dtpWorkDtFr.Enabled = false;
            dtpWorkDtTo.Enabled = false;
            txtItemCd.BackColor = Kind_Gainsboro;
            txtItemCd.Enabled = true;            
            txtItemNm.BackColor = Kind_Gainsboro;
            txtItemNm.ReadOnly = true;            
            txtWcNm.BackColor = Kind_Gainsboro;
            txtWcNm.ReadOnly = true;
            txtWorkDutyNm.BackColor = Kind_Gainsboro;
            txtWorkDutyNm.ReadOnly = true;
            txtMResNm.BackColor = Kind_Gainsboro;
            txtMResCd.ReadOnly = true;
            txtProcJobNm.BackColor = Kind_Gainsboro;
            txtProcJobNm.ReadOnly = true;

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

            strStep = "Y";

            SearchExec();
        }
        #endregion

        protected override void SearchExec()
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {
                if (chkCloseY.Checked == true)
                    strCloseYN = "Y";
                else
                    strCloseYN = "N";

                string strDataType = "";
                if (rdoDirectSTHrs.Checked == true)
                    strDataType = "D";
                else if (rdoIndirHrs.Checked == true)
                    strDataType = "I";

                string Query = " usp_PEA013 @pTYPE = 'P1'";
                Query += ", @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                Query += ", @pWORK_DT_FR = '" + dtpWorkDtFr.Text + "'";
                Query += ", @pWORK_DT_TO = '" + dtpWorkDtTo.Text + "'";
                Query += ", @pWC_CD = '" + txtWcCd.Text + "' ";
                Query += ", @pH_RES_CD = '" + txtWorkDuty.Text + "' ";
                Query += ", @pITEM_CD = '" + txtItemCd.Text + "' ";
                Query += ", @pPROC_SEQ = '" + txtProcSeq.Text + "' ";
                Query += ", @pM_RES_CD = '" + txtMResCd.Text + "' ";
                Query += ", @pJOB_CD = '" + txtProcJobCd.Text + "' ";
                Query += ", @pCLOSE_Y = '" + strCloseYN + "' ";
                Query += ", @pPLAN_COMPT_DT_FR = '" + dtpComptDtFR.Text + "'";
                Query += ", @pPLAN_COMPT_DT_TO = '" + dtpComptDtTO.Text + "'";
                Query += ", @pDATA_TYPE = '" + strDataType + "' ";

                if (rdoDirectSTHrs.Checked == true)
                    UIForm.FPMake.grdCommSheet(fpSpread1, Query, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, true, false, 0, 0, true);
                else if (rdoIndirHrs.Checked == true)
                    UIForm.FPMake.grdCommSheet(fpSpread2, Query, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, true, false, 0, 0, true);
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }

            this.Cursor = Cursors.Default;
        }

        #region 선택 변경 이벤트 처리
        #region rdoDirectHrs_CheckedChanged(): 직접공수 선택 여부에 대한 처리
        private void rdoDirectHrs_CheckedChanged(object sender, EventArgs e)
        {
            if (strStep == "Y")
            {
                if (rdoDirectSTHrs.Checked == true)
                {
                    GridCommPanel1.Visible = true;
                    GridCommPanel2.Visible = false;
                    SearchExec();
                }
                else
                {
                    GridCommPanel1.Visible = false;
                    GridCommPanel2.Visible = true;
                    SearchExec();
                }
            }
        }
        #endregion
        #endregion


        #region 버튼 클릭 이벤트

        private void btnWcCd_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pTYPE = 'P042', @pLANG_CD = 'KOR', @pETC = 'P061' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'"; // 쿼리
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };        // 쿼리 인자값(조회조건)
                string[] strSearch = new string[] { txtWcCd.Text, "" };             // 쿼리 인자값에 들어갈 데이타

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


        private void btnWorkDuty_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pTYPE = 'P054' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";    // 쿼리
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };        // 쿼리 인자값(조회조건)
                string[] strSearch = new string[] { txtWorkDuty.Text, "" };         // 쿼리 인자값에 들어갈 데이타

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

        private void btnJob_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pType='P042', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pETC = 'P001' , @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };
                string[] strSearch = new string[] { txtProcJobCd.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00025", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "공정작업코드 조회");
                pu.Width = 500;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtProcJobCd.Text = Msgs[0].ToString();
                    txtProcJobNm.Value = Msgs[1].ToString();
                    txtProcJobCd.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "작업장 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 검색조건 항목 변경 이벤트
        private void txtWcCd_TextChanged(object sender, EventArgs e)
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

        private void txtWorkDuty_TextChanged(object sender, EventArgs e)
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

        private void txtMResCd_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (strStep == "Y")
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
            }
            catch { }
        }

        private void txtProcJobCd_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (strStep == "Y")
                {
                    if (txtProcJobCd.Text != "")
                    {
                        txtProcJobNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtProcJobCd.Text, " AND MAJOR_CD = 'P001'  AND COMP_CODE='" + SystemBase.Base.gstrCOMCD + "' ");
                    }
                    else
                    {
                        txtProcJobNm.Value = "";
                    }
                }
            }
            catch
            {
            }
        }
        #endregion

    }
}
