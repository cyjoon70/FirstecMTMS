using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CC.CCA002
{
    public partial class CCA002P1 : UIForm.FPCOMM1
    {
        #region 변수선언
        string strPlantCd = "", strProjectNo = "";
        #endregion

        #region 생성자
        public CCA002P1(string PlantCd, string ProjectNo)
        {
            strPlantCd = PlantCd;
            strProjectNo = ProjectNo;

            InitializeComponent();
        }

        public CCA002P1()
        {
            InitializeComponent();
        }
        #endregion

        #region 폼로드 이벤트
        private void CCA002P1_Load(object sender, EventArgs e)
        {
            this.Text = "구성품별 조회(개당)";

            //버튼 재정의
            UIForm.Buttons.ReButton("010000000001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);

            Grid_search(strPlantCd, strProjectNo, false);
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        { Grid_search(strPlantCd, strProjectNo, true); }
        #endregion

        #region 조회함수
        private void Grid_search(string strPlantCd, string strProjectNo, bool Msg)
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {
                string strQuery = "usp_CCA002 @pTYPE = 'S2'";
                strQuery = strQuery + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' ";
                strQuery = strQuery + ", @pPLANT_CD = '" + strPlantCd + "' ";
                strQuery = strQuery + ", @pPROJECT_NO = '" + strProjectNo + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, Msg, 0, 0, true);
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회중 오류가 발생하였습니다.
            }

            this.Cursor = Cursors.Default;
        }
        #endregion
    }
}
