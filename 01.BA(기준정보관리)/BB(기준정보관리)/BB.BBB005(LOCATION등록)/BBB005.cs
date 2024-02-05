#region 작성정보
/*********************************************************************/
// 단위업무명 : LOCATION 등록
// 작 성 자 : 김 현 근
// 작 성 일 : 2013-03-25
// 작성내용 : LOCATION 등록 및 관리
// 수 정 일 :
// 수 정 자 :
// 수정내용 :
// 비    고 :
/*********************************************************************/
#endregion

using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Globalization;
using System.Text.RegularExpressions;

namespace BB.BBB005
{
    public partial class BBB005 : UIForm.FPCOMM2
    {
        #region 생성자
        public BBB005()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void BBB005_Load(object sender, System.EventArgs e)
        {
            //그룹박스 필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            SystemBase.Validation.GroupBox_Setting(groupBox2);

            //콤보박스 세팅
            SystemBase.ComboMake.C1Combo(cboSPlantCd, "usp_B_COMMON @pType='PLANT', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");
            SystemBase.ComboMake.C1Combo(cboPlantCd, "usp_B_COMMON @pType='PLANT', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");

            cboSPlantCd.SelectedValue = SystemBase.Base.gstrPLANT_CD;
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            SystemBase.Validation.GroupBox_Reset(groupBox2);

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt,false, false, 0,0);
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {
                    string strQuery = " usp_BBB005 'S1'";
                    strQuery = strQuery + ", @pLANG_CD ='" + SystemBase.Base.gstrLangCd + "' ";
                    strQuery = strQuery + ", @pPLANT_CD ='" + cboSPlantCd.SelectedValue + "'";
                    strQuery = strQuery + ", @pSL_CD ='" + txtSSlCd.Text.Trim() + "' ";
                    strQuery = strQuery + ", @pSL_NM ='" + txtSSlNm.Text + "' ";
                    strQuery = strQuery + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread2, strQuery, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, true, 0, 0, false);
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }

            this.Cursor = Cursors.Default;
        }
        #endregion
        
        #region SaveExec() 폼에 입력된 데이타 저장 로직
        protected override void SaveExec()
        {
            this.Cursor = Cursors.WaitCursor;

            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox2))  //컨트롤 필수여부체크 
            {
                string ERRCode = "ER", MSGCode = "SY001";	//처리할 내용이 없습니다.
                bool ChkMsg = true;

                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                try
                {
                    //행수만큼 처리
                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {
                        string strHead = fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text;
                        string strGbn = "";
                        if (strHead.Length > 0)
                        {
                            switch (strHead)
                            {
                                case "U": strGbn = "U1"; break;
                                case "D": strGbn = "D1"; break;
                                case "I": strGbn = "U1"; break;
                                default: strGbn = ""; break;
                            }

                            string strSql = " usp_BBB005 '" + strGbn + "'";
                            strSql = strSql + ", @pLANG_CD  = '" + SystemBase.Base.gstrLangCd + "'";
                            strSql = strSql + ", @pPLANT_CD = '" + cboPlantCd.SelectedValue + "'";
                            strSql = strSql + ", @pSL_CD = '" + txtSlCd.Text.Trim() + "'";
                            strSql = strSql + ", @pLOCATION_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "LOCATION 코드")].Text + "'";
                            strSql = strSql + ", @pLOCATION_NM = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "LOCATION 명")].Text + "'";
                            strSql = strSql + ", @pLOCATION_DETAIL = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "LOCATION내용")].Text + "'";
                            strSql = strSql + ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                            strSql = strSql + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                            DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                            ERRCode = ds.Tables[0].Rows[0][0].ToString();
                            MSGCode = ds.Tables[0].Rows[0][1].ToString();

                            if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
                        }
                    }
                    Trans.Commit();
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    Trans.Rollback();
                    MSGCode = "P0001";	//에러가 발생하여 데이터 처리가 취소되었습니다.
                    this.Cursor = Cursors.Default;
                }
            Exit:
                dbConn.Close();

                if (ChkMsg == true)
                {
                    if (ERRCode == "OK")
                    {
                        Search2();
                        MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                    else if (ERRCode == "ER")
                    {
                        MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }

            this.Cursor = Cursors.Default;
        }
        #endregion

        #region 좌측 fpSpread 클릭시 우측상세조회
        private void fpSpread2_SelectionChanged(object sender, FarPoint.Win.Spread.SelectionChangedEventArgs e)
        {
            if (fpSpread2.Sheets[0].Rows.Count > 0)
            {
                Search2();
            }
        }
        #endregion

        #region Location조회
        private void Search2()
        {
            this.Cursor =Cursors.WaitCursor;           

            try
            {
                int intRow = fpSpread2.ActiveSheet.GetSelection(0).Row;
                cboPlantCd.SelectedValue = cboSPlantCd.SelectedValue;
                txtSlCd.Value = fpSpread2.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx2, "창고코드")].Text.ToString();
                txtSlNm.Value = fpSpread2.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx2, "창고명")].Text.ToString();

                string strQuery = " usp_BBB005  'S2'";
                strQuery = strQuery + ", @pLANG_CD ='" + SystemBase.Base.gstrLangCd + "' ";
                strQuery = strQuery + ", @pPLANT_CD ='" + cboPlantCd.SelectedValue + "'";
                strQuery = strQuery + ", @pSL_CD ='" + txtSlCd.Text + "' ";
                strQuery = strQuery + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0, false);
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }

            this.Cursor = Cursors.Default;
        }
        #endregion

    }
}