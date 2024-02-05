#region 작성정보
/*********************************************************************/
// 단위업무명 : 사업별 재공금액 현황
// 작 성 자 : 권 순 철
// 작 성 일 : 2013-04-15
// 작성내용 : 사업별 재공금액 현황 및 관리
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

namespace EI.EIS204
{
    public partial class EIS204 : UIForm.FPCOMM1
    {
        public EIS204()
        {
            InitializeComponent();
        }

        #region Form Load시
        private void EIS204_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            rdoTop.Checked = true;

            ChartDataSeriesCollection coll = c1Chart1.ChartGroups[0].ChartData.SeriesList;
            coll[0].PointData.Clear();
            coll[1].PointData.Clear();
            coll[2].PointData.Clear();

            Axis ax = c1Chart1.ChartArea.AxisX;
            ax.ValueLabels.Clear();

            fpSpread1.Sheets[0].RowHeader.Columns[0].Width = 70;
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            rdoTop.Checked = true;

            ChartDataSeriesCollection coll = c1Chart1.ChartGroups[0].ChartData.SeriesList;
            coll[0].PointData.Clear();
            coll[1].PointData.Clear();
            coll[2].PointData.Clear();

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
                int sum1 = 0;
                int[] sum2 = { 0, 0, 0, 0 };
                try
                {
                    string strType = "S1";

                    if (rdoAll.Checked == true) strType = "S2";
                    string strQuery1 = " usp_EIS204  @pTYPE = '" + strType + "' ";
                    strQuery1 += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    //				DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery1);
                    DataTable dt = SystemBase.DbOpen.TranDataTable2(strQuery1);
                    if (dt.Rows.Count <= 0)
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
                        BindSeries(c1Chart1, 0, dv, "노무비", "사업");
                        BindSeries(c1Chart1, 1, dv, "재료비");
                        BindSeries(c1Chart1, 2, dv, "경비");

                        fpSpread1.Sheets[0].RowCount = 4;
                        fpSpread1.Sheets[0].ColumnCount = dt.Rows.Count + 2;

                        FarPoint.Win.Spread.CellType.NumberCellType num = new FarPoint.Win.Spread.CellType.NumberCellType();
                        num.DecimalSeparator = ".";
                        num.DecimalPlaces = 0;
                        num.FixedPoint = true;
                        num.Separator = ",";
                        num.ShowSeparator = true;
                        num.MaximumValue = 99999999999999;
                        num.MinimumValue = 0;

                        fpSpread1.Sheets[0].RowHeader.Cells[0, 0].Text = "노무비";
                        fpSpread1.Sheets[0].RowHeader.Cells[1, 0].Text = "재료비";
                        fpSpread1.Sheets[0].RowHeader.Cells[2, 0].Text = "경비";
                        fpSpread1.Sheets[0].RowHeader.Cells[3, 0].Text = "합계";
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            fpSpread1.Sheets[0].Columns[i + 1].CellType = num;
                            fpSpread1.Sheets[0].Columns[i + 1].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;
                            fpSpread1.Sheets[0].Columns[i + 1].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
                            fpSpread1.Sheets[0].Columns[i + 1].Locked = true;
                            fpSpread1.Sheets[0].Columns[i + 1].BackColor = Color.White;
                            fpSpread1.Sheets[0].Columns[i + 1].Width = 60;

                            fpSpread1.Sheets[0].ColumnHeader.Cells[0, i + 1].Text = dt.Rows[i]["사업"].ToString();
                            fpSpread1.Sheets[0].Cells[0, i + 1].Value = dt.Rows[i]["노무비"];
                            fpSpread1.Sheets[0].Cells[1, i + 1].Value = dt.Rows[i]["재료비"];
                            fpSpread1.Sheets[0].Cells[2, i + 1].Value = dt.Rows[i]["경비"];

                            sum1 = Convert.ToInt16(dt.Rows[i]["노무비"]) + Convert.ToInt16(dt.Rows[i]["재료비"]) + Convert.ToInt16(dt.Rows[i]["경비"]);

                            fpSpread1.Sheets[0].Cells[3, i + 1].Value = sum1;

                            sum2[0] += Convert.ToInt16(dt.Rows[i]["노무비"]);
                            sum2[1] += Convert.ToInt16(dt.Rows[i]["재료비"]);
                            sum2[2] += Convert.ToInt16(dt.Rows[i]["경비"]);
                            sum2[3] += sum1;

                        }

                        int col = fpSpread1.Sheets[0].ColumnCount - 1;
                        fpSpread1.Sheets[0].Columns[col].CellType = num;
                        fpSpread1.Sheets[0].Columns[col].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;
                        fpSpread1.Sheets[0].Columns[col].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
                        fpSpread1.Sheets[0].Columns[col].Locked = true;
                        fpSpread1.Sheets[0].Columns[col].BackColor = Color.White;
                        fpSpread1.Sheets[0].Columns[col].Width = 60;

                        fpSpread1.Sheets[0].ColumnHeader.Cells[0, col].Text = "합계";
                        fpSpread1.Sheets[0].Cells[0, col].Value = sum2[0];
                        fpSpread1.Sheets[0].Cells[1, col].Value = sum2[1];
                        fpSpread1.Sheets[0].Cells[2, col].Value = sum2[2];
                        fpSpread1.Sheets[0].Cells[3, col].Value = sum2[3];
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

        #region 셀클릭 이벤트
        private void fpSpread1_CellDoubleClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            string EntNm = "";
            EntNm = fpSpread1.Sheets[0].ColumnHeader.Cells[0, e.Column].Text.Trim();

            string Query = "SELECT TOP 1 ENT_CD FROM S_ENTERPRISE_INFO(NOLOCK) WHERE ENT_NM = '" + EntNm + "' ";
            DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);

            if (dt.Rows.Count > 0)
            {
                string entCd = "";
                entCd = dt.Rows[0][0].ToString();

                EIS204P1 form = new EIS204P1(entCd, EntNm);
                form.ShowDialog();
            }
            else
            {
                MessageBox.Show("[" + EntNm + "] 은(는) 잘못된 사업명입니다. ", SystemBase.Base.MessageRtn("Z002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion
    }
}
