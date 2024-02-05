#region 작성정보
/*********************************************************************/
// 단위업무명 : 검사의뢰조회(품질)
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-02-22
// 작성내용 : 검사의뢰조회(품질) 및 관리
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
using FarPoint.Win;
using FarPoint.Win.Spread;
using FarPoint.Win.Spread.CellType;
using System.Threading;
using System.IO;


namespace QE.QRE001
{
    public partial class QRE001 : UIForm.FPCOMM1
    {
        #region 생성자
        public QRE001()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load시
        private void QRE001_Load(object sender, System.EventArgs e)
        { 
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            //콤보박스 세팅
            SystemBase.ComboMake.C1Combo(cboPlantCd, "usp_B_COMMON @pType='TABLE', @pCODE = 'PLANT_CD', @pNAME = 'PLANT_NM', @pSPEC1 = 'B_PLANT_INFO', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//공장
            SystemBase.ComboMake.C1Combo(cboInspClassCd, "usp_B_COMMON @pType='COMM', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "',@pCODE = 'Q001', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0); //검사분류코드
            SystemBase.ComboMake.C1Combo(cboInspStatus, "usp_B_COMMON @pType='COMM2', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "',@pCODE = 'Q003', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0); //검사진행상태
            SystemBase.ComboMake.C1Combo(cboDecisionCd, "usp_B_COMMON @pType='COMM', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "',@pCODE = 'Q004', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 9); //판정

            //SystemBase.ComboMake.C1Combo(cboQaDuty, "usp_B_COMMON @pType='COMM', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "',@pCODE = 'Q030', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 9); //품보담당자 2015.09.01
            SystemBase.ComboMake.C1Combo(cboFIN_INSP_LVL, "usp_B_COMMON @pType='COMM', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "',@pCODE = 'Q013', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 9); //최종검사레벨 2015.09.01


            //그리드초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
            //기타 세팅
            cboPlantCd.SelectedValue = SystemBase.Base.gstrPLANT_CD;
            dtpInspReqDtFr.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-3).ToString().Substring(0,10);
            dtpInspReqDtTo.Value = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0,10);
            dtpInspDtFr.Value = null;
            dtpInspDtTo.Value = null;
            //cboInspStatus.SelectedValue = "Q"; //검사진행상태 기본값 제외 (전체) 2015.09.01
            cboDecisionCd.SelectedValue = "N";  //판정 디폴트값 "미판정" 으로 설정 2015.09.01

        }
        #endregion
        
        #region 조회조건 팝업
        //품목코드
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

                    txtItemCd.Text = Msgs[2].ToString();
                    txtItemNm.Value = Msgs[3].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목코드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        //거래처
        private void btnBpCd_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW002 pu = new WNDW002(txtBpCd.Text, "");
                pu.MaximizeBox = false;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtBpCd.Text = Msgs[1].ToString();
                    txtBpNm.Value = Msgs[2].ToString();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "공급처 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        //프로젝트번호
        private void btnProj2_Click(object sender, System.EventArgs e)
        {
            //strBtn = "Y";
            try
            {
                WNDW007 pu = new WNDW007(txtProjectNo2.Text);
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;
                    txtProjectNo2.Value = Msgs[3].ToString();
                    if (txtProjectSeq2.Text != "*") txtProjectSeq2.Value = "";
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
            //strBtn = "N";
        }

        //프로젝트순번
        private void btnProjSeq2_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON 'PROJ_SEQ', @pSPEC1 = '" + txtProjectNo2.Text + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";										// 쿼리
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };			// 쿼리 인자값(조회조건)
                string[] strSearch = new string[] { "", "" };		// 쿼리 인자값에 들어갈 데이타

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P09001", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "프로젝트차수 조회", false);
                pu.Width = 400;
                pu.ShowDialog();	//공통 팝업 호출

                if (pu.DialogResult == DialogResult.OK)
                {
                    string MSG = pu.ReturnVal.Replace("|", "#");
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(MSG);
                    txtProjectSeq2.Value = Msgs[0].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        //작업장
        private void btnWc_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pType='P042', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pETC = 'P002' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };
                string[] strSearch = new string[] { txtWcCd.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00025", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "작업장 조회");
                pu.Width = 500;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtWcCd.Value = Msgs[0].ToString();
                    txtWcNm.Value = Msgs[1].ToString();
                    txtWcCd.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "작업장 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //품보담당자 2015.09.01 
        private void btnQaDuty_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON 'COMM_POP', @pSPEC1 = 'Q030' ";
                strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                string[] strWhere = new string[] { "@pCODE", "@pNAME" };    // 쿼리 인자값(조회조건)
                string[] strSearch = new string[] { txtQaDuty.Text, "" };       // 쿼리 인자값에 들어갈 데이타

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00055", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "품보담당자 조회");
                pu.ShowDialog();	//공통 팝업 호출

                if (pu.DialogResult == DialogResult.OK)
                {
                    string MSG = pu.ReturnVal.Replace("|", "#");
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(MSG);

                    txtQaDuty.Text = Msgs[0].ToString();
                    txtQaDutyNm.Value = Msgs[1].ToString();
                    txtQaDuty.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품보담당자 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //제품코드
        private void btnGroupCd_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW005 pu = new WNDW005("FS1", "10", txtGroupCd.Text);
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtGroupCd.Value = Msgs[2].ToString();
                    txtGroupNm.Value = Msgs[3].ToString();
                    txtGroupCd.Focus();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목코드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 작업명 팝업
        private void btnJob_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON 'COMM_POP', @pSPEC1 = 'P001', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' "; 	// 쿼리
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };    // 쿼리 인자값(조회조건)
                string[] strSearch = new string[] { txtJobCd.Text, "" };    // 쿼리 인자값에 들어갈 데이타

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("PBA122P", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "작업 조회", false);
                pu.Width = 400;
                pu.ShowDialog();	//공통 팝업 호출

                if (pu.DialogResult == DialogResult.OK)
                {
                    string MSG = pu.ReturnVal.Replace("|", "#");
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(MSG);
                    txtJobCd.Text = Msgs[0].ToString();
                    txtJobNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        //사업코드
        private void btnEntCd_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON @pTYPE ='TABLE_POP', @pSPEC1 = 'ENT_CD', @pSPEC2 = 'ENT_NM', @pSPEC3 = 'S_ENTERPRISE_INFO', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtEntCd.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00007", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "사업 조회");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtEntCd.Text = Msgs[0].ToString();
                    txtEntNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "사업코드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //검사원
        private void btnInspectorCd_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON 'COMM_POP' ,@pSPEC1='Q005', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtInspectorCd.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00067", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "검사원 팝업");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {

                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtInspectorCd.Text = Msgs[0].ToString();
                    txtInspectorNm.Value = Msgs[1].ToString();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        //검사의뢰번호
        private void btnInspReqNo_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW009 pu = new WNDW009(Convert.ToString(cboPlantCd.SelectedValue)
                                                        , txtInspReqNo.Text
                                                        , Convert.ToString(cboInspClassCd.SelectedValue)
                                                        , Convert.ToString(cboInspStatus.SelectedValue));
                pu.MaximizeBox = false;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtInspReqNo.Text = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "검사의뢰번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //제조오더번호
        private void btnWorkOrderNo_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW006 pu = new WNDW006(txtWorkOrderNo.Text);
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtWorkOrderNo.Text = Msgs[1].ToString();
                    txtWorkOrderNo.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "제조오더번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 조회조건 TextChanged

        //작업장
        private void txtWcCd_TextChanged(object sender, EventArgs e)
        {
            txtWcNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtWcCd.Text, " AND MAJOR_CD = 'P002'  AND COMP_CODE ='" + SystemBase.Base.gstrCOMCD + "'");
        }


        // 작업코드 입력
        private void txtJobCd_TextChanged(object sender, EventArgs e)
        {
            try
            {
                txtJobNm.Value = !string.IsNullOrEmpty(txtJobCd.Text) ? SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtJobCd.Text, " AND MAJOR_CD = 'P001' AND COMP_CODE='" + SystemBase.Base.gstrCOMCD + "'") : "";
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "작업코드 조회"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        //품목코드
        private void txtItemCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtItemCd.Text != "")
                {
                    txtItemNm.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtItemCd.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
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

        
        //품보담당자 2015.09.01
        private void txtQaDuty_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtQaDuty.Text != "")
                {
                    txtQaDutyNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtQaDuty.Text, " AND MAJOR_CD = 'Q030'  AND COMP_CODE = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtQaDutyNm.Value = "";
                }
            }
            catch
            {
            }
        }

        //거래처
        private void txtBpCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtBpCd.Text != "")
                {
                    txtBpNm.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtBpCd.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtBpNm.Value = "";
                }
            }
            catch
            {

            }
        }

