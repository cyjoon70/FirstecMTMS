#region 작성정보
/*********************************************************************/
// 단위업무명 : 외주공정발주서
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-02-18
// 작성내용 : 외주공정발주서 및 관리
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
using System.IO;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using WNDW;
using Excel;

namespace MO.MIM517
{
    public partial class MIM517 : UIForm.Buttons
    {
        private Excel.Application excelApp = null;
        private Excel.Workbook excelWorkbook = null;
        private Excel.Sheets excelSheets = null;
        private Excel.Worksheet excelWorksheet = null;

        public MIM517()
        {
            InitializeComponent();
        }

        #region Form Load시
        private void MIM517_Load(object sender, System.EventArgs e)
        {
            //필수 체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            //콤보박스 세팅
            SystemBase.ComboMake.C1Combo(cboPlantCd, "usp_B_COMMON @pType='TABLE', @pCODE = 'PLANT_CD', @pNAME = 'PLANT_NM', @pSPEC1= 'B_PLANT_INFO', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 0); //공장
            SystemBase.ComboMake.C1Combo(cboWcCd, "usp_B_COMMON @pType='REL', @pCODE = 'P002', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "', @pSPEC2 = 'Y', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ", 0); //작업장
            SystemBase.ComboMake.C1Combo(cboOrderStatus, "usp_B_COMMON @pType='COMM', @pCODE = 'P020', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 0); //Status


            //기타세팅
            dtpDeliveryDtFr.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToString().Substring(0,10);
            dtpDeliveryDtTo.Value = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0,10);
            cboOrderStatus.SelectedValue = "ST";
        }
        #endregion

        #region NewExec()
        protected override void NewExec()
        {
            //필수체크
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            //기타세팅
            dtpDeliveryDtFr.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToString().Substring(0,10);
            dtpDeliveryDtTo.Value = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0,10);
            cboOrderStatus.SelectedValue = "ST";
        }
        #endregion

        #region 조회 조건 팝업
        //제조오더번호
        private void btnWorkorderNo_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW006 pu = new WNDW006(txtWorkorderNo.Text);
                pu.MaximizeBox = false;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtWorkorderNo.Text = Msgs[1].ToString();
                    txtWorkorderNo.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "제조오더번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //공정
        private void btnProcSeq_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_MIM517 'P1', @pWORK_ORDER_NO = '" + txtWorkorderNo.Text + "', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";										// 쿼리
                string[] strWhere = new string[] { "@pPROC_SEQ", "" };			// 쿼리 인자값(조회조건)
                string[] strSearch = new string[] { txtProcSeq.Text, "" };		// 쿼리 인자값에 들어갈 데이타

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00089", strQuery, strWhere, strSearch, new int[] { 0 }, "공정 조회", false);
                pu.Width = 600;
                pu.ShowDialog();	//공통 팝업 호출

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtProcSeq.Text = Msgs[0].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "공정 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }
        //거래처 팝업
        private void btnCustCd_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW002 pu = new WNDW002(txtCustCd.Text, "");
                pu.MaximizeBox = false;
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
                string strQuery = " usp_B_COMMON 'PROJ_SEQ', @pSPEC1 = '" + txtProjectNoFr.Text + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";										// 쿼리
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
                string strQuery = " usp_B_COMMON 'PROJ_SEQ', @pSPEC1 = '" + txtProjectNoTo.Text + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";										// 쿼리
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

        //품목코드 FROM
        private void btnItemCdFr_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW005 pu = new WNDW005(txtItemCdFr.Text, "");
                pu.MaximizeBox = false;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;
                    txtItemCdFr.Text = Msgs[2].ToString();
                    txtItemNmFr.Value = Msgs[3].ToString();
                    txtItemCdFr.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //품목코드 TO
        private void btnItemCdTo_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW005 pu = new WNDW005(txtItemCdTo.Text, "");
                pu.MaximizeBox = false;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;
                    txtItemCdTo.Text = Msgs[2].ToString();
                    txtItemNmTo.Value = Msgs[3].ToString();
                    txtItemCdTo.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnPoNoTo_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW018 pu = new WNDW018();
                pu.MaximizeBox = false;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtPoNoTo.Text = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }
        //구매담당자
        private void btnPurDuty_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = " usp_M_COMMON 'M011' ,@pSPEC1='" + SystemBase.Base.gstrBIZCD + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtPurDuty.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00031", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "사용자 팝업");
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

