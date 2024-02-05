using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PC.PCC013
{
    public partial class PCC013P1 : UIForm.FPCOMM1
    {
        string strProjectNo = "", strProjectSeq = "", strItemCd = "", strWoNo = "";

        public PCC013P1(string ProjectNo, string ProjectSeq, string ItemCd, string WoNo)
        {
            strProjectNo = ProjectNo;
            strProjectSeq = ProjectSeq;
            strItemCd = ItemCd;
            strWoNo = WoNo;

            InitializeComponent();
        }

        public PCC013P1()
        {
            InitializeComponent();
        }

        #region 폼로드 이벤트
        private void PCC013P1_Load(object sender, System.EventArgs e)
        {
            this.Text = "공정진행현황";

            //버튼 재정의(조회권한만)
            UIForm.Buttons.ReButton("000000000001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);

            string Query = " usp_PCC013 @pTYPE = 'S3'";
            Query += ", @pPROJECT_NO = '" + strProjectNo + "' ";
            Query += ", @pPROJECT_SEQ = '" + strProjectSeq + "' ";
            Query += ", @pITEM_CD = '" + strItemCd + "' ";
            Query += ", @pWORKORDER_NO = '" + strWoNo + "' ";
            Query += ", @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

            UIForm.FPMake.grdCommSheet(fpSpread1, Query, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0, true);

            if (fpSpread1.Sheets[0].Rows.Count > 0)
            {
                txtWorkOrderNo.Value = strWoNo;
            }
        }
        #endregion
    }
}
