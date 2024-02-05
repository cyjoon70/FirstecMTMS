#region 작성정보
/*********************************************************************/
// 단위업무명 : 출고요청등록
// 작 성 자 : 조  홍  태
// 작 성 일 : 2013-02-21
// 작성내용 : 출고요청등록 및 조회
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


namespace SF.SFA001
{
    public partial class SFA001 : UIForm.FPCOMM2
    {
        #region 변수선언
        int PreRow = -1;   // SelectionChanged 시에 동일 Row에서 데이타변환 처리 안하도록 하기 위함.
        int NewFlg = 1;//마스터 데이터 수정여부 0:등록,수정X, 1:등록, 2:수정
        string strAutoDnNo = "";
        string strSearchData = "", strSaveData = ""; //컨트롤 저장 체크 변수
        #endregion

        #region 생성자
        public SFA001()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void SFA001_Load(object sender, System.EventArgs e)
        {
            //그룹박스 필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            SystemBase.Validation.GroupBox_Setting(groupBox2);

            //GropBox1 조회조건 콤보박스 세팅
            SystemBase.ComboMake.C1Combo(cboSSaleDuty, "usp_S_COMMON @pTYPE = 'S010' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 3); //영업담당
            SystemBase.ComboMake.C1Combo(cboSMoveType, "usp_S_COMMON @pTYPE = 'S080' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 3);//출고형태

            //GroupBox2 입력조건 콤보박스 세팅
            SystemBase.ComboMake.C1Combo(cboSaleDuty, "usp_S_COMMON @pTYPE = 'S010' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 9); //영업담당
            SystemBase.ComboMake.C1Combo(cboMoveType, "usp_S_COMMON @pTYPE = 'S080', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");//출고형태
            SystemBase.ComboMake.C1Combo(cboPlantCd, "usp_B_COMMON @pType='B031', @pSPEC1 = '" + SystemBase.Base.gstrBIZCD + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");//공장
            SystemBase.ComboMake.C1Combo(cboTranMeth, "usp_B_COMMON @pType='COMM', @pCODE = 'S013', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 9);//운송방법

            //그리드 콤보박스 세팅
            //MASTER
            G2Etc[SystemBase.Base.GridHeadIndex(GHIdx2, "영업담당")] = SystemBase.ComboMake.ComboOnGrid("usp_S_COMMON @pType='S010' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//영업담당
            G2Etc[SystemBase.Base.GridHeadIndex(GHIdx2, "출고형태")] = SystemBase.ComboMake.ComboOnGrid("usp_S_COMMON @pTYPE = 'S080' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//출고형태

            //DETAIL
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "출고단위")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'Z005', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//출고단위
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "재고단위")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'Z005', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//재고단위
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "검사구분")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'S021', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//검사구분

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
            UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, false, 0, 0);

            //폼 컨트롤 초기화
            Control_Setting();
        }
        #endregion

        #region ControlSetting()
        private void Control_Setting()
        {
            //기타 세팅
            dtpSDeliveryDtFr.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString();
            dtpSDeliveryDtTo.Value = SystemBase.Base.ServerTime("YYMMDD");

            dtpSReqDtFr.Value = null;
            dtpSReqDtTo.Value = null;

            rdoAll.Checked = true;
            dtpReqDt.Enabled = true;
            btnSoRef.Enabled = true;

            dtpReqDt.Value = SystemBase.Base.ServerTime("YYMMDD");

            txtDnNo.Tag = "";
            txtDnNo.ReadOnly = false;
            txtDnNo.BackColor = SystemBase.Validation.Kind_White;

            string strQuery = "";
            strQuery = strQuery + "SELECT 1 FROM S_SALE_DUTY(NOLOCK) WHERE CO_CD = '"+ SystemBase.Base.gstrCOMCD.ToString() +"' ";
            strQuery = strQuery + "   AND SALE_DUTY = '"+ SystemBase.Base.gstrUserID.ToString() +"'";

            DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

            if (dt.Rows.Count > 0)
            {
                cboSaleDuty.SelectedValue = SystemBase.Base.gstrUserID.ToString();
            }
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            SystemBase.Validation.GroupBox_Reset(groupBox2);

            NewFlg = 1;
            strAutoDnNo = "";

            SystemBase.Validation.GroupBoxControlsLock(groupBox2, false);

            fpSpread1.Sheets[0].Rows.Count = 0;
            fpSpread2.Sheets[0].Rows.Count = 0;

            //폼 컨트롤 초기화
            Control_Setting();
        }
        #endregion

        #region DelExec() 삭제 로직
        protected override void DeleteExec()
        {
            this.Cursor = Cursors.WaitCursor;

            //출고상태가 아니면
            if (chkDnYn.Checked == true)
            {
                MessageBox.Show(SystemBase.Base.MessageRtn("B0041"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);//확정된 데이터는 다른 작업을 할 수 없습니다.
                this.Cursor = Cursors.Default;
                return;
            }

            string msg = SystemBase.Base.MessageRtn("B0027");
            DialogResult dsMsg = MessageBox.Show(msg, "삭제확인", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dsMsg == DialogResult.Yes)
            {
                string ERRCode = "ER", MSGCode = "P0000";	//처리할 내용이 없습니다.
                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                try
                {
                    string strSql = " usp_SFA001  'D1', @pDN_NO = '" + strAutoDnNo + "'";
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
                    ERRCode = "ER";
                    MSGCode = "P0001";	//에러가 발생하여 데이터 처리가 취소되었습니다.
                }
            Exit:
                dbConn.Close();

                if (ERRCode == "OK")
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Search();
                    fpSpread1.Sheets[0].Rows.Count = 0;
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

        #region SearchExec() Master 그리드 조회 로직
        protected override void SearchExec()
        {
            //마스터만 조회
            Search();
        }
        #endregion

        #region Search 조회함수
        private void Search()
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {
                    //주문처 유효성체크
                    if (txtSSoldCustCd.Text != "" && txtSSoldCustNm.Text == "")
                    {
                        MessageBox.Show(SystemBase.Base.MessageRtn("B0036", "주문처"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning); //존재하지 않는 주문처 코드입니다.

                        txtSSoldCustCd.Focus();
                        this.Cursor = Cursors.Default;

                        return;
                    }

                    //납품처 유효성체크
                    if (txtSShipCustCd.Text != "" && txtSShipCustNm.Text == "")
                    {
                        MessageBox.Show(SystemBase.Base.MessageRtn("B0036", "납품처"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning); //존재하지 않는 주문처 코드입니다.

                        txtSShipCustCd.Focus();
                        this.Cursor = Cursors.Default;

                        return;
                    }

                    string strCfmYn = "";
                    if (rdoNo.Checked == true) { strCfmYn = "Y"; }
                    else if (rdoYes.Checked == true) { strCfmYn = "N"; }
                    else { strCfmYn = ""; }

                    string strQuery = " usp_SFA001  @pTYPE = 'S1'";
                    strQuery += ", @pREQ_DT_FR = '" + dtpSReqDtFr.Text + "' ";
                    strQuery += ", @pREQ_DT_TO = '" + dtpSReqDtTo.Text + "' ";
                    strQuery += ", @pDELIVERY_DT_FR = '" + dtpSDeliveryDtFr.Text + "'";
                    strQuery += ", @pDELIVERY_DT_TO = '" + dtpSDeliveryDtTo.Text + "'";
                    strQuery += ", @pPROJECT_NO = '" + txtSProjectNo.Text + "' ";
                    strQuery += ", @pSALE_DUTY = '" + cboSSaleDuty.SelectedValue.ToString() + "' ";
                    strQuery += ", @pMOVE_TYPE = '" + cboSMoveType.SelectedValue.ToString() + "' ";
                    strQuery += ", @pSHIP_CUST = '" + txtSShipCustCd.Text + "' ";
                    strQuery += ", @pSHIP_CUST_NM = '" + txtSShipCustNm.Text + "' ";
                    strQuery += ", @pSOLD_CUST = '" + txtSSoldCustCd.Text + "' ";
                    strQuery += ", @pSOLD_CUST_NM = '" + txtSSoldCustNm.Text + "' ";
                    strQuery += ", @pDN_YN = '" + strCfmYn + "' ";
                    strQuery += ", @pSO_NO = '" + txtSSoNo.Text + "'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    strQuery += ", @pREF_DELV_DT_FR = '" + dtpRefDelvDtFr.Text + "' ";      // 2017.11.10. hma 추가: 납기일(참조) FROM
                    strQuery += ", @pREF_DELV_DT_TO = '" + dtpRefDelvDtTo.Text + "' ";      // 2017.11.10. hma 추가: 납기일(참조) TO

                    UIForm.FPMake.grdCommSheet(fpSpread2, strQuery, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, true, 0, 0, true);

                    if (fpSpread2.Sheets[0].Rows.Count > 0)
                    {
                        NewFlg = 2;
                    }
                    else
                    {
                        NewFlg = 1;
                        SystemBase.Validation.GroupBox_Reset(groupBox2);
                        fpSpread1.Sheets[0].Rows.Count = 0;
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }

            this.Cursor = Cursors.Default;
        }
        #endregion

        #region Master그리드 선택시 상세정보 조회
        private void fpSpread2_SelectionChanged(object sender, FarPoint.Win.Spread.SelectionChangedEventArgs e)
        {
            if (fpSpread2.Sheets[0].Rows.Count > 0)
            {
                try
                {
                    int intRow = fpSpread2.ActiveSheet.GetSelection(0).Row;

                    //같은 Row 조회 되지 않게
                    if (intRow < 0)
                    {
                        return;
                    }

                    if (PreRow == intRow && PreRow != -1 && intRow != 0)   //현 Row에서 컬럼이동시는 조회 안되게
                    {
                        return;
                    }

                    NewFlg = 2;
                    strAutoDnNo = fpSpread2.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx2, "요청번호")].Text.ToString();//요청번호

                    SubSearch(strAutoDnNo);
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
                }
            }
        }
        #endregion

        #region 상세정보 조회
        private void SubSearch(string Code)
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {
                SystemBase.Validation.GroupBox_Reset(groupBox2);

                fpSpread1.Sheets[0].Rows.Count = 0;

                //출고요청Master정보
                string strSql = " usp_SFA001  'S2', @pDN_NO = '" + Code + "' ";
                strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                DataTable dt = SystemBase.DbOpen.NoTranDataTable(strSql);

                if (dt.Rows.Count > 0)
                {
                    NewFlg = 2;

                    bool ConfirmChk = false;

                    //출고여부
                    if (dt.Rows[0]["DN_YN"].ToString() != "")
                    {
                        if (dt.Rows[0]["DN_YN"].ToString() == "Y") { ConfirmChk = true; }
                        else { ConfirmChk = false; }
                    }
                    else { ConfirmChk = false; }

                    txtDnNo.Value = dt.Rows[0]["DN_NO"].ToString();
                    dtpReqDt.Value = dt.Rows[0]["REQ_DT"].ToString();

                    if (ConfirmChk == true)
                    {
                        dtpReqDt.Enabled = false;
                        dtpReqDt.Tag = ";2;;";
                    }
                    else
                    {
                        dtpReqDt.Enabled = true;
                        dtpReqDt.Tag = "출고요청일자;1;;";
                    }

                    if (dt.Rows[0]["ACTUAL_DT"].ToString() != "" && dt.Rows[0]["ACTUAL_DT"] != null)
                    {
                        dtpActualDt.ReadOnly = false;
                        dtpActualDt.Enabled = true;
                        dtpActualDt.Value = dt.Rows[0]["ACTUAL_DT"].ToString().Substring(0, 10);
                    }
                    chkDnYn.Checked = ConfirmChk;
                    cboPlantCd.SelectedValue = dt.Rows[0]["PLANT_CD"].ToString();
                    cboMoveType.SelectedValue = dt.Rows[0]["MOVE_TYPE"].ToString();
                    cboSaleDuty.SelectedValue = dt.Rows[0]["SALE_DUTY"].ToString();
                    if (dt.Rows[0]["TRAN_METH"].ToString() != "")
                    { cboTranMeth.SelectedValue = dt.Rows[0]["TRAN_METH"].ToString(); }
                    txtShipCustCd.Value = dt.Rows[0]["SHIP_CUST"].ToString();
                    txtTranNo.Value = dt.Rows[0]["TRAN_NO"].ToString();
                    txtTranDutyId.Value = dt.Rows[0]["TRAN_DUTY"].ToString();
                    txtRemark.Value = dt.Rows[0]["REMARK"].ToString();
                    dtxtDnAmt.Value = dt.Rows[0]["DN_AMT"];
                    dtxtDnAmtLoc.Value = dt.Rows[0]["DN_AMT_LOC"];
                    dtxtVatAmtLoc.Value = dt.Rows[0]["VAT_AMT"];
                    dtxtNetAmtLoc.Value = dt.Rows[0]["NET_AMT"];

                    txtDnNo.Tag = ";2;;";
                    txtDnNo.ReadOnly = true;
                    txtDnNo.BackColor = SystemBase.Validation.Kind_Gainsboro;
                   
                    //현재 row값 설정
                    PreRow = fpSpread2.ActiveSheet.ActiveRowIndex;

                    SystemBase.Validation.GroupBox_SearchViewValidation(groupBox2); //Key값 컨트롤 세팅

                    //컨트롤 체크값 초기화
                    strSearchData = "";
                    //컨트롤 체크 함수
                    GroupBox[] gBox = new GroupBox[] { groupBox2};
                    SystemBase.Validation.Control_Check(gBox, ref strSearchData);

                    //출고요청Detail그리드 정보.
                    string strSql1 = " usp_SFA001  'S3' , @pDN_NO = '" + Code + "', @pBIZ_CD = '" + SystemBase.Base.gstrBIZCD + "' ";
                    strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    UIForm.FPMake.grdCommSheet(fpSpread1, strSql1, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);

                    //출고여부에 따른 화면 Locking
                    if (ConfirmChk == true)
                    {
                        //Detail Locking설정
                        for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                        {
                            UIForm.FPMake.grdReMake(fpSpread1, i,
                                SystemBase.Base.GridHeadIndex(GHIdx1, "요청수량") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "창고코드") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "창고코드_2") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "Location코드") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "Location코드_2") + "|3"
                                );
                        }

                        SystemBase.Validation.GroupBoxControlsLock(groupBox2, true);
                        btnPreview.Enabled = true;
                    }
                    else
                    {
                        //Detail Locking해제
                        for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                        {
                            UIForm.FPMake.grdReMake(fpSpread1, i,
                                SystemBase.Base.GridHeadIndex(GHIdx1, "요청수량") + "|1"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "창고코드") + "|1"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "창고코드_2") + "|0"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "Location코드") + "|1"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "Location코드_2") + "|0"
                                );
                        }

                        SystemBase.Validation.GroupBoxControlsLock(groupBox2, false);
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }

            this.Cursor = Cursors.Default;
        }
        #endregion

        #region SaveExec() 폼에 입력된 데이타 저장 로직
        protected override void SaveExec()
        {
            this.Cursor = Cursors.WaitCursor;

            string ChkDnNo = strAutoDnNo;
            GroupBox[] gBox = null;

            string strMstType = "";

            /////////////////////////////////////////////// MASTER 저장 시작 /////////////////////////////////////////////////
            //출고상태가 아니면
            if (chkDnYn.Checked == false)
            {
                //컨트롤 체크값 초기화
                strSaveData = "";
                //컨트롤 체크 함수
                gBox = new GroupBox[] { groupBox2 };
                SystemBase.Validation.Control_Check(gBox, ref strSaveData);

                //기존 컨트롤 데이터와 현재 컨트롤 데이터 비교
                if (strSearchData == strSaveData && UIForm.FPMake.HasSaveData(fpSpread1) == false)
                {
                    //변경되거나 처리할 데이터가 없습니다.
                    MessageBox.Show(SystemBase.Base.MessageRtn("SY017"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.Cursor = Cursors.Default;
                    return;
                }

                //상단 그룹박스 필수 체크
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox2))
                {
                    if (fpSpread1.Sheets[0].Rows.Count > 0)
                    {
                        string ERRCode = "ER", MSGCode = "P0000"; //처리할 내용이 없습니다.
                        string strDnSeq = "";

                        SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                        SqlCommand cmd = dbConn.CreateCommand();
                        SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                        try
                        {
                            if (NewFlg == 1) { strMstType = "I1"; }
                            else { strMstType = "U1"; }

                            string strSql = " usp_SFA001 '" + strMstType + "'";
                            strSql += ", @pDN_NO = '" + txtDnNo.Text + "' ";
                            strSql += ", @pPLANT_CD = '" + cboPlantCd.SelectedValue.ToString() + "' ";
                            strSql += ", @pREQ_DT = '" + dtpReqDt.Text + "' ";
                            strSql += ", @pSHIP_CUST = '" + txtShipCustCd.Text + "' ";
                            if (cboSaleDuty.Text != "")
                                strSql += ", @pSALE_DUTY = '" + cboSaleDuty.SelectedValue.ToString() + "' ";
                            if (cboTranMeth.Text != "")
                                strSql += ", @pTRAN_METH = '" + cboTranMeth.SelectedValue.ToString() + "' ";
                            if (cboMoveType.Text != "")
                                strSql += ", @pMOVE_TYPE = '" + cboMoveType.SelectedValue.ToString() + "' ";
                            strSql += ", @pREMARK = '" + txtRemark.Text + "' ";
                            strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                            strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                            DataTable dt = SystemBase.DbOpen.TranDataTable(strSql, dbConn, Trans);
                            ERRCode = dt.Rows[0][0].ToString();
                            MSGCode = dt.Rows[0][1].ToString();
                            strAutoDnNo = dt.Rows[0][2].ToString();
                            ChkDnNo = dt.Rows[0][2].ToString();

                            if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                            /////////////////////////////////////////////// DETAIL 저장 시작 /////////////////////////////////////////////////
                            //그리드 상단 필수 체크
                            if (SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", false) == true)
                            {
                                //Detail정보를 모두 삭제할 경우 Master정보를 삭제할지 물어보고 아니면 취소한다.
                                if (DelCheck() == false)
                                {
                                    string msg = SystemBase.Base.MessageRtn("B0027");
                                    DialogResult dsMsg = MessageBox.Show(msg, SystemBase.Base.MessageRtn("Z0004"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                                    if (dsMsg == DialogResult.Yes)
                                    {
                                        try
                                        {
                                            string strDelSql = " usp_SFA001  'D1'";
                                            strDelSql += ", @pDN_NO = '" + strAutoDnNo + "' ";
                                            strDelSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                                            DataSet ds2 = SystemBase.DbOpen.TranDataSet(strDelSql, dbConn, Trans);
                                            ERRCode = ds2.Tables[0].Rows[0][0].ToString();
                                            MSGCode = ds2.Tables[0].Rows[0][1].ToString();

                                            if (ERRCode != "OK") { Trans.Rollback(); goto Exit1; }	// ER 코드 Return시 점프

                                            Trans.Commit();
                                        }
                                        catch (Exception f)
                                        {
                                            SystemBase.Loggers.Log(this.Name, f.ToString());
                                            Trans.Rollback();
                                            ERRCode = "ER";
                                            MSGCode = "P0001";	//에러가 발생하여 데이터 처리가 취소되었습니다.
                                        }
                                    Exit1:
                                        dbConn.Close();

                                        if (ERRCode == "OK")
                                        {
                                            MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                                            Search();

                                            //컨트롤 체크값 초기화
                                            strSearchData = "";
                                            //컨트롤 체크 함수
                                            gBox = new GroupBox[] { groupBox2 };
                                            SystemBase.Validation.Control_Check(gBox, ref strSearchData);

                                            //그리드 셀 포커스 이동
                                            UIForm.FPMake.GridSetFocus(fpSpread2, strAutoDnNo, SystemBase.Base.GridHeadIndex(GHIdx2, "요청번호"));

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

                                        return;
                                    }
                                    else
                                    {
                                        Trans.Rollback();
                                        MessageBox.Show(SystemBase.Base.MessageRtn("B0040"), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);//작업이 취소되었습니다.
                                        this.Cursor = Cursors.Default;
                                        return;
                                    }

                                }
                                //행수만큼 처리
                                for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                                {
                                    string strHead = fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text;
                                    string strGbn = "";

                                    if (strHead.Length > 0)
                                    {
                                        switch (strHead)
                                        {
                                            case "U": strGbn = "U2"; break;
                                            case "I": strGbn = "I2"; break;
                                            case "D": strGbn = "D2"; break;
                                            default: strGbn = ""; break;
                                        }

                                        if (strGbn == "U2")
                                        {
                                            strDnSeq = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "요청번호순번")].Value.ToString();
                                        }
                                        else
                                        {
                                            strDnSeq = "0";
                                        }

                                        string strSubSql = " usp_SFA001 '" + strGbn + "'";
                                        strSubSql += ", @pDN_NO = '" + strAutoDnNo + "' ";
                                        strSubSql += ", @pDN_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "요청번호순번")].Value + "' ";
                                        strSubSql += ", @pPLANT_CD = '" + cboPlantCd.SelectedValue.ToString() + "' ";
                                        strSubSql += ", @pITEM_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text + "' ";
                                        strSubSql += ", @pPROJECT_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text + "' ";
                                        strSubSql += ", @pPROJECT_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Value + "' ";
                                        strSubSql += ", @pDN_UNIT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "출고단위")].Text + "' ";
                                        strSubSql += ", @pDN_QTY = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "요청수량")].Value + "' ";
                                        strSubSql += ", @pSL_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "창고코드")].Text + "' ";
                                        strSubSql += ", @pLOCATION_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Location코드")].Text + "' ";
                                        strSubSql += ", @pITEM_UNIT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "재고단위")].Text + "' ";
                                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사구분")].Text == "True")
                                        {
                                            strSubSql += ", @pINSP_FLAG = 'Y' ";
                                        }
                                        else
                                        {
                                            strSubSql += ", @pINSP_FLAG = 'N' ";
                                        }
                                        strSubSql += ", @pSO_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수주번호")].Text + "' ";
                                        strSubSql += ", @pSO_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수주순번")].Value + "' ";
                                        strSubSql += ", @pREMARK = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "비고")].Text + "' ";

                                        strSubSql += ", @pDN_PRICE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수주단가")].Value + "' ";
                                        strSubSql += ", @pDN_AMT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "출고요청금액")].Value + "' ";
                                        strSubSql += ", @pDN_AMT_LOC = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "요청자국금액")].Value + "' ";
                                        strSubSql += ", @pVAT_AMT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "VAT금액")].Value + "' ";
                                        strSubSql += ", @pVAT_AMT_LOC = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "VAT자국금액")].Value + "' ";
                                        strSubSql += ", @pNET_AMT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "공급가액")].Value + "' ";
                                        strSubSql += ", @pNET_AMT_LOC = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "공급자국금액")].Value + "' ";
                                        strSubSql += ", @pTOT_AMT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "총금액")].Value + "' ";
                                        strSubSql += ", @pTOT_AMT_LOC = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "총자국금액")].Value + "' ";

                                        strSubSql += ", @pSHIP_CUST = '" + txtShipCustCd.Text + "' ";
                                        strSubSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                                        strSubSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                                        DataSet ds = SystemBase.DbOpen.TranDataSet(strSubSql, dbConn, Trans);
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
                            this.Cursor = Cursors.Default;
                            ERRCode = "ER";
                            MSGCode = "P0001";	//에러가 발생되어 데이터 처리가 취소되었습니다.
                        }
                    Exit:
                        dbConn.Close();

                        if (ERRCode == "OK")
                        {
                            MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);

                            Search();
                            SubSearch(strAutoDnNo);

                            UIForm.FPMake.GridSetFocus(fpSpread2, strAutoDnNo, SystemBase.Base.GridHeadIndex(GHIdx2, "요청번호"));
                            UIForm.FPMake.GridSetFocus(fpSpread1, strDnSeq, SystemBase.Base.GridHeadIndex(GHIdx1, "요청번호순번"));
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
                    else
                    {
                        MessageBox.Show(SystemBase.Base.MessageRtn("B0038"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);//최소 한건 이상의 DETAIL정보가 존재하지 않으면 등록할 수 없습니다.
                    }
                }
            }
            else
            {
                MessageBox.Show(SystemBase.Base.MessageRtn("S0003"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);//확정된 데이터는 다른 작업을 할 수 없습니다.
            }

            this.Cursor = Cursors.Default;
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

        #region 그리드 상 팝업
        protected override void fpButtonClick(int Row, int Column)
        {
            //창고
            if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "창고코드_2"))
            {
                try
                {
                    string strQuery = " usp_S_COMMON 'S020', @pSPEC1 = '" + cboPlantCd.SelectedValue.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                    string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                    string[] strSearch = new string[] { fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고코드")].Text, "" };

                    UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00004", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "창고 조회");	//창고,LOCATION조회
                    pu.Width = 600;
                    pu.ShowDialog();

                    if (pu.DialogResult == DialogResult.OK)
                    {
                        Regex rx1 = new Regex("#");
                        string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고코드")].Text = Msgs[0].ToString();
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고")].Text = Msgs[1].ToString();
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "Location코드")].Text = Msgs[2].ToString();
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "Location")].Text = Msgs[3].ToString();

