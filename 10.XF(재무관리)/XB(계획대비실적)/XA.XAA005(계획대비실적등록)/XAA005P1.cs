#region 작성정보
/*********************************************************************/
// 단위업무명 : 계획대비실적현황
// 작 성 자 : 김 현 근
// 작 성 일 : 2013-04-09
// 작성내용 : 계획대비실적현황
// 수 정 일 :
// 수 정 자 :
// 수정내용 :
// 비    고 :
/*********************************************************************/
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using C1.Win.C1Chart;
namespace XA.XAA005
{
    public partial class XAA005P1 : UIForm.FPCOMM1
    {
        #region 변수선언
        string strYear = "";
        #endregion

        public XAA005P1()
        {
            InitializeComponent();
        }
        public XAA005P1(string Year)
        {
            strYear = Year;
            InitializeComponent();
        }

        #region Form Load 시
        private void XAA005P1_Load(object sender, EventArgs e)
        {
            this.Text = "계획대비분석";
          
            UIForm.Buttons.ReButton("000000001001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);
              
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            txtYear.Value = strYear;

            ChartDataSeriesCollection coll = c1Chart1.ChartGroups[0].ChartData.SeriesList;
            coll[0].PointData.Clear();
            coll[1].PointData.Clear();

            Axis ax = c1Chart1.ChartArea.AxisX;
            ax.ValueLabels.Clear();

            fpSpread1.Sheets[0].RowHeader.Columns[0].Width = 60;
            SearchExec();
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                string strQuery1 = " usp_XAA005   @pTYPE = 'S2' ";
                strQuery1 += " ,@pYEAR ='" + txtYear.Text + "' ";
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
                    BindSeries(c1Chart1, 0, dv, "계획", "월");
                    BindSeries(c1Chart1, 1, dv, "실적");

                    decimal sum_plan = 0;

                    fpSpread1.Sheets[0].RowCount = 4;
                    fpSpread1.Sheets[0].RowHeader.Cells[0, 0].Text = "계획";
                    fpSpread1.Sheets[0].RowHeader.Cells[1, 0].Text = "실적";
                    fpSpread1.Sheets[0].RowHeader.Cells[2, 0].Text = "차액";
                    fpSpread1.Sheets[0].RowHeader.Cells[3, 0].Text = "%";

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (dt.Rows[i]["계획"].ToString() != "0")
                            fpSpread1.Sheets[0].Cells[0, i + 1].Value = dt.Rows[i]["계획"];

                        if (dt.Rows[i]["실적"].ToString() != "0")
                        {
                            fpSpread1.Sheets[0].Cells[1, i + 1].Value = dt.Rows[i]["실적"];
                            if (i != 12) sum_plan += Convert.ToDecimal(dt.Rows[i]["계획"]);
                            fpSpread1.Sheets[0].Cells[2, i + 1].Value = dt.Rows[i]["차이"];
                            if (i < 12)
                            {
                                if(Convert.ToDecimal(dt.Rows[i]["계획"]) != 0)
                                    fpSpread1.Sheets[0].Cells[3, i + 1].Value = Convert.ToDecimal(dt.Rows[i]["실적"]) / Convert.ToDecimal(dt.Rows[i]["계획"]) * 100;
                            }
                            else
                            {
                                if (Convert.ToDecimal(dt.Rows[i]["실적"]) != 0)
                                    fpSpread1.Sheets[0].Cells[3, i + 1].Value = Convert.ToDecimal(dt.Rows[i]["실적"]) / sum_plan * 100;
                            }
                        }
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
            System.Collections.IList list = (System.Collections.IList)dataSource;
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

        #region NewExec() 그리드 및 그룹박스 초기화
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            ChartDataSeriesCollection coll = c1Chart1.ChartGroups[0].ChartData.SeriesList;
            coll[0].PointData.Clear();
            coll[1].PointData.Clear();

            Axis ax = c1Chart1.ChartArea.AxisX;
            ax.ValueLabels.Clear();

            fpSpread1.Sheets[0].RowCount = 0;
        }
        #endregion

    }
}
