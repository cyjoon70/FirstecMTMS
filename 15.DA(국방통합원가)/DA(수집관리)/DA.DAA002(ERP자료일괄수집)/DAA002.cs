#region DAA002 작성 정보
/*************************************************************/
// 단위업무명 : ERP자료 일괄생성
// 작 성 자 :   유재규
// 작 성 일 :   2013-06-19
// 작성내용 :   
// 수 정 일 :
// 수 정 자 :
// 수정내용 :
// 비    고 :  각 생성정보를 구분자를 가지고 처리한다. (구분자 길이 VARCHAR(4) )
// 참    고 : 
/*************************************************************/
#endregion
//using System;
//using System.Data;
//using System.Data.SqlClient;
//using System.Drawing;
//using System.Collections;
//using System.ComponentModel;
//using System.Windows.Forms;
//using System.Globalization;
//using System.Text.RegularExpressions;
//using System.Data.OleDb;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using UIForm;
using System.IO;

namespace DA.DAA002
{
    public partial class DAA002 : UIForm.FPCOMM1 
    {
        #region 변수선언
        int PreRow = -1;   // SelectionChanged 시에 동일 Row에서 데이타변환 처리 안하도록 하기 위함.
        #endregion

        #region DAA002
        public DAA002()
        {
            InitializeComponent();
        }
        #endregion

        #region DAA002_Load
        private void DAA002_Load(object sender, EventArgs e)
        {
            SystemBase.Base.gstrFromLoading = "N";
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            SystemBase.Validation.GroupBox_Setting(groupBox2);

            //제출업체
            SystemBase.ComboMake.C1Combo(cboH_MNUF_CODE, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'D004', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'", 0);   //제출업체

            //부서
            SystemBase.ComboMake.C1Combo(cboM_DPRT_CODE, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'D007', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'", 9);   //부서
            
            //조달업체
            SystemBase.ComboMake.C1Combo(cboM_CTMF_CODE, "usp_B_COMMON @pTYPE='REL1', @pCODE = 'D006', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'", 9);
           
            // 제출용도
            SystemBase.ComboMake.C1Combo(cboM_PRESENT_USE, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'D008', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'", 9);   //제출용도

            txtH_ORDR_YEAR.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("")).ToString().Substring(0, 4);

            //G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "제출용도")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'D008', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 0);

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0);
            
            SystemBase.Base.gstrFromLoading = "Y";

        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            try
            {
                UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0);

                SystemBase.Validation.GroupBox_Reset(groupBox2);
                SystemBase.Validation.GroupBox_Setting(groupBox2);
                Label_Clear();

                PreRow = -1;
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))  //필수조회조건 체크
                {
                    //SystemBase.Validation.GroupBox_Reset(groupBox2);
                    //SystemBase.Validation.GroupBox_Setting(groupBox2);
                    Label_Clear();

                    this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
                    string strSql = " usp_DAA002  ";
                    strSql += "  @pTYPE = 'S1'";
                    strSql += ", @pMNUF_CODE = '" + cboH_MNUF_CODE.SelectedValue + "' ";
                    strSql += ", @pORDR_YEAR = '" + txtH_ORDR_YEAR.Text + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strSql, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0);

                    int iCnt = fpSpread1.Sheets[0].RowCount;
                    

