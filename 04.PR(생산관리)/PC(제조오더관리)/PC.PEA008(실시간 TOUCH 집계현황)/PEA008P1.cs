#region 작성정보
/*********************************************************************/
// 단위업무명:  실시간TOUCH집계현황
// 작 성 자  :  한 미 애
// 작 성 일  :  2020-12-24
// 작성내용  :  작업대기 상태 작업자들에 대한 작업배정 및 TOUCH실적 조회
// 수 정 일  :
// 수 정 자  :
// 수정내용  :
// 비    고  :
/*********************************************************************/
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using WNDW;


namespace PC.PEA008
{
    public partial class PEA008P1 : UIForm.FPCOMM1
    {
        #region 변수선언
        #endregion

        #region 생성자
        public PEA008P1(string ProjectNo, string ProjectSeq, string ItemCd, string WoNo)
        {
            InitializeComponent();
        }

        public PEA008P1()
        {
            InitializeComponent();
        }
        #endregion

        #region 폼로드 이벤트
        private void PEA008P1_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            this.Text = "작업대기 상태 작업자 작업배정 및 TOUCH실적 조회";

            //버튼 재정의(조회권한만)
            UIForm.Buttons.ReButton("010000000001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);

            txtPlantCd.Text = SystemBase.Base.gstrPLANT_CD;
            dtpWorkDt.Value = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0, 10);
            chkWaitWorkerOnly.Checked = false;

            SearchExec();
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {
                string strViewType = "";
                if (chkWaitWorkerOnly.Checked == false)
                    strViewType = "S4";
                else
                    strViewType = "S5";

                string Query = " usp_PEA008 @pTYPE = '" + strViewType + "'";
                Query += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                Query += ", @pPLANT_CD = '" + txtPlantCd.Text + "'";
                Query += ", @pWORK_DT = '" + dtpWorkDt.Value.ToString() + "'";      //txtCurrentDate.Text
                Query += ", @pWC_CD = '" + txtWcCd.Text + "'";
                Query += ", @pH_RES_CD = '" + txtWorkDuty.Text + "'";
                Query += ", @pPROJECT_NO = '" + txtProjectNo.Text + "'";

                UIForm.FPMake.grdCommSheet(fpSpread1, Query, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0, true);
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

        #region 검색조건 버튼 클릭시 이벤트 처리: 해당 항목에 대한 팝업창을 띄운다.
        #region btnPlant_Click(): 공장 버튼 클릭시 공장 팝업창 띄워줌
        private void btnPlant_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pTYPE = 'P013', @pBIZ_CD = '" + SystemBase.Base.gstrBIZCD + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'"; // 쿼리
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };        // 쿼리 인자값(조회조건)
                string[] strSearch = new string[] { txtPlantCd.Text, "" };          // 쿼리 인자값에 들어갈 데이타

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00005", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "공장 조회", false);

                pu.ShowDialog();	// 공통 팝업 호출
                if (pu.DialogResult == DialogResult.OK)
                {
                    string MSG = pu.ReturnVal.Replace("|", "#");
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(MSG);

                    txtPlantCd.Text = Msgs[0].ToString();
                    txtPlantNm.Value = Msgs[1].ToString();
                    txtPlantCd.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "공장 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region btnWcCd_Click(): 작업장 버튼 클릭시 작업장 팝업창 띄워줌
        private void btnWcCd_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pTYPE = 'P042', @pLANG_CD = 'KOR', @pETC = 'P061' , @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'"; // 쿼리
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };    // 쿼리 인자값(조회조건)
                string[] strSearch = new string[] { txtWcCd.Text, "" };         // 쿼리 인자값에 들어갈 데이타

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00025", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "작업장 조회", false);
                pu.ShowDialog();    //공통 팝업 호출

                if (pu.DialogResult == DialogResult.OK)
                {
                    string MSG = pu.ReturnVal.Replace("|", "#");
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(MSG);

                    txtWcCd.Value = Msgs[0].ToString();
                    txtWcNm.Value = Msgs[1].ToString();
                    txtWcCd.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "부서 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region btnWorkDuty_Click(): 작업자(자원) 버튼 클릭시 작업자 팝업창 띄워줌
        private void btnWorkDuty_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pTYPE = 'P054', @pLANG_CD = 'KOR', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";    // 쿼리
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };        // 쿼리 인자값(조회조건)
                string[] strSearch = new string[] { txtWorkDuty.Text, "" };         // 쿼리 인자값에 들어갈 데이타

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00071", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "작업자 조회", false);
                pu.Width = 600;
                pu.ShowDialog();    // 공통 팝업 호출

                if (pu.DialogResult == DialogResult.OK)
                {
                    string MSG = pu.ReturnVal.Replace("|", "#");
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(MSG);

                    txtWorkDuty.Value = Msgs[0].ToString();
                    txtWorkDutyNm.Value = Msgs[1].ToString();
                    txtWorkDuty.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "작업자 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region btnProjectNo_Click(): 프로젝트 버튼 클릭시 프로젝트 팝업창 띄워줌
        private void btnProjectNo_Click(object sender, EventArgs e)
        {
            try
            {
                WNDW003 pu = new WNDW003(btnProjectNo.Text, "S1", "C");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtProjectNo.Value = Msgs[3].ToString();
                    txtProjectNm.Value = Msgs[4].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion
        #endregion

        #region 검색조건 항목 입력시 이벤트 처리: 입력된 코드에 해당하는 명세 보여줌.
        #region txtPlantCd_TextChanged(): 공장 항목 입력시 해당하는 공장을 보여줌.
        private void txtPlantCd_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (txtPlantCd.Text != "")
                {
                    txtPlantNm.Value = SystemBase.Base.CodeName("PLANT_CD", "PLANT_NM", "B_PLANT_INFO", txtPlantCd.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "'");
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

        #region txtWcCd_TextChanged(): 작업장 항목 입력시 해당하는 작업장명을 보여줌.
        private void txtWcCd_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (txtWcCd.Text != "")
                {
                    txtWcNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtWcCd.Text, " AND MAJOR_CD = 'P061'  AND COMP_CODE='" + SystemBase.Base.gstrCOMCD + "' ");
                }
                else
                {
                    txtWcNm.Value = "";
                }
            }
            catch
            {

            }
        }
        #endregion

        #region txtWorkDuty_TextChanged(): 작업자 항목 입력시 해당하는 작업자명을 보여줌.
        private void txtWorkDuty_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (txtWorkDuty.Text != "")
                {
                    txtWorkDutyNm.Value = SystemBase.Base.CodeName("RES_CD", "RES_DIS", "P_RESO_MANAGE", txtWorkDuty.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "' ");
                }
                else
                {
                    txtWorkDutyNm.Value = "";
                }
            }
            catch
            {

            }
        }
        #endregion

        #region txtProjectNo_TextChanged(): 프로젝트번호 항목 입력시 해당하는 프로젝트명을 보여줌.
        private void txtProjectNo_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (txtProjectNo.Text != "")
                {
                    txtProjectNm.Value = SystemBase.Base.CodeName("PROJECT_NO", "PROJECT_NM", "S_SO_MASTER", txtProjectNo.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");
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

        #endregion

    }
}
