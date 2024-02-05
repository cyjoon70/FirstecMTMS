#region DAA003 작성 정보
/*************************************************************/
// 단위업무명 : 원가자료 등록
// 작 성 자 :   유재규
// 작 성 일 :   2013-06-19
// 작성내용 :   
// 수 정 일 :
// 수 정 자 :
// 수정내용 :
// 비    고 : 원가연계기준정보 Upload및 29항목 테이블 Upload
// 참    고 : 
/*************************************************************/
#endregion

using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Data.OleDb;
using UIForm;
using System.IO;

namespace DA.DAA003
{
    public partial class DAA003 : UIForm.FPCOMM2
    {
        #region 변수선언
        string strORDR_YEAR = "";   //지시연도
        string strDCSN_NUMB = "";   //판단번호
        string strCALC_DEGR = "";   //차수
        string strDPRT_CODE = "";   //구매부서
        string strCTMP_CODE = "";   //조달업체
        string strMNUF_CODE = "";   //제출업체                
        string strSTD_YRMON = "";   //기준연월 
        string strNIIN = "";        //재고번호
        string strUNIT = "";        //단위
        string strDMST_ITNB = "";   //항목번호
        string strKeyGroup = "";    //키그룹
        string strESB_BIZNES_TRNSTN_ID = ""; //트랜잭션 ID
        int iESB_BIZNES_TRNSTN_SEQ = 0; //트랜잭션 ID
        int PreRow = -1;   // SelectionChanged 시에 동일 Row에서 데이타변환 처리 안하도록 하기 위함.
        #endregion

        #region DAA003
        public DAA003()
        {
            InitializeComponent();
        }
        #endregion

