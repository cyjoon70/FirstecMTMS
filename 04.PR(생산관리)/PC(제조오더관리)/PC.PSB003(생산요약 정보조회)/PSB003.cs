#region 작성정보
/*********************************************************************/
// 단위업무명 : 생산요약 정보조회
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-04-16
// 작성내용 : 생산요약 정보조회 관리
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
using System.Reflection;

namespace PC.PSB003
{
    public partial class PSB003 : UIForm.FPCOMM1
    {
        #region 생성자
        public PSB003()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load시
        private void PSB003_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            //조회조건 콤보박스
            SystemBase.ComboMake.C1Combo(cboPlantCd, "usp_B_COMMON @pTYPE = 'B031', @pSPEC1 = '" + SystemBase.Base.gstrBIZCD + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");//공장
            SystemBase.ComboMake.C1Combo(cboMpsKind, "usp_B_COMMON @pTYPE = 'COMM', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCODE = 'P014', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 3); //사업구분

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

            dtpDeliveryDtFr.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString().Substring(0,10);
            dtpDeliveryDtTo.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).ToShortDateString().Substring(0,10);
            cboPlantCd.SelectedValue = SystemBase.Base.gstrPLANT_CD;
        }
        #endregion
        
        #region NewExec
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            fpSpread1.Sheets[0].Rows.Count = 0;

