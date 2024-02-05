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
    public partial class EIS101P12 : UIForm.FPCOMM1
    {
        #region 변수선언
        string strProjNo = "";
        string strProjSeq = "";
        string strItemCd = "";
        string strGi_Dt = "";
        #endregion

        public EIS101P12(string P_No, string P_Seq, string P_item, string Gi_Dt)
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

        public EIS101P12()
        {
            InitializeComponent();
        }

        #region Form Load 시
        private void EIS101P12_Load(object sender, System.EventArgs e)
        {
            this.Text = "프로젝트별 제품완성율";
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            UIForm.Buttons.ReButton("010000001001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);
            txtProject_No.Value = strProjNo;
            txtProject_Seq.Value = strProjSeq;
            txtItem_Cd.Value = strItemCd;
            // 
            c1Chart1.Header.Visible = true;
            c1Chart1.Header.Text = "프로젝트별 제품완성율";
            c1Chart1.Header.Compass = C1.Win.C1Chart.CompassEnum.North;

            //
            ChartDataSeriesCollection coll1 = c1Chart1.ChartGroups[0].ChartData.SeriesList;
            coll1[0].PointData.Clear();
            coll1[1].PointData.Clear();


            Axis ax1 = c1Chart1.ChartArea.AxisX;
            ax1.ValueLabels.Clear();

            Search(false);
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            Search(true);
        }

        private void Search(bool msg)
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            try
            {
                string strQuery = " usp_EIS101  @pTYPE = 'S12'";
                strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' ";
                strQuery += ", @pPROJECT_NO = '" + txtProject_No.Text + "' ";
                strQuery += ", @pPROJECT_SEQ = '" + txtProject_Seq.Text + "' ";
                strQuery += ", @pITEM_CD= '" + strItemCd + "' ";
                strQuery += ", @pGI_DT  = '" + strGi_Dt + "' ";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, msg, 0, 0, true);
                fpSpread1.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.Normal;

                //				int i0 = SystemBase.Base.GridHeadIndex(GHIdx1, "제품");
                //				int i1 = SystemBase.Base.GridHeadIndex(GHIdx1, "계획");
                //				int i2 = SystemBase.Base.GridHeadIndex(GHIdx1, "실적");
                //				int i3 = SystemBase.Base.GridHeadIndex(GHIdx1, "사내(총부하)");
                //				for(int i = 1; i < fpSpread1.Sheets[0].Rows.Count; i++) //누적
                //				{
                //					if(fpSpread1.Sheets[0].Cells[i-1,i0].Text == fpSpread1.Sheets[0].Cells[i,i0].Text) 
                //					{
                //						fpSpread1.Sheets[0].Cells[i,i1].Value = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i-1,i1].Value) + Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i,i1].Value);
                //						fpSpread1.Sheets[0].Cells[i,i2].Value = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i-1,i2].Value) + Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i,i2].Value);
                //						fpSpread1.Sheets[0].Cells[i,i3].Value = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i-1,i3].Value) + Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i,i3].Value);
                //
                //					}
                //				}

                string div = "1";

                if (rdoWeek.Checked == true) div = "2";
                else if (rdoMonth.Checked == true) div = "3";


                strQuery = " usp_EIS101  @pTYPE = 'S14'";
                strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' ";
                strQuery += ", @pPROJECT_NO = '" + txtProject_No.Text + "' ";
                strQuery += ", @pPROJECT_SEQ = '" + txtProject_Seq.Text + "' ";
                strQuery += ", @pITEM_CD= '" + strItemCd + "' ";
                strQuery += ", @pGI_DT  = '" + strGi_Dt + "' ";
                strQuery += ", @pDIV  = '" + div + "' ";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

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

                    BindSeries(c1Chart1, 0, dv, "계획(완성률)", "일자");
                    BindSeries(c1Chart1, 1, dv, "완성(완성률)");
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
                /*
                ChartLabels cl = c1c.ChartLabels;

                C1.Win.C1Chart.LabelsCollection clc = cl.LabelsCollection;
                C1.Win.C1Chart.Label lab = clc.AddNewLabel();				
				
                lab.Text = pd.GetValue(list[i]).ToString(); 
                lab.Style.ForeColor = Color.Black;
                lab.Style.HorizontalAlignment = AlignHorzEnum.Far;  
                lab.Style.VerticalAlignment = AlignVertEnum.Center;
                lab.AttachMethod = AttachMethodEnum.DataIndex;
                lab.AttachMethodData.GroupIndex = 0;
                lab.AttachMethodData.SeriesIndex = series;
                lab.AttachMethodData.PointIndex = i;
                lab.Compass = LabelCompassEnum.West;
                lab.Offset = -2;
                lab.Visible = true;
                */
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

        private void BindSeries(C1Chart c1c, int series, object dataSource, string field)
        {
            BindSeries(c1c, series, dataSource, field, null);
        }
        #endregion
    }
}
