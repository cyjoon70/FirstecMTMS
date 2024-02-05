#region 작성정보
/*********************************************************************/
// 단위업무명 : 발주서출력
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-02-14
// 작성내용 : 발주서출력 및 관리
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
using System.IO;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using WNDW;

namespace MP.MPO506
{
    public partial class MPO506 : UIForm.Buttons
    {
        #region 생성자
        public MPO506()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load시
        private void MPO506_Load(object sender, System.EventArgs e)
        {
            //필수 체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            //기타세팅
            dtpPoDtFr.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToString().Substring(0,10);
            dtpPoDtTo.Value = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0,10);
            rdoCfmPoOld.Visible = false;
        }
        #endregion

        #region NewExec()
        protected override void NewExec()
        {
            //필수체크
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            //기타세팅
            dtpPoDtFr.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToString().Substring(0, 10);
            dtpPoDtTo.Value = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0, 10);
        }
        #endregion

        #region 조회 조건 팝업
        //공급처 팝업
        private void btnCustCd_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW002 pu = new WNDW002(txtCustCd.Text, "");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;
                    txtCustCd.Text = Msgs[1].ToString();
                    txtCustNm.Value = Msgs[2].ToString();
                    txtCustCd.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "공급처 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //구매담당자
        private void btnPurDuty_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_M_COMMON 'M011' ,@pSPEC1='" + SystemBase.Base.gstrBIZCD + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtPurDuty.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P01008", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "구매담당자 팝업");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {

                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtPurDuty.Text = Msgs[0].ToString();
                    txtPurDutyNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "구매담당자 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        //발주번호
        private void btnPoNo_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW018 pu = new WNDW018();
                pu.MaximizeBox = false;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtPoNo.Text = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        //프로젝트번호 FROM
        private void btnProjectNoFr_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW007 pu = new WNDW007(txtProjectNoFr.Text);
                pu.MaximizeBox = false;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtProjectNoFr.Text = Msgs[3].ToString();
                    txtProjectNmFr.Value = Msgs[4].ToString();
                    txtProjectSeqFr.Text = "";
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "프로젝트번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //프로젝트번호 TO
        private void btnProjectNoTo_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW007 pu = new WNDW007(txtProjectNoTo.Text);
                pu.MaximizeBox = false;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtProjectNoTo.Text = Msgs[3].ToString();
                    txtProjectNmTo.Value = Msgs[4].ToString();
                    txtProjectSeqTo.Text = "";
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "프로젝트번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //프로젝트차수 FROM
        private void btnProjectSeqFr_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON 'PROJ_SEQ', @pSPEC1 = '" + txtProjectNoFr.Text + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";										// 쿼리
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
                    txtProjectSeqFr.Text = Msgs[0].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        //프로젝트차수 TO
        private void btnProjectSeqTo_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON 'PROJ_SEQ', @pSPEC1 = '" + txtProjectNoTo.Text + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";										// 쿼리
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
                    txtProjectSeqTo.Text = Msgs[0].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }
        #endregion

        #region 조회조건 TextChanged
        //공급처
        private void txtCustCd_TextChanged(object sender, System.EventArgs e)
        {
            try
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
            catch
            {

            }

        }

        //구매담당자
        private void txtPurDuty_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtPurDuty.Text != "")
                {
                    txtPurDutyNm.Value = SystemBase.Base.CodeName("USR_ID", "USR_NM", "B_SYS_USER", txtPurDuty.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtPurDutyNm.Value = "";
                }
            }
            catch
            {

            }
        }
        //발주번호 FROM
        private void txtProjectNoFr_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtProjectNoFr.Text != "")
                {
                    txtProjectNmFr.Value = SystemBase.Base.CodeName("PROJECT_NO", "PROJECT_NM", "S_SO_MASTER", txtProjectNoFr.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtProjectNmFr.Value = "";
                }
                if (txtProjectNmFr.Text == "")
                    txtProjectSeqFr.Text = "";
            }
            catch
            {

            }

        }

        //발주번호 TO
        private void txtProjectNoTo_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtProjectNoTo.Text != "")
                {
                    txtProjectNmTo.Value = SystemBase.Base.CodeName("PROJECT_NO", "PROJECT_NM", "S_SO_MASTER", txtProjectNoTo.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtProjectNmTo.Value = "";
                }
                if (txtProjectNmTo.Text == "")
                    txtProjectSeqTo.Text = "";
            }
            catch
            {

            }

        }
        #endregion
        
        #region 레포트 출력
        private void butPreview_Click(object sender, System.EventArgs e)
        {
            //조회 필수 체크
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                try
                {
                    string RptName = "";    // 레포트경로+레포트명
                    string strType = "";
                    string[] RptParmValue = new string[12];   // SP 파라메타 값

                    if (rdoCfmPoOld.Checked == true)
                    {
                        RptName = SystemBase.Base.ProgramWhere + @"\Report\MPO506_1.rpt";
                        strType = "R1";
                    }
                    else if (rdoCfmOffer.Checked == true)
                    {
                        RptName = SystemBase.Base.ProgramWhere + @"\Report\MPO506_2.rpt";
                        strType = "R2";
                    }
                    else
                    {
                        RptName = SystemBase.Base.ProgramWhere + @"\Report\MPO506_3.rpt";
                        strType = "R3";
                    }

                    RptParmValue[0] = strType.Trim();
                    RptParmValue[1] = SystemBase.Base.gstrCOMCD;
                    RptParmValue[2] = SystemBase.Base.gstrLangCd;
                    RptParmValue[3] = txtCustCd.Text;
                    RptParmValue[4] = txtPurDuty.Text;
                    RptParmValue[5] = txtPoNo.Text;
                    RptParmValue[6] = dtpPoDtFr.Text;
                    RptParmValue[7] = dtpPoDtTo.Text;
                    RptParmValue[8] = txtProjectNoFr.Text;
                    RptParmValue[9] = txtProjectNoTo.Text;
                    RptParmValue[10] = txtProjectSeqFr.Text;
                    RptParmValue[11] = txtProjectSeqTo.Text;

                    UIForm.PRINT10 frm = new UIForm.PRINT10(this.Text + "출력", null, RptName, RptParmValue); //공통크리스탈 10버전				
                    frm.ShowDialog();

                    //PRINT frm = new PRINT(this.Text + "출력", null, RptName, RptParmValue);
                    //frm.ShowDialog();
                }
                catch (Exception f)
                {
                    MessageBox.Show(f.ToString());
                }

            }
        }
        #endregion

        #region 엑셀양식 출력
        private void butExcel_Click(object sender, EventArgs e)
        {
            if (txtPoNo.Text != "")
            {
                string strSheetPage1 = "구매발주서";

                string strFileName = SystemBase.Base.ProgramWhere + @"\Report\구매발주서.xls";

                try
                {
                    this.Cursor = Cursors.WaitCursor;

                    string strQuery = " usp_MPO506  @pTYPE = 'R6'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' ";
                    strQuery += ", @pCUST_CD = '" + txtCustCd.Text + "' ";
                    strQuery += ", @pPUR_DUTY = '" + txtPurDuty.Text + "' ";
                    strQuery += ", @pPO_NO = '" + txtPoNo.Text + "' ";
                    strQuery += ", @pPO_DT_FR = '" + dtpPoDtFr.Text + "' ";
                    strQuery += ", @pPO_DT_TO = '" + dtpPoDtTo.Text + "' ";
                    strQuery += ", @pPROJECT_NO_FR = '" + txtProjectNoFr.Text + "' ";
                    strQuery += ", @pPROJECT_NO_TO = '" + txtProjectNoTo.Text + "' ";
                    strQuery += ", @pPROJECT_SEQ_FR = '" + txtProjectSeqFr.Text + "' ";
                    strQuery += ", @pPROJECT_SEQ_TO = '" + txtProjectSeqTo.Text + "' ";

                    DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

                    if (dt.Rows.Count > 0)
                    {

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


                        // 엑셀쓰기---------------------------------------------------------
                        
                        int iUseRow = 0;
                        int j = 0;
                        int vTotAmt = 0;

                        // Heard 값
                        excel.SetCell(1, 16, dt.Rows[0]["PO_NO"].ToString());           //발주번호
                        excel.SetCell(1, 27, dt.Rows[0]["TEL1"].ToString());            //TEL
                        excel.SetCell(1, 37, dt.Rows[0]["FAX"].ToString());             //FAX
                        excel.SetCell(1, 49, dt.Rows[0]["PRT_DT"].ToString());          //DATE(인쇄일자)
                        excel.SetCell(1, 55, dt.Rows[0]["PUR_DUTY"].ToString());        //구매담당자
                        excel.SetCell(2, 16, dt.Rows[0]["CHARGE_NM1"].ToString());      //담당자
                        excel.SetCell(2, 23, dt.Rows[0]["ZIPCODE1"].ToString());        //공급자우편번호
                        excel.SetCell(2, 43, dt.Rows[0]["ZIPCODE2"].ToString());        //공급받는자우편번호
                        excel.SetCell(3, 23, dt.Rows[0]["CUST_NM1"].ToString());        //공급자사업장명
                        excel.SetCell(3, 43, dt.Rows[0]["CUST_NM2"].ToString());        //공급받는자사업장명
                        excel.SetCell(4, 16, dt.Rows[0]["CHARGE_TEL1"].ToString());     //담당자TEL
                        excel.SetCell(4, 23, dt.Rows[0]["ADDR1"].ToString());           //공급자주소
                        excel.SetCell(4, 43, dt.Rows[0]["ADDR2"].ToString());           //공급받는자주소
                        excel.SetCell(5, 23, dt.Rows[0]["CHARGE_NM2"].ToString());      //공급자 대표
                        excel.SetCell(5, 43, dt.Rows[0]["REPRE_NM2"].ToString());       //공급받는자 대표
                        excel.SetCell(7, 3, dt.Rows[0]["PO_DT"].ToString());            //발주일자
                        excel.SetCell(7, 10, dt.Rows[0]["PAYMENT_METH"].ToString());    //결제방법
                        excel.SetCell(7, 19, dt.Rows[0]["ENT_NM"].ToString());          //사업명
                        excel.SetCell(7, 26, dt.Rows[0]["PO_TYPE_NM"].ToString());      //발주형태
                        excel.SetCell(7, 34, dt.Rows[0]["CURRENCY"].ToString());        //화폐단위
                        excel.SetCell(7, 46, dt.Rows[0]["PROJECT_NO"].ToString());      //프로젝트번호
                        excel.SetCell(7, 54, dt.Rows[0]["ITEM_ACCT"].ToString());       //품목계정
                        excel.SetCell(8, 1, dt.Rows[0]["VAT_FLAG"].ToString());         // 2018.12.28. hma 추가: 부가세포함여부(1:포함,2:별도)

                        for (int x = 1; x < dt.Rows.Count; x++) //행추가 및 셀병합
                        {
                            excel.SetAddRow("A" + (10 + x), "BD" + (10 + x));
                            excel.CellMerge("B" + (10 + x) + ":D" + (10 + x));
                            excel.CellMerge("E" + (10 + x) + ":M" + (10 + x));
                            excel.CellMerge("N" + (10 + x) + ":Y" + (10 + x));
                            excel.CellMerge("Z" + (10 + x) + ":AD" + (10 + x));
                            excel.CellMerge("AE" + (10 + x) + ":AI" + (10 + x));
                            excel.CellMerge("AJ" + (10 + x) + ":AL" + (10 + x));
                            excel.CellMerge("AM" + (10 + x) + ":AR" + (10 + x));
                            excel.CellMerge("AS" + (10 + x) + ":AV" + (10 + x));
                            excel.CellMerge("AW" + (10 + x) + ":AY" + (10 + x));
                            excel.CellMerge("AZ" + (10 + x) + ":BD" + (10 + x));

                            excel.CellBorder("A" + (10 + x) + ":B" + (10 + x));
                            excel.CellBorder("B" + (10 + x) + ":D" + (10 + x));
                            excel.CellBorder("E" + (10 + x) + ":M" + (10 + x));
                            excel.CellBorder("N" + (10 + x) + ":Y" + (10 + x));
                            excel.CellBorder("Z" + (10 + x) + ":AD" + (10 + x));
                            excel.CellBorder("AE" + (10 + x) + ":Ax" + (10 + x));
                            excel.CellBorder("AJ" + (10 + x) + ":AL" + (10 + x));
                            excel.CellBorder("AM" + (10 + x) + ":AR" + (10 + x));
                            excel.CellBorder("AS" + (10 + x) + ":AV" + (10 + x));
                            excel.CellBorder("AW" + (10 + x) + ":AY" + (10 + x));
                            excel.CellBorder("AZ" + (10 + x) + ":BD" + (10 + x));
                        }

                        for (int i = 0; i < dt.Rows.Count; i++) //내용입력
                        {
                            excel.SetCell(10 + i, 1, dt.Rows[i]["NUM"].ToString());    //순번
                            excel.SetCell(10 + i, 2, dt.Rows[i]["ITEM_CD"].ToString());  //품목코드
                            excel.SetCell(10 + i, 5, dt.Rows[i]["ITEM_NM"].ToString());   //품명
                            excel.SetCell(10 + i, 14, dt.Rows[i]["ITEM_SPEC"].ToString()); //규격
                            excel.SetCell(10 + i, 26, dt.Rows[i]["SL_NM"].ToString());    //창고

                            excel.SetCell(10 + i, 31, dt.Rows[i]["DELIVERY_DT"].ToString());    //납기일
                            excel.SetCell(10 + i, 36, dt.Rows[i]["PO_UNIT"].ToString());    //단위
                            excel.SetCell(10 + i, 39, dt.Rows[i]["PO_QTY"].ToString());    //발주수량
                            excel.SetCell(10 + i, 45, dt.Rows[i]["PO_PRICE"].ToString());    //단가
                            excel.SetCell(10 + i, 49, dt.Rows[i]["PO_AMT"].ToString());    //금액
                            excel.SetCell(10 + i, 52, dt.Rows[i]["Q_REQ_DOC"].ToString());    //품질요구사항

                            vTotAmt += Convert.ToInt32(dt.Rows[i]["PO_AMT"]);
                            iUseRow = 10 + i;
                        }

                        excel.SetCell(iUseRow + 1, 45, Convert.ToString(vTotAmt));    //합계
                        excel.SetCell(iUseRow + 2, 2, dt.Rows[0]["REMARK"].ToString());    //비고
                        excel.SetCell(iUseRow + 8, 2, dt.Rows[0]["CUST_REMARK"].ToString());    //거래처비고

                        string strQuery1 = " usp_MPO506_2";
                        strQuery1 += " @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                        strQuery1 += ", @pPO_NO = '" + txtPoNo.Text + "' ";

                        DataTable dt1 = SystemBase.DbOpen.NoTranDataTable(strQuery1);

                        if(dt1.Rows.Count > 0)
                        {
                            excel.SetCell(iUseRow + 13, 2, dt1.Rows[0]["CD_NM"].ToString());    //품질증빙
                        }
                        excel.SetSelect("A1", "A1");

                        excel.ShowExcel(true);
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "구매발주서출력"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    File.SetAttributes(strFileName, System.IO.FileAttributes.Normal);
                }
                this.Cursor = Cursors.Default;
            }else
            {
                MessageBox.Show("발주번호가 입력해주시기 바랍니다.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        #endregion
    }
}
