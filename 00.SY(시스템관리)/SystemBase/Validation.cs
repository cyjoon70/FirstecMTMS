#region Validation 작성 정보
/*************************************************************/
// 단위업무명 : 조회, 저장, 신규시 GroupBox 처리된 각 컨트롤들에 대한 체크사항
// 작 성 자 :   전 성 표
// 작 성 일 :   2012-10-16
// 작성내용 :   
// 수 정 일 :
// 수 정 자 :
// 수정내용 :
// 비    고 : Tag값 --> 타이틀;필수여부;Key값여부;len;검색조건   TagData[0], TagData[1], TagData[2], TagData[3] , TagData[4]
// 참    고 : ctb.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper; //영문 대문자만 사용시
/*************************************************************/
#endregion

using System;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Reflection;
using C1.Win.C1Input;
using C1.Win.C1List;

namespace SystemBase
{
    public class Validation
    {
        #region Color 정의
        public static System.Drawing.Color Kind_LightCyan = Color.LightSkyBlue;//System.Drawing.Color.FromArgb(242, 252, 254);	// 필수 입력
        public static System.Drawing.Color Kind_Gainsboro = System.Drawing.Color.FromArgb(239, 239, 239);   // 읽기전용
        public static System.Drawing.Color Kind_White = System.Drawing.Color.White;	
        public static System.Drawing.Color Kind_Linen = System.Drawing.Color.Linen;
        #endregion

        #region GroupBox Reset 체크 (신규처리)
        public static void GroupBox_Reset(GroupBox groupBox)
        {
            GroupBox_Reset2(groupBox);
        }

        private static void GroupBox_Reset2(Control ctls)
        {
            foreach (System.Windows.Forms.Control c in ctls.Controls)
            {

                switch (c.GetType().Name)
                {
                    case "C1Combo":
                        C1.Win.C1List.C1Combo cbo = (C1.Win.C1List.C1Combo)c;                        
                        if (cbo.ListCount > 0)
                        {
                            cbo.SelectedIndex = 0;
                        }
                        break;
                    case "C1TextBox":
                        C1.Win.C1Input.C1TextBox ctb = (C1.Win.C1Input.C1TextBox)c;
                        ctb.Value = null;
                        break;

                    case "C1NumericEdit":
                        C1.Win.C1Input.C1NumericEdit cne = (C1.Win.C1Input.C1NumericEdit)c;
                        cne.Value = 0;
                        break;

                    case "C1DateEdit":
                        C1.Win.C1Input.C1DateEdit cde = (C1.Win.C1Input.C1DateEdit)c;
                        cde.EmptyAsNull = true;
                        //cde.Value = Convert.ToDateTime(SystemBase.Base.ServerTime(""));
                        cde.Value = null;
                        break;

                    case "C1CheckBox":
                        C1.Win.C1Input.C1CheckBox chk = (C1.Win.C1Input.C1CheckBox)c;
                        chk.Checked = false;
                        break;
                }

                GroupBox_Reset2(c);
            }
        }
        #endregion

        #region GroupBox Settingn 체크 (폼 로딩시 신규처리후)
        public static void GroupBox_Setting(GroupBox groupBox)
        {
            GroupBox_Setting2(groupBox);
        }

