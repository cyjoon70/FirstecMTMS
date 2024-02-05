#region 작성정보
/*********************************************************************/
// 단위업무명 : 
// 작 성 자   : 김 창 진
// 작 성 일   : 2014-08-29
// 작성내용   : 첨부문서 조회
// 수 정 일   :
// 수 정 자   :
// 수정내용   :
// 비    고   :
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
using System.Text.RegularExpressions;
using FarPoint.Win.Spread;
using EDocument.Extensions.FpSpreadExtension;
using EDocument.Network;
using EDocument.Spread;
using WNDW;

namespace QT.QTC004
{
    public partial class QTC004P1 : UIForm.FPCOMM1
    {
        #region 변수선언

        string strPlantCd = "";
        string strProjectNo = "";
        string strProjectSeq = "";
        string strLotNo = "";
        string strItemCd = "";
        string strUnit = "";
        string strInspQty = "";
        string strMvmtNo = "";
        string strMvmtSeq = "";
        string strItemAcct = "";

        #endregion

        #region 문서관련 변수
        // 디테일 그리드 컬럼(문서 목록)
        int colDocId = -1;
        int colDocMvntSeq = -1;
        int colDocBarCode = -1;
        int colDocItemCd = -1;
        int colDocItemNm = -1;
        int colSvrPath = -1;
        int colSvrFnm = -1;
        int colOrgFnm = -1;
        int colFileExt = -1;
        int colDocCd = -1;
        int colDocNm = -1;
        int colDocNo = -1;
        int colRevNo = -1;
        int colRemark = -1;
        int colRegUsrId = -1;
        int colRegUsrNm = -1;

        /// <summary>첨부파일목록 파일버튼 관리자</summary>
        FileButtonManager buttonManager;
        #endregion

        #region 생성자
        public QTC004P1(string sPlantCd, string sProjectNo, string sProjectSeq, string sLotNo, string sItemCd, string sItemAcct, string sUnit, string sInspQty, string sMvmtNo, string sMvmtSeq)
        {
            strPlantCd = sPlantCd;
            strProjectNo = sProjectNo;
            strProjectSeq = sProjectSeq;
            strLotNo = sLotNo;
            strItemCd = sItemCd;
            strUnit = sUnit;
            strInspQty = sInspQty;
            strMvmtNo = sMvmtNo;
            strMvmtSeq = sMvmtSeq;
            strItemAcct = sItemAcct;

            InitializeComponent();
        }

        public QTC004P1()
        {
            InitializeComponent();
        }
        #endregion

