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
using System.Text.RegularExpressions;
using WNDW;

namespace MP.MPO101
{
    public partial class MPO101P2 : UIForm.FPCOMM1
    {
        #region 변수선언
        string strBtn = "N";
        string strPoType = "";
        string returnVal;
        string returnStr;
        FarPoint.Win.Spread.FpSpread spd;
        #endregion

        #region 생성자
        public MPO101P2(FarPoint.Win.Spread.FpSpread spread, string PoType)
        {
            InitializeComponent();
            spd = spread;
            strPoType = PoType;
        }

        public MPO101P2()
        {
            InitializeComponent();
        }
        #endregion

        #region 폼로드 이벤트
        private void MPO101P2_Load(object sender, System.EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            this.Text = "견적팝업";
          
            //버튼 재정의(조회권한만)
            UIForm.Buttons.ReButton("010000001001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

            //기타 세팅
            dtpEstDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString().Substring(0,10);
            dtpEstDtTo.Text = SystemBase.Base.ServerTime("YYMMDD");

            string Query = "SELECT IM_YN FROM M_PO_TYPE(NOLOCK) WHERE PO_TYPE_CD = '" + strPoType + "' AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
            DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);

            strPoType = dt.Rows[0]["IM_YN"].ToString();
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {
                    string sts = "4";
                    if (rdo1.Checked == true) sts = "1";

                    string strQuery = " usp_MPO101  @pTYPE = 'P2'";
                    strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' ";
                    strQuery += ", @pEST_DT_FR = '" + dtpEstDtFr.Text + "' ";
                    strQuery += ", @pEST_DT_TO = '" + dtpEstDtTo.Text + "' ";
                    strQuery += ", @pCUST_CD  = '" + txtCustCd.Text + "' ";
                    strQuery += ", @pPUR_DUTY = '" + txtUserId.Text + "' ";
                    strQuery += ", @pITEM_CD = '" + txtItemCd.Text + "' ";
                    strQuery += ", @pDELIVERY_DT_FR = '" + dtpDeliveryDtFr.Text + "' ";
                    strQuery += ", @pDELIVERY_DT_TO = '" + dtpDeliveryDtTo.Text + "' ";
                    strQuery += ", @pPROJECT_NO = '" + txtProjectNo.Text + "' ";
                    strQuery += ", @pEST_STATUS = '" + sts + "' ";
                    strQuery += ", @pREQ_NO = '" + txtReqNo.Text + "'";
                    strQuery += ", @pEST_NO = '" + txtEstNo.Text + "'";
                    strQuery += ", @pDIV = '" + strPoType + "' ";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 5);
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            this.Cursor = System.Windows.Forms.Cursors.Default;
        }
        #endregion

        #region 버튼 Click
        private void btnOk_Click(object sender, System.EventArgs e)
        {
            string sel = "0";
            int col_sel = SystemBase.Base.GridHeadIndex(GHIdx1, "선택");

            try
            {
                int j = spd.Sheets[0].Rows.Count;
                for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                {
                    if (fpSpread1.Sheets[0].Cells[i, col_sel].Text == "True")
                    {
                        sel = "1";
                        UIForm.FPMake.RowInsert(spd);
                        spd.Sheets[0].RowHeader.Cells[j, 0].Text = "I";
                        spd.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "공장")].Value = SystemBase.Base.gstrPLANT_CD;
                        spd.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "납기일자")].Text
                            = SystemBase.Base.CodeName("ITEM_CD", "SL_CD", "B_PLANT_ITEM_INFO",
                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text, " AND PLANT_CD = '" + SystemBase.Base.gstrPLANT_CD + "' AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");

