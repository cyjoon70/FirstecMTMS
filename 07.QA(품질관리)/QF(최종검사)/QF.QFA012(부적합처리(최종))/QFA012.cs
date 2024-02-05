#region 작성정보
/*********************************************************************/
// 단위업무명 : 부적합처리(최종)
// 작 성 자 : 김 현 근
// 작 성 일 : 2013-04-05
// 작성내용 : 부적합처리(최종) 및 관리
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

namespace QF.QFA012
{
    public partial class QFA012 : UIForm.FPCOMM2
    {
        #region 변수선언
        string strInspReqNo = "";
        int SearchRow = 0;
        int ShowColumn = 0;
        string strInspStatus = "";
        string strPlantCd = "";
        string strInspReqDt = "";
        bool Linked = false;
        #endregion

        #region 생성자
        public QFA012()
        {
            InitializeComponent();
        }

        public QFA012(string param1, string param2, string param3, string param4)
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
        private void QFA012_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            //콤보박스 세팅
            SystemBase.ComboMake.C1Combo(cboSPlantCd, "usp_B_COMMON @pType='TABLE', @pCODE = 'PLANT_CD', @pNAME = 'PLANT_NM', @pSPEC1 = 'B_PLANT_INFO', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//공장
            SystemBase.ComboMake.C1Combo(cboSInspStatus, "usp_B_COMMON @pType='COMM2', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "',@pCODE = 'Q003', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0); //검사진행상태
            SystemBase.ComboMake.C1Combo(cboSDecisionCd, "usp_B_COMMON @pType='COMM', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "',@pCODE = 'Q004', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 9); //판정


            //그리드콤보박스 세팅
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "부적합처리")] = SystemBase.ComboMake.ComboOnGrid("usp_Q_COMMON @pType='Q070', @pCODE = 'F', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0); //부적합처리

            //그리드초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
            UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, 0, 0, false);

            //기타 세팅
            cboSPlantCd.SelectedValue = SystemBase.Base.gstrPLANT_CD;
            dtpSInspReqDtFr.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-3);
            dtpSInspReqDtTo.Value = SystemBase.Base.ServerTime("YYMMDD");
            cboSInspStatus.SelectedValue = "D";

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

            lnkJump2.Text = "검사항목등록";  //화면에 보여지는 링크명
            strJumpFileName2 = "QF.QFA001.QFA001"; //호출할 화면명   
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

        #region NewExec()
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            SystemBase.Validation.GroupBox_Reset(groupBox2);

            fpSpread1.Sheets[0].Rows.Count = 0;
            fpSpread2.Sheets[0].Rows.Count = 0;

