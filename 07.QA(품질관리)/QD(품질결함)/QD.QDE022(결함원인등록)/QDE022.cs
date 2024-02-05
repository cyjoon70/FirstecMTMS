
#region 작성정보
/*********************************************************************/
// 단위업무명 : 결함원인등록
// 작 성 자 : 권순철
// 작 성 일 : 2013-04-02
// 작성내용 : 결함원인등록 및 관리
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

namespace QD.QDE022
{
    public partial class QDE022 : UIForm.FPCOMM1
    {
        #region 변수선언
        string strInspReqNo = "";
        #endregion

        #region 생성자
        public QDE022()
        {
            InitializeComponent();
        }

        public QDE022(string InspReqNo)
        {
            InitializeComponent();
            strInspReqNo = InspReqNo;
        }
        #endregion

        #region Form Load 시
        private void QDE022_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            //콤보박스 세팅
            SystemBase.ComboMake.C1Combo(cboPlantCd, "usp_B_COMMON @pType='TABLE', @pCODE = 'PLANT_CD', @pNAME = 'PLANT_NM', @pSPEC1 = 'B_PLANT_INFO', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 0);//공장

            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "검사관리자")] = SystemBase.ComboMake.ComboOnGrid("P_RESO_MANAGE @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'P033', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
            //그리드초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

            //기타 세팅
            cboPlantCd.SelectedValue = SystemBase.Base.gstrPLANT_CD;
            dtQNCNO_FR.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-3);
            dtQNCNO_TO.Value = SystemBase.Base.ServerTime("YYMMDD");
            if (strInspReqNo != "")
            {
                SearchExec();
            }
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            fpSpread1.Sheets[0].Rows.Count = 0;

            //기타 세팅
            cboPlantCd.SelectedValue = SystemBase.Base.gstrPLANT_CD;
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
                    string DacuYn = "";
                    string EXM_YN = "";
                    string strQuery = " usp_QDE022  @pTYPE = 'S1'";
                    strQuery += ", @pQNC_NO = '" + txtQNC_NO.Text + "' ";
                    strQuery += ", @pQNCNO_DT_FR = '" + dtQNCNO_FR.Text + "' ";
                    strQuery += ", @pQNCNO_DT_TO = '" + dtQNCNO_TO.Text + "' ";

                    strQuery += ", @pITEM_CD = '" + txtItemCd.Text + "' ";
                    strQuery += ", @pPROJECT_NO = '" + txtProjectNo.Text + "'";
                    strQuery += ", @pPROJECT_SEQ = '" + txtProjectSeq.Text + "'";

                    strQuery += ", @pDEPT_CD = '" + txtDeptCd.Text + "'";//귀책부서

                    strQuery += ", @pPLANT_CD = '" + cboPlantCd.SelectedValue.ToString() + "' ";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    if (rdoDcauEntYnAll.Checked == true)
                        DacuYn = "";
                    else
                        DacuYn = "N";

                    strQuery += ", @pDCAU_ENT_YN = '" + DacuYn + "' ";

                    if (rdoEXAMINATION_ALL.Checked == true)
                        EXM_YN = "";
                    else if (rdoEXAMINATION_Y.Checked == true)
                        EXM_YN = "Y";
                    else if (rdoEXAMINATION_N.Checked == true)
                        EXM_YN = "N";


                    strQuery += ", @pEXAMINATION_YN = '" + EXM_YN + "' ";
                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 1, true);
                    if (fpSpread1.Sheets[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                        {
                            for (int j = 0; j < fpSpread1.Sheets[0].ColumnCount; j++)
                            {
                                if (fpSpread1.Sheets[0].Columns[j].CellType.ToString() == "CheckBoxCellType")
                                {
                                    fpSpread1.Sheets[0].Columns[j].Locked = true;
                                }
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
        }
        #endregion

        #region 조회조건 팝업
        //프로젝트번호

        //프로젝트차수

        //품목코드


        //귀책부서
        #endregion

        #region TextChanged
        //프로젝트번호

        //품목코드
        //귀책부서
        #endregion

        #region 결과원인등록
        //결과원인등록
        private void butUpdate_Click(object sender, EventArgs e)
        {
            if (fpSpread1.Sheets[0].Rows.Count > 0)
            {
                int iRow = fpSpread1.ActiveSheet.ActiveRowIndex;

                QDE022P1 myForm = new QDE022P1(fpSpread1, fpSpread1.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx1, "QNC NO")].Text, fpSpread1.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx1, "처리결과")].Text);

                myForm.ShowDialog();

                if (myForm.DialogResult == DialogResult.OK)
                    SearchExec();

            }
        }
        #endregion

        private void btnProjectNo_Click(object sender, EventArgs e)
        {
            try
            {
                WNDW.WNDW007 pu = new WNDW.WNDW007(txtProjectNo.Text);
                pu.MaximizeBox = false;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtProjectNo.Text = Msgs[3].ToString();
                    txtProjectNm.Value = Msgs[4].ToString();
                    txtProjectSeq.Text = "";
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "프로젝트번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnProjectSeq_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON 'PROJ_SEQ', @pSPEC1 = '" + txtProjectNo.Text + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";										// 쿼리
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };			// 쿼리 인자값(조회조건)
                string[] strSearch = new string[] { "", "" };		// 쿼리 인자값에 들어갈 데이타

                //UIForm.PopUpSP pu = new UIForm.PopUpSP(strQuery, strWhere, strSearch, PHeadText7, PTxtAlign7, PCellType7, PHeadWidth7, PSearchLabel7);
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P09001", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "프로젝트차수 조회", false);
                pu.Width = 400;
                pu.ShowDialog();	//공통 팝업 호출

                if (pu.DialogResult == DialogResult.OK)
                {
                    string MSG = pu.ReturnVal.Replace("|", "#");
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(MSG);
                    txtProjectSeq.Text = Msgs[0].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        private void btnDeptCd_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON 'COMM_POP' ,@pSPEC1='Q026', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtDeptCd.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00093", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "귀책부서 조회");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtDeptCd.Text = Msgs[0].ToString();
                    txtDeptNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnItemCd_Click(object sender, EventArgs e)
        {
            try
            {
                WNDW005 pu = new WNDW005("10");
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


        #region SaveExec() 폼에 입력된 데이타 저장 로직
        protected override void SaveExec()
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;


            if (SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", true) == true) // 그리드 상단 필수항목 체크
            {
                string ERRCode = "ER", MSGCode = "P0000";	//처리할 내용이 없습니다.
                string strKeyCd = "";

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
                                case "D": strGbn = "D1"; break;
                                default: strGbn = ""; break;
                            }
                            string strQncNO = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "QNC NO")].Text.ToString();
                            string strSql = "";

                            strSql = " usp_QDE022 '" + strGbn + "'";
                            strSql = strSql + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                            strSql = strSql + ", @pQNC_NO = '" + strQncNO + "'";

                            DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                            ERRCode = ds.Tables[0].Rows[0][0].ToString();
                            MSGCode = ds.Tables[0].Rows[0][1].ToString();

                            if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
                        }
                    }
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
                    SearchExec();
                    UIForm.FPMake.GridSetFocus(fpSpread1, strKeyCd); //그리드 위치를 가져온다

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
            this.Cursor = System.Windows.Forms.Cursors.Default;
        }
        #endregion

    }
}
