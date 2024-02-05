#region 작성정보
/*********************************************************************/
// 단위업무명 : 공정별 진행현황조회
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-04-16
// 작성내용 : 공정별 진행현황조회 관리
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

namespace EI.EISB02
{
    public partial class EISB02 : UIForm.FPCOMM1
    {
        #region 변수선언
        private string strMQuery;
        #endregion

        #region 생성자
        public EISB02()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load시
        private void EISB02_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            // 기본정보 바인딩
            txtPlant_CD.Value = SystemBase.Base.gstrPLANT_CD;

            SystemBase.ComboMake.C1Combo(cboOrderFlag, "usp_B_COMMON @pTYPE = 'COMM', @pCODE = 'P026', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 3);  //지시구분
            SystemBase.ComboMake.C1Combo(cboWorkFlag, "usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'P038', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 3); //작업구분

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 8, true);

            dtpSoDelvDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddYears(-1).ToShortDateString().Substring(0, 10);
            dtpSoDelvDtTo.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).ToShortDateString().Substring(0, 10);

            SearchExec();
        }
        #endregion

        #region NewExec()
        protected override void NewExec()
        {
            // 초기화
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            rdoMileAll.Checked = true;
            rdoDivY.Checked = true;
            rdo4.Checked = true;
            rdoYes.Checked = true;

            SystemBase.ComboMake.C1Combo(cboOrderFlag, "usp_B_COMMON @pTYPE = 'COMM', @pCODE = 'P026', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 3);  //지시구분
            SystemBase.ComboMake.C1Combo(cboWorkFlag, "usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'P038', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 3); //작업구분

            // 기본정보 바인딩
            txtPlant_CD.Value = SystemBase.Base.gstrPLANT_CD;
            dtpSoDelvDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddYears(-1).ToShortDateString().Substring(0, 10);
            dtpSoDelvDtTo.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).ToShortDateString().Substring(0, 10);

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 9, true);

        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {

            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
            DialogResult dsMsg;
            try
            {
                double TotPer = 0;
                string strStartMakeNo = "", strStartWoNo = "";
                string strNewMakeNo = "", strNewWoNo = "";
                int intItemCount = 0;
                int intRow = 0;
                double cnt = 0;
                double ColCnt = 0;
                int rowCnt = 0;

                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {
                    string strOrderFlag = "";
                    if (rdoYes.Checked == true) strOrderFlag = "P";
                    else if (rdoNo.Checked == true) strOrderFlag = "D";

                    string strOrderStatus = "";
                    if (rdo2.Checked == true) strOrderStatus = "2";		//release
                    else if (rdo3.Checked == true) strOrderStatus = "3";	//start
                    else if (rdo4.Checked == true) strOrderStatus = "4";	//release/start
                    else if (rdo5.Checked == true) strOrderStatus = "5";	//close

                    string strReportFlag = "";
                    if (rdoDivY.Checked == true) strReportFlag = "Y";
                    else if (rdoDivN.Checked == true) strReportFlag = "N";

                    string strMileStone = "";
                    if (rdoMileY.Checked == true) strMileStone = "Y";
                    else if (rdoMileN.Checked == true) strMileStone = "N";

                    string strReadyflag = "N";
                    if (rdoY.Checked == true) strReadyflag = "Y";

                    string strCloseYN = "";
                    if (rdoCloseY.Checked == true)
                        strCloseYN = "Y";
                    else if (rdoCloseN.Checked == true)
                        strCloseYN = "N";

                    strMQuery = " usp_EISB02 'S1'";
                    strMQuery += ", @pPLANT_CD='" + txtPlant_CD.Text + "'";
                    strMQuery += ", @pITEM_CD='" + txtITEM_CD.Text.Trim() + "'";
                    strMQuery += ", @pGROUP_CD='" + txtGroupCd.Text.Trim() + "'";
                    strMQuery += ", @pWORKORDER_NO_FR ='" + txtWoNoFr.Text + "'";
                    strMQuery += ", @pWORKORDER_NO_TO ='" + txtWoNoTo.Text + "'";
                    strMQuery += ", @pPROJECT_NO='" + txtProject_No.Text + "'";
                    strMQuery += ", @pPROJECT_SEQ_FR ='" + txtProject_Seq_Fr.Text + "'";
                    strMQuery += ", @pPROJECT_SEQ_TO ='" + txtProject_Seq_To.Text + "'";
                    strMQuery += ", @pSTATUS ='" + strOrderStatus + "'";
                    strMQuery += ", @pMAKEORDER_NO_FR ='" + txtMakeOrderNoFr.Text + "'";
                    strMQuery += ", @pMAKEORDER_NO_TO ='" + txtMakeOrderNoTo.Text + "'";
                    strMQuery += ", @pORDER_FLAG ='" + strOrderFlag + "'";
                    strMQuery += ", @pREPORT_FLAG ='" + strReportFlag + "'";
                    strMQuery += ", @pREPORT_DT_FR ='" + dtpReportDtFr.Text + "'";
                    strMQuery += ", @pREPORT_DT_TO ='" + dtpReportDtTo.Text + "'";
                    strMQuery += ", @pSO_DELV_DT_FR ='" + dtpSoDelvDtFr.Text + "'";
                    strMQuery += ", @pSO_DELV_DT_TO ='" + dtpSoDelvDtTo.Text + "'";
                    strMQuery += ", @pWC_CD ='" + txtWcCd.Text + "'";
                    strMQuery += ", @pJOB_CD ='" + txtJobCd.Text + "'";
                    strMQuery += ", @pMILESTONE_FLAG ='" + strMileStone + "'";
                    strMQuery += ", @pREADY_FLAG ='" + strReadyflag + "'";
                    strMQuery += ", @pORDER_TYPE ='" + cboOrderFlag.SelectedValue.ToString() + "'";
                    strMQuery += ", @pWORK_FLAG ='" + cboWorkFlag.SelectedValue.ToString() + "'";
                    strMQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    strMQuery += ", @pCLOSE_YN ='" + strCloseYN + "'";
                    strMQuery += ", @pPRODMANADUTY ='" + txtProdManaDuty.Text + "'";
                    DataSet ds = SystemBase.DbOpen.NoTranDataSet(strMQuery);

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        fpSpread1.Sheets[0].RowCount = 0;

                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            strNewMakeNo = ds.Tables[0].Rows[i][22].ToString();
                            strNewWoNo = ds.Tables[0].Rows[i][8].ToString();

                            if (strStartWoNo != strNewWoNo)
                            {
                                UIForm.FPMake.RowInsert(fpSpread1);
                                intRow = fpSpread1.ActiveSheet.GetSelection(0).Row;
                                rowCnt = rowCnt + 1;
                                cnt = 0;
                                ColCnt = 0;

                                fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트")].Text = ds.Tables[0].Rows[i][0].ToString();  //프로젝트
                                fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트명")].Text = ds.Tables[0].Rows[i][1].ToString();  //프로젝트
                                fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Text = ds.Tables[0].Rows[i][2].ToString();  //차수
                                fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "제품코드")].Text = ds.Tables[0].Rows[i][3].ToString();  //제품코드
                                fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "제품명")].Text = ds.Tables[0].Rows[i][4].ToString();  //제품명
                                fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "수주납기일")].Text = ds.Tables[0].Rows[i][5].ToString();  //수주납기일
                                fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "최종납기일")].Text = ds.Tables[0].Rows[i][6].ToString();  //수주납기일(참조)
                                fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "배포일")].Text = ds.Tables[0].Rows[i][7].ToString();  //배포일
                                fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "제조오더번호")].Text = ds.Tables[0].Rows[i][8].ToString();  //제조오더번호
                                fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text = ds.Tables[0].Rows[i][9].ToString();  //품목
                                fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "품목명")].Text = ds.Tables[0].Rows[i][10].ToString();  //품목명
                                fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "라우팅번호")].Text = ds.Tables[0].Rows[i][11].ToString();  //라우팅번호
                                fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "지시구분")].Text = ds.Tables[0].Rows[i][12].ToString();  //지시구분
                                fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "규격")].Text = ds.Tables[0].Rows[i][13].ToString();  //품목규격
                                fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "오더수량")].Text = ds.Tables[0].Rows[i][14].ToString(); //오더수량
                                fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "잔량")].Text = ds.Tables[0].Rows[i][15].ToString(); //잔여수량
                                fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "비고")].Text = ds.Tables[0].Rows[i][21].ToString(); //비고
                                fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "작업장")].Text = ds.Tables[0].Rows[i][23].ToString(); //작업장

                                fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "생관담당자")].Text = ds.Tables[0].Rows[i][25].ToString(); //생관담당자
                                fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "문제점 및 대책")].Text = ds.Tables[0].Rows[i][26].ToString(); //문제점및대책


                                fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "완료율(%)")].Text = "";
                                fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "1")].Text = ds.Tables[0].Rows[i][19].ToString(); //작업명


                                if (ds.Tables[0].Rows[i][17].ToString() == "ST") //공정실적상태
                                    fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "1")].ForeColor = Color.Blue;
                                else if (ds.Tables[0].Rows[i][17].ToString() == "CL")
                                {
                                    fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "1")].ForeColor = Color.Red;
                                    cnt++;
                                }

                                ColCnt++;
                                intItemCount = 1;
                                strStartMakeNo = ds.Tables[0].Rows[i][22].ToString();
                                strStartWoNo = ds.Tables[0].Rows[i][8].ToString();
                            }
                            else
                            {
                                fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "1") + intItemCount].Text = ds.Tables[0].Rows[i][19].ToString();

                                if (ds.Tables[0].Rows[i][17].ToString() == "ST")
                                    fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "1") + intItemCount].ForeColor = Color.Blue;
                                else if (ds.Tables[0].Rows[i][17].ToString() == "CL")
                                {
                                    fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "1") + intItemCount].ForeColor = Color.Red;
                                    cnt++;
                                }

                                intItemCount += 1;
                                ColCnt++;
                            }

                            if (cnt > 0)
                            {
                                double per = 0;
                                per = (cnt / ColCnt) * 100;
                                per = Convert.ToInt32(per);
                                fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "완료율(%)")].Text = per + "%";
                            }
                            else
                            {
                                fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "완료율(%)")].Text = "0%";
                            }
                        }

                        for (int z = 0; z < fpSpread1.Sheets[0].RowCount; z++)
                        {
                            fpSpread1.Sheets[0].RowHeader.Cells[z, 0].Text = "";
                            fpSpread1.Sheets[0].RowHeader.Rows[z].BackColor = SystemBase.Base.Color_Org;

                            for (int j = 0; j < fpSpread1.Sheets[0].ColumnCount; j++)
                                fpSpread1.Sheets[0].Cells[z, j].Locked = true;
                        }

                        
                    }
                    else
                    {
                        fpSpread1.Sheets[0].Rows.Count = 0;

                        MessageBox.Show(SystemBase.Base.MessageRtn("B0011"), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                    for (int k = 0; k < fpSpread1.Sheets[0].RowCount; k++)
                    {
                        TotPer = TotPer + Convert.ToDouble(fpSpread1.Sheets[0].Cells[k, SystemBase.Base.GridHeadIndex(GHIdx1, "완료율(%)")].Text.Replace("%", ""));
                    }
                    if (rowCnt > 0)
                    {
                        TotPer = (TotPer / rowCnt);
                        TotPer = Convert.ToInt32(TotPer);
                    }
                    else
                    {
                        TotPer = 0;
                    }

                    txtPer.Value = TotPer + "%";


                    if (fpSpread1.Sheets[0].RowCount > 0)
                    {
                        int col_idx = SystemBase.Base.GridHeadIndex(GHIdx1, "오더수량");
                        int col_idx1 = SystemBase.Base.GridHeadIndex(GHIdx1, "잔량");
                        int col_idx2 = SystemBase.Base.GridHeadIndex(GHIdx1, "최종납기일");
                        for (int i = 0; i < fpSpread1.Sheets[0].RowCount; i++)
                        {
                            for (int j = 0; j < fpSpread1.Sheets[0].ColumnCount; j++)
                            {
                                fpSpread1.Sheets[0].Cells[i, col_idx].ForeColor = Color.Blue;
                                fpSpread1.Sheets[0].Cells[i, col_idx1].ForeColor = Color.Blue;
                                fpSpread1.Sheets[0].Cells[i, col_idx2].ForeColor = Color.Blue;
                            }
                        }
                    }
                }

                fpSpread1.Sheets[0].SetActiveCell(0, 1);
                fpSpread1.ActiveSheet.AddSelection(0, 1, 1, 1);
                fpSpread1.ShowActiveCell(FarPoint.Win.Spread.VerticalPosition.Nearest, FarPoint.Win.Spread.HorizontalPosition.Nearest);
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            this.Cursor = System.Windows.Forms.Cursors.Default;

        }
        #endregion

        #region 버튼 Click
        //공장 팝업
        private void btnPlant_CD_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON 'P011' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";								// 쿼리
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };				// 쿼리 인자값(조회조건)
                string[] strSearch = new string[] { txtPlant_CD.Text, "" };	// 쿼리 인자값에 들어갈 데이타

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00005", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "공장 조회", false);

                pu.ShowDialog();	//공통 팝업 호출
                if (pu.DialogResult == DialogResult.OK)
                {
                    string MSG = pu.ReturnVal.Replace("|", "#");
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(MSG);

                    txtPlant_CD.Value = Msgs[0].ToString();
                    txtPlant_NM.Value = Msgs[1].ToString();
                }


            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //생산담당자
        private void btnProdManaDuty_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON 'B016' ,@pSPEC1='" + SystemBase.Base.gstrBIZCD + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'"; // 2020.07.02. hma 수정: 부서는 체크하지 않도록 부서 매개변수 제외함. , @pDEPT_CD = '01623'"
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtProdManaDuty.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P04003", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "담당자 조회");	//생산관리 사용자조회
                pu.Width = 450;
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtProdManaDuty.Text = Msgs[0].ToString();
                    txtProdManaDutyNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "생산담당자 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //부품팝업
        private void btnITEM_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW005 pu = new WNDW005(txtITEM_CD.Text);
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtITEM_CD.Value = Msgs[2].ToString();
                    txtITEM_NM.Value = Msgs[3].ToString();
                    txtITEM_CD.Focus();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnGroup_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW005 pu = new WNDW005(txtGroupCd.Text, "10");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtGroupCd.Value = Msgs[2].ToString();
                    txtGroupNm.Value = Msgs[3].ToString();
                    txtGroupCd.Focus();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //프로젝트팝업
        private void btnProject_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW003 pu = new WNDW003(txtProject_No.Text, "S1", "C");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtProject_No.Value = Msgs[3].ToString();
                    txtProject_Name.Value = Msgs[4].ToString();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnWoNoFr_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW006 pu = new WNDW006(txtWoNoFr.Text);
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtWoNoFr.Value = Msgs[1].ToString();
                    txtWoNoFr.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "제조오더번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnWoNoTo_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW006 pu = new WNDW006(txtWoNoTo.Text);
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtWoNoTo.Value = Msgs[1].ToString();
                    txtWoNoTo.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "제조오더번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnMakeOrderNoFr_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW008 pu = new WNDW008(txtMakeOrderNoFr.Text, "R");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtMakeOrderNoFr.Value = Msgs[1].ToString();
                    txtProject_No.Value = Msgs[6].ToString();
                    txtProject_Name.Value = Msgs[7].ToString();
                    txtMakeOrderNoFr.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "제품오더번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnMakeOrderNoTo_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW008 pu = new WNDW008(txtMakeOrderNoTo.Text, "R");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtMakeOrderNoTo.Value = Msgs[1].ToString();
                    txtProject_No.Value = Msgs[6].ToString();
                    txtProject_Name.Value = Msgs[7].ToString();
                    txtMakeOrderNoTo.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "제품오더번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //작업장
        private void btnWcCd_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pType='P610', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pETC = 'P002' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };
                string[] strSearch = new string[] { txtWcCd.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P05009", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "작업장 조회");
                pu.Width = 500;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtWcCd.Value = Msgs[0].ToString();
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
        private void btnJob_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pType='P042', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pETC = 'P001' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };
                string[] strSearch = new string[] { txtJobCd.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00025", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "공정작업코드 조회");
                pu.Width = 500;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtJobCd.Value = Msgs[0].ToString();
                    txtJobNm.Value = Msgs[1].ToString();
                    txtJobCd.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "작업장 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region TextChanged

        //생산담당자
        private void txtProdManaDuty_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtProdManaDuty.Text != "")
                {
                    txtProdManaDutyNm.Value = SystemBase.Base.CodeName("USR_ID", "USR_NM", "B_SYS_USER", txtProdManaDuty.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");
                }
                else
                {
                    txtProdManaDutyNm.Value = "";
                }
            }
            catch
            {

            }
        }

        // 부품
        private void txtITEM_CD_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtITEM_CD.Text != "")
                {
                    txtITEM_NM.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtITEM_CD.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");
                }
                else
                {
                    txtITEM_NM.Value = "";
                }
            }
            catch
            {

            }
        }

        // 공장
        private void txtPlant_CD_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtPlant_CD.Text != "")
                {
                    txtPlant_NM.Value = SystemBase.Base.CodeName("PLANT_CD", "PLANT_NM", "B_PLANT_INFO", txtPlant_CD.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");
                }
                else
                {
                    txtPlant_NM.Value = "";
                }
            }
            catch
            {

            }
        }
        private void txtProject_No_TextChanged(object sender, System.EventArgs e)
        {
            string Query = "SELECT TOP 1 PROJECT_NM FROM S_SO_MASTER(NOLOCK) WHERE PROJECT_NO = '" + txtProject_No.Text + "'  AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ";

            DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);

            if (dt.Rows.Count > 0)
            {
                txtProject_Name.Value = dt.Rows[0][0].ToString();
            }
            else
            {
                txtProject_Name.Value = "";
            }
        }
        private void txtWcCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtWcCd.Text != "")
                {
                    txtWcNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtWcCd.Text, " AND MAJOR_CD = 'P002'  AND COMP_CODE='" + SystemBase.Base.gstrCOMCD + "' ");
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
        private void txtGroupCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtGroupCd.Text != "")
                {
                    txtGroupNm.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtGroupCd.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");
                }
                else
                {
                    txtGroupNm.Value = "";
                }
            }
            catch
            {

            }
        }
        private void txtWoNoFr_TextChanged(object sender, System.EventArgs e)
        {
            txtWoNoTo.Value = txtWoNoFr.Text;
        }
        private void txtJobCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtJobCd.Text != "")
                {
                    txtJobNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtJobCd.Text, " AND MAJOR_CD = 'P001' AND COMP_CODE ='" + SystemBase.Base.gstrCOMCD + "' ");
                }
                else
                {
                    txtJobNm.Value = "";
                }
            }
            catch
            {

            }
        }
        #endregion

        #region 라디오버튼 Click
        private void rdoDivY_Click(object sender, System.EventArgs e)
        {
            label17.Text = "배포일";
        }

        private void rdoDivN_Click(object sender, System.EventArgs e)
        {
            label17.Text = "작업완료일";
        }

        private void rdoDivAll_Click(object sender, System.EventArgs e)
        {
            label17.Text = "작업완료일";
        }
        #endregion

        #region 부품내역
        private void btnItemSpec_Click(object sender, System.EventArgs e)
        {
            if (fpSpread1.Sheets[0].Rows.Count > 0)
            {
                int Row = fpSpread1.Sheets[0].ActiveRowIndex;

                if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "제조오더번호")].Text == "")
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0061", "제조오더번호"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                EISB02P2 form = new EISB02P2(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "제조오더번호")].Text, "");
                form.ShowDialog();
            }
        }
        #endregion

        #region 공정진행현황
        private void btnProcInfo_Click(object sender, System.EventArgs e)
        {
            if (fpSpread1.Sheets[0].Rows.Count > 0)
            {
                int Row = fpSpread1.Sheets[0].ActiveRowIndex;

                string ProjectNo = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트")].Text;
                string ProjectSeq = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Text;
                string ItemCd = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text;
                string WoNo = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "제조오더번호")].Text;

                EISB02P1 myForm = new EISB02P1(ProjectNo, ProjectSeq, ItemCd, WoNo);
                myForm.ShowDialog();
            }
        }
        #endregion

        
                
    }
}
