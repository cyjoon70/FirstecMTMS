#region 작성정보
/*********************************************************************/
// 단위업무명 :  발주감안등록
// 작 성 자   :  박 은 수
// 작 성 일   :  2016.12.26
// 작성내용   :  발주감안등록 대상 발주선택 및 수량 입력
// 수 정 일   :  2017.04.01. ~
// 수 정 자   :  한 미 애
// 수정내용   :  
// 비    고   :
/*********************************************************************/
#endregion

using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using WNDW;

namespace MR.MRQ005
{
    public partial class MRQ005P1 : UIForm.FPCOMM1
    {
        #region 변수선언
        string returnVal;
        decimal returnRef;
        string strReqNo;
        string strReqSeq;
        string strItemCd;
        string strItemNm;
        decimal dReqQty;
        decimal dRefQty;
        decimal dRefAvaQty;                     // 2017.07.05. hma 추가: 참조가능수량
        decimal sum = 0;
        string strTranNo = "";					// 수불번호
        string strMovTranNo = "";				// 수불순번
        string strBtn = "N";
        bool locked = false;
        bool isCheck = true;					// 요구수량 대 참조수량 체크
        bool isDetail_save = false;				// 디테일 저장여부
		string strProjectNo  = string.Empty;	// 요청 프로젝트번호
		string strProjectSeq = string.Empty;	// 요청 프로젝트차수
		string strRefYN = string.Empty;			// 재고감안여부
        #endregion

		#region 생성자
		public MRQ005P1(string ReqNo, string ReqSeq, string ItemCd, string ItemNm, decimal ReqQty, decimal RefQty, bool locking,  string ProjectNo, string ProjectSeq, string Ref_YN, decimal RefAvaQty)
        {
            //
            // Windows Form 디자이너 지원에 필요합니다.
            //
            InitializeComponent();
            strReqNo = ReqNo;
            strReqSeq = ReqSeq;
            strItemCd = ItemCd;
            strItemNm = ItemNm;
            dReqQty = ReqQty;
            dRefQty = RefQty;
            dRefAvaQty = RefAvaQty;     // 2017.07.05. hma 추가: 참조가능수량

            locked = locking;
			strProjectNo  = ProjectNo;
			strProjectSeq = ProjectSeq;
			strRefYN = Ref_YN;
        }

        public MRQ005P1()
        {
            InitializeComponent();
        }
		#endregion

