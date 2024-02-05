#region 작성정보
/*********************************************************************/
// 단위업무명 : 예외카렌다
// 작 성 자 : 권 순 철
// 작 성 일 : 2013-04-08
// 작성내용 : 예외카렌다 및 관리
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
using System.Data;
using System.Data.SqlClient;

namespace PA.PBA103
{
    public partial class PBA103 : UIForm.FPCOMM2
    {
        #region 변수선언
        int PreRow = -1;   // SelectionChanged 시에 동일 Row에서 데이타변환 처리 안하도록 하기 위함.
        string strSchKey = "";
        int SearchRow = 0, ShowColumn = 0;
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
        int ActiveRow = 0;
        #endregion

        #region 생성자
        public PBA103()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void PBA103_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            SystemBase.Validation.GroupBox_Setting(groupBox2);

            //콤보박스 세팅
            SystemBase.ComboMake.C1Combo(cboSch_id, "usp_PBA103 @pTYPE = 'C1', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ", 0);
            SystemBase.ComboMake.C1Combo(cboPlant_cd, "usp_PBA103 @pTYPE = 'C4', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ", 0);
            SystemBase.ComboMake.C1Combo(cboRes_Kind, "usp_PBA103 @pTYPE = 'C5', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ", 0);
            SystemBase.ComboMake.C1Combo(cboRes_id, "usp_PBA103 @pTYPE = 'C6', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ", 0);

            //그리드 콤보박스 세팅
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "교대")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'P017', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ", 0);//교대
            G2Etc[SystemBase.Base.GridHeadIndex(GHIdx2, "공장구분")] = SystemBase.ComboMake.ComboOnGrid("usp_PBA103 @pType='C4', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ", 0);
            G2Etc[SystemBase.Base.GridHeadIndex(GHIdx2, "자원그룹")] = SystemBase.ComboMake.ComboOnGrid("usp_PBA103 @pType='C5', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ", 0);
            G2Etc[SystemBase.Base.GridHeadIndex(GHIdx2, "자원번호")] = SystemBase.ComboMake.ComboOnGrid("usp_PBA103 @pType='C3', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ", 0);

