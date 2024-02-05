
#region 작성정보
/*********************************************************************/
// 단위업무명 : 미착재료비상세현황
// 작 성 자 : 권순철
// 작 성 일 : 2013-04-10
// 작성내용 : 미착재료비상세현황 및 관리
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

namespace MI.MIM513
{
    public partial class MIM513 : UIForm.FPCOMM1
    {
        #region 변수선언
        bool form_act_chk = false;
        string strCfmFlag;
        #endregion

        public MIM513()
        {
            InitializeComponent();
        }

        #region Form Load 시
        private void MIM513_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            //기타 세팅
            dtpRecDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString();
            dtpRecDtTo.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).ToShortDateString();

            dtpLoadDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString();
            dtpLoadDtTo.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).ToShortDateString();


            rdoAll.Checked = true;
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

            rdoAll.Checked = true;
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
                    string strProcFlag = "";
                    if (rdoY.Checked == true) { strProcFlag = "Y"; }
                    else if (rdoN.Checked == true) { strProcFlag = "N"; }

                    string strQuery = " usp_MIM513 'S1'";
                    strQuery += ", @pLANG_CD ='" + SystemBase.Base.gstrLangCd + "'";
                    strQuery += ", @pBL_NO  ='" + txtBlNoFr.Text + "'";
                    strQuery += ", @pBL_RECEIPT_DT_FR ='" + dtpRecDtFr.Text + "'";
                    strQuery += ", @pBL_RECEIPT_DT_TO ='" + dtpRecDtTo.Text + "'";
                    strQuery += ", @pLOADING_DT_FR  ='" + dtpLoadDtFr.Text + "'";
                    strQuery += ", @pLOADING_DT_TO  ='" + dtpLoadDtTo.Text + "'";
                   //strQuery += ", @pTRAN_DT_FR  ="'";
                   //strQuery += ", @pTRAN_DT_TO  =;
                    strQuery += ", @pPROC_FLAG = '" + strProcFlag + "'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt,true, true, 0, 0, true);
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
        private void btnBlNo_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (rdoY.Checked == true)
                {
                    strCfmFlag = "Y";
                }
                else if (rdoN.Checked == true)
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
        #endregion

        private void MIM513_Activated(object sender, System.EventArgs e)
        {
            if (form_act_chk == false) txtBlNoFr.Focus();
        }

        private void MIM513_Deactivate(object sender, System.EventArgs e)
        {
            form_act_chk = true;
        }

    }
}
