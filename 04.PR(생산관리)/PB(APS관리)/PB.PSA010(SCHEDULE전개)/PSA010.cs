#region 작성정보
/*********************************************************************/
// 단위업무명 : SCHEDULE 전개
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-02-28
// 작성내용 : SCHEDULE 전개 및 관리
// 수 정 일 :
// 수 정 자 :
// 수정내용 :
// 비    고 :
/*********************************************************************/
#endregion

using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Globalization;
using System.Text.RegularExpressions;
using WNDW;

namespace PB.PSA010
{
    public partial class PSA010 : UIForm.FPCOMM1
    {
        public static int WORK_TYPE_BOM_DEPLOY = 1; // 업무 분류 (봄 전개)
        public static int WORK_TYPE_SCH_DEPLOY = 2; // 업무 분류 (스케쥴 전개)

        // 프로세스 ID작성
        public static string PROC_ID;
        public static string PROC_TYPE = "S";

        // SCH_NO
        public static string SCH_NO;
        
        public PSA010()
        {
            // 프로세스 아이디 설정
            PROC_ID = SCH_PROG.GenProcId();

            InitializeComponent();
        }

        #region Form Load 시
        private void PSA010_Load(object sender, System.EventArgs e)
        {
            try
            {
                SystemBase.Validation.GroupBox_Setting(groupBox1);
                SystemBase.Validation.GroupBox_Setting(groupBox5);

                dtpSCHST_DT.Value = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0, 10);
                dtpPTF.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(1).ToString().Substring(0, 10);
                dtpSCHST_TM.Value = "08:00";

                string Query = " usp_P_COMMON 'P010', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "',@pPLANT_CD='" + SystemBase.Base.gstrPLANT_CD + "'";
                DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);
                if (dt.Rows.Count > 0)
                {
                    txtPlant_CD.Text = dt.Rows[0][0].ToString();
                    txtPlant_NM.Value = dt.Rows[0][1].ToString();
                }

