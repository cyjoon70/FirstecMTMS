using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Reflection;
using FarPoint.Win.Spread;


namespace UIForm
{
    public partial class ExcelProges : Form
    {
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ProgressBar progressBar1;
        FarPoint.Win.Spread.FpSpread baseGrid;
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
       // private System.ComponentModel.Container components = null;

        string FormID = "";
        string GridName = "";
        DataTable dt = null;
        string FormName = "";
        Thread th = null;

        public ExcelProges()
        {
            InitializeComponent();
        }

        public ExcelProges(string formID, string formName, string gridName, DataTable Dt)
        {
            InitializeComponent();
            FormID = formID;
            GridName = gridName;
            dt = Dt;
            FormName = formName;
        }

        public ExcelProges(string formName, DataTable Dt)
        {
            InitializeComponent();
            dt = Dt;
            FormName = formName;
        }

        public ExcelProges(FarPoint.Win.Spread.FpSpread BaseGrid, string formName)
        {
            InitializeComponent();
            baseGrid = BaseGrid;
            FormName = formName;
        }

        #region ExcelProges_Load
        private void ExcelProges_Load(object sender, System.EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;  // 중요~~

            try
            {
                
                FormName = FormName.Replace(@"\", "_").Replace(@"/", "_").Replace(@":", "_").Replace(@"*", "_").Replace(@"?", "_").Replace(@"<", "_").Replace(@">", "_").Replace(@"|", "_").Replace("\"", "_");

                if (FormID != "")
                {	// Head 값 없을때
                    //th = new Thread(new ThreadStart(ExcelDownNoHead));
                    //th.Start();

                    Thread th = new Thread(ExcelDownNoHead);
                    th.SetApartmentState(ApartmentState.STA);
                    th.Start();
                }
                else if (baseGrid != null)
                {	// 그리드로 넘어왔을때
                    //th = new Thread(new ThreadStart(ExcelDownGrid));
                    //th.Start();

                    Thread th = new Thread(ExcelDownGrid);
                    th.SetApartmentState(ApartmentState.STA);
                    th.Start();
                    
                }
                else
                {
                    //th = new Thread(new ThreadStart(ExcelDown));
                    //th.Start();

                    Thread th = new Thread(ExcelDown);
                    th.SetApartmentState(ApartmentState.STA);
                    th.Start();
                }
            }
            catch (Exception f)
            {
                th.Abort();
                SystemBase.Loggers.Log("ExcelProges : ", f.ToString());
                MessageBox.Show("엑셀 출력 중 예기치 못한 오류가 발생하였습니다.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 엑셀 다운로드 데이타 데이블
        public void ExcelDown()
        {

            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Title = "Excel 다운로드 위치 지정";
            dlg.InitialDirectory = dlg.FileName;
            dlg.Filter = "전체(*.*)|*.*|Excel Files(*.xls)|*.xls";
            dlg.FilterIndex = 1;
            dlg.RestoreDirectory = true;
            dlg.FileName = FormName + ".xls";
            dlg.OverwritePrompt = false;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                label1.Text = "엑셀 데이타 준비중입니다.";
                Excel.Application oAppln;
                Excel.Workbook oWorkBook;
                Excel.Worksheet oWorkSheet;
                Excel.Range oRange;
 

                try
                {
                    oAppln = new Excel.Application();
                    oWorkBook = (Excel.Workbook)(oAppln.Workbooks.Add(true));
                    oWorkSheet = (Excel.Worksheet)oWorkBook.ActiveSheet;

                    int iRow = 1;

                    progressBar1.Maximum = dt.Rows.Count;
                    if (dt.Rows.Count > 0)
                    { // 그리드 데이타가 있는경우
                        for (int rowNo = 0; rowNo < dt.Rows.Count; rowNo++)
                        { //내용 저장
                            for (int colNo = 0; colNo < dt.Columns.Count; colNo++)
                            {
                                if (rowNo == 0)
                                {
                                    string strType = dt.Rows[rowNo][colNo].GetType().ToString();
                                    if (strType.ToUpper().Substring(0, 7) == "VARCHAR")
                                    {
                                        oRange = oWorkSheet.get_Range(SystemBase.Base.ExcelCol(colNo + 1) + ":" + SystemBase.Base.ExcelCol(colNo + 1), SystemBase.Base.ExcelCol(colNo + 1) + ":" + SystemBase.Base.ExcelCol(colNo + 1));
                                        oRange.Cells.NumberFormat = "@";
                                    }
                                }

                                oWorkSheet.Cells[iRow, colNo + 1] = dt.Rows[rowNo][colNo].ToString();
                                
                                //oRange.Select();
                                //oRange.NumberFormatLocal = "@";
                            }
                            iRow++;
                            progressBar1.Value = rowNo + 1;
                            label1.Text = "총" + dt.Rows.Count.ToString() + "Row 중 " + iRow.ToString() + "Row를 저장하였습니다.";
                        }
                    }
                    label1.Text = "엑셀 Sheet를 열고 있습니다.";

                    //range of the excel sheet
                    oRange = oWorkSheet.get_Range("A1", "IV1");
                    oRange.EntireColumn.AutoFit();
                    oAppln.UserControl = false;
                    oAppln.Visible = true;	// 저장후 저장된 내용 실행여부

                    // 엑셀 파일로 저장
                    oWorkBook.SaveAs(dlg.FileName, Excel.XlFileFormat.xlWorkbookNormal, null, null, false, false, Excel.XlSaveAsAccessMode.xlShared, false, false, null, null, null);
                    label1.Text = "완료되었습니다.";
                }
                catch //(Exception f)
                {
                    //MessageBox.Show(f.Message.ToString());
                }
                this.Close();
            }
            else
            {
                this.Close();
            }
        }
        #endregion

        #region 엑셀 다운로드 데이타 테이블(Head 미생성)
        public void ExcelDownNoHead()
        {
            //string FilePath = "";
            //			FolderBrowserDialog fd = new FolderBrowserDialog();
            //			if (fd.ShowDialog() == DialogResult.OK)
            //			{

            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Title = "Excel 다운로드 위치 지정";
            dlg.InitialDirectory = dlg.FileName;
            dlg.Filter = "전체(*.*)|*.*|Excel Files(*.xls)|*.xls";
            dlg.FilterIndex = 1;
            dlg.RestoreDirectory = true;
            dlg.FileName = FormName + ".xls";
            dlg.OverwritePrompt = false;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                //string FilePath = fd.SelectedPath.ToString()+@"\"+FormName+".xls";

                //			SaveFileDialog dlg = new SaveFileDialog();
                //			dlg.DefaultExt = "xls";
                //			dlg.FileName = FormName.ToString() + ".xls";
                //			if (dlg.ShowDialog() == DialogResult.OK)
                //			{

                label1.Text = "엑셀 HEAD를 생성중입니다.";
                Excel.Application oAppln;
                Excel.Workbook oWorkBook;
                Excel.Worksheet oWorkSheet;
                Excel.Range oRange;
                try
                {
                    oAppln = new Excel.Application();
                    oWorkBook = (Excel.Workbook)(oAppln.Workbooks.Add(true));
                    oWorkSheet = (Excel.Worksheet)oWorkBook.ActiveSheet;

                    int iRow = 1;

                    progressBar1.Maximum = dt.Rows.Count;
                    if (dt.Rows.Count > 0)
                    { // 그리드 데이타가 있는경우

                        string Query = " usp_CZ004 'S3', @PFORM_ID='" + FormID.ToString() + "', @PGRID_NAME='" + GridName.ToString() + "', @PIN_ID='" + SystemBase.Base.gstrUserID + "' ";
                        DataTable hddt = SystemBase.DbOpen.TranDataTable(Query);

                        //headers 
                        int ColunmCount = Convert.ToInt32(hddt.Rows[0][0].ToString());
                        for (int j = 0; j < hddt.Rows.Count; j++)
                        { // header 저장
                            oWorkSheet.Cells[1, j + 1] = hddt.Rows[j][1].ToString();
                        }
                        if (ColunmCount > 1)
                        {
                            for (int j = 0; j < hddt.Rows.Count; j++)
                            { // header 저장
                                oWorkSheet.Cells[2, j + 1] = hddt.Rows[j][2].ToString();
                            }
                        }
                        if (ColunmCount > 2)
                        {
                            for (int j = 0; j < hddt.Rows.Count; j++)
                            { // header 저장
                                oWorkSheet.Cells[3, j + 1] = hddt.Rows[j][3].ToString();
                            }
                        }

                        for (int rowNo = 0; rowNo < dt.Rows.Count; rowNo++)
                        { //내용 저장
                            for (int colNo = 0; colNo < dt.Columns.Count; colNo++)
                            {
                                if (rowNo == 0)
                                {
                                    string strType = dt.Rows[rowNo][colNo].GetType().ToString();
                                    if (strType.ToUpper().Substring(0, 7) == "VARCHAR")
                                    {
                                        oRange = oWorkSheet.get_Range(SystemBase.Base.ExcelCol(colNo + 1) + ":" + SystemBase.Base.ExcelCol(colNo + 1), SystemBase.Base.ExcelCol(colNo + 1) + ":" + SystemBase.Base.ExcelCol(colNo + 1));
                                        oRange.Cells.NumberFormat = "@";
                                    }
                                }
                                
                                oWorkSheet.Cells[iRow + ColunmCount, colNo + 1] = dt.Rows[rowNo][colNo].ToString();
                            }
                            iRow++;
                            progressBar1.Value = rowNo + 1;
                            label1.Text = "총" + dt.Rows.Count.ToString() + "Row 중 " + iRow.ToString() + "Row를 저장하였습니다.";
                        }
                    }
                    label1.Text = "엑셀 Sheet를 열고 있습니다.";

                    //range of the excel sheet
                    oRange = oWorkSheet.get_Range("A1", "IV1");
                    oRange.EntireColumn.AutoFit();
                    oAppln.UserControl = false;
                    //string strFile ="d:/"+ "report" + ".xls";
                    //string strFile = FilePath;	// 저장경로

                    oAppln.Visible = true;	// 저장후 저장된 내용 실행여부

                    // 엑셀 파일로 저장
                    //oWorkBook.SaveAs( dlg.FileName, Excel.XlFileFormat.xlWorkbookNormal, null, null, false, false, Excel.XlSaveAsAccessMode.xlShared, false, false, null, null, null);
                    oWorkBook.SaveAs(dlg.FileName, Excel.XlFileFormat.xlWorkbookNormal, null, null, false, false, Excel.XlSaveAsAccessMode.xlShared, false, false, null, null, null);
                    label1.Text = "완료되었습니다.";
                }
                catch //(Exception f)
                {
                    //MessageBox.Show(f.Message.ToString());
                }
                this.Close();
            }
            else
            {
                this.Close();
            }

        }
        #endregion

        #region 엑셀 다운로드 그리드
        public void ExcelDownGrid()
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Title = "Excel 다운로드 위치 지정";
            dlg.InitialDirectory = dlg.FileName;
            dlg.Filter = "전체(*.*)|*.*|Excel Files(*.xls)|*.xls";
            dlg.FilterIndex = 1;
            dlg.RestoreDirectory = true;
            dlg.FileName = FormName + ".xls";
            dlg.OverwritePrompt = false;


            if (dlg.ShowDialog() == DialogResult.OK)
            {
                label1.Text = "엑셀 HEAD를 생성중입니다.";

                Excel.Application oAppln;
                Excel.Workbook oWorkBook;
                Excel.Worksheet oWorkSheet;
                Excel.Range oRange;

                try
                {
                    oAppln = new Excel.Application();
                    oWorkBook = (Excel.Workbook)(oAppln.Workbooks.Add(true));                    
                    oWorkSheet = (Excel.Worksheet)oWorkBook.ActiveSheet;

                    //oRange = oWorkSheet.get_Range("A1:A65000", "IV1:IV65000");
                    //oRange.Cells.NumberFormat = "@"; 

                    if (baseGrid.Sheets[0].Rows.Count > 0)
                    {	// 그리드 데이타가 있는경우
                        progressBar1.Maximum = baseGrid.Sheets[0].Rows.Count;

                        // header 저장
                        int headRow = 0;

                        int shtTitleSpanTmp = 1, shtTitleSpanTmp2 = 1, shtTitleSpanTmp3 = 1;

                        for (int HeadColCnt = 1; HeadColCnt < baseGrid.Sheets[0].Columns.Count; HeadColCnt++)
                        {
                            headRow = 1;

                            for (int HeadRowCnt = 0; HeadRowCnt < baseGrid.Sheets[0].ColumnHeaderRowCount; HeadRowCnt++)
                            {
                                oWorkSheet.Cells[headRow, HeadColCnt] = baseGrid.Sheets[0].ColumnHeader.Cells[HeadRowCnt, HeadColCnt].Text;

                                headRow++;
                            }

                            //ColHead 합치기
                            if (baseGrid.Sheets[0].ColumnHeaderRowCount > 3)
                            {
                                if (HeadColCnt > 1 && baseGrid.Sheets[0].ColumnHeader.Cells[2, HeadColCnt].Text == baseGrid.Sheets[0].ColumnHeader.Cells[2, HeadColCnt - 1].Text)
                                {
                                    shtTitleSpanTmp3++;
                                }
                                else
                                {
                                    if (HeadColCnt > 1)
                                    {
                                        oWorkSheet.Application.DisplayAlerts = false;
                                        Excel.Range Value1 = ((Excel.Range)(oWorkSheet.Cells[3, HeadColCnt - shtTitleSpanTmp3]));
                                        Excel.Range Value2 = ((Excel.Range)(oWorkSheet.Cells[3, HeadColCnt - 1]));

                                        //Excel.Range eRange = oWorkSheet.get_Range(oWorkSheet.Cells[3, HeadColCnt - shtTitleSpanTmp3], oWorkSheet.Cells[3, HeadColCnt - 1]);
                                        Excel.Range eRange = oWorkSheet.get_Range(Value1, Value2);
                                        eRange.Merge(Type.Missing);
                                        eRange.HorizontalAlignment = HorizontalAlignment.Center;

                                        oWorkSheet.Application.DisplayAlerts = true;
                                    }

                                    shtTitleSpanTmp3 = 1;
                                }

                                //RowHead 합치기
                                if (baseGrid.Sheets[0].ColumnHeader.Cells[0, HeadColCnt].Text == baseGrid.Sheets[0].ColumnHeader.Cells[1, HeadColCnt].Text
                                    && baseGrid.Sheets[0].ColumnHeader.Cells[1, HeadColCnt].Text == baseGrid.Sheets[0].ColumnHeader.Cells[2, HeadColCnt].Text)
                                {
                                    oWorkSheet.Application.DisplayAlerts = false;
                                    Excel.Range Value1 = ((Excel.Range)(oWorkSheet.Cells[1, HeadColCnt]));
                                    Excel.Range Value2 = ((Excel.Range)(oWorkSheet.Cells[3, HeadColCnt]));

                                    //Excel.Range eRange = oWorkSheet.get_Range(oWorkSheet.Cells[1, HeadColCnt], oWorkSheet.Cells[3, HeadColCnt]);
                                    Excel.Range eRange = oWorkSheet.get_Range(Value1, Value2);
                                    eRange.Merge(Type.Missing);
                                    eRange.HorizontalAlignment = HorizontalAlignment.Center;

                                    oWorkSheet.Application.DisplayAlerts = true;
                                }
                            }

                            if (baseGrid.Sheets[0].ColumnHeaderRowCount > 2)
                            {
                                if (HeadColCnt > 1 && baseGrid.Sheets[0].ColumnHeader.Cells[1, HeadColCnt].Text == baseGrid.Sheets[0].ColumnHeader.Cells[1, HeadColCnt - 1].Text)
                                {
                                    shtTitleSpanTmp2++;
                                }
                                else
                                {
                                    if (HeadColCnt > 1)
                                    {
                                        oWorkSheet.Application.DisplayAlerts = false;
                                        Excel.Range Value1 = ((Excel.Range)(oWorkSheet.Cells[2, HeadColCnt - shtTitleSpanTmp2]));
                                        Excel.Range Value2 = ((Excel.Range)(oWorkSheet.Cells[2, HeadColCnt - 1]));

                                        //Excel.Range eRange = oWorkSheet.get_Range(oWorkSheet.Cells[2, HeadColCnt - shtTitleSpanTmp2], oWorkSheet.Cells[2, HeadColCnt - 1]);
                                        Excel.Range eRange = oWorkSheet.get_Range(Value1, Value2);
                                        eRange.Merge(Type.Missing);
                                        eRange.HorizontalAlignment = HorizontalAlignment.Center;

                                        oWorkSheet.Application.DisplayAlerts = true;
                                    }

                                    shtTitleSpanTmp2 = 1;
                                }

                                //RowHead 합치기
                                if (baseGrid.Sheets[0].ColumnHeader.Cells[1, HeadColCnt].Text == baseGrid.Sheets[0].ColumnHeader.Cells[2, HeadColCnt].Text)
                                {
                                    oWorkSheet.Application.DisplayAlerts = false;
                                    Excel.Range Value1 = ((Excel.Range)(oWorkSheet.Cells[2, HeadColCnt]));
                                    Excel.Range Value2 = ((Excel.Range)(oWorkSheet.Cells[3, HeadColCnt]));

                                    //Excel.Range eRange = oWorkSheet.get_Range(oWorkSheet.Cells[2, HeadColCnt], oWorkSheet.Cells[3, HeadColCnt]);
                                    Excel.Range eRange = oWorkSheet.get_Range(Value1, Value2);
                                    eRange.Merge(Type.Missing);
                                    eRange.HorizontalAlignment = HorizontalAlignment.Center;

                                    oWorkSheet.Application.DisplayAlerts = true;
                                }
                            }

                            if (baseGrid.Sheets[0].ColumnHeaderRowCount > 1)
                            {
                                if (HeadColCnt > 1 && baseGrid.Sheets[0].ColumnHeader.Cells[0, HeadColCnt].Text == baseGrid.Sheets[0].ColumnHeader.Cells[0, HeadColCnt - 1].Text)
                                {
                                    shtTitleSpanTmp++;
                                }
                                else
                                {
                                    if (HeadColCnt > 1)
                                    {
                                        oWorkSheet.Application.DisplayAlerts = false;
                                        Excel.Range Value1 = ((Excel.Range)(oWorkSheet.Cells[1, HeadColCnt - shtTitleSpanTmp]));
                                        Excel.Range Value2 = ((Excel.Range)(oWorkSheet.Cells[1, HeadColCnt - 1]));

                                        //Excel.Range eRange = oWorkSheet.get_Range(oWorkSheet.Cells[1, HeadColCnt - shtTitleSpanTmp], oWorkSheet.Cells[1, HeadColCnt - 1]);
                                        Excel.Range eRange = oWorkSheet.get_Range(Value1, Value2);
                                        eRange.Merge(Type.Missing);
                                        eRange.HorizontalAlignment = HorizontalAlignment.Center;

                                        oWorkSheet.Application.DisplayAlerts = true;
                                    }

                                    shtTitleSpanTmp = 1;
                                }

                                //RowHead 합치기
                                if (baseGrid.Sheets[0].ColumnHeader.Cells[0, HeadColCnt].Text == baseGrid.Sheets[0].ColumnHeader.Cells[0 + 1, HeadColCnt].Text)
                                {
                                    oWorkSheet.Application.DisplayAlerts = false;
                                    Excel.Range Value1 = ((Excel.Range)(oWorkSheet.Cells[1, HeadColCnt]));
                                    Excel.Range Value2 = ((Excel.Range)(oWorkSheet.Cells[2, HeadColCnt]));  

                                    // Excel.Range eRange = oWorkSheet.get_Range(oWorkSheet.Cells[1, HeadColCnt], oWorkSheet.Cells[2, HeadColCnt]);
                                    Excel.Range eRange = oWorkSheet.get_Range(Value1, Value2);
                                    eRange.Merge(Type.Missing);
                                    eRange.HorizontalAlignment = HorizontalAlignment.Center;

                                    oWorkSheet.Application.DisplayAlerts = true;
                                }
                            }
                        }
                        int iRow = headRow;

                        //내용 저장
                        for (int rowNo = 0; rowNo < baseGrid.Sheets[0].Rows.Count; rowNo++)
                        {
                            for (int colNo = 1; colNo < baseGrid.Sheets[0].Columns.Count; colNo++)
                            {
                                //text형식은 text로 엑셀에 뿌린다.////////////////////////////
                                if (rowNo == 0)
                                {
                                    string strType = baseGrid.Sheets[0].Columns[colNo].CellType.ToString();
                                    if (strType == "TextCellType")
                                    {
                                        oRange = oWorkSheet.get_Range(SystemBase.Base.ExcelCol(colNo) + ":" + SystemBase.Base.ExcelCol(colNo), SystemBase.Base.ExcelCol(colNo) + ":" + SystemBase.Base.ExcelCol(colNo));
                                        oRange.Cells.NumberFormat = "@";
                                    }
                                }
                                ///////////////////////////////////////////////////////////////

                                oWorkSheet.Cells[iRow, colNo] = baseGrid.Sheets[0].Cells[rowNo, colNo].Text;

                                if (baseGrid.Sheets[0].Cells[rowNo, colNo].Text != "" && baseGrid.Sheets[0].Cells[rowNo, colNo].ForeColor != Color.Empty)
                                {
                                    Excel.Range Value1 = ((Excel.Range)(oWorkSheet.Cells[iRow, colNo]));

                                    //Excel.Range eRange = oWorkSheet.get_Range(oWorkSheet.Cells[iRow, colNo], oWorkSheet.Cells[iRow, colNo]);
                                    Excel.Range eRange = oWorkSheet.get_Range(Value1, Value1);
                                    eRange.Font.Color = ColorTranslator.ToOle(baseGrid.Sheets[0].Cells[rowNo, colNo].ForeColor);
                                }
                            }
                            iRow++;
                            progressBar1.Value = rowNo + 1;
                            label1.Text = "총" + baseGrid.Sheets[0].Rows.Count.ToString() + " Row 중 " + iRow.ToString() + " Row를 저장하였습니다.";
                        }
                    }
                   
                    label1.Text = "엑셀 Sheet를 열고 있습니다.";
                    //range of the excel sheet
                    oRange = oWorkSheet.get_Range("A1", "IV1");
                    oRange.EntireColumn.AutoFit();
                    oAppln.UserControl = false;

                    oAppln.Visible = true;	// 저장후 저장된 내용 실행여부

                    // 엑셀 파일로 저장
                    //oWorkBook.SaveAs( dlg.FileName, Excel.XlFileFormat.xlWorkbookNormal, null, null, false, false, Excel.XlSaveAsAccessMode.xlNoChange, false, false, null, null, null);


                    oWorkBook.SaveAs(dlg.FileName, Excel.XlFileFormat.xlWorkbookNormal, null, null, false, false, Excel.XlSaveAsAccessMode.xlNoChange, false, false, null, null, null);

                    label1.Text = "완료되었습니다.";
                }
                catch (System.Runtime.InteropServices.COMException)
                {
                    oAppln = null;
                    this.Close();
                }
                catch (Exception f)
                {
                    MessageBox.Show(f.ToString());
                    this.Close();
                }
            }
            else
            {
                this.Close();
            }

            this.Close();


        }
        #endregion

        #region ExcelProges_Closing
        private void ExcelProges_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (th != null)
                th.Abort();
        }
        #endregion

    }
}
