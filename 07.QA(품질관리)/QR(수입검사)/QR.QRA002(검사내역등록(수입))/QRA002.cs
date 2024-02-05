#region 작성정보
/*********************************************************************/
// 단위업무명 : 검사내역등록(수입)
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-02-25
// 작성내용 : 검사내역등록(수입) 및 관리
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
using System.Threading;
using WNDW;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace QR.QRA002
{
    public partial class QRA002 : UIForm.FPCOMM3
    {
        #region 변수선언
        string strSampleNo = "";		//시료번호
        string strDecisionCd = "";		//검사판정 
        string strKey = "";
        string strDefectTypeCd = "";	//불량유형
        int SearchRow = 0;
        int SearchColumn = 0;
        string strInspReqNo = "";
        string strPlantCd = "";
        string strInspReqDt = "";
        string strInspStatus = "";
        string strInspQshowNm = "";
        bool Linked = false;
        Thread th;
        UIForm.ExcelWaiting Waiting_Form = null;
        string FullFileName = "";
        #endregion

        #region 생성자
        public QRA002()
        {
            InitializeComponent();
        }

        public QRA002(string param1, string param2, string param3, string param4)
        {
            //
            // Windows Form 디자이너 지원에 필요합니다.
            strInspReqNo = param1;
            strPlantCd = param2;
            strInspReqDt = param3;
            strInspStatus = param4;
            Linked = true;

            InitializeComponent();

            //
            // TODO: InitializeComponent를 호출한 다음 생성자 코드를 추가합니다.
            //
        }
        #endregion

        #region Form Load시
        private void QRA002_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            //콤보박스 세팅
            SystemBase.ComboMake.C1Combo(cboSPlantCd, "usp_B_COMMON @pType='TABLE', @pCODE = 'PLANT_CD', @pNAME = 'PLANT_NM', @pSPEC1 = 'B_PLANT_INFO', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//공장
            SystemBase.ComboMake.C1Combo(cboSInspStatus, "usp_B_COMMON @pType='COMM2', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "',@pCODE = 'Q003', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0); //검사진행상태
            SystemBase.ComboMake.C1Combo(cboSDecisionCd, "usp_B_COMMON @pType='COMM', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "',@pCODE = 'Q004', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 9); //판정


            //그리드초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
            UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, 0, 0, false);
            UIForm.FPMake.grdCommSheet(fpSpread3, null, G3Head1, G3Head2, G3Head3, G3Width, G3Align, G3Type, G3Color, G3Etc, G3HeadCnt, 0, 0, false);

            //기타 세팅
            cboSPlantCd.SelectedValue = SystemBase.Base.gstrPLANT_CD;
            dtpSInspReqDtFr.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-3).ToString().Substring(0,10).ToString().Substring(0,10);
            dtpSInspReqDtTo.Value = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0, 10).ToString().Substring(0, 10);
            dtpSInspDtFr.Value = null;
            dtpSInspDtTo.Value = null;
            cboSInspStatus.SelectedValue = "V";

            SystemBase.Validation.GroupBoxControlsLock(groupBox2, true);
            SystemBase.Validation.GroupBoxControlsLock(groupBox3, true);
            butSampleCreate.Enabled = false;

            if (Linked == true)
            {
                cboSPlantCd.SelectedValue = strPlantCd;
                txtSInspReqNo.Text = strInspReqNo;
                dtpSInspReqDtFr.Value = strInspReqDt;
                dtpSInspReqDtTo.Value = strInspReqDt;
                cboSInspStatus.SelectedValue = strInspStatus;
                SearchExec();
            }

            lnkJump1.Text = "검사판정";  //화면에 보여지는 링크명
            strJumpFileName1 = "QR.QRA011.QRA011"; //호출할 화면명

            lnkJump2.Text = "불량유형등록";  //화면에 보여지는 링크명
            strJumpFileName2 = "QR.QRA003.QRA003"; //호출할 화면명

            lnkJump3.Text = "검사항목등록";  //화면에 보여지는 링크명
            strJumpFileName3 = "QR.QRA001.QRA001"; //호출할 화면명
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

            SystemBase.Base.RodeFormID = "QRA011";			// 이동할 formid를 적어준다.(메뉴id)
            SystemBase.Base.RodeFormText = "검사판정(수입)"; 	// 이동할 폼명을 적어준다(메뉴명)
        }

        protected override void Link2Exec()
        {
            param = Params();

            SystemBase.Base.RodeFormID = "QRA003";			// 이동할 formid를 적어준다.(메뉴id)
            SystemBase.Base.RodeFormText = "불량유형등록(수입)"; 	// 이동할 폼명을 적어준다(메뉴명)
        }

        protected override void Link3Exec()
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

                    txtSItemCd.Value = Msgs[2].ToString();
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

                    txtSBpCd.Value = Msgs[1].ToString();
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

                    txtSProjectNo.Value = Msgs[3].ToString();
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
            fpSpread2.Sheets[0].Rows.Count = 0;
            fpSpread3.Sheets[0].Rows.Count = 0;

            //기타 세팅
            cboSPlantCd.SelectedValue = SystemBase.Base.gstrPLANT_CD;
            dtpSInspReqDtFr.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-3).ToString().Substring(0, 10);
            dtpSInspReqDtTo.Value = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0, 10);
            dtpSInspDtFr.Value = null;
            dtpSInspDtTo.Value = null;
            cboSInspStatus.SelectedValue = "V";

            SystemBase.Validation.GroupBoxControlsLock(groupBox2, true);
            butSampleCreate.Enabled = false;
            butInspResult.Enabled = false;

            strSampleNo = "";
            strDecisionCd = "";
            strKey = "";
            strDefectTypeCd = "";
            strInspReqNo = "";
        }
        #endregion

        #region 행추가 버튼 클릭 이벤트
        protected override void RowInsExec()
        {
            RowInsert();
            butSampleCreate.Enabled = false;
        }
        #endregion

        #region 행추가 함수
        private void RowInsert()
        {
            if (fpSpread2.Sheets[0].Rows.Count > 0)
            {
                int iRow2 = fpSpread2.Sheets[0].ActiveRowIndex;

                try
                {
                    int iValue = 0, ibig = 0;

                    if (fpSpread1.Sheets[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                        {
                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "KEY")].Text == strKey)
                            {
                                if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "NO")].Text == "")
                                { iValue = 0; }
                                else { iValue = Convert.ToInt32(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "NO")].Value); }

                                if (ibig < iValue)
                                { ibig = iValue; }
                            }
                        }
                    }

                    UIForm.FPMake.RowInsert(fpSpread1);

                    fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "NO")].Value = ibig + 1;
                    fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "품질표시")].Text = fpSpread2.Sheets[0].Cells[iRow2, SystemBase.Base.GridHeadIndex(GHIdx2, "품질표시코드")].Text;
                    fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "검사항목")].Text = fpSpread2.Sheets[0].Cells[iRow2, SystemBase.Base.GridHeadIndex(GHIdx2, "검사항목")].Text;
                    fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "검사차수")].Text = fpSpread2.Sheets[0].Cells[iRow2, SystemBase.Base.GridHeadIndex(GHIdx2, "검사차수")].Text;
                    fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "KEY")].Text = fpSpread2.Sheets[0].Cells[iRow2, SystemBase.Base.GridHeadIndex(GHIdx2, "KEY")].Text;
                    GrdRemake(fpSpread1.Sheets[0].ActiveRowIndex);

                    butSampleCreate.Enabled = false;
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "행추가"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        #endregion

        #region 행복사 버튼 클릭 이벤트
        protected override void RCopyExec()
        {
            if (fpSpread2.Sheets[0].Rows.Count > 0)
            {
                int iValue = 0, ibig = 0;

                try
                {
                    if (fpSpread1.Sheets[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                        {
                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "KEY")].Text == strKey)
                            {
                                if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "NO")].Text == "")
                                { iValue = 0; }
                                else { iValue = Convert.ToInt32(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "NO")].Value); }

                                if (ibig < iValue)
                                { ibig = iValue; }
                            }
                        }

                        UIForm.FPMake.RowCopy(fpSpread1);

                        fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "NO")].Value = ibig + 1;
                        GrdRemake(fpSpread1.Sheets[0].ActiveRowIndex);
                    }
                    else
                    {
                        MessageBox.Show("복사할 데이타가 없습니다.", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "행복사"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        #endregion

        #region  행 취소
        protected override void CancelExec()
        {
            if (fpSpread1.Sheets[0].Rows.Count > 0)
            {
                try
                {
                    if (fpSpread1.ActiveSheet.GetSelection(0) != null)
                    {
                        UIForm.FPMake.Cancel(fpSpread1, fpSpread1.ActiveSheet.GetSelection(0).Row, 1);
                        int Chk = 0;
                        for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                        {
                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "KEY")].Text == strKey)
                            {
                                Chk++;
                            }
                        }

                        if (Chk != 0)
                        {
                            butSampleCreate.Enabled = false;
                        }
                        else
                        {
                            butSampleCreate.Enabled = true;
                        }
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log("Cancel (그리드 취소버튼클릭 에러)", f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0008"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        #endregion

        #region 행삭제
        protected override void DelExec()
        {
            if (fpSpread1.Sheets[0].Rows.Count > 0)
            {
                try
                {
                    UIForm.FPMake.RowRemove(fpSpread1);

                    int Chk = 0;
                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {
                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "KEY")].Text == strKey)
                        {
                            Chk++;
                        }
                    }

                    if (Chk != 0)
                    {
                        butSampleCreate.Enabled = false;
                    }
                    else
                    {
                        butSampleCreate.Enabled = true;
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log("RowRemove (그리드 삭제버튼 클릭에러)", f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0007"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        #endregion

        #region DelExec() 삭제 로직
        protected override void DeleteExec()
        {
            if (fpSpread1.Sheets[0].Rows.Count > 0)
            {
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("P0008"), SystemBase.Base.MessageRtn("Z0004"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (dsMsg == DialogResult.Yes)
                {
                    string ERRCode = "WR", MSGCode = "P0000";	//처리할 내용이 없습니다.
                    SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                    SqlCommand cmd = dbConn.CreateCommand();
                    SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                    try
                    {
                        string strDelSql = " usp_QRA002  'D2'";
                        strDelSql += ", @pINSP_REQ_NO = '" + txtInspReqNo.Text + "' ";
                        strDelSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                        DataSet ds = SystemBase.DbOpen.TranDataSet(strDelSql, dbConn, Trans);
                        ERRCode = ds.Tables[0].Rows[0][0].ToString();
                        MSGCode = ds.Tables[0].Rows[0][1].ToString();

                        if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

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
                        MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Grid3_Search();
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
            }
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            strInspReqNo = "";
            Grid3_Search();
        }
        #endregion

        #region fpSpread3 그리드 조회
        private void Grid3_Search()
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {
                string strQuery = " usp_QRA002  @pTYPE = 'S1'";
                strQuery += ", @pPLANT_CD = '" + cboSPlantCd.SelectedValue + "' ";
                strQuery += ", @pINSP_REQ_DT_FR = '" + dtpSInspReqDtFr.Text + "' ";
                strQuery += ", @pINSP_REQ_DT_TO = '" + dtpSInspReqDtTo.Text + "' ";
                strQuery += ", @pINSP_DT_FR = '" + dtpSInspDtFr.Text + "' ";
                strQuery += ", @pINSP_DT_TO = '" + dtpSInspDtTo.Text + "' ";
                strQuery += ", @pITEM_CD = '" + txtSItemCd.Text + "' ";
                strQuery += ", @pBP_CD = '" + txtSBpCd.Text + "' ";
                strQuery += ", @pINSP_STATUS = '" + cboSInspStatus.SelectedValue + "' ";
                strQuery += ", @pDECISION_CD = '" + cboSDecisionCd.SelectedValue + "'";
                strQuery += ", @pPROJECT_NO = '" + txtSProjectNo.Text + "'";
                strQuery += ", @pINSP_REQ_NO = '" + txtSInspReqNo.Text + "'";
                strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread3, strQuery, G3Head1, G3Head2, G3Head3, G3Width, G3Align, G3Type, G3Color, G3Etc, G3HeadCnt, false, false, 0, 0, true);

                if (fpSpread3.Sheets[0].Rows.Count > 0)
                {
                    fpSpread3.Search(0, strInspReqNo, true, true, true, true, 0, SystemBase.Base.GridHeadIndex(GHIdx3, "검사의뢰번호"), ref SearchRow, ref SearchColumn);

                    if (SearchRow < 0)
                    { SearchRow = 0; }

                    Grid2_Search(SearchRow);
                    fpSpread3.Focus();
                    fpSpread3.ActiveSheet.SetActiveCell(SearchRow, 1); //Row Focus		
                    fpSpread3.ShowRow(0, SearchRow, FarPoint.Win.Spread.VerticalPosition.Center); //Focus Row 보기
                }
                else
                {
                    SystemBase.Validation.GroupBox_Reset(groupBox2);
                    SystemBase.Validation.GroupBox_Reset(groupBox3);
                    fpSpread2.Sheets[0].Rows.Count = 0;
                    fpSpread1.Sheets[0].Rows.Count = 0;
                    butSampleCreate.Enabled = false;
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

        #region fpSpread3 그리드 선택시 상세정보 조회
        private void fpSpread3_LeaveCell(object sender, FarPoint.Win.Spread.LeaveCellEventArgs e)
        {
            if (e.Row != e.NewRow)
            {
                if (fpSpread3.Sheets[0].Rows.Count > 0)
                {
                    try
                    {
                        SearchRow = e.NewRow;
                        Grid2_Search(SearchRow);
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

        #region fpSpread2 그리드 조회
        private void Grid2_Search(int iRow)
        {
            this.Cursor = Cursors.WaitCursor;

            SystemBase.Validation.GroupBox_Reset(groupBox2);
            SystemBase.Validation.GroupBox_Reset(groupBox3);

            strDecisionCd = fpSpread3.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx3, "검사판정")].Text;
            if (fpSpread3.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx3, "통합의뢰번호")].Text.Trim().ToString() == "")
            {
                strInspReqNo = fpSpread3.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx3, "검사의뢰번호")].Text;
            }
            else
            {
                strInspReqNo = fpSpread3.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx3, "통합의뢰번호")].Text;
            }

            //groupBox2 값입력
            txtInspReqNo.Value = strInspReqNo;
            dtpInspReqDt.Value = fpSpread3.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx3, "검사의뢰일")].Text;
            dtpInspDemandDt.Value = fpSpread3.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx3, "검사요구일")].Text;
            txtItemCd.Value = fpSpread3.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx3, "품목코드")].Text;
            txtItemNm.Value = fpSpread3.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx3, "품명")].Text;
            txtBpCd.Value = fpSpread3.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx3, "공급처코드")].Text;
            txtBpNm.Value = fpSpread3.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx3, "공급처명")].Text;
            txtLotSize.Value = Convert.ToDouble(fpSpread3.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx3, "LOT크기")].Value);
            txtStockUnit.Value = fpSpread3.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx3, "단위")].Text;
            txtProjectNo.Value = fpSpread3.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx3, "프로젝트번호")].Text;
            txtProjectNm.Value = fpSpread3.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx3, "프로젝트명")].Text;
            txtInspStatus.Value = fpSpread3.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx3, "진행상태")].Text;
            txtDecisionCd.Value = fpSpread3.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx3, "검사판정명")].Text;
            txtInspQty.Value = Convert.ToDouble(fpSpread3.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx3, "검사수")].Value);
            txtDefectQty.Value = Convert.ToDouble(fpSpread3.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx3, "불량수")].Value);

            try
            {
                string strQuery = " usp_QRA002  @pTYPE = 'S2'";
                strQuery += ", @pINSP_REQ_NO = '" + strInspReqNo + "'";
                strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread2, strQuery, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, false, 0, 0, true);

                if (fpSpread2.Sheets[0].Rows.Count > 0)
                {
                    Grid1_Search1();
                    butInspResult.Enabled = true;
                }
                else
                {
                    SystemBase.Validation.GroupBox_Reset(groupBox3);
                    fpSpread1.Sheets[0].Rows.Count = 0;
                    butSampleCreate.Enabled = false;
                    butInspResult.Enabled = false;
                }
                strQuery = " usp_QRA002  @pTYPE = 'S4'";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                strQuery += ", @pITEM_CD = '" + txtItemCd.Text + "' ";
                DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);
                if (dt.Rows[0]["FILEEXTENSIONS"].ToString() == "JPG" || dt.Rows[0]["FILEEXTENSIONS"].ToString() == "BMP" || dt.Rows[0]["FILEEXTENSIONS"].ToString() == "GIF")
                {
                    btnITEM_PICTURE.Enabled = true;

                    string FtpFile = "ftp://172.30.24.14/ITEM_IMAGE/";
                    FullFileName = FtpFile + txtItemCd.Text;
                }
                else
                {
                    btnITEM_PICTURE.Enabled = false;
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

        #region fpSpread2 그리드 선택시 상세정보 조회
        private void fpSpread2_LeaveCell(object sender, FarPoint.Win.Spread.LeaveCellEventArgs e)
        {
            if (e.Row != e.NewRow)
            {
                if (fpSpread2.Sheets[0].Rows.Count > 0)
                {
                    try
                    {
                        strKey = fpSpread2.Sheets[0].Cells[e.NewRow, SystemBase.Base.GridHeadIndex(GHIdx2, "KEY")].Text;
                        strDefectTypeCd = fpSpread2.Sheets[0].Cells[e.NewRow, SystemBase.Base.GridHeadIndex(GHIdx2, "불량유형")].Text;
                        strInspQshowNm = fpSpread2.Sheets[0].Cells[e.NewRow, SystemBase.Base.GridHeadIndex(GHIdx2, "품질표시")].Text;
                        //groupBox3 값입력
                        txtSampleQty.Value = Convert.ToDouble(fpSpread2.Sheets[0].Cells[e.NewRow, SystemBase.Base.GridHeadIndex(GHIdx2, "시료수")].Value);
                        txtDefectQty1.Value = Convert.ToDouble(fpSpread2.Sheets[0].Cells[e.NewRow, SystemBase.Base.GridHeadIndex(GHIdx2, "불량수")].Value);
                        txtInspMethNm.Value = fpSpread2.Sheets[0].Cells[e.NewRow, SystemBase.Base.GridHeadIndex(GHIdx2, "검사방식")].Text;
                        txtInspQshowNm.Value = fpSpread2.Sheets[0].Cells[e.NewRow, SystemBase.Base.GridHeadIndex(GHIdx2, "품질표시")].Text;
                        txtInspSpec.Value = fpSpread2.Sheets[0].Cells[e.NewRow, SystemBase.Base.GridHeadIndex(GHIdx2, "검사규격")].Text;

                        if (fpSpread2.Sheets[0].Cells[e.NewRow, SystemBase.Base.GridHeadIndex(GHIdx2, "하한규격")].Text != "")
                            txtInspLsl.Value = Convert.ToDouble(fpSpread2.Sheets[0].Cells[e.NewRow, SystemBase.Base.GridHeadIndex(GHIdx2, "하한규격")].Value);

                        if (fpSpread2.Sheets[0].Cells[e.NewRow, SystemBase.Base.GridHeadIndex(GHIdx2, "상한규격")].Text != "")
                            txtInspUsl.Value = Convert.ToDouble(fpSpread2.Sheets[0].Cells[e.NewRow, SystemBase.Base.GridHeadIndex(GHIdx2, "상한규격")].Value);

                        txtMeasureNm.Value = fpSpread2.Sheets[0].Cells[e.NewRow, SystemBase.Base.GridHeadIndex(GHIdx2, "측정기")].Text;
                        txtMeasureUnit.Value = fpSpread2.Sheets[0].Cells[e.NewRow, SystemBase.Base.GridHeadIndex(GHIdx2, "단위")].Text;

                        Grid1_Search2();
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

        #region fpSpread1 그리드 전체 조회
        private void Grid1_Search1()
        {
            this.Cursor = Cursors.WaitCursor;

            SystemBase.Validation.GroupBox_Reset(groupBox3);

            strKey = fpSpread2.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx2, "KEY")].Text;
            strDefectTypeCd = fpSpread2.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx2, "불량유형")].Text;
            strInspQshowNm = fpSpread2.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx2, "품질표시")].Text;

            //groupBox3 값입력
            txtSampleQty.Value = Convert.ToDouble(fpSpread2.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx2, "시료수")].Value);
            txtDefectQty1.Value = Convert.ToDouble(fpSpread2.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx2, "불량수")].Value);
            txtInspMethNm.Value = fpSpread2.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx2, "검사방식")].Text;
            txtInspQshowNm.Value = fpSpread2.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx2, "품질표시")].Text;
            txtInspSpec.Value = fpSpread2.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx2, "검사규격")].Text;

            if (fpSpread2.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx2, "하한규격")].Text != "")
                txtInspLsl.Value = Convert.ToDouble(fpSpread2.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx2, "하한규격")].Value);

            if (fpSpread2.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx2, "상한규격")].Text != "")
                txtInspUsl.Value = Convert.ToDouble(fpSpread2.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx2, "상한규격")].Value);

            txtMeasureNm.Value = fpSpread2.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx2, "측정기")].Text;
            txtMeasureUnit.Value = fpSpread2.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx2, "단위")].Text;

            try
            {
                string strQuery = " usp_QRA002  @pTYPE = 'S3'";
                strQuery += ", @pINSP_REQ_NO = '" + txtInspReqNo.Text + "'";
                strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false, false);

                if (fpSpread1.Sheets[0].Rows.Count > 0)
                {
                    Grd_Lock();
                    Grid1_Search2();
                }
                else
                {
                    if (strDecisionCd != "N")
                    {
                        UIForm.Buttons.ReButton("110000001001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);
                    }
                    else
                    {
                        UIForm.Buttons.ReButton("111111111001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);
                    }
                    butSampleCreate.Enabled = true;
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

        #region fpSpread1 검사항목에 따른 조회
        private void Grid1_Search2()
        {
            fpSpread1.ActiveSheet.DrawingContainer.Redraw = false;

            int iRow = fpSpread1.Sheets[0].Rows.Count;
            int Chk = 0;

            if (strKey != "")
            {
                int j = 0;
                for (int i = 0; i < iRow; i++)
                {
                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "KEY")].Text == strKey)
                    {
                        Chk++;
                        fpSpread1.ActiveSheet.Rows[i].Visible = true;

                        if (fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text != "U"
                            && fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text != "I"
                            && fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text != "D")
                        {
                            j++;
                            fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text = j.ToString();

                            if (strDecisionCd == "N")
                            {
                                if (strDefectTypeCd != "")	//불량유형이 등록되었다면
                                {
                                    UIForm.FPMake.grdReMake(fpSpread1, i,
                                        SystemBase.Base.GridHeadIndex(GHIdx1, "측정치") + "|3"
                                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "불량") + "|3"
                                        );
                                }
                                else
                                {
                                    UIForm.FPMake.grdReMake(fpSpread1, i,
                                        SystemBase.Base.GridHeadIndex(GHIdx1, "측정치") + "|1"
                                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "불량") + "|0"
                                        );

                                    GrdRemake(i);
                                }
                            }
                        }
                        else
                        {
                            GrdRemake(i);
                        }
                    }
                    else
                    {
                        fpSpread1.ActiveSheet.Rows[i].Visible = false;
                        UIForm.FPMake.grdReMake(fpSpread1, i,
                            SystemBase.Base.GridHeadIndex(GHIdx1, "측정치") + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "불량") + "|3"
                            );
                    }
                }

                if (Chk != 0)
                {
                    butSampleCreate.Enabled = false;
                }
                else
                {
                    butSampleCreate.Enabled = true;

                }

                if (strDecisionCd == "N")
                {
                    if (strDefectTypeCd != "")	//불량유형이 등록되었다면
                    {
                        UIForm.Buttons.ReButton("111110011001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);
                    }
                    else
                    {
                        UIForm.Buttons.ReButton("111111111001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);
                    }
                }

                fpSpread1.ActiveSheet.DrawingContainer.Redraw = true;
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
                        if (strInspQshowNm == "특성치")
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

                                        if (strGbn != "")
                                        {
                                            string strSql = " usp_QRA002 '" + strGbn + "'";
                                            strSql += ", @pINSP_REQ_NO = '" + txtInspReqNo.Text + "' ";
                                            strSql += ", @pINSP_ITEM_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사항목")].Text + "' ";
                                            strSql += ", @pINSP_SERIES = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사차수")].Value + "' ";
                                            strSql += ", @pSAMPLE_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "NO")].Value + "' ";

                                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "측정치")].Text != "")
                                                strSql += ", @pINSP_VALUE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "측정치")].Value + "' ";

                                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "불량")].Text == "True")
                                                strSql += ", @pGB_TYPE = 'B' ";
                                            else
                                                strSql += ", @pGB_TYPE = 'G' ";

                                            strSql += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' ";
                                            strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                                            strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                                            DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                                            ERRCode = ds.Tables[0].Rows[0][0].ToString();
                                            MSGCode = ds.Tables[0].Rows[0][1].ToString();

                                            if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프	
                                        }
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
                        else
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

                                    if (strGbn != "")
                                    {
                                        string strSql = " usp_QRA002 '" + strGbn + "'";
                                        strSql += ", @pINSP_REQ_NO = '" + txtInspReqNo.Text + "' ";
                                        strSql += ", @pINSP_ITEM_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사항목")].Text + "' ";
                                        strSql += ", @pINSP_SERIES = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사차수")].Value + "' ";
                                        strSql += ", @pSAMPLE_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "NO")].Value + "' ";

                                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "측정치")].Text != "")
                                            strSql += ", @pINSP_VALUE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "측정치")].Value + "' ";

                                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "불량")].Text == "True")
                                            strSql += ", @pGB_TYPE = 'B' ";
                                        else
                                            strSql += ", @pGB_TYPE = 'G' ";

                                        strSql += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' ";
                                        strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                                        strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                                        DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                                        ERRCode = ds.Tables[0].Rows[0][0].ToString();
                                        MSGCode = ds.Tables[0].Rows[0][1].ToString();

                                        if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프	
                                    }
                                }
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
                    }
                Exit:
                    dbConn.Close();
                    if (ERRCode == "OK")
                    {
                        Grid3_Search();
                       
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

        #region fpSpread1 Change 이벤트
        protected override void fpSpread1_ChangeEvent(int Row, int Column)
        {

            //시료번호
            if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "NO"))
            {

                string NowValue = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "NO")].Text;

                if (fpSpread1.Sheets[0].Rows.Count > 0)
                {
                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {

                        if (NowValue == fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "NO")].Text && Row != i
                            && fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "KEY")].Text == strKey)
                        {
                            MessageBox.Show("시료번호는 동일한 값을 입력할 수 없습니다!", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            fpSpread1.ActiveSheet.SetActiveCell(Row, SystemBase.Base.GridHeadIndex(GHIdx1, "NO"));

                            NowValue = "";
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "NO")].Text = strSampleNo;

                            if (fpSpread1.Sheets[0].RowHeader.Cells[Row, 0].Text != "I")
                                fpSpread1.Sheets[0].RowHeader.Cells[Row, 0].Text = "";
                        }

                    }
                }

            }

            //측정치
            if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "측정치"))
            {

                if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "측정치")].Text != "")
                {
                    string strInspQshow = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품질표시")].Text;

                    if (strInspQshow == "2") //결점수일경우
                    {
                        if (txtInspLsl.Text != "")
                        {
                            if (Convert.ToDouble(txtInspLsl.Value) < Convert.ToDouble(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "측정치")].Value))
                            {
                                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "불량")].Text = "True";
                            }
                            else
                            {
                                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "불량")].Text = "";
                            }
                        }
                    }
                    else if (strInspQshow == "3") //특성치
                    {

                        if (txtInspLsl.Text != "" && txtInspUsl.Text != "")
                        {
                            if (Convert.ToDouble(txtInspLsl.Value) > Convert.ToDouble(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "측정치")].Value)
                                || Convert.ToDouble(txtInspUsl.Value) < Convert.ToDouble(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "측정치")].Value))
                            {
                                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "불량")].Text = "True";
                            }
                            else
                            {
                                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "불량")].Text = "";
                            }
                        }
                        else if (txtInspLsl.Text != "" && txtInspUsl.Text == "")
                        {
                            if (Convert.ToDouble(txtInspLsl.Value) > Convert.ToDouble(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "측정치")].Value))
                            {
                                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "불량")].Text = "True";
                            }
                            else
                            {
                                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "불량")].Text = "";
                            }
                        }
                        else if (txtInspLsl.Text == "" && txtInspUsl.Text != "")
                        {
                            if (Convert.ToDouble(txtInspUsl.Value) < Convert.ToDouble(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "측정치")].Value))
                            {
                                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "불량")].Text = "True";
                            }
                            else
                            {
                                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "불량")].Text = "";
                            }
                        }
                    }
                }
                else
                {
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "불량")].Text = "";
                }
            }

        }

        private void fpSpread1_CellClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            if (fpSpread1.Sheets[0].Rows.Count > 0)
                strSampleNo = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "NO")].Text;
        }
        #endregion

        #region 검사치정보등록 그리드 속성재정의
        private void GrdRemake(int iRow)
        {
            if (fpSpread1.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx1, "품질표시")].Text == "1")
            {
                UIForm.FPMake.grdReMake(fpSpread1, iRow, SystemBase.Base.GridHeadIndex(GHIdx1, "측정치") + "|3#"
                    + SystemBase.Base.GridHeadIndex(GHIdx1, "불량")+"|0");
            }
            else
            {
                UIForm.FPMake.grdReMake(fpSpread1, iRow, SystemBase.Base.GridHeadIndex(GHIdx1, "측정치")+"|1#"
                    + SystemBase.Base.GridHeadIndex(GHIdx1, "불량") + "|3");
            }
        }
        #endregion

        #region 시료행 생성
        private void butSampleCreate_Click(object sender, System.EventArgs e)
        {
            int iSampleQty = 0;

            if (Convert.ToDecimal(txtSampleQty.Value) < 1 & Convert.ToDecimal(txtSampleQty.Value) > 0)
            {
                iSampleQty = 1;
            }
            else
            {
                iSampleQty = Convert.ToInt32(txtSampleQty.Value);
            }

            for (int i = 0; i < iSampleQty; i++)
            {
                RowInsert();
            }

            fpSpread1.ActiveSheet.SetActiveCell(fpSpread1.Sheets[0].ActiveRowIndex - iSampleQty + 1, 1);

            fpSpread1.ActiveSheet.AddSelection(0, 1, 1, 1);
            fpSpread1.ShowActiveCell(FarPoint.Win.Spread.VerticalPosition.Nearest, FarPoint.Win.Spread.HorizontalPosition.Nearest);

            butSampleCreate.Enabled = false;
        }
        #endregion

        #region 판정여부에 따른 화면 Locking, 버튼설정
        private void Grd_Lock()
        {
            //판정여부에 따른 화면 Locking, 버튼설정
            if (strDecisionCd != "N")	//판정
            {
                for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                {
                    UIForm.FPMake.grdReMake(fpSpread1, i,
                        SystemBase.Base.GridHeadIndex(GHIdx1, "측정치") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "불량") + "|3"
                        );
                }
                UIForm.Buttons.ReButton("110000001001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);
            }
            else
            {
                for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                {
                    UIForm.FPMake.grdReMake(fpSpread1, i,
                        SystemBase.Base.GridHeadIndex(GHIdx1, "측정치") + "|1"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "불량") + "|0"
                        );

                    GrdRemake(i);
                }
                UIForm.Buttons.ReButton("111111011001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);
            }

            butSampleCreate.Enabled = false;
        }
        #endregion

        #region FPUpCheck - 그리드 데이타 Check
        private bool FPUpCheck2(FarPoint.Win.Spread.FpSpread fpSpread1, bool EditCheck)
        {
            bool ChkGrid = true;
            int UpCount = 0;
            int MsgRow = 0;
            try
            {
                for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                {	//필수입력사항 체크
                    if (fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text == "I" || fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text == "U" || fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text == "D")
                    {
                        for (int j = 0; j < fpSpread1.Sheets[0].Columns.Count; j++)
                        {
                            if (((fpSpread1.Sheets[0].Cells[i, j].BackColor.Name.ToString() == "Gainsboro"
                                || fpSpread1.Sheets[0].Cells[i, j].BackColor.Name.ToString() == "LightCyan")
                                && (fpSpread1.Sheets[0].Cells[i, j].Value == null
                                || fpSpread1.Sheets[0].Cells[i, j].Value.ToString().Length == 0))
                                && fpSpread1.Sheets[0].GetCellType(i, j).ToString() != "GeneralCellType"
                                && fpSpread1.Sheets[0].GetCellType(i, j).ToString() != "ButtonCellType"
                                && fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text != "D"
                                )
                            {
                                string KEY = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "KEY")].Text;

                                for (int k = 0; k < fpSpread1.Sheets[0].Rows.Count; k++)
                                {
                                    if (KEY == fpSpread1.Sheets[0].Cells[k, SystemBase.Base.GridHeadIndex(GHIdx1, "KEY")].Text)
                                    {
                                        MsgRow++;
                                        if (((fpSpread1.Sheets[0].Cells[k, j].BackColor.Name.ToString() == "Gainsboro"
                                            || fpSpread1.Sheets[0].Cells[k, j].BackColor.Name.ToString() == "LightCyan")
                                            && (fpSpread1.Sheets[0].Cells[k, j].Value == null
                                            || fpSpread1.Sheets[0].Cells[k, j].Value.ToString().Length == 0))
                                            && fpSpread1.Sheets[0].GetCellType(k, j).ToString() != "GeneralCellType"
                                            && fpSpread1.Sheets[0].GetCellType(k, j).ToString() != "ButtonCellType"
                                            && fpSpread1.Sheets[0].RowHeader.Cells[k, 0].Text != "D"
                                            )
                                        {
                                            string strInspItemCd = fpSpread1.Sheets[0].Cells[k, SystemBase.Base.GridHeadIndex(GHIdx1, "검사항목")].Text;
                                            string strInspSeries = fpSpread1.Sheets[0].Cells[k, SystemBase.Base.GridHeadIndex(GHIdx1, "검사차수")].Text;
                                            MessageBox.Show("검사항목 " + strInspItemCd + "의 검사차수 " + strInspSeries + " 의 " + Convert.ToString(MsgRow) + "번째 Row의 [ " + fpSpread1.Sheets[0].ColumnHeader.Cells[0, j].Text.ToString() + " ] 항목은 필수입력 항목입니다.", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                            ChkGrid = false;
                                            break;

                                        }
                                    }
                                }
                            }
                        }
                        UpCount++;
                    }
                    if (ChkGrid == false)
                        break;
                }

                if (UpCount == 0 && EditCheck == true)
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0004"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);//변경된 데이타가 없습니다.
                    ChkGrid = false;
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log("FPUpCheck 2 (그리드 필수항목 체크시 에러발생)", f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0005"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return ChkGrid;
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
                    //th = new Thread(new ThreadStart(Show_Waiting));       // 2015.05.18. hma 주석 처리
                    //th.Start();
                    //Thread.Sleep(200);
                    //Waiting_Form.Activate();
                    this.Cursor = Cursors.WaitCursor;                   // 2015.05.18. hma 추가
                    
                    string strQuery = " usp_QRA002  @pTYPE = 'R1'";
                    strQuery += ", @pINSP_REQ_NO = '" + txtInspReqNo.Text + "' ";
                    strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' ";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

                    if (dt.Rows.Count > 0)
                    {

                        //Waiting_Form.progressBar_temp.Maximum = dt.Rows.Count;        // 2015.05.18. hma 주석 처리

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

                        // 2015.05.18. hma 추가(Start): 검사책임자를 넘겨받아서 출력하도록 함.
                        excel.SetCell(7, 15, dt.Rows[0]["QC_MAN_NAME"].ToString());
                        // 2015.05.18. hma 추가(End)


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
                                int iNextRow;
                                
                                if((Convert.ToInt32(dt.Rows[i - 1]["SAMPLE_QTY"].ToString()) <= 1))
                                    iNextRow = iRow + 2;
                                else
                                 iNextRow = (iRow + ((Convert.ToInt32(dt.Rows[i - 1]["SAMPLE_QTY"].ToString()) * 2) - iUseRow)) + 2;

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
                            //Waiting_Form.progressBar_temp.Value = i + 1;      // 2015.05.18. hma 주석 처리

                        }
                        //통합 나오는곳.
                        if (dt.Rows[dt.Rows.Count - 1]["UNITY_INSP_REQ_NO"].ToString() != "")
                        {
                            string strQuery3 = " usp_QRA002  @pTYPE = 'R3'";
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
                                        excel.SetCell(NextPage + a + 2, 2, dt3.Rows[a]["INSP_REQ_NO"].ToString());
                                        excel.SetCell(NextPage + a + 2, 13, dt3.Rows[a]["WORKORDER_NO"].ToString());
                                    }
                                    else
                                    {
                                        excel.SetCell(NextPage + a + 2 - 35, 5, dt3.Rows[a]["INSP_REQ_NO"].ToString());
                                        excel.SetCell(NextPage + a + 2 - 35, 22, dt3.Rows[a]["WORKORDER_NO"].ToString());
                                    }
                                }
                            }
                        }
                        excel.SetSelect("A1", "A1");

                        //Waiting_Form.label_temp.Text = "완료되었습니다.";        // 2015.05.18. hma 주석 처리
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
                    //Waiting_Form.Close();         // 2015.05.18. hma 주석 처리 
                    //th.Abort();
                    File.SetAttributes(strFileName, System.IO.FileAttributes.Normal);
                }
                this.Cursor = Cursors.Default;      // 2015.05.18. hma 추가
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

        #region 품목사진 뷰어
        private void btnITEM_PICTURE_Click(object sender, EventArgs e)
        {
            WNDW038 pu = new WNDW038(FullFileName);
            pu.ShowDialog();
            if (pu.DialogResult == DialogResult.OK)
            {
            }
        }
        #endregion
        	
    }
}
