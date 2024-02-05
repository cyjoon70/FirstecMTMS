#region DAC001P1 작성 정보
/*************************************************************/
// 단위업무명 : 오더번호 조회
// 작 성 자 :   유재규
// 작 성 일 :   2013-02-14
// 작성내용 :   
// 수 정 일 :
// 수 정 자 :
// 수정내용 :
// 비    고 : 
// 참    고 : 
/*************************************************************/
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DC.DAC001
{
    public partial class DAC001P1 : UIForm.FPCOMM1
    {
        #region 리턴될 변수선언
        private string[] getCUSTOMER_ORDER_ID = null;       //계약번호
        private string[] getCUSTOMER_ORDER_LINE = null;     //계약라인
        private string[] getPART_ID = null;                 //품목
        private string[] getPART_NAME = null;               //품명
        private string[] getPROJECT_ID = null;              //프로젝트번호
        private string[] getPROJECT_NAME = null;            //프로젝트명
        private string[] getREQ_YEAR = null;                //제출년도
        private string[] getREQ_NO = null;                  //판단번호
        private string[] getREQ_DEPT = null;                //구매부서
        private string[] getCUST_ORDER_ID = null;           //사집번호
        private decimal[] getQTY = null;           //계약수량
        private int getRETURN = 0;

        public int RETURN
        {
            get { return getRETURN; }
        }
        public string[] CUSTOMER_ORDER_ID
        {
            get { return getCUSTOMER_ORDER_ID; }
        }
        public string[] CUSTOMER_ORDER_LINE
        {
            get { return getCUSTOMER_ORDER_LINE; }
        }
        public string[] PART_ID
        {
            get { return getPART_ID; }
        }
        public string[] PART_NAME
        {
            get { return getPART_NAME; }
        }
        public string[] PROJECT_ID
        {
            get { return getPROJECT_ID; }
        }
        public string[] PROJECT_NAME
        {
            get { return getPROJECT_NAME; }
        }
        public string[] REQ_YEAR
        {
            get { return getREQ_YEAR; }
        }
        public string[] REQ_NO
        {
            get { return getREQ_NO; }
        }
        public string[] REQ_DEPT
        {
            get { return getREQ_DEPT; }
        }
        public string[] CUST_ORDER_ID
        {
            get { return getCUST_ORDER_ID; }
        }
        public decimal[] QTY
        {
            get { return getQTY; }
        }      

        #endregion

        #region DAC001P1()
        public DAC001P1()
        {
            InitializeComponent();
        } 
        #endregion

        #region DAC001P1_Load()
        private void DAC001P1_Load(object sender, EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            UIForm.Buttons.ReButton("010000001001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);
            this.Text = "계약리스트";

            //SearchExec();

        }
        #endregion
        
        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            try
            {
                this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
                string strSql = " usp_DAC001 ";
                strSql += "  @pTYPE ='S1' ";
                strSql += ", @pID ='" + txtID.Text + "' ";
                strSql += ", @pNAME = '" + txtNAME.Text + "' ";
                strSql += ", @pWID = '" + txtWID.Text + "' ";
                strSql += ", @pDESCRIPTION = '" + txtDESCRIPTION.Text + "' ";
                strSql += ", @pUSER_7 = '" + txtUSER_7.Text + "' ";
                strSql += ", @pORDER_DATE = '" + dtORDER_DATE.Text.ToString() + "' ";
                strSql += ", @pDESIRED_SHIP_DATE = '" + dtDESIRED_SHIP_DATE.Text.ToString() + "' ";
                strSql += ", @pUSER_1 = '" + txtUSER_1.Text + "' ";
                strSql += ", @pCUSTOMER_PO_REF = '" + txtCUSTOMER_PO_REF.Text + "' ";
                strSql += ", @pUSER_3 = '" + txtUSER_3.Text + "' ";


                UIForm.FPMake.grdCommSheet(fpSpread1, strSql, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 5, true);
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            this.Cursor = System.Windows.Forms.Cursors.Default;
        }
        #endregion

        #region 버튼클릭이벤트
        private void picSearch_Click(object sender, EventArgs e)
        {
            SearchExec();
        }
        private void picConfirm_Click(object sender, EventArgs e)
        {
            try
            {
                getCUSTOMER_ORDER_ID = new string[1]; getCUSTOMER_ORDER_LINE = new string[1];
                getPART_ID = new string[1]; getPART_NAME = new string[1];
                getPROJECT_ID = new string[1]; getPROJECT_NAME = new string[1];
                getREQ_YEAR = new string[1]; getREQ_NO = new string[1];
                getREQ_DEPT = new string[1]; getCUST_ORDER_ID = new string[1];
                getQTY = new decimal[1];
                for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                {
                    if (fpSpread1.Sheets[0].Cells[i, 1].Text == "True")
                    {
                        getCUSTOMER_ORDER_ID[0] = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "계약번호")].Text.ToString();
                        getCUSTOMER_ORDER_LINE[0] = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "계약라인")].Text.ToString();
                        getPART_ID[0] = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목")].Text.ToString();
                        getPART_NAME[0] = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품명")].Text.ToString();
                        getPROJECT_ID[0] = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트")].Text.ToString();
                        getPROJECT_NAME[0] = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트명")].Text.ToString();
                        getREQ_YEAR[0] = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "제출년도")].Text.ToString();
                        getREQ_NO[0] = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "판단번호")].Text.ToString();
                        getREQ_DEPT[0] = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "구매부서")].Text.ToString();
                        getCUST_ORDER_ID[0] = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "사집번호")].Text.ToString();
                        getQTY[0] = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "계약수량")].Text.ToString());
                        getRETURN = 1;
                    }
                }
                this.Close();
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void picClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void fpSpread1_CellDoubleClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            try
            {
                getCUSTOMER_ORDER_ID = new string[1]; getCUSTOMER_ORDER_LINE = new string[1];
                getPART_ID = new string[1]; getPART_NAME = new string[1];
                getPROJECT_ID = new string[1]; getPROJECT_NAME = new string[1];
                getREQ_YEAR = new string[1]; getREQ_NO = new string[1];
                getREQ_DEPT = new string[1]; getCUST_ORDER_ID = new string[1];
                getQTY = new decimal[1];

                getCUSTOMER_ORDER_ID[0] = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "계약번호")].Text.ToString();
                getCUSTOMER_ORDER_LINE[0] = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "계약라인")].Text.ToString();
                getPART_ID[0] = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목")].Text.ToString();
                getPART_NAME[0] = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품명")].Text.ToString();
                getPROJECT_ID[0] = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트")].Text.ToString();
                getPROJECT_NAME[0] = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트명")].Text.ToString();
                getREQ_YEAR[0] = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "제출년도")].Text.ToString();
                getREQ_NO[0] = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "판단번호")].Text.ToString();
                getREQ_DEPT[0] = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "구매부서")].Text.ToString();
                getCUST_ORDER_ID[0] = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "사집번호")].Text.ToString();
                getQTY[0] = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "계약수량")].Text.ToString());

                getRETURN = 1;
                this.Close();
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void fpSpread1_ButtonClicked(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            try
            {
                #region 선택
                if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "선택"))
                {
                    fpSpread1.Sheets[0].RowHeader.Cells[e.Row, 0].Text = "";
                    fpSpread1.Sheets[0].RowHeader.Rows[e.Row].BackColor = SystemBase.Base.Color_Org;

                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {
                        if (e.Row != i)
                        {
                            fpSpread1.Sheets[0].Cells[i, 1].Value = 0;
                        }
                    }
                }
                #endregion
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 이미지 전환
        private void picSearch_MouseEnter(object sender, EventArgs e)
        {
            try
            {
                Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\Toolbar\uSearch.gif");
                picSearch.BackgroundImage = bitMap;
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void picSearch_MouseLeave(object sender, EventArgs e)
        {
            try
            {
                Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\Toolbar\Search.gif");
                picSearch.BackgroundImage = bitMap;
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void picConfirm_MouseEnter(object sender, EventArgs e)
        {
            try
            {
                Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\Toolbar\uConfirm.gif");
                picConfirm.BackgroundImage = bitMap;
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void picConfirm_MouseLeave(object sender, EventArgs e)
        {
            try
            {
                Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\Toolbar\Confirm.gif");
                picConfirm.BackgroundImage = bitMap;
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion




    }
}
