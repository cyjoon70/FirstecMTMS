#region 작성정보
/*********************************************************************/
// 단위업무명 : 거래처이력정보등록
// 작 성 자 : 김 현 근
// 작 성 일 : 2013-03-22
// 작성내용 : 거래처이력정보등록 및 관리
// 수 정 일 :
// 수 정 자 :
// 수정내용 :
// 비    고 :
/*********************************************************************/
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
using WNDW;

namespace BB.BBC002
{
    public partial class BBC002 : UIForm.FPCOMM2
    {
        #region 생성자
        public BBC002()
        {
            InitializeComponent();
        }
        #endregion

        #region 팝업창 열기
        private void cmdZipCode_Click(object sender, EventArgs e)
        {
            try
            {
                //string strQuery = " usp_B_COMMON 'B020' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                //string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                //string[] strSearch = new string[] { txtZipCd.Text };
                //UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P04007", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "우편번호검사");
                //pu.ShowDialog();
                //if (pu.DialogResult == DialogResult.OK)
                //{
                //    Regex rx1 = new Regex("#");
                //    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                //    txtZipCd.Value = Msgs[0].ToString();
                //    txtAddr1.Value = Msgs[1].ToString();
                //    txtAddr2.Focus();
                //}
                WNDW030 pu = new WNDW030(txtZipCd.Text.ToString());
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtZipCd.Value = Msgs[1].ToString();
                    txtAddr1.Value = Msgs[2].ToString();
                    txtAddr2.Text = "";
                    txtAddr2.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "사업코드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region Form Load 시
        private void BBC002_Load(object sender, System.EventArgs e)
        {
            //그룹박스 필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            SystemBase.Validation.GroupBox_Setting(groupBox2);
            SystemBase.Validation.GroupBox_Setting(groupBox3);
            SystemBase.Validation.GroupBox_Setting(groupBox4);

            //콤보박스 세팅
            SystemBase.ComboMake.C1Combo(cboSCustType, "usp_B_COMMON @pTYPE = 'COMM', @pCODE='B005', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 3); //거래처구분
 
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
            UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, 0, 0, false);
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            SystemBase.Validation.GroupBox_Reset(groupBox2);
            SystemBase.Validation.GroupBox_Reset(groupBox3);
            SystemBase.Validation.GroupBox_Reset(groupBox4);

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt,false, false, 0,0);
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {
                    string strQuery = " usp_BBC002  'S1'";
                    strQuery = strQuery + ", @pLANG_CD ='" + SystemBase.Base.gstrLangCd + "' ";
                    strQuery = strQuery + ", @pCUST_CD ='" + txtSCustCd.Text.Trim() + "' ";
                    strQuery = strQuery + ", @pCUST_NM ='" + txtSCustNm.Text + "' ";
                    strQuery = strQuery + ", @pCUST_TYPE ='" + cboSCustType.SelectedValue.ToString() + "' ";
                    strQuery = strQuery + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread2, strQuery, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, true, 0, 0, false);
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }

            this.Cursor = Cursors.Default;
        }
        #endregion
        
