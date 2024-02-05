#region 작성정보
/*********************************************************************/
// 단위업무명 : 개발작업일보등록
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-03-25
// 작성내용 : 개발작업일보등록 및 관리
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

namespace PE.PEA004
{
    public partial class PEA004 : UIForm.FPCOMM2_2
    {
        #region 변수선언
        bool chk = true;
        #endregion

        #region 생성자
        public PEA004()
        {
            InitializeComponent();
        }
        #endregion

        #region 폼로드 이벤트
        private void PEA004_Load(object sender, System.EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(groupBox1);//필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox2);//필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox3);//필수체크
            SystemBase.Validation.GroupBox_Setting(GridCommGroupBox2);//필수체크

            txtSPlantCd.Text = SystemBase.Base.gstrPLANT_CD; //로그인ID 소속 공장
            txtWc_Cd.Text = "R021";	//작업장
            dtpWorkDt.Text = SystemBase.Base.ServerTime("YYMMDD").Substring(0, 10);
            //그리드 콤보박스 세팅
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "완료여부")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'B029', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
            

            //string Query = " usp_PEA004 @pTYPE = 'S3' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'"; //간접시수(비가동코드) default 세팅
            //UIForm.FPMake.grdCommSheet(fpSpread2, Query, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, false, 0, 0);

            //for (int i = 0; i < fpSpread2.Sheets[0].Rows.Count; i++)
            //{
            //    fpSpread2.Sheets[0].RowHeader.Cells[i, 0].Text = "I";
            //}

            //UIForm.FPMake.grdReMake(fpSpread2, "1|1#2|1");
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

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);

            //string Query = " usp_PEA004 @pTYPE = 'S3' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'"; //간접시수(비가동코드) default 세팅
            //UIForm.FPMake.grdCommSheet(fpSpread2, Query, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, false, 0, 0);

            //for (int i = 0; i < fpSpread2.Sheets[0].Rows.Count; i++)
            //{
            //    fpSpread2.Sheets[0].RowHeader.Cells[i, 0].Text = "I";
            //}

            //UIForm.FPMake.grdReMake(fpSpread2, "1|1#2|1");

            txtSPlantCd.Value = SystemBase.Base.gstrPLANT_CD;
            txtWc_Cd.Value = "R021";	//작업장

            dtpWorkDt.Tag = ";1;;";
            dtpWorkDt.Enabled = true;
            dtpWorkDt.Value = SystemBase.Base.ServerTime("YYMMDD").Substring(0,10);
            txtWc_Cd.Tag = ";1;;";
            txtWc_Cd.BackColor = SystemBase.Validation.Kind_LightCyan;
            txtWc_Cd.ReadOnly = false;
            btnWc.Enabled = true;
            txtWorkDutyId.Tag = ";1;;";
            txtWorkDutyId.BackColor = SystemBase.Validation.Kind_LightCyan;
            txtWorkDutyId.ReadOnly = false;
            btnWorkDuty.Enabled = true;
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

