#region DAA001 작성 정보
/*************************************************************/
// 단위업무명 : 원가제출자료 등록
// 작 성 자 :   유재규
// 작 성 일 :   2013-06-17
// 작성내용 :   
// 수 정 일 :   
// 수 정 자 :   
// 수정내용 :   
// 비    고 : 원가 제출자료 등록 (팝업 사용 Detail 등록)
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

namespace DA.DAA001
{
    public partial class DAA001 : UIForm.FPCOMM2_2
    {
        #region 변수선언
        string strFpspread1 = "N";
        string strFpspread2 = "N";
        int PreRow = -1;       // SelectionChanged 시에 동일 Row에서 데이타변환 처리 안하도록 하기 위함.
        int FpGrid1_Row = 0;
        int FpGrid1_Col = 0;
        int PreSave_Rows = 0;
        #endregion

        #region DAA001
        public DAA001()
        {
            InitializeComponent();
        }
        #endregion

        #region DAA001_Load
        private void DAA001_Load(object sender, EventArgs e)
        {
            SystemBase.Base.gstrFromLoading = "N";
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            //제출업체
            SystemBase.ComboMake.C1Combo(cboH_MNUF_CODE, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'D004', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'", 0);   //제출업체

            txtH_ORDR_YEAR.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("")).ToString().Substring(0, 4);

            Master_ReSet();
            Detail_ReSet();
            
            SystemBase.Base.gstrFromLoading = "Y";
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            try
            {
                Master_ReSet();
                Detail_ReSet();
                PreRow = -1;
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
                        MessageBox.Show("행 복사할 그리드를 선택 하시기 바랍니다.", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning); //데이터 조회 중 오류가 발생하였습니다.
                        return;
                    }

                    if (strFpspread1 == "Y")
                    {
                        if (fpSpread2.Sheets[0].RowHeader.Cells[fpSpread2.Sheets[0].ActiveRowIndex, 0].Text == "D")
                        {
                            return;
                        }
                        if (fpSpread1.Sheets[0].ActiveRowIndex < 0) return;
                        UIForm.FPMake.RowCopy(fpSpread1);
                        fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "순번2")].Text = "";
                    }
                    else if (strFpspread2 == "Y")
                    {
                        UIForm.FPMake.RowCopy(fpSpread2);
                        fpSpread2.Sheets[0].Cells[fpSpread2.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx2, "순번")].Text = "";
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
                    if (strFpspread1 == "Y")
                    {
                        return;
                    }

