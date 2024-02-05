#region 작성정보
/*********************************************************************/
// 단위업무명 : 생산부하현황(일자별)
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-04-24
// 작성내용 : 생산부하현황(일자별) 및 관리
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
using System.Text.RegularExpressions;
using System.Collections;

namespace EI.PBC107
{
    public partial class PBC107 : UIForm.FPCOMM1
    {
        #region 생성자
        public PBC107()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load시
        private void PBC107_Load(object sender, System.EventArgs e)
        {
            //필수 체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            dtpStartDT.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD").ToString()).ToShortDateString().Substring(0, 10);
            dtpEndDT.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD").ToString()).AddMonths(1).ToShortDateString().Substring(0, 10);

            ChartDataSeriesCollection coll = c1Chart1.ChartGroups[0].ChartData.SeriesList;
            coll[0].PointData.Clear();
            coll[1].PointData.Clear();
            coll[2].PointData.Clear();

            rdoPlot.Tag = Chart2DTypeEnum.XYPlot;
            rdoBar.Tag = Chart2DTypeEnum.Bar;

            rdoBar.Checked = true;
            c1Chart1.ChartGroups[0].ChartType = (Chart2DTypeEnum)rdoBar.Tag;

            Axis ax = c1Chart1.ChartArea.AxisX;
            ax.ValueLabels.Clear();
        }
        #endregion

        #region NewExec
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            dtpStartDT.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD").ToString()).ToShortDateString().Substring(0, 10);
            dtpEndDT.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD").ToString()).AddMonths(1).ToShortDateString().Substring(0, 10);

            ChartDataSeriesCollection coll = c1Chart1.ChartGroups[0].ChartData.SeriesList;
            coll[0].PointData.Clear();
            coll[1].PointData.Clear();

            Axis ax = c1Chart1.ChartArea.AxisX;
            ax.ValueLabels.Clear();
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                string div = "1";

                if (rdoWeek.Checked == true) div = "2";
                else if (rdoMonth.Checked == true) div = "3";

                string strQuery1 = " usp_PBC106 @pType= 'P040', ";
                strQuery1 += " @pST_DT='" + dtpStartDT.Text + "', ";
                strQuery1 += " @pED_DT='" + dtpEndDT.Text + "', ";
                strQuery1 += " @pWC_CD='" + txtWORKCENTER_CD.Text + "', ";
                strQuery1 += " @pGRES_CD ='" + txtGRES_CD.Text + "',";
                strQuery1 += " @pRES_CD ='" + txtResCd.Text + "',";
                strQuery1 += " @pDIV ='" + div + "'";
                strQuery1 += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                DataTable dt = SystemBase.DbOpen.TranDataTable2(strQuery1);
                if (dt.Rows.Count <= 0)
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn("P0019"));

                    ChartDataSeriesCollection coll = c1Chart1.ChartGroups[0].ChartData.SeriesList;
                    coll[0].PointData.Clear();
                    coll[1].PointData.Clear();
                    coll[2].PointData.Clear();

