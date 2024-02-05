#region 작성정보
/*********************************************************************/
// 단위업무명:  실시간TOUCH집계현황
// 작 성 자  :  한 미 애
// 작 성 일  :  2020-12-24
// 작성내용  :  작업대기 상태 작업자들에 대한 작업배정 및 TOUCH실적 조회
// 수 정 일  :
// 수 정 자  :
// 수정내용  :
// 비    고  :
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

namespace PC.PEA008
{
    public partial class PEA008P1 : UIForm.FPCOMM1
    {
        #region 변수선언
        #endregion

        #region 생성자
        public PEA008P1(string ProjectNo, string ProjectSeq, string ItemCd, string WoNo)
        {
            InitializeComponent();
        }

        public PEA008P1()
        {
            InitializeComponent();
        }
        #endregion

        #region 폼로드 이벤트
        private void PEA008P1_Load(object sender, System.EventArgs e)
        {
            this.Text = "작업대기 상태 작업자 작업배정 및 TOUCH실적 조회";

            //버튼 재정의(조회권한만)
            UIForm.Buttons.ReButton("000000000001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);

            txtCurrentDate.Value = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0, 10);

            string Query = " usp_PEA008 @pTYPE = 'S4'";
            Query += ", @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
            Query += ", @pWC_CD = '" + txtWcCd.Text + "'";
            Query += ", @pPROJECT_NO = '" + txtProjectNo.Text + "'";
            Query += ", @pH_RES_CD= '" + txtWorkDuty.Text + "'";

            UIForm.FPMake.grdCommSheet(fpSpread1, Query, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0, true);
        }
        #endregion
    }
}
