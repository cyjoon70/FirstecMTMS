#region 작성정보
/*********************************************************************/
// 단위업무명 : 사업장 정보등록
// 작 성 자 : 조 홍 태
// 작 성 일 : 2013-01-30
// 작성내용 : 사업장 정보등록 및 관리
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

namespace BD.BBD005
{
    public partial class BBD005 : UIForm.FPCOMM1
    {
        #region 변수선언
        int PreRow = -1;   // SelectionChanged 시에 동일 Row에서 데이타변환 처리 안하도록 하기 위함.
        #endregion

        #region BBD005
        public BBD005()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void BBD005_Load(object sender, System.EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(groupBox1);//그룹박스 필수,읽기전용 Setting
            SystemBase.Validation.GroupBox_Setting(groupBox2);//그룹박스 필수,읽기전용 Setting

            //콤보박스 세팅
            SystemBase.ComboMake.C1Combo(cboCoCd, "usp_B_COMMON @pType='CO', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'"); //법인
            SystemBase.ComboMake.C1Combo(cboTaxOffCd, "usp_B_COMMON @pType='COMM', @pCODE='B007', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'", 9); //세무서
            SystemBase.ComboMake.C1Combo(cboTaxBizCd, "usp_B_COMMON @pType='BIZ', @pCO_CD = '"+ SystemBase.Base.gstrCOMCD.ToString() + "' ", 9); //세금신고 사업장

            //그리드 초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0,0);
        }
        #endregion

        #region 팝업창 열기
        //사업장정보
        private void btnSBiz_Click(object sender, System.EventArgs e)
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            try
            {
                string strQuery = " usp_B_COMMON @pTYPE = 'BIZ', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' ";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtSBizCd.Text,"" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00108", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "사업장 조회");
                pu.Width = 400;
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtSBizCd.Value = Msgs[0];
                    txtSBizNm.Value = Msgs[1];
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY008", "사업장 조회 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
            finally
            {
                this.Cursor = System.Windows.Forms.Cursors.Default;
            }
        }

