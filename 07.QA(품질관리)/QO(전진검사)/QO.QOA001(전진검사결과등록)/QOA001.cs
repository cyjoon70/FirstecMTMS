#region 작성정보
/*********************************************************************/
// 단위업무명 : 전진검사결과등록
// 작 성 자 : 한 미 애
// 작 성 일 : 2017.11.29. 
// 작성내용 : 전진검사등록
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

namespace QO.QOA001
{
    public partial class QOA001 : UIForm.FPCOMM1
    {
        #region 변수선언

        #endregion

        #region 생성자
        public QOA001()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load시
        private void QOA001_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            SystemBase.Validation.GroupBox_Setting(groupBox2);

            //콤보박스 세팅
            SystemBase.ComboMake.C1Combo(cboPlantCd, "usp_B_COMMON @pType='TABLE', @pCODE = 'PLANT_CD', @pNAME = 'PLANT_NM', @pSPEC1 = 'B_PLANT_INFO', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 0);  //공장
            SystemBase.ComboMake.C1Combo(cboInspType, "usp_B_COMMON @pType='COMM', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "',@pCODE = 'Q035', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9);    //검사항목
            SystemBase.ComboMake.C1Combo(cboInspSide, "usp_B_COMMON @pType='COMM', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "',@pCODE = 'Q036', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9);    //검사구분

            //그리드 콤보박스 세팅			
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "전진검사원")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM1', @pCODE = 'Q037', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ", 0);
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "품보원")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM1', @pCODE = 'Q030', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ", 0);
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "검사결과")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'Q004', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ", 0);

            //그리드초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

            //기타 세팅
            dtpInspDemandDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-3).ToString().Substring(0, 10);
            dtpInspDemandDtTo.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(+1).ToString().Substring(0, 10); //SystemBase.Base.ServerTime("YYMMDD");
            rdoInspN.Checked = true;

            dtpInspDt.Text = SystemBase.Base.ServerTime("YYMMDD");

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

        // 전진검사원
        private void btnUser_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON 'COMM_POP' ,@pSPEC1='Q037', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtUser.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00067", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "전진검사원 팝업");
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

