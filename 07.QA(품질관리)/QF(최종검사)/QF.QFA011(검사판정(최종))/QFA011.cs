#region 작성정보
/*********************************************************************/
// 단위업무명 : 검사판정(최종)
// 작 성 자 : 김 현 근
// 작 성 일 : 2013-04-04
// 작성내용 : 검사판정(최종) 및 관리
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

namespace QF.QFA011
{
    public partial class QFA011 : UIForm.FPCOMM2
    {
        #region 변수선언
        string strInspReqNo = "";
        int SearchRow = 0;
        int SearchColumn = 0;
        string strInspStatus = "";
        string strStatus = "";
        string strPlantCd = "";
        string strInspReqDt = "";
        bool Linked = false;
        bool bLotDefect = false; // LOT 품목이면 불량수량을 LOT에 할당했는지 여부
        Thread th;
        UIForm.ExcelWaiting Waiting_Form = null;
        #endregion

        #region 생성자
        public QFA011()
        {
            InitializeComponent();
        }

        public QFA011(string param1, string param2, string param3, string param4)
        {
            strInspReqNo = param1;
            strPlantCd = param2;
            strInspReqDt = param3;
            strInspStatus = param4;
            Linked = true;
            
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void QFA011_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            SystemBase.Validation.GroupBox_Setting(groupBox3);

            //콤보박스 세팅
            SystemBase.ComboMake.C1Combo(cboSPlantCd, "usp_B_COMMON @pType='TABLE', @pCODE = 'PLANT_CD', @pNAME = 'PLANT_NM', @pSPEC1 = 'B_PLANT_INFO', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//공장
            SystemBase.ComboMake.C1Combo(cboSInspStatus, "usp_B_COMMON @pType='COMM2', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "',@pCODE = 'Q003', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0); //검사진행상태
            SystemBase.ComboMake.C1Combo(cboSDecisionCd, "usp_B_COMMON @pType='COMM', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "',@pCODE = 'Q004', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 9); //판정
            SystemBase.ComboMake.C1Combo(cboDecisionCd, "usp_B_COMMON @pType='COMM', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "',@pCODE = 'Q004', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 9); //판정


            //그리드콤보박스 세팅
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "판정")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'Q004', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0); //판정

            //그리드초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
            UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, 0, 0, false);

            //기타 세팅
            cboSPlantCd.SelectedValue = SystemBase.Base.gstrPLANT_CD;
            dtpSInspReqDtFr.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-3);
            dtpSInspReqDtTo.Value = SystemBase.Base.ServerTime("YYMMDD");
            dtpSInspDtFr.Value = null;
            dtpSInspDtTo.Value = null;
            cboSInspStatus.SelectedValue = "V";
            
            SystemBase.Validation.GroupBoxControlsLock(groupBox2, true);

            if (Linked == true)
            {
                cboSPlantCd.SelectedValue = strPlantCd;
                txtSInspReqNo.Text = strInspReqNo;
                dtpSInspReqDtFr.Value = strInspReqDt;
                dtpSInspReqDtTo.Value = strInspReqDt;
                cboSInspStatus.Text = strInspStatus;
                SearchExec();
            }

            lnkJump1.Text = "RELEASE";  //화면에 보여지는 링크명
            strJumpFileName1 = "QF.QFA021.QFA021"; //호출할 화면명

            lnkJump2.Text = "부적합처리";  //화면에 보여지는 링크명
            strJumpFileName2 = "QF.QFA012.QFA012"; //호출할 화면명

            lnkJump3.Text = "불량유형등록";  //화면에 보여지는 링크명
            strJumpFileName3 = "QF.QFA003.QFA003"; //호출할 화면명

            lnkJump4.Text = "검사내역등록";  //화면에 보여지는 링크명
            strJumpFileName4 = "QF.QFA002.QFA002"; //호출할 화면명

            lnkJump5.Text = "검사항목등록";  //화면에 보여지는 링크명
            strJumpFileName5 = "QF.QFA001.QFA001"; //호출할 화면명
            bLotDefect = false;
        }
        #endregion
        
        #region Link
        private object[] Params()
        {
            if (txtInspReqNo.Text == "")
                param = null;						// 파라메터를 하나도 넘기지 않을경우
            else
            {
                param = new object[4];					// 파라메터수가 4개인 경우
                param[0] = txtInspReqNo.Text;
                param[1] = cboSPlantCd.SelectedValue.ToString();
                param[2] = dtpInspReqDt.Text;
                param[3] = txtInspStatus.Text;
            }
            return param;
        }

        protected override void Link1Exec()
        {
            param = Params();

            SystemBase.Base.RodeFormID = "QFA021";			// 이동할 formid를 적어준다.(메뉴id)
            SystemBase.Base.RodeFormText = "RELEASE(최종)"; 	// 이동할 폼명을 적어준다(메뉴명)
        }

        protected override void Link2Exec()
        {
            param = Params();

            SystemBase.Base.RodeFormID = "QFA012";			// 이동할 formid를 적어준다.(메뉴id)
            SystemBase.Base.RodeFormText = "부적합처리(최종)"; 	// 이동할 폼명을 적어준다(메뉴명)
        }

        protected override void Link3Exec()
        {
            param = Params();

            SystemBase.Base.RodeFormID = "QFA003";			// 이동할 formid를 적어준다.(메뉴id)
            SystemBase.Base.RodeFormText = "불량유형등록(최종)"; 	// 이동할 폼명을 적어준다(메뉴명)
        }

        protected override void Link4Exec()
        {
            param = Params();

            SystemBase.Base.RodeFormID = "QFA002";			// 이동할 formid를 적어준다.(메뉴id)
            SystemBase.Base.RodeFormText = "검사내역등록(최종)"; 	// 이동할 폼명을 적어준다(메뉴명)
        }

        protected override void Link5Exec()
        {
            param = Params();

            SystemBase.Base.RodeFormID = "QFA001";			// 이동할 formid를 적어준다.(메뉴id)
            SystemBase.Base.RodeFormText = "검사항목등록(최종)"; 	// 이동할 폼명을 적어준다(메뉴명)
        }
        #endregion

