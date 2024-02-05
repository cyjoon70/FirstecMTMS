#region 작성정보
/*********************************************************************/
// 단위업무명 : 프로젝트별 실적공수 현황
// 작 성 자 : 권 순 철
// 작 성 일 : 2013-04-15
// 작성내용 : 프로젝트별 실적공수 및 관리
// 수 정 일 :
// 수 정 자 :
// 수정내용 :
// 비    고 :
/*********************************************************************/
#endregion

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using C1.Win.C1Chart;

namespace EI.EIS206
{
    public partial class EIS206 : UIForm.FPCOMM1
    {
        public EIS206()
        {
            InitializeComponent();
        }

        #region Form Load시
        private void EIS206_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            ChartDataSeriesCollection coll = c1Chart1.ChartGroups[0].ChartData.SeriesList;
            coll[0].PointData.Clear();
            coll[1].PointData.Clear();

            Axis ax = c1Chart1.ChartArea.AxisX;
            ax.ValueLabels.Clear();

            fpSpread1.Sheets[0].RowHeader.Columns[0].Width = 80;

            lblBus_Nm.Text = "";
            dtpYear.Text = SystemBase.Base.ServerTime("YYMMDD").Substring(0, 4);
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            ChartDataSeriesCollection coll = c1Chart1.ChartGroups[0].ChartData.SeriesList;
            coll[0].PointData.Clear();
            coll[1].PointData.Clear();

            Axis ax = c1Chart1.ChartArea.AxisX;
            ax.ValueLabels.Clear();

            fpSpread1.Sheets[0].RowCount = 0;
            fpSpread1.Sheets[0].ColumnCount = 0;
        }
        #endregion

        #region SearchExec 그리드 조회
        protected override void SearchExec()
        {
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                this.Cursor = Cursors.WaitCursor;
                try
                {
                    groupBox2.Controls.Clear();

                    string strQuery = " usp_EIS206 'S1'";
                    strQuery += ", @pYEAR ='" + dtpYear.Text + "'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

                    if (dt.Rows.Count > 0)
                    {
                        int cnt = dt.Rows.Count;
                        int x = 8;
                        int y = 16;
                        int x_gap = 98;
                        int y_gap = 32;
                        int[] temp_x = new int[10] { 9, 0, 1, 2, 3, 4, 5, 6, 7, 8 };
                        int idx = 0;
                        for (int i = 0; i < cnt; i++)
                        {

                            System.Windows.Forms.Control[] Cmd_Bt = new Control[cnt];

                            Cmd_Bt[i] = new C1.Win.C1Input.C1Button();                  //버튼 객체 생성
                            Cmd_Bt[i].BackgroundImage = btnCreate.BackgroundImage;
                            Cmd_Bt[i].Cursor = Cursors.Hand;
                            Cmd_Bt[i].Name = "button" + i.ToString();
                            Cmd_Bt[i].Text = dt.Rows[i]["ENT_NM"].ToString();              //버튼 이름 설정

                            Cmd_Bt[i].Parent = groupBox2;

                            if ((i + 1) % 10 == 1 && i > 9) y += y_gap;

                            idx = (i + 1) % 10;
                            Cmd_Bt[i].Location = new Point(x + (x_gap * temp_x[idx]), y);

                            Cmd_Bt[i].Size = new Size(80, 24);         //버튼 크기 설정
                            Cmd_Bt[i].Tag = dt.Rows[i]["ENT_CD"].ToString();
                            Cmd_Bt[i].Click += new System.EventHandler(this.button_Click);  //버튼 이벤트 설정

                        }
                        if (cnt > 10)
                        {
                            if (cnt % 10 == 0)
                                panel1.Height = 112 + y_gap * ((cnt / 10) - 1);
                            else
                                panel1.Height = 112 + y_gap * (cnt / 10);
                        }
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //데이터 조회 중 오류가 발생하였습니다.
                }
                this.Cursor = Cursors.Default;
            }
        }

        private void button_Click(object sender, System.EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                System.Windows.Forms.Button c = (System.Windows.Forms.Button)sender;

                lblBus_Nm.Text = c.Text;

                string strQuery1 = " usp_EIS206  @pTYPE = 'S2' ";
                strQuery1 += " ,@pYEAR ='" + dtpYear.Text + "' ";
                strQuery1 += " ,@pENT_CD='" + c.Tag + "' ";
                strQuery1 += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                //				DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery1);
                DataTable dt = SystemBase.DbOpen.TranDataTable2(strQuery1);
                if (dt.Rows.Count <= 0 || dt.Rows[0][0].ToString() == "ER")
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn("P0019"));

                    ChartDataSeriesCollection coll = c1Chart1.ChartGroups[0].ChartData.SeriesList;
                    coll[0].PointData.Clear();
                    coll[1].PointData.Clear();

