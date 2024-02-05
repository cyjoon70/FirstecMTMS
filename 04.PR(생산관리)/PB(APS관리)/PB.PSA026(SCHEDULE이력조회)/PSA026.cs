#region 작성정보
/*********************************************************************/
// 단위업무명 : SCHEDULE 이력조회
// 작 성 자 : 김 현근
// 작 성 일 : 2013-04-15
// 작성내용 : SCHEDULE 이력조회
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
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using WNDW;

namespace PB.PSA026
{
    public partial class PSA026 : UIForm.FPCOMM2
    {
        string strSchNo = "";

        public PSA026()
        {
            InitializeComponent();
        }

        public PSA026(string Div)
        {
            strSchNo = Div;
            InitializeComponent();
        }

        #region Form Load 시
        private void PSA026_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false, false);
            UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, 0, 0, false, false);

            SystemBase.ComboMake.C1Combo(cboSch_Type, "usp_P_COMMON @pTYPE = 'P040',@pLANG_CD = 'KOR', @pCOM_CD = 'P058',  @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");  //SCH TYPE

            cboSch_Type.SelectedValue = "S";

            if (strSchNo != "")
            {
                txtSch_No.Text = strSchNo;
                SearchExec();
            }
        }
        #endregion

        #region 스케쥴 NO 팝업 조회
        private void btnSch_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_PSA026 'P1' ,@pSCH_TYPE='" + cboSch_Type.SelectedValue.ToString() + "',  @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pSCH_NO" };
                string[] strSearch = new string[] { "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("PSA026P1", strQuery, strWhere, strSearch, new int[] { 0 });
                pu.Width = 1200;
                pu.FormBorderStyle = FormBorderStyle.Sizable;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtSch_No.Text = Msgs[0].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                SystemBase.MessageBoxComm.Show(f.ToString());
            }

        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false, false);
            UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, 0, 0, false, false);
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {
                    string strQuery = "";
                    strQuery = "   usp_PSA026 @pTYPE = 'S1'";
                    strQuery += ",            @pSCH_NO = '" + txtSch_No.Text + "' ";
                    strQuery += ",            @pSCH_TYPE = '" + cboSch_Type.SelectedValue.ToString() + "' ";
                    strQuery += ", @pCO_CD  = '" + SystemBase.Base.gstrCOMCD + "'";

                    UIForm.FPMake.grdCommSheet(fpSpread2, strQuery, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, true, 0, 0);

                    if (fpSpread2.Sheets[0].Rows.Count > 0)
                    {
                        int Row = 0;
                        SubSearch(Row);
                    }
                    else
                    {
                        fpSpread1.Sheets[0].Rows.Count = 0;
                        txtSch_No.Text = "";
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            this.Cursor = System.Windows.Forms.Cursors.Default;
        }
        #endregion

        #region fpSpread2 Select 이벤트
        private void fpSpread2_SelectionChanged(object sender, FarPoint.Win.Spread.SelectionChangedEventArgs e)
        {
            int Row = 0;
            if (fpSpread2.Sheets[0].Rows.Count > 0)
            {
                Row = fpSpread2.Sheets[0].ActiveRowIndex;

                SubSearch(Row);
            }
            else
            {
                Row = 0;
            }
        }
        #endregion

        #region fpSpread1 조회
        private void SubSearch(int Row)
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {
                    string strQuery = "";
                    strQuery = "   usp_PSA026 @pTYPE = 'S2'";
                    strQuery += ",            @pSCH_NO = '" + fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "SCH_NO")].Text + "' ";
                    strQuery += ",            @pSCH_ID = '" + fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "SCH_ID")].Text + "' ";
                    strQuery += ", @pCO_CD  = '" + SystemBase.Base.gstrCOMCD + "'";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0, true);
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            this.Cursor = System.Windows.Forms.Cursors.Default;
        }
        #endregion
        
    }
}
