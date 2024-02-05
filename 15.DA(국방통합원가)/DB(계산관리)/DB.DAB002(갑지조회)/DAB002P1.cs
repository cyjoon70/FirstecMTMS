#region DAB002P1 작성 정보
/*************************************************************/
// 단위업무명 : 장비구성품목 등록
// 작 성 자 :   유재규
// 작 성 일 :   2013-06-13
// 작성내용 :   
// 수 정 일 :
// 수 정 자 :
// 수정내용 :
// 비    고 : 
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

namespace DB.DAB002
{
    public partial class DAB002P1 : UIForm.FPCOMM1
    {
        #region 변수선언
        int iDETAIL_SEQ = 0;   //순번
        string strMNUF_CODE = "";   //제출업체  
        string strORDR_YEAR = "";   //요구연도
        string strDCSN_NUMB = "";   //판단번호                      
        string strCALC_DEGR = "";   //차수         
        string strDPRT_CODE = "";   //제출년월 
        string strNIIN = "";        //재고번호
        string strSql = "";
        int FrozenCol = 0;
        int FrozenRow = 0;
        string strFormId = ""; 
        #endregion

        #region DAB002P1()
        public DAB002P1()
        {
            InitializeComponent();
        }
        #endregion
      
        #region DAB002P1()
        public DAB002P1(int DETAIL_SEQ, string MNUF_CODE, string ORDR_YEAR, string DPRT_CODE, string DCSN_NUMB, string CALC_DEGR, string NIIN, string FormId)
        {
            InitializeComponent();

            iDETAIL_SEQ = DETAIL_SEQ;   //순번
            strMNUF_CODE = MNUF_CODE;   //제출업체
            strORDR_YEAR = ORDR_YEAR;   //요구연도
            strDPRT_CODE = DPRT_CODE;   //부서
            strDCSN_NUMB = DCSN_NUMB;   //판단번호                            
            strCALC_DEGR = CALC_DEGR;   //차수 
            strNIIN = NIIN;             //재고번호
            strFormId = FormId;            
        }
        #endregion

