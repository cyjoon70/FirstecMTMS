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

namespace PC.PCC006
{
    public partial class PCC006 : UIForm.Buttons
    {
        #region 생성자
        public PCC006()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load시
        private void PCC006_Load(object sender, System.EventArgs e)
        {
            //필수 체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            //콤보박스세팅
            SystemBase.ComboMake.C1Combo(cboOrderStatus, "usp_B_COMMON @pType='COMM', @pCODE = 'P020', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//Status

            //기타세팅
            dtpPlanStartDtFr.Value = SystemBase.Base.ServerTime("YYMMDD");
            dtpPlanStartDtTo.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddDays(7).ToString();
            txtPlantCd.Text = SystemBase.Base.gstrPLANT_CD;
            cboOrderStatus.SelectedValue = "RL";
            rdoMakeorder.Checked = true;

        }
        #endregion

        #region NewExec()
        protected override void NewExec()
        {
            //필수체크
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            //기타세팅
            dtpPlanStartDtFr.Value = SystemBase.Base.ServerTime("YYMMDD");
            dtpPlanStartDtTo.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddDays(7).ToString();
            dtpReportDtFr.Value = null;
            dtpReportDtTo.Value = null;
            txtPlantCd.Text = SystemBase.Base.gstrPLANT_CD;
            cboOrderStatus.SelectedValue = "RL";
        }
        #endregion

        #region 조회조건 팝업
        //공장
        private void btnPlantCd_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = "usp_B_COMMON @pTYPE = 'TABLE_POP' ,@pSPEC1 = 'PLANT_CD', @pSPEC2 = 'PLANT_NM', @pSPEC3 = 'B_PLANT_INFO', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtPlantCd.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00005", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "공장코드 조회");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtPlantCd.Text = Msgs[0].ToString();
                    txtPlantNm.Value = Msgs[1].ToString();
                    txtWorkorderNoFr.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "공장조회 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);	//데이터 조회 중 오류가 발생하였습니다.

            }
        }

        //프로젝트번호 FROM
        private void btnProjectNoFr_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW003 pu = new WNDW003(txtProjectNoFr.Text, "S1");
                pu.MaximizeBox = false;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtProjectNoFr.Text = Msgs[3].ToString();
                    txtProjectNmFr.Value = Msgs[4].ToString();
                    txtProjectSeqFr.Text = Msgs[5].ToString();
                    txtProjectNoTo.Focus();
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
                WNDW003 pu = new WNDW003(txtProjectNoTo.Text, "S1");
                pu.MaximizeBox = false;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtProjectNoTo.Text = Msgs[3].ToString();
                    txtProjectNmTo.Value = Msgs[4].ToString();
                    txtProjectSeqTo.Text = Msgs[5].ToString();
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
                string strQuery = " usp_B_COMMON 'PROJ_SEQ', @pSPEC1 = '" + txtProjectNoFr.Text + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";										// 쿼리
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };			// 쿼리 인자값(조회조건)
                string[] strSearch = new string[] { txtProjectSeqFr.Text, "" };		// 쿼리 인자값에 들어갈 데이타

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
                string[] strSearch = new string[] { txtProjectSeqTo.Text, "" };		// 쿼리 인자값에 들어갈 데이타

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

        //품목코드 From
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
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목코드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //품목코드 To
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
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목코드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    dtpPlanStartDtFr.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "대표오더정보조회 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //작업장 From
        private void btnWcCdFr_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON @pType='COMM_POP', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pSPEC1 = 'P002' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtWcCdFr.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00025", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "작업장 조회");
                
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtWcCdFr.Text = Msgs[0].ToString();
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
             
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtWcCdTo.Text = Msgs[0].ToString();
                    txtWcNmTo.Value = Msgs[1].ToString();
                    txtWorkorderNoRsFr.Focus();
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
                WNDW006 pu = new WNDW006(txtWorkorderNoFr.Text);
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtWorkorderNoFr.Text = Msgs[1].ToString();
                    txtWorkorderNoFr.Focus();
                }
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
                WNDW006 pu = new WNDW006(txtWorkorderNoTo.Text);
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtWorkorderNoTo.Text = Msgs[1].ToString();
                    txtWorkorderNoTo.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f);
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "제조오더번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion		
                
        #region 조회조건 TextChanged
        //공장
        private void txtPlantCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtPlantCd.Text != "")
                {
                    txtPlantNm.Value = SystemBase.Base.CodeName("PLANT_CD", "PLANT_NM", "B_PLANT_INFO", txtPlantCd.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");
                }
                else
                {
                    txtPlantNm.Value = "";
                }
            }
            catch { }
        }

        //프로젝트번호 FROM
        private void txtProjectNoFr_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtProjectNoFr.Text != "")
                {
                    txtProjectNmFr.Value = SystemBase.Base.CodeName("PROJECT_NO", "PROJECT_NM", "S_SO_MASTER", txtProjectNoFr.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");
                }
                else
                {
                    txtProjectNmFr.Value = "";
                }
                if (txtProjectNmFr.Text == "") txtProjectSeqFr.Text = "";
            }
            catch { }
        }

        //프로젝트번호 TO
        private void txtProjectNoTo_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtProjectNoTo.Text != "")
                {
                    txtProjectNmTo.Value = SystemBase.Base.CodeName("PROJECT_NO", "PROJECT_NM", "S_SO_MASTER", txtProjectNoTo.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");
                }
                else
                {
                    txtProjectNmTo.Value = "";
                }
                if (txtProjectNmTo.Text == "") txtProjectSeqTo.Text = "";
            }
            catch { }
        }

        //품목코드 FROM
        private void txtItemCdFr_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtItemCdFr.Text != "")
                {
                    txtItemNmFr.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtItemCdFr.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");
                }
                else
                {
                    txtItemNmFr.Value = "";
                }
            }
            catch { }
        }

        //품목코드 To
        private void txtItemCdTo_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtItemCdTo.Text != "")
                {
                    txtItemNmTo.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtItemCdTo.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");
                }
                else
                {
                    txtItemNmTo.Value = "";
                }
            }
            catch { }
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
            catch { }
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
            catch { }
        }

        #endregion

        #region 인쇄(미완료)
        private void butPrint_Click(object sender, System.EventArgs e)
        {
           
        }
        #endregion

        #region 레포트 출력
        private void butPreview_Click(object sender, System.EventArgs e)
        {
            try

            {
                //조회 필수 체크
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {
                    string RptName = "";
                    string Type = "";

                    if (rdoMakeorder.Checked == true)
                    {
                        RptName = @"Report\PCC006.rpt";    // 레포트경로+레포트명
                        Type = "R1";
                    }
                    else if (rdoItemCd.Checked == true)
                    {
                        RptName = @"Report\PCC006_1.rpt";    // 레포트경로+레포트명
                        Type = "R2";
                    }
                    else if (rdoWc_Cd_Workorder.Checked == true)
                    {
                        RptName = @"Report\PCC006_2.rpt";    // 레포트경로+레포트명
                        Type = "R3";
                    }
                    else if (rdoWc_Cd_ITEM.Checked == true)
                    {
                        RptName = @"Report\PCC006_3.rpt";    // 레포트경로+레포트명
                        Type = "R4";
                    }

                    string[] RptParmValue = new string[42];   // SP 파라메타 값

                    RptParmValue[0] = Type;
                    RptParmValue[1] = SystemBase.Base.gstrLangCd;
                    RptParmValue[2] = txtPlantCd.Text;
                    RptParmValue[3] = cboOrderStatus.SelectedValue.ToString();
                    RptParmValue[4] = txtWorkorderNoFr.Text;
                    RptParmValue[5] = txtWorkorderNoTo.Text;
                    RptParmValue[6] = txtProjectNoFr.Text;
                    RptParmValue[7] = txtProjectNoTo.Text;
                    RptParmValue[8] = txtProjectSeqFr.Text;
                    RptParmValue[9] = txtProjectSeqTo.Text;
                    RptParmValue[10] = txtWcCdFr.Text;
                    RptParmValue[11] = txtWcCdTo.Text;
                    RptParmValue[12] = txtItemCdFr.Text;
                    RptParmValue[13] = txtItemCdTo.Text;
                    RptParmValue[14] = dtpPlanStartDtFr.Text;
                    RptParmValue[15] = dtpPlanStartDtTo.Text;
                    RptParmValue[16] = txtWorkorderNoRsFr.Text;
                    RptParmValue[17] = txtWorkorderNoRsTo.Text;
                    RptParmValue[18] = dtpReportDtFr.Text;
                    RptParmValue[19] = dtpReportDtTo.Text;
                    RptParmValue[20] = SystemBase.Base.gstrCOMCD;


                    RptParmValue[21] = "R5";
                    RptParmValue[22] = SystemBase.Base.gstrLangCd;
                    RptParmValue[23] = txtPlantCd.Text;
                    RptParmValue[24] = "";
                    RptParmValue[25] = "";
                    RptParmValue[26] = "";
                    RptParmValue[27] = "";
                    RptParmValue[28] = "";
                    RptParmValue[29] = "";
                    RptParmValue[30] = "";
                    RptParmValue[31] = "";
                    RptParmValue[32] = "";
                    RptParmValue[33] = txtItemCdFr.Text;
                    RptParmValue[34] = "";
                    RptParmValue[35] = "";
                    RptParmValue[36] = "";
                    RptParmValue[37] = "";
                    RptParmValue[38] = "";
                    RptParmValue[39] = "";
                    RptParmValue[40] = "";
                    RptParmValue[41] = SystemBase.Base.gstrCOMCD;
                    UIForm.PRINT10 frm = new UIForm.PRINT10(this.Text + "출력", null, RptName, RptParmValue); //공통크리스탈 10버전				
                    frm.ShowDialog();

                }
            }
            catch (Exception ex)
            {
                SystemBase.Loggers.Log(this.Name, ex.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn(ex.ToString()), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion		

    }
}
