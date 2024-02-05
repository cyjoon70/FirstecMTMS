#region 작성정보
/*********************************************************************/
// 단위업무명 : 개인별월근태현황
// 작 성 자 : 권순철
// 작 성 일 : 2013-04-04
// 작성내용 : 개인별월근태현황 및 관리
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

namespace HA.HAA009
{
    public partial class HAA009 : UIForm.FPCOMM2
    {
        #region 변수선언
        //string strSchNo = "";     // 2022.07.13. hma 수정: 사용하는 곳이 없으므로 주석 처리
        //string strBtn = "N";
        string strEmpNo = "";       // 2022.07.13. hma 추가
        string strEmpNm = "";       // 2022.07.13. hma 추가
        #endregion

        #region 생성자
        public HAA009()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void HAA009_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            //그리드 초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
            UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, 0, 0, false);

            //기타세팅
            dtpDate.Text = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0, 7);

            Check_RollGroup();      // 2022.07.13. hma 추가: 근태관리 담당자가 아니면 본인 데이터만 조회하도록 사용자 항목 비활성화 처리
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            fpSpread1.Sheets[0].Rows.Count = 0;
            fpSpread2.Sheets[0].Rows.Count = 0;

            dtpDate.Text = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0, 7);

            Check_RollGroup();      // 2022.07.13. hma 추가: 근태관리 담당자가 아니면 본인 데이터만 조회하도록 사용자 항목 비활성화 처리
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            //조회조건 필수 체크
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1) == true)
            {
                this.Cursor = Cursors.WaitCursor;

                try
                {
                    // 2022.07.13. hma 추가(Start): 조회시 근태관리자여부 체크
                    string strQueryChk = " usp_HAA009 @pTYPE = 'C1'";
                    strQueryChk += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    strQueryChk += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "' ";
                    strQueryChk += ", @pEMP_NO = '" + txtEmpNo.Text + "' ";

                    DataSet ds = SystemBase.DbOpen.NoTranDataSet(strQueryChk);

                    string strDiligYn = "", strUsrId = "";
                    strDiligYn = ds.Tables[0].Rows[0]["DILIG_YN"].ToString();
                    strUsrId = ds.Tables[0].Rows[0]["USR_ID"].ToString();

                    if ((strDiligYn != "Y") && (strUsrId != SystemBase.Base.gstrUserID))
                    {
                        MessageBox.Show("근태관리담당자가 아니므로 다른 사원정보는 조회할 수 없습니다.");
                        this.Cursor = Cursors.Default;
                        return;
                    }
                    // 2022.07.13. hma 추가(End)

                    string strQuery = " usp_HAA009 'S1', @pDATE = '" + dtpDate.Text + "' ";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    strQuery += ", @pEMP_NO = '" + txtEmpNo.Text + "' ";      // 2022.07.13. hma 추가

                    UIForm.FPMake.grdCommSheet(fpSpread2, strQuery, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, true, 0, 0, true);

                    if (fpSpread2.Sheets[0].Rows.Count > 0)
                    {
                        // 상세정보조회
                        SubSearch(fpSpread2.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx2, "사원번호")].Text);
                    }
                    else
                    {
                        fpSpread1.Sheets[0].RowCount = 0;
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
        }
        #endregion

        #region 상세정보 조회
        private void SubSearch(string strEmpNo)
        {
            string strQuery = " usp_HAA009  'S2'";
            strQuery = strQuery + ", @pEMP_NO ='" + strEmpNo + "' ";
            strQuery = strQuery + ", @pDATE  ='" + dtpDate.Text + "' ";
            strQuery = strQuery + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

            UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 2, true);
        }
        #endregion

        #region 셀클릭시 상세조회
        private void fpSpread2_CellClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            SubSearch(fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "사원번호")].Text);
        }
        #endregion

        // 2022.07.13. hma 추가(Start)
        #region Check_RollGroup(): 사용자에 대한 근태관리여부를 체크한다.
        private void Check_RollGroup()
        {
            

            string strQuery = " usp_HAA009 @pTYPE = 'C1'";
            strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
            strQuery += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "' ";

            DataSet ds = SystemBase.DbOpen.NoTranDataSet(strQuery);

            string strDiligYn = "";
            strDiligYn = ds.Tables[0].Rows[0]["DILIG_YN"].ToString();
            strEmpNo = ds.Tables[0].Rows[0]["EMP_NO"].ToString();
            strEmpNm = ds.Tables[0].Rows[0]["USR_NM"].ToString();

            // 사용자 항목 활성화/비활성화 처리.
            if (strDiligYn == "Y")     // 근태관리여부 대상자이면 사용자 검색조건 입력 가능하게.
            {
                txtEmpNo.Enabled = true;
                btnEmpNo.Enabled = true;
                txtEmpNm.Enabled = true;
            }
            else
            {
                txtEmpNo.Value = strEmpNo;
                txtEmpNm.Value = strEmpNm;
                txtEmpNo.Enabled = false;
                btnEmpNo.Enabled = false;
                txtEmpNm.Enabled = false;
            }
        }
        #endregion

        #region btnEmpNo_Click(): 사원번호 버튼 클릭시 사원조회 팝업 띄워줌.
        private void btnEmpNo_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = " usp_H_COMMON @pType='H003', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };
                string[] strSearch = new string[] { txtEmpNo.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("H00002", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "사원 조회");
                pu.Width = 700;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtEmpNo.Value = Msgs[0].ToString();
                    txtEmpNm.Value = Msgs[1].ToString();
                    txtEmpNo.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "사원 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region txtEmpNo_TextChanged(): 사원번호 항목에 입력시 해당 사원번호의 사원명을 가져와서 항목에 넣어준다.
        private void txtEmpNo_TextChanged(object sender, EventArgs e)
        {
            string strQuery = "usp_H_COMMON @pType='H004', @pCOM_CD = '" + txtEmpNo.Text + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
            DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

            if (dt.Rows.Count > 0)
            {
                txtEmpNm.Value = dt.Rows[0][1].ToString();
            }
            else
            {
                txtEmpNm.Value = "";
            }
        }
        #endregion
        // 2022.07.13. hma 추가(End)
    }
}
