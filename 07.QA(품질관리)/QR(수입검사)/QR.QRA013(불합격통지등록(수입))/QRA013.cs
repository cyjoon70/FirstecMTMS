#region 작성정보
/*********************************************************************/
// 단위업무명 : 불합격통지등록(수입)
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-02-26
// 작성내용 : 불합격통지등록(수입) 및 관리
// 수 정 일 :
// 수 정 자 :
// 수정내용 :
// 비    고 :
/*********************************************************************/
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using WNDW;
using System.Text.RegularExpressions;
using System.Reflection;

namespace QR.QRA013
{
    public partial class QRA013 : UIForm.FPCOMM1
    {
        #region 변수선언
        int SearchRow = 0;
        int SearchColumn = 0;
        int NewFlg = 1;//groupBox3 데이터 수정여부 0:삭제, 1:등록, 2:수정\
        string strInspReqNo = "";
        string strPlantCd = "";
        string strInspReqDt = "";
        string strInspStatus = "";
        bool Linked = false;
        #endregion

        #region 생성자
        public QRA013()
        {
            InitializeComponent();
        }

        public QRA013(string param1, string param2, string param3, string param4)
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
        private void QRA013_Load(object sender, EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            SystemBase.Validation.GroupBox_Setting(groupBox3);

            //콤보박스 세팅
            SystemBase.ComboMake.C1Combo(cboSPlantCd, "usp_B_COMMON @pType='TABLE', @pCODE = 'PLANT_CD', @pNAME = 'PLANT_NM', @pSPEC1 = 'B_PLANT_INFO', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//공장
            SystemBase.ComboMake.C1Combo(cboSInspStatus, "usp_B_COMMON @pType='COMM2', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "',@pCODE = 'Q003', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0); //검사진행상태

            //그리드초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

            //기타 세팅
            cboSPlantCd.SelectedValue = SystemBase.Base.gstrPLANT_CD;
            dtpSInspReqDtFr.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-3).ToString().Substring(0,10);
            dtpSInspReqDtTo.Value = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0, 10);
            cboSInspStatus.SelectedValue = "D";

            SystemBase.Validation.GroupBoxControlsLock(groupBox2, true);

            if (Linked == true)
            {
                cboSPlantCd.SelectedValue = strPlantCd;
                txtSInspReqNo.Text = strInspReqNo;
                dtpSInspReqDtFr.Value = strInspReqDt;
                dtpSInspReqDtTo.Value = strInspReqDt;
                cboSInspStatus.SelectedValue = strInspStatus;
                SearchExec();
            }

            lnkJump1.Text = "RELEASE";  //화면에 보여지는 링크명
            strJumpFileName1 = "QR.QRA021.QRA021"; //호출할 화면명

            lnkJump2.Text = "검사항목등록";  //화면에 보여지는 링크명
            strJumpFileName2 = "QR.QRA001.QRA001"; //호출할 화면명
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

            SystemBase.Base.RodeFormID = "QRA021";			// 이동할 formid를 적어준다.(메뉴id)
            SystemBase.Base.RodeFormText = "RELEASE(수입)"; 	// 이동할 폼명을 적어준다(메뉴명)
        }

        protected override void Link2Exec()
        {
            param = Params();

            SystemBase.Base.RodeFormID = "QRA001";			// 이동할 formid를 적어준다.(메뉴id)
            SystemBase.Base.RodeFormText = "검사항목등록(수입)"; 	// 이동할 폼명을 적어준다(메뉴명)
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
            SystemBase.Validation.GroupBox_Reset(groupBox3);

            fpSpread1.Sheets[0].Rows.Count = 0;

            //기타 세팅
            cboSPlantCd.SelectedValue = SystemBase.Base.gstrPLANT_CD;
            dtpSInspReqDtFr.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-3).ToString().Substring(0,10);
            dtpSInspReqDtTo.Value = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0,10);
            cboSInspStatus.SelectedValue = "D";

