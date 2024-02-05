#region 작성정보
/*********************************************************************/
// 단위업무명 : 불량유형등록(최종)
// 작 성 자 : 김 현 근
// 작 성 일 : 2013-04-03
// 작성내용 : 불량유형등록(최종) 및 관리
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
using FarPoint.Win.Spread;
using System.Text.RegularExpressions;

namespace QT.QTC002
{
    public partial class QTC002 : UIForm.FPCOMM3
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
        string strActiveSpread = "";

        #endregion

        #region 생성자
        public QTC002()
        {
            InitializeComponent();
        }

        public QTC002(string param1, string param2, string param3, string param4)
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
        private void QTC002_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            //콤보박스 세팅
            SystemBase.ComboMake.C1Combo(cboSPlantCd, "usp_B_COMMON @pType='TABLE', @pCODE = 'PLANT_CD', @pNAME = 'PLANT_NM', @pSPEC1 = 'B_PLANT_INFO', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//공장
            SystemBase.ComboMake.C1Combo(cboSInspStatus, "usp_B_COMMON @pType='COMM2', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "',@pCODE = 'Q003', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0); //검사진행상태
            SystemBase.ComboMake.C1Combo(cboSDecisionCd, "usp_B_COMMON @pType='COMM', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "',@pCODE = 'Q004', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 9); //판정

            //그리드콤보박스 세팅
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "불량유형")] = SystemBase.ComboMake.ComboOnGrid("usp_Q_COMMON @pType='Q050', @pCODE = 'P', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//불량유형

            //그리드초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
            UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, 0, 0, false);
            UIForm.FPMake.grdCommSheet(fpSpread3, null, G3Head1, G3Head2, G3Head3, G3Width, G3Align, G3Type, G3Color, G3Etc, G3HeadCnt, 0, 0, false);

            //기타 세팅
            cboSPlantCd.SelectedValue = SystemBase.Base.gstrPLANT_CD;
            dtpSInspReqDtFr.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-3).ToString().Substring(0, 10).ToString().Substring(0, 10);
            dtpSInspReqDtTo.Value = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0, 10).ToString().Substring(0, 10);
            dtpSInspDtFr.Value = null;
            dtpSInspDtTo.Value = null;
            cboSInspStatus.SelectedValue = "V";

            SystemBase.Validation.GroupBoxControlsLock(groupBox2, true);

            cboSInspStatus.SelectedValue = "R";

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

            SystemBase.Base.RodeFormID = "QFA004";			// 이동할 formid를 적어준다.(메뉴id)
            SystemBase.Base.RodeFormText = "불량원인등록(최종)"; 	// 이동할 폼명을 적어준다(메뉴명)
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
            fpSpread3.Sheets[0].Rows.Count = 0;

            //기타 세팅
            cboSPlantCd.SelectedValue = SystemBase.Base.gstrPLANT_CD;
            dtpSInspReqDtFr.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-3).ToString().Substring(0, 10);
            dtpSInspReqDtTo.Value = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0, 10);
            dtpSInspDtFr.Value = null;
            dtpSInspDtTo.Value = null;
            cboSInspStatus.SelectedValue = "V";

            SystemBase.Validation.GroupBoxControlsLock(groupBox2, true);

            strDecisionCd = "";
            strKey = "";
            strDefectQty = "";
            strInspReqNo = "";
            strActiveSpread = "";
        }
        #endregion

        #region 행삭제 버튼 클릭 이벤트
        protected override void DelExec()
        {
            if (strActiveSpread != "")
            {
                if (strActiveSpread == "fpSpread2")
                {
                    if (fpSpread2.Sheets[0].Rows.Count > 0)
                    {
                        int iRow = fpSpread2.Sheets[0].ActiveRowIndex;

                        try
                        {
                            Private_RowRemove(fpSpread2);
                        }
                        catch (Exception f)
                        {
                            SystemBase.Loggers.Log(this.Name, f.ToString());
                            MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "행삭제"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                else
                {
                    if (fpSpread1.Sheets[0].Rows.Count > 0)
                    {
                        int iRow = fpSpread1.Sheets[0].ActiveRowIndex;

                        try
                        {
                            Private_RowRemove(fpSpread1);
                        }
                        catch (Exception f)
                        {
                            SystemBase.Loggers.Log(this.Name, f.ToString());
                            MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "행삭제"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
        }
        #endregion

        #region Private RowRemove - 그리드 삭제 플레그 등록
        private static void Private_RowRemove(FarPoint.Win.Spread.FpSpread fpSpread1)
        {
            try
            {
                int BeforeRow = 0;
                int Col = 0;
                if (fpSpread1.ActiveSheet.GetSelection(0) == null)
                {
                    BeforeRow = fpSpread1.ActiveSheet.ActiveRowIndex;
                    Col = fpSpread1.ActiveSheet.ActiveColumnIndex; ;
                }
                else
                {
                    BeforeRow = fpSpread1.ActiveSheet.GetSelection(0).Row;
                    Col = fpSpread1.ActiveSheet.GetSelection(0).Column;
                }
                int BeforeRowCount = 1;
                if (fpSpread1.ActiveSheet.GetCellType(BeforeRow, Col).ToString() != "ComboBoxCellType" && fpSpread1.ActiveSheet.GetCellType(BeforeRow, Col).ToString() != "CheckBoxCellType")
                {
                    if (fpSpread1.Sheets[0].GetSelection(0) == null)
                        BeforeRowCount = 1;
                    else
                        BeforeRowCount = fpSpread1.Sheets[0].GetSelection(0).RowCount;
                }

                int TmpRow = 0;
                if (fpSpread1.ActiveSheet.GetSelection(0) == null)
                    TmpRow = fpSpread1.ActiveSheet.ActiveRowIndex;
                else
                    TmpRow = fpSpread1.ActiveSheet.GetSelection(0).Row;

                for (int i = BeforeRow; i < BeforeRow + BeforeRowCount; i++)
                {
                    if (fpSpread1.Sheets[0].RowHeader.Cells[TmpRow, 0].Text == "I")
                    {
                        fpSpread1.Sheets[0].RowHeader.Cells[TmpRow, 0].Text = "I";
                    }
                    else if (fpSpread1.Sheets[0].RowHeader.Cells[TmpRow, 0].Text == "D")
                    {
                        if (fpSpread1.Sheets[0].Cells[TmpRow, 0].Text == "N")
                        {
                            fpSpread1.Sheets[0].RowHeader.Cells[TmpRow, 0].Text = "I";
                        }
                        else
                        {
                            fpSpread1.Sheets[0].RowHeader.Cells[TmpRow, 0].Text = "";
                            fpSpread1.Sheets[0].RowHeader.Rows[TmpRow].BackColor = SystemBase.Base.Color_Org;
                        }
                    }
                    else
                    {
                        fpSpread1.Sheets[0].RowHeader.Cells[TmpRow, 0].Text = "D";
                        fpSpread1.Sheets[0].RowHeader.Rows[TmpRow].BackColor = SystemBase.Base.Color_Delete;
                        TmpRow++;
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log("RowRemove (그리드 삭제버튼 클릭에러)", f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY020"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {
                    string strQuery = " usp_QTC002  @pTYPE = 'S1'";
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
     
                    UIForm.FPMake.grdCommSheet(fpSpread3, strQuery, G3Head1, G3Head2, G3Head3, G3Width, G3Align, G3Type, G3Color, G3Etc, G3HeadCnt, false, false, 0, 0, true);

                    strActiveSpread = "";

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
            txtInspReqNo.Value = strInspReqNo;
            dtpInspReqDt.Value = fpSpread3.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx3, "검사의뢰일")].Text;
            dtpInspDemandDt.Value = fpSpread3.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx3, "검사요구일")].Text;
            txtItemCd.Value = fpSpread3.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx3, "품목코드")].Text;
            txtItemNm.Value = fpSpread3.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx3, "품명")].Text;
            txtFinInspLvl.Value = fpSpread3.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx3, "검사레벨")].Text;
            txtFinInspLvlNm.Value = fpSpread3.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx3, "검사레벨명")].Text;
            txtLotSize.Value = Convert.ToDouble(fpSpread3.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx3, "LOT크기")].Value);
            txtStockUnit.Value = fpSpread3.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx3, "단위")].Text;
            txtProjectNo.Value = fpSpread3.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx3, "프로젝트번호")].Text;
            txtProjectNm.Value = fpSpread3.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx3, "프로젝트명")].Text;
            txtInspStatus.Value = fpSpread3.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx3, "진행상태")].Text;
            txtDecisionCd.Value = fpSpread3.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx3, "검사판정명")].Text;
            txtInspQty.Value = Convert.ToDouble(fpSpread3.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx3, "검사수")].Value);
            txtDefectQty.Value = Convert.ToDouble(fpSpread3.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx3, "불량수")].Value);
            txtUnityInspReqNo.Value = fpSpread3.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx3, "통합의뢰번호")].Text;
            txtWorkOrderNo.Value = fpSpread3.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx3, "제조오더번호")].Text;
            txtPlantCd.Value = fpSpread3.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx3, "공장코드")].Text;

            try
            {
                string strQuery = " usp_QTC002  @pTYPE = 'S2'";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                strQuery += ", @pPLANT_CD = '" + txtPlantCd.Value + "'";
                strQuery += ", @pWORKORDER_NO = '" + txtWorkOrderNo.Value + "'";

                UIForm.FPMake.grdCommSheet(fpSpread2, strQuery, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, 0, 0, false, false);

                int iRowCnt = Convert.ToInt32(txtInspQty.Value);
                int iInsertRow = iRowCnt - fpSpread2.Sheets[0].Rows.Count;
                
                for (int i = 0; i < iInsertRow; i++)
                {
                    UIForm.FPMake.RowInsert(fpSpread2);
                    int iSelectedRow = fpSpread2.ActiveSheet.ActiveRowIndex;
                    fpSpread2.Sheets[0].Cells[iSelectedRow, 0].Text = "N";
                    fpSpread2.Sheets[0].Cells[iSelectedRow, SystemBase.Base.GridHeadIndex(GHIdx2, "DETAIL CNT")].Value = "0";
                }

                if (fpSpread2.Sheets[0].Rows.Count > 0)
                {

                    strActiveSpread = "fpSpread2";

                    if (fpSpread2.Sheets[0].RowHeader.Cells[0, 0].Text != "I")
                    {
                        Grid1_Search(0);
                        fpSpread2.Focus();
                        fpSpread2.ActiveSheet.SetActiveCell(0, 1); //Row Focus		
                        fpSpread2.ShowRow(0, 0, FarPoint.Win.Spread.VerticalPosition.Center); //Focus Row 보기
                    }
                    else
                    {
                        fpSpread1.Sheets[0].Rows.Count = 0;
                    }
                }
                else
                {
                    strActiveSpread = "";
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
                        SearchRow = e.NewRow;
                        Grid1_Search(SearchRow);
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
        private void Grid1_Search(int iRow)
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {
                string strSerial = fpSpread2.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx2, "SERIAL NO")].Text;

                if (fpSpread2.Sheets[0].RowHeader.Cells[iRow, 0].Text != "I" && strSerial != "")
                {
                    string strQuery = " usp_QTC002  @pTYPE = 'S3'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                    strQuery += ", @pPLANT_CD = '" + txtPlantCd.Value + "'";
                    strQuery += ", @pSERIAL_NO = '" + strSerial + "'";
                    strQuery += ", @pWORKORDER_NO = '" + txtWorkOrderNo.Value + "'";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false, false);

                    if (fpSpread1.Sheets[0].Rows.Count > 0)
                    {
                        fpSpread1.Focus();
                        fpSpread1.ActiveSheet.SetActiveCell(0, SystemBase.Base.GridHeadIndex(GHIdx1, "SERIAL NO")); //Row Focus		
                        fpSpread1.ShowRow(0, SystemBase.Base.GridHeadIndex(GHIdx1, "SERIAL NO"), FarPoint.Win.Spread.VerticalPosition.Center); //Focus Row 보기

                        fpSpread1.Sheets[0].SetColumnMerge(SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드"), FarPoint.Win.Spread.Model.MergePolicy.Always);
                        fpSpread1.Sheets[0].SetColumnMerge(SystemBase.Base.GridHeadIndex(GHIdx1, "품명"), FarPoint.Win.Spread.Model.MergePolicy.Always);
                        fpSpread1.Sheets[0].SetColumnMerge(SystemBase.Base.GridHeadIndex(GHIdx1, "규격"), FarPoint.Win.Spread.Model.MergePolicy.Always);

                        for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                        {
                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "SERIAL NO")].Text == "")
                            {
                                fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text = "I";
                                fpSpread1.Sheets[0].RowHeader.Rows[i].BackColor = SystemBase.Base.Color_Insert;
                                UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "SERIAL NO") + "|1");
                            }
                            else
                            {
                                UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "SERIAL NO") + "|3");
                            }
                        }

                    }
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

        #region SaveExec() 데이타 저장 로직
        protected override void SaveExec()
        {
            fpSpread1.Focus();

            //상단 그룹박스 필수 체크
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox2))
            {
                if (fpSpread2.Sheets[0].Rows.Count > 0)
                {
                    this.Cursor = Cursors.WaitCursor;

                    string ERRCode = "WR", MSGCode = "P0000"; //처리할 내용이 없습니다.
                    SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                    SqlCommand cmd = dbConn.CreateCommand();
                    SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                    try
                    {
                        
                        //행수만큼 처리
                        for (int i = 0; i < fpSpread2.Sheets[0].Rows.Count; i++)
                        {
                            //그리드 상단 필수 체크
                            if (fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "SERIAL NO")].Text != "")
                            {
                                string strHead = fpSpread2.Sheets[0].RowHeader.Cells[i, 0].Text;
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
                                        string strSql = " usp_QTC002 '" + strGbn + "'";
                                        strSql += ", @pWORKORDER_NO = '" + txtWorkOrderNo.Value + "'";
                                        strSql += ", @pINSP_REQ_NO = '" + txtInspReqNo.Text + "' ";
                                        strSql += ", @pSERIAL_NO = '" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "SERIAL NO")].Text + "' ";
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
                        

                        int iRow = fpSpread2.Sheets[0].ActiveRowIndex;
                        if (fpSpread2.Sheets[0].RowHeader.Cells[iRow, 0].Text.ToString() != "D")
                        {

                            //행수만큼 처리
                            for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                            {
                                //그리드 상단 필수 체크
                                if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "SERIAL NO")].Text != "")
                                {
                                    string strHead = fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text;
                                    string strGbn = "";

                                    if (strHead.Length > 0)
                                    {
                                        switch (strHead)
                                        {
                                            case "U": strGbn = "U2"; break;
                                            case "I": strGbn = "I2"; break;
                                            case "D": strGbn = "D2"; break;
                                            default: strGbn = ""; break;
                                        }

                                        if (strGbn != "")
                                        {
                                            string strSql = " usp_QTC002 '" + strGbn + "'";
                                            strSql += ", @pPLANT_CD = '" + txtPlantCd.Text + "' ";
                                            strSql += ", @pINSP_REQ_NO = '" + txtInspReqNo.Text + "' ";
                                            strSql += ", @pSERIAL_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "PRNT SERIAL NO")].Text + "' ";
                                            strSql += ", @pCHILD_ITEM_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text + "' ";
                                            strSql += ", @pCHILD_ITEM_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "순번")].Text + "' ";
                                            strSql += ", @pCHILD_SERIAL_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "SERIAL NO")].Text + "' ";
                                            strSql += ", @pWORKORDER_NO = '" + txtWorkOrderNo.Value + "'";
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

        #region 그리드클릭시 선택 그리드 변수 저장
        private void fpSpread2_CellClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            if (e.Row >= 0)
            {
                strActiveSpread = "fpSpread2";
            }

        }

        private void fpSpread1_CellClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            if (e.Row >= 0)
            {
                strActiveSpread = "fpSpread1";
            }
        }
        #endregion

        #region Ctrl+ C, V 설정
        private void fpSpread2_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            try
            {
                if (e.Control && e.KeyCode == Keys.C)
                {
                    e.Handled = true;
                    Clipboard.Clear();
                    //fpSpread1.ClipboardOptions = FarPoint.Win.Spread.ClipboardOptions.AllHeaders;
                    //fpSpread1.Sheets[0].ClipboardCopy(FarPoint.Win.Spread.ClipboardCopyOptions.AsStringSkipHidden);
                    fpSpread2.Sheets[0].ClipboardCopy(ClipboardCopyOptions.All);
                }

                if (e.Control && e.KeyCode == Keys.V)
                {
                    fpSpread2.Sheets[0].ClipboardPaste(ClipboardPasteOptions.Values);

                    // 복사된 행의 열을 구하기 위하여 클립보드 사용.

                    IDataObject iData = Clipboard.GetDataObject();

                    string strClp = (string)iData.GetData(DataFormats.Text);

                    if (strClp != "" && strClp != null && strClp.Length > 0)
                    {
                        Regex rx1 = new Regex("\r\n");
                        string[] arrData = rx1.Split(strClp.ToString());

                        int DataCount = 0;
                        if (arrData.Length > 1)
                            DataCount = arrData.Length - 1;

                        if (DataCount > 0)
                        {
                            int STRow = fpSpread2.ActiveSheet.ActiveRowIndex;
                            if (STRow < 0)
                                STRow = 0;

                            int ClipRowCount = STRow + DataCount;
                            if (fpSpread2.Sheets[0].RowCount < DataCount)
                                ClipRowCount = fpSpread2.Sheets[0].RowCount - STRow;

                            for (int i = STRow; i < ClipRowCount; i++)
                            {
                                if (i < fpSpread2.Sheets[0].RowCount
                                    || fpSpread2.Sheets[0].Cells[i, fpSpread2.ActiveSheet.ActiveColumnIndex].Locked != true)
                                {
                                    if (fpSpread2.Sheets[0].RowHeader.Cells[i, 0].Text != "I")
                                    { fpSpread2.Sheets[0].RowHeader.Cells[i, 0].Text = "U"; }

                                    UIForm.FPMake.fpChange(fpSpread2, i);
                                }
                            }

                        }
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                //MessageBox.Show(SystemBase.Base.MessageRtn("SY008", "Clipboard 이벤트"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion
    }
}
