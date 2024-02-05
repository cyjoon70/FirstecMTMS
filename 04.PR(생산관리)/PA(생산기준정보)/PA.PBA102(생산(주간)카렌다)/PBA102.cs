#region 작성정보
/*********************************************************************/
// 단위업무명 : 생산(주간)카렌다
// 작 성 자 : 권 순 철
// 작 성 일 : 2013-04-05
// 작성내용 : 생산(주간)카렌다 및 관리
// 수 정 일 :
// 수 정 자 :
// 수정내용 :
// 비    고 :
/*********************************************************************/
#endregion

using System;
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;

namespace PA.PBA102
{
    public partial class PBA102 : UIForm.FPCOMM2
    {
        #region 변수선언
        int PreRow = -1;   // SelectionChanged 시에 동일 Row에서 데이타변환 처리 안하도록 하기 위함.
        string strInspReqNo = "";
        int SearchRow = 0;
        int ShowColumn = 0;
        bool chk = true;

        string[] HeadText = new string[] { "", "", "", "", "", "", "", "월요일", "월요일", "", "화요일", "화요일", "", "수요일", "수요일", "", "목요일", "목요일", "", "금요일", "금요일", "", "토요일", "토요일", "", "일요일", "일요일", "" }; // 첫번째 Head Text
        string[] HeadText2 = new string[] { "", "SCH_ID", "공장", "구분", "자원", "적용일", "교대", "START", "END", "능력", "START", "END", "능력", "START", "END", "능력", "START", "END", "능력", "START", "END", "능력", "START", "END", "능력", "START", "END", "능력" }; // 첫번째 Head Text
        string[] TxtAlign = new string[] { "", "C", "C", "C", "C", "C", "C", "C", "C", "C", "R", "C", "C", "R", "C", "C", "R", "C", "C", "R", "C", "C", "R", "C", "C", "R", "C", "C", "R" };					// Cell 데이타 정렬방식
        string[] CellType = new string[] { "", "", "", "", "", "", "CB", "MK99:99", "MK99:99", "NM0", "MK99:99", "MK99:99", "NM0", "MK99:99", "MK99:99", "NM0", "MK99:99", "MK99:99", "NM0", "MK99:99", "MK99:99", "NM0", "MK99:99", "MK99:99", "NM0", "MK99:99", "MK99:99", "NM0" };						// CellType 지정
        string[] ComboMsg = new string[] { "6:1교대#2교대#3교대" };
        int[] HeadWidth = new int[] { 0, 0, 0, 0, 0, 0, 60, 45, 45, 35, 45, 45, 35, 45, 45, 35, 45, 45, 35, 45, 45, 35, 45, 45, 35, 45, 45, 35 };						// Cell 넓이
        int[] shtTitleSpan = new int[] { 1, 1, 1, 1, 1, 1, 1, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3 };							// TitleSpan(Colspan 2인경우 2개 합함)
        int[] HeaderRowCount = new int[] { 2 };									// Head 수량
        int[] CColor = new int[] { 0, 2, 2, 2, 2, 2, 1, 0, 0, 3, 0, 0, 3, 0, 0, 3, 0, 0, 3, 0, 0, 3, 0, 0, 3, 0, 0, 3, 0, 0, 3 };							// Cell 색상 및 ReadOnly 설정(0:일반, 1:필수, 2:ReadOnly)

        string[] HeadText11 = new string[] { "", "스케쥴 ID", "스케쥴 명", "공장구분", "자원그룹", "자원번호", "적용일자" }; // 첫번째 Head Text
        string[] HeadText12 = new string[] { "" }; // 첫번째 Head Text
        string[] TxtAlign11 = new string[] { "", "C", "C", "", "", "", "C" };				// Cell 데이타 정렬방식
        string[] CellType11 = new string[] { "", "", "", "CB", "CB", "CB", "" };					// CellType 지정
        string[] ComboMsg11 = new string[] { "" };
        int[] HeadWidth11 = new int[] { 0, 70, 80, 80, 80, 80, 80 };						// Cell 넓이
        int[] shtTitleSpan11 = new int[] { 1, 1, 1, 1, 1, 1, 1 };								// TitleSpan(Colspan 2인경우 2개 합함)
        int[] HeaderRowCount11 = new int[] { 1 };											// Head 수량
        int[] CColor11 = new int[] { 0, 4, 4, 4, 4, 4, 4 };								// Cell 색상 및 ReadOnly 설정(0:일반, 1:필수, 2:ReadOnly)
        #endregion
        
