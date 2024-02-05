  using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Data;
using System.Data.SqlClient;

namespace BA.BAA002
{
    public partial class BAA002 : UIForm.FPCOMM1
    {
        #region 생성자
        public BAA002()
        {
            InitializeComponent();
        }
        #endregion

        #region 팝업창 열기
        private void cmdMenu_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_CO_COMM_CODE @pTYPE = 'MENU' ";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtMenuid.Text };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P1010", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "메뉴조회");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtMenuid.Text = Msgs[0].ToString();
                    txtMenunm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY008", "메뉴조회 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                string strQuery = " usp_BAA002  'S1'";
                strQuery = strQuery + ", @pMENU_ID ='" + txtMenuid.Text + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0);
            }
        }
        #endregion

        #region SaveExec() 폼에 입력된 데이타 저장 로직
        protected override void SaveExec()
        {
            if ((SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", true) == true))// 그리드 필수항목 체크 
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))  //컨트롤 필수여부체크 
                {

                    string ERRCode = "ER", MSGCode = "SY001";	//처리할 내용이 없습니다.

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
                            string strSql = "";

                            if (strHead.Length > 0)
                            {
                                switch (strHead)
                                {
                                    case "U": strGbn = "U1"; break;
                                    default: strGbn = ""; break;
                                }

                                string strMENU_ID = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "메뉴ID")].Text.ToString();
                                string strT01 = "0"; if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "신규")].Text.ToString() == "True") strT01 = "1";
                                string strT02 = "0"; if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "조회")].Text.ToString() == "True") strT02 = "1";
                                string strT03 = "0"; if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "행복사")].Text.ToString() == "True") strT03 = "1";
                                string strT04 = "0"; if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "행추가")].Text.ToString() == "True") strT04 = "1";
                                string strT05 = "0"; if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "행취소")].Text.ToString() == "True") strT05 = "1";
                                string strT06 = "0"; if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "행삭제")].Text.ToString() == "True") strT06 = "1";
                                string strT07 = "0"; if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "삭제")].Text.ToString() == "True") strT07 = "1";
                                string strT08 = "0"; if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "저장")].Text.ToString() == "True") strT08 = "1";
                                string strT09 = "0"; if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Excel")].Text.ToString() == "True") strT09 = "1";
                                string strT10 = "0"; if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "출력")].Text.ToString() == "True") strT10 = "1";
                                string strT11 = "0"; if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "도움말")].Text.ToString() == "True") strT11 = "1";
                                string strT12 = "0"; if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "종료")].Text.ToString() == "True") strT12 = "1";

                                strSql = " usp_BAA002 '" + strGbn + "'";
                                strSql = strSql + ", @pMENU_ID = '" + strMENU_ID.Trim() + "'";
                                strSql = strSql + ", @pT01 = '" + strT01 + "'";
                                strSql = strSql + ", @pT02 = '" + strT02 + "'";
                                strSql = strSql + ", @pT03 = '" + strT03 + "'";
                                strSql = strSql + ", @pT04 = '" + strT04 + "'";
                                strSql = strSql + ", @pT05 = '" + strT05 + "'";
                                strSql = strSql + ", @pT06 = '" + strT06 + "'";
                                strSql = strSql + ", @pT07 = '" + strT07 + "'";
                                strSql = strSql + ", @pT08 = '" + strT08 + "'";
                                strSql = strSql + ", @pT09 = '" + strT09 + "'";
                                strSql = strSql + ", @pT10 = '" + strT10 + "'";
                                strSql = strSql + ", @pT11 = '" + strT11 + "'";
                                strSql = strSql + ", @pT12 = '" + strT12 + "'";
                                strSql = strSql + ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                                DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                                ERRCode = ds.Tables[0].Rows[0][0].ToString();
                                MSGCode = ds.Tables[0].Rows[0][1].ToString();

                                if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
                                else { int intRows = fpSpread_ReType(fpSpread1, strGbn, i); i = intRows; }
                            }
                        }
                        Trans.Commit();
                    }
                    catch (Exception f)
                    {
                        SystemBase.Loggers.Log(this.Name, f.ToString());
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
            }
        }
        #endregion

        #region txtMenuid 변환시  Menunm 조회
        private void txtMenuid_TextChanged(object sender, System.EventArgs e)
        {
            string strSql = " ";
            txtMenunm.Value = SystemBase.Base.CodeName("MENU_ID", "MENU_NAME", "CO_SYS_MENU", txtMenuid.Text, strSql);
        }
        #endregion

        #region Form Load 시
        private void BAA002_Load(object sender, System.EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(groupBox1);	//필수 적용
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
        }
        #endregion

        #region 행쿼리후 그리드 재정의
        private static int fpSpread_ReType(FarPoint.Win.Spread.FpSpread baseGrid, string strGbn, int intRow)
        {
            if (strGbn == "U1")
            {
                baseGrid.Sheets[0].RowHeader.Cells[intRow, 0].Text = "";
                return intRow;
            }
            else return 0;
        }
        #endregion
    }
}