                    Axis ax = c1Chart1.ChartArea.AxisX;
                    ax.ValueLabels.Clear();
                }
                else
                {
                    // initialize chart
                    c1Chart1.Legend.Visible = true;
                    //					c1Chart1.Legend.Style.Border.BorderStyle =  C1.Win.C1Chart.BorderStyleEnum.Solid ;
                    c1Chart1.Legend.Compass = CompassEnum.East;

                    //label clear
                    ChartLabels cl = c1Chart1.ChartLabels;
                    C1.Win.C1Chart.LabelsCollection clc = cl.LabelsCollection;
                    clc.Clear();

                    DataView dv = dt.DefaultView;
                    BindSeries(c1Chart1, 0, dv, "표준공수", "PROJECT_NO");
                    BindSeries(c1Chart1, 1, dv, "실적공수");

                    FarPoint.Win.Spread.CellType.NumberCellType num = new FarPoint.Win.Spread.CellType.NumberCellType();
                    num.DecimalSeparator = ".";
                    num.DecimalPlaces = 0;
                    num.FixedPoint = true;
                    num.Separator = ",";
                    num.ShowSeparator = true;
                    num.MaximumValue = 99999999999999;
                    num.MinimumValue = 0;
                    fpSpread1.Sheets[0].ColumnCount = dt.Rows.Count + 1;

                    fpSpread1.Sheets[0].RowCount = 3;
                    fpSpread1.Sheets[0].RowHeader.Cells[0, 0].Text = "표준공수";
                    fpSpread1.Sheets[0].RowHeader.Cells[1, 0].Text = "실적공수";
                    fpSpread1.Sheets[0].RowHeader.Cells[2, 0].Text = "공수차이";

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        fpSpread1.Sheets[0].Columns[i + 1].CellType = num;
                        fpSpread1.Sheets[0].Columns[i + 1].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;
                        fpSpread1.Sheets[0].Columns[i + 1].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
                        fpSpread1.Sheets[0].Columns[i + 1].Locked = true;
                        fpSpread1.Sheets[0].Columns[i + 1].BackColor = Color.White;
                        fpSpread1.Sheets[0].Columns[i + 1].Width = 80;

                        fpSpread1.Sheets[0].ColumnHeader.Cells[0, i + 1].Text = dt.Rows[i]["PROJECT_NO"].ToString();

                        fpSpread1.Sheets[0].Cells[0, i + 1].Value = dt.Rows[i]["표준공수"];
                        fpSpread1.Sheets[0].Cells[1, i + 1].Value = dt.Rows[i]["실적공수"];
                        fpSpread1.Sheets[0].Cells[2, i + 1].Value = Convert.ToInt32(dt.Rows[i]["표준공수"]) - Convert.ToInt32(dt.Rows[i]["실적공수"]);
                    }

                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
            this.Cursor = Cursors.Default;
        }
        #endregion

        #region c1Chart
        // copy data from a data source to the chart
        // c1c          chart
        // series       index of the series to bind (0-based, will add if necessary)
        // datasource   datasource object (cannot be DataTable, DataView is OK)
        // field        name of the field that contains the y values
        // labels       name of the field that contains the x labels
        private void BindSeries(C1Chart c1c, int series, object dataSource, string field, string labels)
        {
            // check data source object
            ITypedList il = (ITypedList)dataSource;
            IList list = (IList)dataSource;
            if (list == null || il == null)
                throw new ApplicationException("Invalid DataSource object.");

            // add series if necessary
            ChartDataSeriesCollection coll = c1c.ChartGroups[0].ChartData.SeriesList;
            while (series >= coll.Count)
                coll.AddNewSeries();

            // copy series data
            if (list.Count == 0) return;
            PointF[] data = (PointF[])Array.CreateInstance(typeof(PointF), list.Count);
            PropertyDescriptorCollection pdc = il.GetItemProperties(null);
            PropertyDescriptor pd = pdc[field];
            if (pd == null)
                throw new ApplicationException(string.Format("Invalid field name used for Y values ({0}).", field));

            int i;
            for (i = 0; i < list.Count; i++)
            {
                data[i].X = i;
                try
                {
                    data[i].Y = float.Parse(pd.GetValue(list[i]).ToString());

                }
                catch
                {
                    data[i].Y = float.NaN;
                }
                coll[series].PointData.CopyDataIn(data);
                coll[series].Label = field;


                // Add Chart Labels
                //				ChartLabels cl = c1c.ChartLabels;
                //
                //				C1.Win.C1Chart.LabelsCollection clc = cl.LabelsCollection;
                //				C1.Win.C1Chart.Label lab = clc.AddNewLabel();			
                //				
                //				lab.Text = SystemBase.Base.Comma2(pd.GetValue(list[i]).ToString()); 
                //				lab.Style.ForeColor = Color.Black;
                //				lab.Style.HorizontalAlignment = AlignHorzEnum.Far;
                //				lab.Style.VerticalAlignment = AlignVertEnum.Center;
                //				lab.AttachMethod = AttachMethodEnum.DataIndex;
                //				lab.AttachMethodData.GroupIndex = 0;
                //				lab.AttachMethodData.SeriesIndex = series;
                //				lab.AttachMethodData.PointIndex = i;
                //				lab.Compass = LabelCompassEnum.West;
                //				lab.Offset = -2;
                //				lab.Visible = true;
            }

            // copy series labels
            if (labels != null && labels.Length > 0)
            {
                pd = pdc[labels];
                if (pd == null)
                    throw new ApplicationException(string.Format("Invalid field name used for X values ({0}).", labels));
                Axis ax = c1c.ChartArea.AxisX;
                //				ax.AnnotationRotation =  -30;
                ax.ValueLabels.Clear();
                for (i = 0; i < list.Count; i++)
                {
                    string label = pd.GetValue(list[i]).ToString();
                    ax.ValueLabels.Add(i, label);
                }
                ax.AnnoMethod = AnnotationMethodEnum.ValueLabels;
            }
        }


        private void BindSeries(C1Chart c1c, int series, object dataSource, string field)
        {
            BindSeries(c1c, series, dataSource, field, null);
        }
        #endregion
    }
}