        #region DAA003Load
        private void DAA003Load(object sender, EventArgs e)
        {
            SystemBase.Validation.GroupBox_Reset(groupBox2);
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            SystemBase.Validation.GroupBox_Setting(groupBox2);

            //제출업체
            SystemBase.ComboMake.C1Combo(cboH_MNUF_CODE, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'D004', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'", 0);   //제출업체
            //제출업체
            SystemBase.ComboMake.C1Combo(cboM_MNUF_CODE, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'D004', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'", 0);   //제출업체
            //계약업체(조달업체)
            SystemBase.ComboMake.C1Combo(cboM_CTMP_CODE, "usp_B_COMMON @pTYPE='REL1', @pCODE = 'D006', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pSPEC1 = '" + cboH_MNUF_CODE.SelectedValue.ToString() + "'", 0);
            //계약업체(조달업체)
            SystemBase.ComboMake.C1Combo(cboH_CTMF_CODE, "usp_B_COMMON @pTYPE='REL1', @pCODE = 'D006', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pSPEC1 = '" + cboH_MNUF_CODE.SelectedValue.ToString() + "'", 9);
            

            //양산구분
            SystemBase.ComboMake.C1Combo(cboM_PRJCLS_DVS, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'D011', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'", 0);   //양산구분
            //원가계산적용수량기준
            SystemBase.ComboMake.C1Combo(cboM_CSTACC_APLY_QTY_STD, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'D022', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'", 0);   //원가계산적용수량기준
            //제출용도
            SystemBase.ComboMake.C1Combo(cboH_SBMTR_CHRG_PURPS, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'D008', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'", 3);   //제출용도
            //제출용도
            SystemBase.ComboMake.C1Combo(cboM_SBMTR_CHRG_PURPS, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'D008', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'");   //제출용도


            


            UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, true, 0, 0);

            //UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0, true);
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0);

            txtH_ORDR_YEAR.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("")).ToString().Substring(0, 4);
            cboH_SBMTR_CHRG_PURPS.SelectedValue = "2";

            BtnCreate_MouseLeave(null, null);

        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            try
            {
                SystemBase.Validation.GroupBox_Reset(groupBox2);
                SystemBase.Validation.GroupBox_Setting(groupBox1);
                SystemBase.Validation.GroupBox_Setting(groupBox2);

                txtH_ORDR_YEAR.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("")).ToString().Substring(0, 4);

                cboM_MNUF_CODE.Focus();
                ReSet();
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            try
            {
                SystemBase.Validation.GroupBox_Reset(groupBox2);
                SystemBase.Validation.GroupBox_Setting(groupBox2);

                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))  //필수조회조건 체크
                {
                    this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
                    string strSql = " usp_DAA003  ";
                    strSql += "  @pTYPE = 'S1'";
                    strSql += ", @pMNUF_CODE = '" + cboH_MNUF_CODE.SelectedValue + "' ";
                    strSql += ", @pSBMTR_CHRG_PURPS ='" + cboH_SBMTR_CHRG_PURPS.SelectedValue + "' ";
                    strSql += ", @pSTD_YRMON ='" + SystemBase.Validation.C1DataEdit_ReadFormat(dtH_STD_YRMON.Value.ToString(), "YYYYMM") + "' ";
                    strSql += ", @pORDR_YEAR ='" + txtH_ORDR_YEAR.Text + "' ";
                    strSql += ", @pDPRT_CODE ='" + txtH_DPRT_CODE.Text + "' ";
                    strSql += ", @pDCSN_NUMB ='" + txtH_DCSN_NUMB.Text + "' ";
                    strSql += ", @pCALC_DEGR ='" + txtH_CALC_DEGR.Text + "' ";
                    strSql += ", @pCTMF_CODE = '" + cboH_CTMF_CODE.SelectedValue + "'"; //조달업체

                    UIForm.FPMake.grdCommSheet(fpSpread2, strSql, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, true, 0, 0, true);
                    UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);

                    this.Cursor = System.Windows.Forms.Cursors.Default;
                }
                ReSet();
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Cursor = Cursors.Default;
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
                    string ERRCode = "ER", MSGCode = "SY001";	//처리할 내용이 없습니다.
                    SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                    SqlCommand cmd = dbConn.CreateCommand();
                    SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);
                    this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
                    try
                    {
                        string strSql = " usp_DAA003 ";
                        strSql += "  @pTYPE = 'I1'";
                        strSql += ", @pMNUF_CODE = '" + cboM_MNUF_CODE.SelectedValue + "'";                   //제출업체
                        strSql += ", @pSBMTR_CHRG_PURPS = '" + cboM_SBMTR_CHRG_PURPS.SelectedValue + "'";   //제출자료용도
                        strSql += ", @pSTD_YRMON ='" + SystemBase.Validation.C1DataEdit_ReadFormat(dtM_STD_YRMON.Value.ToString(), "YYYYMM") + "' ";  //기준연월
                        strSql += ", @pORDR_YEAR ='" + txtM_ORDR_YEAR.Text + "' ";  //요구년도
                        strSql += ", @pDPRT_CODE ='" + txtM_DPRT_CODE.Text + "' ";  //구매부서
                        strSql += ", @pDCSN_NUMB ='" + txtM_DCSN_NUMB.Text + "' ";  //판단번호
                        strSql += ", @pCALC_DEGR ='" + txtM_CALC_DEGR.Text + "' ";  //제출차수
                        strSql += ", @pCTMF_CODE ='" + cboM_CTMP_CODE.SelectedValue + "' ";  //계약업체코드
                        strSql += ", @pNIIN ='" + txtM_NIIN.Text + "' ";       //재고번호
                        strSql += ", @pUNIT ='" + txtM_UNIT.Text + "' ";       //단위
                        strSql += ", @pDMST_ITNB ='" + txtM_DMST_ITNB.Text + "' ";  //내자항목번호
                        strSql += ", @pRPST_ITNM ='" + txtM_RPST_ITNM.Text + "' ";  //재고품명
                        strSql += ", @pBOM_INFO_SBMT_STD_CNFMTN_HLNO ='" + numM_BOM_INFO_SBMT_STD_CNFMTN_HLNO.Text + "' "; //BOM정보제출기준확정호기
                        strSql += ", @pCSTACC_APLY_QTY =" + numM_CSTACC_APLY_QTY.Value + " ";                           //원가계산적용수량
                        strSql += ", @pPRJCLS_DVS ='" + cboM_PRJCLS_DVS.SelectedValue + "' ";                           //양산구분
                        strSql += ", @pCSTACC_APLY_QTY_STD ='" + cboM_CSTACC_APLY_QTY_STD.SelectedValue + "' ";         //원가계산적용수량기준
                        strSql += ", @pNOTE ='" + txtM_NOTE.Text + "' ";                                                //비고
                        strSql += ", @pUSR_ID ='" + SystemBase.Base.gstrUserID + "' ";                                  //사용자
                        strSql += ", @pESB_BIZNES_TRNSTN_ID ='" + txtM_ESB_BIZNES_TRNSTN_ID.Text + "' ";                //트랜잭션 ID
                        strSql += ", @pESB_BIZNES_TRNSTN_SEQ ='" + txtM_ESB_BIZNES_TRNSTN_SEQ.Text + "' ";              //트랜잭션 순번


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
                    else
                    {
                        MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
        }
        #endregion

        #region DelExec() 삭제 로직
        protected override void DeleteExec()
        {
            //해당 전체자료 삭제

            string msg = "주의!!!!!!. \n\n방사청 포털에서 자료제출 취소 후에만 삭제를 실행 하세요!. \n\n이미 제출된 경우에는 절대 삭제하지 마세요!\n\n자료를 삭제 하시겠습니까?";

            DialogResult dsMsg = MessageBox.Show(msg, SystemBase.Base.MessageRtn("Z0004"), MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dsMsg == DialogResult.Yes)
            {
                string ERRCode = "ER", MSGCode = "SY001";	//처리할 내용이 없습니다.
                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);
                this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

                try
                {
                    string strSql = " usp_DAA003  'D1'";
                    strSql += ", @pESB_BIZNES_TRNSTN_ID = '" + strESB_BIZNES_TRNSTN_ID + "'";
                    strSql += ", @pESB_BIZNES_TRNSTN_SEQ = '" + iESB_BIZNES_TRNSTN_SEQ + "'";
                    strSql += ", @pKEY_GROUP = '" + strKeyGroup + "' ";

                    DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                    ERRCode = ds.Tables[0].Rows[0][0].ToString();
                    MSGCode = ds.Tables[0].Rows[0][1].ToString();

                    if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                    Trans.Commit();
                    NewExec();
                    ReSet();
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    Trans.Rollback();
                    MSGCode = "SY002";	//에러가 발생하여 데이터 처리가 취소되었습니다.
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
                else
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }
        #endregion

        #region 구매부서 버튼 팝업
        private void btnH_DPRT_POPUP_Click(object sender, EventArgs e)
        {
            try
            {
                Dprt_Popup("Y", "H");  // 버튼클릭, 헤더위치
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnM_DPRT_POPUP_Click(object sender, EventArgs e)
        {
            try
            {
                Dprt_Popup("Y", "M"); // 버튼클릭, 마스터위치
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 단위 버튼 팝업
        private void btnM_UNIT_POPUP_Click(object sender, EventArgs e)
        {
            try
            {
                UNIT_Popup("Y", "M");
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 구매부서코드 변환시  구매부서명 조회
        private void txtH_DPRT_CODE_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string strSql = "AND MAJOR_CD = 'D007' AND COMP_CODE = '" + SystemBase.Base.gstrCOMCD + "'";
                txtH_DPRT_NAME.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtH_DPRT_CODE.Text, strSql);
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtM_DPRT_CODE_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string strSql = "AND MAJOR_CD = 'D007' AND COMP_CODE = '" + SystemBase.Base.gstrCOMCD + "'";
                txtM_DPRT_NAME.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtM_DPRT_CODE.Text, strSql);
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 단위코드 변환시  단위코드명 조회
        private void txtM_UNIT_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string strSql = "AND MAJOR_CD = 'Z005' AND COMP_CODE = '" + SystemBase.Base.gstrCOMCD + "'";
                txtM_UNIT_NM.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtM_UNIT.Text, strSql);
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 좌측그리드 방향키 이동 및 클릭시 우측조회
        private void fpSpread2_SelectionChanged(object sender, FarPoint.Win.Spread.SelectionChangedEventArgs e)
        {
            if (fpSpread2.Sheets[0].Rows.Count > 0)
            {
                int intRow = fpSpread2.ActiveSheet.GetSelection(0).Row;
                if (intRow < 0) return;
                if (PreRow == intRow && PreRow != -1) return;  //현 Row에서 컬럼이동시는 조회 안되게

                /******************************************************* KEY그룹 생성 **********************************************************************************/
                strORDR_YEAR = fpSpread2.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx2, "지시연도")].Text.ToString();      //지시연도
                strDCSN_NUMB = fpSpread2.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx2, "판단번호")].Text.ToString();      //판단번호
                strCALC_DEGR = fpSpread2.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx2, "차수")].Text.ToString();          //차수
                strDPRT_CODE = fpSpread2.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx2, "구매부서")].Text.ToString();      //구매부서
                strCTMP_CODE = fpSpread2.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx2, "조달업체")].Text.ToString();      //조달업체
                strMNUF_CODE = fpSpread2.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx2, "제출업체")].Text.ToString();      //제출업체                
                strSTD_YRMON = fpSpread2.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx2, "기준연월")].Text.ToString();      //기준연월 
                strNIIN = fpSpread2.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx2, "재고번호")].Text.ToString();           //재고번호
                strUNIT = fpSpread2.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx2, "단위")].Text.ToString();               //단위
                strDMST_ITNB = fpSpread2.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx2, "항목")].Text.ToString();          //항목번호

                strKeyGroup = strORDR_YEAR.Trim() + strDCSN_NUMB.Trim() + strCALC_DEGR.Trim() + strDPRT_CODE.Trim() + strCTMP_CODE.Trim();
                strKeyGroup += strMNUF_CODE.Trim() + strSTD_YRMON.Trim() + strNIIN.Trim() + strUNIT.Trim() + strDMST_ITNB.Trim();
                /*********************************************************************************************************************************************************/

                strESB_BIZNES_TRNSTN_ID = fpSpread2.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx2, "트랜잭션ID")].Text.ToString(); //트랜잭션 ID
                iESB_BIZNES_TRNSTN_SEQ = Convert.ToInt32(fpSpread2.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx2, "트랜잭션순번")].Text.ToString()); //트랜잭션 ID

                SystemBase.Validation.GroupBox_Reset(groupBox2);
                SystemBase.Validation.GroupBox_Setting(groupBox2);

                this.Cursor = Cursors.WaitCursor;
                try
                {
                    string strSql = " usp_DAA003 ";
                    strSql += "  @pTYPE = 'S2' ";
                    strSql += ", @pESB_BIZNES_TRNSTN_ID = '" + strESB_BIZNES_TRNSTN_ID + "'";
                    strSql += ", @pESB_BIZNES_TRNSTN_SEQ = " + iESB_BIZNES_TRNSTN_SEQ + " ";
                    strSql += ", @pKEY_GROUP = '" + strKeyGroup + "' ";

                    DataSet ds = SystemBase.DbOpen.NoTranDataSet(strSql);
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        cboM_MNUF_CODE.SelectedValue = ds.Tables[0].Rows[0]["MNUF_CODE"].ToString();    // 제출업체코드
                        txtM_ORDR_YEAR.Value = ds.Tables[0].Rows[0]["ORDR_YEAR"].ToString();            // 요구년도
                        txtM_DCSN_NUMB.Value = ds.Tables[0].Rows[0]["DCSN_NUMB"].ToString();            // 판단번호
                        dtM_STD_YRMON.Value = SystemBase.Validation.C1DataEdit_WriteFormat(ds.Tables[0].Rows[0]["STD_YRMON"].ToString(), "YYYY-MM"); //기준연월                             
                        txtM_CALC_DEGR.Value = ds.Tables[0].Rows[0]["CALC_DEGR"].ToString();            // 차수
                        txtM_DPRT_CODE.Value = ds.Tables[0].Rows[0]["DPRT_CODE"].ToString();            // 구매부서
                        cboM_CTMP_CODE.SelectedValue = ds.Tables[0].Rows[0]["CTMF_CODE"].ToString();    // 계약업체코드
                        txtM_NIIN.Value = ds.Tables[0].Rows[0]["NIIN"].ToString();            // 재고번호
                        txtM_UNIT.Value = ds.Tables[0].Rows[0]["UNIT"].ToString();            // 단위
                        txtM_DMST_ITNB.Value = ds.Tables[0].Rows[0]["DMST_ITNB"].ToString();            // 내자항목번호
                        txtM_RPST_ITNM.Value = ds.Tables[0].Rows[0]["RPST_ITNM"].ToString();            // 재고품명
                        numM_BOM_INFO_SBMT_STD_CNFMTN_HLNO.Value = ds.Tables[0].Rows[0]["BOM_INFO_SBMT_STD_CNFMTN_HLNO"].ToString();  // BOM정보제출기준확정호기
                        numM_CSTACC_APLY_QTY.Value = ds.Tables[0].Rows[0]["CSTACC_APLY_QTY"].ToString();            // 원가계산 적용수량
                        cboM_CSTACC_APLY_QTY_STD.SelectedValue = ds.Tables[0].Rows[0]["CSTACC_APLY_QTY_STD"].ToString();   // 원가계산적용수량기준
                        cboM_SBMTR_CHRG_PURPS.SelectedValue = ds.Tables[0].Rows[0]["SBMTR_CHRG_PURPS"].ToString();   // 제출자료용도
                        //cboM_PRJCLS_DVS.SelectedValue = ds.Tables[0].Rows[0]["PRJCLS_DVS"].ToString();   // 양산구분
                        txtM_NOTE.Value = ds.Tables[0].Rows[0]["NOTE"].ToString();            // 비고
                        txtM_ESB_BIZNES_TRNSTN_ID.Value = ds.Tables[0].Rows[0]["ESB_BIZNES_TRNSTN_ID"].ToString();    // 트랜잭션 ID
                        txtM_ESB_BIZNES_TRNSTN_SEQ.Value = ds.Tables[0].Rows[0]["ESB_BIZNES_TRNSTN_SEQ"].ToString();  // 트랜잭션 순번	                       
                    }

                    //Detail Data
                    string strSql2 = " usp_DAA003 ";
                    strSql2 += "  @pTYPE = 'S3' ";
                    strSql2 += ", @pESB_BIZNES_TRNSTN_ID = '" + strESB_BIZNES_TRNSTN_ID + "'";
                    strSql2 += ", @pKEY_GROUP = '" + strKeyGroup + "' ";
                    strSql2 += ", @pMNUF_CODE = '" + strMNUF_CODE + "' ";
                    UIForm.FPMake.grdCommSheet(fpSpread1, strSql2, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
                    fpSpread1.ActiveSheet.SetColumnMerge(SystemBase.Base.GridHeadIndex(GHIdx1, "구분"), FarPoint.Win.Spread.Model.MergePolicy.Always);

                    //오류자료
                    string strSql3 = " usp_DAA003 ";
                    strSql3 += "  @pTYPE = 'S4' ";
                    strSql3 += ", @pKEY_GROUP = '" + strKeyGroup + "' ";
                    DataSet ds2 = SystemBase.DbOpen.NoTranDataSet(strSql3);
                    if (ds2.Tables[0].Rows.Count > 0)
                    {
                        btnM_ERROR_VIEW.Enabled = true;
                        btnM_ERROR_VIEW.ForeColor = Color.Red;
                    }
                    else
                    {
                        btnM_ERROR_VIEW.Enabled = false;
                        btnM_ERROR_VIEW.ForeColor = Color.Black;
                    }

                    SystemBase.Validation.GroupBox_SearchViewValidation(groupBox2); //Key값 컨트롤 세팅
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("SY013"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Cursor = Cursors.Default;
                }


                SystemBase.Validation.GroupBox_SearchViewValidation(groupBox2);
                this.Cursor = Cursors.Default;
                PreRow = fpSpread2.ActiveSheet.GetSelection(0).Row;
            }
        }
        #endregion

        #region Excel Upload
        //제출기준정보
        private void btnM_EXCEL1_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog dlg = new OpenFileDialog();
                dlg.Filter = "통합 Excel 문서(*.xls)|*.xls|2007 Excel 문서(*.xlsx)|*.xls|2010 Excel 문서(*.xlsx)|*.xlsx";

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    SystemBase.DbOpen.Eecle_Connet(dlg.FileName);
                    using (OleDbConnection connection = new OleDbConnection(SystemBase.Base.gstrExcelConn))
                    {
                        connection.Open();
                        DataTable worksheets = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });

                        /* 엑셀 쉬트별 저장 */
                        DataSet Ds = null;
                        int BlankRow = 0;
                        string strSql = "";

                        this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

                        OleDbCommand command = new OleDbCommand(string.Format("SELECT * FROM [제출자료기준정보(v4.0)$]"), connection);
                        OleDbDataAdapter adapter = new OleDbDataAdapter(command);
                        adapter.SelectCommand = command;
                        Ds = new DataSet();
                        adapter.Fill(Ds);

                        /************  공통저장 체크 ***********************************************************************/
                        /* Data Line : 6 라인 부터 시작 ----->  if (j > 4) */
                        /* Trans 처리 하지 않는다. 오류나는 데이타도 그대로 저장시킴. 추후 검증로직에서 걸러냄....
                         * 순번이 5 Row 이상 비어 있으면 해당 쉬트 종료 처리...
                         * 엑셀 UPLOAD 프로시저는 단위당 하나씩 작성...
                        /**************************************************************************************************/

                        //#region 로그파일삭제
                        //// 제출기준정보의 제출업체와 기준연월 정보
                        //string strSql = " usp_DAA003제출자료기준정보 ";
                        //strSql += "  @pTYPE = 'D1'";
                        //strSql += ", @pMNUF_CODE = '" + cboM_MNUF_CODE.SelectedValue + "'";  //제출업체
                        //strSql += ", @pSTD_YRMON ='" + SystemBase.Validation.C1DataEdit_ReadFormat(dtM_STD_YRMON.Value.ToString(), "YYYYMM") + "' ";  //기준연월
                        //strSql += ", @pUSR_ID ='" + SystemBase.Base.gstrUserID + "' ";                    //사용자

                        //Ds = SystemBase.DbOpen.NoTranDataSet(strSql);
                        //#endregion


                        #region 제출자료기준정보(v4.0)
                        for (int j = 0; j < Ds.Tables[0].Rows.Count; j++)
                        {
                            if (j > 4 && Ds.Tables[0].Rows[j][0].ToString().Trim() != "")
                            {
                                strSql = "";
                                strSql += " usp_DAA003제출자료기준정보 ";
                                strSql += "  @pTYPE = 'I1'";
                                strSql += ", @pREGE_SNUM ='" + Ds.Tables[0].Rows[j][0].ToString().Trim() + "' ";  //등록순번
                                strSql += ", @pORDR_YEAR ='" + Ds.Tables[0].Rows[j][1].ToString().Trim() + "' ";  //요구년도
                                strSql += ", @pDCSN_NUMB ='" + Ds.Tables[0].Rows[j][2].ToString().Trim() + "' ";  //판단번호
                                strSql += ", @pCALC_DEGR ='" + Ds.Tables[0].Rows[j][3].ToString().Trim() + "' ";  //제출차수
                                strSql += ", @pDPRT_CODE ='" + Ds.Tables[0].Rows[j][4].ToString().Trim() + "' ";  //구매부서
                                strSql += ", @pCTMF_CODE ='" + Ds.Tables[0].Rows[j][5].ToString().Trim() + "' ";  //계약업체코드
                                strSql += ", @pMNUF_CODE = '" + Ds.Tables[0].Rows[j][6].ToString().Trim() + "'";  //제출업체
                                strSql += ", @pSTD_YRMON ='" + Ds.Tables[0].Rows[j][7].ToString().Trim() + "' ";  //기준연월
                                strSql += ", @pNIIN ='" + Ds.Tables[0].Rows[j][8].ToString().Trim() + "' ";       //재고번호
                                strSql += ", @pUNIT ='" + Ds.Tables[0].Rows[j][9].ToString().Trim() + "' ";       //단위
                                strSql += ", @pDMST_ITNB ='" + Ds.Tables[0].Rows[j][10].ToString().Trim() + "' ";  //내자항목번호
                                strSql += ", @pRPST_ITNM ='" + Ds.Tables[0].Rows[j][11].ToString().Trim() + "' ";  //재고품명
                                strSql += ", @pSBMTR_CHRG_PURPS = '" + Ds.Tables[0].Rows[j][12].ToString().Trim() + "'";   //제출자료용도
                                strSql += ", @pBOM_INFO_SBMT_STD_CNFMTN_HLNO =" + SystemBase.Validation.Decimal_Data(Ds.Tables[0].Rows[j][13].ToString().Trim(), ",") + " "; //BOM정보제출기준확정호기
                                strSql += ", @pCSTACC_APLY_QTY =" + SystemBase.Validation.Decimal_Data(Ds.Tables[0].Rows[j][14].ToString().Trim(), ",") + " "; //원가계산적용수량
                                strSql += ", @pCSTACC_APLY_QTY_STD ='" + Ds.Tables[0].Rows[j][15].ToString().Trim() + "' ";         //원가계산적용수량기준
                                strSql += ", @pPRJCLS_DVS ='" + Ds.Tables[0].Rows[j][16].ToString().Trim() + "' ";                  //양산구분                                            
                                strSql += ", @pNOTE ='" + Ds.Tables[0].Rows[j][17].ToString().Trim() + "' ";                        //비고
                                strSql += ", @pUSR_ID ='" + SystemBase.Base.gstrUserID + "' ";                                  //사용자

                                DataSet ds = SystemBase.DbOpen.NoTranDataSet(strSql);
                            }

                            if (Ds.Tables[0].Rows[j][0].ToString().Trim() == "") BlankRow++;
                            if (BlankRow >= 5) j = Ds.Tables[0].Rows.Count;

                        }
                        adapter.Dispose();
                        command.Dispose();

                        //업로드 처리 완료 하였습니다.
                        MessageBox.Show(SystemBase.Base.MessageRtn("SY029"), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                        SearchExec();
                        connection.Close();
                        this.Cursor = System.Windows.Forms.Cursors.Default;
                        #endregion
                    }
                }
            }
            catch (Exception f)
            {
                this.Cursor = Cursors.Default;
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(f.Message, SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        //원가자료
        private void btnM_EXCEL2_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtM_ESB_BIZNES_TRNSTN_ID.Text == "")  //선택된 자료가 없으면 Upload 처리 안함.
                {
                    //선택된 내용이 없습니다.
                    MessageBox.Show(SystemBase.Base.MessageRtn("SY028"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                OpenFileDialog dlg = new OpenFileDialog();
                dlg.Filter = "통합 Excel 문서(*.xls)|*.xls|2007 Excel 문서(*.xlsx)|*.xls|2010 Excel 문서(*.xlsx)|*.xlsx";

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    SystemBase.DbOpen.Eecle_Connet(dlg.FileName);
                    using (OleDbConnection connection = new OleDbConnection(SystemBase.Base.gstrExcelConn))
                    {
                        connection.Open();
                        DataTable worksheets = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });

                        /* 엑셀 쉬트별 저장 */
                        string strSql2 = "";
                        DataSet Ds = null;
                        DataSet Ds2 = null;
                        OleDbCommand command = null;
                        OleDbDataAdapter adapter = null;
                        int BlankRow = 0;
                        this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

                        /************  공통저장 체크 *****************************************************************************/
                        /* Data Line : 6 라인 부터 시작 ----->  if (j > 4) 
                         * Trans 처리 하지 않는다. 오류나는 데이타도 그대로 저장시킴. 추후 검증로직에서 걸러냄...UPLOAD ERROR 화면 조회
                         * 순번이 5 Row 이상 비어 있으면 해당 쉬트 종료 처리...
                         * 엑셀 UPLOAD 프로시저는 단위당 하나씩 작성...
                        /*******************************************************************************************************/

                        #region 1.BOM정보
                        command = new OleDbCommand(string.Format("SELECT * FROM [BOM정보$]"), connection);
                        adapter = new OleDbDataAdapter(command);
                        adapter.SelectCommand = command;
                        Ds = new DataSet();
                        adapter.Fill(Ds);
                        for (int j = 0; j < Ds.Tables[0].Rows.Count; j++)
                        {
                            if (j > 4 && Ds.Tables[0].Rows[j][0].ToString().Trim() != "")
                            {
                                string strSql = " usp_DAA003BOM ";
                                strSql += "  @pTYPE = 'I1'";
                                strSql += ", @pREGE_SNUM ='" + Ds.Tables[0].Rows[j][0].ToString().Trim() + "' ";  //등록순번
                                strSql += ", @pORDR_YEAR ='" + Ds.Tables[0].Rows[j][1].ToString().Trim() + "' ";  //요구년도
                                strSql += ", @pDCSN_NUMB ='" + Ds.Tables[0].Rows[j][2].ToString().Trim() + "' ";  //판단번호
                                strSql += ", @pCALC_DEGR ='" + Ds.Tables[0].Rows[j][3].ToString().Trim() + "' ";  //제출차수
                                strSql += ", @pDPRT_CODE ='" + Ds.Tables[0].Rows[j][4].ToString().Trim() + "' ";  //구매부서
                                strSql += ", @pCTMF_CODE ='" + Ds.Tables[0].Rows[j][5].ToString().Trim() + "' ";  //계약업체코드
                                strSql += ", @pMNUF_CODE = '" + Ds.Tables[0].Rows[j][6].ToString().Trim() + "'";                   //제출업체
                                strSql += ", @pSTD_YRMON ='" + Ds.Tables[0].Rows[j][7].ToString().Trim() + "' ";  //기준연월
                                strSql += ", @pNIIN ='" + Ds.Tables[0].Rows[j][8].ToString().Trim() + "' ";       //재고번호
                                strSql += ", @pUNIT ='" + Ds.Tables[0].Rows[j][9].ToString().Trim() + "' ";       //단위
                                strSql += ", @pDMST_ITNB ='" + Ds.Tables[0].Rows[j][10].ToString().Trim() + "' ";  //내자항목번호

                                strSql += ", @pBOM_LEVL ='" + Ds.Tables[0].Rows[j][11].ToString().Trim() + "' ";  //BOM레벨
                                strSql += ", @pHGRNK_CMPNTS_MGTNO ='" + Ds.Tables[0].Rows[j][12].ToString().Trim() + "' ";  //상위부품관리번호
                                strSql += ", @pMTMG_NUMB ='" + Ds.Tables[0].Rows[j][13].ToString().Trim() + "' ";  //부품관리번호
                                strSql += ", @pPART_NAME ='" + SystemBase.Validation.String_Data(Ds.Tables[0].Rows[j][14].ToString().Trim(), "'") + "' ";  //품명
                                strSql += ", @pMNUF_PANO ='" + Ds.Tables[0].Rows[j][15].ToString().Trim() + "' ";  //업체품번
                                strSql += ", @pREPG_ENNO ='" + Ds.Tables[0].Rows[j][16].ToString().Trim() + "' ";  //대체품업체품번
                                strSql += ", @pITKD_PUDV ='" + Ds.Tables[0].Rows[j][17].ToString().Trim() + "' ";  //품목구입선구분
                                strSql += ", @pWBS_TYPE_DVS ='" + Ds.Tables[0].Rows[j][18].ToString().Trim() + "' ";  //WBS 타입
                                strSql += ", @pDNNP_APPN ='" + Ds.Tables[0].Rows[j][19].ToString().Trim() + "' ";  //방산물자지정여부
                                strSql += ", @pPART_UNIT ='" + Ds.Tables[0].Rows[j][20].ToString().Trim() + "' ";  //단위
                                strSql += ", @pCMPT_REQR ='" + SystemBase.Validation.Decimal_Data(Ds.Tables[0].Rows[j][21].ToString().Trim(), ",") + "' ";  //BOM수량 단위당
                                strSql += ", @pCPST_QNTY ='" + SystemBase.Validation.Decimal_Data(Ds.Tables[0].Rows[j][22].ToString().Trim(), ",") + "' ";  //BOM수량 구성수량
                                strSql += ", @pMATR_PRDV ='" + Ds.Tables[0].Rows[j][23].ToString().Trim() + "' ";  //소재구입선(소재정보)
                                strSql += ", @pMATR_CNUN ='" + Ds.Tables[0].Rows[j][24].ToString().Trim() + "' ";  //환산단위(소재정보)
                                strSql += ", @pMATR_CNQY ='" + SystemBase.Validation.Decimal_Data(Ds.Tables[0].Rows[j][25].ToString().Trim(), ",") + "' ";  //환산수량(소재정보)
                                strSql += ", @pMATR_URWG ='" + SystemBase.Validation.Decimal_Data(Ds.Tables[0].Rows[j][26].ToString().Trim(), ",") + "' ";  //실제품량(소재단위당소요량)
                                strSql += ", @pMATR_UEAT ='" + Ds.Tables[0].Rows[j][27].ToString().Trim() + "' ";  //작업설물(손재단위당소요량 - 필드자동계산)
                                strSql += ", @pLOSS_RATE ='" + SystemBase.Validation.Decimal_Data(Ds.Tables[0].Rows[j][28].ToString().Trim(), ",") + "' ";  //손실률(제출감손율)
                                strSql += ", @pIFRR_RATE ='" + SystemBase.Validation.Decimal_Data(Ds.Tables[0].Rows[j][29].ToString().Trim(), ",") + "' ";  //불량률(제출감손율)
                                strSql += ", @pSAMP_ORTE ='" + SystemBase.Validation.Decimal_Data(Ds.Tables[0].Rows[j][30].ToString().Trim(), ",") + "' ";  //시료율(제출감손율)
                                strSql += ", @pREQR_QNTY ='" + Ds.Tables[0].Rows[j][31].ToString().Trim() + "' ";  //소요량(제품단위당소요량 - 필드자동계산)
                                strSql += ", @pPART_REQR ='" + Ds.Tables[0].Rows[j][32].ToString().Trim() + "' ";  //실제품량(제품단위당소요량 - 필드자동계산)
                                strSql += ", @pETAM_QUTY ='" + Ds.Tables[0].Rows[j][33].ToString().Trim() + "' ";  //작업설물(제품단위당소요량 - 필드자동계산)
                                strSql += ", @pWDRL_RATE ='" + SystemBase.Validation.Decimal_Data(Ds.Tables[0].Rows[j][34].ToString().Trim(), "%") + "' ";  //설물회수율(제품단위당소요량 )
                                strSql += ", @pETCX_UPRC ='" + SystemBase.Validation.Decimal_Data(Ds.Tables[0].Rows[j][35].ToString().Trim(), ",") + "' ";  //설물단가원화(제품단위당소요량 )
                                strSql += ", @pAPST_NBMT ='" + Ds.Tables[0].Rows[j][36].ToString().Trim() + "' ";  //시작호기(적용호기)
                                strSql += ", @pAPFN_NBMT ='" + Ds.Tables[0].Rows[j][37].ToString().Trim() + "' ";  //종료호기(적용호기)
                                strSql += ", @pOUTSC_ENTPRZ_BIZPSN_NO ='" + Ds.Tables[0].Rows[j][39].ToString().Trim() + "' ";  //외주업체 사업자번호
                                strSql += ", @pCTRMTHD ='" + Ds.Tables[0].Rows[j][40].ToString().Trim() + "' ";         //계약방법	

                                strSql += ", @pUSR_ID ='" + SystemBase.Base.gstrUserID + "' ";                                   //사용자

                                DataSet ds = SystemBase.DbOpen.NoTranDataSet(strSql);
                            }

                            if (Ds.Tables[0].Rows[j][0].ToString().Trim() == "") BlankRow++;
                            if (BlankRow >= 5) j = Ds.Tables[0].Rows.Count;

                        }
                        adapter.Dispose();
                        command.Dispose();
                        #endregion

                        #region 2.입고이력및외주단가정보
                        BlankRow = 0;
                        command = new OleDbCommand(string.Format("SELECT * FROM [입고이력및외주단가$]"), connection);
                        adapter = new OleDbDataAdapter(command);
                        adapter.SelectCommand = command;
                        Ds = new DataSet();
                        adapter.Fill(Ds);
                        for (int j = 0; j < Ds.Tables[0].Rows.Count; j++)
                        {
                            if (j > 4 && Ds.Tables[0].Rows[j][0].ToString().Trim() != "")
                            {
                                string strSql = " usp_DAA003입고이력및외주단가 ";
                                strSql += "  @pTYPE = 'I1'";
                                strSql += ", @pREGE_SNUM ='" + Ds.Tables[0].Rows[j][0].ToString().Trim() + "' ";  //등록순번
                                strSql += ", @pORDR_YEAR ='" + Ds.Tables[0].Rows[j][1].ToString().Trim() + "' ";  //요구년도
                                strSql += ", @pDCSN_NUMB ='" + Ds.Tables[0].Rows[j][2].ToString().Trim() + "' ";  //판단번호
                                strSql += ", @pCALC_DEGR ='" + Ds.Tables[0].Rows[j][3].ToString().Trim() + "' ";  //제출차수
                                strSql += ", @pDPRT_CODE ='" + Ds.Tables[0].Rows[j][4].ToString().Trim() + "' ";  //구매부서
                                strSql += ", @pCTMF_CODE ='" + Ds.Tables[0].Rows[j][5].ToString().Trim() + "' ";  //계약업체코드
                                strSql += ", @pMNUF_CODE = '" + Ds.Tables[0].Rows[j][6].ToString().Trim() + "'";                   //제출업체
                                strSql += ", @pSTD_YRMON ='" + Ds.Tables[0].Rows[j][7].ToString().Trim() + "' ";  //기준연월
                                strSql += ", @pNIIN ='" + Ds.Tables[0].Rows[j][8].ToString().Trim() + "' ";       //재고번호
                                strSql += ", @pUNIT ='" + Ds.Tables[0].Rows[j][9].ToString().Trim() + "' ";       //단위
                                strSql += ", @pDMST_ITNB ='" + Ds.Tables[0].Rows[j][10].ToString().Trim() + "' ";  //내자항목번호

                                strSql += ", @pMTMG_NUMB ='" + Ds.Tables[0].Rows[j][11].ToString().Trim() + "' ";  //부품관리번호
                                strSql += ", @pMNUF_PANO ='" + Ds.Tables[0].Rows[j][12].ToString().Trim() + "' ";  //업체품번
                                strSql += ", @pMNUF_ITNM ='" + SystemBase.Validation.String_Data(Ds.Tables[0].Rows[j][13].ToString().Trim(), "'") + "' ";  //업체품명
                                strSql += ", @pITKD_PUDV ='" + Ds.Tables[0].Rows[j][14].ToString().Trim() + "' ";  //품목/구입선구분
                                strSql += ", @pMTRL_DIVS ='" + Ds.Tables[0].Rows[j][15].ToString().Trim() + "' ";  //자재구분
                                strSql += ", @pFCTR_CODE ='" + Ds.Tables[0].Rows[j][16].ToString().Trim() + "' ";  //공장코드
                                strSql += ", @pFCTR_NAME ='" + Ds.Tables[0].Rows[j][17].ToString().Trim() + "' ";  //공장명
                                strSql += ", @pWRHO_DATE ='" + Ds.Tables[0].Rows[j][18].ToString().Trim() + "' ";  //입고일
                                strSql += ", @pWRHO_UNIT ='" + Ds.Tables[0].Rows[j][19].ToString().Trim() + "' ";  //입고단위
                                strSql += ", @pQNTY =" + SystemBase.Validation.Decimal_Data(Ds.Tables[0].Rows[j][20].ToString().Trim(), ",") + " ";  //수량
                                strSql += ", @pCURC_UNIT ='" + Ds.Tables[0].Rows[j][21].ToString().Trim() + "' ";  //통화코드
                                strSql += ", @pUNIT_PRCE =" + SystemBase.Validation.Decimal_Data(Ds.Tables[0].Rows[j][22].ToString().Trim(), ",") + " ";  //단가
                                //strSql += ", @pWONX_AMNT ='" + Ds.Tables[0].Rows[j][23].ToString().Trim() + "' ";  //금액 (필드자동계산)
                                strSql += ", @pSUEN_NUMB ='" + Ds.Tables[0].Rows[j][24].ToString().Trim() + "' ";  //공급업체명
                                strSql += ", @pSRC_PRDCR_NM ='" + Ds.Tables[0].Rows[j][25].ToString().Trim() + "' ";  //원생산자명
                                strSql += ", @pNOTE ='" + Ds.Tables[0].Rows[j][26].ToString().Trim() + "' ";  //입고여부(비고)
                                strSql += ", @pTXBL_CONO ='" + Ds.Tables[0].Rows[j][27].ToString().Trim() + "' ";  //세금계산서승인번호
                                strSql += ", @pSUEN_BINO ='" + Ds.Tables[0].Rows[j][28].ToString().Trim() + "' ";  //공급업체사업자번호
                                strSql += ", @pTOTAL_AMTSPL =" + SystemBase.Validation.Decimal_Data(Ds.Tables[0].Rows[j][29].ToString().Trim(), ",") + " ";  //총공급가액
                                strSql += ", @pTXBL_SEQS =" + SystemBase.Validation.Decimal_Data(Ds.Tables[0].Rows[j][30].ToString().Trim(), ",") + " ";  //세금계산서품목순번
                                strSql += ", @pITMNM ='" + Ds.Tables[0].Rows[j][31].ToString().Trim() + "' ";  //세금계산서품목
                                strSql += ", @pAMTSPL =" + SystemBase.Validation.Decimal_Data(Ds.Tables[0].Rows[j][32].ToString().Trim(), ",") + " ";  //품목별공급가액
                                strSql += ", @pTXINV_CMPNTS_CD ='" + Ds.Tables[0].Rows[j][33].ToString().Trim() + "' ";  //부품번호(세금계산서비고)
                                strSql += ", @pBLXX_NUMB ='" + Ds.Tables[0].Rows[j][34].ToString().Trim() + "' ";  //BL NO(AWB) No
                                strSql += ", @pLCXX_NUMB ='" + Ds.Tables[0].Rows[j][35].ToString().Trim() + "' ";  //LC NO
                                strSql += ", @pIMPO_LICE ='" + Ds.Tables[0].Rows[j][36].ToString().Trim() + "' ";  //수입신고필증
                                strSql += ", @pTKIN_DATE ='" + Ds.Tables[0].Rows[j][37].ToString().Trim() + "' ";  //반입일
                                strSql += ", @pTRSPRT_DVS ='" + Ds.Tables[0].Rows[j][38].ToString().Trim() + "' ";  //운송형태 운송수단
                                strSql += ", @pDPTCTR ='" + Ds.Tables[0].Rows[j][39].ToString().Trim() + "' ";  //적출국
                                strSql += ", @pFOBX_DIVS ='" + Ds.Tables[0].Rows[j][40].ToString().Trim() + "' ";  //인도조건
                                strSql += ", @pAPRV_AMTMN =" + SystemBase.Validation.Decimal_Data(Ds.Tables[0].Rows[j][41].ToString().Trim(), ",") + " ";  //신고필증품목결재금액
                                strSql += ", @pAPRV_METHD ='" + Ds.Tables[0].Rows[j][42].ToString().Trim() + "' ";  //결재방법
                                strSql += ", @pCLMN_NUMB =" + SystemBase.Validation.Decimal_Data(Ds.Tables[0].Rows[j][43].ToString().Trim(), ",") + " ";  //란번호
                                strSql += ", @pSPEC_NUMB =" + SystemBase.Validation.Decimal_Data(Ds.Tables[0].Rows[j][44].ToString().Trim(), ",") + " ";  //규격일련번호
                                strSql += ", @pSPEC_NM ='" + Ds.Tables[0].Rows[j][45].ToString().Trim() + "' ";  //규격명
                                strSql += ", @pQTY =" + SystemBase.Validation.Decimal_Data(Ds.Tables[0].Rows[j][46].ToString().Trim(), ",") + " ";  //신고필증품목수량
                                strSql += ", @pHSXX_NUMB ='" + SystemBase.Validation.Decimal_Data(Ds.Tables[0].Rows[j][47].ToString().Trim(), ",") + "' ";  //세번부호(HS 코드)
                                strSql += ", @pTRFRT =" + SystemBase.Validation.Decimal_Data(Ds.Tables[0].Rows[j][48].ToString().Trim(), "%") + " ";  //세종=관 관세율
                                strSql += ", @pRXP_RTE =" + SystemBase.Validation.Decimal_Data(Ds.Tables[0].Rows[j][49].ToString().Trim(), "%") + " ";  //세종=관 감면율
                                strSql += ", @pAGRI_RATE =" + SystemBase.Validation.Decimal_Data(Ds.Tables[0].Rows[j][50].ToString().Trim(), "%") + " ";  //농특세율(%)
                                strSql += ", @pINPT_QNTY =" + SystemBase.Validation.Decimal_Data(Ds.Tables[0].Rows[j][51].ToString().Trim(), ",") + " ";  //투입수량

                                strSql += ", @pUSR_ID ='" + SystemBase.Base.gstrUserID + "' ";                                   //사용자

                                DataSet ds = SystemBase.DbOpen.NoTranDataSet(strSql);
                            }

                            if (Ds.Tables[0].Rows[j][0].ToString().Trim() == "") BlankRow++;
                            if (BlankRow >= 5) j = Ds.Tables[0].Rows.Count;
                        }
                        adapter.Dispose();
                        command.Dispose();

                        #endregion

                        #region 3.단가정보
                        // 제출기준정보의 제출업체와 기준연월 정보
                        strSql2 = " usp_DAA003단가정보 ";
                        strSql2 += "  @pTYPE = 'I1'";
                        strSql2 += ", @pMNUF_CODE = '" + cboM_MNUF_CODE.SelectedValue + "'";  //제출업체
                        strSql2 += ", @pSTD_YRMON ='" + SystemBase.Validation.C1DataEdit_ReadFormat(dtM_STD_YRMON.Value.ToString(), "YYYYMM") + "' ";  //기준연월
                        strSql2 += ", @pUSR_ID ='" + SystemBase.Base.gstrUserID + "' ";                    //사용자

                        Ds2 = SystemBase.DbOpen.NoTranDataSet(strSql2);
                        #endregion

                        #region 4.환산율 및 환산단가
                        BlankRow = 0;
                        command = new OleDbCommand(string.Format("SELECT * FROM [환산율및환산가$]"), connection);
                        adapter = new OleDbDataAdapter(command);
                        adapter.SelectCommand = command;
                        Ds = new DataSet();
                        adapter.Fill(Ds);
                        for (int j = 0; j < Ds.Tables[0].Rows.Count; j++)
                        {
                            if (j > 4 && Ds.Tables[0].Rows[j][0].ToString().Trim() != "")
                            {
                                string strSql = " usp_DAA003환산율및환산단가 ";
                                strSql += "  @pTYPE = 'I1'";
                                strSql += ", @pREGE_SNUM ='" + Ds.Tables[0].Rows[j][0].ToString().Trim() + "' ";  //등록순번
                                strSql += ", @pORDR_YEAR ='" + Ds.Tables[0].Rows[j][1].ToString().Trim() + "' ";  //요구년도
                                strSql += ", @pDCSN_NUMB ='" + Ds.Tables[0].Rows[j][2].ToString().Trim() + "' ";  //판단번호
                                strSql += ", @pCALC_DEGR ='" + Ds.Tables[0].Rows[j][3].ToString().Trim() + "' ";  //제출차수
                                strSql += ", @pDPRT_CODE ='" + Ds.Tables[0].Rows[j][4].ToString().Trim() + "' ";  //구매부서
                                strSql += ", @pCTMF_CODE ='" + Ds.Tables[0].Rows[j][5].ToString().Trim() + "' ";  //계약업체코드
                                strSql += ", @pMNUF_CODE = '" + Ds.Tables[0].Rows[j][6].ToString().Trim() + "'";  //제출업체
                                strSql += ", @pSTD_YRMON ='" + Ds.Tables[0].Rows[j][7].ToString().Trim() + "' ";  //기준연월
                                strSql += ", @pNIIN ='" + Ds.Tables[0].Rows[j][8].ToString().Trim() + "' ";       //재고번호
                                strSql += ", @pUNIT ='" + Ds.Tables[0].Rows[j][9].ToString().Trim() + "' ";       //단위
                                strSql += ", @pDMST_ITNB ='" + Ds.Tables[0].Rows[j][10].ToString().Trim() + "' ";  //내자항목번호

                                strSql += ", @pFCTR_CODE ='" + Ds.Tables[0].Rows[j][11].ToString().Trim() + "' ";  //공장코드
                                strSql += ", @pFCTR_NAME ='" + Ds.Tables[0].Rows[j][12].ToString().Trim() + "' ";  //공장명
                                strSql += ", @pCURC_UNIT ='" + Ds.Tables[0].Rows[j][13].ToString().Trim() + "' ";  //통화코드
                                strSql += ", @pFOBX_DIVS ='" + Ds.Tables[0].Rows[j][14].ToString().Trim() + "' ";  //인도조건
                                strSql += ", @pTRNS_DIVS ='" + Ds.Tables[0].Rows[j][15].ToString().Trim() + "' ";  //운송구분
                                strSql += ", @pAPPL_EXRT =" + SystemBase.Validation.Decimal_Data(Ds.Tables[0].Rows[j][16].ToString().Trim(), ",") + " ";  //적용환율
                                strSql += ", @pFOCU_MTRCST =" + SystemBase.Validation.Decimal_Data(Ds.Tables[0].Rows[j][17].ToString().Trim(), ",") + " ";  //외화(물자대)
                                strSql += ", @pWON_MTRCST =" + SystemBase.Validation.Decimal_Data(Ds.Tables[0].Rows[j][18].ToString().Trim(), ",") + " ";  //원화(물자대)
                                //strSql += ", @pFOB_MTRCST ='" + Ds.Tables[0].Rows[j][19].ToString().Trim() + "' ";  //환율(물자대):자동계산필드
                                strSql += ", @pFARE_AMTMN =" + SystemBase.Validation.Decimal_Data(Ds.Tables[0].Rows[j][20].ToString().Trim(), ",") + " ";  //금액(운임)
                                strSql += ", @pFARE_RATE =" + SystemBase.Validation.Decimal_Data(Ds.Tables[0].Rows[j][21].ToString().Trim(), ",") + " ";  //비율(운임)
                                //strSql += ", @pFARE_CNVT_PRC ='" + Ds.Tables[0].Rows[j][22].ToString().Trim() + "' ";  //환산가(운임):자동계산필드
                                strSql += ", @pISRB_AMTMN =" + SystemBase.Validation.Decimal_Data(Ds.Tables[0].Rows[j][23].ToString().Trim(), ",") + " ";  //금액(보험료)
                                strSql += ", @pISRB_RATE =" + SystemBase.Validation.Decimal_Data(Ds.Tables[0].Rows[j][24].ToString().Trim(), ",") + " ";  //비율(보험료)
                                //strSql += ", @pISRB_CNVT_PRC ='" + Ds.Tables[0].Rows[j][25].ToString().Trim() + "' ";  //환산가(보험료):자동계산필드
                                strSql += ", @pETC_AMTMN =" + SystemBase.Validation.Decimal_Data(Ds.Tables[0].Rows[j][26].ToString().Trim(), ",") + " ";  //금액(기타수입부대비)
                                strSql += ", @pETC_RATE =" + SystemBase.Validation.Decimal_Data(Ds.Tables[0].Rows[j][27].ToString().Trim(), ",") + " ";  //비율(기타수입부대비)
                                //strSql += ", @pETC_CNVT_PRC ='" + Ds.Tables[0].Rows[j][28].ToString().Trim() + "' ";  //환산가(기타수입부대비):자동계산필드
                                //strSql += ", @pCIF_CNVT_RTE ='" + Ds.Tables[0].Rows[j][29].ToString().Trim() + "' ";  //CIF환산율(적용환율):자동계산필드
                                //strSql += ", @pIMPT_UNIT_EXPNS_INCLSN_CVSRT ='" + Ds.Tables[0].Rows[j][30].ToString().Trim() + "' ";  //수입부대비포함환산율(적용환율):자동계산필드
                                strSql += ", @pTRF_AMTMN =" + SystemBase.Validation.Decimal_Data(Ds.Tables[0].Rows[j][31].ToString().Trim(), ",") + " ";  //금액(관세)
                                strSql += ", @pCWON_RATE =" + SystemBase.Validation.Decimal_Data(Ds.Tables[0].Rows[j][32].ToString().Trim(), ",") + " ";  //비율(관세)
                                //strSql += ", @pTRF_CNVT_PRC ='" + Ds.Tables[0].Rows[j][33].ToString().Trim() + "' ";  //환산가(관세):자동계산필드
                                strSql += ", @pPROF_NUMB ='" + Ds.Tables[0].Rows[j][34].ToString().Trim() + "' ";  //증빙		
                                strSql += ", @pNOTE ='" + Ds.Tables[0].Rows[j][35].ToString().Trim() + "' ";  //비고

                                strSql += ", @pUSR_ID ='" + SystemBase.Base.gstrUserID + "' ";                                   //사용자

                                DataSet ds = SystemBase.DbOpen.NoTranDataSet(strSql);
                            }

                            if (Ds.Tables[0].Rows[j][0].ToString().Trim() == "") BlankRow++;
                            if (BlankRow >= 5) j = Ds.Tables[0].Rows.Count;

                        }
                        adapter.Dispose();
                        command.Dispose();
                        #endregion

                        #region 5.투입이력
                        BlankRow = 0;
                        command = new OleDbCommand(string.Format("SELECT * FROM [투입이력$]"), connection);
                        adapter = new OleDbDataAdapter(command);
                        adapter.SelectCommand = command;
                        Ds = new DataSet();
                        adapter.Fill(Ds);
                        for (int j = 0; j < Ds.Tables[0].Rows.Count; j++)
                        {
                            if (j > 4 && Ds.Tables[0].Rows[j][0].ToString().Trim() != "")
                            {
                                string strSql = " usp_DAA003투입이력 ";
                                strSql += "  @pTYPE = 'I1'";
                                strSql += ", @pREGE_SNUM ='" + Ds.Tables[0].Rows[j][0].ToString().Trim() + "' ";  //순번
                                strSql += ", @pORDR_YEAR ='" + Ds.Tables[0].Rows[j][1].ToString().Trim() + "' ";  //지시연도
                                strSql += ", @pDCSN_NUMB ='" + Ds.Tables[0].Rows[j][2].ToString().Trim() + "' ";  //판단번호
                                strSql += ", @pCALC_DEGR ='" + Ds.Tables[0].Rows[j][3].ToString().Trim() + "' ";  //제출차수
                                strSql += ", @pDPRT_CODE ='" + Ds.Tables[0].Rows[j][4].ToString().Trim() + "' ";  //구매부서코드
                                strSql += ", @pCTMF_CODE ='" + Ds.Tables[0].Rows[j][5].ToString().Trim() + "' ";  //조달업체코드
                                strSql += ", @pMNUF_CODE = '" + Ds.Tables[0].Rows[j][6].ToString().Trim() + "'";  //제출업체코드
                                strSql += ", @pSTD_YRMON ='" + Ds.Tables[0].Rows[j][7].ToString().Trim() + "' ";  //제출연월
                                strSql += ", @pNIIN ='" + Ds.Tables[0].Rows[j][8].ToString().Trim() + "' ";       //재고번호
                                strSql += ", @pUNIT ='" + Ds.Tables[0].Rows[j][9].ToString().Trim() + "' ";       //단위
                                strSql += ", @pDMST_ITNB ='" + Ds.Tables[0].Rows[j][10].ToString().Trim() + "' "; //항목

                                strSql += ", @pMTMG_NUMB ='" + Ds.Tables[0].Rows[j][11].ToString().Trim() + "' ";  //부품관리번호
                                strSql += ", @pPART_NAME ='" + SystemBase.Validation.String_Data(Ds.Tables[0].Rows[j][12].ToString().Trim(), "'") + "' ";  //품명
                                strSql += ", @pMNUF_PANO ='" + Ds.Tables[0].Rows[j][13].ToString().Trim() + "' ";  //업체품번
                                strSql += ", @pREPG_ENNO ='" + Ds.Tables[0].Rows[j][14].ToString().Trim() + "' ";  //대체품업체품번
                                strSql += ", @pPART_UNIT ='" + Ds.Tables[0].Rows[j][15].ToString().Trim() + "' ";  //단위
                                strSql += ", @pINPT_QNTY =" + SystemBase.Validation.Decimal_Data(Ds.Tables[0].Rows[j][16].ToString().Trim(), ",") + " ";  //투입수량
                                strSql += ", @pNOTE ='" + Ds.Tables[0].Rows[j][17].ToString().Trim() + "' ";       //비고 

                                strSql += ", @pUSR_ID ='" + SystemBase.Base.gstrUserID + "' ";                     //사용자

                                DataSet ds = SystemBase.DbOpen.NoTranDataSet(strSql);
                            }

                            if (Ds.Tables[0].Rows[j][0].ToString().Trim() == "") BlankRow++;
                            if (BlankRow >= 5) j = Ds.Tables[0].Rows.Count;
                        }
                        adapter.Dispose();
                        command.Dispose();
                        #endregion

                        #region 6.수입내역
                        BlankRow = 0;
                        command = new OleDbCommand(string.Format("SELECT * FROM [수입내역$]"), connection);
                        adapter = new OleDbDataAdapter(command);
                        adapter.SelectCommand = command;
                        Ds = new DataSet();
                        adapter.Fill(Ds);
                        for (int j = 0; j < Ds.Tables[0].Rows.Count; j++)
                        {
                            if (j > 4 && Ds.Tables[0].Rows[j][0].ToString().Trim() != "")
                            {
                                string strSql = " usp_DAA003수입내역 ";
                                strSql += "  @pTYPE = 'I1'";
                                strSql += ", @pREGE_SNUM =" + Ds.Tables[0].Rows[j][0].ToString().Trim() + "";    //등록순번
                                strSql += ", @pORDR_YEAR ='" + Ds.Tables[0].Rows[j][1].ToString().Trim() + "'";  //지시연도
                                strSql += ", @pDCSN_NUMB ='" + Ds.Tables[0].Rows[j][2].ToString().Trim() + "'";  //판단번호
                                strSql += ", @pCALC_DEGR ='" + Ds.Tables[0].Rows[j][3].ToString().Trim() + "'";  //제출차수
                                strSql += ", @pDPRT_CODE ='" + Ds.Tables[0].Rows[j][4].ToString().Trim() + "'";  //구매부서
                                strSql += ", @pCTMF_CODE ='" + Ds.Tables[0].Rows[j][5].ToString().Trim() + "'";  //계약업체코드
                                strSql += ", @pMNUF_CODE = '" + Ds.Tables[0].Rows[j][6].ToString().Trim() + "'"; //제출업체
                                strSql += ", @pSTD_YRMON ='" + Ds.Tables[0].Rows[j][7].ToString().Trim() + "'";  //기준연월
                                strSql += ", @pNIIN ='" + Ds.Tables[0].Rows[j][8].ToString().Trim() + "'";       //재고번호
                                strSql += ", @pUNIT ='" + Ds.Tables[0].Rows[j][9].ToString().Trim() + "'";       //단위
                                strSql += ", @pDMST_ITNB ='" + Ds.Tables[0].Rows[j][10].ToString().Trim() + "'"; //내자항목번호

                                strSql += ", @pFCTR_CODE ='" + Ds.Tables[0].Rows[j][11].ToString().Trim() + "'";  //공장코드
                                strSql += ", @pFCTR_NAME ='" + SystemBase.Validation.String_Data(Ds.Tables[0].Rows[j][12].ToString().Trim(), "'") + "'";   //공장명
                                strSql += ", @pIMPT_YEAR ='" + Ds.Tables[0].Rows[j][13].ToString().Trim() + "'";  //수입연도
                                strSql += ", @pLCXX_NUMB ='" + Ds.Tables[0].Rows[j][14].ToString().Trim() + "'";  //L/C No
                                strSql += ", @pBLXX_NUMB ='" + Ds.Tables[0].Rows[j][15].ToString().Trim() + "'";  //B/L(AWB) No
                                strSql += ", @pITEM_NAME ='" + SystemBase.Validation.String_Data(Ds.Tables[0].Rows[j][16].ToString().Trim(), "'") + "'";  //품명
                                strSql += ", @pIMLI_STNO ='" + Ds.Tables[0].Rows[j][17].ToString().Trim() + "'";  //수입신고필증
                                strSql += ", @pCURC_UNIT ='" + Ds.Tables[0].Rows[j][18].ToString().Trim() + "'";  //통화코드
                                strSql += ", @pFOBX_DIVS ='" + Ds.Tables[0].Rows[j][19].ToString().Trim() + "'";  //인도조건
                                strSql += ", @pTRNS_DIVS ='" + Ds.Tables[0].Rows[j][20].ToString().Trim() + "'";  //운송구분
                                strSql += ", @pADDX_DIVS ='" + Ds.Tables[0].Rows[j][21].ToString().Trim() + "'";  //비목구분
                                strSql += ", @pFOCU_AMNT =" + SystemBase.Validation.Decimal_Data(Ds.Tables[0].Rows[j][22].ToString().Trim(), ",") + "";  //금액(외화)
                                //strSql += ", @pWONX_AMNT =" + SystemBase.Validation.Decimal_Data(Ds.Tables[0].Rows[j][23].ToString().Trim(), ",") + "";  //금액(원화) - 자동계산
                                strSql += ", @pENTR_EXRT =" + SystemBase.Validation.Decimal_Data(Ds.Tables[0].Rows[j][24].ToString().Trim(), ",") + "";  //통관환율
                                strSql += ", @pENTR_EXDT ='" + Ds.Tables[0].Rows[j][25].ToString().Trim() + "'";  //통관환율 기준일
                                strSql += ", @pSANC_PRRT =" + SystemBase.Validation.Decimal_Data(Ds.Tables[0].Rows[j][26].ToString().Trim(), ",") + "";  //결제환율
                                strSql += ", @pPREX_STDT ='" + Ds.Tables[0].Rows[j][27].ToString().Trim() + "' ";  //결제환율기준일
                                strSql += ", @pNOTE ='" + Ds.Tables[0].Rows[j][28].ToString().Trim() + "' ";       //비고

                                strSql += ", @pUSR_ID ='" + SystemBase.Base.gstrUserID + "' ";                   //사용자

                                DataSet ds = SystemBase.DbOpen.NoTranDataSet(strSql);
                            }
                            if (Ds.Tables[0].Rows[j][0].ToString().Trim() == "") BlankRow++;
                            if (BlankRow >= 5) j = Ds.Tables[0].Rows.Count;
                        }
                        adapter.Dispose();
                        command.Dispose();
                        #endregion

                        #region 7.국내주요재료비
                        // 제출기준정보의 제출업체와 기준연월 정보
                        strSql2 = " usp_DAA003국내주요재료비 ";
                        strSql2 += "  @pTYPE = 'I1'";
                        strSql2 += ", @pMNUF_CODE = '" + cboM_MNUF_CODE.SelectedValue + "'";  //제출업체
                        strSql2 += ", @pSTD_YRMON ='" + SystemBase.Validation.C1DataEdit_ReadFormat(dtM_STD_YRMON.Value.ToString(), "YYYYMM") + "' ";  //기준연월
                        strSql2 += ", @pUSR_ID ='" + SystemBase.Base.gstrUserID + "' ";                    //사용자
                        Ds2 = SystemBase.DbOpen.NoTranDataSet(strSql2);
                        #endregion

                        #region 8.반제품비
                        // 제출기준정보의 제출업체와 기준연월 정보
                        strSql2 = " usp_DAA003반제품비 ";
                        strSql2 += "  @pTYPE = 'I1'";
                        strSql2 += ", @pMNUF_CODE = '" + cboM_MNUF_CODE.SelectedValue + "'";  //제출업체
                        strSql2 += ", @pSTD_YRMON ='" + SystemBase.Validation.C1DataEdit_ReadFormat(dtM_STD_YRMON.Value.ToString(), "YYYYMM") + "' ";  //기준연월
                        strSql2 += ", @pUSR_ID ='" + SystemBase.Base.gstrUserID + "' ";                    //사용자
                        Ds2 = SystemBase.DbOpen.NoTranDataSet(strSql2);
                        #endregion

                        #region 9.국내구입부품비
                        // 제출기준정보의 제출업체와 기준연월 정보
                        strSql2 = " usp_DAA003국내구입부품비 ";
                        strSql2 += "  @pTYPE = 'I1'";
                        strSql2 += ", @pMNUF_CODE = '" + cboM_MNUF_CODE.SelectedValue + "'";  //제출업체
                        strSql2 += ", @pSTD_YRMON ='" + SystemBase.Validation.C1DataEdit_ReadFormat(dtM_STD_YRMON.Value.ToString(), "YYYYMM") + "' ";  //기준연월
                        strSql2 += ", @pUSR_ID ='" + SystemBase.Base.gstrUserID + "' ";                    //사용자
                        Ds2 = SystemBase.DbOpen.NoTranDataSet(strSql2);

                        #endregion

                        #region 10.국내구입부품비방산
                        // 제출기준정보의 제출업체와 기준연월 정보
                        strSql2 = " usp_DAA003국내구입부품비방산 ";
                        strSql2 += "  @pTYPE = 'I1'";
                        strSql2 += ", @pMNUF_CODE = '" + cboM_MNUF_CODE.SelectedValue + "'";  //제출업체
                        strSql2 += ", @pSTD_YRMON ='" + SystemBase.Validation.C1DataEdit_ReadFormat(dtM_STD_YRMON.Value.ToString(), "YYYYMM") + "' ";  //기준연월
                        strSql2 += ", @pUSR_ID ='" + SystemBase.Base.gstrUserID + "' ";                    //사용자
                        Ds2 = SystemBase.DbOpen.NoTranDataSet(strSql2);

                        #endregion

                        #region 11.수입주요재료비
                        // 제출기준정보의 제출업체와 기준연월 정보
                        strSql2 = " usp_DAA003수입주요재료비 ";
                        strSql2 += "  @pTYPE = 'I1'";
                        strSql2 += ", @pMNUF_CODE = '" + cboM_MNUF_CODE.SelectedValue + "'";  //제출업체
                        strSql2 += ", @pSTD_YRMON ='" + SystemBase.Validation.C1DataEdit_ReadFormat(dtM_STD_YRMON.Value.ToString(), "YYYYMM") + "' ";  //기준연월
                        strSql2 += ", @pUSR_ID ='" + SystemBase.Base.gstrUserID + "' ";                    //사용자
                        Ds2 = SystemBase.DbOpen.NoTranDataSet(strSql2);
                        #endregion

                        #region 12.수입부품비
                        // 제출기준정보의 제출업체와 기준연월 정보
                        strSql2 = " usp_DAA003수입부품비 ";
                        strSql2 += "  @pTYPE = 'I1'";
                        strSql2 += ", @pMNUF_CODE = '" + cboM_MNUF_CODE.SelectedValue + "'";  //제출업체
                        strSql2 += ", @pSTD_YRMON ='" + SystemBase.Validation.C1DataEdit_ReadFormat(dtM_STD_YRMON.Value.ToString(), "YYYYMM") + "' ";  //기준연월
                        strSql2 += ", @pUSR_ID ='" + SystemBase.Base.gstrUserID + "' ";                    //사용자
                        Ds2 = SystemBase.DbOpen.NoTranDataSet(strSql2);
                        #endregion

                        #region 13.포장재료비
                        BlankRow = 0;
                        command = new OleDbCommand(string.Format("SELECT * FROM [포장재료비$]"), connection);
                        adapter = new OleDbDataAdapter(command);
                        adapter.SelectCommand = command;
                        Ds = new DataSet();
                        adapter.Fill(Ds);
                        for (int j = 0; j < Ds.Tables[0].Rows.Count; j++)
                        {
                            if (j > 4 && Ds.Tables[0].Rows[j][0].ToString().Trim() != "")
                            {
                                string strSql = " usp_DAA003포장재료비 ";
                                strSql += "  @pTYPE = 'I1'";
                                strSql += ", @pREGE_SNUM =" + Ds.Tables[0].Rows[j][0].ToString().Trim() + "";  //등록순번
                                strSql += ", @pORDR_YEAR ='" + Ds.Tables[0].Rows[j][1].ToString().Trim() + "'";  //지시연도
                                strSql += ", @pDCSN_NUMB ='" + Ds.Tables[0].Rows[j][2].ToString().Trim() + "'";  //판단번호
                                strSql += ", @pCALC_DEGR ='" + Ds.Tables[0].Rows[j][3].ToString().Trim() + "'";  //제출차수
                                strSql += ", @pDPRT_CODE ='" + Ds.Tables[0].Rows[j][4].ToString().Trim() + "'";  //구매부서
                                strSql += ", @pCTMF_CODE ='" + Ds.Tables[0].Rows[j][5].ToString().Trim() + "'";  //계약업체코드
                                strSql += ", @pMNUF_CODE = '" + Ds.Tables[0].Rows[j][6].ToString().Trim() + "'";  //제출업체
                                strSql += ", @pSTD_YRMON ='" + Ds.Tables[0].Rows[j][7].ToString().Trim() + "'";  //기준연월
                                strSql += ", @pNIIN ='" + Ds.Tables[0].Rows[j][8].ToString().Trim() + "'";       //재고번호
                                strSql += ", @pUNIT ='" + Ds.Tables[0].Rows[j][9].ToString().Trim() + "'";       //단위
                                strSql += ", @pDMST_ITNB ='" + Ds.Tables[0].Rows[j][10].ToString().Trim() + "'"; //내자항목번호

                                strSql += ", @pMTMG_NUMB ='" + Ds.Tables[0].Rows[j][11].ToString().Trim() + "'";  //부품관리번호
                                strSql += ", @pMNUF_PANO ='" + Ds.Tables[0].Rows[j][12].ToString().Trim() + "'";  //업체품번
                                strSql += ", @pMNUF_ITNM ='" + SystemBase.Validation.String_Data(Ds.Tables[0].Rows[j][13].ToString().Trim(), "'") + "'";  //업체품명
                                strSql += ", @pPKGE_TYPE ='" + Ds.Tables[0].Rows[j][14].ToString().Trim() + "'";  //포장형태
                                strSql += ", @pPKGE_UNIT ='" + Ds.Tables[0].Rows[j][15].ToString().Trim() + "'";  //단위
                                strSql += ", @pREQR_QNTY =" + SystemBase.Validation.Decimal_Data(Ds.Tables[0].Rows[j][16].ToString().Trim(), ",") + "";  //소요량(A)
                                strSql += ", @pPKGE_QNTY =" + SystemBase.Validation.Decimal_Data(Ds.Tables[0].Rows[j][17].ToString().Trim(), ",") + "";  //포장수량(B)
                                strSql += ", @pUNIT_PRCE =" + SystemBase.Validation.Decimal_Data(Ds.Tables[0].Rows[j][19].ToString().Trim(), ",") + "";  //단가(D)
                                strSql += ", @pAPST_NBMT ='" + Ds.Tables[0].Rows[j][21].ToString().Trim() + "'";  //시작호기
                                strSql += ", @pAPFN_NBMT ='" + Ds.Tables[0].Rows[j][22].ToString().Trim() + "'";  //종료호기
                                strSql += ", @pPROF_NUMB ='" + Ds.Tables[0].Rows[j][23].ToString().Trim() + "'";  //증빙
                                strSql += ", @pNOTE ='" + Ds.Tables[0].Rows[j][24].ToString().Trim() + "'";  //비고

                                strSql += ", @pUSR_ID ='" + SystemBase.Base.gstrUserID + "' ";                   //사용자

                                DataSet ds = SystemBase.DbOpen.NoTranDataSet(strSql);
                            }
                            if (Ds.Tables[0].Rows[j][0].ToString().Trim() == "") BlankRow++;
                            if (BlankRow >= 5) j = Ds.Tables[0].Rows.Count;
                        }
                        adapter.Dispose();
                        command.Dispose();
                        #endregion

                        #region 14.설물(부품)
                        // 제출기준정보의 제출업체와 기준연월 정보
                        strSql2 = " usp_DAA003설물부품 ";
                        strSql2 += "  @pTYPE = 'I1'";
                        strSql2 += ", @pMNUF_CODE = '" + cboM_MNUF_CODE.SelectedValue + "'";  //제출업체
                        strSql2 += ", @pSTD_YRMON ='" + SystemBase.Validation.C1DataEdit_ReadFormat(dtM_STD_YRMON.Value.ToString(), "YYYYMM") + "' ";  //기준연월
                        strSql2 += ", @pUSR_ID ='" + SystemBase.Base.gstrUserID + "' ";                    //사용자
                        Ds2 = SystemBase.DbOpen.NoTranDataSet(strSql2);
                        #endregion

                        #region 15.설물발생액
                        BlankRow = 0;
                        command = new OleDbCommand(string.Format("SELECT * FROM [설물(발생액)$]"), connection);
                        adapter = new OleDbDataAdapter(command);
                        adapter.SelectCommand = command;
                        Ds = new DataSet();
                        adapter.Fill(Ds);
                        for (int j = 0; j < Ds.Tables[0].Rows.Count; j++)
                        {
                            if (j > 4 && Ds.Tables[0].Rows[j][0].ToString().Trim() != "")
                            {
                                string strSql = " usp_DAA003설물발생액 ";
                                strSql += "  @pTYPE = 'I1'";
                                strSql += ", @pREGE_SNUM ='" + Ds.Tables[0].Rows[j][0].ToString().Trim() + "' ";  //등록순번
                                strSql += ", @pORDR_YEAR ='" + Ds.Tables[0].Rows[j][1].ToString().Trim() + "' ";  //요구년도
                                strSql += ", @pDCSN_NUMB ='" + Ds.Tables[0].Rows[j][2].ToString().Trim() + "' ";  //판단번호
                                strSql += ", @pCALC_DEGR ='" + Ds.Tables[0].Rows[j][3].ToString().Trim() + "' ";  //제출차수
                                strSql += ", @pDPRT_CODE ='" + Ds.Tables[0].Rows[j][4].ToString().Trim() + "' ";  //구매부서
                                strSql += ", @pCTMF_CODE ='" + Ds.Tables[0].Rows[j][5].ToString().Trim() + "' ";  //계약업체코드
                                strSql += ", @pMNUF_CODE = '" + Ds.Tables[0].Rows[j][6].ToString().Trim() + "'";  //제출업체
                                strSql += ", @pSTD_YRMON ='" + Ds.Tables[0].Rows[j][7].ToString().Trim() + "' ";  //기준연월
                                strSql += ", @pNIIN ='" + Ds.Tables[0].Rows[j][8].ToString().Trim() + "' ";       //재고번호
                                strSql += ", @pUNIT ='" + Ds.Tables[0].Rows[j][9].ToString().Trim() + "' ";       //단위
                                strSql += ", @pDMST_ITNB ='" + Ds.Tables[0].Rows[j][10].ToString().Trim() + "' ";  //내자항목번호

                                strSql += ", @pMTMG_NUMB ='" + Ds.Tables[0].Rows[j][11].ToString().Trim() + "' ";  //부품관리번호
                                strSql += ", @pMATR_DIVS ='" + Ds.Tables[0].Rows[j][12].ToString().Trim() + "' ";  //재료비구분
                                strSql += ", @pETCX_TAMT =" + SystemBase.Validation.Decimal_Data(Ds.Tables[0].Rows[j][13].ToString().Trim(), ",") + " ";  //총설물금액
                                strSql += ", @pMATR_TAMT =" + SystemBase.Validation.Decimal_Data(Ds.Tables[0].Rows[j][14].ToString().Trim(), ",") + " ";  //총재료비
                                //strSql += ", @pETCX_RATE ='" + Ds.Tables[0].Rows[j][15].ToString().Trim() + "' ";  //비율 : 자동계산됨
                                strSql += ", @pMATR_AMNT =" + SystemBase.Validation.Decimal_Data(Ds.Tables[0].Rows[j][16].ToString().Trim(), ",") + " ";  //직접재료비
                                //strSql += ", @pETCX_AMNT ='" + Ds.Tables[0].Rows[j][17].ToString().Trim() + "' ";  //설물금액 : 자동계산됨
                                strSql += ", @pAPST_NBMT ='" + Ds.Tables[0].Rows[j][18].ToString().Trim() + "' ";  //시작호기(적용호기)
                                strSql += ", @pAPFN_NBMT ='" + Ds.Tables[0].Rows[j][19].ToString().Trim() + "' ";  //종료호기(적용호기)
                                strSql += ", @pPROF_NUMB ='" + Ds.Tables[0].Rows[j][20].ToString().Trim() + "' ";  //증빙		
                                strSql += ", @pNOTE ='" + Ds.Tables[0].Rows[j][21].ToString().Trim() + "' ";  //비고		
                                strSql += ", @pUSR_ID ='" + SystemBase.Base.gstrUserID + "' ";                                   //사용자

                                DataSet ds = SystemBase.DbOpen.NoTranDataSet(strSql);
                            }

                            if (Ds.Tables[0].Rows[j][0].ToString().Trim() == "") BlankRow++;
                            if (BlankRow >= 5) j = Ds.Tables[0].Rows.Count;

                        }
                        adapter.Dispose();
                        command.Dispose();
                        #endregion

                        #region 16.관급재료비
                        // 제출기준정보의 제출업체와 기준연월 정보
                        strSql2 = " usp_DAA003관급재료비 ";
                        strSql2 += "  @pTYPE = 'I1'";
                        strSql2 += ", @pMNUF_CODE = '" + cboM_MNUF_CODE.SelectedValue + "'";  //제출업체
                        strSql2 += ", @pSTD_YRMON ='" + SystemBase.Validation.C1DataEdit_ReadFormat(dtM_STD_YRMON.Value.ToString(), "YYYYMM") + "' ";  //기준연월
                        strSql2 += ", @pUSR_ID ='" + SystemBase.Base.gstrUserID + "' ";                    //사용자
                        Ds2 = SystemBase.DbOpen.NoTranDataSet(strSql2);
                        #endregion

                        #region 17.직접노무량
                        BlankRow = 0;
                        command = new OleDbCommand(string.Format("SELECT * FROM [직접노무량$]"), connection);
                        adapter = new OleDbDataAdapter(command);
                        adapter.SelectCommand = command;
                        Ds = new DataSet();
                        adapter.Fill(Ds);
                        for (int j = 0; j < Ds.Tables[0].Rows.Count; j++)
                        {
                            if (j > 4 && Ds.Tables[0].Rows[j][0].ToString().Trim() != "")
                            {
                                string strSql = " usp_DAA003직접노무량 ";
                                strSql += "  @pTYPE = 'I1'";
                                strSql += ", @pREGE_SNUM =" + Ds.Tables[0].Rows[j][0].ToString().Trim() + "";  //등록순번
                                strSql += ", @pORDR_YEAR ='" + Ds.Tables[0].Rows[j][1].ToString().Trim() + "'";  //지시연도
                                strSql += ", @pDCSN_NUMB ='" + Ds.Tables[0].Rows[j][2].ToString().Trim() + "'";  //판단번호
                                strSql += ", @pCALC_DEGR ='" + Ds.Tables[0].Rows[j][3].ToString().Trim() + "'";  //제출차수
                                strSql += ", @pDPRT_CODE ='" + Ds.Tables[0].Rows[j][4].ToString().Trim() + "'";  //구매부서
                                strSql += ", @pCTMF_CODE ='" + Ds.Tables[0].Rows[j][5].ToString().Trim() + "'";  //계약업체코드
                                strSql += ", @pMNUF_CODE = '" + Ds.Tables[0].Rows[j][6].ToString().Trim() + "'";  //제출업체
                                strSql += ", @pSTD_YRMON ='" + Ds.Tables[0].Rows[j][7].ToString().Trim() + "'";  //기준연월
                                strSql += ", @pNIIN ='" + Ds.Tables[0].Rows[j][8].ToString().Trim() + "'";       //재고번호
                                strSql += ", @pUNIT ='" + Ds.Tables[0].Rows[j][9].ToString().Trim() + "'";       //단위
                                strSql += ", @pDMST_ITNB ='" + Ds.Tables[0].Rows[j][10].ToString().Trim() + "'"; //내자항목번호
                                strSql += ", @pMTMG_NUMB ='" + Ds.Tables[0].Rows[j][11].ToString().Trim() + "'"; //부품관리번호
                                strSql += ", @pPART_NAME ='" + SystemBase.Validation.String_Data(Ds.Tables[0].Rows[j][12].ToString().Trim(), "'") + "'"; //품명
                                strSql += ", @pMNUF_PANO ='" + Ds.Tables[0].Rows[j][13].ToString().Trim() + "'"; //업체품번
                                strSql += ", @pAPST_NBMT ='" + Ds.Tables[0].Rows[j][14].ToString().Trim() + "'"; //시작호기
                                strSql += ", @pAPFN_NBMT ='" + Ds.Tables[0].Rows[j][15].ToString().Trim() + "'"; //종료호기
                                strSql += ", @pPRDCN_CMPLT_YN ='" + Ds.Tables[0].Rows[j][16].ToString().Trim() + "'"; //생산완료여부
                                strSql += ", @pJOBX_YYMM ='" + Ds.Tables[0].Rows[j][17].ToString().Trim() + "'"; //작업연월
                                strSql += ", @pFCTR_CODE ='" + Ds.Tables[0].Rows[j][18].ToString().Trim() + "'"; //공장코드
                                strSql += ", @pCMPNTS_BYBUND ='" + Ds.Tables[0].Rows[j][19].ToString().Trim() + "'"; //부품뭉치별
                                strSql += ", @pDPRC_NAME ='" + SystemBase.Validation.String_Data(Ds.Tables[0].Rows[j][20].ToString().Trim(), "'") + "'"; //간접노무비 공정명
                                strSql += ", @pIDMT_NAME ='" + SystemBase.Validation.String_Data(Ds.Tables[0].Rows[j][21].ToString().Trim(), "'") + "'"; //감가상각비 공정명
                                strSql += ", @pPRCS_NAME ='" + SystemBase.Validation.String_Data(Ds.Tables[0].Rows[j][22].ToString().Trim(), "'") + "'"; //공정명
                                strSql += ", @pEMP_NUMB ='" + Ds.Tables[0].Rows[j][23].ToString().Trim() + "'"; //사번
                                strSql += ", @pOPER_NAME ='" + SystemBase.Validation.String_Data(Ds.Tables[0].Rows[j][24].ToString().Trim(), "'") + "'"; //작업자
                                strSql += ", @pPAYO_WMAN =" + SystemBase.Validation.Decimal_Data(Ds.Tables[0].Rows[j][25].ToString().Trim(), ",") + ""; //작업자인원수
                                strSql += ", @pREAL_TDMS =" + SystemBase.Validation.Decimal_Data(Ds.Tables[0].Rows[j][26].ToString().Trim(), ",") + ""; //실동공수(M/H)
                                strSql += ", @pPROF_NUMB ='" + Ds.Tables[0].Rows[j][27].ToString().Trim() + "'"; //증빙
                                strSql += ", @pNOTE ='" + Ds.Tables[0].Rows[j][28].ToString().Trim() + "'"; //비고

                                strSql += ", @pUSR_ID ='" + SystemBase.Base.gstrUserID + "' ";                   //사용자

                                DataSet ds = SystemBase.DbOpen.NoTranDataSet(strSql);
                            }
                            if (Ds.Tables[0].Rows[j][0].ToString().Trim() == "") BlankRow++;
                            if (BlankRow >= 5) j = Ds.Tables[0].Rows.Count;
                        }
                        adapter.Dispose();
                        command.Dispose();
                        #endregion

                        #region 18.단위공수계산
                        // 제출기준정보의 제출업체와 기준연월 정보
                        strSql2 = " usp_DAA003단위공수계산 ";
                        strSql2 += "  @pTYPE = 'I1'";
                        strSql2 += ", @pMNUF_CODE = '" + cboM_MNUF_CODE.SelectedValue + "'";  //제출업체
                        strSql2 += ", @pSTD_YRMON ='" + SystemBase.Validation.C1DataEdit_ReadFormat(dtM_STD_YRMON.Value.ToString(), "YYYYMM") + "' ";  //기준연월
                        strSql2 += ", @pUSR_ID ='" + SystemBase.Base.gstrUserID + "' ";                    //사용자
                        Ds2 = SystemBase.DbOpen.NoTranDataSet(strSql2);
                        #endregion

                        #region 19.직접노무비
                        // 제출기준정보의 제출업체와 기준연월 정보
                        strSql2 = " usp_DAA003직접노무비 ";
                        strSql2 += "  @pTYPE = 'I1'";
                        strSql2 += ", @pMNUF_CODE = '" + cboM_MNUF_CODE.SelectedValue + "'";  //제출업체
                        strSql2 += ", @pSTD_YRMON ='" + SystemBase.Validation.C1DataEdit_ReadFormat(dtM_STD_YRMON.Value.ToString(), "YYYYMM") + "' ";  //기준연월
                        strSql2 += ", @pUSR_ID ='" + SystemBase.Base.gstrUserID + "' ";                    //사용자
                        Ds2 = SystemBase.DbOpen.NoTranDataSet(strSql2);
                        #endregion

                        #region 20.감가상각비(전용)
                        BlankRow = 0;
                        command = new OleDbCommand(string.Format("SELECT * FROM [감가상각비(전용)$]"), connection);
                        adapter = new OleDbDataAdapter(command);
                        adapter.SelectCommand = command;
                        Ds = new DataSet();
                        adapter.Fill(Ds);
                        for (int j = 0; j < Ds.Tables[0].Rows.Count; j++)
                        {
                            if (j > 4 && Ds.Tables[0].Rows[j][0].ToString().Trim() != "")
                            {
                                string strSql = " usp_DAA003감가상각비전용 ";
                                strSql += "  @pTYPE = 'I1'";
                                strSql += ", @pREGE_SNUM ='" + Ds.Tables[0].Rows[j][0].ToString().Trim() + "' ";  //등록순번
                                strSql += ", @pORDR_YEAR ='" + Ds.Tables[0].Rows[j][1].ToString().Trim() + "' ";  //지시연도
                                strSql += ", @pDCSN_NUMB ='" + Ds.Tables[0].Rows[j][2].ToString().Trim() + "' ";  //판단번호
                                strSql += ", @pCALC_DEGR ='" + Ds.Tables[0].Rows[j][3].ToString().Trim() + "' ";  //차수
                                strSql += ", @pDPRT_CODE ='" + Ds.Tables[0].Rows[j][4].ToString().Trim() + "' ";  //구매부서코드
                                strSql += ", @pCTMF_CODE ='" + Ds.Tables[0].Rows[j][5].ToString().Trim() + "' ";  //조달업체코드
                                strSql += ", @pMNUF_CODE = '" + Ds.Tables[0].Rows[j][6].ToString().Trim() + "'";  //제출업체코드
                                strSql += ", @pSTD_YRMON ='" + Ds.Tables[0].Rows[j][7].ToString().Trim() + "' ";  //기준연월
                                strSql += ", @pNIIN ='" + Ds.Tables[0].Rows[j][8].ToString().Trim() + "' ";       //재고번호
                                strSql += ", @pUNIT ='" + Ds.Tables[0].Rows[j][9].ToString().Trim() + "' ";       //단위
                                strSql += ", @pDMST_ITNB ='" + Ds.Tables[0].Rows[j][10].ToString().Trim() + "' "; //항목

                                strSql += ", @pMFPT_SEQS ='" + Ds.Tables[0].Rows[j][11].ToString().Trim() + "' "; //업체자산관리번호
                                strSql += ", @pPRPT_NAME ='" + Ds.Tables[0].Rows[j][12].ToString().Trim() + "' "; //자산명
                                strSql += ", @pPRPT_DIVS ='" + Ds.Tables[0].Rows[j][13].ToString().Trim() + "' "; //자산구분
                                strSql += ", @pPRCS_NAME ='" + Ds.Tables[0].Rows[j][14].ToString().Trim() + "' "; //공정명
                                strSql += ", @pKIND_QNTY =" + SystemBase.Validation.Decimal_Data(Ds.Tables[0].Rows[j][15].ToString().Trim(), ","); //종수
                                strSql += ", @pTACQ_DIVS ='" + Ds.Tables[0].Rows[j][16].ToString().Trim() + "' "; //취득구분
                                strSql += ", @pTACQ_PLMT =" + SystemBase.Validation.Decimal_Data(Ds.Tables[0].Rows[j][17].ToString().Trim(), ","); //취득가액(A)(장부가액)  
                                strSql += ", @pCRED_AMNT=" + SystemBase.Validation.Decimal_Data(Ds.Tables[0].Rows[j][18].ToString().Trim(), ","); //재평가차액(B)
                                strSql += ", @pCPEX_AMNT=" + SystemBase.Validation.Decimal_Data(Ds.Tables[0].Rows[j][19].ToString().Trim(), ","); //자본적지출액 (C) 
                                //strSql += ", @pTOTAL_ACQ_AMTMN=" + SystemBase.Validation.Decimal_Data(Ds.Tables[0].Rows[j][20].ToString().Trim(), ",") ; //총취득금액(A+B+C):자동계산필드
                                strSql += ", @pTACQ_DATE ='" + Ds.Tables[0].Rows[j][21].ToString().Trim() + "' "; //취득일자
                                strSql += ", @pULFS_DIVS ='" + Ds.Tables[0].Rows[j][22].ToString().Trim() + "' "; //내용연수구분
                                strSql += ", @pCTNT_NUMYR =" + SystemBase.Validation.Decimal_Data(Ds.Tables[0].Rows[j][23].ToString().Trim(), ",");  //내용연수
                                strSql += ", @pBIZ_PRD_NUMYR =" + SystemBase.Validation.Decimal_Data(Ds.Tables[0].Rows[j][24].ToString().Trim(), ",");  //사업기간연수
                                strSql += ", @pDPRC_AMNT=" + SystemBase.Validation.Decimal_Data(Ds.Tables[0].Rows[j][25].ToString().Trim(), ","); //상각대상액(E)
                                strSql += ", @pDPRC_RMAM=" + SystemBase.Validation.Decimal_Data(Ds.Tables[0].Rows[j][26].ToString().Trim(), ","); //미상각잔액(F)
                                strSql += ", @pDSTRBT_STD=" + SystemBase.Validation.Decimal_Data(Ds.Tables[0].Rows[j][27].ToString().Trim(), ","); //배부기준(G)
                                //strSql += ", @pUNIT_DPRC=" + SystemBase.Validation.Decimal_Data(Ds.Tables[0].Rows[j][28].ToString().Trim(), ","); //단위 감가상각비(H=E/G):자동계산필드
                                //strSql += ", @pUI_PER_NT_DPRC_BLNC=" + SystemBase.Validation.Decimal_Data(Ds.Tables[0].Rows[j][29].ToString().Trim(), ","); //단위당 미상각잔액 (F/G):자동계산필드
                                strSql += ", @pPROF_NUMB ='" + Ds.Tables[0].Rows[j][30].ToString().Trim() + "' ";  //증빙		
                                strSql += ", @pNOTE ='" + Ds.Tables[0].Rows[j][31].ToString().Trim() + "' ";       //비고	

                                strSql += ", @pUSR_ID ='" + SystemBase.Base.gstrUserID + "' ";                     //사용자

                                DataSet ds = SystemBase.DbOpen.NoTranDataSet(strSql);
                            }

                            if (Ds.Tables[0].Rows[j][0].ToString().Trim() == "") BlankRow++;
                            if (BlankRow >= 5) j = Ds.Tables[0].Rows.Count;

                        }
                        adapter.Dispose();
                        command.Dispose();
                        #endregion

                        #region 21.지급임차료(전용)
                        BlankRow = 0;
                        command = new OleDbCommand(string.Format("SELECT * FROM [지급임차료(전용)$]"), connection);
                        adapter = new OleDbDataAdapter(command);
                        adapter.SelectCommand = command;
                        Ds = new DataSet();
                        adapter.Fill(Ds);
                        for (int j = 0; j < Ds.Tables[0].Rows.Count; j++)
                        {
                            if (j > 4 && Ds.Tables[0].Rows[j][0].ToString().Trim() != "")
                            {
                                string strSql = " usp_DAA003지급임차료전용 ";
                                strSql += "  @pTYPE = 'I1'";
                                strSql += ", @pREGE_SNUM ='" + Ds.Tables[0].Rows[j][0].ToString().Trim() + "' ";  //등록순번
                                strSql += ", @pORDR_YEAR ='" + Ds.Tables[0].Rows[j][1].ToString().Trim() + "' ";  //요구년도
                                strSql += ", @pDCSN_NUMB ='" + Ds.Tables[0].Rows[j][2].ToString().Trim() + "' ";  //판단번호
                                strSql += ", @pCALC_DEGR ='" + Ds.Tables[0].Rows[j][3].ToString().Trim() + "' ";  //제출차수
                                strSql += ", @pDPRT_CODE ='" + Ds.Tables[0].Rows[j][4].ToString().Trim() + "' ";  //구매부서
                                strSql += ", @pCTMF_CODE ='" + Ds.Tables[0].Rows[j][5].ToString().Trim() + "' ";  //계약업체코드
                                strSql += ", @pMNUF_CODE = '" + Ds.Tables[0].Rows[j][6].ToString().Trim() + "'";  //제출업체
                                strSql += ", @pSTD_YRMON ='" + Ds.Tables[0].Rows[j][7].ToString().Trim() + "' ";  //기준연월
                                strSql += ", @pNIIN ='" + Ds.Tables[0].Rows[j][8].ToString().Trim() + "' ";       //재고번호
                                strSql += ", @pUNIT ='" + Ds.Tables[0].Rows[j][9].ToString().Trim() + "' ";       //단위
                                strSql += ", @pDMST_ITNB ='" + Ds.Tables[0].Rows[j][10].ToString().Trim() + "' ";  //내자항목번호

                                strSql += ", @pOCRC_DATE ='" + Ds.Tables[0].Rows[j][11].ToString().Trim() + "' ";  //발생일자		
                                strSql += ", @pOCIT_NAME ='" + Ds.Tables[0].Rows[j][12].ToString().Trim() + "' ";  //세부항목명	
                                strSql += ", @pPRCS_NAME ='" + Ds.Tables[0].Rows[j][13].ToString().Trim() + "' ";  //공정명	
                                strSql += ", @pRENT_PEEX =" + SystemBase.Validation.Decimal_Data(Ds.Tables[0].Rows[j][14].ToString().Trim(), ",") + " ";  //지급임차료(A)
                                strSql += ", @pUNIT_CAMA ='" + Ds.Tables[0].Rows[j][15].ToString().Trim() + "' ";  //상각방법
                                strSql += ", @pPRCM_TAMT =" + SystemBase.Validation.Decimal_Data(Ds.Tables[0].Rows[j][16].ToString().Trim(), ",") + " ";  //총발생공수 or 총생산량(B)
                                // strSql += ", @pUNIT_RENT =" + SystemBase.Validation.Decimal_Data(Ds.Tables[0].Rows[j][17].ToString().Trim(),",") + " ";  //단위당지급임차료(C=A/B) : 자동계산
                                strSql += ", @pAPPL_WAMT =" + SystemBase.Validation.Decimal_Data(Ds.Tables[0].Rows[j][18].ToString().Trim(), ",") + " ";  //직접공수 or 생산물량(D)
                                // strSql += ", @pPAYMNT_RENT =" + SystemBase.Validation.Decimal_Data(Ds.Tables[0].Rows[j][19].ToString().Trim(),",") + " ";  //지급임차료(C+D)       : 자동계산
                                strSql += ", @pPROF_NUMB ='" + Ds.Tables[0].Rows[j][20].ToString().Trim() + "' ";  //증빙
                                strSql += ", @pNOTE ='" + Ds.Tables[0].Rows[j][21].ToString().Trim() + "' ";    //비고		

                                strSql += ", @pUSR_ID ='" + SystemBase.Base.gstrUserID + "' ";                                   //사용자

                                DataSet ds = SystemBase.DbOpen.NoTranDataSet(strSql);
                            }

                            if (Ds.Tables[0].Rows[j][0].ToString().Trim() == "") BlankRow++;
                            if (BlankRow >= 5) j = Ds.Tables[0].Rows.Count;

                        }
                        adapter.Dispose();
                        command.Dispose();
                        #endregion

                        #region 22.외주가공비(원화)
                        // 제출기준정보의 제출업체와 기준연월 정보
                        strSql2 = " usp_DAA003외주가공비원화 ";
                        strSql2 += "  @pTYPE = 'I1'";
                        strSql2 += ", @pMNUF_CODE = '" + cboM_MNUF_CODE.SelectedValue + "'";  //제출업체
                        strSql2 += ", @pSTD_YRMON ='" + SystemBase.Validation.C1DataEdit_ReadFormat(dtM_STD_YRMON.Value.ToString(), "YYYYMM") + "' ";  //기준연월
                        strSql2 += ", @pUSR_ID ='" + SystemBase.Base.gstrUserID + "' ";                    //사용자
                        Ds2 = SystemBase.DbOpen.NoTranDataSet(strSql2);

                        #endregion

                        #region 23.외주가공비(외화)
                        // 제출기준정보의 제출업체와 기준연월 정보
                        strSql2 = " usp_DAA003외주가공비외화 ";
                        strSql2 += "  @pTYPE = 'I1'";
                        strSql2 += ", @pMNUF_CODE = '" + cboM_MNUF_CODE.SelectedValue + "'";  //제출업체
                        strSql2 += ", @pSTD_YRMON ='" + SystemBase.Validation.C1DataEdit_ReadFormat(dtM_STD_YRMON.Value.ToString(), "YYYYMM") + "' ";  //기준연월
                        strSql2 += ", @pUSR_ID ='" + SystemBase.Base.gstrUserID + "' ";                    //사용자
                        Ds2 = SystemBase.DbOpen.NoTranDataSet(strSql2);

                        #endregion

                        #region 24.상각대상경비
                        BlankRow = 0;
                        command = new OleDbCommand(string.Format("SELECT * FROM [상각대상경비$]"), connection);
                        adapter = new OleDbDataAdapter(command);
                        adapter.SelectCommand = command;
                        Ds = new DataSet();
                        adapter.Fill(Ds);
                        for (int j = 0; j < Ds.Tables[0].Rows.Count; j++)
                        {
                            if (j > 4 && Ds.Tables[0].Rows[j][0].ToString().Trim() != "")
                            {
                                string strSql = " usp_DAA003상각대상경비 ";
                                strSql += "  @pTYPE = 'I1'";
                                strSql += ", @pREGE_SNUM ='" + Ds.Tables[0].Rows[j][0].ToString().Trim() + "' ";  //등록순번
                                strSql += ", @pORDR_YEAR ='" + Ds.Tables[0].Rows[j][1].ToString().Trim() + "' ";  //요구년도
                                strSql += ", @pDCSN_NUMB ='" + Ds.Tables[0].Rows[j][2].ToString().Trim() + "' ";  //판단번호
                                strSql += ", @pCALC_DEGR ='" + Ds.Tables[0].Rows[j][3].ToString().Trim() + "' ";  //제출차수
                                strSql += ", @pDPRT_CODE ='" + Ds.Tables[0].Rows[j][4].ToString().Trim() + "' ";  //구매부서
                                strSql += ", @pCTMF_CODE ='" + Ds.Tables[0].Rows[j][5].ToString().Trim() + "' ";  //계약업체코드
                                strSql += ", @pMNUF_CODE = '" + Ds.Tables[0].Rows[j][6].ToString().Trim() + "'";  //제출업체
                                strSql += ", @pSTD_YRMON ='" + Ds.Tables[0].Rows[j][7].ToString().Trim() + "' ";  //기준연월
                                strSql += ", @pNIIN ='" + Ds.Tables[0].Rows[j][8].ToString().Trim() + "' ";       //재고번호
                                strSql += ", @pUNIT ='" + Ds.Tables[0].Rows[j][9].ToString().Trim() + "' ";       //단위
                                strSql += ", @pDMST_ITNB ='" + Ds.Tables[0].Rows[j][10].ToString().Trim() + "' ";  //내자항목번호

                                strSql += ", @pADDX_DIVS ='" + Ds.Tables[0].Rows[j][11].ToString().Trim() + "' ";  //비목구분
                                strSql += ", @pOCRC_DATE ='" + Ds.Tables[0].Rows[j][12].ToString().Trim() + "' ";  //발생일자
                                strSql += ", @pOCIT_NAME ='" + Ds.Tables[0].Rows[j][13].ToString().Trim() + "' ";  //세부항목명
                                strSql += ", @pOCRC_AMNT =" + SystemBase.Validation.Decimal_Data(Ds.Tables[0].Rows[j][14].ToString().Trim(), ",") + " ";  //발생금액
                                strSql += ", @pDPRC_YEAR ='" + Ds.Tables[0].Rows[j][15].ToString().Trim() + "' ";  //상각년수
                                //strSql += ", @pDPRC_AMNT ='" + Ds.Tables[0].Rows[j][16].ToString().Trim() + "' ";  //상각대상금액 : 자동계산필드
                                strSql += ", @pSTND_QNTY =" + SystemBase.Validation.Decimal_Data(Ds.Tables[0].Rows[j][17].ToString().Trim(), ",") + " ";  //배부기준수량
                                //strSql += ", @pCOST_AMNT ='" + Ds.Tables[0].Rows[j][18].ToString().Trim() + "' ";  //경비	    : 자동계산필드
                                strSql += ", @pPROF_NUMB ='" + Ds.Tables[0].Rows[j][19].ToString().Trim() + "' ";  //증빙
                                strSql += ", @pNOTE ='" + Ds.Tables[0].Rows[j][20].ToString().Trim() + "' ";       //비고		

                                strSql += ", @pUSR_ID ='" + SystemBase.Base.gstrUserID + "' ";                                   //사용자

                                DataSet ds = SystemBase.DbOpen.NoTranDataSet(strSql);
                            }

                            if (Ds.Tables[0].Rows[j][0].ToString().Trim() == "") BlankRow++;
                            if (BlankRow >= 5) j = Ds.Tables[0].Rows.Count;

                        }
                        adapter.Dispose();
                        command.Dispose();
                        #endregion

                        #region 25.기타경비
                        BlankRow = 0;
                        command = new OleDbCommand(string.Format("SELECT * FROM [기타경비$]"), connection);
                        adapter = new OleDbDataAdapter(command);
                        adapter.SelectCommand = command;
                        Ds = new DataSet();
                        adapter.Fill(Ds);
                        for (int j = 0; j < Ds.Tables[0].Rows.Count; j++)
                        {
                            if (j > 4 && Ds.Tables[0].Rows[j][0].ToString().Trim() != "")
                            {
                                string strSql = " usp_DAA003기타경비 ";
                                strSql += "  @pTYPE = 'I1'";
                                strSql += ", @pREGE_SNUM =" + Ds.Tables[0].Rows[j][0].ToString().Trim() + "";         //등록순번
                                strSql += ", @pORDR_YEAR ='" + Ds.Tables[0].Rows[j][1].ToString().Trim() + "'";       //지시연도
                                strSql += ", @pDCSN_NUMB ='" + Ds.Tables[0].Rows[j][2].ToString().Trim() + "'";       //판단번호
                                strSql += ", @pCALC_DEGR ='" + Ds.Tables[0].Rows[j][3].ToString().Trim() + "'";       //제출차수
                                strSql += ", @pDPRT_CODE ='" + Ds.Tables[0].Rows[j][4].ToString().Trim() + "'";       //구매부서
                                strSql += ", @pCTMF_CODE ='" + Ds.Tables[0].Rows[j][5].ToString().Trim() + "'";       //계약업체코드
                                strSql += ", @pMNUF_CODE = '" + Ds.Tables[0].Rows[j][6].ToString().Trim() + "'";      //제출업체
                                strSql += ", @pSTD_YRMON ='" + Ds.Tables[0].Rows[j][7].ToString().Trim() + "'";       //기준연월
                                strSql += ", @pNIIN ='" + Ds.Tables[0].Rows[j][8].ToString().Trim() + "'";            //재고번호
                                strSql += ", @pUNIT ='" + Ds.Tables[0].Rows[j][9].ToString().Trim() + "'";            //단위
                                strSql += ", @pDMST_ITNB ='" + Ds.Tables[0].Rows[j][10].ToString().Trim() + "'";      //내자항목번호
                                strSql += ", @pADDX_DIVS ='" + Ds.Tables[0].Rows[j][11].ToString().Trim() + "'";       //비목구분
                                strSql += ", @pUNIT_CAMA ='" + Ds.Tables[0].Rows[j][12].ToString().Trim() + "'";       //상각방법
                                strSql += ", @pOCRC_DATE ='" + Ds.Tables[0].Rows[j][13].ToString().Trim() + "'";       //발생일자
                                strSql += ", @pOCIT_NAME ='" + SystemBase.Validation.String_Data(Ds.Tables[0].Rows[j][14].ToString().Trim(), "'") + "'";       //발생항목명
                                strSql += ", @pOCRC_AMNT =" + SystemBase.Validation.Decimal_Data(Ds.Tables[0].Rows[j][15].ToString().Trim(), ",") + "";       //발생금액
                                strSql += ", @pTOTL_TIME =" + SystemBase.Validation.Decimal_Data(Ds.Tables[0].Rows[j][16].ToString().Trim(), ",") + "";       //총작업시간
                                strSql += ", @pSTND_QNTY =" + SystemBase.Validation.Decimal_Data(Ds.Tables[0].Rows[j][17].ToString().Trim(), ",") + "";       //배부기준수량
                                strSql += ", @pPROF_NUMB ='" + Ds.Tables[0].Rows[j][19].ToString().Trim() + "'";       //증빙
                                strSql += ", @pNOTE ='" + Ds.Tables[0].Rows[j][20].ToString().Trim() + "'";            //비고

                                strSql += ", @pUSR_ID ='" + SystemBase.Base.gstrUserID + "' ";                        //사용자

                                DataSet ds = SystemBase.DbOpen.NoTranDataSet(strSql);
                            }
                            if (Ds.Tables[0].Rows[j][0].ToString().Trim() == "") BlankRow++;
                            if (BlankRow >= 5) j = Ds.Tables[0].Rows.Count;
                        }
                        adapter.Dispose();
                        command.Dispose();
                        #endregion

                        #region 26.추가기타경비
                        BlankRow = 0;
                        command = new OleDbCommand(string.Format("SELECT * FROM [추가기타경비$]"), connection);
                        adapter = new OleDbDataAdapter(command);
                        adapter.SelectCommand = command;
                        Ds = new DataSet();
                        adapter.Fill(Ds);
                        for (int j = 0; j < Ds.Tables[0].Rows.Count; j++)
                        {
                            if (j > 4 && Ds.Tables[0].Rows[j][0].ToString().Trim() != "")
                            {
                                string strSql = " usp_DAA003기타추가경비 ";
                                strSql += "  @pTYPE = 'I1'";
                                strSql += ", @pREGE_SNUM ='" + Ds.Tables[0].Rows[j][0].ToString().Trim() + "' ";  //등록순번
                                strSql += ", @pORDR_YEAR ='" + Ds.Tables[0].Rows[j][1].ToString().Trim() + "' ";  //지시연도
                                strSql += ", @pDCSN_NUMB ='" + Ds.Tables[0].Rows[j][2].ToString().Trim() + "' ";  //판단번호
                                strSql += ", @pCALC_DEGR ='" + Ds.Tables[0].Rows[j][3].ToString().Trim() + "' ";  //차수
                                strSql += ", @pDPRT_CODE ='" + Ds.Tables[0].Rows[j][4].ToString().Trim() + "' ";  //구매부서코드
                                strSql += ", @pCTMF_CODE ='" + Ds.Tables[0].Rows[j][5].ToString().Trim() + "' ";  //조달업체코드
                                strSql += ", @pMNUF_CODE = '" + Ds.Tables[0].Rows[j][6].ToString().Trim() + "'";  //제출업체코드
                                strSql += ", @pSTD_YRMON ='" + Ds.Tables[0].Rows[j][7].ToString().Trim() + "' ";  //기준연월
                                strSql += ", @pNIIN ='" + Ds.Tables[0].Rows[j][8].ToString().Trim() + "' ";       //재고번호
                                strSql += ", @pUNIT ='" + Ds.Tables[0].Rows[j][9].ToString().Trim() + "' ";       //단위
                                strSql += ", @pDMST_ITNB ='" + Ds.Tables[0].Rows[j][10].ToString().Trim() + "' "; //항목

                                strSql += ", @pADDX_DIVS ='" + Ds.Tables[0].Rows[j][11].ToString().Trim() + "' "; //비목명
                                strSql += ", @pUNIT_CAMA ='" + Ds.Tables[0].Rows[j][12].ToString().Trim() + "' "; //상각방법
                                strSql += ", @pOCRC_DATE ='" + Ds.Tables[0].Rows[j][13].ToString().Trim() + "' "; //발생일자
                                strSql += ", @pOCIT_NAME ='" + Ds.Tables[0].Rows[j][14].ToString().Trim() + "' "; //발생항목명
                                strSql += ", @pOCRC_AMNT =" + SystemBase.Validation.Decimal_Data(Ds.Tables[0].Rows[j][15].ToString().Trim(), ","); //발생금액
                                strSql += ", @pTOTL_TIME =" + SystemBase.Validation.Decimal_Data(Ds.Tables[0].Rows[j][16].ToString().Trim(), ","); //총작업시간
                                strSql += ", @pSTND_QNTY =" + SystemBase.Validation.Decimal_Data(Ds.Tables[0].Rows[j][17].ToString().Trim(), ","); //배부기준량
                                strSql += ", @pCOST_AMNT =" + SystemBase.Validation.Decimal_Data(Ds.Tables[0].Rows[j][18].ToString().Trim(), ","); //경비:자동계산필드                                
                                strSql += ", @pPROF_NUMB ='" + Ds.Tables[0].Rows[j][19].ToString().Trim() + "' ";  //증빙		
                                strSql += ", @pNOTE ='" + Ds.Tables[0].Rows[j][20].ToString().Trim() + "' ";       //비고	

                                strSql += ", @pUSR_ID ='" + SystemBase.Base.gstrUserID + "' ";                     //사용자

                                DataSet ds = SystemBase.DbOpen.NoTranDataSet(strSql);
                            }

                            if (Ds.Tables[0].Rows[j][0].ToString().Trim() == "") BlankRow++;
                            if (BlankRow >= 5) j = Ds.Tables[0].Rows.Count;

                        }
                        adapter.Dispose();
                        command.Dispose();
                        #endregion

                        #region 27.순매출액경비(원화)
                        BlankRow = 0;
                        command = new OleDbCommand(string.Format("SELECT * FROM [순매출액경비(원화)$]"), connection);
                        adapter = new OleDbDataAdapter(command);
                        adapter.SelectCommand = command;
                        Ds = new DataSet();
                        adapter.Fill(Ds);
                        for (int j = 0; j < Ds.Tables[0].Rows.Count; j++)
                        {
                            if (j > 4 && Ds.Tables[0].Rows[j][0].ToString().Trim() != "")
                            {
                                string strSql = " usp_DAA003순매출액경비원화 ";
                                strSql += "  @pTYPE = 'I1'";
                                strSql += ", @pREGE_SNUM ='" + Ds.Tables[0].Rows[j][0].ToString().Trim() + "' ";  //등록순번
                                strSql += ", @pORDR_YEAR ='" + Ds.Tables[0].Rows[j][1].ToString().Trim() + "' ";  //지시연도
                                strSql += ", @pDCSN_NUMB ='" + Ds.Tables[0].Rows[j][2].ToString().Trim() + "' ";  //판단번호
                                strSql += ", @pCALC_DEGR ='" + Ds.Tables[0].Rows[j][3].ToString().Trim() + "' ";  //차수
                                strSql += ", @pDPRT_CODE ='" + Ds.Tables[0].Rows[j][4].ToString().Trim() + "' ";  //구매부서코드
                                strSql += ", @pCTMF_CODE ='" + Ds.Tables[0].Rows[j][5].ToString().Trim() + "' ";  //조달업체코드
                                strSql += ", @pMNUF_CODE = '" + Ds.Tables[0].Rows[j][6].ToString().Trim() + "'";  //제출업체코드
                                strSql += ", @pSTD_YRMON ='" + Ds.Tables[0].Rows[j][7].ToString().Trim() + "' ";  //기준연월
                                strSql += ", @pNIIN ='" + Ds.Tables[0].Rows[j][8].ToString().Trim() + "' ";       //재고번호
                                strSql += ", @pUNIT ='" + Ds.Tables[0].Rows[j][9].ToString().Trim() + "' ";       //단위
                                strSql += ", @pDMST_ITNB ='" + Ds.Tables[0].Rows[j][10].ToString().Trim() + "' "; //항목

                                strSql += ", @pADDX_DIVS ='" + Ds.Tables[0].Rows[j][11].ToString().Trim() + "' "; //비목명
                                strSql += ", @pTSAL_AMNT =" + SystemBase.Validation.Decimal_Data(Ds.Tables[0].Rows[j][12].ToString().Trim(), ","); //총매출액
                                strSql += ", @pDDCT_TAMT =" + SystemBase.Validation.Decimal_Data(Ds.Tables[0].Rows[j][13].ToString().Trim(), ","); //공제총액
                                strSql += ", @pTARI =" + SystemBase.Validation.Decimal_Data(Ds.Tables[0].Rows[j][14].ToString().Trim(), "%"); //요율
                                strSql += ", @pCOST_AMNT =" + SystemBase.Validation.Decimal_Data(Ds.Tables[0].Rows[j][15].ToString().Trim(), ","); //경비
                                strSql += ", @pPROF_NUMB ='" + Ds.Tables[0].Rows[j][16].ToString().Trim() + "' ";  //증빙		
                                strSql += ", @pNOTE ='" + Ds.Tables[0].Rows[j][17].ToString().Trim() + "' ";       //비고	

                                strSql += ", @pUSR_ID ='" + SystemBase.Base.gstrUserID + "' ";                     //사용자

                                DataSet ds = SystemBase.DbOpen.NoTranDataSet(strSql);
                            }

                            if (Ds.Tables[0].Rows[j][0].ToString().Trim() == "") BlankRow++;
                            if (BlankRow >= 5) j = Ds.Tables[0].Rows.Count;

                        }
                        adapter.Dispose();
                        command.Dispose();
                        #endregion

                        #region 28.순매출액경비(외화)
                        BlankRow = 0;
                        command = new OleDbCommand(string.Format("SELECT * FROM [순매출액경비(외화)$]"), connection);
                        adapter = new OleDbDataAdapter(command);
                        adapter.SelectCommand = command;
                        Ds = new DataSet();
                        adapter.Fill(Ds);
                        for (int j = 0; j < Ds.Tables[0].Rows.Count; j++)
                        {
                            if (j > 4 && Ds.Tables[0].Rows[j][0].ToString().Trim() != "")
                            {
                                string strSql = " usp_DAA003순매출액경비외화 ";
                                strSql += "  @pTYPE = 'I1'";
                                strSql += ", @pREGE_SNUM =" + Ds.Tables[0].Rows[j][0].ToString().Trim() + "";         //등록순번
                                strSql += ", @pORDR_YEAR ='" + Ds.Tables[0].Rows[j][1].ToString().Trim() + "'";       //지시연도
                                strSql += ", @pDCSN_NUMB ='" + Ds.Tables[0].Rows[j][2].ToString().Trim() + "'";       //판단번호
                                strSql += ", @pCALC_DEGR ='" + Ds.Tables[0].Rows[j][3].ToString().Trim() + "'";       //제출차수
                                strSql += ", @pDPRT_CODE ='" + Ds.Tables[0].Rows[j][4].ToString().Trim() + "'";       //구매부서
                                strSql += ", @pCTMF_CODE ='" + Ds.Tables[0].Rows[j][5].ToString().Trim() + "'";       //계약업체코드
                                strSql += ", @pMNUF_CODE = '" + Ds.Tables[0].Rows[j][6].ToString().Trim() + "'";      //제출업체
                                strSql += ", @pSTD_YRMON ='" + Ds.Tables[0].Rows[j][7].ToString().Trim() + "'";       //기준연월
                                strSql += ", @pNIIN ='" + Ds.Tables[0].Rows[j][8].ToString().Trim() + "'";            //재고번호
                                strSql += ", @pUNIT ='" + Ds.Tables[0].Rows[j][9].ToString().Trim() + "'";            //단위
                                strSql += ", @pDMST_ITNB ='" + Ds.Tables[0].Rows[j][10].ToString().Trim() + "'";      //내자항목번호
                                strSql += ", @pADDX_DIVS ='" + Ds.Tables[0].Rows[j][11].ToString().Trim() + "'";      //비목구분
                                strSql += ", @pTSAL_INCM =" + SystemBase.Validation.Decimal_Data(Ds.Tables[0].Rows[j][12].ToString().Trim(), ",") + "";      //부품판매가
                                strSql += ", @pDDCT_TAMT =" + SystemBase.Validation.Decimal_Data(Ds.Tables[0].Rows[j][13].ToString().Trim(), ",") + "";      //공제총액
                                strSql += ", @pTARI =" + SystemBase.Validation.Decimal_Data(Ds.Tables[0].Rows[j][14].ToString().Trim(), "%") + "";           //요율(%)
                                strSql += ", @pCURC_UNIT ='" + Ds.Tables[0].Rows[j][15].ToString().Trim() + "'";      //통화코드
                                strSql += ", @pAPLC_DATE ='" + Ds.Tables[0].Rows[j][16].ToString().Trim() + "'";      //적용일자
                                strSql += ", @pAPLC_ECRT =" + SystemBase.Validation.Decimal_Data(Ds.Tables[0].Rows[j][17].ToString().Trim(), "%") + "";      //적용환율
                                strSql += ", @pCOST_AMNT =" + SystemBase.Validation.Decimal_Data(Ds.Tables[0].Rows[j][18].ToString().Trim(), ",") + "";      //경비
                                strSql += ", @pCWON_EXRT =" + SystemBase.Validation.Decimal_Data(Ds.Tables[0].Rows[j][19].ToString().Trim(), "%") + "";      //관세환산율
                                strSql += ", @pCWON_UPRC =" + SystemBase.Validation.Decimal_Data(Ds.Tables[0].Rows[j][20].ToString().Trim(), ",") + "";      //관세단가
                                strSql += ", @pCUST_RATE =" + SystemBase.Validation.Decimal_Data(Ds.Tables[0].Rows[j][21].ToString().Trim(), "%") + "";      //관세율
                                strSql += ", @pGAMX_RATE =" + SystemBase.Validation.Decimal_Data(Ds.Tables[0].Rows[j][22].ToString().Trim(), "%") + "";      //감면율
                                strSql += ", @pAGRI_RATE =" + SystemBase.Validation.Decimal_Data(Ds.Tables[0].Rows[j][23].ToString().Trim(), "%") + "";      //농특세율
                                strSql += ", @pCUST_AMNT =" + SystemBase.Validation.Decimal_Data(Ds.Tables[0].Rows[j][24].ToString().Trim(), ",") + "";      //관세금액
                                strSql += ", @pPROF_NUMB ='" + Ds.Tables[0].Rows[j][25].ToString().Trim() + "'";      //증빙
                                strSql += ", @pNOTE ='" + Ds.Tables[0].Rows[j][26].ToString().Trim() + "'";           //비고

                                strSql += ", @pUSR_ID ='" + SystemBase.Base.gstrUserID + "' ";                        //사용자

                                DataSet ds = SystemBase.DbOpen.NoTranDataSet(strSql);
                            }
                            if (Ds.Tables[0].Rows[j][0].ToString().Trim() == "") BlankRow++;
                            if (BlankRow >= 5) j = Ds.Tables[0].Rows.Count;
                        }
                        adapter.Dispose();
                        command.Dispose();
                        #endregion

                        #region 28.순매출액경비(공제내역)
                        BlankRow = 0;
                        command = new OleDbCommand(string.Format("SELECT * FROM [순매출액경비(공제내역)$]"), connection);
                        adapter = new OleDbDataAdapter(command);
                        adapter.SelectCommand = command;
                        Ds = new DataSet();
                        adapter.Fill(Ds);
                        for (int j = 0; j < Ds.Tables[0].Rows.Count; j++)
                        {
                            if (j > 4 && Ds.Tables[0].Rows[j][0].ToString().Trim() != "")
                            {
                                string strSql = " usp_DAA003순매출액경비공제내역 ";
                                strSql += "  @pTYPE = 'I1'";
                                strSql += ", @pREGE_SNUM =" + Ds.Tables[0].Rows[j][0].ToString().Trim() + "";         //등록순번
                                strSql += ", @pORDR_YEAR ='" + Ds.Tables[0].Rows[j][1].ToString().Trim() + "'";       //지시연도
                                strSql += ", @pDCSN_NUMB ='" + Ds.Tables[0].Rows[j][2].ToString().Trim() + "'";       //판단번호
                                strSql += ", @pCALC_DEGR ='" + Ds.Tables[0].Rows[j][3].ToString().Trim() + "'";       //제출차수
                                strSql += ", @pDPRT_CODE ='" + Ds.Tables[0].Rows[j][4].ToString().Trim() + "'";       //구매부서
                                strSql += ", @pCTMF_CODE ='" + Ds.Tables[0].Rows[j][5].ToString().Trim() + "'";       //계약업체코드
                                strSql += ", @pMNUF_CODE = '" + Ds.Tables[0].Rows[j][6].ToString().Trim() + "'";      //제출업체
                                strSql += ", @pSTD_YRMON ='" + Ds.Tables[0].Rows[j][7].ToString().Trim() + "'";       //기준연월
                                strSql += ", @pNIIN ='" + Ds.Tables[0].Rows[j][8].ToString().Trim() + "'";            //재고번호
                                strSql += ", @pUNIT ='" + Ds.Tables[0].Rows[j][9].ToString().Trim() + "'";            //단위
                                strSql += ", @pDMST_ITNB ='" + Ds.Tables[0].Rows[j][10].ToString().Trim() + "'";      //내자항목번호
                                strSql += ", @pDMST_DIVS ='" + Ds.Tables[0].Rows[j][11].ToString().Trim() + "'";      //매출구분
                                strSql += ", @pADDX_DIVS ='" + Ds.Tables[0].Rows[j][12].ToString().Trim() + "'";      //비목
                                strSql += ", @pDDIT_NAME ='" + SystemBase.Validation.String_Data(Ds.Tables[0].Rows[j][13].ToString().Trim(), "'") + "'"; //공제항목명
                                strSql += ", @pDDCT_AMNT =" + SystemBase.Validation.Decimal_Data(Ds.Tables[0].Rows[j][14].ToString().Trim(), ",") + "";      //공제금액
                                strSql += ", @pNOTE ='" + Ds.Tables[0].Rows[j][15].ToString().Trim() + "'";      //비고

                                strSql += ", @pUSR_ID ='" + SystemBase.Base.gstrUserID + "' ";                        //사용자

                                DataSet ds = SystemBase.DbOpen.NoTranDataSet(strSql);
                            }
                            if (Ds.Tables[0].Rows[j][0].ToString().Trim() == "") BlankRow++;
                            if (BlankRow >= 5) j = Ds.Tables[0].Rows.Count;
                        }
                        adapter.Dispose();
                        command.Dispose();
                        #endregion


                        connection.Close();
                        this.Cursor = System.Windows.Forms.Cursors.Default;
                        //업로드 처리 완료 하였습니다.
                        MessageBox.Show(SystemBase.Base.MessageRtn("SY029"), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }

                Detail_List();

            }
            catch (Exception f)
            {
                this.Cursor = Cursors.Default;
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(f.Message, SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region fpButtonClick
        protected override void fpButtonClick(int Row, int Column)
        {
            try
            {
                if (strESB_BIZNES_TRNSTN_ID == "")  //선택된 자료가 없으면 처리 안함.
                {
                    //선택된 내용이 없습니다.
                    MessageBox.Show(SystemBase.Base.MessageRtn("SY028"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string strLOAD_TYPE_NM = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "원가자료명")].Text.ToString(); //원가자료명
                string strLOAD_TYPE_CD = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "순번")].Text.ToString(); ;     //자료기준

                #region 자료보기
                if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "자료보기"))
                {
                    if (strLOAD_TYPE_CD == "1")  //BOM정보조회
                    {
                        DAA003P01 pu = new DAA003P01(strORDR_YEAR, strDCSN_NUMB, strCALC_DEGR, strDPRT_CODE, strCTMP_CODE, strMNUF_CODE,
                                                   strSTD_YRMON, strNIIN, strUNIT, strDMST_ITNB, strKeyGroup, this.Name, "1", strESB_BIZNES_TRNSTN_ID);
                        pu.Owner = this;
                        pu.Show();
                    }
                    else if (strLOAD_TYPE_CD == "2") //입고이력 및 외주단가조회
                    {
                        DAA003P02 pu = new DAA003P02(strORDR_YEAR, strDCSN_NUMB, strCALC_DEGR, strDPRT_CODE, strCTMP_CODE, strMNUF_CODE,
                                                   strSTD_YRMON, strNIIN, strUNIT, strDMST_ITNB, strKeyGroup, this.Name, "2", strESB_BIZNES_TRNSTN_ID);
                        pu.Owner = this;
                        pu.Show();
                    }
                    else if (strLOAD_TYPE_CD == "3") //단가정보
                    {
                        DAA003P03 pu = new DAA003P03(strORDR_YEAR, strDCSN_NUMB, strCALC_DEGR, strDPRT_CODE, strCTMP_CODE, strMNUF_CODE,
                                                   strSTD_YRMON, strNIIN, strUNIT, strDMST_ITNB, strKeyGroup, this.Name, "3", strESB_BIZNES_TRNSTN_ID);
                        pu.Owner = this;
                        pu.Show();
                    }
                    else if (strLOAD_TYPE_CD == "4") //투입이력
                    {
                        DAA003P04 pu = new DAA003P04(strORDR_YEAR, strDCSN_NUMB, strCALC_DEGR, strDPRT_CODE, strCTMP_CODE, strMNUF_CODE,
                                                   strSTD_YRMON, strNIIN, strUNIT, strDMST_ITNB, strKeyGroup, this.Name, "4", strESB_BIZNES_TRNSTN_ID);
                        pu.Owner = this;
                        pu.Show();
                    }
                    else if (strLOAD_TYPE_CD == "5") //수입내역
                    {
                        DAA003P05 pu = new DAA003P05(strORDR_YEAR, strDCSN_NUMB, strCALC_DEGR, strDPRT_CODE, strCTMP_CODE, strMNUF_CODE,
                                                   strSTD_YRMON, strNIIN, strUNIT, strDMST_ITNB, strKeyGroup, this.Name, "5", strESB_BIZNES_TRNSTN_ID);
                        pu.Owner = this;
                        pu.Show();
                    }
                    else if (strLOAD_TYPE_CD == "6") //환산율및 환산가
                    {
                        DAA003P06 pu = new DAA003P06(strORDR_YEAR, strDCSN_NUMB, strCALC_DEGR, strDPRT_CODE, strCTMP_CODE, strMNUF_CODE,
                                                   strSTD_YRMON, strNIIN, strUNIT, strDMST_ITNB, strKeyGroup, this.Name, "6", strESB_BIZNES_TRNSTN_ID);
                        pu.Owner = this;
                        pu.Show();
                    }
                    else if (strLOAD_TYPE_CD == "7") //국내재료비
                    {
                        DAA003P07 pu = new DAA003P07(strORDR_YEAR, strDCSN_NUMB, strCALC_DEGR, strDPRT_CODE, strCTMP_CODE, strMNUF_CODE,
                                                   strSTD_YRMON, strNIIN, strUNIT, strDMST_ITNB, strKeyGroup, this.Name, "7", strESB_BIZNES_TRNSTN_ID);
                        pu.Owner = this;
                        pu.Show();
                    }
                    else if (strLOAD_TYPE_CD == "8") //반제품비
                    {
                        DAA003P08 pu = new DAA003P08(strORDR_YEAR, strDCSN_NUMB, strCALC_DEGR, strDPRT_CODE, strCTMP_CODE, strMNUF_CODE,
                                                   strSTD_YRMON, strNIIN, strUNIT, strDMST_ITNB, strKeyGroup, this.Name, "8", strESB_BIZNES_TRNSTN_ID);
                        pu.Owner = this;
                        pu.Show();
                    }
                    else if (strLOAD_TYPE_CD == "9") //국내구입부품비
                    {
                        DAA003P09 pu = new DAA003P09(strORDR_YEAR, strDCSN_NUMB, strCALC_DEGR, strDPRT_CODE, strCTMP_CODE, strMNUF_CODE,
                                                   strSTD_YRMON, strNIIN, strUNIT, strDMST_ITNB, strKeyGroup, this.Name, "9", strESB_BIZNES_TRNSTN_ID);
                        pu.Owner = this;
                        pu.Show();
                    }
                    else if (strLOAD_TYPE_CD == "10") //국내구입부품비(방산)
                    {
                        DAA003P10 pu = new DAA003P10(strORDR_YEAR, strDCSN_NUMB, strCALC_DEGR, strDPRT_CODE, strCTMP_CODE, strMNUF_CODE,
                                                   strSTD_YRMON, strNIIN, strUNIT, strDMST_ITNB, strKeyGroup, this.Name, "10", strESB_BIZNES_TRNSTN_ID);
                        pu.Owner = this;
                        pu.Show();
                    }
                    else if (strLOAD_TYPE_CD == "11") //수입재료비
                    {
                        DAA003P11 pu = new DAA003P11(strORDR_YEAR, strDCSN_NUMB, strCALC_DEGR, strDPRT_CODE, strCTMP_CODE, strMNUF_CODE,
                                                   strSTD_YRMON, strNIIN, strUNIT, strDMST_ITNB, strKeyGroup, this.Name, "11", strESB_BIZNES_TRNSTN_ID);
                        pu.Owner = this;
                        pu.Show();
                    }
                    else if (strLOAD_TYPE_CD == "12") //수입부품비
                    {
                        DAA003P12 pu = new DAA003P12(strORDR_YEAR, strDCSN_NUMB, strCALC_DEGR, strDPRT_CODE, strCTMP_CODE, strMNUF_CODE,
                                                   strSTD_YRMON, strNIIN, strUNIT, strDMST_ITNB, strKeyGroup, this.Name, "12", strESB_BIZNES_TRNSTN_ID);
                        pu.Owner = this;
                        pu.Show();
                    }
                    else if (strLOAD_TYPE_CD == "13") //포장재료비
                    {
                        DAA003P13 pu = new DAA003P13(strORDR_YEAR, strDCSN_NUMB, strCALC_DEGR, strDPRT_CODE, strCTMP_CODE, strMNUF_CODE,
                                                   strSTD_YRMON, strNIIN, strUNIT, strDMST_ITNB, strKeyGroup, this.Name, "13", strESB_BIZNES_TRNSTN_ID);
                        pu.Owner = this;
                        pu.Show();
                    }
                    else if (strLOAD_TYPE_CD == "14") //설물부품
                    {
                        DAA003P14 pu = new DAA003P14(strORDR_YEAR, strDCSN_NUMB, strCALC_DEGR, strDPRT_CODE, strCTMP_CODE, strMNUF_CODE,
                                                   strSTD_YRMON, strNIIN, strUNIT, strDMST_ITNB, strKeyGroup, this.Name, "14", strESB_BIZNES_TRNSTN_ID);
                        pu.Owner = this;
                        pu.Show();
                    }
                    else if (strLOAD_TYPE_CD == "15") //설물발생액
                    {
                        DAA003P15 pu = new DAA003P15(strORDR_YEAR, strDCSN_NUMB, strCALC_DEGR, strDPRT_CODE, strCTMP_CODE, strMNUF_CODE,
                                                   strSTD_YRMON, strNIIN, strUNIT, strDMST_ITNB, strKeyGroup, this.Name, "15", strESB_BIZNES_TRNSTN_ID);
                        pu.Owner = this;
                        pu.Show();
                    }
                    else if (strLOAD_TYPE_CD == "16") //관급재료비
                    {
                        DAA003P16 pu = new DAA003P16(strORDR_YEAR, strDCSN_NUMB, strCALC_DEGR, strDPRT_CODE, strCTMP_CODE, strMNUF_CODE,
                                                   strSTD_YRMON, strNIIN, strUNIT, strDMST_ITNB, strKeyGroup, this.Name, "16", strESB_BIZNES_TRNSTN_ID);
                        pu.Owner = this;
                        pu.Show();
                    }
                    else if (strLOAD_TYPE_CD == "17") //직접노무량
                    {
                        DAA003P17 pu = new DAA003P17(strORDR_YEAR, strDCSN_NUMB, strCALC_DEGR, strDPRT_CODE, strCTMP_CODE, strMNUF_CODE,
                                                   strSTD_YRMON, strNIIN, strUNIT, strDMST_ITNB, strKeyGroup, this.Name, "17", strESB_BIZNES_TRNSTN_ID);
                        pu.Owner = this;
                        pu.Show();
                    }
                    else if (strLOAD_TYPE_CD == "18") //단위공수계산
                    {
                        DAA003P18 pu = new DAA003P18(strORDR_YEAR, strDCSN_NUMB, strCALC_DEGR, strDPRT_CODE, strCTMP_CODE, strMNUF_CODE,
                                                   strSTD_YRMON, strNIIN, strUNIT, strDMST_ITNB, strKeyGroup, this.Name, "18", strESB_BIZNES_TRNSTN_ID);
                        pu.Owner = this;
                        pu.Show();
                    }
                    else if (strLOAD_TYPE_CD == "19") //직접노무비
                    {
                        DAA003P19 pu = new DAA003P19(strORDR_YEAR, strDCSN_NUMB, strCALC_DEGR, strDPRT_CODE, strCTMP_CODE, strMNUF_CODE,
                                                   strSTD_YRMON, strNIIN, strUNIT, strDMST_ITNB, strKeyGroup, this.Name, "19", strESB_BIZNES_TRNSTN_ID);
                        pu.Owner = this;
                        pu.Show();
                    }
                    else if (strLOAD_TYPE_CD == "20") //감가상각비(전용)
                    {
                        DAA003P20 pu = new DAA003P20(strORDR_YEAR, strDCSN_NUMB, strCALC_DEGR, strDPRT_CODE, strCTMP_CODE, strMNUF_CODE,
                                                   strSTD_YRMON, strNIIN, strUNIT, strDMST_ITNB, strKeyGroup, this.Name, "20", strESB_BIZNES_TRNSTN_ID);
                        pu.Owner = this;
                        pu.Show();
                    }
                    else if (strLOAD_TYPE_CD == "21") //지급임차료(전용)
                    {
                        DAA003P21 pu = new DAA003P21(strORDR_YEAR, strDCSN_NUMB, strCALC_DEGR, strDPRT_CODE, strCTMP_CODE, strMNUF_CODE,
                                                   strSTD_YRMON, strNIIN, strUNIT, strDMST_ITNB, strKeyGroup, this.Name, "21", strESB_BIZNES_TRNSTN_ID);
                        pu.Owner = this;
                        pu.Show();
                    }
                    else if (strLOAD_TYPE_CD == "22") //외주가공비(원화)
                    {
                        DAA003P22 pu = new DAA003P22(strORDR_YEAR, strDCSN_NUMB, strCALC_DEGR, strDPRT_CODE, strCTMP_CODE, strMNUF_CODE,
                                                   strSTD_YRMON, strNIIN, strUNIT, strDMST_ITNB, strKeyGroup, this.Name, "22", strESB_BIZNES_TRNSTN_ID);
                        pu.Owner = this;
                        pu.Show();
                    }
                    else if (strLOAD_TYPE_CD == "23") //외주가공비(외화)
                    {
                        DAA003P23 pu = new DAA003P23(strORDR_YEAR, strDCSN_NUMB, strCALC_DEGR, strDPRT_CODE, strCTMP_CODE, strMNUF_CODE,
                                                   strSTD_YRMON, strNIIN, strUNIT, strDMST_ITNB, strKeyGroup, this.Name, "23", strESB_BIZNES_TRNSTN_ID);
                        pu.Owner = this;
                        pu.Show();
                    }
                    else if (strLOAD_TYPE_CD == "24") //상각대상경비
                    {
                        DAA003P24 pu = new DAA003P24(strORDR_YEAR, strDCSN_NUMB, strCALC_DEGR, strDPRT_CODE, strCTMP_CODE, strMNUF_CODE,
                                                   strSTD_YRMON, strNIIN, strUNIT, strDMST_ITNB, strKeyGroup, this.Name, "24", strESB_BIZNES_TRNSTN_ID);
                        pu.Owner = this;
                        pu.Show();
                    }
                    else if (strLOAD_TYPE_CD == "25") //기타경비
                    {
                        DAA003P25 pu = new DAA003P25(strORDR_YEAR, strDCSN_NUMB, strCALC_DEGR, strDPRT_CODE, strCTMP_CODE, strMNUF_CODE,
                                                   strSTD_YRMON, strNIIN, strUNIT, strDMST_ITNB, strKeyGroup, this.Name, "25", strESB_BIZNES_TRNSTN_ID);
                        pu.Owner = this;
                        pu.Show();
                    }
                    else if (strLOAD_TYPE_CD == "26") //기타추가경비
                    {
                        DAA003P26 pu = new DAA003P26(strORDR_YEAR, strDCSN_NUMB, strCALC_DEGR, strDPRT_CODE, strCTMP_CODE, strMNUF_CODE,
                                                   strSTD_YRMON, strNIIN, strUNIT, strDMST_ITNB, strKeyGroup, this.Name, "26", strESB_BIZNES_TRNSTN_ID);
                        pu.Owner = this;
                        pu.Show();
                    }
                    else if (strLOAD_TYPE_CD == "27") //순매출액경비(원화)
                    {
                        DAA003P27 pu = new DAA003P27(strORDR_YEAR, strDCSN_NUMB, strCALC_DEGR, strDPRT_CODE, strCTMP_CODE, strMNUF_CODE,
                                                   strSTD_YRMON, strNIIN, strUNIT, strDMST_ITNB, strKeyGroup, this.Name, "27", strESB_BIZNES_TRNSTN_ID);
                        pu.Owner = this;
                        pu.Show();
                    }
                    else if (strLOAD_TYPE_CD == "28") //순매출액경비외화
                    {
                        DAA003P28 pu = new DAA003P28(strORDR_YEAR, strDCSN_NUMB, strCALC_DEGR, strDPRT_CODE, strCTMP_CODE, strMNUF_CODE,
                                                   strSTD_YRMON, strNIIN, strUNIT, strDMST_ITNB, strKeyGroup, this.Name, "28", strESB_BIZNES_TRNSTN_ID);
                        pu.Owner = this;
                        pu.Show();
                    }
                    else if (strLOAD_TYPE_CD == "29") //순매출액경비공제내역
                    {
                        DAA003P29 pu = new DAA003P29(strORDR_YEAR, strDCSN_NUMB, strCALC_DEGR, strDPRT_CODE, strCTMP_CODE, strMNUF_CODE,
                                                   strSTD_YRMON, strNIIN, strUNIT, strDMST_ITNB, strKeyGroup, this.Name, "29", strESB_BIZNES_TRNSTN_ID);
                        pu.Owner = this;
                        pu.Show();                        
                    }

                }
                #endregion

                #region 자료삭제
                if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "자료삭제"))
                {
                    string msg = SystemBase.Base.MessageRtn("SY010");
                    DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn(msg), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question); ;
                    if (dsMsg == DialogResult.Yes)
                    {
                        string ERRCode = "ER", MSGCode = "SY001";	//처리할 내용이 없습니다.
                        SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                        SqlCommand cmd = dbConn.CreateCommand();
                        SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                        string strSql = " usp_DAA003 ";
                        strSql += "  @pTYPE = 'D2' ";
                        strSql += ", @pKEY_GROUP = '" + strKeyGroup + "' ";
                        strSql += ", @pESB_BIZNES_TRNSTN_ID = '" + strESB_BIZNES_TRNSTN_ID + "' ";
                        strSql += ", @pLOAD_TYPE_CD = '" + strLOAD_TYPE_CD + "' ";

                        DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                        ERRCode = ds.Tables[0].Rows[0][0].ToString();
                        MSGCode = ds.Tables[0].Rows[0][1].ToString();
                        if (ERRCode == "OK")
                        {
                            Trans.Commit();
                            MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            Trans.Rollback();
                            MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        dbConn.Close();
                        Detail_List();
                    }
                }
                #endregion


            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY008", "그리드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }

        }
        #endregion

        #region Detail_List
        private void Detail_List()
        {
            try
            {
                //조회
                string strSql2 = " usp_DAA003 ";
                strSql2 += "  @pTYPE = 'S3' ";
                strSql2 += ", @pESB_BIZNES_TRNSTN_ID = '" + strESB_BIZNES_TRNSTN_ID + "'";
                strSql2 += ", @pKEY_GROUP = '" + strKeyGroup + "' ";
                strSql2 += ", @pMNUF_CODE = '" + strMNUF_CODE + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread1, strSql2, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0);
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region Excel Upload Error View
        private void btnM_ERROR_VIEW_Click(object sender, EventArgs e)
        {
            try
            {
                DAA003P30 pu = new DAA003P30(strKeyGroup);
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    // string[] Msgs = pu.ReturnVal;
                    //txtItemCd.Text = Msgs[2].ToString();
                    //txtItemNm.Text = Msgs[3].ToString();
                    //txtItemCd.Focus();
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region ReSet
        private void ReSet()
        {
            try
            {
                PreRow = -1;
                btnM_ERROR_VIEW.Enabled = false;
                btnM_ERROR_VIEW.ForeColor = Color.Black;
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 구매부서 ENTER 키처리
        private void txtH_DPRT_CODE_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    Dprt_Popup("N", "H");  // 엔터키, 헤더위치
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtM_DPRT_CODE_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    Dprt_Popup("N", "M");  // 엔터키, 마스터위치
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 단위버튼 ENTER 키처리
        private void txtM_UNIT_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    UNIT_Popup("N", "M");
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 구매부서 ENTER 키처리
        private void Dprt_Popup(string ButtonYn, string Location)
        {
            try
            {
                string[] strSearch = null;
                string strQuery = " usp_B_COMMON @pTYPE = 'COMM_POP', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' , @pSPEC1 = 'D007' ";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };

                if (ButtonYn == "Y")
                    strSearch = new string[] { };                     // 버튼 팝업은 무조건 초기화 해서 보여준다.                
                else
                    if (Location == "H")
                        strSearch = new string[] { txtH_DPRT_CODE.Text }; // ENTER키 값시는 해당 자료를 가지고 like 처리해서 보여줌                   
                    else
                        strSearch = new string[] { txtM_DPRT_CODE.Text }; // ENTER키 값시는 해당 자료를 가지고 like 처리해서 보여줌      

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("D0001", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "구매부서");
                pu.Width = 800;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    if (Location == "H")
                    {
                        txtH_DPRT_CODE.Value = Msgs[0].ToString();
                        txtH_DPRT_NAME.Value = Msgs[1].ToString();
                    }
                    else
                    {
                        txtM_DPRT_CODE.Value = Msgs[0].ToString();
                        txtM_DPRT_NAME.Value = Msgs[1].ToString();
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY008", "구매부서 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        private void UNIT_Popup(string ButtonYn, string Location)
        {
            try
            {
                string[] strSearch = null;
                string strQuery = " usp_B_COMMON @pTYPE = 'COMM_POP' , @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' , @pSPEC1 = 'Z005' ";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };

                if (ButtonYn == "Y")
                    strSearch = new string[] { }; // 버튼 팝업은 무조건 초기화 해서 보여준다.
                else
                    if (Location == "M")
                        strSearch = new string[] { txtM_UNIT.Text };


                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00029", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "단위");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());
                    if (Location == "M")
                    {
                        txtM_UNIT.Value = Msgs[0].ToString();
                        txtM_UNIT_NM.Value = Msgs[1].ToString();
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY008", "단위 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }

        }
        #endregion

        #region 버튼 이미지 처리        
        private void btnSend_Click(object sender, EventArgs e)
        {
            string ERRCode = "", MSGCode = "";

            SqlConnection dbConn = SystemBase.DbOpen.DBCON();
            SqlCommand cmd = dbConn.CreateCommand();
            SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox2))  //필수여부체크
            {
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("GA001"), SystemBase.Base.MessageRtn("Z0004"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dsMsg == DialogResult.Yes)
                {
                    try
                    {
                        string strPtype = "I1";
                        string strSql = " usp_DAA003_T01 ";
                        strSql += "  @pTYPE = '" + strPtype + "' ";
                        strSql += ", @pMNUF_CODE = '" + cboM_MNUF_CODE.SelectedValue + "'"; //제출업체
                        strSql += ", @pORDR_YEAR = '" + txtM_ORDR_YEAR.Text + "'";          //지시연도                  
                        strSql += ", @pDCSN_NUMB = '" + txtM_DCSN_NUMB.Text + "'";          //판단번호
                        strSql += ", @pSTD_YRMON = '" + SystemBase.Validation.C1DataEdit_ReadFormat(dtM_STD_YRMON.Value.ToString(), "YYYYMM") + "'"; //기준연월
                        strSql += ", @pCALC_DEGR = '" + txtM_CALC_DEGR.Text + "'";          //차수
                        strSql += ", @pDPRT_CODE = '" + txtM_DPRT_CODE.Text + "'";          //부서
                        strSql += ", @pCTMF_CODE = '" + cboH_CTMF_CODE.SelectedValue + "'"; //조달업체
                        strSql += ", @pIN_ID ='" + SystemBase.Base.gstrUserID + "' ";       //사용자

                        DataSet ds2 = SystemBase.DbOpen.NoTranDataSet(strSql);
                        ERRCode = ds2.Tables[0].Rows[0][0].ToString();
                        MSGCode = ds2.Tables[0].Rows[0][1].ToString();

                        if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                        Trans.Commit();
                    }
                    catch (Exception f)
                    {
                        SystemBase.Loggers.Log(this.Name, f.ToString());
                        Trans.Rollback();
                        MSGCode = "SY002";	//에러가 발생하여 데이터 처리가 취소되었습니다.
                    }
                Exit:
                    dbConn.Close();
                    this.Cursor = System.Windows.Forms.Cursors.Default;

                    if (ERRCode == "OK")
                    {
                        MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                        SearchExec();
                    }
                    else if (ERRCode == "ER")
                    {
                        MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
        }

        private void btnSend_MouseEnter(object sender, EventArgs e)
        {
            try
            {
                Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\Toolbar\uDocSend.gif");
                btnSend.Image = bitMap;
            }
            catch (Exception f)
            {
            }
        }

        private void btnSend_MouseLeave(object sender, EventArgs e)
        {
            try
            {
                Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\Toolbar\DocSend.gif");
                btnSend.Image = bitMap;
            }
            catch (Exception f)
            {
            }
        }
        

        private void BtnCreate_Click(object sender, EventArgs e)
        {
            int iRow = fpSpread2.Sheets[0].ActiveRowIndex;
            string strSTD_YRMO = "";

            strSTD_YRMO = (dtH_STD_YRMON.Text == null ? "" : SystemBase.Validation.C1DataEdit_ReadFormat(dtH_STD_YRMON.Value.ToString(), "YYYYMM"));

            if (strSTD_YRMO == "")
            {
                MessageBox.Show("기준연월은 필수입력 항목입니다.", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dtH_STD_YRMON.Focus();
                return;
            }

            if (txtH_ORDR_YEAR.Text == "")
            {
                MessageBox.Show("지시연도는 필수입력 항목입니다.", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtH_ORDR_YEAR.Focus();
                return;
            }
            if (txtH_DCSN_NUMB.Text == "")
            {
                MessageBox.Show("판단번호는 필수입력 항목입니다.", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtH_DCSN_NUMB.Focus();
                return;
            }
            if (cboH_CTMF_CODE.Text == "")
            {
                MessageBox.Show("조달업체는 필수입력 항목입니다.", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cboH_CTMF_CODE.Focus();
                return;
            }

            DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("SY077"), SystemBase.Base.MessageRtn("Z0004"), MessageBoxButtons.YesNo, MessageBoxIcon.Question); //제출자료용도, 제출연월의 데이터를 생성하시겠습니까? 기존에 집계된 데이터는 삭제됩니다.
            if (dsMsg == DialogResult.Yes)
            {
                string ERRCode = "ER", MSGCode = "";
                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                try
                {
                    string strSql = " usp_DAA003_B01 ";
                    strSql += "  @pTYPE = '" + "B1" + "'";
                    strSql += ", @pMNUF_CODE = '" + cboH_MNUF_CODE.SelectedValue + "'"; //제출업체
                    strSql += ", @pSBMTR_CHRG_PURPS = '" + (cboH_SBMTR_CHRG_PURPS.SelectedValue == null ? "" : cboH_SBMTR_CHRG_PURPS.SelectedValue) + "'"; //제출용도
                    strSql += ", @pSTD_YRMON = '" + strSTD_YRMO + "'";                  //기준연월
                    strSql += ", @pDPRT_CODE = '" + txtH_DPRT_CODE.Text + "'";          //부서
                    strSql += ", @pDCSN_NUMB = '" + txtH_DCSN_NUMB.Text + "'";          //판단번호
                    strSql += ", @pCALC_DEGR = '" + txtH_CALC_DEGR.Text + "'";          //차수
                    strSql += ", @pORDR_YEAR = '" + txtH_ORDR_YEAR.Text + "'";          //지시연도
                    strSql += ", @pCTMF_CODE = '" + cboH_CTMF_CODE.SelectedValue + "'"; //조달업체
                    strSql += ", @pIN_ID ='" + SystemBase.Base.gstrUserID + "' ";       //사용자

                    DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                    ERRCode = ds.Tables[0].Rows[0][0].ToString();
                    MSGCode = ds.Tables[0].Rows[0][1].ToString();

                    if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                    Trans.Commit();
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    Trans.Rollback();
                    MSGCode = "SY002";	//에러가 발생하여 데이터 처리가 취소되었습니다.
                }
            Exit:
                dbConn.Close();
                this.Cursor = System.Windows.Forms.Cursors.Default;

                if (ERRCode == "OK")
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    SearchExec();
                }
                else if (ERRCode == "ER")
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }

        }

        private void BtnCreate_MouseEnter(object sender, EventArgs e)
        {
            try
            {
                Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\Toolbar\uDocCrt.gif");
                BtnCreate.Image = bitMap;
            }
            catch (Exception f)
            {
            }
        }

        private void BtnCreate_MouseLeave(object sender, EventArgs e)
        {
            try
            {
                Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\Toolbar\DocCrt.gif");
                BtnCreate.Image = bitMap;
            }
            catch (Exception f)
            {
            }
        }


        #region MASTER 팝업
        private void btnMasterPop_Click(object sender, EventArgs e)
        {
            try
            {
                WNDW.WNDW029 pu = new WNDW.WNDW029();
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtH_ORDR_YEAR.Value = Msgs[1].ToString();  //지시연도
                    txtH_DCSN_NUMB.Value = Msgs[2].ToString();  //판단번호
                    txtH_CALC_DEGR.Value = Msgs[3].ToString();  //차수
                    cboH_SBMTR_CHRG_PURPS.SelectedValue = Msgs[7].ToString(); //제출용도
                    dtH_STD_YRMON.Value = Msgs[8].ToString();  //기준년월
                    cboH_CTMF_CODE.SelectedValue = Msgs[4].ToString();  //조달업체
                    
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

    }
    #endregion
}
