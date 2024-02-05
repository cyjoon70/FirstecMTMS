#region 작성정보
/*********************************************************************/
// 단위업무명 : 검사항목등록(최종)
// 작 성 자 : 김 현 근
// 작 성 일 : 2013-04-01
// 작성내용 : 검사항목등록(공정) 및 관리
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
using System.Threading;
using WNDW;
using System.Text.RegularExpressions;
using System.IO;
using System.Reflection;
using EDocument.Network;
namespace QF.QFA001
{
    public partial class QFA001 : UIForm.FPCOMM2
    {
        #region 변수선언
        string strInspReqNo = "";
        int SearchRow = 0;
        int ShowColumn = 0;
        string strDecisionCd = ""; //검사판정
        string strPlantCd = "";
        string strInspReqDt = "";
        string strInspStatus = "";
        bool Linked = false;
        string FullFileName = "";
        Thread th;
        UIForm.ExcelWaiting Waiting_Form = null;
        #endregion

        #region 생성자
        public QFA001()
        {
            InitializeComponent();
        }

        public QFA001(string param1, string param2, string param3, string param4)
        {
            strInspReqNo = param1;
            strPlantCd = param2;
            strInspReqDt = param3;
            strInspStatus = param4;
            Linked = true;
            
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void QFA001_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            //콤보박스 세팅
            SystemBase.ComboMake.C1Combo(cboSPlantCd, "usp_B_COMMON @pType='TABLE', @pCODE = 'PLANT_CD', @pNAME = 'PLANT_NM', @pSPEC1 = 'B_PLANT_INFO', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//공장
            SystemBase.ComboMake.C1Combo(cboSInspStatus, "usp_B_COMMON @pType='COMM2', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "',@pCODE = 'Q003', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0); //검사진행상태
            SystemBase.ComboMake.C1Combo(cboSDecisionCd, "usp_B_COMMON @pType='COMM', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "',@pCODE = 'Q004', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 9); //판정

            //그리드초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
            UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, 0, 0, false);

            //기타 세팅
            cboSPlantCd.SelectedValue = SystemBase.Base.gstrPLANT_CD;
            dtpSInspReqDtFr.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-3).ToString().Substring(0,10);
            dtpSInspReqDtTo.Value = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0, 10);
            dtpSInspDtFr.Value = null;
            dtpSInspDtTo.Value = null;
            cboSInspStatus.SelectedValue = "Q";

            SystemBase.Validation.GroupBoxControlsLock(groupBox2, true);
            butQInspItemCall.Enabled = false;
            btnInspBase.Enabled = false;
            btnITEM_PICTURE.Enabled = false;

            if (Linked == true)
            {
                cboSPlantCd.SelectedValue = strPlantCd;
                txtSInspReqNo.Text = strInspReqNo;
                dtpSInspReqDtFr.Value = strInspReqDt;
                dtpSInspReqDtTo.Value = strInspReqDt;
                cboSInspStatus.Text = strInspStatus;
                SearchExec();
            }

            lnkJump1.Text = "RELEASE";  //화면에 보여지는 링크명
            strJumpFileName1 = "QF.QFA021.QFA021"; //호출할 화면명

            lnkJump2.Text = "부적합처리";  //화면에 보여지는 링크명
            strJumpFileName2 = "QF.QFA012.QFA012"; //호출할 화면명

            lnkJump3.Text = "검사판정";  //화면에 보여지는 링크명
            strJumpFileName3 = "QF.QFA011.QFA011"; //호출할 화면명

            lnkJump4.Text = "불량유형등록";  //화면에 보여지는 링크명
            strJumpFileName4 = "QF.QFA003.QFA003"; //호출할 화면명

            lnkJump5.Text = "검사내역등록";  //화면에 보여지는 링크명
            strJumpFileName5 = "QF.QFA002.QFA002"; //호출할 화면명
        }
        #endregion
        
        #region Link
        private object[] Params()
        {
            if (txtInspReqNo.Text == "")
                param = null;						// 파라메터를 하나도 넘기지 않을경우
            else
            {
                param = new object[4];					// 파라메터수가 4개인 경우
                param[0] = txtInspReqNo.Text;
                param[1] = cboSPlantCd.SelectedValue.ToString();
                param[2] = dtpInspReqDt.Text;
                param[3] = txtInspStatus.Text;
            }
            return param;
        }

        protected override void Link1Exec()
        {
            param = Params();

            SystemBase.Base.RodeFormID = "QFA021";			// 이동할 formid를 적어준다.(메뉴id)
            SystemBase.Base.RodeFormText = "RELEASE(최종)"; 	// 이동할 폼명을 적어준다(메뉴명)
        }

        protected override void Link2Exec()
        {
            param = Params();

            SystemBase.Base.RodeFormID = "QFA012";			// 이동할 formid를 적어준다.(메뉴id)
            SystemBase.Base.RodeFormText = "부적합처리(최종)"; 	// 이동할 폼명을 적어준다(메뉴명)
        }

        protected override void Link3Exec()
        {
            param = Params();

            SystemBase.Base.RodeFormID = "QFA011";			// 이동할 formid를 적어준다.(메뉴id)
            SystemBase.Base.RodeFormText = "검사판정(최종)"; 	// 이동할 폼명을 적어준다(메뉴명)
        }

        protected override void Link4Exec()
        {
            param = Params();

            SystemBase.Base.RodeFormID = "QFA003";			// 이동할 formid를 적어준다.(메뉴id)
            SystemBase.Base.RodeFormText = "불량유형등록(최종)"; 	// 이동할 폼명을 적어준다(메뉴명)
        }

        protected override void Link5Exec()
        {
            param = Params();

            SystemBase.Base.RodeFormID = "QFA002";			// 이동할 formid를 적어준다.(메뉴id)
            SystemBase.Base.RodeFormText = "검사내역등록(최종)"; 	// 이동할 폼명을 적어준다(메뉴명)
        }
        #endregion

