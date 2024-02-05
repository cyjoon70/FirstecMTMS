#region 작성정보
/*********************************************************************/
// 단위업무명 :기초자료등록(집계)
// 작 성 자 : 김 현 근
// 작 성 일 : 2013-04-26
// 작성내용 : 기초자료등록(집계)
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
namespace HC.HCA014
{
    public partial class HCA014 : UIForm.FPCOMM1
    {
        string strEmpNo = "";
        public HCA014()
        {
            InitializeComponent();

            strEmpNo = SystemBase.Base.gstrUserID.Replace("FST", "").ToString();
        }


        #region 로그인 사용자 체크 후 필수유무
        private void UsrCheck()
        {
            //로그인 유저가 'ADMIN' 이거나 'INSA' 인 경우 수정가능하게 한다.
            if (strEmpNo == "ADMIN" || strEmpNo == "INSA")
            {
                txtEmpNo.Tag = ";1;;";
               // btnEmpNo.Tag = "";
                btnEmpNo.Enabled = true;
                SystemBase.Base.GroupBoxLang(groupBox1);
            }
            else
            {
                txtEmpNo.Tag = ";2;;";
                //btnEmpNo.Tag = "2";
                btnEmpNo.Enabled = false;
                SystemBase.Base.GroupBoxLang(groupBox1);
            }

            txtEmpNo.Text = strEmpNo;

            EmpDataInput(strEmpNo);
        }
        #endregion

        #region 사원데이터 자동기입
        private void EmpDataInput(string EmpNo)
        {
            string strQuery = "usp_H_COMMON @pType='H004', @pCOM_CD = '" + EmpNo + "' ";
            DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

            if (dt.Rows.Count > 0)
            {
                txtEmpNm.Value = dt.Rows[0][1].ToString();
                txtDeptNm.Value = dt.Rows[0][2].ToString();
                txtRollPstn.Value = dt.Rows[0][3].ToString();
                txtEntrDt.Value = dt.Rows[0][4].ToString();
                txtPayGrd1.Value = dt.Rows[0][5].ToString();
                txtEmpNo.Focus();
            }
            else
            {
                txtEmpNm.Value = "";
                txtDeptNm.Value = "";
                txtRollPstn.Value = "";
                txtEntrDt.Value = "";
                txtPayGrd1.Value = "";
                txtEmpNo.Focus();
            }
        }
        #endregion

        #region Form Load 시
        private void HCA014_Load(object sender, System.EventArgs e)
        {
            //GroupBox1 초기화
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            dtpYear.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddYears(-1).Year.ToString();

            //사용자 체크
            UsrCheck();
         	
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            dtpYear.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddYears(-1).Year.ToString();

            //사용자 체크
            UsrCheck();
    
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            try
            { 
                //조회조건 필수 체크
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1) == true)
                {
                    string strQuery = " usp_HCA014  @pTYPE = 'S1', @pYY = '" + dtpYear.Text + "', @pEMP_NO = '" + txtEmpNo.Text + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0, true);

                    if(fpSpread1.Sheets[0].Rows.Count > 0)
					{
						fpSpread1.Sheets[0].SetColumnMerge(2, FarPoint.Win.Spread.Model.MergePolicy.Always);
						fpSpread1.Sheets[0].Cells[0,2, fpSpread1.Sheets[0].Rows.Count-1, 2].VerticalAlignment =  FarPoint.Win.Spread.CellVerticalAlignment.Top;

						for(int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
						{
							if(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수정가능여부")].Text == "True")
							{
								if(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "문자여부")].Text == "True")
								{
									UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "기초항목(숫자)") + "|3");
									UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "기초항목(문자)") + "|0");
								}
								else
								{
									UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "기초항목(숫자)") + "|0");
									UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "기초항목(문자)") + "|3");
								}
							}
							else
							{
								UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "기초항목(숫자)") + "|3");
								UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "기초항목(문자)") + "|3");
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
           
            this.Cursor = System.Windows.Forms.Cursors.Default;
        }       
        #endregion

        #region Save
        protected override void SaveExec()
        {
            if (SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", false) == true)
            {
                string ERRCode = "ER", MSGCode = "P0000";	//처리할 내용이 없습니다.
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

                            string strSql = " usp_HCA014 @pTYPE = '" + strGbn + "' ";
                            strSql = strSql + ", @pYY = '" + dtpYear.Text + "'";
                            strSql = strSql + ", @pEMP_NO = '" + txtEmpNo.Text + "'";
                            strSql = strSql + ", @pUP_ID  = '" + SystemBase.Base.gstrUserID.ToString() + "'";
                            strSql = strSql + ", @pD_TYPE_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "기초항목코드")].Text + "' ";
                            strSql = strSql + ", @pD_TYPE_VAL1 = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "기초항목(숫자)")].Value + "' ";
                            strSql = strSql + ", @pD_TYPE_VAL2 = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "기초항목(문자)")].Text + "' ";

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

        #region 조회조건 팝업
        private void btnEmpNo_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = " usp_H_COMMON @pType='H003'";
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
                    txtDeptNm.Value = Msgs[2].ToString();
                    txtRollPstn.Value = Msgs[3].ToString();
                    txtEntrDt.Value = Msgs[4].ToString();
                    txtPayGrd1.Value = Msgs[5].ToString();
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
        private void txtEmpNo_TextChanged(object sender, EventArgs e)
        {
            EmpDataInput(txtEmpNo.Text);
        }
        #endregion

        #region 기초자료생성, 삭제
        private void btnDataInsert_Click(object sender, EventArgs e)
        {
            string msg = SystemBase.Base.MessageRtn("기초자료를 생성 하시겠습니까?");
            DialogResult dsMsg = MessageBox.Show(msg, "기초자료생성", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dsMsg == DialogResult.Yes)
            {
                DataSave("Y");
            }

            SearchExec();
        }

        private void btnDataDelete_Click(object sender, EventArgs e)
        {
            if (fpSpread1.Sheets[0].Rows.Count > 0)
            {
                string msg = SystemBase.Base.MessageRtn("기초자료를 삭제 하시겠습니까?");
                DialogResult dsMsg = MessageBox.Show(msg, "기초자료삭제", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (dsMsg == DialogResult.Yes)
                {
                    DataSave("N");
                }

                SearchExec();
            }
            else
            {
                MessageBox.Show(SystemBase.Base.MessageRtn("조회된 데이터가 존재하지 않습니다."), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private void DataSave(string Flag)
        {
            string ERRCode = "ER", MSGCode = "P0000";	//처리할 내용이 없습니다.
            SqlConnection dbConn = SystemBase.DbOpen.DBCON();
            SqlCommand cmd = dbConn.CreateCommand();
            SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

            try
            {
                string strType = "D1";

                if (Flag == "Y") { strType = "I1"; }

                string strSql = " usp_HCA014 @pTYPE = '" + strType + "' ";
                strSql = strSql + ", @pYY = '" + dtpYear.Text + "'";
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

    }
}
