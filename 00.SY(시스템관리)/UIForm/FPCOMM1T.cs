using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;
using FarPoint.Win.Spread;
using System.Text.RegularExpressions;
using UIForm.Extension.FpSpread;


namespace UIForm
{
    public partial class FPCOMM1T : Buttons
    {
        int GridSelectRow = 0;
        int GridSelectRowCount = 0;
        int ClipboardRowCount = 1;
        int ClipboardColCount = 1;
        string ClipValue = "";

        #region 그리드 디자인 변수 정의
        public string[] G1Head1 = null;// 첫번째 Head Text
        public string[] G1Head2 = null;// 두번째 Head Text
        public string[] G1Head3 = null;// 세번째 Head Text
        public int[] G1Width = null;// Cell 넓이
        public string[] G1Align = null;// Cell 데이타 정렬방식
        public string[] G1Type = null;// CellType 지정
        public int[] G1Color = null;// Cell 색상 및 ReadOnly 설정(0:일반, 1:필수, 2:ReadOnly)
        public string[] G1Etc = new string[100];// Mask 양식 등
        public int[] G1SEQ = null;// 키
        public int G1HeadCnt = 0;	// Head 수
        #endregion

        public System.Windows.Forms.GroupBox GridCommGroupBox;
        public System.Windows.Forms.Panel GridCommPanel;
        public FarPoint.Win.Spread.FpSpread fpSpread1;
        public FarPoint.Win.Spread.SheetView fpSpread1_Sheet1;
        private System.Windows.Forms.ContextMenu ctmGrid1;
        private System.Windows.Forms.MenuItem menuItem1;
        private System.Windows.Forms.MenuItem menuItem2;
        private System.Windows.Forms.MenuItem menuItem3;
        private System.Windows.Forms.TextBox txtRowCnt;
        private System.Windows.Forms.MenuItem menuItem4;
        private System.Windows.Forms.MenuItem menuItem5;
        public C1.Win.C1Command.C1DockingTabPage c1DockingTabPage1;
        public C1.Win.C1Command.C1DockingTab c1TabCommon;
        public System.Windows.Forms.Panel TabCommPanel;

        private FindText frm = new FindText();

