#region 작성정보
/*********************************************************************/
// 단위업무명 : 개발일정현황조회
// 작 성 자 : 김현근
// 작 성 일 : 2013-04-15
// 작성내용 : 개발일정현황조회
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

namespace PA.SBA013
{
    public partial class SBA013 : UIForm.FPCOMM1
    {
        public SBA013()
        {
            InitializeComponent();
        }
         
        #region Form Load 시
        private void SBA013_Load(object sender, System.EventArgs e)
        {
            //GroupBox1 초기화
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            dtpSoDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString();
            dtpSoDtTo.Text = SystemBase.Base.ServerTime("YYMMDD");
        }
        #endregion
        
        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            Search();
        }
        #endregion

        #region Search함수
        private void Search()
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            try
            {
                string InsertChkYn = "";
                if (rdo2.Checked == true)
                {
                    InsertChkYn = "N";
                }
                else if (rdo3.Checked == true)
                {
                    InsertChkYn = "Y";
                }
                else if (rdo4.Checked == true)
                {
                    InsertChkYn = "C";
                }

                string strQuery = " usp_SBA013  @pTYPE = 'S1'";
                strQuery += ", @pSO_DT_FR = '" + dtpSoDtFr.Text + "' ";
                strQuery += ", @pSO_DT_TO = '" + dtpSoDtTo.Text + "'";
                strQuery += ", @pSO_NO = '" + txtSoNo.Text + "' ";
                strQuery += ", @pPROJECT_NO = '" + txtProjectNo.Text + "'";
                strQuery += ", @pSHIP_CD = '" + txtShipCd.Text + "'";
                strQuery += ", @pCHK_YN = '" + InsertChkYn + "'";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";

                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0);
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            this.Cursor = System.Windows.Forms.Cursors.Default;
        }
        #endregion

        #region 거래처 팝업
        private void btnShip_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW002 pu = new WNDW002(txtShipCd.Text, "");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtShipCd.Text = Msgs[1].ToString();
                    txtShipNm.Value = Msgs[2].ToString();
                    txtShipCd.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "거래처 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 거래처 자동 입력
        private void txtShipCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtShipCd.Text != "")
                {
                    txtShipNm.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtShipCd.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtShipNm.Value = "";
                }
            }
            catch { }
        }
        #endregion

    }
}