        #region DAB002P1_Load
        private void DAB002P1_Load(object sender, EventArgs e)
        {
            UIForm.Buttons.ReButton("111111110001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);
            
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            //제출업체
            SystemBase.ComboMake.C1Combo(cboM_MNUF_CODE, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'D004', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'", 0);   //제출업체
            //부서
            SystemBase.ComboMake.C1Combo(cboM_DPRT_CODE, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'D007', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'", 0);

            txtM_DETAIL_SEQ.Value = iDETAIL_SEQ;    //순번
            cboM_MNUF_CODE.SelectedValue = strMNUF_CODE;   //제출업체       
            txtM_ORDR_YEAR.Value = strORDR_YEAR;   //요구연도
            cboM_DPRT_CODE.SelectedValue = strDPRT_CODE;   //부서        
            txtM_DCSN_NUMB.Value = strDCSN_NUMB;   //판단번호
            txtM_CALC_DEGR.Value = strCALC_DEGR;   //차수 
            txtM_NIIM.Value = strNIIN;             //재고번호     

            SystemBase.Validation.GroupBox_SearchViewValidation(groupBox1);
           // SearchExec();
            this.Text = SystemBase.Base.GetMenuTree(strFormId) + " > 장비구성품등록"; 

        }
        #endregion

        #region 행추가 버튼 클릭 이벤트
        protected override void RowInsExec()
        {
            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))  //필수조회조건 체크
                {
                    UIForm.FPMake.RowInsert(fpSpread1);

                    fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "DETAIL순번")].Text = txtM_DETAIL_SEQ.Text;
                    fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "제출업체코드")].Text = cboM_MNUF_CODE.SelectedValue.ToString();
                    fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "요구년도")].Text = txtM_ORDR_YEAR.Text;
                    fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "판단번호")].Text = txtM_DCSN_NUMB.Text;
                    fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "부서")].Text = cboM_DPRT_CODE.SelectedValue.ToString();
                    fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Text = txtM_CALC_DEGR.Text;
                    fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "계약품목")].Text = txtM_NIIM.Text;
                    fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "BOM전개")].Value = 1;
                }
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
                    FpGrid_DataSet();
                    UIForm.FPMake.grdCommSheet(fpSpread1, strSql, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, FrozenRow, FrozenCol, false);
                    fpSpread1.Sheets[0].Columns[SystemBase.Base.GridHeadIndex(GHIdx1, "BOM전개")].Locked = true;
                }
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
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))  //필수여부체크
            {
                if (SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", true) == true)
                {
                    DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("SY048"), SystemBase.Base.MessageRtn("Z0004"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dsMsg == DialogResult.Yes)
                    {
 
                        string strHead = ""; string strGbn = "";
                        string ERRCode = "", MSGCode = "";
                        SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                        SqlCommand cmd = dbConn.CreateCommand();
                        SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);
                        this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
                        try
                        {
                            for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                            {
                                strHead = fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text;
                                strGbn = "";
                                if (strHead.Length > 0)
                                {
                                    switch (strHead)
                                    {
                                        case "D": strGbn = "D1"; break;
                                        case "U": strGbn = "I1"; break;  // UPDATE 및 INSERT 동일 처리
                                        case "I": strGbn = "I1"; break;  // UPDATE 및 INSERT 동일 처리
                                        default: strGbn = ""; break;
                                    }

                                    string strSql = " usp_DAB002P1 ";
                                    strSql += "  @pTYPE = '" + strGbn + "'";
                                    strSql += ", @pDETAIL_SEQ = " + SystemBase.Validation.Decimal_Data(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "DETAIL순번")].Text.ToString(), ",")  + " ";
                                    strSql += ", @pMNUF_CODE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "제출업체코드")].Text.ToString() + "'";
                                    strSql += ", @pORDR_YEAR = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "요구년도")].Text.ToString() + "'";
                                    strSql += ", @pDCSN_NUMB = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "판단번호")].Text.ToString() + "'";
                                    strSql += ", @pDPRT_CODE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "부서")].Text.ToString() + "'";
                                    strSql += ", @pCALC_DEGR = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Text.ToString() + "'";
                                    strSql += ", @pNIIN = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "계약품목")].Text.ToString() + "'";

                                    strSql += ", @pNATION_STOCK_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "국가재고번호")].Text.ToString() + "'";
                                    strSql += ", @pMANAGER_PART_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "부품관리번호")].Text.ToString() + "'";
                                    strSql += ", @pPART_NAME = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품명")].Text.ToString() + "'";
                                    strSql += ", @pPART_ID = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "ERP품목")].Text.ToString() + "'";
                                    strSql += ", @pFLOOR_PLAN_NUMB = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "도면번호")].Text.ToString() + "'";
                                    strSql += ", @pFLOOR_ITEM_NUMB = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "도면부품번호")].Text.ToString() + "'";

                                    strSql += ", @pPART_UNIT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].Value.ToString() + "'";
                                    strSql += ", @pCMPT_REQR = " + SystemBase.Validation.Decimal_Data(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수량(대당)")].Text.ToString(), ",") + " ";
                                    strSql += ", @pAPPLY_SPEC = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "적용규격")].Text.ToString() + "' ";
                                   strSql += ",  @pBOM_FLAG = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "BOM전개")].Value.ToString() + "' ";
                                    strSql += ", @pEXCEPTION_FLAG = '" + (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "제외")].Value == null ? "0" :"1") + "' ";
                                    strSql += ", @pSORT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "정렬순서")].Text.ToString() + "' ";
                                    
                                    strSql += ", @pNOTE	= '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "비고")].Text.ToString() + "' ";
                                    strSql += ", @pIN_ID ='" + SystemBase.Base.gstrUserID + "' ";                                  //사용자

                                    DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                                    ERRCode = ds.Tables[0].Rows[0][0].ToString();
                                    MSGCode = ds.Tables[0].Rows[0][1].ToString();

                                    if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }
                                }
                            }

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
        }
        #endregion

        #region FpGrid_DataSet
        private void FpGrid_DataSet()
        {
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "단위")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'Z005', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 0);

            strSql = " usp_DAB002P1  ";
            strSql += "  @pTYPE = 'S1'";
            strSql += ", @pDETAIL_SEQ = " + Convert.ToInt32(txtM_DETAIL_SEQ.Text) + " ";
            strSql += ", @pMNUF_CODE = '" + cboM_MNUF_CODE.SelectedValue + "' ";
            strSql += ", @pORDR_YEAR = '" + txtM_ORDR_YEAR.Text + "' ";
            strSql += ", @pDCSN_NUMB = '" + txtM_DCSN_NUMB.Text + "' ";
            strSql += ", @pDPRT_CODE = '" + cboM_DPRT_CODE.SelectedValue.ToString() + "' ";
            strSql += ", @pCALC_DEGR = '" + txtM_CALC_DEGR.Text + "' ";
            strSql += ", @pNIIN = '" + txtM_NIIM.Text + "' ";
        }
        #endregion

        #region 이미지 전환
        private void BtnPart_MouseEnter(object sender, EventArgs e)
        {
            try
            {
                Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\Toolbar\uExcelUpload.gif");
                BtnPart.Image = bitMap;
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnPart_MouseLeave(object sender, EventArgs e)
        {
            try
            {
                Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\Toolbar\ExcelUpload.gif");
                BtnPart.Image = bitMap;
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region BtnPart_Click 전환
        private void BtnPart_Click(object sender, EventArgs e)
        {
            try
            {
                //WNDW.WNDW029 pu = new WNDW.WNDW029("", "", "", "", "MULTI");  // 프로잭트, 품번, 국가재고, 부품관리번호, 구분(SINGLE, MULTI)
                //pu.ShowDialog();

                //if (pu.RETURN > 0)
                //{
                //    this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
                //    fpSpread1.Sheets[0].ActiveRowIndex = fpSpread1.Sheets[0].RowCount - 1;  //마지막 라인부터 추가하기위해 인덱스 처리
                //    for (int i = 0; i < pu.RETURN; i++)
                //    {
                //        UIForm.FPMake.RowInsert(fpSpread1);

                //        fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "DETAIL순번")].Text = txtM_DETAIL_SEQ.Text;
                //        fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "제출업체코드")].Text = cboM_MNUF_CODE.SelectedValue.ToString();
                //        fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "요구년도")].Text = txtM_ORDR_YEAR.Text;
                //        fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "판단번호")].Text = txtM_DCSN_NUMB.Text;
                //        fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "부서")].Text = cboM_DPRT_CODE.SelectedValue.ToString();
                //        fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Text = txtM_CALC_DEGR.Text;
                //        fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "계약품목")].Text = txtM_NIIM.Text;
                //        fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "BOM전개")].Value = 1;

                //        fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "국가재고번호")].Text = pu.NATION_STOCK_NO[i];
                //        fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "부품관리번호")].Text = pu.MANAGER_PART_NO[i];
                //        fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "도면번호")].Text = pu.FLOOR_PLAN_NUMB[i];
                //        fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "도면부품번호")].Text = pu.FLOOR_PLAN_NUMB[i];
                //        fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "ERP품목")].Text = pu.PART_ID[i];
                //        fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "품명")].Text = pu.PART_NAME[i];
                //        fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].Value = pu.STOCK_UM[i];
                //    }
                //    this.Cursor = System.Windows.Forms.Cursors.Default;
                //}
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        private void DAB002P1_Shown(object sender, EventArgs e)
        {
            try
            {
                SearchExec();
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}
