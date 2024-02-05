#region 작성정보
/*********************************************************************/
// 단위업무명 : 원가마감작업
// 작 성 자 : 권 순 철
// 작 성 일 : 2013-04-04
// 작성내용 : 원가마감작업 및 관리
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

namespace CZ.CZA090
{
    public partial class CZA090 : UIForm.Buttons
    {
        public CZA090()
        {
            InitializeComponent();
        }

        #region Form Load시
        private void CZA090_Load(object sender, System.EventArgs e)
        {
            //필수 체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            //초기화
            UIForm.Buttons.ReButton("100000000001", BtnNew, BtnPrint, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnHelp, BtnExcel, BtnClose);

            CloseMont();
        }
        #endregion

        #region NewExec()
        protected override void NewExec()
        {
            //필수체크
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            CloseMont();
        }
        #endregion

        #region 작업실행 버튼클릭
        private void btnExec_Click(object sender, EventArgs e)
        {

            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1) == true)
            {
                string strWorkType = "";

                if (rdoCloseDivY.Checked == true) { strWorkType = "R"; }
                else { strWorkType = "C"; }

                CZA090P1 frm = new CZA090P1(strWorkType
                    , dtpCloseMonth.Text.Replace("-", ""));

                frm.ShowDialog();

                if (frm.DialogResult == DialogResult.OK)
                {
                    rdoCloseDivY.Checked = true;
                    CloseMont();
                }

            }
        }
        #endregion

        #region 공장선택 이벤트
        private void cboPlantCd_SelectedIndexChanged(object sender, EventArgs e)
        {
            CloseMont();
        }
        #endregion

        #region 작업구분 체크 이벤트
        private void rdoCloseDivY_CheckedChanged(object sender, EventArgs e)
        {
            CloseMont();
        }

        private void rdoCloseDivN_CheckedChanged(object sender, EventArgs e)
        {
            CloseMont();
        }
        #endregion

        #region 작업년월
        private void CloseMont()
        {
            string strGbn = "";

            try
            {
                if (rdoCloseDivY.Checked == true) { strGbn = "S1"; }
                else { strGbn = "S2"; }

                string Query = " usp_CZA090 @pTYPE = '" + strGbn + "'";
                Query += ", @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                //Query += ", @pPLANT_CD = '" + cboPlantCd.SelectedValue.ToString() + "'";
                DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);

                if (dt.Rows.Count > 0)
                { dtpCloseMonth.Value = dt.Rows[0][0].ToString(); btnExec.Enabled = true; }
                else if (strGbn == "S2")
                { dtpCloseMonth.Value = null; btnExec.Enabled = false; }
                else
                { dtpCloseMonth.Value = SystemBase.Base.ServerTime("YYMMDD"); btnExec.Enabled = true; }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }
        #endregion

    }
}
