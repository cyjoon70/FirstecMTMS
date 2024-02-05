#region DAB002 작성 정보
/*************************************************************/
// 단위업무명 : 원가제출자료 등록
// 작 성 자 :   유재규
// 작 성 일 :   2013-06-13
// 작성내용 :   
// 수 정 일 :   
// 수 정 자 :   
// 수정내용 :   
// 비    고 : 갑지 (팝업 사용 Detail 등록)
// 참    고 : 
/*************************************************************/
#endregion

using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Data.OleDb;
using UIForm;
using System.IO;
using C1.C1Preview;
using C1.C1Preview.DataBinding;
using C1.Win.C1Preview;

namespace DB.DAB002
{
    public partial class DAB002 : UIForm.FPCOMM2
    {
        #region 변수선언
        int iPK_SEQ = 0;
        int iDETAIL_SEQ = 0;
        string strMNUF_CODE = "";
        string strDPRT_CODE = "";
        string strORDR_YEAR = "";
        string strDCSN_NUMB = "";
        string strCALC_DEGR = "";
        string strNIIN = "";
        string strPROJECT_ID = "";
        string strFpspread1 = "N";
        string strFpspread2 = "N";
        int PreRow = -1;       // SelectionChanged 시에 동일 Row에서 데이타변환 처리 안하도록 하기 위함.
        int FpGrid1_Row = 0;
        int FpGrid1_Col = 0;
        int PreSave_Rows = 0;
        #endregion

        #region DAB002
        public DAB002()
        {
            InitializeComponent();
        }
        #endregion

        #region DAB002_Load
        private void DAB002_Load(object sender, EventArgs e)
        {
            SystemBase.Base.gstrFromLoading = "N";
            SystemBase.Validation.GroupBox_Setting(groupBox1);//필수 적용
            //제출업체
            SystemBase.ComboMake.C1Combo(cboH_MNUF_CODE, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'D004', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'", 0);   //제출업체

            txtH_ORDR_YEAR.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("")).ToString().Substring(0, 4);
            txtH_ORDR_YEAR.Value = SystemBase.Base.ServerTime("YYMMDD").Substring(0,4);

            Master_ReSet();
            Detail_ReSet();
             
            SystemBase.Base.gstrFromLoading = "Y";
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            Master_ReSet();
            Detail_ReSet();
            PreRow = -1;
        }
        #endregion

