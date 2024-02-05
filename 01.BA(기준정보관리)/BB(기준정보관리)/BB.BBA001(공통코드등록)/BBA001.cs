using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Globalization;

namespace BB.BBA001
{
    public partial class BBA001 : UIForm.FPCOMM2
    {
        #region 변수선언
        int PreRow = -1;
        #endregion

        #region 생성자
        public BBA001()
        {
            InitializeComponent();
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox2);
            SystemBase.Validation.GroupBox_Setting(groupBox2);
            SystemBase.Validation.Control_SearchCheck(groupBox2);

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt,false, false, 0,0);
            UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, false, 0, 0);

            PreRow = -1;
            txtMCode.Focus();

        }
        #endregion

        #region 행추가 버튼 클릭 이벤트
        protected override void RowInsExec()
        {
            UIForm.FPMake.RowInsert(fpSpread1);

            fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "사용여부")].Text = "True";

        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                string strQuery = " usp_BBA001  'S1'";
                strQuery = strQuery + ", @pCOMP_CODE= '"+ SystemBase.Base.gstrCOMCD.ToString() +"' ";
                strQuery = strQuery + ", @pMAJOR_CODE ='" + txtH_Code.Text + "' ";
                strQuery = strQuery + ", @pCODE_NAME ='" + txtCodenm.Text + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread2, strQuery, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, true, 0, 0, false);
            }
        }
        #endregion

        #region SaveExec() 폼에 입력된 데이타 저장 로직
        protected override void SaveExec()
        {
            //Major 코드 필수항목 체크
            if ((SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", false) == true))// 그리드 필수항목 체크 
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox2))  //컨트롤 필수여부체크 
                {
                    Major_Save();

                    string strMajorCd = txtMCode.Text;
                    string strMinorCd = "";

                    string ERRCode = "ER", MSGCode = "SY001";	//처리할 내용이 없습니다.
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
                                    case "I": strGbn = "I2"; break;
                                    default: strGbn = ""; break;
                                }

                                strMinorCd = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "코드")].Text.ToString();
                                string strCodeNm = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "코드명")].Text.ToString();

                                int iSort = 0;
                                if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "정렬순서")].Text.ToString() == "")
                                    iSort = 0;
                                else
                                    iSort = Convert.ToInt32(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "정렬순서")].Text.ToString());

                                string strRel01 = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Rel.01")].Text.ToString();
                                string strRel02 = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Rel.02")].Text.ToString();
                                string strRel03 = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Rel.03")].Text.ToString();
                                string strRel04 = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Rel.04")].Text.ToString();
                                string strRel05 = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Rel.05")].Text.ToString();
                                string strRel06 = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Rel.06")].Text.ToString();
                                string strRel07 = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Rel.07")].Text.ToString();
                                string strRel08 = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Rel.08")].Text.ToString();
                                string strRel09 = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Rel.09")].Text.ToString();
                                string strRel10 = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Rel.10")].Text.ToString();
                                string strDefFlag = "0"; if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Default")].Text.ToString() == "True") strDefFlag = "1";
                                string strUseyn = "0"; if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "사용여부")].Text.ToString() == "True") strUseyn = "1";
                                string strRemark = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "비고")].Text.ToString();

                                string strSql = " usp_BBA001 '" + strGbn + "'";
                                strSql = strSql + ", @pCOMP_CODE = '" + SystemBase.Base.gstrCOMCD.ToString() + "' ";
                                strSql = strSql + ", @pMAJOR_CODE = '" + strMajorCd + "'";
                                strSql = strSql + ", @pMINOR_CODE = '" + strMinorCd + "'";
                                strSql = strSql + ", @pCODE_NAME    = '" + strCodeNm + "'";
                                strSql = strSql + ", @pSORT_NO  = '" + iSort + "'";
                                strSql = strSql + ", @pDEF_FLAG  = '" + strDefFlag + "'";
                                strSql = strSql + ", @pREL_CD1  = '" + strRel01 + "'";
                                strSql = strSql + ", @pREL_CD2  = '" + strRel02 + "'";
                                strSql = strSql + ", @pREL_CD3  = '" + strRel03 + "'";
                                strSql = strSql + ", @pREL_CD4  = '" + strRel04 + "'";
                                strSql = strSql + ", @pREL_CD5  = '" + strRel05 + "'";
                                strSql = strSql + ", @pREL_CD6  = '" + strRel06 + "'";
                                strSql = strSql + ", @pREL_CD7  = '" + strRel07 + "'";
                                strSql = strSql + ", @pREL_CD8  = '" + strRel08 + "'";
                                strSql = strSql + ", @pREL_CD9  = '" + strRel09 + "'";
                                strSql = strSql + ", @pREL_CD10  = '" + strRel10 + "'";
                                strSql = strSql + ", @pUSE_YN  = '" + strUseyn + "'";
                                strSql = strSql + ", @pREMARK   = '" + strRemark + "'";

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
                        MSGCode = "SY002";	//에러가 발생하여 데이터 처리가 취소되었습니다.
                    }
                Exit:
                    dbConn.Close();

                    if (ERRCode == "OK")
                    {
                        SearchExec();
                        Right_Focus(strMajorCd);

                        //좌측 frSpread 재조회
                        string strSql1 = " usp_BBA001  'S2' ";
                        strSql1 = strSql1 + ", @pCOMP_CODE = '" + SystemBase.Base.gstrCOMCD.ToString() + "' ";
                        strSql1 = strSql1 + ", @pMAJOR_CODE = '" + strMajorCd + "'";

                        UIForm.FPMake.grdCommSheet(fpSpread1, strSql1, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);

                        UIForm.FPMake.GridSetFocus(fpSpread2, strMajorCd, SystemBase.Base.GridHeadIndex(GHIdx2, "코드"));
                        UIForm.FPMake.GridSetFocus(fpSpread1, strMinorCd, SystemBase.Base.GridHeadIndex(GHIdx1, "코드"));

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

        #region DelExec() 삭제 로직
        protected override void DeleteExec()
        {
            DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("SY010"), SystemBase.Base.MessageRtn("Z0004"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dsMsg == DialogResult.Yes)
            {
                string ERRCode = "ER", MSGCode = "SY001";	//처리할 내용이 없습니다.
                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                try
                {
                    string strSql = " usp_BBA001  'D2'";
                    strSql = strSql + ", @pCOMP_CODE = '" + SystemBase.Base.gstrCOMCD.ToString() + "' ";
                    strSql = strSql + ", @pMAJOR_CODE = '" + txtMCode.Text.Trim() + "'";

                    DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                    ERRCode = ds.Tables[0].Rows[0][0].ToString();
                    MSGCode = ds.Tables[0].Rows[0][1].ToString();

                    if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                    Trans.Commit();
                    NewExec();
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    Trans.Rollback();
                    MSGCode = "SY002";	//에러가 발생하여 데이터 처리가 취소되었습니다.
                }
            Exit:
                dbConn.Close();

                if (ERRCode == "OK")
                {
                    SearchExec();
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
        #endregion

        #region Major 코드 저장
        private void btnSave_Click(object sender, System.EventArgs e)
        {
            SystemBase.Validation.Control_SaveCheck(groupBox2);
            if (SystemBase.Base.gstrControl_OrgData == SystemBase.Base.gstrControl_SaveData)
            {
                MessageBox.Show(SystemBase.Base.MessageRtn("SY017"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);//변경되거나 처리 할 자료가 없습니다.
                return;
            }

            string strMajorCd = txtMCode.Text;
            // 그리드 상단 필수항목 체크
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox2))  //필수여부체크
            {
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("SY048"), SystemBase.Base.MessageRtn("Z0004"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dsMsg == DialogResult.Yes)
                {
                    Major_Save();
                    
                    //그리드 재조회
                    SearchExec();

                    string strSql1 = " usp_BBA001  'S2' ";
                    strSql1 = strSql1 + ", @pCOMP_CODE = '"+ SystemBase.Base.gstrCOMCD.ToString() +"' ";
                    strSql1 = strSql1 + ", @pMAJOR_CODE = '" + strMajorCd + "'";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strSql1, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0);
                    Header_arrangement();

                    UIForm.FPMake.GridSetFocus(fpSpread2, strMajorCd, SystemBase.Base.GridHeadIndex(GHIdx2, "코드"));

                    SystemBase.Validation.Control_SearchCheck(groupBox2);
                }
            }
        }
        #endregion

        #region Form Load 시
        private void BBA001_Load(object sender, System.EventArgs e)
        {
            SystemBase.Validation.GroupBox_Reset(groupBox2);
            SystemBase.Validation.GroupBox_Setting(groupBox2);
            SystemBase.Validation.Control_SearchCheck(groupBox2);

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
        }
        #endregion

        #region Major 코드 저장 함수
        private void Major_Save()
        {
            //Major Code 저장
            string strCode = txtMCode.Text;
            string strCodeNm = txtMCodenm.Text;
            string strHead1 = txtHead1.Text;
            string strHead2 = txtHead2.Text;
            string strHead3 = txtHead3.Text;
            string strHead4 = txtHead4.Text;
            string strHead5 = txtHead5.Text;
            string strHead6 = txtHead6.Text;
            string strHead7 = txtHead7.Text;
            string strHead8 = txtHead8.Text;
            string strHead9 = txtHead9.Text;
            string strHead10 = txtHead10.Text;
            string strRemark = txtRemark.Text;

            string ERRCode = "ER", MSGCode = "SY001";	//처리할 내용이 없습니다.
            SqlConnection dbConn = SystemBase.DbOpen.DBCON();
            SqlCommand cmd = dbConn.CreateCommand();
            SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

            try
            {
                string strSql = " usp_BBA001  'I1' ";
                strSql = strSql + ", @pCOMP_CODE = '"+ SystemBase.Base.gstrCOMCD.ToString() +"' ";
                strSql = strSql + ", @pMAJOR_CODE = '" + strCode + "'";
                strSql = strSql + ", @pCODE_NAME    = '" + strCodeNm + "'";
                strSql = strSql + ", @pREL_CD1 = '" + strHead1 + "'";
                strSql = strSql + ", @pREL_CD2 = '" + strHead2 + "'";
                strSql = strSql + ", @pREL_CD3 = '" + strHead3 + "'";
                strSql = strSql + ", @pREL_CD4 = '" + strHead4 + "'";
                strSql = strSql + ", @pREL_CD5 = '" + strHead5 + "'";
                strSql = strSql + ", @pREL_CD6 = '" + strHead6 + "'";
                strSql = strSql + ", @pREL_CD7 = '" + strHead7 + "'";
                strSql = strSql + ", @pREL_CD8 = '" + strHead8 + "'";
                strSql = strSql + ", @pREL_CD9 = '" + strHead9 + "'";
                strSql = strSql + ", @pREL_CD10 = '" + strHead10 + "'";
                strSql = strSql + ", @pREMARK = '" + strRemark + "'";

                DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                ERRCode = ds.Tables[0].Rows[0][0].ToString();
                MSGCode = ds.Tables[0].Rows[0][1].ToString();

                if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                Trans.Commit();
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                Trans.Rollback();
                MSGCode = "SY002";	//에러가 발생하여 데이터 처리가 취소되었습니다.
            }
        Exit:
            dbConn.Close();

            if (ERRCode == "ER")
            {
                MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 디폴트값 체크시 체크확인
        private void fpSpread1_ButtonClicked(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "Default"))
            {
                int intRow = e.Row;
                for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                {
                    if (i != intRow) fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Default")].Value = 0;
                }
            }
        }
        #endregion

        #region 좌측그리드 방향키 이동시 우측조회
        private void fpSpread2_SelectionChanged(object sender, FarPoint.Win.Spread.SelectionChangedEventArgs e)
        {
            string strCode = "";
            if (fpSpread2.Sheets[0].Rows.Count > 0)
            {
                int intRow = fpSpread2.ActiveSheet.GetSelection(0).Row;
                if (intRow < 0) return;
                if (PreRow == intRow && PreRow != -1) return;  //현 Row에서 컬럼이동시는 조회 안되게

                strCode = fpSpread2.Sheets[0].Cells[intRow, 1].Text.ToString();

                //서브스프레드 상위 텍스트 입력
                txtMCode.Value = fpSpread2.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx2, "코드")].Text.ToString();
                txtMCodenm.Value = fpSpread2.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx2, "코드명")].Text.ToString();

                txtHead1.Text = fpSpread2.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx2, "Rel.01")].Text.ToString();
                txtHead2.Text = fpSpread2.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx2, "Rel.02")].Text.ToString();
                txtHead3.Text = fpSpread2.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx2, "Rel.03")].Text.ToString();
                txtHead4.Text = fpSpread2.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx2, "Rel.04")].Text.ToString();
                txtHead5.Text = fpSpread2.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx2, "Rel.05")].Text.ToString();
                txtHead6.Text = fpSpread2.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx2, "Rel.06")].Text.ToString();
                txtHead7.Text = fpSpread2.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx2, "Rel.07")].Text.ToString();
                txtHead8.Text = fpSpread2.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx2, "Rel.08")].Text.ToString();
                txtHead9.Text = fpSpread2.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx2, "Rel.09")].Text.ToString();
                txtHead10.Text = fpSpread2.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx2, "Rel.10")].Text.ToString();
                txtRemark.Text = fpSpread2.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx2, "비고")].Text.ToString();

                //조회문.
                string strSql = " usp_BBA001  'S2' ";
                strSql = strSql + ", @pCOMP_CODE = '"+ SystemBase.Base.gstrCOMCD.ToString() +"' ";
                strSql = strSql + ", @pMAJOR_CODE = '" + strCode + "'";

                UIForm.FPMake.grdCommSheet(fpSpread1, strSql, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);

                //헤더값 재정리
                Header_arrangement();

                SystemBase.Validation.GroupBox_SearchViewValidation(groupBox2);  //컨트롤 Key값 처리
                SystemBase.Validation.Control_SearchCheck(groupBox2);

                PreRow = fpSpread2.ActiveSheet.GetSelection(0).Row;
            }
            else
            { txtMCode.Text = ""; txtMCodenm.Text = ""; }
        }

        private void Right_Focus(string strScode)
        {
            for (int i = 0; i < fpSpread2.ActiveSheet.Rows.Count; i++)
            {
                string strItemCd = fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "코드")].Text.ToString();
                if (strScode == strItemCd)
                {
                    fpSpread2.ActiveSheet.AddSelection(i, 0, 1, 4);
                }
            }
        }
        #endregion 

        #region 헤더값 정리
        private void Header_arrangement()
        {
            if (txtHead1.Text != "")
            {
                fpSpread1.Sheets[0].ColumnHeader.Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "Rel.01")].Text = txtHead1.Text;
            }
            else
            {
                fpSpread1.Sheets[0].ColumnHeader.Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "Rel.01")].Text = "Rel.01";
            }

            if (txtHead2.Text != "")
            {
                fpSpread1.Sheets[0].ColumnHeader.Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "Rel.02")].Text = txtHead2.Text;
            }
            else
            {
                fpSpread1.Sheets[0].ColumnHeader.Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "Rel.02")].Text = "Rel.02";
            }

            if (txtHead3.Text != "")
            {
                fpSpread1.Sheets[0].ColumnHeader.Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "Rel.03")].Text = txtHead3.Text;
            }
            else
            {
                fpSpread1.Sheets[0].ColumnHeader.Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "Rel.03")].Text = "Rel.03";
            }

            if (txtHead4.Text != "")
            {
                fpSpread1.Sheets[0].ColumnHeader.Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "Rel.04")].Text = txtHead4.Text;
            }
            else
            {
                fpSpread1.Sheets[0].ColumnHeader.Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "Rel.04")].Text = "Rel.04";
            }

            if (txtHead5.Text != "")
            {
                fpSpread1.Sheets[0].ColumnHeader.Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "Rel.05")].Text = txtHead5.Text;
            }
            else
            {
                fpSpread1.Sheets[0].ColumnHeader.Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "Rel.05")].Text = "Rel.05";
            }

            if (txtHead6.Text != "")
            {
                fpSpread1.Sheets[0].ColumnHeader.Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "Rel.06")].Text = txtHead6.Text;
            }
            else
            {
                fpSpread1.Sheets[0].ColumnHeader.Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "Rel.06")].Text = "Rel.06";
            }

            if (txtHead7.Text != "")
            {
                fpSpread1.Sheets[0].ColumnHeader.Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "Rel.07")].Text = txtHead7.Text;
            }
            else
            {
                fpSpread1.Sheets[0].ColumnHeader.Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "Rel.07")].Text = "Rel.07";
            }

            if (txtHead8.Text != "")
            {
                fpSpread1.Sheets[0].ColumnHeader.Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "Rel.08")].Text = txtHead8.Text;
            }
            else
            {
                fpSpread1.Sheets[0].ColumnHeader.Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "Rel.08")].Text = "Rel.08";
            }

            if (txtHead9.Text != "")
            {
                fpSpread1.Sheets[0].ColumnHeader.Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "Rel.09")].Text = txtHead9.Text;
            }
            else
            {
                fpSpread1.Sheets[0].ColumnHeader.Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "Rel.09")].Text = "Rel.09";
            }

            if (txtHead10.Text != "")
            {
                fpSpread1.Sheets[0].ColumnHeader.Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "Rel.10")].Text = txtHead10.Text;
            }
            else
            {
                fpSpread1.Sheets[0].ColumnHeader.Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "Rel.10")].Text = "Rel.10";
            }
        }
        #endregion

    }
}