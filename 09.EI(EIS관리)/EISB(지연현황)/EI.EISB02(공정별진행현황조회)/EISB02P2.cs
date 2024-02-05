#region 작성정보
/*********************************************************************/
// 단위업무명 : 공정별 진행현황조회
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-04-16
// 작성내용 : 공정별 진행현황조회 관리
// 수 정 일 :
// 수 정 자 :
// 수정내용 :
// 비    고 :
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

namespace EI.EISB02
{
    public partial class EISB02P2 : UIForm.FPCOMM1
    {
        #region 변수선언
        string strWo_No = "", strProcSeq = "";
        #endregion

        #region 생성자
        public EISB02P2(string Wo_No, string ProcSeq)
        {
            //
            // Windows Form 디자이너 지원에 필요합니다.
            //

            strWo_No = Wo_No;
            strProcSeq = ProcSeq;

            InitializeComponent();

            //
            // TODO: InitializeComponent를 호출한 다음 생성자 코드를 추가합니다.
            //
        }

        public EISB02P2()
        {
            //
            // Windows Form 디자이너 지원에 필요합니다.
            //
            InitializeComponent();

            //
            // TODO: InitializeComponent를 호출한 다음 생성자 코드를 추가합니다.
            //
        }
        #endregion

        #region 폼로드 이벤트
        private void EISB02P2_Load(object sender, System.EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            this.Text = "부품내역";

            txtWorkOrderNo.Value = strWo_No;
            txtProcSeq.Value = strProcSeq;

            //버튼 재정의(조회권한만)
            UIForm.Buttons.ReButton("000000000001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);

            string Query = " usp_PCC007 @pTYPE = 'S4'";
            Query += ", @pBIZ_CD = '" + SystemBase.Base.gstrBIZCD + "' ";
            Query += ", @pWORKORDER_NO = '" + strWo_No + "' ";
            Query += ", @pPROC_SEQ = '" + strProcSeq + "' ";
            Query += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

            UIForm.FPMake.grdCommSheet(fpSpread1, Query, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0, true);
		
        }
        #endregion

    }
}
