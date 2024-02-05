#region 작성정보
/*********************************************************************/
// 단위업무명 : 생산진행현황
// 작 성 자 : 김현근
// 작 성 일 : 2013-04-16
// 작성내용 : 생산진행현황
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
namespace PC.PSB001
{
    public partial class PSB001 : UIForm.Buttons
    {
        #region 변수선언
        string strProjectNo = "", strProjectNm = "", strProjectSeq = "", strMakeOrderNo = "", strEntNm = "";
        string strCustNm = "", strItemCd = "", strItemNm = "", strDelvDt = "";
        int iOrderQty = 0, iSumWorkTm = 0, iSumResultTm = 0;
        double dblMakeRate = 0;

        bool ParmChk = false;
        #endregion

        public PSB001()
        {
            InitializeComponent();
        }
        public PSB001(string ProjectNo,
            string ProjectNm,
            string ProjectSeq,
            string EntNm,
            string CustNm,
            string ItemCd,
            string ItemNm,
            string MakeOrderNo,
            string OrderQty,
            string DelvDt,
            string SumWorkTm,
            string SumResultTm,
            string MakeRate)
        {
            if (ProjectNo.Length > 0)
            {
                strProjectNo = ProjectNo;
                strProjectNm = ProjectNm;
                strProjectSeq = ProjectSeq;
                strEntNm = EntNm;
                strCustNm = CustNm;
                strItemCd = ItemCd;
                strItemNm = ItemNm;
                strMakeOrderNo = MakeOrderNo;
                iOrderQty = Convert.ToInt32(OrderQty);
                strDelvDt = DelvDt;
                iSumWorkTm = Convert.ToInt32(SumWorkTm);
                iSumResultTm = Convert.ToInt32(SumResultTm);
                dblMakeRate = Convert.ToDouble(MakeRate);

                ParmChk = true;
            }

            InitializeComponent();
        }

        #region Form Load시
        private void PSB001_Load(object sender, System.EventArgs e)
        {
            //필수 체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            SystemBase.Validation.GroupBox_Setting(groupBox3);

            //그리드 세팅
            string[] Title = new string[] { "WBC", "제조오더번호", "공정", "자원", "Plan시작일", "Plan완료일", "Actual시작일", "Actual완료일", "부하시수", "실적시수", "진척율" };
            string[] CellType = new string[] { "", "", "", "", "", "", "", "", "", "", "" };
            int[] Width = new int[] { 100, 100, 80, 80, 80, 80, 80, 80, 60, 60, 60, 60 };
            string[] HorzAlign = new string[] { "", "", "", "C", "C", "C", "C", "R", "R", "C", "" };
            
            //GTP 그리드 세팅
            ///파라메터 설명
            ///GanGridSet(GANTT명,총COL수,보여줄최소일자,보여줄최대일자,차트헤드디자인시작일자(현재일자+-),차트헤드디자인종료일자(현재일자+-),
            ///헤드제목,Cell타입,Cell넓이), 데이터정렬(L,C,R), 폰트명, 폰트사이즈
            SystemBase.Base.GantGridSet(this.gantt1, 11, "2000-01-01", "2999-12-31", 0, 7, Title, CellType, Width, HorzAlign, "굴림", 9);
            
            //			gantt1.GridProperties.Columns[10].Hide = true;
            //			gantt1.GridProperties.Columns[10].Width = 0;

            //생산요약정보조회에서 링크되어 넘어왔을때
            if (ParmChk == true)
            {
                txtProjectNo.Text = strProjectNo;
                txtProjectNm.Text = strProjectNm;
                txtProjectSeq.Text = strProjectSeq;
                txtEntNm.Text = strEntNm;
                txtCustNm.Text = strCustNm;
                txtItemCd.Text = strItemCd;
                txtItemNm.Text = strItemNm;
                txtMakeOrderNo.Text = strMakeOrderNo;
                txtOrderQty.Text = iOrderQty.ToString();
                dtpDelvDt.Text = strDelvDt;
                txtWorkTmSum.Text = iSumWorkTm.ToString();
                txtResultSum.Text = iSumResultTm.ToString();
                dtxtMakeRate.Value = dblMakeRate;

                SearchExec();
            }
        }
        #endregion

