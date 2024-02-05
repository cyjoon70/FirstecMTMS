#region 작성정보
/*********************************************************************/
// 단위업무명 : 매입등록
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-04-08
// 작성내용 : 매입등록 및 관리
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

namespace PC.PCC031
{
    public partial class PCC007P1 : UIForm.FPCOMM1
    {
        #region 변수선언
        string strProjectNo = "", strProjectSeq = "", strItemCd = "", strWoNo = "";
        #endregion

        #region 생성자
        public PCC007P1(string ProjectNo, string ProjectSeq, string ItemCd, string WoNo)
		{
			strProjectNo = ProjectNo;
			strProjectSeq = ProjectSeq;
			strItemCd = ItemCd;
			strWoNo = WoNo;

			InitializeComponent();
		}

		public PCC007P1()
		{
			InitializeComponent();
		}
        #endregion   

        #region 폼로드 이벤트
        private void PCC007P1_Load(object sender, System.EventArgs e)
        {
            this.Text = "공정진행현황";
                        
            //버튼 재정의(조회권한만)
            UIForm.Buttons.ReButton("000000000001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);

            string Query = " usp_PCC007 @pTYPE = 'S3'";
			Query += ", @pPROJECT_NO = '"+ strProjectNo +"' ";
			Query += ", @pPROJECT_SEQ = '"+ strProjectSeq +"' ";
			Query += ", @pITEM_CD = '"+ strItemCd +"' ";
			Query += ", @pWORKORDER_NO = '"+ strWoNo +"' ";
            Query += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

			UIForm.FPMake.grdCommSheet(fpSpread1, Query, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false,false, 0, 0, true);

			if(fpSpread1.Sheets[0].Rows.Count > 0)
			{
				txtWorkOrderNo.Text = strWoNo;
			}
        }
        #endregion        
        
    }
}