        #region 조회조건 팝업
        //품목코드
        private void btnSItemCd_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW005 pu = new WNDW005(cboSPlantCd.SelectedValue.ToString(), true, txtSItemCd.Text);
                pu.MaximizeBox = false;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtSItemCd.Text = Msgs[2].ToString();
                    txtSItemNm.Value = Msgs[3].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목코드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        //공급처
        private void btnSBpCd_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW002 pu = new WNDW002(txtSBpCd.Text, "");
                pu.MaximizeBox = false;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtSBpCd.Text = Msgs[1].ToString();
                    txtSBpNm.Value = Msgs[2].ToString();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "공급처 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        //프로젝트번호
        private void btnSProjectNo_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW007 pu = new WNDW007(txtSProjectNo.Text);
                pu.MaximizeBox = false;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtSProjectNo.Text = Msgs[3].ToString();
                    txtSProjectNm.Value = Msgs[4].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "프로젝트번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //검사의뢰번호
        private void btnInspReqNo_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW009 pu = new WNDW009(Convert.ToString(cboSPlantCd.SelectedValue)
                    , txtSInspReqNo.Text
                    , "R"
                    , Convert.ToString(cboSInspStatus.SelectedValue));
                pu.MaximizeBox = false;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtSInspReqNo.Text = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "검사의뢰번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //제조오더번호
        private void btnSWorkOrderNo_Click(object sender, EventArgs e)
        {
            try
            {
                WNDW006 pu = new WNDW006(txtSWorkorderNo.Text);
                pu.MaximizeBox = false;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtSWorkorderNo.Text = Msgs[1].ToString();
                    txtSWorkorderNo.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "검사의뢰번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 조회조건 TextChanged
        //품목코드
        private void txtSItemCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtSItemCd.Text != "")
                {
                    txtSItemNm.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtSItemCd.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtSItemNm.Value = "";
                }
            }
            catch
            {

            }
        }

