#region 작성정보
/*********************************************************************/
// 단위업무명 : 지급내역등록
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-04-08
// 작성내용 : 지급내역등록 및 관리
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

namespace MV.MIV002
{
    public partial class MIV002P2 : UIForm.FPCOMM1
    {
        #region 변수선언
        FarPoint.Win.Spread.FpSpread fpGrid;
        int ActiveRow = 0;
        string returnVal = "";
        string strCustCd = "";
        #endregion

        #region 생성자
        public MIV002P2(FarPoint.Win.Spread.FpSpread fpRtrGrid, int Row, string CustCd)
        {
            fpGrid = fpRtrGrid;
            ActiveRow = Row;
            strCustCd = CustCd;

            InitializeComponent();
        }

        public MIV002P2()
        {
            InitializeComponent();
        }
        #endregion

        #region 폼로드 이벤트
        private void MIV002P2_Load(object sender, System.EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            this.Text = "어음번호 조회";
          
            //버튼 재정의(조회권한만)
            UIForm.Buttons.ReButton("010000001001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

            txtCustCd.Text = strCustCd;

            SearchExec();
        }
        #endregion
        
        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = Cursors.WaitCursor;

            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                try
                {
                    string strNoteFg = "D1"; //받을어음
                    if (rdoNoteFg2.Checked == true)
                    { strNoteFg = "D3"; }		//지급어음

                    string strQuery = " usp_MIV002  @pTYPE = 'C2' ";
                    strQuery += ", @pDATE_FR = '" + dtpIssueDtFr.Text + "' ";
                    strQuery += ", @pDATE_TO = '" + dtpIssueDtTo.Text + "' ";
                    strQuery += ", @pCUST_CD = '" + txtCustCd.Text + "' ";
                    strQuery += ", @pNOTE_NO_FR = '" + txtNoteNoFr.Text + "' ";
                    strQuery += ", @pNOTE_NO_TO = '" + txtNoteNoTo.Text + "' ";
                    strQuery += ", @pNOTE_FG = '" + strNoteFg + "' ";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0);

                    fpSpread1.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.SingleSelect;

                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
                }
            }

            this.Cursor = Cursors.Default;
        }
        #endregion

        #region 종료
        private void btnExit_Click(object sender, System.EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
        #endregion

        #region 확인(선택된 값 넘기기)
        public string ReturnVal { get { return returnVal; } set { returnVal = value; } }

        private void btnOk_Click(object sender, System.EventArgs e)
        {
            try
            {
                RtnStr(fpSpread1.Sheets[0].GetSelection(0).Row);

                strFormClosingMsg = false;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch { }
        }
        private void fpSpread1_CellDoubleClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            try
            {
                RtnStr(e.Row);
                strFormClosingMsg = false;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch { }
        }
        private void RtnStr(int R)
        {
            try
            {
                if (fpSpread1.Sheets[0].Rows.Count > 0)
                {
                    returnVal = "";
                    for (int i = 0; i < fpSpread1.Sheets[0].Columns.Count; i++)
                    {
                        if (returnVal.Length > 0)
                            returnVal = returnVal + "#" + fpSpread1.Sheets[0].Cells[R, i].Value.ToString();
                        else
                            returnVal = fpSpread1.Sheets[0].Cells[R, i].Value.ToString();
                    }
                }
            }
            catch { }
        }
        #endregion

        #region 거래처 팝업창
        private void btnCust_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW002 pu = new WNDW002(txtCustCd.Text, "P");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtCustCd.Value = Msgs[1].ToString();
                    txtCustNm.Value = Msgs[2].ToString();
                    txtCustCd.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 코드 입력시 코드명 자동입력
        //거래처
        private void txtCustCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtCustCd.Text != "")
                {
                    txtCustNm.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtCustCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtCustNm.Value = "";
                }
            }
            catch
            {

            }
        }
        #endregion

    }
}
