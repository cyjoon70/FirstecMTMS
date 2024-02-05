using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MP.MPO101
{
    public partial class MPO101P3 : UIForm.FPCOMM1
    {
        #region 변수선언
        string returnVal;
        string returnStr;
        string strPoNo;
        string strPoSeq;
        string strState = "N";
        string strTemp = "";
        #endregion

        #region 생성자
        public MPO101P3()
        {
            InitializeComponent();
        }

        public MPO101P3(string PoNo, string PoSeq)
        {
            InitializeComponent();
            strPoNo = PoNo;
            strPoSeq = PoSeq;
        }
        #endregion

        #region 폼로드 이벤트
        private void MPO101P3_Load(object sender, EventArgs e)
        {
            this.Text = "품질증빙팝업";
            UIForm.Buttons.ReButton("010000000001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
            SearchExec(); 
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            try
            {
                string strQuery = " usp_MPO101  @pTYPE = 'P3'";
                strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' ";
                strQuery += ", @pPO_NO = '" + strPoNo + "' ";
                strQuery += ", @pPO_SEQ = '" + strPoSeq + "' ";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false, true);

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            this.Cursor = System.Windows.Forms.Cursors.Default;
        }
        #endregion

        #region 버튼 Click
        private void btnOk_Click(object sender, System.EventArgs e)
        {

            for (int i = 0; i < fpSpread1.Sheets[0].RowCount; i++)
            {
                if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "선택")].Value.ToString() == "1")
                {
                    strTemp += fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "증빙코드")].Text + ";";      // 2016.07.25. hma 수정: 문서코드 뒤에 세미콜론이 붙도록 함.
                }
            }
            strState = "Y";
            this.Close();
        }

        private void butCancel_Click(object sender, System.EventArgs e)
        {
            strState = "N";
            this.Close();
        }
        #endregion

        #region 값 전송
        public string ReturnVal { get { return returnVal; } set { returnVal = value; } }
        public string ReturnStr { get { return returnStr; } set { returnStr = value; } }

        public void RtnStr(string strCode, string strValue)
        {
            returnVal = strCode;
            returnStr = strValue;
        }
        #endregion
        
        #region fpSpread1_ButtonClicked
        private void fpSpread1_ButtonClicked(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            fpSpread1.Sheets[0].RowHeader.Cells[e.Row, 0].Text = "";
        }
        #endregion

        #region MPO101P3_FormClosing
        private void MPO101P3_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (strState == "Y")
            {
                RtnStr("Y", strTemp);
                this.DialogResult = DialogResult.OK;
            }
            else
            {
                RtnStr("N", "");
                this.DialogResult = DialogResult.Cancel;
            }
        }
        #endregion

    }
}
