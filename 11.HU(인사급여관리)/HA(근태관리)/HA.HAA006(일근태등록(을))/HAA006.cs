#region 작성정보
/*********************************************************************/
// 단위업무명 : 일근태등록(을)
// 작 성 자 : 김 현 근
// 작 성 일 : 2013-04-10
// 작성내용 : 일근태등록(을)
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

namespace HA.HAA006
{
    public partial class HAA006 : UIForm.FPCOMM1
    {
        #region 변수선언
        string strEmpNo = "";
        #endregion

        #region 생성자
        public HAA006()
        {
            InitializeComponent();
            strEmpNo = SystemBase.Base.gstrUserID.Replace("FST", "").ToString();
        }
        #endregion

        #region 로그인 사용자 체크 후 필수유무
        private void UsrCheck()
        {
            //로그인 유저가 'ADMIN' 이거나 'INSA' 인 경우 수정가능하게 한다.
            //			if(strEmpNo == "ADMIN" || strEmpNo == "INSA")
            //			{
            //				txtDeptCd.Tag = "1";
            //				btnDept.Tag = "";
            //				SystemBase.Base.GroupBoxLang(groupBox1);
            //			}
            //			else
            //			{
            //				txtDeptCd.Tag = "2";
            //				btnDept.Tag = "2";
            //				SystemBase.Base.GroupBoxLang(groupBox1);
            //			}

            EmpDataInput(strEmpNo);
        }
        #endregion

        #region 사원데이터 자동기입
        private void EmpDataInput(string EmpNo)
        {
            string strQuery = "usp_H_COMMON @pType='H004', @pCOM_CD = '" + EmpNo + "' ";
            strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

            DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

            if (dt.Rows.Count > 0)
            {
                txtDeptCd.Text = dt.Rows[0][6].ToString();
                txtDeptNm.Value = dt.Rows[0][2].ToString();
                txtInternalCd.Value = dt.Rows[0][7].ToString();
                txtDeptCd.Focus();
            }
            else
            {
                txtDeptCd.Text = "";
                txtDeptNm.Value = "";
                txtInternalCd.Value = "";
                txtDeptCd.Focus();
            }
        }
        #endregion

        #region Form Load 시
        private void HAA006_Load(object sender, System.EventArgs e)
        {
            //GroupBox1 초기화
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            dtpDate.Text = SystemBase.Base.ServerTime("YYMMDD");

            //사용자체크
            UsrCheck();

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0);           
        }
        #endregion

