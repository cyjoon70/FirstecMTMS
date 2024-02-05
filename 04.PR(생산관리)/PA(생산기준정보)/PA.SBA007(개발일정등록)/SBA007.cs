#region 작성정보
/*********************************************************************/
// 단위업무명 : 개발일정등록
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-02-01
// 작성내용 : 개발일정등록 및 관리
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

namespace PA.SBA007
{
    public partial class SBA007 : UIForm.FPCOMM1
    {
        public SBA007()
        {
            InitializeComponent();
        }

        #region Form Load 시
        private void SBA007_Load(object sender, System.EventArgs e)
        {
            //GroupBox1 초기화
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            //그리드 콤보박스 세팅
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "작업장")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'P002', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'" , 0);//작업장
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "직/간구분")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'P015', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 0);//직/간구분	
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "EIS적용")] = SystemBase.ComboMake.ComboOnGrid("usp_C_COMMON @pType='E010', @pCODE = 'EIS001', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 1);//EIS적용	

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

            btnProc.Enabled = false;
            btnProcCancel.Enabled = false;
            dtpBaseDt.Value = null;
        }
        #endregion

        #region 행추가 버튼 클릭 이벤트
        protected override void RowInsExec()
        {
            string ChkYn = SystemBase.Base.CodeName("PROJECT_NO", "CONFIRM_YN", "S_DPLAN_SCH", txtProjectNo.Text, " AND PROJECT_SEQ = '" + txtProjectSeq.Text + "' AND ENT_CD = '" + txtEntCd.Text + "' AND ITEM_CD = '" + txtItemCd.Text + "' ");

            if (ChkYn == "Y")
            {
                MessageBox.Show(SystemBase.Base.MessageRtn("B0041"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            UIForm.FPMake.RowInsert(fpSpread1);

            fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "직/간구분")].Value = "A";
            fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "담당자코드")].Text = SystemBase.Base.gstrUserID.ToString();
            fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "담당자")].Text = SystemBase.Base.gstrUserName.ToString();

            if (fpSpread1.Sheets[0].ActiveRowIndex > 0)
            {
                fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "순")].Text
                    = fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex - 1, SystemBase.Base.GridHeadIndex(GHIdx1, "순")].Text;
                fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "부문코드")].Text
                    = fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex - 1, SystemBase.Base.GridHeadIndex(GHIdx1, "부문코드")].Text;
            }
            else
            {
                fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "순")].Text = "1";
                fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "부문코드")].Text = "D";
            }
        }
        #endregion  

        #region 행복사 이벤트
        protected override void RCopyExec()
        {
            string ChkYn = SystemBase.Base.CodeName("PROJECT_NO", "CONFIRM_YN", "S_DPLAN_SCH", txtProjectNo.Text, " AND PROJECT_SEQ = '" + txtProjectSeq.Text + "' AND ENT_CD = '" + txtEntCd.Text + "' AND ITEM_CD = '" + txtItemCd.Text + "' ");

            if (ChkYn == "Y")
            {
                MessageBox.Show(SystemBase.Base.MessageRtn("B0041"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }


            try
            {
                UIForm.FPMake.RowCopy(fpSpread1);
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "행복사"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region DelExec 행 삭제
        protected override void DelExec()
        {	// 행 삭제

            string ChkYn = SystemBase.Base.CodeName("PROJECT_NO", "CONFIRM_YN", "S_DPLAN_SCH", txtProjectNo.Text, " AND PROJECT_SEQ = '" + txtProjectSeq.Text + "' AND ENT_CD = '" + txtEntCd.Text + "' AND ITEM_CD = '" + txtItemCd.Text + "' ");

            if (ChkYn == "Y")
            {
                MessageBox.Show(SystemBase.Base.MessageRtn("B0041"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                UIForm.FPMake.RowRemove(fpSpread1);
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "행삭제"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion
       
        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                string strQuery = " usp_SBA007  @pTYPE = 'S4'";
                strQuery += ", @pENT_CD = '" + txtEntCd.Text + "' ";
                strQuery += ", @pPROJECT_NO = '" + txtProjectNo.Text + "'";
                strQuery += ", @pPROJECT_SEQ = '" + txtProjectSeq.Text + "'";
                strQuery += ", @pITEM_CD = '" + txtItemCd.Text + "'";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";

                DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

                if (dt.Rows.Count > 0)
                {
                    if (dt.Rows[0][0].ToString() == "D001")
                        rdoD001.Checked = true;
                    else
                        rdoD002.Checked = true;

                    //이미 등록되어있습니다. 조회하시겠습니까?
                    if (MessageBox.Show(SystemBase.Base.MessageRtn("S0011", "개발품"), SystemBase.Base.MessageRtn("Z0004"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        SearchExec();
                    }
                }
                else
                {
                    fpSpread1.Sheets[0].Rows.Count = 0;
                    Default_Search();
                }
            }

            dtpBaseDt.Value = null;
        }
        #endregion

        #region Default 조회
        private void Default_Search()
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            try
            {
                string strMajorCd = "";
                if (rdoD001.Checked == true) { strMajorCd = "D001"; }
                else if (rdoD002.Checked == true) { strMajorCd = "D002"; }
                else { strMajorCd = "D003"; }

                string strQuery = " usp_SBA007  @pTYPE = 'S1'";
                strQuery += ", @pMAJOR_CD = '" + strMajorCd + "'";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";

                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false, true);

                //셀 합치기
                if (fpSpread1.Sheets[0].Rows.Count > 0)
                {
                    fpSpread1.Sheets[0].RowHeader.Cells[0, 0].Text = "I";

                    string Dtype = fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "부문")].Text;
                    int SpanCnt = 1;

                    for (int i = 1; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {
                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "부문")].Text == Dtype)
                        {
                            SpanCnt++;

                            fpSpread1.Sheets[0].Cells[i + 1 - SpanCnt, SystemBase.Base.GridHeadIndex(GHIdx1, "부문")].RowSpan = SpanCnt;
                        }
                        else
                        {
                            SpanCnt = 1;
                        }

                        Dtype = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "부문")].Text;

                        fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text = "I";
                    }
                }

                btnProc.Enabled = false;
                btnProcCancel.Enabled = false;
                btnBaseDt.Enabled = true;
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            this.Cursor = System.Windows.Forms.Cursors.Default;
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                try
                {
                    string strMajorCd = "";
                    if (rdoD001.Checked == true) { strMajorCd = "D001"; }
                    else { strMajorCd = "D002"; }

                    string strQuery = " usp_SBA007  @pTYPE = 'S2'";
                    strQuery += ", @pENT_CD = '" + txtEntCd.Text + "' ";
                    strQuery += ", @pPROJECT_NO = '" + txtProjectNo.Text + "'";
                    strQuery += ", @pPROJECT_SEQ = '" + txtProjectSeq.Text + "'";
                    strQuery += ", @pMAJOR_CD = '" + strMajorCd + "'";
                    strQuery += ", @pITEM_CD = '" + txtItemCd.Text + "'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false, true);

                    if (fpSpread1.Sheets[0].Rows.Count > 0)
                    {
                        string Dtype = fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "부문")].Text;
                        int SpanCnt = 1;

                        //셀 합치기
                        for (int i = 1; i < fpSpread1.Sheets[0].Rows.Count; i++)
                        {
                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "부문")].Text == Dtype)
                            {
                                SpanCnt++;

                                fpSpread1.Sheets[0].Cells[i + 1 - SpanCnt, SystemBase.Base.GridHeadIndex(GHIdx1, "부문")].RowSpan = SpanCnt;
                            }
                            else
                            {
                                SpanCnt = 1;
                            }

                            Dtype = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "부문")].Text;
                        }

                        //확정유무
                        string ChkYn = SystemBase.Base.CodeName("PROJECT_NO", "CONFIRM_YN", "S_DPLAN_SCH", txtProjectNo.Text, " AND PROJECT_SEQ = '" + txtProjectSeq.Text + "' AND ENT_CD = '" + txtEntCd.Text + "' AND ITEM_CD = '" + txtItemCd.Text + "' ");

                        if (ChkYn == "Y")
                        {
                            for (int k = 0; k < fpSpread1.Sheets[0].Rows.Count; k++)
                            {
                                //그리드 속성 재정의 - 읽기전용
                                UIForm.FPMake.grdReMake(fpSpread1, k,
                                    SystemBase.Base.GridHeadIndex(GHIdx1, "공정코드") + "|3"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "공정코드_2") + "|3"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "자원코드") + "|3"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "자원코드_2") + "|3"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "시작일자") + "|3"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "완료일자") + "|3"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "L/T") + "|3"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "직/간구분") + "|3"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "EIS적용") + "|3"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "담당자코드") + "|3"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "담당자코드_2") + "|3"
                                    );
                            }

                            string strChkQuery = " USP_SBA007 'C2', @pPROJECT_NO = '" + txtProjectNo.Text + "', @pPROJECT_SEQ = '" + txtProjectSeq.Text + "', @pITEM_CD = '" + txtItemCd.Text + "' ";
                            DataTable ChkDt = SystemBase.DbOpen.NoTranDataTable(strChkQuery);

                            if (ChkDt.Rows.Count > 0)
                            {
                                btnProc.Enabled = false;
                                btnProcCancel.Enabled = false;
                                btnBaseDt.Enabled = false;
                            }
                            else
                            {
                                btnProc.Enabled = false;
                                btnProcCancel.Enabled = true;
                                btnBaseDt.Enabled = false;
                            }
                        }
                        else
                        {
                            for (int k = 0; k < fpSpread1.Sheets[0].Rows.Count; k++)
                            {
                                //그리드 속성 재정의 - 필수/일반
                                UIForm.FPMake.grdReMake(fpSpread1, k,
                                    SystemBase.Base.GridHeadIndex(GHIdx1, "공정코드") + "|1"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "공정코드_2") + "|0"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "자원코드") + "|1"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "자원코드_2") + "|0"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "시작일자") + "|1"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "완료일자") + "|1"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "L/T") + "|1"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "직/간구분") + "|1"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "EIS적용") + "|0"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "담당자코드") + "|1"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "담당자코드_2") + "|0"
                                    );
                            }
                            btnProc.Enabled = true;
                            btnProcCancel.Enabled = false;
                            btnBaseDt.Enabled = true;
                        }

                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            this.Cursor = System.Windows.Forms.Cursors.Default;
        }
        #endregion
                
        #region SaveExec() 폼에 입력된 데이타 저장 로직
        protected override void SaveExec()
        {
            this.Cursor = Cursors.WaitCursor;

            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                if (SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", true))
                {
                    string ERRCode = "ER", MSGCode = "P0000"; //처리할 내용이 없습니다.
                    string strProcSeq = "";

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

                                strProcSeq = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "공정순서")].Text;

                                string strSql = " usp_SBA007 '" + strGbn + "'";
                                strSql += ", @pENT_CD = '" + txtEntCd.Text + "'";
                                strSql += ", @pPROJECT_NO = '" + txtProjectNo.Text + "'";
                                strSql += ", @pPROJECT_SEQ = '" + txtProjectSeq.Text + "'";
                                strSql += ", @pITEM_CD = '" + txtItemCd.Text + "'";
                                strSql += ", @pDPLAN_TYPE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "부문코드")].Value.ToString() + "'";
                                strSql += ", @pPROC_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "공정순서")].Text + "'";
                                strSql += ", @pJOB_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "공정코드")].Text + "'";
                                strSql += ", @pRES_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "자원코드")].Text + "'";
                                strSql += ", @pWC_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "작업장")].Value.ToString() + "'";
                                strSql += ", @pRUN_TIME = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "L/T")].Value + "'";
                                strSql += ", @pSTART_DT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "시작일자")].Text + "'";
                                strSql += ", @pEND_DT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "완료일자")].Text + "'";
                                strSql += ", @pDIRECT_FLAG = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "직/간구분")].Value.ToString() + "'";
                                strSql += ", @pEIS_ELEMENT= '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "EIS적용")].Value + "'";
                                strSql += ", @pDPLAN_DUTY = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "담당자코드")].Text + "'";
                                strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                                strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";

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
                        MSGCode = "P0001";	//에러가 발생되어 데이터 처리가 취소되었습니다.
                    }
                Exit:
                    dbConn.Close();

                    if (ERRCode == "OK")
                    {
                        MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                        SearchExec();
                        UIForm.FPMake.GridSetFocus(fpSpread1, strProcSeq); //저장 후 그리드 포커스 이동

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
            this.Cursor = Cursors.Default;
        }
        #endregion
        
        #region 그리드 버튼 클릭
        private void fpSpread1_ButtonClicked(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            //공정작업코드
            if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "공정코드_2"))
            {
                try
                {
                    string strQuery = " usp_B_COMMON @pTYPE = 'COMM_POP', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pSPEC1 = 'P001', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                    string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                    string[] strSearch = new string[] { fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "공정코드")].Text, "" };

                    UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P01007", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "공정 조회");	//공정작업코드 사용자조회
                    pu.ShowDialog();

                    if (pu.DialogResult == DialogResult.OK)
                    {
                        Regex rx1 = new Regex("#");
                        string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "공정코드")].Text = Msgs[0].ToString();
                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "공정명")].Text = Msgs[1].ToString();
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "공정 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
            }
            else if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "자원코드_2"))
            {

                try
                {
                    string strQuery = " usp_P_COMMON @pTYPE ='P063', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'"; ;
                    string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };
                    string[] strSearch = new string[] { fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "자원코드")].Text, "" };
                    UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00066", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "자원 조회"); //자원코드 사용자조회
                    pu.ShowDialog();
                    if (pu.DialogResult == DialogResult.OK)
                    {
                        Regex rx1 = new Regex("#");
                        string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "자원코드")].Text = Msgs[0].ToString();
                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "자원명")].Text = Msgs[1].ToString();
                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "작업장")].Value = Msgs[2].ToString();
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "자원 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
            }
            else if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "담당자코드_2"))
            {
                try
                {
                    string strQuery = " usp_B_COMMON @pTYPE ='TABLE_POP', @pSPEC1 = 'USR_ID', @pSPEC2 = 'USR_NM', @pSPEC3 = 'B_SYS_USER', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                    string[] strWhere = new String[] { "@pCODE", "@pNAME" };
                    string[] strSearch = new string[] { fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "담당자코드")].Text, "" };
                    UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P01008", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "담당자 조회"); //담당자코드 사용자조회
                    pu.ShowDialog();
                    if (pu.DialogResult == DialogResult.OK)
                    {
                        Regex rx1 = new Regex("#");
                        string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "담당자코드")].Text = Msgs[0].ToString();
                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "담당자명")].Text = Msgs[1].ToString();
                    }

                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "담당자 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
            }
        }
        #endregion	
        
        #region 프로젝트 팝업(수주참조)
        private void btnProject_Click(object sender, System.EventArgs e)
        {
            try
            {
                SBA007P1 frm = new SBA007P1(txtProjectNo.Text);
                frm.ShowDialog();

                if (frm.DialogResult == DialogResult.OK)
                {
                    txtProjectNo.Text = frm.strProjectNo;
                    txtProjectNm.Value = frm.strProjectNm;
                    txtProjectSeq.Text = frm.strProjectSeq;
                    txtEntCd.Text = frm.strEntCd;
                    txtEntNm.Value = frm.strEntNm;
                    txtShipCd.Value = frm.strShipCd;
                    txtShipNm.Value = frm.strShipNm;
                    txtItemCd.Text = frm.strItemCd;
                    txtItemNm.Value = frm.strItemNm;
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "수주참조 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        #endregion

        #region 그리드 체인지 이벤트 - 시작, 완료일자 변경시 L/T자동계산
        protected override void fpSpread1_ChangeEvent(int Row, int Column)
        {
            try
            {
                if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "시작일자"))
                {
                    if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "시작일자")].Text == "")
                    {
                        MessageBox.Show(SystemBase.Base.MessageRtn("시작일자를 입력하여주세요!"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                        fpSpread1.ActiveSheet.SetActiveCell(Row, SystemBase.Base.GridHeadIndex(GHIdx1, "시작일자"));
                        return;
                    }

                    if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "완료일자")].Text != "")
                    {
                        DataTable dt = SystemBase.DbOpen.NoTranDataTable("USP_SBA007 'C1', @pDATE_FR = '" + fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "시작일자")].Text + "', @pDATE_TO = '" + fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "완료일자")].Text + "' ");

                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "L/T")].Text = dt.Rows[0][0].ToString();
                    }
                    else
                    {
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "L/T")].Text = "0";
                    }
                }
                else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "완료일자"))
                {
                    if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "완료일자")].Text == "")
                    {
                        MessageBox.Show(SystemBase.Base.MessageRtn("완료일자를 입력하여주세요!"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                        fpSpread1.ActiveSheet.SetActiveCell(Row, SystemBase.Base.GridHeadIndex(GHIdx1, "완료일자"));
                        return;
                    }

                    if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "시작일자")].Text != "")
                    {
                        DataTable dt = SystemBase.DbOpen.NoTranDataTable("USP_SBA007 'C1', @pDATE_FR = '" + fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "시작일자")].Text + "', @pDATE_TO = '" + fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "완료일자")].Text + "' ");

                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "L/T")].Text = dt.Rows[0][0].ToString();
                    }
                    else
                    {
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "L/T")].Text = "0";
                    }

                }
                else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "공정코드"))
                {
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "공정명")].Text
                        = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "공정코드")].Text, " AND MAJOR_CD = 'P001' ");
                }
                else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "담당자코드"))
                {
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "담당자명")].Text
                        = SystemBase.Base.CodeName("USR_ID", "USR_NM", "B_SYS_USER", fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "담당자코드")].Text, "");
                }
                else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "자원코드"))
                {
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "자원명")].Text
                        = SystemBase.Base.CodeName("RES_CD", "RES_DIS", "P_RESO_MANAGE", fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "자원코드")].Text, "");
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "작업장")].Value
                        = SystemBase.Base.CodeName("RES_CD", "WORKCENTER_CD", "P_RESO_MANAGE", fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "자원코드")].Text, "");

                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "그리드 체인지 이벤트"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion
        
        #region 확정
        private void btnProc_Click(object sender, System.EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;

            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                if (UIForm.FPMake.FPUpCheck(fpSpread1, false) == true)
                {
                    string ERRCode = "ER", MSGCode = "P0000"; //처리할 내용이 없습니다.

                    SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                    SqlCommand cmd = dbConn.CreateCommand();
                    SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                    try
                    {
                        string strSql = " usp_SBA007 'P1'";
                        strSql += ", @pENT_CD = '" + txtEntCd.Text + "'";
                        strSql += ", @pPROJECT_NO = '" + txtProjectNo.Text + "'";
                        strSql += ", @pPROJECT_SEQ = '" + txtProjectSeq.Text + "'";
                        strSql += ", @pITEM_CD = '" + txtItemCd.Text + "'";
                        strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                        strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";

                        DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                        ERRCode = ds.Tables[0].Rows[0][0].ToString();
                        MSGCode = ds.Tables[0].Rows[0][1].ToString();

                        if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                        Trans.Commit();
                    }
                    catch (Exception f)
                    {
                        SystemBase.Loggers.Log(this.Name, f.ToString());
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

            this.Cursor = Cursors.Default;
        }
        #endregion

        #region 확정취소
        private void btnProcCancel_Click(object sender, System.EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;

            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                if (UIForm.FPMake.FPUpCheck(fpSpread1, false) == true)
                {
                    string msgCode = "S0009";//확정 취소 시 현재 등록되어있는 ||의 MPS 및 작업지시서의 내용이 모두 삭제됩니다. ||계속 하시겠습니까?

                    string msg = SystemBase.Base.MessageRtn(msgCode, txtProjectNo.Text + "#\n");
                    DialogResult dsMsg = MessageBox.Show(msg, SystemBase.Base.MessageRtn("Z0004"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (dsMsg == DialogResult.Yes)
                    {
                        string ERRCode = "ER", MSGCode = "P0000"; //처리할 내용이 없습니다.

                        SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                        SqlCommand cmd = dbConn.CreateCommand();
                        SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                        try
                        {
                            string strSql = " usp_SBA007 'P2'";
                            strSql += ", @pENT_CD = '" + txtEntCd.Text + "'";
                            strSql += ", @pPROJECT_NO = '" + txtProjectNo.Text + "'";
                            strSql += ", @pPROJECT_SEQ = '" + txtProjectSeq.Text + "'";
                            strSql += ", @pITEM_CD = '" + txtItemCd.Text + "'";
                            strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                            strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";

                            DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                            ERRCode = ds.Tables[0].Rows[0][0].ToString();
                            MSGCode = ds.Tables[0].Rows[0][1].ToString();

                            if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                            Trans.Commit();
                        }
                        catch (Exception f)
                        {
                            SystemBase.Loggers.Log(this.Name, f.ToString());
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

            this.Cursor = Cursors.Default;
        }
        #endregion
        
        #region 품목코드 입력시
        private void txtProjectNo_TextChanged(object sender, System.EventArgs e)
        {
            //내용없음
        }
        #endregion

        #region 일괄적용버튼클릭시
        private void btnBaseDt_Click(object sender, System.EventArgs e)
        {
            if (fpSpread1.Sheets[0].Rows.Count > 0)
            {
                try
                {
                    if (dtpBaseDt.Text != "")
                    {
                        DateTime dTime = Convert.ToDateTime(dtpBaseDt.Text);

                        //기준일이 휴무일이거나 주말이면다시입력
                        if (dTime != Get_Date(dTime, 0))
                        {
                            MessageBox.Show(SystemBase.Base.MessageRtn("기준일이 휴일입니다. 다시 입력하세요!"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            dtpBaseDt.Focus();
                            return;
                        }

                        Set_Date(dTime, 0);
                    }
                    else
                    {
                        MessageBox.Show(SystemBase.Base.MessageRtn("기준일을 입력하세요"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        dtpBaseDt.Focus();
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "기준일 일괄적용"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        #endregion
        
        #region L/T 계산
        private int Set_LT(DateTime dtFr, DateTime dtTo)
        {
            DateTime dTimeFr = dtFr;
            DateTime dTimeTo = dtTo;
            int iLt = 0;

            while (dTimeFr != dTimeTo)
            {
                dTimeFr = dTimeFr.AddDays(1);

                //휴무일
                string strQuery = " usp_SBA007  @pTYPE = 'S5', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                DataTable dTable = SystemBase.DbOpen.NoTranDataTable(strQuery);

                for (int j = 0; j < dTable.Rows.Count; j++)
                {
                    //휴무일
                    if (dTimeFr.ToString().IndexOf(dTable.Rows[j][0].ToString()) >= 0)
                    {
                        dTimeFr = dTimeFr.AddDays(1);
                    }

                    //주말
                    if (dTimeFr.ToLongDateString().IndexOf("토요일") >= 0) //토요일이면
                    {
                        dTimeFr = dTimeFr.AddDays(2);
                    }
                    else if (dTimeFr.ToLongDateString().IndexOf("일요일") >= 0)//일요일이면
                    {
                        dTimeFr = dTimeFr.AddDays(1);
                    }
                }

                iLt++;
            }

            return iLt;
        }
        #endregion

        #region 날자계산
        private DateTime Get_Date(DateTime dt, int LT)
        {
            DateTime dTime = dt;

            try
            {
                //LT 값 받아서 값 생성
                for (int i = 0; i <= LT; i++)
                {
                    if (i > 0)
                        dTime = dTime.AddDays(1);

                    //휴무일
                    string strQuery = " usp_SBA007  @pTYPE = 'S5', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                    DataTable dTable = SystemBase.DbOpen.NoTranDataTable(strQuery);

                    for (int j = 0; j < dTable.Rows.Count; j++)
                    {
                        //휴무일
                        if (dTime.ToShortDateString().IndexOf(dTable.Rows[j][0].ToString(), 5, 5) >= 0)
                        {
                            dTime = dTime.AddDays(1);
                        }
                    }

                    //주말
                    if (dTime.ToLongDateString().IndexOf("토요일") >= 0) //토요일이면
                    {
                        dTime = dTime.AddDays(2);
                    }
                    else if (dTime.ToLongDateString().IndexOf("일요일") >= 0)//일요일이면
                    {
                        dTime = dTime.AddDays(1);
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "날자계산"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return dTime;
        }
        #endregion

        #region 그리드에 날자입력
        private void Set_Date(DateTime dt, int Row)
        {
            DateTime dTime = dt;

            //그리드에 날자입력
            for (int i = Row; i < fpSpread1.Sheets[0].Rows.Count; i++)
            {
                dTime = Get_Date(dTime, 0);

                fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "시작일자")].Value = dTime;

                if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "L/T")].Text == ""
                    || Convert.ToInt32(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "L/T")].Text) <= 0)
                {
                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "L/T")].Value = 1;
                }

                dTime = Get_Date(dTime, Convert.ToInt32(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "L/T")].Text));

                fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "완료일자")].Value = dTime;

                dTime = dTime.AddDays(1);
            }
        }
        #endregion

    }
}