                    Axis ax = c1Chart1.ChartArea.AxisX;
                    ax.ValueLabels.Clear();
                }
                else
                {
                    if (dt.Rows[0][0].ToString() == "ER")
                    {
                        MessageBox.Show(dt.Rows[0][1].ToString(), SystemBase.Base.MessageRtn("Z002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                        this.Cursor = Cursors.Default;
                        return;
                    }
                    // initialize chart
                    c1Chart1.Legend.Visible = true;
                    c1Chart1.Legend.Compass = CompassEnum.East;

                    //label clear
                    ChartLabels cl = c1Chart1.ChartLabels;
                    C1.Win.C1Chart.LabelsCollection clc = cl.LabelsCollection;
                    clc.Clear();

                    DataView dv = dt.DefaultView;
                    BindSeries(c1Chart1, 0, dv, "기준능력", "일자");
                    BindSeries(c1Chart1, 1, dv, "기준+OT능력");
                    BindSeries(c1Chart1, 2, dv, "부하공수");


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

                    fpSpread1.Sheets[0].RowHeader.Cells[0, 0].Text = "기준능력";
                    fpSpread1.Sheets[0].RowHeader.Cells[1, 0].Text = "기준+OT능력";
                    fpSpread1.Sheets[0].RowHeader.Cells[2, 0].Text = "부하공수";
                    fpSpread1.Sheets[0].RowHeader.Columns[0].Width = 100;
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        fpSpread1.Sheets[0].ColumnHeader.Cells[0, i + 1].Text = dt.Rows[i]["일자"].ToString();

                        fpSpread1.Sheets[0].Columns[i + 1].CellType = num;
                        fpSpread1.Sheets[0].Columns[i + 1].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;
                        fpSpread1.Sheets[0].Columns[i + 1].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
                        fpSpread1.Sheets[0].Columns[i + 1].Locked = true;
                        fpSpread1.Sheets[0].Columns[i + 1].BackColor = Color.White;
                        fpSpread1.Sheets[0].Columns[i + 1].Width = 60;

                        fpSpread1.Sheets[0].Cells[0, i + 1].Value = dt.Rows[i]["기준능력"];
                        fpSpread1.Sheets[0].Cells[1, i + 1].Value = dt.Rows[i]["기준+OT능력"];
                        fpSpread1.Sheets[0].Cells[2, i + 1].Value = dt.Rows[i]["부하공수"];
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
            }

            // copy series labels
            if (labels != null && labels.Length > 0)
            {
                pd = pdc[labels];
                if (pd == null)
                    throw new ApplicationException(string.Format("Invalid field name used for X values ({0}).", labels));
                Axis ax = c1c.ChartArea.AxisX;
                ax.AnnotationRotation = -30;
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
        
        #region  팝업
        private void btnWORKCENTER_CD_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pType='P042', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pETC = 'P002' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };
                string[] strSearch = new string[] { txtWORKCENTER_CD.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00025", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "작업장 조회");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtWORKCENTER_CD.Value = Msgs[0].ToString();
                    txtWORKCENTER_NM.Value = Msgs[1].ToString();
                    txtWORKCENTER_CD.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f);
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnGRES_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pType='P052', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCOM_NM" };
                string[] strSearch = new string[] { txtGRES_NM.Text };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00060", strQuery, strWhere, strSearch, new int[] { 1 }, "자원그룹 조회", true);
                pu.Width = 500;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtGRES_CD.Value = Msgs[0].ToString();
                    txtGRES_NM.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log("자원그룹 조회 : ", f);
                MessageBox.Show(f.Message);
            }
        }

        private void btnRes_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = "";
                if (txtGRES_CD.Text.Trim() == "")
                    strQuery = " usp_P_COMMON @pType='P056', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                else
                    strQuery = " usp_P_COMMON @pType='P058', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pETC = '" + txtGRES_CD.Text.Trim() + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };
                string[] strSearch = new string[] { txtResCd.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00068", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "자원 조회");
                pu.Width = 500;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtResCd.Value = Msgs[0].ToString();
                    txtResNm.Value = Msgs[1].ToString();
                    txtResCd.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "자원 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region  TextChanged
        private void txtWORKCENTER_CD_TextChanged(object sender, System.EventArgs e)
        {
            string strSql = "and LANG_CD = '" + SystemBase.Base.gstrLangCd + "' and MAJOR_CD = 'P002' AND COMP_CODE = '" + SystemBase.Base.gstrCOMCD + "'";
            try
            {
                if (txtWORKCENTER_CD.Text != "")
                {
                    txtWORKCENTER_NM.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtWORKCENTER_CD.Text, strSql);
                }
                else
                {
                    txtWORKCENTER_NM.Value = "";
                }
            }
            catch
            {

            }
        }

        private void txtGRES_CD_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtGRES_CD.Text != "")
                {
                    txtGRES_NM.Value = SystemBase.Base.CodeName("RES_CD", "RES_DIS", "P_RESO_MANAGE", txtGRES_CD.Text, " AND RES_KIND = 'G'  AND USE_YN = 'Y' AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "' ");
                }
                else
                {
                    txtGRES_NM.Value = "";
                }
            }
            catch
            {

            }
        }

        private void txtResCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtResCd.Text != "")
                {
                    txtResNm.Value = SystemBase.Base.CodeName("RES_CD", "RES_DIS", "P_RESO_MANAGE", txtResCd.Text, " AND USE_YN = 'Y' AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "' ");
                }
                else
                {
                    txtResNm.Value = "";
                }
            }
            catch
            {

            }
        }
        #endregion
        
        #region 그래프유형선택
        private void radioChartTypeChanged(object sender, System.EventArgs e)
        {
            RadioButton rb = (RadioButton)sender;
            if (rb.Checked)
            {
                Chart2DTypeEnum ct = (Chart2DTypeEnum)rb.Tag;
                c1Chart1.ChartGroups[0].ChartType = ct;
            }
        }

        private void radioDivChanged(object sender, System.EventArgs e)
        {
            RadioButton rb = (RadioButton)sender;
            if (rb.Checked)
            {
                if (rb.Name == "rdoDay")
                {
                    dtpEndDT.Text = Convert.ToDateTime(dtpStartDT.Text).AddMonths(1).ToShortDateString().Substring(0, 10);
                }
                else
                {
                    dtpEndDT.Text = Convert.ToDateTime(dtpStartDT.Text).AddMonths(3).ToShortDateString().Substring(0, 10);
                }
            }
        }
        #endregion

    }
}
