#region 작성정보
/*********************************************************************/
// 단위업무명 : 사원정보등록
// 작 성 자   : 김 창 진
// 작 성 일   : 2014-04-21
// 작성내용   : 사원정보등록 및 관리
// 수 정 일   :
// 수 정 자   :
// 수정내용   :
// 비    고   :
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

namespace HD.HDA001
{
    public partial class HDA001 : UIForm.FPCOMM1
    {
        #region 변수선언
        int PreRow = -1;   // SelectionChanged 시에 동일 Row에서 데이타변환 처리 안하도록 하기 위함.
        #endregion

        #region HDA001
        public HDA001()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void HDA001_Load(object sender, System.EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(groupBox1);//그룹박스 필수,읽기전용 Setting
            SystemBase.Validation.GroupBox_Setting(groupBox2);//그룹박스 필수,읽기전용 Setting

            //콤보박스 세팅
            SystemBase.ComboMake.C1Combo(cboDirIndir, "usp_B_COMMON @pType='COMM', @pCODE='H0071', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 9); //직간접구분

            //그리드 초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0,0);
        }
        #endregion

        #region 팝업창 열기
        //부서코드 팝업
        private void btnDept_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = "";

                if (SystemBase.Base.HumanRoll(SystemBase.Base.gstrUserID.ToString()) == "Y")
                {
                    strQuery = " usp_H_COMMON @pType='H014', @pDATE = '" + SystemBase.Base.ServerTime("YYMMDD") +"' ";
                }
                else
                {
                    strQuery = " usp_H_COMMON @pType='H001', @pDATE = '" + SystemBase.Base.ServerTime("YYMMDD") +"', @pSPEC1 = '" + txtInternalCd.Text + "' ";
                }
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };
                string[] strSearch = new string[] { txtDeptCd.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("H00001", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "부서 조회");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtDeptCd.Value = Msgs[0].ToString();
                    txtDeptNm.Value = Msgs[1].ToString();
                    txtInternalCd.Value = Msgs[2].ToString();
                    txtDeptCd.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "부서 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //우편번호
        private void cmdZipCode_Click(object sender, EventArgs e)
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            try
            {
                WNDW030 pu = new WNDW030(txtZipCode.Text.ToString());
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtZipCode.Text = Msgs[1].ToString();
                    txtAddr1.Value = Msgs[2].ToString();
                    txtAddr1.Focus();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "우편번호조회 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }

            this.Cursor = System.Windows.Forms.Cursors.Default;
        }

        //구매조직
        private void btnRoll_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON 'COMM_POP', @pLANG_CD ='" + SystemBase.Base.gstrLangCd + "', @pSPEC1 = 'H0002', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' ";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtRollCd.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("BBI002P1", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "직위 조회");
                pu.Width = 500;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtRollCd.Value = Msgs[0].ToString();
                    txtRollNm.Value = Msgs[1].ToString();
                    txtRollCd.Focus();
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "구매조직 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox2); //그룹박스 초기화
            SystemBase.Validation.GroupBox_Setting(groupBox2); //그룹박스 필수,읽기전용 Setting

            txtEmpNo.Focus();
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1)) //필수체크
            {
                string strQuery = " usp_HDA001  'S1', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                strQuery = strQuery + ", @pEMP_NO = '" + txtSEmpCd.Text + "'";
                strQuery = strQuery + ", @pEMP_NM = '" + txtSEmpNm.Text + "'";

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
                    string strSql = " usp_HDA001 'U1'";
                    strSql = strSql + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                    strSql = strSql + ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'";
                    strSql = strSql + ", @pEMP_NO = '" + txtEmpNo.Text + "'";
                    strSql = strSql + ", @pEMP_NM = '" + txtEmpNm.Text + "'";
                    strSql = strSql + ", @pDEPT_CD = '" + txtDeptCd.Text + "'";
                    strSql = strSql + ", @pDEPT_NM = '" + txtDeptNm.Text + "'";
                    strSql = strSql + ", @pINTERNAL_CD = '" + txtInternalCd.Text + "'";
                    strSql = strSql + ", @pROLL_CD = '" + txtRollCd.Text + "'";
                    strSql = strSql + ", @pENTER_DT = '" + dtpEntrDt.Text + "'";
                    strSql = strSql + ", @pRETIRE_DT = '" + dtpRetireDt.Text + "'";
                    strSql = strSql + ", @pBIRT = '" + dtpBirt.Text + "'";
                    strSql = strSql + ", @pDIR_INDIR = '" + cboDirIndir.SelectedValue.ToString() + "'";
                    strSql = strSql + ", @pZIPCODE = '" + txtZipCode.Text + "'";
                    strSql = strSql + ", @pADDR = '" + txtAddr1.Text + "'";
                    strSql = strSql + ", @pTEL = '" + txtTel.Text + "'";
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
                    UIForm.FPMake.GridSetFocus(fpSpread1, txtEmpNo.Text);

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

            if (txtEmpNo.Text != "")
            {
                if (MessageBox.Show(SystemBase.Base.MessageRtn("SY010"), "삭제", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                {
                    string ERRCode = "ER", MSGCode = "SY001"; //처리할 내용이 없습니다.

                    SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                    SqlCommand cmd = dbConn.CreateCommand();
                    SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                    try
                    {
                        string strSql = " usp_HDA001 'D1'";
                        strSql = strSql + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                        strSql = strSql + ", @pEMP_NO = '" + txtEmpNo.Text + "'";

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

        #region TextChanged
        //부서코드
        private void txtDeptCd_TextChanged(object sender, EventArgs e)
        {
            string strQuery = "usp_H_COMMON @pType='H002', @pDATE = '" + SystemBase.Base.ServerTime("YYMMDD") + "', @pCOM_CD = '" + txtDeptCd.Text + "' ";
            strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

            DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

            if (dt.Rows.Count > 0)
            {
                txtDeptNm.Value = dt.Rows[0][1].ToString();
                txtInternalCd.Value = dt.Rows[0][2].ToString();
                txtDeptCd.Focus();
            }
            else
            {
                txtDeptNm.Value = "";
                txtInternalCd.Value = "";
                txtDeptCd.Focus();
            }
        }

        //직위코드
        private void txtRollCd_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (txtRollCd.Text != "")
                {
                    txtRollNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtRollCd.Text, " AND MAJOR_CD = 'H0002' AND LANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' AND COMP_CODE = '" + SystemBase.Base.gstrCOMCD + "' ");
                }
                else
                {
                    txtRollNm.Value = "";
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

            string strQuery = " usp_HDA001  'S2'";
            strQuery = strQuery + ", @pCO_CD ='" + SystemBase.Base.gstrCOMCD + "' ";
            strQuery = strQuery + ", @pEMP_NO ='" + fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "사원번호")].Text + "' ";

            DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

            if (dt.Rows.Count > 0)
            {
                txtEmpNo.Value = dt.Rows[0]["EMP_NO"].ToString();			    //사원코드
                txtEmpNm.Value = dt.Rows[0]["NAME"].ToString();			        //사원명
                txtUsrId.Value = dt.Rows[0]["USR_ID"].ToString();			    //User id
                txtDeptCd.Value = dt.Rows[0]["DEPT_CD"].ToString();			    //부서코드
                txtDeptNm.Value = dt.Rows[0]["DEPT_NM"].ToString();			    //부서명
                txtInternalCd.Value = dt.Rows[0]["INTERNAL_CD"].ToString();	    //내부부서코드
                txtUsrId.Value = dt.Rows[0]["USR_ID"].ToString();			    //User id
                dtpBirt.Value = dt.Rows[0]["BIRT"].ToString();		            //생년월일
                dtpEntrDt.Value = dt.Rows[0]["ENTR_DT"].ToString();		        //입사일
                dtpRetireDt.Value = dt.Rows[0]["RETIRE_DT"].ToString();		    //퇴사일

                txtRollCd.Value = dt.Rows[0]["ROLL_PSTN"].ToString();		    //직위
                txtRollNm.Value = dt.Rows[0]["ROLL_PSTN_NM"].ToString();		//직위명

                cboDirIndir.SelectedValue = dt.Rows[0]["DIR_INDIR"].ToString();	//직간접구분

                txtZipCode.Value = dt.Rows[0]["ZIP_CD"].ToString();		        //우편번호
                txtAddr1.Value = dt.Rows[0]["ADDR"].ToString();			        //주소1
                txtTel.Value = dt.Rows[0]["HAND_TEL_NO"].ToString();			//전화번호1

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

        private void btnHistory_Click(object sender, EventArgs e)
        {
            HDA001History pu = new HDA001History(txtEmpNo.Text, txtEmpNm.Text);
            pu.ShowDialog();

        }
    }
}
