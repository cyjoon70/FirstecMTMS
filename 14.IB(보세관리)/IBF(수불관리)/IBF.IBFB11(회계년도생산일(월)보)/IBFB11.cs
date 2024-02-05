#region 작성정보
/*********************************************************************/
// 단위업무명 : 가공품실무게관리
// 작 성 자 : 김현근
// 작 성 일 : 2013-06-05
// 작성내용 : 가공품실무게관리 및 조회
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


namespace IBF.IBFB11
{
    public partial class IBFB11 : UIForm.FPCOMM1
    {
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private bool chk = false;
        public IBFB11()
        {
            InitializeComponent();
        }

        #region Form Load 시
        private void IBFB11_Load(object sender, System.EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(groupBox1);	//컨트롤 필수 Setting
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
           
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Base.GroupBoxReset(groupBox1);
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
             this.Cursor = Cursors.WaitCursor;

             try
             {
                 if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                 {
                     string strQuery = " usp_IBFB11  'S1',";
                     strQuery = strQuery + " @pTRACKING_NO = '" + txtTRNo.Text + "', ";
                     strQuery = strQuery + " @pSO_NO = '" + txtSO_NO.Text + "', ";
                     strQuery = strQuery + " @pDT_FR = '" + dtpDT_FR.Text + "', ";
                     strQuery = strQuery + " @pDT_TO = '" + dtpDT_TO.Text + "', ";
                     strQuery = strQuery + " @pITEM_CD = '" + txtItemCd.Text + "', "; 
                     strQuery = strQuery + " @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                     UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0, true);
                 }
             }
             catch (Exception f)
             {
                 SystemBase.Loggers.Log(this.Name, f.ToString());
                 MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
             }

            this.Cursor = Cursors.Default;
            fpSpread1.Focus();
        }
        #endregion

        #region PrintExec() 그리드 출력 로직
        protected override void PrintExec()
        {

            string[] RptParmValue = new string[7];      // 2015.02.25. hma 수정: 6=>7
            string RptName = "";

            if (fpSpread1.Sheets[0].Rows.Count <= 0) return;
            //--레포트 파일 선택

            RptName = @"Report\" + "IBFB23P.rpt";
            RptParmValue[0] = "S1";

            if (txtTRNo.Text.Trim() == "") RptParmValue[1] = " ";
            else RptParmValue[1] = txtTRNo.Text;

            if (txtSO_NO.Text.Trim() == "") RptParmValue[2] = " ";
            else RptParmValue[2] = txtSO_NO.Text;

            RptParmValue[3] = dtpDT_FR.Text;
            RptParmValue[4] = dtpDT_TO.Text;

            if (txtItemCd.Text.Trim() == "") RptParmValue[5] = " ";
            else RptParmValue[5] = txtItemCd.Text;

            RptParmValue[6] = SystemBase.Base.gstrCOMCD;        // 2015.02.25. hma 추가: 프로시저에는 매개변수가 있는데 프로그램에는 없음.

            UIForm.PRINT10 frm = new UIForm.PRINT10(this.Text + " 출력", null, null, RptName, RptParmValue);	//공통크리스탈 10버전

            frm.ShowDialog();
        }
        #endregion


        #region 팝업창 열기
        private void btnTRNo_Click(object sender, EventArgs e)
        {
            try
            {
                //Tracking No. 팝업
                this.Cursor = Cursors.WaitCursor;
                string strQuery = " Nusp_BF_Comm 'BF10' ";
                string[] strWhere = new string[] { "@pValue" };
                string[] strSearch = new string[] { txtTRNo.Text };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("BFP001", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "Tracking No.팝업");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtTRNo.Text = Msgs[0].ToString();
                    txtSO_NO.Text = Msgs[1].ToString();
                }

                this.Cursor = Cursors.Default; 
            }
            catch (Exception f)
            {
                this.Cursor = Cursors.Default;
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(f.ToString());
            }
        }

        //제품코드
        private void btnItemCd_Click(object sender, EventArgs e)
        {
            try
            {
                WNDW.WNDW005 pu = new WNDW.WNDW005("10");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtItemCd.Text = Msgs[2].ToString();
                    txtItemNm.Value = Msgs[3].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목코드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 제품코드 체인지
        private void txtItemCd_TextChanged(object sender, EventArgs e)
        {
            txtItemNm.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "MTMS_FT.dbo.B_ITEM_INFO", txtItemCd.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "'");
        }
        #endregion

        private void IBFB11_Activated(object sender, System.EventArgs e)
        {
            if (chk == false)
            {
                txtTRNo.Focus();
            }
        }

        private void IBFB11_Deactivate(object sender, System.EventArgs e)
        {
            chk = true;
        }

        private void txtTRNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && txtTRNo.Text.Trim() != "") SearchExec(); 
        }

        private void txtSO_NO_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) SearchExec(); 
        }

    }
}
