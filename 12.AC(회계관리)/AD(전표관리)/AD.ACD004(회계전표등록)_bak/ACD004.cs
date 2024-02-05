

#region 작성정보
/*********************************************************************/
// 단위업무명 : 결의전표등록
// 작 성 자 : 유 재 규
// 작 성 일 : 2013-02-18
// 작성내용 : 결의전표등록
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
using WNDW;         // 2015.03.23. hma 추가

namespace AD.ACD004
{
    public partial class ACD004 : UIForm.FPCOMM1
    {
        #region 변수선언
        string SaveData = "", SearchData = ""; //컨트롤에 대한 조회후 데이터와 저장시 변경된 데이터 체크위한 변수
        string strNewFlag = "";
        string strREORG_ID = "";
        string strBIZ_CD = "";
        string strSubType = "";
        string strAcctType = "";
        int PreRow = -1;   // SelectionChanged 시에 동일 Row에서 데이타변환 처리 안하도록 하기 위함.
        //strCTRL_CD, strCTRL_NULL : 8개가 필요하지만 관리항목 번호에 맞게 0번은 쓰지 않고 1번부터 8번까지 사용
        string[] strCTRL_CD = new string[9];
        string[] strCTRL_NULL = new string[9];
        DataTable Ar_Dt; //채권
        DataTable Ar_Dt_Temp; //채권
        DataTable Ap_Dt; //채무
        DataTable Ap_Dt_Temp; //채무
        DataTable Loan_Dt; //차입금
        DataTable Loan_Dt_Temp; //차입금
        DataTable Asset_Dt; //고정자산
        DataTable Asset_Dt_Temp; //고정자산
        string strCH_CHK = "";
        bool strSaveChk = false;

        #endregion

        public ACD004()
        {
            InitializeComponent();
        }

        #region Form Load 시
        private void ACD004_Load(object sender, System.EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(groupBox1);//필수 적용
            SystemBase.Validation.GroupBox_Setting(groupBox2);//필수 적용
            SystemBase.Validation.GroupBox_Setting(groupBox3);//필수 적용
            SystemBase.Validation.GroupBox_Setting(groupBox4);//필수 적용

            SystemBase.ComboMake.C1Combo(cboSlipType, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'A113', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9);   //전표형태
            SystemBase.ComboMake.C1Combo(cboCreathPath, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'A101', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9);   //발생경로
            SystemBase.ComboMake.C1Combo(cboCurCd, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'Z003', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9);   //화폐단위

            POP_ENABLED();

            NewExec();
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            try
            {
                btnDept.Enabled = true;
                txtDeptCd.Enabled = true;
                dtpSlipDt.Enabled = true;
                cboSlipType.Enabled = true;

                SystemBase.Validation.GroupBox_Reset(groupBox1);
                SystemBase.Validation.GroupBox_Reset(groupBox2);

                txtDeptCd.Text = SystemBase.Base.gstrDEPT;

                dtpSlipDt.Text = SystemBase.Base.ServerTime("YYMMDD");
                cboCreathPath.SelectedValue = "GL";
                cboSlipType.SelectedValue = "03";

                ETC_TABLE_SET();
                Detail_New();
                txtInputDeptCd.Value = SystemBase.Base.gstrDEPT;
                SystemBase.Validation.GroupBoxControlsLock(groupBox3, false);

                GroupBox[] gBox = new GroupBox[] { groupBox2 };
                SystemBase.Validation.Control_Check(gBox, ref SearchData);

                UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
                txtDrAmt.Value = 0;
                txtCrAmt.Value = 0;
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 전표상세 초기화
        protected void Detail_New()
        {
            try
            {
                string strDeptCd_Temp = txtInputDeptCd.Text;
                strSaveChk = false;
                SystemBase.Validation.GroupBoxControlsLock(groupBox3, false);
                strNewFlag = "Y";
                SystemBase.Validation.GroupBox_Reset(groupBox3);
                strNewFlag = "N";
                txtInputDeptCd.Value = strDeptCd_Temp;
                strBIZ_CD = SystemBase.Base.gstrBIZCD;
                cboCurCd.SelectedValue = "KRW";
                txtExch_Rate.Value = 1;
                txtSlipAmt.Text = "";

                txtAcctCd.Tag = "계정코드;1;;";
                btnAcct.Tag = ";;;";

                for (int i = 1; i < strCTRL_CD.Length; i++)
                {
                    strCTRL_CD[i] = "";
                    strCTRL_NULL[i] = "";
                }

                Ctrl_Color_Chk();
                PreRow = -1;

                if (cboSlipType.SelectedValue.ToString() == "01")
                {
                    optCr.Checked = true;
                    optCr.Enabled = false;
                    optDr.Enabled = false;
                }
                else if (cboSlipType.SelectedValue.ToString() == "02")
                {
                    optDr.Checked = true;
                    optCr.Enabled = false;
                    optDr.Enabled = false;
                }
                else
                {
                    optCr.Enabled = true;
                    optDr.Enabled = true;
                }

                if (Ar_Dt_Temp == null) { Ar_Dt_Temp = Ar_Dt.Clone(); Ar_Dt_Temp.Clear(); } else Ar_Dt_Temp.Clear(); //채권
                if (Ap_Dt_Temp == null) { Ap_Dt_Temp = Ap_Dt.Clone(); Ap_Dt_Temp.Clear(); } else Ap_Dt_Temp.Clear(); //채무
                if (Loan_Dt_Temp == null) { Loan_Dt_Temp = Loan_Dt.Clone(); Loan_Dt_Temp.Clear(); } else Loan_Dt_Temp.Clear(); //차입금
                if (Asset_Dt_Temp == null) { Asset_Dt_Temp = Asset_Dt.Clone(); Asset_Dt_Temp.Clear(); } else Asset_Dt_Temp.Clear(); //고정자산

                if (cboCreathPath.SelectedValue.ToString() == "TG" && txtConfirm_YN.Text == "승인")
                {
                    SystemBase.Validation.GroupBoxControlsLock(groupBox3, true);
                }
                else
                {
                    SystemBase.Validation.GroupBoxControlsLock(groupBox3, false);
                }
                SLIP_AMT_SUM();
                txtAcctCd.Focus();
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
                    string strSSlipNo = txtSSlipNo.Text;
                    fpSpread1.Sheets[0].Rows.Count = 0;
                    SEARCH_SLIP(txtSSlipNo.Text);

                    string strQuery = " usp_ACD001  'S2'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    strQuery += ", @pSLIP_NO = '" + txtSSlipNo.Text + "' ";
                    strQuery += ", @pSLIP_DIV = 'G' ";
                    strQuery += ", @pUP_EMP_NO = '" + SystemBase.Base.gstrUserID + "'";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0, true);
                    Detail_New();
                    ETC_TABLE_SET();
                    if (strSSlipNo != txtSSlipNo.Text) txtSSlipNo.Text = strSSlipNo;
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            this.Cursor = Cursors.Default;
        }

        private void SEARCH_SLIP(string SLIP_NO)
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                //상세조회 SQL
                string strQuery = " usp_ACD001  'S1'";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                strQuery = strQuery + ", @pSLIP_NO ='" + SLIP_NO + "' ";
                strQuery += ", @pSLIP_DIV = 'G' ";
                strQuery += ", @pUP_EMP_NO = '" + SystemBase.Base.gstrUserID + "'";

                DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);
                if (dt.Rows.Count > 0)
                {
                    txtSlipNo.Value = dt.Rows[0]["SLIP_NO"].ToString();
                    dtpSlipDt.Value = dt.Rows[0]["SLIP_RES_DT"].ToString();
                    txtConfirm_YN.Value = dt.Rows[0]["CONFIRM_YN"].ToString();
                    txtDeptCd.Value = dt.Rows[0]["DEPT_CD"].ToString();
                    cboSlipType.SelectedValue = dt.Rows[0]["SLIP_TYPE"].ToString();
                    cboCreathPath.SelectedValue = dt.Rows[0]["CREATE_PATH"].ToString();
                    txtRemark.Value = dt.Rows[0]["REMARK"].ToString();

                    //Detail_New();
                    //Ctrl_Color_Chk();
                    //Ctrl_Readonly_Chk();

                    //Set_Sum_Row();

                    dtpSlipDt.Enabled = false;
                    txtDeptCd.Enabled = false;
                    btnDept.Enabled = false;
                    cboSlipType.Enabled = false;

                    if (cboCreathPath.SelectedValue.ToString() != "GL" || txtConfirm_YN.Text == "승인")
                    {
                        SystemBase.Validation.GroupBoxControlsLock(groupBox3, true);
                    }
                    else
                    {
                        SystemBase.Validation.GroupBoxControlsLock(groupBox3, false);
                    }
                }
                else
                {
                    NewExec();
                }
                SearchData = "";
                //컨트롤 체크 함수
                GroupBox[] gBox = new GroupBox[] { groupBox2 };
                SystemBase.Validation.Control_Check(gBox, ref SearchData);
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            this.Cursor = Cursors.Default;
        }

