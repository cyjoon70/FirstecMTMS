#region 작성정보
/*********************************************************************/
// 단위업무명 : 발주변경
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-04-01
// 작성내용 : 발주변경 및 관리
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

namespace IT.ITR003
{
    public partial class ITR003P4 : UIForm.FPCOMM1
    {

        #region 변수선언
        string[] returnVal = null;
        string strState = "N";
        public string strItemCd = "";
        #endregion

        #region 생성자
        public ITR003P4(string item_cd)
        {
            InitializeComponent();
            strItemCd = item_cd;
        }

        public ITR003P4()
        {
            InitializeComponent();
        }
        #endregion

        #region 폼로드 이벤트
        private void ITR003P4_Load(object sender, System.EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            this.Text = "현재고 팝업";
                        
            //버튼 재정의(조회권한만)
            UIForm.Buttons.ReButton("010000001001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);

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

			txtITEM_CD.Text = strItemCd;

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

			if (string.IsNullOrEmpty(txtITEM_CD.Text) == false) { SearchExec(); }
        }
        #endregion
        
        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

                try
                {
                    string strCloseYn = "";
                    if (rdoY.Checked == true)
                    {
                        strCloseYn = "Y";
                    }
                    else if (rdoN.Checked == true)
                    {
                        strCloseYn = "N";
                    }

                    string strQuery = " usp_ITR003  @pTYPE = 'P4'";
                    strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' ";
                    strQuery += ", @pPLANT_CD = '" + cboPlantCd.SelectedValue + "' ";
                    strQuery += ", @pITEM_ACCT = '" + cboItemAcct.SelectedValue + "' ";
                    strQuery += ", @pITEM_CD = '" + txtITEM_CD.Text.Trim() + "' ";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    strQuery += ", @pCLOSE_YN = '" + strCloseYn + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0, true);

                    fpSpread1.Sheets[0].Columns[SystemBase.Base.GridHeadIndex(GHIdx1, "Lot 추적")].Locked = true;

                    if (fpSpread1.Sheets[0].RowCount > 0)
                    {
                        int Count = 0;

                        for (int i = 0; i < fpSpread1.Sheets[0].RowCount - 1; i++)
                        {

                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text == fpSpread1.Sheets[0].Cells[i + 1, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text &&
                                fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목명")].Text == fpSpread1.Sheets[0].Cells[i + 1, SystemBase.Base.GridHeadIndex(GHIdx1, "품목명")].Text &&
                                fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "규격")].Text == fpSpread1.Sheets[0].Cells[i + 1, SystemBase.Base.GridHeadIndex(GHIdx1, "규격")].Text &&
                                fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text == fpSpread1.Sheets[0].Cells[i + 1, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text &&
                                fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Text == fpSpread1.Sheets[0].Cells[i + 1, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Text &&
                                fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "창고")].Text == fpSpread1.Sheets[0].Cells[i + 1, SystemBase.Base.GridHeadIndex(GHIdx1, "창고")].Text &&
                                fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "창고명")].Text == fpSpread1.Sheets[0].Cells[i + 1, SystemBase.Base.GridHeadIndex(GHIdx1, "창고명")].Text &&
                                fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "위치")].Text == fpSpread1.Sheets[0].Cells[i + 1, SystemBase.Base.GridHeadIndex(GHIdx1, "위치")].Text &&
                                fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "위치명")].Text == fpSpread1.Sheets[0].Cells[i + 1, SystemBase.Base.GridHeadIndex(GHIdx1, "위치명")].Text &&
                                fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].Text == fpSpread1.Sheets[0].Cells[i + 1, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].Text &&
                                fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "재고수량")].Text == fpSpread1.Sheets[0].Cells[i + 1, SystemBase.Base.GridHeadIndex(GHIdx1, "재고수량")].Text
                               )
                            {
                                Count++;
                            }
                            else
                            {
                                if (Count > 0)
                                {
                                    fpSpread1.Sheets[0].Cells[i - Count, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].RowSpan = Count + 1;
                                    fpSpread1.Sheets[0].Cells[i - Count, SystemBase.Base.GridHeadIndex(GHIdx1, "품목명")].RowSpan = Count + 1;
                                    fpSpread1.Sheets[0].Cells[i - Count, SystemBase.Base.GridHeadIndex(GHIdx1, "규격")].RowSpan = Count + 1;
                                    fpSpread1.Sheets[0].Cells[i - Count, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].RowSpan = Count + 1;
                                    fpSpread1.Sheets[0].Cells[i - Count, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트명")].RowSpan = Count + 1;
                                    fpSpread1.Sheets[0].Cells[i - Count, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].RowSpan = Count + 1;
                                    fpSpread1.Sheets[0].Cells[i - Count, SystemBase.Base.GridHeadIndex(GHIdx1, "창고")].RowSpan = Count + 1;
                                    fpSpread1.Sheets[0].Cells[i - Count, SystemBase.Base.GridHeadIndex(GHIdx1, "창고명")].RowSpan = Count + 1;
                                    fpSpread1.Sheets[0].Cells[i - Count, SystemBase.Base.GridHeadIndex(GHIdx1, "위치")].RowSpan = Count + 1;
                                    fpSpread1.Sheets[0].Cells[i - Count, SystemBase.Base.GridHeadIndex(GHIdx1, "위치명")].RowSpan = Count + 1;
                                    fpSpread1.Sheets[0].Cells[i - Count, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].RowSpan = Count + 1;

                                    fpSpread1.Sheets[0].Cells[i - Count, SystemBase.Base.GridHeadIndex(GHIdx1, "재고수량")].RowSpan = Count + 1;

                                }
                                Count = 0;
                            }
                        }

                        for (int i = 0; i < fpSpread1.Sheets[0].RowCount; i++)
                        {
                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목명")].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "규격")].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트명")].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "창고")].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "창고명")].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "위치")].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "위치명")].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "재고수량")].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;

                        }
                    }

                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                this.Cursor = System.Windows.Forms.Cursors.Default;
            }
        }
        #endregion

        #region 버튼 Click
        private void btnOk_Click(object sender, System.EventArgs e)
        {

			if (fpSpread1.Sheets[0].Rows.Count > 0)
			{

				//if (Convert.ToDecimal(fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "이동수량")].Value) == 0)
				//{
				//    MessageBox.Show("현재 선택된 " + fpSpread1.Sheets[0].ActiveRowIndex.ToString() + "행의 이동수량은 0보다 커야 합니다.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
				//    return;
				//}

				RtnStr(fpSpread1.Sheets[0].ActiveRowIndex);
				this.DialogResult = DialogResult.OK;
				this.Close();
			}
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
			//if (Convert.ToDecimal(fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "이동수량")].Value) == 0)
			//{
			//    MessageBox.Show("이동수량은 0보다 커야 합니다.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			//    return;
			//}

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

		#region cud flag 조정
		private void fpSpread1_EditChange(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
		{
			try 
			{
				//fpSpread1.Sheets[0].RowHeader.Cells[e.Row, 0].Text = "";

				//if (
				//    Convert.ToDecimal(fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "재고수량")].Value) <
				//    Convert.ToDecimal(fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "이동수량")].Value)
				//   )
				//{
				//    MessageBox.Show("이동수량은 재고수량보다 클 수 없습니다.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
				//    fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "이동수량")].Value = 0;
				//}

				////하나의 행만 선택
				//for (int i = 0; i <= fpSpread1.Sheets[0].Rows.Count - 1; i++)
				//{
				//    if (i != fpSpread1.Sheets[0].ActiveRowIndex)
				//    {
				//        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "이동수량")].Value = 0;
				//    }
				//}
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				DialogResult dsMsg = MessageBox.Show("이동수량 오류", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		#endregion

		#region 선택행 강조
		private void fpSpread1_SelectionChanged(object sender, FarPoint.Win.Spread.SelectionChangedEventArgs e)
		{
			//fpSpread1.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.RowMode;
		}
		#endregion

	}
}
