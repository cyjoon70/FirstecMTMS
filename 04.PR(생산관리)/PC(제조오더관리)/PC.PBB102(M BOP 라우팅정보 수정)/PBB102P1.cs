using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PC.PBB102
{
    public partial class PBB102P1 : Form
    {
        #region 변수선언
        string[] returnVal = null;
        public string[] ReturnVal // property 설정
        {
            get { return returnVal; }
        }

        string strProjectNo = "";
        string strProjectSeq = "";
        string strGroupCd = "";
        string strPlantCd = "";
        string strItemCd = "";

        string[] strPHeadText = new string[] { };
        string[] strPTxtAlign = new string[] { };
        string[] strPCellType = new string[] { };
        int[] strHeadWidth = new int[] { };

        string[] PHeadText = null;
        string[] PTxtAlign = null;
        string[] PCellType = null;
        int[] PHeadWidth = null;
        #endregion

        #region 생성자
        public PBB102P1(string PROJECT_NO, string PROJECT_SEQ, string GROUP_CD,
                        string PLANT_CD, string ITEM_CD)
        {

            strProjectNo = PROJECT_NO;
            strProjectSeq = PROJECT_SEQ;
            strGroupCd = GROUP_CD;
            strPlantCd = PLANT_CD;
            strItemCd = ITEM_CD;

            //
            // Windows Form 디자이너 지원에 필요합니다.
            //
            InitializeComponent();

            //
            // TODO: InitializeComponent를 호출한 다음 생성자 코드를 추가합니다.
            //
        }
        #endregion
        
        #region Form Load 시
        private void PBB102P1_Load(object sender, System.EventArgs e)
        {

            if (SystemBase.Base.ProgramWhere.Length > 0)
            {

                string HeadQuery = "";
                string routFrmId = "WP1002"; //  라우팅 FORM ID

                HeadQuery += " SELECT HEAD_ONE, DATA_ALIGN, DATA_TYPE, HEAD_WIDTH ";
                HeadQuery += " FROM   CO_GRID_DESIGN ";
                HeadQuery += " WHERE   FORM_ID='" + routFrmId + "'";
                HeadQuery += " ORDER BY DATA_SEQ ";

                DataTable dt = SystemBase.DbOpen.TranDataTable(HeadQuery);
                int G1RowCount = dt.Rows.Count;

                if (G1RowCount > 0)
                {
                    PHeadText = new string[G1RowCount];
                    PTxtAlign = new string[G1RowCount];
                    PCellType = new string[G1RowCount];
                    PHeadWidth = new int[G1RowCount];

                    for (int i = 0; i < G1RowCount; i++)
                    {
                        PHeadText[i] = dt.Rows[i][0].ToString();
                        PTxtAlign[i] = dt.Rows[i][1].ToString();
                        PCellType[i] = dt.Rows[i][2].ToString();
                        PHeadWidth[i] = Convert.ToInt32(dt.Rows[i][3].ToString());
                    }
                }
            }

            // 그리드 HEAD정보 설정
            strPHeadText = PHeadText;
            strPTxtAlign = PTxtAlign;
            strPCellType = PCellType;
            strHeadWidth = PHeadWidth;

            // 공정확인조회
            string Query = " usp_PBB102 @pType='S3', ";
            Query += " @pPROJECT_NO='" + strProjectNo + "',";
            Query += " @pPROJECT_SEQ='" + strProjectSeq + "',";
            Query += " @pGROUP_CD='" + strGroupCd + "',";
            Query += " @pPLANT_CD='" + strPlantCd + "',";
            Query += " @pITEM_CD='" + strItemCd + "'";
            Query += " ,@pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

            UIForm.FPMake.grdMakeSheet(fpSpread1, Query, strPHeadText, strPTxtAlign, strPCellType, strHeadWidth);

        }
        #endregion

        #region 닫기 클릭시
        private void btnClose_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }
        #endregion

        #region CELL 더블 클릭시
        private void fpSpread1_CellDoubleClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            try
            {
                returnVal = new string[2];
                returnVal[0] = fpSpread1.Sheets[0].Cells[e.Row, 0].Value.ToString();
                returnVal[1] = fpSpread1.Sheets[0].Cells[e.Row, 1].Value.ToString();

                this.DialogResult = DialogResult.OK;

            }
            catch { }
        }
        #endregion


    }
}
