#region 작성정보
/*********************************************************************/
// 단위업무명 : 작업일보집계현황
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-02-13
// 작성내용 : 작업일보집계현황 관리
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
using FarPoint.Win.Spread.CellType;

namespace PE.PEA003
{
    public partial class PEA003 : UIForm.FPCOMM1
    {
        int lastCol = 30;

        public PEA003()
        {
            InitializeComponent();
        }


        #region Form Load 시
        private void PEA003_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            //콤보박스 세팅
            SystemBase.ComboMake.C1Combo(cboPlantCd, "usp_B_COMMON @pType='TABLE2', @pCODE = 'PLANT_CD', @pNAME = 'PLANT_NM', @pSPEC1 = 'B_PLANT_INFO', @pCO_CD='" + SystemBase.Base.gstrCOMCD +"'", 0);//공장

            //기타 세팅
            cboPlantCd.SelectedValue = SystemBase.Base.gstrPLANT_CD;
            dtpWorkDtFr.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToString().Substring(0, 10);
            dtpWorkDtTo.Value = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0, 10);

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
        }
        #endregion

        #region 그리드 디자인
        private void Grd_Set()
        {
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

            try
            {
                string strQuery = " usp_PEA003  @pTYPE = 'S2', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

                DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

                if (dt.Rows.Count > 0)
                {
                    //그리드 헤드 디자인
                    fpSpread1.Sheets[0].AlternatingRows[0].BackColor = Color.FromArgb(230, 230, 230);
                    fpSpread1.Sheets[0].AlternatingRows[1].BackColor = Color.FromArgb(245, 245, 245);

                    fpSpread1.Sheets[0].Columns[1].CellType = new TextCellType();
                    FarPoint.Win.Spread.CellType.TextCellType textCellType1 = new FarPoint.Win.Spread.CellType.TextCellType();
                    textCellType1.Multiline = true;
                    textCellType1.WordWrap = true;
                    fpSpread1.Sheets[0].Columns.Get(1).CellType = textCellType1;

                    fpSpread1.Sheets[0].ColumnCount = 30 + dt.Rows.Count + 1;
                    fpSpread1.Sheets[0].ColumnHeader.Columns.Count = fpSpread1.Sheets[0].ColumnCount;
                    FarPoint.Win.Spread.CellType.NumberCellType num = new FarPoint.Win.Spread.CellType.NumberCellType();
                    FarPoint.Win.Spread.CellType.PercentCellType num1 = new FarPoint.Win.Spread.CellType.PercentCellType();
                    num.DecimalSeparator = ".";
                    num.DecimalPlaces = 2;
                    num.FixedPoint = true;
                    num.Separator = ",";
                    num.ShowSeparator = true;
                    num.MaximumValue = 99999999999999;
                    num.MinimumValue = -99999999999999;

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        fpSpread1.Sheets[0].ColumnHeader.Cells[0, i + 30].Text = dt.Rows[i]["CD_NM"].ToString();

                        fpSpread1.Sheets[0].Columns[i + 30].CellType = num;
                        fpSpread1.Sheets[0].Columns[i + 30].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;
                        fpSpread1.Sheets[0].Columns[i + 30].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
                        fpSpread1.Sheets[0].Columns[i + 30].Locked = true;
                        fpSpread1.Sheets[0].Columns[i + 30].Width = 80;

                    }

                    lastCol += dt.Rows.Count;
                    fpSpread1.Sheets[0].ColumnHeader.Cells[0, lastCol].Text = "비고";
                    fpSpread1.Sheets[0].Columns[lastCol].CellType = textCellType1;
                    fpSpread1.Sheets[0].Columns[lastCol].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
                    fpSpread1.Sheets[0].Columns[lastCol].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
                    fpSpread1.Sheets[0].Columns[lastCol].Locked = true;
                    fpSpread1.Sheets[0].Columns[lastCol].Width = 200;
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "그리드생성"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }
        #endregion

        #region 조회조건 팝업
        //품목코드
        private void btnItemCd_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW005 pu = new WNDW005(cboPlantCd.SelectedValue.ToString(), true, txtItemCd.Text);
                pu.MaximizeBox = false;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtItemCd.Text = Msgs[2].ToString();
                    txtItemNm.Value = Msgs[3].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목코드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        //프로젝트번호
        private void btnProjectNo_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW007 pu = new WNDW007(txtProjectNo.Text);
                pu.MaximizeBox = false;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;
                    txtEntCd.Value = Msgs[1].ToString();
                    txtProjectNo.Text = Msgs[3].ToString();
                    txtProjectNm.Value = Msgs[4].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "프로젝트번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //작업일보번호 FROM
        private void btnWorkDayNoFr_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pTYPE = 'P120', @pCO_CD='" + SystemBase.Base.gstrCOMCD +"'";				// 쿼리
                string[] strWhere = new string[] { "@pCOM_CD" };						// 쿼리 인자값(조회조건)
                string[] strSearch = new string[] { txtWorkDayNoFr.Text };			// 쿼리 인자값에 들어갈 데이타

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00076", strQuery, strWhere, strSearch, new int[] { 0 }, "작업일보번호 조회", false);
                pu.ShowDialog();	//공통 팝업 호출
                if (pu.DialogResult == DialogResult.OK)
                {
                    string MSG = pu.ReturnVal.Replace("|", "#");
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(MSG);

                    txtWorkDayNoFr.Text = Msgs[0].ToString();
                    txtWorkDayNoFr.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "작업일보번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //작업일보번호 TO
        private void btnWorkDayNoTo_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pTYPE = 'P120' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";				// 쿼리
                string[] strWhere = new string[] { "@pCOM_CD" };						// 쿼리 인자값(조회조건)
                string[] strSearch = new string[] { txtWorkDayNoTo.Text };			// 쿼리 인자값에 들어갈 데이타

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00076", strQuery, strWhere, strSearch, new int[] { 0 }, "작업일보번호 조회", false);
                pu.ShowDialog();	//공통 팝업 호출
                if (pu.DialogResult == DialogResult.OK)
                {
                    string MSG = pu.ReturnVal.Replace("|", "#");
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(MSG);

                    txtWorkDayNoTo.Text = Msgs[0].ToString();
                    txtWorkDayNoTo.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "작업일보번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //제조오더번호 FROM
        private void btnWorkorderNoFr_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW006 pu = new WNDW006(txtWorkorderNoFr.Text);
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtWorkorderNoFr.Text = Msgs[1].ToString();
                    txtWorkorderNoFr.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "제조오더번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //제조오더번호 TO
        private void btnWorkorderNoTo_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW006 pu = new WNDW006(txtWorkorderNoTo.Text);
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtWorkorderNoTo.Text = Msgs[1].ToString();
                    txtWorkorderNoTo.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "제조오더번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //작업자
        private void btnWorkDuty_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pTYPE = 'P054' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";				// 쿼리
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };			// 쿼리 인자값(조회조건)
                string[] strSearch = new string[] { txtWorkDuty.Text, "" };							// 쿼리 인자값에 들어갈 데이타

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00071", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "작업자 조회", false);
                pu.Width = 600;
                pu.ShowDialog();	//공통 팝업 호출
                if (pu.DialogResult == DialogResult.OK)
                {
                    string MSG = pu.ReturnVal.Replace("|", "#");
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(MSG);

                    txtWorkDuty.Text = Msgs[0].ToString();
                    txtWorkDutyNm.Value = Msgs[1].ToString();
                    txtWorkDuty.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "작업자 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //작업장
        private void btnWcCd_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pTYPE = 'P042', @pLANG_CD = 'KOR', @pETC = 'P061', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";					// 쿼리
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
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "부서 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //제품오더번호 FROM
        private void btnMakeorderNoFr_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW008 pu = new WNDW008(txtMakeorderNoFr.Text, "R");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtMakeorderNoFr.Text = Msgs[1].ToString();
                    txtMakeorderNoFr.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "제품오더번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //제품오더번호 TO
        private void btnMakeorderNoTo_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW008 pu = new WNDW008(txtMakeorderNoTo.Text, "R");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtMakeorderNoTo.Text = Msgs[1].ToString();
                    txtMakeorderNoTo.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "제품오더번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        #endregion

        #region 조회조건 TextChanged
        //품목코드
        private void txtItemCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtItemCd.Text != "")
                {
                    txtItemNm.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtItemCd.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");
                }
                else
                {
                    txtItemNm.Value = "";
                }
            }
            catch { }
        }

        //프로젝트번호
        private void txtProjectNo_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtProjectNo.Text != "")
                {
                    txtProjectNm.Value = SystemBase.Base.CodeName("PROJECT_NO", "PROJECT_NM", "S_SO_MASTER", txtProjectNo.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");
                }
                else
                {
                    txtProjectNm.Value = "";
                }
                if (txtProjectNm.Text == "")
                { txtEntCd.Text = ""; }
                else
                { txtEntCd.Text = SystemBase.Base.CodeName("PROJECT_NO", "ENT_CD", "S_SO_MASTER", txtProjectNo.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' "); }
            }
            catch { }
        }

        //사업코드
        private void txtEntCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtEntCd.Text != "")
                {
                    txtEntNm.Value = SystemBase.Base.CodeName("ENT_CD", "ENT_NM", "S_ENTERPRISE_INFO", txtEntCd.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");
                }
                else
                {
                    txtEntNm.Value = "";
                }
            }
            catch { }
        }

        //작업자
        private void txtWorkDuty_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtWorkDuty.Text != "")
                {
                    txtWorkDutyNm.Value = SystemBase.Base.CodeName("RES_CD", "RES_DIS", "P_RESO_MANAGE", txtWorkDuty.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");
                }
                else
                {
                    txtWorkDutyNm.Value = "";
                }
            }
            catch { }
        }

        //작업장
        private void txtWcCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtWcCd.Text != "")
                {
                    txtWcNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtWcCd.Text, " AND MAJOR_CD = 'P061'  AND COMP_CODE='" + SystemBase.Base.gstrCOMCD + "' ");
                }
                else
                {
                    txtWcNm.Value = "";
                }
            }
            catch { }
        }
        #endregion

        #region NewExec()
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            fpSpread1.Sheets[0].Rows.Count = 0;

            //기타 세팅
            cboPlantCd.SelectedValue = SystemBase.Base.gstrPLANT_CD;
            dtpWorkDtFr.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToString().Substring(0, 10);
            dtpWorkDtTo.Value = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0, 10);
            rdoAutoFlagAll.Checked = true;
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {
                string strAutoFlg = "";

                if (rdoAutoFlagY.Checked == true) { strAutoFlg = "Y"; }
                else if (rdoAutoFlagN.Checked == true) { strAutoFlg = "N"; }
                else { strAutoFlg = ""; }

                string strQa = "N";
                if(chkQa.Checked == true)
                {
                    strQa = "Y";
                }

                string strQuery = " usp_PEA003  @pTYPE = 'S1'";
                strQuery += ", @pPROJECT_NO = '" + txtProjectNo.Text + "'";
                strQuery += ", @pWORK_DAY_NO_FR = '" + txtWorkDayNoFr.Text + "'";
                strQuery += ", @pWORK_DAY_NO_TO = '" + txtWorkDayNoTo.Text + "'";
                strQuery += ", @pWORKORDER_NO_FR = '" + txtWorkorderNoFr.Text + "'";
                strQuery += ", @pWORKORDER_NO_TO = '" + txtWorkorderNoTo.Text + "'";
                strQuery += ", @pPLANT_CD = '" + cboPlantCd.SelectedValue.ToString() + "'";
                strQuery += ", @pWORK_DUTY = '" + txtWorkDuty.Text + "'";
                strQuery += ", @pITEM_CD = '" + txtItemCd.Text + "'";
                strQuery += ", @pWC_CD = '" + txtWcCd.Text + "'";
                strQuery += ", @pWORK_DT_FR = '" + dtpWorkDtFr.Text + "'";
                strQuery += ", @pWORK_DT_TO = '" + dtpWorkDtTo.Text + "'";
                strQuery += ", @pAUTO_FLAG = '" + strAutoFlg + "'";
                strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'";

                strQuery += ", @pPROJECT_SEQ = '" + txtProjectSeq.Text + "'";
                strQuery += ", @pMAKEORDER_NO_FR = '" + txtMakeorderNoFr.Text + "' ";
                strQuery += ", @pMAKEORDER_NO_TO = '" + txtMakeorderNoTo.Text + "' ";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                strQuery += ", @pCHK_QA = '" + strQa + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0, true);

                if (fpSpread1.Sheets[0].RowCount > 0)
                {
                    decimal dDirectTm = 0;
                    decimal dIndirectTm = 0;
                    decimal dPDirectTm = 0;
                    decimal dPIndirectTm = 0;

                    for (int i = 0; i < fpSpread1.Sheets[0].RowCount; i++)
                    {
                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "오더구분")].Text != "간접")
                        {
                            dDirectTm += Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "실동시간(분)")].Value);
                        }

                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "오더구분")].Text == "간접")
                        {
                            dIndirectTm += Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "실동시간(분)")].Value);
                        }

                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "오더구분")].Text == "표준")
                        {
                            dPDirectTm += Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "실동시간(분)")].Value);
                        }

                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "오더구분")].Text == "표준")
                        {
                            dPIndirectTm += Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "간접시간(분)")].Value);
                        }
                    }
                    dtxtDirectTm.ReadOnly = false;
                    dtxtDirectTm.Value = dDirectTm.ToString();
                    dtxtDirectTm.ReadOnly = true;

                    dtxtIndirectTm.ReadOnly = false;
                    dtxtIndirectTm.Value = dIndirectTm.ToString();
                    dtxtIndirectTm.ReadOnly = true;

                    dtxtTotTm.ReadOnly = false;
                    dtxtTotTm.Value = Convert.ToString((dDirectTm + dIndirectTm));
                    dtxtTotTm.ReadOnly = true;

                    dtxtPDirectTm.ReadOnly = false;
                    dtxtPDirectTm.Value = dPDirectTm.ToString();
                    dtxtPDirectTm.ReadOnly = true;

                    dtxtPIndirectTm.ReadOnly = false;
                    dtxtPIndirectTm.Value = dPIndirectTm.ToString();
                    dtxtPIndirectTm.ReadOnly = true;

                    dtxtPTotTm.ReadOnly = false;
                    dtxtPTotTm.Value = Convert.ToString((dPDirectTm + dPIndirectTm));
                    dtxtPTotTm.ReadOnly = true;
                }
                else
                {
                    dtxtDirectTm.ReadOnly = false;
                    dtxtDirectTm.Value = "0";
                    dtxtDirectTm.ReadOnly = true;

                    dtxtIndirectTm.ReadOnly = false;
                    dtxtIndirectTm.Value = "0";
                    dtxtIndirectTm.ReadOnly = true;

                    dtxtTotTm.ReadOnly = false;
                    dtxtTotTm.Value = "0";
                    dtxtTotTm.ReadOnly = true;

                    dtxtPDirectTm.ReadOnly = false;
                    dtxtPDirectTm.Value = "0";
                    dtxtPDirectTm.ReadOnly = true;

                    dtxtPIndirectTm.ReadOnly = false;
                    dtxtPIndirectTm.Value = "0";
                    dtxtPIndirectTm.ReadOnly = true;

                    dtxtPTotTm.ReadOnly = false;
                    dtxtPTotTm.Value = "0";
                    dtxtPTotTm.ReadOnly = true;
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

        #region 프로젝트 차수 버튼 클릭
        private void btnProjectSeq_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON 'PROJ_SEQ', @pSPEC1 = '" + txtProjectNo.Text + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";										// 쿼리
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };			// 쿼리 인자값(조회조건)
                string[] strSearch = new string[] { "", "" };		// 쿼리 인자값에 들어갈 데이타

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P09001", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "프로젝트차수 조회", false);
                pu.Width = 400;
                pu.ShowDialog();	//공통 팝업 호출

                if (pu.DialogResult == DialogResult.OK)
                {
                    string MSG = pu.ReturnVal.Replace("|", "#");
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(MSG);
                    txtProjectSeq.Text = Msgs[0].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }
        #endregion

    }
}
