#region 작성정보
/*********************************************************************/
// 단위업무명 : 프로젝트종료현황
// 작 성 자 : 권 순 철
// 작 성 일 : 2013-04-11
// 작성내용 : 프로젝트종료현황 및 관리
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

namespace IN.INV123
{
    public partial class INV123 : UIForm.FPCOMM1
    {
        #region 변수선언
        bool form_act_chk = false;
        #endregion

        public INV123()
        {
            InitializeComponent();
        }

        #region Form Load시
        private void INV123_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            SystemBase.Validation.GroupBox_Setting(groupBox2);

            SystemBase.ComboMake.C1Combo(cboPlantCd, "usp_B_COMMON @pType='PLANT', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ", 9);//공장

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

            //기타 세팅
            rdoAll.Checked = true;
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            SystemBase.Validation.GroupBox_Reset(groupBox2);
            fpSpread1.Sheets[0].Rows.Count = 0;
            //기타 세팅
            rdoAll.Checked = true;
        }
        #endregion

        #region SearchExec 그리드 조회
        protected override void SearchExec()
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {
                    string strQuery1 = " usp_INV123 'S1'";
                    string strQuery2 = ", @pPLANT_CD ='" + cboPlantCd.SelectedValue.ToString() + "'";

                    if (rdoY.Checked == true)
                        strQuery2 += ", @pCLOSE_YN ='Y'";
                    else if (rdoN.Checked == true)
                        strQuery2 += ", @pCLOSE_YN ='N'";

                    strQuery2 += ", @pPROJECT_NO ='" + txtProject_No.Text.Trim() + "'";

                    if (txtOutQty.Text != "")
                        strQuery2 += ", @pCNT ='" + txtOutQty.Value + "'";

                    strQuery2 += ", @pDT = '" + mskDT.Text + "'";
                    strQuery2 += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery1 + strQuery2, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 2, true);
                    fpSpread1.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.Normal;
                    if (fpSpread1.Sheets[0].RowCount > 0)
                    {

                        strQuery1 = " usp_INV123 'S2'" + strQuery2;
                        DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery1);

                        if (dt.Rows.Count > 0)
                        {
                            txtStockAmt.Value = dt.Rows[0][0];
                        }
                        else
                        {
                            txtStockAmt.Text = "";
                        }
                        Set_Color();
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
            this.Cursor = Cursors.Default;
        }

		private void Set_Color()
		{
			int col_idx = SystemBase.Base.GridHeadIndex(GHIdx1, "종료여부");
		
			for(int i = 0; i < fpSpread1.Sheets[0].RowCount ; i ++)
			{
				if(fpSpread1.Sheets[0].Cells[i, col_idx].Text == "Y")
				{
					for(int j = 0; j <fpSpread1.Sheets[0].ColumnCount; j++)
					{
						fpSpread1.Sheets[0].Cells[i,j].ForeColor =  Color.Red;
					}
				}
			}

		}
		#endregion

		#region 버튼 Click  
		// 프로젝트
		private void btnProject_Click(object sender, System.EventArgs e)
		{
			try
			{
				WNDW007 pu = new WNDW007(txtProject_No.Text) ;
				pu.ShowDialog();	
				if(pu.DialogResult==DialogResult.OK)
				{
					string[] Msgs	= pu.ReturnVal;
					txtProject_No.Text	= Msgs[3].ToString();
					txtProject_Nm.Value	= Msgs[4].ToString();
				}	
			}
			catch(Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
				//데이터 조회 중 오류가 발생하였습니다.
			}
		}
		#endregion

		#region TextChanged
		private void txtProject_No_TextChanged(object sender, System.EventArgs e)
		{
            txtProject_Nm.Value = SystemBase.Base.CodeName("PROJECT_NO", "PROJECT_NM", "S_SO_MASTER", txtProject_No.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");				
		}
		#endregion

        #region Form Activated & Deactivate
        private void INV123_Activated(object sender, System.EventArgs e)
        {
            if (form_act_chk == false) cboPlantCd.Focus();
        }

        private void INV123_Deactivate(object sender, System.EventArgs e)
        {
            form_act_chk = true;
        }
        #endregion


    }
}
