#region 작성정보
/*********************************************************************/
// 단위업무명 : 예외입/출고등록
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-02-18
// 작성내용 : 예외입/출고등록 및 관리
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
using WNDW;

namespace IT.ITR001
{  
    public partial class ITR001P1 : UIForm.FPCOMM1
	{

		#region 변수선언
		string strState = "N";
        public string strItemCd = "";
		public string[] returnVal = null;
		#endregion

		#region 생성자
		public ITR001P1(string item_cd)
        {
            strItemCd = item_cd;
            InitializeComponent();           
        }
		#endregion

		#region Form Load 시
		private void ITR001P1_Load(object sender, System.EventArgs e)
        {
            this.Text = "현재고 팝업";

			//버튼 재정의(조회권한만)
			UIForm.Buttons.ReButton("010000001001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);

			//GroupBo x1 초기화
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            SystemBase.ComboMake.C1Combo(cboPlantCd, "usp_B_COMMON @pType='B031', @pSPEC1 = '" + SystemBase.Base.gstrBIZCD + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//공장
            SystemBase.ComboMake.C1Combo(cboItemAcct, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'B036', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0); //품목계정

            txtITEM_CD.Value = strItemCd;

            string strItemAcct = SystemBase.Base.CodeName("ITEM_CD", "ITEM_ACCT", "B_ITEM_INFO", strItemCd, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
            
			if (strItemAcct == "")
            {
                cboItemAcct.SelectedIndex = 3;
            }
            else
            {
                cboItemAcct.SelectedValue = strItemAcct;
            }

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

			if (string.IsNullOrEmpty(txtITEM_CD.Text) == false) { SearchExec(); }

        }
        #endregion

		#region SearchExec() 그리드 조회 로직
		protected override void SearchExec()
		{
			this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

			try
			{
				string strQuery = " usp_ITR001  @pTYPE = 'P1'";
				strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' ";
				strQuery += ", @pPLANT_CD = '" + cboPlantCd.SelectedValue + "' ";
				strQuery += ", @pITEM_ACCT = '" + cboItemAcct.SelectedValue + "' ";
				strQuery += ", @pITEM_CD = '" + txtITEM_CD.Text.Trim() + "' ";
				strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

				UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0, true);

				fpSpread1.Sheets[0].Columns[SystemBase.Base.GridHeadIndex(GHIdx1, "Lot 추적")].Locked = true;

			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}

			this.Cursor = System.Windows.Forms.Cursors.Default;
		}
		#endregion

        #region 버튼 Click
        private void btnOk_Click(object sender, System.EventArgs e)
        {
            RtnStr(fpSpread1.Sheets[0].ActiveRowIndex);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void butCancel_Click(object sender, System.EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
        #endregion

        #region 값 전송
        public string[] ReturnVal { get { return returnVal; } set { returnVal = value; } }

        public void RtnStr(int R)
        {
            if (fpSpread1.Sheets[0].Rows.Count > 0)
            {
                returnVal = new string[fpSpread1.Sheets[0].Columns.Count];
                for (int i = 0; i < fpSpread1.Sheets[0].Columns.Count; i++)
                {
                    returnVal[i] = fpSpread1.Sheets[0].Cells[R, i].Value.ToString();
                }
            }
        }
        #endregion

        #region fpSpread1
        private void fpSpread1_ButtonClicked(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            fpSpread1.Sheets[0].RowHeader.Cells[e.Row, 0].Text = "";
        }

        private void fpSpread1_CellDoubleClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            RtnStr(e.Row);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        #endregion

        #region btnITEM_Click
        private void btnITEM_Click(object sender, System.EventArgs e)
        {
            WNDW005 pu1 = new WNDW005(cboPlantCd.SelectedValue.ToString(), cboItemAcct.SelectedValue.ToString(), txtITEM_CD.Text);
            pu1.ShowDialog();
            if (pu1.DialogResult == DialogResult.OK)
            {
                string[] Msgs1 = pu1.ReturnVal;

                txtITEM_CD.Value = Msgs1[2].ToString();
                txtITEM_NM.Value = Msgs1[3].ToString();
            }
        }

        private void txtITEM_CD_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtITEM_CD.Text != "")
                {
                    txtITEM_NM.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtITEM_CD.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtITEM_NM.Value = "";
                }
            }
            catch
            {

            }
        }
        #endregion

    }
}