        public FPCOMM1T()
        {
            InitializeComponent();

            this.SetStyle(ControlStyles.DoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            this.UpdateStyles();
        }

        #region SaveExec 저장
        protected virtual void SaveExec() { }
        protected override void SaveExec2()
        {	// 저장
            try
            {
                this.BtnInsert.Focus();
                SaveExec();
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY008", "저장"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        #endregion

        #region RCopyExec 그리드 Row 복사
        protected virtual void RCopyExe() { }
        protected override void RCopyExec()
        {
            try
            {
                UIForm.FPMake.RowCopy(fpSpread1);
                RCopyExe();
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "행복사"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region DelExec 행 삭제
        protected virtual void DelExe() { }
        protected override void DelExec()
        {	// 행 삭제
            try
            {
                UIForm.FPMake.RowRemove(fpSpread1);
                DelExe();
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "행삭제"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        #endregion

        #region DelExec 싱글 삭제로직
        protected virtual void DeleteExe() { }
        protected override void DeleteExec()
        {	// 행 삭제
            try
            {
                if (MessageBox.Show(SystemBase.Base.MessageRtn("B0047"), "전체삭제", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                {
                    DeleteExe();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "전체삭제"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        #endregion

        #region CancelExec 행 취소
        protected virtual void CancelExe() { }
        protected override void CancelExec()
        {	// 행 삭제
            try
            {
                if (fpSpread1.ActiveSheet.GetSelection(0) == null || fpSpread1.ActiveSheet.ActiveRowIndex < 0) return;

                //				if(fpSpread1.ActiveSheet.GetSelection(0) != null)
                //				{
                //					GridSelectRow = fpSpread1.ActiveSheet.GetSelection(0).Row;
                //					GridSelectRowCount = fpSpread1.ActiveSheet.GetSelection(0).RowCount;
                //				}
                if (fpSpread1.ActiveSheet.ActiveRowIndex.ToString() != "")
                {
                    GridSelectRow = fpSpread1.ActiveSheet.ActiveRowIndex; //fpSpread1.ActiveSheet.GetSelection(0).Row;//
                    //					GridSelectRowCount = fpSpread1.ActiveSheet.GetSelection(0).RowCount;

                    int Row = fpSpread1.ActiveSheet.ActiveRowIndex;
                    int Col = fpSpread1.ActiveSheet.ActiveColumnIndex;

                    GridSelectRowCount = 1;
                    if (fpSpread1.ActiveSheet.GetCellType(Row, Col).ToString() != "ComboBoxCellType" && fpSpread1.ActiveSheet.GetCellType(Row, Col).ToString() != "CheckBoxCellType")
                    {
                        GridSelectRowCount = fpSpread1.Sheets[0].GetSelection(0).RowCount;
                    }
                }
                UIForm.FPMake.Cancel(fpSpread1, GridSelectRow, GridSelectRowCount);
                CancelExe();
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "행삭제"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        #endregion

        #region RowInsExec 행 추가
        protected virtual void RowInsExe() { }
        protected override void RowInsExec()
        {	// 행 추가
            try
            {
                UIForm.FPMake.RowInsert(fpSpread1);
                RowInsExe();
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "행추가"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        #endregion

        #region ExcelExec() Excel 저장
        protected virtual void ExcelExe() { }
        protected override void ExcelExec()
        {
            try
            {
                UIForm.FPMake.ExcelMake(fpSpread1, this.Text.ToString());
                ExcelExe();
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "Excel 저장"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        #endregion

        #region PrintMake(그리드, 미리보기) 그리드 Print
        protected virtual void PrintExe() { }
        protected override void PrintExec()
        {
            try
            {
                //				UIForm.FPMake.PrintMake(fpSpread1, true, 2);
                UIForm.FPMake.PrintMake(fpSpread1, this.Text);
                PrintExe();
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "프린트"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        #endregion

        #region fpSpread1_ComboCloseUp 스프레드에서 콤보 선택시 Focus 이동
        protected virtual void fpComboCloseUp() { }
        private void fpSpread1_ComboCloseUp(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            try
            {
                fpSpread1.ActiveSheet.SetActiveCell(e.Row, e.Column + 1);
                fpComboCloseUp();
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "그리드 콤보 선택"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        #endregion

        #region fpSpread1_Change 데이타 수정시 U 플래그 등록
        protected virtual void fpChange() { }
        private void fpSpread1_EditChange(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            try
            {
                UIForm.FPMake.fpChange(fpSpread1, e.Row);
                fpChange();
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "그리드 EditChange 이벤트"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        #endregion

        #region fpSpread1_ButtonClicked 버튼 클릭 Event
        protected virtual void fpButtonClick(int Row, int Column) { }
        private void fpSpread1_ButtonClicked(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            try
            {
                fpButtonClick(e.Row, e.Column);
                if (fpSpread1.Sheets[0].GetCellType(e.Row, e.Column).ToString() == "CheckBoxCellType" && fpSpread1.Sheets[0].RowHeader.Cells[e.Row, 0].Text != "I")
                {
                    fpSpread1.Sheets[0].RowHeader.Cells[e.Row, 0].Text = "U";
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "그리드 버튼 클릭"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        #endregion

        #region FPCOMM1T_Load
        private void FPCOMM1T_Load(object sender, System.EventArgs e)
        {
            try
            {
                if (SystemBase.Base.ProgramWhere.Length > 0)
                {
                    string Query = " usp_BAA004 'S3',@PFORM_ID='" + this.Name.ToString() + "', @PGRID_NAME='fpSpread1', @PIN_ID='" + SystemBase.Base.gstrUserID + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' ";
                    DataTable dt = SystemBase.DbOpen.TranDataTable(Query);
                    int G1RowCount = dt.Rows.Count + 1;

                    if (G1RowCount > 0)
                    {
                        G1Head1 = new string[G1RowCount];// 첫번째 Head Text
                        G1Head2 = new string[G1RowCount];// 두번째 Head Text
                        G1Head3 = new string[G1RowCount];// 세번째 Head Text
                        G1Width = new int[G1RowCount];// Cell 넓이
                        G1Align = new string[G1RowCount];// Cell 데이타 정렬방식
                        G1Type = new string[G1RowCount];// CellType 지정
                        G1Color = new int[G1RowCount];// Cell 색상 및 ReadOnly 설정(0:일반, 1:필수, 2:ReadOnly)

                        G1SEQ = new int[G1RowCount];// 키

                        //G1Etc		= new string[G1RowCount];
                        G1HeadCnt = Convert.ToInt32(dt.Rows[0][0].ToString());

                        //####################1번째 숨김필드 정의######################
                        G1Head1[0] = "";
                        if (Convert.ToInt32(dt.Rows[0][0].ToString()) >= 1)
                            G1Head2[0] = "";
                        if (Convert.ToInt32(dt.Rows[0][0].ToString()) == 3)
                            G1Head3[0] = "";
                        G1Width[0] = 0;
                        G1Align[0] = "";
                        G1Type[0] = "";
                        G1Color[0] = 0;
                        G1Etc[0] = "";
                        //####################1번째 숨김필드 정의######################

                        //####################그리드 Head 순번######################
                        GHIdx1 = new string[G1RowCount - 1, 2];	// 그리드 Head Index 변수 길이
                        //string OldHeadName = null;
                        int OldHeadNameCount = 1;
                        //####################그리드 Head 순번######################
                        for (int i = 1; i < G1RowCount; i++)
                        {
                            G1Head1[i] = dt.Rows[i - 1][1].ToString();
                            if (Convert.ToInt32(dt.Rows[i - 1][0].ToString()) >= 1)
                                G1Head2[i] = dt.Rows[i - 1][2].ToString();
                            if (Convert.ToInt32(dt.Rows[i - 1][0].ToString()) == 3)
                                G1Head3[i] = dt.Rows[i - 1][3].ToString();

                            G1Width[i] = Convert.ToInt32(dt.Rows[i - 1][4].ToString());
                            G1Align[i] = dt.Rows[i - 1][5].ToString();
                            G1Type[i] = dt.Rows[i - 1][6].ToString();
                            G1Color[i] = Convert.ToInt32(dt.Rows[i - 1][7].ToString());

                            if (G1Etc[i] == null)
                                G1Etc[i] = dt.Rows[i - 1][8].ToString();

                            G1SEQ[i] = Convert.ToInt32(dt.Rows[i - 1][9].ToString());


                            //####################그리드 Head 순번######################
                            OldHeadNameCount = 1;
                            GHIdx1[0, 0] = dt.Rows[0][1].ToString().ToUpper();
                            for (int k = 0; k < i - 1; k++)
                            {
                                if (dt.Rows[i - 1][1].ToString().ToUpper() == GHIdx1[k, 0].ToUpper())
                                {
                                    OldHeadNameCount++;
                                }
                                else if (GHIdx1[k, 0].ToUpper().LastIndexOf("_") > 0 && dt.Rows[i - 1][1].ToString().ToUpper() == GHIdx1[k, 0].ToUpper().Substring(0, GHIdx1[k, 0].ToUpper().LastIndexOf("_")))
                                {
                                    OldHeadNameCount++;
                                }

                            }
                            if (OldHeadNameCount > 1)
                            {
                                GHIdx1[i - 1, 0] = dt.Rows[i - 1][1].ToString().ToUpper() + "_" + OldHeadNameCount.ToString();	// 그리드 Head명
                            }
                            else
                            {
                                GHIdx1[i - 1, 0] = dt.Rows[i - 1][1].ToString().ToUpper();	// 그리드 Head명
                            }

                            GHIdx1[i - 1, 1] = Convert.ToString(i);			// 그리드 Head 위치
                            //####################그리드 Head 순번######################

                        }
                        UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false , 0, 0 );
                    }

                    /*----------------------------------------------------------------------------------*/
                    // 행추가 SUBMENU
                    // 툴바에 Add 메뉴가 비활성화시 행추가도 비활성화
                    string btnQuery = "usp_TOOLBARSET '" + SystemBase.Base.gstrUserID.ToString() + "','" + SystemBase.Base.RodeFormID.ToString() + "'";
                    DataTable Rowdt = SystemBase.DbOpen.NoTranDataTable(btnQuery);

                    if (Rowdt.Rows[0][4].ToString() == "0")
                    {
                        menuItem3.Enabled = false;
                    }
                    /*----------------------------------------------------------------------------------*/
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY008", "화면 생성"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

		#region fpSpread 전체선택 체크박스 핸들러 (2014.9.12, 이재광)
		private void fpSpreads_CellClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
		{
			// 헤더 체크박스 클릭시 전체행 체크 처리
			if (e.ColumnHeader)
				if (((FarPoint.Win.Spread.FpSpread)sender).ActiveSheet.ToggleCheckAll(e.Column))
					e.Cancel = true;
		}
		#endregion

        #region FPCOMM1T_Closing
        private void FPCOMM1T_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (strFormClosingMsg == true)
            {
                int UpCount = 0;
                for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                {
                    if (fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text == "I" || fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text == "U")
                        UpCount++;
                }

                if (UpCount > 0)
                {
                    DialogResult Rtn = MessageBox.Show(SystemBase.Base.MessageRtn("SY011"), SystemBase.Base.MessageRtn("Z0004"), MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                    if (Rtn != DialogResult.OK)
                        e.Cancel = true;
                }
            }
        }
        #endregion

        #region 그리드 넓이 사용자정의
        private void menuItem1_Click(object sender, System.EventArgs e)
        {	//초기화
            if (MessageBox.Show(SystemBase.Base.MessageRtn("SY012"), SystemBase.Base.MessageRtn("Z0004"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {	//초기화하시겠습니까?
                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);
                cmd.Transaction = Trans;
                //cmd.CommandTimeout = 10000;
                try
                {
                    string Query = " usp_BAA004 'S5' ";
                    Query = Query + ", @pFORM_ID='" + this.Name.ToString() + "'";
                    Query = Query + ", @pGRID_NAME='fpSpread1'";
                    Query = Query + ", @pIN_ID='" + SystemBase.Base.gstrUserID + "' ";

                    cmd.CommandText = Query;
                    cmd.ExecuteNonQuery();
                    Trans.Commit();


                }
                catch//(Exception f)
                {
                    Trans.Rollback();
                    //RtnMsg = "에러가 발생되어 롤백되었습니다.\n\r\n\r" + f.ToString();
                    //MessageBox.Show(RtnMsg);
                }

                try
                {
                    // 초기화된 그리드 넓이 화면에 적용
                    string Query2 = " usp_BAA004 'S3' ";
                    Query2 = Query2 + ", @pFORM_ID='" + this.Name.ToString() + "'";
                    Query2 = Query2 + ", @pGRID_NAME='fpSpread1'";
                    Query2 = Query2 + ", @pIN_ID='" + SystemBase.Base.gstrUserID + "' ";
                    DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query2);

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        fpSpread1.Sheets[0].Columns[i + 1].Width = Convert.ToInt32(dt.Rows[i]["HEAD_WIDTH"].ToString());
                        G1Width[i + 1] = Convert.ToInt32(dt.Rows[i]["HEAD_WIDTH"].ToString());
                    }
                    // 초기화된 그리드 넓이 화면에 적용
                }
                catch { }

                dbConn.Close();
            }
        }

        private void fpSpread1_ColumnWidthChanged(object sender, FarPoint.Win.Spread.ColumnWidthChangedEventArgs e)
        {	//그리드 넓이 저장
            SqlConnection dbConn = SystemBase.DbOpen.DBCON();
            SqlCommand cmd = dbConn.CreateCommand();
            SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);
            cmd.Transaction = Trans;
            //cmd.CommandTimeout = 10000;
            try
            {
                for (int i = 1; i < fpSpread1.Sheets[0].Columns.Count; i++)
                {
                    string Query = " usp_BAA004 'S4' ";
                    Query = Query + ", @pFORM_ID = '" + this.Name.ToString() + "'";
                    Query = Query + ", @pGRID_NAME = 'fpSpread1' ";
                    Query = Query + ", @pIN_ID = '" + SystemBase.Base.gstrUserID + "' ";
                    Query = Query + ", @PSEQ = '" + G1SEQ[i].ToString() + "' ";
                    Query = Query + ", @PHEAD_WIDTH = '" + fpSpread1.Sheets[0].ColumnHeader.Cells[0, i].Column.Width.ToString() + "' ";
                    Query = Query + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' ";

                    cmd.CommandText = Query;
                    cmd.ExecuteNonQuery();

                    G1Width[i] = Convert.ToInt32(fpSpread1.Sheets[0].ColumnHeader.Cells[0, i].Column.Width.ToString());
                }
                Trans.Commit();
            }
            catch//(Exception f)
            {
                Trans.Rollback();
                //RtnMsg = "에러가 발생되어 롤백되었습니다.\n\r\n\r" + f.ToString();
                //MessageBox.Show(RtnMsg);
            }
            dbConn.Close();
        }
        #endregion

        #region menuItem2_Click
        private void menuItem2_Click(object sender, System.EventArgs e)
        {
            if (fpSpread1.Sheets[0].GetColumnAllowAutoSort(0))
            {
                fpSpread1.Sheets[0].SetColumnAllowAutoSort(-1, false);
            }
            else
            {
                fpSpread1.Sheets[0].SetColumnAllowAutoSort(-1, true);
            }
        }
        #endregion

        #region menuItem3_Click - 행추가
        private void menuItem3_Click(object sender, System.EventArgs e)
        {
            InsertRow frm = new InsertRow(txtRowCnt);
            frm.ShowDialog();

            this.Cursor = Cursors.WaitCursor;

            if (txtRowCnt.Text != "")
            {
                int Row = 0;

                if (fpSpread1.Sheets[0].Rows.Count > 0)
                {
                    Row = fpSpread1.Sheets[0].ActiveRowIndex;
                }

                fpSpread1.ActiveSheet.DrawingContainer.Redraw = false;

                for (int i = Row; i < Row + Convert.ToInt32(txtRowCnt.Text); i++)
                {
                    RowInsExec();
                }

                fpSpread1.ActiveSheet.DrawingContainer.Redraw = true;

                if (Convert.ToInt32(txtRowCnt.Text) > 0)
                {
                    if (Row > 0)
                    {
                        fpSpread1.ActiveSheet.SetActiveCell(Row + 1, 1);
                    }
                    else
                    {
                        fpSpread1.ActiveSheet.SetActiveCell(Row, 1);
                    }

                    fpSpread1.ShowActiveCell(FarPoint.Win.Spread.VerticalPosition.Center, FarPoint.Win.Spread.HorizontalPosition.Center);
                }
            }

            this.Cursor = Cursors.Default;
        }
        #endregion

        #region Ctrl+ C, V 설정
        private void fpSpread1_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            try
            {
                if (e.Control && e.KeyCode == Keys.C)
                {
                    e.Handled = true;
                    Clipboard.Clear();
                    //fpSpread1.ClipboardOptions = FarPoint.Win.Spread.ClipboardOptions.AllHeaders;
                    //fpSpread1.Sheets[0].ClipboardCopy(FarPoint.Win.Spread.ClipboardCopyOptions.AsStringSkipHidden);
                    fpSpread1.Sheets[0].ClipboardCopy(ClipboardCopyOptions.All);
                }

                if (e.Control && e.KeyCode == Keys.V)
                {
                    fpSpread1.Sheets[0].ClipboardPaste(ClipboardPasteOptions.Values);

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
                        else
                            DataCount = arrData.Length;

                        if (DataCount > 0)
                        {
                            int STRow = fpSpread1.ActiveSheet.ActiveRowIndex;
                            if (STRow < 0)
                                STRow = 0;

                            int ClipRowCount = STRow + DataCount;
                            if (fpSpread1.Sheets[0].RowCount < DataCount)
                                ClipRowCount = fpSpread1.Sheets[0].RowCount - STRow;

                            for (int i = STRow; i < ClipRowCount; i++)
                            {
                                if (i < fpSpread1.Sheets[0].RowCount
                                    || fpSpread1.Sheets[0].Cells[i, fpSpread1.ActiveSheet.ActiveColumnIndex].Locked != true)
                                {
                                    if (fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text != "I")
                                    {
                                        fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text = "U";
                                    }

                                    //									for(int Colum = fpSpread1.ActiveSheet.ActiveColumnIndex; Colum < fpSpread1.Sheets[0].Columns.Count; Colum ++)
                                    //									{
                                    fpSpread1_ChangeEvent(i, fpSpread1.ActiveSheet.ActiveColumnIndex);
                                    //										fpSpread1_ChangeEvent(i, Colum);
                                    //									}
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

        #region 그리드 ChangeEvent
        protected virtual void fpSpread1_ChangeEvent(int Row, int Col) { }
        private void fpSpread1_Change(object sender, FarPoint.Win.Spread.ChangeEventArgs e)
        {
            fpSpread1_ChangeEvent(e.Row, e.Column);
        }
        #endregion

        #region menuItem5_Click

        private void menuItem5_Click(object sender, System.EventArgs e)
        {
            try
            {
                UIForm.FPMake.ExcelMake(fpSpread1, this.Text.ToString());
                ExcelExe();
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY008", "Excel 저장"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region menuItem6_Click(Grid Print)
        private void menuItem6_Click(object sender, EventArgs e)
        {
            try
            {
                UIForm.FPMake.PrintMake(fpSpread1, this.Text);
                PrintExe();
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY008", "Grid Print"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 찾기
        private void FPCOMM1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.F)
            {
                if (!frm.Created)
                {
                    frm = new FindText(fpSpread1);
                    frm.ShowDialog();
                }
                else
                {
                    frm.Activate();
                }
            }
        }

        private void menuItem7_Click(object sender, EventArgs e)
        {
            if (!frm.Created)
            {
                frm = new FindText(fpSpread1);
                frm.ShowDialog();
            }
            else
            {
                frm.Activate();
            }
        }
        #endregion
    }
}
