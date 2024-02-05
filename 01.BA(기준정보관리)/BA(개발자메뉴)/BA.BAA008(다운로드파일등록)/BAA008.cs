using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Data;
using System.Data.SqlClient;

namespace BA.BAA008
{
    public partial class BAA008 : UIForm.FPCOMM1
    {
        #region 생성자
        public BAA008()
        {
            InitializeComponent();
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            txtFILE_NM.Text = "";
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
            txtFILE_NM.Focus();
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                string strQuery = " usp_BAA008  'S1'";
                strQuery = strQuery + ", @pDOWNFILENAME ='" + txtFILE_NM.Text + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false, true);
                fpSpread1.Sheets[0].SetColumnAllowAutoSort(-1, true);
            }
        }
        #endregion

        #region SaveExec() 폼에 입력된 데이타 저장 로직
        protected override void SaveExec()
        {
            //Major 코드 필수항목 체크
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                if (UIForm.FPMake.FPUpCheck(fpSpread1) == true) // 그리드 상단 필수항목 체크
                {
                    bool chk = true;
                    string ERRCode = "ER", MSGCode = "P0000";	//처리할 내용이 없습니다.
                    string strKeyCd = "";
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
                                    case "I": strGbn = "I1"; break;
                                    default: strGbn = ""; break;
                                }

                                string strNUM = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "NUM")].Text.ToString();
                                string strFILE_NM = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "FILE NAME")].Text.ToString();
                                string strVER = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "버전")].Value.ToString();
                                string strWHERE = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "파일위치")].Text.ToString();
                                string strUSE = "N"; if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "사용여부")].Text.ToString() == "True") strUSE = "Y";
                                string strUSE_TYPE = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "사용구분")].Value.ToString();
                                strKeyCd = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "FILE NAME")].Text.ToString();

                                string strSql = " usp_BAA008 '" + strGbn + "'";
                                strSql = strSql + ", @pNUM = '" + strNUM + "'";
                                strSql = strSql + ", @pDOWNFILENAME = '" + strFILE_NM + "'";
                                strSql = strSql + ", @pPRGSIZE = '" + strVER.Trim() + "'";
                                strSql = strSql + ", @pFILEWHERE = '" + strWHERE.Trim() + "'";
                                strSql = strSql + ", @pUSERYN = '" + strUSE + "'";
                                strSql = strSql + ", @pUSE_TYPE = '" + strUSE_TYPE + "'";
                                strSql = strSql + ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";

                                DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                                ERRCode = ds.Tables[0].Rows[0][0].ToString();
                                MSGCode = ds.Tables[0].Rows[0][1].ToString();

                                if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
                            }
                        }
                        Trans.Commit();
                    }
                    catch
                    {
                        Trans.Rollback();
                        MSGCode = "P0001";	//에러가 발생하여 데이터 처리가 취소되었습니다.
                        chk = false;
                    }
                Exit:
                    dbConn.Close();

                    if (ERRCode == "OK")
                    {
                        if (chk == true)
                        {
                            SearchExec();
                            UIForm.FPMake.GridSetFocus(fpSpread1, strKeyCd);
                        }
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
        }
        #endregion

        #region Form Load 시
        private void BZA008_Load(object sender, System.EventArgs e)
        {
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "사용구분")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'B053', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);

        }
        #endregion

        #region 버전업
        private void button1_Click(object sender, System.EventArgs e)
        {
            if (fpSpread1.ActiveSheet.GetSelection(0) != null && fpSpread1.Sheets[0].Rows.Count > 0)
            {
                try
                {
                    int iRow = fpSpread1.ActiveSheet.GetSelection(0).Row;

                    string strBefoValue = fpSpread1.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx1, "버전")].Text;

                    if (fpSpread1.Sheets[0].RowHeader.Cells[iRow, 0].Text != "U")
                    {
                        fpSpread1.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx1, "버전")].Value = Convert.ToDouble(strBefoValue) + 0.01;
                        fpSpread1.Sheets[0].RowHeader.Cells[iRow, 0].Text = "U";
                    }
                }
                catch (Exception f)
                {
                    MessageBox.Show(f.ToString());
                }
            }
        }
        #endregion

    }
}