        #region 행 추가
        protected override void RowInsExec()
        {
            UIForm.FPMake.RowInsert(fpSpread1);

            fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "가동여부")].Text = "True";
            //			fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "가동시간")].Text = "08:00";
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            dtpDate.Text = SystemBase.Base.ServerTime("YYMMDD");

            //그리드 초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0);
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            //조회조건 필수 체크
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                try
                {
                    string strQuery = " usp_HAA006  @pTYPE = 'S1', @pINTERNAL_CD = '" + txtInternalCd.Text + "' ";
                    strQuery = strQuery + " , @pDATE = '" + dtpDate.Text + "' ";
                    strQuery = strQuery + " , @pEMP_NO = '" + txtEmpNo.Text + "' ";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0, true);

                    if (fpSpread1.Sheets[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                        {
                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "반영여부")].Text == "True")
                            {
                                UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "비가동코드") + "|3"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "비가동코드_2") + "|3"
                                    //									+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "가동시간") + "|3"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "근태횟수") + "|3"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "시간외시간") + "|3"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "근태코드") + "|3"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "근태코드_2") + "|3"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "비고") + "|3"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "가동여부") + "|3"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "사원번호_2") + "|3"
                                    );
                            }
                            else
                            {

                                UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "비고") + "|0"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "가동여부") + "|0"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "근태코드_2") + "|0"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "사원번호_2") + "|3");

                                if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "가동여부")].Text == "True")
                                {
                                    UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "비가동코드") + "|3"
                                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "비가동코드_2") + "|3"
                                        //										+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "가동시간") + "|1"
                                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "근태횟수") + "|3"
                                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "시간외시간") + "|0"
                                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "근태코드") + "|0"
                                        );
                                }
                                else
                                {
                                    UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "비가동코드") + "|1"
                                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "비가동코드_2") + "|0"
                                        //										+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "가동시간") + "|3"
                                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "근태횟수") + "|1"
                                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "시간외시간") + "|3"
                                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "근태코드") + "|1"
                                        );
                                }
                            }
                        }
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회중 오류가 발생하였습니다.
                }
            }
            this.Cursor = System.Windows.Forms.Cursors.Default;
        }
        #endregion

        #region Save
        protected override void SaveExec()
        {
            if (UIForm.FPMake.FPUpCheck(fpSpread1, false) == true)
            {
                string ERRCode = "ER", MSGCode = "P0000";	//처리할 내용이 없습니다.
                string strKeyCd = "";
                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                try
                {
                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {
                        string strHead = fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text;
                        string strGbn = "";

                        if (strHead.Length > 0)
                        {
                            switch (strHead)
                            {
                                case "U": strGbn = "U1"; break;
                                case "I": strGbn = "I1"; break;
                                case "D": strGbn = "D1"; break;
                                default: strGbn = ""; break;
                            }

                            string strYear_flag = "N";
                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "반영여부")].Text == "True")
                            {
                                strYear_flag = "Y";
                            }

                            if (strYear_flag == "Y")
                            {
                                int Row = i + 1;

                                ERRCode = "WR";
                                MSGCode = "반영여부가 'Y'인 " + Row.ToString() + "행은 수정 또는 삭제할 수 없습니다.";
                                Trans.Rollback(); goto Exit;
                            }

                            string strHour = null, strMin = null, strWorkHour = null, strWorkMin = null;
                            strKeyCd = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "사원번호")].Text;

                            string strSql = " usp_HAA006 @pTYPE = '" + strGbn + "' ";
                            strSql = strSql + ", @pDATE = '" + dtpDate.Text + "' ";
                            strSql = strSql + ", @pEMP_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "사원번호")].Text + "'";
                            strSql = strSql + ", @pDILIG_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "근태코드")].Text + "'";
                            strSql = strSql + ", @pDILIG_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "순번")].Text + "'";
                            strSql = strSql + ", @pDILIG_CNT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "근태횟수")].Value + "'";
                            strSql = strSql + ", @pREMARK = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "비고")].Text + "'";

                            string strWorkYn = "N";
                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "가동여부")].Text == "True")
                            {
                                strWorkYn = "Y";
                            }
                            strSql = strSql + ", @pWORK_YN = '" + strWorkYn + "'";

                            strSql = strSql + ", @pINDIR_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "비가동코드")].Text + "'";

                            //							if(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "가동시간")].Text.Replace("_","").Replace(":","") != "")
                            //							{			
                            //								if(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "가동시간")].Text.Replace("_","").Replace(":","").Length != 4)
                            //								{
                            //									Trans.Rollback();
                            //									ERRCode = "WR";
                            //									i = i+1;
                            //									MSGCode = i.ToString() + " 열의 가동시간 정보가 잘못되었습니다.";
                            //									goto Exit;
                            //								}
                            //																 
                            //								strWorkHour = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "가동시간")].Text.Substring(0,2);
                            //								strWorkMin = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "가동시간")].Text.Substring(3,2);
                            //
                            //								strSql = strSql + ", @pWORK_HH  = '" + strWorkHour + "'";
                            //								strSql = strSql + ", @pWORK_MM  = '" + strWorkMin + "'";
                            //							}

                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "시간외시간")].Text.Replace("_", "").Replace(":", "") != "")
                            {
                                if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "시간외시간")].Text.Replace("_", "").Replace(":", "").Length != 4)
                                {
                                    Trans.Rollback();
                                    ERRCode = "WR";
                                    i = i + 1;
                                    MSGCode = i.ToString() + " 열의 시간외시간 정보가 잘못되었습니다.";
                                    goto Exit;
                                }

                                strHour = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "시간외시간")].Text.Substring(0, 2);
                                strMin = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "시간외시간")].Text.Substring(3, 2);

                                strSql = strSql + ", @pDILIG_HH  = '" + strHour + "'";
                                strSql = strSql + ", @pDILIG_MM  = '" + strMin + "'";
                            }

                            strSql = strSql + ", @pUP_ID  = '" + SystemBase.Base.gstrUserID.ToString() + "'";
                            strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                            DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                            ERRCode = ds.Tables[0].Rows[0][0].ToString();
                            MSGCode = ds.Tables[0].Rows[0][1].ToString();

                            if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
                        }
                    }

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
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);

                    SearchExec();
                    UIForm.FPMake.GridSetFocus(fpSpread1, strKeyCd);
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

        #region 팝업
        //부서코드
        private void btnDept_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = "";

                if (SystemBase.Base.HumanRoll(SystemBase.Base.gstrUserID.ToString()) == "Y")
                {
                    strQuery = " usp_H_COMMON @pType='H014', @pDATE = '" + dtpDate.Text + "' ";
                }
                else
                {
                    strQuery = " usp_H_COMMON @pType='H001', @pDATE = '" + dtpDate.Text + "', @pSPEC1 = '" + txtInternalCd.Text + "' ";
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
       
        //사원번호
        private void btnEmpNo_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = " usp_H_COMMON @pType='H003'";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };
                string[] strSearch = new string[] { txtEmpNo.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("H00002", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "사원 조회");
                pu.Width = 700;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtEmpNo.Value = Msgs[0].ToString();
                    txtEmpNm.Value = Msgs[1].ToString();
                    txtEmpNo.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "사원 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 조회조건 TextChanged
        //부서코드
        private void txtDeptCd_TextChanged(object sender, EventArgs e)
        {
            string strQuery = "usp_H_COMMON @pType='H002', @pDATE = '" + dtpDate.Text + "', @pCOM_CD = '" + txtDeptCd.Text + "' ";
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

        //사원번호
        private void txtEmpNo_TextChanged(object sender, EventArgs e)
        {
            string strQuery = "usp_H_COMMON @pType='H004', @pCOM_CD = '" + txtEmpNo.Text + "' ";
            strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

            DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

            if (dt.Rows.Count > 0)
            {
                txtEmpNm.Value = dt.Rows[0][1].ToString();
                txtEmpNo.Focus();
            }
            else
            {
                txtEmpNm.Value = "";
                txtEmpNo.Focus();
            }
        }
        #endregion

        #region 그리드 상 팝업
        protected override void fpButtonClick(int Row, int Column)
        {
            //사원번호
            if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "사원번호_2"))
            {
                string strQuery = " usp_H_COMMON 'H003' ";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };
                string[] strSearch = new string[] { fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "사원번호")].Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("H00002", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "사원번호 팝업");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "사원번호")].Text = Msgs[0].ToString();
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "이름")].Text = Msgs[1].ToString();

                    UIForm.FPMake.fpChange(fpSpread1, Row);//수정플래그

                }
            }

            //비가동코드
            if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "비가동코드_2"))
            {
                string strQuery = " usp_H_COMMON 'H017'";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };
                string[] strSearch = new string[] { fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "비가동코드")].Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("H00006", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "비가동코드 팝업");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "비가동코드")].Text = Msgs[0].ToString();
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "비가동명")].Text = Msgs[1].ToString();

                    UIForm.FPMake.fpChange(fpSpread1, Row);//수정플래그

                }
            }

            //근태코드
            if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "근태코드_2"))
            {
                string strWorkYn = "1";

                if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "가동여부")].Text == "True")
                {
                    strWorkYn = "2";
                }

                string strQuery = " usp_H_COMMON 'H012', @pSPEC1 = '" + strWorkYn + "' ";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };
                string[] strSearch = new string[] { fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "근태코드")].Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("H00005", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "근태코드 팝업");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "근태코드")].Text = Msgs[0].ToString();
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "근태명")].Text = Msgs[1].ToString();
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "근태유형")].Text = Msgs[2].ToString();

                    UIForm.FPMake.fpChange(fpSpread1, Row);//수정플래그

                }
            }
        }
        #endregion

        #region 그리드 상 Change
        protected override void fpSpread1_ChangeEvent(int Row, int Column)
        {
            //사원번호 
            if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "사원번호"))
            {
                string strQuery = " usp_H_COMMON 'H004'";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                strQuery = strQuery + " , @pCOM_CD = '" + fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "사원번호")].Text + "' ";
                DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

                if (dt.Rows.Count > 0)
                {
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "이름")].Text = dt.Rows[0][1].ToString();
                }
                else
                {
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "이름")].Text = "";

                    MessageBox.Show(SystemBase.Base.MessageRtn("존재하지 않는 사원번호입니다."), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }

            //비가동코드
            if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "비가동코드"))
            {
                string strQuery = " usp_H_COMMON 'H018'";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                strQuery = strQuery + " , @pCOM_CD = '" + fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "비가동코드")].Text + "' ";
                DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

                if (dt.Rows.Count > 0)
                {
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "비가동명")].Text = dt.Rows[0][1].ToString();
                }
                else
                {
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "비가동명")].Text = "";
                }
            }

            //근태코드
            if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "근태코드"))
            {
                string strWorkYn = "1";

                if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "가동여부")].Text == "True")
                {
                    strWorkYn = "2";
                }

                string strQuery = " usp_H_COMMON 'H013', @pSPEC1 = '" + strWorkYn + "'";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                strQuery = strQuery + " , @pCOM_CD = '" + fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "근태코드")].Text + "' ";
                DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

                if (dt.Rows.Count > 0)
                {
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "근태명")].Text = dt.Rows[0][1].ToString();
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "근태유형")].Text = dt.Rows[0][2].ToString();
                }
                else
                {
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "근태명")].Text = "";
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "근태유형")].Text = "";

                    if (strWorkYn == "1")
                    {
                        MessageBox.Show(SystemBase.Base.MessageRtn("횟수 근태코드만 입력 가능합니다."), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                    {
                        MessageBox.Show(SystemBase.Base.MessageRtn("시간 근태코드만 입력 가능합니다."), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
        }
        #endregion
        
        #region 전체 근태생성
        private void btnAllCreate_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;

            string chkFlag = "N";

            //근태재실행체크
            string strQuery = "usp_HAA006 @pTYPE = 'C1', @pDATE = '" + dtpDate.Text + "' ";
            strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

            DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

            if (dt.Rows.Count > 0)
            {
                string msg = SystemBase.Base.MessageRtn(dtpDate.Text + "에 이미 생성된 데이터가 존재합니다. 재생성 하시겠습니까?");
                DialogResult dsMsg = MessageBox.Show(msg, "근태생성(작업일보)", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (dsMsg == DialogResult.Yes)
                {
                    Save("S3");

                    chkFlag = "Y";
                }
            }
            else
            {
                string msg = SystemBase.Base.MessageRtn("근태생성(작업일보)을 하시겠습니까?");
                DialogResult dsMsg = MessageBox.Show(msg, "근태생성(작업일보)", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (dsMsg == DialogResult.Yes)
                {
                    Save("S3");

                    chkFlag = "Y";
                }
            }

            //실행되었을때만 조회
            if (chkFlag == "Y")
            {
                SearchExec();
            }

            this.Cursor = Cursors.Default;
        }
        #endregion

        #region 근태생성 부서별(작업일보)
        private void btnDeptCreate_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;

            string chkFlag = "N";

            if (txtInternalCd.Text == "")
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show("부서별 생성입니다. 부서코드가 잘못되었습니다", "부서별 근태생성(작업일보)", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            //근태재실행체크
            string strQuery = "usp_HAA006 @pTYPE = 'C1', @pDATE = '" + dtpDate.Text + "', @pINTERNAL_CD = '" + txtInternalCd.Text + "'  ";
            strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

            DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

            if (dt.Rows.Count > 0)
            {
                string msg = SystemBase.Base.MessageRtn(dtpDate.Text + "에 이미 생성된 데이터가 존재합니다. 재생성 하시겠습니까?");
                DialogResult dsMsg = MessageBox.Show(msg, "부서별 근태생성(작업일보)", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (dsMsg == DialogResult.Yes)
                {
                    Save("S1");

                    chkFlag = "Y";
                }
            }
            else
            {
                string msg = SystemBase.Base.MessageRtn("근태생성(작업일보)을 하시겠습니까?");
                DialogResult dsMsg = MessageBox.Show(msg, "근태생성(작업일보)", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (dsMsg == DialogResult.Yes)
                {
                    Save("S1");

                    chkFlag = "Y";
                }
            }

            //실행되었을때만 조회
            if (chkFlag == "Y")
            {
                SearchExec();
            }

            this.Cursor = Cursors.Default;
        }
        #endregion

        #region 사원별 근태생성
        private void btnEmpCreate_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;

            string chkFlag = "N";

            if (txtEmpNo.Text == "")
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show("사원별 생성입니다. 사원코드가 잘못되었습니다", "사원별 근태생성(작업일보)", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            //근태재실행체크
            string strQuery = "usp_HAA006 @pTYPE = 'C1', @pDATE = '" + dtpDate.Text + "', @pINTERNAL_CD = '" + txtInternalCd.Text + "', @pEMP_NO = '" + txtEmpNo.Text + "'  ";
            strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

            DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

            if (dt.Rows.Count > 0)
            {
                string msg = SystemBase.Base.MessageRtn(dtpDate.Text + "에 이미 생성된 데이터가 존재합니다. 재생성 하시겠습니까?");
                DialogResult dsMsg = MessageBox.Show(msg, "사원별 근태생성(작업일보)", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (dsMsg == DialogResult.Yes)
                {
                    if (chkNight.Checked == true)
                    {
                        Save("S4"); //야간조계산
                    }
                    else
                    {
                        Save("S2");	//일반계산
                    }

                    chkFlag = "Y";
                }
            }
            else
            {
                string msg = SystemBase.Base.MessageRtn("근태생성(작업일보)을 하시겠습니까?");
                DialogResult dsMsg = MessageBox.Show(msg, "근태생성(작업일보)", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (dsMsg == DialogResult.Yes)
                {
                    if (chkNight.Checked == true)
                    {
                        Save("S4"); //야간조계산
                    }
                    else
                    {
                        Save("S2");	//일반계산
                    }

                    chkFlag = "Y";
                }
            }

            //실행되었을때만 조회
            if (chkFlag == "Y")
            {
                SearchExec();
            }

            this.Cursor = Cursors.Default;
        }
        #endregion

        #region 근태생성함수
        private void Save(string strType)
        {
            string ERRCode = "ER", MSGCode = "P0000";	//처리할 내용이 없습니다.
            SqlConnection dbConn = SystemBase.DbOpen.DBCON();
            SqlCommand cmd = dbConn.CreateCommand();
            SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

            try
            {
                string strSql = " usp_H_DAY_DILIG @pTYPE = '" + strType + "' ";
                strSql = strSql + ", @pDILIG_DT  = '" + dtpDate.Text.Replace("-", "") + "'";
                strSql = strSql + ", @pINTERNAL_CD  = '" + txtInternalCd.Text.Trim() + "'";
                strSql = strSql + ", @pEMP_NO  = '" + txtEmpNo.Text.Trim() + "'";
                strSql = strSql + ", @pUP_ID  = '" + SystemBase.Base.gstrUserID.ToString() + "'";
                strSql = strSql + ", @pERR_MSG = '' ";
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
        #endregion

        #region 가동여부 클릭
        private void fpSpread1_ButtonClicked(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            //가동여부
            if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "가동여부"))
            {
                if (fpSpread1.Sheets[0].Cells[e.Row, e.Column].Text == "True" || fpSpread1.Sheets[0].Cells[e.Row, e.Column].Text == "")
                {
                    UIForm.FPMake.grdReMake(fpSpread1, e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "비가동코드") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "비가동코드_2") + "|3"
                        //						+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "가동시간") + "|1"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "근태횟수") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "시간외시간") + "|0"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "근태코드") + "|0"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "근태코드_2") + "|0"
                        );

                    //					fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "가동시간")].Text = "08:00";
                }
                else
                {
                    UIForm.FPMake.grdReMake(fpSpread1, e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "비가동코드") + "|1"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "비가동코드_2") + "|0"
                        //						+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "가동시간") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "근태횟수") + "|1"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "시간외시간") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "근태코드") + "|1"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "근태코드_2") + "|0"
                        );

                    //					fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "가동시간")].Text = "";
                }

                fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "비가동코드")].Text = "";
                fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "비가동명")].Text = "";
                fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "시간외시간")].Text = "";
                fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "근태코드")].Text = "";
                fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "근태횟수")].Text = "";
                fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "근태명")].Text = "";
                fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "근태유형")].Text = "";
                fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "비고")].Text = "";

            }
        }
        #endregion
    }
}
