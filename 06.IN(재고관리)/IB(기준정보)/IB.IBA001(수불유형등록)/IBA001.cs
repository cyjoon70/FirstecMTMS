#region 작성정보
/*********************************************************************/
// 단위업무명 : 수불유형등록
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-02-18
// 작성내용 : 수불유형등록 및 관리
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

namespace IB.IBA001
{
    public partial class IBA001 : UIForm.FPCOMM1
    {
        public IBA001()
        {
            InitializeComponent();
        }

        #region Form Load 시
        private void IBA001_Load(object sender, System.EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            //그리드 콤보박스 세팅			
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "증감구분")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'Z015', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ", 0);//증감구분
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "양불구분(공통)")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'Q002', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ", 0);//양불구분
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "양불구분(이동)")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'Q002', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ");//양불구분
            
            //그리드 초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
        }
        #endregion

        #region 행추가 버튼 클릭 이벤트
        protected override void RowInsExec()
        {
            UIForm.FPMake.RowInsert(fpSpread1);

            fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "사용")].Text = "True";
        }
        #endregion

        #region 수불구분 팝업
        private void btnTranType_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = "usp_B_COMMON @pTYPE = 'COMM_POP', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pSPEC1 = 'I001', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtTranType.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00053", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "수불구분 조회");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtTranType.Text = Msgs[0].ToString();
                    txtTranTypeNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }
        #endregion

        #region 양불구분(이동) grdReMake
        protected void InvType2(int Row)
        {
            if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "수불구분")].Text == "ST")
            {
                UIForm.FPMake.grdReMake(fpSpread1, Row, SystemBase.Base.GridHeadIndex(GHIdx1, "양불구분(이동)") + "|2");
            }
            else
            {
                UIForm.FPMake.grdReMake(fpSpread1, Row, SystemBase.Base.GridHeadIndex(GHIdx1, "양불구분(이동)") + "|4");
            }
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {
                string strQuery = "usp_IBA001 @pTYPE = 'S1'";
                strQuery += ", @pTRAN_TYPE = '" + txtTranType.Text + "'";
                strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 2, false, true);

                for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                {
                    InvType2(i);
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
            if (SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", true))
            {
                string ERRCode = "ER", MSGCode = "P0000"; //처리할 내용이 없습니다.
                string strMOVE_TYPE = "";
                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                this.Cursor = Cursors.WaitCursor;

                try
                {
                    for (int i = 0; i < fpSpread1.ActiveSheet.Rows.Count; i++)
                    {
                        string strHead = fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text;
                        string strGbn = "";

                        if (strHead.Length > 0)
                        {
                            switch (strHead)
                            {
                                case "I": strGbn = "I1"; break;
                                case "D": strGbn = "D1"; break;
                                case "U": strGbn = "U1"; break;
                                default: strGbn = ""; break;
                            }

                            string strPostingYn = "N";
                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "전표발생")].Text == "True") { strPostingYn = "Y"; }
                            string strUseYn = "N";
                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "사용")].Text == "True") { strUseYn = "Y"; }

                            strMOVE_TYPE = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수불유형")].Text;
                            
                            string strSql = "usp_IBA001 '" + strGbn + "'";
                            strSql += ", @pMOVE_TYPE = '" + strMOVE_TYPE + "'";
                            strSql += ", @pMOVE_TYPE_NM = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수불유형명")].Text + "'";
                            strSql += ", @pTRAN_TYPE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수불구분")].Text + "'";
                            strSql += ", @pINV_DCR_FLAG = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "증감구분")].Value + "'";
                            strSql += ", @pINV_TYPE1 = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "양불구분(공통)")].Value + "'";
                            strSql += ", @pINV_TYPE2 = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "양불구분(이동)")].Value + "'";
                            strSql += ", @pPOSTING_YN = '" + strPostingYn + "'";
                            strSql += ", @pUSE_YN = '" + strUseYn + "'";
                            strSql += ", @pREMARK = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "비고")].Text + "'";
                            strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                            strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                            DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                            ERRCode = ds.Tables[0].Rows[0][0].ToString();
                            MSGCode = ds.Tables[0].Rows[0][1].ToString();

                            if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	//ER 코드 Return시 점프
                        }
                    }
                    Trans.Commit();
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    Trans.Rollback();
                    MSGCode = "P0001"; //에러가 발생되어 데이터 처리가 취소 되었습니다.
                }
            Exit:
                dbConn.Close();

                if (ERRCode == "OK")
                {
                    SearchExec();
                    UIForm.FPMake.GridSetFocus(fpSpread1, strMOVE_TYPE, SystemBase.Base.GridHeadIndex(GHIdx1, "수불유형"));
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

        #region 그리드 버튼 클릭
        private void fpSpread1_ButtonClicked(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "수불구분_2"))
            {
                try
                {
                    string strQuery = " usp_B_COMMON 'COMM_POP' ,@pLANG_CD='" + SystemBase.Base.gstrLangCd + "' ," + "@pSPEC1='I001', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                    string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                    string[] strSearch = new string[] { "", "" };


                    UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00053", strQuery, strWhere, strSearch, new int[] { 0, 1 });	//수불구분코드 사용자조회
                    pu.ShowDialog();

                    if (pu.DialogResult == DialogResult.OK)
                    {
                        Regex rx1 = new Regex("#");
                        string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "수불구분")].Text = Msgs[0].ToString();
                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "수불구분명")].Text = Msgs[1].ToString();

                        if (fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "수불구분")].Text == "ST")
                        {
                            UIForm.FPMake.grdReMake(fpSpread1, e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "양불구분(이동)") + "|1");
                            fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "증감구분")].Value = "-";
                            UIForm.FPMake.grdReMake(fpSpread1, e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "증감구분") + "|3");
                        }
                        else
                        {
                            UIForm.FPMake.grdReMake(fpSpread1, e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "양불구분(이동)") + "|0");
                            fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "증감구분")].Value = "";
                            UIForm.FPMake.grdReMake(fpSpread1, e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "증감구분") + "|1");
                        }
                    }
                }
                catch (Exception f)
                {

                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
                }
            }
        }
        #endregion

        #region fpSpread1_Change
        private void fpSpread1_Change(object sender, FarPoint.Win.Spread.ChangeEventArgs e)
        {

            if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "수불구분"))
            {
                fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "수불구분명")].Text
                    = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "수불구분")].Text, " AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND MAJOR_CD = 'I001' AND COMP_CODE = '" + SystemBase.Base.gstrCOMCD + "'");

                InvType2(e.Row);
            }

        }
        #endregion

        #region 조회조건 TextChanged
        private void txtTranType_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtTranType.Text != "")
                {
                    txtTranTypeNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtTranType.Text, " AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND MAJOR_CD = 'I001' AND COMP_CODE = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtTranTypeNm.Value = "";
                }
            }
            catch
            {

            }
        }
        #endregion

    }
}
