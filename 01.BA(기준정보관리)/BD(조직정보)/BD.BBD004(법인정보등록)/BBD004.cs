#region 작성정보
/*********************************************************************/
// 단위업무명 : 법인정보등록
// 작 성 자 : 조 홍 태
// 작 성 일 : 2013-01-25
// 작성내용 : 법인정보등록 및 관리
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

namespace BD.BBD004
{
    public partial class BBD004 : UIForm.FPCOMM1
    {
        #region 변수선언
        int PreRow = -1;   // SelectionChanged 시에 동일 Row에서 데이타변환 처리 안하도록 하기 위함.
        #endregion

        #region BBD004
        public BBD004()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void BBD004_Load(object sender, System.EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(groupBox1);//그룹박스 필수,읽기전용 Setting
            SystemBase.Validation.GroupBox_Setting(groupBox2);//그룹박스 필수,읽기전용 Setting

            //콤보박스 세팅
            SystemBase.ComboMake.C1Combo(cboNatCd, "usp_B_COMMON @pType='COMM', @pCODE='B006', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 9); //국가

            //그리드 초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0,0);
        }
        #endregion

        #region 팝업창 열기
        //법인정보
        private void btnSCo_Click(object sender, System.EventArgs e)
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            try
            {
                string strQuery = " usp_B_COMMON @pTYPE = 'CO' ";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtSCoCd.Text };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00107", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "법인 조회");
                pu.Width = 400;
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtSCoCd.Value = Msgs[0];
                    txtSCoNm.Value = Msgs[1];
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY008", "법인 조회 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
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
                //strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
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
            txtCoCd.Focus();
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1)) //필수체크
            {
                string strQuery = " usp_BBD004  'S1', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";

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
                    string strSql = " usp_BBD004 'U1'";
                    strSql = strSql + ", @pCO_CD = '" + txtCoCd.Text + "'";
                    strSql = strSql + ", @pCO_NM = '" + txtCoNm.Text + "'";
                    strSql = strSql + ", @pCO_FULL_NM = '" + txtCoFullNm.Text + "'";
                    strSql = strSql + ", @pCO_ENG_NM = '" + txtCoEngNm.Text + "'";
                    strSql = strSql + ", @pCORP_RGST_NO = '" + txtCoRgstNo.Text + "'";
                    strSql = strSql + ", @pREPRE_NM = '" + txtRepreNm.Text + "'";
                    strSql = strSql + ", @pREPRE_RGST_NO= '" + txtRepreRgstNo.Text + "'";
                    strSql = strSql + ", @pFOUND_DT = '" + dtpOpenDt.Text + "'";
                    strSql = strSql + ", @pINDU_TYPE = '" + txtInduType.Text + "'";
                    strSql = strSql + ", @pINDU_KIND = '" + txtInduKind.Text + "'";
                    strSql = strSql + ", @pZIPCODE = '" + txtZipCode.Text + "'";
                    strSql = strSql + ", @pADDR1 = '" + txtAddr1.Text + "'";
                    strSql = strSql + ", @pADDR2 = '" + txtAddr2.Text + "'";
                    strSql = strSql + ", @pADDR1_ENG = '" + txtAddrEng1.Text + "'";
                    strSql = strSql + ", @pADDR2_ENG = '" + txtAddrEng2.Text + "'";
                    strSql = strSql + ", @pADDR3_ENG = '" + txtAddrEng3.Text + "'";

                    strSql = strSql + ", @pNAT_CD = '" + cboNatCd.SelectedValue.ToString() + "'";
                    strSql = strSql + ", @pTEL1 = '" + txtTel1.Text + "'";
                    strSql = strSql + ", @pTEL2 = '" + txtTel2.Text + "'";
                    strSql = strSql + ", @pFAX = '" + txtFax.Text + "'";
                    strSql = strSql + ", @pFISC_CNT = '" + txtFiscCnt.Value + "'";
                    strSql = strSql + ", @pFISC_FR_DT = '" + dtpFiscSt.Text + "'";
                    strSql = strSql + ", @pFISC_TO_DT = '" + dtpFiscEt.Text + "'";

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
                    UIForm.FPMake.GridSetFocus(fpSpread1, txtCoCd.Text);

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

            if (txtCoCd.Text != "")
            {
                if (MessageBox.Show(SystemBase.Base.MessageRtn("SY010"), "삭제", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                {
                    string ERRCode = "ER", MSGCode = "SY001"; //처리할 내용이 없습니다.

                    SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                    SqlCommand cmd = dbConn.CreateCommand();
                    SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                    try
                    {
                        string strSql = " usp_BBD004 'D1'";
                        strSql = strSql + ", @pCO_CD = '" + txtCoCd.Text + "'";

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

        #region txtSCoCd_TextChanged 이벤트
        private void txtSCoCd_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (txtSCoCd.Text != "")
                {
                    txtSCoNm.Value = SystemBase.Base.CodeName("CO_CD", "CO_NM", "B_COMP_INFO", txtSCoCd.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtSCoNm.Value = "";
                }
            }
            catch { }
        }
        #endregion

        #region 좌측그리드 방향키 이동 및 클릭시 우측조회
        private void fpSpread1_SelectionChanged(object sender, FarPoint.Win.Spread.SelectionChangedEventArgs e)
        {

            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            LeftGridSelect(0);

            this.Cursor = System.Windows.Forms.Cursors.Default;
        }

        private void LeftGridSelect(int intRow)
        {
            intRow = fpSpread1.ActiveSheet.GetSelection(0).Row;
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

            string strQuery = " usp_BBD004  'S2'";
            strQuery = strQuery + ", @pCO_CD ='" + fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "법인코드")].Text + "' ";

            DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

            if (dt.Rows.Count > 0)
            {
                txtCoCd.Value = dt.Rows[0]["CO_CD"].ToString();			        //법인코드
                txtCoNm.Text = dt.Rows[0]["CO_NM"].ToString();			        //법인명
                txtCoFullNm.Text = dt.Rows[0]["CO_FULL_NM"].ToString();		    //법인전명
                txtCoEngNm.Text = dt.Rows[0]["CO_ENG_NM"].ToString();		    //법인영문명
                txtCoRgstNo.Text = dt.Rows[0]["CORP_RGST_NO"].ToString();	    //법인등록번호
                txtRepreNm.Text = dt.Rows[0]["REPRE_NM"].ToString();		    //대표자명
                txtRepreRgstNo.Text = dt.Rows[0]["REPRE_RGST_NO"].ToString();	//대표자주민번호=				
                dtpOpenDt.Value = dt.Rows[0]["FOUND_DT"].ToString();		    //창립기념일
                txtInduType.Text = dt.Rows[0]["INDU_TYPE"].ToString();		    //업태
                txtInduKind.Text = dt.Rows[0]["INDU_KIND"].ToString();		    //업종
                txtZipCode.Text = dt.Rows[0]["ZIPCODE"].ToString();		        //우편번호
                txtAddr1.Value = dt.Rows[0]["ADDR1"].ToString();			    //주소1
                txtAddr2.Text = dt.Rows[0]["ADDR2"].ToString();			        //주소2
                txtAddrEng1.Text = dt.Rows[0]["ADDR1_ENG"].ToString();		    //영어주소1
                txtAddrEng2.Text = dt.Rows[0]["ADDR2_ENG"].ToString();		    //영어주소2
                txtAddrEng3.Text = dt.Rows[0]["ADDR3_ENG"].ToString();		    //영어주소3
                cboNatCd.SelectedValue = dt.Rows[0]["NAT_CD"].ToString();		//국가번호
                txtTel1.Text = dt.Rows[0]["TEL1"].ToString();			        //전화번호1
                txtTel2.Text = dt.Rows[0]["TEL2"].ToString();			        //전화번호2
                txtFax.Text = dt.Rows[0]["FAX"].ToString();				        //팩스
                txtFiscCnt.Text = dt.Rows[0]["FISC_CNT"].ToString();		    //회기
                dtpFiscSt.Value = dt.Rows[0]["FISC_FR_DT"].ToString();		    //회기시작일
                dtpFiscEt.Value = dt.Rows[0]["FISC_TO_DT"].ToString();          //회기종료일

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