        // btnProj_Click(): 프로젝트번호 클릭시 팝업창 띄우기
        private void btnProj_Click(object sender, EventArgs e)
        {
            try
             {
                string strQuery = " usp_M_COMMON 'P001', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";   // 쿼리
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };			// 쿼리 인자값(조회조건)
                string[] strSearch = new string[] { txtProjectNo.Text, "" };		// 쿼리 인자값에 들어갈 데이타

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

        // btnProjectSeq_Click(): 프로젝트차수 클릭시 팝업창 띄우기
        private void btnProjectSeq_Click(object sender, EventArgs e)
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

        // btnWorkOrderNo_Fr_Click(): 제조오더번호 클릭시 팝업창 띄우기
        private void btnWorkOrderNo_Fr_Click(object sender, EventArgs e)
        {
            try
            {
                WNDW006 pu = new WNDW006(txtWorkOrderNo.Text);
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtWorkOrderNo.Text = Msgs[1].ToString();
                    txtWorkOrderNo.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "제조오더번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // btnPoNo_Click(): 발주번호 클릭시 팝업창 띄우기
        private void btnPoNo_Click(object sender, EventArgs e)
        {
            try
            {
                WNDW018 pu = new WNDW018();
                pu.MaximizeBox = false;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtPoNo.Text = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        // btnInspReqNo_Click(): 검사요청번호 클릭시 팝업창 띄우기
        private void btnInspReqNo_Click(object sender, EventArgs e)
        {
            try
            {
                WNDW041 pu = new WNDW041(Convert.ToString(cboPlantCd.SelectedValue), txtInspReqNo.Text, txtCustCd.Text, dtpInspDemandDtFr.Text, dtpInspDemandDtTo.Text);
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
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "검사요청번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // btnGoInspector_Click(): 전진검사원 클릭시 팝업창 띄우기
        private void btnGoInspector_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON 'COMM_POP' ,@pSPEC1='Q037', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtGoInspector.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00067", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "전진검사원 팝업");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtGoInspector.Text = Msgs[0].ToString();
                    txtGoInspectorNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        // btnQCDuty_Click(): 품보원 클릭시 팝업창 띄우기
        private void btnQCDuty_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON 'COMM_POP' ,@pSPEC1='Q030', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtQCDuty.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00067", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "품보원 팝업");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtQCDuty.Text = Msgs[0].ToString();
                    txtQCDutyNm.Value = Msgs[1].ToString();
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

        //공급처
        private void txtCustCd_TextChanged(object sender, EventArgs e)
        {
            txtCustNm.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtCustCd.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
        }
        #endregion

        #region NewExec()
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            SystemBase.Validation.GroupBox_Reset(groupBox2);

            fpSpread1.Sheets[0].Rows.Count = 0;

            //기타 세팅
            dtpInspDemandDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-3).ToString().Substring(0, 10);
            dtpInspDemandDtTo.Text = SystemBase.Base.ServerTime("YYMMDD");

            rdoInspN.Checked = true;
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
                    string strYn = "A";
                    if (rdoInspY.Checked == true) { strYn = "Y"; }
                    else if (rdoInspN.Checked == true) { strYn = "N"; }

                    string strQuery = " usp_QOA001  @pTYPE = 'S1'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' ";
                    strQuery += ", @pCUST_CD = '" + txtCustCd.Text + "'";
                    strQuery += ", @pPO_NO = '" + txtPoNo.Text + "'";
                    strQuery += ", @pINSP_DEMAND_DT_FR = '" + dtpInspDemandDtFr.Text + "'";     // 검사요청일FROM
                    strQuery += ", @pINSP_DEMAND_DT_TO = '" + dtpInspDemandDtTo.Text + "'";     // 검사요청일TO
                    strQuery += ", @pINSP_SCHD_DT_FR = '" + dtpInspSchdDtFr.Text + "'";         // 전진검사예정일FROM
                    strQuery += ", @pINSP_SCHD_DT_TO = '" + dtpInspSchdDtTo.Text + "'";         // 전진검사예정일TO
                    strQuery += ", @pINSP_DT_FR = '" + dtpInspDtFr.Text + "'";      // 전진검사일FROM
                    strQuery += ", @pINSP_DT_TO = '" + dtpInspDtTo.Text + "'";      // 전진검사일TO
                    strQuery += ", @pPO_DT_FR = '" + dtpPoDtFr.Text + "'";
                    strQuery += ", @pPO_DT_TO = '" + dtpPoDtTo.Text + "'";
                    strQuery += ", @pDELIVERY_DT_FR = '" + dtpPODelivDtFr.Text + "'";
                    strQuery += ", @pDELIVERY_DT_TO = '" + dtpPODelivDtTo.Text + "'";
                    strQuery += ", @pPROJECT_NO = '" + txtProjectNo.Text + "' ";
                    strQuery += ", @pPROJECT_SEQ = '" + txtProjectSeq.Text + "' ";
                    strQuery += ", @pWORKORDER_NO = '" + txtWorkOrderNo.Text + "' ";
                    strQuery += ", @pITEM_CD = '" + txtItemCd.Text + "' ";
                    strQuery += ", @pINSP_CLASS_CD = '" + cboInspType.SelectedValue.ToString() + "' ";
                    strQuery += ", @pINSP_SIDE_CD = '" + cboInspSide.SelectedValue.ToString() + "' ";
                    strQuery += ", @pINSP_YN = '" + strYn + "'";
                    strQuery += ", @pINSP_REQ_NO = '" + txtInspReqNo.Text + "'";
                    strQuery += ", @pINSPECTOR_CD = '" + txtQCDuty.Text + "'";
                    strQuery += ", @pPROG_INSPECTOR_CD = '" + txtGoInspector.Text + "'";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0);
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

                                        string strQuery = " usp_QOA001";
                                        strQuery += " @pTYPE = '" + strGbn + "'";
                                        strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                                        strQuery += ", @pINSP_REQ_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사요청번호")].Text + "'";
                                        strQuery += ", @pPO_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "발주번호")].Text + "'";
                                        strQuery += ", @pPO_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "발주순번")].Text + "'";
                                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "선택")].Text == "True")
                                        {
                                            strQuery += ", @pINSP_CHK = 'Y' ";
                                            strQuery += ", @pINSP_QTY = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사요청수량")].Text + "'";
                                            strQuery += ", @pINSP_PROG_SCHED_DT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "전진검사예정일")].Value.ToString() + "'";
                                            // 2018.01.10. hma 수정(Sart): 전진검사일, 전진검사원, 품보원, 검사결과를 업체에서 등록하는걸로 요청하여 주석 처리함.
                                            //strQuery += ", @pINSP_DT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "전진검사일")].Value + "'";
                                            //strQuery += ", @pDECISION_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사결과")].Value.ToString() + "'";
                                            //strQuery += ", @pINSPECTOR_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품보원")].Value.ToString() + "'";
                                            //strQuery += ", @pPROG_INSPECTOR_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "전진검사원")].Value.ToString() + "'";
                                            //strQuery += ", @pPROG_RATE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "진척율")].Text + "'";
                                            // 2018.01.10. hma 수정(End)
                                        }
                                        else
                                        {
                                            strQuery += ", @pINSP_CHK = 'N' ";
                                            
                                        }
                                        strQuery += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";

                                        DataSet ds = SystemBase.DbOpen.TranDataSet(strQuery, dbConn, Trans);
                                        ERRCode = ds.Tables[0].Rows[0][0].ToString();
                                        MSGCode = ds.Tables[0].Rows[0][1].ToString();

                                        if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }
                                       // ER 코드 Return시 점프
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
                if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "검사결과"))
                {
                    if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "검사여부")].Text == "True"
                        && fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "검사결과")].Value.ToString() == "N")

                        dsMsg = MessageBox.Show("검사여부가 체크되었기 때문에 미판정이 될 수 없습니다!", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "진척율"))
                {
                    if (Convert.ToDouble(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "진척율")].Value) > 100)
                    {
                        dsMsg = MessageBox.Show("진척율은 100을 넘을 수 없습니다!", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
            if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "선택"))
            {
                if (fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "선택")].Text == "True")
                {
                    //if (fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "전진검사일")].Text == "")
                    //    fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "전진검사일")].Text = dtpInspDt.Text;
                    //if (fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "전진검사원")].Text == "")
                    //    fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "전진검사원")].Value = txtUser.Text;
                    //if (fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "검사결과")].Value.ToString() == "N")        // 미판정 상태이면
                    //    fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "검사결과")].Value = "A";

                    if (fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "전진검사예정일")].Text == "")
                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "전진검사예정일")].Text = dtpInspDt.Text;

                    UIForm.FPMake.grdReMake(fpSpread1, e.Row,
                                SystemBase.Base.GridHeadIndex(GHIdx1, "전진검사예정일") + "|1"
                                //+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "전진검사일") + "|1"
                                //+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "전진검사원") + "|1"
                                //+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "품보원") + "|1"
                                //+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "검사결과") + "|1"
                            );
                    UIForm.FPMake.fpChange(fpSpread1, e.Row);
                }
                else
                {
                    fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "전진검사예정일")].Text = "";
                    fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "전진검사일")].Text = "";
                    fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "전진검사원")].Text = "";
                    fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "검사결과")].Value = "N";
                    fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "진척율")].Value = "0";

                    UIForm.FPMake.grdReMake(fpSpread1, e.Row,
                                SystemBase.Base.GridHeadIndex(GHIdx1, "전진검사예정일") + "|0"
                                //+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "전진검사일") + "|0"
                                //+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "전진검사원") + "|0"
                                //+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "품보원") + "|0"
                                //+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "검사결과") + "|0"
                            );
                    UIForm.FPMake.fpChange(fpSpread1, e.Row);
                }
            }
        }
        #endregion

        #region 그리드 Click 이벤트
        private void fpSpread1_CellClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            //txtUser.Text = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "검사원")].Text;

            //if (fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "검사원")].Text == "")
            //    txtUserNm.Value = "";
        }
        #endregion

        #region 선택
        private void btnSelectAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
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

        private void btnSelectCancel_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
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
        #endregion

        #region 검사여부에 따른 날짜 컨트롤 세팅
        private void rdoInspY_CheckedChanged(object sender, EventArgs e)
        {
            dtpInspDemandDtFr.Text = "";
            dtpInspDemandDtTo.Text = "";
            dtpInspDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-3).ToString().Substring(0, 10);
            dtpInspDtTo.Text = SystemBase.Base.ServerTime("YYMMDD");
        }

        private void rdoInspN_CheckedChanged(object sender, EventArgs e)
        {
            dtpInspDemandDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-3).ToString().Substring(0, 10);
            //dtpInspReqDtTo.Text = SystemBase.Base.ServerTime("YYMMDD");
            dtpInspDemandDtTo.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(+1).ToString().Substring(0, 10);
            dtpInspDtFr.Text = "";
            dtpInspDtTo.Text = "";
        }

        private void rdoInspAll_CheckedChanged(object sender, EventArgs e)
        {
            dtpInspDemandDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-3).ToString().Substring(0, 10);
            //dtpInspReqDtTo.Text = SystemBase.Base.ServerTime("YYMMDD");
            dtpInspDemandDtTo.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(+1).ToString().Substring(0, 10);
            dtpInspDtFr.Text = "";
            dtpInspDtTo.Text = "";
        }
        #endregion

    }
}
