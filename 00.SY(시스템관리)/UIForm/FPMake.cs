using System;
using System.Drawing;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using FarPoint.Win;
using FarPoint.Win.Spread;
using FarPoint.Win.Spread.CellType;
using System.Collections.Generic;

namespace UIForm
{
    public class FPMake
    {
        public FPMake() { }

        #region grdMakeSheet(11) - 그리드 초기화 후 데이타 조회
        public static void grdMakeSheet(FarPoint.Win.Spread.FpSpread baseGrid
            , string Query
            , string[] sheetTitle
            , int[] shtTitleSpan
            , string[] sheetTitle2
            , string[] sheetAlign
            , int[] sheetWidth
            , string[] shtComboBoxMsg
            , int[] shtHeaderRowCount
            , string[] shtCellType
            , int[] shtCellColor
            )
        {
            //baseGrid.Sheets[0].Dispose();
            //baseGrid.Sheets[0].Reset();


            DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);

            baseGrid.Sheets[0].DataSource = dt;
            baseGrid.ActiveSheet.Rows.Count = dt.Rows.Count;//그리드Row수 지정
            baseGrid.Font = new System.Drawing.Font("돋움", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));

            //****************************************************************************************************************
            //: 버튼 설정
            //****************************************************************************************************************
            FarPoint.Win.Spread.CellType.ButtonCellType btnType = new FarPoint.Win.Spread.CellType.ButtonCellType();
            btnType.Text = "...";

            //****************************************************************************************************************
            //: Key Enter시 Next Col로 Focus 이동
            //****************************************************************************************************************
            FarPoint.Win.Spread.InputMap im = baseGrid.GetInputMap(FarPoint.Win.Spread.InputMapMode.WhenFocused);
            im.Put(new FarPoint.Win.Spread.Keystroke(Keys.Enter, Keys.None), FarPoint.Win.Spread.SpreadActions.MoveToNextColumnWrap);


            int sheetLength = 0, shtTitleSpanTmp = 0, shtTitleSpanTmp2 = 0;
            sheetLength = sheetTitle.Length;

            if (sheetTitle.Length < sheetTitle2.Length)
                sheetLength = sheetTitle2.Length;

            //--------------------------------------------------------------------------------------------------------------------------

            for (int i = 0; i < dt.Columns.Count; i++)
            {
                //****************************************************************************************************************
                //: Column 폭 넓이 설정
                //****************************************************************************************************************
                if (sheetWidth.Length > i)
                    baseGrid.Sheets[0].Columns[i].Width = sheetWidth[i];

                //****************************************************************************************************************
                //: columnSpan 설정
                //****************************************************************************************************************
                if (shtTitleSpanTmp == i && shtTitleSpan.Length > 0)
                {
                    baseGrid.Sheets[0].ColumnHeader.Cells[0, i].ColumnSpan = shtTitleSpan[shtTitleSpanTmp2];
                    shtTitleSpanTmp = shtTitleSpanTmp + shtTitleSpan[shtTitleSpanTmp2];
                    shtTitleSpanTmp2++;
                }

                //****************************************************************************************************************
                //: sheet 1번째 Header Text 입력
                //****************************************************************************************************************
                if (sheetTitle.Length > i)
                    baseGrid.Sheets[0].ColumnHeader.Cells[0, i].Text = sheetTitle[i].ToString();

                //****************************************************************************************************************
                //: sheet 2번째 Header Text 입력
                //****************************************************************************************************************
                if (sheetTitle2.Length > i && baseGrid.Sheets[0].ColumnHeader.RowCount > 1)
                    baseGrid.Sheets[0].ColumnHeader.Cells[1, i].Text = sheetTitle2[i].ToString();

                /********************** Column Type 설정 **************************************/
                if (shtCellType.Length > i)
                {
                    string CellType = "";
                    if (shtCellType[i].Length > 2)
                        CellType = shtCellType[i].Substring(0, 2);
                    else
                        CellType = shtCellType[i];

                    switch (CellType)
                    {
                        case "BT":	// Button 셀 설정
                            baseGrid.Sheets[0].Columns[i].CellType = btnType;
                            break;
                        case "CK":	// CheckBox 셀 설정
                            baseGrid.Sheets[0].Columns[i].CellType = new CheckBoxCellType();
                            break;
                        case "DT":	// Date 셀 설정(년월일)
                            baseGrid.Sheets[0].Columns[i].CellType = new DateTimeCellType();
                            break;
                        case "DM":	// Date 셀 설정(년월)                            
                            FarPoint.Win.Spread.CellType.DateTimeCellType UseDt = new FarPoint.Win.Spread.CellType.DateTimeCellType();
                            UseDt.DateTimeFormat = FarPoint.Win.Spread.CellType.DateTimeFormat.UserDefined;
                            UseDt.UserDefinedFormat = "yyyy/MM";
                            baseGrid.Sheets[0].Columns[i].CellType = UseDt;
                            break;
                        case "DY":	// Date 셀 설정(년)                            
                            FarPoint.Win.Spread.CellType.DateTimeCellType UseDy = new FarPoint.Win.Spread.CellType.DateTimeCellType();
                            UseDy.DateTimeFormat = FarPoint.Win.Spread.CellType.DateTimeFormat.UserDefined;
                            UseDy.UserDefinedFormat = "yyyy/MM";
                            baseGrid.Sheets[0].Columns[i].CellType = UseDy;
                            break;
                        case "DD":	// Date 셀 설정(월)                            
                            FarPoint.Win.Spread.CellType.DateTimeCellType UseDd = new FarPoint.Win.Spread.CellType.DateTimeCellType();
                            UseDd.DateTimeFormat = FarPoint.Win.Spread.CellType.DateTimeFormat.UserDefined;
                            UseDd.UserDefinedFormat = "yyyy/MM";
                            baseGrid.Sheets[0].Columns[i].CellType = UseDd;
                            break;
                        case "CC":	// Currency 셀 설정
                            baseGrid.Sheets[0].Columns[i].CellType = new CurrencyCellType();
                            break;
                        case "MK":	// MaskCellType 셀 설정
                            FarPoint.Win.Spread.CellType.MaskCellType picType = new FarPoint.Win.Spread.CellType.MaskCellType();
                            if (shtCellType[i].Length > 2)
                                picType.Mask = shtCellType[i].Substring(2, shtCellType[i].Length - 2).ToString();
                            picType.MaskChar = Convert.ToChar("_");
                            baseGrid.Sheets[0].Columns[i].CellType = picType;
                            break;
                        case "PW":	// Password 셀 설정
                            FarPoint.Win.Spread.CellType.TextCellType passType = new FarPoint.Win.Spread.CellType.TextCellType();
                            passType.PasswordChar = Convert.ToChar("*");
                            baseGrid.Sheets[0].Columns[i].CellType = passType;
                            break;
                        case "NM":	// Number 셀 설정
                            int Place = 0;
                            if (shtCellType[i].Length == 3)
                                Place = Convert.ToInt32(shtCellType[i].Substring(2, 1).ToString());
                            FarPoint.Win.Spread.CellType.NumberCellType num = new FarPoint.Win.Spread.CellType.NumberCellType();
                            //num.LeadingZero = FarPoint.Win.Spread.CellType.LeadingZero.No;
                            num.DecimalSeparator = ".";
                            num.DecimalPlaces = Place;
                            num.FixedPoint = true;
                            num.Separator = ",";
                            num.ShowSeparator = true;
                            num.MaximumValue = 99999999999999;
                            num.MinimumValue = -99999999999999;
                            baseGrid.Sheets[0].Columns[i].CellType = num;
                            break;
                        case "HL":	// HyperLink 셀 설정
                            baseGrid.Sheets[0].Columns[i].CellType = new HyperLinkCellType();
                            break;
                        case "PG":	// Progress 셀 설정
                            baseGrid.Sheets[0].Columns[i].CellType = new ProgressCellType();
                            break;
                        case "RT":	// RichText 셀 설정
                            baseGrid.Sheets[0].Columns[i].CellType = new RichTextCellType();
                            break;
                        case "SC":	// SliderCell 셀 설정
                            baseGrid.Sheets[0].Columns[i].CellType = new SliderCellType();
                            break;
                        case "NL":	// 필수입력 사항 체크하지 않는 로직을 탄다
                            baseGrid.Sheets[0].Columns[i].CellType = new GeneralCellType();
                            baseGrid.Sheets[0].Columns[i].CanFocus = false;
                            if (shtCellType[i].ToString() == "NLV") // NLV인 경우 숨김
                                baseGrid.ActiveSheet.Columns[i].Visible = false;
                            break;
                        case "PC":	// Percent 셀 설정
                            baseGrid.Sheets[0].Columns[i].CellType = new PercentCellType();
                            break;
                        case "CB":	// ComboBox 셀 설정
                            for (int k = 0; k < shtComboBoxMsg.Length; k++)
                            {
                                if (i == Convert.ToInt32(shtComboBoxMsg[k].Substring(0, shtComboBoxMsg[k].IndexOf(":"))))
                                {
                                    ComboBoxCellType comboType = new ComboBoxCellType();
                                    comboType.MaxDrop = 20;

                                    Regex rx = new Regex("#");
                                    string Tmp = shtComboBoxMsg[k].Substring(shtComboBoxMsg[k].IndexOf(":") + 1, shtComboBoxMsg[k].Length - shtComboBoxMsg[k].IndexOf(":") - 1);

                                    string Tmp1 = "";
                                    string Tmp2 = "";
                                    if (Tmp.IndexOf("|") > 0)
                                    {
                                        Tmp1 = Tmp.Substring(0, Tmp.IndexOf("|"));
                                        Tmp2 = Tmp.Substring(Tmp.IndexOf("|") + 1, Tmp.Length - Tmp.IndexOf("|") - 1);
                                        string[] cboMsg1 = rx.Split(Tmp1);
                                        string[] cboMsg2 = rx.Split(Tmp2);

                                        comboType.ItemData = cboMsg1;		// Value
                                        comboType.Items = cboMsg2;	// Key, Text
                                        comboType.EditorValue = FarPoint.Win.Spread.CellType.EditorValue.ItemData;
                                        baseGrid.Sheets[0].Columns[i].CellType = comboType;
                                    }
                                    else
                                    {
                                        string[] cboMsg1 = rx.Split(Tmp);

                                        comboType.Items = cboMsg1;
                                        baseGrid.Sheets[0].Columns[i].CellType = comboType;
                                    }
                                    if (shtCellType[i].ToString() == "CBV") // NLV인 경우 숨김
                                        baseGrid.ActiveSheet.Columns[i].Visible = false;

                                }
                            }
                            break;
                        case "ML": //TextBox MultiLine
                            FarPoint.Win.Spread.CellType.TextCellType MultiType = new FarPoint.Win.Spread.CellType.TextCellType();
                            MultiType.Multiline = true;

                            baseGrid.Sheets[0].Columns[i].CellType = MultiType;
                            break;

                        default:	// General 설정
                            baseGrid.Sheets[0].Columns[i].CellType = new TextCellType();
                            break;
                    }
                }
                /****************************************************************************/

                /********************** Column 정렬 설정 **************************************/
                if (sheetAlign.Length > i)
                {
                    switch (sheetAlign[i])
                    {
                        case "L":	// 왼쪽 정렬 MIDDLE
                            baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
                            baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
                            break;
                        case "LT":	// 왼쪽 정렬 TOP
                            baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
                            baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                            break;
                        case "LB":	// 왼쪽 정렬 BOTTOM
                            baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
                            baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Bottom;
                            break;
                        case "C": // 가운데 정렬
                            baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
                            baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
                            break;
                        case "CT":	// 가운데 정렬 TOP
                            baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
                            baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                            break;
                        case "CB":	// 가운데 정렬 BOTTOM
                            baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
                            baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Bottom;
                            break;
                        case "R":	// 오른쪽 정렬
                            baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;
                            baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
                            break;
                        case "RT":	// 오른쪽 정렬 TOP
                            baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;
                            baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                            break;
                        case "RB":	// 오른쪽 정렬 BOTTOM
                            baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;
                            baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Bottom;
                            break;
                        default:	// 디폴트 왼쪽 정렬
                            baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
                            baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
                            break;
                    }
                }
                /****************************************************************************/

                /********************** Column 색상 및 Lock 설정 **************************************/
                if (shtCellColor.Length > i)
                {
                    switch (shtCellColor[i])
                    {
                        case 0:	// 일반
                            baseGrid.Sheets[0].Columns[i].BackColor = Color.White;
                            baseGrid.Sheets[0].Columns[i].Locked = false;
                            break;
                        case 1:	// 필수입력
                            baseGrid.Sheets[0].Columns[i].BackColor = SystemBase.Validation.Kind_LightCyan; //System.Drawing.Color.FromArgb(242, 252, 254);
                            baseGrid.Sheets[0].Columns[i].Locked = false;
                            break;
                        case 2:	// 읽기전용, 필수
                            baseGrid.Sheets[0].Columns[i].BackColor = System.Drawing.Color.FromArgb(239, 239, 239);
                            baseGrid.Sheets[0].Columns[i].Locked = true;
                            break;
                        case 3:	// 읽기전용, 필수항목에서 제외
                            baseGrid.Sheets[0].Columns[i].BackColor = System.Drawing.Color.FromArgb(238, 238, 238);
                            baseGrid.Sheets[0].Columns[i].Locked = true;
                            break;
                        case 4:	// 읽기전용 & ReadOnly & White
                            baseGrid.Sheets[0].Columns[i].BackColor = Color.White;
                            baseGrid.Sheets[0].Columns[i].Locked = true;
                            break;
                        case 5:	// 읽기전용, 필수항목에서 제외, Focus 제외
                            baseGrid.Sheets[0].Columns[i].BackColor = System.Drawing.Color.FromArgb(238, 238, 238);
                            baseGrid.Sheets[0].Columns[i].Locked = true;
                            baseGrid.Sheets[0].Columns[i].CanFocus = false;
                            break;
                        case 6:	// 읽기전용, 필수항목, Focus 제외
                            baseGrid.Sheets[0].Columns[i].BackColor = System.Drawing.Color.FromArgb(239, 239, 239);
                            baseGrid.Sheets[0].Columns[i].Locked = true;
                            baseGrid.Sheets[0].Columns[i].CanFocus = false;
                            break;
                        default:	// 일반
                            baseGrid.Sheets[0].Columns[i].BackColor = Color.White;
                            baseGrid.Sheets[0].Columns[i].Locked = false;
                            break;
                    }
                }
                /********************** Column Type 설정 **************************************/
            }
        }
        #endregion

        #region grdMakeSheet(12) - 그리드 초기화 후 데이타 조회 (팝업전용)
        public static void grdMakeSheet(FarPoint.Win.Spread.FpSpread baseGrid
            , string Query
            , string[] sheetTitle
            , int[] shtTitleSpan
            , string[] sheetTitle2
            , string[] sheetAlign
            , int[] sheetWidth
            , string[] shtComboBoxMsg
            , int[] shtHeaderRowCount
            , string[] shtCellType
            , int[] shtCellColor
            , int shtOperationMode
            )
        {

            baseGrid.Font = new System.Drawing.Font("돋움", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            /********************** 스크롤바 정의 **************************************/
            baseGrid.HorizontalScrollBarPolicy = FarPoint.Win.Spread.ScrollBarPolicy.AsNeeded;
            /********************** 스크롤바 정의 **************************************/
            baseGrid.VerticalScrollBarPolicy = FarPoint.Win.Spread.ScrollBarPolicy.AsNeeded;
            /********************** 그리드 칼럼수 지정 **************************************/
            baseGrid.ActiveSheet.Columns.Count = sheetTitle.Length;
            /********************** 그리드 Row수 지정 **************************************/
            baseGrid.ActiveSheet.Rows.Count = 0;
            /********************** 그리드 Row색상 지정 **************************************/
            baseGrid.Sheets[0].AlternatingRows[0].BackColor = Color.FromName("WhiteSmoke");
            /********************** 그리드 Head 높이 지정 **************************************/
            baseGrid.Sheets[0].ColumnHeader.Rows[0].Height = 28;
            /********************** 그리드 바탕색상 지정 **************************************/
            baseGrid.Sheets[0].GrayAreaBackColor = Color.FromName("white");
            /********************** 1Row 선택 기준(단일, 1Row별, 1Cell별) **************************************/
            if (shtOperationMode == 1)
                baseGrid.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.RowMode;				//.ExtendedSelect;
            else if (shtOperationMode == 2)
                baseGrid.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.ReadOnly;				//.ExtendedSelect;
            else if (shtOperationMode == 3)
                baseGrid.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.MultiSelect;				//.ExtendedSelect;
            else if (shtOperationMode == 4)
                baseGrid.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.SingleSelect;				//.ExtendedSelect;
            else
                baseGrid.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.Normal;				//.ExtendedSelect;

            /********************** ColumnHeader 수 지정**************************************/
            baseGrid.Sheets[0].ColumnHeader.RowCount = shtHeaderRowCount[0];
            /********************** HorizontalGridLine 가로 가이드라인 색상 지정**************************************/
            baseGrid.Sheets[0].HorizontalGridLine = new FarPoint.Win.Spread.GridLine(FarPoint.Win.Spread.GridLineType.Flat, Color.FromArgb(193, 201, 212));
            /********************** VerticalGridLine 세로 가이드라인 색상 지정**************************************/
            baseGrid.Sheets[0].VerticalGridLine = new FarPoint.Win.Spread.GridLine(FarPoint.Win.Spread.GridLineType.Flat, Color.FromArgb(193, 201, 212));
           
            /********************** 왼쪽 상단 코너 색상 지정**************************************/
            baseGrid.Sheets[0].SheetCornerStyle.BackColor = System.Drawing.Color.FromArgb(242, 244, 246); // .FromName("Beige");
            /********************** 첫번째 Head color지정**************************************/
            baseGrid.Sheets[0].ColumnHeader.Rows[0].BackColor = System.Drawing.Color.FromArgb(242, 244, 246); // .FromName("Beige");
            /********************** 두번째 Head color지정**************************************/
            if (baseGrid.Sheets[0].ColumnHeader.RowCount > 1)
                baseGrid.Sheets[0].ColumnHeader.Rows[1].BackColor = System.Drawing.Color.FromArgb(242, 244, 246); // .FromName("Beige");
            /********************** 첫번째 Colum color 지정**************************************/
            baseGrid.Sheets[0].RowHeader.Columns[0].BackColor = System.Drawing.Color.FromArgb(242, 244, 246); // .FromName("Beige");
            /********************** 1번째 Cell(프라이머리) 키 숨김 지정**************************************/
            baseGrid.ActiveSheet.Columns[0].Visible = false;
            /********************** 입력시 기존 데이타 삭제후 입력**************************************/
            baseGrid.EditModeReplace = true;
            /********************** Clipboard 복사시 Head값 미포함**************************************/
            baseGrid.ClipboardOptions = FarPoint.Win.Spread.ClipboardOptions.NoHeaders;
            //********************** 그리드 focus 칼라***************************************************/
            baseGrid.FocusRenderer = new FarPoint.Win.Spread.SolidFocusIndicatorRenderer(Color.CornflowerBlue, 2);
            //********************** Clipboard False***********************************/
            baseGrid.AutoClipboard = false;

            DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);

            baseGrid.Sheets[0].DataSource = dt;
            baseGrid.ActiveSheet.Rows.Count = dt.Rows.Count;//그리드Row수 지정


            //****************************************************************************************************************
            //:버튼 설정
            //****************************************************************************************************************
            FarPoint.Win.Spread.CellType.ButtonCellType btnType = new FarPoint.Win.Spread.CellType.ButtonCellType();
            btnType.Text = "...";

            //baseGrid.ActiveSheet.Columns[0].Visible = false; //프라이머리 숨김


            int sheetLength = 0, shtTitleSpanTmp = 0, shtTitleSpanTmp2 = 0;
            sheetLength = sheetTitle.Length;

            if (sheetTitle.Length < sheetTitle2.Length)
                sheetLength = sheetTitle2.Length;

            //int shtTitleSpanTmp = 0;
            //int shtTitleSpanTmp2 = 0;

            //--------------------------------------------------------------------------------------------------------------------------

            for (int i = 0; i < dt.Columns.Count; i++)
            {
                //****************************************************************************************************************
                //:Column 폭 넓이 설정
                //****************************************************************************************************************
                if (sheetWidth.Length > i)
                    baseGrid.Sheets[0].Columns[i].Width = sheetWidth[i];

                //****************************************************************************************************************
                //:columnSpan 설정
                //****************************************************************************************************************
                if (shtTitleSpanTmp == i && shtTitleSpan.Length > 0)
                {
                    baseGrid.Sheets[0].ColumnHeader.Cells[0, i].ColumnSpan = shtTitleSpan[shtTitleSpanTmp2];
                    shtTitleSpanTmp = shtTitleSpanTmp + shtTitleSpan[shtTitleSpanTmp2];
                    shtTitleSpanTmp2++;
                }

                //****************************************************************************************************************
                //:sheet 1번째 Header Text 입력
                //****************************************************************************************************************
                if (sheetTitle.Length > i)
                    baseGrid.Sheets[0].ColumnHeader.Cells[0, i].Text = sheetTitle[i].ToString();

                //****************************************************************************************************************
                //:sheet 2번째 Header Text 입력
                //****************************************************************************************************************
                if (sheetTitle2.Length > i && baseGrid.Sheets[0].ColumnHeader.RowCount > 1)
                    baseGrid.Sheets[0].ColumnHeader.Cells[1, i].Text = sheetTitle2[i].ToString();

                /********************** Column Type 설정 **************************************/
                if (shtCellType.Length > i)
                {
                    string CellType = "";
                    if (shtCellType[i].Length > 2)
                        CellType = shtCellType[i].Substring(0, 2);
                    else
                        CellType = shtCellType[i];

                    switch (CellType)
                    {
                        case "BT":	// Button 셀 설정
                            baseGrid.Sheets[0].Columns[i].CellType = btnType;
                            break;
                        case "CK":	// CheckBox 셀 설정
                            baseGrid.Sheets[0].Columns[i].CellType = new CheckBoxCellType();
                            break;
                        case "DT":	// Date 셀 설정(년월일)
                            baseGrid.Sheets[0].Columns[i].CellType = new DateTimeCellType();
                            break;
                        case "DM":	// Date 셀 설정(년월)                            
                            FarPoint.Win.Spread.CellType.DateTimeCellType UseDt = new FarPoint.Win.Spread.CellType.DateTimeCellType();
                            UseDt.DateTimeFormat = FarPoint.Win.Spread.CellType.DateTimeFormat.UserDefined;
                            UseDt.UserDefinedFormat = "yyyy/MM";
                            baseGrid.Sheets[0].Columns[i].CellType = UseDt;
                            break;
                        case "DY":	// Date 셀 설정(년)                            
                            FarPoint.Win.Spread.CellType.DateTimeCellType UseDy = new FarPoint.Win.Spread.CellType.DateTimeCellType();
                            UseDy.DateTimeFormat = FarPoint.Win.Spread.CellType.DateTimeFormat.UserDefined;
                            UseDy.UserDefinedFormat = "yyyy/MM";
                            baseGrid.Sheets[0].Columns[i].CellType = UseDy;
                            break;
                        case "DD":	// Date 셀 설정(월)                            
                            FarPoint.Win.Spread.CellType.DateTimeCellType UseDd = new FarPoint.Win.Spread.CellType.DateTimeCellType();
                            UseDd.DateTimeFormat = FarPoint.Win.Spread.CellType.DateTimeFormat.UserDefined;
                            UseDd.UserDefinedFormat = "yyyy/MM";
                            baseGrid.Sheets[0].Columns[i].CellType = UseDd;
                            break;
                        case "CC":	// Currency 셀 설정
                            baseGrid.Sheets[0].Columns[i].CellType = new CurrencyCellType();
                            break;
                        case "PW":	// Password 셀 설정
                            FarPoint.Win.Spread.CellType.TextCellType passType = new FarPoint.Win.Spread.CellType.TextCellType();
                            passType.PasswordChar = Convert.ToChar("*");
                            baseGrid.Sheets[0].Columns[i].CellType = passType;
                            break;
                        case "MK":	// MaskCellType 셀 설정
                            FarPoint.Win.Spread.CellType.MaskCellType picType = new FarPoint.Win.Spread.CellType.MaskCellType();
                            if (shtCellType[i].Length > 2)
                                picType.Mask = shtCellType[i].Substring(2, shtCellType[i].Length - 2).ToString();
                            picType.MaskChar = Convert.ToChar("_");
                            baseGrid.Sheets[0].Columns[i].CellType = picType;
                            break;
                        case "NM":	// Number 셀 설정
                            int Place = 0;
                            if (shtCellType[i].Length == 3)
                                Place = Convert.ToInt32(shtCellType[i].Substring(2, 1).ToString());
                            FarPoint.Win.Spread.CellType.NumberCellType num = new FarPoint.Win.Spread.CellType.NumberCellType();
                            //num.LeadingZero = FarPoint.Win.Spread.CellType.LeadingZero.No;
                            num.DecimalSeparator = ".";
                            num.DecimalPlaces = Place;
                            num.FixedPoint = true;
                            num.Separator = ",";
                            num.ShowSeparator = true;
                            num.MaximumValue = 99999999999999;
                            num.MinimumValue = -99999999999999;
                            baseGrid.Sheets[0].Columns[i].CellType = num;
                            break;
                        case "HL":	// HyperLink 셀 설정
                            baseGrid.Sheets[0].Columns[i].CellType = new HyperLinkCellType();
                            break;
                        case "PG":	// Progress 셀 설정
                            baseGrid.Sheets[0].Columns[i].CellType = new ProgressCellType();
                            break;
                        case "RT":	// RichText 셀 설정
                            baseGrid.Sheets[0].Columns[i].CellType = new RichTextCellType();
                            break;
                        case "SC":	// SliderCell 셀 설정
                            baseGrid.Sheets[0].Columns[i].CellType = new SliderCellType();
                            break;
                        case "NL":	// 필수입력 사항 체크하지 않는 로직을 탄다
                            baseGrid.Sheets[0].Columns[i].CellType = new GeneralCellType();
                            baseGrid.Sheets[0].Columns[i].CanFocus = false;
                            if (shtCellType[i].ToString() == "NLV") // NLV인 경우 숨김
                                baseGrid.ActiveSheet.Columns[i].Visible = false;
                            break;
                        case "PC":	// Percent 셀 설정
                            baseGrid.Sheets[0].Columns[i].CellType = new PercentCellType();
                            break;
                        case "CB":	// ComboBox 셀 설정
                            for (int k = 0; k < shtComboBoxMsg.Length; k++)
                            {
                                if (i == Convert.ToInt32(shtComboBoxMsg[k].Substring(0, shtComboBoxMsg[k].IndexOf(":"))))
                                {
                                    ComboBoxCellType comboType = new ComboBoxCellType();
                                    comboType.MaxDrop = 20;
                                    Regex rx = new Regex("#");
                                    string Tmp = shtComboBoxMsg[k].Substring(shtComboBoxMsg[k].IndexOf(":") + 1, shtComboBoxMsg[k].Length - shtComboBoxMsg[k].IndexOf(":") - 1);

                                    string Tmp1 = "";
                                    string Tmp2 = "";
                                    if (Tmp.IndexOf("|") > 0)
                                    {
                                        Tmp1 = Tmp.Substring(0, Tmp.IndexOf("|"));
                                        Tmp2 = Tmp.Substring(Tmp.IndexOf("|") + 1, Tmp.Length - Tmp.IndexOf("|") - 1);
                                        string[] cboMsg1 = rx.Split(Tmp1);
                                        string[] cboMsg2 = rx.Split(Tmp2);

                                        comboType.ItemData = cboMsg1;		// Value
                                        comboType.Items = cboMsg2;	// Key, Text
                                        comboType.EditorValue = FarPoint.Win.Spread.CellType.EditorValue.ItemData;

                                        baseGrid.Sheets[0].Columns[i].CellType = comboType;
                                    }
                                    else
                                    {
                                        string[] cboMsg1 = rx.Split(Tmp);

                                        comboType.Items = cboMsg1;
                                        baseGrid.Sheets[0].Columns[i].CellType = comboType;
                                    }
                                }
                            }
                            break;
                        case "ML": //TextBox MultiLine
                            FarPoint.Win.Spread.CellType.TextCellType MultiType = new FarPoint.Win.Spread.CellType.TextCellType();
                            MultiType.Multiline = true;

                            baseGrid.Sheets[0].Columns[i].CellType = MultiType;
                            break;
                        default:	// General 설정
                            baseGrid.Sheets[0].Columns[i].CellType = new TextCellType();
                            break;
                    }
                }
                /****************************************************************************/

                /********************** Column 정렬 설정 **************************************/
                if (sheetAlign.Length > i)
                {
                    switch (sheetAlign[i])
                    {
                        case "L":	// 왼쪽 정렬 MIDDLE
                            baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
                            baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
                            break;
                        case "LT":	// 왼쪽 정렬 TOP
                            baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
                            baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                            break;
                        case "LB":	// 왼쪽 정렬 BOTTOM
                            baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
                            baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Bottom;
                            break;
                        case "C": // 가운데 정렬
                            baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
                            baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
                            break;
                        case "CT":	// 가운데 정렬 TOP
                            baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
                            baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                            break;
                        case "CB":	// 가운데 정렬 BOTTOM
                            baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
                            baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Bottom;
                            break;
                        case "R":	// 오른쪽 정렬
                            baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;
                            baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
                            break;
                        case "RT":	// 오른쪽 정렬 TOP
                            baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;
                            baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                            break;
                        case "RB":	// 오른쪽 정렬 BOTTOM
                            baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;
                            baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Bottom;
                            break;
                        default:	// 디폴트 왼쪽 정렬
                            baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
                            baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
                            break;
                    }
                }
                /****************************************************************************/

                /********************** Column 색상 및 Lock 설정 **************************************/
                if (shtCellColor.Length > i)
                {
                    switch (shtCellColor[i])
                    {
                        case 0:	// 일반
                            baseGrid.Sheets[0].Columns[i].BackColor = Color.White;
                            break;
                        case 1:	// 필수입력
                            baseGrid.Sheets[0].Columns[i].BackColor = SystemBase.Validation.Kind_LightCyan; //System.Drawing.Color.FromArgb(242, 252, 254);
                            break;
                        case 2:	// 읽기전용
                            baseGrid.Sheets[0].Columns[i].BackColor = Color.White;
                            baseGrid.Sheets[0].Columns[i].Locked = true;
                            break;
                        case 3:	// 읽기전용이면서 필수항목에서 제외
                            baseGrid.Sheets[0].Columns[i].BackColor = System.Drawing.Color.FromArgb(238, 238, 238);
                            baseGrid.Sheets[0].Columns[i].Locked = true;
                            break;
                        case 4:	// 읽기전용 & ReadOnly & White
                            baseGrid.Sheets[0].Columns[i].BackColor = Color.White;
                            baseGrid.Sheets[0].Columns[i].Locked = true;
                            break;
                        case 5:	// 읽기전용, 필수항목에서 제외, Focus 제외
                            baseGrid.Sheets[0].Columns[i].BackColor = System.Drawing.Color.FromArgb(238, 238, 238);
                            baseGrid.Sheets[0].Columns[i].Locked = true;
                            baseGrid.Sheets[0].Columns[i].CanFocus = false;
                            break;
                        case 6:	// 읽기전용, 필수항목, Focus 제외
                            baseGrid.Sheets[0].Columns[i].BackColor = System.Drawing.Color.FromArgb(239, 239, 239);
                            baseGrid.Sheets[0].Columns[i].Locked = true;
                            baseGrid.Sheets[0].Columns[i].CanFocus = false;
                            break;
                        default:	// 일반
                            baseGrid.Sheets[0].Columns[i].BackColor = Color.White;
                            break;
                    }
                }
                /********************** Column Type 설정 **************************************/
            }
        }
        #endregion

        #region grdMakeSheet(6) - 그리드 초기화 후 데이타 조회 (팝업전용)
        public static void grdMakeSheet(FarPoint.Win.Spread.FpSpread baseGrid
            , string Query
            , string[] sheetTitle
            , string[] sheetAlign
            , string[] shtCellType
            , int[] sheetWidth
            )
        {
            DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);
            baseGrid.Sheets[0].DataSource = dt;
            baseGrid.ActiveSheet.Rows.Count = dt.Rows.Count;//그리드Row수 지정

            baseGrid.Font = new System.Drawing.Font("돋움", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            /********************** 스크롤바 정의 **************************************/
            baseGrid.HorizontalScrollBarPolicy = FarPoint.Win.Spread.ScrollBarPolicy.AsNeeded;
            /********************** 스크롤바 정의 **************************************/
            baseGrid.VerticalScrollBarPolicy = FarPoint.Win.Spread.ScrollBarPolicy.AsNeeded;
            /********************** 그리드 칼럼수 지정 **************************************/
            baseGrid.ActiveSheet.Columns.Count = dt.Columns.Count;
            /********************** 그리드 Row색상 지정 **************************************/
            baseGrid.Sheets[0].AlternatingRows[0].BackColor = Color.FromArgb(255, 255, 255);
            baseGrid.Sheets[0].AlternatingRows[1].BackColor = Color.FromArgb(245, 245, 245);
            /********************** 그리드 Head 높이 지정 **************************************/
            baseGrid.Sheets[0].ColumnHeader.Rows[0].Height = 28;
            /********************** 그리드 바탕색상 지정 **************************************/
            baseGrid.Sheets[0].GrayAreaBackColor = Color.FromName("White");
            /********************** 1Row 선택 기준(단일, 1Row별, 1Cell별) **************************************/
            baseGrid.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.SingleSelect;


            /********************** ColumnHeader 수 지정**************************************/
            baseGrid.Sheets[0].ColumnHeader.RowCount = 1;
            /********************** HorizontalGridLine 가로 가이드라인 색상 지정**************************************/
            baseGrid.Sheets[0].HorizontalGridLine = new FarPoint.Win.Spread.GridLine(FarPoint.Win.Spread.GridLineType.Flat, Color.FromArgb(193, 201, 212));
            /********************** VerticalGridLine 세로 가이드라인 색상 지정**************************************/
            baseGrid.Sheets[0].VerticalGridLine = new FarPoint.Win.Spread.GridLine(FarPoint.Win.Spread.GridLineType.Flat, Color.FromArgb(193, 201, 212));
            /********************** 왼쪽 상단 코너 색상 지정**************************************/
            baseGrid.Sheets[0].SheetCornerStyle.BackColor = System.Drawing.Color.FromArgb(242, 244, 246); // .FromName("Beige");
            /********************** 첫번째 Head color지정**************************************/
            baseGrid.Sheets[0].ColumnHeader.Rows[0].BackColor = System.Drawing.Color.FromArgb(242, 244, 246); // .FromName("Beige");
            /********************** 첫번째 Colum color 지정**************************************/
            baseGrid.Sheets[0].RowHeader.Columns[0].BackColor = System.Drawing.Color.FromArgb(242, 244, 246); // .FromName("Beige");
            /********************** 입력시 기존 데이타 삭제후 입력**************************************/
            baseGrid.EditModeReplace = true;
            /********************** Clipboard 복사시 Head값 미포함**************************************/
            baseGrid.ClipboardOptions = FarPoint.Win.Spread.ClipboardOptions.NoHeaders;
            //********************** 그리드 focus 칼라***************************************************/
            baseGrid.FocusRenderer = new FarPoint.Win.Spread.SolidFocusIndicatorRenderer(Color.CornflowerBlue, 2);
            //********************** Clipboard False***********************************/
            baseGrid.AutoClipboard = false;



            //****************************************************************************************************************
            //:버튼 설정
            //****************************************************************************************************************
            FarPoint.Win.Spread.CellType.ButtonCellType btnType = new FarPoint.Win.Spread.CellType.ButtonCellType();
            btnType.Text = "...";


            //--------------------------------------------------------------------------------------------------------------------------

            for (int i = 0; i < dt.Columns.Count; i++)
            {
                //****************************************************************************************************************
                //:Column 폭 넓이 설정
                //****************************************************************************************************************
                if (sheetWidth.Length > i)
                    baseGrid.Sheets[0].Columns[i].Width = sheetWidth[i];


                //****************************************************************************************************************
                //:sheet 1번째 Header Text 입력
                //****************************************************************************************************************
                if (sheetTitle.Length > i)
                    baseGrid.Sheets[0].ColumnHeader.Cells[0, i].Text = sheetTitle[i].ToString();

                /********************** Column Type 설정 **************************************/
                if (shtCellType.Length > i)
                {
                    string CellType = "";
                    if (shtCellType[i].Length > 2)
                        CellType = shtCellType[i].Substring(0, 2);
                    else
                        CellType = shtCellType[i];

                    switch (CellType)
                    {
                        case "BT":	// Button 셀 설정
                            baseGrid.Sheets[0].Columns[i].CellType = btnType;
                            break;
                        case "CK":	// CheckBox 셀 설정
                            baseGrid.Sheets[0].Columns[i].CellType = new CheckBoxCellType();
                            break;
                        case "DT":	// Date 셀 설정(년월일)
                            baseGrid.Sheets[0].Columns[i].CellType = new DateTimeCellType();
                            break;
                        case "DM":	// Date 셀 설정(년월)                            
                            FarPoint.Win.Spread.CellType.DateTimeCellType UseDt = new FarPoint.Win.Spread.CellType.DateTimeCellType();
                            UseDt.DateTimeFormat = FarPoint.Win.Spread.CellType.DateTimeFormat.UserDefined;
                            UseDt.UserDefinedFormat = "yyyy/MM";
                            baseGrid.Sheets[0].Columns[i].CellType = UseDt;
                            break;
                        case "DY":	// Date 셀 설정(년)                            
                            FarPoint.Win.Spread.CellType.DateTimeCellType UseDy = new FarPoint.Win.Spread.CellType.DateTimeCellType();
                            UseDy.DateTimeFormat = FarPoint.Win.Spread.CellType.DateTimeFormat.UserDefined;
                            UseDy.UserDefinedFormat = "yyyy/MM";
                            baseGrid.Sheets[0].Columns[i].CellType = UseDy;
                            break;
                        case "DD":	// Date 셀 설정(월)                            
                            FarPoint.Win.Spread.CellType.DateTimeCellType UseDd = new FarPoint.Win.Spread.CellType.DateTimeCellType();
                            UseDd.DateTimeFormat = FarPoint.Win.Spread.CellType.DateTimeFormat.UserDefined;
                            UseDd.UserDefinedFormat = "yyyy/MM";
                            baseGrid.Sheets[0].Columns[i].CellType = UseDd;
                            break;
                        case "CC":	// Currency 셀 설정
                            baseGrid.Sheets[0].Columns[i].CellType = new CurrencyCellType();
                            break;
                        case "PW":	// Password 셀 설정
                            FarPoint.Win.Spread.CellType.TextCellType passType = new FarPoint.Win.Spread.CellType.TextCellType();
                            passType.PasswordChar = Convert.ToChar("*");
                            baseGrid.Sheets[0].Columns[i].CellType = passType;
                            break;
                        case "MK":	// MaskCellType 셀 설정
                            FarPoint.Win.Spread.CellType.MaskCellType picType = new FarPoint.Win.Spread.CellType.MaskCellType();
                            if (shtCellType[i].Length > 2)
                                picType.Mask = shtCellType[i].Substring(2, shtCellType[i].Length - 2).ToString();
                            picType.MaskChar = Convert.ToChar("_");
                            baseGrid.Sheets[0].Columns[i].CellType = picType;
                            break;
                        case "NM":	// Number 셀 설정
                            int Place = 0;
                            if (shtCellType[i].Length == 3)
                                Place = Convert.ToInt32(shtCellType[i].Substring(2, 1).ToString());
                            FarPoint.Win.Spread.CellType.NumberCellType num = new FarPoint.Win.Spread.CellType.NumberCellType();
                            //num.LeadingZero = FarPoint.Win.Spread.CellType.LeadingZero.No;
                            num.DecimalSeparator = ".";
                            num.DecimalPlaces = Place;
                            num.FixedPoint = true;
                            num.Separator = ",";
                            num.ShowSeparator = true;
                            num.MaximumValue = 99999999999999;
                            num.MinimumValue = -99999999999999;
                            baseGrid.Sheets[0].Columns[i].CellType = num;
                            break;
                        case "HL":	// HyperLink 셀 설정
                            baseGrid.Sheets[0].Columns[i].CellType = new HyperLinkCellType();
                            break;
                        case "PG":	// Progress 셀 설정
                            baseGrid.Sheets[0].Columns[i].CellType = new ProgressCellType();
                            break;
                        case "RT":	// RichText 셀 설정
                            baseGrid.Sheets[0].Columns[i].CellType = new RichTextCellType();
                            break;
                        case "SC":	// SliderCell 셀 설정
                            baseGrid.Sheets[0].Columns[i].CellType = new SliderCellType();
                            break;
                        case "NL":	// 필수입력 사항 체크하지 않는 로직을 탄다
                            baseGrid.Sheets[0].Columns[i].CellType = new GeneralCellType();
                            baseGrid.Sheets[0].Columns[i].CanFocus = false;
                            if (shtCellType[i].ToString() == "NLV") // NLV인 경우 숨김
                                baseGrid.ActiveSheet.Columns[i].Visible = false;
                            break;
                        case "PC":	// Percent 셀 설정
                            baseGrid.Sheets[0].Columns[i].CellType = new PercentCellType();
                            break;
                        case "ML": //TextBox MultiLine
                            FarPoint.Win.Spread.CellType.TextCellType MultiType = new FarPoint.Win.Spread.CellType.TextCellType();
                            MultiType.Multiline = true;

                            baseGrid.Sheets[0].Columns[i].CellType = MultiType;
                            break;
                        default:	// General 설정
                            baseGrid.Sheets[0].Columns[i].CellType = new TextCellType();
                            break;
                    }
                }
                /****************************************************************************/

                /********************** Column 정렬 설정 **************************************/
                if (sheetAlign.Length > i)
                {
                    switch (sheetAlign[i])
                    {
                        case "L":	// 왼쪽 정렬 MIDDLE
                            baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
                            baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
                            break;
                        case "LT":	// 왼쪽 정렬 TOP
                            baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
                            baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                            break;
                        case "LB":	// 왼쪽 정렬 BOTTOM
                            baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
                            baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Bottom;
                            break;
                        case "C": // 가운데 정렬
                            baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
                            baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
                            break;
                        case "CT":	// 가운데 정렬 TOP
                            baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
                            baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                            break;
                        case "CB":	// 가운데 정렬 BOTTOM
                            baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
                            baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Bottom;
                            break;
                        case "R":	// 오른쪽 정렬
                            baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;
                            baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
                            break;
                        case "RT":	// 오른쪽 정렬 TOP
                            baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;
                            baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                            break;
                        case "RB":	// 오른쪽 정렬 BOTTOM
                            baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;
                            baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Bottom;
                            break;
                        default:	// 디폴트 왼쪽 정렬
                            baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
                            baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
                            break;
                    }
                }
                /****************************************************************************/

                /********************** Column Lock 설정 **************************************/
                baseGrid.Sheets[0].Columns[i].Locked = true;
                /****************************************************************************/

            }
        }
        #endregion

        #region grdMakeSheet(10) - 그리드 초기화
        public static void grdMakeSheet(FarPoint.Win.Spread.FpSpread baseGrid
            , string[] sheetTitle
            , int[] shtTitleSpan
            , string[] sheetTitle2
            , string[] sheetAlign
            , int[] sheetWidth
            , string[] shtComboBoxMsg
            , int[] shtHeaderRowCount
            , string[] shtCellType
            , int[] shtCellColor
            )
        {
            baseGrid.Font = new System.Drawing.Font("돋움", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            /********************** 스크롤바 정의 **************************************/
            baseGrid.HorizontalScrollBarPolicy = FarPoint.Win.Spread.ScrollBarPolicy.AsNeeded;
            /********************** 스크롤바 정의 **************************************/
            baseGrid.VerticalScrollBarPolicy = FarPoint.Win.Spread.ScrollBarPolicy.AsNeeded;
            /********************** 그리드 칼럼수 지정 **************************************/
            baseGrid.ActiveSheet.Columns.Count = sheetTitle.Length;
            /********************** 그리드 Row수 지정 **************************************/
            baseGrid.ActiveSheet.Rows.Count = 0;
            /********************** 그리드 Row색상 지정 **************************************/
            baseGrid.Sheets[0].AlternatingRows[0].BackColor = Color.FromName("WhiteSmoke");
            /********************** 그리드 Head 높이 지정 **************************************/
            baseGrid.Sheets[0].ColumnHeader.Rows[0].Height = 28;
            /********************** 그리드 RowHeader 넓이 지정 *********************************/
            baseGrid.Sheets[0].RowHeader.Columns[0].Width = 32;
            /********************** 그리드 바탕색상 지정 **************************************/
            baseGrid.Sheets[0].GrayAreaBackColor = Color.FromName("white");
            /********************** 1Row 선택 기준(단일, 1Row별, 1Cell별) **************************************/
            baseGrid.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.Normal;				//.ExtendedSelect;
            /********************** ColumnHeader 수 지정**************************************/
            baseGrid.Sheets[0].ColumnHeader.RowCount = shtHeaderRowCount[0];
            /********************** HorizontalGridLine 가로 가이드라인 색상 지정**************************************/
            baseGrid.Sheets[0].HorizontalGridLine = new FarPoint.Win.Spread.GridLine(FarPoint.Win.Spread.GridLineType.Flat, Color.FromArgb(193, 201, 212));
            /********************** VerticalGridLine 세로 가이드라인 색상 지정**************************************/
            baseGrid.Sheets[0].VerticalGridLine = new FarPoint.Win.Spread.GridLine(FarPoint.Win.Spread.GridLineType.Flat, Color.FromArgb(193, 201, 212));
            /********************** 왼쪽 상단 코너 색상 지정**************************************/
            baseGrid.Sheets[0].SheetCornerStyle.BackColor = System.Drawing.Color.FromArgb(242, 244, 246); // .FromName("Beige");
            /********************** 첫번째 Head color지정**************************************/
            baseGrid.Sheets[0].ColumnHeader.Rows[0].BackColor = System.Drawing.Color.FromArgb(242, 244, 246); // .FromName("Beige");
            /********************** 두번째 Head color지정**************************************/
            if (baseGrid.Sheets[0].ColumnHeader.RowCount > 1)
                baseGrid.Sheets[0].ColumnHeader.Rows[1].BackColor = System.Drawing.Color.FromArgb(242, 244, 246); // .FromName("Beige");
            /********************** 첫번째 Colum color 지정**************************************/
            baseGrid.Sheets[0].RowHeader.Columns[0].BackColor = System.Drawing.Color.FromArgb(242, 244, 246); // .FromName("Beige");
            /********************** 1번째 Cell(프라이머리) 키 숨김 지정**************************************/
            baseGrid.ActiveSheet.Columns[0].Visible = false;
            /********************** 입력시 기존 데이타 삭제후 입력**************************************/
            baseGrid.EditModeReplace = true;
            /********************** Clipboard 복사시 Head값 미포함**************************************/
            baseGrid.ClipboardOptions = FarPoint.Win.Spread.ClipboardOptions.NoHeaders;
            //********************** 그리드 focus 칼라***************************************************/
            baseGrid.FocusRenderer = new FarPoint.Win.Spread.SolidFocusIndicatorRenderer(Color.CornflowerBlue, 2);
            //********************** 그리드 숫자 입력시 초기화 후 입력***********************************/
            baseGrid.EditModeReplace = true;

            //****************************************************************************************************************
            //:버튼 설정
            //****************************************************************************************************************
            FarPoint.Win.Spread.CellType.ButtonCellType btnType = new FarPoint.Win.Spread.CellType.ButtonCellType();
            btnType.Text = "...";

            //****************************************************************************************************************
            //: Key Enter시 Next Col로 Focus 이동
            //****************************************************************************************************************
            FarPoint.Win.Spread.InputMap im = baseGrid.GetInputMap(FarPoint.Win.Spread.InputMapMode.WhenFocused);
            im.Put(new FarPoint.Win.Spread.Keystroke(Keys.Enter, Keys.None), FarPoint.Win.Spread.SpreadActions.MoveToNextColumnWrap);

            int sheetLength = 0, shtTitleSpanTmp = 0, shtTitleSpanTmp2 = 0;
            sheetLength = sheetTitle.Length;

            if (sheetTitle.Length < sheetTitle2.Length)
                sheetLength = sheetTitle2.Length;

            for (int i = 0; i < sheetTitle.Length; i++)
            {
                //****************************************************************************************************************
                //:Column 폭 넓이 설정
                //****************************************************************************************************************
                if (sheetWidth.Length > i)
                    baseGrid.Sheets[0].Columns[i].Width = sheetWidth[i];

                //****************************************************************************************************************
                //:columnSpan 설정
                //****************************************************************************************************************
                if (shtTitleSpanTmp == i && shtTitleSpan.Length > 0)
                {
                    baseGrid.Sheets[0].ColumnHeader.Cells[0, i].ColumnSpan = shtTitleSpan[shtTitleSpanTmp2];
                    shtTitleSpanTmp = shtTitleSpanTmp + shtTitleSpan[shtTitleSpanTmp2];
                    shtTitleSpanTmp2++;
                }

                //****************************************************************************************************************
                //:sheet 1번째 Header Text 입력
                //****************************************************************************************************************
                if (sheetTitle.Length > i)
                    baseGrid.Sheets[0].ColumnHeader.Cells[0, i].Text = sheetTitle[i].ToString();

                //****************************************************************************************************************
                //:sheet 2번째 Header Text 입력
                //****************************************************************************************************************
                if (sheetTitle2.Length > i && baseGrid.Sheets[0].ColumnHeader.RowCount > 1)
                    baseGrid.Sheets[0].ColumnHeader.Cells[1, i].Text = sheetTitle2[i].ToString();


                /********************** Column Type 설정 **************************************/
                if (shtCellType.Length > i)
                {
                    string CellType = "";
                    if (shtCellType[i].Length > 2)
                        CellType = shtCellType[i].Substring(0, 2);
                    else
                        CellType = shtCellType[i];

                    switch (CellType)
                    {
                        case "BT":	// Button 셀 설정
                            baseGrid.Sheets[0].Columns[i].CellType = btnType;
                            break;
                        case "CK":	// CheckBox 셀 설정
                            baseGrid.Sheets[0].Columns[i].CellType = new CheckBoxCellType();
                            break;
                        case "DT":	// Date 셀 설정(년월일)
                            baseGrid.Sheets[0].Columns[i].CellType = new DateTimeCellType();
                            break;
                        case "DM":	// Date 셀 설정(년월)                            
                            FarPoint.Win.Spread.CellType.DateTimeCellType UseDt = new FarPoint.Win.Spread.CellType.DateTimeCellType();
                            UseDt.DateTimeFormat = FarPoint.Win.Spread.CellType.DateTimeFormat.UserDefined;
                            UseDt.UserDefinedFormat = "yyyy/MM";
                            baseGrid.Sheets[0].Columns[i].CellType = UseDt;
                            break;
                        case "DY":	// Date 셀 설정(년)                            
                            FarPoint.Win.Spread.CellType.DateTimeCellType UseDy = new FarPoint.Win.Spread.CellType.DateTimeCellType();
                            UseDy.DateTimeFormat = FarPoint.Win.Spread.CellType.DateTimeFormat.UserDefined;
                            UseDy.UserDefinedFormat = "yyyy/MM";
                            baseGrid.Sheets[0].Columns[i].CellType = UseDy;
                            break;
                        case "DD":	// Date 셀 설정(월)                            
                            FarPoint.Win.Spread.CellType.DateTimeCellType UseDd = new FarPoint.Win.Spread.CellType.DateTimeCellType();
                            UseDd.DateTimeFormat = FarPoint.Win.Spread.CellType.DateTimeFormat.UserDefined;
                            UseDd.UserDefinedFormat = "yyyy/MM";
                            baseGrid.Sheets[0].Columns[i].CellType = UseDd;
                            break;
                        case "CC":	// Currency 셀 설정
                            baseGrid.Sheets[0].Columns[i].CellType = new CurrencyCellType();
                            break;
                        case "PW":	// Password 셀 설정
                            FarPoint.Win.Spread.CellType.TextCellType passType = new FarPoint.Win.Spread.CellType.TextCellType();
                            passType.PasswordChar = Convert.ToChar("*");
                            baseGrid.Sheets[0].Columns[i].CellType = passType;
                            break;
                        case "MK":	// MaskCellType 셀 설정
                            FarPoint.Win.Spread.CellType.MaskCellType picType = new FarPoint.Win.Spread.CellType.MaskCellType();
                            if (shtCellType[i].Length > 2)
                                picType.Mask = shtCellType[i].Substring(2, shtCellType[i].Length - 2).ToString();
                            picType.MaskChar = Convert.ToChar("_");
                            baseGrid.Sheets[0].Columns[i].CellType = picType;
                            break;
                        case "NM":	// Number 셀 설정
                            int Place = 0;
                            if (shtCellType[i].Length == 3)
                                Place = Convert.ToInt32(shtCellType[i].Substring(2, 1).ToString());
                            FarPoint.Win.Spread.CellType.NumberCellType num = new FarPoint.Win.Spread.CellType.NumberCellType();
                            num.DecimalSeparator = ".";
                            num.DecimalPlaces = Place;
                            num.FixedPoint = true;
                            num.Separator = ",";
                            num.ShowSeparator = true;
                            num.MaximumValue = 99999999999999;
                            num.MinimumValue = -99999999999999;

                            //num.LeadingZero = FarPoint.Win.Spread.CellType.LeadingZero.No;

                            baseGrid.Sheets[0].Columns[i].CellType = num;
                            break;
                        case "HL":	// HyperLink 셀 설정
                            baseGrid.Sheets[0].Columns[i].CellType = new HyperLinkCellType();
                            break;
                        case "PG":	// Progress 셀 설정
                            baseGrid.Sheets[0].Columns[i].CellType = new ProgressCellType();
                            break;
                        case "RT":	// RichText 셀 설정
                            baseGrid.Sheets[0].Columns[i].CellType = new RichTextCellType();
                            break;
                        case "SC":	// SliderCell 셀 설정
                            baseGrid.Sheets[0].Columns[i].CellType = new SliderCellType();
                            break;
                        case "NL":	// 필수입력 사항 체크하지 않는 로직을 탄다
                            baseGrid.Sheets[0].Columns[i].CellType = new GeneralCellType();
                            baseGrid.Sheets[0].Columns[i].CanFocus = false;
                            if (shtCellType[i].ToString() == "NLV") // NLV인 경우 숨김
                                baseGrid.ActiveSheet.Columns[i].Visible = false;
                            break;
                        case "PC":	// Percent 셀 설정
                            baseGrid.Sheets[0].Columns[i].CellType = new PercentCellType();
                            break;
                        case "CB":	// ComboBox 셀 설정
                            for (int k = 0; k < shtComboBoxMsg.Length; k++)
                            {
                                if (shtComboBoxMsg[k].Length > 0)
                                {
                                    if (i == Convert.ToInt32(shtComboBoxMsg[k].Substring(0, shtComboBoxMsg[k].IndexOf(":"))))
                                    {
                                        ComboBoxCellType comboType = new ComboBoxCellType();
                                        comboType.MaxDrop = 20;

                                        Regex rx = new Regex("#");
                                        string Tmp = shtComboBoxMsg[k].Substring(shtComboBoxMsg[k].IndexOf(":") + 1, shtComboBoxMsg[k].Length - shtComboBoxMsg[k].IndexOf(":") - 1);
                                        string Tmp1 = "";
                                        string Tmp2 = "";
                                        if (Tmp.IndexOf("|") > 0)
                                        {
                                            Tmp1 = Tmp.Substring(0, Tmp.IndexOf("|"));
                                            Tmp2 = Tmp.Substring(Tmp.IndexOf("|") + 1, Tmp.Length - Tmp.IndexOf("|") - 1);
                                            string[] cboMsg1 = rx.Split(Tmp1);
                                            string[] cboMsg2 = rx.Split(Tmp2);

                                            comboType.ItemData = cboMsg1;		// Value
                                            comboType.Items = cboMsg2;	// Key, Text
                                            comboType.EditorValue = FarPoint.Win.Spread.CellType.EditorValue.ItemData;

                                            baseGrid.Sheets[0].Columns[i].CellType = comboType;
                                        }
                                        else
                                        {
                                            string[] cboMsg1 = rx.Split(Tmp);
                                            comboType.Items = cboMsg1;
                                            baseGrid.Sheets[0].Columns[i].CellType = comboType;
                                        }
                                        if (shtCellType[i].ToString() == "CBV") // NLV인 경우 숨김
                                            baseGrid.ActiveSheet.Columns[i].Visible = false;

                                    }
                                }
                                else
                                {
                                    MessageBox.Show(SystemBase.Base.MessageRtn("SY016"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                            break;
                        case "ML": //TextBox MultiLine
                            FarPoint.Win.Spread.CellType.TextCellType MultiType = new FarPoint.Win.Spread.CellType.TextCellType();
                            MultiType.Multiline = true;

                            baseGrid.Sheets[0].Columns[i].CellType = MultiType;
                            break;
                        default:	// General 설정
                            baseGrid.Sheets[0].Columns[i].CellType = new TextCellType();
                            break;
                    }
                }
                /****************************************************************************/


                /********************** Column 정렬 설정 **************************************/
                if (sheetAlign.Length > i)
                {
                    switch (sheetAlign[i])
                    {
                        case "L":	// 왼쪽 정렬 MIDDLE
                            baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
                            baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
                            break;
                        case "LT":	// 왼쪽 정렬 TOP
                            baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
                            baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                            break;
                        case "LB":	// 왼쪽 정렬 BOTTOM
                            baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
                            baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Bottom;
                            break;
                        case "C": // 가운데 정렬
                            baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
                            baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
                            break;
                        case "CT":	// 가운데 정렬 TOP
                            baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
                            baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                            break;
                        case "CB":	// 가운데 정렬 BOTTOM
                            baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
                            baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Bottom;
                            break;
                        case "R":	// 오른쪽 정렬
                            baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;
                            baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
                            break;
                        case "RT":	// 오른쪽 정렬 TOP
                            baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;
                            baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                            break;
                        case "RB":	// 오른쪽 정렬 BOTTOM
                            baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;
                            baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Bottom;
                            break;
                        default:	// 디폴트 왼쪽 정렬
                            baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
                            baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
                            break;
                    }
                }

                /********************** Column 색상 및 Lock 설정 **************************************/
                if (shtCellColor.Length > i)
                {
                    switch (shtCellColor[i])
                    {
                        case 0:	// 일반
                            baseGrid.Sheets[0].Columns[i].BackColor = Color.White;
                            break;
                        case 1:	// 필수입력
                            baseGrid.Sheets[0].Columns[i].BackColor = SystemBase.Validation.Kind_LightCyan; //System.Drawing.Color.FromArgb(242, 252, 254);
                            break;
                        case 2:	// 읽기전용
                            baseGrid.Sheets[0].Columns[i].BackColor = System.Drawing.Color.FromArgb(239, 239, 239);
                            baseGrid.Sheets[0].Columns[i].Locked = true;
                            break;
                        case 3:	// 읽기전용이면서 필수항목에서 제외
                            baseGrid.Sheets[0].Columns[i].BackColor = System.Drawing.Color.FromArgb(238, 238, 238);
                            baseGrid.Sheets[0].Columns[i].Locked = true;
                            break;
                        case 4:	// 읽기전용 & ReadOnly & White
                            baseGrid.Sheets[0].Columns[i].BackColor = Color.White;
                            baseGrid.Sheets[0].Columns[i].Locked = true;
                            break;
                        case 5:	// 읽기전용, 필수항목에서 제외, Focus 제외
                            baseGrid.Sheets[0].Columns[i].BackColor = System.Drawing.Color.FromArgb(238, 238, 238);
                            baseGrid.Sheets[0].Columns[i].Locked = true;
                            baseGrid.Sheets[0].Columns[i].CanFocus = false;
                            break;
                        case 6:	// 읽기전용, 필수항목, Focus 제외
                            baseGrid.Sheets[0].Columns[i].BackColor = System.Drawing.Color.FromArgb(239, 239, 239);
                            baseGrid.Sheets[0].Columns[i].Locked = true;
                            baseGrid.Sheets[0].Columns[i].CanFocus = false;
                            break;
                        default:	// 일반
                            baseGrid.Sheets[0].Columns[i].BackColor = Color.White;
                            break;
                    }
                }
                /****************************************************************************/
            }
        }
        #endregion

        #region grdMakeSheet(12) - 그리드 초기화(Header Count = 3)
        public static void grdMakeSheet(FarPoint.Win.Spread.FpSpread baseGrid
            , string[] sheetTitle
            , int[] shtTitleSpan
            , int[] shtTitleSpan2
            , string[] sheetTitle2
            , string[] sheetTitle3
            , string[] sheetAlign
            , int[] sheetWidth
            , string[] shtComboBoxMsg
            , int[] shtHeaderRowCount
            , string[] shtCellType
            , int[] shtCellColor
            )
        {
            baseGrid.Font = new System.Drawing.Font("돋움", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            /********************** 스크롤바 정의 **************************************/
            baseGrid.HorizontalScrollBarPolicy = FarPoint.Win.Spread.ScrollBarPolicy.AsNeeded;
            /********************** 스크롤바 정의 **************************************/
            baseGrid.VerticalScrollBarPolicy = FarPoint.Win.Spread.ScrollBarPolicy.AsNeeded;
            /********************** 그리드 칼럼수 지정 **************************************/
            baseGrid.ActiveSheet.Columns.Count = sheetTitle.Length;
            /********************** 그리드 Row수 지정 **************************************/
            baseGrid.ActiveSheet.Rows.Count = 0;
            /********************** 그리드 Row색상 지정 **************************************/
            baseGrid.Sheets[0].AlternatingRows[0].BackColor = Color.FromName("WhiteSmoke");
            /********************** 그리드 Head 높이 지정 **************************************/
            baseGrid.Sheets[0].ColumnHeader.Rows[0].Height = 28;
            /********************** 그리드 RowHeader 넓이 지정 *********************************/
            baseGrid.Sheets[0].RowHeader.Columns[0].Width = 32;
            /********************** 그리드 바탕색상 지정 **************************************/
            baseGrid.Sheets[0].GrayAreaBackColor = Color.FromName("white");
            /********************** 1Row 선택 기준(단일, 1Row별, 1Cell별) **************************************/
            baseGrid.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.Normal;				//.ExtendedSelect;
            /********************** ColumnHeader 수 지정**************************************/
            baseGrid.Sheets[0].ColumnHeader.RowCount = shtHeaderRowCount[0];
            /********************** HorizontalGridLine 가로 가이드라인 색상 지정**************************************/
            baseGrid.Sheets[0].HorizontalGridLine = new FarPoint.Win.Spread.GridLine(FarPoint.Win.Spread.GridLineType.Flat, Color.FromArgb(193, 201, 212));
            /********************** VerticalGridLine 세로 가이드라인 색상 지정**************************************/
            baseGrid.Sheets[0].VerticalGridLine = new FarPoint.Win.Spread.GridLine(FarPoint.Win.Spread.GridLineType.Flat, Color.FromArgb(193, 201, 212));
            /********************** 왼쪽 상단 코너 색상 지정**************************************/
            baseGrid.Sheets[0].SheetCornerStyle.BackColor = System.Drawing.Color.FromArgb(242, 244, 246); // .FromName("Beige");
            /********************** 첫번째 Head color지정**************************************/
            baseGrid.Sheets[0].ColumnHeader.Rows[0].BackColor = System.Drawing.Color.FromArgb(242, 244, 246); // .FromName("Beige");
            /********************** 두번째 Head color지정**************************************/
            if (baseGrid.Sheets[0].ColumnHeader.RowCount > 1)
                baseGrid.Sheets[0].ColumnHeader.Rows[1].BackColor = System.Drawing.Color.FromArgb(242, 244, 246); // .FromName("Beige");
            /********************** 세번째 Head color지정**************************************/
            if (baseGrid.Sheets[0].ColumnHeader.RowCount > 1)
                baseGrid.Sheets[0].ColumnHeader.Rows[2].BackColor = System.Drawing.Color.FromArgb(242, 244, 246); // .FromName("Beige");
            /********************** 첫번째 Colum color 지정**************************************/
            baseGrid.Sheets[0].RowHeader.Columns[0].BackColor = System.Drawing.Color.FromArgb(242, 244, 246); // .FromName("Beige");
            /********************** 1번째 Cell(프라이머리) 키 숨김 지정**************************************/
            baseGrid.ActiveSheet.Columns[0].Visible = false;
            /********************** 입력시 기존 데이타 삭제후 입력**************************************/
            baseGrid.EditModeReplace = true;
            /********************** Clipboard 복사시 Head값 미포함**************************************/
            baseGrid.ClipboardOptions = FarPoint.Win.Spread.ClipboardOptions.NoHeaders;
            //********************** 그리드 focus 칼라***************************************************/
            baseGrid.FocusRenderer = new FarPoint.Win.Spread.SolidFocusIndicatorRenderer(Color.CornflowerBlue, 2);
            //********************** 그리드 숫자 입력시 초기화 후 입력***********************************/
            baseGrid.EditModeReplace = true;

            //****************************************************************************************************************
            //:버튼 설정
            //****************************************************************************************************************
            FarPoint.Win.Spread.CellType.ButtonCellType btnType = new FarPoint.Win.Spread.CellType.ButtonCellType();
            btnType.Text = "...";

            //****************************************************************************************************************
            //: Key Enter시 Next Col로 Focus 이동
            //****************************************************************************************************************
            FarPoint.Win.Spread.InputMap im = baseGrid.GetInputMap(FarPoint.Win.Spread.InputMapMode.WhenFocused);
            im.Put(new FarPoint.Win.Spread.Keystroke(Keys.Enter, Keys.None), FarPoint.Win.Spread.SpreadActions.MoveToNextColumnWrap);

            int sheetLength = 0, shtTitleSpanTmp = 0, shtTitleSpanTmp2 = 0, shtTitleSpanTmp3 = 0, shtTitleSpanTmp4 = 0;
            sheetLength = sheetTitle.Length;

            if (sheetTitle.Length < sheetTitle2.Length)
                sheetLength = sheetTitle2.Length;

            for (int i = 0; i < sheetTitle.Length; i++)
            {
                //****************************************************************************************************************
                //:Column 폭 넓이 설정
                //****************************************************************************************************************
                if (sheetWidth.Length > i)
                    baseGrid.Sheets[0].Columns[i].Width = sheetWidth[i];

                //****************************************************************************************************************
                //:Sheet 1번째 columnSpan 설정
                //****************************************************************************************************************
                if (shtTitleSpanTmp == i && shtTitleSpan.Length > 0)
                {
                    baseGrid.Sheets[0].ColumnHeader.Cells[0, i].ColumnSpan = shtTitleSpan[shtTitleSpanTmp2];
                    shtTitleSpanTmp = shtTitleSpanTmp + shtTitleSpan[shtTitleSpanTmp2];
                    shtTitleSpanTmp2++;
                }

                //****************************************************************************************************************
                //:Sheet 2번째 columnSpan 설정
                //****************************************************************************************************************
                if (shtTitleSpanTmp3 == i && shtTitleSpan.Length > 1)
                {
                    baseGrid.Sheets[0].ColumnHeader.Cells[1, i].ColumnSpan = shtTitleSpan2[shtTitleSpanTmp4];
                    shtTitleSpanTmp3 = shtTitleSpanTmp3 + shtTitleSpan2[shtTitleSpanTmp4];
                    shtTitleSpanTmp4++;
                }

                //****************************************************************************************************************
                //:sheet 1번째 Header Text 입력
                //****************************************************************************************************************
                if (sheetTitle.Length > i)
                    baseGrid.Sheets[0].ColumnHeader.Cells[0, i].Text = sheetTitle[i].ToString();

                //****************************************************************************************************************
                //:sheet 2번째 Header Text 입력
                //****************************************************************************************************************
                if (sheetTitle2.Length > i && baseGrid.Sheets[0].ColumnHeader.RowCount > 1)
                    baseGrid.Sheets[0].ColumnHeader.Cells[1, i].Text = sheetTitle2[i].ToString();

                //****************************************************************************************************************
                //:sheet 3번째 Header Text 입력
                //****************************************************************************************************************
                if (sheetTitle3.Length > i && baseGrid.Sheets[0].ColumnHeader.RowCount > 2)
                    baseGrid.Sheets[0].ColumnHeader.Cells[2, i].Text = sheetTitle3[i].ToString();


                /********************** Column Type 설정 **************************************/
                if (shtCellType.Length > i)
                {
                    string CellType = "";
                    if (shtCellType[i].Length > 2)
                        CellType = shtCellType[i].Substring(0, 2);
                    else
                        CellType = shtCellType[i];

                    switch (CellType)
                    {
                        case "BT":	// Button 셀 설정
                            baseGrid.Sheets[0].Columns[i].CellType = btnType;
                            break;
                        case "CK":	// CheckBox 셀 설정
                            baseGrid.Sheets[0].Columns[i].CellType = new CheckBoxCellType();
                            break;
                        case "DT":	// Date 셀 설정(년월일)
                            baseGrid.Sheets[0].Columns[i].CellType = new DateTimeCellType();
                            break;
                        case "DM":	// Date 셀 설정(년월)                            
                            FarPoint.Win.Spread.CellType.DateTimeCellType UseDt = new FarPoint.Win.Spread.CellType.DateTimeCellType();
                            UseDt.DateTimeFormat = FarPoint.Win.Spread.CellType.DateTimeFormat.UserDefined;
                            UseDt.UserDefinedFormat = "yyyy/MM";
                            baseGrid.Sheets[0].Columns[i].CellType = UseDt;
                            break;
                        case "DY":	// Date 셀 설정(년)                            
                            FarPoint.Win.Spread.CellType.DateTimeCellType UseDy = new FarPoint.Win.Spread.CellType.DateTimeCellType();
                            UseDy.DateTimeFormat = FarPoint.Win.Spread.CellType.DateTimeFormat.UserDefined;
                            UseDy.UserDefinedFormat = "yyyy/MM";
                            baseGrid.Sheets[0].Columns[i].CellType = UseDy;
                            break;
                        case "DD":	// Date 셀 설정(월)                            
                            FarPoint.Win.Spread.CellType.DateTimeCellType UseDd = new FarPoint.Win.Spread.CellType.DateTimeCellType();
                            UseDd.DateTimeFormat = FarPoint.Win.Spread.CellType.DateTimeFormat.UserDefined;
                            UseDd.UserDefinedFormat = "yyyy/MM";
                            baseGrid.Sheets[0].Columns[i].CellType = UseDd;
                            break;
                        case "CC":	// Currency 셀 설정
                            baseGrid.Sheets[0].Columns[i].CellType = new CurrencyCellType();
                            break;
                        case "PW":	// Password 셀 설정
                            FarPoint.Win.Spread.CellType.TextCellType passType = new FarPoint.Win.Spread.CellType.TextCellType();
                            passType.PasswordChar = Convert.ToChar("*");
                            baseGrid.Sheets[0].Columns[i].CellType = passType;
                            break;
                        case "MK":	// MaskCellType 셀 설정
                            FarPoint.Win.Spread.CellType.MaskCellType picType = new FarPoint.Win.Spread.CellType.MaskCellType();
                            if (shtCellType[i].Length > 2)
                                picType.Mask = shtCellType[i].Substring(2, shtCellType[i].Length - 2).ToString();
                            picType.MaskChar = Convert.ToChar("_");
                            baseGrid.Sheets[0].Columns[i].CellType = picType;
                            break;
                        case "NM":	// Number 셀 설정
                            int Place = 0;
                            if (shtCellType[i].Length == 3)
                                Place = Convert.ToInt32(shtCellType[i].Substring(2, 1).ToString());
                            FarPoint.Win.Spread.CellType.NumberCellType num = new FarPoint.Win.Spread.CellType.NumberCellType();
                            num.DecimalSeparator = ".";
                            num.DecimalPlaces = Place;
                            num.FixedPoint = true;
                            num.Separator = ",";
                            num.ShowSeparator = true;
                            num.MaximumValue = 99999999999999;
                            num.MinimumValue = -99999999999999;

                            //num.LeadingZero = FarPoint.Win.Spread.CellType.LeadingZero.No;

                            baseGrid.Sheets[0].Columns[i].CellType = num;
                            break;
                        case "HL":	// HyperLink 셀 설정
                            baseGrid.Sheets[0].Columns[i].CellType = new HyperLinkCellType();
                            break;
                        case "PG":	// Progress 셀 설정
                            baseGrid.Sheets[0].Columns[i].CellType = new ProgressCellType();
                            break;
                        case "RT":	// RichText 셀 설정
                            baseGrid.Sheets[0].Columns[i].CellType = new RichTextCellType();
                            break;
                        case "SC":	// SliderCell 셀 설정
                            baseGrid.Sheets[0].Columns[i].CellType = new SliderCellType();
                            break;
                        case "NL":	// 필수입력 사항 체크하지 않는 로직을 탄다
                            baseGrid.Sheets[0].Columns[i].CellType = new GeneralCellType();
                            baseGrid.Sheets[0].Columns[i].CanFocus = false;
                            if (shtCellType[i].ToString() == "NLV") // NLV인 경우 숨김
                                baseGrid.ActiveSheet.Columns[i].Visible = false;
                            break;
                        case "PC":	// Percent 셀 설정
                            baseGrid.Sheets[0].Columns[i].CellType = new PercentCellType();
                            break;
                        case "CB":	// ComboBox 셀 설정
                            for (int k = 0; k < shtComboBoxMsg.Length; k++)
                            {
                                if (shtComboBoxMsg[k].Length > 0)
                                {
                                    if (i == Convert.ToInt32(shtComboBoxMsg[k].Substring(0, shtComboBoxMsg[k].IndexOf(":"))))
                                    {
                                        ComboBoxCellType comboType = new ComboBoxCellType();
                                        comboType.MaxDrop = 20;

                                        Regex rx = new Regex("#");
                                        string Tmp = shtComboBoxMsg[k].Substring(shtComboBoxMsg[k].IndexOf(":") + 1, shtComboBoxMsg[k].Length - shtComboBoxMsg[k].IndexOf(":") - 1);
                                        string Tmp1 = "";
                                        string Tmp2 = "";
                                        if (Tmp.IndexOf("|") > 0)
                                        {
                                            Tmp1 = Tmp.Substring(0, Tmp.IndexOf("|"));
                                            Tmp2 = Tmp.Substring(Tmp.IndexOf("|") + 1, Tmp.Length - Tmp.IndexOf("|") - 1);
                                            string[] cboMsg1 = rx.Split(Tmp1);
                                            string[] cboMsg2 = rx.Split(Tmp2);

                                            comboType.ItemData = cboMsg1;		// Value
                                            comboType.Items = cboMsg2;	// Key, Text
                                            comboType.EditorValue = FarPoint.Win.Spread.CellType.EditorValue.ItemData;

                                            baseGrid.Sheets[0].Columns[i].CellType = comboType;
                                        }
                                        else
                                        {
                                            string[] cboMsg1 = rx.Split(Tmp);
                                            comboType.Items = cboMsg1;
                                            baseGrid.Sheets[0].Columns[i].CellType = comboType;
                                        }
                                        if (shtCellType[i].ToString() == "CBV") // NLV인 경우 숨김
                                            baseGrid.ActiveSheet.Columns[i].Visible = false;
                                    }

                                }
                                else
                                {
                                    MessageBox.Show(SystemBase.Base.MessageRtn("SY016"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                            break;
                        case "ML": //TextBox MultiLine
                            FarPoint.Win.Spread.CellType.TextCellType MultiType = new FarPoint.Win.Spread.CellType.TextCellType();
                            MultiType.Multiline = true;

                            baseGrid.Sheets[0].Columns[i].CellType = MultiType;
                            break;
                        default:	// General 설정
                            baseGrid.Sheets[0].Columns[i].CellType = new TextCellType();
                            break;
                    }
                }
                /****************************************************************************/


                /********************** Column 정렬 설정 **************************************/
                if (sheetAlign.Length > i)
                {
                    switch (sheetAlign[i])
                    {
                        case "L":	// 왼쪽 정렬 MIDDLE
                            baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
                            baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
                            break;
                        case "LT":	// 왼쪽 정렬 TOP
                            baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
                            baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                            break;
                        case "LB":	// 왼쪽 정렬 BOTTOM
                            baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
                            baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Bottom;
                            break;
                        case "C": // 가운데 정렬
                            baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
                            baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
                            break;
                        case "CT":	// 가운데 정렬 TOP
                            baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
                            baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                            break;
                        case "CB":	// 가운데 정렬 BOTTOM
                            baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
                            baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Bottom;
                            break;
                        case "R":	// 오른쪽 정렬
                            baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;
                            baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
                            break;
                        case "RT":	// 오른쪽 정렬 TOP
                            baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;
                            baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                            break;
                        case "RB":	// 오른쪽 정렬 BOTTOM
                            baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;
                            baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Bottom;
                            break;
                        default:	// 디폴트 왼쪽 정렬
                            baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
                            baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
                            break;
                    }
                }

                /********************** Column 색상 및 Lock 설정 **************************************/
                if (shtCellColor.Length > i)
                {
                    switch (shtCellColor[i])
                    {
                        case 0:	// 일반
                            baseGrid.Sheets[0].Columns[i].BackColor = Color.White;
                            break;
                        case 1:	// 필수입력
                            baseGrid.Sheets[0].Columns[i].BackColor = SystemBase.Validation.Kind_LightCyan; //System.Drawing.Color.FromArgb(242, 252, 254);
                            break;
                        case 2:	// 읽기전용
                            baseGrid.Sheets[0].Columns[i].BackColor = System.Drawing.Color.FromArgb(239, 239, 239);
                            baseGrid.Sheets[0].Columns[i].Locked = true;
                            break;
                        case 3:	// 읽기전용이면서 필수항목에서 제외
                            baseGrid.Sheets[0].Columns[i].BackColor = System.Drawing.Color.FromArgb(238, 238, 238);
                            baseGrid.Sheets[0].Columns[i].Locked = true;
                            break;
                        case 4:	// 읽기전용 & ReadOnly & White
                            baseGrid.Sheets[0].Columns[i].BackColor = Color.White;
                            baseGrid.Sheets[0].Columns[i].Locked = true;
                            break;
                        case 5:	// 읽기전용, 필수항목에서 제외, Focus 제외
                            baseGrid.Sheets[0].Columns[i].BackColor = System.Drawing.Color.FromArgb(238, 238, 238);
                            baseGrid.Sheets[0].Columns[i].Locked = true;
                            baseGrid.Sheets[0].Columns[i].CanFocus = false;
                            break;
                        case 6:	// 읽기전용, 필수항목, Focus 제외
                            baseGrid.Sheets[0].Columns[i].BackColor = System.Drawing.Color.FromArgb(239, 239, 239);
                            baseGrid.Sheets[0].Columns[i].Locked = true;
                            baseGrid.Sheets[0].Columns[i].CanFocus = false;
                            break;
                        default:	// 일반
                            baseGrid.Sheets[0].Columns[i].BackColor = Color.White;
                            break;
                    }
                }
                /****************************************************************************/

            }
        }
        #endregion

        #region grdComboRemake(4)
        /**************************************************
		Query = "usp_PCOMMON @pType='Z3', @pCO_CD = '"+SystemBase.Base.gstrCOMCD+"', @pBIZ_CD ='"+fpSpread2.Sheets[0].Cells[e.Row,19].Value.ToString()+"'";//공장
		UIForm.FPMake.grdComboRemake(fpSpread2, e.Row, 20, SystemBase.ComboMake.ComboOnGrid(Query,0));
		**************************************************/
        public static void grdComboRemake(FarPoint.Win.Spread.FpSpread baseGrid, int Row, int Col, string Value)
        {
            try
            {
                ComboBoxCellType comboType = new ComboBoxCellType();
                comboType.MaxDrop = 20;

                Regex rx = new Regex("#");

                string Tmp1 = Value.Substring(0, Value.IndexOf("|"));
                string Tmp2 = Value.Substring(Value.IndexOf("|") + 1, Value.Length - Value.IndexOf("|") - 1);
                string[] cboMsg1 = rx.Split(Tmp1);
                string[] cboMsg2 = rx.Split(Tmp2);

                comboType.ItemData = cboMsg1;	// Key, Text
                comboType.Items = cboMsg2;		// Value
                comboType.EditorValue = FarPoint.Win.Spread.CellType.EditorValue.ItemData;

                baseGrid.Sheets[0].Cells[Row, Col].CellType = comboType;
            }
            catch (Exception e)
            {
                SystemBase.Loggers.Log("grdComboRemake (로우,칼럼별 Combo 재정의중 에러)", e.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY013"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region grdMakeSheet(14) - 그리드 데이타 조회 후 Frozen Row 또는 Colunm 정의
        public static void grdMakeSheet(FarPoint.Win.Spread.FpSpread baseGrid
            , string Query
            , string[] sheetTitle
            , int[] shtTitleSpan
            , string[] sheetTitle2
            , string[] sheetAlign
            , int[] sheetWidth
            , string[] shtComboBoxMsg
            , int[] shtHeaderRowCount
            , string[] shtCellType
            , int[] shtCellColor
            , int shtFixR
            , int shtFixC
            , bool shtSummary
            )
        {
            try
            {
                /********************** 그리드 초기화 **************************************/
                baseGrid.Reset();
                //********************** Clipboard False***********************************/
                baseGrid.AutoClipboard = false;
                /********************** 그리드 폰트 **************************************/
                baseGrid.Font = new System.Drawing.Font("돋움", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
                /********************** 그리드 스타일 정의 **************************************/
                baseGrid.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
                /********************** 스크롤바 정의 **************************************/
                baseGrid.HorizontalScrollBarPolicy = FarPoint.Win.Spread.ScrollBarPolicy.AsNeeded;
                /********************** 스크롤바 정의 **************************************/
                baseGrid.VerticalScrollBarPolicy = FarPoint.Win.Spread.ScrollBarPolicy.AsNeeded;
                /********************** 스크롤바 정의 **************************************/
                baseGrid.ScrollBarTrackPolicy = FarPoint.Win.Spread.ScrollBarTrackPolicy.Both;
                /********************** 그리드 칼럼수 지정 **************************************/
                baseGrid.ActiveSheet.Columns.Count = sheetTitle.Length;
                /********************** 그리드 Row수 지정 **************************************/
                //baseGrid.ActiveSheet.Rows.Count = 0;
                /********************** 그리드 Row색상 지정 **************************************/
                baseGrid.Sheets[0].AlternatingRows[0].BackColor = Color.FromName("WhiteSmoke");
                /********************** 그리드 Head 높이 지정 **************************************/
                baseGrid.Sheets[0].ColumnHeader.Rows[0].Height = 28;
                /********************** 그리드 RowHeader 넓이 지정 *********************************/
                baseGrid.Sheets[0].RowHeader.Columns[0].Width = 32;
                /********************** 그리드 바탕색상 지정 **************************************/
                baseGrid.Sheets[0].GrayAreaBackColor = Color.FromName("white");
                /********************** 1Row 선택 기준(단일, 1Row별, 1Cell별) **************************************/
                baseGrid.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.Normal;				//.ExtendedSelect;
                /********************** ColumnHeader 수 지정**************************************/
                baseGrid.Sheets[0].ColumnHeader.RowCount = shtHeaderRowCount[0];
                /********************** HorizontalGridLine 가로 가이드라인 색상 지정**************************************/
                baseGrid.Sheets[0].HorizontalGridLine = new FarPoint.Win.Spread.GridLine(FarPoint.Win.Spread.GridLineType.Flat, Color.FromArgb(193, 201, 212));
                /********************** VerticalGridLine 세로 가이드라인 색상 지정**************************************/
                baseGrid.Sheets[0].VerticalGridLine = new FarPoint.Win.Spread.GridLine(FarPoint.Win.Spread.GridLineType.Flat, Color.FromArgb(193, 201, 212));
                /********************** 왼쪽 상단 코너 색상 지정**************************************/
                baseGrid.Sheets[0].SheetCornerStyle.BackColor = System.Drawing.Color.FromArgb(242, 244, 246); // .FromName("Beige");
                /********************** 첫번째 Head color지정**************************************/
                baseGrid.Sheets[0].ColumnHeader.Rows[0].BackColor = System.Drawing.Color.FromArgb(242, 244, 246); // .FromName("Beige");
                /********************** 두번째 Head color지정**************************************/
                if (baseGrid.Sheets[0].ColumnHeader.RowCount > 1)
                    baseGrid.Sheets[0].ColumnHeader.Rows[1].BackColor = System.Drawing.Color.FromArgb(242, 244, 246); // .FromName("Beige");
                /********************** 첫번째 Colum color 지정**************************************/
                baseGrid.Sheets[0].RowHeader.Columns[0].BackColor = System.Drawing.Color.FromArgb(242, 244, 246); // .FromName("Beige");
                /********************** 1번째 Cell(프라이머리) 키 숨김 지정**************************************/
                baseGrid.ActiveSheet.Columns[0].Visible = false;
                /********************** 입력시 기존 데이타 삭제후 입력**************************************/
                baseGrid.EditModeReplace = true;
                /********************** Clipboard 복사시 Head값 미포함**************************************/
                baseGrid.ClipboardOptions = FarPoint.Win.Spread.ClipboardOptions.NoHeaders;
                //********************** 그리드 focus 칼라***************************************************/
                baseGrid.FocusRenderer = new FarPoint.Win.Spread.SolidFocusIndicatorRenderer(Color.CornflowerBlue, 2);
                //********************** 그리드 숫자 입력시 초기화 후 입력***********************************/
                baseGrid.EditModeReplace = true;



                DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);

                baseGrid.Sheets[0].DataSource = dt;
                if (dt.Rows.Count > 0 && shtSummary == true)
                    baseGrid.ActiveSheet.Rows.Count = dt.Rows.Count + 1;//그리드Row수 지정
                else
                    baseGrid.ActiveSheet.Rows.Count = dt.Rows.Count;//그리드Row수 지정

                //****************************************************************************************************************
                //: 고정 Column 또는 Row 설정
                //****************************************************************************************************************
                baseGrid.Sheets[0].FrozenColumnCount = shtFixC + 1;
                baseGrid.Sheets[0].FrozenRowCount = shtFixR;
                //****************************************************************************************************************
                //: 버튼 설정
                //****************************************************************************************************************
                FarPoint.Win.Spread.CellType.ButtonCellType btnType = new FarPoint.Win.Spread.CellType.ButtonCellType();
                btnType.Text = "...";

                //****************************************************************************************************************
                //: Key Enter시 Next Col로 Focus 이동
                //****************************************************************************************************************
                FarPoint.Win.Spread.InputMap im = baseGrid.GetInputMap(FarPoint.Win.Spread.InputMapMode.WhenFocused);
                im.Put(new FarPoint.Win.Spread.Keystroke(Keys.Enter, Keys.None), FarPoint.Win.Spread.SpreadActions.MoveToNextColumnWrap);


                int sheetLength = 0, shtTitleSpanTmp = 0, shtTitleSpanTmp2 = 0;
                sheetLength = sheetTitle.Length;

                if (sheetTitle.Length < sheetTitle2.Length)
                    sheetLength = sheetTitle2.Length;

                //--------------------------------------------------------------------------------------------------------------------------

                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    //****************************************************************************************************************
                    //: Column 폭 넓이 설정
                    //****************************************************************************************************************
                    if (sheetWidth.Length > i)
                        baseGrid.Sheets[0].Columns[i].Width = sheetWidth[i];

                    //****************************************************************************************************************
                    //: columnSpan 설정
                    //****************************************************************************************************************
                    if (shtTitleSpanTmp == i && shtTitleSpan.Length > i)
                    {
                        baseGrid.Sheets[0].ColumnHeader.Cells[0, i].ColumnSpan = shtTitleSpan[shtTitleSpanTmp2];
                        shtTitleSpanTmp = shtTitleSpanTmp + shtTitleSpan[shtTitleSpanTmp2];
                        shtTitleSpanTmp2++;
                    }

                    //****************************************************************************************************************
                    //: sheet 1번째 Header Text 입력
                    //****************************************************************************************************************
                    if (sheetTitle.Length > i)
                        baseGrid.Sheets[0].ColumnHeader.Cells[0, i].Text = sheetTitle[i].ToString();

                    //****************************************************************************************************************
                    //: sheet 2번째 Header Text 입력
                    //****************************************************************************************************************
                    if (sheetTitle2.Length > i && baseGrid.Sheets[0].ColumnHeader.RowCount > 1)
                        baseGrid.Sheets[0].ColumnHeader.Cells[1, i].Text = sheetTitle2[i].ToString();

                    /********************** Column Type 설정 **************************************/
                    if (shtCellType.Length > i)
                    {
                        string CellType = "";
                        if (shtCellType[i].Length > 2)
                            CellType = shtCellType[i].Substring(0, 2);
                        else
                            CellType = shtCellType[i];

                        switch (CellType)
                        {
                            case "BT":	// Button 셀 설정
                                baseGrid.Sheets[0].Columns[i].CellType = btnType;
                                break;
                            case "CK":	// CheckBox 셀 설정
                                baseGrid.Sheets[0].Columns[i].CellType = new CheckBoxCellType();
                                break;
                            case "CC":	// Currency 셀 설정
                                baseGrid.Sheets[0].Columns[i].CellType = new CurrencyCellType();
                                break;
                            case "DT":	// Date 셀 설정(년월일)
                                baseGrid.Sheets[0].Columns[i].CellType = new DateTimeCellType();
                                break;
                            case "DM":	// Date 셀 설정(년월)                            
                                FarPoint.Win.Spread.CellType.DateTimeCellType UseDt = new FarPoint.Win.Spread.CellType.DateTimeCellType();
                                UseDt.DateTimeFormat = FarPoint.Win.Spread.CellType.DateTimeFormat.UserDefined;
                                UseDt.UserDefinedFormat = "yyyy/MM";
                                baseGrid.Sheets[0].Columns[i].CellType = UseDt;
                                break;
                            case "DY":	// Date 셀 설정(년)                            
                                FarPoint.Win.Spread.CellType.DateTimeCellType UseDy = new FarPoint.Win.Spread.CellType.DateTimeCellType();
                                UseDy.DateTimeFormat = FarPoint.Win.Spread.CellType.DateTimeFormat.UserDefined;
                                UseDy.UserDefinedFormat = "yyyy/MM";
                                baseGrid.Sheets[0].Columns[i].CellType = UseDy;
                                break;
                            case "DD":	// Date 셀 설정(월)                            
                                FarPoint.Win.Spread.CellType.DateTimeCellType UseDd = new FarPoint.Win.Spread.CellType.DateTimeCellType();
                                UseDd.DateTimeFormat = FarPoint.Win.Spread.CellType.DateTimeFormat.UserDefined;
                                UseDd.UserDefinedFormat = "yyyy/MM";
                                baseGrid.Sheets[0].Columns[i].CellType = UseDd;
                                break;
                            case "PW":	// Password 셀 설정
                                FarPoint.Win.Spread.CellType.TextCellType passType = new FarPoint.Win.Spread.CellType.TextCellType();
                                passType.PasswordChar = Convert.ToChar("*");
                                baseGrid.Sheets[0].Columns[i].CellType = passType;
                                break;
                            case "MK":	// MaskCellType 셀 설정 | Mask 사용예 : MK###-###-####
                                FarPoint.Win.Spread.CellType.MaskCellType picType = new FarPoint.Win.Spread.CellType.MaskCellType();
                                if (shtCellType[i].Length > 2)
                                    picType.Mask = shtCellType[i].Substring(2, shtCellType[i].Length - 2).ToString();
                                picType.MaskChar = Convert.ToChar("_");
                                baseGrid.Sheets[0].Columns[i].CellType = picType;
                                break;
                            case "NM":	// Number 셀 설정
                                int Place = 0;
                                if (shtCellType[i].Length == 3)
                                    Place = Convert.ToInt32(shtCellType[i].Substring(2, 1).ToString());
                                FarPoint.Win.Spread.CellType.NumberCellType num = new FarPoint.Win.Spread.CellType.NumberCellType();
                                num.DecimalSeparator = ".";
                                num.DecimalPlaces = Place;
                                num.FixedPoint = true;
                                num.Separator = ",";
                                num.ShowSeparator = true;
                                num.MaximumValue = 99999999999999;
                                num.MinimumValue = -99999999999999;

                                //num.LeadingZero = FarPoint.Win.Spread.CellType.LeadingZero.No;

                                baseGrid.Sheets[0].Columns[i].CellType = num;
                                break;
                            case "HL":	// HyperLink 셀 설정
                                baseGrid.Sheets[0].Columns[i].CellType = new HyperLinkCellType();
                                break;
                            case "PG":	// Progress 셀 설정
                                baseGrid.Sheets[0].Columns[i].CellType = new ProgressCellType();
                                break;
                            case "RT":	// RichText 셀 설정
                                baseGrid.Sheets[0].Columns[i].CellType = new RichTextCellType();
                                break;
                            case "SC":	// SliderCell 셀 설정
                                baseGrid.Sheets[0].Columns[i].CellType = new SliderCellType();
                                break;
                            case "NL":	// 필수입력 사항 체크하지 않는 로직을 탄다
                                baseGrid.Sheets[0].Columns[i].CellType = new GeneralCellType();
                                baseGrid.Sheets[0].Columns[i].CanFocus = false;
                                if (shtCellType[i].ToString() == "NLV") // NLV인 경우 숨김
                                    baseGrid.ActiveSheet.Columns[i].Visible = false;
                                break;
                            case "PC":	// Percent 셀 설정
                                baseGrid.Sheets[0].Columns[i].CellType = new PercentCellType();
                                break;
                            case "CB":	// ComboBox 셀 설정
                                for (int k = 0; k < shtComboBoxMsg.Length; k++)
                                {
                                    if (i == Convert.ToInt32(shtComboBoxMsg[k].Substring(0, shtComboBoxMsg[k].IndexOf(":"))))
                                    {
                                        ComboBoxCellType comboType = new ComboBoxCellType();
                                        comboType.MaxDrop = 20;

                                        Regex rx = new Regex("#");
                                        string Tmp = shtComboBoxMsg[k].Substring(shtComboBoxMsg[k].IndexOf(":") + 1, shtComboBoxMsg[k].Length - shtComboBoxMsg[k].IndexOf(":") - 1);

                                        string Tmp1 = "";
                                        string Tmp2 = "";
                                        if (Tmp.IndexOf("|") > 0)
                                        {
                                            Tmp1 = Tmp.Substring(0, Tmp.IndexOf("|"));
                                            Tmp2 = Tmp.Substring(Tmp.IndexOf("|") + 1, Tmp.Length - Tmp.IndexOf("|") - 1);
                                            string[] cboMsg1 = rx.Split(Tmp1);
                                            string[] cboMsg2 = rx.Split(Tmp2);

                                            comboType.ItemData = cboMsg1;	// Key, Text
                                            comboType.Items = cboMsg2;		// Value
                                            comboType.EditorValue = FarPoint.Win.Spread.CellType.EditorValue.ItemData;

                                            baseGrid.Sheets[0].Columns[i].CellType = comboType;
                                        }
                                        else
                                        {
                                            string[] cboMsg1 = rx.Split(Tmp);

                                            comboType.Items = cboMsg1;
                                            baseGrid.Sheets[0].Columns[i].CellType = comboType;
                                        }
                                        if (shtCellType[i].ToString() == "CBV") // NLV인 경우 숨김
                                            baseGrid.ActiveSheet.Columns[i].Visible = false;

                                    }
                                }
                                break;
                            case "GN":	// General & 영문 대문자로 변환
                                baseGrid.Sheets[0].Columns[i].CellType = new TextCellType();
                                break;
                            case "ML": //TextBox MultiLine
                                FarPoint.Win.Spread.CellType.TextCellType MultiType = new FarPoint.Win.Spread.CellType.TextCellType();
                                MultiType.Multiline = true;

                                baseGrid.Sheets[0].Columns[i].CellType = MultiType;
                                break;
                            default:	// General 설정
                                baseGrid.Sheets[0].Columns[i].CellType = new TextCellType();
                                FarPoint.Win.Spread.CellType.TextCellType textCellType1 = new FarPoint.Win.Spread.CellType.TextCellType();
                                textCellType1.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
                                baseGrid.Sheets[0].Columns.Get(i).CellType = textCellType1;
                                break;
                        }

                        for (int j = 0; j < baseGrid.Sheets[0].Rows.Count; j++)
                        {

                            switch (CellType)
                            {
                                case "BT":	// Button 셀 설정
                                    baseGrid.Sheets[0].Cells[j, i].CellType = btnType;
                                    break;
                                case "CK":	// CheckBox 셀 설정
                                    baseGrid.Sheets[0].Cells[j, i].CellType = new CheckBoxCellType();
                                    break;
                                case "CC":	// Currency 셀 설정
                                    baseGrid.Sheets[0].Cells[j, i].CellType = new CurrencyCellType();
                                    break;
                                case "DT":	// Date 셀 설정(년월일)
                                    baseGrid.Sheets[0].Cells[j, i].CellType = new DateTimeCellType();
                                    break;
                                case "DM":	// Date 셀 설정(년월)                            
                                    FarPoint.Win.Spread.CellType.DateTimeCellType UseDt = new FarPoint.Win.Spread.CellType.DateTimeCellType();
                                    UseDt.DateTimeFormat = FarPoint.Win.Spread.CellType.DateTimeFormat.UserDefined;
                                    UseDt.UserDefinedFormat = "yyyy/MM";
                                    baseGrid.Sheets[0].Cells[j, i].CellType = UseDt;
                                    break;
                                case "DY":	// Date 셀 설정(년)                            
                                    FarPoint.Win.Spread.CellType.DateTimeCellType UseDy = new FarPoint.Win.Spread.CellType.DateTimeCellType();
                                    UseDy.DateTimeFormat = FarPoint.Win.Spread.CellType.DateTimeFormat.UserDefined;
                                    UseDy.UserDefinedFormat = "yyyy/MM";
                                    baseGrid.Sheets[0].Cells[j, i].CellType = UseDy;
                                    break;
                                case "DD":	// Date 셀 설정(월)                            
                                    FarPoint.Win.Spread.CellType.DateTimeCellType UseDd = new FarPoint.Win.Spread.CellType.DateTimeCellType();
                                    UseDd.DateTimeFormat = FarPoint.Win.Spread.CellType.DateTimeFormat.UserDefined;
                                    UseDd.UserDefinedFormat = "yyyy/MM";
                                    baseGrid.Sheets[0].Cells[j, i].CellType = UseDd;
                                    break;
                                case "PW":	// Password 셀 설정
                                    FarPoint.Win.Spread.CellType.TextCellType passType = new FarPoint.Win.Spread.CellType.TextCellType();
                                    passType.PasswordChar = Convert.ToChar("*");
                                    baseGrid.Sheets[0].Cells[j, i].CellType = passType;
                                    break;
                                case "MK":	// MaskCellType 셀 설정 | Mask 사용예 : MK###-###-####
                                    FarPoint.Win.Spread.CellType.MaskCellType picType = new FarPoint.Win.Spread.CellType.MaskCellType();
                                    if (shtCellType[i].Length > 2)
                                        picType.Mask = shtCellType[i].Substring(2, shtCellType[i].Length - 2).ToString();
                                    picType.MaskChar = Convert.ToChar("_");
                                    baseGrid.Sheets[0].Cells[j, i].CellType = picType;
                                    break;
                                case "NM":	// Number 셀 설정
                                    int Place = 0;
                                    if (shtCellType[i].Length == 3)
                                        Place = Convert.ToInt32(shtCellType[i].Substring(2, 1).ToString());
                                    FarPoint.Win.Spread.CellType.NumberCellType num = new FarPoint.Win.Spread.CellType.NumberCellType();
                                    num.DecimalSeparator = ".";
                                    num.DecimalPlaces = Place;
                                    num.FixedPoint = true;
                                    num.Separator = ",";
                                    num.ShowSeparator = true;
                                    num.MaximumValue = 99999999999999;
                                    num.MinimumValue = -99999999999999;
                                    //num.LeadingZero = FarPoint.Win.Spread.CellType.LeadingZero.No;
                                    baseGrid.Sheets[0].Cells[j, i].CellType = num;

                                    /* LeadingZero 를쓰면 소숫자리수가 표현되서 강제 코딩처리함 : 화면상 0값은 안보이게 처리함...   */
                                    if (Convert.ToDecimal(baseGrid.Sheets[0].Cells[j, i].Value) == 0)
                                    {
                                        baseGrid.Sheets[0].Cells[j, i].Value = null;
                                    }

                                    break;
                                case "HL":	// HyperLink 셀 설정
                                    baseGrid.Sheets[0].Cells[j, i].CellType = new HyperLinkCellType();
                                    break;
                                case "PG":	// Progress 셀 설정
                                    baseGrid.Sheets[0].Cells[j, i].CellType = new ProgressCellType();
                                    break;
                                case "RT":	// RichText 셀 설정
                                    baseGrid.Sheets[0].Cells[j, i].CellType = new RichTextCellType();
                                    break;
                                case "SC":	// SliderCell 셀 설정
                                    baseGrid.Sheets[0].Cells[j, i].CellType = new SliderCellType();
                                    break;
                                case "NL":	// 필수입력 사항 체크하지 않는 로직을 탄다
                                    baseGrid.Sheets[0].Cells[j, i].CellType = new GeneralCellType();
                                    baseGrid.Sheets[0].Cells[j, i].CanFocus = false;
                                    if (shtCellType[i].ToString() == "NLV") // NLV인 경우 숨김
                                        baseGrid.ActiveSheet.Columns[i].Visible = false;
                                    break;
                                case "PC":	// Percent 셀 설정
                                    baseGrid.Sheets[0].Cells[j, i].CellType = new PercentCellType();
                                    break;
                                case "CB":	// ComboBox 셀 설정
                                    for (int k = 0; k < shtComboBoxMsg.Length; k++)
                                    {
                                        if (i == Convert.ToInt32(shtComboBoxMsg[k].Substring(0, shtComboBoxMsg[k].IndexOf(":"))))
                                        {
                                            ComboBoxCellType comboType = new ComboBoxCellType();
                                            comboType.MaxDrop = 20;

                                            Regex rx = new Regex("#");
                                            string Tmp = shtComboBoxMsg[k].Substring(shtComboBoxMsg[k].IndexOf(":") + 1, shtComboBoxMsg[k].Length - shtComboBoxMsg[k].IndexOf(":") - 1);

                                            string Tmp1 = "";
                                            string Tmp2 = "";
                                            if (Tmp.IndexOf("|") > 0)
                                            {
                                                Tmp1 = Tmp.Substring(0, Tmp.IndexOf("|"));
                                                Tmp2 = Tmp.Substring(Tmp.IndexOf("|") + 1, Tmp.Length - Tmp.IndexOf("|") - 1);
                                                string[] cboMsg1 = rx.Split(Tmp1);
                                                string[] cboMsg2 = rx.Split(Tmp2);

                                                comboType.ItemData = cboMsg1;	// Key, Text
                                                comboType.Items = cboMsg2;		// Value
                                                comboType.EditorValue = FarPoint.Win.Spread.CellType.EditorValue.ItemData;

                                                baseGrid.Sheets[0].Cells[j, i].CellType = comboType;
                                            }
                                            else
                                            {
                                                string[] cboMsg1 = rx.Split(Tmp);

                                                comboType.Items = cboMsg1;
                                                baseGrid.Sheets[0].Cells[j, i].CellType = comboType;
                                            }
                                            if (shtCellType[i].ToString() == "CBV") // NLV인 경우 숨김
                                                baseGrid.ActiveSheet.Columns[i].Visible = false;
                                        }
                                    }
                                    break;
                                case "GN":	// General & 영문 대문자로 변환
                                    baseGrid.Sheets[0].Cells[j, i].CellType = new TextCellType();
                                    break;
                                default:	// General 설정
                                    baseGrid.Sheets[0].Cells[j, i].CellType = new TextCellType();
                                    FarPoint.Win.Spread.CellType.TextCellType textCellType1 = new FarPoint.Win.Spread.CellType.TextCellType();
                                    textCellType1.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
                                    baseGrid.Sheets[0].Columns.Get(i).CellType = textCellType1;
                                    break;
                            }
                        }
                    }
                    /****************************************************************************/

                    /********************** Column 정렬 설정 **************************************/
                    if (sheetAlign.Length > i)
                    {
                        switch (sheetAlign[i])
                        {
                            case "L":	// 왼쪽 정렬 MIDDLE
                                baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
                                baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
                                break;
                            case "LT":	// 왼쪽 정렬 TOP
                                baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
                                baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                                break;
                            case "LB":	// 왼쪽 정렬 BOTTOM
                                baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
                                baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Bottom;
                                break;
                            case "C": // 가운데 정렬
                                baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
                                baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
                                break;
                            case "CT":	// 가운데 정렬 TOP
                                baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
                                baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                                break;
                            case "CB":	// 가운데 정렬 BOTTOM
                                baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
                                baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Bottom;
                                break;
                            case "R":	// 오른쪽 정렬
                                baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;
                                baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
                                break;
                            case "RT":	// 오른쪽 정렬 TOP
                                baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;
                                baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                                break;
                            case "RB":	// 오른쪽 정렬 BOTTOM
                                baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;
                                baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Bottom;
                                break;
                            default:	// 디폴트 왼쪽 정렬
                                baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
                                baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
                                break;
                        }

                        for (int j = 0; j < baseGrid.Sheets[0].Rows.Count; j++)
                        {
                            switch (sheetAlign[i])
                            {
                                case "L":	// 왼쪽 정렬 MIDDLE
                                    baseGrid.Sheets[0].Cells[j, i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
                                    baseGrid.Sheets[0].Cells[j, i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
                                    break;
                                case "LT":	// 왼쪽 정렬 TOP
                                    baseGrid.Sheets[0].Cells[j, i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
                                    baseGrid.Sheets[0].Cells[j, i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                                    break;
                                case "LB":	// 왼쪽 정렬 BOTTOM
                                    baseGrid.Sheets[0].Cells[j, i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
                                    baseGrid.Sheets[0].Cells[j, i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Bottom;
                                    break;
                                case "C": // 가운데 정렬
                                    baseGrid.Sheets[0].Cells[j, i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
                                    baseGrid.Sheets[0].Cells[j, i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
                                    break;
                                case "CT":	// 가운데 정렬 TOP
                                    baseGrid.Sheets[0].Cells[j, i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
                                    baseGrid.Sheets[0].Cells[j, i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                                    break;
                                case "CB":	// 가운데 정렬 BOTTOM
                                    baseGrid.Sheets[0].Cells[j, i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
                                    baseGrid.Sheets[0].Cells[j, i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Bottom;
                                    break;
                                case "R":	// 오른쪽 정렬
                                    baseGrid.Sheets[0].Cells[j, i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;
                                    baseGrid.Sheets[0].Cells[j, i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
                                    break;
                                case "RT":	// 오른쪽 정렬 TOP
                                    baseGrid.Sheets[0].Cells[j, i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;
                                    baseGrid.Sheets[0].Cells[j, i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                                    break;
                                case "RB":	// 오른쪽 정렬 BOTTOM
                                    baseGrid.Sheets[0].Cells[j, i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;
                                    baseGrid.Sheets[0].Cells[j, i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Bottom;
                                    break;
                                default:	// 디폴트 왼쪽 정렬
                                    baseGrid.Sheets[0].Cells[j, i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
                                    baseGrid.Sheets[0].Cells[j, i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
                                    break;
                            }
                        }
                    }
                    /****************************************************************************/

                    /********************** Column 색상 및 Lock 설정 **************************************/
                    if (shtCellColor.Length > i)
                    {
                        switch (shtCellColor[i])
                        {
                            case 0:	// 일반
                                baseGrid.Sheets[0].Columns[i].BackColor = Color.White;
                                baseGrid.Sheets[0].Columns[i].Locked = false;
                                baseGrid.Sheets[0].Columns[i].CanFocus = true;
                                break;
                            case 1:	// 필수입력
                                baseGrid.Sheets[0].Columns[i].BackColor = SystemBase.Validation.Kind_LightCyan; //System.Drawing.Color.FromArgb(242, 252, 254);
                                baseGrid.Sheets[0].Columns[i].Locked = false;
                                baseGrid.Sheets[0].Columns[i].CanFocus = true;
                                break;
                            case 2:	// 읽기전용, 필수
                                baseGrid.Sheets[0].Columns[i].BackColor = System.Drawing.Color.FromArgb(239, 239, 239);
                                baseGrid.Sheets[0].Columns[i].Locked = true;
                                baseGrid.Sheets[0].Columns[i].CanFocus = true;
                                break;
                            case 3:	// 읽기전용, 필수항목에서 제외
                                baseGrid.Sheets[0].Columns[i].BackColor = System.Drawing.Color.FromArgb(238, 238, 238);
                                baseGrid.Sheets[0].Columns[i].Locked = true;
                                baseGrid.Sheets[0].Columns[i].CanFocus = true;
                                break;
                            case 4:	// 읽기전용, 팝업전용
                                baseGrid.Sheets[0].Columns[i].BackColor = Color.White;
                                baseGrid.Sheets[0].Columns[i].Locked = true;
                                baseGrid.Sheets[0].Columns[i].CanFocus = true;
                                break;
                            case 5:	// 읽기전용, 필수항목에서 제외, Focus 제외
                                baseGrid.Sheets[0].Columns[i].BackColor = System.Drawing.Color.FromArgb(238, 238, 238);
                                baseGrid.Sheets[0].Columns[i].Locked = true;
                                baseGrid.Sheets[0].Columns[i].CanFocus = false;
                                break;
                            case 6:	// 읽기전용, 필수항목, Focus 제외
                                baseGrid.Sheets[0].Columns[i].BackColor = System.Drawing.Color.FromArgb(239, 239, 239);
                                baseGrid.Sheets[0].Columns[i].Locked = true;
                                baseGrid.Sheets[0].Columns[i].CanFocus = false;
                                break;
                            default:	// 일반
                                baseGrid.Sheets[0].Columns[i].BackColor = Color.White;
                                baseGrid.Sheets[0].Columns[i].Locked = false;
                                baseGrid.Sheets[0].Columns[i].CanFocus = true;
                                break;
                        }
                        for (int j = 0; j < baseGrid.Sheets[0].Rows.Count; j++)
                        {
                            switch (shtCellColor[i])
                            {
                                case 0:	// 일반
                                    baseGrid.Sheets[0].Cells[j, i].BackColor = Color.White;
                                    baseGrid.Sheets[0].Cells[j, i].Locked = false;
                                    baseGrid.Sheets[0].Cells[j, i].CanFocus = true;
                                    break;
                                case 1:	// 필수입력
                                    baseGrid.Sheets[0].Cells[j, i].BackColor = SystemBase.Validation.Kind_LightCyan; //System.Drawing.Color.FromArgb(242, 252, 254);
                                    baseGrid.Sheets[0].Cells[j, i].Locked = false;
                                    baseGrid.Sheets[0].Cells[j, i].CanFocus = true;
                                    break;
                                case 2:	// 읽기전용, 필수
                                    baseGrid.Sheets[0].Cells[j, i].BackColor = System.Drawing.Color.FromArgb(239, 239, 239);
                                    baseGrid.Sheets[0].Cells[j, i].Locked = true;
                                    baseGrid.Sheets[0].Cells[j, i].CanFocus = true;
                                    break;
                                case 3:	// 읽기전용, 필수항목에서 제외
                                    baseGrid.Sheets[0].Cells[j, i].BackColor = System.Drawing.Color.FromArgb(238, 238, 238);
                                    baseGrid.Sheets[0].Cells[j, i].Locked = true;
                                    baseGrid.Sheets[0].Cells[j, i].CanFocus = true;
                                    break;
                                case 4:	// 읽기전용, 팝업전용
                                    baseGrid.Sheets[0].Cells[j, i].BackColor = Color.White;
                                    baseGrid.Sheets[0].Cells[j, i].Locked = true;
                                    baseGrid.Sheets[0].Cells[j, i].CanFocus = true;
                                    break;
                                case 5:	// 읽기전용, 필수항목에서 제외, Focus 제외
                                    baseGrid.Sheets[0].Cells[j, i].BackColor = System.Drawing.Color.FromArgb(238, 238, 238);
                                    baseGrid.Sheets[0].Cells[j, i].Locked = true;
                                    baseGrid.Sheets[0].Cells[j, i].CanFocus = false;
                                    break;
                                case 6:	// 읽기전용, 필수항목, Focus 제외
                                    baseGrid.Sheets[0].Cells[j, i].BackColor = System.Drawing.Color.FromArgb(239, 239, 239);
                                    baseGrid.Sheets[0].Cells[j, i].Locked = true;
                                    baseGrid.Sheets[0].Cells[j, i].CanFocus = false;
                                    break;
                                default:	// 일반
                                    baseGrid.Sheets[0].Cells[j, i].BackColor = Color.White;
                                    baseGrid.Sheets[0].Cells[j, i].Locked = false;
                                    baseGrid.Sheets[0].Cells[j, i].CanFocus = true;
                                    break;
                            }
                        }
                    }
                    /********************** Column Type 설정 **************************************/

                    /********************** Summery 하단 고정합계 **************************************/
                    if (dt.Rows.Count > 0 && shtSummary == true)
                    {
                        if (i == 1)
                        {
                            baseGrid.Sheets[0].FrozenTrailingRowCount = 1;	//하단 Column 1줄 고정

                            baseGrid.Sheets[0].RowHeader.Cells[dt.Rows.Count, 0].Text = "합계";
                            baseGrid.Sheets[0].Rows[dt.Rows.Count].BackColor = System.Drawing.Color.FromArgb(242, 244, 246); // .FromName("Beige");
                            baseGrid.Sheets[0].Rows[dt.Rows.Count].Locked = true;
                        }
                        FarPoint.Win.ComplexBorder complexBorder1 = new FarPoint.Win.ComplexBorder(new FarPoint.Win.ComplexBorderSide(FarPoint.Win.ComplexBorderSideStyle.None), new FarPoint.Win.ComplexBorderSide(FarPoint.Win.ComplexBorderSideStyle.ThinLine, System.Drawing.Color.FromArgb(((System.Byte)(100)), ((System.Byte)(100)), ((System.Byte)(100)))), new FarPoint.Win.ComplexBorderSide(FarPoint.Win.ComplexBorderSideStyle.None), new FarPoint.Win.ComplexBorderSide(FarPoint.Win.ComplexBorderSideStyle.None));
                        baseGrid.Sheets[0].Cells.Get(dt.Rows.Count, i).Border = complexBorder1;

                        if (baseGrid.Sheets[0].GetCellType(dt.Rows.Count, i).ToString() == "NumberCellType" ||
                            baseGrid.Sheets[0].GetCellType(dt.Rows.Count, i).ToString() == "PercentCellType")
                        {
                            string Str = IntToString(i);
                            string Area = Str + "1:" + Str + dt.Rows.Count.ToString();
                            Cell r = baseGrid.ActiveSheet.Cells[dt.Rows.Count, i];

                            r.Formula = "SUM(" + Area + ")";
                        }
                        else
                        {
                            baseGrid.Sheets[0].Cells[dt.Rows.Count, i].CellType = new TextCellType();
                        }
                    }
                    /****************************************************************************/

                }
            }
            catch (Exception e)
            {
                SystemBase.Loggers.Log("grdMakeSheet (그리드 데이타 조회중 에러(Frozen Row, Colunm) )", e.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY013"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                baseGrid.Sheets[0].Rows.Count = 0;
            }
        }
        #endregion

        #region grdCommSheet(14) - 그리드 데이타 조회 후 Frozen Row 또는 Colunm 정의 20070501 QUERY용
        public static void grdCommSheet(FarPoint.Win.Spread.FpSpread baseGrid
            , string Query
            , string[] sheetTitle
            , string[] sheetTitle2
            , string[] sheetTitle3
            , int[] sheetWidth
            , string[] sheetAlign
            , string[] shtCellType
            , int[] shtCellColor
            , string[] G1Etc
            , int shtHeaderRowCount
            , int shtFixR
            , int shtFixC
            , bool shtSummary
            )
        {
            try
            {
                /********************** 그리드 초기화 **************************************/
                baseGrid.Reset();
                //********************** Clipboard False***********************************/
                baseGrid.AutoClipboard = false;
                /********************** 폰트 정의 **************************************/
                baseGrid.Font = new System.Drawing.Font("돋움", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
                /********************** 그리드 스타일 정의 **************************************/
                baseGrid.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
                /********************** 스크롤바 정의 **************************************/
                baseGrid.HorizontalScrollBarPolicy = FarPoint.Win.Spread.ScrollBarPolicy.AsNeeded;
                /********************** 스크롤바 정의 **************************************/
                baseGrid.VerticalScrollBarPolicy = FarPoint.Win.Spread.ScrollBarPolicy.AsNeeded;
                /********************** 스크롤바 정의 **************************************/
                baseGrid.ScrollBarTrackPolicy = FarPoint.Win.Spread.ScrollBarTrackPolicy.Both;
                /********************** 그리드 칼럼수 지정 **************************************/
                baseGrid.ActiveSheet.Columns.Count = sheetTitle.Length;
                /********************** 그리드 Row수 지정 **************************************/
                //baseGrid.ActiveSheet.Rows.Count = 0;
                /********************** 그리드 Row색상 지정 **************************************/
                baseGrid.Sheets[0].AlternatingRows[0].BackColor = Color.FromName("WhiteSmoke");
                /********************** 그리드 RowHeader 넓이 지정 *********************************/
                baseGrid.Sheets[0].RowHeader.Columns[0].Width = 32;
                /********************** 그리드 바탕색상 지정 **************************************/
                baseGrid.Sheets[0].GrayAreaBackColor = Color.FromName("white");
                /********************** 1Row 선택 기준(단일, 1Row별, 1Cell별) **************************************/
                baseGrid.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.Normal;				//.ExtendedSelect;
                /********************** ColumnHeader 수 지정**************************************/
                baseGrid.Sheets[0].ColumnHeader.RowCount = shtHeaderRowCount;
                /********************** HorizontalGridLine 가로 가이드라인 색상 지정**************************************/
                baseGrid.Sheets[0].HorizontalGridLine = new FarPoint.Win.Spread.GridLine(FarPoint.Win.Spread.GridLineType.Flat, Color.FromArgb(193, 201, 212));
                /********************** VerticalGridLine 세로 가이드라인 색상 지정**************************************/
                baseGrid.Sheets[0].VerticalGridLine = new FarPoint.Win.Spread.GridLine(FarPoint.Win.Spread.GridLineType.Flat, Color.FromArgb(193, 201, 212));
                /********************** 왼쪽 상단 코너 색상 지정**************************************/
                baseGrid.Sheets[0].SheetCornerStyle.BackColor = System.Drawing.Color.FromArgb(242, 244, 246); // .FromName("Beige");
                /********************** 첫번째 Head color지정**************************************/
                baseGrid.Sheets[0].ColumnHeader.Rows[0].BackColor = System.Drawing.Color.FromArgb(242, 244, 246); // .FromName("Beige");
                /********************** 두번째 Head color지정**************************************/
                if (baseGrid.Sheets[0].ColumnHeader.RowCount > 1)
                    baseGrid.Sheets[0].ColumnHeader.Rows[1].BackColor = System.Drawing.Color.FromArgb(242, 244, 246); // .FromName("Beige");
                /********************** 세번째 Head color지정**************************************/
                if (baseGrid.Sheets[0].ColumnHeader.RowCount > 2)
                    baseGrid.Sheets[0].ColumnHeader.Rows[2].BackColor = System.Drawing.Color.FromArgb(242, 244, 246); // .FromName("Beige");

                /********************** 첫번째 Colum color 지정**************************************/
                baseGrid.Sheets[0].RowHeader.Columns[0].BackColor = System.Drawing.Color.FromArgb(242, 244, 246); // .FromName("Beige");
                /********************** 1번째 Cell(프라이머리) 키 숨김 지정**************************************/
                baseGrid.ActiveSheet.Columns[0].Visible = false;
                /********************** 입력시 기존 데이타 삭제후 입력**************************************/
                baseGrid.EditModeReplace = true;
                /********************** Clipboard 복사시 Head값 미포함**************************************/
                baseGrid.ClipboardOptions = FarPoint.Win.Spread.ClipboardOptions.NoHeaders;
                ///********************** 입력 RowMode********************************************************/
                //baseGrid.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.RowMode;
                //********************** 그리드 focus 칼라***************************************************/
                baseGrid.FocusRenderer = new FarPoint.Win.Spread.SolidFocusIndicatorRenderer(Color.CornflowerBlue, 2);
                //********************** 그리드 숫자 입력시 초기화 후 입력***********************************/
                baseGrid.EditModeReplace = true;

                if (Query != null)
                {

                    DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);

                    baseGrid.Sheets[0].DataSource = dt;

                    if (dt.Rows.Count > 0 && shtSummary == true)
                        baseGrid.ActiveSheet.Rows.Count = dt.Rows.Count + 1;//그리드Row수 지정
                    else
                    {
                        baseGrid.ActiveSheet.Rows.Count = dt.Rows.Count;//그리드Row수 지정
                    }

                    if (dt.Rows.Count > 999 && dt.Rows.Count < 10000)
                        baseGrid.Sheets[0].RowHeader.Columns[0].Width = 38;
                    else if (dt.Rows.Count > 9999)
                        baseGrid.Sheets[0].RowHeader.Columns[0].Width = 44;


                }
                else
                {
                    baseGrid.ActiveSheet.Rows.Count = 0;
                }
                /********************** 그리드 Head 높이 지정 **************************************/
                baseGrid.Sheets[0].ColumnHeader.Rows[0].Height = 28;
                if (shtHeaderRowCount > 1)
                {
                    /********************** 그리드 Head 높이 지정 **************************************/
                    baseGrid.Sheets[0].ColumnHeader.Rows[1].Height = 28;
                }
                if (shtHeaderRowCount > 2)
                {
                    /********************** 그리드 Head 높이 지정 **************************************/
                    baseGrid.Sheets[0].ColumnHeader.Rows[2].Height = 28;
                }

                //****************************************************************************************************************
                //: 고정 Column 또는 Row 설정
                //****************************************************************************************************************
                baseGrid.Sheets[0].FrozenColumnCount = shtFixC + 1;
                baseGrid.Sheets[0].FrozenRowCount = shtFixR;
                //****************************************************************************************************************
                //: 버튼 설정
                //****************************************************************************************************************
                FarPoint.Win.Spread.CellType.ButtonCellType btnType = new FarPoint.Win.Spread.CellType.ButtonCellType();
                btnType.Text = "...";

                //****************************************************************************************************************
                //: Key Enter시 Next Col로 Focus 이동
                //****************************************************************************************************************
                FarPoint.Win.Spread.InputMap im = baseGrid.GetInputMap(FarPoint.Win.Spread.InputMapMode.WhenFocused);
                im.Put(new FarPoint.Win.Spread.Keystroke(Keys.Enter, Keys.None), FarPoint.Win.Spread.SpreadActions.MoveToNextColumnWrap);


                int sheetLength = 0, shtTitleSpanTmp = 1, shtTitleSpanTmp2 = 1, shtTitleSpanTmp3 = 1;
                sheetLength = sheetTitle.Length;

                if (sheetTitle.Length < sheetTitle2.Length)
                    sheetLength = sheetTitle2.Length;

                //--------------------------------------------------------------------------------------------------------------------------

                for (int i = 0; i < sheetTitle.Length; i++)
                {
                    //****************************************************************************************************************
                    //: Column 폭 넓이 설정
                    //****************************************************************************************************************
                    if (sheetWidth.Length > i)
                        baseGrid.Sheets[0].Columns[i].Width = sheetWidth[i];

                    //****************************************************************************************************************
                    //: sheet 1번째 Header Text 입력
                    //****************************************************************************************************************
                    if (sheetTitle.Length > i)
                        baseGrid.Sheets[0].ColumnHeader.Cells[0, i].Text = sheetTitle[i].ToString();

                    //****************************************************************************************************************
                    //: sheet 2번째 Header Text 입력
                    //****************************************************************************************************************
                    if (sheetTitle2.Length > i && baseGrid.Sheets[0].ColumnHeader.RowCount > 1)
                        baseGrid.Sheets[0].ColumnHeader.Cells[1, i].Text = sheetTitle2[i].ToString();

                    //****************************************************************************************************************
                    //: sheet 3번째 Header Text 입력
                    //****************************************************************************************************************
                    if (sheetTitle3.Length > i && baseGrid.Sheets[0].ColumnHeader.RowCount > 2)
                        baseGrid.Sheets[0].ColumnHeader.Cells[2, i].Text = sheetTitle3[i].ToString();

                    //****************************************************************************************************************
                    //: columnSpan 설정
                    //****************************************************************************************************************
                    if (sheetTitle.Length > i)
                    {
                        if (shtHeaderRowCount > 0)
                        {
                            if (i > 1 && sheetTitle[i].ToString() == sheetTitle[i - 1].ToString())
                            {
                                shtTitleSpanTmp++;
                                baseGrid.Sheets[0].ColumnHeader.Cells[0, i + 1 - shtTitleSpanTmp].ColumnSpan = shtTitleSpanTmp;

                            }
                            else
                                shtTitleSpanTmp = 1;
                        }

                        if (shtHeaderRowCount > 1)
                        {
                            if (i > 1 && sheetTitle2[i].ToString() == sheetTitle2[i - 1].ToString())
                            {
                                shtTitleSpanTmp2++;
                                baseGrid.Sheets[0].ColumnHeader.Cells[1, i + 1 - shtTitleSpanTmp2].ColumnSpan = shtTitleSpanTmp2;
                            }
                            else
                                shtTitleSpanTmp2 = 1;
                        }

                        if (shtHeaderRowCount > 2)
                        {
                            if (i > 1 && sheetTitle3[i].ToString() == sheetTitle3[i - 1].ToString())
                            {
                                shtTitleSpanTmp3++;
                                baseGrid.Sheets[0].ColumnHeader.Cells[2, i + 1 - shtTitleSpanTmp3].ColumnSpan = shtTitleSpanTmp3;
                            }
                            else
                                shtTitleSpanTmp3 = 1;
                        }


                        if (sheetTitle2[i] != null)
                        {
                            if (sheetTitle[i].ToString() == sheetTitle2[i].ToString())
                            {
                                baseGrid.Sheets[0].ColumnHeader.Cells[0, i].RowSpan = 2;
                            }
                        }

                        if (sheetTitle2[i] != null && sheetTitle3[i] != null)
                        {
                            if (sheetTitle[i].ToString() == sheetTitle2[i].ToString() && sheetTitle2[i].ToString() == sheetTitle3[i].ToString())
                            {
                                baseGrid.Sheets[0].ColumnHeader.Cells[0, i].RowSpan = 3;
                            }
                            if (sheetTitle2[i].ToString() == sheetTitle3[i].ToString())
                            {
                                baseGrid.Sheets[0].ColumnHeader.Cells[1, i].RowSpan = 2;
                            }
                        }
                    }

                    /********************** Column Type 설정 **************************************/
                    if (shtCellType.Length > i)
                    {
                        string CellType = "";
                        if (shtCellType[i].Length > 2)
                            CellType = shtCellType[i].Substring(0, 2);
                        else
                            CellType = shtCellType[i];

                        switch (CellType)
                        {
                            case "BT":	// Button 셀 설정
                                baseGrid.Sheets[0].Columns[i].CellType = btnType;
                                break;
                            case "CK":	// CheckBox 셀 설정
                                baseGrid.Sheets[0].Columns[i].CellType = new CheckBoxCellType();
                                break;
                            case "CH":	// CheckBox Head에 추가
                                FarPoint.Win.Spread.CellType.CheckBoxCellType checkBoxCellType1 = new FarPoint.Win.Spread.CellType.CheckBoxCellType();
                                
                                if (baseGrid.Sheets[0].ColumnHeader.RowCount > 2)
                                {
                                    checkBoxCellType1.Caption = sheetTitle3[i];
                                    baseGrid.Sheets[0].ColumnHeader.Cells.Get(2, i).CellType = checkBoxCellType1;
                                    baseGrid.Sheets[0].ColumnHeader.Cells.Get(2, i).Border = new LineBorder(ColorTranslator.FromHtml("#9eb6ce"), 1, true, true, true, true);
                                    baseGrid.Sheets[0].Columns[i].CellType = new CheckBoxCellType();
                                }
                                else if (baseGrid.Sheets[0].ColumnHeader.RowCount > 1)
                                {
                                    checkBoxCellType1.Caption = sheetTitle2[i];
                                    baseGrid.Sheets[0].ColumnHeader.Cells.Get(1, i).CellType = checkBoxCellType1;
                                    baseGrid.Sheets[0].ColumnHeader.Cells.Get(1, i).Border = new LineBorder(ColorTranslator.FromHtml("#9eb6ce"), 1, true, true, true, true);
                                    baseGrid.Sheets[0].Columns[i].CellType = new CheckBoxCellType();
                                }
                                else
                                {
                                    checkBoxCellType1.Caption = sheetTitle[i];
                                    baseGrid.Sheets[0].ColumnHeader.Cells.Get(0, i).CellType = checkBoxCellType1;
                                    baseGrid.Sheets[0].ColumnHeader.Cells.Get(0, i).Border = new LineBorder(ColorTranslator.FromHtml("#9eb6ce"), 1, true, true, true, true);
                                    baseGrid.Sheets[0].Columns[i].CellType = new CheckBoxCellType();
                                }
                                break;
                            case "CC":	// Currency 셀 설정
                                baseGrid.Sheets[0].Columns[i].CellType = new CurrencyCellType();
                                break;
                            case "DT":	// Date 셀 설정
                                baseGrid.Sheets[0].Columns[i].CellType = new DateTimeCellType();
                                break;
                            case "DM":	// Date 셀 설정(년월)                            
                                FarPoint.Win.Spread.CellType.DateTimeCellType UseDt = new FarPoint.Win.Spread.CellType.DateTimeCellType();
                                UseDt.DateTimeFormat = FarPoint.Win.Spread.CellType.DateTimeFormat.UserDefined;
                                UseDt.UserDefinedFormat = "yyyy/MM";
                                baseGrid.Sheets[0].Columns[i].CellType = UseDt;
                                break;
                            case "DY":	// Date 셀 설정(년)                            
                                FarPoint.Win.Spread.CellType.DateTimeCellType UseDy = new FarPoint.Win.Spread.CellType.DateTimeCellType();
                                UseDy.DateTimeFormat = FarPoint.Win.Spread.CellType.DateTimeFormat.UserDefined;
                                UseDy.UserDefinedFormat = "yyyy";
                                baseGrid.Sheets[0].Columns[i].CellType = UseDy;
                                break;
                            case "DD":	// Date 셀 설정(월)                            
                                FarPoint.Win.Spread.CellType.DateTimeCellType UseDd = new FarPoint.Win.Spread.CellType.DateTimeCellType();
                                UseDd.DateTimeFormat = FarPoint.Win.Spread.CellType.DateTimeFormat.UserDefined;
                                UseDd.UserDefinedFormat = "MM";
                                baseGrid.Sheets[0].Columns[i].CellType = UseDd;
                                break;
                            case "PW":	// Password 셀 설정
                                FarPoint.Win.Spread.CellType.TextCellType passType = new FarPoint.Win.Spread.CellType.TextCellType();
                                passType.PasswordChar = Convert.ToChar("*");
                                baseGrid.Sheets[0].Columns[i].CellType = passType;
                                break;
                            case "MK":	// MaskCellType 셀 설정 | Mask 사용예 : MK###-###-####
                                FarPoint.Win.Spread.CellType.MaskCellType picType = new FarPoint.Win.Spread.CellType.MaskCellType();
                                if (G1Etc[i].ToString().Length > 0)
                                    picType.Mask = G1Etc[i].ToString();	//shtCellType[i].Substring(2,shtCellType[i].Length-2).ToString();
                                picType.MaskChar = Convert.ToChar("_");
                                baseGrid.Sheets[0].Columns[i].CellType = picType;
                                break;
                            case "NM":	// Number 셀 설정
                                int Place = 0;
                                if (shtCellType[i].Length == 3)
                                    Place = Convert.ToInt32(shtCellType[i].Substring(2, 1).ToString());
                                FarPoint.Win.Spread.CellType.NumberCellType num = new FarPoint.Win.Spread.CellType.NumberCellType();
                                //num.LeadingZero = FarPoint.Win.Spread.CellType.LeadingZero.No;
                                num.DecimalSeparator = ".";
                                num.DecimalPlaces = Place;
                                num.FixedPoint = true;
                                num.Separator = ",";
                                num.ShowSeparator = true;
                                num.MaximumValue = 99999999999999;
                                num.MinimumValue = -99999999999999;
                                baseGrid.Sheets[0].Columns[i].CellType = num;
                                break;
                            case "HL":	// HyperLink 셀 설정
                                baseGrid.Sheets[0].Columns[i].CellType = new HyperLinkCellType();
                                break;
                            case "PG":	// Progress 셀 설정
                                baseGrid.Sheets[0].Columns[i].CellType = new ProgressCellType();
                                break;
                            case "RT":	// RichText 셀 설정
                                baseGrid.Sheets[0].Columns[i].CellType = new RichTextCellType();
                                break;
                            case "SC":	// SliderCell 셀 설정
                                baseGrid.Sheets[0].Columns[i].CellType = new SliderCellType();
                                break;
                            case "NL":	// 필수입력 사항 체크하지 않는 로직을 탄다
                                baseGrid.Sheets[0].Columns[i].CellType = new TextCellType();
                                baseGrid.Sheets[0].Columns[i].CanFocus = false;
                                if (shtCellType[i].ToString() == "NLV") // NLV인 경우 숨김
                                    baseGrid.ActiveSheet.Columns[i].Visible = false;
                                break;
                            case "PC":	// Percent 셀 설정
                                baseGrid.Sheets[0].Columns[i].CellType = new PercentCellType();
                                break;
                            case "CB":	// ComboBox 셀 설정
                                if (G1Etc[i] != null && G1Etc[i].Length > 0)
                                {
                                    ComboBoxCellType comboType = new ComboBoxCellType();
                                    comboType.MaxDrop = 20;

                                    Regex rx = new Regex("#");
                                    string Tmp = G1Etc[i].ToString();	//.Substring(G1Etc[k].IndexOf(":")+1, G1Etc[k].Length - G1Etc[k].IndexOf(":")-1);

                                    string Tmp1 = "";
                                    string Tmp2 = "";
                                    if (Tmp.IndexOf("|") > 0)
                                    {
                                        Tmp1 = Tmp.Substring(0, Tmp.IndexOf("|"));
                                        Tmp2 = Tmp.Substring(Tmp.IndexOf("|") + 1, Tmp.Length - Tmp.IndexOf("|") - 1);
                                        string[] cboMsg1 = rx.Split(Tmp1);
                                        string[] cboMsg2 = rx.Split(Tmp2);

                                        comboType.ItemData = cboMsg1;	// Key, Text
                                        comboType.Items = cboMsg2;		// Value
                                        comboType.EditorValue = FarPoint.Win.Spread.CellType.EditorValue.ItemData;
                                        baseGrid.Sheets[0].Columns[i].CellType = comboType;
                                    }
                                    else
                                    {
                                        string[] cboMsg1 = rx.Split(Tmp);

                                        comboType.Items = cboMsg1;
                                        baseGrid.Sheets[0].Columns[i].CellType = comboType;
                                    }
                                    if (shtCellType[i].ToString() == "CBV") // NLV인 경우 숨김
                                        baseGrid.ActiveSheet.Columns[i].Visible = false;

                                }
                                break;
                            case "ML": //TextBox MultiLine
                                FarPoint.Win.Spread.CellType.TextCellType MultiType = new FarPoint.Win.Spread.CellType.TextCellType();
                                MultiType.Multiline = true;

                                baseGrid.Sheets[0].Columns[i].CellType = MultiType;
                                break;
                            case "GN":	// General & 영문 대문자로 변환
                                baseGrid.Sheets[0].Columns[i].CellType = new TextCellType();
                                break;
                            default:	// General 설정
                                baseGrid.Sheets[0].Columns[i].CellType = new TextCellType();
                                FarPoint.Win.Spread.CellType.TextCellType textCellType1 = new FarPoint.Win.Spread.CellType.TextCellType();
                                textCellType1.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
                                baseGrid.Sheets[0].Columns.Get(i).CellType = textCellType1;
                                break;
                        }
                        for (int j = 0; j < baseGrid.Sheets[0].Rows.Count; j++)
                        {
                            switch (CellType)
                            {
                                case "BT":	// Button 셀 설정
                                    baseGrid.Sheets[0].Cells[j, i].CellType = btnType;
                                    break;
                                case "CK":	// CheckBox 셀 설정
                                    baseGrid.Sheets[0].Cells[j, i].CellType = new CheckBoxCellType();
                                    break;
                                case "CH":	// CheckBox Head에 추가
                                    FarPoint.Win.Spread.CellType.CheckBoxCellType checkBoxCellType1 = new FarPoint.Win.Spread.CellType.CheckBoxCellType();
                                    checkBoxCellType1.Caption = sheetTitle[i];
                                    if (baseGrid.Sheets[0].ColumnHeader.RowCount > 2)
                                    {
                                        baseGrid.Sheets[0].ColumnHeader.Cells.Get(2, i).CellType = checkBoxCellType1;
                                        baseGrid.Sheets[0].ColumnHeader.Cells.Get(2, i).Border = new LineBorder(ColorTranslator.FromHtml("#9eb6ce"), 1, true, true, true, true);
                                        baseGrid.Sheets[0].Columns[i].CellType = new CheckBoxCellType();
                                    }
                                    else if (baseGrid.Sheets[0].ColumnHeader.RowCount > 1)
                                    {
                                        baseGrid.Sheets[0].ColumnHeader.Cells.Get(1, i).CellType = checkBoxCellType1;
                                        baseGrid.Sheets[0].ColumnHeader.Cells.Get(1, i).Border = new LineBorder(ColorTranslator.FromHtml("#9eb6ce"), 1, true, true, true, true);
                                        baseGrid.Sheets[0].Columns[i].CellType = new CheckBoxCellType();
                                    }
                                    else
                                    {
                                        baseGrid.Sheets[0].ColumnHeader.Cells.Get(0, i).CellType = checkBoxCellType1;
                                        baseGrid.Sheets[0].ColumnHeader.Cells.Get(0, i).Border = new LineBorder(ColorTranslator.FromHtml("#9eb6ce"), 1, true, true, true, true);
                                        baseGrid.Sheets[0].Columns[i].CellType = new CheckBoxCellType();
                                    }
                                    break;
                                case "CC":	// Currency 셀 설정
                                    baseGrid.Sheets[0].Cells[j, i].CellType = new CurrencyCellType();
                                    break;
                                case "DT":	// Date 셀 설정(년월일)
                                    baseGrid.Sheets[0].Cells[j, i].CellType = new DateTimeCellType();
                                    break;
                                case "DM":	// Date 셀 설정(년월)                            
                                    FarPoint.Win.Spread.CellType.DateTimeCellType UseDt = new FarPoint.Win.Spread.CellType.DateTimeCellType();
                                    UseDt.DateTimeFormat = FarPoint.Win.Spread.CellType.DateTimeFormat.UserDefined;
                                    UseDt.UserDefinedFormat = "yyyy/MM";
                                    baseGrid.Sheets[0].Cells[j, i].CellType = UseDt;
                                    break;
                                case "DY":	// Date 셀 설정(년)                            
                                    FarPoint.Win.Spread.CellType.DateTimeCellType UseDy = new FarPoint.Win.Spread.CellType.DateTimeCellType();
                                    UseDy.DateTimeFormat = FarPoint.Win.Spread.CellType.DateTimeFormat.UserDefined;
                                    UseDy.UserDefinedFormat = "yyyy";
                                    baseGrid.Sheets[0].Cells[j, i].CellType = UseDy;
                                    break;
                                case "DD":	// Date 셀 설정(월)                            
                                    FarPoint.Win.Spread.CellType.DateTimeCellType UseDd = new FarPoint.Win.Spread.CellType.DateTimeCellType();
                                    UseDd.DateTimeFormat = FarPoint.Win.Spread.CellType.DateTimeFormat.UserDefined;
                                    UseDd.UserDefinedFormat = "MM";
                                    baseGrid.Sheets[0].Cells[j, i].CellType = UseDd;
                                    break;

                                case "PW":	// Password 셀 설정
                                    FarPoint.Win.Spread.CellType.TextCellType passType = new FarPoint.Win.Spread.CellType.TextCellType();
                                    passType.PasswordChar = Convert.ToChar("*");
                                    baseGrid.Sheets[0].Cells[j, i].CellType = passType;
                                    break;
                                case "MK":	// MaskCellType 셀 설정 | Mask 사용예 : MK###-###-####
                                    FarPoint.Win.Spread.CellType.MaskCellType picType = new FarPoint.Win.Spread.CellType.MaskCellType();
                                    if (G1Etc[i].ToString().Length > 0)
                                        picType.Mask = G1Etc[i].ToString();	//shtCellType[i].Substring(2,shtCellType[i].Length-2).ToString();
                                    picType.MaskChar = Convert.ToChar("_");
                                    baseGrid.Sheets[0].Cells[j, i].CellType = picType;
                                    break;
                                case "NM":	// Number 셀 설정
                                    int Place = 0;
                                    if (shtCellType[i].Length == 3)
                                        Place = Convert.ToInt32(shtCellType[i].Substring(2, 1).ToString());
                                    FarPoint.Win.Spread.CellType.NumberCellType num = new FarPoint.Win.Spread.CellType.NumberCellType();
                                    //num.LeadingZero = FarPoint.Win.Spread.CellType.LeadingZero.No;
                                    num.DecimalSeparator = ".";
                                    num.DecimalPlaces = Place;
                                    num.FixedPoint = true;
                                    num.Separator = ",";
                                    num.ShowSeparator = true;
                                    num.MaximumValue = 99999999999999;
                                    num.MinimumValue = -99999999999999;
                                    baseGrid.Sheets[0].Cells[j, i].CellType = num;

                                    /* LeadingZero 를쓰면 소숫자리수가 표현되서 강제 코딩처리함 : 화면상 0값은 안보이게 처리함...   */
                                    if (Convert.ToDecimal(baseGrid.Sheets[0].Cells[j, i].Value) == 0)
                                    {
                                        baseGrid.Sheets[0].Cells[j, i].Value = null;
                                    }
                                    break;
                                case "HL":	// HyperLink 셀 설정
                                    baseGrid.Sheets[0].Cells[j, i].CellType = new HyperLinkCellType();
                                    break;
                                case "PG":	// Progress 셀 설정
                                    baseGrid.Sheets[0].Cells[j, i].CellType = new ProgressCellType();
                                    break;
                                case "RT":	// RichText 셀 설정
                                    baseGrid.Sheets[0].Cells[j, i].CellType = new RichTextCellType();
                                    break;
                                case "SC":	// SliderCell 셀 설정
                                    baseGrid.Sheets[0].Cells[j, i].CellType = new SliderCellType();
                                    break;
                                case "NL":	// 필수입력 사항 체크하지 않는 로직을 탄다
                                    baseGrid.Sheets[0].Cells[j, i].CellType = new TextCellType();
                                    baseGrid.Sheets[0].Cells[j, i].CanFocus = false;
                                    if (shtCellType[i].ToString() == "NLV") // NLV인 경우 숨김
                                        baseGrid.ActiveSheet.Columns[i].Visible = false;
                                    break;
                                case "PC":	// Percent 셀 설정
                                    baseGrid.Sheets[0].Cells[j, i].CellType = new PercentCellType();
                                    break;
                                case "CB":	// ComboBox 셀 설정
                                    if (G1Etc[i] != null && G1Etc[i].Length > 0)
                                    {
                                        ComboBoxCellType comboType = new ComboBoxCellType();
                                        comboType.MaxDrop = 20;

                                        Regex rx = new Regex("#");
                                        string Tmp = G1Etc[i].ToString();	//.Substring(G1Etc[k].IndexOf(":")+1, G1Etc[k].Length - G1Etc[k].IndexOf(":")-1);

                                        string Tmp1 = "";
                                        string Tmp2 = "";
                                        if (Tmp.IndexOf("|") > 0)
                                        {
                                            Tmp1 = Tmp.Substring(0, Tmp.IndexOf("|"));
                                            Tmp2 = Tmp.Substring(Tmp.IndexOf("|") + 1, Tmp.Length - Tmp.IndexOf("|") - 1);
                                            string[] cboMsg1 = rx.Split(Tmp1);
                                            string[] cboMsg2 = rx.Split(Tmp2);

                                            comboType.ItemData = cboMsg1;	// Key, Text
                                            comboType.Items = cboMsg2;		// Value
                                            comboType.EditorValue = FarPoint.Win.Spread.CellType.EditorValue.ItemData;

                                            baseGrid.Sheets[0].Cells[j, i].CellType = comboType;
                                        }
                                        else
                                        {
                                            string[] cboMsg1 = rx.Split(Tmp);

                                            comboType.Items = cboMsg1;
                                            baseGrid.Sheets[0].Cells[j, i].CellType = comboType;
                                        }
                                        if (shtCellType[i].ToString() == "CBV") // NLV인 경우 숨김
                                            baseGrid.ActiveSheet.Columns[i].Visible = false;
                                    }
                                    break;
                                case "ML": //TextBox MultiLine
                                    FarPoint.Win.Spread.CellType.TextCellType MultiType = new FarPoint.Win.Spread.CellType.TextCellType();
                                    MultiType.Multiline = true;

                                    baseGrid.Sheets[0].Columns[i].CellType = MultiType;
                                    break;
                                case "GN":	// General & 영문 대문자로 변환
                                    baseGrid.Sheets[0].Cells[j, i].CellType = new TextCellType();
                                    break;
                                default:	// General 설정
                                    baseGrid.Sheets[0].Cells[j, i].CellType = new TextCellType();
                                    FarPoint.Win.Spread.CellType.TextCellType textCellType1 = new FarPoint.Win.Spread.CellType.TextCellType();
                                    textCellType1.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
                                    baseGrid.Sheets[0].Columns.Get(i).CellType = textCellType1;
                                    break;
                            }
                        }
                    }
                    /****************************************************************************/

                    /********************** Column 정렬 설정 **************************************/
                    if (sheetAlign.Length > i)
                    {
                        switch (sheetAlign[i])
                        {
                            case "L":	// 왼쪽 정렬 MIDDLE
                                baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
                                baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
                                break;
                            case "LT":	// 왼쪽 정렬 TOP
                                baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
                                baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                                break;
                            case "LB":	// 왼쪽 정렬 BOTTOM
                                baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
                                baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Bottom;
                                break;
                            case "C": // 가운데 정렬
                                baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
                                baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
                                break;
                            case "CT":	// 가운데 정렬 TOP
                                baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
                                baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                                break;
                            case "CB":	// 가운데 정렬 BOTTOM
                                baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
                                baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Bottom;
                                break;
                            case "R":	// 오른쪽 정렬
                                baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;
                                baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
                                break;
                            case "RT":	// 오른쪽 정렬 TOP
                                baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;
                                baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                                break;
                            case "RB":	// 오른쪽 정렬 BOTTOM
                                baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;
                                baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Bottom;
                                break;
                            default:	// 디폴트 왼쪽 정렬
                                baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
                                baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
                                break;
                        }

                        for (int j = 0; j < baseGrid.Sheets[0].Rows.Count; j++)
                        {

                            switch (sheetAlign[i])
                            {
                                case "L":	// 왼쪽 정렬 MIDDLE
                                    baseGrid.Sheets[0].Cells[j, i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
                                    baseGrid.Sheets[0].Cells[j, i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
                                    break;
                                case "LT":	// 왼쪽 정렬 TOP
                                    baseGrid.Sheets[0].Cells[j, i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
                                    baseGrid.Sheets[0].Cells[j, i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                                    break;
                                case "LB":	// 왼쪽 정렬 BOTTOM
                                    baseGrid.Sheets[0].Cells[j, i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
                                    baseGrid.Sheets[0].Cells[j, i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Bottom;
                                    break;
                                case "C": // 가운데 정렬
                                    baseGrid.Sheets[0].Cells[j, i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
                                    baseGrid.Sheets[0].Cells[j, i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
                                    break;
                                case "CT":	// 가운데 정렬 TOP
                                    baseGrid.Sheets[0].Cells[j, i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
                                    baseGrid.Sheets[0].Cells[j, i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                                    break;
                                case "CB":	// 가운데 정렬 BOTTOM
                                    baseGrid.Sheets[0].Cells[j, i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
                                    baseGrid.Sheets[0].Cells[j, i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Bottom;
                                    break;
                                case "R":	// 오른쪽 정렬
                                    baseGrid.Sheets[0].Cells[j, i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;
                                    baseGrid.Sheets[0].Cells[j, i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
                                    break;
                                case "RT":	// 오른쪽 정렬 TOP
                                    baseGrid.Sheets[0].Cells[j, i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;
                                    baseGrid.Sheets[0].Cells[j, i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                                    break;
                                case "RB":	// 오른쪽 정렬 BOTTOM
                                    baseGrid.Sheets[0].Cells[j, i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;
                                    baseGrid.Sheets[0].Cells[j, i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Bottom;
                                    break;
                                default:	// 디폴트 왼쪽 정렬
                                    baseGrid.Sheets[0].Cells[j, i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
                                    baseGrid.Sheets[0].Cells[j, i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
                                    break;
                            }
                        }
                    }
                    /****************************************************************************/

                    /********************** Column 색상 및 Lock 설정 **************************************/
                    if (shtCellColor.Length > i)
                    {

                        switch (shtCellColor[i])
                        {
                            case 0:	// 일반
                                baseGrid.Sheets[0].Columns[i].BackColor = Color.White;
                                baseGrid.Sheets[0].Columns[i].Locked = false;
                                baseGrid.Sheets[0].Columns[i].CanFocus = true;
                                break;
                            case 1:	// 필수입력
                                baseGrid.Sheets[0].Columns[i].BackColor = SystemBase.Validation.Kind_LightCyan; //System.Drawing.Color.FromArgb(242, 252, 254);
                                baseGrid.Sheets[0].Columns[i].Locked = false;
                                baseGrid.Sheets[0].Columns[i].CanFocus = true;
                                break;
                            case 2:	// 읽기전용, 필수
                                baseGrid.Sheets[0].Columns[i].BackColor = System.Drawing.Color.FromArgb(239, 239, 239);
                                baseGrid.Sheets[0].Columns[i].Locked = true;
                                baseGrid.Sheets[0].Columns[i].CanFocus = true;
                                break;
                            case 3:	// 읽기전용, 필수항목에서 제외
                                baseGrid.Sheets[0].Columns[i].BackColor = System.Drawing.Color.FromArgb(238, 238, 238);
                                baseGrid.Sheets[0].Columns[i].Locked = true;
                                baseGrid.Sheets[0].Columns[i].CanFocus = true;
                                break;
                            case 4:	// 읽기전용, 팝업전용
                                baseGrid.Sheets[0].Columns[i].BackColor = Color.White;
                                baseGrid.Sheets[0].Columns[i].Locked = true;
                                baseGrid.Sheets[0].Columns[i].CanFocus = true;
                                break;
                            case 5:	// 읽기전용, 필수항목에서 제외, Focus 제외
                                baseGrid.Sheets[0].Columns[i].BackColor = System.Drawing.Color.FromArgb(238, 238, 238);
                                baseGrid.Sheets[0].Columns[i].Locked = true;
                                baseGrid.Sheets[0].Columns[i].CanFocus = false;
                                break;
                            case 6:	// 읽기전용, 필수항목, Focus 제외
                                baseGrid.Sheets[0].Columns[i].BackColor = System.Drawing.Color.FromArgb(239, 239, 239); 
                                baseGrid.Sheets[0].Columns[i].Locked = true;
                                baseGrid.Sheets[0].Columns[i].CanFocus = false;
                                break;
                            default:	// 일반
                                baseGrid.Sheets[0].Columns[i].BackColor = Color.White;
                                baseGrid.Sheets[0].Columns[i].Locked = false;
                                baseGrid.Sheets[0].Columns[i].CanFocus = true;
                                break;
                        }

                        //						for(int j=0; j < baseGrid.Sheets[0].Rows.Count; j++)
                        //						{
                        //							switch(shtCellColor[i])
                        //							{
                        //								case 0 :	// 일반
                        //									baseGrid.Sheets[0].Cells[j, i].BackColor = Color.White;
                        //									baseGrid.Sheets[0].Cells[j, i].Locked = false;
                        //									baseGrid.Sheets[0].Cells[j, i].CanFocus = true;
                        //									break;
                        //								case 1 :	// 필수입력
                        //									baseGrid.Sheets[0].Cells[j, i].BackColor = System.Drawing.Color.FromArgb(242, 252, 254);
                        //									baseGrid.Sheets[0].Cells[j, i].Locked = false;
                        //									baseGrid.Sheets[0].Cells[j, i].CanFocus = true;
                        //									break;
                        //								case 2 :	// 읽기전용, 필수
                        //									baseGrid.Sheets[0].Cells[j, i].BackColor = System.Drawing.Color.FromArgb(239, 239, 239);
                        //									baseGrid.Sheets[0].Cells[j, i].Locked = true;
                        //									baseGrid.Sheets[0].Cells[j, i].CanFocus = true;
                        //									break;
                        //								case 3 :	// 읽기전용, 필수항목에서 제외
                        //									baseGrid.Sheets[0].Cells[j, i].BackColor = System.Drawing.Color.FromArgb(238, 238, 238);
                        //									baseGrid.Sheets[0].Cells[j, i].Locked = true;
                        //									baseGrid.Sheets[0].Cells[j, i].CanFocus = true;
                        //									break;
                        //								case 4 :	// 읽기전용, 팝업전용
                        //									baseGrid.Sheets[0].Cells[j, i].BackColor = Color.White;
                        //									baseGrid.Sheets[0].Cells[j, i].Locked = true;
                        //									baseGrid.Sheets[0].Cells[j, i].CanFocus = true;
                        //									break;
                        //								case 5 :	// 읽기전용, 필수항목에서 제외, Focus 제외
                        //									baseGrid.Sheets[0].Cells[j, i].BackColor = System.Drawing.Color.FromArgb(238, 238, 238);
                        //									baseGrid.Sheets[0].Cells[j, i].Locked = true;
                        //									baseGrid.Sheets[0].Cells[j, i].CanFocus = false;
                        //									break;
                        //								case 6 :	// 읽기전용, 필수항목, Focus 제외
                        //									baseGrid.Sheets[0].Cells[j, i].BackColor = System.Drawing.Color.FromArgb(239, 239, 239);
                        //									baseGrid.Sheets[0].Cells[j, i].Locked = true;
                        //									baseGrid.Sheets[0].Cells[j, i].CanFocus = false;
                        //									break;
                        //								default :	// 일반
                        //									baseGrid.Sheets[0].Cells[j, i].BackColor = Color.White;
                        //									baseGrid.Sheets[0].Cells[j, i].Locked = false;
                        //									baseGrid.Sheets[0].Cells[j, i].CanFocus = true;
                        //									break;
                        //							}
                        //						}
                    }
                    /********************** Column Type 설정 **************************************/

                    /********************** Summery 하단 고정합계 **************************************/
                    if (sheetTitle.Length > 0 && shtSummary == true && baseGrid.Sheets[0].Rows.Count > 0)
                    {
                        if (i == 1)
                        {
                            baseGrid.Sheets[0].FrozenTrailingRowCount = 1;	//하단 Column 1줄 고정

                            baseGrid.Sheets[0].RowHeader.Cells[baseGrid.Sheets[0].Rows.Count - 1, 0].Text = "합계";
                            baseGrid.Sheets[0].Rows[baseGrid.Sheets[0].Rows.Count - 1].BackColor = System.Drawing.Color.FromArgb(242, 244, 246); // .FromName("Beige");
                            baseGrid.Sheets[0].Rows[baseGrid.Sheets[0].Rows.Count - 1].Locked = true;
                        }
                        FarPoint.Win.ComplexBorder complexBorder1 = new FarPoint.Win.ComplexBorder(new FarPoint.Win.ComplexBorderSide(FarPoint.Win.ComplexBorderSideStyle.None), new FarPoint.Win.ComplexBorderSide(FarPoint.Win.ComplexBorderSideStyle.ThinLine, System.Drawing.Color.FromArgb(((System.Byte)(100)), ((System.Byte)(100)), ((System.Byte)(100)))), new FarPoint.Win.ComplexBorderSide(FarPoint.Win.ComplexBorderSideStyle.None), new FarPoint.Win.ComplexBorderSide(FarPoint.Win.ComplexBorderSideStyle.None));
                        baseGrid.Sheets[0].Cells.Get(baseGrid.Sheets[0].Rows.Count - 1, i).Border = complexBorder1;

                        if (baseGrid.Sheets[0].GetCellType(baseGrid.Sheets[0].Rows.Count - 1, i).ToString() == "NumberCellType" ||
                            baseGrid.Sheets[0].GetCellType(baseGrid.Sheets[0].Rows.Count - 1, i).ToString() == "PercentCellType")
                        {
                            string Str = IntToString(i);
                            string Area = Str + "1:" + Str + Convert.ToString(baseGrid.Sheets[0].Rows.Count - 1);
                            Cell r = baseGrid.ActiveSheet.Cells[baseGrid.Sheets[0].Rows.Count - 1, i];

                            r.Formula = "SUM(" + Area + ")";
                        }
                        else
                        {
                            baseGrid.Sheets[0].Cells[baseGrid.Sheets[0].Rows.Count - 1, i].CellType = new TextCellType();
                        }
                    }
                    /****************************************************************************/
                }
                if (Query != null && baseGrid.Sheets[0].Rows.Count == 0)
                    MessageBox.Show(SystemBase.Base.MessageRtn("SY014"), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log("grdCommSheet (그리드공통 조회실패 )", Query.ToString() + "\n" + f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY013") + f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                baseGrid.Sheets[0].Rows.Count = 0;
            }
        }
        #endregion

        #region grdCommSheet(14) - 그리드 데이타 조회 후 Frozen Row 또는 Colunm 정의 20070501 DATATABLE용
        public static void grdCommSheet(
            DataTable dt
            , FarPoint.Win.Spread.FpSpread baseGrid
            , string[] sheetTitle
            , string[] sheetTitle2
            , string[] sheetTitle3
            , int[] sheetWidth
            , string[] sheetAlign
            , string[] shtCellType
            , int[] shtCellColor
            , string[] G1Etc
            , int shtHeaderRowCount
            , int shtFixR
            , int shtFixC
            , bool shtSummary
            )
        {
            try
            {
                /********************** 그리드 초기화 **************************************/
                baseGrid.Reset();
                //********************** Clipboard False***********************************/
                baseGrid.AutoClipboard = false;
                /********************** 폰트 정의 **************************************/
                baseGrid.Font = new System.Drawing.Font("돋움", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
                /********************** 그리드 스타일 정의 **************************************/
                baseGrid.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
                /********************** 스크롤바 정의 **************************************/
                baseGrid.HorizontalScrollBarPolicy = FarPoint.Win.Spread.ScrollBarPolicy.AsNeeded;
                /********************** 스크롤바 정의 **************************************/
                baseGrid.VerticalScrollBarPolicy = FarPoint.Win.Spread.ScrollBarPolicy.AsNeeded;
                /********************** 스크롤바 정의 **************************************/
                baseGrid.ScrollBarTrackPolicy = FarPoint.Win.Spread.ScrollBarTrackPolicy.Both;
                /********************** 그리드 칼럼수 지정 **************************************/
                baseGrid.ActiveSheet.Columns.Count = sheetTitle.Length;
                /********************** 그리드 Row수 지정 **************************************/
                //baseGrid.ActiveSheet.Rows.Count = 0;
                /********************** 그리드 Row색상 지정 **************************************/
                baseGrid.Sheets[0].AlternatingRows[0].BackColor = Color.FromName("WhiteSmoke");
                /********************** 그리드 RowHeader 넓이 지정 *********************************/
                if (dt != null && dt.Rows.Count > 999 && dt.Rows.Count < 10000)
                    baseGrid.Sheets[0].RowHeader.Columns[0].Width = 38;
                else if (dt != null && dt.Rows.Count > 9999)
                    baseGrid.Sheets[0].RowHeader.Columns[0].Width = 44;
                else
                    baseGrid.Sheets[0].RowHeader.Columns[0].Width = 32;
                /********************** 그리드 바탕색상 지정 **************************************/
                baseGrid.Sheets[0].GrayAreaBackColor = Color.FromName("white");
                /********************** 1Row 선택 기준(단일, 1Row별, 1Cell별) **************************************/
                baseGrid.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.Normal;				//.ExtendedSelect;
                /********************** ColumnHeader 수 지정**************************************/
                baseGrid.Sheets[0].ColumnHeader.RowCount = shtHeaderRowCount;
                /********************** HorizontalGridLine 가로 가이드라인 색상 지정**************************************/
                baseGrid.Sheets[0].HorizontalGridLine = new FarPoint.Win.Spread.GridLine(FarPoint.Win.Spread.GridLineType.Flat, Color.FromArgb(193, 201, 212));
                /********************** VerticalGridLine 세로 가이드라인 색상 지정**************************************/
                baseGrid.Sheets[0].VerticalGridLine = new FarPoint.Win.Spread.GridLine(FarPoint.Win.Spread.GridLineType.Flat, Color.FromArgb(193, 201, 212));
                /********************** 왼쪽 상단 코너 색상 지정**************************************/
                baseGrid.Sheets[0].SheetCornerStyle.BackColor = System.Drawing.Color.FromArgb(242, 244, 246); // .FromName("Beige");
                /********************** 첫번째 Head color지정**************************************/
                baseGrid.Sheets[0].ColumnHeader.Rows[0].BackColor = System.Drawing.Color.FromArgb(242, 244, 246); // .FromName("Beige");
                /********************** 두번째 Head color지정**************************************/
                if (baseGrid.Sheets[0].ColumnHeader.RowCount > 1)
                    baseGrid.Sheets[0].ColumnHeader.Rows[1].BackColor = System.Drawing.Color.FromArgb(242, 244, 246); // .FromName("Beige");
                /********************** 세번째 Head color지정**************************************/
                if (baseGrid.Sheets[0].ColumnHeader.RowCount > 2)
                    baseGrid.Sheets[0].ColumnHeader.Rows[2].BackColor = System.Drawing.Color.FromArgb(242, 244, 246); // .FromName("Beige");

                /********************** 첫번째 Colum color 지정**************************************/
                baseGrid.Sheets[0].RowHeader.Columns[0].BackColor = System.Drawing.Color.FromArgb(242, 244, 246); // .FromName("Beige");
                /********************** 1번째 Cell(프라이머리) 키 숨김 지정**************************************/
                baseGrid.ActiveSheet.Columns[0].Visible = false;
                /********************** 입력시 기존 데이타 삭제후 입력**************************************/
                baseGrid.EditModeReplace = true;
                /********************** Clipboard 복사시 Head값 미포함**************************************/
                baseGrid.ClipboardOptions = FarPoint.Win.Spread.ClipboardOptions.NoHeaders;
                ///********************** 입력 RowMode********************************************************/
                //baseGrid.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.RowMode;
                //********************** 그리드 focus 칼라***************************************************/
                baseGrid.FocusRenderer = new FarPoint.Win.Spread.SolidFocusIndicatorRenderer(Color.CornflowerBlue, 2);
                //********************** 그리드 숫자 입력시 초기화 후 입력***********************************/
                baseGrid.EditModeReplace = true;

                if (dt != null && dt.Rows.Count > 0)
                {
                    //DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);
                    baseGrid.Sheets[0].DataSource = dt;
                    if (dt.Rows.Count > 0 && shtSummary == true)
                        baseGrid.ActiveSheet.Rows.Count = dt.Rows.Count + 1;//그리드Row수 지정
                    else
                        baseGrid.ActiveSheet.Rows.Count = dt.Rows.Count;//그리드Row수 지정
                }
                else
                {
                    baseGrid.ActiveSheet.Rows.Count = 0;
                    //MessageBox.Show(SystemBase.Base.MessageRtn("SY011"));
                }
                /********************** 그리드 Head 높이 지정 **************************************/
                baseGrid.Sheets[0].ColumnHeader.Rows[0].Height = 28;
                if (shtHeaderRowCount > 1)
                {
                    /********************** 그리드 Head 높이 지정 **************************************/
                    baseGrid.Sheets[0].ColumnHeader.Rows[1].Height = 28;
                }
                if (shtHeaderRowCount > 2)
                {
                    /********************** 그리드 Head 높이 지정 **************************************/
                    baseGrid.Sheets[0].ColumnHeader.Rows[2].Height = 28;
                }

                //****************************************************************************************************************
                //: 고정 Column 또는 Row 설정
                //****************************************************************************************************************
                baseGrid.Sheets[0].FrozenColumnCount = shtFixC + 1;
                baseGrid.Sheets[0].FrozenRowCount = shtFixR;
                //****************************************************************************************************************
                //: 버튼 설정
                //****************************************************************************************************************
                FarPoint.Win.Spread.CellType.ButtonCellType btnType = new FarPoint.Win.Spread.CellType.ButtonCellType();
                btnType.Text = "...";

                //****************************************************************************************************************
                //: Key Enter시 Next Col로 Focus 이동
                //****************************************************************************************************************
                FarPoint.Win.Spread.InputMap im = baseGrid.GetInputMap(FarPoint.Win.Spread.InputMapMode.WhenFocused);
                im.Put(new FarPoint.Win.Spread.Keystroke(Keys.Enter, Keys.None), FarPoint.Win.Spread.SpreadActions.MoveToNextColumnWrap);


                int sheetLength = 0, shtTitleSpanTmp = 1, shtTitleSpanTmp2 = 1, shtTitleSpanTmp3 = 1;
                sheetLength = sheetTitle.Length;

                if (sheetTitle.Length < sheetTitle2.Length)
                    sheetLength = sheetTitle2.Length;

                //--------------------------------------------------------------------------------------------------------------------------

                for (int i = 0; i < sheetTitle.Length; i++)
                {
                    //****************************************************************************************************************
                    //: Column 폭 넓이 설정
                    //****************************************************************************************************************
                    if (sheetWidth.Length > i)
                    {
                        baseGrid.Sheets[0].Columns[i].Width = sheetWidth[i];
                    }

                    //****************************************************************************************************************
                    //: sheet 1번째 Header Text 입력
                    //****************************************************************************************************************
                    if (sheetTitle.Length > i)
                        baseGrid.Sheets[0].ColumnHeader.Cells[0, i].Text = sheetTitle[i].ToString();

                    //****************************************************************************************************************
                    //: sheet 2번째 Header Text 입력
                    //****************************************************************************************************************
                    if (sheetTitle2.Length > i && baseGrid.Sheets[0].ColumnHeader.RowCount > 1)
                        baseGrid.Sheets[0].ColumnHeader.Cells[1, i].Text = sheetTitle2[i].ToString();

                    //****************************************************************************************************************
                    //: sheet 3번째 Header Text 입력
                    //****************************************************************************************************************
                    if (sheetTitle3.Length > i && baseGrid.Sheets[0].ColumnHeader.RowCount > 2)
                        baseGrid.Sheets[0].ColumnHeader.Cells[2, i].Text = sheetTitle3[i].ToString();

                    //****************************************************************************************************************
                    //: columnSpan 설정
                    //****************************************************************************************************************
                    if (sheetTitle.Length > i)
                    {
                        if (shtHeaderRowCount > 0)
                        {
                            if (i > 1 && sheetTitle[i].ToString() == sheetTitle[i - 1].ToString())
                            {
                                shtTitleSpanTmp++;
                                baseGrid.Sheets[0].ColumnHeader.Cells[0, i + 1 - shtTitleSpanTmp].ColumnSpan = shtTitleSpanTmp;

                            }
                            else
                                shtTitleSpanTmp = 1;
                        }

                        if (shtHeaderRowCount > 1)
                        {
                            if (i > 1 && sheetTitle2[i].ToString() == sheetTitle2[i - 1].ToString())
                            {
                                shtTitleSpanTmp2++;
                                baseGrid.Sheets[0].ColumnHeader.Cells[1, i + 1 - shtTitleSpanTmp2].ColumnSpan = shtTitleSpanTmp2;
                            }
                            else
                                shtTitleSpanTmp2 = 1;
                        }

                        if (shtHeaderRowCount > 2)
                        {
                            if (i > 1 && sheetTitle3[i].ToString() == sheetTitle3[i - 1].ToString())
                            {
                                shtTitleSpanTmp3++;
                                baseGrid.Sheets[0].ColumnHeader.Cells[2, i + 1 - shtTitleSpanTmp3].ColumnSpan = shtTitleSpanTmp3;
                            }
                            else
                                shtTitleSpanTmp3 = 1;
                        }


                        if (sheetTitle2[i] != null)
                        {
                            if (sheetTitle[i].ToString() == sheetTitle2[i].ToString())
                            {
                                baseGrid.Sheets[0].ColumnHeader.Cells[0, i].RowSpan = 2;
                            }
                        }

                        if (sheetTitle2[i] != null && sheetTitle3[i] != null)
                        {
                            if (sheetTitle[i].ToString() == sheetTitle2[i].ToString() && sheetTitle2[i].ToString() == sheetTitle3[i].ToString())
                            {
                                baseGrid.Sheets[0].ColumnHeader.Cells[0, i].RowSpan = 3;
                            }
                            if (sheetTitle2[i].ToString() == sheetTitle3[i].ToString())
                            {
                                baseGrid.Sheets[0].ColumnHeader.Cells[1, i].RowSpan = 2;
                            }
                        }

                    }



                    /********************** Column Type 설정 **************************************/
                    if (shtCellType.Length > i)
                    {
                        string CellType = "";
                        if (shtCellType[i].Length > 2)
                            CellType = shtCellType[i].Substring(0, 2);
                        else
                            CellType = shtCellType[i];

                        switch (CellType)
                        {
                            case "BT":	// Button 셀 설정
                                baseGrid.Sheets[0].Columns[i].CellType = btnType;
                                break;
                            case "CK":	// CheckBox 셀 설정
                                baseGrid.Sheets[0].Columns[i].CellType = new CheckBoxCellType();
                                break;
                            case "CH":	// CheckBox Head에 추가
                                FarPoint.Win.Spread.CellType.CheckBoxCellType checkBoxCellType1 = new FarPoint.Win.Spread.CellType.CheckBoxCellType();
                                if (baseGrid.Sheets[0].ColumnHeader.RowCount > 2)
                                {
                                    checkBoxCellType1.Caption = sheetTitle3[i];
                                    baseGrid.Sheets[0].ColumnHeader.Cells.Get(2, i).CellType = checkBoxCellType1;
                                    baseGrid.Sheets[0].ColumnHeader.Cells.Get(2, i).Border = new LineBorder(ColorTranslator.FromHtml("#9eb6ce"), 1, true, true, true, true);
                                    baseGrid.Sheets[0].Columns[i].CellType = new CheckBoxCellType();
                                }
                                else if (baseGrid.Sheets[0].ColumnHeader.RowCount > 1)
                                {
                                    checkBoxCellType1.Caption = sheetTitle2[i];
                                    baseGrid.Sheets[0].ColumnHeader.Cells.Get(1, i).CellType = checkBoxCellType1;
                                    baseGrid.Sheets[0].ColumnHeader.Cells.Get(1, i).Border = new LineBorder(ColorTranslator.FromHtml("#9eb6ce"), 1, true, true, true, true);
                                    baseGrid.Sheets[0].Columns[i].CellType = new CheckBoxCellType();
                                }
                                else
                                {
                                    checkBoxCellType1.Caption = sheetTitle[i];
                                    baseGrid.Sheets[0].ColumnHeader.Cells.Get(0, i).CellType = checkBoxCellType1;
                                    baseGrid.Sheets[0].ColumnHeader.Cells.Get(0, i).Border = new LineBorder(ColorTranslator.FromHtml("#9eb6ce"), 1, true, true, true, true);
                                    baseGrid.Sheets[0].Columns[i].CellType = new CheckBoxCellType();
                                }
                                break;
                            case "CC":	// Currency 셀 설정
                                baseGrid.Sheets[0].Columns[i].CellType = new CurrencyCellType();
                                break;
                            case "DT":	// Date 셀 설정
                                baseGrid.Sheets[0].Columns[i].CellType = new DateTimeCellType();
                                break;
                            case "DM":	// Date 셀 설정(년월)                            
                                FarPoint.Win.Spread.CellType.DateTimeCellType UseDt = new FarPoint.Win.Spread.CellType.DateTimeCellType();
                                UseDt.DateTimeFormat = FarPoint.Win.Spread.CellType.DateTimeFormat.UserDefined;
                                UseDt.UserDefinedFormat = "yyyy/MM";
                                baseGrid.Sheets[0].Columns[i].CellType = UseDt;
                                break;
                            case "DY":	// Date 셀 설정(년)                            
                                FarPoint.Win.Spread.CellType.DateTimeCellType UseDy = new FarPoint.Win.Spread.CellType.DateTimeCellType();
                                UseDy.DateTimeFormat = FarPoint.Win.Spread.CellType.DateTimeFormat.UserDefined;
                                UseDy.UserDefinedFormat = "yyyy";
                                baseGrid.Sheets[0].Columns[i].CellType = UseDy;
                                break;
                            case "DD":	// Date 셀 설정(월)                            
                                FarPoint.Win.Spread.CellType.DateTimeCellType UseDd = new FarPoint.Win.Spread.CellType.DateTimeCellType();
                                UseDd.DateTimeFormat = FarPoint.Win.Spread.CellType.DateTimeFormat.UserDefined;
                                UseDd.UserDefinedFormat = "MM";
                                baseGrid.Sheets[0].Columns[i].CellType = UseDd;
                                break;
                            case "PW":	// Password 셀 설정
                                FarPoint.Win.Spread.CellType.TextCellType passType = new FarPoint.Win.Spread.CellType.TextCellType();
                                passType.PasswordChar = Convert.ToChar("*");
                                baseGrid.Sheets[0].Columns[i].CellType = passType;
                                break;
                            case "MK":	// MaskCellType 셀 설정 | Mask 사용예 : MK###-###-####
                                FarPoint.Win.Spread.CellType.MaskCellType picType = new FarPoint.Win.Spread.CellType.MaskCellType();
                                if (G1Etc[i].ToString().Length > 0)
                                    picType.Mask = G1Etc[i].ToString();	//shtCellType[i].Substring(2,shtCellType[i].Length-2).ToString();
                                picType.MaskChar = Convert.ToChar("_");
                                baseGrid.Sheets[0].Columns[i].CellType = picType;
                                break;
                            case "NM":	// Number 셀 설정
                                int Place = 0;
                                if (shtCellType[i].Length == 3)
                                    Place = Convert.ToInt32(shtCellType[i].Substring(2, 1).ToString());
                                FarPoint.Win.Spread.CellType.NumberCellType num = new FarPoint.Win.Spread.CellType.NumberCellType();
                                //num.LeadingZero = FarPoint.Win.Spread.CellType.LeadingZero.No;
                                num.DecimalSeparator = ".";
                                num.DecimalPlaces = Place;
                                num.FixedPoint = true;
                                num.Separator = ",";
                                num.ShowSeparator = true;
                                num.MaximumValue = 99999999999999;
                                num.MinimumValue = -99999999999999;
                                baseGrid.Sheets[0].Columns[i].CellType = num;
                                break;
                            case "HL":	// HyperLink 셀 설정
                                baseGrid.Sheets[0].Columns[i].CellType = new HyperLinkCellType();
                                break;
                            case "PG":	// Progress 셀 설정
                                baseGrid.Sheets[0].Columns[i].CellType = new ProgressCellType();
                                break;
                            case "RT":	// RichText 셀 설정
                                baseGrid.Sheets[0].Columns[i].CellType = new RichTextCellType();
                                break;
                            case "SC":	// SliderCell 셀 설정
                                baseGrid.Sheets[0].Columns[i].CellType = new SliderCellType();
                                break;
                            case "NL":	// 필수입력 사항 체크하지 않는 로직을 탄다
                                baseGrid.Sheets[0].Columns[i].CellType = new TextCellType();
                                baseGrid.Sheets[0].Columns[i].CanFocus = false;
                                if (shtCellType[i].ToString() == "NLV") // NLV인 경우 숨김
                                    baseGrid.ActiveSheet.Columns[i].Visible = false;
                                break;
                            case "PC":	// Percent 셀 설정
                                baseGrid.Sheets[0].Columns[i].CellType = new PercentCellType();
                                break;
                            case "CB":	// ComboBox 셀 설정
                                if (G1Etc[i].Length > 0)
                                {
                                    ComboBoxCellType comboType = new ComboBoxCellType();
                                    comboType.MaxDrop = 20;

                                    Regex rx = new Regex("#");
                                    string Tmp = G1Etc[i].ToString();	//.Substring(G1Etc[k].IndexOf(":")+1, G1Etc[k].Length - G1Etc[k].IndexOf(":")-1);

                                    string Tmp1 = "";
                                    string Tmp2 = "";
                                    if (Tmp.IndexOf("|") > 0)
                                    {
                                        Tmp1 = Tmp.Substring(0, Tmp.IndexOf("|"));
                                        Tmp2 = Tmp.Substring(Tmp.IndexOf("|") + 1, Tmp.Length - Tmp.IndexOf("|") - 1);
                                        string[] cboMsg1 = rx.Split(Tmp1);
                                        string[] cboMsg2 = rx.Split(Tmp2);

                                        comboType.ItemData = cboMsg1;	// Key, Text
                                        comboType.Items = cboMsg2;		// Value
                                        comboType.EditorValue = FarPoint.Win.Spread.CellType.EditorValue.ItemData;

                                        baseGrid.Sheets[0].Columns[i].CellType = comboType;
                                    }
                                    else
                                    {
                                        string[] cboMsg1 = rx.Split(Tmp);

                                        comboType.Items = cboMsg1;
                                        baseGrid.Sheets[0].Columns[i].CellType = comboType;
                                    }
                                    if (shtCellType[i].ToString() == "CBV") // NLV인 경우 숨김
                                        baseGrid.ActiveSheet.Columns[i].Visible = false;

                                }
                                break;
                            case "ML": //TextBox MultiLine
                                FarPoint.Win.Spread.CellType.TextCellType MultiType = new FarPoint.Win.Spread.CellType.TextCellType();
                                MultiType.Multiline = true;

                                baseGrid.Sheets[0].Columns[i].CellType = MultiType;
                                break;
                            case "GN":	// General & 영문 대문자로 변환
                                baseGrid.Sheets[0].Columns[i].CellType = new TextCellType();
                                break;
                            default:	// General 설정
                                baseGrid.Sheets[0].Columns[i].CellType = new TextCellType();
                                FarPoint.Win.Spread.CellType.TextCellType textCellType1 = new FarPoint.Win.Spread.CellType.TextCellType();
                                textCellType1.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
                                baseGrid.Sheets[0].Columns.Get(i).CellType = textCellType1;
                                break;
                        }

                        for (int j = 0; j < baseGrid.Sheets[0].Rows.Count; j++)
                        {

                            switch (CellType)
                            {
                                case "BT":	// Button 셀 설정
                                    baseGrid.Sheets[0].Cells[j, i].CellType = btnType;
                                    break;
                                case "CK":	// CheckBox 셀 설정
                                    baseGrid.Sheets[0].Cells[j, i].CellType = new CheckBoxCellType();
                                    break;
                                case "CH":	// CheckBox Head에 추가
                                    FarPoint.Win.Spread.CellType.CheckBoxCellType checkBoxCellType1 = new FarPoint.Win.Spread.CellType.CheckBoxCellType();
                                    checkBoxCellType1.Caption = sheetTitle[i];
                                    baseGrid.Sheets[0].ColumnHeader.Cells.Get(0, i).CellType = checkBoxCellType1;
                                    baseGrid.Sheets[0].ColumnHeader.Cells.Get(0, i).Border = new LineBorder(ColorTranslator.FromHtml("#9eb6ce"), 1, true, true, true, true);
                                    baseGrid.Sheets[0].Columns[i].CellType = new CheckBoxCellType();
                                    break;
                                case "CC":	// Currency 셀 설정
                                    baseGrid.Sheets[0].Cells[j, i].CellType = new CurrencyCellType();
                                    break;
                                case "DT":	// Date 셀 설정
                                    baseGrid.Sheets[0].Cells[j, i].CellType = new DateTimeCellType();
                                    break;
                                case "DM":	// Date 셀 설정(년월)                            
                                    FarPoint.Win.Spread.CellType.DateTimeCellType UseDt = new FarPoint.Win.Spread.CellType.DateTimeCellType();
                                    UseDt.DateTimeFormat = FarPoint.Win.Spread.CellType.DateTimeFormat.UserDefined;
                                    UseDt.UserDefinedFormat = "yyyy/MM";
                                    baseGrid.Sheets[0].Cells[j, i].CellType = UseDt;
                                    break;
                                case "DY":	// Date 셀 설정(년)                            
                                    FarPoint.Win.Spread.CellType.DateTimeCellType UseDy = new FarPoint.Win.Spread.CellType.DateTimeCellType();
                                    UseDy.DateTimeFormat = FarPoint.Win.Spread.CellType.DateTimeFormat.UserDefined;
                                    UseDy.UserDefinedFormat = "yyyy";
                                    baseGrid.Sheets[0].Cells[j, i].CellType = UseDy;
                                    break;
                                case "DD":	// Date 셀 설정(월)                            
                                    FarPoint.Win.Spread.CellType.DateTimeCellType UseDd = new FarPoint.Win.Spread.CellType.DateTimeCellType();
                                    UseDd.DateTimeFormat = FarPoint.Win.Spread.CellType.DateTimeFormat.UserDefined;
                                    UseDd.UserDefinedFormat = "MM";
                                    baseGrid.Sheets[0].Cells[j, i].CellType = UseDd;
                                    break;
                                case "PW":	// Password 셀 설정
                                    FarPoint.Win.Spread.CellType.TextCellType passType = new FarPoint.Win.Spread.CellType.TextCellType();
                                    passType.PasswordChar = Convert.ToChar("*");
                                    baseGrid.Sheets[0].Cells[j, i].CellType = passType;
                                    break;
                                case "MK":	// MaskCellType 셀 설정 | Mask 사용예 : MK###-###-####
                                    FarPoint.Win.Spread.CellType.MaskCellType picType = new FarPoint.Win.Spread.CellType.MaskCellType();
                                    if (G1Etc[i].ToString().Length > 0)
                                        picType.Mask = G1Etc[i].ToString();	//shtCellType[i].Substring(2,shtCellType[i].Length-2).ToString();
                                    picType.MaskChar = Convert.ToChar("_");
                                    baseGrid.Sheets[0].Cells[j, i].CellType = picType;
                                    break;
                                case "NM":	// Number 셀 설정
                                    int Place = 0;
                                    if (shtCellType[i].Length == 3)
                                        Place = Convert.ToInt32(shtCellType[i].Substring(2, 1).ToString());
                                    FarPoint.Win.Spread.CellType.NumberCellType num = new FarPoint.Win.Spread.CellType.NumberCellType();
                                    //num.LeadingZero = FarPoint.Win.Spread.CellType.LeadingZero.No;
                                    num.DecimalSeparator = ".";
                                    num.DecimalPlaces = Place;
                                    num.FixedPoint = true;
                                    num.Separator = ",";
                                    num.ShowSeparator = true;
                                    num.MaximumValue = 99999999999999;
                                    num.MinimumValue = -99999999999999;
                                    baseGrid.Sheets[0].Cells[j, i].CellType = num;

                                    /* LeadingZero 를쓰면 소숫자리수가 표현되서 강제 코딩처리함 : 화면상 0값은 안보이게 처리함...   */
                                    if (Convert.ToDecimal(baseGrid.Sheets[0].Cells[j, i].Value) == 0)
                                    {
                                        baseGrid.Sheets[0].Cells[j, i].Value = null;
                                    }

                                    break;
                                case "HL":	// HyperLink 셀 설정
                                    baseGrid.Sheets[0].Cells[j, i].CellType = new HyperLinkCellType();
                                    break;
                                case "PG":	// Progress 셀 설정
                                    baseGrid.Sheets[0].Cells[j, i].CellType = new ProgressCellType();
                                    break;
                                case "RT":	// RichText 셀 설정
                                    baseGrid.Sheets[0].Cells[j, i].CellType = new RichTextCellType();
                                    break;
                                case "SC":	// SliderCell 셀 설정
                                    baseGrid.Sheets[0].Cells[j, i].CellType = new SliderCellType();
                                    break;
                                case "NL":	// 필수입력 사항 체크하지 않는 로직을 탄다
                                    baseGrid.Sheets[0].Cells[j, i].CellType = new TextCellType();
                                    baseGrid.Sheets[0].Cells[j, i].CanFocus = false;
                                    if (shtCellType[i].ToString() == "NLV") // NLV인 경우 숨김
                                        baseGrid.ActiveSheet.Columns[i].Visible = false;
                                    break;
                                case "PC":	// Percent 셀 설정
                                    baseGrid.Sheets[0].Cells[j, i].CellType = new PercentCellType();
                                    break;
                                case "CB":	// ComboBox 셀 설정
                                    if (G1Etc[i].Length > 0)
                                    {
                                        ComboBoxCellType comboType = new ComboBoxCellType();
                                        comboType.MaxDrop = 20;

                                        Regex rx = new Regex("#");
                                        string Tmp = G1Etc[i].ToString();	//.Substring(G1Etc[k].IndexOf(":")+1, G1Etc[k].Length - G1Etc[k].IndexOf(":")-1);

                                        string Tmp1 = "";
                                        string Tmp2 = "";
                                        if (Tmp.IndexOf("|") > 0)
                                        {
                                            Tmp1 = Tmp.Substring(0, Tmp.IndexOf("|"));
                                            Tmp2 = Tmp.Substring(Tmp.IndexOf("|") + 1, Tmp.Length - Tmp.IndexOf("|") - 1);
                                            string[] cboMsg1 = rx.Split(Tmp1);
                                            string[] cboMsg2 = rx.Split(Tmp2);

                                            comboType.ItemData = cboMsg1;	// Key, Text
                                            comboType.Items = cboMsg2;		// Value
                                            comboType.EditorValue = FarPoint.Win.Spread.CellType.EditorValue.ItemData;

                                            baseGrid.Sheets[0].Cells[j, i].CellType = comboType;
                                        }
                                        else
                                        {
                                            string[] cboMsg1 = rx.Split(Tmp);

                                            comboType.Items = cboMsg1;
                                            baseGrid.Sheets[0].Cells[j, i].CellType = comboType;
                                        }
                                        if (shtCellType[i].ToString() == "CBV") // NLV인 경우 숨김
                                            baseGrid.ActiveSheet.Columns[i].Visible = false;

                                    }
                                    break;
                                case "ML": //TextBox MultiLine
                                    FarPoint.Win.Spread.CellType.TextCellType MultiType = new FarPoint.Win.Spread.CellType.TextCellType();
                                    MultiType.Multiline = true;

                                    baseGrid.Sheets[0].Columns[i].CellType = MultiType;
                                    break;
                                case "GN":	// General & 영문 대문자로 변환
                                    baseGrid.Sheets[0].Cells[j, i].CellType = new TextCellType();
                                    break;
                                default:	// General 설정
                                    baseGrid.Sheets[0].Cells[j, i].CellType = new TextCellType();
                                    FarPoint.Win.Spread.CellType.TextCellType textCellType1 = new FarPoint.Win.Spread.CellType.TextCellType();
                                    textCellType1.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
                                    baseGrid.Sheets[0].Columns.Get(i).CellType = textCellType1;
                                    break;
                            }
                        }
                    }
                    /****************************************************************************/

                    /********************** Column 정렬 설정 **************************************/
                    if (sheetAlign.Length > i)
                    {

                        switch (sheetAlign[i])
                        {
                            case "L":	// 왼쪽 정렬 MIDDLE
                                baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
                                baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
                                break;
                            case "LT":	// 왼쪽 정렬 TOP
                                baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
                                baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                                break;
                            case "LB":	// 왼쪽 정렬 BOTTOM
                                baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
                                baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Bottom;
                                break;
                            case "C": // 가운데 정렬
                                baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
                                baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
                                break;
                            case "CT":	// 가운데 정렬 TOP
                                baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
                                baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                                break;
                            case "CB":	// 가운데 정렬 BOTTOM
                                baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
                                baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Bottom;
                                break;
                            case "R":	// 오른쪽 정렬
                                baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;
                                baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
                                break;
                            case "RT":	// 오른쪽 정렬 TOP
                                baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;
                                baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                                break;
                            case "RB":	// 오른쪽 정렬 BOTTOM
                                baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;
                                baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Bottom;
                                break;
                            default:	// 디폴트 왼쪽 정렬
                                baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
                                baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
                                break;
                        }

                        for (int j = 0; j < baseGrid.Sheets[0].Rows.Count; j++)
                        {
                            switch (sheetAlign[i])
                            {
                                case "L":	// 왼쪽 정렬 MIDDLE
                                    baseGrid.Sheets[0].Cells[j, i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
                                    baseGrid.Sheets[0].Cells[j, i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
                                    break;
                                case "LT":	// 왼쪽 정렬 TOP
                                    baseGrid.Sheets[0].Cells[j, i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
                                    baseGrid.Sheets[0].Cells[j, i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                                    break;
                                case "LB":	// 왼쪽 정렬 BOTTOM
                                    baseGrid.Sheets[0].Cells[j, i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
                                    baseGrid.Sheets[0].Cells[j, i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Bottom;
                                    break;
                                case "C": // 가운데 정렬
                                    baseGrid.Sheets[0].Cells[j, i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
                                    baseGrid.Sheets[0].Cells[j, i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
                                    break;
                                case "CT":	// 가운데 정렬 TOP
                                    baseGrid.Sheets[0].Cells[j, i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
                                    baseGrid.Sheets[0].Cells[j, i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                                    break;
                                case "CB":	// 가운데 정렬 BOTTOM
                                    baseGrid.Sheets[0].Cells[j, i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
                                    baseGrid.Sheets[0].Cells[j, i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Bottom;
                                    break;
                                case "R":	// 오른쪽 정렬
                                    baseGrid.Sheets[0].Cells[j, i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;
                                    baseGrid.Sheets[0].Cells[j, i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
                                    break;
                                case "RT":	// 오른쪽 정렬 TOP
                                    baseGrid.Sheets[0].Cells[j, i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;
                                    baseGrid.Sheets[0].Cells[j, i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                                    break;
                                case "RB":	// 오른쪽 정렬 BOTTOM
                                    baseGrid.Sheets[0].Cells[j, i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;
                                    baseGrid.Sheets[0].Cells[j, i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Bottom;
                                    break;
                                default:	// 디폴트 왼쪽 정렬
                                    baseGrid.Sheets[0].Cells[j, i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
                                    baseGrid.Sheets[0].Cells[j, i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
                                    break;
                            }
                        }
                    }
                    /****************************************************************************/

                    /********************** Column 색상 및 Lock 설정 **************************************/
                    if (shtCellColor.Length > i)
                    {

                        switch (shtCellColor[i])
                        {
                            case 0:	// 일반
                                baseGrid.Sheets[0].Columns[i].BackColor = Color.White;
                                baseGrid.Sheets[0].Columns[i].Locked = false;
                                baseGrid.Sheets[0].Columns[i].CanFocus = true;
                                break;
                            case 1:	// 필수입력
                                baseGrid.Sheets[0].Columns[i].BackColor = SystemBase.Validation.Kind_LightCyan; //System.Drawing.Color.FromArgb(242, 252, 254);
                                baseGrid.Sheets[0].Columns[i].Locked = false;
                                baseGrid.Sheets[0].Columns[i].CanFocus = true;
                                break;
                            case 2:	// 읽기전용, 필수
                                baseGrid.Sheets[0].Columns[i].BackColor = System.Drawing.Color.FromArgb(239, 239, 239);
                                baseGrid.Sheets[0].Columns[i].Locked = true;
                                baseGrid.Sheets[0].Columns[i].CanFocus = true;
                                break;
                            case 3:	// 읽기전용, 필수항목에서 제외
                                baseGrid.Sheets[0].Columns[i].BackColor = System.Drawing.Color.FromArgb(238, 238, 238);
                                baseGrid.Sheets[0].Columns[i].Locked = true;
                                baseGrid.Sheets[0].Columns[i].CanFocus = true;
                                break;
                            case 4:	// 읽기전용, 팝업전용
                                baseGrid.Sheets[0].Columns[i].BackColor = Color.White;
                                baseGrid.Sheets[0].Columns[i].Locked = true;
                                baseGrid.Sheets[0].Columns[i].CanFocus = true;
                                break;
                            case 5:	// 읽기전용, 필수항목에서 제외, Focus 제외
                                baseGrid.Sheets[0].Columns[i].BackColor = System.Drawing.Color.FromArgb(238, 238, 238);
                                baseGrid.Sheets[0].Columns[i].Locked = true;
                                baseGrid.Sheets[0].Columns[i].CanFocus = false;
                                break;
                            case 6:	// 읽기전용, 필수항목, Focus 제외
                                baseGrid.Sheets[0].Columns[i].BackColor = System.Drawing.Color.FromArgb(239, 239, 239);
                                baseGrid.Sheets[0].Columns[i].Locked = true;
                                baseGrid.Sheets[0].Columns[i].CanFocus = false;
                                break;
                            default:	// 일반
                                baseGrid.Sheets[0].Columns[i].BackColor = Color.White;
                                baseGrid.Sheets[0].Columns[i].Locked = false;
                                baseGrid.Sheets[0].Columns[i].CanFocus = true;
                                break;
                        }

                        for (int j = 0; j < baseGrid.Sheets[0].Rows.Count; j++)
                        {
                            switch (shtCellColor[i])
                            {
                                case 0:	// 일반
                                    baseGrid.Sheets[0].Cells[j, i].BackColor = Color.White;
                                    baseGrid.Sheets[0].Cells[j, i].Locked = false;
                                    baseGrid.Sheets[0].Cells[j, i].CanFocus = true;
                                    break;
                                case 1:	// 필수입력
                                    baseGrid.Sheets[0].Cells[j, i].BackColor = SystemBase.Validation.Kind_LightCyan; //System.Drawing.Color.FromArgb(242, 252, 254);
                                    baseGrid.Sheets[0].Cells[j, i].Locked = false;
                                    baseGrid.Sheets[0].Cells[j, i].CanFocus = true;
                                    break;
                                case 2:	// 읽기전용, 필수
                                    baseGrid.Sheets[0].Cells[j, i].BackColor = System.Drawing.Color.FromArgb(239, 239, 239);
                                    baseGrid.Sheets[0].Cells[j, i].Locked = true;
                                    baseGrid.Sheets[0].Cells[j, i].CanFocus = true;
                                    break;
                                case 3:	// 읽기전용, 필수항목에서 제외
                                    baseGrid.Sheets[0].Cells[j, i].BackColor = System.Drawing.Color.FromArgb(238, 238, 238);
                                    baseGrid.Sheets[0].Cells[j, i].Locked = true;
                                    baseGrid.Sheets[0].Cells[j, i].CanFocus = true;
                                    break;
                                case 4:	// 읽기전용, 팝업전용
                                    baseGrid.Sheets[0].Cells[j, i].BackColor = Color.White;
                                    baseGrid.Sheets[0].Cells[j, i].Locked = true;
                                    baseGrid.Sheets[0].Cells[j, i].CanFocus = true;
                                    break;
                                case 5:	// 읽기전용, 필수항목에서 제외, Focus 제외
                                    baseGrid.Sheets[0].Cells[j, i].BackColor = System.Drawing.Color.FromArgb(238, 238, 238);
                                    baseGrid.Sheets[0].Cells[j, i].Locked = true;
                                    baseGrid.Sheets[0].Cells[j, i].CanFocus = false;
                                    break;
                                case 6:	// 읽기전용, 필수항목, Focus 제외
                                    baseGrid.Sheets[0].Cells[j, i].BackColor = System.Drawing.Color.FromArgb(239, 239, 239);
                                    baseGrid.Sheets[0].Cells[j, i].Locked = true;
                                    baseGrid.Sheets[0].Cells[j, i].CanFocus = false;
                                    break;
                                default:	// 일반
                                    baseGrid.Sheets[0].Cells[j, i].BackColor = Color.White;
                                    baseGrid.Sheets[0].Cells[j, i].Locked = false;
                                    baseGrid.Sheets[0].Cells[j, i].CanFocus = true;
                                    break;
                            }
                        }
                    }
                    /********************** Column Type 설정 **************************************/

                    /********************** Summery 하단 고정합계 **************************************/
                    if (sheetTitle.Length > 0 && shtSummary == true && baseGrid.Sheets[0].Rows.Count > 0)
                    {
                        if (i == 1)
                        {
                            baseGrid.Sheets[0].FrozenTrailingRowCount = 1;	//하단 Column 1줄 고정

                            baseGrid.Sheets[0].RowHeader.Cells[baseGrid.Sheets[0].Rows.Count - 1, 0].Text = "합계";
                            baseGrid.Sheets[0].Rows[baseGrid.Sheets[0].Rows.Count - 1].BackColor = System.Drawing.Color.FromArgb(242, 244, 246); // .FromName("Beige");
                            baseGrid.Sheets[0].Rows[baseGrid.Sheets[0].Rows.Count - 1].Locked = true;
                        }
                        FarPoint.Win.ComplexBorder complexBorder1 = new FarPoint.Win.ComplexBorder(new FarPoint.Win.ComplexBorderSide(FarPoint.Win.ComplexBorderSideStyle.None), new FarPoint.Win.ComplexBorderSide(FarPoint.Win.ComplexBorderSideStyle.ThinLine, System.Drawing.Color.FromArgb(((System.Byte)(100)), ((System.Byte)(100)), ((System.Byte)(100)))), new FarPoint.Win.ComplexBorderSide(FarPoint.Win.ComplexBorderSideStyle.None), new FarPoint.Win.ComplexBorderSide(FarPoint.Win.ComplexBorderSideStyle.None));
                        baseGrid.Sheets[0].Cells.Get(baseGrid.Sheets[0].Rows.Count - 1, i).Border = complexBorder1;

                        if (baseGrid.Sheets[0].GetCellType(baseGrid.Sheets[0].Rows.Count - 1, i).ToString() == "NumberCellType" ||
                            baseGrid.Sheets[0].GetCellType(baseGrid.Sheets[0].Rows.Count - 1, i).ToString() == "PercentCellType")
                        {
                            string Str = IntToString(i);
                            string Area = Str + "1:" + Str + Convert.ToString(baseGrid.Sheets[0].Rows.Count - 1);
                            Cell r = baseGrid.ActiveSheet.Cells[baseGrid.Sheets[0].Rows.Count - 1, i];

                            r.Formula = "SUM(" + Area + ")";
                        }
                        else
                        {
                            baseGrid.Sheets[0].Cells[baseGrid.Sheets[0].Rows.Count - 1, i].CellType = new TextCellType();
                        }
                    }
                    /****************************************************************************/
                }
                if (dt != null && dt.Rows.Count == 0)
                    MessageBox.Show(SystemBase.Base.MessageRtn("SY014"), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log("grdCommSheet (그리드공통 조회실패 )", f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY013"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                baseGrid.Sheets[0].Rows.Count = 0;
            }

        }
        #endregion

        #region grdCommSheet(15) - 그리드 데이타 조회 후 Frozen Row 또는 Colunm 정의 20070501 QUERY용
        public static void grdCommSheet(FarPoint.Win.Spread.FpSpread baseGrid
            , string Query
            , string[] sheetTitle
            , string[] sheetTitle2
            , string[] sheetTitle3
            , int[] sheetWidth
            , string[] sheetAlign
            , string[] shtCellType
            , int[] shtCellColor
            , string[] G1Etc
            , int shtHeaderRowCount
            , int shtFixR
            , int shtFixC
            , bool shtSummary
            , bool shtMessage
            )
        {
            try
            {
                /********************** 그리드 초기화 **************************************/
                baseGrid.Reset();
                //********************** Clipboard False***********************************/
                baseGrid.AutoClipboard = false;
                /********************** 폰트 정의 **************************************/
                baseGrid.Font = new System.Drawing.Font("돋움", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));

               
                /********************** 그리드 스타일 정의 **************************************/
                baseGrid.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
                /********************** 스크롤바 정의 **************************************/
                baseGrid.HorizontalScrollBarPolicy = FarPoint.Win.Spread.ScrollBarPolicy.AsNeeded;
                /********************** 스크롤바 정의 **************************************/
                baseGrid.VerticalScrollBarPolicy = FarPoint.Win.Spread.ScrollBarPolicy.AsNeeded;
                /********************** 스크롤바 정의 **************************************/
                baseGrid.ScrollBarTrackPolicy = FarPoint.Win.Spread.ScrollBarTrackPolicy.Both;
                /********************** 그리드 칼럼수 지정 **************************************/
                baseGrid.ActiveSheet.Columns.Count = sheetTitle.Length;
                /********************** 그리드 Row수 지정 **************************************/
                //baseGrid.ActiveSheet.Rows.Count = 0;
                /********************** 그리드 Row색상 지정 **************************************/
                baseGrid.Sheets[0].AlternatingRows[0].BackColor = Color.FromName("WhiteSmoke");
                /********************** 그리드 RowHeader 넓이 지정 *********************************/
                baseGrid.Sheets[0].RowHeader.Columns[0].Width = 32;
                /********************** 그리드 바탕색상 지정 **************************************/
                baseGrid.Sheets[0].GrayAreaBackColor = Color.FromName("white");
                /********************** 1Row 선택 기준(단일, 1Row별, 1Cell별) **************************************/
                baseGrid.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.Normal;				//.ExtendedSelect;
                /********************** ColumnHeader 수 지정**************************************/
                baseGrid.Sheets[0].ColumnHeader.RowCount = shtHeaderRowCount;
                 
                /********************** HorizontalGridLine 가로 가이드라인 색상 지정**************************************/
                baseGrid.Sheets[0].HorizontalGridLine = new FarPoint.Win.Spread.GridLine(FarPoint.Win.Spread.GridLineType.Flat, Color.FromArgb(193, 201, 212));
                /********************** VerticalGridLine 세로 가이드라인 색상 지정**************************************/
                baseGrid.Sheets[0].VerticalGridLine = new FarPoint.Win.Spread.GridLine(FarPoint.Win.Spread.GridLineType.Flat, Color.FromArgb(193, 201, 212));
                
                /********************** 왼쪽 상단 코너 색상 지정**************************************/
                baseGrid.Sheets[0].SheetCornerStyle.BackColor = System.Drawing.Color.FromArgb(242, 244, 246); // .FromName("Beige");
                /********************** 첫번째 Head color지정**************************************/
                baseGrid.Sheets[0].ColumnHeader.Rows[0].BackColor = System.Drawing.Color.FromArgb(242,244,246) ; // .FromName("Beige");
                /********************** 두번째 Head color지정**************************************/
                if (baseGrid.Sheets[0].ColumnHeader.RowCount > 1)
                    baseGrid.Sheets[0].ColumnHeader.Rows[1].BackColor = System.Drawing.Color.FromArgb(242, 244, 246); // .FromName("Beige");
                /********************** 세번째 Head color지정**************************************/
                if (baseGrid.Sheets[0].ColumnHeader.RowCount > 2)
                    baseGrid.Sheets[0].ColumnHeader.Rows[2].BackColor = System.Drawing.Color.FromArgb(242, 244, 246); // .FromName("Beige");
                /********************** 첫번째 Colum color 지정**************************************/
                baseGrid.Sheets[0].RowHeader.Columns[0].BackColor = System.Drawing.Color.FromArgb(242, 244, 246); // .FromName("Beige");
                
                /********************** 1번째 Cell(프라이머리) 키 숨김 지정**************************************/
                baseGrid.ActiveSheet.Columns[0].Visible = false;
                /********************** 입력시 기존 데이타 삭제후 입력**************************************/
                baseGrid.EditModeReplace = true;
                /********************** Clipboard 복사시 Head값 미포함**************************************/
                baseGrid.ClipboardOptions = FarPoint.Win.Spread.ClipboardOptions.NoHeaders;
                //********************** 입력 RowMode********************************************************/
                //baseGrid.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.RowMode;
                //********************** 그리드 focus 칼라***************************************************/
                baseGrid.FocusRenderer = new FarPoint.Win.Spread.SolidFocusIndicatorRenderer(Color.CornflowerBlue, 2);
                //********************** 그리드 숫자 입력시 초기화 후 입력***********************************/
                baseGrid.EditModeReplace = true;

               

                if (Query != null)
                {
                    DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);

                    baseGrid.Sheets[0].DataSource = dt;

                    if (dt.Rows.Count > 0 && shtSummary == true)
                        baseGrid.ActiveSheet.Rows.Count = dt.Rows.Count + 1;//그리드Row수 지정
                    else
                    {
                        baseGrid.ActiveSheet.Rows.Count = dt.Rows.Count;//그리드Row수 지정
                    }

                    if (dt.Rows.Count > 999 && dt.Rows.Count < 10000)
                        baseGrid.Sheets[0].RowHeader.Columns[0].Width = 38;
                    else if (dt.Rows.Count > 9999)
                        baseGrid.Sheets[0].RowHeader.Columns[0].Width = 44;

                }
                else
                {
                    baseGrid.ActiveSheet.Rows.Count = 0;
                }
                /********************** 그리드 Head 높이 지정 **************************************/
                baseGrid.Sheets[0].ColumnHeader.Rows[0].Height = 28;
                if (shtHeaderRowCount > 1)
                {
                    /********************** 그리드 Head 높이 지정 **************************************/
                    baseGrid.Sheets[0].ColumnHeader.Rows[1].Height = 28;
                }
                if (shtHeaderRowCount > 2)
                {
                    /********************** 그리드 Head 높이 지정 **************************************/
                    baseGrid.Sheets[0].ColumnHeader.Rows[2].Height = 28;
                }

                //****************************************************************************************************************
                //: 고정 Column 또는 Row 설정
                //****************************************************************************************************************
                baseGrid.Sheets[0].FrozenColumnCount = shtFixC + 1;
                baseGrid.Sheets[0].FrozenRowCount = shtFixR;
                //****************************************************************************************************************
                //: 버튼 설정
                //****************************************************************************************************************
                FarPoint.Win.Spread.CellType.ButtonCellType btnType = new FarPoint.Win.Spread.CellType.ButtonCellType();
                btnType.Text = "...";

                //****************************************************************************************************************
                //: Key Enter시 Next Col로 Focus 이동
                //****************************************************************************************************************
                FarPoint.Win.Spread.InputMap im = baseGrid.GetInputMap(FarPoint.Win.Spread.InputMapMode.WhenFocused);
                im.Put(new FarPoint.Win.Spread.Keystroke(Keys.Enter, Keys.None), FarPoint.Win.Spread.SpreadActions.MoveToNextColumnWrap);


                int sheetLength = 0, shtTitleSpanTmp = 1, shtTitleSpanTmp2 = 1, shtTitleSpanTmp3 = 1;
                sheetLength = sheetTitle.Length;

                if (sheetTitle.Length < sheetTitle2.Length)
                    sheetLength = sheetTitle2.Length;

                //--------------------------------------------------------------------------------------------------------------------------

                for (int i = 0; i < sheetTitle.Length; i++)
                {
                    //****************************************************************************************************************
                    //: Column 폭 넓이 설정
                    //****************************************************************************************************************
                    if (sheetWidth.Length > i)
                        baseGrid.Sheets[0].Columns[i].Width = sheetWidth[i];

                    //****************************************************************************************************************
                    //: sheet 1번째 Header Text 입력
                    //****************************************************************************************************************
                    if (sheetTitle.Length > i)
                        baseGrid.Sheets[0].ColumnHeader.Cells[0, i].Text = sheetTitle[i].ToString();

                    //****************************************************************************************************************
                    //: sheet 2번째 Header Text 입력
                    //****************************************************************************************************************
                    if (sheetTitle2.Length > i && baseGrid.Sheets[0].ColumnHeader.RowCount > 1)
                        baseGrid.Sheets[0].ColumnHeader.Cells[1, i].Text = sheetTitle2[i].ToString();

                    //****************************************************************************************************************
                    //: sheet 3번째 Header Text 입력
                    //****************************************************************************************************************
                    if (sheetTitle3.Length > i && baseGrid.Sheets[0].ColumnHeader.RowCount > 2)
                        baseGrid.Sheets[0].ColumnHeader.Cells[2, i].Text = sheetTitle3[i].ToString();

                    //****************************************************************************************************************
                    //: columnSpan 설정
                    //****************************************************************************************************************
                    if (sheetTitle.Length > i)
                    {
                        if (shtHeaderRowCount > 0)
                        {
                            if (i > 1 && sheetTitle[i].ToString() == sheetTitle[i - 1].ToString())
                            {
                                shtTitleSpanTmp++;
                                baseGrid.Sheets[0].ColumnHeader.Cells[0, i + 1 - shtTitleSpanTmp].ColumnSpan = shtTitleSpanTmp;

                            }
                            else
                                shtTitleSpanTmp = 1;
                        }

                        if (shtHeaderRowCount > 1)
                        {
                            if (i > 1 && sheetTitle2[i].ToString() == sheetTitle2[i - 1].ToString())
                            {
                                shtTitleSpanTmp2++;
                                baseGrid.Sheets[0].ColumnHeader.Cells[1, i + 1 - shtTitleSpanTmp2].ColumnSpan = shtTitleSpanTmp2;
                            }
                            else
                                shtTitleSpanTmp2 = 1;
                        }

                        if (shtHeaderRowCount > 2)
                        {
                            if (i > 1 && sheetTitle3[i].ToString() == sheetTitle3[i - 1].ToString())
                            {
                                shtTitleSpanTmp3++;
                                baseGrid.Sheets[0].ColumnHeader.Cells[2, i + 1 - shtTitleSpanTmp3].ColumnSpan = shtTitleSpanTmp3;
                            }
                            else
                                shtTitleSpanTmp3 = 1;
                        }


                        if (sheetTitle2[i] != null)
                        {
                            if (sheetTitle[i].ToString() == sheetTitle2[i].ToString())
                            {
                                baseGrid.Sheets[0].ColumnHeader.Cells[0, i].RowSpan = 2;
                            }
                        }

                        if (sheetTitle2[i] != null && sheetTitle3[i] != null)
                        {
                            if (sheetTitle[i].ToString() == sheetTitle2[i].ToString() && sheetTitle2[i].ToString() == sheetTitle3[i].ToString())
                            {
                                baseGrid.Sheets[0].ColumnHeader.Cells[0, i].RowSpan = 3;
                            }
                            if (sheetTitle2[i].ToString() == sheetTitle3[i].ToString())
                            {
                                baseGrid.Sheets[0].ColumnHeader.Cells[1, i].RowSpan = 2;
                            }
                        }
                    }

                    /********************** Column Type 설정 **************************************/
                    if (shtCellType.Length > i)
                    {

                        string CellType = "";
                        if (shtCellType[i].Length > 2)
                            CellType = shtCellType[i].Substring(0, 2);
                        else
                            CellType = shtCellType[i];

                        switch (CellType)
                        {
                            case "BT":	// Button 셀 설정
                                baseGrid.Sheets[0].Columns[i].CellType = btnType;
                                break;
                            case "CK":	// CheckBox 셀 설정
                                baseGrid.Sheets[0].Columns[i].CellType = new CheckBoxCellType();
                                break;
                            case "CH":	// CheckBox Head에 추가
                                FarPoint.Win.Spread.CellType.CheckBoxCellType checkBoxCellType1 = new FarPoint.Win.Spread.CellType.CheckBoxCellType();
                                if (baseGrid.Sheets[0].ColumnHeader.RowCount > 2)
                                {
                                    checkBoxCellType1.Caption = sheetTitle3[i];
                                    baseGrid.Sheets[0].ColumnHeader.Cells.Get(2, i).CellType = checkBoxCellType1;
                                    baseGrid.Sheets[0].ColumnHeader.Cells.Get(2, i).Border = new LineBorder(ColorTranslator.FromHtml("#9eb6ce"), 1, true, true, true, true);
                                    baseGrid.Sheets[0].Columns[i].CellType = new CheckBoxCellType();
                                }
                                else if (baseGrid.Sheets[0].ColumnHeader.RowCount > 1)
                                {
                                    checkBoxCellType1.Caption = sheetTitle2[i];
                                    baseGrid.Sheets[0].ColumnHeader.Cells.Get(1, i).CellType = checkBoxCellType1;
                                    baseGrid.Sheets[0].ColumnHeader.Cells.Get(1, i).Border = new LineBorder(ColorTranslator.FromHtml("#9eb6ce"), 1, true, true, true, true);
                                    baseGrid.Sheets[0].Columns[i].CellType = new CheckBoxCellType();
                                }
                                else
                                {
                                    checkBoxCellType1.Caption = sheetTitle[i];
                                    baseGrid.Sheets[0].ColumnHeader.Cells.Get(0, i).CellType = checkBoxCellType1;
                                    baseGrid.Sheets[0].ColumnHeader.Cells.Get(0, i).Border = new LineBorder(ColorTranslator.FromHtml("#9eb6ce"), 1, true, true, true, true);
                                    baseGrid.Sheets[0].Columns[i].CellType = new CheckBoxCellType();
                                }
                                break;
                            case "CC":	// Currency 셀 설정
                                baseGrid.Sheets[0].Columns[i].CellType = new CurrencyCellType();
                                break;
                            case "DT":	// Date 셀 설정
                                baseGrid.Sheets[0].Columns[i].CellType = new DateTimeCellType();
                                break;
                            case "DM":	// Date 셀 설정(년월)                            
                                FarPoint.Win.Spread.CellType.DateTimeCellType UseDt = new FarPoint.Win.Spread.CellType.DateTimeCellType();
                                UseDt.DateTimeFormat = FarPoint.Win.Spread.CellType.DateTimeFormat.UserDefined;
                                UseDt.UserDefinedFormat = "yyyy/MM";
                                baseGrid.Sheets[0].Columns[i].CellType = UseDt;
                                break;
                            case "DY":	// Date 셀 설정(년)                            
                                FarPoint.Win.Spread.CellType.DateTimeCellType UseDy = new FarPoint.Win.Spread.CellType.DateTimeCellType();
                                UseDy.DateTimeFormat = FarPoint.Win.Spread.CellType.DateTimeFormat.UserDefined;
                                UseDy.UserDefinedFormat = "yyyy";
                                baseGrid.Sheets[0].Columns[i].CellType = UseDy;
                                break;
                            case "DD":	// Date 셀 설정(월)                            
                                FarPoint.Win.Spread.CellType.DateTimeCellType UseDd = new FarPoint.Win.Spread.CellType.DateTimeCellType();
                                UseDd.DateTimeFormat = FarPoint.Win.Spread.CellType.DateTimeFormat.UserDefined;
                                UseDd.UserDefinedFormat = "MM";
                                baseGrid.Sheets[0].Columns[i].CellType = UseDd;
                                break;
                            case "PW":	// Password 셀 설정
                                FarPoint.Win.Spread.CellType.TextCellType passType = new FarPoint.Win.Spread.CellType.TextCellType();
                                passType.PasswordChar = Convert.ToChar("*");
                                baseGrid.Sheets[0].Columns[i].CellType = passType;
                                break;
                            case "MK":	// MaskCellType 셀 설정 | Mask 사용예 : MK###-###-####
                                FarPoint.Win.Spread.CellType.MaskCellType picType = new FarPoint.Win.Spread.CellType.MaskCellType();
                                if (G1Etc[i].ToString().Length > 0)
                                    picType.Mask = G1Etc[i].ToString();	//shtCellType[i].Substring(2,shtCellType[i].Length-2).ToString();
                                picType.MaskChar = Convert.ToChar("_");
                                baseGrid.Sheets[0].Columns[i].CellType = picType;
                                break;
                            case "NM":	// Number 셀 설정
                                int Place = 0;
                                if (shtCellType[i].Length == 3)
                                    Place = Convert.ToInt32(shtCellType[i].Substring(2, 1).ToString());
                                FarPoint.Win.Spread.CellType.NumberCellType num = new FarPoint.Win.Spread.CellType.NumberCellType();
                                //num.LeadingZero = FarPoint.Win.Spread.CellType.LeadingZero.No;
                                num.DecimalSeparator = ".";
                                num.DecimalPlaces = Place;
                                num.FixedPoint = false;
                                num.Separator = ",";
                                num.ShowSeparator = true;
                                num.MaximumValue = 99999999999999;
                                num.MinimumValue = -99999999999999;
                                baseGrid.Sheets[0].Columns[i].CellType = num;
                                break;
                            case "HL":	// HyperLink 셀 설정
                                baseGrid.Sheets[0].Columns[i].CellType = new HyperLinkCellType();
                                break;
                            case "PG":	// Progress 셀 설정
                                baseGrid.Sheets[0].Columns[i].CellType = new ProgressCellType();
                                break;
                            case "RT":	// RichText 셀 설정
                                baseGrid.Sheets[0].Columns[i].CellType = new RichTextCellType();
                                break;
                            case "SC":	// SliderCell 셀 설정
                                baseGrid.Sheets[0].Columns[i].CellType = new SliderCellType();
                                break;
                            case "NL":	// 필수입력 사항 체크하지 않는 로직을 탄다
                                baseGrid.Sheets[0].Columns[i].CellType = new TextCellType();
                                baseGrid.Sheets[0].Columns[i].CanFocus = false;
                                if (shtCellType[i].ToString() == "NLV") // NLV인 경우 숨김
                                    baseGrid.ActiveSheet.Columns[i].Visible = false;
                                break;
                            case "PC":	// Percent 셀 설정
                                baseGrid.Sheets[0].Columns[i].CellType = new PercentCellType();
                                break;
                            case "CB":	// ComboBox 셀 설정
                                if (G1Etc[i] != null && G1Etc[i].Length > 0)
                                {
                                    ComboBoxCellType comboType = new ComboBoxCellType();
                                    comboType.MaxDrop = 20;

                                    Regex rx = new Regex("#");
                                    string Tmp = G1Etc[i].ToString();	//.Substring(G1Etc[k].IndexOf(":")+1, G1Etc[k].Length - G1Etc[k].IndexOf(":")-1);

                                    string Tmp1 = "";
                                    string Tmp2 = "";
                                    if (Tmp.IndexOf("|") > 0)
                                    {
                                        Tmp1 = Tmp.Substring(0, Tmp.IndexOf("|"));
                                        Tmp2 = Tmp.Substring(Tmp.IndexOf("|") + 1, Tmp.Length - Tmp.IndexOf("|") - 1);
                                        string[] cboMsg1 = rx.Split(Tmp1);
                                        string[] cboMsg2 = rx.Split(Tmp2);

                                        comboType.ItemData = cboMsg1;	// Key, Text
                                        comboType.Items = cboMsg2;		// Value
                                        comboType.EditorValue = FarPoint.Win.Spread.CellType.EditorValue.ItemData;

                                        comboType.Editable = true;
                                        baseGrid.Sheets[0].Columns[i].CellType = comboType;
                                    }
                                    else
                                    {
                                        string[] cboMsg1 = rx.Split(Tmp);

                                        comboType.Items = cboMsg1;
                                        baseGrid.Sheets[0].Columns[i].CellType = comboType;
                                    }
                                    if (shtCellType[i].ToString() == "CBV") // NLV인 경우 숨김
                                        baseGrid.ActiveSheet.Columns[i].Visible = false;

                                }
                                break;
                            case "ML": //TextBox MultiLine
                                FarPoint.Win.Spread.CellType.TextCellType MultiType = new FarPoint.Win.Spread.CellType.TextCellType();
                                MultiType.Multiline = true;

                                baseGrid.Sheets[0].Columns[i].CellType = MultiType;
                                break;
                            case "GN":	// General & 영문 대문자로 변환
                                baseGrid.Sheets[0].Columns[i].CellType = new TextCellType();
                                break;
                            default:	// General 설정
                                baseGrid.Sheets[0].Columns[i].CellType = new TextCellType();
                                FarPoint.Win.Spread.CellType.TextCellType textCellType1 = new FarPoint.Win.Spread.CellType.TextCellType();
                                textCellType1.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
                                baseGrid.Sheets[0].Columns.Get(i).CellType = textCellType1;
                                break;
                        }
                        for (int j = 0; j < baseGrid.Sheets[0].Rows.Count; j++)
                        {
                            switch (CellType)
                            {
                                case "BT":	// Button 셀 설정
                                    baseGrid.Sheets[0].Cells[j, i].CellType = btnType;
                                    break;
                                case "CK":	// CheckBox 셀 설정
                                    baseGrid.Sheets[0].Cells[j, i].CellType = new CheckBoxCellType();
                                    break;
                                case "CH":	// CheckBox Head에 추가
                                    FarPoint.Win.Spread.CellType.CheckBoxCellType checkBoxCellType1 = new FarPoint.Win.Spread.CellType.CheckBoxCellType();
                                    checkBoxCellType1.Caption = sheetTitle[i];
                                    baseGrid.Sheets[0].ColumnHeader.Cells.Get(0, i).CellType = checkBoxCellType1;
                                    baseGrid.Sheets[0].ColumnHeader.Cells.Get(0, i).Border = new LineBorder(ColorTranslator.FromHtml("#9eb6ce"), 1, true, true, true, true);
                                    baseGrid.Sheets[0].Columns[i].CellType = new CheckBoxCellType();
                                    break;
                                case "CC":	// Currency 셀 설정
                                    baseGrid.Sheets[0].Cells[j, i].CellType = new CurrencyCellType();
                                    break;
                                case "DT":	// Date 셀 설정
                                    baseGrid.Sheets[0].Cells[j, i].CellType = new DateTimeCellType();
                                    break;
                                case "DM":	// Date 셀 설정(년월)                            
                                    FarPoint.Win.Spread.CellType.DateTimeCellType UseDt = new FarPoint.Win.Spread.CellType.DateTimeCellType();
                                    UseDt.DateTimeFormat = FarPoint.Win.Spread.CellType.DateTimeFormat.UserDefined;
                                    UseDt.UserDefinedFormat = "yyyy/MM";
                                    baseGrid.Sheets[0].Cells[j, i].CellType = UseDt;
                                    break;
                                case "DY":	// Date 셀 설정(년)                            
                                    FarPoint.Win.Spread.CellType.DateTimeCellType UseDy = new FarPoint.Win.Spread.CellType.DateTimeCellType();
                                    UseDy.DateTimeFormat = FarPoint.Win.Spread.CellType.DateTimeFormat.UserDefined;
                                    UseDy.UserDefinedFormat = "yyyy";
                                    baseGrid.Sheets[0].Cells[j, i].CellType = UseDy;
                                    break;
                                case "DD":	// Date 셀 설정(월)                            
                                    FarPoint.Win.Spread.CellType.DateTimeCellType UseDd = new FarPoint.Win.Spread.CellType.DateTimeCellType();
                                    UseDd.DateTimeFormat = FarPoint.Win.Spread.CellType.DateTimeFormat.UserDefined;
                                    UseDd.UserDefinedFormat = "MM";
                                    baseGrid.Sheets[0].Cells[j, i].CellType = UseDd;
                                    break;
                                case "PW":	// Password 셀 설정
                                    FarPoint.Win.Spread.CellType.TextCellType passType = new FarPoint.Win.Spread.CellType.TextCellType();
                                    passType.PasswordChar = Convert.ToChar("*");
                                    baseGrid.Sheets[0].Cells[j, i].CellType = passType;
                                    break;
                                case "MK":	// MaskCellType 셀 설정 | Mask 사용예 : MK###-###-####
                                    FarPoint.Win.Spread.CellType.MaskCellType picType = new FarPoint.Win.Spread.CellType.MaskCellType();
                                    if (G1Etc[i].ToString().Length > 0)
                                        picType.Mask = G1Etc[i].ToString();	//shtCellType[i].Substring(2,shtCellType[i].Length-2).ToString();
                                    picType.MaskChar = Convert.ToChar("_");
                                    baseGrid.Sheets[0].Cells[j, i].CellType = picType;
                                    break;
                                case "NM":	// Number 셀 설정
                                    int Place = 0;
                                    if (shtCellType[i].Length == 3)
                                        Place = Convert.ToInt32(shtCellType[i].Substring(2, 1).ToString());
                                    FarPoint.Win.Spread.CellType.NumberCellType num = new FarPoint.Win.Spread.CellType.NumberCellType();
                                    //num.LeadingZero = FarPoint.Win.Spread.CellType.LeadingZero.No;
                                    num.DecimalSeparator = ".";
                                    num.DecimalPlaces = Place;
                                    num.FixedPoint = true;
                                    num.Separator = ",";
                                    num.ShowSeparator = true;
                                    num.MaximumValue = 99999999999999;
                                    num.MinimumValue = -99999999999999;
                                    baseGrid.Sheets[0].Cells[j, i].CellType = num;

                                    /* LeadingZero 를쓰면 소숫자리수가 표현되서 강제 코딩처리함 : 화면상 0값은 안보이게 처리함...   */
                                    if (Convert.ToDecimal(baseGrid.Sheets[0].Cells[j, i].Value) == 0)
                                    {
                                        baseGrid.Sheets[0].Cells[j, i].Value = null;
                                    }
                                    break;
                                case "HL":	// HyperLink 셀 설정
                                    baseGrid.Sheets[0].Cells[j, i].CellType = new HyperLinkCellType();
                                    break;
                                case "PG":	// Progress 셀 설정
                                    baseGrid.Sheets[0].Cells[j, i].CellType = new ProgressCellType();
                                    break;
                                case "RT":	// RichText 셀 설정
                                    baseGrid.Sheets[0].Cells[j, i].CellType = new RichTextCellType();
                                    break;
                                case "SC":	// SliderCell 셀 설정
                                    baseGrid.Sheets[0].Cells[j, i].CellType = new SliderCellType();
                                    break;
                                case "NL":	// 필수입력 사항 체크하지 않는 로직을 탄다
                                    baseGrid.Sheets[0].Cells[j, i].CellType = new TextCellType();
                                    baseGrid.Sheets[0].Cells[j, i].CanFocus = false;
                                    if (shtCellType[i].ToString() == "NLV") // NLV인 경우 숨김
                                        baseGrid.ActiveSheet.Columns[i].Visible = false;
                                    break;
                                case "PC":	// Percent 셀 설정
                                    baseGrid.Sheets[0].Cells[j, i].CellType = new PercentCellType();
                                    break;
                                case "CB":	// ComboBox 셀 설정
                                    if (G1Etc[i] != null && G1Etc[i].Length > 0)
                                    {
                                        ComboBoxCellType comboType = new ComboBoxCellType();
                                        comboType.MaxDrop = 20;

                                        Regex rx = new Regex("#");
                                        string Tmp = G1Etc[i].ToString();	//.Substring(G1Etc[k].IndexOf(":")+1, G1Etc[k].Length - G1Etc[k].IndexOf(":")-1);

                                        string Tmp1 = "";
                                        string Tmp2 = "";
                                        if (Tmp.IndexOf("|") > 0)
                                        {
                                            Tmp1 = Tmp.Substring(0, Tmp.IndexOf("|"));
                                            Tmp2 = Tmp.Substring(Tmp.IndexOf("|") + 1, Tmp.Length - Tmp.IndexOf("|") - 1);
                                            string[] cboMsg1 = rx.Split(Tmp1);
                                            string[] cboMsg2 = rx.Split(Tmp2);

                                            comboType.ItemData = cboMsg1;	// Key, Text
                                            comboType.Items = cboMsg2;		// Value
                                            comboType.EditorValue = FarPoint.Win.Spread.CellType.EditorValue.ItemData;

                                            comboType.Editable = true;
                                            baseGrid.Sheets[0].Cells[j, i].CellType = comboType;
                                        }
                                        else
                                        {
                                            string[] cboMsg1 = rx.Split(Tmp);

                                            comboType.Items = cboMsg1;
                                            baseGrid.Sheets[0].Cells[j, i].CellType = comboType;
                                        }
                                        if (shtCellType[i].ToString() == "CBV") // NLV인 경우 숨김
                                            baseGrid.ActiveSheet.Columns[i].Visible = false;
                                    }
                                    break;
                                case "ML": //TextBox MultiLine
                                    FarPoint.Win.Spread.CellType.TextCellType MultiType = new FarPoint.Win.Spread.CellType.TextCellType();
                                    MultiType.Multiline = true;

                                    baseGrid.Sheets[0].Columns[i].CellType = MultiType;
                                    break;
                                case "GN":	// General & 영문 대문자로 변환
                                    baseGrid.Sheets[0].Cells[j, i].CellType = new TextCellType();
                                    break;
                                default:	// General 설정
                                    baseGrid.Sheets[0].Cells[j, i].CellType = new TextCellType();
                                    FarPoint.Win.Spread.CellType.TextCellType textCellType1 = new FarPoint.Win.Spread.CellType.TextCellType();
                                    textCellType1.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
                                    baseGrid.Sheets[0].Columns.Get(i).CellType = textCellType1;
                                    break;
                            }
                        }
                    }
                    /****************************************************************************/

                    /********************** Column 정렬 설정 **************************************/
                    if (sheetAlign.Length > i)
                    {
                        switch (sheetAlign[i])
                        {
                            case "L":	// 왼쪽 정렬 MIDDLE
                                baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
                                baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
                                break;
                            case "LT":	// 왼쪽 정렬 TOP
                                baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
                                baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                                break;
                            case "LB":	// 왼쪽 정렬 BOTTOM
                                baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
                                baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Bottom;
                                break;
                            case "C": // 가운데 정렬
                                baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
                                baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
                                break;
                            case "CT":	// 가운데 정렬 TOP
                                baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
                                baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                                break;
                            case "CB":	// 가운데 정렬 BOTTOM
                                baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
                                baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Bottom;
                                break;
                            case "R":	// 오른쪽 정렬
                                baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;
                                baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
                                break;
                            case "RT":	// 오른쪽 정렬 TOP
                                baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;
                                baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                                break;
                            case "RB":	// 오른쪽 정렬 BOTTOM
                                baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;
                                baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Bottom;
                                break;
                            default:	// 디폴트 왼쪽 정렬
                                baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
                                baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
                                break;
                        }

                        for (int j = 0; j < baseGrid.Sheets[0].Rows.Count; j++)
                        {

                            switch (sheetAlign[i])
                            {
                                case "L":	// 왼쪽 정렬 MIDDLE
                                    baseGrid.Sheets[0].Cells[j, i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
                                    baseGrid.Sheets[0].Cells[j, i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
                                    break;
                                case "LT":	// 왼쪽 정렬 TOP
                                    baseGrid.Sheets[0].Cells[j, i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
                                    baseGrid.Sheets[0].Cells[j, i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                                    break;
                                case "LB":	// 왼쪽 정렬 BOTTOM
                                    baseGrid.Sheets[0].Cells[j, i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
                                    baseGrid.Sheets[0].Cells[j, i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Bottom;
                                    break;
                                case "C": // 가운데 정렬
                                    baseGrid.Sheets[0].Cells[j, i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
                                    baseGrid.Sheets[0].Cells[j, i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
                                    break;
                                case "CT":	// 가운데 정렬 TOP
                                    baseGrid.Sheets[0].Cells[j, i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
                                    baseGrid.Sheets[0].Cells[j, i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                                    break;
                                case "CB":	// 가운데 정렬 BOTTOM
                                    baseGrid.Sheets[0].Cells[j, i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
                                    baseGrid.Sheets[0].Cells[j, i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Bottom;
                                    break;
                                case "R":	// 오른쪽 정렬
                                    baseGrid.Sheets[0].Cells[j, i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;
                                    baseGrid.Sheets[0].Cells[j, i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
                                    break;
                                case "RT":	// 오른쪽 정렬 TOP
                                    baseGrid.Sheets[0].Cells[j, i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;
                                    baseGrid.Sheets[0].Cells[j, i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                                    break;
                                case "RB":	// 오른쪽 정렬 BOTTOM
                                    baseGrid.Sheets[0].Cells[j, i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;
                                    baseGrid.Sheets[0].Cells[j, i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Bottom;
                                    break;
                                default:	// 디폴트 왼쪽 정렬
                                    baseGrid.Sheets[0].Cells[j, i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
                                    baseGrid.Sheets[0].Cells[j, i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
                                    break;
                            }
                        }
                    }
                    /****************************************************************************/

                    /********************** Column 색상 및 Lock 설정 **************************************/
                    if (shtCellColor.Length > i)
                    {

                        switch (shtCellColor[i])
                        {
                            case 0:	// 일반
                                baseGrid.Sheets[0].Columns[i].BackColor = Color.White;
                                baseGrid.Sheets[0].Columns[i].Locked = false;
                                baseGrid.Sheets[0].Columns[i].CanFocus = true;
                                break;
                            case 1:	// 필수입력
                                baseGrid.Sheets[0].Columns[i].BackColor = SystemBase.Validation.Kind_LightCyan; //System.Drawing.Color.FromArgb(242, 252, 254);
                                baseGrid.Sheets[0].Columns[i].Locked = false;
                                baseGrid.Sheets[0].Columns[i].CanFocus = true;
                                break;
                            case 2:	// 읽기전용, 필수
                                baseGrid.Sheets[0].Columns[i].BackColor = System.Drawing.Color.FromArgb(239, 239, 239);
                                baseGrid.Sheets[0].Columns[i].Locked = true;
                                baseGrid.Sheets[0].Columns[i].CanFocus = true;
                                break;
                            case 3:	// 읽기전용, 필수항목에서 제외
                                baseGrid.Sheets[0].Columns[i].BackColor = System.Drawing.Color.FromArgb(238, 238, 238);
                                baseGrid.Sheets[0].Columns[i].Locked = true;
                                baseGrid.Sheets[0].Columns[i].CanFocus = true;
                                break;
                            case 4:	// 읽기전용, 팝업전용
                                baseGrid.Sheets[0].Columns[i].BackColor = Color.White;
                                baseGrid.Sheets[0].Columns[i].Locked = true;
                                baseGrid.Sheets[0].Columns[i].CanFocus = true;
                                break;
                            case 5:	// 읽기전용, 필수항목에서 제외, Focus 제외
                                baseGrid.Sheets[0].Columns[i].BackColor = System.Drawing.Color.FromArgb(238, 238, 238);
                                baseGrid.Sheets[0].Columns[i].Locked = true;
                                baseGrid.Sheets[0].Columns[i].CanFocus = false;
                                break;
                            case 6:	// 읽기전용, 필수항목, Focus 제외
                                baseGrid.Sheets[0].Columns[i].BackColor = System.Drawing.Color.FromArgb(239, 239, 239);
                                baseGrid.Sheets[0].Columns[i].Locked = true;
                                baseGrid.Sheets[0].Columns[i].CanFocus = false;
                                break;
                            default:	// 일반
                                baseGrid.Sheets[0].Columns[i].BackColor = Color.White;
                                baseGrid.Sheets[0].Columns[i].Locked = false;
                                baseGrid.Sheets[0].Columns[i].CanFocus = true;
                                break;
                        }

                        for (int j = 0; j < baseGrid.Sheets[0].Rows.Count; j++)
                        {
                            switch (shtCellColor[i])
                            {
                                case 0:	// 일반
                                    baseGrid.Sheets[0].Cells[j, i].BackColor = Color.White;
                                    baseGrid.Sheets[0].Cells[j, i].Locked = false;
                                    baseGrid.Sheets[0].Cells[j, i].CanFocus = true;
                                    break;
                                case 1:	// 필수입력
                                    baseGrid.Sheets[0].Cells[j, i].BackColor = SystemBase.Validation.Kind_LightCyan;  //System.Drawing.Color.FromArgb(242, 252, 254);
                                    baseGrid.Sheets[0].Cells[j, i].Locked = false;
                                    baseGrid.Sheets[0].Cells[j, i].CanFocus = true;
                                    break;
                                case 2:	// 읽기전용, 필수
                                    baseGrid.Sheets[0].Cells[j, i].BackColor = System.Drawing.Color.FromArgb(239, 239, 239);
                                    baseGrid.Sheets[0].Cells[j, i].Locked = true;
                                    baseGrid.Sheets[0].Cells[j, i].CanFocus = true;
                                    break;
                                case 3:	// 읽기전용, 필수항목에서 제외
                                    baseGrid.Sheets[0].Cells[j, i].BackColor = System.Drawing.Color.FromArgb(238, 238, 238);
                                    baseGrid.Sheets[0].Cells[j, i].Locked = true;
                                    baseGrid.Sheets[0].Cells[j, i].CanFocus = true;
                                    break;
                                case 4:	// 읽기전용, 팝업전용
                                    baseGrid.Sheets[0].Cells[j, i].BackColor = Color.White;
                                    baseGrid.Sheets[0].Cells[j, i].Locked = true;
                                    baseGrid.Sheets[0].Cells[j, i].CanFocus = true;
                                    break;
                                case 5:	// 읽기전용, 필수항목에서 제외, Focus 제외
                                    baseGrid.Sheets[0].Cells[j, i].BackColor = System.Drawing.Color.FromArgb(238, 238, 238);
                                    baseGrid.Sheets[0].Cells[j, i].Locked = true;
                                    baseGrid.Sheets[0].Cells[j, i].CanFocus = false;
                                    break;
                                case 6:	// 읽기전용, 필수항목, Focus 제외
                                    baseGrid.Sheets[0].Cells[j, i].BackColor = System.Drawing.Color.FromArgb(239, 239, 239);
                                    baseGrid.Sheets[0].Cells[j, i].Locked = true;
                                    baseGrid.Sheets[0].Cells[j, i].CanFocus = false;
                                    break;
                                default:	// 일반
                                    baseGrid.Sheets[0].Cells[j, i].BackColor = Color.White;
                                    baseGrid.Sheets[0].Cells[j, i].Locked = false;
                                    baseGrid.Sheets[0].Cells[j, i].CanFocus = true;
                                    break;
                            }
                        }
                    }
                    /********************** Column Type 설정 **************************************/

                    /********************** Summery 하단 고정합계 **************************************/
                    if (sheetTitle.Length > 0 && shtSummary == true && baseGrid.Sheets[0].Rows.Count > 0)
                    {
                        if (i == 1)
                        {
                            baseGrid.Sheets[0].FrozenTrailingRowCount = 1;	//하단 Column 1줄 고정

                            baseGrid.Sheets[0].RowHeader.Cells[baseGrid.Sheets[0].Rows.Count - 1, 0].Text = "합계";
                            baseGrid.Sheets[0].Rows[baseGrid.Sheets[0].Rows.Count - 1].BackColor = System.Drawing.Color.FromArgb(242, 244, 246); // .FromName("Beige");
                            baseGrid.Sheets[0].Rows[baseGrid.Sheets[0].Rows.Count - 1].Locked = true;
                        }
                        FarPoint.Win.ComplexBorder complexBorder1 = new FarPoint.Win.ComplexBorder(new FarPoint.Win.ComplexBorderSide(FarPoint.Win.ComplexBorderSideStyle.None), new FarPoint.Win.ComplexBorderSide(FarPoint.Win.ComplexBorderSideStyle.ThinLine, System.Drawing.Color.FromArgb(((System.Byte)(100)), ((System.Byte)(100)), ((System.Byte)(100)))), new FarPoint.Win.ComplexBorderSide(FarPoint.Win.ComplexBorderSideStyle.None), new FarPoint.Win.ComplexBorderSide(FarPoint.Win.ComplexBorderSideStyle.None));
                        baseGrid.Sheets[0].Cells.Get(baseGrid.Sheets[0].Rows.Count - 1, i).Border = complexBorder1;

                        if (baseGrid.Sheets[0].GetCellType(baseGrid.Sheets[0].Rows.Count - 1, i).ToString() == "NumberCellType" ||
                            baseGrid.Sheets[0].GetCellType(baseGrid.Sheets[0].Rows.Count - 1, i).ToString() == "PercentCellType")
                        {
                            string Str = IntToString(i);
                            string Area = Str + "1:" + Str + Convert.ToString(baseGrid.Sheets[0].Rows.Count - 1);
                            Cell r = baseGrid.ActiveSheet.Cells[baseGrid.Sheets[0].Rows.Count - 1, i];

                            r.Formula = "SUM(" + Area + ")";
                        }
                        else
                        {
                            baseGrid.Sheets[0].Cells[baseGrid.Sheets[0].Rows.Count - 1, i].CellType = new TextCellType();
                        }
                    }
                    /****************************************************************************/                    
                }
                if (shtMessage == true)
                {
                    if (Query != null && baseGrid.Sheets[0].Rows.Count == 0)
                        MessageBox.Show(SystemBase.Base.MessageRtn("SY014"), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                /* 블럭지정 */
                //baseGrid.Sheets[0].SelectionStyle = FarPoint.Win.Spread.SelectionStyles.SelectionColors;
                //baseGrid.Sheets[0].SelectionBackColor = System.Drawing.Color.FromArgb(232, 255, 204);    
                //baseGrid.Sheets[0].SelectionForeColor = System.Drawing.Color.FromArgb(100, 115, 137);
                                
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log("grdCommSheet (그리드공통 조회실패 )", Query.ToString() + "\n" + f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY013"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                baseGrid.Sheets[0].Rows.Count = 0;
            }
        }
        #endregion

        #region grdCommSheet(15) - 그리드 데이타 조회 후 Frozen Row 또는 Colunm 정의 20070501 DATATABLE용
        public static void grdCommSheet(
            DataTable dt
            , FarPoint.Win.Spread.FpSpread baseGrid
            , string[] sheetTitle
            , string[] sheetTitle2
            , string[] sheetTitle3
            , int[] sheetWidth
            , string[] sheetAlign
            , string[] shtCellType
            , int[] shtCellColor
            , string[] G1Etc
            , int shtHeaderRowCount
            , int shtFixR
            , int shtFixC
            , bool shtSummary
            , bool shtMessage
            )
        {
            try
            {
                /********************** 그리드 초기화 **************************************/
                baseGrid.Reset();
                //********************** Clipboard False***********************************/
                baseGrid.AutoClipboard = false;
                /********************** 폰트 정의 **************************************/
                baseGrid.Font = new System.Drawing.Font("돋움", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
                /********************** 그리드 스타일 정의 **************************************/
                baseGrid.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
                /********************** 스크롤바 정의 **************************************/
                baseGrid.HorizontalScrollBarPolicy = FarPoint.Win.Spread.ScrollBarPolicy.AsNeeded;
                /********************** 스크롤바 정의 **************************************/
                baseGrid.VerticalScrollBarPolicy = FarPoint.Win.Spread.ScrollBarPolicy.AsNeeded;
                /********************** 스크롤바 정의 **************************************/
                baseGrid.ScrollBarTrackPolicy = FarPoint.Win.Spread.ScrollBarTrackPolicy.Both;
                /********************** 그리드 칼럼수 지정 **************************************/
                baseGrid.ActiveSheet.Columns.Count = sheetTitle.Length;
                /********************** 그리드 Row수 지정 **************************************/
                //baseGrid.ActiveSheet.Rows.Count = 0;
                /********************** 그리드 Row색상 지정 **************************************/
                baseGrid.Sheets[0].AlternatingRows[0].BackColor = Color.FromName("WhiteSmoke");
                /********************** 그리드 RowHeader 넓이 지정 *********************************/
                if (dt != null && dt.Rows.Count > 999 && dt.Rows.Count < 10000)
                    baseGrid.Sheets[0].RowHeader.Columns[0].Width = 38;
                else if (dt != null && dt.Rows.Count > 9999)
                    baseGrid.Sheets[0].RowHeader.Columns[0].Width = 44;
                else
                    baseGrid.Sheets[0].RowHeader.Columns[0].Width = 32;

                /********************** 그리드 바탕색상 지정 **************************************/
                baseGrid.Sheets[0].GrayAreaBackColor = Color.FromName("white");
                /********************** 1Row 선택 기준(단일, 1Row별, 1Cell별) **************************************/
                baseGrid.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.Normal;				//.ExtendedSelect;
                /********************** ColumnHeader 수 지정**************************************/
                baseGrid.Sheets[0].ColumnHeader.RowCount = shtHeaderRowCount;
                /********************** HorizontalGridLine 가로 가이드라인 색상 지정**************************************/
                baseGrid.Sheets[0].HorizontalGridLine = new FarPoint.Win.Spread.GridLine(FarPoint.Win.Spread.GridLineType.Flat, Color.FromArgb(193, 201, 212));
                /********************** VerticalGridLine 세로 가이드라인 색상 지정**************************************/
                baseGrid.Sheets[0].VerticalGridLine = new FarPoint.Win.Spread.GridLine(FarPoint.Win.Spread.GridLineType.Flat, Color.FromArgb(193, 201, 212));
                /********************** 왼쪽 상단 코너 색상 지정**************************************/
                baseGrid.Sheets[0].SheetCornerStyle.BackColor = System.Drawing.Color.FromArgb(242, 244, 246); // .FromName("Beige");
                /********************** 첫번째 Head color지정**************************************/
                baseGrid.Sheets[0].ColumnHeader.Rows[0].BackColor = System.Drawing.Color.FromArgb(242, 244, 246); // .FromName("Beige");
                /********************** 두번째 Head color지정**************************************/
                if (baseGrid.Sheets[0].ColumnHeader.RowCount > 1)
                    baseGrid.Sheets[0].ColumnHeader.Rows[1].BackColor = System.Drawing.Color.FromArgb(242, 244, 246); // .FromName("Beige");
                /********************** 세번째 Head color지정**************************************/
                if (baseGrid.Sheets[0].ColumnHeader.RowCount > 2)
                    baseGrid.Sheets[0].ColumnHeader.Rows[2].BackColor = System.Drawing.Color.FromArgb(242, 244, 246); // .FromName("Beige");

                /********************** 첫번째 Colum color 지정**************************************/
                baseGrid.Sheets[0].RowHeader.Columns[0].BackColor = System.Drawing.Color.FromArgb(242, 244, 246); // .FromName("Beige");
                /********************** 1번째 Cell(프라이머리) 키 숨김 지정**************************************/
                baseGrid.ActiveSheet.Columns[0].Visible = false;
                /********************** 입력시 기존 데이타 삭제후 입력**************************************/
                baseGrid.EditModeReplace = true;
                /********************** Clipboard 복사시 Head값 미포함**************************************/
                baseGrid.ClipboardOptions = FarPoint.Win.Spread.ClipboardOptions.NoHeaders;

                ///********************** 입력 RowMode********************************************************/
                //baseGrid.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.RowMode;
                //********************** 그리드 focus 칼라***************************************************/
                baseGrid.FocusRenderer = new FarPoint.Win.Spread.SolidFocusIndicatorRenderer(Color.CornflowerBlue, 2);
                //********************** 그리드 숫자 입력시 초기화 후 입력***********************************/
                baseGrid.EditModeReplace = true;

                if (dt.Rows.Count > 0)
                {
                    //DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);
                    baseGrid.Sheets[0].DataSource = dt;
                    if (dt.Rows.Count > 0 && shtSummary == true)
                        baseGrid.ActiveSheet.Rows.Count = dt.Rows.Count + 1;//그리드Row수 지정
                    else
                        baseGrid.ActiveSheet.Rows.Count = dt.Rows.Count;//그리드Row수 지정
                }
                else
                {
                    baseGrid.ActiveSheet.Rows.Count = 0;
                    //MessageBox.Show(SystemBase.Base.MessageRtn("SY011"));
                }
                /********************** 그리드 Head 높이 지정 **************************************/
                baseGrid.Sheets[0].ColumnHeader.Rows[0].Height = 28;
                if (shtHeaderRowCount > 1)
                {
                    /********************** 그리드 Head 높이 지정 **************************************/
                    baseGrid.Sheets[0].ColumnHeader.Rows[1].Height = 28;
                }
                if (shtHeaderRowCount > 2)
                {
                    /********************** 그리드 Head 높이 지정 **************************************/
                    baseGrid.Sheets[0].ColumnHeader.Rows[2].Height = 28;
                }

                //****************************************************************************************************************
                //: 고정 Column 또는 Row 설정
                //****************************************************************************************************************
                baseGrid.Sheets[0].FrozenColumnCount = shtFixC + 1;
                baseGrid.Sheets[0].FrozenRowCount = shtFixR;
                //****************************************************************************************************************
                //: 버튼 설정
                //****************************************************************************************************************
                FarPoint.Win.Spread.CellType.ButtonCellType btnType = new FarPoint.Win.Spread.CellType.ButtonCellType();
                btnType.Text = "...";

                //****************************************************************************************************************
                //: Key Enter시 Next Col로 Focus 이동
                //****************************************************************************************************************
                FarPoint.Win.Spread.InputMap im = baseGrid.GetInputMap(FarPoint.Win.Spread.InputMapMode.WhenFocused);
                im.Put(new FarPoint.Win.Spread.Keystroke(Keys.Enter, Keys.None), FarPoint.Win.Spread.SpreadActions.MoveToNextColumnWrap);


                int sheetLength = 0, shtTitleSpanTmp = 1, shtTitleSpanTmp2 = 1, shtTitleSpanTmp3 = 1;
                sheetLength = sheetTitle.Length;

                if (sheetTitle.Length < sheetTitle2.Length)
                    sheetLength = sheetTitle2.Length;

                //--------------------------------------------------------------------------------------------------------------------------

                for (int i = 0; i < sheetTitle.Length; i++)
                {
                    //****************************************************************************************************************
                    //: Column 폭 넓이 설정
                    //****************************************************************************************************************
                    if (sheetWidth.Length > i)
                    {
                        baseGrid.Sheets[0].Columns[i].Width = sheetWidth[i];
                    }

                    //****************************************************************************************************************
                    //: sheet 1번째 Header Text 입력
                    //****************************************************************************************************************
                    if (sheetTitle.Length > i)
                        baseGrid.Sheets[0].ColumnHeader.Cells[0, i].Text = sheetTitle[i].ToString();

                    //****************************************************************************************************************
                    //: sheet 2번째 Header Text 입력
                    //****************************************************************************************************************
                    if (sheetTitle2.Length > i && baseGrid.Sheets[0].ColumnHeader.RowCount > 1)
                        baseGrid.Sheets[0].ColumnHeader.Cells[1, i].Text = sheetTitle2[i].ToString();

                    //****************************************************************************************************************
                    //: sheet 3번째 Header Text 입력
                    //****************************************************************************************************************
                    if (sheetTitle3.Length > i && baseGrid.Sheets[0].ColumnHeader.RowCount > 2)
                        baseGrid.Sheets[0].ColumnHeader.Cells[2, i].Text = sheetTitle3[i].ToString();

                    //****************************************************************************************************************
                    //: columnSpan 설정
                    //****************************************************************************************************************
                    if (sheetTitle.Length > i)
                    {
                        if (shtHeaderRowCount > 0)
                        {
                            if (i > 1 && sheetTitle[i].ToString() == sheetTitle[i - 1].ToString())
                            {
                                shtTitleSpanTmp++;
                                baseGrid.Sheets[0].ColumnHeader.Cells[0, i + 1 - shtTitleSpanTmp].ColumnSpan = shtTitleSpanTmp;

                            }
                            else
                                shtTitleSpanTmp = 1;
                        }

                        if (shtHeaderRowCount > 1)
                        {
                            if (i > 1 && sheetTitle2[i].ToString() == sheetTitle2[i - 1].ToString())
                            {
                                shtTitleSpanTmp2++;
                                baseGrid.Sheets[0].ColumnHeader.Cells[1, i + 1 - shtTitleSpanTmp2].ColumnSpan = shtTitleSpanTmp2;
                            }
                            else
                                shtTitleSpanTmp2 = 1;
                        }

                        if (shtHeaderRowCount > 2)
                        {
                            if (i > 1 && sheetTitle3[i].ToString() == sheetTitle3[i - 1].ToString())
                            {
                                shtTitleSpanTmp3++;
                                baseGrid.Sheets[0].ColumnHeader.Cells[2, i + 1 - shtTitleSpanTmp3].ColumnSpan = shtTitleSpanTmp3;
                            }
                            else
                                shtTitleSpanTmp3 = 1;
                        }


                        if (sheetTitle2[i] != null)
                        {
                            if (sheetTitle[i].ToString() == sheetTitle2[i].ToString())
                            {
                                baseGrid.Sheets[0].ColumnHeader.Cells[0, i].RowSpan = 2;
                            }
                        }

                        if (sheetTitle2[i] != null && sheetTitle3[i] != null)
                        {
                            if (sheetTitle[i].ToString() == sheetTitle2[i].ToString() && sheetTitle2[i].ToString() == sheetTitle3[i].ToString())
                            {
                                baseGrid.Sheets[0].ColumnHeader.Cells[0, i].RowSpan = 3;
                            }
                            if (sheetTitle2[i].ToString() == sheetTitle3[i].ToString())
                            {
                                baseGrid.Sheets[0].ColumnHeader.Cells[1, i].RowSpan = 2;
                            }
                        }
                    }


                    /********************** Column Type 설정 **************************************/
                    if (shtCellType.Length > i)
                    {
                        string CellType = "";
                        if (shtCellType[i].Length > 2)
                            CellType = shtCellType[i].Substring(0, 2);
                        else
                            CellType = shtCellType[i];

                        switch (CellType)
                        {
                            case "BT":	// Button 셀 설정
                                baseGrid.Sheets[0].Columns[i].CellType = btnType;
                                break;
                            case "CK":	// CheckBox 셀 설정
                                baseGrid.Sheets[0].Columns[i].CellType = new CheckBoxCellType();
                                break;
                            case "CH":	// CheckBox Head에 추가
                                FarPoint.Win.Spread.CellType.CheckBoxCellType checkBoxCellType1 = new FarPoint.Win.Spread.CellType.CheckBoxCellType();
                                if (baseGrid.Sheets[0].ColumnHeader.RowCount > 2)
                                {
                                    checkBoxCellType1.Caption = sheetTitle3[i];
                                    baseGrid.Sheets[0].ColumnHeader.Cells.Get(2, i).CellType = checkBoxCellType1;
                                    baseGrid.Sheets[0].ColumnHeader.Cells.Get(2, i).Border = new LineBorder(ColorTranslator.FromHtml("#9eb6ce"), 1, true, true, true, true);
                                    baseGrid.Sheets[0].Columns[i].CellType = new CheckBoxCellType();
                                }
                                else if (baseGrid.Sheets[0].ColumnHeader.RowCount > 1)
                                {
                                    checkBoxCellType1.Caption = sheetTitle2[i];
                                    baseGrid.Sheets[0].ColumnHeader.Cells.Get(1, i).CellType = checkBoxCellType1;
                                    baseGrid.Sheets[0].ColumnHeader.Cells.Get(1, i).Border = new LineBorder(ColorTranslator.FromHtml("#9eb6ce"), 1, true, true, true, true);
                                    baseGrid.Sheets[0].Columns[i].CellType = new CheckBoxCellType();
                                }
                                else
                                {
                                    checkBoxCellType1.Caption = sheetTitle[i];
                                    baseGrid.Sheets[0].ColumnHeader.Cells.Get(0, i).CellType = checkBoxCellType1;
                                    baseGrid.Sheets[0].ColumnHeader.Cells.Get(0, i).Border = new LineBorder(ColorTranslator.FromHtml("#9eb6ce"), 1, true, true, true, true);
                                    baseGrid.Sheets[0].Columns[i].CellType = new CheckBoxCellType();
                                }
                                break;
                            case "CC":	// Currency 셀 설정
                                baseGrid.Sheets[0].Columns[i].CellType = new CurrencyCellType();
                                break;
                            case "DT":	// Date 셀 설정
                                baseGrid.Sheets[0].Columns[i].CellType = new DateTimeCellType();
                                break;
                            case "DM":	// Date 셀 설정(년월)                            
                                FarPoint.Win.Spread.CellType.DateTimeCellType UseDt = new FarPoint.Win.Spread.CellType.DateTimeCellType();
                                UseDt.DateTimeFormat = FarPoint.Win.Spread.CellType.DateTimeFormat.UserDefined;
                                UseDt.UserDefinedFormat = "yyyy/MM";
                                baseGrid.Sheets[0].Columns[i].CellType = UseDt;
                                break;
                            case "DY":	// Date 셀 설정(년)                            
                                FarPoint.Win.Spread.CellType.DateTimeCellType UseDy = new FarPoint.Win.Spread.CellType.DateTimeCellType();
                                UseDy.DateTimeFormat = FarPoint.Win.Spread.CellType.DateTimeFormat.UserDefined;
                                UseDy.UserDefinedFormat = "yyyy";
                                baseGrid.Sheets[0].Columns[i].CellType = UseDy;
                                break;
                            case "DD":	// Date 셀 설정(월)                            
                                FarPoint.Win.Spread.CellType.DateTimeCellType UseDd = new FarPoint.Win.Spread.CellType.DateTimeCellType();
                                UseDd.DateTimeFormat = FarPoint.Win.Spread.CellType.DateTimeFormat.UserDefined;
                                UseDd.UserDefinedFormat = "MM";
                                baseGrid.Sheets[0].Columns[i].CellType = UseDd;
                                break;
                            case "PW":	// Password 셀 설정
                                FarPoint.Win.Spread.CellType.TextCellType passType = new FarPoint.Win.Spread.CellType.TextCellType();
                                passType.PasswordChar = Convert.ToChar("*");
                                baseGrid.Sheets[0].Columns[i].CellType = passType;
                                break;
                            case "MK":	// MaskCellType 셀 설정 | Mask 사용예 : MK###-###-####
                                FarPoint.Win.Spread.CellType.MaskCellType picType = new FarPoint.Win.Spread.CellType.MaskCellType();
                                if (G1Etc[i].ToString().Length > 0)
                                    picType.Mask = G1Etc[i].ToString();	//shtCellType[i].Substring(2,shtCellType[i].Length-2).ToString();
                                picType.MaskChar = Convert.ToChar("_");
                                baseGrid.Sheets[0].Columns[i].CellType = picType;
                                break;
                            case "NM":	// Number 셀 설정
                                int Place = 0;
                                if (shtCellType[i].Length == 3)
                                    Place = Convert.ToInt32(shtCellType[i].Substring(2, 1).ToString());
                                FarPoint.Win.Spread.CellType.NumberCellType num = new FarPoint.Win.Spread.CellType.NumberCellType();

                                //num.LeadingZero = FarPoint.Win.Spread.CellType.LeadingZero.No;

                                num.DecimalSeparator = ".";
                                num.DecimalPlaces = Place;
                                num.FixedPoint = true;
                                num.Separator = ",";
                                num.ShowSeparator = true;
                                num.MaximumValue = 99999999999999;
                                num.MinimumValue = -99999999999999;
                                baseGrid.Sheets[0].Columns[i].CellType = num;
                                break;
                            case "HL":	// HyperLink 셀 설정
                                baseGrid.Sheets[0].Columns[i].CellType = new HyperLinkCellType();
                                break;
                            case "PG":	// Progress 셀 설정
                                baseGrid.Sheets[0].Columns[i].CellType = new ProgressCellType();
                                break;
                            case "RT":	// RichText 셀 설정
                                baseGrid.Sheets[0].Columns[i].CellType = new RichTextCellType();
                                break;
                            case "SC":	// SliderCell 셀 설정
                                baseGrid.Sheets[0].Columns[i].CellType = new SliderCellType();
                                break;
                            case "NL":	// 필수입력 사항 체크하지 않는 로직을 탄다
                                baseGrid.Sheets[0].Columns[i].CellType = new TextCellType();
                                baseGrid.Sheets[0].Columns[i].CanFocus = false;
                                if (shtCellType[i].ToString() == "NLV") // NLV인 경우 숨김
                                    baseGrid.ActiveSheet.Columns[i].Visible = false;
                                break;
                            case "PC":	// Percent 셀 설정
                                baseGrid.Sheets[0].Columns[i].CellType = new PercentCellType();
                                break;
                            case "CB":	// ComboBox 셀 설정
                                if (G1Etc[i].Length > 0)
                                {
                                    ComboBoxCellType comboType = new ComboBoxCellType();
                                    comboType.MaxDrop = 20;

                                    Regex rx = new Regex("#");
                                    string Tmp = G1Etc[i].ToString();	//.Substring(G1Etc[k].IndexOf(":")+1, G1Etc[k].Length - G1Etc[k].IndexOf(":")-1);

                                    string Tmp1 = "";
                                    string Tmp2 = "";
                                    if (Tmp.IndexOf("|") > 0)
                                    {
                                        Tmp1 = Tmp.Substring(0, Tmp.IndexOf("|"));
                                        Tmp2 = Tmp.Substring(Tmp.IndexOf("|") + 1, Tmp.Length - Tmp.IndexOf("|") - 1);
                                        string[] cboMsg1 = rx.Split(Tmp1);
                                        string[] cboMsg2 = rx.Split(Tmp2);

                                        comboType.ItemData = cboMsg1;	// Key, Text
                                        comboType.Items = cboMsg2;		// Value
                                        comboType.EditorValue = FarPoint.Win.Spread.CellType.EditorValue.ItemData;

                                        comboType.Editable = true;
                                        baseGrid.Sheets[0].Columns[i].CellType = comboType;
                                    }
                                    else
                                    {
                                        string[] cboMsg1 = rx.Split(Tmp);

                                        comboType.Items = cboMsg1;
                                        baseGrid.Sheets[0].Columns[i].CellType = comboType;
                                    }
                                    if (shtCellType[i].ToString() == "CBV") // NLV인 경우 숨김
                                        baseGrid.ActiveSheet.Columns[i].Visible = false;

                                }
                                break;
                            case "ML": //TextBox MultiLine
                                FarPoint.Win.Spread.CellType.TextCellType MultiType = new FarPoint.Win.Spread.CellType.TextCellType();
                                MultiType.Multiline = true;

                                baseGrid.Sheets[0].Columns[i].CellType = MultiType;
                                break;
                            case "GN":	// General & 영문 대문자로 변환
                                baseGrid.Sheets[0].Columns[i].CellType = new TextCellType();
                                break;
                            default:	// General 설정
                                baseGrid.Sheets[0].Columns[i].CellType = new TextCellType();
                                FarPoint.Win.Spread.CellType.TextCellType textCellType1 = new FarPoint.Win.Spread.CellType.TextCellType();
                                textCellType1.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;                                    
                                baseGrid.Sheets[0].Columns.Get(i).CellType = textCellType1;
                                break;
                        }

                        for (int j = 0; j < baseGrid.Sheets[0].Rows.Count; j++)
                        {

                            switch (CellType)
                            {
                                case "BT":	// Button 셀 설정
                                    baseGrid.Sheets[0].Cells[j, i].CellType = btnType;
                                    break;
                                case "CK":	// CheckBox 셀 설정
                                    baseGrid.Sheets[0].Cells[j, i].CellType = new CheckBoxCellType();
                                    break;
                                case "CH":	// CheckBox Head에 추가
                                    FarPoint.Win.Spread.CellType.CheckBoxCellType checkBoxCellType1 = new FarPoint.Win.Spread.CellType.CheckBoxCellType();
                                    checkBoxCellType1.Caption = sheetTitle[i];
                                    baseGrid.Sheets[0].ColumnHeader.Cells.Get(0, i).CellType = checkBoxCellType1;
                                    baseGrid.Sheets[0].ColumnHeader.Cells.Get(0, i).Border = new LineBorder(ColorTranslator.FromHtml("#9eb6ce"), 1, true, true, true, true);
                                    baseGrid.Sheets[0].Columns[i].CellType = new CheckBoxCellType();
                                    break;
                                case "CC":	// Currency 셀 설정
                                    baseGrid.Sheets[0].Cells[j, i].CellType = new CurrencyCellType();
                                    break;
                                case "DT":	// Date 셀 설정
                                    baseGrid.Sheets[0].Cells[j, i].CellType = new DateTimeCellType();
                                    break;
                                case "DM":	// Date 셀 설정(년월)                            
                                    FarPoint.Win.Spread.CellType.DateTimeCellType UseDt = new FarPoint.Win.Spread.CellType.DateTimeCellType();
                                    UseDt.DateTimeFormat = FarPoint.Win.Spread.CellType.DateTimeFormat.UserDefined;
                                    UseDt.UserDefinedFormat = "yyyy/MM";
                                    baseGrid.Sheets[0].Cells[j, i].CellType = UseDt;
                                    break;
                                case "DY":	// Date 셀 설정(년)                            
                                    FarPoint.Win.Spread.CellType.DateTimeCellType UseDy = new FarPoint.Win.Spread.CellType.DateTimeCellType();
                                    UseDy.DateTimeFormat = FarPoint.Win.Spread.CellType.DateTimeFormat.UserDefined;
                                    UseDy.UserDefinedFormat = "yyyy";
                                    baseGrid.Sheets[0].Cells[j, i].CellType = UseDy;
                                    break;
                                case "DD":	// Date 셀 설정(월)                            
                                    FarPoint.Win.Spread.CellType.DateTimeCellType UseDd = new FarPoint.Win.Spread.CellType.DateTimeCellType();
                                    UseDd.DateTimeFormat = FarPoint.Win.Spread.CellType.DateTimeFormat.UserDefined;
                                    UseDd.UserDefinedFormat = "MM";
                                    baseGrid.Sheets[0].Cells[j, i].CellType = UseDd;
                                    break;
                                case "PW":	// Password 셀 설정
                                    FarPoint.Win.Spread.CellType.TextCellType passType = new FarPoint.Win.Spread.CellType.TextCellType();
                                    passType.PasswordChar = Convert.ToChar("*");
                                    baseGrid.Sheets[0].Cells[j, i].CellType = passType;
                                    break;
                                case "MK":	// MaskCellType 셀 설정 | Mask 사용예 : MK###-###-####
                                    FarPoint.Win.Spread.CellType.MaskCellType picType = new FarPoint.Win.Spread.CellType.MaskCellType();
                                    if (G1Etc[i].ToString().Length > 0)
                                        picType.Mask = G1Etc[i].ToString();	//shtCellType[i].Substring(2,shtCellType[i].Length-2).ToString();
                                    picType.MaskChar = Convert.ToChar("_");
                                    baseGrid.Sheets[0].Cells[j, i].CellType = picType;
                                    break;
                                case "NM":	// Number 셀 설정
                                    int Place = 0;
                                    if (shtCellType[i].Length == 3)
                                        Place = Convert.ToInt32(shtCellType[i].Substring(2, 1).ToString());
                                    FarPoint.Win.Spread.CellType.NumberCellType num = new FarPoint.Win.Spread.CellType.NumberCellType();
                                    //num.LeadingZero = FarPoint.Win.Spread.CellType.LeadingZero.No;
                                    num.DecimalSeparator = ".";
                                    num.DecimalPlaces = Place;
                                    num.FixedPoint = true;
                                    num.Separator = ",";
                                    num.ShowSeparator = true;
                                    num.MaximumValue = 99999999999999;
                                    num.MinimumValue = -99999999999999;
                                    baseGrid.Sheets[0].Cells[j, i].CellType = num;

                                    /* LeadingZero 를쓰면 소숫자리수가 표현되서 강제 코딩처리함 : 화면상 0값은 안보이게 처리함...   */
                                    //if (Convert.ToDecimal(baseGrid.Sheets[0].Cells[j, i].Value) == 0)
                                    //{
                                    //    baseGrid.Sheets[0].Cells[j, i].Value = null;
                                    //}
                                    break;
                                case "HL":	// HyperLink 셀 설정
                                    baseGrid.Sheets[0].Cells[j, i].CellType = new HyperLinkCellType();
                                    break;
                                case "PG":	// Progress 셀 설정
                                    baseGrid.Sheets[0].Cells[j, i].CellType = new ProgressCellType();
                                    break;
                                case "RT":	// RichText 셀 설정
                                    baseGrid.Sheets[0].Cells[j, i].CellType = new RichTextCellType();
                                    break;
                                case "SC":	// SliderCell 셀 설정
                                    baseGrid.Sheets[0].Cells[j, i].CellType = new SliderCellType();
                                    break;
                                case "NL":	// 필수입력 사항 체크하지 않는 로직을 탄다
                                    baseGrid.Sheets[0].Cells[j, i].CellType = new TextCellType();
                                    baseGrid.Sheets[0].Cells[j, i].CanFocus = false;
                                    if (shtCellType[i].ToString() == "NLV") // NLV인 경우 숨김
                                        baseGrid.ActiveSheet.Columns[i].Visible = false;
                                    break;
                                case "PC":	// Percent 셀 설정
                                    baseGrid.Sheets[0].Cells[j, i].CellType = new PercentCellType();
                                    break;
                                case "CB":	// ComboBox 셀 설정
                                    if (G1Etc[i].Length > 0)
                                    {
                                        ComboBoxCellType comboType = new ComboBoxCellType();
                                        comboType.MaxDrop = 20;

                                        Regex rx = new Regex("#");
                                        string Tmp = G1Etc[i].ToString();	//.Substring(G1Etc[k].IndexOf(":")+1, G1Etc[k].Length - G1Etc[k].IndexOf(":")-1);

                                        string Tmp1 = "";
                                        string Tmp2 = "";
                                        if (Tmp.IndexOf("|") > 0)
                                        {
                                            Tmp1 = Tmp.Substring(0, Tmp.IndexOf("|"));
                                            Tmp2 = Tmp.Substring(Tmp.IndexOf("|") + 1, Tmp.Length - Tmp.IndexOf("|") - 1);
                                            string[] cboMsg1 = rx.Split(Tmp1);
                                            string[] cboMsg2 = rx.Split(Tmp2);

                                            comboType.ItemData = cboMsg1;	// Key, Text
                                            comboType.Items = cboMsg2;		// Value
                                            comboType.EditorValue = FarPoint.Win.Spread.CellType.EditorValue.ItemData;

                                            comboType.Editable = true;
                                            baseGrid.Sheets[0].Cells[j, i].CellType = comboType;
                                        }
                                        else
                                        {
                                            string[] cboMsg1 = rx.Split(Tmp);

                                            comboType.Items = cboMsg1;
                                            baseGrid.Sheets[0].Cells[j, i].CellType = comboType;
                                        }
                                        if (shtCellType[i].ToString() == "CBV") // NLV인 경우 숨김
                                            baseGrid.ActiveSheet.Columns[i].Visible = false;

                                    }
                                    break;
                                case "ML": //TextBox MultiLine
                                    FarPoint.Win.Spread.CellType.TextCellType MultiType = new FarPoint.Win.Spread.CellType.TextCellType();
                                    MultiType.Multiline = true;

                                    baseGrid.Sheets[0].Columns[i].CellType = MultiType;
                                    break;
                                case "GN":	// General & 영문 대문자로 변환
                                    baseGrid.Sheets[0].Cells[j, i].CellType = new TextCellType();
                                    break;
                                default:	// General 설정
                                    baseGrid.Sheets[0].Cells[j, i].CellType = new TextCellType();
                                    FarPoint.Win.Spread.CellType.TextCellType textCellType1 = new FarPoint.Win.Spread.CellType.TextCellType();
                                    textCellType1.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
                                    baseGrid.Sheets[0].Columns.Get(i).CellType = textCellType1;
                                    break;
                            }
                        }
                    }
                    /****************************************************************************/

                    /********************** Column 정렬 설정 **************************************/
                    if (sheetAlign.Length > i)
                    {

                        switch (sheetAlign[i])
                        {
                            case "L":	// 왼쪽 정렬 MIDDLE
                                baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
                                baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
                                break;
                            case "LT":	// 왼쪽 정렬 TOP
                                baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
                                baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                                break;
                            case "LB":	// 왼쪽 정렬 BOTTOM
                                baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
                                baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Bottom;
                                break;
                            case "C": // 가운데 정렬
                                baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
                                baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
                                break;
                            case "CT":	// 가운데 정렬 TOP
                                baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
                                baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                                break;
                            case "CB":	// 가운데 정렬 BOTTOM
                                baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
                                baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Bottom;
                                break;
                            case "R":	// 오른쪽 정렬
                                baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;
                                baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
                                break;
                            case "RT":	// 오른쪽 정렬 TOP
                                baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;
                                baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                                break;
                            case "RB":	// 오른쪽 정렬 BOTTOM
                                baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;
                                baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Bottom;
                                break;
                            default:	// 디폴트 왼쪽 정렬
                                baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
                                baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
                                break;
                        }

                        for (int j = 0; j < baseGrid.Sheets[0].Rows.Count; j++)
                        {
                            switch (sheetAlign[i])
                            {
                                case "L":	// 왼쪽 정렬 MIDDLE
                                    baseGrid.Sheets[0].Cells[j, i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
                                    baseGrid.Sheets[0].Cells[j, i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
                                    break;
                                case "LT":	// 왼쪽 정렬 TOP
                                    baseGrid.Sheets[0].Cells[j, i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
                                    baseGrid.Sheets[0].Cells[j, i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                                    break;
                                case "LB":	// 왼쪽 정렬 BOTTOM
                                    baseGrid.Sheets[0].Cells[j, i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
                                    baseGrid.Sheets[0].Cells[j, i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Bottom;
                                    break;
                                case "C": // 가운데 정렬
                                    baseGrid.Sheets[0].Cells[j, i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
                                    baseGrid.Sheets[0].Cells[j, i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
                                    break;
                                case "CT":	// 가운데 정렬 TOP
                                    baseGrid.Sheets[0].Cells[j, i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
                                    baseGrid.Sheets[0].Cells[j, i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                                    break;
                                case "CB":	// 가운데 정렬 BOTTOM
                                    baseGrid.Sheets[0].Cells[j, i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
                                    baseGrid.Sheets[0].Cells[j, i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Bottom;
                                    break;
                                case "R":	// 오른쪽 정렬
                                    baseGrid.Sheets[0].Cells[j, i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;
                                    baseGrid.Sheets[0].Cells[j, i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
                                    break;
                                case "RT":	// 오른쪽 정렬 TOP
                                    baseGrid.Sheets[0].Cells[j, i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;
                                    baseGrid.Sheets[0].Cells[j, i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                                    break;
                                case "RB":	// 오른쪽 정렬 BOTTOM
                                    baseGrid.Sheets[0].Cells[j, i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;
                                    baseGrid.Sheets[0].Cells[j, i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Bottom;
                                    break;
                                default:	// 디폴트 왼쪽 정렬
                                    baseGrid.Sheets[0].Cells[j, i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
                                    baseGrid.Sheets[0].Cells[j, i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
                                    break;
                            }
                        }
                    }
                    /****************************************************************************/

                    /********************** Column 색상 및 Lock 설정 **************************************/
                    if (shtCellColor.Length > i)
                    {

                        switch (shtCellColor[i])
                        {
                            case 0:	// 일반
                                baseGrid.Sheets[0].Columns[i].BackColor = Color.White;
                                baseGrid.Sheets[0].Columns[i].Locked = false;
                                baseGrid.Sheets[0].Columns[i].CanFocus = true;
                                break;
                            case 1:	// 필수입력
                                baseGrid.Sheets[0].Columns[i].BackColor = SystemBase.Validation.Kind_LightCyan; //System.Drawing.Color.FromArgb(242, 252, 254);
                                baseGrid.Sheets[0].Columns[i].Locked = false;
                                baseGrid.Sheets[0].Columns[i].CanFocus = true;
                                break;
                            case 2:	// 읽기전용, 필수
                                baseGrid.Sheets[0].Columns[i].BackColor = System.Drawing.Color.FromArgb(239, 239, 239);
                                baseGrid.Sheets[0].Columns[i].Locked = true;
                                baseGrid.Sheets[0].Columns[i].CanFocus = true;
                                break;
                            case 3:	// 읽기전용, 필수항목에서 제외
                                baseGrid.Sheets[0].Columns[i].BackColor = System.Drawing.Color.FromArgb(238, 238, 238);
                                baseGrid.Sheets[0].Columns[i].Locked = true;
                                baseGrid.Sheets[0].Columns[i].CanFocus = true;
                                break;
                            case 4:	// 읽기전용, 팝업전용
                                baseGrid.Sheets[0].Columns[i].BackColor = Color.White;
                                baseGrid.Sheets[0].Columns[i].Locked = true;
                                baseGrid.Sheets[0].Columns[i].CanFocus = true;
                                break;
                            case 5:	// 읽기전용, 필수항목에서 제외, Focus 제외
                                baseGrid.Sheets[0].Columns[i].BackColor = System.Drawing.Color.FromArgb(238, 238, 238);
                                baseGrid.Sheets[0].Columns[i].Locked = true;
                                baseGrid.Sheets[0].Columns[i].CanFocus = false;
                                break;
                            case 6:	// 읽기전용, 필수항목, Focus 제외
                                baseGrid.Sheets[0].Columns[i].BackColor = System.Drawing.Color.FromArgb(239, 239, 239);
                                baseGrid.Sheets[0].Columns[i].Locked = true;
                                baseGrid.Sheets[0].Columns[i].CanFocus = false;
                                break;
                            default:	// 일반
                                baseGrid.Sheets[0].Columns[i].BackColor = Color.White;
                                baseGrid.Sheets[0].Columns[i].Locked = false;
                                baseGrid.Sheets[0].Columns[i].CanFocus = true;
                                break;
                        }

                        for (int j = 0; j < baseGrid.Sheets[0].Rows.Count; j++)
                        {
                            switch (shtCellColor[i])
                            {
                                case 0:	// 일반
                                    baseGrid.Sheets[0].Cells[j, i].BackColor = Color.White;
                                    baseGrid.Sheets[0].Cells[j, i].Locked = false;
                                    baseGrid.Sheets[0].Cells[j, i].CanFocus = true;
                                    break;
                                case 1:	// 필수입력
                                    baseGrid.Sheets[0].Cells[j, i].BackColor = SystemBase.Validation.Kind_LightCyan; //System.Drawing.Color.FromArgb(242, 252, 254);
                                    baseGrid.Sheets[0].Cells[j, i].Locked = false;
                                    baseGrid.Sheets[0].Cells[j, i].CanFocus = true;
                                    break;
                                case 2:	// 읽기전용, 필수
                                    baseGrid.Sheets[0].Cells[j, i].BackColor = System.Drawing.Color.FromArgb(239, 239, 239);
                                    baseGrid.Sheets[0].Cells[j, i].Locked = true;
                                    baseGrid.Sheets[0].Cells[j, i].CanFocus = true;
                                    break;
                                case 3:	// 읽기전용, 필수항목에서 제외
                                    baseGrid.Sheets[0].Cells[j, i].BackColor = System.Drawing.Color.FromArgb(238, 238, 238);
                                    baseGrid.Sheets[0].Cells[j, i].Locked = true;
                                    baseGrid.Sheets[0].Cells[j, i].CanFocus = true;
                                    break;
                                case 4:	// 읽기전용, 팝업전용
                                    baseGrid.Sheets[0].Cells[j, i].BackColor = Color.White;
                                    baseGrid.Sheets[0].Cells[j, i].Locked = true;
                                    baseGrid.Sheets[0].Cells[j, i].CanFocus = true;
                                    break;
                                case 5:	// 읽기전용, 필수항목에서 제외, Focus 제외
                                    baseGrid.Sheets[0].Cells[j, i].BackColor = System.Drawing.Color.FromArgb(238, 238, 238);
                                    baseGrid.Sheets[0].Cells[j, i].Locked = true;
                                    baseGrid.Sheets[0].Cells[j, i].CanFocus = false;
                                    break;
                                case 6:	// 읽기전용, 필수항목, Focus 제외
                                    baseGrid.Sheets[0].Cells[j, i].BackColor = System.Drawing.Color.FromArgb(239, 239, 239);
                                    baseGrid.Sheets[0].Cells[j, i].Locked = true;
                                    baseGrid.Sheets[0].Cells[j, i].CanFocus = false;
                                    break;
                                default:	// 일반
                                    baseGrid.Sheets[0].Cells[j, i].BackColor = Color.White;
                                    baseGrid.Sheets[0].Cells[j, i].Locked = false;
                                    baseGrid.Sheets[0].Cells[j, i].CanFocus = true;
                                    break;
                            }
                        }
                    }
                    /********************** Column Type 설정 **************************************/

                    /********************** Summery 하단 고정합계 **************************************/
                    if (sheetTitle.Length > 0 && shtSummary == true && baseGrid.Sheets[0].Rows.Count > 0)
                    {
                        if (i == 1)
                        {
                            baseGrid.Sheets[0].FrozenTrailingRowCount = 1;	//하단 Column 1줄 고정

                            baseGrid.Sheets[0].RowHeader.Cells[baseGrid.Sheets[0].Rows.Count - 1, 0].Text = "합계";
                            baseGrid.Sheets[0].Rows[baseGrid.Sheets[0].Rows.Count - 1].BackColor = System.Drawing.Color.FromArgb(242, 244, 246); // .FromName("Beige");
                            baseGrid.Sheets[0].Rows[baseGrid.Sheets[0].Rows.Count - 1].Locked = true;
                        }
                        FarPoint.Win.ComplexBorder complexBorder1 = new FarPoint.Win.ComplexBorder(new FarPoint.Win.ComplexBorderSide(FarPoint.Win.ComplexBorderSideStyle.None), new FarPoint.Win.ComplexBorderSide(FarPoint.Win.ComplexBorderSideStyle.ThinLine, System.Drawing.Color.FromArgb(((System.Byte)(100)), ((System.Byte)(100)), ((System.Byte)(100)))), new FarPoint.Win.ComplexBorderSide(FarPoint.Win.ComplexBorderSideStyle.None), new FarPoint.Win.ComplexBorderSide(FarPoint.Win.ComplexBorderSideStyle.None));
                        baseGrid.Sheets[0].Cells.Get(baseGrid.Sheets[0].Rows.Count - 1, i).Border = complexBorder1;

                        if (baseGrid.Sheets[0].GetCellType(baseGrid.Sheets[0].Rows.Count - 1, i).ToString() == "NumberCellType" ||
                            baseGrid.Sheets[0].GetCellType(baseGrid.Sheets[0].Rows.Count - 1, i).ToString() == "PercentCellType")
                        {
                            string Str = IntToString(i);
                            string Area = Str + "1:" + Str + Convert.ToString(baseGrid.Sheets[0].Rows.Count - 1);
                            Cell r = baseGrid.ActiveSheet.Cells[baseGrid.Sheets[0].Rows.Count - 1, i];

                            r.Formula = "SUM(" + Area + ")";
                        }
                        else
                        {
                            baseGrid.Sheets[0].Cells[baseGrid.Sheets[0].Rows.Count - 1, i].CellType = new TextCellType();
                        }
                    }
                    /****************************************************************************/
                }
                if (shtMessage == true)
                {
                    if (dt != null && dt.Rows.Count == 0)
                        MessageBox.Show(SystemBase.Base.MessageRtn("SY014"), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log("grdCommSheet (그리드공통 조회실패 )", f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY013"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                baseGrid.Sheets[0].Rows.Count = 0;
            }

        }
        #endregion

        #region 방산원가에서는  15, 16 만 사용

        #region grdCommSheet(15) - Row 단위 속정 정의 안함 (칼럼단위로만 정의함) - 속도문제 2008-08-28
        public static void grdCommSheet(FarPoint.Win.Spread.FpSpread baseGrid
            , string Query
            , string[] sheetTitle
            , string[] sheetTitle2
            , string[] sheetTitle3
            , int[] sheetWidth
            , string[] sheetAlign
            , string[] shtCellType
            , int[] shtCellColor
            , string[] G1Etc
            , int shtHeaderRowCount
            , bool shtSummary
            , bool shtMessage
            , int shtFixR
            , int shtFixC
            )
        {
            try
            {
                /********************** 그리드 초기화 **************************************/
                baseGrid.Reset();
                //********************** Clipboard False***********************************/
                baseGrid.AutoClipboard = false;
                 /********************** 폰트 정의 **************************************/
                baseGrid.Font = new System.Drawing.Font("돋움", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
                /********************** 그리드 스타일 정의 **************************************/
                baseGrid.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
                /********************** 스크롤바 정의 **************************************/
                baseGrid.HorizontalScrollBarPolicy = FarPoint.Win.Spread.ScrollBarPolicy.AsNeeded;
                /********************** 스크롤바 정의 **************************************/
                baseGrid.VerticalScrollBarPolicy = FarPoint.Win.Spread.ScrollBarPolicy.AsNeeded;
                /********************** 스크롤바 정의 **************************************/
                baseGrid.ScrollBarTrackPolicy = FarPoint.Win.Spread.ScrollBarTrackPolicy.Both;
                /********************** 그리드 칼럼수 지정 **************************************/
                baseGrid.ActiveSheet.Columns.Count = sheetTitle.Length;
                /********************** 그리드 Row수 지정 **************************************/
                //baseGrid.ActiveSheet.Rows.Count = 0;
                /********************** 그리드 Row색상 지정 **************************************/
                baseGrid.Sheets[0].AlternatingRows[0].BackColor = Color.FromName("WhiteSmoke");
                /********************** 그리드 RowHeader 넓이 지정 *********************************/
                baseGrid.Sheets[0].RowHeader.Columns[0].Width = 32;
                /********************** 그리드 바탕색상 지정 **************************************/
                baseGrid.Sheets[0].GrayAreaBackColor = Color.FromName("white");
                /********************** 1Row 선택 기준(단일, 1Row별, 1Cell별) **************************************/
                baseGrid.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.Normal;				//.ExtendedSelect;
                /********************** ColumnHeader 수 지정**************************************/
                baseGrid.Sheets[0].ColumnHeader.RowCount = shtHeaderRowCount;
                /********************** HorizontalGridLine 가로 가이드라인 색상 지정**************************************/
                baseGrid.Sheets[0].HorizontalGridLine = new FarPoint.Win.Spread.GridLine(FarPoint.Win.Spread.GridLineType.Flat, Color.FromArgb(193, 201, 212));
                /********************** VerticalGridLine 세로 가이드라인 색상 지정**************************************/
                baseGrid.Sheets[0].VerticalGridLine = new FarPoint.Win.Spread.GridLine(FarPoint.Win.Spread.GridLineType.Flat, Color.FromArgb(193, 201, 212));
                /********************** 왼쪽 상단 코너 색상 지정**************************************/
                baseGrid.Sheets[0].SheetCornerStyle.BackColor = System.Drawing.Color.FromArgb(242, 244, 246);
                /********************** 첫번째 Head color지정**************************************/
                baseGrid.Sheets[0].ColumnHeader.Rows[0].BackColor = System.Drawing.Color.FromArgb(242, 244, 246); // .FromName("Beige");
                /********************** 두번째 Head color지정**************************************/
                if (baseGrid.Sheets[0].ColumnHeader.RowCount > 1)
                    baseGrid.Sheets[0].ColumnHeader.Rows[1].BackColor = System.Drawing.Color.FromArgb(242, 244, 246); // .FromName("Beige");
                /********************** 세번째 Head color지정**************************************/
                if (baseGrid.Sheets[0].ColumnHeader.RowCount > 2)
                    baseGrid.Sheets[0].ColumnHeader.Rows[2].BackColor = System.Drawing.Color.FromArgb(242, 244, 246); // .FromName("Beige");
                /********************** 첫번째 Colum color 지정**************************************/
                baseGrid.Sheets[0].RowHeader.Columns[0].BackColor = System.Drawing.Color.FromArgb(242, 244, 246); // .FromName("Beige");
                /********************** 1번째 Cell(프라이머리) 키 숨김 지정**************************************/
                baseGrid.ActiveSheet.Columns[0].Visible = false;
                /********************** 입력시 기존 데이타 삭제후 입력**************************************/
                baseGrid.EditModeReplace = true;
                /********************** Clipboard 복사시 Head값 미포함**************************************/
                baseGrid.ClipboardOptions = FarPoint.Win.Spread.ClipboardOptions.NoHeaders;
                //********************** 입력 RowMode********************************************************/
                //baseGrid.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.RowMode;
                //********************** 그리드 focus 칼라***************************************************/
                baseGrid.FocusRenderer = new FarPoint.Win.Spread.SolidFocusIndicatorRenderer(Color.CornflowerBlue, 2);
                //********************** 그리드 숫자 입력시 초기화 후 입력***********************************/
                baseGrid.EditModeReplace = true;

                
                if (Query != null)
                {
                    DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);
                    baseGrid.Sheets[0].DataSource = dt;

                    if (dt.Rows.Count > 0 && shtSummary == true)
                        baseGrid.ActiveSheet.Rows.Count = dt.Rows.Count + 1;//그리드Row수 지정
                    else
                    {
                        baseGrid.ActiveSheet.Rows.Count = dt.Rows.Count;//그리드Row수 지정
                    }

                    if (dt.Rows.Count > 999 && dt.Rows.Count < 10000)
                        baseGrid.Sheets[0].RowHeader.Columns[0].Width = 38;
                    else if (dt.Rows.Count > 9999)
                        baseGrid.Sheets[0].RowHeader.Columns[0].Width = 44;
                }
                else
                {
                    baseGrid.ActiveSheet.Rows.Count = 0;
                }
                /********************** 그리드 Head 높이 지정 **************************************/
                baseGrid.Sheets[0].ColumnHeader.Rows[0].Height = 28;
                if (shtHeaderRowCount > 1)
                {
                    /********************** 그리드 Head 높이 지정 **************************************/
                    baseGrid.Sheets[0].ColumnHeader.Rows[1].Height = 28;
                }
                if (shtHeaderRowCount > 2)
                {
                    /********************** 그리드 Head 높이 지정 **************************************/
                    baseGrid.Sheets[0].ColumnHeader.Rows[2].Height = 28;
                }

                //****************************************************************************************************************
                //: 고정 Column 또는 Row 설정
                //****************************************************************************************************************
                baseGrid.Sheets[0].FrozenColumnCount = shtFixC + 1;
                baseGrid.Sheets[0].FrozenRowCount = shtFixR;
                //****************************************************************************************************************
                //: 버튼 설정
                //****************************************************************************************************************
                FarPoint.Win.Spread.CellType.ButtonCellType btnType = new FarPoint.Win.Spread.CellType.ButtonCellType();
                btnType.Text = "...";

                //****************************************************************************************************************
                //: Key Enter시 Next Col로 Focus 이동
                //****************************************************************************************************************
                FarPoint.Win.Spread.InputMap im = baseGrid.GetInputMap(FarPoint.Win.Spread.InputMapMode.WhenFocused);
                im.Put(new FarPoint.Win.Spread.Keystroke(Keys.Enter, Keys.None), FarPoint.Win.Spread.SpreadActions.MoveToNextColumnWrap);


                int sheetLength = 0, shtTitleSpanTmp = 1, shtTitleSpanTmp2 = 1, shtTitleSpanTmp3 = 1;
                sheetLength = sheetTitle.Length;

                if (sheetTitle.Length < sheetTitle2.Length)
                    sheetLength = sheetTitle2.Length;

                //--------------------------------------------------------------------------------------------------------------------------

                for (int i = 0; i < sheetTitle.Length; i++)
                {
                    //****************************************************************************************************************
                    //: Column 폭 넓이 설정
                    //****************************************************************************************************************
                    if (sheetWidth.Length > i)
                        baseGrid.Sheets[0].Columns[i].Width = sheetWidth[i];

                    //****************************************************************************************************************
                    //: sheet 1번째 Header Text 입력
                    //****************************************************************************************************************
                    if (sheetTitle.Length > i)
                        baseGrid.Sheets[0].ColumnHeader.Cells[0, i].Text = sheetTitle[i].ToString();

                    //****************************************************************************************************************
                    //: sheet 2번째 Header Text 입력
                    //****************************************************************************************************************
                    if (sheetTitle2.Length > i && baseGrid.Sheets[0].ColumnHeader.RowCount > 1)
                        baseGrid.Sheets[0].ColumnHeader.Cells[1, i].Text = sheetTitle2[i].ToString();

                    //****************************************************************************************************************
                    //: sheet 3번째 Header Text 입력
                    //****************************************************************************************************************
                    if (sheetTitle3.Length > i && baseGrid.Sheets[0].ColumnHeader.RowCount > 2)
                        baseGrid.Sheets[0].ColumnHeader.Cells[2, i].Text = sheetTitle3[i].ToString();

                    //****************************************************************************************************************
                    //: columnSpan 설정
                    //****************************************************************************************************************
                    if (sheetTitle.Length > i)
                    {
                        if (shtHeaderRowCount > 0)
                        {
                            if (i > 1 && sheetTitle[i].ToString() == sheetTitle[i - 1].ToString())
                            {
                                shtTitleSpanTmp++;
                                baseGrid.Sheets[0].ColumnHeader.Cells[0, i + 1 - shtTitleSpanTmp].ColumnSpan = shtTitleSpanTmp;

                            }
                            else
                                shtTitleSpanTmp = 1;
                        }

                        if (shtHeaderRowCount > 1)
                        {
                            if (i > 1 && sheetTitle2[i].ToString() == sheetTitle2[i - 1].ToString())
                            {
                                shtTitleSpanTmp2++;
                                baseGrid.Sheets[0].ColumnHeader.Cells[1, i + 1 - shtTitleSpanTmp2].ColumnSpan = shtTitleSpanTmp2;
                            }
                            else
                                shtTitleSpanTmp2 = 1;
                        }

                        if (shtHeaderRowCount > 2)
                        {
                            if (i > 1 && sheetTitle3[i].ToString() == sheetTitle3[i - 1].ToString())
                            {
                                shtTitleSpanTmp3++;
                                baseGrid.Sheets[0].ColumnHeader.Cells[2, i + 1 - shtTitleSpanTmp3].ColumnSpan = shtTitleSpanTmp3;
                            }
                            else
                                shtTitleSpanTmp3 = 1;
                        }


                        if (sheetTitle2[i] != null)
                        {
                            if (sheetTitle[i].ToString() == sheetTitle2[i].ToString())
                            {
                                baseGrid.Sheets[0].ColumnHeader.Cells[0, i].RowSpan = 2;
                            }
                        }

                        if (sheetTitle2[i] != null && sheetTitle3[i] != null)
                        {
                            if (sheetTitle[i].ToString() == sheetTitle2[i].ToString() && sheetTitle2[i].ToString() == sheetTitle3[i].ToString())
                            {
                                baseGrid.Sheets[0].ColumnHeader.Cells[0, i].RowSpan = 3;
                            }
                            if (sheetTitle2[i].ToString() == sheetTitle3[i].ToString())
                            {
                                baseGrid.Sheets[0].ColumnHeader.Cells[1, i].RowSpan = 2;
                            }
                        }
                    }

                    /********************** Column Type 설정 **************************************/
                    if (shtCellType.Length > i)
                    {
                        string CellType = "";
                        if (shtCellType[i].Length > 2)
                            CellType = shtCellType[i].Substring(0, 2);
                        else
                            CellType = shtCellType[i];

                        switch (CellType)
                        {
                            case "BT":	// Button 셀 설정
                                baseGrid.Sheets[0].Columns[i].CellType = btnType;
                              
                                break;
                            case "CK":	// CheckBox 셀 설정
                                baseGrid.Sheets[0].Columns[i].CellType = new CheckBoxCellType();
                                break;
                            case "CH":	// CheckBox Head에 추가
                                FarPoint.Win.Spread.CellType.CheckBoxCellType checkBoxCellType1 = new FarPoint.Win.Spread.CellType.CheckBoxCellType();
                                if (baseGrid.Sheets[0].ColumnHeader.RowCount > 2)
                                {
                                    checkBoxCellType1.Caption = sheetTitle3[i];
                                    baseGrid.Sheets[0].ColumnHeader.Cells.Get(2, i).CellType = checkBoxCellType1;
                                    baseGrid.Sheets[0].ColumnHeader.Cells.Get(2, i).Border = new LineBorder(ColorTranslator.FromHtml("#9eb6ce"), 1, true, true, true, true);
                                    baseGrid.Sheets[0].Columns[i].CellType = new CheckBoxCellType();
                                }
                                else if (baseGrid.Sheets[0].ColumnHeader.RowCount > 1)
                                {
                                    checkBoxCellType1.Caption = sheetTitle2[i];
                                    baseGrid.Sheets[0].ColumnHeader.Cells.Get(1, i).CellType = checkBoxCellType1;
                                    baseGrid.Sheets[0].ColumnHeader.Cells.Get(1, i).Border = new LineBorder(ColorTranslator.FromHtml("#9eb6ce"), 1, true, true, true, true);
                                    baseGrid.Sheets[0].Columns[i].CellType = new CheckBoxCellType();
                                }
                                else
                                {
                                    checkBoxCellType1.Caption = sheetTitle[i];
                                    baseGrid.Sheets[0].ColumnHeader.Cells.Get(0, i).CellType = checkBoxCellType1;
                                    baseGrid.Sheets[0].ColumnHeader.Cells.Get(0, i).Border = new LineBorder(ColorTranslator.FromHtml("#9eb6ce"), 1, true, true, true, true);
                                    baseGrid.Sheets[0].Columns[i].CellType = new CheckBoxCellType();
                                }
                                break;
                            case "CC":	// Currency 셀 설정
                                baseGrid.Sheets[0].Columns[i].CellType = new CurrencyCellType();
                                break;
                            case "DT":	// Date 셀 설정
                                baseGrid.Sheets[0].Columns[i].CellType = new DateTimeCellType();
                                break;
                            case "DY":	// Mask 셀 설정(년월)                            
                                FarPoint.Win.Spread.CellType.MaskCellType dm = new FarPoint.Win.Spread.CellType.MaskCellType();
                                dm.Mask = "####-##";  
                                baseGrid.Sheets[0].Columns[i].CellType = dm;
                                break;
                            case "DD":	// Date 셀 설정(월콤보)
                                FarPoint.Win.Spread.CellType.ComboBoxCellType cb = new FarPoint.Win.Spread.CellType.ComboBoxCellType();
                                ListBox list = new ListBox();
                                list.Items.AddRange(new Object[] {"01", "02", "03","04","05","06","07","08", "09","10","11","12"});
                                cb.ListControl = list;
                                cb.Items = (new String[] { "", "", "", "", "", "", "", "", "", "", "", ""});
                                baseGrid.ActiveSheet.Columns[i].CellType = cb;
                                break;
                            case "PW":	// Password 셀 설정
                                FarPoint.Win.Spread.CellType.TextCellType passType = new FarPoint.Win.Spread.CellType.TextCellType();
                                passType.PasswordChar = Convert.ToChar("*");
                                baseGrid.Sheets[0].Columns[i].CellType = passType;
                                break;
                            case "MK":	// MaskCellType 셀 설정 | Mask 사용예 : MK###-###-####
                                FarPoint.Win.Spread.CellType.MaskCellType picType = new FarPoint.Win.Spread.CellType.MaskCellType();
                                if (G1Etc[i].ToString().Length > 0)
                                    picType.Mask = G1Etc[i].ToString();	//shtCellType[i].Substring(2,shtCellType[i].Length-2).ToString();
                                picType.MaskChar = Convert.ToChar("_");
                                baseGrid.Sheets[0].Columns[i].CellType = picType;
                                break;
                            case "NM":	// Number 셀 설정
                                int Place = 0;
                                if (shtCellType[i].Length == 3)
                                    Place = Convert.ToInt32(shtCellType[i].Substring(2, 1).ToString());
                                FarPoint.Win.Spread.CellType.NumberCellType num = new FarPoint.Win.Spread.CellType.NumberCellType();

                                //num.LeadingZero = FarPoint.Win.Spread.CellType.LeadingZero.No;

                                num.DecimalSeparator = ".";
                                num.DecimalPlaces = Place;
                                num.FixedPoint = true;
                                num.Separator = ",";
                                num.ShowSeparator = true;
                                num.MaximumValue = 99999999999999;
                                num.MinimumValue = -99999999999999;
                                baseGrid.Sheets[0].Columns[i].CellType = num;
                                break;
                            case "HL":	// HyperLink 셀 설정
                                baseGrid.Sheets[0].Columns[i].CellType = new HyperLinkCellType();
                                break;
                            case "PG":	// Progress 셀 설정
                                baseGrid.Sheets[0].Columns[i].CellType = new ProgressCellType();
                                break;
                            case "RT":	// RichText 셀 설정
                                baseGrid.Sheets[0].Columns[i].CellType = new RichTextCellType();
                                break;
                            case "SC":	// SliderCell 셀 설정
                                baseGrid.Sheets[0].Columns[i].CellType = new SliderCellType();
                                break;
                            case "NL":	// 필수입력 사항 체크하지 않는 로직을 탄다
                                baseGrid.Sheets[0].Columns[i].CellType = new TextCellType();
                                baseGrid.Sheets[0].Columns[i].CanFocus = false;
                                if (shtCellType[i].ToString() == "NLV") // NLV인 경우 숨김
                                    baseGrid.ActiveSheet.Columns[i].Visible = false;
                                break;
                            case "PC":	// Percent 셀 설정
                                baseGrid.Sheets[0].Columns[i].CellType = new PercentCellType();
                                break;
                            case "CB":	// ComboBox 셀 설정
                                if (G1Etc[i] != null && G1Etc[i].Length > 0)
                                {
                                    ComboBoxCellType comboType = new ComboBoxCellType();
                                    comboType.MaxDrop = 20;

                                    Regex rx = new Regex("#");
                                    string Tmp = G1Etc[i].ToString();	//.Substring(G1Etc[k].IndexOf(":")+1, G1Etc[k].Length - G1Etc[k].IndexOf(":")-1);

                                    string Tmp1 = "";
                                    string Tmp2 = "";
                                    if (Tmp.IndexOf("|") > 0)
                                    {
                                        Tmp1 = Tmp.Substring(0, Tmp.IndexOf("|"));
                                        Tmp2 = Tmp.Substring(Tmp.IndexOf("|") + 1, Tmp.Length - Tmp.IndexOf("|") - 1);
                                        string[] cboMsg1 = rx.Split(Tmp1);
                                        string[] cboMsg2 = rx.Split(Tmp2);

                                        comboType.ItemData = cboMsg1;	// Key, Text
                                        comboType.Items = cboMsg2;		// Value
                                        comboType.EditorValue = FarPoint.Win.Spread.CellType.EditorValue.ItemData;

                                        baseGrid.Sheets[0].Columns[i].CellType = comboType;
                                    }
                                    else
                                    {
                                        string[] cboMsg1 = rx.Split(Tmp);

                                        comboType.Items = cboMsg1;
                                        baseGrid.Sheets[0].Columns[i].CellType = comboType;
                                    }
                                    if (shtCellType[i].ToString() == "CBV") // NLV인 경우 숨김
                                        baseGrid.ActiveSheet.Columns[i].Visible = false;

                                }
                                break;
                            case "ML": //TextBox MultiLine
                                FarPoint.Win.Spread.CellType.TextCellType MultiType = new FarPoint.Win.Spread.CellType.TextCellType();
                                MultiType.Multiline = true;

                                baseGrid.Sheets[0].Columns[i].CellType = MultiType;
                                break;
                            case "GN":	// General & 영문 대문자로 변환
                                baseGrid.Sheets[0].Columns[i].CellType = new TextCellType();
                                FarPoint.Win.Spread.CellType.TextCellType textCellType = new FarPoint.Win.Spread.CellType.TextCellType();
                                baseGrid.Sheets[0].Columns[i].CellType = textCellType;
                                //if (G1Etc[i] != null && G1Etc[i].Length > 0)  // length 체크
                                //{
                                //    string[] EtcData = null;
                                //    EtcData = G1Etc[i].ToString().Split(';');
                                //    textCellType.MaxLength = Convert.ToInt32(EtcData[0]);
                                //}
                                break;
                            default:	// General 설정
                                baseGrid.Sheets[0].Columns[i].CellType = new TextCellType();
                                FarPoint.Win.Spread.CellType.TextCellType textCellType1 = new FarPoint.Win.Spread.CellType.TextCellType();
                                textCellType1.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
                                baseGrid.Sheets[0].Columns.Get(i).CellType = textCellType1;                                
                                //if (G1Etc[i] != null && G1Etc[i].Length > 0)  // length 체크
                                //{
                                //    string[] EtcData = null;
                                //    EtcData = G1Etc[i].ToString().Split(';');
                                //    textCellType1.MaxLength = Convert.ToInt32(EtcData[0]);
                                //}
                                break;
                        }

                    }
                    /****************************************************************************/

                    /********************** Column 정렬 설정 **************************************/
                    if (sheetAlign.Length > i)
                    {
                        switch (sheetAlign[i])
                        {
                            case "L":	// 왼쪽 정렬 MIDDLE
                                baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
                                baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
                                break;
                            case "LT":	// 왼쪽 정렬 TOP
                                baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
                                baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                                break;
                            case "LB":	// 왼쪽 정렬 BOTTOM
                                baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
                                baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Bottom;
                                break;
                            case "C": // 가운데 정렬
                                baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
                                baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
                                break;
                            case "CT":	// 가운데 정렬 TOP
                                baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
                                baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                                break;
                            case "CB":	// 가운데 정렬 BOTTOM
                                baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
                                baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Bottom;
                                break;
                            case "R":	// 오른쪽 정렬
                                baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;
                                baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
                                break;
                            case "RT":	// 오른쪽 정렬 TOP
                                baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;
                                baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                                break;
                            case "RB":	// 오른쪽 정렬 BOTTOM
                                baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;
                                baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Bottom;
                                break;
                            default:	// 디폴트 왼쪽 정렬
                                baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
                                baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
                                break;
                        }
                    }
                    /****************************************************************************/

                    /********************** Column 색상 및 Lock 설정 **************************************/
                    if (shtCellColor.Length > i)
                    {
                        switch (shtCellColor[i])
                        {
                            case 0:	// 일반
                                baseGrid.Sheets[0].Columns[i].BackColor = Color.White;
                                baseGrid.Sheets[0].Columns[i].Locked = false;
                                baseGrid.Sheets[0].Columns[i].CanFocus = true;
                                break;
                            case 1:	// 필수입력
                                baseGrid.Sheets[0].Columns[i].BackColor = SystemBase.Validation.Kind_LightCyan; //System.Drawing.Color.FromArgb(242, 252, 254); //Color.LightCyan
                                baseGrid.Sheets[0].Columns[i].Locked = false;
                                baseGrid.Sheets[0].Columns[i].CanFocus = true;
                                break;
                            case 2:	// 읽기전용, 필수
                                baseGrid.Sheets[0].Columns[i].BackColor = System.Drawing.Color.FromArgb(239, 239, 239); //Color.Gainsboro
                                baseGrid.Sheets[0].Columns[i].Locked = true;
                                baseGrid.Sheets[0].Columns[i].CanFocus = true;
                                break;
                            case 3:	// 읽기전용, 필수항목에서 제외
                                baseGrid.Sheets[0].Columns[i].BackColor = System.Drawing.Color.FromArgb(238, 238, 238); // Color.LightGray
                                baseGrid.Sheets[0].Columns[i].Locked = true;
                                baseGrid.Sheets[0].Columns[i].CanFocus = true;
                                break;
                            case 4:	// 읽기전용, 팝업전용
                                baseGrid.Sheets[0].Columns[i].BackColor = Color.White;
                                baseGrid.Sheets[0].Columns[i].Locked = true;
                                baseGrid.Sheets[0].Columns[i].CanFocus = true;
                                break;
                            case 5:	// 읽기전용, 필수항목에서 제외, Focus 제외
                                baseGrid.Sheets[0].Columns[i].BackColor = System.Drawing.Color.FromArgb(238, 238, 238);
                                baseGrid.Sheets[0].Columns[i].Locked = true;
                                baseGrid.Sheets[0].Columns[i].CanFocus = false;
                                break;
                            case 6:	// 읽기전용, 필수항목, Focus 제외
                                baseGrid.Sheets[0].Columns[i].BackColor = System.Drawing.Color.FromArgb(239, 239, 239);
                                baseGrid.Sheets[0].Columns[i].Locked = true;
                                baseGrid.Sheets[0].Columns[i].CanFocus = false;
                                break;
                            default:	// 일반
                                baseGrid.Sheets[0].Columns[i].BackColor = Color.White;
                                baseGrid.Sheets[0].Columns[i].Locked = false;
                                baseGrid.Sheets[0].Columns[i].CanFocus = true;
                                break;
                        }

                    }
                    /********************** Column Type 설정 **************************************/

                    /********************** Summery 하단 고정합계 **************************************/
                    if (sheetTitle.Length > 0 && shtSummary == true && baseGrid.Sheets[0].Rows.Count > 0)
                    {
                        if (i == 1)
                        {
                            baseGrid.Sheets[0].FrozenTrailingRowCount = 1;	//하단 Column 1줄 고정

                            baseGrid.Sheets[0].RowHeader.Cells[baseGrid.Sheets[0].Rows.Count - 1, 0].Text = "합계";
                            baseGrid.Sheets[0].Rows[baseGrid.Sheets[0].Rows.Count - 1].BackColor = System.Drawing.Color.FromArgb(242, 244, 246); // .FromName("Beige");
                            baseGrid.Sheets[0].Rows[baseGrid.Sheets[0].Rows.Count - 1].Locked = true;
                        }
                        FarPoint.Win.ComplexBorder complexBorder1 = new FarPoint.Win.ComplexBorder(new FarPoint.Win.ComplexBorderSide(FarPoint.Win.ComplexBorderSideStyle.None), new FarPoint.Win.ComplexBorderSide(FarPoint.Win.ComplexBorderSideStyle.ThinLine, System.Drawing.Color.FromArgb(((System.Byte)(100)), ((System.Byte)(100)), ((System.Byte)(100)))), new FarPoint.Win.ComplexBorderSide(FarPoint.Win.ComplexBorderSideStyle.None), new FarPoint.Win.ComplexBorderSide(FarPoint.Win.ComplexBorderSideStyle.None));
                        baseGrid.Sheets[0].Cells.Get(baseGrid.Sheets[0].Rows.Count - 1, i).Border = complexBorder1;

                        if (baseGrid.Sheets[0].GetCellType(baseGrid.Sheets[0].Rows.Count - 1, i).ToString() == "NumberCellType" ||
                            baseGrid.Sheets[0].GetCellType(baseGrid.Sheets[0].Rows.Count - 1, i).ToString() == "PercentCellType")
                        {
                            string Str = IntToString(i);
                            string Area = Str + "1:" + Str + Convert.ToString(baseGrid.Sheets[0].Rows.Count - 1);
                            Cell r = baseGrid.ActiveSheet.Cells[baseGrid.Sheets[0].Rows.Count - 1, i];

                            r.Formula = "SUM(" + Area + ")";
                        }
                        else
                        {
                            baseGrid.Sheets[0].Cells[baseGrid.Sheets[0].Rows.Count - 1, i].CellType = new TextCellType();
                        }
                    }
                    /****************************************************************************/
                }
                if (shtMessage == true)
                {
                    if (Query != null && baseGrid.Sheets[0].Rows.Count == 0)
                        MessageBox.Show(SystemBase.Base.MessageRtn("SY014"), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log("grdCommSheet (그리드공통 조회실패 )", Query.ToString() + "\n" + f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY013"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                baseGrid.Sheets[0].Rows.Count = 0;
            }
        }
        #endregion

        #region grdCommSheet(16) - Row 단위 속정 정의 안하고 조회용 그리드 홀짝 색깔 틀림 - 2009-04-03
        public static void grdCommSheet(FarPoint.Win.Spread.FpSpread baseGrid
            , string Query
            , string[] sheetTitle
            , string[] sheetTitle2
            , string[] sheetTitle3
            , int[] sheetWidth
            , string[] sheetAlign
            , string[] shtCellType
            , int[] shtCellColor
            , string[] G1Etc
            , int shtHeaderRowCount
            , bool shtSummary
            , bool shtMessage
            , int shtFixR
            , int shtFixC
            , bool SearchFlag
            )
        {
            try
            {
                /********************** 그리드 초기화 **************************************/
                baseGrid.Reset();
                //********************** Clipboard False***********************************/
                baseGrid.AutoClipboard = false;
                /********************** 폰트 정의 **************************************/
                baseGrid.Font = new System.Drawing.Font("돋움", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
                /********************** 그리드 스타일 정의 **************************************/
                baseGrid.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
                /********************** 스크롤바 정의 **************************************/
                baseGrid.HorizontalScrollBarPolicy = FarPoint.Win.Spread.ScrollBarPolicy.AsNeeded;
                /********************** 스크롤바 정의 **************************************/
                baseGrid.VerticalScrollBarPolicy = FarPoint.Win.Spread.ScrollBarPolicy.AsNeeded;
                /********************** 스크롤바 정의 **************************************/
                baseGrid.ScrollBarTrackPolicy = FarPoint.Win.Spread.ScrollBarTrackPolicy.Both;
                /********************** 그리드 칼럼수 지정 **************************************/
                baseGrid.ActiveSheet.Columns.Count = sheetTitle.Length;
                /********************** 그리드 Row수 지정 **************************************/
                //baseGrid.ActiveSheet.Rows.Count = 0;
                /********************** 그리드 Row색상 지정 **************************************/
                baseGrid.Sheets[0].AlternatingRows[0].BackColor = Color.FromArgb(255, 255, 255);
                baseGrid.Sheets[0].AlternatingRows[1].BackColor = Color.FromArgb(255, 255, 255);
                //baseGrid.Sheets[0].AlternatingRows[1].BackColor = Color.FromArgb(245, 245, 245);
                /********************** 그리드 RowHeader 넓이 지정 *********************************/
                baseGrid.Sheets[0].RowHeader.Columns[0].Width = 32;
                /********************** 그리드 바탕색상 지정 **************************************/
                baseGrid.Sheets[0].GrayAreaBackColor = Color.FromName("white");
                /********************** 1Row 선택 기준(단일, 1Row별, 1Cell별) **************************************/
                //				baseGrid.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.SingleSelect;				//.ExtendedSelect;
                /********************** ColumnHeader 수 지정**************************************/
                baseGrid.Sheets[0].ColumnHeader.RowCount = shtHeaderRowCount;
                /********************** HorizontalGridLine 가로 가이드라인 색상 지정**************************************/
                baseGrid.Sheets[0].HorizontalGridLine = new FarPoint.Win.Spread.GridLine(FarPoint.Win.Spread.GridLineType.Flat, Color.FromArgb(193, 201, 212));
                /********************** VerticalGridLine 세로 가이드라인 색상 지정**************************************/
                baseGrid.Sheets[0].VerticalGridLine = new FarPoint.Win.Spread.GridLine(FarPoint.Win.Spread.GridLineType.Flat, Color.FromArgb(193, 201, 212));
                /********************** 왼쪽 상단 코너 색상 지정**************************************/
                baseGrid.Sheets[0].SheetCornerStyle.BackColor = System.Drawing.Color.FromArgb(242, 244, 246); // .FromName("Beige");
                /********************** 첫번째 Head color지정**************************************/
                baseGrid.Sheets[0].ColumnHeader.Rows[0].BackColor = System.Drawing.Color.FromArgb(242, 244, 246); // .FromName("Beige");
                /********************** 두번째 Head color지정**************************************/
                if (baseGrid.Sheets[0].ColumnHeader.RowCount > 1)
                    baseGrid.Sheets[0].ColumnHeader.Rows[1].BackColor = System.Drawing.Color.FromArgb(242, 244, 246); // .FromName("Beige");
                /********************** 세번째 Head color지정**************************************/
                if (baseGrid.Sheets[0].ColumnHeader.RowCount > 2)
                    baseGrid.Sheets[0].ColumnHeader.Rows[2].BackColor = System.Drawing.Color.FromArgb(242, 244, 246); // .FromName("Beige");
                /********************** 첫번째 Colum color 지정**************************************/
                baseGrid.Sheets[0].RowHeader.Columns[0].BackColor = System.Drawing.Color.FromArgb(242, 244, 246); // .FromName("Beige");
                /********************** 1번째 Cell(프라이머리) 키 숨김 지정**************************************/
                baseGrid.ActiveSheet.Columns[0].Visible = false;
                /********************** 입력시 기존 데이타 삭제후 입력**************************************/
                baseGrid.EditModeReplace = true;
                /********************** Clipboard 복사시 Head값 미포함**************************************/
                baseGrid.ClipboardOptions = FarPoint.Win.Spread.ClipboardOptions.NoHeaders;
                //********************** 입력 RowMode********************************************************/
                //baseGrid.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.RowMode;
                //********************** 그리드 focus 칼라***************************************************/
                baseGrid.FocusRenderer = new FarPoint.Win.Spread.SolidFocusIndicatorRenderer(Color.CornflowerBlue, 2);

                if (Query != null)
                {
                    DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);
                    baseGrid.Sheets[0].DataSource = dt;

                    if (dt.Rows.Count > 0 && shtSummary == true)
                        baseGrid.ActiveSheet.Rows.Count = dt.Rows.Count + 1;//그리드Row수 지정
                    else
                    {
                        baseGrid.ActiveSheet.Rows.Count = dt.Rows.Count;//그리드Row수 지정
                    }

                    if (dt.Rows.Count > 999 && dt.Rows.Count < 10000)
                        baseGrid.Sheets[0].RowHeader.Columns[0].Width = 38;
                    else if (dt.Rows.Count > 9999)
                        baseGrid.Sheets[0].RowHeader.Columns[0].Width = 44;
                }
                else
                {
                    baseGrid.ActiveSheet.Rows.Count = 0;
                }
                /********************** 그리드 Head 높이 지정 **************************************/
                baseGrid.Sheets[0].ColumnHeader.Rows[0].Height = 28;
                if (shtHeaderRowCount > 1)
                {
                    /********************** 그리드 Head 높이 지정 **************************************/
                    baseGrid.Sheets[0].ColumnHeader.Rows[1].Height = 28;
                }
                if (shtHeaderRowCount > 2)
                {
                    /********************** 그리드 Head 높이 지정 **************************************/
                    baseGrid.Sheets[0].ColumnHeader.Rows[2].Height = 28;
                }

                //****************************************************************************************************************
                //: 고정 Column 또는 Row 설정
                //****************************************************************************************************************
                baseGrid.Sheets[0].FrozenColumnCount = shtFixC + 1;
                baseGrid.Sheets[0].FrozenRowCount = shtFixR;
                //****************************************************************************************************************
                //: 버튼 설정
                //****************************************************************************************************************
                FarPoint.Win.Spread.CellType.ButtonCellType btnType = new FarPoint.Win.Spread.CellType.ButtonCellType();
                btnType.Text = "...";

                //****************************************************************************************************************
                //: Key Enter시 Next Col로 Focus 이동
                //****************************************************************************************************************
                FarPoint.Win.Spread.InputMap im = baseGrid.GetInputMap(FarPoint.Win.Spread.InputMapMode.WhenFocused);
                im.Put(new FarPoint.Win.Spread.Keystroke(Keys.Enter, Keys.None), FarPoint.Win.Spread.SpreadActions.MoveToNextColumnWrap);


                int sheetLength = 0, shtTitleSpanTmp = 1, shtTitleSpanTmp2 = 1, shtTitleSpanTmp3 = 1;
                sheetLength = sheetTitle.Length;

                if (sheetTitle.Length < sheetTitle2.Length)
                    sheetLength = sheetTitle2.Length;

                //--------------------------------------------------------------------------------------------------------------------------

                for (int i = 0; i < sheetTitle.Length; i++)
                {
                    //****************************************************************************************************************
                    //: Column 폭 넓이 설정
                    //****************************************************************************************************************
                    if (sheetWidth.Length > i)
                        baseGrid.Sheets[0].Columns[i].Width = sheetWidth[i];

                    //****************************************************************************************************************
                    //: sheet 1번째 Header Text 입력
                    //****************************************************************************************************************
                    if (sheetTitle.Length > i)
                        baseGrid.Sheets[0].ColumnHeader.Cells[0, i].Text = sheetTitle[i].ToString();

                    //****************************************************************************************************************
                    //: sheet 2번째 Header Text 입력
                    //****************************************************************************************************************
                    if (sheetTitle2.Length > i && baseGrid.Sheets[0].ColumnHeader.RowCount > 1)
                        baseGrid.Sheets[0].ColumnHeader.Cells[1, i].Text = sheetTitle2[i].ToString();

                    //****************************************************************************************************************
                    //: sheet 3번째 Header Text 입력
                    //****************************************************************************************************************
                    if (sheetTitle3.Length > i && baseGrid.Sheets[0].ColumnHeader.RowCount > 2)
                        baseGrid.Sheets[0].ColumnHeader.Cells[2, i].Text = sheetTitle3[i].ToString();

                    //****************************************************************************************************************
                    //: columnSpan 설정
                    //****************************************************************************************************************
                    if (sheetTitle.Length > i)
                    {
                        if (shtHeaderRowCount > 0)
                        {
                            if (i > 1 && sheetTitle[i].ToString() == sheetTitle[i - 1].ToString())
                            {
                                shtTitleSpanTmp++;
                                baseGrid.Sheets[0].ColumnHeader.Cells[0, i + 1 - shtTitleSpanTmp].ColumnSpan = shtTitleSpanTmp;

                            }
                            else
                                shtTitleSpanTmp = 1;
                        }

                        if (shtHeaderRowCount > 1)
                        {
                            if (i > 1 && sheetTitle2[i].ToString() == sheetTitle2[i - 1].ToString())
                            {
                                shtTitleSpanTmp2++;
                                baseGrid.Sheets[0].ColumnHeader.Cells[1, i + 1 - shtTitleSpanTmp2].ColumnSpan = shtTitleSpanTmp2;
                            }
                            else
                                shtTitleSpanTmp2 = 1;
                        }

                        if (shtHeaderRowCount > 2)
                        {
                            if (i > 1 && sheetTitle3[i].ToString() == sheetTitle3[i - 1].ToString())
                            {
                                shtTitleSpanTmp3++;
                                baseGrid.Sheets[0].ColumnHeader.Cells[2, i + 1 - shtTitleSpanTmp3].ColumnSpan = shtTitleSpanTmp3;
                            }
                            else
                                shtTitleSpanTmp3 = 1;
                        }


                        if (sheetTitle2[i] != null)
                        {
                            if (sheetTitle[i].ToString() == sheetTitle2[i].ToString())
                            {
                                baseGrid.Sheets[0].ColumnHeader.Cells[0, i].RowSpan = 2;
                            }
                        }

                        if (sheetTitle2[i] != null && sheetTitle3[i] != null)
                        {
                            if (sheetTitle[i].ToString() == sheetTitle2[i].ToString() && sheetTitle2[i].ToString() == sheetTitle3[i].ToString())
                            {
                                baseGrid.Sheets[0].ColumnHeader.Cells[0, i].RowSpan = 3;
                            }
                            if (sheetTitle2[i].ToString() == sheetTitle3[i].ToString())
                            {
                                baseGrid.Sheets[0].ColumnHeader.Cells[1, i].RowSpan = 2;
                            }
                        }
                    }

                    /********************** Column Type 설정 **************************************/
                    if (shtCellType.Length > i)
                    {
                        string CellType = "";
                        if (shtCellType[i].Length > 2)
                            CellType = shtCellType[i].Substring(0, 2);
                        else
                            CellType = shtCellType[i];

                        switch (CellType)
                        {
                            case "BT":	// Button 셀 설정
                                baseGrid.Sheets[0].Columns[i].CellType = btnType;
                                break;
                            case "CK":	// CheckBox 셀 설정
                                baseGrid.Sheets[0].Columns[i].CellType = new CheckBoxCellType();
                                break;
                            case "CH":	// CheckBox Head에 추가
                                FarPoint.Win.Spread.CellType.CheckBoxCellType checkBoxCellType1 = new FarPoint.Win.Spread.CellType.CheckBoxCellType();
                                checkBoxCellType1.Caption = sheetTitle[i];
                                baseGrid.Sheets[0].ColumnHeader.Cells.Get(0, i).CellType = checkBoxCellType1;
                                baseGrid.Sheets[0].ColumnHeader.Cells.Get(0, i).Border = new LineBorder(ColorTranslator.FromHtml("#9eb6ce"), 1, true, true, true, true);
                                baseGrid.Sheets[0].Columns[i].CellType = new CheckBoxCellType();
                                break;
                            case "CC":	// Currency 셀 설정
                                baseGrid.Sheets[0].Columns[i].CellType = new CurrencyCellType();
                                break;
                            case "DT":	// Date 셀 설정
                                baseGrid.Sheets[0].Columns[i].CellType = new DateTimeCellType();
                                break;
                            case "DY":	// Mask 셀 설정(년월)                            
                                FarPoint.Win.Spread.CellType.MaskCellType dm = new FarPoint.Win.Spread.CellType.MaskCellType();
                                dm.Mask = "####-##";
                                baseGrid.Sheets[0].Columns[i].CellType = dm;
                                break;
                            case "DD":	// Date 셀 설정(월콤보)                            
                                FarPoint.Win.Spread.CellType.ComboBoxCellType cb = new FarPoint.Win.Spread.CellType.ComboBoxCellType();
                                ListBox list = new ListBox();
                                list.Items.AddRange(new Object[] { "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12" });
                                cb.ListControl = list;
                                cb.Items = (new String[] { "", "", "", "", "", "", "", "", "", "", "", "" });
                                baseGrid.ActiveSheet.Columns[i].CellType = cb;
                                break;
                            case "PW":	// Password 셀 설정
                                FarPoint.Win.Spread.CellType.TextCellType passType = new FarPoint.Win.Spread.CellType.TextCellType();
                                passType.PasswordChar = Convert.ToChar("*");
                                baseGrid.Sheets[0].Columns[i].CellType = passType;
                                break;
                            case "MK":	// MaskCellType 셀 설정 | Mask 사용예 : MK###-###-####
                                FarPoint.Win.Spread.CellType.MaskCellType picType = new FarPoint.Win.Spread.CellType.MaskCellType();
                                if (G1Etc[i].ToString().Length > 0)
                                    picType.Mask = G1Etc[i].ToString();	//shtCellType[i].Substring(2,shtCellType[i].Length-2).ToString();
                                picType.MaskChar = Convert.ToChar("_");
                                baseGrid.Sheets[0].Columns[i].CellType = picType;
                                break;
                            case "NM":	// Number 셀 설정
                                int Place = 0;
                                if (shtCellType[i].Length == 3)
                                    Place = Convert.ToInt32(shtCellType[i].Substring(2, 1).ToString());
                                FarPoint.Win.Spread.CellType.NumberCellType num = new FarPoint.Win.Spread.CellType.NumberCellType();

                                //num.LeadingZero = FarPoint.Win.Spread.CellType.LeadingZero.No;

                                num.DecimalSeparator = ".";
                                num.DecimalPlaces = Place;
                                num.FixedPoint = true;
                                num.Separator = ",";
                                num.ShowSeparator = true;
                                num.MaximumValue = 99999999999999;
                                num.MinimumValue = -99999999999999;
                                baseGrid.Sheets[0].Columns[i].CellType = num;
                                break;
                            case "HL":	// HyperLink 셀 설정
                                baseGrid.Sheets[0].Columns[i].CellType = new HyperLinkCellType();
                                break;
                            case "PG":	// Progress 셀 설정
                                baseGrid.Sheets[0].Columns[i].CellType = new ProgressCellType();
                                break;
                            case "RT":	// RichText 셀 설정
                                baseGrid.Sheets[0].Columns[i].CellType = new RichTextCellType();
                                break;
                            case "SC":	// SliderCell 셀 설정
                                baseGrid.Sheets[0].Columns[i].CellType = new SliderCellType();
                                break;
                            case "NL":	// 필수입력 사항 체크하지 않는 로직을 탄다
                                baseGrid.Sheets[0].Columns[i].CellType = new TextCellType();
                                baseGrid.Sheets[0].Columns[i].CanFocus = false;
                                if (shtCellType[i].ToString() == "NLV") // NLV인 경우 숨김
                                    baseGrid.ActiveSheet.Columns[i].Visible = false;
                                break;
                            case "PC":	// Percent 셀 설정
                                baseGrid.Sheets[0].Columns[i].CellType = new PercentCellType();
                                break;
                            case "CB":	// ComboBox 셀 설정
                                if (G1Etc[i] != null && G1Etc[i].Length > 0)
                                {
                                    ComboBoxCellType comboType = new ComboBoxCellType();
                                    comboType.MaxDrop = 20;

                                    Regex rx = new Regex("#");
                                    string Tmp = G1Etc[i].ToString();	//.Substring(G1Etc[k].IndexOf(":")+1, G1Etc[k].Length - G1Etc[k].IndexOf(":")-1);

                                    string Tmp1 = "";
                                    string Tmp2 = "";
                                    if (Tmp.IndexOf("|") > 0)
                                    {
                                        Tmp1 = Tmp.Substring(0, Tmp.IndexOf("|"));
                                        Tmp2 = Tmp.Substring(Tmp.IndexOf("|") + 1, Tmp.Length - Tmp.IndexOf("|") - 1);
                                        string[] cboMsg1 = rx.Split(Tmp1);
                                        string[] cboMsg2 = rx.Split(Tmp2);

                                        comboType.ItemData = cboMsg1;	// Key, Text
                                        comboType.Items = cboMsg2;		// Value
                                        comboType.EditorValue = FarPoint.Win.Spread.CellType.EditorValue.ItemData;

                                        baseGrid.Sheets[0].Columns[i].CellType = comboType;
                                    }
                                    else
                                    {
                                        string[] cboMsg1 = rx.Split(Tmp);

                                        comboType.Items = cboMsg1;
                                        baseGrid.Sheets[0].Columns[i].CellType = comboType;
                                    }
                                    if (shtCellType[i].ToString() == "CBV") // NLV인 경우 숨김
                                        baseGrid.ActiveSheet.Columns[i].Visible = false;

                                }
                                break;
                            case "GN":	// General & 영문 대문자로 변환
                                baseGrid.Sheets[0].Columns[i].CellType = new TextCellType();
                                break;
                            case "ML": //TextBox MultiLine
                                FarPoint.Win.Spread.CellType.TextCellType MultiType = new FarPoint.Win.Spread.CellType.TextCellType();
                                MultiType.Multiline = true;

                                baseGrid.Sheets[0].Columns[i].CellType = MultiType;
                                break;
                            default:	// General 설정
                                baseGrid.Sheets[0].Columns[i].CellType = new TextCellType();
                                FarPoint.Win.Spread.CellType.TextCellType textCellType1 = new FarPoint.Win.Spread.CellType.TextCellType();
                                textCellType1.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
                                baseGrid.Sheets[0].Columns.Get(i).CellType = textCellType1;
                                break;
                        }

                    }
                    /****************************************************************************/

                    /********************** Column 정렬 설정 **************************************/
                    if (sheetAlign.Length > i)
                    {
                        switch (sheetAlign[i])
                        {
                            case "L":	// 왼쪽 정렬 MIDDLE
                                baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
                                baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
                                break;
                            case "LT":	// 왼쪽 정렬 TOP
                                baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
                                baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                                break;
                            case "LB":	// 왼쪽 정렬 BOTTOM
                                baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
                                baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Bottom;
                                break;
                            case "C": // 가운데 정렬
                                baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
                                baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
                                break;
                            case "CT":	// 가운데 정렬 TOP
                                baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
                                baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                                break;
                            case "CB":	// 가운데 정렬 BOTTOM
                                baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
                                baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Bottom;
                                break;
                            case "R":	// 오른쪽 정렬
                                baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;
                                baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
                                break;
                            case "RT":	// 오른쪽 정렬 TOP
                                baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;
                                baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                                break;
                            case "RB":	// 오른쪽 정렬 BOTTOM
                                baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;
                                baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Bottom;
                                break;
                            default:	// 디폴트 왼쪽 정렬
                                baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
                                baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
                                break;
                        }
                    }
                    /****************************************************************************/

                    /********************** Summery 하단 고정합계 **************************************/
                    if (sheetTitle.Length > 0 && shtSummary == true && baseGrid.Sheets[0].Rows.Count > 0)
                    {
                        if (i == 1)
                        {
                            baseGrid.Sheets[0].FrozenTrailingRowCount = 1;	//하단 Column 1줄 고정

                            baseGrid.Sheets[0].RowHeader.Cells[baseGrid.Sheets[0].Rows.Count - 1, 0].Text = "합계";
                            baseGrid.Sheets[0].Rows[baseGrid.Sheets[0].Rows.Count - 1].BackColor = System.Drawing.Color.FromArgb(242, 244, 246); // .FromName("Beige");
                            baseGrid.Sheets[0].Rows[baseGrid.Sheets[0].Rows.Count - 1].Locked = true;
                        }
                        FarPoint.Win.ComplexBorder complexBorder1 = new FarPoint.Win.ComplexBorder(new FarPoint.Win.ComplexBorderSide(FarPoint.Win.ComplexBorderSideStyle.None), new FarPoint.Win.ComplexBorderSide(FarPoint.Win.ComplexBorderSideStyle.ThinLine, System.Drawing.Color.FromArgb(((System.Byte)(100)), ((System.Byte)(100)), ((System.Byte)(100)))), new FarPoint.Win.ComplexBorderSide(FarPoint.Win.ComplexBorderSideStyle.None), new FarPoint.Win.ComplexBorderSide(FarPoint.Win.ComplexBorderSideStyle.None));
                        baseGrid.Sheets[0].Cells.Get(baseGrid.Sheets[0].Rows.Count - 1, i).Border = complexBorder1;

                        if (baseGrid.Sheets[0].GetCellType(baseGrid.Sheets[0].Rows.Count - 1, i).ToString() == "NumberCellType" ||
                            baseGrid.Sheets[0].GetCellType(baseGrid.Sheets[0].Rows.Count - 1, i).ToString() == "PercentCellType")
                        {
                            string Str = IntToString(i);
                            string Area = Str + "1:" + Str + Convert.ToString(baseGrid.Sheets[0].Rows.Count - 1);
                            Cell r = baseGrid.ActiveSheet.Cells[baseGrid.Sheets[0].Rows.Count - 1, i];

                            r.Formula = "SUM(" + Area + ")";
                        }
                        else
                        {
                            baseGrid.Sheets[0].Cells[baseGrid.Sheets[0].Rows.Count - 1, i].CellType = new TextCellType();
                        }
                    }
                    /****************************************************************************/

                    /********************** Column Lock 설정 **************************************/
                    if (baseGrid.Sheets[0].Columns[i].CellType.ToString() != "CheckBoxCellType")
                    {
                        baseGrid.Sheets[0].Columns[i].Locked = true;
                    }
                    /****************************************************************************/
                }
                if (shtMessage == true)
                {
                    if (Query != null && baseGrid.Sheets[0].Rows.Count == 0)
                        MessageBox.Show(SystemBase.Base.MessageRtn("SY014"), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log("grdCommSheet (그리드공통 조회실패 )", Query.ToString() + "\n" + f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY013"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                baseGrid.Sheets[0].Rows.Count = 0;
            }
        }
        #endregion
        
        #endregion

        #region grdCommSheet(13) - 그리드 디자인만 재정의
        public static void grdCommSheet(FarPoint.Win.Spread.FpSpread baseGrid
            //, string		Query
            , string[] sheetTitle
            , string[] sheetTitle2
            , string[] sheetTitle3
            , int[] sheetWidth
            , string[] sheetAlign
            , string[] shtCellType
            , int[] shtCellColor
            , string[] G1Etc
            , int shtHeaderRowCount
            , int shtFixR
            , int shtFixC
            , bool shtSummary
            )
        {
            try
            {
                /********************** 그리드 스타일 정의 **************************************/
                baseGrid.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
                /********************** 폰트 정의 **************************************/
                baseGrid.Font = new System.Drawing.Font("돋움", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
                /********************** 스크롤바 정의 **************************************/
                baseGrid.HorizontalScrollBarPolicy = FarPoint.Win.Spread.ScrollBarPolicy.AsNeeded;
                /********************** 스크롤바 정의 **************************************/
                baseGrid.VerticalScrollBarPolicy = FarPoint.Win.Spread.ScrollBarPolicy.AsNeeded;
                /********************** 그리드 칼럼수 지정 **************************************/
                baseGrid.ActiveSheet.Columns.Count = sheetTitle.Length;
                /********************** 그리드 Row수 지정 **************************************/
                //baseGrid.ActiveSheet.Rows.Count = 0;
                /********************** 그리드 Row색상 지정 **************************************/
                baseGrid.Sheets[0].AlternatingRows[0].BackColor = Color.FromName("WhiteSmoke");
                /********************** 그리드 RowHeader 넓이 지정 *********************************/
                baseGrid.Sheets[0].RowHeader.Columns[0].Width = 32;
                /********************** 그리드 바탕색상 지정 **************************************/
                baseGrid.Sheets[0].GrayAreaBackColor = Color.FromName("white");
                /********************** 1Row 선택 기준(단일, 1Row별, 1Cell별) **************************************/
                baseGrid.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.Normal;				//.ExtendedSelect;
                /********************** ColumnHeader 수 지정**************************************/
                baseGrid.Sheets[0].ColumnHeader.RowCount = shtHeaderRowCount;
                /********************** HorizontalGridLine 가로 가이드라인 색상 지정**************************************/
                baseGrid.Sheets[0].HorizontalGridLine = new FarPoint.Win.Spread.GridLine(FarPoint.Win.Spread.GridLineType.Flat, Color.FromArgb(193, 201, 212));
                /********************** VerticalGridLine 세로 가이드라인 색상 지정**************************************/
                baseGrid.Sheets[0].VerticalGridLine = new FarPoint.Win.Spread.GridLine(FarPoint.Win.Spread.GridLineType.Flat, Color.FromArgb(193, 201, 212));
                /********************** 왼쪽 상단 코너 색상 지정**************************************/
                baseGrid.Sheets[0].SheetCornerStyle.BackColor = System.Drawing.Color.FromArgb(242, 244, 246); // .FromName("Beige");
                /********************** 첫번째 Head color지정**************************************/
                baseGrid.Sheets[0].ColumnHeader.Rows[0].BackColor = System.Drawing.Color.FromArgb(242, 244, 246); // .FromName("Beige");
                /********************** 두번째 Head color지정**************************************/
                if (baseGrid.Sheets[0].ColumnHeader.RowCount > 1)
                    baseGrid.Sheets[0].ColumnHeader.Rows[1].BackColor = System.Drawing.Color.FromArgb(242, 244, 246); // .FromName("Beige");
                /********************** 첫번째 Colum color 지정**************************************/
                baseGrid.Sheets[0].RowHeader.Columns[0].BackColor = System.Drawing.Color.FromArgb(242, 244, 246); // .FromName("Beige");
                /********************** 1번째 Cell(프라이머리) 키 숨김 지정**************************************/
                baseGrid.ActiveSheet.Columns[0].Visible = false;
                /********************** 입력시 기존 데이타 삭제후 입력**************************************/
                baseGrid.EditModeReplace = true;
                /********************** Clipboard 복사시 Head값 미포함**************************************/
                baseGrid.ClipboardOptions = FarPoint.Win.Spread.ClipboardOptions.NoHeaders;
                //********************** 그리드 focus 칼라***************************************************/
                baseGrid.FocusRenderer = new FarPoint.Win.Spread.SolidFocusIndicatorRenderer(Color.RoyalBlue, 2);
                //********************** 그리드 숫자 입력시 초기화 후 입력***********************************/
                baseGrid.EditModeReplace = true;

                /********************** 그리드 Head 높이 지정 **************************************/
                baseGrid.Sheets[0].ColumnHeader.Rows[0].Height = 28;
                if (shtHeaderRowCount > 1)
                {
                    /********************** 그리드 Head 높이 지정 **************************************/
                    baseGrid.Sheets[0].ColumnHeader.Rows[1].Height = 28;
                }
                if (shtHeaderRowCount > 2)
                {
                    /********************** 그리드 Head 높이 지정 **************************************/
                    baseGrid.Sheets[0].ColumnHeader.Rows[2].Height = 28;
                }

                //****************************************************************************************************************
                //: 고정 Column 또는 Row 설정
                //****************************************************************************************************************
                baseGrid.Sheets[0].FrozenColumnCount = shtFixC + 1;
                baseGrid.Sheets[0].FrozenRowCount = shtFixR;
                //****************************************************************************************************************
                //: 버튼 설정
                //****************************************************************************************************************
                FarPoint.Win.Spread.CellType.ButtonCellType btnType = new FarPoint.Win.Spread.CellType.ButtonCellType();
                btnType.Text = "...";

                //****************************************************************************************************************
                //: Key Enter시 Next Col로 Focus 이동
                //****************************************************************************************************************
                FarPoint.Win.Spread.InputMap im = baseGrid.GetInputMap(FarPoint.Win.Spread.InputMapMode.WhenFocused);
                im.Put(new FarPoint.Win.Spread.Keystroke(Keys.Enter, Keys.None), FarPoint.Win.Spread.SpreadActions.MoveToNextColumnWrap);


                int sheetLength = 0, shtTitleSpanTmp = 1, shtTitleSpanTmp2 = 1, shtTitleSpanTmp3 = 1;
                sheetLength = sheetTitle.Length;

                if (sheetTitle.Length < sheetTitle2.Length)
                    sheetLength = sheetTitle2.Length;

                //--------------------------------------------------------------------------------------------------------------------------

                for (int i = 0; i < sheetTitle.Length; i++)
                {
                    //****************************************************************************************************************
                    //: Column 폭 넓이 설정
                    //****************************************************************************************************************
                    if (sheetWidth.Length > i)
                        baseGrid.Sheets[0].Columns[i].Width = sheetWidth[i];

                    //****************************************************************************************************************
                    //: columnSpan 설정
                    //****************************************************************************************************************
                    if (sheetTitle.Length > i)
                    {
                        if (i > 1 && sheetTitle[i].ToString() == sheetTitle[i - 1].ToString())
                        {
                            shtTitleSpanTmp++;
                            baseGrid.Sheets[0].ColumnHeader.Cells[0, i + 1 - shtTitleSpanTmp].ColumnSpan = shtTitleSpanTmp;
                            if (shtHeaderRowCount > 1)
                            {
                                if (i > 1 && sheetTitle2[i].ToString() == sheetTitle2[i - 1].ToString())
                                {
                                    shtTitleSpanTmp2++;
                                    baseGrid.Sheets[0].ColumnHeader.Cells[1, i + 1 - shtTitleSpanTmp2].ColumnSpan = shtTitleSpanTmp2;
                                }
                                else
                                    shtTitleSpanTmp2 = 1;
                            }

                            if (shtHeaderRowCount > 2)
                            {
                                if (i > 1 && sheetTitle3[i].ToString() == sheetTitle3[i - 1].ToString())
                                {
                                    shtTitleSpanTmp3++;
                                    baseGrid.Sheets[0].ColumnHeader.Cells[2, i + 1 - shtTitleSpanTmp3].ColumnSpan = shtTitleSpanTmp3;
                                }
                                else
                                    shtTitleSpanTmp3 = 1;
                            }
                        }
                        else
                            shtTitleSpanTmp = 1;

                        if (sheetTitle2[i] != null)
                        {
                            if (sheetTitle[i].ToString() == sheetTitle2[i].ToString())
                            {
                                baseGrid.Sheets[0].ColumnHeader.Cells[0, i].RowSpan = 2;
                            }
                        }

                        if (sheetTitle2[i] != null && sheetTitle3[i] != null)
                        {
                            if (sheetTitle[i].ToString() == sheetTitle2[i].ToString() && sheetTitle2[i].ToString() == sheetTitle3[i].ToString())
                            {
                                baseGrid.Sheets[0].ColumnHeader.Cells[0, i].RowSpan = 3;
                            }
                            if (sheetTitle2[i].ToString() == sheetTitle3[i].ToString())
                            {
                                baseGrid.Sheets[0].ColumnHeader.Cells[1, i].RowSpan = 2;
                            }
                        }

                    }

                    //****************************************************************************************************************
                    //: sheet 1번째 Header Text 입력
                    //****************************************************************************************************************
                    if (sheetTitle.Length > i)
                        baseGrid.Sheets[0].ColumnHeader.Cells[0, i].Text = sheetTitle[i].ToString();

                    //****************************************************************************************************************
                    //: sheet 2번째 Header Text 입력
                    //****************************************************************************************************************
                    if (sheetTitle2.Length > i && baseGrid.Sheets[0].ColumnHeader.RowCount > 1)
                        baseGrid.Sheets[0].ColumnHeader.Cells[1, i].Text = sheetTitle2[i].ToString();

                    /********************** Column Type 설정 **************************************/
                    if (shtCellType.Length > i)
                    {
                        string CellType = "";
                        if (shtCellType[i].Length > 2)
                            CellType = shtCellType[i].Substring(0, 2);
                        else
                            CellType = shtCellType[i];

                        switch (CellType)
                        {
                            case "BT":	// Button 셀 설정
                                baseGrid.Sheets[0].Columns[i].CellType = btnType;
                                break;
                            case "CK":	// CheckBox 셀 설정
                                baseGrid.Sheets[0].Columns[i].CellType = new CheckBoxCellType();
                                break;
                            case "CC":	// Currency 셀 설정
                                baseGrid.Sheets[0].Columns[i].CellType = new CurrencyCellType();
                                break;
                            case "DT":	// Date 셀 설정
                                baseGrid.Sheets[0].Columns[i].CellType = new DateTimeCellType();
                                break;
                            case "DD":	// Date 셀 설정(월콤보)                            
                                FarPoint.Win.Spread.CellType.ComboBoxCellType cb = new FarPoint.Win.Spread.CellType.ComboBoxCellType();
                                ListBox list = new ListBox();
                                list.Items.AddRange(new Object[] { "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12" });
                                cb.ListControl = list;
                                cb.Items = (new String[] { "", "", "", "", "", "", "", "", "", "", "", "" });
                                baseGrid.ActiveSheet.Columns[i].CellType = cb;
                                break;
                            case "PW":	// Password 셀 설정
                                FarPoint.Win.Spread.CellType.TextCellType passType = new FarPoint.Win.Spread.CellType.TextCellType();
                                passType.PasswordChar = Convert.ToChar("*");
                                baseGrid.Sheets[0].Columns[i].CellType = passType;
                                break;
                            case "MK":	// MaskCellType 셀 설정 | Mask 사용예 : MK###-###-####
                                FarPoint.Win.Spread.CellType.MaskCellType picType = new FarPoint.Win.Spread.CellType.MaskCellType();
                                if (G1Etc[i].ToString().Length > 0)
                                    picType.Mask = G1Etc[i].ToString();	//shtCellType[i].Substring(2,shtCellType[i].Length-2).ToString();
                                picType.MaskChar = Convert.ToChar("_");
                                baseGrid.Sheets[0].Columns[i].CellType = picType;
                                break;
                            case "NM":	// Number 셀 설정
                                int Place = 0;
                                if (shtCellType[i].Length == 3)
                                    Place = Convert.ToInt32(shtCellType[i].Substring(2, 1).ToString());
                                FarPoint.Win.Spread.CellType.NumberCellType num = new FarPoint.Win.Spread.CellType.NumberCellType();
                                //num.LeadingZero = FarPoint.Win.Spread.CellType.LeadingZero.No;
                                num.DecimalSeparator = ".";
                                num.DecimalPlaces = Place;
                                num.FixedPoint = true;
                                num.Separator = ",";
                                num.ShowSeparator = true;
                                num.MaximumValue = 99999999999999;
                                num.MinimumValue = -99999999999999;
                                baseGrid.Sheets[0].Columns[i].CellType = num;
                                break;
                            case "HL":	// HyperLink 셀 설정
                                baseGrid.Sheets[0].Columns[i].CellType = new HyperLinkCellType();
                                break;
                            case "PG":	// Progress 셀 설정
                                baseGrid.Sheets[0].Columns[i].CellType = new ProgressCellType();
                                break;
                            case "RT":	// RichText 셀 설정
                                baseGrid.Sheets[0].Columns[i].CellType = new RichTextCellType();
                                break;
                            case "SC":	// SliderCell 셀 설정
                                baseGrid.Sheets[0].Columns[i].CellType = new SliderCellType();
                                break;
                            case "NL":	// 필수입력 사항 체크하지 않는 로직을 탄다
                                baseGrid.Sheets[0].Columns[i].CellType = new TextCellType();
                                baseGrid.Sheets[0].Columns[i].CanFocus = false;
                                if (shtCellType[i].ToString() == "NLV") // NLV인 경우 숨김
                                    baseGrid.ActiveSheet.Columns[i].Visible = false;
                                break;
                            case "PC":	// Percent 셀 설정
                                baseGrid.Sheets[0].Columns[i].CellType = new PercentCellType();
                                break;
                            case "CB":	// ComboBox 셀 설정
                                if (G1Etc[i] != null && G1Etc[i].Length > 0)
                                {
                                    ComboBoxCellType comboType = new ComboBoxCellType();
                                    comboType.MaxDrop = 20;

                                    Regex rx = new Regex("#");
                                    string Tmp = G1Etc[i].ToString();	//.Substring(G1Etc[k].IndexOf(":")+1, G1Etc[k].Length - G1Etc[k].IndexOf(":")-1);

                                    string Tmp1 = "";
                                    string Tmp2 = "";
                                    if (Tmp.IndexOf("|") > 0)
                                    {
                                        Tmp1 = Tmp.Substring(0, Tmp.IndexOf("|"));
                                        Tmp2 = Tmp.Substring(Tmp.IndexOf("|") + 1, Tmp.Length - Tmp.IndexOf("|") - 1);
                                        string[] cboMsg1 = rx.Split(Tmp1);
                                        string[] cboMsg2 = rx.Split(Tmp2);

                                        comboType.ItemData = cboMsg1;	// Key, Text
                                        comboType.Items = cboMsg2;		// Value
                                        comboType.EditorValue = FarPoint.Win.Spread.CellType.EditorValue.ItemData;

                                        baseGrid.Sheets[0].Columns[i].CellType = comboType;
                                    }
                                    else
                                    {
                                        string[] cboMsg1 = rx.Split(Tmp);

                                        comboType.Items = cboMsg1;
                                        baseGrid.Sheets[0].Columns[i].CellType = comboType;
                                    }
                                    if (shtCellType[i].ToString() == "CBV") // NLV인 경우 숨김
                                        baseGrid.ActiveSheet.Columns[i].Visible = false;

                                }
                                break;
                            case "ML": //TextBox MultiLine
                                FarPoint.Win.Spread.CellType.TextCellType MultiType = new FarPoint.Win.Spread.CellType.TextCellType();
                                MultiType.Multiline = true;
                                baseGrid.Sheets[0].Columns[i].CellType = MultiType;
                                break;
                            case "GN":	// General & 영문 대문자로 변환
                                baseGrid.Sheets[0].Columns[i].CellType = new TextCellType();
                                break;
                            default:	// General 설정
                                baseGrid.Sheets[0].Columns[i].CellType = new TextCellType();
                                FarPoint.Win.Spread.CellType.TextCellType textCellType1 = new FarPoint.Win.Spread.CellType.TextCellType();
                                textCellType1.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
                                baseGrid.Sheets[0].Columns.Get(i).CellType = textCellType1;
                                break;
                        }

                        for (int j = 0; j < baseGrid.Sheets[0].Rows.Count; j++)
                        {

                            switch (CellType)
                            {
                                case "BT":	// Button 셀 설정
                                    baseGrid.Sheets[0].Cells[j, i].CellType = btnType;
                                    break;
                                case "CK":	// CheckBox 셀 설정
                                    baseGrid.Sheets[0].Cells[j, i].CellType = new CheckBoxCellType();
                                    break;
                                case "CC":	// Currency 셀 설정
                                    baseGrid.Sheets[0].Cells[j, i].CellType = new CurrencyCellType();
                                    break;
                                case "DT":	// Date 셀 설정
                                    baseGrid.Sheets[0].Cells[j, i].CellType = new DateTimeCellType();
                                    break;
                                case "DD":	// Date 셀 설정(월콤보)                            
                                    FarPoint.Win.Spread.CellType.ComboBoxCellType cb = new FarPoint.Win.Spread.CellType.ComboBoxCellType();
                                    ListBox list = new ListBox();
                                    list.Items.AddRange(new Object[] { "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12" });
                                    cb.ListControl = list;
                                    cb.Items = (new String[] { "", "", "", "", "", "", "", "", "", "", "", "" });
                                    baseGrid.ActiveSheet.Columns[i].CellType = cb;
                                    break;
                                case "PW":	// Password 셀 설정
                                    FarPoint.Win.Spread.CellType.TextCellType passType = new FarPoint.Win.Spread.CellType.TextCellType();
                                    passType.PasswordChar = Convert.ToChar("*");
                                    baseGrid.Sheets[0].Cells[j, i].CellType = passType;
                                    break;
                                case "MK":	// MaskCellType 셀 설정 | Mask 사용예 : MK###-###-####
                                    FarPoint.Win.Spread.CellType.MaskCellType picType = new FarPoint.Win.Spread.CellType.MaskCellType();
                                    if (G1Etc[i].ToString().Length > 0)
                                        picType.Mask = G1Etc[i].ToString();	//shtCellType[i].Substring(2,shtCellType[i].Length-2).ToString();
                                    picType.MaskChar = Convert.ToChar("_");
                                    baseGrid.Sheets[0].Cells[j, i].CellType = picType;
                                    break;
                                case "NM":	// Number 셀 설정
                                    int Place = 0;
                                    if (shtCellType[i].Length == 3)
                                        Place = Convert.ToInt32(shtCellType[i].Substring(2, 1).ToString());
                                    FarPoint.Win.Spread.CellType.NumberCellType num = new FarPoint.Win.Spread.CellType.NumberCellType();
                                    //num.LeadingZero = FarPoint.Win.Spread.CellType.LeadingZero.No;
                                    num.DecimalSeparator = ".";
                                    num.DecimalPlaces = Place;
                                    num.FixedPoint = true;
                                    num.Separator = ",";
                                    num.ShowSeparator = true;
                                    num.MaximumValue = 99999999999999;
                                    num.MinimumValue = -99999999999999;
                                    baseGrid.Sheets[0].Cells[j, i].CellType = num;

                                    /* LeadingZero 를쓰면 소숫자리수가 표현되서 강제 코딩처리함 : 화면상 0값은 안보이게 처리함...   */
                                    if (Convert.ToDecimal(baseGrid.Sheets[0].Cells[j, i].Value) == 0)
                                    {
                                        baseGrid.Sheets[0].Cells[j, i].Value = null;
                                    }


                                    break;
                                case "HL":	// HyperLink 셀 설정
                                    baseGrid.Sheets[0].Cells[j, i].CellType = new HyperLinkCellType();
                                    break;
                                case "PG":	// Progress 셀 설정
                                    baseGrid.Sheets[0].Cells[j, i].CellType = new ProgressCellType();
                                    break;
                                case "RT":	// RichText 셀 설정
                                    baseGrid.Sheets[0].Cells[j, i].CellType = new RichTextCellType();
                                    break;
                                case "SC":	// SliderCell 셀 설정
                                    baseGrid.Sheets[0].Cells[j, i].CellType = new SliderCellType();
                                    break;
                                case "NL":	// 필수입력 사항 체크하지 않는 로직을 탄다
                                    baseGrid.Sheets[0].Cells[j, i].CellType = new TextCellType();
                                    baseGrid.Sheets[0].Cells[j, i].CanFocus = false;
                                    if (shtCellType[i].ToString() == "NLV") // NLV인 경우 숨김
                                        baseGrid.ActiveSheet.Columns[i].Visible = false;
                                    break;
                                case "PC":	// Percent 셀 설정
                                    baseGrid.Sheets[0].Cells[j, i].CellType = new PercentCellType();
                                    break;
                                case "CB":	// ComboBox 셀 설정
                                    if (G1Etc[i] != null && G1Etc[i].Length > 0)
                                    {
                                        ComboBoxCellType comboType = new ComboBoxCellType();
                                        comboType.MaxDrop = 20;

                                        Regex rx = new Regex("#");
                                        string Tmp = G1Etc[i].ToString();	//.Substring(G1Etc[k].IndexOf(":")+1, G1Etc[k].Length - G1Etc[k].IndexOf(":")-1);

                                        string Tmp1 = "";
                                        string Tmp2 = "";
                                        if (Tmp.IndexOf("|") > 0)
                                        {
                                            Tmp1 = Tmp.Substring(0, Tmp.IndexOf("|"));
                                            Tmp2 = Tmp.Substring(Tmp.IndexOf("|") + 1, Tmp.Length - Tmp.IndexOf("|") - 1);
                                            string[] cboMsg1 = rx.Split(Tmp1);
                                            string[] cboMsg2 = rx.Split(Tmp2);

                                            comboType.ItemData = cboMsg1;	// Key, Text
                                            comboType.Items = cboMsg2;		// Value
                                            comboType.EditorValue = FarPoint.Win.Spread.CellType.EditorValue.ItemData;

                                            baseGrid.Sheets[0].Cells[j, i].CellType = comboType;
                                        }
                                        else
                                        {
                                            string[] cboMsg1 = rx.Split(Tmp);

                                            comboType.Items = cboMsg1;
                                            baseGrid.Sheets[0].Cells[j, i].CellType = comboType;
                                        }
                                    }
                                    break;
                                case "ML": //TextBox MultiLine
                                    FarPoint.Win.Spread.CellType.TextCellType MultiType = new FarPoint.Win.Spread.CellType.TextCellType();
                                    MultiType.Multiline = true;

                                    baseGrid.Sheets[0].Columns[i].CellType = MultiType;
                                    break;
                                case "GN":	// General & 영문 대문자로 변환
                                    baseGrid.Sheets[0].Cells[j, i].CellType = new TextCellType();
                                    break;
                                default:	// General 설정
                                    baseGrid.Sheets[0].Cells[j, i].CellType = new TextCellType();
                                    FarPoint.Win.Spread.CellType.TextCellType textCellType1 = new FarPoint.Win.Spread.CellType.TextCellType();
                                    textCellType1.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
                                    baseGrid.Sheets[0].Columns.Get(i).CellType = textCellType1;
                                    break;
                            }
                        }
                    }
                    /****************************************************************************/

                    /********************** Column 정렬 설정 **************************************/
                    if (sheetAlign.Length > i)
                    {
                        switch (sheetAlign[i])
                        {
                            case "L":	// 왼쪽 정렬 MIDDLE
                                baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
                                baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
                                break;
                            case "LT":	// 왼쪽 정렬 TOP
                                baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
                                baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                                break;
                            case "LB":	// 왼쪽 정렬 BOTTOM
                                baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
                                baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Bottom;
                                break;
                            case "C": // 가운데 정렬
                                baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
                                baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
                                break;
                            case "CT":	// 가운데 정렬 TOP
                                baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
                                baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                                break;
                            case "CB":	// 가운데 정렬 BOTTOM
                                baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
                                baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Bottom;
                                break;
                            case "R":	// 오른쪽 정렬
                                baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;
                                baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
                                break;
                            case "RT":	// 오른쪽 정렬 TOP
                                baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;
                                baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                                break;
                            case "RB":	// 오른쪽 정렬 BOTTOM
                                baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;
                                baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Bottom;
                                break;
                            default:	// 디폴트 왼쪽 정렬
                                baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
                                baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
                                break;
                        }


                        for (int j = 0; j < baseGrid.Sheets[0].Rows.Count; j++)
                        {

                            switch (sheetAlign[i])
                            {
                                case "L":	// 왼쪽 정렬 MIDDLE
                                    baseGrid.Sheets[0].Cells[j, i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
                                    baseGrid.Sheets[0].Cells[j, i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
                                    break;
                                case "LT":	// 왼쪽 정렬 TOP
                                    baseGrid.Sheets[0].Cells[j, i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
                                    baseGrid.Sheets[0].Cells[j, i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                                    break;
                                case "LB":	// 왼쪽 정렬 BOTTOM
                                    baseGrid.Sheets[0].Cells[j, i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
                                    baseGrid.Sheets[0].Cells[j, i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Bottom;
                                    break;
                                case "C": // 가운데 정렬
                                    baseGrid.Sheets[0].Cells[j, i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
                                    baseGrid.Sheets[0].Cells[j, i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
                                    break;
                                case "CT":	// 가운데 정렬 TOP
                                    baseGrid.Sheets[0].Cells[j, i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
                                    baseGrid.Sheets[0].Cells[j, i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                                    break;
                                case "CB":	// 가운데 정렬 BOTTOM
                                    baseGrid.Sheets[0].Cells[j, i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
                                    baseGrid.Sheets[0].Cells[j, i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Bottom;
                                    break;
                                case "R":	// 오른쪽 정렬
                                    baseGrid.Sheets[0].Cells[j, i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;
                                    baseGrid.Sheets[0].Cells[j, i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
                                    break;
                                case "RT":	// 오른쪽 정렬 TOP
                                    baseGrid.Sheets[0].Cells[j, i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;
                                    baseGrid.Sheets[0].Cells[j, i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                                    break;
                                case "RB":	// 오른쪽 정렬 BOTTOM
                                    baseGrid.Sheets[0].Cells[j, i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;
                                    baseGrid.Sheets[0].Cells[j, i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Bottom;
                                    break;
                                default:	// 디폴트 왼쪽 정렬
                                    baseGrid.Sheets[0].Cells[j, i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
                                    baseGrid.Sheets[0].Cells[j, i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
                                    break;
                            }
                        }
                    }
                    /****************************************************************************/

                    /********************** Column 색상 및 Lock 설정 **************************************/
                    if (shtCellColor.Length > i)
                    {
                        switch (shtCellColor[i])
                        {
                            case 0:	// 일반
                                baseGrid.Sheets[0].Columns[i].BackColor = Color.White;
                                baseGrid.Sheets[0].Columns[i].Locked = false;
                                baseGrid.Sheets[0].Columns[i].CanFocus = true;
                                break;
                            case 1:	// 필수입력
                                baseGrid.Sheets[0].Columns[i].BackColor = SystemBase.Validation.Kind_LightCyan; //System.Drawing.Color.FromArgb(242, 252, 254);
                                baseGrid.Sheets[0].Columns[i].Locked = false;
                                baseGrid.Sheets[0].Columns[i].CanFocus = true;
                                break;
                            case 2:	// 읽기전용, 필수
                                baseGrid.Sheets[0].Columns[i].BackColor = System.Drawing.Color.FromArgb(239, 239, 239);
                                baseGrid.Sheets[0].Columns[i].Locked = true;
                                baseGrid.Sheets[0].Columns[i].CanFocus = true;
                                break;
                            case 3:	// 읽기전용, 필수항목에서 제외
                                baseGrid.Sheets[0].Columns[i].BackColor = System.Drawing.Color.FromArgb(238, 238, 238);
                                baseGrid.Sheets[0].Columns[i].Locked = true;
                                baseGrid.Sheets[0].Columns[i].CanFocus = true;
                                break;
                            case 4:	// 읽기전용, 팝업전용
                                baseGrid.Sheets[0].Columns[i].BackColor = Color.White;
                                baseGrid.Sheets[0].Columns[i].Locked = true;
                                baseGrid.Sheets[0].Columns[i].CanFocus = true;
                                break;
                            case 5:	// 읽기전용, 필수항목에서 제외, Focus 제외
                                baseGrid.Sheets[0].Columns[i].BackColor = System.Drawing.Color.FromArgb(238, 238, 238);
                                baseGrid.Sheets[0].Columns[i].Locked = true;
                                baseGrid.Sheets[0].Columns[i].CanFocus = false;
                                break;
                            case 6:	// 읽기전용, 필수항목, Focus 제외
                                baseGrid.Sheets[0].Columns[i].BackColor = System.Drawing.Color.FromArgb(239, 239, 239);
                                baseGrid.Sheets[0].Columns[i].Locked = true;
                                baseGrid.Sheets[0].Columns[i].CanFocus = false;
                                break;
                            default:	// 일반
                                baseGrid.Sheets[0].Columns[i].BackColor = Color.White;
                                baseGrid.Sheets[0].Columns[i].Locked = false;
                                baseGrid.Sheets[0].Columns[i].CanFocus = true;
                                break;
                        }

                        for (int j = 0; j < baseGrid.Sheets[0].Rows.Count; j++)
                        {
                            switch (shtCellColor[i])
                            {
                                case 0:	// 일반
                                    baseGrid.Sheets[0].Cells[j, i].BackColor = Color.White;
                                    baseGrid.Sheets[0].Cells[j, i].Locked = false;
                                    baseGrid.Sheets[0].Cells[j, i].CanFocus = true;
                                    break;
                                case 1:	// 필수입력
                                    baseGrid.Sheets[0].Cells[j, i].BackColor = SystemBase.Validation.Kind_LightCyan; //System.Drawing.Color.FromArgb(242, 252, 254);
                                    baseGrid.Sheets[0].Cells[j, i].Locked = false;
                                    baseGrid.Sheets[0].Cells[j, i].CanFocus = true;
                                    break;
                                case 2:	// 읽기전용, 필수
                                    baseGrid.Sheets[0].Cells[j, i].BackColor = System.Drawing.Color.FromArgb(239, 239, 239);
                                    baseGrid.Sheets[0].Cells[j, i].Locked = true;
                                    baseGrid.Sheets[0].Cells[j, i].CanFocus = true;
                                    break;
                                case 3:	// 읽기전용, 필수항목에서 제외
                                    baseGrid.Sheets[0].Cells[j, i].BackColor = System.Drawing.Color.FromArgb(238, 238, 238);
                                    baseGrid.Sheets[0].Cells[j, i].Locked = true;
                                    baseGrid.Sheets[0].Cells[j, i].CanFocus = true;
                                    break;
                                case 4:	// 읽기전용, 팝업전용
                                    baseGrid.Sheets[0].Cells[j, i].BackColor = Color.White;
                                    baseGrid.Sheets[0].Cells[j, i].Locked = true;
                                    baseGrid.Sheets[0].Cells[j, i].CanFocus = true;
                                    break;
                                case 5:	// 읽기전용, 필수항목에서 제외, Focus 제외
                                    baseGrid.Sheets[0].Cells[j, i].BackColor = System.Drawing.Color.FromArgb(238, 238, 238);
                                    baseGrid.Sheets[0].Cells[j, i].Locked = true;
                                    baseGrid.Sheets[0].Cells[j, i].CanFocus = false;
                                    break;
                                case 6:	// 읽기전용, 필수항목, Focus 제외
                                    baseGrid.Sheets[0].Cells[j, i].BackColor = System.Drawing.Color.FromArgb(239, 239, 239);
                                    baseGrid.Sheets[0].Cells[j, i].Locked = true;
                                    baseGrid.Sheets[0].Cells[j, i].CanFocus = false;
                                    break;
                                default:	// 일반
                                    baseGrid.Sheets[0].Cells[j, i].BackColor = Color.White;
                                    baseGrid.Sheets[0].Cells[j, i].Locked = false;
                                    baseGrid.Sheets[0].Cells[j, i].CanFocus = true;
                                    break;
                            }
                        }
                    }
                    /********************** Column Type 설정 **************************************/

                    /********************** Summery 하단 고정합계 **************************************/
                    if (sheetTitle.Length > 0 && shtSummary == true && baseGrid.Sheets[0].Rows.Count > 0)
                    {
                        if (i == 1)
                        {
                            baseGrid.Sheets[0].FrozenTrailingRowCount = 1;	//하단 Column 1줄 고정

                            baseGrid.Sheets[0].RowHeader.Cells[sheetTitle.Length, 0].Text = "합계";
                            baseGrid.Sheets[0].Rows[sheetTitle.Length].BackColor = System.Drawing.Color.FromArgb(242, 244, 246); // .FromName("Beige");
                            baseGrid.Sheets[0].Rows[sheetTitle.Length].Locked = true;
                        }
                        FarPoint.Win.ComplexBorder complexBorder1 = new FarPoint.Win.ComplexBorder(new FarPoint.Win.ComplexBorderSide(FarPoint.Win.ComplexBorderSideStyle.None), new FarPoint.Win.ComplexBorderSide(FarPoint.Win.ComplexBorderSideStyle.ThinLine, System.Drawing.Color.FromArgb(((System.Byte)(100)), ((System.Byte)(100)), ((System.Byte)(100)))), new FarPoint.Win.ComplexBorderSide(FarPoint.Win.ComplexBorderSideStyle.None), new FarPoint.Win.ComplexBorderSide(FarPoint.Win.ComplexBorderSideStyle.None));
                        baseGrid.Sheets[0].Cells.Get(sheetTitle.Length, i).Border = complexBorder1;

                        if (baseGrid.Sheets[0].GetCellType(sheetTitle.Length, i).ToString() == "NumberCellType" ||
                            baseGrid.Sheets[0].GetCellType(sheetTitle.Length, i).ToString() == "PercentCellType")
                        {
                            string Str = IntToString(i);
                            string Area = Str + "1:" + Str + sheetTitle.Length.ToString();
                            Cell r = baseGrid.ActiveSheet.Cells[sheetTitle.Length, i];

                            r.Formula = "SUM(" + Area + ")";
                        }
                        else
                        {
                            baseGrid.Sheets[0].Cells[sheetTitle.Length, i].CellType = new TextCellType();
                        }
                    }
                    /****************************************************************************/

                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log("grdCommSheet (그리드 디자인 재정의 실패 )", f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY013"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                baseGrid.Sheets[0].Rows.Count = 0;
            }
        }
        #endregion

        #region grdReMakeCT - CellType 재정의
        public static void grdReMakeCT(FarPoint.Win.Spread.FpSpread baseGrid, string Kind)
        {	//그리드 속성 재정의
            try
            {
                if (Kind == null)
                    Kind = "";

                //****************************************************************************************************************
                //:버튼 설정
                //****************************************************************************************************************
                FarPoint.Win.Spread.CellType.ButtonCellType btnType = new FarPoint.Win.Spread.CellType.ButtonCellType();
                btnType.Text = "...";

                Regex rx = new Regex("#");
                string[] remake = rx.Split(Kind);

                for (int i = 0; i < remake.Length; i++)
                {
                    int Tmp1 = Convert.ToInt32(remake[i].Substring(0, remake[i].IndexOf("|")));
                    string Tmp2 = remake[i].Substring(remake[i].IndexOf("|") + 1, remake[i].Length - remake[i].IndexOf("|") - 1);

                    for (int j = 0; j < baseGrid.Sheets[0].Rows.Count; j++)
                    {
                        switch (Tmp2.Substring(0, 2))
                        {
                            case "BT":	// Button 셀 설정
                                baseGrid.Sheets[0].Cells[j, Tmp1].CellType = btnType;
                                break;
                            case "CK":	// CheckBox 셀 설정
                                baseGrid.Sheets[0].Cells[j, Tmp1].CellType = new CheckBoxCellType();
                                break;
                            case "DT":	// Date 셀 설정
                                baseGrid.Sheets[0].Cells[j, Tmp1].CellType = new DateTimeCellType();
                                break;
                            case "DM":	// Date 셀 설정(년월)                            
                                FarPoint.Win.Spread.CellType.DateTimeCellType UseDt = new FarPoint.Win.Spread.CellType.DateTimeCellType();
                                UseDt.DateTimeFormat = FarPoint.Win.Spread.CellType.DateTimeFormat.UserDefined;
                                UseDt.UserDefinedFormat = "yyyy/MM";
                                baseGrid.Sheets[0].Cells[j, Tmp1].CellType = UseDt;
                                break;
                            case "DY":	// Date 셀 설정(년)                            
                                FarPoint.Win.Spread.CellType.DateTimeCellType UseDy = new FarPoint.Win.Spread.CellType.DateTimeCellType();
                                UseDy.DateTimeFormat = FarPoint.Win.Spread.CellType.DateTimeFormat.UserDefined;
                                UseDy.UserDefinedFormat = "yyyy";
                                baseGrid.Sheets[0].Cells[j, Tmp1].CellType = UseDy;
                                break;
                            case "DD":	// Date 셀 설정(월)                            
                                FarPoint.Win.Spread.CellType.DateTimeCellType UseDd = new FarPoint.Win.Spread.CellType.DateTimeCellType();
                                UseDd.DateTimeFormat = FarPoint.Win.Spread.CellType.DateTimeFormat.UserDefined;
                                UseDd.UserDefinedFormat = "MM";
                                baseGrid.Sheets[0].Cells[j, Tmp1].CellType = UseDd;
                                break;
                            case "CC":	// Currency 셀 설정
                                baseGrid.Sheets[0].Cells[j, Tmp1].CellType = new CurrencyCellType();
                                break;
                            case "PW":	// Password 셀 설정
                                FarPoint.Win.Spread.CellType.TextCellType passType = new FarPoint.Win.Spread.CellType.TextCellType();
                                passType.PasswordChar = Convert.ToChar("*");
                                baseGrid.Sheets[0].Columns[i].CellType = passType;
                                break;
                            case "MK":	// MaskCellType 셀 설정
                                FarPoint.Win.Spread.CellType.MaskCellType picType = new FarPoint.Win.Spread.CellType.MaskCellType();
                                if (Tmp2.Length > 2)
                                    picType.Mask = Tmp2.Substring(2, Tmp2.Length - 2).ToString();
                                picType.MaskChar = Convert.ToChar("_");
                                baseGrid.Sheets[0].Cells[j, Tmp1].CellType = picType;
                                break;
                            case "NM":	// Number 셀 설정
                                int Place = 0;
                                if (Tmp2.Length == 3)
                                    Place = Convert.ToInt32(Tmp2.Substring(2, 1).ToString());

                                FarPoint.Win.Spread.CellType.NumberCellType num = new FarPoint.Win.Spread.CellType.NumberCellType();
                                //num.LeadingZero = FarPoint.Win.Spread.CellType.LeadingZero.No;
                                num.DecimalSeparator = ".";
                                num.DecimalPlaces = Place;
                                num.FixedPoint = true;
                                num.Separator = ",";
                                num.ShowSeparator = true;
                                num.MaximumValue = 99999999999999;
                                num.MinimumValue = -99999999999999;
                                baseGrid.Sheets[0].Cells[j, Tmp1].CellType = num;
                                break;
                            case "HL":	// HyperLink 셀 설정
                                baseGrid.Sheets[0].Cells[j, Tmp1].CellType = new HyperLinkCellType();
                                break;
                            case "PG":	// Progress 셀 설정
                                baseGrid.Sheets[0].Cells[j, Tmp1].CellType = new ProgressCellType();
                                break;
                            case "RT":	// RichText 셀 설정
                                baseGrid.Sheets[0].Cells[j, Tmp1].CellType = new RichTextCellType();
                                break;
                            case "SC":	// SliderCell 셀 설정
                                baseGrid.Sheets[0].Cells[j, Tmp1].CellType = new SliderCellType();
                                break;
                            case "PC":	// Percent 셀 설정
                                baseGrid.Sheets[0].Columns[i].CellType = new PercentCellType();
                                break;
                            case "NL":	// 필수입력 사항 체크하지 않는 로직을 탄다
                                baseGrid.Sheets[0].Cells[j, Tmp1].CellType = new GeneralCellType();
                                baseGrid.Sheets[0].Cells[j, Tmp1].CanFocus = false;
                                if (Tmp2.ToString() == "NLV") // NLV인 경우 숨김
                                    baseGrid.ActiveSheet.Columns[Tmp1].Visible = false;
                                else if (Tmp2.ToString() == "NLR") // NLV인 경우 숨김
                                {
                                    baseGrid.ActiveSheet.Columns[Tmp1].Visible = true;
                                    baseGrid.Sheets[0].Cells[j, Tmp1].CellType = new TextCellType();
                                }
                                break;
                            case "ML": //TextBox MultiLine
                                FarPoint.Win.Spread.CellType.TextCellType MultiType = new FarPoint.Win.Spread.CellType.TextCellType();
                                MultiType.Multiline = true;

                                baseGrid.Sheets[0].Columns[i].CellType = MultiType;
                                break;

                            default:	// General 설정
                                baseGrid.Sheets[0].Cells[j, Tmp1].CellType = new TextCellType();
                                break;
                        }
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log("grdCommSheet (Cell Type 재정의 실패 )", f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY015"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region Close Tab 그리드 데이타 조회 후 Frozen Row 또는 Colunm 정의
        public static void grdMakeSheet(FarPoint.Win.Spread.FpSpread baseGrid
            , DataTable dt
            , string[] sheetTitle
            , int[] shtTitleSpan
            , string[] sheetTitle2
            , string[] sheetAlign
            , int[] sheetWidth
            , string[] shtComboBoxMsg
            , int[] shtHeaderRowCount
            , string[] shtCellType
            , int[] shtCellColor
            , int shtFixR
            , int shtFixC
            , bool shtSummary
            )
        {
            try
            {
                /********************** 그리드 초기화 **************************************/
                baseGrid.Reset();
                //********************** Clipboard False***********************************/
                baseGrid.AutoClipboard = false;
                /********************** 그리드 스타일 정의 **************************************/
                baseGrid.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
                /********************** 스크롤바 정의 **************************************/
                baseGrid.HorizontalScrollBarPolicy = FarPoint.Win.Spread.ScrollBarPolicy.AsNeeded;
                /********************** 스크롤바 정의 **************************************/
                baseGrid.VerticalScrollBarPolicy = FarPoint.Win.Spread.ScrollBarPolicy.AsNeeded;
                /********************** 그리드 칼럼수 지정 **************************************/
                baseGrid.ActiveSheet.Columns.Count = sheetTitle.Length;
                /********************** 그리드 Row수 지정 **************************************/
                //baseGrid.ActiveSheet.Rows.Count = 0;
                /********************** 그리드 Row색상 지정 **************************************/
                baseGrid.Sheets[0].AlternatingRows[0].BackColor = Color.FromName("WhiteSmoke");
                /********************** 그리드 Head 높이 지정 **************************************/
                baseGrid.Sheets[0].ColumnHeader.Rows[0].Height = 28;
                /********************** 그리드 RowHeader 넓이 지정 *********************************/
                baseGrid.Sheets[0].RowHeader.Columns[0].Width = 32;
                /********************** 그리드 바탕색상 지정 **************************************/
                baseGrid.Sheets[0].GrayAreaBackColor = Color.FromName("white");
                /********************** 1Row 선택 기준(단일, 1Row별, 1Cell별) **************************************/
                baseGrid.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.Normal;				//.ExtendedSelect;
                /********************** ColumnHeader 수 지정**************************************/
                baseGrid.Sheets[0].ColumnHeader.RowCount = shtHeaderRowCount[0];
                /********************** HorizontalGridLine 가로 가이드라인 색상 지정**************************************/
                baseGrid.Sheets[0].HorizontalGridLine = new FarPoint.Win.Spread.GridLine(FarPoint.Win.Spread.GridLineType.Flat, Color.FromArgb(193, 201, 212));
                /********************** VerticalGridLine 세로 가이드라인 색상 지정**************************************/
                baseGrid.Sheets[0].VerticalGridLine = new FarPoint.Win.Spread.GridLine(FarPoint.Win.Spread.GridLineType.Flat, Color.FromArgb(193, 201, 212));
                /********************** 왼쪽 상단 코너 색상 지정**************************************/
                baseGrid.Sheets[0].SheetCornerStyle.BackColor = System.Drawing.Color.FromArgb(242, 244, 246); // .FromName("Beige");
                /********************** 첫번째 Head color지정**************************************/
                baseGrid.Sheets[0].ColumnHeader.Rows[0].BackColor = System.Drawing.Color.FromArgb(242, 244, 246); // .FromName("Beige");
                /********************** 두번째 Head color지정**************************************/
                if (baseGrid.Sheets[0].ColumnHeader.RowCount > 1)
                    baseGrid.Sheets[0].ColumnHeader.Rows[1].BackColor = System.Drawing.Color.FromArgb(242, 244, 246); // .FromName("Beige");
                /********************** 첫번째 Colum color 지정**************************************/
                baseGrid.Sheets[0].RowHeader.Columns[0].BackColor = System.Drawing.Color.FromArgb(242, 244, 246); // .FromName("Beige");
                /********************** 1번째 Cell(프라이머리) 키 숨김 지정**************************************/
                baseGrid.ActiveSheet.Columns[0].Visible = false;
                /********************** Clipboard 복사시 Head값 미포함**************************************/
                baseGrid.ClipboardOptions = FarPoint.Win.Spread.ClipboardOptions.NoHeaders;



                //DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);
                baseGrid.Sheets[0].DataSource = dt;
                if (dt.Rows.Count > 0 && shtSummary == true)
                    baseGrid.ActiveSheet.Rows.Count = dt.Rows.Count + 1;//그리드Row수 지정
                else
                    baseGrid.ActiveSheet.Rows.Count = dt.Rows.Count;//그리드Row수 지정

                //****************************************************************************************************************
                //: 고정 Column 또는 Row 설정
                //****************************************************************************************************************
                baseGrid.Sheets[0].FrozenColumnCount = shtFixC + 1;
                baseGrid.Sheets[0].FrozenRowCount = shtFixR;
                //****************************************************************************************************************
                //: 버튼 설정
                //****************************************************************************************************************
                FarPoint.Win.Spread.CellType.ButtonCellType btnType = new FarPoint.Win.Spread.CellType.ButtonCellType();
                btnType.Text = "...";

                //****************************************************************************************************************
                //: Key Enter시 Next Col로 Focus 이동
                //****************************************************************************************************************
                FarPoint.Win.Spread.InputMap im = baseGrid.GetInputMap(FarPoint.Win.Spread.InputMapMode.WhenFocused);
                im.Put(new FarPoint.Win.Spread.Keystroke(Keys.Enter, Keys.None), FarPoint.Win.Spread.SpreadActions.MoveToNextColumnWrap);


                int sheetLength = 0, shtTitleSpanTmp = 0, shtTitleSpanTmp2 = 0;
                sheetLength = sheetTitle.Length;

                if (sheetTitle.Length < sheetTitle2.Length)
                    sheetLength = sheetTitle2.Length;

                //--------------------------------------------------------------------------------------------------------------------------

                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    //****************************************************************************************************************
                    //: Column 폭 넓이 설정
                    //****************************************************************************************************************
                    if (sheetWidth.Length > i)
                        baseGrid.Sheets[0].Columns[i].Width = sheetWidth[i];

                    //****************************************************************************************************************
                    //: columnSpan 설정
                    //****************************************************************************************************************
                    if (shtTitleSpanTmp == i && shtTitleSpan.Length > i)
                    {
                        baseGrid.Sheets[0].ColumnHeader.Cells[0, i].ColumnSpan = shtTitleSpan[shtTitleSpanTmp2];
                        shtTitleSpanTmp = shtTitleSpanTmp + shtTitleSpan[shtTitleSpanTmp2];
                        shtTitleSpanTmp2++;
                    }

                    //****************************************************************************************************************
                    //: sheet 1번째 Header Text 입력
                    //****************************************************************************************************************
                    if (sheetTitle.Length > i)
                        baseGrid.Sheets[0].ColumnHeader.Cells[0, i].Text = sheetTitle[i].ToString();

                    //****************************************************************************************************************
                    //: sheet 2번째 Header Text 입력
                    //****************************************************************************************************************
                    if (sheetTitle2.Length > i && baseGrid.Sheets[0].ColumnHeader.RowCount > 1)
                        baseGrid.Sheets[0].ColumnHeader.Cells[1, i].Text = sheetTitle2[i].ToString();

                    /********************** Column Type 설정 **************************************/
                    if (shtCellType.Length > i)
                    {
                        string CellType = "";
                        if (shtCellType[i].Length > 2)
                            CellType = shtCellType[i].Substring(0, 2);
                        else
                            CellType = shtCellType[i];

                        switch (CellType)
                        {
                            case "BT":	// Button 셀 설정
                                baseGrid.Sheets[0].Columns[i].CellType = btnType;
                                break;
                            case "CK":	// CheckBox 셀 설정
                                baseGrid.Sheets[0].Columns[i].CellType = new CheckBoxCellType();
                                break;
                            case "CC":	// Currency 셀 설정
                                baseGrid.Sheets[0].Columns[i].CellType = new CurrencyCellType();
                                break;
                            case "DT":	// Date 셀 설정
                                baseGrid.Sheets[0].Columns[i].CellType = new DateTimeCellType();
                                break;
                            case "DM":	// Date 셀 설정(년월)                            
                                FarPoint.Win.Spread.CellType.DateTimeCellType UseDt = new FarPoint.Win.Spread.CellType.DateTimeCellType();
                                UseDt.DateTimeFormat = FarPoint.Win.Spread.CellType.DateTimeFormat.UserDefined;
                                UseDt.UserDefinedFormat = "yyyy/MM";
                                baseGrid.Sheets[0].Columns[i].CellType = UseDt;
                                break;
                            case "DY":	// Date 셀 설정(년)                            
                                FarPoint.Win.Spread.CellType.DateTimeCellType UseDy = new FarPoint.Win.Spread.CellType.DateTimeCellType();
                                UseDy.DateTimeFormat = FarPoint.Win.Spread.CellType.DateTimeFormat.UserDefined;
                                UseDy.UserDefinedFormat = "yyyy";
                                baseGrid.Sheets[0].Columns[i].CellType = UseDy;
                                break;
                            case "DD":	// Date 셀 설정(월)                            
                                FarPoint.Win.Spread.CellType.DateTimeCellType UseDd = new FarPoint.Win.Spread.CellType.DateTimeCellType();
                                UseDd.DateTimeFormat = FarPoint.Win.Spread.CellType.DateTimeFormat.UserDefined;
                                UseDd.UserDefinedFormat = "MM";
                                baseGrid.Sheets[0].Columns[i].CellType = UseDd;
                                break;
                            case "PW":	// Password 셀 설정
                                FarPoint.Win.Spread.CellType.TextCellType passType = new FarPoint.Win.Spread.CellType.TextCellType();
                                passType.PasswordChar = Convert.ToChar("*");
                                baseGrid.Sheets[0].Columns[i].CellType = passType;
                                break;
                            case "MK":	// MaskCellType 셀 설정 | Mask 사용예 : MK###-###-####
                                FarPoint.Win.Spread.CellType.MaskCellType picType = new FarPoint.Win.Spread.CellType.MaskCellType();
                                if (shtCellType[i].Length > 2)
                                    picType.Mask = shtCellType[i].Substring(2, shtCellType[i].Length - 2).ToString();
                                picType.MaskChar = Convert.ToChar("_");
                                baseGrid.Sheets[0].Columns[i].CellType = picType;
                                break;
                            case "NM":	// Number 셀 설정
                                int Place = 0;
                                if (shtCellType[i].Length == 3)
                                    Place = Convert.ToInt32(shtCellType[i].Substring(2, 1).ToString());
                                FarPoint.Win.Spread.CellType.NumberCellType num = new FarPoint.Win.Spread.CellType.NumberCellType();
                                num.DecimalSeparator = ".";
                                num.DecimalPlaces = Place;
                                num.FixedPoint = true;
                                num.Separator = ",";
                                num.ShowSeparator = true;
                                num.MaximumValue = 99999999999999;
                                num.MinimumValue = -99999999999999;

                                //num.LeadingZero = FarPoint.Win.Spread.CellType.LeadingZero.No;

                                baseGrid.Sheets[0].Columns[i].CellType = num;
                                break;
                            case "HL":	// HyperLink 셀 설정
                                baseGrid.Sheets[0].Columns[i].CellType = new HyperLinkCellType();
                                break;
                            case "PG":	// Progress 셀 설정
                                baseGrid.Sheets[0].Columns[i].CellType = new ProgressCellType();
                                break;
                            case "RT":	// RichText 셀 설정
                                baseGrid.Sheets[0].Columns[i].CellType = new RichTextCellType();
                                break;
                            case "SC":	// SliderCell 셀 설정
                                baseGrid.Sheets[0].Columns[i].CellType = new SliderCellType();
                                break;
                            case "NL":	// 필수입력 사항 체크하지 않는 로직을 탄다
                                baseGrid.Sheets[0].Columns[i].CellType = new GeneralCellType();
                                baseGrid.Sheets[0].Columns[i].CanFocus = false;
                                if (shtCellType[i].ToString() == "NLV") // NLV인 경우 숨김
                                    baseGrid.ActiveSheet.Columns[i].Visible = false;
                                break;
                            case "PC":	// Percent 셀 설정
                                baseGrid.Sheets[0].Columns[i].CellType = new PercentCellType();
                                break;
                            case "CB":	// ComboBox 셀 설정
                                for (int k = 0; k < shtComboBoxMsg.Length; k++)
                                {
                                    if (i == Convert.ToInt32(shtComboBoxMsg[k].Substring(0, shtComboBoxMsg[k].IndexOf(":"))))
                                    {
                                        ComboBoxCellType comboType = new ComboBoxCellType();
                                        comboType.MaxDrop = 20;

                                        Regex rx = new Regex("#");
                                        string Tmp = shtComboBoxMsg[k].Substring(shtComboBoxMsg[k].IndexOf(":") + 1, shtComboBoxMsg[k].Length - shtComboBoxMsg[k].IndexOf(":") - 1);

                                        string Tmp1 = "";
                                        string Tmp2 = "";
                                        if (Tmp.IndexOf("|") > 0)
                                        {
                                            Tmp1 = Tmp.Substring(0, Tmp.IndexOf("|"));
                                            Tmp2 = Tmp.Substring(Tmp.IndexOf("|") + 1, Tmp.Length - Tmp.IndexOf("|") - 1);
                                            string[] cboMsg1 = rx.Split(Tmp1);
                                            string[] cboMsg2 = rx.Split(Tmp2);

                                            comboType.ItemData = cboMsg1;		// Value
                                            comboType.Items = cboMsg2;	// Key, Text
                                            comboType.EditorValue = FarPoint.Win.Spread.CellType.EditorValue.ItemData;

                                            baseGrid.Sheets[0].Columns[i].CellType = comboType;
                                        }
                                        else
                                        {
                                            string[] cboMsg1 = rx.Split(Tmp);

                                            comboType.Items = cboMsg1;
                                            baseGrid.Sheets[0].Columns[i].CellType = comboType;
                                        }
                                    }
                                }
                                break;
                            case "ML": //TextBox MultiLine
                                FarPoint.Win.Spread.CellType.TextCellType MultiType = new FarPoint.Win.Spread.CellType.TextCellType();
                                MultiType.Multiline = true;

                                baseGrid.Sheets[0].Columns[i].CellType = MultiType;
                                break;
                            default:	// General 설정
                                baseGrid.Sheets[0].Columns[i].CellType = new TextCellType();
                                break;
                        }
                    }
                    /****************************************************************************/

                    /********************** Column 정렬 설정 **************************************/
                    if (sheetAlign.Length > i)
                    {
                        switch (sheetAlign[i])
                        {
                            case "L":	// 왼쪽 정렬 MIDDLE
                                baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
                                baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
                                break;
                            case "LT":	// 왼쪽 정렬 TOP
                                baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
                                baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                                break;
                            case "LB":	// 왼쪽 정렬 BOTTOM
                                baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
                                baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Bottom;
                                break;
                            case "C": // 가운데 정렬
                                baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
                                baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
                                break;
                            case "CT":	// 가운데 정렬 TOP
                                baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
                                baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                                break;
                            case "CB":	// 가운데 정렬 BOTTOM
                                baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
                                baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Bottom;
                                break;
                            case "R":	// 오른쪽 정렬
                                baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;
                                baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
                                break;
                            case "RT":	// 오른쪽 정렬 TOP
                                baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;
                                baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                                break;
                            case "RB":	// 오른쪽 정렬 BOTTOM
                                baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;
                                baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Bottom;
                                break;
                            default:	// 디폴트 왼쪽 정렬
                                baseGrid.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
                                baseGrid.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
                                break;
                        }
                    }
                    /****************************************************************************/

                    /********************** Column 색상 및 Lock 설정 **************************************/
                    if (shtCellColor.Length > i)
                    {
                        switch (shtCellColor[i])
                        {
                            case 0:	// 일반
                                baseGrid.Sheets[0].Columns[i].BackColor = Color.White;
                                break;
                            case 1:	// 필수입력
                                baseGrid.Sheets[0].Columns[i].BackColor = SystemBase.Validation.Kind_LightCyan; //System.Drawing.Color.FromArgb(242, 252, 254);
                                break;
                            case 2:	// 읽기전용, 필수
                                baseGrid.Sheets[0].Columns[i].BackColor = System.Drawing.Color.FromArgb(239, 239, 239);
                                baseGrid.Sheets[0].Columns[i].Locked = true;
                                break;
                            case 3:	// 읽기전용, 필수항목에서 제외
                                baseGrid.Sheets[0].Columns[i].BackColor = System.Drawing.Color.FromArgb(238, 238, 238);
                                baseGrid.Sheets[0].Columns[i].Locked = true;
                                break;
                            case 4:	// 읽기전용, 팝업전용
                                baseGrid.Sheets[0].Columns[i].BackColor = Color.White;
                                baseGrid.Sheets[0].Columns[i].Locked = true;
                                break;
                            case 5:	// 읽기전용, 필수항목에서 제외, Focus 제외
                                baseGrid.Sheets[0].Columns[i].BackColor = System.Drawing.Color.FromArgb(238, 238, 238);
                                baseGrid.Sheets[0].Columns[i].Locked = true;
                                baseGrid.Sheets[0].Columns[i].CanFocus = false;
                                break;
                            case 6:	// 읽기전용, 필수항목, Focus 제외
                                baseGrid.Sheets[0].Columns[i].BackColor = System.Drawing.Color.FromArgb(239, 239, 239);
                                baseGrid.Sheets[0].Columns[i].Locked = true;
                                baseGrid.Sheets[0].Columns[i].CanFocus = false;
                                break;
                            default:	// 일반
                                baseGrid.Sheets[0].Columns[i].BackColor = Color.White;
                                break;
                        }
                    }
                    /********************** Column Type 설정 **************************************/

                    /********************** Summery 하단 고정합계 **************************************/
                    if (dt.Rows.Count > 0 && shtSummary == true)
                    {
                        if (i == 1)
                        {
                            baseGrid.Sheets[0].FrozenTrailingRowCount = 1;	//하단 Column 1줄 고정

                            baseGrid.Sheets[0].RowHeader.Cells[dt.Rows.Count, 0].Text = "SUM";
                            baseGrid.Sheets[0].Rows[dt.Rows.Count].BackColor = Color.NavajoWhite;
                            baseGrid.Sheets[0].Rows[dt.Rows.Count].Locked = true;
                        }
                        FarPoint.Win.ComplexBorder complexBorder1 = new FarPoint.Win.ComplexBorder(new FarPoint.Win.ComplexBorderSide(FarPoint.Win.ComplexBorderSideStyle.None), new FarPoint.Win.ComplexBorderSide(FarPoint.Win.ComplexBorderSideStyle.ThinLine, System.Drawing.Color.FromArgb(((System.Byte)(100)), ((System.Byte)(100)), ((System.Byte)(100)))), new FarPoint.Win.ComplexBorderSide(FarPoint.Win.ComplexBorderSideStyle.None), new FarPoint.Win.ComplexBorderSide(FarPoint.Win.ComplexBorderSideStyle.None));
                        baseGrid.Sheets[0].Cells.Get(dt.Rows.Count, i).Border = complexBorder1;

                        if (baseGrid.Sheets[0].GetCellType(dt.Rows.Count, i).ToString() == "NumberCellType" ||
                            baseGrid.Sheets[0].GetCellType(dt.Rows.Count, i).ToString() == "PercentCellType")
                        {
                            string Str = IntToString(i);
                            string Area = Str + "1:" + Str + dt.Rows.Count.ToString();
                            Cell r = baseGrid.ActiveSheet.Cells[dt.Rows.Count, i];

                            r.Formula = "SUM(" + Area + ")";
                        }
                        else
                        {
                            baseGrid.Sheets[0].Cells[dt.Rows.Count, i].CellType = new TextCellType();
                        }
                    }
                    /****************************************************************************/

                }
            }
            catch (Exception e)
            {
                SystemBase.Loggers.Log("grdMakeSheet (그리드 상단 데이타 조회 에러)", e.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY013"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                baseGrid.Sheets[0].Rows.Count = 0;
            }
        }
        #endregion

        #region DropDownString - 그리드 상단 콤보박스에 데이타 넣기
        public static string DropDownString(string Query, string Where, int NullValue)
        {	//                              쿼리,		  Return 위치   0:Null 1:전체 else:없음
            string Rtn;
            string RtnTmp1 = "";
            string RtnTmp2 = "";
            try
            {

                DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);

                if (NullValue == 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        RtnTmp1 = RtnTmp1 + "#" + dt.Rows[i][0].ToString();
                        RtnTmp2 = RtnTmp2 + "#" + dt.Rows[i][1].ToString();
                    }
                }
                else if (NullValue == 1)
                {
                    RtnTmp1 = "전체";
                    RtnTmp2 = "";

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        RtnTmp1 = RtnTmp1 + "#" + dt.Rows[i][0].ToString();
                        RtnTmp2 = RtnTmp2 + "#" + dt.Rows[i][1].ToString();
                    }
                }
                else
                {
                    RtnTmp1 = "";
                    RtnTmp2 = "";

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        RtnTmp1 = RtnTmp1 + "#" + dt.Rows[i][0].ToString();
                        RtnTmp2 = RtnTmp2 + "#" + dt.Rows[i][1].ToString();
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log("DropDownString (그리드 상단 콤보박스에 데이타 생성 에러)", f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY016"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return Rtn = Where + ":" + RtnTmp1 + "|" + RtnTmp2;
        }
        #endregion

        #region IntToString - 숫자를 칼럼명으로 변환하여 SUMMARY 필드명 가공
        public static string IntToString(int Ints)
        {
            string Rtn = "";
            switch (Ints)
            {
                case 0:
                    Rtn = "A";
                    break;
                case 1:
                    Rtn = "B";
                    break;
                case 2:
                    Rtn = "C";
                    break;
                case 3:
                    Rtn = "D";
                    break;
                case 4:
                    Rtn = "E";
                    break;
                case 5:
                    Rtn = "F";
                    break;
                case 6:
                    Rtn = "G";
                    break;
                case 7:
                    Rtn = "H";
                    break;
                case 8:
                    Rtn = "I";
                    break;
                case 9:
                    Rtn = "J";
                    break;
                case 10:
                    Rtn = "K";
                    break;
                case 11:
                    Rtn = "L";
                    break;
                case 12:
                    Rtn = "M";
                    break;
                case 13:
                    Rtn = "N";
                    break;
                case 14:
                    Rtn = "O";
                    break;
                case 15:
                    Rtn = "P";
                    break;
                case 16:
                    Rtn = "Q";
                    break;
                case 17:
                    Rtn = "R";
                    break;
                case 18:
                    Rtn = "S";
                    break;
                case 19:
                    Rtn = "T";
                    break;
                case 20:
                    Rtn = "U";
                    break;
                case 21:
                    Rtn = "V";
                    break;
                case 22:
                    Rtn = "W";
                    break;
                case 23:
                    Rtn = "X";
                    break;
                case 24:
                    Rtn = "Y";
                    break;
                case 25:
                    Rtn = "Z";
                    break;
                case 26:
                    Rtn = "AA";
                    break;
                case 27:
                    Rtn = "AB";
                    break;
                case 28:
                    Rtn = "AC";
                    break;
                case 29:
                    Rtn = "AD";
                    break;
                case 30:
                    Rtn = "AE";
                    break;
                case 31:
                    Rtn = "AF";
                    break;
                case 32:
                    Rtn = "AG";
                    break;
                case 33:
                    Rtn = "AH";
                    break;
                case 34:
                    Rtn = "AI";
                    break;
                case 35:
                    Rtn = "AJ";
                    break;
                case 36:
                    Rtn = "AK";
                    break;
                case 37:
                    Rtn = "AL";
                    break;
                case 38:
                    Rtn = "AM";
                    break;
                case 39:
                    Rtn = "AN";
                    break;
                case 40:
                    Rtn = "AO";
                    break;
                case 41:
                    Rtn = "AP";
                    break;
                case 42:
                    Rtn = "AQ";
                    break;
                case 43:
                    Rtn = "AR";
                    break;
                case 44:
                    Rtn = "AS";
                    break;
                case 45:
                    Rtn = "AT";
                    break;
                case 46:
                    Rtn = "AU";
                    break;
                case 47:
                    Rtn = "AV";
                    break;
                case 48:
                    Rtn = "AW";
                    break;
                case 49:
                    Rtn = "AX";
                    break;
                case 50:
                    Rtn = "AY";
                    break;
                case 51:
                    Rtn = "AZ";
                    break;
                case 52:
                    Rtn = "BA";
                    break;
                case 53:
                    Rtn = "BB";
                    break;
                case 54:
                    Rtn = "BC";
                    break;
                case 55:
                    Rtn = "BD";
                    break;
                case 56:
                    Rtn = "BE";
                    break;
                case 57:
                    Rtn = "BF";
                    break;
                case 58:
                    Rtn = "BG";
                    break;
                case 59:
                    Rtn = "BH";
                    break;
                case 60:
                    Rtn = "BI";
                    break;
                default:
                    Rtn = "BJ";
                    break;
            }
            return Rtn;
        }
        #endregion

        #region RowInsert - 그리드 Row 신규 추가
        public static void RowInsert(FarPoint.Win.Spread.FpSpread fpSpread1)
        {
            try
            {
                if (fpSpread1.Sheets[0].FrozenTrailingRowCount > 0 && fpSpread1.Sheets[0].Rows.Count == 0)
                    fpSpread1.Sheets[0].FrozenTrailingRowCount = 0;

                int TmpRow = 0;
                int TmpColunm = 0;
                if (fpSpread1.Sheets[0].Rows.Count > 0)
                {
                    if (fpSpread1.ActiveSheet.GetSelection(0) == null)
                    {
                        TmpRow = fpSpread1.ActiveSheet.ActiveRowIndex;
                        TmpColunm = fpSpread1.ActiveSheet.ActiveRowIndex;
                    }
                    else
                    {
                        TmpRow = fpSpread1.ActiveSheet.GetSelection(0).Row;
                        TmpColunm = fpSpread1.ActiveSheet.GetSelection(0).Column;
                    }

                    fpSpread1.Sheets[0].Rows.Add(TmpRow + 1, 1);	//Row 추가

                    fpSpread1.Sheets[0].RowHeader.Cells[TmpRow + 1, 0].Text = "I";	//Row Head에 I자 업데이트

                    fpSpread1.Sheets[0].RowHeader.Rows[TmpRow + 1].BackColor = SystemBase.Base.Color_Insert;

                    for (int i = 0; i < fpSpread1.Sheets[0].Columns.Count; i++)
                    {	// Column 신규 추가시 Lock 해제후 바탕색상 변경 로직

                        // Column 신규 추가시 Columns 속성을 Cells 속성에 대입
                        fpSpread1.Sheets[0].Cells[TmpRow + 1, i].BackColor = fpSpread1.Sheets[0].Columns[i].BackColor;
                        fpSpread1.Sheets[0].Cells[TmpRow + 1, i].Locked = fpSpread1.Sheets[0].Columns[i].Locked;
                        fpSpread1.Sheets[0].Cells[TmpRow + 1, i].CanFocus = fpSpread1.Sheets[0].Columns[i].CanFocus;
                        fpSpread1.Sheets[0].Cells[TmpRow + 1, i].HorizontalAlignment = fpSpread1.Sheets[0].Columns[i].HorizontalAlignment;
                        fpSpread1.Sheets[0].Cells[TmpRow + 1, i].VerticalAlignment = fpSpread1.Sheets[0].Columns[i].VerticalAlignment;
                        fpSpread1.Sheets[0].Cells[TmpRow + 1, i].CellType = fpSpread1.Sheets[0].Columns[i].CellType;

                        if (fpSpread1.Sheets[0].GetCellType(TmpRow + 1, i).ToString() != "GeneralCellType")
                        {	// NL  GeneralCellType이 아닌 경우
                            if (fpSpread1.Sheets[0].GetCellType(TmpRow + 1, i).ToString() == "ButtonCellType")
                            {	// Button인 경우 Lock 해제
                                fpSpread1.Sheets[0].Cells[TmpRow + 1, i].Locked = false;
                                fpSpread1.Sheets[0].Cells[TmpRow + 1, i].BackColor = SystemBase.Validation.Kind_LightCyan; //System.Drawing.Color.FromArgb(242, 252, 254);
                            }
                            else
                            {	// Button이 아닌 경우도 Lock 해제
                                if (fpSpread1.Sheets[0].Columns[i].BackColor == System.Drawing.Color.FromArgb(239, 239, 239))
                                {	// 바탕색 속성이 ReadOnly인 경우 필수입력 색상으로 변경
                                    fpSpread1.Sheets[0].Cells[TmpRow + 1, i].BackColor = SystemBase.Validation.Kind_LightCyan; //System.Drawing.Color.FromArgb(242, 252, 254);
                                    fpSpread1.Sheets[0].Cells[TmpRow + 1, i].Locked = false;
                                }
                                else if (fpSpread1.Sheets[0].Columns[i].BackColor == System.Drawing.Color.FromArgb(238, 238, 238))
                                {	// 속성이 3인 경우 Lock true
                                    //fpSpread1.Sheets[0].Cells[TmpRow, i].BackColor = System.Drawing.Color.FromArgb(242, 252, 254);
                                    fpSpread1.Sheets[0].Cells[TmpRow + 1, i].Locked = true;
                                }
                                else
                                {
                                    fpSpread1.Sheets[0].Cells[TmpRow + 1, i].Locked = false;
                                }
                            }
                        }
                    }
                    fpSpread1.ActiveSheet.SetActiveCell(TmpRow + 1, TmpColunm);
                    fpSpread1.ActiveSheet.AddSelection(TmpRow + 1, TmpColunm, 1, 1);
                    fpSpread1.ShowActiveCell(FarPoint.Win.Spread.VerticalPosition.Nearest, FarPoint.Win.Spread.HorizontalPosition.Nearest);
                }
                else
                {
                    fpSpread1.Sheets[0].Rows.Add(0, 1);
                    fpSpread1.Sheets[0].RowHeader.Cells[0, 0].Text = "I";
                    fpSpread1.Sheets[0].RowHeader.Rows[0].BackColor = SystemBase.Base.Color_Insert;

                    for (int i = 0; i < fpSpread1.Sheets[0].ColumnHeader.Columns.Count; i++)
                    {	// Column 신규 추가시 Lock 해제후 바탕색상 변경 로직
                        // Column 신규 추가시 Columns 속성을 Cells 속성에 대입
                        fpSpread1.Sheets[0].Cells[0, i].BackColor = fpSpread1.Sheets[0].Columns[i].BackColor;
                        fpSpread1.Sheets[0].Cells[0, i].Locked = fpSpread1.Sheets[0].Columns[i].Locked;
                        fpSpread1.Sheets[0].Cells[0, i].CanFocus = fpSpread1.Sheets[0].Columns[i].CanFocus;
                        fpSpread1.Sheets[0].Cells[0, i].HorizontalAlignment = fpSpread1.Sheets[0].Columns[i].HorizontalAlignment;
                        fpSpread1.Sheets[0].Cells[0, i].VerticalAlignment = fpSpread1.Sheets[0].Columns[i].VerticalAlignment;
                        fpSpread1.Sheets[0].Cells[0, i].CellType = fpSpread1.Sheets[0].Columns[i].CellType;

                        if (fpSpread1.Sheets[0].GetCellType(0, i).ToString() != "GeneralCellType")
                        {	// NL  GeneralCellType이 아닌 경우
                            if (fpSpread1.Sheets[0].GetCellType(0, i).ToString() == "ButtonCellType")
                            {	// Button인 경우 Lock 해제
                                fpSpread1.Sheets[0].Cells[0, i].Locked = false;
                                fpSpread1.Sheets[0].Cells[0, i].BackColor = SystemBase.Validation.Kind_LightCyan; //System.Drawing.Color.FromArgb(242, 252, 254);
                            }
                            else
                            {	// Button이 아닌 경우도 Lock 해제
                                if (fpSpread1.Sheets[0].Columns[i].BackColor == System.Drawing.Color.FromArgb(239, 239, 239))
                                {	// 바탕색 속성이 ReadOnly인 경우 필수입력 색상으로 변경
                                    fpSpread1.Sheets[0].Cells[0, i].BackColor = SystemBase.Validation.Kind_LightCyan; //System.Drawing.Color.FromArgb(242, 252, 254);
                                    fpSpread1.Sheets[0].Cells[0, i].Locked = false;
                                }
                                else if (fpSpread1.Sheets[0].Columns[i].BackColor == System.Drawing.Color.FromArgb(238, 238, 238))
                                {	// 속성이 3인 경우 Lock true
                                    fpSpread1.Sheets[0].Cells[0, i].Locked = true;
                                }
                                else
                                {
                                    fpSpread1.Sheets[0].Cells[0, i].Locked = false;
                                }
                            }
                        }
                    }
                    fpSpread1.ActiveSheet.SetActiveCell(0, 1);
                    fpSpread1.ActiveSheet.AddSelection(0, 1, 1, 1);
                    fpSpread1.ShowActiveCell(FarPoint.Win.Spread.VerticalPosition.Nearest, FarPoint.Win.Spread.HorizontalPosition.Nearest);
                }
            }
            catch//(Exception e)
            {
                fpSpread1.Sheets[0].Rows.Add(0, 1);
                fpSpread1.Sheets[0].RowHeader.Cells[0, 0].Text = "I";
                for (int i = 0; i < fpSpread1.Sheets[0].ColumnHeader.Columns.Count; i++)
                {	// Column 신규 추가시 Lock 해제후 바탕색상 변경 로직
                    // Column 신규 추가시 Columns 속성을 Cells 속성에 대입
                    fpSpread1.Sheets[0].Cells[0, i].BackColor = fpSpread1.Sheets[0].Columns[i].BackColor;
                    fpSpread1.Sheets[0].Cells[0, i].Locked = fpSpread1.Sheets[0].Columns[i].Locked;
                    fpSpread1.Sheets[0].Cells[0, i].CanFocus = fpSpread1.Sheets[0].Columns[i].CanFocus;
                    fpSpread1.Sheets[0].Cells[0, i].HorizontalAlignment = fpSpread1.Sheets[0].Columns[i].HorizontalAlignment;
                    fpSpread1.Sheets[0].Cells[0, i].VerticalAlignment = fpSpread1.Sheets[0].Columns[i].VerticalAlignment;
                    fpSpread1.Sheets[0].Cells[0, i].CellType = fpSpread1.Sheets[0].Columns[i].CellType;

                    if (fpSpread1.Sheets[0].GetCellType(0, i).ToString() != "GeneralCellType")
                    {	// NL  GeneralCellType이 아닌 경우
                        if (fpSpread1.Sheets[0].GetCellType(0, i).ToString() == "ButtonCellType")
                        {	// Button인 경우 Lock 해제
                            fpSpread1.Sheets[0].Cells[0, i].Locked = false;
                            fpSpread1.Sheets[0].Cells[0, i].BackColor = SystemBase.Validation.Kind_LightCyan; //System.Drawing.Color.FromArgb(242, 252, 254);
                        }
                        else
                        {	// Button이 아닌 경우도 Lock 해제
                            if (fpSpread1.Sheets[0].Columns[i].BackColor == System.Drawing.Color.FromArgb(239, 239, 239))
                            {	// 바탕색 속성이 ReadOnly인 경우 필수입력 색상으로 변경
                                fpSpread1.Sheets[0].Cells[0, i].BackColor = SystemBase.Validation.Kind_LightCyan; //System.Drawing.Color.FromArgb(242, 252, 254);
                                fpSpread1.Sheets[0].Cells[0, i].Locked = false;
                            }
                            else if (fpSpread1.Sheets[0].Columns[i].BackColor == System.Drawing.Color.FromArgb(238, 238, 238))
                            {	// 속성이 3인 경우 Lock true
                                fpSpread1.Sheets[0].Cells[0, i].Locked = true;
                            }
                            else
                            {
                                fpSpread1.Sheets[0].Cells[0, i].Locked = false;
                            }
                        }
                    }
                }
                fpSpread1.ActiveSheet.SetActiveCell(0, 1);
                fpSpread1.ActiveSheet.AddSelection(0, 1, 1, 1);
                fpSpread1.ShowActiveCell(FarPoint.Win.Spread.VerticalPosition.Nearest, FarPoint.Win.Spread.HorizontalPosition.Nearest);
            }
        }
        #endregion

        #region FPUpCheck - 그리드 데이타 필수항목 Check
        public static bool FPUpCheck(FarPoint.Win.Spread.FpSpread fpSpread1)
        {
            bool ChkGrid = true;
            int UpCount = 0;
            try
            {
                for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                {	//필수입력사항 체크
                    if (fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text == "I" || fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text == "U" || fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text == "D")
                    {
                        for (int j = 0; j < fpSpread1.Sheets[0].Columns.Count; j++)
                        {
                            //							if(((fpSpread1.Sheets[0].Cells[i, j].BackColor == System.Drawing.Color.FromArgb(239, 239, 239)
                            //								|| fpSpread1.Sheets[0].Cells[i, j].BackColor == System.Drawing.Color.FromArgb(242, 252, 254))
                            if (((fpSpread1.Sheets[0].Columns[j].BackColor == System.Drawing.Color.FromArgb(239, 239, 239)
                                || fpSpread1.Sheets[0].Columns[j].BackColor == SystemBase.Validation.Kind_LightCyan //System.Drawing.Color.FromArgb(242, 252, 254)
                                || fpSpread1.Sheets[0].Cells[i, j].BackColor == System.Drawing.Color.FromArgb(239, 239, 239)
                                || fpSpread1.Sheets[0].Cells[i, j].BackColor == SystemBase.Validation.Kind_LightCyan) //System.Drawing.Color.FromArgb(242, 252, 254))

                                && (fpSpread1.Sheets[0].Cells[i, j].Value == null
                                || fpSpread1.Sheets[0].Cells[i, j].Text.Length == 0))
                                && fpSpread1.Sheets[0].GetCellType(i, j).ToString() != "GeneralCellType"
                                && fpSpread1.Sheets[0].GetCellType(i, j).ToString() != "ButtonCellType"
                                && fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text != "D"
                                )
                            {
                                MessageBox.Show(Convert.ToString(i + 1) + "번째 Row의 [ " + fpSpread1.Sheets[0].ColumnHeader.Cells[0, j].Text.ToString() + " ] 항목은 필수입력 항목입니다.", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                ChkGrid = false;
                                break;
                            }
                        }
                        UpCount++;
                    }
                    if (ChkGrid == false)
                        break;
                }

                if (UpCount == 0)
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn("SY017"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);//변경된 데이타가 없습니다.
                    ChkGrid = false;
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log("FPUpCheck 1 (그리드 필수항목 체크시 에러발생)", f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY018"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return ChkGrid;
        }
        #endregion

        #region FPUpCheck - 그리드 데이타 Check
        public static bool FPUpCheck(FarPoint.Win.Spread.FpSpread fpSpread1, bool EditCheck)
        {
            bool ChkGrid = true;
            int UpCount = 0;
            try
            {
                for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                {	//필수입력사항 체크
                    if (fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text == "I" || fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text == "U" || fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text == "D")
                    {
                        for (int j = 0; j < fpSpread1.Sheets[0].Columns.Count; j++)
                        {
                            //							if(((fpSpread1.Sheets[0].Cells[i, j].BackColor == System.Drawing.Color.FromArgb(239, 239, 239)
                            //								|| fpSpread1.Sheets[0].Cells[i, j].BackColor == System.Drawing.Color.FromArgb(242, 252, 254)) 
                            if (((fpSpread1.Sheets[0].Columns[j].BackColor == System.Drawing.Color.FromArgb(239, 239, 239)
                                || fpSpread1.Sheets[0].Columns[j].BackColor == SystemBase.Validation.Kind_LightCyan //System.Drawing.Color.FromArgb(242, 252, 254)
                                || fpSpread1.Sheets[0].Cells[i, j].BackColor == System.Drawing.Color.FromArgb(239, 239, 239)
                                || fpSpread1.Sheets[0].Cells[i, j].BackColor == SystemBase.Validation.Kind_LightCyan) //System.Drawing.Color.FromArgb(242, 252, 254))
                                && (fpSpread1.Sheets[0].Cells[i, j].Value == null
                                || fpSpread1.Sheets[0].Cells[i, j].Text.Length == 0))
                                && fpSpread1.Sheets[0].GetCellType(i, j).ToString() != "GeneralCellType"
                                && fpSpread1.Sheets[0].GetCellType(i, j).ToString() != "ButtonCellType"
                                && fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text != "D"
                                )
                            {
                                MessageBox.Show(Convert.ToString(i + 1) + "번째 Row의 [ " + fpSpread1.Sheets[0].ColumnHeader.Cells[0, j].Text.ToString() + " ] 항목은 필수입력 항목입니다.", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                ChkGrid = false;
                                break;
                            }
                        }
                        UpCount++;
                    }
                    if (ChkGrid == false)
                        break;
                }

                if (UpCount == 0 && EditCheck == true)
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn("SY017"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);//변경된 데이타가 없습니다.
                    ChkGrid = false;
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log("FPUpCheck 2 (그리드 필수항목 체크시 에러발생)", f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY018"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return ChkGrid;
        }
        #endregion

        #region ComboCloseUp - 콤보 선택 이벤트
        public static void ComboCloseUp(FarPoint.Win.Spread.FpSpread fpSpread1, string Query, string ReturnDT, int NowRow, int Focus)
        {//								그리드,                                 쿼리,         Return 위치,     선택한 Row, Focus위치
            try
            {
                Regex rx = new Regex("#");
                string[] ReturnDTs = rx.Split(ReturnDT);

                DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);

                for (int i = 0; i < ReturnDTs.Length; i++)
                {
                    fpSpread1.Sheets[0].Cells[NowRow, Convert.ToInt32(ReturnDTs[i].ToString())].Value = dt.Rows[0][i].ToString();
                    fpSpread1.ActiveSheet.SetActiveCell(NowRow, Focus);
                }
            }
            catch { }
        }
        #endregion

        #region EnterCell - 엔터시 발생되는 이벤트
        public static string[] EnterCell(FarPoint.Win.Spread.FpSpread fpSpread1, string strQuery, string[] strWhere, string[] strSearch, int Rows, int Colunms, int Focus, string RtnDTCol, string[] PHeadText, string[] PTxtAlign, string[] PCellType, int[] PHeadWidth, int[] PSearchLabel, int frmWidth)
        {//							                                 그리드,         쿼리,선택한 Row, 선택한 Col, Focus위치,     Return 위치,           팝업헤드,          팝업 정렬,             셀타입,        헤드넓이 , 팝업라벨 텍스트위치,창 넓이
            string[] RtnVal = null;
            try
            {
                //if (fpSpread1.Sheets[0].Cells[Rows, Colunms].BackColor.Name.ToString() != "0" && fpSpread1.Sheets[0].Cells[Rows, Colunms].BackColor.Name.ToString() != "LightGray" && fpSpread1.Sheets[0].Cells[Rows, Colunms].BackColor.Name.ToString() != "Gainsboro")
                if (fpSpread1.Sheets[0].Cells[Rows, Colunms].BackColor.Name.ToString() != "0" && fpSpread1.Sheets[0].Cells[Rows, Colunms].BackColor != System.Drawing.Color.FromArgb(238, 238, 238) && fpSpread1.Sheets[0].Cells[Rows, Colunms].BackColor != System.Drawing.Color.FromArgb(239, 239, 239))
                {
                    if (fpSpread1.Sheets[0].GetCellType(Rows, Colunms).ToString() == "ButtonCellType")
                    {	// 버튼 Focus 후 KeyEnter시 팝업창 호출// && fpSpread1.Sheets[0].Cells[Rows,Colunms].BackColor != Color.Empty
                        RtnVal = PopupCommon(fpSpread1, strQuery, strWhere, strSearch, Rows, RtnDTCol, PHeadText, PTxtAlign, PCellType, PHeadWidth, PSearchLabel, frmWidth);
                        fpSpread1.ActiveSheet.SetActiveCell(Rows, Focus);
                        fpChange(fpSpread1, Rows);
                        //fpSpread1.ShowActiveCell(FarPoint.Win.Spread.VerticalPosition.Nearest, FarPoint.Win.Spread.HorizontalPosition.Nearest);
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log("EnterCell (엔터시 발생되는 이벤트 에러)", f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY019"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return RtnVal;
        }

        public static string[] PopupCommon(FarPoint.Win.Spread.FpSpread fpSpread1, string strQuery, string[] strWhere, string[] strSearch, int Rows, string RtnDTCol, string[] PHeadText, string[] PTxtAlign, string[] PCellType, int[] PHeadWidth, int[] PSearchLabel, int frmWidth)
        {
            string[] RtnVals = null;
            PopUpSP pu = new PopUpSP(strQuery, strWhere, strSearch, PHeadText, PTxtAlign, PCellType, PHeadWidth, PSearchLabel);
            pu.ClientSize = new System.Drawing.Size(frmWidth, 450);
            pu.ShowDialog();
            if (pu.DialogResult == DialogResult.OK)
            {
                string RtnVal = pu.ReturnVal.ToString();

                Regex rx = new Regex("#");
                RtnVals = rx.Split(RtnVal.Replace("|", "#"));

                Regex rx2 = new Regex("#");
                string[] RtnDTCols = rx2.Split(RtnDTCol);

                for (int i = 0; i < RtnDTCols.Length; i++)
                {
                    if (RtnDTCols[i].ToString().Length > 0)
                        fpSpread1.Sheets[0].Cells[Rows, Convert.ToInt32(RtnDTCols[i].ToString())].Value = RtnVals[i].ToString();
                }

            }
            return RtnVals;
        }

        #endregion

        #region FPUpdate - 그리드 데이타 저장, 수정, 삭제
        public static void FPUpdate(string SaveStr, FarPoint.Win.Spread.FpSpread fpSpread1)
        {
            try
            {
                if (SaveStr == null)
                    SaveStr = "";

                //UpdateCK = "";
                bool ChkGrid = true;
                int UpCount = 0;
                for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                {	//필수입력사항 체크
                    if (fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text == "I" || fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text == "U" || fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text == "D")
                    {
                        for (int j = 0; j < fpSpread1.Sheets[0].Columns.Count; j++)
                        {
                            if (((fpSpread1.Sheets[0].Cells[i, j].BackColor == System.Drawing.Color.FromArgb(239, 239, 239)
                                || fpSpread1.Sheets[0].Cells[i, j].BackColor == SystemBase.Validation.Kind_LightCyan) //System.Drawing.Color.FromArgb(242, 252, 254))
                                && (fpSpread1.Sheets[0].Cells[i, j].Value == null
                                || fpSpread1.Sheets[0].Cells[i, j].Value.ToString().Length == 0))
                                && fpSpread1.Sheets[0].GetCellType(i, j).ToString() != "GeneralCellType"
                                && fpSpread1.Sheets[0].GetCellType(i, j).ToString() != "ButtonCellType")
                            {//3번째 Row의 aaa 항목은 필수입력사항입니다.
                                MessageBox.Show(Convert.ToString(i + 1) + "번째 ROW의 [" + fpSpread1.Sheets[0].ColumnHeader.Cells[0, j].Text.ToString() + "] 항목은 필수입력 사항입니다.", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                ChkGrid = false;
                                return;
                            }
                        }
                        UpCount++;
                    }
                }

                if ((SaveStr.Length == 0 && UpCount == 0) || (SaveStr.Length > 0 && SaveStr.Substring(0, 1) != "@" && UpCount == 0))
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn("SY017"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);//변경된 데이타가 없습니다.
                    ChkGrid = false;
                    return;
                }

                if (ChkGrid == true)
                {
                    DataTable dt = new DataTable();

                    dt.Columns.Add("0");
                    for (int m = 0; m < fpSpread1.Sheets[0].Columns.Count; m++)
                    {
                        dt.Columns.Add(Convert.ToString(m + 1));
                    }
                    dt.Columns.Add(Convert.ToString(fpSpread1.Sheets[0].Columns.Count + 1));

                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {
                        if (fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text == "I" || fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text == "U" || fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text == "D")
                        {
                            DataRow dr = dt.NewRow();
                            dr[0] = fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text;
                            for (int j = 0; j < fpSpread1.Sheets[0].Columns.Count; j++)
                            {
                                try
                                {
                                    dr[j + 1] = fpSpread1.Sheets[0].Cells[i, j].Value.ToString();
                                    //MessageBox.Show(fpSpread1.Sheets[0].Cells[i, j].Value.ToString() + " | " +fpSpread1.Sheets[0].Cells[i, j].Text.ToString());
                                }
                                catch
                                {
                                    dr[j + 1] = "";
                                }
                            }
                            //dr[dt.Columns.Count-1] = memberID.ToString();

                            dt.Rows.Add(dr);
                        }
                    }
                    DataSet ds = new DataSet();
                    ds.Tables.Add(dt);

                    //					SmartClient.CWebServer._Default WDT = new SmartClient.CWebServer._Default();
                    //					string Msg = WDT.WSFPUpdate(ds, FrmName, SaveStr.Replace("@|","").Replace("@",""));
                    //					MessageBox.Show(Msg.ToString());
                    //					UpdateCK = "OK";
                }

            }
            catch (Exception e)
            {
                SystemBase.Loggers.Log("FPUpdate (그리드 다중저장중 에러)", e.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY008", "그리드 다중저장"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region RowRemove - 그리드 삭제 플레그 등록
        public static void RowRemove(FarPoint.Win.Spread.FpSpread fpSpread1)
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
                        fpSpread1.Sheets[0].Rows.Remove(TmpRow, 1);
                    }
                    else if (fpSpread1.Sheets[0].RowHeader.Cells[TmpRow, 0].Text == "D")
                        fpSpread1.Sheets[0].RowHeader.Cells[TmpRow, 0].Text = "";
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

        #region Cancel - 그리드 취소
        public static void Cancel(FarPoint.Win.Spread.FpSpread fpSpread1, int GridSelectRow, int GridSelectRowCount)
        {
            try
            {
                int TmpRow = GridSelectRow;
                int MoveFcRow = 0;
                int MoveFcCol = 0;

                if (fpSpread1.ActiveSheet.GetSelection(0) == null)
                {
                    MoveFcRow = fpSpread1.ActiveSheet.ActiveRowIndex;
                    MoveFcCol = fpSpread1.ActiveSheet.ActiveRowIndex;
                }
                else
                {
                    MoveFcRow = fpSpread1.ActiveSheet.GetSelection(0).Row;
                    MoveFcCol = fpSpread1.ActiveSheet.GetSelection(0).Column;
                }

                for (int i = GridSelectRow + GridSelectRowCount - 1; i > GridSelectRow - 1; i--)
                {
                    fpSpread1.Sheets[0].RowHeader.Rows[i].BackColor = SystemBase.Base.Color_Org;

                    if (fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text == "I")
                    {
                        fpSpread1.Sheets[0].Rows.Remove(i, 1);

                        if (fpSpread1.Sheets[0].Rows.Count > 0)
                        {
                            fpSpread1.ActiveSheet.SetActiveCell(i - 1, MoveFcCol);
                            fpSpread1.ActiveSheet.AddSelection(i - 1, MoveFcCol, 1, 1);
                            fpSpread1.ShowActiveCell(FarPoint.Win.Spread.VerticalPosition.Nearest, FarPoint.Win.Spread.HorizontalPosition.Nearest);
                        }
                        
                    }
                    else if (fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text == "U")
                    {
                        fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text = "";
                        TmpRow++;

                        if (fpSpread1.Sheets[0].Rows.Count > 0)
                        {
                            fpSpread1.ActiveSheet.SetActiveCell(i, MoveFcCol);
                            fpSpread1.ActiveSheet.AddSelection(i, MoveFcCol, 1, 1);
                            fpSpread1.ShowActiveCell(FarPoint.Win.Spread.VerticalPosition.Nearest, FarPoint.Win.Spread.HorizontalPosition.Nearest);
                        }

                    }
                    else if (fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text == "D")
                    {
                        fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text = "";
                        TmpRow++;

                        if (fpSpread1.Sheets[0].Rows.Count > 0)
                        {
                            fpSpread1.ActiveSheet.SetActiveCell(i, MoveFcCol);
                            fpSpread1.ActiveSheet.AddSelection(i, MoveFcCol, 1, 1);
                            fpSpread1.ShowActiveCell(FarPoint.Win.Spread.VerticalPosition.Nearest, FarPoint.Win.Spread.HorizontalPosition.Nearest);
                        }

                    }

                    
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log("Cancel (그리드 취소버튼클릭 에러)", f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY021"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region ExcelMake - Excel 저장
        public static void ExcelMake(FarPoint.Win.Spread.FpSpread fpSpread1, string formName)
        {
            try
            {
                ExcelDown.ExcelDownLoad(fpSpread1, formName);

                /*
                SaveFileDialog dlg = new SaveFileDialog();
                dlg.DefaultExt = "xls";
                dlg.FileName = FileName.ToString() + ".xls";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    ExcelDown.ExcelDownLoad(fpSpread1, dlg.FileName);
                    // 엑셀 콤포넌트 이용 다운로드

                    //fpSpread1.SaveExcel(dlg.FileName, FarPoint.Win.Spread.Model.IncludeHeaders.ColumnHeadersCustomOnly);
                    //FarPoint 지원 엑셀 변환 (속성까지 다운로드 됨)
                }
                */
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log("ExcelMake (엑셀 다운로드중 에러)", f.ToString());
                MessageBox.Show("ExcelMake - " + SystemBase.Base.MessageRtn("SY022"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region PrintMake - Print	사용예 : UIForm.FPMake.PrintMake(fpSpread1, true);
        public static void PrintMake(FarPoint.Win.Spread.FpSpread fpSpread1, bool Show, int ShowKind)
        {
            try
            {
                FarPoint.Win.Spread.PrintInfo pi = new FarPoint.Win.Spread.PrintInfo();
                pi.Preview = Show;
                if (ShowKind == 0)
                    pi.Orientation = FarPoint.Win.Spread.PrintOrientation.Landscape;
                else if (ShowKind == 1)
                    pi.Orientation = FarPoint.Win.Spread.PrintOrientation.Portrait;
                else
                    pi.Orientation = FarPoint.Win.Spread.PrintOrientation.Auto;
                fpSpread1.Sheets[0].PrintInfo = pi;

                fpSpread1.PrintSheet(0);
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log("PrintMake (프린트 공통 에러)", f.ToString());
                MessageBox.Show("PrintMake - " + SystemBase.Base.MessageRtn("SY023"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        //2010-02-01 by C.H.T 스프레드 출력
        public static void PrintMake(FarPoint.Win.Spread.FpSpread fpGrid, string Name)
        {
            FarPoint.Win.Spread.PrintInfo printset = new FarPoint.Win.Spread.PrintInfo();
            printset.ShowPrintDialog = true;
            printset.Preview = true;
            
            printset.Orientation = FarPoint.Win.Spread.PrintOrientation.Landscape; // Portrait;
            printset.Margin.Top = 30;
            printset.Margin.Left = 30;
            printset.Margin.Right = 20;
            printset.Margin.Bottom = 20;
            printset.Centering = FarPoint.Win.Spread.Centering.None;
            printset.ShowGrid = true;
            printset.ShowShadows = true;
            printset.ShowBorder = true;
            //			printset.ShowColor = false;
            printset.PrintNotes = PrintNotes.AsDisplayed;

            printset.PrintType = FarPoint.Win.Spread.PrintType.CellRange;
            //			printset.UseSmartPrint = false;
            printset.UseMax = false;

            //printset.Header = "/c/fz\"22\"/fb1" + Name + "/fb0/fz\"10\"/n";
            //printset.Header = printset.Header + "/l/fz\"10\"                                                                                                                              사용자 : " + SystemBase.Base.gstrUserName.ToString() + "/n";
            //printset.Header = printset.Header + "/l/fz\"10\"                                                                                                                            인쇄일자 : " + DateTime.Now.ToString("yyyy-MM-dd") + "/n\n\n\n\n";
            printset.Header = "/c/fz\"22\"/fb1" + Name + "/fb0/fz\"10\"";
            printset.Header = printset.Header + "/r/fz\"10\"       사용자 : " + SystemBase.Base.gstrUserName.ToString() + "";
            printset.Header = printset.Header + "/r/fz\"10\"     인쇄일자 : " + DateTime.Now.ToString("yyyy-MM-dd") + "/n\n\n";

            printset.Footer = "/c/fz\"10\"/p / /pc";
            
            fpGrid.Sheets[0].PrintInfo = printset;
            
            fpGrid.PrintSheet(0);
        }
        #endregion

        #region fpChange 수정플래그 저장
        public static void fpChange(FarPoint.Win.Spread.FpSpread fpSpread1, int Row)
        {
            try
            {
                if (fpSpread1.Sheets[0].RowHeader.Cells[Row, 0].Text != "I")
                {
                    fpSpread1.Sheets[0].RowHeader.Cells[Row, 0].Text = "U";

                    fpSpread1.Sheets[0].RowHeader.Rows[Row].BackColor = SystemBase.Base.Color_Update;
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log("fpChange (수정 플래그 등록 실패)", f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY008", "수정 플래그 등록"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region RowCopy 복사
        public static void RowCopy(FarPoint.Win.Spread.FpSpread fpSpread1)
        {
            try
            {
                //if (fpSpread1.Sheets[0].ActiveRowIndex.ToString() == "")
                //				if(fpSpread1.ActiveSheet.GetSelection(0) == null)
                //				{
                //					MessageBox.Show("복사할 Row를 선택하지 않았습니다.");
                //				}
                //				else
                //				{
                if (fpSpread1.Sheets[0].Rows.Count > 0)
                {
                    int SelectedRow = 0;
                    //int SelectedRow = fpSpread1.ActiveSheet.ActiveRowIndex; //fpSpread1.ActiveSheet.GetSelection(0).Row;
                    if (fpSpread1.ActiveSheet.GetSelection(0) == null)
                    {
                        SelectedRow = fpSpread1.ActiveSheet.ActiveRowIndex;
                    }
                    else
                    {
                        SelectedRow = fpSpread1.ActiveSheet.GetSelection(0).Row;
                    }


                    UIForm.FPMake.RowInsert(fpSpread1);

                    for (int i = 0; i < fpSpread1.Sheets[0].Columns.Count; i++)
                    {
                        fpSpread1.Sheets[0].Cells[SelectedRow + 1, i].Value = fpSpread1.Sheets[0].Cells[SelectedRow, i].Value;
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

        #region grdCommSheet(2) - 그리드 (Top 100 1Row씩 추가)
        public static void grdCommSheet(FarPoint.Win.Spread.FpSpread baseGrid, string Query)
        {
            try
            {
                int GrdRowCount = baseGrid.Sheets[0].Rows.Count;
                DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);

                if (dt.Rows.Count > 0)
                {
                    if (GrdRowCount > 999 && GrdRowCount < 10000)
                        baseGrid.Sheets[0].RowHeader.Columns[0].Width = 38;
                    else if (GrdRowCount > 9999)
                        baseGrid.Sheets[0].RowHeader.Columns[0].Width = 44;

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        baseGrid.Sheets[0].Rows.Add(GrdRowCount + i, 1);	//Row 추가
                        for (int j = 0; j < baseGrid.Sheets[0].Columns.Count; j++)
                        {
                            if (dt.Rows[i][j].ToString().Trim() == "")
                                baseGrid.Sheets[0].Cells[GrdRowCount + i, j].Value = null;
                            else
                                baseGrid.Sheets[0].Cells[GrdRowCount + i, j].Value = dt.Rows[i][j].ToString();
                        }
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log("grdCommSheet (그리드 (Top 100) )", Query.ToString() + "\n" + f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY024"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                baseGrid.Sheets[0].Rows.Count = 0;
            }
        }
        #endregion

        #region grdCommSheet(2) - 그리드 (Top 100 100Row씩 추가 스크롤이 빠른 경우 에러???)
        public static void grdCommSheet(string Query, FarPoint.Win.Spread.FpSpread baseGrid)
        {
            try
            {
                int GrdRowCount = baseGrid.Sheets[0].Rows.Count;
                DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);

                if (dt.Rows.Count > 0)
                {
                    if (GrdRowCount > 999 && GrdRowCount < 10000)
                        baseGrid.Sheets[0].RowHeader.Columns[0].Width = 38;
                    else if (GrdRowCount > 9999)
                        baseGrid.Sheets[0].RowHeader.Columns[0].Width = 44;

                    baseGrid.Sheets[0].Rows.Add(GrdRowCount, dt.Rows.Count);	//Row 추가

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        for (int j = 0; j < baseGrid.Sheets[0].Columns.Count; j++)
                        {
                            if (dt.Rows[i][j].ToString().Trim() == "")
                                baseGrid.Sheets[0].Cells[GrdRowCount + i, j].Value = null;
                            else
                                baseGrid.Sheets[0].Cells[GrdRowCount + i, j].Value = dt.Rows[i][j].ToString();
                        }
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log("grdCommSheet (그리드 (Top 100) )", Query.ToString() + "\n" + f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY024"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                baseGrid.Sheets[0].Rows.Count = 0;
            }
        }
        #endregion

        #region grdCommSheet(3) - 그리드 (일반)
        //		public static void grdCommSheet(FarPoint.Win.Spread.FpSpread baseGrid, string Query, int RowCnt)
        //		{
        //			try
        //			{
        //				int AddRowCnt = 100;
        //				DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);
        //
        //				if(dt.Rows.Count > 0)
        //				{
        //					if(RowCnt > 999 && RowCnt < 10000)
        //						baseGrid.Sheets[0].RowHeader.Columns[0].Width = 38;
        //					else if(RowCnt > 9999)
        //						baseGrid.Sheets[0].RowHeader.Columns[0].Width = 44;
        //
        //					baseGrid.Sheets[0].Rows.Add(RowCnt-AddRowCnt, AddRowCnt);	//Row 추가
        //
        //					for(int i = RowCnt-100; i < RowCnt; i++)
        //					{
        //						for(int j = 0; j < baseGrid.Sheets[0].Columns.Count; j++)
        //						{
        //							baseGrid.Sheets[0].Cells[i, j].Value = dt.Rows[i][j].ToString();
        //						}
        //					}
        //				}
        //			}
        //			catch(Exception f)
        //			{
        //				SystemBase.Loggers.Log("grdCommSheet (그리드 (Top 100) )", Query.ToString() + "\n" + f.ToString());
        //				MessageBox.Show(SystemBase.Base.MessageRtn("SY013") + f.ToString());
        //			}
        //		}
        #endregion

        #region grdReMake - 그리드 속성 재정의
        public static void grdReMake(FarPoint.Win.Spread.FpSpread baseGrid, string Kind)
        {	//그리드 속성 재정의	 grdReMake(baseGrid, "1|3#5|3#5|1")
            try
            {
                if (Kind == null)
                    Kind = "";

                Regex rx = new Regex("#");
                string[] remake = rx.Split(Kind);

                for (int i = 0; i < remake.Length; i++)
                {
                    int Tmp1 = Convert.ToInt32(remake[i].Substring(0, remake[i].IndexOf("|")));
                    int Tmp2 = Convert.ToInt32(remake[i].Substring(remake[i].IndexOf("|") + 1, remake[i].Length - remake[i].IndexOf("|") - 1));

                    for (int j = 0; j < baseGrid.Sheets[0].Rows.Count; j++)
                    {
                        switch (Tmp2)
                        {
                            case 0:	// 일반
                                baseGrid.Sheets[0].Cells[j, Tmp1].BackColor = Color.White;
                                baseGrid.Sheets[0].Cells[j, Tmp1].Locked = false;
                                baseGrid.Sheets[0].Cells[j, Tmp1].CanFocus = true;
                                break;
                            case 1:	// 필수입력
                                baseGrid.Sheets[0].Cells[j, Tmp1].BackColor = SystemBase.Validation.Kind_LightCyan; //System.Drawing.Color.FromArgb(242, 252, 254);
                                baseGrid.Sheets[0].Cells[j, Tmp1].Locked = false;
                                baseGrid.Sheets[0].Cells[j, Tmp1].CanFocus = true;
                                break;
                            case 2:	// 읽기전용, 필수항목
                                baseGrid.Sheets[0].Cells[j, Tmp1].BackColor = System.Drawing.Color.FromArgb(239, 239, 239);
                                baseGrid.Sheets[0].Cells[j, Tmp1].Locked = true;
                                baseGrid.Sheets[0].Cells[j, Tmp1].CanFocus = true;
                                break;
                            case 3:	// 읽기전용이면서 필수항목에서 제외
                                baseGrid.Sheets[0].Cells[j, Tmp1].BackColor = System.Drawing.Color.FromArgb(238, 238, 238);
                                baseGrid.Sheets[0].Cells[j, Tmp1].Locked = true;
                                baseGrid.Sheets[0].Cells[j, Tmp1].CanFocus = true;
                                break;
                            case 4:	// 읽기전용 & ReadOnly & White
                                baseGrid.Sheets[0].Cells[j, Tmp1].BackColor = Color.White;
                                baseGrid.Sheets[0].Cells[j, Tmp1].Locked = true;
                                baseGrid.Sheets[0].Cells[j, Tmp1].CanFocus = true;
                                break;
                            case 5:	// 읽기전용, 필수항목에서 제외, Focus 제외
                                baseGrid.Sheets[0].Cells[j, Tmp1].BackColor = System.Drawing.Color.FromArgb(238, 238, 238);
                                baseGrid.Sheets[0].Cells[j, Tmp1].Locked = true;
                                baseGrid.Sheets[0].Cells[j, Tmp1].CanFocus = false;
                                break;
                            case 6:	// 읽기전용, 필수항목, Focus 제외
                                baseGrid.Sheets[0].Cells[j, Tmp1].BackColor = System.Drawing.Color.FromArgb(239, 239, 239);
                                baseGrid.Sheets[0].Cells[j, Tmp1].Locked = true;
                                baseGrid.Sheets[0].Cells[j, Tmp1].CanFocus = false;
                                break;
                            default:	// 일반
                                baseGrid.Sheets[0].Cells[j, Tmp1].BackColor = Color.White;
                                baseGrid.Sheets[0].Cells[j, Tmp1].Locked = false;
                                baseGrid.Sheets[0].Cells[j, Tmp1].CanFocus = true;
                                break;
                        }
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log("grdReMake (그리드 속성 재정의 실패 2)", f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY025"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        //2024-01-18 수정 by Cht
        public static void grdReMake(FarPoint.Win.Spread.FpSpread baseGrid, Dictionary<int, int> dicKind)
        {	//그리드 속성 재정의  grdReMake(baseGrid, 5, "1|3#5|3#5|1")
            try
            {
                if (dicKind == null || dicKind.Count == 0)
                    return;

                foreach (int col in dicKind.Keys)
                {
                    int kind = dicKind[col];

                    switch (kind)
                    {
                        case 0:	// 일반
                            baseGrid.Sheets[0].Columns[col].BackColor = Color.White;
                            baseGrid.Sheets[0].Columns[col].Locked = false;
                            baseGrid.Sheets[0].Columns[col].CanFocus = true;
                            break;
                        case 1:	// 필수입력
                            baseGrid.Sheets[0].Columns[col].BackColor = SystemBase.Validation.Kind_LightCyan; //System.Drawing.Color.FromArgb(242, 252, 254);
                            baseGrid.Sheets[0].Columns[col].Locked = false;
                            baseGrid.Sheets[0].Columns[col].CanFocus = true;
                            break;
                        case 2:	// 읽기전용, 필수항목
                            baseGrid.Sheets[0].Columns[col].BackColor = System.Drawing.Color.FromArgb(239, 239, 239);
                            baseGrid.Sheets[0].Columns[col].Locked = true;
                            baseGrid.Sheets[0].Columns[col].CanFocus = true;
                            break;
                        case 3:	// 읽기전용이면서 필수항목에서 제외
                            baseGrid.Sheets[0].Columns[col].BackColor = System.Drawing.Color.FromArgb(238, 238, 238);
                            baseGrid.Sheets[0].Columns[col].Locked = true;
                            baseGrid.Sheets[0].Columns[col].CanFocus = true;
                            break;
                        case 4:	// 읽기전용 & ReadOnly & White
                            baseGrid.Sheets[0].Columns[col].BackColor = Color.White;
                            baseGrid.Sheets[0].Columns[col].Locked = true;
                            baseGrid.Sheets[0].Columns[col].CanFocus = true;
                            break;
                        case 5:	// 읽기전용, 필수항목에서 제외, Focus 제외
                            baseGrid.Sheets[0].Columns[col].BackColor = System.Drawing.Color.FromArgb(238, 238, 238);
                            baseGrid.Sheets[0].Columns[col].Locked = true;
                            baseGrid.Sheets[0].Columns[col].CanFocus = false;
                            break;
                        case 6:	// 읽기전용, 필수항목, Focus 제외
                            baseGrid.Sheets[0].Columns[col].BackColor = System.Drawing.Color.FromArgb(239, 239, 239);
                            baseGrid.Sheets[0].Columns[col].Locked = true;
                            baseGrid.Sheets[0].Columns[col].CanFocus = false;
                            break;
                        default:	// 일반
                            baseGrid.Sheets[0].Columns[col].BackColor = Color.White;
                            baseGrid.Sheets[0].Columns[col].Locked = false;
                            baseGrid.Sheets[0].Columns[col].CanFocus = true;
                            break;
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log("grdReMake (그리드 속성 재정의 실패 3)", f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY025"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        public static void grdReMake(FarPoint.Win.Spread.FpSpread baseGrid, int Cnt, int Kind)
        {	//그리드 속성 재정의	 grdReMake(baseGrid, 21, 3)	Cnt는 속성 재정의할 칼럼 수 0~21까지
            try
            {
                if (baseGrid.Sheets[0].Columns.Count < Cnt)
                    Cnt = baseGrid.Sheets[0].Columns.Count;
                for (int i = 0; i < Cnt; i++)
                {
                    for (int j = 0; j < baseGrid.Sheets[0].Rows.Count; j++)
                    {
                        switch (Kind)
                        {
                            case 0:	// 일반
                                baseGrid.Sheets[0].Cells[j, i].BackColor = Color.White;
                                baseGrid.Sheets[0].Cells[j, i].Locked = false;
                                baseGrid.Sheets[0].Cells[j, i].CanFocus = true;
                                break;
                            case 1:	// 필수입력
                                baseGrid.Sheets[0].Cells[j, i].BackColor = SystemBase.Validation.Kind_LightCyan; //System.Drawing.Color.FromArgb(242, 252, 254);
                                baseGrid.Sheets[0].Cells[j, i].Locked = false;
                                baseGrid.Sheets[0].Cells[j, i].CanFocus = true;
                                break;
                            case 2:	// 읽기전용, 필수항목
                                baseGrid.Sheets[0].Cells[j, i].BackColor = System.Drawing.Color.FromArgb(239, 239, 239);
                                baseGrid.Sheets[0].Cells[j, i].Locked = true;
                                baseGrid.Sheets[0].Cells[j, i].CanFocus = true;
                                break;
                            case 3:	// 읽기전용이면서 필수항목에서 제외
                                baseGrid.Sheets[0].Cells[j, i].BackColor = System.Drawing.Color.FromArgb(238, 238, 238);
                                baseGrid.Sheets[0].Cells[j, i].Locked = true;
                                baseGrid.Sheets[0].Cells[j, i].CanFocus = true;
                                break;
                            case 4:	// 읽기전용 & ReadOnly & White
                                baseGrid.Sheets[0].Cells[j, i].BackColor = Color.White;
                                baseGrid.Sheets[0].Cells[j, i].Locked = true;
                                baseGrid.Sheets[0].Cells[j, i].CanFocus = true;
                                break;
                            case 5:	// 읽기전용, 필수항목에서 제외, Focus 제외
                                baseGrid.Sheets[0].Cells[j, i].BackColor = System.Drawing.Color.FromArgb(238, 238, 238);
                                baseGrid.Sheets[0].Cells[j, i].Locked = true;
                                baseGrid.Sheets[0].Cells[j, i].CanFocus = false;
                                break;
                            case 6:	// 읽기전용, 필수항목, Focus 제외
                                baseGrid.Sheets[0].Cells[j, i].BackColor = System.Drawing.Color.FromArgb(239, 239, 239);
                                baseGrid.Sheets[0].Cells[j, i].Locked = true;
                                baseGrid.Sheets[0].Cells[j, i].CanFocus = false;
                                break;
                            default:	// 일반
                                baseGrid.Sheets[0].Cells[j, i].BackColor = Color.White;
                                baseGrid.Sheets[0].Cells[j, i].Locked = false;
                                baseGrid.Sheets[0].Cells[j, i].CanFocus = true;
                                break;
                        }
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log("grdReMake (그리드 속성 재정의 실패 2)", f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY025"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }


        public static void grdReMake(FarPoint.Win.Spread.FpSpread baseGrid, int Row, string Kind)
        {	//그리드 속성 재정의  grdReMake(baseGrid, 5, "1|3#5|3#5|1")
            try
            {
                if (Kind == null)
                    Kind = "";

                Regex rx = new Regex("#");
                string[] remake = rx.Split(Kind);


                for (int i = 0; i < remake.Length; i++)
                {
                    int Tmp1 = Convert.ToInt32(remake[i].Substring(0, remake[i].IndexOf("|")));
                    int Tmp2 = Convert.ToInt32(remake[i].Substring(remake[i].IndexOf("|") + 1, remake[i].Length - remake[i].IndexOf("|") - 1));

                    switch (Tmp2)
                    {
                        case 0:	// 일반
                            baseGrid.Sheets[0].Cells[Row, Tmp1].BackColor = Color.White;
                            baseGrid.Sheets[0].Cells[Row, Tmp1].Locked = false;
                            baseGrid.Sheets[0].Cells[Row, Tmp1].CanFocus = true;
                            break;
                        case 1:	// 필수입력
                            baseGrid.Sheets[0].Cells[Row, Tmp1].BackColor = SystemBase.Validation.Kind_LightCyan; //System.Drawing.Color.FromArgb(242, 252, 254);
                            baseGrid.Sheets[0].Cells[Row, Tmp1].Locked = false;
                            baseGrid.Sheets[0].Cells[Row, Tmp1].CanFocus = true;
                            break;
                        case 2:	// 읽기전용, 필수항목
                            baseGrid.Sheets[0].Cells[Row, Tmp1].BackColor = System.Drawing.Color.FromArgb(239, 239, 239);
                            baseGrid.Sheets[0].Cells[Row, Tmp1].Locked = true;
                            baseGrid.Sheets[0].Cells[Row, Tmp1].CanFocus = true;
                            break;
                        case 3:	// 읽기전용이면서 필수항목에서 제외
                            baseGrid.Sheets[0].Cells[Row, Tmp1].BackColor = System.Drawing.Color.FromArgb(238, 238, 238);
                            baseGrid.Sheets[0].Cells[Row, Tmp1].Locked = true;
                            baseGrid.Sheets[0].Cells[Row, Tmp1].CanFocus = true;
                            break;
                        case 4:	// 읽기전용 & ReadOnly & White
                            baseGrid.Sheets[0].Cells[Row, Tmp1].BackColor = Color.White;
                            baseGrid.Sheets[0].Cells[Row, Tmp1].Locked = true;
                            baseGrid.Sheets[0].Cells[Row, Tmp1].CanFocus = true;
                            break;
                        case 5:	// 읽기전용, 필수항목에서 제외, Focus 제외
                            baseGrid.Sheets[0].Cells[Row, Tmp1].BackColor = System.Drawing.Color.FromArgb(238, 238, 238);
                            baseGrid.Sheets[0].Cells[Row, Tmp1].Locked = true;
                            baseGrid.Sheets[0].Cells[Row, Tmp1].CanFocus = false;
                            break;
                        case 6:	// 읽기전용, 필수항목, Focus 제외
                            baseGrid.Sheets[0].Cells[Row, Tmp1].BackColor = System.Drawing.Color.FromArgb(239, 239, 239);
                            baseGrid.Sheets[0].Cells[Row, Tmp1].Locked = true;
                            baseGrid.Sheets[0].Cells[Row, Tmp1].CanFocus = false;
                            break;
                        default:	// 일반
                            baseGrid.Sheets[0].Cells[Row, Tmp1].BackColor = Color.White;
                            baseGrid.Sheets[0].Cells[Row, Tmp1].Locked = false;
                            baseGrid.Sheets[0].Cells[Row, Tmp1].CanFocus = true;
                            break;
                    }

                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log("grdReMake (그리드 속성 재정의 실패 3)", f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY025"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }


        public static void grdReMake(FarPoint.Win.Spread.FpSpread baseGrid, int Row, int[] G1Color)
        {	// 그리드 속성 재정의  grdReMake(baseGrid, 5, "1|3#5|3#5|1")
            try
            {
                for (int i = 0; i < G1Color.Length; i++)
                {
                    switch (G1Color[i])
                    {
                        case 0:	// 일반
                            baseGrid.Sheets[0].Cells[Row, i].BackColor = Color.White;
                            baseGrid.Sheets[0].Cells[Row, i].Locked = false;
                            baseGrid.Sheets[0].Cells[Row, i].CanFocus = true;
                            break;
                        case 1:	// 필수입력
                            baseGrid.Sheets[0].Cells[Row, i].BackColor = SystemBase.Validation.Kind_LightCyan; //System.Drawing.Color.FromArgb(242, 252, 254);
                            baseGrid.Sheets[0].Cells[Row, i].Locked = false;
                            baseGrid.Sheets[0].Cells[Row, i].CanFocus = true;
                            break;
                        case 2:	// 읽기전용, 필수항목
                            baseGrid.Sheets[0].Cells[Row, i].BackColor = System.Drawing.Color.FromArgb(239, 239, 239);
                            baseGrid.Sheets[0].Cells[Row, i].Locked = true;
                            baseGrid.Sheets[0].Cells[Row, i].CanFocus = true;
                            break;
                        case 3:	// 읽기전용이면서 필수항목에서 제외
                            baseGrid.Sheets[0].Cells[Row, i].BackColor = System.Drawing.Color.FromArgb(238, 238, 238);
                            baseGrid.Sheets[0].Cells[Row, i].Locked = true;
                            baseGrid.Sheets[0].Cells[Row, i].CanFocus = true;
                            break;
                        case 4:	// 읽기전용 & ReadOnly & White
                            baseGrid.Sheets[0].Cells[Row, i].BackColor = Color.White;
                            baseGrid.Sheets[0].Cells[Row, i].Locked = true;
                            baseGrid.Sheets[0].Cells[Row, i].CanFocus = true;
                            break;
                        case 5:	// 읽기전용, 필수항목에서 제외, Focus 제외
                            baseGrid.Sheets[0].Cells[Row, i].BackColor = System.Drawing.Color.FromArgb(238, 238, 238);
                            baseGrid.Sheets[0].Cells[Row, i].Locked = true;
                            baseGrid.Sheets[0].Cells[Row, i].CanFocus = false;
                            break;
                        case 6:	// 읽기전용, 필수항목, Focus 제외
                            baseGrid.Sheets[0].Cells[Row, i].BackColor = System.Drawing.Color.FromArgb(239, 239, 239);
                            baseGrid.Sheets[0].Cells[Row, i].Locked = true;
                            baseGrid.Sheets[0].Cells[Row, i].CanFocus = false;
                            break;
                        default:	// 일반
                            baseGrid.Sheets[0].Cells[Row, i].BackColor = Color.White;
                            baseGrid.Sheets[0].Cells[Row, i].Locked = false;
                            baseGrid.Sheets[0].Cells[Row, i].CanFocus = true;
                            break;
                    }

                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log("grdReMake (그리드 속성 재정의 실패 4)", f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY025"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        #endregion

        #region ButtonClick - 버튼 클릭 이벤트
        public static void ButtonClick(FarPoint.Win.Spread.FpSpread fpSpread1, string strFORM_ID, string RtnDTCol, int Focus, int frmWidth, int Rows, int Colunms, string strQuery, string[] strWhere, string[] strSearch, int[] PSearchLabel)
        {//							                                 그리드,         쿼리                                          ,선택한 Row, 선택한 Col, Focus위치,     Return 위치,           팝업헤드,          팝업 정렬,             셀타입,        헤드넓이 , 팝업라벨 텍스트위치,창 넓이
            try
            {
               // if (fpSpread1.Sheets[0].Cells[Rows, Colunms].BackColor.Name.ToString() != "0" && fpSpread1.Sheets[0].Cells[Rows, Colunms].BackColor.Name.ToString() != "LightGray" && fpSpread1.Sheets[0].Cells[Rows, Colunms].BackColor.Name.ToString() != "Gainsboro")
                if (fpSpread1.Sheets[0].Cells[Rows, Colunms].BackColor.Name.ToString() != "0" && fpSpread1.Sheets[0].Cells[Rows, Colunms].BackColor != System.Drawing.Color.FromArgb(238, 238, 238) && fpSpread1.Sheets[0].Cells[Rows, Colunms].BackColor != System.Drawing.Color.FromArgb(239, 239, 239))
                {
                    if (fpSpread1.Sheets[0].GetCellType(Rows, Colunms).ToString() == "ButtonCellType")
                    {	// 버튼 Focus 후 KeyEnter시 팝업창 호출// && fpSpread1.Sheets[0].Cells[Rows,Colunms].BackColor != Color.Empty
                        fpPopupCommon(fpSpread1, strFORM_ID, strQuery, strWhere, strSearch, Rows, RtnDTCol, PSearchLabel, frmWidth);
                        fpSpread1.ActiveSheet.SetActiveCell(Rows, Focus);
                        fpChange(fpSpread1, Rows);
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log("ButtonClick (버튼 클릭 이벤트 에러)", f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY008", "그리드 버튼 클릭"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void fpPopupCommon(FarPoint.Win.Spread.FpSpread fpSpread1, string strFORM_ID, string strQuery, string[] strWhere, string[] strSearch, int Rows, string RtnDTCol, int[] PSearchLabel, int frmWidth)
        {
            try
            {
                FPPOPUP pu = new FPPOPUP(strFORM_ID, strQuery, strWhere, strSearch, PSearchLabel);
                pu.ClientSize = new System.Drawing.Size(frmWidth, 450);
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string RtnVal = pu.ReturnVal.ToString();

                    Regex rx = new Regex("#");
                    string[] RtnVals = rx.Split(RtnVal.ToString());

                    Regex rx2 = new Regex("#");
                    string[] RtnDTCols = rx2.Split(RtnDTCol);

                    for (int i = 0; i < RtnDTCols.Length; i++)
                    {
                        if (RtnDTCols[i].ToString().Length > 0)
                            fpSpread1.Sheets[0].Cells[Rows, Convert.ToInt32(RtnDTCols[i].ToString())].Value = RtnVals[i].ToString();
                    }

                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log("fpPopupCommon (버튼 클릭 Popup 에러)", f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY008", "그리드 팝업 호출"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 그리드 1Row씩 저장 수정 삭제시 그리드 속성 재정의
        //## UIForm.FPMake.fpSpread_ReType(fpSpread1, RowHeader, i, G1Color); if(strHead == "D")i=i-1;//삭제인 경우 Row -1
        //## UIForm.FPMake.fpSpread_ReType(그리드,I, U, D( 저장 수정 삭제구분자) , 1(Row), 그리드 속성 배열)
        public static void fpSpread_ReType(FarPoint.Win.Spread.FpSpread baseGrid, string RowHeader, int intRow, int[] G1Color)
        {
            if (RowHeader == "U")
            {
                baseGrid.Sheets[0].RowHeader.Cells[intRow, 0].Text = "";
            }
            else if (RowHeader == "I")
            {
                baseGrid.Sheets[0].RowHeader.Cells[intRow, 0].Text = "";
                grdReMake(baseGrid, intRow, G1Color);
            }
            else if (RowHeader == "D")
            {
                baseGrid.Sheets[0].Rows[intRow].Remove();
            }
        }
        #endregion

        #region cell안의 Text 세로로 나오게 하기
        public static void CellTextOrientation(FarPoint.Win.Spread.FpSpread baseGrid, FarPoint.Win.Spread.CellType.TextCellType tc, int Row, int Col)
        {
            baseGrid.Sheets[0].Cells[Row, Col].CellType = tc;
            tc.TextOrientation = FarPoint.Win.TextOrientation.TextTopDown;
        }
        #endregion

        #region cell 가운데 정렬
        public static void CellCenter(FarPoint.Win.Spread.FpSpread baseGrid, int Row, int Col)
        {
            baseGrid.Sheets[0].Cells[Row, Col].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
            baseGrid.Sheets[0].Cells[Row, Col].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
        }
        #endregion

        #region CellSpan하고 데이터 입력
        public static void CellTextRowSpan(FarPoint.Win.Spread.FpSpread baseGrid, int Row, int Col, int RowSpanCount, string Text)
        {
            baseGrid.Sheets[0].Cells[Row, Col].RowSpan = RowSpanCount;
            baseGrid.Sheets[0].Cells[Row, Col].Text = Text;
        }

        public static void CellTextColumnSpan(FarPoint.Win.Spread.FpSpread baseGrid, int Row, int Col, int ColSpanCount, string Text)
        {
            baseGrid.Sheets[0].Cells[Row, Col].ColumnSpan = ColSpanCount;
            baseGrid.Sheets[0].Cells[Row, Col].Text = Text;
        }

        public static void CellTextSpan(FarPoint.Win.Spread.FpSpread baseGrid, int Row, int Col, int RowSpanCount, int ColSpanCount, string Text)
        {
            baseGrid.Sheets[0].Cells[Row, Col].RowSpan = RowSpanCount;
            baseGrid.Sheets[0].Cells[Row, Col].ColumnSpan = ColSpanCount;
            baseGrid.Sheets[0].Cells[Row, Col].Text = Text;
        }
        #endregion

        #region 저장정보 존재 여부 검사
        public static bool HasSaveData(FarPoint.Win.Spread.FpSpread baseGrid)
        {
            for (int i = 0; i < baseGrid.Sheets[0].RowCount; i++)
                if (baseGrid.Sheets[0].RowHeader.Cells[i, 0].Text.Length > 0)
                    return true;
            return false;
        }
        #endregion

        #region 데이터신규저장

        public static bool grdCommSheetRowAdd(FarPoint.Win.Spread.FpSpread baseGrid, string strQuery)
        {
            bool bRet = true;
            try
            {
                DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);
                if (dt != null && dt.Rows.Count > 0)
                {
                    if (baseGrid.DataSource == null)
                    {
                        baseGrid.DataSource = dt;
                    }
                    else
                    {
                        baseGrid.ActiveSheet.Rows.Add(0, 1);

                        for (int i = 0; i < dt.Columns.Count; i++)
                        {
                            baseGrid.ActiveSheet.SetValue(0, i, dt.Rows[0][i]);
                        }

                        //						if(baseGrid.DataSource.GetType().Equals(typeof(DataTable)))
                        //						{
                        //							DataTable dtOriginal = (DataTable)baseGrid.DataSource;
                        //							DataRow row = dtOriginal.NewRow();
                        //					
                        //							foreach(DataColumn col in dtOriginal.Columns)
                        //							{
                        //								if(dt.Columns.Contains(col.ColumnName))
                        //								{
                        //									row[col.ColumnName] = dt.Rows[0][col.ColumnName];
                        //								}
                        //							}
                        //							dtOriginal.Rows.InsertAt(row, 0);
                        //						}
                    }
                }
            }
            catch
            {
                bRet = false;
            }
            return bRet;
        }

        #endregion

        #region 데이터 수정

        public static bool grdCommSheetRowUpdate(FarPoint.Win.Spread.FpSpread baseGrid, string strQuery, int rowIdx)
        {
            bool bRet = true;
            try
            {
                DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);
                if (dt != null && dt.Rows.Count > 0)
                {
                    if (baseGrid.DataSource == null)
                    {
                        baseGrid.DataSource = dt;
                    }
                    else
                    {
                        for (int i = 0; i < dt.Columns.Count; i++)
                        {
                            baseGrid.ActiveSheet.SetValue(rowIdx, i, dt.Rows[0][i]);
                        }
                        //						if(baseGrid.DataSource.GetType().Equals(typeof(DataTable)))
                        //						{
                        //							DataTable dtOriginal = (DataTable)baseGrid.DataSource;
                        //				
                        //							foreach(DataColumn col in dtOriginal.Columns)
                        //							{
                        //								if(dt.Columns.Contains(col.ColumnName))
                        //								{
                        //									dtOriginal.Rows[rowIdx][col.ColumnName] = dt.Rows[0][col.ColumnName];
                        //								}
                        //							}
                        //						}
                    }
                }
            }
            catch
            {
                bRet = false;
            }
            return bRet;
        }

        #endregion

        #region 데이터 삭제

        public static bool grdCommSheetRowDel(FarPoint.Win.Spread.FpSpread baseGrid, int rowIdx)
        {
            bool bRet = true;
            try
            {
                baseGrid.ActiveSheet.RemoveRows(rowIdx, 1);
            }
            catch
            {
                bRet = false;
            }
            return bRet;
        }

        #endregion

        #region 조회 후 그리드 셀 포커스 이동
        public static void GridSetFocus(FarPoint.Win.Spread.FpSpread baseGrid, string FindText)
        {
            if (baseGrid.Sheets[0].RowCount > 0)
            {
                int i = 0, j = 0;
                baseGrid.Search(0, FindText, false, false, false, false, 0, 1, ref i, ref j);

                //findCol이 hidden이면 hidden이 아닐때까지 +해준다
                while (baseGrid.Sheets[0].Columns[j].Visible == false)
                {
                    j++;
                }

                baseGrid.Sheets[0].SetActiveCell(i, j);
                baseGrid.ShowActiveCell(FarPoint.Win.Spread.VerticalPosition.Top, FarPoint.Win.Spread.HorizontalPosition.Nearest);
            }
        }

        public static void GridSetFocus(FarPoint.Win.Spread.FpSpread baseGrid, string FindText, int FindCol)
        {
            if (baseGrid.Sheets[0].RowCount > 0)
            {
                int i = 0, j = 0;
                baseGrid.Search(0, FindText, false, false, false, false, 0, FindCol, baseGrid.Sheets[0].RowCount - 1, FindCol, ref i, ref j);

                //findCol이 hidden이면 hidden이 아닐때까지 +해준다
                while (baseGrid.Sheets[0].Columns[j].Visible == false)
                {
                    j++;
                }

                baseGrid.Sheets[0].SetActiveCell(i, j);
                baseGrid.ShowActiveCell(FarPoint.Win.Spread.VerticalPosition.Top, FarPoint.Win.Spread.HorizontalPosition.Nearest);
            }
        }
        #endregion
    }
}