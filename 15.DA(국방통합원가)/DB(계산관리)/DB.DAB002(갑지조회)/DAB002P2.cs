#region DAB002P2 작성 정보
/*************************************************************/
// 단위업무명 : 경영노력보상율 & 이윤율/불확정
// 작 성 자 :   유재규
// 작 성 일 :   2013-06-13
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
using System.IO;

namespace DB.DAB002
{
    public partial class DAB002P2 : UIForm.FPCOMM2
    {
        #region 변수선언
        int iMS_PK_SEQ = 0;
        //int iDETAIL_SEQ = 0;
        string strMNUF_CODE = "";
        string strORDR_YEAR = "";
        string strFormId;
        #endregion

        #region DAB002P2
        public DAB002P2()
        {
            InitializeComponent();
        }
        #endregion


        #region DAB002P2()
        public DAB002P2(string MNUF_CODE, int MS_PK_SEQ, string ORDR_YEAR, string FormId)
        {
            InitializeComponent();
            
            
            strMNUF_CODE = MNUF_CODE;
            iMS_PK_SEQ = MS_PK_SEQ;   //순번
            strFormId = FormId;
            strORDR_YEAR = ORDR_YEAR;
        }
        #endregion

        #region DAB002P2_Load
        private void DAB002P2_Load(object sender, EventArgs e)
        {
             UIForm.Buttons.ReButton("010000010001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);
            //제출업체
            SystemBase.ComboMake.C1Combo(cboM_MNUF_CODE, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'D004', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'", 0);   //제출업체
            cboM_MNUF_CODE.SelectedValue = strMNUF_CODE;   //제출업체    
            txtH_ORDR_YEAR.Value = strORDR_YEAR; //요구연도

            //SearchExec(); 
            SystemBase.Validation.GroupBox_SearchViewValidation(groupBox1);
            this.Text = SystemBase.Base.GetMenuTree(strFormId) + " > 이윤율등록"; 
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            try
            {
                this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
                //SystemBase.Validation.GroupBox_Setting(groupBox1);

                string strSql = " usp_DAB002P2  ";
                strSql += "  @pTYPE = 'S1'";
                strSql += ", @pCO_CD =  '" + SystemBase.Base.gstrCOMCD + "'";
                strSql += ", @pMS_PK_SEQ =  " + iMS_PK_SEQ;

                UIForm.FPMake.grdCommSheet(fpSpread2, strSql, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, false, 0, 0);

                //for (int i = 0; i < fpSpread2.Sheets[0].Rows.Count; i++)
                //{
                //    fpSpread2.Sheets[0].RowHeader.Cells[i, 0].Text = "U";
                //}

                strSql = " usp_DAB002P2  ";
                strSql += "  @pTYPE = 'S2'";
                strSql += ", @pCO_CD =  '" + SystemBase.Base.gstrCOMCD + "'";
                strSql += ", @pMS_PK_SEQ =  " + iMS_PK_SEQ;

                UIForm.FPMake.grdCommSheet(fpSpread1, strSql, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
                fpSpread1.Sheets[0].SetColumnMerge(SystemBase.Base.GridHeadIndex(GHIdx1, "비목구분"), FarPoint.Win.Spread.Model.MergePolicy.Always);
                fpSpread1.Sheets[0].Columns[SystemBase.Base.GridHeadIndex(GHIdx1, "불확정여부")].Locked = true;

                //for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                //{
                //    fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text = "U";
                //}

                if ((fpSpread2.Sheets[0].Rows.Count == 0) && (fpSpread1.Sheets[0].Rows.Count == 0))
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn("SY014"), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                }


                this.Cursor = System.Windows.Forms.Cursors.Default;
            }
            catch (Exception f)
            {
                this.Cursor = System.Windows.Forms.Cursors.Default;
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region SaveExec() 폼에 입력된 데이타 저장 로직
        protected override void SaveExec()
        {           

            if (SaveCheck() == false) return;
 
            DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("SY048"), SystemBase.Base.MessageRtn("Z0004"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dsMsg == DialogResult.Yes)
            {
                //if (Master_Save() == false) return;  // 마스터 저장실패시 리턴처리
                string strSql = "";
                string ERRCode = "OK", MSGCode = "SY067";   // 에러코드는  OK처리 마스터만 저장할경우도 같이 처리
                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);
                DataSet ds = null;
                this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

                try
                {
                    strSql = " usp_DAB002P2 ";
                    strSql += "  @pTYPE = '" + "D1" + "'";
                    strSql += ", @pMS_PK_SEQ   = " + iMS_PK_SEQ; ;

                    ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                    ERRCode = ds.Tables[0].Rows[0][0].ToString();
                    MSGCode = ds.Tables[0].Rows[0][1].ToString();

                    if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }

                    strSql = " usp_DAB002P2 ";
                    strSql += "  @pTYPE = '" + "D2" + "'";
                    strSql += ", @pMS_PK_SEQ   = " + iMS_PK_SEQ; ;

                    ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                    ERRCode = ds.Tables[0].Rows[0][0].ToString();
                    MSGCode = ds.Tables[0].Rows[0][1].ToString();

                    if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	 

                    for (int i = 0; i < fpSpread2.Sheets[0].Rows.Count; i++)
                    {

                        strSql = " usp_DAB002P2 ";
                        strSql += "  @pTYPE = '" + "U1" + "'";
                        strSql += ", @pMS_PK_SEQ   = " + iMS_PK_SEQ;
                        strSql += ", @pCOMM_CODE   = '" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "평가항목코드")].Text.ToString() + "'";
                        strSql += ", @pREWARD_RATE = " + SystemBase.Validation.Decimal_Data(fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "보상율")].Text, ",");

                        ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                        ERRCode = ds.Tables[0].Rows[0][0].ToString();
                        MSGCode = ds.Tables[0].Rows[0][1].ToString();

                        if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }

                    }


                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {
                        string strUNCERTAIN_YN = "N"; if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "불확정여부")].Text.ToString() == "True") strUNCERTAIN_YN = "Y";

                        strSql = " usp_DAB002P2 ";
                        strSql += "  @pTYPE = '" + "U2" + "'";
                        strSql += ", @pMS_PK_SEQ    = " + iMS_PK_SEQ;
                        strSql += ", @pCOMM_CODE    = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "비목구분코드")].Text.ToString() + "'";
                        strSql += ", @pREWARD_RATE  = " + SystemBase.Validation.Decimal_Data(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "이윤율")].Text, ",");
                        strSql += ", @pUNCERTAIN_YN = '" + strUNCERTAIN_YN  + "'";         

                        ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                        ERRCode = ds.Tables[0].Rows[0][0].ToString();
                        MSGCode = ds.Tables[0].Rows[0][1].ToString();

                        if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }
                      
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
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (ERRCode == "ER")
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
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

                Status = FPGrid_SaveCheck2(fpSpread2, this.Name, "fpSpread2", true);
                if (Status == false)   // 에러상태 : 바로 리턴
                {
                    chk = false;
                    return chk;
                }
                else if (Status == true)   // 수정상태 : 다음 그리드도 체크해야 하므로 저장값 가지고 있음..
                {
                    SaveRow++;
                }


                Status = FPGrid_SaveCheck2(fpSpread1, this.Name, "fpSpread1", true);
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

        private void DAB002P2_Shown(object sender, EventArgs e)
        {
            SearchExec(); 
        }

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