        #region 생성자
        public PBA102()
        {
            InitializeComponent();
        }
        #endregion

        #region 행 추가, 삭제
        protected override void DelExec() { UIForm.FPMake.RowRemove(fpSpread1); }		// 행 삭제
        protected override void RowInsExec()
        {	// 행 추가
            UIForm.FPMake.RowInsert(fpSpread1);
            int intRow = fpSpread1.ActiveSheet.GetSelection(0).Row;
            fpSpread1.ActiveSheet.Cells[intRow, 1].Text = cboSch_id.SelectedValue.ToString();
            string strPlantCd = cboPlant_cd.SelectedValue.ToString();
            fpSpread1.ActiveSheet.Cells[intRow, 2].Text = strPlantCd;
            if (strPlantCd != "*")
            {
                string strResKind = "*"; if (cboRes_Kind.SelectedValue.ToString() != "") strResKind = cboRes_Kind.SelectedValue.ToString();
                fpSpread1.ActiveSheet.Cells[intRow, 3].Text = strResKind;
                fpSpread1.ActiveSheet.Cells[intRow, 4].Text = cboRes_id.SelectedValue.ToString();
            }
            else
            {
                fpSpread1.ActiveSheet.Cells[intRow, 3].Text = "*";
                fpSpread1.ActiveSheet.Cells[intRow, 4].Text = "*";
            }

            fpSpread1.ActiveSheet.Cells[intRow, 5].Text = dtmCon_dt.Text.ToString();
        }
        #endregion

        #region RCopyExec 그리드 Row 복사
        protected override void RCopyExec()
        {
            UIForm.FPMake.RowCopy(fpSpread1);
        }
        #endregion

        #region PrintMake(그리드, 미리보기) 그리드 Print
        protected override void PrintExec()
        {
            UIForm.FPMake.PrintMake(fpSpread1, true, 9);
        }
        #endregion

        #region Form Load 시
        private void PBA102_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            SystemBase.Validation.GroupBox_Setting(groupBox2);

