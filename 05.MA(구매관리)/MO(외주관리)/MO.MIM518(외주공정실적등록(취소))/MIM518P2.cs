#region 작성정보
/*********************************************************************/
// 단위업무명 : 외주공정실적등록/취소
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-04-08
// 작성내용 : 외주공정실적등록/취소 및 관리
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

namespace MO.MIM518
{  
    public partial class MIM518P2 : UIForm.FPCOMM1
    {
        #region 변수선언
        string strWo_No = "", strProcSeq = "";
        #endregion

        #region 생성자
        public MIM518P2(string Wo_No, string ProcSeq)
        {
            strWo_No = Wo_No;
            strProcSeq = ProcSeq;

            InitializeComponent();
        }

        public MIM518P2()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void MIM518P2_Load(object sender, System.EventArgs e)
        {
            this.Text = "부품내역";
            //GroupBo x1 초기화
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            txtWorkOrderNo.Value = strWo_No;
            txtProcSeq.Value = strProcSeq;

            UIForm.Buttons.ReButton("010000000001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);

            string Query = " usp_MIM518 @pTYPE = 'S4'";
            Query += ", @pBIZ_CD = '" + SystemBase.Base.gstrBIZCD + "' ";
            Query += ", @pWORKORDER_NO = '" + strWo_No + "' ";
            Query += ", @pPROC_SEQ = '" + strProcSeq + "' ";
            Query += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

            UIForm.FPMake.grdCommSheet(fpSpread1, Query, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0, true);		
        }
        #endregion

    }
}
