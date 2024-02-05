﻿#region 작성정보
/*********************************************************************/
// 단위업무명 : 검사항목기준정보
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-02-19
// 작성내용 : 검사항목기준정보 및 관리
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

namespace QB.QBA002
{
    public partial class QBA002 : UIForm.FPCOMM1
    {
        #region 생성자
        public QBA002()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void QBA002_Load(object sender, System.EventArgs e)
        {
            //필수 항목 체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            SystemBase.ComboMake.C1Combo(cboInspItemClassCd, "usp_B_COMMON @pType='COMM', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "',@pCODE = 'Q006', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 3); //검사항목분류

            //그리드 콤보박스 세팅			
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "검사항목분류")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'Q006', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0); //검사항목분류 
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "검사항목속성")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'Q007', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0); //검사항목속성
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "검사항목구분")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'Q028', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0); //검사항목구분

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

        #region SearchExec()
        protected override void SearchExec()
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {
                string strQuery = " usp_QBA002 @pTYPE = 'S1'";
                strQuery += ", @pINSP_ITEM_CLASS_CD = '" + cboInspItemClassCd.SelectedValue + "' ";
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
            //그리드상단 필수 체크
            if (SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", true))
            {
                this.Cursor = Cursors.WaitCursor;

                string ERRCode = "ER", MSGCode = "P0000"; //처리할 내용이 없습니다.
                string strINSP_ITEM_CD = "";
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
                            strINSP_ITEM_CD = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사항목코드")].Text;
                            string strQuery = " usp_QBA002 @pTYPE = '" + strGbn + "'";
                            strQuery += ", @pINSP_ITEM_CD = '" + strINSP_ITEM_CD + "'";
                            strQuery += ", @pINSP_ITEM_NM = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사항목명")].Text + "'";
                            strQuery += ", @pINSP_ITEM_CLASS_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사항목분류")].Value + "'";
                            strQuery += ", @pINSP_ITEM_CHAR = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사항목속성")].Value + "'";
                            strQuery += ", @pINSP_ITEM_TYPE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사항목구분")].Value + "'";
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
                    UIForm.FPMake.GridSetFocus(fpSpread1, strINSP_ITEM_CD, SystemBase.Base.GridHeadIndex(GHIdx1, "검사항목코드"));
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
	
    }
}
