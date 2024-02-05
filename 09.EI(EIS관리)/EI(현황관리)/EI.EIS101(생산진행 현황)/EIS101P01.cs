using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace EI.EIS101
{
    public partial class EIS101P01 : UIForm.FPCOMM1
    {
        #region 변수선언
        string strProjNo = "";
        string strProjSeq = "";
        string strItemCd = "";
        string strGi_Dt = "";
        #endregion

        public EIS101P01(string P_No, string P_Seq, string P_item, string Gi_Dt)
        {
            //
            // Windows Form 디자이너 지원에 필요합니다.
            //
            strProjNo = P_No;
            strProjSeq = P_Seq;
            strItemCd = P_item;
            strGi_Dt = Gi_Dt;
            InitializeComponent();

            //
            // TODO: InitializeComponent를 호출한 다음 생성자 코드를 추가합니다.
            //
        }

        public EIS101P01()
        {
            InitializeComponent();
        }

        #region Form Load 시
        private void EIS101P01_Load(object sender, System.EventArgs e)
        {
            this.Text = "개발일정";

            SystemBase.Validation.GroupBox_Setting(groupBox1);
            UIForm.Buttons.ReButton("000000001001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);
            txtProject_No.Value = strProjNo;
            txtProject_Seq.Value = strProjSeq;
            txtItem_Cd.Value = strItemCd;
            Search(false);
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            Search(true);
        }

        private void Search(bool msg)
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            try
            {
                string strQuery = " usp_EIS101  @pTYPE = 'S01'";
                strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' ";
                strQuery += ", @pPROJECT_NO = '" + txtProject_No.Text + "' ";
                strQuery += ", @pPROJECT_SEQ = '" + txtProject_Seq.Text + "' ";
                strQuery += ", @pITEM_CD= '" + strItemCd + "' ";
                strQuery += ", @pGI_DT  = '" + strGi_Dt + "' ";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, msg, 0, 0, true);
                fpSpread1.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.Normal;

                if (fpSpread1.Sheets[0].RowCount > 0)
                {
                    int col_idx1 = SystemBase.Base.GridHeadIndex(GHIdx1, "지연일수");
                    int col_idx2 = SystemBase.Base.GridHeadIndex(GHIdx1, "부문");
                    string Dtype = fpSpread1.Sheets[0].Cells[0, col_idx2].Text;
                    int SpanCnt = 1;

                    if (fpSpread1.Sheets[0].Rows.Count > 0 && Convert.ToInt16(fpSpread1.Sheets[0].Cells[0, col_idx1].Value.ToString()) > 0)
                        fpSpread1.Sheets[0].Cells[0, col_idx1].ForeColor = Color.Red;

                    //셀 합치기
                    for (int i = 1; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {
                        if (Convert.ToInt16(fpSpread1.Sheets[0].Cells[i, col_idx1].Value.ToString()) > 0)
                            fpSpread1.Sheets[0].Cells[i, col_idx1].ForeColor = Color.Red;
                        if (fpSpread1.Sheets[0].Cells[i, col_idx2].Text == Dtype)
                        {
                            SpanCnt++;

                            fpSpread1.Sheets[0].Cells[i + 1 - SpanCnt, col_idx2].RowSpan = SpanCnt;
                        }
                        else
                        {
                            SpanCnt = 1;
                        }

                        Dtype = fpSpread1.Sheets[0].Cells[i, col_idx2].Text;
                    }
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }

            this.Cursor = System.Windows.Forms.Cursors.Default;
        }
        #endregion
    }
}