        private static void GroupBox_Setting2(Control ctls)
        {
            try
            {
                // Tag값의 필수여부로만 체크하여 읽기 , 쓰기, Enable 처리
                string[] TagData = null;

                foreach (System.Windows.Forms.Control c in ctls.Controls)
                {
                    #region 컨트롤 체크
                    if (c.GetType().Name == "C1Combo")
                    {
                        C1.Win.C1List.C1Combo cbo = (C1.Win.C1List.C1Combo)c;
                        if (cbo.Tag != null && cbo.Tag.ToString() != "")
                        {
                            // 타이틀;필수여부;Key값여부;Length;검색조건
                            TagData = cbo.Tag.ToString().Split(';');

                            if (TagData[1] == "1")
                            {	// 필수항목
                                cbo.EditorBackColor = Kind_LightCyan;
                                cbo.Enabled = true;

                            }
                            else if (TagData[1] == "2")
                            {	// 읽기전용
                                cbo.EditorBackColor = Kind_Gainsboro;
                                cbo.Enabled = false;
                            }
                            else
                            {
                                cbo.EditorBackColor = Kind_White;
                                cbo.Enabled = true;
                            }
                        }
                        else
                        {
                            cbo.EditorBackColor = Kind_White;
                            cbo.Enabled = true;
                        }
                    }
                    else if (c.GetType().Name == "C1TextBox")
                    {
                        C1.Win.C1Input.C1TextBox ctb = (C1.Win.C1Input.C1TextBox)c;
                        if (ctb.Tag != null && ctb.Tag.ToString() != "")
                        {
                            // 타이틀;필수여부;Key값여부;Length;검색조건
                            TagData = ctb.Tag.ToString().Split(';');

                            if (TagData[1] == "1")
                            {	// 필수항목

                                ctb.BackColor = Kind_LightCyan;
                                ctb.ReadOnly = false;

                            }
                            else if (TagData[1] == "2")
                            {	// 읽기전용
                                ctb.BackColor = Kind_Gainsboro;
                                ctb.ReadOnly = true;
                            }
                            else
                            {
                                ctb.BackColor = Kind_White;
                                ctb.ReadOnly = false;
                            }
                        }
                        else
                        {
                            ctb.BackColor = Kind_White;
                            ctb.ReadOnly = false;
                        }
                    }
                    else if (c.GetType().Name == "C1NumericEdit")
                    {
                        C1.Win.C1Input.C1NumericEdit cne = (C1.Win.C1Input.C1NumericEdit)c;
                        if (cne.Tag != null && cne.Tag.ToString() != "")
                        {
                            // 타이틀;필수여부;Key값여부;Length;검색조건
                            TagData = cne.Tag.ToString().Split(';');

                            if (TagData[1] == "1")
                            {	// 필수항목
                                cne.BackColor = Kind_LightCyan;
                                cne.ReadOnly = false;

                            }
                            else if (TagData[1] == "2")
                            {	// 읽기전용
                                cne.BackColor = Kind_Gainsboro;
                                cne.ReadOnly = true;
                            }
                            else
                            {
                                cne.BackColor = Kind_White;
                                cne.ReadOnly = false;
                            }
                        }
                        else
                        {
                            cne.BackColor = Kind_White;
                            cne.ReadOnly = false;
                        }
                    }
                    else if (c.GetType().Name == "C1DateEdit")
                    {
                        C1.Win.C1Input.C1DateEdit cde = (C1.Win.C1Input.C1DateEdit)c;
                        cde.EmptyAsNull = true;
                        if (cde.Tag != null && cde.Tag.ToString() != "")
                        {
                            // 타이틀;필수여부;Key값여부;Length;검색조건
                            TagData = cde.Tag.ToString().Split(';');

                            if (TagData[1] == "1")
                            {	// 필수항목
                                cde.BackColor = Kind_LightCyan;
                                cde.ReadOnly = false;

                            }
                            else if (TagData[1] == "2")
                            {	// 읽기전용
                                cde.BackColor = Kind_Gainsboro;
                                cde.ReadOnly = true;
                            }
                            else
                            {
                                cde.BackColor = Kind_White;
                                cde.ReadOnly = false;
                            }
                        }
                        else
                        {
                            cde.BackColor = Kind_White;
                            cde.ReadOnly = false;
                        }
                    }
                    else if (c.GetType().Name == "C1Button")
                    {
                        C1.Win.C1Input.C1Button cbt = (C1.Win.C1Input.C1Button)c;
                        if (cbt.Tag != null && cbt.Tag.ToString() != "")
                        {
                            // 타이틀;필수여부;Key값여부;Length;검색조건
                            TagData = cbt.Tag.ToString().Split(';');

                            if (TagData[1] == "1")
                            {	// 필수항목                           
                                cbt.Enabled = true;
                            }
                            else if (TagData[1] == "2")
                            {	// 읽기전용                           
                                cbt.Enabled = false;
                            }
                            else
                            {
                                cbt.Enabled = true;
                            }
                        }
                        else
                        {
                            cbt.Enabled = true;
                        }
                    }
                    else if (c.GetType().Name == "RadioButton")
                    {
                        System.Windows.Forms.RadioButton rdo = (System.Windows.Forms.RadioButton)c;
                        if (rdo.Tag != null && rdo.Tag.ToString() != "")
                        {
                            // 타이틀;필수여부;Key값여부;Length;검색조건
                            TagData = rdo.Tag.ToString().Split(';');

                            if (TagData[1] == "1")
                            {	// 필수항목                           
                                rdo.Enabled = true;
                            }
                            else if (TagData[1] == "2")
                            {	// 읽기전용                           
                                rdo.Enabled = false;
                            }
                            else
                            {
                                rdo.Enabled = true;
                            }
                        }
                        else
                        {
                            rdo.Enabled = true;
                        }
                    }
                    else if (c.GetType().Name == "C1CheckBox")
                    {
                        C1.Win.C1Input.C1CheckBox chk = (C1.Win.C1Input.C1CheckBox)c;
                        if (chk.Tag != null && chk.Tag.ToString() != "")
                        {
                            // 타이틀;필수여부;Key값여부;Length;검색조건
                            TagData = chk.Tag.ToString().Split(';');

                            if (TagData[1] == "1")
                            {	// 필수항목                           
                                chk.Enabled = true;
                            }
                            else if (TagData[1] == "2")
                            {	// 읽기전용                           
                                chk.Enabled = false;
                            }
                            else
                            {
                                chk.Enabled = true;
                            }
                        }
                        else
                        {
                            chk.Enabled = true;
                        }
                    }

                    #endregion

                    GroupBox_Setting2(c);
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(ctls.Name + "  :  " + f.ToString());
            }
        }
        #endregion

        #region GroupBox SaveSearch Validation 체크 (저장시, 조회시 컨트롤 체크)
        public static bool GroupBox_SaveSearchValidation(GroupBox groupBox)
        {
            return GroupBox_SaveSearchValidation2(groupBox);
        }

        private static bool GroupBox_SaveSearchValidation2(Control ctls)
        {
            bool Rtn = true;
            string[] TagData = null;
            string MsgTag = "";
            string MsgGubun = "";  

            foreach (System.Windows.Forms.Control c in ctls.Controls)
            {
                switch (c.GetType().Name)
                {
                    #region C1Combo
                    case "C1Combo":
                        C1.Win.C1List.C1Combo cbo = (C1.Win.C1List.C1Combo)c;
                        if (cbo.Tag != null && cbo.Tag.ToString() != "")
                        {
                            // 타이틀;필수여부;Key값여부;Length;검색조건
                            TagData = cbo.Tag.ToString().Split(';');                          
                            MsgTag = TagData[0];
                        }
                        else break;   //Tag값 없으면 Skip

                        if (cbo.Text == "" && TagData[1] == "1" )
                        {
                            cbo.EditorBackColor = Kind_Linen;
                            cbo.Focus();
                            Rtn = false;
                            MsgGubun = "1";
                        }
                        break;
                    #endregion

                    #region C1TextBox
                    case "C1TextBox":
                        C1.Win.C1Input.C1TextBox ctb = (C1.Win.C1Input.C1TextBox)c;
                        if (ctb.Tag != null && ctb.Tag.ToString() != "")
                        {
                            // 타이틀;필수여부;Key값여부;Length;검색조건
                            TagData = ctb.Tag.ToString().Split(';');
                            MsgTag = TagData[0];
                        }
                        else break;   //Tag값 없으면 Skip


                        if (TagData[1] == "1" && ctb.Text == "")  //필수여부 체크
                        {
                            ctb.BackColor = Kind_Linen;
                            ctb.Focus();
                            Rtn = false;
                            MsgGubun = "1";
                        }

                        if (TagData.Length >= 4)
                        {
                            if (TagData[3] != "" && ctb.Text != "")  // Length 체크
                            {
                                if (TagData[3] != ctb.Text.Length.ToString())
                                {
                                    ctb.BackColor = Kind_Linen;
                                    ctb.Focus();
                                    Rtn = false;
                                    MsgGubun = "3";
                                }
                            }
                        }

                        break;
                    #endregion

                    #region C1NumericEdit
                    case "C1NumericEdit":
                        C1.Win.C1Input.C1NumericEdit ntxt = (C1.Win.C1Input.C1NumericEdit)c;
                        if (ntxt.Tag != null && ntxt.Tag.ToString() != "")
                        {
                            // 타이틀;필수여부;Key값여부;Length;검색조건
                            TagData = ntxt.Tag.ToString().Split(';');
                            MsgTag = TagData[0];
                        }
                        else break;   //Tag값 없으면 Skip


                        if (TagData[1] == "1" && ntxt.Text == "")  //필수여부 체크
                        {
                            ntxt.BackColor = Kind_Linen;
                            ntxt.Focus();
                            Rtn = false;
                            MsgGubun = "1";
                        }

                        if (TagData.Length >= 4)
                        {
                            if (TagData[3] != "" && ntxt.Text != "")  // Length 체크
                            {
                                if (TagData[3] != ntxt.Text.Length.ToString())
                                {
                                    ntxt.BackColor = Kind_Linen;
                                    ntxt.Focus();
                                    Rtn = false;
                                    MsgGubun = "3";
                                }
                            }
                        }

                        break;
                    #endregion

                    #region C1DateEdit
                    case "C1DateEdit":
                        C1.Win.C1Input.C1DateEdit dtp = (C1.Win.C1Input.C1DateEdit)c;
                        if (dtp.Tag != null && dtp.Tag.ToString() != "")
                        {
                            // 타이틀;필수여부;Key값여부;Length;검색조건
                            TagData = dtp.Tag.ToString().Split(';');
                            MsgTag = TagData[0];
                        }
                        else break;   //Tag값 없으면 Skip


                        if (TagData[1] == "1" && dtp.Text == "")  //필수여부 체크
                        {
                            dtp.BackColor = Kind_Linen;
                            dtp.Focus();
                            Rtn = false;
                            MsgGubun = "1";
                        }

                        if (TagData.Length >= 4)
                        {
                            if (TagData[3] != "" && dtp.Text != "")  // Length 체크
                            {
                                if (TagData[3] != dtp.Text.Length.ToString())
                                {
                                    dtp.BackColor = Kind_Linen;
                                    dtp.Focus();
                                    Rtn = false;
                                    MsgGubun = "3";
                                }
                            }
                        }

                        break;
                    #endregion

                }

                if (Rtn == false)
                {
                    if (MsgGubun == "1")
                    {
                        MessageBox.Show("필수항목 " + MsgTag + " 를 입력하시기 바랍니다.", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning); //필수항목을 입력하십시오.
                    }
                    else if (MsgGubun == "3")
                    {
                        MessageBox.Show("해당항목 " + MsgTag + " 의 자료길이가 맞지않습니다.", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning); //필수항목을 입력하십시오.
                    }

                    c.Focus();
                    return false;
                }

                Rtn = GroupBox_SaveSearchValidation2(c);
            }

            return Rtn;
        }
        #endregion

        #region GroupBox SearchView Validation 체크 (조회후 컨트롤 처리)
        public static void GroupBox_SearchViewValidation(GroupBox groupBox)
        {
            GroupBox_SearchViewValidation2(groupBox);
        }

        private static void GroupBox_SearchViewValidation2(Control ctls)
        {
            // Key값의 True, false 여부로만 체크하여 읽기 , 쓰기, Enable 처리
            string[] TagData = null;

            foreach (System.Windows.Forms.Control c in ctls.Controls)
            {
                #region 컨트롤 체크
                if (c.GetType().Name == "C1Combo")
                {
                    C1.Win.C1List.C1Combo cbo = (C1.Win.C1List.C1Combo)c;
                    if (cbo.Tag != null && cbo.Tag.ToString() != "")
                    {
                        // 타이틀;필수여부;Key값여부;Length;검색조건
                        TagData = cbo.Tag.ToString().Split(';');

                        if (TagData.Length >= 3)
                        {
                            if (TagData[2] == "true")
                            {	// 읽기전용
                                cbo.EditorBackColor = Kind_Gainsboro;
                                cbo.Enabled = false;
                            }
                            //else
                            //{
                            //    cbo.EditorBackColor = Kind_White;
                            //    cbo.Enabled = true;
                            //}
                        }
                    }
                }
                else if (c.GetType().Name == "C1TextBox")
                {
                    C1.Win.C1Input.C1TextBox ctb = (C1.Win.C1Input.C1TextBox)c;
                    if (ctb.Tag != null && ctb.Tag.ToString() != "")
                    {
                        // 타이틀;필수여부;Key값여부;Length;검색조건
                        TagData = ctb.Tag.ToString().Split(';');

                        if (TagData.Length >= 3)
                        {
                            if (TagData[2] == "true")
                            {	// 읽기전용
                                ctb.BackColor = Kind_Gainsboro;
                                ctb.ReadOnly = true;
                            }
                            else
                            {
                                if (TagData[1] == "2")  // 읽기전용
                                {
                                    ctb.BackColor = Kind_Gainsboro;
                                    ctb.ReadOnly = true;
                                }
                                //else
                                //{
                                //    ctb.BackColor = Kind_White;
                                //    ctb.ReadOnly = false;
                                //}
                            }
                        }
                    }
                }
                else if (c.GetType().Name == "C1NumericEdit")
                {
                    C1.Win.C1Input.C1NumericEdit cne = (C1.Win.C1Input.C1NumericEdit)c;
                    if (cne.Tag != null && cne.Tag.ToString() != "")
                    {
                        // 타이틀;필수여부;Key값여부;Length;검색조건
                        TagData = cne.Tag.ToString().Split(';');

                        if (TagData.Length >= 3)
                        {
                            if (TagData[2] == "true")
                            {	// 읽기전용
                                cne.BackColor = Kind_Gainsboro;
                                cne.ReadOnly = true;
                            }
                            //else
                            //{
                            //    cne.BackColor = Kind_White;
                            //    cne.ReadOnly = false;
                            //}
                        }
                    }
                }
                else if (c.GetType().Name == "C1DateEdit")
                {
                    C1.Win.C1Input.C1DateEdit cde = (C1.Win.C1Input.C1DateEdit)c;
                    if (cde.Tag != null && cde.Tag.ToString() != "")
                    {
                        // 타이틀;필수여부;Key값여부;Length;검색조건
                        TagData = cde.Tag.ToString().Split(';');
                        if (TagData.Length >= 3)
                        {
                            if (TagData[2] == "true")
                            {	// 읽기전용
                                cde.BackColor = Kind_Gainsboro;
                                cde.ReadOnly = true;
                            }
                            //else
                            //{
                            //    cde.BackColor = Kind_White;
                            //    cde.ReadOnly = false;
                            //}
                        }
                    }
                }
                else if (c.GetType().Name == "C1Button")
                {
                    C1.Win.C1Input.C1Button cbt = (C1.Win.C1Input.C1Button)c;
                    if (cbt.Tag != null && cbt.Tag.ToString() != "")
                    {
                        // 타이틀;필수여부;Key값여부;Length;검색조건
                        TagData = cbt.Tag.ToString().Split(';');

                        if (TagData.Length >= 3)
                        {
                            if (TagData[2] == "true")
                            {	// 읽기전용                           
                                cbt.Enabled = false;
                            }
                            else
                            {
                                cbt.Enabled = true;
                            }
                        }
                    }
                }

                #endregion

                GroupBox_SearchViewValidation2(c);
            }

        }
        #endregion

        #region C1DataEdit의 값을 Format맞게 처리
        public static string C1DataEdit_ReadFormat(string sC1Data, string sFormat)
		{
			string RtnData = "";
            if (sC1Data == null || sC1Data == "") return RtnData;
            sC1Data = sC1Data.Replace("-", "");

            if (sFormat == "YYYYMM")
            {
                RtnData = sC1Data.Substring(0, 6);
            }
             if (sFormat == "YYYYMMDD")
            {
                RtnData = sC1Data.Substring(0, 8);
            }
            return RtnData;
		}

        public static string C1DataEdit_WriteFormat(string sC1Data, string sFormat)
		{
			string RtnData = "";
            if (sC1Data == null || sC1Data == "") return RtnData;

            sC1Data = sC1Data.Replace("-", "");

            if (sFormat == "YYYYMM")
            {
                RtnData = sC1Data.Substring(0, 6) ;
            }
            if (sFormat == "YYYY-MM")
            {
                RtnData = sC1Data.Substring(0, 4) + "-" + sC1Data.Substring(4, 2);
            }
            if (sFormat == "YYYYMMDD")
            {
                RtnData = sC1Data.Substring(0, 8);
            }
            if (sFormat == "YYYY-MM-DD")
            {
                RtnData = sC1Data.Substring(0, 4) + "-" + sC1Data.Substring(4, 2) + "-" + sC1Data.Substring(6, 2);
            }
            return RtnData;
		}
		#endregion

        #region String, Numeric,Decimal Data 처리
        public static string String_Data(string StringData, string Literals)
        {
            string RtnData = "";

            if (StringData == "" || StringData == null) 
            {
                RtnData = "";
                return RtnData;
            }
            
            if (Literals == "'") RtnData = StringData.Replace("'", "");

            return RtnData;
        }

        public static decimal Decimal_Data(string DecimalData, string Literals)
        {
            decimal RtnData = 0;

            if (DecimalData == "" || DecimalData == null)
            {
                RtnData = 0;
                return RtnData;
            }

            if (Literals == ",") RtnData = Convert.ToDecimal(DecimalData.Replace(",", ""));
            if (Literals == "%") RtnData = Convert.ToDecimal(DecimalData.Replace("%", ""));

            return RtnData;
        }
        #endregion

        #region GroupBox Global Validation 체크 (업체코드,공장,결산기간 변경시 모든폼에 적용)
        public static void GroupBox_GlobalApply(GroupBox groupBox)
        {
            GroupBox_GlobalApply2(groupBox);
        }
        private static void GroupBox_GlobalApply2(Control ctls)
        {
            /* 적용범위는 업체코드콤보박스, 공장코드콤보박스, 결산기간 C1DataEdit
               gstrMNUF_CODE
               gstrFCTR_CODE
               gstrPERIOD_FROM
               gstrPERIOD_TO
            */
            // Key값의 타이틀로 범위처리
            string[] TagData = null;
            foreach (System.Windows.Forms.Control c in ctls.Controls)
            {
                #region 컨트롤 적용
                if (c.GetType().Name == "C1Combo")
                {
                    C1.Win.C1List.C1Combo cbo = (C1.Win.C1List.C1Combo)c;
                    if (cbo.Tag != null)
                    {
                        // 타이틀;필수여부;Key값여부;Length;검색조건
                        TagData = cbo.Tag.ToString().Split(';');

                        if (TagData[0] == "제출업체")
                        {
                            if (SystemBase.Base.gstrMNUF_CODE == "")
                            {
                                cbo.SelectedIndex = 0;
                            }
                            else
                            {
                                cbo.SelectedValue = SystemBase.Base.gstrMNUF_CODE;
                            }
                        }
                        else if (TagData[0] == "공장")
                        {
                            cbo.SelectedIndex = -1;
                            if (cbo.ListCount > 0)
                            {
                                if (SystemBase.Base.gstrFCTR_CODE == "")
                                {
                                    cbo.SelectedIndex = 0;
                                }
                                else
                                {
                                    cbo.SelectedValue = SystemBase.Base.gstrFCTR_CODE;
                                }
                            }
                        }
                    }
                }
                else if (c.GetType().Name == "C1DateEdit")
                {
                    
                    C1.Win.C1Input.C1DateEdit cde = (C1.Win.C1Input.C1DateEdit)c;
                    if (cde.Tag != null)
                    {
                        // 타이틀;필수여부;Key값여부;Length;검색조건
                        TagData = cde.Tag.ToString().Split(';');

                        if (TagData[0] == "결산기간(F)")
                        {
                            if (SystemBase.Base.gstrPERIOD_FROM == "")
                            {
                                cde.Value = Convert.ToDateTime(SystemBase.Base.GetYearMonth(SystemBase.Base.ServerTime(""), -1, "Y")); 
                            }
                            else
                            {
                                cde.Value = SystemBase.Base.gstrPERIOD_FROM;
                            }
                        }
                        else if (TagData[0] == "결산기간(T)")
                        {
                            if (SystemBase.Base.gstrPERIOD_FROM == "")
                            {
                                cde.Value = Convert.ToDateTime(SystemBase.Base.GetYearMonth(SystemBase.Base.ServerTime(""), -1, "M")); 
                            }
                            else
                            {
                                cde.Value = SystemBase.Base.gstrPERIOD_TO;
                            }
                        }
                    }
                }
 
                #endregion

                GroupBox_GlobalApply2(c);
            }

        }
        #endregion

        #region FPGrid_SaveCheck - 그리드 데이타 필수항목,Length Check
        public static bool FPGrid_SaveCheck(FarPoint.Win.Spread.FpSpread FPGrid, string FormID, string GridNM, bool Msg)
        {
            bool ChkGrid = true;
            int UpCount = 0;
            int col =  0;

            try
            {
                string Query = " usp_BAA004 'S7',@PFORM_ID='" + FormID.ToString() + "' , @PGRID_NAME='" + GridNM + "' ";
                DataTable dt = SystemBase.DbOpen.TranDataTable(Query);

                //필수입력사항 체크
                for (int i = 0; i < FPGrid.Sheets[0].Rows.Count; i++)
                {
                    // Row추가자료, Row수정자료, 삭제자료아닌것
                    if (FPGrid.Sheets[0].RowHeader.Cells[i, 0].Text == "I" || FPGrid.Sheets[0].RowHeader.Cells[i, 0].Text == "U" || FPGrid.Sheets[0].RowHeader.Cells[i, 0].Text == "D")
                    {
                        for (int j = 0; j < FPGrid.Sheets[0].Columns.Count-1; j++)
                        {
                            col = j;

                            //필수항목란 체크---->1:필수, 2:읽기전용/필수, 6:읽기전용/필수/포커스제외
                            //if ((dt.Rows[j][3].ToString() == "1" || dt.Rows[j][3].ToString() == "2" || dt.Rows[j][3].ToString() == "6")
                            if ((FPGrid.Sheets[0].Cells[i, j + 1].BackColor == SystemBase.Validation.Kind_LightCyan) //System.Drawing.Color.FromArgb(242, 252, 254))
                                    && (dt.Rows[j][2].ToString() == ""          // 대문자
                                        || dt.Rows[j][2].ToString() == "GN"     // 일반
                                        || dt.Rows[j][2].ToString() == "DT"     // 날짜(전체)
                                        || dt.Rows[j][2].ToString() == "DY"     // 날짜(년월)
                                        || dt.Rows[j][2].ToString() == "DD"     // 날짜(월콤보)
                                        || dt.Rows[j][2].ToString() == "CB"     // 콤보
                                        || dt.Rows[j][2].ToString().Substring(0,2) ==  "NM" ))  // 숫자  
                            {
                                if ((FPGrid.Sheets[0].Cells[i, j+1].Value == null || FPGrid.Sheets[0].Cells[i, j+1].Text.Length == 0)
                                        && FPGrid.Sheets[0].GetCellType(i, j+1).ToString() != "GeneralCellType"
                                        && FPGrid.Sheets[0].GetCellType(i, j+1).ToString() != "ButtonCellType"
                                        && FPGrid.Sheets[0].RowHeader.Cells[i, 0].Text != "D")
                                {                                   
                                    MessageBox.Show(Convert.ToString(i + 1) + "번째 Row의 [ " + FPGrid.Sheets[0].ColumnHeader.Cells[0, j+1].Text.ToString() + " ] 항목은 필수입력 항목입니다.", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    FPGrid.Focus();
                                    FPGrid.Sheets[0].SetActiveCell(i , j + 1);
                                    ChkGrid = false;
                                    break;
                                }
                            }

                            if (dt.Rows[j][2].ToString() == "DY")  // 마스크에 적용된 년월 체크
                            {
                                if (Convert.ToInt32(FPGrid.Sheets[0].Cells[i, j + 1].Text.Substring(5, 2)) > 12)
                                {
                                    MessageBox.Show(Convert.ToString(i + 1) + "번째 Row의 [ " + FPGrid.Sheets[0].ColumnHeader.Cells[0, j + 1].Text.ToString() + " ] 항목은 날짜형식이 맞지 않습니다.", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    FPGrid.Focus();
                                    FPGrid.Sheets[0].SetActiveCell(i, j + 1);
                                    ChkGrid = false;
                                    break;

                                }
                            }

                            //LENGTH 체크
                            if (dt.Rows[j][4].ToString() != "" )                                             
                            {
                                // Length;
                                if (FPGrid.Sheets[0].Cells[i, j + 1].Text.Length == 0)
                                    break;
                                if (dt.Rows[j][4].ToString().Length != FPGrid.Sheets[0].Cells[i, j + 1].Text.Length)
                                {
                                    MessageBox.Show(Convert.ToString(i + 1) + "번째 Row의 [ " + FPGrid.Sheets[0].ColumnHeader.Cells[0, j + 1].Text.ToString() + " ] 항목은 Length(" + dt.Rows[j][4].ToString().Length.ToString() + ")가 맞지 않습니다.", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    FPGrid.Focus();
                                    FPGrid.Sheets[0].SetActiveCell(i, j + 1);                                    
                                    ChkGrid = false;
                                    break;
                                }
                            }
                        }
                        UpCount++;
                    }
                    if (ChkGrid == false)
                        break;
                }

                if (UpCount == 0 && Msg == true)
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn("SY017"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);//변경되거나 처리 할 자료가 없습니다.
                    ChkGrid = false;
                }
            }
            catch (Exception f)
            {
                ChkGrid = false;
                SystemBase.Loggers.Log("FPGrid_SaveCheck (그리드 필수항목 체크시 에러발생)", col.ToString() + "---" + f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY018"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return ChkGrid;
        }
        #endregion

        #region Control_SearchCheck, Control_SaveCheck 체크 (조회, 저장시 컨트롤 변경유무체크)
        public static void Control_SearchCheck(GroupBox groupBox)
        {
            SystemBase.Base.gstrControl_OrgData = "";
            Control_SearchCheck2(groupBox);
        }
        private static void Control_SearchCheck2(Control ctls)
        {
            foreach (System.Windows.Forms.Control c in ctls.Controls)
            {
                #region 컨트롤 체크
                if (c.GetType().Name == "C1Combo")
                {
                    C1.Win.C1List.C1Combo cbo = (C1.Win.C1List.C1Combo)c;
                    SystemBase.Base.gstrControl_OrgData += cbo.SelectedValue.ToString();
                }
                else if (c.GetType().Name == "C1TextBox")
                {
                    C1.Win.C1Input.C1TextBox ctb = (C1.Win.C1Input.C1TextBox)c;
                    SystemBase.Base.gstrControl_OrgData += ctb.Value.ToString();
                }
                else if (c.GetType().Name == "C1NumericEdit")
                {
                    C1.Win.C1Input.C1NumericEdit cne = (C1.Win.C1Input.C1NumericEdit)c;
                    SystemBase.Base.gstrControl_OrgData += cne.Value.ToString();
                }
                else if (c.GetType().Name == "C1DateEdit")
                {
                    C1.Win.C1Input.C1DateEdit cde = (C1.Win.C1Input.C1DateEdit)c;
                    SystemBase.Base.gstrControl_OrgData += cde.Value.ToString();
                }
                else if (c.GetType().Name == "RadioButton")
                {
                    System.Windows.Forms.RadioButton rdo = (System.Windows.Forms.RadioButton)c;
                    SystemBase.Base.gstrControl_OrgData += (rdo.Checked == true) ? "1" : "0";
                }
                else if (c.GetType().Name == "C1CheckBox")
                {
                    C1.Win.C1Input.C1CheckBox chk = (C1.Win.C1Input.C1CheckBox)c;
                    SystemBase.Base.gstrControl_OrgData += (chk.Checked == true) ? "1" : "0";
                }
                #endregion

                Control_SearchCheck2(c);
            }
        }

        public static void Control_SaveCheck(GroupBox groupBox)
        {
            SystemBase.Base.gstrControl_SaveData = "";
            Control_SaveCheck2(groupBox);
        }
        private static void Control_SaveCheck2(Control ctls)
        {
            foreach (System.Windows.Forms.Control c in ctls.Controls)
            {
                #region 컨트롤 체크
                if (c.GetType().Name == "C1Combo")
                {
                    C1.Win.C1List.C1Combo cbo = (C1.Win.C1List.C1Combo)c;
                    SystemBase.Base.gstrControl_SaveData += cbo.SelectedValue.ToString();
                }
                else if (c.GetType().Name == "C1TextBox")
                {
                    C1.Win.C1Input.C1TextBox ctb = (C1.Win.C1Input.C1TextBox)c;
                    SystemBase.Base.gstrControl_SaveData += ctb.Value.ToString();
                }
                else if (c.GetType().Name == "C1NumericEdit")
                {
                    C1.Win.C1Input.C1NumericEdit cne = (C1.Win.C1Input.C1NumericEdit)c;
                    SystemBase.Base.gstrControl_SaveData += cne.Value.ToString();
                }
                else if (c.GetType().Name == "C1DateEdit")
                {
                    C1.Win.C1Input.C1DateEdit cde = (C1.Win.C1Input.C1DateEdit)c;
                    SystemBase.Base.gstrControl_SaveData += cde.Value.ToString();
                }
                else if (c.GetType().Name == "RadioButton")
                {
                    System.Windows.Forms.RadioButton rdo = (System.Windows.Forms.RadioButton)c;
                    SystemBase.Base.gstrControl_SaveData += (rdo.Checked == true) ? "1" : "0";
                }
                else if (c.GetType().Name == "C1CheckBox")
                {
                    C1.Win.C1Input.C1CheckBox chk = (C1.Win.C1Input.C1CheckBox)c;
                    SystemBase.Base.gstrControl_SaveData += (chk.Checked == true) ? "1" : "0";
                }
                #endregion

                Control_SaveCheck2(c);
            }
        }

        #endregion

        #region  Master 그리드 방향키 이동 및 클릭시 Detail 조회 변경유무 체크
        public static bool FPGrid_SelectionChanged(FarPoint.Win.Spread.FpSpread FPGrid, bool Msg)
        {
            bool ChkSelectionChange = true;
            int ChangeCount = 0;

            try
            {
                //변경입력사항 체크
                for (int i = 0; i < FPGrid.Sheets[0].Rows.Count; i++)
                {

                    if (FPGrid.Sheets[0].RowHeader.Cells[i, 0].Text == "I" || FPGrid.Sheets[0].RowHeader.Cells[i, 0].Text == "U" || FPGrid.Sheets[0].RowHeader.Cells[i, 0].Text == "D")
                    {
                        ChangeCount++;
                    }

                    if (ChangeCount != 0)
                    {
                        if (Msg == true)
                        {
                            MessageBox.Show(SystemBase.Base.MessageRtn("SY068"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);//변경되거나 처리 할 자료가 없습니다.
                        }
                        ChkSelectionChange = false;
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log("FPGrid_SelectionChanged (그리드 SelectionChanged 체크시 에러발생)", f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY018"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


            return ChkSelectionChange;
        }
        #endregion        

        #region 그룹박스 컨트롤 전체 락
        /// <summary>
        /// 그룹박스 전체 컨트롤에 대한 Locking 여부
        /// <param name="groupBox">그룹박스</param>
        /// <param name="LockingFlag">Locking여부</param>
        /// </summary>
        public static void GroupBoxControlsLock(System.Windows.Forms.GroupBox groupBox, bool LockingFlag)
        {
            try
            {
                if (LockingFlag == true)
                {
                    LockTrue(groupBox);
                }
                else
                {
                    GroupBox_Setting2(groupBox);
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log("GroupBoxControlsLock", f.ToString());
            }
        }

        private static void LockTrue(Control ctls)
        {
            foreach (System.Windows.Forms.Control c in ctls.Controls)
            {
                #region 컨트롤 체크
                if (c.GetType().Name == "C1Combo")
                {
                    C1.Win.C1List.C1Combo cbo = (C1.Win.C1List.C1Combo)c;
                    cbo.EditorBackColor = Kind_Gainsboro;
                    cbo.Enabled = false;
                }
                else if (c.GetType().Name == "C1TextBox")
                {
                    C1.Win.C1Input.C1TextBox ctb = (C1.Win.C1Input.C1TextBox)c;
                    ctb.BackColor = Kind_Gainsboro;
                    ctb.ReadOnly = true;
                }
                else if (c.GetType().Name == "C1NumericEdit")
                {
                    C1.Win.C1Input.C1NumericEdit cne = (C1.Win.C1Input.C1NumericEdit)c;
                    cne.BackColor = Kind_Gainsboro;
                    cne.ReadOnly = true;
                }
                else if (c.GetType().Name == "C1DateEdit")
                {
                    C1.Win.C1Input.C1DateEdit cde = (C1.Win.C1Input.C1DateEdit)c;
                    cde.BackColor = Kind_Gainsboro;
                    cde.ReadOnly = true;
                }
                else if (c.GetType().Name == "C1Button")
                {
                    C1.Win.C1Input.C1Button cbt = (C1.Win.C1Input.C1Button)c;
                    cbt.Enabled = false;
                }
                else if (c.GetType().Name == "RadioButton")
                {
                    System.Windows.Forms.RadioButton rdo = (System.Windows.Forms.RadioButton)c;
                    rdo.Enabled = false;
                }
                else if (c.GetType().Name == "C1CheckBox")
                {
                    C1.Win.C1Input.C1CheckBox chk = (C1.Win.C1Input.C1CheckBox)c;
                    chk.Enabled = false;
                }
                #endregion

                LockTrue(c);
            }
        }
        #endregion

        #region 조회, 저장시 컨트롤 변경유무체크
        /// <summary>
        /// 조회, 저장시 컨트롤 변경 유무 체크
        /// <param name="groupBox">groupBox</param>
        /// <param name="Data">return 받을 데이터</param>
        /// <example>example
        /// <code> string strSearchData = "", strSaveData = "";
        /// SystemBase.Validation.Control_Check(groupBox1, ref strSearchData); //조회 후 컨트롤 값 저장
        /// SystemBase.Validation.Control_Check(groupBox1, ref strSaveData); //저장 전 컨트롤 값 저장
        /// if(strSearchData == strSaveData) //저장시 변경된 컨트롤 값이 없다면 
        /// {
        ///     return;
        /// }
        /// </code>
        /// </example>
        /// </summary>
        public static void Control_Check(GroupBox[] groupBox, ref string Data)
        {
            try
            {
                if (groupBox.Length > 0)
                {
                    for (int i = 0; i < groupBox.Length; i++)
                    {
                        string GetData = "";

                        Control_Check2(groupBox[i], ref GetData);

                        Data += GetData;
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log("Control_Check", f.ToString());
            }
        }

        private static void Control_Check2(Control ctls, ref string Data)
        {
            foreach (System.Windows.Forms.Control c in ctls.Controls)
            {
                #region 컨트롤 체크
                if (c.GetType().Name == "C1Combo")
                {
                    C1.Win.C1List.C1Combo cbo = (C1.Win.C1List.C1Combo)c;
                    if (cbo.SelectedText.ToString() != null)
                    {
                        Data += cbo.SelectedText.ToString();
                    }
                    
                }
                else if (c.GetType().Name == "C1TextBox")
                {
                    C1.Win.C1Input.C1TextBox ctb = (C1.Win.C1Input.C1TextBox)c;
                    if (ctb.Value.ToString() != null)
                    {
                        Data += ctb.Value.ToString();
                    }
                }
                else if (c.GetType().Name == "C1NumericEdit")
                {
                    C1.Win.C1Input.C1NumericEdit cne = (C1.Win.C1Input.C1NumericEdit)c;
                    if (cne.Value.ToString() != null)
                    {
                        Data += cne.Value.ToString();
                    }
                }
                else if (c.GetType().Name == "C1DateEdit")
                {
                    C1.Win.C1Input.C1DateEdit cde = (C1.Win.C1Input.C1DateEdit)c;
                    if (cde.Value.ToString() != null)
                    {
                        Data += cde.Value.ToString();
                    }
                }
                else if (c.GetType().Name == "RadioButton")
                {
                    System.Windows.Forms.RadioButton rdo = (System.Windows.Forms.RadioButton)c;
                    Data += (rdo.Checked == true) ? "1" : "0";
                }
                else if (c.GetType().Name == "C1CheckBox")
                {
                    C1.Win.C1Input.C1CheckBox chk = (C1.Win.C1Input.C1CheckBox)c;
                    Data += (chk.Checked == true) ? "1" : "0";
                }
                #endregion

                Control_Check2(c, ref Data);
            }
        }
       #endregion

		#region FPGrid_SaveCheck - 그리드 데이타 필수항목,Length Check => 동적으로 칼럼이 변하는 경우 사용
		public static bool FPGrid_SaveCheck_NEW(FarPoint.Win.Spread.FpSpread FPGrid, string FormID, string GridNM, bool Msg)
		{
			bool ChkGrid = true;
			int UpCount = 0;
			int col = 0;

			try
			{
				string Query = " usp_BAA004 'S7',@PFORM_ID='" + FormID.ToString() + "' , @PGRID_NAME='" + GridNM + "' ";
				DataTable dt = SystemBase.DbOpen.TranDataTable(Query);

				//필수입력사항 체크
				for (int i = 0; i < FPGrid.Sheets[0].Rows.Count; i++)
				{
					// Row추가자료, Row수정자료, 삭제자료아닌것
					if (FPGrid.Sheets[0].RowHeader.Cells[i, 0].Text == "I" || FPGrid.Sheets[0].RowHeader.Cells[i, 0].Text == "U" || FPGrid.Sheets[0].RowHeader.Cells[i, 0].Text == "D")
					{
						for (int j = 0; j < dt.Rows.Count - 1; j++)
						{
							col = j;

							//필수항목란 체크---->1:필수, 2:읽기전용/필수, 6:읽기전용/필수/포커스제외
							//if ((dt.Rows[j][3].ToString() == "1" || dt.Rows[j][3].ToString() == "2" || dt.Rows[j][3].ToString() == "6")
							if ((FPGrid.Sheets[0].Cells[i, j + 1].BackColor == SystemBase.Validation.Kind_LightCyan) //System.Drawing.Color.FromArgb(242, 252, 254))
									&& (dt.Rows[j][2].ToString() == ""          // 대문자
										|| dt.Rows[j][2].ToString() == "GN"     // 일반
										|| dt.Rows[j][2].ToString() == "DT"     // 날짜(전체)
										|| dt.Rows[j][2].ToString() == "DY"     // 날짜(년월)
										|| dt.Rows[j][2].ToString() == "DD"     // 날짜(월콤보)
										|| dt.Rows[j][2].ToString() == "CB"     // 콤보
										|| dt.Rows[j][2].ToString().Substring(0, 2) == "NM"))  // 숫자  
							{
								if ((FPGrid.Sheets[0].Cells[i, j + 1].Value == null || FPGrid.Sheets[0].Cells[i, j + 1].Text.Length == 0)
										&& FPGrid.Sheets[0].GetCellType(i, j + 1).ToString() != "GeneralCellType"
										&& FPGrid.Sheets[0].GetCellType(i, j + 1).ToString() != "ButtonCellType"
										&& FPGrid.Sheets[0].RowHeader.Cells[i, 0].Text != "D")
								{
									MessageBox.Show(Convert.ToString(i + 1) + "번째 Row의 [ " + FPGrid.Sheets[0].ColumnHeader.Cells[0, j + 1].Text.ToString() + " ] 항목은 필수입력 항목입니다.", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
									FPGrid.Focus();
									FPGrid.Sheets[0].SetActiveCell(i, j + 1);
									ChkGrid = false;
									break;
								}
							}

							if (dt.Rows[j][2].ToString() == "DY")  // 마스크에 적용된 년월 체크
							{
								if (Convert.ToInt32(FPGrid.Sheets[0].Cells[i, j + 1].Text.Substring(5, 2)) > 12)
								{
									MessageBox.Show(Convert.ToString(i + 1) + "번째 Row의 [ " + FPGrid.Sheets[0].ColumnHeader.Cells[0, j + 1].Text.ToString() + " ] 항목은 날짜형식이 맞지 않습니다.", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
									FPGrid.Focus();
									FPGrid.Sheets[0].SetActiveCell(i, j + 1);
									ChkGrid = false;
									break;

								}
							}

							//LENGTH 체크
							if (dt.Rows[j][4].ToString() != "")
							{
								// Length;
								if (dt.Rows[j][4].ToString().Length != FPGrid.Sheets[0].Cells[i, j + 1].Text.Length)
								{
									MessageBox.Show(Convert.ToString(i + 1) + "번째 Row의 [ " + FPGrid.Sheets[0].ColumnHeader.Cells[0, j + 1].Text.ToString() + " ] 항목은 Length(" + dt.Rows[j][4].ToString().Length.ToString() + ")가 맞지 않습니다.", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
									FPGrid.Focus();
									FPGrid.Sheets[0].SetActiveCell(i, j + 1);
									ChkGrid = false;
									break;
								}
							}
						}
						UpCount++;
					}
					if (ChkGrid == false)
						break;
				}

				if (UpCount == 0 && Msg == true)
				{
					MessageBox.Show(SystemBase.Base.MessageRtn("SY017"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);//변경되거나 처리 할 자료가 없습니다.
					ChkGrid = false;
				}
			}
			catch (Exception f)
			{
				ChkGrid = false;
				SystemBase.Loggers.Log("FPGrid_SaveCheck (그리드 필수항목 체크시 에러발생)", col.ToString() + "---" + f.ToString());
				MessageBox.Show(SystemBase.Base.MessageRtn("SY018"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}

			return ChkGrid;
		}
		#endregion

    }
}