		#region Form Load 시
		private void MRQ005P1_Load(object sender, System.EventArgs e)
        {
            this.Text = "발주참조팝업";

            SystemBase.Validation.GroupBox_Setting(groupBox1);
            SystemBase.Validation.GroupBox_Setting(groupBox2);

            UIForm.Buttons.ReButton("010000001000", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);

			SystemBase.ComboMake.C1Combo(cboPlantCd, "usp_B_COMMON @pType='B031', @pSPEC1 = '" + SystemBase.Base.gstrBIZCD + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 9);//공장
			cboPlantCd.SelectedValue = SystemBase.Base.gstrPLANT_CD.ToString();

            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "공장")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='B030', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ", 0);
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "창고")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='SL', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 0);
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "창고위치")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='LOC', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ", 0);
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "요청단위")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'Z005', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ", 0);

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0, true);

            txtItemCd.Value = strItemCd;
            txtItemNm.Value = strItemNm;
            txtReqNo.Value = strReqNo;
            txtReqSeq.Value = strReqSeq;

            if (dReqQty != 0)
                txtReqQty.Value = dReqQty;
            else
                txtReqQty.Value = 0;

            if (dRefQty != 0)
                txtRefQty.Value = dRefQty;
            else
                txtRefQty.Value = 0;

            txtRefAvaQty.Value = dRefAvaQty;    // 2017.07.05. hma 추가

            txtReqQty.Enabled = false;
            txtRefQty.Enabled = false;
            txtRefAvaQty.Enabled = false;       // 2017.07.05. hma 추가

            strTranNo = "";
            strMovTranNo = "";

            SearchExec();
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            try
            {
                string strQuery = " usp_MRQ005 ";
				strQuery += "  @pTYPE = 'P1'";                				
				strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' ";
                strQuery += ", @pITEM_CD = '" + strItemCd + "' ";
                strQuery += ", @pPLANT_CD = '" + cboPlantCd.SelectedValue + "' ";
                strQuery += ", @pREQ_NO = '" + strReqNo + "' ";
                strQuery += ", @pREQ_SEQ = '" + strReqSeq + "' ";
                strQuery += ", @pPROJECT_NO = '" + txtProjNo.Text + "' ";
                strQuery += ", @pPROJECT_SEQ = '" + txtProjSeq.Text + "' ";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' ";
                strQuery += ", @pREQ_PO_REF_YN = '" + strRefYN + "' ";

				UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0);

                Sum_RefQty();

				if (fpSpread1.Sheets[0].Rows.Count == 0)
				{
					this.Close();
				}

                if (txtTranDuty.Text == "")
                {
                    txtTranDutyNm.Value = SystemBase.Base.CodeName("USR_ID", "USR_NM", "B_SYS_USER", SystemBase.Base.gstrUserID, " AND TRAN_YN = 'Y' AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                    if (txtTranDutyNm.Text != "") txtTranDuty.Text = SystemBase.Base.gstrUserID;

                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }

            this.Cursor = System.Windows.Forms.Cursors.Default;
        }
        #endregion


        #region SaveExec_Detail() 폼에 입력된 발주감안 데이터를 발주감안정보에 저장
        private string SaveExec_Detail()
        {			
			this.Cursor = Cursors.WaitCursor;

			strTranNo = "";
			strMovTranNo = "";

			string strTranSeq = "";					
			string strMovTranSeq = "0";
			
            if (isCheck != true) return "ER";

            //그리드 상단 필수 체크
            if (UIForm.FPMake.FPUpCheck(fpSpread1, false) == true && SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox2))
            {
                string ERRCode = "", MSGCode = "";
                int cnt = 0;
                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

				try
				{
					//행수만큼 처리
					for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
					{
						string strHead = fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text;
						//string strGbn = "";       // 2017.07.11. hma 주석 처리

						strTranNo = "";
						strMovTranNo = "0";

                        // 2017.07.11. hma 수정: 선택 항목에 체크된 건만 처리되도록 && 조건 추가함.
						if ((strHead.Length > 0) && (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "선택")].Text == "True"))
						{
							fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text = "";

                            // 2017.07.11. hma 주석 처리: 매개변수를 U2로 딱 지정하므로 선택 항목 체크하는게 의미 없으므로.
                            //if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "선택")].Text == "True")
                            //    strGbn = "U2";
                            //else
                            //    strGbn = "D2";

                            string strSql = " usp_MRQ005 @pTYPE = 'U2'";  // + strGbn + "'";
							strSql += ", @pREQ_NO = '" + strReqNo + "'";
							strSql += ", @pREQ_SEQ = " + strReqSeq;
							strSql += ", @pITEM_CD = '" + strItemCd + "'";
							strSql += ", @pPLANT_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "공장")].Value + "'";
							strSql += ", @pPO_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "발주번호")].Text + "'";
							strSql += ", @pPO_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "발주순번")].Text + "'";
                            strSql += ", @pPO_QTY =  " + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "발주수량")].Value;
                            strSql += ", @pREQ_QTY =  " + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "요청수량")].Value;
                            strSql += ", @pREQ_SUM =  " + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "기감안수량")].Value;
                            strSql += ", @pPO_REF_QTY =  " + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "참조수량")].Value;
                            strSql += ", @pPROJECT_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트")].Text + "'";
                            strSql += ", @pPROJECT_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Text + "'";
							strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
							strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

							DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
							ERRCode = ds.Tables[0].Rows[0][0].ToString();
							MSGCode = ds.Tables[0].Rows[0][1].ToString();
							if (ERRCode != "OK") { Trans.Rollback(); strTranNo = ""; strMovTranNo = ""; cnt++; goto Exit; }	// ER 코드 Return시 점프

							if (ERRCode == "OK")
							{
								strTranNo = ds.Tables[0].Rows[0][2].ToString();
								strMovTranNo = ds.Tables[0].Rows[0][3].ToString();
								strTranSeq = ds.Tables[0].Rows[0][4].ToString();	// T_IN_INFO.IN_TRAN_NO,	T_IN_INFO.MVMT_NO
								strMovTranSeq = ds.Tables[0].Rows[0][5].ToString();	// T_IN_INFO.IN_TRAN_SEQ,	T_IN_INFO.MVMT_SEQ
							}

							isDetail_save = true;
						}
					}

					if (cnt == 0) Trans.Commit();
				}
				catch (Exception e)
				{
					SystemBase.Loggers.Log(this.Name, e.ToString());
					Trans.Rollback();
					strTranNo = ""; 
					strMovTranNo = "";
					ERRCode = "ER";
					MSGCode = e.Message;
					//MSGCode = "P0001";	//에러가 발생되어 데이터 처리가 취소되었습니다.
				}
				finally
				{
					this.Cursor = Cursors.Default;
				}

            Exit:
                dbConn.Close();
                if (ERRCode == "ER")
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (ERRCode == "WR")
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                return ERRCode;
            }
            else
            {
                return "ER";
            }

            return "OK";
        }
        #endregion


        #region SaveExec_Master() 발주감안된 수량을 구매요청 데이터에 저장
        private string SaveExec_Master()
        {
            // 그리드 상단 필수항목 체크
            if (UIForm.FPMake.FPUpCheck(fpSpread1, false) == true)
            {
                string ERRCode = "WR", MSGCode = "P0000"; //처리할 내용이 없습니다.

                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                try
                {
                    string strSql = " usp_MRQ005 'U3'";
                    strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";       // 2017.06.30. hma 추가
                    strSql += ", @pREQ_NO = '" + strReqNo + "'";
                    strSql += ", @pREQ_SEQ = " + strReqSeq;
                    strSql += ", @pITEM_CD = '" + txtItemCd.Text + "'";
                    strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";

                    DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                    ERRCode = ds.Tables[0].Rows[0][0].ToString();
                    MSGCode = ds.Tables[0].Rows[0][1].ToString();

                    if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

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
                DialogResult dsMsg;
                if (ERRCode == "ER")
                    dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return ERRCode;
            }
            else
            {
                return "ER";
            }

            return "OK";
        }
        #endregion


        #region 버튼 Click
        private void btnOk_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (SaveExec_Detail() == "ER")
                   return;
                if (isCheck != true) return;
                if (isDetail_save)
                {
                    if (SaveExec_Master() == "ER") return;
                }
                Sum_RefQty();

                if (isDetail_save)      // 2017.08.31. hma 추가: 발주감안 처리 결과 체크하고 리턴값 지정되도록 함.
                    RtnStr("Y", sum);
                else
                    RtnStr("N", 0);     // 2017.08.31. hma 추가: 발주감안 저장이 안된 경우엔 결과값을 N으로 가져가도록 함.
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(f.Message, SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            Close();
            this.DialogResult = DialogResult.OK;
        }

        private void butCancel_Click(object sender, System.EventArgs e)
        {
            RtnStr("N", 0);
            Close();
            this.DialogResult = DialogResult.OK;
        }
        #endregion

        #region 값 전송
        public string ReturnVal { get { return returnVal; } set { returnVal = value; } }
        public decimal ReturnRef { get { return returnRef; } set { returnRef = value; } }

        public void RtnStr(string strCode, decimal strValue)
        {
            returnVal = strCode;
            returnRef = strValue;
        }
        #endregion


        private void Sum_RefQty()
        {
            int Col = SystemBase.Base.GridHeadIndex(GHIdx1, "선택");      // 2017.08.31. hma 추가: 선택 체크한 경우만 합산하도록 함.
            sum = 0;
        
            for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
            {
                if (fpSpread1.Sheets[0].Cells[i, Col].Text == "True")       // 2017.08.31. hma 추가: 선택 체크한 경우만 합산하도록 함.
                    sum += Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "참조수량")].Value);

                //발주수량과 입고수량이 같으면 수정 못하게 함. 2017.03.24
                if (Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "발주수량")].Value) == Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "입고수량")].Value))
                {
                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "참조수량")].Locked = true;
                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "발주미입고량")].BackColor = Color.FromName("WhiteSmoke"); 
                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "참조수량")].BackColor = Color.FromName("WhiteSmoke");                        
                }
            }

            txtRefQty.Enabled = true;
            txtRefQty.Value = sum;
            txtRefQty.Enabled = false;

            if (dRefAvaQty < sum)       // 2017.07.05. hma 수정: 요청수량이 아닌 참조가능수량으로 비교하도록 함.dReqQty => dRefAvaQty
            {
                DialogResult dsMsg = MessageBox.Show("참조수량이 참조가능수량보다 많습니다!", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);

                if (dRefQty != 0)
                    txtRefQty.Value = dRefQty;
                else
                    txtRefQty.Value = 0;

                isCheck = false;

                for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                {
                    if (fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text == "U")
                    {
                        //fpSpread1.Sheets[0].Cells[i, Col].Value = 0;
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "참조수량")].Value = 0;
                        isCheck = true;
                    }
                }
            }
            else
            {
                isCheck = true;
            }
        }


        #region fpSpread1_Change
        protected override void fpSpread1_ChangeEvent(int Row, int Column)
        {
            int Col = SystemBase.Base.GridHeadIndex(GHIdx1, "참조수량");
            if (Column == Col)
            {
                if (Convert.ToDecimal(fpSpread1.Sheets[0].Cells[Row, Col].Value) >
                    Convert.ToDecimal(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "발주미입고량")].Value) +
                    Convert.ToDecimal(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "이전참조수량")].Value)        // 이전 참조수량 2017.03.23 참조수량을 변경할때 사용(남은수량과 이전 참조되었던 수량을 더한 만큼 수정가능한 량 이므로)
                    )
                {
                    DialogResult dsMsg = MessageBox.Show("참조수량이 발주미입고량보다 클 수 없습니다!", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    fpSpread1.Sheets[0].Cells[Row, Col].Value = 0;
                    fpSpread1.ActiveSheet.SetActiveCell(Row, Col);
                }
                else
                {
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "선택")].Value = 1;      // 2017.08.31. hma 주석 해제
                }

                Sum_RefQty();
            }
        }
        #endregion

        #region MRQ005P1_Activated
        private void MRQ005P1_Activated(object sender, System.EventArgs e)
        {
            //			if(first) 	SearchExec(); 
            //			first = false;
        }
        #endregion

        #region 조건버튼 Click
        private void btnTranDuty_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                string strQuery = "usp_I_COMMON @pTYPE= 'I012', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtTranDuty.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P01008", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "수불담당자 조회");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtTranDuty.Text = Msgs[0].ToString();
                    txtTranDutyNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
            strBtn = "N";
        }

        private void btnProj_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                WNDW007 pu = new WNDW007(txtProjNo.Text);
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;
                    txtProjNo.Text = Msgs[3].ToString();
                    if (txtProjSeq.Text != "*") txtProjSeq.Text = "";
                }        
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
            strBtn = "N";
        }

        private void btnProjSeq_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                string strQuery = " usp_B_COMMON 'PROJ_SEQ', @pSPEC1 = '" + txtProjNo.Text + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";										// 쿼리
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

                    txtProjSeq.Text = Msgs[0].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
            strBtn = "N";
        }
        #endregion

        #region TextChange
        private void txtTranDuty_TextChanged(object sender, System.EventArgs e)
        {
            if (strBtn == "N")
                txtTranDutyNm.Value = SystemBase.Base.CodeName("USR_ID", "USR_NM", "B_SYS_USER", txtTranDuty.Text, " AND TRAN_YN = 'Y' AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
        }

        private void txtProjSeq_Leave(object sender, System.EventArgs e)
        {
            if (strBtn == "N" && txtProjSeq.Text != "*")
                txtProjSeq.Text = SystemBase.Base.CodeName("PROJECT_NO", "PROJECT_SEQ", "P_MPS_REGISTER", txtProjNo.Text, " AND PROJECT_SEQ = '" + txtProjSeq.Text + "' AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");

        }
        #endregion


        private void fpSpread1_ButtonClicked(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            try
            {
                int Col = SystemBase.Base.GridHeadIndex(GHIdx1, "선택");
                if (e.Column == Col)
                {
                    if (fpSpread1.Sheets[0].Cells[e.Row, Col].Text == "False")
                    {
                        if (fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "참조수량")].Locked == false)
                            fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "참조수량")].Value = 0;
                    }
                    else
                    {
                        if (Convert.ToDecimal(fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "참조수량")].Value) == 0)
                        {
                            // 확인 항목에 체크시
                            if (Convert.ToDecimal(fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "발주미입고량")].Value) >= (dRefAvaQty - sum))
                                fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "참조수량")].Value = dRefAvaQty - sum;
                            else
                                fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "참조수량")].Value
                                    = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "발주미입고량")].Value;
                        }
                    }
                }

                Sum_RefQty();
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(f.Message, SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }
    }
}