                        fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "실동시간(분)")].Value = 0;
                        fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "완료여부")].Value = "N";
                        fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "설비가동시간(분)")].Value = 0;
                        fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "작업시간(분)")].Value = 0;
                        fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "공정실적율(%)")].Value = 0;
                        fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "양품수량")].Value = 0;
                        fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "불량수량")].Value = 0;
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
                    string strSql = " usp_PEA004  'D1'";
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

                    string strSql = " usp_PEA004 '" + strMType + "'";
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

                    if (strMType == "I1")
                    { txtWorkDayNo.Value = ds.Tables[0].Rows[0][2].ToString(); }

                    if (SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", false)) //그리드 필수체크
                    {
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

                                fcsStr = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "제조오더번호")].Text;

                                string strDSql = " usp_PEA004 '" + strGbn + "'";
                                strDSql += ", @pWORK_DAY_NO = '" + txtWorkDayNo.Text + "' ";
                                strDSql += ", @pWORK_DAY_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "SEQ")].Value + "' ";
                                strDSql += ", @pPLANT_CD = '" + txtSPlantCd.Text + "' ";
                                strDSql += ", @pWORKORDER_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "제조오더번호")].Text + "' ";
                                strDSql += ", @pITEM_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목")].Text + "' ";
                                strDSql += ", @pPROJECT_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text + "' ";
                                strDSql += ", @pPROJECT_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Text + "' ";
                                strDSql += ", @pPROC_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "공정")].Text + "' ";
                                strDSql += ", @pJOB_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "작업")].Text.Trim() + "' ";
                                strDSql += ", @pWC_CD = '" + txtWc_Cd.Text + "' ";
                                strDSql += ", @pRES_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "설비자원")].Text + "' ";
                                if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "오더수량")].Text != "")
                                { strDSql += ", @pORDER_QTY = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "오더수량")].Value + "' "; }
                                strDSql += ", @pORDER_UNIT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "오더단위")].Text + "' ";
                                if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "설비가동시간(분)")].Text != "")
                                { strDSql += ", @pRUN_TIME = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "설비가동시간(분)")].Value + "' "; }
                                if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "실동시간(분)")].Text != "")
                                { strDSql += ", @pWORK_TIME = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "실동시간(분)")].Value + "' "; }
                                if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "작업시간(분)")].Text != "")
                                { strDSql += ", @pWORK_HUMANTIME = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "작업시간(분)")].Value + "' "; }
                                if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "양품수량")].Text != "")
                                { strDSql += ", @pWORK_GOOD_QTY = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "양품수량")].Value + "' "; }
                                if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "불량수량")].Text != "")
                                { strDSql += ", @pWORK_BAD_QTY = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "불량수량")].Value + "' "; }

                                strDSql += ", @pWORK_DUTY = '" + txtWorkDutyId.Text + "' ";
                                strDSql += ", @pWORK_DT = '" + dtpWorkDt.Text + "' ";
                                strDSql += ", @pDEV_COMPT_YN = '" + Convert.ToString(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "완료여부")].Value) + "' ";

                                strDSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                                strDSql += ", @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

                                DataSet ds1 = SystemBase.DbOpen.TranDataSet(strDSql, dbConn, Trans);

                                ERRCode = ds1.Tables[0].Rows[0][0].ToString();
                                MSGCode = ds1.Tables[0].Rows[0][1].ToString();

                                if (ERRCode != "OK")
                                {
                                    if (strGbn == "I2") { txtWorkDayNo.Value = ""; }
                                    Trans.Rollback();
                                    goto Exit;
                                }	// ER 코드 Return시 점프
                            }
                        }
                    }
                    else
                    {
                        ERRCode = "WR";
                        MSGCode = "P0001";
                        Trans.Rollback();
                        goto Exit;
                    }

                    if (SystemBase.Validation.FPGrid_SaveCheck(fpSpread2, this.Name, "fpSpread2", false)) //그리드 필수체크
                    {
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

                                // 그리드 상단 필수항목 체크
                                fcsStr2 = fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "항목")].Text;

                                string strISql = " usp_PEA004 '" + strGbn + "'";
                                strISql += ", @pWORK_DAY_NO = '" + txtWorkDayNo.Text + "' ";
                                strISql += ", @pINDIRECT_NO = '" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "항목")].Text + "' ";
                                strISql += ", @pINDIRECT_TM = '" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "시수(분)")].Value + "' ";

                                strISql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                                strISql += ", @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

                                DataSet ds2 = SystemBase.DbOpen.TranDataSet(strISql, dbConn, Trans);
                                ERRCode = ds2.Tables[0].Rows[0][0].ToString();
                                MSGCode = ds2.Tables[0].Rows[0][1].ToString();

                                if (ERRCode != "OK")
                                {
                                    if (strGbn == "I3") { txtWorkDayNo.Value = ""; }
                                    Trans.Rollback();
                                    goto Exit;
                                }	// ER 코드 Return시 점프
                            }
                        }
                    }
                    else
                    {
                        ERRCode = "WR";
                        MSGCode = "P0001";
                        Trans.Rollback();
                        goto Exit;
                    }

                    Trans.Commit();
                }
                catch (Exception e)
                {
                    SystemBase.Loggers.Log(this.Name, e.ToString());
                    MessageBox.Show(e.ToString());
                    Trans.Rollback();
                    ERRCode = "ER";
                    if (strGbn == "I2" || strGbn == "I3") { txtWorkDayNo.Value = ""; }
                    MSGCode = "P0001";	//에러가 발생되어 데이터 처리가 취소되었습니다.
                }
            Exit:
                dbConn.Close();

            if (ERRCode == "OK")
            {
                MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtSWorkDayNo.Value = txtWorkDayNo.Text;
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
                    string strQuery = " usp_PEA004  @pTYPE = 'S1'";
                    strQuery += ", @pPLANT_CD = '" + txtSPlantCd.Text + "' ";
                    strQuery += ", @pWORK_DAY_NO = '" + txtSWorkDayNo.Text + "' ";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);

                    SystemBase.Validation.GroupBox_Reset(groupBox3);

                    if (fpSpread1.Sheets[0].Rows.Count > 0)
                    {
                        chk = false;

                        SubData(0);
                    }

                    MasterData(txtSWorkDayNo.Text);
                    SubSearch(txtSWorkDayNo.Text);

                    if (fpSpread1.Sheets[0].Rows.Count == 0 && fpSpread2.Sheets[0].Rows.Count == 0)
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
                string strQuery = " usp_PEA004  @pTYPE = 'S4'";
                strQuery += ", @pWORK_DAY_NO = '" + strWorkDayNo + "' ";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread2, strQuery, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, false, 0, 0);

                //간접소계
                int InWorkTm = 0;

                for (int i = 0; i < fpSpread2.Sheets[0].Rows.Count; i++)
                {
                    InWorkTm = InWorkTm + Convert.ToInt32(fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "시수(분)")].Value);
                }

                txtInDirectSum.Value = InWorkTm.ToString();
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
                WNDW.WNDW025 pu = new WNDW.WNDW025(txtSWorkDayNo.Text, "2" );
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
                string strQuery = " usp_P_COMMON @pTYPE = 'P042', @pLANG_CD = 'KOR', @pETC = 'P002' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";					// 쿼리
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
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "부서 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 폼 닫을때 변경된 데이터 체크
        private void PEA004_Closing(object sender, System.ComponentModel.CancelEventArgs e)
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
            //제조오더번호
            if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "제조오더번호_2"))
            {
                try
                {
                    string WoNo = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "제조오더번호")].Text;
                    PEA004P2 pu = new PEA004P2(WoNo);
                    pu.ShowDialog();
                    if (pu.DialogResult == DialogResult.OK)
                    {
                        string[] Msgs = pu.ReturnVal;

                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "제조오더번호")].Text = Msgs[1].ToString();

                        //상세정보 숨김필드
                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text = Msgs[3].ToString();
                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트명")].Text = Msgs[4].ToString();
                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Text = Msgs[5].ToString();
                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목")].Text = Msgs[6].ToString();
                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "규격")].Text = Msgs[8].ToString();
                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "오더수량")].Text = Msgs[9].ToString();
                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "오더단위")].Text = Msgs[10].ToString();
                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "사업")].Text = Msgs[2].ToString();

                        //상세정보
                        txtProjectNo.Value = Msgs[3].ToString();
                        txtProjectNm.Value = Msgs[4].ToString();
                        txtItemCd.Value = Msgs[6].ToString();
                        txtItemSpec.Value = Msgs[8].ToString();
                        txtOrderQty.Value = Msgs[9].ToString();
                        txtOrderUnit.Value = Msgs[10].ToString();
                        txtEntCd.Value = Msgs[2].ToString();
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "제조오더번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            //공정
            if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "공정_2"))
            {
                if (fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "제조오더번호")].Text != "")
                {
                    try
                    {
                        string strQuery = " usp_PEA001 'P1'";
                        strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' "; 

                        string[] strWhere = new string[] { "@pWORKORDER_NO", "" };
                        string[] strSearch = new string[] { fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "제조오더번호")].Text, "" };

                        UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00073", strQuery, strWhere, strSearch, new int[] { 7 }, "작업지시 공정조회");	//공정조회
                        pu.Width = 700;
                        pu.ShowDialog();

                        if (pu.DialogResult == DialogResult.OK)
                        {
                            Regex rx1 = new Regex("#");
                            string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                            fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "공정")].Text = Msgs[0].ToString();
                            fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "공정명")].Text = Msgs[2].ToString();

                            //작업,작업장 숨김필드
                            fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "작업")].Text = Msgs[1].ToString();
                            fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "작업장")].Text = txtWc_Cd.Text;

                            //상세정보
                            txtJobCd.Value = Msgs[1].ToString();
                            txtWcCd.Value = Msgs[8].ToString();

                            string Query = "";
                            Query = "SELECT CD_NM FROM B_COMM_CODE(NOLOCK) WHERE MAJOR_CD = 'P015' AND MINOR_CD = (SELECT TOP 1 DIRECT_FLAG FROM S_DPLAN_SCH(NOLOCK) ";
                            Query = Query + " WHERE WORKORDER_NO = '" + fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "제조오더번호")].Text + "' ";
                            Query = Query + " AND PROC_SEQ = '" + fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "공정")].Text + "' )";
                            Query = Query + " AND COMP_CODE = '" + SystemBase.Base.gstrCOMCD + "' "; 

                            DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);

                            fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "직/간구분")].Text = dt.Rows[0][0].ToString();
                        }
                    }
                    catch (Exception f)
                    {
                        SystemBase.Loggers.Log(this.Name, f.ToString());
                        MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "공정순서 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0049", "제조오더번호"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning); //제조오더번호를 확인하십시오.
                }
            }
            //설비자원
            if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "설비자원_2"))
            {
                try
                {
                    string strQuery = " usp_P_COMMON 'P055', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                    string[] strWhere = new string[] { "@pCOM_CD", "" };
                    string[] strSearch = new string[] { fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "설비자원")].Text, "" };

                    UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P05006", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "설비자원조회");	//설비자원조회
                    pu.ShowDialog();

                    if (pu.DialogResult == DialogResult.OK)
                    {
                        Regex rx1 = new Regex("#");
                        string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "설비자원")].Text = Msgs[0].ToString();
                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "자원명")].Text = Msgs[1].ToString();

                        if (Msgs.ToString() != "")
                        {
                            fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "설비가동시간(분)")].BackColor = Color.LightCyan;
                            fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "설비가동시간(분)")].Locked = false;
                        }
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "설비자원 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    string strQuery = " usp_P_COMMON 'P614' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
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
            if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "제조오더번호"))
            {
                if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "제조오더번호")].Text != "")
                {
                    string strQuery = "usp_PEA004 'S6', @pWORKORDER_NO = '" + fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "제조오더번호")].Text + "' ";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' "; 

                    DataTable WorkDt = SystemBase.DbOpen.NoTranDataTable(strQuery);

                    if (WorkDt.Rows.Count > 0)
                    {
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text = WorkDt.Rows[0]["PROJECT_NO"].ToString();
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트명")].Text = WorkDt.Rows[0]["PROJECT_NM"].ToString();
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Text = WorkDt.Rows[0]["PROJECT_SEQ"].ToString();
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목")].Text = WorkDt.Rows[0]["ITEM_CD"].ToString();
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "규격")].Text = WorkDt.Rows[0]["ITEM_SPEC"].ToString();
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "오더수량")].Text = WorkDt.Rows[0]["PRODT_ORDER_QTY"].ToString();
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "오더단위")].Text = WorkDt.Rows[0]["PRODT_ORDER_UNIT"].ToString();
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "사업")].Text = WorkDt.Rows[0]["ENT_CD"].ToString();

                    }
                    else
                    {

                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text = "";
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트명")].Text = "";
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Text = "";
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목")].Text = "";
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "규격")].Text = "";
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "오더수량")].Text = "";
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "오더단위")].Text = "";
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "사업")].Text = "";
                    }
                }
            }
            else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "공정"))
            {
                if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "제조오더번호")].Text != "")
                {
                    if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "")].Text != "공정")
                    {
                        string ProcQuery = "usp_P_COMMON 'P110', @pCOM_CD = '" + fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "제조오더번호")].Text + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                        DataTable ProcDt = SystemBase.DbOpen.NoTranDataTable(ProcQuery);

                        if (ProcDt.Rows.Count > 0)
                        {
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "작업")].Text = ProcDt.Rows[0]["JOB_CD"].ToString();
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "작업장")].Text = ProcDt.Rows[0]["WC_CD"].ToString();

                            string Query = "";
                            Query = "SELECT CD_NM FROM B_COMM_CODE(NOLOCK) WHERE MAJOR_CD = 'P015' AND MINOR_CD = (SELECT TOP 1 DIRECT_FLAG FROM S_DPLAN_SCH(NOLOCK) ";
                            Query = Query + " WHERE WORKORDER_NO = '" + fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "제조오더번호")].Text + "' ";
                            Query = Query + " AND PROC_SEQ = '" + fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "공정")].Text + "' ";

                            DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);

                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "직/간구분")].Text = dt.Rows[0][0].ToString();
                        }
                        else
                        {
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "작업")].Text = "";
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "작업장")].Text = "";

                            MessageBox.Show(SystemBase.Base.MessageRtn("B0049", "공정"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning); //공정를 확인하십시오.
                        }
                    }
                }
                else
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0049", "제조오더번호"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning); //제조오더번호를 확인하십시오.
                }
            }
            else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "설비자원"))
            {
                if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "설비자원")].Text == "")
                {
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "자원명")].Text = "";
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "설비가동시간(분)")].Value = 0;
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "설비가동시간(분)")].BackColor = Color.Gainsboro;
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "설비가동시간(분)")].Locked = true;
                }
                else
                {
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "자원명")].Text =
                        SystemBase.Base.CodeName("RES_CD", "RES_DIS", "P_RESO_MANAGE", fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "설비자원")].Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");

                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "설비가동시간(분)")].BackColor = Color.LightCyan;
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "설비가동시간(분)")].Locked = false;
                }
            }
            else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "실동시간(분)"))
            {
                if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "설비자원")].Text != "")
                {
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "설비가동시간(분)")].Value
                        = Convert.ToInt32(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "실동시간(분)")].Value);
                }

                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "작업시간(분)")].Value
                    = Convert.ToInt32(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "실동시간(분)")].Value);
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
            string Query = "usp_PEA004 @pTYPE = 'S2', @pWORK_DAY_NO = '" + strWorkDayNo + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

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
            txtItemCd.Value = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목")].Text;
            txtItemSpec.Value = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "규격")].Text;
            txtOrderQty.Value = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "오더수량")].Text;
            txtOrderUnit.Value = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "오더단위")].Text;
            txtEntCd.Value = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "사업")].Text;
            txtJobCd.Value = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "작업")].Text;
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

        #region touch조회
        private void TouchSearch(string ResCd, string WorkDt)
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            try
            {
                string strQuery = " usp_PEA004  @pTYPE = 'S5'";
                strQuery += ", @pWORK_DUTY = '" + ResCd + "' ";
                strQuery += ", @pWORK_DT = '" + WorkDt + "' ";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' "; 

                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0);

                if (fpSpread1.Sheets[0].Rows.Count > 0)
                {
                    for (int j = 0; j < fpSpread1.Sheets[0].Rows.Count; j++)
                    {
                        fpSpread1.Sheets[0].RowHeader.Cells[j, 0].Text = "I";
                    }

                    SubData(0);

                    fpSpread2.Sheets[0].ActiveRowIndex = fpSpread2.Sheets[0].Rows.Count - 1;

                    int StopTm = 0;
                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {
                        StopTm = StopTm + Convert.ToInt32(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "보류시수")].Value);
                    }

                    if (StopTm > 0)
                    {
                        UIForm.FPMake.RowInsert(fpSpread2);
                        fpSpread2.Sheets[0].RowHeader.Cells[fpSpread2.Sheets[0].Rows.Count - 1, 0].Text = "I";
                        fpSpread2.Sheets[0].Cells[fpSpread2.Sheets[0].Rows.Count - 1, SystemBase.Base.GridHeadIndex(GHIdx2, "항목")].Text = "Z01";
                        fpSpread2.Sheets[0].Cells[fpSpread2.Sheets[0].Rows.Count - 1, SystemBase.Base.GridHeadIndex(GHIdx2, "항목명")].Text = "기타";
                        fpSpread2.Sheets[0].Cells[fpSpread2.Sheets[0].Rows.Count - 1, SystemBase.Base.GridHeadIndex(GHIdx2, "시수(분)")].Value = StopTm;
                    }

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

        #region 그리드2 체인지 이벤트
        protected override void fpSpread2_ChangeEvent(int Row, int Column)
        {
            if (Column == SystemBase.Base.GridHeadIndex(GHIdx2, "항목"))
            {
                fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "항목명")].Text
                    = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "항목")].Text, " AND MAJOR_CD = 'P025' AND MINOR_CD NOT IN ('Z01','B11') AND REL_CD3 = 'Y' AND COMP_CODE='" + SystemBase.Base.gstrCOMCD + "' ");
            }
        }
        #endregion

        #region 날짜 변경시 touch 데이터 검색
        private void dtpWorkDt_CloseUp(object sender, System.EventArgs e)
        {
            if (chk == true)
            {
                TouchSearch(txtWorkDutyId.Text, dtpWorkDt.Text);
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

        #region 그리드1 변경시
        private void fpSpread1_EditChange(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            try
            {
                int Column = e.Column;
                int Row = e.Row;
                if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "제조오더번호"))
                {
                    if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "제조오더번호")].Text != "")
                    {
                        string strQuery = "usp_PEA004 'S6', @pWORKORDER_NO = '" + fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "제조오더번호")].Text + "' ";
                        strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' "; 

                        DataTable WorkDt = SystemBase.DbOpen.NoTranDataTable(strQuery);

                        if (WorkDt.Rows.Count > 0)
                        {
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text = WorkDt.Rows[0]["PROJECT_NO"].ToString();
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트명")].Text = WorkDt.Rows[0]["PROJECT_NM"].ToString();
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Text = WorkDt.Rows[0]["PROJECT_SEQ"].ToString();
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목")].Text = WorkDt.Rows[0]["ITEM_CD"].ToString();
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "규격")].Text = WorkDt.Rows[0]["ITEM_SPEC"].ToString();
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "오더수량")].Text = WorkDt.Rows[0]["PRODT_ORDER_QTY"].ToString();
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "오더단위")].Text = WorkDt.Rows[0]["PRODT_ORDER_UNIT"].ToString();
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "사업")].Text = WorkDt.Rows[0]["ENT_CD"].ToString();

                       }
                        else
                        {
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text = "";
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트명")].Text = "";
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Text = "";
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목")].Text = "";
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "규격")].Text = "";
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "오더수량")].Text = "";
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "오더단위")].Text = "";
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "사업")].Text = "";
                        }
                    }
                }
                else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "공정"))
                {
                    if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "제조오더번호")].Text != "")
                    {
                        if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "")].Text != "공정")
                        {
                            string ProcQuery = "usp_P_COMMON 'P110', @pCOM_CD = '" + fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "제조오더번호")].Text + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                            DataTable ProcDt = SystemBase.DbOpen.NoTranDataTable(ProcQuery);

                            if (ProcDt.Rows.Count > 0)
                            {
                                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "작업")].Text = ProcDt.Rows[0]["JOB_CD"].ToString();
                                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "작업장")].Text = ProcDt.Rows[0]["WC_CD"].ToString();
                            }
                            else
                            {
                                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "작업")].Text = "";
                                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "작업장")].Text = "";

                                MessageBox.Show(SystemBase.Base.MessageRtn("B0049", "공정"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning); //공정를 확인하십시오.
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show(SystemBase.Base.MessageRtn("B0049", "제조오더번호"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning); //제조오더번호를 확인하십시오.
                    }
                }
                else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "설비자원"))
                {
                    if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "설비자원")].Text == "")
                    {
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "자원명")].Text = "";
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "설비가동시간(분)")].Value = 0;
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "설비가동시간(분)")].BackColor = Color.Gainsboro;
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "설비가동시간(분)")].Locked = true;
                    }
                    else
                    {
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "자원명")].Text =
                            SystemBase.Base.CodeName("RES_CD", "RES_DIS", "P_RESO_MANAGE", fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "설비자원")].Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");

                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "설비가동시간(분)")].BackColor = Color.LightCyan;
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "설비가동시간(분)")].Locked = false;
                    }
                }
                else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "실동시간(분)"))
                {
                    if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "설비자원")].Text != "")
                    {
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "설비가동시간(분)")].Value
                            = Convert.ToInt32(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "실동시간(분)")].Value);
                    }

                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "작업시간(분)")].Value
                        = Convert.ToInt32(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "실동시간(분)")].Value);
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(f.ToString());
            }
        }
        #endregion
    }
}
