using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace IN.INV122
{
    public partial class INV122P1 : UIForm.FPCOMM1
    {
        #region 변수선언
        string strPlant;
        string strPlantNm;
        string strItem;
        string strItemNm;
        string strUnit;
        string strYm;
        bool first = false;
        #endregion

        public INV122P1(string plant, string plant_nm, string item, string item_nm, string unit, string ym)
        {
            //
            // Windows Form 디자이너 지원에 필요합니다.
            //
            InitializeComponent();

            strPlant = plant;
            strPlantNm = plant_nm;
            strItem = item;
            strItemNm = item_nm;
            strUnit = unit;
            strYm = ym;
        }

        public INV122P1()
        {
            InitializeComponent();
        }

        #region Form Load 시
        private void INV122P1_Load(object sender, System.EventArgs e)
        {
            this.Text = "수불상세 팝업";

            SystemBase.Validation.GroupBox_Setting(groupBox1);

            txtPlant.Value = strPlant;
            txtPlantNm.Value = strPlantNm;
            txtItemCd.Value = strItem;
            txtItemNm.Value = strItemNm;
            dtpTranDt.Value = strYm;
            txtUnit.Value = strUnit;

            UIForm.Buttons.ReButton("010000001000", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
            first = true;
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            //조회조건 필수 체크
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1) == true)
            {
                this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

                try
                {
                    string strQuery = " usp_INV122  @pTYPE = 'S2'";
                    strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' ";
                    strQuery += ", @pITEM_CD = '" + strItem + "' ";
                    strQuery += ", @pYEAR_MON = '" + dtpTranDt.Text.Replace("-", "") + "' ";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0, true);
                    fpSpread1.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.Normal;

                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
                }

                this.Cursor = System.Windows.Forms.Cursors.Default;
            }
        }
        #endregion

        #region 버튼 클릭
        private void butCancel_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }
        #endregion


        #region INV122P1_Activated
        private void INV122P1_Activated(object sender, System.EventArgs e)
        {
            if (first) SearchExec();
            first = false;
        }
        #endregion

    }
}
