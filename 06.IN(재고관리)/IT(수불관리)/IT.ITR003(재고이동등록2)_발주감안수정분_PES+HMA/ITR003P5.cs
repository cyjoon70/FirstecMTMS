#region 작성정보
/**********************************************************************************************/
// 단위업무명: 발주감안등록참조 팝업
// 작 성 자 :  박은수
// 작 성 일 :  
// 작성내용 :  발주감안등록된 구매오더의 입고 데이터를 이용하여 이동 대상 선택하도록 함.
// 수 정 일 :  2017.03.15
// 수 정 자 : 
// 수정내용 : 
// 비    고 :
/**********************************************************************************************/
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

namespace IT.ITR003
{
	public partial class ITR003P5 : UIForm.FPCOMM2
	{

        #region 변수선언
        // 2017.09.11. hma 수정(Start)
        //string returnVal;
        // decimal returnRef;
        string[] returnVal = null;
        // 2017.09.11. hma 수정(End)        
        
        string strReqNo;                    // 요청번호
        string strReqSeq;                   // 요청순번
        string strTranNo = "";				// 수불번호
        string strTranSeq = "";				// 수불순번

        string strItemCd;
        string strItemNm;
        decimal dReqQty;
        decimal dRefQty;
        decimal sum = 0;
        bool first = false;
        
        string strBtn = "N";
        bool locked = false;
        bool isCheck = true;					// 요구수량 대 참조수량 체크
        bool isDetail_save = false;				// 디테일 저장여부
		string strLotYn = string.Empty;			// lot 추적 여부
		string strProjectNo  = string.Empty;	// 요청 프로젝트번호
		string strProjectSeq = string.Empty;	// 요청 프로젝트차수
		string strRefYN = string.Empty;			// 재고감안여부

        FarPoint.Win.Spread.FpSpread spd;

        #endregion

		#region 생성자
        public ITR003P5(string ProjectNo, string ProjectSeq, FarPoint.Win.Spread.FpSpread fpSpread)
        {
            InitializeComponent();
			strProjectNo  = ProjectNo;
			strProjectSeq = ProjectSeq;
            spd = fpSpread;
        }

        public ITR003P5()
        {
            InitializeComponent();
        }
		#endregion

		#region Form Load 시
		private void ITR003P5_Load(object sender, EventArgs e)
		{
			this.Text = "발주감안참조 팝업";

			SystemBase.Validation.GroupBox_Setting(groupBox1);
			SystemBase.Validation.GroupBox_Setting(groupBox2);

			UIForm.Buttons.ReButton("010000001000", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);

			SystemBase.ComboMake.C1Combo(cboPlantCd, "usp_B_COMMON @pType='B031', @pSPEC1 = '" + SystemBase.Base.gstrBIZCD + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 9);//공장
			cboPlantCd.SelectedValue = SystemBase.Base.gstrPLANT_CD.ToString();
			
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
            UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, 0, 0, false);

			strTranNo = "";
			first = true;

            dteMvmtDtFrom.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-3).ToShortDateString();
            dteMvmtDtTo.Text = SystemBase.Base.ServerTime("YYMMDD");

            txtProjectNo2.Text = strProjectNo;
            txtProjectSeq2.Text = strProjectSeq;

