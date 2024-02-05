#region 작성정보
/*********************************************************************/
// 단위업무명 : 라우팅정보조회
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-04-15
// 작성내용 : 라우팅정보조회 및 관리
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

namespace PA.PBA161
{
    public partial class PBA161 : UIForm.FPCOMM2
    {
        #region 변수선언
        string strInDataYn = "";
        #endregion

        #region 생성자
        public PBA161()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void PBA161_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            G2Etc[SystemBase.Base.GridHeadIndex(GHIdx2, "주라우팅")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'B029', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 1);
            //그리드 초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
            UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, 0, 0, false);

            //기타세팅
            txtPlantCd.Value = SystemBase.Base.gstrPLANT_CD;
            dtpValdToDtFr.Value = "";
            dtpValdToDtTo.Value = "";

            rdoInDataY.Checked = true;

        }
        #endregion
        
        #region NewExec()
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            fpSpread1.Sheets[0].Rows.Count = 0;
            fpSpread2.Sheets[0].Rows.Count = 0;

            txtPlantCd.Value = SystemBase.Base.gstrPLANT_CD;
            dtpValdToDtFr.Value = "";
            dtpValdToDtTo.Value = "";
        }
        #endregion

        #region 조회조건 팝업
        //공장
        private void btnPlantCd_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = "usp_B_COMMON @pTYPE = 'TABLE_POP' ,@pSPEC1 = 'PLANT_CD', @pSPEC2 = 'PLANT_NM', @pSPEC3 = 'B_PLANT_INFO', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtPlantCd.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00005", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "공장코드 조회");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtPlantCd.Value = Msgs[0].ToString();
                    txtPlantNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "공장코드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);	//데이터 조회 중 오류가 발생하였습니다.

            }
        }

        //품목코드
        private void btnItemCd_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW005 pu = new WNDW005(txtPlantCd.Text, true, txtItemCd.Text);
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtItemCd.Value = Msgs[2].ToString();
                    txtItemNm.Value = Msgs[3].ToString();
                    txtItemCd.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목코드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //라우팅
        private void btnRoutNo_Click(object sender, System.EventArgs e)
        {
            try
            {

                
                if (txtItemCd.Text == "")  // 품목코드 검사
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn("P0030"));
                    return;
                }

                string strQuery = "usp_Q_COMMON @pType='Q030', @pSPEC1 = '" + txtPlantCd.Text + "', @pSPEC2 = '" + txtItemCd.Text + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtRoutNo.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P06005", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "라우팅번호 조회");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtRoutNo.Value = Msgs[0].ToString();
                    txtRoutNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "라우팅번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }
        #endregion

        #region 조회조건 TextChanged
        //공장
        private void txtPlantCd_TextChanged(object sender, System.EventArgs e)
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

        //품목코드
        private void txtItemCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtItemCd.Text != "")
                {
                    txtItemNm.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtItemCd.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtItemNm.Value = "";
                }
            }
            catch
            {

            }
        }

        //라우팅
        private void txtRoutNo_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtRoutNo.Text != "")
                {
                    txtRoutNm.Value = SystemBase.Base.CodeName("ROUT_NO", "DESCRIPTION", "P_BOP_PROC_MASTER", txtRoutNo.Text, " AND PLANT_CD = '" + txtPlantCd.Text + "' AND ITEM_CD = '" + txtItemCd.Text + "' AND CO_CD='" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtRoutNm.Value = "";
                }
            }
            catch
            {

            }
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {
                    if (rdoInDataY.Checked == true) { strInDataYn = "Y"; }
                    else if (rdoInDataN.Checked == true) { strInDataYn = "N"; }

                    string strQuery = " usp_PBA161 @pTYPE = 'S1'";
                    strQuery += ", @pPLANT_CD ='" + txtPlantCd.Text + "'";
                    strQuery += ", @pITEM_CD ='" + txtItemCd.Text + "'";
                    strQuery += ", @pROUT_NO ='" + txtRoutNo.Text + "'";
                    strQuery += ", @pVALD_TO_DT_FR ='" + dtpValdToDtFr.Text + "'";
                    strQuery += ", @pVALD_TO_DT_TO ='" + dtpValdToDtTo.Text + "'";
                    strQuery += ", @pIN_DATA_YN ='" + strInDataYn + "'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread2, strQuery, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, true, 0, 1);
                    if (strInDataYn == "Y")
                    {
                        UIForm.FPMake.grdReMake(fpSpread2, SystemBase.Base.GridHeadIndex(GHIdx2, "주라우팅") + "|1");
                    }
                    else
                    {
                        UIForm.FPMake.grdReMake(fpSpread2, SystemBase.Base.GridHeadIndex(GHIdx2, "주라우팅") + "|3");
                    }

                    if (fpSpread2.Sheets[0].Rows.Count > 0)
                    { SubSearch(0); }
                    else
                    { fpSpread1.Sheets[0].Rows.Count = 0; }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            this.Cursor = System.Windows.Forms.Cursors.Default;
        }
        #endregion

        #region 그리드상세조회
        private void SubSearch(int Row)
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {
                string strQuery = " usp_PBA161 @pTYPE = 'S2'";
                strQuery += ", @pPLANT_CD ='" + fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "공장코드")].Text + "'";
                strQuery += ", @pITEM_CD ='" + fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "품목")].Text + "'";
                strQuery += ", @pROUT_NO ='" + fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "라우팅")].Text + "'";
                strQuery += ", @pLANG_CD ='" + SystemBase.Base.gstrLangCd + "'";
                strQuery += ", @pCO_CD ='" + SystemBase.Base.gstrCOMCD + "'";

                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 1);

                for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                {
                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "공정내용")].Value != null)
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "공정내용")].Value = 'Y';
                    else
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "공정내용")].Value = 'N';
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            this.Cursor = Cursors.Default;
        }
        #endregion
        
        #region SaveExec() 폼에 입력된 데이타 저장 로직
        protected override void SaveExec()
        {
            fpSpread2.Focus();

            if ((SystemBase.Validation.FPGrid_SaveCheck(fpSpread2, this.Name, "fpSpread2", true) || SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", true)) && strInDataYn == "Y")
            {
                string ERRCode = "ER", MSGCode = "P0000"; //처리할 내용이 없습니다.

                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                this.Cursor = Cursors.WaitCursor;
                string s_item = "";
                int item_idx = SystemBase.Base.GridHeadIndex(GHIdx2, "품목");
                int major_idx = SystemBase.Base.GridHeadIndex(GHIdx2, "주라우팅");
                try
                {
                    for (int i = 0; i < fpSpread2.ActiveSheet.Rows.Count; i++)
                    {
                        string strHead = fpSpread2.Sheets[0].RowHeader.Cells[i, 0].Text;
                        string strGbn = "";

                        if (strHead.Length > 0)
                        {
                            switch (strHead)
                            {
                                case "U": strGbn = "U1"; break;
                                default: strGbn = ""; break;
                            }
                            string strSql = " usp_PBA161";
                            strSql = strSql + " @pType = '" + strGbn + "'";
                            strSql = strSql + ", @pPLANT_CD = '" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "공장코드")].Text + "'";
                            strSql = strSql + ", @pITEM_CD = '" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "품목")].Text + "'";
                            strSql = strSql + ", @pROUT_NO = '" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "라우팅")].Text + "'";
                            strSql = strSql + ", @pMAJOR_FLG= '" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "주라우팅")].Value + "'";
                            strSql = strSql + ", @pUPDT_USER_ID = '" + SystemBase.Base.gstrUserID + "'";
                            strSql = strSql + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";

                            DataSet ds1 = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                            ERRCode = ds1.Tables[0].Rows[0][0].ToString();
                            MSGCode = ds1.Tables[0].Rows[0][1].ToString();

                            if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                        }
                    }

                    for (int i = 0; i < fpSpread1.ActiveSheet.Rows.Count; i++)
                    {
                        string strHead = fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text;
                        string strGbn = "";

                        if (strHead.Length > 0)
                        {
                            switch (strHead)
                            {
                                case "U": strGbn = "U2"; break;
                                default: strGbn = ""; break;
                            }

                            string strSql = " usp_PBA161";
                            strSql = strSql + " @pType = '" + strGbn + "'";
                            strSql = strSql + ", @pPLANT_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "공장코드")].Text + "'";
                            strSql = strSql + ", @pITEM_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목")].Text + "'";
                            strSql = strSql + ", @pROUT_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "라우팅")].Text + "'";
                            strSql = strSql + ", @pPROC_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "공정")].Text + "'";
                            strSql = strSql + ", @pROUT_CYCLE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "공정주기")].Text + "'";
                            strSql = strSql + ", @pUPDT_USER_ID = '" + SystemBase.Base.gstrUserID + "'";
                            strSql = strSql + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                            strSql = strSql + ", @pRUN_TIME= '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "변동가동시간")].Text + "'";
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
                    ERRCode = "ER";
                    MSGCode = f.Message;
                    //MSGCode = "P0019";
                }
            Exit:
                dbConn.Close();
                if (ERRCode == "OK")
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    SearchExec();
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
        
        #region 마스터그리드 선택시
        private void fpSpread2_LeaveCell(object sender, FarPoint.Win.Spread.LeaveCellEventArgs e)
        {
            if (fpSpread2.Sheets[0].Rows.Count > 0)
            {
                if (e.Row != e.NewRow)
                {
                    try
                    {
                        SubSearch(e.NewRow);
                    }
                    catch (Exception f)
                    {
                        SystemBase.Loggers.Log(this.Name, f.ToString());
                        MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                        //데이터 조회 중 오류가 발생하였습니다.				
                    }
                }
            }
        }
        #endregion

        #region fpSpread2_ComboSelChange
        private void fpSpread2_ComboSelChange(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            fpSpread2.Focus();

            if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx2, "주라우팅"))
            {
                string strItemCd = fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "품목")].Text;

                if (strItemCd.Length > 0 && strItemCd.Trim() != "")
                {
                    if (Convert.ToString(fpSpread2.Sheets[0].Cells[e.Row, e.Column].Value) == "N") // 하나가 N 이면 안된다고 메세지 처리!
                    {
                        bool chk = true;

                        for (int i = 0; i < fpSpread2.Sheets[0].Rows.Count; i++)
                        {
                            if (fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "품목")].Text == strItemCd
                                && Convert.ToString(fpSpread2.Sheets[0].Cells[i, e.Column].Value) == "Y")
                            {
                                chk = false;
                            }
                        }

                        if (chk == true)
                        {
                            fpSpread2.Sheets[0].Cells[e.Row, e.Column].Value = "Y";
                            MessageBox.Show("품목코드 : [" + strItemCd + "] 의 주라우팅이 하나라도 선택되어야 합니다.", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }

                    fpSpread2.Sheets[0].RowHeader.Cells[e.Row, 0].Text = "U";

                    if (Convert.ToString(fpSpread2.Sheets[0].Cells[e.Row, e.Column].Value) == "Y") // 하나가 Y 이면 나머지는 N로 변경!
                    {
                        for (int i = 0; i < fpSpread2.Sheets[0].Rows.Count; i++)
                        {
                            if (fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "품목")].Text == strItemCd
                                && i != e.Row)
                            {
                                fpSpread2.Sheets[0].RowHeader.Cells[i, 0].Text = "U";
                                fpSpread2.Sheets[0].Cells[i, e.Column].Value = "N";
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region fpSpread1_ButtonClicked 그리드 상단 버튼 클릭
        private void fpSpread1_ButtonClicked(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            try
            {

                if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "공정내용_2")) // 공정내용
                {
                    string PROC_PLAN = "X";
                    if(fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "공정내용")].Text == "Y")
                        PROC_PLAN = "U1";


                    WNDW.WNDW035 pu = new WNDW.WNDW035("X",
                                                       fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목")].Text,
                                                       fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "라우팅")].Text,
                                                       fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "공정")].Text,
                                                       fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "공정코드")].Text,
                                                       fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "공정명")].Text,
                                                       fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "자원")].Text,
                                                       fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "자원명")].Text,
                                                       PROC_PLAN);
                    pu.ShowDialog();

                    if (pu.DialogResult == DialogResult.Cancel)
                    {
                        string Msgs = pu.ReturnData();

                        UIForm.FPMake.fpChange(fpSpread1, e.Row);
                    }

                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "그리드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion
    }
}
