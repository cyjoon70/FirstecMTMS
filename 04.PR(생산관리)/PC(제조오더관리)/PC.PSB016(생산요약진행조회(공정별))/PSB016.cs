#region 작성정보
/*********************************************************************/
// 단위업무명 : 생산요약진행조회(공정별)
// 작 성 자 : 김현근
// 작 성 일 : 2013-04-15
// 작성내용 : 생산요약진행조회(공정별)
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

namespace PC.PSB016
{
    public partial class PSB016 : UIForm.FPCOMM1
    {
        public PSB016()
        {
            InitializeComponent();
        }

        #region Form Load시
        private void PSB016_Load(object sender, System.EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(groupBox1); //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox2);

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0, true);

            //기타세팅
            dtpDelvDt.Value = null;
            txtPlantCd.Text = SystemBase.Base.gstrPLANT_CD;
        }
        #endregion

        #region NewExec()
        protected override void NewExec()
        {
            //조회조건 초기화
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            SystemBase.Validation.GroupBox_Reset(groupBox2);

            //기타세팅
            dtpDelvDt.Value = null;
            txtPlantCd.Text = SystemBase.Base.gstrPLANT_CD;
        }
        #endregion
        
        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            Search();
        }

        private void Search()
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {
                    string strQuery = " usp_PSB016  @pTYPE = 'S1'";
                    strQuery += ", @pGROUP_CD = '" + txtItemCd.Text + "' ";
                    strQuery += ", @pPLANT_CD = '" + txtPlantCd.Text + "' ";
                    strQuery += ", @pPROJECT_NO = '" + txtProjectNo.Text + "' ";
                    strQuery += ", @pPROJECT_SEQ = '" + txtProjectSeq.Text + "' ";
                    strQuery += ", @pMAKEORDER_NO = '" + txtMakeOrderNo.Text + "' ";
                    strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' ";
                    strQuery += ", @pWC_CD = '" + txtWcCd.Text + "' ";
                    strQuery += ", @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 5, true);
             
                    int iRowCount = fpSpread1.Sheets[0].Rows.Count;

                    if (iRowCount > 0)
                    {
                        int iDplanWorkTm = 0, iProdWorkTm = 0, iOutWorkTm = 0;			//개발,생산,외주 작업시수
                        int iDplanResultTm = 0, iProdResultTm = 0, iOutResultTm = 0;	//개발,생산,외주 실적시수
                        int iWorkTmSum = 0, iResultSum = 0, iMakeRate = 0;				//총작업시수, 총실적시수, 제품완성율


                        for (int i = 0; i < iRowCount; i++)
                        {
                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "부하시수")].Text != "")
                            {
                                if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "작업장코드")].Text == "R021") //개발이면
                                {
                                    iDplanWorkTm += Convert.ToInt32(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "부하시수")].Value);
                                }
                                else if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "작업장코드")].Text == "R009") //외주이면
                                {
                                    iOutWorkTm += Convert.ToInt32(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "부하시수")].Value);
                                }
                                else
                                {
                                    iProdWorkTm += Convert.ToInt32(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "부하시수")].Value); //생산
                                }

                                iWorkTmSum += Convert.ToInt32(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "부하시수")].Value); //총부하
                            }

                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "실적시수")].Text != "")
                            {
                                if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "작업장코드")].Text == "R021") //개발이면
                                {
                                    iDplanResultTm += Convert.ToInt32(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "실적시수")].Value);
                                }
                                else if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "작업장코드")].Text == "R009") //외주이면
                                {
                                    iOutResultTm += Convert.ToInt32(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "실적시수")].Value);
                                }
                                else
                                {
                                    iProdResultTm += Convert.ToInt32(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "실적시수")].Value); //생산
                                }

                                iResultSum += Convert.ToInt32(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "실적시수")].Value); //총부하
                            }
                        }

                        txtDplanWorkTm.Value = SystemBase.Base.Comma2(Convert.ToString(iDplanWorkTm));
                        txtDplanResultTm.Value = SystemBase.Base.Comma2(Convert.ToString(iDplanResultTm));
                        txtProdWorkTm.Value = SystemBase.Base.Comma2(Convert.ToString(iProdWorkTm));
                        txtProdResultTm.Value = SystemBase.Base.Comma2(Convert.ToString(iProdResultTm));
                        txtOutWorkTm.Value = SystemBase.Base.Comma2(Convert.ToString(iOutWorkTm));
                        txtOutResultTm.Value = SystemBase.Base.Comma2(Convert.ToString(iOutResultTm));

                        txtWorkTmSum.Value = SystemBase.Base.Comma2(Convert.ToString(iWorkTmSum));
                        txtResultSum.Value = SystemBase.Base.Comma2(Convert.ToString(iResultSum));

                        if (iResultSum == 0)
                        {
                            dtxtMakeRate.Value = 0;
                        }
                        else
                        {
                            dtxtMakeRate.Value = (Convert.ToDouble(iResultSum) / Convert.ToDouble(iWorkTmSum));
                        }
                    }
                    else
                    {
                        SystemBase.Validation.GroupBox_Reset(groupBox2);
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            this.Cursor = System.Windows.Forms.Cursors.Default;
        }
        #endregion

        #region 조회조건 팝업
        //공장
        private void btnPlantCd_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pType='P011' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };
                string[] strSearch = new string[] { txtPlantCd.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00005", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "공장 조회");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtPlantCd.Text = Msgs[0].ToString();
                    txtPlantNm.Value = Msgs[1].ToString();

                    txtPlantCd.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "공장 조회 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //프로젝트
        private void btnProject_Click(object sender, EventArgs e)
        {
            try
            {
                WNDW003 pu = new WNDW003(txtProjectNo.Text, "S1", "C");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtEntNm.Value = Msgs[2].ToString() + " (" + Msgs[1].ToString() + ")";
                    txtProjectNo.Text = Msgs[3].ToString();
                    txtProjectNm.Value = Msgs[4].ToString();
                    txtProjectSeq.Text = Msgs[5].ToString();
                    txtItemCd.Text = Msgs[6].ToString();
                    txtItemNm.Value = Msgs[7].ToString();
                    dtpDelvDt.Value = Msgs[12].ToString();
                    txtMakeOrderNo.Text = Msgs[13].ToString();
                    txtOrderQty.Value = Msgs[14].ToString();
                    txtCustNm.Value = Msgs[17].ToString() + " (" + Msgs[16].ToString() + ")";

                    txtProjectNo.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "프로젝트 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //제품오더번호
        private void btnMakeOrderNo_Click(object sender, EventArgs e)
        {
            try
            {
                WNDW008 pu = new WNDW008(txtMakeOrderNo.Text, "C");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtEntNm.Value = Msgs[5].ToString() + " (" + Msgs[4].ToString() + ")";
                    txtProjectNo.Text = Msgs[6].ToString();
                    txtProjectNm.Value = Msgs[7].ToString();
                    txtProjectSeq.Text = Msgs[8].ToString();
                    txtItemCd.Text = Msgs[9].ToString();
                    txtItemNm.Value = Msgs[10].ToString();
                    dtpDelvDt.Value = Msgs[15].ToString();
                    txtMakeOrderNo.Text = Msgs[1].ToString();
                    txtOrderQty.Value = Msgs[2].ToString();
                    txtCustNm.Value = Msgs[17].ToString() + " (" + Msgs[16].ToString() + ")";
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "제품오더번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
   
        //작업장
        private void btnWcCd_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pType='P042', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pETC = 'P061', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };
                string[] strSearch = new string[] { txtWcCd.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00025", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "작업장 조회");
                pu.Width = 500;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtWcCd.Text = Msgs[0].ToString();
                    txtWcNm.Value = Msgs[1].ToString();
                    txtWcCd.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "작업장 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }        
        }
        #endregion

        #region 텍스트박스 코드 입력시 코드명 자동입력
        //공장
        private void txtPlantCd_TextChanged(object sender, EventArgs e)
        {
            txtPlantNm.Value = SystemBase.Base.CodeName("PLANT_CD", "PLANT_NM", "B_PLANT_INFO", txtPlantCd.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");
        }
        //프로젝트번호
        private void txtProjectNo_TextChanged(object sender, EventArgs e)
        {
            txtProjectNm.Value = SystemBase.Base.CodeName("PROJECT_NO", "PROJECT_NM", "S_SO_MASTER", txtProjectNo.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");

            if (txtProjectNm.Text == "")
            {
                txtProjectSeq.Text = "";
                txtItemCd.Text = "";
                txtItemNm.Value = "";
                txtMakeOrderNo.Text = "";
                txtEntNm.Value = "";
                txtCustNm.Value = "";
                txtOrderQty.Value = "";
            }
        }
        //작업장
        private void txtWcCd_TextChanged_1(object sender, EventArgs e)
        {
            string strSql = "and LANG_CD = '" + SystemBase.Base.gstrLangCd + "' and MAJOR_CD = 'P002' AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ";
            txtWcNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtWcCd.Text, strSql);
		}       
        #endregion

    }
}
