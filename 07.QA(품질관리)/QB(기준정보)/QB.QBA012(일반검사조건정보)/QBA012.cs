#region 작성정보
/*********************************************************************/
// 단위업무명 : 일반검사조건정보
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-02-20
// 작성내용 : 일반검사조건정보 관리
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

namespace QB.QBA012
{
    public partial class QBA012 : UIForm.FPCOMM2
    {
        #region 변수선언
        //팝업을 위한 변수
        string strFinInspLvl = "";	//최종검사레벨

        int NewFlg = 1;//마스터 그리드 조회여부 0:0포커스조회, 1:로우조회
        int MasterRow = 0; //Master Row
        int MasterColumn = 0; //Master Column
        string MasterRowKey = "";	//로우 찾을 키
        string strInspSeries = "1";	//차수값
        string strKey = "";

        bool Linked = false;

        string strItemCd = "";
        string strInspClass = "";
        string strPlantCd = "";
        #endregion

        #region 생성자
        public QBA012()
        {
            InitializeComponent();
        }

        public QBA012(string param1, string param2, string param3)
        {
            strItemCd = param1;
            strPlantCd = param2;
            strInspClass = param3;
            Linked = true;
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void QBA012_Load(object sender, System.EventArgs e)
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

            //그리드초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
            UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, 0, 0, false);

            //기타 세팅
            cboSPlantCd.SelectedValue = SystemBase.Base.gstrPLANT_CD;
            cboPlantCd.SelectedValue = SystemBase.Base.gstrPLANT_CD;
            SystemBase.Validation.GroupBoxControlsLock(groupBox2, true);

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

            lnkJump3.Text = "품목별검사기준정보";  //화면에 보여지는 링크명
            strJumpFileName3 = "QB.QBA011.QBA011"; //호출할 화면명			
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

            SystemBase.Base.RodeFormID = "QBA011";			// 이동할 formid를 적어준다.(메뉴id)
            SystemBase.Base.RodeFormText = "품목별검사기준정보"; 	// 이동할 폼명을 적어준다(메뉴명)
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

        //검사항목
        private void btnInspItemCd_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = "usp_QBA012 @pType='C1'";
                strQuery += ", @pPLANT_CD ='" + cboPlantCd.SelectedValue + "'";
                strQuery += ", @pINSP_CLASS_CD ='" + cboInspClassCd.SelectedValue + "'";
                strQuery += ", @pITEM_CD ='" + txtItemCd.Text + "'";

                if (txtFinInspLvl.Text != "")
                    strQuery += ", @pFIN_INSP_LVL ='" + txtFinInspLvl.Text + "'";

                if (txtRoutNo.Text != "")
                    strQuery += ", @pROUT_NO ='" + txtRoutNo.Text + "'";

                if (txtProcSeq.Text != "")
                    strQuery += ", @pPROC_SEQ ='" + txtProcSeq.Text + "'";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtInspItemCd.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P06007", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "검사항목 조회");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtInspItemCd.Value = Msgs[0].ToString();
                    txtInspItemNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "검사항목 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }
        #endregion

        #region groupBox2 TextChanged
        //품목코드
        private void txtItemCd_TextChanged(object sender, System.EventArgs e)
        {
            string Query = "usp_QBA012 @pTYPE = 'T1', @pPLANT_CD = '" + cboPlantCd.SelectedValue.ToString() + "', @pITEM_CD = '" + txtItemCd.Text + "'";
            Query += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

            DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);