        #region SaveExec() 폼에 입력된 데이타 저장 로직
        protected override void SaveExec()
        {
            this.Cursor = Cursors.WaitCursor;

            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox2))
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox3))
                {
                    if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox4))
                    {
                        string ERRCode = "ER", MSGCode = "SY001";	//처리할 내용이 없습니다.
                        bool ChkMsg = true;

                        SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                        SqlCommand cmd = dbConn.CreateCommand();
                        SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                        try
                        {
                            string strSql = " usp_BBC002 'U1' ";
                            strSql = strSql + ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'";
                            strSql = strSql + ", @pCUST_CD = '" + txtCustCd.Text.ToString().Trim() + "'";
                            strSql = strSql + ", @pCHG_RESN = '" + txtChgResn.Text.ToString().Trim() + "'";
                            strSql = strSql + ", @pAPPLY_DT = '" + dtpApplyDt.Text.ToString() + "'";
                            strSql = strSql + ", @pRGST_NO = '" + txtRgstNo.Text.ToString() + "'";
                            strSql = strSql + ", @pCUST_NM = '" + txtCustNm.Text.ToString() + "'";
                            strSql = strSql + ", @pCUST_FULL_NM = '" + txtCustFullNm.Text.ToString() + "'";
                            strSql = strSql + ", @pREPRE_NM = '" + txtRepreNm.Text.ToString() + "'";
                            strSql = strSql + ", @pINDU_TYPE = '" + txtInduType.Text.ToString() + "'";
                            strSql = strSql + ", @pINDU_KIND = '" + txtInduKind.Text.ToString() + "'";
                            strSql = strSql + ", @pZIPCODE = '" + txtZipCd.Text.ToString() + "'";
                            strSql = strSql + ", @pADDR1 = '" + txtAddr1.Text.ToString() + "'";
                            strSql = strSql + ", @pADDR2 = '" + txtAddr2.Text.ToString() + "'";
                            strSql = strSql + ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                            strSql = strSql + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

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
                            MSGCode = "P0001";	//에러가 발생하여 데이터 처리가 취소되었습니다.
                            this.Cursor = Cursors.Default;
                        }
                    Exit:
                        dbConn.Close();

                        if (ChkMsg == true)
                        {
                            if (ERRCode == "OK")
                            {
                                int intRow = fpSpread2.ActiveSheet.GetSelection(0).Row;
                                string strSScode = fpSpread2.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx2, "거래처코드")].Text.ToString();
                                Right_Search(strSScode);
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
            }

            this.Cursor = Cursors.Default;
        }
        #endregion

        #region DelExec() 삭제 로직
        protected override void DeleteExec()
        {
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox2)) 
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox3))  
                {
                    if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox4))  
                    {
                        DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0027"), SystemBase.Base.MessageRtn("Z0004"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                        if (dsMsg == DialogResult.Yes)
                        {
                            string ERRCode = "ER", MSGCode = "P0000";	//처리할 내용이 없습니다.
                            SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                            SqlCommand cmd = dbConn.CreateCommand();
                            SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                            try
                            {
                                string strSql = " usp_BBC002  'D1'";
                                strSql = strSql + ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'";
                                strSql = strSql + ", @pCUST_CD = '" + txtCustCd.Text + "'";
                                strSql = strSql + ", @pAPPLY_DT = '" + dtpApplyDt.Text + "'";
                                strSql = strSql + ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                                strSql = strSql + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

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
                                MSGCode = "P0001";	//에러가 발생하여 데이터 처리가 취소되었습니다.
                            }
                        Exit:
                            dbConn.Close();

                            if (ERRCode == "OK")
                            {
                                int intRow = fpSpread2.ActiveSheet.GetSelection(0).Row;
                                string strSScode = fpSpread2.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx2, "거래처코드")].Text.ToString();
                                Right_Search(strSScode);
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
            }
        }
        #endregion

        #region 좌측 fpSpread 클릭시 우측상세조회
        private void fpSpread2_CellClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            string strCode = "";
            if (fpSpread2.Sheets[0].Rows.Count > 0)
            {
                strCode = fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "거래처코드")].Text.ToString();
                Right_Search(strCode);

                if (fpSpread1.Sheets[0].Rows.Count > 0)
                {
                    string strSDate = fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "적용일자")].Text;
                    string strSCode = fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "거래처코드")].Text.ToString();
                    Right_Sub_Search(strSCode, strSDate);
                }
            }
        }

        //그리드 키보드 이동시 우측조회
        private void fpSpread2_SelectionChanged(object sender, FarPoint.Win.Spread.SelectionChangedEventArgs e)
        {
            string strCode = "";
            if (fpSpread2.Sheets[0].Rows.Count > 0)
            {
                int intRow = fpSpread2.ActiveSheet.GetSelection(0).Row;
                strCode = fpSpread2.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx2, "거래처코드")].Text.ToString();
                Right_Search(strCode);
            }

            if (fpSpread1.Sheets[0].RowCount > 0)
            {
                string strSDate = fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "적용일자")].Text;
                string strSCode = fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "거래처코드")].Text.ToString();
                Right_Sub_Search(strSCode, strSDate);
            }
        }
        #endregion

        #region 우측 fpSpread 클릭시 우측하단 상세
        private void fpSpread1_SelectionChanged(object sender, FarPoint.Win.Spread.SelectionChangedEventArgs e)
        {

            string strCode = ""; string strDate = "";
            if (fpSpread1.Sheets[0].Rows.Count > 0)
            {
                int intRow = fpSpread1.ActiveSheet.GetSelection(0).Row;
                strDate = fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "적용일자")].Text;
                strCode = fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "거래처코드")].Text.ToString();
                Right_Sub_Search(strCode, strDate);
            }
        }
        #endregion

        #region 우측 그리드검색
        private void Right_Search(string strScode)
        {
            try
            {
                string strSql = " usp_BBC002  'S2' ";
                strSql = strSql + ", @pLANG_CD='" + SystemBase.Base.gstrLangCd + "'";
                strSql = strSql + ", @pCUST_CD = '" + strScode + "'";
                strSql = strSql + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread1, strSql, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0, false);
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }
        #endregion

        #region 우측 상세검색
        private void Right_Sub_Search(string strScode, string strSdate)
        {
            SystemBase.Validation.GroupBox_Reset(groupBox2);
            SystemBase.Validation.GroupBox_Reset(groupBox3);
            SystemBase.Validation.GroupBox_Reset(groupBox4);

            try
            {
                string strSql = " usp_BBC002  'S3' ";
                strSql = strSql + ", @pAPPLY_DT ='" + strSdate + "'";
                strSql = strSql + ", @pCUST_CD = '" + strScode + "'";
                strSql = strSql + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                DataSet ds = SystemBase.DbOpen.NoTranDataSet(strSql);
                txtCustCd.Value = ds.Tables[0].Rows[0]["CUST_CD"].ToString();
                dtpApplyDt.Value = ds.Tables[0].Rows[0]["APPLY_DT"].ToString();
                txtChgResn.Text = ds.Tables[0].Rows[0]["CHG_RESN"].ToString();
                txtCustNm.Text = ds.Tables[0].Rows[0]["CUST_NM"].ToString();
                txtCustFullNm.Text = ds.Tables[0].Rows[0]["CUST_FULL_NM"].ToString();
                txtRepreNm.Text = ds.Tables[0].Rows[0]["REPRE_NM"].ToString();
                txtRgstNo.Text = ds.Tables[0].Rows[0]["RGST_NO"].ToString();
                txtInduType.Text = ds.Tables[0].Rows[0]["INDU_TYPE"].ToString();
                txtInduKind.Text = ds.Tables[0].Rows[0]["INDU_KIND"].ToString();
                txtZipCd.Value = ds.Tables[0].Rows[0]["ZIPCODE"].ToString();
                txtAddr1.Value = ds.Tables[0].Rows[0]["ADDR1"].ToString();
                txtAddr2.Text = ds.Tables[0].Rows[0]["ADDR2"].ToString();   
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }
        #endregion

    }
}