                        UIForm.FPMake.fpChange(fpSpread1, Row);//수정플래그
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "창고 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            //LOCATION
            if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "Location코드_2"))
            {
                try
                {
                    string strQuery = " usp_S_COMMON 'S022', @pSPEC1 = '" + fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고코드")].Text + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                    string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                    string[] strSearch = new string[] { fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "Location코드")].Text, "" };

                    UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00004", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "Location 조회");	//LOCATION조회
                    pu.Width = 600;
                    pu.ShowDialog();

                    if (pu.DialogResult == DialogResult.OK)
                    {
                        Regex rx1 = new Regex("#");
                        string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "Location코드")].Text = Msgs[2].ToString();
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "Location")].Text = Msgs[3].ToString();

                        UIForm.FPMake.fpChange(fpSpread1, Row);//수정플래그
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "Location 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        #endregion

        #region 그리드 상 데이터 변경시 연계데이터 자동입력
        private void fpSpread1_Change(object sender, FarPoint.Win.Spread.ChangeEventArgs e)
        {
            //요청수량
            if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "요청수량"))
            {
                double dSoQty = 0, dDnQty = 0, dReqQty = 0, dDnPrice = 0;
                dSoQty = Convert.ToDouble(fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "수주수량")].Value);
                dDnQty = Convert.ToDouble(fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "출고수량")].Value);
                dReqQty = Convert.ToDouble(fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "요청수량")].Value);
                dDnPrice = Convert.ToDouble(fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "수주단가")].Value);

                if (dReqQty > (dSoQty - dDnQty))
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn("S0004", "수주||출고")); //요청수량은 잔량(수주수량 - 출고수량) 보다 같거나 적은 범위내에서 수정 가능합니다.
                }

                string So_No = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "수주번호")].Text;
                int So_Seq = Convert.ToInt32(fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "수주순번")].Value);
                int Dn_Qty = Convert.ToInt32(fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "요청수량")].Value);

                string Query = "usp_SFA001 'C2', @pSO_NO = '" + So_No + "' , @pSO_SEQ = '" + So_Seq + "', @pDN_QTY = '" + Dn_Qty + "' ";
                Query += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);

                if (dt.Rows.Count > 0)
                {
                    fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "출고요청금액")].Value = dt.Rows[0][0];
                    fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "요청자국금액")].Value = dt.Rows[0][1];
                    fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "공급가액")].Value = dt.Rows[0][2];
                    fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "공급자국금액")].Value = dt.Rows[0][3];
                    fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "VAT금액")].Value = dt.Rows[0][4];
                    fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "VAT자국금액")].Value = dt.Rows[0][5];
                    fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "총금액")].Value = dt.Rows[0][6];
                    fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "총자국금액")].Value = dt.Rows[0][7];

                    double dblDnAmt1 = 0, dblDnAmtLoc1 = 0, dblDnNetAmt1 = 0, dblDnVatAmt1 = 0;

                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {
                        dblDnAmt1 += Convert.ToDouble(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "출고요청금액")].Value);
                        dblDnAmtLoc1 += Convert.ToDouble(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "요청자국금액")].Value);
                        dblDnNetAmt1 += Convert.ToDouble(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "공급가액")].Value);
                        dblDnVatAmt1 += Convert.ToDouble(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "VAT금액")].Value);
                    }

                    dtxtDnAmt.Value = dblDnAmt1;
                    dtxtDnAmtLoc.Value = dblDnAmtLoc1;
                    dtxtNetAmtLoc.Value = dblDnNetAmt1;
                    dtxtVatAmtLoc.Value = dblDnVatAmt1;
                }
            }
            //창고
            if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "창고코드"))
            {
                fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고")].Text
                    = SystemBase.Base.CodeName("SL_CD", "SL_NM", "B_STORAGE_INFO", fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고코드")].Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");

                //재고수량 조회
                StockQty(e.Row);
            }
            //location
            if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "Location코드"))
            {
                fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "Location")].Text
                    = SystemBase.Base.CodeName("LOCATION_CD", "LOCATION_NM", "B_LOCATION_INFO", fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "Location코드")].Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");

                //재고수량 조회
                StockQty(e.Row);
            }
        }
        #endregion

        #region 재고수량 조회
        private void StockQty(int Row)
        {
            string strQuery = " usp_SFA001  @pTYPE = 'C1'";
            strQuery += ", @pBIZ_CD = '" + SystemBase.Base.gstrBIZCD.ToString() + "' ";
            strQuery += ", @pPLANT_CD = '" + cboPlantCd.SelectedValue.ToString() + "' ";
            strQuery += ", @pPROJECT_NO = '" + fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text + "' ";
            strQuery += ", @pPROJECT_SEQ = '" + fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Text + "' ";
            strQuery += ", @pSL_CD = '" + fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고코드")].Text + "' ";
            strQuery += ", @pLOCATION_CD = '" + fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "Location코드")].Text + "' ";
            strQuery += ", @pITEM_CD = '" + fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text + "' ";
            strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

            DataTable dt = new DataTable(strQuery);

            if (dt.Rows.Count > 0)
            {
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "재고수량")].Value = dt.Rows[0]["STOCK_QTY"];
            }
            else
            {
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "재고수량")].Value = 0;
            }

        }
        #endregion

        #region textBox 코드 입력시 코드명 자동입력
        //주문처 조회조건
        private void txtSSoldCustCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtSSoldCustCd.Text != "")
                {
                    txtSSoldCustNm.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtSSoldCustCd.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtSSoldCustNm.Value = "";
                }
            }
            catch { }
        }
        //납품처(좌측 그리드 검색조건의 납품처)
        private void txtSShipCustCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtSShipCustCd.Text != "")
                {
                    txtSShipCustNm.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtSShipCustCd.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtSShipCustNm.Value = "";
                }
            }
            catch { }
        }
        //납품처(우측 그리드 출고요청건 상세 정보의 납품처)
        private void txtShipCustCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtShipCustCd.Text != "")
                {
                    // 2015.08.21. hma 수정(Start): 특정일자 기준의 거래처명을 가져오도록 함.
                    //txtShipCustNm.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtShipCustCd.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
                    txtShipCustNm.Value = SystemBase.Base.GetCustName(dtpReqDt.Text, txtShipCustCd.Text);
                    // 2015.08.21. hma 수정(End)
                }
                else
                {
                    txtShipCustNm.Value = "";
                }
            }
            catch { }
        }
        //재고담당자
        private void txtTranDutyId_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtTranDutyId.Text != "")
                {
                    txtTranDutyNm.Value = SystemBase.Base.CodeName("USR_ID", "USR_NM", "B_SYS_USER", txtTranDutyId.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtTranDutyNm.Value = "";
                }
            }
            catch { }
        }
        #endregion

        #region 팝업창 이벤트
        //프로젝트번호
        private void btnSProjectNo_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW.WNDW007 pu = new WNDW.WNDW007(txtSProjectNo.Text);
                pu.MaximizeBox = false;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtSProjectNo.Value = Msgs[3].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "프로젝트번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //주문처
        private void btnSSoldCust_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW.WNDW002 pu = new WNDW.WNDW002(txtSSoldCustCd.Text, "");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtSSoldCustCd.Value = Msgs[1].ToString();
                    txtSSoldCustNm.Value = Msgs[2].ToString();
                    txtSSoldCustCd.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "주문처 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //납품처
        private void btnSShipCust_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW.WNDW002 pu = new WNDW.WNDW002(txtSShipCustCd.Text, "");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtSShipCustCd.Value = Msgs[1].ToString();
                    txtSShipCustNm.Value = Msgs[2].ToString();
                    txtSShipCustCd.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "납품처 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 레포트 출력
        private void btnPreview_Click(object sender, System.EventArgs e)
        {
            if (txtDnNo.Text != "")
            {
                //조회 필수 체크
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {
                    string RptName = SystemBase.Base.ProgramWhere + @"\Report\SFA001.rpt";    // 레포트경로+레포트명
                    string[] RptParmValue = new string[4];   // SP 파라메타 값

                    RptParmValue[0] = "R1";
                    RptParmValue[1] = SystemBase.Base.gstrLangCd;
                    RptParmValue[2] = txtDnNo.Text;
                    RptParmValue[3] = SystemBase.Base.gstrCOMCD;

                    UIForm.PRINT10 frm = new UIForm.PRINT10(this.Text + "출력", null, RptName, RptParmValue); //공통크리스탈 10버전				
                    frm.ShowDialog();
                }
            }
            else
            {
                MessageBox.Show("출고요청번호를 선택하세요.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
        }
        #endregion

        #region 수주참조
        private void btnSoRef_Click(object sender, System.EventArgs e)
        {
            try
            {
                DataTable PopDt = new DataTable();
                SFA001P1 myForm = new SFA001P1(txtDnNo.Text);
                myForm.ShowDialog();

                if (myForm.DialogResult == DialogResult.OK)
                {
                    cboPlantCd.SelectedValue = myForm.cboPlantCd.SelectedValue.ToString();
                    cboMoveType.SelectedValue = myForm.cboMoveType.SelectedValue.ToString();
                    cboSaleDuty.SelectedValue = myForm.cboSaleDuty.SelectedValue.ToString();
                    txtShipCustCd.Value = myForm.txtShipCustCd.Text;
                    txtShipCustNm.Value = myForm.txtShipCustNm.Text;

                    DataTable MyFormDt = new DataTable();
                    MyFormDt = myForm.ReturnDt;

                    if (MyFormDt != null)
                    {
                        fpSpread1.Sheets[0].Rows.Count = 0; //grid초기화

                        int row = 0;

                        for (int i = 0; i < MyFormDt.Rows.Count; i++)
                        {
                            if (MyFormDt.Rows[i]["CHK"].ToString() == "1")
                            {
                                UIForm.FPMake.RowInsert(fpSpread1);//행추가

                                fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text = MyFormDt.Rows[i]["ITEM_CD"].ToString();
                                fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목명")].Text = MyFormDt.Rows[i]["ITEM_NM"].ToString();
                                fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "규격")].Text = MyFormDt.Rows[i]["ITEM_SPEC"].ToString();
                                fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text = MyFormDt.Rows[i]["PROJECT_NO"].ToString();
                                fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Text = MyFormDt.Rows[i]["PROJECT_SEQ"].ToString();
                                fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "출고단위")].Value = MyFormDt.Rows[i]["SO_UNIT"];
                                fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "수주수량")].Value = MyFormDt.Rows[i]["SO_QTY"];
                                fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "출고수량")].Value = MyFormDt.Rows[i]["DN_QTY"];
                                fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "요청수량")].Value = MyFormDt.Rows[i]["BALANCE_QTY"];
                                fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "재고단위")].Value = MyFormDt.Rows[i]["ITEM_UNIT"];
                                fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "재고수량")].Value = MyFormDt.Rows[i]["STOCK_QTY"];
                                fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "수주단가")].Value = MyFormDt.Rows[i]["SO_PRICE"];
                                fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고코드")].Text = MyFormDt.Rows[i]["SL_CD"].ToString();
                                fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고")].Text = MyFormDt.Rows[i]["SL_NM"].ToString();
                                fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "Location코드")].Text = MyFormDt.Rows[i]["LOCATION_CD"].ToString();
                                fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "Location")].Text = MyFormDt.Rows[i]["LOCATION_NM"].ToString();
                                fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "수주번호")].Text = MyFormDt.Rows[i]["SO_NO"].ToString();
                                fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "수주순번")].Value = MyFormDt.Rows[i]["SO_SEQ"];
                                fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "비고")].Text = MyFormDt.Rows[i]["REMARK"].ToString();

                                row++;
                            }
                        }

                        double dblDnAmt = 0, dblDnAmtLoc = 0, dblDnNetAmt = 0, dblDnVatAmt = 0;

                        for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                        {
                            string So_No = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수주번호")].Text;
                            int So_Seq = Convert.ToInt32(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수주순번")].Value);
                            int Dn_Qty = Convert.ToInt32(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "요청수량")].Value);

                            string Query = "usp_SFA001 'C2', @pSO_NO = '" + So_No + "' , @pSO_SEQ = '" + So_Seq + "', @pDN_QTY = '" + Dn_Qty + "' ";
                            Query += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                            DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);

                            if (dt.Rows.Count > 0)
                            {
                                // SELECT @dDN_AMT, @dDN_AMT_LOC, @dNET_AMT, @dNET_AMT_LOC, @dVAT_AMT, @dVAT_AMT_LOC, @dTOT_AMT, @dTOT_AMT_LOC
                                fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "출고요청금액")].Value = dt.Rows[0][0];
                                fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "요청자국금액")].Value = dt.Rows[0][1];
                                fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "공급가액")].Value = dt.Rows[0][2];
                                fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "공급자국금액")].Value = dt.Rows[0][3];
                                fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "VAT금액")].Value = dt.Rows[0][4];
                                fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "VAT자국금액")].Value = dt.Rows[0][5];
                                fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "총금액")].Value = dt.Rows[0][6];
                                fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "총자국금액")].Value = dt.Rows[0][7];

                                dblDnAmt += Convert.ToDouble(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "출고요청금액")].Value);
                                dblDnAmtLoc += Convert.ToDouble(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "요청자국금액")].Value);
                                dblDnNetAmt += Convert.ToDouble(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "공급가액")].Value);
                                dblDnVatAmt += Convert.ToDouble(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "VAT금액")].Value);
                            }
                        }

                        dtxtDnAmt.Value = dblDnAmt;
                        dtxtDnAmtLoc.Value = dblDnAmtLoc;
                        dtxtNetAmtLoc.Value = dblDnNetAmt;
                        dtxtVatAmtLoc.Value = dblDnVatAmt;
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "수주참조 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion
    }
}