        #region 폼로드 이벤트
        private void QTC004P1_Load(object sender, System.EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            this.Text = "첨부문서";

            SystemBase.ComboMake.C1Combo(cboPlantCd, "usp_B_COMMON @pTYPE = 'PLANT', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'"); //공장

            cboPlantCd.SelectedValue = strPlantCd;
            txtProjectNo.Value = strProjectNo;
            txtProjectSeq.Value = strProjectSeq;
            txtLotNo.Value = strLotNo;
            txtItemCd.Value = strItemCd;
            txtUnit.Value = strUnit;
            txtInspQty.Value = strInspQty;
            txtMvmtNo.Value = strMvmtNo;
            txtMvmtSeq.Value = strMvmtSeq;
            txtItemAcct.Value = strItemAcct;

            if(strItemAcct == "10" || strItemAcct == "20")
            {
                c1Label3.Text = "오더번호";
                c1Label8.Visible = false;
                txtMvmtSeq.Visible = false;
            }

            #region 문서관련
            // 컬럼 인덱스
			SheetView sheet = fpSpread1.ActiveSheet;
			colDocId = sheet.FindHeaderColumnIndex("문서ID");
			colDocMvntSeq = sheet.FindHeaderColumnIndex("입고순번");
			colDocBarCode = sheet.FindHeaderColumnIndex("바코드");
			colDocItemCd = sheet.FindHeaderColumnIndex("품목코드");
			colDocItemNm = sheet.FindHeaderColumnIndex("품목명");
			colSvrPath = sheet.FindHeaderColumnIndex("서버경로");
			colSvrFnm = sheet.FindHeaderColumnIndex("서버파일명");
			colOrgFnm = sheet.FindHeaderColumnIndex("파일명") + 3; // 파일선택 버튼, 미리보기 버튼, 다운로드 버튼 다음이 파일명 컬럼
			colFileExt = sheet.FindHeaderColumnIndex("파일확장자");
			colDocCd = sheet.FindHeaderColumnIndex("문서코드");
			colDocNm = sheet.FindHeaderColumnIndex("문서종류");
			colDocNo = sheet.FindHeaderColumnIndex("문서번호");
			colRevNo = sheet.FindHeaderColumnIndex("개정번호");
			colRemark = sheet.FindHeaderColumnIndex("비고");
			colRegUsrId = sheet.FindHeaderColumnIndex("등록자ID");
			colRegUsrNm = sheet.FindHeaderColumnIndex("등록자");


            // 첨부파일목록 파일버튼 관리자 초기화
            buttonManager = new FileButtonManager(fpSpread1.ActiveSheet, FileButtonManager.ServerFileType.DocumentFile)
            {
                FilenameColumnIndex = colOrgFnm,
                ServerPathColumnIndex = colSvrPath,
                ServerFilenameColumnIndex = colSvrFnm,
                FileSelectButtonColumnIndex = colOrgFnm - 3,
                FileViewButtonColumnIndex = colOrgFnm - 2,
                FileDownloadButtonColumnIndex = colOrgFnm - 1,
                DocTypeNameColumnIndex = colDocNm,
                DocRevisionColumnIndex = colRevNo,
                DocNumberColumnIndex = colDocNo,
            };
            #endregion


            //버튼 재정의(조회권한만)
            UIForm.Buttons.ReButton("010000000001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

            SearchExec();
        }
        #endregion
        
        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            try
            {

                string strPType = "";
                if (txtItemAcct.Text == "10" || txtItemAcct.Text == "20")
                {
                    strPType = "P2";
                }
                else
                {
                    strPType = "P1";
                }

                string strQuery = " usp_QTC004_NEW ";
                strQuery += "  @pTYPE = '" + strPType + "' ";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                strQuery += ", @pPLANT_CD = '" + strPlantCd + "' ";
                strQuery += ", @pLOT_NO = '" + txtLotNo.Text + "' ";
                strQuery += ", @pMVMT_NO = '" + txtMvmtNo.Text + "' ";
                strQuery += ", @pMVMT_SEQ = '" + txtMvmtSeq.Text + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0, true);

                buttonManager.UpdateButtons(); // 버튼 업데이트

                if (strPType == "P2")
                {
                    fpSpread1.Sheets[0].ColumnHeader.Cells[0, 2].Text = "오더번호";
                    fpSpread1.Sheets[0].ColumnHeader.Cells[0, 3].Text = "공정순번";
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            this.Cursor = System.Windows.Forms.Cursors.Default;
        }
        #endregion

        #region TextChanged
        //품목코드
        private void txtItemCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtItemCd.Text != "")
                {
                    txtItemNm.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtItemCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                    txtItemSpec.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_SPEC", "B_ITEM_INFO", txtItemCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtItemNm.Value = "";
                    txtItemSpec.Value = "";
                }
            }
            catch
            {

            }
        }
        

        //프로젝트번호
        private void txtProjectNo_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtProjectNo.Text != "")
                {
                    txtProjectNm.Value = SystemBase.Base.CodeName("PROJECT_NO", "PROJECT_NM", "S_SO_MASTER", txtProjectNo.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtProjectNm.Value = "";
                }
            }
            catch
            {

            }

            if (txtProjectNm.Text == "")
            {
                txtProjectSeq.Value = "";
            }
        }
        #endregion

        #region 그리드 이벤트 핸들러
        private void fpSpread1_ButtonClicked(object sender, EditorNotifyEventArgs e)
        {
            try
            {
                if (fpSpread1.Sheets[0].GetCellType(e.Row, e.Column).ToString() == "ButtonCellType")
                {
                    fpSpread1.Sheets[0].RowHeader.Cells[e.Row, 0].Text = "";

                    fpSpread1.Sheets[0].RowHeader.Rows[e.Row].BackColor = SystemBase.Base.Color_Org;
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }
        #endregion
    }
}
