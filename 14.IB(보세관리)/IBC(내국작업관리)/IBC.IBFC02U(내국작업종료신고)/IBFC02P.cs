#region 작성정보
/*********************************************************************/
// 단위업무명 : 내국작업신청서
// 작 성 자 : 이태규
// 작 성 일 : 2013-06-12
// 작성내용 : 내국작업신청서 및 관리
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

namespace IBC.IBFC02U
{ 
    public partial class IBFC02P : UIForm.FPCOMM1
    {
        #region 변수선언
        private bool chk = false;
        private FarPoint.Win.Spread.FpSpread spd;
        private System.Windows.Forms.TextBox txtDegree;
        #endregion

        #region 생성자
        public IBFC02P()
        {
            InitializeComponent();
        }

        public IBFC02P(FarPoint.Win.Spread.FpSpread spread, string txtno, System.Windows.Forms.TextBox txtDeg)
        {
            InitializeComponent();
            spd = spread;
            txtDegree = txtDeg;
            txtNO.Value = txtno;
        }
        #endregion

        #region Form Load 시
        private void IBFC02P_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            UIForm.Buttons.ReButton("010000000001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);
            this.Text = "내국작업신청 참조팝업";
            SearchExec();
        }
        #endregion
        
        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            //그리드 초기화
            fpSpread1.Sheets[0].Rows.Count = 0;
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = Cursors.WaitCursor;
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                try
                {
                    // 조회 Exceptions 체크
                    string strQuery;

                    strQuery = " usp_IBFC01U  'P2', ";
                    strQuery = strQuery + " @pREQUEST_NO = '" + txtNO.Text + "'";
                    strQuery = strQuery + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 4, false, false);
                    fpSpread1.Sheets[0].SetColumnAllowAutoSort(5, 2, true);

                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(f.ToString());
                }
            }
            this.Cursor = Cursors.Default;
        }
        #endregion
         
        #region 버튼 클릭 이벤트
        private void butNO_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " Nusp_BF_Comm 'BF26' ";
                string[] strWhere = new string[] { "@pValue" };
                string[] strSearch = new string[] { txtNO.Text };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("BFP015", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "내국작업신청번호팝업");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtNO.Value = Msgs[0].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(f.ToString());
            }
        }

        private void btnAllSelect_Click(object sender, System.EventArgs e)
        {
            for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
            {
                fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "선택")].Value = 1;
            }
        }

        private void btnAllCancel_Click(object sender, System.EventArgs e)
        {
            for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
            {
                fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "선택")].Value = 0;
            }
        }

        private void btnOk_Click(object sender, System.EventArgs e)
        {
            for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
            {
                fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text = "";
            }

            string strSql = "Select ISNULL(MAX(FINISH_DEGREE),0) From BF_INTERNAL_WORK(Nolock) Where REQUEST_NO = '" + txtNO.Text.Trim() + "' ";
            DataSet ds = SystemBase.DbOpen.NoTranDataSet(strSql);

            int idegree = 0;
            if (ds.Tables[0].Rows.Count > 0)
            {
                idegree = Convert.ToInt16(ds.Tables[0].Rows[0][0].ToString()) + 1;
            }

            try
            {
                if (fpSpread1.Sheets[0].Rows.Count > 0)
                {
                    int j = spd.Sheets[0].Rows.Count;
                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {
                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "선택")].Text == "True")
                        {
                            spd.Sheets[0].Rows.Count = j + 1;
                            spd.Sheets[0].RowHeader.Cells[j, 0].Text = "I";
                            spd.Sheets[0].Cells[j, 1].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "관리번호")].Text;
                            spd.Sheets[0].Cells[j, 2].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "내국작업신청번호")].Text;
                            spd.Sheets[0].Cells[j, 3].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "계약번호(T/R)")].Text;
                            spd.Sheets[0].Cells[j, 4].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "시작일")].Text;
                            spd.Sheets[0].Cells[j, 5].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "종료일")].Text;
                            spd.Sheets[0].Cells[j, 6].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "내국작업허가번호")].Text;
                            spd.Sheets[0].Cells[j, 7].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "작업의 종류")].Text;
                            spd.Sheets[0].Cells[j, 8].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "반출예정일")].Text;
                            spd.Sheets[0].Cells[j, 9].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "반출사유")].Text;
                            spd.Sheets[0].Cells[j, 10].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "종료허가번호")].Text;
                            spd.Sheets[0].Cells[j, 11].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "종료여부")].Text;
                            spd.Sheets[0].Cells[j, 12].Value = idegree;
                            j++;

                            txtDegree.Text = Convert.ToString(idegree);

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            Close();
        }
        #endregion

        #region Form Activated & Deactivate
        private void IBFC02P_Activated(object sender, System.EventArgs e)
        {
            if (chk == false)
            {
                txtNO.Focus();
            }
        }

        private void IBFC02P_Deactivate(object sender, System.EventArgs e)
        {
            chk = true;
        }
        #endregion
    }
}
