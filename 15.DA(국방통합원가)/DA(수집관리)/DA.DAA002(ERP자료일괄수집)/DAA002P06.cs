using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.OleDb;
using System.IO;
using System.Data;
using System.Data.SqlClient;

namespace DA.DAA002
{
    public partial class DAA002P06 : UIForm.FPCOMM1
    {
        #region 변수선언
        int iPK_SEQ = 0;
        string strMNUF_CODE = "";   //제출업체   
        string strORDR_YEAR = "";   //요구연도
        string strDPRT_CODE = "";   //구매부서
        string strDCSN_NUMB = "";   //판단번호
        string strCALC_DEGR = "";   //차수        
        string strFormId = "";
        string strSql = "";
        #endregion

        #region DAA002P06
        public DAA002P06()
        {
            InitializeComponent();
        }
        #endregion

        #region DAA002P06 값 받아옴
        public DAA002P06(int PK_SEQ, string MNUF_CODE, string ORDR_YEAR, string DPRT_CODE, string DCSN_NUMB, string CALC_DEGR, string FormId)
        {
            InitializeComponent();

            iPK_SEQ = PK_SEQ;
            strMNUF_CODE = MNUF_CODE;   //제출업체   
            strORDR_YEAR = ORDR_YEAR;   //요구연도
            strDPRT_CODE = DPRT_CODE;   //구매부서
            strDCSN_NUMB = DCSN_NUMB;   //판단번호
            strCALC_DEGR = CALC_DEGR;   //차수
            strFormId = FormId;
        }
        #endregion

        #region DAA002P06_Load
        private void DAA002P06_Load(object sender, EventArgs e)
        {
            UIForm.Buttons.ReButton("010000011001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);

            SystemBase.Validation.GroupBox_Reset(groupBox1);
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            txtM_ORDR_YEAR.Value = strORDR_YEAR;   //요구연도      
            txtM_DCSN_NUMB.Value = strDCSN_NUMB;   //판단번호
            txtM_CALC_DEGR.Value = strCALC_DEGR;   //차수  

            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "자산구분")] = SystemBase.ComboMake.ComboOnGrid("usp_CO_COMM_CODE @pTYPE='COMM', @pCOMP_CODE = 'SYS', @pCODE = 'GA018'", 0);
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "취득구분")] = SystemBase.ComboMake.ComboOnGrid("usp_CO_COMM_CODE @pTYPE='COMM', @pCOMP_CODE = 'SYS', @pCODE = 'GA019'", 0);
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "내용연수구분")] = SystemBase.ComboMake.ComboOnGrid("usp_CO_COMM_CODE @pTYPE='COMM', @pCOMP_CODE = 'SYS', @pCODE = 'GA020'", 0);
            
            
            this.Text = SystemBase.Base.GetMenuTree(strFormId) + " > 감가상각비";
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))  //필수조회조건 체크
                {
                    strSql = " usp_DAA002P01  ";
                    strSql += "  @pTYPE = 'S3'";
                    strSql += ", @pDATA_FLAG = 'DEPR' ";
                    strSql += ", @pMASTER_SEQ = " + iPK_SEQ + " ";
                    strSql += ", @pMNUF_CODE = '" + strMNUF_CODE + "' ";
                    strSql += ", @pORDR_YEAR = '" + txtM_ORDR_YEAR.Text + "' ";
                    strSql += ", @pDPRT_CODE = '" + strDPRT_CODE + "' ";
                    strSql += ", @pDCSN_NUMB = '" + txtM_DCSN_NUMB.Text + "' ";
                    strSql += ", @pCALC_DEGR = '" + txtM_CALC_DEGR.Text + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strSql, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, true, true, 0, 0);
                }
                SystemBase.Validation.GroupBox_SearchViewValidation(groupBox1);
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region SaveExec() 저장
        protected override void SaveExec()
        {
            string strSql = ""; string strHead = ""; string strGbn = "";
            string ERRCode = "", MSGCode = "";
            SqlConnection dbConn = SystemBase.DbOpen.DBCON();
            SqlCommand cmd = dbConn.CreateCommand();
            SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            try
            {
                if (SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", false) == true) // 그리드 상단 필수항목 체크
                {
                    for (int i = 0; i < (fpSpread1.Sheets[0].Rows.Count - fpSpread1.Sheets[0].FrozenTrailingRowCount); i++)
                    {
                        strHead = fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text;
                        strGbn = "";
                        if (strHead.Length > 0)
                        {
                            if (strHead != "합계")
                            {
                                switch (strHead)
                                {
                                    case "U": strGbn = "U1"; break;
                                    default: strGbn = ""; break;
                                }

                                strSql = " usp_DAA002P01  ";
                                strSql += "  @pTYPE = '" + strGbn + "'";
                                strSql += ", @pDATA_FLAG = 'DEPR' ";
                                strSql += ", @pKEY_ID = " + SystemBase.Validation.Decimal_Data(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "MASTER_SEQ")].Text.ToString(), ",");
                                strSql += ", @pNIIN = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "계약품목재고번호")].Text.ToString() + "'";
                                strSql += ", @pASSET_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "자산관리번호")].Text.ToString() + "'";
                                strSql += ", @pPROCESS_PK_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "공정명")].Text.ToString() + "'";
                                strSql += ", @pDIV_RULE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "배부기준(G)")].Text.ToString() + "'";

                                DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                                ERRCode = ds.Tables[0].Rows[0][0].ToString();
                                MSGCode = ds.Tables[0].Rows[0][1].ToString();

                                if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }
                            }
                        }
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
                MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                SearchExec();
            }
            else if (ERRCode == "ER")
            {
                MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (ERRCode == "WR")
            {
                MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        #endregion

        #region DAA002P06_Shown 조회
        private void DAA002P06_Shown(object sender, EventArgs e)
        {
            SearchExec();
        }
        #endregion

    }
}