                    UIForm.FPMake.RowInsert(fpSpread2);
                    fpSpread2.Sheets[0].Cells[fpSpread2.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx2, "제출업체")].Text = cboH_MNUF_CODE.SelectedValue.ToString();
                    fpSpread2.Sheets[0].Cells[fpSpread2.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx2, "지시연도")].Text = Convert.ToDateTime(SystemBase.Base.ServerTime("")).ToString().Substring(0, 4);
                    fpSpread2.Sheets[0].Cells[fpSpread2.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx2, "차수")].Text = "00";
                    fpSpread2.Sheets[0].Cells[fpSpread2.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx2, "조달업체")].Value = cboH_MNUF_CODE.SelectedValue.ToString();
                    fpSpread2.Sheets[0].Cells[fpSpread2.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx2, "기준년월")].Text = Convert.ToDateTime(SystemBase.Base.ServerTime("")).ToString().Substring(0, 7);
                    fpSpread2.Sheets[0].Cells[fpSpread2.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx2, "추출시작월")].Text = "1900-01";
                    fpSpread2.Sheets[0].Cells[fpSpread2.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx2, "추출종료월")].Text = Convert.ToDateTime(SystemBase.Base.ServerTime("")).ToString().Substring(0, 7);


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
                        MessageBox.Show(SystemBase.Base.MessageRtn("SY020", "그리드행 삭제"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning); //데이터 조회 중 오류가 발생하였습니다.
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

                    string strSql = " usp_DAA001  ";
                    strSql += "  @pTYPE = 'S1'";
                    strSql += ", @pMNUF_CODE = '" + cboH_MNUF_CODE.SelectedValue + "' ";
                    strSql += ", @pORDR_YEAR = '" + txtH_ORDR_YEAR.Text + "' ";
                    strSql += ", @pDCSN_NUMB = '" + txtDCSN_NUMB.Text + "' ";
                    strSql += ", @pCALC_DEGR = '" + txtCAL_C_DEGR.Text + "' ";
                    
                    UIForm.FPMake.grdCommSheet(fpSpread2, strSql, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, true, 0, 0);

                    Detail_ReSet();

                    this.Cursor = System.Windows.Forms.Cursors.Default;
                }

                PreRow = -1;
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Cursor = System.Windows.Forms.Cursors.Default;
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
                    SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                    SqlCommand cmd = dbConn.CreateCommand();
                    SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                    string strHead = ""; string strGbn = "";
                    string ERRCode = "OK", MSGCode = "SY067";   // 에러코드는  OK처리 마스터만 저장할경우도 같이 처리
                    this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

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
                                    case "U": strGbn = "U1"; break;  // UPDATE 및 INSERT 동일 처리
                                    case "I": strGbn = "I1"; break;  // UPDATE 및 INSERT 동일 처리
                                    default: strGbn = ""; break;
                                }

                                string strSql = " usp_DAA001 ";
                                strSql += "  @pTYPE = '" + strGbn + "'";
                                strSql += ", @pSTD_SEQ = '" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "순번")].Text.ToString() + "'";
                                strSql += ", @pMNUF_CODE = '" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "제출업체")].Text.ToString() + "'";
                                strSql += ", @pORDR_YEAR = '" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "지시연도")].Text.ToString() + "'";
                                strSql += ", @pDCSN_NUMB = '" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "판단번호")].Text.ToString() + "'";
                                strSql += ", @pCALC_DEGR = '" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "차수")].Text.ToString() + "'";

                                strSql += ", @pCTMF_CODE = '" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "조달업체")].Value.ToString() + "'";
                                strSql += ", @pDPRT_CODE = '" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "구매부서")].Value.ToString() + "'";
                                strSql += ", @pRPST_ITNM = '" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "대표품명")].Text.ToString() + "'";
                                strSql += ", @pSBMTR_CHRG_PURPS = '" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "제출용도")].Value.ToString() + "'";
                                strSql += ", @pSTD_YRMON = '" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "기준연월")].Value.ToString().Replace("-", "") + "'";
                                strSql += ", @pIM_FROM_MON = '" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "추출시작월")].Value.ToString().Replace("-", "") + "'";
                                strSql += ", @pIM_TO_MON = '" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "추출종료월")].Value.ToString().Replace("-", "") + "'";
                                strSql += ", @pUP_ID ='" + SystemBase.Base.gstrUserID + "' ";                                  //사용자

                                DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                                ERRCode = ds.Tables[0].Rows[0][0].ToString();
                                MSGCode = ds.Tables[0].Rows[0][1].ToString();

                                if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }
                            }
                        }


                        for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                        {
                            strHead = fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text;
                            strGbn = "";
                            if (strHead.Length > 0)
                            {
                                switch (strHead)
                                {
                                    case "D": strGbn = "D2"; break;
                                    case "U": strGbn = "U2"; break;  // UPDATE 및 INSERT 동일 처리
                                    case "I": strGbn = "I2"; break;  // UPDATE 및 INSERT 동일 처리
                                    default: strGbn = ""; break;
                                }

                                string strSql = " usp_DAA001 ";
                                strSql += "  @pTYPE = '" + strGbn + "'";
                                strSql += ", @pSTD_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "순번")].Text.ToString() + "' ";
                                strSql += ", @pSTD_DTL_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "순번2")].Text.ToString() + "' ";
                                //strSql += ", @pMNUF_CODE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "제출업체")].Text.ToString() + "'";
                                //strSql += ", @pORDR_YEAR = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "지시년도")].Text.ToString() + "'";
                                //strSql += ", @pDCSN_NUMB = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "판단번호")].Text.ToString() + "'";
                                //strSql += ", @pCALC_DEGR = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Text.ToString() + "'";

                                strSql += ", @pNIIN = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "재고번호")].Text.ToString() + "'";
                                strSql += ", @pUNIT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].Value.ToString() + "'";
                                strSql += ", @pDMST_ITNB = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "항목")].Text.ToString() + "'";
                                strSql += ", @pITEM_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "ERP품목코드")].Text.ToString() + "'";

                                strSql += ", @pPROJECT_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text.ToString() + "'";

                                strSql += ", @pBOM_INFO_SBMT_STD_CNFMTN_HLNO = " + SystemBase.Validation.Decimal_Data(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "BOM정보제출기준")].Text.ToString(), ",") + "";
                                strSql += ", @pCSTACC_APLY_QTY = " + SystemBase.Validation.Decimal_Data(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "원가계산적용수량")].Text.ToString(), ",") + "";
                                strSql += ", @pCSTACC_APLY_QTY_STD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "원가계산적용수량기준")].Value.ToString() + "'";
                                strSql += ", @pPRJCLS_DVS = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "양산구분")].Value.ToString() + "'";
                                strSql += ", @pDNNP_APPN = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "방산물자 지정여부")].Value.ToString() + "'";

                                strSql += ", @pUP_ID ='" + SystemBase.Base.gstrUserID + "' ";                                  //사용자

                                DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                                ERRCode = ds.Tables[0].Rows[0][0].ToString();
                                MSGCode = ds.Tables[0].Rows[0][1].ToString();

                                if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }
                            }
                        }
                        //Trans.Commit();
                    }
                    catch (Exception f)
                    {
                        SystemBase.Loggers.Log(this.Name, f.ToString());
                        Trans.Rollback();
                        MSGCode = "SY002";	//에러가 발생하여 데이터 처리가 취소되었습니다.
                    }

                    Trans.Commit();

                Exit:
                    dbConn.Close();

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
                    else
                    {
                        MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            this.Cursor = System.Windows.Forms.Cursors.Default;
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

        #region Detail_List
        private void Detail_List(int intRow)
        {
            try
            {
                //접수현황 상태가 "D"이면 계약품목 그리드 Lock처리
                if (PreSave_Rows > fpSpread2.ActiveSheet.RowCount)
                {
                    PreSave_Rows = fpSpread2.ActiveSheet.RowCount;  //저장후 RowCount 가짐...
                    return;  // 삭제가 있을시는 Row 수가 달라짐...하여 저장전과 비교처리
                }

                int iSTD_SEQ = fpSpread2.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx2, "순번")].Value == null ? 0 : Convert.ToInt32(fpSpread2.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx2, "순번")].Text.ToString()); ;
                
                string strSql = " usp_DAA001  ";
                strSql += "  @pTYPE = 'S2'";
                strSql += ", @pSTD_SEQ = '" + iSTD_SEQ + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread1, strSql, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);

                fpSpread2.Focus();  // 마스터 선택 Row 포커스 처리
                fpSpread2.ActiveSheet.SetActiveCell(FpGrid1_Row, FpGrid1_Col);

                //접수현황이 "D"이면 계약품목 작업불가
                if (fpSpread2.Sheets[0].RowHeader.Cells[FpGrid1_Row, 0].Text == "D")
                {
                    fpSpread1.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.ReadOnly;
                }
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
                G2Etc[SystemBase.Base.GridHeadIndex(GHIdx2, "조달업체")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'D006', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 1);
                G2Etc[SystemBase.Base.GridHeadIndex(GHIdx2, "구매부서")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'D007', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 1);
                G2Etc[SystemBase.Base.GridHeadIndex(GHIdx2, "제출용도")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'D008', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 1);
                
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
                G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "원가계산적용수량기준")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'D022', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 1);
                G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "양산구분")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'D011', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 1);
                G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "방산물자 지정여부")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'B029', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 1);
                
                UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region SaveCheck 저장전 자료 여부 체크
        private bool SaveCheck()
        {
            try
            {
                bool chk = true;

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
                    strORDR_YEAR = fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "지시연도")].Text.ToString();
                    strDCSN_NUMB = fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "판단번호")].Text.ToString();
                    strCALC_DEGR = fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "차수")].Text.ToString();
                    strDPRT_CODE = fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "구매부서")].Value.ToString();
                    strPRESENT_USE = fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "제출용도")].Value.ToString();
                    strSTD_YRMON = fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "기준연월")].Value.ToString().Replace("-", "");
                    
                    for (int j = i + 1; j < fpSpread2.Sheets[0].Rows.Count; j++)
                    {
                        if (strORDR_YEAR == fpSpread2.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx2, "지시연도")].Text.ToString() &&
                            strDCSN_NUMB == fpSpread2.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx2, "판단번호")].Text.ToString() &&
                            strCALC_DEGR == fpSpread2.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx2, "차수")].Text.ToString() && 
                            strDPRT_CODE == fpSpread2.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx2, "구매부서")].Value.ToString() &&
                            strPRESENT_USE == fpSpread2.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx2, "제출용도")].Value.ToString() &&
                            strSTD_YRMON == fpSpread2.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx2, "기준연월")].Value.ToString().Replace("-", ""))
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
                if (fpSpread2.Sheets[0].Rows.Count > 0)
                {
                    if ((e.Column == SystemBase.Base.GridHeadIndex(GHIdx2, "조달업체")) ||
                        (e.Column == SystemBase.Base.GridHeadIndex(GHIdx2, "구매부서")) ||
                        (e.Column == SystemBase.Base.GridHeadIndex(GHIdx2, "제출용도")))
                    {
                        FpGrid1_Row = e.Row;  // 마스터 선택 Row               

                        if (FpGrid1_Row < 0) return;
                        if (PreRow == FpGrid1_Row && PreRow != -1) return;  //현 Row에서 컬럼이동시는 조회 안되게

                        Detail_List(e.Row);
                        PreRow = e.Row;
                    }
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
                //Value_Selected(e, null, null);
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

        #region 수주참조 팝업
        private void btnSoPop_Click(object sender, EventArgs e)
        {
            try
            {
                if (fpSpread2.Sheets[0].Rows.Count == 0 || fpSpread2.Sheets[0].ActiveRowIndex < 0)
                {
                    MessageBox.Show("수주 추가할 마스터 그리드를 선택 하시기 바랍니다.", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning); //데이터 조회 중 오류가 발생하였습니다.
                    return;
                }
                if (fpSpread2.Sheets[0].GetSelection(0) == null)
                {
                    return;
                }
                if (fpSpread2.Sheets[0].RowHeader.Cells[fpSpread2.Sheets[0].ActiveRowIndex, 0].Text == "D")
                {
                    return;
                }
                if (fpSpread2.Sheets[0].RowHeader.Cells[fpSpread2.Sheets[0].ActiveRowIndex, 0].Text == "I")
                {
                    MessageBox.Show("추가행 저장 후 자료등록 하시기바랍니다.", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning); //데이터 조회 중 오류가 발생하였습니다.
                    return;
                }

                string strORDER_YEAR = "";
                string strDCSN_NUMB = "";
                string strStd_Seq = "";

                strORDER_YEAR = fpSpread2.Sheets[0].Cells[fpSpread2.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx2, "지시연도")].Text;
                strDCSN_NUMB = fpSpread2.Sheets[0].Cells[fpSpread2.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx2, "판단번호")].Text;
                strStd_Seq = fpSpread2.Sheets[0].Cells[fpSpread2.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx2, "순번")].Text;


                DAA001P1 pu = new DAA001P1(strORDER_YEAR, strDCSN_NUMB);
                pu.Width = 1000;
                pu.Height = 500;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    bool strAddFlag = true;
                    for (int i = 0; i < pu.DT.Rows.Count; i++)
                    {
                        strAddFlag = true;
                        for (int j = 0; j < fpSpread1.Sheets[0].Rows.Count; j++)
                        {
                            if (fpSpread1.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "재고번호")].Text == pu.DT.Rows[i]["NIIN"].ToString() &&
                                fpSpread1.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text == pu.DT.Rows[i]["PROJECT_NO"].ToString())
                            {
                                strAddFlag = false;
                            }
                        }
                        if (strAddFlag == true)
                        {
                            UIForm.FPMake.RowInsert(fpSpread1);
                            fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "순번")].Text = strStd_Seq;
                            fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "재고번호")].Text = pu.DT.Rows[i]["NIIN"].ToString();
                            fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].Text = pu.DT.Rows[i]["ITEM_UNIT"].ToString();
                            fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "항목")].Text = "***";
                            fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "ERP품목코드")].Text = pu.DT.Rows[i]["ITEM_CD"].ToString();
                            fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "품명")].Text = pu.DT.Rows[i]["ITEM_NM"].ToString();
                            fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text = pu.DT.Rows[i]["PROJECT_NO"].ToString();
                            fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "BOM정보제출기준")].Text = pu.DT.Rows[i]["SO_QTY"].ToString();
                            fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "원가계산적용수량")].Text = pu.DT.Rows[i]["SO_QTY"].ToString();
                            fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "원가계산적용수량기준")].Value = "1";
                            fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "양산구분")].Value = "BOM";
                            fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "방산물자 지정여부")].Value = "N";
                        }
                    }
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion
    }
}
