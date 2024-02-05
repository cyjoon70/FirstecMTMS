﻿#region 작성정보
/*********************************************************************/
// 단위업무명 :프로젝트별 계약공수 upLoad
// 작 성 자 : 조 홍 태
// 작 성 일 : 2013-08-27
// 작성내용 : 프로젝트별 계약공수 upLoad
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
using System.Data.OleDb;

namespace CC.CCA004
{
    public partial class CCA004 : UIForm.FPCOMM1
    {
        #region 생성자
        public CCA004()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void CCA004_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            txtPlant_CD.Text = SystemBase.Base.gstrPLANT_CD.ToString();

            //그리드 초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);

            rdoWorkTypeMfg.Checked = true;      // 2019.01.25. hma 추가
        }
        #endregion

        #region NewExec()
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            txtPlant_CD.Text = SystemBase.Base.gstrPLANT_CD.ToString();

            //그리드 초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
        }
        #endregion

        #region SearchExec()
        protected override void SearchExec()
        {
            //조회조건 필수 체크
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1) == true)
            {
                this.Cursor = Cursors.WaitCursor;

                try
                {
                    // 2019.01.25. hma 추가(Start): 작업구분 검색조건 체크
                    string strWorkType = "";
                    
                    if (rdoWorkTypeMfg.Checked == true)
                        strWorkType = "M";
                    else if (rdoWorkTypeInsp.Checked == true)
                        strWorkType = "I";
                    else if (rdoWorkTypeDraw.Checked == true)
                        strWorkType = "D";
                    // 2019.01.25. hma 추가(End)

                    string strQuery = "usp_CCA004 @pTYPE = 'S1'";
                    strQuery = strQuery + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' ";
                    strQuery = strQuery + ", @pPLANT_CD = '" + txtPlant_CD.Text + "' ";
                    strQuery = strQuery + ", @pPROJECT_NO = '" + txtProject_No.Text + "' ";
                    strQuery = strQuery + ", @pPROJECT_SEQ = '" + txtProjectSeq.Text + "' ";
                    strQuery = strQuery + ", @pWORK_TYPE = '" + strWorkType + "' ";     // 2019.01.25. hma 추가

                    //string strDate = "";      // 2019.01.25. hma 수정: 사용하지 않는 변수이므로 주석처리함.
                    //if (txtProjectSeq.Text == "")
                    //{
                    //    strQuery = strQuery + ", @pDATE_FR = '" + dtpDtFr.Text + "' ";
                    //    strQuery = strQuery + ", @pDATE_TO = '" + dtpDtTo.Text + "' ";
                    //}
                    //else if (txtProject_No.Text != "" && (dtpDtFr.Text == "" || dtpDtTo.Text == ""))
                    //{
                    //    strDate = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD").Substring(1,7) + "-01").AddDays(-1).ToShortDateString();
                    //    strQuery = strQuery + ", @pDATE_FR = '1900-01-01' ";
                    //    strQuery = strQuery + ", @pDATE_TO = '" + strDate + "' ";
                    //}

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0, true);

                    //합계 컬러 넣기 및 Cell Span
                    int spanRow = 0;

                    for (int i = 0; i < fpSpread1.Sheets[0].RowCount; i++)
                    {
                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text == "ZZZZZZZZZZ")
                        {
                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text = "합계";
                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목명")].Text = "";

                            fpSpread1.Sheets[0].Cells[i, 0, i, fpSpread1.Sheets[0].ColumnCount - 1].BackColor = SystemBase.Base.gColor1;

                            spanRow++;

                            if (spanRow == 3)
                            {
                                fpSpread1.Sheets[0].Cells[i - 2, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].RowSpan = 3;
                                fpSpread1.Sheets[0].Cells[i - 2, SystemBase.Base.GridHeadIndex(GHIdx1, "품목명")].RowSpan = 3;
                            }
                        }
                        else
                        {
                            spanRow = 0;
                        }
                    }

                    fpSpread1.Sheets[0].SetColumnMerge(SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드"), FarPoint.Win.Spread.Model.MergePolicy.Always);
                    fpSpread1.Sheets[0].SetColumnMerge(SystemBase.Base.GridHeadIndex(GHIdx1, "품목명"), FarPoint.Win.Spread.Model.MergePolicy.Always);
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회중 오류가 발생하였습니다.
                }

                this.Cursor = Cursors.Default;
            }
        }
        #endregion

        #region 버튼 Click
        //공장 팝업
        private void btnPlant_CD_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON 'P011' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";								// 쿼리
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };				// 쿼리 인자값(조회조건)
                string[] strSearch = new string[] { txtPlant_CD.Text, "" };	// 쿼리 인자값에 들어갈 데이타

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00005", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "공장 조회", false);

                pu.ShowDialog();	//공통 팝업 호출
                if (pu.DialogResult == DialogResult.OK)
                {
                    string MSG = pu.ReturnVal.Replace("|", "#");
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(MSG);

                    txtPlant_CD.Value = Msgs[0].ToString();
                    txtPlant_NM.Value = Msgs[1].ToString();
                }


            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //프로젝트팝업
        private void btnProject_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW003 pu = new WNDW003(txtProject_No.Text, "S1", "");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtProject_No.Value = Msgs[3].ToString();
                    txtProject_Nm.Value = Msgs[4].ToString();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region TextChanged 이벤트
        // 공장
        private void txtPlant_CD_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtPlant_CD.Text != "")
                {
                    txtPlant_NM.Value = SystemBase.Base.CodeName("PLANT_CD", "PLANT_NM", "B_PLANT_INFO", txtPlant_CD.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");
                }
                else
                {
                    txtPlant_NM.Value = "";
                }
            }
            catch
            {

            }
        }

        //프로젝트번호
        private void txtProject_No_TextChanged(object sender, System.EventArgs e)
        {
            string Query = "SELECT TOP 1 PROJECT_NM FROM S_SO_MASTER(NOLOCK) WHERE PROJECT_NO = '" + txtProject_No.Text + "'  AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ";

            DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);

            if (dt.Rows.Count > 0)
            {
                txtProject_Nm.Value = dt.Rows[0][0].ToString();
            }
            else
            {
                txtProject_Nm.Value = "";
            }
        }
        #endregion
    }
}
