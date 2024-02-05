
#region 작성정보
/*********************************************************************/
// 단위업무명 : 미착재료비현황
// 작 성 자 : 권순철
// 작 성 일 : 2013-04-10
// 작성내용 : 미착재료비현황 및 관리
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

namespace MI.MIM512
{
    public partial class MIM512 : UIForm.FPCOMM1
    {
        #region 변수선언
        bool form_act_chk = false;
        string strCfmFlag;
        #endregion

        public MIM512()
        {
            InitializeComponent();
        }

        #region Form Load 시
        private void MIM512_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            //기타 세팅
            dtpRecDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString();
            dtpRecDtTo.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).ToShortDateString();

            dtpLoadDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString();
            dtpLoadDtTo.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).ToShortDateString();

            dtpMvmtDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString();
            dtpMvmtDtTo.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).ToShortDateString();

            rdoCfmAll.Checked = true;
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            fpSpread1.Sheets[0].Rows.Count = 0;

            //기타 세팅
            dtpRecDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString();
            dtpRecDtTo.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).ToShortDateString();

            dtpLoadDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString();
            dtpLoadDtTo.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).ToShortDateString();

            dtpMvmtDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString();
            dtpMvmtDtTo.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).ToShortDateString();

            rdoCfmAll.Checked = true;
        }
        #endregion

        #region SearchExec()  그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {
                    string strCfmYn = "";
                    if (rdoCfmYes.Checked == true) { strCfmYn = "Y"; }
                    else if (rdoCfmNo.Checked == true) { strCfmYn = "N"; }

                    string strQuery = " usp_MIM512 'S1'";
                    strQuery += ", @pLANG_CD ='" + SystemBase.Base.gstrLangCd + "'";
                    strQuery += ", @pBIZ_CD  ='" + txtTaxBizCd.Text.Trim() + "'";
                    strQuery += ", @pBL_NO_FR  ='" + txtBlNoFr.Text + "'";
                    strQuery += ", @pBL_NO_TO  ='" + txtBlNoTo.Text + "'";
                    strQuery += ", @pCUST_CD ='" + txtBeneficiaryCust.Text.Trim() + "'";
                    strQuery += ", @pBL_RECEIPT_DT_FR ='" + dtpRecDtFr.Text + "'";
                    strQuery += ", @pBL_RECEIPT_DT_TO ='" + dtpRecDtTo.Text + "'";
                    strQuery += ", @pLOADING_DT_FR  ='" + dtpLoadDtFr.Text + "'";
                    strQuery += ", @pLOADING_DT_TO  ='" + dtpLoadDtTo.Text + "'";
                    strQuery += ", @pTRAN_DT_FR  ='" + dtpMvmtDtFr.Text + "'";
                    strQuery += ", @pTRAN_DT_TO  ='" + dtpMvmtDtTo.Text + "'";
                    strQuery += ", @pCONFIRM_YN ='" + strCfmYn + "'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0, true);
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

        #region 버튼 Click
        //수출자
        private void btnBeneficiaryCust_Click(object sender, System.EventArgs e)
        {

            try
            {
                WNDW002 pu = new WNDW002(txtBeneficiaryCust.Text, "P");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtBeneficiaryCust.Text = Msgs[1].ToString();
                    txtBeneficiaryCustNm.Value = Msgs[2].ToString();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }

        }

        private void btnBlNoFr_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (rdoCfmYes.Checked == true)
                {
                    strCfmFlag = "Y";
                }
                else if (rdoCfmNo.Checked == true)
                {
                    strCfmFlag = "N";
                }
                else
                {
                    strCfmFlag = "";
                }

                WNDW022 pu = new WNDW022(strCfmFlag);
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtBlNoFr.Text = Msgs[1].ToString();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        private void btnBlNoTo_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (rdoCfmYes.Checked == true)
                {
                    strCfmFlag = "Y";
                }
                else if (rdoCfmNo.Checked == true)
                {
                    strCfmFlag = "N";
                }
                else
                {
                    strCfmFlag = "";
                }

                WNDW022 pu = new WNDW022(strCfmFlag);
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtBlNoTo.Text = Msgs[1].ToString();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        //사업장
        private void btnTaxBiz_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON @pTYPE ='TABLE_POP', @pSPEC1 = 'BIZ_CD', @pSPEC2 = 'BIZ_NM', @pSPEC3 = 'B_BIZ_PLACE', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtTaxBizCd.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00010", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "사업장 조회");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtTaxBizCd.Text = Msgs[0].ToString();
                    txtTaxBizNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "사업장 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region TextChanged
        //수출자
        private void txtBeneficiaryCust_TextChanged(object sender, System.EventArgs e)
        {
            txtBeneficiaryCustNm.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtBeneficiaryCust.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
        }

        //사업장
        private void txtTaxBizCd_TextChanged(object sender, System.EventArgs e)
        {
            txtTaxBizNm.Value = SystemBase.Base.CodeName("BIZ_CD", "BIZ_NM", "B_BIZ_PLACE", txtTaxBizCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
        }
        #endregion

        #region Activated,Deactivate
        private void MIM512_Activated(object sender, System.EventArgs e)
        {
            if (form_act_chk == false) txtTaxBizCd.Focus();
        }

        private void MIM512_Deactivate(object sender, System.EventArgs e)
        {
            form_act_chk = true;
        }
        #endregion
    }
}
