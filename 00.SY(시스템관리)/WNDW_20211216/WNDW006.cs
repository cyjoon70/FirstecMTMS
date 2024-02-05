#region 작성정보
/*********************************************************************/
// 단위업무명 : 공통팝업 제조오더 정보조회
// 작 성 자 : 조 홍 태
// 작 성 일 : 2013-02-05
// 작성내용 : 제조오더 정보조회
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

#region 예제 - 복사해서 쓰세요
/*
try
{
    WNDW.WNDW006 pu = new WNDW.WNDW006();
    pu.ShowDialog();
    if (pu.DialogResult == DialogResult.OK)
    {
        string[] Msgs = pu.ReturnVal;

        textBox1.Text = Msgs[1].ToString();
        textBox2.Value = Msgs[2].ToString();
    }
}
catch (Exception f)
{
    SystemBase.Loggers.Log(this.Name, f.ToString());
    MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "제조오더정보조회 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
}
 */
#endregion

namespace WNDW
{
    /// <summary>
    /// 제조오더정보조회
    /// <para>예제는 소스안에서 복사해쓰세요</para>
    /// <para>Msgs[1] = 제조오더번호 </para>
    /// <para>Msgs[2] = 제품오더번호 </para>
    /// <para>Msgs[3] = 프로젝트번호 </para>
    /// <para>Msgs[4] = 프로젝트명 </para>
    /// <para>Msgs[5] = 프로젝트차수 </para>
    /// <para>Msgs[6] = 품목코드 </para>
    /// <para>Msgs[7] = 품목명 </para>
    /// </summary>

    public partial class WNDW006 : UIForm.FPCOMM1
    {
        #region 변수선언
        string[] returnVal = null;
        int SDown = 1;		// 조회 횟수
        int AddRow = 100;	// 조회 건수
        string strWoNo = "";
        string strType = "";
        #endregion

        #region WNDW006 생성자
        public WNDW006(string WoNo)
        {
            strWoNo = WoNo;

            InitializeComponent();
        }

        public WNDW006(string WoNo, string Type)
        {
            strWoNo = WoNo;
            strType = Type;

            InitializeComponent();
        }