            SystemBase.ComboMake.C1Combo(cboSch_id, "usp_PBA102 'C1', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);
            SystemBase.ComboMake.C1Combo(cboPlant_cd, "usp_PBA102 'C4', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);
            SystemBase.ComboMake.C1Combo(cboRes_Kind, "usp_PBA102 'C5', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);
            SystemBase.ComboMake.C1Combo(cboRes_id, "usp_PBA102 'C6', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);

            cboRes_Kind.Enabled = false;
            cboRes_id.Enabled = false;
            // 그리드 초기화
            UIForm.FPMake.grdMakeSheet(fpSpread1, HeadText, shtTitleSpan, HeadText2, TxtAlign, HeadWidth, ComboMsg, HeaderRowCount, CellType, CColor);//그리드 데이타 리셋

            // 오른쪽 그리드 조회
            //UIForm.FPMake.grdMakeSheet(fpSpread2, HeadText11, shtTitleSpan11, HeadText12, TxtAlign11, HeadWidth11, ComboMsg11, HeaderRowCount11, CellType11, CColor11);//그리드 데이타 리셋

            string SQuery11 = " usp_PBA102 'S3', @pSch_id='" + txtSSch_id.Text.ToString() + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

            ComboMsg11 = new string[3];
            ComboMsg11[0] = ComboGrid("usp_PBA102 @pType='C4', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 3);
            ComboMsg11[1] = ComboGrid("usp_PBA102 @pType='C5', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 4);
            ComboMsg11[2] = ComboGrid("usp_PBA102 @pType='C3', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 5);
            UIForm.FPMake.grdMakeSheet(fpSpread2, SQuery11, HeadText11, shtTitleSpan11, HeadText12, TxtAlign11, HeadWidth11, ComboMsg11, HeaderRowCount11, CellType11, CColor11, 0, 0, false);

            dtmCon_dt.Text = SystemBase.Base.ServerTime("YYMMDD");
        }
        #endregion

        #region NewExec()
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            cboSch_id.Enabled = true;
            cboPlant_cd.Enabled = true;
            UIForm.FPMake.grdMakeSheet(fpSpread1, HeadText, shtTitleSpan, HeadText2, TxtAlign, HeadWidth, ComboMsg, HeaderRowCount, CellType, CColor);//그리드 데이타 초기화
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                // 초기화
                UIForm.FPMake.grdMakeSheet(fpSpread1, HeadText, shtTitleSpan, HeadText2, TxtAlign, HeadWidth, ComboMsg, HeaderRowCount, CellType, CColor);//그리드 데이타 리셋
                // 조  회
                string SQuery = " usp_PBA102 'S1', @pSch_id='" + cboSch_id.SelectedValue.ToString() + "'";
                SQuery += ", @pPLANT_CD='" + cboPlant_cd.SelectedValue.ToString() + "'";

                if (cboPlant_cd.SelectedIndex != 0)
                {
                    SQuery += ", @pRES_KIND='" + cboRes_Kind.SelectedText.ToString() + "'";
                    SQuery += ", @pRES_ID='" + cboRes_id.SelectedValue.ToString() + "'";
                }
                SQuery += ", @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

                UIForm.FPMake.grdMakeSheet(fpSpread1, SQuery, HeadText, shtTitleSpan, HeadText2, TxtAlign, HeadWidth, ComboMsg, HeaderRowCount, CellType, CColor, 0, 1, true);

                string SQuery11 = " usp_PBA102 'S3', @pSch_id='" + txtSSch_id.Text.ToString() + "'";
                SQuery11 += ", @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

                UIForm.FPMake.grdMakeSheet(fpSpread2, SQuery11, HeadText11, shtTitleSpan11, HeadText12, TxtAlign11, HeadWidth11, ComboMsg11, HeaderRowCount11, CellType11, CColor11, 0, 0, false);
            }
        }
        #endregion

        #region SaveExec() 폼에 입력된 데이타 저장 로직
        protected override void SaveExec()
        {
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                string RtnMsg = "성공적으로 처리되었습니다.";

                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);
                cmd.Transaction = Trans;

                try
                {
                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {
                        string Query = "";
                        if (fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text == "U")
                        {
                            string iSchId = cboSch_id.SelectedValue.ToString();
                            string iPlantCd = cboPlant_cd.SelectedValue.ToString();
                            string iResId = "*", iResKind = "*";
                            if (iPlantCd != "*")
                            {
                                iResId = cboRes_id.SelectedValue.ToString();
                                iResKind = cboRes_Kind.SelectedValue.ToString();
                            }

                            Query = " usp_PBA102 'U1'";
                            Query += ", @pSch_id = '" + iSchId + "' ";
                            Query += ", @pPlant_cd = '" + iPlantCd + "' ";
                            Query += ", @pRes_id = '" + iResId + "' ";
                            Query += ", @pRes_Kind = '" + iResKind + "' ";
                            Query += ", @pCon_dt = '" + dtmCon_dt.Text + "' ";
                            Query += ", @pSCH_NUM = '" + fpSpread1.Sheets[0].Cells[i, 0].Value + "' ";
                            Query += ", @pShift = '" + fpSpread1.Sheets[0].Cells[i, 6].Value + "' ";
                            Query += ", @pMON_ST = '" + fpSpread1.Sheets[0].Cells[i, 7].Value + "' ";
                            Query += ", @pMON_ED = '" + fpSpread1.Sheets[0].Cells[i, 8].Value + "' ";
                            Query += ", @pTUE_ST = '" + fpSpread1.Sheets[0].Cells[i, 10].Value + "' ";
                            Query += ", @pTUE_ED = '" + fpSpread1.Sheets[0].Cells[i, 11].Value + "' ";
                            Query += ", @pWED_ST = '" + fpSpread1.Sheets[0].Cells[i, 13].Value + "' ";
                            Query += ", @pWED_ED = '" + fpSpread1.Sheets[0].Cells[i, 14].Value + "' ";
                            Query += ", @pTHU_ST = '" + fpSpread1.Sheets[0].Cells[i, 16].Value + "' ";
                            Query += ", @pTHU_ED = '" + fpSpread1.Sheets[0].Cells[i, 17].Value + "' ";
                            Query += ", @pFRI_ST = '" + fpSpread1.Sheets[0].Cells[i, 19].Value + "' ";
                            Query += ", @pFRI_ED = '" + fpSpread1.Sheets[0].Cells[i, 20].Value + "' ";
                            Query += ", @pSAT_ST = '" + fpSpread1.Sheets[0].Cells[i, 22].Value + "' ";
                            Query += ", @pSAT_ED = '" + fpSpread1.Sheets[0].Cells[i, 23].Value + "' ";
                            Query += ", @pSUN_ST = '" + fpSpread1.Sheets[0].Cells[i, 25].Value + "' ";
                            Query += ", @pSUN_ED = '" + fpSpread1.Sheets[0].Cells[i, 26].Value + "' ";
                            Query += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "' ";
                            Query += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                            cmd.CommandText = Query;
                            cmd.ExecuteNonQuery();
                        }
                        else if (fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text == "I")
                        {
                            Query = " usp_PBA102 'I1'";

                            string iSchId = cboSch_id.SelectedValue.ToString();
                            string iPlantCd = cboPlant_cd.SelectedValue.ToString();
                            string iResId = "*", iResKind = "*";
                            if (iPlantCd != "*")
                            {
                                iResId = cboRes_id.SelectedValue.ToString();
                                iResKind = cboRes_Kind.SelectedValue.ToString();
                            }

                            Query += ", @pSch_id = '" + iSchId + "' ";
                            Query += ", @pPlant_cd = '" + iPlantCd + "' ";
                            Query += ", @pRes_id = '" + iResId + "' ";
                            Query += ", @pRes_Kind = '" + iResKind + "' ";
                            Query += ", @pCon_dt = '" + dtmCon_dt.Text + "' ";
                            Query += ", @pSCH_NUM = '" + fpSpread1.Sheets[0].Cells[i, 0].Value + "' ";
                            Query += ", @pShift = '" + fpSpread1.Sheets[0].Cells[i, 6].Value + "' ";
                            Query += ", @pMON_ST = '" + fpSpread1.Sheets[0].Cells[i, 7].Value + "' ";
                            Query += ", @pMON_ED = '" + fpSpread1.Sheets[0].Cells[i, 8].Value + "' ";
                            Query += ", @pTUE_ST = '" + fpSpread1.Sheets[0].Cells[i, 10].Value + "' ";
                            Query += ", @pTUE_ED = '" + fpSpread1.Sheets[0].Cells[i, 11].Value + "' ";
                            Query += ", @pWED_ST = '" + fpSpread1.Sheets[0].Cells[i, 13].Value + "' ";
                            Query += ", @pWED_ED = '" + fpSpread1.Sheets[0].Cells[i, 14].Value + "' ";
                            Query += ", @pTHU_ST = '" + fpSpread1.Sheets[0].Cells[i, 16].Value + "' ";
                            Query += ", @pTHU_ED = '" + fpSpread1.Sheets[0].Cells[i, 17].Value + "' ";
                            Query += ", @pFRI_ST = '" + fpSpread1.Sheets[0].Cells[i, 19].Value + "' ";
                            Query += ", @pFRI_ED = '" + fpSpread1.Sheets[0].Cells[i, 20].Value + "' ";
                            Query += ", @pSAT_ST = '" + fpSpread1.Sheets[0].Cells[i, 22].Value + "' ";
                            Query += ", @pSAT_ED = '" + fpSpread1.Sheets[0].Cells[i, 23].Value + "' ";
                            Query += ", @pSUN_ST = '" + fpSpread1.Sheets[0].Cells[i, 25].Value + "' ";
                            Query += ", @pSUN_ED = '" + fpSpread1.Sheets[0].Cells[i, 26].Value + "' ";
                            Query += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "' ";
                            Query += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                            cmd.CommandText = Query;
                            cmd.ExecuteNonQuery();
                        }
                        else if (fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text == "D")
                        {
                            Query = " usp_PBA102 'D1' ";
                            Query += ",@pSCH_NUM='" + fpSpread1.Sheets[0].Cells[i, 0].Value + "' ";
                            Query += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                            cmd.CommandText = Query;
                            cmd.ExecuteNonQuery();
                        }
                    }

                    string Query2 = " usp_PBA102 'I2'";
                    Query2 = Query2 + ", @pSch_id = '" + cboSch_id.SelectedValue.ToString() + "'";
                    string strPlantCd = cboPlant_cd.SelectedValue.ToString();
                    Query2 = Query2 + ", @pPlant_cd = '" + strPlantCd + "'";
                    if (strPlantCd != "*")
                    {
                        Query2 = Query2 + ", @pRes_id = '" + cboRes_id.SelectedValue.ToString() + "'";
                        Query2 = Query2 + ", @pRes_Kind = '" + cboRes_Kind.SelectedValue.ToString() + "'";
                    }
                    else
                    {
                        Query2 = Query2 + ", @pRes_id = '*'";
                        Query2 = Query2 + ", @pRes_Kind = '*'";
                    }
                    Query2 = Query2 + ", @pCon_dt = '" + dtmCon_dt.Text.ToString() + "'";
                    Query2 = Query2 + ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "' ";
                    Query2 = Query2 + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    cmd.CommandText = Query2;
                    cmd.ExecuteNonQuery();

                    Trans.Commit();

                    // 초기화
                    string SQuery = " usp_PBA102 'S1', @pSch_id='" + cboSch_id.SelectedValue.ToString() + "'";
                    SQuery += ", @pPLANT_CD='" + cboPlant_cd.SelectedValue.ToString() + "'";
                    //					if(cboPlant_cd.SelectedIndex != 0)
                    //					{
                    //						SQuery = SQuery + ", @pRES_KIND='"+ cboRes_Kind.SelectedValue.ToString() +"'";
                    //						SQuery = SQuery + ", @pRES_ID='"+ cboRes_id.SelectedValue.ToString() +"'";
                    //					}
                    //					else
                    //					{
                    SQuery += ", @pRES_KIND='" + fpSpread2.ActiveSheet.Cells[fpSpread2.ActiveSheet.ActiveRowIndex, 4].Value + "'";
                    SQuery += ", @pRES_ID='" + fpSpread2.ActiveSheet.Cells[fpSpread2.ActiveSheet.ActiveRowIndex, 5].Value + "'";
                    SQuery += ", @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

                    //					}

                    UIForm.FPMake.grdMakeSheet(fpSpread1, SQuery, HeadText, shtTitleSpan, HeadText2, TxtAlign, HeadWidth, ComboMsg, HeaderRowCount, CellType, CColor, 0, 1, true);

                    //string SQuery11 = " usp_PBA102 'S3', @pSch_id='"+ txtSSch_id.Text.ToString() +"'";
                    //UIForm.FPMake.grdMakeSheet(fpSpread2, SQuery11, HeadText11, shtTitleSpan11, HeadText12, TxtAlign11, HeadWidth11, ComboMsg11, HeaderRowCount11, CellType11, CColor11, 0, 0, false);

                    string SQuery11 = " usp_PBA102 'S3', @pSch_id='" + txtSSch_id.Text.ToString() + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                    UIForm.FPMake.grdMakeSheet(fpSpread2, SQuery11, HeadText11, shtTitleSpan11, HeadText12, TxtAlign11, HeadWidth11, ComboMsg11, HeaderRowCount11, CellType11, CColor11, 0, 0, false);

                }
                catch (Exception f)
                {
                    Trans.Rollback();
                    RtnMsg = "에러가 발생되어 롤백되었습니다.\n\r\n\r" + f.ToString();
                }
                dbConn.Close();
                MessageBox.Show(RtnMsg);
            }
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

        #region 이벤트들
        private void cboSch_id_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (cboSch_id.SelectedValue.ToString().Length > 0)
            {
                cboPlant_cd.Enabled = true;
                //cboRes_Kind.Enabled = true;
                //cboRes_id.Enabled = true;
            }
            else
            {
                cboPlant_cd.Enabled = false;
                cboRes_Kind.Enabled = false;
                cboRes_id.Enabled = false;
                //				cboPlant_cd.SelectedIndex = 0;
                //				cboRes_Kind.SelectedIndex = 0;
                //				cboRes_id.SelectedIndex = 0;
            }
        }


        private void cboPlant_cd_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (cboPlant_cd.SelectedIndex == 0)
            {
                cboRes_Kind.Enabled = false;
                cboRes_id.Enabled = false;

                if (fpSpread1.Sheets[0].Rows.Count > 0)
                {
                    if (chk == true)
                    {
                        for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                        {
                            fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text = "";
                        }
                    }
                }
            }
            else
            {
                cboRes_Kind.Enabled = true;
                cboRes_id.Enabled = true;

                if (fpSpread1.Sheets[0].Rows.Count > 0)
                {
                    if (chk == true)
                    {
                        for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count - 1; i++)
                        {
                            fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text = "U";
                        }
                    }
                }
            }
        }

