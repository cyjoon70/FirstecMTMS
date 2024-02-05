#region 작성정보
/*********************************************************************/
// 단위업무명 : 창고입고등록
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-04-01
// 작성내용 : 창고입고등록 및 관리
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
using System.Threading;
using System.IO;
using System.Reflection;

namespace MI.MIM002
{
    public partial class MIM002 : UIForm.FPCOMM1
    {
        #region 변수선언
        string strBtn = "N";
        bool form_act_chk = false;
        #endregion

        #region 생성자
        public MIM002()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load시
        private void MIM002_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            SystemBase.Validation.GroupBox_Setting(groupBox2);

            SystemBase.ComboMake.C1Combo(cboPlantCd, "usp_B_COMMON @pType='TABLE2', @pCODE = 'PLANT_CD', @pNAME = 'PLANT_NM', @pSPEC1 = 'B_PLANT_INFO', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//공장
               
            dtpSlMvmtDt.Text = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0,10);
            cboPlantCd.SelectedValue = SystemBase.Base.gstrPLANT_CD;
            txtSlCd.Text = "W03";
            rdoSlMvmt_N.Checked = true;
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            SystemBase.Validation.GroupBox_Reset(groupBox2);
            fpSpread1.Sheets[0].Rows.Count = 0;
            
            dtpSlMvmtDt.Text = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0,10);
            cboPlantCd.SelectedValue = SystemBase.Base.gstrPLANT_CD;
            txtSlCd.Text = "W03";
            rdoSlMvmt_N.Checked = true;
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            try
            {
                string strDiv = "";
                if (rdoSlMvmt_Y.Checked == true) { strDiv = "S1"; }
                else if (rdoSlMvmt_N.Checked == true) { strDiv = "S2"; }
                else strDiv = "S3";

                string strBadQty = "N";
                if (chkBadQty.Checked == true)
                {
                    strBadQty = "Y";
                }

                string strQuery = " usp_MIM002  @pTYPE = '" + strDiv + "'";
                strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' ";
                strQuery += ", @pMVMT_DT_FR = '" + dtpMvmtDtFr.Text + "' ";
                strQuery += ", @pMVMT_DT_TO = '" + dtpMvmtDtTo.Text + "' ";
                strQuery += ", @pPLANT_CD = '" + cboPlantCd.SelectedValue.ToString() + "' ";
                strQuery += ", @pMVMT_NO = '" + txtMvmtNo.Text.Trim() + "' ";
                strQuery += ", @pTRAN_NO = '" + txtTranNo.Text.Trim() + "' ";
                strQuery += ", @pPO_NO = '" + txtPoNo.Text.Trim() + "' ";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                strQuery += ", @pCHK_BAD_QTY = '" + strBadQty + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0);
                if (fpSpread1.Sheets[0].RowCount > 0) Set_Locking(strDiv);
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }

            this.Cursor = System.Windows.Forms.Cursors.Default;
        }

        private void Set_Locking(string div)
        {
            decimal sum = 0;
            int idx = SystemBase.Base.GridHeadIndex(GHIdx1, "입고자국금액");
            if (div == "S1")
            {
                for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                {
                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "매입여부")].Value.ToString() == "0")
                    {
                        UIForm.FPMake.grdReMake(fpSpread1, i,
                            SystemBase.Base.GridHeadIndex(GHIdx1, "선택") + "|0"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "창고입고일자") + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "창고입고처리자") + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "창고입고일자") + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "입고창고") + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "입고창고_2") + "|5"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "Location") + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "Location_2") + "|5"
                            );
                    }
                    else
                    {
                        UIForm.FPMake.grdReMake(fpSpread1, i,
                            SystemBase.Base.GridHeadIndex(GHIdx1, "선택") + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "창고입고일자") + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "창고입고처리자") + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "입고창고") + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "입고창고_2") + "|5"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "Location") + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "Location_2") + "|5"
                            );
                    }
                    sum += Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, idx].Value);
                }
            }
            else if (div == "S2")
            {
                for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                {
                    UIForm.FPMake.grdReMake(fpSpread1, i,
                        SystemBase.Base.GridHeadIndex(GHIdx1, "선택") + "|0"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "창고입고일자") + "|1"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "입고창고") + "|1"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "입고창고_2") + "|0"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "Location") + "|1"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "Location_2") + "|0"
                        );
                }
            }
            else
            {
                for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                {
                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "선택")].Text == "True")
                    {
                        sum += Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, idx].Value);

                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "매입여부")].Value.ToString() == "0")
                        {
                            UIForm.FPMake.grdReMake(fpSpread1, i,
                                SystemBase.Base.GridHeadIndex(GHIdx1, "선택") + "|0"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "창고입고일자") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "창고입고처리자") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "입고창고") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "입고창고_2") + "|5"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "Location") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "Location_2") + "|5"
                                );
                        }
                        else
                        {
                            UIForm.FPMake.grdReMake(fpSpread1, i,
                                SystemBase.Base.GridHeadIndex(GHIdx1, "선택") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "창고입고일자") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "창고입고처리자") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "입고창고") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "입고창고_2") + "|5"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "Location") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "Location_2") + "|5"
                                );
                        }
                    }
                    else
                    {
                        UIForm.FPMake.grdReMake(fpSpread1, i,
                            SystemBase.Base.GridHeadIndex(GHIdx1, "선택") + "|0"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "창고입고일자") + "|1"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "입고창고") + "|1"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "입고창고_2") + "|0"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "Location") + "|1"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "Location_2") + "|0"
                            );
                    }
                }

            }
            txtSum.Value = sum;
        }
        #endregion

        #region SaveExec() 폼에 입력된 데이타 저장 로직
        protected override void SaveExec()
        {
			this.Cursor = Cursors.WaitCursor;
			// 그리드 상단 필수항목 체크
			if (SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", false))
			{
				string ERRCode = "WR", MSGCode = "M0014"; //처리할 내용이 없습니다.
				SqlConnection dbConn = SystemBase.DbOpen.DBCON();
				SqlCommand cmd = dbConn.CreateCommand();
				SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);
	
				try
				{  
					string chk = "";
					string tranNo = ""; 
					string in_dt = "";
					string chk_first = "N";

					//행수만큼 처리
					for(int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
					{
						if ((fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "창고입고번호")].Text == "" && fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "선택")].Text == "True")
							|| (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "창고입고번호")].Text != "" && fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "선택")].Text != "True"))
						{
							string strGbn = "U1";

							string strSql = " usp_MIM002 '" + strGbn + "'";
							strSql += ", @pMVMT_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "구매입고번호")].Text + "'";
							strSql += ", @pMVMT_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "구매입고순번")].Text + "'";
							strSql += ", @pITEM_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text + "'";
							strSql += ", @pMVMT_UNIT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "입고단위")].Text + "'";
							if( fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "선택")].Text == "True")
							{									
								strSql += ", @pSL_MVMT_YN = 'Y' "; 								
								chk = "Y"; 								
								if(chk_first == "N")  chk_first = "Y";
							}
							else
							{ 
								strSql += ", @pSL_MVMT_YN = 'N' "; 
								chk = "N";
							}
							strSql += ", @pSL_MVMT_DT = '" +  fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "창고입고일자")].Text  + "'";
							strSql += ", @pSL_MVMT_ID = '" +  fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "창고입고처리자")].Text.Trim()  + "'";
							strSql += ", @pMVMT_QTY = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "창고입고량")].Value + "'";
							strSql += ", @pMVMT_AMT = " + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "입고금액")].Value;
							strSql += ", @pMVMT_AMT_LOC = " + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "입고자국금액")].Value;
							strSql += ", @pPO_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "발주번호")].Text + "'";
							strSql += ", @pPO_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "발주순번")].Text + "'";

							strSql += ", @pSL_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "입고창고")].Value + "'";
							strSql += ", @pLOCATION_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Location")].Value + "'";

							if(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "창고입고번호")].Text.Trim()  == "")
							{
								if(chk == "Y")
								{
									if(chk_first == "Y")
									{
										strSql += ", @pTRAN_NO = ''";
										chk_first = "T";
									}
									else if(in_dt == fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "창고입고일자")].Text)
										strSql += ", @pTRAN_NO = '"+tranNo+"'";
									else
										strSql += ", @pTRAN_NO = ''";
								}
							}
							else
								strSql += ", @pTRAN_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "창고입고번호")].Text + "'";
							strSql += ", @pTRAN_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "창고입고순번")].Text + "'";
							strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                            strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

							DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
							ERRCode = ds.Tables[0].Rows[0][0].ToString();
							MSGCode	= ds.Tables[0].Rows[0][1].ToString();

							if(ERRCode != "OK"){Trans.Rollback();goto Exit;}	// ER 코드 Return시 점프
							if(ERRCode == "OK" && chk == "Y") 
							{
								tranNo = ds.Tables[0].Rows[0][3].ToString(); 
								in_dt =  fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "창고입고일자")].Text;
