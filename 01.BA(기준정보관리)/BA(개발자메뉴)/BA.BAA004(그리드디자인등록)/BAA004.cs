  using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;
using FarPoint.Win;
using FarPoint.Win.Spread;
using FarPoint.Win.Spread.CellType;

namespace BA.BAA004
{
    public partial class BAA004 : UIForm.Buttons
    {
        #region 그리드 디자인 변수 정의
        string[] HeadText = new string[] { "", "" }; // 첫번째 Head Text
        string[] HeadText2 = new string[] { "" }; // 첫번째 Head Text
        string[] TxtAlign = new string[] { "", "" };					// Cell 데이타 정렬방식
        string[] CellType = new string[] { "" };						// CellType 지정
        string[] ComboMsg = new string[] { "" };
        int[] HeadWidth = new int[] { 0, 80 };						// Cell 넓이
        int[] shtTitleSpan = new int[] { 1, 1 };							// TitleSpan(Colspan 2인경우 2개 합함)
        int[] HeaderRowCount = new int[] { 1 };									// Head 수량
        int[] CColor = new int[] { 0, 1 };							// Cell 색상 및 ReadOnly 설정(0:일반, 1:필수, 2:ReadOnly)

        string[] HeadText11 = new string[] { "", "화면명", "그리드명", "설명" }; // 첫번째 Head Text
        string[] HeadText12 = new string[] { "" }; // 첫번째 Head Text
        string[] TxtAlign11 = new string[] { "", "C", "C", "L" };					// Cell 데이타 정렬방식
        string[] CellType11 = new string[] { "","", "", "" };						// CellType 지정
        string[] ComboMsg11 = new string[] { "" };
        int[] HeadWidth11 = new int[] { 0, 70, 80, 150 };						// Cell 넓이
        int[] shtTitleSpan11 = new int[] { 1, 1, 1, 1 };							// TitleSpan(Colspan 2인경우 2개 합함)
        int[] HeaderRowCount11 = new int[] { 1 };									// Head 수량
        int[] CColor11 = new int[] { 0, 4, 4, 4 };							// Cell 색상 및 ReadOnly 설정(0:일반, 1:필수, 2:ReadOnly)

        string SQuery = "";
        int Row = 0;
        #endregion

        #region 그리드 디자인 컨트롤 정의
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private FarPoint.Win.Spread.FpSpread fpSpread1;
        private FarPoint.Win.Spread.SheetView fpSpread1_Sheet1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.GroupBox groupBox9;
        private FarPoint.Win.Spread.FpSpread fpSpread2;
        private FarPoint.Win.Spread.SheetView sheetView1;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.GroupBox groupBox8;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Panel panCopy;
        #endregion

        #region 생성자
        public BAA004()
        {
            InitializeComponent();
        }
        #endregion

        #region DelExec, RowInsExec 행 삭제, 추가
        protected override void DelExec()
        {	
            // 행 삭제
            //UIForm.FPMake.RowRemove(fpSpread1);
            if (fpSpread1.Sheets[0].ColumnHeader.Cells[0, fpSpread1.ActiveSheet.GetSelection(0).Column].Text == "I")
                fpSpread1.Sheets[0].Columns.Remove(fpSpread1.ActiveSheet.GetSelection(0).Column, 1);
            else
                fpSpread1.Sheets[0].ColumnHeader.Cells[0, fpSpread1.ActiveSheet.GetSelection(0).Column].Text = "D";

            for (int i = 1; i < fpSpread1.Sheets[0].Columns.Count; i++)
            {
                fpSpread1.Sheets[0].Cells[6, i].Text = i.ToString();
                if (fpSpread1.Sheets[0].ColumnHeader.Cells[0, i].Text.Trim() == "")
                {
                    fpSpread1.Sheets[0].ColumnHeader.Cells[0, i].Text = "U";
                }
            }
        }
        protected override void RowInsExec()
        {	// 행 추가
            int Col = 1;
            if (fpSpread1.ActiveSheet.GetSelection(0) != null)
                Col = fpSpread1.ActiveSheet.GetSelection(0).Column;

            fpSpread1.Sheets[0].Columns.Add(Col + 1, 1);	//Row 추가

            fpSpread1.Sheets[0].ColumnHeader.Cells[0, Col + 1].Text = "I";
            fpSpread1.Sheets[0].Cells[2, Col + 1].Text = "60";
            fpSpread1.Sheets[0].Cells[3, Col + 1].Text = "왼쪽";
            fpSpread1.Sheets[0].Cells[4, Col + 1].Text = "대문자";
            fpSpread1.Sheets[0].Cells[5, Col + 1].Text = "일반";

            for (int i = 1; i < fpSpread1.Sheets[0].Columns.Count; i++)
            {
                //				if(i > Col)
                //				{
                fpSpread1.Sheets[0].Cells[6, i].Text = i.ToString();

                if (fpSpread1.Sheets[0].ColumnHeader.Cells[0, i].Text.Trim() == "")
                {
                    fpSpread1.Sheets[0].ColumnHeader.Cells[0, i].Text = "U";
                }
                //				}
            }
        }
        #endregion