        #region 조회조건 팝업
        //품목코드
        private void btnSItemCd_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW005 pu = new WNDW005(cboSPlantCd.SelectedValue.ToString(), true, txtSItemCd.Text);
                pu.MaximizeBox = false;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtSItemCd.Text = Msgs[2].ToString();
                    txtSItemNm.Value = Msgs[3].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목코드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        //공급처
        private void btnSBpCd_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW002 pu = new WNDW002(txtSBpCd.Text, "");
                pu.MaximizeBox = false;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtSBpCd.Text = Msgs[1].ToString();
                    txtSBpNm.Value = Msgs[2].ToString();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "공급처 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        //프로젝트번호
        private void btnSProjectNo_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW007 pu = new WNDW007(txtSProjectNo.Text);
                pu.MaximizeBox = false;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtSProjectNo.Text = Msgs[3].ToString();
                    txtSProjectNm.Value = Msgs[4].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "프로젝트번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //검사의뢰번호
        private void btnInspReqNo_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW009 pu = new WNDW009(Convert.ToString(cboSPlantCd.SelectedValue)
                    , txtSInspReqNo.Text
                    , "R"
                    , Convert.ToString(cboSInspStatus.SelectedValue));
                pu.MaximizeBox = false;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtSInspReqNo.Text = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "검사의뢰번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //제조오더번호
        private void btnSWorkOrderNo_Click(object sender, EventArgs e)
        {
            try
            {
                WNDW006 pu = new WNDW006(txtSWorkOrderNo.Text);
                pu.MaximizeBox = false;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtSWorkOrderNo.Text = Msgs[1].ToString();
                    txtSWorkOrderNo.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "검사의뢰번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 조회조건 TextChanged
        //품목코드
        private void txtSItemCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtSItemCd.Text != "")
                {
                    txtSItemNm.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtSItemCd.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtSItemNm.Value = "";
                }
            }
            catch
            {

            }
        }

