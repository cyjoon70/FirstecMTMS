#region 작성정보
/*********************************************************************/
// 단위업무명 : 출고처리
// 작 성 자 : 조  홍  태
// 작 성 일 : 2013-02-25
// 작성내용 : 출고처리
// 수 정 일 : 2014-09-26
// 수 정 자 : 최 용 준
// 수정내용 : LOT별 출고 기능 추가
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


namespace SF.SFA002
{
    public partial class SFA002 : UIForm.FPCOMM2
    {
        #region 변수선언
        int PreRow = -1;   // SelectionChanged 시에 동일 Row에서 데이타변환 처리 안하도록 하기 위함.
        string strAutoDnNo = "";
		DataTable dt = new DataTable();			// lot 분할 팝업 그리드 정보 데이터 테이블
        #endregion

        #region 생성자
        public SFA002()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void SFA002_Load(object sender, System.EventArgs e)
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

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
            UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, 0, 0, false);

            //폼 컨트롤 초기화
            Control_Setting();
        }
        #endregion

        #region ControlSetting()
        private void Control_Setting()
        {
            //기타 세팅
            dtpSReqDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString();
            dtpSReqDtTo.Text = SystemBase.Base.ServerTime("YYMMDD");
            dtpActualDt.Text = SystemBase.Base.ServerTime("YYMMDD");
            dtpSDeliveryDtFr.Value = null;
            dtpSDeliveryDtTo.Value = null;

            rdoAll.Checked = true;
            chkProcessBn.Checked = false;
            chkProcessCollect.Checked = false;
            dtpActualDt.Enabled = true;
            dtpActualDt.Text = SystemBase.Base.ServerTime("YYMMDD");

            btnBnOk.Enabled = false;
            btnBnCancel.Enabled = false;
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            SystemBase.Validation.GroupBox_Reset(groupBox2);

            strAutoDnNo = "";

            SystemBase.Validation.GroupBoxControlsLock(groupBox2, false);

            fpSpread1.Sheets[0].Rows.Count = 0;
            fpSpread2.Sheets[0].Rows.Count = 0;

            //폼 컨트롤 초기화
            Control_Setting();

			dt.Clear();

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

                    string strQuery = " usp_SFA002  @pTYPE = 'S1'";
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
                    strQuery += ", @pDN_NO = '" + txtSDnNo.Text + "'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    strQuery += ", @pREF_DELV_DT_FR = '" + dtpRefDelvDtFr.Text + "' ";      // 2017.11.10. hma 추가: 납기일(참조) FROM
                    strQuery += ", @pREF_DELV_DT_TO = '" + dtpRefDelvDtTo.Text + "' ";      // 2017.11.10. hma 추가: 납기일(참조) TO

                    UIForm.FPMake.grdCommSheet(fpSpread2, strQuery, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, true, 0, 0, true);

                    SystemBase.Validation.GroupBox_Reset(groupBox2);
                    fpSpread1.Sheets[0].Rows.Count = 0;

					dt.Clear();
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

                    strAutoDnNo = fpSpread2.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx2, "요청번호")].Text.ToString();//요청번호

                    chkProcessBn.Checked = false;
                    chkProcessCollect.Checked = false;

					dt.Clear();
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
                string strSql = " usp_SFA002  'S2', @pDN_NO = '" + Code + "' ";
                strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                DataTable dt = SystemBase.DbOpen.NoTranDataTable(strSql);

                if (dt.Rows.Count > 0)
                {
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
                    dtpReqDt.Enabled = false;
                    dtpReqDt.Tag = ";2;;";

                    if (dt.Rows[0]["ACTUAL_DT"].ToString() != "" && dt.Rows[0]["ACTUAL_DT"] != null)
                    {
                        dtpActualDt.ReadOnly = false;
                        dtpActualDt.Enabled = true;
                        dtpActualDt.Value = dt.Rows[0]["ACTUAL_DT"].ToString().Substring(0, 10);
                    }
                    else
                    {
                        dtpActualDt.Value = SystemBase.Base.ServerTime("YYMMDD");
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

                    txtProjectNm.Value = dt.Rows[0]["PROJECT_NM"].ToString();
                    int DnStatus = Convert.ToInt32(dt.Rows[0]["DN_STATUS"]);

                    if (dt.Rows[0]["BN_YN"].ToString() == "Y")
                    { chkProcessBn.Checked = true; }
                    if (dt.Rows[0]["TAX_YN"].ToString() == "Y")
                    { chkProcessCollect.Checked = true; }

                    //출고요청Master정보
                    string strQuery = " usp_SFA002  'C1', @pDN_NO = '" + Code + "' ";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    DataTable AmtDt = SystemBase.DbOpen.NoTranDataTable(strQuery);

                    if (AmtDt.Rows.Count > 0)
                    {
                        dtxtDnAmt.Value = AmtDt.Rows[0]["DN_AMT"];
                        dtxtDnAmtLoc.Value = AmtDt.Rows[0]["DN_AMT_LOC"];
                        dtxtNetAmtLoc.Value = AmtDt.Rows[0]["NET_AMT"];
                        dtxtVatAmtLoc.Value = AmtDt.Rows[0]["VAT_AMT"];
                    }
                   
                    //현재 row값 설정
                    PreRow = fpSpread2.ActiveSheet.ActiveRowIndex;

                    SystemBase.Validation.GroupBox_SearchViewValidation(groupBox2); //Key값 컨트롤 세팅

                    //출고요청Detail그리드 정보.
                    string strSql1 = " usp_SFA002  'S3' , @pDN_NO = '" + Code + "', @pBIZ_CD = '" + SystemBase.Base.gstrBIZCD + "' ";
                    strSql1 += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    UIForm.FPMake.grdCommSheet(fpSpread1, strSql1, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);

                    //출고여부에 따른 화면 Locking
                    if (ConfirmChk == true)
                    {
                        //Detail Locking설정
                        SystemBase.Validation.GroupBoxControlsLock(groupBox2, true);

                        chkProcessBn.Enabled = false;
                        chkProcessCollect.Enabled = false;

                        if (Convert.ToInt32(DnStatus) > 1)
                        {
                            btnBnOk.Enabled = false;
                            btnBnCancel.Enabled = false;
                        }
                        else
                        {
                            btnBnOk.Enabled = false;
                            btnBnCancel.Enabled = true;
                        }

						fpSpread1.Sheets[0].Columns[SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No_2")].Locked = true;
                    }
                    else
                    {
                        //Detail Locking해제
                        SystemBase.Validation.GroupBoxControlsLock(groupBox2, false);

                        chkProcessBn.Enabled = true;
                        chkProcessCollect.Enabled = true;

                        btnBnOk.Enabled = true;
                        btnBnCancel.Enabled = false;
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
        //납품처
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
        //납품처
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

                    txtSProjectNo.Text = Msgs[3].ToString();
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

                    txtSSoldCustCd.Text = Msgs[1].ToString();
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

                    txtSShipCustCd.Text = Msgs[1].ToString();
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
        //재고담당자
        private void btnTranDuty_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON 'B010', @pSPEC1 = '" + SystemBase.Base.gstrBIZCD + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtTranDutyId.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P04003", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "재고 담당자 조회");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtTranDutyId.Value = Msgs[0].ToString();
                    txtTranDutyNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "재고 담당자 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 체크박스 이벤트
        private void chkProcessCollect_CheckedChanged(object sender, System.EventArgs e)
        {
            if (chkProcessCollect.Checked == true)
            {
                chkProcessBn.Checked = true;
            }
        }

        private void chkProcessBn_CheckedChanged(object sender, System.EventArgs e)
        {
            if (chkProcessBn.Checked == false)
            {
                chkProcessCollect.Checked = false;
            }
        }
        #endregion

        #region 출고처리, 출고취소

        //출고처리
        private void btnBnOk_Click(object sender, System.EventArgs e)
        {
            string msg = SystemBase.Base.MessageRtn("SY068", "출고번호 : " + txtDnNo.Text);
            DialogResult dsMsg = MessageBox.Show(msg, SystemBase.Base.MessageRtn("Z0004"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dsMsg == DialogResult.Yes)
            {
                // 2017.10.20. hma 추가(Start): 출고 처리시 매출채권 항목에 체크를 하지 않은 경우 맞는지 확인 메시지 띄워서 확인하도록 함.
                if (chkProcessBn.Checked == false)
                { 
                    msg = "매출채권 항목이 체크되지 않았습니다. 매출채권 생성 없이 출고처리하시겠습니까?";
                    DialogResult dsMsg1 = MessageBox.Show(msg, SystemBase.Base.MessageRtn("Z0004"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (dsMsg1 == DialogResult.No)
                    {
                        this.Cursor = Cursors.Default;
                        return;
                    }
                }
                // 2017.10.20. hma 추가(End)
                DnConfirm("Y");
            }
        }

        //출고취소
        private void btnBnCancel_Click(object sender, System.EventArgs e)
        {
            string msg = SystemBase.Base.MessageRtn("SY069", "출고번호 : " + txtDnNo.Text);
            DialogResult dsMsg = MessageBox.Show(msg, SystemBase.Base.MessageRtn("Z0004"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dsMsg == DialogResult.Yes)
            {
                DnConfirm("N");
            }
        }
        
		//출고처리 함수
        private void DnConfirm(string strConfirmYn)
        {
            this.Cursor = Cursors.WaitCursor;

			DataTable dtResult = new DataTable();
            string ERRCode = "ER", MSGCode = "P0000";	//처리할 내용이 없습니다.
            SqlConnection dbConn = SystemBase.DbOpen.DBCON();
            SqlCommand cmd = dbConn.CreateCommand();
            SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

			try
			{
				string strProcessBn = "N", strProcessCollect = "N";

				if (string.Compare(strConfirmYn, "Y", true) == 0 && CheckLot() == false)
				{
					MessageBox.Show("Lot 추적 대상 품목은\r\n반드시 Lot별 출고를 해야 합니다.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
					this.Cursor = Cursors.Default;
					return;
				}

                // 출고취소이면 기 등록된 출하번호를 저장 후 lot 출고취소에서 사용한다.
                if (string.Compare(strConfirmYn, "N", true) == 0)
				{
					string strQSql = "SELECT TRAN_NO, TRAN_SEQ, DN_SEQ FROM I_ITEM_MVMT_DETAIL (NOLOCK) WHERE CO_CD = '" + SystemBase.Base.gstrCOMCD + "' AND DN_NO = '" + txtDnNo.Text + "' ORDER BY DN_SEQ";
					dtResult = SystemBase.DbOpen.NoTranDataTable(strQSql);
				}

				if (chkProcessBn.Checked == true)
				{ strProcessBn = "Y"; }
				if (chkProcessCollect.Checked == true)
				{ strProcessCollect = "Y"; }

				string strSql = " usp_SFA002  'P1', @pBIZ_CD = '" + SystemBase.Base.gstrBIZCD + "'";
				strSql += ", @pDN_NO = '" + txtDnNo.Text + "' ";
				strSql += ", @pDN_YN = '" + strConfirmYn + "' ";
				strSql += ", @pACTUAL_DT = '" + dtpActualDt.Text + "' ";
				strSql += ", @pTRAN_DUTY = '" + txtTranDutyId.Text + "' ";
				strSql += ", @pCHK_PROCESS_BN = '" + strProcessBn + "' ";
				strSql += ", @pCHK_PROCESS_COLLECT = '" + strProcessCollect + "' ";
				if (cboSaleDuty.Text != "")
				{
					strSql += ", @pSALE_DUTY = '" + cboSaleDuty.SelectedValue.ToString() + "' ";
				}
				strSql += ", @pREQ_DT = '" + dtpReqDt.Text + "' ";

				strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "' ";
				strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

				DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
				ERRCode = ds.Tables[0].Rows[0][0].ToString();
				MSGCode = ds.Tables[0].Rows[0][1].ToString();

				if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

				// Lot 출고처리
				if (string.Compare(strConfirmYn, "Y", true) == 0)
				{

					// 저장된 수불번호 조회 후 Lot별 처리
					strSql = "SELECT TRAN_NO, TRAN_SEQ, DN_SEQ FROM I_ITEM_MVMT_DETAIL (NOLOCK) WHERE CO_CD = '" + SystemBase.Base.gstrCOMCD + "' AND DN_NO = '" + txtDnNo.Text + "' ORDER BY DN_SEQ";
					dtResult = SystemBase.DbOpen.NoTranDataTable(strSql);

					if (dtResult.Rows.Count > 0)
					{
						for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
						{
							for (int k = 0; k <= dtResult.Rows.Count - 1; k++)
							{
								if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "요청번호순번")].Text == dtResult.Rows[k]["DN_SEQ"].ToString())
								{
									if (dt.Rows.Count > 0)
									{
										for (int j = 0; j <= dt.Rows.Count - 1; j++)
										{
											if (Convert.ToDecimal(dt.Rows[j]["OUT_QTY"]) > 0 &&
												dt.Rows[j]["DN_SEQ"].ToString() == dtResult.Rows[k]["DN_SEQ"].ToString() )
											{
												strSql = "usp_T_OUT_INFO_CUDR ";
												strSql += "  @pTYPE        = 'I1'";
												strSql += ", @pCO_CD       = '" + SystemBase.Base.gstrCOMCD + "' ";
												strSql += ", @pPLANT_CD    = '" + SystemBase.Base.gstrPLANT_CD + "' ";
												strSql += ", @pBAR_CODE    = '" + dt.Rows[j]["BAR_CODE"].ToString() + "' ";
												strSql += ", @pMVMT_NO     = '" + dt.Rows[j]["MVMT_NO"].ToString() + "' ";
												strSql += ", @pMVMT_SEQ    = '" + dt.Rows[j]["MVMT_SEQ"].ToString() + "' ";
												strSql += ", @pOUT_TRAN_NO = '" + dtResult.Rows[k]["TRAN_NO"].ToString() + "' ";
												strSql += ", @pOUT_TRAN_SEQ= '" + dtResult.Rows[k]["DN_SEQ"].ToString() + "' ";
												strSql += ", @pITEM_CD     = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text + "' ";
												strSql += ", @pTR_TYPE     = 'O' ";
												strSql += ", @pOUT_DATE    = NULL ";
												strSql += ", @pLOT_NO      = '" + dt.Rows[j]["LOT_NO"].ToString() + "' ";
												strSql += ", @pOUT_PROJECT_NO  = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text + "' ";
												strSql += ", @pOUT_PROJECT_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Text + "' ";
												strSql += ", @pOUT_QTY     = '" + dt.Rows[j]["OUT_QTY"].ToString() + "' ";
												strSql += ", @pSTOCK_UNIT  = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "재고단위")].Text + "' ";
												strSql += ", @pREMARK      = '' ";
												strSql += ", @pIN_ID       = '" + SystemBase.Base.gstrUserID + "' ";
												strSql += ", @pUP_ID       = '" + SystemBase.Base.gstrUserID + "' ";
												strSql += ", @pPROC_SEQ	   ='" + dt.Rows[j]["PROC_SEQ"].ToString() + "'";
												strSql += ", @pDN_NO	   = '" + txtDnNo.Text + "' ";
												strSql += ", @pDN_SEQ	   ='" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "요청번호순번")].Text + "'";

												DataSet ds4 = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
												ERRCode = ds4.Tables[0].Rows[0][0].ToString();
												MSGCode = ds4.Tables[0].Rows[0][1].ToString();
												if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
											}
										}
									}
								}
							}
						}
					}
				}
				// Lot 출고취소
				else
				{
					if (dtResult.Rows.Count > 0)
					{
						for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
						{
							for (int k = 0; k <= dtResult.Rows.Count - 1; k++)
							{
								if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "요청번호순번")].Text == dtResult.Rows[k]["DN_SEQ"].ToString())
								{
									strSql = "usp_T_OUT_INFO_CUDR ";
									strSql += "  @pTYPE        = 'D2'";
									strSql += ", @pCO_CD       = '" + SystemBase.Base.gstrCOMCD + "' ";
									strSql += ", @pPLANT_CD    = '" + SystemBase.Base.gstrPLANT_CD + "' ";
									strSql += ", @pOUT_TRAN_NO = '" + dtResult.Rows[k]["TRAN_NO"].ToString() + "' ";
									strSql += ", @pOUT_TRAN_SEQ= '" + dtResult.Rows[k]["TRAN_SEQ"].ToString() + "' ";
									strSql += ", @pOUT_PROJECT_NO  = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text + "' ";
									strSql += ", @pOUT_PROJECT_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Text + "' ";
									strSql += ", @pIN_ID       = '" + SystemBase.Base.gstrUserID + "' ";
									strSql += ", @pUP_ID       = '" + SystemBase.Base.gstrUserID + "' ";
									strSql += ", @pDN_NO	   = '" + txtDnNo.Text + "' ";
									strSql += ", @pDN_SEQ	   ='" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "요청번호순번")].Text + "'";

									DataSet ds4 = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
									ERRCode = ds4.Tables[0].Rows[0][0].ToString();
									MSGCode = ds4.Tables[0].Rows[0][1].ToString();
									if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
								}
							}
						}
					}
				}
				
				Trans.Commit();
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				Trans.Rollback();
				MSGCode = "ER";
				MSGCode = "P0001";	//에러가 발생하여 데이터 처리가 취소되었습니다.
			}
			finally 
			{
				this.Cursor = Cursors.Default;
			}
        Exit:
            dbConn.Close();

            if (ERRCode == "OK")
            {
                MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
				dt.Clear();
				SubSearch(txtDnNo.Text);
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

        #endregion

		#region Grid Button Click Event
		private void fpSpread1_ButtonClicked(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
		{
			try
			{
				if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No_2"))
				{
					SetLotOut(e.Row);
				}
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		#endregion

		#region Lot별 출고처리
		private void SetLotOut(int row)
		{
			decimal dSum = 0;
			decimal dOutQty = 0;
			string strLotNo = string.Empty;
			int iLotCount = 0;

			SFA002P1 pu = new SFA002P1();

			pu.strPLANT_CD = SystemBase.Base.gstrPLANT_CD;
			pu.strPROJECT_NO = fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Value.ToString();
			pu.strPROJECT_SEQ = fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Value.ToString();
			pu.strITEM_CD = fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Value.ToString();
			pu.strITEM_NM = fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목명")].Value.ToString();
			pu.strITEM_SPEC = fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "규격")].Value.ToString();
			pu.strREM_QTY = fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "요청수량")].Value.ToString();
			pu.strDN_SEQ = fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "요청번호순번")].Value.ToString();

			pu.ShowDialog();

			if (pu.DialogResult == DialogResult.OK)
			{

				if (dt.Rows.Count > 0)
				{
					if (pu.dt != null && pu.dt.Rows.Count > 0)
					{
						for (int i = 0; i <= pu.dt.Rows.Count - 1; i++)
						{
							DataRow dr = pu.dt.Rows[i];
							dt.Rows.Add(dr.ItemArray);
						}
					}
				}
				else
				{
					if (pu.dt != null && pu.dt.Rows.Count > 0)
					{
						dt = pu.dt;
					}
				}

				// 단일 lot no 구분
				for (int i = 0; i <= pu.dt.Rows.Count - 1; i++)
				{
					if (pu.dt.Rows[0]["OUT_QTY"] == DBNull.Value) { pu.dt.Rows[0]["OUT_QTY"] = 0; }
					dSum += Convert.ToDecimal(pu.dt.Rows[i]["OUT_QTY"]);

					if (Convert.ToDecimal(pu.dt.Rows[i]["OUT_QTY"]) > 0)
					{
						iLotCount++;
						dOutQty = Convert.ToDecimal(pu.dt.Rows[i]["OUT_QTY"]);
						strLotNo = pu.dt.Rows[i]["LOT_NO"].ToString();
					}
				}

				if (iLotCount == 1)
				{
					fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No")].Value = strLotNo;
					fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "출고수량")].Value = dOutQty;
				}
				else
				{
					fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No")].Value = "Lot 분할";
					fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "출고수량")].Value = dSum;
				}
				
			}

			pu.dLotSum = 0;

		}
		#endregion

		#region Lot 추적 대상 품목의 경우, Lot별 출고 여부 체크
		private bool CheckLot()
		{
			bool bReturn = true;
			int iCnt = 0;

			if (fpSpread1.Sheets[0].Rows.Count > 0)
			{
				for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
				{
					if (string.Compare(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot 추적")].Text, "True", true) == 0 &&
						string.IsNullOrEmpty(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No")].Text))
					{
						iCnt++; 
					}
				}
			}

			if (iCnt > 0) { bReturn = false; }

			return bReturn;
		}
		#endregion
	}
}
