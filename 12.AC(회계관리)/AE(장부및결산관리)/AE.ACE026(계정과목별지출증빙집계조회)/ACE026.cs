
#region 작성정보
/*********************************************************************/
// 단위업무명 : 계정과목별지출증빙집계표
// 작 성 자 : 한 미 애
// 작 성 일 : 2016-09-12
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

namespace AE.ACE026
{
    public partial class ACE026 : UIForm.FPCOMM1
    {
        string strREORG_ID = "";
        int jobColStartIdx = 3;
        int jobColEndIdx = 13;

        public ACE026()
        {
            InitializeComponent();
        }


        #region Form Load 시
        private void ACE026_Load(object sender, System.EventArgs e)
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
            string strQuery = " usp_ACE026 @pTYPE = 'S1', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
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

            if (txtEvCd.Text != "")
            {
                strQuery += ", @pEV_CD = '" + txtEvCd.Text + "' ";
            }

            DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

            if (dt == null || dt.Rows.Count == 0)
            {
                MessageBox.Show("지출증빙 정보를 불러오지 못했습니다." + Environment.NewLine + "관리자에게 문의바랍니다.");
            }
            else
            {
                // 헤더높이
                fpSpread1.Sheets[0].ColumnHeader.Rows[0].Height = 28;
                int iVisibleCols = 0;

                // 헤더명설정
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    fpSpread1.Sheets[0].ColumnHeader.Cells[0, i + jobColStartIdx].Text = dt.Rows[i]["EV_NM"].ToString();
                    fpSpread1.Sheets[0].ColumnHeader.Cells[0, i + jobColStartIdx].Tag = dt.Rows[i]["EV_NM"].ToString();
                    fpSpread1.Sheets[0].Columns[i].Visible = true;
                    iVisibleCols = i;
                }

                // 컬럼헤더 숨김
                for (int i = jobColStartIdx + dt.Rows.Count; i < jobColEndIdx; i++)
                {
                    fpSpread1.Sheets[0].Columns[i].Visible = false;
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
                     // 상세 내용
                    string strQuery = " usp_ACE026  'S2'";
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

                    if (txtEvCd.Text != "")
                    {
                        strQuery += ", @pEV_CD = '" + txtEvCd.Text + "' ";
                    }

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0, true);
                    Form_Setting();

                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {
                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "계정명")].Text.EndsWith(" 소계") == true)
                        {
                            fpSpread1.Sheets[0].Rows[i].BackColor = SystemBase.Base.gColor2;
                        }
                        else if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "계정명")].Text == "전체 합계") 
                        {
                            fpSpread1.Sheets[0].Rows[i].BackColor = SystemBase.Base.gColor1;
                        }
                    }
                    //fpSpread1.Sheets[0].ColumnHeader.Cells[0, iVisibleCols + jobColStartIdx].RowSpan = 2;
                    fpSpread1.ActiveSheet.Columns[1].MergePolicy = FarPoint.Win.Spread.Model.MergePolicy.Always;
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString());
            }
            this.Cursor = Cursors.Default;
        }
        #endregion


        #region TextChanged() 이벤트 처리:  발생부서,계정코드,지출증빙 입력값 변경시 해당 코드에 대한 명세 가져와서 보여줌.
        // 발생부서 
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

        // 지출증빙 
        private void txtEvCd_TextChanged(object sender, EventArgs e)
        {
            try
            {
                DataTable dt = new DataTable();

                string strQuery = " usp_ACD001  'S3'";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                strQuery = strQuery + ", @pCTRL_CD = 'EV' ";                // 지출증빙 관리항목
                strQuery = strQuery + ", @pCODE_CD1 ='" + txtEvCd.Text + "' ";

                dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

                if (dt.Rows.Count > 0)
                {
                    txtEvNm.Value = dt.Rows[0]["NAME"].ToString();
                }
                else
                {
                    txtEvNm.Value = "";
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "지출증빙 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion


        #region btnXXXXX_Click() 검색 버튼 Click 이벤트 처리:  해당 코드와 명세 팝업창 띄워줌. 귀속부서, 계정코드, 지출증빙
        // 발생부서 검색버튼 클릭 이벤트. 귀속부서 팝업창 띄우기 
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

        // 지출증빙 검색버튼 클릭 이벤트. 지출증빙 팝업창 띄우기(공통코드)
        private void btnCarCd_Click(object sender, EventArgs e)
        {
            try
            {
                string strEV_CD = txtEvCd.Text;

                string strQuery = " usp_B_COMMON @pType='COMM_POP', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pSPEC1 = 'A114' ";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtEvCd.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("ACD001_P11", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "지출증빙 조회");
                pu.Width = 800;
                pu.Height = 800;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtEvCd.Text = Msgs[0].ToString();
                    txtEvCd.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "지출증빙 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

    }
}
