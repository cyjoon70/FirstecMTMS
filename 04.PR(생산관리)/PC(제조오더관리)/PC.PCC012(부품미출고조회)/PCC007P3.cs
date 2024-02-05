#region 작성정보
/*********************************************************************/
// 단위업무명 : 부품미출고조회
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-02-13
// 작성내용 : 부품미출고조회 관리
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

namespace PC.PCC012
{  
    public partial class PCC007P3 : UIForm.FPCOMM1
    {
        string[] returnVal = null;
        string strWo_No_Rs = "";
        string strWo_No = "";

        public PCC007P3(string WO_NO_RS)
        {
            strWo_No_Rs = WO_NO_RS;

            InitializeComponent();           
        }

        #region Form Load 시
        private void PCC007P3_Load(object sender, System.EventArgs e)
        {
            this.Text = "대표오더번호 조회";
            //GroupBo x1 초기화
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            UIForm.Buttons.ReButton("010000001001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);

            txtWoNoRs.Text = strWo_No_Rs;
            txtWorkOrderNo.Text = strWo_No;

            Search();
        }
        #endregion

        #region 조회버튼 클릭
        protected override void SearchExec()
        { Search(); }
        #endregion

        #region 조회
        private void Search()
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {
                string strQuery = " usp_PCC007 @pTYPE = 'S5'";
                strQuery += ", @pWORKORDER_NO_RS = '" + txtWoNoRs.Text + "' ";
                strQuery += ", @pWORKORDER_NO = '" + txtWorkOrderNo.Text + "' ";
                strQuery += ", @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0, true);

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }

            this.Cursor = Cursors.Default;
        }
        #endregion

        #region 그리드 더블클릭
        private void fpSpread1_CellDoubleClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            RtnStr(e.Row);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        #endregion

        #region 그리드 선택값 입력밑 전송
        public string[] ReturnVal { get { return returnVal; } set { returnVal = value; } }

        public void RtnStr(int R)
        {
            if (fpSpread1.Sheets[0].Rows.Count > 0)
            {
                returnVal = new string[fpSpread1.Sheets[0].Columns.Count];
                for (int i = 0; i < fpSpread1.Sheets[0].Columns.Count; i++)
                {
                    returnVal[i] = Convert.ToString(fpSpread1.Sheets[0].Cells[R, i].Value);
                }
            }
        }
        #endregion
	
    }
}
