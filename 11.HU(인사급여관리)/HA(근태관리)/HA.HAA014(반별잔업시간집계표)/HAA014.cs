
#region 작성정보
/*********************************************************************/
// 단위업무명:  반별잔업시간집계표
// 작 성 자  :  한미애
// 작 성 일  :  2020-08-31
// 작성내용  :  작업장별 개인별 잔업시간을 조회한다.
// 수 정 일  :
// 수 정 자  :
// 수정내용  :
// 비    고  :
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

namespace HA.HAA014
{
    public partial class HAA014 : UIForm.FPCOMM1
    {
        int iYmColStartIdx = 5;         // 월별공수 시작위치
        int iYmColEndIdx = 52;          // 월별공수 종료위치

        #region 생성자
        public HAA014()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void HAA014_Load(object sender, System.EventArgs e)
        {
            //필수 적용
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            //dtpDate.Text = SystemBase.Base.ServerTime("YYYY") + "-01";
            //dtpDateTo.Text = SystemBase.Base.ServerTime("YYYY-MM");
            dtpDate.Value = SystemBase.Base.ServerTime("YYMMDD").Substring(0, 4) + "-01-01";
            dtpDateTo.Value = SystemBase.Base.ServerTime("YYMMDD");

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

            rdoWcPd.Checked = true;     // 작업장구분의 기본 선택값을 '생산'으로
            rdoOverTm.Checked = true;   // 조회구분의 기본 선택값을 '잔업'으로.
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            //필수 적용
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            //그리드 초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 8, false);
            //기타 셋팅
            dtpDate.Text = SystemBase.Base.ServerTime("YYMMDD");
            dtpDateTo.Text = SystemBase.Base.ServerTime("YYMMDD");
        }
        #endregion

        #region SearchExec()
        protected override void SearchExec()
        {
            //조회조건 필수 체크
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1) == true)
            {
                this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

                try
                {
                    //그리드 초기화				
                    UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 1);

                    string strQuery_H = " usp_HAA014  @pTYPE = 'S2' ";
                    strQuery_H += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    strQuery_H += ", @pSTART_YYMM = '" + dtpDate.Text.Replace("-", "") + "' ";
                    strQuery_H += ", @pEND_YYMM = '" + dtpDateTo.Text.Replace("-", "") + "' ";

                    DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery_H);

                    if (dt == null || dt.Rows.Count == 0)
                    {
                        MessageBox.Show("년월 데이터를 가져오지 못했습니다." + Environment.NewLine + "관리자에게 문의바랍니다.");
                        return;
                    }
                    else
                    {
                        string strWctype = "", strViewType = "";

                        if (rdoWcAll.Checked == true)
                            strWctype = "ALL";
                        else if (rdoWcPd.Checked == true)
                            strWctype = "PD";
                        else if (rdoWcQc.Checked == true)
                            strWctype = "QC";
                        else if (rdoWcPt.Checked == true)
                            strWctype = "PT";
                        else if (rdoWcRd.Checked == true)
                            strWctype = "RD";

                        if (rdoOverTm.Checked == true)
                            strViewType = "OVER";
                        else
                            strViewType = "ALL";

                        string strQuery = " usp_HAA014  @pTYPE = 'S1' ";
                        strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                        strQuery += ", @pSTART_YYMM = '" + dtpDate.Text.Replace("-", "") + "' ";
                        strQuery += ", @pEND_YYMM = '" + dtpDateTo.Text.Replace("-", "") + "' ";
                        strQuery += ", @pWC_TYPE = '" + strWctype + "' ";
                        strQuery += ", @pWC_CD = '" + txtWcCd.Text + "' ";
                        strQuery += ", @pVIEW_TYPE = '" + strViewType + "' ";

                        UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 8);

                        // Merge
                        fpSpread1.Sheets[0].SetColumnMerge(1, FarPoint.Win.Spread.Model.MergePolicy.Always);
                        fpSpread1.Sheets[0].SetColumnMerge(2, FarPoint.Win.Spread.Model.MergePolicy.Always);
                        fpSpread1.Sheets[0].SetColumnMerge(3, FarPoint.Win.Spread.Model.MergePolicy.Always);
                        fpSpread1.Sheets[0].SetColumnMerge(4, FarPoint.Win.Spread.Model.MergePolicy.Always);

