#region 작성정보
/*********************************************************************/
// 단위업무명 : 공통팝업 작업일보정보조회
// 작 성 자 : 조  홍  태
// 작 성 일 : 2013-02-18
// 작성내용 : 작업일보정보조회
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

#region 예제 - 복사해서 쓰세요
/*
try
{
    WNDW.WNDW025 pu = new WNDW.WNDW025();
    pu.ShowDialog();
    if (pu.DialogResult == DialogResult.OK)
    {
        string[] Msgs = pu.ReturnVal;

        textBox1.Text = Msgs[1].ToString();
        textBox2.Value = Msgs[2].ToString();
    }
}
catch (Exception f)
{
    SystemBase.Loggers.Log(this.Name, f.ToString());
    MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "작업일보정보조회 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
}
 */
#endregion

namespace WNDW
{
    /// <summary>
    /// 작업일보정보조회 조회
    /// <para>예제는 소스안에서 복사해쓰세요</para>
    /// <para>Msgs[1] = 작업일보번호 </para>
    /// </summary>

    public partial class WNDW025 : UIForm.FPCOMM1
    {
        #region 변수선언
        string[] returnVal = null;
        int SDown = 1;		// 조회 횟수
        int AddRow = 100;	// 조회 건수
        string strWorkDayNo = "", strWorkFlag = "1";
        #endregion

        #region 생성자
        public WNDW025(string WorkDayNo, string WorkFlag)
        {
            strWorkDayNo = WorkDayNo;
            strWorkFlag = WorkFlag;
            InitializeComponent();
        }

        public WNDW025(string WorkFlag)
        {
            strWorkFlag = WorkFlag;
            InitializeComponent();
        }

        public WNDW025()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void WNDW025_Load(object sender, System.EventArgs e)
        {
            //버튼 재정의
            UIForm.Buttons.ReButton("010000000001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);

            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);//필수적용

            //그리드 콤보박스
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "작업일보구분")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE = 'COMM', @pCODE = 'P059', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'", 1);

            //그리드 초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);

            //기타 세팅
            dtpWorkDtFr.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddDays(-1).ToString();
            dtpWorkDtTo.Value = SystemBase.Base.ServerTime("YYMMDD");
            txtSPlantCd.Text = SystemBase.Base.gstrPLANT_CD.ToString();

            Grid_Search(false);

            this.Text = "작업일보정보조회 조회";
        }
        #endregion