            //그리드 초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
            UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, 0, 0, false);
        }
        #endregion

        #region NewExec()
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox2);
            SystemBase.Validation.GroupBoxControlsLock(groupBox2, false);

            fpSpread1.Sheets[0].Rows.Count = 0;

            cboRes_Kind.Enabled = false;
            cboRes_id.Enabled = false;
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            strSchKey = "";
            Grid2_Search();
        }
        #endregion

        #region 그리드조회
        private void Grid2_Search()
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {

                    string strQuery = " usp_PBA103 'S3', @pSch_id='" + txtSSch_id.Text.ToString() + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread2, strQuery, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, true, 0, 0, true);

                    if (fpSpread2.Sheets[0].Rows.Count > 0)
                    {
                        ResIdChange();
                        fpSpread2.Search(0, strSchKey, true, true, true, true, 0, SystemBase.Base.GridHeadIndex(GHIdx2, "스케쥴 KEY"), ref SearchRow, ref ShowColumn);

                        if (SearchRow < 0)
                        { SearchRow = 0; }

                    }
                    else
                    {
                        fpSpread1.Sheets[0].Rows.Count = 0;
                    }
                    PreRow = -1;
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이타 조회 중 오류가 발생하였습니다.

            }
            this.Cursor = Cursors.Default;
        }
        #endregion

        #region ExcelExec() Excel 저장
        protected override void ExcelExec()
        {
            UIForm.FPMake.ExcelMake(fpSpread1, this.Text.ToString());
        }
        #endregion

        #region 이벤트들
        //스케쥴ID 선택시
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

        //공장 선택시
        private void cboPlant_cd_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (cboPlant_cd.SelectedIndex == 0)
            {
                cboRes_Kind.Enabled = false;
                cboRes_id.Enabled = false;
            }
            else
            {
                cboRes_Kind.Enabled = true;
                cboRes_id.Enabled = true;
            }
        }

        //자원그룹선택시
        private void cboRes_Kind_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (cboRes_Kind.Text == "전체")
            {
                cboRes_id.DataSource = null;
                SystemBase.ComboMake.C1Combo(cboRes_id, "usp_PBA102 'C6', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 0);
                btnRecMake.Visible = false;

            }
            else if (cboRes_Kind.Text == "자원번호")
            {
                cboRes_id.DataSource = null;
                SystemBase.ComboMake.C1Combo(cboRes_id, "usp_PBA102 'C3', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 0);
                btnRecMake.Visible = false;
            }
            else
            {
                cboRes_id.DataSource = null;
                SystemBase.ComboMake.C1Combo(cboRes_id, "usp_PBA102 'C2', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 0);
                btnRecMake.Visible = false;
            }
        }
        #endregion

        #region 자원번호콤보박스재설정
        private void ResIdChange()
        {
            for (int i = 0; i < fpSpread2.Sheets[0].Rows.Count; i++)
            {
                if (fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "자원그룹")].Text == "전체")
                {
                    UIForm.FPMake.grdComboRemake(fpSpread2, i, SystemBase.Base.GridHeadIndex(GHIdx2, "자원번호"), SystemBase.ComboMake.ComboOnGrid("usp_PBA102 'C6', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0));
                    btnRecMake.Visible = false;

                }
                else if (fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "자원그룹")].Text == "자원번호")
                {
                    UIForm.FPMake.grdComboRemake(fpSpread2, i, SystemBase.Base.GridHeadIndex(GHIdx2, "자원번호"), SystemBase.ComboMake.ComboOnGrid("usp_PBA102 'C3', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0));
                    btnRecMake.Visible = false;
                }
                else
                {
                    UIForm.FPMake.grdComboRemake(fpSpread2, i, SystemBase.Base.GridHeadIndex(GHIdx2, "자원번호"), SystemBase.ComboMake.ComboOnGrid("usp_PBA102 'C2', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0));
                    btnRecMake.Visible = false;
                }
            }
        }
        #endregion

        #region Master그리드 선택시 상세정보 조회
        private void fpSpread2_SelectionChanged(object sender, FarPoint.Win.Spread.SelectionChangedEventArgs e)
        {
            if (fpSpread2.Sheets[0].Rows.Count > 0)
            {
                try
                {
                    int intRow = fpSpread2.ActiveSheet.GetSelection(0).Row;

                    //같은 Row 조회 되지 않게
                    if (intRow < 0)
                    {
                        return;
                    }

                    if (PreRow == intRow && PreRow != -1 && intRow != 0)   //현 Row에서 컬럼이동시는 조회 안되게
                    {
                        return;
                    }

                    SubSearch(intRow);
                    PreRow = intRow;
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
                }
            }
        }
        #endregion

        #region 상세조회
        private void SubSearch(int iRow)
        {
            this.Cursor = Cursors.WaitCursor;

            SystemBase.Validation.GroupBox_Reset(groupBox2);

            strSchKey = fpSpread2.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx2, "스케쥴 KEY")].Text;
            //groupBox2 값입력
            cboSch_id.SelectedValue = fpSpread2.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx2, "스케쥴 ID")].Text;
            cboPlant_cd.SelectedValue = fpSpread2.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx2, "공장구분")].Value;
            cboRes_Kind.SelectedValue = fpSpread2.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx2, "자원그룹")].Value;
            cboRes_id.SelectedValue = fpSpread2.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx2, "자원번호")].Value;
            dtmCon_dt.Value = fpSpread2.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx2, "적용일자")].Text;

            try
            {
                string strQuery = " usp_PBA103 'S2'";
                strQuery += ", @pSch_id='" + cboSch_id.SelectedValue.ToString() + "'";
                strQuery += ", @pPLANT_CD='" + cboPlant_cd.SelectedValue.ToString() + "'";
                strQuery += ", @pRES_KIND='" + cboRes_Kind.SelectedValue.ToString() + "'";
                strQuery += ", @pRES_ID='" + cboRes_id.SelectedValue.ToString() + "'";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false, false);

                if (fpSpread1.Sheets[0].Rows.Count > 0)
                {
                    SystemBase.Validation.GroupBoxControlsLock(groupBox2, true);
                }
                else
                {
                    SystemBase.Validation.GroupBoxControlsLock(groupBox2, false);
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }

            this.Cursor = Cursors.Default;
        }
        #endregion

        #region SaveExec() 데이타 저장 로직
        protected override void SaveExec()
        {
            string focusStr = "";
            fpSpread1.Focus();

            //상단 그룹박스 필수 체크
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox2))
            {

                if (fpSpread1.Sheets[0].Rows.Count > 0)
                {
                    this.Cursor = Cursors.WaitCursor;

                    string ERRCode = "WR", MSGCode = "P0000"; //처리할 내용이 없습니다.
                    SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                    SqlCommand cmd = dbConn.CreateCommand();
                    SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                    try
                    {
                        //그리드 상단 필수 체크
                        if (SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", false))
                        {

                            //Detail정보를 모두 삭제할 경우 Master정보를 삭제할지 물어보고 아니면 취소한다.
                            if (DelCheck() == false)
                            {
                                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0027"), SystemBase.Base.MessageRtn("Z0004"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                                if (dsMsg == DialogResult.Yes)
                                {
                                    try
                                    {
                                        string strDelSql = " usp_PBA103  'D2'";
                                        strDelSql += ", @pSCH_ID = '" + cboSch_id.SelectedValue.ToString() + "' ";
                                        strDelSql += ", @pPLANT_CD = '" + cboPlant_cd.SelectedValue.ToString() + "' ";
                                        strDelSql += ", @pRES_KIND = '" + cboRes_Kind.SelectedValue.ToString() + "' ";
                                        strDelSql += ", @pRES_ID = '" + cboRes_id.SelectedValue.ToString() + "' ";
                                        strDelSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                                        DataSet ds2 = SystemBase.DbOpen.TranDataSet(strDelSql, dbConn, Trans);
                                        ERRCode = ds2.Tables[0].Rows[0][0].ToString();
                                        MSGCode = ds2.Tables[0].Rows[0][1].ToString();

                                        if (ERRCode != "OK") { Trans.Rollback(); goto Exit1; }	// ER 코드 Return시 점프

                                        Trans.Commit();
                                    }
                                    catch (Exception f)
                                    {
                                        SystemBase.Loggers.Log(this.Name, f.ToString());
                                        Trans.Rollback();
                                        MSGCode = "P0001";	//에러가 발생하여 데이터 처리가 취소되었습니다.
                                    }
                                Exit1:
                                    dbConn.Close();

                                    if (ERRCode == "OK")
                                    {
                                        MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        SearchExec();
                                    }
                                    else if (ERRCode == "ER")
                                    {
                                        MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    }
                                    else
                                    {
                                        MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    }

                                    return;
                                }
                                else
                                {
                                    Trans.Rollback();
                                    MessageBox.Show(SystemBase.Base.MessageRtn("B0040"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);//작업이 취소되었습니다.
                                    this.Cursor = Cursors.Default;
                                    return;
                                }
                            }

                            //행수만큼 처리
                            for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                            {

                                string strHead = fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text;
                                string strGbn = "";

                                if (strHead.Length > 0)
                                {
                                    switch (strHead)
                                    {
                                        case "U": strGbn = "U1"; break;
                                        case "I": strGbn = "I1"; break;
                                        case "D": strGbn = "D1"; break;
                                        default: strGbn = ""; break;
                                    }

                                    focusStr = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "시작일")].Text;
                                    string strSql = " usp_PBA103 '" + strGbn + "'";
                                    strSql += ", @pSCH_NUM = '" + fpSpread1.Sheets[0].Cells[i, 0].Value + "'";
                                    strSql += ", @pSCH_ID = '" + cboSch_id.SelectedValue.ToString() + "' ";
                                    strSql += ", @pPLANT_CD = '" + cboPlant_cd.SelectedValue.ToString() + "' ";
                                    strSql += ", @pRES_KIND = '" + cboRes_Kind.SelectedValue.ToString() + "' ";
                                    strSql += ", @pRES_ID = '" + cboRes_id.SelectedValue.ToString() + "' ";
                                    strSql += ", @pCON_DT = '" + dtmCon_dt.Text + "' ";
                                    strSql += ", @pST_DATE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "시작일")].Text + "' ";
                                    strSql += ", @pED_DATE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "종료일")].Text + "' ";
                                    strSql += ", @pSHIFT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "교대")].Text + "' ";
                                    strSql += ", @pST_TIME = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "START")].Value + "' ";
                                    strSql += ", @pED_TIME = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "END")].Value + "' ";
                                    strSql += ", @pCONTENTS = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "내용")].Text + "' ";
                                    strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                                    strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";

                                    DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                                    ERRCode = ds.Tables[0].Rows[0][0].ToString();
                                    MSGCode = ds.Tables[0].Rows[0][1].ToString();

                                    if (ERRCode == "OK")

                                    if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프					
                                }
                            }
                        }
                        else
                        {
                            Trans.Rollback();
                            this.Cursor = Cursors.Default;
                            return;
                        }
                        Trans.Commit();
                    }
                    catch (Exception e)
                    {
                        SystemBase.Loggers.Log(this.Name, e.ToString());
                        Trans.Rollback();
                        ERRCode = "ER";
                        MSGCode = e.Message;
                        //MSGCode = "P0001";	//에러가 발생되어 데이터 처리가 취소되었습니다.
                    }
                Exit:
                    dbConn.Close();
                    if (ERRCode == "OK")
                    {           
                        ActiveRow  = fpSpread2.ActiveSheet.ActiveRowIndex;
                        Grid2_Search();
                        SubSearch(SearchRow);
                        //UIForm.FPMake.GridSetFocus(fpSpread2, strSchKey); 
                        UIForm.FPMake.GridSetFocus(fpSpread1, focusStr); //저장 후 그리드 포커스 이동
                        MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else if (ERRCode == "ER")
                    {
                        MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    this.Cursor = Cursors.Default;
                }
            }
        }
        #endregion

        #region 삭제Row Count 체크
        private bool DelCheck()
        {
            bool delChk = true;
            int delCount = 0;

            for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
            {
                if (fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text == "D")
                {
                    delCount++;
                }
            }

            if (delCount == fpSpread1.Sheets[0].Rows.Count)
            { delChk = false; }

            return delChk;
        }
        #endregion

        #region 휴무일 일괄 선택
        private void btnCreate_Click(object sender, System.EventArgs e)
        {
            PBA103P2 frm = new PBA103P2(cboSch_id.SelectedValue.ToString(), fpSpread1);
            frm.ShowDialog();

            if (frm.DialogResult == DialogResult.OK)
            {
                string[] Msgs = frm.ReturnVal;
                if (Msgs == null)
                    return;
                cboSch_id.SelectedValue = Msgs[0].ToString();
                cboPlant_cd.SelectedValue = Msgs[1].ToString();
            }
        }
        #endregion

        #region DelExec() 삭제 로직
        protected override void DeleteExec()
        {
            DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0027"), SystemBase.Base.MessageRtn("Z0004"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dsMsg == DialogResult.Yes)
            {
                string ERRCode = "ER", MSGCode = "P0000";	//처리할 내용이 없습니다.
                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                try
                {
                    string strDelSql = " usp_PBA103  'D2'";
                    strDelSql += ", @pSCH_ID = '" + cboSch_id.SelectedValue.ToString() + "' ";
                    strDelSql += ", @pPLANT_CD = '" + cboPlant_cd.SelectedValue.ToString() + "' ";
                    strDelSql += ", @pRES_KIND = '" + cboRes_Kind.SelectedValue.ToString() + "' ";
                    strDelSql += ", @pRES_ID = '" + cboRes_id.SelectedValue.ToString() + "' ";
                    strDelSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    DataSet ds = SystemBase.DbOpen.TranDataSet(strDelSql, dbConn, Trans);
                    ERRCode = ds.Tables[0].Rows[0][0].ToString();
                    MSGCode = ds.Tables[0].Rows[0][1].ToString();

                    if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                    Trans.Commit();
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    Trans.Rollback();
                    MSGCode = "P0001";	//에러가 발생하여 데이터 처리가 취소되었습니다.
                }
            Exit:
                dbConn.Close();

                if (ERRCode == "OK")
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    SearchExec();
                }
                else if (ERRCode == "ER")
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }
        #endregion

    }
}