        //프로젝트번호 FROM
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

        //프로젝트번호 TO
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

        //품목코드 FROM
        private void txtItemCdFr_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtItemCdFr.Text != "")
                {
                    txtItemNmFr.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtItemCdFr.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtItemNmFr.Value = "";
                }
            }
            catch
            {

            }
        }

        //품목코드 TO
        private void txtItemCdTo_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtItemCdTo.Text != "")
                {
                    txtItemNmTo.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtItemCdTo.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtItemNmTo.Value = "";
                }
            }
            catch
            {

            }
        }

        //구매담당자
        private void txtPurDuty_TextChanged(object sender, EventArgs e)
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
        #endregion

        #region 레포트 출력
        private void butPreview_Click(object sender, System.EventArgs e)
        {
            //조회 필수 체크
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                try
                {
                    string RptName = SystemBase.Base.ProgramWhere + @"\Report\MIM517.rpt";    // 레포트경로+레포트명
                    string[] RptParmValue = new string[21];   // SP 파라메타 값

                    RptParmValue[0] = "R1";
                    RptParmValue[1] = SystemBase.Base.gstrCOMCD;
                    RptParmValue[2] = SystemBase.Base.gstrLangCd;
                    RptParmValue[3] = cboPlantCd.SelectedValue.ToString();
                    RptParmValue[4] = txtCustCd.Text;
                    RptParmValue[5] = cboOrderStatus.SelectedValue.ToString();
                    RptParmValue[6] = txtWorkorderNo.Text;
                    RptParmValue[7] = txtPoNo.Text;
                    RptParmValue[8] = txtProcSeq.Text;
                    RptParmValue[9] = cboWcCd.SelectedValue.ToString();
                    RptParmValue[10] = txtRemark.Text;
                    RptParmValue[11] = dtpDeliveryDtFr.Text;
                    RptParmValue[12] = dtpDeliveryDtTo.Text;
                    RptParmValue[13] = txtProjectNoFr.Text;
                    RptParmValue[14] = txtProjectNoTo.Text;
                    RptParmValue[15] = txtProjectSeqFr.Text;
                    RptParmValue[16] = txtProjectSeqTo.Text;
                    RptParmValue[17] = txtItemCdFr.Text;
                    RptParmValue[18] = txtItemCdTo.Text;
                    RptParmValue[19] = txtPoNoTo.Text;
                    RptParmValue[20] = txtPurDuty.Text;

                    UIForm.PRINT10 frm = new UIForm.PRINT10(this.Text + "출력", null, RptName, RptParmValue); //공통크리스탈 10버전				
                    frm.ShowDialog();
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
            if (txtPoNo.Text != "" && txtPoNoTo.Text != "" && txtPoNo.Text == txtPoNoTo.Text)
            {

                string strSheetPage1 = "외주공정발주서";

                string strFileName = SystemBase.Base.ProgramWhere + @"\Report\외주공정발주서.xls";

                try
                {
                    this.Cursor = Cursors.WaitCursor;

                    string strQuery = " usp_MIM517  @pTYPE = 'R2'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' ";
                    strQuery += ", @pPLANT_CD = '" + cboPlantCd.SelectedValue.ToString() + "' ";
                    strQuery += ", @pCUST_CD = '" + txtCustCd.Text + "' ";
                    strQuery += ", @pORDER_STATUS = '" + cboOrderStatus.SelectedValue.ToString() + "' ";
                    strQuery += ", @pWORK_ORDER_NO = '" + txtWorkorderNo.Text + "' ";
                    strQuery += ", @pPO_NO = '" + txtPoNo.Text + "' ";
                    strQuery += ", @pPROC_SEQ = '" + txtProcSeq.Text + "' ";
                    strQuery += ", @pWC_CD = '" + cboWcCd.SelectedValue.ToString() + "' ";
                    strQuery += ", @pREMARK = '" + txtRemark.Text + "' ";
                    strQuery += ", @pDELIVERY_DT_FR = '" + dtpDeliveryDtFr.Text + "' ";
                    strQuery += ", @pDELIVERY_DT_TO = '" + dtpDeliveryDtTo.Text + "' ";
                    strQuery += ", @pPROJECT_NO_FR = '" + txtProjectNoFr.Text + "' ";
                    strQuery += ", @pPROJECT_NO_TO = '" + txtProjectNoTo.Text + "' ";
                    strQuery += ", @pPROJECT_SEQ_FR = '" + txtProjectSeqFr.Text + "' ";
                    strQuery += ", @pPROJECT_SEQ_TO = '" + txtProjectSeqTo.Text + "' ";
                    strQuery += ", @pITEM_CD_FR = '" + txtItemCdFr.Text + "' ";
                    strQuery += ", @pITEM_CD_TO = '" + txtItemCdTo.Text + "' ";
                    strQuery += ", @pPO_NO_TO = '" + txtPoNoTo.Text + "' ";
                    strQuery += ", @pPUR_DUTY = '" + txtPurDuty.Text + "' ";

                   System.Data.DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

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

                        // Header 값
                        excel.SetCell(3, 3, dt.Rows[0]["PO_NO"].ToString());                //발주번호
                        excel.SetCell(3, 7, dt.Rows[0]["CUST_NM"].ToString());              //업체명
                        excel.SetCell(3, 12, dt.Rows[0]["ENT_NM"].ToString());              //사업명
                        excel.SetCell(4, 7, dt.Rows[0]["ADDR"].ToString());                 //주소
                        excel.SetCell(4, 12, dt.Rows[0]["PO_DT"].ToString());               //발주일
                        excel.SetCell(5, 3, dt.Rows[0]["PAYMENT_METH_NM"].ToString());      //대금지불조건
                        excel.SetCell(5, 7, dt.Rows[0]["TEL1"].ToString());                 //전화번호
                        excel.SetCell(5, 9, dt.Rows[0]["FAX"].ToString());                  //FAX
                        excel.SetCell(5, 12, dt.Rows[0]["MAX_DELIVERY_DT"].ToString());     //최종납기일
                        //excel.SetCell(6, 3, dt.Rows[0]["PAYMENT_METH_NM"].ToString());    //발주금액
                        excel.SetCell(6, 7, dt.Rows[0]["VAT_INC_FLAG"].ToString());         //부가세
                        excel.SetCell(6, 9, dt.Rows[0]["PROJECT_NO"].ToString());           //프로젝트번호
                        excel.SetCell(7, 1, dt.Rows[0]["VAT_FLAG"].ToString());             // 2018.12.28. hma 추가: 부가세포함여부(1:포함,2:별도)

                        for (int x = 1; x <= dt.Rows.Count * 2; x++)//행추가 및 셀병합
                        {
                            //행추가
                            excel.SetAddRow("A" + (10 + x), "M" + (10 + x));

                            //병합
                            excel.CellMerge("B" + (10 + x) + ":C" + (10 + x));
                            excel.CellMerge("D" + (10 + x) + ":F" + (10 + x));
                            excel.CellMerge("G" + (10 + x) + ":H" + (10 + x));

                            //정렬
                            excel.CellLeftAlign("B" + (10 + x) + ":C" + (10 + x ));   //제조오더번호
                            excel.CellLeftAlign("D" + (10 + x) + ":F" + (10 + x));    //품목코드, 공정
                            excel.CellLeftAlign("G" + (10 + x) + ":H" + (10 + x));    //품명, 규격
                            excel.CellRightAlign("J" + (10 + x) + ":J" + (10 + x));   //단위
                            excel.CellRightAlign("K" + (10 + x) + ":K" + (10 + x));   //수량
                            excel.CellRightAlign("L" + (10 + x) + ":L" + (10 + x));   //단가
                            excel.CellRightAlign("M" + (10 + x) + ":M" + (10 + x));   //금액

                            //높이세팅
                            excel.SetRowHeight("A" + (10 + x), "M" + (10 + x), 12.75);

                            if (x % 2 == 0)
                            {
                                excel.CellMerge("A" + (10 + x) + ":A" + (10 + x - 1));
                                excel.CellBottomBorder("A" + (10 + x), "M" + (10 + x));
                            }
                        }

                        for (int i = 0; i < dt.Rows.Count; i++) //내용입력
                        {
                            // 2018.12.06. hma 추가(Start): 셀 타입 지정. @:텍스트, 0.00:숫자, yyyy-mm-dd:일자
                            excel.SetCellType(11 + j, 4, "@");          //품목코드
                            excel.SetCellType(11 + j + 1, 4, "@");      //공정
                            excel.SetCell(11 + j, 7, "@");              //품명
                            excel.SetCell(11 + j + 1, 7, "@");          //규격
                            excel.SetCell(11 + j + 1, 9, "@");          //작업명
                            // 2018.12.06. hma 추가(End)

                            excel.SetCell(11 + j, 1, dt.Rows[i]["NUM"].ToString());    //순번
                            excel.SetCell(11 + j, 2, dt.Rows[i]["WORK_ORDER_NO"].ToString());   //제조오더번호
                            excel.SetCell(11 + j, 4, dt.Rows[i]["ITEM_CD"].ToString());         //품목코드
                            excel.SetCell(11 + j + 1, 4, dt.Rows[i]["PROC_SEQ"].ToString());    //공정
                            excel.SetCell(11 + j, 7, dt.Rows[i]["ITEM_NM"].ToString());         //품명
                            excel.SetCell(11 + j + 1, 7, dt.Rows[i]["ITEM_SPEC"].ToString());   //규격
                            excel.SetCell(11 + j, 9, dt.Rows[i]["DELIVERY_DT"].ToString());     //납기일자
                            excel.SetCell(11 + j + 1, 9, dt.Rows[i]["JOB_NM"].ToString());      //작업명
                            excel.SetCell(11 + j, 11, dt.Rows[i]["PO_UNIT"].ToString());        //단위
                            excel.SetCell(11 + j, 11, dt.Rows[i]["PO_QTY"].ToString());         //수량
                            excel.SetCell(11 + j, 12, dt.Rows[i]["PO_PRICE"].ToString());       //단가
                            excel.SetCell(11 + j, 13, dt.Rows[i]["PO_AMT"].ToString());         //금액

                            j += 2;

                            vTotAmt += Convert.ToInt32(dt.Rows[i]["PO_AMT"]);                            
                            iUseRow = 11 + j;
                        }

                        excel.SetCell(iUseRow, 13, Convert.ToString(vTotAmt));    //합계
                        excel.SetRowHeight("A" + iUseRow, "M" + iUseRow, 21.75);    //합계 높이세팅
                        excel.SetCell(iUseRow + 6, 2, dt.Rows[0]["REMARK"].ToString());    //비고
                        excel.SetSelect("A1", "A1");

                        excel.ShowExcel(true);
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "외주공정발주서출력"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    File.SetAttributes(strFileName, System.IO.FileAttributes.Normal);
                }
                this.Cursor = Cursors.Default;
            }
            else
            {
                MessageBox.Show("발주번호를 모두 입력 해야 하며 발주번호 한건만 출력가능합니다.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        #endregion

        // 2018.12.06. hma 추가(Start): FROM발주번호 입력시 TO발주번호로 들어가도록 함.
        private void txtPoNo_TextChanged(object sender, EventArgs e)
        {
            txtPoNoTo.Text = txtPoNo.Text;
        }
        // 2018.12.06. hma 추가(End)
    }
}
