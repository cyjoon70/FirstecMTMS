#region TabFPMake 작성 정보
/*************************************************************/
// 단위업무명 : Tab이 사용되는 폼에서 그리드에 대한 적용
// 작 성 자 :   전 성 표
// 작 성 일 :   2012-11-07
// 작성내용 :   
// 수 정 일 :
// 수 정 자 :
// 수정내용 :
// 비    고 : 
// 참    고 : 
/*************************************************************/
#endregion
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;
using FarPoint.Win.Spread;
using System.Text.RegularExpressions;

namespace UIForm
{
    public class TabFPMake
    {
        public static string[] TabG1Head1 = null;// 첫번째 Head Text
        public static string[] TabG1Head2 = null;// 두번째 Head Text
        public static string[] TabG1Head3 = null;// 세번째 Head Text
        public static int[] TabG1Width = null;// Cell 넓이
        public static string[] TabG1Align = null;// Cell 데이타 정렬방식
        public static string[] TabG1Type = null;// CellType 지정
        public static int[] TabG1Color = null;// Cell 색상 및 ReadOnly 설정(0:일반, 1:필수, 2:ReadOnly)
        public static string[] TabG1Etc = new string[100];// Mask 양식 등
        public static int[] TabG1SEQ = null;// 키
        public static int TabG1HeadCnt = 0;	// Head 수
        public static string[,] TabGHIdx1 = null;	// Grid Head 위치       

