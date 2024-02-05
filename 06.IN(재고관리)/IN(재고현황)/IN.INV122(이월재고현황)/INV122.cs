#region 작성정보
/*********************************************************************/
// 단위업무명 : 이월재고현황
// 작 성 자 : 권 순 철
// 작 성 일 : 2013-04-11
// 작성내용 : 이월재고현황 및 관리
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

namespace IN.INV122
{
    public partial class INV122 : UIForm.FPCOMM1
    {
        #region 변수선언
        bool form_act_chk = false;
        int SDown = 1;		// 조회 횟수
        int AddRow = 100;
        #endregion

        public INV122()
        {
            InitializeComponent();
        }

        #region Form Load시
        private void INV122_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            SystemBase.ComboMake.C1Combo(cboPlantCd, "usp_B_COMMON @pType='B031', @pSPEC1 = '" + SystemBase.Base.gstrBIZCD + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ", 0);//공장
            SystemBase.ComboMake.C1Combo(cboItemAcct, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'B036', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 0); //품목계정

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

            //기타 세팅
            dtpTranDt.Text = SystemBase.Base.ServerTime("YYMMDD").Substring(0, 7);
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            fpSpread1.Sheets[0].Rows.Count = 0;

            //기타 세팅
            dtpTranDt.Text = SystemBase.Base.ServerTime("YYMMDD").Substring(0, 7);
        }
        #endregion

        #region SearchExec 그리드 조회
        protected override void SearchExec()
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {
                    string strQuery = " usp_INV122 'S1'";
                    strQuery += ", @pLANG_CD ='" + SystemBase.Base.gstrLangCd + "'";
                    strQuery += ", @pPLANT_CD ='" + cboPlantCd.SelectedValue.ToString() + "'";
                    strQuery += ", @pITEM_ACCT ='" + cboItemAcct.SelectedValue.ToString() + "'";
                    strQuery += ", @pITEM_CD ='" + txtItemCd.Text.Trim() + "'";
                    strQuery += ", @pYEAR_MON  ='" + dtpTranDt.Text.Replace("-", "") + "'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    //strQuery += ", @pTOPCOUNT ='"+ AddRow +"'";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 2, true);
                    fpSpread1.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.Normal;
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
            this.Cursor = Cursors.Default;
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

                string strQuery = " usp_INV122 'S1'";
                strQuery += ", @pLANG_CD ='" + SystemBase.Base.gstrLangCd + "'";
                strQuery += ", @pPLANT_CD ='" + cboPlantCd.SelectedValue.ToString() + "'";
                strQuery += ", @pITEM_ACCT ='" + cboItemAcct.SelectedValue.ToString() + "'";
                strQuery += ", @pITEM_CD ='" + txtItemCd.Text.Trim() + "'";
                strQuery += ", @pYEAR_MON ='" + dtpTranDt.Text.Replace("-", "") + "'";
                //strQuery += ", @pTOPCOUNT ='"+ AddRow * SDown +"'";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery);

                this.Cursor = Cursors.Default;
            }
        }
        #endregion

        #region 버튼 Click
        private void btnItem_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW005 pu1 = new WNDW005(cboPlantCd.SelectedValue.ToString(), true, txtItemCd.Text);
                pu1.ShowDialog();
                if (pu1.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu1.ReturnVal;

                    txtItemCd.Text = Msgs[2].ToString();
                    txtItemNm.Value = Msgs[3].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }
        #endregion

        #region TextChanged
        private void txtItemCd_TextChanged(object sender, System.EventArgs e)
        {
            txtItemNm.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtItemCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
        }

        #endregion

        #region 수불상세
        private void btnDetail_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (fpSpread1.ActiveSheet.GetSelection(0) == null)
                {
                    DialogResult dsMsg = MessageBox.Show("행을 먼저 선택하세요!", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                int Row = fpSpread1.Sheets[0].ActiveRowIndex;
                string item = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목")].Text;
                string item_nm = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목명")].Text;
                string plant = cboPlantCd.SelectedValue.ToString();
                string plant_nm = cboPlantCd.Text;
                string unit = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].Text;
                string ym = dtpTranDt.Value.ToString();

                INV122P1 frm1 = new INV122P1(plant, plant_nm, item, item_nm, unit, ym);
                frm1.ShowDialog();
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }
        #endregion

        #region Form Activated & Deactivate
        private void INV122_Activated(object sender, System.EventArgs e)
        {
            if (form_act_chk == false) cboPlantCd.Focus();
        }

        private void INV122_Deactivate(object sender, System.EventArgs e)
        {
            form_act_chk = true;
        }
        #endregion


    }
}
