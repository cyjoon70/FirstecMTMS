#region 작성정보
/*********************************************************************/
// 단위업무명 :  부품미출고조회
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-02-13
// 작성내용 : 부품미출고조회 관리
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

namespace PC.PCC012
{  
    public partial class PCC007P1 : UIForm.FPCOMM1
    {
        string strProjectNo = "", strProjectSeq = "", strItemCd = "", strWoNo = "";

        public PCC007P1(string ProjectNo, string ProjectSeq, string ItemCd, string WoNo)
        {
            //
            // Windows Form 디자이너 지원에 필요합니다.
            //
            strProjectNo = ProjectNo;
            strProjectSeq = ProjectSeq;
            strItemCd = ItemCd;
            strWoNo = WoNo;

            InitializeComponent();

            //
            // TODO: InitializeComponent를 호출한 다음 생성자 코드를 추가합니다.
            //
        }

        #region Form Load 시
        private void PCC007P1_Load(object sender, System.EventArgs e)
        { 
            //GroupBo x1 초기화
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            this.Text = "공정진행현황";

            //버튼 재정의(조회권한만)
            UIForm.Buttons.ReButton("000000000001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);

            string Query = " usp_PCC007 @pTYPE = 'S3'";
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
