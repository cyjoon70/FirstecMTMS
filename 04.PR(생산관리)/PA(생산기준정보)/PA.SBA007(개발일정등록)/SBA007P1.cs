#region 작성정보
/*********************************************************************/
// 단위업무명 : 개발일정등록
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-02-01
// 작성내용 : 개발일정등록 및 관리
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

namespace PA.SBA007
{ 
    public partial class SBA007P1 : UIForm.FPCOMM1
    {
        public string strProjectNo;
        public string strProjectSeq;
        public string strProjectNm;
        public string strEntCd;
        public string strEntNm;
        public string strShipCd;
        public string strShipNm;
        public string strItemCd;
        public string strItemNm;

        string ProjectNo = "";

        public SBA007P1(string ProjNo)
        {
            ProjectNo = ProjNo;
            InitializeComponent();           
        }

        #region Form Load 시
        private void SBA007P1_Load(object sender, System.EventArgs e)
        { 
            //GroupBo x1 초기화
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            UIForm.Buttons.ReButton("010000000001",  BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);

            dtpSoDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString();
            dtpSoDtTo.Text = SystemBase.Base.ServerTime("YYMMDD");

            this.Text = "수주 참조 조회";

            txtProjectNo.Text = ProjectNo;
            Search(false);
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

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            Search(true);
        }
        #endregion
        
        #region Search함수
        private void Search(bool Msg)
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

                string strQuery = " usp_SBA007  @pTYPE = 'S3'";
                strQuery += ", @pSO_DT_FR = '" + dtpSoDtFr.Text + "' ";
                strQuery += ", @pSO_DT_TO = '" + dtpSoDtTo.Text + "'";
                strQuery += ", @pSO_NO = '" + txtSoNo.Text + "' ";
                strQuery += ", @pPROJECT_NO = '" + txtProjectNo.Text + "'";
                strQuery += ", @pPROJECT_NM = '" + txtProjectNm.Text + "'";
                strQuery += ", @pSHIP_CD = '" + txtShipCd.Text + "'";
                strQuery += ", @pCHK_YN = '" + InsertChkYn + "'";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";

                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, Msg, 0, 0, true);
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            this.Cursor = System.Windows.Forms.Cursors.Default;
        }
        #endregion

        #region 그리드 더블클릭 이벤트
        private void fpSpread1_CellDoubleClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            strProjectNo = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text;
            strProjectNm = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트명")].Text;
            strProjectSeq = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Text;
            strEntCd = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "사업코드")].Text;
            strEntNm = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "사업명")].Text;
            strShipCd = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "납품처")].Text;
            strShipNm = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "납품처명")].Text;
            strItemCd = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text;
            strItemNm = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목명")].Text;

            this.DialogResult = DialogResult.OK;

            this.Close();
        }
        #endregion
    }
}
