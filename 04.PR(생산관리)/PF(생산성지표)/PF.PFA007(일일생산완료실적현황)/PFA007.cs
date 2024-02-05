#region 작성정보
/*********************************************************************/
// 단위업무명 : 작업자별 생산성지표조회
// 작 성 자 : pes
// 작 성 일 : 2015-06-15
// 작성내용 : 작업자별 생산성지표조회
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
using FarPoint.Win.Spread;
using WNDW;

namespace PF.PFA007
{
    public partial class PFA007 : UIForm.FPCOMM1
    {
        #region 변수선언
        int lastCol = 30;
        #endregion

        public PFA007()
        {
            InitializeComponent();
        }

        #region Form Load시
        private void PFA007_Load(object sender, System.EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(groupBox1); //필수체크
           
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0, true);
            dtpWorkDtFr.Value = SystemBase.Base.ServerTime("YYMMDD");
            dtpWorkDtTo.Value = SystemBase.Base.ServerTime("YYMMDD");
        }
        #endregion

        #region NewExec()
        protected override void NewExec()
        {
            //조회조건 초기화
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            fpSpread1.Sheets[0].Rows.Count = 0;

            dtpWorkDtFr.Value = SystemBase.Base.ServerTime("YYMMDD");
            dtpWorkDtTo.Value = SystemBase.Base.ServerTime("YYMMDD");      
        }
        #endregion
        
        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            // 작업일자 체크  from이 to보다 크면 메시지-------------
            DateTime Date1 = DateTime.Parse(dtpWorkDtFr.Text);
            DateTime Date2 = DateTime.Parse(dtpWorkDtTo.Text);

            if (DateTime.Compare(Date1, Date2) > 0)
            {
                MessageBox.Show("작업일자를 확인하세요!");
                return;
            }
            // 작업일자 체크 2015.07.03 ----------------------------


            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {
                    string strQuery = " usp_PFA007  @pTYPE = 'S1'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                    strQuery += ", @pWORK_DT_FR = '" + dtpWorkDtFr.Text + "'";
                    strQuery += ", @pWORK_DT_TO = '" + dtpWorkDtTo.Text + "'";


                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 5, true);

                    SheetView sheet = fpSpread1.ActiveSheet;
                    //sheet.Columns[1].MergePolicy = FarPoint.Win.Spread.Model.MergePolicy.Always;
                    //sheet.Columns[2].MergePolicy = FarPoint.Win.Spread.Model.MergePolicy.Always;
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            this.Cursor = System.Windows.Forms.Cursors.Default;
        }
        #endregion

   

    }
}