                        spd.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "창고명")].Text
                            = SystemBase.Base.CodeName("ITEM_CD", "RCPT_LOCATION_CD", "B_PLANT_ITEM_INFO",
                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text, " AND PLANT_CD = '" + SystemBase.Base.gstrPLANT_CD + "' AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");

                        spd.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "요청번호")].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "요청번호")].Text;
                        spd.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "요청순번")].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "요청순번")].Text;
                        spd.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "견적번호")].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "견적번호")].Text;
                        spd.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "견적순번")].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "견적순번")].Text;
                        spd.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text;
                        spd.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "품목명")].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품명")].Text;
                        spd.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "품목규격")].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "규격")].Text;
                        spd.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "규격")].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "규격")].Text;
                        spd.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "요청수량")].Value = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "잔량")].Value;
                        spd.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "발주수량")].Value = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "잔량")].Value;
                        spd.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].Text;
                        spd.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "합계금액")].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "납품가능일")].Text;
                        spd.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "위치명")].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text;
                        spd.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Text;
                        spd.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수")].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품질증명여부")].Text;
                        spd.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "품질증빙코드")].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품질증명")].Text;

                        spd.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "단가")].Value = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "결정단가")].Value; // 단가 
                        spd.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "단가구분")].Value = "T";//진단가	
                        spd.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "종전단가")].Value = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "결정금액")].Value; ; // 견적금액
                        spd.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "견적금액")].Value = 0; // NEGO금액
                        spd.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "NEGO금액")].Value = 0; // 원가금액 
                        // 2019.03.05. hma 수정: 원가금액=>원가단가로 항목명 변경
                        spd.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "원가단가")].Value = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "결정금액")].Value; // 발주금액 
                        spd.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "원가검토단가")].Value = 0;      // 2019.03.06. hma 추가
                        spd.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "발주금액")].Value = "2";//별도

                        if (strPoType == "Y") //외자직수입
                        {
                            spd.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "VAT포함구분")].Value = "C";//영세율
                            spd.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "VAT유형")].Text = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", "C", " AND MAJOR_CD = 'B040' AND LANG_CD ='" + SystemBase.Base.gstrLangCd + "' AND COMP_CODE = '" + SystemBase.Base.gstrCOMCD + "'");
                            spd.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "VAT유형명")].Value = 0;//VAT율 0
                        }
                        else
                        {
                            spd.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "VAT포함구분")].Value = "A";//일반세금계산서
                            spd.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "VAT유형")].Text = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", "A", " AND MAJOR_CD = 'B040' AND LANG_CD ='" + SystemBase.Base.gstrLangCd + "' AND COMP_CODE = '" + SystemBase.Base.gstrCOMCD + "'");
                            spd.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "VAT유형명")].Value = 10;//VAT율 10
                        }
                        spd.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "VAT율")].Value = 0;//VAT금액0
                        spd.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "공급가액")].Value = 0;//공급가액0
                        spd.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "VAT금액")].Value = 0;//합계금액0 
                        spd.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "MOQ여부")].Text = "N";  //MOQ
                        spd.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "비고")].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "채택사유")].Text;
                        j++;
                    }
                }
                if (sel == "1")
                {
                    RtnStr("Y", txtCustCd.Text.Trim());
                }
                else
                {
                    RtnStr("N", "");
                }
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
            RtnStr("N", "");
            Close();
            this.DialogResult = DialogResult.OK;
        }
        #endregion

        #region 값 전송
        public string ReturnVal { get { return returnVal; } set { returnVal = value; } }
        public string ReturnStr { get { return returnStr; } set { returnStr = value; } }

        public void RtnStr(string strCode, string strValue)
        {
            returnVal = strCode;
            returnStr = strValue;
        }
        #endregion

        #region 버튼 Click
        private void btnUser_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                string strQuery = " usp_B_COMMON 'B011' ,@pSPEC1='" + SystemBase.Base.gstrBIZCD + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtUserId.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00031", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "사용자 팝업");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {

                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtUserId.Text = Msgs[0].ToString();
                    txtUserNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
            strBtn = "N";
        }

        private void fpSpread1_ButtonClicked(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            fpSpread1.Sheets[0].RowHeader.Cells[e.Row, 0].Text = "";
        }

        private void butCust_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                WNDW002 pu = new WNDW002(txtCustCd.Text, "P");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtCustCd.Text = Msgs[1].ToString();
                    txtCustNm.Value = Msgs[2].ToString();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            strBtn = "N";
        }

        private void butItem_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                WNDW005 pu = new WNDW005(SystemBase.Base.gstrPLANT_CD, true);
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
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
            strBtn = "N";
        }

        private void btnProj_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW007 pu = new WNDW007(txtProjectNo.Text, "N");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;
                    txtProjectNo.Text = Msgs[3].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        //구매요청번호
        private void btnReqNo_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = "usp_MRQ499 @pTYPE = 'P1'";
                string[] strWhere = new string[] { "@pCODE", "" };
                string[] strSearch = new string[] { txtReqNo.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00085", strQuery, strWhere, strSearch, new int[] { 0 }, "구매요청번호 조회");
                pu.Width = 600;
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtReqNo.Text = Msgs[0].ToString();

                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "구매요청번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);	//데이터 조회 중 오류가 발생하였습니다.

            }
        }

        //견적의뢰번호
        private void btnEstNo_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = "usp_M_COMMON @pTYPE = 'M050', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "" };
                string[] strSearch = new string[] { txtEstNo.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00092", strQuery, strWhere, strSearch, new int[] { 0 }, "견적의뢰번호 조회");
                pu.Width = 600;
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtEstNo.Text = Msgs[0].ToString();

                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "견적의뢰번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);	//데이터 조회 중 오류가 발생하였습니다.

            }
        }
        #endregion

        #region TextChanged
        private void txtUserId_Leave(object sender, System.EventArgs e)
        {
            try
            {
                if (strBtn == "N" && txtUserId.Text.Trim() != "")
                {
                    string temp = "";
                    temp = SystemBase.Base.CodeName("PUR_DUTY", "PUR_DUTY", "M_PUR_DUTY", txtUserId.Text, " AND USE_YN = 'Y' AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "' ");
                    if (temp != "")
                    {
                        if (txtUserId.Text != "")
                        {
                            txtUserNm.Value = SystemBase.Base.CodeName("USR_ID", "USR_NM", "B_SYS_USER", txtUserId.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                        }
                        else
                        {
                            txtUserNm.Value = "";
                        }
                    }
                    else
                    {
                        MessageBox.Show(SystemBase.Base.MessageRtn("M0001"));  //구매담당자가 아닙니다
                        txtUserId.Text = "";
                        txtUserNm.Value = "";
                        txtUserId.Focus();
                    }
                }                
            }
            catch
            {

            }
        }

        private void txtCustCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (strBtn == "N")
                {
                    if (txtCustCd.Text != "")
                    {
                        txtCustNm.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtCustCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                    }
                    else
                    {
                        txtCustNm.Value = "";
                    }
                }
            }
            catch
            {

            }
        }

        private void txtItemCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (strBtn == "N")
                {
                    if (txtItemCd.Text != "")
                    {
                        txtItemNm.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtItemCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                    }
                    else
                    {
                        txtItemNm.Value = "";
                    }
                }
            }
            catch
            {

            }
        }
        #endregion

        #region rdo_CheckedChanged
        private void rdo1_CheckedChanged(object sender, System.EventArgs e)
        {
            if (rdo1.Checked == true)
            {
                fpSpread1.Sheets[0].ColumnHeader.Cells[0, 7].Text = "의뢰수량";
            }
            else
            {
                fpSpread1.Sheets[0].ColumnHeader.Cells[0, 7].Text = "채택수량";
            }

        }

        private void rdo3_CheckedChanged(object sender, System.EventArgs e)
        {
            if (rdo3.Checked == true)
            {
                fpSpread1.Sheets[0].ColumnHeader.Cells[0, 7].Text = "채택수량";
            }
            else
            {
                fpSpread1.Sheets[0].ColumnHeader.Cells[0, 7].Text = "의뢰수량";
            }
        }
        #endregion


    }
}
