#region 작성정보
/*********************************************************************/
// 단위업무명 : 공정별현황조회(APS)
// 작 성 자 : 김현근
// 작 성 일 : 2013-04-15
// 작성내용 : 공정별현황조회(APS)
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

namespace PB.PBC110
{
    public partial class PBC110 : UIForm.FPCOMM1
    {
        public PBC110()
        {
            InitializeComponent();
        }
         
        #region Form Load 시
        private void PBC110_Load(object sender, System.EventArgs e)
        {  
            //GroupBox1 초기화
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            txtPlantCd.Text = SystemBase.Base.gstrPLANT_CD;

            SystemBase.ComboMake.C1Combo(cboSchId, "usp_P_COMMON @pTYPE = 'P081' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 3, true);
            SystemBase.ComboMake.C1Combo(cboSTATUS, "usp_B_COMMON @pTYPE	= 'COMM2', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCODE = 'P012' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0); //진행상태

            //그리드초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

            //기타세팅
            cboSTATUS.SelectedValue = "R";
        }
        #endregion

        #region NewExec()
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            fpSpread1.Sheets[0].Rows.Count = 0;

            txtPlantCd.Text = SystemBase.Base.gstrPLANT_CD.ToString();
            cboSTATUS.SelectedValue = "R";
        }
        #endregion

        #region 조회조건팝업
        //프로젝트번호
        private void btnProjectNo_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW003 pu = new WNDW003(txtProjectNo.Text, "S1", "S");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtProjectNo.Text = Msgs[3].ToString();
                    txtProjectNm.Value = Msgs[4].ToString();
                    txtProjectSeq.Text = Msgs[5].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "프로젝트 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //품목
        private void btnItemCd_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW005 pu = new WNDW005(txtItemCd.Text, "");
                pu.MaximizeBox = false;
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
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목코드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }
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
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "공장 조회 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //제품코드
        private void btnGroupCd_Click(object sender, EventArgs e)
        {
            try
            {
                WNDW005 pu = new WNDW005(txtPlantCd.Text, true, txtGroupCd.Text);
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtGroupCd.Text = Msgs[2].ToString();
                    txtGroupNm.Value = Msgs[3].ToString();
                    txtGroupCd.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목코드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //생산담당자
        private void btnUserId_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON 'B011' ,@pSPEC1='" + SystemBase.Base.gstrBIZCD + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtUserId.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00031", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "생산담당자 팝업");
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
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "생산담당자 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        //제품오더번호 FROM
        private void btnMakeorderNoFr_Click(object sender, EventArgs e)
        {
            try
            {
                WNDW008 pu = new WNDW008(txtMakeorderNoFr.Text, "R");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtMakeorderNoFr.Text = Msgs[1].ToString();
                    //					txtProjectNo.Text = Msgs[6].ToString();
                    //					txtProjectNm.Text = Msgs[7].ToString();
                    //					txtProjectSeqFr.Text = Msgs[8].ToString();
                    //					txtItemCd.Text = Msgs[9].ToString();
                    //					txtItemNm.Text = Msgs[10].ToString();
                    //					txtMakeorderNoFr.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "제품오더번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //제품오더번호 TO
        private void btnMakeorderNoTo_Click(object sender, EventArgs e)
        {
            try
            {
                WNDW008 pu = new WNDW008(txtMakeorderNoTo.Text, "R");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtMakeorderNoTo.Text = Msgs[1].ToString();
                    txtMakeorderNoTo.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "제품오더번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //작업장
        private void btnWc_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pType='P042', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pETC = 'P002' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
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

        //자원
        private void btnRES_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pTYPE = 'P056', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };
                string[] strSearch = new string[] { txtRES_CD.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00068", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "자원 조회");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtRES_CD.Text = Msgs[0].ToString();
                    txtRES_DIS.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "자원 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion
        
        #region 조회조건 TextChanged
        //공장
        private void txtPlantCd_TextChanged(object sender, EventArgs e)
        {
            txtPlantNm.Value = SystemBase.Base.CodeName("PLANT_CD", "PLANT_NM", "B_PLANT_INFO", txtPlantCd.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "'");
        }

        //프로젝트번호
        private void txtProjectNo_TextChanged(object sender, EventArgs e)
        {
            txtProjectNm.Value = SystemBase.Base.CodeName("PROJECT_NO", "PROJECT_NM", "S_SO_MASTER", txtProjectNo.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "'");
        }

        //품목코드
        private void txtItemCd_TextChanged(object sender, EventArgs e)
        {
            txtItemNm.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtItemCd.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "'");
        }

        //제품코드
        private void txtGroupCd_TextChanged(object sender, EventArgs e)
        {
            txtGroupNm.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtGroupCd.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "'");
        }
        //생산담당자
        private void txtUserId_TextChanged(object sender, EventArgs e)
        {
            txtUserNm.Value = SystemBase.Base.CodeName("USR_ID", "USR_NM", "B_SYS_USER", txtUserId.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "'");
        }
        //작업장
        private void txtWcCd_TextChanged(object sender, EventArgs e)
        {
            txtWcNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtWcCd.Text, " AND MAJOR_CD = 'P002' AND COMP_CODE='" + SystemBase.Base.gstrCOMCD + "'");
        }
        //자원
        private void txtRES_CD_TextChanged(object sender, EventArgs e)
        {
            txtRES_DIS.Value = SystemBase.Base.CodeName("RES_CD", "RES_DIS", "P_RESO_MANAGE", txtRES_CD.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "'");
        }
        #endregion
        
        #region SearchExec() 그리드 조회 로직
		protected override void SearchExec() 
		{
			this.Cursor = Cursors.WaitCursor;
			
			try
			{
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {
                    string strMQuery = "";
                    strMQuery = "   usp_PBC110 @pTYPE = 'S1'";
                    strMQuery += ",            @pDT_FR = '" + dtpDtFr.Text + "' ";
                    strMQuery += ",            @pDT_TO = '" + dtpDtTo.Text + "' ";
                    strMQuery += ",            @pSCH_ID = '" + cboSchId.SelectedValue + "' ";
                    strMQuery += ",            @pWORKORDER_NO = '" + txtWorkOrderNo.Text + "' ";
                    strMQuery += ",            @pITEM_CD = '" + txtItemCd.Text + "' ";
                    strMQuery += ",            @pGROUP_CD = '" + txtGroupCd.Text + "' ";
                    strMQuery += ",            @pWC_CD = '" + txtWcCd.Text + "' ";
                    strMQuery += ",            @pPLANT_CD = '" + txtPlantCd.Text + "' ";
                    strMQuery += ",            @pPROJECT_NO = '" + txtProjectNo.Text + "' ";
                    strMQuery += ",            @pPROJECT_SEQ = '" + txtProjectSeq.Text + "' ";
                    strMQuery += ",            @pRES_CD = '" + txtRES_CD.Text + "' ";
                    strMQuery += ",            @pMPS_STATUS = '" + Convert.ToString(cboSTATUS.SelectedValue) + "'";
                    strMQuery += ",            @pMF_PLAN_USER = '" + txtUserId.Text + "' ";
                    strMQuery += ",			   @pMAKEORDER_NO_FR = '" + txtMakeorderNoFr.Text + "'";
                    strMQuery += ",			   @pMAKEORDER_NO_TO = '" + txtMakeorderNoTo.Text + "'";
                    strMQuery += ",            @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";


                    UIForm.FPMake.grdCommSheet(fpSpread1, strMQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 1, true);
                }
			}
			catch(Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
				//데이터 조회 중 오류가 발생하였습니다.
			}
		
			this.Cursor = Cursors.Default;	
		}
		#endregion

    }
}
