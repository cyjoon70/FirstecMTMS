#region 작성정보
/*********************************************************************/
// 단위업무명 : 통합오더조회
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-02-04
// 작성내용 : 통합오더조회 관리
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

namespace PB.PSB017
{
    public partial class PSB017 : UIForm.FPCOMM1
    {
        #region 생성자
        public PSB017()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void PSB017_Load(object sender, System.EventArgs e)
        {  
            //GroupBox1 초기화
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            
            //조회조건 콤보
            SystemBase.ComboMake.C1Combo(cboSCH_ID, "usp_P_COMMON 'P520', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 3);

            //그리드초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);	
            
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

        //대표오더번호
        private void btnRepsn_Ord_NO_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW.WNDW028 pu = new WNDW.WNDW028();
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtRepsn_Ord_NO.Value = Msgs[1].ToString();
                    txtRepsn_Ord_NO.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "대표오더정보조회 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion
        
        #region 조회조건 TextChanged
        //프로젝트번호
        private void txtProjectNo_TextChanged(object sender, System.EventArgs e)
        {    
            try
            {
                if (txtProjectNo.Text != "")
                {
                    txtProjectNm.Value = SystemBase.Base.CodeName("PROJECT_NO", "PROJECT_NM", "S_SO_MASTER", txtProjectNo.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtProjectNm.Value = "";
                }
                if (txtProjectNm.Text == "") txtProjectSeq.Text = "";
            }
            catch { }
        }

        //품목코드
        private void txtItemCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtItemCd.Text != "")
                {
                    txtItemNm.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtItemCd.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtItemNm.Value = "";
                }
            }
            catch { }
        }
        #endregion

        #region NewExec()
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            fpSpread1.Sheets[0].Rows.Count = 0;
        }
        #endregion
        
        #region SearchExec() 그리드 조회 로직
		protected override void SearchExec() 
		{
			this.Cursor = Cursors.WaitCursor;
			
			try
			{
				string strQuery = " usp_PSB017  @pTYPE = 'S1'";
				strQuery += ", @pPROJECT_NO = '"+ txtProjectNo.Text + "' ";
				strQuery += ", @pPROJECT_SEQ = '"+ txtProjectSeq.Text + "' ";
				strQuery += ", @pWORKORDER_NO_RS = '"+ txtRepsn_Ord_NO.Text + "' ";
				strQuery += ", @pITEM_CD = '" + txtItemCd.Text + "' ";
				strQuery += ", @pSCH_ID = '" + Convert.ToString(cboSCH_ID.SelectedValue) + "'";
                strQuery += ", @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

				UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 1, true);

				txtOrder_cnt.Value = 0;

				if(fpSpread1.Sheets[0].Rows.Count > 0)
				{
					for(int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
					{
						if(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1,"수량")].Text != "")
							txtOrder_cnt.Value = Convert.ToDouble(txtOrder_cnt.Value) + Convert.ToDouble(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1,"수량")].Value);
					}
				}
				else
				{
					txtOrder_cnt.Value = 0;
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
