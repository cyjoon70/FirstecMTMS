#region 작성정보
/*********************************************************************/
// 단위업무명 : 부품출고등록
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-03-22
// 작성내용 : 부품출고등록 관리
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

namespace PC.PCC022
{
    public partial class PCC022 : UIForm.FPCOMM1
    {
        #region 변수선언
        private string strMQuery;
        bool SaveChk = false;
        #endregion

        #region 생성자
        public PCC022()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void PCC022_Load(object sender, System.EventArgs e)
        {
            // 필수 확인
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            SystemBase.Validation.GroupBox_Setting(groupBox2);

            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "단위")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'Z005', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//단위
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "재고단위")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'Z005', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//재고단위
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "작업장")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'P002', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//작업장
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "창고")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='B032', @pSPEC1 = '" + SystemBase.Base.gstrPLANT_CD + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//창고
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "LOCATION")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='LOC', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//LOCATION
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

            // 기본정보 바인딩
            txtPlant_CD.Text = SystemBase.Base.gstrPLANT_CD;
            dtpSTART_DT.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-6).ToShortDateString().Substring(0,10);
            dtpEND_DT.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(6).ToShortDateString().Substring(0,10);
            dtpOutDt.Text = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0,10);
            dtpReportDtFr.Text = "";
            dtpReportDtTo.Text = "";
        }
        #endregion

        #region NewExec()
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            SystemBase.Validation.GroupBox_Reset(groupBox2);

            txtPlant_CD.Text = SystemBase.Base.gstrPLANT_CD;
            dtpSTART_DT.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-6).ToShortDateString().Substring(0,10);
            dtpEND_DT.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(6).ToShortDateString().Substring(0,10);
            dtpOutDt.Text = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0,10);
            dtpReportDtFr.Text = "";
            dtpReportDtTo.Text = "";
            rdoNo.Checked = true;
            rdoIssueNo.Checked = true;
            //그리드 초기화
            fpSpread1.Sheets[0].Rows.Count = 0;
        }
        #endregion
        
        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
            DialogResult dsMsg;
            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {
                    string Chk = "N";
                    if (rdoYes.Checked == true)
                        Chk = "Y";
                    else if (rdoAll.Checked == true)
                        Chk = "";

                    string IssueChk = "";
                    if (rdoIssueYes.Checked == true)
                        IssueChk = "A";
                    else if (rdoIssueNo.Checked == true)
                        IssueChk = "M";

                    string ClChk = "";
                    if (rdoClyes.Checked == true)
                    {
                        ClChk = "Y";
                    }
                    else if (rdoClno.Checked == true)
                    {
                        ClChk = "N";
                    }

                    string ReportChk = "N";
                    if (chkReport.Checked == true)
                    {
                        ReportChk = "Y";
                    }

                    strMQuery = " usp_PCC022 'S1'";
                    strMQuery += ", @pPLANT_CD='" + txtPlant_CD.Text + "'";
                    strMQuery += ", @pSTART_DT='" + dtpSTART_DT.Text.ToString() + "'";
                    strMQuery += ", @pEND_DT='" + dtpEND_DT.Text.ToString() + "'";
                    strMQuery += ", @pITEM_CD='" + txtITEM_CD.Text.Trim() + "'";
                    strMQuery += ", @pWORKORDER_NO_FR ='" + txtWoNoFr.Text + "'";
                    strMQuery += ", @pWORKORDER_NO_TO ='" + txtWoNoTo.Text + "'";
                    strMQuery += ", @pSL_CD='" + txtSSL_CD.Text + "'";
                    strMQuery += ", @pPROJECT_NO='" + txtProject_No.Text + "'";
                    strMQuery += ", @pPROJECT_SEQ ='" + txtProject_Seq.Text + "'";
                    strMQuery += ", @pGROUP_CD='" + txtGroup_CD.Text + "'";
                    strMQuery += ", @pWC_CD='" + txtSWc_CD.Text.Trim() + "'";
                    strMQuery += ", @pISSUED_FLAG ='" + Chk + "'";
                    strMQuery += ", @pBIZ_CD ='" + SystemBase.Base.gstrBIZCD + "'";
                    strMQuery += ", @pISSUED_MTHD ='" + IssueChk + "'";
                    strMQuery += ", @pCLOSE_YN ='" + ClChk + "'";
                    strMQuery += ", @pREPORT_YN = '" + ReportChk + "' ";
                    strMQuery += ", @pREPORT_DT_FR ='" + dtpReportDtFr.Text.ToString() + "'";
                    strMQuery += ", @pREPORT_DT_TO ='" + dtpReportDtTo.Text.ToString() + "'";
                    strMQuery += ", @pCUST_CD = '" + txtCustCd.Text + "' ";
                    strMQuery += ", @pPO_NO_FR ='" + txtPoNoFr.Text + "'";
                    strMQuery += ", @pPO_NO_TO ='" + txtPoNoTo.Text + "'";
                    strMQuery += ", @pPRNT_ITEM_CD = '" + txtPrntItemCd.Text + "'";
                    strMQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strMQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 3);

                    for (int i = 0; i < fpSpread1.Sheets[0].RowCount; i++)
                    {
                        if (Convert.ToDouble(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "출고잔량")].Value) <= 0 ||
                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "출고방법")].Text == "자동")
                        {
                            UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "선택") + "|3"
                                                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "출고수량") + "|3");
                        }

                        if (Convert.ToDouble(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "출고잔량")].Value) > Convert.ToDouble(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "양품재고수량")].Value))
                        {
                            fpSpread1.Sheets[0].Rows[i].ForeColor = Color.Red;
                        }

                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "오더상태")].Text == "CL")
                        {
                            UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "선택") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "출고수량") + "|3");
                        }
                    }

                    for (int i = 0; i < fpSpread1.Sheets[0].RowCount; i++)
                    {
                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "LOCATION")].Text == "")
                        {
                            fpSpread1.Sheets[0].Cells[i, 4].BackColor = Color.Bisque;
                            fpSpread1.Sheets[0].Cells[i, 5].BackColor = Color.Bisque;
                            fpSpread1.Sheets[0].Cells[i,5].ColumnSpan = 37;
                        }
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            this.Cursor = System.Windows.Forms.Cursors.Default;
        }
        #endregion

        #region fpSpread1_Change
        protected override void fpSpread1_ChangeEvent(int Row, int Column)
        {
            DialogResult dsMsg;
            //출고수량 check
            if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "출고수량"))
            {
                double out_qty = Convert.ToDouble(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "출고수량")].Value);
                double rest_qty = Convert.ToDouble(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "출고잔량")].Value);
                double on_qty = Convert.ToDouble(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "양품재고수량")].Value);

                if (out_qty > on_qty)
                {
                    dsMsg = MessageBox.Show(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "제조오더번호")].Text + " : "
                                            + fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "공정순서")].Text + " : "
                                            + fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "부품")].Text
                                            + " - 출고수량은 양품재고수량보다 많을 수 없습니다.", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    fpSpread1.ActiveSheet.SetActiveCell(Row, Column);
                    SaveChk = true;
                }
                else if (out_qty > rest_qty)
                {
                    dsMsg = MessageBox.Show(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "제조오더번호")].Text + " : "
                                            + fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "공정순서")].Text + " : "
                                            + fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "부품")].Text
                                            + " - 출고수량은 출고잔량보다 많을 수 없습니다.", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    fpSpread1.ActiveSheet.SetActiveCell(Row, Column);
                    SaveChk = true;
                }
                else
                {
                    SaveChk = false;
                }

            }
        }
        #endregion

        #region 버튼 Click
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

                    txtPlant_CD.Text = Msgs[0].ToString();
                    txtPlant_NM.Value = Msgs[1].ToString();
                }


            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnITEM_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW005 pu = new WNDW005();
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtITEM_CD.Text = Msgs[2].ToString();
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

        private void btnPrntItem_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW005 pu = new WNDW005();
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtPrntItemCd.Text = Msgs[2].ToString();
                    txtPrntItemNm.Value = Msgs[3].ToString();
                    txtPrntItemCd.Focus();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnGroupCd_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW005 pu = new WNDW005();
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtGroup_CD.Text = Msgs[2].ToString();
                    txtGROUP_NM.Value = Msgs[3].ToString();
                    txtGroup_CD.Focus();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnProject_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW003 pu = new WNDW003(txtProject_No.Text, "S1", "C");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtProject_No.Text = Msgs[3].ToString();
                    txtProject_Name.Value = Msgs[4].ToString();
                    txtProject_Seq.Text = Msgs[5].ToString();
                    txtGroup_CD.Text = Msgs[6].ToString();
                    txtGROUP_NM.Value = Msgs[7].ToString();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSWc_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pType='P042', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pETC = 'P002' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };
                string[] strSearch = new string[] { txtSWc_CD.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00025", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "작업장 조회", true);
                pu.Width = 500;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtSWc_CD.Text = Msgs[0].ToString();
                    txtSWc_NM.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSSL_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON @pType='B035', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pSPEC1 = '" + txtPlant_CD.Text + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtSSL_CD.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00056", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "창고 조회", false);

                pu.ShowDialog();	//공통 팝업 호출
                if (pu.DialogResult == DialogResult.OK)
                {
                    string MSG = pu.ReturnVal.Replace("|", "#");
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(MSG);

                    txtSSL_CD.Text = Msgs[0].ToString();
                    txtSSL_NM.Value = Msgs[1].ToString();
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

                    txtWoNoFr.Text = Msgs[1].ToString();
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

                    txtWoNoTo.Text = Msgs[1].ToString();
                    txtWoNoTo.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "제조오더번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSL_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON @pType='B035', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pSPEC1 = '" + txtPlant_CD.Text + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtSL_CD.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00056", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "창고 조회", false);

                pu.ShowDialog();	//공통 팝업 호출
                if (pu.DialogResult == DialogResult.OK)
                {
                    string MSG = pu.ReturnVal.Replace("|", "#");
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(MSG);

                    txtSL_CD.Text = Msgs[0].ToString();
                    txtSL_NM.Value = Msgs[1].ToString();
                }


            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void btnCust_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW002 pu = new WNDW002(txtCustCd.Text, "");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtCustCd.Text = Msgs[1].ToString();
                    txtCustNm.Value = Msgs[2].ToString();

                    txtCustCd.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "외주거래처 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region TextChanged
        // 작업장
        private void txtSWc_CD_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtSWc_CD.Text != "")
                {
                    txtSWc_NM.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtSWc_CD.Text, "AND MAJOR_CD = 'P002' AND COMP_CODE='" + SystemBase.Base.gstrCOMCD + "' ");
                }
                else
                {
                    txtSWc_NM.Value = "";
                }
            }
            catch
            {

            }
        }
        //프로젝트번호
        private void txtProject_No_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtProject_No.Text != "")
                {
                    txtProject_Name.Value = SystemBase.Base.CodeName("PROJECT_NO", "PROJECT_NM", "S_SO_MASTER", txtProject_No.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");
                }
                else
                {
                    txtProject_Name.Value = "";
                }
                if (txtProject_Name.Text == "")
                    txtProject_Seq.Text = "";
            }
            catch { }
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

        // 창고
        private void txtSSL_CD_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtSSL_CD.Text != "")
                {
                    txtSSL_NM.Value = SystemBase.Base.CodeName("SL_CD", "SL_NM", "B_STORAGE_INFO", txtSSL_CD.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");
                }
                else
                {
                    txtSSL_NM.Value = "";
                }
            }
            catch
            {

            }
        }

        // 제품코드
        private void txtGroup_CD_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtGroup_CD.Text != "")
                {
                    txtGROUP_NM.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtGroup_CD.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");
                }
                else
                {
                    txtGROUP_NM.Value = "";
                }
            }
            catch
            {

            }
        }

        private void txtSL_CD_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtSL_CD.Text != "")
                {
                    txtSL_NM.Value = SystemBase.Base.CodeName("SL_CD", "SL_NM", "B_STORAGE_INFO", txtSL_CD.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");
                }
                else
                {
                    txtSL_NM.Value = "";
                }
            }
            catch
            {

            }
        }

        private void txtPrntItemCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtPrntItemCd.Text != "")
                {
                    txtPrntItemNm.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtPrntItemCd.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");
                }
                else
                {
                    txtPrntItemNm.Value = "";
                }
            }
            catch
            {

            }
        }

        private void txtCustCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtCustCd.Text != "")
                {
                    txtCustNm.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtCustCd.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");
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

        #region 일괄선택 & 일괄취소
        private void btnSelectAll_Click(object sender, System.EventArgs e)
        {
            for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
            {
                if (Convert.ToDouble(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "양품재고수량")].Value.ToString()) > 0
                    && Convert.ToDouble(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "출고잔량")].Value.ToString()) > 0)
                {
                    if (Convert.ToDouble(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "양품재고수량")].Value.ToString())
                        >= Convert.ToDouble(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "출고잔량")].Value.ToString()))
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "출고수량")].Value
                            = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "출고잔량")].Value;
                    else
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "출고수량")].Value
                            = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "양품재고수량")].Value;

                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "출고방법")].Text == "수동")
                    {
                        UIForm.FPMake.fpChange(fpSpread1, i);

                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "선택")].Text = "True";
                    }
                }
            }
        }

        private void btnSelectCancel_Click(object sender, System.EventArgs e)
        {
            for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
            {
                fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "출고수량")].Value = 0;
                fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text = "";
                fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "선택")].Text = "False";
            }
        }
        #endregion

    }
}
