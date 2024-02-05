#region 작성정보
/*********************************************************************/
// 단위업무명 : 구매성과지표
// 작 성 자 : 김현근
// 작 성 일 : 2013-04-17
// 작성내용 : 구매성과지표
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

namespace MT.MRT004
{
    public partial class MRT004 : UIForm.FPCOMM1
    {
        public MRT004()
        {
            InitializeComponent();
        }

        #region Form Load 시
        private void MRT004_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            //그리드 초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

            //기타 세팅	
            dtpDeliveryDtFr.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToString();
            dtpDeliveryDtTo.Value = SystemBase.Base.ServerTime("YYMMDD");
        }
        #endregion

        #region NewExec()
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            //그리드 초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

            //기타 세팅	
            dtpDeliveryDtFr.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToString();
            dtpDeliveryDtTo.Value = SystemBase.Base.ServerTime("YYMMDD");
        }
        #endregion

        #region 조회조건 팝업         
        //프로젝트번호
        private void btnProjectNo_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW007 pu = new WNDW007(txtProjectNo.Text);
                pu.MaximizeBox = false;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtProjectNo.Text = Msgs[3].ToString();
                    txtProjectNm.Value = Msgs[4].ToString();
                    txtProjectSeq.Text = "";
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "프로젝트번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //프로젝트차수
        private void btnProjectSeq_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON 'PROJ_SEQ', @pSPEC1 = '" + txtProjectNo.Text + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";										// 쿼리
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
                    txtProjectSeq.Text = Msgs[0].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }
       
        //거래처 FROM
        private void btnCustCdFr_Click(object sender, EventArgs e)
        {
            try
            {
                WNDW002 pu = new WNDW002(txtCustCdFr.Text, "");
                pu.MaximizeBox = false;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtCustCdFr.Text = Msgs[1].ToString();
                    txtCustNmFr.Value = Msgs[2].ToString();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "거래처 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }
        //거래처 TO
        private void btnCustCdTo_Click(object sender, EventArgs e)
        {
            try
            {
                WNDW002 pu = new WNDW002(txtCustCdTo.Text, "");
                pu.MaximizeBox = false;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtCustCdTo.Text = Msgs[1].ToString();
                    txtCustNmTo.Value = Msgs[2].ToString();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "거래처 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        #endregion

        #region 조회조건 TextChanged
        //거래처 FROM
        private void txtCustCdFr_TextChanged(object sender, EventArgs e)
        {
            txtCustNmFr.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtCustCdFr.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
        }
        //거래처 TO
        private void txtCustCdTo_TextChanged(object sender, EventArgs e)
        {
            txtCustNmTo.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtCustCdTo.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
        }
        //프로젝트번호
        private void txtProjectNo_TextChanged(object sender, EventArgs e)
        {
            txtProjectNm.Value = SystemBase.Base.CodeName("PROJECT_NO", "PROJECT_NM", "S_SO_MASTER", txtProjectNo.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");

            if (txtProjectNm.Value == "")
                txtProjectSeq.Text = "";
        }
        #endregion

        #region SearchExec()
        protected override void SearchExec()
        {
            //조회조건 필수 체크
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                this.Cursor = Cursors.WaitCursor;
                string strPoType = "";

                try
                {
                    if (rdoPoTypePo.Checked == true) strPoType = "PO";
                    else if (rdoPoTypeDo.Checked == true) strPoType = "DO";
                    else if (rdoPoTypeOther.Checked == true) strPoType = "OTHER";

                    string strQuery = "usp_MRT004 @pTYPE = 'S1'";
                    strQuery += ", @pDELIVERY_DT_FR = '" + dtpDeliveryDtFr.Text + "'";
                    strQuery += ", @pDELIVERY_DT_TO = '" + dtpDeliveryDtTo.Text + "'";
                    strQuery += ", @pCUST_CD_FR = '" + txtCustCdFr.Text + "'";
                    strQuery += ", @pCUST_CD_TO = '" + txtCustCdTo.Text + "'";
                    strQuery += ", @pPO_TYPE = '" + strPoType + "'";
                    strQuery += ", @pPROJECT_NO = '" + txtProjectNo.Text + "'";
                    strQuery += ", @pPROJECT_SEQ = '" + txtProjectSeq.Text + "'";
                    strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0, true);
                
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회중 오류가 발생하였습니다.
                }

                //납기준수율
                if (fpSpread1.Sheets[0].RowCount > 0)
                {
                    int iRow = fpSpread1.Sheets[0].RowCount;
                    double iRatio = 0;

                    for (int i = 0; i < iRow; i++)
                    {
                        iRatio += Convert.ToDouble(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "납기준수율(%)")].Value);
                    }

                    dtxtRatio.Value = iRatio / iRow;
                }
                else
                {
                    dtxtRatio.Value = 0;
                }

                this.Cursor = Cursors.Default;

                this.Cursor = Cursors.Default;
            }
        }
        #endregion

    }
}
