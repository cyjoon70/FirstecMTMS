using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PC.PCC013
{
    public partial class PCC013P3 : UIForm.FPCOMM1
    {
        string[] returnVal = null;
        string strWo_No_Rs = "";
        string strWo_No = "";

        public PCC013P3(string WO_NO_RS)
        {
            strWo_No_Rs = WO_NO_RS;
            InitializeComponent();
        }

        public PCC013P3()
        {
            InitializeComponent();
        }

        #region 폼로드 이벤트
        private void PCC013P3_Load(object sender, System.EventArgs e)
        {
            this.Text = "공정진행현황";

            //버튼 재정의(조회권한만)
            UIForm.Buttons.ReButton("010000000000", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);
            
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
                string strQuery = " usp_PCC013 @pTYPE = 'S5'";
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
