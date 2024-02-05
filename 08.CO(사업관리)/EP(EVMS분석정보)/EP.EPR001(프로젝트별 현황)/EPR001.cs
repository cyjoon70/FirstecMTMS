#region 작성정보
/*********************************************************************/
// 단위업무명 : 프로젝트별 현황
// 작 성 자 : 권 순 철
// 작 성 일 : 2013-04-12
// 작성내용 : 프로젝트별 현황 및 관리
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
using WNDW;

namespace EP.EPR001
{
    public partial class EPR001 : UIForm.FPCOMM1
    {
        #region 변수선언
        string strProjectNo = "";
        string strProjectNo1 = "";
        string strProjectNo2 = "";
        string strProjectNo3 = "";
        string strProjectNo4 = "";
        string strProjectNo5 = "";
        string strProjectNo6 = "";
        string strProjectNo7 = "";
        string strProjectNo8 = "";
        string strProjectNo9 = "";
        int TempRow = 10000;
        #endregion

        public EPR001()
        {
            InitializeComponent();
        }

        #region Form Load시
        private void EPR001_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            c1DockingTab1.SelectedIndex = 0;
            Set_Chart_Hearder();
            Set_Chart_Clear();
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            fpSpread1.Sheets[0].RowCount = 0;

            TempRow = 10000;
            strProjectNo = "";
            strProjectNo1 = "";
            strProjectNo2 = "";
            strProjectNo3 = "";
            strProjectNo4 = "";
            strProjectNo5 = "";
            strProjectNo6 = "";
            strProjectNo7 = "";
            strProjectNo8 = "";
            strProjectNo9 = "";

            c1DockingTab1.SelectedIndex = 0;
            Set_Chart_Hearder();
            Set_Chart_Clear();
        }
        #endregion

        #region Set_Chart_Hearder
        private void Set_Chart_Hearder()
        {
            c1Chart1.Header.Visible = true;
            c1Chart1.Header.Text = "비용편차" + Environment.NewLine + "(Cost Variance)";
            c1Chart1.Header.Compass = C1.Win.C1Chart.CompassEnum.North;

            c1Chart2.Header.Visible = true;
            c1Chart2.Header.Text = "비용성과지수" + Environment.NewLine + "(Cost Performance Index)";
            c1Chart2.Header.Compass = C1.Win.C1Chart.CompassEnum.North;

            c1Chart3.Header.Visible = true;
            c1Chart3.Header.Text = "일정편차" + Environment.NewLine + "(Schedule Variance)";
            c1Chart3.Header.Compass = C1.Win.C1Chart.CompassEnum.North;

            c1Chart4.Header.Visible = true;
            c1Chart4.Header.Text = "일정성과지수" + Environment.NewLine + "(Schedule Performance Index)";
            c1Chart4.Header.Compass = C1.Win.C1Chart.CompassEnum.North;

            c1Chart5.Header.Visible = true;
            c1Chart5.Header.Text = "미래성과지수" + Environment.NewLine + "(To Complete Performance Index)";
            c1Chart5.Header.Compass = C1.Win.C1Chart.CompassEnum.North;

            c1Chart6.Header.Visible = true;
            c1Chart6.Header.Text = "비용 및 일정 누적성과" + Environment.NewLine + "(Cost and Schedule Performance)";
            c1Chart6.Header.Compass = C1.Win.C1Chart.CompassEnum.North;

            c1Chart7.Header.Visible = true;
            c1Chart7.Header.Text = "최종사업비추정액 비교" + Environment.NewLine + "(Estimate At Completion)";
            c1Chart7.Header.Compass = C1.Win.C1Chart.CompassEnum.North;

            c1Chart8.Header.Visible = true;
            c1Chart8.Header.Text = "CPI & SPI & TCPI";
            c1Chart8.Header.Compass = C1.Win.C1Chart.CompassEnum.North;

            c1Chart9.Header.Visible = true;
            c1Chart9.Header.Text = "PV-Driver";
            c1Chart9.Header.Compass = C1.Win.C1Chart.CompassEnum.North;

        }
        #endregion

