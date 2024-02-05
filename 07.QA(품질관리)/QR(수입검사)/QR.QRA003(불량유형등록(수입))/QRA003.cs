#region 작성정보
/*********************************************************************/
// 단위업무명 : 불량유형등록(수입)
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-02-25
// 작성내용 : 불량유형등록(수입) 및 관리
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

namespace QR.QRA003
{
    public partial class QRA003 : UIForm.FPCOMM3
    {
        #region 변수선언
        string strDecisionCd = "";		//검사판정 
        string strKey = "";
        string strDefectQty = "";		//불량수
        int SearchRow = 0;
        int SearchColumn = 0;
        string strInspReqNo = "";
        string strPlantCd = "";
        string strInspReqDt = "";
        string strInspStatus = "";
        bool Linked = false;
        #endregion

        #region 생성자
        public QRA003()
        {
            InitializeComponent();
        }

        public QRA003(string param1, string param2, string param3, string param4)
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
        private void QRA003_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            //콤보박스 세팅
            SystemBase.ComboMake.C1Combo(cboSPlantCd, "usp_B_COMMON @pType='TABLE', @pCODE = 'PLANT_CD', @pNAME = 'PLANT_NM', @pSPEC1 = 'B_PLANT_INFO', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//공장
            SystemBase.ComboMake.C1Combo(cboSInspStatus, "usp_B_COMMON @pType='COMM2', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "',@pCODE = 'Q003', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0); //검사진행상태
            SystemBase.ComboMake.C1Combo(cboSDecisionCd, "usp_B_COMMON @pType='COMM', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "',@pCODE = 'Q004', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 9); //판정

            //그리드콤보박스 세팅
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "불량유형")] = SystemBase.ComboMake.ComboOnGrid("usp_Q_COMMON @pType='Q050', @pCODE = 'R', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//불량유형

            //그리드초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
            UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, 0, 0, false);
            UIForm.FPMake.grdCommSheet(fpSpread3, null, G3Head1, G3Head2, G3Head3, G3Width, G3Align, G3Type, G3Color, G3Etc, G3HeadCnt, 0, 0, false);

            //기타 세팅
            cboSPlantCd.SelectedValue = SystemBase.Base.gstrPLANT_CD;
            dtpSInspReqDtFr.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-3).ToString().Substring(0,10);
            dtpSInspReqDtTo.Value = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0, 10);
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
                cboSInspStatus.SelectedValue = strInspStatus;
                SearchExec();
            }

            lnkJump1.Text = "검사판정";  //화면에 보여지는 링크명
            strJumpFileName1 = "QR.QRA011.QRA011"; //호출할 화면명

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

            SystemBase.Base.RodeFormID = "QRA011";			// 이동할 formid를 적어준다.(메뉴id)
            SystemBase.Base.RodeFormText = "검사판정(수입)"; 	// 이동할 폼명을 적어준다(메뉴명)
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

            fpSpread1.Sheets[0].Rows.Count = 0;
            fpSpread2.Sheets[0].Rows.Count = 0;
            fpSpread3.Sheets[0].Rows.Count = 0;

            //기타 세팅
            cboSPlantCd.SelectedValue = SystemBase.Base.gstrPLANT_CD;
            dtpSInspReqDtFr.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-3).ToString().Substring(0,10);
            dtpSInspReqDtTo.Value = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0, 10);
            dtpSInspDtFr.Value = null;
            dtpSInspDtTo.Value = null;
            cboSInspStatus.SelectedValue = "V";

            SystemBase.Validation.GroupBoxControlsLock(groupBox2, true);

            strDecisionCd = "";
            strKey = "";
            strDefectQty = "";
            strInspReqNo = "";
        }
        #endregion

        #region 행추가 버튼 클릭 이벤트
        protected override void RowInsExec()
        {
            if (fpSpread2.Sheets[0].Rows.Count > 0)
            {
                int iRow2 = fpSpread2.Sheets[0].ActiveRowIndex;

                try
                {
                    UIForm.FPMake.RowInsert(fpSpread1);

                    fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "불량수")].Value = 0;
                    fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "KEY")].Text = fpSpread2.Sheets[0].Cells[iRow2, SystemBase.Base.GridHeadIndex(GHIdx2, "KEY")].Text;
                    fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "검사항목")].Text = fpSpread2.Sheets[0].Cells[iRow2, SystemBase.Base.GridHeadIndex(GHIdx2, "검사항목")].Text;
                    fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "검사차수")].Text = fpSpread2.Sheets[0].Cells[iRow2, SystemBase.Base.GridHeadIndex(GHIdx2, "검사차수")].Text;
                    fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "검사치불량수")].Text = fpSpread2.Sheets[0].Cells[iRow2, SystemBase.Base.GridHeadIndex(GHIdx2, "불량수")].Text;
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "행추가"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                string strQuery = " usp_QRA003  @pTYPE = 'S1'";
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
                    fpSpread2.Sheets[0].Rows.Count = 0;
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

            strDecisionCd = fpSpread3.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx3, "검사판정")].Text;
            strInspReqNo = fpSpread3.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx3, "검사의뢰번호")].Text;

            //groupBox2 값입력
            txtInspReqNo.Value = fpSpread3.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx3, "검사의뢰번호")].Text;
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
                string strQuery = " usp_QRA003  @pTYPE = 'S2'";
                strQuery += ", @pINSP_REQ_NO = '" + fpSpread3.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx3, "검사의뢰번호")].Text + "'";
                strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread2, strQuery, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, false, 0, 0, true);

                if (fpSpread2.Sheets[0].Rows.Count > 0)
                {
                    Grid1_Search1();
                }
                else
                {
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

            try
            {
                strKey = fpSpread2.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx2, "KEY")].Text;

                string strQuery = " usp_QRA003  @pTYPE = 'S3'";
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
                        UIForm.Buttons.ReButton("101000000011", BtnNew, BtnPrint, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnHelp, BtnExcel, BtnClose);
                    }
                    else
                    {
                        UIForm.Buttons.ReButton("101111101011", BtnNew, BtnPrint, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnHelp, BtnExcel, BtnClose);
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

        #region fpSpread1 검사항목에 따른 조회
        private void Grid1_Search2()
        {
            fpSpread1.ActiveSheet.DrawingContainer.Redraw = false;

            int iRow = fpSpread1.Sheets[0].Rows.Count;

            if (strKey != "")
            {
                int j = 0;
                for (int i = 0; i < iRow; i++)
                {
                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "KEY")].Text == strKey)
                    {

                        fpSpread1.ActiveSheet.Rows[i].Visible = true;

                        if (fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text != "U"
                            && fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text != "I"
                            && fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text != "D")
                        {
                            j++;
                            fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text = j.ToString();

                            if (strDecisionCd == "N")
                            {
                                if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "불량원인")].Text != "")
                                {
                                    UIForm.FPMake.grdReMake(fpSpread1, i,
                                        SystemBase.Base.GridHeadIndex(GHIdx1, "불량수") + "|3"
                                        );
                                }
                                else
                                {
                                    UIForm.FPMake.grdReMake(fpSpread1, i,
                                        SystemBase.Base.GridHeadIndex(GHIdx1, "불량수") + "|1"
                                        );
                                }
                            }
                        }
                    }
                    else
                    {
                        fpSpread1.ActiveSheet.Rows[i].Visible = false;
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
                        if (SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", false) == true)
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
                                        string strSql = " usp_QRA003 '" + strGbn + "'";
                                        strSql += ", @pINSP_REQ_NO = '" + txtInspReqNo.Text + "' ";
                                        strSql += ", @pINSP_ITEM_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사항목")].Text + "' ";
                                        strSql += ", @pINSP_SERIES = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사차수")].Value + "' ";
                                        strSql += ", @pDEFECT_TYPE_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "불량유형")].Value + "' ";
                                        strSql += ", @pDEFECT_QTY = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "불량수")].Value + "' ";
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
            //불량수
            if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "불량수"))
            {

                if (fpSpread1.Sheets[0].Rows.Count > 0)
                {
                    if (Convert.ToDouble(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "검사치불량수")].Value)
                        < Convert.ToDouble(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "불량수")].Value))
                    {
                        MessageBox.Show(SystemBase.Base.MessageRtn("불량수가 검사치 불량수보다 클수 없습니다."), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "불량수")].Text = strDefectQty;
                    }
                }
            }
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
                        SystemBase.Base.GridHeadIndex(GHIdx1, "불량수") + "|3"
                        );
                }
                UIForm.Buttons.ReButton("110000001001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);
            }
            else
            {
                for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                {
                    UIForm.FPMake.grdReMake(fpSpread1, i,
                        SystemBase.Base.GridHeadIndex(GHIdx1, "불량수") + "|1"
                        );
                }
                UIForm.Buttons.ReButton("111111011001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);
            }
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

        #region 불량수량
        private void fpSpread1_CellClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            if (fpSpread1.Sheets[0].Rows.Count > 0)
            {
                strDefectQty = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "불량수")].Text;

                if (strDecisionCd == "N")
                {
                    if (fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "불량원인")].Text != "")
                    {
                        UIForm.Buttons.ReButton("111110011001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);
                    }
                    else
                    {
                        UIForm.Buttons.ReButton("111111011001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);
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
