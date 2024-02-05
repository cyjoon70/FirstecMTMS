#region DAA005 작성 정보
/*************************************************************/
// 단위업무명 : 기준정보등록
// 작 성 자 :   김창진
// 작 성 일 :   2013-09-12
// 작성내용 :   
// 수 정 일 :   
// 수 정 자 :   
// 수정내용 :   
// 비    고 : 
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
using WNDW;
using System.IO;

namespace DA.DAA005
{
    public partial class DAA005 : UIForm.FPCOMM2_2
    {
        #region 변수선언
        string strFpspread1 = "N";
        string strFpspread2 = "N";
        int PreRow = -1;       // SelectionChanged 시에 동일 Row에서 데이타변환 처리 안하도록 하기 위함.
        int FpGrid1_Row = 0;
        int FpGrid1_Col = 0;
        int PreSave_Rows = 0;
        #endregion

        #region DAA005
        public DAA005()
        {
            InitializeComponent();
        }
        #endregion

        #region DAA005_Load
        private void DAA005_Load(object sender, EventArgs e)
        {
            SystemBase.Base.gstrFromLoading = "N";
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            //제출업체
            SystemBase.ComboMake.C1Combo(cboEND_YN, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'D036', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'", 0);   //완료여부
            Detail_ReSet();

            G2Etc[SystemBase.Base.GridHeadIndex(GHIdx2, "완료여부")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'D036', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 1);

            UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, true, 0, 0);

            SystemBase.Base.gstrFromLoading = "Y";
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            try
            {
                Detail_ReSet();
                fpSpread2.Sheets[0].Rows.Count = 0;
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
                if (strFpspread1 == "N")
                {
                    MessageBox.Show("행 복사할 그리드를 선택 하시기 바랍니다.", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning); //데이터 조회 중 오류가 발생하였습니다.
                    return;
                }

                if (strFpspread1 == "Y")
                {
                    if (fpSpread1.Sheets[0].RowHeader.Cells[fpSpread1.Sheets[0].ActiveRowIndex, 0].Text == "D")
                    {
                        return;
                    }

                    if (fpSpread1.Sheets[0].ActiveRowIndex < 0) return;

                    UIForm.FPMake.RowCopy(fpSpread1);

                    fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "순번")].Text = "";
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
                if (fpSpread2.Sheets[0].Rows.Count > 0)
                {
                    int intRow = fpSpread2.ActiveSheet.ActiveRowIndex;  // 마스터 선택 Row

                    if (intRow < 0)
                    {
                        return;
                    }
                    else
                    {
                        
                        UIForm.FPMake.RowInsert(fpSpread1);

                        string strPROJET_NO = fpSpread2.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx2, "프로젝트번호")].Text.ToString();
                        string strDCSN_NUMB = fpSpread2.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx2, "판단번호")].Text.ToString();

                        string strSql = " usp_DAA005  ";
                        strSql += "  @pTYPE = 'S3'";
                        strSql += ", @pPROJECT_NO = '" + strPROJET_NO + "' ";
                        strSql += ", @pDCSN_NUMB = '" + strDCSN_NUMB + "' ";