        #region Set_Chart_Clear
        private void Set_Chart_Clear()
        {
            //chart1---------------------- 
            ChartDataSeriesCollection coll1 = c1Chart1.ChartGroups[0].ChartData.SeriesList;
            coll1[0].PointData.Clear();
            coll1[1].PointData.Clear();

            Axis ax1 = c1Chart1.ChartArea.AxisX;
            ax1.ValueLabels.Clear();

            //chart2---------------------- 
            ChartDataSeriesCollection coll2 = c1Chart2.ChartGroups[0].ChartData.SeriesList;
            coll2[0].PointData.Clear();
            coll2[1].PointData.Clear();

            Axis ax2 = c1Chart2.ChartArea.AxisX;
            ax2.ValueLabels.Clear();

            //chart3---------------------- 
            ChartDataSeriesCollection coll3 = c1Chart3.ChartGroups[0].ChartData.SeriesList;
            coll3[0].PointData.Clear();
            coll3[1].PointData.Clear();

            Axis ax3 = c1Chart3.ChartArea.AxisX;
            ax3.ValueLabels.Clear();

            //chart4---------------------- 
            ChartDataSeriesCollection coll4 = c1Chart4.ChartGroups[0].ChartData.SeriesList;
            coll4[0].PointData.Clear();
            coll4[1].PointData.Clear();

            Axis ax4 = c1Chart4.ChartArea.AxisX;
            ax4.ValueLabels.Clear();

            //chart5---------------------- 
            ChartDataSeriesCollection coll5 = c1Chart5.ChartGroups[0].ChartData.SeriesList;
            coll5[0].PointData.Clear();

            Axis ax5 = c1Chart5.ChartArea.AxisX;
            ax5.ValueLabels.Clear();

            //chart6---------------------- 
            ChartDataSeriesCollection coll6 = c1Chart6.ChartGroups[0].ChartData.SeriesList;
            coll6[0].PointData.Clear();
            coll6[1].PointData.Clear();
            coll6[2].PointData.Clear();
            coll6[3].PointData.Clear();

            Axis ax6 = c1Chart6.ChartArea.AxisX;
            ax6.ValueLabels.Clear();

            //chart7---------------------- 
            //			ChartDataSeriesCollection coll7 = c1Chart7.ChartGroups[0].ChartData.SeriesList;
            //			coll7[0].PointData.Clear();
            //			coll7[1].PointData.Clear();
            //
            //			Axis ax7 = c1Chart7.ChartArea.AxisX;
            //			ax7.ValueLabels.Clear();	

            //chart8---------------------- 
            ChartDataSeriesCollection coll8 = c1Chart8.ChartGroups[0].ChartData.SeriesList;
            coll8[0].PointData.Clear();
            coll8[1].PointData.Clear();
            coll8[2].PointData.Clear();

            Axis ax8 = c1Chart8.ChartArea.AxisX;
            ax8.ValueLabels.Clear();

            //chart9---------------------- 
            ChartDataSeriesCollection coll9 = c1Chart9.ChartGroups[0].ChartData.SeriesList;
            coll9[0].PointData.Clear();
            coll9[1].PointData.Clear();
            coll9[2].PointData.Clear();
            coll9[3].PointData.Clear();
            coll9[4].PointData.Clear();

            Axis ax9 = c1Chart9.ChartArea.AxisX;
            ax9.ValueLabels.Clear();
        }
        #endregion

        #region SearchExec 그리드 조회
        protected override void SearchExec()
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {
                    string strQuery = " usp_EPR001 @pType='S1'  ";
                    strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' ";
                    strQuery += ", @pPROJECT_NO='" + txtProjectNo.Text.Trim() + "'";
                    strQuery += ", @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 2, true);
                    if (fpSpread1.Sheets[0].RowCount > 0) Set_Color();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
            this.Cursor = Cursors.Default;
        }

        private void Set_Color()
        {
            for (int i = 0; i < fpSpread1.Sheets[0].RowCount; i++)
            {
                if (fpSpread1.Sheets[0].Cells[i, 3].Text == "R")
                    fpSpread1.Sheets[0].Cells[i, 3].ForeColor = Color.Red;
                else
                    fpSpread1.Sheets[0].Cells[i, 3].ForeColor = Color.GreenYellow;
                fpSpread1.Sheets[0].Cells[i, 3].Text = "●";
            }
        }