            //기타 세팅
            cboSPlantCd.SelectedValue = SystemBase.Base.gstrPLANT_CD;
            dtpSInspReqDtFr.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-3).ToString().Substring(0,10);
            dtpSInspReqDtTo.Value = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0, 10);
            cboSInspStatus.SelectedValue = "D";

            SystemBase.Validation.GroupBoxControlsLock(groupBox2, true);
            strInspReqNo = "";
            strInspStatus = "";
        }
        #endregion

        #region 행추가 버튼 클릭 이벤트
        protected override void RowInsExec()
        {
            try
            {
                UIForm.FPMake.RowInsert(fpSpread1);
                fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "검사항목")].Locked = true;
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "행추가"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

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
                    string strQuery = " usp_QFA012  @pTYPE = 'S1'";
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
                    strQuery += ", @pWORKORDER_NO = '" + txtSWorkOrderNo.Text + "'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
     
                    UIForm.FPMake.grdCommSheet(fpSpread2, strQuery, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, false, 0, 0, true);

                    if (fpSpread2.Sheets[0].Rows.Count > 0)
                    {
                        fpSpread2.Search(0, strInspReqNo, true, true, true, true, 0, SystemBase.Base.GridHeadIndex(GHIdx2, "검사의뢰번호"), ref SearchRow, ref ShowColumn);

                        if (SearchRow < 0)
                        { SearchRow = 0; }

                        fpSpread2.Focus();
                        fpSpread2.ActiveSheet.SetActiveCell(SearchRow, 1); //Row Focus		
                        fpSpread2.ShowRow(0, SearchRow, FarPoint.Win.Spread.VerticalPosition.Center); //Focus Row 보기

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

        #region  Master그리드 선택시 상세정보 조회
        private void fpSpread2_LeaveCell(object sender, FarPoint.Win.Spread.LeaveCellEventArgs e)
        {
            if (fpSpread2.Sheets[0].Rows.Count > 0)
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

        #region fpSpread1 그리드 조회
        private void SubSearch(int iRow)
        {
            this.Cursor = Cursors.WaitCursor;

            SystemBase.Validation.GroupBox_Reset(groupBox2);

            strInspStatus = fpSpread2.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx2, "진행상태코드")].Text;
            strInspReqNo = fpSpread2.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx2, "검사의뢰번호")].Text;

            //groupBox2 값입력
            txtInspReqNo.Value = fpSpread2.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx2, "검사의뢰번호")].Text;
            dtpInspReqDt.Value = fpSpread2.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx2, "검사의뢰일")].Text;
            dtpInspDemandDt.Value = fpSpread2.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx2, "검사요구일")].Text;
            txtWorkorderNo.Value = fpSpread2.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx2, "제조오더번호")].Text;
            txtProcSeq.Value = fpSpread2.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx2, "공정순번")].Text;
            txtItemCd.Value = fpSpread2.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx2, "품목코드")].Text;
            txtItemNm.Value = fpSpread2.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx2, "품명")].Text;
            txtFinInspLvl.Value = fpSpread2.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx2, "검사레벨")].Text;
            txtFinInspLvlNm.Value = fpSpread2.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx2, "레벨명")].Text;
            txtLotSize.Value = Convert.ToDouble(fpSpread2.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx2, "LOT크기")].Value);
            txtStockUnit.Value = fpSpread2.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx2, "단위")].Text;
            txtProjectNo.Value = fpSpread2.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx2, "프로젝트번호")].Text;
            txtProjectNm.Value = fpSpread2.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx2, "프로젝트명")].Text;
            txtInspStatus.Value = fpSpread2.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx2, "진행상태")].Text;
            txtDecisionCd.Value = fpSpread2.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx2, "검사판정명")].Text;
            txtInspQty.Value = Convert.ToDouble(fpSpread2.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx2, "검사수")].Value);
            txtDefectQty.Value = Convert.ToDouble(fpSpread2.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx2, "불량수")].Value);

            try
            {
                string strQuery = " usp_QFA012  @pTYPE = 'S2'";
                strQuery += ", @pINSP_REQ_NO = '" + fpSpread2.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx2, "검사의뢰번호")].Text + "'";
                strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
     
                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false, false);

                Grd_Set();
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

        #region SaveExec() 데이타 저장 로직
        protected override void SaveExec()
        {
            fpSpread1.Focus();

            //상단 그룹박스 필수 체크
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox2))
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

                                    #region 유효성 체크
                                    //검사항목코드 유효성 체크
                                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "공정명")].Text == "")                                       
                                    {
                                        //존재하지 않는 공정 코드입니다
                                        MessageBox.Show(SystemBase.Base.MessageRtn("B0036", "공정"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                        fpSpread1.ActiveSheet.SetActiveCell(i, SystemBase.Base.GridHeadIndex(GHIdx1, "발생공정"));

                                        Trans.Rollback();
                                        this.Cursor = Cursors.Default;
                                        return;
                                    }
                                    #endregion

                                    string strSql = " usp_QFA012 '" + strGbn + "'";
                                    strSql += ", @pINSP_REQ_NO = '" + txtInspReqNo.Text + "' ";
                                    strSql += ", @pPROC_SEQ	= '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "발생공정")].Text + "'";
                                    strSql += ", @pDISPOSAL_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "부적합처리")].Value + "' ";
                                    strSql += ", @pQTY = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수량")].Value + "' ";
                                    strSql += ", @pREMARK = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "비고")].Value + "' ";
                                    strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                                    strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                                    DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                                    ERRCode = ds.Tables[0].Rows[0][0].ToString();
                                    MSGCode = ds.Tables[0].Rows[0][1].ToString();

                                    if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프					
                                }
                            }
                            
                            //수량체크
							string strSql1 = " usp_QFA012 'C1'";
							strSql1 += ", @pINSP_REQ_NO = '" + txtInspReqNo.Text + "'";
                            strSql1 += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

							DataSet ds1 = SystemBase.DbOpen.TranDataSet(strSql1, dbConn, Trans);
							ERRCode = ds1.Tables[0].Rows[0][0].ToString();

                            if (ERRCode != "OK")
                            {
                                MSGCode = ds1.Tables[0].Rows[0][1].ToString();
                                Trans.Rollback();
                                goto Exit;
                            }	// ER 코드 Return시 점프
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

        #region 그리드 재정의, 버튼설정
        private void Grd_Set()
        {
            if (fpSpread1.Sheets[0].Rows.Count > 0)
            {
                if (strInspStatus != "D")
                {
                    //Detail Locking설정
                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {
                        UIForm.FPMake.grdReMake(fpSpread1, i,
                            SystemBase.Base.GridHeadIndex(GHIdx1, "수량") + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "비고") + "|3"
                            );
                    }

                    //버튼설정
                    UIForm.Buttons.ReButton("110000001001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);
                }
                else
                {
                    //Detail Locking해제
                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {
                        UIForm.FPMake.grdReMake(fpSpread1, i,
                            SystemBase.Base.GridHeadIndex(GHIdx1, "수량") + "|1"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "비고") + "|0"
                            );
                    }

                    //버튼설정
                    UIForm.Buttons.ReButton("111111011001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);
                 }

            }
            else
            {
                if (strInspStatus != "D")
                {
                    //버튼설정
                    UIForm.Buttons.ReButton("110000001001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);
                }
                else
                {
                    //버튼설정
                    UIForm.Buttons.ReButton("111111011001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);
                }
            }
        }
        #endregion	

        #region 그리드 버튼클릭
        protected override void fpButtonClick(int Row, int Column)
        {
            if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "발생공정_2"))
            {
                try
                {
                    string strQuery = " usp_Q_COMMON 'Q090' ,@pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pSPEC1='" + txtWorkorderNo.Text + "', @pSPEC2 = '" + txtProcSeq.Text + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                    string[] strWhere = new string[] { "@pCODE" };
                    string[] strSearch = new string[] { fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "발생공정")].Text };
				
                    UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00097", strQuery, strWhere, strSearch, new int[] { 0 });	//수불구분코드 사용자조회
                    pu.ShowDialog();

                    if (pu.DialogResult == DialogResult.OK)
                    {
                        Regex rx1 = new Regex("#");
                        string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "발생공정")].Text = Msgs[0].ToString();
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "공정명")].Text = Msgs[2].ToString();
                    }
                }
                catch (Exception f)
                {

                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
                }
            }

        }
        #endregion

        #region 그리드 변경이벤트
        protected override void fpSpread1_ChangeEvent(int Row, int Col)
        {
            if (Col == SystemBase.Base.GridHeadIndex(GHIdx1, "발생공정"))
            {
                string strQuery = " usp_Q_COMMON 'Q091' ,@pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pSPEC1='" + txtWorkorderNo.Text + "', @pSPEC2 = '" + txtProcSeq.Text + "'";
                strQuery += ", @pCODE = '" + fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "발생공정")].Text + "'";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

                if (dt.Rows.Count > 0)
                {
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "공정명")].Text = dt.Rows[0][0].ToString();
                }
                else
                {
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "공정명")].Text = "";
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