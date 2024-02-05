using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace PA.PBA103
{
    public partial class PBA103P2 : UIForm.FPCOMM1
    {
        #region 변수선언
        string INIT_SCH_ID = "";

        FarPoint.Win.Spread.FpSpread spd;
        string[] returnVal = null;
        #endregion

        public PBA103P2(string SCH_ID, FarPoint.Win.Spread.FpSpread spread)
        {
            //
            // Windows Form 디자이너 지원에 필요합니다.
            //
            InitializeComponent();

            //
            // TODO: InitializeComponent를 호출한 다음 생성자 코드를 추가합니다.
            //

            INIT_SCH_ID = SCH_ID;
            spd = spread;
        }

        public PBA103P2()
        {
            InitializeComponent();
        }

        #region Form Load 시
        private void PBA103P2_Load(object sender, System.EventArgs e)
        {
			this.Text = "휴무선택 팝업";

            UIForm.Buttons.ReButton("010000001000", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);

			//필수 체크
			SystemBase.Validation.GroupBox_Setting(groupBox1);

			//콤보박스설정
            SystemBase.ComboMake.C1Combo(cboSch_id, "usp_PBA103 'C1', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);
            SystemBase.ComboMake.C1Combo(cboPlant_cd, "usp_PBA103 'C4', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);

			UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

			//기타세팅
			cboSch_id.SelectedValue = INIT_SCH_ID;
            dtpYear.Text = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0, 4); 
            SearchExec();
		}        
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {

                    string Query = " usp_PBA103 @pType='S5', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

                    UIForm.FPMake.grdCommSheet(fpSpread1, Query, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }

            this.Cursor = System.Windows.Forms.Cursors.Default;
        }
        #endregion

        #region 버튼 Click
        private void butOk_Click(object sender, System.EventArgs e)
        {
            int col_sel = SystemBase.Base.GridHeadIndex(GHIdx1, "선택");

            try
            {
                int j = spd.Sheets[0].Rows.Count;
                for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                {
                    if (fpSpread1.Sheets[0].Cells[i, col_sel].Text == "True")
                    {
                        spd.ActiveSheet.ActiveRowIndex = j;
                        UIForm.FPMake.RowInsert(spd);

                        DateTime dt = Convert.ToDateTime(dtpYear.Value.ToString().Substring(0,4) + "-" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "휴일")].Value);

                        spd.Sheets[0].Cells[j, 1].Value = dt.Date;  //시작일
                        spd.Sheets[0].Cells[j, 2].Value = dt.Date;   //종료일
                        spd.Sheets[0].Cells[j, 3].Value = "1";	   //교대
                        spd.Sheets[0].Cells[j, 4].Value = "0000";
                        spd.Sheets[0].Cells[j, 5].Value = "0000";
                        spd.Sheets[0].Cells[j, 6].Value = 0;
                        spd.Sheets[0].Cells[j, 7].Text = "휴무 일괄 선택";

                        j++;
                    }
                }
                RtnStr();

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(f.Message, SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            Close();
            this.DialogResult = DialogResult.OK;
        }

        private void butCancel_Click(object sender, System.EventArgs e)
        {
            Close();
            this.DialogResult = DialogResult.Cancel;
        }
        #endregion

        #region 값 전송
        public string[] ReturnVal { get { return returnVal; } set { returnVal = value; } }

        public void RtnStr()
        {
            returnVal = new string[2];
            returnVal[0] = cboSch_id.SelectedValue.ToString();
            returnVal[1] = cboPlant_cd.SelectedValue.ToString();
        }
        #endregion
    }
}
