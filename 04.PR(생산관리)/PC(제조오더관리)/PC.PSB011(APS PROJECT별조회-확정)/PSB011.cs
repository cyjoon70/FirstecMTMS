#region 작성정보
/*********************************************************************/
// 단위업무명 : APS일정계획(프로젝트별-확정)
// 작 성 자 : 김현근
// 작 성 일 : 2013-04-16
// 작성내용 : APS일정계획(프로젝트별-확정)
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
namespace PC.PSB011
{
    public partial class PSB011 : UIForm.Buttons
    {
        public PSB011()
        {
            InitializeComponent();
        }       

        #region Form Load시
        private void PSB011_Load(object sender, System.EventArgs e)
        {
            //필수 체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            gantt1.GridProperties.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;					// BorderStyle
            this.gantt1.DateScaler.CultureInfoDateTimeFormat = new CultureInfo("ko-KR", false).DateTimeFormat; // 날짜 포맷
            this.gantt1.VerticalDayStripes = true;

            txtPLANT_CD.Text = SystemBase.Base.gstrPLANT_CD;
        }
        #endregion

        #region NEW
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            txtPLANT_CD.Text = SystemBase.Base.gstrPLANT_CD;
        }
        #endregion
        
        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {
                    string sql = "usp_PSB011 'S1'";
                    sql += ", @pPROJECT_NO = '" + txtProjectNo.Text + "'";
                    sql += ", @pPROJECT_SEQ = '" + txtProjectSeq.Text + "'";
                    sql += ", @pPLANT_CD = '" + txtPLANT_CD.Text.ToString() + "'";
                    sql += ", @pMAKEORDER_NO = '" + txtMakeOrderNo.Text.ToString() + "'";
                    sql += ", @pGROUP_CD = '" + txtItemCd.Text.ToString() + "'";
                    sql += ", @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                    DataTable dt = SystemBase.DbOpen.NoTranDataTable(sql);

                    /******************************GANTT************************************/
                    this.gantt1.Grid.RootNodes.Clear();
                    if (dt.Rows.Count > 0)
                    {
                        string strInpinity = "유한";
                        if (dt.Rows[0][23].ToString() == "True")
                            strInpinity = "무한";
                        lblSchData.Text = dt.Rows[0]["DEPLOY"] + " (" + dt.Rows[0]["INFINITY"] + "-" + strInpinity + ") ";

                        DateTime TmpStartTime = DateTime.Now.AddDays(1000);
                        DateTime TmpEndTime = DateTime.Now.AddDays(-1000);
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            if (TmpStartTime > Convert.ToDateTime(dt.Rows[i]["START_DT"].ToString()))
                                TmpStartTime = Convert.ToDateTime(dt.Rows[i]["START_DT"].ToString());
                            if (TmpEndTime < Convert.ToDateTime(dt.Rows[i]["END_DT"].ToString()))
                                TmpEndTime = Convert.ToDateTime(dt.Rows[i]["END_DT"].ToString()).AddHours(4);
                        }
                        if (TmpStartTime.AddDays(30) < TmpEndTime)
                            TmpEndTime = TmpStartTime.AddDays(30);

                        this.gantt1.DateScalerProperties.StartTime = TmpStartTime.AddHours(-2);					// 차트 헤드 디자인 시작일자
                        this.gantt1.DateScalerProperties.StopTime = TmpEndTime;									// 차트 헤드 디자인 종료일자

                        string BarColor = "";
                        int r = 0;
                        int g = 0;
                        int b = 0;

                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            GridNode gn1 = gantt1.Grid.GridStructure.RootNodes.AddNode();
                            gn1.GetCell(0).Content.Value = dt.Rows[i]["FIG_NO"].ToString();
                            gn1.GetCell(1).Content.Value = dt.Rows[i]["WORKORDER_NO"].ToString();
                            gn1.GetCell(2).Content.Value = dt.Rows[i]["ITEM_CD"].ToString();
                            gn1.GetCell(3).Content.Value = dt.Rows[i]["WC_NM"].ToString();
                            gn1.GetCell(4).Content.Value = dt.Rows[i]["RES_DIS"].ToString();
                            gn1.GetCell(5).Content.Value = dt.Rows[i]["OPR_NO"].ToString();
                            gn1.GetCell(6).Content.Value = dt.Rows[i]["JOB_CD"].ToString();

                            TimeItem ti1 = Gantt.GanttRowFromGridNode(gn1).Layers[0].AddNewTimeItem();
                            ti1.TimeItemLayout = new TimeItemLayout();

                            if (BarColor != (dt.Rows[i]["PROJECT_NO"].ToString() + dt.Rows[i]["PROJECT_SEQ"].ToString() + dt.Rows[i]["WC_NM"].ToString()))
                            {
                                int prgKind = (i + 1) * 9;
                                r = (51 * prgKind) % 237;
                                g = (56 * prgKind) % 157;
                                b = (221 * prgKind) % 231;

                                if (r == 255 && g == 0 && b == 0)
                                {
                                    prgKind = (i + 1) * 9;
                                    r = (51 * prgKind) % 237;
                                    g = (56 * prgKind) % 157;
                                    b = (221 * prgKind) % 231;
                                }
                            }

                            ti1.TimeItemLayout.Color = Color.FromArgb(r, g, b);

                            ti1.Start = Convert.ToDateTime(dt.Rows[i]["START_DT"].ToString());
                            DateTime STM = Convert.ToDateTime(dt.Rows[i]["START_DT"].ToString());
                            DateTime ETM = Convert.ToDateTime(dt.Rows[i]["END_DT"].ToString());	//DateTime에 종료일자 저장
                            TimeSpan TS = ETM - STM;
                            int GABTIME = Convert.ToInt32(TS.TotalMinutes);				//남은 근무기간 추출
                            ti1.Stop = ti1.Start.AddMinutes(GABTIME);

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

                            BarColor = dt.Rows[i]["PROJECT_NO"].ToString() + dt.Rows[i]["PROJECT_SEQ"].ToString() + dt.Rows[i]["WC_NM"].ToString();
                        }
                    }
                    else
                    {
                        lblSchData.Text = "";
                        MessageBox.Show(SystemBase.Base.MessageRtn("B0011"), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    /******************************GANTT************************************/
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 간트챠트 이벤트
        private void gantt1_OnTimeItem_Hoover(Gantt aGantt, TimeItemEventArgs e)
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

        private void gantt1_OnTimeItemAreaMouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (gantt1.TimeItemFromPoint(e.X, e.Y) != null)
            {	// 드레그 금지용
                TimeItem thedraggedTimedItem = gantt1.TimeItemFromPoint(e.X, e.Y);
                gantt1.TimeItemArea.DoDragDrop(thedraggedTimedItem, DragDropEffects.All);
                gantt1.MouseMoveCancel();
            }
        }
        #endregion

        #region 조회조건 팝업
        //공장
        private void btnPlantCd_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pType='P011' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };
                string[] strSearch = new string[] { txtPLANT_CD.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00005", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "공장 조회");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtPLANT_CD.Text = Msgs[0].ToString();
                    txtPlantNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "공장 조회 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnMakeOrderNo_Click(object sender, EventArgs e)
        {
            try
            {
                WNDW003 pu = new WNDW003(txtProjectNo.Text, "S1", "C");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtEntNm.Value = Msgs[2].ToString() + " (" + Msgs[1].ToString() + ")";
                    txtProjectNo.Text = Msgs[3].ToString();
                    txtProjectNm.Value = Msgs[4].ToString();
                    txtProjectSeq.Text = Msgs[5].ToString();
                    txtItemCd.Text = Msgs[6].ToString();
                    txtItemNm.Value = Msgs[7].ToString();
                    dtpDelvDt.Value = Msgs[12].ToString();
                    txtMakeOrderNo.Text = Msgs[13].ToString();
                    txtOrderQty.Value = Msgs[14].ToString();
                    txtCustNm.Value = Msgs[17].ToString() + " (" + Msgs[16].ToString() + ")";

                    txtProjectNo.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "프로젝트 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //프로젝트팦업
        private void btnProject_Click(object sender, EventArgs e)
        {
            try
            {
                WNDW003 pu = new WNDW003(txtProjectNo.Text, "S1", "C");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtEntNm.Value = Msgs[2].ToString();
                    txtProjectNo.Text = Msgs[3].ToString();
                    txtProjectNm.Value = Msgs[4].ToString();
                    txtProjectSeq.Text = Msgs[5].ToString();
                    txtItemCd.Text = Msgs[6].ToString();
                    txtItemNm.Value = Msgs[7].ToString();
                    dtpDelvDt.Value = Msgs[12].ToString().Substring(0, 10);
                    txtMakeOrderNo.Text = Msgs[13].ToString();
                    txtOrderQty.Value = Msgs[14].ToString();
                    txtCustNm.Value = Msgs[16].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "프로젝트 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 코드 입력시 코드명 자동입력
        private void txtProjectNo_TextChanged(object sender, EventArgs e)
        {
            txtProjectNm.Value = SystemBase.Base.CodeName("PROJECT_NO", "PROJECT_NM", "S_SO_MASTER", txtProjectSeq.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");

            if (txtProjectNo.Text == "")
            {
                txtEntNm.Value = "";
                txtProjectSeq.Text = "";
                txtItemCd.Text = "";
                txtItemNm.Value = "";
                dtpDelvDt.Value = "";
                txtMakeOrderNo.Text = "";
                txtOrderQty.Value = "";
                txtCustNm.Value = "";

            }
        }
        private void txtPLANT_CD_TextChanged(object sender, EventArgs e)
        {
            txtPlantNm.Value = SystemBase.Base.CodeName("PLANT_CD", "PLANT_NM", "B_PLANT_INFO", txtPLANT_CD.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");
        }        

        private void txtItemCd_TextChanged(object sender, EventArgs e)
        {
            txtItemNm.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtItemCd.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");
        }
        #endregion
    }
}
