#region 작성정보
/*********************************************************************/
// 단위업무명 : 검사결과등록
// 작 성 자 : 권 순 철
// 작 성 일 : 2013-04-02
// 작성내용 : 검사결과등록 및 관리
// 수 정 일 :
// 수 정 자 :
// 수정내용 :
// 비    고 :
/*********************************************************************/
#endregion


using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Globalization;
using System.Threading;
using WNDW;
using System.Text.RegularExpressions;
using System.IO;
using System.Reflection;

namespace QM.QMA003
{
    public partial class QMA003 : UIForm.FPCOMM1
    {
        #region 변수선언
        bool Linked = false;
        string strPlantCd = "";
        string strInspClass = "";
        string strInspReqNo = "";
        string strInspReqDt = "";
        #endregion

        #region 생성자
        public QMA003()
        {
            InitializeComponent();
        }

        public QMA003(string param1, string param2, string param3, string param4)
        {
            //
            // Windows Form 디자이너 지원에 필요합니다.
            //
            strInspReqNo = param1;
            strPlantCd = param2;
            strInspClass = param3;
            strInspReqDt = param4;
            Linked = true;

            InitializeComponent();

            //
            // TODO: InitializeComponent를 호출한 다음 생성자 코드를 추가합니다.
            //
        }
        #endregion

        #region Form Load시
        private void QMA003_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            SystemBase.Validation.GroupBox_Setting(groupBox2);

            //콤보박스 세팅
            SystemBase.ComboMake.C1Combo(cboPlantCd, "usp_B_COMMON @pType='TABLE', @pCODE = 'PLANT_CD', @pNAME = 'PLANT_NM', @pSPEC1 = 'B_PLANT_INFO', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 0);//공장
            SystemBase.ComboMake.C1Combo(cboDecision, "usp_B_COMMON @pType='COMM', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "',@pCODE = 'Q004', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9); //판정

            //그리드 콤보박스 세팅			
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "판정")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'Q004', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ", 0);
            
            //그리드초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

            //기타 세팅
            //dtpInspReqDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-3).ToString().Substring(0,10);
            //dtpInspReqDtTo.Text = SystemBase.Base.ServerTime("YYMMDD");
            rdoInspN.Checked = true;
            rdoInspClassP.Checked = true;

            dtpInspDt.Text = SystemBase.Base.ServerTime("YYMMDD");

            if (Linked == true)
            {
                cboPlantCd.SelectedValue = strPlantCd;
                txtInspReqNo.Text = strInspReqNo;
                if (strInspClass == "R")
                {
                    rdoInspClassR.Checked = true;
                }
                else if (strInspClass == "F")
                {
                    rdoInspClassP.Checked = true;
                }
                else if (strInspClass == "P")
                {
                    rdoInspClassP.Checked = true;
                }
                else if (strInspClass == "S")
                {
                    rdoInspClassS.Checked = true;
                }

                dtpInspReqDtFr.Text = strInspReqDt;
                dtpInspReqDtTo.Text = strInspReqDt;
                rdoInspAll.Checked = true;
                SearchExec();
            }

            lnkJump1.Text = "부적합처리";  //화면에 보여지는 링크명
            strJumpFileName1 = "QM.QMA011.QMA011"; //호출할 화면명
        }
        #endregion

        #region Link
        private object[] Params()
        {

            if (fpSpread1.Sheets[0].Rows.Count <= 0 || fpSpread1.Sheets[0].Cells[fpSpread1.ActiveSheet.ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "검사의뢰번호")].Text == "")
                param = null;						// 파라메터를 하나도 넘기지 않을경우
            else
            {
                param = new object[5];					// 파라메터수가 5개인 경우
                param[0] = fpSpread1.Sheets[0].Cells[fpSpread1.ActiveSheet.ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "검사의뢰번호")].Text;
                param[1] = Convert.ToString(cboPlantCd.SelectedValue);
                string strInspClass = "";
                if (rdoInspClassR.Checked == true)
                {
                    strInspClass = "R";
                }
                else if (rdoInspClassP.Checked == true)
                {
                    strInspClass = "P";
                }
                else if (rdoInspClassF.Checked == true)
                {
                    strInspClass = "F";
                }
                else if (rdoInspClassS.Checked == true)
                {
                    strInspClass = "S";
                }

