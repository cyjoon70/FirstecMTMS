
#region 작성정보
/*********************************************************************/
// 단위업무명 : 라우팅 REV 이력조회
// 작 성 자 : 권순철
// 작 성 일 : 2013-04-15
// 작성내용 : 라우팅 REV 이력조회 및 관리
// 수 정 일 :
// 수 정 자 :
// 수정내용 :
// 비    고 :
/*********************************************************************/
#endregion

using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Globalization;
using System.Threading;
using WNDW;
using System.Text.RegularExpressions;
using System.IO;
using System.Reflection;

namespace PA.PBA173
{
    public partial class PBA173 : UIForm.FPCOMM1
    {
        #region 변수선언
        private string strMQuery;
        #endregion

        public PBA173()
        {
            InitializeComponent();
        }

        #region Form Load 시
        private void PBA173_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            //기타 세팅
            string date1 = SystemBase.Base.ServerTime("YYMMDD").Substring(0, 7);
            string date2 = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(1).ToShortDateString().Substring(0, 7);

            dtpSTART_DT.Text = date1 + "-01";
            dtpEND_DT.Text = Convert.ToDateTime(date2 + "-01").AddDays(-1).ToShortDateString();
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            fpSpread1.Sheets[0].Rows.Count = 0;
            //기타 세팅
            string date1 = SystemBase.Base.ServerTime("YYMMDD").Substring(0, 7);
            string date2 = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(1).ToShortDateString().Substring(0, 7);

            dtpSTART_DT.Text = date1 + "-01";
            dtpEND_DT.Text = Convert.ToDateTime(date2 + "-01").AddDays(-1).ToShortDateString();
        }
        #endregion

        #region SearchExec()
        protected override void SearchExec()
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {
                    strMQuery = " usp_PBA173 'S1'";
                    strMQuery += ", @pREVISION_DT_FR = '" + dtpSTART_DT.Text + "'";
                    strMQuery += ", @pREVISION_DT_TO = '" + dtpEND_DT.Text + "'";
                    strMQuery += ", @pITEM_CD = '" + txtSITEM_CD.Text + "'";
                    strMQuery += ", @pROUT_NO = '" + txtROUTING_NO.Text + "'";
                    strMQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strMQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0, true);
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
        #endregion

        #region 조회조건 팝업
        private void btnSITEM_CD_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pTYPE = 'P030', @pPLANT_CD = '" + SystemBase.Base.gstrPLANT_CD + "' , @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };
                string[] strSearch = new string[] { txtSITEM_CD.Text, txtSITEM_NM.Text };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00001", strQuery, strWhere, strSearch, "품목코드 조회", new int[] { 1, 2 }, true);
                pu.Width = 500;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {

                    txtSITEM_CD.Text = pu.ReturnValue[1].ToString();
                    txtSITEM_NM.Value = pu.ReturnValue[2].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목코드 조회"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnROUT_Click(object sender, EventArgs e)
        {

            try
            {
                if (txtSITEM_CD.Text == "")  // 품목코드 검사
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn("P0030"));
                    return;
                }
                string strQuery = "usp_Q_COMMON @pType='Q030', @pSPEC1 = '" + SystemBase.Base.gstrPLANT_CD + "', @pSPEC2 = '" + txtSITEM_CD.Text + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtROUTING_NO.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P06005", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "라우팅번호 조회");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtROUTING_NO.Value = Msgs[0].ToString();
                    txtROUTING_NM.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "라우팅정보 조회"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 코드 입력시 코드명 자동입력
        private void txtSITEM_CD_TextChanged(object sender, System.EventArgs e)
        {
            if (txtSITEM_CD.Text != "")
            {
                txtSITEM_NM.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtSITEM_CD.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "'");
            }
            else
            {
                txtSITEM_NM.Value = "";
            }
        }
        #endregion

    }
}
