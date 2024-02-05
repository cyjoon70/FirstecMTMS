#region 작성정보
/*********************************************************************/
// 단위업무명 : 자재출고요청서
// 작 성 자 : 이  태  규
// 작 성 일 : 2013-04-16
// 작성내용 : 자재출고요청서
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
using WNDW;

namespace PC.PCC003
{
    public partial class PCC003 : UIForm.Buttons
    {
        #region 생성자
        public PCC003()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void PCC003_Load(object sender, System.EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            dtpReqDtFr.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToString().Substring(0,10);
            dtpReqDtTo.Value = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0,10);
            txtChilePlantCd.Value = SystemBase.Base.gstrPLANT_CD;

        }
        #endregion
        
        #region NewExec()
        protected override void NewExec()
        {
            //필수체크
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            //기타세팅
            dtpReqDtFr.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToString().Substring(0,10);
            dtpReqDtTo.Value = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0,10);
            txtChilePlantCd.Value = SystemBase.Base.gstrPLANT_CD;
        }
        #endregion

        #region 조회조건 팝업
        //공장
        private void btnChilePlantCd_Click(object sender, System.EventArgs e)
        {

        }

        //품목구분
        private void btnItemCd_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = "usp_B_COMMON @pTYPE = 'COMM_POP' ,@pLANG_CD ='" + SystemBase.Base.gstrLangCd + "',@pSPEC1 = 'P032', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtItemCd.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00077", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "품목구분코드 조회");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtItemCd.Value = Msgs[0].ToString();
                    txtItemNm.Value = Msgs[1].ToString();
                    txtSlCd.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목구분 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);	//데이터 조회 중 오류가 발생하였습니다.

            }
        }

        //출고창고
        private void btnSlCd_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pType='P012', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };
                string[] strSearch = new string[] { txtSlCd.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00005", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "창고 조회", false);

                pu.ShowDialog();	//공통 팝업 호출
                if (pu.DialogResult == DialogResult.OK)
                {
                    string MSG = pu.ReturnVal.Replace("|", "#");
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(MSG);

                    txtSlCd.Value = Msgs[0].ToString();
                    txtSlNm.Value = Msgs[1].ToString();
                    txtProjectNo.Focus();

                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "출고창고 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //프로젝트번호
        private void btnProjectNo_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW003 pu = new WNDW003(txtProjectNo.Text, "S1");
                pu.MaximizeBox = false;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtProjectNo.Value = Msgs[3].ToString();
                    txtProjectNm.Value = Msgs[4].ToString();
                    txtProjectSeqFr.Value = Msgs[5].ToString();
                    txtProjectSeqFr.Focus();
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
                string strQuery = " usp_B_COMMON 'PROJ_SEQ', @pSPEC1 = '" + txtProjectNo.Text + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";										// 쿼리
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
                    txtProjectSeqFr.Value = Msgs[0].ToString();
                    txtProjectSeqTo.Focus();
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
                string strQuery = " usp_B_COMMON 'PROJ_SEQ', @pSPEC1 = '" + txtProjectNo.Text + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";										// 쿼리
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
                    txtProjectSeqTo.Value = Msgs[0].ToString();
                    txtChildItemCdFr.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        //품목코드 From
        private void btnChildItemCdFr_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW005 pu = new WNDW005(txtChildItemCdFr.Text, "");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtChildItemCdFr.Value = Msgs[2].ToString();
                    txtChildItemNmFr.Value = Msgs[3].ToString();
                    txtSlCd.Text = Msgs[18].ToString();
                    txtChildItemCdTo.Focus();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목코드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //품목코드 To
        private void btnChildItemCdTo_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW005 pu = new WNDW005(txtChildItemCdTo.Text, "");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtChildItemCdTo.Value = Msgs[2].ToString();
                    txtChildItemNmTo.Value = Msgs[3].ToString();
                    txtWcCdFr.Focus();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목코드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //작업장 From
        private void btntWcCdFr_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON @pType='COMM_POP', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pSPEC1 = 'P002' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtWcCdFr.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00025", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "작업장 조회");
                //pu.Width = 500;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtWcCdFr.Value = Msgs[0].ToString();
                    txtWcNmFr.Value = Msgs[1].ToString();
                    txtWcCdTo.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "작업장 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //작업장 To
        private void btnWcCdTo_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON @pType='COMM_POP', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pSPEC1 = 'P002' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtWcCdTo.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00025", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "작업장 조회");
                //pu.Width = 500;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtWcCdTo.Value = Msgs[0].ToString();
                    txtWcNmTo.Value = Msgs[1].ToString();
                    txtWorkorderNoFr.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "작업장 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //제조오더번호 From
        private void btntxtWorkorderNoFr_Click(object sender, System.EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                string strQuery = " usp_P_COMMON @pTYPE ='P100', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };
                string[] strSearch = new string[] { txtWorkorderNoFr.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00057", strQuery, strWhere, strSearch, new int[] { 0, 1 });
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtWorkorderNoFr.Value = Msgs[0].ToString();
                    txtChildItemCdFr.Value = Msgs[1].ToString();
                    txtProjectNo.Value = Msgs[4].ToString();
                    txtWorkorderNoTo.Focus();
                }

                this.Cursor = Cursors.Default;
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f);
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "제조오더번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //제조오더번호 To
        private void btnWorkorderNoTo_Click(object sender, System.EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;
                string strQuery = " usp_P_COMMON @pTYPE ='P100', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };
                string[] strSearch = new string[] { txtWorkorderNoTo.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00057", strQuery, strWhere, strSearch, new int[] { 0, 1 });
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtWorkorderNoTo.Value = Msgs[0].ToString();
                    txtChildItemCdTo.Value = Msgs[1].ToString();
                    txtWorkorderNoRsFr.Focus();

                }

                this.Cursor = Cursors.Default;
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f);
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "제조오더번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //대표오더번호 FROM
        private void btnWorkorderNoRsFr_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW.WNDW028 pu = new WNDW.WNDW028();
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtWorkorderNoRsFr.Value = Msgs[1].ToString();
                    txtWorkorderNoRsTo.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "대표오더정보조회 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //대표오더번호 TO
        private void btnWorkorderNoRsTo_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW.WNDW028 pu = new WNDW.WNDW028();
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtWorkorderNoRsTo.Value = Msgs[1].ToString();
                    txtRemark.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "대표오더정보조회 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //제품오더번호 FROM
        private void btnMakeorderNoFr_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW008 pu = new WNDW008(txtMakeorderNoFr.Text, "R");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtMakeorderNoFr.Value = Msgs[1].ToString();
                    txtMakeorderNoFr.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "제품오더번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //제품오더번호 TO
        private void btnMakeorderNoTo_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW008 pu = new WNDW008(txtMakeorderNoTo.Text, "R");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtMakeorderNoTo.Value = Msgs[1].ToString();
                    txtMakeorderNoTo.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "제품오더번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        #endregion

        #region 조회조건 TextChanged
        //공장
        private void txtChilePlantCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtChilePlantCd.Text != "")
                {
                    txtChilePlantNm.Value = SystemBase.Base.CodeName("PLANT_CD", "PLANT_NM", "B_PLANT_INFO", txtChilePlantCd.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");
                }
                else
                {
                    txtChilePlantNm.Value = "";
                }
            }
            catch
            {

            }
        }

        //품목구분
        private void txtItemCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtItemCd.Text != "")
                {
                    txtItemNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtItemCd.Text, " AND MAJOR_CD='P032' AND LANG_CD='" + SystemBase.Base.gstrLangCd + "' AND COMP_CODE='" + SystemBase.Base.gstrCOMCD + "' ");
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

        //출하창고
        private void txtSlCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtSlCd.Text != "")
                {
                    txtSlNm.Value = SystemBase.Base.CodeName("SL_CD", "SL_NM", "B_STORAGE_INFO", txtSlCd.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");
                }
                else
                {
                    txtSlNm.Value = "";
                }
            }
            catch
            {

            }
        }

        //프로젝트번호
        private void txtProjectNo_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtProjectNo.Text != "")
                {
                    txtProjectNm.Value = SystemBase.Base.CodeName("PROJECT_NO", "PROJECT_NM", "S_SO_MASTER", txtProjectNo.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");
                }
                else
                {
                    txtProjectNm.Value = "";
                }
                if (txtProjectNm.Text == "")
                    txtProjectSeqFr.Value = ""; 
                txtProjectSeqTo.Value = "";
            }
            catch
            {

            }
        }

        //품목코드 From
        private void txtChildItemCdFr_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtChildItemCdFr.Text != "")
                {
                    txtChildItemNmFr.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtChildItemCdFr.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");
                }
                else
                {
                    txtChildItemNmFr.Value = "";
                }
            }
            catch
            {

            }

        }

        //품목코드 To
        private void txtChildItemCdTo_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtChildItemCdTo.Text != "")
                {
                    txtChildItemNmTo.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtChildItemCdTo.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");
                }
                else
                {
                    txtChildItemNmTo.Value = "";
                }
            }
            catch
            {

            }

        }

        //작업장 From
        private void txtWcCdFr_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtWcCdFr.Text != "")
                {
                    txtWcNmFr.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtWcCdFr.Text, " AND MAJOR_CD = 'P002' AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND COMP_CODE='" + SystemBase.Base.gstrCOMCD + "' ");
                }
                else
                {
                    txtWcNmFr.Value = "";
                }
            }
            catch
            {

            }
        }

        //작업장 To
        private void txtWcCdTo_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtWcCdTo.Text != "")
                {
                    txtWcNmTo.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtWcCdTo.Text, " AND MAJOR_CD = 'P002' AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND COMP_CODE='" + SystemBase.Base.gstrCOMCD + "' ");
                }
                else
                {
                    txtWcNmTo.Value = "";
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
            this.Cursor = Cursors.WaitCursor;

            //조회 필수 체크
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                //string[] FormulaField = new string[2];	  //formula 값			
                string RptName = "";    // 레포트경로+레포트명
                string[] RptParmValue = new string[25];   // SP 파라메타 값
                string[] FormulaFieldName = new string[1]; //formula 값
                string[] FormulaFieldValue = new string[1]; //formula 이름

                if (rdoWorkOrder.Checked == true)
                {
                    RptName = SystemBase.Base.ProgramWhere + @"\Report\PCC003_1.rpt";
                }
                else
                {
                    RptName = SystemBase.Base.ProgramWhere + @"\Report\PCC003_2.rpt";
                }

                string stockYn = "N";
                if (rdoStockQty1.Checked == true)
                {
                    stockYn = "Y";
                }
                else
                {
                    stockYn = "N";
                }

                string strReqYn = "N";
                if (rdoReqQty1.Checked == true)
                {
                    strReqYn = "Y";
                }
                else
                {
                    strReqYn = "N";
                }

                RptParmValue[0] = "R1";
                RptParmValue[1] = SystemBase.Base.gstrLangCd;
                RptParmValue[2] = txtChilePlantCd.Text;
                RptParmValue[3] = txtSlCd.Text;
                RptParmValue[4] = txtProjectNo.Text;
                RptParmValue[5] = dtpReqDtFr.Text;
                RptParmValue[6] = dtpReqDtTo.Text;
                RptParmValue[7] = txtItemCd.Text;
                RptParmValue[8] = txtChildItemCdFr.Text;
                RptParmValue[9] = txtChildItemCdTo.Text;
                RptParmValue[10] = txtWcCdFr.Text;
                RptParmValue[11] = txtWcCdTo.Text;
                RptParmValue[12] = txtWorkorderNoFr.Text;
                RptParmValue[13] = txtWorkorderNoTo.Text;
                RptParmValue[14] = txtProjectSeqFr.Text;
                RptParmValue[15] = txtProjectSeqTo.Text;
                RptParmValue[16] = txtWorkorderNoRsFr.Text;
                RptParmValue[17] = txtWorkorderNoRsTo.Text;
                RptParmValue[18] = txtMakeorderNoFr.Text;
                RptParmValue[19] = txtMakeorderNoTo.Text;
                RptParmValue[20] = dtpReportDtFr.Text;
                RptParmValue[21] = dtpReportDtTo.Text;
                RptParmValue[22] = stockYn;
                RptParmValue[23] = strReqYn;
                RptParmValue[24] = SystemBase.Base.gstrCOMCD;

                FormulaFieldValue[0] = "\"" + txtRemark.Text + "\"";
                FormulaFieldName[0] = "REMARK";

                UIForm.PRINT10 frm = new UIForm.PRINT10(this.Text + "출력", FormulaFieldValue, FormulaFieldName, RptName, RptParmValue); //공통크리스탈 10버전	
                frm.ShowDialog();

                this.Cursor = Cursors.Default;
            }
        }
        #endregion
        
    }
}
