#region 작성정보
/*********************************************************************/
// 단위업무명 : 부대비일괄배부
// 작 성 자 : 권 순 철
// 작 성 일 : 2013-04-04
// 작성내용 : 부대비일괄배부 및 관리
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

namespace AR.ARA001
{
    public partial class ARA001 : UIForm.Buttons
    {
        public ARA001()
        {
            InitializeComponent();
        }

        #region Form Load시
        private void ARA001_Load(object sender, System.EventArgs e)
        {
            //필수 체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            //콤보박스 세팅
            SystemBase.ComboMake.C1Combo(cboPlantCd, "usp_B_COMMON @pType='TABLE', @pCODE = 'PLANT_CD', @pNAME = 'PLANT_NM', @pSPEC1 = 'B_PLANT_INFO', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 3);//공장

            //초기화
            UIForm.Buttons.ReButton("100000000001", BtnNew, BtnPrint, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnHelp, BtnExcel, BtnClose);

            CloseMont();
            CloseStep();
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

        #region 제조오더 마감
        private void btnWorkClose_Click(object sender, EventArgs e)
        {
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1) == true)
            {
                string strWorkType = "W";

                ARA001P1 frm = new ARA001P1(strWorkType
                    , Convert.ToString(cboPlantCd.SelectedValue)
                    , dtpCloseMonth.Text.Replace("-", ""));

                frm.ShowDialog();

                if (frm.DialogResult == DialogResult.OK)
                {
                    CloseMont();
                    CloseStep();
                }
            }
        }
        #endregion

        #region 원가적상
        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1) == true)
            {
                string strWorkType = "A";

                ARA001P1 frm = new ARA001P1(strWorkType
                    , Convert.ToString(cboPlantCd.SelectedValue)
                    , dtpCloseMonth.Text.Replace("-", ""));

                frm.ShowDialog();

                if (frm.DialogResult == DialogResult.OK)
                {
                    CloseMont();
                    CloseStep();
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
            try
            {
                string Query = " usp_ARA001 @pTYPE = 'S1' ";
                Query += " , @pPLANT_CD = '" + cboPlantCd.SelectedValue.ToString() + "' ";
                Query += " , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "' ";

                DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);

                if (dt.Rows.Count > 0)
                { dtpCloseMonth.Value = dt.Rows[0][0].ToString();}
                else
                { dtpCloseMonth.Value = SystemBase.Base.ServerTime("YYMMDD");}
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }
        #endregion

        #region 마감단계
        private void CloseStep()
        {
            string strGbn = "S2";

            try
            {
                string Query = " usp_ARA001 @pTYPE = '" + strGbn + "'";
                Query += ", @pPLANT_CD = '" + cboPlantCd.SelectedValue.ToString() + "'";
                Query += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                Query += ", @pYYMM = '" + dtpCloseMonth.Text.Replace("-", "") + "'";

                DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);

                if (dt.Rows.Count > 0)
                {

                    if (dt.Rows[0][0].ToString() == "W")
                    {
                        btnWorkClose.Enabled = false;
                        btnAdd.Enabled = true;
                        txtCloseStatus.Value = "제조오더 마감이 완료 되었습니다.";
                    }
                    else if (dt.Rows[0][0].ToString() == "A")
                    {
                        btnWorkClose.Enabled = false;
                        btnAdd.Enabled = true;
                        txtCloseStatus.Value = "원가 적상이 완료 되었습니다.";
                    }
                    else if (dt.Rows[0][0].ToString() == "E")
                    {
                        btnWorkClose.Enabled = false;
                        btnAdd.Enabled = false;
                        txtCloseStatus.Value = "실제원가 마감이 완료 되었습니다.";
                    }
                }
                else
                {
                    txtCloseStatus.Value = "";
                    btnWorkClose.Enabled = true;
                    btnAdd.Enabled = false;
                    txtCloseStatus.Value = "실제원가 마감 전 입니다.";
                }
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