        //공급처
        private void txtSBpCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtSBpCd.Text != "")
                {
                    txtSBpNm.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtSBpCd.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtSBpNm.Value = "";
                }
            }
            catch
            {

            }
        }

        //프로젝트번호
        private void txtSProjectNo_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtSProjectNo.Text != "")
                {
                    txtSProjectNm.Value = SystemBase.Base.CodeName("PROJECT_NO", "PROJECT_NM", "S_SO_MASTER", txtSProjectNo.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtSProjectNm.Value = "";
                }
            }
            catch
            {

            }
        }
        #endregion

        #region NewExec()
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            SystemBase.Validation.GroupBox_Reset(groupBox2);

            fpSpread1.Sheets[0].Rows.Count = 0;
            fpSpread2.Sheets[0].Rows.Count = 0;

            //기타 세팅
            cboSPlantCd.SelectedValue = SystemBase.Base.gstrPLANT_CD;
            dtpSInspReqDtFr.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-3).ToString().Substring(0,10);
            dtpSInspReqDtTo.Value = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0, 10);
            dtpSInspDtFr.Value = null;
            dtpSInspDtTo.Value = null;
            cboSInspStatus.SelectedValue = "Q";

            SystemBase.Validation.GroupBoxControlsLock(groupBox2, true);
            butQInspItemCall.Enabled = false;
            btnInspBase.Enabled = false;
            strInspReqNo = "";
        }
        #endregion

        #region 행추가 버튼 클릭 이벤트
        protected override void RowInsExec()
        {
            try
            {
                UIForm.FPMake.RowInsert(fpSpread1);
                fpSpread1.Sheets[0].ActiveRow.Height = 30;
                fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "검사항목")].Locked = true;
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "행추가"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        #endregion

        #region 행복사 버튼 클릭 이벤트
        protected override void RCopyExec()
        {
            try
            {
                if (fpSpread1.Sheets[0].Rows.Count > 0)
                {
                    UIForm.FPMake.RowCopy(fpSpread1);
                    fpSpread1.Sheets[0].ActiveRow.Height = 30;
                    fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "검사항목")].Locked = true;
                }
                else
                {
                    MessageBox.Show("복사할 데이타가 없습니다.", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "행복사"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            strInspReqNo = "";
            Grid2_Search();
        }
        #endregion

        #region 그리드조회
        private void Grid2_Search()
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {
                    string strQuery = " usp_QFA001  @pTYPE = 'S1'";
                    strQuery += ", @pPLANT_CD = '" + cboSPlantCd.SelectedValue.ToString() + "' ";
                    strQuery += ", @pINSP_REQ_DT_FR = '" + dtpSInspReqDtFr.Text + "' ";
                    strQuery += ", @pINSP_REQ_DT_TO = '" + dtpSInspReqDtTo.Text + "' ";
                    strQuery += ", @pINSP_DT_FR = '" + dtpSInspDtFr.Text + "' ";
                    strQuery += ", @pINSP_DT_TO = '" + dtpSInspDtTo.Text + "' ";
                    strQuery += ", @pITEM_CD = '" + txtSItemCd.Text + "' ";
                    strQuery += ", @pBP_CD = '" + txtSBpCd.Text + "' ";
                    strQuery += ", @pINSP_STATUS = '" + cboSInspStatus.SelectedValue.ToString() + "' ";
                    strQuery += ", @pDECISION_CD = '" + cboSDecisionCd.SelectedValue.ToString() + "'";
                    strQuery += ", @pPROJECT_NO = '" + txtSProjectNo.Text + "'";
                    strQuery += ", @pINSP_REQ_NO = '" + txtSInspReqNo.Text + "'";
                    strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'";
                    strQuery += ", @pWORKORDER_NO = '" + txtSWorkOrderNo.Text + "'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
     
                    UIForm.FPMake.grdCommSheet(fpSpread2, strQuery, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, false, 0, 0, true);

                    if (fpSpread2.Sheets[0].Rows.Count > 0)
                    {
                        fpSpread2.Search(0, strInspReqNo, true, true, true, true, 0, SystemBase.Base.GridHeadIndex(GHIdx2, "검사의뢰번호"), ref SearchRow, ref ShowColumn);

                        if (SearchRow < 0)
                        { SearchRow = 0; }

                        Grid1_Search(SearchRow);

                        btnInspBase.Enabled = true;
                    }
                    else
                    {
                        SystemBase.Validation.GroupBox_Reset(groupBox2);
                        fpSpread1.Sheets[0].Rows.Count = 0;
                        butQInspItemCall.Enabled = false;
                        btnInspBase.Enabled = false;
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

        #region fpSpre2 그리드 선택시 상세정보 조회
        private void fpSpread2_LeaveCell(object sender, FarPoint.Win.Spread.LeaveCellEventArgs e)
        {
            if (fpSpread2.Sheets[0].Rows.Count > 0)
            {
                if (e.Row != e.NewRow)
                {
                    try
                    {
                        SearchRow = e.NewRow;
                        Grid1_Search(SearchRow);
                    }
                    catch (Exception f)
                    {
                        SystemBase.Loggers.Log(this.Name, f.ToString());
                        MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                        //데이터 조회 중 오류가 발생하였습니다.				
                    }

                }
            }
        }
        #endregion

        #region fpSpread1 그리드 조회
        private void Grid1_Search(int iRow)
        {
            this.Cursor = Cursors.WaitCursor;

            SystemBase.Validation.GroupBox_Reset(groupBox2);

            if (fpSpread2.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx2, "통합의뢰번호")].Text.Trim().ToString() == "")
            {
                strInspReqNo = fpSpread2.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx2, "검사의뢰번호")].Text;
            }
            else
            {
                strInspReqNo = fpSpread2.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx2, "통합의뢰번호")].Text;
            }

            strDecisionCd = fpSpread2.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx2, "검사판정")].Text;

            //groupBox2 값입력

            txtInspReqNo.Value = strInspReqNo;
            dtpInspReqDt.Value = fpSpread2.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx2, "검사의뢰일")].Text;
            dtpInspDemandDt.Value = fpSpread2.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx2, "검사요구일")].Text;
            txtItemCd.Value = fpSpread2.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx2, "품목코드")].Text;
            txtItemNm.Value = fpSpread2.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx2, "품명")].Text;
            txtWorkOrderNo.Value = fpSpread2.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx2, "제조오더번호")].Text;
            txtFinInspLvl.Value = fpSpread2.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx2, "최종검사레벨")].Text;
            txtFinInspLvlNm.Value = fpSpread2.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx2, "최종검사레벨명")].Text;
            txtLotSize.Value = Convert.ToDouble(fpSpread2.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx2, "LOT크기")].Value);
            txtStockUnit.Value = fpSpread2.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx2, "단위")].Text;
            txtProjectNo.Value = fpSpread2.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx2, "프로젝트번호")].Text;
            txtProjectNm.Value = fpSpread2.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx2, "프로젝트명")].Text;
            txtInspStatus.Value = fpSpread2.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx2, "진행상태")].Text;
            txtDecisionCd.Value = fpSpread2.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx2, "검사판정명")].Text;
            txtInspQty.Value = Convert.ToDouble(fpSpread2.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx2, "검사수")].Value);
            txtDefectQty.Value = Convert.ToDouble(fpSpread2.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx2, "불량수")].Value);

            try
            {
                string strQuery = " usp_QFA001  @pTYPE = 'S2'";
                strQuery += ", @pINSP_REQ_NO = '" + txtInspReqNo.Text + "'";
                strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
     
                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false, false);

                if (fpSpread1.Sheets[0].Rows.Count > 0)
                {
                    fpSpread1.Sheets[0].Rows[0, fpSpread1.Sheets[0].Rows.Count - 1].Height = 30;
                    //판정여부에 따른 화면 Locking
                    if (strDecisionCd != "N")	//판정
                    {
                        //Detail Locking설정
                        for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                        {
                            UIForm.FPMake.grdReMake(fpSpread1, i,
                                SystemBase.Base.GridHeadIndex(GHIdx1, "시료수") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "합격판정개수") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "불합격판정개수") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "합격판정계수") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "최대허용불량률") + "|3"
                                );
                        }

                        //버튼설정
                        UIForm.Buttons.ReButton("110000001001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);
                    }
                    else
                    {

                        for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                        {
                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "시료번호")].Text != "")
                            {

                                UIForm.FPMake.grdReMake(fpSpread1, i,
                                    SystemBase.Base.GridHeadIndex(GHIdx1, "시료수") + "|3"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "합격판정개수") + "|0"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "불합격판정개수") + "|0"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "합격판정계수") + "|0"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "최대허용불량률") + "|0"
                                    );

                            }
                            else
                            {
                                UIForm.FPMake.grdReMake(fpSpread1, i,
                                    SystemBase.Base.GridHeadIndex(GHIdx1, "시료수") + "|1"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "합격판정개수") + "|0"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "불합격판정개수") + "|0"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "합격판정계수") + "|0"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "최대허용불량률") + "|0"
                                    );
                            }

                        }

                        //버튼설정
                        UIForm.Buttons.ReButton("111111011001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);
                    }

                    butQInspItemCall.Enabled = false;
                }
                else
                {
                    if (strDecisionCd != "N")	//판정
                    {
                        UIForm.Buttons.ReButton("110000001001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);                    
                    }
                    else
                    {
                        UIForm.Buttons.ReButton("111111011001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);                    
                    }

                    butQInspItemCall.Enabled = true;
                }
                strQuery = " usp_QFA002  @pTYPE = 'S4'";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                strQuery += ", @pITEM_CD = '" + txtItemCd.Text + "' ";
                DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);
                if (dt.Rows[0]["FILEEXTENSIONS"].ToString() == "JPG" || dt.Rows[0]["FILEEXTENSIONS"].ToString() == "BMP" || dt.Rows[0]["FILEEXTENSIONS"].ToString() == "GIF")
                {
                    btnITEM_PICTURE.Enabled = true;

                    string FtpFile = "ftp://172.30.24.14/ITEM_IMAGE/";
                    FullFileName = FtpFile + txtItemCd.Text;
                }
                else
                {
                    btnITEM_PICTURE.Enabled = false;
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

        #region SaveExec() 데이타 저장 로직
        protected override void SaveExec()
        {
            fpSpread1.Focus();

            //상단 그룹박스 필수 체크
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox2))
            {

                if (fpSpread1.Sheets[0].Rows.Count > 0)
                {
                    this.Cursor = Cursors.WaitCursor;

                    string ERRCode = "WR", MSGCode = "P0000"; //처리할 내용이 없습니다.
                    SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                    SqlCommand cmd = dbConn.CreateCommand();
                    SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                    try
                    {
                        //그리드 상단 필수 체크
                        if (SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", false))
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

                                    #region 유효성 체크
                                    //검사항목코드 유효성 체크
                                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사항목명")].Text == ""
                                        || fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사순서")].Text == ""
                                        || fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사차수")].Text == "")
                                    {
                                        //존재하지 않는 검사항목 코드입니다
                                        MessageBox.Show(SystemBase.Base.MessageRtn("B0036", "검사항목"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                        fpSpread1.ActiveSheet.SetActiveCell(i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사항목코드"));

                                        Trans.Rollback();
                                        this.Cursor = Cursors.Default;
                                        return;
                                    }
                                    #endregion

                                    string strSql = " usp_QFA001 '" + strGbn + "'";
                                    strSql += ", @pINSP_REQ_NO = '" + txtInspReqNo.Text + "' ";
                                    strSql += ", @pFIN_INSP_LVL	= '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사레벨")].Text + "'";
                                    strSql += ", @pINSP_ITEM_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사항목")].Text + "'";
                                    strSql += ", @pINSP_SERIES = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사차수")].Value + "'";
                                    strSql += ", @pINSP_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사순서")].Value + "'";
                                    strSql += ", @pINSP_METH_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사방식코드")].Text + "'";
                                    strSql += ", @pINSP_QSHOW = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품질표시코드")].Text + "'";

                                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "시료수")].Text != "")
                                        strSql += ", @pSAMPLE_QTY = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "시료수")].Value + "'";

                                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "합격판정개수")].Text != "")
                                        strSql += ", @pACC_DEC_QTY = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "합격판정개수")].Value + "'";

                                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "불합격판정개수")].Text != "")
                                        strSql += ", @pREJ_DEC_QTY = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "불합격판정개수")].Value + "'";

                                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "합격판정계수")].Text != "")
                                        strSql += ", @pACC_DEC_FAC = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "합격판정계수")].Value + "'";

                                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "최대허용불량률")].Text != "")
                                        strSql += ", @pMAX_DEF_RAT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "최대허용불량률")].Value + "'";

                                    strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                                    strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                                    DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                                    ERRCode = ds.Tables[0].Rows[0][0].ToString();
                                    MSGCode = ds.Tables[0].Rows[0][1].ToString();

                                    if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프					
                                }
                            }
                        }
                        else
                        {
                            Trans.Rollback();
                            this.Cursor = Cursors.Default;
                            return;
                        }
                        Trans.Commit();
                    }
                    catch (Exception e)
                    {
                        SystemBase.Loggers.Log(this.Name, e.ToString());
                        Trans.Rollback();
                        ERRCode = "ER";
                        MSGCode = e.Message;
                        //MSGCode = "P0001";	//에러가 발생되어 데이터 처리가 취소되었습니다.
                    }
                Exit:
                    dbConn.Close();
                    if (ERRCode == "OK")
                    {
                        cboSInspStatus.Text = "";
                        txtSInspReqNo.Text = strInspReqNo;
                        Grid2_Search();
                       
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
            }
        }
        #endregion

        #region 그리드 버튼 클릭시
        private void fpSpread1_ButtonClicked(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            //검사항목코드 팝업
            if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "검사항목_2"))
            {
                try
                {
                    string strQuery = " usp_QINSP_ITEM_CALL @pType='P1', @pINSP_REQ_NO ='" + txtInspReqNo.Text + "', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    string[] strWhere = new string[] { "" };
                    string[] strSearch = new string[] { "*" };
                    UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P06008", strQuery, strWhere, strSearch, new int[] { 6 }, "검사항목 조회", false);
                    pu.ShowDialog();
                    if (pu.DialogResult == DialogResult.OK)
                    {
                        Regex rx1 = new Regex("#");
                        string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "검사레벨")].Text = Msgs[0].ToString();
                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "레벨명")].Text = Msgs[1].ToString();
                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "검사항목")].Text = Msgs[2].ToString();
                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "검사항목명")].Text = Msgs[3].ToString();
                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "검사순서")].Text = Msgs[4].ToString();
                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "검사차수")].Text = Msgs[5].ToString();

                        string strQuery1 = "usp_QINSP_ITEM_CALL @pTYPE='C1', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'";
                        strQuery1 += ", @pINSP_REQ_NO ='" + txtInspReqNo.Text + "' ";
                        strQuery1 += ", @pFIN_INSP_LVL = '" + Msgs[0].ToString() + "'";
                        strQuery1 += ", @pINSP_ITEM_CD = '" + Msgs[2].ToString() + "'";
                        strQuery1 += ", @pINSP_SERIES = '" + Msgs[5].ToString() + "'";
                        strQuery1 += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                        DataTable dt = SystemBase.DbOpen.TranDataTable(strQuery1);

                        if (dt.Rows[0][0].ToString() != "WR")
                        {
                            fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "시료수")].Text = dt.Rows[0][0].ToString();
                            fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "합격판정개수")].Text = dt.Rows[0][1].ToString();
                            fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "불합격판정개수")].Text = dt.Rows[0][2].ToString();
                            fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "합격판정계수")].Text = dt.Rows[0][3].ToString();
                            fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "최대허용불량률")].Text = dt.Rows[0][4].ToString();
                            fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "검사방식코드")].Text = dt.Rows[0][5].ToString();
                            fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "검사방식")].Text = dt.Rows[0][6].ToString();
                            fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품질표시코드")].Text = dt.Rows[0][7].ToString();
                            fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품질표시")].Text = dt.Rows[0][8].ToString();
                            fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "검사규격")].Text = dt.Rows[0][9].ToString();
                            fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "하한규격")].Text = dt.Rows[0][10].ToString();
                            fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "상한규격")].Text = dt.Rows[0][11].ToString();
                            fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "측정기코드")].Text = dt.Rows[0][12].ToString();
                            fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "측정기")].Text = dt.Rows[0][13].ToString();
                            fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].Text = dt.Rows[0][14].ToString();
                        }
                        else
                        {
                            string MSGCode = dt.Rows[0][1].ToString();

                            MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);

                            fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "검사레벨")].Text = "";
                            fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "레벨명")].Text = "";
                            fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "검사항목")].Text = "";
                            fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "검사항목명")].Text = "";
                            fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "검사순서")].Text = "";
                            fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "검사차수")].Text = "";

                            return;
                        }
                    }

                    if ((fpSpread1.Sheets[0].GetCellType(e.Row, e.Column).ToString() == "CheckBoxCellType" && fpSpread1.Sheets[0].RowHeader.Cells[e.Row, 0].Text != "I")
                        || (fpSpread1.Sheets[0].GetCellType(e.Row, e.Column).ToString() == "ButtonCellType" && fpSpread1.Sheets[0].RowHeader.Cells[e.Row, 0].Text != "I"))
                    {
                        fpSpread1.Sheets[0].RowHeader.Cells[e.Row, 0].Text = "U";
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "검사항목코드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
                }
            }
        }
        #endregion

        #region 검사항목 불러오기 버튼
        private void butQInspItemCall_Click(object sender, System.EventArgs e)
        {
            if (fpSpread1.Sheets[0].Rows.Count == 0)
            {
                try
                {
                    string strQuery = " usp_QINSP_ITEM_CALL @pType='P1', @pINSP_REQ_NO ='" + txtInspReqNo.Text + "', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    DataTable dt = SystemBase.DbOpen.TranDataTable(strQuery);

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        UIForm.FPMake.RowInsert(fpSpread1);
                        fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "검사항목")].Locked = true;

                        fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "검사레벨")].Text = dt.Rows[i][0].ToString();
                        fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "레벨명")].Text = dt.Rows[i][1].ToString();
                        fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "검사항목")].Text = dt.Rows[i][2].ToString();
                        fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "검사항목명")].Text = dt.Rows[i][3].ToString();
                        fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "검사순서")].Text = dt.Rows[i][4].ToString();
                        fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "검사차수")].Text = dt.Rows[i][5].ToString();

                        string strQuery1 = "usp_QINSP_ITEM_CALL @pTYPE='C1', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'";
                        strQuery1 += ", @pINSP_REQ_NO ='" + txtInspReqNo.Text + "' ";
                        strQuery1 += ", @pFIN_INSP_LVL = '" + dt.Rows[i][0].ToString() + "'";
                        strQuery1 += ", @pINSP_ITEM_CD = '" + dt.Rows[i][2].ToString() + "'";
                        strQuery1 += ", @pINSP_SERIES = '" + dt.Rows[i][5].ToString() + "'";
                        strQuery1 += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                        DataTable dt1 = SystemBase.DbOpen.TranDataTable(strQuery1);

                        if (dt1.Rows[0][0].ToString() != "WR")
                        {
                            fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "시료수")].Text = dt1.Rows[0][0].ToString();
                            fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "합격판정개수")].Text = dt1.Rows[0][1].ToString();
                            fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "불합격판정개수")].Text = dt1.Rows[0][2].ToString();
                            fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "합격판정계수")].Text = dt1.Rows[0][3].ToString();
                            fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "최대허용불량률")].Text = dt1.Rows[0][4].ToString();
                            fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "검사방식코드")].Text = dt1.Rows[0][5].ToString();
                            fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "검사방식")].Text = dt1.Rows[0][6].ToString();
                            fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "품질표시코드")].Text = dt1.Rows[0][7].ToString();
                            fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "품질표시")].Text = dt1.Rows[0][8].ToString();
                            fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "검사규격")].Text = dt1.Rows[0][9].ToString();
                            fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "하한규격")].Text = dt1.Rows[0][10].ToString();
                            fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "상한규격")].Text = dt1.Rows[0][11].ToString();
                            fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "측정기코드")].Text = dt1.Rows[0][12].ToString();
                            fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "측정기")].Text = dt1.Rows[0][13].ToString();
                            fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].Text = dt1.Rows[0][14].ToString();
                        }
                        else
                        {
                            string MSGCode = dt1.Rows[0][1].ToString();

                            MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);

                            fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "검사레벨")].Text = "";
                            fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "레벨명")].Text = "";
                            fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "검사항목")].Text = "";
                            fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "검사항목명")].Text = "";
                            fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "검사순서")].Text = "";
                            fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "검사차수")].Text = "";

                            return;
                        }
                    }

                    if (fpSpread1.Sheets[0].RowCount > 0)
                        fpSpread1.Sheets[0].Rows[0, fpSpread1.Sheets[0].Rows.Count - 1].Height = 30;

                    butQInspItemCall.Enabled = false;
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "검사항목을 불러오는"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
                }
            }
        }
        #endregion

        #region 검사기준서 출력
        private void btnInspBase_Click(object sender, System.EventArgs e)
        {
            string strSheetPage1 = "검사기준서1";

            string strFileName = SystemBase.Base.ProgramWhere + @"\Report\검사기준서.xls";

            try
            {
                th = new Thread(new ThreadStart(Show_Waiting));
                th.Start();
                Thread.Sleep(200);
                Waiting_Form.Activate();
                
                string strQuery1 = " usp_QFA001  @pTYPE = 'R1'";
                strQuery1 += ", @pPLANT_CD = '" + cboSPlantCd.SelectedValue.ToString() + "' ";
                strQuery1 += ", @pITEM_CD = '" + txtItemCd.Text + "' ";
                strQuery1 += ", @pFIN_INSP_LVL = '" + txtFinInspLvl.Text + "' ";
                strQuery1 += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' ";
                strQuery1 += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                DataTable dt1 = SystemBase.DbOpen.NoTranDataTable(strQuery1);

                if (dt1.Rows.Count > 0)
                {
                    Waiting_Form.progressBar_temp.Maximum = dt1.Rows.Count;

                    double dCount = 22;
                    double dRowCount = Convert.ToDouble(dt1.Rows.Count);
                    int iTotPage = Convert.ToInt32(Math.Ceiling((dRowCount - 10) / dCount));

                    string strQuery2 = " usp_QFA001  @pTYPE = 'R2'";
                    strQuery2 += ", @pPLANT_CD = '" + cboSPlantCd.SelectedValue.ToString() + "' ";
                    strQuery2 += ", @pITEM_CD = '" + txtItemCd.Text + "' ";
                    strQuery2 += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    DataTable dt2 = SystemBase.DbOpen.NoTranDataTable(strQuery2);

                    UIForm.VkExcel excel = null;


                    if (File.Exists(strFileName))
                    {
                        File.SetAttributes(strFileName, System.IO.FileAttributes.ReadOnly);
                    }
                    else
                    {
                        // 엑셀 데이터를 생성할 수 없습니다. 원본 파일이 존재하지 않습니다.
                        MessageBox.Show("엑셀 데이터를 생성할 수 없습니다. 원본 파일이 존재하지 않습니다."); ;
                        return;
                    }

                    excel = new UIForm.VkExcel(false);

                    excel.OpenFile(strFileName);
                    // 현재 시트 선택
                    excel.FindExcelWorksheet(strSheetPage1);


                    //데이터수만큼 미리 복사------------------------------------------
                    if (dt1.Rows.Count > 10)
                    {
                        for (int i = 0; i < iTotPage; i++)
                        {
                            excel.SetSelect("A36", "A36");
                            excel.RunMacro("PageAdd");
                        }
                    }
                    //------------------------------------------------------

                    // 엑셀쓰기--------------------------------------------------------- 

                    if (dt1.Rows.Count > 10)
                    {
                        int iRow = 0;
                        int iRow1 = 0;

                        for (int i = 0; i < iTotPage + 1; i++)
                        {
                            Waiting_Form.progressBar_temp.Maximum = iTotPage + 1;

                            if (i == 0) //1Page
                            {
                                // Heard 값
                                excel.SetCell(2, 1, dt1.Rows[0]["TITLE"].ToString());
                                excel.SetCell(5, 4, dt1.Rows[0]["ITEM_SPEC"].ToString());
                                excel.SetCell(5, 8, dt1.Rows[0]["ITEM_NM"].ToString());
                                excel.SetCell(6, 4, dt1.Rows[0]["ITEM_CD"].ToString());
                                excel.SetCell(6, 8, dt1.Rows[0]["KKJGBH"].ToString());
                                excel.SetCell(7, 8, dt1.Rows[0]["ST_NO"].ToString());
                                excel.SetCell(8, 8, dt1.Rows[0]["DRAW_REV"].ToString());


                                //내용입력
                                for (iRow1 = 0; iRow1 < 10; iRow1++)
                                {
                                    excel.SetCell(iRow1 + 20, 1, dt1.Rows[iRow1]["INSP_WEIGHT_NM"].ToString());
                                    excel.SetCell(iRow1 + 20, 2, dt1.Rows[iRow1]["INSP_SEQ"].ToString());
                                    excel.SetCell(iRow1 + 20, 3, dt1.Rows[iRow1]["MAP_COOR"].ToString());
                                    excel.SetCell(iRow1 + 20, 4, dt1.Rows[iRow1]["INSP_ITEM_NM"].ToString());
                                    excel.SetCell(iRow1 + 20, 5, dt1.Rows[iRow1]["INSP_SPEC"].ToString());
                                    excel.SetCell(iRow1 + 20, 7, dt1.Rows[iRow1]["MEASURE_NM"].ToString());
                                    excel.SetCell(iRow1 + 20, 8, dt1.Rows[iRow1]["INSP_METH_NM"].ToString());
                                    excel.SetCell(iRow1 + 20, 9, dt1.Rows[iRow1]["AQL"].ToString());
                                }

                                //개정정보입력
                                if (dt2.Rows.Count > 0)
                                {
                                    for (int j = 0; j < 4; j++)
                                    {
                                        int iCell = 32 + j;
                                        string strValue = excel.GetCellValue("A" + iCell);

                                        for (int k = 0; k < dt2.Rows.Count; k++)
                                        {
                                            if (dt2.Rows[k]["REV_NO"].ToString() == strValue)
                                            {
                                                excel.SetCell(j + 32, 2, dt2.Rows[k]["REV_BASE"].ToString());
                                                excel.SetCell(j + 32, 4, dt2.Rows[k]["REV_DESC_FR"].ToString());
                                                excel.SetCell(j + 32, 6, dt2.Rows[k]["REV_DESC_TO"].ToString());
                                                excel.SetCell(j + 32, 8, dt2.Rows[k]["REV_DT"].ToString());
                                                excel.SetCell(j + 32, 9, dt2.Rows[k]["WRITER"].ToString());
                                                excel.SetCell(j + 32, 10, dt2.Rows[k]["APPROVER"].ToString());
                                            }
                                        }
                                    }
                                }

                                iRow += 35;

                            }
                            else //2Page ....
                            {
                                excel.SetCell(iRow + 2, 1, dt1.Rows[0]["TITLE"].ToString());
                                excel.SetCell(iRow + 5, 4, dt1.Rows[0]["ITEM_SPEC"].ToString());
                                excel.SetCell(iRow + 5, 8, dt1.Rows[0]["ITEM_NM"].ToString());
                                excel.SetCell(iRow + 6, 4, dt1.Rows[0]["ITEM_CD"].ToString());
                                excel.SetCell(iRow + 6, 8, dt1.Rows[0]["KKJGBH"].ToString());
                                excel.SetCell(iRow + 7, 8, dt1.Rows[0]["ST_NO"].ToString());
                                excel.SetCell(iRow + 8, 8, dt1.Rows[0]["DRAW_REV"].ToString());

                                //내용입력
                                int Count = dt1.Rows.Count - iRow1;

                                if (Count > 21)
                                {
                                    for (int j = 0; j < 22; j++)
                                    {
                                        excel.SetCell(j + iRow + 9, 1, dt1.Rows[iRow1]["INSP_WEIGHT_NM"].ToString());
                                        excel.SetCell(j + iRow + 9, 2, dt1.Rows[iRow1]["INSP_SEQ"].ToString());
                                        excel.SetCell(j + iRow + 9, 3, dt1.Rows[iRow1]["MAP_COOR"].ToString());
                                        excel.SetCell(j + iRow + 9, 4, dt1.Rows[iRow1]["INSP_ITEM_NM"].ToString());
                                        excel.SetCell(j + iRow + 9, 5, dt1.Rows[iRow1]["INSP_SPEC"].ToString());
                                        excel.SetCell(j + iRow + 9, 7, dt1.Rows[iRow1]["MEASURE_NM"].ToString());
                                        excel.SetCell(j + iRow + 9, 8, dt1.Rows[iRow1]["INSP_METH_NM"].ToString());
                                        excel.SetCell(j + iRow + 9, 9, dt1.Rows[iRow1]["AQL"].ToString());

                                        iRow1++;
                                    }
                                    iRow += 30;
                                }
                                else
                                {
                                    for (int j = 0; j < Count; j++)
                                    {
                                        excel.SetCell(j + iRow + 9, 1, dt1.Rows[iRow1]["INSP_WEIGHT_NM"].ToString());
                                        excel.SetCell(j + iRow + 9, 2, dt1.Rows[iRow1]["INSP_SEQ"].ToString());
                                        excel.SetCell(j + iRow + 9, 3, dt1.Rows[iRow1]["MAP_COOR"].ToString());
                                        excel.SetCell(j + iRow + 9, 4, dt1.Rows[iRow1]["INSP_ITEM_NM"].ToString());
                                        excel.SetCell(j + iRow + 9, 5, dt1.Rows[iRow1]["INSP_SPEC"].ToString());
                                        excel.SetCell(j + iRow + 9, 7, dt1.Rows[iRow1]["MEASURE_NM"].ToString());
                                        excel.SetCell(j + iRow + 9, 8, dt1.Rows[iRow1]["INSP_METH_NM"].ToString());
                                        excel.SetCell(j + iRow + 9, 9, dt1.Rows[iRow1]["AQL"].ToString());
                                        iRow1++;
                                    }
                                    iRow += 30;
                                }
                            }
                            Waiting_Form.progressBar_temp.Value = iTotPage + 1;

                        }
                    }
                    else  //1Page 만 있을경우
                    {

                        //heard 값입력
                        excel.SetCell(2, 1, dt1.Rows[0]["TITLE"].ToString());
                        excel.SetCell(5, 4, dt1.Rows[0]["ITEM_SPEC"].ToString());
                        excel.SetCell(5, 8, dt1.Rows[0]["ITEM_NM"].ToString());
                        excel.SetCell(6, 4, dt1.Rows[0]["ITEM_CD"].ToString());
                        excel.SetCell(6, 8, dt1.Rows[0]["KKJGBH"].ToString());
                        excel.SetCell(7, 8, dt1.Rows[0]["ST_NO"].ToString());
                        excel.SetCell(8, 8, dt1.Rows[0]["DRAW_REV"].ToString());

                        //개정정보입력
                        if (dt2.Rows.Count > 0)
                        {
                            for (int j = 0; j < 4; j++)
                            {
                                int iCell = 32 + j;
                                string strValue = excel.GetCellValue("A" + iCell);

                                for (int k = 0; k < dt2.Rows.Count; k++)
                                {
                                    if (dt2.Rows[k]["REV_NO"].ToString() == strValue)
                                    {
                                        excel.SetCell(j + 32, 2, dt2.Rows[k]["REV_BASE"].ToString());
                                        excel.SetCell(j + 32, 4, dt2.Rows[k]["REV_DESC_FR"].ToString());
                                        excel.SetCell(j + 32, 6, dt2.Rows[k]["REV_DESC_TO"].ToString());
                                        excel.SetCell(j + 32, 8, dt2.Rows[k]["REV_DT"].ToString());
                                        excel.SetCell(j + 32, 9, dt2.Rows[k]["WRITER"].ToString());
                                        excel.SetCell(j + 32, 10, dt2.Rows[k]["APPROVER"].ToString());
                                    }
                                }
                            }
                        }

                        //내용입력
                        for (int i = 0; i < dt1.Rows.Count; i++)
                        {
                            excel.SetCell(i + 20, 1, dt1.Rows[i]["INSP_WEIGHT_NM"].ToString());
                            excel.SetCell(i + 20, 2, dt1.Rows[i]["INSP_SEQ"].ToString());
                            excel.SetCell(i + 20, 3, dt1.Rows[i]["MAP_COOR"].ToString());
                            excel.SetCell(i + 20, 4, dt1.Rows[i]["INSP_ITEM_NM"].ToString());
                            excel.SetCell(i + 20, 5, dt1.Rows[i]["INSP_SPEC"].ToString());
                            excel.SetCell(i + 20, 7, dt1.Rows[i]["MEASURE_NM"].ToString());
                            excel.SetCell(i + 20, 8, dt1.Rows[i]["INSP_METH_NM"].ToString());
                            excel.SetCell(i + 20, 9, dt1.Rows[i]["AQL"].ToString());
                            Waiting_Form.progressBar_temp.Value = i + 1;

                        }
                    }
                    Waiting_Form.label_temp.Text = "완료되었습니다.";
                    Thread.Sleep(500);
                    excel.ShowExcel(true);
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "검사기준서출력"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            finally
            {
                Waiting_Form.Close();
                th.Abort();
                File.SetAttributes(strFileName, System.IO.FileAttributes.Normal);
            }
        }

        private void Show_Waiting()
        {
            Waiting_Form = new UIForm.ExcelWaiting("검사기준서출력...");
            Waiting_Form.ShowDialog();
        }
        #endregion
        
        #region lnkJump_Click 점프 클릭 이벤트
        private void lnkJump1_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (strJumpFileName1.Length > 0)
                {
                    string DllName = strJumpFileName1.Substring(0, strJumpFileName1.IndexOf("."));
                    string FrmName = strJumpFileName1.Substring(strJumpFileName1.IndexOf(".") + 1, strJumpFileName1.Length - strJumpFileName1.IndexOf(".") - 1);

                    for (int k = 0; k < this.MdiParent.MdiChildren.Length; k++)
                    {	// 폼이 이미 열려있으면 닫기
                        if (MdiParent.MdiChildren[k].Name == FrmName.Substring(0, 6))
                        {
                            MdiParent.MdiChildren[k].BringToFront();
                            MdiParent.MdiChildren[k].Close();
                            break;
                        }
                    }

                    Link1Exec();

                    Assembly ServiceAssembly = Assembly.LoadFile(SystemBase.Base.ProgramWhere.ToString() + "\\" + DllName + "." + FrmName.Substring(0, 6) + ".dll");
                    Form myForm = (Form)System.Activator.CreateInstance(ServiceAssembly.GetType(strJumpFileName1), param);
                    myForm.MdiParent = this.MdiParent;
                    myForm.Show();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "화면 링크"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void lnkJump2_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (strJumpFileName2.Length > 0)
                {
                    string DllName = strJumpFileName2.Substring(0, strJumpFileName2.IndexOf("."));
                    string FrmName = strJumpFileName2.Substring(strJumpFileName2.IndexOf(".") + 1, strJumpFileName2.Length - strJumpFileName2.IndexOf(".") - 1);

                    for (int k = 0; k < this.MdiParent.MdiChildren.Length; k++)
                    {	// 폼이 이미 열려있으면 닫기
                        if (MdiParent.MdiChildren[k].Name == FrmName.Substring(0, 6))
                        {
                            MdiParent.MdiChildren[k].BringToFront();
                            MdiParent.MdiChildren[k].Close();
                            break;
                        }
                    }

                    Link2Exec();

                    Assembly ServiceAssembly = Assembly.LoadFile(SystemBase.Base.ProgramWhere.ToString() + "\\" + DllName + "." + FrmName.Substring(0, 6) + ".dll");
                    Form myForm = (Form)System.Activator.CreateInstance(ServiceAssembly.GetType(strJumpFileName2), param);
                    myForm.MdiParent = this.MdiParent;
                    myForm.Show();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "화면 링크"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void lnkJump3_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (strJumpFileName3.Length > 0)
                {
                    string DllName = strJumpFileName3.Substring(0, strJumpFileName3.IndexOf("."));
                    string FrmName = strJumpFileName3.Substring(strJumpFileName3.IndexOf(".") + 1, strJumpFileName3.Length - strJumpFileName3.IndexOf(".") - 1);

                    for (int k = 0; k < this.MdiParent.MdiChildren.Length; k++)
                    {	// 폼이 이미 열려있으면 닫기
                        if (MdiParent.MdiChildren[k].Name == FrmName.Substring(0, 6))
                        {
                            MdiParent.MdiChildren[k].BringToFront();
                            MdiParent.MdiChildren[k].Close();
                            break;
                        }
                    }

                    Link3Exec();

                    Assembly ServiceAssembly = Assembly.LoadFile(SystemBase.Base.ProgramWhere.ToString() + "\\" + DllName + "." + FrmName.Substring(0, 6) + ".dll");
                    Form myForm = (Form)System.Activator.CreateInstance(ServiceAssembly.GetType(strJumpFileName3), param);
                    myForm.MdiParent = this.MdiParent;
                    myForm.Show();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "화면 링크"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void lnkJump4_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (strJumpFileName4.Length > 0)
                {
                    string DllName = strJumpFileName4.Substring(0, strJumpFileName4.IndexOf("."));
                    string FrmName = strJumpFileName4.Substring(strJumpFileName4.IndexOf(".") + 1, strJumpFileName4.Length - strJumpFileName4.IndexOf(".") - 1);

                    for (int k = 0; k < this.MdiParent.MdiChildren.Length; k++)
                    {	// 폼이 이미 열려있으면 닫기
                        if (MdiParent.MdiChildren[k].Name == FrmName.Substring(0, 6))
                        {
                            MdiParent.MdiChildren[k].BringToFront();
                            MdiParent.MdiChildren[k].Close();
                            break;
                        }
                    }

                    Link4Exec();

                    Assembly ServiceAssembly = Assembly.LoadFile(SystemBase.Base.ProgramWhere.ToString() + "\\" + DllName + "." + FrmName.Substring(0, 6) + ".dll");
                    Form myForm = (Form)System.Activator.CreateInstance(ServiceAssembly.GetType(strJumpFileName4), param);
                    myForm.MdiParent = this.MdiParent;
                    myForm.Show();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "화면 링크"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void lnkJump5_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (strJumpFileName5.Length > 0)
                {
                    string DllName = strJumpFileName5.Substring(0, strJumpFileName5.IndexOf("."));
                    string FrmName = strJumpFileName5.Substring(strJumpFileName5.IndexOf(".") + 1, strJumpFileName5.Length - strJumpFileName5.IndexOf(".") - 1);

                    for (int k = 0; k < this.MdiParent.MdiChildren.Length; k++)
                    {	// 폼이 이미 열려있으면 닫기
                        if (MdiParent.MdiChildren[k].Name == FrmName.Substring(0, 6))
                        {
                            MdiParent.MdiChildren[k].BringToFront();
                            MdiParent.MdiChildren[k].Close();
                            break;
                        }
                    }

                    Link5Exec();

                    Assembly ServiceAssembly = Assembly.LoadFile(SystemBase.Base.ProgramWhere.ToString() + "\\" + DllName + "." + FrmName.Substring(0, 6) + ".dll");
                    Form myForm = (Form)System.Activator.CreateInstance(ServiceAssembly.GetType(strJumpFileName5), param);
                    myForm.MdiParent = this.MdiParent;
                    myForm.Show();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "화면 링크"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void lnkJump6_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (strJumpFileName6.Length > 0)
                {
                    string DllName = strJumpFileName6.Substring(0, strJumpFileName6.IndexOf("."));
                    string FrmName = strJumpFileName6.Substring(strJumpFileName6.IndexOf(".") + 1, strJumpFileName6.Length - strJumpFileName6.IndexOf(".") - 1);

                    for (int k = 0; k < this.MdiParent.MdiChildren.Length; k++)
                    {	// 폼이 이미 열려있으면 닫기
                        if (MdiParent.MdiChildren[k].Name == FrmName.Substring(0, 6))
                        {
                            MdiParent.MdiChildren[k].BringToFront();
                            MdiParent.MdiChildren[k].Close();
                            break;
                        }
                    }

                    Link6Exec();

                    Assembly ServiceAssembly = Assembly.LoadFile(SystemBase.Base.ProgramWhere.ToString() + "\\" + DllName + "." + FrmName.Substring(0, 6) + ".dll");
                    Form myForm = (Form)System.Activator.CreateInstance(ServiceAssembly.GetType(strJumpFileName6), param);
                    myForm.MdiParent = this.MdiParent;
                    myForm.Show();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "화면 링크"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

		#region 품질증빙 확인
		private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			try
			{
				if (fpSpread2.Sheets[0].Rows.Count > 0)
				{
					WNDW037 pu = new WNDW037();
					pu.strWORKORDER_NO = fpSpread2.Sheets[0].Cells[fpSpread2.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx2, "제조오더번호")].Text;
					pu.strPROC_SEQ = fpSpread2.Sheets[0].Cells[fpSpread2.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx2, "공정순번")].Text;
					pu.strPre_PROC_SEQ = fpSpread2.Sheets[0].Cells[fpSpread2.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx2, "전공정")].Text;
					pu.strREQ_TYPE = "RP";
					pu.strDOC_TYPE = "OUT";
					pu.strFormGubn = "QFA001";

					pu.ShowDialog();
				}

			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				DialogResult dsMsg = MessageBox.Show(f.Message, SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		#endregion

        #region 품목사진 뷰어
        private void btnITEM_PICTURE_Click(object sender, EventArgs e)
        {
            WNDW038 pu = new WNDW038(FullFileName);
            pu.ShowDialog();
            if (pu.DialogResult == DialogResult.OK)
            {
            }
        }
        #endregion

    }
}