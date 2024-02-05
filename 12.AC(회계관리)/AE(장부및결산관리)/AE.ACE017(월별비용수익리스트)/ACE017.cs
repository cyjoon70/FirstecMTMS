

#region 작성정보
/*********************************************************************/
// 단위업무명 : 재무상태표조회
// 작 성 자 : 유 재 규
// 작 성 일 : 2013-03-07
// 작성내용 : 재무상태표조회
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

namespace AE.ACE017
{
    public partial class ACE017 : UIForm.FPCOMM1 
    {
        public ACE017()
        {
            InitializeComponent();
        }

        #region Form Load 시
        private void ACE017_Load(object sender, System.EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(groupBox1);//필수 적용
            SystemBase.ComboMake.C1Combo(cboBizAreaCd, "usp_B_COMMON @pTYPE ='BIZ', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9);      //사업장
            dtpYYYY.Text = SystemBase.Base.ServerTime("YYMMDD").Substring(0, 4);
            
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            dtpYYYY.Value = SystemBase.Base.ServerTime("YYMMDD").Substring(0, 4);

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
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
                    string strQuery = " usp_ACE017 ";
                    strQuery += " @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    strQuery += ", @pYYYY = '" + dtpYYYY.Text + "' ";
                    strQuery += ", @pTYPE_CD = '" + txtTypeCd.Text + "' ";
                    strQuery += ", @pBIZ_AREA_CD = '" + cboBizAreaCd.SelectedValue.ToString() + "' ";
                    
                    DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);
                    if (dt.Rows.Count > 0)
                    {
                        if (dt.Rows[0][0].ToString() == "ER")
                        {
                            MessageBox.Show(dt.Rows[0][1].ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                            fpSpread1.Sheets[0].Rows.Count = 0;
                        }
                        else
                        {
                            UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0, true);
                        }
                    }
                    else
                    {
                        //MessageBox.Show("관리자에게 문의하세요(MS-SQL Qury 에러)", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                        fpSpread1.Sheets[0].Rows.Count = 0;
                    }
                    
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(SystemBase.Base.MessageRtn(f.ToString()), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            this.Cursor = Cursors.Default;
        }
        #endregion

        #region 팝업
        //월별명세서유형
        private void btnTypeCd_Click(object sender, EventArgs e)
        {
            try
            {
                try
                {
                    string strQuery = " usp_B_COMMON @pType='COMM_POP', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pSPEC1 = 'A120', @pSPEC2 = 'MM' ";
                    string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                    string[] strSearch = new string[] { txtTypeCd.Text, "" };
                    UIForm.FPPOPUP pu = new UIForm.FPPOPUP("ACD001_P7", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "재무제표코드 조회");
                    pu.Width = 800;
                    pu.Height = 800;
                    pu.ShowDialog();
                    if (pu.DialogResult == DialogResult.OK)
                    {
                        Regex rx1 = new Regex("#");
                        string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                        txtTypeCd.Value = Msgs[0].ToString();
                        txtTypeNm.Value = Msgs[1].ToString();
                        txtTypeNm.Focus();
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "재무제표코드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region TextChanged
        //월별명세서유형
        private void txtTypeCd_TextChanged(object sender, EventArgs e)
        {
            try
            {
                txtTypeNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtTypeCd.Text, " AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND REL_CD1 = 'MM' AND COMP_CODE = '" + SystemBase.Base.gstrCOMCD + "'");
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

    }
}
