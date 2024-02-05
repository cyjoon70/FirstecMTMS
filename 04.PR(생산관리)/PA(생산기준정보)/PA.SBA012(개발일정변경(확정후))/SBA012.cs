#region 작성정보
/*********************************************************************/
// 단위업무명 : 개인일정변경(확정후)
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-02-04
// 작성내용 : 개인일정변경(확정후) 등록 및 관리
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

namespace PA.SBA012
{
    public partial class SBA012 : UIForm.FPCOMM1
    {
        #region 생성자
        public SBA012()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void SBA012_Load(object sender, System.EventArgs e)
        {
            //GroupBox1 초기화
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            //그리드 콤보박스 세팅
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "작업장")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'P002', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//작업장
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "직/간구분")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'P015', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//직/간구분	
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "변경직/간구분")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'P015', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 1);//직/간구분	
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "EIS적용")] = SystemBase.ComboMake.ComboOnGrid("usp_C_COMMON @pType='E010', @pCODE = 'EIS001', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 1);//EIS적용	

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
        }
        #endregion

        #region 행추가 버튼 클릭 이벤트
        protected override void RowInsExec()
        {
            UIForm.FPMake.RowInsert(fpSpread1);

            fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "직/간구분")].Value = "A";
            fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "담당자코드")].Text = SystemBase.Base.gstrUserID.ToString();
            fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "담당자")].Text = SystemBase.Base.gstrUserName.ToString();

            //그리드 속성 재정의 - 필수/일반
            UIForm.FPMake.grdReMake(fpSpread1, fpSpread1.Sheets[0].ActiveRowIndex,
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
                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "부문코드") + "|1"
                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "부문코드_2") + "|1"
                );
        }
        #endregion

        #region 행복사 이벤트
        protected override void RCopyExec()
        {
            try
            {
                UIForm.FPMake.RowCopy(fpSpread1);

                //그리드 속성 재정의 - 필수/일반
                UIForm.FPMake.grdReMake(fpSpread1, fpSpread1.Sheets[0].ActiveRowIndex,
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
                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "부문코드") + "|1"
                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "부문코드_2") + "|1"
                    );
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
        {
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
            SystemBase.Validation.GroupBox_Reset(groupBox1);
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
                    string strQuery = " usp_SBA012  @pTYPE = 'S2'";
                    strQuery += ", @pENT_CD = '" + txtEntCd.Text + "' ";
                    strQuery += ", @pPROJECT_NO = '" + txtProjectNo.Text + "'";
                    strQuery += ", @pPROJECT_SEQ = '" + txtProjectSeq.Text + "'";
                    strQuery += ", @pITEM_CD = '" + txtItemCd.Text + "'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false, true);
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
                    string strPROC_SEQ = "";
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

                                strPROC_SEQ = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "공정순서")].Text;
                                string strSql = " usp_SBA012 '" + strGbn + "'";
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
                                strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                                strSql += ", @pCLS_MONTH = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "변경월")].Text + "'";
                                strSql += ", @pCLS_DIRECT_FLAG = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "변경직/간구분")].Value.ToString() + "'";
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
                        UIForm.FPMake.GridSetFocus(fpSpread1, strPROC_SEQ); //그리드 위치를 가져온다
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
                    string strQuery = " usp_B_COMMON @pTYPE = 'COMM_POP', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pSPEC1 = 'P001' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
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
            else if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "부문코드_2"))
            {

                try
                {
                    string strQuery = " usp_P_COMMON @pTYPE ='P230', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                    string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };
                    string[] strSearch = new string[] { fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "부문코드")].Text, "" };
                    UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P01015", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "부문코드 조회"); //자원코드 사용자조회
                    pu.ShowDialog();
                    if (pu.DialogResult == DialogResult.OK)
                    {
                        Regex rx1 = new Regex("#");
                        string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "부문코드")].Text = Msgs[0].ToString();
                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "부문")].Text = Msgs[1].ToString();
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "부문코드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
            }
            else if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "자원코드_2"))
            {

                try
                {
                    string strQuery = " usp_P_COMMON @pTYPE ='P063', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
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
                    string strQuery = " usp_B_COMMON @pTYPE ='TABLE_POP', @pSPEC1 = 'USR_ID', @pSPEC2 = 'USR_NM', @pSPEC3 = 'B_SYS_USER', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
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
                SBA012P1 frm = new SBA012P1(txtProjectNo.Text);
                frm.ShowDialog();

                if (frm.DialogResult == DialogResult.OK)
                {
                    txtProjectNo.Text = frm.strProjectNo;
                    txtProjectNm.Value = frm.strProjectNm;
                    txtProjectSeq.Value = frm.strProjectSeq;
                    txtEntCd.Value = frm.strEntCd;
                    txtEntNm.Value = frm.strEntNm;
                    txtShipCd.Value = frm.strShipCd;
                    txtShipNm.Value = frm.strShipNm;
                    txtItemCd.Value = frm.strItemCd;
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
                        DataTable dt = SystemBase.DbOpen.NoTranDataTable("USP_SBA012 'C1', @pDATE_FR = '" + fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "시작일자")].Text + "', @pDATE_TO = '" + fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "완료일자")].Text + "' , @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ");

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
                        DataTable dt = SystemBase.DbOpen.NoTranDataTable("USP_SBA012 'C1', @pDATE_FR = '" + fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "시작일자")].Text + "', @pDATE_TO = '" + fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "완료일자")].Text + "' , @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ");

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
                        = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "공정코드")].Text, " AND MAJOR_CD = 'P001'  AND COMP_CODE='" + SystemBase.Base.gstrCOMCD + "'");
                }
                else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "담당자코드"))
                {
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "담당자명")].Text
                        = SystemBase.Base.CodeName("USR_ID", "USR_NM", "B_SYS_USER", fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "담당자코드")].Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
                }
                else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "자원코드"))
                {
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "자원명")].Text
                        = SystemBase.Base.CodeName("RES_CD", "RES_DIS", "P_RESO_MANAGE", fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "자원코드")].Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "작업장")].Value
                        = SystemBase.Base.CodeName("RES_CD", "WORKCENTER_CD", "P_RESO_MANAGE", fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "자원코드")].Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");

                }
                else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "부문코드"))
                {
                    string Query = "usp_SBA012 'C3', @pCOM_CD = '" + fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "부문코드")].Text + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);

                    if (dt.Rows.Count > 0)
                    {
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "부문")].Text = dt.Rows[0][0].ToString();
                    }
                    else
                    {
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "부문")].Text = "";
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "그리드 체인지 이벤트"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 품목코드 입력시
        private void txtProjectNo_TextChanged(object sender, System.EventArgs e)
        {

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
                string strQuery = " usp_SBA012  @pTYPE = 'S5', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
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
                    string strQuery = " usp_SBA012  @pTYPE = 'S5', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
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
