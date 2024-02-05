#region 작성정보
/*********************************************************************/
// 단위업무명 : 사업별품질담당등록
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-02-21
// 작성내용 : 사업별품질담당등록 및 관리
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

namespace QB.QBA031
{
    public partial class QBA031 : UIForm.FPCOMM1
    {
        #region 변수선언
        int SDown = 1;		// 조회 횟수
        int AddRow = 100;	// 조회 건수
        #endregion

        #region 생성자
        public QBA031()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load시
        private void PCC045_Load(object sender, System.EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            //조회 콤보박스 세팅
            SystemBase.ComboMake.C1Combo(cboLpart, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'S022', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 3); //사업대분류
            SystemBase.ComboMake.C1Combo(cboMpart, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'S023', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 3); //사업중분류

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
            txtEntCd.Focus();
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            fpSpread1.Sheets[0].Rows.Count = 0;
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            try
            {
                string strQuery = " usp_QBA031  @pTYPE = 'S1'";
                strQuery += ", @pENT_CD = '" + txtEntCd.Text + "' ";
                strQuery += ", @pENT_NM = '" + txtEntNm.Text + "' ";
                strQuery += ", @pL_PART = '" + Convert.ToString(cboLpart.SelectedValue) + "' ";
                strQuery += ", @pM_PART = '" + Convert.ToString(cboMpart.SelectedValue) + "' ";
                strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0);
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            this.Cursor = System.Windows.Forms.Cursors.Default;
        }
        #endregion

        #region SaveExec() 폼에 입력된 데이타 저장 로직
        protected override void SaveExec()
        {
            this.Cursor = Cursors.WaitCursor;

            if (SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", true))
            {
                string ERRCode = "ER", MSGCode = "P0000"; //처리할 내용이 없습니다.
                string strENT_CD = "";
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
                                default: strGbn = ""; break;
                            }

                            strENT_CD = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "사업코드")].Text;
                            string strSql = " usp_QBA031 '" + strGbn + "'";
                            strSql += ", @pENT_CD = '" + strENT_CD + "'";
                            strSql += ", @pQC_MAC_RES_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "전자QC담당자ID")].Text + "'";
                            strSql += ", @pQC_ELE_RES_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "기계QC담당자ID")].Text + "'";
                            strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                            strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                            DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                            ERRCode = ds.Tables[0].Rows[0][0].ToString();
                            MSGCode = ds.Tables[0].Rows[0][1].ToString();

                            if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
                        }
                    }
                    Trans.Commit();
                }
                catch (Exception e)
                {
                    SystemBase.Loggers.Log(this.Name, e.ToString());
                    Trans.Rollback();
                    ERRCode = "ER";
                    MSGCode = "P0001";	//에러가 발생되어 데이터 처리가 취소되었습니다.
                }
            Exit:
                dbConn.Close();
                if (ERRCode == "OK")
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    SearchExec();
                    UIForm.FPMake.GridSetFocus(fpSpread1, strENT_CD, SystemBase.Base.GridHeadIndex(GHIdx1, "사업코드"));
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

        #region 그리드 버튼 클릭
        private void fpSpread1_ButtonClicked(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "전자QC담당자ID_2"))
            {
                string strQuery = " usp_QBA031 @pTYPE = 'P1' , @pGRES_CD='F008-G', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                string[] strWhere = new string[] { "@pRES_CD", "@pRES_DIS" };
                string[] strSearch = new string[] { fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "전자QC담당자ID")].Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00068", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "자원 조회");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "전자QC담당자ID")].Text = Msgs[0].ToString();
                    fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "전자QC담당자명")].Text = Msgs[1].ToString();
                }
            }
            else if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "기계QC담당자ID_2"))
            {
                string strQuery = " usp_QBA031 @pTYPE = 'P1' , @pGRES_CD='F007-G', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                string[] strWhere = new string[] { "@pRES_CD", "@pRES_DIS" };
                string[] strSearch = new string[] { fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "기계QC담당자ID")].Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00068", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "자원 조회");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "기계QC담당자ID")].Text = Msgs[0].ToString();
                    fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "기계QC담당자명")].Text = Msgs[1].ToString();
                }
            }
        }
        #endregion

        #region 그리드 ChangedEvent
        protected override void fpSpread1_ChangeEvent(int Row, int Column)
        {
            if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "전자QC담당자ID"))
            {
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "전자QC담당자명")].Text
                    = SystemBase.Base.CodeName("RES_CD", "RES_DIS", "P_RESO_GROUP", fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "전자QC담당자ID")].Text, " AND GRES_CD = 'F008-G' AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");

            }
            else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "기계QC담당자ID"))
            {
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "기계QC담당자명")].Text
                    = SystemBase.Base.CodeName("RES_CD", "RES_DIS", "P_RESO_GROUP", fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "기계QC담당자ID")].Text, " AND GRES_CD = 'F007-G' AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");

            }
        }
        #endregion
		
    }
}