        #region SearchExec() Master 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {
                    //그리드에 데이터를 넣고 챠트를 그려준다
                    GanttRow gr1 = null;
                    GridNode gn1 = null;
                    TimeItem ti1 = null;
                    TimeItem ti2 = null;
                    DateTime TmpStartTime = new DateTime();
                    DateTime TmpEndTime = new DateTime();

                    //준비공정 시수 집계변수
                    int iSumWorkTm = 0;
                    int iSumResultTm = 0;
                    double dblMakeRate = 0;

                    /*||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||*/
                    /*======================================창정비 준비공정=====================================================*/
                    /*||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||*/
                    string strQuery = " usp_PSB001 @pTYPE = 'S1'";
                    strQuery += ", @pPROJECT_NO = '" + txtProjectNo.Text + "' ";
                    strQuery += ", @pPROJECT_SEQ = '" + txtProjectSeq.Text + "' ";
                    strQuery += ", @pITEM_CD = '" + txtItemCd.Text + "' ";
                    strQuery += ", @pMAKEORDER_NO = '" + txtMakeOrderNo.Text + "' ";
                    strQuery += ", @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

                    DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

                    /******************************GANTT************************************/
                    this.gantt1.Grid.RootNodes.Clear();

                    if (dt.Rows.Count > 0)
                    {
                        if (dt.Rows[0]["PROC_SEQ"].ToString() != "")
                        {
                            gantt1.GridProperties.Columns[0].Title = "WBC";
                            gantt1.GridProperties.Columns[1].Title = "제조오더번호";
                            gantt1.GridProperties.Columns[2].Title = "공정";
                            gantt1.GridProperties.Columns[3].Title = "자원";

                            //챠트 최소일정에서 최대일정까지 먼저 계산 후 챠트 달력일정을 그려준다
                            TmpStartTime = Convert.ToDateTime(dt.Rows[0]["START_DT"].ToString());
                            TmpEndTime = Convert.ToDateTime(dt.Rows[0]["END_DT"].ToString());

                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                if (dt.Rows[i]["RESULT_END_DT"].ToString() != "")									//실적완료일자가 있다면
                                {
                                    if (TmpEndTime < Convert.ToDateTime(dt.Rows[i]["RESULT_END_DT"].ToString()))	//실적일자와 달력 최대일자를 비교
                                    {
                                        if (dt.Rows[i]["END_DT"].ToString() != "")									//계획완료일자가 있다면
                                        {
                                            if (TmpEndTime < Convert.ToDateTime(dt.Rows[i]["END_DT"].ToString()))	//계획완료일자와 달력 최대일자를 비교
                                            {
                                                TmpEndTime = Convert.ToDateTime(dt.Rows[i]["END_DT"].ToString());
                                            }
                                            else
                                            {
                                                TmpEndTime = Convert.ToDateTime(dt.Rows[i]["RESULT_END_DT"].ToString());
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (dt.Rows[i]["END_DT"].ToString() != "")										//계획완료일자가 있다면
                                    {
                                        if (TmpEndTime < Convert.ToDateTime(dt.Rows[i]["END_DT"].ToString()))		//계획완료일자와 달력 최대일자를 비교
                                        {
                                            TmpEndTime = Convert.ToDateTime(dt.Rows[i]["END_DT"].ToString());
                                        }
                                    }
                                }
                            }

                            this.gantt1.DateScalerProperties.StartTime = TmpStartTime.AddDays(-1);		// 차트 헤드 디자인 시작일자(여백으로 인해 -1)
                            this.gantt1.DateScalerProperties.StopTime = TmpEndTime.AddDays(2);			// 차트 헤드 디자인 종료일자(여백으로 인해 +2)

                            for (int i = 0; i < dt.Rows.Count; i++)
                            {

                                gn1 = null;
                                ti1 = null;

                                //그리드 Node(Row)추가 후 데이터 입력
                                gn1 = gantt1.Grid.GridStructure.RootNodes.AddNode();
                                gn1.GetCell(0).Content.Value = dt.Rows[i]["PROC_SEQ"].ToString();
                                gn1.GetCell(1).Content.Value = dt.Rows[i]["WORKORDER_NO"].ToString();
                                gn1.GetCell(2).Content.Value = dt.Rows[i]["JOB_NM"].ToString();
                                gn1.GetCell(3).Content.Value = dt.Rows[i]["RES_DIS"].ToString();
                                gn1.GetCell(4).Content.Value = dt.Rows[i]["START_DT"].ToString();
                                gn1.GetCell(5).Content.Value = dt.Rows[i]["VISUAL_END_DT"].ToString();
                                gn1.GetCell(6).Content.Value = dt.Rows[i]["RESULT_START_DT"].ToString();
                                gn1.GetCell(7).Content.Value = dt.Rows[i]["RESULT_END_DT"].ToString();
                                if (dt.Rows[i]["WORK_TM"].ToString() != "0")
                                    gn1.GetCell(8).Content.Value = Convert.ToString(SystemBase.Base.Comma2(dt.Rows[i]["WORK_TM"].ToString()));
                                if (dt.Rows[i]["RESULT_WORK_TM"].ToString() != "0")
                                    gn1.GetCell(9).Content.Value = Convert.ToString(SystemBase.Base.Comma2(dt.Rows[i]["RESULT_WORK_TM"].ToString()));
                                if (dt.Rows[i]["MAKE_RATE"].ToString() != "0")
                                    gn1.GetCell(10).Content.Value = dt.Rows[i]["MAKE_RATE"].ToString() + '%';

                                //부하시수, 실적시수, 진척율 집계
                                iSumWorkTm += Convert.ToInt32(dt.Rows[i]["WORK_TM"].ToString());
                                iSumResultTm += Convert.ToInt32(dt.Rows[i]["RESULT_WORK_TM"].ToString());
                                dblMakeRate += Convert.ToDouble(dt.Rows[i]["MAKE_RATE"].ToString());

                                //챠트 Node 추가 - 계획, 실적 겹치게 LAYER따로 구성
                                gr1 = GanttRow.FromGridNode(gn1);
                                gr1.CollisionDetect = true;
                                gr1.CollisionDetectBetweenLayers = false;
                                gr1.Layers.AddLayer();
                                gr1.Layers.AddLayer();
                                ti1 = gr1.Layers[0].AddNewTimeItem();
                                ti2 = gr1.Layers[1].AddNewTimeItem();

                                //실적 챠트안에 text 추가
                                TimeItemText txt1 = new TimeItemText();
                                txt1.Text = dt.Rows[i]["MAKE_RATE"].ToString() + "%";
                                txt1.TimeItemTextLayout = gantt1.TimeItemTextLayouts.GetFromName("New");
                                txt1.TimeItemTextLayout.Color = Color.White;
                                txt1.TimeItemTextLayout.Font = new System.Drawing.Font("굴림", 9.25F, FontStyle.Bold);
                                txt1.TimeItemTextLayout.VertAlign = StringAlignment.Center;
                                txt1.TimeItemTextLayout.HorzAlign = StringAlignment.Center;
                                ti2.TimeItemTexts.Add(txt1);

                                ti1.TimeItemLayout = new TimeItemLayout();
                                ti2.TimeItemLayout = new TimeItemLayout();

                                //챠트 색깔
                                ti1.TimeItemLayout.Color = Color.LightBlue;		//계획
                                ti2.TimeItemLayout.Color = Color.LightCoral;	//실적

                                //챠트 일정 시작 에서 완료까지.. 데이터로 그려준다
                                if (dt.Rows[i]["START_DT"].ToString() != "")
                                {
                                    ti1.Start = Convert.ToDateTime(dt.Rows[i]["START_DT"].ToString());
                                }
                                if (dt.Rows[i]["END_DT"].ToString() != "")
                                {
                                    ti1.Stop = Convert.ToDateTime(dt.Rows[i]["END_DT"].ToString());
                                }

                                if (dt.Rows[i]["RESULT_START_DT"].ToString() != "")
                                {
                                    ti2.Start = Convert.ToDateTime(dt.Rows[i]["RESULT_START_DT"].ToString());
                                }
                                if (dt.Rows[i]["RESULT_END_DT"].ToString() != "")
                                {
                                    ti2.Stop = Convert.ToDateTime(dt.Rows[i]["RESULT_END_DT"].ToString());
                                }
                            }
                        }
                    }

                    /*||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||*/
                    /*======================================창정비 생산공정=====================================================*/
                    /*||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||*/

                    string strQuery1 = " usp_PSB001 @pTYPE = 'S2' ";
                    strQuery1 += ", @pPROJECT_NO = '" + txtProjectNo.Text + "' ";
                    strQuery1 += ", @pPROJECT_SEQ = '" + txtProjectSeq.Text + "' ";
                    strQuery1 += ", @pITEM_CD = '" + txtItemCd.Text + "' ";
                    strQuery1 += ", @pMAKEORDER_NO = '" + txtMakeOrderNo.Text + "' ";
                    strQuery1 += ", @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

                    DataTable dt1 = SystemBase.DbOpen.NoTranDataTable(strQuery1);

                    /******************************GANTT************************************/
                    if (dt1.Rows.Count > 0 && dt1.Rows[0]["FIG_NO"].ToString() != "")
                    {
                        //창정비 준비공정 데이터가 있다면 공백 두줄 후 헤더 추가
                        if (dt.Rows.Count > 0)
                        {
                            //공백두줄
                            gn1 = gantt1.Grid.GridStructure.RootNodes.AddNode();
                            gn1 = gantt1.Grid.GridStructure.RootNodes.AddNode();

                            //작업공정구분제목 세팅
                            gn1 = gantt1.Grid.GridStructure.RootNodes.AddNode();
                            gn1.GetCell(0).Content.Value = "제품오더기준";
                            gn1 = gantt1.Grid.GridStructure.RootNodes.AddNode();
                            gn1.GetCell(0).Content.Value = "FIG_NO";
                            gn1.GetCell(1).Content.Value = "제조오더번호";
                            gn1.GetCell(2).Content.Value = "품목코드";
                            gn1.GetCell(3).Content.Value = "품목명";
                            gn1.GetCell(4).Content.Value = "Plan시작일";
                            gn1.GetCell(5).Content.Value = "Plan완료일";
                            gn1.GetCell(6).Content.Value = "Actual시작일";
                            gn1.GetCell(7).Content.Value = "Actual완료일";
                            gn1.GetCell(8).Content.Value = "부하시수";
                            gn1.GetCell(9).Content.Value = "실적시수";
                            gn1.GetCell(10).Content.Value = "진척율";

                        }
                        else //창정비 준비공정 데이터가 없다면
                        {
                            gantt1.GridProperties.Columns[0].Title = "FIG_NO";
                            gantt1.GridProperties.Columns[1].Title = "제조오더번호";
                            gantt1.GridProperties.Columns[2].Title = "품목코드";
                            gantt1.GridProperties.Columns[3].Title = "품목명";

                            //챠트 최소일정에서 최대일정까지 먼저 계산 후 챠트 달력일정을 그려준다
                            TmpStartTime = Convert.ToDateTime(dt1.Rows[0]["START_DT"].ToString());
                            TmpEndTime = Convert.ToDateTime(dt1.Rows[0]["END_DT"].ToString());

                            for (int i = 0; i < dt1.Rows.Count; i++)
                            {
                                if (dt1.Rows[i]["START_DT"].ToString() != "")
                                {
                                    if (TmpStartTime > Convert.ToDateTime(dt1.Rows[i]["START_DT"].ToString()))
                                    {
                                        TmpStartTime = Convert.ToDateTime(dt1.Rows[i]["START_DT"].ToString());
                                    }
                                }
                            }
                        }

                        for (int i = 0; i < dt1.Rows.Count; i++)
                        {
                            if (dt1.Rows[i]["RESULT_END_DT"].ToString() != "")									//실적완료일자가 있다면
                            {
                                if (TmpEndTime < Convert.ToDateTime(dt1.Rows[i]["RESULT_END_DT"].ToString()))	//실적일자와 달력 최대일자를 비교
                                {
                                    if (dt1.Rows[i]["END_DT"].ToString() != "")									//계획완료일자가 있다면
                                    {
                                        if (TmpEndTime < Convert.ToDateTime(dt1.Rows[i]["END_DT"].ToString()))	//계획완료일자와 달력 최대일자를 비교
                                        {
                                            TmpEndTime = Convert.ToDateTime(dt1.Rows[i]["END_DT"].ToString());
                                        }
                                        else
                                        {
                                            TmpEndTime = Convert.ToDateTime(dt1.Rows[i]["RESULT_END_DT"].ToString());
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (dt1.Rows[i]["END_DT"].ToString() != "")										//계획완료일자가 있다면
                                {
                                    if (TmpEndTime < Convert.ToDateTime(dt1.Rows[i]["END_DT"].ToString()))		//계획완료일자와 달력 최대일자를 비교
                                    {
                                        TmpEndTime = Convert.ToDateTime(dt1.Rows[i]["END_DT"].ToString());
                                    }
                                }
                            }
                        }

                        this.gantt1.DateScalerProperties.StartTime = TmpStartTime.AddDays(-1);		// 차트 헤드 디자인 시작일자(여백으로 인해 -1)
                        this.gantt1.DateScalerProperties.StopTime = TmpEndTime.AddDays(2);			// 차트 헤드 디자인 종료일자(여백으로 인해 +2)

                        for (int i = 0; i < dt1.Rows.Count; i++)
                        {
                            gn1 = null;
                            ti1 = null;

                            //그리드 Node(Row)추가 후 데이터 입력
                            gn1 = gantt1.Grid.GridStructure.RootNodes.AddNode();
                            gn1.GetCell(0).Content.Value = dt1.Rows[i]["FIG_NO"].ToString();
                            gn1.GetCell(1).Content.Value = dt1.Rows[i]["WORKORDER_NO"].ToString();
                            gn1.GetCell(2).Content.Value = dt1.Rows[i]["ITEM_CD"].ToString();
                            gn1.GetCell(3).Content.Value = dt1.Rows[i]["ITEM_NM"].ToString();
                            gn1.GetCell(4).Content.Value = dt1.Rows[i]["START_DT"].ToString();
                            gn1.GetCell(5).Content.Value = dt1.Rows[i]["END_DT"].ToString();
                            gn1.GetCell(6).Content.Value = dt1.Rows[i]["RESULT_START_DT"].ToString();
                            gn1.GetCell(7).Content.Value = dt1.Rows[i]["RESULT_END_DT"].ToString();
                            if (dt1.Rows[i]["WORK_TM"].ToString() != "0")
                                gn1.GetCell(8).Content.Value = Convert.ToString(SystemBase.Base.Comma2(dt1.Rows[i]["WORK_TM"].ToString()));
                            if (dt1.Rows[i]["RESULT_WORK_TM"].ToString() != "0")
                                gn1.GetCell(9).Content.Value = Convert.ToString(SystemBase.Base.Comma2(dt1.Rows[i]["RESULT_WORK_TM"].ToString()));
                            if (dt1.Rows[i]["MAKE_RATE"].ToString() != "0")
                                gn1.GetCell(10).Content.Value = Convert.ToString(SystemBase.Base.MyRound(Convert.ToDouble(dt1.Rows[i]["MAKE_RATE"].ToString()), 2)) + '%';


                            //부하시수, 실적시수, 진척율 집계
                            iSumWorkTm += Convert.ToInt32(dt1.Rows[i]["WORK_TM"].ToString());
                            iSumResultTm += Convert.ToInt32(dt1.Rows[i]["RESULT_WORK_TM"].ToString());
                            dblMakeRate += Convert.ToDouble(dt1.Rows[i]["MAKE_RATE"].ToString());

                            //챠트 Node 추가 - 계획, 실적 겹치게 LAYER따로 구성
                            gr1 = GanttRow.FromGridNode(gn1);
                            gr1.CollisionDetect = true;
                            gr1.CollisionDetectBetweenLayers = false;
                            gr1.Layers.AddLayer();
                            gr1.Layers.AddLayer();
                            ti1 = gr1.Layers[0].AddNewTimeItem();
                            ti2 = gr1.Layers[1].AddNewTimeItem();

                            //실적 챠트안에 text 추가
                            TimeItemText txt2 = new TimeItemText();
                            txt2.Text = dt1.Rows[i]["MAKE_RATE"].ToString() + "%";
                            txt2.TimeItemTextLayout = gantt1.TimeItemTextLayouts.GetFromName("New");
                            txt2.TimeItemTextLayout.Color = Color.White;
                            txt2.TimeItemTextLayout.Font = new System.Drawing.Font("굴림", 9.25F, FontStyle.Bold);
                            txt2.TimeItemTextLayout.VertAlign = StringAlignment.Center;
                            txt2.TimeItemTextLayout.HorzAlign = StringAlignment.Center;
                            ti2.TimeItemTexts.Add(txt2);

                            ti1.TimeItemLayout = new TimeItemLayout();
                            ti2.TimeItemLayout = new TimeItemLayout();

                            //챠트 색깔
                            ti1.TimeItemLayout.Color = Color.LightBlue;		//계획
                            ti2.TimeItemLayout.Color = Color.LightCoral;	//실적

                            //챠트 일정 시작 에서 완료까지.. 데이터로 그려준다
                            if (dt1.Rows[i]["START_DT"].ToString() != "")
                            {
                                ti1.Start = Convert.ToDateTime(dt1.Rows[i]["START_DT"].ToString());
                            }
                            if (dt1.Rows[i]["END_DT"].ToString() != "")
                            {
                                ti1.Stop = Convert.ToDateTime(dt1.Rows[i]["END_DT"].ToString());
                            }
                            if (dt1.Rows[i]["RESULT_START_DT"].ToString() != "")
                            {
                                ti2.Start = Convert.ToDateTime(dt1.Rows[i]["RESULT_START_DT"].ToString());
                            }
                            if (dt1.Rows[i]["RESULT_END_DT"].ToString() != "")
                            {
                                ti2.Stop = Convert.ToDateTime(dt1.Rows[i]["RESULT_END_DT"].ToString());
                            }
                        }
                    }

                    if (rdoWeek.Checked == true)
                    {
                        gantt1.DateScalerProperties.UseDayNumbersNotWeeks = false;	//주간
                    }
                    else
                    {
                        gantt1.DateScalerProperties.UseDayNumbersNotWeeks = true;	//일간
                    }

                    txtWorkTmSum.Value = SystemBase.Base.Comma2(iSumWorkTm.ToString()).ToString();
                    txtResultSum.Value = SystemBase.Base.Comma2(iSumResultTm.ToString()).ToString();

                    dtxtMakeRate.Value = Convert.ToDouble(txtResultSum.Text.Replace(",", "")) / Convert.ToDouble(txtWorkTmSum.Text.Replace(",", ""));
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                this.Cursor = Cursors.Default;
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            this.Cursor = Cursors.Default;

        }
        #endregion

        #region 마우스 클릭시 드래그 금지
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

        #region 공정 상세정보 조회
        private void gantt1_OnGridDoubleClick(object sender, EventArgs e)
        {
            if (gantt1.Grid.GridStructure.FocusedCell != null) //포커스가 그리드이면
            {
                //그리드 item_cd값 받아오기
                string strItemCd = gantt1.Grid.GridStructure.RootNodes[gantt1.Grid.GridStructure.FocusedCell.Node.Index].GetCell(2).Content.Value.ToString();
                string strWoNo = gantt1.Grid.GridStructure.RootNodes[gantt1.Grid.GridStructure.FocusedCell.Node.Index].GetCell(1).Content.Value.ToString();

                PSB001P1 frm = new PSB001P1(txtProjectNo.Text, txtProjectSeq.Text, strItemCd, strWoNo);
                frm.ShowDialog();
            }
        }
        #endregion

        #region 조회조건 팝업
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

        //품목코드
        private void btnItemCd_Click(object sender, EventArgs e)
        {
            try
            {
                WNDW003 pu = new WNDW003("", txtItemCd.Text, "S1", "C");
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
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "프로젝트 품목코드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 코드 입력시 코드명 자동입력
        private void txtProjectNo_TextChanged(object sender, EventArgs e)
        {
            txtProjectNm.Value = SystemBase.Base.CodeName("PROJECT_NO", "PROJECT_NM", "S_SO_MASTER", txtProjectNo.Text, "");
        }

        private void txtItemCd_TextChanged(object sender, EventArgs e)
        {
            txtItemNm.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtItemCd.Text, "");
        }
        #endregion
    }
}
