#region 작성정보
/*********************************************************************/
// 단위업무명:  부품출고대비실적조회
// 작 성 자  :  한 미 애
// 작 성 일  :  2018-06-01
// 작성내용  :  부품출고된 수량 대비 공정실적에 의한 소비수량 조회
// 수 정 일  :
// 수 정 자  :
// 수정내용  :
// 비    고  :
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

namespace PC.PCC015
{
    public partial class PCC015 : UIForm.FPCOMM1
    {
        private string strMQuery;

        public PCC015()
        {
            InitializeComponent();
        }


        #region Form Load 시
        private void PCC015_Load(object sender, System.EventArgs e)
        {
            // 필수 확인
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            // 기본정보 바인딩
            SystemBase.ComboMake.C1Combo(cboPlantCd, "usp_B_COMMON @pType='B031', @pSPEC1 = '" + SystemBase.Base.gstrBIZCD + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ", 0);//공장
            SystemBase.ComboMake.C1Combo(cboItemAcct, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'B036', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 1); //품목계정

            dtpTRAN_DT_FR.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-4).ToShortDateString().Substring(0,10);
            dtpTRAN_DT_TO.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).ToShortDateString().Substring(0, 10);

            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "단위")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'Z005', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//단위

            cboItemAcct.SelectedValue = "*";       // 품목계정 기본값을 '전체'로.
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
        }
        #endregion


        #region NewExec()
        protected override void NewExec()
        {
            // 초기화
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            // 기본정보 바인딩
            SystemBase.ComboMake.C1Combo(cboPlantCd, "usp_B_COMMON @pType='B031', @pSPEC1 = '" + SystemBase.Base.gstrBIZCD + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//공장
            SystemBase.ComboMake.C1Combo(cboItemAcct, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'B036', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 0); //품목계정

            rdoImDo.Checked = true;
            dtpTRAN_DT_FR.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-4).ToShortDateString().Substring(0,10);
            dtpTRAN_DT_TO.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).ToShortDateString().Substring(0, 10);

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
        }
        #endregion


        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {

            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
            DialogResult dsMsg;
            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {
                    string strInOutFlag = "";
                    if (rdoDo.Checked == true)
                        strInOutFlag = "1";
                    else if (rdoIm.Checked == true)
                        strInOutFlag = "2";
                    else if (rdoEtc.Checked == true)
                        strInOutFlag = "3";
                    else if (rdoImDo.Checked == true)
                        strInOutFlag = "4";

                    string strOverQtyYn = "N";
                    if (chkOVER_YN.Checked == true)
                        strOverQtyYn = "Y";

                    strMQuery = " usp_PCC015 'S1'";
                    strMQuery += ", @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                    strMQuery += ", @pPLANT_CD='" + cboPlantCd.SelectedValue.ToString() + "'";
                    strMQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'";
                    strMQuery += ", @pTRAN_DT_FR='" + dtpTRAN_DT_FR.Text.ToString() + "'";
                    strMQuery += ", @pTRAN_DT_TO='" + dtpTRAN_DT_TO.Text.ToString() + "'";
                    strMQuery += ", @pITEM_CD='" + txtITEM_CD.Text.Trim() + "'";
                    strMQuery += ", @pITEM_ACCT ='" + cboItemAcct.SelectedValue.ToString() + "'";
                    strMQuery += ", @pITEM_TYPE ='" + txtItemTypeCd.Text + "'";
                    strMQuery += ", @pIN_OUT_FLAG = '" + strInOutFlag + "'";            // 내/외자구분
                    strMQuery += ", @pWORKORDER_NO_FR ='" + txtWoNoFr.Text + "'";
                    strMQuery += ", @pWORKORDER_NO_TO ='" + txtWoNoTo.Text + "'";
                    strMQuery += ", @pMAKEORDER_NO = '" + txtMakeorder_No.Text + "'";
                    strMQuery += ", @pPROJECT_NO='" + txtProject_No.Text + "'";
                    strMQuery += ", @pPROJECT_SEQ ='" + txtProject_Seq.Text + "'";
                    strMQuery += ", @pGROUP_CD='" + txtGroup_CD.Text + "'";                    
                    strMQuery += ", @pPRNT_ITEM_CD = '" + txtPrntItemCd.Text + "'";
                    strMQuery += ", @pOVER_YN = '" + strOverQtyYn + "'";            // 초과실적소비건만 여부

                    UIForm.FPMake.grdCommSheet(fpSpread1, strMQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 8, true);
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            this.Cursor = System.Windows.Forms.Cursors.Default;

        }
        #endregion


        #region 버튼 Click
        #region btnITEM_Click(): 품목코드 버튼 클릭 이벤트
        private void btnITEM_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW005 pu = new WNDW005();
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtITEM_CD.Text = Msgs[2].ToString();
                    txtITEM_NM.Value = Msgs[3].ToString();
                    txtITEM_CD.Focus();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region btnProject_Click(): 프로젝트 버튼 클릭 이벤트
        private void btnProject_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW003 pu = new WNDW003(txtProject_No.Text, "S1", "C");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtProject_No.Text = Msgs[3].ToString();
                    txtProject_Name.Value = Msgs[4].ToString();
                    txtProject_Seq.Text = Msgs[5].ToString();
                    txtGroup_CD.Text = Msgs[6].ToString();
                    txtGROUP_NM.Value = Msgs[7].ToString();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region btnWoNoFr_Click(): 제조오더번호FROM 버튼 클릭 이벤트
        private void btnWoNoFr_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW006 pu = new WNDW006(txtWoNoFr.Text);
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtWoNoFr.Text = Msgs[1].ToString();
                    txtWoNoFr.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "제조오더번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region btnWoNoTo_Click(): 제조오더번호FROM 버튼 클릭 이벤트
        private void btnWoNoTo_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW006 pu = new WNDW006(txtWoNoTo.Text);
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtWoNoTo.Text = Msgs[1].ToString();
                    txtWoNoTo.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "제조오더번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region btnPrntItem_Click(): 모품목코드 버튼 클릭 이벤트
        private void btnPrntItem_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW005 pu = new WNDW005();
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtPrntItemCd.Text = Msgs[2].ToString();
                    txtPrntItemNm.Value = Msgs[3].ToString();
                    txtPrntItemCd.Focus();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region btnPrntItem_Click(): 모품목코드 버튼 클릭 이벤트
        private void btnItemType_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON @pType='COMM_POP', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pSPEC1 = 'P032', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtItemTypeCd.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00105", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "품목구분 조회", false);

                pu.ShowDialog();	//공통 팝업 호출
                if (pu.DialogResult == DialogResult.OK)
                {
                    string MSG = pu.ReturnVal.Replace("|", "#");
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(MSG);

                    txtItemTypeCd.Text = Msgs[0].ToString();
                    txtItemTypeNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목구분팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region btnMakeorder_No_Click(): 제품오더번호 버튼 클릭 이벤트
        private void btnMakeorder_No_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW008 pu = new WNDW008(txtMakeorder_No.Text);
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtMakeorder_No.Text = Msgs[1].ToString();
                    txtMakeorder_No.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "제품오더번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region btnMakeorder_No_Click(): 제품코드 버튼 클릭 이벤트
        private void btnGroup_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW005 pu = new WNDW005("10");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtGroup_CD.Text = Msgs[2].ToString();
                    txtGROUP_NM.Value = Msgs[3].ToString();
                    txtGroup_CD.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion
        #endregion


        #region 텍스트 변경
        #region txtITEM_CD_TextChanged(): 모품목코드 버튼 클릭 이벤트
        // 부품
        private void txtITEM_CD_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtITEM_CD.Text != "")
                {
                    txtITEM_NM.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtITEM_CD.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");
                }
                else
                {
                    txtITEM_NM.Value = "";
                }
            }
            catch { }
        }
        #endregion

        #region txtITEM_CD_TextChanged(): 품목코드 변경 이벤트
        private void txtGroup_CD_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtGroup_CD.Text != "")
                {
                    txtGROUP_NM.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtGroup_CD.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");
                }
                else
                {
                    txtGROUP_NM.Value = "";
                }
            }
            catch { }
        }
        #endregion

        #region txtITEM_CD_TextChanged(): 프로젝트번호 변경 이벤트
        private void txtProject_No_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtProject_No.Text != "")
                {
                    txtProject_Name.Value = SystemBase.Base.CodeName("PROJECT_NO", "PROJECT_NM", "S_SO_MASTER", txtProject_No.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");
                }
                else
                {
                    txtProject_Name.Value = "";
                }
                if (txtProject_Name.Text == "")
                    txtProject_Seq.Text = "";
            }
            catch { }
        }
        #endregion

        #region txtPrntItemCd_TextChanged(): 모품목코드 변경 이벤트
        private void txtPrntItemCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtPrntItemCd.Text != "")
                {
                    txtPrntItemNm.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtPrntItemCd.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");
                }
                else
                {
                    txtPrntItemNm.Value = "";
                }
            }
            catch { }
        }
        #endregion

        #region txtItemTypeCd_TextChanged(): 품목구분 변경 이벤트
        private void txtItemTypeCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtItemTypeCd.Text != "")
                {
                    txtItemTypeNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtItemTypeCd.Text, " AND MAJOR_CD = 'P032' AND COMP_CODE='" + SystemBase.Base.gstrCOMCD + "' ");
                }
                else
                {
                    txtItemTypeNm.Value = "";
                }
            }
            catch { }
        }
        #endregion
        #endregion

    }
}
