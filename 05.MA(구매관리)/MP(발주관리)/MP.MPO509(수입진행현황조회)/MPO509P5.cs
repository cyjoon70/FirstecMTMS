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
using System.Text.RegularExpressions;
using WNDW;

namespace MP.MPO509
{
    public partial class MPO509P5 : UIForm.FPCOMM1
    {
        #region 변수선언
        string strMvmtNo = "";
        #endregion

        #region 생성자
        public MPO509P5(string MvmtNo)
        {
            strMvmtNo = MvmtNo;
            //
            // Windows Form 디자이너 지원에 필요합니다.
            //
            InitializeComponent();

            //
            // TODO: InitializeComponent를 호출한 다음 생성자 코드를 추가합니다.
            //
        }
        
        public MPO509P5()
        {
            InitializeComponent();
        }
        #endregion

        #region 폼로드 이벤트
        private void MPO509P5_Load(object sender, System.EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            this.Text = "입고번호팝업";
                        
            //버튼 재정의(조회권한만)
            UIForm.Buttons.ReButton("010000000001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

            btnPurDuty.Enabled = false;
            btnCustCd.Enabled = false;
            btnIoType.Enabled = false;
            txtMvmtNo.Value = strMvmtNo;

            Grid_Search(strMvmtNo);
        }
        #endregion
        
        #region 그리드조회
        private void Grid_Search(string MvmtNo)
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {
                string strQuery = "usp_MPO509 @pTYPE = 'P5'";
                strQuery += ", @pMVMT_NO = '" + MvmtNo + "'";
                strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0, true);

                //텍스트박스에 값넣기
                int iRow = fpSpread1.Sheets[0].Rows.Count;

                if (iRow > 0)
                {
                    txtIoType.Value = fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "입고형태")].Text;
                    dtpMvmtDt.Value = fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "입고일")].Text;
                    txtCustCd.Value = fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "공급처")].Text;
                    txtPurDuty.Value = fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "구매담당자")].Text;
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

        #region TextChanged
        //구매담당자
        private void txtPurDuty_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtPurDuty.Text != "")
                {
                    txtPurDutyNm.Value = SystemBase.Base.CodeName("USR_ID", "USR_NM", "B_SYS_USER", txtPurDuty.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtPurDutyNm.Value = "";
                }
            }
            catch
            {

            }
        }

        //입고형태
        private void txtIoType_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtIoType.Text != "")
                {
                    txtIoTypeNm.Value = SystemBase.Base.CodeName("IO_TYPE", "IO_TYPE_NM", "M_MVMT_TYPE", txtIoType.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtIoTypeNm.Value = "";
                }
            }
            catch
            {

            }
        }

        //공급처
        private void txtCustCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtCustCd.Text != "")
                {
                    txtCustNm.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtCustCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtCustNm.Value = "";
                }
            }
            catch
            {

            }
        }
        #endregion

    }
}
