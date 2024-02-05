#region 작성정보
/************************************************************************/
// 단위업무명:  발주감안 재고이동처리
// 작 성 자  :  2017.09.11.
// 작 성 일  : 
// 작성내용  :  발주감안 재고이동의 팝업
// 비    고  :
/************************************************************************/
#endregion

using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using WNDW;

namespace IT.ITR117
{
    public partial class ITR117P1 : UIForm.FPCOMM1
    {
        #region 변수선언
        string strPoNo;
        string strPoSeq;
        decimal dPoQty;
        string strReqNo;
        string strReqSeq;
        decimal dReqQty;
        string strProjectNo;
        string strProjectSeq;
        string strItemCd;
        string strItemNm;
        decimal dPoRefQty;
        decimal dRcptSlQty;
        decimal dMoveQty;
        #endregion

        #region 생성자
        public ITR117P1(string PoNo, string PoSeq, decimal PoQty, string ReqNo, string ReqSeq, decimal ReqQty, string ProjectNo, string ProjectSeq, string ItemCd, string ItemNm, 
                        decimal PoRefQty, decimal RcptSlQty, decimal MoveQty)
        {
            //
            // Windows Form 디자이너 지원에 필요합니다.
            //
            InitializeComponent();

            strPoNo = PoNo;
            strPoSeq = PoSeq;
            dPoQty = PoQty;
            strReqNo = ReqNo;
            strReqSeq = ReqSeq;
            dReqQty = ReqQty;
            strProjectNo = ProjectNo;
            strProjectSeq = ProjectSeq;
            strItemCd = ItemCd;
            strItemNm = ItemNm;
            dPoRefQty = PoRefQty;
            dRcptSlQty = RcptSlQty;
            dMoveQty = MoveQty;
        }

        public ITR117P1()
        {
            InitializeComponent();
        }
		#endregion

		#region Form Load 시
		private void ITR117P1_Load(object sender, System.EventArgs e)
        {
            this.Text = "발주감안오더 입고 및 재고이동 상세 팝업";

            SystemBase.Validation.GroupBox_Setting(groupBox1);

            UIForm.Buttons.ReButton("010000000000", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);

            SystemBase.ComboMake.C1Combo(cboPlantCd, "usp_B_COMMON @pType='B031', @pSPEC1 = '" + SystemBase.Base.gstrBIZCD + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 9); //공장
			cboPlantCd.SelectedValue = SystemBase.Base.gstrPLANT_CD.ToString();

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0, true);

            // 매개변수로 받은 값들을 해당 항목에 지정
            ArgumentAssignLocking();
            
            SearchExec();
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            try
            {
				string strQuery = " usp_ITR117";
				strQuery += "  @pTYPE = 'P1'";                				
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' ";
                strQuery += ", @pPLANT_CD = '" + cboPlantCd.SelectedValue + "' ";
                strQuery += ", @pPO_NO = '" + txtPoNo.Text + "' ";
                strQuery += ", @pPO_SEQ = '" + txtPoSeq.Text + "' ";            

				UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0);
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }

            this.Cursor = System.Windows.Forms.Cursors.Default;
        }
        #endregion

        private void ArgumentAssignLocking()
        {
            txtPoNo.Text = strPoNo;
            txtPoSeq.Text = strPoSeq;
            txtPoQty.Text = dPoQty.ToString();
            txtReqNo.Text = strReqNo;
            txtReqSeq.Text = strReqSeq;
            txtReqQty.Text = dReqQty.ToString();
            txtProjectNo.Text = strProjectNo;
            txtProjectSeq.Text = strProjectSeq;
            txtItemCd.Text = strItemCd;
            txtItemNm.Text = strItemNm;
            txtPoRefQty.Text = dPoRefQty.ToString();
            txtRcptSlQty.Text = dRcptSlQty.ToString();
            txtMoveQty.Text = dMoveQty.ToString();

            txtPoNo.Enabled = false;
            txtPoSeq.Enabled = false;
            txtPoQty.Enabled = false;
            txtReqNo.Enabled = false;
            txtReqSeq.Enabled = false;
            txtReqQty.Enabled = false;
            txtProjectNo.Enabled = false;
            txtProjectSeq.Enabled = false;
            txtItemCd.Enabled = false;
            txtItemNm.Enabled = false;
            txtPoRefQty.Enabled = false;
            txtRcptSlQty.Enabled = false;
            txtMoveQty.Enabled = false;
        }
    }
}
