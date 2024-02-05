

#region 작성정보
/*********************************************************************/
// 단위업무명 : 기준일별어음조회
// 작 성 자 : 유 재 규
// 작 성 일 : 2013-03-18
// 작성내용 : 기준일별어음조회
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

namespace AG.ACG303
{
    public partial class ACG303 : UIForm.FPCOMM1 
    {
        public ACG303()
        {
            InitializeComponent();
        }

        #region Form Load 시
        private void ACG303_Load(object sender, System.EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(groupBox1);//필수 적용
            SystemBase.ComboMake.C1Combo(cboNoteKind, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'A502', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9);   //어음구분
            
            NewExec();
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            dtpExpDtFr.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-3).ToShortDateString();
            dtpExpDtTo.Value = SystemBase.Base.ServerTime("YYMMDD");
            cboNoteKind.SelectedValue = "D1";

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
                    string strQuery = " usp_ACG303  'S1'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' ";
                    strQuery += ", @pNOTE_KIND = '" + cboNoteKind.SelectedValue.ToString() + "' ";
                    if (optExp.Checked == true)
                    {
                        strQuery += ", @pEXP_DT_FROM = '" + dtpExpDtFr.Text + "' ";
                        strQuery += ", @pEXP_DT_TO = '" + dtpExpDtTo.Text + "' ";
                    }
                    else if (optOpen.Checked == true)
                    {
                        strQuery += ", @pOPEN_DT_FROM = '" + dtpExpDtFr.Text + "' ";
                        strQuery += ", @pOPEN_DT_TO = '" + dtpExpDtTo.Text + "' ";
                    }
                    strQuery += ", @pCUST_CD = '" + txtCustCd.Text + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0);
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(SystemBase.Base.MessageRtn(f.ToString()), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            this.Cursor = Cursors.Default;
        }
        #endregion

        #region TextChanged 이벤트
        //거래처
        private void txtCustCd_TextChanged(object sender, EventArgs e)
        {
            try
            {
                txtCustNm.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtCustCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
            }
            catch (Exception f)
            {
                MessageBox.Show(SystemBase.Base.MessageRtn(f.ToString()), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //조회구분 변경시 라벨명 변경
        private void optExp_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (optExp.Checked == true)
                {
                    c1Label3.Text = "만기일자";
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //조회구분 변경시 라벨명 변경
        private void optOpen_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (optOpen.Checked == true)
                {
                    c1Label3.Text = "발행일자";
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 버튼 클릭
        //거래처
        private void btnCust_Click(object sender, EventArgs e)
        {
            try
            {
                WNDW.WNDW002 pu = new WNDW.WNDW002(txtCustCd.Text, "PS");
                pu.MaximizeBox = false;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtCustCd.Text = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "거래처 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }
        #endregion


        
    }
}
