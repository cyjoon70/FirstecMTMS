#region 작성정보
/*********************************************************************/
// 단위업무명 : APS스케줄별 조회
// 작 성 자 : 권 순 철
// 작 성 일 : 2013-04-08
// 작성내용 : APS스케줄별 조회 및 관리
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
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using PlexityHide.GTP;
using System.Globalization;
using WNDW;

namespace PB.PSB023
{
    public partial class PSB023 : UIForm.Buttons
    {
        public PSB023()
        {
            InitializeComponent();
        }

        #region Form Load시
        private void PSB023_Load(object sender, System.EventArgs e)
        {
            //필수 체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            dtpStartDt.Text = DateTime.Now.ToShortDateString();
            dtpEndDt.Text = DateTime.Now.AddMonths(1).ToShortDateString();

            gantt1.GridProperties.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;					// BorderStyle
            this.gantt1.DateScaler.CultureInfoDateTimeFormat = new CultureInfo("ko-KR", false).DateTimeFormat; // 날짜 포맷
            this.gantt1.VerticalDayStripes = true;

            txtPLANT_CD.Text = SystemBase.Base.gstrPLANT_CD;
        }
        #endregion

        #region NewExec
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            dtpStartDt.Text = DateTime.Now.ToShortDateString();
            dtpEndDt.Text = DateTime.Now.AddMonths(1).ToShortDateString();

