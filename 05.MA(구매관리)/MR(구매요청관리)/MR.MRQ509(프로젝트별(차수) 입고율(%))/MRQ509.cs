#region 작성정보
/*********************************************************************/
// 단위업무명 : 구매요청진행조회
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-02-14
// 작성내용 : 구매요청진행조회 관리
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

namespace MR.MRQ509
{
    public partial class MRQ509 : UIForm.FPCOMM1
    {
        #region 생성자
        public MRQ509()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void MRQ509_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            
            //그리드 초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

            //기타 세팅	
            txtPlantCd.Text = SystemBase.Base.gstrPLANT_CD;
            dtpReqDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString().Substring(0,10);
            dtpReqDtTo.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).ToShortDateString().Substring(0,10);
            dtpDeliveryDtFr.Text = null;
            dtpDeliveryDtTo.Text = null;

            chkItemDiv.Checked = true;

        }
        #endregion

        #region NewExec()
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            //그리드 초기화
            fpSpread1.Sheets[0].Rows.Count = 0;

            //기타 세팅	
            txtPlantCd.Text = SystemBase.Base.gstrPLANT_CD;
            dtpReqDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString().Substring(0,10);
            dtpReqDtTo.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).ToShortDateString().Substring(0,10);
            dtpDeliveryDtFr.Text = null;
            dtpDeliveryDtTo.Text = null;
            dtpDelvDtFr.Text = null;
            dtpDelvDtTo.Text = null;
            chkItemDiv.Checked = true;

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
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "공장코드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);	//데이터 조회 중 오류가 발생하였습니다.

            }
        }



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
                    txtProjectSeqFR.Text = "";
                    txtProjectSeqTO.Text = "";
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
                    txtProjectSeqFR.Text = Msgs[0].ToString();
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
        //공장
        private void txtPlantCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtPlantCd.Text != "")
                {
                    txtPlantNm.Value = SystemBase.Base.CodeName("PLANT_CD", "PLANT_NM", "B_PLANT_INFO", txtPlantCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtPlantNm.Value = "";
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
                    txtProjectNm.Value = SystemBase.Base.CodeName("PROJECT_NO", "PROJECT_NM", "S_SO_MASTER", txtProjectNo.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtProjectNm.Value = "";
                }
                if (txtProjectNm.Text == "")
                    txtProjectSeqFR.Text = "";
            }
            catch
            {

            }
        }

 

        #endregion

        #region SearchExec()
        protected override void SearchExec()
        {
            //조회조건 필수 체크
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                this.Cursor = Cursors.WaitCursor;

                try
                {
                    string strCfm = "";
                    string strCloseYn = "";

                    if (rdoCloseYes.Checked == true) { strCloseYn = "Y"; }
                    else if (rdoCloseNo.Checked == true) { strCloseYn = "N"; }

                    string strQuery = "usp_MRQ509 @pTYPE = 'S1'";
                    strQuery += ", @pREQ_DT_FR = '" + dtpReqDtFr.Text + "'";
                    strQuery += ", @pREQ_DT_TO = '" + dtpReqDtTo.Text + "'";
                    strQuery += ", @pPROJECT_NO = '" + txtProjectNo.Text + "'";
                    strQuery += ", @pPROJECT_SEQFR = '" + txtProjectSeqFR.Text + "'";
                    strQuery += ", @pPROJECT_SEQTO = '" + txtProjectSeqTO.Text + "'";
                    strQuery += ", @pPLANT_CD = '" + txtPlantCd.Text + "'";
                    strQuery += ", @pDELIVERY_DT_FR = '" + dtpDeliveryDtFr.Text + "'";
                    strQuery += ", @pDELIVERY_DT_TO = '" + dtpDeliveryDtTo.Text + "'";
                    strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'";
                    strQuery += ", @pCLOSE_YN = '" + strCloseYn + "'";
                    strQuery += ", @pDELV_DT_FR = '" + dtpDelvDtFr.Text + "'";
                    strQuery += ", @pDELV_DT_TO = '" + dtpDelvDtTo.Text + "'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    strQuery += ", @pDIV = '" + strCfm + "'";

                    string strItemDiv1 = "";
                    if (chkItemDiv1.Checked == true) strItemDiv1 = chkItemDiv1.Text;
                    string strItemDiv2 = "";
                    if (chkItemDiv2.Checked == true) strItemDiv2 = chkItemDiv2.Text;
                    string strItemDiv3 = "";
                    if (chkItemDiv3.Checked == true) strItemDiv3 = chkItemDiv3.Text;

                    strQuery += ", @pITEM_DIV1 = '" + strItemDiv1 + "'";
                    strQuery += ", @pITEM_DIV2 = '" + strItemDiv2 + "'";
                    strQuery += ", @pITEM_DIV3 = '" + strItemDiv3 + "'";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, true, true, 0, 0, true);


                    if (fpSpread1.Sheets[0].Rows.Count > 0)
                    {
                        int Per = 0;

                        for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count-1; i++)
                        {
                            Per = Per + Convert.ToInt32(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "입고율")].Value);
                        }

                        Per = Per / (fpSpread1.Sheets[0].Rows.Count-1);

                        txtPer.Value = Per.ToString();
                    }
                    else
                    {
                        txtPer.Value = "";
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회중 오류가 발생하였습니다.
                }

                this.Cursor = Cursors.Default;
            }

        }
        #endregion       

        #region 체크박스 CheckedChanged

        private void chkItemDiv_CheckedChanged(object sender, System.EventArgs e)
        {
            if (chkItemDiv.Checked == true)
            {
                chkItemDiv1.Checked = false;
                chkItemDiv2.Checked = false;
                chkItemDiv3.Checked = false;
            }
        }

        private void chkItemDiv1_CheckedChanged(object sender, System.EventArgs e)
        {
            if (chkItemDiv1.Checked == true)
                chkItemDiv.Checked = false;
            if (chkItemDiv1.Checked == false && chkItemDiv2.Checked == false && chkItemDiv3.Checked == false)
            {
                chkItemDiv.Checked = true;
            }
        }

        private void chkItemDiv2_CheckedChanged(object sender, System.EventArgs e)
        {
            if (chkItemDiv2.Checked == true)
                chkItemDiv.Checked = false;
            if (chkItemDiv1.Checked == false && chkItemDiv2.Checked == false && chkItemDiv3.Checked == false)
            {
                chkItemDiv.Checked = true;
            }
        }

        private void chkItemDiv3_CheckedChanged(object sender, System.EventArgs e)
        {
            if (chkItemDiv3.Checked == true)
                chkItemDiv.Checked = false;
            if (chkItemDiv1.Checked == false && chkItemDiv2.Checked == false && chkItemDiv3.Checked == false)
            {
                chkItemDiv.Checked = true;
            }
        }

        #endregion

        private void btnProjectSeqFrom_Click(object sender, EventArgs e)
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
                    txtProjectSeqTO.Text = Msgs[0].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }


    }
}
