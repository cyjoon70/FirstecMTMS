#region 작성정보
/*********************************************************************/
// 단위업무명 : 
// 작 성 자   : 김 창 진
// 작 성 일   : 2014-08-29
// 작성내용   : 사용재고현황 조회(이동현황조회)
// 수 정 일   :
// 수 정 자   :
// 수정내용   :
// 비    고   :
/*********************************************************************/
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using WNDW;

namespace QT.QTC005
{
    public partial class QTC005P1 : UIForm.FPCOMM1
    {
        #region 변수선언
        string returnVal = "";
        string strProjectNo = "";
        string strProjectSeq = "";
        string strInspReqNo = "";
        string strUnityReqNo = "";
        string strWorkOrderNo = "";
        string strItemCd = "";
        string strInspDt = "";
        string strUnit = "";
        string strInspQty = "";
        string strMvmtNo = "";
        string strMvmtSeq = "";
        #endregion

        #region 생성자
        public QTC005P1(string sProjectNo, string sProjectSeq, string sInspReqNo, string sUnityReqNo, string sWorkOrderNo, string sItemCd, string sInspDt, string sUnit, string sInspQty, string sMvmtNo, string sMvmtSeq)
        {
            strProjectNo = sProjectNo;
            strProjectSeq = sProjectSeq;
            strInspReqNo = sInspReqNo;
            strUnityReqNo = sUnityReqNo;
            strWorkOrderNo = sWorkOrderNo;
            strItemCd = sItemCd;
            strInspDt = sInspDt;
            strUnit = sUnit;
            strInspQty = sInspQty;
            strMvmtNo = sMvmtNo;
            strMvmtSeq = sMvmtSeq;

            InitializeComponent();
        }

        public QTC005P1()
        {
            InitializeComponent();
        }
        #endregion

        #region 폼로드 이벤트
        private void QTC005P1_Load(object sender, System.EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            this.Text = "이동현황조회";

            txtProjectNo.Value = strProjectNo;
            txtProjectSeq.Value = strProjectSeq;
            txtInspReqNo.Value = strInspReqNo;
            txtUnityInspReqNo.Value = strUnityReqNo;
            txtMvmtNo.Value = strMvmtNo;
            txtMvmtSeq.Value = strMvmtSeq;
            txtItemCd.Value = strItemCd;
            dtpInspDt.Value = strInspDt;
            txtUnit.Value = strUnit;
            txtInspQty.Value = strInspQty;
            
            //버튼 재정의(조회권한만)
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

                string strQuery = " usp_QTC005 @pTYPE = 'P1'";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                strQuery += ", @pMVMT_NO = '" + txtMvmtNo.Text + "' ";
                strQuery += ", @pMVMT_SEQ = '" + txtMvmtSeq.Text + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0, true);
                
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            this.Cursor = System.Windows.Forms.Cursors.Default;
        }
        #endregion

        #region TextChanged
        //품목코드
        private void txtItemCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtItemCd.Text != "")
                {
                    txtItemNm.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtItemCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtItemNm.Value = "";
                }
            }
            catch
            {

            }
        }
        

        //프로젝트번호
        private void txtProjectNo_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtProjectNo.Text != "")
                {
                    txtProjectNm.Value = SystemBase.Base.CodeName("PROJECT_NO", "PROJECT_NM", "S_SO_MASTER", txtProjectNo.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtProjectNm.Value = "";
                }
            }
            catch
            {

            }

            if (txtProjectNm.Text == "")
            {
                txtProjectSeq.Value = "";
            }
        }
        #endregion
    }
}