        protected void SEARCH_SLIP_DETAIL(int Row)
        {
            try
            {
                txtSeq.Value = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "순번")].Text;
                txtInputDeptCd.Value = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "귀속부서")].Text;
                txtAcctCd.Value = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "계정코드")].Text;

                txtSlipAmt.Value = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "전표금액")].Text.Replace(",", "");
                cboCurCd.SelectedValue = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "화폐단위")].Text.Replace(",", "");
                txtExch_Rate.Value = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "환율")].Text.Replace(",", "");
                txtSlipAmtLoc.Value = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "전표금액(자국)")].Text.Replace(",", "");

                if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "차대구분")].Text == "DR")
                    optDr.Checked = true;
                else optCr.Checked = true;

                txtRemark2.Value = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "적요")].Text;
                txtCTRL_VAL1.Value = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "관리항목1")].Text;
                txtCTRL_VAL_NM1.Value = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "관리항목1명")].Text;
                txtCTRL_VAL2.Value = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "관리항목2")].Text;
                txtCTRL_VAL_NM2.Value = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "관리항목2명")].Text;
                txtCTRL_VAL3.Value = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "관리항목3")].Text;
                txtCTRL_VAL_NM3.Value = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "관리항목3명")].Text;
                txtCTRL_VAL4.Value = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "관리항목4")].Text;
                txtCTRL_VAL_NM4.Value = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "관리항목4명")].Text;
                txtCTRL_VAL5.Value = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "관리항목5")].Text;
                txtCTRL_VAL_NM5.Value = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "관리항목5명")].Text;
                txtCTRL_VAL6.Value = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "관리항목6")].Text;
                txtCTRL_VAL_NM6.Value = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "관리항목6명")].Text;
                txtCTRL_VAL7.Value = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "관리항목7")].Text;
                txtCTRL_VAL_NM7.Value = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "관리항목7명")].Text;
                txtCTRL_VAL8.Value = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "관리항목8")].Text;
                txtCTRL_VAL_NM8.Value = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "관리항목8명")].Text;
                if (fpSpread1.Sheets[0].RowHeader.Cells[Row, 0].Text != "I")
                {
                    txtAcctCd.Tag = "계정코드;2;;";
                    btnAcct.Tag = ";2;;";
                }
                else
                {
                    txtAcctCd.Tag = "계정코드;1;;";
                    btnAcct.Tag = ";;;";
                }
                txtInputDeptCd.Focus();
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region SaveExec() 폼에 입력된 데이타 저장 로직
        protected override void SaveExec()
        {
            this.Cursor = Cursors.WaitCursor;

            if (fpSpread1.Sheets[0].Rows.Count == 0)
            {
                MessageBox.Show("저장할 정보가 없습니다.", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.Cursor = Cursors.Default;
                return;
            }

            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox2))
            {
                int iDel_cnt = 0;
                int iTotal_cnt = 0;
                for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                {
                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "계정코드")].Text != "11110001" || cboSlipType.SelectedValue.ToString() == "03")
                    {
                        iTotal_cnt++;
                    }
                    string strHead = fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text;
                    if (strHead == "D")
                    {
                        iDel_cnt++;
                    }
                }
                if (iTotal_cnt > 0 && iTotal_cnt == iDel_cnt)
                {
                    MessageBox.Show("전체삭제시 전체삭제 버튼을 사용하세요.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Cursor = Cursors.Default;
                    return;
                }

                //컨트롤 체크값 초기화
                SaveData = "";
                //컨트롤 체크 함수
                GroupBox[] gBox = new GroupBox[] { groupBox2 };
                SystemBase.Validation.Control_Check(gBox, ref SaveData);

                //기존 컨트롤 데이터와 현재 컨트롤 데이터 비교
                if (SearchData == SaveData)
                {
                    if ((SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", true) == false))// 그리드 필수항목 체크 
                    {
                        this.Cursor = Cursors.Default;
                        return;
                    }
                }
                Detail_New();
                for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                {
                    strSaveChk = true;
                    if (fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text == "U")
                    {
                        strSaveChk = false;
                    }
                    if (fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text != "D" && fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "AUTO_YN")].Text != "Y")
                    {
                        fpSpread1_SelectionChanged_Event(i);
                        if (CONFIRM_EVENT() == false) return;
                    }
                }
                strSaveChk = false;
                
                string ERRCode = "ER", MSGCode = "SY001";	//처리할 내용이 없습니다.
                string strCtrlcd = "";
                string strSLIPNO = txtSlipNo.Text;

                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                try
                {
                    double DR_AMT = 0;
                    double CR_AMT = 0;

                    double DR_AMT_LOC = 0;
                    double CR_AMT_LOC = 0;
                    //string strCurCd_Krw_YN = "";

                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {
                        if (fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text != "D")
                        {
                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "차대구분")].Text == "DR")
                            {
                                if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "전표금액")].Text != "")
                                    DR_AMT += Convert.ToDouble(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "전표금액")].Text.Replace(",", ""));
                                if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "전표금액(자국)")].Text != "")
                                    DR_AMT_LOC += Convert.ToDouble(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "전표금액(자국)")].Text.Replace(",", ""));
                            }
                            else
                            {
                                if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "전표금액")].Text != "")
                                    CR_AMT += Convert.ToDouble(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "전표금액")].Text.Replace(",", ""));
                                if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "전표금액(자국)")].Text != "")
                                    CR_AMT_LOC += Convert.ToDouble(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "전표금액(자국)")].Text.Replace(",", ""));
                            }

                            //if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "화폐단위")].Text != "KRW")
                            //{
                            //    strCurCd_Krw_YN = "Y";
                            //}
                        }
                    }
                    if (cboSlipType.SelectedValue.ToString() == "03")
                    {
                        if (DR_AMT != CR_AMT || DR_AMT_LOC != CR_AMT_LOC)
                        {
                            //if (strCurCd_Krw_YN == "Y" && DR_AMT == CR_AMT)
                            //{
                            //    strCH_CHK = "Y";
                            //}

                            if (Ar_Dt != null)
                            {
                                for (int i = 0; i < Ar_Dt.Rows.Count; i++)
                                {
                                    if (Ar_Dt.Rows[i].RowState.ToString() != "Deleted")
                                    {
                                        if (Ar_Dt.Rows[i]["CLS_AMT"].ToString() != "" && Ar_Dt.Rows[i]["CLS_AMT"].ToString() != "0" &&
                                            Ar_Dt.Rows[i]["DC_AMT"].ToString() != "" && Ar_Dt.Rows[i]["DC_AMT"].ToString() != "0")
                                        {
                                            strCH_CHK = "Y";
                                        }
                                    }
                                }
                            }
                            if (Ap_Dt != null)
                            {
                                for (int i = 0; i < Ap_Dt.Rows.Count; i++)
                                {
                                    if (Ap_Dt.Rows[i].RowState.ToString() != "Deleted")
                                    {
                                        if (Ap_Dt.Rows[i]["CLS_AMT"].ToString() != "" && Ap_Dt.Rows[i]["CLS_AMT"].ToString() != "0" &&
                                            Ap_Dt.Rows[i]["DC_AMT"].ToString() != "" && Ap_Dt.Rows[i]["DC_AMT"].ToString() != "0")
                                        {
                                            strCH_CHK = "Y";
                                        }
                                    }
                                }
                            }
                            //유콘 기초자산 업테이트를 위해서...(차후 txtSlipNo.Text != "GL201303310001" 부분 삭제 및 usp_A_SLIP_SUB 에도 삭제
                            if (strCH_CHK != "Y" && txtSlipNo.Text != "GL201303310001")
                            {
                                ERRCode = "ER";
                                MSGCode = "차/대변 금액이 일치하지 않습니다.";
                                Trans.Rollback(); goto Exit;
                            }
                        }
                    }
                    if (DR_AMT < CR_AMT)
                    {
                        DR_AMT = CR_AMT;
                        DR_AMT_LOC = CR_AMT_LOC;
                    }
                    if (txtDeptNm.Text == "")
                    {
                        ERRCode = "ER";
                        MSGCode = "차없는 발생부서입니다.";
                        txtDeptCd.Focus();
                        Trans.Rollback(); goto Exit;
                    }

                    ERRCode = "ER";

                    string strSql = " usp_ACD001 ";

                    if (txtSlipNo.Text == "")
                    {
                        strSql = strSql + " 'I1'";
                    }
                    else
                    {
                        strSql = strSql + " 'U1'";
                    }

                    strSql = strSql + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                    strSql = strSql + ", @pSLIP_NO = '" + txtSlipNo.Text.ToUpper().Trim() + "'";
                    strSql = strSql + ", @pSLIP_DT = '" + dtpSlipDt.Text + "'";
                    strSql = strSql + ", @pDEPT_CD = '" + txtDeptCd.Text + "'";
                    strSql = strSql + ", @pREORG_ID = '" + strREORG_ID + "'";
                    strSql = strSql + ", @pSLIP_TYPE = '" + cboSlipType.SelectedValue.ToString() + "'";
                    strSql = strSql + ", @pDR_AMT = '" + DR_AMT.ToString() + "'";
                    strSql = strSql + ", @pDR_AMT_LOC = '" + DR_AMT_LOC.ToString() + "'";
                    strSql = strSql + ", @pCREATE_PATH = '" + cboCreathPath.SelectedValue.ToString() + "'";
                    strSql = strSql + ", @pREMARK = '" + txtRemark.Text + "'";
                    strSql = strSql + ", @pSLIP_DIV = 'G'";
                    strSql = strSql + ", @pUP_EMP_NO = '" + SystemBase.Base.gstrUserID + "'";
                    strSql = strSql + ", @pUP_IP = '" + SystemBase.Base.gstrUserIp + "'";

                    DataSet ds2 = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                    ERRCode = ds2.Tables[0].Rows[0][0].ToString();
                    MSGCode = ds2.Tables[0].Rows[0][1].ToString();
                    strSLIPNO = ds2.Tables[0].Rows[0][2].ToString();



                    if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                    //채권반제정보 저장
                    if (Ar_Dt != null)
                    {
                        int iFirst_Check = 1;
                        for (int i = 0; i < Ar_Dt.Rows.Count; i++)
                        {
                            if (Ar_Dt.Rows[i].RowState.ToString() != "Deleted")
                            {
                                ERRCode = "ER";
                                strSql = " usp_ACD001 'I3'";
                                strSql = strSql + ", @pCO_CD  = '" + SystemBase.Base.gstrCOMCD + "'";
                                strSql = strSql + ", @pSLIP_NO = '" + strSLIPNO + "' ";
                                strSql = strSql + ", @pSLIP_SEQ = '" + Ar_Dt.Rows[i]["SLIP_SEQ"].ToString() + "' ";
                                strSql = strSql + ", @pFIRST_CHECK = '" + (iFirst_Check).ToString() + "' ";
                                strSql = strSql + ", @pAR_NO = '" + Ar_Dt.Rows[i]["AR_NO"].ToString() + "' ";
                                strSql = strSql + ", @pSLIP_DT = '" + dtpSlipDt.Text + "' ";
                                strSql = strSql + ", @pCLS_AMT = '" + Ar_Dt.Rows[i]["CLS_AMT"].ToString() + "' ";
                                strSql = strSql + ", @pCLS_AMT_LOC = '" + Ar_Dt.Rows[i]["CLS_AMT_LOC"].ToString() + "' ";
                                if(Ar_Dt.Rows[i]["DC_AMT"].ToString() != "") strSql = strSql + ", @pDC_AMT = '" + Ar_Dt.Rows[i]["DC_AMT"].ToString() + "' ";
                                if (Ar_Dt.Rows[i]["DC_AMT_LOC"].ToString() != "") strSql = strSql + ", @pDC_AMT_LOC = '" + Ar_Dt.Rows[i]["DC_AMT_LOC"].ToString() + "' ";
                                strSql = strSql + ", @pDC_ACCT_CD = '" + Ar_Dt.Rows[i]["DC_ACCT_CD"].ToString() + "' ";
                                strSql = strSql + ", @pREMARK = '" + Ar_Dt.Rows[i]["REMARK"].ToString() + "' ";

                                strSql = strSql + ", @pUP_EMP_NO = '" + SystemBase.Base.gstrUserID + "'";
                                strSql = strSql + ", @pUP_IP = '" + SystemBase.Base.gstrUserIp + "'";

                                DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                                ERRCode = ds.Tables[0].Rows[0][0].ToString();
                                MSGCode = ds.Tables[0].Rows[0][1].ToString();

                                iFirst_Check++;

                                if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
                            }
                        }
                        if (iFirst_Check == 1 && Ar_Dt.Rows.Count > 0)
                        {
                            ERRCode = "ER";
                            strSql = " usp_ACD001 'D3'";
                            strSql = strSql + ", @pCO_CD  = '" + SystemBase.Base.gstrCOMCD + "'";
                            strSql = strSql + ", @pSLIP_NO = '" + strSLIPNO + "' ";

                            strSql = strSql + ", @pUP_EMP_NO = '" + SystemBase.Base.gstrUserID + "'";
                            strSql = strSql + ", @pUP_IP = '" + SystemBase.Base.gstrUserIp + "'";

                            DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                            ERRCode = ds.Tables[0].Rows[0][0].ToString();
                            MSGCode = ds.Tables[0].Rows[0][1].ToString();

                            if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
                        }
                    }

                    //채무반제정보 저장
                    if (Ap_Dt != null)
                    {
                        int iFirst_Check = 1;
                        for (int i = 0; i < Ap_Dt.Rows.Count; i++)
                        {
                            if (Ap_Dt.Rows[i].RowState.ToString() != "Deleted")
                            {
                                ERRCode = "ER";
                                strSql = " usp_ACD001 'I4'";
                                strSql = strSql + ", @pCO_CD  = '" + SystemBase.Base.gstrCOMCD + "'";
                                strSql = strSql + ", @pSLIP_NO = '" + strSLIPNO + "' ";
                                strSql = strSql + ", @pSLIP_SEQ = '" + Ap_Dt.Rows[i]["SLIP_SEQ"].ToString() + "' ";
                                strSql = strSql + ", @pFIRST_CHECK = '" + (iFirst_Check).ToString() + "' ";
                                strSql = strSql + ", @pAP_NO = '" + Ap_Dt.Rows[i]["AP_NO"].ToString() + "' ";
                                strSql = strSql + ", @pSLIP_DT = '" + dtpSlipDt.Text + "' ";
                                strSql = strSql + ", @pCLS_AMT = '" + Ap_Dt.Rows[i]["CLS_AMT"].ToString() + "' ";
                                strSql = strSql + ", @pCLS_AMT_LOC = '" + Ap_Dt.Rows[i]["CLS_AMT_LOC"].ToString() + "' ";
                                if (Ap_Dt.Rows[i]["DC_AMT"].ToString() != "") strSql = strSql + ", @pDC_AMT = '" + Ap_Dt.Rows[i]["DC_AMT"].ToString() + "' ";
                                if (Ap_Dt.Rows[i]["DC_AMT_LOC"].ToString() != "") strSql = strSql + ", @pDC_AMT_LOC = '" + Ap_Dt.Rows[i]["DC_AMT_LOC"].ToString() + "' ";
                                strSql = strSql + ", @pDC_ACCT_CD = '" + Ap_Dt.Rows[i]["DC_ACCT_CD"].ToString() + "' ";
                                strSql = strSql + ", @pREMARK = '" + Ap_Dt.Rows[i]["REMARK"].ToString() + "' ";

                                strSql = strSql + ", @pUP_EMP_NO = '" + SystemBase.Base.gstrUserID + "'";
                                strSql = strSql + ", @pUP_IP = '" + SystemBase.Base.gstrUserIp + "'";

                                DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                                ERRCode = ds.Tables[0].Rows[0][0].ToString();
                                MSGCode = ds.Tables[0].Rows[0][1].ToString();

                                iFirst_Check++;

                                if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
                            }
                        }
                        if (iFirst_Check == 1 && Ap_Dt.Rows.Count > 0)
                        {
                            ERRCode = "ER";
                            strSql = " usp_ACD001 'D4'";
                            strSql = strSql + ", @pCO_CD  = '" + SystemBase.Base.gstrCOMCD + "'";
                            strSql = strSql + ", @pSLIP_NO = '" + strSLIPNO + "' ";

                            strSql = strSql + ", @pUP_EMP_NO = '" + SystemBase.Base.gstrUserID + "'";
                            strSql = strSql + ", @pUP_IP = '" + SystemBase.Base.gstrUserIp + "'";

                            DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                            ERRCode = ds.Tables[0].Rows[0][0].ToString();
                            MSGCode = ds.Tables[0].Rows[0][1].ToString();

                            if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
                        }
                    }

                    //차입금정보 저장
                    if (Loan_Dt != null)
                    {
                        int iFirst_Check = 1;
                        for (int i = 0; i < Loan_Dt.Rows.Count; i++)
                        {
                            if (Loan_Dt.Rows[i].RowState.ToString() != "Deleted")
                            {
                                ERRCode = "ER";
                                strSql = " usp_ACD001 'I5'";
                                strSql = strSql + ", @pCO_CD  = '" + SystemBase.Base.gstrCOMCD + "'";
                                strSql = strSql + ", @pSLIP_NO = '" + strSLIPNO + "' ";
                                strSql = strSql + ", @pSLIP_SEQ = '" + Loan_Dt.Rows[i]["SLIP_SEQ"].ToString() + "' ";
                                strSql = strSql + ", @pFIRST_CHECK = '" + (iFirst_Check).ToString() + "' ";
                                strSql = strSql + ", @pLOAN_NO = '" + Loan_Dt.Rows[i]["LOAN_NO"].ToString() + "' ";
                                strSql = strSql + ", @pLOAN_NM = '" + Loan_Dt.Rows[i]["LOAN_NM"].ToString() + "' ";
                                strSql = strSql + ", @pLOAN_DIV = '" + Loan_Dt.Rows[i]["LOAN_DIV"].ToString() + "' ";
                                strSql = strSql + ", @pLOAN_TYPE = '" + Loan_Dt.Rows[i]["LOAN_TYPE"].ToString() + "' ";
                                strSql = strSql + ", @pLOAN_BANK_CUST_CD = '" + Loan_Dt.Rows[i]["LOAN_BANK_CUST_CD"].ToString() + "' ";
                                strSql = strSql + ", @pLOAN_USE_CD = '" + Loan_Dt.Rows[i]["LOAN_USE_CD"].ToString() + "' ";
                                strSql = strSql + ", @pOPEN_DT = '" + dtpSlipDt.Text + "' ";
                                strSql = strSql + ", @pEXP_DT = '" + Loan_Dt.Rows[i]["EXP_DT"].ToString() + "' ";
                                strSql = strSql + ", @pCUR_CD = '" + Loan_Dt.Rows[i]["CUR_CD"].ToString() + "' ";
                                strSql = strSql + ", @pLOAN_AMT = '" + Loan_Dt.Rows[i]["LOAN_AMT"].ToString() + "' ";
                                strSql = strSql + ", @pEXCH_RATE = '" + Loan_Dt.Rows[i]["EXCH_RATE"].ToString() + "' ";
                                strSql = strSql + ", @pLOAN_AMT_LOC = '" + Loan_Dt.Rows[i]["LOAN_AMT_LOC"].ToString() + "' ";
                                strSql = strSql + ", @pDEFER_TERM = '" + Loan_Dt.Rows[i]["DEFER_TERM"].ToString() + "' ";
                                strSql = strSql + ", @pREPAYMENT_METHOD = '" + Loan_Dt.Rows[i]["REPAYMENT_METHOD"].ToString() + "' ";
                                strSql = strSql + ", @pREPAYMENT_CYCLE = '" + Loan_Dt.Rows[i]["REPAYMENT_CYCLE"].ToString() + "' ";
                                if (Loan_Dt.Rows[i]["FIRST_C_REPAYMENT_DT"].ToString() != "")
                                    strSql = strSql + ", @pFIRST_C_REPAYMENT_DT = '" + Loan_Dt.Rows[i]["FIRST_C_REPAYMENT_DT"].ToString() + "' ";
                                strSql = strSql + ", @pINTEREST_PAYMENT_TYPE = '" + Loan_Dt.Rows[i]["INTEREST_PAYMENT_TYPE"].ToString() + "' ";
                                if (Loan_Dt.Rows[i]["FIRST_I_REPAYMENT_DT"].ToString() != "")
                                    strSql = strSql + ", @pFIRST_I_REPAYMENT_DT = '" + Loan_Dt.Rows[i]["FIRST_I_REPAYMENT_DT"].ToString() + "' ";
                                strSql = strSql + ", @pINTEREST_RATE_CHANGE = '" + Loan_Dt.Rows[i]["INTEREST_RATE_CHANGE"].ToString() + "' ";
                                strSql = strSql + ", @pLOAN_INTEREST_RATE = '" + Loan_Dt.Rows[i]["LOAN_INTEREST_RATE"].ToString() + "' ";
                                strSql = strSql + ", @pREMARK = '" + Loan_Dt.Rows[i]["REMARK"].ToString() + "' ";
                                strSql = strSql + ", @pUP_EMP_NO = '" + SystemBase.Base.gstrUserID + "'";
                                strSql = strSql + ", @pUP_IP = '" + SystemBase.Base.gstrUserIp + "'";

                                DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                                ERRCode = ds.Tables[0].Rows[0][0].ToString();
                                MSGCode = ds.Tables[0].Rows[0][1].ToString();

                                iFirst_Check++;

                                if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
                            }
                        }
                        if (iFirst_Check == 1 && Ap_Dt.Rows.Count > 0)
                        {
                            ERRCode = "ER";
                            strSql = " usp_ACD001 'D5'";
                            strSql = strSql + ", @pCO_CD  = '" + SystemBase.Base.gstrCOMCD + "'";
                            strSql = strSql + ", @pSLIP_NO = '" + strSLIPNO + "' ";

                            strSql = strSql + ", @pUP_EMP_NO = '" + SystemBase.Base.gstrUserID + "'";
                            strSql = strSql + ", @pUP_IP = '" + SystemBase.Base.gstrUserIp + "'";

                            DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                            ERRCode = ds.Tables[0].Rows[0][0].ToString();
                            MSGCode = ds.Tables[0].Rows[0][1].ToString();

                            if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
                        }
                    }

                    //고정자산정보 저장
                    if (Asset_Dt != null)
                    {
                        int iFirst_Check = 1;
                        for (int i = 0; i < Asset_Dt.Rows.Count; i++)
                        {
                            if (Asset_Dt.Rows[i].RowState.ToString() != "Deleted")
                            {
                                ERRCode = "ER";
                                strSql = " usp_ACD001 'I6'";
                                strSql = strSql + ", @pCO_CD  = '" + SystemBase.Base.gstrCOMCD + "'";
                                strSql = strSql + ", @pSLIP_NO = '" + strSLIPNO + "' ";
                                strSql = strSql + ", @pSLIP_SEQ = '" + Asset_Dt.Rows[i]["SLIP_SEQ"].ToString() + "' ";
                                strSql = strSql + ", @pFIRST_CHECK = '" + (iFirst_Check).ToString() + "' ";
                                strSql = strSql + ", @pASSET_NO = '" + Asset_Dt.Rows[i]["ASSET_NO"].ToString() + "' ";
                                strSql = strSql + ", @pASSET_NM = '" + Asset_Dt.Rows[i]["ASSET_NM"].ToString() + "' ";
                                strSql = strSql + ", @pREORG_ID = '" + Asset_Dt.Rows[i]["REORG_ID"].ToString() + "' ";
                                strSql = strSql + ", @pDEPT_CD = '" + Asset_Dt.Rows[i]["DEPT_CD"].ToString() + "' ";
                                strSql = strSql + ", @pACCT_CD = '" + Asset_Dt.Rows[i]["ACCT_CD"].ToString() + "' ";
                                strSql = strSql + ", @pSLIP_DT = '" + dtpSlipDt.Text + "' ";
                                strSql = strSql + ", @pDEPR_ACCT_CD = '" + Asset_Dt.Rows[i]["DEPR_ACCT_CD"].ToString() + "' ";
                                strSql = strSql + ", @pCUR_CD = '" + cboCurCd.SelectedValue.ToString() + "' ";
                                strSql = strSql + ", @pASSET_AMT = '" + Asset_Dt.Rows[i]["ASSET_AMT"].ToString() + "' ";
                                strSql = strSql + ", @pASSET_AMT_LOC = '" + Asset_Dt.Rows[i]["ASSET_AMT_LOC"].ToString() + "' ";
                                strSql = strSql + ", @pACQ_QTY = '" + Asset_Dt.Rows[i]["ACQ_QTY"].ToString() + "' ";
                                strSql = strSql + ", @pDEPR_METHOD = '" + Asset_Dt.Rows[i]["DEPR_METHOD"].ToString() + "' ";
                                if (Asset_Dt.Rows[i]["MATTER_YEAR"].ToString() != "")
                                    strSql = strSql + ", @pMATTER_YEAR = '" + Asset_Dt.Rows[i]["MATTER_YEAR"].ToString() + "' ";
                                strSql = strSql + ", @pSURVIVAL_AMT = '" + Asset_Dt.Rows[i]["SURVIVAL_AMT"].ToString() + "' ";
                                strSql = strSql + ", @pEXCLU_YN = '" + Asset_Dt.Rows[i]["EXCLU_YN"].ToString() + "' ";
                                //strSql = strSql + ", @pEXCLU_ENT_CD = '" + Asset_Dt.Rows[i]["EXCLU_ENT_CD"].ToString() + "' ";
                                //strSql = strSql + ", @pEXCLU_NIIN = '" + Asset_Dt.Rows[i]["EXCLU_NIIN"].ToString() + "' ";
                                strSql = strSql + ", @pEXCLU_ENT_NM = '" + Asset_Dt.Rows[i]["EXCLU_ENT_NM"].ToString() + "' ";
                                strSql = strSql + ", @pITEM_CD = '" + Asset_Dt.Rows[i]["ITEM_CD"].ToString() + "' ";
                                strSql = strSql + ", @pREMARK = '" + Asset_Dt.Rows[i]["REMARK"].ToString() + "' ";
                                strSql = strSql + ", @pUP_EMP_NO = '" + SystemBase.Base.gstrUserID + "'";
                                strSql = strSql + ", @pUP_IP = '" + SystemBase.Base.gstrUserIp + "'";

                                DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                                ERRCode = ds.Tables[0].Rows[0][0].ToString();
                                MSGCode = ds.Tables[0].Rows[0][1].ToString();

                                iFirst_Check++;

                                if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
                            }
                        }
                        if (iFirst_Check == 1 && Ap_Dt.Rows.Count > 0)
                        {
                            ERRCode = "ER";
                            strSql = " usp_ACD001 'D6'";
                            strSql = strSql + ", @pCO_CD  = '" + SystemBase.Base.gstrCOMCD + "'";
                            strSql = strSql + ", @pSLIP_NO = '" + strSLIPNO + "' ";

                            strSql = strSql + ", @pUP_EMP_NO = '" + SystemBase.Base.gstrUserID + "'";
                            strSql = strSql + ", @pUP_IP = '" + SystemBase.Base.gstrUserIp + "'";

                            DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                            ERRCode = ds.Tables[0].Rows[0][0].ToString();
                            MSGCode = ds.Tables[0].Rows[0][1].ToString();

                            if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
                        }
                    }



                    if (SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", false) == true)
                    {
                        int iEnd_Row = 0;
                        for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                        {
                            string strHead = fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text;
                            if (strHead.Length > 0 && (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "계정코드")].Text != "11110001" || cboSlipType.SelectedValue.ToString() == "03"))
                            {
                                iEnd_Row = i;
                            }
                        }

                        //행수만큼 처리
                        for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                        {
                            string strHead = fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text;
                            string strGbn = "";

                            if (strHead.Length > 0 && (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "계정코드")].Text != "11110001" || cboSlipType.SelectedValue.ToString() == "03"))
                            {
                                switch (strHead)
                                {
                                    case "U": strGbn = "U2"; break;
                                    case "I": strGbn = "I2"; break;
                                    case "D": strGbn = "D2"; break;
                                    default: strGbn = ""; break;
                                }


                                ERRCode = "ER";

                                strSql = " usp_ACD001 '" + strGbn + "'";
                                strSql = strSql + ", @pCO_CD  = '" + SystemBase.Base.gstrCOMCD + "'";
                                strSql = strSql + ", @pSLIP_NO = '" + strSLIPNO + "' ";
                                strSql = strSql + ", @pSLIP_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "순번")].Text + "' ";
                                strSql = strSql + ", @pDEPT_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "귀속부서")].Text + "' ";
                                strSql = strSql + ", @pREORG_ID = '" + strREORG_ID + "' ";
                                strSql = strSql + ", @pBIZ_AREA_CD = '" + strBIZ_CD + "' ";
                                strSql = strSql + ", @pACCT_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "계정코드")].Text + "' ";
                                strSql = strSql + ", @pDR_CR = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "차대구분")].Text + "' ";
                                strSql = strSql + ", @pCUR_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "화폐단위")].Text + "' ";
                                strSql = strSql + ", @pEXCH_RATE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "환율")].Text + "' ";
                                strSql = strSql + ", @pSLIP_AMT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "전표금액")].Text + "' ";
                                strSql = strSql + ", @pSLIP_AMT_LOC = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "전표금액(자국)")].Text + "' ";
                                strSql = strSql + ", @pREMARK = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "적요")].Text + "' ";
                                strSql = strSql + ", @pCTRL_CD1 = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "관리항목1코드")].Text + "' ";
                                strSql = strSql + ", @pCTRL_VAL1 = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "관리항목1")].Text + "' ";
                                strSql = strSql + ", @pCTRL_VAL1_NM = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "관리항목1명")].Text + "' ";
                                strSql = strSql + ", @pCTRL_CD2 = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "관리항목2코드")].Text + "' ";
                                strSql = strSql + ", @pCTRL_VAL2 = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "관리항목2")].Text + "' ";
                                strSql = strSql + ", @pCTRL_VAL2_NM = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "관리항목2명")].Text + "' ";
                                strSql = strSql + ", @pCTRL_CD3 = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "관리항목3코드")].Text + "' ";
                                strSql = strSql + ", @pCTRL_VAL3 = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "관리항목3")].Text + "' ";
                                strSql = strSql + ", @pCTRL_VAL3_NM = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "관리항목3명")].Text + "' ";
                                strSql = strSql + ", @pCTRL_CD4 = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "관리항목4코드")].Text + "' ";
                                strSql = strSql + ", @pCTRL_VAL4 = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "관리항목4")].Text + "' ";
                                strSql = strSql + ", @pCTRL_VAL4_NM = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "관리항목4명")].Text + "' ";
                                strSql = strSql + ", @pCTRL_CD5 = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "관리항목5코드")].Text + "' ";
                                strSql = strSql + ", @pCTRL_VAL5 = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "관리항목5")].Text + "' ";
                                strSql = strSql + ", @pCTRL_VAL5_NM = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "관리항목5명")].Text + "' ";
                                strSql = strSql + ", @pCTRL_CD6 = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "관리항목6코드")].Text + "' ";
                                strSql = strSql + ", @pCTRL_VAL6 = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "관리항목6")].Text + "' ";
                                strSql = strSql + ", @pCTRL_VAL6_NM = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "관리항목6명")].Text + "' ";
                                strSql = strSql + ", @pCTRL_CD7 = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "관리항목7코드")].Text + "' ";
                                strSql = strSql + ", @pCTRL_VAL7 = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "관리항목7")].Text + "' ";
                                strSql = strSql + ", @pCTRL_VAL7_NM = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "관리항목7명")].Text + "' ";
                                strSql = strSql + ", @pCTRL_CD8 = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "관리항목8코드")].Text + "' ";
                                strSql = strSql + ", @pCTRL_VAL8 = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "관리항목8")].Text + "' ";
                                strSql = strSql + ", @pCTRL_VAL8_NM = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "관리항목8명")].Text + "' ";
                                if (i == iEnd_Row) strSql = strSql + ", @pEND_FLAG = 'Y' ";
                                else strSql = strSql + ", @pEND_FLAG = 'N' ";
                                strSql = strSql + ", @pSLIP_DIV = 'G'";
                                strSql = strSql + ", @pUP_EMP_NO = '" + SystemBase.Base.gstrUserID + "'";
                                strSql = strSql + ", @pUP_IP = '" + SystemBase.Base.gstrUserIp + "'";

                                DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                                ERRCode = ds.Tables[0].Rows[0][0].ToString();
                                MSGCode = ds.Tables[0].Rows[0][1].ToString();
                                strSLIPNO = ds.Tables[0].Rows[0][2].ToString();

                                if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
                            }
                        }
                    }

                    Trans.Commit();
                }
                catch
                {
                    Trans.Rollback();
                    MSGCode = "SY002";	//에러가 발생하여 데이터 처리가 취소되었습니다.
                }
            Exit:
                dbConn.Close();

                if (ERRCode == "OK")
                {
                    txtSSlipNo.Text = strSLIPNO;
                    SearchExec();
                    SearchData = "";
                    //컨트롤 체크 함수
                    gBox = new GroupBox[] { groupBox2 };
                    SystemBase.Validation.Control_Check(gBox, ref SearchData);

                    UIForm.FPMake.GridSetFocus(fpSpread1, strCtrlcd);
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
            }

            this.Cursor = Cursors.Default;
        }
        #endregion

        #region PrintExec() PRINT 버튼 클릭 이벤트
        protected override void PrintExec()
        {
            try
            {
                if (txtSlipNo.Text == "")
                {
                    MessageBox.Show("전표를 조회 후 출력하세요.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    string RptName = SystemBase.Base.ProgramWhere + @"\Report\ACD001.rpt";    // 레포트경로+레포트명
                    string[] RptParmValue = new string[4];   // SP 파라메타 값

                    RptParmValue[0] = "P1";
                    RptParmValue[1] = SystemBase.Base.gstrCOMCD;
                    RptParmValue[2] = txtSlipNo.Text;
                    RptParmValue[3] = "G";

                    UIForm.PRINT10 frm = new UIForm.PRINT10(this.Text + "출력", null, null, RptName, RptParmValue); //공통크리스탈 10버전
                    //UIForm.PRINT10 frm = new UIForm.PRINT10( this.Text + "출력", null, RptName, RptParmValue);	//공통크리스탈 10버전
                    frm.ShowDialog();
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(SystemBase.Base.MessageRtn(f.ToString()), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region DelExec() 삭제 로직
        protected override void DeleteExec()
        {
            DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("SY010"), SystemBase.Base.MessageRtn("Z0004"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dsMsg == DialogResult.Yes)
            {
                string ERRCode = "ER", MSGCode = "SY001";	//처리할 내용이 없습니다.
                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                try
                {
                    string strSql = " usp_ACD001 'D1'";
                    strSql = strSql + ", @pCO_CD  = '" + SystemBase.Base.gstrCOMCD + "'";
                    strSql = strSql + ", @pSLIP_NO = '" + txtSlipNo.Text + "' ";
                    strSql = strSql + ", @pSLIP_DIV = 'G'";
                    strSql = strSql + ", @pUP_EMP_NO = '" + SystemBase.Base.gstrUserID + "'";
                    strSql = strSql + ", @pUP_IP = '" + SystemBase.Base.gstrUserIp + "'";

                    DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                    ERRCode = ds.Tables[0].Rows[0][0].ToString();
                    MSGCode = ds.Tables[0].Rows[0][1].ToString();

                    if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                    Trans.Commit();
                    NewExec();
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    Trans.Rollback();
                    MSGCode = "SY002";	//에러가 발생하여 데이터 처리가 취소되었습니다.
                }
            Exit:
                dbConn.Close();

                if (ERRCode == "OK")
                {
                    txtSSlipNo.Value = "";
                    NewExec();
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
            }
        }
        #endregion

        #region RowInsExec 행 추가
        protected override void RowInsExec()
        {	// 행 추가
            try
            {
                Detail_New();
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        #endregion

        #region DelExec 행 삭제
        protected override void DelExec()
        {	// 행 삭제
            try
            {
                int iRow = fpSpread1.ActiveSheet.ActiveRowIndex;
                if (fpSpread1.Sheets[0].RowHeader.Cells[iRow, 0].Text == "I") Detail_New();
                UIForm.FPMake.RowRemove(fpSpread1);
                //DelExe();

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY008", "행삭제"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        #endregion

        #region 계정 상세조회
        private void SEARCH_ACCT(string ACCT_CD)
        {
            try
            {
                string strType = "U";
                if (txtSeq.Text == "")
                {
                    strType = "I";
                }
                for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                {
                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "순번")].Text == txtSeq.Text && fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text == "I")
                    {
                        strType = "I";
                    }
                }

                string strQuery = "";

                if (strType == "I")
                {
                    strQuery = " usp_A_COMMON  'A031'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    strQuery = strQuery + ", @pCODE ='" + ACCT_CD + "' ";
                }
                else
                {
                    strQuery = " usp_ACD001  'S5'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    strQuery = strQuery + ", @pSLIP_NO ='" + txtSlipNo.Text + "' ";
                    strQuery = strQuery + ", @pSLIP_SEQ ='" + txtSeq.Text + "' ";
                }
                DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);
                if (dt.Rows.Count > 0)
                {
                    strCTRL_CD[1] = dt.Rows[0]["CTRL_CD1"].ToString().Trim();
                    c1Label_Crtl1.Text = dt.Rows[0]["CTRL_CD1_NM"].ToString().Trim();
                    strCTRL_NULL[1] = dt.Rows[0]["CTRL_NULL1"].ToString().Trim();

                    strCTRL_CD[2] = dt.Rows[0]["CTRL_CD2"].ToString().Trim();
                    c1Label_Crtl2.Text = dt.Rows[0]["CTRL_CD2_NM"].ToString().Trim();
                    strCTRL_NULL[2] = dt.Rows[0]["CTRL_NULL2"].ToString().Trim();

                    strCTRL_CD[3] = dt.Rows[0]["CTRL_CD3"].ToString().Trim();
                    c1Label_Crtl3.Text = dt.Rows[0]["CTRL_CD3_NM"].ToString().Trim();
                    strCTRL_NULL[3] = dt.Rows[0]["CTRL_NULL3"].ToString().Trim();

                    strCTRL_CD[4] = dt.Rows[0]["CTRL_CD4"].ToString().Trim();
                    c1Label_Crtl4.Text = dt.Rows[0]["CTRL_CD4_NM"].ToString().Trim();
                    strCTRL_NULL[4] = dt.Rows[0]["CTRL_NULL4"].ToString().Trim();

                    strCTRL_CD[5] = dt.Rows[0]["CTRL_CD5"].ToString().Trim();
                    c1Label_Crtl5.Text = dt.Rows[0]["CTRL_CD5_NM"].ToString().Trim();
                    strCTRL_NULL[5] = dt.Rows[0]["CTRL_NULL5"].ToString().Trim();

                    strCTRL_CD[6] = dt.Rows[0]["CTRL_CD6"].ToString().Trim();
                    c1Label_Crtl6.Text = dt.Rows[0]["CTRL_CD6_NM"].ToString().Trim();
                    strCTRL_NULL[6] = dt.Rows[0]["CTRL_NULL6"].ToString().Trim();

                    strCTRL_CD[7] = dt.Rows[0]["CTRL_CD7"].ToString().Trim();
                    c1Label_Crtl7.Text = dt.Rows[0]["CTRL_CD7_NM"].ToString().Trim();
                    strCTRL_NULL[7] = dt.Rows[0]["CTRL_NULL7"].ToString().Trim();

                    strCTRL_CD[8] = dt.Rows[0]["CTRL_CD8"].ToString().Trim();
                    c1Label_Crtl8.Text = dt.Rows[0]["CTRL_CD8_NM"].ToString().Trim();
                    strCTRL_NULL[8] = dt.Rows[0]["CTRL_NULL8"].ToString().Trim();

                    strSubType = dt.Rows[0]["SUB_TYPE"].ToString().Trim();
                    strAcctType = dt.Rows[0]["ACCT_TYPE"].ToString().Trim();

                    //입금전표는 대변
                    if (cboSlipType.SelectedValue.ToString() == "01")
                    {
                        optCr.Checked = true;
                    }
                    else if (cboSlipType.SelectedValue.ToString() == "02")
                    {
                        optDr.Checked = true;
                    }
                    else
                    {
                        if (dt.Rows[0]["DR_CR"].ToString() == "DR")
                        {
                            optDr.Checked = true;
                        }
                        else if (dt.Rows[0]["DR_CR"].ToString() == "CR")
                        {
                            optCr.Checked = true;
                        }
                    }

                    #region 날짜형식변환
                    if (strCTRL_CD[1] == "C1" || strCTRL_CD[1] == "C2" || strCTRL_CD[1] == "V2")
                        txtCTRL_VAL1.FormatType = C1.Win.C1Input.FormatTypeEnum.ShortDate;
                    else
                        txtCTRL_VAL1.FormatType = C1.Win.C1Input.FormatTypeEnum.DefaultFormat;

                    if (strCTRL_CD[2] == "C1" || strCTRL_CD[2] == "C2" || strCTRL_CD[2] == "V2")
                        txtCTRL_VAL2.FormatType = C1.Win.C1Input.FormatTypeEnum.ShortDate;
                    else
                        txtCTRL_VAL2.FormatType = C1.Win.C1Input.FormatTypeEnum.DefaultFormat;

                    if (strCTRL_CD[3] == "C1" || strCTRL_CD[3] == "C2" || strCTRL_CD[3] == "V2")
                        txtCTRL_VAL3.FormatType = C1.Win.C1Input.FormatTypeEnum.ShortDate;
                    else
                        txtCTRL_VAL3.FormatType = C1.Win.C1Input.FormatTypeEnum.DefaultFormat;

                    if (strCTRL_CD[4] == "C1" || strCTRL_CD[4] == "C2" || strCTRL_CD[4] == "V2")
                        txtCTRL_VAL4.FormatType = C1.Win.C1Input.FormatTypeEnum.ShortDate;
                    else
                        txtCTRL_VAL4.FormatType = C1.Win.C1Input.FormatTypeEnum.DefaultFormat;

                    if (strCTRL_CD[5] == "C1" || strCTRL_CD[5] == "C2" || strCTRL_CD[5] == "V2")
                        txtCTRL_VAL5.FormatType = C1.Win.C1Input.FormatTypeEnum.ShortDate;
                    else
                        txtCTRL_VAL5.FormatType = C1.Win.C1Input.FormatTypeEnum.DefaultFormat;

                    if (strCTRL_CD[6] == "C1" || strCTRL_CD[6] == "C2" || strCTRL_CD[6] == "V2")
                        txtCTRL_VAL6.FormatType = C1.Win.C1Input.FormatTypeEnum.ShortDate;
                    else
                        txtCTRL_VAL6.FormatType = C1.Win.C1Input.FormatTypeEnum.DefaultFormat;

                    if (strCTRL_CD[7] == "C1" || strCTRL_CD[7] == "C2" || strCTRL_CD[7] == "V2")
                        txtCTRL_VAL7.FormatType = C1.Win.C1Input.FormatTypeEnum.ShortDate;
                    else
                        txtCTRL_VAL7.FormatType = C1.Win.C1Input.FormatTypeEnum.DefaultFormat;

                    if (strCTRL_CD[8] == "C1" || strCTRL_CD[8] == "C2" || strCTRL_CD[8] == "V2")
                        txtCTRL_VAL8.FormatType = C1.Win.C1Input.FormatTypeEnum.ShortDate;
                    else
                        txtCTRL_VAL8.FormatType = C1.Win.C1Input.FormatTypeEnum.DefaultFormat;
                    #endregion
                }
                else
                {
                    for (int i = 1; i < strCTRL_CD.Length; i++)
                    {
                        strCTRL_CD[i] = "";
                        strCTRL_NULL[i] = "";
                    }
                    c1Label_Crtl1.Text = "";
                    c1Label_Crtl2.Text = "";
                    c1Label_Crtl3.Text = "";
                    c1Label_Crtl4.Text = "";
                    c1Label_Crtl5.Text = "";
                    c1Label_Crtl6.Text = "";
                    c1Label_Crtl7.Text = "";
                    c1Label_Crtl8.Text = "";

                    txtCTRL_VAL_NM1.Value = "";
                    txtCTRL_VAL_NM2.Value = "";
                    txtCTRL_VAL_NM3.Value = "";
                    txtCTRL_VAL_NM4.Value = "";
                    txtCTRL_VAL_NM5.Value = "";
                    txtCTRL_VAL_NM6.Value = "";
                    txtCTRL_VAL_NM7.Value = "";
                    txtCTRL_VAL_NM8.Value = "";
                    strSubType = "";
                    strAcctType = "";
                }
                if (strCTRL_CD[1] == "C1") txtCTRL_VAL1.Value = "2999-12-31"; else txtCTRL_VAL1.Value = "";
                if (strCTRL_CD[2] == "C1") txtCTRL_VAL2.Value = "2999-12-31"; else txtCTRL_VAL2.Value = "";
                if (strCTRL_CD[3] == "C1") txtCTRL_VAL3.Value = "2999-12-31"; else txtCTRL_VAL3.Value = "";
                if (strCTRL_CD[4] == "C1") txtCTRL_VAL4.Value = "2999-12-31"; else txtCTRL_VAL4.Value = "";
                if (strCTRL_CD[5] == "C1") txtCTRL_VAL5.Value = "2999-12-31"; else txtCTRL_VAL5.Value = "";
                if (strCTRL_CD[6] == "C1") txtCTRL_VAL6.Value = "2999-12-31"; else txtCTRL_VAL6.Value = "";
                if (strCTRL_CD[7] == "C1") txtCTRL_VAL7.Value = "2999-12-31"; else txtCTRL_VAL7.Value = "";
                if (strCTRL_CD[8] == "C1") txtCTRL_VAL8.Value = "2999-12-31"; else txtCTRL_VAL8.Value = "";

                txtCTRL_VAL_NM1.Value = "";
                txtCTRL_VAL_NM2.Value = "";
                txtCTRL_VAL_NM3.Value = "";
                txtCTRL_VAL_NM4.Value = "";
                txtCTRL_VAL_NM5.Value = "";
                txtCTRL_VAL_NM6.Value = "";
                txtCTRL_VAL_NM7.Value = "";
                txtCTRL_VAL_NM8.Value = "";

                Ctrl_Color_Chk();
                POP_ENABLED();

                //GroupBox[] gBox = new GroupBox[] { groupBox2 };
                //SystemBase.Validation.Control_Check(gBox, ref SearchData);

            }

            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 관리항목 테크 변경 및 색깔, 읽기전용 변경(공통 GroupBox_Setting 이용)
        protected void Ctrl_Color_Chk()
        {
            try
            {
                txtCTRL_VAL1.Tag = CTRL_VAL_TAG(1, c1Label_Crtl1.Text);
                txtCTRL_VAL2.Tag = CTRL_VAL_TAG(2, c1Label_Crtl2.Text);
                txtCTRL_VAL3.Tag = CTRL_VAL_TAG(3, c1Label_Crtl3.Text);
                txtCTRL_VAL4.Tag = CTRL_VAL_TAG(4, c1Label_Crtl4.Text);
                txtCTRL_VAL5.Tag = CTRL_VAL_TAG(5, c1Label_Crtl5.Text);
                txtCTRL_VAL6.Tag = CTRL_VAL_TAG(6, c1Label_Crtl6.Text);
                txtCTRL_VAL7.Tag = CTRL_VAL_TAG(7, c1Label_Crtl7.Text);
                txtCTRL_VAL8.Tag = CTRL_VAL_TAG(8, c1Label_Crtl8.Text);
                Ctrl_Readonly_Chk();
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        protected string CTRL_VAL_TAG(int SEQ, string LABEL)
        {
            try
            {
                string strLabel_Tag = "";
                if (strCTRL_NULL[SEQ].Trim() == "C")
                {
                    strLabel_Tag = LABEL + ";1;;";
                }
                else if (optDr.Checked == true && strCTRL_NULL[SEQ] == "A")
                {
                    strLabel_Tag = LABEL + ";1;;";
                }
                else if (optCr.Checked == true && strCTRL_NULL[SEQ] == "B")
                {
                    strLabel_Tag = LABEL + ";1;;";
                }
                else if (strCTRL_NULL[SEQ] == "")
                {
                    strLabel_Tag = LABEL + ";2;;";
                }
                else
                {
                    strLabel_Tag = LABEL + ";;;";
                }
                return strLabel_Tag;
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return "";
            }
        }

        protected void Ctrl_Readonly_Chk()
        {
            try
            {
                SystemBase.Validation.GroupBox_Setting(groupBox3);//필수 적용
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region RCopyExec 그리드 Row 복사
        protected override void RCopyExec()
        {
            try
            {
                int Up_Row = 0;
                int iMAxRow = 0;

                UIForm.FPMake.RowCopy(fpSpread1);
                RCopyExe();
                if (fpSpread1.Sheets[0].ActiveRow != null)
                {
                    Up_Row = fpSpread1.Sheets[0].ActiveRow.Index;


                    for (int i = 0; i < fpSpread1.Sheets[0].RowCount; i++)
                    {
                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "순번")].Text != "")
                        {
                            if (iMAxRow < Convert.ToInt32(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "순번")].Text))
                            {
                                iMAxRow = Convert.ToInt32(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "순번")].Text);
                            }
                        }
                    }
                    iMAxRow++;
                    fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRow.Index, SystemBase.Base.GridHeadIndex(GHIdx1, "순번")].Text = (iMAxRow).ToString();
                    Detail_New();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY008", "행복사"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 전표복사
        private void btnSlipCopy_Click(object sender, EventArgs e)
        {
            try
            {
                txtSlipNo.Value = "";

                btnDept.Enabled = true;
                txtDeptCd.Enabled = true;
                dtpSlipDt.Enabled = true;
                txtConfirm_YN.Value = "미승인";

                SearchData = "";
                
                for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                {
                    fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text = "I";
                }
                Ar_Dt.Clear(); //채권
                Ar_Dt_Temp.Clear(); //채권
                Ap_Dt.Clear(); //채무
                Ap_Dt_Temp.Clear(); //채무
                Loan_Dt.Clear(); //차입금
                Loan_Dt_Temp.Clear(); //차입금
                Asset_Dt.Clear(); //고정자산
                Asset_Dt_Temp.Clear(); //고정자산
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 그리드 선택변경
        private void fpSpread1_SelectionChanged(object sender, FarPoint.Win.Spread.SelectionChangedEventArgs e)
        {
            try
            {
                int intRow = fpSpread1.ActiveSheet.GetSelection(0).Row;
                if(intRow > -1) fpSpread1_SelectionChanged_Event(intRow);
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        protected void fpSpread1_SelectionChanged_Event(int intRow)
        {
            try
            {
                string strHead = fpSpread1.Sheets[0].RowHeader.Cells[intRow, 0].Text;
                if (intRow < 0)
                {
                    this.Cursor = System.Windows.Forms.Cursors.Default;
                    return;
                }

                if (PreRow == intRow && PreRow != -1 && intRow != -1)   //현 Row에서 컬럼이동시는 조회 안되게
                {
                    this.Cursor = System.Windows.Forms.Cursors.Default;
                    return;
                }
                SEARCH_SLIP_DETAIL(intRow);
                PreRow = intRow;

                if (cboSlipType.SelectedValue.ToString() == "01")
                {
                    //optCr.Checked = true;
                    optCr.Enabled = false;
                    optDr.Enabled = false;
                }
                else if (cboSlipType.SelectedValue.ToString() == "02")
                {
                    //optDr.Checked = true;
                    optCr.Enabled = false;
                    optDr.Enabled = false;
                }
                else
                {
                    optCr.Enabled = true;
                    optDr.Enabled = true;
                }

                Ar_Dt_Temp.Clear();
                for (int iRow = 0; iRow < Ar_Dt.Rows.Count; iRow++)
                {
                    if (Ar_Dt.Rows[iRow].RowState.ToString() != "Deleted")
                    {
                        if (Ar_Dt.Rows[iRow]["SLIP_SEQ"].ToString() == txtSeq.Text)
                        {
                            DataRow Tr = Ar_Dt_Temp.NewRow();
                            DataRow Dr = Ar_Dt.Rows[iRow];
                            for (int i = 0; i < Ar_Dt.Columns.Count; i++)
                            {
                                Tr[i] = Dr[i];
                            }
                            Ar_Dt_Temp.Rows.Add(Tr);
                        }
                    }
                }

                Ap_Dt_Temp.Clear();
                for (int iRow = 0; iRow < Ap_Dt.Rows.Count; iRow++)
                {
                    if (Ap_Dt.Rows[iRow].RowState.ToString() != "Deleted")
                    {
                        if (Ap_Dt.Rows[iRow]["SLIP_SEQ"].ToString() == txtSeq.Text)
                        {
                            DataRow Tr = Ap_Dt_Temp.NewRow();
                            DataRow Dr = Ap_Dt.Rows[iRow];
                            for (int i = 0; i < Ap_Dt.Columns.Count; i++)
                            {
                                Tr[i] = Dr[i];
                            }
                            Ap_Dt_Temp.Rows.Add(Tr);
                        }
                    }
                }

                Loan_Dt_Temp.Clear();
                for (int iRow = 0; iRow < Loan_Dt.Rows.Count; iRow++)
                {
                    if (Loan_Dt.Rows[iRow].RowState.ToString() != "Deleted")
                    {
                        if (Loan_Dt.Rows[iRow]["SLIP_SEQ"].ToString() == txtSeq.Text)
                        {
                            DataRow Tr = Loan_Dt_Temp.NewRow();
                            DataRow Dr = Loan_Dt.Rows[iRow];
                            for (int i = 0; i < Loan_Dt.Columns.Count; i++)
                            {
                                Tr[i] = Dr[i];
                            }
                            Loan_Dt_Temp.Rows.Add(Tr);
                        }
                    }
                }

                Asset_Dt_Temp.Clear();
                for (int iRow = 0; iRow < Asset_Dt.Rows.Count; iRow++)
                {
                    if (Asset_Dt.Rows[iRow].RowState.ToString() != "Deleted")
                    {
                        if (Asset_Dt.Rows[iRow]["SLIP_SEQ"].ToString() == txtSeq.Text)
                        {
                            DataRow Tr = Asset_Dt_Temp.NewRow();
                            DataRow Dr = Asset_Dt.Rows[iRow];
                            for (int i = 0; i < Asset_Dt.Columns.Count; i++)
                            {
                                Tr[i] = Dr[i];
                            }
                            Asset_Dt_Temp.Rows.Add(Tr);
                        }
                    }
                }
                string strAcctBizCd = SystemBase.Base.CodeName("ACCT_CD", "BIZ_AREA_CD", "A_ACCT_CODE", txtAcctCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "' ");
                //if (txtConfirm_YN.Text == "승인")
                //{
                //    SystemBase.Validation.GroupBoxControlsLock(groupBox3, true);
                //}
                //else 
                if (strAcctBizCd != "" && strHead != "I")
                {
                    SystemBase.Validation.GroupBoxControlsLock(groupBox3, true);
                }
                else if (cboCreathPath.SelectedValue.ToString() != "GL" && strHead != "I")
                {
                    SystemBase.Validation.GroupBoxControlsLock(groupBox3, true);
                    txtRemark2.BackColor = SystemBase.Validation.Kind_White;
                    txtRemark2.ReadOnly = false;
                    btnConfirm.Enabled = true;
                }
                else if (txtAcctCd.Text == "11110001" && cboSlipType.SelectedValue.ToString() != "03" && strHead != "I")
                {
                    SystemBase.Validation.GroupBoxControlsLock(groupBox3, true);
                    txtRemark2.BackColor = SystemBase.Validation.Kind_White;
                    txtRemark2.ReadOnly = false;
                    btnConfirm.Enabled = true;
                }
                else if (fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "AUTO_YN")].Text == "Y")
                {
                    SystemBase.Validation.GroupBoxControlsLock(groupBox3, true);
                    txtRemark2.BackColor = SystemBase.Validation.Kind_White;
                    txtRemark2.ReadOnly = false;
                    btnConfirm.Enabled = true;
                }
                else
                {
                    SystemBase.Validation.GroupBoxControlsLock(groupBox3, false);
                    txtRemark2.BackColor = SystemBase.Validation.Kind_White;
                    txtRemark2.ReadOnly = false;
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 텍스트 체인지
        //전표금액 변경 : 전표금액자국 = 전표금액 * 환율
        private void txtSlipAmt_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (txtSlipAmt.Text.Replace("-", "") != "" && txtExch_Rate.Text.Replace("-", "") != "")
                {
                    txtSlipAmtLoc.Value = Math.Round(Convert.ToDecimal(txtSlipAmt.Text.Replace(",", "")) * Convert.ToDecimal(txtExch_Rate.Text.Replace(",", "")), 0);
                }
                else
                {
                    txtSlipAmtLoc.Value = 0;
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        //전표금액 변경 : 전표금액자국 = 전표금액 * 환율
        private void txtExch_Rate_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (txtSlipAmt.Text.Replace("-", "") != "" && txtExch_Rate.Text.Replace("-", "") != "")
                {
                    txtSlipAmtLoc.Value = Math.Round(Convert.ToDecimal(txtSlipAmt.Text.Replace(",", "")) * Convert.ToDecimal(txtExch_Rate.Text.Replace(",", "")), 0);
                }
                else
                {
                    txtSlipAmtLoc.Value = 0;
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //발생부서
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

        //귀속부서
        private void txtInputDeptCd_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string strPreInputDeptNm = txtInputDeptNm.Text;
                txtInputDeptNm.Value = SystemBase.Base.CodeName("DEPT_CD", "DEPT_NM", "B_DEPT_INFO", txtInputDeptCd.Text, " AND REORG_ID = '" + strREORG_ID + "' AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                strBIZ_CD = SystemBase.Base.CodeName("DEPT_CD", "BIZ_CD", "B_DEPT_INFO", txtInputDeptCd.Text, " AND REORG_ID = '" + strREORG_ID + "' AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                if (strPreInputDeptNm != txtInputDeptNm.Text) ESTIMATE_SET();
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //계정코드
        private void txtAcctCd_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string strPreAcctNm = txtAcctNm.Text;
                txtAcctNm.Value = SystemBase.Base.CodeName("ACCT_CD", "ACCT_NM", "A_ACCT_CODE", txtAcctCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "' AND ENTRY_YN = 'Y'");
                if (strPreAcctNm != txtAcctNm.Text || strNewFlag == "Y")
                {
                    SEARCH_ACCT(txtAcctCd.Text);
                    ESTIMATE_SET();
                    if (txtAcctCd.Text == "11310001" || txtAcctCd.Text == "21090005")
                        ACCT_DEFAULT();
                }
                strNewFlag = "N";
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 팝업 클릭
        //전표번호
        private void btnSSlip_Click(object sender, EventArgs e)
        {
            try
            {
                try
                {
                    ACD004P1 pu = new ACD004P1();
                    pu.ShowDialog();
                    if (pu.DialogResult == DialogResult.OK)
                    {
                        string[] Msgs = pu.ReturnVal;

                        txtSSlipNo.Value = Msgs[1].ToString();
                        txtSSlipNo.Focus();
                        SearchExec();
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "부서 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //발생부서
        private void btnDept_Click(object sender, EventArgs e)
        {
            try
            {
                WNDW.WNDW011 pu = new WNDW.WNDW011(dtpSlipDt.Text);
                pu.MaximizeBox = false;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtDeptCd.Value = Msgs[1].ToString();
                    txtDeptCd.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "부서 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        //귀속부서
        private void btnInputDept_Click(object sender, EventArgs e)
        {
            try
            {
                WNDW.WNDW011 pu = new WNDW.WNDW011(dtpSlipDt.Text);
                pu.MaximizeBox = false;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtInputDeptCd.Value = Msgs[1].ToString();
                    strBIZ_CD = Msgs[3].ToString();
                    ESTIMATE_SET();
                    txtInputDeptCd.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "부서 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        //계정코드
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
                    SEARCH_ACCT(txtAcctCd.Text);
                    ESTIMATE_SET();
                    if (txtAcctCd.Text == "11310001" || txtAcctCd.Text == "21090005") ACCT_DEFAULT();
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //관리항목1
        private void btnCtrl1_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtCTRL_VAL1.ReadOnly == false)
                    CTRL_POPUP(strCTRL_CD[1], txtCTRL_VAL1);
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //관리항목2
        private void btnCtrl2_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtCTRL_VAL2.ReadOnly == false)
                    CTRL_POPUP(strCTRL_CD[2], txtCTRL_VAL2);
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //관리항목3
        private void btnCtrl3_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtCTRL_VAL3.ReadOnly == false)
                    CTRL_POPUP(strCTRL_CD[3], txtCTRL_VAL3);
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //관리항목4
        private void btnCtrl4_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtCTRL_VAL4.ReadOnly == false)
                    CTRL_POPUP(strCTRL_CD[4], txtCTRL_VAL4);
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //관리항목5
        private void btnCtrl5_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtCTRL_VAL5.ReadOnly == false)
                    CTRL_POPUP(strCTRL_CD[5], txtCTRL_VAL5);
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //관리항목6
        private void btnCtrl6_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtCTRL_VAL6.ReadOnly == false)
                    CTRL_POPUP(strCTRL_CD[6], txtCTRL_VAL6);
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //관리항목7
        private void btnCtrl7_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtCTRL_VAL7.ReadOnly == false)
                    CTRL_POPUP(strCTRL_CD[7], txtCTRL_VAL7);
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //관리항목8
        private void btnCtrl8_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtCTRL_VAL8.ReadOnly == false)
                    CTRL_POPUP(strCTRL_CD[8], txtCTRL_VAL8);
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 관리항목 팝업
        protected void CTRL_POPUP(string CTRL_CD, TextBox CTRL_VALUE)
        {
            try
            {
                if (CTRL_CD == "BA")//계좌번호
                {
                    try
                    {
                        int iBANK_CTRL_SEQ = 0;
                        string strBANK_CD = "";
                        for (int i = 1; i < strCTRL_CD.Length; i++)
                        {
                            if (strCTRL_CD[i] == "BK")
                            {
                                iBANK_CTRL_SEQ = i;
                                if (i == 1)
                                    strBANK_CD = txtCTRL_VAL1.Text;
                                else if (i == 2)
                                    strBANK_CD = txtCTRL_VAL2.Text;
                                else if (i == 3)
                                    strBANK_CD = txtCTRL_VAL3.Text;
                                else if (i == 4)
                                    strBANK_CD = txtCTRL_VAL4.Text;
                                else if (i == 5)
                                    strBANK_CD = txtCTRL_VAL5.Text;
                                else if (i == 6)
                                    strBANK_CD = txtCTRL_VAL6.Text;
                                else if (i == 7)
                                    strBANK_CD = txtCTRL_VAL7.Text;
                                else if (i == 8)
                                    strBANK_CD = txtCTRL_VAL8.Text;
                            }
                        }

                        string strQuery = " usp_ACD001 @pType='P1', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "', @pCTRL_CD = '" + CTRL_CD + "' ";
                        string[] strWhere = new string[] { "@pCODE_CD1", "@pCODE_CD2" };
                        string[] strSearch = new string[] { CTRL_VALUE.Text, strBANK_CD };
                        UIForm.FPPOPUP pu = new UIForm.FPPOPUP("ACD001_P1", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "계좌번호 조회");
                        pu.Width = 800;
                        pu.Height = 800;
                        pu.ShowDialog();
                        if (pu.DialogResult == DialogResult.OK)
                        {
                            Regex rx1 = new Regex("#");
                            string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                            CTRL_VALUE.Text = Msgs[0].ToString();
                            if (strBANK_CD != Msgs[1].ToString())
                            {
                                if (iBANK_CTRL_SEQ == 1)
                                    txtCTRL_VAL1.Value = Msgs[1].ToString();
                                else if (iBANK_CTRL_SEQ == 2)
                                    txtCTRL_VAL2.Value = Msgs[1].ToString();
                                else if (iBANK_CTRL_SEQ == 3)
                                    txtCTRL_VAL3.Value = Msgs[1].ToString();
                                else if (iBANK_CTRL_SEQ == 4)
                                    txtCTRL_VAL4.Value = Msgs[1].ToString();
                                else if (iBANK_CTRL_SEQ == 5)
                                    txtCTRL_VAL5.Value = Msgs[1].ToString();
                                else if (iBANK_CTRL_SEQ == 6)
                                    txtCTRL_VAL6.Value = Msgs[1].ToString();
                                else if (iBANK_CTRL_SEQ == 7)
                                    txtCTRL_VAL7.Value = Msgs[1].ToString();
                                else if (iBANK_CTRL_SEQ == 8)
                                    txtCTRL_VAL8.Value = Msgs[1].ToString();
                            }
                            CTRL_VALUE.Focus();
                        }
                    }
                    catch (Exception f)
                    {
                        SystemBase.Loggers.Log(this.Name, f.ToString());
                        MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "계좌번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else if (CTRL_CD == "BK") //은행
                {
                    try
                    {
                        string strBANK_CD = CTRL_VALUE.Text;

                        string strQuery = " usp_ACD001 @pType='P1', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "', @pCTRL_CD = '" + CTRL_CD + "' ";
                        string[] strWhere = new string[] { "@pCODE_CD1", "@pCODE_CD2" };
                        string[] strSearch = new string[] { CTRL_VALUE.Text, "" };
                        UIForm.FPPOPUP pu = new UIForm.FPPOPUP("ACD001_P2", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "은행 조회");
                        pu.Width = 800;
                        pu.Height = 800;
                        pu.ShowDialog();
                        if (pu.DialogResult == DialogResult.OK)
                        {
                            Regex rx1 = new Regex("#");
                            string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                            CTRL_VALUE.Text = Msgs[0].ToString();
                            for (int i = 1; i < strCTRL_CD.Length; i++)
                            {
                                if (strCTRL_CD[i] == "BA")
                                {
                                    if (i == 1)
                                        txtCTRL_VAL1.Value = "";
                                    else if (i == 2)
                                        txtCTRL_VAL2.Value = "";
                                    else if (i == 3)
                                        txtCTRL_VAL3.Value = "";
                                    else if (i == 4)
                                        txtCTRL_VAL4.Value = "";
                                    else if (i == 5)
                                        txtCTRL_VAL5.Value = "";
                                    else if (i == 6)
                                        txtCTRL_VAL6.Value = "";
                                    else if (i == 7)
                                        txtCTRL_VAL7.Value = "";
                                    else if (i == 8)
                                        txtCTRL_VAL8.Value = "";
                                }
                            }
                            CTRL_VALUE.Focus();
                        }
                    }
                    catch (Exception f)
                    {
                        SystemBase.Loggers.Log(this.Name, f.ToString());
                        MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "은행 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else if (CTRL_CD == "BP" || CTRL_CD == "V6") //거래처
                {
                    try
                    {
                        WNDW.WNDW002 pu = new WNDW.WNDW002(CTRL_VALUE.Text, "");
                        pu.MaximizeBox = false;
                        pu.ShowDialog();
                        if (pu.DialogResult == DialogResult.OK)
                        {
                            string[] Msgs = pu.ReturnVal;

                            CTRL_VALUE.Text = Msgs[1].ToString();
                            CTRL_VALUE.Focus();
                        }
                    }
                    catch (Exception f)
                    {
                        SystemBase.Loggers.Log(this.Name, f.ToString());
                        MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "거래처 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
                    }
                }
                else if (CTRL_CD == "CP" && (strAcctType != "D3" || optCr.Checked != true)) //구매카드번호
                {
                    try
                    {
                        string strQuery = " usp_ACD001 @pType='P1', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "', @pCTRL_CD = '" + CTRL_CD + "', @pSPEC1 = '" + strAcctType + "'";
                        string[] strWhere = new string[] { "@pCODE_CD1", "@pCODE_CD2" };
                        string[] strSearch = new string[] { "", CTRL_VALUE.Text };
                        UIForm.FPPOPUP pu = new UIForm.FPPOPUP("ACD001_P3", strQuery, strWhere, strSearch, new int[] { 2, 0 }, "구매카드번호정보 조회");
                        pu.Width = 800;
                        pu.Height = 800;
                        pu.ShowDialog();
                        if (pu.DialogResult == DialogResult.OK)
                        {
                            Regex rx1 = new Regex("#");
                            string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                            CTRL_VALUE.Text = Msgs[0].ToString();
                            CTRL_VALUE.Focus();
                        }
                    }
                    catch (Exception f)
                    {
                        SystemBase.Loggers.Log(this.Name, f.ToString());
                        MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "구매카드번호정보 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else if (CTRL_CD == "D1") //신용카드번호
                {
                    try
                    {
                        string strQuery = " usp_ACD001 @pType='P1', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "', @pCTRL_CD = '" + CTRL_CD + "' ";
                        string[] strWhere = new string[] { "@pCODE_CD1", "@pCODE_CD2" };
                        string[] strSearch = new string[] { CTRL_VALUE.Text, "" };
                        UIForm.FPPOPUP pu = new UIForm.FPPOPUP("ACD001_P4", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "신용카드번호 조회");
                        pu.Width = 800;
                        pu.Height = 800;
                        pu.ShowDialog();
                        if (pu.DialogResult == DialogResult.OK)
                        {
                            Regex rx1 = new Regex("#");
                            string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                            CTRL_VALUE.Text = Msgs[0].ToString();
                            CTRL_VALUE.Focus();
                        }
                    }
                    catch (Exception f)
                    {
                        SystemBase.Loggers.Log(this.Name, f.ToString());
                        MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "신용카드번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else if (CTRL_CD == "L1" && optDr.Checked == true) //차입번호
                {
                    try
                    {
                        string strQuery = " usp_ACD001 @pType='P1', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "', @pCTRL_CD = '" + CTRL_CD + "' ";
                        string[] strWhere = new string[] { "@pCODE_CD1", "@pCODE_CD2", "@pSPEC1" };
                        string[] strSearch = new string[] { CTRL_VALUE.Text, "", "" };
                        UIForm.FPPOPUP pu = new UIForm.FPPOPUP("ACD001_P5", strQuery, strWhere, strSearch, new int[] { 0, 1, 3 }, "차입번호 조회");
                        pu.Width = 800;
                        pu.Height = 800;
                        pu.ShowDialog();
                        if (pu.DialogResult == DialogResult.OK)
                        {
                            Regex rx1 = new Regex("#");
                            string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                            CTRL_VALUE.Text = Msgs[0].ToString();
                            CTRL_VALUE.Focus();
                        }
                    }
                    catch (Exception f)
                    {
                        SystemBase.Loggers.Log(this.Name, f.ToString());
                        MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "차입번호조회 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else if (CTRL_CD == "MK") //품목코드
                {
                    try
                    {
                        WNDW.WNDW001 pu = new WNDW.WNDW001(CTRL_VALUE.Text, "");
                        pu.MaximizeBox = false;
                        pu.ShowDialog();
                        if (pu.DialogResult == DialogResult.OK)
                        {
                            string[] Msgs = pu.ReturnVal;

                            CTRL_VALUE.Text = Msgs[1].ToString();
                            CTRL_VALUE.Focus();
                        }
                    }
                    catch (Exception f)
                    {
                        SystemBase.Loggers.Log(this.Name, f.ToString());
                        MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목정보 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
                    }
                }
                else if (CTRL_CD == "NN" && (strAcctType != "D1" || optDr.Checked != true) && (strAcctType != "D3" || optCr.Checked != true)) //어음번호
                {
                    try
                    {
                        string strQuery = " usp_ACD001 @pType='P1', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "', @pCTRL_CD = '" + CTRL_CD + "', @pSPEC1 = '" + strAcctType + "'";
                        string[] strWhere = new string[] { "@pCODE_CD1", "@pCODE_CD2" };
                        string[] strSearch = new string[] { "", CTRL_VALUE.Text };
                        UIForm.FPPOPUP pu = new UIForm.FPPOPUP("ACD001_P6", strQuery, strWhere, strSearch, new int[] { 2, 0 }, "어음번호 조회");
                        pu.Width = 800;
                        pu.Height = 800;
                        pu.ShowDialog();
                        if (pu.DialogResult == DialogResult.OK)
                        {
                            Regex rx1 = new Regex("#");
                            string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                            CTRL_VALUE.Text = Msgs[0].ToString();
                            CTRL_VALUE.Focus();
                        }
                    }
                    catch (Exception f)
                    {
                        SystemBase.Loggers.Log(this.Name, f.ToString());
                        MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "어음번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else if (CTRL_CD == "V4") //계산서유형
                {
                    try
                    {
                        string strQuery = " usp_B_COMMON @pType='COMM_POP', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pSPEC1 = 'B040' ";
                        string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                        string[] strSearch = new string[] { CTRL_VALUE.Text, "" };
                        UIForm.FPPOPUP pu = new UIForm.FPPOPUP("ACD001_P7", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "계산서유형 조회");
                        pu.Width = 800;
                        pu.Height = 800;
                        pu.ShowDialog();
                        if (pu.DialogResult == DialogResult.OK)
                        {
                            Regex rx1 = new Regex("#");
                            string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                            CTRL_VALUE.Text = Msgs[0].ToString();
                            CTRL_VALUE.Focus();
                        }
                    }
                    catch (Exception f)
                    {
                        SystemBase.Loggers.Log(this.Name, f.ToString());
                        MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "계산서유형 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else if (CTRL_CD == "V5") //신고사업장
                {
                    try
                    {
                        string strQuery = " usp_ACD001 @pType='P1', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "', @pCTRL_CD = '" + CTRL_CD + "' ";
                        string[] strWhere = new string[] { "@pCODE_CD1", "@pCODE_CD2" };
                        string[] strSearch = new string[] { CTRL_VALUE.Text, "" };
                        UIForm.FPPOPUP pu = new UIForm.FPPOPUP("ACD001_P8", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "신고사업장 조회");
                        pu.Width = 800;
                        pu.Height = 800;
                        pu.ShowDialog();
                        if (pu.DialogResult == DialogResult.OK)
                        {
                            Regex rx1 = new Regex("#");
                            string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                            CTRL_VALUE.Text = Msgs[0].ToString();
                            CTRL_VALUE.Focus();
                        }
                    }
                    catch (Exception f)
                    {
                        SystemBase.Loggers.Log(this.Name, f.ToString());
                        MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "신고사업장 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else if (CTRL_CD == "X1" && optCr.Checked == true) //선급금번호
                {
                    try
                    {
                        string strCust_Cd = "";
                        if (strCTRL_CD[1] == "BP" || strCTRL_CD[1] == "V6") strCust_Cd = txtCTRL_VAL1.Text;
                        else if (strCTRL_CD[2] == "BP" || strCTRL_CD[2] == "V6") strCust_Cd = txtCTRL_VAL2.Text;
                        else if (strCTRL_CD[3] == "BP" || strCTRL_CD[3] == "V6") strCust_Cd = txtCTRL_VAL3.Text;
                        else if (strCTRL_CD[4] == "BP" || strCTRL_CD[4] == "V6") strCust_Cd = txtCTRL_VAL4.Text;
                        else if (strCTRL_CD[5] == "BP" || strCTRL_CD[5] == "V6") strCust_Cd = txtCTRL_VAL5.Text;
                        else if (strCTRL_CD[6] == "BP" || strCTRL_CD[6] == "V6") strCust_Cd = txtCTRL_VAL6.Text;
                        else if (strCTRL_CD[7] == "BP" || strCTRL_CD[7] == "V6") strCust_Cd = txtCTRL_VAL7.Text;
                        else if (strCTRL_CD[8] == "BP" || strCTRL_CD[8] == "V6") strCust_Cd = txtCTRL_VAL8.Text;

                        ACD004P6 pu = new ACD004P6("P10");
                        pu.MaximizeBox = false;
                        pu.Width = 1000;
                        pu.Height = 800;
                        pu.ShowDialog();
                        if (pu.DialogResult == DialogResult.OK)
                        {
                            string[] Msgs = pu.ReturnVal;

                            if (strCTRL_CD[1] == "BP" || strCTRL_CD[1] == "V6") txtCTRL_VAL1.Value = Msgs[1].ToString();
                            else if (strCTRL_CD[2] == "BP" || strCTRL_CD[2] == "V6") txtCTRL_VAL2.Value = Msgs[1].ToString();
                            else if (strCTRL_CD[3] == "BP" || strCTRL_CD[3] == "V6") txtCTRL_VAL3.Value = Msgs[1].ToString();
                            else if (strCTRL_CD[4] == "BP" || strCTRL_CD[4] == "V6") txtCTRL_VAL4.Value = Msgs[1].ToString();
                            else if (strCTRL_CD[5] == "BP" || strCTRL_CD[5] == "V6") txtCTRL_VAL5.Value = Msgs[1].ToString();
                            else if (strCTRL_CD[6] == "BP" || strCTRL_CD[6] == "V6") txtCTRL_VAL6.Value = Msgs[1].ToString();
                            else if (strCTRL_CD[7] == "BP" || strCTRL_CD[7] == "V6") txtCTRL_VAL7.Value = Msgs[1].ToString();
                            else if (strCTRL_CD[8] == "BP" || strCTRL_CD[8] == "V6") txtCTRL_VAL8.Value = Msgs[1].ToString();

                            if (strCTRL_CD[1] == "PN") txtCTRL_VAL1.Value = Msgs[7].ToString();
                            else if (strCTRL_CD[2] == "PN") txtCTRL_VAL2.Value = Msgs[7].ToString();
                            else if (strCTRL_CD[3] == "PN") txtCTRL_VAL3.Value = Msgs[7].ToString();
                            else if (strCTRL_CD[4] == "PN") txtCTRL_VAL4.Value = Msgs[7].ToString();
                            else if (strCTRL_CD[5] == "PN") txtCTRL_VAL5.Value = Msgs[7].ToString();
                            else if (strCTRL_CD[6] == "PN") txtCTRL_VAL6.Value = Msgs[7].ToString();
                            else if (strCTRL_CD[7] == "PN") txtCTRL_VAL7.Value = Msgs[7].ToString();
                            else if (strCTRL_CD[8] == "PN") txtCTRL_VAL8.Value = Msgs[7].ToString();

                            CTRL_VALUE.Text = Msgs[3].ToString();
                            CTRL_VALUE.Focus();
                        }
                    }
                    catch (Exception f)
                    {
                        SystemBase.Loggers.Log(this.Name, f.ToString());
                        MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "선급금번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else if (CTRL_CD == "X2" && optDr.Checked == true) //선수금번호
                {
                    try
                    {
                        string strCust_Cd = "";
                        if (strCTRL_CD[1] == "BP" || strCTRL_CD[1] == "V6") strCust_Cd = txtCTRL_VAL1.Text;
                        else if (strCTRL_CD[2] == "BP" || strCTRL_CD[2] == "V6") strCust_Cd = txtCTRL_VAL2.Text;
                        else if (strCTRL_CD[3] == "BP" || strCTRL_CD[3] == "V6") strCust_Cd = txtCTRL_VAL3.Text;
                        else if (strCTRL_CD[4] == "BP" || strCTRL_CD[4] == "V6") strCust_Cd = txtCTRL_VAL4.Text;
                        else if (strCTRL_CD[5] == "BP" || strCTRL_CD[5] == "V6") strCust_Cd = txtCTRL_VAL5.Text;
                        else if (strCTRL_CD[6] == "BP" || strCTRL_CD[6] == "V6") strCust_Cd = txtCTRL_VAL6.Text;
                        else if (strCTRL_CD[7] == "BP" || strCTRL_CD[7] == "V6") strCust_Cd = txtCTRL_VAL7.Text;
                        else if (strCTRL_CD[8] == "BP" || strCTRL_CD[8] == "V6") strCust_Cd = txtCTRL_VAL8.Text;

                        ACD004P6 pu = new ACD004P6("P11");
                        pu.MaximizeBox = false;
                        pu.Width = 1000;
                        pu.Height = 800;
                        pu.ShowDialog();
                        if (pu.DialogResult == DialogResult.OK)
                        {
                            string[] Msgs = pu.ReturnVal;

                            if (strCTRL_CD[1] == "BP" || strCTRL_CD[1] == "V6") txtCTRL_VAL1.Value = Msgs[1].ToString();
                            else if (strCTRL_CD[2] == "BP" || strCTRL_CD[2] == "V6") txtCTRL_VAL2.Value = Msgs[1].ToString();
                            else if (strCTRL_CD[3] == "BP" || strCTRL_CD[3] == "V6") txtCTRL_VAL3.Value = Msgs[1].ToString();
                            else if (strCTRL_CD[4] == "BP" || strCTRL_CD[4] == "V6") txtCTRL_VAL4.Value = Msgs[1].ToString();
                            else if (strCTRL_CD[5] == "BP" || strCTRL_CD[5] == "V6") txtCTRL_VAL5.Value = Msgs[1].ToString();
                            else if (strCTRL_CD[6] == "BP" || strCTRL_CD[6] == "V6") txtCTRL_VAL6.Value = Msgs[1].ToString();
                            else if (strCTRL_CD[7] == "BP" || strCTRL_CD[7] == "V6") txtCTRL_VAL7.Value = Msgs[1].ToString();
                            else if (strCTRL_CD[8] == "BP" || strCTRL_CD[8] == "V6") txtCTRL_VAL8.Value = Msgs[1].ToString();

                            if (strCTRL_CD[1] == "PN") txtCTRL_VAL1.Value = Msgs[7].ToString();
                            else if (strCTRL_CD[2] == "PN") txtCTRL_VAL2.Value = Msgs[7].ToString();
                            else if (strCTRL_CD[3] == "PN") txtCTRL_VAL3.Value = Msgs[7].ToString();
                            else if (strCTRL_CD[4] == "PN") txtCTRL_VAL4.Value = Msgs[7].ToString();
                            else if (strCTRL_CD[5] == "PN") txtCTRL_VAL5.Value = Msgs[7].ToString();
                            else if (strCTRL_CD[6] == "PN") txtCTRL_VAL6.Value = Msgs[7].ToString();
                            else if (strCTRL_CD[7] == "PN") txtCTRL_VAL7.Value = Msgs[7].ToString();
                            else if (strCTRL_CD[8] == "PN") txtCTRL_VAL8.Value = Msgs[7].ToString();

                            CTRL_VALUE.Text = Msgs[3].ToString();
                            CTRL_VALUE.Focus();
                        }
                    }
                    catch (Exception f)
                    {
                        SystemBase.Loggers.Log(this.Name, f.ToString());
                        MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "선수금번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                // 2015.03.23. hma 추가(Start): 프로젝트번호 팝업
                else if (CTRL_CD == "PN")   //프로젝트번호
                {
                    try
                    {
                        WNDW003 pu = new WNDW003(CTRL_VALUE.Text, "S1");
                        pu.MaximizeBox = false;
                        pu.ShowDialog();
                        if (pu.DialogResult == DialogResult.OK)
                        {
                            string[] Msgs = pu.ReturnVal;

                            CTRL_VALUE.Text = Msgs[3].ToString();
                            CTRL_VALUE.Focus();
                        }
                    }
                    catch (Exception f)
                    {
                        SystemBase.Loggers.Log(this.Name, f.ToString());
                        MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "프로젝트번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);

                    }
                }
                // 2015.03.23. hma 추가(End)

            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region KeyPress 이벤트
        //조회 전표번호
        private void txtSSlipNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (e.KeyChar == Convert.ToChar(Keys.Enter))
                {
                    SearchExec();
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtRemark2_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (e.KeyChar == Convert.ToChar(Keys.Enter))
                {
                    string strName = ((Control)sender).Name;
                    if (SLIP_DETAIL_CHECK(strName))
                        btnConfirm_Click(null, null);
                }

            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtCTRL_VAL1_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (strCTRL_CD[1] == "V1" || strCTRL_CD[1] == "V7")
                {
                    if (!(char.IsDigit(e.KeyChar) || e.KeyChar == Convert.ToChar(Keys.Back)))
                    {
                        e.Handled = true;
                    }
                }
                if (e.KeyChar == Convert.ToChar(Keys.Enter))
                {
                    string strName = ((Control)sender).Name;
                    if (SLIP_DETAIL_CHECK(strName))
                        btnConfirm_Click(null, null);
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtCTRL_VAL2_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (strCTRL_CD[2] == "V1" || strCTRL_CD[2] == "V7")
                {
                    if (!(char.IsDigit(e.KeyChar) || e.KeyChar == Convert.ToChar(Keys.Back)))
                    {
                        e.Handled = true;
                    }
                }
                if (e.KeyChar == Convert.ToChar(Keys.Enter))
                {
                    string strName = ((Control)sender).Name;
                    if (SLIP_DETAIL_CHECK(strName))
                        btnConfirm_Click(null, null);
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtCTRL_VAL3_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (strCTRL_CD[3] == "V1" || strCTRL_CD[3] == "V7")
                {
                    if (!(char.IsDigit(e.KeyChar) || e.KeyChar == Convert.ToChar(Keys.Back)))
                    {
                        e.Handled = true;
                    }
                }
                if (e.KeyChar == Convert.ToChar(Keys.Enter))
                {
                    string strName = ((Control)sender).Name;
                    if (SLIP_DETAIL_CHECK(strName))
                        btnConfirm_Click(null, null);
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtCTRL_VAL4_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (strCTRL_CD[4] == "V1" || strCTRL_CD[4] == "V7")
                {
                    if (!(char.IsDigit(e.KeyChar) || e.KeyChar == Convert.ToChar(Keys.Back)))
                    {
                        e.Handled = true;
                    }
                }
                if (e.KeyChar == Convert.ToChar(Keys.Enter))
                {
                    string strName = ((Control)sender).Name;
                    if (SLIP_DETAIL_CHECK(strName))
                        btnConfirm_Click(null, null);
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtCTRL_VAL5_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (strCTRL_CD[5] == "V1" || strCTRL_CD[5] == "V7")
                {
                    if (!(char.IsDigit(e.KeyChar) || e.KeyChar == Convert.ToChar(Keys.Back)))
                    {
                        e.Handled = true;
                    }
                }
                if (e.KeyChar == Convert.ToChar(Keys.Enter))
                {
                    string strName = ((Control)sender).Name;
                    if (SLIP_DETAIL_CHECK(strName))
                        btnConfirm_Click(null, null);
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtCTRL_VAL6_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (strCTRL_CD[6] == "V1" || strCTRL_CD[6] == "V7")
                {
                    if (!(char.IsDigit(e.KeyChar) || e.KeyChar == Convert.ToChar(Keys.Back)))
                    {
                        e.Handled = true;
                    }
                }
                if (e.KeyChar == Convert.ToChar(Keys.Enter))
                {
                    string strName = ((Control)sender).Name;
                    if (SLIP_DETAIL_CHECK(strName))
                        btnConfirm_Click(null, null);
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtCTRL_VAL7_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (strCTRL_CD[7] == "V1" || strCTRL_CD[7] == "V7")
                {
                    if (!(char.IsDigit(e.KeyChar) || e.KeyChar == Convert.ToChar(Keys.Back)))
                    {
                        e.Handled = true;
                    }
                }
                if (e.KeyChar == Convert.ToChar(Keys.Enter))
                {
                    string strName = ((Control)sender).Name;
                    if (SLIP_DETAIL_CHECK(strName))
                        btnConfirm_Click(null, null);
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtCTRL_VAL8_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (strCTRL_CD[8] == "V1" || strCTRL_CD[8] == "V7")
                {
                    if (!(char.IsDigit(e.KeyChar) || e.KeyChar == Convert.ToChar(Keys.Back)))
                    {
                        e.Handled = true;
                    }
                }
                if (e.KeyChar == Convert.ToChar(Keys.Enter))
                {
                    string strName = ((Control)sender).Name;
                    if (SLIP_DETAIL_CHECK(strName))
                        btnConfirm_Click(null, null);
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region TextChanged 이벤트
        //관리항목1
        private void txtCTRL_VAL1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (strCTRL_CD[1] == "V1" || strCTRL_CD[1] == "V7")
                {
                    string lgsText;
                    lgsText = txtCTRL_VAL1.Text.Replace(",", ""); //** 숫자변환시 콤마로 발생하는 에러방지...
                    if (lgsText != "")
                    {
                        txtCTRL_VAL1.Text = String.Format("{0:#,##0}", Convert.ToDouble(lgsText));
                        txtCTRL_VAL1.SelectionStart = txtCTRL_VAL1.TextLength; //** 캐럿을 맨 뒤로 보낸다...
                        txtCTRL_VAL1.SelectionLength = 0;
                    }
                }
                else if (strCTRL_CD[1] == "BA" || strCTRL_CD[1] == "BK" || strCTRL_CD[1] == "CP"
                      || strCTRL_CD[1] == "D1" || strCTRL_CD[1] == "L1" || strCTRL_CD[1] == "NN"
                      || strCTRL_CD[1] == "V5" || strCTRL_CD[1] == "X1" || strCTRL_CD[1] == "X2" || strCTRL_CD[1] == "V5"
                      || strCTRL_CD[1] == "BP" || strCTRL_CD[1] == "V6" || strCTRL_CD[1] == "MK" || strCTRL_CD[1] == "V4"
                      || strCTRL_CD[2] == "PN")     // 2015.03.23. hma 추가
                {
                    DataTable dt = SLIP_DETAIL_VALUE_CHECK(1, txtCTRL_VAL1, "01");
                    if (dt.Rows.Count > 0)
                    {
                        txtCTRL_VAL_NM1.Value = dt.Rows[0]["NAME"].ToString();
                        if (strCTRL_CD[1] == "V4")
                        {
                            if (strCTRL_CD[2] == "V7") txtCTRL_VAL2.Value = dt.Rows[0]["REL_CD1"].ToString();
                            else if (strCTRL_CD[3] == "V7") txtCTRL_VAL3.Value = dt.Rows[0]["REL_CD1"].ToString();
                            else if (strCTRL_CD[4] == "V7") txtCTRL_VAL4.Value = dt.Rows[0]["REL_CD1"].ToString();
                            else if (strCTRL_CD[5] == "V7") txtCTRL_VAL5.Value = dt.Rows[0]["REL_CD1"].ToString();
                            else if (strCTRL_CD[6] == "V7") txtCTRL_VAL6.Value = dt.Rows[0]["REL_CD1"].ToString();
                            else if (strCTRL_CD[7] == "V7") txtCTRL_VAL7.Value = dt.Rows[0]["REL_CD1"].ToString();
                            else if (strCTRL_CD[8] == "V7") txtCTRL_VAL8.Value = dt.Rows[0]["REL_CD1"].ToString();
                        }
                    }
                    else
                    {
                        txtCTRL_VAL_NM1.Value = "";
                    }
                }
                else
                {
                    txtCTRL_VAL_NM1.Value = "";
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
 
        //관리항목2
        private void txtCTRL_VAL2_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (strCTRL_CD[2] == "V1" || strCTRL_CD[2] == "V7")
                {
                    string lgsText;
                    lgsText = txtCTRL_VAL2.Text.Replace(",", ""); //** 숫자변환시 콤마로 발생하는 에러방지...
                    if (lgsText != "")
                    {
                        txtCTRL_VAL2.Text = String.Format("{0:#,##0}", Convert.ToDouble(lgsText));
                        txtCTRL_VAL2.SelectionStart = txtCTRL_VAL2.TextLength; //** 캐럿을 맨 뒤로 보낸다...
                    }
                    txtCTRL_VAL2.SelectionLength = 0;
                }
                else if (strCTRL_CD[2] == "BA" || strCTRL_CD[2] == "BK" || strCTRL_CD[2] == "CP"
                      || strCTRL_CD[2] == "D1" || strCTRL_CD[2] == "L1" || strCTRL_CD[2] == "NN"
                      || strCTRL_CD[2] == "V5" || strCTRL_CD[2] == "X1" || strCTRL_CD[1] == "X2" || strCTRL_CD[2] == "V5"
                      || strCTRL_CD[2] == "BP" || strCTRL_CD[2] == "V6" || strCTRL_CD[2] == "MK" || strCTRL_CD[2] == "V4"
                      || strCTRL_CD[2] == "PN")     // 2015.03.23. hma 추가
                {
                    DataTable dt = SLIP_DETAIL_VALUE_CHECK(2, txtCTRL_VAL2, "01");
                    if (dt.Rows.Count > 0)
                    {
                        txtCTRL_VAL_NM2.Value = dt.Rows[0]["NAME"].ToString();
                        if (strCTRL_CD[2] == "V4")      // 계산서유형
                        {
                            if (strCTRL_CD[1] == "V7") txtCTRL_VAL1.Value = dt.Rows[0]["REL_CD1"].ToString();
                            else if (strCTRL_CD[3] == "V7") txtCTRL_VAL3.Value = dt.Rows[0]["REL_CD1"].ToString();
                            else if (strCTRL_CD[4] == "V7") txtCTRL_VAL4.Value = dt.Rows[0]["REL_CD1"].ToString();
                            else if (strCTRL_CD[5] == "V7") txtCTRL_VAL5.Value = dt.Rows[0]["REL_CD1"].ToString();
                            else if (strCTRL_CD[6] == "V7") txtCTRL_VAL6.Value = dt.Rows[0]["REL_CD1"].ToString();
                            else if (strCTRL_CD[7] == "V7") txtCTRL_VAL7.Value = dt.Rows[0]["REL_CD1"].ToString();
                            else if (strCTRL_CD[8] == "V7") txtCTRL_VAL8.Value = dt.Rows[0]["REL_CD1"].ToString();
                        }
                    }
                    else
                    {
                        txtCTRL_VAL_NM2.Value = "";
                    }
                }
                else
                {
                    txtCTRL_VAL_NM2.Value = "";
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
  
        //관리항목3
        private void txtCTRL_VAL3_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (strCTRL_CD[3] == "V1" || strCTRL_CD[3] == "V7")
                {
                    string lgsText;
                    lgsText = txtCTRL_VAL3.Text.Replace(",", ""); //** 숫자변환시 콤마로 발생하는 에러방지...
                    if (lgsText != "")
                    {
                        txtCTRL_VAL3.Text = String.Format("{0:#,##0}", Convert.ToDouble(lgsText));
                        txtCTRL_VAL3.SelectionStart = txtCTRL_VAL3.TextLength; //** 캐럿을 맨 뒤로 보낸다...
                        txtCTRL_VAL3.SelectionLength = 0;
                    }
                }
                else if (strCTRL_CD[3] == "BA" || strCTRL_CD[3] == "BK" || strCTRL_CD[3] == "CP"
                      || strCTRL_CD[3] == "D1" || strCTRL_CD[3] == "L1" || strCTRL_CD[3] == "NN"
                      || strCTRL_CD[3] == "V5" || strCTRL_CD[3] == "X1" || strCTRL_CD[1] == "X2" || strCTRL_CD[3] == "V5"
                      || strCTRL_CD[3] == "BP" || strCTRL_CD[3] == "V6" || strCTRL_CD[3] == "MK" || strCTRL_CD[3] == "V4"
                      || strCTRL_CD[2] == "PN")     // 2015.03.23. hma 추가
                {
                    DataTable dt = SLIP_DETAIL_VALUE_CHECK(3, txtCTRL_VAL3, "01");
                    if (dt.Rows.Count > 0)
                    {
                        txtCTRL_VAL_NM3.Value = dt.Rows[0]["NAME"].ToString();
                        if (strCTRL_CD[3] == "V4")
                        {
                            if (strCTRL_CD[1] == "V7") txtCTRL_VAL1.Value = dt.Rows[0]["REL_CD1"].ToString();
                            else if (strCTRL_CD[2] == "V7") txtCTRL_VAL2.Value = dt.Rows[0]["REL_CD1"].ToString();
                            else if (strCTRL_CD[4] == "V7") txtCTRL_VAL4.Value = dt.Rows[0]["REL_CD1"].ToString();
                            else if (strCTRL_CD[5] == "V7") txtCTRL_VAL5.Value = dt.Rows[0]["REL_CD1"].ToString();
                            else if (strCTRL_CD[6] == "V7") txtCTRL_VAL6.Value = dt.Rows[0]["REL_CD1"].ToString();
                            else if (strCTRL_CD[7] == "V7") txtCTRL_VAL7.Value = dt.Rows[0]["REL_CD1"].ToString();
                            else if (strCTRL_CD[8] == "V7") txtCTRL_VAL8.Value = dt.Rows[0]["REL_CD1"].ToString();
                        }
                    }
                    else
                    {
                        txtCTRL_VAL_NM3.Value = "";
                    }
                }
                else
                {
                    txtCTRL_VAL_NM3.Value = "";
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
  
        //관리항목4
        private void txtCTRL_VAL4_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (strCTRL_CD[4] == "V1" || strCTRL_CD[4] == "V7")
                {
                    string lgsText;
                    lgsText = txtCTRL_VAL4.Text.Replace(",", "");   //** 숫자변환시 콤마로 발생하는 에러방지...
                    if (lgsText != "")
                    {
                        txtCTRL_VAL4.Text = String.Format("{0:#,##0}", Convert.ToDouble(lgsText));
                        txtCTRL_VAL4.SelectionStart = txtCTRL_VAL4.TextLength; //** 캐럿을 맨 뒤로 보낸다...
                    }
                    txtCTRL_VAL4.SelectionLength = 0;
                }
                else if (strCTRL_CD[4] == "BA" || strCTRL_CD[4] == "BK" || strCTRL_CD[4] == "CP"
                      || strCTRL_CD[4] == "D1" || strCTRL_CD[4] == "L1" || strCTRL_CD[4] == "NN"
                      || strCTRL_CD[4] == "V5" || strCTRL_CD[4] == "X1" || strCTRL_CD[1] == "X2" || strCTRL_CD[4] == "V5"
                      || strCTRL_CD[4] == "BP" || strCTRL_CD[4] == "V6" || strCTRL_CD[4] == "MK" || strCTRL_CD[4] == "V4"
                      || strCTRL_CD[2] == "PN")     // 2015.03.23. hma 추가
                {
                    DataTable dt = SLIP_DETAIL_VALUE_CHECK(4, txtCTRL_VAL4, "01");
                    if (dt.Rows.Count > 0)
                    {
                        txtCTRL_VAL_NM4.Value = dt.Rows[0]["NAME"].ToString();
                        if (strCTRL_CD[4] == "V4")
                        {
                            if (strCTRL_CD[1] == "V7") txtCTRL_VAL1.Value = dt.Rows[0]["REL_CD1"].ToString();
                            else if (strCTRL_CD[2] == "V7") txtCTRL_VAL2.Value = dt.Rows[0]["REL_CD1"].ToString();
                            else if (strCTRL_CD[3] == "V7") txtCTRL_VAL3.Value = dt.Rows[0]["REL_CD1"].ToString();
                            else if (strCTRL_CD[5] == "V7") txtCTRL_VAL5.Value = dt.Rows[0]["REL_CD1"].ToString();
                            else if (strCTRL_CD[6] == "V7") txtCTRL_VAL6.Value = dt.Rows[0]["REL_CD1"].ToString();
                            else if (strCTRL_CD[7] == "V7") txtCTRL_VAL7.Value = dt.Rows[0]["REL_CD1"].ToString();
                            else if (strCTRL_CD[8] == "V7") txtCTRL_VAL8.Value = dt.Rows[0]["REL_CD1"].ToString();
                        }
                    }
                    else
                    {
                        txtCTRL_VAL_NM4.Value = "";
                    }
                }
                else
                {
                    txtCTRL_VAL_NM4.Value = "";
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
  
        //관리항목5
        private void txtCTRL_VAL5_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (strCTRL_CD[5] == "V1" || strCTRL_CD[5] == "V7")
                {
                    string lgsText;
                    lgsText = txtCTRL_VAL5.Text.Replace(",", ""); //** 숫자변환시 콤마로 발생하는 에러방지...
                    if (lgsText != "")
                    {
                        txtCTRL_VAL5.Text = String.Format("{0:#,##0}", Convert.ToDouble(lgsText));
                        txtCTRL_VAL5.SelectionStart = txtCTRL_VAL5.TextLength; //** 캐럿을 맨 뒤로 보낸다...
                        txtCTRL_VAL5.SelectionLength = 0;
                    }
                }
                else if (strCTRL_CD[5] == "BA" || strCTRL_CD[5] == "BK" || strCTRL_CD[5] == "CP"
                      || strCTRL_CD[5] == "D1" || strCTRL_CD[5] == "L1" || strCTRL_CD[5] == "NN"
                      || strCTRL_CD[5] == "V5" || strCTRL_CD[5] == "X1" || strCTRL_CD[1] == "X2" || strCTRL_CD[5] == "V5"
                      || strCTRL_CD[5] == "BP" || strCTRL_CD[5] == "V6" || strCTRL_CD[5] == "MK" || strCTRL_CD[5] == "V4"
                      || strCTRL_CD[2] == "PN")     // 2015.03.23. hma 추가
                {
                    DataTable dt = SLIP_DETAIL_VALUE_CHECK(5, txtCTRL_VAL5, "01");
                    if (dt.Rows.Count > 0)
                    {
                        txtCTRL_VAL_NM5.Value = dt.Rows[0]["NAME"].ToString();
                        if (strCTRL_CD[5] == "V4")
                        {
                            if (strCTRL_CD[1] == "V7") txtCTRL_VAL1.Value = dt.Rows[0]["REL_CD1"].ToString();
                            else if (strCTRL_CD[2] == "V7") txtCTRL_VAL2.Value = dt.Rows[0]["REL_CD1"].ToString();
                            else if (strCTRL_CD[3] == "V7") txtCTRL_VAL3.Value = dt.Rows[0]["REL_CD1"].ToString();
                            else if (strCTRL_CD[4] == "V7") txtCTRL_VAL4.Value = dt.Rows[0]["REL_CD1"].ToString();
                            else if (strCTRL_CD[6] == "V7") txtCTRL_VAL6.Value = dt.Rows[0]["REL_CD1"].ToString();
                            else if (strCTRL_CD[7] == "V7") txtCTRL_VAL7.Value = dt.Rows[0]["REL_CD1"].ToString();
                            else if (strCTRL_CD[8] == "V7") txtCTRL_VAL8.Value = dt.Rows[0]["REL_CD1"].ToString();
                        }
                    }
                    else
                    {
                        txtCTRL_VAL_NM5.Value = "";
                    }
                }
                else
                {
                    txtCTRL_VAL_NM5.Value = "";
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
 
        //관리항목6
        private void txtCTRL_VAL6_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (strCTRL_CD[6] == "V1" || strCTRL_CD[6] == "V7")
                {
                    string lgsText;
                    lgsText = txtCTRL_VAL6.Text.Replace(",", ""); //** 숫자변환시 콤마로 발생하는 에러방지...
                    if (lgsText != "")
                    {
                        txtCTRL_VAL6.Text = String.Format("{0:#,##0}", Convert.ToDouble(lgsText));
                        txtCTRL_VAL6.SelectionStart = txtCTRL_VAL6.TextLength; //** 캐럿을 맨 뒤로 보낸다...
                        txtCTRL_VAL6.SelectionLength = 0;
                    }
                }
                else if (strCTRL_CD[6] == "BA" || strCTRL_CD[6] == "BK" || strCTRL_CD[6] == "CP"
                      || strCTRL_CD[6] == "D1" || strCTRL_CD[6] == "L1" || strCTRL_CD[6] == "NN"
                      || strCTRL_CD[6] == "V5" || strCTRL_CD[6] == "X1" || strCTRL_CD[1] == "X2" || strCTRL_CD[6] == "V5"
                      || strCTRL_CD[6] == "BP" || strCTRL_CD[6] == "V6" || strCTRL_CD[6] == "MK" || strCTRL_CD[6] == "V4"
                      || strCTRL_CD[2] == "PN")     // 2015.03.23. hma 추가
                {
                    DataTable dt = SLIP_DETAIL_VALUE_CHECK(6, txtCTRL_VAL6, "01");
                    if (dt.Rows.Count > 0)
                    {
                        txtCTRL_VAL_NM6.Value = dt.Rows[0]["NAME"].ToString();
                        if (strCTRL_CD[6] == "V4")
                        {
                            if (strCTRL_CD[1] == "V7") txtCTRL_VAL1.Value = dt.Rows[0]["REL_CD1"].ToString();
                            else if (strCTRL_CD[2] == "V7") txtCTRL_VAL2.Value = dt.Rows[0]["REL_CD1"].ToString();
                            else if (strCTRL_CD[3] == "V7") txtCTRL_VAL3.Value = dt.Rows[0]["REL_CD1"].ToString();
                            else if (strCTRL_CD[4] == "V7") txtCTRL_VAL4.Value = dt.Rows[0]["REL_CD1"].ToString();
                            else if (strCTRL_CD[5] == "V7") txtCTRL_VAL5.Value = dt.Rows[0]["REL_CD1"].ToString();
                            else if (strCTRL_CD[7] == "V7") txtCTRL_VAL7.Value = dt.Rows[0]["REL_CD1"].ToString();
                            else if (strCTRL_CD[8] == "V7") txtCTRL_VAL8.Value = dt.Rows[0]["REL_CD1"].ToString();
                        }
                    }
                    else
                    {
                        txtCTRL_VAL_NM6.Value = "";
                    }
                }
                else
                {
                    txtCTRL_VAL_NM6.Value = "";
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
  
        //관리항목7
        private void txtCTRL_VAL7_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (strCTRL_CD[7] == "V1" || strCTRL_CD[7] == "V7")
                {
                    string lgsText;
                    lgsText = txtCTRL_VAL7.Text.Replace(",", ""); //** 숫자변환시 콤마로 발생하는 에러방지...
                    if (lgsText != "")
                    {
                        txtCTRL_VAL7.Text = String.Format("{0:#,##0}", Convert.ToDouble(lgsText));
                        txtCTRL_VAL7.SelectionStart = txtCTRL_VAL7.TextLength; //** 캐럿을 맨 뒤로 보낸다...
                        txtCTRL_VAL7.SelectionLength = 0;
                    }
                }
                else if (strCTRL_CD[7] == "BA" || strCTRL_CD[7] == "BK" || strCTRL_CD[7] == "CP"
                      || strCTRL_CD[7] == "D1" || strCTRL_CD[7] == "L1" || strCTRL_CD[7] == "NN"
                      || strCTRL_CD[7] == "V5" || strCTRL_CD[7] == "X1" || strCTRL_CD[1] == "X2" || strCTRL_CD[7] == "V5"
                      || strCTRL_CD[7] == "BP" || strCTRL_CD[7] == "V6" || strCTRL_CD[7] == "MK" || strCTRL_CD[7] == "V4"
                      || strCTRL_CD[2] == "PN")     // 2015.03.23. hma 추가
                {
                    DataTable dt = SLIP_DETAIL_VALUE_CHECK(7, txtCTRL_VAL7, "01");
                    if (dt.Rows.Count > 0)
                    {
                        txtCTRL_VAL_NM7.Value = dt.Rows[0]["NAME"].ToString();
                        if (strCTRL_CD[7] == "V4")
                        {
                            if (strCTRL_CD[1] == "V7") txtCTRL_VAL1.Value = dt.Rows[0]["REL_CD1"].ToString();
                            else if (strCTRL_CD[2] == "V7") txtCTRL_VAL2.Value = dt.Rows[0]["REL_CD1"].ToString();
                            else if (strCTRL_CD[3] == "V7") txtCTRL_VAL3.Value = dt.Rows[0]["REL_CD1"].ToString();
                            else if (strCTRL_CD[4] == "V7") txtCTRL_VAL4.Value = dt.Rows[0]["REL_CD1"].ToString();
                            else if (strCTRL_CD[5] == "V7") txtCTRL_VAL5.Value = dt.Rows[0]["REL_CD1"].ToString();
                            else if (strCTRL_CD[6] == "V7") txtCTRL_VAL6.Value = dt.Rows[0]["REL_CD1"].ToString();
                            else if (strCTRL_CD[8] == "V7") txtCTRL_VAL8.Value = dt.Rows[0]["REL_CD1"].ToString();
                        }
                    }
                    else
                    {
                        txtCTRL_VAL_NM7.Value = "";
                    }
                }
                else
                {
                    txtCTRL_VAL_NM7.Value = "";
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
  
        //관리항목8
        private void txtCTRL_VAL8_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (strCTRL_CD[8] == "V1" || strCTRL_CD[8] == "V7")
                {
                    string lgsText;
                    lgsText = txtCTRL_VAL8.Text.Replace(",", ""); //** 숫자변환시 콤마로 발생하는 에러방지...
                    if (lgsText != "")
                    {
                        txtCTRL_VAL8.Text = String.Format("{0:#,##0}", Convert.ToDouble(lgsText));
                        txtCTRL_VAL8.SelectionStart = txtCTRL_VAL8.TextLength; //** 캐럿을 맨 뒤로 보낸다...
                        txtCTRL_VAL8.SelectionLength = 0;
                    }
                }
                else if (strCTRL_CD[8] == "BA" || strCTRL_CD[8] == "BK" || strCTRL_CD[8] == "CP"
                      || strCTRL_CD[8] == "D1" || strCTRL_CD[8] == "L1" || strCTRL_CD[8] == "NN"
                      || strCTRL_CD[8] == "V5" || strCTRL_CD[8] == "X1" || strCTRL_CD[1] == "X2" || strCTRL_CD[8] == "V5"
                      || strCTRL_CD[8] == "BP" || strCTRL_CD[8] == "V6" || strCTRL_CD[8] == "MK" || strCTRL_CD[8] == "V4"
                      || strCTRL_CD[2] == "PN")     // 2015.03.23. hma 추가
                {
                    DataTable dt = SLIP_DETAIL_VALUE_CHECK(8, txtCTRL_VAL8, "01");
                    if (dt.Rows.Count > 0)
                    {
                        txtCTRL_VAL_NM8.Value = dt.Rows[0]["NAME"].ToString();
                        if (strCTRL_CD[8] == "V4")
                        {
                            if (strCTRL_CD[1] == "V7") txtCTRL_VAL1.Value = dt.Rows[0]["REL_CD1"].ToString();
                            else if (strCTRL_CD[2] == "V7") txtCTRL_VAL2.Value = dt.Rows[0]["REL_CD1"].ToString();
                            else if (strCTRL_CD[3] == "V7") txtCTRL_VAL3.Value = dt.Rows[0]["REL_CD1"].ToString();
                            else if (strCTRL_CD[4] == "V7") txtCTRL_VAL4.Value = dt.Rows[0]["REL_CD1"].ToString();
                            else if (strCTRL_CD[5] == "V7") txtCTRL_VAL5.Value = dt.Rows[0]["REL_CD1"].ToString();
                            else if (strCTRL_CD[6] == "V7") txtCTRL_VAL6.Value = dt.Rows[0]["REL_CD1"].ToString();
                            else if (strCTRL_CD[7] == "V7") txtCTRL_VAL7.Value = dt.Rows[0]["REL_CD1"].ToString();
                        }
                    }
                    else
                    {
                        txtCTRL_VAL_NM8.Value = "";
                    }
                }


                else
                {
                    txtCTRL_VAL_NM8.Value = "";
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //전표일자
        private void dtpSlipDt_TextChanged(object sender, EventArgs e)
        {
            try
            {
                REORG_ID_CHECK(dtpSlipDt.Text, txtDeptCd, txtDeptNm);
                REORG_ID_CHECK(dtpSlipDt.Text, txtInputDeptCd, txtInputDeptNm);
                ESTIMATE_SET();
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //부서개편ID 체크
        protected void REORG_ID_CHECK(string SLIP_DT, C1.Win.C1Input.C1TextBox DEPT_CD, C1.Win.C1Input.C1TextBox DEPT_NM)
        {
            try
            {
                if (SLIP_DT.Length != 10)
                {
                    strREORG_ID = "";
                    DEPT_NM.Value = "";
                    return;
                }
                string strQuery = " usp_ACD001  'P5'";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                strQuery = strQuery + ", @pSLIP_DT ='" + SLIP_DT + "' ";
                strQuery = strQuery + ", @pDEPT_CD ='" + DEPT_CD.Text + "' ";
                DataSet ds = SystemBase.DbOpen.NoTranDataSet(strQuery);
                if (ds.Tables.Count == 2)
                {
                    if (ds.Tables[1].Rows.Count > 0)
                    {
                        strREORG_ID = ds.Tables[1].Rows[0]["REORG_ID"].ToString();
                    }
                    else
                    {
                        strREORG_ID = "";
                    }
                }
                if (ds.Tables[0].Rows.Count > 0)
                {
                    DEPT_NM.Value = ds.Tables[0].Rows[0]["DEPT_NM"].ToString();
                }
                else
                {
                    DEPT_NM.Value = "";
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //전표형태
        private void cboSlipType_TextChanged(object sender, EventArgs e)
        {
            try
            {
                //입금전표일 경우 대변 만 등록
                if (cboSlipType.SelectedValue.ToString() == "01")
                {
                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {
                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "차대구분")].Text == "DR")
                        {
                            string strSEQ = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "순번")].Text;
                            MessageBox.Show("[순번:" + strSEQ + "]이미 확인한 전표에 차변이 있습니다.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                            cboSlipType.SelectedValue = "03";
                            return;
                        }
                    }
                    optCr.Checked = true;
                    optCr.Tag = ";2;;";
                    optDr.Tag = ";2;;";
                    optCr.Enabled = false;
                    optDr.Enabled = false;
                }
                else if (cboSlipType.SelectedValue.ToString() == "02")
                {
                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {
                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "차대구분")].Text == "CR")
                        {
                            string strSEQ = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "순번")].Text;
                            MessageBox.Show("[순번:" + strSEQ + "]이미 확인한 대표에 차변이 있습니다.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                            cboSlipType.SelectedValue = "03";
                            return;
                        }
                    }
                    optDr.Checked = true;
                    optCr.Tag = ";2;;";
                    optDr.Tag = ";2;;";
                    optCr.Enabled = false;
                    optDr.Enabled = false;
                }
                else
                {
                    optCr.Tag = ";;;";
                    optDr.Tag = ";;;";
                    optCr.Enabled = true;
                    optDr.Enabled = true;
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //화폐단위
        private void cboCurCd_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboCurCd.SelectedValue.ToString() == "KRW")
                {
                    txtExch_Rate.Text = "1";
                    txtExch_Rate.Enabled = false;
                    txtSlipAmtLoc.Enabled = false;
                }
                else
                {
                    txtExch_Rate.Enabled = true;
                    txtSlipAmtLoc.Enabled = true;
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //차대구분
        private void optDr_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                Ctrl_Color_Chk();
                POP_ENABLED();
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 확인버튼 클릭
        private void btnConfirm_Click(object sender, EventArgs e)
        {
            strSaveChk = false;
            CONFIRM_EVENT();
            if (e == null)
            {
                txtInputDeptCd.Focus();
            }
        }

        protected bool CONFIRM_EVENT()
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox3))
                {
                    if (!SLIP_DETAIL_VALUE_CHECK())
                    {
                        this.Cursor = Cursors.Default;
                        return false;
                    }
                    bool Add_Chk = true;
                    int Up_Row = 0;
                    int iMAxRow = 0;

                    if (txtSeq.Text != "")
                    {
                        for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                        {
                            if (txtSeq.Text == fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "순번")].Text)
                            {
                                Add_Chk = false;
                                Up_Row = i;
                                iMAxRow = Convert.ToInt32(txtSeq.Text);
                                if (fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text != "I" && strSaveChk == false)
                                {
                                    fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text = "U";
                                }
                            }
                        }
                    }

                    if (Add_Chk == true)
                    {
                        if (fpSpread1.Sheets[0].Rows.Count == 0)
                            Up_Row = 0;
                        else
                            fpSpread1.Sheets[0].SetActiveCell(fpSpread1.Sheets[0].Rows.Count - 1, SystemBase.Base.GridHeadIndex(GHIdx1, "순번"));

                        UIForm.FPMake.RowInsert(fpSpread1);
                        RowInsExe();
                        Up_Row = fpSpread1.Sheets[0].ActiveRow.Index;


                        for (int i = 0; i < fpSpread1.Sheets[0].RowCount; i++)
                        {
                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "순번")].Text != "")
                            {
                                if (iMAxRow < Convert.ToInt32(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "순번")].Text))
                                {
                                    iMAxRow = Convert.ToInt32(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "순번")].Text);
                                }
                            }
                        }
                        iMAxRow++;
                    }

                    fpSpread1.Sheets[0].Cells[Up_Row, SystemBase.Base.GridHeadIndex(GHIdx1, "순번")].Text = (iMAxRow).ToString();
                    fpSpread1.Sheets[0].Cells[Up_Row, SystemBase.Base.GridHeadIndex(GHIdx1, "귀속부서")].Text = txtInputDeptCd.Text;
                    fpSpread1.Sheets[0].Cells[Up_Row, SystemBase.Base.GridHeadIndex(GHIdx1, "부서명")].Text = txtInputDeptNm.Text;
                    fpSpread1.Sheets[0].Cells[Up_Row, SystemBase.Base.GridHeadIndex(GHIdx1, "계정코드")].Text = txtAcctCd.Text;
                    fpSpread1.Sheets[0].Cells[Up_Row, SystemBase.Base.GridHeadIndex(GHIdx1, "계정명")].Text = txtAcctNm.Text;
                    if (optDr.Checked == true)
                    {
                        fpSpread1.Sheets[0].Cells[Up_Row, SystemBase.Base.GridHeadIndex(GHIdx1, "차변금액(자)")].Text = txtSlipAmtLoc.Text;
                        fpSpread1.Sheets[0].Cells[Up_Row, SystemBase.Base.GridHeadIndex(GHIdx1, "차변금액")].Text = txtSlipAmt.Text;
                        fpSpread1.Sheets[0].Cells[Up_Row, SystemBase.Base.GridHeadIndex(GHIdx1, "대변금액(자)")].Text = "";
                        fpSpread1.Sheets[0].Cells[Up_Row, SystemBase.Base.GridHeadIndex(GHIdx1, "차대구분")].Text = "DR";
                    }
                    else
                    {
                        fpSpread1.Sheets[0].Cells[Up_Row, SystemBase.Base.GridHeadIndex(GHIdx1, "대변금액(자)")].Text = txtSlipAmtLoc.Text;
                        fpSpread1.Sheets[0].Cells[Up_Row, SystemBase.Base.GridHeadIndex(GHIdx1, "대변금액")].Text = txtSlipAmt.Text;
                        fpSpread1.Sheets[0].Cells[Up_Row, SystemBase.Base.GridHeadIndex(GHIdx1, "차변금액(자)")].Text = "";
                        fpSpread1.Sheets[0].Cells[Up_Row, SystemBase.Base.GridHeadIndex(GHIdx1, "차대구분")].Text = "CR";
                    }

                    fpSpread1.Sheets[0].Cells[Up_Row, SystemBase.Base.GridHeadIndex(GHIdx1, "적요")].Text = txtRemark2.Text;

                    fpSpread1.Sheets[0].Cells[Up_Row, SystemBase.Base.GridHeadIndex(GHIdx1, "전표금액")].Text = txtSlipAmt.Text.Replace(",", "");
                    fpSpread1.Sheets[0].Cells[Up_Row, SystemBase.Base.GridHeadIndex(GHIdx1, "화폐단위")].Text = cboCurCd.SelectedValue.ToString();
                    fpSpread1.Sheets[0].Cells[Up_Row, SystemBase.Base.GridHeadIndex(GHIdx1, "환율")].Text = txtExch_Rate.Text.Replace(",", "");
                    fpSpread1.Sheets[0].Cells[Up_Row, SystemBase.Base.GridHeadIndex(GHIdx1, "전표금액(자국)")].Text = txtSlipAmtLoc.Text.Replace(",", "");
                    fpSpread1.Sheets[0].Cells[Up_Row, SystemBase.Base.GridHeadIndex(GHIdx1, "관리항목1코드")].Text = strCTRL_CD[1];
                    fpSpread1.Sheets[0].Cells[Up_Row, SystemBase.Base.GridHeadIndex(GHIdx1, "관리항목2코드")].Text = strCTRL_CD[2];
                    fpSpread1.Sheets[0].Cells[Up_Row, SystemBase.Base.GridHeadIndex(GHIdx1, "관리항목3코드")].Text = strCTRL_CD[3];
                    fpSpread1.Sheets[0].Cells[Up_Row, SystemBase.Base.GridHeadIndex(GHIdx1, "관리항목4코드")].Text = strCTRL_CD[4];
                    fpSpread1.Sheets[0].Cells[Up_Row, SystemBase.Base.GridHeadIndex(GHIdx1, "관리항목5코드")].Text = strCTRL_CD[5];
                    fpSpread1.Sheets[0].Cells[Up_Row, SystemBase.Base.GridHeadIndex(GHIdx1, "관리항목6코드")].Text = strCTRL_CD[6];
                    fpSpread1.Sheets[0].Cells[Up_Row, SystemBase.Base.GridHeadIndex(GHIdx1, "관리항목7코드")].Text = strCTRL_CD[7];
                    fpSpread1.Sheets[0].Cells[Up_Row, SystemBase.Base.GridHeadIndex(GHIdx1, "관리항목8코드")].Text = strCTRL_CD[8];

                    fpSpread1.Sheets[0].Cells[Up_Row, SystemBase.Base.GridHeadIndex(GHIdx1, "관리항목1")].Text = txtCTRL_VAL1.Text;
                    fpSpread1.Sheets[0].Cells[Up_Row, SystemBase.Base.GridHeadIndex(GHIdx1, "관리항목2")].Text = txtCTRL_VAL2.Text;
                    fpSpread1.Sheets[0].Cells[Up_Row, SystemBase.Base.GridHeadIndex(GHIdx1, "관리항목3")].Text = txtCTRL_VAL3.Text;
                    fpSpread1.Sheets[0].Cells[Up_Row, SystemBase.Base.GridHeadIndex(GHIdx1, "관리항목4")].Text = txtCTRL_VAL4.Text;
                    fpSpread1.Sheets[0].Cells[Up_Row, SystemBase.Base.GridHeadIndex(GHIdx1, "관리항목5")].Text = txtCTRL_VAL5.Text;
                    fpSpread1.Sheets[0].Cells[Up_Row, SystemBase.Base.GridHeadIndex(GHIdx1, "관리항목6")].Text = txtCTRL_VAL6.Text;
                    fpSpread1.Sheets[0].Cells[Up_Row, SystemBase.Base.GridHeadIndex(GHIdx1, "관리항목7")].Text = txtCTRL_VAL7.Text;
                    fpSpread1.Sheets[0].Cells[Up_Row, SystemBase.Base.GridHeadIndex(GHIdx1, "관리항목8")].Text = txtCTRL_VAL8.Text;

                    fpSpread1.Sheets[0].Cells[Up_Row, SystemBase.Base.GridHeadIndex(GHIdx1, "관리항목1명")].Text = txtCTRL_VAL_NM1.Text;
                    fpSpread1.Sheets[0].Cells[Up_Row, SystemBase.Base.GridHeadIndex(GHIdx1, "관리항목2명")].Text = txtCTRL_VAL_NM2.Text;
                    fpSpread1.Sheets[0].Cells[Up_Row, SystemBase.Base.GridHeadIndex(GHIdx1, "관리항목3명")].Text = txtCTRL_VAL_NM3.Text;
                    fpSpread1.Sheets[0].Cells[Up_Row, SystemBase.Base.GridHeadIndex(GHIdx1, "관리항목4명")].Text = txtCTRL_VAL_NM4.Text;
                    fpSpread1.Sheets[0].Cells[Up_Row, SystemBase.Base.GridHeadIndex(GHIdx1, "관리항목5명")].Text = txtCTRL_VAL_NM5.Text;
                    fpSpread1.Sheets[0].Cells[Up_Row, SystemBase.Base.GridHeadIndex(GHIdx1, "관리항목6명")].Text = txtCTRL_VAL_NM6.Text;
                    fpSpread1.Sheets[0].Cells[Up_Row, SystemBase.Base.GridHeadIndex(GHIdx1, "관리항목7명")].Text = txtCTRL_VAL_NM7.Text;
                    fpSpread1.Sheets[0].Cells[Up_Row, SystemBase.Base.GridHeadIndex(GHIdx1, "관리항목8명")].Text = txtCTRL_VAL_NM8.Text;

                    fpSpread1.Sheets[0].SetActiveCell(fpSpread1.Sheets[0].RowCount - 1, SystemBase.Base.GridHeadIndex(GHIdx1, "순번"));



                    if (Ar_Dt_Temp != null)
                    {
                        for (int iSelRow = 0; iSelRow < Ar_Dt.Rows.Count; iSelRow++)
                        {
                            if (Ar_Dt.Rows[iSelRow].RowState.ToString() != "Deleted")
                            {
                                if (Ar_Dt.Rows[iSelRow]["SLIP_SEQ"].ToString() == iMAxRow.ToString())
                                {
                                    if (Ar_Dt.Rows[iSelRow].RowState.ToString() != "Added")
                                    {
                                        Ar_Dt.Rows[iSelRow].Delete();
                                    }
                                    else
                                    {
                                        Ar_Dt.Rows[iSelRow].Delete();
                                        iSelRow--;
                                    }
                                }
                            }
                        }

                        for (int iRow = 0; iRow < Ar_Dt_Temp.Rows.Count; iRow++)
                        {
                            Ar_Dt_Temp.Rows[iRow]["SLIP_SEQ"] = (iMAxRow).ToString();

                            DataRow Tr = Ar_Dt.NewRow();
                            DataRow Dr = Ar_Dt_Temp.Rows[iRow];
                            for (int i = 0; i < Ar_Dt.Columns.Count; i++)
                            {
                                Tr[i] = Dr[i];
                            }
                            Ar_Dt.Rows.Add(Tr);
                        }
                    }

                    if (Ap_Dt_Temp != null)
                    {
                        for (int iSelRow = 0; iSelRow < Ap_Dt.Rows.Count; iSelRow++)
                        {
                            if (Ap_Dt.Rows[iSelRow].RowState.ToString() != "Deleted")
                            {
                                if (Ap_Dt.Rows[iSelRow]["SLIP_SEQ"].ToString() == iMAxRow.ToString())
                                {
                                    if (Ap_Dt.Rows[iSelRow].RowState.ToString() != "Added")
                                    {
                                        Ap_Dt.Rows[iSelRow].Delete();
                                    }
                                    else
                                    {
                                        Ap_Dt.Rows[iSelRow].Delete();
                                        iSelRow--;
                                    }
                                }
                            }
                        }

                        for (int iRow = 0; iRow < Ap_Dt_Temp.Rows.Count; iRow++)
                        {
                            Ap_Dt_Temp.Rows[iRow]["SLIP_SEQ"] = (iMAxRow).ToString();

                            DataRow Tr = Ap_Dt.NewRow();
                            DataRow Dr = Ap_Dt_Temp.Rows[iRow];
                            for (int i = 0; i < Ap_Dt.Columns.Count; i++)
                            {
                                Tr[i] = Dr[i];
                            }
                            Ap_Dt.Rows.Add(Tr);
                        }
                    }

                    if (Loan_Dt_Temp != null)
                    {
                        for (int iSelRow = 0; iSelRow < Loan_Dt.Rows.Count; iSelRow++)
                        {
                            if (Loan_Dt.Rows[iSelRow].RowState.ToString() != "Deleted")
                            {
                                if (Loan_Dt.Rows[iSelRow]["SLIP_SEQ"].ToString() == iMAxRow.ToString())
                                {
                                    if (Loan_Dt.Rows[iSelRow].RowState.ToString() != "Added")
                                    {
                                        Loan_Dt.Rows[iSelRow].Delete();
                                    }
                                    else
                                    {
                                        Loan_Dt.Rows[iSelRow].Delete();
                                        iSelRow--;
                                    }
                                }
                            }
                        }

                        for (int iRow = 0; iRow < Loan_Dt_Temp.Rows.Count; iRow++)
                        {
                            Loan_Dt_Temp.Rows[iRow]["SLIP_SEQ"] = (iMAxRow).ToString();

                            DataRow Tr = Loan_Dt.NewRow();
                            DataRow Dr = Loan_Dt_Temp.Rows[iRow];
                            for (int i = 0; i < Loan_Dt.Columns.Count; i++)
                            {
                                Tr[i] = Dr[i];
                            }
                            Loan_Dt.Rows.Add(Tr);
                        }
                    }

                    if (Asset_Dt_Temp != null)
                    {
                        for (int iSelRow = 0; iSelRow < Asset_Dt.Rows.Count; iSelRow++)
                        {
                            if (Asset_Dt.Rows[iSelRow].RowState.ToString() != "Deleted")
                            {
                                if (Asset_Dt.Rows[iSelRow]["SLIP_SEQ"].ToString() == iMAxRow.ToString())
                                {
                                    if (Asset_Dt.Rows[iSelRow].RowState.ToString() != "Added")
                                    {
                                        Asset_Dt.Rows[iSelRow].Delete();
                                    }
                                    else
                                    {
                                        Asset_Dt.Rows[iSelRow].Delete();
                                        iSelRow--;
                                    }
                                }
                            }
                        }

                        for (int iRow = 0; iRow < Asset_Dt_Temp.Rows.Count; iRow++)
                        {
                            Asset_Dt_Temp.Rows[iRow]["SLIP_SEQ"] = (iMAxRow).ToString();
                            Asset_Dt_Temp.Rows[iRow]["REORG_ID"] = strREORG_ID;
                            Asset_Dt_Temp.Rows[iRow]["DEPT_CD"] = txtInputDeptCd.Text;

                            DataRow Tr = Asset_Dt.NewRow();
                            DataRow Dr = Asset_Dt_Temp.Rows[iRow];
                            for (int i = 0; i < Asset_Dt.Columns.Count; i++)
                            {
                                Tr[i] = Dr[i];
                            }
                            Asset_Dt.Rows.Add(Tr);
                        }
                    }

                    Detail_New();
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            this.Cursor = Cursors.Default;
            return true;
        }
        #endregion

        #region 전표 차변 대변 합계금액 셋팅
        protected void SLIP_AMT_SUM()
        {
            double DR_AMT = 0;
            double CR_AMT = 0;
            double DR_AMT_LOC = 0;
            double CR_AMT_LOC = 0;

            for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
            {
                if (fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text != "D")
                {
                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "차대구분")].Text == "DR")
                    {
                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "전표금액")].Text != "")
                            DR_AMT += Convert.ToDouble(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "전표금액")].Text.Replace(",", ""));
                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "전표금액(자국)")].Text != "")
                            DR_AMT_LOC += Convert.ToDouble(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "전표금액(자국)")].Text.Replace(",", ""));
                    }
                    else
                    {
                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "전표금액")].Text != "")
                            CR_AMT += Convert.ToDouble(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "전표금액")].Text.Replace(",", ""));
                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "전표금액(자국)")].Text != "")
                            CR_AMT_LOC += Convert.ToDouble(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "전표금액(자국)")].Text.Replace(",", ""));
                    }
                }
            }
            txtDrAmt.Value = DR_AMT.ToString();
            txtCrAmt.Value = CR_AMT.ToString();
            txtDrAmtLoc.Value = DR_AMT_LOC.ToString();
            txtCrAmtLoc.Value = CR_AMT_LOC.ToString();
        }
        #endregion

        #region 전표 차변 대변 합계금액 셋팅
        protected void ESTIMATE_SET()
        {
            try
            {
                if (dtpSlipDt.Text != "" && strREORG_ID != "" && txtInputDeptNm.Text != "" && txtAcctNm.Text != "")
                {
                    string strQuery = " usp_ACD001  'S4'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    strQuery = strQuery + ", @pSLIP_DT ='" + dtpSlipDt.Text + "' ";
                    strQuery = strQuery + ", @pREORG_ID ='" + strREORG_ID + "' ";
                    strQuery = strQuery + ", @pDEPT_CD ='" + txtInputDeptCd.Text + "' ";
                    strQuery = strQuery + ", @pACCT_CD ='" + txtAcctCd.Text + "' ";
                    DataSet ds = SystemBase.DbOpen.NoTranDataSet(strQuery);
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        txtEstAmt.Value = ds.Tables[0].Rows[0]["EST_AMT"].ToString();
                        txtEstBalance.Value = ds.Tables[0].Rows[0]["EST_BAL_AMT"].ToString();
                    }
                    else
                    {
                        txtEstAmt.Value = 0;
                        txtEstBalance.Value = 0;
                    }
                }
                else
                {
                    txtEstAmt.Value = 0;
                    txtEstBalance.Value = 0;
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 관리항목 마지막 값 확인
        protected bool SLIP_DETAIL_CHECK(string TEXT_NAME)
        {
            try
            {
                int iCTRL_Seq = 0;
                if (TEXT_NAME == "txtRemark2") iCTRL_Seq = 0;
                else if (TEXT_NAME == "txtCTRL_VAL1") iCTRL_Seq = 1;
                else if (TEXT_NAME == "txtCTRL_VAL2") iCTRL_Seq = 2;
                else if (TEXT_NAME == "txtCTRL_VAL3") iCTRL_Seq = 3;
                else if (TEXT_NAME == "txtCTRL_VAL4") iCTRL_Seq = 4;
                else if (TEXT_NAME == "txtCTRL_VAL5") iCTRL_Seq = 5;
                else if (TEXT_NAME == "txtCTRL_VAL6") iCTRL_Seq = 6;
                else if (TEXT_NAME == "txtCTRL_VAL7") iCTRL_Seq = 7;
                else if (TEXT_NAME == "txtCTRL_VAL8") iCTRL_Seq = 8;

                if (txtAcctNm.Text == "")
                {
                    return false;
                }
                string strReturnValue = "";
                string strThisValue = "";
                strThisValue = strCTRL_NULL[iCTRL_Seq];
                for (int iSeq = iCTRL_Seq + 1; iSeq < strCTRL_NULL.Length; iSeq++)
                {
                    strReturnValue += strCTRL_NULL[iSeq];
                }
                if (strReturnValue == "" && strThisValue != "")
                {
                    return true;
                }
                return false;
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
        #endregion

        #region 전표상세 값 체크
        protected bool SLIP_DETAIL_VALUE_CHECK()
        {
            try
            {
                if (txtInputDeptNm.Text == "")
                {
                    MessageBox.Show("없는 귀속부서입니다.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtInputDeptCd.Focus();
                    return false;
                }
                if (txtAcctNm.Text == "")
                {
                    MessageBox.Show("없는 계정코드입니다.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtAcctCd.Focus();
                    return false;
                }
                DataTable dt = SLIP_DETAIL_VALUE_CHECK(1, txtCTRL_VAL1, "02");
                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("[" + c1Label_Crtl1.Text + "] 없는 정보입니다.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtCTRL_VAL1.Focus();
                    return false;
                }
                dt = SLIP_DETAIL_VALUE_CHECK(2, txtCTRL_VAL2, "02");
                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("[" + c1Label_Crtl2.Text + "] 없는 정보입니다.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtCTRL_VAL2.Focus();
                    return false;
                }
                dt = SLIP_DETAIL_VALUE_CHECK(3, txtCTRL_VAL3, "02");
                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("[" + c1Label_Crtl3.Text + "] 없는 정보입니다.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtCTRL_VAL3.Focus();
                    return false;
                }
                dt = SLIP_DETAIL_VALUE_CHECK(4, txtCTRL_VAL4, "02");
                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("[" + c1Label_Crtl4.Text + "] 없는 정보입니다.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtCTRL_VAL4.Focus();
                    return false;
                }
                dt = SLIP_DETAIL_VALUE_CHECK(5, txtCTRL_VAL5, "02");
                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("[" + c1Label_Crtl5.Text + "] 없는 정보입니다.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtCTRL_VAL5.Focus();
                    return false;
                }
                dt = SLIP_DETAIL_VALUE_CHECK(6, txtCTRL_VAL6, "02");
                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("[" + c1Label_Crtl6.Text + "] 없는 정보입니다.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtCTRL_VAL6.Focus();
                    return false;
                }
                dt = SLIP_DETAIL_VALUE_CHECK(7, txtCTRL_VAL7, "02");
                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("[" + c1Label_Crtl7.Text + "] 없는 정보입니다.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtCTRL_VAL7.Focus();
                    return false;
                }
                dt = SLIP_DETAIL_VALUE_CHECK(8, txtCTRL_VAL8, "02");
                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("[" + c1Label_Crtl8.Text + "] 없는 정보입니다.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtCTRL_VAL8.Focus();
                    return false;
                }

                if (strSubType == "AR" && optCr.Checked == true)
                {
                    if (Ar_Dt_Temp == null)
                    {
                        MessageBox.Show("채권반제 정보가 없습니다.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                    else if (Ar_Dt_Temp.Rows.Count == 0)
                    {
                        MessageBox.Show("채권반제 정보가 없습니다.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }
                else if (strSubType == "AP" && optDr.Checked == true) //채무(EX:21010001)
                {
                    if (Ap_Dt_Temp == null)
                    {
                        MessageBox.Show("채무반제 정보가 없습니다.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                    else if (Ap_Dt_Temp.Rows.Count == 0)
                    {
                        MessageBox.Show("채무반제 정보가 없습니다.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }
                else if (strSubType == "LN" && optCr.Checked == true) //차입금(EX:)
                {
                    if (Loan_Dt_Temp == null)
                    {
                        MessageBox.Show("차입금 정보가 없습니다.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                    else if (Loan_Dt_Temp.Rows.Count == 0)
                    {
                        MessageBox.Show("차입금 정보가 없습니다.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }
                else if (strSubType == "AS" && optDr.Checked == true) //고정자산정보(EX:)
                {
                    if (Asset_Dt_Temp == null)
                    {
                        MessageBox.Show("고정자산정보 정보가 없습니다.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                    else if (Asset_Dt_Temp.Rows.Count == 0)
                    {
                        MessageBox.Show("고정자산정보 정보가 없습니다.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }

                //채권반제정보 체크
                if (Ar_Dt_Temp != null)
                {
                    if (Ar_Dt_Temp.Rows.Count > 0)
                    {
                        Decimal dCls_Amt = 0;
                        Decimal dCls_Amt_Loc = 0;

                        string strCust_Cd = "";
                        if (strCTRL_CD[1] == "BP" || strCTRL_CD[1] == "V6") strCust_Cd = txtCTRL_VAL1.Text;
                        else if (strCTRL_CD[2] == "BP" || strCTRL_CD[2] == "V6") strCust_Cd = txtCTRL_VAL2.Text;
                        else if (strCTRL_CD[3] == "BP" || strCTRL_CD[3] == "V6") strCust_Cd = txtCTRL_VAL3.Text;
                        else if (strCTRL_CD[4] == "BP" || strCTRL_CD[4] == "V6") strCust_Cd = txtCTRL_VAL4.Text;
                        else if (strCTRL_CD[5] == "BP" || strCTRL_CD[5] == "V6") strCust_Cd = txtCTRL_VAL5.Text;
                        else if (strCTRL_CD[6] == "BP" || strCTRL_CD[6] == "V6") strCust_Cd = txtCTRL_VAL6.Text;
                        else if (strCTRL_CD[7] == "BP" || strCTRL_CD[7] == "V6") strCust_Cd = txtCTRL_VAL7.Text;
                        else if (strCTRL_CD[8] == "BP" || strCTRL_CD[8] == "V6") strCust_Cd = txtCTRL_VAL8.Text;

                        for (int iRow = 0; iRow < Ar_Dt_Temp.Rows.Count; iRow++)
                        {
                            if (cboCurCd.SelectedValue.ToString() != Ar_Dt_Temp.Rows[iRow]["CUR_CD"].ToString())
                            {
                                MessageBox.Show("채권반제정보의 화폐단위와 일치하지 않습니다.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                                cboCurCd.Focus();
                                return false;
                            }
                            if (strCust_Cd != Ar_Dt_Temp.Rows[iRow]["CUST_CD"].ToString())
                            {
                                MessageBox.Show("채권반제정보의 거래처와 일치하지 않습니다.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                                if (strCTRL_CD[1] == "BP" || strCTRL_CD[1] == "V6") txtCTRL_VAL1.Focus();
                                else if (strCTRL_CD[2] == "BP" || strCTRL_CD[2] == "V6") txtCTRL_VAL2.Focus();
                                else if (strCTRL_CD[3] == "BP" || strCTRL_CD[3] == "V6") txtCTRL_VAL3.Focus();
                                else if (strCTRL_CD[4] == "BP" || strCTRL_CD[4] == "V6") txtCTRL_VAL4.Focus();
                                else if (strCTRL_CD[5] == "BP" || strCTRL_CD[5] == "V6") txtCTRL_VAL5.Focus();
                                else if (strCTRL_CD[6] == "BP" || strCTRL_CD[6] == "V6") txtCTRL_VAL6.Focus();
                                else if (strCTRL_CD[7] == "BP" || strCTRL_CD[7] == "V6") txtCTRL_VAL7.Focus();
                                else if (strCTRL_CD[8] == "BP" || strCTRL_CD[8] == "V6") txtCTRL_VAL8.Focus();
                                return false;
                            }
                            dCls_Amt += Convert.ToDecimal(Ar_Dt_Temp.Rows[iRow]["CLS_AMT"].ToString());
                            dCls_Amt_Loc += Convert.ToDecimal(Ar_Dt_Temp.Rows[iRow]["CLS_AMT_LOC"].ToString());
                            if (Ar_Dt_Temp.Rows[iRow]["DC_AMT"].ToString() != "") dCls_Amt += Convert.ToDecimal(Ar_Dt_Temp.Rows[iRow]["DC_AMT"].ToString());
                            if (Ar_Dt_Temp.Rows[iRow]["DC_AMT_LOC"].ToString() != "") dCls_Amt_Loc += Convert.ToDecimal(Ar_Dt_Temp.Rows[iRow]["DC_AMT_LOC"].ToString());
                        }

                        if (dCls_Amt != Convert.ToDecimal(SystemBase.Base.SH_DBNULL(txtSlipAmt.Value, 0)))
                        {
                            MessageBox.Show("채권반제정보의 전표금액이 일치하지 않습니다.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                            txtSlipAmt.Focus();
                            return false;
                        }
                        if (cboCurCd.SelectedValue.ToString() == "KRW")
                        {
                            if (dCls_Amt_Loc != Convert.ToDecimal(SystemBase.Base.SH_DBNULL(txtSlipAmtLoc.Value, 0)))
                            {
                                MessageBox.Show("채권반제정보의 전표금액자국이 일치하지 않습니다.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                                txtSlipAmtLoc.Focus();
                                return false;
                            }
                        }
                        if (btnArCls.Enabled == false)
                        {
                            if (MessageBox.Show("임시 채권반제정보를 삭제하시겠습니까?", "확인", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                            {
                                Ar_Dt_Temp.Clear();
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                }

                //채무반제정보 체크
                if (Ap_Dt_Temp != null)
                {
                    if (Ap_Dt_Temp.Rows.Count > 0)
                    {
                        Decimal dCls_Amt = 0;
                        Decimal dCls_Amt_Loc = 0;

                        string strCust_Cd = "";
                        if (strCTRL_CD[1] == "BP" || strCTRL_CD[1] == "V6") strCust_Cd = txtCTRL_VAL1.Text;
                        else if (strCTRL_CD[2] == "BP" || strCTRL_CD[2] == "V6") strCust_Cd = txtCTRL_VAL2.Text;
                        else if (strCTRL_CD[3] == "BP" || strCTRL_CD[3] == "V6") strCust_Cd = txtCTRL_VAL3.Text;
                        else if (strCTRL_CD[4] == "BP" || strCTRL_CD[4] == "V6") strCust_Cd = txtCTRL_VAL4.Text;
                        else if (strCTRL_CD[5] == "BP" || strCTRL_CD[5] == "V6") strCust_Cd = txtCTRL_VAL5.Text;
                        else if (strCTRL_CD[6] == "BP" || strCTRL_CD[6] == "V6") strCust_Cd = txtCTRL_VAL6.Text;
                        else if (strCTRL_CD[7] == "BP" || strCTRL_CD[7] == "V6") strCust_Cd = txtCTRL_VAL7.Text;
                        else if (strCTRL_CD[8] == "BP" || strCTRL_CD[8] == "V6") strCust_Cd = txtCTRL_VAL8.Text;

                        for (int iRow = 0; iRow < Ap_Dt_Temp.Rows.Count; iRow++)
                        {
                            if (cboCurCd.SelectedValue.ToString() != Ap_Dt_Temp.Rows[iRow]["CUR_CD"].ToString())
                            {
                                MessageBox.Show("채무반제정보의 화폐단위와 일치하지 않습니다.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                                cboCurCd.Focus();
                                return false;
                            }
                            if (strCust_Cd != Ap_Dt_Temp.Rows[iRow]["CUST_CD"].ToString())
                            {
                                MessageBox.Show("채무반제정보의 거래처와 일치하지 않습니다.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                                if (strCTRL_CD[1] == "BP" || strCTRL_CD[1] == "V6") txtCTRL_VAL1.Focus();
                                else if (strCTRL_CD[2] == "BP" || strCTRL_CD[2] == "V6") txtCTRL_VAL2.Focus();
                                else if (strCTRL_CD[3] == "BP" || strCTRL_CD[3] == "V6") txtCTRL_VAL3.Focus();
                                else if (strCTRL_CD[4] == "BP" || strCTRL_CD[4] == "V6") txtCTRL_VAL4.Focus();
                                else if (strCTRL_CD[5] == "BP" || strCTRL_CD[5] == "V6") txtCTRL_VAL5.Focus();
                                else if (strCTRL_CD[6] == "BP" || strCTRL_CD[6] == "V6") txtCTRL_VAL6.Focus();
                                else if (strCTRL_CD[7] == "BP" || strCTRL_CD[7] == "V6") txtCTRL_VAL7.Focus();
                                else if (strCTRL_CD[8] == "BP" || strCTRL_CD[8] == "V6") txtCTRL_VAL8.Focus();
                                return false;
                            }
                            dCls_Amt += Convert.ToDecimal(Ap_Dt_Temp.Rows[iRow]["CLS_AMT"].ToString());
                            dCls_Amt_Loc += Convert.ToDecimal(Ap_Dt_Temp.Rows[iRow]["CLS_AMT_LOC"].ToString());
                            if (Ap_Dt_Temp.Rows[iRow]["DC_AMT"].ToString() != "") dCls_Amt += Convert.ToDecimal(Ap_Dt_Temp.Rows[iRow]["DC_AMT"].ToString());
                            if (Ap_Dt_Temp.Rows[iRow]["DC_AMT_LOC"].ToString() != "") dCls_Amt_Loc += Convert.ToDecimal(Ap_Dt_Temp.Rows[iRow]["DC_AMT_LOC"].ToString());
                        }

                        if (dCls_Amt != Convert.ToDecimal(SystemBase.Base.SH_DBNULL(txtSlipAmt.Value, 0)))
                        {
                            MessageBox.Show("채무반제정보의 전표금액이 일치하지 않습니다.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                            txtSlipAmt.Focus();
                            return false;
                        }
                        if (cboCurCd.SelectedValue.ToString() == "KRW")
                        {
                            if (dCls_Amt_Loc != Convert.ToDecimal(SystemBase.Base.SH_DBNULL(txtSlipAmtLoc.Value, 0)))
                            {
                                MessageBox.Show("채무반제정보의 전표금액자국이 일치하지 않습니다.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                                txtSlipAmtLoc.Focus();
                                return false;
                            }
                        }
                        if (btnApCls.Enabled == false)
                        {
                            if (MessageBox.Show("임시 채무반제정보를 삭제하시겠습니까?", "확인", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                            {
                                Ap_Dt_Temp.Clear();
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                }

                //차입금정보 체크
                if (Loan_Dt_Temp != null)
                {
                    if (Loan_Dt_Temp.Rows.Count > 0)
                    {
                        Decimal dLoan_Amt = 0;
                        Decimal dLoan_Amt_Loc = 0;

                        string strLoan_Cd = "";
                        if (strCTRL_CD[1] == "L1") strLoan_Cd = txtCTRL_VAL1.Text;
                        else if (strCTRL_CD[2] == "L1") strLoan_Cd = txtCTRL_VAL2.Text;
                        else if (strCTRL_CD[3] == "L1") strLoan_Cd = txtCTRL_VAL3.Text;
                        else if (strCTRL_CD[4] == "L1") strLoan_Cd = txtCTRL_VAL4.Text;
                        else if (strCTRL_CD[5] == "L1") strLoan_Cd = txtCTRL_VAL5.Text;
                        else if (strCTRL_CD[6] == "L1") strLoan_Cd = txtCTRL_VAL6.Text;
                        else if (strCTRL_CD[7] == "L1") strLoan_Cd = txtCTRL_VAL7.Text;
                        else if (strCTRL_CD[8] == "L1") strLoan_Cd = txtCTRL_VAL8.Text;

                        for (int iRow = 0; iRow < Loan_Dt_Temp.Rows.Count; iRow++)
                        {
                            if (cboCurCd.SelectedValue.ToString() != Loan_Dt_Temp.Rows[iRow]["CUR_CD"].ToString())
                            {
                                MessageBox.Show("차입금정보의 화폐단위와 일치하지 않습니다.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                                cboCurCd.Focus();
                                return false;
                            }
                            if (strLoan_Cd != Loan_Dt_Temp.Rows[iRow]["LOAN_NO"].ToString())
                            {
                                MessageBox.Show("차입번호가 일치하지 않습니다.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                                if (strCTRL_CD[1] == "L1") txtCTRL_VAL1.Focus();
                                else if (strCTRL_CD[2] == "L1") txtCTRL_VAL2.Focus();
                                else if (strCTRL_CD[3] == "L1") txtCTRL_VAL3.Focus();
                                else if (strCTRL_CD[4] == "L1") txtCTRL_VAL4.Focus();
                                else if (strCTRL_CD[5] == "L1") txtCTRL_VAL5.Focus();
                                else if (strCTRL_CD[6] == "L1") txtCTRL_VAL6.Focus();
                                else if (strCTRL_CD[7] == "L1") txtCTRL_VAL7.Focus();
                                else if (strCTRL_CD[8] == "L1") txtCTRL_VAL8.Focus();
                                return false;
                            }
                            dLoan_Amt += Convert.ToDecimal(Loan_Dt_Temp.Rows[iRow]["LOAN_AMT"].ToString());
                            dLoan_Amt_Loc += Convert.ToDecimal(Loan_Dt_Temp.Rows[iRow]["LOAN_AMT_LOC"].ToString());
                        }

                        if (dLoan_Amt != Convert.ToDecimal(SystemBase.Base.SH_DBNULL(txtSlipAmt.Value, 0)))
                        {
                            MessageBox.Show("차입금정보의 전표금액이 일치하지 않습니다.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                            txtSlipAmt.Focus();
                            return false;
                        }
                        if (cboCurCd.SelectedValue.ToString() == "KRW")
                        {
                            if (dLoan_Amt_Loc != Convert.ToDecimal(SystemBase.Base.SH_DBNULL(txtSlipAmtLoc.Value, 0)))
                            {
                                MessageBox.Show("차입금정보의 전표금액자국이 일치하지 않습니다.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                                txtSlipAmtLoc.Focus();
                                return false;
                            }
                        }
                        if (btnLoan.Enabled == false)
                        {
                            if (MessageBox.Show("임시 차입금정보를 삭제하시겠습니까?", "확인", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                            {
                                Loan_Dt_Temp.Clear();
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                }

                //고정자산정보 체크
                if (Asset_Dt_Temp != null)
                {
                    if (Asset_Dt_Temp.Rows.Count > 0)
                    {
                        Decimal dAsset_Amt = 0;
                        Decimal dAsset_Amt_Loc = 0;

                        for (int iRow = 0; iRow < Asset_Dt_Temp.Rows.Count; iRow++)
                        {
                            if (cboCurCd.SelectedValue.ToString() != Asset_Dt_Temp.Rows[iRow]["CUR_CD"].ToString())
                            {
                                MessageBox.Show("고정자산정보의 화폐단위와 일치하지 않습니다.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                                cboCurCd.Focus();
                                return false;
                            }
                            if (txtAcctCd.Text != Asset_Dt_Temp.Rows[iRow]["ACCT_CD"].ToString())
                            {
                                MessageBox.Show("고정자산정보의 계정코드와 일치하지 않습니다.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                                txtAcctCd.Focus();
                                return false;
                            }
                            dAsset_Amt += Convert.ToDecimal(Asset_Dt_Temp.Rows[iRow]["ASSET_AMT"].ToString());
                            dAsset_Amt_Loc += Convert.ToDecimal(Asset_Dt_Temp.Rows[iRow]["ASSET_AMT_LOC"].ToString());
                        }

                        if (dAsset_Amt != Convert.ToDecimal(SystemBase.Base.SH_DBNULL(txtSlipAmt.Value, 0)))
                        {
                            MessageBox.Show("고정자산정보의 전표금액이 일치하지 않습니다.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                            txtSlipAmt.Focus();
                            return false;
                        }
                        if (cboCurCd.SelectedValue.ToString() == "KRW")
                        {
                            if (dAsset_Amt_Loc != Convert.ToDecimal(SystemBase.Base.SH_DBNULL(txtSlipAmtLoc.Value, 0)))
                            {
                                MessageBox.Show("고정자산정보의 전표금액자국이 일치하지 않습니다.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                                txtSlipAmtLoc.Focus();
                                return false;
                            }
                        }
                        if (btnAsset.Enabled == false)
                        {
                            if (MessageBox.Show("임시 고정자산정보를 삭제하시겠습니까?", "확인", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                            {
                                Asset_Dt_Temp.Clear();
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                }



                return true;
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
        #endregion

        #region 전표상세 관리항목값 체크
        protected DataTable SLIP_DETAIL_VALUE_CHECK(int SEQ, C1.Win.C1Input.C1TextBox CTRL_TEXT, string SAVE_SEARCH_FLAG)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("CODE");
            dt.Columns.Add("NAME");
            try
            {
                bool bCheck = true;
                if (strCTRL_CD[SEQ] == "L1" && strSubType == "LN")
                {
                    if (txtSeq.Text == "")
                    {
                        bCheck = false;
                    }
                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {
                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "순번")].Text == txtSeq.Text && fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text == "I")
                        {
                            bCheck = false;
                        }
                    }
                }
                else if ((/*strAcctType == "D1" && */optDr.Checked == true && (strCTRL_CD[SEQ] == "CP" || strCTRL_CD[SEQ] == "NN")) ||
                         ((strAcctType == "D3" || strAcctType == "CP") && optCr.Checked == true && (strCTRL_CD[SEQ] == "CP" || strCTRL_CD[SEQ] == "NN")))
                {
                    if (txtSeq.Text == "")
                    {
                        bCheck = false;
                    }
                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {
                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "순번")].Text == txtSeq.Text && fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text == "I")
                        {
                            bCheck = false;
                        }
                    }
                }
                else if ((strCTRL_CD[SEQ] == "X1" && optDr.Checked == true) ||
                         (strCTRL_CD[SEQ] == "X2" && optCr.Checked == true))
                {
                    if (txtSeq.Text == "")
                    {
                        bCheck = false;
                    }
                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {
                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "순번")].Text == txtSeq.Text && fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text == "I")
                        {
                            bCheck = false;
                        }
                    }
                }
                if (bCheck == true)
                {
                    if (strCTRL_NULL[SEQ].Trim() == "C")
                    {
                        bCheck = true;
                    }
                    else if (optDr.Checked == true && strCTRL_NULL[SEQ] == "A")
                    {
                        bCheck = true;
                    }
                    else if (optCr.Checked == true && strCTRL_NULL[SEQ] == "B")
                    {
                        bCheck = true;
                    }
                    else if (CTRL_TEXT.Text == "" && SAVE_SEARCH_FLAG == "02")
                    {
                        bCheck = false;
                    }
                }

                if ((strCTRL_CD[SEQ] == "BA" || strCTRL_CD[SEQ] == "BK" || strCTRL_CD[SEQ] == "CP"
                 || /*strCTRL_CD[SEQ] == "D1" || */strCTRL_CD[SEQ] == "L1" || strCTRL_CD[SEQ] == "NN"
                 || strCTRL_CD[SEQ] == "V5" || strCTRL_CD[SEQ] == "X1" || strCTRL_CD[1] == "X2" || strCTRL_CD[SEQ] == "V5"
                 || strCTRL_CD[SEQ] == "BP" || strCTRL_CD[SEQ] == "V6" || strCTRL_CD[SEQ] == "MK" || strCTRL_CD[SEQ] == "V4"
                 || strCTRL_CD[SEQ] == "PN") && bCheck == true)             // 2015.03.23. hma 수정: "PN" 추가
                {
                    string strQuery = " usp_ACD001  'S3'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    strQuery = strQuery + ", @pCTRL_CD ='" + strCTRL_CD[SEQ] + "' ";
                    strQuery = strQuery + ", @pCODE_CD1 ='" + CTRL_TEXT.Text + "' ";

                    if (strCTRL_CD[SEQ] == "BA")
                    {
                        string strBANK_CD = "";
                        for (int i = 1; i < strCTRL_CD.Length; i++)
                        {
                            if (strCTRL_CD[i] == "BK")
                            {
                                if (i == 1)
                                    strBANK_CD = txtCTRL_VAL1.Text;
                                else if (i == 2)
                                    strBANK_CD = txtCTRL_VAL2.Text;
                                else if (i == 3)
                                    strBANK_CD = txtCTRL_VAL3.Text;
                                else if (i == 4)
                                    strBANK_CD = txtCTRL_VAL4.Text;
                                else if (i == 5)
                                    strBANK_CD = txtCTRL_VAL5.Text;
                                else if (i == 6)
                                    strBANK_CD = txtCTRL_VAL6.Text;
                                else if (i == 7)
                                    strBANK_CD = txtCTRL_VAL7.Text;
                                else if (i == 8)
                                    strBANK_CD = txtCTRL_VAL8.Text;
                            }
                        }

                        strQuery = strQuery + ", @pCODE_CD2 ='" + strBANK_CD + "' ";

                    }

                    dt = SystemBase.DbOpen.NoTranDataTable(strQuery);
                    if (dt.Rows.Count == 0)
                    {
                        return dt;
                    }
                }
                if (dt.Rows.Count == 0)
                    dt.Rows.Add();
                return dt;
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }
        #endregion

        #region 엔터케 -> Tab
        private void ACD001_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (e.KeyChar == Convert.ToChar(Keys.Enter))
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 채권반제, 채무반제, 차입금정보, 고정자산정보 팝업 활성화/비활성화
        protected void POP_ENABLED()
        {
            try
            {
                //채권(EX:11150001)
                if (strSubType == "AR" && optCr.Checked == true)
                {
                    btnArCls.Enabled = true;
                    btnApCls.Enabled = false;
                    btnLoan.Enabled = false;
                    btnAsset.Enabled = false;
                }
                else if (strSubType == "AP" && optDr.Checked == true) //채무(EX:21010001)
                {
                    btnArCls.Enabled = false;
                    btnApCls.Enabled = true;
                    btnLoan.Enabled = false;
                    btnAsset.Enabled = false;
                }
                else if (strSubType == "LN" && optCr.Checked == true) //차입금(EX:)
                {
                    btnArCls.Enabled = false;
                    btnApCls.Enabled = false;
                    btnLoan.Enabled = true;
                    btnAsset.Enabled = false;
                }
                else if (strSubType == "AS" && optDr.Checked == true) //고정자산정보(EX:)
                {
                    btnArCls.Enabled = false;
                    btnApCls.Enabled = false;
                    btnLoan.Enabled = false;
                    btnAsset.Enabled = true;
                }
                else
                {
                    btnArCls.Enabled = false;
                    btnApCls.Enabled = false;
                    btnLoan.Enabled = false;
                    btnAsset.Enabled = false;
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 팝업
        //채권반제
        private void btnArCls_Click(object sender, EventArgs e)
        {
            try
            {
                string strCust_Cd = "";
                if (strCTRL_CD[1] == "BP" || strCTRL_CD[1] == "V6") strCust_Cd = txtCTRL_VAL1.Text;
                else if (strCTRL_CD[2] == "BP" || strCTRL_CD[2] == "V6") strCust_Cd = txtCTRL_VAL2.Text;
                else if (strCTRL_CD[3] == "BP" || strCTRL_CD[3] == "V6") strCust_Cd = txtCTRL_VAL3.Text;
                else if (strCTRL_CD[4] == "BP" || strCTRL_CD[4] == "V6") strCust_Cd = txtCTRL_VAL4.Text;
                else if (strCTRL_CD[5] == "BP" || strCTRL_CD[5] == "V6") strCust_Cd = txtCTRL_VAL5.Text;
                else if (strCTRL_CD[6] == "BP" || strCTRL_CD[6] == "V6") strCust_Cd = txtCTRL_VAL6.Text;
                else if (strCTRL_CD[7] == "BP" || strCTRL_CD[7] == "V6") strCust_Cd = txtCTRL_VAL7.Text;
                else if (strCTRL_CD[8] == "BP" || strCTRL_CD[8] == "V6") strCust_Cd = txtCTRL_VAL8.Text;

                string strAcctCd = "";
                string strAcctNm = "";

                strAcctCd = txtAcctCd.Text;
                strAcctNm = txtAcctNm.Text;

                ACD004P2 pu = new ACD004P2(Ar_Dt_Temp, cboCurCd.SelectedValue.ToString(), strCust_Cd, strBIZ_CD, strAcctCd, strAcctNm);
                pu.Width = 1600;
                pu.Height = 800;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Ar_Dt_Temp = pu.Ar_Dt;

                    cboCurCd.SelectedValue = pu.CUR_CD;
                    txtSlipAmt.Value = pu.SLIP_AMT;
                    txtSlipAmtLoc.Value = pu.SLIP_AMT_LOC;

                    if (txtSeq.Text == "")
                    {
                        if (strREORG_ID != pu.REORG_ID)
                        {
                            string strQuery = " usp_ACD001  'P51'";
                            strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                            strQuery = strQuery + ", @pSLIP_DT ='" + dtpSlipDt.Text + "' ";
                            strQuery = strQuery + ", @pREORG_ID ='" + pu.REORG_ID + "' ";
                            strQuery = strQuery + ", @pDEPT_CD ='" + pu.DEPT_CD + "' ";
                            DataSet ds = SystemBase.DbOpen.NoTranDataSet(strQuery);
                            if (ds.Tables[0].Rows.Count > 0)
                            {
                                txtInputDeptCd.Text = ds.Tables[0].Rows[0]["DEPT_CD"].ToString();
                            }
                        }
                        else
                        {
                            txtInputDeptCd.Text = pu.DEPT_CD;
                        }
                    }

                    if (strCTRL_CD[1] == "BP" || strCTRL_CD[1] == "V6") txtCTRL_VAL1.Value = pu.CUST_CD;
                    else if (strCTRL_CD[2] == "BP" || strCTRL_CD[2] == "V6") txtCTRL_VAL2.Value = pu.CUST_CD;
                    else if (strCTRL_CD[3] == "BP" || strCTRL_CD[3] == "V6") txtCTRL_VAL3.Value = pu.CUST_CD;
                    else if (strCTRL_CD[4] == "BP" || strCTRL_CD[4] == "V6") txtCTRL_VAL4.Value = pu.CUST_CD;
                    else if (strCTRL_CD[5] == "BP" || strCTRL_CD[5] == "V6") txtCTRL_VAL5.Value = pu.CUST_CD;
                    else if (strCTRL_CD[6] == "BP" || strCTRL_CD[6] == "V6") txtCTRL_VAL6.Value = pu.CUST_CD;
                    else if (strCTRL_CD[7] == "BP" || strCTRL_CD[7] == "V6") txtCTRL_VAL7.Value = pu.CUST_CD;
                    else if (strCTRL_CD[8] == "BP" || strCTRL_CD[8] == "V6") txtCTRL_VAL8.Value = pu.CUST_CD;
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //채무반제
        private void btnApCls_Click(object sender, EventArgs e)
        {
            try
            {
                string strCust_Cd = "";
                if (strCTRL_CD[1] == "BP" || strCTRL_CD[1] == "V6") strCust_Cd = txtCTRL_VAL1.Text;
                else if (strCTRL_CD[2] == "BP" || strCTRL_CD[2] == "V6") strCust_Cd = txtCTRL_VAL2.Text;
                else if (strCTRL_CD[3] == "BP" || strCTRL_CD[3] == "V6") strCust_Cd = txtCTRL_VAL3.Text;
                else if (strCTRL_CD[4] == "BP" || strCTRL_CD[4] == "V6") strCust_Cd = txtCTRL_VAL4.Text;
                else if (strCTRL_CD[5] == "BP" || strCTRL_CD[5] == "V6") strCust_Cd = txtCTRL_VAL5.Text;
                else if (strCTRL_CD[6] == "BP" || strCTRL_CD[6] == "V6") strCust_Cd = txtCTRL_VAL6.Text;
                else if (strCTRL_CD[7] == "BP" || strCTRL_CD[7] == "V6") strCust_Cd = txtCTRL_VAL7.Text;
                else if (strCTRL_CD[8] == "BP" || strCTRL_CD[8] == "V6") strCust_Cd = txtCTRL_VAL8.Text;

                string strAcctCd = "";
                string strAcctNm = "";

                strAcctCd = txtAcctCd.Text;
                strAcctNm = txtAcctNm.Text;


                ACD004P3 pu = new ACD004P3(Ap_Dt_Temp, cboCurCd.SelectedValue.ToString(), strCust_Cd, strBIZ_CD, strAcctCd, strAcctNm);
                pu.Width = 1600;
                pu.Height = 800;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Ap_Dt_Temp = pu.Ap_Dt;

                    cboCurCd.SelectedValue = pu.CUR_CD;
                    txtSlipAmt.Value = pu.SLIP_AMT;
                    txtSlipAmtLoc.Value = pu.SLIP_AMT_LOC;

                    if (txtSeq.Text == "")
                    {
                        if (strREORG_ID != pu.REORG_ID)
                        {
                            string strQuery = " usp_ACD001  'P51'";
                            strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                            strQuery = strQuery + ", @pSLIP_DT ='" + dtpSlipDt.Text + "' ";
                            strQuery = strQuery + ", @pREORG_ID ='" + pu.REORG_ID + "' ";
                            strQuery = strQuery + ", @pDEPT_CD ='" + pu.DEPT_CD + "' ";
                            DataSet ds = SystemBase.DbOpen.NoTranDataSet(strQuery);
                            if (ds.Tables[0].Rows.Count > 0)
                            {
                                txtInputDeptCd.Text = ds.Tables[0].Rows[0]["DEPT_CD"].ToString();
                            }
                        }
                        else
                        {
                            txtInputDeptCd.Text = pu.DEPT_CD;
                        }
                    }

                    if (strCTRL_CD[1] == "BP" || strCTRL_CD[1] == "V6") txtCTRL_VAL1.Value = pu.CUST_CD;
                    else if (strCTRL_CD[2] == "BP" || strCTRL_CD[2] == "V6") txtCTRL_VAL2.Value = pu.CUST_CD;
                    else if (strCTRL_CD[3] == "BP" || strCTRL_CD[3] == "V6") txtCTRL_VAL3.Value = pu.CUST_CD;
                    else if (strCTRL_CD[4] == "BP" || strCTRL_CD[4] == "V6") txtCTRL_VAL4.Value = pu.CUST_CD;
                    else if (strCTRL_CD[5] == "BP" || strCTRL_CD[5] == "V6") txtCTRL_VAL5.Value = pu.CUST_CD;
                    else if (strCTRL_CD[6] == "BP" || strCTRL_CD[6] == "V6") txtCTRL_VAL6.Value = pu.CUST_CD;
                    else if (strCTRL_CD[7] == "BP" || strCTRL_CD[7] == "V6") txtCTRL_VAL7.Value = pu.CUST_CD;
                    else if (strCTRL_CD[8] == "BP" || strCTRL_CD[8] == "V6") txtCTRL_VAL8.Value = pu.CUST_CD;
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //차입금정보
        private void btnLoan_Click(object sender, EventArgs e)
        {
            try
            {
                string strLoan_No = "";
                if (strCTRL_CD[1] == "L1") strLoan_No = txtCTRL_VAL1.Text;
                else if (strCTRL_CD[2] == "L1") strLoan_No = txtCTRL_VAL2.Text;
                else if (strCTRL_CD[3] == "L1") strLoan_No = txtCTRL_VAL3.Text;
                else if (strCTRL_CD[4] == "L1") strLoan_No = txtCTRL_VAL4.Text;
                else if (strCTRL_CD[5] == "L1") strLoan_No = txtCTRL_VAL5.Text;
                else if (strCTRL_CD[6] == "L1") strLoan_No = txtCTRL_VAL6.Text;
                else if (strCTRL_CD[7] == "L1") strLoan_No = txtCTRL_VAL7.Text;
                else if (strCTRL_CD[8] == "L1") strLoan_No = txtCTRL_VAL8.Text;

                ACD004P4 pu = new ACD004P4(Loan_Dt_Temp, cboCurCd.SelectedValue.ToString(), strLoan_No, dtpSlipDt.Text, txtAcctCd.Text);
                //pu.Width = 1300;
                //pu.Height = 800;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Loan_Dt_Temp = pu.Loan_Dt;

                    cboCurCd.SelectedValue = pu.CUR_CD;
                    txtSlipAmt.Value = pu.SLIP_AMT;
                    txtSlipAmtLoc.Value = pu.SLIP_AMT_LOC;
                    if (strCTRL_CD[1] == "L1") txtCTRL_VAL1.Value = pu.LOAN_NO;
                    else if (strCTRL_CD[2] == "L1") txtCTRL_VAL2.Value = pu.LOAN_NO;
                    else if (strCTRL_CD[3] == "L1") txtCTRL_VAL3.Value = pu.LOAN_NO;
                    else if (strCTRL_CD[4] == "L1") txtCTRL_VAL4.Value = pu.LOAN_NO;
                    else if (strCTRL_CD[5] == "L1") txtCTRL_VAL5.Value = pu.LOAN_NO;
                    else if (strCTRL_CD[6] == "L1") txtCTRL_VAL6.Value = pu.LOAN_NO;
                    else if (strCTRL_CD[7] == "L1") txtCTRL_VAL7.Value = pu.LOAN_NO;
                    else if (strCTRL_CD[8] == "L1") txtCTRL_VAL8.Value = pu.LOAN_NO;
                    txtRemark2.Value = pu.REMARK;
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //고정자산정보
        private void btnAsset_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtExch_Rate.Text == "" || cboCurCd.Text == "")
                {
                    MessageBox.Show("화폐단위와 환율을 먼저 입력하세요.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                ACD004P5 pu = new ACD004P5(Asset_Dt_Temp, cboCurCd.SelectedValue.ToString(), Convert.ToDouble(txtExch_Rate.Text.Replace(",", "")), txtAcctCd.Text);
                pu.Width = 1500;
                pu.Height = 400;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Asset_Dt_Temp = pu.Asset_Dt;

                    txtSlipAmt.Value = pu.SLIP_AMT;
                    txtSlipAmtLoc.Value = pu.SLIP_AMT_LOC;
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 채권반제, 채무반제, 차입금정보, 고정자산정보 정보 DataSet에 셋팅
        protected void ETC_TABLE_SET()
        {
            try
            {
                string strQuery = " usp_ACD001  'P6'";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                strQuery = strQuery + ", @pSLIP_NO ='" + txtSlipNo.Text + "' ";
                DataSet dt = SystemBase.DbOpen.NoTranDataSet(strQuery);
                if (dt.Tables.Count != 4)
                {
                    MessageBox.Show("채권반제, 채무반제, 차입금정보, 고정자산정보를 가져오지 못했습니다.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    Ar_Dt = dt.Tables[0];
                    Ap_Dt = dt.Tables[1];
                    Loan_Dt = dt.Tables[2];
                    Asset_Dt = dt.Tables[3];
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        private void ACCT_DEFAULT()
        {
            try
            {
                double dNetAmt = 0;
                string strCustCd = "";
                if (fpSpread1.Sheets[0].Rows.Count > 0 && txtSeq.Text == "")
                {
                    dNetAmt = Convert.ToDouble(fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "전표금액")].Text.Replace(",", ""));
                    if (fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "관리항목1코드")].Text == "BP" || fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "관리항목1코드")].Text == "V6")
                        strCustCd = fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "관리항목1")].Text.Replace(",", "");
                    else if (fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "관리항목2코드")].Text == "BP" || fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "관리항목2코드")].Text == "V6")
                        strCustCd = fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "관리항목2")].Text.Replace(",", "");
                    else if (fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "관리항목3코드")].Text == "BP" || fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "관리항목3코드")].Text == "V6")
                        strCustCd = fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "관리항목3")].Text.Replace(",", "");
                    else if (fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "관리항목4코드")].Text == "BP" || fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "관리항목4코드")].Text == "V6")
                        strCustCd = fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "관리항목4")].Text.Replace(",", "");
                    else if (fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "관리항목5코드")].Text == "BP" || fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "관리항목5코드")].Text == "V6")
                        strCustCd = fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "관리항목5")].Text.Replace(",", "");
                    else if (fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "관리항목6코드")].Text == "BP" || fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "관리항목6코드")].Text == "V6")
                        strCustCd = fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "관리항목6")].Text.Replace(",", "");
                    else if (fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "관리항목7코드")].Text == "BP" || fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "관리항목7코드")].Text == "V6")
                        strCustCd = fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "관리항목7")].Text.Replace(",", "");
                    else if (fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "관리항목8코드")].Text == "BP" || fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "관리항목8코드")].Text == "V6")
                        strCustCd = fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "관리항목8")].Text.Replace(",", "");
                }
                string strTaxBizCd = SystemBase.Base.CodeName("BIZ_CD", "TAX_BIZ_CD", "B_BIZ_PLACE", SystemBase.Base.gstrBIZCD, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                //V1 	공급가액
                //BP 	거래처
                //V6 	거래처코드
                //V4 	계산서유형
                //V7 	부가세율
                //V2 	계산서일
                //V5 	신고사업장

                string strVatType = SystemBase.Base.CodeName("MAJOR_CD", "MINOR_CD", "B_COMM_CODE", "B040", " AND COMP_CODE = '" + SystemBase.Base.gstrCOMCD + "' AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND REL_CD7 = 'Y' ");
                if (strVatType == "") strVatType = "A";

                if (strCTRL_CD[1] == "V1") txtCTRL_VAL1.Value = dNetAmt;
                else if (strCTRL_CD[1] == "BP" || strCTRL_CD[1] == "V6") txtCTRL_VAL1.Value = strCustCd;
                else if (strCTRL_CD[1] == "V4") txtCTRL_VAL1.Value = strVatType;
                else if (strCTRL_CD[1] == "V7") txtCTRL_VAL1.Value = "10";
                else if (strCTRL_CD[1] == "V2") txtCTRL_VAL1.Value = dtpSlipDt.Text;
                else if (strCTRL_CD[1] == "V5") txtCTRL_VAL1.Value = strTaxBizCd;

                if (strCTRL_CD[2] == "V1") txtCTRL_VAL2.Value = dNetAmt;
                else if (strCTRL_CD[2] == "BP" || strCTRL_CD[2] == "V6") txtCTRL_VAL2.Value = strCustCd;
                else if (strCTRL_CD[2] == "V4") txtCTRL_VAL2.Value = strVatType;
                else if (strCTRL_CD[2] == "V7") txtCTRL_VAL2.Value = "10";
                else if (strCTRL_CD[2] == "V2") txtCTRL_VAL2.Value = dtpSlipDt.Text;
                else if (strCTRL_CD[2] == "V5") txtCTRL_VAL2.Value = strTaxBizCd;

                if (strCTRL_CD[3] == "V1") txtCTRL_VAL3.Value = dNetAmt;
                else if (strCTRL_CD[3] == "BP" || strCTRL_CD[3] == "V6") txtCTRL_VAL3.Value = strCustCd;
                else if (strCTRL_CD[3] == "V4") txtCTRL_VAL3.Value = strVatType;
                else if (strCTRL_CD[3] == "V7") txtCTRL_VAL3.Value = "10";
                else if (strCTRL_CD[3] == "V2") txtCTRL_VAL3.Value = dtpSlipDt.Text;
                else if (strCTRL_CD[3] == "V5") txtCTRL_VAL3.Value = strTaxBizCd;

                if (strCTRL_CD[4] == "V1") txtCTRL_VAL4.Value = dNetAmt;
                else if (strCTRL_CD[4] == "BP" || strCTRL_CD[4] == "V6") txtCTRL_VAL4.Value = strCustCd;
                else if (strCTRL_CD[4] == "V4") txtCTRL_VAL4.Value = strVatType;
                else if (strCTRL_CD[4] == "V7") txtCTRL_VAL4.Value = "10";
                else if (strCTRL_CD[4] == "V2") txtCTRL_VAL4.Value = dtpSlipDt.Text;
                else if (strCTRL_CD[4] == "V5") txtCTRL_VAL4.Value = strTaxBizCd;

                if (strCTRL_CD[5] == "V1") txtCTRL_VAL5.Value = dNetAmt;
                else if (strCTRL_CD[5] == "BP" || strCTRL_CD[5] == "V6") txtCTRL_VAL5.Value = strCustCd;
                else if (strCTRL_CD[5] == "V4") txtCTRL_VAL5.Value = strVatType;
                else if (strCTRL_CD[5] == "V7") txtCTRL_VAL5.Value = "10";
                else if (strCTRL_CD[5] == "V2") txtCTRL_VAL5.Value = dtpSlipDt.Text;
                else if (strCTRL_CD[5] == "V5") txtCTRL_VAL5.Value = strTaxBizCd;

                if (strCTRL_CD[6] == "V1") txtCTRL_VAL6.Value = dNetAmt;
                else if (strCTRL_CD[6] == "BP" || strCTRL_CD[6] == "V6") txtCTRL_VAL6.Value = strCustCd;
                else if (strCTRL_CD[6] == "V4") txtCTRL_VAL6.Value = strVatType;
                else if (strCTRL_CD[6] == "V7") txtCTRL_VAL6.Value = "10";
                else if (strCTRL_CD[6] == "V2") txtCTRL_VAL6.Value = dtpSlipDt.Text;
                else if (strCTRL_CD[6] == "V5") txtCTRL_VAL6.Value = strTaxBizCd;

                if (strCTRL_CD[7] == "V1") txtCTRL_VAL7.Value = dNetAmt;
                else if (strCTRL_CD[7] == "BP" || strCTRL_CD[7] == "V6") txtCTRL_VAL7.Value = strCustCd;
                else if (strCTRL_CD[7] == "V4") txtCTRL_VAL7.Value = strVatType;
                else if (strCTRL_CD[7] == "V7") txtCTRL_VAL7.Value = "10";
                else if (strCTRL_CD[7] == "V2") txtCTRL_VAL7.Value = dtpSlipDt.Text;
                else if (strCTRL_CD[7] == "V5") txtCTRL_VAL7.Value = strTaxBizCd;

                if (strCTRL_CD[8] == "V1") txtCTRL_VAL8.Value = dNetAmt;
                else if (strCTRL_CD[8] == "BP" || strCTRL_CD[8] == "V6") txtCTRL_VAL8.Value = strCustCd;
                else if (strCTRL_CD[8] == "V4") txtCTRL_VAL8.Value = strVatType;
                else if (strCTRL_CD[8] == "V7") txtCTRL_VAL8.Value = "10";
                else if (strCTRL_CD[8] == "V2") txtCTRL_VAL8.Value = dtpSlipDt.Text;
                else if (strCTRL_CD[8] == "V5") txtCTRL_VAL8.Value = strTaxBizCd;


            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