        #region FpSpread_Load (폼 ID별 그리드 디자인)
        public static void FpSpread_Load(string FormID, FarPoint.Win.Spread.FpSpread FpGrid, string[] TabG1Etc, string GridNM)
        {
            try
            {
                if (SystemBase.Base.ProgramWhere.Length > 0)
                {
                    string Query = " usp_BAA004 'S3',@PFORM_ID='" + FormID.ToString() + "', @PGRID_NAME='"+GridNM+"', @PIN_ID='" + SystemBase.Base.gstrUserID + "' ";
                    DataTable dt = SystemBase.DbOpen.TranDataTable(Query);
                    int G1RowCount = dt.Rows.Count + 1;

                    if (G1RowCount > 0)
                    {
                        TabG1Head1 = new string[G1RowCount];// 첫번째 Head Text
                        TabG1Head2 = new string[G1RowCount];// 두번째 Head Text
                        TabG1Head3 = new string[G1RowCount];// 세번째 Head Text
                        TabG1Width = new int[G1RowCount];// Cell 넓이
                        TabG1Align = new string[G1RowCount];// Cell 데이타 정렬방식
                        TabG1Type = new string[G1RowCount];// CellType 지정
                        TabG1Color = new int[G1RowCount];// Cell 색상 및 ReadOnly 설정(0:일반, 1:필수, 2:ReadOnly)

                        TabG1SEQ = new int[G1RowCount];// 키

                        //G1Etc		= new string[G1RowCount];
                        TabG1HeadCnt = Convert.ToInt32(dt.Rows[0][0].ToString());

                        //####################1번째 숨김필드 정의######################
                        TabG1Head1[0] = "";
                        if (Convert.ToInt32(dt.Rows[0][0].ToString()) >= 1)
                            TabG1Head2[0] = "";
                        if (Convert.ToInt32(dt.Rows[0][0].ToString()) == 3)
                            TabG1Head3[0] = "";
                        TabG1Width[0] = 0;
                        TabG1Align[0] = "";
                        TabG1Type[0] = "";
                        TabG1Color[0] = 0;
                        TabG1Etc[0] = "";
                        //####################1번째 숨김필드 정의######################

                        //####################그리드 Head 순번######################
                        TabGHIdx1 = new string[G1RowCount - 1, 2];	// 그리드 Head Index 변수 길이
                        //string OldHeadName = null;
                        int OldHeadNameCount = 1;
                        //####################그리드 Head 순번######################
                        for (int i = 1; i < G1RowCount; i++)
                        {
                            TabG1Head1[i] = dt.Rows[i - 1][1].ToString();
                            if (Convert.ToInt32(dt.Rows[i - 1][0].ToString()) >= 1)
                                TabG1Head2[i] = dt.Rows[i - 1][2].ToString();
                            if (Convert.ToInt32(dt.Rows[i - 1][0].ToString()) == 3)
                                TabG1Head3[i] = dt.Rows[i - 1][3].ToString();

                            TabG1Width[i] = Convert.ToInt32(dt.Rows[i - 1][4].ToString());
                            TabG1Align[i] = dt.Rows[i - 1][5].ToString();
                            TabG1Type[i] = dt.Rows[i - 1][6].ToString();
                            TabG1Color[i] = Convert.ToInt32(dt.Rows[i - 1][7].ToString());

                            if (TabG1Etc[i] == null)
                                TabG1Etc[i] = dt.Rows[i - 1][8].ToString();

                            TabG1SEQ[i] = Convert.ToInt32(dt.Rows[i - 1][9].ToString());


                            //####################그리드 Head 순번######################
                            OldHeadNameCount = 1;
                            TabGHIdx1[0, 0] = dt.Rows[0][1].ToString().ToUpper();
                            for (int k = 0; k < i - 1; k++)
                            {
                                if (dt.Rows[i - 1][1].ToString().ToUpper() == TabGHIdx1[k, 0].ToUpper())
                                {
                                    OldHeadNameCount++;
                                }
                                else if (TabGHIdx1[k, 0].ToUpper().LastIndexOf("_") > 0 && dt.Rows[i - 1][1].ToString().ToUpper() == TabGHIdx1[k, 0].ToUpper().Substring(0, TabGHIdx1[k, 0].ToUpper().LastIndexOf("_")))
                                {
                                    OldHeadNameCount++;
                                }

                            }

                            if (OldHeadNameCount > 1)
                            {
                                TabGHIdx1[i - 1, 0] = dt.Rows[i - 1][1].ToString().ToUpper() + "_" + OldHeadNameCount.ToString();	// 그리드 Head명
                            }
                            else
                            {
                                TabGHIdx1[i - 1, 0] = dt.Rows[i - 1][1].ToString().ToUpper();	// 그리드 Head명
                            }
                            TabGHIdx1[i - 1, 1] = Convert.ToString(i);			// 그리드 Head 위치
                            //####################그리드 Head 순번######################

                        }
                        UIForm.FPMake.grdCommSheet(FpGrid, null, TabG1Head1, TabG1Head2, TabG1Head3, TabG1Width, TabG1Align, TabG1Type, TabG1Color, TabG1Etc, TabG1HeadCnt, false, false, 0, 0);
                    }

                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(FormID, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY008", "화면 생성"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region FpSpread_RowInsExec (행추가 그리드 등록 플레그 등록)
        public static void FpSpread_RowInsExec(string FormID, FarPoint.Win.Spread.FpSpread FpGrid)
        {
            try
            {
                UIForm.FPMake.RowInsert(FpGrid);                
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(FormID, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY008", "행추가"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }      
        }
        #endregion

        #region FpSpread_CancelExec (행취소)
        public static void FpSpread_CancelExec(string FormID, FarPoint.Win.Spread.FpSpread FpGrid)
        {
            try
            {
                int GridSelectRow = 0;
                int GridSelectRowCount = 0;

                if (FpGrid.ActiveSheet.GetSelection(0) == null || FpGrid.ActiveSheet.ActiveRowIndex < 0) return;

                if (FpGrid.ActiveSheet.ActiveRowIndex.ToString() != "")
                {
                    GridSelectRow = FpGrid.ActiveSheet.ActiveRowIndex;
                    int Row = FpGrid.ActiveSheet.ActiveRowIndex;
                    int Col = FpGrid.ActiveSheet.ActiveColumnIndex;

                    GridSelectRowCount = 1;
                    if (FpGrid.ActiveSheet.GetCellType(Row, Col).ToString() != "ComboBoxCellType" && FpGrid.ActiveSheet.GetCellType(Row, Col).ToString() != "CheckBoxCellType")
                    {
                        GridSelectRowCount = FpGrid.Sheets[0].GetSelection(0).RowCount;
                    }
                }
                UIForm.FPMake.Cancel(FpGrid, GridSelectRow, GridSelectRowCount);
                
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(FormID, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY008", "행취소"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        #endregion

        #region FpSpread_RowRemove (행삭제 그리드 삭제 플레그 등록)
        public static void FpSpread_RowRemove(string FormID, FarPoint.Win.Spread.FpSpread FpGrid)
        {
            try
            {
                int BeforeRow = 0;
                int Col = 0;
                if (FpGrid.ActiveSheet.GetSelection(0) == null)
                {
                    BeforeRow = FpGrid.ActiveSheet.ActiveRowIndex;
                    Col = FpGrid.ActiveSheet.ActiveColumnIndex; ;
                }
                else
                {
                    BeforeRow = FpGrid.ActiveSheet.GetSelection(0).Row;
                    Col = FpGrid.ActiveSheet.GetSelection(0).Column;
                }
                int BeforeRowCount = 1;
                if (FpGrid.ActiveSheet.GetCellType(BeforeRow, Col).ToString() != "ComboBoxCellType" && FpGrid.ActiveSheet.GetCellType(BeforeRow, Col).ToString() != "CheckBoxCellType")
                {
                    if (FpGrid.Sheets[0].GetSelection(0) == null)
                        BeforeRowCount = 1;
                    else
                        BeforeRowCount = FpGrid.Sheets[0].GetSelection(0).RowCount;
                }

                int TmpRow = 0;
                if (FpGrid.ActiveSheet.GetSelection(0) == null)
                    TmpRow = FpGrid.ActiveSheet.ActiveRowIndex;
                else
                    TmpRow = FpGrid.ActiveSheet.GetSelection(0).Row;

                for (int i = BeforeRow; i < BeforeRow + BeforeRowCount; i++)
                {
                    if (FpGrid.Sheets[0].RowHeader.Cells[TmpRow, 0].Text == "I")
                    {
                        FpGrid.Sheets[0].Rows.Remove(TmpRow, 1);
                    }
                    else if (FpGrid.Sheets[0].RowHeader.Cells[TmpRow, 0].Text == "D")
                        FpGrid.Sheets[0].RowHeader.Cells[TmpRow, 0].Text = "";
                    else
                    {
                        FpGrid.Sheets[0].RowHeader.Cells[TmpRow, 0].Text = "D";
                        FpGrid.Sheets[0].RowHeader.Rows[TmpRow].BackColor = SystemBase.Base.Color_Delete;
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

        #region FpSpread_RowCopy (행복사)
        public static void FpSpread_RowCopy(string FormID, FarPoint.Win.Spread.FpSpread FpGrid)
        {
            try
            {
                if (FpGrid.Sheets[0].Rows.Count > 0)
                {
                    int SelectedRow = 0;

                    if (FpGrid.ActiveSheet.GetSelection(0) == null)
                    {
                        SelectedRow = FpGrid.ActiveSheet.ActiveRowIndex;
                    }
                    else
                    {
                        SelectedRow = FpGrid.ActiveSheet.GetSelection(0).Row;
                    }

                    UIForm.FPMake.RowInsert(FpGrid);

                    for (int i = 0; i < FpGrid.Sheets[0].Columns.Count; i++)
                    {
                        FpGrid.Sheets[0].Cells[SelectedRow + 1, i].Value = FpGrid.Sheets[0].Cells[SelectedRow, i].Value;
                    }
                }
                else
                {
                    MessageBox.Show("복사할 데이타가 없습니다.", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                //				}
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log("RowCopy (Row 복사 실패)", f.ToString());
                MessageBox.Show("Row 복사 실패", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region FpSpread_CellHeaderClick (체크박스 그리드 헤더클릭)
        public static void FpSpread_CellHeaderClick(FarPoint.Win.Spread.FpSpread FpGrid, bool ColumnHeader, int Column)
        {
            if (FpGrid.Sheets[0].Rows.Count > 0)
            {
                if (FpGrid.Sheets[0].ColumnHeader.Cells.Get(0, Column).CellType != null)
                {
                    if (ColumnHeader == true)
                    {
                        if (FpGrid.Sheets[0].ColumnHeader.Cells[0, Column].Text == "True")
                        {
                            FpGrid.Sheets[0].ColumnHeader.Cells.Get(0, Column).Value = false;
                            for (int i = 0; i < FpGrid.Sheets[0].Rows.Count; i++)
                            {
                                if (FpGrid.Sheets[0].Cells[i, Column].Locked == false)
                                {
                                    FpGrid.Sheets[0].Cells[i, Column].Value = false;
                                }
                                if (FpGrid.Sheets[0].RowHeader.Cells[i, 0].Text != "I")
                                {
                                    FpGrid.Sheets[0].RowHeader.Cells[i, 0].Text = "U";
                                }
                            }
                        }
                        else
                        {
                            FpGrid.Sheets[0].ColumnHeader.Cells.Get(0, Column).Value = true;
                            for (int i = 0; i < FpGrid.Sheets[0].Rows.Count; i++)
                            {
                                if (FpGrid.Sheets[0].Cells[i, Column].Locked == false)
                                {
                                    FpGrid.Sheets[0].Cells[i, Column].Value = true;
                                }
                                if (FpGrid.Sheets[0].RowHeader.Cells[i, 0].Text != "I")
                                {
                                    FpGrid.Sheets[0].RowHeader.Cells[i, 0].Text = "U";
                                }
                            }
                        }                        
                    }
                }
            }

        }
        #endregion

        #region FpSpread_EditChange 수정플래그 저장
        public static void FpSpread_EditChange(FarPoint.Win.Spread.FpSpread FpGrid, int Row)
        {
            try
            {
                if (FpGrid.Sheets[0].RowHeader.Cells[Row, 0].Text != "I")
                {
                    FpGrid.Sheets[0].RowHeader.Cells[Row, 0].Text = "U";

                    FpGrid.Sheets[0].RowHeader.Rows[Row].BackColor = SystemBase.Base.Color_Update;

                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log("fpChange (수정 플래그 등록 실패)", f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY008", "수정 플래그 등록"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region FpSpread_ButtonClicked (그리드 버튼 클릭)
        public static void FpSpread_ButtonClicked(FarPoint.Win.Spread.FpSpread FpGrid, int Row, int Column)
        {
            try
            {
                if ((FpGrid.Sheets[0].GetCellType(Row, Column).ToString() == "CheckBoxCellType" && FpGrid.Sheets[0].RowHeader.Cells[Row, 0].Text != "I")
                    || (FpGrid.Sheets[0].GetCellType(Row, Column).ToString() == "ButtonCellType" && FpGrid.Sheets[0].RowHeader.Cells[Row, 0].Text != "I"))
                {
                    FpGrid.Sheets[0].RowHeader.Cells[Row, 0].Text = "U";

                    FpGrid.Sheets[0].RowHeader.Rows[Row].BackColor = SystemBase.Base.Color_Update;

                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log("그리드 버튼 클릭 실패", f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY008", "그리드 버튼 클릭"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        #endregion
        
        #region FpSpread (헤더배열값 재정의)
        public static void FpSpread_Array_Head(string FormID, string GridNM)
        {
            try
            {
                string Query = " usp_BAA004 'S3',@PFORM_ID='" + FormID.ToString() + "', @PGRID_NAME='"+GridNM+"', @PIN_ID='" + SystemBase.Base.gstrUserID + "' ";
                DataTable dt = SystemBase.DbOpen.TranDataTable(Query);
                int G1RowCount = dt.Rows.Count + 1;

                if (G1RowCount > 0)
                {
                    TabG1Head1 = new string[G1RowCount];// 첫번째 Head Text
                    TabG1Head2 = new string[G1RowCount];// 두번째 Head Text
                    TabG1Head3 = new string[G1RowCount];// 세번째 Head Text
                    TabG1Width = new int[G1RowCount];// Cell 넓이
                    TabG1Align = new string[G1RowCount];// Cell 데이타 정렬방식
                    TabG1Type = new string[G1RowCount];// CellType 지정
                    TabG1Color = new int[G1RowCount];// Cell 색상 및 ReadOnly 설정(0:일반, 1:필수, 2:ReadOnly)

                    TabG1SEQ = new int[G1RowCount];// 키
                    TabG1HeadCnt = Convert.ToInt32(dt.Rows[0][0].ToString());

                    //####################1번째 숨김필드 정의######################
                    TabG1Head1[0] = "";
                    if (Convert.ToInt32(dt.Rows[0][0].ToString()) >= 1)
                        TabG1Head2[0] = "";
                    if (Convert.ToInt32(dt.Rows[0][0].ToString()) == 3)
                        TabG1Head3[0] = "";
                    TabG1Width[0] = 0;
                    TabG1Align[0] = "";
                    TabG1Type[0] = "";
                    TabG1Color[0] = 0;
                    TabG1Etc[0] = "";

                    TabGHIdx1 = new string[G1RowCount - 1, 2];	// 그리드 Head Index 변수 길이                    
                    int OldHeadNameCount = 1;
                    
                    for (int i = 1; i < G1RowCount; i++)
                    {

                        TabG1Head1[i] = dt.Rows[i - 1][1].ToString();
                        if (Convert.ToInt32(dt.Rows[i - 1][0].ToString()) >= 1)
                            TabG1Head2[i] = dt.Rows[i - 1][2].ToString();
                        if (Convert.ToInt32(dt.Rows[i - 1][0].ToString()) == 3)
                            TabG1Head3[i] = dt.Rows[i - 1][3].ToString();

                        TabG1Width[i] = Convert.ToInt32(dt.Rows[i - 1][4].ToString());
                        TabG1Align[i] = dt.Rows[i - 1][5].ToString();
                        TabG1Type[i] = dt.Rows[i - 1][6].ToString();
                        TabG1Color[i] = Convert.ToInt32(dt.Rows[i - 1][7].ToString());

                        if (TabG1Etc[i] == null)
                            TabG1Etc[i] = dt.Rows[i - 1][8].ToString();

                        TabG1SEQ[i] = Convert.ToInt32(dt.Rows[i - 1][9].ToString());


                        OldHeadNameCount = 1;
                        TabGHIdx1[0, 0] = dt.Rows[0][1].ToString().ToUpper();
                        for (int k = 0; k < i - 1; k++)
                        {
                            if (dt.Rows[i - 1][1].ToString().ToUpper() == TabGHIdx1[k, 0].ToUpper())
                            {
                                OldHeadNameCount++;
                            }
                            else if (TabGHIdx1[k, 0].ToUpper().LastIndexOf("_") > 0 && dt.Rows[i - 1][1].ToString().ToUpper() == TabGHIdx1[k, 0].ToUpper().Substring(0, TabGHIdx1[k, 0].ToUpper().LastIndexOf("_")))
                            {
                                OldHeadNameCount++;
                            }

                        }

                        if (OldHeadNameCount > 1)
                        {
                            TabGHIdx1[i - 1, 0] = dt.Rows[i - 1][1].ToString().ToUpper() + "_" + OldHeadNameCount.ToString();	// 그리드 Head명
                        }
                        else
                        {
                            TabGHIdx1[i - 1, 0] = dt.Rows[i - 1][1].ToString().ToUpper();	// 그리드 Head명
                        }
                        TabGHIdx1[i - 1, 1] = Convert.ToString(i);			// 그리드 Head 위치
                    }

                }
            }                        
            catch (Exception f)
            {
                SystemBase.Loggers.Log(FormID, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY008", "그리드 헤더배열생성"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        #endregion

        #region FpSpread_SetColumnAllowAutoSort (그리드 오른쪽버튼 헤더 정렬)
        public static void FpSpread_SetColumnAllowAutoSort(FarPoint.Win.Spread.FpSpread FpGrid)
        {
            if (FpGrid.Sheets[0].GetColumnAllowAutoSort(0))
            {
                FpGrid.Sheets[0].SetColumnAllowAutoSort(-1, false);
            }
            else
            {
                FpGrid.Sheets[0].SetColumnAllowAutoSort(-1, true);
            }
        }
        #endregion

        #region FpSpread_SetExcelPrint (그리드 오른쪽 버튼 엑셀 출력)
        public static void FpSpread_SetExcelPrint(FarPoint.Win.Spread.FpSpread FpGrid, string ExcelName, string FormName)
       {
           try
           {
               UIForm.FPMake.ExcelMake(FpGrid, ExcelName + "_1");
           }
           catch (Exception f)
           {
               SystemBase.Loggers.Log(FormName, f.ToString());
               MessageBox.Show(SystemBase.Base.MessageRtn("SY008", "Excel 저장"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
           }
       }
        #endregion

        #region FPGrid_Closing (폼 클로징)
        public static int FPGrid_Closing(FarPoint.Win.Spread.FpSpread FpGrid)
        {            
            int UpCount = 0;
            for (int i = 0; i < FpGrid.Sheets[0].Rows.Count; i++)
            {
                if (FpGrid.Sheets[0].RowHeader.Cells[i, 0].Text == "I" || FpGrid.Sheets[0].RowHeader.Cells[i, 0].Text == "U")
                    UpCount++;
            }
            return UpCount;
        }
        #endregion

        #region FPGrid_ColumnWidthChanged (컬럼 조정)
        public static void FPGrid_ColumnWidthChanged(string FormID, FarPoint.Win.Spread.FpSpread FpGrid)
        {	//그리드 넓이 저장
            SqlConnection dbConn = SystemBase.DbOpen.DBCON();
            SqlCommand cmd = dbConn.CreateCommand();
            SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);
            cmd.Transaction = Trans;
            //cmd.CommandTimeout = 10000;
            try
            {
                for (int i = 1; i < FpGrid.Sheets[0].Columns.Count; i++)
                {
                    string Query = " usp_BAA004 'S4' ";
                    Query = Query + ", @pFORM_ID = '" + FormID.ToString() + "'";
                    Query = Query + ", @pGRID_NAME = 'fpSpread1' ";
                    Query = Query + ", @pIN_ID = '" + SystemBase.Base.gstrUserID + "' ";
                    Query = Query + ", @PSEQ = '" + TabG1SEQ[i].ToString() + "' ";
                    Query = Query + ", @PHEAD_WIDTH = '" + FpGrid.Sheets[0].ColumnHeader.Cells[0, i].Column.Width.ToString() + "' ";

                    cmd.CommandText = Query;
                    cmd.ExecuteNonQuery();

                    TabG1Width[i] = Convert.ToInt32(FpGrid.Sheets[0].ColumnHeader.Cells[0, i].Column.Width.ToString());
                }
                Trans.Commit();
            }
            catch
            {
                Trans.Rollback();
            }
            dbConn.Close();
        }
        #endregion

        #region Ctrl+ C, V 설정
        public static void FPGrid_KeyDown(string FormID, FarPoint.Win.Spread.FpSpread FpGrid, System.Windows.Forms.KeyEventArgs e)
        {
            
            try
            {
                if (e.Control && e.KeyCode == Keys.C)
                {
                    FpGrid.ClipboardOptions = FarPoint.Win.Spread.ClipboardOptions.AllHeaders;
                    FpGrid.Sheets[0].ClipboardCopy(FarPoint.Win.Spread.ClipboardCopyOptions.AsStringSkipHidden);
                }

                if (e.Control && e.KeyCode == Keys.V)
                {
                    FpGrid.Sheets[0].ClipboardPaste(ClipboardPasteOptions.Values);

                    // 복사된 행의 열을 구하기 위하여 클립보드 사용.

                    IDataObject iData = Clipboard.GetDataObject();

                    string strClp = (string)iData.GetData(DataFormats.Text);

                    if (strClp != "" || strClp != null || strClp.Length > 0)
                    {
                        Regex rx1 = new Regex("\r\n");
                        string[] arrData = rx1.Split(strClp.ToString());

                        int DataCount = 0;
                        if (arrData.Length > 1)
                            DataCount = arrData.Length - 1;
                        else
                            DataCount = arrData.Length;

                        if (DataCount > 0)
                        {
                            int STRow = FpGrid.ActiveSheet.ActiveRowIndex;
                            if (STRow < 0)
                                STRow = 0;

                            int ClipRowCount = STRow + DataCount;
                            if (FpGrid.Sheets[0].RowCount < DataCount)
                                ClipRowCount = FpGrid.Sheets[0].RowCount - STRow;

                            for (int i = STRow; i < ClipRowCount; i++)
                            {
                                if (i < FpGrid.Sheets[0].RowCount
                                    || FpGrid.Sheets[0].Cells[i, FpGrid.ActiveSheet.ActiveColumnIndex].Locked != true)
                                {
                                    if (FpGrid.Sheets[0].RowHeader.Cells[i, 0].Text != "I")
                                    {
                                        FpGrid.Sheets[0].RowHeader.Cells[i, 0].Text = "U";
                                    }
                                   // fpSpread1_ChangeEvent(i, FpGrid.ActiveSheet.ActiveColumnIndex);
                                 }
                            }

                        }
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(FormID, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY008", "Clipboard 이벤트"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region TabPageColor Setting
        public static void TabPageColor(C1.Win.C1Command.C1DockingTabPage TabPage)
        {
            TabPage.TabBackColor = System.Drawing.Color.FromArgb(88, 107, 137);
            TabPage.TabBackColorSelected = System.Drawing.Color.FromArgb(255, 255, 255);
            TabPage.TabForeColor = System.Drawing.Color.FromArgb(255, 255, 255);
            TabPage.TabForeColorSelected = System.Drawing.Color.FromArgb(0, 0, 0);
        }
        #endregion
    }
}
