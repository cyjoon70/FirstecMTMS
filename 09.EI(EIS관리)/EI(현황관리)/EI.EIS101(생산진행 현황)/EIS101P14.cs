using System;
using System.Drawing;
using System.Data;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using C1.Win.C1Chart;

namespace EI.EIS101
{
    public partial class EIS101P14 : UIForm.Buttons
    {
        #region 변수선언
        string strProjNo = "";
        string strProjSeq = "";
        string strItemCd = "";
        string strGi_Dt = "";
        #endregion

        public EIS101P14(string P_No, string P_Seq, string P_item, string Gi_Dt)
        {
            //
            // Windows Form 디자이너 지원에 필요합니다.
            //
            strProjNo = P_No;
            strProjSeq = P_Seq;
            strItemCd = P_item;
            strGi_Dt = Gi_Dt;
            InitializeComponent();

            //
            // TODO: InitializeComponent를 호출한 다음 생성자 코드를 추가합니다.
            //
        }

        public EIS101P14()
        {
            InitializeComponent();
        }

        #region Form Load 시
        private void EIS101P14_Load(object sender, System.EventArgs e)
        {

            this.Text = "원가현황";
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            UIForm.Buttons.ReButton("000000001001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);
            txtProject_No.Value = strProjNo;
            txtProject_Seq.Value = strProjSeq;
            txtItem_Cd.Value = strItemCd;

            //
            init_Chart();
            SearchExec();
        }
        #endregion

        private void init_Chart()
        {
            //c1Chart1
            ChartDataSeriesCollection coll1 = c1Chart1.ChartGroups[0].ChartData.SeriesList;
            coll1[0].PointData.Clear();
            coll1[1].PointData.Clear();

            Axis ax1 = c1Chart1.ChartArea.AxisX;
            ax1.ValueLabels.Clear();

            //c1Chart2
            ChartDataSeriesCollection coll2 = c1Chart2.ChartGroups[0].ChartData.SeriesList;
            coll2[0].PointData.Clear();
            coll2[1].PointData.Clear();

            Axis ax2 = c1Chart2.ChartArea.AxisX;
            ax2.ValueLabels.Clear();

            //c1Chart3
            ChartDataSeriesCollection coll3 = c1Chart3.ChartGroups[0].ChartData.SeriesList;
            coll3[0].PointData.Clear();
            coll3[1].PointData.Clear();

            Axis ax3 = c1Chart3.ChartArea.AxisX;
            ax3.ValueLabels.Clear();

            //c1Chart4
            ChartDataSeriesCollection coll4 = c1Chart4.ChartGroups[0].ChartData.SeriesList;
            coll4[0].PointData.Clear();
            coll4[1].PointData.Clear();

            Axis ax4 = c1Chart4.ChartArea.AxisX;
            ax4.ValueLabels.Clear();
        }

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            try
            {
                //				string strQuery = " usp_EIS101  @pTYPE = 'S13'";
                //				strQuery += ", @pLANG_CD = '"+ SystemBase.Base.gstrLangCd +"' ";
                //				strQuery += ", @pPROJECT_NO = '"+  txtProject_No.Text +"' ";
                //				strQuery += ", @pPROJECT_SEQ = '"+  txtProject_Seq.Text +"' ";
                //				strQuery += ", @pITEM_CD= '"+  strItemCd +"' ";
                //				strQuery += ", @pGI_DT  = '"+  strGi_Dt +"' ";
                //
                //				UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0, true );
                //				fpSpread1.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.Normal;	

                string strQuery1 = " usp_EIS101  @pTYPE = 'S16'";
                string strQuery2 = " usp_EIS101  @pTYPE = 'S17'";
                string strQuery3 = " usp_EIS101  @pTYPE = 'S18'";
                string strQuery4 = " usp_EIS101  @pTYPE = 'S19'";

                string strQuery = ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' ";
                strQuery += ", @pPROJECT_NO = '" + txtProject_No.Text + "' ";
                strQuery += ", @pPROJECT_SEQ = '" + txtProject_Seq.Text + "' ";
                strQuery += ", @pITEM_CD= '" + strItemCd + "' ";
                strQuery += ", @pGI_DT  = '" + strGi_Dt + "' ";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery1 + strQuery);
                if (dt.Rows.Count > 0)
                {
                    // initialize chart
                    c1Chart1.Legend.Visible = true;
                    c1Chart1.Legend.Compass = CompassEnum.East;
                    c1Chart1.Legend.Style.Border.BorderStyle = C1.Win.C1Chart.BorderStyleEnum.Raised;

                    //label clear
                    ChartLabels cl = c1Chart1.ChartLabels;
                    C1.Win.C1Chart.LabelsCollection clc = cl.LabelsCollection;
                    clc.Clear();

                    DataView dv = dt.DefaultView;

                    BindSeries(c1Chart1, 0, dv, "계약공수", "구분");
                    BindSeries(c1Chart1, 1, dv, "실적공수");
                }
                dt = null;
                dt = SystemBase.DbOpen.NoTranDataTable(strQuery2 + strQuery);
                if (dt.Rows.Count > 0)
                {
                    // initialize chart
                    c1Chart2.Legend.Visible = true;
                    c1Chart2.Legend.Compass = CompassEnum.East;
                    c1Chart2.Legend.Style.Border.BorderStyle = C1.Win.C1Chart.BorderStyleEnum.Raised;

                    //label clear
                    ChartLabels cl = c1Chart2.ChartLabels;
                    C1.Win.C1Chart.LabelsCollection clc = cl.LabelsCollection;
                    clc.Clear();

                    DataView dv = dt.DefaultView;

                    BindSeries(c1Chart2, 0, dv, "계약금액", "구분");
                    BindSeries(c1Chart2, 1, dv, "실적금액");
                }

                dt = null;
                dt = SystemBase.DbOpen.NoTranDataTable(strQuery3 + strQuery);
                if (dt.Rows.Count > 0)
                {
                    // initialize chart
                    c1Chart3.Legend.Visible = true;
                    c1Chart3.Legend.Compass = CompassEnum.East;
                    c1Chart3.Legend.Style.Border.BorderStyle = C1.Win.C1Chart.BorderStyleEnum.Raised;

                    //label clear
                    ChartLabels cl = c1Chart3.ChartLabels;
                    C1.Win.C1Chart.LabelsCollection clc = cl.LabelsCollection;
                    clc.Clear();

                    DataView dv = dt.DefaultView;

                    BindSeries(c1Chart3, 0, dv, "계약금액", "구분");
                    BindSeries(c1Chart3, 1, dv, "실적금액");
                }

                dt = null;
                dt = SystemBase.DbOpen.NoTranDataTable(strQuery4 + strQuery);
                if (dt.Rows.Count > 0)
                {
                    // initialize chart
                    c1Chart4.Legend.Visible = true;
                    c1Chart4.Legend.Compass = CompassEnum.East;
                    c1Chart4.Legend.Style.Border.BorderStyle = C1.Win.C1Chart.BorderStyleEnum.Raised;

                    //label clear
                    ChartLabels cl = c1Chart4.ChartLabels;
                    C1.Win.C1Chart.LabelsCollection clc = cl.LabelsCollection;
                    clc.Clear();

                    DataView dv = dt.DefaultView;

                    BindSeries(c1Chart4, 0, dv, "계약금액", "구분");
                    BindSeries(c1Chart4, 1, dv, "실적금액");
                }


            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }

