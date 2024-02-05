
#region 작성정보
/*********************************************************************/
// 단위업무명 : 월별납품계획
// 작 성 자 : 권순철
// 작 성 일 : 2013-04-04
// 작성내용 : 월별납품계획 및 관리
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

namespace SO.SOB007
{
    public partial class SOB007 : UIForm.FPCOMM1
    {
        #region 생성자
        public SOB007()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void SOB007_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            SystemBase.ComboMake.C1Combo(cboContractType, "usp_B_COMMON @pType='COMM', @pCODE = 'S014', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ", 3);//계약구분

            //그리드 초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

            //기타 세팅	
            dtpDeliveryDtFr.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-2).ToString().Substring(0, 7);
            dtpDeliveryDtTo.Value = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0, 7);

            dtpContractDtFr.Text = null;
            dtpContractDtTo.Text = null;

            txtBizCd.Text = SystemBase.Base.gstrBIZCD;
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            //그리드 초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

            //기타 세팅	
            dtpDeliveryDtFr.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-2).ToString().Substring(0,7);
            dtpDeliveryDtTo.Value = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0,7);

            dtpContractDtFr.Text = null;
            dtpContractDtTo.Text = null;

            txtBizCd.Text = SystemBase.Base.gstrBIZCD;
        }
        #endregion

        #region SearchExec()
        protected override void SearchExec()
        {
            //조회조건 필수 체크
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1) == true)
            {
                this.Cursor = Cursors.WaitCursor;

                if (Convert.ToDateTime(dtpDeliveryDtFr.Value) <= Convert.ToDateTime(dtpDeliveryDtTo.Value)) // 납기일From 이 To 보다 크면 조회내용이 없다.
                {
                    try
                    {
                        string strQuery = "usp_SOB007 @pTYPE = 'S1'";
                        strQuery += ", @pBIZ_CD = '" + txtBizCd.Text + "'";
                        strQuery += ", @pPROJECT_NO = '" + txtProjectNo.Text + "'";
                        strQuery += ", @pPROJECT_SEQ = '" + txtProjectSeq.Text + "'";
                        strQuery += ", @pDELIVERY_DT_FR = '" + dtpDeliveryDtFr.Text + "'";
                        strQuery += ", @pDELIVERY_DT_TO = '" + dtpDeliveryDtTo.Text + "'";
                        strQuery += ", @pSOLD_CUST_FR = '" + txtSoldCustFr.Text + "'";
                        strQuery += ", @pSOLD_CUST_TO = '" + txtSoldCustTo.Text + "'";
                        strQuery += ", @pCONTRACT_DT_FR = '" + dtpContractDtFr.Text + "'";
                        strQuery += ", @pCONTRACT_DT_TO = '" + dtpContractDtTo.Text + "'";
                        strQuery += ", @pSO_NO = '" + txtSoNo.Text + "'";
                        strQuery += ", @pCONTRACT_TYPE = '" + cboContractType.SelectedValue.ToString() + "'";
                        strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                        strQuery += ", @pREF_DELV_DT_FR = '" + dtpRefDelvDtFr.Text + "' ";      // 2017.11.01. hma 추가: 납기일(참조) FROM
                        strQuery += ", @pREF_DELV_DT_TO = '" + dtpRefDelvDtTo.Text + "' ";      // 2017.11.01. hma 추가: 납기일(참조) TO

                        DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

                        UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0, true);

                        if (fpSpread1.Sheets[0].RowCount > 0)
                        {
                            string strDeliveryDt = dtpDeliveryDtFr.Text;
                            int i = 0;
                            string strgrdRemake = "11|4";
                            string strgrdRemakeCT = "";

                            fpSpread1.Sheets[0].Columns.Count = dt.Columns.Count;

                            //그리드헤드명 바꾸기					
                            for (i = 10; i < dt.Columns.Count - 2; i++)
                            {
                                fpSpread1.Sheets[0].ColumnHeader.Cells[0, i].Text = strDeliveryDt.Substring(0, 7).Replace("-", "년") + "월 금액";
                                strDeliveryDt = Convert.ToDateTime(dtpDeliveryDtFr.Value).AddMonths(i - 9).ToString();

                                strgrdRemake += "#" + i + "|3";
                                strgrdRemakeCT += i + "|NM2#";

                                fpSpread1.Sheets[0].Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;
                                fpSpread1.Sheets[0].Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
                            }

                            fpSpread1.Sheets[0].ColumnHeader.Cells[0, i - 1].Text = "비고";
                            fpSpread1.Sheets[0].ColumnHeader.Cells[0, i].Text = "계약구분";
                            fpSpread1.Sheets[0].ColumnHeader.Cells[0, i + 1].Text = "정산구분";

                            strgrdRemakeCT = strgrdRemakeCT + (i - 1) + "|ZZ" + i + "|ZZ" + (i + 1) + "IZZ";
                            strgrdRemake += "#" + (i - 1) + "|3" + "#" + (i) + "|3" + "#" + (i + 1) + "|3";

                            fpSpread1.Sheets[0].Columns[i - 1, i + 1].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
                            fpSpread1.Sheets[0].Columns[i - 1, i + 1].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
                            UIForm.FPMake.grdReMake(fpSpread1, strgrdRemake);
                            UIForm.FPMake.grdReMakeCT(fpSpread1, strgrdRemakeCT);

                            Set_Section();
                        }
                    }
                    catch (Exception f)
                    {
                        SystemBase.Loggers.Log(this.Name, f.ToString());
                        MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회중 오류가 발생하였습니다.
                    }
                }
                else
                {
                    //그리드 초기화
                    UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0011"), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                this.Cursor = Cursors.Default;
            }
        }
        #endregion

        #region 조회조건 팝업
        //프로젝트번호
        private void btnProjectNo_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW.WNDW007 pu = new WNDW.WNDW007(txtProjectNo.Text);
                pu.MaximizeBox = false;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtProjectNo.Text = Msgs[3].ToString();
                    txtProjectNm.Value = Msgs[4].ToString();
                    txtProjectSeq.Text = "";
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "프로젝트번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //프로젝트차수
        private void btnProjectSeq_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON 'PROJ_SEQ', @pSPEC1 = '" + txtProjectNo.Text + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";										// 쿼리
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

        //사업장
        private void btnBizCd_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = "usp_B_COMMON @pTYPE = 'TABLE_POP' ,@pSPEC1 = 'BIZ_CD', @pSPEC2 = 'BIZ_NM', @pSPEC3 = 'B_BIZ_PLACE', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtBizCd.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00086", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "사업장 조회");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtBizCd.Value = Msgs[0].ToString();
                    txtBizNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "사업장 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);	//데이터 조회 중 오류가 발생하였습니다.

            }
        }

        //수주번호
        private void btnSoNo_Click(object sender, EventArgs e)
        {
            try
            {
                WNDW012 pu = new WNDW012();
                pu.MaximizeBox = false;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtSoNo.Text = Msgs[1].ToString();
                    txtSoNo.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "수주정보 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //거래처 From
        private void btnSoldCustFr_Click(object sender, EventArgs e)
        {
            try
            {
                WNDW002 pu = new WNDW002(txtSoldCustFr.Text, "");
                pu.MaximizeBox = false;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtSoldCustFr.Text = Msgs[1].ToString();
                    txtSoldCustNmFr.Value = Msgs[2].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "거래처 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);	//데이터 조회 중 오류가 발생하였습니다.

            }
        }

        //거래처 To
        private void btnSoldCustTo_Click(object sender, EventArgs e)
        {
            try
            {
                WNDW002 pu = new WNDW002(txtSoldCustTo.Text, "");
                pu.MaximizeBox = false;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtSoldCustTo.Text = Msgs[1].ToString();
                    txtSoldCustNmTo.Value = Msgs[2].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "거래처 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);	//데이터 조회 중 오류가 발생하였습니다.

            }
        }
        #endregion

        #region TextChanged
        //프로젝트번호
        private void txtProjectNo_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtProjectNo.Text != "")
                {
                    txtProjectNm.Value = SystemBase.Base.CodeName("PROJECT_NO", "PROJECT_NM", "S_SO_MASTER", txtProjectNo.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtProjectNm.Value = "";
                    txtProjectSeq.Text = "";
                }
            }
            catch { }
        }

        //사업장
        private void txtBizCd_TextChanged(object sender, EventArgs e)
        {
            txtBizNm.Value = SystemBase.Base.CodeName("BIZ_CD", "BIZ_NM", "B_BIZ_PLACE", txtBizCd.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
        }

        //거래처 From
        private void txtSoldCustFr_TextChanged(object sender, EventArgs e)
        {
            txtSoldCustNmFr.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtSoldCustFr.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
        }
        //거래처 To
        private void txtSoldCustTo_TextChanged(object sender, EventArgs e)
        {
            txtSoldCustNmTo.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtSoldCustTo.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
        }
        #endregion

        #region 그리드 재정의
        private void Set_Section()
        {
            int iCnt = fpSpread1.Sheets[0].RowCount;
            int iRow1 = 0;

            //소계, 합계 컬럼 합치고 색 변경
            for (int i = 0; i < iCnt; i++)
            {

                if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text == "")
                {
                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "거래처")].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "거래처명")].Text;
                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "거래처")].ColumnSpan = 7;
                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "거래처")].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;

                    for (int j = 1; j < fpSpread1.Sheets[0].ColumnCount; j++)
                    {
                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "거래처명")].Text == "합계")
                            fpSpread1.Sheets[0].Cells[i, j].BackColor = SystemBase.Base.gColor1;
                        else
                            fpSpread1.Sheets[0].Cells[i, j].BackColor = SystemBase.Base.gColor2;

                    }
                    fpSpread1.Sheets[0].Cells[iRow1, SystemBase.Base.GridHeadIndex(GHIdx1, "거래처")].RowSpan = i - iRow1;
                    fpSpread1.Sheets[0].Cells[iRow1, SystemBase.Base.GridHeadIndex(GHIdx1, "거래처명")].RowSpan = i - iRow1;

                    iRow1 = i + 1;
                }
                else
                {
                    for (int j = 1; j < fpSpread1.Sheets[0].ColumnCount; j++)
                    {
                        if (i % 2 == 0)
                        {
                            fpSpread1.Sheets[0].Cells[i, j].BackColor = Color.FromArgb(230, 230, 230);
                        }
                        else
                        {
                            fpSpread1.Sheets[0].Cells[i, j].BackColor = Color.FromArgb(245, 245, 245);
                        }
                    }
                }
            }

        }
        #endregion

        #region 셀  이벤트
        private void fpSpread1_CellDoubleClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            string proj_no = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text;
            string proj_seq = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Text;
            string ent_nm = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "사업명")].Text;
            if (proj_no != "" && proj_seq != "")
            {
                SOB007P2 form = new SOB007P2(proj_no, proj_seq, ent_nm);
                form.ShowDialog();
            }

        }
        #endregion

    }
}