			//SearchExec();     // 2017.09.11. hma 수정: 주석 처리
		}
        #endregion

		#region SearchExec() 그리드 조회 로직
		protected override void SearchExec()
		{
			this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox2))     // 2017.09.12. hma 추가: 필수항목 체크
                {
                    // 2017.09.12. hma 추가(Start): 이동입고 프로젝트/차수가 존재하는지 체크
                    string strPrjChk = "Y";
                    string strQueryPrj = "usp_ITR003 @pTYPE = 'S7'";
                    strQueryPrj += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                    strQueryPrj += ", @pMOV_PROJECT_NO = '" + txtProjectNo2.Text + "'";    //요청 프로젝트번호 (이동입고 프로젝트)
                    strQueryPrj += ", @pMOV_PROJECT_SEQ = '" + txtProjectSeq2.Text + "'";  //요청 프로젝트차수 (이동입고 차수)

                    DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQueryPrj);

                    if (dt != null && dt.Rows.Count > 0 && dt.Rows[0][0].ToString() == "ER")
                    {
                        strPrjChk = "N";
                        MessageBox.Show(dt.Rows[0][1].ToString());
                        this.Cursor = System.Windows.Forms.Cursors.Default;
                        return;
                    }

                    if (strPrjChk == "Y")
                    {
                    // 2017.09.12. hma 추가(End)
                        string strQuery = "usp_ITR003 @pTYPE = 'P5'";
                        strQuery += ", @pMOV_PROJECT_NO = '" + txtProjectNo2.Text + "'";    //요청 프로젝트번호 (이동입고 프로젝트)
                        strQuery += ", @pMOV_PROJECT_SEQ = '" + txtProjectSeq2.Text + "'";  //요청 프로젝트차수 (이동입고 차수)

                        strQuery += ", @pTRAN_DT_FR = '" + dteMvmtDtFrom.Text + "'";    //창고입고일
                        strQuery += ", @pTRAN_DT_TO = '" + dteMvmtDtTo.Text + "'";      //창고입고일
                        strQuery += ", @pTRAN_NO = '" + txtMvmtNo.Text + "'";
                        strQuery += ", @pITEM_CD = '" + txtItemCd.Text + "'";
                        strQuery += ", @pPROJECT_NO = '" + txtProjNo.Text + "'";        // 입고 프로젝트번호
                        strQuery += ", @pPROJECT_SEQ = '" + txtProjSeq.Text + "'";      // 입고 프로젝트차수
                        strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'";
                        strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                        strQuery += ", @pPLANT_CD = '" + cboPlantCd.SelectedValue.ToString() + "'";

                        UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0);

                        for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                        {
                            if (Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "이동잔량")].Value) == 0)
                            {
                                fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "이동수량")].Locked = true;
                            }
                        }
                    }
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


		#region 버튼 Click
        private void btnOk_Click(object sender, System.EventArgs e)
        {
            int col_sel = SystemBase.Base.GridHeadIndex(GHIdx1, "선택");

            if (fpSpread1.Sheets[0].Rows.Count > 0)
            {
                int j = spd.Sheets[0].Rows.Count;
                for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                {
                    if (fpSpread1.Sheets[0].Cells[i, col_sel].Text == "True")
                    {

                        spd.Sheets[0].ActiveRowIndex = spd.Sheets[0].RowCount;

                        UIForm.FPMake.RowInsert(spd);
                        spd.Sheets[0].Rows.Count = j + 1;
                        spd.Sheets[0].RowHeader.Cells[j, 0].Text = "I";

                        spd.Sheets[0].Cells[j, 2].Value = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text;			//품목코드
                        spd.Sheets[0].Cells[j, 4].Value = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목명")].Text;			//품명
                        spd.Sheets[0].Cells[j, 5].Value = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "규격")].Text;				//규격

                        spd.Sheets[0].Cells[j, 6].Value = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].Value;			//재고단위
                        spd.Sheets[0].Cells[j, 7].Value = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "이동수량")].Value;		//발주감안 이동수량
                        spd.Sheets[0].Cells[j, 8].Value = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "재고수량")].Value;		//수불수량
                        spd.Sheets[0].Cells[j, 9].Value = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "변경품목코드")].Value;	//변경품목코드

                        spd.Sheets[0].Cells[j, 11].Value = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text;	//프로젝트번호
                        spd.Sheets[0].Cells[j, 13].Value = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Text;			//프로젝트차수

                        //spd.Sheets[0].Cells[j, 16].Value = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No")].Value;			//Lot No       
                        spd.Sheets[0].Cells[j, 15].Value = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot 추적")].Value;		//Lot 추적 여부

                        spd.Sheets[0].Cells[j, 20].Value = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "창고")].Text;			//창고        
                        spd.Sheets[0].Cells[j, 22].Value = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "창고명")].Text;			//창고명      
                        //spd.Sheets[0].Cells[j, 23].Value = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "위치")].Text;			//창고위치   
                        //spd.Sheets[0].Cells[j, 25].Value = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "위치명")].Text;			//위치명  

                        spd.Sheets[0].Cells[j, 26].Value = 0;		//출고단가(재고단가)
                        spd.Sheets[0].Cells[j, 27].Value = 0;       //출고금액
                        spd.Sheets[0].Cells[j, 30].Value = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "이동수량")].Value;		//기존수량

                        spd.Sheets[0].Cells[j, 39].Value = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "요청번호")].Text;		//요청번호
                        spd.Sheets[0].Cells[j, 40].Value = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "요청순번")].Text;		//요청순번
                        spd.Sheets[0].Cells[j, 41].Value = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수불번호")].Text;		//수불번호
                        spd.Sheets[0].Cells[j, 42].Value = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수불순번")].Text;		//수불순번

                        //spd.Sheets[0].Cells[j, 31].Value = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "바코드")].Value;		//바코드         
                        //spd.Sheets[0].Cells[j, 32].Value = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "입고번호")].Value;		//입고번호     
                        //spd.Sheets[0].Cells[j, 33].Value = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "입고순번")].Value;		//입고순번     

                        j++;

                    }
                }

                RtnStr("Y");       // 2017.09.11. hma 추가: 리턴값 지정

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }


		private void butCancel_Click(object sender, System.EventArgs e)
		{
			try
			{
                // 2017.09.11. hma 수정(Start)
                //RtnStr("N", 0);
                RtnStr("N");
                // 2017.09.11. hma 수정(End)
                Close();
				this.DialogResult = DialogResult.OK;
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				DialogResult dsMsg = MessageBox.Show(f.Message, SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		#endregion

		#region 값 전송
		public string[] ReturnVal { get { return returnVal; } set { returnVal = value; } }
    
        // 2017.09.11. hma 수정(Start): 문자열 변수값 전송되도록 함.
		//public decimal ReturnRef { get { return returnRef; } set { returnRef = value; } }

		//public void RtnStr(string strCode, decimal strValue)
		//{
		//	returnVal = strCode;
		//	returnRef = strValue;
		//}
        public void RtnStr(string strCode)
        {
            returnVal = new string[3];
            returnVal[0] = strCode;
            returnVal[1] = txtProjectNo2.Text;
            returnVal[2] = txtProjectSeq2.Text;
        }
        // 2017.09.11. hma 수정(End)
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
                        if (fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "이동수량")].Locked == false)
                            fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "이동수량")].Value = 0;
                    }
                    else
                    {
                        if (Convert.ToDecimal(fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "이동수량")].Value) == 0)
                        {
                            fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "이동수량")].Value =
                                    Convert.ToDecimal(fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "이동잔량")].Value);
                        }
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(f.Message, SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }


        private void fpSpread1_Change(object sender, FarPoint.Win.Spread.ChangeEventArgs e)
        {
            fpSpread1.Sheets[0].Cells[e.Row, 0].Text = "";
            fpSpread1.Sheets[0].RowHeader.Cells[e.Row, 0].Text = "";
        }


        #region 상세정보 조회
        private void SubSearch(string strTranNo, string strTranSeq, string strReqNo, string strReqSeq)
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                fpSpread2.Sheets[0].Rows.Count = 0;

                string strSql = " usp_ITR003  @pTYPE = 'P6' "; 
                strSql = strSql + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                strSql = strSql + ", @pTRAN_NO_B = '" + strTranNo + "' ";
                strSql = strSql + ", @pTRAN_SEQ_B = '" + strTranSeq + "' ";
                strSql = strSql + ", @pREQ_NO = '" + strReqNo + "' ";
                strSql = strSql + ", @pREQ_SEQ = '" + strReqSeq + "' ";
                strSql = strSql + ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread2, strSql, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, true, 0, 0);

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
            
            this.Cursor = Cursors.Default;
        }
        #endregion


        #region 조건버튼 Click
        private void btnMvmtNo_Click(object sender, EventArgs e)
        {
            try
            {
                WNDW019 dialog = new WNDW019();
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string[] Msgs = dialog.ReturnVal;
                    txtMvmtNo.Text = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(f.Message, SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnPoNo_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = "usp_MIM518 @pTYPE = 'P1', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pOUT_PO_NO", "" };
                string[] strSearch = new string[] { txtPoNo.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00078", strQuery, strWhere, strSearch, new int[] { 0 }, "발주번호 조회");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtPoNo.Value = Msgs[0].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "발주번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnItemCd_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pTYPE = 'P030', @pPLANT_CD = '" + SystemBase.Base.gstrPLANT_CD + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };
                string[] strSearch = new string[] { txtItemCd.Text, txtItemNm.Text };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00001", strQuery, strWhere, strSearch, "품목코드 조회", new int[] { 1, 2 }, true);
                pu.Width = 500;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    txtItemCd.Value = pu.ReturnValue[1].ToString();
                    txtItemNm.Value = pu.ReturnValue[2].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목코드 조회"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
        private void txtItemCd_TextChanged(object sender, EventArgs e)
        {
            if (txtItemCd.Text != "") txtItemNm.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtItemCd.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "'");
            else txtItemNm.Value = "";
        }

        private void txtProjNo_TextChanged(object sender, EventArgs e)
        {
            txtProjNm.Value = SystemBase.Base.CodeName("PROJECT_NO", "PROJECT_NM", "S_SO_MASTER", txtProjNo.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
        }

		private void txtTranDuty_TextChanged(object sender, System.EventArgs e)
		{

		}

		private void txtProjSeq_Leave(object sender, System.EventArgs e)
		{
			if (strBtn == "N" && txtProjSeq.Text != "*")
				txtProjSeq.Text = SystemBase.Base.CodeName("PROJECT_NO", "PROJECT_SEQ", "P_MPS_REGISTER", txtProjNo.Text, " AND PROJECT_SEQ = '" + txtProjSeq.Text + "' AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");

		}


        #endregion


        #region fpSpread1_CellDoubleClick(): 셀 더블클릭시 우측 그리드에 재고이동 내역 보여줌.
        private void fpSpread1_CellDoubleClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            if (fpSpread1.Sheets[0].Rows.Count > 0)
            {
                try
                {
                    int intRow = fpSpread1.Sheets[0].GetSelection(0).Row;
                    strReqNo = fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "요청번호")].Text;
                    strReqSeq = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "요청순번")].Text).ToString();
                    strTranNo = fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "수불번호")].Text;
                    strTranSeq = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "수불순번")].Text).ToString();

                    //c1DockingTab1.SelectedIndex = 0;
                    SubSearch(strTranNo, strTranSeq, strReqNo, strReqSeq);
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //데이터 조회 중 오류가 발생하였습니다.				
                }
            }
        }
        #endregion


        // 2017.09.12. hma 추가(Start): 이동프로젝트 및 이동차수 항목을 직접 입력할 수 있도록 하고 선택 가능하도록 함.
        private void btnProj2_Click(object sender, EventArgs e)
        {
            try
            {
                WNDW007 pu = new WNDW007(txtProjectNo2.Text, "A");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;
                    txtProjectNo2.Value = Msgs[3].ToString();
                    if (txtProjectSeq2.Text != "*") txtProjectSeq2.Value = "";
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        private void btnProjSeq2_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON 'PROJ_SEQ', @pSPEC1 = '" + txtProjectNo2.Text + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";  // 쿼리
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };			// 쿼리 인자값(조회조건)
                string[] strSearch = new string[] { "", "" };		// 쿼리 인자값에 들어갈 데이타

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P09001", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "프로젝트차수 조회", false);
                pu.Width = 400;
                pu.ShowDialog();	//공통 팝업 호출

                if (pu.DialogResult == DialogResult.OK)
                {
                    string MSG = pu.ReturnVal.Replace("|", "#");
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(MSG);
                    txtProjectSeq2.Value = Msgs[0].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }
        // 2017.09.12. hma 추가(End)
    }
}
