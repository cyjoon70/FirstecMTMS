#region 작성정보
/*********************************************************************/
// 단위업무명 : 구매입고등록
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-04-01
// 작성내용 : 구매입고등록 및 관리
// 수 정 일 : 2014-07-21
// 수 정 자 : 최 용 준
// 수정내용 : 품질증빙확인 기능 추가
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

namespace MI.MIM001
{ 
    public partial class MIM001P5 : UIForm.FPCOMM1
    {
        #region 변수선언
        string strBtn = "N";
        FarPoint.Win.Spread.FpSpread spd;
        string[] returnVal = null;
        string rdochk = "1";
        string scmNo = "";
        #endregion

        #region 생성자
        public MIM001P5()
        {
            InitializeComponent();           
        }

        public MIM001P5(FarPoint.Win.Spread.FpSpread spread)
        {
            InitializeComponent();
            spd = spread;
        }
        #endregion

        #region Form Load 시
        private void MIM001P5_Load(object sender, System.EventArgs e)
        {
            this.Text = "입고대상참조팝업";

            SystemBase.Validation.GroupBox_Setting(groupBox1);

            UIForm.Buttons.ReButton("010000001000", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);

            SystemBase.ComboMake.C1Combo(cboPlantCd, "usp_B_COMMON @pType='B031', @pSPEC1 = '" + SystemBase.Base.gstrBIZCD + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 9);//공장

            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "단가구분")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'S011', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//단가구분
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "공장")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='B031', @pSPEC1 = '" + SystemBase.Base.gstrBIZCD.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//공장
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "입고창고")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='SL'  , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "창고위치")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='LOC'  , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

            ////기타 세팅
            //dtpPoDtFr.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString().Substring(0,10);
            //dtpPoDtTo.Value = SystemBase.Base.ServerTime("YYMMDD").Substring(0, 10);

            rdoScmMvmtNo_Y.Checked = true;

            cboPlantCd.SelectedValue = SystemBase.Base.gstrPLANT_CD.ToString();
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
                    string strQuery = " usp_MIM001  @pTYPE = 'P5' ";
                    strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' ";
                    strQuery += ", @pPO_DT_FR = '" + dtpPoDtFr.Text + "' ";
                    strQuery += ", @pPO_DT_TO = '" + dtpPoDtTo.Text + "' ";
                    strQuery += ", @pPLANT_CD = '" + cboPlantCd.SelectedValue + "' ";
                    strQuery += ", @pCUST_CD = '" + txtCustCd.Text + "' ";
                    strQuery += ", @pPO_TYPE = '" + txtPoType.Text + "' ";
                    strQuery += ", @pPUR_DUTY = '" + txtPurDuty.Text + "' ";
                    strQuery += ", @pPROJECT_NO = '" + txtProjectNo.Text + "' ";
                    strQuery += ", @pSCM_MVMT_NO = '" + txtScmMvmtNo.Text + "'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    
                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 8);

					// 품질증빙문서 관련 칼럼 설정
					fpSpread1.ActiveSheet.Columns[33].Visible = false; // KEY_NO
					fpSpread1.ActiveSheet.Columns[34].Visible = false; // KEY_SEQ

					for (int i = 33; i <= fpSpread1.ActiveSheet.Columns.Count - 1; i++)
					{
						fpSpread1.ActiveSheet.Columns[i].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
						fpSpread1.ActiveSheet.Columns[i].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
						fpSpread1.ActiveSheet.Columns[i].Locked = true;
						fpSpread1.ActiveSheet.Columns[i].BackColor = System.Drawing.Color.FromArgb(238, 238, 238); // Color.LightGray
					}

                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }

            this.Cursor = System.Windows.Forms.Cursors.Default;
        }
        #endregion

        #region 버튼 Click
        private void btnOk_Click(object sender, System.EventArgs e)
        {
            int col_sel = SystemBase.Base.GridHeadIndex(GHIdx1, "선택");
            string strTop = "Y";
            bool check_is = false;
            decimal sum1 = 0;
            decimal sum2 = 0;

            try
            {
                int j = spd.Sheets[0].Rows.Count;
                for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                {
                    if (fpSpread1.Sheets[0].Cells[i, col_sel].Text == "True")
                    {
                        check_is = true;
                        scmNo = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "SCM입고번호")].Text;
                        if (strTop == "Y") RtnStr(i);
                        strTop = "N";

						spd.Sheets[0].ActiveRowIndex = spd.Sheets[0].RowCount;

						UIForm.FPMake.RowInsert(spd);
                        spd.Sheets[0].Rows.Count = j + 1;
                        spd.Sheets[0].RowHeader.Cells[j, 0].Text = "I";

                        spd.Sheets[0].Cells[j, 1].Value = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "공장")].Value;
                        spd.Sheets[0].Cells[j, 2].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text;
                        spd.Sheets[0].Cells[j, 3].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품명")].Text;
                        spd.Sheets[0].Cells[j, 4].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "규격")].Text;

                        spd.Sheets[0].Cells[j, 5].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text;
                        spd.Sheets[0].Cells[j, 6].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Text;

						if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사구분")].Text == "Y")
						{

							// Lot 추적 대상이면 Release = False (바코드 처리 완료와 상관없이 자동으로 검사의뢰)
							if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot 추적")].Text == "True")
							{
							    spd.Sheets[0].Cells[j, 8].Value = "False";
							    spd.Sheets[0].Cells[j, 7].Value = "4";	//검사의뢰 대기
							    spd.Sheets[0].Cells[j, 12].Locked = false;
							}
							else
							{
							    spd.Sheets[0].Cells[j, 8].Value = "True";
							    spd.Sheets[0].Cells[j, 7].Value = "1";	//검사의뢰
							    spd.Sheets[0].Cells[j, 12].Locked = true;
							}

						}
						else
						{
							spd.Sheets[0].Cells[j, 7].Value = "0";	//무검사
							spd.Sheets[0].Cells[j, 8].Value = "True";

							if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot 추적")].Text == "True")
							{
								spd.Sheets[0].Cells[j, 12].Locked = false;
							}
							else
							{
								spd.Sheets[0].Cells[j, 12].Locked = true;
							}
						}

						spd.Sheets[0].Cells[j, 9].Value = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot 추적")].Value; // Lot 추적
						spd.Sheets[0].Cells[j, 10].Value = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Serial 추적")].Value; // Lot 추적

                        spd.Sheets[0].Cells[j, 15].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].Text;
                        spd.Sheets[0].Cells[j, 16].Value = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "미입고량")].Value;

                        spd.Sheets[0].Cells[j, 19].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "입고창고")].Value.ToString();
                        spd.Sheets[0].Cells[j, 20].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "입고창고")].Text;
                        spd.Sheets[0].Cells[j, 22].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "창고위치")].Value.ToString();
                        spd.Sheets[0].Cells[j, 23].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "창고위치")].Text;

                        spd.Sheets[0].Cells[j, 27].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "구매담당자")].Text;
                        spd.Sheets[0].Cells[j, 28].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "구매담당자명")].Text;

                        spd.Sheets[0].Cells[j, 30].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "화폐단위")].Text;
                        spd.Sheets[0].Cells[j, 31].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "환율")].Text;

                        spd.Sheets[0].Cells[j, 32].Value = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "단가")].Value;

                        spd.Sheets[0].Cells[j, 33].Value = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "금액")].Value;
                        spd.Sheets[0].Cells[j, 34].Value = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "자국금액")].Value;

                        sum1 += Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "금액")].Value);
                        sum2 += Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "자국금액")].Value);

                        spd.Sheets[0].Cells[j, 44].Value = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "단가구분")].Value;
                        spd.Sheets[0].Cells[j, 45].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "SCM입고번호")].Text;
                        spd.Sheets[0].Cells[j, 38].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "발주번호")].Text;
                        spd.Sheets[0].Cells[j, 39].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "발주순번")].Text;
                        j++;
                    }
                }
                if (check_is)
                {
                    returnVal[4] = Convert.ToInt64(sum1).ToString();
                    returnVal[5] = Convert.ToInt64(sum2).ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(f.Message, SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            Close();
            this.DialogResult = DialogResult.OK;
        }

        private void butCancel_Click(object sender, System.EventArgs e)
        {
            Close();
            this.DialogResult = DialogResult.Cancel;
        }
        #endregion

        #region 값 전송
        public string[] ReturnVal { get { return returnVal; } set { returnVal = value; } }

        public void RtnStr(int Row)
        {
            returnVal = new string[8];
            for (int i = 27; i < fpSpread1.Sheets[0].Columns.Count - 1; i++)
            {
				if (i - 27 <= 7)
				{
					returnVal[i - 27] = fpSpread1.Sheets[0].Cells[Row, i].Text.ToString();
				}
            }
            returnVal[6] = scmNo;
            returnVal[7] = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "SCM출고일자")].Text.ToString();
        }
        #endregion

        #region 버튼 Click  TextChanged
        private void btnPurDuty_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                string strQuery = " usp_M_COMMON 'M011' ,@pSPEC1='" + SystemBase.Base.gstrBIZCD + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtPurDuty.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P01008", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "사용자 팝업");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtPurDuty.Value = Msgs[0].ToString();
                    txtPurDutyNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
            strBtn = "N";
        }

        private void btnCust_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                WNDW002 pu = new WNDW002(txtCustCd.Text, "P");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtCustCd.Value = Msgs[1].ToString();
                    txtCustNm.Value = Msgs[2].ToString();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
            strBtn = "N";
        }

        private void btnPoType_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                string strQuery;

                strQuery = " usp_M_COMMON 'M024'  , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtPoType.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P01005", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "발주형태 팝업");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtPoType.Value = Msgs[0].ToString();
                    txtPoTypeNm.Value = Msgs[1].ToString();

                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                SystemBase.MessageBoxComm.Show(f.ToString());
            }
            strBtn = "N";
        }

        private void btnProj_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                WNDW007 pu = new WNDW007(txtProjectNo.Text, "N");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;
                    txtProjectNo.Value = Msgs[3].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
            strBtn = "N";
        }
        //SCM번호
        private void btnScmMvmtNo_Click(object sender, System.EventArgs e)
        {
            try
            {
                MIM001P6 frm1 = new MIM001P6();
                frm1.ShowDialog();
                if (frm1.DialogResult == DialogResult.OK)
                {
                    string Msgs = frm1.ReturnVal;
                    txtScmMvmtNo.Value = Msgs;
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(f.Message, SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }	
        }
        private void txtPoType_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (strBtn == "N")
                {
                    if (txtPoType.Text != "")
                    {
                        txtPoTypeNm.Value = SystemBase.Base.CodeName("PO_TYPE_CD", "PO_TYPE_NM", "M_PO_TYPE", txtPoType.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                    }
                    else
                    {
                        txtPoTypeNm.Value = "";
                    }
                }
            }
            catch
            {

            }
        }

        private void txtCustCd_TextChanged(object sender, System.EventArgs e)
        {
            if (strBtn == "N")
                txtCustNm.Text = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtCustCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
            try
            {
                if (strBtn == "N")
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
            }
            catch
            {

            }
        }



        private void txtPurDuty_Leave(object sender, System.EventArgs e)
        {
            try
            {
                if (strBtn == "N" && txtPurDuty.Text.Trim() != "")
                {
                    string temp = "";
                    temp = SystemBase.Base.CodeName("PUR_DUTY", "PUR_DUTY", "M_PUR_DUTY", txtPurDuty.Text, " AND USE_YN = 'Y' AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "' ");
                    if (temp != "")
                    {
                        if (txtPurDuty.Text != "")
                        {
                            txtPurDutyNm.Value = SystemBase.Base.CodeName("USR_ID", "USR_NM", "B_SYS_USER", txtPurDuty.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                        }
                        else
                        {
                            txtPurDutyNm.Value = "";
                        }
                    }
                    else
                    {
                        DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("M0001"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        //구매담당자가 아닙니다
                        txtPurDuty.Value = "";
                        txtPurDutyNm.Value = "";
                        txtPurDuty.Focus();
                    }
                }
            }
            catch
            {

            }
        }

        #endregion

        #region fpSpread1_ButtonClicked
        private void fpSpread1_ButtonClicked(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            fpSpread1.Sheets[0].RowHeader.Cells[e.Row, 0].Text = "";
        }
        #endregion

        #region radio CheckedChanged
        private void rdoScmMvmtNo_Y_CheckedChanged(object sender, System.EventArgs e)
        {
            if (rdoScmMvmtNo_Y.Checked == true) Set_Tag("1");
        }

        private void rdoScmMvmtNo_N_CheckedChanged(object sender, System.EventArgs e)
        {
            if (rdoScmMvmtNo_N.Checked == true) Set_Tag("2");
        }


        private void Set_Tag(string div)
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            if (div == "1")
            {
                txtScmMvmtNo.Tag = "SCM번호;1;;";
                btnScmMvmtNo.Tag = "";

                dtpPoDtFr.Tag = ";2;;";
                dtpPoDtTo.Tag = ";2;;";
                txtPoType.Tag = ";2;;";
                btnPoType.Tag = ";2;;";
                txtCustCd.Tag = ";2;;";
                btnCust.Tag = ";2;;";
                cboPlantCd.Tag = ";2;;";

            }
            else
            {
                txtScmMvmtNo.Tag = ";2;;";
                btnScmMvmtNo.Tag = ";2;;";

                dtpPoDtFr.Tag = "SCM입고일자;1;;";
                dtpPoDtTo.Tag = "SCM입고일자;1;;";
                txtPoType.Tag = "발주형태;1;;";
                btnPoType.Tag = "발주형태;1;;";
                txtCustCd.Tag = "거래처;1;;";
                btnCust.Tag = "거래처;1;;";
                cboPlantCd.Tag = "공장;1;;";

                dtpPoDtFr.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString().Substring(0,10);
                dtpPoDtTo.Value = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0,10);
            }

            SystemBase.Validation.GroupBox_Setting(groupBox1); //필수체크

        }
        #endregion	
        
    }
}
