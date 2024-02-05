#region 작성정보
/*********************************************************************/
// 단위업무명 : 입고대비매입실적
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-04-18
// 작성내용 : 입고대비매입실적 관리
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
using WNDW;
using FarPoint.Win.Spread.CellType;

namespace MV.MIV504
{
    public partial class MIV504 : UIForm.FPCOMM1
    {
        #region 변수선언
        bool form_act_chk = false;
        #endregion

        #region 생성자
        public MIV504()
        {
            InitializeComponent();
        }
        #endregion 

        #region Form Load 시
        private void MIV504_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            SystemBase.ComboMake.C1Combo(cboPlantCd, "usp_B_COMMON @pType='B031', @pSPEC1 = '" + SystemBase.Base.gstrBIZCD + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 9);//공장

            //기타 세팅
            dtpMvmtDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString().Substring(0,10);
            dtpMvmtDtTo.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).ToShortDateString().Substring(0,10);

            rdoCfmAll.Checked = true;
        }
        #endregion
        
        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            fpSpread1.Sheets[0].Rows.Count = 0;

            //기타 세팅
            dtpMvmtDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString().Substring(0,10);
            dtpMvmtDtTo.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).ToShortDateString().Substring(0,10);

            rdoCfmAll.Checked = true;
        }
        #endregion

        #region SearchExec()  그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {
                    string strCfmYn = "";
                    if (rdoCfmYes.Checked == true) { strCfmYn = "Y"; }
                    else if (rdoCfmNo.Checked == true) { strCfmYn = "N"; }

                    string strQuery = " usp_MIV504 'S1'";
                    strQuery += ", @pIO_TYPE ='" + txtIoType.Text.Trim() + "'";
                    strQuery += ", @pIV_TYPE   ='" + txtIvType.Text.Trim() + "'";
                    strQuery += ", @pCUST_CD ='" + txtCust.Text.Trim() + "'";
                    strQuery += ", @pPO_NO ='" + txtPoNo.Text.Trim() + "'";
                    strQuery += ", @pMVMT_NO ='" + txtMvmtNo.Text.Trim() + "'";
                    strQuery += ", @pPLANT_CD  ='" + cboPlantCd.SelectedValue.ToString() + "'";
                    strQuery += ", @pITEM_CD  ='" + txtITEM_CD.Text.Trim() + "'";
                    strQuery += ", @pCONFIRM_YN ='" + strCfmYn + "'";
                    strQuery += ", @pMVMT_DT_FR  ='" + dtpMvmtDtFr.Text + "'";
                    strQuery += ", @pMVMT_DT_TO  ='" + dtpMvmtDtTo.Text + "'";
                    strQuery += ", @pITEM_TYPE = '" + txtItemType.Text + "'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0, true);
                    fpSpread1.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.Normal;

                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }

            this.Cursor = Cursors.Default;
        }
        #endregion

        #region 버튼 Click
        private void btnIoType_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_M_COMMON 'M020', @pSPEC1 = '' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtIoType.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P01001", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "입고형태 팝업");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {

                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtIoType.Value = Msgs[0].ToString();
                    txtIoTypeNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }


        private void btnIvType_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = "usp_B_COMMON @pTYPE = 'TABLE_POP', @pSPEC1 = 'IV_TYPE', @pSPEC2 = 'IV_TYPE_NM', @pSPEC3 = 'M_IV_TYPE', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtIvType.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00006", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "매입형태 조회");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtIvType.Value = Msgs[0].ToString();
                    txtIvTypeNm.Value = Msgs[1].ToString();

                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }


        private void btnCust_Click(object sender, System.EventArgs e)
        {

            try
            {
                WNDW002 pu = new WNDW002(txtCust.Text, "P");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtCust.Value = Msgs[1].ToString();
                    txtCustNm.Value = Msgs[2].ToString();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }

        }

        private void btnITEM_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW001 pu = new WNDW001(txtITEM_CD.Text, "");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtITEM_CD.Value = Msgs[1].ToString();
                    txtITEM_NM.Value = Msgs[2].ToString();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        //입고번호
        private void btnMvmtNo_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW.WNDW019 pu = new WNDW.WNDW019();
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtMvmtNo.Text = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "구매입고정보 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnPoNo_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW.WNDW018 pu = new WNDW.WNDW018();
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtPoNo.Text = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "구매발주정보 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnItemType_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON  @pTYPE = 'COMM_POP' ,@pSPEC1='P032', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtItemType.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00077", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "품목구분 팝업");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {

                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtItemType.Value = Msgs[0].ToString();
                    txtItemTypeNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목구분 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region TextChanged
        private void txtCust_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtCust.Text != "")
                {
                    txtCustNm.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtCust.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtCustNm.Value = "";
                }
            }
            catch
            {

            }
        }

        private void txtITEM_CD_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtITEM_CD.Text != "")
                {
                    txtITEM_NM.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtITEM_CD.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtITEM_NM.Value = "";
                }
            }
            catch
            {

            }
        }

        private void txtIoType_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtIoType.Text != "")
                {
                    txtIoTypeNm.Value = SystemBase.Base.CodeName("IO_TYPE", "IO_TYPE_NM", "M_MVMT_TYPE", txtIoType.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtIoTypeNm.Value = "";
                }
            }
            catch
            {

            }
        }

        private void txtIvType_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtIvType.Text != "")
                {
                    txtIvTypeNm.Value = SystemBase.Base.CodeName("IV_TYPE", "IV_TYPE_NM", "M_IV_TYPE", txtIvType.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtIvTypeNm.Value = "";
                }
            }
            catch
            {

            }
        }

        private void txtItemType_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtItemType.Text != "")
                {
                    txtItemTypeNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtItemType.Text, " AND MAJOR_CD = 'P032' AND COMP_CODE = '" + SystemBase.Base.gstrCOMCD + "' ");
                }
                else
                {
                    txtItemTypeNm.Value = "";
                }
            }
            catch
            {

            }
        }
        #endregion

        #region 폼 Activated & Deactivate
        private void MIV504_Activated(object sender, System.EventArgs e)
        {
            if (form_act_chk == false) txtIoType.Focus();
        }

        private void MIV504_Deactivate(object sender, System.EventArgs e)
        {
            form_act_chk = true;
        }
        #endregion
    }
}
