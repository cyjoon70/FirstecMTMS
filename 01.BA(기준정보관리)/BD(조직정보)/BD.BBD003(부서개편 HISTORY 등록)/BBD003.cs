#region 작성정보
/*********************************************************************/
// 단위업무명 : 부서개편 HISTORY 등록
// 작 성 자 :   김 한 진
// 작 성 일 :   2014-06-02
// 작성내용 :   부서개편 HISTORY 수정 삭제 등록.
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

namespace BD.BBD003
{
    public partial class BBD003 : UIForm.FPCOMM1
    {
        #region 생성자
        public BBD003()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void BBD001_Load(object sender, System.EventArgs e)
        {
            //GroupBox1 초기화
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            //조회 콤보박스 세팅
            //gstrCOMCD

            SystemBase.ComboMake.C1Combo(cboReorgId, "usp_BBD003 @pTYPE = 'S3', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 0);

            SystemBase.ComboMake.C1Combo(cboReorgNm, "usp_BBD003 @pTYPE = 'S3', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 0);


            string strQuery = " usp_BBD003 'S3'";
            strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
            DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

            cboReorgId.Text = dt.Rows[1][1].ToString();
            cboReorgNm.Text = dt.Rows[0][1].ToString();

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            SystemBase.ComboMake.C1Combo(cboReorgId, "usp_BBD003 @pTYPE = 'S3', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 0);

            SystemBase.ComboMake.C1Combo(cboReorgNm, "usp_BBD003 @pTYPE = 'S3', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 0);


            string strQuery = " usp_BBD003 'S3'";
            strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
            DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

            cboReorgId.Text = dt.Rows[1][1].ToString();
            cboReorgNm.Text = dt.Rows[0][1].ToString();
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
        }
        #endregion

        #region fpButtonClick() 그리드 버튼클릭
        protected override void fpButtonClick(int Row, int Column)
        {
            try
            {
                //개편전부서조회
                if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "변경전 부서_2"))
                {
                    string strQuery = " usp_B_COMMON 'D023' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                    strQuery += " , @pLANG_CD='" + SystemBase.Base.gstrLangCd + "' ";
                    strQuery += " , @pREORG_ID = '" + cboReorgId.Text.Trim() + "' ";

                    string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                    string[] strSearch = new string[] { fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "변경전 부서")].Text, "" };
                    UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P04004", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "변경전부서명");
                    pu.Width = 500;
                    pu.ShowDialog();
                    if (pu.DialogResult == DialogResult.OK)
                    {
                        Regex rx1 = new Regex("#");
                        string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "변경전 부서")].Text = Msgs[0].ToString();	//부서코드
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "변경전부서명")].Text = Msgs[1].ToString();	//부서명

                        UIForm.FPMake.fpChange(fpSpread1, Row);//수정플래그
                    }
                }

                //개편후부서조회
                if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "변경후 부서_2"))
                {
                    string strQuery = " usp_B_COMMON 'D023' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                    strQuery += " , @pLANG_CD='" + SystemBase.Base.gstrLangCd + "' ";
                    strQuery += " , @pREORG_ID = '" + cboReorgNm.Text.Trim() + "' ";

                    string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                    string[] strSearch = new string[] { fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "변경후 부서")].Text, "" };
                    UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P04004", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "변경후부서명");
                    pu.Width = 500;
                    pu.ShowDialog();
                    if (pu.DialogResult == DialogResult.OK)
                    {
                        Regex rx1 = new Regex("#");
                        string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "변경후 부서")].Text = Msgs[0].ToString();	//부서코드
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "변경후부서명")].Text = Msgs[1].ToString();	//부서명

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

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                string strQuery = " usp_BBD003  'S1'";
                strQuery = strQuery + ", @pCO_CD='" + SystemBase.Base.gstrCOMCD + "' ";
                strQuery = strQuery + ", @pOLDREORG_ID ='" + cboReorgId.Text.Trim() + "' ";
                strQuery = strQuery + ", @pREORG_ID ='" + cboReorgNm.Text.Trim() + "' ";
                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0);
            }

            this.Cursor = System.Windows.Forms.Cursors.Default;
        }
        #endregion

        #region SaveExec() 폼에 입력된 데이타 저장 로직
        protected override void SaveExec()
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;


            if (SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", true) == true) // 그리드 상단 필수항목 체크
            {
                string ERRCode = "ER", MSGCode = "P0000";	//처리할 내용이 없습니다.
                string strKeyCd = "";

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

                            strKeyCd = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "변경전ID")].Text.ToString();
                            string strOldreorgID = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "변경전ID")].Text.ToString();
                            string strOldDeptCd = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "변경전 부서")].Text.ToString();
                            string strReorgID = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "변경후ID")].Text.ToString();
                            string strDeptCd = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "변경후 부서")].Text.ToString();
                            string strHistdesc = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "비고")].Text.ToString();


                            string strSql = " usp_BBD003 '" + strGbn + "'";
                            strSql = strSql + ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'";
                            strSql = strSql + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                            strSql = strSql + ", @pOLDREORG_ID = '" + cboReorgId.Text + "'";
                            strSql = strSql + ", @pOLDDEPT_Cd = '" + strOldDeptCd.Trim().ToUpper() + "'";
                            strSql = strSql + ", @pREORG_ID = '" + cboReorgNm.Text + "'";
                            strSql = strSql + ", @pDEPT_CD = '" + strDeptCd.Trim().ToUpper() + "'";
                            strSql = strSql + ", @pHIST_DESC = '" + strHistdesc.Trim().ToUpper() + "'";

                            if (strKeyCd.Trim() != "*")
                            {
                                //    strSql = strSql + ", @pBIZ_CD = '" + strBizCd.Trim() + "'";
                                //    strSql = strSql + ", @pCOST_DIST_TYPE = '" + strCostDistType.Trim() + "'";
                            }
                            strSql = strSql + ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
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
                    SearchExec();
                    UIForm.FPMake.GridSetFocus(fpSpread1, strKeyCd); //그리드 위치를 가져온다

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
            this.Cursor = System.Windows.Forms.Cursors.Default;
        }
        #endregion

        #region 불러오기 버튼 클릭시.
        private void btnCRT_RELOAD_Click(object sender, EventArgs e)
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                string strQuery = " usp_BBD003  'S2'";
                strQuery = strQuery + ", @pCO_CD='" + SystemBase.Base.gstrCOMCD + "' ";
                strQuery = strQuery + ", @pOLDREORG_ID ='" + cboReorgId.Text.Trim() + "' ";
                strQuery = strQuery + ", @pREORG_ID ='" + cboReorgNm.Text.Trim() + "' ";
                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0);

                for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                {
                    fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text = "I";
                }
            }



            this.Cursor = System.Windows.Forms.Cursors.Default;

        }
        #endregion
    }

}
