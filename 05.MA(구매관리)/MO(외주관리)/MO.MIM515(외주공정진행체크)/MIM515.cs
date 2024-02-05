#region 작성정보
/*********************************************************************/
// 단위업무명 : 외주공정진행체크
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-02-15
// 작성내용 : 외주공정진행체크 및 관리
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
using WNDW;

namespace MO.MIM515
{
    public partial class MIM515 : UIForm.FPCOMM1
    {
        int SDown = 1;		// 조회 횟수
        int AddRow = 100;	// 조회 건수

        public MIM515()
        {
            InitializeComponent();
        }

        #region Form Load시
        private void PCC045_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            //콤보박스세팅
            SystemBase.ComboMake.C1Combo(cboPlantCd, "usp_B_COMMON @pType='TABLE', @pCODE = 'PLANT_CD', @pNAME = 'PLANT_NM', @pSPEC1 = 'B_PLANT_INFO', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 0);//공장
            SystemBase.ComboMake.C1Combo(cboOrderFlag, "usp_B_COMMON @pType='COMM', @pCODE = 'P026', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ", 3);  //지시구분

            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "지시")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'P026', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ", 0);//지시구분

            //그리드초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

            //기타세팅
            cboPlantCd.SelectedValue = SystemBase.Base.gstrPLANT_CD;
            dtpPlanStartDtFr.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToString().Substring(0,10);
            dtpPlanStartDtTo.Value = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0, 10);
        }
        #endregion

        #region 팝업 클릭시
        //제조오더 팝업
        private void btnWorkorderNo_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW006 pu = new WNDW006(txtWorkorderNo.Text);
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtWorkorderNo.Text = Msgs[1].ToString();
                    txtWorkorderNo.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "제조오더번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //프로젝트 팝업
        private void btnProjectNo_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW003 pu = new WNDW003(txtProjectNo.Text, "S1");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtProjectNo.Text = Msgs[3].ToString();
                    txtProjectNm.Value = Msgs[4].ToString();
                    txtItemCd.Text = Msgs[6].ToString();
                    txtItemNm.Value = Msgs[7].ToString();

                    txtProjectNo.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "프로젝트 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //작업장 팝업
        private void btnWc_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pTYPE = 'P042', @pLANG_CD = 'KOR', @pETC = 'P002', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";					// 쿼리
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
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "작업장 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //사업코드 팝업
        private void btnEntCd_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = "usp_B_COMMON @pTYPE = 'TABLE_POP', @pSPEC1='ENT_CD', @pSPEC2 = 'ENT_NM', @pSPEC3 = 'S_ENTERPRISE_INFO', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtEntCd.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P05008", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "사업코드 조회");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtEntCd.Text = Msgs[0].ToString();
                    txtEntNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "사업코드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);	//데이터 조회 중 오류가 발생하였습니다.

            }
        }

        //품목코드 팝업
        private void btnItemCd_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW005 pu = new WNDW005(Convert.ToString(cboPlantCd.SelectedValue), true, txtItemCd.Text);
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtItemCd.Text = Msgs[2].ToString();
                    txtItemNm.Value = Msgs[3].ToString();

                    txtItemCd.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목코드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //거래처 팝업
        private void btnCustCd_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW002 pu = new WNDW002(txtCustCd.Text);
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtCustCd.Text = Msgs[1].ToString();
                    txtCustNm.Value = Msgs[2].ToString();

                    txtItemCd.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목코드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region NewExec()
        protected override void NewExec()
        {
            //조회조건 초기화
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            //그리드 초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

            //기타세팅
            cboPlantCd.SelectedValue = SystemBase.Base.gstrPLANT_CD;
            dtpPlanStartDtFr.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToString().Substring(0,10);
            dtpPlanStartDtTo.Value = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0,10);
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {
                    SDown = 1;

                    string cfmYn = "";
                    if (rdoDivY.Checked == true) { cfmYn = "Y"; }
                    else if (rdoDivN.Checked == true) { cfmYn = "N"; }
                    else { cfmYn = ""; }

                    string strQuery = " usp_MIM515  @pTYPE = 'S1'";
                    strQuery += ", @pPLANT_CD = '" + Convert.ToString(cboPlantCd.SelectedValue) + "' ";
                    if (dtpPlanStartDtFr.Text != "")
                        strQuery += ", @pPLAN_START_DT_FR = '" + dtpPlanStartDtFr.Text + "' ";
                    if (dtpPlanStartDtTo.Text != "")
                        strQuery += ", @pPLAN_START_DT_TO = '" + dtpPlanStartDtTo.Text + "' ";
                    strQuery += ", @pWORKORDER_NO = '" + txtWorkorderNo.Text + "' ";
                    strQuery += ", @pENT_CD = '" + txtEntCd.Text + "' ";
                    strQuery += ", @pPROJECT_NO = '" + txtProjectNo.Text + "'";
                    strQuery += ", @pITEM_CD = '" + txtItemCd.Text + "' ";
                    strQuery += ", @pWC_CD = '" + txtWcCd.Text + "' ";
                    strQuery += ", @pCUST_CD = '" + txtCustCd.Text + "' ";
                    strQuery += ", @pDIV = '" + cfmYn + "'";
                    strQuery += ", @pPRODT_ORDER_TYPE = '" + Convert.ToString(cboOrderFlag.SelectedValue) + "'";
                    strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'";
                    strQuery += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                    strQuery += ", @pTOPCOUNT ='" + AddRow + "'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 1);

                    Grid_Set(0);
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

        #region SaveExec() 저장
        protected override void SaveExec()
        {
            this.Cursor = Cursors.WaitCursor;

            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                if (SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", true)) //그리드 필수체크
                {

                    string ERRCode = "ER", MSGCode = "P0000"; //처리할 내용이 없습니다.

                    SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                    SqlCommand cmd = dbConn.CreateCommand();
                    SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                    try
                    {
                        //행수만큼 처리
                        for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                        {
                            string strHead = fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text;

                            if (strHead.Length > 0)
                            {
                                string strDSql = " usp_MIM515 'U1'";
                                strDSql += ", @pWORKORDER_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "제조오더번호")].Text + "' ";
                                strDSql += ", @pPROC_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "공정")].Text + "' ";
                                if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "확인")].Value.ToString() == "True")
                                    strDSql += ", @pCHK_FLAG = '" + "Y" + "'";
                                else
                                    strDSql += ", @pCHK_FLAG = '" + "N" + "'";

                                strDSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                                strDSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                                DataSet ds1 = SystemBase.DbOpen.TranDataSet(strDSql, dbConn, Trans);

                                ERRCode = ds1.Tables[0].Rows[0][0].ToString();
                                MSGCode = ds1.Tables[0].Rows[0][1].ToString();

                                if (ERRCode != "OK")
                                {
                                    Trans.Rollback();
                                    goto Exit;
                                }	// ER 코드 Return시 점프
                            }
                        }
                        Trans.Commit();
                    }
                    catch (Exception e)
                    {
                        SystemBase.Loggers.Log(this.Name, e.ToString());
                        MessageBox.Show(e.ToString());
                        Trans.Rollback();
                        ERRCode = "ER";
                        MSGCode = "P0001";	//에러가 발생되어 데이터 처리가 취소되었습니다.
                    }
                Exit:
                    dbConn.Close();

                    if (ERRCode == "OK")
                    {
                        SearchExec();
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
            this.Cursor = Cursors.Default;
        }

        #endregion

        #region 코드입력시 코드명 자동입력
        //사업자 코드
        private void txtEntCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtEntCd.Text != "")
                {
                    txtEntNm.Value = SystemBase.Base.CodeName("ENT_CD", "ENT_NM", "S_ENTERPRISE_INFO", txtEntCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtEntNm.Value = "";
                }
            }
            catch
            {

            }
        }

        //프로젝트 코드
        private void txtProjectNo_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtProjectNo.Text != "")
                {
                    txtProjectNm.Value = SystemBase.Base.CodeName("PROJECT_NO", "PROJECT_NM", "S_SO_MASTER", txtProjectNo.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtProjectNm.Value = "";
                }
            }
            catch
            {

            }
        }

        //품목
        private void txtItemCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtItemCd.Text != "")
                {
                    txtItemNm.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtItemCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtItemNm.Value = "";
                }
            }
            catch
            {

            }
        }

        //작업장
        private void txtWcCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtWcCd.Text != "")
                {
                    txtWcNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtWcCd.Text, " AND MAJOR_CD = 'P002' AND COMP_CODE = '" + SystemBase.Base.gstrCOMCD + "' ");
                }
                else
                {
                    txtWcNm.Value = "";
                }
            }
            catch
            {

            }

        }

        //거래처
        private void txtCustCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtCustCd.Text != "")
                {
                    txtCustNm.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtCustCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtCustNm.Value = "";
                }
            }
            catch
            {

            }
        }
        #endregion

        #region fpSpread1_TopChange
        private void fpSpread1_TopChange(object sender, FarPoint.Win.Spread.TopChangeEventArgs e)
        {
            int FPHeight = (fpSpread1.Size.Height - 28) / 20;
            if (e.NewTop >= ((AddRow * SDown) - FPHeight))
            {
                int cnt_prev = AddRow * SDown;
                SDown++;
                int cnt = AddRow * SDown;

                this.Cursor = Cursors.WaitCursor;

                string cfmYn = "";
                if (rdoDivY.Checked == true) { cfmYn = "Y"; }
                else if (rdoDivN.Checked == true) { cfmYn = "N"; }
                else { cfmYn = ""; }

                string strQuery = " usp_MIM515  @pTYPE = 'S1'";
                strQuery += ", @pPLANT_CD = '" + Convert.ToString(cboPlantCd.SelectedValue) + "' ";
                if (dtpPlanStartDtFr.Text != "")
                    strQuery += ", @pPLAN_START_DT_FR = '" + dtpPlanStartDtFr.Text + "' ";
                if (dtpPlanStartDtTo.Text != "")
                    strQuery += ", @pPLAN_START_DT_TO = '" + dtpPlanStartDtTo.Text + "' ";
                strQuery += ", @pWORKORDER_NO = '" + txtWorkorderNo.Text + "' ";
                strQuery += ", @pENT_CD = '" + txtEntCd.Text + "' ";
                strQuery += ", @pPROJECT_NO = '" + txtProjectNo.Text + "'";
                strQuery += ", @pITEM_CD = '" + txtItemCd.Text + "' ";
                strQuery += ", @pWC_CD = '" + txtWcCd.Text + "' ";
                strQuery += ", @pCUST_CD = '" + txtCustCd.Text + "' ";
                strQuery += ", @pDIV = '" + cfmYn + "'";
                strQuery += ", @pPRODT_ORDER_TYPE = '" + Convert.ToString(cboOrderFlag.SelectedValue) + "'";
                strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'";
                strQuery += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                strQuery += ", @pTOPCOUNT ='" + cnt + "'";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery);

                Grid_Set(cnt_prev);
            }
        }

        #endregion

        #region 공정진행현황
        private void btnProcInfo_Click(object sender, System.EventArgs e)
        {
            if (fpSpread1.Sheets[0].Rows.Count > 0)
            {
                int Row = fpSpread1.Sheets[0].ActiveRowIndex;

                string ProjectNo = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text;
                string ProjectSeq = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Text;
                string ItemCd = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text;
                string WoNo = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "제조오더번호")].Text;

                MIM515P1 myForm = new MIM515P1(ProjectNo, ProjectSeq, ItemCd, WoNo);
                myForm.ShowDialog();
            }
        }
        #endregion

        #region CL 그리드 빨간색 글로 표시
        private void Grid_Set(int start)
        {
            try
            {
                for (int i = start; i < fpSpread1.Sheets[0].RowCount; i++)
                {

                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "상태")].Text == "CL")
                    {
                        for (int j = 0; j < fpSpread1.Sheets[0].ColumnCount; j++)
                        {
                            fpSpread1.Sheets[0].Cells[i, j].ForeColor = Color.Red;
                        }
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }

            this.Cursor = Cursors.Default;
        }
        #endregion


    }
}