//								txtTranNo.Text = tranNo;
//								rdoSlMvmt_Y.Checked = true;
								rdoSlMvmt_N.Checked = true;
							}
							else
							{
								txtTranNo.Text = "";
								rdoSlMvmt_N.Checked = true;
							}
						}
					}
					Trans.Commit();
				}
				catch(Exception e)
				{
					SystemBase.Loggers.Log(this.Name, e.ToString());
					Trans.Rollback();
					ERRCode = "ER";
					MSGCode = e.Message;
					//MSGCode = "P0001";	//에러가 발생되어 데이터 처리가 취소되었습니다.
				}
			Exit:
				dbConn.Close();
				if(ERRCode == "OK")
				{					
					MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
					SearchExec();
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

        #region 버튼 click
        private void btnSlMvmtId_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                string strQuery = " usp_B_COMMON 'B013' ,@pSPEC1='" + SystemBase.Base.gstrBIZCD + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtSlMvmtId.Text, "" };
                
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P04003", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "사용자 팝업");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {

                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtSlMvmtId.Text = Msgs[0].ToString();
                    txtSlMvmtNm.Value = Msgs[1].ToString();

                    Set_Change();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
            strBtn = "N";

        }

        private void btnMvmtNo_Click(object sender, System.EventArgs e)
        {
            try
            {
                string in_yn = "";

                if (rdoSlMvmt_Y.Checked == true) in_yn = "Y";
                else if (rdoSlMvmt_N.Checked == true) in_yn = "N";

                WNDW019 frm1 = new WNDW019(in_yn);

                frm1.ShowDialog();
                if (frm1.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = frm1.ReturnVal;
                    txtMvmtNo.Text = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(f.Message, SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSl_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = "usp_I_COMMON @pTYPE ='I010', @pSPEC1 = '" + SystemBase.Base.gstrBIZCD + "', @pSPEC2 = '" + SystemBase.Base.gstrPLANT_CD + "' , @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtSlCd.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00056", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "창고 조회");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtSlCd.Text = Msgs[0].ToString();
                    txtSlNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        private void btnLocation_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (txtSlCd.Text.Trim() == "")
                {
                    MessageBox.Show("창고 먼저 선택하세요!");
                    txtSlCd.Focus();
                    return;
                }
                string strQuery = " usp_B_COMMON 'B036', @pSPEC1 = '" + txtSlCd.Text + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtLocation.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00030", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "창고위치팝업");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtLocation.Text = Msgs[0].ToString();
                    txtLocationNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        private void btnTranNo_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW.WNDW020 pu = new WNDW.WNDW020();
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtTranNo.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "창고입고정보조회 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnPoNo_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW018 myForm = new WNDW018();
                myForm.ShowDialog();
                if (myForm.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = myForm.ReturnVal;

                    txtPoNo.Text = Msgs[1].ToString();
                    txtPoNo.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "발주번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);	//데이터 조회 중 오류가 발생하였습니다.
            }
        }
        #endregion

        #region TextChanged
        private void txtSlMvmtId_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (strBtn == "N")
                {
                    txtSlMvmtNm.Text = SystemBase.Base.CodeName("USR_ID", "USR_NM", "B_SYS_USER", txtSlMvmtId.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                    if (txtSlMvmtId.Text != "")
                    {
                        txtSlMvmtNm.Value = SystemBase.Base.CodeName("USR_ID", "USR_NM", "B_SYS_USER", txtSlMvmtId.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                    }
                    else
                    {
                        txtSlMvmtNm.Value = "";
                    }
                    Set_Change();
                }               
            }
            catch
            {

            }
        }

        private void dtpSlMvmtDt_TextChanged(object sender, System.EventArgs e)
        {
            Set_Change();
        }


        private void Set_Change()
        {
            for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
            {
                if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "매입여부")].Text == "0")
                {
                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "선택")].Text == "True" && fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "창고입고번호")].Text == "")
                    {
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "창고입고일자")].Text = dtpSlMvmtDt.Text;
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "창고입고처리자")].Text = txtSlMvmtId.Text;
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "창고입고처리자명")].Text = txtSlMvmtNm.Text;
                        if (txtSlCd.Text.Trim() != "")
                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "입고창고")].Text = txtSlCd.Text;
                        if (txtLocation.Text.Trim() != "")
                        {
                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Location")].Text = txtLocation.Text;
                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Location명")].Text = txtLocationNm.Text;
                        }

                    }
                }
            }
        }

        private void txtSlCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtSlCd.Text != "")
                {
                    txtSlNm.Value = SystemBase.Base.CodeName("SL_CD", "SL_NM", "B_STORAGE_INFO", txtSlCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtSlNm.Value = "";
                }
                txtLocation.Text = "";
                txtLocationNm.Value = "";
            }
            catch
            {

            }
        }

        private void txtLocation_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtSlCd.Text.Trim() == "")
                {
                    DialogResult dsMsg = MessageBox.Show("창고 먼저 선택하세요!", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtSlCd.Focus();
                    txtLocation.Text = "";
                    txtLocationNm.Value = "";
                }
                else
                {
                    if (txtLocation.Text != "")
                    {
                        if (txtLocation.Text != "")
                        {
                            txtLocationNm.Value = SystemBase.Base.CodeName("LOCATION_CD", "LOCATION_NM", "B_LOCATION_INFO", txtLocation.Text, " AND SL_CD ='" + txtSlCd.Text + "' AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                        }
                        else
                        {
                            txtLocationNm.Value = "";
                        }
                        if (txtLocationNm.Text != "")
                        {
                            if (fpSpread1.Sheets[0].Rows.Count > 0)
                            {
                                for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                                {
                                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "선택")].Text == "True")
                                    {
                                        if (txtLocation.Text != "")
                                        {
                                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Location")].Text = txtLocation.Text;
                                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Location명")].Text = txtLocationNm.Text;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            //창고에 소속된 location이 아닙니다.
                            MessageBox.Show(SystemBase.Base.MessageRtn("M0013"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);

                            txtLocation.Focus();
                        }
                    }
                }
            }
            catch
            {

            }
            
        }
        #endregion

        #region 선택버튼
        private void btnSelectAll_Click(object sender, System.EventArgs e)
        {
            for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
            {
                if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "창고입고번호")].Text == "")
                {
                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "선택")].Text != "True")
                    {
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "선택")].Value = 1;
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "창고입고일자")].Text = dtpSlMvmtDt.Text;
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "창고입고처리자")].Text = txtSlMvmtId.Text;
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "창고입고처리자명")].Text = txtSlMvmtNm.Text;
                        if (txtSlCd.Text.Trim() != "")
                        {
                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "입고창고")].Text = txtSlCd.Text;
                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "창고명")].Text = txtSlNm.Text;
                        }
                        if (txtLocation.Text.Trim() != "")
                        {
                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Location")].Text = txtLocation.Text;
                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Location명")].Text = txtLocationNm.Text;
                        }                        
                        UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "창고입고일자") + "|1"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "창고입고처리자") + "|1"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "입고창고") + "|1"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "입고창고_2") + "|0"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "Location") + "|1"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "Location_2") + "|0");

                    }
                }
            }
        }

        private void btnSelectCancel_Click(object sender, System.EventArgs e)
        {
            for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
            {

                if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "창고입고번호")].Text != "")
                {
                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "선택")].Text == "True")
                    {
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "선택")].Value = 0;
                        UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "창고입고일자") + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "창고입고처리자") + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "입고창고") + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "입고창고_2") + "|5"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "Location") + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "Location_2") + "|5");
                    }
                }
                else
                {
                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "선택")].Text == "True")
                    {
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "선택")].Value = 0;
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "창고입고일자")].Text = "";
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "창고입고처리자")].Text = "";
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "창고입고처리자명")].Text = "";

                        UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "창고입고일자") + "|1"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "창고입고처리자") + "|1"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "입고창고") + "|1"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "입고창고_2") + "|0"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "Location") + "|1"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "Location_2") + "|0");
                    }
                }
            }
        }
        #endregion

        #region fpButtonClick
        protected override void fpButtonClick(int Row, int Column)
        {
            try
            {
                //선택
                if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "선택"))
                {
                    if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "선택")].Text != "True")
                    {
                        if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고입고처리자명")].Text == "")
                        {
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고입고일자")].Text = "";
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고입고처리자")].Text = "";
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고입고처리자명")].Text = "";

                            UIForm.FPMake.grdReMake(fpSpread1, Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고입고일자") + "|1"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "창고입고처리자") + "|1"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "입고창고") + "|1"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "입고창고_2") + "|0"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "Location") + "|1"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "Location_2") + "|0");
                        }
                        else
                        {
                            UIForm.FPMake.grdReMake(fpSpread1, Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고입고일자") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "창고입고처리자") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "입고창고") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "입고창고_2") + "|5"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "Location") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "Location_2") + "|5");
                        }
                    }
                    else
                    {

                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고입고일자")].Text = dtpSlMvmtDt.Text;
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고입고처리자")].Text = txtSlMvmtId.Text;
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고입고처리자명")].Text = txtSlMvmtNm.Text;

                        if (txtSlCd.Text.Trim() != "")
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "입고창고")].Text = txtSlCd.Text;
                        if (txtLocation.Text.Trim() != "")
                        {
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "Location")].Text = txtLocation.Text;
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "Location명")].Text = txtLocationNm.Text;
                        }

                        UIForm.FPMake.grdReMake(fpSpread1, Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고입고일자") + "|1"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "창고입고처리자") + "|1"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "입고창고") + "|1"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "입고창고_2") + "|0"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "Location") + "|1"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "Location_2") + "|0");

                    }
                }
                else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "입고창고_2"))
                {
                    string strQuery = " usp_B_COMMON 'B035', @pSPEC1 = '" + fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "공장")].Text + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                    string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                    string[] strSearch = new string[] { fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "입고창고")].Text, "" };

                    UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00014", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "창고팝업");
                    pu.ShowDialog();

                    if (pu.DialogResult == DialogResult.OK)
                    {
                        Regex rx1 = new Regex("#");
                        string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "입고창고")].Text = Msgs[0].ToString();
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고명")].Text = Msgs[1].ToString();

                    }
                }
                else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "Location_2"))
                {
                    string strQuery = " usp_B_COMMON 'B036', @pSPEC1 = '" + fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "입고창고")].Value + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                    string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                    string[] strSearch = new string[] { fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "Location")].Text, "" };

                    UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00030", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "창고위치팝업");
                    pu.ShowDialog();

                    if (pu.DialogResult == DialogResult.OK)
                    {
                        Regex rx1 = new Regex("#");
                        string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "Location")].Text = Msgs[0].ToString();
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "Location명")].Text = Msgs[1].ToString();
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(f.Message, SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 그리드 상 Change
        protected override void fpSpread1_ChangeEvent(int Row, int Column)
        {
            try
            {
                // 창고 
                if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "입고창고"))
                {
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고명")].Text
                        = SystemBase.Base.CodeName("SL_CD", "SL_NM", "B_STORAGE_INFO", fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "입고창고")].Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                //위치
                else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "Location"))
                {
                    if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "입고창고")].Text.Trim() == "")
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "Location명")].Text = "";
                    else
                    {
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "Location명")].Text
                            = SystemBase.Base.CodeName("LOCATION_CD", "LOCATION_NM", "B_LOCATION_INFO", fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "Location")].Text, " AND SL_CD ='" + fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "입고창고")].Text + "' AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");

                        if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "Location명")].Text == "")
                        {
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "Location")].Text = "";
                            //창고에 소속된 location이 아닙니다.
                            MessageBox.Show(SystemBase.Base.MessageRtn("M0013"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }

                }
                else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "창고입고처리자"))
                {
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고입고처리자명")].Text
                        = SystemBase.Base.CodeName("USR_ID", "USR_NM", "B_SYS_USER", fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고입고처리자")].Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(f.Message, SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 폼 활성화 시
        private void MIM002_Activated(object sender, System.EventArgs e)
        {
            if (form_act_chk == false) dtpMvmtDtFr.Focus();
        }
        #endregion

        #region 폼 비활성화 시
        private void MIM002_Deactivate(object sender, System.EventArgs e)
        {
            form_act_chk = true;
        }
        #endregion

        #region 헤드 U 없애기
        private void fpSpread1_EditChange(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            fpSpread1.Sheets[0].RowHeader.Cells[e.Row, 0].Text = "";
        }

        private void fpSpread1_ButtonClicked(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            fpSpread1.Sheets[0].RowHeader.Cells[e.Row, 0].Text = "";
        }
        #endregion
    }
}
