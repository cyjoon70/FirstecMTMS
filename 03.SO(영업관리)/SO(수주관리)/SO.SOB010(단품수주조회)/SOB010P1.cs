using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SO.SOB010
{
    public partial class SOB010P1 : UIForm.FPCOMM1
    {
        #region 변수선언
        string strProjectNo = "";
        string strProjectSeq = "";
        string strItemCd = "";
        string strPoNo = "";
        string strItemNm = "";
        #endregion

        #region 생성자
        public SOB010P1(string PoNo, string ProjectNo, string ProjectSeq, string ItemCd, string ItemNm)
        {
            strPoNo = PoNo;
            strProjectNo = ProjectNo;
            strProjectSeq = ProjectSeq;
            strItemCd = ItemCd;
            strItemNm = ItemNm;

            InitializeComponent();
        }

        public SOB010P1()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void SOB010P1_Load(object sender, System.EventArgs e)
        {
            this.Text = "구매정보참조팝업";

            //버튼 재정의
            UIForm.Buttons.ReButton("000000000001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);

            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            //그리드 초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

            txtPoNo.Value = strPoNo;
            txtProjectNo.Value = strProjectNo;
            txtProjectSeq.Value = strProjectSeq;
            txtItemCd.Value = strItemCd;
            txtItemNm.Value = strItemNm;

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
                    string strQuery = "usp_SOB010 @pTYPE = 'P1'";
                    strQuery += ", @pPROJECT_NO = '" + txtProjectNo.Text + "'";
                    strQuery += ", @pPROJECT_SEQ = '" + txtProjectSeq.Text + "'";
                    strQuery += ", @pITEM_CD = '" + txtItemCd.Text + "'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0, true);
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
    }
}
