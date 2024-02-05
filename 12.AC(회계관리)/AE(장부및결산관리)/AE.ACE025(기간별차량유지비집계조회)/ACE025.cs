
#region 작성정보
/*********************************************************************/
// 단위업무명 : 기간별차량유지비집계조회
// 작 성 자 : 한 미 애
// 작 성 일 : 2016-08-22
// 작성내용 : 
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

namespace AE.ACE025
{
    public partial class ACE025 : UIForm.FPCOMM1
    {
        string strREORG_ID = "";
        int jobColStartIdx = 2;
        int jobColEndIdx = 30;

        public ACE025()
        {
            InitializeComponent();
        }


        #region Form Load 시
        private void ACE025_Load(object sender, System.EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(groupBox1);  // 필수 적용
            SystemBase.ComboMake.C1Combo(cboBizAreaCd, "usp_B_COMMON @pTYPE ='BIZ', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9);      // 사업장 리스트
            dtpSlipDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD").ToString()).AddMonths(-1).ToShortDateString();
            dtpSlipDtTo.Text = SystemBase.Base.ServerTime("YYMMDD");
            strREORG_ID = SystemBase.Base.gstrREORG_ID;

            cboBizAreaCd.SelectedValue = SystemBase.Base.gstrBIZCD;     // 기본사업장 세팅

            //그리드 초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
            Form_Setting();
        }
        #endregion


        #region Form_Settging(): 헤더 세팅
        private void Form_Setting()
        {
            string strQuery = " usp_ACE025 @pTYPE = 'S1', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
            DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

            if (dt == null || dt.Rows.Count == 0)
            {
                MessageBox.Show("차량유지 계정 정보를 불러오지 못했습니다." + Environment.NewLine + "관리자에게 문의바랍니다.");
            }
            else
            {
                // 헤더높이
                fpSpread1.Sheets[0].ColumnHeader.Rows[0].Height = 28;
                fpSpread1.Sheets[0].ColumnHeader.Rows[1].Height = 28;
                int iVisibleCols = 0;

                // 헤더명설정
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    fpSpread1.Sheets[0].ColumnHeader.Cells[0, i + jobColStartIdx].Text = dt.Rows[i]["HDR_NM1"].ToString();
                    fpSpread1.Sheets[0].ColumnHeader.Cells[0, i + jobColStartIdx].Tag = dt.Rows[i]["HDR_NM1"].ToString();
                    fpSpread1.Sheets[0].ColumnHeader.Cells[1, i + jobColStartIdx].Text = dt.Rows[i]["HDR_NM2"].ToString();
                    fpSpread1.Sheets[0].ColumnHeader.Cells[1, i + jobColStartIdx].Tag = dt.Rows[i]["HDR_NM2"].ToString();
                    fpSpread1.Sheets[0].Columns[i].Visible = true;
                    iVisibleCols = i;
                }

                // 컬럼헤더 숨김
                for (int i = jobColStartIdx + dt.Rows.Count; i <= jobColEndIdx; i++)
                {
                    fpSpread1.Sheets[0].Columns[i].Visible = false;
                }

                // 헤더명 Merge 처리
                // 맨마지막 컬럼: 합계액
                fpSpread1.Sheets[0].ColumnHeader.Cells[0, iVisibleCols + jobColStartIdx].RowSpan = 2;
                // 상단헤더: 상위 계정명
                int iStartCols = 0;
                int iSpanCnt = 1;
                for (int j = 0; j < dt.Rows.Count; j++ )
                {
                    // 0보다 크고 이전 컬럼과 동일한 경우 Merge 컬럼수 증가시키고 Merge수 지정
                    if (j > 0 && 
                          (fpSpread1.Sheets[0].ColumnHeader.Cells[0, j + jobColStartIdx].Text == fpSpread1.Sheets[0].ColumnHeader.Cells[0, j + jobColStartIdx - 1].Text))
                    {
                        iSpanCnt = iSpanCnt + 1;
                        fpSpread1.Sheets[0].ColumnHeader.Cells[0, iStartCols + jobColStartIdx].ColumnSpan = iSpanCnt;
                    }
                    else
                    {
                        // Merge 시작위치를 현재 컬럼으로, Merge 컬럼수는 1로 Reset 처리
                        iStartCols = j;
                        iSpanCnt = 1;
                    }
                }
            }
        }
        #endregion



        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            dtpSlipDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD").ToString()).AddMonths(-1).ToShortDateString();
            dtpSlipDtTo.Text = SystemBase.Base.ServerTime("YYMMDD");

            strREORG_ID = SystemBase.Base.gstrREORG_ID;