            SystemBase.Validation.GroupBoxControlsLock(groupBox2, true);
            SearchRow = 0;
            NewFlg = 1;
            strInspReqNo = "";
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            strInspReqNo = "";
            Grid1_Search();
        }
        #endregion

        #region fpSpread1 조회 로직
        private void Grid1_Search()
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {
                string strPLANT_CD = txtSInspReqNo.Text;
                string strQuery = " usp_QRA013  @pTYPE = 'S1'";
                strQuery += ", @pPLANT_CD = '" + cboSPlantCd.SelectedValue + "' ";
                strQuery += ", @pINSP_REQ_DT_FR = '" + dtpSInspReqDtFr.Text + "' ";
                strQuery += ", @pINSP_REQ_DT_TO = '" + dtpSInspReqDtTo.Text + "' ";
                strQuery += ", @pINSP_DT_FR = '" + dtpSInspDtFr.Text + "' ";
                strQuery += ", @pINSP_DT_TO = '" + dtpSInspDtTo.Text + "' ";
                strQuery += ", @pITEM_CD = '" + txtSItemCd.Text + "' ";
                strQuery += ", @pBP_CD = '" + txtSBpCd.Text + "' ";
                strQuery += ", @pINSP_STATUS = '" + cboSInspStatus.SelectedValue + "' ";
                strQuery += ", @pPROJECT_NO = '" + txtSProjectNo.Text + "'";
                strQuery += ", @pINSP_REQ_NO = '" + txtSInspReqNo.Text + "'";
                strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0, true);

                if (fpSpread1.Sheets[0].Rows.Count > 0)
                {
                    fpSpread1.Search(0, strInspReqNo, true, true, true, true, 0, SystemBase.Base.GridHeadIndex(GHIdx1, "검사의뢰번호"), ref SearchRow, ref SearchColumn);

                    if (SearchRow < 0)
                    { SearchRow = 0; }

                    Grid2_Search(SearchRow);

                    UIForm.FPMake.GridSetFocus(fpSpread1, strPLANT_CD, SystemBase.Base.GridHeadIndex(GHIdx1, "검사의뢰번호"));

                }
                else
                {
                    SystemBase.Validation.GroupBox_Reset(groupBox2);
                    SystemBase.Validation.GroupBoxControlsLock(groupBox3, false);
                    fpSpread1.Sheets[0].Rows.Count = 0;
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }

            this.Cursor = Cursors.Default;
        }
        #endregion

        #region fpSpread1 그리드 선택시 상세정보 조회
        private void fpSpread1_LeaveCell(object sender, FarPoint.Win.Spread.LeaveCellEventArgs e)
        {
            if (fpSpread1.Sheets[0].Rows.Count > 0)
            {
                if (e.Row != e.NewRow)
                {
                    try
                    {
                        SearchRow = e.NewRow;
                        Grid2_Search(SearchRow);
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

        #region 상세정보 조회
        private void Grid2_Search(int iRow)
        {
            this.Cursor = Cursors.WaitCursor;

            SystemBase.Validation.GroupBox_Reset(groupBox2);

            strInspReqNo = fpSpread1.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx1, "검사의뢰번호")].Text;

            //groupBox2 값입력
            txtInspReqNo.Value = fpSpread1.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx1, "검사의뢰번호")].Text;
            dtpInspReqDt.Value = fpSpread1.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx1, "검사의뢰일")].Text;
            dtpInspDemandDt.Value = fpSpread1.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx1, "검사요구일")].Text;
            txtItemCd.Value = fpSpread1.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text;
            txtItemNm.Value = fpSpread1.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx1, "품명")].Text;
            txtBpCd.Value = fpSpread1.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx1, "공급처코드")].Text;
            txtBpNm.Value = fpSpread1.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx1, "공급처명")].Text;
            txtLotSize.Value = Convert.ToDouble(fpSpread1.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx1, "LOT크기")].Value);
            txtStockUnit.Value = fpSpread1.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].Text;
            txtProjectNo.Value = fpSpread1.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text;
            txtProjectNm.Value = fpSpread1.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트명")].Text;
            txtInspStatus.Value = fpSpread1.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx1, "진행상태")].Text;
            txtDecisionCd.Value = fpSpread1.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx1, "검사판정명")].Text;
            txtInspQty.Value = Convert.ToDouble(fpSpread1.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx1, "검사수")].Value);
            txtDefectQty.Value = Convert.ToDouble(fpSpread1.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx1, "불량수")].Value);

            try
            {
                string strQuery = " usp_QRA013  @pTYPE = 'S2'";
                strQuery += ", @pINSP_REQ_NO = '" + fpSpread1.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx1, "검사의뢰번호")].Text + "'";
                strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

                if (dt.Rows.Count > 0)
                {
                    dtpWriteDt.Value = dt.Rows[0][0].ToString();
                    txtWriter.Text = dt.Rows[0][1].ToString();
                    txtDefectState.Text = dt.Rows[0][2].ToString();
                    txtDefectContents.Text = dt.Rows[0][3].ToString();
                    txtImprovement.Text = dt.Rows[0][4].ToString();
                    txtRemark.Text = dt.Rows[0][5].ToString();

                    //화면 Lock 설정
                    if (fpSpread1.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx1, "진행상태코드")].Text != "D"
                        && fpSpread1.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx1, "검사판정")].Text != "R")
                    {
                        //등록폼 Locking설정
                        SystemBase.Validation.GroupBoxControlsLock(groupBox3, true);

                        //버튼설정
                        UIForm.Buttons.ReButton("110000001001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);
                    }
                    else
                    {
                        //등록폼 Locking설정
                        SystemBase.Validation.GroupBoxControlsLock(groupBox3, false);

                        //버튼설정
                        UIForm.Buttons.ReButton("110000111001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);
                    }

                    NewFlg = 2;
                }
                else
                {
                    SystemBase.Validation.GroupBox_Reset(groupBox3);
                    dtpWriteDt.Value = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0,10);
                    txtWriter.Text = SystemBase.Base.gstrUserName;

                    //화면 Lock 설정
                    if (fpSpread1.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx1, "진행상태코드")].Text != "D"
                        && fpSpread1.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx1, "검사판정")].Text != "R")
                    {
                        //버튼설정
                        UIForm.Buttons.ReButton("110000001001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);
                    }
                    else
                    {
                        //버튼설정
                        UIForm.Buttons.ReButton("110000011001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);
                    }

                    NewFlg = 1;
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }

            this.Cursor = Cursors.Default;
        }
        #endregion

        #region DelExec()
        protected override void DeleteExec()
        {
            try
            {
                if (fpSpread1.Sheets[0].Rows.Count > 0)
                {
                    if (MessageBox.Show(SystemBase.Base.MessageRtn("B0047"), "삭제", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        NewFlg = 0;
                        SaveExec();
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "삭제"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        #endregion

        #region SaveExec() 데이타 저장 로직
        protected override void SaveExec()
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
                    //입력폼 필수 체트
                    if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox3))
                    {
                        string strGbn = "";

                        switch (NewFlg)
                        {
                            case 0: strGbn = "D1"; break;
                            case 1: strGbn = "I1"; break;
                            case 2: strGbn = "U1"; break;
                            default: strGbn = ""; break;
                        }

                        string strSql = " usp_QRA013 '" + strGbn + "'";
                        strSql += ", @pINSP_REQ_NO = '" + txtInspReqNo.Text + "'";
                        strSql += ", @pWRITE_DT = '" + dtpWriteDt.Text + "'";
                        strSql += ", @pWRITER = '" + txtWriter.Text + "'";
                        strSql += ", @pDEFECT_STATE = '" + txtDefectState.Text + "'";
                        strSql += ", @pDEFECT_CONTENTS = '" + txtDefectContents.Text + "'";
                        strSql += ", @pIMPROVEMENT = '" + txtImprovement.Text + "'";
                        strSql += ", @pREMARK = '" + txtRemark.Text + "'";
                        strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                        strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                        DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                        ERRCode = ds.Tables[0].Rows[0][0].ToString();
                        MSGCode = ds.Tables[0].Rows[0][1].ToString();

                        if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프							
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
                    if (NewFlg != 2)
                    { Grid1_Search(); }
                    else
                    { Grid2_Search(SearchRow); }

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
