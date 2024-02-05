
#region 작성정보
/*********************************************************************/
// 단위업무명 : 품목별 단위 환산등록
// 작 성 자 : 조 홍 태
// 작 성 일 : 2013-02-01
// 작성내용 : 품목별 단위 환산등록 및 관리
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

namespace BI.BBI005
{
    public partial class BBI005 : UIForm.FPCOMM1
    {
        #region 생성자
        public BBI005()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void BBI005_Load(object sender, System.EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(groupBox1);//필수 적용

            SystemBase.ComboMake.C1Combo(cboPlant, "usp_B_COMMON @pTYPE = 'PLANT' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");	//공장
            SystemBase.ComboMake.C1Combo(cboFrUnit, "usp_B_COMMON @pType='COMM', @pCODE = 'Z005', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 3);
            SystemBase.ComboMake.C1Combo(cboToUnit, "usp_B_COMMON @pType='COMM', @pCODE = 'Z005', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 3);
            
            cboPlant.SelectedValue = SystemBase.Base.gstrPLANT_CD;

            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "기준단위")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'Z005', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0); //기준단위
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "변환단위")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'Z005', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0); //변환단위
			
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            fpSpread1.Sheets[0].RowCount = 0;
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = Cursors.WaitCursor;

            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                string strQuery = "usp_BBI005  'S1'";
                strQuery = strQuery + ", @pPLANT_CD ='" + Convert.ToString(cboPlant.SelectedValue) + "' ";
                strQuery = strQuery + ", @pITEM_CD ='" + txtItemCd.Text.Trim() + "' ";
                strQuery = strQuery + ", @pFR_UNIT ='" + Convert.ToString(cboFrUnit.SelectedValue) + "' ";
                strQuery = strQuery + ", @pTO_UNIT ='" + Convert.ToString(cboToUnit.SelectedValue) + "' ";
                strQuery = strQuery + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0);

                if (fpSpread1.Sheets[0].RowCount > 0) UIForm.FPMake.grdReMake(fpSpread1, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드_2") + "|5");
            }

            this.Cursor = Cursors.Default;
        }
        #endregion

        #region SaveExec() 폼에 입력된 데이타 저장 로직
        protected override void SaveExec()
        {
            this.Cursor = Cursors.WaitCursor;

            if ((SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", true) == true))// 그리드 필수항목 체크 
            {
                string ERRCode = "ER", MSGCode = "SY001";	//처리할 내용이 없습니다.
                string strItemCd = "";

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

                            strItemCd = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text;

                            string strSql = " usp_BBI005 '" + strGbn + "'";
                            strSql = strSql + ", @pPLANT_CD = '" + Convert.ToString(cboPlant.SelectedValue) + "'";
                            strSql = strSql + ", @pITEM_CD  = '" + strItemCd + "'";
                            strSql = strSql + ", @pFR_UNIT  = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "기준단위")].Value + "'";
                            strSql = strSql + ", @pTO_UNIT  = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "변환단위")].Value + "'";
                            strSql = strSql + ", @pTRAN_FAC = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "변환계수")].Value + "'";
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
                catch
                {
                    Trans.Rollback();
                    MSGCode = "SY002";	//에러가 발생하여 데이터 처리가 취소되었습니다.
                }
            Exit:
                dbConn.Close();

                if (ERRCode == "OK")
                {
                    SearchExec();
                    UIForm.FPMake.GridSetFocus(fpSpread1, strItemCd);//
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

            this.Cursor = Cursors.Default;
        }
        #endregion

        #region fpButtonClick() 그리드 버튼클릭
        protected override void fpButtonClick(int Row, int Column)
        {
            try
            {
                if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드_2"))
                {
                    string strPlantCd = Convert.ToString(cboPlant.SelectedValue);
                    WNDW.WNDW005 pu = new WNDW.WNDW005(strPlantCd, true); //일반품목
                    pu.ShowDialog();
                    if (pu.DialogResult == DialogResult.OK)
                    {
                        string[] Msgs = pu.ReturnVal;

                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text = Msgs[2].ToString();
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목명")].Value = Msgs[3].ToString();
                        fpSpread1.ActiveSheet.SetActiveCell(Row, SystemBase.Base.GridHeadIndex(GHIdx1, "기준단위"));
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn(f.ToString()), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 그리드 change
        protected override void fpSpread1_ChangeEvent(int Row, int Col)
        {
            if (Col == SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드"))
            {
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목명")].Text = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
            }
        }
        #endregion

        #region 품목
        private void txtItemCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtItemCd.Text != "")
                {
                    txtItemNm.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtItemCd.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtItemNm.Value = "";
                }
            }
            catch { }
        }

        private void btnITEM_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW.WNDW005 pu = new WNDW.WNDW005(cboPlant.SelectedValue.ToString(), true, txtItemCd.Text);
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtItemCd.Text = Msgs[2].ToString();
                    txtItemNm.Value = Msgs[3].ToString();
                    txtItemCd.Focus();
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
