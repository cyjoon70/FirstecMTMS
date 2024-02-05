#region 작성정보
/*********************************************************************/
// 단위업무명 : 표준자원능력일괄생성
// 작 성 자 : 권 순 철
// 작 성 일 : 2013-04-08
// 작성내용 : 표준자원능력일괄생성 및 관리
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
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace PA.PBA107
{
    public partial class PBA107 : UIForm.Buttons
    {
        public PBA107()
        {
            InitializeComponent();
        }

        #region Form Load시
        private void PBA107_Load(object sender, System.EventArgs e)
        {
            //필수 체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);


            // 공장 DEFAULT 세팅
            txtPlant_CD.Text = SystemBase.Base.gstrPLANT_CD;
            dtpSTARTDT.Text = SystemBase.Base.ServerTime("YYMMDD").Substring(0, 7);
            dtpENDDT.Text = SystemBase.Base.ServerTime("YYMMDD").Substring(0, 7);
        }
        #endregion

        #region 실행버튼
        private void btnExec_Click(object sender, System.EventArgs e)
        {
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                exec();
            }
        }
        #endregion

        #region 실행
        private void exec()
        {
            this.Cursor = Cursors.WaitCursor;

            string ERRCode = "", MSGCode = "P0000";

            SqlConnection dbConn = SystemBase.DbOpen.DBCON();
            SqlCommand cmd = dbConn.CreateCommand();
            SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

            DialogResult dsMsg = DialogResult.Yes;

            try
            {
                string startDt = "", endDt = "";
                startDt = dtpSTARTDT.Text + "-01";
                endDt = Convert.ToDateTime(dtpENDDT.Text + "-01").AddMonths(1).AddDays(-1).ToString().Substring(0, 10);

                string strSql = " SELECT TOP 1 CAPA_DT FROM P_RESOURCE_CAPA(NOLOCK) WHERE RES_FLAG = 'R' ";
                strSql += " AND PLANT_CD = '" + txtPlant_CD.Text.ToString() + "' ";
                strSql += " AND CAPA_DT BETWEEN '" + startDt + "' ";
                strSql += " AND '" + endDt + "' ";
                strSql += " AND SCH_ID = '" + txtSCH_ID.Text.ToString() + "' ";
                strSql += " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                DataTable dt = SystemBase.DbOpen.NoTranDataTable(strSql);

                if (dt.Rows.Count > 0)
                {
                    dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("P0015"), SystemBase.Base.MessageRtn("Z0004"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                }

                if (dt.Rows.Count == 0 || dsMsg == DialogResult.Yes)
                {
                    this.Cursor = Cursors.WaitCursor;

                    strSql = " usp_PBA107  @pTYPE = 'P1'";
                    strSql += ", @pPLANT_CD = '" + txtPlant_CD.Text + "' ";
                    strSql += ", @pSTART_DT = '" + startDt + "' ";
                    strSql += ", @pEND_DT = '" + endDt + "' ";
                    strSql += ", @pSCH_ID = '" + txtSCH_ID.Text + "' ";
                    strSql += ", @pIN_ID = '" + SystemBase.Base.gstrUserID + "' ";
                    strSql += ", @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

                    DataSet ds2 = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                    ERRCode = ds2.Tables[0].Rows[0][0].ToString();
                    MSGCode = ds2.Tables[0].Rows[0][1].ToString();

                    if (ERRCode == "ER")
                    {
                        Trans.Rollback();
                        goto Exit;		// ER 코드 Return시 점프
                    }
                }
                Trans.Commit();
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                Trans.Rollback();
                ERRCode = "ER";
                MSGCode = "P0001";					//에러가 발생하여 데이터 처리가 취소되었습니다.
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

            this.Cursor = Cursors.Default;
        }
        #endregion

        #region 조회 팝업
        //공장
        private void btnPlant_CD_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pTYPE = 'P011', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' "; // 쿼리
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };											  // 쿼리 인자값(조회조건)
                string[] strSearch = new string[] { txtPlant_CD.Text, "" };											  // 쿼리 인자값에 들어갈 데이타

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00005", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "공장 조회");

                pu.ShowDialog();	//공통 팝업 호출
                if (pu.DialogResult == DialogResult.OK)
                {
                    string MSG = pu.ReturnVal.Replace("|", "#");
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(MSG);

                    txtPlant_CD.Text = Msgs[0].ToString();
                    txtPlant_NM.Value = Msgs[1].ToString();
                    txtPlant_CD.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "공장 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //스케쥴id
        private void btnSCH_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pType='P082', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };
                string[] strSearch = new string[] { txtSCH_ID.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P05007", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "스케줄ID 조회");
                pu.Width = 500;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtSCH_ID.Text = Msgs[0].ToString();
                    txtSCH_NM.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "스케쥴ID 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region textBox 체인지 이벤트
        //공장
        private void txtPlant_CD_TextChanged(object sender, System.EventArgs e)
        {
            txtPlant_NM.Value = SystemBase.Base.CodeName("PLANT_CD", "PLANT_NM", "B_PLANT_INFO", txtPlant_CD.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "'");
        }
        //스케쥴id
        private void txtSCH_ID_TextChanged(object sender, System.EventArgs e)
        {
            txtSCH_NM.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtSCH_ID.Text, " AND MAJOR_CD = 'P008' AND COMP_CODE='" + SystemBase.Base.gstrCOMCD + "'");
        }
        #endregion 
    }
}
