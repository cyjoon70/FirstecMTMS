#region 작성정보
/*********************************************************************/
// 단위업무명 : 생산요약정보조회
// 작 성 자 : 김 현 근
// 작 성 일 : 2013-04-15
// 작성내용 : 생산요약정보조회
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

namespace PC.PSB001 
{
    public partial class PSB001P1 : UIForm.FPCOMM1
    {
        #region 변수선언
        string strProjectNo = "", strProjectSeq = "", strItemCd = "", strWoNo = "";
        #endregion

        public PSB001P1()
        {
            InitializeComponent();
        }

        public PSB001P1(string ProjectNo, string ProjectSeq, string ItemCd, string WoNo)
        {
            strProjectNo = ProjectNo;
            strProjectSeq = ProjectSeq;
            strItemCd = ItemCd;
            strWoNo = WoNo;

            InitializeComponent();
        }

        #region Form Load 시
        private void PSB001P1_Load(object sender, System.EventArgs e)
        {
            //버튼 재정의
            UIForm.Buttons.ReButton("000000000001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);
            
            string Query = " usp_PSB001 @pTYPE = 'S3'";
            Query += ", @pPROJECT_NO = '" + strProjectNo + "' ";
            Query += ", @pPROJECT_SEQ = '" + strProjectSeq + "' ";
            Query += ", @pITEM_CD = '" + strItemCd + "' ";
            Query += ", @pWORKORDER_NO = '" + strWoNo + "' ";
            Query += ", @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

            UIForm.FPMake.grdCommSheet(fpSpread1, Query, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0, true);
            
            if (fpSpread1.Sheets[0].Rows.Count > 0)
            {
                txtWorkOrderNo.Text = strWoNo;
            }            
        }
        #endregion
    }
}