                G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "방향")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE = 'COMM', @pCODE = 'P065', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");
                // 그리드 초기화
                UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0066"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);                
            }
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
                    string strMQuery = " usp_PSA010 'S1',@pPLANT_CD='" + txtPlant_CD.Text + "' ";
                    strMQuery += ", @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strMQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "방향")].Text = "역전개";
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "전공정미감안")].Value = false;

                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "재고감안")].Text == "True")
                        {
                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "가용재고감안")].Locked = false;
                        }
                        else
                        {
                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "가용재고감안")].Value = false;
                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "가용재고감안")].Locked = true;
                        }
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }

            this.Cursor = Cursors.Default;
        }
        #endregion

        #region NewExec()
        protected override void NewExec()
        {
            if (fpSpread1.Sheets[0].Rows.Count > 0)
            {
                fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "가용재고감안")].Text = "";
                fpSpread1.Sheets[0].Cells[1, SystemBase.Base.GridHeadIndex(GHIdx1, "가용재고감안")].Text = "";
                fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "전공정미감안")].Text = "";
                fpSpread1.Sheets[0].Cells[1, SystemBase.Base.GridHeadIndex(GHIdx1, "전공정미감안")].Text = "";
                fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "Schedule기준일시")].Text = "";
                fpSpread1.Sheets[0].Cells[1, SystemBase.Base.GridHeadIndex(GHIdx1, "Schedule기준일시")].Text = "";
                fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "일정전개개시일시")].Text = "";
                fpSpread1.Sheets[0].Cells[1, SystemBase.Base.GridHeadIndex(GHIdx1, "일정전개개시일시")].Text = "";
                fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "일정전개완료일시")].Text = "";
                fpSpread1.Sheets[0].Cells[1, SystemBase.Base.GridHeadIndex(GHIdx1, "일정전개완료일시")].Text = "";
                fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "생산오더Header수")].Text = "";
                fpSpread1.Sheets[0].Cells[1, SystemBase.Base.GridHeadIndex(GHIdx1, "생산오더Header수")].Text = "";
            }
        }
        #endregion

        #region SaveExec 저장
        protected override void SaveExec()
        {
            string fcsStr = "";
            this.Cursor = Cursors.WaitCursor;

            string ERRCode = "OK", MSGCode = "P0010";
            SqlConnection dbConn = SystemBase.DbOpen.DBCON();
            SqlCommand cmd = dbConn.CreateCommand();
            SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

            try
            {
                //행수만큼 처리
                for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                {
                    fcsStr = fpSpread1.Sheets[0].Cells[i,0].Text;
                    string strGbn = "U1";

                    string SCH_ID = fpSpread1.Sheets[0].Cells[i, 0].Text;      // SCH_ID설정
                    string ACTIVE = "0";  // ACTIVE설정 
                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Active")].Text == "True")
                        ACTIVE = "1";
                    string DEPLOY = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "방향")].Text;     // 전개방식
                    string INFINITY = "0"; // 무한능력 설정
                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "무한능력")].Text == "True")
                        INFINITY = "1";
                    string RESOURCE = "0"; // 재고감안
                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "재고감안")].Text == "True")
                        RESOURCE = "1";
                    string CAPA = "0"; // 가용재고감안
                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "가용재고감안")].Text == "True")
                        CAPA = "1";
                    string BF_PROCESS = "0";
                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "전공정미감안")].Text == "True")
                        BF_PROCESS = "1";

                    DateTime tmpDT;
                    string SCHST_DT = "";
                    string SCHST_TM = "";
                    string UNFOLD_ST_DT = "";
                    string UNFOLD_ST_TM = "";
                    string UNFOLD_ED_DT = "";
                    string UNFOLD_ED_TM = "";

                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Schedule기준일시")].Text != "")
                    {
                        tmpDT = Convert.ToDateTime(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Schedule기준일시")].Text);
                        SCHST_DT = tmpDT.ToString("yyyy-MM-dd");
                        SCHST_TM = tmpDT.ToString("HHmm");
                    }

                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "일정전개개시일시")].Text != "")
                    {
                        tmpDT = Convert.ToDateTime(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "일정전개개시일시")].Text);
                        UNFOLD_ST_DT = tmpDT.ToString("yyyy-MM-dd");
                        UNFOLD_ST_TM = tmpDT.ToString("HHmm");
                    }

                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "일정전개완료일시")].Text != "")
                    {
                        tmpDT = Convert.ToDateTime(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "일정전개완료일시")].Text);
                        UNFOLD_ED_DT = tmpDT.ToString("yyyy-MM-dd");
                        UNFOLD_ED_TM = tmpDT.ToString("HHmm");
                    }
                    string ORDER_CNT = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "생산오더Header수")].Text;
                    string WORK_CNT = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "작업지시수")].Text;
                    string MATERIAL_CNT = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "구매요청수")].Text;

                    string strSql = " usp_PSA010 @pTYPE = '" + strGbn + "'";
                    strSql += ", @pSCH_ID = '" + SCH_ID + "'";
                    strSql += ", @pACTIVE = " + ACTIVE;
                    strSql += ", @pDEPLOY = '" + DEPLOY + "'";
                    strSql += ", @pINFINITY = " + INFINITY; // 무한능력
                    strSql += ", @pRESOURCE = " + RESOURCE; // 재고감안
                    strSql += ", @pCAPA     = " + CAPA;    // 가용재고감안
                    strSql += ", @pBF_PROCESS = " + BF_PROCESS; // 전공정미감안
                    strSql += ", @pSCHST_DT   = '" + SCHST_DT + "'";   // Schedule기준일시
                    strSql += ", @pSCHST_TM   = '" + SCHST_TM + "'";   // Schedule기준일시
                    strSql += ", @pUNFOLD_ST_DT = '" + UNFOLD_ST_DT + "'"; // 일정전개개시일시
                    strSql += ", @pUNFOLD_ST_TM = '" + UNFOLD_ST_TM + "'"; // 일정전개개시일시
                    strSql += ", @pUNFOLD_ED_DT = '" + UNFOLD_ED_DT + "'"; // 일정전개완료일시
                    strSql += ", @pUNFOLD_ED_TM = '" + UNFOLD_ED_TM + "'"; // 일정전개완료일시
                    strSql += ", @pORDER_CNT = '" + ORDER_CNT + "'";    // 생산오더 Header수
                    strSql += ", @pWORK_CNT  = '" + WORK_CNT + "'";           // 작업지시수
                    strSql += ", @pMATERIAL_CNT = '" + MATERIAL_CNT + "'";        // 구매요청수
                    strSql += ", @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

                    SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);

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
            dbConn.Close();

            if (ERRCode == "OK")
            {
                MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                SearchExec();                
                UIForm.FPMake.GridSetFocus(fpSpread1, fcsStr); //저장 후 그리드 포커스 이동
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

        #region btnPlant_CD_Click
        private void btnPlant_CD_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pType='P011' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };
                string[] strSearch = new string[] { txtPlant_CD.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("BBB003", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "공장 조회", true);
                pu.Width = 500;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtPlant_CD.Text = Msgs[0].ToString();
                    txtPlant_NM.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn(f.Message), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        #endregion

        #region btnSchedule_ST_Click
        public void ScheduleST()
        {
            try
            {
                if (fpSpread1.Sheets[0].Rows.Count > 0)
                {
                    if (dtpSCHST_TM.Text == "")
                    {
                        MessageBox.Show(SystemBase.Base.MessageRtn("P0006"));
                    }
                    else
                    {
                        PSA010P1 frm = new PSA010P1(fpSpread1,
                                                    txtPlant_CD.Text,
                                                    dtpSCHST_DT.Text,
                                                    dtpSCHST_TM.Text.Replace(":", ""),
                                                    dtpPTF.Value.ToString(),
                                                    txtProjectNo.Text,
                                                    txtProjectSeq.Text,
                                                    WORK_TYPE_SCH_DEPLOY,
                                                    txtReasonCd.Text,
                                                    txtMemo.Text
                                                    );
                        frm.Show();

                        if (frm.DialogResult == DialogResult.OK)
                        {
                            string strMQuery = " usp_PSA010 'S1',@pPLANT_CD='" + txtPlant_CD.Text + "' ";
                            strMQuery += ", @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

                            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
                            fpSpread1.Sheets[0].ColumnHeader.Rows[0].Height = 35;
                        }
                    }
                }
                else
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn("P0012"));
                }

                for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                {
                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "방향")].Text == "역전개")
                    {
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "무한능력")].Locked = false;

                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "전공정미감안")].Value = false;
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "전공정미감안")].Locked = true;
                    }
                    else
                    {
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "무한능력")].Value = false;
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "무한능력")].Locked = true;

                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "전공정미감안")].Locked = false;
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn(f.Message), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);            
            }
        }
        #endregion

        #region BOM전개
        public void BomDeply()
        {
            try
            {
                PSA010P1 frm = new PSA010P1(fpSpread1,
                                            txtPlant_CD.Text,
                                            dtpSCHST_DT.Text,
                                            dtpSCHST_TM.Text.Replace(":", ""),
                                            dtpPTF.Value.ToString(),
                                            txtProjectNo.Text,
                                            txtProjectSeq.Text,
                                            WORK_TYPE_BOM_DEPLOY, "", "");
                frm.Show();

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn(f.Message), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);            
            }
        }
        #endregion

        #region fpSpread1_ComboCloseUp
        private void fpSpread1_ComboCloseUp(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            if (fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "방향")].Text == "역전개")
            {
                fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "무한능력")].Locked = false;

                fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "전공정미감안")].Value = false;
                fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "전공정미감안")].Locked = true;
            }
            else
            {
                fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "무한능력")].Value = false;
                fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "무한능력")].Locked = true;

                fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "전공정미감안")].Locked = false;
            }
        }
        #endregion

        #region fpSpread1_ButtonClicked
        private void fpSpread1_ButtonClicked(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            if (e.Column == 5)
            {
                if (fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "재고감안")].Text == "True")
                {
                    fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "가용재고감안")].Locked = false;
                }
                else
                {
                    fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "가용재고감안")].Value = false;
                    fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "가용재고감안")].Locked = true;
                }
            }
        }
        #endregion

        #region btnConfirm_Click
        private void btnConfirm_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (fpSpread1.Sheets[0].Rows.Count > 0)
                {
                    int ActiveRow1 = 0;
                    string strSch_Id = "";
                    if (fpSpread1.ActiveSheet.GetSelection(0) != null)	// 그리드가 Row가 선택된 경우 Row 위치를 ActiveRow1에 저장
                    {
                        ActiveRow1 = fpSpread1.ActiveSheet.GetSelection(0).Row;
                        strSch_Id = fpSpread1.Sheets[0].Cells[ActiveRow1, SystemBase.Base.GridHeadIndex(GHIdx1, "Schedule ID")].Text;

                        PSA010P2 frm = new PSA010P2(strSch_Id, txtReasonCd.Text, txtMemo.Text);
                        frm.ShowDialog();
                    }
                    else
                    {
                        MessageBox.Show(SystemBase.Base.MessageRtn("P0012", "확정"), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);//선택된 SCHEDULE이 없습니다.
                    }

                }
                else
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn("P0012", "확정"), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);//선택된 SCHEDULE이 없습니다.
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString());
            }
        }
        #endregion

        #region btnProject_Click
        private void btnProject_Click(object sender, System.EventArgs e)
        {
            try
            {               
                WNDW003 pu = new WNDW003();
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtProjectNo.Text = Msgs[3].ToString();
                    txtProjectNm.Value = Msgs[4].ToString();
                    txtProjectSeq.Text = Msgs[5].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn(f.Message), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region txtProjectNo_KeyDown
        private void txtProjectNo_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                txtProjectNo.Text = "";
                txtProjectNm.Value = "";
                txtProjectSeq.Text = "";
            }
            else if (e.KeyCode == Keys.Back)
            {
                txtProjectNm.Value = "";
                txtProjectSeq.Text = "";
            }
        }
        #endregion

        #region btnRsn_Click
        private void btnRsn_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pTYPE = 'P042', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pETC = 'P022' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };
                string[] strSearch = new string[] { "", "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00063", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "스케쥴 전개사유", true);
                pu.Width = 500;
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtReasonCd.Text = Msgs[0].ToString();
                    txtReasonNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log("스케쥴 전개사유 : ", f);
                MessageBox.Show(f.Message);
            }
        }
        #endregion

        #region btnProcessChk_Click
        private void btnProcessChk_Click(object sender, System.EventArgs e)
        {
            string strMQuery = " usp_PSA010 @pTYPE = 'C1'";
            strMQuery += ", @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

            DataTable dt = SystemBase.DbOpen.NoTranDataTable(strMQuery);

            txtProcessChk.Value = dt.Rows[0][0].ToString();
        }
        #endregion

        #region btnProcessShow_Click
        private void btnProcessShow_Click(object sender, System.EventArgs e)
        {
            try
            {
                PSA010P3 pu = new PSA010P3();
                pu.Show();
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log("PSA010 스케줄 전개 : ", f);
                MessageBox.Show(f.Message);
            }

        }
        #endregion

        #region 봄전개 버튼 클릭
        private void btnBOM_DEPLOY_Click(object sender, System.EventArgs e)
        {
            if (fpSpread1.Sheets[0].Rows.Count > 0)
            {
                string Msg = "P0022";

                if (MessageBox.Show(SystemBase.Base.MessageRtn(Msg), "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    BomDeply();
                }
            }
            else
            {
                MessageBox.Show(SystemBase.Base.MessageRtn("P0012", "전개"), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        #endregion

        #region 오더 통합버튼 클릭
        private void btnUNITY_ORDER_Click(object sender, System.EventArgs e)
        {
            if (fpSpread1.Sheets[0].Rows.Count > 0)
            {
                int ActiveRow1 = 0;
                string strSch_Id = "";

                if (fpSpread1.ActiveSheet.GetSelection(0) != null)	// 그리드가 Row가 선택된 경우 Row 위치를 ActiveRow1에 저장
                {
                    ActiveRow1 = fpSpread1.ActiveSheet.GetSelection(0).Row;
                    strSch_Id = fpSpread1.Sheets[0].Cells[ActiveRow1, 0].Text;

                    PSA010P5 frm = new PSA010P5(strSch_Id);
                    frm.ShowDialog();

                }
                else
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn("P0012", "통합"), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

            }
            else
            {
                MessageBox.Show(SystemBase.Base.MessageRtn("P0012", "통합"), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        #endregion

        #region 스케쥴 전개 버튼 클릭
        private void btnSchedule_ST_Click(object sender, System.EventArgs e)
        {
            if (fpSpread1.Sheets[0].Rows.Count > 0)
                ScheduleST();
            else
                MessageBox.Show(SystemBase.Base.MessageRtn("P0012", "전개"), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information); 
        }
        #endregion

        #region 공정시수 체크 및 확인
        private void btnProcTimeChk_Click(object sender, System.EventArgs e)
        {
            string strMQuery = " usp_PSA010 @pTYPE = 'C3'";
            strMQuery += ", @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

            DataTable dt = SystemBase.DbOpen.NoTranDataTable(strMQuery);

            txtProcTimeCnt.Value = dt.Rows[0][0].ToString();
        }

        private void btnProcTimeShow_Click(object sender, System.EventArgs e)
        {
            try
            {
                PSA010P6 pu = new PSA010P6();
                pu.Show();
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log("PSA010 스케줄 전개 : ", f);
                MessageBox.Show(f.Message);
            }
        }
        #endregion

       
    }
}
