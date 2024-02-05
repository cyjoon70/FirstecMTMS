#region 작성정보
/*********************************************************************/
// 단위업무명 : 재고일보출력
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-04-19
// 작성내용 : 재고일보출력 관리
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
using FarPoint.Win.Spread.CellType;

namespace IT.ITR105
{
    public partial class ITR105 : UIForm.FPCOMM1
    {
        #region 변수선언
        int SDown = 1;		// 조회 횟수
        int AddRow = 100;
        bool form_act_chk = false;
        #endregion

        #region 생성자
        public ITR105()
        {
            InitializeComponent();
        }
        #endregion 

        #region Form Load 시
        private void ITR105_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            SystemBase.ComboMake.C1Combo(cboPlantCd, "usp_B_COMMON @pType='B031', @pSPEC1 = '" + SystemBase.Base.gstrBIZCD + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//공장			
            dtpTranDt.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).ToShortDateString().Substring(0,10);
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            fpSpread1.Sheets[0].Rows.Count = 0;
            dtpTranDt.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).ToShortDateString().Substring(0,10);
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
                    string strQuery = " usp_ITR105 'S1'";
                    strQuery += ", @pPLANT_CD ='" + cboPlantCd.SelectedValue.ToString() + "'";
                    strQuery += ", @pTRAN_DT  ='" + dtpTranDt.Text + "'";
                    strQuery += ", @pSL_CD ='" + txtSL_CD.Text.Trim() + "'";
                    strQuery += ", @pITEM_CD_FR ='" + txtItemCdFr.Text.Trim() + "'";
                    strQuery += ", @pITEM_CD_TO ='" + txtItemCdTo.Text.Trim() + "'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

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

                string strQuery = " usp_ITR105 'S1'";
                strQuery += ", @pPLANT_CD ='" + cboPlantCd.SelectedValue.ToString() + "'";
                strQuery += ", @pTRAN_DT  ='" + dtpTranDt.Text + "'";
                strQuery += ", @pSL_CD ='" + txtSL_CD.Text.Trim() + "'";
                strQuery += ", @pITEM_CD_FR ='" + txtItemCdFr.Text.Trim() + "'";
                strQuery += ", @pITEM_CD_TO ='" + txtItemCdTo.Text.Trim() + "'";
                strQuery += ", @pTOPCOUNT ='" + AddRow * SDown + "'";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery);
                this.Cursor = Cursors.Default;
            }
        }
        #endregion

        #region 버튼 Click
        private void btnItemFr_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW005 pu1 = new WNDW005(cboPlantCd.SelectedValue.ToString(), true, txtItemCdFr.Text);
                pu1.ShowDialog();
                if (pu1.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu1.ReturnVal;

                    txtItemCdFr.Value = Msgs[2].ToString();
                    txtItemNmFr.Value = Msgs[3].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        private void btnItemTo_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW005 pu1 = new WNDW005(cboPlantCd.SelectedValue.ToString(), true, txtItemCdTo.Text);
                pu1.ShowDialog();
                if (pu1.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu1.ReturnVal;

                    txtItemCdTo.Value = Msgs[2].ToString();
                    txtItemNmTo.Value = Msgs[3].ToString();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        private void btnSL_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON @pType='B035', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pSPEC1 = '" + cboPlantCd.SelectedValue.ToString() + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtSL_CD.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00056", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "창고 조회", false);

                pu.ShowDialog();	//공통 팝업 호출
                if (pu.DialogResult == DialogResult.OK)
                {
                    string MSG = pu.ReturnVal.Replace("|", "#");
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(MSG);

                    txtSL_CD.Value = Msgs[0].ToString();
                    txtSL_NM.Value = Msgs[1].ToString();
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
        private void txtItemCdFr_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtItemCdFr.Text != "")
                {
                    txtItemNmFr.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtItemCdFr.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtItemNmFr.Value = "";
                }
            }
            catch
            {

            }
        }

        private void txtItemCdTo_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtItemCdTo.Text != "")
                {
                    txtItemNmTo.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtItemCdTo.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtItemNmTo.Value = "";
                }
            }
            catch
            {

            }
        }

        // 창고
        private void txtSL_CD_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtSL_CD.Text != "")
                {
                    txtSL_NM.Value = SystemBase.Base.CodeName("SL_CD", "SL_NM", "B_STORAGE_INFO", txtSL_CD.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtSL_NM.Value = "";
                }
            }
            catch
            {

            }
        }
        #endregion

        #region 폼 Activated & Deactivate
        private void ITR105_Activated(object sender, System.EventArgs e)
        {
            if (form_act_chk == false) cboPlantCd.Focus();
        }

        private void ITR105_Deactivate(object sender, System.EventArgs e)
        {
            form_act_chk = true;
        }
        #endregion

    }
}
