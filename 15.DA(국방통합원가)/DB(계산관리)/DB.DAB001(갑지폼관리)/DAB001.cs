 #region DAB001 작성 정보
/*************************************************************/
// 단위업무명 : 갑지 폼 항목 등록
// 작 성 자 :   유 재 규
// 작 성 일 :   2013-06-12
// 작성내용 :   
// 수 정 일 :
// 수 정 자 :
// 수정내용 :
// 비    고 : 
// 참    고 : 
/*************************************************************/
#endregion
 
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Globalization;
using C1.C1Preview;
using C1.C1Preview.DataBinding;
using C1.Win.C1Preview;

 

namespace DB.DAB001
{
    public partial class DAB001 : UIForm.FPCOMM2
    {
        int PreRow = -1;

        public DAB001()
        {
            InitializeComponent();
        }

        #region Form Load 시
        private void DAB001_Load(object sender, EventArgs e)
        {
            SystemBase.Base.gstrFromLoading = "N";
            SystemBase.Validation.GroupBox_Setting(groupBox1);//필수 적용

            dtAPPLY_YYYYMM.Value = SystemBase.Base.ServerTime("YYMMDD");
            //제출업체
            SystemBase.ComboMake.C1Combo(cboH_MNUF_CODE, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'D004', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'", 0);   //제출업체
            //계약업체(조달업체)
            SystemBase.ComboMake.C1Combo(cboH_FCTR_CODE, "usp_B_COMMON @pTYPE='REL1', @pCODE = 'D006', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pSPEC1 = '" + cboH_MNUF_CODE.SelectedValue.ToString() + "'", 0);   //공장
            
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "계산식")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'D005', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 0);

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
            UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, false, 0, 0);

