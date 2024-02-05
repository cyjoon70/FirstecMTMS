#region 작성정보
/*********************************************************************/
// 단위업무명 : 수입진행현황조회
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-04-17
// 작성내용 : 수입진행현황조회 관리
// 수 정 일 :
// 수 정 자 :
// 수정내용 :
// 비    고 :
/*********************************************************************/
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace MP.MPO509
{
    public partial class MPO509P4 : UIForm.FPCOMM1
    {
        #region 변수선언
        string strCcNo = "";
        #endregion

        #region 생성자
        public MPO509P4()
        {
            InitializeComponent();
        }
        public MPO509P4(string CcNo)
        {
            strCcNo = CcNo;
            //
            // Windows Form 디자이너 지원에 필요합니다.
            //
            InitializeComponent();

            //
            // TODO: InitializeComponent를 호출한 다음 생성자 코드를 추가합니다.
            //
        }
        #endregion
        
        #region 폼로드 이벤트
        private void MPO509P4_Load(object sender, EventArgs e)
        {
            //GroupBo x1 초기화
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            this.Text = "통관번호팝업";

            //버튼 재정의(조회권한만)
            UIForm.Buttons.ReButton("010000000001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);

            //콤보박스 세팅
            SystemBase.ComboMake.C1Combo(cboCurrency, "usp_B_COMMON @pType='COMM', @pCODE = 'Z003', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 9); //화폐단위

            //그리드 초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

            btnbeneficiaryCust.Enabled = false;
            txtCcNo.Value = strCcNo;

            Grid_Search(strCcNo);
        }
        #endregion

        #region 그리드조회
        private void Grid_Search(string CcNo)
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {
                string strQuery = "usp_MPO509 @pTYPE = 'P4'";
                strQuery += ", @pCC_NO = '" + CcNo + "'";
                strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0, true);

                //텍스트박스에 값넣기
                int iRow = fpSpread1.Sheets[0].Rows.Count;

                if (iRow > 0)
                {
                    txtbeneficiaryCust.Text = fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "수출자")].Text;
                    txtIdNo.Text = fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "신고번호")].Text;
                    dtpReportDt.Value = fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "신고일")].Text;
                    txtLicenseNo.Text = fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "면허번호")].Text;
                    dtpLicenseDt.Value = fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "면허일")].Text;
                    cboCurrency.SelectedValue = fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "화폐")].Text;

                    double dTotCcAmt = 0;

                    for (int i = 0; i < iRow; i++)
                    {
                        dTotCcAmt += Convert.ToDouble(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "금액")].Text);
                    }
                    txtTotCcAmt.Value = dTotCcAmt;
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회중 오류가 발생하였습니다.
            }

            this.Cursor = Cursors.Default;
        }
        #endregion
        
        #region 텍스트박스 TextChanged
        private void txtbeneficiaryCust_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtbeneficiaryCust.Text != "")
                {
                    txtbeneficiaryCustNm.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtbeneficiaryCust.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtbeneficiaryCustNm.Value = "";
                }
            }
            catch
            {

            }
        }
        #endregion

    }
}
