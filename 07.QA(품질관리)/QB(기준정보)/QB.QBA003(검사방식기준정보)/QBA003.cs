#region 작성정보
/*********************************************************************/
// 단위업무명 : 검사방식기준정보
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-02-19
// 작성내용 : 검사방식기준정보 및 관리
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

namespace QB.QBA003
{
    public partial class QBA003 : UIForm.FPCOMM1
    {
        #region 생성자
        public QBA003()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void QBA003_Load(object sender, System.EventArgs e)
        {
            //필수 항목 체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            SystemBase.ComboMake.C1Combo(cboInspGauge, "usp_B_COMMON @pType='COMM', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "',@pCODE = 'Q009', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 3); //검사측정속성

            //그리드 콤보박스 세팅			
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "검사기준방식")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'Q008', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0); //검사기준방식 
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "검사측정속성")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'Q009', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0); //검사측정속성

            //그리드 초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
        }
        #endregion

        #region NewExec()
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            //그리드 초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
        }
        #endregion

        #region 샘플링회수 1~7
        private void fpSpread1_Change(object sender, FarPoint.Win.Spread.ChangeEventArgs e)
        {
            if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "샘플링횟수"))
            {
                int iSampleCnt = Convert.ToInt32(fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "샘플링횟수")].Text);

                if (iSampleCnt < 1 || iSampleCnt > 7)
                {
                    MessageBox.Show("샘플링횟수는 1 ~ 7 사이 입니다!", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "샘플링횟수")].Text = "";
                }
            }
        }
        #endregion

        #region SearchExec()
        protected override void SearchExec()
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {
                string strQuery = " usp_QBA003 @pTYPE = 'S1'";
                strQuery += ", @pINSP_GAUGE = '" + cboInspGauge.SelectedValue + "' ";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false, true);

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이타 조회 중 오류가 발생하였습니다.
            }
            this.Cursor = Cursors.Default;
        }
        #endregion

        #region SaveExec()
        protected override void SaveExec()
        {
            fpSpread1.Focus();

            //그리드상단 필수 체크
            if (SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", true))
            {
                this.Cursor = Cursors.WaitCursor;

                string ERRCode = "ER", MSGCode = "P0000"; //처리할 내용이 없습니다.
                string strINSP_METH_CD = "";
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
                                case "I": strGbn = "I1"; break;
                                case "D": strGbn = "D1"; break;
                                default: strGbn = ""; break;
                            }
                            strINSP_METH_CD = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사기준방식")].Value.ToString() 
                                            + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사측정속성")].Value.ToString() 
                                            + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "샘플링횟수")].Text;
                            string strQuery = " usp_QBA003 @pTYPE = '" + strGbn + "'";
                            strQuery += ", @pINSP_METH_CD = '" + strINSP_METH_CD + "'";
                            strQuery += ", @pINSP_METH_NM = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사방식명")].Text + "'";
                            strQuery += ", @pINSP_STAND = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사기준방식")].Value + "'";
                            strQuery += ", @pINSP_GAUGE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사측정속성")].Value + "'";
                            strQuery += ", @pSAMPLE_CNT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "샘플링횟수")].Text + "'";
                            strQuery += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                            strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                            DataSet ds = SystemBase.DbOpen.TranDataSet(strQuery, dbConn, Trans);
                            ERRCode = ds.Tables[0].Rows[0][0].ToString();
                            MSGCode = ds.Tables[0].Rows[0][1].ToString();

                            if (ERRCode != "OK") { Trans.Rollback(); goto Exit; } //ER 코드 Return시 점프
                        }
                    }
                    Trans.Commit();
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    Trans.Rollback();
                    MSGCode = "P0001"; //에러가 발생되어 데이터 처리가 취소되었습니다.
                }
            Exit:
                dbConn.Close();

                if (ERRCode == "OK")
                {
                    SearchExec();
                    UIForm.FPMake.GridSetFocus(fpSpread1, strINSP_METH_CD, SystemBase.Base.GridHeadIndex(GHIdx1, "검사방식코드"));
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

                this.Cursor = Cursors.Default;
            }
        }
        #endregion

        #region 검사기준방식:2 검사측정속성:2 될수 없다
        private void fpSpread1_ComboCloseUp(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "검사기준방식"))
            {
                if (fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "검사기준방식")].Value != null)
                {
                    if (fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "검사기준방식")].Value.ToString() == "2")
                    {
                        string Query = "usp_QBA003 @pType='COMM', @pCODE = 'Q009', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' ";
                        Query += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                        UIForm.FPMake.grdComboRemake(fpSpread1, e.Row, e.Column + 1, SystemBase.ComboMake.ComboOnGrid(Query, 0));
                    }
                    else
                    {
                        string Query = "usp_B_COMMON @pType='COMM', @pCODE = 'Q009', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                        UIForm.FPMake.grdComboRemake(fpSpread1, e.Row, e.Column + 1, SystemBase.ComboMake.ComboOnGrid(Query, 0));
                    }
                }
            }
        }
        #endregion
    }
}
