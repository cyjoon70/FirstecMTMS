#region 작성정보
/*********************************************************************/
// 단위업무명 : PRINT
// 작 성 자 : 조  홍  태
// 작 성 일 : 2013-02-06
// 작성내용 : 크리스탈레포트 view
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
using CrystalDecisions.Windows.Forms;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using CrystalDecisions.Windows;

namespace MP.MPO506
{
    public partial class PRINT : Form
    {
        #region 변수선언

        CrystalDecisions.CrystalReports.Engine.ReportDocument rpt = new CrystalDecisions.CrystalReports.Engine.ReportDocument();

        string[] FormulaField = null;
        string[] FormulaFieldName = null;

        string RptName1 = null;
        string[] RptParmValue1 = null;

        string SubRptName2 = null;
        string[] RptParmValue2 = null;

        string SubRptName3 = null;
        string[] RptParmValue3 = null;

        string SubRptName4 = null;
        string[] RptParmValue4 = null;

        string SubRptName5 = null;
        string[] RptParmValue5 = null;

        #endregion

        #region 생성자

        public PRINT(
            string FormText1
            , string[] FormulaField2
            , string RptName11
            , string[] RptParmValue11
            )
        {
            InitializeComponent();
            this.Text = FormText1;

            FormulaField = FormulaField2;
            FormulaFieldName = null;
            RptName1 = RptName11;
            RptParmValue1 = RptParmValue11;
        }

        public PRINT(
            string FormText1
            , string[] FormulaFieldValue
            , string[] FormulaFieldNameValue
            , string RptName11
            , string[] RptParmValue11
            )
        {
            InitializeComponent();
            this.Text = FormText1;

            FormulaField = FormulaFieldValue;
            FormulaFieldName = FormulaFieldNameValue;

            RptName1 = RptName11;
            RptParmValue1 = RptParmValue11;
        }

        public PRINT(
            string FormText1
            , string[] FormulaField2
            , string RptName11
            , string[] RptParmValue11
            , string SubRptName21
            , string[] RptParmValue21)
        {
            InitializeComponent();
            this.Text = FormText1;

            FormulaField = FormulaField2;

            RptName1 = RptName11;
            RptParmValue1 = RptParmValue11;

            SubRptName2 = SubRptName21;
            RptParmValue2 = RptParmValue21;
        }

        public PRINT(
            string FormText1
            , string[] FormulaField2
            , string RptName11
            , string[] RptParmValue11
            , string SubRptName21
            , string[] RptParmValue21
            , string SubRptName31
            , string[] RptParmValue31
            )
        {
            InitializeComponent();
            this.Text = FormText1;

            FormulaField = FormulaField2;

            RptName1 = RptName11;
            RptParmValue1 = RptParmValue11;

            SubRptName2 = SubRptName21;
            RptParmValue2 = RptParmValue21;

            SubRptName3 = SubRptName31;
            RptParmValue3 = RptParmValue31;
        }

        public PRINT(
            string FormText1
            , string[] FormulaField2
            , string RptName11
            , string[] RptParmValue11
            , string SubRptName21
            , string[] RptParmValue21
            , string SubRptName31
            , string[] RptParmValue31
            , string SubRptName41
            , string[] RptParmValue41
            )
        {
            InitializeComponent();
            this.Text = FormText1;

            FormulaField = FormulaField2;

            RptName1 = RptName11;
            RptParmValue1 = RptParmValue11;

            SubRptName2 = SubRptName21;
            RptParmValue2 = RptParmValue21;

            SubRptName3 = SubRptName31;
            RptParmValue3 = RptParmValue31;

            SubRptName4 = SubRptName41;
            RptParmValue4 = RptParmValue41;
        }

        public PRINT(
            string FormText1
            , string[] FormulaField2
            , string RptName11
            , string[] RptParmValue11
            , string SubRptName21
            , string[] RptParmValue21
            , string SubRptName31
            , string[] RptParmValue31
            , string SubRptName41
            , string[] RptParmValue41
            , string SubRptName51
            , string[] RptParmValue51
            )
        {
            InitializeComponent();
            this.Text = FormText1;

            FormulaField = FormulaField2;

            RptName1 = RptName11;
            RptParmValue1 = RptParmValue11;

            SubRptName2 = SubRptName21;
            RptParmValue2 = RptParmValue21;

            SubRptName3 = SubRptName31;
            RptParmValue3 = RptParmValue31;

            SubRptName4 = SubRptName41;
            RptParmValue4 = RptParmValue41;

            SubRptName5 = SubRptName51;
            RptParmValue5 = RptParmValue51;
        }
        #endregion

        #region 크리스탈레포트 login
        private void LogonToReport(string server, string database, string ID, string password)
        {
            TableLogOnInfo logonInfo = new TableLogOnInfo();
            foreach (CrystalDecisions.CrystalReports.Engine.Table table in rpt.Database.Tables)
            {
                logonInfo = table.LogOnInfo;
                logonInfo.ConnectionInfo.ServerName = server;
                logonInfo.ConnectionInfo.DatabaseName = database;
                logonInfo.ConnectionInfo.UserID = ID;
                logonInfo.ConnectionInfo.Password = password;
                table.ApplyLogOnInfo(logonInfo);
            }
        }
        #endregion

        private void PRINT_Load(object sender, System.EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                rpt.Load(RptName1);

                LogonToReport(SystemBase.Base.gstrServerNM, SystemBase.Base.gstrDbName, SystemBase.Base.gstrServerId, SystemBase.Base.gstrServerPwd);

                if (FormulaField != null)
                {
                    for (int i = 0; i < rpt.DataDefinition.FormulaFields.Count; i++)
                    {
                        string FieldName = rpt.DataDefinition.FormulaFields[i].Name;

                        for (int j = 0; j < FormulaField.Length; j++)
                        {
                            if (FieldName == FormulaFieldName[j])
                            {
                                rpt.DataDefinition.FormulaFields[i].Text = FormulaField[j];
                            }
                        }
                    }
                }

                for (int i = 0; i < RptParmValue1.Length; i++)
                {
                    rpt.SetParameterValue(i, RptParmValue1[i]);
                }
                crystalReportViewer1.ReportSource = rpt;

                if (RptParmValue2 != null)
                {
                    rpt.OpenSubreport(SubRptName2);
                    for (int i = 0; i < RptParmValue2.Length; i++)
                    {
                        rpt.SetParameterValue(i, RptParmValue2[i]);
                    }
                    crystalReportViewer1.ReportSource = rpt;
                }

                if (RptParmValue3 != null)
                {
                    rpt.OpenSubreport(SubRptName3);
                    for (int i = 0; i < RptParmValue3.Length; i++)
                    {
                        rpt.SetParameterValue(i, RptParmValue3[i]);
                    }
                    crystalReportViewer1.ReportSource = rpt;
                }

                if (RptParmValue4 != null)
                {
                    rpt.OpenSubreport(SubRptName4);
                    for (int i = 0; i < RptParmValue4.Length; i++)
                    {
                        rpt.SetParameterValue(i, RptParmValue4[i]);
                    }
                    crystalReportViewer1.ReportSource = rpt;
                }

                if (RptParmValue5 != null)
                {
                    rpt.OpenSubreport(SubRptName5);
                    for (int i = 0; i < RptParmValue5.Length; i++)
                    {
                        rpt.SetParameterValue(i, RptParmValue5[i]);
                    }
                    crystalReportViewer1.ReportSource = rpt;
                }

                this.Cursor = Cursors.Default;
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log("PRINT_Load", f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "(PRINT) 프린트"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