        public WNDW006()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void WNDW006_Load(object sender, System.EventArgs e)
        {
            //버튼 재정의
            UIForm.Buttons.ReButton("010000000001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);

            SystemBase.Validation.GroupBox_Setting(groupBox1);//필수적용

            //조회 콤보박스세팅
            SystemBase.ComboMake.C1Combo(cboOrderStatus, "usp_B_COMMON @pTYPE = 'COMM', @pCODE = 'P020', @pCO_CD = '"+ SystemBase.Base.gstrCOMCD.ToString() +"' ", 3, true);  //지시상태
            SystemBase.ComboMake.C1Combo(cboOrderType, "usp_B_COMMON @pTYPE = 'COMM', @pCODE = 'P026', @pCO_CD = '"+ SystemBase.Base.gstrCOMCD.ToString() +"' ", 3);  //지시구분
            SystemBase.ComboMake.C1Combo(cboCon_Type, "usp_B_COMMON @pTYPE = 'COMM', @pCODE = 'S014', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' ", 3);  //계약구분

            //그리드 콤보박스세팅
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "지시상태")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE = 'COMM', @pCODE = 'P020', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' ");
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "재작업")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE = 'COMM', @pCODE = 'P027', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' ");
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "지시구분")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE = 'COMM', @pCODE = 'P026' , @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'");

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);

            txtWorkOrderNo.Text = strWoNo;

            dtpStartDt.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD").ToString()).AddMonths(-1).ToShortDateString();
            dtpEndDt.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD").ToString()).ToShortDateString();

            if (strType != "")
            {
                cboCon_Type.SelectedValue = strType;
                cboCon_Type.Enabled = false;
            }

            if (txtWorkOrderNo.Text != "")
                Grid_search(false);

            this.Text = "제조오더 조회";
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        { Grid_search(true); }
        #endregion

        #region 그리드조회
        private void Grid_search(bool Msg)
        {
            this.Cursor = Cursors.WaitCursor;

            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                try
                {
                    SDown = 1;

                    string strQuery = " usp_WNDW006 @pTYPE = 'S1'";
                    strQuery += ", @pPROJECT_NO = '" + txtProjectNo.Text + "' ";
                    strQuery += ", @pITEM_CD = '" + txtItemCd.Text + "' ";
                    strQuery += ", @pWORKORDER_NO = '" + txtWorkOrderNo.Text + "' ";
                    strQuery += ", @pSTART_DT = '" + dtpStartDt.Text + "' ";
                    strQuery += ", @pEND_DT = '" + dtpEndDt.Text + "' ";
                    strQuery += ", @pCONTRACT_TYPE = '" + cboCon_Type.SelectedValue.ToString() + "' ";

                    strQuery += ", @pORDER_STATUS = '" + cboOrderStatus.SelectedValue.ToString() + "' ";
                    strQuery += ", @pORDER_FLAG = '" + cboOrderType.SelectedValue.ToString() + "' ";

                    strQuery += ", @pTOPCOUNT ='" + AddRow + "'";
                    strQuery += ", @pCO_CD ='" + SystemBase.Base.gstrCOMCD.ToString() + "'";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, Msg, 0, 0, true);
                    fpSpread1.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.SingleSelect;
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

        #region 조회조건 팝업창
        //품목코드
        private void btnItem_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW.WNDW005 pu = new WNDW.WNDW005(SystemBase.Base.gstrPLANT_CD, true, txtItemCd.Text);
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtItemCd.Text = Msgs[2].ToString();
                    txtItemNm.Value = Msgs[3].ToString();
                    txtItemCd.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목코드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //프로젝트
        private void btnProject_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW.WNDW003 pu = new WNDW.WNDW003(txtProjectNo.Text, "S1");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtProjectNo.Text = Msgs[3].ToString();
                    txtProjectNm.Value = Msgs[4].ToString();

                    txtProjectNo.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "프로젝트 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region TextBox코드입력시 코드명 자동입력
        //품목코드
        private void txtItemCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtItemCd.Text != "")
                {
                    txtItemNm.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtItemCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'");
                }
                else
                {
                    txtItemNm.Value = "";
                }
            }
            catch { }
        }

        //프로젝트
        private void txtProjectNo_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtProjectNo.Text != "")
                {
                    txtProjectNm.Value = SystemBase.Base.CodeName("PROJECT_NO", "PROJECT_NM", "S_SO_MASTER", txtProjectNo.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'");
                }
                else
                {
                    txtProjectNm.Value = "";
                }
            }
            catch { }
        }
        #endregion

        #region 100건씩 조회
        private void fpSpread1_TopChange(object sender, FarPoint.Win.Spread.TopChangeEventArgs e)
        {
            int FPHeight = (fpSpread1.Size.Height - 28) / 20;
            if (e.NewTop >= ((AddRow * SDown) - FPHeight))
            {
                SDown++;

                this.Cursor = Cursors.WaitCursor;

                string strQuery = " usp_WNDW006 'S1'";
                strQuery += ", @pPROJECT_NO = '" + txtProjectNo.Text + "' ";
                strQuery += ", @pITEM_CD = '" + txtItemCd.Text + "' ";
                strQuery += ", @pWORKORDER_NO = '" + txtWorkOrderNo.Text + "' ";
                strQuery += ", @pSTART_DT = '" + dtpStartDt.Text + "' ";
                strQuery += ", @pEND_DT = '" + dtpEndDt.Text + "' ";
                strQuery += ", @pCONTRACT_TYPE = '" + cboCon_Type.SelectedValue.ToString() + "' ";
                strQuery += ", @pORDER_STATUS = '" + cboOrderStatus.SelectedValue.ToString() + "' ";
                strQuery += ", @pORDER_FLAG = '" + cboOrderType.SelectedValue.ToString() + "' ";
                strQuery += ", @pTOPCOUNT ='" + AddRow * SDown + "'";
                strQuery += ", @pCO_CD ='" + SystemBase.Base.gstrCOMCD.ToString() + "'";

                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery);
                fpSpread1.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.SingleSelect;

                this.Cursor = Cursors.Default;
            }
        }
        #endregion

        #region 그리드 더블클릭
        private void fpSpread1_CellDoubleClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            RtnStr(e.Row);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        #endregion

        #region 그리드 선택값 입력밑 전송
        public string[] ReturnVal { get { return returnVal; } set { returnVal = value; } }

        public void RtnStr(int R)
        {
            if (fpSpread1.Sheets[0].Rows.Count > 0)
            {
                returnVal = new string[fpSpread1.Sheets[0].Columns.Count];
                for (int i = 0; i < fpSpread1.Sheets[0].Columns.Count; i++)
                {
                    returnVal[i] = Convert.ToString(fpSpread1.Sheets[0].Cells[R, i].Value);
                }
            }
        }
        #endregion

        #region Text에서 Enter시 조회
        private void txtProjectNo_KeyDown_1(object sender, System.Windows.Forms.KeyEventArgs e)
        { if (e.KeyCode == Keys.Enter) Grid_search(true); }
        private void txtProjectNm_KeyDown_1(object sender, System.Windows.Forms.KeyEventArgs e)
        { if (e.KeyCode == Keys.Enter) Grid_search(true); }
        private void txtWorkOrderNo_KeyDown_1(object sender, System.Windows.Forms.KeyEventArgs e)
        { if (e.KeyCode == Keys.Enter) Grid_search(true); }
        private void txtItemCd_KeyDown_1(object sender, System.Windows.Forms.KeyEventArgs e)
        { if (e.KeyCode == Keys.Enter) Grid_search(true); }
        private void txtItemNm_KeyDown_1(object sender, System.Windows.Forms.KeyEventArgs e)
        { if (e.KeyCode == Keys.Enter) Grid_search(true); }
        #endregion
    }
}