                        if (fpSpread1.Sheets[0].RowCount > 0)
                        {
                            for (int i = 0; i < fpSpread1.Sheets[0].RowCount; i++)
                            {
                                if ((fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "작업장코드")].Text != "합계") &&
                                        (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "사원번호")].Text == ""))
                                {
                                    fpSpread1.Sheets[0].Rows[i].BackColor = Color.Beige;
                                }
                                else if ((fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "작업장코드")].Text == "합계") &&
                                            (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "사원번호")].Text == ""))
                                {
                                    fpSpread1.Sheets[0].Rows[i].BackColor = Color.Bisque;
                                }
                            }
                        }

                        // 컬럼 틀고정
                        fpSpread1.Sheets[0].FrozenColumnCount = 5;

                        string strThisMonth = "";
                        // 헤더명설정
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            strThisMonth = dt.Rows[i]["YYYYMM"].ToString();
                            fpSpread1.Sheets[0].ColumnHeader.Cells[0, i + iYmColStartIdx].Text = strThisMonth.Substring(2, 2) + "-" + strThisMonth.Substring(4, 2);
                            fpSpread1.Sheets[0].ColumnHeader.Cells[0, i + iYmColStartIdx].Tag = strThisMonth.Substring(2, 2) + "-" + strThisMonth.Substring(4, 2);
                            fpSpread1.Sheets[0].Columns[i].Visible = true;
                        }

                        // 컬럼헤더 숨김
                        for (int i = iYmColStartIdx + dt.Rows.Count; i <= iYmColEndIdx; i++)
                        {
                            fpSpread1.Sheets[0].Columns[i].Visible = false;
                        }

                        // 조회구분을 잔업으로 조회시 직접/간접 항목 보이지 않게 처리
                        if (rdoOverTm.Checked == true)
                        {
                            for (int i = iYmColStartIdx; i < fpSpread1.Sheets[0].Columns.Count; i=i+3)
                            {
                                fpSpread1.Sheets[0].Columns[i].Visible = false;
                                fpSpread1.Sheets[0].Columns[i+1].Visible = false;
                                fpSpread1.Sheets[0].Columns[i+2].Width = 100;
                            }
                        }
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                this.Cursor = System.Windows.Forms.Cursors.Default;
            }
        }
        #endregion

        #region 조회조건 팝업
        private void btnWcCd_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pTYPE = 'P042', @pLANG_CD = 'KOR', @pETC = 'P061', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";					// 쿼리
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };				// 쿼리 인자값(조회조건)
                string[] strSearch = new string[] { txtWcCd.Text, "" };						// 쿼리 인자값에 들어갈 데이타

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

        #region TextChanged
        //부서코드
        private void txtWcCd_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (txtWcCd.Text != "")
                {
                    txtWcNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtWcCd.Text, " AND MAJOR_CD = 'P061' AND COMP_CODE = '" + SystemBase.Base.gstrCOMCD + "' ");
                }
                else
                {
                    txtWcNm.Value = "";
                }
            }
            catch { }
        }
        #endregion

        private void fpSpread1_CellDoubleClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            if ((fpSpread1.Sheets[0].ColumnHeader.Cells[0, e.Column].Text.Trim() == "사원번호") ||
                (fpSpread1.Sheets[0].ColumnHeader.Cells[0, e.Column].Text.Trim() == "성명") )
            {
                string strStartYm = dtpDate.Text;
                string strEndYm = dtpDateTo.Text;
                string strEmpNo = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "사원번호")].Text.Trim();
                string strEmpNm = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "성명")].Text.Trim();
                string strWcCd = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "작업장코드")].Text.Trim();
                string strWcNm = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "작업장명")].Text.Trim();

                HAA014P1 form = new HAA014P1(strStartYm, strEndYm, strWcCd, strWcNm, strEmpNo, strEmpNm);

                form.ShowDialog();
            }
        }
    }
}
