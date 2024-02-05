#region 작성정보
/*********************************************************************/
// 단위업무명:  반별작업자별 TOUCH실적 상세조회
// 작 성 자  :  한 미 애
// 작 성 일  :  2020-09-01
// 작성내용  :  반별잔업시간집계표에서 작업자 더블클릭시 해당 작업자에 대한 실적을 상세 조회한다.
// 수 정 일  :
// 수 정 자  :
// 수정내용  :
// 비    고  :
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

namespace HA.HAA014
{
    public partial class HAA014P1 : UIForm.FPCOMM2
    {
        #region 변수선언
        string strStartYm = "", strEndYm = "", strWcCd = "", strWcNm = "", strEmpNo = "", strEmpNm = "";

        #endregion

        #region 생성자
        public HAA014P1()
        {
            InitializeComponent();
        }

        //HAA014P1(strStartYm, strEndYm, strEmpNo, strEmpNm, strWcCd, strWcNm);
        public HAA014P1(string StartYm, string EndYm, string WcCd, string WcNm, string EmpNo, string EmpNm)
        {
            InitializeComponent();

            strStartYm = StartYm;
            strEndYm = EndYm;
            strWcCd = WcCd;
            strWcNm = WcNm;
            strEmpNo = EmpNo;
            strEmpNm = EmpNm;           
        }
        #endregion

        #region 폼로드 이벤트
        private void HAA014P1_Load(object sender, EventArgs e)
        {
            //필수 적용
            SystemBase.Validation.GroupBox_Setting(groupBox1);
        
            this.Text = "반별작업자별 TOUCH실적 상세조회";
            UIForm.Buttons.ReButton("010000001001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

            dtpDate.Value = strStartYm;
            dtpDateTo.Value = strEndYm;

            txtWcCd.ReadOnly = false;
            txtWcNm.ReadOnly = false;
            txtWorkDuty.ReadOnly = false;
            txtWorkDutyNm.ReadOnly = false;

            txtWcCd.Text = strWcCd;
            txtWcNm.Text = strWcNm;
            txtWorkDuty.Text = strEmpNo;
            txtWorkDutyNm.Text = strEmpNm;

            txtWcCd.ReadOnly = true;
            txtWcNm.ReadOnly = true;
            txtWorkDuty.ReadOnly = true;
            txtWorkDutyNm.ReadOnly = true;

            rdoDailySum.Checked = true;

            SearchExec(); 
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1) == true)
            {
                this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

                try
                {
                    if (rdoDailySum.Checked == true)
                    {
                        string strQuery = " usp_HAA014  @pTYPE = 'S3'";
                        strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                        strQuery += ", @pSTART_YYMM = '" + dtpDate.Text.Replace("-", "") + "' ";
                        strQuery += ", @pEND_YYMM = '" + dtpDateTo.Text.Replace("-", "") + "' ";
                        strQuery += ", @pWC_CD = '" + txtWcCd.Text + "' ";
                        strQuery += ", @pH_RES_CD = '" + txtWorkDuty.Text + "' ";

                        UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false, true);
                    }
                    else
                    {
                        string strQuery = " usp_HAA014  @pTYPE = 'S4'";
                        strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                        strQuery += ", @pSTART_YYMM = '" + dtpDate.Text.Replace("-", "") + "' ";
                        strQuery += ", @pEND_YYMM = '" + dtpDateTo.Text.Replace("-", "") + "' ";
                        strQuery += ", @pWC_CD = '" + txtWcCd.Text + "' ";
                        strQuery += ", @pH_RES_CD = '" + txtWorkDuty.Text + "' ";

                        UIForm.FPMake.grdCommSheet(fpSpread2, strQuery, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, 0, 0, false, true);
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

        #region rdoDailySum_CheckedChanged()
        private void rdoDailySum_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoDailySum.Checked == true)
            {
                GridCommPanel1.Visible = true;
                GridCommPanel2.Visible = false;
            }
            else
            {
                GridCommPanel1.Visible = false;
                GridCommPanel2.Visible = true;
            }
        }
        #endregion

        private void rdoWorkNo_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoWorkNo.Checked == true)
            {
                GridCommPanel1.Visible = false;
                GridCommPanel2.Visible = true;
            }
            else
            {
                GridCommPanel1.Visible = true;
                GridCommPanel2.Visible = false;
            }
        }
    }
}
