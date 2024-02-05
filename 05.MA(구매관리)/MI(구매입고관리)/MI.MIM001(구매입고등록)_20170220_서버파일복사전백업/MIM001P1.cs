#region 작성정보
/*********************************************************************/
// 단위업무명 : 개발일정등록
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-02-01
// 작성내용 : 개발일정등록 및 관리
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

namespace MI.MIM001
{ 
    public partial class MIM001P1 : UIForm.FPCOMM1
    {
        #region 변수선언
        string strBtn = "N";
        string[] returnVal = null;
        string rdochk = "1";
        FarPoint.Win.Spread.FpSpread spd;
        #endregion

        #region 생성자
        public MIM001P1()
        {
            InitializeComponent();           
        }

        public MIM001P1(FarPoint.Win.Spread.FpSpread spread)
        {
            InitializeComponent();
            spd = spread;
        }
        #endregion

        #region Form Load 시
        private void MIM001P1_Load(object sender, System.EventArgs e)
        {
            this.Text = "입고대상참조팝업";
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            
            UIForm.Buttons.ReButton("010000001000", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);

            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "단가구분")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'S011', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//단가구분
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "공장")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='B031', @pSPEC1 = '" + SystemBase.Base.gstrBIZCD.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//공장
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "입고창고")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='SL'  , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "창고위치")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='LOC'  , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

            //기타 세팅
            dtpPoDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString().Substring(0,10);
            dtpPoDtTo.Text = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0,10);

            rdoPo.Checked = true;
            rdoAll.Checked = true;

            Set_Tag(";2;;");
            
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

                    string strType = "";
                    if (rdochk == "1") { strType = "P11"; }
                    else if (rdochk == "2") { strType = "P12"; }
                    else if (rdochk == "3") { strType = "P13"; }

                    string strQuery = " usp_MIM001  @pTYPE = '" + strType + "' ";
                    strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' ";
                    strQuery += ", @pPO_DT_FR = '" + dtpPoDtFr.Text + "' ";
                    strQuery += ", @pPO_DT_TO = '" + dtpPoDtTo.Text + "' ";
                    strQuery += ", @pPO_NO = '" + txtPoNo.Text + "' ";
                    strQuery += ", @pCUST_CD = '" + txtCustCd.Text + "' ";
                    strQuery += ", @pPO_TYPE = '" + txtPoType.Text + "' ";
                    strQuery += ", @pPUR_DUTY = '" + txtPurDuty.Text + "' ";
                    strQuery += ", @pPROJECT_NO = '" + txtProjectNo.Text + "' ";
                    strQuery += ", @pITEM_CD = '" + txtItemCd.Text + "' ";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0);

                    //spread header 변경
                    if (rdochk == "1")
                    {
                        fpSpread1.Sheets[0].ColumnHeader.Cells[0, 3].Text = "발주번호";
                        fpSpread1.Sheets[0].ColumnHeader.Cells[0, 4].Text = "발주순번";
                        fpSpread1.Sheets[0].ColumnHeader.Cells[0, 9].Text = "발주수량";
                    }
                    else if (rdochk == "2")
                    {
                        fpSpread1.Sheets[0].ColumnHeader.Cells[0, 3].Text = "통관번호";
                        fpSpread1.Sheets[0].ColumnHeader.Cells[0, 4].Text = "통관순번";
                        fpSpread1.Sheets[0].ColumnHeader.Cells[0, 9].Text = "통관수량";
                    }
                    else
                    {
                        fpSpread1.Sheets[0].ColumnHeader.Cells[0, 3].Text = "매입번호";
                        fpSpread1.Sheets[0].ColumnHeader.Cells[0, 4].Text = "매입순번";
                        fpSpread1.Sheets[0].ColumnHeader.Cells[0, 9].Text = "매입수량";
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
							    spd.Sheets[0].Cells[j, 8].Value = "False";	// Release Check
							    spd.Sheets[0].Cells[j, 7].Value = "4";		// 검사구분 - 검사의뢰 대기
							    spd.Sheets[0].Cells[j, 12].Locked = false;	// Lot 분할/수정/삭제
							}
							else
							{
							    spd.Sheets[0].Cells[j, 8].Value = "True";
							    spd.Sheets[0].Cells[j, 7].Value = "1";		//검사의뢰
							    spd.Sheets[0].Cells[j, 12].Locked = true;
							}

						}
						else
						{
							spd.Sheets[0].Cells[j, 7].Value = "0";			//무검사
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

						spd.Sheets[0].Cells[j, 9].Value = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot 추적")].Value;
						spd.Sheets[0].Cells[j, 10].Value = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Serial 추적")].Value;
						spd.Sheets[0].Cells[j, 15].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].Text;
						spd.Sheets[0].Cells[j, 16].Value = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "미입고량")].Value;

						spd.Sheets[0].Cells[j, 19].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "입고창고")].Value.ToString();
						spd.Sheets[0].Cells[j, 20].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "입고창고")].Text;
						spd.Sheets[0].Cells[j, 21].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "창고위치")].Value.ToString();
						spd.Sheets[0].Cells[j, 22].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "창고위치")].Text;

						spd.Sheets[0].Cells[j, 27].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "구매담당자")].Text;
						spd.Sheets[0].Cells[j, 28].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "구매담당자명")].Text;

						spd.Sheets[0].Cells[j, 30].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "화폐단위")].Text;
						spd.Sheets[0].Cells[j, 31].Value = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "환율")].Value;

						spd.Sheets[0].Cells[j, 32].Value = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "단가")].Value;

						spd.Sheets[0].Cells[j, 33].Value = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "금액")].Value;
						spd.Sheets[0].Cells[j, 34].Value = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "자국금액")].Value;

						sum1 += Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "금액")].Value);
						sum2 += Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "자국금액")].Value);

						if (rdochk == "2")
						{
							spd.Sheets[0].Cells[j, 40].Text = fpSpread1.Sheets[0].Cells[i, 3].Text;   //통관번호
							spd.Sheets[0].Cells[j, 41].Text = fpSpread1.Sheets[0].Cells[i, 4].Text;   //통관순번
						}
						else if (rdochk == "3")
						{
							spd.Sheets[0].Cells[j, 42].Text = fpSpread1.Sheets[0].Cells[i, 3].Text;   //매입번호
							spd.Sheets[0].Cells[j, 43].Text = fpSpread1.Sheets[0].Cells[i, 4].Text;   //매입순번
						}
						spd.Sheets[0].Cells[j, 44].Value = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "단가구분")].Value;
						spd.Sheets[0].Cells[j, 38].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "발주번호_2")].Text;
						spd.Sheets[0].Cells[j, 39].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "발주순번_2")].Text;


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
            returnVal = new string[6];
            for (int i = 26; i < fpSpread1.Sheets[0].Columns.Count - 3; i++)
            {
                returnVal[i - 26] = fpSpread1.Sheets[0].Cells[Row, i + 1].Text.ToString();
            }
        }
        #endregion

        #region 버튼 Click  TextChanged
        private void btnItem_Click(object sender, EventArgs e)
        {
            try
            {
                WNDW.WNDW005 pu = new WNDW.WNDW005(SystemBase.Base.gstrPLANT_CD, "10", txtItemCd.Text );
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

        private void btnPoNo_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (rdoPo.Checked == true)
                {

                    MIM001P2 frm1 = new MIM001P2();
                    frm1.ShowDialog();
                    if (frm1.DialogResult == DialogResult.OK)
                    {
                        string Msgs = frm1.ReturnVal;
                        txtPoNo.Value = Msgs;
                    }
                }
                else if (rdoCc.Checked == true)
                {

                    MIM001P3 frm1 = new MIM001P3();
                    frm1.ShowDialog();
                    if (frm1.DialogResult == DialogResult.OK)
                    {
                        string Msgs = frm1.ReturnVal;
                        txtPoNo.Value = Msgs;
                    }
                }
                else if (rdoIv.Checked == true)
                {

                    MIM001P4 frm1 = new MIM001P4();
                    frm1.ShowDialog();
                    if (frm1.DialogResult == DialogResult.OK)
                    {
                        string Msgs = frm1.ReturnVal;
                        txtPoNo.Value = Msgs;
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(f.Message, SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

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

                    txtPurDuty.Text = Msgs[0].ToString();
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

                    txtCustCd.Text = Msgs[1].ToString();
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
                if (rdoPo.Checked == true)
                {
                    strQuery = " usp_M_COMMON 'M029'  , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                }
                else if (rdoCc.Checked == true)
                {
                    strQuery = " usp_M_COMMON 'M026'  , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                }
                else
                {
                    strQuery = " usp_M_COMMON 'M028' , @pSPEC1 = 'Y'  , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                }

                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtPoType.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P01005", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "발주형태 팝업");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtPoType.Text = Msgs[0].ToString();
                    txtPoTypeNm.Value = Msgs[1].ToString();

                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(f.Message, SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                    txtProjectNo.Text = Msgs[3].ToString();
                    //					txtProjectSeq.Text	= "";
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
            if (strBtn == "N" && txtPurDuty.Text.Trim() != "")
            {
                string temp = "";
                temp = SystemBase.Base.CodeName("PUR_DUTY", "PUR_DUTY", "M_PUR_DUTY", txtPurDuty.Text, " AND USE_YN = 'Y' AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "' ");
                if (temp != "")
                    txtPurDutyNm.Text = SystemBase.Base.CodeName("USR_ID", "USR_NM", "B_SYS_USER", txtPurDuty.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                else
                {
                    DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("M0001"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    //구매담당자가 아닙니다
                    txtPurDuty.Text = "";
                    txtPurDutyNm.Text = "";
                    txtPurDuty.Focus();
                }
            }
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
                        txtPurDuty.Text = "";
                        txtPurDutyNm.Value = "";
                        txtPurDuty.Focus();
                    }
                }                
            }
            catch
            {

            }
        }

        private void txtItemCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtItemCd.Text != "")
                {
                    txtItemNm.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtItemCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'");
                }
                else
                {
                    txtItemNm.Value = "";
                }
            }
            catch { }
        }

        #endregion

        #region fpSpread1_ButtonClicked
        private void fpSpread1_ButtonClicked(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            fpSpread1.Sheets[0].RowHeader.Cells[e.Row, 0].Text = "";
        }
        #endregion
        
        #region radio CheckedChanged
        private void rdoPo_CheckedChanged(object sender, System.EventArgs e)
        {
            if (rdoPo.Checked == true) Set_Titile("1");
        }

        private void rdoCc_CheckedChanged(object sender, System.EventArgs e)
        {
            if (rdoCc.Checked == true) Set_Titile("2");
        }

        private void rdoIv_CheckedChanged(object sender, System.EventArgs e)
        {
            if (rdoIv.Checked == true) Set_Titile("3");
        }

        private void Set_Titile(string strchk)
        {
            rdochk = strchk;
            if (rdochk == "1")
            {
                label2.Text = "발주번호";
                label4.Text = "발주일자";
            }
            else if (rdochk == "2")
            {
                label2.Text = "통관번호";
                label4.Text = "신고일자";
            }
            else
            {
                label2.Text = "매입번호";
                label4.Text = "매입일자";
            }
        }

        private void rdoAll_CheckedChanged(object sender, System.EventArgs e)
        {
            if (rdoAll.Checked == true)
            {
                Set_Tag("2");
            }
        }

        private void rdoNo_CheckedChanged(object sender, System.EventArgs e)
        {
            if (rdoNo.Checked == true)
            {
                Set_Tag("1");
            }
        }

        private void Set_Tag(string div)
        {

            if (div == "1")
            {
                txtPoNo.BackColor = SystemBase.Validation.Kind_LightCyan;
                txtPoNo.Tag = "발주번호;1;;";
                txtPoNo.Enabled = true;
                txtPoNo.ReadOnly = false;
                btnPoNo.Enabled = true;
				// TEST
				//txtPoNo.Value = "PO2014071800001";



                dtpPoDtFr.Value = "";
                dtpPoDtTo.Value = "";
                txtProjectNo.Value = "";
                txtPoType.Value = "";
                txtPoTypeNm.Value = "";
                txtCustCd.Value = "";
                txtCustNm.Value = "";
                txtPurDuty.Value = "";
                txtPurDutyNm.Value = "";

                dtpPoDtFr.BackColor = SystemBase.Validation.Kind_Gainsboro; 
                dtpPoDtFr.Tag = ";2;;";
                dtpPoDtFr.Enabled = false;
                dtpPoDtFr.ReadOnly = true;

                dtpPoDtTo.BackColor = SystemBase.Validation.Kind_Gainsboro;
                dtpPoDtTo.Tag = ";2;;";
                dtpPoDtTo.Enabled = false;
                dtpPoDtTo.ReadOnly = true;

                txtProjectNo.BackColor = SystemBase.Validation.Kind_Gainsboro;
                txtProjectNo.Tag = ";2;;";
                txtProjectNo.Enabled = false;
                txtProjectNo.ReadOnly = true;
                btnProj.Enabled = false;

                txtPoType.BackColor = SystemBase.Validation.Kind_Gainsboro;
                txtPoType.Tag = ";2;;";
                txtPoType.Enabled = false;
                txtPoType.ReadOnly = true;
                btnPoType.Enabled = false;

                txtCustCd.BackColor = SystemBase.Validation.Kind_White;
                txtCustCd.Tag = ";0;;";
                txtCustCd.Enabled = false;
                txtCustCd.ReadOnly = true;
                btnCust.Enabled = false;

                txtPurDuty.BackColor = SystemBase.Validation.Kind_Gainsboro;
                txtPurDuty.Tag = ";2;;";
                txtPurDuty.Enabled = false;
                txtPurDuty.ReadOnly = true;
                btnPurDuty.Enabled = false;

            }
            else
            {
                txtPoNo.BackColor = SystemBase.Validation.Kind_Gainsboro;
                txtPoNo.Tag = ";2;;";
                txtPoNo.Value = "";
                txtPoNo.Enabled = false;
                txtPoNo.ReadOnly = true;
                btnPoNo.Enabled = false;

                txtProjectNo.BackColor = SystemBase.Validation.Kind_White;
                txtProjectNo.Tag = "";
                txtProjectNo.Enabled = true;
                txtProjectNo.ReadOnly = false;
                btnProj.Enabled = true;

                txtPoType.BackColor = SystemBase.Validation.Kind_LightCyan;
                txtPoType.Tag = "발주형태;1;;";
                txtPoType.Enabled = true;
                txtPoType.ReadOnly = false;
                btnPoType.Enabled = true;

                txtCustCd.BackColor = SystemBase.Validation.Kind_White;
                txtCustCd.Tag = "거래처;;;";
                txtCustCd.Enabled = true;
                txtCustCd.ReadOnly = false;
                btnCust.Enabled = true;

                txtPurDuty.BackColor = SystemBase.Validation.Kind_White;
                txtPurDuty.Tag = "";
                txtPurDuty.Enabled = true;
                txtPurDuty.ReadOnly = false;
                btnPurDuty.Enabled = true;

                dtpPoDtFr.BackColor = SystemBase.Validation.Kind_LightCyan;
                dtpPoDtFr.Tag = "발주일자;1;;";
                dtpPoDtFr.Enabled = true;
                dtpPoDtFr.ReadOnly = false;

                dtpPoDtTo.BackColor = SystemBase.Validation.Kind_LightCyan;
                dtpPoDtTo.Tag = "발주일자;1;;";
                dtpPoDtTo.Enabled = true;
                dtpPoDtTo.ReadOnly = false;

                dtpPoDtFr.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString().Substring(0,10);
                dtpPoDtTo.Value = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0,10);
            }
        }
        #endregion
        
    }
}
