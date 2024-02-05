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

namespace PF.PFA001
{
    public partial class PFA001 : UIForm.FPCOMM1
    {
        #region 변수선언
        int lastCol = 30;
        #endregion

        public PFA001()
        {
            InitializeComponent();
        }

        #region Form Load시
        private void PFA001_Load(object sender, System.EventArgs e)
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
                    string strQuery = " usp_PFA001  @pTYPE = 'S1'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                    strQuery += ", @pWORK_DT_FR = '" + dtpWorkDtFr.Text + "'";
                    strQuery += ", @pWORK_DT_TO = '" + dtpWorkDtTo.Text + "'";
                    strQuery += ", @pWC_CD = '" + txtWcCd.Text + "'";
                    strQuery += ", @pWORK_DUTY = '" + txtWorkDuty.Text + "'";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 5, true);

                    SheetView sheet = fpSpread1.ActiveSheet;
                    sheet.Columns[1].MergePolicy = FarPoint.Win.Spread.Model.MergePolicy.Always;
                    sheet.Columns[2].MergePolicy = FarPoint.Win.Spread.Model.MergePolicy.Always;
                }

                Set_Section();  // 작업장별 평균 컬럼 합치고 색 변경
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            this.Cursor = System.Windows.Forms.Cursors.Default;
        }
        #endregion

        #region 그리드 재정의
        private void Set_Section()
        {
            int iCnt = fpSpread1.Sheets[0].RowCount;
            int iRow1 = 0;

            // 작업장별 평균 컬럼 합치고 색 변경
            for (int i = 0; i < iCnt; i++)
            {
                if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1,"작업자")].Text == "평균")
                {
                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "작업장")].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "작업장")].Text +" " +fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "작업자")].Text;
                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1,"작업장")].ColumnSpan = 2;
                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1,"작업장")].RowSpan = 4;
                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1,"작업장")].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "작업장")].BackColor = SystemBase.Base.gColor1;

                    for (int j = 1; j < fpSpread1.Sheets[0].ColumnCount; j++)
                    {
                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "작업자")].Text == "평균")
                            fpSpread1.Sheets[0].Cells[i, j].BackColor = SystemBase.Base.gColor1;
                        else
                            fpSpread1.Sheets[0].Cells[i, j].BackColor = SystemBase.Base.gColor2;
                    }
                    iRow1 = i + 1;
                }
            }
        }
        #endregion


        private void fpSpread1_CellDoubleClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {

            if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "작업자"))
            {
                try
                {
                    PFA001_P1 pu = new PFA001_P1("P1", SystemBase.Base.gstrCOMCD, dtpWorkDtFr.Text, dtpWorkDtTo.Text, txtWcCd.Text
                                    , fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "작업자코드")].Text.ToString()
                                    , fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "작업장")].Text.ToString()
                                    , fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "작업자")].Text.ToString() );

                    //PFA001_P1 pu = new PFA001_P1(fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text.ToString());
                    //pu.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
                    //pu.Owner = this;
                    pu.ShowDialog();
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("SY008", "TKDTPWKFY 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //|| 중 예기치 못한 오류가 발생하였습니다. 관리자에게 문의 하십시오.
                }
           
            }

     
        } 


        #region 조회조건 팝업
        //작업자
        private void btnWorkDuty_Click(object sender, EventArgs e)
        {

            try
            {
                string strQuery = " usp_P_COMMON @pTYPE = 'P054' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";				// 쿼리
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };			// 쿼리 인자값(조회조건)
                string[] strSearch = new string[] { txtWorkDuty.Text, "" };							// 쿼리 인자값에 들어갈 데이타

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00071", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "작업자 조회", false);
                pu.Width = 600;
                pu.ShowDialog();	//공통 팝업 호출
                if (pu.DialogResult == DialogResult.OK)
                {
                    string MSG = pu.ReturnVal.Replace("|", "#");
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(MSG);

                    txtWorkDuty.Text = Msgs[0].ToString();
                    txtWorkDutyNm.Value = Msgs[1].ToString();
                    txtWorkDuty.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "작업자 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //작업장
        private void btnWcCd_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pTYPE = 'P042', @pLANG_CD = 'KOR', @pETC = 'P061' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";					// 쿼리
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };					// 쿼리 인자값(조회조건)
                string[] strSearch = new string[] { txtWcCd.Text, "" };								// 쿼리 인자값에 들어갈 데이타

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00025", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "작업장 조회", false);
                pu.ShowDialog();	//공통 팝업 호출
                if (pu.DialogResult == DialogResult.OK)
                {
                    string MSG = pu.ReturnVal.Replace("|", "#");
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(MSG);

                    txtWcCd.Text = Msgs[0].ToString();
                    txtWcNm.Value = Msgs[1].ToString();
                    txtWcCd.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "부서 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
      
       
        #endregion

        #region 텍스트박스 코드 입력시 코드명 자동입력
        //작업자
        private void txtWorkDuty_TextChanged(object sender, EventArgs e)
        {
            txtWorkDutyNm.Value = SystemBase.Base.CodeName("RES_CD", "RES_DIS", "P_RESO_MANAGE", txtWorkDuty.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");
        }
        //작업장
        private void txtWcCd_TextChanged(object sender, EventArgs e)
        {
            txtWcNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtWcCd.Text, " AND MAJOR_CD = 'P061'  AND COMP_CODE='" + SystemBase.Base.gstrCOMCD + "' ");
        }
        #endregion

   

    }
}