            dtpDeliveryDtFr.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString().Substring(0,10);
            dtpDeliveryDtTo.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).ToShortDateString().Substring(0,10);
            cboPlantCd.SelectedValue = SystemBase.Base.gstrPLANT_CD;
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            try
            {
                string strQuery = " usp_PSB003  @pTYPE = 'S1'";
                strQuery += ", @pPLANT_CD = '" + Convert.ToString(cboPlantCd.SelectedValue) + "' ";
                strQuery += ", @pPROJECT_NO = '" + txtProjectNo.Text + "'";
                strQuery += ", @pENT_CD = '" + txtEntCd.Text + "'";
                strQuery += ", @pITEM_CD = '" + txtItemCd.Text + "'";
                strQuery += ", @pBUYER_CD = '" + txtBuyerCd.Text + "'";
                strQuery += ", @pMAKEORDER_NO_FR = '" + txtMakeorderNoFr.Text + "'";
                strQuery += ", @pMAKEORDER_NO_TO = '" + txtMakeorderNoTo.Text + "'";
                strQuery += ", @pPROJECT_SEQ_FR = '" + txtProjectSeqFr.Text + "'";
                strQuery += ", @pPROJECT_SEQ_TO = '" + txtProjectSeqTo.Text + "'";
                strQuery += ", @pMF_PLAN_USER = '" + txtMfPlanUser.Text + "'";
                strQuery += ", @pMPS_KIND = '" + Convert.ToString(cboMpsKind.SelectedValue) + "'";
                strQuery += ", @pDELV_DT_FR = '" + dtpDeliveryDtFr.Text + "'";
                strQuery += ", @pDELV_DT_TO = '" + dtpDeliveryDtTo.Text + "'";
                strQuery += ", @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0, true);

                if (fpSpread1.Sheets[0].Rows.Count > 0)
                {
                    double iqty = 0;
                    double istd_tm = 0;
                    double iwork_tm = 0;

                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count - 1; i++)
                    {
                        //납기일자가 현재보다 지연되었으면 빨간색
                        if (Convert.ToDateTime(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "생산납기")].Text) < Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")))
                        {
                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "생산납기")].ForeColor = Color.Red;
                        }
                        else
                        {
                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "생산납기")].ForeColor = Color.Black;
                        }

                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "오더량")].Text != "")
                        {
                            iqty = iqty + Convert.ToDouble(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "오더량")].Value.ToString());
                        }
                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "총부하시수")].Text != "")
                        {
                            istd_tm = istd_tm + Convert.ToDouble(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "총부하시수")].Value.ToString());
                        }
                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "총실적시수")].Text != "")
                        {
                            iwork_tm = iwork_tm + Convert.ToDouble(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "총실적시수")].Value.ToString());
                        }

                    }

                    fpSpread1.Sheets[0].RowCount = fpSpread1.Sheets[0].RowCount + 1;

                    fpSpread1.Sheets[0].FrozenTrailingRowCount = 1;	//하단 Column 1줄 고정

                    fpSpread1.Sheets[0].RowHeader.Cells[fpSpread1.Sheets[0].Rows.Count - 1, 0].Text = "합계";
                    fpSpread1.Sheets[0].Rows[fpSpread1.Sheets[0].Rows.Count - 1].BackColor = System.Drawing.Color.FromName("Beige");
                    fpSpread1.Sheets[0].Rows[fpSpread1.Sheets[0].Rows.Count - 1].Locked = true;

                    FarPoint.Win.ComplexBorder complexBorder1 = new FarPoint.Win.ComplexBorder(new FarPoint.Win.ComplexBorderSide(FarPoint.Win.ComplexBorderSideStyle.None), new FarPoint.Win.ComplexBorderSide(FarPoint.Win.ComplexBorderSideStyle.ThinLine, System.Drawing.Color.FromArgb(((System.Byte)(100)), ((System.Byte)(100)), ((System.Byte)(100)))), new FarPoint.Win.ComplexBorderSide(FarPoint.Win.ComplexBorderSideStyle.None), new FarPoint.Win.ComplexBorderSide(FarPoint.Win.ComplexBorderSideStyle.None));
                    fpSpread1.Sheets[0].Cells.Get(fpSpread1.Sheets[0].Rows.Count - 1, 0, fpSpread1.Sheets[0].Rows.Count - 1, fpSpread1.Sheets[0].Columns.Count - 1).Border = complexBorder1;

                    fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].Rows.Count - 1, SystemBase.Base.GridHeadIndex(GHIdx1, "오더량")].Value = iqty;
                    fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].Rows.Count - 1, SystemBase.Base.GridHeadIndex(GHIdx1, "총부하시수")].Value = istd_tm;
                    fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].Rows.Count - 1, SystemBase.Base.GridHeadIndex(GHIdx1, "총실적시수")].Value = iwork_tm;

                    if (istd_tm > 0 && iwork_tm > 0)
                    {
                        fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].Rows.Count - 1, SystemBase.Base.GridHeadIndex(GHIdx1, "진척율(%)")].Value = iwork_tm / istd_tm;
                    }
                    else
                    {
                        fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].Rows.Count - 1, SystemBase.Base.GridHeadIndex(GHIdx1, "진척율(%)")].Value = 0;
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
        #endregion

        #region 조회조건 팝업창
        //프로젝트번호
        private void btnProjectNo_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW003 pu = new WNDW003(txtProjectNo.Text, "S1", "C");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtEntCd.Value = Msgs[1].ToString();
                    txtEntNm.Value = Msgs[2].ToString();
                    txtProjectNo.Value = Msgs[3].ToString();
                    txtProjectNm.Value = Msgs[4].ToString();
                    txtItemCd.Value = Msgs[6].ToString();
                    txtItemNm.Value = Msgs[7].ToString();
                    txtBuyerCd.Value = Msgs[16].ToString();
                    txtBuyerNm.Value = Msgs[17].ToString();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "프로젝트 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //프로젝트차수 FROM
        private void btnProjectSeqFr_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON 'PROJ_SEQ', @pSPEC1 = '" + txtProjectNo.Text + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";										// 쿼리
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };			// 쿼리 인자값(조회조건)
                string[] strSearch = new string[] { txtProjectSeqFr.Text, "" };		// 쿼리 인자값에 들어갈 데이타

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P09001", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "프로젝트차수 조회", false);
                pu.Width = 400;
                pu.ShowDialog();	//공통 팝업 호출

                if (pu.DialogResult == DialogResult.OK)
                {
                    string MSG = pu.ReturnVal.Replace("|", "#");
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(MSG);
                    txtProjectSeqFr.Value = Msgs[0].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "프로젝트차수 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        //프로젝트차수 TO
        private void btnProjectSeqTo_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON 'PROJ_SEQ', @pSPEC1 = '" + txtProjectNo.Text + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";										// 쿼리
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };			// 쿼리 인자값(조회조건)
                string[] strSearch = new string[] { txtProjectSeqTo.Text, "" };		// 쿼리 인자값에 들어갈 데이타

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P09001", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "프로젝트차수 조회", false);
                pu.Width = 400;
                pu.ShowDialog();	//공통 팝업 호출

                if (pu.DialogResult == DialogResult.OK)
                {
                    string MSG = pu.ReturnVal.Replace("|", "#");
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(MSG);
                    txtProjectSeqTo.Value = Msgs[0].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "프로젝트차수 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        //제품코드
        private void btnItemCd_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW003 pu = new WNDW003("", txtItemCd.Text, "S1", "C");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtItemCd.Value = Msgs[6].ToString();
                    txtItemNm.Value = Msgs[7].ToString();
                    txtItemCd.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목코드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //사업코드
        private void btnEntCd_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON @pTYPE ='TABLE_POP', @pSPEC1 = 'ENT_CD', @pSPEC2 = 'ENT_NM', @pSPEC3 = 'S_ENTERPRISE_INFO', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtEntCd.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00007", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "사업 조회");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtEntCd.Value = Msgs[0].ToString();
                    txtEntNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "사업코드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //제품오더번호 FROM
        private void btnMakeorderNoFr_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW008 pu = new WNDW008(txtMakeorderNoFr.Text);
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtMakeorderNoFr.Value = Msgs[1].ToString();
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
                WNDW008 pu = new WNDW008(txtMakeorderNoTo.Text);
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtMakeorderNoTo.Value = Msgs[1].ToString();
                    txtMakeorderNoTo.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "제품오더번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //고객명
        private void btnBuyerCd_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW002 pu = new WNDW002(txtBuyerCd.Text, "");
                pu.MaximizeBox = false;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtBuyerCd.Value = Msgs[1].ToString();
                    txtBuyerNm.Value = Msgs[2].ToString();
                    txtBuyerCd.Focus();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "고객명 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        //생산담당자
        private void btnMfPlanUser_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON 'B010' ,@pSPEC1='" + SystemBase.Base.gstrBIZCD + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtMfPlanUser.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P04003", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "담당자 조회");	//생산관리 사용자조회
                pu.Width = 450;
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtMfPlanUser.Value = Msgs[0].ToString();
                    txtMfPlanUserNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "생산담당자 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 코드입력시 코드명 자동입력
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
                {
                    txtProjectSeqFr.Value = "";
                    txtProjectSeqTo.Value = "";
                    txtEntCd.Value = "";
                    txtItemCd.Value = "";
                    txtBuyerCd.Value = "";
                }
            }
            catch
            {

            }
        }

        //제품코드
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
            catch
            {

            }
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
            catch
            {

            }
        }

        //고객명
        private void txtBuyerCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtBuyerCd.Text != "")
                {
                    txtBuyerNm.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtBuyerCd.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");
                }
                else
                {
                    txtBuyerNm.Value = "";
                }
            }
            catch
            {

            }
        }

        //생산담당자
        private void txtMfPlanUser_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtMfPlanUser.Text != "")
                {
                    txtMfPlanUserNm.Value = SystemBase.Base.CodeName("USR_ID", "USR_NM", "B_SYS_USER", txtMfPlanUser.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");
                }
                else
                {
                    txtMfPlanUserNm.Value = "";
                }
            }
            catch
            {

            }
        }
        #endregion

        #region 그리드 더블클릭시 프로젝트별 납기일정검토 화면으로 이동
        private void fpSpread1_CellDoubleClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            try
            {
                for (int k = 0; k < this.MdiParent.MdiChildren.Length; k++)
                {	// 폼이 이미 열려있으면 닫기
                    if (MdiParent.MdiChildren[k].Name == "PSB001")
                    {
                        MdiParent.MdiChildren[k].BringToFront(); //화면을 앞으로 가져오고.. 
                        MdiParent.MdiChildren[k].Close();
                        break;
                    }
                }

                object[] param = new object[13];
                param[0] = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text;
                param[1] = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트명")].Text;
                param[2] = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Text;
                param[3] = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "사업명")].Text;
                param[4] = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "고객")].Text;
                param[5] = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "제품코드")].Text;
                param[6] = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "제품명")].Text;
                param[7] = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "제품오더번호")].Text;
                if (fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "오더량")].Text == "") { param[8] = "0"; }
                else { param[8] = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "오더량")].Value.ToString(); }
                param[9] = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "생산납기")].Text;
                if (fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "총부하시수")].Text == "") { param[10] = "0"; }
                else { param[10] = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "총부하시수")].Value.ToString(); }
                if (fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "총실적시수")].Text == "") { param[11] = "0"; }
                else { param[11] = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "총실적시수")].Value.ToString(); }
                if (fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "진척율(%)")].Text == "") { param[12] = "0"; }
                else { param[12] = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "진척율(%)")].Value.ToString(); }

                Assembly ServiceAssembly = Assembly.LoadFile(SystemBase.Base.ProgramWhere.ToString() + "\\PSB001.dll");
                Form myForm = (Form)System.Activator.CreateInstance(ServiceAssembly.GetType("PSB001.PSB001"), param);
                myForm.MdiParent = this.MdiParent;
                SystemBase.Base.RodeFormText = "생산진행현황";
                myForm.Show();
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0046", "생산진행현황"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);	// || 화면으로 이동 중 에러가 발생하였습니다.

            }
        }
        #endregion		
                
    }
}
