using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;

namespace PA.PBA102
{
    public partial class PBA102P1 : UIForm.FPCOMM1
    {
        #region 변수선언
        // Tab 상단 그리드 디자인
        string[] HeadText = new string[] { "", "순서", "자원번호", "설명", "적용여부" }; // 첫번째 Head Text
        string[] HeadText2 = new string[] { "" }; // 두번째 Head Text
        string[] TxtAlign = new string[] { "", "C", "C", "L", "C" };					// Cell 데이타 정렬방식
        string[] CellType = new string[] { "", "NM", "CB", "", "CK" };						// CellType 지정
        string[] ComboMsg = null;
        int[] HeadWidth = new int[] { 0, 50, 80, 280, 40 };						// Cell 넓이
        int[] shtTitleSpan = new int[] { 1, 1, 1, 1, 1 };							// TitleSpan(Colspan 2인경우 2개 합함)
        int[] HeaderRowCount = new int[] { 1 };									// Head 수량
        int[] CColor = new int[] { 0, 2, 1, 0, 0 };							// Cell 색상 및 ReadOnly 설정(0:일반, 1:필수, 2:ReadOnly)
        #endregion

        public PBA102P1(string strRes_idValue, string strRes_idText)
        {
            //
            // Windows Form 디자이너 지원에 필요합니다.
            //
            InitializeComponent();

            txtGRES_CD.Text = strRes_idValue;
            txtGRES_DIS.Text = strRes_idText;
        }

        public PBA102P1()
        {
            InitializeComponent();
        }

        #region Form Load 시
        private void PBA102P1_Load(object sender, System.EventArgs e)
        {
            //필수체크
            string Query2 = "usp_PBA102 'C3'";
            ComboMsg = new string[] { SystemBase.ComboMake.ComboOnGrid(Query2, "2"), "4:Y#N" };
            // 초기화
            UIForm.FPMake.grdMakeSheet(fpSpread1, HeadText, shtTitleSpan, HeadText2, TxtAlign, HeadWidth, ComboMsg, HeaderRowCount, CellType, CColor);//그리드 데이타 리셋
            // 조회
            string SQuery = " usp_PBA102P 'S1', @pGRES_CD='" + txtGRES_CD.Text + "' ";
            UIForm.FPMake.grdMakeSheet(fpSpread1, SQuery, HeadText, shtTitleSpan, HeadText2, TxtAlign, HeadWidth, ComboMsg, HeaderRowCount, CellType, CColor, 0, 0, false);
        }
        #endregion

        #region NewExec() 그리드 및 그룹박스 초기화
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            UIForm.FPMake.grdMakeSheet(fpSpread1, HeadText, shtTitleSpan, HeadText2, TxtAlign, HeadWidth, ComboMsg, HeaderRowCount, CellType, CColor);//그리드 데이타 초기화
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            // 초기화
            UIForm.FPMake.grdMakeSheet(fpSpread1, HeadText, shtTitleSpan, HeadText2, TxtAlign, HeadWidth, ComboMsg, HeaderRowCount, CellType, CColor);//그리드 데이타 리셋
            // 조회
            string SQuery = " usp_PBA102P 'S1', @pGRES_CD='" + txtGRES_CD.Text + "' ";
            UIForm.FPMake.grdMakeSheet(fpSpread1, SQuery, HeadText, shtTitleSpan, HeadText2, TxtAlign, HeadWidth, ComboMsg, HeaderRowCount, CellType, CColor, 0, 0, false);
        }
        #endregion