        #region 조회조건 팝업
        //공장
        private void btnSPlant_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pTYPE = 'P013', @pBIZ_CD = '" + SystemBase.Base.gstrBIZCD + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' "; // 쿼리
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };											  // 쿼리 인자값(조회조건)
                string[] strSearch = new string[] { txtSPlantCd.Text, "" };															  // 쿼리 인자값에 들어갈 데이타

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00005", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "공장 조회", false);

                pu.ShowDialog();	//공통 팝업 호출
                if (pu.DialogResult == DialogResult.OK)
                {
                    string MSG = pu.ReturnVal.Replace("|", "#");
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(MSG);

                    txtSPlantCd.Text = Msgs[0].ToString();
                    txtSPlantNm.Value = Msgs[1].ToString();
                    txtSPlantCd.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "공장 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //작업장
        private void btnWc_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pTYPE = 'P042', @pLANG_CD = 'KOR', @pETC = 'P002', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' ";					// 쿼리
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };					// 쿼리 인자값(조회조건)
                string[] strSearch = new string[] { txtWc_Cd.Text, "" };								// 쿼리 인자값에 들어갈 데이타

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00025", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "작업장 조회", false);
                pu.ShowDialog();	//공통 팝업 호출
                if (pu.DialogResult == DialogResult.OK)
                {
                    string MSG = pu.ReturnVal.Replace("|", "#");
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(MSG);

                    txtWc_Cd.Text = Msgs[0].ToString();
                    txtWc_Nm.Value = Msgs[1].ToString();
                    txtWc_Cd.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "부서 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //작업자
        private void btnWorkDuty_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (txtWc_Cd.Text == "")
                {
                    MessageBox.Show("소속 작업장이 선택되지 않았습니다. 작업장을 먼저 선택하십시오.", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                string strQuery = " usp_P_COMMON @pTYPE = 'P121', @pETC = '" + txtWc_Cd.Text + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' ";				// 쿼리
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };			// 쿼리 인자값(조회조건)
                string[] strSearch = new string[] { txtWorkDutyId.Text, "" };			// 쿼리 인자값에 들어갈 데이타

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00071", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "작업자 조회", false);
                pu.ShowDialog();	//공통 팝업 호출
                if (pu.DialogResult == DialogResult.OK)
                {
                    string MSG = pu.ReturnVal.Replace("|", "#");
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(MSG);

                    txtWorkDutyId.Text = Msgs[0].ToString();
                    txtWorkDutyNm.Value = Msgs[1].ToString();
                    txtWorkDutyId.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "작업자 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 조회조건 TextChanged
        //공장
        private void txtSPlantCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtSPlantCd.Text != "")
                {
                    txtSPlantNm.Value = SystemBase.Base.CodeName("PLANT_CD", "PLANT_NM", "B_PLANT_INFO", txtSPlantCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'");
                }
                else
                {
                    txtSPlantNm.Value = "";
                }
            }
            catch { }
        }

        //작업장
        private void txtWc_Cd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtWc_Cd.Text != "")
                {
                    txtWc_Nm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtWc_Cd.Text, " AND MAJOR_CD = 'P002' AND COMP_CODE = '" + SystemBase.Base.gstrCOMCD.ToString() + "' ");
                }
                else
                {
                    txtWc_Nm.Value = "";
                }
            }
            catch { }
        }

        //작업자
        private void txtWorkDutyId_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtWorkDutyId.Text != "")
                {
                    txtWorkDutyNm.Value = SystemBase.Base.CodeName("RES_CD", "RES_DIS", "P_RESO_MANAGE", txtWorkDutyId.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'");
                }
                else
                {
                    txtWorkDutyNm.Value = "";
                }
            }
            catch { }
        }
        #endregion

        #region SearchExec()
        protected override void SearchExec()
        { Grid_Search(true); }
        #endregion

        #region 그리드 조회
        private void Grid_Search(bool Msg)
        {
            this.Cursor = Cursors.WaitCursor;

            //조회조건 필수 체크
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                try
                {
                    SDown = 1;

                    string strType = "S1";

                    if (strWorkFlag == "2")
                    {
                        strType = "S2";
                    }
                    else if (strWorkFlag == "3")
                    {
                        strType = "S3";
                    }

                    string strQuery = " usp_WNDW025 @pTYPE = '" + strType + "' ";
                    strQuery += ", @pPLANT_CD = '" + txtSPlantCd.Text + "' ";
                    strQuery += ", @pWC_CD = '" + txtWc_Cd.Text + "' ";
                    strQuery += ", @pWORK_DUTY = '" + txtWorkDutyId.Text + "' ";
                    strQuery += ", @pWORK_DT_FR = '" + dtpWorkDtFr.Text + "' ";
                    strQuery += ", @pWORK_DT_TO = '" + dtpWorkDtTo.Text + "' ";
                    strQuery += ", @pTOPCOUNT ='" + AddRow + "'";
                    strQuery += ", @pCO_CD ='" + SystemBase.Base.gstrCOMCD.ToString() + "'";
                    strQuery += ", @pWORK_DAY_NO ='" + strWorkDayNo + "'";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, Msg, 0, 0, true);
                    fpSpread1.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.SingleSelect;
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회중 오류가 발생하였습니다.
                }
            }

            this.Cursor = Cursors.Default;
        }
        #endregion

        #region 100건씩 조회
        private void fpSpread1_TopChange(object sender, FarPoint.Win.Spread.TopChangeEventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;

            int FPHeight = (fpSpread1.Size.Height - 28) / 20;
            if (e.NewTop >= ((AddRow * SDown) - FPHeight))
            {
                int cnt_prev = AddRow * SDown;
                SDown++;
                int cnt = AddRow * SDown;

                string strType = "S1";

                if (strWorkFlag == "2")
                {
                    strType = "S2";
                }
                else if (strWorkFlag == "3")
                {
                    strType = "S3";
                }

                string strQuery = " usp_WNDW025 @pTYPE = '" + strType + "' ";
                strQuery += ", @pPLANT_CD = '" + txtSPlantCd.Text + "' ";
                strQuery += ", @pWC_CD = '" + txtWc_Cd.Text + "' ";
                strQuery += ", @pWORK_DUTY = '" + txtWorkDutyId.Text + "' ";
                strQuery += ", @pWORK_DT_FR = '" + dtpWorkDtFr.Text + "' ";
                strQuery += ", @pWORK_DT_TO = '" + dtpWorkDtTo.Text + "' ";
                strQuery += ", @pTOPCOUNT ='" + AddRow + "'";
                strQuery += ", @pCO_CD ='" + SystemBase.Base.gstrCOMCD.ToString() + "'";
                strQuery += ", @pWORK_DAY_NO ='" + strWorkDayNo + "'";

                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery);
                fpSpread1.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.SingleSelect;
            }

            this.Cursor = Cursors.Default;
        }
        #endregion

        #region 그리드 더블클릭
        private void fpSpread1_CellDoubleClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            RtnStr(e.Row);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        #endregion

        #region 그리드 선택값 입력밑 전송
        public string[] ReturnVal { get { return returnVal; } set { returnVal = value; } }

        public void RtnStr(int R)
        {
            if (fpSpread1.Sheets[0].Rows.Count > 0)
            {
                returnVal = new string[fpSpread1.Sheets[0].Columns.Count];
                for (int i = 0; i < fpSpread1.Sheets[0].Columns.Count; i++)
                {
                    returnVal[i] = fpSpread1.Sheets[0].Cells[R, i].Value.ToString();
                }
            }
        }
        #endregion	
    }
}
