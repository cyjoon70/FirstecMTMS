#region 작성정보
/*********************************************************************/
// 단위업무명 : 일근태등록(갑)
// 작 성 자 : 김 현 근
// 작 성 일 : 2013-04-10
// 작성내용 : 일근태등록(갑)
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

namespace HA.HAA005
{
    public partial class HAA005 : UIForm.FPCOMM1
    {
        #region 변수선언
        string strEmpNo = "";
        #endregion

        #region 생성자
        public HAA005()
        {
            InitializeComponent();
            strEmpNo = SystemBase.Base.gstrUserID.Replace("FST", "").ToString();
        }
        #endregion

        #region Form Load 시
        private void HAA005_Load(object sender, System.EventArgs e)
        {
            //GroupBox1 초기화
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            dtpDate.Text = SystemBase.Base.ServerTime("YYMMDD");

            //사용자체크
            UsrCheck();

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0);           
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

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            //조회조건 필수 체크
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1) == true)
            {
                try
                {
                    string strQuery = " usp_HAA005  @pTYPE = 'S1', @pINTERNAL_CD = '" + txtInternalCd.Text + "' ";
                    strQuery = strQuery + " , @pDATE = '" + dtpDate.Text + "' ";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0, true);

                    if (fpSpread1.Sheets[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                        {
                            UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "사원번호_2") + "|3");

                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "반영여부")].Text == "True")
                            {
                                UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "근태코드") + "|3"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "근태코드_2") + "|3"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "횟수") + "|3"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "시간") + "|3"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "비고") + "|3"
                                    );
                            }
                            else
                            {
                                UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "근태코드") + "|0"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "근태코드_2") + "|0"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "비고") + "|0"
                                    );

                                if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "근태유형")].Text == "1")
                                {
                                    UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "횟수") + "|1"
                                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "시간") + "|3"
                                        );
                                }
                                else if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "근태유형")].Text == "2")
                                {
                                    UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "횟수") + "|3"
                                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "시간") + "|1"
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
            if (UIForm.FPMake.FPUpCheck(fpSpread1) == true)// 그리드 상단 필수항목 체크
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

                            string strHour = null, strMin = null;

                            strKeyCd = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "사원번호")].Text;
                            string strSql = " usp_HAA005 @pTYPE = '" + strGbn + "' ";
                            strSql = strSql + ", @pDATE = '" + dtpDate.Text + "' ";
                            strSql = strSql + ", @pEMP_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "사원번호")].Text + "'";
                            strSql = strSql + ", @pDILIG_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "근태코드")].Text + "'";
                            strSql = strSql + ", @pDILIG_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "순번")].Text + "'";
                            strSql = strSql + ", @pDIR_INDIR = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "직간접구분")].Text + "'";
                            strSql = strSql + ", @pDILIG_CNT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "횟수")].Value + "'";
                            strSql = strSql + ", @pREMARK = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "비고")].Text + "'";
                            strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "시간")].Text.Replace("_", "").Replace(":", "") != "")
                            {
                                if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "시간")].Text.Replace("_", "").Replace(":", "").Length != 4)
                                {
                                    Trans.Rollback();
                                    ERRCode = "WR";
                                    i = i + 1;
                                    MSGCode = i.ToString() + " 열의 시간 정보가 잘못되었습니다.";
                                    goto Exit;
                                }

                                strHour = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "시간")].Text.Substring(0, 2);
                                strMin = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "시간")].Text.Substring(3, 2);

                                strSql = strSql + ", @pDILIG_HH  = '" + strHour + "'";
                                strSql = strSql + ", @pDILIG_MM  = '" + strMin + "'";
                            }

                            strSql = strSql + ", @pUP_ID  = '" + SystemBase.Base.gstrUserID.ToString() + "'";

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

        #region 부서코드 팝업
        private void btnDept_Click(object sender, EventArgs e)
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
         #endregion

        #region 조회조건 TextChanged
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
        #endregion
        
        #region 그리드 상 팝업
        protected override void fpButtonClick(int Row, int Column)
        {
            //사원번호
            if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "사원번호_2"))
            {
                string strQuery = " usp_H_COMMON 'H010', @pSPEC1 = '" + txtInternalCd.Text + "', @pSPEC2 = '" + dtpDate.Text + "' ";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };
                string[] strSearch = new string[] { fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "사원번호")].Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("H00004", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "사원번호 팝업");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "사원번호")].Text = Msgs[0].ToString();
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "이름")].Text = Msgs[1].ToString();
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "직위")].Text = Msgs[2].ToString();
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "직간접구분")].Text = Msgs[4].ToString();

                    UIForm.FPMake.fpChange(fpSpread1, Row);//수정플래그

                }
            }

            //근태코드
            if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "근태코드_2"))
            {
                string strQuery = " usp_H_COMMON 'H012' ";
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

                    if (Msgs[2].ToString() == "1")
                    {
                        UIForm.FPMake.grdReMake(fpSpread1, Row, SystemBase.Base.GridHeadIndex(GHIdx1, "횟수") + "|1");
                        UIForm.FPMake.grdReMake(fpSpread1, Row, SystemBase.Base.GridHeadIndex(GHIdx1, "시간") + "|3");

                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "시간")].Text = "";
                    }
                    else if (Msgs[2].ToString() == "2")
                    {
                        UIForm.FPMake.grdReMake(fpSpread1, Row, SystemBase.Base.GridHeadIndex(GHIdx1, "횟수") + "|3");
                        UIForm.FPMake.grdReMake(fpSpread1, Row, SystemBase.Base.GridHeadIndex(GHIdx1, "시간") + "|1");

                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "횟수")].Text = "";
                    }

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
                string strQuery = " usp_H_COMMON 'H011', @pSPEC1 = '" + txtInternalCd.Text + "', @pSPEC2 = '" + dtpDate.Text + "' ";
                strQuery = strQuery + " , @pCOM_CD = '" + fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "사원번호")].Text + "' ";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

                if (dt.Rows.Count > 0)
                {
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "이름")].Text = dt.Rows[0][1].ToString();
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "직위")].Text = dt.Rows[0][2].ToString();
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "직간접구분")].Text = dt.Rows[0][4].ToString();
                }
                else
                {
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "이름")].Text = "";
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "직위")].Text = "";
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "직간접구분")].Text = "";

                    MessageBox.Show(SystemBase.Base.MessageRtn("존재하지 않는 사원번호입니다."), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }

            //근태코드
            if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "근태코드"))
            {
                string strQuery = " usp_H_COMMON 'H013'";
                strQuery = strQuery + " , @pCOM_CD = '" + fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "근태코드")].Text + "' ";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

                if (dt.Rows.Count > 0)
                {
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "근태명")].Text = dt.Rows[0][1].ToString();
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "근태유형")].Text = dt.Rows[0][2].ToString();

                    if (dt.Rows[0][2].ToString() == "1")
                    {
                        UIForm.FPMake.grdReMake(fpSpread1, Row, SystemBase.Base.GridHeadIndex(GHIdx1, "횟수") + "|1");
                        UIForm.FPMake.grdReMake(fpSpread1, Row, SystemBase.Base.GridHeadIndex(GHIdx1, "시간") + "|3");

                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "시간")].Text = "";
                    }
                    else if (dt.Rows[0][2].ToString() == "2")
                    {
                        UIForm.FPMake.grdReMake(fpSpread1, Row, SystemBase.Base.GridHeadIndex(GHIdx1, "횟수") + "|3");
                        UIForm.FPMake.grdReMake(fpSpread1, Row, SystemBase.Base.GridHeadIndex(GHIdx1, "시간") + "|1");

                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "횟수")].Text = "";
                    }
                }
                else
                {
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "근태명")].Text = "";
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "근태유형")].Text = "";

                    UIForm.FPMake.grdReMake(fpSpread1, Row, SystemBase.Base.GridHeadIndex(GHIdx1, "횟수") + "|3");
                    UIForm.FPMake.grdReMake(fpSpread1, Row, SystemBase.Base.GridHeadIndex(GHIdx1, "시간") + "|3");

                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "시간")].Text = "";
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "횟수")].Text = "";

                    MessageBox.Show(SystemBase.Base.MessageRtn("존재하지 않는 근태코드입니다."), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }
        #endregion

    }
}
