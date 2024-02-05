using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using WNDW;

namespace ME.MEB001
{
    public partial class MEA002P1 : UIForm.FPCOMM1
    {
        #region 변수선언
        string strEstNo;
        string strEstSeq;
        string strCustCd;
        string strDiv;
        #endregion

        #region 생성자
        public MEA002P1()
        {
            InitializeComponent();
        }

        public MEA002P1(string EstNo, string Gubun, string EstSeq)
        {
            //
            // Windows Form 디자이너 지원에 필요합니다.
            //
            InitializeComponent();

            strEstNo = EstNo;
            strDiv = Gubun;
            strEstSeq = EstSeq;
        }

        public MEA002P1(string EstNo, string Gubun, string EstSeq, string CustCd)
        {
            //
            // Windows Form 디자이너 지원에 필요합니다.
            //
            InitializeComponent();

            strCustCd = CustCd;
            strEstNo = EstNo;
            strDiv = Gubun;
            strEstSeq = EstSeq;
        }
        #endregion

        #region Form Load 시
        private void MEA002P3_Load(object sender, System.EventArgs e)
        {
            this.Text = "품질증빙팝업";

            UIForm.Buttons.ReButton("010000001001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);

            if (strDiv == "1")
            {
                this.Text = "요구품질증빙팝업";
            }
            else
            {
                this.Text = "기능품질증빙팝업";
            }

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
                string strQuery = " usp_MEA002 ";
                if (strDiv == "1") strQuery += " @pTYPE = 'P1'";
                else strQuery += " @pTYPE = 'P2'";

                strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' ";
                strQuery += ", @pEST_NO = '" + strEstNo + "' ";
                strQuery += ", @pEST_SEQ = '" + strEstSeq + "' ";
                strQuery += ", @pCUST_CD = '" + strCustCd + "'";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0);
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }

            this.Cursor = System.Windows.Forms.Cursors.Default;
        }
        #endregion

        #region 버튼 Click
        private void butCancel_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }
        #endregion

    }
}