        private void cboRes_Kind_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (cboRes_Kind.Text == "전체")
            {
                cboRes_id.DataSource = null;
                SystemBase.ComboMake.C1Combo(cboRes_id, "usp_PBA102 'C6', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);
                btnRecMake.Visible = false;

            }
            else if (cboRes_Kind.Text == "자원번호")
            {
                cboRes_id.DataSource = null;
                SystemBase.ComboMake.C1Combo(cboRes_id, "usp_PBA102 'C3', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);
                btnRecMake.Visible = false;
            }
            else
            {
                cboRes_id.DataSource = null;
                SystemBase.ComboMake.C1Combo(cboRes_id, "usp_PBA102 'C2', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);
                btnRecMake.Visible = false;
            }

            if (fpSpread1.Sheets[0].Rows.Count > 0)
            {
                if (chk == true)
                {
                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count - 1; i++)
                    {
                        fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text = "U";
                    }
                }
            }
        }


        private void fpSpread2_CellClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            try
            {
                if (fpSpread2.Sheets[0].Rows.Count > 0)
                {
                    chk = false;

                    UIForm.FPMake.grdMakeSheet(fpSpread1, HeadText, shtTitleSpan, HeadText2, TxtAlign, HeadWidth, ComboMsg, HeaderRowCount, CellType, CColor);//그리드 데이타 리셋
                    // 조  회
                    string SQuery = " usp_PBA102 'S1'";
                    SQuery += ", @pSch_id='" + fpSpread2.ActiveSheet.Cells[e.Row, 1].Text + "'";
                    SQuery += ", @pPLANT_CD='" + fpSpread2.ActiveSheet.Cells[e.Row, 3].Value + "'";
                    SQuery += ", @pRES_KIND='" + fpSpread2.ActiveSheet.Cells[e.Row, 4].Value + "'";
                    SQuery += ", @pRES_ID='" + fpSpread2.ActiveSheet.Cells[e.Row, 5].Value + "'";
                    SQuery += ", @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

                    UIForm.FPMake.grdMakeSheet(fpSpread1, SQuery, HeadText, shtTitleSpan, HeadText2, TxtAlign, HeadWidth, ComboMsg, HeaderRowCount, CellType, CColor, 0, 1, true);

                    if (e.Row > -1)
                    {
                        cboSch_id.SelectedValue = fpSpread2.ActiveSheet.Cells[e.Row, 1].Text.ToString();
                        cboPlant_cd.SelectedValue = fpSpread2.ActiveSheet.Cells[e.Row, 3].Value;
                        cboRes_Kind.SelectedValue = fpSpread2.ActiveSheet.Cells[e.Row, 4].Value;
                        cboRes_id.SelectedValue = fpSpread2.ActiveSheet.Cells[e.Row, 5].Value;
                        dtmCon_dt.Text = fpSpread2.ActiveSheet.Cells[e.Row, 6].Text.ToString();

                        cboSch_id.Enabled = false;
                        //						cboPlant_cd.Enabled		= false;
                        if (cboPlant_cd.SelectedIndex == 0)
                        {
                            cboRes_Kind.Enabled = false;
                            cboRes_id.Enabled = false;
                            btnRecMake.Visible = false;
                        }
                        else
                        {
                            cboRes_Kind.Enabled = true;
                            cboRes_id.Enabled = true;

                            if (cboRes_Kind.SelectedIndex == 2)
                            {
                                btnRecMake.Visible = false;
                            }
                            else
                            {
                                btnRecMake.Visible = false;
                            }
                        }
                    }
                    else
                        SystemBase.Validation.GroupBox_Reset(groupBox1);
                }

                chk = true;
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString());
            }
        }


