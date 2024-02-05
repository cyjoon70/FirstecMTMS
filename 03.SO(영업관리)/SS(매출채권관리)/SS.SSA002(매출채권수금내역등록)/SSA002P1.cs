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

namespace SS.SSA002
{
    public partial class SSA002P1 : UIForm.FPCOMM1
    {
        #region 변수선언
        FarPoint.Win.Spread.FpSpread fpGrid;
        int ActiveRow = 0;
        string returnVal = "";
        string strBankCd = "";
        #endregion

        #region 생성자
        public SSA002P1(FarPoint.Win.Spread.FpSpread fpRtrGrid, int Row, string BankCd)
        {
            fpGrid = fpRtrGrid;
            ActiveRow = Row;
            strBankCd = BankCd;

            InitializeComponent();

            //
            // TODO: InitializeComponent를 호출한 다음 생성자 코드를 추가합니다.
            //
        }

        public SSA002P1()
        {
            //
            // Windows Form 디자이너 지원에 필요합니다.
            //
            InitializeComponent();

            //
            // TODO: InitializeComponent를 호출한 다음 생성자 코드를 추가합니다.
            //
        }
        #endregion   

        #region 폼로드 이벤트
        private void SSA002P1_Load(object sender, System.EventArgs e)
        {
            UIForm.Buttons.ReButton("010000001001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);
            SystemBase.Validation.GroupBox_Setting(groupBox1);//필수체크

            this.Text = "계좌번호 조회";

            txtBankCd.Value = strBankCd;
            Search(false);
        }
        #endregion
        
        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            Search(true);
        }
        #endregion

        #region 조회함수
        private void Search(bool Msg)
        {
            this.Cursor = Cursors.WaitCursor;

            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                try
                {
                    string strQuery = " usp_SSA002  @pTYPE = 'C1' ";
                    strQuery += ", @pBANK_CD = '" + txtBankCd.Text + "' ";
                    strQuery += ", @pDPST_TYPE = '" + txtDpstTypeCd.Text + "' ";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, Msg, 0, 0, true);

                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
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

        #region 조회 팝업창
        //은행
        private void btnBank_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON @pTYPE ='TABLE_POP', @pSPEC1 = 'BANK_CD', @pSPEC2 = 'BANK_NM', @pSPEC3 = 'B_BANK', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtBankCd.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00012", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "은행 조회");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtBankCd.Value = Msgs[0].ToString();
                    txtBankNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "은행 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //예적금유형
        private void btnDpstType_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON @pTYPE ='COMM_POP', @pSPEC1 = 'B018', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "', @pLANG_CD = '"+ SystemBase.Base.gstrLangCd.ToString() +"' ";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtDpstTypeCd.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00013", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "예적금 유형 조회");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtDpstTypeCd.Value = Msgs[0].ToString();
                    txtDpstTypeNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "예적금 유형 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 조회조건 코드 입력시 코드명 자동변환
        private void txtBankCd_TextChanged(object sender, System.EventArgs e)
        {
            txtBankNm.Value = SystemBase.Base.CodeName("BANK_CD", "BANK_NM", "B_BANK", txtBankCd.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
        }

        private void txtDpstTypeCd_TextChanged(object sender, System.EventArgs e)
        {
            txtDpstTypeNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtDpstTypeCd.Text, " AND MAJOR_CD = 'BO18' AND COMP_CODE ='" + SystemBase.Base.gstrCOMCD + "'");
        }
        #endregion
    }
}