        #region DeleteExe() 삭제로직
        protected override void DeleteExec()
        {
            if (MessageBox.Show(SystemBase.Base.MessageRtn("SY010"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                string DQuery = "usp_BAA004 'D2', @PFORM_ID='" + txtFormId.Text.ToString() + "', @PGRID_NAME='" + cboGRID_NAME.Text.ToString() + "' ";
                string Msg = SystemBase.DbOpen.TranNonQuery(DQuery, "삭제하였습니다.");
                MessageBox.Show(Msg.ToString(), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                SearchExe();
                fpSp1Search();
            }
        }
        #endregion

        #region NewExec() 그리드 및 그룹박스 초기화
        protected override void NewExec()
        {
            this.panCopy.Visible = false;
            txtFormId.Enabled = true;
            cboGRID_NAME.Enabled = true;
            cboHeadCnt.Enabled = true;

            SystemBase.Validation.GroupBox_Reset(groupBox1);
            frmLoad();
          //  UIForm.FPMake.grdMakeSheet(fpSpread2, HeadText11, shtTitleSpan11, HeadText12, TxtAlign11, HeadWidth11, ComboMsg11, HeaderRowCount11, CellType11, CColor11);//그리드 데이타 리셋
        }
        #endregion

        #region RCopyExec 그리드 Row 복사
        protected override void RCopyExec()
        {
            try
            {
                if (fpSpread1.ActiveSheet.GetSelection(0) == null)
                {
                    MessageBox.Show("복사할 Row를 선택하지 않았습니다.", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    if (fpSpread1.Sheets[0].Columns.Count > 0)
                    {
                        int SelectedColumn = fpSpread1.ActiveSheet.GetSelection(0).Column;

                        int Col = 1;
                        if (fpSpread1.ActiveSheet.GetSelection(0) != null)
                            Col = fpSpread1.ActiveSheet.GetSelection(0).Column;
                        fpSpread1.Sheets[0].Columns.Add(Col + 1, 1);	//Row 추가
                        fpSpread1.Sheets[0].ColumnHeader.Cells[0, Col + 1].Text = "I";

                        for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                        {
                            fpSpread1.Sheets[0].Cells[i, SelectedColumn + 1].Value = fpSpread1.Sheets[0].Cells[i, SelectedColumn].Value;
                        }
                    }
                    else
                    {
                        MessageBox.Show("복사할 데이타가 없습니다.", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY008", "Row 복사"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }
        #endregion

        #region BAA004_Load
        private void BAA004_Load(object sender, System.EventArgs e)
        {
            frmLoad();
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            // 그리드 초기화
            UIForm.FPMake.grdMakeSheet(fpSpread2, HeadText11, shtTitleSpan11, HeadText12, TxtAlign11, HeadWidth11, ComboMsg11, HeaderRowCount11, CellType11, CColor11);//그리드 데이타 리셋

            //콤보박스 값 입력
            cboHeadCnt.ClearItems();
            cboGRID_NAME.ClearItems();
            cboHeadCnt.DataMode = C1.Win.C1List.DataModeEnum.AddItem;
            cboGRID_NAME.DataMode = C1.Win.C1List.DataModeEnum.AddItem;

            this.cboHeadCnt.ColumnHeaders = false; //캡션 안보이게
            this.cboGRID_NAME.ColumnHeaders = false;
            this.cboHeadCnt.AddItem("1");
            this.cboHeadCnt.AddItem("2");
            this.cboHeadCnt.AddItem("3");

            this.cboGRID_NAME.AddItem("fpSpread1");
            this.cboGRID_NAME.AddItem("fpSpread2");
            this.cboGRID_NAME.AddItem("fpSpread3");
            this.cboGRID_NAME.AddItem("fpSpread4");
            this.cboGRID_NAME.AddItem("fpSpread5");
            this.cboGRID_NAME.AddItem("fpSpread6");
            this.cboGRID_NAME.AddItem("fpSpread7");
            this.cboGRID_NAME.AddItem("fpSpread8");
            this.cboGRID_NAME.AddItem("fpSpread9");
            this.cboGRID_NAME.AddItem("fpSpread10");
            
            this.cboHeadCnt.ColumnWidth = 97; //버티컬 스크롤 안생기게 컬럼 사이즈 조절 
            this.cboGRID_NAME.ColumnWidth = 123;

            this.cboHeadCnt.SelectedIndex = 0;
            this.cboGRID_NAME.SelectedIndex = 0;
        }
        #endregion

        #region 폼 Setting
        public void frmLoad()
        {
            //회사코드
            //SystemBase.ComboMake.Combo(cboCOMP_CODE, "usp_CO_COMM_CODE @pTYPE = 'COMM', @pCODE = 'CO010' ,@pCOMP_CODE='SYS'");
            //SystemBase.ComboMake.Combo(cboCOMP_CODE2, "usp_CO_COMM_CODE @pTYPE = 'COMM', @pCODE = 'CO010' ,@pCOMP_CODE='SYS'");

            // 그리드 초기화
            UIForm.FPMake.grdMakeSheet(fpSpread1, HeadText, shtTitleSpan, HeadText2, TxtAlign, HeadWidth, ComboMsg, HeaderRowCount, CellType, CColor);//그리드 데이타 리셋

            //fpSpread1.Sheets[0].ColumnHeader.RowCount = 0;
            fpSpread1.Sheets[0].RowHeader.Columns[0].Width = 80;
            fpSpread1.ActiveSheet.Rows.Count = 8;
            //fpSpread1.Sheets[0].ColumnHeader.Cells[0, 0].Text = "I";
            fpSpread1.Sheets[0].ColumnHeader.Cells[0, 1].Text = "I";
            fpSpread1.ActiveSheet.Rows[0].Visible = false;	//1번째 Cell(프라이머리) 키 숨김 지정

            fpSpread1.Sheets[0].RowHeader.Cells[1, 0].Text = "Head명(1)";
            fpSpread1.Sheets[0].Rows[1].CellType = new TextCellType();
            fpSpread1.Sheets[0].Rows[1].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
            fpSpread1.Sheets[0].Rows[1].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
            fpSpread1.Sheets[0].Rows[1].BackColor = Color.LavenderBlush;

            fpSpread1.Sheets[0].RowHeader.Cells[2, 0].Text = "Head 넓이";
            FarPoint.Win.Spread.CellType.NumberCellType num = new FarPoint.Win.Spread.CellType.NumberCellType();
            num.DecimalSeparator = ".";
            num.DecimalPlaces = 0;
            num.FixedPoint = true;
            num.Separator = ",";
            num.ShowSeparator = true;
            num.MaximumValue = 9999999;
            num.MinimumValue = -9999999;
            fpSpread1.Sheets[0].Rows[2].CellType = num;
            fpSpread1.Sheets[0].Rows[2].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;
            fpSpread1.Sheets[0].Rows[2].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
            fpSpread1.Sheets[0].Rows[2].BackColor = Color.LavenderBlush;

            fpSpread1.Sheets[0].RowHeader.Cells[3, 0].Text = "정렬방법";
            fpSpread1.Sheets[0].Rows[3].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
            fpSpread1.Sheets[0].Rows[3].BackColor = Color.LavenderBlush;

            fpSpread1.Sheets[0].RowHeader.Cells[4, 0].Text = "Data Type";
            fpSpread1.Sheets[0].Rows[4].CellType = new TextCellType();
            fpSpread1.Sheets[0].Rows[4].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
            fpSpread1.Sheets[0].Rows[4].BackColor = Color.LavenderBlush;

            fpSpread1.Sheets[0].RowHeader.Cells[5, 0].Text = "속성";
            fpSpread1.Sheets[0].Rows[5].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
            fpSpread1.Sheets[0].Rows[5].BackColor = Color.LavenderBlush;

            fpSpread1.Sheets[0].RowHeader.Cells[6, 0].Text = "정렬순서";
            fpSpread1.Sheets[0].Rows[6].CellType = num;
            fpSpread1.Sheets[0].Rows[6].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;
            fpSpread1.Sheets[0].Rows[6].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
            fpSpread1.Sheets[0].Rows[6].BackColor = Color.LavenderBlush;

            fpSpread1.Sheets[0].RowHeader.Cells[7, 0].Text = "기타";
            fpSpread1.Sheets[0].Rows[7].CellType = new TextCellType();
            fpSpread1.Sheets[0].Rows[7].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
            fpSpread1.Sheets[0].Rows[7].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
            fpSpread1.Sheets[0].Rows[7].BackColor = Color.LavenderBlush;

            /***********************콤보*************************/
            ComboBoxCellType comboType = new ComboBoxCellType();
            comboType.Items = new string[] { "왼쪽", "가운데", "오른쪽" };
            comboType.ItemData = new string[] { "L", "C", "R" };
            comboType.EditorValue = FarPoint.Win.Spread.CellType.EditorValue.ItemData;
            fpSpread1.Sheets[0].Rows[3].CellType = comboType;

            ComboBoxCellType comboType4 = new ComboBoxCellType();
            comboType4.Items = new string[] { "대문자", "일반", "숫자", "숫자1", "숫자2", "숫자3", "숫자4", "숫자5", "숫자6", "버튼", "체크박스", "체크H", "날짜(년월일)", "날짜(년월)", "날짜(월콤보)", "비밀번호", "콤보", "콤보H", "Not Focus", "Hidden", "MASK", "통화", "링크", "프로그레스", "슬라이드", "퍼센트", "MultiLine" };
            comboType4.ItemData = new string[] { "", "GN", "NM", "NM1", "NM2", "NM3", "NM4", "NM5", "NM6", "BT", "CK", "CH", "DT","DY","DD","PW", "CB", "CBV", "NL", "NLV", "MK", "CC", "HL", "PG", "SC", "PC", "ML" };
            comboType4.EditorValue = FarPoint.Win.Spread.CellType.EditorValue.ItemData;
            fpSpread1.Sheets[0].Rows[4].CellType = comboType4;

            ComboBoxCellType comboType5 = new ComboBoxCellType();
            comboType5.Items = new string[] { "일반", "필수", "읽기전용/필수", "읽기전용", "읽기전용/흰색", "읽기전용/포커스제외", "읽기전용/필수/포커스제외" };
            comboType5.ItemData = new string[] { "0", "1", "2", "3", "4", "5", "6" };
            comboType5.EditorValue = FarPoint.Win.Spread.CellType.EditorValue.ItemData;
            fpSpread1.Sheets[0].Rows[5].CellType = comboType5;
            /***********************콤보*************************/

            //fpSpread1.Sheets[0].Columns.Add(Col+1,1);	//Row 추가

            fpSpread1.Sheets[0].ColumnHeader.Cells[0, 1].Text = "I";
            fpSpread1.Sheets[0].Cells[2, 1].Text = "60";
            fpSpread1.Sheets[0].Cells[3, 1].Text = "왼쪽";
            fpSpread1.Sheets[0].Cells[4, 1].Text = "대문자";
            fpSpread1.Sheets[0].Cells[5, 1].Text = "일반";
            fpSpread1.Sheets[0].Cells[6, 1].Text = "1";

            
        }
        #endregion

        #region SaveExec() 폼에 입력된 데이타 저장 로직
        protected override void SaveExec2()
        {
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))  //필수여부체크
            {
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("SY048"), SystemBase.Base.MessageRtn("Z0004"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dsMsg == DialogResult.Yes)
                {

                    string RtnMsg = "성공적으로 처리되었습니다.";

                    SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                    SqlCommand cmd = dbConn.CreateCommand();
                    SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);
                    cmd.Transaction = Trans;
                    //cmd.CommandTimeout = 10000;

                    try
                    {
                        for (int i = 1; i < fpSpread1.Sheets[0].Columns.Count; i++)
                        {
                            string Query = "";
                            if (fpSpread1.Sheets[0].ColumnHeader.Cells[0, i].Text == "U")
                            {
                                Query = " usp_BAA004 'U1'";
                                Query = Query + ", @pFORM_ID='" + txtFormId.Text.ToString() + "'";
                                Query = Query + ", @pGRID_NAME='" + cboGRID_NAME.Text.ToString() + "'";
                                Query = Query + ", @pHEAD_CNT='" + cboHeadCnt.Text.ToString() + "'";
                                Query = Query + ", @pDETAIL='" + txtDetail.Text.ToString() + "'";

                                Query = Query + ",@PSEQ='" + fpSpread1.Sheets[0].Cells[0, i].Value + "' ";
                                Query = Query + ",@pHEAD_ONE='" + fpSpread1.Sheets[0].Cells[1, i].Value.ToString().Replace("'", "''") + "' ";

                                if (cboHeadCnt.Text.ToString() == "1")
                                {
                                    Query = Query + ",@pHEAD_WIDTH='" + fpSpread1.Sheets[0].Cells[2, i].Value + "' ";
                                    Query = Query + ",@pDATA_ALIGN='" + fpSpread1.Sheets[0].Cells[3, i].Value + "' ";
                                    Query = Query + ",@pDATA_TYPE='" + fpSpread1.Sheets[0].Cells[4, i].Value + "' ";
                                    Query = Query + ",@pDATA_KIND='" + fpSpread1.Sheets[0].Cells[5, i].Value + "' ";
                                    Query = Query + ",@pDATA_SEQ='" + fpSpread1.Sheets[0].Cells[6, i].Value + "' ";
                                    Query = Query + ",@pETC='" + fpSpread1.Sheets[0].Cells[7, i].Value + "' ";
                                }
                                else if (cboHeadCnt.Text.ToString() == "2")
                                {
                                    Query = Query + ",@pHEAD_TWO='" + fpSpread1.Sheets[0].Cells[2, i].Value.ToString().Replace("'", "''") + "' ";
                                    Query = Query + ",@pHEAD_WIDTH='" + fpSpread1.Sheets[0].Cells[3, i].Value + "' ";
                                    Query = Query + ",@pDATA_ALIGN='" + fpSpread1.Sheets[0].Cells[4, i].Value + "' ";
                                    Query = Query + ",@pDATA_TYPE='" + fpSpread1.Sheets[0].Cells[5, i].Value + "' ";
                                    Query = Query + ",@pDATA_KIND='" + fpSpread1.Sheets[0].Cells[6, i].Value + "' ";
                                    Query = Query + ",@pDATA_SEQ='" + fpSpread1.Sheets[0].Cells[7, i].Value + "' ";
                                    Query = Query + ",@pETC='" + fpSpread1.Sheets[0].Cells[8, i].Value + "' ";
                                }
                                else if (cboHeadCnt.Text.ToString() == "3")
                                {
                                    Query = Query + ",@pHEAD_TWO='" + fpSpread1.Sheets[0].Cells[2, i].Value.ToString().Replace("'", "''") + "' ";
                                    Query = Query + ",@pHEAD_THR='" + fpSpread1.Sheets[0].Cells[3, i].Value.ToString().Replace("'", "''") + "' ";
                                    Query = Query + ",@pHEAD_WIDTH='" + fpSpread1.Sheets[0].Cells[4, i].Value + "' ";
                                    Query = Query + ",@pDATA_ALIGN='" + fpSpread1.Sheets[0].Cells[5, i].Value + "' ";
                                    Query = Query + ",@pDATA_TYPE='" + fpSpread1.Sheets[0].Cells[6, i].Value + "' ";
                                    Query = Query + ",@pDATA_KIND='" + fpSpread1.Sheets[0].Cells[7, i].Value + "' ";
                                    Query = Query + ",@pDATA_SEQ='" + fpSpread1.Sheets[0].Cells[8, i].Value + "' ";
                                    Query = Query + ",@pETC='" + fpSpread1.Sheets[0].Cells[9, i].Value + "' ";
                                }

                                Query = Query + ",@pIN_ID='" + SystemBase.Base.gstrUserID + "' ";

                                cmd.CommandText = Query;
                                cmd.ExecuteNonQuery();
                            }
                            else if (fpSpread1.Sheets[0].ColumnHeader.Cells[0, i].Text == "I")
                            {
                                Query = " usp_BAA004 'I1'";
                                Query = Query + ", @pFORM_ID='" + txtFormId.Text.ToString() + "'";
                                Query = Query + ", @pGRID_NAME='" + cboGRID_NAME.Text.ToString() + "'";
                                Query = Query + ", @pHEAD_CNT='" + cboHeadCnt.Text.ToString() + "'";
                                Query = Query + ", @pDETAIL='" + txtDetail.Text.ToString() + "'";
                                Query = Query + ",@pHEAD_ONE='" + fpSpread1.Sheets[0].Cells[1, i].Value.ToString().Replace("'", "''") + "' ";

                                if (cboHeadCnt.Text.ToString() == "1")
                                {
                                    Query = Query + ",@pHEAD_WIDTH='" + fpSpread1.Sheets[0].Cells[2, i].Value + "' ";
                                    Query = Query + ",@pDATA_ALIGN='" + fpSpread1.Sheets[0].Cells[3, i].Value + "' ";
                                    Query = Query + ",@pDATA_TYPE='" + fpSpread1.Sheets[0].Cells[4, i].Value + "' ";
                                    Query = Query + ",@pDATA_KIND='" + fpSpread1.Sheets[0].Cells[5, i].Value + "' ";
                                    Query = Query + ",@pDATA_SEQ='" + fpSpread1.Sheets[0].Cells[6, i].Value + "' ";
                                    Query = Query + ",@pETC='" + fpSpread1.Sheets[0].Cells[7, i].Value + "' ";
                                }
                                else if (cboHeadCnt.Text.ToString() == "2")
                                {
                                    Query = Query + ",@pHEAD_TWO='" + fpSpread1.Sheets[0].Cells[2, i].Value.ToString().Replace("'", "''") + "' ";
                                    Query = Query + ",@pHEAD_WIDTH='" + fpSpread1.Sheets[0].Cells[3, i].Value + "' ";
                                    Query = Query + ",@pDATA_ALIGN='" + fpSpread1.Sheets[0].Cells[4, i].Value + "' ";
                                    Query = Query + ",@pDATA_TYPE='" + fpSpread1.Sheets[0].Cells[5, i].Value + "' ";
                                    Query = Query + ",@pDATA_KIND='" + fpSpread1.Sheets[0].Cells[6, i].Value + "' ";
                                    Query = Query + ",@pDATA_SEQ='" + fpSpread1.Sheets[0].Cells[7, i].Value + "' ";
                                    Query = Query + ",@pETC='" + fpSpread1.Sheets[0].Cells[8, i].Value + "' ";
                                }
                                else if (cboHeadCnt.Text.ToString() == "3")
                                {
                                    Query = Query + ",@pHEAD_TWO='" + fpSpread1.Sheets[0].Cells[2, i].Value.ToString().Replace("'", "''") + "' ";
                                    Query = Query + ",@pHEAD_THR='" + fpSpread1.Sheets[0].Cells[3, i].Value.ToString().Replace("'", "''") + "' ";
                                    Query = Query + ",@pHEAD_WIDTH='" + fpSpread1.Sheets[0].Cells[4, i].Value + "' ";
                                    Query = Query + ",@pDATA_ALIGN='" + fpSpread1.Sheets[0].Cells[5, i].Value + "' ";
                                    Query = Query + ",@pDATA_TYPE='" + fpSpread1.Sheets[0].Cells[6, i].Value + "' ";
                                    Query = Query + ",@pDATA_KIND='" + fpSpread1.Sheets[0].Cells[7, i].Value + "' ";
                                    Query = Query + ",@pDATA_SEQ='" + fpSpread1.Sheets[0].Cells[8, i].Value + "' ";
                                    Query = Query + ",@pETC='" + fpSpread1.Sheets[0].Cells[9, i].Value + "' ";
                                }

                                Query = Query + ",@pIN_ID='" + SystemBase.Base.gstrUserID + "' ";

                                cmd.CommandText = Query;
                                cmd.ExecuteNonQuery();
                            }
                            else if (fpSpread1.Sheets[0].ColumnHeader.Cells[0, i].Text == "D")
                            {
                                Query = " usp_BAA004 'D1' ";
                                Query = Query + ", @pFORM_ID='" + txtFormId.Text.ToString() + "'";
                                Query = Query + ", @pGRID_NAME='" + cboGRID_NAME.Text.ToString() + "'";
                                Query = Query + ",@PSEQ='" + fpSpread1.Sheets[0].Cells[0, i].Value + "' ";

                                cmd.CommandText = Query;
                                cmd.ExecuteNonQuery();
                            }

                        }
                        Trans.Commit();
                        fpSp1Search();
                    }
                    catch (Exception f)
                    {
                        Trans.Rollback();
                        RtnMsg = "에러가 발생되어 롤백되었습니다.\n\r\n\r" + f.ToString();
                    }
                    dbConn.Close();

                    MessageBox.Show(SystemBase.Base.MessageRtn(RtnMsg), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            SearchExe();
        }

        public void SearchExe()
        {
            this.panCopy.Visible = false;
            SQuery = " usp_BAA004 'S1', @PFORM_ID='" + C1TextBox.Text.ToString() + "'";

            UIForm.FPMake.grdMakeSheet(fpSpread2, SQuery, HeadText11, shtTitleSpan11, HeadText12, TxtAlign11, HeadWidth11, ComboMsg11, HeaderRowCount11, CellType11, CColor11, 0, 0, false);

            fpSpread2.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.SingleSelect;
        }
        #endregion

        #region fpSpread1_Change 데이타 수정시 U 플래그 등록
        private void fpSpread1_Change(object sender, FarPoint.Win.Spread.ChangeEventArgs e)
        {
            try
            {
                if (fpSpread1.Sheets[0].ColumnHeader.Cells[0, e.Column].Text != "I")
                    fpSpread1.Sheets[0].ColumnHeader.Cells[0, e.Column].Text = "U";
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY008", "수정 플래그 등록"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }
        #endregion

        #region ExcelExec() Excel 저장
        protected override void ExcelExec()
        {
            UIForm.FPMake.ExcelMake(fpSpread1, this.Text.ToString());
        }
        #endregion

        #region fpSpread2_CellClick
        private void fpSpread2_CellClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            try
            {
                Row = e.Row;
                fpSp1Search();
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY013"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        public void fpSp1Search()
        {
            try
            {
                if (fpSpread2.Sheets[0].Rows.Count > 0)
                {
                    frmLoad();
                    fpSpread1.Sheets[0].ColumnHeader.Cells[0, 1].Text = " ";

                   

                    SQuery = " usp_BAA004 'S2', @PFORM_ID='" + fpSpread2.Sheets[0].Cells[Row, 1].Value + "', @PGRID_NAME='" + fpSpread2.Sheets[0].Cells[Row, 2].Value + "' ";
                    DataSet ds = SystemBase.DbOpen.NoTranDataSet(SQuery);
                    
                    //cboCOMP_CODE2.SelectedValue = ds.Tables[1].Rows[0][0].ToString();  
                    txtFormId.Value = ds.Tables[1].Rows[0][0].ToString();
                    cboGRID_NAME.Text = ds.Tables[1].Rows[0][1].ToString();
                    cboHeadCnt.Text = ds.Tables[1].Rows[0][2].ToString();
                    txtDetail.Value = ds.Tables[1].Rows[0][3].ToString();

                    txtFormId.Enabled = false;
                    cboGRID_NAME.Enabled = false;
                    //cboCOMP_CODE2.Enabled = false;
                    //cboHeadCnt.Enabled = false;

                    HeadChange();

                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        if (i + 1 == fpSpread1.Sheets[0].Columns.Count)
                        {
                            fpSpread1.Sheets[0].Columns.Add(i + 1, 1);	//Row 추가
                            fpSpread1.Sheets[0].ColumnHeader.Cells[0, i + 1].Text = " ";
                        }

                        for (int j = 0; j < ds.Tables[0].Columns.Count; j++)
                        {
                            fpSpread1.Sheets[0].Cells[j, i + 1].Value = ds.Tables[0].Rows[i][j].ToString();
                        }

                        if (Convert.ToInt32(cboHeadCnt.SelectedIndex) == 0)
                        {
                            fpSpread1.Sheets[0].Columns[i + 1].Width = Convert.ToInt32(fpSpread1.Sheets[0].Cells[2, i + 1].Value.ToString());
                        }
                        else if (Convert.ToInt32(cboHeadCnt.SelectedIndex) == 1)
                        {
                            fpSpread1.Sheets[0].Columns[i + 1].Width = Convert.ToInt32(fpSpread1.Sheets[0].Cells[3, i + 1].Value.ToString());
                        }
                        else if (Convert.ToInt32(cboHeadCnt.SelectedIndex) == 2)
                        {
                            fpSpread1.Sheets[0].Columns[i + 1].Width = Convert.ToInt32(fpSpread1.Sheets[0].Cells[4, i + 1].Value.ToString());
                        }
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY013"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }
        #endregion

        #region fpSpread1_ColumnWidthChanged
        private void fpSpread1_ColumnWidthChanged(object sender, FarPoint.Win.Spread.ColumnWidthChangedEventArgs e)
        {
            for (int i = 1; i < fpSpread1.Sheets[0].Columns.Count; i++)
            {
                if (fpSpread1.Sheets[0].ColumnHeader.Cells[0, i].Text != "I")
                    fpSpread1.Sheets[0].ColumnHeader.Cells[0, i].Text = "U";
                if (cboHeadCnt.Text.ToString() == "1")
                    fpSpread1.Sheets[0].Cells[2, i].Text = fpSpread1.Sheets[0].ColumnHeader.Cells[0, i].Column.Width.ToString();
                else if (cboHeadCnt.Text.ToString() == "2")
                    fpSpread1.Sheets[0].Cells[3, i].Text = fpSpread1.Sheets[0].ColumnHeader.Cells[0, i].Column.Width.ToString();
                else if (cboHeadCnt.Text.ToString() == "3")
                    fpSpread1.Sheets[0].Cells[4, i].Text = fpSpread1.Sheets[0].ColumnHeader.Cells[0, i].Column.Width.ToString();
            }
        }
        #endregion

        #region 헤드 수 변경 이벤트
        public void HeadChange()
        {
            if (Convert.ToInt32(cboHeadCnt.SelectedIndex.ToString()) == 0)
            {
                if (fpSpread1.Sheets[0].Rows.Count == 9)
                {
                    fpSpread1.Sheets[0].Rows.Remove(2, 1);
                }
                else if (fpSpread1.Sheets[0].Rows.Count == 10)
                {
                    fpSpread1.Sheets[0].Rows.Remove(2, 2);
                }

            }
            else if (Convert.ToInt32(cboHeadCnt.SelectedIndex.ToString()) == 1)
            {
                if (fpSpread1.Sheets[0].Rows.Count == 8)
                {
                    fpSpread1.Sheets[0].Rows.Add(2, 1);
                    fpSpread1.Sheets[0].RowHeader.Cells[2, 0].Text = "Head명(2)";
                    fpSpread1.Sheets[0].Rows[2].BackColor = Color.LavenderBlush;
                    fpSpread1.Sheets[0].Rows[2].CellType = new TextCellType();
                    fpSpread1.Sheets[0].Rows[2].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
                    fpSpread1.Sheets[0].Rows[2].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
                }
                else if (fpSpread1.Sheets[0].Rows.Count == 10)
                {
                    fpSpread1.Sheets[0].Rows.Remove(3, 1);
                }

            }
            else if (Convert.ToInt32(cboHeadCnt.SelectedIndex.ToString()) == 2)
            {
                if (fpSpread1.Sheets[0].Rows.Count == 8)
                {
                    fpSpread1.Sheets[0].Rows.Add(2, 2);
                    fpSpread1.Sheets[0].RowHeader.Cells[2, 0].Text = "Head명(2)";
                    fpSpread1.Sheets[0].Rows[2].BackColor = Color.LavenderBlush;
                    fpSpread1.Sheets[0].Rows[2].CellType = new TextCellType();
                    fpSpread1.Sheets[0].Rows[2].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
                    fpSpread1.Sheets[0].Rows[2].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;

                    fpSpread1.Sheets[0].RowHeader.Cells[3, 0].Text = "Head명(3)";
                    fpSpread1.Sheets[0].Rows[3].BackColor = Color.LavenderBlush;
                    fpSpread1.Sheets[0].Rows[3].CellType = new TextCellType();
                    fpSpread1.Sheets[0].Rows[3].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
                    fpSpread1.Sheets[0].Rows[3].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;

                }
                else if (fpSpread1.Sheets[0].Rows.Count == 9)
                {
                    fpSpread1.Sheets[0].Rows.Add(3, 1);
                    fpSpread1.Sheets[0].RowHeader.Cells[3, 0].Text = "Head명(3)";
                    fpSpread1.Sheets[0].Rows[3].BackColor = Color.LavenderBlush;
                    fpSpread1.Sheets[0].Rows[3].CellType = new TextCellType();
                    fpSpread1.Sheets[0].Rows[3].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
                    fpSpread1.Sheets[0].Rows[3].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;

                }
            }
        }
        #endregion

        #region 취소
        private void button2_Click(object sender, System.EventArgs e)
        {
            this.panCopy.Visible = false;
        }
        #endregion

        #region 현재값으로 이후 컬럼 복사
        private void button1_Click(object sender, System.EventArgs e)
        {
            this.panCopy.Visible = false;
            fpSpread1.Focus();
            object val = fpSpread1.Sheets[0].ActiveCell.Value;
            for (int i = fpSpread1.Sheets[0].ActiveColumnIndex; i < fpSpread1.Sheets[0].ColumnCount; i++)
            {
                fpSpread1.Sheets[0].SetValue(fpSpread1.Sheets[0].ActiveRowIndex, i, val);
            }
        }
        #endregion

        #region fpSpread1_CellClick
        private void fpSpread1_CellClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.ColumnHeader == false && e.RowHeader == false)
            {
                fpSpread1.Sheets[0].ActiveRowIndex = e.Row;
                fpSpread1.Sheets[0].ActiveColumnIndex = e.Column;
                this.panCopy.Location = new Point(e.X + 5, e.Y + 25);
                this.panCopy.Visible = true;
            }
            else
            {
                this.panCopy.Visible = false;
            }
        }
        #endregion

        #region cboHeadCnt_RowChange
        private void cboHeadCnt_RowChange(object sender, EventArgs e)
        {
            HeadChange();
        }
        #endregion
    }
}
