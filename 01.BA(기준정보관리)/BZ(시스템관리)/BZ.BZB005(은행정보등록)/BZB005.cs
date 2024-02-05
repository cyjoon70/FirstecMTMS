#region 작성정보
/*********************************************************************/
// 단위업무명 : 은행정보등록
// 작 성 자 : 김 현 근
// 작 성 일 : 2013-03-22
// 작성내용 : 은행정보등록 및 관리
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

namespace BZ.BZB005
{
    public partial class BZB005 : UIForm.FPCOMM2
    {
        #region
        public BZB005()
        {
            InitializeComponent();
        }
        #endregion

        #region 팝업창 열기
        private void cmdZipCode_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON 'B020' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtZipCd.Text };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P04007", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "우편번호검사");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtZipCd.Value = Msgs[0].ToString();
                    txtAddr1.Value = Msgs[1].ToString();
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
        private void BZB005_Load(object sender, System.EventArgs e)
        {
            //그룹박스 필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            SystemBase.Validation.GroupBox_Setting(groupBox2);

            //콤보박스 세팅
            SystemBase.ComboMake.C1Combo(cboBankType, "usp_B_COMMON @pTYPE = 'COMM', @pCODE='B016', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'"); //은행구분
            SystemBase.ComboMake.C1Combo(cboNatCd, "usp_B_COMMON @pTYPE = 'COMM', @pCODE='B006', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'"); //국가코드


            //DETAIL 그리드 콤보박스 세팅            
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "계좌구분")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'B017', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//계좌구분
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "계좌유형")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'B018', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");//계좌유형
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "거래상태")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'A506', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//거래상태
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "화폐")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'Z003', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");//화폐
          
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
            UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, 0, 0, false);
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox2);

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
                    string strQuery = " usp_BZB005  'S1'";
                    strQuery = strQuery + ", @pLANG_CD ='" + SystemBase.Base.gstrLangCd + "' ";
                    strQuery = strQuery + ", @pBANK_CD ='" + txtCode.Text.Trim() + "' ";
                    strQuery = strQuery + ", @pBANK_NM ='" + txtName.Text + "' ";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

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
            string grdFocus = "";
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox2))  //컨트롤 필수여부체크 
            {
                string ERRCode = "ER", MSGCode = "SY001";	//처리할 내용이 없습니다.
                bool ChkMsg = true;

                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                try
                {
                    //우측상위 텍스트 저장
                    string strSql = " usp_BZB005 'U1' ";
                    strSql = strSql + ", @pLANG_CD	 = '" + SystemBase.Base.gstrLangCd + "'";
                    strSql = strSql + ", @pCO_CD	 = '" + SystemBase.Base.gstrCOMCD + "'";
                    strSql = strSql + ", @pBANK_CD	 = '" + txtBankCd.Text.Trim() + "'";
                    strSql = strSql + ", @pBANK_NM	 = '" + txtBankNm.Text + "'";
                    strSql = strSql + ", @pBANK_TYPE = '" + cboBankType.SelectedValue.ToString() + "'";
                    strSql = strSql + ", @pZIPCODE   = '" + txtZipCd.Text + "'";
                    strSql = strSql + ", @pADDR1	 = '" + txtAddr1.Text + "'";
                    strSql = strSql + ", @pADDR2	 = '" + txtAddr2.Text + "'";
                    strSql = strSql + ", @pNAT_CD	 = '" + cboNatCd.SelectedValue.ToString() + "'";
                    strSql = strSql + ", @pUP_ID	 = '" + SystemBase.Base.gstrUserID + "'";

                    DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                    ERRCode = ds.Tables[0].Rows[0][0].ToString();
                    MSGCode = ds.Tables[0].Rows[0][1].ToString();

                    if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                    //우측 하위 그리드 저장                    
                    if ((SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", false) == true))// 그리드 필수항목 체크 
                    {
                        for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                        {
                            string strHead = fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text;
                            string strGbn = "";
                            if (strHead.Length > 0)
                            {
                                switch (strHead)
                                {
                                    case "U": strGbn = "U2"; break;
                                    case "D": strGbn = "D2"; break;
                                    case "I": strGbn = "I2"; break;
                                    default: strGbn = ""; break;
                                }

                                grdFocus = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "계좌번호")].Text.ToString() + "'";
                                string strSql1 = " usp_BZB005 ";
                                strSql1 = strSql1 + " @pType		= '" + strGbn + "'";
                                strSql1 = strSql1 + ", @pLANG_CD	= '" + SystemBase.Base.gstrLangCd + "'";
                                strSql1 = strSql1 + ", @pCO_CD		= '" + SystemBase.Base.gstrCOMCD + "'";
                                strSql1 = strSql1 + ", @pBANK_CD	= '" + txtBankCd.Text.Trim() + "'";
                                strSql1 = strSql1 + ", @pACCT_NO	= '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "계좌번호")].Text.ToString() + "'";
                                strSql1 = strSql1 + ", @pACCT_TYPE	= '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "계좌구분")].Value.ToString() + "'";
                                strSql1 = strSql1 + ", @pACCT_PART	= '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "계좌유형")].Value.ToString() + "'";
                                strSql1 = strSql1 + ", @pDEPOSIT_OWNER = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "예금주")].Text.ToString() + "'";
                                strSql1 = strSql1 + ", @pOPEN_DT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "거래일자")].Text.ToString() + "'";
                                strSql1 = strSql1 + ", @pOPEN_STATUS = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "거래상태")].Value.ToString() + "'";
                                strSql1 = strSql1 + ", @pCUR_CD	= '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "화폐")].Value.ToString() + "'";
                                strSql1 = strSql1 + ", @pREMARK	= '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "비고")].Text.ToString() + "'";
                                strSql1 = strSql1 + ", @pBIZ_CD	= '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "사업장")].Text.ToString() + "'";
                                strSql1 = strSql1 + ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";

                                DataSet ds1 = SystemBase.DbOpen.TranDataSet(strSql1, dbConn, Trans);
                                ERRCode = ds1.Tables[0].Rows[0][0].ToString();
                                MSGCode = ds1.Tables[0].Rows[0][1].ToString();

                                if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
                            }
                        }
                    }
                    else
                    {
                        ERRCode = "ER"; ChkMsg = false;
                        Trans.Rollback(); goto Exit;
                    }
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
                        MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);

                        SearchExec();
                        Right_Sub_Search();
                        //그리드 셀 포커스 이동
                        UIForm.FPMake.GridSetFocus(fpSpread2, grdFocus, SystemBase.Base.GridHeadIndex(GHIdx2, "수주번호"));  
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

            this.Cursor = Cursors.Default;
        }
        #endregion

        #region DelExec() 삭제 로직
        protected override void DeleteExec()
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
                    string strSql = " usp_BZB005  'D1'";
                    strSql = strSql + ", @pLANG_CD	 = '" + SystemBase.Base.gstrLangCd + "'";
                    strSql = strSql + ", @pBANK_CD  = '" + txtBankCd.Text.Trim() + "'";
                    strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

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

        #region 좌측 fpSpread 클릭시 우측상세조회
        private void fpSpread2_CellClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            string strCode = "";
            if (fpSpread2.Sheets[0].Rows.Count > 0)
            {
                strCode = fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "은행코드")].Text.ToString();
                Right_Search(strCode);
            }
        }

        //그리드 키보드 이동시 우측조회
        private void fpSpread2_SelectionChanged(object sender, FarPoint.Win.Spread.SelectionChangedEventArgs e)
        {
            string strCode = "";
            if (fpSpread2.Sheets[0].Rows.Count > 0)
            {
                int intRow = fpSpread2.ActiveSheet.GetSelection(0).Row;
                strCode = fpSpread2.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx2, "은행코드")].Text.ToString();
                Right_Search(strCode);
            }
        }
        #endregion
        
        #region 우측 조회
        private void Right_Search(string strScode)
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {
                string strSql = " usp_BZB005  'S2' ";
                strSql = strSql + ", @pLANG_CD='" + SystemBase.Base.gstrLangCd + "'";
                strSql = strSql + ", @pBANK_CD = '" + strScode + "'";
                strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                DataSet ds = SystemBase.DbOpen.NoTranDataSet(strSql);
                txtBankCd.Value = ds.Tables[0].Rows[0]["BANK_CD"].ToString();
                txtBankNm.Value = ds.Tables[0].Rows[0]["BANK_NM"].ToString();
                if (ds.Tables[0].Rows[0]["BANK_TYPE"].ToString() != "") cboBankType.SelectedValue = ds.Tables[0].Rows[0]["BANK_TYPE"];
                txtZipCd.Value = ds.Tables[0].Rows[0]["ZIPCODE"].ToString();
                txtAddr1.Value = ds.Tables[0].Rows[0]["ADDR1"].ToString();
                txtAddr2.Value = ds.Tables[0].Rows[0]["ADDR2"].ToString();
                if (ds.Tables[0].Rows[0]["NAT_CD"].ToString() != "") cboNatCd.SelectedValue = ds.Tables[0].Rows[0]["NAT_CD"];

                Right_Sub_Search();
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }

            this.Cursor = Cursors.Default;
        }
        #endregion
       
        #region 우측 그리드검색
        private void Right_Sub_Search()
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                string strSql = " usp_BZB005  'S3' ";
                strSql = strSql + ", @pLANG_CD	= '" + cboNatCd.SelectedValue.ToString() + "'";
                strSql = strSql + ", @pCO_CD	= '" + SystemBase.Base.gstrCOMCD + "'";
                strSql = strSql + ", @pBANK_CD	= '" + txtBankCd.Text.Trim() + "'";

                UIForm.FPMake.grdCommSheet(fpSpread1, strSql, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }

            this.Cursor = Cursors.Default;
        }
        #endregion

        #region 그리드 상 팝업
        protected override void fpButtonClick(int Row, int Column)
        {
            //사업장
            if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "사업장_2"))
            {
                try
                {
                    string strQuery = " usp_B_COMMON 'B042', @pCO_CD = '"+ SystemBase.Base.gstrCOMCD.ToString()+"' ";
                    string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                    string[] strSearch = new string[] { fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "사업장")].Text, "" };

                    UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00086", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "사업장 조회");	//창고, LOCATION조회
                    pu.ShowDialog();

                    if (pu.DialogResult == DialogResult.OK)
                    {
                        Regex rx1 = new Regex("#");
                        string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "사업장")].Text = Msgs[0].ToString();
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "사업장명")].Text = Msgs[1].ToString();

                        UIForm.FPMake.fpChange(fpSpread1, Row);//수정플래그
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "사업장 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        #endregion

        #region 그리드 상 데이터 변경시 연계데이터 자동입력
        protected override void fpSpread1_ChangeEvent(int Row, int Column)
        {
            //사업장
            if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "사업장"))
            {
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "사업장명")].Text
                    = SystemBase.Base.CodeName("BIZ_CD", "BIZ_NM", "B_BIZ_PLACE", fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "사업장")].Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
            }
        }
        #endregion

    }
}