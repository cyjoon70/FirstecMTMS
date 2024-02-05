#region 작성정보
/*********************************************************************/
// 단위업무명 : 품목별검사기준정보
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-02-19
// 작성내용 : 품목별검사기준정보 관리
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

namespace QB.QBA011
{
    public partial class QBA011 : UIForm.FPCOMM2
    {
        #region 변수선언
        string strInspSeq = "";
        string strFinInspLvl = "";
        string[] strCode = new string[7];
        int ShowRow = 0;	//마스터 스프레드에 보여줄 로우
        int SearchRow = 0;
        int ShowColumn = 0;
        int NewFlg = 1;//마스터 그리드 조회여부 0:0포커스조회, 1:로우조회, 2:수정
        string FullFileName = "";
        Thread th;
        Thread thread;
        UIForm.ExcelWaiting Waiting_Form = null;

        bool Linked = false;

        string strItemCd = "";
        string strInspClass = "";
        string strPlantCd = "";
        #endregion

        #region 생성자
        public QBA011()
        {
            InitializeComponent();
        }
        public QBA011(string param1, string param2, string param3)
        {
            strItemCd = param1;
            strPlantCd = param2;
            strInspClass = param3;
            Linked = true;
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void QBA011_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            SystemBase.Validation.GroupBox_Setting(groupBox2);

            //콤보박스 세팅
            //groupBox1
            SystemBase.ComboMake.C1Combo(cboSPlantCd, "usp_B_COMMON @pType='TABLE', @pCODE = 'PLANT_CD', @pNAME = 'PLANT_NM', @pSPEC1 = 'B_PLANT_INFO', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//공장
            SystemBase.ComboMake.C1Combo(cboSInspClassCd, "usp_B_COMMON @pType='COMM', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "',@pCODE = 'Q001', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0); //검사분류코드
            SystemBase.ComboMake.C1Combo(cboSItemAcct, "usp_B_COMMON @pTYPE = 'COMM', @pCODE = 'B036', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 9);	//품목계정
            //groupBox2
            SystemBase.ComboMake.C1Combo(cboPlantCd, "usp_B_COMMON @pType='TABLE', @pCODE = 'PLANT_CD', @pNAME = 'PLANT_NM', @pSPEC1 = 'B_PLANT_INFO', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//공장
            SystemBase.ComboMake.C1Combo(cboInspClassCd, "usp_B_COMMON @pType='COMM', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "',@pCODE = 'Q001', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0); //검사분류코드

            //DETAIL
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "품질표시")] = SystemBase.ComboMake.ComboOnGrid("usp_QBA011 @pType='COMM', @pCODE = 'Q010', @pCFM ='', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//품질표시
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "중요도")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'Q011', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 1);//중요도
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "관리한계산출방법")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'Q012', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 1);//관리한계산출방법

            //그리드초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
            UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, 0, 0, false);

            // 2015.06.03. hma 추가: 기준일자 세팅 
            dtpBaseDt.Value = SystemBase.Base.ServerTime("YYMMDD");

            //기타 세팅
            cboSPlantCd.SelectedValue = SystemBase.Base.gstrPLANT_CD;
            cboPlantCd.SelectedValue = SystemBase.Base.gstrPLANT_CD;
            btnRevision.Enabled = false;
            btnInspBase.Enabled = false;
            btnInspResult.Enabled = false;
            btnITEM_PICTURE.Enabled = false;
            if (Linked == true)
            {
                cboSPlantCd.SelectedValue = strPlantCd;
                cboSInspClassCd.SelectedValue = strInspClass;
                txtSItemCd.Text = strItemCd;
                SearchExec();
            }

            lnkJump1.Text = "선별형검사조건정보";  //화면에 보여지는 링크명
            strJumpFileName1 = "QB.QBA014.QBA014"; //호출할 화면명			

            lnkJump2.Text = "조정형검사조건정보";  //화면에 보여지는 링크명
            strJumpFileName2 = "QB.QBA013.QBA013"; //호출할 화면명

            lnkJump3.Text = "일반검사조건정보";  //화면에 보여지는 링크명
            strJumpFileName3 = "QB.QBA012.QBA012"; //호출할 화면명		
        }
        #endregion
        
        #region Link
        private object[] Params()
        {
            if (txtItemCd.Text == "")
                param = null;						// 파라메터를 하나도 넘기지 않을경우
            else
            {
                param = new object[3];					// 파라메터수가 3개인 경우
                param[0] = txtItemCd.Text;
                param[1] = cboPlantCd.SelectedValue.ToString();
                param[2] = cboInspClassCd.SelectedValue.ToString();
            }
            return param;
        }

        protected override void Link1Exec()
        {
            param = Params();

            SystemBase.Base.RodeFormID = "QBA014";			// 이동할 formid를 적어준다.(메뉴id)
            SystemBase.Base.RodeFormText = "선별형검사조건정보"; 	// 이동할 폼명을 적어준다(메뉴명)			
        }

        protected override void Link2Exec()
        {
            param = Params();

            SystemBase.Base.RodeFormID = "QBA013";			// 이동할 formid를 적어준다.(메뉴id)
            SystemBase.Base.RodeFormText = "조정형검사조건정보"; 	// 이동할 폼명을 적어준다(메뉴명)
        }

        protected override void Link3Exec()
        {
            param = Params();

            SystemBase.Base.RodeFormID = "QBA012";			// 이동할 formid를 적어준다.(메뉴id)
            SystemBase.Base.RodeFormText = "일반검사조건정보"; 	// 이동할 폼명을 적어준다(메뉴명)
        }
        #endregion

        #region NewExec()
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            SystemBase.Validation.GroupBox_Reset(groupBox2);

            SystemBase.Validation.GroupBoxControlsLock(groupBox2, false);

            //그리드초기화
            fpSpread1.Sheets[0].Rows.Count = 0;
            fpSpread2.Sheets[0].Rows.Count = 0;
            //기타 세팅
            InspClassCd();

