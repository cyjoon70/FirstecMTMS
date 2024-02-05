#region 작성정보
/*********************************************************************/
// 단위업무명 : 가공품실무게관리
// 작 성 자 : 김현근
// 작 성 일 : 2013-06-05
// 작성내용 : 가공품실무게관리 및 조회
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


namespace IBB.IBFA03U
{
    public partial class IBFA03U : UIForm.FPCOMM1
    {
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private bool chk = false;
        public IBFA03U()
        {
            InitializeComponent();
        }

        #region Form Load 시
        private void IBFA03U_Load(object sender, System.EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(groupBox1);	//컨트롤 필수 Setting

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
             this.Cursor = Cursors.WaitCursor;

             try
             {
                 if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                 {
                     string strQuery = " usp_IBFA03U  'S1',";
                     strQuery += " @pITEM_CD = '" + txtItemCd.Text + "' ";	
                     strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                     UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0);
                 }
             }
             catch (Exception f)
             {
                 SystemBase.Loggers.Log(this.Name, f.ToString());
                 MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
             }

            this.Cursor = Cursors.Default;
        }
        #endregion

        #region SaveExec() 폼에 입력된 데이타 저장 로직
        protected override void SaveExec()
        {
            //Major 코드 필수항목 체크
            if (SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", true))// 그리드 필수항목 체크 
            {
                string ERRCode = "ER", MSGCode = "SY001";	//처리할 내용이 없습니다.
                string strItemCd = "";

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

                            strItemCd = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목")].Text;

                            string strQuery = " usp_IBFA03U '" + strGbn + "'";
                            strQuery = strQuery + ", @pSAME_ITEM_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "동일품목코드")].Text + "'";
                            strQuery = strQuery + ", @pITEM_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목")].Text + "'";
                            strQuery = strQuery + ", @pSAME_ITEM_SPEC = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "동일품규격")].Text + "'";
                            strQuery = strQuery + ", @pINSRT_USER_ID = '" + SystemBase.Base.gstrUserID + "'";
                            strQuery = strQuery + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                            DataSet ds = SystemBase.DbOpen.TranDataSet(strQuery, dbConn, Trans);
                            ERRCode = ds.Tables[0].Rows[0][0].ToString();
                            MSGCode = ds.Tables[0].Rows[0][1].ToString();

                            if (ERRCode == "ER") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
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
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);

                    SearchExec();

                    UIForm.FPMake.GridSetFocus(fpSpread1, strItemCd, SystemBase.Base.GridHeadIndex(GHIdx1, "품목"));
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

        #region 팝업창 열기
        private void btnItemCd_Click(object sender, EventArgs e)
        {
            try
            {
                WNDW.WNDW005 pu = new WNDW.WNDW005(SystemBase.Base.gstrPLANT_CD.ToString(), "10", txtItemCd.Text);
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtItemCd.Text = Msgs[2].ToString();
                    txtItemNm.Value = Msgs[3].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목코드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 품목코드 변환시       
        private void txtItemCd_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (txtItemCd.Text != "")
                {
                    txtItemNm.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtItemCd.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtItemNm.Value = "";
                }
            }
            catch { }
        }
        #endregion

        #region 그리드 상 팝업
        protected override void fpButtonClick(int Row, int Column)
        {
            if (fpSpread1.Sheets[0].RowHeader.Cells[Row, 0].Text == "I")
            {
                if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "품목_2"))
                {
                    try
                    {
                        WNDW.WNDW005 pu = new WNDW.WNDW005(SystemBase.Base.gstrPLANT_CD.ToString(), "10", txtItemCd.Text);
                        pu.ShowDialog();
                        if (pu.DialogResult == DialogResult.OK)
                        {
                            string[] Msgs = pu.ReturnVal;

                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목")].Text = Msgs[2].ToString();
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목명")].Text = Msgs[3].ToString();
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목계정")].Text = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", Msgs[5].ToString(), " AND LANG_CD='" + SystemBase.Base.gstrLangCd + "' AND COMP_CODE='" + SystemBase.Base.gstrCOMCD + "' AND MAJOR_CD='B036'");
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "규격")].Text = Msgs[7].ToString();

                            UIForm.FPMake.fpChange(fpSpread1, Row);//수정플래그
                        }
                    }
                    catch (Exception f)
                    {
                        SystemBase.Loggers.Log(this.Name, f.ToString());
                        MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목코드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        #endregion

        #region fpSpread1_Change
        protected override void fpSpread1_ChangeEvent(int Row, int Col)
        {
            try
            {
                if (Col == SystemBase.Base.GridHeadIndex(GHIdx1, "품목"))
                {

                    string strSql = "SELECT ITEM_NM,  ";
                    strSql = strSql + " dbo.ufn_GetCodeName_C('" + SystemBase.Base.gstrCOMCD + "', 'KOR','B036', ITEM_ACCT)  , ITEM_SPEC ";
                    strSql = strSql + " FROM B_ITEM_INFO(NOLOCK) WHERE ITEM_CD ='" + fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목")].Text + "'";
                    strSql = strSql + " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "'";

                    DataSet ds = SystemBase.DbOpen.NoTranDataSet(strSql);

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목명")].Text = ds.Tables[0].Rows[0][0].ToString();
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "규격")].Text = ds.Tables[0].Rows[0][1].ToString();
                        if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "동일품규격")].Text == "")
                        {
                            int temp = ds.Tables[0].Rows[0][1].ToString().IndexOf("(");
                            if (temp > 0) fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "동일품규격")].Text = ds.Tables[0].Rows[0][1].ToString().Substring(0, temp - 1);
                            else fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "동일품규격")].Text = ds.Tables[0].Rows[0][1].ToString();

                        }
                    }
                    else 
                    {
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목명")].Text = "";
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "규격")].Text = "";
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "동일품규격")].Text = "";
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(f.ToString());
            }
        }
        #endregion

        #region Form Activated & Deactivated
        private void IBFA03U_Activated(object sender, System.EventArgs e)
        {
            if (chk == false)
            {
                txtItemCd.Focus();
            }
        }

        private void IBFA03U_Deactivate(object sender, System.EventArgs e)
        {
            chk = true;
        }
        #endregion

    }
}
