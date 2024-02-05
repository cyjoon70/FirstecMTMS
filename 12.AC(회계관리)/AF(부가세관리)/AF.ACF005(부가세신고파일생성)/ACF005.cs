

#region 작성정보
/*********************************************************************/
// 단위업무명 : 부가세신고파일생성
// 작 성 자 : 유 재 규
// 작 성 일 : 2013-03-11
// 작성내용 : 부가세신고파일생성
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
using System.IO;

namespace AF.ACF005
{
    public partial class ACF005 : UIForm.Buttons
    {
        public ACF005()
        {
            InitializeComponent();
        }

        #region Form Load 시
        private void ACF005_Load(object sender, System.EventArgs e)
        {
            SystemBase.ComboMake.C1Combo(cboBizAreaCd, "usp_B_COMMON @pTYPE ='BIZ', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9);      //사업장
            SystemBase.ComboMake.C1Combo(cboREPORT_PER, "SELECT '1' CODE, '1기' NAME, 'N' UNION SELECT '2' CODE, '2기' NAME, 'N' ", 9); //기구분
            SystemBase.ComboMake.C1Combo(cboREPORT_DIV, "SELECT '1' CODE, '예정' NAME, 'N' UNION SELECT '2' CODE, '확정' NAME, 'N' ", 9); //신고구분
            //기구분
            //신고구분
            NewExec();
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            ////////////////////////////그룹박스 초기화 //////////////////////////////////////////////////////////////////////////
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            dtpIssue_Dt_F.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-3).ToShortDateString();
            dtpIssue_Dt_T.Value = SystemBase.Base.ServerTime("YYMMDD");

            dtpREPORT_DT.Value = SystemBase.Base.ServerTime("YYMMDD");

            dtpREVERS_YY.Value = SystemBase.Base.ServerTime("YYMMDD");
        }
        #endregion

        #region 실행버튼 클릭
        private void btnSave_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;

            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                string ERRCode = "ER", MSGCode = "SY001";	//처리할 내용이 없습니다.

                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                try
                {
                    string strQuery = " usp_ACF005 ";
                    strQuery += " @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    if (optWORK_TYPE_R.Checked == true) strQuery = strQuery + ", @pWORK_TYPE ='R' ";
                    else strQuery = strQuery + ", @pWORK_TYPE ='C' ";
                    if (optVAT_TYPE_A.Checked == true) strQuery = strQuery + ", @pVAT_TYPE ='A' ";
                    else strQuery = strQuery + ", @pVAT_TYPE ='B' ";
                    strQuery += ", @pISSUE_DT_F = '" + dtpIssue_Dt_F.Text + "' ";
                    strQuery += ", @pISSUE_DT_T = '" + dtpIssue_Dt_T.Text + "' ";
                    strQuery += ", @pTAX_BIZ_CD = '" + cboBizAreaCd.SelectedValue.ToString() + "' ";
                    strQuery += ", @pREPORT_DT = '" + dtpREPORT_DT.Text + "' ";
                    strQuery += ", @pREVERS_YY = '" + dtpREVERS_YY.Text + "' ";
                    strQuery += ", @pREPORT_PER = '" + cboREPORT_PER.SelectedValue.ToString() + "' ";
                    strQuery += ", @pREPORT_DIV = '" + cboREPORT_DIV.SelectedValue.ToString() + "' ";
                    strQuery += ", @pUPD_EMP_NO = '" + SystemBase.Base.gstrUserID + "'";
                    strQuery += ", @pUPD_IP = '" + SystemBase.Base.gstrUserIp + "'";

                    DataTable dt = SystemBase.DbOpen.TranDataTable(strQuery, dbConn, Trans);
                    if (dt.Rows.Count > 0)
                    {
                        ERRCode = dt.Rows[0][0].ToString();
                        MSGCode = dt.Rows[0][1].ToString();

                        if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
                        if (optWORK_TYPE_R.Checked == true)
                        {
                            //byte[] MyData = null;
                            //MyData = (byte[])dt.Rows[0]["HOME_TAX_FILE"];
                            //int ArraySize = new int();
                            //ArraySize = MyData.GetUpperBound(0);

                            //string FilePath = SystemBase.Base.ProgramWhere + @"\부가세신고파일\" + txtFileNm.Text;
                            string FilePath = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + @"\" + txtFileNm.Text + ".txt";

                            StreamWriter sw = new StreamWriter(new FileStream(FilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite));
                            //fs.Write(MyData, 0, ArraySize + 1);
                            
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                sw.Write(dt.Rows[i]["HOME_TAX_FILE"].ToString());
                            }

                            sw.Close();

                            System.Diagnostics.Process.Start(FilePath);
                        }

                        Trans.Commit();
                    }
                    else
                    {
                        Trans.Rollback(); goto Exit;
                    }
                }
                catch
                {
                    Trans.Rollback();
                    MSGCode = "SY002";	//에러가 발생하여 데이터 처리가 취소되었습니다.
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
            this.Cursor = Cursors.Default;
        }
        #endregion

        private void optWORK_TYPE_R_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (optWORK_TYPE_R.Checked == true && optVAT_TYPE_A.Checked == true)
                {
                    txtFileNm.Tag = "파일명;1;;";
                    dtpREVERS_YY.Tag = "귀속년도;1;;";
                    cboREPORT_PER.Tag = "기구분;1;;";
                    cboREPORT_DIV.Tag = "신고구분;1;;";
                    txtFileNm.Value = dtpREPORT_DT.Text.Replace("-", "") + ".101";
                    dtpREVERS_YY.Value = SystemBase.Base.ServerTime("YYMMDD");
                }
                else
                {
                    txtFileNm.Value = "";
                    dtpREVERS_YY.Value = "";
                    cboREPORT_PER.SelectedValue = "";
                    cboREPORT_DIV.SelectedValue = "";

                    txtFileNm.Tag = "파일명;2;;";
                    dtpREVERS_YY.Tag = "귀속년도;2;;";
                    cboREPORT_PER.Tag = "기구분;2;;";
                    cboREPORT_DIV.Tag = "신고구분;2;;";
                }
                SystemBase.Validation.GroupBox_Setting(groupBox1);
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void optVAT_TYPE_A_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (optWORK_TYPE_R.Checked == true && optVAT_TYPE_A.Checked == true)
                {
                    txtFileNm.Tag = "파일명;1;;";
                    dtpREVERS_YY.Tag = "귀속년도;1;;";
                    cboREPORT_PER.Tag = "기구분;1;;";
                    cboREPORT_DIV.Tag = "신고구분;1;;";
                    txtFileNm.Value = dtpREPORT_DT.Text.Replace("-","") + ".101";
                    dtpREVERS_YY.Value = SystemBase.Base.ServerTime("YYMMDD");
                }
                else
                {
                    txtFileNm.Value = "";
                    dtpREVERS_YY.Value = "";
                    cboREPORT_PER.SelectedValue = "";
                    cboREPORT_DIV.SelectedValue = "";

                    txtFileNm.Tag = "파일명;2;;";
                    dtpREVERS_YY.Tag = "귀속년도;2;;";
                    cboREPORT_PER.Tag = "기구분;2;;";
                    cboREPORT_DIV.Tag = "신고구분;2;;";
                }
                SystemBase.Validation.GroupBox_Setting(groupBox1);
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dtpREPORT_DT_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (optWORK_TYPE_R.Checked == true && optVAT_TYPE_A.Checked == true)
                {
                    txtFileNm.Value = dtpREPORT_DT.Text.Replace("-", "") + ".101";
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
