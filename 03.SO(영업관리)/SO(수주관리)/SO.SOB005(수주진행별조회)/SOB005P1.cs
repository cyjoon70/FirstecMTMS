#region 작성정보
/*********************************************************************/
// 단위업무명 : 구매정보팦업
// 작 성 자 : 김 현 근
// 작 성 일 : 2013-04-11
// 작성내용 : 구매정보팦업
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

namespace SO.SOB005
{
    public partial class SOB005P1 : UIForm.FPCOMM1
    {
        #region 변수선언
        string strPoNo, strProjectNo, strProjectSeq, strItemCd, strItemNm;
        #endregion

        #region 생성자
        public SOB005P1()
        {
            InitializeComponent();
        }
        public SOB005P1(string PoNo, string ProjectNo, string ProjectSeq, string ItemCd, string ItemNm)
        {
            strPoNo = PoNo;
            strProjectNo = ProjectNo;
            strProjectSeq = ProjectSeq;
            strItemCd = ItemCd;
            strItemNm = ItemNm;

            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void SOB005P1_Load(object sender, System.EventArgs e)
        {
            this.Text = "구매정보참조팝업";

            //버튼 재정의
            UIForm.Buttons.ReButton("000000000001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);

            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            //그리드 초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0);

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
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            //조회조건 필수 체크
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1) == true)
            {
                try
                {
                    string strQuery = "usp_SOB005 @pTYPE = 'P1'";
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
            }
            this.Cursor = System.Windows.Forms.Cursors.Default;
        }
        #endregion

    }
}