        //우편번호
        private void cmdZipCode_Click(object sender, EventArgs e)
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            try
            {
                //string strQuery = " usp_B_COMMON @pType = 'B020' ";
                //string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                //string[] strSearch = new string[] { txtZipCode.Text, "" };
                //UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P04007", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "우편번호조회");
                //pu.Width = 520;
                //pu.ShowDialog();
                //if (pu.DialogResult == DialogResult.OK)
                //{
                //    Regex rx1 = new Regex("#");
                //    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                //    txtZipCode.Text = Msgs[0].ToString();
                //    txtAddr1.Value = Msgs[1].ToString();
                //    txtAddr2.Text = "";
                //    txtAddr2.Focus();
                //}

                WNDW030 pu = new WNDW030(txtZipCode.Text.ToString());
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtZipCode.Text = Msgs[1].ToString();
                    txtAddr1.Value = Msgs[2].ToString();
                    txtAddr2.Text = "";
                    txtAddr2.Focus();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "우편번호조회 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }

            this.Cursor = System.Windows.Forms.Cursors.Default;
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox2); //그룹박스 초기화
            SystemBase.Validation.GroupBox_Setting(groupBox2); //그룹박스 필수,읽기전용 Setting
            txtBizCd.Focus();
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1)) //필수체크
            {
                string strQuery = " usp_BBD005  'S1', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'";
                strQuery = strQuery + ", @pBIZ_CD ='" + txtSBizCd.Text + "' ";

                //그리드 Binding
                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0);
            }

            this.Cursor = System.Windows.Forms.Cursors.Default;
        }
        #endregion

        #region SaveExec() 폼에 입력된 데이타 저장 로직
        protected override void SaveExec()
        {
            this.Cursor = Cursors.WaitCursor;

            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox2))
            {
                SystemBase.Validation.Control_SaveCheck(groupBox2); //현재 컨트롤 데이터 저장

                //기존 컨트롤 데이터와 현재 컨트롤 데이터 비교
                if(SystemBase.Base.gstrControl_OrgData == SystemBase.Base.gstrControl_SaveData) 
                {
                    //변경되거나 처리할 데이터가 없습니다.
                    MessageBox.Show(SystemBase.Base.MessageRtn("SY017"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.Cursor = Cursors.Default;
                    return;
                }

                string ERRCode = "ER", MSGCode = "SY001"; //처리할 내용이 없습니다.

                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                try
                {                             
                    string strSql = " usp_BBD005 'U1'";
                    strSql = strSql + ", @pBIZ_CD = '" + txtBizCd.Text + "'";
                    strSql = strSql + ", @pBIZ_NM = '" + txtBizNm.Text + "'";
                    strSql = strSql + ", @pBIZ_FULL_NM = '" + txtBizFullNm.Text + "'";
                    strSql = strSql + ", @pBIZ_ENG_NM = '" + txtBizEngNm.Text + "'";
                    strSql = strSql + ", @pRGST_NO = '" + txtRgstNo.Text + "'";
                    strSql = strSql + ", @pREPRE_NM = '" + txtRepreNm.Text + "'";
                    strSql = strSql + ", @pINDU_TYPE = '" + txtInduType.Text + "'";
                    strSql = strSql + ", @pINDU_KIND = '" + txtInduKind.Text + "'";
                    strSql = strSql + ", @pZIPCODE = '" + txtZipCode.Text + "'";
                    strSql = strSql + ", @pADDR1 = '" + txtAddr1.Text + "'";
                    strSql = strSql + ", @pADDR2 = '" + txtAddr2.Text + "'";
                    strSql = strSql + ", @pADDR1_ENG = '" + txtAddrEng1.Text + "'";
                    strSql = strSql + ", @pADDR2_ENG = '" + txtAddrEng2.Text + "'";
                    strSql = strSql + ", @pADDR3_ENG = '" + txtAddrEng3.Text + "'";

                    strSql = strSql + ", @pCO_CD = '" + cboCoCd.SelectedValue.ToString() + "'";
                    strSql = strSql + ", @pTAX_OFF_CD = '" + cboTaxOffCd.SelectedValue.ToString() + "'";
                    strSql = strSql + ", @pTAX_BIZ_CD = '" + cboTaxBizCd.SelectedValue.ToString() + "'"; 
                    
                    strSql = strSql + ", @pTEL1 = '" + txtTel1.Text + "'";
                    strSql = strSql + ", @pTEL2 = '" + txtTel2.Text + "'";
                    strSql = strSql + ", @pFAX = '" + txtFax.Text + "'";

                    strSql = strSql + ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";

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
                    MSGCode = "SY002"; // 에러가 발생되어 데이터 처리가 취소되었습니다.
                }
            Exit:
                dbConn.Close();

                if (ERRCode == "OK")
                {
                    SearchExec();

                    //그리드 셀 포커스 이동
                    UIForm.FPMake.GridSetFocus(fpSpread1, txtBizCd.Text);

                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (ERRCode == "ER") //ERROR
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else   //ERRCode == "WR" WARING
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }

            this.Cursor = Cursors.Default;
        }
        #endregion

        #region DeleteExec() 데이타 삭제 로직
        protected override void DeleteExec()
        {
            this.Cursor = Cursors.WaitCursor;

            if (txtBizCd.Text != "")
            {
                if (MessageBox.Show(SystemBase.Base.MessageRtn("SY010"), "삭제", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                {
                    string ERRCode = "ER", MSGCode = "SY001"; //처리할 내용이 없습니다.

                    SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                    SqlCommand cmd = dbConn.CreateCommand();
                    SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                    try
                    {
                        string strSql = " usp_BBD005 'D1', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'";
                        strSql = strSql + ", @pBIZ_CD = '" + txtBizCd.Text + "'";

                        strSql = strSql + ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";

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
                        MSGCode = "SY002"; // 에러가 발생되어 데이터 처리가 취소되었습니다.
                    }
                Exit:
                    dbConn.Close();

                    if (ERRCode == "OK")
                    {
                        SearchExec();
                        SystemBase.Validation.GroupBox_Reset(groupBox2);
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

            this.Cursor = Cursors.Default;
        }
        #endregion

        #region txtSBizCd_TextChanged 이벤트
        private void txtSBizCd_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (txtSBizCd.Text != "")
                {
                    txtSBizNm.Value = SystemBase.Base.CodeName("BIZ_CD", "BIZ_NM", "B_BIZ_PLACE", txtSBizCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'");
                }
                else
                {
                    txtSBizNm.Value = "";
                }
            }
            catch { }
        }
        #endregion

        #region 좌측그리드 방향키 이동 및 클릭시 우측조회
        private void fpSpread1_SelectionChanged(object sender, FarPoint.Win.Spread.SelectionChangedEventArgs e)
        {

            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            LeftGridSelect();

            this.Cursor = System.Windows.Forms.Cursors.Default;
        }

        private void LeftGridSelect()
        {
            int intRow = fpSpread1.ActiveSheet.GetSelection(0).Row;
            if (intRow < 0)
            {
                this.Cursor = System.Windows.Forms.Cursors.Default;
                return;
            }

            if (PreRow == intRow && PreRow != -1 && intRow != 0)   //현 Row에서 컬럼이동시는 조회 안되게
            {
                this.Cursor = System.Windows.Forms.Cursors.Default;
                return;
            }

            string strQuery = " usp_BBD005  'S2', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' ";
            strQuery = strQuery + ", @pBIZ_CD ='" + fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "사업장코드")].Text + "' ";

            DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

            if (dt.Rows.Count > 0)
            {
                txtBizCd.Value = dt.Rows[0]["BIZ_CD"].ToString();			    //사업장코드
                txtBizNm.Text = dt.Rows[0]["BIZ_NM"].ToString();			    //사업장명
                txtBizFullNm.Text = dt.Rows[0]["BIZ_FULL_NM"].ToString();		//사업장전명
                txtBizEngNm.Text = dt.Rows[0]["BIZ_ENG_NM"].ToString();		    //사업장영문명
                txtRgstNo.Text = dt.Rows[0]["RGST_NO"].ToString();	            //사업자등록번호
                txtRepreNm.Text = dt.Rows[0]["REPRE_NM"].ToString();		    //대표자명
                txtInduType.Text = dt.Rows[0]["INDU_TYPE"].ToString();		    //업태
                txtInduKind.Text = dt.Rows[0]["INDU_KIND"].ToString();		    //업종
                txtZipCode.Text = dt.Rows[0]["ZIPCODE"].ToString();		        //우편번호
                txtAddr1.Value = dt.Rows[0]["ADDR1"].ToString();			    //주소1
                txtAddr2.Text = dt.Rows[0]["ADDR2"].ToString();			        //주소2
                txtAddrEng1.Text = dt.Rows[0]["ADDR1_ENG"].ToString();		    //영어주소1
                txtAddrEng2.Text = dt.Rows[0]["ADDR2_ENG"].ToString();		    //영어주소2
                txtAddrEng3.Text = dt.Rows[0]["ADDR3_ENG"].ToString();		    //영어주소3
                cboCoCd.SelectedValue = dt.Rows[0]["CO_CD"].ToString();		    //법인코드
                cboTaxOffCd.SelectedValue = dt.Rows[0]["TAX_OFF_CD"].ToString();//세무서
                cboTaxBizCd.SelectedValue = dt.Rows[0]["TAX_BIZ_CD"].ToString();//세금신고사업장
                txtTel1.Text = dt.Rows[0]["TEL1"].ToString();			        //전화번호1
                txtTel2.Text = dt.Rows[0]["TEL2"].ToString();			        //전화번호2
                txtFax.Text = dt.Rows[0]["FAX"].ToString();				        //팩스

                SystemBase.Validation.Control_SearchCheck(groupBox2);           //초기 컨트롤 데이터 저장
            }
            else
            {
                //그룹박스 초기화
                SystemBase.Validation.GroupBox_Reset(groupBox2);
            }

            //현재 row값 설정
            PreRow = fpSpread1.ActiveSheet.GetSelection(0).Row;

            //키값 컨트롤 읽기전용으로 셋팅
            SystemBase.Validation.GroupBox_SearchViewValidation(groupBox2);
        }
        #endregion
    }
}
