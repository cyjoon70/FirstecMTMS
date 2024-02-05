#region 작성정보
/*********************************************************************/
// 단위업무명 : 가공품실무게관리
// 작 성 자 : 김현근
// 작 성 일 : 2013-06-05
// 작성내용 : 가공품실무게관리 및 조회
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


namespace IBF.IBFB09
{
    public partial class IBFB09 : UIForm.FPCOMM1
    {
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private bool chk = false;
        public IBFB09()
        {
            InitializeComponent();
        }

        #region Form Load 시
        private void IBFB09_Load(object sender, System.EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(groupBox1);	//컨트롤 필수 Setting
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
           
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Base.GroupBoxReset(groupBox1);
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
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
                     string strQuery = " usp_IBFB09  'S1',";
                     strQuery = strQuery + " @pTRACKING_NO = '" + txtTRNo.Text + "',  ";
                     strQuery = strQuery + " @pDT_FR = '" + txtDT_FR.Text + "',";
                     strQuery = strQuery + " @pDT_TO = '" + txtDT_TO.Text + "',";
                     strQuery = strQuery + " @pNO_FR = '" + txtNO_FR.Text + "',";
                     strQuery = strQuery + " @pNO_TO = '" + txtNO_TO.Text + "' ";
                     strQuery = strQuery + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                     UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0);

                     if (fpSpread1.Sheets[0].Rows.Count > 0) Compute_Rest_Qty();
                 }
             }
             catch (Exception f)
             {
                 SystemBase.Loggers.Log(this.Name, f.ToString());
                 MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
             }

            this.Cursor = Cursors.Default;
            fpSpread1.Focus();
        }
        #endregion

        private void Compute_Rest_Qty()
        {
            decimal sum_qty = 0;
            decimal dblshort_qty = 0;
            string strITEM_CD = "";
            try
            {
                for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                {
                    if (Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수량_2")].Value) >= (Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수량_3")].Value) + Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수량_4")].Value))) dblshort_qty = 0;
                    else dblshort_qty = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수량_3")].Value) + Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수량_4")].Value) - Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수량_2")].Value);

                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수량_4")].Value = dblshort_qty;

                    if (i == 0 || strITEM_CD != fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "자품목코드")].Text)
                    {
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수량_5")].Value = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수량_7")].Value); ;
                        sum_qty = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수량_7")].Value);
                    }
                    else
                    {
                        sum_qty += Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수량_7")].Value);
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수량_5")].Value = sum_qty;
                    }

                    strITEM_CD = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "자품목코드")].Text;

                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수량_6")].Value = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수량_4")].Value) - sum_qty;

                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(f.ToString());
            }
        }

        #region 팝업창 열기
        private void btnTRNo_Click(object sender, EventArgs e)
        {
            try
            {
                //Tracking No. 팝업
                this.Cursor = Cursors.WaitCursor;
                string strQuery = " Nusp_BF_Comm 'BF11' ";
                string[] strWhere = new string[] { "@pValue" };
                string[] strSearch = new string[] { txtTRNo.Text };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("BFP001", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "Tracking No.팝업");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtTRNo.Text = Msgs[0].ToString();
                    txtBUSINESS_CD.Value = Msgs[7].ToString();
                    txtBUSINESS_NM.Value = Msgs[8].ToString();
                }

                this.Cursor = Cursors.Default;
            }
            catch (Exception f)
            {
                this.Cursor = Cursors.Default;
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(f.ToString());
            }
        }
        #endregion

        private void IBFB09_Activated(object sender, System.EventArgs e)
        {
            if (chk == false)
            {
                txtTRNo.Focus();
            }
        }

        private void IBFB09_Deactivate(object sender, System.EventArgs e)
        {
            chk = true;
        }

        private void txtBASED_NO_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && txtTRNo.Text.Trim() != "") SearchExec(); 
        }

        private void txtTRNo_Leave(object sender, EventArgs e)
        {
            try
            {
                if (txtTRNo.Text.Trim() != "")
                {
                    string strSql = "Select ENT_CD, ENT_NM  From MTMS_FT.dbo.UVW_S_PROJECT_ENT  Where PROJECT_NO = '" + txtTRNo.Text.Trim() + "' AND BONDED_YN = 'Y' AND Rtrim(ENT_NM) <> '' ";
                    strSql += " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                    DataSet ds = SystemBase.DbOpen.NoTranDataSet(strSql);

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        txtBUSINESS_CD.Text = ds.Tables[0].Rows[0][0].ToString();
                        txtBUSINESS_NM.Text = ds.Tables[0].Rows[0][1].ToString();
                    }

                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(f.ToString());
            }
        }

        private void txtTRNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && txtTRNo.Text.Trim() != "") SearchExec(); 
        }

    

    }
}