        //공급처
        private void txtSBpCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtSBpCd.Text != "")
                {
                    txtSBpNm.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtSBpCd.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtSBpNm.Value = "";
                }
            }
            catch
            {

            }
        }

        //프로젝트번호
        private void txtSProjectNo_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtSProjectNo.Text != "")
                {
                    txtSProjectNm.Value = SystemBase.Base.CodeName("PROJECT_NO", "PROJECT_NM", "S_SO_MASTER", txtSProjectNo.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtSProjectNm.Value = "";
                }
            }
            catch
            {

            }
        }
        #endregion

        #region 입력조건 팝업
        //검사원
        private void btnInspectorCd_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON 'COMM_POP' ,@pSPEC1='Q005', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtInspectorCd.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00067", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "검사원 팝업");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {

                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtInspectorCd.Text = Msgs[0].ToString();
                    txtInspectorNm.Value = Msgs[1].ToString();
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

        #region 입력조건 TextChanged
        //검사원
        private void txtInspectorCd_TextChanged(object sender, EventArgs e)
        {
            txtInspectorNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtInspectorCd.Text, " AND MAJOR_CD = 'Q005' AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND COMP_CODE ='" + SystemBase.Base.gstrCOMCD + "'");
        }
        #endregion

        #region NewExec()
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            SystemBase.Validation.GroupBox_Reset(groupBox2);
            SystemBase.Validation.GroupBox_Reset(groupBox3);

            fpSpread1.Sheets[0].Rows.Count = 0;
            fpSpread2.Sheets[0].Rows.Count = 0;

            //기타 세팅
            cboSPlantCd.SelectedValue = SystemBase.Base.gstrPLANT_CD;
            dtpSInspReqDtFr.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-3);
            dtpSInspReqDtTo.Value = SystemBase.Base.ServerTime("YYMMDD");
            dtpSInspDtFr.Value = null;
            dtpSInspDtTo.Value = null;
            cboSInspStatus.SelectedValue = "V";

            SystemBase.Validation.GroupBoxControlsLock(groupBox2, true);
            strInspReqNo = "";
            strInspStatus = "";
            strStatus = "";
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            bLotDefect = false;
            strInspReqNo = "";
            Grid2_Search();
        }
        #endregion

        #region 그리드조회
        private void Grid2_Search()
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {
                    string strQuery = " usp_QFA011  @pTYPE = 'S1'";
                    strQuery += ", @pPLANT_CD = '" + cboSPlantCd.SelectedValue.ToString() + "' ";
                    strQuery += ", @pINSP_REQ_DT_FR = '" + dtpSInspReqDtFr.Text + "' ";
                    strQuery += ", @pINSP_REQ_DT_TO = '" + dtpSInspReqDtTo.Text + "' ";
                    strQuery += ", @pINSP_DT_FR = '" + dtpSInspDtFr.Text + "' ";
                    strQuery += ", @pINSP_DT_TO = '" + dtpSInspDtTo.Text + "' ";
                    strQuery += ", @pITEM_CD = '" + txtSItemCd.Text + "' ";
                    strQuery += ", @pBP_CD = '" + txtSBpCd.Text + "' ";
                    strQuery += ", @pINSP_STATUS = '" + cboSInspStatus.SelectedValue.ToString() + "' ";
                    strQuery += ", @pDECISION_CD = '" + cboSDecisionCd.SelectedValue.ToString() + "'";
                    strQuery += ", @pPROJECT_NO = '" + txtSProjectNo.Text + "'";
                    strQuery += ", @pINSP_REQ_NO = '" + txtSInspReqNo.Text + "'";
                    strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'";
                    strQuery += ", @pWORKORDER_NO = '" + txtSWorkorderNo.Text + "'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
     
                    UIForm.FPMake.grdCommSheet(fpSpread2, strQuery, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, false, 0, 0, true);

                    if (fpSpread2.Sheets[0].Rows.Count > 0)
                    {
                        fpSpread2.Search(0, strInspReqNo, true, true, true, true, 0, SystemBase.Base.GridHeadIndex(GHIdx2, "검사의뢰번호"), ref SearchRow, ref SearchColumn);

                        if (SearchRow < 0)
                        { SearchRow = 0; }

                        fpSpread2.Focus();
                        fpSpread2.ActiveSheet.SetActiveCell(SearchRow, 1); //Row Focus		
                        fpSpread2.ShowRow(0, SearchRow, FarPoint.Win.Spread.VerticalPosition.Center); //Focus Row 보기

                        Grid1_Search(SearchRow);
                    }
                    else
                    {
                        SystemBase.Validation.GroupBox_Reset(groupBox2);
                        fpSpread1.Sheets[0].Rows.Count = 0;
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
        private void Grid2_Search(string strCode)
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {
                string strQuery = " usp_QFA011  @pTYPE = 'S1'";
                strQuery += ", @pPLANT_CD = '" + SystemBase.Base.gstrPLANT_CD + "' ";
                strQuery += ", @pINSP_REQ_NO = '" + strCode + "'";
                strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread2, strQuery, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, false, 0, 0, true);

                if (fpSpread2.Sheets[0].Rows.Count > 0)
                {
                    fpSpread2.Search(0, strInspReqNo, true, true, true, true, 0, SystemBase.Base.GridHeadIndex(GHIdx2, "검사의뢰번호"), ref SearchRow, ref SearchColumn);

                    if (SearchRow < 0)
                    { SearchRow = 0; }

                    fpSpread2.Focus();
                    fpSpread2.ActiveSheet.SetActiveCell(SearchRow, 1); //Row Focus		
                    fpSpread2.ShowRow(0, SearchRow, FarPoint.Win.Spread.VerticalPosition.Center); //Focus Row 보기

                    Grid1_Search(SearchRow);
                }
                else
                {
                    SystemBase.Validation.GroupBox_Reset(groupBox2);
                    fpSpread1.Sheets[0].Rows.Count = 0;
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
        #endregion

        #region fpSpread1 그리드 조회
        private void Grid1_Search(int iRow)
        {
            this.Cursor = Cursors.WaitCursor;

            SystemBase.Validation.GroupBox_Reset(groupBox2);

            strInspStatus = fpSpread2.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx2, "진행상태코드")].Text;
            strStatus = fpSpread2.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx2, "상태")].Text;
            
            if (fpSpread2.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx2, "통합의뢰번호")].Text.Trim().ToString() == "")
            {
                strInspReqNo = fpSpread2.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx2, "검사의뢰번호")].Text;
            }
            else
            {
                strInspReqNo = fpSpread2.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx2, "통합의뢰번호")].Text;
            }

            //groupBox2 값입력
            txtInspReqNo.Value = strInspReqNo;
            dtpInspReqDt.Value = fpSpread2.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx2, "검사의뢰일")].Text;
            dtpInspDemandDt.Value = fpSpread2.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx2, "검사요구일")].Text;
            txtItemCd.Value = fpSpread2.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx2, "품목코드")].Text;
            txtItemNm.Value = fpSpread2.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx2, "품명")].Text;
            txtWorkorderNo.Value = fpSpread2.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx2, "제조오더번호")].Text;
            txtRoutNo.Value = fpSpread2.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx2, "라우팅번호")].Text;
            txtRoutNm.Value = fpSpread2.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx2, "라우팅명")].Text;
            txtProcSeq.Value = fpSpread2.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx2, "공정순번")].Text;
            txtProcSeqNm.Value = fpSpread2.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx2, "공정명")].Text;
            txtLotSize.Value = Convert.ToDouble(fpSpread2.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx2, "LOT크기")].Value);
            txtStockUnit.Value = fpSpread2.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx2, "단위")].Text;
            txtProjectNo.Value = fpSpread2.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx2, "프로젝트번호")].Text;
            txtProjectNm.Value = fpSpread2.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx2, "프로젝트명")].Text;
            txtInspStatus.Value = fpSpread2.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx2, "진행상태")].Text;
            txtDecisionCd.Value = fpSpread2.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx2, "검사판정명")].Text;
           
            //groupBox3 값입력
            txtInspQty.Value = Convert.ToDouble(fpSpread2.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx2, "LOT크기")].Value);
            txtDefectQty.Value = Convert.ToDouble(fpSpread2.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx2, "불량수")].Value);
            cboDecisionCd.SelectedValue = fpSpread2.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx2, "검사판정")].Text;
            txtInspectorCd.Value = fpSpread2.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx2, "검사원")].Text;
            dtpInspDt.Value = fpSpread2.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx2, "검사일")].Text;
            txtRemark.Value = fpSpread2.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx2, "비고")].Text;
            txtLotNo.Value = fpSpread2.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx2, "Lot No")].Text;

            try
            {               	
				string strQuery = " usp_QFA011  @pTYPE = 'S2'";
                strQuery += ", @pINSP_REQ_NO = '" + strInspReqNo + "'";
				strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";

                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
				Grd_Set();                
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }

            if (string.Compare(fpSpread2.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx2, "Lot 추적")].Text, "True", true) == 0)
                LOT_YN.Checked = true;
            else
                LOT_YN.Checked = false;

            if (string.Compare(fpSpread2.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx2, "SERIAL 추적")].Text, "True", true) == 0)
                SERIAL_YN.Checked = true;
            else
                SERIAL_YN.Checked = false;


            if (string.Compare(fpSpread2.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx2, "Lot 추적")].Text, "True", true) == 0)
            {
                // 2015.12.28. hma 수정(Start): LOT품목인 경우도 불량수를 입력할 수 있도록 하기 위해 주석 처리함.
                //txtDefectQty.ReadOnly = true;
                //txtDefectQty.BackColor = SystemBase.Validation.Kind_LightCyan;
                // 2015.12.28. hma 수정(End)

                //Detail Locking설정
                for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                {
                    UIForm.FPMake.grdReMake(fpSpread1, i,
                        SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No_2") + "|0"       // case 0: 일반,
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "불량수") + "|3"         // case 3: 읽기전용이면서 필수항목에서 제외
                        );

                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "불량수")].BackColor = SystemBase.Validation.Kind_LightCyan;
                }

            }
            else
            {
                // 2015.12.28. hma 수정(Start): LOT품목인 경우도 불량수를 입력할 수 있도록 하기 위해 주석 처리함.
                //txtDefectQty.ReadOnly = false;
                //txtDefectQty.BackColor = SystemBase.Validation.Kind_LightCyan;
                // 2015.12.28. hma 수정(End)

                //Detail Locking설정
                for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                {
                    UIForm.FPMake.grdReMake(fpSpread1, i,
                        SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No") + "|3"             // case 3: 읽기전용이면서 필수항목에서 제외
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No_2") + "|3"   // case 3: 읽기전용이면서 필수항목에서 제외
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "불량수") + "|1"     // case 1: 필수입력
                        );
                }
            }


            this.Cursor = Cursors.Default;
        }
        #endregion

        #region fpSpread2 그리드 선택시 상세정보 조회
        private void fpSpread2_LeaveCell(object sender, FarPoint.Win.Spread.LeaveCellEventArgs e)
        {
            if (fpSpread2.Sheets[0].Rows.Count > 0)
            {
                if (e.Row != e.NewRow)
                {
                    try
                    {
                        SearchRow = e.NewRow;
                        Grid1_Search(SearchRow);
                    }
                    catch (Exception f)
                    {
                        SystemBase.Loggers.Log(this.Name, f.ToString());
                        MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                        //데이터 조회 중 오류가 발생하였습니다.				
                    }
                }
            }
        }
        #endregion

        #region SaveExec() 데이타 저장 로직
        protected override void SaveExec()
        {
            fpSpread1.Focus();

            //상단 그룹박스 필수 체크
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox3))
            {
                /////////////////////////////////////////////// MASTER 저장 시작 /////////////////////////////////////////////////				

                this.Cursor = Cursors.WaitCursor;

                string ERRCode = "WR", MSGCode = "P0000"; //처리할 내용이 없습니다.
                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                // 2015.11.27. hma 수정(Start): 생산품인 경우 LOT번호가 제조지시번호이고 T_IN_INFO에 Insert되는 시점이 생산입고 처리하는 시점이므로,
                //                              검사판정시 체크할 필요없음. 또한 저장시 LOT번호를 사용하지도 않음.
                //// 불량수량이 있을때 Lot 추적 대상이면 Lot No를 선택해야 함
                //if (CheckLotNo() == false || (bLotDefect == true && string.IsNullOrEmpty(txtLotNo.Text)))
                //{
                //    MessageBox.Show(SystemBase.Base.MessageRtn("Lot 추적대상이고, 불량수량이 있으면\r\n반드시 Lot No를 선택해야 합니다."), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //    this.Cursor = Cursors.Default;
                //    return;
                //}
                // 2015.11.27. hma 수정(End)

                try
                {
                    string strSql = " usp_QFA011 'U1'";
                    strSql += ", @pINSP_REQ_NO = '" + txtInspReqNo.Text + "'";
                    strSql += ", @pINSPECTOR_CD = '" + txtInspectorCd.Text + "'";

                    if (dtpInspDt.Text != "")
                        strSql += ", @pINSP_DT = '" + dtpInspDt.Text + "'";

                    if (Convert.ToDouble(txtInspQty.Value) != 0)
                        strSql += ", @pINSP_QTY = '" + txtInspQty.Value + "'";

                    if (Convert.ToDouble(txtDefectQty.Value) != 0)
                        strSql += ", @pDEFECT_QTY = '" + txtDefectQty.Value + "'";

                    strSql += ", @pDECISION_CD = '" + cboDecisionCd.SelectedValue.ToString() + "'";

                    if (txtRemark.Text != "")
                        strSql += ", @pREMARK = '" + txtRemark.Text + "'";

                    strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                    strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    DataTable dt = SystemBase.DbOpen.TranDataTable(strSql, dbConn, Trans);
                    ERRCode = dt.Rows[0][0].ToString();
                    MSGCode = dt.Rows[0][1].ToString();

                    if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                    /////////////////////////////////////////////// DETAIL 저장 시작 /////////////////////////////////////////////////

                    if (fpSpread1.Sheets[0].Rows.Count > 0)
                    {
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
                                        case "U": strGbn = "U2"; break;
                                        default: strGbn = ""; break;
                                    }

                                    string strSql1 = " usp_QFA011 '" + strGbn + "'";
                                    strSql1 += ", @pINSP_REQ_NO = '" + txtInspReqNo.Text + "' ";
                                    strSql1 += ", @pFIN_INSP_LVL = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사레벨")].Text + "'";
                                    strSql1 += ", @pDECISION_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "판정")].Value + "' ";

                                    if (Convert.ToDouble(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "불량수")].Value) != 0)
                                        strSql1 += ", @pDEFECT_QTY = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "불량수")].Value + "' ";

                                    strSql1 += ", @pINSP_ITEM_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사항목")].Text + "' ";
                                    strSql1 += ", @pINSP_SERIES = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사차수")].Text + "' ";
                                    strSql1 += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                                    strSql1 += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                                    DataSet ds = SystemBase.DbOpen.TranDataSet(strSql1, dbConn, Trans);
                                    ERRCode = ds.Tables[0].Rows[0][0].ToString();
                                    MSGCode = ds.Tables[0].Rows[0][1].ToString();

                                    if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프					
                                }
                            }
                        }
                        else
                        {
                            Trans.Rollback();
                            this.Cursor = Cursors.Default;
                            return;
                        }
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
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Grid2_Search(txtInspReqNo.Text);
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
        #endregion

        #region 그리드 재정의, 버튼설정, 판정설정 로직
        private void Grd_Set()
        {
            if (fpSpread1.Sheets[0].Rows.Count > 0)
            {
                fpSpread1.Sheets[0].Rows[0, fpSpread1.Sheets[0].Rows.Count - 1].Height = 30;

                if (strInspStatus == "Q" || strInspStatus == "R" || strStatus != "")
                {
                    SystemBase.Validation.GroupBoxControlsLock(groupBox3, true);

                    //Detail Locking설정
                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {
                        UIForm.FPMake.grdReMake(fpSpread1, i,
                            SystemBase.Base.GridHeadIndex(GHIdx1, "판정") + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "불량수") + "|3"
                            );
                    }

                    //버튼설정
                    UIForm.Buttons.ReButton("110000001001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);
                }
                else if (strInspStatus == "D")
                {
                    SystemBase.Validation.GroupBoxControlsLock(groupBox3, false);

                    //버튼설정
                    UIForm.Buttons.ReButton("110000111001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);
                }
                else
                {
                    SystemBase.Validation.GroupBoxControlsLock(groupBox3, false);
                    dtpInspDt.Value = SystemBase.Base.ServerTime("YYMMDD").ToString();

                    int iCount = 0;

                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {
                        if (Convert.ToDouble(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "불량수")].Value)
                            >= Convert.ToDouble(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "불합격판정개수")].Value))
                        {
                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "판정")].Value = "R"; //불합격
                            iCount++;
                        }
                        else
                        {
                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "판정")].Value = "A"; //합격
                        }

                        if (iCount > 0) //불합격
                        {
                            cboDecisionCd.SelectedValue = "R";
                        }
                        else //합격
                        {
                            cboDecisionCd.SelectedValue = "A";
                        }

                        UIForm.FPMake.fpChange(fpSpread1, i);
                    }

                    //버튼설정
                    UIForm.Buttons.ReButton("110000011001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);
                }
            }
            else
            {
                SystemBase.Validation.GroupBoxControlsLock(groupBox3, true);
                dtpInspDt.Value = null;

                //버튼설정
                UIForm.Buttons.ReButton("1100000001001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);
            }
        }
        #endregion    

        #region 불량률 자동Changed
        private void txtInspQty_TextChanged(object sender, EventArgs e)
        {
            double dInspQty, dDefectQty, dDefectRat;

            try
            {
                if (txtInspQty.Text != "" && txtDefectQty.Text != "")
                {
                    dInspQty = Convert.ToDouble(txtInspQty.Text);
                    dDefectQty = Convert.ToDouble(txtDefectQty.Text);
                    dDefectRat = 0;

                    if (dDefectQty > 0 && dInspQty > 0)
                    {
                        dDefectRat = (dDefectQty / dInspQty) * 100;
                        txtDefectRat.Value = dDefectRat;
                    }
                    else
                        txtDefectRat.Value = null;
                }
                else
                {
                    txtDefectRat.Value = null;
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "불량률 변경"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void txtDefectQty_TextChanged(object sender, EventArgs e)
        {
            double dInspQty, dDefectQty, dDefectRat;

            try
            {
                if (txtInspQty.Text != "" && txtDefectQty.Text != "")
                {
                    dInspQty = Convert.ToDouble(txtInspQty.Text);
                    dDefectQty = Convert.ToDouble(txtDefectQty.Text);
                    dDefectRat = 0;

                    if (dDefectQty > 0 && dInspQty > 0)
                    {
                        dDefectRat = (dDefectQty / dInspQty) * 100;
                        txtDefectRat.Value = dDefectRat;
                    }
                    else
                        txtDefectRat.Value = null;
                }
                else
                {
                    txtDefectRat.Value = null;
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "불량률 변경"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region DelExec() 삭제 로직
        protected override void DeleteExec()
        {
            DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("판정을 취소하시겠습니까?"), SystemBase.Base.MessageRtn("Z0004"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dsMsg == DialogResult.Yes)
            {
                this.Cursor = Cursors.WaitCursor;

                string ERRCode = "WR", MSGCode = "P0000";	//처리할 내용이 없습니다.
                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                try
                {
                    string strSql = " usp_QFA011 'U3'";
                    strSql += ", @pINSP_REQ_NO = '" + txtInspReqNo.Text + "'";
                    strSql += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'";
                    strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                    strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                    ERRCode = ds.Tables[0].Rows[0][0].ToString();
                    MSGCode = ds.Tables[0].Rows[0][1].ToString();

                    if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                    //행수만큼 처리
                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {

                        string strSql1 = " usp_QFA011 'U4'";
                        strSql1 += ", @pINSP_REQ_NO = '" + txtInspReqNo.Text + "' ";
                        strSql1 += ", @pFIN_INSP_LVL = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사레벨")].Text + "'";
                        strSql1 += ", @pINSP_ITEM_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사항목")].Text + "' ";
                        strSql1 += ", @pINSP_SERIES = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사차수")].Text + "' ";
                        strSql1 += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                        strSql1 += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                        DataSet ds1 = SystemBase.DbOpen.TranDataSet(strSql1, dbConn, Trans);
                        ERRCode = ds1.Tables[0].Rows[0][0].ToString();
                        MSGCode = ds1.Tables[0].Rows[0][1].ToString();

                        if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                    }

                    Trans.Commit();
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    Trans.Rollback();
                    ERRCode = "ER";
                    MSGCode = "P0001";	//에러가 발생하여 데이터 처리가 취소되었습니다.
                }
            Exit:
                dbConn.Close();

                if (ERRCode == "OK")
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Grid2_Search();
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
        #endregion	

        #region 검사성적서 출력
        private void butInspResult_Click(object sender, System.EventArgs e)
        {
            if (txtInspReqNo.Text != "")
            {
                string strSheetPage1 = "검사성적서";

                string strFileName = SystemBase.Base.ProgramWhere + @"\Report\검사성적서.xls";

                try
                {
                    //th = new Thread(new ThreadStart(Show_Waiting));   // 2015.05.12. hma 주석 처리 
                    //th.Start();
                    //Thread.Sleep(200);
                    //Waiting_Form.Activate();

                    this.Cursor = Cursors.WaitCursor;       // 2015.05.12. hma 추가

                    string strQuery = " usp_QFA002  @pTYPE = 'R1'";
                    strQuery += ", @pINSP_REQ_NO = '" + txtInspReqNo.Text + "' ";
                    strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' ";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

                    if (dt.Rows.Count > 0)
                    {

                        //Waiting_Form.progressBar_temp.Maximum = dt.Rows.Count;    // 2015.05.12. hma 주석 처리

                        string strInspItemCd = "";
                        int strSampleQty = 0;
                        int iTotPage = 0;

                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            if (dt.Rows[i]["INSP_ITEM_CD"].ToString() != strInspItemCd)
                            {
                                strSampleQty += Convert.ToInt32(dt.Rows[i]["SAMPLE_QTY"].ToString());
                                strInspItemCd = dt.Rows[i]["INSP_ITEM_CD"].ToString();

                            }
                        }

                        iTotPage = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(strSampleQty - 8) / Convert.ToDouble("10")));

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


                        //데이터수만큼 미리 복사------------------------------------------					

                        for (int i = 0; i < iTotPage; i++)
                        {
                            excel.SetSelect("A28", "A28");
                            excel.RunMacro("PageListAdd");
                        }
                        //------------------------------------------------------

                        // 엑셀쓰기---------------------------------------------------------

                        strInspItemCd = "";
                        int iUseRow = 2;
                        int iRow = 10;
                        int iCol = 6;
                        int j = 0;
                        int[] iAddCol = { 2, 2, 2, 2, 2, 2, 2, 2, 2 };
                        int NextPage = 28;
                        int iPage = 1;

                        // Heard 값
                        excel.SetCell(1, 24, dt.Rows[0]["TPAGE"].ToString() + " 매중 1 매");
                        string str_REQ = dt.Rows[0]["WORKORDER_NO"].ToString().Substring(0, 2);
                        if (str_REQ == "RQ")
                        {
                        }
                        else
                            excel.SetCell(2, 23, dt.Rows[j]["WORKORDER_NO"].ToString());

                        //사업명 계약번호 재고번호품명 제작처&구입처
                        excel.SetCell(4, 3, dt.Rows[0]["PROJECT_NM"].ToString());
                        excel.SetCell(5, 3, dt.Rows[0]["PROJECT_NO"].ToString());
                        excel.SetCell(6, 3, dt.Rows[0]["KKJGBH"].ToString());
                        excel.SetCell(7, 3, dt.Rows[0]["ITEM_NM"].ToString());
                        excel.SetCell(8, 3, dt.Rows[0]["MAKE_BUY"].ToString());

                        excel.SetCell(9, 3, dt.Rows[0]["MATERIAL"].ToString());

                        //품목코드 규격번호 도면번호/REV.NO 부품번호 로트수량(단위), 검사수량(단위)
                        excel.SetCell(4, 7, dt.Rows[0]["ITEM_CD"].ToString());
                        excel.SetCell(5, 7, dt.Rows[0]["SPEC_NO"].ToString());
                        excel.SetCell(6, 7, dt.Rows[0]["DRAW_NO"].ToString());
                        excel.SetCell(7, 7, dt.Rows[0]["ITEM_SPEC"].ToString());
                        excel.SetCell(8, 7, dt.Rows[0]["LOT_SIZE_STOCK_UNIT"].ToString());
                        excel.SetCell(9, 7, dt.Rows[0]["INSP_QTY"].ToString());
                       
                        excel.SetCell(4, 15, dt.Rows[0]["INSP_REQ_NO"].ToString()); ;


                        excel.SetCell(5, 15, dt.Rows[0]["INSP_METH_NM"].ToString()); ;

                        if (dt.Rows[0]["INSP_DT"].ToString() != "")
                            excel.SetCell(8, 15, dt.Rows[0]["INSP_DT"].ToString());

                        // 2015.05.06. hma 추가(Start): 검사책임자를 넘겨받아서 출력하도록 함.
                        excel.SetCell(7, 15, dt.Rows[0]["QC_MAN_NAME"].ToString());
                        // 2015.05.06. hma 추가(End)


                        if (dt.Rows[0]["INSPECTOR_NM"].ToString() != "")
                            excel.SetCell(9, 15, dt.Rows[0]["INSPECTOR_NM"].ToString());


                        for (int i = 0; i < dt.Rows.Count; i++) //내용입력
                        {

                            if (dt.Rows[i]["INSP_ITEM_CD"].ToString() == strInspItemCd)
                            {
                                if (iCol == 24)
                                {
                                    if (iRow == NextPage - 2)
                                    {
                                        iPage++;
                                        excel.SetCell(NextPage, 24, dt.Rows[i]["TPAGE"].ToString() + " 매중 " + iPage.ToString() + " 매");
                                        excel.SetCell(NextPage, 3, dt.Rows[i]["PROJECT_NM"].ToString());
                                        excel.SetCell(NextPage, 7, dt.Rows[i]["ITEM_NM"].ToString());
                                        excel.SetCell(NextPage, 15, dt.Rows[i]["ITEM_CD"].ToString());

                                        iRow = NextPage + 3;
                                        NextPage += 23;
                                    }
                                    else
                                    {
                                        iRow += 2;
                                    }

                                    j = 0;
                                    iCol = 6;
                                    iUseRow += 2;
                                }
                                else
                                {
                                    iCol += iAddCol[j];
                                    j++;
                                }
                            }
                            else if (strInspItemCd != "")
                            {
                                strInspItemCd = dt.Rows[i]["INSP_ITEM_CD"].ToString();

                                int iNextRow = (iRow + ((Convert.ToInt32(dt.Rows[i - 1]["SAMPLE_QTY"].ToString()) * 2) - iUseRow)) + 2;

                                if (iNextRow == NextPage)
                                {
                                    iRow = iNextRow;
                                    iPage++;
                                    excel.SetCell(NextPage, 24, dt.Rows[i]["TPAGE"].ToString() + " 매중 " + iPage.ToString() + " 매");
                                    excel.SetCell(iRow, 3, dt.Rows[i]["PROJECT_NM"].ToString());
                                    excel.SetCell(iRow, 7, dt.Rows[i]["ITEM_NM"].ToString());
                                    excel.SetCell(iRow, 15, dt.Rows[i]["ITEM_CD"].ToString());
                                    iRow += 3;
                                    NextPage += 23;

                                }
                                else if (iNextRow > NextPage)
                                {
                                    iPage++;
                                    excel.SetCell(NextPage, 24, dt.Rows[i]["TPAGE"].ToString() + " 매중 " + iPage.ToString() + " 매");
                                    excel.SetCell(NextPage, 3, dt.Rows[i]["PROJECT_NM"].ToString());
                                    excel.SetCell(NextPage, 7, dt.Rows[i]["ITEM_NM"].ToString());

                                    excel.SetCell(NextPage, 15, dt.Rows[i]["ITEM_CD"].ToString());

                                    iRow = iNextRow + 3;
                                    NextPage += 23;
                                }
                                else
                                {
                                    iRow = iNextRow;
                                }

                                iCol = 6;
                                j = 0;
                                iUseRow = 2;

                                excel.SetCell(iRow, 1, dt.Rows[i]["INSP_SEQ"].ToString());
                                excel.SetCell(iRow, 2, dt.Rows[i]["INSP_ITEM_NM"].ToString());
                                excel.SetCell(iRow + 1, 2, dt.Rows[i]["MAP_COOR"].ToString());
                                excel.SetCell(iRow, 3, dt.Rows[i]["INSP_SPEC"].ToString().Replace("\r\n", "\n"));
                                excel.SetCell(iRow, 4, dt.Rows[i]["MEASURE_NM"].ToString());
                                excel.SetCell(iRow, 26, dt.Rows[i]["AQL"].ToString());
                            }
                            else
                            {
                                strInspItemCd = dt.Rows[i]["INSP_ITEM_CD"].ToString();
                                iRow += 2;
                                iCol = 6;
                                j = 0;

                                excel.SetCell(iRow, 1, dt.Rows[i]["INSP_SEQ"].ToString());
                                excel.SetCell(iRow, 2, dt.Rows[i]["INSP_ITEM_NM"].ToString());
                                excel.SetCell(iRow + 1, 2, dt.Rows[i]["MAP_COOR"].ToString());
                                excel.SetCell(iRow, 3, dt.Rows[i]["INSP_SPEC"].ToString().Replace("\r\n", "\n"));
                                excel.SetCell(iRow, 4, dt.Rows[i]["MEASURE_NM"].ToString());
                                excel.SetCell(iRow, 26, dt.Rows[i]["AQL"].ToString());
                            }

                            if (dt.Rows[i]["VALUE"].ToString() != "")
                            {
                                excel.SetCell(iRow, iCol, dt.Rows[i]["VALUE"].ToString());
                            }
                            //Waiting_Form.progressBar_temp.Value = i + 1;      // 2015.05.12. hma 주석처리

                        }

                        if (dt.Rows[dt.Rows.Count - 1]["UNITY_INSP_REQ_NO"].ToString() != "")
                        {
                            string strQuery3 = " usp_QFA002  @pTYPE = 'R3'";
                            strQuery3 += ", @pINSP_REQ_NO = '" + dt.Rows[dt.Rows.Count - 1]["UNITY_INSP_REQ_NO"] + "'";
                            strQuery3 += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                            DataTable dt3 = SystemBase.DbOpen.NoTranDataTable(strQuery3);
                            {
                                excel.SetSelect("A" + NextPage, "A" + NextPage);
                                excel.RunMacro("PageOrderAdd");

                                iPage++;

                                excel.SetCell(NextPage, 24, dt.Rows[dt.Rows.Count - 1]["TPAGE"].ToString() + " 매중 " + iPage.ToString() + " 매");
                                excel.SetCell(NextPage, 3, dt.Rows[dt.Rows.Count - 1]["PROJECT_NM"].ToString());
                                excel.SetCell(NextPage, 7, dt.Rows[dt.Rows.Count - 1]["ITEM_NM"].ToString());

                                excel.SetCell(NextPage, 15, dt.Rows[dt.Rows.Count - 1]["ITEM_CD"].ToString());


                                for (int a = 0; a < dt3.Rows.Count; a++)
                                {
                                    if (a <= 34)
                                    {
                                        excel.SetCell(NextPage + 2 + a, 2, dt3.Rows[a]["INSP_REQ_NO"].ToString());
                                        excel.SetCell(NextPage + 2 + a, 13, dt3.Rows[a]["WORKORDER_NO"].ToString());
                                    }
                                    else
                                    {
                                        excel.SetCell(NextPage + 2 + a - 35, 5, dt3.Rows[a]["INSP_REQ_NO"].ToString());
                                        excel.SetCell(NextPage + 2 + a - 35, 22, dt3.Rows[a]["WORKORDER_NO"].ToString());
                                    }
                                }
                            }
                        }

                        excel.SetSelect("A1", "A1");

                        //Waiting_Form.label_temp.Text = "완료되었습니다.";        // 2015.05.12. hma 주석처리
                        //Thread.Sleep(500);

                        excel.ShowExcel(true);
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "검사성적서출력"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    //Waiting_Form.Close();     // 2015.05.12. hma 주석처리
                    //th.Abort();
                    File.SetAttributes(strFileName, System.IO.FileAttributes.Normal);
                }
                this.Cursor = Cursors.Default;      // 2015.05.12. hma 추가

            }
        }


        private void Show_Waiting()
        {
            Waiting_Form = new UIForm.ExcelWaiting("검사성적서출력...");
            Waiting_Form.ShowDialog();
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

        private void lnkJump2_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (strJumpFileName2.Length > 0)
                {
                    string DllName = strJumpFileName2.Substring(0, strJumpFileName2.IndexOf("."));
                    string FrmName = strJumpFileName2.Substring(strJumpFileName2.IndexOf(".") + 1, strJumpFileName2.Length - strJumpFileName2.IndexOf(".") - 1);

                    for (int k = 0; k < this.MdiParent.MdiChildren.Length; k++)
                    {	// 폼이 이미 열려있으면 닫기
                        if (MdiParent.MdiChildren[k].Name == FrmName.Substring(0, 6))
                        {
                            MdiParent.MdiChildren[k].BringToFront();
                            MdiParent.MdiChildren[k].Close();
                            break;
                        }
                    }

                    Link2Exec();

                    Assembly ServiceAssembly = Assembly.LoadFile(SystemBase.Base.ProgramWhere.ToString() + "\\" + DllName + "." + FrmName.Substring(0, 6) + ".dll");
                    Form myForm = (Form)System.Activator.CreateInstance(ServiceAssembly.GetType(strJumpFileName2), param);
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

        private void lnkJump3_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (strJumpFileName3.Length > 0)
                {
                    string DllName = strJumpFileName3.Substring(0, strJumpFileName3.IndexOf("."));
                    string FrmName = strJumpFileName3.Substring(strJumpFileName3.IndexOf(".") + 1, strJumpFileName3.Length - strJumpFileName3.IndexOf(".") - 1);

                    for (int k = 0; k < this.MdiParent.MdiChildren.Length; k++)
                    {	// 폼이 이미 열려있으면 닫기
                        if (MdiParent.MdiChildren[k].Name == FrmName.Substring(0, 6))
                        {
                            MdiParent.MdiChildren[k].BringToFront();
                            MdiParent.MdiChildren[k].Close();
                            break;
                        }
                    }

                    Link3Exec();

                    Assembly ServiceAssembly = Assembly.LoadFile(SystemBase.Base.ProgramWhere.ToString() + "\\" + DllName + "." + FrmName.Substring(0, 6) + ".dll");
                    Form myForm = (Form)System.Activator.CreateInstance(ServiceAssembly.GetType(strJumpFileName3), param);
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

        private void lnkJump4_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (strJumpFileName4.Length > 0)
                {
                    string DllName = strJumpFileName4.Substring(0, strJumpFileName4.IndexOf("."));
                    string FrmName = strJumpFileName4.Substring(strJumpFileName4.IndexOf(".") + 1, strJumpFileName4.Length - strJumpFileName4.IndexOf(".") - 1);

                    for (int k = 0; k < this.MdiParent.MdiChildren.Length; k++)
                    {	// 폼이 이미 열려있으면 닫기
                        if (MdiParent.MdiChildren[k].Name == FrmName.Substring(0, 6))
                        {
                            MdiParent.MdiChildren[k].BringToFront();
                            MdiParent.MdiChildren[k].Close();
                            break;
                        }
                    }

                    Link4Exec();

                    Assembly ServiceAssembly = Assembly.LoadFile(SystemBase.Base.ProgramWhere.ToString() + "\\" + DllName + "." + FrmName.Substring(0, 6) + ".dll");
                    Form myForm = (Form)System.Activator.CreateInstance(ServiceAssembly.GetType(strJumpFileName4), param);
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

        private void lnkJump5_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (strJumpFileName5.Length > 0)
                {
                    string DllName = strJumpFileName5.Substring(0, strJumpFileName5.IndexOf("."));
                    string FrmName = strJumpFileName5.Substring(strJumpFileName5.IndexOf(".") + 1, strJumpFileName5.Length - strJumpFileName5.IndexOf(".") - 1);

                    for (int k = 0; k < this.MdiParent.MdiChildren.Length; k++)
                    {	// 폼이 이미 열려있으면 닫기
                        if (MdiParent.MdiChildren[k].Name == FrmName.Substring(0, 6))
                        {
                            MdiParent.MdiChildren[k].BringToFront();
                            MdiParent.MdiChildren[k].Close();
                            break;
                        }
                    }

                    Link5Exec();

                    Assembly ServiceAssembly = Assembly.LoadFile(SystemBase.Base.ProgramWhere.ToString() + "\\" + DllName + "." + FrmName.Substring(0, 6) + ".dll");
                    Form myForm = (Form)System.Activator.CreateInstance(ServiceAssembly.GetType(strJumpFileName5), param);
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

        private void lnkJump6_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (strJumpFileName6.Length > 0)
                {
                    string DllName = strJumpFileName6.Substring(0, strJumpFileName6.IndexOf("."));
                    string FrmName = strJumpFileName6.Substring(strJumpFileName6.IndexOf(".") + 1, strJumpFileName6.Length - strJumpFileName6.IndexOf(".") - 1);

                    for (int k = 0; k < this.MdiParent.MdiChildren.Length; k++)
                    {	// 폼이 이미 열려있으면 닫기
                        if (MdiParent.MdiChildren[k].Name == FrmName.Substring(0, 6))
                        {
                            MdiParent.MdiChildren[k].BringToFront();
                            MdiParent.MdiChildren[k].Close();
                            break;
                        }
                    }

                    Link6Exec();

                    Assembly ServiceAssembly = Assembly.LoadFile(SystemBase.Base.ProgramWhere.ToString() + "\\" + DllName + "." + FrmName.Substring(0, 6) + ".dll");
                    Form myForm = (Form)System.Activator.CreateInstance(ServiceAssembly.GetType(strJumpFileName6), param);
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

        #region 불량수량이 있을때 Lot 추적 대상이면 Lot No를 선택해야 함
        private bool CheckLotNo()
        {
            bool bReturn = true;

            if (fpSpread1.Sheets[0].Rows.Count > 0)
            {
                for (int i = 0; i <= fpSpread1.Sheets[0].Rows.Count - 1; i++)
                {
                    if (Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "불량수")].Value) > 0 &&
                        LOT_YN.Checked == true)
                    {

                        bLotDefect = true;

                        if (string.IsNullOrEmpty(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No")].Text) == true)
                        {
                            bReturn = false;
                            break;
                        }
                    }
                }
            }

            return bReturn;
        }
        #endregion

        	
    }
}