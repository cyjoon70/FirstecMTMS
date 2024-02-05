#region 작성정보
/*********************************************************************/
// 단위업무명 : 품질검사 작업일보 등록
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-03-26
// 작성내용 : 품질검사 작업일보 등록 및 관리
// 수 정 일 :
// 수 정 자 :
// 수정내용 :
// 비    고 :
/*********************************************************************/
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WNDW;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace PE.PEA012
{
    public partial class PEA012 : UIForm.FPCOMM2_2
    {
        #region 변수선언
        bool chk = true;
        string NewFlag = "1";
        #endregion

        #region 생성자
        public PEA012()
        {
            InitializeComponent();
        }
        #endregion

        #region 폼로드 이벤트
        private void PEA012_Load(object sender, System.EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(groupBox1);//필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox2);//필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox3);//필수체크
            SystemBase.Validation.GroupBox_Setting(GridCommGroupBox2);//필수체크

            txtSPlantCd.Value = SystemBase.Base.gstrPLANT_CD; //로그인ID 소속 공장
            dtpWorkDt.Text = SystemBase.Base.ServerTime("YYMMDD");
            //그리드 콤보박스 세팅
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "구분")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'P064', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);

            string Query = " usp_PEA012 @pTYPE = 'S3', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'"; //간접시수(비가동코드) default 세팅
            UIForm.FPMake.grdCommSheet(fpSpread2, Query, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, false, 0, 0);

            for (int i = 0; i < fpSpread2.Sheets[0].Rows.Count; i++)
            {
                fpSpread2.Sheets[0].RowHeader.Cells[i, 0].Text = "I";
            }
            UIForm.FPMake.grdReMake(fpSpread2, "1|1#2|1");

            NewFlag = "1";
        }
        #endregion
        
        #region NewExec() 신규
        protected override void NewExec()
        {
            txtSPlantCd.Value = SystemBase.Base.gstrPLANT_CD;

            SystemBase.Validation.GroupBox_Reset(groupBox1);
            SystemBase.Validation.GroupBox_Reset(groupBox2);
            SystemBase.Validation.GroupBox_Reset(groupBox3);
            SystemBase.Validation.GroupBox_Reset(GridCommGroupBox2);
            dtpWorkDt.Text = SystemBase.Base.ServerTime("YYMMDD");
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "구분")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'P064', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);

            string Query = " usp_PEA012 @pTYPE = 'S3', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'" ; //간접시수(비가동코드) default 세팅
            UIForm.FPMake.grdCommSheet(fpSpread2, Query, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, false, 0, 0);

            for (int i = 0; i < fpSpread2.Sheets[0].Rows.Count; i++)
            {
                fpSpread2.Sheets[0].RowHeader.Cells[i, 0].Text = "I";
            }
            UIForm.FPMake.grdReMake(fpSpread2, "1|1#2|1");

            txtSPlantCd.Value = SystemBase.Base.gstrPLANT_CD;

            dtpWorkDt.Tag = ";1;;";
            dtpWorkDt.Enabled = true;
            dtpWorkDt.Value = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0,10);
            txtWc_Cd.Tag = "작업장;1;;";
            txtWc_Cd.BackColor = SystemBase.Validation.Kind_LightCyan;
            txtWc_Cd.ReadOnly = false;
            btnWc.Enabled = true;
            txtWorkDutyId.Tag = "작업자;1;;";
            txtWorkDutyId.BackColor = SystemBase.Validation.Kind_LightCyan;
            txtWorkDutyId.ReadOnly = false;
            btnWorkDuty.Enabled = true;

            NewFlag = "1";
        }
        #endregion

        #region RowInsert() 행추가
        protected override void RowInsExec()
        {
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox2))
            {
                try
                {
                    if (fpSpread2.Focused == true)
                    {

                        UIForm.FPMake.RowInsert(fpSpread2);

                        fpSpread2.Sheets[0].Cells[fpSpread2.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx2, "시수(분)")].Value = 0;
                    }
                    else
                    {
                        UIForm.FPMake.RowInsert(fpSpread1);

                        fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "구분")].Value = "A";
                        fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "실동시간(분)")].Value = 0;
                        fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "준비시간(분)")].Value = 0;
                        fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "작업시간(분)")].Value = 0;

                        UIForm.FPMake.grdReMake(fpSpread1, fpSpread1.Sheets[0].ActiveRowIndex,
                            SystemBase.Base.GridHeadIndex(GHIdx1, "검사의뢰번호") + "|1"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "검사의뢰번호_2") + "|0"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호") + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호_2") + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드") + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드_2") + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "작업코드") + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "작업코드_2") + "|3"
                            );
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0052"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        #endregion

        #region DelExec() 삭제 로직
        protected override void DeleteExec()
        {
            this.Cursor = Cursors.WaitCursor;

            string msg = SystemBase.Base.MessageRtn("B0027");
            DialogResult dsMsg = MessageBox.Show(msg, SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dsMsg == DialogResult.Yes)
            {
                string ERRCode = "ER", MSGCode = "P0000";	//처리할 내용이 없습니다.
                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                try
                {
                    string strSql = " usp_PEA012  'D1'";
                    strSql += ", @pWORK_DAY_NO = '" + txtWorkDayNo.Text + "' ";
                    strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

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
                    MSGCode = "P0001";	//에러가 발생하여 데이터 처리가 취소되었습니다.
                }
            Exit:
                dbConn.Close();

                if (ERRCode == "OK")
                {
                    NewExec();
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

            this.Cursor = Cursors.Default;
        }
        #endregion

        #region SaveExec() 저장
        protected override void SaveExec()
        {
            string fcsStr = "";
            string fcsStr2 = "";

            txtSWorkDayNo.Focus();
            string strWorkDayNo = "";

            this.Cursor = Cursors.WaitCursor;

            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox2))
            {
                string ERRCode = "ER", MSGCode = "P0000"; //처리할 내용이 없습니다.
                string strGbn = "";

                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                try
                {
                    /*##################################### 직접시간 등록 ###############################################*/

                    /*------------------------------------ MASTER 등록/수정 ---------------------------------------------*/

                    string strMType = "I1";
                    if (txtWorkDayNo.Text != "") { strMType = "U1"; }

                    string strSql = " usp_PEA012 '" + strMType + "'";
                    strSql += ", @pWORK_DAY_NO = '" + txtWorkDayNo.Text + "' ";
                    strSql += ", @pPLANT_CD = '" + txtSPlantCd.Text + "' ";
                    strSql += ", @pWORK_DUTY = '" + txtWorkDutyId.Text + "' ";
                    strSql += ", @pWC_CD = '" + txtWc_Cd.Text + "' ";
                    strSql += ", @pWORK_DT = '" + dtpWorkDt.Text + "' ";

                    strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                    strSql += ", @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

                    DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                    ERRCode = ds.Tables[0].Rows[0][0].ToString();
                    MSGCode = ds.Tables[0].Rows[0][1].ToString();

                    if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                    strWorkDayNo = ds.Tables[0].Rows[0][2].ToString();
                    
                    //행수만큼 처리
                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {
                        string strHead = fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text;
                        strGbn = "";

                        if (strHead.Length > 0)
                        {
                            switch (strHead)
                            {
                                case "I": strGbn = "I2"; break;
                                case "U": strGbn = "U2"; break;
                                case "D": strGbn = "D2"; break;
                                default: strGbn = ""; break;
                            }

                            //필수체크
                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "구분")].Value.ToString() == "A")
                            {
                                if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사의뢰번호")].Text == "")
                                {
                                    MessageBox.Show("[ " + Convert.ToString(i + 1) + " ] 번째 Row의 검사의뢰번호가 입력되지 않았습니다.", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    Trans.Rollback();
                                    return;
                                }
                            }
                            else
                            {
                                if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text == "")
                                {
                                    MessageBox.Show("[ " + Convert.ToString(i + 1) + " ] 번째 Row의 프로젝트번호가 입력되지 않았습니다.", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    Trans.Rollback();
                                    return;
                                }

                                if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Text == "")
                                {
                                    MessageBox.Show("[ " + Convert.ToString(i + 1) + " ] 번째 Row의 차수가 입력되지 않았습니다. 프로젝트 팝업을 이용해주십시오.", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    Trans.Rollback();
                                    return;
                                }

                                if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text == "")
                                {
                                    MessageBox.Show("[ " + Convert.ToString(i + 1) + " ] 번째 Row의 품목코드가 입력되지 않았습니다.", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    Trans.Rollback();
                                    return;
                                }

                                if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "작업코드")].Text == "")
                                {
                                    MessageBox.Show("[ " + Convert.ToString(i + 1) + " ] 번째 Row의 작업코드가 입력되지 않았습니다.", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    Trans.Rollback();
                                    return;
                                }
                            }

                            if (txtWorkDayNo.Text != "")
                            { strWorkDayNo = txtWorkDayNo.Text; }

                            fcsStr = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사의뢰번호")].Text;

                            string strDSql = " usp_PEA012 '" + strGbn + "'";
                            strDSql += ", @pINSP_STAND_FLAG = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "구분")].Value + "' ";
                            strDSql += ", @pINSP_REQ_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사의뢰번호")].Text + "' ";
                            strDSql += ", @pINSP_CLASS_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사유형코드")].Text + "' ";
                            strDSql += ", @pWORK_DAY_NO = '" + strWorkDayNo + "' ";
                            strDSql += ", @pWORK_DAY_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "SEQ")].Value + "' ";
                            strDSql += ", @pPROJECT_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text + "' ";
                            strDSql += ", @pPROJECT_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Text + "' ";
                            strDSql += ", @pJOB_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "작업코드")].Text.Trim() + "' ";
                            strDSql += ", @pWC_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "작업장")].Text + "' ";
                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "준비시간(분)")].Text != "")
                            { strDSql += ", @pSETUP_TIME = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "준비시간(분)")].Value + "' "; }
                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "실동시간(분)")].Text != "")
                            { strDSql += ", @pWORK_TIME = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "실동시간(분)")].Value + "' "; }
                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "작업시간(분)")].Text != "")
                            { strDSql += ", @pWORK_HUMANTIME = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "작업시간(분)")].Value + "' "; }
                            strDSql += ", @pINSP_WORK_REMARK = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "비고")].Text + "' ";
                            strDSql += ", @pWORK_DUTY = '" + txtWorkDutyId.Text + "' ";
                            strDSql += ", @pWORK_DT = '" + dtpWorkDt.Text + "' ";
                            strDSql += ", @pITEM_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text + "' ";
                            strDSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                            strDSql += ", @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

                            DataSet ds1 = SystemBase.DbOpen.TranDataSet(strDSql, dbConn, Trans);

                            ERRCode = ds1.Tables[0].Rows[0][0].ToString();
                            MSGCode = ds1.Tables[0].Rows[0][1].ToString();

                            if (ERRCode != "OK")
                            {
                                Trans.Rollback();
                                goto Exit;
                            }	// ER 코드 Return시 점프
                        }
                    }

                    /*##################################### 간접시간 등록 ###############################################*/
                    //행수만큼 처리
                    for (int i = 0; i < fpSpread2.Sheets[0].Rows.Count; i++)
                    {
                        string strHead = fpSpread2.Sheets[0].RowHeader.Cells[i, 0].Text;
                        strGbn = "";

                        if (strHead.Length > 0)
                        {
                            switch (strHead)
                            {
                                case "I": strGbn = "I3"; break;
                                case "U": strGbn = "U3"; break;
                                case "D": strGbn = "D3"; break;
                                default: strGbn = ""; break;
                            }

                            if (fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "항목")].Text == "")
                            {
                                MessageBox.Show("[ " + Convert.ToString(i + 1) + " ] 번째 Row의 항목코드가 입력되지 않았습니다.", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                Trans.Rollback();
                                return;
                            }

                            if (txtWorkDayNo.Text != "")
                            { strWorkDayNo = txtWorkDayNo.Text; }

                            // 그리드 상단 필수항목 체크
                            fcsStr2 = fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "항목")].Text;

                            string strISql = " usp_PEA012 '" + strGbn + "'";
                            strISql += ", @pWORK_DAY_NO = '" + strWorkDayNo + "' ";
                            strISql += ", @pINDIRECT_NO = '" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "항목")].Text + "' ";
                            strISql += ", @pINDIRECT_TM = '" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "시수(분)")].Value + "' ";

                            strISql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                            strISql += ", @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

                            DataSet ds2 = SystemBase.DbOpen.TranDataSet(strISql, dbConn, Trans);
                            ERRCode = ds2.Tables[0].Rows[0][0].ToString();
                            MSGCode = ds2.Tables[0].Rows[0][1].ToString();

                            if (ERRCode != "OK")
                            {
                                Trans.Rollback();
                                goto Exit;
                            }	// ER 코드 Return시 점프
                        }
                    }

                    Trans.Commit();
                }
                catch (Exception e)
                {
                    SystemBase.Loggers.Log(this.Name, e.ToString());
                    MessageBox.Show(e.ToString());
                    Trans.Rollback();
                    ERRCode = "ER";
                    MSGCode = "P0001";	//에러가 발생되어 데이터 처리가 취소되었습니다.
                }
            Exit:
                dbConn.Close();

                if (ERRCode == "OK")
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtSWorkDayNo.Value = strWorkDayNo;
                    SearchExec();
                    UIForm.FPMake.GridSetFocus(fpSpread1, fcsStr); //저장 후 그리드 포커스 이동
                    UIForm.FPMake.GridSetFocus(fpSpread2, fcsStr2); //저장 후 그리드 포커스 이동                    
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

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

                try
                {
                    string strQuery = " usp_PEA012  @pTYPE = 'S1'";
                    strQuery += ", @pPLANT_CD = '" + txtSPlantCd.Text + "' ";
                    strQuery += ", @pWORK_DAY_NO = '" + txtSWorkDayNo.Text + "' ";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);

                    SystemBase.Validation.GroupBox_Reset(groupBox3);
                    if (fpSpread1.Sheets[0].Rows.Count > 0)
                    {
                        chk = false;

                        SubData(0);

                        NewFlag = "2";
                    }
                    MasterData(txtSWorkDayNo.Text);
                    SubSearch(txtSWorkDayNo.Text);

                    if (txtWorkDayNo.Text == "" && fpSpread1.Sheets[0].Rows.Count == 0 && fpSpread2.Sheets[0].Rows.Count == 0)
                    {
                        NewExec();
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                this.Cursor = System.Windows.Forms.Cursors.Default;

                chk = true;
            }
        }
        #endregion

        #region SubSearch() 간접항목 그리드 조회 로직
        protected void SubSearch(string strWorkDayNo)
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            try
            {
                string strQuery = " usp_PEA012  @pTYPE = 'S4'";
                strQuery += ", @pWORK_DAY_NO = '" + strWorkDayNo + "' ";
                strQuery += ", @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

                UIForm.FPMake.grdCommSheet(fpSpread2, strQuery, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, false, 0, 0);

                if (fpSpread2.Sheets[0].Rows.Count > 0)
                {
                    //간접소계
                    int InWorkTm = 0;

                    for (int i = 0; i < fpSpread2.Sheets[0].Rows.Count; i++)
                    {
                        InWorkTm = InWorkTm + Convert.ToInt32(fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "시수(분)")].Value);
                    }

                    txtInDirectSum.Value = InWorkTm.ToString();

                    NewFlag = "2";
                }
                else
                {
                    txtInDirectSum.Value = "0";
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            this.Cursor = System.Windows.Forms.Cursors.Default;
        }
        #endregion

        #region 코드입력시 코드명 자동입력
        //공장
        private void txtSPlantCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtSPlantCd.Text != "")
                {
                    txtSPlantNm.Value = SystemBase.Base.CodeName("PLANT_CD", "PLANT_NM", "B_PLANT_INFO", txtSPlantCd.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");
                }
                else
                {
                    txtSPlantNm.Value = "";
                }
            }
            catch
            {

            }
        }
        //작업자
        private void txtWorkDutyId_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtWorkDutyId.Text != "")
                {
                    txtWorkDutyNm.Value = SystemBase.Base.CodeName("RES_CD", "RES_DIS", "P_RESO_MANAGE", txtWorkDutyId.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");
                }
                else
                {
                    txtWorkDutyNm.Value = "";
                }
            }
            catch
            {

            }
        }
        //품목코드
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
        //작업장
        private void txtWc_Cd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtWc_Cd.Text != "")
                {
                    txtWc_Nm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtWc_Cd.Text, " AND MAJOR_CD = 'P002'  AND COMP_CODE='" + SystemBase.Base.gstrCOMCD + "' ");
                }
                else
                {
                    txtWc_Nm.Value = "";
                }
            }
            catch
            {

            }
        }
        //작업
        private void txtJobCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtJobCd.Text != "")
                {
                    txtJobNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtJobCd.Text, " AND MAJOR_CD = 'P001'  AND COMP_CODE='" + SystemBase.Base.gstrCOMCD + "' ");
                }
                else
                {
                    txtJobNm.Value = "";
                }
            }
            catch
            {

            }
        }
        //사업
        private void txtEntCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtEntCd.Text != "")
                {
                    txtEntNm.Value = SystemBase.Base.CodeName("ENT_CD", "ENT_NM", "S_ENTERPRISE_INFO", txtEntCd.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");
                }
                else
                {
                    txtEntNm.Value = "";
                }
            }
            catch
            {

            }
        }
        #endregion

        #region 팝업창
        //공장
        private void btnSPlant_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pTYPE = 'P013', @pBIZ_CD = '" + SystemBase.Base.gstrBIZCD + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'"; // 쿼리
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };											  // 쿼리 인자값(조회조건)
                string[] strSearch = new string[] { txtSPlantCd.Text, "" };															  // 쿼리 인자값에 들어갈 데이타

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00005", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "공장 조회", false);

                pu.ShowDialog();	//공통 팝업 호출
                if (pu.DialogResult == DialogResult.OK)
                {
                    string MSG = pu.ReturnVal.Replace("|", "#");
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(MSG);

                    txtSPlantCd.Value = Msgs[0].ToString();
                    txtSPlantNm.Value = Msgs[1].ToString();
                    txtSPlantCd.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "공장 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //작업일보번호
        private void btnWorkDay_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW.WNDW025 pu = new WNDW.WNDW025(txtSWorkDayNo.Text, "3");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtSWorkDayNo.Value = Msgs[1].ToString();
                    txtSWorkDayNo.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "자산정보 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //작업자
        private void btnWorkDuty_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (txtWc_Cd.Text == "")
                {
                    MessageBox.Show("소속 작업장이 선택되지 않았습니다. 작업장을 먼저 선택하십시오.", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                string strQuery = " usp_P_COMMON @pTYPE = 'P121', @pETC = '" + txtWc_Cd.Text + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";				// 쿼리
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };			// 쿼리 인자값(조회조건)
                string[] strSearch = new string[] { txtWorkDutyId.Text, "" };			// 쿼리 인자값에 들어갈 데이타

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00071", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "작업자 조회", false);
                pu.ShowDialog();	//공통 팝업 호출
                if (pu.DialogResult == DialogResult.OK)
                {
                    string MSG = pu.ReturnVal.Replace("|", "#");
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(MSG);

                    txtWorkDutyId.Value = Msgs[0].ToString();
                    txtWorkDutyNm.Value = Msgs[1].ToString();
                    txtWorkDutyId.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "작업자 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //작업장
        private void btnWc_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pTYPE = 'P242', @pLANG_CD = 'KOR', @pETC = 'P002' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";					// 쿼리
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };					// 쿼리 인자값(조회조건)
                string[] strSearch = new string[] { txtWc_Cd.Text, "" };								// 쿼리 인자값에 들어갈 데이타

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00025", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "작업장 조회", false);
                pu.ShowDialog();	//공통 팝업 호출
                if (pu.DialogResult == DialogResult.OK)
                {
                    string MSG = pu.ReturnVal.Replace("|", "#");
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(MSG);

                    txtWc_Cd.Value = Msgs[0].ToString();
                    txtWc_Nm.Value = Msgs[1].ToString();
                    txtWcCd.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "작업장 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 폼 닫을때 변경된 데이터 체크
        private void PEA012_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            int UpCount = 0;

            for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
            {
                if (fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text == "I" || fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text == "U")
                    UpCount++;
            }

            if (UpCount == 0)
            {
                if (strFormClosingMsg == true)
                {
                    for (int i = 0; i < fpSpread2.Sheets[0].Rows.Count; i++)
                    {
                        if (fpSpread2.Sheets[0].RowHeader.Cells[i, 0].Text == "I" || fpSpread2.Sheets[0].RowHeader.Cells[i, 0].Text == "U")
                            UpCount++;
                    }

                    if (UpCount > 0)
                    {
                        DialogResult Rtn = MessageBox.Show(SystemBase.Base.MessageRtn("B0016"), "Confirm", MessageBoxButtons.OKCancel);
                        if (Rtn != DialogResult.OK)
                            e.Cancel = true;
                    }
                }
            }
        }
        #endregion

        #region 그리드 버튼 클릭 이벤트
        //spread1
        private void fpSpread1_ButtonClicked(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            //검사의뢰번호
            if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "검사의뢰번호_2"))
            {
                try
                {
                    string InspDtFr = DateTime.Now.AddDays(-7).ToShortDateString();
                    string InspDtTo = DateTime.Now.ToShortDateString();

                    WNDW009 pu = new WNDW009(txtSPlantCd.Text, fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "검사의뢰번호")].Text, "", "", InspDtFr, InspDtTo);
                    pu.MaximizeBox = false;
                    pu.ShowDialog();
                    if (pu.DialogResult == DialogResult.OK)
                    {
                        string[] Msgs = pu.ReturnVal;

                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "검사의뢰번호")].Text = Msgs[1].ToString();
                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "검사유형")].Text = Msgs[2].ToString();
                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "검사유형코드")].Text = Msgs[34].ToString();
                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text = Msgs[13].ToString();
                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트명")].Text = Msgs[14].ToString();
                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Text = Msgs[15].ToString();
                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "제조오더번호")].Text = Msgs[28].ToString();
                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "공정순서")].Text = Msgs[29].ToString();
                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "입고번호")].Text = Msgs[24].ToString();
                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "입고순번")].Text = Msgs[25].ToString();
                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text = Msgs[3].ToString();
                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목명")].Text = Msgs[4].ToString();
                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "작업코드")].Text = Msgs[35].ToString();
                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "작업명")].Text = Msgs[10].ToString();
                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "작업장")].Text = Msgs[11].ToString();
                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "규격")].Text = Msgs[5].ToString();
                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "오더수량")].Text = Msgs[18].ToString();
                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "오더단위")].Text = Msgs[19].ToString();
                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "사업")].Text = Msgs[36].ToString();
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "검사의뢰번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            // 공정조회
            else if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "작업코드_2"))
            {
                string strQuery = " usp_P_COMMON 'P243' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM", "@pETC", "@pLANG_CD" };
                string[] strSearch = new string[] { "", "", "P001", SystemBase.Base.gstrLangCd };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("WB5101", strQuery, strWhere, strSearch, new int[] { 0, 1 });
                pu.Width = 500;
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "작업코드")].Text = Msgs[0].ToString(); //공정코드
                    fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "작업명")].Value = Msgs[1].ToString(); //공정명

                    UIForm.FPMake.fpChange(fpSpread1, e.Row);
                }
            }
            else if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호_2"))
            {
                //프로젝트번호
                try
                {
                    WNDW003 pu = new WNDW003(fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text, "S1");
                    pu.ShowDialog();
                    if (pu.DialogResult == DialogResult.OK)
                    {
                        string[] Msgs = pu.ReturnVal;

                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text = Msgs[3].ToString();
                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트명")].Text = Msgs[4].ToString();
                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Text = Msgs[5].ToString();
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "프로젝트 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드_2"))
            {
                try
                {
                    WNDW005 pu = new WNDW005(txtItemCd.Text, "");
                    pu.ShowDialog();
                    if (pu.DialogResult == DialogResult.OK)
                    {
                        string[] Msgs = pu.ReturnVal;

                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text = Msgs[2].ToString();
                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목명")].Text = Msgs[3].ToString();
                    }

                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목코드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        //spread2
        private void fpSpread2_ButtonClicked(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx2, "항목_2"))
            {
                try
                {
                    string strQuery = " usp_P_COMMON 'P613' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                    string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };
                    string[] strSearch = new string[] { fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "항목")].Text, "" };

                    UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00072", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "비가동항목조회");	//비가동항목 조회
                    pu.ShowDialog();

                    if (pu.DialogResult == DialogResult.OK)
                    {
                        Regex rx1 = new Regex("#");
                        string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                        fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "항목")].Text = Msgs[0].ToString();
                        fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "항목명")].Text = Msgs[1].ToString();
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "항목 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        #endregion

        #region 그리드 Change이벤트
        protected override void fpSpread1_ChangeEvent(int Row, int Column)
        {
            if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "실동시간(분)"))
            {
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "작업시간(분)")].Value
                    = Convert.ToInt32(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "실동시간(분)")].Value)
                    + Convert.ToInt32(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "준비시간(분)")].Value);
            }
            else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "준비시간(분)"))
            {
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "작업시간(분)")].Value
                    = Convert.ToInt32(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "실동시간(분)")].Value)
                    + Convert.ToInt32(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "준비시간(분)")].Value);
            }
            else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드"))
            {
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목명")].Text
                    = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");
            }
            else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호"))
            {
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트명")].Text
                    = SystemBase.Base.CodeName("PROJECT_NO", "PROJECT_NM", "UVW_S_PROJECT_INFO", fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");

                if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트명")].Text == "")
                {
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Text = "";
                }
            }
            else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "작업코드"))
            {
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "작업명")].Text
                    = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "작업코드")].Text, " AND MAJOR_CD = 'P001'  AND COMP_CODE='" + SystemBase.Base.gstrCOMCD + "' ");
            }
        }
        #endregion

        #region 그리드 Cell 클릭이벤트
        private void fpSpread1_CellClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            if (fpSpread1.Sheets[0].Rows.Count > 0)
            {
                SubData(e.Row);
            }
        }
        #endregion

        #region 작업일보 Master 상세정보
        private void MasterData(string strWorkDayNo)
        {
            string Query = "usp_PEA012 @pTYPE = 'S2', @pWORK_DAY_NO = '" + strWorkDayNo + "' ";
            Query += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' "; 

            DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);

            if (dt.Rows.Count > 0)
            {
                txtWorkDayNo.Value = dt.Rows[0]["WORK_DAY_NO"].ToString();
                txtWorkDutyId.Value = dt.Rows[0]["WORK_DUTY"].ToString();
                txtWc_Cd.Value = dt.Rows[0]["WC_CD"].ToString();
                dtpWorkDt.Value = dt.Rows[0]["WORK_DT"].ToString();
                txtWorkDutyId.Tag = ";2;;";
                txtWorkDutyId.BackColor = SystemBase.Validation.Kind_Gainsboro;
                txtWorkDutyId.ReadOnly = true;
                btnWorkDuty.Enabled = false;


                //직접소계, 시수합계
                int WorkTm = 0;

                for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                {
                    WorkTm = WorkTm + Convert.ToInt32(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "작업시간(분)")].Value);
                }

                txtDirectSum.Value = WorkTm.ToString();

            }
            else
            {
                SystemBase.Validation.GroupBox_Reset(groupBox2);
            }

        }
        #endregion

        #region 작업일보 Detail 상세정보
        private void SubData(int Row)
        {
            txtProjectNo.Value = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text;
            txtProjectNm.Value = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트명")].Text;
            txtItemCd.Value = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text;
            txtItemSpec.Value = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "규격")].Text;
            txtOrderQty.Value = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "오더수량")].Text;
            txtOrderUnit.Value = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "오더단위")].Text;
            txtEntCd.Value = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "사업")].Text;
            txtJobCd.Value = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "작업코드")].Text;
            txtWcCd.Value = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "작업장")].Text;
        }
        #endregion

        #region 삭제Row Count 체크
        private bool DelCheck()
        {
            bool delChk = true;
            int delCount = 0;

            for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
            {
                if (fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text == "D")
                {
                    delCount++;
                }
            }

            if (delCount == fpSpread1.Sheets[0].Rows.Count)
            { delChk = false; }

            return delChk;
        }
        #endregion

        #region 그리드2 체인지 이벤트
        protected override void fpSpread2_ChangeEvent(int Row, int Column)
        {
            if (Column == SystemBase.Base.GridHeadIndex(GHIdx2, "항목"))
            {
                fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "항목명")].Text
                    = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "항목")].Text, " AND MAJOR_CD = 'P025' AND MINOR_CD <> 'Z01'  AND COMP_CODE='" + SystemBase.Base.gstrCOMCD + "' ");
            }
        }
        #endregion

        #region 비가동 시수 합계 변경시 이벤트
        private void txtInDirectSum_TextChanged(object sender, System.EventArgs e)
        {
            int InDirectSum = 0;
            int DirectSum = 0;

            if (txtInDirectSum.Text != "")
            {
                InDirectSum = Convert.ToInt32(txtInDirectSum.Text);
            }
            if (txtDirectSum.Text != "")
            {
                DirectSum = Convert.ToInt32(txtDirectSum.Text);
            }

            txtTotalSum.Value = Convert.ToString(InDirectSum + DirectSum);
        }
        #endregion

        #region 직접시수 합계 변경시 이벤트
        private void txtDirectSum_TextChanged(object sender, System.EventArgs e)
        {
            int InDirectSum = 0;
            int DirectSum = 0;

            if (txtInDirectSum.Text != "")
            {
                InDirectSum = Convert.ToInt32(txtInDirectSum.Text);
            }
            if (txtDirectSum.Text != "")
            {
                DirectSum = Convert.ToInt32(txtDirectSum.Text);
            }

            txtTotalSum.Value = Convert.ToString(InDirectSum + DirectSum);
        }
        #endregion

        #region 그리드1 체인지 이벤트
        private void fpSpread1_EditChange(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            try
            {
                int Column = e.Column;
                int Row = e.Row;
                if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "실동시간(분)"))
                {
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "작업시간(분)")].Value
                        = Convert.ToInt32(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "실동시간(분)")].Value)
                        + Convert.ToInt32(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "준비시간(분)")].Value);
                }
                else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "준비시간(분)"))
                {
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "작업시간(분)")].Value
                        = Convert.ToInt32(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "실동시간(분)")].Value)
                        + Convert.ToInt32(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "준비시간(분)")].Value);
                }
                else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드"))
                {
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목명")].Text
                        = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");
                }
                else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호"))
                {
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트명")].Text
                        = SystemBase.Base.CodeName("PROJECT_NO", "PROJECT_NM", "UVW_S_PROJECT_INFO", fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");

                    if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트명")].Text == "")
                    {
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Text = "";
                    }
                }
                else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "작업코드"))
                {
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "작업명")].Text
                        = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "작업코드")].Text, " AND MAJOR_CD = 'P001'  AND COMP_CODE='" + SystemBase.Base.gstrCOMCD + "' ");
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(f.ToString());
            }
        }
        #endregion

        #region 품질 구분 변경시 필수 변경
        private void fpSpread1_ComboSelChange(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            for (int i = 2; i < fpSpread1.Sheets[0].Columns.Count; i++)
            {
                fpSpread1.Sheets[0].Cells[e.Row, i].Text = "";
            }

            fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "실동시간(분)")].Text = "0";
            fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "준비시간(분)")].Text = "0";
            fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "작업시간(분)")].Text = "0";


            if (fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "구분")].Value.ToString() == "A")
            {
                UIForm.FPMake.grdReMake(fpSpread1, e.Row,
                    SystemBase.Base.GridHeadIndex(GHIdx1, "검사의뢰번호") + "|1"
                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "검사의뢰번호_2") + "|0"
                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호") + "|3"
                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호_2") + "|3"
                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드") + "|3"
                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드_2") + "|3"
                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "작업코드") + "|3"
                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "작업코드_2") + "|3"
                    );
            }
            else
            {
                UIForm.FPMake.grdReMake(fpSpread1, e.Row,
                    SystemBase.Base.GridHeadIndex(GHIdx1, "검사의뢰번호") + "|3"
                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "검사의뢰번호_2") + "|3"
                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호") + "|1"
                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호_2") + "|0"
                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드") + "|1"
                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드_2") + "|0"
                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "작업코드") + "|1"
                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "작업코드_2") + "|0"
                    );
            }
        }
        #endregion
	
    }
}