            //UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
            Form_Setting();
        }
        #endregion


        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {
                    // 헤더 세팅

                    // 상세 내용
                    string strQuery = " usp_ACE025  'S2'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    strQuery += ", @pSLIP_DT_FROM = '" + dtpSlipDtFr.Text + "' ";
                    strQuery += ", @pSLIP_DT_TO = '" + dtpSlipDtTo.Text + "' ";

                    if (txtDeptCd.Text != "")
                    {
                        strQuery += ", @pREORG_ID = '" + strREORG_ID + "' ";
                        strQuery += ", @pDEPT_CD = '" + txtDeptCd.Text + "' ";
                    }

                    if (txtAcctCd.Text != "")
                    {
                        strQuery += ", @pACCT_CD = '" + txtAcctCd.Text + "' ";
                    }

                    if (txtCarCd.Text != "")
                    {
                        strQuery += ", @pCAR_CD = '" + txtCarCd.Text + "' ";
                    }

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0, true);
                    Form_Setting();

                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {
                        // 납품차량/개인차량 소계
                        if ((fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "차종")].Text == "납품차량") ||
                            (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "차종")].Text == "개인차량"))
                        {
                            fpSpread1.Sheets[0].Rows[i].BackColor = SystemBase.Base.gColor2;
                        }
                        else if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "차종")].Text == "합계")
                        {
                            fpSpread1.Sheets[0].Rows[i].BackColor = SystemBase.Base.gColor1;
                        }
                    }

                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString());
            }
            this.Cursor = Cursors.Default;
        }
        #endregion


        #region TextChanged() 이벤트 처리:  귀속부서,계정코드,차량번호 입력값 변경시 해당 코드에 대한 명세 가져와서 보여줌.
        // 귀속부서 
        private void txtDeptCd_TextChanged(object sender, EventArgs e)
        {
            try
            {
                txtDeptNm.Value = SystemBase.Base.CodeName("DEPT_CD", "DEPT_NM", "B_DEPT_INFO", txtDeptCd.Text, " AND REORG_ID = '" + strREORG_ID + "' AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 계정코드
        private void txtAcctCd_TextChanged(object sender, EventArgs e)
        {
            try
            {
                txtAcctNm.Value = SystemBase.Base.CodeName("ACCT_CD", "ACCT_NM", "A_ACCT_CODE", txtAcctCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "' AND ENTRY_YN = 'Y'");
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 차량번호 
        private void txtCarCd_TextChanged(object sender, EventArgs e)
        {
            try
            {
                DataTable dt = new DataTable();

                string strQuery = " usp_ACD001  'S3'";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                strQuery = strQuery + ", @pCTRL_CD = 'CA' ";                // 차량번호 관리항목
                strQuery = strQuery + ", @pCODE_CD1 ='" + txtCarCd.Text + "' ";

                dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

                if (dt.Rows.Count > 0)
                {
                    txtCarNm.Value = dt.Rows[0]["NAME"].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "차량번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion


        #region btnXXXXX_Click() 검색 버튼 Click 이벤트 처리:  해당 코드와 명세 팝업창 띄워줌. 귀속부서, 계정코드, 차량번호
        // 귀속부서 검색버튼 클릭 이벤트. 귀속부서 팝업창 띄우기 
        private void btnDept_Click(object sender, EventArgs e)
        {
            try
            {
                WNDW.WNDW011 pu = new WNDW.WNDW011();
                pu.MaximizeBox = false;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtDeptCd.Value = Msgs[1].ToString();
                    strREORG_ID = Msgs[5].ToString();
                    txtDeptCd.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "부서 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        // 계정코드 검색버튼 클릭 이벤트. 계정코드 팝업창 띄우기
        private void btnAcct_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = " usp_A_COMMON @pTYPE = 'A030', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' , @pSPEC1 = 'Y' ";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtAcctCd.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00110", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "계정코드 조회");
                pu.Width = 800;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());
                    txtAcctCd.Value = Msgs[0].ToString();
                    txtAcctNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 차량번호 검색버튼 클릭 이벤트. 차량번호 팝업창 띄우기(공통코드)
        private void btnCarCd_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = " usp_ACD001 @pType='P12', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "', @pCTRL_CD = '" + txtCarCd + "' ";
                string[] strWhere = new string[] { "@pCODE_CD1", "@pCODE_CD2" };
                string[] strSearch = new string[] { "A115", "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("ACD001_P12", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "차량번호 조회");
                pu.Width = 800;
                pu.Height = 800;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtCarCd.Text = Msgs[0].ToString();
                    txtCarCd.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "차량번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

    }
}