        //제품코드
        private void txtGroupCd_TextChanged(object sender, System.EventArgs e)
        {

        }


        //사업코드
        private void txtEntCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtEntCd.Text != "")
                {
                    txtEntNm.Value = SystemBase.Base.CodeName("ENT_CD", "ENT_NM", "S_ENTERPRISE_INFO", txtEntCd.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtEntNm.Value = "";
                }
            }
            catch
            {

            }
        }

        //검사원
        private void txtInspectorCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtInspectorCd.Text != "")
                {
                    txtInspectorNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtInspectorCd.Text, " AND MAJOR_CD = 'Q005' AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND COMP_CODE ='" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtInspectorNm.Value = "";
                }
            }
            catch
            {

            }
        }
        #endregion

        #region fpSpread1 ButtonClicked  팝업
        private void fpSpread1_ButtonClicked(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            if (e.Row >= 0)
            {
                if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "증빙자료확인_2"))
                {
                    fpSpread1.Sheets[0].RowHeader.Cells[e.Row, 0].Text = "";
                    fpSpread1.Sheets[0].RowHeader.Rows[e.Row].BackColor = SystemBase.Base.Color_Org;

                    if (fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "증빙자료확인")].Text != "")
                    {
                        string strMvmtNo = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "제조오더/입고번호")].Text;
                        string strMvmtSeq = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "공정/순번")].Text; 
                        
                        QRE001P1 frm1 = new QRE001P1(cboPlantCd.SelectedValue.ToString(), strMvmtNo, strMvmtSeq, cboInspClassCd.SelectedValue.ToString()); //, strProjectNo, strProjectSeq, strItemCd );
                        frm1.ShowDialog();
                    }
                }
                
            }
        }
        #endregion


        #region NewExec()
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            //그리드초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

            //기타 세팅
            cboPlantCd.SelectedValue = SystemBase.Base.gstrPLANT_CD;
            dtpInspReqDtFr.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-3).ToString().Substring(0,10);
            dtpInspReqDtTo.Value = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0, 10);
            dtpInspDtFr.Value = null;
            dtpInspDtTo.Value = null;
            cboInspStatus.SelectedValue = "Q";
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {
                string strGbn = "";

                switch (cboInspClassCd.SelectedValue.ToString())
                {
                    case "R": strGbn = "S11"; break;    // 2020.06.04. hma 수정: S1 => S11로 변경
                    case "S": strGbn = "S11"; break;    // 2020.06.04. hma 수정: S1 => S11로 변경
                    case "P": strGbn = "S21"; break;    // 2020.06.04. hma 수정: S2 => S21로 변경
                    case "F": strGbn = "S31"; break;    // 2020.06.04. hma 수정: S3 => S31로 변경
                    default: strGbn = ""; break;
                }

                string strQuery = " usp_QRE001  @pTYPE = '" + strGbn + "'";
                strQuery += ", @pPLANT_CD = '" + cboPlantCd.SelectedValue.ToString() + "' ";
                strQuery += ", @pINSP_REQ_DT_FR = '" + dtpInspReqDtFr.Text + "' ";
                strQuery += ", @pINSP_REQ_DT_TO = '" + dtpInspReqDtTo.Text + "' ";
                strQuery += ", @pINSP_CLASS_CD = '" + cboInspClassCd.SelectedValue.ToString() + "'";
                strQuery += ", @pINSP_DT_FR = '" + dtpInspDtFr.Text + "' ";
                strQuery += ", @pINSP_DT_TO = '" + dtpInspDtTo.Text + "' ";
                strQuery += ", @pITEM_CD = '" + txtItemCd.Text + "' ";
                strQuery += ", @pBP_CD = '" + txtBpCd.Text + "' ";
                strQuery += ", @pINSP_STATUS = '" + cboInspStatus.SelectedValue.ToString() + "' ";
                strQuery += ", @pDECISION_CD = '" + cboDecisionCd.SelectedValue.ToString() + "'";
                strQuery += ", @pPROJECT_NO = '" + txtProjectNo2.Text + "'";
                strQuery += ", @pINSP_REQ_NO = '" + txtInspReqNo.Text + "'";
                strQuery += ", @pENT_CD = '" + txtEntCd.Text + "'";
                strQuery += ", @pINSPECTOR_CD = '" + txtInspectorCd.Text + "'";
                strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'";
                strQuery += ", @pWORKORDER_NO = '" + txtWorkOrderNo.Text + "'";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                strQuery += ", @pPROJECT_SEQ = '" + txtProjectSeq2.Text + "'";              //프로젝트차수 2015.09.01
                strQuery += ", @pGROUP_CD = '" + txtGroupCd.Text + "'";                     //제품코드 2015.09.01
                strQuery += ", @pQADUTY = '" + txtQaDuty.Text + "'";    //품보담당자 2015.09.01
                strQuery += ", @pJOB_CD = '" + txtJobCd.Text + "'";                         //작업명 2015.09.01
                strQuery += ", @pWC_CD = '" + txtWcCd.Text + "'";                           //작업장 2015.09.01
                strQuery += ", @pFIN_INSP_LVL = '" + cboFIN_INSP_LVL.SelectedValue.ToString() + "'";    //최종검사레벨 2015.09.01
                strQuery += ", @pFINAL_DT_FR = '" + dtpFinalDtFr.Text + "'";                //최종납기일자 2015.09.01
                strQuery += ", @pFINAL_DT_TO = '" + dtpFinalDtTo.Text + "'";                //최종납기일자 2015.09.01

                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);

                //if (fpSpread1.Sheets[0].Rows.Count > 0)
                //{
                //    //그리드 Header Text 입력
                //    string strText = "";

                //    if (strGbn == "S1")
                //    { 
                //        strText = "거래처";
                //    }
                //    else if (strGbn == "S2")
                //    { 
                //        strText = "제조오더번호";
                //        fpSpread1.Sheets[0].Columns[SystemBase.Base.GridHeadIndex(GHIdx1, "공정명")].Visible = true;
                //    }
                //    else
                //    { 
                //        strText = "최종검사레벨";
                //    }

                //    fpSpread1.Sheets[0].ColumnHeader.Cells[0, 5].Text = strText;
                //}
                //else
                //{
                //    fpSpread1.Sheets[0].Rows.Count = 0;
                //}

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

        #region SaveExec() 폼에 입력된 데이타 저장 로직
        protected override void SaveExec()
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;


            if (SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", true) == true) // 그리드 상단 필수항목 체크
            {
                string ERRCode = "ER", MSGCode = "P0000";	//처리할 내용이 없습니다.
                string strKeyCd = "";

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
                                default: strGbn = ""; break;
                            }

                            string strINSP_REQ_NO = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사의뢰번호")].Text.ToString();
                            string strINSP_DELAY_REASON = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사지연사유")].Text;
                            string strDOC_DELAY_REASON = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품질증빙지연사유")].Text;

                            string strSql = " usp_QRE001 '" + strGbn + "'";
                            strSql = strSql + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                            strSql = strSql + ", @pINSP_REQ_NO = '" + strINSP_REQ_NO + "'";
                            strSql = strSql + ", @pINSP_DELAY_REASON = '" + strINSP_DELAY_REASON + "'";
                            strSql = strSql + ", @pDOC_DELAY_REASON = '" + strDOC_DELAY_REASON + "'";

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
                    SearchExec();
                    UIForm.FPMake.GridSetFocus(fpSpread1, strKeyCd); //그리드 위치를 가져온다

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
            this.Cursor = System.Windows.Forms.Cursors.Default;
        }
        #endregion


    }
}
