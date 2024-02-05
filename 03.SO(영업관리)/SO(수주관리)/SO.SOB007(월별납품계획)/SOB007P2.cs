using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace SO.SOB007
{
    public partial class SOB007P2 : UIForm.FPCOMM1
    {
        #region 변수선언
        string strProjNo = "";
        string strProjSeq = "";
        string strEntNm = "";
        #endregion

        #region 생성자
        public SOB007P2(string ProjNo, string ProjSeq, string EntNm)
        {
            //
            // Windows Form 디자이너 지원에 필요합니다.
            //
            strProjNo = ProjNo;
            strProjSeq = ProjSeq;
            strEntNm = EntNm;

            InitializeComponent();

            //
            // TODO: InitializeComponent를 호출한 다음 생성자 코드를 추가합니다.
            //
        }

        public SOB007P2()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void SOB007P2_Load(object sender, System.EventArgs e)
        {

            //버튼 재정의
            UIForm.Buttons.ReButton("000000000001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);

            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            txtProjectNo.Value = strProjNo;
            txtProjectSeq.Value = strProjSeq;
            txtEntCd.Value = strEntNm;

            SearchExec();
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            //조회조건 필수 체크
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1) == true)
            {
                this.Cursor = Cursors.WaitCursor;

                try
                {

                    string strQuery = "usp_PSA027 @pTYPE = 'S2'";
                    strQuery += ", @pPROJECT_NO = '" + strProjNo + "'";
                    strQuery += ", @pPROJECT_SEQ = '" + strProjSeq + "'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0, true);


                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회중 오류가 발생하였습니다.
                }

                this.Cursor = Cursors.Default;
            }
        }
        #endregion

        #region 닫기 버튼클릭
        private void button1_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }
        #endregion
    }
}
