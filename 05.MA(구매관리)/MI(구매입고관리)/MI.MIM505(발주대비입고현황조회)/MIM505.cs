#region 작성정보
/*********************************************************************/
// 단위업무명 : 공급처별입고현황
// 작 성 자 : 김현근
// 작 성 일 : 2013-04-17
// 작성내용 : 공급처별입고현황
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

namespace MI.MIM505
{
    public partial class MIM505 : UIForm.FPCOMM1
    {
        #region 변수 정의
        bool form_act_chk = false;
        public string[] G2Head1 = null;// 첫번째 Head Text
        public string[] G2Head2 = null;// 두번째 Head Text
        public string[] G2Head3 = null;// 세번째 Head Text
        public int[] G2Width = null;// Cell 넓이
        public string[] G2Align = null;// Cell 데이타 정렬방식
        public string[] G2Type = null;// CellType 지정
        public int[] G2Color = null;// Cell 색상 및 ReadOnly 설정(0:일반, 1:필수, 2:ReadOnly)
        public string[] G2Etc = null;// Mask 양식 등
        public int G2HeadCnt = 0;   // Head 수
        public int[] G2SEQ = null;// 키
        #endregion

        public MIM505()
        {
            InitializeComponent();
        }

        #region Form Load시
        private void MIM505_Load(object sender, System.EventArgs e)
        {
            try
            {
                if (SystemBase.Base.ProgramWhere.Length > 0)
                {
                    /********************2번째 그리드(fpSpread2) 디자인 시작******************/
                    string Query2 = " usp_BAA004 'S3',@PFORM_ID='" + this.Name.ToString() + "', @PGRID_NAME='fpSpread2', @PIN_ID='" + SystemBase.Base.gstrUserID + "' ";
                    DataTable dt2 = SystemBase.DbOpen.TranDataTable(Query2);
                    int G2RowCount = dt2.Rows.Count + 1;

                    if (G2RowCount > 1)
                    {
                        G2Head1 = new string[G2RowCount];// 첫번째 Head Text
                        G2Head2 = new string[G2RowCount];// 두번째 Head Text
                        G2Head3 = new string[G2RowCount];// 세번째 Head Text
                        G2Width = new int[G2RowCount];// Cell 넓이
                        G2Align = new string[G2RowCount];// Cell 데이타 정렬방식
                        G2Type = new string[G2RowCount];// CellType 지정
                        G2Color = new int[G2RowCount];// Cell 색상 및 ReadOnly 설정(0:일반, 1:필수, 2:ReadOnly)
                        G2Etc = new string[G2RowCount];
                        G2HeadCnt = Convert.ToInt32(dt2.Rows[0][0].ToString());
                        G2SEQ = new int[G2RowCount];// 키

                        /********************1번째 숨김필드 정의******************/
                        G2Head1[0] = "";
                        if (Convert.ToInt32(dt2.Rows[0][0].ToString()) >= 1)
                            G2Head2[0] = "";
                        if (Convert.ToInt32(dt2.Rows[0][0].ToString()) >= 2)
                            G2Head3[0] = "";
                        G2Width[0] = 0;
                        G2Align[0] = "";
                        G2Type[0] = "";
                        G2Color[0] = 0;
                        G2Etc[0] = "";
                        /********************1번째 숨김필드 정의******************/

                        //####################그리드 Head 순번######################
                        GHIdx2 = new string[G2RowCount - 1, 2];	// 그리드 Head Index 변수 길이
                        //string OldHeadName2 = null;
                        int OldHeadNameCount2 = 1;
                        //####################그리드 Head 순번######################
                        for (int i = 1; i < G2RowCount; i++)
                        {
                            G2Head1[i] = dt2.Rows[i - 1][1].ToString();
                            if (Convert.ToInt32(dt2.Rows[i - 1][0].ToString()) >= 1)
                                G2Head2[i] = dt2.Rows[i - 1][2].ToString();
                            if (Convert.ToInt32(dt2.Rows[i - 1][0].ToString()) >= 2)
                                G2Head3[i] = dt2.Rows[i - 1][3].ToString();

                            G2Width[i] = Convert.ToInt32(dt2.Rows[i - 1][4].ToString());
                            G2Align[i] = dt2.Rows[i - 1][5].ToString();
                            G2Type[i] = dt2.Rows[i - 1][6].ToString();
                            G2Color[i] = Convert.ToInt32(dt2.Rows[i - 1][7].ToString());
                            G2Etc[i] = dt2.Rows[i - 1][8].ToString();

                            G2SEQ[i] = Convert.ToInt32(dt2.Rows[i - 1][9].ToString());


                            //####################그리드 Head 순번######################
                            OldHeadNameCount2 = 1;
                            GHIdx2[0, 0] = dt2.Rows[0][1].ToString().ToUpper();
                            for (int k = 0; k < i - 1; k++)
                            {
                                if (dt2.Rows[i - 1][1].ToString().ToUpper() == GHIdx2[k, 0].ToUpper())
                                {
                                    OldHeadNameCount2++;
                                }
                                else if (GHIdx2[k, 0].ToUpper().LastIndexOf("_") > 0 && dt2.Rows[i - 1][1].ToString().ToUpper() == GHIdx2[k, 0].ToUpper().Substring(0, GHIdx2[k, 0].ToUpper().LastIndexOf("_")))
                                {
                                    OldHeadNameCount2++;
                                }

                            }

                            if (OldHeadNameCount2 > 1)
                            {
                                GHIdx2[i - 1, 0] = dt2.Rows[i - 1][1].ToString().ToUpper() + "_" + OldHeadNameCount2.ToString();	// 그리드 Head명
                            }
                            else
                            {
                                GHIdx2[i - 1, 0] = dt2.Rows[i - 1][1].ToString().ToUpper();	// 그리드 Head명
                            }
                            GHIdx2[i - 1, 1] = Convert.ToString(i);			// 그리드 Head 위치
                            //####################그리드 Head 순번######################
                        }
                        UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, 0, 0, false);
                    }

                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "화면 생성"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            SystemBase.Validation.GroupBox_Setting(groupBox1);
            SystemBase.ComboMake.C1Combo(cboPlantCd, "usp_B_COMMON @pType='B031', @pSPEC1 = '" + SystemBase.Base.gstrBIZCD + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ", 0);//공장
            SystemBase.ComboMake.C1Combo(cboPoType, "usp_M_COMMON @pTYPE = 'M024', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9);
            //기타 세팅
            dtpPoDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString();
            dtpPoDtTo.Text = SystemBase.Base.ServerTime("YYMMDD");
            rdoAll.Checked = true;
        }
        #endregion

        #region NewExec()
        protected override void NewExec()
        {
            //조회조건 초기화
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            fpSpread1.Sheets[0].Rows.Count = 0;

            //기타 세팅
            dtpPoDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString();
            dtpPoDtTo.Text = SystemBase.Base.ServerTime("YYMMDD");
            rdoAll.Checked = true;
        }
        #endregion

        #region 조회조건 팝업  
        private void btnCust_Click(object sender, EventArgs e)
        {
            try
            {
                WNDW002 pu = new WNDW002(txtCust.Text, "P");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtCust.Text = Msgs[1].ToString();
                    txtCustNm.Value = Msgs[2].ToString();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        private void btnUser_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = " usp_M_COMMON 'M011' ,@pSPEC1='" + SystemBase.Base.gstrBIZCD + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtUserId.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00031", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "사용자 팝업");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {

                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtUserId.Text = Msgs[0].ToString();
                    txtUserNm.Value = Msgs[1].ToString();
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

        #region 텍스트박스 코드 입력시 코드명 자동입력
        private void txtCust_TextChanged(object sender, EventArgs e)
        {
            txtCustNm.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtCust.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
        }
        private void txtUserId_TextChanged(object sender, EventArgs e)
        {
            txtUserNm.Value = SystemBase.Base.CodeName("USR_ID", "USR_NM", "B_SYS_USER", txtUserId.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
        }

        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            //조회조건 필수 체크
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                this.Cursor = Cursors.WaitCursor;
                
                try
                {
                    string strQuery = " usp_MIM505 ";

                    if (rdoAll.Checked == true) strQuery += " 'S1'";
                    else strQuery += " 'S2'";

                    strQuery += ", @pLANG_CD ='" + SystemBase.Base.gstrLangCd + "'";
                    strQuery += ", @pCUST_CD ='" + txtCust.Text.Trim() + "'";
                    strQuery += ", @pPLANT_CD ='" + cboPlantCd.SelectedValue.ToString() + "'";
                    strQuery += ", @pPO_DT_FR ='" + dtpPoDtFr.Text + "'";
                    strQuery += ", @pPO_DT_TO ='" + dtpPoDtTo.Text + "'";
                    strQuery += ", @pPO_TYPE ='" + cboPoType.SelectedValue + "'";
                    strQuery += ", @pPUR_DUTY  ='" + txtUserId.Text.Trim() + "'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    if (rdoAll.Checked == true)
                    {
                        UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0, true);
                        fpSpread1.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.Normal;
                        if (fpSpread1.Sheets[0].RowCount > 0) Set_Color(fpSpread1);
                    }
                    else
                    {
                        UIForm.FPMake.grdCommSheet(fpSpread2, strQuery, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, true, 0, 0, true);
                        fpSpread2.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.Normal;
                        if (fpSpread2.Sheets[0].RowCount > 0) Set_Color(fpSpread2);
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회중 오류가 발생하였습니다.
                }

                this.Cursor = Cursors.Default;
            }
        }
        private void Set_Color(FarPoint.Win.Spread.FpSpread spd)
        {
            for (int i = 0; i < spd.Sheets[0].RowCount; i++)
            {
                if (spd.Sheets[0].Cells[i, 6].Text == "zzzzzzzz") //소계
                {
                    for (int j = 0; j < spd.Sheets[0].ColumnCount; j++)
                    {
                        spd.Sheets[0].Cells[i, j].BackColor = SystemBase.Base.gColor2;
                    }
                }
                else if (spd.Sheets[0].Cells[i, 4].Text == "zzzzzzzz") //합계
                {
                    for (int k = 0; k < spd.Sheets[0].ColumnCount; k++)
                    {
                        spd.Sheets[0].Cells[i, k].BackColor = SystemBase.Base.gColor1;
                    }
                }
            }
        }
        #endregion

        private void MIM505_Activated(object sender, System.EventArgs e)
        {
            if (form_act_chk == false) cboPlantCd.Focus();
        }

        private void MIM505_Deactivate(object sender, System.EventArgs e)
        {
            form_act_chk = true;
        }

        private void rdoAll_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoAll.Checked == true)
            {
                fpSpread1.Visible = true;
                fpSpread2.Visible = false;
            }
            else
            {
                fpSpread1.Visible = false;
                fpSpread2.Visible = true;
            }
        }

        private void rdoNotIn_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoNotIn.Checked == true)
            {
                fpSpread1.Visible = false;
                fpSpread2.Visible = true;
            }
            else
            {
                fpSpread1.Visible = true;
                fpSpread2.Visible = false;
            }
        }
       
    }
}
