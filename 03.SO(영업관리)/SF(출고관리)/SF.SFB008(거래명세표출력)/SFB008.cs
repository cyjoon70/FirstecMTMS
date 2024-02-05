#region 작성정보
/*********************************************************************/
// 단위업무명 : 거래명세표 출력
// 작 성 자 : 조  홍  태
// 작 성 일 : 2013-02-05
// 작성내용 : 거래명세표 출력
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

namespace SF.SFB008
{
    public partial class SFB008 : UIForm.Buttons
    {
        #region 변수선언
        bool form_act_chk = false;
        #endregion

        #region 생성자
        public SFB008()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void SFB008_Load(object sender, System.EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            dtpDeliveryDtFr.Value = null;
            dtpDeliveryDtTo.Value = null;

        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            dtpDeliveryDtFr.Value = null;
            dtpDeliveryDtTo.Value = null;
        }
        #endregion

        #region 미리보기
        private void btnPreview_Click(object sender, System.EventArgs e)
        {
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                string[] RptParmValue = new string[11];     // 2017.11.10. hma 수정: 9=>11
                string[] FormulaFieldName = new string[1]; //formula 값
                string[] FormulaFieldValue = new string[1]; //formula 이름
                string RptName = "";

                //--레포트 파일 선택
                RptName = SystemBase.Base.ProgramWhere + @"\Report\" + "SFB008.rpt";

                RptParmValue[0] = "R1";
                RptParmValue[1] = SystemBase.Base.gstrCOMCD;
                RptParmValue[2] = SystemBase.Base.gstrLangCd;
                RptParmValue[3] = txtDnNo.Text.Trim();
                RptParmValue[4] = txtSoNo.Text;
                RptParmValue[5] = txtProjectNo.Text.Trim();
                RptParmValue[6] = txtProjectSeq.Text.Trim();
                RptParmValue[7] = dtpDeliveryDtFr.Text;
                RptParmValue[8] = dtpDeliveryDtTo.Text;
                RptParmValue[9] = dtpRefDelvDtFr.Text;      // 2017.11.10. hma 추가: 납기일(참조)FROM
                RptParmValue[10] = dtpRefDelvDtTo.Text;     // 2017.11.10. hma 추가: 납기일(참조)TO

                string div = "";

                if (rdo1.Checked == true) div = rdo1.Text;
                else div = rdo2.Text;

                FormulaFieldValue[0] = "\"" + div + "\"";
                FormulaFieldName[0] = "DIV";

                UIForm.PRINT10 frm = new UIForm.PRINT10(this.Text, FormulaFieldValue, FormulaFieldName, RptName, RptParmValue);
                frm.ShowDialog();

            }
        }
        #endregion

        #region 팝업창 열기(품목)
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
                    txtProjectSeq.Text = "";
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "프로젝트번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //프로젝트차수
        private void btnProjectSeq_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON 'PROJ_SEQ', @pSPEC1 = '" + txtProjectNo.Text + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";										// 쿼리
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };			// 쿼리 인자값(조회조건)
                string[] strSearch = new string[] { "", "" };		// 쿼리 인자값에 들어갈 데이타

                //UIForm.PopUpSP pu = new UIForm.PopUpSP(strQuery, strWhere, strSearch, PHeadText7, PTxtAlign7, PCellType7, PHeadWidth7, PSearchLabel7);
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P09001", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "프로젝트차수 조회", false);
                pu.Width = 400;
                pu.ShowDialog();	//공통 팝업 호출

                if (pu.DialogResult == DialogResult.OK)
                {
                    string MSG = pu.ReturnVal.Replace("|", "#");
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(MSG);
                    txtProjectSeq.Text = Msgs[0].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }
        //출고번호
        private void btnDnNo_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW.WNDW013 pu = new WNDW.WNDW013();
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtDnNo.Value = Msgs[1].ToString();
                    txtDnNo.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "출고정보 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //수주번호
        private void btnSoNo_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW.WNDW012 pu = new WNDW.WNDW012();
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtSoNo.Value = Msgs[1].ToString();
                    txtSoNo.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "수주정보 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 폼 활성화/비활성화 시 변수 설정
        private void SFB008_Activated(object sender, System.EventArgs e)
        {
            if (form_act_chk == false) txtDnNo.Focus();
        }

        private void SFB008_Deactivate(object sender, System.EventArgs e)
        {
            form_act_chk = true;
        }
        #endregion
    }
}