            this.Cursor = System.Windows.Forms.Cursors.Default;
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
                ChartLabels cl = c1c.ChartLabels;

                C1.Win.C1Chart.LabelsCollection clc = cl.LabelsCollection;
                C1.Win.C1Chart.Label lab = clc.AddNewLabel();

                lab.Text = string.Format("{0:#,###.##}", Convert.ToDecimal(pd.GetValue(list[i]).ToString()));
                lab.Style.ForeColor = Color.Black;
                lab.Style.HorizontalAlignment = AlignHorzEnum.Center;
                lab.Style.VerticalAlignment = AlignVertEnum.Center;
                lab.AttachMethod = AttachMethodEnum.DataIndex;
                lab.AttachMethodData.GroupIndex = 0;
                lab.AttachMethodData.SeriesIndex = series;
                lab.AttachMethodData.PointIndex = i;
                lab.Compass = LabelCompassEnum.West;
                lab.Offset = 0;
                lab.Visible = true;
            }

            // copy series labels
            if (labels != null && labels.Length > 0)
            {
                pd = pdc[labels];
                if (pd == null)
                    throw new ApplicationException(string.Format("Invalid field name used for X values ({0}).", labels));
                Axis ax = c1c.ChartArea.AxisX;
                //				ax.AnnotationRotation = 90;
                ax.ValueLabels.Clear();
                for (i = 0; i < list.Count; i++)
                {
                    string label = pd.GetValue(list[i]).ToString();
                    ax.ValueLabels.Add(i, label);
                }
                ax.AnnoMethod = AnnotationMethodEnum.ValueLabels;
            }
        }


        private void BindSeries(C1Chart c1c, int series, object dataSource, string field, int div)
        {
            // check data source object
            ITypedList il = (ITypedList)dataSource;
            IList list = (IList)dataSource;
            if (list == null || il == null)
                throw new ApplicationException("Invalid DataSource object.");

            // add series if necessary
            ChartDataSeriesCollection coll = c1c.ChartGroups[1].ChartData.SeriesList;
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
            }
        }

        private void BindSeries(C1Chart c1c, int series, object dataSource, string field)
        {
            BindSeries(c1c, series, dataSource, field, null);
        }
        #endregion
    }
}
