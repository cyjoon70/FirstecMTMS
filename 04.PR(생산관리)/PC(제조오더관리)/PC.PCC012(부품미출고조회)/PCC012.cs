#region 작성정보
/*********************************************************************/
// 단위업무명 : 부품미출고조회
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-02-13
// 작성내용 : 부품미출고조회 관리
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

namespace PC.PCC012
{
    public partial class PCC012 : UIForm.FPCOMM1
    {
        string end_date = "";
        private string strMQuery;

        public PCC012()
        {
            InitializeComponent();
        }

        public PCC012(string ed_date)
        {
            end_date = ed_date;
            InitializeComponent();
        }


        #region Form Load 시
        private void PCC012_Load(object sender, System.EventArgs e)
        {
            // 필수 확인
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            // 기본정보 바인딩
            txtPlant_CD.Text = SystemBase.Base.gstrPLANT_CD;
            dtpSTART_DT.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-4).ToShortDateString().Substring(0,10);
            dtpEND_DT.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).ToShortDateString().Substring(0, 10);

            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "단위")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'Z005', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//단위
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "재고단위")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'Z005', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//재고단위
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "요청단위")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'Z005', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//요청단위
            
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

            rdoNo.Checked = true;
            rdo4.Checked = true;

            if (end_date != "")
            {
                dtpSTART_DT.Value = "";
                dtpEND_DT.Value = end_date;
                SystemBase.Validation.GroupBoxControlsLock(groupBox1, true);
                SearchExec();
            }

        }
        #endregion

        #region NewExec()
        protected override void NewExec()
        {
            // 초기화
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            // 기본정보 바인딩
            rdoNo.Checked = true;
            rdo4.Checked = true;
            txtPlant_CD.Text = SystemBase.Base.gstrPLANT_CD;
            dtpSTART_DT.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-4).ToShortDateString().Substring(0,10);
            dtpEND_DT.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).ToShortDateString().Substring(0, 10);
            dtpSoDelvDtFr.Value = null;
            dtpSoDelvDtTo.Value = null;
            dtpReportDtFr.Value = null;
            dtpReportDtTo.Value = null;

            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "단위")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'Z005', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//단위
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "재고단위")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'Z005', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//재고단위
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "요청단위")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'Z005', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//요청단위
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
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

                    string ReportChk = "";
                    if (rdoReportYes.Checked == true)
                        ReportChk = "Y";
                    else if (rdoReportNo.Checked == true)
                        ReportChk = "N";

                    string InOutFlag = "";
                    if (rdo1.Checked == true)
                        InOutFlag = "1";
                    else if (rdo2.Checked == true)
                        InOutFlag = "2";
                    else if (rdo3.Checked == true)
                        InOutFlag = "3";
                    else if (rdo4.Checked == true)
                        InOutFlag = "4";

                    string strOrderStatus = "";
                    if (rdoStatusR.Checked == true) strOrderStatus = "2";    //release
                    else if (rdoStatusS.Checked == true) strOrderStatus = "3";	//start
                    else if (rdoStatusRS.Checked == true) strOrderStatus = "4";	//release/start
                    else if (rdoStatusC.Checked == true) strOrderStatus = "5";	//close

                    strMQuery = " usp_PCC012 'S1'";
                    strMQuery += ", @pPLANT_CD='" + txtPlant_CD.Text + "'";
                    strMQuery += ", @pSTART_DT='" + dtpSTART_DT.Text.ToString() + "'";
                    strMQuery += ", @pEND_DT='" + dtpEND_DT.Text.ToString() + "'";
                    strMQuery += ", @pITEM_CD='" + txtITEM_CD.Text.Trim() + "'";
                    strMQuery += ", @pWORKORDER_NO_FR ='" + txtWoNoFr.Text + "'";
                    strMQuery += ", @pWORKORDER_NO_TO ='" + txtWoNoTo.Text + "'";
                    strMQuery += ", @pSL_CD='" + txtSL_CD.Text + "'";
                    strMQuery += ", @pPROJECT_NO='" + txtProject_No.Text + "'";
                    strMQuery += ", @pPROJECT_SEQ ='" + txtProject_Seq.Text + "'";
                    strMQuery += ", @pGROUP_CD='" + txtGroup_CD.Text + "'";
                    strMQuery += ", @pWC_CD='" + txtWc_CD.Text.Trim() + "'";
                    strMQuery += ", @pISSUED_FLAG ='" + Chk + "'";
                    strMQuery += ", @pITEM_TYPE ='" + txtItemTypeCd.Text + "'";
                    strMQuery += ", @pBIZ_CD ='" + SystemBase.Base.gstrBIZCD + "'";
                    strMQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'";
                    strMQuery += ", @pWORKORDER_NO_RS = '" + txtWorkorderNoRs.Text + "'";
                    strMQuery += ", @pMAKEORDER_NO = '" + txtMakeorder_No.Text + "'";
                    strMQuery += ", @pISSUED_MTHD = '" + IssueChk + "'";
                    strMQuery += ", @pPRNT_ITEM_CD = '" + txtPrntItemCd.Text + "'";
                    strMQuery += ", @pREPORT_YN = '" + ReportChk + "'";
                    strMQuery += ", @pIN_OUT_FLAG = '" + InOutFlag + "'";
                    strMQuery += ", @pSO_DELV_DT_FR='" + dtpSoDelvDtFr.Text.ToString() + "'";
                    strMQuery += ", @pSO_DELV_DT_TO='" + dtpSoDelvDtTo.Text.ToString() + "'";
                    strMQuery += ", @pORDER_STATUS = '" + strOrderStatus + "'";
                    strMQuery += ", @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                    strMQuery += ", @pREPORT_DT_FR='" + dtpReportDtFr.Text.ToString() + "'";
                    strMQuery += ", @pREPORT_DT_TO='" + dtpReportDtTo.Text.ToString() + "'";


                    UIForm.FPMake.grdCommSheet(fpSpread1, strMQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 8, true);

                    if (fpSpread1.Sheets[0].Rows.Count > 0)
                    {
                        int cnt = 0;

                        for (int i = 0; i < fpSpread1.Sheets[0].RowCount; i++)
                        {
                            cnt = cnt + Convert.ToInt32(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "출고율(%)")].Value);

                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "출고잔량")].Text != ""
                                && fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "출고율(%)")].Text == "100")
                            {
                                fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "출고율(%)")].ForeColor = Color.Red;
                            }

                        }

                        int RowCnt = fpSpread1.Sheets[0].RowCount;

                        cnt = cnt / RowCnt;

                        txtIssuePer.Value = cnt.ToString();
                    }
                    else
                    {
                        txtIssuePer.Value = "";
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

        private void btnWc_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pType='P042', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pETC = 'P002' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };
                string[] strSearch = new string[] { txtWc_CD.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00025", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "작업장 조회", true);
                pu.Width = 500;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtWc_CD.Text = Msgs[0].ToString();
                    txtWc_NM.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        //대표오더번호
        private void btnWorkorderNoRs_Click(object sender, System.EventArgs e)
        {
            try
            {
                PCC007P3 pu = new PCC007P3(txtWorkorderNoRs.Text);
                pu.Width = 700;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtWorkorderNoRs.Text = Msgs[1].ToString();
                    txtWorkorderNoRs.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "대표오더번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //모품목
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

        //품목구분
        private void btnItemType_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON @pType='COMM_POP', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pSPEC1 = 'P032', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtItemTypeCd.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00105", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "품목구분 조회", false);

                pu.ShowDialog();	//공통 팝업 호출
                if (pu.DialogResult == DialogResult.OK)
                {
                    string MSG = pu.ReturnVal.Replace("|", "#");
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(MSG);

                    txtItemTypeCd.Text = Msgs[0].ToString();
                    txtItemTypeNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목구분팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region TextChanged
        // 작업장
        private void txtWc_CD_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtWc_CD.Text != "")
                {
                    txtWc_NM.Value = SystemBase.Base.CodeName("RES_CD", "RES_DIS", "P_RESO_MANAGE", txtWc_CD.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");
                }
                else
                {
                    txtWc_NM.Value = "";
                }
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
            catch { }
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
            catch { }
        }

        // 창고
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
            catch { }
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
            catch { }
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
        //모품목
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
            catch { }
        }

        //품목구분
        private void txtItemTypeCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtItemTypeCd.Text != "")
                {
                    txtItemTypeNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtItemTypeCd.Text, " AND MAJOR_CD = 'P032' AND COMP_CODE='" + SystemBase.Base.gstrCOMCD + "' ");
                }
                else
                {
                    txtItemTypeNm.Value = "";
                }
            }
            catch { }
        }
        #endregion

        #region 공정진행현황조회
        private void btnProcInfo_Click(object sender, System.EventArgs e)
        {
            if (fpSpread1.Sheets[0].Rows.Count > 0)
            {
                int Row = fpSpread1.Sheets[0].ActiveRowIndex;

                string ProjectNo = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text;
                string ProjectSeq = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Text;
                string ItemCd = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "모품목코드")].Text;
                string WoNo = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "제조오더번호")].Text;

                PCC007P1 myForm = new PCC007P1(ProjectNo, ProjectSeq, ItemCd, WoNo);
                myForm.ShowDialog();
            }
        }
        #endregion

        #region 제품오더번호 팝업
        private void btnMakeorder_No_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW008 pu = new WNDW008(txtMakeorder_No.Text);
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtMakeorder_No.Text = Msgs[1].ToString();
                    txtMakeorder_No.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "제품오더번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 제품 팝업
        private void btnGroup_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW005 pu = new WNDW005("10");
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
        #endregion


    }
}