        #region 행복사 버튼 클릭 이벤트
        protected override void RCopyExec()
        {
            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))  //필수조회조건 체크
                {
                    if (strFpspread1 == "N" && strFpspread2 == "N")
                    {
                        MessageBox.Show("행 추가할 그리드를 선택 하시기 바랍니다.", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning); //데이터 조회 중 오류가 발생하였습니다.
                        return;
                    }

                    if (strFpspread1 == "Y")
                    {
                        if (fpSpread2.Sheets[0].RowHeader.Cells[fpSpread2.Sheets[0].ActiveRowIndex, 0].Text == "D")
                        {
                            return;
                        }

                        UIForm.FPMake.RowCopy(fpSpread1);
                    }
                    else if (strFpspread2 == "Y")
                    {

                        UIForm.FPMake.RowCopy(fpSpread2);

                    }
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 행추가 버튼 클릭 이벤트
        protected override void RowInsExec()
        {
            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))  //필수조회조건 체크
                {
                    if (strFpspread1 == "N" && strFpspread2 == "N")
                    {
                        MessageBox.Show("행 추가할 그리드를 선택 하시기 바랍니다.", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if (strFpspread1 == "Y")
                    {
                        if (fpSpread2.Sheets[0].Rows.Count == 0)
                        {
                            return;
                        }
                        if (fpSpread2.Sheets[0].RowHeader.Cells[fpSpread2.Sheets[0].ActiveRowIndex, 0].Text == "D")
                        {
                            return;
                        }

                        if (fpSpread2.Sheets[0].RowHeader.Cells[fpSpread2.Sheets[0].ActiveRowIndex, 0].Text == "I")
                        {
                            MessageBox.Show("마스터 저장 후 행 추가하시기 바랍니다.", SystemBase.Base.MessageRtn("Z0004"), MessageBoxButtons.OK, MessageBoxIcon.Warning); //마스터 저장 후 행 추가하시기 바랍니다.
                            return;
                        }

                        UIForm.FPMake.RowInsert(fpSpread1);

                        fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "마스터순번")].Text
                                    = fpSpread2.Sheets[0].Cells[fpSpread2.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx2, "순번")].Text;
                        fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "제출업체")].Text
                                    = fpSpread2.Sheets[0].Cells[fpSpread2.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx2, "제출업체")].Text;
                        fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "요구년도")].Text
                                    = fpSpread2.Sheets[0].Cells[fpSpread2.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx2, "요구년도")].Text;
                        fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "부서")].Value
                                    = fpSpread2.Sheets[0].Cells[fpSpread2.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx2, "부서")].Value.ToString();
                        fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "판단번호")].Text
                                    = fpSpread2.Sheets[0].Cells[fpSpread2.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx2, "판단번호")].Text;
                        fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Text
                                    = fpSpread2.Sheets[0].Cells[fpSpread2.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx2, "차수")].Text;

                    }
                    else if (strFpspread2 == "Y")
                    {
                        UIForm.FPMake.RowInsert(fpSpread2);
                        fpSpread2.Sheets[0].Cells[fpSpread2.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx2, "제출업체")].Text = cboH_MNUF_CODE.SelectedValue.ToString();
                        fpSpread2.Sheets[0].Cells[fpSpread2.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx2, "요구년도")].Text = txtH_ORDR_YEAR.Text;

                        Detail_ReSet();
                    }
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 행삭제 버튼 클릭 이벤트
        protected override void DelExec()
        {
            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))  //필수조회조건 체크
                {
                    if (strFpspread1 == "N" && strFpspread2 == "N")
                    {
                        MessageBox.Show(SystemBase.Base.MessageRtn("B0007", "그리드행 삭제"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if (strFpspread1 == "Y")
                    {
                        if (fpSpread2.Sheets[0].RowHeader.Cells[fpSpread2.Sheets[0].ActiveRowIndex, 0].Text == "D")
                        {
                            return;
                        }

                        UIForm.FPMake.RowRemove(fpSpread1);
                    }
                    else if (strFpspread2 == "Y")
                    {
                        UIForm.FPMake.RowRemove(fpSpread2);

                        //신규추가된 행은 취소처리
                        for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                        {
                            if (fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text == "I")
                            {
                                fpSpread1.ActiveSheet.ActiveRowIndex = i;
                                UIForm.FPMake.RowRemove(fpSpread1);
                                i = 0;
                            }
                        }

                        //접수현황이 "D"이면 계약품목 작업불가
                        fpSpread1.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.ReadOnly;
                    }
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 행취소 버튼 클릭 이벤트
        protected override void CancelExec()
        {
            try
            {
                int GridSelectRow = 0;
                int GridSelectRowCount = 0;

                if (strFpspread1 == "Y")
                {
                    if (fpSpread2.Sheets[0].RowHeader.Cells[fpSpread2.Sheets[0].ActiveRowIndex, 0].Text == "D")
                    {
                        return;
                    }

                    if (fpSpread1.ActiveSheet.ActiveRowIndex < 0) return;

                    if (fpSpread1.ActiveSheet.ActiveRowIndex.ToString() != "")
                    {
                        GridSelectRow = fpSpread1.ActiveSheet.ActiveRowIndex;

                        int Row = fpSpread1.ActiveSheet.ActiveRowIndex;
                        int Col = fpSpread1.ActiveSheet.ActiveColumnIndex;

                        GridSelectRowCount = 1;
                        //if (fpSpread1.ActiveSheet.GetCellType(Row, Col).ToString() != "ComboBoxCellType" && fpSpread1.ActiveSheet.GetCellType(Row, Col).ToString() != "CheckBoxCellType")
                        //{
                        if (fpSpread1.Sheets[0].GetSelection(0) == null)
                            GridSelectRowCount = 1;
                        else
                            GridSelectRowCount = fpSpread1.Sheets[0].GetSelection(0).RowCount;
                        //}
                    }
                    UIForm.FPMake.Cancel(fpSpread1, GridSelectRow, GridSelectRowCount);
                }
                else if (strFpspread2 == "Y")
                {
                    if (fpSpread2.Sheets[0].RowHeader.Cells[fpSpread2.Sheets[0].ActiveRowIndex, 0].Text == "D")
                    {
                        fpSpread1.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.Normal;
                    }

                    if (fpSpread2.ActiveSheet.ActiveRowIndex < 0) return;

                    if (fpSpread2.ActiveSheet.ActiveRowIndex.ToString() != "")
                    {
                        GridSelectRow = fpSpread2.ActiveSheet.ActiveRowIndex;

                        int Row = fpSpread2.ActiveSheet.ActiveRowIndex;
                        int Col = fpSpread2.ActiveSheet.ActiveColumnIndex;

                        GridSelectRowCount = 1;
                        //if (fpSpread2.ActiveSheet.GetCellType(Row, Col).ToString() != "ComboBoxCellType" && fpSpread2.ActiveSheet.GetCellType(Row, Col).ToString() != "CheckBoxCellType")
                        //{
                        if (fpSpread2.Sheets[0].GetSelection(0) == null)
                            GridSelectRowCount = 1;
                        else
                            GridSelectRowCount = fpSpread2.Sheets[0].GetSelection(0).RowCount;
                        //}
                    }
                    UIForm.FPMake.Cancel(fpSpread2, GridSelectRow, GridSelectRowCount);

                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))  //필수조회조건 체크
                {
                    this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
                    SystemBase.Validation.GroupBox_Setting(groupBox1);

                    string strSql = " usp_DAB002  ";
                    strSql += "  @pTYPE = 'S1'";
                    strSql += ", @pMNUF_CODE = '" + cboH_MNUF_CODE.SelectedValue + "' ";
                    strSql += ", @pORDR_YEAR = '" + txtH_ORDR_YEAR.Text + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread2, strSql, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, true, 0, 0);
                    fpSpread2.Sheets[0].Columns[SystemBase.Base.GridHeadIndex(GHIdx2, "이윤율")].Locked = false;  // 이윤율버튼 활성

                    Detail_ReSet();

                    this.Cursor = System.Windows.Forms.Cursors.Default;
                }

                PreRow = -1;
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region SaveExec() 폼에 입력된 데이타 저장 로직
        protected override void SaveExec()
        {

            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))  //필수여부체크
            {
                if (SaveCheck() == false) return;
                PreSave_Rows = fpSpread2.ActiveSheet.RowCount;  //저장전 RowCount 가짐...

                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("SY048"), SystemBase.Base.MessageRtn("Z0004"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dsMsg == DialogResult.Yes)
                {
                    if (Master_Save() == false) return;  // 마스터 저장실패시 리턴처리

                    string strHead = ""; string strGbn = "";
                    string ERRCode = "OK", MSGCode = "SY067";   // 에러코드는  OK처리 마스터만 저장할경우도 같이 처리
                    SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                    SqlCommand cmd = dbConn.CreateCommand();
                    SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);
                    this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
                    try
                    {
                        for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                        {
                            strHead = fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text;
                            strGbn = "";
                            if (strHead.Length > 0)
                            {
                                switch (strHead)
                                {
                                    case "D": strGbn = "D2"; break;
                                    case "U": strGbn = "I2"; break;  // UPDATE 및 INSERT 동일 처리
                                    case "I": strGbn = "I2"; break;  // UPDATE 및 INSERT 동일 처리
                                    default: strGbn = ""; break;
                                }

                                string strSql = " usp_DAB002 ";
                                strSql += "  @pTYPE = '" + strGbn + "'";
                                strSql += ", @pPK_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "순번")].Text.ToString() + "' ";
                                strSql += ", @pMASTER_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "마스터순번")].Text.ToString() + "' ";
                                
                                strSql += ", @pMNUF_CODE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "제출업체")].Text.ToString() + "'";
                                strSql += ", @pORDR_YEAR = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "요구년도")].Text.ToString() + "'";
                                strSql += ", @pDPRT_CODE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "부서")].Text.ToString() + "'";
                                strSql += ", @pDCSN_NUMB = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "판단번호")].Text.ToString() + "'";
                                strSql += ", @pCALC_DEGR = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Text.ToString() + "'";
                                strSql += ", @pNIIN = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "재고번호")].Text.ToString() + "'";
                                strSql += ", @pUPPER_NIIN = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "상위재고번호")].Text.ToString() + "'";
                                strSql += ", @pMANAGER_PART_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "부품관리번호")].Text.ToString() + "'";
                                strSql += ", @pFLOOR_PLAN_NUMB = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "도면번호")].Text.ToString() + "'";
                                strSql += ", @pFLOOR_ITEM_NUMB = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "도면부품번호")].Text.ToString() + "'";
                                strSql += ", @pUNIT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].Value.ToString() + "'";
                                strSql += ", @pDMST_ITNB = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "항목")].Text.ToString() + "'";
                                strSql += ", @pPRESENT_USE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "제출용도")].Value.ToString() + "'";
                                strSql += ", @pSUPPLY_DEMAND_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "조달요구번호")].Text.ToString() + "'";
                                strSql += ", @pCONTRACT_ERP_PART_ID = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "ERP품목")].Text.ToString() + "'";
                                strSql += ", @pCONTRACT_ERP_PART_NAME = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품명")].Text.ToString().Replace("'", " ") + "'";
                                strSql += ", @pPROJECT_ID = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트")].Text.ToString() + "'";
                                strSql += ", @pPROJECT_NAME = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트명")].Text.ToString() + "'";
                                strSql += ", @pAPST_NBMT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "시작호기")].Text.ToString() + "'";
                                strSql += ", @pAPFN_NBMT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "종료호기")].Text.ToString() + "'";
                                strSql += ", @pCALC_PLAN = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "원가계산방법")].Value.ToString() + "'";
                                strSql += ", @pPRJCLS_DVS = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "양산구분")].Value.ToString() + "'";
                                strSql += ", @pCOST_APPLY_QNTY = " + SystemBase.Validation.Decimal_Data(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "원가계산적용수량")].Text.ToString(), ",") + "";
                                strSql += ", @pIN_ID ='" + SystemBase.Base.gstrUserID + "' ";                                  //사용자

                                System.Data.DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                                ERRCode = ds.Tables[0].Rows[0][0].ToString();
                                MSGCode = ds.Tables[0].Rows[0][1].ToString();

                                if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }
                            }
                        }

                        Trans.Commit();
                    }
                    catch (Exception f)
                    {
                        SystemBase.Loggers.Log(this.Name, f.ToString());
                        Trans.Rollback();
                        MSGCode = "SY002";	//에러가 발생하여 데이터 처리가 취소되었습니다.
                    }

                Exit:
                    dbConn.Close();
                    this.Cursor = System.Windows.Forms.Cursors.Default;

                    if (ERRCode == "OK")
                    {
                        SearchExec();

                        Detail_List(FpGrid1_Row);

                        MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else if (ERRCode == "ER")
                    {
                        MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

            }
        }
        #endregion

        #region DelExec() 삭제 로직(사용안함)
        protected override void DeleteExec()
        {
            //접수현황이 "D"이면 계약품목 작업불가
            //if (fpSpread1.Sheets[0].RowHeader.Cells[FpGrid1_Row, 0].Text == "D")
            //{
            //    fpSpread1.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.ReadOnly;
           // } 
            
            //if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))  //필수여부체크
            //{
            //    해당 전체자료 삭제
            //    DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("SY030"), SystemBase.Base.MessageRtn("Z0004"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            //    if (dsMsg == DialogResult.Yes)
            //    {
            //        string ERRCode = "", MSGCode = "";
            //        SqlConnection dbConn = SystemBase.DbOpen.DBCON();
            //        SqlCommand cmd = dbConn.CreateCommand();
            //        SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);
            //        this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            //        try
            //        {
            //            string strSql = " usp_DAB002 ";
            //            strSql += "  @pTYPE = 'D2'";
            //            strSql += ", @pMASTER_SEQ = '" 
            //            strSql += ", @pMNUF_CODE = '" + cboH_MNUF_CODE.SelectedValue.ToString() + "'";
            //            strSql += ", @pORDR_YEAR = '" + txtH_ORDR_YEAR.Text + "'";

            //            DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
            //            ERRCode = ds.Tables[0].Rows[0][0].ToString();
            //            MSGCode = ds.Tables[0].Rows[0][1].ToString();

            //            if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

            //            Trans.Commit();

            //        }
            //        catch (Exception f)
            //        {
            //            SystemBase.Loggers.Log(this.Name, f.ToString());
            //            Trans.Rollback();
            //            MSGCode = "SY002";	//에러가 발생하여 데이터 처리가 취소되었습니다.
            //        }
            //    Exit:
            //        dbConn.Close();
            //        this.Cursor = System.Windows.Forms.Cursors.Default;

            //        if (ERRCode == "OK")
            //        {
            //            SearchExec();
            //            MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
            //        }
            //        else if (ERRCode == "ER")
            //        {
            //            MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            //        }
            //    }
            //}
        }
        #endregion

        #region PrintExe() 출력 로직
        protected override void PrintExe()
        {
            int iRow = fpSpread2.Sheets[0].ActiveRowIndex;
  
            if (iRow < 0)
            {
                //선택된 내용이 없습니다.
                MessageBox.Show(SystemBase.Base.MessageRtn("SY028"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            this.Cursor = Cursors.WaitCursor;

            iPK_SEQ = Convert.ToInt32(fpSpread2.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx2, "순번")].Text.ToString());            
            
            C1PrintDocument doc = new C1PrintDocument();

            C1.C1Preview.RenderTable rt = new C1.C1Preview.RenderTable();
            rt.Style.Font = new Font("맑은고딕", 8);//기본8로 설정
            rt.Style.TextAlignHorz = AlignHorzEnum.Left;
            rt.Style.TextAlignVert = AlignVertEnum.Center;
            rt.Style.GridLines.All = new LineDef("0.1mm", Color.Black);
            rt.CellStyle.GridLines.Bottom = LineDef.Empty;
 

            //속성 설정 순서
            //1. text 출력
            //2. SpanRow, SpanCol
            //3. font 설정
            //4. 색상 설정
            //5. 길이 설정
            //6. 정렬 설정
            //7. 그리드라인 설정

            //1.페이지 설정
            SetPage(doc);

            //2.페이지 헤더 설정 
            SetPageHeader(doc, iPK_SEQ);

            //3.칼럼 헤더 설정
            SetColumnHeader(rt);

            //4. 데이타 연결  
            SetDataBinding(doc, rt, iPK_SEQ);

            //To Do:0이면 출력안하기

            //5. 미리보기
            C1PrintPreviewDialog d = new C1PrintPreviewDialog();

            d.Document = doc;
            d.PreviewPane.ZoomFactor = 1;
            d.WindowState = FormWindowState.Maximized;

            d.ShowDialog();

            this.Cursor = Cursors.Default;
        }
        #endregion


        #region Master 그리드 방향키 이동 및 클릭시 Detail 조회
        private void fpSpread2_SelectionChanged(object sender, FarPoint.Win.Spread.SelectionChangedEventArgs e)
        {
            try
            {
                if (fpSpread2.Sheets[0].Rows.Count > 0)
                {

                    FpGrid1_Row = fpSpread2.ActiveSheet.ActiveRowIndex;  // 마스터 선택 Row
                    FpGrid1_Col = fpSpread2.ActiveSheet.ActiveColumnIndex;  // 마스터 선택 Column

                    if (FpGrid1_Row < 0) return;
                    if (PreRow == FpGrid1_Row && PreRow != -1) return;  //현 Row에서 컬럼이동시는 조회 안되게

                    Detail_List(FpGrid1_Row);

                    PreRow = fpSpread2.ActiveSheet.ActiveRowIndex;

                    this.Cursor = Cursors.Default;

                }
            }
            catch (Exception f)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region fpButtonClick
        protected override void fpButtonClick(int Row, int Column)
        {
            try
            {
                //#region 재고번호 팝업
                //if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "재고번호_2"))
                //{
                //    strNIIN = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "재고번호")].Text.ToString();

                //    WNDW.WNDW029 pu = new WNDW.WNDW029("", "", strNIIN, "", "SINGLE");  // 프로잭트, 품번, 국가재고, 부품관리번호, 구분(SINGLE, MANY)
                //    pu.ShowDialog();

                //    if (pu.RETURN > 0)
                //    {
                //        if (pu.NATION_STOCK_NO[0].Length > 0)
                //            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "재고번호")].Text = pu.NATION_STOCK_NO[0].Replace("-", "");
                //        else
                //            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "재고번호")].Text = "";

                //        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "부품관리번호")].Value = pu.MANAGER_PART_NO[0];
                //        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "도면번호")].Value = pu.FLOOR_PLAN_NUMB[0];
                //        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "도면부품번호")].Value = pu.FLOOR_PLAN_NUMB[0];

                //        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].Value = pu.STOCK_UM[0];
                //        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "ERP품목")].Text = pu.PART_ID[0];
                //        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품명")].Text = pu.PART_NAME[0];
                //        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트")].Text = pu.PROJECT_ID[0];
                //        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트명")].Text = pu.PROJECT_NAME[0];
                //    }
                //}
                //#endregion

                #region 프로젝트 팝업
                if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트_2"))
                {
                    strPROJECT_ID = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트")].Text.ToString();

                    //WNDW.WNDW029 pu = new WNDW.WNDW029(strPROJECT_ID, "", "", "", "SINGLE");  // 프로잭트, 품번, 국가재고, 부품관리번호, 구분(SINGLE, MANY)
                    //pu.ShowDialog();

                    //if (pu.RETURN > 0)
                    //{
                    //    //fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "재고번호")].Text = pu.NATION_STOCK_NO[0];
                    //    //fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].Value = pu.STOCK_UM[0];
                    //    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "ERP품목")].Text = pu.PART_ID[0];
                    //    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품명")].Text = pu.PART_NAME[0];
                    //    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트")].Text = pu.PROJECT_ID[0];
                    //    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트명")].Text = pu.PROJECT_NAME[0];

                    //    if (fpSpread1.Sheets[0].RowHeader.Cells[Row, 0].Text != "I")
                    //    {
                    //        fpSpread1.Sheets[0].RowHeader.Cells[Row, 0].Text = "U";
                    //        fpSpread1.Sheets[0].RowHeader.Rows[Row].BackColor = SystemBase.Base.Color_Update;
                    //    }
                    //}
                }
                #endregion

                #region 자료등록
                if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "구성품"))
                {
                    if (fpSpread1.Sheets[0].RowHeader.Cells[Row, 0].Text == "I")
                    {
                        MessageBox.Show("추가행 저장 후 자료등록 하시기 바랍니다.", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning); //추가행 저장 후 자료등록 하시기바랍니다. 
                        return;   
                    }

                    if (fpSpread1.Sheets[0].RowHeader.Cells[Row, 0].Text == "U")
                    {
                        MessageBox.Show("변경행 저장 후 자료등록 하시기 바랍니다.", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning); //변경행 저장 후 자료등록 하시기 바랍니다.
                        return;  
                    } 

                    if (fpSpread1.Sheets[0].RowHeader.Cells[Row, 0].Text == "D")
                    {
                        MessageBox.Show("삭제행 저장 후 자료등록 하시기 바랍니다.", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning); //삭제행 저장 후 자료등록 하시기 바랍니다.
                        return;
                    }

                    iDETAIL_SEQ = Convert.ToInt32(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "순번")].Text.ToString());
                    strMNUF_CODE = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "제출업체")].Text.ToString();
                    strORDR_YEAR = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "요구년도")].Text.ToString();
                    strDPRT_CODE = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "부서")].Text.ToString();
                    strDCSN_NUMB = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "판단번호")].Text.ToString();
                    strCALC_DEGR = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Text.ToString();
                    strNIIN = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "재고번호")].Text.ToString();

                    DAB002P1 pu = new DAB002P1(iDETAIL_SEQ, strMNUF_CODE, strORDR_YEAR, strDPRT_CODE, strDCSN_NUMB, strCALC_DEGR, strNIIN, this.Name);
                    pu.ShowDialog();

                }
                #endregion
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY008", "그리드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }

        }
        #endregion

        #region Detail_List
        private void Detail_List(int intRow)
        {
            //접수현황 상태가 "D"이면 계약품목 그리드 Lock처리

            try
            {

                if (PreSave_Rows > fpSpread2.ActiveSheet.RowCount)
                {
                    PreSave_Rows = fpSpread2.ActiveSheet.RowCount;  //저장후 RowCount 가짐...
                    return;  // 삭제가 있을시는 Row 수가 달라짐...하여 저장전과 비교처리
                }

                iPK_SEQ = fpSpread2.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx2, "순번")].Value == null ? 0 : Convert.ToInt32(fpSpread2.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx2, "순번")].Text.ToString()); ;
                strMNUF_CODE = fpSpread2.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx2, "제출업체")].Text.ToString();
                strORDR_YEAR = fpSpread2.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx2, "요구년도")].Text.ToString();
                strDPRT_CODE = fpSpread2.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx2, "부서")].Value == null ? "" : fpSpread2.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx2, "부서")].Value.ToString(); // fpSpread2.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx2, "부서")].Value.ToString();     
                strDCSN_NUMB = fpSpread2.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx2, "판단번호")].Text.ToString();
                strCALC_DEGR = fpSpread2.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx2, "차수")].Text.ToString();

                string strSql = " usp_DAB002  ";
                strSql += "  @pTYPE = 'S2'";
                strSql += ", @pPK_SEQ = '" + iPK_SEQ + "' ";
                strSql += ", @pMNUF_CODE = '" + strMNUF_CODE + "' ";
                strSql += ", @pORDR_YEAR = '" + strORDR_YEAR + "' ";
                strSql += ", @pDPRT_CODE = '" + strDPRT_CODE + "' ";
                strSql += ", @pDCSN_NUMB = '" + strDCSN_NUMB + "' ";
                strSql += ", @pCALC_DEGR = '" + strCALC_DEGR + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread1, strSql, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);

                fpSpread2.Focus();  // 마스터 선택 Row 포커스 처리
                fpSpread2.ActiveSheet.SetActiveCell(FpGrid1_Row, FpGrid1_Col);

                //접수현황이 "D"이면 계약품목 작업불가
                if (fpSpread2.Sheets[0].RowHeader.Cells[FpGrid1_Row, 0].Text == "D")
                {
                    fpSpread1.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.ReadOnly;
                }

                // 구성품버튼 활성
                fpSpread1.Sheets[0].Columns[SystemBase.Base.GridHeadIndex(GHIdx1, "구성품")].Locked = false;
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region Master_ReSet, Detail_ReSet
        private void Master_ReSet()
        {
            try
            {
                G2Etc[SystemBase.Base.GridHeadIndex(GHIdx2, "부서")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'D007', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 1);
                G2Etc[SystemBase.Base.GridHeadIndex(GHIdx2, "제출용도")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'D008', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 1);
                G2Etc[SystemBase.Base.GridHeadIndex(GHIdx2, "계약업체")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'D006', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 1);
                G2Etc[SystemBase.Base.GridHeadIndex(GHIdx2, "원가계산방법")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'D009', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 1);
                G2Etc[SystemBase.Base.GridHeadIndex(GHIdx2, "반제품구분")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'D010', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 0);

                UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, true, 0, 0);
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void Detail_ReSet()
        {
            try
            {
                G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "단위")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'Z005', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 1);
                G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "제출용도")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'D008', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 1);
                G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "원가계산방법")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'D009', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 1);
                G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "양산구분")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'D011', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 1);

                UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region Master_Save
        private bool Master_Save()
        {
            bool Master_Save = true;

            string ERRCode = "", MSGCode = "";
            string strHead = ""; string strGbn = "";
            SqlConnection dbConn = SystemBase.DbOpen.DBCON();
            SqlCommand cmd = dbConn.CreateCommand();
            SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

            try
            {
                for (int i = 0; i < fpSpread2.Sheets[0].Rows.Count; i++)
                {
                    strHead = fpSpread2.Sheets[0].RowHeader.Cells[i, 0].Text;
                    strGbn = "";
                    if (strHead.Length > 0)
                    {
                        switch (strHead)
                        {
                            case "D": strGbn = "D1"; break;
                            case "U": strGbn = "I1"; break;  // UPDATE 및 INSERT 동일 처리
                            case "I": strGbn = "I1"; break;  // UPDATE 및 INSERT 동일 처리
                            default: strGbn = ""; break;
                        }

                        string strITEM_FLAG = "";
                        if (fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "반제품구분")].Value != null)
                        {
                            strITEM_FLAG = fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "반제품구분")].Value.ToString();
                        }

                        string strSql = " usp_DAB002 ";
                        strSql += "  @pTYPE = '" + strGbn + "'";
                        strSql += ", @pPK_SEQ = '" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "순번")].Text.ToString() + "'";
                        strSql += ", @pMNUF_CODE = '" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "제출업체")].Text.ToString() + "'";
                        strSql += ", @pORDR_YEAR = '" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "요구년도")].Text.ToString() + "'";
                        strSql += ", @pDPRT_CODE = '" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "부서")].Value.ToString() + "'";
                        strSql += ", @pDCSN_NUMB = '" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "판단번호")].Text.ToString() + "'";
                        strSql += ", @pCALC_DEGR = '" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "차수")].Text.ToString() + "'";
                        strSql += ", @pPRESENT_USE = '" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "제출용도")].Value.ToString() + "'";
                        strSql += ", @pBUSINESS_EXECUTION_NO = '" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "사집번호")].Text.ToString() + "'";
                        strSql += ", @pCONTRACT_NO = '" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "계약번호")].Text.ToString() + "'";
                        strSql += ", @pCONTRACT_AMT            = " + SystemBase.Validation.Decimal_Data(fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "계약금액")].Text, ",");
                        strSql += ", @pRPST_ITNM = '" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "대표품명")].Text.ToString() + "'";
                        strSql += ", @pSTD_YRMON = '" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "제출년월")].Value.ToString().Replace("-", "") + "'";
                        strSql += ", @pCTMF_CODE = '" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "계약업체")].Value.ToString() + "'";
                        strSql += ", @pCALC_PLAN = '" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "원가계산방법")].Value.ToString() + "'";
                        strSql += ", @pITEM_FLAG = '" + strITEM_FLAG + "'";     //반제품구분
                        strSql += ", @pIN_ID ='" + SystemBase.Base.gstrUserID + "' ";                                  //사용자

                        System.Data.DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                        ERRCode = ds.Tables[0].Rows[0][0].ToString();
                        MSGCode = ds.Tables[0].Rows[0][1].ToString();

                        if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }
                    }
                }

                Trans.Commit();
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                Trans.Rollback();
                MSGCode = "SY002";	//에러가 발생하여 데이터 처리가 취소되었습니다.
            }
        Exit:
            dbConn.Close();

            if (ERRCode == "ER")
            {
                MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                Master_Save = false;
            }
            else if (ERRCode == "WR")
            {
                MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                Master_Save = false;
            }

            return Master_Save;
        }
        #endregion

        #region SaveCheck 저장전 자료 여부 체크
        private bool SaveCheck()
        {
            bool chk = true;
            try
            {
                int SaveRow = 0;
                bool Status = false;

                string strORDR_YEAR = "";
                string strDPRT_CODE = "";
                string strDCSN_NUMB = "";
                string strCALC_DEGR = "";
                string strPRESENT_USE = "";
                string strSTD_YRMON = "";

                for (int i = 0; i < fpSpread2.Sheets[0].Rows.Count; i++)
                {
                    strORDR_YEAR = fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "요구년도")].Text.ToString();
                    strDPRT_CODE = fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "부서")].Value.ToString();
                    strDCSN_NUMB = fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "판단번호")].Text.ToString();
                    strCALC_DEGR = fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "차수")].Text.ToString();
                    strPRESENT_USE = fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "제출용도")].Value.ToString();
                    strSTD_YRMON = fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "제출년월")].Value.ToString().Replace("-", "");

                    for (int j = i + 1; j < fpSpread2.Sheets[0].Rows.Count; j++)
                    {
                        if (strORDR_YEAR == fpSpread2.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx2, "요구년도")].Text.ToString() &&
                              strDPRT_CODE == fpSpread2.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx2, "부서")].Value.ToString() &&
                              strDCSN_NUMB == fpSpread2.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx2, "판단번호")].Text.ToString() &&
                              strCALC_DEGR == fpSpread2.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx2, "차수")].Text.ToString() &&
                              strPRESENT_USE == fpSpread2.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx2, "제출용도")].Value.ToString() &&
                              strSTD_YRMON == fpSpread2.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx2, "제출년월")].Value.ToString().Replace("-", ""))
                        {

                            MessageBox.Show(Convert.ToString(j + 1) + "번째 Row의 데이타 중복입니다.", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);

                            fpSpread2.Focus();
                            return false;
                        }
                    }

                }

                Status = FPGrid_SaveCheck2(fpSpread2, this.Name, "fpSpread2", false);
                if (Status == false)   // 에러상태 : 바로 리턴
                {
                    chk = false;
                    return chk;
                }
                else if (Status == true)   // 수정상태 : 다음 그리드도 체크해야 하므로 저장값 가지고 있음..
                {
                    SaveRow++;
                }

                Status = FPGrid_SaveCheck2(fpSpread1, this.Name, "fpSpread1", false);
                if (Status == false) // 에러상태 : 바로 리턴
                {
                    chk = false;
                    return chk;
                }
                else if (Status == true) // 수정상태 : 다음 그리드도 체크해야 하므로 저장값 가지고 있음..
                {
                    SaveRow++;
                }


                if (SaveRow == 0)  // 그리드 변화가 없으면  메시지처리 (변경되거나 처리 할 자료가 없습니다.)
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn("SY017"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    chk = false;
                }

                return chk;
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
        #endregion

        #region fpSpread1_CellDoubleClick
        private void fpSpread1_CellDoubleClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            try
            {
                if (fpSpread1.Sheets[0].RowHeader.Cells[e.Row, 0].Text == "I") return;  // 입력모드시는 팝업처리 안함..

                if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "제출년월") || e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "재고번호"))
                {
                    strMNUF_CODE = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "제출업체코드")].Text.ToString();
                    strORDR_YEAR = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "요구년도")].Text.ToString();
                    strDPRT_CODE = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "부서")].Text.ToString();
                    strDCSN_NUMB = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "판단번호")].Text.ToString();
                    strCALC_DEGR = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Text.ToString();
                    strNIIN = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "재고번호")].Text.ToString();

                    //DAB002P1 pu = new DAB002P1(strMNUF_CODE, strORDR_YEAR, strDCSN_NUMB, strPART_NAME, strSTD_YRMON, strNIIN, this.Name);
                    //pu.Show(Owner);
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY008", "그리드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }
        #endregion

        #region fpSpread_MouseDown
        private void fpSpread2_MouseDown(object sender, MouseEventArgs e)
        {
            strFpspread1 = "N";
            strFpspread2 = "Y";
        }

        private void fpSpread1_MouseDown(object sender, MouseEventArgs e)
        {
            strFpspread1 = "Y";
            strFpspread2 = "N";
        }
        #endregion

        #region fpSpread2 부서콤보 클릭
        private void fpSpread2_CellClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            try
            {
                if ((e.Column == SystemBase.Base.GridHeadIndex(GHIdx2, "부서")) ||
                    (e.Column == SystemBase.Base.GridHeadIndex(GHIdx2, "제출용도")) ||
                    (e.Column == SystemBase.Base.GridHeadIndex(GHIdx2, "계약업체")) ||
                    (e.Column == SystemBase.Base.GridHeadIndex(GHIdx2, "원가게산방법")) ||
                    (e.Column == SystemBase.Base.GridHeadIndex(GHIdx2, "반제품구분")))
                {
                    if (fpSpread2.Sheets[0].ActiveRow == null) return;
                    FpGrid1_Row = e.Row;  // 마스터 선택 Row               

                    if (FpGrid1_Row < 0) return;
                    if (PreRow == FpGrid1_Row && PreRow != -1) return;  //현 Row에서 컬럼이동시는 조회 안되게

                    Detail_List(e.Row);
                    PreRow = e.Row;

                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region **************************************  업체, 공장, 결산기간(From~To)은 변경시 초기화 및 변경여부체크 ******************************
        private void cboH_MNUF_CODE_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (SystemBase.Base.gstrFromLoading == "Y")
                {
                    SystemBase.Base.gstrMNUF_CODE = (cboH_MNUF_CODE.SelectedValue == null ? "" : cboH_MNUF_CODE.SelectedValue.ToString());
                    NewExec();
                    SearchExec();
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void cboH_MNUF_CODE_BeforeOpen(object sender, CancelEventArgs e)
        {
            try
            {
                Value_Selected(e, null, null);
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Value_Selected(CancelEventArgs e, KeyPressEventArgs f, C1.Win.C1Input.UpDownButtonClickEventArgs g)
        {
            try
            {
                //그리드 변경여부체크를 쓰기위하여 TabFPMake에 만든함수를 사용함.
                if (UIForm.TabFPMake.FPGrid_Closing(fpSpread1) > 0)
                {
                    if (FpGrid_DialogResult(fpSpread1, e, f, g) == false) return;
                }

                NewExec();
            }
            catch (Exception o)
            {
                MessageBox.Show(o.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private bool FpGrid_DialogResult(FarPoint.Win.Spread.FpSpread FPGrid, CancelEventArgs e, KeyPressEventArgs f, C1.Win.C1Input.UpDownButtonClickEventArgs g)
        {
            try
            {
                if (FPGrid.ActiveSheet.RowCount <= 0) return false;

                DialogResult Rtn = MessageBox.Show(SystemBase.Base.MessageRtn("SY066"), SystemBase.Base.MessageRtn("Z0004"), MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (Rtn != DialogResult.OK)
                {
                    if (e != null)
                    {
                        e.Cancel = true;
                    }
                    if (f != null)
                    {
                        f.Handled = true;
                    }
                    if (g != null)
                    {
                        g.Done = true;
                    }

                    return false;
                }
                else
                {
                    NewExec();

                    return true;
                }
            }
            catch (Exception o)
            {
                MessageBox.Show(o.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
        #endregion
        
        #region fpSpread2_ButtonClicked
        private void fpSpread2_ButtonClicked(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            try
            {
                #region 자료등록
                if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx2, "이윤율"))
                {
                    if (fpSpread2.Sheets[0].RowHeader.Cells[e.Row, 0].Text == "I")
                    {
                        MessageBox.Show("추가행 저장 후 자료등록 하시기바랍니다. ", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning); //추가행 저장 후 자료등록 하시기바랍니다. 
                        return;   
                    }

                    if (fpSpread2.Sheets[0].RowHeader.Cells[e.Row, 0].Text == "U")
                    {
                        MessageBox.Show("변경행 저장 후 자료등록 하시기 바랍니다.", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning); //변경행 저장 후 자료등록 하시기 바랍니다.
                        return;
                    }

                    if (fpSpread2.Sheets[0].RowHeader.Cells[e.Row, 0].Text == "D")
                    {
                        MessageBox.Show("삭제행 저장 후 자료등록 하시기 바랍니다.", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning); //삭제행 저장 후 자료등록 하시기 바랍니다.
                        return;
                    }

                    iPK_SEQ = Convert.ToInt32(fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "순번")].Text.ToString());
                    strORDR_YEAR = fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "요구년도")].Text.ToString();

                    DAB002P2 pu = new DAB002P2(cboH_MNUF_CODE.SelectedValue.ToString(), iPK_SEQ, strORDR_YEAR, this.Name);
                    pu.ShowDialog();

                }
                #endregion
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY008", "그리드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }
        #endregion

        #region 크리스탈레포트 사용하면 사용안할 함수들
        private void SetPage(C1PrintDocument doc)
        {
            doc.Clear();
            doc.PageLayout.PageSettings.Landscape = true; //가로
            doc.PageLayout.PageSettings.LeftMargin = "2.5cm";
            doc.PageLayout.PageSettings.RightMargin = "1cm";
            doc.PageLayout.PageSettings.TopMargin = "0.4cm";
            doc.PageLayout.PageSettings.BottomMargin = "0.8cm";

        }

        private void SetPageHeader(C1PrintDocument doc, int PK_SEQ)
        {
            TableCell c = null;
            RenderTable hTable = new RenderTable();
            RenderTable hSubTable1 = new RenderTable();
            RenderTable hSubTable2 = new RenderTable();

            hTable.Rows.Insert(0, 2);

            //헤더
            hTable.Style.Font = new Font("맑은고딕", 8);
            hTable.Style.TextAlignHorz = AlignHorzEnum.Left;
            hTable.Style.TextAlignVert = AlignVertEnum.Center;
            hTable.Style.GridLines.All = LineDef.Empty;

            hTable.RowGroups[1, 1].Style.Borders.Top = new LineDef("0.1mm", Color.Black);
            hTable.Cells[1, 0].Style.Borders.Left = new LineDef("0.1mm", Color.Black);
            hTable.Cells[1, 0].Style.Borders.Right = new LineDef("0.1mm", Color.Black);

            //보고서번호 라인, 담당자ID 라인
            hSubTable1.Style.Font = new Font("맑은고딕", 8);
            hSubTable1.Style.TextAlignHorz = AlignHorzEnum.Left;
            hSubTable1.Style.TextAlignVert = AlignVertEnum.Center;
            hSubTable1.Style.GridLines.All = LineDef.Empty;

            //요구 라인
            hSubTable2.Style.Font = new Font("맑은고딕", 9, System.Drawing.FontStyle.Bold);
            hSubTable2.Style.TextAlignHorz = AlignHorzEnum.Left;
            hSubTable2.Style.TextAlignVert = AlignVertEnum.Center;
            hSubTable2.Style.GridLines.All = LineDef.Empty;

            hSubTable2.Cells[0, 0].Style.Borders.Right = new LineDef("0.1mm", Color.Black);
            hSubTable2.Cells[0, 1].Style.Borders.Right = new LineDef("0.1mm", Color.Black);
            hSubTable2.Cells[0, 2].Style.Borders.Right = new LineDef("0.1mm", Color.Black);
            hSubTable2.Cells[0, 3].Style.Borders.Right = new LineDef("0.1mm", Color.Black);
            hSubTable2.Cells[0, 4].Style.Borders.Right = new LineDef("0.1mm", Color.Black);
            hSubTable2.Cells[0, 5].Style.Borders.Right = new LineDef("0.1mm", Color.Black);
            hSubTable2.Cells[0, 6].Style.Borders.Right = new LineDef("0.1mm", Color.Black);
            hSubTable2.Cells[0, 7].Style.Borders.Right = new LineDef("0.1mm", Color.Black);
            hSubTable2.Cells[0, 8].Style.Borders.Right = new LineDef("0.1mm", Color.Black);

            //hSubTable2.CellStyle.GridLines.Bottom = LineDef.Empty;

            hSubTable2.Cells[0, 0].Style.Borders.Bottom = new LineDef("0.1mm", Color.Black);
            hSubTable2.Cells[0, 1].Style.Borders.Bottom = new LineDef("0.1mm", Color.Black);
            hSubTable2.Cells[0, 2].Style.Borders.Bottom = new LineDef("0.1mm", Color.Black);
            hSubTable2.Cells[0, 3].Style.Borders.Bottom = new LineDef("0.1mm", Color.Black);
            hSubTable2.Cells[0, 4].Style.Borders.Bottom = new LineDef("0.1mm", Color.Black);
            hSubTable2.Cells[0, 5].Style.Borders.Bottom = new LineDef("0.1mm", Color.Black);
            hSubTable2.Cells[0, 6].Style.Borders.Bottom = new LineDef("0.1mm", Color.Black);
            hSubTable2.Cells[0, 7].Style.Borders.Bottom = new LineDef("0.1mm", Color.Black);
            hSubTable2.Cells[0, 8].Style.Borders.Bottom = new LineDef("0.1mm", Color.Black);
            hSubTable2.Cells[0, 9].Style.Borders.Bottom = new LineDef("0.1mm", Color.Black);

            //페이지 헤더 첫번째 설정 - 보고서번호,담당자 ID 2줄 동시에 설정
            c = hSubTable1.Cells[0, 0];
            c.Text = ""; //보고서 번호   :
            hSubTable1.Cols[0].Width = "2.6cm";

            //보고서번호
            c = hSubTable1.Cells[0, 1];
            c.Text = "";
            hSubTable1.Cols[1].Width = "6cm";

            c = hSubTable1.Cells[0, 2];
            c.Text = "방산 원가계산서(갑)";
            c.SpanRows = 2;
            c.Style.Font = new Font("맑은고딕", 15, System.Drawing.FontStyle.Bold);
            c.Style.TextAlignHorz = AlignHorzEnum.Center;
            c.Style.FontUnderline = true;

            c = hSubTable1.Cells[0, 3];
            c.Text = "날     짜 :";
            hSubTable1.Cols[3].Width = "1.8cm";

            //날짜
            c = hSubTable1.Cells[0, 4];
            c.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            hSubTable1.Cols[4].Width = "2.9cm";

            c = hSubTable1.Cells[1, 0];
            c.Text = "";//담 당 자 ID   :

            //담당자ID
            c = hSubTable1.Cells[1, 1];
            c.Text = "";

            c = hSubTable1.Cells[1, 3];
            c.Text = "페 이 지 :";

            //페이지
            c = hSubTable1.Cells[1, 4];
            c.Text = "[PageNo] / [PageCount]";
            c.Style.TextAlignHorz = AlignHorzEnum.Center;

            hTable.Cells[0, 0].RenderObject = hSubTable1;

            c = hSubTable2.Cells[0, 0];
            c.Text = "요구";
            c.Style.BackColor = Color.LightGray;
            hSubTable2.Cols[0].Width = "0.8cm";
            c.Style.TextAlignHorz = AlignHorzEnum.Center;

            string strSql = "";
            strSql = " usp_DAB002_R01 ";
            strSql += "  @pTYPE   = 'S2'";
            strSql += ", @pPK_SEQ = " + PK_SEQ;
            strSql += ", @pMNUF_CODE = '" + cboH_MNUF_CODE.SelectedValue.ToString() + "' ";
            strSql += ", @pSTD_YRMON = '" + fpSpread2.Sheets[0].Cells[fpSpread2.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx2, "제출년월")].Value.ToString().Replace("-", "") + "' ";
            strSql += ", @pIN_ID  ='" + SystemBase.Base.gstrUserID + "' ";
            DataTable dt = SystemBase.DbOpen.NoTranDataTable(strSql);

            //연도
            c = hSubTable2.Cells[0, 1];
            if (dt != null && dt.Rows.Count > 0)
            {
                c.Text = dt.Rows[0]["ORDR_YEAR"].ToString();
            }
            c.Style.TextAlignHorz = AlignHorzEnum.Center;
            hSubTable2.Cols[1].Width = "1.2cm";

            c = hSubTable2.Cells[0, 2];
            c.Text = "부서";
            c.Style.BackColor = Color.LightGray;
            c.Style.TextAlignHorz = AlignHorzEnum.Center;
            hSubTable2.Cols[2].Width = "0.8cm";

            //부서명
            c = hSubTable2.Cells[0, 3];
            if (dt != null && dt.Rows.Count > 0)
            {
                c.Text = dt.Rows[0]["DPRT_NAME"].ToString();
            }
            c.CellStyle.Padding.Left = "0.1cm";            
            hSubTable2.Cols[3].Width = "4cm";

            c = hSubTable2.Cells[0, 4];
            c.Text = "판단번호";
            c.Style.BackColor = Color.LightGray;
            c.Style.TextAlignHorz = AlignHorzEnum.Center;
            hSubTable2.Cols[4].Width = "1.5cm";

            //판단번호
            c = hSubTable2.Cells[0, 5];
            if (dt != null && dt.Rows.Count > 0)
            {
                c.Text = dt.Rows[0]["DCSN_NUMB"].ToString();
            }
            c.CellStyle.Padding.Left = "0.1cm";
            hSubTable2.Cols[5].Width = "2cm";

            //SC
            c = hSubTable2.Cells[0, 6];
            c.Text = "";
            c.Style.TextAlignHorz = AlignHorzEnum.Center;
            hSubTable2.Cols[6].Width = "0.7cm";

            //부품
            c = hSubTable2.Cells[0, 7];
            if (dt != null && dt.Rows.Count > 0)
            {
                c.Text = dt.Rows[0]["RPST_ITNM"].ToString();
            }
            c.CellStyle.Padding.Left = "0.1cm";    

            //"업체" 타이틀 
            c = hSubTable2.Cells[0, 8];
            c.Text = "업체";
            c.Style.BackColor = Color.LightGray;
            c.Style.TextAlignHorz = AlignHorzEnum.Center;
            hSubTable2.Cols[8].Width = "1cm";

            //업체
            c = hSubTable2.Cells[0, 9];
            if (dt != null && dt.Rows.Count > 0)
            {
                c.Text = dt.Rows[0]["MNUF_NAME"].ToString();
            }
            c.CellStyle.Padding.Left = "0.1cm";
            hSubTable2.Cols[9].Width = "6.5cm";

            hTable.Cells[1, 0].RenderObject = hSubTable2;

            doc.PageLayout.PageHeader = hTable;
            //doc.PageLayout.PageHeader.Style.TextAlignHorz = AlignHorzEnum.Right;
            //doc.PageLayout.PageHeader.Style.Spacing.Bottom = "0.0cm";
            //doc.PageLayout.PageHeader.Style.Borders.Bottom = LineDef.Default; 
        }

        private void SetColumnHeader(RenderTable rt)
        {
            rt.Cols[0].Width = "0.5cm";//첫번째 타이틀
            rt.Cols[1].Width = "0.5cm";//두번째 타이틀
            rt.Cols[2].Width = "1.9cm";//세번째 타이틀
            rt.Cols[3].Width = "0.83cm";//비율

            // rt.Cols[3].CellStyle.Padding.Right = "0.1cm"; //
            // rt.Cols[5].CellStyle.Padding.Right = "0.1cm"; //
            // rt.Cols[6].CellStyle.Padding.Right = "0.1cm"; //

            //  rt.Cols[3].Style.TextAlignHorz = AlignHorzEnum.Right; //
            //  rt.Cols[5].Style.TextAlignHorz = AlignHorzEnum.Right; //
            //  rt.Cols[6].Style.TextAlignHorz = AlignHorzEnum.Right; //

            int iRow = 0;
            TableCell c;
            //1.3 "재고번호" 타이틀, "품명" 타이틀, "구분" 타이틀 헤더 설정하기 : 데이타 출력 Col과 맞물리는 헤더임

            iRow = 0;
            c = rt.Cells[iRow, 0];
            c.Text = "재고번호 단위 항목";
            c.SpanCols = 4;
            c.Style.BackColor = Color.LightGray;
            c.Style.TextAlignHorz = AlignHorzEnum.Center;

            //"품명" 타이틀:폰트, 백그라운드, 선, 길이 지정
            iRow = iRow + 1;

            c = rt.Cells[iRow, 0];
            c.Text = "품명";
            c.SpanCols = 4;
            c.Style.BackColor = Color.LightGray;
            c.Style.TextAlignHorz = AlignHorzEnum.Center;

            //"구분" 타이틀:폰트, 백그라운드, 선, 길이 지정
            iRow = iRow + 1;
            c = rt.Cells[iRow, 0];
            c.Text = "구분";
            c.SpanCols = 3;
            c.Style.BackColor = Color.LightGray;
            c.Style.TextAlignHorz = AlignHorzEnum.Center;

            //"비율" 타이틀:폰트, 백그라운드, 선, 길이 지정
            c = rt.Cells[iRow, 3];
            c.Text = "비율";
            c.Style.BackColor = Color.LightGray;
            c.Style.TextAlignHorz = AlignHorzEnum.Center;

            //재료비 
            iRow = iRow + 1;
            c = rt.Cells[iRow, 0];
            c.Text = "재료비";
            c.SpanRows = 12;
            c.Style.FontBold = true;
            c.Style.TextAlignHorz = AlignHorzEnum.Center;

            c = rt.Cells[iRow, 1];
            c.Text = "직접";
            c.SpanRows = 6;
            c.Style.FontBold = true;
            c.Style.TextAlignHorz = AlignHorzEnum.Center;

            rt.Cells[iRow, 2].Text = "주요재료비";

            iRow = iRow + 1;
            rt.Cells[iRow, 2].Text = "구입부품비";

            iRow = iRow + 1;
            rt.Cells[iRow, 2].Text = "방산부품비";

            iRow = iRow + 1;
            rt.Cells[iRow, 2].Text = "수입재료비";

            iRow = iRow + 1;
            rt.Cells[iRow, 2].Text = "수입부품비";

            iRow = iRow + 1;
            rt.Cells[iRow, 2].Text = "포장재료비";

            iRow = iRow + 1;
            c = rt.Cells[iRow, 1];
            c.Text = "(반제품비)";
            c.SpanCols = 2;

            iRow = iRow + 1;
            c = rt.Cells[iRow, 1];
            c.Text = "간접재료비";
            c.SpanCols = 2;

            iRow = iRow + 1;
            c = rt.Cells[iRow, 1];
            c.Text = "소계";
            c.SpanCols = 3;

            iRow = iRow + 1;
            c = rt.Cells[iRow, 1];
            c.Text = "작업설물(-)";
            c.SpanCols = 2;

            iRow = iRow + 1;
            c = rt.Cells[iRow, 1];
            c.Text = "합계";
            c.SpanCols = 3;

            iRow = iRow + 1;
            c = rt.Cells[iRow, 1];
            c.Text = "(관급재료비)";
            c.SpanCols = 2;

            iRow = iRow + 1;
            c = rt.Cells[iRow, 0];
            c.Text = "";
            c.SpanRows = 3;

            c = rt.Cells[iRow, 1];
            c.Text = "직접노무비";
            c.SpanCols = 2;

            iRow = iRow + 1;
            c = rt.Cells[iRow, 0];
            c.Text = "";
            c.SpanRows = 3;

            c = rt.Cells[iRow, 1];
            c.Text = "간접";
            c.SpanCols = 2;

            iRow = iRow + 1;
            c = rt.Cells[iRow, 0];
            c.Text = "";
            c.SpanRows = 3;

            c = rt.Cells[iRow, 1];
            c.Text = "계";
            c.SpanCols = 3;


            //경비
            iRow = iRow + 1;
            c = rt.Cells[iRow, 0];
            c.Text = "경비";
            c.SpanRows = 18;
            c.Style.FontBold = true;
            c.Style.TextAlignHorz = AlignHorzEnum.Center;

            c = rt.Cells[iRow, 1];
            c.Text = "직접";
            c.SpanRows = 17;
            c.Style.FontBold = true;
            c.Style.TextAlignHorz = AlignHorzEnum.Center;

            rt.Cells[iRow, 2].Text = "감가상각비";

            iRow = iRow + 1;
            rt.Cells[iRow, 2].Text = "연구개발비";

            iRow = iRow + 1;
            rt.Cells[iRow, 2].Text = "기  술  료";

            iRow = iRow + 1;
            rt.Cells[iRow, 2].Text = "시험검사비";

            iRow = iRow + 1;
            rt.Cells[iRow, 2].Text = "지급임차료";

            iRow = iRow + 1;
            rt.Cells[iRow, 2].Text = "외주가공비";

            iRow = iRow + 1;
            rt.Cells[iRow, 2].Text = "중소외주가공";

            iRow = iRow + 1;
            rt.Cells[iRow, 2].Text = "설치시운전비";

            iRow = iRow + 1;
            rt.Cells[iRow, 2].Text = "특허권사용료";

            iRow = iRow + 1;
            rt.Cells[iRow, 2].Text = "공사비";

            iRow = iRow + 1;
            rt.Cells[iRow, 2].Text = "공식행사비";

            iRow = iRow + 1;
            rt.Cells[iRow, 2].Text = "설계비";

            iRow = iRow + 1;
            rt.Cells[iRow, 2].Text = "보관비";

            iRow = iRow + 1;
            rt.Cells[iRow, 2].Text = "";

            iRow = iRow + 1;
            rt.Cells[iRow, 2].Text = "";

            iRow = iRow + 1;
            rt.Cells[iRow, 2].Text = "";

            //rt.Cells[iRow, 2].Text = "소계";
            iRow = iRow + 1;
            c = rt.Cells[iRow, 2];
            c.Text = "소계";
            c.SpanCols = 2;

            iRow = iRow + 1;
            c = rt.Cells[iRow, 0];
            c.Text = "";
            c.SpanRows = 3;
            c = rt.Cells[iRow, 1];
            c.Text = "간접";
            c.SpanCols = 2;

            iRow = iRow + 1;
            c = rt.Cells[iRow, 0];
            c.Text = "합계";
            c.SpanCols = 4;
            c.Style.BackColor = Color.LightGray;

            iRow = iRow + 1;
            c = rt.Cells[iRow, 0];
            c.Text = "제조원가(관급포함)";
            c.SpanCols = 4;

            iRow = iRow + 1;
            c = rt.Cells[iRow, 0];
            c.Text = "제조원가(관급제외)";
            c.SpanCols = 4;

            iRow = iRow + 1;
            c = rt.Cells[iRow, 0];
            c.Text = "일반관리비";
            c.SpanCols = 3;
            c = rt.Cells[iRow, 3];

            iRow = iRow + 1;
            c = rt.Cells[iRow, 0];
            c.Text = "총  원  가";
            c.SpanCols = 4;
            c.Style.BackColor = Color.DarkGray;

            iRow = iRow + 1;
            c = rt.Cells[iRow, 0];
            c.Text = "투하자본보상비";
            c.SpanCols = 3;
            c = rt.Cells[iRow, 3];

            //이윤
            iRow = iRow + 1;
            c = rt.Cells[iRow, 0];
            c.Text = "이윤";
            c.SpanRows = 11;
            c.Style.FontBold = true;
            c.Style.TextAlignHorz = AlignHorzEnum.Center;

            c = rt.Cells[iRow, 1];
            c.Text = "기본보상";
            c.SpanCols = 2;

            iRow = iRow + 1;
            c = rt.Cells[iRow, 1];
            c.Text = "기술위험보상";
            c.SpanCols = 2;

            iRow = iRow + 1;
            c = rt.Cells[iRow, 1];
            c.Text = "계약위험보상";
            c.SpanCols = 2;

            iRow = iRow + 1;
            c = rt.Cells[iRow, 1];
            c.Text = "계약수행노력보";
            c.SpanCols = 2;

            iRow = iRow + 1;
            c = rt.Cells[iRow, 1];
            c.Text = "원가절감노력보";
            c.SpanCols = 2;

            iRow = iRow + 1;
            c = rt.Cells[iRow, 1];
            c.Text = "설비투자노력보";
            c.SpanCols = 2;

            iRow = iRow + 1;
            c = rt.Cells[iRow, 1];
            c.Text = "경영노력(통보율)";
            c.SpanCols = 2;

            iRow = iRow + 1;
            c = rt.Cells[iRow, 1];
            c.Text = "경영노력(품질)";
            c.SpanCols = 2;

            iRow = iRow + 1;
            c = rt.Cells[iRow, 1];
            c.Text = "경영노력(연계)";
            c.SpanCols = 2;

            iRow = iRow + 1;
            c = rt.Cells[iRow, 1];
            c.Text = "경영노력(부당이득가산금)";
            c.SpanCols = 2;

            iRow = iRow + 1;
            c = rt.Cells[iRow, 1];
            c.Text = "소계";
            c.SpanCols = 2;

            iRow = iRow + 1;
            c = rt.Cells[iRow, 0];
            c.Text = "부품국산화";
            c.SpanCols = 4;

            iRow = iRow + 1;
            c = rt.Cells[iRow, 0];
            c.Text = "관세 등";
            c.SpanCols = 4;

            iRow = iRow + 1;
            c = rt.Cells[iRow, 0];
            c.Text = "B.I.I";
            c.SpanCols = 4;

            iRow = iRow + 1;
            c = rt.Cells[iRow, 0];
            c.Text = "추가비목경비";
            c.SpanCols = 4;

            iRow = iRow + 1;
            c = rt.Cells[iRow, 0];
            c.Text = "수출물량(감가상각비)";
            c.SpanCols = 4;

            iRow = iRow + 1;
            c = rt.Cells[iRow, 0];
            c.Text = "계산가격";
            c.SpanCols = 4;
            c.Style.BackColor = Color.DarkGray;

        }

        private void SetDataBinding(C1PrintDocument doc, RenderTable rt, int PK_SEQ)
        {
            /*
            TableCell c;
            int iRow = 0;

            string strSql = "";

            strSql = " usp_DAB002_R01 ";
            strSql += "  @pTYPE   = 'I1'";
            strSql += ", @pPK_SEQ = " + PK_SEQ;
            strSql += ", @pMNUF_CODE = '" + cboH_MNUF_CODE.SelectedValue.ToString() + "' ";
            strSql += ", @pSTD_YRMON = '" + fpSpread2.Sheets[0].Cells[fpSpread2.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx2, "제출년월")].Value.ToString().Replace("-", "") + "' ";
            strSql += ", @pIN_ID ='" + SystemBase.Base.gstrUserID + "' ";                           

    
            DataTable dt = SystemBase.DbOpen.NoTranDataTable(strSql);
            if (dt != null && dt.Rows.Count > 0)
            {
                //01.주요재료비~10.작업설물(-)
                iRow = 3;
                for (int row = 1; row <= 10; ++row)
                {

                    if (row != 9)
                    {
                        c = rt.Cells[iRow, 3];
                        c.Text = string.Format("{0:#,###.##}", dt.Rows[0]["A" + string.Format("{0:00}", row) + "_RATE"]);
                        c.Style.TextAlignHorz = AlignHorzEnum.Right;
                        c.CellStyle.Padding.Right = "0.1cm";
                    }

                    iRow++;
                }

                //19.관급재료비
                c = rt.Cells[14, 3];
                c.Text = string.Format("{0:#,###.##}", dt.Rows[0]["A19_RATE"]);
                c.Style.TextAlignHorz = AlignHorzEnum.Right;
                c.CellStyle.Padding.Right = "0.1cm";

                //20.직접노무비
                c = rt.Cells[15, 3];
                c.Text = string.Format("{0:#,###.##}", dt.Rows[0]["A20_RATE"]);
                c.Style.TextAlignHorz = AlignHorzEnum.Right;
                c.CellStyle.Padding.Right = "0.1cm";

                //21.간접노무비
                c = rt.Cells[16, 1];
                c.Text = "간접(    " + string.Format("{0:#,###.##}", dt.Rows[0]["A21_ETC_RATE"]) + " %)";

                c = rt.Cells[16, 3];
                c.Text = string.Format("{0:#,###.##}", dt.Rows[0]["A21_RATE"]);
                c.Style.TextAlignHorz = AlignHorzEnum.Right;
                c.CellStyle.Padding.Right = "0.1cm";

                //30.감가상각비~42.보관비
                iRow = 18;
                for (int row = 30; row <= 42; ++row)
                {
                    c = rt.Cells[iRow, 3];
                    c.Text = string.Format("{0:#,###.##}", dt.Rows[0]["A" + string.Format("{0:00}", row) + "_RATE"]);
                    c.Style.TextAlignHorz = AlignHorzEnum.Right;
                    c.CellStyle.Padding.Right = "0.1cm";

                    iRow++;
                }

                //48.간접경비
                c = rt.Cells[35, 1];
                c.Text = "간접(    " + string.Format("{0:#,###.##}", dt.Rows[0]["A48_ETC_RATE"]) + " %)";

                c = rt.Cells[35, 3];
                c.Text = string.Format("{0:#,###.##}", dt.Rows[0]["A48_RATE"]);
                c.Style.TextAlignHorz = AlignHorzEnum.Right;
                c.CellStyle.Padding.Right = "0.1cm";

                //55.일반관리비
                c = rt.Cells[39, 0];
                c.Text = "일반관리비(    " + string.Format("{0:#,###.##}", dt.Rows[0]["A55_ETC_RATE"]) + " %)";
                c.SpanCols = 3;
                c.Style.TextAlignHorz = AlignHorzEnum.Right;
                c.CellStyle.Padding.Right = "0.1cm";

                c = rt.Cells[39, 3];
                c.Text = string.Format("{0:#,###.##}", dt.Rows[0]["A55_RATE"]);
                c.Style.TextAlignHorz = AlignHorzEnum.Right;
                c.CellStyle.Padding.Right = "0.1cm";

                //60.투하자본보상비~63.계약위험보상
                iRow = 41;
                for (int row = 60; row <= 63; ++row)
                {
                    c = rt.Cells[iRow, 3];
                    c.Text = string.Format("{0:#,###.##}", dt.Rows[0]["A" + string.Format("{0:00}", row) + "_RATE"]);
                    c.Style.TextAlignHorz = AlignHorzEnum.Right;
                    c.CellStyle.Padding.Right = "0.1cm";

                    iRow++;
                }

                //65.원가절감노력보상~70.경영노력(부당이득가산금) (-)
                iRow = 46;
                for (int row = 65; row <= 70; ++row)
                {
                    c = rt.Cells[iRow, 3];
                    c.Text = string.Format("{0:#,###.##}", dt.Rows[0]["A" + string.Format("{0:00}", row) + "_RATE"]);
                    c.Style.TextAlignHorz = AlignHorzEnum.Right;
                    c.CellStyle.Padding.Right = "0.1cm";

                    iRow++;
                }
            }
 

            strSql = " usp_DAB002_R01 ";
            strSql += "  @pTYPE   = 'I1'";
            strSql += ", @pPK_SEQ = " + PK_SEQ;
            strSql += ", @pMNUF_CODE = '" + cboH_MNUF_CODE.SelectedValue.ToString() + "' ";
            strSql += ", @pSTD_YRMON = '" + fpSpread2.Sheets[0].Cells[fpSpread2.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx2, "제출년월")].Value.ToString().Replace("-", "") + "' ";
            strSql += ", @pIN_ID ='" + SystemBase.Base.gstrUserID + "' ";                        

             

            //칼럼값 정의 1
            rt.ColGroups[4, 4].DataBinding.DataSource = SystemBase.DbOpenForReport.C1ReportDataSet(doc, strSql);

            //rt.Cols[4].CellStyle.Padding.Right = "0.1cm"; //순번
            //rt.Cols[5].CellStyle.Padding.Right = "0.1cm"; //금액
            // rt.Cols[6].CellStyle.Padding.Right = "0.1cm"; //이윤액

            //rt.Cols[4].Style.TextAlignHorz = AlignHorzEnum.Right; //순번
            //rt.Cols[5].Style.TextAlignHorz = AlignHorzEnum.Right; //금액
            // rt.Cols[6].Style.TextAlignHorz = AlignHorzEnum.Right; //이윤액

            rt.Cols[4].Width = "0.5cm";  //순번
            rt.Cols[5].Width = "1.87cm"; //금액
            rt.Cols[6].Width = "1.87cm"; //이윤액
            rt.Cols[7].Width = "0.2cm";  //공백

            iRow = 0;
            c = rt.Cells[iRow, 4];
            c.Text = "[Fields!SEQ.Value]";
            c.SpanRows = 2;
            c.Style.BackColor = Color.LightGray;
            c.Style.TextAlignHorz = AlignHorzEnum.Center;

            c = rt.Cells[iRow, 5];
            c.Text = "[Fields!NIIN.Value]" + " " + "[Fields!UNIT.Value]" + " " + "[Fields!DMST_ITNB.Value]";
            c.SpanCols = 2;
            c.Style.BackColor = Color.LightGray;
            c.Style.TextAlignHorz = AlignHorzEnum.Left;
            c.CellStyle.Padding.Left = "0.1cm";

            c = rt.Cells[iRow, 7];
            c.SpanRows = rt.Rows.Count;
            c.Text = "";

            //칼럼값 정의 2
            iRow++;
            c = rt.Cells[iRow, 5];
            c.Text = "[Fields!RPST_ITNM.Value]";
            c.SpanCols = 2;
            c.Style.BackColor = Color.LightGray;
            c.Style.TextAlignHorz = AlignHorzEnum.Left;
            c.CellStyle.Padding.Left = "0.1cm";

            iRow++;
            c = rt.Cells[iRow, 4];
            c.Text = "금액";
            c.SpanCols = 2;
            c.Style.BackColor = Color.LightGray;
            c.Style.TextAlignHorz = AlignHorzEnum.Center;

            c = rt.Cells[iRow, 6];
            c.Text = "이윤액";
            c.Style.BackColor = Color.LightGray;
            c.Style.TextAlignHorz = AlignHorzEnum.Center;

            //1.주요재료비 ~ 10.작업설물(-)
            for (int row = 1; row <= 10; ++row)
            {
                iRow++;

                c = rt.Cells[iRow, 4];
                c.Text = "[string.Format(\"{0:#,###.##}\", Fields!A" + string.Format("{0:00}", row) + "_AMT.Value)]";
                c.SpanCols = 2;
                c.Style.TextAlignHorz = AlignHorzEnum.Right;
                c.CellStyle.Padding.Right = "0.1cm";

                c = rt.Cells[iRow, 6];
                c.Text = "[string.Format(\"{0:#,###.##}\", Fields!A" + string.Format("{0:00}", row) + "_PROFIT.Value)]";
                c.Style.TextAlignHorz = AlignHorzEnum.Right;
                c.CellStyle.Padding.Right = "0.1cm";
            }
            //18.[재료비합계]
            iRow++;
            c = rt.Cells[iRow, 4];
            c.Text = "[string.Format(\"{0:#,###.##}\", Fields!A18_AMT.Value)]";
            c.SpanCols = 2;
            c.Style.TextAlignHorz = AlignHorzEnum.Right;
            c.CellStyle.Padding.Right = "0.1cm";

            c = rt.Cells[iRow, 6];
            c.Text = "[string.Format(\"{0:#,###.##}\", Fields!A18_PROFIT.Value)]";
            c.Style.TextAlignHorz = AlignHorzEnum.Right;
            c.CellStyle.Padding.Right = "0.1cm";

            //19.관급재료비
            iRow++;
            c = rt.Cells[iRow, 4];
            c.Text = "[string.Format(\"{0:#,###.##}\", Fields!A19_AMT.Value)]";
            c.SpanCols = 2;
            c.Style.TextAlignHorz = AlignHorzEnum.Right;
            c.CellStyle.Padding.Right = "0.1cm";

            c = rt.Cells[iRow, 6];
            c.Text = "[string.Format(\"{0:#,###.##}\", Fields!A19_PROFIT.Value)]";
            c.Style.TextAlignHorz = AlignHorzEnum.Right;
            c.CellStyle.Padding.Right = "0.1cm";

            //20.직접노무비
            iRow++;
            c = rt.Cells[iRow, 4];
            c.Text = "[string.Format(\"{0:#,###.##}\", Fields!A20_AMT.Value)]";
            c.SpanCols = 2;
            c.Style.TextAlignHorz = AlignHorzEnum.Right;
            c.CellStyle.Padding.Right = "0.1cm";

            c = rt.Cells[iRow, 6];
            c.Text = "[string.Format(\"{0:#,###.##}\", Fields!A20_PROFIT.Value)]";
            c.Style.TextAlignHorz = AlignHorzEnum.Right;
            c.CellStyle.Padding.Right = "0.1cm";

            //21.간접노무비
            iRow++;
            c = rt.Cells[iRow, 4];
            c.Text = "[string.Format(\"{0:#,###.##}\", Fields!A21_AMT.Value)]";
            c.SpanCols = 2;
            c.Style.TextAlignHorz = AlignHorzEnum.Right;
            c.CellStyle.Padding.Right = "0.1cm";

            c = rt.Cells[iRow, 6];
            c.Text = "[string.Format(\"{0:#,###.##}\", Fields!A21_PROFIT.Value)]";
            c.Style.TextAlignHorz = AlignHorzEnum.Right;
            c.CellStyle.Padding.Right = "0.1cm";

            //25.[노무비합계]
            iRow++;
            c = rt.Cells[iRow, 4];
            c.Text = "[string.Format(\"{0:#,###.##}\", Fields!A25_AMT.Value)]";
            c.SpanCols = 2;
            c.Style.TextAlignHorz = AlignHorzEnum.Right;
            c.CellStyle.Padding.Right = "0.1cm";

            c = rt.Cells[iRow, 6];
            c.Text = "[string.Format(\"{0:#,###.##}\", Fields!A25_PROFIT.Value)]";
            c.Style.TextAlignHorz = AlignHorzEnum.Right;
            c.CellStyle.Padding.Right = "0.1cm";


            //30.감가상각비 ~ 42.보관비
            for (int row = 30; row <= 42; ++row)
            {
                iRow++;
                c = rt.Cells[iRow, 4];
                c.Text = "[string.Format(\"{0:#,###.##}\", Fields!A" + string.Format("{0:00}", row) + "_AMT.Value)]";
                c.SpanCols = 2;
                c.Style.TextAlignHorz = AlignHorzEnum.Right;
                c.CellStyle.Padding.Right = "0.1cm";

                c = rt.Cells[iRow, 6];
                c.Text = "[string.Format(\"{0:#,###.##}\", Fields!A" + string.Format("{0:00}", row) + "_PROFIT.Value)]";
                c.Style.TextAlignHorz = AlignHorzEnum.Right;
                c.CellStyle.Padding.Right = "0.1cm";
            }

            //공백3줄
            for (int row = 42; row <= 44; ++row)
            {
                iRow++;
                c = rt.Cells[iRow, 4];
                c.Text = "";
                c.SpanCols = 2;

                c = rt.Cells[iRow, 6];
                c.Text = "";

            }

            //47.소계
            iRow++;
            c = rt.Cells[iRow, 4];
            c.Text = "[string.Format(\"{0:#,###.##}\", Fields!A47_AMT.Value)]";
            c.SpanCols = 2;
            c.Style.TextAlignHorz = AlignHorzEnum.Right;
            c.CellStyle.Padding.Right = "0.1cm";

            c = rt.Cells[iRow, 6];
            c.Text = "[string.Format(\"{0:#,###.##}\", Fields!A47_PROFIT.Value)]";
            c.Style.TextAlignHorz = AlignHorzEnum.Right;
            c.CellStyle.Padding.Right = "0.1cm";

            //48.간접경비
            iRow++;
            c = rt.Cells[iRow, 4];
            c.Text = "[string.Format(\"{0:#,###.##}\", Fields!A48_AMT.Value)]";
            c.SpanCols = 2;
            c.Style.TextAlignHorz = AlignHorzEnum.Right;
            c.CellStyle.Padding.Right = "0.1cm";

            c = rt.Cells[iRow, 6];
            c.Text = "[string.Format(\"{0:#,###.##}\", Fields!A48_PROFIT.Value)]";
            c.Style.TextAlignHorz = AlignHorzEnum.Right;
            c.CellStyle.Padding.Right = "0.1cm";


            //49.[경비합계]
            iRow++;
            c = rt.Cells[iRow, 4];
            c.Text = "[string.Format(\"{0:#,###.##}\", Fields!A49_AMT.Value)]";
            c.SpanCols = 2;
            c.Style.TextAlignHorz = AlignHorzEnum.Right;
            c.CellStyle.Padding.Right = "0.1cm";

            c = rt.Cells[iRow, 6];
            c.Text = "[string.Format(\"{0:#,###.##}\", Fields!A49_PROFIT.Value)]";
            c.Style.TextAlignHorz = AlignHorzEnum.Right;
            c.CellStyle.Padding.Right = "0.1cm";


            //50.제조원가(관급포함)
            iRow++;
            c = rt.Cells[iRow, 4];
            c.Text = "[string.Format(\"{0:#,###.##}\", Fields!A50_AMT.Value)]";
            c.SpanCols = 2;
            c.Style.TextAlignHorz = AlignHorzEnum.Right;
            c.CellStyle.Padding.Right = "0.1cm";

            c = rt.Cells[iRow, 6];
            c.Text = "[string.Format(\"{0:#,###.##}\", Fields!A50PROFIT.Value)]";
            c.Style.TextAlignHorz = AlignHorzEnum.Right;
            c.CellStyle.Padding.Right = "0.1cm";


            //51.제조원가(관급제외)
            iRow++;
            c = rt.Cells[iRow, 4];
            c.Text = "[string.Format(\"{0:#,###.##}\", Fields!A51_AMT.Value)]";
            c.SpanCols = 2;
            c.Style.TextAlignHorz = AlignHorzEnum.Right;
            c.CellStyle.Padding.Right = "0.1cm";

            c = rt.Cells[iRow, 6];
            c.Text = "[string.Format(\"{0:#,###.##}\", Fields!A51_PROFIT.Value)]";
            c.Style.TextAlignHorz = AlignHorzEnum.Right;
            c.CellStyle.Padding.Right = "0.1cm";

            //55.일반관리비
            iRow++;
            c = rt.Cells[iRow, 4];
            c.Text = "[string.Format(\"{0:#,###.##}\", Fields!A55_AMT.Value)]";
            c.SpanCols = 2;
            c.Style.TextAlignHorz = AlignHorzEnum.Right;
            c.CellStyle.Padding.Right = "0.1cm";

            c = rt.Cells[iRow, 6];
            c.Text = "[string.Format(\"{0:#,###.##}\", Fields!A55_PROFIT.Value)]";
            c.Style.TextAlignHorz = AlignHorzEnum.Right;
            c.CellStyle.Padding.Right = "0.1cm";

            //59.총원가
            iRow++;
            c = rt.Cells[iRow, 4];
            c.Text = "[string.Format(\"{0:#,###.##}\", Fields!A59_AMT.Value)]";
            c.SpanCols = 2;
            c.Style.TextAlignHorz = AlignHorzEnum.Right;
            c.CellStyle.Padding.Right = "0.1cm";

            c = rt.Cells[iRow, 6];
            c.Text = "[string.Format(\"{0:#,###.##}\", Fields!A59_PROFIT.Value)]";
            c.Style.TextAlignHorz = AlignHorzEnum.Right;
            c.CellStyle.Padding.Right = "0.1cm";

            //60.투하자본보상비
            iRow++;
            c = rt.Cells[iRow, 4];
            c.Text = "[string.Format(\"{0:#,###.##}\", Fields!A60_AMT.Value)]";
            c.SpanCols = 2;
            c.Style.TextAlignHorz = AlignHorzEnum.Right;
            c.CellStyle.Padding.Right = "0.1cm";

            c = rt.Cells[iRow, 6];
            c.Text = "[string.Format(\"{0:#,###.##}\", Fields!A60_PROFIT.Value)]";
            c.Style.TextAlignHorz = AlignHorzEnum.Right;
            c.CellStyle.Padding.Right = "0.1cm";


            //61.기본보상
            iRow++;
            c = rt.Cells[iRow, 4];
            c.Text = "[string.Format(\"{0:#,###.##}\", Fields!A61_AMT.Value)]";
            c.SpanCols = 2;
            c.Style.TextAlignHorz = AlignHorzEnum.Right;
            c.CellStyle.Padding.Right = "0.1cm";

            //62.기술위험보상
            iRow++;
            c = rt.Cells[iRow, 4];
            c.Text = "[string.Format(\"{0:#,###.##}\", Fields!A62_AMT.Value)]";
            c.SpanCols = 2;
            c.Style.TextAlignHorz = AlignHorzEnum.Right;
            c.CellStyle.Padding.Right = "0.1cm";

            //63.계약위험보상
            iRow++;
            c = rt.Cells[iRow, 4];
            c.Text = "[string.Format(\"{0:#,###.##}\", Fields!A63_AMT.Value)]";
            c.SpanCols = 2;
            c.Style.TextAlignHorz = AlignHorzEnum.Right;
            c.CellStyle.Padding.Right = "0.1cm";

            //64.계약수행노력보상
            iRow++;
            c = rt.Cells[iRow, 4];
            c.Text = "[string.Format(\"{0:#,###.##}\", Fields!A64_AMT.Value)]";
            c.SpanCols = 2;
            c.Style.TextAlignHorz = AlignHorzEnum.Right;
            c.CellStyle.Padding.Right = "0.1cm";

            //65.원가절감노력보상
            iRow++;
            c = rt.Cells[iRow, 4];
            c.Text = "[string.Format(\"{0:#,###.##}\", Fields!A65_AMT.Value)]";
            c.SpanCols = 2;
            c.Style.TextAlignHorz = AlignHorzEnum.Right;
            c.CellStyle.Padding.Right = "0.1cm";

            //66.설비투자노력보상
            iRow++;
            c = rt.Cells[iRow, 4];
            c.Text = "[string.Format(\"{0:#,###.##}\", Fields!A66_AMT.Value)]";
            c.SpanCols = 2;
            c.Style.TextAlignHorz = AlignHorzEnum.Right;
            c.CellStyle.Padding.Right = "0.1cm";

            //67.경영노력(통보율)
            iRow++;
            c = rt.Cells[iRow, 4];
            c.Text = "[string.Format(\"{0:#,###.##}\", Fields!A67_AMT.Value)]";
            c.SpanCols = 2;
            c.Style.TextAlignHorz = AlignHorzEnum.Right;
            c.CellStyle.Padding.Right = "0.1cm";

            //68.경영노력(품질)
            iRow++;
            c = rt.Cells[iRow, 4];
            c.Text = "[string.Format(\"{0:#,###.##}\", Fields!A68_AMT.Value)]";
            c.SpanCols = 2;
            c.Style.TextAlignHorz = AlignHorzEnum.Right;
            c.CellStyle.Padding.Right = "0.1cm";

            //69.경영노력(연계)
            iRow++;
            c = rt.Cells[iRow, 4];
            c.Text = "[string.Format(\"{0:#,###.##}\", Fields!A69_AMT.Value)]";
            c.SpanCols = 2;
            c.Style.TextAlignHorz = AlignHorzEnum.Right;
            c.CellStyle.Padding.Right = "0.1cm";

            //70.경영노력(부당이득가산금) (-)
            iRow++;
            c = rt.Cells[iRow, 4];
            c.Text = "[string.Format(\"{0:#,###.##}\", Fields!A70_AMT.Value)]";
            c.SpanCols = 2;
            c.Style.TextAlignHorz = AlignHorzEnum.Right;
            c.CellStyle.Padding.Right = "0.1cm";

            //75.[이윤소계]
            iRow++;
            c = rt.Cells[iRow, 4];
            c.Text = "[string.Format(\"{0:#,###.##}\", Fields!A75_AMT.Value)]";
            c.SpanCols = 2;
            c.Style.TextAlignHorz = AlignHorzEnum.Right;
            c.CellStyle.Padding.Right = "0.1cm";

            //76.부품국산화
            iRow++;
            c = rt.Cells[iRow, 4];
            c.Text = "[string.Format(\"{0:#,###.##}\", Fields!A76_AMT.Value)]";
            c.SpanCols = 2;
            c.Style.TextAlignHorz = AlignHorzEnum.Right;
            c.CellStyle.Padding.Right = "0.1cm";

            //77.관세 등
            iRow++;
            c = rt.Cells[iRow, 4];
            c.Text = "[string.Format(\"{0:#,###.##}\", Fields!A77_AMT.Value)]";
            c.SpanCols = 2;
            c.Style.TextAlignHorz = AlignHorzEnum.Right;
            c.CellStyle.Padding.Right = "0.1cm";

            //78.B.I.I
            iRow++;
            c = rt.Cells[iRow, 4];
            c.Text = "[string.Format(\"{0:#,###.##}\", Fields!A78_AMT.Value)]";
            c.SpanCols = 2;
            c.Style.TextAlignHorz = AlignHorzEnum.Right;
            c.CellStyle.Padding.Right = "0.1cm";


            //79.추가비목경비
            iRow++;
            c = rt.Cells[iRow, 4];
            c.Text = "[string.Format(\"{0:#,###.##}\", Fields!A79_AMT.Value)]";
            c.SpanCols = 2;
            c.Style.TextAlignHorz = AlignHorzEnum.Right;
            c.CellStyle.Padding.Right = "0.1cm";

            //80.수출물량(감가상각비)
            iRow++;
            c = rt.Cells[iRow, 4];
            c.Text = "[string.Format(\"{0:#,###.##}\", Fields!A80_AMT.Value)]";
            c.SpanCols = 2;
            c.Style.TextAlignHorz = AlignHorzEnum.Right;
            c.CellStyle.Padding.Right = "0.1cm";

            //85.계산가격
            iRow++;
            c = rt.Cells[iRow, 4];
            c.Text = "[string.Format(\"{0:#,###.##}\", Fields!A85_AMT.Value)]";
            c.SpanCols = 2;
            c.Style.TextAlignHorz = AlignHorzEnum.Right;
            c.CellStyle.Padding.Right = "0.1cm";

            doc.Body.Children.Add(rt);

            rt.SplitHorzBehavior = SplitBehaviorEnum.SplitIfNeeded;
            rt.Width = "auto";
            rt.RowGroups[0, 3].Header = TableHeaderEnum.Page;
            rt.ColGroups[0, 4].Header = TableHeaderEnum.All;
            rt.ColGroups[0, 4].Style.BackColor = Color.White;

            rt.Rows[0].Height = "0.3cm";
            rt.Rows[1].Height = "0.3cm";
            rt.Rows[2].Height = "0.3cm";
            rt.Rows[3].Height = "0.3cm";
            for (int row = 4; row < rt.Rows.Count; ++row)
            {

                rt.Rows[row].Height = "0.3cm";
            }
             * */
        }
        #endregion

        #region FPGrid_SaveCheck - 그리드 데이타 필수항목,Length Check
        private bool FPGrid_SaveCheck2(FarPoint.Win.Spread.FpSpread FPGrid, string FormID, string GridNM, bool Msg)
        {
            bool ChkGrid = true;
            int UpCount = 0;

            try
            {
                string Query = " usp_BAA004 'S7',@PFORM_ID='" + FormID.ToString() + "' , @PGRID_NAME='" + GridNM + "' ";
                DataTable dt = SystemBase.DbOpen.TranDataTable(Query);

                //필수입력사항 체크
                for (int i = 0; i < FPGrid.Sheets[0].Rows.Count; i++)
                {
                    // Row추가자료, Row수정자료, 삭제자료아닌것
                    if (FPGrid.Sheets[0].RowHeader.Cells[i, 0].Text == "I" || FPGrid.Sheets[0].RowHeader.Cells[i, 0].Text == "U" || FPGrid.Sheets[0].RowHeader.Cells[i, 0].Text == "D")
                    {
                        for (int j = 0; j < FPGrid.Sheets[0].Columns.Count - 1; j++)
                        {
                            //필수항목란 체크---->1:필수, 2:읽기전용/필수, 6:읽기전용/필수/포커스제외
                            if ((dt.Rows[j][3].ToString() == "1" || dt.Rows[j][3].ToString() == "2" || dt.Rows[j][3].ToString() == "6")
                                    && (dt.Rows[j][2].ToString() == ""          // 대문자
                                        || dt.Rows[j][2].ToString() == "GN"     // 일반
                                        || dt.Rows[j][2].ToString() == "DT"     // 날짜(전체)
                                        || dt.Rows[j][2].ToString() == "DY"     // 날짜(년월)
                                        || dt.Rows[j][2].ToString() == "DD"     // 날짜(월콤보)
                                        || dt.Rows[j][2].ToString() == "CB"     // 콤보
                                        || dt.Rows[j][2].ToString().Substring(0, 2) == "NM"))  // 숫자  
                            {
                                if ((FPGrid.Sheets[0].Cells[i, j + 1].Value == null || FPGrid.Sheets[0].Cells[i, j + 1].Text.Length == 0)
                                        && FPGrid.Sheets[0].GetCellType(i, j + 1).ToString() != "GeneralCellType"
                                        && FPGrid.Sheets[0].GetCellType(i, j + 1).ToString() != "ButtonCellType"
                                        && FPGrid.Sheets[0].RowHeader.Cells[i, 0].Text != "D")
                                {
                                    MessageBox.Show(Convert.ToString(i + 1) + "번째 Row의 [ " + FPGrid.Sheets[0].ColumnHeader.Cells[0, j + 1].Text.ToString() + " ] 항목은 필수입력 항목입니다.", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    FPGrid.Focus();
                                    FPGrid.ActiveSheet.SetActiveCell(i, j + 1);
                                    ChkGrid = false;
                                    break;
                                }
                            }

                            if (dt.Rows[j][2].ToString() == "DY")  // 마스크에 적용된 년월 체크
                            {
                                if (Convert.ToInt32(FPGrid.Sheets[0].Cells[i, j + 1].Text.Substring(5, 2)) > 12)
                                {
                                    MessageBox.Show(Convert.ToString(i + 1) + "번째 Row의 [ " + FPGrid.Sheets[0].ColumnHeader.Cells[0, j + 1].Text.ToString() + " ] 항목은 날짜형식이 맞지 않습니다.", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    FPGrid.Focus();
                                    FPGrid.ActiveSheet.SetActiveCell(i, j + 1);
                                    ChkGrid = false;
                                    break;

                                }
                            }

                            //LENGTH 체크
                            string[] EtcData = null;
                            if (dt.Rows[j][4].ToString() != "")
                            {
                                // Length;
                                EtcData = dt.Rows[j][4].ToString().Split(';');
                                if (Convert.ToInt32(EtcData[0]) != FPGrid.Sheets[0].Cells[i, j + 1].Text.Length)
                                {
                                    MessageBox.Show(Convert.ToString(i + 1) + "번째 Row의 [ " + FPGrid.Sheets[0].ColumnHeader.Cells[0, j + 1].Text.ToString() + " ] 항목은 Length(" + EtcData[0] + ")가 맞지 않습니다.", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    FPGrid.Focus();
                                    FPGrid.ActiveSheet.SetActiveCell(i, j + 1);
                                    ChkGrid = false;
                                    break;
                                }
                            }
                        }
                        UpCount++;
                    }
                    if (ChkGrid == false)
                        break;
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log("FPGrid_SaveCheck2 (그리드 필수항목 체크시 에러발생)", f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY018"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return ChkGrid;
        }
        #endregion
    }
}
