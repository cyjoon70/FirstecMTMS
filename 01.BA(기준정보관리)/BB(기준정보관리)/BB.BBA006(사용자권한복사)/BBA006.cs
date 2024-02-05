using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Collections;
using System.Data.SqlClient;

namespace BB.BBA006
{
    public partial class BBA006 : UIForm.FPCOMM2
    {
        #region 생성자
        public BBA006()
        {
            InitializeComponent();
        }
        #endregion

        #region 폼로드 이벤트
        private void BBA006_Load(object sender, EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(groupBox1);//필수적용
            SystemBase.Validation.GroupBox_Setting(groupBox2);//필수적용

            G2Etc[SystemBase.Base.GridHeadIndex(GHIdx2, "접근권한")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE='COMM', @pCODE = 'B001', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'");
            G2Etc[SystemBase.Base.GridHeadIndex(GHIdx2, "자료권한")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE='COMM', @pCODE = 'B002', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'");

            UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, true, 0, 0);
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0);
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            SystemBase.Validation.GroupBox_Reset(groupBox2);

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0);
            UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, true, 0, 0);
            txtUserId.Focus();
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                string strQuery = " usp_BBA006  'S1'";
                strQuery = strQuery + ", @pLANG_CD='" + SystemBase.Base.gstrLangCd + "' ";
                strQuery = strQuery + ", @pUSR_ID ='" + txtRollId.Text + "' ";
                strQuery = strQuery + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread2, strQuery, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, true, 0, 0, true);

            }

            this.Cursor = System.Windows.Forms.Cursors.Default;
        }
        #endregion

        #region 코드 변환시 명조회
        //사용자
        private void txtUserId_TextChanged(object sender, System.EventArgs e)
        {
            txtUserNm.Value = SystemBase.Base.CodeName("USR_ID", "USR_NM", "B_SYS_USER", txtUserId.Text, " AND CO_CD = '"+ SystemBase.Base.gstrCOMCD.ToString() +"'");
            txtRollId.Value = SystemBase.Base.CodeName("USR_ID", "ROLL_ID", "CO_SYS_USER_ROLL", txtUserId.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'");
        }
        #endregion

        #region 사용자조회 버튼
        private void btnUserId_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON 'B010', @pSPEC1 = '" + SystemBase.Base.gstrBIZCD + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' ";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtUserId.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P04003", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "사용자 조회");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtUserId.Text = Msgs[0].ToString();
                    txtUserNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "사용자조회 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }
        #endregion

        #region 부서별 사용자조회
        private void DeptUsrSearch(string DeptCd)
        {
            this.Cursor = Cursors.WaitCursor;

            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox2))
            {
                string strQuery = " usp_BBA006  'S2'";
                strQuery = strQuery + ", @pDEPT_CD ='" + DeptCd + "' ";
                strQuery = strQuery + ", @pREORG_ID ='" + SystemBase.Base.gstrREORG_ID + "' ";
                strQuery = strQuery + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0, true);		
            }

            this.Cursor = System.Windows.Forms.Cursors.Default;
        }
        #endregion

        #region SaveExec() 폼에 입력된 데이타 저장 로직
        protected override void SaveExec()
        {
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

                string ERRCode = "ER", MSGCode = "P0000";	//처리할 내용이 없습니다.

                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                try
                {
                    //행수만큼 처리
                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {
                        if (fpSpread1.Sheets[0].Cells[i, 1].Text == "True")
                        {
                            string strSql = " usp_BBA006 'U1'";
                            strSql = strSql + ", @pLANG_CD        = '" + SystemBase.Base.gstrLangCd + "'";
                            strSql = strSql + ", @pORG_USR_ID     = '" + txtRollId.Text + "'";
                            strSql = strSql + ", @pFOLLOW_USR_ID  = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "권한그룹")].Text.ToString() + "'";
                            strSql = strSql + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' ";
                            strSql = strSql + ", @pUP_ID = '" + SystemBase.Base.gstrUserID.ToString() +"' ";

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

                    this.Cursor = System.Windows.Forms.Cursors.Default;
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

                this.Cursor = System.Windows.Forms.Cursors.Default;
            }
        }
        #endregion

        #region 부서별 사용자 불러오기 버튼 클릭
        private void btnOk_Click(object sender, System.EventArgs e)
        {
            BBA006P1 pu = new BBA006P1();
            pu.ShowDialog();

            if (pu.DialogResult == DialogResult.OK)
            {

                string[] strMsgs = pu.ReturnVal;

                int intRowCount = strMsgs.Length;		//popup창에서 넘어온 Row수
                if (intRowCount > 0)
                {
                    fpSpread1.Sheets[0].RowCount = 0;

                    int intRealRow = fpSpread1.ActiveSheet.Rows.Count;		//현재그리드 행수

                    for (int i = 0; i < intRowCount; i++)
                    {
                        Regex rx1 = new Regex("!!");
                        string[] Msgs = rx1.Split(strMsgs[i].ToString());

                        DeptUsrSearch(Msgs[0].ToString());
                    }
                }
            }
        }

        private void btnUser_Click(object sender, System.EventArgs e)
        {
            try
            {
                BBA006P2 pu = new BBA006P2(fpSpread1);
                pu.ShowDialog();
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(f.Message, SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        #endregion

        private void fpSpread1_ButtonClicked(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            fpSpread1.Sheets[0].RowHeader.Cells[e.Row, 0].Text = "";
        }
    }
}
