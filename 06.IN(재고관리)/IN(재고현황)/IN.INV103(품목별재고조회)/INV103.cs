#region 작성정보
/*********************************************************************/
// 단위업무명 : 품목별재고조회
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-04-19
// 작성내용 : 품목별재고조회 관리
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
using System.Data.SqlClient;
using WNDW;

namespace IN.INV103
{
    public partial class INV103 : UIForm.FPCOMM2
    {
        #region 변수선언
        string strPlantCd;
        bool form_act_chk = false;
        #endregion

        #region 생성자
        public INV103()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void INV102_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            //그리드 초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
            UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, 0, 0, false);

            strPlantCd = "";

        }
        #endregion

        #region NewExec()
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            //그리드 초기화
            fpSpread1.Sheets[0].Rows.Count = 0;
            fpSpread2.Sheets[0].Rows.Count = 0;
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
                    string strQuery = " usp_INV103 'S1'";
                    strQuery += ", @pLANG_CD ='" + SystemBase.Base.gstrLangCd + "'";
                    strQuery += ", @pITEM_CD ='" + txtITEM_CD.Text.Trim() + "'";
                    strQuery += ", @pITEM_SPEC ='" + txtITEM_SPEC.Text.Trim() + "'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread2, strQuery, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, true, 0, 2, true);
                    fpSpread1.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.Normal;
                    if (fpSpread2.Sheets[0].Rows.Count > 0)
                    {
                        strPlantCd = fpSpread2.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx2, "공장")].Text;
                        //상세정보조회
                        SubSearch(strPlantCd, fpSpread2.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx2, "품목코드")].Text);
                    }
                    else
                    {
                        fpSpread1.Sheets[0].RowCount = 0;
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
        #endregion

        #region 조회조건 팝업
        // 품목
        private void btnITEM_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW.WNDW001 pu = new WNDW.WNDW001();
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtITEM_CD.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목정보 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 코드입력시 이름조회

        // 품목
        private void txtITEM_CD_TextChanged(object sender, System.EventArgs e)
        {
            string Query = " usp_M_COMMON @pTYPE = 'M013', @pCODE = '" + txtITEM_CD.Text + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
            DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);

            if (dt.Rows.Count > 0)
            {
                txtITEM_NM.Value = dt.Rows[0]["ITEM_NM"].ToString();
                txtITEM_SPEC.Value = dt.Rows[0]["ITEM_SPEC"].ToString();
                txtITEM_UNIT.Value = dt.Rows[0]["ITEM_UNIT"].ToString();
            }
            else
            {
                txtITEM_NM.Value = "";
                txtITEM_UNIT.Value = "";
            }
        }
        #endregion

        #region 상세정보 조회
        private void SubSearch(string strCode, string strItemCd)
        {
            string strQuery = " usp_INV103  'S2'";
            strQuery = strQuery + ", @pLANG_CD ='" + SystemBase.Base.gstrLangCd + "' ";
            strQuery = strQuery + ", @pITEM_CD ='" + strItemCd + "' ";
            strQuery = strQuery + ", @pPLANT_CD  ='" + strCode + "' ";
            strQuery = strQuery + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

            UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 2, true);
            fpSpread1.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.Normal;
        }
        #endregion

        #region fpSpread2_CellClick
        private void fpSpread2_CellClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            strPlantCd = fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "공장")].Text;
            //상세정보조회
            SubSearch(strPlantCd, fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "품목코드")].Text);
        }
        #endregion

        #region 폼 Activated & Deactivated
        private void INV103_Activated(object sender, System.EventArgs e)
        {
            if (form_act_chk == false) txtITEM_CD.Focus();
        }

        private void INV103_Deactivate(object sender, System.EventArgs e)
        {
            form_act_chk = true;
        }
        #endregion
    }
}
