
#region 작성정보
/*********************************************************************/
// 단위업무명 : 부양가족공제자등록
// 작 성 자 : 권순철
// 작 성 일 : 2013-04-16
// 작성내용 : 부양가족공제자등록 및 관리
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
using System.Threading;
using WNDW;
using System.Text.RegularExpressions;
using System.IO;
using System.Reflection;

namespace HC.HCA007
{
    public partial class HCA007 : UIForm.FPCOMM1
    {
        #region 변수선언
        string strEmpNo = "";
        #endregion

        #region 생성자
        public HCA007()
        {
            InitializeComponent();
            strEmpNo = SystemBase.Base.gstrUserID.Replace("FST", "").ToString();
        }
        #endregion

        #region 로그인 사용자 체크 후 필수유무
        private void UsrCheck()
        {
            //로그인 유저가 'ADMIN' 이거나 'INSA' 인 경우 수정가능하게 한다.
            if (strEmpNo == "ADMIN" || strEmpNo == "INSA")
            {
                txtEmpNo.Tag = "사원번호;1;;";
                btnEmpNo.Tag = "";
                SystemBase.Validation.GroupBox_Setting(groupBox1);
            }
            else
            {
                txtEmpNo.Tag = ";2;;";
                btnEmpNo.Tag = ";2;;";
                SystemBase.Validation.GroupBox_Setting(groupBox1);
            }

            txtEmpNo.Text = strEmpNo;

            EmpDataInput(strEmpNo);
        }
        #endregion

        #region 사원데이터 자동기입
        private void EmpDataInput(string EmpNo)
        {
            string strQuery = "usp_H_COMMON @pType='H004', @pCOM_CD = '" + EmpNo + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
            DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

            if (dt.Rows.Count > 0)
            {
                txtEmpNm.Value = dt.Rows[0][1].ToString();
                txtDeptNm.Value = dt.Rows[0][2].ToString();
                txtRollPstn.Value = dt.Rows[0][3].ToString();
                txtEntrDt.Value = dt.Rows[0][4].ToString();
                txtPayGrd1.Value = dt.Rows[0][5].ToString();
                txtEmpNo.Focus();
            }
            else
            {
                txtEmpNm.Value = "";
                txtDeptNm.Value = "";
                txtRollPstn.Value = "";
                txtEntrDt.Value = "";
                txtPayGrd1.Value = "";
                txtEmpNo.Focus();
            }
        }
        #endregion

        #region Form Load 시
        private void HCA007_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            dtpYear.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).Year.ToString();

            //사용자 체크
            UsrCheck();

