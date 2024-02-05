
#region 작성정보
/*********************************************************************/
// 단위업무명 : MRP구매요청유형변경
// 작 성 자 : 권순철
// 작 성 일 : 2013-04-09
// 작성내용 : MRP구매요청유형변경 및 관리
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

namespace PB.PSA028
{
    public partial class PSA028 : UIForm.FPCOMM1
    {
        public PSA028()
        {
            InitializeComponent();
        }

        #region Form Load 시
        private void PSA028_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            //그리드 초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
        }
        #endregion

        #region SearchExec()
        protected override void SearchExec()
        {
            //조회조건 필수 체크
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1) == true)
            {
                this.Cursor = Cursors.WaitCursor;

                try
                {
                    string strQuery = "usp_PSA028 @pTYPE = 'S1'";
                    strQuery += ", @pMRP_NO = '" + txtMrpNo.Text + "'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0, true);
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회중 오류가 발생하였습니다.
                }

                this.Cursor = Cursors.Default;
            }
        }
        #endregion

        #region 저장
        protected override void SaveExec()
        {
           // string fcsStr = "";
            txtMrpNo.Focus();

            this.Cursor = Cursors.WaitCursor;

            string ERRCode = "ER", MSGCode = "P0000"; //처리할 내용이 없습니다.

            SqlConnection dbConn = SystemBase.DbOpen.DBCON();
            SqlCommand cmd = dbConn.CreateCommand();
            SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

            try
            {
                //행수만큼 처리
                for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                {

                    //fcsStr = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "MRP_NO")].Text;
                    string check = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "선택")].Text;

                    if (check == "True")
                    {
                        string strSql = " usp_PSA028 ";
                        strSql = strSql + " @pType = 'U1'";

                        strSql += ", @pMRP_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "MRP_NO")].Text + "' ";
                        strSql += ", @pREQ_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "구매요청번호")].Text + "' ";
                        strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID.ToString() + "' ";
                        strSql += ", @pCO_CD ='" + SystemBase.Base.gstrCOMCD + "' ";

                        DataSet df = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                        ERRCode = df.Tables[0].Rows[0][0].ToString();
                        MSGCode = df.Tables[0].Rows[0][1].ToString();

                        if (ERRCode != "OK")
                        {
                            Trans.Rollback();
                            goto Exit;
                        }
                    }
                    // ER 코드 Return시 점프
                }

                Trans.Commit();
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                Trans.Rollback();
                MSGCode = "P0001";	//에러가 발생되어 데이터 처리가 취소되었습니다.
            }
        Exit:
            dbConn.Close();

            if (ERRCode == "OK")
            {
                MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                SearchExec();
                //UIForm.FPMake.GridSetFocus(fpSpread1, fcsStr); //저장 후 그리드 포커스 이동
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
        #endregion

        #region 조회조건팝업
        private void btnMrpNo_Click(object sender, System.EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                string strQuery = "usp_P_COMMON 'P200' , @pCOM_NM = 'S', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                string[] strWhere = new string[] { "@pCOM_CD", "@pETC" };
                string[] strSearch = new string[] { txtMrpNo.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00064", strQuery, strWhere, strSearch, new int[] { 0, 3 }, "MRP No 조회");
                pu.Width = 800;
                pu.ShowDialog();	//공통 팝업 호출

                if (pu.DialogResult == DialogResult.OK)
                {
                    string MSG = pu.ReturnVal.Replace("|", "#");
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(MSG);

                    txtMrpNo.Text = Msgs[0].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "MRP No 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            this.Cursor = Cursors.Default;
        }
        #endregion
    }
}
