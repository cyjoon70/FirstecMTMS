using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Data;
using System.Data.SqlClient;


namespace BA.BAA001
{
    public partial class BAA001 : UIForm.FPCOMM1
    {
        #region 변수선언
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox1;
        #endregion

        #region 생성자
        public BAA001()
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

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
            txtMenuid.Focus();
        }
        #endregion

        #region 행추가 버튼 클릭 이벤트
        protected override void RowInsExec()
        {
            UIForm.FPMake.RowInsert(fpSpread1);

            fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "레포트ID")].Text = "*";
            fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "창열림옵션")].Value = "P";
            fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "사용여부")].Text = "True";

        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                string strMenuId = txtMenuid.Text.Trim();
                if (txtMenuid.Text == "*") strMenuId = "";

                string strQuery = " usp_BAA001  'S1'";
                strQuery = strQuery + ", @pMENU_ID ='" + strMenuId + "' ";
                strQuery = strQuery + ", @pMENU_NAME ='" + txtMenunm.Text + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0);
            }
        }
        #endregion

        #region SaveExec() 폼에 입력된 데이타 저장 로직
        protected override void SaveExec()
        {
            //Major 코드 필수항목 체크
            if (SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", true))// 그리드 필수항목 체크 
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
                        if (strHead.Length > 0)
                        {
                            switch (strHead)
                            {
                                case "U": strGbn = "U1"; break;
                                case "D": strGbn = "D1"; break;
                                case "I": strGbn = "I1"; break;
                                default: strGbn = ""; break;
                            }

                            string strMENU_ID = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "메뉴ID")].Text.ToString();
                            string strMENU_NAME = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "메뉴명")].Text.ToString();
                            string strUP_MENU_ID = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "상위메뉴ID")].Text.ToString();
                            string strMENU_POS = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "순서")].Text.ToString();
                            string strPGM_ID = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "실행프로그램")].Text.ToString();
                            string strRPT_ID = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "레포트ID")].Text.ToString();
                            string strShowKind = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "창열림옵션")].Value.ToString();
                            string strUSE = "N"; if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "사용여부")].Text.ToString() == "True") strUSE = "Y";
                            string strEND = "N"; if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "END구분")].Text.ToString() == "True") strEND = "Y";
                            string strPGM_KIND = "";
                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "구분")].Text.ToString() != "")
                                strPGM_KIND = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "구분")].Value.ToString();

                            string strSql = " usp_BAA001 '" + strGbn + "'";
                            strSql = strSql + ", @pMENU_ID	  = '" + strMENU_ID.Trim() + "'";
                            strSql = strSql + ", @pUP_MENU_ID = '" + strUP_MENU_ID.Trim() + "'";
                            strSql = strSql + ", @pEND_FLAG   = '" + strEND + "'";
                            strSql = strSql + ", @pMENU_POS	  = '" + strMENU_POS + "'";
                            strSql = strSql + ", @pMENU_NAME    = '" + strMENU_NAME + "'";
                            strSql = strSql + ", @pPGM_ID     = '" + strPGM_ID + "'";
                            strSql = strSql + ", @pRPT_ID     = '" + strRPT_ID + "'";
                            strSql = strSql + ", @pSHOW_KIND  = '" + strShowKind + "'";
                            strSql = strSql + ", @pUSE_FLAG   = '" + strUSE + "'";
                            strSql = strSql + ", @pPGM_KIND   = '" + strPGM_KIND + "'";


                            strSql = strSql + ", @pUP_ID	  = '" + SystemBase.Base.gstrUserID + "'";

                            DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                            ERRCode = ds.Tables[0].Rows[0][0].ToString();
                            MSGCode = ds.Tables[0].Rows[0][1].ToString();

                            if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
                        }
                    }
                    Trans.Commit();
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
                    SearchExec();
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

        #region txtMenuid 변환시  Menunm 조회
        private void txtMenuid_TextChanged(object sender, System.EventArgs e)
        {
            string strSql = "";
            txtMenunm.Value = SystemBase.Base.CodeName("MENU_ID", "MENU_NAME", "CO_SYS_MENU", txtMenuid.Text, strSql);
        }
        #endregion

        #region Form Load 시
        private void BAA001_Load(object sender, System.EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(groupBox1);	//컨트롤 필수 Setting

            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "창열림옵션")] = SystemBase.ComboMake.ComboOnGrid("usp_CO_COMM_CODE @pType='COMM', @pCODE = 'CO003', @pCOMP_CODE = 'SYS'", 0); //창열림옵션
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "구분")] = SystemBase.ComboMake.ComboOnGrid("usp_CO_COMM_CODE @pType='COMM', @pCODE = 'CO004', @pCOMP_CODE = 'SYS'", 1); //창열림옵션
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
        }
        #endregion

        #region 화면ID등록시 실행프로그램 자동등록
        private void fpSpread1_LeaveCell(object sender, FarPoint.Win.Spread.LeaveCellEventArgs e)
        {
            //if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "메뉴ID"))
            //{
            //    string strMenuId = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "메뉴ID")].Text;

            //    if (strMenuId.Length >= 6)
            //    {
            //        string ExePrg = strMenuId + "." + strMenuId;

            //        //switch (strMenuId.Substring(0, 1))
            //        //{
            //        //    case "A":
            //        //        ExePrg = "ACC." + strMenuId + "." + strMenuId;
            //        //        break;
            //        //    case "B":
            //        //        ExePrg = "BASE." + strMenuId + "." + strMenuId;
            //        //        break;
            //        //    case "C":
            //        //        ExePrg = "COST." + strMenuId + "." + strMenuId;
            //        //        break;
            //        //    case "E":
            //        //        ExePrg = "EIS." + strMenuId + "." + strMenuId;
            //        //        break;
            //        //    case "I":
            //        //        ExePrg = "INTO." + strMenuId + "." + strMenuId;
            //        //        break;
            //        //    case "M":
            //        //        ExePrg = "MATL." + strMenuId + "." + strMenuId;
            //        //        break;
            //        //    case "P":
            //        //        ExePrg = "PROD." + strMenuId + "." + strMenuId;
            //        //        break;
            //        //    case "Q":
            //        //        ExePrg = "QATY." + strMenuId + "." + strMenuId;
            //        //        break;
            //        //    case "S":
            //        //        ExePrg = "SALE." + strMenuId + "." + strMenuId;
            //        //        break;
            //        //    case "Z":
            //        //        ExePrg = "COMM." + strMenuId + "." + strMenuId;
            //        //        break;
            //        //    default:

            //        //        break;
            //        //}

            //        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "실행프로그램")].Text = ExePrg;
            //    }
            //}
        }
        #endregion

    }
}
