#region 작성정보
/*********************************************************************/
// 단위업무명 : 작업일보등록
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-03-22
// 작성내용 : 작업일보등록 및 관리
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
    public partial class MPO101P1 : UIForm.FPCOMM1
    {
        #region 변수선언
        string strBtn = "N";
        string strPoType = "";
        string returnVal;
        FarPoint.Win.Spread.FpSpread spd;
        #endregion

        #region 생성자
        public MPO101P1(FarPoint.Win.Spread.FpSpread spread, string PoType)
        {        
            InitializeComponent();
            spd = spread;
            strPoType = PoType;
        }

        public MPO101P1()
        {
            InitializeComponent();
        }
        #endregion

        #region 폼로드 이벤트
        private void MPO101P1_Load(object sender, System.EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            this.Text = "구매요청팝업";
                        
            //버튼 재정의(조회권한만)
            UIForm.Buttons.ReButton("010000001001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

            //기타 세팅
            dtpReqDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString().Substring(0,10);
            dtpReqDtTo.Text = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0,10);

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
                string strReqPart = "";
                if (rdoPartM.Checked == true) { strReqPart = "M"; }
                else if (rdoPartS.Checked == true) { strReqPart = "S"; }


                string strReqType = "";
                if (rdoTypeM.Checked == true) { strReqType = "M"; }
                else if (rdoTypeE.Checked == true) { strReqType = "E"; }

                string strQuery = " usp_MPO101  @pTYPE = 'P1'";
                strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' ";
                strQuery += ", @pREQ_DT_FR = '" + dtpReqDtFr.Text + "' ";
                strQuery += ", @pREQ_DT_TO = '" + dtpReqDtTo.Text + "' ";
                strQuery += ", @pREQ_PART = '" + strReqPart + "' ";
                strQuery += ", @pREQ_TYPE = '" + strReqType + "' ";
                strQuery += ", @pREQ_ID = '" + txtUserId.Text + "' ";
                strQuery += ", @pREQ_DEPT_CD = '" + txtReqDeptCd.Text + "' ";
                strQuery += ", @pREQ_REORG_ID = '" + txtReqReorgId.Text + "' ";
                strQuery += ", @pPROJECT_NO = '" + txtProjectNo.Text + "' ";
                strQuery += ", @pREQ_NO = '" + txtReqNo.Text + "' ";
                strQuery += ", @pDIV = '" + strPoType + "' ";
                strQuery += ", @pITEM_CD = '" + txtItemCd.Text + "'";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 5);
               
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
            int col_sel = SystemBase.Base.GridHeadIndex(GHIdx1, "선택");

            try
            {
                int j = spd.Sheets[0].Rows.Count;
                for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                {
                    if (fpSpread1.Sheets[0].Cells[i, col_sel].Text == "True")
                    {
                        spd.Sheets[0].ActiveRowIndex = spd.Sheets[0].RowCount;

                        UIForm.FPMake.RowInsert(spd);
                        spd.Sheets[0].RowHeader.Cells[j, 0].Text = "I";
                        spd.Sheets[0].Cells[j, 3].Value = SystemBase.Base.gstrPLANT_CD;
                        spd.Sheets[0].Cells[j, 4].Value = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목계정")].Text;

                        spd.Sheets[0].Cells[j, 34].Text         // 2022.05.24. hma 수정: 32=>34로
                            = SystemBase.Base.CodeName("ITEM_CD", "SL_CD", "B_PLANT_ITEM_INFO",
                                fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text, " AND PLANT_CD = '" + SystemBase.Base.gstrPLANT_CD + "' AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");

                        spd.Sheets[0].Cells[j, 37].Text         // 2022.05.24. hma 수정: 35=>37로
                            = SystemBase.Base.CodeName("ITEM_CD", "RCPT_LOCATION_CD", "B_PLANT_ITEM_INFO",
                                fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text, " AND PLANT_CD = '" + SystemBase.Base.gstrPLANT_CD + "' AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");

                        spd.Sheets[0].Cells[j, 47].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "요청번호")].Text; // 2022.05.24. hma 수정: 45=>47로
                        spd.Sheets[0].Cells[j, 48].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "요청순번")].Text; // 2022.05.24. hma 수정: 46=>48로
                        spd.Sheets[0].Cells[j, 49].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "요청부서")].Text; // 2022.05.24. hma 수정: 47=>49로
                        spd.Sheets[0].Cells[j, 50].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "요청자")].Text;   // 2022.05.24. hma 수정: 48=>50로

                        spd.Sheets[0].Cells[j, 5].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text;
                        spd.Sheets[0].Cells[j, 7].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품명")].Text;
                        spd.Sheets[0].Cells[j, 8].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "규격")].Text;
                        // 2019.01.22. hma 수정: 구매요청 비고 항목이 발주등록 '규격' 항목에 나오도록 규격=>비고로 변경함.
                        spd.Sheets[0].Cells[j, 9].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "비고")].Text;

                        spd.Sheets[0].Cells[j, 10].Value = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "잔량")].Value;
                        spd.Sheets[0].Cells[j, 11].Value = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "잔량")].Value;
                        spd.Sheets[0].Cells[j, 12].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].Text;
                        spd.Sheets[0].Cells[j, 33].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "요청납기일")].Text;     // 2022.05.24. hma 수정: 31=>33로
                        spd.Sheets[0].Cells[j, 40].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text;   // 2022.05.24. hma 수정: 38=>40로
                        spd.Sheets[0].Cells[j, 42].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Text;           // 2022.05.24. hma 수정: 40=>42로
                        spd.Sheets[0].Cells[j, 43].Text = "N";  //품질증명      // 2019.03.05. hma 수정: 40=>41로 변경   // 2022.05.24. hma 수정: 41=>43로(43이면 차수 버튼임. 뭔지 정확히 모르겠으나 일단 2 증가 처리)

                        spd.Sheets[0].Cells[j, 14].Value = 0;   // 단가 
                        spd.Sheets[0].Cells[j, 16].Value = "T"; //진단가	
                        spd.Sheets[0].Cells[j, 19].Value = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "종전단가")].Value;   // 2022.05.24. hma 수정: 17=>19로
                        spd.Sheets[0].Cells[j, 20].Value = 0;   // 견적금액     // 2022.05.24. hma 수정: 18=>20로
                        spd.Sheets[0].Cells[j, 21].Value = 0;   // NEGO금액     // 2022.05.24. hma 수정: 19=>21로
                        spd.Sheets[0].Cells[j, 22].Value = 0;   // 원가단가     // 2022.05.24. hma 수정: 20=>22로
                        spd.Sheets[0].Cells[j, 23].Value = 0;   // 2019.03.06. hma 추가: 원가검토단가   // 2022.05.24. hma 수정: 21=>23로
                        spd.Sheets[0].Cells[j, 24].Value = 0;   // 발주금액     // 2022.05.24. hma 수정: 22=>24로

                        spd.Sheets[0].Cells[j, 25].Value = "2"; //별도          // 2022.05.24. hma 수정: 23=>25로

                        if (strPoType == "Y") //외자직수입
                        {
                            spd.Sheets[0].Cells[j, 26].Value = "C";//영세율    // 2022.05.24. hma 수정: 24=>26로
                            spd.Sheets[0].Cells[j, 28].Text = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", "C", " AND MAJOR_CD = 'B040' AND LANG_CD ='" + SystemBase.Base.gstrLangCd + "' AND COMP_CODE = '" + SystemBase.Base.gstrCOMCD + "'");  // 2022.05.24. hma 수정: 26=>28로
                            spd.Sheets[0].Cells[j, 29].Value = 0;//VAT율 0     // 2022.05.24. hma 수정: 27=>29로
                        }
                        else
                        {
                            spd.Sheets[0].Cells[j, 26].Value = "A";//일반세금계산서    // 2022.05.24. hma 수정: 24=>26로
                            spd.Sheets[0].Cells[j, 28].Text = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", "A", " AND MAJOR_CD = 'B040' AND LANG_CD ='" + SystemBase.Base.gstrLangCd + "' AND COMP_CODE = '" + SystemBase.Base.gstrCOMCD + "'");  // 2022.05.24. hma 수정: 26=>28로
                            spd.Sheets[0].Cells[j, 29].Value = 10;//VAT율 10   // 2022.05.24. hma 수정: 27=>29로
                        }
                        spd.Sheets[0].Cells[j, 30].Value = 0;//공급가액금액0  // 2022.05.24. hma 수정: 28=>30로
                        spd.Sheets[0].Cells[j, 31].Value = 0;//VAT금액0       // 2022.05.24. hma 수정: 29=>31로
                        spd.Sheets[0].Cells[j, 32].Value = 0;//합계금액0      // 2022.05.24. hma 수정: 30=>32로
                        spd.Sheets[0].Cells[j, 53].Text = "N";  //MOQ   // 2019.03.05. hma 수정: 53=>51로 변경 // 2022.05.24. hma 수정: 51=>53로 
                        j++;
                        RtnStr("Y");
                    }
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
            RtnStr("N");
            Close();
            this.DialogResult = DialogResult.OK;
        }
        #endregion

        #region 값 전송
        public string ReturnVal { get { return returnVal; } set { returnVal = value; } }

        public void RtnStr(string strCode)
        {
            returnVal = strCode;
        }
        #endregion

        #region 버튼 Click  TextChanged
        private void btnUser_Click(object sender, System.EventArgs e)
        {
            try
            {
                strBtn = "Y";
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
                strBtn = "N";
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        private void butReqDept_Click(object sender, System.EventArgs e)
        {
            try
            {
                strBtn = "Y";
                string strQuery = " usp_B_COMMON 'D011' ,@pSPEC1='" + SystemBase.Base.gstrBIZCD + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtReqDeptCd.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P04004", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "부서 팝업");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {

                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtReqDeptCd.Text = Msgs[0].ToString();
                    txtReqDeptNm.Value = Msgs[1].ToString();
                    txtReqReorgId.Text = Msgs[3].ToString();
                }
                strBtn = "N";
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
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

        //품목코드
        private void btnItemCd_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW005 pu = new WNDW005(SystemBase.Base.gstrPLANT_CD, true, txtItemCd.Text);
                pu.MaximizeBox = false;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtItemCd.Text = Msgs[2].ToString();
                    txtItemNm.Value = Msgs[3].ToString();

                    txtItemCd.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);	//데이터 조회 중 오류가 발생하였습니다.

            }
        }

        private void txtUserId_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (strBtn == "N")
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
            }
            catch
            {

            }
        }

        private void txtReqDeptCd_TextChanged(object sender, System.EventArgs e)
        {            
            try
            {
                if (strBtn == "N")
                {
                    string Query = " usp_B_COMMON 'D021', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                    DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);

                    if (dt.Rows.Count > 0)
                    {
                        txtReqReorgId.Text = dt.Rows[0][0].ToString();
                    }
                    else
                    {
                        txtReqReorgId.Text = "";
                    }
                    if (txtReqDeptCd.Text != "")
                    {
                        txtReqDeptNm.Value = SystemBase.Base.CodeName("DEPT_CD", "DEPT_NM", "B_DEPT_INFO", txtReqDeptCd.Text, " And REORG_ID = '" + txtReqReorgId.Text + "' AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                    }
                    else
                    {
                        txtReqDeptNm.Value = "";
                    }
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
                    txtItemNm.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtItemCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
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

        #endregion

        #region fpSpread1_ButtonClicked
        private void fpSpread1_ButtonClicked(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            fpSpread1.Sheets[0].RowHeader.Cells[e.Row, 0].Text = "";
        }
        #endregion

    }
}