            txtPLANT_CD.Text = SystemBase.Base.gstrPLANT_CD;
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            try
            {
                if (SystemBase.Base.GroupBoxExceptions(groupBox1))
                {
                    string strSql;

                    strSql = "usp_PSB023 'S1'";
                    if (optSort1.Checked == true)
                        strSql += ", @pSORT ='P' ";
                    else
                        strSql += ", @pSORT ='R' ";

                    strSql += ", @pPLANT_CD ='" + txtPLANT_CD.Text + "' ";
                    strSql += ", @pWC_CD = '" + txtWcCd.Text + "' ";
                    strSql += ", @pPROJ_NO = '" + txtPROJ_NO.Text + "' ";
                    strSql += ", @pPROJECT_SEQ = '" + txtProjectSeq.Text + "' ";
                    strSql += ", @pSTART_DT = '" + dtpStartDt.Text.ToString() + "' ";
                    strSql += ", @pEND_DT = '" + dtpEndDt.Text.ToString() + "' ";
                    strSql += ", @pGROUP_CD = '" + txtItemCd.Text.ToString() + "' ";
                    strSql += ", @pMAKEORDER_NO = '" + txtMakeOrderNo.Text.ToString() + "' ";
                    strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";		
                    DataTable dt = SystemBase.DbOpen.NoTranDataTable(strSql);

                    /******************************프로젝트 별 색깔 주기위해***********************************/
                    string strSql2 = "usp_PSB023 'S2'";
                    strSql2 += ", @pPLANT_CD ='" + txtPLANT_CD.Text + "' ";
                    strSql2 += ", @pWC_CD = '" + txtWcCd.Text + "' ";
                    strSql2 += ", @pPROJ_NO = '" + txtPROJ_NO.Text + "' ";
                    strSql2 += ", @pPROJECT_SEQ = '" + txtProjectSeq.Text + "' ";
                    strSql2 += ", @pSTART_DT = '" + dtpStartDt.Text.ToString() + "' ";
                    strSql2 += ", @pEND_DT = '" + dtpEndDt.Text.ToString() + "' ";
                    strSql2 += ", @pGROUP_CD = '" + txtItemCd.Text.ToString() + "' ";
                    strSql2 += ", @pMAKEORDER_NO = '" + txtMakeOrderNo.Text.ToString() + "' ";
                    strSql2 += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";		
                    DataTable dt2 = SystemBase.DbOpen.NoTranDataTable(strSql2);

                    string[] Project = new string[dt2.Rows.Count];

                    int colorR = 59, colorG = 99, colorB = 59;

                    int prgKind = 4;
                    int[] r = new int[dt2.Rows.Count];
                    int[] g = new int[dt2.Rows.Count];
                    int[] b = new int[dt2.Rows.Count];

                    if (dt2.Rows.Count > 0)
                    {
                        //Loop돌면서 프로젝트별로 색깔을 지정해준다
                        for (int k = 0; k < dt2.Rows.Count; k++)
                        {
                            Project[k] = dt2.Rows[k]["PROJECT"].ToString();

                            r[k] = (colorR * prgKind) % 220;
                            g[k] = (colorG * prgKind) % 220;
                            b[k] = (colorB * prgKind) % 220;

                            //지연프로젝트 색깔인 빨간색이 나오면 색깔을 바꿔준다
                            if (r[k].ToString() == "255" && g[k].ToString() == "0" && b[k].ToString() == "0")
                            {
                                prgKind = prgKind + 3;

                                r[k] = (colorR * prgKind) % 220;
                                g[k] = (colorG * prgKind) % 220;
                                b[k] = (colorB * prgKind) % 220;
                            }

                            //기존 프로젝트 색깔과 같은것이 있으면 색깔을 바꿔준다.
                            for (int chk = 0; chk < k; chk++)
                            {
                                if (r[k] == r[chk] && g[k] == g[chk] && b[k] == b[chk])
                                {
                                    prgKind = prgKind + 3;

                                    r[k] = (colorR * prgKind) % 220;
                                    g[k] = (colorG * prgKind) % 220;
                                    b[k] = (colorB * prgKind) % 220;

                                    break;
                                }
                            }

                            prgKind = prgKind + 3;
                        }
                    }
                    /**********************************************************************/


                    /******************************GANTT***********************************/
                    this.gantt1.Grid.RootNodes.Clear();
                    if (dt.Rows.Count > 0)
                    {
                        DateTime TmpStartTime = DateTime.Now.AddDays(1000);
                        DateTime TmpEndTime = DateTime.Now.AddDays(-1000);
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            if (TmpStartTime > Convert.ToDateTime(dt.Rows[i]["START_DT"].ToString()))
                                TmpStartTime = Convert.ToDateTime(dt.Rows[i]["START_DT"].ToString());
                            if (TmpEndTime < Convert.ToDateTime(dt.Rows[i]["END_DT"].ToString()))
                                TmpEndTime = Convert.ToDateTime(dt.Rows[i]["END_DT"].ToString()).AddHours(4);
                        }
                        if (TmpStartTime.AddDays(45) < TmpEndTime)
                            TmpEndTime = TmpStartTime.AddDays(45);

                        this.gantt1.DateScalerProperties.StartTime = TmpStartTime.AddHours(-2);					// 차트 헤드 디자인 시작일자
                        this.gantt1.DateScalerProperties.StopTime = TmpEndTime;					// 차트 헤드 디자인 종료일자

                        string RESOURCE_CD = "";
                        string strSchId = "";
                        GridNode gn1 = null;
                        TimeItem ti1 = null;

                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            if (optSort1.Checked == true)
                            {
                                if (RESOURCE_CD != dt.Rows[i]["RES_DIS"].ToString() || strSchId != dt.Rows[i]["SCH_ID"].ToString())
                                {
                                    gn1 = null;
                                    ti1 = null;

                                    if (strSchId != dt.Rows[i]["SCH_ID"].ToString())
                                    {
                                        if (i != 0)
                                        {
                                            gn1 = gantt1.Grid.GridStructure.RootNodes.AddNode();
                                            prgKind = 0;
                                        }
                                    }
                                    gn1 = gantt1.Grid.GridStructure.RootNodes.AddNode();
                                    string strInpinity = "유한"; if (dt.Rows[i]["INFINITY"].ToString() == "True") strInpinity = "무한";
                                    gn1.GetCell(0).Content.Value = dt.Rows[i]["WC_NM"].ToString();
                                    gn1.GetCell(1).Content.Value = dt.Rows[i]["RES_DIS"].ToString();
                                    gn1.GetCell(2).Content.Value = dt.Rows[i]["SCH_ID"].ToString() + " (" + dt.Rows[i]["DEPLOY"].ToString() + "/" + strInpinity + ")";
                                }
                            }
                            else
                            {
                                if (RESOURCE_CD != dt.Rows[i]["RES_DIS"].ToString() || strSchId != dt.Rows[i]["SCH_ID"].ToString())
                                {
                                    gn1 = null;
                                    ti1 = null;

                                    if (RESOURCE_CD != dt.Rows[i]["RES_DIS"].ToString())
                                    {
                                        if (i != 0)
                                        {
                                            gn1 = gantt1.Grid.GridStructure.RootNodes.AddNode();
                                        }
                                    }
                                    gn1 = gantt1.Grid.GridStructure.RootNodes.AddNode();
                                    string strInpinity = "유한"; if (dt.Rows[i]["INFINITY"].ToString() == "True") strInpinity = "무한";

                                    gn1.GetCell(0).Content.Value = dt.Rows[i]["WC_NM"].ToString();
                                    gn1.GetCell(1).Content.Value = dt.Rows[i]["RES_DIS"].ToString();
                                    gn1.GetCell(2).Content.Value = dt.Rows[i]["SCH_ID"].ToString() + " (" + dt.Rows[i][11].ToString() + "/" + strInpinity + ")";
                                }
                            }

                            ti1 = Gantt.GanttRowFromGridNode(gn1).Layers[0].AddNewTimeItem();
                            ti1.TimeItemLayout = new TimeItemLayout();

                            string ChkProject = dt.Rows[i]["PROJECT_NO"].ToString() + dt.Rows[i]["PROJECT_SEQ"].ToString();

                            for (int j = 0; j < dt2.Rows.Count; j++)
                            {
                                if (Project[j].ToString() == ChkProject)
                                {
                                    ti1.TimeItemLayout.Color = Color.FromArgb(r[j], g[j], b[j]);
                                }
                            }

                            //일정 지연되면 빨간색 처리
                            if (Convert.ToDateTime(dt.Rows[i]["END_DT"].ToString()) > Convert.ToDateTime(dt.Rows[i]["MAKEFINISH_DT"].ToString()))
                            {
                                ti1.TimeItemLayout.Color = Color.FromArgb(255, 0, 0); //빨간색
                            }

                            string strMemo = "";
                            strMemo = "             업체명 : " + dt.Rows[i]["CUST_CD"].ToString();
                            strMemo += "\n             납기일 : " + dt.Rows[i]["DELIVERY_DT"].ToString();
                            strMemo += "\n    프로젝트번호 : " + dt.Rows[i]["PROJECT_NO"].ToString() + " - " + dt.Rows[i]["PROJECT_SEQ"].ToString();
                            strMemo += "\n    제품오더번호 : " + dt.Rows[i]["MAKEORDER_NO"].ToString();
                            strMemo += "\n    제조오더번호 : " + dt.Rows[i]["WORKORDER_NO"].ToString();
                            strMemo += "\n 제품[도면번호] : " + dt.Rows[i]["GROUP_CD"].ToString();
                            strMemo += "\n 부품[도면번호] : " + dt.Rows[i]["ITEM_CD"].ToString();
                            strMemo += "\n             작업장 : " + dt.Rows[i]["WC_NM"].ToString();
                            strMemo += "\n          오더수량 : " + dt.Rows[i]["WORK_QTY"].ToString();
                            ti1.UserReference = strMemo;

                            ti1.Start = Convert.ToDateTime(dt.Rows[i]["START_DT"].ToString());
                            DateTime STM = Convert.ToDateTime(dt.Rows[i]["START_DT"].ToString());
                            DateTime ETM = Convert.ToDateTime(dt.Rows[i]["END_DT"].ToString());		//DateTime에 종료일자 저장
                            TimeSpan TS = ETM - STM;
                            int GABTIME = Convert.ToInt32(TS.TotalMinutes);					//남은 근무기간 추출
                            ti1.Stop = ti1.Start.AddMinutes(GABTIME);

                            RESOURCE_CD = dt.Rows[i]["RES_DIS"].ToString();
                            strSchId = dt.Rows[i]["SCH_ID"].ToString();
                        }
                    }
                    else
                    {
                        MessageBox.Show(SystemBase.Base.MessageRtn("B0011"), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 조회 팝업
        //공장
        private void btnPLANT_CD_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pType='P011', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };
                string[] strSearch = new string[] { txtPLANT_CD.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00005", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "공장 조회");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtPLANT_CD.Text = Msgs[0].ToString();
                    txtPLANT_NM.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "공장 조회 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //프로젝트번호
        private void btnPROJ_NO_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW003 pu = new WNDW003(txtPROJ_NO.Text, "S1", "R");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtEntNm.Value = Msgs[2].ToString() + " (" + Msgs[1].ToString() + ")";
                    txtPROJ_NO.Value = Msgs[3].ToString();
                    txtPROJ_NM.Value = Msgs[4].ToString();
                    txtProjectSeq.Value = Msgs[5].ToString();
                    txtItemCd.Value = Msgs[6].ToString();
                    txtItemNm.Value = Msgs[7].ToString();
                    dtpDelvDt.Value = Msgs[12].ToString();
                    txtMakeOrderNo.Value = Msgs[13].ToString();
                    txtOrderQty.Value = Msgs[14].ToString();
                    txtCustNm.Value = Msgs[17].ToString() + " (" + Msgs[16].ToString() + ")";

                    txtPROJ_NO.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "프로젝트 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //작업장
        private void btnWcCd_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pType='P042', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pETC = 'P002', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };
                string[] strSearch = new string[] { txtWcCd.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00025", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "작업장 조회");
                pu.Width = 500;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtWcCd.Text = Msgs[0].ToString();
                    txtWcNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "공장 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 간트 챠트 이벤트
        private void gantt1_OnTimeItem_Hoover(PlexityHide.GTP.Gantt aGantt, PlexityHide.GTP.TimeItemEventArgs e)
        {
            string newTooltipText = "";
            if (e.TimeItem != null)
            {
                string strMemo = e.TimeItem.UserReference.ToString();
                newTooltipText = strMemo + "\n          시작시간 : " + e.TimeItem.Start.ToLongDateString() + " " + e.TimeItem.Start.ToLongTimeString();
                newTooltipText = newTooltipText + "\n          종료시간 : " + e.TimeItem.Stop.ToLongDateString() + " " + e.TimeItem.Stop.ToLongTimeString();
            }
            else
            {
                newTooltipText = "";
            }

            toolTip1.SetToolTip(gantt1.TimeItemArea, newTooltipText);
        }

        private void gantt1_OnTimeItemAreaMouseDown(object sender, MouseEventArgs e)
        {
            if (gantt1.TimeItemFromPoint(e.X, e.Y) != null)
            {	// 드레그 금지용
                TimeItem thedraggedTimedItem = gantt1.TimeItemFromPoint(e.X, e.Y);
                gantt1.TimeItemArea.DoDragDrop(thedraggedTimedItem, DragDropEffects.All);
                gantt1.MouseMoveCancel();
            }
        }
        #endregion

        #region 코드 입력시 코드명 자동입력
        //공장
        private void txtPLANT_CD_TextChanged(object sender, System.EventArgs e)
        {
            txtPLANT_NM.Value = SystemBase.Base.CodeName("PLANT_CD", "PLANT_NM", "B_PLANT_INFO", txtPLANT_CD.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "'");
        }

        //프로젝트번호
        private void txtPROJ_NO_TextChanged(object sender, System.EventArgs e)
        {
            txtPROJ_NM.Value = SystemBase.Base.CodeName("PROJECT_NO", "PROJECT_NM", "S_SO_MASTER", txtPROJ_NO.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "'");

            if (txtPROJ_NM.Text == "")
            {
                txtEntNm.Value = "";
                txtProjectSeq.Value = "";
                txtItemCd.Value = "";
                txtItemNm.Value = "";
                dtpDelvDt.Value = "";
                txtMakeOrderNo.Value = "";
                txtOrderQty.Value = "";
                txtCustNm.Value = "";

            }
        }

        //품목코드
        private void txtItemCd_TextChanged(object sender, System.EventArgs e)
        {
            txtItemNm.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtItemCd.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "'");
        }

        //작업장
        private void txtWcCd_TextChanged(object sender, System.EventArgs e)
        {
            txtWcNm.Value = SystemBase.Base.CodeName("RES_CD", "RES_DIS", "P_RESO_MANAGE", txtWcCd.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "'");
        }
        #endregion

    }
}
