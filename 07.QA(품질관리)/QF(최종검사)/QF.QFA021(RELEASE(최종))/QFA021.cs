#region 작성정보
/*********************************************************************/
// 단위업무명 : RELEASE(최종)
// 작 성 자 : 김현근
// 작 성 일 : 2013-04-08
// 작성내용 : RELEASE(최종) 및 관리
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
using System.Reflection;

namespace QF.QFA021
{
    public partial class QFA021 : UIForm.FPCOMM1
    {
        #region 변수선언
        string strInspReqNo = "";	//검사의뢰번호
        int SearchRow = 0;
        int ShowColumn = 0;
        string strPlantCd = "";
        string strInspReqDt = "";
        string strInspStatus = "";
        bool Linked = false;
        Thread th;
        UIForm.ExcelWaiting Waiting_Form = null;
        #endregion

        #region 생성자
        public QFA021()
        {
            InitializeComponent();
        }

        public QFA021(string param1, string param2, string param3, string param4)
        {
            strInspReqNo = param1;
            strPlantCd = param2;
            strInspReqDt = param3;
            strInspStatus = param4;
            Linked = true;

            InitializeComponent();
        }
        #endregion

        #region Form Load시
        private void QFA021_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            SystemBase.Validation.GroupBox_Setting(groupBox3);

            //콤보박스 세팅
            SystemBase.ComboMake.C1Combo(cboSPlantCd, "usp_B_COMMON @pType='TABLE', @pCODE = 'PLANT_CD', @pNAME = 'PLANT_NM', @pSPEC1 = 'B_PLANT_INFO', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//공장
            SystemBase.ComboMake.C1Combo(cboSInspStatus, "usp_B_COMMON @pType='COMM2', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "',@pCODE = 'Q003', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0); //검사진행상태
            SystemBase.ComboMake.C1Combo(cboSDecisionCd, "usp_B_COMMON @pType='COMM', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "',@pCODE = 'Q004', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 9); //판정

            //그리드초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

            //기타 세팅
            cboSPlantCd.SelectedValue = SystemBase.Base.gstrPLANT_CD;
            dtpSInspDtFr.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-3);
            dtpSInspDtTo.Value = SystemBase.Base.ServerTime("YYMMDD");
            cboSInspStatus.SelectedValue = "D";

            SystemBase.Validation.GroupBoxControlsLock(groupBox2, true);

            if (Linked == true)
            {
                cboSPlantCd.SelectedValue = strPlantCd;
                txtSInspReqNo.Text = strInspReqNo;
                cboSInspStatus.Text = strInspStatus;
                dtpSInspDtFr.Value = null;
                dtpSInspDtTo.Value = null;
                SearchExec();
            }

            lnkJump1.Text = "부적합처리";  //화면에 보여지는 링크명
            strJumpFileName1 = "QF.QFA012.QFA012"; //호출할 화면명

            lnkJump2.Text = "검사판정";  //화면에 보여지는 링크명
            strJumpFileName2 = "QF.QFA011.QFA011"; //호출할 화면명

            lnkJump3.Text = "불량유형등록";  //화면에 보여지는 링크명
            strJumpFileName3 = "QF.QFA003.QFA003"; //호출할 화면명

            lnkJump4.Text = "검사내역등록";  //화면에 보여지는 링크명
            strJumpFileName4 = "QF.QFA002.QFA002"; //호출할 화면명

            lnkJump5.Text = "검사항목등록";  //화면에 보여지는 링크명
            strJumpFileName5 = "QF.QFA001.QFA001"; //호출할 화면명
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

            SystemBase.Base.RodeFormID = "QFA012";			// 이동할 formid를 적어준다.(메뉴id)
            SystemBase.Base.RodeFormText = "부적합처리(최종)"; 	// 이동할 폼명을 적어준다(메뉴명)
        }

        protected override void Link2Exec()
        {
            param = Params();

            SystemBase.Base.RodeFormID = "QFA011";			// 이동할 formid를 적어준다.(메뉴id)
            SystemBase.Base.RodeFormText = "검사판정(최종)"; 	// 이동할 폼명을 적어준다(메뉴명)
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
                WNDW006 pu = new WNDW006(txtSWorkOrderNo.Text);
                pu.MaximizeBox = false;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtSWorkOrderNo.Text = Msgs[1].ToString();
                    txtSWorkOrderNo.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "검사의뢰번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //Release 담당자
        private void BtnReleaserCd_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON 'COMM_POP' ,@pSPEC1='Q005', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
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

        //Release 담당자
        private void txtReleaserCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtReleaserCd.Text != "")
                {
                    txtReleaserNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtReleaserCd.Text, " AND MAJOR_CD = 'Q005' AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND COMP_CODE ='" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtReleaserNm.Value = "";
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
            SystemBase.Validation.GroupBox_Reset(groupBox2);
            SystemBase.Validation.GroupBox_Reset(groupBox3);

            fpSpread1.Sheets[0].Rows.Count = 0;

            //기타 세팅
            cboSPlantCd.SelectedValue = SystemBase.Base.gstrPLANT_CD;
            dtpSInspDtFr.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-3).ToString().Substring(0,10);
            dtpSInspDtTo.Value = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0, 10);
            cboSInspStatus.SelectedValue = "D";