        #region SaveExec() 폼에 입력된 데이타 저장 로직
        protected override void SaveExec()
        {
            // 그리드 상단 필수항목 체크
            if (UIForm.FPMake.FPUpCheck(fpSpread1) == true)
            {
                string RtnMsg = "성공적으로 처리되었습니다.";

                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);
                cmd.Transaction = Trans;
                //cmd.CommandTimeout = 10000;

                try
                {
                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {
                        string Query = "";
                        if (fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text == "U")
                        {
                            Query = " usp_PBA102P 'U1'";
                            Query += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";	//법인코드
                            Query += ", @pBIZ_CD = ''";	//사업장코드
                            //Query +=", '"+ cboPlant_cd.SelectedValue.ToString() +"'";	//공장코드
                            Query += ", @pGRES_CD = '" + txtGRES_CD.Text.ToString() + "'";
                            Query += ", @pGRES_DIS = '" + txtGRES_DIS.Text.ToString() + "'";
                            Query += ", @pSEQ = '" + fpSpread1.Sheets[0].Cells[i, 1].Value + "' ";
                            Query += ", @pRES_CD = '" + fpSpread1.Sheets[0].Cells[i, 2].Value + "' ";
                            Query += ", @pRES_DIS = '" + fpSpread1.Sheets[0].Cells[i, 3].Value + "' ";
                            Query += ", @pUSE_YN = '" + fpSpread1.Sheets[0].Cells[i, 4].Value + "' ";
                            Query += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "' ";

                            cmd.CommandText = Query;
                            cmd.ExecuteNonQuery();
                        }
                        else if (fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text == "I")
                        {
                            Query = " usp_PBA102P 'I1'";
                            Query += ", '" + SystemBase.Base.gstrCOMCD + "'";	//법인코드
                            Query += ", ''";	//사업장코드
                            //							Query +=", '"+ cboPlant_cd.SelectedValue.ToString() +"'";	//공장코드
                            Query += ", '" + txtGRES_CD.Text.ToString() + "'";
                            Query += ", '" + txtGRES_DIS.Text.ToString() + "'";

                            Query += ",'" + fpSpread1.Sheets[0].Cells[i, 1].Value + "' ";
                            Query += ",'" + fpSpread1.Sheets[0].Cells[i, 2].Value + "' ";
                            Query += ",'" + fpSpread1.Sheets[0].Cells[i, 3].Value + "' ";
                            Query += ",'" + fpSpread1.Sheets[0].Cells[i, 4].Value + "' ";

                            Query += ",'" + SystemBase.Base.gstrUserID + "' ";
                            cmd.CommandText = Query;
                            cmd.ExecuteNonQuery();
                        }
                        else if (fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text == "D")
                        {
                            Query = " usp_PBA102P 'D1' ";
                            Query += ",@pGRES_CD='" + txtGRES_CD.Text.ToString() + "' ";
                            Query += ",@PRES_CD='" + fpSpread1.Sheets[0].Cells[i, 2].Value + "' ";

                            cmd.CommandText = Query;
                            cmd.ExecuteNonQuery();
                        }

                    }

                    Trans.Commit();
                }
                catch (Exception f)
                {
                    Trans.Rollback();
                    RtnMsg = "에러가 발생되어 롤백되었습니다.\n\r\n\r" + f.ToString();
                }
                dbConn.Close();

                MessageBox.Show(RtnMsg);

                // 초기화
                UIForm.FPMake.grdMakeSheet(fpSpread1, HeadText, shtTitleSpan, HeadText2, TxtAlign, HeadWidth, ComboMsg, HeaderRowCount, CellType, CColor);//그리드 데이타 리셋
                // 조회
                string SQuery = " usp_PBA102P 'S1', @pGRES_CD='" + txtGRES_CD.Text + "' ";
                UIForm.FPMake.grdMakeSheet(fpSpread1, SQuery, HeadText, shtTitleSpan, HeadText2, TxtAlign, HeadWidth, ComboMsg, HeaderRowCount, CellType, CColor, 0, 0, false);
            }
        }
        #endregion

        #region 행삭제
        protected override void DelExec()
        {
            UIForm.FPMake.RowRemove(fpSpread1);
        }
        #endregion

        #region 행추가
        protected override void RowInsExec()
        {
            UIForm.FPMake.RowInsert(fpSpread1);
        }
        #endregion

        #region PrintMake(그리드, 미리보기) 그리드 Print
        protected override void PrintExec()
        {
            UIForm.FPMake.PrintMake(fpSpread1, true, 9);
        }
        #endregion

        #region fpSpread1_Change 데이타 수정시 U 플래그 등록
        private void fpSpread1_Change(object sender, FarPoint.Win.Spread.ChangeEventArgs e)
        {
            UIForm.FPMake.fpChange(fpSpread1, e.Row);
        }
        #endregion

        #region ExcelExec() Excel 저장
        protected override void ExcelExec()
        {
            UIForm.FPMake.ExcelMake(fpSpread1, this.Text.ToString());
        }
        #endregion

        private void fpSpread1_ComboCloseUp(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            string Query = " usp_PBA102P 'C1', @pRES_CD='" + fpSpread1.Sheets[0].Cells[e.Row, 2].Value + "'";
            UIForm.FPMake.ComboCloseUp(fpSpread1, Query, "3", e.Row, 4);
        }
    }
}
