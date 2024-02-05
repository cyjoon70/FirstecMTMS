#region 작성정보
/*********************************************************************/
// 단위업무명 : 구매담당등록
// 작 성 자   : 김창진
// 작 성 일   : 2013-09-25
// 작성내용   : 하도급업체관리(통합원가)
// 수 정 일   :
// 수 정 자   :
// 수정내용   :
// 비    고   :
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

namespace MA.MAB004
{
    public partial class MAB004 : UIForm.FPCOMM1
    {
        #region 변수선언
        private bool form_act_chk = false;
        string strBtn = "N";
        #endregion

        #region 생성자
        public MAB004()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void MAB004_Load(object sender, System.EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(groupBox1);
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            fpSpread1.Sheets[0].Rows.Count = 0;
        }
        #endregion

        #region 행추가 버튼 클릭 이벤트
        protected override void RowInsExec()
        {
            UIForm.FPMake.RowInsert(fpSpread1);

            fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "사용여부")].Text = "True";
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            try
            {
                string strQuery = " usp_MAB004  @pTYPE = 'S1'";
                strQuery += ", @pPROJECT_NO = '" + txtProjNo.Text + "' ";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0);

                UIForm.FPMake.grdReMake(fpSpread1, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호_2") + "|5");
                UIForm.FPMake.grdReMake(fpSpread1, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드_2") + "|5");
                UIForm.FPMake.grdReMake(fpSpread1, SystemBase.Base.GridHeadIndex(GHIdx1, "거래처_2") + "|5");

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
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

                            // 그리드 상단 필수항목 체크

                            string strSql = " usp_MAB004 '" + strGbn + "'";
                            strSql += ", @pPROJECT_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Value.ToString() + "'";
                            strSql += ", @pITEM_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text + "'";
                            strSql += ", @pCUST_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "거래처")].Text + "'";
                            strSql += ", @pCONTRACT_WAY = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "계약방법")].Text + "'";
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
                    MSGCode = e.Message;
                    //MSGCode = "P0001";	//에러가 발생되어 데이터 처리가 취소되었습니다.
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

                this.Cursor = Cursors.Default;
            }
        }
        #endregion

        #region 그리드 버튼 클릭
        protected override void fpButtonClick(int Row, int Column)
        {
            strBtn = "Y";
            if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드_2"))
            {
                try
                {
                    WNDW005 pu = new WNDW005(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text, "30");
                    pu.ShowDialog();
                    if (pu.DialogResult == DialogResult.OK)
                    {
                        string[] Msgs = pu.ReturnVal;

                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text = Msgs[2].ToString();
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목명")].Text = Msgs[3].ToString();
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "규격")].Text = Msgs[7].ToString();

                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //데이터 조회 중 오류가 발생하였습니다.
                }
            }
            //프로젝트번호
            else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호_2"))
            {
                try
                {
                    WNDW007 pu = new WNDW007(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text, "N");
                    pu.ShowDialog();
                    if (pu.DialogResult == DialogResult.OK)
                    {
                        string[] Msgs = pu.ReturnVal;

                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text = Msgs[3].ToString();

                        UIForm.FPMake.fpChange(fpSpread1, Row);//수정플래그
                    }

                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //데이터 조회 중 오류가 발생하였습니다.
                }
            }
            else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "거래처_2"))
            {
                try
                {
                    WNDW002 pu = new WNDW002(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "거래처")].Text, "N");
                    pu.ShowDialog();
                    if (pu.DialogResult == DialogResult.OK)
                    {
                        string[] Msgs = pu.ReturnVal;

                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "거래처")].Text = Msgs[1].ToString();
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "거래처명")].Text = Msgs[2].ToString();
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "사업자등록번호")].Text = Msgs[5].ToString();

                        UIForm.FPMake.fpChange(fpSpread1, Row);//수정플래그
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            strBtn = "N";
        }
        #endregion

        #region fpSpread1_Change
        protected override void fpSpread1_ChangeEvent(int Row, int Column)
        {
            if (strBtn == "N")
            {
                //품목코드
                if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드"))
                {
                    string Query = " usp_M_COMMON @pTYPE = 'M013', @pCODE = '" + fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                    DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);

                    if (dt.Rows.Count > 0)
                    {
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목명")].Text = dt.Rows[0]["ITEM_NM"].ToString();
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "규격")].Text = dt.Rows[0]["ITEM_SPEC"].ToString();
                    }
                    else
                    {
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목명")].Text = "";
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "규격")].Text = "";
                    }
                }
                //품목코드
                else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "거래처"))
                {
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "거래처명")].Text
                    = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "거래처")].Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");

                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "사업자등록번호")].Text
                    = SystemBase.Base.CodeName("CUST_CD", "RGST_NO", "B_CUST_INFO", fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "거래처")].Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");

                }
            }
        }
        #endregion

        #region MAB004_Activated
        private void MAB004_Activated(object sender, System.EventArgs e)
        {
            if (form_act_chk == false) txtProjNo.Focus();
        }

        private void MAB004_Deactivate(object sender, System.EventArgs e)
        {
            form_act_chk = true;
        }
        #endregion

        #region 버튼 Click
        private void btnProj_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW007 pu = new WNDW007(txtProjNo.Text);
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtProjNo.Text = Msgs[3].ToString();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }
        #endregion

    }
}
