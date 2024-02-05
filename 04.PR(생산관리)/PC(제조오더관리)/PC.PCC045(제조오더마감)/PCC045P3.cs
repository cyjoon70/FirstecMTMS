#region 작성정보
/*********************************************************************/
// 단위업무명 : 제조오더마감
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-02-04
// 작성내용 : 제조오더마감 관리
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

namespace PC.PCC045
{  
    public partial class PCC045P3 : UIForm.FPCOMM1
    {
        #region 변수선언
        string strProjectNo = "", strProjectSeq = "", strItemCd = "", strWoNo = "";
        #endregion

        #region 생성자
        public PCC045P3(string ProjectNo, string ProjectSeq, string ItemCd, string WoNo)
        {
            strProjectNo = ProjectNo;
            strProjectSeq = ProjectSeq;
            strItemCd = ItemCd;
            strWoNo = WoNo;

            InitializeComponent();           
        }
        #endregion

        #region Form Load 시
        private void PCC045P3_Load(object sender, System.EventArgs e)
        {
            this.Text = "공정진행현황";
            //GroupBo x1 초기화
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            UIForm.Buttons.ReButton("000000000001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);

            string Query = " usp_PCC045 @pTYPE = 'S3'";
            Query += ", @pPROJECT_NO = '" + strProjectNo + "' ";
            Query += ", @pPROJECT_SEQ = '" + strProjectSeq + "' ";
            Query += ", @pITEM_CD = '" + strItemCd + "' ";
            Query += ", @pWORKORDER_NO = '" + strWoNo + "' ";
            Query += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

            UIForm.FPMake.grdCommSheet(fpSpread1, Query, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0, true);

            if (fpSpread1.Sheets[0].Rows.Count > 0)
            {
                txtWorkOrderNo.Text = strWoNo;
            }
        }
        #endregion

    }
}
