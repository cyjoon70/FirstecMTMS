
#region 작성정보
/*********************************************************************/
// 단위업무명 : 부하능력검토(전체)
// 작 성 자 : 권순철
// 작 성 일 : 2013-04-09
// 작성내용 : 부하능력검토(전체) 및 관리
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
using System.Text.RegularExpressions;
using System.Data;
using System.Data.SqlClient;
using WNDW;

namespace PB.PBC103
{
    public partial class PBC103 : UIForm.FPCOMM1
    {
        public PBC103()
        {
            InitializeComponent();
        }

        #region Form Load 시
        private void PBC103_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            SystemBase.ComboMake.C1Combo(cboSch_id, "usp_P_COMMON 'P520', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'");

            dtpStartDT.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD").ToString()).ToShortDateString();
            dtpEndDT.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD").ToString()).AddMonths(2).ToShortDateString();
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
        }
        #endregion

        #region SearchExec()
        protected override void SearchExec()
        {
            //조회조건 필수 체크
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1) == true)
            {
                this.Cursor = Cursors.WaitCursor;

                try
                {
                    string strQuery1 = " usp_PBC103 @pType='S1', ";
                    strQuery1 += " @pSCH_ID='" + cboSch_id.SelectedValue.ToString() + "', ";
                    strQuery1 += " @pSTART_DT='" + dtpStartDT.Text + "', ";
                    strQuery1 += " @pEND_DT='" + dtpEndDT.Text + "', ";
                    strQuery1 += " @pWC_CD='" + txtWORKCENTER_CD.Text + "' ";
                    strQuery1 += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery1, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, true, true, 0, 0, true);

                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "가동능력")].BackColor = Color.LightBlue;
                        fpSpread1.Sheets[0].Cells[i, 14].BackColor = Color.LightBlue; //OPEN부하 사내합
                        fpSpread1.Sheets[0].Cells[i, 15].BackColor = Color.LightBlue; //OPEN부하 외주합
                        fpSpread1.Sheets[0].Cells[i, 22].BackColor = Color.LightBlue; //PLAN부하 사내합
                        fpSpread1.Sheets[0].Cells[i, 23].BackColor = Color.LightBlue; //PLAN부하 외주합
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                this.Cursor = Cursors.Default;
            }
        }
        #endregion

        #region 조회조건 팝업
        //작업장 조회
        private void btnWc_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pType='P042', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pETC = 'P002', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };
                string[] strSearch = new string[] { txtWORKCENTER_CD.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00025", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "작업장 조회");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtWORKCENTER_CD.Text = Msgs[0].ToString();
                    txtWORKCENTER_NM.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "작업장 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region Text Change
        //작업장
        private void txtWc_CD_TextChanged(object sender, System.EventArgs e)
        {
            txtWORKCENTER_NM.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtWORKCENTER_CD.Text, " AND MAJOR_CD = 'P002' AND COMP_CODE='" + SystemBase.Base.gstrCOMCD + "'");
        }
        #endregion

    }
}