            if (fpSpread2.Sheets[0].Rows.Count > 0) { NewFlg = 2; }
            else { NewFlg = 1; }
        }
        #endregion

        #region groupBox2 팝업
        //품목코드 팝업
        private void btnItemCd_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW005 pu = new WNDW005(cboPlantCd.SelectedValue.ToString(), true, txtItemCd.Text);
                pu.MaximizeBox = false;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtItemCd.Value = Msgs[2].ToString();
                    txtItemNm.Value = Msgs[3].ToString();
                    strFinInspLvl = Msgs[11].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목코드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        //최종검사레벨
        private void btnFinInspLvl_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = "";

                if (strFinInspLvl == "N")
                    strQuery = "usp_Q_COMMON @pType='Q020', @pSPEC1 = 'Q013', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pSPEC2 = '9', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                else
                    strQuery = "usp_Q_COMMON @pType='Q020', @pSPEC1 = 'Q013', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pSPEC2 = '" + strFinInspLvl + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtFinInspLvl.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P06004", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "최종검사레벨 조회");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtFinInspLvl.Value = Msgs[0].ToString();
                    txtFinInspLvlNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "최종검사레벨 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        //라우팅번호
        private void btnRoutNo_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = "usp_Q_COMMON @pType='Q030', @pSPEC1 = '" + cboPlantCd.SelectedValue.ToString() + "', @pSPEC2 = '" + txtItemCd.Text + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
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

        //공정순서
        private void btnProcSeq_Click(object sender, System.EventArgs e)
        {
            try
            {
                //라우팅 유효성 체크
                if (txtRoutNo.Text == "")
                {
                    //존재하지 않는 품목 코드입니다
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0036", "라우팅"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtRoutNo.Focus();

                    return;
                }

                string strQuery = "usp_Q_COMMON @pType='Q040', @pSPEC1 = '" + cboPlantCd.SelectedValue.ToString() + "', @pSPEC2 = '" + txtItemCd.Text + "', @pSPEC3 = '" + txtRoutNo.Text + "', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtProcSeq.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P06006", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "공정순서 조회");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtProcSeq.Value = Msgs[0].ToString();
                    txtProcSeqNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "공정순서 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }
        #endregion

        #region groupBox2 TextChanged
        //품목코드
        private void txtItemCd_TextChanged(object sender, System.EventArgs e)
        {
            string Query = " SELECT B.ITEM_NM, FINAL_INSP_FLAG ";
            Query += " FROM B_PLANT_ITEM_INFO A(NOLOCK) LEFT JOIN B_ITEM_INFO B(NOLOCK) ";
            Query += " ON A.ITEM_CD = B.ITEM_CD ";
            Query += " WHERE A.PLANT_CD = '" + cboPlantCd.SelectedValue.ToString() + "'";
            Query += " AND A.ITEM_CD = '" + txtItemCd.Text + "'";

            DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);

            if (dt.Rows.Count > 0)
            {
                txtItemNm.Value = dt.Rows[0][0].ToString();
                strFinInspLvl = dt.Rows[0][1].ToString();
                btnRevision.Enabled = true;
            }
            else
            {
                txtItemNm.Value = "";
                btnRevision.Enabled = false;
            }

            InspClassCd();
        }

        //최종검사레벨
        private void txtFinInspLvl_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (strFinInspLvl == "N")
                {
                    if (txtFinInspLvl.Text != "")
                    {
                        txtFinInspLvlNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtFinInspLvl.Text, "  AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND MAJOR_CD = 'Q013' AND MINOR_CD <= '9' AND REL_CD1 = 'USE' AND COMP_CODE ='" + SystemBase.Base.gstrCOMCD + "'");
                    }
                    else
                    {
                        txtFinInspLvlNm.Value = "";
                    }
                }
                else
                {
                    if (txtFinInspLvl.Text != "")
                    {
                        txtFinInspLvlNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtFinInspLvl.Text, "  AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND MAJOR_CD = 'Q013' AND MINOR_CD <= '" + strFinInspLvl + "' AND REL_CD1 = 'USE' AND COMP_CODE ='" + SystemBase.Base.gstrCOMCD + "'");
                    }
                    else
                    {
                        txtFinInspLvlNm.Value = "";
                    }
                }
            }
            catch
            {

            }
        }

        //라우팅번호
        private void txtRoutNo_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtRoutNo.Text != "")
                {
                    txtRoutNm.Value = SystemBase.Base.CodeName("ROUT_NO", "DESCRIPTION", "P_BOP_PROC_MASTER", txtRoutNo.Text, " AND PLANT_CD = '" + cboPlantCd.SelectedValue.ToString() + "' AND ITEM_CD = '" + txtItemCd.Text + "' AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
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

        //공정순번
        private void txtProcSeq_TextChanged(object sender, System.EventArgs e)
        {
            string Query = "SELECT ISNULL(B.CD_NM,'')";
            Query += " FROM P_BOP_PROC_DETAIL A(NOLOCK) LEFT JOIN B_COMM_CODE B(NOLOCK) ";
            Query += " ON A.JOB_CD = B.MINOR_CD";
            Query += " AND B.MAJOR_CD = 'P001'";
            Query += " AND B.LANG_CD = '" + SystemBase.Base.gstrLangCd + "'";
            Query += " AND B.COMP_CODE = '" + SystemBase.Base.gstrCOMCD + "'";
            Query += " WHERE A.PLANT_CD = '" + cboPlantCd.SelectedValue.ToString() + "'";
            Query += " AND A.ITEM_CD  = '" + txtItemCd.Text + "'";
            Query += " AND A.ROUT_NO  = '" + txtRoutNo.Text + "'";
            Query += " AND A.PROC_SEQ = '" + txtProcSeq.Text + "'";

            DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);

            if (dt.Rows.Count > 0)
                txtProcSeqNm.Value = dt.Rows[0][0].ToString();
            else
                txtProcSeqNm.Value = "";
        }
        #endregion

        #region 검사분류코드 변경시 발생하는 이벤트
        private void InspClassCd()
        {
            txtFinInspLvl.Value = "";
            txtFinInspLvlNm.Value = "";
            txtRoutNo.Value = "";
            txtProcSeq.Value = "";

            if (txtItemNm.Text != "" && cboInspClassCd.SelectedValue.ToString() == "F")
            {
                txtRoutNo.BackColor = SystemBase.Validation.Kind_Gainsboro;
                txtRoutNo.ReadOnly = true;
                btnRoutNo.Enabled = false;
                txtRoutNo.Tag = ";2;;";

                txtProcSeq.BackColor = SystemBase.Validation.Kind_Gainsboro;
                txtProcSeq.ReadOnly = true;
                btnProcSeq.Enabled = false;
                txtProcSeq.Tag = ";2;;";

                txtFinInspLvl.BackColor = SystemBase.Validation.Kind_LightCyan;
                txtFinInspLvl.ReadOnly = false;
                btnFinInspLvl.Enabled = true;
                txtFinInspLvl.Tag = "최종검사레벨;1;;";

            }
            else if (txtItemNm.Text != "" && cboInspClassCd.SelectedValue.ToString() == "P")
            {
                txtFinInspLvl.BackColor = SystemBase.Validation.Kind_Gainsboro;
                txtFinInspLvl.ReadOnly = true;
                btnFinInspLvl.Enabled = false;
                txtFinInspLvl.Tag = ";2;;";

                txtRoutNo.BackColor = SystemBase.Validation.Kind_LightCyan;
                txtRoutNo.ReadOnly = false;
                btnRoutNo.Enabled = true;
                txtRoutNo.Tag = "라우팅번호;1;;";

                txtProcSeq.BackColor = SystemBase.Validation.Kind_LightCyan;
                txtProcSeq.ReadOnly = false;
                btnProcSeq.Enabled = true;
                txtProcSeq.Tag = "공정순번;1;;";

            }
            else
            {
                txtFinInspLvl.BackColor = SystemBase.Validation.Kind_Gainsboro;
                txtFinInspLvl.ReadOnly = true;
                btnFinInspLvl.Enabled = false;
                txtFinInspLvl.Tag = ";2;;";

                txtRoutNo.BackColor = SystemBase.Validation.Kind_Gainsboro;
                txtRoutNo.ReadOnly = true;
                btnRoutNo.Enabled = false;
                txtRoutNo.Tag = ";2;;";

                txtProcSeq.BackColor = SystemBase.Validation.Kind_Gainsboro;
                txtProcSeq.ReadOnly = true;
                btnProcSeq.Enabled = false;
                txtProcSeq.Tag = ";2;;";
            }
        }
        #endregion

        #region 행추가 버튼 클릭 이벤트
        protected override void RowInsExec()
        {

            try
            {
                int iValue = 0, ibig = 0;

                if (fpSpread1.Sheets[0].Rows.Count > 0)
                {
                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {
                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사순서")].Text == "")
                        { iValue = 0; }
                        else { iValue = Convert.ToInt32(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사순서")].Value); }

                        if (ibig < iValue)
                        { ibig = iValue; }
                    }
                }

                UIForm.FPMake.RowInsert(fpSpread1);
                fpSpread1.Sheets[0].Rows[fpSpread1.Sheets[0].ActiveRowIndex].Height = 30;

                fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "검사순서")].Value = ibig + 1;
                fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "관리한계계산수")].Value = 0;//0

                fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "시작유효일")].Value = SystemBase.Base.ServerTime("YYMMDD");     // 2015.06.02. hma 추가
                fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "종료유효일")].Value = "2999-12-31";     // 2015.06.02. hma 추가

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "행추가"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        #endregion

        #region 행복사 버튼 클릭 이벤트
        protected override void RCopyExec()
        {
            int iValue = 0, ibig = 0;

            try
            {
                if (fpSpread1.Sheets[0].Rows.Count > 0)
                {
                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {
                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사순서")].Text == "")
                        { iValue = 0; }
                        else { iValue = Convert.ToInt32(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사순서")].Value); }

                        if (ibig < iValue)
                        { ibig = iValue; }
                    }

                    UIForm.FPMake.RowCopy(fpSpread1);
                    fpSpread1.Sheets[0].Rows[fpSpread1.Sheets[0].ActiveRowIndex].Height = 30;
                    int iRow = fpSpread1.Sheets[0].ActiveRowIndex;
                    fpSpread1.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx1, "검사순서")].Value = ibig + 1;
                    Insp_Q(iRow);

                }
                else
                {
                    MessageBox.Show("복사할 데이타가 없습니다.", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "행복사"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        { NewFlg = 0; Grid_Search(false); }
        #endregion

        #region 그리드조회
        private void Grid_Search(bool Msg)
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {
                string strSInspFlag = cboSInspClassCd.SelectedValue.ToString();
                string strSInspSql = "";
                switch (strSInspFlag)
                {
                    case "P": strSInspSql = ", @pPROD_INSP_FLAG = '[^N]'"; break;
                    case "R": strSInspSql = ", @pRECV_INSP_FLAG = '[^N]'"; break;
                    case "F": strSInspSql = ", @pFINAL_INSP_FLAG = '[^N]'"; break;
                    case "S": strSInspSql = ", @pSHIP_INSP_FLAG = '[^N]'"; break;
                    default: strSInspSql = ""; break;
                }

                string strQuery = " usp_QBA011  @pTYPE = 'S1'";
                strQuery += ", @pPLANT_CD = '" + cboSPlantCd.SelectedValue.ToString() + "' ";
                strQuery += strSInspSql;
                strQuery += ", @pITEM_ACCT = '" + cboSItemAcct.SelectedValue.ToString() + "' ";
                strQuery += ", @pITEM_CD = '" + txtSItemCd.Text + "' ";
                strQuery += ", @pITEM_NM = '" + txtSItemNm.Text + "' ";
                strQuery += ", @pINSP_CLASS_CD = '" + strSInspFlag + "'";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread2, strQuery, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, Msg, 0, 0, true);

                if (fpSpread2.Sheets[0].Rows.Count > 0)
                {
                    if (NewFlg == 0)
                    {
                        ShowRow = 0;
                        strCode[0] = fpSpread2.Sheets[0].Cells[ShowRow, SystemBase.Base.GridHeadIndex(GHIdx2, "등록")].Text.Trim();
                        strCode[1] = fpSpread2.Sheets[0].Cells[ShowRow, SystemBase.Base.GridHeadIndex(GHIdx2, "공장코드")].Text.Trim();
                        strCode[2] = fpSpread2.Sheets[0].Cells[ShowRow, SystemBase.Base.GridHeadIndex(GHIdx2, "검사분류코드")].Text.Trim();
                        strCode[3] = fpSpread2.Sheets[0].Cells[ShowRow, SystemBase.Base.GridHeadIndex(GHIdx2, "품목코드")].Text.Trim();
                        strCode[5] = fpSpread2.Sheets[0].Cells[ShowRow, SystemBase.Base.GridHeadIndex(GHIdx2, "KEY")].Text.Trim();
                        strCode[6] = fpSpread2.Sheets[0].Cells[ShowRow, SystemBase.Base.GridHeadIndex(GHIdx2, "기준서번호")].Text.Trim();
                    }
                    SubSearch(strCode);

                }
                else
                {
                    SystemBase.Validation.GroupBox_Reset(groupBox2);
                    SystemBase.Validation.GroupBoxControlsLock(groupBox2, false);

                    //그리드초기화
                    fpSpread1.Sheets[0].Rows.Count = 0;

                    //기타 세팅
                    InspClassCd();
                    NewFlg = 1;
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }

            this.Cursor = Cursors.Default;
        }
        #endregion

        #region Master그리드 선택시 상세정보 조회
        private void fpSpread2_LeaveCell(object sender, FarPoint.Win.Spread.LeaveCellEventArgs e)
        {
            if (fpSpread2.Sheets[0].Rows.Count > 0)
            {
                if (e.Row != e.NewRow)
                {
                    try
                    {
                        ShowRow = e.NewRow;

                        strCode[0] = fpSpread2.Sheets[0].Cells[ShowRow, SystemBase.Base.GridHeadIndex(GHIdx2, "등록")].Text.Trim();
                        strCode[1] = fpSpread2.Sheets[0].Cells[ShowRow, SystemBase.Base.GridHeadIndex(GHIdx2, "공장코드")].Text.Trim();
                        strCode[2] = fpSpread2.Sheets[0].Cells[ShowRow, SystemBase.Base.GridHeadIndex(GHIdx2, "검사분류코드")].Text.Trim();
                        strCode[3] = fpSpread2.Sheets[0].Cells[ShowRow, SystemBase.Base.GridHeadIndex(GHIdx2, "품목코드")].Text.Trim();
                        strCode[5] = fpSpread2.Sheets[0].Cells[ShowRow, SystemBase.Base.GridHeadIndex(GHIdx2, "KEY")].Text;
                        strCode[6] = fpSpread2.Sheets[0].Cells[ShowRow, SystemBase.Base.GridHeadIndex(GHIdx2, "기준서번호")].Text;

                        //상세정보조회
                        SubSearch(strCode);
                    }
                    catch (Exception f)
                    {
                        SystemBase.Loggers.Log(this.Name, f.ToString());
                        MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                        //데이터 조회 중 오류가 발생하였습니다.				
                    }
                }

            }
        }
        #endregion

        #region 상세정보 조회
        private void SubSearch(string[] Code) //0:등록 , 1:공장코드, 2:검사분류코드, 3: 품목코드, 4:Detail 그리드수, 5:KEY, 6:기준서번호
        {
            this.Cursor = Cursors.WaitCursor;
            string[] CODE = null;

            SystemBase.Validation.GroupBox_Reset(groupBox2);
            fpSpread1.Sheets[0].Rows.Count = 0;

            cboPlantCd.SelectedValue = Code[1];
            cboInspClassCd.SelectedValue = Code[2];
            txtItemCd.Value = Code[3];
            txtStNo.Text = Code[6];

            if (Code[0] != "") //등록정보가 있다면
            {
                SystemBase.Validation.GroupBoxControlsLock(groupBox2, true);
                txtStNo.ReadOnly = false;
                txtStNo.BackColor = Color.White;
                txtStNo.Tag = "";

                try
                {
                    //groupBox2 조회조건에 값넘기기
                    if (Code[0].IndexOf("-") != -1) //공정검사
                    {
                        Regex rx = new Regex("-");
                        CODE = rx.Split(Code[0]);

                        txtRoutNo.Value = CODE[0];
                        txtProcSeq.Value = CODE[1];
                    }
                    else if (Code[0] != "Y") //수입,출하 검사가 아니면
                    {
                        txtFinInspLvl.Value = Code[0];
                    }

                    string strQuery = " usp_QBA011  @pTYPE = 'S2'";
                    strQuery += ", @pPLANT_CD = '" + Code[1] + "' ";
                    strQuery += ", @pINSP_CLASS_CD = '" + Code[2] + "' ";
                    strQuery += ", @pITEM_CD = '" + Code[3] + "' ";

                    if (CODE != null) //공정검사이면
                    {
                        strQuery += ", @pFIN_INSP_LVL = '*' ";
                        strQuery += ", @pROUT_NO = '" + CODE[0] + "' ";
                        strQuery += ", @pPROC_SEQ = '" + CODE[1] + "' ";
                    }
                    else if (Code[0] != "Y")//수입,출하 검사가 아니면
                    {
                        strQuery += ", @pFIN_INSP_LVL = '" + Code[0] + "'";
                        strQuery += ", @pROUT_NO = '*' ";
                        strQuery += ", @pPROC_SEQ = '*' ";
                    }

                    strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' ";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    // 2015.06.03. hma 추가(Start): 기준일자 및 전체일자 조회 여부
                    strQuery += ", @pBASE_DT = '" + dtpBaseDt.Text + "' ";
                    if (chkAllDt.Checked == true)
                        strQuery += ", @pALL_DT = 'Y' ";
                    else 
                        strQuery += ", @pALL_DT = 'N' ";
                    // 2015.06.03. hma 추가(End)

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 3);

                    strCode[4] = fpSpread1.Sheets[0].Rows.Count.ToString();

                    if (Convert.ToInt32(strCode[4]) > 0)
                    {

                        fpSpread1.Sheets[0].Rows[0, fpSpread1.Sheets[0].Rows.Count - 1].Height = 30;
                        for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                        {
                            Insp_Q(i);
                        }
                        btnInspBase.Enabled = true;
                        btnInspResult.Enabled = true;
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //데이터 조회 중 오류가 발생하였습니다.
                }
            }
            else	//등록정보가 없다면
            {
                SystemBase.Validation.GroupBoxControlsLock(groupBox2, false);
                InspClassCd();
                btnInspBase.Enabled = false;
                btnInspResult.Enabled = false;
            }
 
            string strQuery1 = "";
            strQuery1 = " usp_QFA002  @pTYPE = 'S4'";
            strQuery1 += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
            strQuery1 += ", @pITEM_CD = '" + txtItemCd.Text + "' ";
            DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery1);
            if (dt.Rows[0]["FILEEXTENSIONS"].ToString() == "jpg" || dt.Rows[0]["FILEEXTENSIONS"].ToString() == "bmp" || dt.Rows[0]["FILEEXTENSIONS"].ToString() == "gif" || dt.Rows[0]["FILEEXTENSIONS"].ToString() == "JPG" || dt.Rows[0]["FILEEXTENSIONS"].ToString() == "BMP" || dt.Rows[0]["FILEEXTENSIONS"].ToString() == "GIF")
            {
                btnITEM_PICTURE.Enabled = true;

                string FtpFile = "ftp://172.30.24.14/ITEM_IMAGE/";
                FullFileName = FtpFile + txtItemCd.Text;
            }
            else
            {
                btnITEM_PICTURE.Enabled = false;
            }

            NewFlg = 2;
            this.Cursor = Cursors.Default;
        }
        #endregion

        #region SaveExec() 데이타 저장 로직
        protected override void SaveExec()
        {
            fpSpread1.Focus();

            //상단 그룹박스 필수 체크
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox2))
            {

                #region 유효성 체크
                if (cboInspClassCd.SelectedValue.ToString() == "F")
                {
                    if (txtFinInspLvlNm.Text == "")
                    {
                        //존재하지 않는 최종검사레벨 코드입니다
                        MessageBox.Show(SystemBase.Base.MessageRtn("B0036", "최종검사레벨"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtFinInspLvl.Focus();
                        return;
                    }
                }
                else if (cboInspClassCd.SelectedValue.ToString() == "P")
                {
                    if (txtProcSeqNm.Text == "")
                    {
                        //존재하지 않는 공정 코드입니다
                        MessageBox.Show(SystemBase.Base.MessageRtn("B0036", "공정"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtProcSeq.Focus();
                        return;

                    }
                }
                #endregion
                if (fpSpread1.Sheets[0].Rows.Count > 0)
                {
                    this.Cursor = Cursors.WaitCursor;

                    string ERRCode = "ER", MSGCode = "P0000"; //처리할 내용이 없습니다.
                    SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                    SqlCommand cmd = dbConn.CreateCommand();
                    SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                    try
                    {
                        //그리드 상단 필수 체크
                        if (SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", false))
                        {
                            //행수만큼 처리
                            for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                            {
                                string strHead = fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text;
                                string strGbn = "";

                                if (strHead.Length > 0)
                                {
                                    #region 유효성 체크
                                    //검사항목코드 유효성 체크
                                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사항목명")].Text == "")
                                    {
                                        //존재하지 않는 검사항목 코드입니다
                                        MessageBox.Show(SystemBase.Base.MessageRtn("B0036", "검사항목"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                        fpSpread1.ActiveSheet.SetActiveCell(i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사항목코드"));

                                        Trans.Rollback();
                                        this.Cursor = Cursors.Default;
                                        return;
                                    }

                                    //검사방식코드 유효성 체크
                                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사방식명")].Text == "")
                                    {
                                        //존재하지 않는 검사방식 코드입니다
                                        MessageBox.Show(SystemBase.Base.MessageRtn("B0036", "검사방식"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                        fpSpread1.ActiveSheet.SetActiveCell(i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사방식코드"));

                                        Trans.Rollback();
                                        this.Cursor = Cursors.Default;
                                        return;
                                    }

                                    //측정기코드 유효성 체크
                                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "측정기코드")].Text != "")
                                    {
                                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "측정기명")].Text == "")
                                        {
                                            //존재하지 않는 측정기 코드입니다
                                            MessageBox.Show(SystemBase.Base.MessageRtn("B0036", "측정기"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                            fpSpread1.ActiveSheet.SetActiveCell(i, SystemBase.Base.GridHeadIndex(GHIdx1, "측정기코드"));

                                            Trans.Rollback();
                                            this.Cursor = Cursors.Default;
                                            return;
                                        }
                                    }

                                    //하한,상한 제약조건
                                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품질표시")].Text != "")
                                    {
                                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품질표시")].Value.ToString() == "3")
                                        {
                                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "하한규격")].Text == ""
                                                && fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "상한규격")].Text == "")
                                            {
                                                MessageBox.Show("하한,상한 규격중 하나이상 입력하세요!", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                                fpSpread1.ActiveSheet.SetActiveCell(i, SystemBase.Base.GridHeadIndex(GHIdx1, "하한규격"));

                                                Trans.Rollback();
                                                this.Cursor = Cursors.Default;
                                                return;
                                            }
                                        }
                                    }

                                    //하한,상한 규격 하한>상한 오류
                                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "하한규격")].Text != ""
                                        && fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "상한규격")].Text != "")
                                    {

                                        if (Convert.ToDouble(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "하한규격")].Value)
                                            > Convert.ToDouble(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "상한규격")].Value))
                                        {
                                            MessageBox.Show("하한치가 상한치 보다 클수 없습니다!", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                            fpSpread1.ActiveSheet.SetActiveCell(i, SystemBase.Base.GridHeadIndex(GHIdx1, "하한규격"));

                                            Trans.Rollback();
                                            this.Cursor = Cursors.Default;
                                            return;
                                        }
                                    }

                                    //관리하한,상한 하한>상한 오류
                                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "관리하한")].Text != ""
                                        && fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "관리상한")].Text != "")
                                    {
                                        if (Convert.ToDouble(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "관리하한")].Value)
                                            > Convert.ToDouble(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "관리상한")].Value))
                                        {
                                            MessageBox.Show("관리하한치가 관리상한치 보다 클수 없습니다!", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                            fpSpread1.ActiveSheet.SetActiveCell(i, SystemBase.Base.GridHeadIndex(GHIdx1, "관리하한"));

                                            Trans.Rollback();
                                            this.Cursor = Cursors.Default;
                                            return;
                                        }
                                    }
                                    #endregion

                                    switch (strHead)
                                    {
                                        case "U": strGbn = "U1"; break;
                                        case "I": strGbn = "I1"; break;
                                        case "D": strGbn = "D1"; break;
                                        default: strGbn = ""; break;
                                    }

                                    string strSql = " usp_QBA011 '" + strGbn + "'";
                                    strSql += ", @pPLANT_CD = '" + cboPlantCd.SelectedValue.ToString() + "' ";
                                    strSql += ", @pINSP_CLASS_CD = '" + cboInspClassCd.SelectedValue.ToString() + "' ";
                                    strSql += ", @pITEM_CD = '" + txtItemCd.Text + "' ";

                                    if (txtFinInspLvl.Text == "")
                                        strSql += ", @pFIN_INSP_LVL = '*' ";
                                    else
                                        strSql += ", @pFIN_INSP_LVL = '" + txtFinInspLvl.Text + "' ";

                                    if (txtRoutNo.Text == "")
                                        strSql += ", @pROUT_NO = '*' ";
                                    else
                                        strSql += ", @pROUT_NO = '" + txtRoutNo.Text + "' ";

                                    if (txtProcSeq.Text == "")
                                        strSql += ", @pPROC_SEQ = '*' ";
                                    else
                                        strSql += ", @pPROC_SEQ = '" + txtProcSeq.Text + "' ";

                                    strSql += ", @pST_NO = '" + txtStNo.Text + "' ";

                                    strSql += ", @pINSP_ITEM_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사항목코드")].Text + "' ";
                                    strSql += ", @pINSP_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사순서")].Text + "' ";
                                    strSql += ", @pINSP_METH_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사방식코드")].Text + "' ";
                                    strSql += ", @pINSP_QSHOW = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품질표시")].Value + "' ";
                                    strSql += ", @pINSP_WEIGHT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "중요도")].Value + "' ";
                                    strSql += ", @pINSP_SPEC = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사규격")].Value + "' ";

                                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "하한규격")].Text == "")
                                        strSql += ", @pINSP_LSL = NULL";
                                    else
                                        strSql += ", @pINSP_LSL = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "하한규격")].Value + "' ";

                                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "상한규격")].Text == "")
                                        strSql += ", @pINSP_USL = NULL";
                                    else
                                        strSql += ", @pINSP_USL = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "상한규격")].Value + "' ";

                                    strSql += ", @pCONT_CMTD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "관리한계산출방법")].Value + "' ";

                                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "관리한계계산수")].Text != "")
                                        strSql += ", @pCONT_CQTY = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "관리한계계산수")].Value + "' ";

                                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "관리하한")].Text == "")
                                        strSql += ", @pCONT_LCL = NULL";
                                    else
                                        strSql += ", @pCONT_LCL = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "관리하한")].Value + "' ";

                                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "관리상한")].Text == "")
                                        strSql += ", @pCONT_UCL = NULL";
                                    else
                                        strSql += ", @pCONT_UCL = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "관리상한")].Value + "' ";

                                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "측정기코드")].Text == "")
                                        strSql += ", @pMEASURE_CD = NULL ";
                                    else
                                        strSql += ", @pMEASURE_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "측정기코드")].Text + "' ";

                                    strSql += ", @pMEASURE_UNIT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "측정단위")].Text + "' ";
                                    strSql += ", @pINSP_DESC = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사내역")].Text + "' ";
                                    strSql += ", @pREMARK = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "비고")].Text + "' ";
                                    strSql += ", @pCFM = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "항목속성코드")].Text + "' ";
                                    strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                                    strSql += ", @pMAP_COOR = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "도면좌표")].Text + "' ";
                                    strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                                    // 2015.06.02. hma 추가(Start): 유효시작일과 종료일도 저장되도록 함.
                                    strSql += ", @pEFF_DT_START = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "시작유효일")].Text + "' ";
                                    strSql += ", @pEFF_DT_END = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "종료유효일")].Text + "' ";
                                    // 2015.06.02. hma 추가(End)

                                    DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                                    ERRCode = ds.Tables[0].Rows[0][0].ToString();
                                    MSGCode = ds.Tables[0].Rows[0][1].ToString();

                                    if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프					
                                }
                            }
                        }
                        else
                        {
                            Trans.Rollback();
                            this.Cursor = Cursors.Default;
                            return;
                        }
                        Trans.Commit();
                    }
                    catch (Exception e)
                    {
                        SystemBase.Loggers.Log(this.Name, e.ToString());
                        Trans.Rollback();
                        ERRCode = "ER";
                        MSGCode = e.Message;
                        //MSGCode = "P0001";	//에러가 발생되어 데이터 처리가 취소되었습니다.
                    }
                Exit:
                    dbConn.Close();
                    if (ERRCode == "OK")
                    {
                        if (strCode[1] != cboPlantCd.SelectedValue.ToString() || strCode[2] != cboInspClassCd.SelectedValue.ToString())
                            NewFlg = 1;

                        cboSPlantCd.SelectedValue = cboPlantCd.SelectedValue;
                        cboSInspClassCd.SelectedValue = cboInspClassCd.SelectedValue;

                        if (txtFinInspLvl.Text != "")
                            strCode[0] = txtFinInspLvl.Text;
                        else if (cboInspClassCd.SelectedValue.ToString() == "P")
                            strCode[0] = txtRoutNo.Text + "-" + txtProcSeq.Text;
                        else
                            strCode[0] = "Y";

                        strCode[1] = cboPlantCd.SelectedValue.ToString();
                        strCode[2] = cboInspClassCd.SelectedValue.ToString();
                        strCode[3] = txtItemCd.Text;
                        strCode[6] = txtStNo.Text;

                        if (NewFlg == 1)
                        {
                            Grid_Search(true);
                            Mastergrd_Grid();
                        }
                        else if (NewFlg == 2)
                        {
                            SubSearch(strCode);
                            Mastergrd_Grid();
                        }
                        else { SearchExec(); }

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
        }
        #endregion

        #region 그리드버튼 클릭
        private void fpSpread1_ButtonClicked(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            //검사항목코드 팝업
            if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "검사항목코드_2"))
            {
                fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "검사방식코드")].Text = "";
                fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "검사방식명")].Text = "";

                try
                {
                    string strQuery = " usp_Q_COMMON @pType='Q010', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                    string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                    string[] strSearch = new string[] { fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "검사항목코드")].Text, "" };
                    UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P06001", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "검사항목 조회", false);
                    pu.ShowDialog();
                    if (pu.DialogResult == DialogResult.OK)
                    {
                        Regex rx1 = new Regex("#");
                        string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "검사항목코드")].Text = Msgs[0].ToString();
                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "검사항목명")].Text = Msgs[1].ToString();
                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "항목속성코드")].Text = Msgs[2].ToString();
                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "항목속성")].Text = Msgs[3].ToString();

                        //품질표시 제약조건
                        if (fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "항목속성코드")].Text == "C")
                        {
                            string Query = "usp_QBA011 @pType='COMM', @pCODE = 'Q010', @pCFM ='C', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' ";
                            Query += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                            UIForm.FPMake.grdComboRemake(fpSpread1, e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품질표시"), SystemBase.ComboMake.ComboOnGrid(Query, 0));
                        }
                        else
                        {
                            string Query = "usp_QBA011 @pType='COMM', @pCODE = 'Q010', @pCFM ='', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' ";
                            Query += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                            UIForm.FPMake.grdComboRemake(fpSpread1, e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품질표시"), SystemBase.ComboMake.ComboOnGrid(Query, 0));
                        }
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "검사항목코드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
                }
            }

            //검사방식코드 팝업
            if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "검사방식코드_2"))
            {
                try
                {
                    string strQuery = "";

                    //검사방식 제약조건
                    if (fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "항목속성코드")].Text == "C")
                    {
                        strQuery = " usp_B_COMMON @pType='TABLE_POP1', @pSPEC1 = 'INSP_METH_CD',  @pSPEC2 = 'INSP_METH_NM', @pSPEC3 = 'Q_BAS_INSPECTION_METHOD', @pSPEC4 = 'LEFT(INSP_METH_CD,1) ', @pSPEC5 = '[^2]' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                    }
                    else if (fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "항목속성코드")].Text == "")
                    {
                        MessageBox.Show("검사항목을 먼저 선택하세요!", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "검사방식코드")].Text = "";
                        fpSpread1.ActiveSheet.SetActiveCell(e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "검사항목코드"));

                        return;
                    }
                    else
                    {
                        strQuery = " usp_B_COMMON @pType='TABLE_POP', @pSPEC1 = 'INSP_METH_CD',  @pSPEC2 = 'INSP_METH_NM', @pSPEC3 = 'Q_BAS_INSPECTION_METHOD', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                    }

                    string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                    string[] strSearch = new string[] { fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "검사방식코드")].Text, "" };
                    UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P06002", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "검사방식 조회", false);
                    pu.ShowDialog();
                    if (pu.DialogResult == DialogResult.OK)
                    {
                        Regex rx1 = new Regex("#");
                        string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "검사방식코드")].Text = Msgs[0].ToString();
                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "검사방식명")].Text = Msgs[1].ToString();

                        //품질표시제약조건
                        if (fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "검사방식코드")].Text.Substring(0, 1) == "2"
                            && fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "검사방식명")].Text != "")
                        {
                            string Query = "usp_QBA011 @pType='COMM', @pCODE = 'Q010', @pCFM ='2', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' ";
                            Query += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                            UIForm.FPMake.grdComboRemake(fpSpread1, e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품질표시"), SystemBase.ComboMake.ComboOnGrid(Query, 0));
                        }
                        else
                        {
                            string Query = "usp_QBA011 @pType='COMM', @pCODE = 'Q010', @pCFM ='', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' ";
                            Query += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                            UIForm.FPMake.grdComboRemake(fpSpread1, e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품질표시"), SystemBase.ComboMake.ComboOnGrid(Query, 0));
                        }
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "검사방식코드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
                }
            }

            //측정기코드 팝업
            if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "측정기코드_2"))
            {
                try
                {
                    string strQuery = " usp_B_COMMON @pType='TABLE_POP', @pSPEC1 = 'MEASURE_CD',  @pSPEC2 = 'MEASURE_NM', @pSPEC3 = 'Q_BAS_MEASURE', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                    string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                    string[] strSearch = new string[] { fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "측정기코드")].Text, "" };
                    UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P06003", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "측정기 조회", false);
                    pu.ShowDialog();
                    if (pu.DialogResult == DialogResult.OK)
                    {
                        Regex rx1 = new Regex("#");
                        string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "측정기코드")].Text = Msgs[0].ToString();
                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "측정기명")].Text = Msgs[1].ToString();

                        UIForm.FPMake.fpChange(fpSpread1, e.Row);
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "측정기코드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
                }
            }

            //측정단위 팝업
            if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "측정단위_2"))
            {
                try
                {
                    string strQuery = " usp_B_COMMON @pType='COMM_POP', @pSPEC1 = 'Z005', @pLANG_CD ='" + SystemBase.Base.gstrLangCd + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                    string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                    string[] strSearch = new string[] { fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "측정단위")].Text, "" };
                    UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00029", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "측정단위 조회", false);
                    pu.ShowDialog();
                    if (pu.DialogResult == DialogResult.OK)
                    {
                        Regex rx1 = new Regex("#");
                        string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "측정단위")].Text = Msgs[0].ToString();

                        UIForm.FPMake.fpChange(fpSpread1, e.Row);
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "측정단위 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
                }
            }
        }
        #endregion

        #region 그리드 Change 이벤트
        private void fpSpread1_Change(object sender, FarPoint.Win.Spread.ChangeEventArgs e)
        {
            if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "검사항목코드"))
            {
                fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "검사방식코드")].Text = "";
                fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "검사방식명")].Text = "";

                fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "검사항목명")].Text
                    = SystemBase.Base.CodeName("INSP_ITEM_CD", "INSP_ITEM_NM", "Q_BAS_INSPECTION_ITEM", fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "검사항목코드")].Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");

                fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "항목속성코드")].Text
                    = SystemBase.Base.CodeName("INSP_ITEM_CD", "INSP_ITEM_CHAR", "Q_BAS_INSPECTION_ITEM", fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "검사항목코드")].Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");

                fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "항목속성")].Text
                    = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "항목속성코드")].Text, " AND MAJOR_CD = 'Q007' AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND COMP_CODE ='" + SystemBase.Base.gstrCOMCD + "'");

                //품질표시제약조건
                Insp_Q(e.Row);
            }

            if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "검사방식코드"))
            {
                //검사방식 제약조건
                if (fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "항목속성코드")].Text == "C")
                {
                    fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "검사방식명")].Text
                        = SystemBase.Base.CodeName("INSP_METH_CD", "INSP_METH_NM", "Q_BAS_INSPECTION_METHOD", fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "검사방식코드")].Text, " AND LEFT(INSP_METH_CD,1) <> '2' AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
                }
                else if (fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "항목속성코드")].Text == "")
                {
                    MessageBox.Show("검사항목을 먼저 선택하세요!", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "검사방식코드")].Text = "";
                    fpSpread1.ActiveSheet.SetActiveCell(e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "검사항목코드"));
                }
                else
                {
                    fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "검사방식명")].Text
                        = SystemBase.Base.CodeName("INSP_METH_CD", "INSP_METH_NM", "Q_BAS_INSPECTION_METHOD", fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "검사방식코드")].Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
                }

                //품질표시 제약조건
                Insp_Q(e.Row);
            }

            //측정기코드
            if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "측정기코드"))
            {
                fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "측정기명")].Text
                    = SystemBase.Base.CodeName("MEASURE_CD", "MEASURE_NM", "Q_BAS_MEASURE", fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "측정기코드")].Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
            }

            //검사순서
            if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "검사순서"))
            {
                string NowValue = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "검사순서")].Text;
                string NowInspItemCd = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "검사항목코드")].Text;

                if (fpSpread1.Sheets[0].Rows.Count > 0)
                {
                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {
                        if (NowValue == fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사순서")].Text && e.Row != i
                            && NowInspItemCd == fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사항목코드")].Text)
                        {
                            MessageBox.Show("검사순서는 동일한 값을 입력할 수 없습니다!", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            fpSpread1.ActiveSheet.SetActiveCell(e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "검사순서"));

                            NowValue = "";
                            fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "검사순서")].Text = strInspSeq;

                            if (fpSpread1.Sheets[0].RowHeader.Cells[e.Row, 0].Text != "I")
                                fpSpread1.Sheets[0].RowHeader.Cells[e.Row, 0].Text = "";
                        }
                    }
                }
            }
        }
        #endregion

        #region 품질표시 제약조건
        private void Insp_Q(int iRow)
        {

            if (fpSpread1.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx1, "항목속성코드")].Text == "C")
            {
                string Query = "usp_QBA011 @pType='COMM', @pCODE = 'Q010', @pCFM ='C', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' ";
                Query += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                UIForm.FPMake.grdComboRemake(fpSpread1, iRow, SystemBase.Base.GridHeadIndex(GHIdx1, "품질표시"), SystemBase.ComboMake.ComboOnGrid(Query, 0));
            }
            else
            {
                string Query = "usp_QBA011 @pType='COMM', @pCODE = 'Q010', @pCFM ='', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' ";
                Query += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                UIForm.FPMake.grdComboRemake(fpSpread1, iRow, SystemBase.Base.GridHeadIndex(GHIdx1, "품질표시"), SystemBase.ComboMake.ComboOnGrid(Query, 0));
            }

            if (fpSpread1.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx1, "검사방식코드")].Text != "")
            {
                if (fpSpread1.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx1, "검사방식코드")].Text.Substring(0, 1) == "2")
                {
                    string Query = "usp_QBA011 @pType='COMM', @pCODE = 'Q010', @pCFM ='2', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' ";
                    Query += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    UIForm.FPMake.grdComboRemake(fpSpread1, iRow, SystemBase.Base.GridHeadIndex(GHIdx1, "품질표시"), SystemBase.ComboMake.ComboOnGrid(Query, 0));
                }
                else
                {
                    string Query = "usp_QBA011 @pType='COMM', @pCODE = 'Q010', @pCFM ='', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' ";
                    Query += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    UIForm.FPMake.grdComboRemake(fpSpread1, iRow, SystemBase.Base.GridHeadIndex(GHIdx1, "품질표시"), SystemBase.ComboMake.ComboOnGrid(Query, 0));
                }
            }
        }
        #endregion

        #region 검사순서 값
        private void fpSpread1_CellClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            if (fpSpread1.Sheets[0].Rows.Count > 0)
                strInspSeq = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "검사순서")].Text;
        }
        #endregion

        #region Master 그리드 변경
        private void Mastergrd_Grid()
        {
            try
            {
                //품목코드 변경시 로우 제설정
                if (fpSpread2.Sheets[0].Cells[ShowRow, SystemBase.Base.GridHeadIndex(GHIdx2, "품목코드")].Text != strCode[3])
                {
                    txtSItemCd.Text = strCode[3];
                    Grid_Search(true);
                    fpSpread2.Search(0, strCode[3].Trim(), true, true, true, true, 0, SystemBase.Base.GridHeadIndex(GHIdx2, "품목코드"), ref SearchRow, ref ShowColumn);
                    ShowRow = SearchRow;
                }

                if (fpSpread2.Sheets[0].Cells[ShowRow, SystemBase.Base.GridHeadIndex(GHIdx2, "등록")].Text == "") //첫등록일때
                {
                    fpSpread2.Sheets[0].Cells[ShowRow, SystemBase.Base.GridHeadIndex(GHIdx2, "등록")].Text = strCode[0];
                    fpSpread2.Sheets[0].Cells[ShowRow, SystemBase.Base.GridHeadIndex(GHIdx2, "KEY")].Text = strCode[3] + strCode[0] + strCode[1] + strCode[2];
                }
                else if (strCode[4] == "0") //DETAIL 데이타가 없을때
                {
                    int iStartRow = 0;
                    int iEndRow = fpSpread2.Sheets[0].Rows.Count - 1;

                    if (iEndRow == iStartRow)
                    {
                        fpSpread2.Sheets[0].Cells[ShowRow, SystemBase.Base.GridHeadIndex(GHIdx2, "등록")].Text = "";
                        fpSpread2.Sheets[0].Cells[ShowRow, SystemBase.Base.GridHeadIndex(GHIdx2, "KEY")].Text = strCode[3] + strCode[1] + strCode[2];

                        NewFlg = 2;
                        SystemBase.Validation.GroupBoxControlsLock(groupBox2, false);
                        InspClassCd();
                    }
                    else
                    {
                        if (ShowRow == iStartRow)
                        {
                            //같은 품목이 등록되어있으면
                            if (fpSpread2.Sheets[0].Cells[ShowRow, SystemBase.Base.GridHeadIndex(GHIdx2, "품목코드")].Text
                                == fpSpread2.Sheets[0].Cells[ShowRow + 1, SystemBase.Base.GridHeadIndex(GHIdx2, "품목코드")].Text)
                            {
                                fpSpread2.Sheets[0].Rows.Remove(ShowRow, 1);

                                strCode[0] = fpSpread2.Sheets[0].Cells[ShowRow, SystemBase.Base.GridHeadIndex(GHIdx2, "등록")].Text.Trim();
                                strCode[1] = fpSpread2.Sheets[0].Cells[ShowRow, SystemBase.Base.GridHeadIndex(GHIdx2, "공장코드")].Text.Trim();
                                strCode[2] = fpSpread2.Sheets[0].Cells[ShowRow, SystemBase.Base.GridHeadIndex(GHIdx2, "검사분류코드")].Text.Trim();
                                strCode[3] = fpSpread2.Sheets[0].Cells[ShowRow, SystemBase.Base.GridHeadIndex(GHIdx2, "품목코드")].Text.Trim();
                                strCode[5] = fpSpread2.Sheets[0].Cells[ShowRow, SystemBase.Base.GridHeadIndex(GHIdx2, "KEY")].Text;

                                //상세정보조회
                                SubSearch(strCode);

                                if (strCode[0] == "") { NewFlg = 1; }
                                else { NewFlg = 2; }
                            }
                            else
                            {
                                fpSpread2.Sheets[0].Cells[ShowRow, SystemBase.Base.GridHeadIndex(GHIdx2, "등록")].Text = "";
                                fpSpread2.Sheets[0].Cells[ShowRow, SystemBase.Base.GridHeadIndex(GHIdx2, "KEY")].Text = strCode[3] + strCode[1] + strCode[2];

                                NewFlg = 2;
                                SystemBase.Validation.GroupBoxControlsLock(groupBox2, false);
                                InspClassCd();
                            }
                        }
                        else if (ShowRow == iEndRow)
                        {
                            //같은 품목이 등록되어있으면
                            if (fpSpread2.Sheets[0].Cells[ShowRow, SystemBase.Base.GridHeadIndex(GHIdx2, "품목코드")].Text
                                == fpSpread2.Sheets[0].Cells[ShowRow - 1, SystemBase.Base.GridHeadIndex(GHIdx2, "품목코드")].Text)
                            {
                                fpSpread2.Sheets[0].Rows.Remove(ShowRow, 1);

                                strCode[0] = fpSpread2.Sheets[0].Cells[ShowRow, SystemBase.Base.GridHeadIndex(GHIdx2, "등록")].Text.Trim();
                                strCode[1] = fpSpread2.Sheets[0].Cells[ShowRow, SystemBase.Base.GridHeadIndex(GHIdx2, "공장코드")].Text.Trim();
                                strCode[2] = fpSpread2.Sheets[0].Cells[ShowRow, SystemBase.Base.GridHeadIndex(GHIdx2, "검사분류코드")].Text.Trim();
                                strCode[3] = fpSpread2.Sheets[0].Cells[ShowRow, SystemBase.Base.GridHeadIndex(GHIdx2, "품목코드")].Text.Trim();
                                strCode[5] = fpSpread2.Sheets[0].Cells[ShowRow, SystemBase.Base.GridHeadIndex(GHIdx2, "KEY")].Text;

                                //상세정보조회
                                SubSearch(strCode);

                                if (strCode[0] == "") { NewFlg = 1; }
                                else { NewFlg = 2; }
                            }
                            else
                            {
                                fpSpread2.Sheets[0].Cells[ShowRow, SystemBase.Base.GridHeadIndex(GHIdx2, "등록")].Text = "";
                                fpSpread2.Sheets[0].Cells[ShowRow, SystemBase.Base.GridHeadIndex(GHIdx2, "KEY")].Text = strCode[3] + strCode[1] + strCode[2];

                                NewFlg = 2;
                                SystemBase.Validation.GroupBoxControlsLock(groupBox2, false);
                                InspClassCd();
                            }
                        }
                        else
                        {
                            //같은 품목이 등록되어있으면
                            if (fpSpread2.Sheets[0].Cells[ShowRow, SystemBase.Base.GridHeadIndex(GHIdx2, "품목코드")].Text
                                == fpSpread2.Sheets[0].Cells[ShowRow + 1, SystemBase.Base.GridHeadIndex(GHIdx2, "품목코드")].Text
                                || fpSpread2.Sheets[0].Cells[ShowRow, SystemBase.Base.GridHeadIndex(GHIdx2, "품목코드")].Text
                                == fpSpread2.Sheets[0].Cells[ShowRow - 1, SystemBase.Base.GridHeadIndex(GHIdx2, "품목코드")].Text)
                            {
                                fpSpread2.Sheets[0].Rows.Remove(ShowRow, 1);

                                strCode[0] = fpSpread2.Sheets[0].Cells[ShowRow, SystemBase.Base.GridHeadIndex(GHIdx2, "등록")].Text.Trim();
                                strCode[1] = fpSpread2.Sheets[0].Cells[ShowRow, SystemBase.Base.GridHeadIndex(GHIdx2, "공장코드")].Text.Trim();
                                strCode[2] = fpSpread2.Sheets[0].Cells[ShowRow, SystemBase.Base.GridHeadIndex(GHIdx2, "검사분류코드")].Text.Trim();
                                strCode[3] = fpSpread2.Sheets[0].Cells[ShowRow, SystemBase.Base.GridHeadIndex(GHIdx2, "품목코드")].Text.Trim();
                                strCode[5] = fpSpread2.Sheets[0].Cells[ShowRow, SystemBase.Base.GridHeadIndex(GHIdx2, "KEY")].Text;

                                //상세정보조회
                                SubSearch(strCode);

                                if (strCode[0] == "") { NewFlg = 1; }
                                else { NewFlg = 2; }
                            }
                            else
                            {
                                fpSpread2.Sheets[0].Cells[ShowRow, SystemBase.Base.GridHeadIndex(GHIdx2, "등록")].Text = "";
                                fpSpread2.Sheets[0].Cells[ShowRow, SystemBase.Base.GridHeadIndex(GHIdx2, "KEY")].Text = strCode[3] + strCode[1] + strCode[2];

                                NewFlg = 2;
                                SystemBase.Validation.GroupBoxControlsLock(groupBox2, false);
                                InspClassCd();
                            }
                        }
                    }
                }
                else //두번째 등록 저장 로직
                {
                    string strKey = strCode[3] + strCode[0] + strCode[1] + strCode[2];

                    //로우찾기
                    fpSpread2.Search(0, strKey.Trim(), true, true, true, true, 0, SystemBase.Base.GridHeadIndex(GHIdx2, "KEY"), ref SearchRow, ref ShowColumn);

                    if (SearchRow == -1)
                    {
                        SearchRow = ShowRow + 1;
                        fpSpread2.Sheets[0].Rows.Add(SearchRow, 1);
                        fpSpread2.Sheets[0].Cells[SearchRow, SystemBase.Base.GridHeadIndex(GHIdx2, "품목코드")].Text = strCode[3];
                        fpSpread2.Sheets[0].Cells[SearchRow, SystemBase.Base.GridHeadIndex(GHIdx2, "품목명")].Text = fpSpread2.Sheets[0].Cells[ShowRow, SystemBase.Base.GridHeadIndex(GHIdx2, "품목명")].Text;
                        fpSpread2.Sheets[0].Cells[SearchRow, SystemBase.Base.GridHeadIndex(GHIdx2, "등록")].Text = strCode[0];
                        fpSpread2.Sheets[0].Cells[SearchRow, SystemBase.Base.GridHeadIndex(GHIdx2, "공장코드")].Text = strCode[1];
                        fpSpread2.Sheets[0].Cells[SearchRow, SystemBase.Base.GridHeadIndex(GHIdx2, "검사분류코드")].Text = strCode[2];
                        fpSpread2.Sheets[0].Cells[SearchRow, SystemBase.Base.GridHeadIndex(GHIdx2, "KEY")].Text = strCode[3] + strCode[0] + strCode[1] + strCode[2];
                    }

                    ShowRow = SearchRow;
                }

                fpSpread2.Focus();
                fpSpread2.ActiveSheet.SetActiveCell(ShowRow, 1); //Row Focus		
                fpSpread2.ShowRow(0, ShowRow, FarPoint.Win.Spread.VerticalPosition.Center); //Focus Row 보기

            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString());
            }
        }
        #endregion

        #region 개정정보관리 버튼
        private void btnRevision_Click(object sender, System.EventArgs e)
        {
            if (txtItemNm.Text != "")
            {

                try
                {
                    QBA011P1 myForm = new QBA011P1(cboPlantCd.SelectedValue.ToString(), cboInspClassCd.SelectedValue.ToString(), txtItemCd.Text);
                    myForm.ShowDialog();
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "개정정보관리 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);	//데이터 조회 중 오류가 발생하였습니다.
                }
            }
        }
        #endregion

        #region 검사기준서 출력        
        private void btnInspBase_Click(object sender, System.EventArgs e)
        {
            string strFileName = SystemBase.Base.ProgramWhere + @"\Report\검사기준서.xls";

            try
            {
                CheckForIllegalCrossThreadCalls = false;

                th = new Thread(new ThreadStart(Show_Waiting));
                th.Start();
                Thread.Sleep(100);
                Waiting_Form.Activate();
                string strSheetPage1 = "검사기준서1";

                string strQuery1 = " usp_QBA011  @pTYPE = 'R1'";
                strQuery1 += ", @pPLANT_CD = '" + strCode[1] + "' ";
                strQuery1 += ", @pINSP_CLASS_CD = '" + strCode[2] + "' ";
                strQuery1 += ", @pITEM_CD = '" + strCode[3] + "' ";

                if (strCode[2] == "P")
                {
                    string[] CODE = null;

                    Regex rx = new Regex("-");
                    CODE = rx.Split(strCode[0]);

                    strQuery1 += ", @pFIN_INSP_LVL = '*' ";
                    strQuery1 += ", @pROUT_NO = '" + CODE[0] + "' ";
                    strQuery1 += ", @pPROC_SEQ = '" + CODE[1] + "' ";
                }
                else if (strCode[2] == "F")
                {
                    strQuery1 += ", @pFIN_INSP_LVL = '" + strCode[0] + "'";
                    strQuery1 += ", @pROUT_NO = '*' ";
                    strQuery1 += ", @pPROC_SEQ = '*' ";
                }

                strQuery1 += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' ";
                strQuery1 += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                strQuery1 += ", @pBASE_DT = '" + dtpBaseDt.Text + "' ";     // 2015.06.02. hma 추가: 기준일자

                DataTable dt1 = SystemBase.DbOpen.NoTranDataTable(strQuery1);

                if (dt1.Rows.Count > 0)
                {
                    Waiting_Form.progressBar_temp.Maximum = dt1.Rows.Count;

                    double dCount = 22;
                    double dRowCount = Convert.ToDouble(dt1.Rows.Count);
                    int iTotPage = Convert.ToInt32(Math.Ceiling((dRowCount - 10) / dCount));

                    string strQuery2 = " usp_QBA011  @pTYPE = 'R2'";
                    strQuery2 += ", @pPLANT_CD = '" + strCode[1] + "' ";
                    strQuery2 += ", @pINSP_CLASS_CD = '" + strCode[2] + "' ";
                    strQuery2 += ", @pITEM_CD = '" + strCode[3] + "' ";
                    strQuery2 += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    strQuery2 += ", @pBASE_DT = '" + dtpBaseDt.Text + "' ";             // 2015.06.02. hma 추가: 기준일자

                    DataTable dt2 = SystemBase.DbOpen.NoTranDataTable(strQuery2);

                    UIForm.VkExcel excel = null;
                    
                    if (File.Exists(strFileName))
                    {
                        File.SetAttributes(strFileName, System.IO.FileAttributes.ReadOnly);
                    }
                    else
                    {
                        // 엑셀 데이터를 생성할 수 없습니다. 원본 파일이 존재하지 않습니다.
                        MessageBox.Show("엑셀 데이터를 생성할 수 없습니다. 원본 파일이 존재하지 않습니다."); ;
                        return;
                    }

                    excel = new UIForm.VkExcel(false);

                    excel.OpenFile(strFileName);
                    // 현재 시트 선택
                    excel.FindExcelWorksheet(strSheetPage1);


                    //데이터수만큼 미리 복사------------------------------------------
                    if (dt1.Rows.Count > 10)
                    {
                        for (int i = 0; i < iTotPage; i++)
                        {
                            excel.SetSelect("A36", "A36");
                            excel.RunMacro("PageAdd");
                        }
                    }
                    //------------------------------------------------------

                    // 엑셀쓰기--------------------------------------------------------- 

                    if (dt1.Rows.Count > 10)
                    {
                        int iRow = 0;
                        int iRow1 = 0;

                        Waiting_Form.progressBar_temp.Maximum = iTotPage + 1;

                        for (int i = 0; i < iTotPage + 1; i++)
                        {

                            if (i == 0) //1Page
                            {
                                // Heard 값
                                excel.SetCell(2, 1, dt1.Rows[0][0].ToString());
                                excel.SetCell(5, 4, dt1.Rows[0][1].ToString());
                                excel.SetCell(5, 8, dt1.Rows[0][2].ToString());
                                excel.SetCell(6, 4, dt1.Rows[0][3].ToString());
                                excel.SetCell(6, 8, dt1.Rows[0][4].ToString());
                                excel.SetCell(7, 8, dt1.Rows[0][13].ToString());
                                excel.SetCell(8, 8, dt1.Rows[0]["DRAW_REV"].ToString());

                                //내용입력
                                for (iRow1 = 0; iRow1 < 10; iRow1++)
                                {
                                    excel.SetCell(iRow1 + 20, 1, dt1.Rows[iRow1][5].ToString());
                                    excel.SetCell(iRow1 + 20, 2, dt1.Rows[iRow1][6].ToString());
                                    excel.SetCell(iRow1 + 20, 3, dt1.Rows[iRow1][7].ToString());
                                    excel.SetCell(iRow1 + 20, 4, dt1.Rows[iRow1][8].ToString());
                                    excel.SetCell(iRow1 + 20, 5, dt1.Rows[iRow1][9].ToString());
                                    excel.SetCell(iRow1 + 20, 7, dt1.Rows[iRow1][10].ToString());
                                    excel.SetCell(iRow1 + 20, 8, dt1.Rows[iRow1][11].ToString());
                                    excel.SetCell(iRow1 + 20, 9, dt1.Rows[iRow1][12].ToString());
                                }

                                //개정정보입력
                                if (dt2.Rows.Count > 0)
                                {
                                    for (int j = 0; j < 4; j++)
                                    {
                                        int iCell = 32 + j;
                                        string strValue = excel.GetCellValue("A" + iCell);

                                        for (int k = 0; k < dt2.Rows.Count; k++)
                                        {
                                            if (dt2.Rows[k][0].ToString() == strValue)
                                            {
                                                excel.SetCell(j + 32, 2, dt2.Rows[k][1].ToString());
                                                excel.SetCell(j + 32, 4, dt2.Rows[k][2].ToString());
                                                excel.SetCell(j + 32, 6, dt2.Rows[k][3].ToString());
                                                excel.SetCell(j + 32, 8, dt2.Rows[k][4].ToString());
                                                excel.SetCell(j + 32, 9, dt2.Rows[k][5].ToString());
                                                excel.SetCell(j + 32, 10, dt2.Rows[k][6].ToString());
                                            }
                                        }
                                    }
                                }

                                iRow += 35;

                            }
                            else //2Page ....
                            {
                                excel.SetCell(iRow + 2, 1, dt1.Rows[0][0].ToString());
                                excel.SetCell(iRow + 5, 4, dt1.Rows[0][1].ToString());
                                excel.SetCell(iRow + 5, 8, dt1.Rows[0][2].ToString());
                                excel.SetCell(iRow + 6, 4, dt1.Rows[0][3].ToString());
                                excel.SetCell(iRow + 6, 8, dt1.Rows[0][4].ToString());
                                excel.SetCell(iRow + 7, 8, dt1.Rows[0][13].ToString());
                                excel.SetCell(iRow + 8, 8, dt1.Rows[0]["DRAW_REV"].ToString());


                                //내용입력
                                int Count = dt1.Rows.Count - iRow1;

                                if (Count > 21)
                                {
                                    for (int j = 0; j < 22; j++)
                                    {
                                        excel.SetCell(j + iRow + 9, 1, dt1.Rows[iRow1][5].ToString());
                                        excel.SetCell(j + iRow + 9, 2, dt1.Rows[iRow1][6].ToString());
                                        excel.SetCell(j + iRow + 9, 3, dt1.Rows[iRow1][7].ToString());
                                        excel.SetCell(j + iRow + 9, 4, dt1.Rows[iRow1][8].ToString());
                                        excel.SetCell(j + iRow + 9, 5, dt1.Rows[iRow1][9].ToString());
                                        excel.SetCell(j + iRow + 9, 7, dt1.Rows[iRow1][10].ToString());
                                        excel.SetCell(j + iRow + 9, 8, dt1.Rows[iRow1][11].ToString());
                                        excel.SetCell(j + iRow + 9, 9, dt1.Rows[iRow1][12].ToString());

                                        iRow1++;
                                    }
                                    iRow += 30;
                                }
                                else
                                {
                                    for (int j = 0; j < Count; j++)
                                    {
                                        excel.SetCell(j + iRow + 9, 1, dt1.Rows[iRow1][5].ToString());
                                        excel.SetCell(j + iRow + 9, 2, dt1.Rows[iRow1][6].ToString());
                                        excel.SetCell(j + iRow + 9, 3, dt1.Rows[iRow1][7].ToString());
                                        excel.SetCell(j + iRow + 9, 4, dt1.Rows[iRow1][8].ToString());
                                        excel.SetCell(j + iRow + 9, 5, dt1.Rows[iRow1][9].ToString());
                                        excel.SetCell(j + iRow + 9, 7, dt1.Rows[iRow1][10].ToString());
                                        excel.SetCell(j + iRow + 9, 8, dt1.Rows[iRow1][11].ToString());
                                        excel.SetCell(j + iRow + 9, 9, dt1.Rows[iRow1][12].ToString());
                                        iRow1++;
                                    }
                                    iRow += 30;
                                }
                            }
                            Waiting_Form.progressBar_temp.Value = iTotPage + 1;
                        }

                    }
                    else  //1Page 만 있을경우
                    {

                        //heard 값입력
                        excel.SetCell(2, 1, dt1.Rows[0][0].ToString());
                        excel.SetCell(5, 4, dt1.Rows[0][1].ToString());
                        excel.SetCell(5, 8, dt1.Rows[0][2].ToString());
                        excel.SetCell(6, 4, dt1.Rows[0][3].ToString());
                        excel.SetCell(6, 8, dt1.Rows[0][4].ToString());
                        excel.SetCell(7, 8, dt1.Rows[0][13].ToString());
                        excel.SetCell(8, 8, dt1.Rows[0]["DRAW_REV"].ToString());

                        //개정정보입력
                        if (dt2.Rows.Count > 0)
                        {
                            for (int j = 0; j < 4; j++)
                            {
                                int iCell = 32 + j;
                                string strValue = excel.GetCellValue("A" + iCell);

                                for (int k = 0; k < dt2.Rows.Count; k++)
                                {
                                    if (dt2.Rows[k][0].ToString() == strValue)
                                    {
                                        excel.SetCell(j + 32, 2, dt2.Rows[k][1].ToString());
                                        excel.SetCell(j + 32, 4, dt2.Rows[k][2].ToString());
                                        excel.SetCell(j + 32, 6, dt2.Rows[k][3].ToString());
                                        excel.SetCell(j + 32, 8, dt2.Rows[k][4].ToString());
                                        excel.SetCell(j + 32, 9, dt2.Rows[k][5].ToString());
                                        excel.SetCell(j + 32, 10, dt2.Rows[k][6].ToString());
                                    }
                                }
                            }
                        }

                        //내용입력
                        for (int i = 0; i < dt1.Rows.Count; i++)
                        {
                            excel.SetCell(i + 20, 1, dt1.Rows[i][5].ToString());
                            excel.SetCell(i + 20, 2, dt1.Rows[i][6].ToString());
                            excel.SetCell(i + 20, 3, dt1.Rows[i][7].ToString());
                            excel.SetCell(i + 20, 4, dt1.Rows[i][8].ToString());
                            excel.SetCell(i + 20, 5, dt1.Rows[i][9].ToString());
                            excel.SetCell(i + 20, 7, dt1.Rows[i][10].ToString());
                            excel.SetCell(i + 20, 8, dt1.Rows[i][11].ToString());
                            excel.SetCell(i + 20, 9, dt1.Rows[i][12].ToString());

                            Waiting_Form.progressBar_temp.Value = i + 1;
                        }
                    }

                    Waiting_Form.label_temp.Text = "완료되었습니다.";
                    Thread.Sleep(500);
                    excel.ShowExcel(true);
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "검사기준서출력"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Waiting_Form.Close();
                th.Abort();
                File.SetAttributes(strFileName, System.IO.FileAttributes.Normal);
            }
        }

        private void Show_Waiting()
        {
            Waiting_Form = new UIForm.ExcelWaiting("검사기준서출력...");
            Waiting_Form.ShowDialog();
        }

        private void Show_Wait()
        {
            Waiting_Form = new UIForm.ExcelWaiting("검사성적서출력...");
            Waiting_Form.ShowDialog();
        }
        #endregion

        #region 그리드 EditChage
        private void fpSpread1_EditChange(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "검사항목코드"))
            {
                fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "검사방식코드")].Text = "";
                fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "검사방식명")].Text = "";

                fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "검사항목명")].Text
                    = SystemBase.Base.CodeName("INSP_ITEM_CD", "INSP_ITEM_NM", "Q_BAS_INSPECTION_ITEM", fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "검사항목코드")].Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");

                fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "항목속성코드")].Text
                    = SystemBase.Base.CodeName("INSP_ITEM_CD", "INSP_ITEM_CHAR", "Q_BAS_INSPECTION_ITEM", fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "검사항목코드")].Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");

                fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "항목속성")].Text
                    = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "항목속성코드")].Text, " AND MAJOR_CD = 'Q007' AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND COMP_CODE ='" + SystemBase.Base.gstrCOMCD + "'");

                //품질표시제약조건
                Insp_Q(e.Row);
            }

            if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "검사방식코드"))
            {
                //검사방식 제약조건
                if (fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "항목속성코드")].Text == "C")
                {
                    fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "검사방식명")].Text
                        = SystemBase.Base.CodeName("INSP_METH_CD", "INSP_METH_NM", "Q_BAS_INSPECTION_METHOD", fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "검사방식코드")].Text, " AND LEFT(INSP_METH_CD,1) <> '2' AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
                }
                else if (fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "항목속성코드")].Text == "")
                {
                    MessageBox.Show("검사항목을 먼저 선택하세요!", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "검사방식코드")].Text = "";
                    fpSpread1.ActiveSheet.SetActiveCell(e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "검사항목코드"));
                }
                else
                {
                    fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "검사방식명")].Text
                        = SystemBase.Base.CodeName("INSP_METH_CD", "INSP_METH_NM", "Q_BAS_INSPECTION_METHOD", fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "검사방식코드")].Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
                }

                //품질표시 제약조건
                Insp_Q(e.Row);
            }

            //측정기코드
            if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "측정기코드"))
            {
                fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "측정기명")].Text
                    = SystemBase.Base.CodeName("MEASURE_CD", "MEASURE_NM", "Q_BAS_MEASURE", fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "측정기코드")].Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
            }

            //검사순서
            if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "검사순서"))
            {
                string NowValue = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "검사순서")].Text;
                string NowInspItemCd = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "검사항목코드")].Text;

                if (fpSpread1.Sheets[0].Rows.Count > 0)
                {
                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {
                        if (NowValue == fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사순서")].Text && e.Row != i
                            && NowInspItemCd == fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사항목코드")].Text)
                        {
                            MessageBox.Show("검사순서는 동일한 값을 입력할 수 없습니다!", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            fpSpread1.ActiveSheet.SetActiveCell(e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "검사순서"));

                            NowValue = "";
                            fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "검사순서")].Text = strInspSeq;

                            if (fpSpread1.Sheets[0].RowHeader.Cells[e.Row, 0].Text != "I")
                                fpSpread1.Sheets[0].RowHeader.Cells[e.Row, 0].Text = "";
                        }
                    }
                }
            }
        }
        #endregion

        #region 그리드 상 Change
        protected override void fpSpread1_ChangeEvent(int Row, int Column)
        {
            try
            {

                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "검사항목명")].Text
                    = SystemBase.Base.CodeName("INSP_ITEM_CD", "INSP_ITEM_NM", "Q_BAS_INSPECTION_ITEM", fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "검사항목코드")].Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");

                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "항목속성코드")].Text
                    = SystemBase.Base.CodeName("INSP_ITEM_CD", "INSP_ITEM_CHAR", "Q_BAS_INSPECTION_ITEM", fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "검사항목코드")].Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");

                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "항목속성")].Text
                    = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "항목속성코드")].Text, " AND MAJOR_CD = 'Q007' AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND COMP_CODE ='" + SystemBase.Base.gstrCOMCD + "'");


                //검사방식 제약조건
                if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "항목속성코드")].Text == "C")
                {
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "검사방식명")].Text
                        = SystemBase.Base.CodeName("INSP_METH_CD", "INSP_METH_NM", "Q_BAS_INSPECTION_METHOD", fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "검사방식코드")].Text, " AND LEFT(INSP_METH_CD,1) <> '2' AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
                }
                else if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "항목속성코드")].Text == "")
                {
                    MessageBox.Show("검사항목을 먼저 선택하세요!", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "검사방식코드")].Text = "";
                    fpSpread1.ActiveSheet.SetActiveCell(Row, SystemBase.Base.GridHeadIndex(GHIdx1, "검사항목코드"));
                }
                else
                {
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "검사방식명")].Text
                        = SystemBase.Base.CodeName("INSP_METH_CD", "INSP_METH_NM", "Q_BAS_INSPECTION_METHOD", fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "검사방식코드")].Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
                }

                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "측정기명")].Text
                    = SystemBase.Base.CodeName("MEASURE_CD", "MEASURE_NM", "Q_BAS_MEASURE", fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "측정기코드")].Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");

                //품질표시제약조건
                Insp_Q(Row);
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }
        #endregion

        #region 텍스트박스 TextChanged
        private void txtStNo_TextChanged(object sender, System.EventArgs e)
        {
            if (fpSpread1.Sheets[0].RowCount > 0)
            {
                for (int i = 0; i < fpSpread1.Sheets[0].RowCount; i++)
                {
                    fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text = "U";
                }
            }
        }
        #endregion

        #region lnkJump_Click 점프 클릭 이벤트
        private void lnkJump1_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (strJumpFileName1.Length > 0)
                {
                    string DllName = strJumpFileName1.Substring(0, strJumpFileName1.IndexOf("."));
                    string FrmName = strJumpFileName1.Substring(strJumpFileName1.IndexOf(".") + 1, strJumpFileName1.Length - strJumpFileName1.IndexOf(".") - 1);

                    for (int k = 0; k < this.MdiParent.MdiChildren.Length; k++)
                    {	// 폼이 이미 열려있으면 닫기
                        if (MdiParent.MdiChildren[k].Name == FrmName.Substring(0, 6))
                        {
                            MdiParent.MdiChildren[k].BringToFront();
                            MdiParent.MdiChildren[k].Close();
                            break;
                        }
                    }

                    Link1Exec();

                    Assembly ServiceAssembly = Assembly.LoadFile(SystemBase.Base.ProgramWhere.ToString() + "\\" + DllName + "." + FrmName.Substring(0,6) + ".dll");
                    Form myForm = (Form)System.Activator.CreateInstance(ServiceAssembly.GetType(strJumpFileName1), param);
                    myForm.MdiParent = this.MdiParent;
                    myForm.Show();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "화면 링크"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void lnkJump2_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (strJumpFileName2.Length > 0)
                {
                    string DllName = strJumpFileName2.Substring(0, strJumpFileName2.IndexOf("."));
                    string FrmName = strJumpFileName2.Substring(strJumpFileName2.IndexOf(".") + 1, strJumpFileName2.Length - strJumpFileName2.IndexOf(".") - 1);

                    for (int k = 0; k < this.MdiParent.MdiChildren.Length; k++)
                    {	// 폼이 이미 열려있으면 닫기
                        if (MdiParent.MdiChildren[k].Name == FrmName.Substring(0, 6))
                        {
                            MdiParent.MdiChildren[k].BringToFront();
                            MdiParent.MdiChildren[k].Close();
                            break;
                        }
                    }

                    Link2Exec();

                    Assembly ServiceAssembly = Assembly.LoadFile(SystemBase.Base.ProgramWhere.ToString() + "\\" + DllName + "." + FrmName.Substring(0, 6) + ".dll");
                    Form myForm = (Form)System.Activator.CreateInstance(ServiceAssembly.GetType(strJumpFileName2), param);
                    myForm.MdiParent = this.MdiParent;
                    myForm.Show();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "화면 링크"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void lnkJump3_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (strJumpFileName3.Length > 0)
                {
                    string DllName = strJumpFileName3.Substring(0, strJumpFileName3.IndexOf("."));
                    string FrmName = strJumpFileName3.Substring(strJumpFileName3.IndexOf(".") + 1, strJumpFileName3.Length - strJumpFileName3.IndexOf(".") - 1);

                    for (int k = 0; k < this.MdiParent.MdiChildren.Length; k++)
                    {	// 폼이 이미 열려있으면 닫기
                        if (MdiParent.MdiChildren[k].Name == FrmName.Substring(0, 6))
                        {
                            MdiParent.MdiChildren[k].BringToFront();
                            MdiParent.MdiChildren[k].Close();
                            break;
                        }
                    }

                    Link3Exec();

                    Assembly ServiceAssembly = Assembly.LoadFile(SystemBase.Base.ProgramWhere.ToString() + "\\" + DllName + "." + FrmName.Substring(0, 6) + ".dll");
                    Form myForm = (Form)System.Activator.CreateInstance(ServiceAssembly.GetType(strJumpFileName3), param);
                    myForm.MdiParent = this.MdiParent;
                    myForm.Show();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "화면 링크"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void lnkJump4_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (strJumpFileName4.Length > 0)
                {
                    string DllName = strJumpFileName4.Substring(0, strJumpFileName4.IndexOf("."));
                    string FrmName = strJumpFileName4.Substring(strJumpFileName4.IndexOf(".") + 1, strJumpFileName4.Length - strJumpFileName4.IndexOf(".") - 1);

                    for (int k = 0; k < this.MdiParent.MdiChildren.Length; k++)
                    {	// 폼이 이미 열려있으면 닫기
                        if (MdiParent.MdiChildren[k].Name == FrmName.Substring(0, 6))
                        {
                            MdiParent.MdiChildren[k].BringToFront();
                            MdiParent.MdiChildren[k].Close();
                            break;
                        }
                    }

                    Link4Exec();

                    Assembly ServiceAssembly = Assembly.LoadFile(SystemBase.Base.ProgramWhere.ToString() + "\\" + DllName + "." + FrmName.Substring(0, 6) + ".dll");
                    Form myForm = (Form)System.Activator.CreateInstance(ServiceAssembly.GetType(strJumpFileName4), param);
                    myForm.MdiParent = this.MdiParent;
                    myForm.Show();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "화면 링크"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void lnkJump5_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (strJumpFileName5.Length > 0)
                {
                    string DllName = strJumpFileName5.Substring(0, strJumpFileName5.IndexOf("."));
                    string FrmName = strJumpFileName5.Substring(strJumpFileName5.IndexOf(".") + 1, strJumpFileName5.Length - strJumpFileName5.IndexOf(".") - 1);

                    for (int k = 0; k < this.MdiParent.MdiChildren.Length; k++)
                    {	// 폼이 이미 열려있으면 닫기
                        if (MdiParent.MdiChildren[k].Name == FrmName.Substring(0, 6))
                        {
                            MdiParent.MdiChildren[k].BringToFront();
                            MdiParent.MdiChildren[k].Close();
                            break;
                        }
                    }

                    Link5Exec();

                    Assembly ServiceAssembly = Assembly.LoadFile(SystemBase.Base.ProgramWhere.ToString() + "\\" + DllName + "." + FrmName.Substring(0, 6) + ".dll");
                    Form myForm = (Form)System.Activator.CreateInstance(ServiceAssembly.GetType(strJumpFileName5), param);
                    myForm.MdiParent = this.MdiParent;
                    myForm.Show();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "화면 링크"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void lnkJump6_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (strJumpFileName6.Length > 0)
                {
                    string DllName = strJumpFileName6.Substring(0, strJumpFileName6.IndexOf("."));
                    string FrmName = strJumpFileName6.Substring(strJumpFileName6.IndexOf(".") + 1, strJumpFileName6.Length - strJumpFileName6.IndexOf(".") - 1);

                    for (int k = 0; k < this.MdiParent.MdiChildren.Length; k++)
                    {	// 폼이 이미 열려있으면 닫기
                        if (MdiParent.MdiChildren[k].Name == FrmName.Substring(0, 6))
                        {
                            MdiParent.MdiChildren[k].BringToFront();
                            MdiParent.MdiChildren[k].Close();
                            break;
                        }
                    }

                    Link6Exec();

                    Assembly ServiceAssembly = Assembly.LoadFile(SystemBase.Base.ProgramWhere.ToString() + "\\" + DllName + "." + FrmName.Substring(0, 6) + ".dll");
                    Form myForm = (Form)System.Activator.CreateInstance(ServiceAssembly.GetType(strJumpFileName6), param);
                    myForm.MdiParent = this.MdiParent;
                    myForm.Show();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "화면 링크"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 검사분류코드 변경시 발생하는 이벤트
        private void cboInspClassCd_SelectedValueChanged(object sender, EventArgs e)
        {
            InspClassCd();
        }
        #endregion

        #region 검사성적서
        private void btnInspResult_Click(object sender, EventArgs e)
        {
            string strSheetPage1 = "검사성적서";

            string strFileName = SystemBase.Base.ProgramWhere + @"\Report\검사성적서.xls";

            try
            {
                CheckForIllegalCrossThreadCalls = false;

                thread = new Thread(new ThreadStart(Show_Wait));
                thread.Start();
                Thread.Sleep(200);
                Waiting_Form.Activate();

                string strQuery = " usp_QBA011  @pTYPE = 'R1'";
                strQuery += ", @pPLANT_CD = '" + strCode[1] + "' ";
                strQuery += ", @pINSP_CLASS_CD = '" + strCode[2] + "' ";
                strQuery += ", @pITEM_CD = '" + strCode[3] + "' ";

                if (strCode[2] == "P")
                {
                    string[] CODE = null;

                    Regex rx = new Regex("-");
                    CODE = rx.Split(strCode[0]);

                    strQuery += ", @pFIN_INSP_LVL = '*' ";
                    strQuery += ", @pROUT_NO = '" + CODE[0] + "' ";
                    strQuery += ", @pPROC_SEQ = '" + CODE[1] + "' ";
                }
                else if (strCode[2] == "F")
                {
                    strQuery += ", @pFIN_INSP_LVL = '" + strCode[0] + "'";
                    strQuery += ", @pROUT_NO = '*' ";
                    strQuery += ", @pPROC_SEQ = '*' ";
                }

                strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' ";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                strQuery += ", @pBASE_DT = '" + dtpBaseDt.Text + "' ";     // 2015.06.02. hma 추가: 기준일자

                DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

                if (dt.Rows.Count > 0)
                {

                    Waiting_Form.progressBar_temp.Maximum = dt.Rows.Count;

                    string strInspItemCd = "";
                    int strSampleQty = 0;
                    int iTotPage = 0;

                    iTotPage = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(dt.Rows.Count - 8) / Convert.ToDouble("10")));

                    UIForm.VkExcel excel = null;

                    if (File.Exists(strFileName))
                    {
                        File.SetAttributes(strFileName, System.IO.FileAttributes.ReadOnly);
                    }
                    else
                    {
                        // 엑셀 데이터를 생성할 수 없습니다. 원본 파일이 존재하지 않습니다.
                        MessageBox.Show("엑셀 데이터를 생성할 수 없습니다. 원본 파일이 존재하지 않습니다."); ;
                        return;
                    }

                    excel = new UIForm.VkExcel(false);

                    excel.OpenFile(strFileName);
                    // 현재 시트 선택
                    excel.FindExcelWorksheet(strSheetPage1);


                    //데이터수만큼 미리 복사------------------------------------------					

                    for (int i = 0; i < iTotPage; i++)
                    {
                        excel.SetSelect("A28", "A28");
                        excel.RunMacro("PageListAdd");
                    }
                    //------------------------------------------------------

                    // 엑셀쓰기---------------------------------------------------------

                    strInspItemCd = "";
                    int iUseRow = 2;
                    int iRow = 10;
                    int iCol = 6;
                    int j = 0;
                    int[] iAddCol = { 2, 2, 2, 2, 2, 2, 2, 2, 2 };
                    int NextPage = 28;
                    int iPage = 1;

                    // Heard 값
                    excel.SetCell(4, 7, dt.Rows[0]["ITEM_CD"].ToString());
                    excel.SetCell(5, 7, dt.Rows[0]["ITEM_SPEC"].ToString());
                    excel.SetCell(6, 3, dt.Rows[0]["KKJGBH"].ToString());
                    excel.SetCell(7, 3, dt.Rows[0]["ITEM_NM"].ToString());

                    // 2015.06.03. hma 추가(Start): 검사책임자를 넘겨받아서 출력하도록 함.
                    excel.SetCell(7, 15, dt.Rows[0]["QC_MAN_NAME"].ToString());
                    // 2015.06.03. hma 추가(End)

                    for (int i = 0; i < dt.Rows.Count; i++) //내용입력
                    {
                        if (dt.Rows[i]["INSP_ITEM_CD"].ToString() == strInspItemCd)
                        {
                            if (iCol == 24)
                            {
                                if (iRow == NextPage - 2)
                                {
                                    iPage++;
                                    //excel.SetCell(NextPage, 24, dt.Rows[i]["TPAGE"].ToString() + " 매중 " + iPage.ToString() + " 매");
                                    excel.SetCell(NextPage, 3, dt.Rows[i]["PROJECT_NM"].ToString());
                                    excel.SetCell(NextPage, 7, dt.Rows[i]["ITEM_NM"].ToString());
                                    excel.SetCell(NextPage, 15, dt.Rows[i]["ITEM_CD"].ToString());

                                    iRow = NextPage + 3;
                                    NextPage += 23;
                                }
                                else
                                {
                                    iRow += 2;
                                }

                                j = 0;
                                iCol = 6;
                                iUseRow += 2;
                            }
                            else
                            {
                                iCol += iAddCol[j];
                                j++;
                            }
                        }
                        else if (strInspItemCd != "")
                        {
                            strInspItemCd = dt.Rows[i]["INSP_ITEM_CD"].ToString();

                            //int iNextRow = (iRow + ((Convert.ToInt32(dt.Rows[i - 1]["SAMPLE_QTY"].ToString()) * 2) - iUseRow)) + 2;
                            int iNextRow = iRow + 2;

                            if (iNextRow == NextPage)
                            {
                                iRow = iNextRow;
                                iPage++;
                                //excel.SetCell(NextPage, 24, dt.Rows[i]["TPAGE"].ToString() + " 매중 " + iPage.ToString() + " 매");
                                //excel.SetCell(iRow, 3, dt.Rows[i]["PROJECT_NM"].ToString());
                                excel.SetCell(iRow, 7, dt.Rows[i]["ITEM_NM"].ToString());
                                excel.SetCell(iRow, 15, dt.Rows[i]["ITEM_CD"].ToString());
                                iRow += 3;
                                NextPage += 23;

                            }
                            //else if (iNextRow > NextPage)
                            //{
                            //    iPage++;
                            //    //excel.SetCell(NextPage, 24, dt.Rows[i]["TPAGE"].ToString() + " 매중 " + iPage.ToString() + " 매");
                            //    //excel.SetCell(NextPage, 3, dt.Rows[i]["PROJECT_NM"].ToString());
                            //    excel.SetCell(NextPage, 7, dt.Rows[i]["ITEM_NM"].ToString());

                            //    excel.SetCell(NextPage, 15, dt.Rows[i]["ITEM_CD"].ToString());

                            //    iRow = iNextRow + 3;
                            //    NextPage += 23;
                            //}
                            else
                            {
                                iRow = iNextRow;
                            }

                            iCol = 6;
                            j = 0;
                            iUseRow = 2;

                            excel.SetCell(iRow, 1, dt.Rows[i]["INSP_SEQ"].ToString());
                            excel.SetCell(iRow, 2, dt.Rows[i]["INSP_ITEM_NM"].ToString());
                            excel.SetCell(iRow + 1, 2, dt.Rows[i]["MAP_COOR"].ToString());
                            excel.SetCell(iRow, 3, dt.Rows[i]["INSP_SPEC"].ToString().Replace("\r\n", "\n"));
                            excel.SetCell(iRow, 4, dt.Rows[i]["MEASURE_NM"].ToString());
                            excel.SetCell(iRow, 26, dt.Rows[i]["INSP_METH_NM"].ToString());
                            //excel.SetCell(iRow, 26, dt.Rows[i]["AQL"].ToString());
                        }
                        else
                        {
                            strInspItemCd = dt.Rows[i]["INSP_ITEM_CD"].ToString();
                            iRow += 2;
                            iCol = 6;
                            j = 0;

                            excel.SetCell(iRow, 1, dt.Rows[i]["INSP_SEQ"].ToString());
                            excel.SetCell(iRow, 2, dt.Rows[i]["INSP_ITEM_NM"].ToString());
                            excel.SetCell(iRow + 1, 2, dt.Rows[i]["MAP_COOR"].ToString());
                            excel.SetCell(iRow, 3, dt.Rows[i]["INSP_SPEC"].ToString().Replace("\r\n", "\n"));
                            excel.SetCell(iRow, 4, dt.Rows[i]["MEASURE_NM"].ToString());
                            excel.SetCell(iRow, 26, dt.Rows[i]["INSP_METH_NM"].ToString());
                            //excel.SetCell(iRow, 26, dt.Rows[i]["AQL"].ToString());
                        }


                        Waiting_Form.progressBar_temp.Value = i + 1;

                    }
                    Waiting_Form.label_temp.Text = "완료되었습니다.";
                    Thread.Sleep(500);
                    excel.ShowExcel(true);
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "검사성적서출력"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Waiting_Form.Close();
                thread.Abort();
                File.SetAttributes(strFileName, System.IO.FileAttributes.Normal);
            }
        }
        #endregion

        #region 품목사진 뷰어
        private void btnITEM_PICTURE_Click(object sender, EventArgs e)
        {
            WNDW038 pu = new WNDW038(FullFileName);
            pu.ShowDialog();
            if (pu.DialogResult == DialogResult.OK)
            {
            }
        }
        #endregion
    }
}