                param[2] = strInspClass;
                param[3] = dtpInspReqDtFr.Text;
                param[4] = dtpInspReqDtTo.Text;
            }
            return param;
        }

        protected override void Link1Exec()
        {
            param = Params();

            SystemBase.Base.RodeFormID = "QMA011";			// 이동할 formid를 적어준다.(메뉴id)
            SystemBase.Base.RodeFormText = "부적합처리(SIMPLE)"; 	// 이동할 폼명을 적어준다(메뉴명)
        }
        #endregion
        
        #region 버튼 Click
        //품목코드
        private void btnItemCd_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW005 pu = new WNDW005(cboPlantCd.SelectedValue.ToString(), true, txtItemCd.Text);
                pu.MaximizeBox = false;
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
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        //검사원
        private void btnUser_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON 'COMM_POP' ,@pSPEC1='Q005', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtUser.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00067", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "검사원 팝업");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {

                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtUser.Text = Msgs[0].ToString();
                    txtUserNm.Value = Msgs[1].ToString();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        //검사의뢰번호
        private void btnInspReqNo_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strInspClass = "";
                if (rdoInspClassR.Checked == true)
                    strInspClass = "R";
                else if (rdoInspClassF.Checked == true)
                    strInspClass = "F";
                else if (rdoInspClassP.Checked == true)
                    strInspClass = "P";
                else if (rdoInspClassS.Checked == true)
                    strInspClass = "S";



                WNDW009 pu = new WNDW009(Convert.ToString(cboPlantCd.SelectedValue)
                    , txtInspReqNo.Text
                    , strInspClass
                    , "Q");
                pu.MaximizeBox = false;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtInspReqNo.Text = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "검사의뢰번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //공급처
        private void btnCust_Click(object sender, EventArgs e)
        {
            try
            {
                WNDW002 pu = new WNDW002("P");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtCustCd.Text = Msgs[1].ToString();
                    txtCustNm.Value = Msgs[2].ToString();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        //프로젝트번호
        private void btnProj_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = " usp_M_COMMON 'P001', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";										// 쿼리
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };				// 쿼리 인자값(조회조건)
                string[] strSearch = new string[] { txtProjectNo.Text, "" };		// 쿼리 인자값에 들어갈 데이타

                //UIForm.PopUpSP pu = new UIForm.PopUpSP(strQuery, strWhere, strSearch, PHeadText7, PTxtAlign7, PCellType7, PHeadWidth7, PSearchLabel7);
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00074", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "프로젝트 조회", false);
                pu.Width = 500;
                pu.ShowDialog();	//공통 팝업 호출

                if (pu.DialogResult == DialogResult.OK)
                {
                    string MSG = pu.ReturnVal.Replace("|", "#");
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(MSG);

                    txtProjectNo.Text = Msgs[0].ToString();
                    txtProjectNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        //제조오더번호From
        private void btnWorkOrderNo_Fr_Click(object sender, EventArgs e)
        {
            try
            {
                WNDW006 pu = new WNDW006(txtWorkOrderNo_Fr.Text);
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtWorkOrderNo_Fr.Text = Msgs[1].ToString();
                    txtWorkOrderNo_Fr.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "제조오더번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //제조오더번호To
        private void btnWorkOrderNo_To_Click(object sender, EventArgs e)
        {
            try
            {
                WNDW006 pu = new WNDW006(txtWorkOrderNo_To.Text);
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtWorkOrderNo_To.Text = Msgs[1].ToString();
                    txtWorkOrderNo_To.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "제조오더번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //작업장
        private void btnWc_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pTYPE = 'P242', @pLANG_CD = 'KOR', @pETC = 'P002', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";					// 쿼리
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };					// 쿼리 인자값(조회조건)
                string[] strSearch = new string[] { txtWcCd.Text, "" };								// 쿼리 인자값에 들어갈 데이타

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00025", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "작업장 조회", false);
                pu.ShowDialog();	//공통 팝업 호출
                if (pu.DialogResult == DialogResult.OK)
                {
                    string MSG = pu.ReturnVal.Replace("|", "#");
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(MSG);

                    txtWcCd.Text = Msgs[0].ToString();
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

        //Release 담당자
        private void BtnReleaserCd_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON 'COMM_POP' ,@pSPEC1='Q005', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtReleaserCd.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00067", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "Release 담당자 팝업");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {

                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtReleaserCd.Text = Msgs[0].ToString();
                    txtReleaserNm.Value = Msgs[1].ToString();
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
        //품목코드
        private void txtItemCd_TextChanged(object sender, System.EventArgs e)
        {
            txtItemNm.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtItemCd.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
        }

        //프로젝트번호
        private void txtProjectNo_TextChanged(object sender, System.EventArgs e)
        {
            txtProjectNm.Value = SystemBase.Base.CodeName("PROJECT_NO", "PROJECT_NM", "S_SO_MASTER", txtProjectNo.Text, " AND SO_CONFIRM_YN = 'Y'  AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
        }

        //검사원
        private void txtUser_TextChanged(object sender, EventArgs e)
        {
            txtUserNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtUser.Text, " AND MAJOR_CD = 'Q005' AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND COMP_CODE ='" + SystemBase.Base.gstrCOMCD + "'");
        }

        //Release 담당자
        private void txtReleaserCd_TextChanged(object sender, EventArgs e)
        {
            txtReleaserNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtReleaserCd.Text, " AND MAJOR_CD = 'Q005' AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND COMP_CODE ='" + SystemBase.Base.gstrCOMCD + "'");
        }

        //공급처
        private void txtCustCd_TextChanged(object sender, EventArgs e)
        {
            txtCustNm.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtCustCd.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
        }

        //작업장
        private void txtWcCd_TextChanged(object sender, EventArgs e)
        {
            txtWcNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtWcCd.Text, " AND MAJOR_CD = 'P002'  AND COMP_CODE ='" + SystemBase.Base.gstrCOMCD + "'");
        }
        #endregion

        #region NewExec()
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            SystemBase.Validation.GroupBox_Reset(groupBox2);

            fpSpread1.Sheets[0].Rows.Count = 0;

            //기타 세팅
            //dtpInspReqDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-3).ToString().Substring(0,10);
            //dtpInspReqDtTo.Text = SystemBase.Base.ServerTime("YYMMDD");

            rdoInspN.Checked = true;
            dtpInspDt.Text = SystemBase.Base.ServerTime("YYMMDD");
            //cboInspClass.SelectedValue = "R"; //수입검사
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1) == true)
                {
                    string strYn = "";
                    if (rdoInspY.Checked == true) { strYn = "Y"; }
                    else if (rdoInspN.Checked == true) { strYn = "N"; }
                    else if (rdoInspR.Checked == true) { strYn = "R"; }

                    string strInspClass = "";
                    if (rdoInspClassF.Checked == true) { strInspClass = "F"; }
                    else if (rdoInspClassP.Checked == true) { strInspClass = "P"; }
                    else if (rdoInspClassR.Checked == true) { strInspClass = "R"; }
                    else if (rdoInspClassS.Checked == true) { strInspClass = "S"; }

                    string strQuery = " usp_QMA001  @pTYPE = 'S1'";
                    strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' ";
                    strQuery += ", @pINSP_REQ_DT_FR = '" + dtpInspReqDtFr.Text + "' ";
                    strQuery += ", @pINSP_REQ_DT_TO = '" + dtpInspReqDtTo.Text + "' ";
                    strQuery += ", @pINSP_DT_FR = '" + dtpInspDtFr.Text + "' ";
                    strQuery += ", @pINSP_DT_TO = '" + dtpInspDtTo.Text + "' ";
                    strQuery += ", @pRELEASE_DT_FR = '" + dtpReleaseDtFr.Text + "' ";
                    strQuery += ", @pRELEASE_DT_TO = '" + dtpReleaseDtTo.Text + "' ";
                    strQuery += ", @pPLANT_CD = '" + cboPlantCd.SelectedValue.ToString() + "' ";

                    strQuery += ", @pINSP_CLASS_CD = '" + strInspClass + "' ";

                    strQuery += ", @pINSP_YN = '" + strYn + "' ";
                    strQuery += ", @pPROJECT_NO = '" + txtProjectNo.Text + "' ";
                    strQuery += ", @pITEM_CD = '" + txtItemCd.Text + "' ";
                    strQuery += ", @pDECISION_CD = '" + cboDecision.SelectedValue.ToString() + "' ";
                    strQuery += ", @pBP_CD = '" + txtCustCd.Text + "' ";
                    strQuery += ", @pINSP_REQ_NO = '" + txtInspReqNo.Text + "'";
                    strQuery += ", @pWORKORDER_NO_FR = '" + txtWorkOrderNo_Fr.Text + "'";
                    strQuery += ", @pWORKORDER_NO_TO = '" + txtWorkOrderNo_To.Text + "'";

                    strQuery += ", @pPO_NO = '" + txtPoNo.Text + "'";
                    strQuery += ", @pMVMT_NO = '" + txtMvmtNo.Text + "'";
                    strQuery += ", @pWC_CD = '" + txtWcCd.Text + "'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0);

                    //화면 Locking
                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {
                        if (rdoInspClassP.Checked == true)
                        {
                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "완료여부")].Text == "Y")
                            {
                                UIForm.FPMake.grdReMake(fpSpread1, i,
                                    SystemBase.Base.GridHeadIndex(GHIdx1, "검사여부") + "|3"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "Release여부") + "|3"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "검사일") + "|3"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "Release일자") + "|3"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "판정") + "|3"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "검사수량") + "|3"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "불량수량") + "|3"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "비고") + "|3"
                                    );
                            }
                            else if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Release여부")].Text == "True")
                            {
                                UIForm.FPMake.grdReMake(fpSpread1, i,
                                    SystemBase.Base.GridHeadIndex(GHIdx1, "검사여부") + "|3"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "Release여부") + "|0"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "검사일") + "|3"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "Release일자") + "|1"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "판정") + "|3"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "검사수량") + "|3"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "불량수량") + "|3"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "비고") + "|3"
                                    );
                            }
                            else if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사여부")].Text == "True")
                            {
                                UIForm.FPMake.grdReMake(fpSpread1, i,
                                    SystemBase.Base.GridHeadIndex(GHIdx1, "검사여부") + "|0"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "Release여부") + "|0"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "검사일") + "|1"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "Release일자") + "|0"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "판정") + "|1"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "검사수량") + "|1"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "불량수량") + "|1"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "비고") + "|0"
                                    );
                            }
                            else
                            {
                                UIForm.FPMake.grdReMake(fpSpread1, i,
                                    SystemBase.Base.GridHeadIndex(GHIdx1, "검사여부") + "|0"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "Release여부") + "|3"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "검사일") + "|1"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "Release일자") + "|3"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "판정") + "|1"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "검사수량") + "|1"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "불량수량") + "|1"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "비고") + "|0"
                                    );
                            }
                        }
                        else
                        {
                            UIForm.FPMake.grdReMake(fpSpread1, i,
                                    SystemBase.Base.GridHeadIndex(GHIdx1, "검사여부") + "|3"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "Release여부") + "|3"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "검사일") + "|3"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "Release일자") + "|3"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "판정") + "|3"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "검사수량") + "|3"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "불량수량") + "|3"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "비고") + "|3"
                                    );
                        }

						if (string.IsNullOrEmpty(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "공급처명")].Text))
						{
							UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "증빙") + "|3");
						}

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

        #region SaveExec() 폼에 입력된 데이타 저장 로직
        protected override void SaveExec()
        {
            txtUser.Focus();

            //상단 그룹박스 필수 체크
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1) == true)
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox2) == true)
                {
                    if (fpSpread1.Sheets[0].Rows.Count > 0)
                    {
                        this.Cursor = Cursors.WaitCursor;

                        string ERRCode = "WR", MSGCode = "P0000"; //처리할 내용이 없습니다.
                        SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                        SqlCommand cmd = dbConn.CreateCommand();
                        SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                        try
                        {
                            /////////////////////////////////////////////// DETAIL 저장 시작 /////////////////////////////////////////////////
                            //그리드 상단 필수 체크
                            if (SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", false))
                            {

                                //행수만큼 처리
                                for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                                {
                                    string strHead = fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text;
                                    string strGbn = "";

                                    if (strHead.Length > 0)
                                    {
                                        switch (strHead)
                                        {
                                            case "U": strGbn = "U1"; break;
                                            case "I": strGbn = "I1"; break;
                                            case "D": strGbn = "D1"; break;
                                            default: strGbn = ""; break;
                                        }

                                        string strSql = " usp_QMA001 '" + strGbn + "'";
                                        strSql += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' ";

                                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사여부")].Locked == true)
                                            strSql += ", @pINSP_DIV = '' ";
                                        else if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사여부")].Text == "True")
                                        {
                                            strSql += ", @pINSP_DIV = 'Y' ";
                                            strSql += ", @pINSP_DT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사일")].Text + "' ";
                                        }
                                        else
                                            strSql += ", @pINSP_DIV = 'N' ";


                                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Release여부")].Locked == true)
                                            strSql += ", @pRELEASE_DIV = '' ";
                                        else if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Release여부")].Text == "True")
                                        {
                                            strSql += ", @pRELEASE_DIV = 'R' ";
                                            strSql += ", @pRELEASE_DT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Release일자")].Text + "' ";
                                        }
                                        else
                                            strSql += ", @pRELEASE_DIV = 'D' ";

                                        strSql += ", @pINSP_CLASS_CD  = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사분류")].Text + "' ";
                                        strSql += ", @pINSP_REQ_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사의뢰번호")].Text + "' ";
                                        strSql += ", @pDECISION_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "판정")].Value + "' ";
                                        strSql += ", @pINSP_QTY = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사수량")].Value + "' ";
                                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "불량수량")].Text != "")
                                        {
                                            strSql += ", @pDEFECT_QTY = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "불량수량")].Value + "' ";
                                        }
                                        else
                                        {
                                            strSql += ", @pDEFECT_QTY = 0.00 ";
                                        }
                                        strSql += ", @pINSPECTOR_CD = '" + txtUser.Text + "' ";
                                        strSql += ", @pRELEASER_CD = '" + txtReleaserCd.Text + "' ";
                                        strSql += ", @pREMARK = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "비고")].Text + "' ";
                                        strSql += ", @pWORKORDER_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "제조오더번호")].Text + "' ";
                                        strSql += ", @pPROC_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "공정순번")].Text + "' ";
                                        strSql += ", @pDN_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "출하번호")].Text + "' ";
                                        strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                                        strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                                        DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                                        ERRCode = ds.Tables[0].Rows[0][0].ToString();
                                        MSGCode = ds.Tables[0].Rows[0][1].ToString();

                                        if (ERRCode == "ER") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                                    }
                                }
                            }
                            else
                            {
                                Trans.Rollback();
                                this.Cursor = Cursors.Default;
                                return;
                            }
                            Trans.Commit();
                        }
                        catch (Exception e)
                        {
                            SystemBase.Loggers.Log(this.Name, e.ToString());
                            Trans.Rollback();
                            ERRCode = "ER";
                            MSGCode = e.Message;
                            //MSGCode = "P0001";	//에러가 발생되어 데이터 처리가 취소되었습니다.
                        }
                    Exit:
                        dbConn.Close();
                        if (ERRCode == "OK")
                        {
                            SearchExec();
                            MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else if (ERRCode == "ER")
                        {
                            MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        this.Cursor = Cursors.Default;
                    }
                }
            }
        }
        #endregion	

        #region fpSpread1_Change
        protected override void fpSpread1_ChangeEvent(int Row, int Column)
        {
            DialogResult dsMsg;
            try
            {

                if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "판정"))
                {
                    if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "검사여부")].Text == "True"
                        && fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "판정")].Value.ToString() == "N")

                        dsMsg = MessageBox.Show("검사여부가 체크되었기 때문에 미판정이 될 수 없습니다!", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "검사수량"))
                {
                    if (Convert.ToDouble(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "검사수량")].Value)
                        > Convert.ToDouble(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot크기")].Value))
                    {
                        dsMsg = MessageBox.Show("검사수량이 Lot크기 이하여야 합니다!", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "검사수량")].Value
                            = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "검사수량")].Value;
                    }
                }
                else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "불량수량"))
                {
                    if (Convert.ToDouble(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "검사수량")].Value)
                        < Convert.ToDouble(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "불량수량")].Value))
                    {
                        dsMsg = MessageBox.Show("불량수량 검사수량 이하여야 합니다!", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "불량수량")].Value = 0;
                    }
                    else
                    {
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "양품수량")].Value
                            = Convert.ToDouble(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot크기")].Value)
                              - Convert.ToDouble(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "불량수량")].Value);
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                dsMsg = MessageBox.Show(f.Message, SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        #endregion

        #region Grid Button Click
        private void fpSpread1_ButtonClicked(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "검사여부"))
            {
                if (fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "검사여부")].Text == "True")
                {
                    fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "검사일")].Text = dtpInspDt.Text;
                    fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "판정")].Value = "A";
                    fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "검사수량")].Value = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot크기")].Value;
                    fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "불량수량")].Value = 0;
                    UIForm.FPMake.grdReMake(fpSpread1, e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "검사일") + "|1");
                    UIForm.FPMake.fpChange(fpSpread1, e.Row);
                }
                else
                {
                    if (fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "Release여부")].Text == "True")
                    {
                        MessageBox.Show("Release가 선택되어있습니다 !", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "검사여부")].Value = 1;
                        return;
                    }
                    else
                    {
                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "검사일")].Text = "";
                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "판정")].Value = "N";
                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "검사수량")].Value = 0;
                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "불량수량")].Value = 0;
                        UIForm.FPMake.grdReMake(fpSpread1, e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "검사일") + "|0");
                        UIForm.FPMake.fpChange(fpSpread1, e.Row);
                    }
                }

            }
            else if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "Release여부"))
            {
                if (fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "Release여부")].Text == "True")
                {
                    if (fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "검사여부")].Text != "True")
                    {
                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "Release여부")].Value = 0;
                        MessageBox.Show("검사를 먼저하세요!", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                    {
                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "Release일자")].Text = dtpInspDt.Text;
                        UIForm.FPMake.grdReMake(fpSpread1, e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "Release일자") + "|1");
                        UIForm.FPMake.fpChange(fpSpread1, e.Row);
                    }
                }
                else
                {
                    fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "Release일자")].Text = "";
                    UIForm.FPMake.grdReMake(fpSpread1, e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "Release일자") + "|0");
                    UIForm.FPMake.fpChange(fpSpread1, e.Row);
                }

            }
			else if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "증빙"))
			{
				// 외주 공정의 경우만 증빙 조회
				if (string.IsNullOrEmpty(fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "공급처명")].Text))
				{
					
				}
				else
				{
					if (rdoInspClassR.Checked == true)
					{
						WNDW036 pu = new WNDW036();
						pu.strKEY_NO = fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "입고번호")].Text;
						pu.strKEY_SEQ = fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "입고순번")].Text;
						pu.strREQ_TYPE = "PO";
						pu.strDOC_TYPE = "PUR";
						pu.strFormGubn = "QMA003";

						pu.ShowDialog();
					}
					else
					{
						WNDW037 pu = new WNDW037();
						pu.strWORKORDER_NO = fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "제조오더번호")].Text;
						pu.strPROC_SEQ = fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "공정순번")].Text;
						pu.strPre_PROC_SEQ = fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "전공정")].Text;
						pu.strREQ_TYPE = "RP";
						pu.strDOC_TYPE = "OUT";
						pu.strFormGubn = "QMA001";

						pu.ShowDialog();
					}
				}

				fpSpread1.Sheets[0].RowHeader.Cells[e.Row, 0].Text = "";
			}
        }
        #endregion

        #region 그리드 Click 이벤트
        private void fpSpread1_CellClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            txtUser.Text = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "검사원")].Text;

            if (fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "검사원")].Text == "")
                txtUserNm.Value = "";

            txtReleaserCd.Text = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "담당자")].Text;

            if (fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "담당자")].Text == "")
                txtReleaserNm.Value = "";
        }
        #endregion

        #region 선택
        private void btnSelectAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
            {
                if (rdoD.Checked == true)
                {
                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사여부")].Text != "True"
                        && fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사여부")].Locked != true)
                    {
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사여부")].Value = 1;
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사일")].Text = dtpInspDt.Text;
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "판정")].Value = "A";
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사수량")].Value = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot크기")].Value;
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "불량수량")].Value = 0;
                        UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사일") + "|1");
                        UIForm.FPMake.fpChange(fpSpread1, i);
                    }
                }
                else
                {
                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Release여부")].Text != "True"
                        && fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Release여부")].Locked != true)
                    {
                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사여부")].Text == "True")
                        {
                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Release여부")].Value = 1;
                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Release일자")].Text = dtpInspDt.Text;
                            UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "Release일자") + "|1");
                            UIForm.FPMake.fpChange(fpSpread1, i);
                        }
                    }
                }
            }
        }

        private void btnSelectCancel_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
            {
                if (rdoD.Checked == true)
                {
                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사여부")].Text == "True"
                        && fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사여부")].Locked != true)
                    {
                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Release여부")].Text != "True")
                        {
                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사여부")].Value = 0;
                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사일")].Text = "";
                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "판정")].Value = "N";
                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사수량")].Value = 0;
                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "불량수량")].Value = 0;
                            UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사일") + "|0");
                            UIForm.FPMake.fpChange(fpSpread1, i);
                        }
                    }
                }
                else
                {
                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Release여부")].Text == "True"
                        && fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Release여부")].Locked != true)
                    {
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Release여부")].Value = 0;
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Release일자")].Text = "";
                        UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "Release일자") + "|0");
                        UIForm.FPMake.fpChange(fpSpread1, i);
                    }
                }
            }
        }
        #endregion

        #region lnkJump_Click 점프 클릭 이벤트
        private void lnkJump1_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (strJumpFileName1.Length > 0)
                {
                    string DllName = strJumpFileName1.Substring(0, strJumpFileName1.IndexOf("."));
                    string FrmName = strJumpFileName1.Substring(strJumpFileName1.IndexOf(".") + 1, strJumpFileName1.Length - strJumpFileName1.IndexOf(".") - 1);

                    for (int k = 0; k < this.MdiParent.MdiChildren.Length; k++)
                    {	// 폼이 이미 열려있으면 닫기
                        if (MdiParent.MdiChildren[k].Name == FrmName.Substring(0, 6))
                        {
                            MdiParent.MdiChildren[k].BringToFront();
                            MdiParent.MdiChildren[k].Close();
                            break;
                        }
                    }

                    Link1Exec();

                    Assembly ServiceAssembly = Assembly.LoadFile(SystemBase.Base.ProgramWhere.ToString() + "\\" + DllName + "." + FrmName.Substring(0, 6) + ".dll");
                    Form myForm = (Form)System.Activator.CreateInstance(ServiceAssembly.GetType(strJumpFileName1), param);
                    myForm.MdiParent = this.MdiParent;
                    myForm.Show();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "화면 링크"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 검사여부에 따른 날짜 컨트롤 세팅
        private void rdoInspY_CheckedChanged(object sender, EventArgs e)
        {
            dtpInspReqDtFr.Text = "";
            dtpInspReqDtTo.Text = "";
            dtpInspDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-3).ToString().Substring(0, 10);
            dtpInspDtTo.Text = SystemBase.Base.ServerTime("YYMMDD");
            dtpReleaseDtFr.Text = "";
            dtpReleaseDtTo.Text = "";
        }

        private void rdoInspN_CheckedChanged(object sender, EventArgs e)
        {
            dtpInspReqDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-3).ToString().Substring(0, 10);
            dtpInspReqDtTo.Text = SystemBase.Base.ServerTime("YYMMDD");
            dtpInspDtFr.Text = "";
            dtpInspDtTo.Text = "";
            dtpReleaseDtFr.Text = "";
            dtpReleaseDtTo.Text = "";
        }

        private void rdoInspR_CheckedChanged(object sender, EventArgs e)
        {
            dtpInspReqDtFr.Text = "";
            dtpInspReqDtTo.Text = "";
            dtpInspDtFr.Text = "";
            dtpInspDtTo.Text = "";
            dtpReleaseDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-3).ToString().Substring(0, 10);
            dtpReleaseDtTo.Text = SystemBase.Base.ServerTime("YYMMDD");
        }

        private void rdoInspAll_CheckedChanged(object sender, EventArgs e)
        {
            dtpInspReqDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-3).ToString().Substring(0, 10);
            dtpInspReqDtTo.Text = SystemBase.Base.ServerTime("YYMMDD");
            dtpInspDtFr.Text = "";
            dtpInspDtTo.Text = "";
            dtpReleaseDtFr.Text = "";
            dtpReleaseDtTo.Text = "";
        }
        #endregion
    }
}