            SystemBase.Base.gstrFromLoading = "Y";
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
            UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, false, 0, 0);
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            try
            {
                string strQuery = " usp_DAB001  'S1'";
                strQuery = strQuery + ", @pMNUF_CODE ='" + cboH_MNUF_CODE.SelectedValue + "' ";
                strQuery = strQuery + ", @pFACTORY_CODE='" + cboH_FCTR_CODE.SelectedValue.ToString() + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
                UIForm.FPMake.grdCommSheet(fpSpread2, strQuery, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, true, 0, 0, false);

                this.Cursor = System.Windows.Forms.Cursors.Default;

                PreRow = -1;
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region SaveExec() 폼에 입력된 데이타 저장 로직
        protected override void SaveExec()
        {
            if (SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", true) == true)
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox2))  //컨트롤 필수여부체크 
                {
                    DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("SY048"), SystemBase.Base.MessageRtn("Z0004"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dsMsg == DialogResult.Yes)
                    {
                        string ERRCode = "ER"; string MSGCode = "";
                        MSGCode = "SY001";	    //처리할 내용이 없습니다.
                        SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                        SqlCommand cmd = dbConn.CreateCommand();
                        SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                        try
                        {
                            string strHead = "";
                            string strGbn = "";
                            string strMNUF_CODE = "";
                            string strFACTORY_CODE = "";

                            string strCODE = "";
                            string strD_CLASS_NAME = "";
                            string strM_CLASS_NAME = "";
                            string strS_CLASS_NAME = "";
                            string strDETAIL_NAME = "";
                            string strDEFENSE_RATE = "0";
                            string strPROFIT_RATE = "0";
                            string strUPPER_CODE = "";
                            string strCACULATION_CODE = "";
                            string strSql = "";
                            string strAPPLY_YYYYMM = "";

                            strMNUF_CODE = cboH_MNUF_CODE.SelectedValue.ToString();         // 제출업체 코드
                            strFACTORY_CODE = cboH_FCTR_CODE.SelectedValue.ToString();      // 공장코드 

                            //행수만큼 처리
                            for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                            {

                                strHead = fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text;
                                if (strHead.Length > 0)
                                {
                                    switch (strHead)
                                    {
                                        case "U": strGbn = "U1"; break;
                                        case "D": strGbn = "D1"; break;
                                        case "I": strGbn = "I1"; break;
                                        default: strGbn = ""; break;
                                    }

                                    strAPPLY_YYYYMM = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "적용년월")].Value.ToString().Replace("-", "").Substring(0, 6);
                                    strCODE = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "코드")].Text.ToString();
                                    strD_CLASS_NAME = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "대분류")].Text.ToString();
                                    strM_CLASS_NAME = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "중분류")].Text.ToString();
                                    strS_CLASS_NAME = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "소분류")].Text.ToString();
                                    strDETAIL_NAME = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "상세")].Text.ToString();
                                    strDEFENSE_RATE = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "비율")].Text.ToString().Replace(",", "");
                                    strPROFIT_RATE = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "비율_2")].Text.ToString().Replace(",", "");
                                    strUPPER_CODE = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "상위코드")].Text.ToString();
                                    strCACULATION_CODE = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "계산식")].Value.ToString();

                                    strDEFENSE_RATE = (strDEFENSE_RATE.Trim() == "" ? "0" : strDEFENSE_RATE);
                                    strPROFIT_RATE = (strPROFIT_RATE.Trim() == "" ? "0" : strPROFIT_RATE);

                                    strSql = " usp_DAB001 '" + strGbn + "' ";
                                    strSql = strSql + ", @pMNUF_CODE   = '" + strMNUF_CODE.Trim() + "' ";
                                    strSql = strSql + ", @pFACTORY_CODE   = '" + strFACTORY_CODE.Trim() + "' ";
                                    strSql = strSql + ", @pAPPLY_YYYYMM   = '" + strAPPLY_YYYYMM.Trim() + "' ";
                                    strSql = strSql + ", @pCODE   = '" + strCODE.Trim() + "'";
                                    strSql = strSql + ", @pD_CLASS_NAME   = '" + strD_CLASS_NAME.Trim() + "' ";
                                    strSql = strSql + ", @pM_CLASS_NAME   = '" + strM_CLASS_NAME.Trim() + "' ";
                                    strSql = strSql + ", @pS_CLASS_NAME   = '" + strS_CLASS_NAME.Trim() + "' ";
                                    strSql = strSql + ", @pDETAIL_NAME   = '" + strDETAIL_NAME.Trim() + "' ";
                                    strSql = strSql + ", @pDEFENSE_RATE   = " + strDEFENSE_RATE + " ";
                                    strSql = strSql + ", @pPROFIT_RATE   =  " + strPROFIT_RATE + " ";
                                    strSql = strSql + ", @pUPPER_CODE   = '" + strUPPER_CODE.Trim() + "' ";
                                    strSql = strSql + ", @pCALCULATION_CODE   = '" + strCACULATION_CODE.Trim() + "' ";
                                    strSql = strSql + ", @pIN_ID   = '" + SystemBase.Base.gstrUserID + "' ";

                                    System.Data.DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                                    ERRCode = ds.Tables[0].Rows[0][0].ToString();
                                    MSGCode = ds.Tables[0].Rows[0][1].ToString();

                                    if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }
                                }
                            }


                            Trans.Commit();
                        }
                        catch (Exception f)
                        {
                            SystemBase.Loggers.Log(this.Name, f.ToString());
                            Trans.Rollback();
                            MSGCode = "SY002";	//에러가 발생하여 데이터 처리가 취소되었습니다.
                        }
                    Exit:
                        dbConn.Close();

                        if (ERRCode == "OK")
                        {
                            SearchExec();

                            //좌측 frSpread 재조회
                            string strSql1 = " usp_DAB001  'S2' ";
                            strSql1 = strSql1 + ", @pMNUF_CODE   = '" + cboH_MNUF_CODE.SelectedValue.ToString().Trim() + "' ";            // 제출업체 코드
                            strSql1 = strSql1 + ", @pFACTORY_CODE   = '" + cboH_FCTR_CODE.SelectedValue.ToString().Trim() + "' "; ;       // 공장코드
                            strSql1 = strSql1 + ", @pAPPLY_YYYYMM  = '" + fpSpread2.Sheets[0].Cells[fpSpread2.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx2, "적용년월")].Value.ToString().Replace("-", "") + "' "; ;        // 적용년월

                            UIForm.FPMake.grdCommSheet(fpSpread1, strSql1, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);

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
                }
            }
        }
        #endregion

        #region DeleteExec() 삭제 로직
        protected override void DeleteExec()
        {
            DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("SY010"), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dsMsg == DialogResult.Yes)
            {
                string ERRCode = "ER", MSGCode = "SY001";	//처리할 내용이 없습니다.
                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                try
                {
                    string strSql = " usp_DAB001  'D2'";
                    strSql = strSql + ", @pMNUF_CODE   = '" + cboH_MNUF_CODE.SelectedValue.ToString().Trim() + "' "; ;          // 제출업체 코드
                    strSql = strSql + ", @pFACTORY_CODE   = '" + cboH_FCTR_CODE.SelectedValue.ToString().Trim() + "' ";         // 공장코드
                    strSql = strSql + ", @pAPPLY_YYYYMM   = '" + SystemBase.Validation.C1DataEdit_ReadFormat(dtAPPLY_YYYYMM.Value.ToString().Trim(), "YYYYMM") + "' ";  //적용년월

                    System.Data.DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                    ERRCode = ds.Tables[0].Rows[0][0].ToString();
                    MSGCode = ds.Tables[0].Rows[0][1].ToString();

                    if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                    Trans.Commit();
                    NewExec();
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    Trans.Rollback();
                    MSGCode = "SY002";	//에러가 발생하여 데이터 처리가 취소되었습니다.
                }
            Exit:
                dbConn.Close();

                if (ERRCode == "OK")
                {
                    SearchExec();
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
        }
        #endregion



        #region PrintExe() 출력 로직
        protected override void PrintExe()
        {
            /*
            C1PrintDocument doc = new C1PrintDocument();

            C1.C1Preview.RenderTable rt = new C1.C1Preview.RenderTable();
            rt.Style.Font = new Font("맑은고딕", 8);//기본8로 설정
            rt.Style.TextAlignHorz = AlignHorzEnum.Left;
            rt.Style.TextAlignVert = AlignVertEnum.Center;
            rt.Style.GridLines.All = new LineDef("0.1mm", Color.Black);

            //속성 설정 순서
            //1. text 출력
            //2. SpanRow, SpanCol
            //3. font 설정
            //4. 색상 설정
            //5. 길이 설정
            //6. 정렬 설정
            //7. 그리드라인 설정

            //1.페이지 설정
            SetPage(doc);

            //2.페이지 헤더 설정 
            SetPageHeader(doc);

            //3.칼럼 헤더 설정
            SetColumnHeader(rt);

            //4. 데이타 연결  
            SetDataBinding(doc, rt);

            //To Do:0이면 출력안하기

            //5. 미리보기
            C1PrintPreviewDialog d = new C1PrintPreviewDialog();

            d.Document = doc;
            d.PreviewPane.ZoomFactor = 1;
            d.WindowState = FormWindowState.Maximized;
             
            d.ShowDialog();
             * */
        }
        #endregion


        #region 주석
        /*
        #region DeleteExec() 삭제 로직
        protected override void DeleteExec()
        {

            DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("선택된 적용년월의 데이터를 삭제하시겠습니까"), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dsMsg == DialogResult.Yes)
            {
                string ERRCode = "ER", MSGCode = "SY001";	//처리할 내용이 없습니다.
                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);
                 
                try
                {
                    string strSql = " usp_DAB001  'D2'";
                    strSql = strSql + ", @pMNUF_CODE   = '" + cboH_MNUF_CODE.SelectedValue.ToString().Trim() + "' "; ;          // 제출업체 코드
                    strSql = strSql + ", @pFACTORY_CODE   = '" + cboH_FCTR_CODE.SelectedValue.ToString().Trim() + "' ";         // 공장코드
                    strSql = strSql + ", @pAPPLY_YYYYMM   = '" + fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "적용년월")].Value.ToString().Replace("-", "") + "' ";

                    DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                    ERRCode = ds.Tables[0].Rows[0][0].ToString();
                    MSGCode = ds.Tables[0].Rows[0][1].ToString();

                    if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                    Trans.Commit();
                    NewExec();
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    Trans.Rollback();
                    MSGCode = "SY002";	//에러가 발생하여 데이터 처리가 취소되었습니다.
                }
            Exit:
                dbConn.Close();

                if (ERRCode == "OK")
                {
                    SearchExec();
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
        }
        #endregion
        */
        #endregion

        #region RowInsExec() 행추가 클릭 이벤트
        protected override void RowInsExec()
        {
            try
            {
                if (fpSpread2.Sheets[0].ActiveRowIndex != -1)
                {
                    UIForm.FPMake.RowInsert(fpSpread1);
                    fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "적용년월")].Text = fpSpread2.Sheets[0].Cells[fpSpread2.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx2, "적용년월")].Value.ToString();
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 디폴트값 체크시 체크확인
        private void fpSpread1_ButtonClicked(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            try
            {
                if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "Default"))
                {
                    int intRow = e.Row;
                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {
                        if (i != intRow) fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Default")].Value = 0;
                    }
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion


        #region 좌측그리드 방향키 이동시 우측조회
        private void fpSpread2_SelectionChanged(object sender, FarPoint.Win.Spread.SelectionChangedEventArgs e)
        {
            try
            {
                string strAPPLY_YYYYMM = "";
                if (fpSpread2.Sheets[0].Rows.Count > 0)
                {
                    int intRow = fpSpread2.ActiveSheet.GetSelection(0).Row;
                    if (intRow < 0) return;
                    if (PreRow == intRow && PreRow != -1) return;  //현 Row에서 컬럼이동시는 조회 안되게

                    strAPPLY_YYYYMM = fpSpread2.Sheets[0].Cells[intRow, 1].Text.ToString().Replace("-", "");


                    //조회문.
                    string strSql = " usp_DAB001  'S2' ";
                    strSql = strSql + ", @pMNUF_CODE ='" + cboH_MNUF_CODE.SelectedValue.ToString() + "' ";
                    strSql = strSql + ", @pFACTORY_CODE='" + cboH_FCTR_CODE.SelectedValue.ToString() + "' ";
                    strSql = strSql + ", @pAPPLY_YYYYMM   = '" + strAPPLY_YYYYMM + "' ";  //적용년월

                    UIForm.FPMake.grdCommSheet(fpSpread1, strSql, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);

                    fpSpread1.Sheets[0].SetColumnMerge(SystemBase.Base.GridHeadIndex(GHIdx1, "적용년월"), FarPoint.Win.Spread.Model.MergePolicy.Always);
                    fpSpread1.Sheets[0].SetColumnMerge(SystemBase.Base.GridHeadIndex(GHIdx1, "코드"), FarPoint.Win.Spread.Model.MergePolicy.Always);
                    fpSpread1.Sheets[0].SetColumnMerge(SystemBase.Base.GridHeadIndex(GHIdx1, "대분류"), FarPoint.Win.Spread.Model.MergePolicy.Always);
                    fpSpread1.Sheets[0].SetColumnMerge(SystemBase.Base.GridHeadIndex(GHIdx1, "중분류"), FarPoint.Win.Spread.Model.MergePolicy.Always);
                    fpSpread1.Sheets[0].SetColumnMerge(SystemBase.Base.GridHeadIndex(GHIdx1, "소분류"), FarPoint.Win.Spread.Model.MergePolicy.Always);

                    PreRow = fpSpread2.ActiveSheet.GetSelection(0).Row;


                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Right_Focus(string strScode)
        {
            try
            {
                for (int i = 0; i < fpSpread2.ActiveSheet.Rows.Count; i++)
                {
                    string strItemCd = fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "코드")].Text.ToString();
                    if (strScode == strItemCd)
                    {
                        fpSpread2.ActiveSheet.AddSelection(i, 0, 1, 4);
                    }
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion


        


        #region 최근자료 생성 버튼 클릭
        private void btnSave_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (fpSpread2.Sheets[0].ActiveRowIndex < 0)
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn("SY066"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    fpSpread2.Focus();
                    return;
                }

                CopyNew_Save();
                SearchExec();
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 생성기준월 데이터를 복사하여 최근자료로 생성 저장 루틴
        private void CopyNew_Save()
        {
            //Major Code 저장

            // MessageBox.Show(fpSpread2.Sheets[0].ActiveRowIndex, 1]);



            int intRow = fpSpread2.ActiveSheet.GetSelection(0).Row;
            if (intRow < 0) return;

            string ERRCode = "ER", MSGCode = "SY001";	//처리할 내용이 없습니다.
            SqlConnection dbConn = SystemBase.DbOpen.DBCON();
            SqlCommand cmd = dbConn.CreateCommand();
            SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

            try
            {
                string strSql = " usp_DAB001  'C1' ";
                strSql = strSql + ", @pMNUF_CODE    = '" + cboH_MNUF_CODE.SelectedValue.ToString().Trim() + "' ";
                strSql = strSql + ", @pFACTORY_CODE = '" + cboH_FCTR_CODE.SelectedValue.ToString().Trim() + "' ";
                strSql = strSql + ", @pFROM_YYYYMM  = '" + fpSpread2.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx2, "적용년월")].Value.ToString().Replace("-", "") + "' ";
                strSql = strSql + ", @pAPPLY_YYYYMM = '" + SystemBase.Validation.C1DataEdit_ReadFormat(dtAPPLY_YYYYMM.Value.ToString().Trim(), "YYYYMM") + "' ";

                System.Data.DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                ERRCode = ds.Tables[0].Rows[0][0].ToString();
                MSGCode = ds.Tables[0].Rows[0][1].ToString();

                if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                Trans.Commit();
                SearchExec();
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                Trans.Rollback();
                MSGCode = "SY002";	//에러가 발생하여 데이터 처리가 취소되었습니다.
            }
        Exit:
            dbConn.Close();

            if (ERRCode == "OK")
            {
                MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                if (ERRCode == "ER")
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }

        }
        #endregion

        #region **************************************  업체 글로벌 변수에 할당********************************************
        private void cboH_MNUF_CODE_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (SystemBase.Base.gstrFromLoading == "Y")
                {
                    SystemBase.Base.gstrMNUF_CODE = (cboH_MNUF_CODE.SelectedValue == null ? "" : cboH_MNUF_CODE.SelectedValue.ToString());
                    //공장
                    SystemBase.ComboMake.C1Combo(cboH_FCTR_CODE, "usp_B_COMMON @pTYPE='REL1', @pCODE = 'D006', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pSPEC1 = '" + cboH_MNUF_CODE.SelectedValue.ToString() + "'", 0);   //공장
                    SearchExec();
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void cboH_FCTR_CODE_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (SystemBase.Base.gstrFromLoading == "Y")
                {
                    SystemBase.Base.gstrFCTR_CODE = (cboH_FCTR_CODE.SelectedValue == null ? "" : cboH_FCTR_CODE.SelectedValue.ToString());

                    if (SystemBase.Base.gstrFCTR_CODE == "") cboH_FCTR_CODE.ResetText();
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        private void fpSpread2_Sheet1_RowChanged(object sender, FarPoint.Win.Spread.SheetViewEventArgs e)
        {
            try
            {
                string strAPPLY_YYYYMM = "";
                if (fpSpread2.Sheets[0].Rows.Count > 0)
                {
                    int intRow = fpSpread2.ActiveSheet.GetSelection(0).Row;
                    if (intRow < 0) return;
                    if (PreRow == intRow && PreRow != -1) return;  //현 Row에서 컬럼이동시는 조회 안되게

                    strAPPLY_YYYYMM = fpSpread2.Sheets[0].Cells[e.Row, 1].Text.ToString().Replace("-", "");

                    //조회문.
                    string strSql = " usp_DAB001  'S2' ";
                    strSql = strSql + ", @pMNUF_CODE ='" + cboH_MNUF_CODE.SelectedValue.ToString() + "' ";
                    strSql = strSql + ", @pFACTORY_CODE='" + cboH_FCTR_CODE.SelectedValue.ToString() + "' ";
                    strSql = strSql + ", @pAPPLY_YYYYMM   = '" + strAPPLY_YYYYMM + "' ";  //적용년월

                    UIForm.FPMake.grdCommSheet(fpSpread1, strSql, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

                    fpSpread1.Sheets[0].SetColumnMerge(SystemBase.Base.GridHeadIndex(GHIdx1, "적용년월"), FarPoint.Win.Spread.Model.MergePolicy.Always);
                    fpSpread1.Sheets[0].SetColumnMerge(SystemBase.Base.GridHeadIndex(GHIdx1, "코드"), FarPoint.Win.Spread.Model.MergePolicy.Always);
                    fpSpread1.Sheets[0].SetColumnMerge(SystemBase.Base.GridHeadIndex(GHIdx1, "대분류"), FarPoint.Win.Spread.Model.MergePolicy.Always);
                    fpSpread1.Sheets[0].SetColumnMerge(SystemBase.Base.GridHeadIndex(GHIdx1, "중분류"), FarPoint.Win.Spread.Model.MergePolicy.Always);
                    fpSpread1.Sheets[0].SetColumnMerge(SystemBase.Base.GridHeadIndex(GHIdx1, "소분류"), FarPoint.Win.Spread.Model.MergePolicy.Always);

                    //SystemBase.Validation.GroupBox_SearchViewValidation(groupBox2);  //컨트롤 Key값 처리

                    PreRow = fpSpread2.ActiveSheet.GetSelection(0).Row;
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #region 크리스탈레포트 사용시 사용 안하는 함수
        //크리스탈레포트 사용시 사용 안하는 함수
        private void SetPage(C1PrintDocument doc)
        {
            doc.Clear();
            doc.PageLayout.PageSettings.Landscape = true; //가로
            doc.PageLayout.PageSettings.LeftMargin = "2.5cm";
            doc.PageLayout.PageSettings.RightMargin = "1cm";
            doc.PageLayout.PageSettings.TopMargin = "0.4cm";
            doc.PageLayout.PageSettings.BottomMargin = "0.8cm";
 
        }
        //크리스탈레포트 사용시 사용 안하는 함수
        private void SetPageHeader(C1PrintDocument doc)
        {
            TableCell c = null;
            RenderTable hTable = new RenderTable();
            RenderTable hSubTable1 = new RenderTable();
            RenderTable hSubTable2 = new RenderTable();

            hTable.Rows.Insert(0, 2);

            //헤더
            hTable.Style.Font = new Font("맑은고딕", 8);
            hTable.Style.TextAlignHorz = AlignHorzEnum.Left;
            hTable.Style.TextAlignVert = AlignVertEnum.Center;
            hTable.Style.GridLines.All = LineDef.Empty;

            hTable.RowGroups[1, 1].Style.Borders.Top = new LineDef("0.1mm", Color.Black);
            hTable.Cells[1, 0].Style.Borders.Left = new LineDef("0.1mm", Color.Black);
            hTable.Cells[1, 0].Style.Borders.Right = new LineDef("0.1mm", Color.Black);
           
            //보고서번호 라인, 담당자ID 라인
            hSubTable1.Style.Font = new Font("맑은고딕", 8);
            hSubTable1.Style.TextAlignHorz = AlignHorzEnum.Left;
            hSubTable1.Style.TextAlignVert = AlignVertEnum.Center;
            hSubTable1.Style.GridLines.All = LineDef.Empty;

            //요구 라인
            hSubTable2.Style.Font = new Font("맑은고딕", 9, System.Drawing.FontStyle.Bold);
            hSubTable2.Style.TextAlignHorz = AlignHorzEnum.Left;
            hSubTable2.Style.TextAlignVert = AlignVertEnum.Center;
            hSubTable2.Style.GridLines.All = LineDef.Empty;
 
            hSubTable2.Cells[0, 0].Style.Borders.Right = new LineDef("0.1mm", Color.Black);
            hSubTable2.Cells[0, 1].Style.Borders.Right = new LineDef("0.1mm", Color.Black);
            hSubTable2.Cells[0, 2].Style.Borders.Right = new LineDef("0.1mm", Color.Black);
            hSubTable2.Cells[0, 3].Style.Borders.Right = new LineDef("0.1mm", Color.Black);
            hSubTable2.Cells[0, 4].Style.Borders.Right = new LineDef("0.1mm", Color.Black);
            hSubTable2.Cells[0, 5].Style.Borders.Right = new LineDef("0.1mm", Color.Black);
            hSubTable2.Cells[0, 6].Style.Borders.Right = new LineDef("0.1mm", Color.Black);
            hSubTable2.Cells[0, 7].Style.Borders.Right = new LineDef("0.1mm", Color.Black);
            hSubTable2.Cells[0, 8].Style.Borders.Right = new LineDef("0.1mm", Color.Black);

            hSubTable2.CellStyle.GridLines.Bottom = LineDef.Empty;

            /*hSubTable2.Cells[0, 0].Style.Borders.Bottom = new LineDef("0.1mm", Color.Black);
            hSubTable2.Cells[0, 1].Style.Borders.Bottom = new LineDef("0.1mm", Color.Black);
            hSubTable2.Cells[0, 2].Style.Borders.Bottom = new LineDef("0.1mm", Color.Black);
            hSubTable2.Cells[0, 3].Style.Borders.Bottom = new LineDef("0.1mm", Color.Black);
            hSubTable2.Cells[0, 4].Style.Borders.Bottom = new LineDef("0.1mm", Color.Black);
            hSubTable2.Cells[0, 5].Style.Borders.Bottom = new LineDef("0.1mm", Color.Black);
            hSubTable2.Cells[0, 6].Style.Borders.Bottom = new LineDef("0.1mm", Color.Black);
            hSubTable2.Cells[0, 7].Style.Borders.Bottom = new LineDef("0.1mm", Color.Black);
            hSubTable2.Cells[0, 8].Style.Borders.Bottom = new LineDef("0.1mm", Color.Black);
            hSubTable2.Cells[0, 9].Style.Borders.Bottom = new LineDef("0.1mm", Color.Black);*/
      
            //페이지 헤더 첫번째 설정 - 보고서번호,담당자 ID 2줄 동시에 설정
            c = hSubTable1.Cells[0, 0];
            c.Text = "보고서 번호   :";
            hSubTable1.Cols[0].Width = "2.6cm";

            //보고서번호
            c = hSubTable1.Cells[0, 1];
            c.Text = "DICS_CC_PAM_A1_004_2012";
            hSubTable1.Cols[1].Width = "6cm";

            c = hSubTable1.Cells[0, 2];
            c.Text = "방산 원가계산서(갑)";
            c.SpanRows = 2;
            c.Style.Font = new Font("맑은고딕", 15, System.Drawing.FontStyle.Bold);
            c.Style.TextAlignHorz = AlignHorzEnum.Center;
            c.Style.FontUnderline = true;

            c = hSubTable1.Cells[0, 3];
            c.Text = "날     짜 :";
            hSubTable1.Cols[3].Width = "1.8cm";

            //날짜
            c = hSubTable1.Cells[0, 4];
            c.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            hSubTable1.Cols[4].Width = "2.9cm";

            c = hSubTable1.Cells[1, 0];
            c.Text = "담 당 자 ID   :";

            //담당자ID
            c = hSubTable1.Cells[1, 1];
            c.Text = "1238106174th";

            c = hSubTable1.Cells[1, 3];
            c.Text = "페 이 지 :";

            //페이지
            c = hSubTable1.Cells[1, 4];
            c.Text = "[PageNo] / [PageCount]";
            c.Style.TextAlignHorz = AlignHorzEnum.Center;

            hTable.Cells[0, 0].RenderObject = hSubTable1; 

            c = hSubTable2.Cells[0, 0];
            c.Text = "요구";
            c.Style.BackColor = Color.LightGray;
            hSubTable2.Cols[0].Width = "0.8cm";
            c.Style.TextAlignHorz = AlignHorzEnum.Center;
            
 
            //연도
            c = hSubTable2.Cells[0, 1];
            c.Text = "2012";
            c.Style.TextAlignHorz = AlignHorzEnum.Center;
            hSubTable2.Cols[1].Width = "1.2cm";

            c = hSubTable2.Cells[0, 2];
            c.Text = "부서";
            c.Style.BackColor = Color.LightGray;
            c.Style.TextAlignHorz = AlignHorzEnum.Center;
            hSubTable2.Cols[2].Width = "0.8cm";

            //부서명
            c = hSubTable2.Cells[0, 3];
            c.Text = "[EHB] 통신장비계약팀";
            hSubTable2.Cols[3].Width = "4cm";

            c = hSubTable2.Cells[0, 4];
            c.Text = "판단번호";
            c.Style.BackColor = Color.LightGray;
            c.Style.TextAlignHorz = AlignHorzEnum.Center;
            hSubTable2.Cols[4].Width = "1.5cm";

            //판단번호
            c = hSubTable2.Cells[0, 5];
            c.Text = "0350B";
            hSubTable2.Cols[5].Width = "2cm";

            //S
            c = hSubTable2.Cells[0, 6];
            c.Text = "SC";
            c.Style.TextAlignHorz = AlignHorzEnum.Center;
            hSubTable2.Cols[6].Width = "0.7cm";

            //부품
            c = hSubTable2.Cells[0, 7];

            //"업체" 타이틀 
            c = hSubTable2.Cells[0, 8];
            c.Text = "업체";
            c.Style.BackColor = Color.LightGray;
            c.Style.TextAlignHorz = AlignHorzEnum.Center;
            hSubTable2.Cols[8].Width = "1cm";

            //업체
            c = hSubTable2.Cells[0, 9];
            c.Text = "[1257F] (주)휴니드테크놀러지스";
            hSubTable2.Cols[9].Width = "6.5cm";

            hTable.Cells[1, 0].RenderObject = hSubTable2;

            doc.PageLayout.PageHeader = hTable;
            //doc.PageLayout.PageHeader.Style.TextAlignHorz = AlignHorzEnum.Right;
            //doc.PageLayout.PageHeader.Style.Spacing.Bottom = "0.0cm";
            //doc.PageLayout.PageHeader.Style.Borders.Bottom = LineDef.Default; 
        }
        //크리스탈레포트 사용시 사용 안하는 함수
        private void SetColumnHeader(RenderTable rt)
        {
            rt.Cols[0].Width = "0.5cm";//첫번째 타이틀
            rt.Cols[1].Width = "0.5cm";//두번째 타이틀
            rt.Cols[2].Width = "1.9cm";//세번째 타이틀
            rt.Cols[3].Width = "0.83cm";//비율

           // rt.Cols[3].CellStyle.Padding.Right = "0.1cm"; //
           // rt.Cols[5].CellStyle.Padding.Right = "0.1cm"; //
           // rt.Cols[6].CellStyle.Padding.Right = "0.1cm"; //

          //  rt.Cols[3].Style.TextAlignHorz = AlignHorzEnum.Right; //
          //  rt.Cols[5].Style.TextAlignHorz = AlignHorzEnum.Right; //
          //  rt.Cols[6].Style.TextAlignHorz = AlignHorzEnum.Right; //

            int iRow = 0;
            TableCell c;
            //1.3 "재고번호" 타이틀, "품명" 타이틀, "구분" 타이틀 헤더 설정하기 : 데이타 출력 Col과 맞물리는 헤더임

            iRow = 0;
            c = rt.Cells[iRow, 0];
            c.Text = "재고번호 단위 항목";
            c.SpanCols = 4;
            c.Style.BackColor = Color.LightGray;
            c.Style.TextAlignHorz = AlignHorzEnum.Center;

            //"품명" 타이틀:폰트, 백그라운드, 선, 길이 지정
            iRow = iRow + 1;

            c = rt.Cells[iRow, 0];
            c.Text = "품명";
            c.SpanCols = 4;
            c.Style.BackColor = Color.LightGray;
            c.Style.TextAlignHorz = AlignHorzEnum.Center;

            //"구분" 타이틀:폰트, 백그라운드, 선, 길이 지정
            iRow = iRow + 1;
            c = rt.Cells[iRow, 0];
            c.Text = "구분";
            c.SpanCols = 3;
            c.Style.BackColor = Color.LightGray;
            c.Style.TextAlignHorz = AlignHorzEnum.Center;

            //"비율" 타이틀:폰트, 백그라운드, 선, 길이 지정
            c = rt.Cells[iRow, 3];
            c.Text = "비율";
            c.Style.BackColor = Color.LightGray;
            c.Style.TextAlignHorz = AlignHorzEnum.Center;

            //재료비 
            iRow = iRow + 1;
            c = rt.Cells[iRow, 0];
            c.Text = "재료비";
            c.SpanRows = 12;
            c.Style.FontBold = true;
            c.Style.TextAlignHorz = AlignHorzEnum.Center;

            c = rt.Cells[iRow, 1];
            c.Text = "직접";
            c.SpanRows = 6;
            c.Style.FontBold = true;
            c.Style.TextAlignHorz = AlignHorzEnum.Center;

            rt.Cells[iRow, 2].Text = "주요재료비";

            iRow = iRow + 1;
            rt.Cells[iRow, 2].Text = "구입부품비";            

            iRow = iRow + 1;
            rt.Cells[iRow, 2].Text = "방산부품비";            

            iRow = iRow + 1;
            rt.Cells[iRow, 2].Text = "수입재료비";            

            iRow = iRow + 1;
            rt.Cells[iRow, 2].Text = "수입부품비";            

            iRow = iRow + 1;
            rt.Cells[iRow, 2].Text = "포장재료비";            

            iRow = iRow + 1;
            c = rt.Cells[iRow, 1];
            c.Text = "(반제품비)";
            c.SpanCols = 2;            

            iRow = iRow + 1;
            c = rt.Cells[iRow, 1];
            c.Text = "간접재료비";
            c.SpanCols = 2;            

            iRow = iRow + 1;
            c = rt.Cells[iRow, 1];
            c.Text = "소계";
            c.SpanCols = 3;            

            iRow = iRow + 1;
            c = rt.Cells[iRow, 1];
            c.Text = "작업설물(-)";
            c.SpanCols = 2;            

            iRow = iRow + 1;
            c = rt.Cells[iRow, 1];
            c.Text = "합계";
            c.SpanCols = 3;            

            iRow = iRow + 1;
            c = rt.Cells[iRow, 1];
            c.Text = "(관급재료비)";
            c.SpanCols = 2;            

            iRow = iRow + 1;
            c = rt.Cells[iRow, 0];
            c.Text = "";
            c.SpanRows = 3;

            c = rt.Cells[iRow, 1];
            c.Text = "직접노무비";
            c.SpanCols = 2;
            
            iRow = iRow + 1;
            c = rt.Cells[iRow, 0];
            c.Text = "";
            c.SpanRows = 3;

            c = rt.Cells[iRow, 1];
            c.Text = "간접";
            c.SpanCols = 2;            

            iRow = iRow + 1;
            c = rt.Cells[iRow, 0];
            c.Text = "";
            c.SpanRows = 3;

            c = rt.Cells[iRow, 1];
            c.Text = "계";
            c.SpanCols = 3;            


            //경비
            iRow = iRow + 1;
            c = rt.Cells[iRow, 0];
            c.Text = "경비";
            c.SpanRows = 18;
            c.Style.FontBold = true;
            c.Style.TextAlignHorz = AlignHorzEnum.Center;

            c = rt.Cells[iRow, 1];
            c.Text = "직접";
            c.SpanRows = 17;
            c.Style.FontBold = true;
            c.Style.TextAlignHorz = AlignHorzEnum.Center;

            rt.Cells[iRow, 2].Text = "감가상각비";            

            iRow = iRow + 1;
            rt.Cells[iRow, 2].Text = "연구개발비";            

            iRow = iRow + 1;
            rt.Cells[iRow, 2].Text = "기  술  료";            

            iRow = iRow + 1;
            rt.Cells[iRow, 2].Text = "시험검사비";            

            iRow = iRow + 1;
            rt.Cells[iRow, 2].Text = "지급임차료";            

            iRow = iRow + 1;
            rt.Cells[iRow, 2].Text = "외주가공비";            

            iRow = iRow + 1;
            rt.Cells[iRow, 2].Text = "중소외주가공";            

            iRow = iRow + 1;
            rt.Cells[iRow, 2].Text = "설치시운전비";            

            iRow = iRow + 1;
            rt.Cells[iRow, 2].Text = "특허권사용료";            

            iRow = iRow + 1;
            rt.Cells[iRow, 2].Text = "공사비";            

            iRow = iRow + 1;
            rt.Cells[iRow, 2].Text = "공식행사비";            

            iRow = iRow + 1;
            rt.Cells[iRow, 2].Text = "설계비";            

            iRow = iRow + 1;
            rt.Cells[iRow, 2].Text = "보관비";            

            iRow = iRow + 1;
            rt.Cells[iRow, 2].Text = "";            

            iRow = iRow + 1;
            rt.Cells[iRow, 2].Text = "";            

            iRow = iRow + 1;
            rt.Cells[iRow, 2].Text = "";

            //rt.Cells[iRow, 2].Text = "소계";
            iRow = iRow + 1;
            c = rt.Cells[iRow, 2];
            c.Text = "소계";
            c.SpanCols = 2;               

            iRow = iRow + 1;
            c = rt.Cells[iRow, 0];
            c.Text = "";
            c.SpanRows = 3;
            c = rt.Cells[iRow, 1];
            c.Text = "간접";
            c.SpanCols = 2;            

            iRow = iRow + 1;
            c = rt.Cells[iRow, 0];
            c.Text = "합계";
            c.SpanCols = 4;
            c.Style.BackColor = Color.LightGray;

            iRow = iRow + 1;
            c = rt.Cells[iRow, 0];
            c.Text = "제조원가(관급포함)";
            c.SpanCols = 4;

            iRow = iRow + 1;
            c = rt.Cells[iRow, 0];
            c.Text = "제조원가(관급제외)";
            c.SpanCols = 4;

            iRow = iRow + 1;
            c = rt.Cells[iRow, 0];
            c.Text = "일반관리비";
            c.SpanCols = 3;
            c = rt.Cells[iRow, 3];

            iRow = iRow + 1;
            c = rt.Cells[iRow, 0];
            c.Text = "총  원  가";
            c.SpanCols = 4;
            c.Style.BackColor = Color.DarkGray;

            iRow = iRow + 1;
            c = rt.Cells[iRow, 0];
            c.Text = "투하자본보상비";
            c.SpanCols = 3;
            c = rt.Cells[iRow, 3];
 
            //이윤
            iRow = iRow + 1;
            c = rt.Cells[iRow, 0];
            c.Text = "이윤";
            c.SpanRows = 11;
            c.Style.FontBold = true;
            c.Style.TextAlignHorz = AlignHorzEnum.Center;

            c = rt.Cells[iRow, 1];
            c.Text = "기본보상";
            c.SpanCols = 2;
  
            iRow = iRow + 1;
            c = rt.Cells[iRow, 1];
            c.Text = "기술위험보상";
            c.SpanCols = 2;
     
            iRow = iRow + 1;
            c = rt.Cells[iRow, 1];
            c.Text = "계약위험보상";
            c.SpanCols = 2;
   
            iRow = iRow + 1;
            c = rt.Cells[iRow, 1];
            c.Text = "계약수행노력보";
            c.SpanCols = 2;
  
            iRow = iRow + 1;
            c = rt.Cells[iRow, 1];
            c.Text = "원가절감노력보";
            c.SpanCols = 2;

            iRow = iRow + 1;
            c = rt.Cells[iRow, 1];
            c.Text = "설비투자노력보";
            c.SpanCols = 2;

            iRow = iRow + 1;
            c = rt.Cells[iRow, 1];
            c.Text = "경영노력(통보율)";
            c.SpanCols = 2;

            iRow = iRow + 1;
            c = rt.Cells[iRow, 1];
            c.Text = "경영노력(품질)";
            c.SpanCols = 2;

            iRow = iRow + 1;
            c = rt.Cells[iRow, 1];
            c.Text = "경영노력(연계)";
            c.SpanCols = 2;

            iRow = iRow + 1;
            c = rt.Cells[iRow, 1];
            c.Text = "경영노력(부당이득가산금)";
            c.SpanCols = 2;

            iRow = iRow + 1;
            c = rt.Cells[iRow, 1];
            c.Text = "소계";
            c.SpanCols = 2;

            iRow = iRow + 1;
            c = rt.Cells[iRow, 0];
            c.Text = "부품국산화";
            c.SpanCols = 4;

            iRow = iRow + 1;
            c = rt.Cells[iRow, 0];
            c.Text = "관세 등";
            c.SpanCols = 4;

            iRow = iRow + 1;
            c = rt.Cells[iRow, 0];
            c.Text = "B.I.I";
            c.SpanCols = 4;

            iRow = iRow + 1;
            c = rt.Cells[iRow, 0];
            c.Text = "추가비목경비";
            c.SpanCols = 4;

            iRow = iRow + 1;
            c = rt.Cells[iRow, 0];
            c.Text = "수출물량(감가상각비)";
            c.SpanCols = 4;

            iRow = iRow + 1;
            c = rt.Cells[iRow, 0];
            c.Text = "계산가격";
            c.SpanCols = 4;
            c.Style.BackColor = Color.DarkGray;

        }
        //크리스탈레포트 사용시 사용 안하는 함수
        private void SetDataBinding(C1PrintDocument doc, RenderTable rt)
        {
            /*
            TableCell c;
            int iRow = 0;

            String strSql = "";

            strSql = "  usp_갑지생성_EKLEE @pTYPE = 'I1', @pPK_SEQ = 2, @pIN_ID = 'sys' ";
            DataTable dt = SystemBase.DbOpen.NoTranDataTable(strSql);
            if (dt != null && dt.Rows.Count > 0)
            {
                //01.주요재료비~10.작업설물(-)
                iRow = 3;
                for (int row = 1; row <= 10; ++row)
                { 

                    if (row != 9)
                    {
                        c = rt.Cells[iRow, 3];
                        c.Text = string.Format("{0:#,###.##}", dt.Rows[0]["A" + string.Format("{0:00}", row) + "_RATE"]);
                        c.Style.TextAlignHorz = AlignHorzEnum.Right;
                        c.CellStyle.Padding.Right = "0.1cm";
                    }

                    iRow++;
                }

                //19.관급재료비
                c = rt.Cells[14, 3];
                c.Text = string.Format("{0:#,###.##}", dt.Rows[0]["A19_RATE"]);
                c.Style.TextAlignHorz = AlignHorzEnum.Right;
                c.CellStyle.Padding.Right = "0.1cm";

                //20.직접노무비
                c = rt.Cells[15, 3];
                c.Text = string.Format("{0:#,###.##}", dt.Rows[0]["A20_RATE"]);
                c.Style.TextAlignHorz = AlignHorzEnum.Right;
                c.CellStyle.Padding.Right = "0.1cm";

                //21.간접노무비
                c = rt.Cells[16, 1];
                c.Text = "간접(    " + string.Format("{0:#,###.##}", dt.Rows[0]["A21_ETC_RATE"]) + " %)"; 
     
                c = rt.Cells[16, 3];
                c.Text = string.Format("{0:#,###.##}", dt.Rows[0]["A21_RATE"]);
                c.Style.TextAlignHorz = AlignHorzEnum.Right;
                c.CellStyle.Padding.Right = "0.1cm";

                //30.감가상각비~42.보관비
                iRow = 18;
                for (int row = 30; row <= 42; ++row)
                {
                    c = rt.Cells[iRow, 3];
                    c.Text = string.Format("{0:#,###.##}", dt.Rows[0]["A" + string.Format("{0:00}", row) + "_RATE"]);
                    c.Style.TextAlignHorz = AlignHorzEnum.Right;
                    c.CellStyle.Padding.Right = "0.1cm";

                    iRow++;
                }

                //48.간접경비
                c = rt.Cells[35, 1];
                c.Text = "간접(    " + string.Format("{0:#,###.##}", dt.Rows[0]["A48_ETC_RATE"]) + " %)"; 

                c = rt.Cells[35, 3];
                c.Text = string.Format("{0:#,###.##}", dt.Rows[0]["A48_RATE"]);
                c.Style.TextAlignHorz = AlignHorzEnum.Right;
                c.CellStyle.Padding.Right = "0.1cm";

                //55.일반관리비
                c = rt.Cells[39, 0];
                c.Text = "일반관리비(    " + string.Format("{0:#,###.##}", dt.Rows[0]["A55_ETC_RATE"]) + " %)";
                c.SpanCols = 3;
                c.Style.TextAlignHorz = AlignHorzEnum.Right;
                c.CellStyle.Padding.Right = "0.1cm";

                c = rt.Cells[39, 3];
                c.Text = string.Format("{0:#,###.##}", dt.Rows[0]["A55_RATE"]);
                c.Style.TextAlignHorz = AlignHorzEnum.Right;
                c.CellStyle.Padding.Right = "0.1cm";

                //60.투하자본보상비~63.계약위험보상
                iRow = 41;
                for (int row = 60; row <= 63; ++row)
                {
                    c = rt.Cells[iRow, 3];
                    c.Text = string.Format("{0:#,###.##}", dt.Rows[0]["A" + string.Format("{0:00}", row) + "_RATE"]);
                    c.Style.TextAlignHorz = AlignHorzEnum.Right;
                    c.CellStyle.Padding.Right = "0.1cm";

                    iRow++;
                }  

                //65.원가절감노력보상~70.경영노력(부당이득가산금) (-)
                iRow = 46;
                for (int row = 65; row <= 70; ++row)
                {
                    c = rt.Cells[iRow, 3];
                    c.Text = string.Format("{0:#,###.##}", dt.Rows[0]["A" + string.Format("{0:00}", row) + "_RATE"]);
                    c.Style.TextAlignHorz = AlignHorzEnum.Right;
                    c.CellStyle.Padding.Right = "0.1cm";

                    iRow++;
                }  
            }  


            strSql = " usp_갑지생성_EKLEE @pTYPE = 'I1', @pPK_SEQ = 2, @pIN_ID = 'sys'   ";

            //칼럼값 정의 1
            rt.ColGroups[4, 4].DataBinding.DataSource = SystemBase.DbOpenForReport.C1ReportDataSet(doc, strSql); 

            //rt.Cols[4].CellStyle.Padding.Right = "0.1cm"; //순번
            //rt.Cols[5].CellStyle.Padding.Right = "0.1cm"; //금액
           // rt.Cols[6].CellStyle.Padding.Right = "0.1cm"; //이윤액

            //rt.Cols[4].Style.TextAlignHorz = AlignHorzEnum.Right; //순번
            //rt.Cols[5].Style.TextAlignHorz = AlignHorzEnum.Right; //금액
           // rt.Cols[6].Style.TextAlignHorz = AlignHorzEnum.Right; //이윤액

            rt.Cols[4].Width = "0.5cm";  //순번
            rt.Cols[5].Width = "1.87cm"; //금액
            rt.Cols[6].Width = "1.87cm"; //이윤액
            rt.Cols[7].Width = "0.2cm";  //공백

            iRow = 0;
            c = rt.Cells[iRow, 4];
            c.Text = "[Fields!SEQ.Value]";
            c.SpanRows = 2;
            c.Style.BackColor = Color.LightGray;
            c.Style.TextAlignHorz = AlignHorzEnum.Center;

            c = rt.Cells[iRow, 5];
            c.Text = "[Fields!NIIN.Value]" + " " + "[Fields!UNIT.Value]" + " " + "[Fields!DMST_ITNB.Value]";
            c.SpanCols = 2;
            c.Style.BackColor = Color.LightGray;
            c.Style.TextAlignHorz = AlignHorzEnum.Left;
            c.CellStyle.Padding.Left = "0.1cm"; 

            c = rt.Cells[iRow, 7];
            c.SpanRows = rt.Rows.Count;
            c.Text = "";

            //칼럼값 정의 2
            iRow ++;
            c = rt.Cells[iRow, 5];
            c.Text = "[Fields!RPST_ITNM.Value]";
            c.SpanCols = 2;
            c.Style.BackColor = Color.LightGray;
            c.Style.TextAlignHorz = AlignHorzEnum.Left;
            c.CellStyle.Padding.Left = "0.1cm"; 

            iRow++;
            c = rt.Cells[iRow, 4];
            c.Text = "금액";
            c.SpanCols = 2;
            c.Style.BackColor = Color.LightGray;
            c.Style.TextAlignHorz = AlignHorzEnum.Center; 

            c = rt.Cells[iRow, 6];
            c.Text = "이윤액";
            c.Style.BackColor = Color.LightGray;
            c.Style.TextAlignHorz = AlignHorzEnum.Center;

            //1.주요재료비 ~ 10.작업설물(-)
            for (int row = 1; row <= 10; ++row)
            {
                iRow++;
 
                c = rt.Cells[iRow, 4];
                c.Text = "[string.Format(\"{0:#,###.##}\", Fields!A" + string.Format("{0:00}", row) + "_AMT.Value)]";
                c.SpanCols = 2;
                c.Style.TextAlignHorz = AlignHorzEnum.Right;
                c.CellStyle.Padding.Right = "0.1cm";

                c = rt.Cells[iRow, 6];
                c.Text = "[string.Format(\"{0:#,###.##}\", Fields!A" + string.Format("{0:00}", row) + "_PROFIT.Value)]";
                c.Style.TextAlignHorz = AlignHorzEnum.Right;
                c.CellStyle.Padding.Right = "0.1cm";                 
            }
            //18.[재료비합계]
            iRow++;
            c = rt.Cells[iRow, 4];
            c.Text = "[string.Format(\"{0:#,###.##}\", Fields!A18_AMT.Value)]"; 
            c.SpanCols = 2;
            c.Style.TextAlignHorz = AlignHorzEnum.Right; 
            c.CellStyle.Padding.Right = "0.1cm"; 

            c = rt.Cells[iRow, 6];
            c.Text = "[string.Format(\"{0:#,###.##}\", Fields!A18_PROFIT.Value)]"; 
            c.Style.TextAlignHorz = AlignHorzEnum.Right; 
            c.CellStyle.Padding.Right = "0.1cm";

            //19.관급재료비
            iRow++;
            c = rt.Cells[iRow, 4];
            c.Text = "[string.Format(\"{0:#,###.##}\", Fields!A19_AMT.Value)]"; 
            c.SpanCols = 2;
            c.Style.TextAlignHorz = AlignHorzEnum.Right;
            c.CellStyle.Padding.Right = "0.1cm";
             
            c = rt.Cells[iRow, 6];
            c.Text = "[string.Format(\"{0:#,###.##}\", Fields!A19_PROFIT.Value)]"; 
            c.Style.TextAlignHorz = AlignHorzEnum.Right;
            c.CellStyle.Padding.Right = "0.1cm";

            //20.직접노무비
            iRow++;
            c = rt.Cells[iRow, 4];
            c.Text = "[string.Format(\"{0:#,###.##}\", Fields!A20_AMT.Value)]"; 
            c.SpanCols = 2;
            c.Style.TextAlignHorz = AlignHorzEnum.Right;
            c.CellStyle.Padding.Right = "0.1cm";
             
            c = rt.Cells[iRow, 6];
            c.Text = "[string.Format(\"{0:#,###.##}\", Fields!A20_PROFIT.Value)]"; 
            c.Style.TextAlignHorz = AlignHorzEnum.Right;
            c.CellStyle.Padding.Right = "0.1cm";

            //21.간접노무비
            iRow++;
            c = rt.Cells[iRow, 4];
            c.Text = "[string.Format(\"{0:#,###.##}\", Fields!A21_AMT.Value)]"; 
            c.SpanCols = 2;
            c.Style.TextAlignHorz = AlignHorzEnum.Right;
            c.CellStyle.Padding.Right = "0.1cm";

            c = rt.Cells[iRow, 6];
            c.Text = "[string.Format(\"{0:#,###.##}\", Fields!A21_PROFIT.Value)]"; 
            c.Style.TextAlignHorz = AlignHorzEnum.Right;
            c.CellStyle.Padding.Right = "0.1cm";

            //25.[노무비합계]
            iRow++;
            c = rt.Cells[iRow, 4];
            c.Text = "[string.Format(\"{0:#,###.##}\", Fields!A25_AMT.Value)]"; 
            c.SpanCols = 2;
            c.Style.TextAlignHorz = AlignHorzEnum.Right;
            c.CellStyle.Padding.Right = "0.1cm";

            c = rt.Cells[iRow, 6];
            c.Text = "[string.Format(\"{0:#,###.##}\", Fields!A25_PROFIT.Value)]"; 
            c.Style.TextAlignHorz = AlignHorzEnum.Right;
            c.CellStyle.Padding.Right = "0.1cm";


            //30.감가상각비 ~ 42.보관비
            for (int row = 30; row <= 42; ++row)
            {
                iRow++;
                c = rt.Cells[iRow, 4];
                c.Text = "[string.Format(\"{0:#,###.##}\", Fields!A" + string.Format("{0:00}", row) + "_AMT.Value)]"; 
                c.SpanCols = 2;
                c.Style.TextAlignHorz = AlignHorzEnum.Right;
                c.CellStyle.Padding.Right = "0.1cm";

                c = rt.Cells[iRow, 6];
                c.Text = "[string.Format(\"{0:#,###.##}\", Fields!A" + string.Format("{0:00}", row) + "_PROFIT.Value)]"; 
                c.Style.TextAlignHorz = AlignHorzEnum.Right;
                c.CellStyle.Padding.Right = "0.1cm";
            }

            //공백3줄
            for (int row = 42; row <= 44; ++row)
            {
                iRow++;
                c = rt.Cells[iRow, 4];
                c.Text = "";
                c.SpanCols = 2;

                c = rt.Cells[iRow, 6];
                c.Text = "";

            }

            //47.소계
            iRow++;
            c = rt.Cells[iRow, 4];
            c.Text = "[string.Format(\"{0:#,###.##}\", Fields!A47_AMT.Value)]"; 
            c.SpanCols = 2;
            c.Style.TextAlignHorz = AlignHorzEnum.Right;
            c.CellStyle.Padding.Right = "0.1cm";

            c = rt.Cells[iRow, 6];
            c.Text = "[string.Format(\"{0:#,###.##}\", Fields!A47_PROFIT.Value)]"; 
            c.Style.TextAlignHorz = AlignHorzEnum.Right;
            c.CellStyle.Padding.Right = "0.1cm";
 
            //48.간접경비
            iRow++;
            c = rt.Cells[iRow, 4];
            c.Text = "[string.Format(\"{0:#,###.##}\", Fields!A48_AMT.Value)]"; 
            c.SpanCols = 2;
            c.Style.TextAlignHorz = AlignHorzEnum.Right;
            c.CellStyle.Padding.Right = "0.1cm";

            c = rt.Cells[iRow, 6];
            c.Text = "[string.Format(\"{0:#,###.##}\", Fields!A48_PROFIT.Value)]"; 
            c.Style.TextAlignHorz = AlignHorzEnum.Right;
            c.CellStyle.Padding.Right = "0.1cm";

 
            //49.[경비합계]
            iRow++;
            c = rt.Cells[iRow, 4];
            c.Text = "[string.Format(\"{0:#,###.##}\", Fields!A49_AMT.Value)]"; 
            c.SpanCols = 2;
            c.Style.TextAlignHorz = AlignHorzEnum.Right;
            c.CellStyle.Padding.Right = "0.1cm";

            c = rt.Cells[iRow, 6];
            c.Text = "[string.Format(\"{0:#,###.##}\", Fields!A49_PROFIT.Value)]"; 
            c.Style.TextAlignHorz = AlignHorzEnum.Right;
            c.CellStyle.Padding.Right = "0.1cm";


            //50.제조원가(관급포함)
            iRow++;
            c = rt.Cells[iRow, 4];
            c.Text = "[string.Format(\"{0:#,###.##}\", Fields!A50_AMT.Value)]"; 
            c.SpanCols = 2;
            c.Style.TextAlignHorz = AlignHorzEnum.Right;
            c.CellStyle.Padding.Right = "0.1cm";

            c = rt.Cells[iRow, 6];
            c.Text = "[string.Format(\"{0:#,###.##}\", Fields!A50PROFIT.Value)]"; 
            c.Style.TextAlignHorz = AlignHorzEnum.Right;
            c.CellStyle.Padding.Right = "0.1cm";

              
            //51.제조원가(관급제외)
            iRow++;
            c = rt.Cells[iRow, 4];
            c.Text = "[string.Format(\"{0:#,###.##}\", Fields!A51_AMT.Value)]"; 
            c.SpanCols = 2;
            c.Style.TextAlignHorz = AlignHorzEnum.Right;
            c.CellStyle.Padding.Right = "0.1cm";

            c = rt.Cells[iRow, 6];
            c.Text = "[string.Format(\"{0:#,###.##}\", Fields!A51_PROFIT.Value)]"; 
            c.Style.TextAlignHorz = AlignHorzEnum.Right;
            c.CellStyle.Padding.Right = "0.1cm";

            //55.일반관리비
            iRow++;
            c = rt.Cells[iRow, 4];
            c.Text = "[string.Format(\"{0:#,###.##}\", Fields!A55_AMT.Value)]"; 
            c.SpanCols = 2;
            c.Style.TextAlignHorz = AlignHorzEnum.Right;
            c.CellStyle.Padding.Right = "0.1cm";

            c = rt.Cells[iRow, 6];
            c.Text = "[string.Format(\"{0:#,###.##}\", Fields!A55_PROFIT.Value)]"; 
            c.Style.TextAlignHorz = AlignHorzEnum.Right;
            c.CellStyle.Padding.Right = "0.1cm"; 
    
            //59.총원가
            iRow++;
            c = rt.Cells[iRow, 4];
            c.Text = "[string.Format(\"{0:#,###.##}\", Fields!A59_AMT.Value)]"; 
            c.SpanCols = 2;
            c.Style.TextAlignHorz = AlignHorzEnum.Right;
            c.CellStyle.Padding.Right = "0.1cm";

            c = rt.Cells[iRow, 6];
            c.Text = "[string.Format(\"{0:#,###.##}\", Fields!A59_PROFIT.Value)]"; 
            c.Style.TextAlignHorz = AlignHorzEnum.Right;
            c.CellStyle.Padding.Right = "0.1cm";

            //60.투하자본보상비
            iRow++;
            c = rt.Cells[iRow, 4];
            c.Text = "[string.Format(\"{0:#,###.##}\", Fields!A60_AMT.Value)]"; 
            c.SpanCols = 2;
            c.Style.TextAlignHorz = AlignHorzEnum.Right;
            c.CellStyle.Padding.Right = "0.1cm";

            c = rt.Cells[iRow, 6];
            c.Text = "[string.Format(\"{0:#,###.##}\", Fields!A60_PROFIT.Value)]"; 
            c.Style.TextAlignHorz = AlignHorzEnum.Right;
            c.CellStyle.Padding.Right = "0.1cm"; 

   
            //61.기본보상
            iRow++;
            c = rt.Cells[iRow, 4];
            c.Text = "[string.Format(\"{0:#,###.##}\", Fields!A61_AMT.Value)]"; 
            c.SpanCols = 2;
            c.Style.TextAlignHorz = AlignHorzEnum.Right;
            c.CellStyle.Padding.Right = "0.1cm";
             
            //62.기술위험보상
            iRow++;
            c = rt.Cells[iRow, 4];
            c.Text = "[string.Format(\"{0:#,###.##}\", Fields!A62_AMT.Value)]"; 
            c.SpanCols = 2;
            c.Style.TextAlignHorz = AlignHorzEnum.Right;
            c.CellStyle.Padding.Right = "0.1cm";
   
            //63.계약위험보상
            iRow++;
            c = rt.Cells[iRow, 4];
            c.Text = "[string.Format(\"{0:#,###.##}\", Fields!A63_AMT.Value)]"; 
            c.SpanCols = 2;
            c.Style.TextAlignHorz = AlignHorzEnum.Right;
            c.CellStyle.Padding.Right = "0.1cm";

            //64.계약수행노력보상
            iRow++;
            c = rt.Cells[iRow, 4];
            c.Text = "[string.Format(\"{0:#,###.##}\", Fields!A64_AMT.Value)]"; 
            c.SpanCols = 2;
            c.Style.TextAlignHorz = AlignHorzEnum.Right;
            c.CellStyle.Padding.Right = "0.1cm";
             
            //65.원가절감노력보상
            iRow++;
            c = rt.Cells[iRow, 4];
            c.Text = "[string.Format(\"{0:#,###.##}\", Fields!A65_AMT.Value)]"; 
            c.SpanCols = 2;
            c.Style.TextAlignHorz = AlignHorzEnum.Right;
            c.CellStyle.Padding.Right = "0.1cm";

            //66.설비투자노력보상
            iRow++;
            c = rt.Cells[iRow, 4];
            c.Text = "[string.Format(\"{0:#,###.##}\", Fields!A66_AMT.Value)]"; 
            c.SpanCols = 2;
            c.Style.TextAlignHorz = AlignHorzEnum.Right;
            c.CellStyle.Padding.Right = "0.1cm";
             
            //67.경영노력(통보율)
            iRow++;
            c = rt.Cells[iRow, 4];
            c.Text = "[string.Format(\"{0:#,###.##}\", Fields!A67_AMT.Value)]"; 
            c.SpanCols = 2;
            c.Style.TextAlignHorz = AlignHorzEnum.Right;
            c.CellStyle.Padding.Right = "0.1cm";
             
            //68.경영노력(품질)
            iRow++;
            c = rt.Cells[iRow, 4];
            c.Text = "[string.Format(\"{0:#,###.##}\", Fields!A68_AMT.Value)]"; 
            c.SpanCols = 2;
            c.Style.TextAlignHorz = AlignHorzEnum.Right;
            c.CellStyle.Padding.Right = "0.1cm";
              
            //69.경영노력(연계)
            iRow++;
            c = rt.Cells[iRow, 4];
            c.Text = "[string.Format(\"{0:#,###.##}\", Fields!A69_AMT.Value)]"; 
            c.SpanCols = 2;
            c.Style.TextAlignHorz = AlignHorzEnum.Right;
            c.CellStyle.Padding.Right = "0.1cm";
 
            //70.경영노력(부당이득가산금) (-)
            iRow++;
            c = rt.Cells[iRow, 4];
            c.Text = "[string.Format(\"{0:#,###.##}\", Fields!A70_AMT.Value)]"; 
            c.SpanCols = 2;
            c.Style.TextAlignHorz = AlignHorzEnum.Right;
            c.CellStyle.Padding.Right = "0.1cm";
               
            //75.[이윤소계]
            iRow++;
            c = rt.Cells[iRow, 4];
            c.Text = "[string.Format(\"{0:#,###.##}\", Fields!A75_AMT.Value)]"; 
            c.SpanCols = 2;
            c.Style.TextAlignHorz = AlignHorzEnum.Right;
            c.CellStyle.Padding.Right = "0.1cm";

            //76.부품국산화
            iRow++;
            c = rt.Cells[iRow, 4];
            c.Text = "[string.Format(\"{0:#,###.##}\", Fields!A76_AMT.Value)]"; 
            c.SpanCols = 2;
            c.Style.TextAlignHorz = AlignHorzEnum.Right;
            c.CellStyle.Padding.Right = "0.1cm";
              
            //77.관세 등
            iRow++;
            c = rt.Cells[iRow, 4];
            c.Text = "[string.Format(\"{0:#,###.##}\", Fields!A77_AMT.Value)]"; 
            c.SpanCols = 2;
            c.Style.TextAlignHorz = AlignHorzEnum.Right;
            c.CellStyle.Padding.Right = "0.1cm";
             
            //78.B.I.I
            iRow++;
            c = rt.Cells[iRow, 4];
            c.Text = "[string.Format(\"{0:#,###.##}\", Fields!A78_AMT.Value)]"; 
            c.SpanCols = 2;
            c.Style.TextAlignHorz = AlignHorzEnum.Right;
            c.CellStyle.Padding.Right = "0.1cm";
                        
  
            //79.추가비목경비
            iRow++;
            c = rt.Cells[iRow, 4];
            c.Text = "[string.Format(\"{0:#,###.##}\", Fields!A79_AMT.Value)]"; 
            c.SpanCols = 2;
            c.Style.TextAlignHorz = AlignHorzEnum.Right;
            c.CellStyle.Padding.Right = "0.1cm";
            
            //80.수출물량(감가상각비)
            iRow++;
            c = rt.Cells[iRow, 4];
            c.Text = "[string.Format(\"{0:#,###.##}\", Fields!A80_AMT.Value)]"; 
            c.SpanCols = 2;
            c.Style.TextAlignHorz = AlignHorzEnum.Right;
            c.CellStyle.Padding.Right = "0.1cm";

            //85.계산가격
            iRow++;
            c = rt.Cells[iRow, 4];
            c.Text = "[string.Format(\"{0:#,###.##}\", Fields!A85_AMT.Value)]"; 
            c.SpanCols = 2;
            c.Style.TextAlignHorz = AlignHorzEnum.Right;
            c.CellStyle.Padding.Right = "0.1cm"; 

            doc.Body.Children.Add(rt);

            rt.SplitHorzBehavior = SplitBehaviorEnum.SplitIfNeeded;
            rt.Width = "auto";
            rt.RowGroups[0, 3].Header = TableHeaderEnum.Page;
            rt.ColGroups[0, 4].Header = TableHeaderEnum.All;
            rt.ColGroups[0, 4].Style.BackColor = Color.Yellow;

            rt.Rows[0].Height = "0.3cm";
            rt.Rows[1].Height = "0.3cm";
            rt.Rows[2].Height = "0.3cm";
            rt.Rows[3].Height = "0.3cm";
            for (int row = 4; row < rt.Rows.Count; ++row)
            {

                rt.Rows[row].Height = "0.3cm";
            }
             * */
        }
        #endregion

        

        



    }

}