                        DataSet ds = SystemBase.DbOpen.NoTranDataSet(strSql);
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "제출업체")].Value = ds.Tables[0].Rows[0]["MNUF_CODE"].ToString();
                            fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "지시연도")].Text = ds.Tables[0].Rows[0]["ORDR_YEAR"].ToString();
                            fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "판단번호")].Text = ds.Tables[0].Rows[0]["DCSN_NUMB"].ToString();
                            fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Text = ds.Tables[0].Rows[0]["CALC_DEGR"].ToString();
                            fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "조달업체")].Value = ds.Tables[0].Rows[0]["CTMF_CODE"].ToString();
                            fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "구매부서")].Value = ds.Tables[0].Rows[0]["DPRT_CODE"].ToString();
                            fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "대표품명")].Value = ds.Tables[0].Rows[0]["CONTRACT_NAME"].ToString();
                            fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "제출용도")].Value = ds.Tables[0].Rows[0]["SBMTR_CHRG_PURPS"].ToString();
                            fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "기준연월")].Text = Convert.ToDateTime(SystemBase.Base.ServerTime("")).ToString().Substring(0, 7);
                            fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "추출시작월")].Text = ds.Tables[0].Rows[0]["IM_FROM_MON"].ToString(); ;
                            fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "추출종료월")].Text = Convert.ToDateTime(SystemBase.Base.ServerTime("")).ToString().Substring(0, 7);
                            fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "제출유무")].Text = "N";
                        }

                    }

                }
                else
                {
                    MessageBox.Show("프로젝트 조회후 행추가 하시기 바랍니다.", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning); //데이터 조회 중 오류가 발생하였습니다.
                    return;
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
                    if (strFpspread1 == "N")
                    {
                        MessageBox.Show(SystemBase.Base.MessageRtn("SY020", "그리드행 삭제"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning); //데이터 조회 중 오류가 발생하였습니다.
                        return;
                    }

                    if (strFpspread1 == "Y")
                    {
                        if (fpSpread1.Sheets[0].RowHeader.Cells[fpSpread1.Sheets[0].ActiveRowIndex, 0].Text == "D")
                        {
                            return;
                        }

                        UIForm.FPMake.RowRemove(fpSpread1);
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

                    string strSql = " usp_DAA005  ";
                    strSql += "  @pTYPE = 'S1'";
                    strSql += ", @pEND_YN = '" + cboEND_YN.SelectedValue + "' ";
                    strSql += ", @pPROJECT_NO = '" + txtPROJECT_NO.Text + "' ";
                    strSql += ", @pDCSN_NUMB = '" + txtDCSN_NUMB.Text + "' ";
                    
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
                PreSave_Rows = fpSpread1.ActiveSheet.RowCount;  //저장전 RowCount 가짐...

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
                                    case "U": strGbn = "U2"; break;  // UPDATE 및 INSERT 동일 처리
                                    default: strGbn = ""; break;
                                }

                                string strSql1 = " usp_DAA005 ";
                                strSql1 += "  @pTYPE = '" + strGbn + "'";
                                strSql1 += ", @pPROJECT_NO = '" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "프로젝트번호")].Text.ToString() + "'";
                                strSql1 += ", @pEND_YN = '" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "완료여부")].Value + "'";
                                strSql1 += ", @pUP_ID ='" + SystemBase.Base.gstrUserID + "' ";                                  //사용자

                                DataSet ds1 = SystemBase.DbOpen.TranDataSet(strSql1, dbConn, Trans);
                                ERRCode = ds1.Tables[0].Rows[0][0].ToString();
                                MSGCode = ds1.Tables[0].Rows[0][1].ToString();

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
                                    case "D": strGbn = "D1"; break;
                                    case "U": strGbn = "U1"; break;  // UPDATE 및 INSERT 동일 처리
                                    case "I": strGbn = "I1"; break;  // UPDATE 및 INSERT 동일 처리
                                    default: strGbn = ""; break;
                                }

                                string strSql = " usp_DAA005 ";
                                strSql += "  @pTYPE = '" + strGbn + "'";
                                strSql += ", @pSTD_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "순번")].Text.ToString() + "'";
                                strSql += ", @pMNUF_CODE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "제출업체")].Text.ToString() + "'";
                                strSql += ", @pORDR_YEAR = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "지시연도")].Text.ToString() + "'";
                                strSql += ", @pDCSN_NUMB = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "판단번호")].Text.ToString() + "'";
                                strSql += ", @pCALC_DEGR = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Text.ToString() + "'";
                                strSql += ", @pCTMF_CODE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "조달업체")].Value.ToString() + "'";
                                strSql += ", @pDPRT_CODE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "구매부서")].Value.ToString() + "'";
                                strSql += ", @pRPST_ITNM = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "대표품명")].Text.ToString() + "'";
                                strSql += ", @pSBMTR_CHRG_PURPS = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "제출용도")].Value.ToString() + "'";
                                strSql += ", @pSTD_YRMON = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "기준연월")].Value.ToString().Replace("-", "") + "'";
                                strSql += ", @pIM_FROM_MON = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "추출시작월")].Value.ToString().Replace("-", "") + "'";
                                strSql += ", @pIM_TO_MON = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "추출종료월")].Value.ToString().Replace("-", "") + "'";
                                strSql += ", @pUP_ID ='" + SystemBase.Base.gstrUserID + "' ";                                  //사용자

                                DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                                ERRCode = ds.Tables[0].Rows[0][0].ToString();
                                MSGCode = ds.Tables[0].Rows[0][1].ToString();

                                if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }
                            }
                        }
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

                string strPROJET_NO = fpSpread2.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx2, "프로젝트번호")].Text.ToString();
                string strDCSN_NUMB = fpSpread2.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx2, "판단번호")].Text.ToString();
                
                string strSql = " usp_DAA005  ";
                strSql += "  @pTYPE = 'S2'";
                strSql += ", @pPROJECT_NO = '" + strPROJET_NO + "' ";
                strSql += ", @pDCSN_NUMB = '" + strDCSN_NUMB + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread1, strSql, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);

                fpSpread2.Focus();  // 마스터 선택 Row 포커스 처리
                fpSpread2.ActiveSheet.SetActiveCell(FpGrid1_Row, FpGrid1_Col);

                //접수현황이 "D"이면 계약품목 작업불가
                if (fpSpread2.Sheets[0].RowHeader.Cells[FpGrid1_Row, 0].Text == "D")
                {
                    fpSpread1.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.ReadOnly;
                }

                //Detail Locking설정
                for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                {
                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "제출유무")].Text.ToString() == "Y")
                    {
                        UIForm.FPMake.grdReMake(fpSpread1, i,
                            SystemBase.Base.GridHeadIndex(GHIdx1, "추출시작월") + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "추출종료월") + "|3");
                    }
                    else
                    {
                        UIForm.FPMake.grdReMake(fpSpread1, i,
                            SystemBase.Base.GridHeadIndex(GHIdx1, "추출시작월") + "|1"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "추출종료월") + "|1");
                    }
                }

            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region Detail_ReSet
        private void Detail_ReSet()
        {
            try
            {
                G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "조달업체")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'D006', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 1);
                G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "구매부서")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'D007', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 1);
                G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "제출용도")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'D008', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 1);
                
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

                for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                {
                    strORDR_YEAR = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "지시연도")].Text.ToString();
                    strDCSN_NUMB = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "판단번호")].Text.ToString();
                    strCALC_DEGR = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Text.ToString();
                    strDPRT_CODE = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "구매부서")].Value.ToString();
                    strPRESENT_USE = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "제출용도")].Value.ToString();
                    strSTD_YRMON = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "기준연월")].Value.ToString().Replace("-", "");

                    for (int j = i + 1; j < fpSpread1.Sheets[0].Rows.Count; j++)
                    {
                        if (strORDR_YEAR == fpSpread1.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "지시연도")].Text.ToString() &&
                            strDCSN_NUMB == fpSpread1.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "판단번호")].Text.ToString() &&
                            strCALC_DEGR == fpSpread1.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Text.ToString() &&
                            strDPRT_CODE == fpSpread1.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "구매부서")].Value.ToString() &&
                            strPRESENT_USE == fpSpread1.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "제출용도")].Value.ToString() &&
                            strSTD_YRMON == fpSpread1.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "기준연월")].Value.ToString().Replace("-", ""))
                        {

                            MessageBox.Show(Convert.ToString(j + 1) + "번째 Row의 데이타 중복입니다.", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);

                            fpSpread1.Focus();
                            return false;
                        }
                    }

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

        #region 조회조건 팝업
        //프로젝트번호
        private void btnProjectNo_Click(object sender, EventArgs e)
        {
            
            try
            {
                WNDW007 pu = new WNDW007(txtPROJECT_NO.Text);
                pu.MaximizeBox = false;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;
                    txtPROJECT_NO.Text = Msgs[3].ToString();
                    txtPROJECT_NM.Value = Msgs[4].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "프로젝트번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        #endregion

        #region Text Changed
        //프로젝트번호
        private void txtPROJECT_NO_TextChanged(object sender, EventArgs e)
        {
            if (txtPROJECT_NO.Text != "")
            {
                txtPROJECT_NM.Value = SystemBase.Base.CodeName("PROJECT_NO", "PROJECT_NM", "S_SO_MASTER", txtPROJECT_NO.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
            }
            else
            {
                txtPROJECT_NM.Value = "";
            }
        }
        #endregion
    }
}
