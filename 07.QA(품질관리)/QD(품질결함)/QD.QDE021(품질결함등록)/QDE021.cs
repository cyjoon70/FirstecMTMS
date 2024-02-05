
#region 작성정보
/*********************************************************************/
// 단위업무명 : 품질결함등록
// 작 성 자 : 권순철
// 작 성 일 : 2013-04-03
// 작성내용 : 품질결함등록 및 관리
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
using System.Threading;
using System.IO;

namespace QD.QDE021
{
    public partial class QDE021 : UIForm.FPCOMM1
    {
        #region 변수선언
        string strInspReqNo = "";
        Thread th;
        UIForm.ExcelWaiting Waiting_Form = null;
        #endregion

        #region 생성자
        public QDE021()
        {
            InitializeComponent();
        }

        public QDE021(string InspReqNo)
        {
            InitializeComponent();
            strInspReqNo = InspReqNo;
        }
        #endregion

        #region Form Load 시
        private void QDE021_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            //콤보박스 세팅
            SystemBase.ComboMake.C1Combo(cboPlantCd, "usp_B_COMMON @pType='TABLE', @pCODE = 'PLANT_CD', @pNAME = 'PLANT_NM', @pSPEC1 = 'B_PLANT_INFO', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 0);//공장

            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "청구")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'B029', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "폐기여부")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'B029', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "반납여부")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'B029', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "재입고여부")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'B029', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);
            //그리드초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

            //기타 세팅
            cboPlantCd.SelectedValue = SystemBase.Base.gstrPLANT_CD;
            dtQNCNO_FR.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-3);
            dtQNCNO_TO.Value = SystemBase.Base.ServerTime("YYMMDD");
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            fpSpread1.Sheets[0].Rows.Count = 0;

            //기타 세팅
            cboPlantCd.SelectedValue = SystemBase.Base.gstrPLANT_CD;
            dtQNCNO_FR.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-3);
            dtQNCNO_TO.Value = SystemBase.Base.ServerTime("YYMMDD");
        }
        #endregion

        #region SearchExec()
        protected override void SearchExec()
        {
            //조회조건 필수 체크
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1) == true)
            {
                this.Cursor = Cursors.WaitCursor;

                try
                {
                    // 2020.08.14. hma 추가(Start)
                    string strLoginUsrId = "";
                    string strInspUsrId = "";
                    strLoginUsrId = SystemBase.Base.gstrUserID.ToString();
                    // 2020.08.14. hma 추가(End)

                    string EXM_YN = "";
                    string strQuery = " usp_QDE021  @pTYPE = 'S1'";
                    strQuery += ", @pQNC_NO = '" + txtQNC_NO.Text + "' ";
                    strQuery += ", @pQNCNO_DT_FR = '" + dtQNCNO_FR.Text + "' ";
                    strQuery += ", @pQNCNO_DT_TO = '" + dtQNCNO_TO.Text + "' ";
                    strQuery += ", @pITEM_CD = '" + txtItemCd.Text + "' ";
                    strQuery += ", @pPROJECT_NO = '" + txtProjectNo.Text + "'";
                    strQuery += ", @pPROJECT_SEQ = '" + txtProjectSeq.Text + "'";
                    strQuery += ", @pDEPT_CD = '" + txtDeptCd.Text + "'";//귀책부서
                    strQuery += ", @pPLANT_CD = '" + cboPlantCd.SelectedValue.ToString() + "' ";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    if (rdoEXAMINATION_ALL.Checked == true)
                        EXM_YN = "";

                    if (rdoEXAMINATION_Y.Checked == true)
                        EXM_YN = "Y";
                    else if (rdoEXAMINATION_N.Checked == true)
                        EXM_YN = "N";

                    strQuery += ", @pRELEASE_YN = '" + EXM_YN + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false, false);

                    if (fpSpread1.Sheets[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                        {
                            strInspUsrId = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사원사번")].Text.ToString();        // 2020.08.14. hma 추가
                            for (int j = 0; j < fpSpread1.Sheets[0].ColumnCount; j++)
                            {
                                if (fpSpread1.Sheets[0].Columns[j].CellType.ToString() == "CheckBoxCellType")
                                { 
                                    fpSpread1.Sheets[0].Columns[j].Locked = true;
                                }
                            }

                            // 2020.08.14. hma 추가(Start): 로그인한 사용자와 검사자가 동일한 경우에만 재입고여부를 수정 가능하도록 함.
                            if ((strLoginUsrId.ToUpper() != "ADMIN") && (strLoginUsrId.ToUpper() != strInspUsrId.ToUpper()))
                            {
                                fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "재입고여부")].Locked = true;
                                fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "재입고여부")].BackColor = System.Drawing.Color.FromArgb(238, 238, 238);
                            }
                            // 2020.08.14. hma 추가(End)
                        }
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //데이터 조회 중 오류가 발생하였습니다.
                }

                this.Cursor = Cursors.Default;
            }
        }
        #endregion

        #region 행추가
        protected override void RowInsExec()
        {// 행 추가
            QDE021P1 myForm = new QDE021P1();

            myForm.ShowDialog();

            if (myForm.DialogResult == DialogResult.OK)
                SearchExec();
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
                string strQuery = " usp_B_COMMON 'PROJ_SEQ', @pSPEC1 = '" + txtProjectNo.Text + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";										// 쿼리
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };			// 쿼리 인자값(조회조건)
                string[] strSearch = new string[] { "", "" };		// 쿼리 인자값에 들어갈 데이타

                //UIForm.PopUpSP pu = new UIForm.PopUpSP(strQuery, strWhere, strSearch, PHeadText7, PTxtAlign7, PCellType7, PHeadWidth7, PSearchLabel7);
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


        //품목코드
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
        private void btnDeptCd_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON 'COMM_POP' ,@pSPEC1='Q026', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtDeptCd.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00093", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "귀책부서 조회");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtDeptCd.Text = Msgs[0].ToString();
                    txtDeptNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnProjectSeq_Click_1(object sender, EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON 'PROJ_SEQ', @pSPEC1 = '" + txtProjectNo.Text + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";										// 쿼리
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };			// 쿼리 인자값(조회조건)
                string[] strSearch = new string[] { "", "" };		// 쿼리 인자값에 들어갈 데이타

                //UIForm.PopUpSP pu = new UIForm.PopUpSP(strQuery, strWhere, strSearch, PHeadText7, PTxtAlign7, PCellType7, PHeadWidth7, PSearchLabel7);
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

        private void btnProjectNo_Click_1(object sender, EventArgs e)
        {
            try
            {
                WNDW.WNDW007 pu = new WNDW.WNDW007(txtProjectNo.Text);
                pu.MaximizeBox = false;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

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

        private void btnItemCd_Click_1(object sender, EventArgs e)
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


        #endregion

        #region TextChanged
        //프로젝트번호
        private void txtProjectNo_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtProjectNo.Text != "")
                {
                    txtProjectNm.Value = SystemBase.Base.CodeName("PROJECT_NO", "PROJECT_NM", "S_SO_MASTER", txtProjectNo.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtProjectNm.Value = "";
                    txtProjectSeq.Text = "";
                }
            }
            catch { }
        }

        //품목코드
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

        //귀책부서
        private void txtDeptCd_TextChanged(object sender, EventArgs e)
        {
            txtDeptNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtDeptCd.Text, " AND MAJOR_CD = 'Q026'  AND COMP_CODE ='" + SystemBase.Base.gstrCOMCD + "'");
        }

        #endregion

        #region 결과원인등록
        //결과원인등록
        private void butNonDisInsert_Click(object sender, EventArgs e)
        {

            int iRow = fpSpread1.ActiveSheet.ActiveRowIndex;

            if (iRow > -1)
            {
                QDE021P1 myForm = new QDE021P1(fpSpread1, fpSpread1.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx1, "QNC NO")].Text, fpSpread1.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx1, "처리결과")].Text);

                myForm.ShowDialog();

                if (myForm.DialogResult == DialogResult.OK)
                    SearchExec();
            }
        }
        #endregion

        #region SaveExec() 폼에 입력된 데이타 저장 로직
        protected override void SaveExec()
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            if (SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", true) == true) // 그리드 상단 필수항목 체크
            {
                string ERRCode = "ER", MSGCode = "P0000";	//처리할 내용이 없습니다.
                string strKeyCd = "";

                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                try
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
                                case "U": strGbn = "U2"; break;
                                case "D": strGbn = "D1"; break;
                                default: strGbn = ""; break;
                            }

                            strKeyCd = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "변경전ID")].Text.ToString();
                            string strQncNO = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "QNC NO")].Text.ToString();
                            string strSql = "";
                            if (strGbn == "D1")
                            {
                                if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "결함원인")].Text == "True" || fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "기술판정")].Text == "True" || fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품질판정")].Text == "True" || fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "처리결과")].Text == "True")
                                {
                                    MessageBox.Show(SystemBase.Base.MessageRtn("삭제할수없습니다."), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);

                                    Trans.Rollback();
                                    return;
                                }

                                strSql = " usp_QDE021 '" + strGbn + "'";
                                strSql = strSql + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                                strSql = strSql + ", @pQNC_NO = '" + strQncNO + "'";
                            }
                            else if (strGbn == "U2")
                            {
                                strSql = " usp_QDE021 '" + strGbn + "'";
                                strSql = strSql + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                                strSql = strSql + ", @pQNC_NO = '" + strQncNO + "'";
                                strSql = strSql + ", @pCLAIM = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "청구")].Text.ToString() + "'";
                                strSql = strSql + ", @pREINSPECTOR_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사자")].Text.ToString() + "'";
                                strSql = strSql + ", @pPASS_QTY = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "합격수량")].Text.ToString() + "'";

                                if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사의뢰일")].Text.ToString() == "")
                                    strSql = strSql + ", @pEXAMINATION_REQUEST_DT = '" +"null" + "'";
                                else
                                    strSql = strSql + ", @pEXAMINATION_REQUEST_DT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사의뢰일")].Text.ToString() + "'";

                                strSql = strSql + ", @pEXAMINATION_REQUEST_QTY = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사의뢰수량")].Text.ToString() + "'";
                                strSql = strSql + ", @pREMARK1 = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "비고1")].Text.ToString() + "'";
                                strSql = strSql + ", @pREMARK2 = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "비고2")].Text.ToString() + "'";
                                strSql = strSql + ", @pRETURN_YN = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "부적합품반납여부")].Text.ToString() + "'";      // 2021.02.09. hma 수정: 반납여부 => 부적합품반납여부로. 그리드디자인 항목명 기준으로 변경.
                                strSql = strSql + ", @pDISUSE_YN = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "폐기여부")].Text.ToString() + "'";
                                strSql = strSql + ", @pEXAMINATION_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사자")].Text.ToString() + "'";
                                if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "재입고여부")].Text.ToString() == "Y")
                                {
                                    strSql = strSql + ", @pRESTOCK_YN = '" +"Y" + "'";
                                }
                                if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "배포")].Text.ToString() == "True")      // 2021.02.09. hma 수정: 검사결과 => 배포로. 그리드디자인 항목명 기준으로 변경. 
                                {
                                    strSql = strSql + ", @pEXAMINATION_YN = '" + "Y" + "'";
                                }
                                if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Release 일자")].Text.ToString() != "")
                                {
                                    strSql = strSql + ", @pRELEASE_DT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "RELEASE 일자")].Text.ToString() + "'";
                                    strSql = strSql + ", @pRELEASE_YN = '" + "Y" + "'";
                                }
                                //2020.09.22. ksh 수정(Start) : 저장되는 항목 추가
                                strSql = strSql + ", @pREMARK3 = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "비고3")].Text.ToString() + "'";
                                strSql = strSql + ", @pREMARK4 = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "비고4")].Text.ToString() + "'";
                                strSql = strSql + ", @pRECEIPT_DT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "구매접수일자")].Text.ToString() + "'";
                                strSql = strSql + ", @pLOT_QTY = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot수량")].Text.ToString() + "'";
                                strSql = strSql + ", @pSERIAL_NUMBER = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "일련번호")].Text.ToString() + "'";
                                strSql = strSql + ", @pPROCESS_ENT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "공정명/업체명")].Text.ToString() + "'";
                                strSql = strSql + ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";       // 2021.02.15. hma 추가: 변경자ID 저장되도록 함. 
                                //2020.09.22. ksh 수정(End)
                            }

                            DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                            ERRCode = ds.Tables[0].Rows[0][0].ToString();
                            MSGCode = ds.Tables[0].Rows[0][1].ToString();

                            if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
                        }
                    }
                    Trans.Commit();
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    Trans.Rollback();
                    MSGCode = "P0001";	//에러가 발생하여 데이터 처리가 취소되었습니다.
                }
            Exit:
                dbConn.Close();

                if (ERRCode == "OK")
                {
                    SearchExec();
                    UIForm.FPMake.GridSetFocus(fpSpread1, strKeyCd); //그리드 위치를 가져온다

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
            }
            this.Cursor = System.Windows.Forms.Cursors.Default;
        }
        #endregion
        private void Show_Waiting()
        {
            Waiting_Form = new UIForm.ExcelWaiting("품질결함표출력...");
            Waiting_Form.ShowDialog();
        }

        private void butPriview_Click(object sender, EventArgs e)
        {
            int iRow1 = fpSpread1.ActiveSheet.ActiveRowIndex;

            if (fpSpread1.Sheets[0].Rows.Count > 0)
            {
                string strSheetPage1 = "품질결함표";
                int iRow = fpSpread1.ActiveSheet.ActiveRowIndex;
                string strFileName = SystemBase.Base.ProgramWhere + @"\Report\품질결함표.xls";

                try
                {
                    CheckForIllegalCrossThreadCalls = false;

                    th = new Thread(new ThreadStart(Show_Waiting));
                    th.Start();
                    Thread.Sleep(200);
                    Waiting_Form.Activate();

                    string strQuery = " usp_QDE025  @pTYPE = 'R1'";
                    strQuery += ", @pQNC_NO = '" + fpSpread1.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx1, "QNC NO")].Text + "' ";
                    strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' ";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

                    if (dt.Rows.Count > 0)
                    {
                        Waiting_Form.progressBar_temp.Maximum = dt.Rows.Count;

                        UIForm.VkExcel excel = null;

                        if (File.Exists(strFileName))
                        {
                            File.SetAttributes(strFileName, System.IO.FileAttributes.ReadOnly);
                        }
                        else
                        {
                            // 엑셀 데이터를 생성할 수 없습니다. 원본 파일이 존재하지 않습니다.
                            MessageBox.Show("엑셀 데이터를 생성할 수 없습니다. 원본 파일이 존재하지 않습니다."); ;
                            return;
                        }

                        excel = new UIForm.VkExcel(false);

                        excel.OpenFile(strFileName);
                        // 현재 시트 선택
                        excel.FindExcelWorksheet(strSheetPage1);

                        // 엑셀쓰기---------------------------------------------------------

                        excel.SetCell(1, 3, dt.Rows[0]["QNC_NO"].ToString());
                        excel.SetCell(3, 3, dt.Rows[0]["IN_DT"].ToString());
                        excel.SetCell(2, 16, dt.Rows[0]["INSPECTOR_NM"].ToString());
                        excel.SetCell(2, 18, dt.Rows[0]["EXAMINER_NM"].ToString());
                        excel.SetCell(2, 20, dt.Rows[0]["APPROVER_NM"].ToString());

                        excel.SetCell(4, 16, dt.Rows[0]["QDEF_ENT_DT"].ToString());
                        excel.SetCell(4, 18, dt.Rows[0]["QDEC_ENT_DT"].ToString());
                        excel.SetCell(4, 20, dt.Rows[0]["HRES_ENT_DT"].ToString());

                        excel.SetCell(7, 1, dt.Rows[0]["EXAMINATION_CD"].ToString());
                        excel.SetCell(7, 4, dt.Rows[0]["ORDER_NO"].ToString());
                        excel.SetCell(7, 6, dt.Rows[0]["LOT_SEQ"].ToString());
                        excel.SetCell(7, 7, dt.Rows[0]["PROJECT_NO"].ToString());
                        excel.SetCell(7, 11, dt.Rows[0]["ITEM_CD"].ToString());
                        excel.SetCell(7, 15, dt.Rows[0]["OP_NO"].ToString());


                        excel.SetCell(11, 1, dt.Rows[0]["PROJECT_NM"].ToString());
                        excel.SetCell(11, 5, dt.Rows[0]["ITEM_NM"].ToString());
                        excel.SetCell(11, 8, dt.Rows[0]["ITEM_SPEC"].ToString());
                        excel.SetCell(11, 14, dt.Rows[0]["SERIAL_NO"].ToString());
                        excel.SetCell(11, 17, dt.Rows[0]["INSP_CLASS_CD"].ToString());
                        excel.SetCell(11, 19, dt.Rows[0]["DEFECT_QTY"].ToString());


                        excel.SetCell(15, 2, dt.Rows[0]["QDEF_CONTENT"].ToString());
                        excel.SetCell(15, 9, dt.Rows[0]["DCAU_CONTENT"].ToString());

                        excel.SetCell(25, 9, dt.Rows[0]["PREV_CONTENT"].ToString());
                        excel.SetCell(30, 2, dt.Rows[0]["QPROC_CONTENT"].ToString());

                        excel.SetCell(35, 8, dt.Rows[0]["DEPT_NM"].ToString());

                        excel.SetCell(35, 13, dt.Rows[0]["WC_NM"].ToString());
                        excel.SetCell(35, 17, dt.Rows[0]["WORKER_CD"].ToString());
                        excel.SetCell(35, 19, dt.Rows[0]["MANAGER_CD"].ToString());

                        excel.SetCell(37, 17, dt.Rows[0]["QDEF_ENT_DT"].ToString());
                        excel.SetCell(37, 19, dt.Rows[0]["QDEF_ENT_DT"].ToString());


                        excel.SetCell(41, 2, dt.Rows[0]["TDEC_CONTENT"].ToString());
                        excel.SetCell(47, 2, dt.Rows[0]["QDEC_CONTENT"].ToString());
                        excel.SetCell(54, 2, dt.Rows[0]["HRES_CONTENT"].ToString());


                        excel.SetCell(42, 10, dt.Rows[0]["DEFECT_CD"].ToString());

                        excel.SetCell(42, 13, dt.Rows[0]["DEFECT_QTY"].ToString());


                        Waiting_Form.label_temp.Text = "완료되었습니다.";
                        Thread.Sleep(500);
                        excel.ShowExcel(true);
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품질결함표출력"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    Waiting_Form.Close();
                    th.Abort();
                    File.SetAttributes(strFileName, System.IO.FileAttributes.Normal);
                }
            }
        }



        protected override void fpChange()
        {
            try
            {
                int childItemCdCol = SystemBase.Base.GridHeadIndex(GHIdx1, "재입고여부");

                for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                {
                    string strHead = fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text;
                        string strGbn = "";
                        if (strHead.Length > 0)
                        {
                            if (strHead == "U")
                            {
                                if (childItemCdCol == SystemBase.Base.GridHeadIndex(GHIdx1, "재입고여부"))
                                {
                                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "재입고여부")].Text.ToString() == "Y")
                                    {
                                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "청구")].Text = "N";
                                    }
                                    else
                                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "청구")].Text = "Y";
                                }

                            }
                        }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY008", "그리드 EditChange 이벤트"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
    }
}