        private void txtSSch_id_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            if (Convert.ToInt32(e.KeyChar) == 13)
            {
                string SQuery11 = " usp_PBA102 'S3', @pSch_id='" + txtSSch_id.Text.ToString() + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                UIForm.FPMake.grdMakeSheet(fpSpread2, SQuery11, HeadText11, shtTitleSpan11, HeadText12, TxtAlign11, HeadWidth11, ComboMsg11, HeaderRowCount11, CellType11, CColor11, 0, 0, false);

                if (fpSpread2.ActiveSheet.Rows.Count < 1)
                { UIForm.FPMake.grdMakeSheet(fpSpread2, HeadText11, shtTitleSpan11, HeadText12, TxtAlign11, HeadWidth11, ComboMsg11, HeaderRowCount11, CellType11, CColor11); }
            }
        }

        private void btnRecMake_Click(object sender, System.EventArgs e)
        {
            if (cboRes_id.SelectedValue.ToString().Length == 0)
            {
                MessageBox.Show("자원그룹명을 선택하세요.");
            }
            else
            {
                PBA102P1 frm = new PBA102P1(cboRes_id.SelectedValue.ToString(), cboRes_id.Text.ToString());
                frm.ShowDialog();
            }

        }

        private void cboRes_id_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (fpSpread1.Sheets[0].Rows.Count > 0)
            {
                if (chk == true)
                {
                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count - 1; i++)
                    {
                        fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text = "U";
                    }
                }
            }
        }

        private void dtmCon_dt_ValueChanged(object sender, System.EventArgs e)
        {
            if (fpSpread1.Sheets[0].Rows.Count > 0)
            {
                if (chk == true)
                {
                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count - 1; i++)
                    {
                        fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text = "U";
                    }
                }
            }
        }
        #endregion

        #region DelExec() 삭제 로직
        protected override void DeleteExec()
        {
            string msg = SystemBase.Base.MessageRtn("P0008");
            DialogResult dsMsg = MessageBox.Show(msg, "삭제확인", MessageBoxButtons.YesNo, MessageBoxIcon.Stop);

            if (dsMsg == DialogResult.Yes)
            {
                string ERRCode, MSGCode = "";
                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                try
                {
                    string strSql1 = " usp_PBA102  'D2'";
                    strSql1 = strSql1 + ", @pSch_id = '" + cboSch_id.SelectedValue.ToString() + "'";
                    string strPlantCd = cboPlant_cd.SelectedValue.ToString();
                    strSql1 = strSql1 + ", @pPlant_cd = '" + strPlantCd + "'";
                    if (strPlantCd != "*")
                    {
                        strSql1 = strSql1 + ", @pRes_id = '" + cboRes_id.SelectedValue.ToString() + "'";
                        strSql1 = strSql1 + ", @pRes_Kind = '" + cboRes_Kind.SelectedValue.ToString() + "'";
                    }
                    else
                    {
                        strSql1 = strSql1 + ", @pRes_id = '*'";
                        strSql1 = strSql1 + ", @pRes_Kind = '*'";
                    }
                    strSql1 = strSql1 + ", @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

                    DataSet ds = SystemBase.DbOpen.TranDataSet(strSql1, dbConn, Trans);
                    ERRCode = ds.Tables[0].Rows[0][0].ToString();
                    MSGCode = ds.Tables[0].Rows[0][1].ToString();

                    if (ERRCode == "ER") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                    Trans.Commit();
                    NewExec();
                    string SQuery11 = " usp_PBA102 'S3', @pSch_id='" + txtSSch_id.Text.ToString() + "'";
                    SQuery11 = SQuery11 + ", @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                    UIForm.FPMake.grdMakeSheet(fpSpread2, SQuery11, HeadText11, shtTitleSpan11, HeadText12, TxtAlign11, HeadWidth11, ComboMsg11, HeaderRowCount11, CellType11, CColor11, 0, 0, false);
                }
                catch (Exception e)
                {
                    SystemBase.Loggers.Log(this.Name, e.ToString());
                    Trans.Rollback();
                    MSGCode = "P0019";
                }
            Exit:
                dbConn.Close();
                MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode));
            }
        }
        #endregion

        #region fpSpread1_EditChange
        private void fpSpread1_EditChange(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            UIForm.FPMake.fpChange(fpSpread1, e.Row);
        }
        #endregion

        #region ComboGrid
        public static string ComboGrid(string Query, int Where)
        {	//                           쿼리,	       Return 위치
            string Rtn;
            string RtnTmp1 = "";
            string RtnTmp2 = "";
            DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                RtnTmp1 = RtnTmp1 + "#" + dt.Rows[i][0].ToString();
                RtnTmp2 = RtnTmp2 + "#" + dt.Rows[i][1].ToString();
            }
            return Rtn = Convert.ToString(Where) + ":" + RtnTmp1 + "|" + RtnTmp2;
        }
        #endregion

        #region 기준달력 생성
        private void btnCRT_RESO_CAL_Click(object sender, System.EventArgs e)
        {
            PBA102P2 frm = new PBA102P2();
            frm.ShowDialog();
        }
        #endregion

    }
}