            if (dt.Rows.Count > 0)
            {
                txtItemNm.Value = dt.Rows[0][0].ToString();
                strFinInspLvl = dt.Rows[0][1].ToString();
            }
            else
            {
                txtItemNm.Value = "";
                txtInspItemCd.Value = "";
            }
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
                        txtFinInspLvlNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtFinInspLvl.Text, "  AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND MAJOR_CD = 'Q013' AND MINOR_CD <= '9' AND COMP_CODE ='" + SystemBase.Base.gstrCOMCD + "'");
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
                        txtFinInspLvlNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtFinInspLvl.Text, "  AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND MAJOR_CD = 'Q013' AND MINOR_CD <= '" + strFinInspLvl + "' AND COMP_CODE ='" + SystemBase.Base.gstrCOMCD + "'");
                    }
                    else
                    {
                        txtFinInspLvlNm.Value = "";
                    }
                
                }
                if (txtFinInspLvlNm.Text == "")
                {
                    txtInspItemCd.Text = "";
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
            string Query = "usp_QBA012 @pTYPE = 'T2'";
            Query += ", @pPLANT_CD = '" + cboPlantCd.SelectedValue.ToString() + "'";
            Query += ", @pITEM_CD = '" + txtItemCd.Text + "'";
            Query += ", @pROUT_NO = '" + txtRoutNo.Text + "'";
            Query += ", @pPROC_SEQ = '" + txtProcSeq.Text + "'";
            Query += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

            DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);

            if (dt.Rows.Count > 0)
                txtProcSeqNm.Value = dt.Rows[0][0].ToString();
            else
            {
                txtProcSeqNm.Value = "";
                txtInspItemCd.Value = "";
            }

        }

        //검사항목
        private void txtInspItemCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtInspItemCd.Text != "")
                {
                    txtInspItemNm.Value = SystemBase.Base.CodeName("INSP_ITEM_CD", "INSP_ITEM_NM", "Q_BAS_INSPECTION_ITEM", txtInspItemCd.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtInspItemNm.Value = "";
                }
                if (txtInspItemNm.Text != "")
                {
                    string Query = "usp_QBA012 @pTYPE = 'T3'";
                    Query += ", @pPLANT_CD = '" + cboPlantCd.SelectedValue.ToString() + "'";
                    Query += ", @pINSP_CLASS_CD = '" + cboInspClassCd.SelectedValue.ToString() + "'";
                    Query += ", @pITEM_CD = '" + txtItemCd.Text + "'";

                    if (txtFinInspLvl.Text != "")
                        Query += ", @pFIN_INSP_LVL = '" + txtFinInspLvl.Text + "'";

                    if (txtRoutNo.Text != "")
                        Query += ", @pROUT_NO = '" + txtRoutNo.Text + "'";

                    if (txtProcSeq.Text != "")
                        Query += ", @pPROC_SEQ = '" + txtProcSeq.Text + "'";

                    Query += ", @pINSP_ITEM_CD = '" + txtInspItemCd.Text + "'";
                    Query += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);

                    if (dt.Rows.Count > 0)
                    {
                        txtInspMethCd.Value = dt.Rows[0][0].ToString();
                        txtInspMethNm.Value = dt.Rows[0][1].ToString();
                    }
                    else
                    {
                        txtInspMethCd.Value = "";
                        txtInspMethNm.Value = "";
                    }
                }
                else
                {
                    txtInspMethCd.Value = "";
                    txtInspMethNm.Value = "";
                }
            }
            catch
            {

            }            
        }
        #endregion
                
        #region 검사분류코드 변경시 발생하는 함수
        private void InspClassCd()
        {
            txtFinInspLvl.Value = "";
            txtRoutNo.Value = "";
            txtProcSeq.Value = "";
            txtInspMethCd.Value = "";
            txtInspMethNm.Value = "";
            txtInspItemCd.Value = "";

            if (txtItemNm.Text != "" && cboInspClassCd.SelectedValue.ToString() == "F")
            {
                txtRoutNo.BackColor = SystemBase.Validation.Kind_Gainsboro ;
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
                txtFinInspLvl.Tag = "최종검사라벨;1;;";

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

        #region NewExec()
        protected override void NewExec()
        {
            try
            {
                SystemBase.Validation.GroupBox_Reset(groupBox2);

                //그리드초기화
                fpSpread1.Sheets[0].Rows.Count = 0;
                fpSpread2.Sheets[0].Rows.Count = 0;

                //기타 세팅
                InspClassCd();
                cboSPlantCd.Focus();

                NewFlg = 1;
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
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
                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Text == "")
                        { iValue = 0; }
                        else { iValue = Convert.ToInt32(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Value); }

                        if (ibig < iValue)
                        { ibig = iValue; }
                    }
                }

                UIForm.FPMake.RowInsert(fpSpread1);

                fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Value = ibig + 1;
                fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "시료수")].Value = 0;
                fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "합격판정개수")].Value = 0;
                fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "불합격판정개수")].Value = 0;
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
                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Text == "")
                        { iValue = 0; }
                        else { iValue = Convert.ToInt32(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Value); }

                        if (ibig < iValue)
                        { ibig = iValue; }
                    }

                    UIForm.FPMake.RowCopy(fpSpread1);
                    fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Value = ibig + 1;
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
        { NewFlg = 0; Grid_Search(); }
        #endregion

        #region 그리드조회
        private void Grid_Search()
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {
                string strQuery = " usp_QBA012  @pTYPE = 'S1'";
                strQuery += ", @pPLANT_CD = '" + cboSPlantCd.SelectedValue.ToString() + "' ";
                strQuery += ", @pINSP_CLASS_CD = '" + cboSInspClassCd.SelectedValue.ToString() + "' ";
                strQuery += ", @pITEM_ACCT = '" + cboSItemAcct.SelectedValue.ToString() + "' ";
                strQuery += ", @pITEM_CD = '" + txtSItemCd.Text + "' ";
                strQuery += ", @pITEM_NM = '" + txtSItemNm.Text + "' ";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread2, strQuery, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, false, 0, 0, true);

                if (fpSpread2.Sheets[0].Rows.Count > 0)
                {
                    fpSpread2.Sheets[0].Rows[0, fpSpread2.Sheets[0].Rows.Count - 1].Height = 30;

                    if (NewFlg == 0)
                    {
                        MasterRow = 0;
                        SubSearch();
                    }
                }
                else
                {
                    SystemBase.Validation.GroupBox_Reset(groupBox2);

                    //그리드초기화
                    fpSpread1.Sheets[0].Rows.Count = 0;

                    //기타 세팅
                    InspClassCd();
                    cboSPlantCd.Focus();

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

        #region 상세정보 조회
        private void SubSearch()
        {
            this.Cursor = Cursors.WaitCursor;
            //그리드 조회
            try
            {
                SystemBase.Validation.GroupBox_Reset(groupBox2);
                fpSpread1.Sheets[0].Rows.Count = 0;

                ////groupBox2 데이타 넣키
                cboPlantCd.SelectedValue = fpSpread2.Sheets[0].Cells[MasterRow, SystemBase.Base.GridHeadIndex(GHIdx2, "공장코드")].Text.Trim();
                cboInspClassCd.SelectedValue = fpSpread2.Sheets[0].Cells[MasterRow, SystemBase.Base.GridHeadIndex(GHIdx2, "검사분류코드")].Text.Trim();
                txtItemCd.Value = fpSpread2.Sheets[0].Cells[MasterRow, SystemBase.Base.GridHeadIndex(GHIdx2, "품목코드")].Text.Trim();
                txtFinInspLvl.Value = fpSpread2.Sheets[0].Cells[MasterRow, SystemBase.Base.GridHeadIndex(GHIdx2, "최종검사레벨")].Text.Trim();
                txtRoutNo.Value = fpSpread2.Sheets[0].Cells[MasterRow, SystemBase.Base.GridHeadIndex(GHIdx2, "라우팅번호")].Text.Trim();
                txtProcSeq.Value = fpSpread2.Sheets[0].Cells[MasterRow, SystemBase.Base.GridHeadIndex(GHIdx2, "공정순번")].Text.Trim();
                txtInspItemCd.Value = fpSpread2.Sheets[0].Cells[MasterRow, SystemBase.Base.GridHeadIndex(GHIdx2, "검사항목")].Text.Trim();
                strKey = fpSpread2.Sheets[0].Cells[MasterRow, SystemBase.Base.GridHeadIndex(GHIdx2, "KEY")].Text.Trim();


                string strQuery = " usp_QBA012  @pTYPE = 'S2'";
                strQuery += ", @pPLANT_CD = '" + cboPlantCd.SelectedValue.ToString() + "' ";
                strQuery += ", @pINSP_CLASS_CD = '" + cboInspClassCd.SelectedValue.ToString() + "' ";
                strQuery += ", @pITEM_CD = '" + txtItemCd.Text + "' ";

                if (txtFinInspLvl.Text != "")
                    strQuery += ", @pFIN_INSP_LVL = '" + txtFinInspLvl.Text + "' ";

                if (txtRoutNo.Text != "")
                    strQuery += ", @pROUT_NO = '" + txtRoutNo.Text + "' ";

                if (txtProcSeq.Text != "")
                    strQuery += ", @pPROC_SEQ = '" + txtProcSeq.Text + "' ";

                strQuery += ", @pINSP_ITEM_CD = '" + txtInspItemCd.Text + "' ";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);

                if (fpSpread1.Sheets[0].Rows.Count > 0)
                {
                    fpSpread2.Sheets[0].Cells[MasterRow, SystemBase.Base.GridHeadIndex(GHIdx2, "등록")].Text = "Y";
                }
                else
                {
                    fpSpread2.Sheets[0].Cells[MasterRow, SystemBase.Base.GridHeadIndex(GHIdx2, "등록")].Text = "";
                }
                SystemBase.Validation.GroupBoxControlsLock(groupBox2, true);
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }

            fpSpread2.Focus();
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
                        MasterRow = e.NewRow;

                        //상세정보조회
                        SubSearch();
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

        #region SaveExec() 데이타 저장 로직
        protected override void SaveExec()
        {
            fpSpread1.Focus();

            if (fpSpread1.Sheets[0].Rows.Count > 0)
            {
                this.Cursor = Cursors.WaitCursor;

                //품목코드 유효성 체크
                if (txtItemNm.Text == "")
                {
                    //존재하지 않는 품목코드 코드입니다
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0036", "품목"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtItemCd.Focus();

                    this.Cursor = Cursors.Default;
                    return;
                }

                //검사방식 유효성 체크
                if (txtInspMethCd.Text == "" && txtInspMethNm.Text == "")
                {

                    MessageBox.Show("존재하지 않는 데이터입니다!", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtInspItemCd.Focus();

                    this.Cursor = Cursors.Default;
                    return;

                }

                string ERRCode = "ER", MSGCode = "P0000"; //처리할 내용이 없습니다.
                string strINSP_SERIES = "";
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
                            //입력데이터 유효성 체크
                            if (Convert.ToDouble(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "시료수")].Value) <= 0)
                            {
                                MessageBox.Show("시료수는 0 보다 큰 값이어야합니다!", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                fpSpread1.ActiveSheet.SetActiveCell(i, SystemBase.Base.GridHeadIndex(GHIdx1, "시료수"));

                                Trans.Rollback();
                                this.Cursor = Cursors.Default;
                                return;
                            }

                            if (txtInspMethCd.Text.Substring(0, 1) != "2")
                            {
                                if (Convert.ToDouble(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "합격판정개수")].Value)
                                    >= Convert.ToDouble(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "시료수")].Value))
                                {
                                    MessageBox.Show("합격판정개수는 시료수 보다 같거나 클수 없습니다!", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    fpSpread1.ActiveSheet.SetActiveCell(i, SystemBase.Base.GridHeadIndex(GHIdx1, "합격판정개수"));

                                    Trans.Rollback();
                                    this.Cursor = Cursors.Default;
                                    return;
                                }

                                if (Convert.ToDouble(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "합격판정개수")].Value)
                                    >= Convert.ToDouble(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "불합격판정개수")].Value))
                                {
                                    MessageBox.Show("합격판정개수는 불합격판정개수 보다 같거나 클수 없습니다!", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    fpSpread1.ActiveSheet.SetActiveCell(i, SystemBase.Base.GridHeadIndex(GHIdx1, "합격판정개수"));

                                    Trans.Rollback();
                                    this.Cursor = Cursors.Default;
                                    return;
                                }

                                if (Convert.ToDouble(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "불합격판정개수")].Value)
                                    > Convert.ToDouble(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "시료수")].Value))
                                {
                                    MessageBox.Show("불합격판정개수는 시료수 보다 클수 없습니다!", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    fpSpread1.ActiveSheet.SetActiveCell(i, SystemBase.Base.GridHeadIndex(GHIdx1, "불합격판정개수"));

                                    Trans.Rollback();
                                    this.Cursor = Cursors.Default;
                                    return;
                                }
                            }
                            else if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "합격판정계수")].Text == ""
                                && fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "최대허용불량률")].Text == "")
                            {
                                MessageBox.Show("합격판정계수, 최대허용불량률을 입력하세요!", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);

                                if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "합격판정계수")].Text == "")
                                {
                                    fpSpread1.ActiveSheet.SetActiveCell(i, SystemBase.Base.GridHeadIndex(GHIdx1, "합격판정계수"));
                                }
                                else
                                {
                                    fpSpread1.ActiveSheet.SetActiveCell(i, SystemBase.Base.GridHeadIndex(GHIdx1, "최대허용불량률"));
                                }

                                Trans.Rollback();
                                this.Cursor = Cursors.Default;
                                return;
                            }

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

                                string strSql = " usp_QBA012 '" + strGbn + "'";
                                strSql += ", @pPLANT_CD = '" + cboPlantCd.SelectedValue + "'";
                                strSql += ", @pINSP_CLASS_CD = '" + cboInspClassCd.SelectedValue + "'";
                                strSql += ", @pITEM_CD = '" + txtItemCd.Text + "'";

                                if (txtFinInspLvl.Text != "")
                                    strSql += ", @pFIN_INSP_LVL = '" + txtFinInspLvl.Text + "'";

                                if (txtRoutNo.Text != "")
                                    strSql += ", @pROUT_NO = '" + txtRoutNo.Text + "'";

                                if (txtProcSeq.Text != "")
                                    strSql += ", @pPROC_SEQ = '" + txtProcSeq.Text + "'";

                                if (txtInspItemCd.Text != "")
                                    strSql += ", @pINSP_ITEM_CD = '" + txtInspItemCd.Text + "'";

                                strINSP_SERIES = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Text;

                                strSql += ", @pINSP_SERIES = '" + strINSP_SERIES + "'";

                                if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "시료수")].Text != "")
                                    strSql += ", @pSAMPLE_QTY = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "시료수")].Value + "'";

                                if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "합격판정개수")].Text != "")
                                    strSql += ", @pACC_DEC_QTY = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "합격판정개수")].Value + "'";

                                if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "불합격판정개수")].Text != "")
                                    strSql += ", @pREJ_DEC_QTY = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "불합격판정개수")].Value + "'";

                                if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "합격판정계수")].Text != "")
                                    strSql += ", @pACC_DEC_FAC = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "합격판정계수")].Value + "'";

                                if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "최대허용불량률")].Text != "")
                                    strSql += ", @pMAX_DEF_RAT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "최대허용불량률")].Value + "'";

                                strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                                strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

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
                    SubSearch();
                    UIForm.FPMake.GridSetFocus(fpSpread1, strINSP_SERIES, SystemBase.Base.GridHeadIndex(GHIdx1, "차수"));
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

        #region Master 그리드 Focus
        private void Mastergrd_Grid()
        {
            try
            {
                fpSpread2.Search(0, MasterRowKey.Trim(), true, true, true, true, 0, SystemBase.Base.GridHeadIndex(GHIdx2, "KEY"), ref MasterRow, ref MasterColumn);

                fpSpread2.Focus();
                fpSpread2.ActiveSheet.SetActiveCell(MasterRow, 1); //Row Focus		
                fpSpread2.ShowRow(0, MasterRow, FarPoint.Win.Spread.VerticalPosition.Center); //Focus Row 보기

            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString());
            }
        }
        #endregion

        #region 그리드 Change
        private void fpSpread1_Change(object sender, FarPoint.Win.Spread.ChangeEventArgs e)
        {
            if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "차수"))
            {
                string NowValue = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Text;

                if (fpSpread1.Sheets[0].Rows.Count > 0)
                {
                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {
                        if (NowValue == fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Text && e.Row != i)
                        {
                            MessageBox.Show("검사순서는 동일한 값을 입력할 수 없습니다!", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            fpSpread1.ActiveSheet.SetActiveCell(e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "차수"));

                            NowValue = "";
                            fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Text = strInspSeries;

                            if (fpSpread1.Sheets[0].RowHeader.Cells[e.Row, 0].Text != "I")
                                fpSpread1.Sheets[0].RowHeader.Cells[e.Row, 0].Text = "";

                        }

                        if (NowValue == "0")
                        {
                            MessageBox.Show("0 보다 커야합니다!", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);

                            NowValue = "";
                            fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Text = strInspSeries;

                            if (fpSpread1.Sheets[0].RowHeader.Cells[e.Row, 0].Text != "I")
                                fpSpread1.Sheets[0].RowHeader.Cells[e.Row, 0].Text = "";
                        }
                    }
                }
            }
        }
        #endregion

        #region 그리드 차수값
        private void fpSpread1_CellClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            if (fpSpread1.Sheets[0].Rows.Count > 0)
                strInspSeries = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Text;
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

                    Assembly ServiceAssembly = Assembly.LoadFile(SystemBase.Base.ProgramWhere.ToString() + "\\" + DllName + "." + FrmName.Substring(0, 6) + ".dll");
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
        
    }
}