#region 작성정보
/*********************************************************************/
// 단위업무명 : 품질관리/추적관리/사용재고 현황조회
// 작 성 자   : 김 창 진
// 작 성 일   : 2014-08-22
// 작성내용   : 사용재고 현황조회
// 수 정 일   :
// 수 정 자   :
// 수정내용   :
// 비    고   :
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
namespace QT.QTC008
{
    public partial class QTC008 : UIForm.FPCOMM1
    {
        #region 변수선언
        string strAutoSoNo = "";
        #endregion

        #region 생성자
        public QTC008()
        {
            InitializeComponent();

        }
        public QTC008(string So_No)
        {
            // 알리미 클릭시- 알리미
            strAutoSoNo = So_No;
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void QTC008_Load(object sender, System.EventArgs e)
        {
            //GroupBox1 초기화
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            // 콤보박스 세팅
            SystemBase.ComboMake.C1Combo(cboPlantCd, "usp_B_COMMON @pTYPE = 'PLANT', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'"); //공장

            //그리드 세팅
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0, true);
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            //그리드 초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0, true);
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            //조회조건 필수 체크
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1) == true)
            {
                try
                {

					string strQuery = "usp_QTC008 @pTYPE = 'S1'";
                    strQuery += ", @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                    strQuery += ", @pPLANT_CD = '" + cboPlantCd.SelectedValue.ToString() + "'";
					strQuery += ", @pPROJECT_NO = '" + txtProjectNo.Text + "'";
                    strQuery += ", @pPROJECT_SEQ = '" + txtProjectSeq.Text + "'";
					strQuery += ", @pITEM_CD = '" + txtItemCd.Text + "'";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0, true);

                    fpSpread1.Sheets[0].SetColumnMerge(SystemBase.Base.GridHeadIndex(GHIdx1, "수주품목"), FarPoint.Win.Spread.Model.MergePolicy.Always);
                    fpSpread1.Sheets[0].SetColumnMerge(SystemBase.Base.GridHeadIndex(GHIdx1, "수주품목명"), FarPoint.Win.Spread.Model.MergePolicy.Always);

                    for (int i = 0; i < fpSpread1.ActiveSheet.Rows.Count; i++)
                    {
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수주품목")].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수주품목명")].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회중 오류가 발생하였습니다.
                }
            }
            this.Cursor = System.Windows.Forms.Cursors.Default;
        }
        #endregion

        #region 조회조건 팝업
        //프로젝트번호 
        private void btnProjectNo_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW.WNDW007 pu = new WNDW.WNDW007(txtProjectNo.Text);
                pu.MaximizeBox = false;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtProjectNo.Text = Msgs[3].ToString();
                    txtProjectNm.Value = Msgs[4].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "프로젝트번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //품목
        private void btnItemCd_Click(object sender, EventArgs e)
        {
            try
            {
                WNDW005 pu = new WNDW005("10");
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
            }
        }
        #endregion

        #region 조회조건 TextChanged
        //품목 
        private void txtItemCd_TextChanged(object sender, EventArgs e)
        {
            try
            {
                txtItemNm.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtItemCd.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목명 가져오기"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }	
        }
        //프로젝트 번호
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
            }
            catch
            {

            }
        }
        #endregion

    }
}
