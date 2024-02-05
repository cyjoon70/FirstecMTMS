using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using UIForm;
using System.Text.RegularExpressions;

namespace BB.BBA008
{
    public partial class BBA008 : UIForm.FPCOMM1
    {
        #region 생성자
        public BBA008()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void BBA008_Load(object sender, System.EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
            txtUser.Focus();
        }
        #endregion


        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                string strQuery = " usp_BBA008  'S1'";
                strQuery = strQuery + ", @pCO_CD ='" + SystemBase.Base.gstrCOMCD.ToString() + "' ";
                strQuery = strQuery + ", @pUSR_ID ='" + txtUser.Text + "' ";
                strQuery = strQuery + ", @pUSR_NM ='" + txtUsernm.Text + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0);
              
            }

            this.Cursor = System.Windows.Forms.Cursors.Default;
        }
        #endregion


        #region SaveExec() 폼에 입력된 데이타 저장 로직
        protected override void SaveExec()
        {
            if (SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", true))// 그리드 상단 필수항목 체크
            {
                string ERRCode = "ER", MSGCode = "SY001"; //처리할 내용이 없습니다.

                string strUSR_ID = "";

                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                try
                {
                    //행수만큼 처리
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

                            // 변경그룹 없으면 처리안함
                            if (string.IsNullOrEmpty(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "변경권한그룹")].Text))
                                continue;

                            string strSql = " usp_BBA008 '" + strGbn + "'";
                            strSql +=  ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'";
                            strSql +=  ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'";
                            strSql +=  ", @pUSR_ID = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "사용자ID")].Text.ToString() + "'";
                            strSql +=  ", @pREORG_ID = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "개편ID")].Text.ToString() + "'";
                            strSql +=  ", @pDEPT_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "부서코드")].Text.ToString() + "'";
                            strSql +=  ", @pPLANT_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "공장코드")].Text.ToString() + "'";
                            strSql +=  ", @pROLL_ID = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "변경권한그룹")].Text.ToString() + "'";
                            strSql +=  ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";

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
                    MSGCode = "SY002"; // 에러가 발생되어 데이터 처리가 취소되었습니다.
                }
            Exit:
                dbConn.Close();

                if (ERRCode == "OK")
                {
                    SearchExec();
                    UIForm.FPMake.GridSetFocus(fpSpread1, strUSR_ID);
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

        #region txtUser 변환시  txtUsernm 조회
        private void txtUser_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (txtUser.Text != "")
                    txtUsernm.Text = SystemBase.Base.CodeName("USR_ID", "USR_NM", "B_SYS_USER", txtUser.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                else
                    txtUsernm.Text = "";
            }
            catch { }
        }
        #endregion

        #region FrSpeadenu 컬럼 변환시 Name 조회
        private void fpSpread1_EditChange(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "변경권한그룹"))
            {
                string RollId = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "변경권한그룹")].Text.ToString();
                string RollIdNm = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", RollId, " AND COMP_CODE ='" + SystemBase.Base.gstrCOMCD + "' and MAJOR_CD = 'CO006' ");
                fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "변경권한그룹명")].Text = RollIdNm;

                UIForm.FPMake.fpChange(fpSpread1, e.Row);//수정플래그
            }

        }
        #endregion


        #region fpButtonClick() 그리드 버튼클릭
        protected override void fpButtonClick(int Row, int Column)
        {
            try
            {
                //변경권한그룹
                if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "변경권한그룹_2"))
                {
                    string strQuery = " usp_B_COMMON 'COMM_POP' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                    strQuery += " , @pLANG_CD='" + SystemBase.Base.gstrLangCd + "' ";
                    strQuery += " , @pREORG_ID = '" + SystemBase.Base.gstrREORG_ID + "' ";
                    strQuery += " , @pSPEC1 =   'CO006' ";

                    string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                    string[] strSearch = new string[] { fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "변경권한그룹")].Text, "" };
                    UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P1060", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "권한그룹조회");
                    pu.Width = 500;
                    pu.ShowDialog();
                    if (pu.DialogResult == DialogResult.OK)
                    {
                        Regex rx1 = new Regex("#");
                        string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "변경권한그룹")].Text = Msgs[0].ToString();	//변경권한그룹
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "변경권한그룹명")].Text = Msgs[1].ToString();	//변경권한그룹명

                        UIForm.FPMake.fpChange(fpSpread1, Row);//수정플래그
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY008", "그리드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }
        #endregion

    }
}