            //콤보박스 세팅
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "의료증빙코드")] = SystemBase.ComboMake.ComboOnGrid("usp_H_COMMON @pTYPE = 'H007', @pCOM_CD = 'H0152', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ", 0);//의료증빙코드
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "제출구분")] = SystemBase.ComboMake.ComboOnGrid("usp_H_COMMON @pTYPE = 'H008', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ", 0);//제출구분

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            dtpYear.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).Year.ToString();

            //사용자 체크
            UsrCheck();

            //콤보박스 세팅
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "의료증빙코드")] = SystemBase.ComboMake.ComboOnGrid("usp_H_COMMON @pTYPE = 'H007', @pCOM_CD = 'H0152', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ", 0);//의료증빙코드
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "제출구분")] = SystemBase.ComboMake.ComboOnGrid("usp_H_COMMON @pTYPE = 'H008', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ", 0);//제출구분

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1) == true)
                {
                    string strQuery = " usp_HCA007  @pTYPE = 'S1', @pYY = '" + dtpYear.Text + "', @pEMP_NO = '" + txtEmpNo.Text + "' ";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0);

                    if (fpSpread1.Sheets[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                        {
                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "관계코드")].Text == "0")
                            {
                                UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "세대주") + "|0");
                            }
                            else
                            {
                                UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "세대주") + "|3");
                            }
                        }
                    }
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

        #region Save
        protected override void SaveExec()
        {
            if (UIForm.FPMake.FPUpCheck(fpSpread1) == true)// 그리드 상단 필수항목 체크
            {
                string ERRCode = "ER", MSGCode = "P0000";	//처리할 내용이 없습니다.
                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                try
                {
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


                            string strBASE_YN = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "기본공제")].Text == "True" ? "Y" : "N";
                            string strLADY_YN = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "부녀자공제")].Text == "True" ? "Y" : "N";
                            string strOLD_YN = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "경로우대자공제")].Text == "True" ? "Y" : "N";
                            string strYOUNG_YN = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "다자녀추가공제")].Text == "True" ? "Y" : "N";
                            string strPARIA_YN = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "장애인")].Text == "True" ? "Y" : "N";
                            string strCHILD_YN = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "자녀양육비")].Text == "True" ? "Y" : "N";
                            string strCHILDBIRTH_YN = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "출산및입양추가공제")].Text == "True" ? "Y" : "N";
                            string strBASELIFE_YN = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "기초수급")].Text == "True" ? "Y" : "N";
                            string strHEAD_FLAG = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "세대주")].Text == "True" ? "1" : "2";
                            string strFOSTER_YN = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "위탁아동")].Text == "True" ? "Y" : "N";
                            string strINSUR_YN = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "보험료")].Text == "True" ? "Y" : "N";
                            string strMEDI_YN = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "의료비")].Text == "True" ? "Y" : "N";
                            string strEDU_YN = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "교육비")].Text == "True" ? "Y" : "N";
                            string strCARD_YN = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "신용카드등")].Text == "True" ? "Y" : "N";
                            string strCONTR_YN = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "기부금")].Text == "True" ? "Y" : "N";

                            string strSql = " usp_HCA007 @pTYPE = '" + strGbn + "' ";
                            strSql = strSql + ", @pYY = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "정산년도")].Text + "'";
                            strSql = strSql + ", @pEMP_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "사원번호")].Text + "'";
                            strSql = strSql + ", @pFAMILY_RES_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "주민번호")].Text + "'";
                            strSql = strSql + ", @pBASE_YN = '" + strBASE_YN + "'";
                            strSql = strSql + ", @pLADY_YN = '" + strLADY_YN + "'";
                            strSql = strSql + ", @pOLD_YN = '" + strOLD_YN + "'";
                            strSql = strSql + ", @pYOUNG_YN = '" + strYOUNG_YN + "'";
                            strSql = strSql + ", @pPARIA_YN = '" + strPARIA_YN + "'";
                            strSql = strSql + ", @pCHILD_YN = '" + strCHILD_YN + "'";
                            strSql = strSql + ", @pCHILDBIRTH_YN = '" + strCHILDBIRTH_YN + "'";
                            strSql = strSql + ", @pBASELIFE_YN = '" + strBASELIFE_YN + "'";
                            strSql = strSql + ", @pHEAD_FLAG = '" + strHEAD_FLAG + "'";
                            strSql = strSql + ", @pFOSTER_YN = '" + strFOSTER_YN + "'";
                            strSql = strSql + ", @pINSUR_YN = '" + strINSUR_YN + "'";
                            strSql = strSql + ", @pMEDI_YN = '" + strMEDI_YN + "'";
                            strSql = strSql + ", @pEDU_YN = '" + strEDU_YN + "'";
                            strSql = strSql + ", @pCARD_YN = '" + strCARD_YN + "'";
                            strSql = strSql + ", @pCONTR_YN = '" + strCONTR_YN + "'";

                            strSql = strSql + ", @pUP_ID  = '" + SystemBase.Base.gstrUserID.ToString() + "'";
                            strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

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
            }

        }
        #endregion

        #region 사원번호 팝업
        private void btnEmpNo_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_H_COMMON @pType='H003', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
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
                    txtDeptNm.Value = Msgs[2].ToString();
                    txtRollPstn.Value = Msgs[3].ToString();
                    txtEntrDt.Value = Msgs[4].ToString();
                    txtPayGrd1.Value = Msgs[5].ToString();
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

        #region 사원번호 TextChanged 이벤트
        private void txtEmpNo_TextChanged(object sender, System.EventArgs e)
        {
            EmpDataInput(txtEmpNo.Text);
        }
        #endregion

        #region 그리드 상 팝업
        protected override void fpButtonClick(int Row, int Column)
        {
            ////가족성명
            //if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "가족성명_2"))
            //{
            //    string strQuery = " usp_H_COMMON 'H005', @pSPEC1 = '" + txtEmpNo.Text + "', @pSPEC2 = '" + dtpYear.Text + "', @pSPEC3 = 'MEDI', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
            //    string[] strWhere = new string[] { "@pCOM_CD", "" };
            //    string[] strSearch = new string[] { fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "가족성명")].Text, "" };

            //    UIForm.FPPOPUP pu = new UIForm.FPPOPUP("H00003", strQuery, strWhere, strSearch, new int[] { 0 }, "가족성명 팝업");
            //    pu.ShowDialog();

            //    if (pu.DialogResult == DialogResult.OK)
            //    {
            //        Regex rx1 = new Regex("#");
            //        string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

            //        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "가족성명")].Text = Msgs[0].ToString();
            //        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "가족관계코드")].Text = Msgs[1].ToString();
            //        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "가족관계")].Text = Msgs[2].ToString();
            //        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "주민번호")].Text = Msgs[3].ToString();
            //        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "대상자구분")].Text = Msgs[4].ToString();

            //        UIForm.FPMake.fpChange(fpSpread1, Row);//수정플래그

            //    }
            //}
        }
        #endregion

        #region 그리드 상 Change
        protected override void fpSpread1_ChangeEvent(int Row, int Column)
        {
            //			//가족성명 
            //			if(Column == SystemBase.Base.GridHeadIndex(GHIdx1, "가족성명"))
            //			{
            //				string strQuery = " usp_H_COMMON 'H006', @pSPEC1 = '"+ txtEmpNo.Text +"', @pSPEC2 = '"+ dtpYear.Text +"', @pSPEC3 = 'MEDI', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
            //				strQuery = strQuery + " , @pCOM_CD = '"+ fpSpread1.Sheets[0].Cells[Row,SystemBase.Base.GridHeadIndex(GHIdx1,"가족성명")].Text +"' ";
            //				DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);
            //
            //				if(dt.Rows.Count > 0)
            //				{
            //					fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "가족관계코드")].Text = dt.Rows[0][1].ToString();
            //					fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "가족관계")].Text = dt.Rows[0][2].ToString();
            //					fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "주민번호")].Text = dt.Rows[0][3].ToString();
            //					fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "대상자구분")].Text = dt.Rows[0][4].ToString();
            //				}
            //				else
            //				{
            //					fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "가족관계코드")].Text = "";
            //					fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "가족관계")].Text = "";
            //					fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "주민번호")].Text = "";
            //					fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "대상자구분")].Text = "";
            //
            //					if(fpSpread1.Sheets[0].Cells[Row,SystemBase.Base.GridHeadIndex(GHIdx1,"가족성명")].Text != "")
            //					{
            //						MessageBox.Show(SystemBase.Base.MessageRtn("의료비 여부가 'N'이거나 존재하지 않는 가족성명입니다."), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //					}
            //				}
            //			}
        }
        #endregion
    }
}