        private void c1DockingTab1_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            switch (c1DockingTab1.SelectedIndex)
            {
                case 0:
                    Search_1(c1DockingTab1.SelectedIndex, "비용편차(CV)", "누적비용편차(CVcum)"); break;
                case 1:
                    Search_1(c1DockingTab1.SelectedIndex, "비용성과지수(CPI)", "누적비용성과지수(CPIcum)"); break;
                case 2:
                    Search_1(c1DockingTab1.SelectedIndex, "일정편차(SV)", "누적일정편차(SVcum)"); break;
                case 3:
                    Search_1(c1DockingTab1.SelectedIndex, "일정성과지수(SPI)", "누적일정성과지수(SPIcum)"); break;
                case 4:
                    Search_5(); break;
                case 5:
                    Search_6(); break;
                case 6:
                    Search_7(); break;
                case 7:
                    Search_8(); break;
                case 8:
                    Search_9(); break;
                default:
                    c1DockingTab1.SelectedIndex = 0;
                    Search_1(c1DockingTab1.SelectedIndex, "비용편차(CV)", "누적비용편차(CVcum)");
                    break;

            }
        }

        private void fpSpread1_LeaveCell(object sender, FarPoint.Win.Spread.LeaveCellEventArgs e)
        {
            if (TempRow != e.NewRow)
            {
                Set_Chart_Clear();

                int Row = e.NewRow;
                strProjectNo = fpSpread1.Sheets[0].Cells[e.NewRow, 1].Text;
                switch (c1DockingTab1.SelectedIndex)
                {
                    case 0:
                        Search_1(c1DockingTab1.SelectedIndex, "비용편차(CV)", "누적비용편차(CVcum)"); break;
                    case 1:
                        Search_1(c1DockingTab1.SelectedIndex, "비용성과지수(CPI)", "누적비용성과지수(CPIcum)"); break;
                    case 2:
                        Search_1(c1DockingTab1.SelectedIndex, "일정편차(SV)", "누적일정편차(SVcum)"); break;
                    case 3:
                        Search_1(c1DockingTab1.SelectedIndex, "일정성과지수(SPI)", "누적일정성과지수(SPIcum)"); break;
                    case 4:
                        Search_5(); break;
                    case 5:
                        Search_6(); break;
                    case 6:
                        Search_7(); break;
                    case 7:
                        Search_8(); break;
                    case 8:
                        Search_9(); break;
                    default:
                        c1DockingTab1.SelectedIndex = 0;
                        Search_1(c1DockingTab1.SelectedIndex, "비용편차(CV)", "누적비용편차(CVcum)");
                        break;

                }
                TempRow = e.NewRow;

            }
        }

        private void Search_1(int idx, string Series1, string Series2)
        {

            try
            {
                if (idx == 0)
                {
                    if (strProjectNo1 == strProjectNo) return;
                }
                else if (idx == 1)
                {
                    if (strProjectNo2 == strProjectNo) return;
                }
                else if (idx == 2)
                {
                    if (strProjectNo3 == strProjectNo) return;
                }
                else
                {
                    if (strProjectNo4 == strProjectNo) return;
                }

                this.Cursor = Cursors.WaitCursor;

                string strQuery1 = " usp_EPR001 @pType='G" + Convert.ToString(idx + 1) + "'  ";
                strQuery1 += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' ";
                strQuery1 += ", @pPROJECT_NO='" + strProjectNo + "'";
                strQuery1 += ", @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

                DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery1);
                if (dt.Rows.Count <= 0)
                    MessageBox.Show(SystemBase.Base.MessageRtn("P0019"));
                else
                {

                    if (idx == 0)
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

                        BindSeries(c1Chart1, 0, dv, Series1, "일자");
                        BindSeries(c1Chart1, 1, dv, Series2);
                        strProjectNo1 = strProjectNo;
                    }
                    else if (idx == 1)
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

                        BindSeries(c1Chart2, 0, dv, Series1, "일자");
                        BindSeries(c1Chart2, 1, dv, Series2);
                        strProjectNo2 = strProjectNo;
                    }
                    else if (idx == 2)
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

                        BindSeries(c1Chart3, 0, dv, Series1, "일자");
                        BindSeries(c1Chart3, 1, dv, Series2);
                        strProjectNo3 = strProjectNo;
                    }
                    else
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

                        BindSeries(c1Chart4, 0, dv, Series1, "일자");
                        BindSeries(c1Chart4, 1, dv, Series2);
                        strProjectNo4 = strProjectNo;
                    }
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
            this.Cursor = Cursors.Default;

        }

        private void Search_5()
        {

            try
            {
                if (strProjectNo5 == strProjectNo) return;

                this.Cursor = Cursors.WaitCursor;

                string strQuery1 = " usp_EPR001 @pType='G5'  ";
                strQuery1 += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' ";
                strQuery1 += ", @pPROJECT_NO='" + strProjectNo + "'";
                strQuery1 += ", @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

                DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery1);
                if (dt.Rows.Count <= 0)
                    MessageBox.Show(SystemBase.Base.MessageRtn("P0019"));
                else
                {
                    // initialize chart
                    c1Chart5.Legend.Visible = true;
                    c1Chart5.Legend.Compass = CompassEnum.East;
                    c1Chart5.Legend.Style.Border.BorderStyle = C1.Win.C1Chart.BorderStyleEnum.Raised;

                    //label clear
                    ChartLabels cl = c1Chart5.ChartLabels;
                    C1.Win.C1Chart.LabelsCollection clc = cl.LabelsCollection;
                    clc.Clear();

                    DataView dv = dt.DefaultView;

                    BindSeries(c1Chart5, 0, dv, "비용성과지수(TCPI)", "일자");
                }
                strProjectNo5 = strProjectNo;
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
            this.Cursor = Cursors.Default;

        }


        private void Search_6()
        {

            try
            {
                if (strProjectNo6 == strProjectNo) return;

                this.Cursor = Cursors.WaitCursor;

                string strQuery1 = " usp_EPR001 @pType='G6'  ";
                strQuery1 += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' ";
                strQuery1 += ", @pPROJECT_NO='" + strProjectNo + "'";
                strQuery1 += ", @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

                DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery1);
                if (dt.Rows.Count <= 0)
                    MessageBox.Show(SystemBase.Base.MessageRtn("P0019"));
                else
                {
                    // initialize chart
                    c1Chart6.Legend.Visible = true;
                    c1Chart6.Legend.Compass = CompassEnum.East;
                    c1Chart6.Legend.Style.Border.BorderStyle = C1.Win.C1Chart.BorderStyleEnum.Raised;

                    //label clear
                    ChartLabels cl = c1Chart6.ChartLabels;
                    C1.Win.C1Chart.LabelsCollection clc = cl.LabelsCollection;
                    clc.Clear();

                    DataView dv = dt.DefaultView;

                    BindSeries(c1Chart6, 0, dv, "계획예산(PV)", "일자");
                    BindSeries(c1Chart6, 1, dv, "실비용(AC)");
                    BindSeries(c1Chart6, 2, dv, "성과가치(EV)");
                    BindSeries(c1Chart6, 3, dv, "사업비추정액(EAC)");
                }
                strProjectNo6 = strProjectNo;
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
            this.Cursor = Cursors.Default;

        }

        private void Search_7()
        {

            try
            {
                if (strProjectNo7 == strProjectNo) return;

                this.Cursor = Cursors.WaitCursor;

                string strQuery1 = " usp_EPR001 @pType='G7'  ";
                strQuery1 += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' ";
                strQuery1 += ", @pPROJECT_NO='" + strProjectNo + "'";
                strQuery1 += ", @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

                DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery1);
                if (dt.Rows.Count <= 0)
                    MessageBox.Show(SystemBase.Base.MessageRtn("P0019"));
                else
                {
                    // initialize chart
                    c1Chart7.Legend.Visible = true;
                    c1Chart7.Legend.Compass = CompassEnum.East;
                    c1Chart7.Legend.Style.Border.BorderStyle = C1.Win.C1Chart.BorderStyleEnum.Raised;

                    //label clear
                    ChartLabels cl = c1Chart7.ChartLabels;
                    C1.Win.C1Chart.LabelsCollection clc = cl.LabelsCollection;
                    clc.Clear();

                    //					DataView dv = dt.DefaultView;
                    //
                    //					BindSeries(c1Chart7, 0, dv, "비용성과지수(CPI)",   "일자");					
                    //					BindSeries(c1Chart7, 1, dv, "일정성과지수(SPI)");			
                    //					BindSeries(c1Chart7, 2, dv, "미래성과지수(TCPI)");				
                }
                strProjectNo7 = strProjectNo;
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
            this.Cursor = Cursors.Default;

        }

        private void Search_8()
        {

            try
            {
                if (strProjectNo8 == strProjectNo) return;

                this.Cursor = Cursors.WaitCursor;

                string strQuery1 = " usp_EPR001 @pType='G8'  ";
                strQuery1 += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' ";
                strQuery1 += ", @pPROJECT_NO='" + strProjectNo + "'";
                strQuery1 += ", @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

                DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery1);
                if (dt.Rows.Count <= 0)
                    MessageBox.Show(SystemBase.Base.MessageRtn("P0019"));
                else
                {
                    // initialize chart
                    c1Chart8.Legend.Visible = true;
                    c1Chart8.Legend.Compass = CompassEnum.East;
                    c1Chart8.Legend.Style.Border.BorderStyle = C1.Win.C1Chart.BorderStyleEnum.Raised;

                    //label clear
                    ChartLabels cl = c1Chart8.ChartLabels;
                    C1.Win.C1Chart.LabelsCollection clc = cl.LabelsCollection;
                    clc.Clear();

                    DataView dv = dt.DefaultView;

                    BindSeries(c1Chart8, 0, dv, "비용성과지수(CPI)", "일자");
                    BindSeries(c1Chart8, 1, dv, "일정성과지수(SPI)");
                    BindSeries(c1Chart8, 2, dv, "미래성과지수(TCPI)");
                }
                strProjectNo8 = strProjectNo;
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
            this.Cursor = Cursors.Default;

        }

        private void Search_9()
        {
            try
            {
                if (strProjectNo9 == strProjectNo) return;

                this.Cursor = Cursors.WaitCursor;

                string strQuery1 = " usp_EPR001 @pType='G9'  ";
                strQuery1 += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' ";
                strQuery1 += ", @pPROJECT_NO='" + strProjectNo + "'";
                strQuery1 += ", @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

                DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery1);
                if (dt.Rows.Count <= 0)
                    MessageBox.Show(SystemBase.Base.MessageRtn("P0019"));
                else
                {
                    // initialize chart
                    c1Chart9.Legend.Visible = true;
                    c1Chart9.Legend.Compass = CompassEnum.East;
                    c1Chart9.Legend.Style.Border.BorderStyle = C1.Win.C1Chart.BorderStyleEnum.Raised;

                    //label clear
                    ChartLabels cl = c1Chart9.ChartLabels;
                    C1.Win.C1Chart.LabelsCollection clc = cl.LabelsCollection;
                    clc.Clear();

                    DataView dv = dt.DefaultView;

                    BindSeries(c1Chart9, 0, dv, "계획예산(PV)", "품목");
                    BindSeries(c1Chart9, 1, dv, "성과(EV)");
                    BindSeries(c1Chart9, 2, dv, "실비용(AC)");
                    BindSeries(c1Chart9, 3, dv, "비용편차(CV)");
                    BindSeries(c1Chart9, 4, dv, "일정편차(SV)");
                }
                strProjectNo9 = strProjectNo;
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        #region   팝업
        private void btnProject_Click(object sender, System.EventArgs e)
        {
            try
            {

                WNDW007 pu = new WNDW007(txtProjectNo.Text);
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;
                    txtProjectNo.Text = Msgs[3].ToString();
                    txtProjectNm.Value = Msgs[4].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        private void txtProjectNo_TextChanged(object sender, System.EventArgs e)
        {
            txtProjectNm.Value = SystemBase.Base.CodeName("PROJECT_NO", "PROJECT_NM", "S_SO_MASTER", txtProjectNo.Text, " AND SO_CONFIRM_YN = 'Y' AND CO_CD='" + SystemBase.Base.gstrCOMCD + "'");
        }
        #endregion

        #region Actived
        private void EPR001_Activated(object sender, System.EventArgs e)
        {
            txtProjectNo.Focus();
        }
        #endregion
    }
}
