#region 작성정보
/*********************************************************************/
// 단위업무명 : 공정별 미완료현황
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-04-16
// 작성내용 : 공정별 미완료현황 관리
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

namespace PC.PSB018
{
    public partial class PSB018 : UIForm.FPCOMM1
    {
        #region 변수선언
        string strDiv = "";
        #endregion

        #region 생성자
        public PSB018()
        {
            InitializeComponent();
        }
        public PSB018(string div)
        {
            strDiv = div;
            InitializeComponent();
        }
        #endregion

        #region Form Load시
        private void PSB018_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            txtPlantCd.Value = SystemBase.Base.gstrPLANT_CD;

            if (strDiv != "")
            {

                dtpDt.Value = strDiv;
                SearchExec();
            }
            else
                dtpDt.Value = SystemBase.Base.ServerTime("YYMMDD").Substring(0,10);
        }
        #endregion
        
        #region NewExec()
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            fpSpread1.Sheets[0].Rows.Count = 0;

            //기타세팅
            txtPlantCd.Value = SystemBase.Base.gstrPLANT_CD;
            dtpDt.Value = SystemBase.Base.ServerTime("YYMMDD").Substring(0, 10);
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {

            this.Cursor = Cursors.WaitCursor;

            try
            {
                string strQuery = " usp_PSB018  @pTYPE = 'S1'";
                strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' ";
                strQuery += ", @pDATE = '" + strDiv + "' ";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0, true);
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }

            this.Cursor = Cursors.Default;

        }
        #endregion
        
        #region txtPlantCd_TextChanged
        private void txtPlantCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtPlantCd.Text != "")
                {
                    txtPlantNm.Value = SystemBase.Base.CodeName("PLANT_CD", "PLANT_NM", "B_PLANT_INFO", txtPlantCd.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");
                }
                else
                {
                    txtPlantNm.Value = "";
                }
            }
            catch
            {

            }
        }
        #endregion
        
    }
}
