#region 작성정보
/*********************************************************************/
// 단위업무명 : 제조오더배포등록
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-03-18
// 작성내용 : 제조오더배포등록 및 관리
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

namespace PC.PUA101
{
    public partial class PUA101 : UIForm.FPCOMM1
    {
        #region 변수선언
        int NewFlg = 0;

        string PROJECT_NO = "";
        string PROJECT_SEQ = "";
        string GROUP_CD = "";

        public static string PROC_ID = "";
        public static string PROC_TYPE = "E"; // 긴급작지
        #endregion

        #region 생성자
        public PUA101()
        {
            InitializeComponent();
        }
        #endregion

        #region SearchExec() 그리드 조회
        protected override void SearchExec()
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
            try
            {           
                string strSql = " usp_PUA101 @pTYPE = 'S1' ";
                strSql += " ,@pPROJECT_NO='" + txtProj_No.Text + "' ";
                strSql += " ,@pPROJECT_SEQ_FR ='" + txtProj_Seq_Fr.Text + "' ";
                strSql += " ,@pPROJECT_SEQ_TO ='" + txtProj_Seq_To.Text + "' ";
                strSql += " ,@pITEM_CD ='" + txtItemCd.Text + "' ";
                strSql += " ,@pSTATUS='" + cboSTATUS.SelectedValue.ToString() + "' ";
                strSql += " ,@pDELIVERY_FR_DT='" + dtpDLV_FR_DT.Text + "' ";
                strSql += " ,@pDELIVERY_TO_DT='" + dtpDLV_TO_DT.Text + "' ";
                strSql += " ,@pMAKEORDER_NO_FR ='" + txtMakeOrderFr.Text + "' ";
                strSql += " ,@pMAKEORDER_NO_TO ='" + txtMakeOrderTo.Text + "' ";
                strSql += " ,@pWORKORDER_NO_FR ='" + txtWorkOrderFr.Text + "' ";
                strSql += " ,@pWORKORDER_NO_TO ='" + txtWorkOrderTo.Text + "' ";
                strSql += " ,@pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

                UIForm.FPMake.grdCommSheet(fpSpread1, strSql, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false, false);

                // 확정 데이터는 모두 LOCK시킨다.
                for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                {
                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "처리상태")].Value.ToString() == "C")
                    {
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Locked = true;
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수")].Locked = true;
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호_2")].Locked = true;

                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "그룹코드")].Locked = true;
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Locked = true;

                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "생산수량")].Locked = true;
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "단계")].Locked = true;
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "생산완료일자")].Locked = true;

                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "재고감안여부")].Locked = true;
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "재작업여부")].Locked = true; ;

                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "작업지시구분")].Locked = true;
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "제조오더번호")].Locked = true;
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "제품오더번호")].Locked = true;

                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "대상")].Locked = true;
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "처리상태")].Locked = true;
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "비고")].Locked = true;

                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호_2")].Locked = true;
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "그룹코드_2")].Locked = true;
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드_2")].Locked = true;

                    }
                }

                GridReMake();
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            this.Cursor = System.Windows.Forms.Cursors.Default;
        }
        #endregion

        #region Form Load시
        private void PUA101_Load(object sender, System.EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(gbxITEM_MASTER);

            // 그리드 설정
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "단계")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'P040', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "재고감안여부")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'B029', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "재작업여부")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'P027',  @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "작업지시구분")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'P038', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "처리상태")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'P039', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "단위")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'Z005',  @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");

            // 콤보 설정
            SystemBase.ComboMake.C1Combo(cboSTATUS, "usp_P_COMMON @pType='P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'P039', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);	// 처리상태
            cboSTATUS.SelectedValue = "P";

            SystemBase.ComboMake.C1Combo(cboSCH_ID, "usp_P_COMMON @pType='P043', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'P008', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);	// 처리상태
            cboSCH_ID.SelectedValue = "PB0614";
            cboSCH_ID.Enabled = false;

            dtpDLV_TO_DT.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(1).ToShortDateString().Substring(0,10);
            dtpDLV_FR_DT.Value = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0, 10);
            // 프로세스 ID 설정
            PROC_ID = SCH_PROG.GenProcId();

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false, true);	
        }
        #endregion

        #region 그리드 상단 팝업
        private void fpSpread1_ButtonClicked(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            try
            {
                // 프로젝트 조회일경우
                if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호_2"))
                {
                    WNDW003 pu = new WNDW003();
                    pu.ShowDialog();

                    if (pu.DialogResult == DialogResult.OK)
                    {
                        string[] Msgs = pu.ReturnVal;

                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text = Msgs[3].ToString();
                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수")].Text = Msgs[5].ToString();
                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "그룹코드")].Text = Msgs[6].ToString();
                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text = Msgs[6].ToString();
                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목명")].Text = Msgs[7].ToString();
                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].Value
                            = SystemBase.Base.CodeName("ITEM_CD", "ITEM_UNIT", "B_ITEM_INFO", Msgs[6].ToString(), " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");

                    }
                }
                else if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드_2")) // 품목코드
                {
                    WNDW005 pu = new WNDW005();
                    pu.ShowDialog();

                    // 프로젝트 차수 클릭
                    if (pu.DialogResult == DialogResult.OK)
                    {
                        string[] Msgs = pu.ReturnVal;
                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text = Msgs[2].ToString();
                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목명")].Text = Msgs[3].ToString();
                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].Value = Msgs[8].ToString();
                    }
                }
                else if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "ROUT NO_2")) // ROUT정보
                {
                    if (fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text != "")
                    {
                        if (fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "단계")].Value.ToString() == "1")
                        {
                            string strQuery = "usp_PUA101 'P1', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                            string[] strWhere = new string[] { "@pITEM_CD" };
                            string[] strSearch = new string[] { fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text };

                            UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00062", strQuery, strWhere, strSearch, new int[] { });
                            pu.Width = 400;
                            pu.ShowDialog();

                            if (pu.DialogResult == DialogResult.OK)
                            {
                                Regex rx1 = new Regex("#");
                                string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                                fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "ROUT NO")].Value = Msgs[0].ToString(); //공정명
                                UIForm.FPMake.fpChange(fpSpread1, e.Row);
                            }
                        }
                        else
                        {
                            MessageBox.Show("다단계는 라우팅을 선택할수 없습니다.", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    else
                    {
                        MessageBox.Show("품목코드를 입력하셔야 합니다.", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }

                else if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "생산담당자_2"))
                {
                    string strQuery = " usp_P_COMMON @pType='P180' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                    string[] strWhere = new string[] { };
                    string[] strSearch = new string[] { };
                    UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00095", strQuery, strWhere, strSearch, new int[] { 2, 3 }, "");
                    pu.ShowDialog();
                    if (pu.DialogResult == DialogResult.OK)
                    {
                        Regex rx1 = new Regex("#");
                        string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "생산담당자")].Text = Msgs[0].ToString();
                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "담당자명")].Text = Msgs[1].ToString();
                    }
                }
                else if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "원생산오더_2"))
                {
                    try
                    {
                        WNDW006 pu = new WNDW006(txtWorkOrderFr.Text);
                        pu.ShowDialog();
                        if (pu.DialogResult == DialogResult.OK)
                        {
                            string[] Msgs = pu.ReturnVal;

                            fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "원생산오더")].Text = Msgs[1].ToString();
                        }
                    }
                    catch (Exception f)
                    {
                        SystemBase.Loggers.Log(this.Name, f.ToString());
                        MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "제조오더번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region fpSpread1_Change
        protected override void fpSpread1_ChangeEvent(int Row, int Column)
        {
            string strProjectNo = "", strProjectSeq = "", strGroupCd = "", strItemCd = "";

            strItemCd = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text;
            strProjectNo = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text;
            strProjectSeq = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수")].Text;
            strGroupCd = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "그룹코드")].Text;

            if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드"))
            {
                try
                {
                    string strSql = " usp_PUA101 'M2'";
                    strSql += ", @pITEM_CD= '" + strItemCd + "'";
                    strSql += " ,@pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

                    DataTable dt = SystemBase.DbOpen.NoTranDataTable(strSql);
                    string ERRCode = dt.Rows[0][0].ToString();
                    string MSGCode = dt.Rows[0][1].ToString();

                    if (ERRCode == "ER")
                    {
                        MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text = "";
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목명")].Text = "";
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].Text = "";
                    }
                    else
                    {
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목명")].Text
                            = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].Value
                            = SystemBase.Base.CodeName("ITEM_CD", "ITEM_UNIT", "B_ITEM_INFO", fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
                    }
                }
                catch
                {
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목명")].Text = "";
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].Text = "";
                }
            }

            if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호") || Column == SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수") || Column == SystemBase.Base.GridHeadIndex(GHIdx1, "그룹코드"))
            {
                if (strProjectNo != "" && strProjectSeq != "" && strGroupCd != "")
                {
                    try
                    {
                        string strSql = " usp_PUA101 'M1'";
                        strSql += ", @pPROJECT_NO= '" + strProjectNo + "'";
                        strSql += ", @pPROJECT_SEQ= '" + strProjectSeq + "'";
                        strSql += ", @pGROUP_CD= '" + strGroupCd + "'";
                        strSql += " ,@pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

                        DataTable dt = SystemBase.DbOpen.NoTranDataTable(strSql);
                        string ERRCode = dt.Rows[0][0].ToString();
                        string MSGCode = dt.Rows[0][1].ToString();

                        if (ERRCode == "ER")
                        {
                            MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "그룹코드")].Text = "";
                        }
                    }
                    catch
                    {
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "그룹코드")].Text = "";
                    }
                }
            }

            if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "생산완료일자"))
            {
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "수주납기일")].Text
                    = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "생산완료일자")].Text;
            }
            if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "작업지시구분"))
            {
                if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "작업지시구분")].Text == "불량재작업")
                {
                   // fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "원생산오더")].
                }
                
            }
 

        }
        #endregion

        #region MASTER 삭제
        protected override void DeleteExec()
        {// 행 추가
            try
            {
                if (MessageBox.Show(SystemBase.Base.MessageRtn("P0003"), "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    string strSql = " usp_PUA101 'D2' ";

                    strSql += ", @pPROJECT_NO = '" + PROJECT_NO.ToString() + "'";
                    strSql += ", @pPROJECT_SEQ = '" + PROJECT_SEQ.ToString() + "'";
                    strSql += ", @pGROUP_CD = '" + GROUP_CD.ToString() + "'";
                    strSql += " ,@pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

                    DataTable dt = SystemBase.DbOpen.NoTranDataTable(strSql);
                    MessageBox.Show(dt.Rows[0][1].ToString());
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("P0001"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 행추가
        protected override void RowInsExec()
        {// 행 추가
            try
            {
                UIForm.FPMake.RowInsert(fpSpread1);
                int RowNum = fpSpread1.ActiveSheet.ActiveRowIndex;

                fpSpread1.Sheets[0].Cells[RowNum, SystemBase.Base.GridHeadIndex(GHIdx1, "단계")].Value = "1";      // 단단계
                fpSpread1.Sheets[0].Cells[RowNum, SystemBase.Base.GridHeadIndex(GHIdx1, "재고감안여부")].Value = "Y"; // 재고감안
                fpSpread1.Sheets[0].Cells[RowNum, SystemBase.Base.GridHeadIndex(GHIdx1, "재작업여부")].Value = "N"; // 재작업여부
                fpSpread1.Sheets[0].Cells[RowNum, SystemBase.Base.GridHeadIndex(GHIdx1, "처리상태")].Value = "P";      // 처리상태
             }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("P0001"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region SaveExec() 폼에 입력된 데이타 저장 로직
        protected override void SaveExec()
        {
            string fcsStr = "";
            if (fpSpread1.Sheets[0].Rows.Count > 0)
            {
                if (SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", true))// 그리드 상단 필수항목 체크
                {
                    string ERRCode = "ER";
                    string MSGCode = "P0000";
                    SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                    SqlCommand cmd = dbConn.CreateCommand();
                    SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                    try
                    {
                        for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                        {

                            string strGbn = "";
                            string strHead = fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text;

                            if (strHead.Length > 0)
                            {
                                switch (strHead)
                                {
                                    case "D": strGbn = "D1"; break;
                                    case "U": strGbn = "U1"; break;
                                    case "I": strGbn = "I1"; break;
                                    default: strGbn = ""; break;
                                }

                                fcsStr = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text;

                                string strSql = " usp_PUA101 '" + strGbn + "'";
                                strSql += ", @pPROJECT_NO= '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text + "'";
                                strSql += ", @pPROJECT_SEQ= '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수")].Text + "'";
                                strSql += ", @pGROUP_CD= '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "그룹코드")].Text + "'";
                                strSql += ", @pITEM_CD='" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text + "'";
                                strSql += ", @pITEM_QTY ='" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "생산수량")].Value + "'";
                                strSql += ", @pLEVEL ='" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "단계")].Value.ToString() + "'";
                                strSql += ", @pROUT_NO ='" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "ROUT NO")].Text + "'";
                                strSql += ", @pDELIVERY_DT ='" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "생산완료일자")].Text + "'";
                                strSql += ", @pSO_DELIVERY_DT ='" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수주납기일")].Text + "'";
                                strSql += ", @pREWORK_FLG ='" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "재작업여부")].Value.ToString() + "'";
                                strSql += ", @pWORKORDER_TYPE='" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "작업지시구분")].Value.ToString() + "'";
                                strSql += ", @pWORKORDER_NO='" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "제조오더번호")].Text + "'";
                                strSql += ", @pMAKEORDER_NO='" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "제품오더번호")].Text + "'";
                                strSql += ", @pORG_WORKORDER_NO='" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "원생산오더")].Text + "'";
                                strSql += ", @pCONF_OBJ_FLG='" + (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "대상")].Text == "True" ? "1" : "0") + "'";
                                strSql += ", @pSTATUS='" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "처리상태")].Value.ToString() + "'";
                                strSql += ", @pMF_PLAN_USER='" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "생산담당자")].Text + "'";
                                strSql += ", @pREMARK='" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "비고")].Text + "'";

                                strSql += ", @pSTOCK_CONSD_FLG ='0'";
                                strSql += ", @pLANG_CD='" + SystemBase.Base.gstrLangCd + "'";
                                strSql += ", @pUSR_ID= '" + SystemBase.Base.gstrUserID + "'";
                                strSql += " ,@pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

                                DataTable dt = SystemBase.DbOpen.TranDataTable(strSql, dbConn, Trans);
                                ERRCode = dt.Rows[0][0].ToString();
                                MSGCode = dt.Rows[0][1].ToString();

                                if (ERRCode == "ER") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
                            }
                        }
                        // 결과 처리
                        Trans.Commit();

                        SearchExec();
                        NewFlg = 0;

                    }
                    catch
                    {
                        Trans.Rollback();
                        ERRCode = "ER";
                        MSGCode = "P0001";
                    }
                Exit:
                    dbConn.Close();
                    if (ERRCode == "OK")
                    {
                        MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                        UIForm.FPMake.GridSetFocus(fpSpread1, fcsStr); //저장 후 그리드 포커스 이동
                    }
                    else
                    {
                        MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

            }
            else
            {
                MessageBox.Show(SystemBase.Base.MessageRtn("P0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region NewExec() 그리드 및 그룹박스 초기화
        protected override void NewExec()
        {
            try
            {
                SystemBase.Validation.GroupBox_Reset(gbxITEM_MASTER);

                // 콤보 설정
                SystemBase.ComboMake.C1Combo(cboSTATUS, "usp_P_COMMON @pType='P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'P039', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);	// 처리상태
                cboSTATUS.SelectedValue = "P";

                SystemBase.ComboMake.C1Combo(cboSCH_ID, "usp_P_COMMON @pType='P043', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'P008', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);	// 처리상태
                cboSCH_ID.SelectedValue = "PB0614";
                cboSCH_ID.Enabled = false;

                dtpDLV_TO_DT.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(1).ToShortDateString().Substring(0, 10);
                dtpDLV_FR_DT.Value = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0, 10);
                // 프로세스 ID 설정
                PROC_ID = SCH_PROG.GenProcId();

                UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false, false);

                NewFlg = 1;
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("P0001"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region btnPROJECT_Click
        private void btnPROJECT_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW003 pu = new WNDW003();
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtProj_No.Text = Msgs[3].ToString();
                    txtProj_Nm.Value = Msgs[4].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("P0001"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 확정
        private void btnCONF_Click(object sender, System.EventArgs e)
        {
            if (fpSpread1.Sheets[0].Rows.Count <= 0) // 확정할 데이터가 없을 경우
            {
                MessageBox.Show(SystemBase.Base.MessageRtn("P0035"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (MessageBox.Show(SystemBase.Base.MessageRtn("P0021"), SystemBase.Base.MessageRtn("Z0004"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {

                string ERRCode = "ER";
                string MSGCode = "P0000";
                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                try
                {
                    // 긴급작지 대상을 초기화 시킨다.
                    bool hasConfData = false; // 확정데이터 존재여부
                    string strSql = " usp_PUA101 'U2'";
                    strSql += " ,@pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

                    DataTable dt = SystemBase.DbOpen.TranDataTable(strSql, dbConn, Trans);
                    ERRCode = dt.Rows[0][0].ToString();
                    MSGCode = dt.Rows[0][1].ToString();

                    if (ERRCode == "ER") { throw new Exception(MSGCode); }	// ER 코드 Return시 점프

                    // 현재 설정값을 저장한다.
                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {
                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "대상")].Text == "True")
                        {
                            strSql = " usp_PUA101 'U3'";
                            strSql += ", @pWORKORDER_NO= '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "제조오더번호")].Text + "'";
                            strSql += ", @pCONF_OBJ_FLG='" + (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "대상")].Text == "True" ? "1" : "0") + "'";
                            strSql += ", @pITEM_CD='" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text + "'";
                            strSql += " ,@pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

                            dt = SystemBase.DbOpen.TranDataTable(strSql, dbConn, Trans);
                            ERRCode = dt.Rows[0][0].ToString();
                            MSGCode = dt.Rows[0][1].ToString();

                            if (ERRCode == "ER")
                            { throw new Exception(); }	// ER 코드 Return시 점프

                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "대상")].Text == "True")
                                hasConfData = true;
                        }
                    }

                    if (!hasConfData) // 확정 데이터가 없을 경우
                    {
                        MSGCode = "P0035";
                        throw new Exception();
                    }

                    // 결과 처리
                    Trans.Commit();
                }
                catch
                {
                    Trans.Rollback();
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);

                    return;
                }

                finally
                {
                    dbConn.Close();
                }

                CheckForIllegalCrossThreadCalls = false;

                // 긴급 처리
                PUA101P1 pu = new PUA101P1(cboSCH_ID.SelectedValue.ToString());
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                    SearchExec();
            }
        }
        #endregion

        #region 그리드 콤보 선택 변경
        private void fpSpread1_ComboSelChange(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            try
            {
                // 재작업 여부/작업지시구분 변경시
                if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "재작업여부"))
                    if (fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "재작업여부")].Value.ToString() == "Y")
                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "재고감안여부")].Value = false;

                if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "작업지시구분"))
                    if (fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "작업지시구분")].Value.ToString() == "3")
                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "재고감안여부")].Value = false;

                if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "단계"))
                {
                    if (fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text != "")
                    {

                        if (fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "단계")].Value.ToString() == "0")
                        {
                            fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "ROUT NO")].Text
                                = SystemBase.Base.CodeName("ITEM_CD", "ROUT_NO", "P_BOP_PROC_MASTER", fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text, "AND MAJOR_FLG = 'Y' AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
                        }
                        else
                        {
                            fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "ROUT NO")].Text = "";
                        }
                    }
                    else
                    {
                        MessageBox.Show(SystemBase.Base.MessageRtn("품목정보를 입력하셔야 선택가능합니다."), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("P0001"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region GridReMake() 그리드 재정의
        public void GridReMake()
        {
            try
            {
                string strStatus = "";
                if (fpSpread1.Sheets[0].Rows.Count > 0)
                {
                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {
                        strStatus = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "처리상태")].Value.ToString();

                        if (strStatus != "P")
                        {
                            UIForm.FPMake.grdReMake(fpSpread1, i,
                                  SystemBase.Base.GridHeadIndex(GHIdx1, "대상")+"|3#" 
                                + SystemBase.Base.GridHeadIndex(GHIdx1, "제조오더번호")+ "|3#"
                                + SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호") + "|3#"
                                + SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호") + "|3#"
                                + SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수") + "|3#"
                                + SystemBase.Base.GridHeadIndex(GHIdx1, "그룹코드") + "|3#"
                                + SystemBase.Base.GridHeadIndex(GHIdx1, "그룹코드") + "|3#"
                                + SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드") + "|3#"
                                + SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드") + "|3#"
                                + SystemBase.Base.GridHeadIndex(GHIdx1, "생산수량") + "|3#"
                                + SystemBase.Base.GridHeadIndex(GHIdx1, "단계") + "|3#"
                                + SystemBase.Base.GridHeadIndex(GHIdx1, "ROUT NO") + "|3#"
                                + SystemBase.Base.GridHeadIndex(GHIdx1, "생산완료일자") + "|3#"
                                + SystemBase.Base.GridHeadIndex(GHIdx1, "수주납기일") + "|3#"
                                + SystemBase.Base.GridHeadIndex(GHIdx1, "재작업여부") + "|3#"
                                + SystemBase.Base.GridHeadIndex(GHIdx1, "작업지시구분") + "|3#"
                                + SystemBase.Base.GridHeadIndex(GHIdx1, "생산담당자") + "|3#"
                                + SystemBase.Base.GridHeadIndex(GHIdx1, "생산담당자") + "|3#"
                                + SystemBase.Base.GridHeadIndex(GHIdx1, "비고") + "|3");
                        }
                        else
                        {
                            UIForm.FPMake.grdReMake(fpSpread1, i,
                                SystemBase.Base.GridHeadIndex(GHIdx1, "대상") + "|0#"
                                + SystemBase.Base.GridHeadIndex(GHIdx1, "제조오더번호") + "|0#"
                                + SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호") + "|1#"
                                + SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호") + "|0#"
                                + SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수") + "|1#"
                                + SystemBase.Base.GridHeadIndex(GHIdx1, "그룹코드") + "|1#"
                                + SystemBase.Base.GridHeadIndex(GHIdx1, "그룹코드") + "|0#"
                                + SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드") + "|1#"
                                + SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드") + "|0#"
                                + SystemBase.Base.GridHeadIndex(GHIdx1, "생산수량") + "|1#"
                                + SystemBase.Base.GridHeadIndex(GHIdx1, "단계") + "|1#"
                                + SystemBase.Base.GridHeadIndex(GHIdx1, "ROUT NO") + "|0#"
                                + SystemBase.Base.GridHeadIndex(GHIdx1, "샌산완료일자") + "|1#"
                                + SystemBase.Base.GridHeadIndex(GHIdx1, "수주납기일") + "|1#"
                                + SystemBase.Base.GridHeadIndex(GHIdx1, "재작업여부") + "|1#"
                                + SystemBase.Base.GridHeadIndex(GHIdx1, "작업지시구분") + "|13#"
                                + SystemBase.Base.GridHeadIndex(GHIdx1, "생산담당자") + "|0#"
                                + SystemBase.Base.GridHeadIndex(GHIdx1, "생산담당자") + "|0#"
                                + SystemBase.Base.GridHeadIndex(GHIdx1, "비고") + "|0");
                        }
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "그리드 재정의"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region TextChanged
        private void txtProj_No_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtProj_No.Text != "")
                {
                    txtProj_Nm.Value = SystemBase.Base.CodeName("PROJECT_NO", "PROJECT_NM", "S_SO_MASTER", txtProj_No.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");
                }
                else
                {
                    txtProj_Nm.Value = "";
                }
            }
            catch
            {

            }
        }

        private void txtItemCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtItemCd.Text != "")
                {
                    txtItemNm.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtItemCd.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");
                }
                else
                {
                    txtItemNm.Value = "";
                }
            }
            catch
            {

            }
        }
        #endregion

        #region 팝업
        //품목코드
        private void btnItem_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW005 pu = new WNDW005(txtItemCd.Text, "");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtItemCd.Text = Msgs[2].ToString();
                    txtItemNm.Value = Msgs[3].ToString();

                    txtItemCd.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목코드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //제품오더fr
        private void btnMakeOrderFr_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW008 pu = new WNDW008(txtMakeOrderFr.Text);
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtMakeOrderFr.Text = Msgs[1].ToString();
                    txtMakeOrderFr.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "제품오더번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //제품오더to
        private void btnMakeOrderTo_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW008 pu = new WNDW008(txtMakeOrderTo.Text);
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtMakeOrderTo.Text = Msgs[1].ToString();
                    txtMakeOrderTo.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "제품오더번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //제조오더fr
        private void btnWorkOrderFr_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW006 pu = new WNDW006(txtWorkOrderFr.Text);
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtWorkOrderFr.Text = Msgs[1].ToString();
                    txtWorkOrderFr.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "제조오더번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //제조오더to
        private void btnWorkOrderTo_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW006 pu = new WNDW006(txtWorkOrderTo.Text);
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtWorkOrderTo.Text = Msgs[1].ToString();
                    txtWorkOrderTo.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "제조오더번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion


    }
}