                    //소계, 합계 컬럼 합치고 색 변경
                    for (int i = 0; i < iCnt; i++)
                    {

                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "제출유무")].Text == "N")
                        {

                            fpSpread1.Sheets[0].Rows[i].BackColor = SystemBase.Base.gColor2;

                        }

                    }



                    this.Cursor = System.Windows.Forms.Cursors.Default;
                }

                PreRow = -1;
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region SaveExec() 폼에 입력된 데이타 저장 로직
        protected override void SaveExec()
        {
            SystemBase.Validation.Control_SaveCheck(groupBox2);
            if (SystemBase.Base.gstrControl_OrgData == SystemBase.Base.gstrControl_SaveData)
            {
                MessageBox.Show(SystemBase.Base.MessageRtn("SY017"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);//변경되거나 처리 할 자료가 없습니다.
                return;
            }

            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox2))  //필수여부체크
            {
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("SY048"), SystemBase.Base.MessageRtn("Z0004"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dsMsg == DialogResult.Yes)
                {
                    string ERRCode = "", MSGCode = "";
                    SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                    SqlCommand cmd = dbConn.CreateCommand();
                    SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);
                    this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
                    try
                    {
                        string strSql = " usp_DAA002 ";
                        strSql += "  @pTYPE = 'I1'";
                        //strSql += ", @pMNUF_CODE = '" + cboM_MNUF_CODE.SelectedValue + "'";                   //제출업체
                        //strSql += ", @pSTD_YRMON ='" + SystemBase.Validation.C1DataEdit_ReadFormat(dtM_STD_YRMON.Value.ToString(), "YYYYMM") + "' ";  //제출년월
                        //strSql += ", @pORDR_YEAR ='" + txtM_ORDR_YEAR.Text + "' ";  //요구년도
                        //strSql += ", @pDPRT_CODE ='" + txtM_DPRT_CODE.Text + "' ";  //구매부서
                        //strSql += ", @pDCSN_NUMB ='" + txtM_DCSN_NUMB.Text + "' ";  //판단번호
                        //strSql += ", @pCALC_DEGR ='" + txtM_CALC_DEGR.Text + "' ";  //제출차수
                        //strSql += ", @pCTMF_CODE ='" + cboM_CTMP_CODE.SelectedValue + "' ";  //계약업체코드
                        //strSql += ", @pNIIN ='" + txtM_NIIN.Text + "' ";       //재고번호
                        //strSql += ", @pUSR_ID ='" + SystemBase.Base.gstrUserID + "' ";                                  //사용자


                        DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                        ERRCode = ds.Tables[0].Rows[0][0].ToString();
                        MSGCode = ds.Tables[0].Rows[0][1].ToString();

                        if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }

                        Trans.Commit();

                        SystemBase.Validation.Control_SearchCheck(groupBox2);  //초기 기본값 저장
                    }
                    catch (Exception f)
                    {
                        SystemBase.Loggers.Log(this.Name, f.ToString());
                        Trans.Rollback();
                        MSGCode = "SY002";	//에러가 발생하여 데이터 처리가 취소되었습니다.
                        MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                Exit:
                    dbConn.Close();
                    this.Cursor = System.Windows.Forms.Cursors.Default;

                    if (ERRCode == "OK")
                    {
                        SearchExec();

                        MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else if (ERRCode == "ER")
                    {
                        MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        #endregion

        #region 좌측그리드 방향키 이동 및 클릭시 우측조회
        private void fpSpread1_SelectionChanged(object sender, FarPoint.Win.Spread.SelectionChangedEventArgs e)
        {
            if (fpSpread1.Sheets[0].Rows.Count > 0)
            {
                int intRow = fpSpread1.ActiveSheet.GetSelection(0).Row;
                if (intRow < 0) return;
                if (PreRow == intRow && PreRow != -1) return;  //현 Row에서 컬럼이동시는 조회 안되게

                SystemBase.Validation.GroupBox_Reset(groupBox2);
                SystemBase.Validation.GroupBox_Setting(groupBox2);
                Label_Clear();

                this.Cursor = Cursors.WaitCursor;
                try
                {
                    string strSql = " usp_DAA002 ";
                    strSql += "  @pTYPE = 'S2' ";
                    strSql += ", @pSTD_SEQ = '" + fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "순번")].Text.ToString() + "' ";
                    strSql += ", @pMNUF_CODE = '" + fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "제출업체")].Text.ToString() + "' ";
                    strSql += ", @pORDR_YEAR = '" + fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "요구년도")].Text.ToString() + "' ";
                    strSql += ", @pDPRT_CODE = '" + fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "부서")].Text.ToString() + "' ";
                    strSql += ", @pDCSN_NUMB = '" + fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "판단번호")].Text.ToString() + "' ";
                    strSql += ", @pCALC_DEGR = '" + fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Text.ToString() + "' ";

                    DataSet ds = SystemBase.DbOpen.NoTranDataSet(strSql);
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        txtM_PK_SEQ.Value = ds.Tables[0].Rows[0]["STD_SEQ"].ToString();                         // 순번
                        cboM_DPRT_CODE.SelectedValue = ds.Tables[0].Rows[0]["DPRT_CODE"].ToString();            // 구매부서
                        txtM_DCSN_NUMB.Value = ds.Tables[0].Rows[0]["DCSN_NUMB"].ToString();                    // 판단번호
                        txtM_CALC_DEGR.Value = ds.Tables[0].Rows[0]["CALC_DEGR"].ToString();                    // 차수
                        txtM_RPST_ITNM.Value = ds.Tables[0].Rows[0]["RPST_ITNM"].ToString();                    // 대표품명
                        dtM_STD_YRMON.Value = SystemBase.Validation.C1DataEdit_WriteFormat(ds.Tables[0].Rows[0]["STD_YRMON"].ToString(), "YYYY-MM"); //기준연월                             
                        cboM_CTMF_CODE.SelectedValue = ds.Tables[0].Rows[0]["CTMF_CODE"].ToString();            // 조달업체
                        cboM_PRESENT_USE.SelectedValue = ds.Tables[0].Rows[0]["SBMTR_CHRG_PURPS"].ToString();   // 제출용도

                        // 제출용도별 버튼 처리
                        if (cboM_PRESENT_USE.SelectedValue.ToString() == "1")
                        {
                            btnAll_Process.Enabled = true;
                            btnBom.Enabled = true; btnBom_Result.Enabled = true;
                            btnRcpt.Enabled = true; btnRcpt_Result.Enabled = true;
                            btnCostInput.Enabled = false; btnCostInput_Result.Enabled = false;
                            btnCostImport.Enabled = true; btnCostImport_Result.Enabled = true;
                            btnExchange.Enabled = true; btnExchange_Result.Enabled = true;
                            btnPacking.Enabled = true; btnPacking_Result.Enabled = true;
                            btnScrap.Enabled = true; btnScrap_Result.Enabled = true;
                            btnLabor.Enabled = true; btnLabor_Result.Enabled = true;
                            btnDepr.Enabled = true; btnDepr_result.Enabled = true;
                            btnRent.Enabled = true; btnRent_result.Enabled = true;
                            btnDepe.Enabled = true; btnDepe_result.Enabled = true;
                            btnOthe.Enabled = true; btnOthe_result.Enabled = true;
                            btnAdde.Enabled = true; btnAdde_result.Enabled = true;
                            btnKrev.Enabled = true; btnKrev_result.Enabled = true;
                            btnFrev.Enabled = true; btnFrev_result.Enabled = true;
                            btnRevd.Enabled = true; btnRevd_result.Enabled = true;
                        }
                        if (cboM_PRESENT_USE.SelectedValue.ToString() == "2")
                        {
                            btnAll_Process.Enabled = true;
                            btnBom.Enabled = true; btnBom_Result.Enabled = true;
                            btnRcpt.Enabled = true; btnRcpt_Result.Enabled = true;
                            btnCostInput.Enabled = false; btnCostInput_Result.Enabled = false;
                            btnCostImport.Enabled = true; btnCostImport_Result.Enabled = true;
                            btnExchange.Enabled = false; btnExchange_Result.Enabled = false;
                            btnPacking.Enabled = false; btnPacking_Result.Enabled = false;
                            btnScrap.Enabled = false; btnScrap_Result.Enabled = false;
                            btnLabor.Enabled = true; btnLabor_Result.Enabled = true;
                            btnDepr.Enabled = false; btnDepr_result.Enabled = false;
                            btnRent.Enabled = false; btnRent_result.Enabled = false;
                            btnDepe.Enabled = false; btnDepe_result.Enabled = false;
                            btnOthe.Enabled = false; btnOthe_result.Enabled = false;
                            btnAdde.Enabled = false; btnAdde_result.Enabled = false;
                            btnKrev.Enabled = false; btnKrev_result.Enabled = false;
                            btnFrev.Enabled = false; btnFrev_result.Enabled = false;
                            btnRevd.Enabled = false; btnRevd_result.Enabled = false;
                        }
                    }

                    #region BOM
                    if (ds.Tables[1].Rows.Count > 0)  // BOM
                    {
                        lblBom_st.Text = ds.Tables[1].Rows[0]["START_TIME"].ToString();    // 시작시간
                        lblBom_et.Text = ds.Tables[1].Rows[0]["END_TIME"].ToString();    // 종료시간
                        lblBom_co.Text = ds.Tables[1].Rows[0]["DATA_STATUS"].ToString();    // 상태
                        if (ds.Tables[1].Rows[0]["DATA_STATUS"].ToString() == "OK")
                            lblBom_co.ForeColor = Color.Blue;
                        else
                            lblBom_co.ForeColor = Color.Red;
                    }
                    #endregion

                    #region 입고이력및외주단가
                    if (ds.Tables[2].Rows.Count > 0)  // 입고이력및외주단가
                    {
                        lblRcpt_st.Text = ds.Tables[2].Rows[0]["START_TIME"].ToString();    // 시작시간
                        lblRcpt_et.Text = ds.Tables[2].Rows[0]["END_TIME"].ToString();    // 종료시간
                        lblRcpt_co.Text = ds.Tables[2].Rows[0]["DATA_STATUS"].ToString();    // 상태
                        if (ds.Tables[2].Rows[0]["DATA_STATUS"].ToString() == "OK")
                            lblRcpt_co.ForeColor = Color.Blue;
                        else
                            lblRcpt_co.ForeColor = Color.Red;
                    }
                    #endregion

                    #region 원가수이이력
                    if (ds.Tables[3].Rows.Count > 0)  // 입고이력및외주단가
                    {
                        lblCostImport_st.Text = ds.Tables[3].Rows[0]["START_TIME"].ToString();    // 시작시간
                        lblCostImport_et.Text = ds.Tables[3].Rows[0]["END_TIME"].ToString();    // 종료시간
                        lblCostImport_co.Text = ds.Tables[3].Rows[0]["DATA_STATUS"].ToString();    // 상태
                        if (ds.Tables[3].Rows[0]["DATA_STATUS"].ToString() == "OK")
                            lblCostImport_co.ForeColor = Color.Blue;
                        else
                            lblCostImport_co.ForeColor = Color.Red;
                    }
                    #endregion

                    #region 환산율 및 환산단가
                    if (ds.Tables[4].Rows.Count > 0)  // 환산율 및 환산단가
                    {
                        lblExchange_st.Text = ds.Tables[4].Rows[0]["START_TIME"].ToString();    // 시작시간
                        lblExchange_et.Text = ds.Tables[4].Rows[0]["END_TIME"].ToString();      // 종료시간
                        lblExchange_co.Text = ds.Tables[4].Rows[0]["DATA_STATUS"].ToString();    // 상태
                        if (ds.Tables[4].Rows[0]["DATA_STATUS"].ToString() == "OK")
                            lblExchange_co.ForeColor = Color.Blue;
                        else
                            lblExchange_co.ForeColor = Color.Red;
                    }
                    #endregion

                    #region 직접노무량
                    if (ds.Tables[5].Rows.Count > 0)  // 직접노무량
                    {
                        lblLabor_st.Text = ds.Tables[5].Rows[0]["START_TIME"].ToString();    // 시작시간
                        lblLabor_et.Text = ds.Tables[5].Rows[0]["END_TIME"].ToString();      // 종료시간
                        lblLabor_co.Text = ds.Tables[5].Rows[0]["DATA_STATUS"].ToString();    // 상태
                        if (ds.Tables[5].Rows[0]["DATA_STATUS"].ToString() == "OK")
                            lblLabor_co.ForeColor = Color.Blue;
                        else
                            lblLabor_co.ForeColor = Color.Red;
                    }
                    #endregion

                    #region 감가상각비
                    if (ds.Tables[6].Rows.Count > 0)  // 감가상각비
                    {
                        lblDepr_st.Text = ds.Tables[6].Rows[0]["START_TIME"].ToString();    // 시작시간
                        lblDepr_et.Text = ds.Tables[6].Rows[0]["END_TIME"].ToString();      // 종료시간
                        lblDepr_co.Text = ds.Tables[6].Rows[0]["DATA_STATUS"].ToString();    // 상태
                        if (ds.Tables[6].Rows[0]["DATA_STATUS"].ToString() == "OK")
                            lblDepr_co.ForeColor = Color.Blue;
                        else
                            lblDepr_co.ForeColor = Color.Red;
                    }
                    #endregion

                    #region 지급임차료
                    if (ds.Tables[7].Rows.Count > 0)  // 지급임차료
                    {
                        lblRent_st.Text = ds.Tables[7].Rows[0]["START_TIME"].ToString();    // 시작시간
                        lblRent_et.Text = ds.Tables[7].Rows[0]["END_TIME"].ToString();      // 종료시간
                        lblRent_co.Text = ds.Tables[7].Rows[0]["DATA_STATUS"].ToString();    // 상태
                        if (ds.Tables[7].Rows[0]["DATA_STATUS"].ToString() == "OK")
                            lblRent_co.ForeColor = Color.Blue;
                        else
                            lblRent_co.ForeColor = Color.Red;
                    }
                    #endregion

                    #region 감가상각대상경비
                    if (ds.Tables[8].Rows.Count > 0)  // 지급임차료
                    {
                        lblDepe_st.Text = ds.Tables[8].Rows[0]["START_TIME"].ToString();    // 시작시간
                        lblDepe_et.Text = ds.Tables[8].Rows[0]["END_TIME"].ToString();      // 종료시간
                        lblDepe_co.Text = ds.Tables[8].Rows[0]["DATA_STATUS"].ToString();    // 상태
                        if (ds.Tables[8].Rows[0]["DATA_STATUS"].ToString() == "OK")
                            lblDepe_co.ForeColor = Color.Blue;
                        else
                            lblDepe_co.ForeColor = Color.Red;
                    }
                    #endregion

                    #region 기타경비
                    if (ds.Tables[9].Rows.Count > 0)  // 지급임차료
                    {
                        lblOthe_st.Text = ds.Tables[9].Rows[0]["START_TIME"].ToString();    // 시작시간
                        lblOthe_et.Text = ds.Tables[9].Rows[0]["END_TIME"].ToString();      // 종료시간
                        lblOthe_co.Text = ds.Tables[9].Rows[0]["DATA_STATUS"].ToString();    // 상태
                        if (ds.Tables[9].Rows[0]["DATA_STATUS"].ToString() == "OK")
                            lblOthe_co.ForeColor = Color.Blue;
                        else
                            lblOthe_co.ForeColor = Color.Red;
                    }
                    #endregion

                    #region 추가경비
                    if (ds.Tables[10].Rows.Count > 0)  // 지급임차료
                    {
                        lblAdde_st.Text = ds.Tables[10].Rows[0]["START_TIME"].ToString();    // 시작시간
                        lblAdde_et.Text = ds.Tables[10].Rows[0]["END_TIME"].ToString();      // 종료시간
                        lblAdde_co.Text = ds.Tables[10].Rows[0]["DATA_STATUS"].ToString();    // 상태
                        if (ds.Tables[10].Rows[0]["DATA_STATUS"].ToString() == "OK")
                            lblAdde_co.ForeColor = Color.Blue;
                        else
                            lblAdde_co.ForeColor = Color.Red;
                    }
                    #endregion

                    #region 포장재료비
                    if (ds.Tables[11].Rows.Count > 0)  // 포장재료비
                    {
                        lblPacking_st.Text = ds.Tables[11].Rows[0]["START_TIME"].ToString();    // 시작시간
                        lblPacking_et.Text = ds.Tables[11].Rows[0]["END_TIME"].ToString();      // 종료시간
                        lblPacking_co.Text = ds.Tables[11].Rows[0]["DATA_STATUS"].ToString();    // 상태
                        if (ds.Tables[11].Rows[0]["DATA_STATUS"].ToString() == "OK")
                            lblPacking_co.ForeColor = Color.Blue;
                        else
                            lblPacking_co.ForeColor = Color.Red;
                    }
                    #endregion

                    #region 원가투입이력
                    if (ds.Tables[12].Rows.Count > 0)  // 원가투입이력
                    {
                        lblCostInput_st.Text = ds.Tables[12].Rows[0]["START_TIME"].ToString();    // 시작시간
                        lblCostInput_et.Text = ds.Tables[12].Rows[0]["END_TIME"].ToString();      // 종료시간
                        lblCostInput_co.Text = ds.Tables[12].Rows[0]["DATA_STATUS"].ToString();    // 상태
                        if (ds.Tables[12].Rows[0]["DATA_STATUS"].ToString() == "OK")
                            lblCostInput_co.ForeColor = Color.Blue;
                        else
                            lblCostInput_co.ForeColor = Color.Red;
                    }
                    #endregion

                    SystemBase.Validation.Control_SearchCheck(groupBox2);  //초기 기본값 저장
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("SY013"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Cursor = Cursors.Default;
                }


                SystemBase.Validation.GroupBox_SearchViewValidation(groupBox2);

                PreRow = fpSpread1.ActiveSheet.GetSelection(0).Row;
                this.Cursor = Cursors.Default;
            }
        }
        #endregion

        #region **************************************  업체, 공장, 결산기간(From~To)은 글로벌 변수에 할당********************************************
        private void cboH_MNUF_CODE_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (SystemBase.Base.gstrFromLoading == "Y")
                {
                    SystemBase.Base.gstrMNUF_CODE = (cboH_MNUF_CODE.SelectedValue == null ? "" : cboH_MNUF_CODE.SelectedValue.ToString());

                    //구매부서
                    SystemBase.ComboMake.C1Combo(cboM_DPRT_CODE, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'D007', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'", 0);   //부서
                    
                    //조달업체
                    SystemBase.ComboMake.C1Combo(cboM_CTMF_CODE, "usp_B_COMMON @pTYPE='REL1', @pCODE = 'D006', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pSPEC1 = '" + cboH_MNUF_CODE.SelectedValue.ToString() + "'", 0);
                    
                    NewExec();
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region **************************************  업체, 공장, 결산기간(From~To)은 변경시 초기화 및 변경여부체크 ******************************
        private void cboH_MNUF_CODE_BeforeOpen(object sender, CancelEventArgs e)
        {
            try
            {
                Value_Selected(e, null, null);
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Value_Selected(CancelEventArgs e, KeyPressEventArgs f, C1.Win.C1Input.UpDownButtonClickEventArgs g)
        {
            try
            {
                //그리드 변경여부체크를 쓰기위하여 TabFPMake에 만든함수를 사용함.
                if (UIForm.TabFPMake.FPGrid_Closing(fpSpread1) > 0)
                {
                    if (FpGrid_DialogResult(fpSpread1, e, f, g) == false) return;
                }

                NewExec();
            }
            catch (Exception o)
            {
                MessageBox.Show(o.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private bool FpGrid_DialogResult(FarPoint.Win.Spread.FpSpread FPGrid, CancelEventArgs e, KeyPressEventArgs f, C1.Win.C1Input.UpDownButtonClickEventArgs g)
        {
            try
            {
                if (FPGrid.ActiveSheet.RowCount <= 0) return false;

                DialogResult Rtn = MessageBox.Show(SystemBase.Base.MessageRtn("SY066"), SystemBase.Base.MessageRtn("Z0004"), MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (Rtn != DialogResult.OK)
                {
                    if (e != null)
                    {
                        e.Cancel = true;
                    }
                    if (f != null)
                    {
                        f.Handled = true;
                    }
                    if (g != null)
                    {
                        g.Done = true;
                    }

                    return false;
                }
                else
                {
                    NewExec();

                    return true;
                }
            }
            catch (Exception o)
            {
                MessageBox.Show(o.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
        #endregion

        #region LABEL CLEAR
        private void Label_Clear()
        {
            try
            {
                //lblAll_Process_st.Text = ""; lblAll_Process_et.Text = ""; lblAll_Process_co.Text = "";
                lblBom_st.Text = ""; lblBom_et.Text = ""; lblBom_co.Text = "";
                lblRcpt_st.Text = ""; lblRcpt_et.Text = ""; lblRcpt_co.Text = "";
                lblCostInput_st.Text = ""; lblCostInput_et.Text = ""; lblCostInput_co.Text = "";
                lblCostImport_st.Text = ""; lblCostImport_et.Text = ""; lblCostImport_co.Text = "";
                lblExchange_st.Text = ""; lblExchange_et.Text = ""; lblExchange_co.Text = "";
                lblPacking_st.Text = ""; lblPacking_et.Text = ""; lblPacking_co.Text = "";
                lblScrap_st.Text = ""; lblScrap_et.Text = ""; lblScrap_co.Text = "";
                lblLabor_st.Text = ""; lblLabor_et.Text = ""; lblLabor_co.Text = "";
                lblDepr_st.Text = ""; lblDepr_et.Text = ""; lblDepr_co.Text = "";
                lblRent_st.Text = ""; lblRent_et.Text = ""; lblRent_co.Text = "";
                lblDepe_st.Text = ""; lblDepe_et.Text = ""; lblDepe_co.Text = "";
                lblOthe_st.Text = ""; lblOthe_et.Text = ""; lblOthe_co.Text = "";
                lblAdde_st.Text = ""; lblAdde_et.Text = ""; lblAdde_co.Text = "";
                lblKrev_st.Text = ""; lblKrev_et.Text = ""; lblKrev_co.Text = "";
                lblFrev_st.Text = ""; lblFrev_et.Text = ""; lblFrev_co.Text = "";
                lblRevd_st.Text = ""; lblRevd_et.Text = ""; lblRevd_co.Text = "";
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 수집결과 버튼 조회
        private void btnBom_Result_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtM_PK_SEQ.Text == "") return;
                // 마스터키, 제출업체,요구년도,부서,판단번호,차수,폼ID
                DAA002P01 pu = new DAA002P01(Convert.ToInt32(txtM_PK_SEQ.Text), cboH_MNUF_CODE.SelectedValue.ToString(), txtH_ORDR_YEAR.Text, cboM_DPRT_CODE.SelectedValue.ToString(), txtM_DCSN_NUMB.Text, txtM_CALC_DEGR.Text, this.Name);
                pu.Owner = this;
                pu.Show();
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnRcpt_Result_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtM_PK_SEQ.Text == "") return;
                // 마스터키, 제출업체,요구년도,부서,판단번호,차수,폼ID
                DAA002P02 pu = new DAA002P02(Convert.ToInt32(txtM_PK_SEQ.Text), cboH_MNUF_CODE.SelectedValue.ToString(), txtH_ORDR_YEAR.Text, cboM_DPRT_CODE.SelectedValue.ToString(), txtM_DCSN_NUMB.Text, txtM_CALC_DEGR.Text, this.Name);
                pu.Owner = this;
                pu.Show();
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnCostImport_Result_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtM_PK_SEQ.Text == "") return;
                // 마스터키, 제출업체,요구년도,부서,판단번호,차수,폼ID
                DAA002P03 pu = new DAA002P03(Convert.ToInt32(txtM_PK_SEQ.Text), cboH_MNUF_CODE.SelectedValue.ToString(), txtH_ORDR_YEAR.Text, cboM_DPRT_CODE.SelectedValue.ToString(), txtM_DCSN_NUMB.Text, txtM_CALC_DEGR.Text, this.Name);
                pu.Owner = this;
                pu.Show();
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnExchange_Result_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtM_PK_SEQ.Text == "") return;
                // 마스터키, 제출업체,요구년도,부서,판단번호,차수,폼ID
                DAA002P04 pu = new DAA002P04(Convert.ToInt32(txtM_PK_SEQ.Text), cboH_MNUF_CODE.SelectedValue.ToString(), txtH_ORDR_YEAR.Text, cboM_DPRT_CODE.SelectedValue.ToString(), txtM_DCSN_NUMB.Text, txtM_CALC_DEGR.Text, this.Name);
                pu.Owner = this;
                pu.Show();
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnLabor_Result_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtM_PK_SEQ.Text == "") return;
                // 마스터키, 제출업체,요구년도,부서,판단번호,차수,폼ID
                DAA002P05 pu = new DAA002P05(Convert.ToInt32(txtM_PK_SEQ.Text), cboH_MNUF_CODE.SelectedValue.ToString(), txtH_ORDR_YEAR.Text, cboM_DPRT_CODE.SelectedValue.ToString(), txtM_DCSN_NUMB.Text, txtM_CALC_DEGR.Text, this.Name);
                pu.Owner = this;
                pu.Show();
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnDepr_Result_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtM_PK_SEQ.Text == "") return;
                // 마스터키, 제출업체,요구년도,부서,판단번호,차수,폼ID
                DAA002P06 pu = new DAA002P06(Convert.ToInt32(txtM_PK_SEQ.Text), cboH_MNUF_CODE.SelectedValue.ToString(), txtH_ORDR_YEAR.Text, cboM_DPRT_CODE.SelectedValue.ToString(), txtM_DCSN_NUMB.Text, txtM_CALC_DEGR.Text, this.Name);
                pu.Owner = this;
                pu.Show();
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnRent_result_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtM_PK_SEQ.Text == "") return;
                // 마스터키, 제출업체,요구년도,부서,판단번호,차수,폼ID
                DAA002P07 pu = new DAA002P07(Convert.ToInt32(txtM_PK_SEQ.Text), cboH_MNUF_CODE.SelectedValue.ToString(), txtH_ORDR_YEAR.Text, cboM_DPRT_CODE.SelectedValue.ToString(), txtM_DCSN_NUMB.Text, txtM_CALC_DEGR.Text, this.Name);
                pu.Owner = this;
                pu.Show();
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnDepe_result_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtM_PK_SEQ.Text == "") return;
                // 마스터키, 제출업체,요구년도,부서,판단번호,차수,폼ID
                DAA002P08 pu = new DAA002P08(Convert.ToInt32(txtM_PK_SEQ.Text), cboH_MNUF_CODE.SelectedValue.ToString(), txtH_ORDR_YEAR.Text, cboM_DPRT_CODE.SelectedValue.ToString(), txtM_DCSN_NUMB.Text, txtM_CALC_DEGR.Text, this.Name);
                pu.Owner = this;
                pu.Show();
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnOthe_result_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtM_PK_SEQ.Text == "") return;
                // 마스터키, 제출업체,요구년도,부서,판단번호,차수,폼ID
                DAA002P09 pu = new DAA002P09(Convert.ToInt32(txtM_PK_SEQ.Text), cboH_MNUF_CODE.SelectedValue.ToString(), txtH_ORDR_YEAR.Text, cboM_DPRT_CODE.SelectedValue.ToString(), txtM_DCSN_NUMB.Text, txtM_CALC_DEGR.Text, this.Name);
                pu.Owner = this;
                pu.Show();
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnAdde_result_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtM_PK_SEQ.Text == "") return;
                // 마스터키, 제출업체,요구년도,부서,판단번호,차수,폼ID
                DAA002P10 pu = new DAA002P10(Convert.ToInt32(txtM_PK_SEQ.Text), cboH_MNUF_CODE.SelectedValue.ToString(), txtH_ORDR_YEAR.Text, cboM_DPRT_CODE.SelectedValue.ToString(), txtM_DCSN_NUMB.Text, txtM_CALC_DEGR.Text, this.Name);
                pu.Owner = this;
                pu.Show();
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnPacking_Result_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtM_PK_SEQ.Text == "") return;
                // 마스터키, 제출업체,요구년도,부서,판단번호,차수,폼ID
                DAA002P12 pu = new DAA002P12(Convert.ToInt32(txtM_PK_SEQ.Text), cboH_MNUF_CODE.SelectedValue.ToString(), txtH_ORDR_YEAR.Text, cboM_DPRT_CODE.SelectedValue.ToString(), txtM_DCSN_NUMB.Text, txtM_CALC_DEGR.Text, this.Name);
                pu.Owner = this;
                pu.Show();
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnScrap_Result_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtM_PK_SEQ.Text == "") return;
                // 마스터키, 제출업체,요구년도,부서,판단번호,차수,폼ID
                DAA002P13 pu = new DAA002P13(Convert.ToInt32(txtM_PK_SEQ.Text), cboH_MNUF_CODE.SelectedValue.ToString(), txtH_ORDR_YEAR.Text, cboM_DPRT_CODE.SelectedValue.ToString(), txtM_DCSN_NUMB.Text, txtM_CALC_DEGR.Text, this.Name);
                pu.Owner = this;
                pu.Show();
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnKrev_result_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtM_PK_SEQ.Text == "") return;
                // 마스터키, 제출업체,요구년도,부서,판단번호,차수,폼ID
                DAA002P14 pu = new DAA002P14(Convert.ToInt32(txtM_PK_SEQ.Text), cboH_MNUF_CODE.SelectedValue.ToString(), txtH_ORDR_YEAR.Text, cboM_DPRT_CODE.SelectedValue.ToString(), txtM_DCSN_NUMB.Text, txtM_CALC_DEGR.Text, this.Name);
                pu.Owner = this;
                pu.Show();
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnFrev_result_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtM_PK_SEQ.Text == "") return;
                // 마스터키, 제출업체,요구년도,부서,판단번호,차수,폼ID
                DAA002P15 pu = new DAA002P15(Convert.ToInt32(txtM_PK_SEQ.Text), cboH_MNUF_CODE.SelectedValue.ToString(), txtH_ORDR_YEAR.Text, cboM_DPRT_CODE.SelectedValue.ToString(), txtM_DCSN_NUMB.Text, txtM_CALC_DEGR.Text, this.Name);
                pu.Owner = this;
                pu.Show();
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnRevd_result_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtM_PK_SEQ.Text == "") return;
                // 마스터키, 제출업체,요구년도,부서,판단번호,차수,폼ID
                DAA002P16 pu = new DAA002P16(Convert.ToInt32(txtM_PK_SEQ.Text), cboH_MNUF_CODE.SelectedValue.ToString(), txtH_ORDR_YEAR.Text, cboM_DPRT_CODE.SelectedValue.ToString(), txtM_DCSN_NUMB.Text, txtM_CALC_DEGR.Text, this.Name);
                pu.Owner = this;
                pu.Show();
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnCostInput_Result_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtM_PK_SEQ.Text == "") return;
                // 마스터키, 제출업체,요구년도,부서,판단번호,차수,폼ID
                DAA002P17 pu = new DAA002P17(Convert.ToInt32(txtM_PK_SEQ.Text), cboH_MNUF_CODE.SelectedValue.ToString(), txtH_ORDR_YEAR.Text, cboM_DPRT_CODE.SelectedValue.ToString(), txtM_DCSN_NUMB.Text, txtM_CALC_DEGR.Text, this.Name);
                pu.Owner = this;
                pu.Show();
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 수집생성
        private void btnAll_Process_Click(object sender, EventArgs e)   // 일괄처리
        {
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox2))  //필수여부체크
            {
                if (Date_Check() == false) return; // 일자체크

                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("GB003"), SystemBase.Base.MessageRtn("Z0004"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dsMsg == DialogResult.Yes)
                {
                    this.Cursor = Cursors.WaitCursor;

                    string ERRCode = "", MSGCode = "";
                    SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                    SqlCommand cmd = dbConn.CreateCommand();
                    SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);
                    try
                    {
                        string strSql = " usp_DAA002 ";
                        strSql += "  @pTYPE = 'D2' ";
                        strSql += ", @pSTD_SEQ = " + Convert.ToInt32(txtM_PK_SEQ.Text) + " ";
                        DataSet ds2 = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                        ERRCode = ds2.Tables[0].Rows[0][0].ToString();
                        MSGCode = ds2.Tables[0].Rows[0][1].ToString();

                        if (ERRCode == "ER")
                        {
                            SystemBase.Loggers.Log(this.Name, MSGCode.ToString());
                            this.Cursor = Cursors.Default;
                            Trans.Rollback();
                            MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        Trans.Commit();
                    }
                    catch (Exception f)
                    {
                        SystemBase.Loggers.Log(this.Name, f.ToString());
                        Trans.Rollback();
                        MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    Label_Clear();
                    Application.DoEvents();

                    try{
                        if (cboM_PRESENT_USE.SelectedValue.ToString() == "1")
                        {
                            if (Batch_Create("BOM") == false) return;            // BOM 생성   
                            if (Batch_Create("RCPT") == false) return;           // 입고이력 및 외주단가
                            if (Batch_Create("IMPT") == false) return;           // 수입이력
                            if (Batch_Create("EXCH") == false) return;           // 환산율 및 환산단가
                            if (Batch_Create("PACK") == false) return;           // 포장재료비
                            if (Batch_Create("LABR") == false) return;           // 직접노무량
                            if (Batch_Create("DEPR") == false) return;           // 감가상각비
                            if (Batch_Create("RENT") == false) return;           // 지급임차료
                            if (Batch_Create("DEPE") == false) return;           // 감가상각대상경비
                            if (Batch_Create("OTHE") == false) return;           // 기타경비
                            if (Batch_Create("ADDE") == false) return;           // 추가경비
                        }
                        else if (cboM_PRESENT_USE.SelectedValue.ToString() == "2")
                        {
                            if (Batch_Create("BOM") == false) return;            // BOM 생성   
                            if (Batch_Create("RCPT") == false) return;           // 입고이력 및 외주단가
                            //if (Batch_Create("ISSU") == false) return;           // 투입이력
                            if (Batch_Create("IMPT") == false) return;           // 수입이력
                            if (Batch_Create("LABR") == false) return;           // 직접노무량
                        }
                    }
                    catch (Exception f)
                    {
                        SystemBase.Loggers.Log(this.Name, f.ToString());
                        MessageBox.Show(SystemBase.Base.MessageRtn(f.ToString()), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    this.Cursor = Cursors.Default;
                }
            }

        }

        private void btnBom_Click(object sender, EventArgs e)           // BOM 생성
        {
            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox2))  //필수여부체크
                {
                    DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("GB001", btnBom.Text), SystemBase.Base.MessageRtn("Z0004"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dsMsg == DialogResult.Yes)
                    {
                        if (Create_Check("BOM", btnBom.Text) == false) return;  // 수집자료존재 체크

                        lblBom_st.Text = ""; lblBom_et.Text = ""; lblBom_co.Text = "";
                        Application.DoEvents();

                        Batch_Create("BOM");
                    }
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            this.Cursor = Cursors.Default;
        }
        private void btnRcpt_Click(object sender, EventArgs e)          // 입고이력 및 외주단가
        {
            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox2))  //필수여부체크
                {
                    DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("GB001", btnRcpt.Text), SystemBase.Base.MessageRtn("Z0004"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dsMsg == DialogResult.Yes)
                    {
                        if (Create_Check("RCPT", btnRcpt.Text) == false) return;  // 수집자료존재 체크

                        lblRcpt_st.Text = ""; lblRcpt_et.Text = ""; lblRcpt_co.Text = "";
                        Application.DoEvents();

                        Batch_Create("RCPT");
                    }
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnCostInput_Click(object sender, EventArgs e)     // 투입이력
        {
            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox2))  //필수여부체크
                {
                    DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("GB001", btnCostInput.Text), SystemBase.Base.MessageRtn("Z0004"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dsMsg == DialogResult.Yes)
                    {
                        if (Create_Check("ISSU", btnCostInput.Text) == false) return;  // 수집자료존재 체크

                        lblCostInput_st.Text = ""; lblCostInput_et.Text = ""; lblCostInput_co.Text = "";
                        Application.DoEvents();

                        Batch_Create("ISSU");
                    }
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnCostImport_Click(object sender, EventArgs e)    // 수입이력
        {
            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox2))  //필수여부체크
                {
                    DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("GB001", btnCostImport.Text), SystemBase.Base.MessageRtn("Z0004"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dsMsg == DialogResult.Yes)
                    {
                        if (Create_Check("IMPT", btnCostImport.Text) == false) return;  // 수집자료존재 체크                    

                        lblCostImport_st.Text = ""; lblCostImport_et.Text = ""; lblCostImport_co.Text = "";
                        Application.DoEvents();

                        Batch_Create("IMPT");
                    }
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnExchange_Click(object sender, EventArgs e)      // 환산율 및 환산단가
        {
            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox2))  //필수여부체크
                {
                    DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("GB001", btnExchange.Text), SystemBase.Base.MessageRtn("Z0004"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dsMsg == DialogResult.Yes)
                    {
                        if (Create_Check("EXCH", btnExchange.Text) == false) return;  // 수집자료존재 체크                    

                        lblExchange_st.Text = ""; lblExchange_et.Text = ""; lblExchange_co.Text = "";
                        Application.DoEvents();

                        Batch_Create("EXCH");
                    }
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnPacking_Click(object sender, EventArgs e)       // 포장재료비
        {
            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox2))  //필수여부체크
                {
                    DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("GB001", btnPacking.Text), SystemBase.Base.MessageRtn("Z0004"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dsMsg == DialogResult.Yes)
                    {
                        if (Create_Check("PACK", btnBom.Text) == false) return;  // 수집자료존재 체크

                        lblPacking_st.Text = ""; lblPacking_et.Text = ""; lblPacking_co.Text = "";
                        Application.DoEvents();

                        Batch_Create("PACK");
                    }
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnLabor_Click(object sender, EventArgs e)   // 직접노무량
        {
            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox2))  //필수여부체크
                {
                    DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("GB001", btnLabor.Text), SystemBase.Base.MessageRtn("Z0004"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dsMsg == DialogResult.Yes)
                    {
                        if (Create_Check("LABR", btnLabor.Text) == false) return;  // 수집자료존재 체크                    

                        lblLabor_st.Text = ""; lblLabor_et.Text = ""; lblLabor_co.Text = "";
                        Application.DoEvents();

                        Batch_Create("LABR");
                    }
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnDepr_Click(object sender, EventArgs e)    // 감가상각비(전용)
        {
            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox2))  //필수여부체크
                {
                    DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("GB001", btnDepr.Text), SystemBase.Base.MessageRtn("Z0004"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dsMsg == DialogResult.Yes)
                    {
                        if (Create_Check("DEPR", btnDepr.Text) == false) return;  // 수집자료존재 체크                    

                        lblDepr_st.Text = ""; lblDepr_et.Text = ""; lblDepr_co.Text = "";
                        Application.DoEvents();

                        Batch_Create("DEPR");
                    }
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnRent_Click(object sender, EventArgs e)    // 지급임차료(전용)
        {
            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox2))  //필수여부체크
                {
                    DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("GB001", btnRent.Text), SystemBase.Base.MessageRtn("Z0004"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dsMsg == DialogResult.Yes)
                    {
                        if (Create_Check("RENT", btnRent.Text) == false) return;  // 수집자료존재 체크                    

                        lblRent_st.Text = ""; lblRent_et.Text = ""; lblRent_co.Text = "";
                        Application.DoEvents();

                        Batch_Create("RENT");
                    }
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnDepe_Click(object sender, EventArgs e)    // 감가상각대상경비
        {
            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox2))  //필수여부체크
                {
                    DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("GB001", btnDepe.Text), SystemBase.Base.MessageRtn("Z0004"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dsMsg == DialogResult.Yes)
                    {
                        if (Create_Check("DEPE", btnDepe.Text) == false) return;  // 수집자료존재 체크                    

                        lblDepe_st.Text = ""; lblDepe_et.Text = ""; lblDepe_co.Text = "";
                        Application.DoEvents();

                        Batch_Create("DEPE");
                    }
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnOthe_Click(object sender, EventArgs e)    // 기타경비
        {
            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox2))  //필수여부체크
                {
                    DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("GB001", btnOthe.Text), SystemBase.Base.MessageRtn("Z0004"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dsMsg == DialogResult.Yes)
                    {
                        if (Create_Check("OTHE", btnOthe.Text) == false) return;  // 수집자료존재 체크                    

                        lblOthe_st.Text = ""; lblOthe_et.Text = ""; lblOthe_co.Text = "";
                        Application.DoEvents();

                        Batch_Create("OTHE");
                    }
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnAdde_Click(object sender, EventArgs e)    // 추가경비
        {
            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox2))  //필수여부체크
                {
                    DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("GB001", btnAdde.Text), SystemBase.Base.MessageRtn("Z0004"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dsMsg == DialogResult.Yes)
                    {
                        if (Create_Check("ADDE", btnAdde.Text) == false) return;  // 수집자료존재 체크                    

                        lblAdde_st.Text = ""; lblAdde_et.Text = ""; lblAdde_co.Text = "";
                        Application.DoEvents();

                        Batch_Create("ADDE");
                    }
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }      
        #endregion

        #region 자료수집생성
        private bool Batch_Create(string Create_Flag)
        {
            this.Cursor = Cursors.WaitCursor;
            SqlConnection dbConn = SystemBase.DbOpen.DBCON();
            SqlCommand cmd = dbConn.CreateCommand();

            bool Batch_Create = true;
            string strTYPE = "";
            switch (Create_Flag)
            {
                case "BOM": strTYPE = "BOM"; break;     // BOM 생성
                case "RCPT": strTYPE = "RCPT"; break;   // 입고이력 및 외주단가
                case "ISSU": strTYPE = "ISSU"; break;   // 투입이력
                case "IMPT": strTYPE = "IMPT"; break;   // 수입이력
                case "EXCH": strTYPE = "EXCH"; break;   // 환산율 및 환산단가
                case "PACK": strTYPE = "PACK"; break;   // 포장재료비
                case "LABR": strTYPE = "LABR"; break;   // 직접노무량
                case "DEPR": strTYPE = "DEPR"; break;   // 감가상각비
                case "RENT": strTYPE = "RENT"; break;   // 지급임차료
                case "DEPE": strTYPE = "DEPE"; break;   // 감가상각대상경비
                case "OTHE": strTYPE = "OTHE"; break;   // 기타경비
                case "ADDE": strTYPE = "ADDE"; break;   // 추가경비
                default: strTYPE = ""; break;
            }

            try
            {
                Label_Status("ST", strTYPE, "");

                string ERRCode = "", MSGCode = "";
                string strSql = " usp_DAA002 ";
                strSql += "  @pTYPE = '" + strTYPE + "' ";
                strSql += ", @pSTD_SEQ = " + Convert.ToInt32(txtM_PK_SEQ.Text) + " ";            //순번
                strSql += ", @pMNUF_CODE = '" + cboH_MNUF_CODE.SelectedValue.ToString() + "' "; //제출업체코드
                strSql += ", @pORDR_YEAR = '" + txtH_ORDR_YEAR.Text.ToString() + "' ";          //요구년도
                strSql += ", @pDPRT_CODE = '" + cboM_DPRT_CODE.SelectedValue.ToString() + "' ";
                strSql += ", @pDCSN_NUMB = '" + txtM_DCSN_NUMB.Text.ToString() + "' ";
                strSql += ", @pCALC_DEGR = '" + txtM_CALC_DEGR.Text.ToString() + "' ";
                strSql += ", @pSTD_YRMON = '" + SystemBase.Validation.C1DataEdit_ReadFormat(dtM_STD_YRMON.Value.ToString(), "YYYYMM") + "' ";
                strSql += ", @pIN_ID ='" + SystemBase.Base.gstrUserID + "' ";
                strSql += ", @pCOMP_STD_DATE ='" + txtM_COMP_STD_DATE.Value.ToString() + "'"; // 계산기준일
                strSql += ", @pPRESENT_USE ='" + cboM_PRESENT_USE.SelectedValue.ToString() + "'"; // 제출용도

                DataSet ds = SystemBase.DbOpen.NoTranDataSet(strSql);
                ERRCode = ds.Tables[0].Rows[0][0].ToString();
                MSGCode = ds.Tables[0].Rows[0][1].ToString();
                Label_Status("CO", strTYPE, ERRCode);

                if (ERRCode == "ER")
                {
                    SystemBase.Loggers.Log(this.Name, MSGCode.ToString());
                    Batch_Create = false;
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception f)
            {
                Label_Status("CO", strTYPE, "ERROR");
                SystemBase.Loggers.Log(this.Name, f.ToString());
                Batch_Create = false;
                MessageBox.Show(SystemBase.Base.MessageRtn(f.ToString()), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            dbConn.Close();
            Label_Status("ET", strTYPE, "");

            this.Cursor = Cursors.Default;

            return Batch_Create;
        }
        #endregion

        #region 생성전 체크작업
        private bool Create_Check(string Create_Flag, string BtnText)
        {
            bool Create_Check = true;
            string strDATA_FLAG = "";

            if (Date_Check() == false)   // 일자체크
            {
                Create_Check = false;
                return Create_Check;
            }

            switch (Create_Flag)
            {
                case "BOM": strDATA_FLAG = "BOM"; break;        // BOM 생성
                case "RCPT": strDATA_FLAG = "RCPT"; break;      // 입고이력 및 외주단가
                case "ISSU": strDATA_FLAG = "ISSU"; break;      // 투입이력
                case "IMPT": strDATA_FLAG = "IMPT"; break;      // 수입이력
                case "EXCH": strDATA_FLAG = "EXCH"; break;      // 환산율 및 환산단가
                case "PACK": strDATA_FLAG = "PACK"; break;      // 포장재료비
                case "LABR": strDATA_FLAG = "LABR"; break;      // 직접노무량
                case "DEPR": strDATA_FLAG = "DEPR"; break;      // 감가상각비
                case "RENT": strDATA_FLAG = "RENT"; break;      // 지급임차료
                case "DEPE": strDATA_FLAG = "DEPE"; break;      // 감가상각대상경비
                case "OTHE": strDATA_FLAG = "OTHE"; break;      // 기타경비
                case "ADDE": strDATA_FLAG = "ADDE"; break;      // 추가경비
                default: strDATA_FLAG = ""; break;
            }

            string strSql = " usp_DAA002 ";
            strSql += "  @pTYPE = 'C1' ";
            strSql += ", @pDATA_FLAG = '" + strDATA_FLAG + "' ";
            strSql += ", @pSTD_SEQ = " + Convert.ToInt32(txtM_PK_SEQ.Text) + " ";
            DataSet ds = SystemBase.DbOpen.NoTranDataSet(strSql);
            if (ds.Tables[0].Rows.Count > 0)
            {
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("GB002", BtnText), SystemBase.Base.MessageRtn("Z0004"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dsMsg == DialogResult.No)
                {
                    Create_Check = false;
                    return Create_Check;
                }
                else
                {
                    string ERRCode = "", MSGCode = "";
                    SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                    SqlCommand cmd = dbConn.CreateCommand();
                    SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);
                    try
                    {
                        strSql = " usp_DAA002 ";
                        strSql += "  @pTYPE = 'D1' ";
                        strSql += ", @pDATA_FLAG = '" + strDATA_FLAG + "' ";
                        strSql += ", @pSTD_SEQ = " + Convert.ToInt32(txtM_PK_SEQ.Text) + " ";
                        DataSet ds2 = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                        ERRCode = ds2.Tables[0].Rows[0][0].ToString();
                        MSGCode = ds2.Tables[0].Rows[0][1].ToString();

                        if (ERRCode == "ER")
                        {
                            Trans.Rollback();
                            MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Create_Check = false;
                            return Create_Check;
                        }

                        Trans.Commit();
                    }
                    catch (Exception f)
                    {
                        SystemBase.Loggers.Log(this.Name, f.ToString());
                        Trans.Rollback();
                        MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Create_Check = false;
                        return Create_Check;
                    }
                }
            }

            return Create_Check;
        }
        #endregion

        #region 일자체크
        private bool Date_Check()
        {
            try
            {
                bool Date_Check = true;

                if (fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "기준연월")].Text.ToString() == "")
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn("SY079"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Date_Check = false;
                    return Date_Check;
                }
                if (fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "추출시작월")].Text.ToString() == "")
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn("SY080"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Date_Check = false;
                    return Date_Check;
                }
                if (fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "추출종료월")].Text.ToString() == "")
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn("SY080"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Date_Check = false;
                    return Date_Check;
                }

                return Date_Check;
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
        #endregion

        #region Label_Status
        private void Label_Status(string StatusTime, string Create_Flag, string ERRCode)
        {
            try
            {
                if (StatusTime == "ST")
                {
                    if (Create_Flag == "BOM") lblBom_st.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    if (Create_Flag == "RCPT") lblRcpt_st.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    if (Create_Flag == "ISSU") lblCostInput_st.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    if (Create_Flag == "IMPT") lblCostImport_st.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    if (Create_Flag == "EXCH") lblExchange_st.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    if (Create_Flag == "PACK") lblPacking_st.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    if (Create_Flag == "LABR") lblLabor_st.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    if (Create_Flag == "DEPR") lblDepr_st.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    if (Create_Flag == "RENT") lblRent_st.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    if (Create_Flag == "DEPE") lblDepe_st.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    if (Create_Flag == "OTHE") lblOthe_st.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    if (Create_Flag == "ADDE") lblAdde_st.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                }
                else if (StatusTime == "ET")
                {
                    if (Create_Flag == "BOM") lblBom_et.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    if (Create_Flag == "RCPT") lblRcpt_et.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    if (Create_Flag == "ISSU") lblCostInput_et.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    if (Create_Flag == "IMPT") lblCostImport_et.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    if (Create_Flag == "EXCH") lblExchange_et.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    if (Create_Flag == "PACK") lblPacking_et.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    if (Create_Flag == "LABR") lblLabor_et.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    if (Create_Flag == "DEPR") lblDepr_et.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    if (Create_Flag == "RENT") lblRent_et.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    if (Create_Flag == "DEPE") lblDepe_et.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    if (Create_Flag == "OTHE") lblOthe_et.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    if (Create_Flag == "ADDE") lblAdde_et.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                }
                else if (StatusTime == "CO")
                {
                    if (Create_Flag == "BOM")
                    {
                        if (ERRCode == "OK")
                        { lblBom_co.Text = "OK"; lblBom_co.ForeColor = Color.Blue; }
                        else
                        { lblBom_co.Text = "ERROR"; lblBom_co.ForeColor = Color.Red; }
                    }
                    if (Create_Flag == "RCPT")
                    {
                        if (ERRCode == "OK")
                        { lblRcpt_co.Text = "OK"; lblRcpt_co.ForeColor = Color.Blue; }
                        else
                        { lblRcpt_co.Text = "ERROR"; lblRcpt_co.ForeColor = Color.Red; }
                    }
                    if (Create_Flag == "ISSU")
                    {
                        if (ERRCode == "OK")
                        { lblCostInput_co.Text = "OK"; lblCostInput_co.ForeColor = Color.Blue; }
                        else
                        { lblCostInput_co.Text = "ERROR"; lblCostInput_co.ForeColor = Color.Red; }
                    }
                    if (Create_Flag == "IMPT")
                    {
                        if (ERRCode == "OK")
                        { lblCostImport_co.Text = "OK"; lblCostImport_co.ForeColor = Color.Blue; }
                        else
                        { lblCostImport_co.Text = "ERROR"; lblCostImport_co.ForeColor = Color.Red; }
                    }
                    if (Create_Flag == "EXCH")
                    {
                        if (ERRCode == "OK")
                        { lblExchange_co.Text = "OK"; lblExchange_co.ForeColor = Color.Blue; }
                        else
                        { lblExchange_co.Text = "ERROR"; lblExchange_co.ForeColor = Color.Red; }
                    }
                    if (Create_Flag == "PACK")
                    {
                        if (ERRCode == "OK")
                        { lblPacking_co.Text = "OK"; lblPacking_co.ForeColor = Color.Blue; }
                        else
                        { lblPacking_co.Text = "ERROR"; lblPacking_co.ForeColor = Color.Red; }
                    }
                    if (Create_Flag == "LABR")
                    {
                        if (ERRCode == "OK")
                        { lblLabor_co.Text = "OK"; lblLabor_co.ForeColor = Color.Blue; }
                        else
                        { lblLabor_co.Text = "ERROR"; lblLabor_co.ForeColor = Color.Red; }
                    }
                    if (Create_Flag == "DEPR")
                    {
                        if (ERRCode == "OK")
                        { lblDepr_co.Text = "OK"; lblDepr_co.ForeColor = Color.Blue; }
                        else
                        { lblDepr_co.Text = "ERROR"; lblDepr_co.ForeColor = Color.Red; }
                    }
                    if (Create_Flag == "RENT")
                    {
                        if (ERRCode == "OK")
                        { lblRent_co.Text = "OK"; lblRent_co.ForeColor = Color.Blue; }
                        else
                        { lblRent_co.Text = "ERROR"; lblRent_co.ForeColor = Color.Red; }
                    }
                    if (Create_Flag == "DEPE")
                    {
                        if (ERRCode == "OK")
                        { lblDepe_co.Text = "OK"; lblDepe_co.ForeColor = Color.Blue; }
                        else
                        { lblDepe_co.Text = "ERROR"; lblDepe_co.ForeColor = Color.Red; }
                    }
                    if (Create_Flag == "OTHE")
                    {
                        if (ERRCode == "OK")
                        { lblOthe_co.Text = "OK"; lblOthe_co.ForeColor = Color.Blue; }
                        else
                        { lblOthe_co.Text = "ERROR"; lblOthe_co.ForeColor = Color.Red; }
                    }
                    if (Create_Flag == "ADDE")
                    {
                        if (ERRCode == "OK")
                        { lblAdde_co.Text = "OK"; lblAdde_co.ForeColor = Color.Blue; }
                        else
                        { lblAdde_co.Text = "ERROR"; lblAdde_co.ForeColor = Color.Red; }
                    }

                }

                Application.DoEvents();
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region log버튼 
        private void BtnLog_MouseEnter(object sender, EventArgs e)
        {
            try
            {
                Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\Toolbar\uLog.gif");
                BtnLog.Image = bitMap;
            }
            catch (Exception f)
            {
            }
        }
        private void BtnLog_MouseLeave(object sender, EventArgs e)
        {
            try
            {
                Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\Toolbar\Log.gif");
                BtnLog.Image = bitMap;
            }
            catch (Exception f)
            {
            }
        } 
        private void BtnLog_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtM_PK_SEQ.Text == "") return;

                DAA002P99 pu = new DAA002P99(Convert.ToInt32(txtM_PK_SEQ.Text), cboH_MNUF_CODE.SelectedValue.ToString(), txtH_ORDR_YEAR.Text, cboM_DPRT_CODE.SelectedValue.ToString(), txtM_DCSN_NUMB.Text, txtM_CALC_DEGR.Text, this.Name);
                pu.Owner = this;
                pu.Show();
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion
    }
}
