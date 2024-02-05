#region 작성정보
/*********************************************************************/
// 단위업무명 : 발주번호별진행조회
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-04-17
// 작성내용 : 발주번호별진행조회 및 관리
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

namespace MP.MPO504
{  
    public partial class MPO504P2 : UIForm.FPCOMM1
    {
        #region 변수선언
        string strPoNo = "";
        string strPoSeq = "";
        string strItemCd = "";
        string strItemNm = "";
        #endregion

        #region 생성자
        public MPO504P2()
        {
            InitializeComponent();           
        }
        public MPO504P2(string PoNo, string PoSeq, string ItemCd, string ItemNm)
        {

            InitializeComponent();
            strPoNo = PoNo;
            strPoSeq = PoSeq;
            strItemCd = ItemCd;
            strItemNm = ItemNm;

        }
        #endregion

        #region Form Load 시
        private void MPO504P2_Load(object sender, System.EventArgs e)
        {
            this.Text = "품목별발주번호별진행조회";
            //GroupBo x1 초기화
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            UIForm.Buttons.ReButton("010000000001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);

            //그리드 초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

            //기타 세팅	
            txtPoNo.Value = strPoNo;
            txtPoSeq.Value = strPoSeq;
            txtItemCd.Value = strItemCd;
            txtItemNm.Value = strItemNm;

            btnItemCd.Enabled = false;

            SearchExec();
        }
        #endregion

        #region SearchExec()
        protected override void SearchExec()
        {
            //조회조건 필수 체크
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                this.Cursor = Cursors.WaitCursor;

                try
                {

                    string strQuery = "usp_MPO504 @pTYPE = 'S3'";
                    strQuery += ", @pPO_NO = '" + txtPoNo.Text + "'";
                    strQuery += ", @pPO_SEQ = '" + txtPoSeq.Text + "'";
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
	
    }
}
