#region 작성정보
/*********************************************************************/
// 단위업무명 : 선별형검사조건정보
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-02-21
// 작성내용 : 선별형검사조건정보 관리
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
using System.Data.SqlClient;
using WNDW;
using System.Text.RegularExpressions;
using System.Reflection;

namespace QB.QBA014
{
    public partial class QBA014 : UIForm.FPCOMM1
    {
        #region 변수선언
        //팝업을 위한 변수
        string strFinInspLvl = "";	//최종검사레벨

        int NewFlg = 1;//groupBox3 데이터 수정여부 0:삭제, 1:등록, 2:수정\
        int MasterRow = 0; //Master Row

        bool Linked = false;

        string strItemCd = "";
        string strInspClass = "";
        string strPlantCd = "";
        #endregion

        #region 생성자
        public QBA014()
        {
            InitializeComponent();
        }

        public QBA014(string param1, string param2, string param3)
        {
            strItemCd = param1;
            strPlantCd = param2;
            strInspClass = param3;
            Linked = true;

            InitializeComponent();
        }
        #endregion

        #region Form Load
        private void QBA014_Load(object sender, EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            SystemBase.Validation.GroupBox_Setting(groupBox2);
            SystemBase.Validation.GroupBox_Setting(groupBox3);

            //콤보박스 세팅
            //groupBox1
            SystemBase.ComboMake.C1Combo(cboSPlantCd, "usp_B_COMMON @pType='TABLE', @pCODE = 'PLANT_CD', @pNAME = 'PLANT_NM', @pSPEC1 = 'B_PLANT_INFO', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//공장
            SystemBase.ComboMake.C1Combo(cboSInspClassCd, "usp_B_COMMON @pType='COMM', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "',@pCODE = 'Q001', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0); //검사분류코드
            SystemBase.ComboMake.C1Combo(cboSItemAcct, "usp_B_COMMON @pTYPE = 'COMM', @pCODE = 'B036', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 9);	//품목계정
            //groupBox2
            SystemBase.ComboMake.C1Combo(cboPlantCd, "usp_B_COMMON @pType='TABLE', @pCODE = 'PLANT_CD', @pNAME = 'PLANT_NM', @pSPEC1 = 'B_PLANT_INFO', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//공장
            SystemBase.ComboMake.C1Combo(cboInspClassCd, "usp_B_COMMON @pType='COMM', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "',@pCODE = 'Q001', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0); //검사분류코드
            //groupBox3
            SystemBase.ComboMake.C1Combo(cboQualityGuide, "usp_B_COMMON @pType='COMM', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "',@pCODE = 'Q021', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0); //로트품질지표
            SystemBase.ComboMake.C1Combo(cboGuideValueAoql, "usp_B_COMMON @pType='COMM', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "',@pCODE = 'Q022', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 9); //AOQL
            SystemBase.ComboMake.C1Combo(cboGuideValueLtpd, "usp_B_COMMON @pType='COMM', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "',@pCODE = 'Q023', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 9); //LTPD

            //그리드초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

            //기타 세팅
            cboSPlantCd.SelectedValue = SystemBase.Base.gstrPLANT_CD;
            cboPlantCd.SelectedValue = SystemBase.Base.gstrPLANT_CD;

            SystemBase.Validation.GroupBoxControlsLock(groupBox2, true);

            if (Linked == true)
            {
                cboSPlantCd.SelectedValue = strPlantCd;
                cboSInspClassCd.SelectedValue = strInspClass;
                txtSItemCd.Value = strItemCd;
                SearchExec();
            }

            lnkJump1.Text = "조정형검사조건정보";  //화면에 보여지는 링크명
            strJumpFileName1 = "QB.QBA013.QBA013"; //호출할 화면명			

            lnkJump2.Text = "일반검사조건정보";  //화면에 보여지는 링크명
            strJumpFileName2 = "QB.QBA012.QBA012"; //호출할 화면명

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

            SystemBase.Base.RodeFormID = "QBA013";			// 이동할 formid를 적어준다.(메뉴id)
            SystemBase.Base.RodeFormText = "조정형검사조건정보"; 	// 이동할 폼명을 적어준다(메뉴명)			
        }

        protected override void Link2Exec()
        {
            param = Params();

            SystemBase.Base.RodeFormID = "QBA012";			// 이동할 formid를 적어준다.(메뉴id)
            SystemBase.Base.RodeFormText = "일반검사조건정보"; 	// 이동할 폼명을 적어준다(메뉴명)
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
                string strQuery = "usp_QBA014 @pType='C1'";
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
            string Query = "usp_QBA014 @pTYPE = 'T1', @pPLANT_CD = '" + cboPlantCd.SelectedValue.ToString() + "', @pITEM_CD = '" + txtItemCd.Text + "'";
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
                    txtInspItemCd.Value = "";
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
            string Query = "usp_QBA014 @pTYPE = 'T2'";
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
                    string Query = "usp_QBA014 @pTYPE = 'T3'";
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

        #region NewExec()
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            SystemBase.Validation.GroupBox_Reset(groupBox2);
            SystemBase.Validation.GroupBox_Reset(groupBox3);

            fpSpread1.Sheets[0].Rows.Count = 0;

            txtAvgDefRat.Value = 0.00;
        }
        #endregion

        #region DelExec()
        protected override void DeleteExec()
        {
            try
            {
                if (MessageBox.Show(SystemBase.Base.MessageRtn("B0047"), "삭제", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                {
                    NewFlg = 0;
                    SaveExec();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "삭제"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {
                string strQuery = " usp_QBA014  @pTYPE = 'S1'";
                strQuery += ", @pPLANT_CD = '" + cboSPlantCd.SelectedValue.ToString() + "' ";
                strQuery += ", @pINSP_CLASS_CD = '" + cboSInspClassCd.SelectedValue.ToString() + "' ";
                strQuery += ", @pITEM_ACCT = '" + cboSItemAcct.SelectedValue.ToString() + "' ";
                strQuery += ", @pITEM_CD = '" + txtSItemCd.Text + "' ";
                strQuery += ", @pITEM_NM = '" + txtSItemNm.Text + "' ";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0, true);

                if (fpSpread1.Sheets[0].Rows.Count > 0)
                {
                    MasterRow = 0;
                    SubSearch(MasterRow);
                }
                else
                {
                    SystemBase.Validation.GroupBox_Reset(groupBox2);
                    SystemBase.Validation.GroupBox_Reset(groupBox3);

                    fpSpread1.Sheets[0].Rows.Count = 0;

                    txtAvgDefRat.Value = 0.00;
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
        private void SubSearch(int iRow)
        {
            this.Cursor = Cursors.WaitCursor;

            SystemBase.Validation.GroupBox_Reset(groupBox2);
            SystemBase.Validation.GroupBox_Reset(groupBox3);
            txtAvgDefRat.Value = 0.00;

            //groupBox2 데이타 넣기
            cboPlantCd.SelectedValue = fpSpread1.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx1, "공장코드")].Text.Trim();
            cboInspClassCd.SelectedValue = fpSpread1.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx1, "검사분류코드")].Text.Trim();
            txtItemCd.Value = fpSpread1.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text.Trim();
            txtFinInspLvl.Value = fpSpread1.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx1, "최종검사레벨")].Text.Trim();
            txtRoutNo.Value = fpSpread1.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx1, "라우팅번호")].Text.Trim();
            txtProcSeq.Value = fpSpread1.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx1, "공정순번")].Text.Trim();
            txtInspItemCd.Value = fpSpread1.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx1, "검사항목")].Text.Trim();

            //grouBox3 데이타 넣기
            try
            {
                string strQuery = " usp_QBA014  @pTYPE = 'S2'";
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

                DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

                if (dt.Rows.Count > 0)
                {
                    fpSpread1.Sheets[0].Cells[MasterRow, SystemBase.Base.GridHeadIndex(GHIdx1, "등록")].Text = "Y";

                    cboQualityGuide.SelectedValue = dt.Rows[0][0].ToString();

                    if (dt.Rows[0][0].ToString() == "A")
                        cboGuideValueAoql.SelectedValue = dt.Rows[0][1].ToString();
                    else
                        cboGuideValueLtpd.SelectedValue = dt.Rows[0][1].ToString();

                    txtAvgDefRat.Value = dt.Rows[0][2].ToString();

                    NewFlg = 2;
                }
                else
                {
                    fpSpread1.Sheets[0].Cells[MasterRow, SystemBase.Base.GridHeadIndex(GHIdx1, "등록")].Text = "";

                    NewFlg = 1;
                }

                SystemBase.Validation.GroupBoxControlsLock(groupBox2, true);
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }

            fpSpread1.Focus();
            this.Cursor = Cursors.Default;
        }
        #endregion

        #region Master그리드 선택시 상세정보 조회
        private void fpSpread1_LeaveCell(object sender, FarPoint.Win.Spread.LeaveCellEventArgs e)
        {
            if (fpSpread1.Sheets[0].Rows.Count > 0)
            {
                if (e.Row != e.NewRow)
                {
                    try
                    {
                        MasterRow = e.NewRow;

                        //상세정보조회
                        SubSearch(MasterRow);
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

        #region 로트품질지표 변경이벤트
        private void cboQualityGuide_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (cboQualityGuide.SelectedValue.ToString() == "A")
            {
                cboGuideValueAoql.Tag = "AOQL;1;;";
                cboGuideValueLtpd.Tag = ";2;;";
                cboGuideValueLtpd.SelectedValue = "";
            }
            else
            {
                cboGuideValueLtpd.Tag = "AOQL;1;;";
                cboGuideValueAoql.Tag = ";2;;";
                cboGuideValueAoql.SelectedValue = "";
            }
            SystemBase.Validation.GroupBox_Setting(groupBox3);
        }
        #endregion

        #region SaveExec() 데이타 저장 로직
        protected override void SaveExec()
        {
            this.Cursor = Cursors.WaitCursor;

            string ERRCode = "ER", MSGCode = "P0000"; //처리할 내용이 없습니다.
            SqlConnection dbConn = SystemBase.DbOpen.DBCON();
            SqlCommand cmd = dbConn.CreateCommand();
            SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

            try
            {
                //입력폼 필수 체트
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox3))
                {
                    string strGbn = "";

                    if (NewFlg == 1) { strGbn = "I1"; }
                    else if (NewFlg == 2) { strGbn = "U1"; }
                    else if (NewFlg == 0) { strGbn = "D1"; }

                    string strSql = " usp_QBA014 '" + strGbn + "'";
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

                    strSql += ", @pQUALITY_GUIDE = '" + cboQualityGuide.SelectedValue + "'";

                    if (cboQualityGuide.SelectedValue.ToString() == "A")
                        strSql += ", @pGUIDE_VALUE = '" + cboGuideValueAoql.SelectedValue + "'";
                    else
                        strSql += ", @pGUIDE_VALUE = '" + cboGuideValueLtpd.SelectedValue + "'";

                    strSql += ", @pAVG_DEF_RAT = '" + txtAvgDefRat.Text + "'";
                    strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                    strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                    ERRCode = ds.Tables[0].Rows[0][0].ToString();
                    MSGCode = ds.Tables[0].Rows[0][1].ToString();

                    if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프							
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
                SubSearch(MasterRow);
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
            //			}					
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