            SystemBase.Validation.GroupBoxControlsLock(groupBox2, true);

            strInspReqNo = "";
            SearchRow = 0;
            ShowColumn = 0;

        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            strInspReqNo = "";
            Grid_Search();
        }
        #endregion

        #region 그리드조회
        private void Grid_Search()
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {
                    string strQuery = " usp_QFA021  @pTYPE = 'S1'";
                    strQuery += ", @pPLANT_CD = '" + cboSPlantCd.SelectedValue.ToString() + "' ";
                    strQuery += ", @pINSP_DT_FR = '" + dtpSInspDtFr.Text + "' ";
                    strQuery += ", @pINSP_DT_TO = '" + dtpSInspDtTo.Text + "' ";
                    strQuery += ", @pITEM_CD = '" + txtSItemCd.Text + "' ";
                    strQuery += ", @pBP_CD = '" + txtSBpCd.Text + "' ";
                    strQuery += ", @pINSP_STATUS = '" + cboSInspStatus.SelectedValue.ToString() + "' ";
                    strQuery += ", @pDECISION_CD = '" + cboSDecisionCd.SelectedValue.ToString() + "'";
                    strQuery += ", @pPROJECT_NO = '" + txtSProjectNo.Text + "'";
                    strQuery += ", @pINSP_REQ_NO = '" + txtSInspReqNo.Text + "'";
                    strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'";
                    strQuery += ", @pWORKORDER_NO = '" + txtSWorkOrderNo.Text + "'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false, true);

                    if (fpSpread1.Sheets[0].Rows.Count > 0)
                    {
                        fpSpread1.Search(0, strInspReqNo, true, true, true, true, 0, SystemBase.Base.GridHeadIndex(GHIdx1, "검사의뢰번호"), ref SearchRow, ref ShowColumn);

                        for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                        { Grd_Set(i); }
                       
                        if (SearchRow < 0)
                        { SearchRow = 0; }

                        SubSearch(SearchRow);
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
        #endregion

        #region 상세조회
        private void SubSearch(int iRow)
        {
            strInspReqNo = fpSpread1.Sheets[0].Cells[SearchRow, SystemBase.Base.GridHeadIndex(GHIdx1, "검사의뢰번호")].Text;

            //groupBox2 값입력
            txUnitytInspReqNo.Value = fpSpread1.Sheets[0].Cells[SearchRow, SystemBase.Base.GridHeadIndex(GHIdx1, "통합의뢰번호")].Text;
            txtInspReqNo.Value = fpSpread1.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx1, "검사의뢰번호")].Text;
            dtpInspReqDt.Value = fpSpread1.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx1, "검사의뢰일")].Text;
            dtpInspDt.Value = fpSpread1.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx1, "검사일")].Text;
            txtItemCd.Value = fpSpread1.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text;
            txtItemNm.Value = fpSpread1.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx1, "품목명")].Text;
            txtFinInspLvl.Value = fpSpread1.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx1, "검사레벨")].Text;
            txtFinInspLvlNm.Value = fpSpread1.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx1, "레벨명")].Text;
            txtLotSize.Value = Convert.ToDouble(fpSpread1.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx1, "LOT크기")].Value);
            txtStockUnit.Value = fpSpread1.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].Text;
            txtProjectNo.Value = fpSpread1.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text;
            txtProjectNm.Value = fpSpread1.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트명")].Text;
            txtInspStatus.Value = fpSpread1.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx1, "검사진행상태")].Text;
            txtDecisionCd.Value = fpSpread1.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx1, "판정")].Text;
            txtInspGoodQty.Value = Convert.ToDouble(fpSpread1.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx1, "양품수")].Value);
            txtDefectQty.Value = Convert.ToDouble(fpSpread1.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx1, "불량수")].Value);
            txtReleaserCd.Value = fpSpread1.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx1, "담당자")].Text;

        }
        #endregion

        #region fpSpread1_LeaveCell
        private void fpSpread1_LeaveCell(object sender, FarPoint.Win.Spread.LeaveCellEventArgs e)
        {
            if (fpSpread1.Sheets[0].Rows.Count > 0)
            {
                if (e.Row != e.NewRow)
                {
                    try
                    {
                        SearchRow = e.NewRow;
                        SubSearch(SearchRow);
                    }
                    catch (Exception f)
                    {
                        SystemBase.Loggers.Log(this.Name, f.ToString());
                        MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                        //데이터 조회 중 오류가 발생하였습니다.				
                    }
                }
            }
        }
        #endregion

        #region fpSpread1_ButtonClicked 버튼 클릭 Event
        protected override void fpButtonClick(int Row, int Column)
        {
            if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "Release여부"))
            {
                if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "Release여부")].Text == "True")
                {
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "Release일자")].Text = SystemBase.Base.ServerTime("YYMMDD");
                }
                else
                {
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "Release일자")].Text = "";
                }

           
                Grd_Set(Row);
            }

            if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "증빙"))
            {
                WNDW036 pu = new WNDW036();
                pu.strKEY_NO = fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "입고번호")].Text;
                pu.strKEY_SEQ = fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "입고순번")].Text;
                pu.strREQ_TYPE = "PO";
                pu.strDOC_TYPE = "PUR";
                pu.strFormGubn = "QRA021";

                pu.ShowDialog();
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
                if (fpSpread1.Sheets[0].Rows.Count > 0)
                {
                    this.Cursor = Cursors.WaitCursor;

                    string ERRCode = "WR", MSGCode = "P0000"; //처리할 내용이 없습니다.
                    SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                    SqlCommand cmd = dbConn.CreateCommand();
                    SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                    try
                    {
                        //그리드 상단 필수 체크
                        if (SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", true))
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

                                    string strSql = " usp_QFA021 '" + strGbn + "'";
                                    strSql += ", @pINSP_REQ_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사의뢰번호")].Text + "'";

                                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Release여부")].Text == "True")
                                    { strSql += ", @pRELEASE_DIV = 'R'"; }
                                    else
                                    { strSql += ", @pRELEASE_DIV = 'D'"; }

                                    strSql += ", @pRELEASE_DT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Release일자")].Text + "'";
                                    strSql += ", @pINSP_RESULT_QTY = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "성적서수량")].Value + "'";
                                    strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                                    strSql += ", @pRELEASER_CD = '" + txtReleaserCd.Text + "'";
                                    strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                                    DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
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
                        Grid_Search();
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
        #endregion

        #region 그리드 재정의
        private void Grd_Set(int iRow)
        {
            if (fpSpread1.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx1, "마감여부")].Text != "Y")
            {
                if (fpSpread1.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx1, "Release여부")].Text == "True")
                {
                    UIForm.FPMake.grdReMake(fpSpread1, iRow, SystemBase.Base.GridHeadIndex(GHIdx1, "Release여부") + "|0"
                                                             + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "Release일자") + "|1");

                }
                else
                {
                    UIForm.FPMake.grdReMake(fpSpread1, iRow, SystemBase.Base.GridHeadIndex(GHIdx1, "Release여부") + "|0"
                                                             + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "Release일자") + "|0");
                }
            }
            else
            {
                UIForm.FPMake.grdReMake(fpSpread1, iRow, SystemBase.Base.GridHeadIndex(GHIdx1, "Release여부") + "|3"
                                                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "Release일자") + "|3");
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
                    // 2015.05.07. hma 수정(Start): 실행시 오류가 발생하여 Waiting Form이 나오지 않도록 주석 처리함. 
                    //th = new Thread(new ThreadStart(Show_Waiting));
                    //th.Start();
                    //Thread.Sleep(200);
                    //Waiting_Form.Activate();
                    this.Cursor = System.Windows.Forms.Cursors.WaitCursor;     // 2015.05.07. hma 추가: 마우스 모양
                    // 2015.05.07. hma 수정(End)

                    string strQuery = " usp_QFA002  @pTYPE = 'R1'";
                    strQuery += ", @pINSP_REQ_NO = '" + txtInspReqNo.Text + "' ";
                    strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' ";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

                    if (dt.Rows.Count > 0)
                    {

                        //Waiting_Form.progressBar_temp.Maximum = dt.Rows.Count;        // 2015.05.07. hma 수정: 실행시 오류가 발생하여 Waiting Form이 나오지 않도록 주석 처리함.

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

                        // 2015.05.07. hma 추가(Start): 검사책임자를 넘겨받아서 출력하도록 함.
                        excel.SetCell(7, 15, dt.Rows[0]["QC_MAN_NAME"].ToString());
                        // 2015.05.07. hma 추가(End)

                        if (dt.Rows[0]["INSP_DT"].ToString() != "")
                            excel.SetCell(8, 15, dt.Rows[0]["INSP_DT"].ToString());


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
                            //Waiting_Form.progressBar_temp.Value = i + 1;          // 2015.05.07. hma 수정: 실행시 오류가 발생하여 Waiting Form이 나오지 않도록 주석 처리함.

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

                        // 2015.05.07. hma 수정(Start): 실행시 오류가 발생하여 Waiting Form이 나오지 않도록 주석 처리함.
                        //Waiting_Form.label_temp.Text = "완료되었습니다.";
                        //Thread.Sleep(500);
                        // 2015.05.07. hma 수정(End)
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
                    // 2015.05.07. hma 수정(Start): 실행시 오류가 발생하여 Waiting Form이 나오지 않도록 주석 처리함.
                    //Waiting_Form.Close();
                    //th.Abort();
                    // 2015.05.07. hma 수정(End)
                    File.SetAttributes(strFileName, System.IO.FileAttributes.Normal);
                }
            }
            this.Cursor = System.Windows.Forms.Cursors.Default;     // 2015.05.07. hma 추가: 마우스 모양

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
        	
    }
}
