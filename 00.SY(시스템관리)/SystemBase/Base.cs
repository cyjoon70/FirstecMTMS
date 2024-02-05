using System;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using PlexityHide.GTP;
using System.Globalization;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace SystemBase
{
	/// <summary>
	/// Base에 대한 요약 설명입니다.
	/// </summary>
	public class Base
	{
        public static string gstrFromLoading    = "N";  //From Loading여부
        public static string gstrMNUF_CODE      = "";	//업체코드
        public static string gstrFCTR_CODE      = "";	//공장
        public static string gstrPERIOD_FROM    = "";	//결산기간(From)
        public static string gstrPERIOD_TO      = "";	//결산기간(To)

		public static string gstrDbConn			= "";	//DB 연결정보
		public static string gstrDbName		    = "";	//db Name
		public static string gstrServerNM		= "";	//접속서버Ip
		public static string gstrServerId       = "";   //접속서버 id
		public static string gstrServerPwd      = "";   //접속서버 pwd
		public static string gstrUserID			= "";	//사용자 ID 저장
		public static string gstrUserPWD		= "";	//사용자 비밀번호
		public static string gstrUserName		= "";	//사용자 이름 저장
        public static string gstrExcelConn      = "";	//EXCEL 연결정보

		public static string gstrMacAddress		= "";	//사용자 맥어드레스
        public static string gstrUserIp = "";	//사용자 접속 IP

		//public static string gstrLodeFormName	= "";	//로드폼명
		public static int gstrFormClosingMsg	= 0;	//화면종료시 그리드 변경데이타 확인 메세지 표시여부 0 or 1

        public static string gstrLangCd = "KOR";//국가코드

        public static  System.Drawing.Color gColor1 = Color.PaleTurquoise;	//총합계
		public static  System.Drawing.Color gColor2 = Color.PaleGreen;		//소계1
		public static  System.Drawing.Color gColor3 = Color.PaleVioletRed;	//소계2
		public static  System.Drawing.Color gColor4 = Color.Moccasin;		//소계3
		public static  System.Drawing.Color gColor5 = Color.PaleGoldenrod;	//소계4

        public static System.Drawing.Color Color_Update = System.Drawing.Color.FromArgb(115, 181, 223);	  // Update U 플래그
        public static System.Drawing.Color Color_Insert = System.Drawing.Color.FromArgb(224, 245, 89);    // Insert I 플래그
        public static System.Drawing.Color Color_Delete = System.Drawing.Color.FromArgb(228, 120, 58);     // Delete D 플래그
        public static System.Drawing.Color Color_Org = System.Drawing.Color.FromArgb(242, 244, 246);      // 원상태 

        public static string gstrControl_OrgData = "";	//컨트롤 변경이전 자료
        public static string gstrControl_SaveData = "";	//컨트롤 변경이후 자료

		public static string gstrCOMCD			= "1";  //법인코드
		public static string gstrCOMNM			= "";  //법인명
		public static string gstrBIZCD			= "";  //사업장코드
		public static string gstrBIZNM			= "";  //사업장명

		public static string gstrPLANT_CD		= "";  //공장명

		public static string gstrREORG_ID		= "";  //부서개편ID
		public static string gstrDEPT			= "";  //부서코드
		public static string gstrDEPTNM			= "";  //부서명
		public static string gstrMSG_CODE			= "";  //메세지코드

		public static string gstrDOC_NO		= "";		//문서번호
		public static string gstrWO_NO		= "";		//workorder no

        // scm관련 추가
        public static string gstrTRADE_TYPE = ""; //거래유형
        public static string gstrCUST_TYPE = ""; //거래처구분

		public static string RodeFormName		= "";	//로딩된 폼 이름 저장
		public static string ProgramWhere		= "";	//현재 프로그램 위치
		public static string RodeFormID			= "";	//로딩된 폼 ID 저장
		public static string RodeFormText		= "";	//로딩된 폼 한글명
		public static string Query1				= "";	//Query문 저장
		public static string Query2				= "";
		public static int InputBoxHeight;			//입력창 높이 조절
		public static int GridKind;					//그리드 종류
		public static int lblHeadColorR;			//
		public static int lblHeadColorG;			//
		public static int lblHeadColorB;			//
		public static int lblLineColorR;			//
		public static int lblLineColorG;			//
		public static int lblLineColorB;			//
		public static int lblSelectColorR;			//
		public static int lblSelectColorG;			//
		public static int lblSelectColorB;			//
		public static int lblSelectFontR;			//
		public static int lblSelectFontG;			//
		public static int lblSelectFontB;			//
		public static int lbl2_1ColorR;			//
		public static int lbl2_1ColorG;			//
		public static int lbl2_1ColorB;			//
		public static int lblColColorR;			//
		public static int lblColColorG;			//
		public static int lblColColorB;			//
		//public static string [] Grid1 = new string[]{"",""};//그리드 타이틀명 배열로 저장
		//public static string [] Grid2;

		public static Form BaseForm = null; //메인폼

		#region 그룹박스 위에 있는 컨트롤 Reset
		public static void GroupBoxReset(GroupBox groupBox1)
		{// 사용법 : SystemBase.Base.GroupBoxReset(groupBox1)
			GroupBoxReset2(groupBox1);
		}
		private static void GroupBoxReset2(Control ctls)
		{// 사용법 : SystemBase.Base.GroupBoxReset(groupBox1)
			foreach(System.Windows.Forms.Control c in ctls.Controls)                           
			{   

				switch(c.GetType().Name)
				{
					case "ComboBox":
						ComboBox cb = (ComboBox)c;
						cb.DropDownStyle = ComboBoxStyle.DropDownList;

						if(cb.Items.Count > 0)
							cb.SelectedIndex = 0;
						break;

					case "TextBox":
						c.Text = "";  
						break;
						
					case "NumericUpDown":
						NumericUpDown Nud = (NumericUpDown)c;
						Nud.Value = 1;
						break;

					case "C1TextBox":
						C1.Win.C1Input.C1TextBox ctb = (C1.Win.C1Input.C1TextBox)c;
						ctb.Value = null;
						break;

					case "C1NumericEdit":
						C1.Win.C1Input.C1NumericEdit cne = (C1.Win.C1Input.C1NumericEdit)c;
						cne.Value = null;
						break;

					case "C1DateEdit":
						C1.Win.C1Input.C1DateEdit cde = (C1.Win.C1Input.C1DateEdit)c;
						cde.Value = null;
						break;
					case "RadioButton":
						RadioButton rd = (RadioButton)c;
						if(rd.Checked == true)
							rd.Checked = false;
						break;
					case "CheckBox":
						CheckBox ck = (CheckBox)c;
						if(ck.Checked == true)
							ck.Checked = false;
						break;

					case "DateTimePicker":
						DateTimePicker dt = (DateTimePicker)c;
						dt.Value = Convert.ToDateTime(SystemBase.Base.ServerTime(""));
						break;

				}

				//User Contorl 일경우엔 재귀호출
				//if(Convert.ToString(c.Tag) == "UC")
					GroupBoxReset2(c);

			}
		}
		#endregion

		#region c1DockingTab 위에 있는 컨트롤 Reset
		public static void c1DockingTabReset(C1.Win.C1Command.C1DockingTabPage c1DockingTabPage1)
		{// 사용법 : NamYoung.Common.Etc.GroupBoxReset(groupBox1)
			c1DockingTabReset2(c1DockingTabPage1);
		}

		private static void c1DockingTabReset2(Control ctls)
		{
			foreach(System.Windows.Forms.Control c in ctls.Controls)                           
			{   

				switch(c.GetType().Name)
				{
					case "ComboBox":
						ComboBox cb = (ComboBox)c;
						cb.DropDownStyle = ComboBoxStyle.DropDownList;

						if(cb.Items.Count > 0)
							cb.SelectedIndex = 0;
						break;

					case "TextBox":
						c.Text = "";  
						break;
						
					case "NumericUpDown":
						NumericUpDown Nud = (NumericUpDown)c;
						Nud.Value = 1;
						break;

					case "RadioButton":
						RadioButton rd = (RadioButton)c;
						if(rd.Checked == true)
							rd.Checked = false;
						break;

					case "CheckBox":
						CheckBox ck = (CheckBox)c;
						if(ck.Checked == true)
							ck.Checked = false;
						break;

					case "C1TextBox":
						C1.Win.C1Input.C1TextBox ctb = (C1.Win.C1Input.C1TextBox)c;
						ctb.Value = null;
						break;

					case "C1NumericEdit":
						C1.Win.C1Input.C1NumericEdit cne = (C1.Win.C1Input.C1NumericEdit)c;
						cne.Value = null;
						break;

					case "C1DateEdit":
						C1.Win.C1Input.C1DateEdit cde = (C1.Win.C1Input.C1DateEdit)c;
						cde.Value = null;
						break;

					case "DateTimePicker":
						DateTimePicker dt = (DateTimePicker)c;
						dt.Value = Convert.ToDateTime(SystemBase.Base.ServerTime(""));
						break;
				}

				//User Contorl 일경우엔 재귀호출
				//if(Convert.ToString(c.Tag) == "UC")
					c1DockingTabReset2(c);
			}
		}
		#endregion

		#region TabPage 위에 있는 컨트롤 Reset
		public static void TabPageReset(TabPage tabPage1)
		{// 사용법 : NamYoung.Common.Etc.GroupBoxReset(groupBox1)
			TabPageReset2(tabPage1);
		}

		private static void TabPageReset2(Control ctls)
		{
			foreach(System.Windows.Forms.Control c in ctls.Controls)                           
			{   
				switch(c.GetType().Name)
				{
					case "ComboBox":
						ComboBox cb = (ComboBox)c;
						cb.DropDownStyle = ComboBoxStyle.DropDownList;

						if(cb.Items.Count > 0)
							cb.SelectedIndex = 0;
						break;

					case "TextBox":
						c.Text = "";  
						break;
						
					case "NumericUpDown":
						NumericUpDown Nud = (NumericUpDown)c;
						Nud.Value = 1;
						break;

					case "RadioButton":
						RadioButton rd = (RadioButton)c;
						if(rd.Checked == true)
							rd.Checked = false;
						break;

					case "CheckBox":
						CheckBox ck = (CheckBox)c;
						if(ck.Checked == true)
							ck.Checked = false;
						break;

					case "C1TextBox":
						C1.Win.C1Input.C1TextBox ctb = (C1.Win.C1Input.C1TextBox)c;
						ctb.Value = null;
						break;

					case "C1NumericEdit":
						C1.Win.C1Input.C1NumericEdit cne = (C1.Win.C1Input.C1NumericEdit)c;
						cne.Value = null;
						break;

					case "C1DateEdit":
						C1.Win.C1Input.C1DateEdit cde = (C1.Win.C1Input.C1DateEdit)c;
						cde.Value = null;
						break;

					case "DateTimePicker":
						DateTimePicker dt = (DateTimePicker)c;
						dt.Value = Convert.ToDateTime(SystemBase.Base.ServerTime(""));
						break;
				}

				//User Contorl 일경우엔 재귀호출
				//if(Convert.ToString(c.Tag) == "UC")
					TabPageReset2(c);

			}
		}
		#endregion

		#region Panel 위에 있는 컨트롤 Reset
		public static void PanelReset(Panel panel1)
		{// 사용법 : NamYoung.Common.Etc.PanelReset(panel1)
			PanelReset2(panel1);
		}

		private static void PanelReset2(Control ctls)
		{// 사용법 : NamYoung.Common.Etc.PanelReset(panel1)
			foreach(System.Windows.Forms.Control c in ctls.Controls)                           
			{   
				switch(c.GetType().Name)
				{
					case "ComboBox":
						ComboBox cb = (ComboBox)c;
						cb.DropDownStyle = ComboBoxStyle.DropDownList;

						if(cb.Items.Count > 0)
							cb.SelectedIndex = 0;
						break;

					case "TextBox":
						c.Text = "";  
						break;
						
					case "NumericUpDown":
						NumericUpDown Nud = (NumericUpDown)c;
						Nud.Value = 1;
						break;

					case "RadioButton":
						RadioButton rd = (RadioButton)c;
						if(rd.Checked == true)
							rd.Checked = false;
						break;

					case "CheckBox":
						CheckBox ck = (CheckBox)c;
						if(ck.Checked == true)
							ck.Checked = false;
						break;

					case "C1TextBox":
						C1.Win.C1Input.C1TextBox ctb = (C1.Win.C1Input.C1TextBox)c;
						ctb.Value = null;
						break;

					case "C1NumericEdit":
						C1.Win.C1Input.C1NumericEdit cne = (C1.Win.C1Input.C1NumericEdit)c;
						cne.Value = null;
						break;

					case "C1DateEdit":
						C1.Win.C1Input.C1DateEdit cde = (C1.Win.C1Input.C1DateEdit)c;
						cde.Value = null;
						break;

					case "DateTimePicker":
						DateTimePicker dt = (DateTimePicker)c;
						dt.Value = Convert.ToDateTime(SystemBase.Base.ServerTime(""));
						break;
				}

				//User Contorl 일경우엔 재귀호출
				//if(Convert.ToString(c.Tag) == "UC")
					PanelReset2(c);

			}
		}
		#endregion

		#region Panel 위에 있는 Radio Checked 값 return
		public static string PanelRdoValue(Panel panel1)
		{// 사용법 : string CheckedText = NamYoung.Common.Etc.PanelRdoValue(panel1)
			string Rtn = "";
			foreach(System.Windows.Forms.Control c in panel1.Controls)                           
			{   
				switch(c.GetType().Name)
				{
					case "RadioButton":
						RadioButton cb = (RadioButton)c;
						if(cb.Checked == true)
							Rtn = cb.Text;
						break;
				}
			}
			return Rtn;
		}

		#endregion

		#region Panel 위 Radio Checked
		public static void PanelRdoCheck(Panel panel1, string Value)
		{// 사용법 : NamYoung.Common.Etc.PanelRdoCheck(panel9,"aaa");
			foreach(System.Windows.Forms.Control c in panel1.Controls)                           
			{   
				switch(c.GetType().Name)
				{
					case "RadioButton":
						RadioButton cb = (RadioButton)c;
						if(cb.Text == Value)
							cb.Checked = true;
						break;
				}
			}
		}
		#endregion

		#region GroupBox 위 RadioButton Checked
		public static void GroupBoxRdoCheck(GroupBox groupBox1, string Value)
		{// 사용법 : SystemBase.Base.GroupBoxRdoCheck(groupBox1,"aaa");
			foreach(System.Windows.Forms.Control c in groupBox1.Controls)                           
			{   
				switch(c.GetType().Name)
				{
					case "RadioButton":
						RadioButton cb = (RadioButton)c;
						if(cb.Text == Value)
							cb.Checked = true;
						break;
				}
			}
		}
		#endregion

		#region GroupBox 위 RadioButton TAG 값으로 Checked
		public static void GroupBoxRdoCheck(string TAG, GroupBox groupBox1)
		{// 사용법 : SystemBase.Base.GroupBoxRdoCheck("aaa",groupBox1);
			foreach(System.Windows.Forms.Control c in groupBox1.Controls)                           
			{   
				switch(c.GetType().Name)
				{
					case "RadioButton":
						RadioButton cb = (RadioButton)c;
						if(cb.Tag.ToString() == TAG)
							cb.Checked = true;
						break;
				}
			}
		}
		#endregion

		#region Panel 위 CheckBox Text return
		public static string PanelChkValue(Panel panel1)
		{// 사용법 : string CheckedText = NamYoung.Common.Etc.PanelChkValue(panel1)
			string Rtn = "";
			foreach(System.Windows.Forms.Control c in panel1.Controls)                           
			{   
				switch(c.GetType().Name)
				{
					case "CheckBox":
						CheckBox cb = (CheckBox)c;
						if(cb.Checked == true)
							if(Rtn.Length == 0)
								Rtn = cb.Text;
							else
								Rtn = Rtn +"|"+ cb.Text;
						break;
				}
			}
			return Rtn;
		}
		#endregion

		#region groupBox 위에 있는 Radio Checked 값 return
		public static string GroupBoxRdoValue(GroupBox groupBox1)
		{// 사용법 : string CheckedText = NamYoung.Common.Etc.GroupBoxRdoValue(panel1)
			string Rtn = "";
			foreach(System.Windows.Forms.Control c in groupBox1.Controls)                           
			{   
				switch(c.GetType().Name)
				{
					case "RadioButton":
						RadioButton cb = (RadioButton)c;
						if(cb.Checked == true)
							Rtn = cb.Text;
						break;
				}
			}
			return Rtn;
		}
		#endregion

		#region groupBox 위에 있는 Radio Checked 값 return
		public static string GroupBoxRdoValue(GroupBox groupBox1, bool TAG)
		{// 사용법 : string CheckedText = NamYoung.Common.Etc.GroupBoxRdoValue(groupBox1, true)
			string Rtn = "";
			if(TAG == true)
			{
				foreach(System.Windows.Forms.Control c in groupBox1.Controls)                           
				{   
					switch(c.GetType().Name)
					{
						case "RadioButton":
							RadioButton cb = (RadioButton)c;
							if(cb.Checked == true)
								Rtn = cb.Tag.ToString();
							break;
					}
				}
			}
			else
			{
				foreach(System.Windows.Forms.Control c in groupBox1.Controls)                           
				{   
					switch(c.GetType().Name)
					{
						case "RadioButton":
							RadioButton cb = (RadioButton)c;
							if(cb.Checked == true)
								Rtn = cb.Text;
							break;
					}
				}
			}
			return Rtn;
		}
		#endregion

		#region GroupBox 위에 있는 CheckBox Checked 값 return
		public static string GroupBoxChkValue(GroupBox groupBox1)
		{// 사용법 : string CheckedText = NamYoung.Common.Etc.PanelChkValue(panel1)
			string Rtn = "";
			foreach(System.Windows.Forms.Control c in groupBox1.Controls)                           
			{   
				switch(c.GetType().Name)
				{
					case "CheckBox":
						CheckBox cb = (CheckBox)c;
						if(cb.Checked == true)
							if(Rtn.Length == 0)
								Rtn = cb.Text;
							else
								Rtn = Rtn +"|"+ cb.Text;
						break;
				}
			}
			return Rtn;
		}
		#endregion

		#region 사업자 등록번호
		public static string BusinessNo(string Txt)
		{// 콤마 넣기	NamYoung.Common.Etc.BusinessNo("2120852121");
			string Rtn= Txt;
			if(Txt.Length == 10)
			{
				Rtn = Rtn.Replace("-","");
				string temp = Rtn.ToString().Substring(0,3) +"-"+ Rtn.ToString().Substring(3,2) +"-"+ Rtn.ToString().Substring(5,5);
				Rtn = temp;
			}
			else
				Rtn = Txt;

			return Rtn;
		}
		#endregion

		#region 주민등록번호
		public static string JuminNo(string Txt)
		{// 콤마 넣기	NamYoung.Common.Etc.BusinessNo("2120852121");
			string Rtn= Txt;
			if(Txt.Length == 13)
			{
				Rtn = Rtn.Replace("-","");
				string temp = Rtn.ToString().Substring(0,6) +"-"+ Rtn.ToString().Substring(6,7);
				Rtn = temp;
			}
			else
				Rtn = Txt;

			return Rtn;
		}
		#endregion

		#region Panel 위 CheckBox Checked
		public static void PanelChkChecked(Panel panel1, string Value)
		{// 사용법 : NamYoung.Common.Etc.PanelChkChecked(panel11,"aaa-bbb-ccc");

			Value = Value.ToString().Replace("|","-");

			Regex r = new Regex("-");
			string[] s = r.Split(Value);

			foreach(System.Windows.Forms.Control c in panel1.Controls)                           
			{   
				if(c.GetType().Name == "CheckBox")
				{
					CheckBox cb = (CheckBox)c;
					for(int j = 0; j < s.Length; j++)
					{
						if(cb.Text == s[j])
							cb.Checked = true;
					}
				}
			}
		}
		#endregion

		#region GroupBox Exception Check
		public static bool GroupBoxException(GroupBox groupBox1)
		{// 사용법 : SystemBase.Base.GroupBoxException(groupBox1);
			return GroupBoxException2(groupBox1);
		}

		private static bool GroupBoxException2(Control ctls)
		{// 사용법 : SystemBase.Base.GroupBoxException(groupBox1);
			bool Rtn = true;
			foreach(System.Windows.Forms.Control c in ctls.Controls)                           
			{   
				switch(c.GetType().Name)
				{
					case "ComboBox":
						ComboBox cb = (ComboBox)c;

						if(c.Tag != null && c.Text == "" && c.Tag.ToString() == "T")
						{
							c.BackColor = Color.Pink;
							c.Focus();
							Rtn = false;
						}
						else if(c.Tag != null && c.Text != "" && c.Tag.ToString() == "T")
						{
							c.BackColor = Color.Linen;
						}
						break;

					case "TextBox":
						if(c.Tag != null && c.Text == "" && c.Tag.ToString() == "T")
						{
							c.BackColor = Color.Pink;
							c.Focus();
							Rtn = false;
						}
						else if(c.Tag != null && c.Text != "" && c.Tag.ToString() == "T")
						{
							c.BackColor = Color.Linen;
						}
						break;
				}

				if(Rtn == false)
				{
					c.Focus();
					MessageBox.Show(SystemBase.Base.MessageRtn("SY007"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning); //필수항목을 입력하십시오.
					return false;
				}

				//User Contorl 일경우엔 재귀호출
				//if(Convert.ToString(c.Tag) == "UC")
					Rtn = GroupBoxException2(c);
				
			}
			return Rtn;
		}
		#endregion

		#region GroupBox Exception Check 신규 Exception 2007-05-04

		public static bool GroupBoxExceptions(GroupBox groupBox1)
		{// 사용법 : SystemBase.Base.GroupBoxExceptions(groupBox1);
			return GroupBoxExceptions2(groupBox1);
		}

		public static bool GroupBoxExceptions(Panel groupBox1)
		{// 사용법 : SystemBase.Base.GroupBoxExceptions(groupBox1);
			return GroupBoxExceptions2(groupBox1);
		}

		private static bool GroupBoxExceptions2(Control ctls)
		{// 사용법 : SystemBase.Base.GroupBoxExceptions(groupBox1);
			bool Rtn = true;
			foreach(System.Windows.Forms.Control c in ctls.Controls)                           
			{   
				switch(c.GetType().Name)
				{
					case "ComboBox":
						ComboBox cb = (ComboBox)c;

						if(c.Tag != null && c.Text == "" && c.Tag.ToString() == "1")
						{
							c.BackColor = Color.Linen;
							c.Focus();
							Rtn = false;
						}
						//						else if(c.Tag != null && c.Text != "" && c.Tag.ToString() == "1")
						//						{
						//							c.BackColor = Color.LightCyan;
						//						}
						if(c.Tag != null && c.Text == "" && c.Tag.ToString() == "3")
						{
							c.BackColor = Color.Linen;
							c.Focus();
							Rtn = false;
						}
						else if(c.Tag != null && c.Text != "" && c.Tag.ToString() == "3")
						{
							c.BackColor = Color.LightCyan;
						}
						break;

					case "TextBox":
						if(c.Tag != null && c.Text == "" && c.Tag.ToString() == "1")
						{
							c.BackColor = Color.Linen;
							c.Focus();
							Rtn = false;
						}
						//						else if(c.Tag != null && c.Text != "" && c.Tag.ToString() == "1")
						//						{
						//							c.BackColor = Color.LightCyan;
						//						}

						if(c.Tag != null && c.Text == "" && c.Tag.ToString() == "3")
						{
							c.BackColor = Color.Linen;
							c.Focus();
							Rtn = false;
						}
						else if(c.Tag != null && c.Text != "" && c.Tag.ToString() == "3")
						{
							c.BackColor = Color.LightCyan;
						}
						break;
					case "C1TextBox":
						if(c.Tag != null && c.Text == "" && c.Tag.ToString() == "1")
						{
							c.BackColor = Color.Linen;
							c.Focus();
							Rtn = false;
						}
						else if(c.Tag != null && c.Text != "" && c.Tag.ToString() == "1")
						{
							c.BackColor = Color.LightCyan;
						}

						if(c.Tag != null && c.Text == "" && c.Tag.ToString() == "3")
						{
							c.BackColor = Color.Linen;
							c.Focus();
							Rtn = false;
						}
						else if(c.Tag != null && c.Text != "" && c.Tag.ToString() == "3")
						{
							c.BackColor = Color.LightCyan;
						}
						break;
					case "C1NumericEdit":
						if(c.Tag != null && c.Text == "" && c.Tag.ToString() == "1")
						{
							c.BackColor = Color.Linen;
							c.Focus();
							Rtn = false;
						}
						else if(c.Tag != null && c.Text != "" && c.Tag.ToString() == "1")
						{
							c.BackColor = Color.LightCyan;
						}

						if(c.Tag != null && c.Text == "" && c.Tag.ToString() == "3")
						{
							c.BackColor = Color.Linen;
							c.Focus();
							Rtn = false;
						}
						else if(c.Tag != null && c.Text != "" && c.Tag.ToString() == "3")
						{
							c.BackColor = Color.LightCyan;
						}
						break;
					case "C1DateEdit":
						if(c.Tag != null && c.Text == "" && c.Tag.ToString() == "1")
						{
							c.BackColor = Color.Linen;
							c.Focus();
							Rtn = false;
						}
						else if(c.Tag != null && c.Text != "" && c.Tag.ToString() == "1")
						{
							c.BackColor = Color.LightCyan;
						}

						if(c.Tag != null && c.Text == "" && c.Tag.ToString() == "3")
						{
							c.BackColor = Color.Linen;
							c.Focus();
							Rtn = false;
						}
						else if(c.Tag != null && c.Text != "" && c.Tag.ToString() == "3")
						{
							c.BackColor = Color.LightCyan;
						}
						break;						
				}

				if(Rtn == false)
				{
					MessageBox.Show(SystemBase.Base.MessageRtn("SY007"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning); //필수항목을 입력하십시오.
					c.Focus();
					return false;
				}

				//User Contorl 일경우엔 재귀호출
				//if(Convert.ToString(c.Tag) == "UC")
					Rtn = GroupBoxExceptions2(c);

			}
			
			return Rtn;
		}
		#endregion

		#region Panel Exception 체크
		public static bool PanelException(Panel panel1)
		{// 사용법 : NamYoung.Common.Etc.PanelException(panel1)
			return PanelException2(panel1);
		}

		private static bool PanelException2(Control ctls)
		{// 사용법 : NamYoung.Common.Etc.PanelException(panel1)
			bool Rtn = false;
			foreach(System.Windows.Forms.Control c in ctls.Controls)                           
			{   
				//MessageBox.Show(c.GetType().Name.ToString());

				switch(c.GetType().Name)
				{
					case "ComboBox":
						ComboBox cb = (ComboBox)c;

						if(c.Tag != null && c.Text == "" && c.Tag.ToString() == "T")
						{
							c.BackColor = Color.Pink;
							Rtn = true;
						}
						else
						{
							c.BackColor = Color.White;
						}
						break;

					case "TextBox":
						if(c.Tag != null && c.Text == "" && c.Tag.ToString() == "T")
						{
							c.BackColor = Color.Pink;
							Rtn = true;
						}
						else
						{
							c.BackColor = Color.White;
						}
						break;
				}

				//User Contorl 일경우엔 재귀호출
				//if(Convert.ToString(c.Tag) == "UC")
					Rtn = PanelException2(c);
			}
			return Rtn;
		}
		#endregion

		#region TextBox 키 KeyPressEvent시 콤마 처리
		public static void Comma(System.Windows.Forms.KeyPressEventArgs e, System.Windows.Forms.TextBox textBox1)
		{// 콤마 넣기	NamYoung.Common.Etc.Comma(e, textBox1);

			if ((Convert.ToInt32(e.KeyChar) > 47 && Convert.ToInt32(e.KeyChar) < 58))
			{   
				if (textBox1.Text.Length > 2)
				{
					string temp = textBox1.Text.Replace(",", "");     // e.KeyChar의 문자는 아직 textBox에 써진 상태가 아닙니다.
					textBox1.Text = string.Format("{0:#,###}", Convert.ToInt64(temp + e.KeyChar.ToString()));
					textBox1.SelectionStart = textBox1.Text.Length;
					e.Handled = true;
				}
			}
			else if(Convert.ToInt32(e.KeyChar) == 8 )
			{
				if (textBox1.Text.Length > 2)
				{					
					string temp = textBox1.Text.Replace(",", "");     // e.KeyChar의 문자는 아직 textBox에 써진 상태가 아닙니다.
					textBox1.Text = string.Format("{0:#,###}", Convert.ToInt64(temp));
					textBox1.SelectionStart = textBox1.Text.Length;
					e.Handled = true;
				}
			}
			else        // 여기서 엔터나 백스페이스 등등 다른 키코드에 대한 처리를 하면.....
				e.Handled = true;
		}
		#endregion
		
		#region Text KeyPressEventArgs시 콤마 넣기 사용법 : Comma(e, textBox1, 2);
		public static void Comma(System.Windows.Forms.KeyPressEventArgs e, System.Windows.Forms.TextBox textBox1, int Lan)
		{	// 사용법 : Comma(e, textBox1, 2);
			if( (Convert.ToInt32(e.KeyChar) > 47 && Convert.ToInt32(e.KeyChar) < 58) || Convert.ToInt32(e.KeyChar) == 8 ||  Convert.ToInt32(e.KeyChar) == 46)
			{   
				if(Convert.ToInt32(e.KeyChar) == 8 && textBox1.SelectionStart == 0)
				{// SelectionStart는 0이고 백스페이스일때 아무런 변화 없음
				}
				else
				{
					int Focus = textBox1.SelectionStart;
					int MFocus = 0;

					string TxtOr;
					if(Convert.ToInt32(e.KeyChar) == 8 && Focus > 0)	//BackSpace를 누를때
					{
						if(textBox1.Text.Substring(textBox1.SelectionStart-1, 1) == ".")
						{
							TxtOr = textBox1.Text.Substring(0, textBox1.Text.Length - Lan -1) + textBox1.Text.Substring(textBox1.Text.Length - Lan -1, textBox1.Text.Length - textBox1.Text.Length + Lan -1);
							MFocus = -1;
						}
						else
							TxtOr = textBox1.Text.Substring(0, Focus-1) + textBox1.Text.Substring(Focus, textBox1.Text.Length - Focus);

					}
					else
						TxtOr = textBox1.Text.Substring(0, Focus) + e.KeyChar.ToString() + textBox1.Text.Substring(Focus, textBox1.Text.Length - Focus);

					if(e.KeyChar.ToString() == ".")
						TxtOr = textBox1.Text.Substring(0, Focus) + textBox1.Text.Substring(Focus, textBox1.Text.Length - Focus);

					string temp = TxtOr.Replace(",", "");

					if(Lan == 1)
						temp = string.Format("{0:#,###.0}", Convert.ToDecimal(temp));
					else if(Lan == 2)
						temp = string.Format("{0:#,###.00}", Convert.ToDecimal(temp));
					else if(Lan == 3)
						temp = string.Format("{0:#,###.000}", Convert.ToDecimal(temp));
					else if(Lan == 4)
						temp = string.Format("{0:#,###.0000}", Convert.ToDecimal(temp));
					else if(Lan == 5)
						temp = string.Format("{0:#,###.00000}", Convert.ToDecimal(temp));
					else if(Lan == 6)
						temp = string.Format("{0:#,###.000000}", Convert.ToDecimal(temp));
					else
						temp = string.Format("{0:#,###}", Convert.ToDecimal(temp));

					if(TxtOr.Length == 1)
						MFocus = MFocus + 0;
					else
						MFocus = MFocus + temp.Length - TxtOr.Length;

					if(temp.Length == 6 && MFocus > 1)
					{
						MFocus = 0;
					}

					int bbb = textBox1.Text.Substring(0, textBox1.SelectionStart).IndexOf(".",0);
					if(bbb > 0)
					{
						if(Convert.ToInt32(e.KeyChar) == 8)
						{
							MFocus = MFocus - 1;
						}
						else
						{
							MFocus = MFocus + 1;
						}
					}

					if(Convert.ToInt32(e.KeyChar) == 8)	//BackSpace를 클릭했을때 Focus -2
						MFocus = MFocus -2;

					textBox1.Text = temp;

					textBox1.SelectionStart = Focus+1   + MFocus;
					e.Handled = true;
				}
			}
			else
				e.Handled = true;

		}
		#endregion

		#region Text에 콤마 넣기
		public static string Comma2(string Txt)
		{// 콤마 넣기	SystemBase.Base.Comma2("55000");
			string Rtn="";
			if(Txt.Length > 2)
			{
				string temp = Txt.ToString().Replace(",", "");     // e.KeyChar의 문자는 아직 textBox에 써진 상태가 아닙니다.
				Rtn = string.Format("{0:#,###}", Convert.ToInt64(temp));
			}
			else
				Rtn = Txt;

			return Rtn;
			//		if(c1FlexGrid1.Rows.Count > 1)	// 호출 예제
			//		c1FlexGrid1.Rows[c1FlexGrid1.Row][10] = NamYoung.Common.Etc.Comma2(c1FlexGrid1[c1FlexGrid1.Row, 10].ToString());
		}
		#endregion

		#region 주민등록번호 체크
		public static bool JuminChk(string juminno)
		{//주민등록번호 체크
			bool Msg = false;
			string Jumin = juminno.Replace("-","");
			if(Jumin.Length != 13)
			{
				Msg = false;
			}
			else
			{
				int j = 2;
				int Cnt = 0;
				for(int i = 0; i < Jumin.Length-1; i++)
				{
					if(j > 1 && j < 10)
					{
						Cnt = Cnt + (j * Convert.ToInt32(Jumin.Substring(i,1).ToString()));
						j++;
					}
					else
					{
						j = 2;
						Cnt = Cnt + (j * Convert.ToInt32(Jumin.Substring(i,1).ToString()));
						j++;
					}
				}
				int Mod = Cnt % 11;
				Mod = 11 - Mod;
				Mod=Mod % 10;

				if(Mod != Convert.ToInt32(Jumin.Substring(12,1)) )
				{
					Msg = false;
				}
				else
				{
					Msg = true;
				}
			}
			return Msg;

			//			if(JuminChk(Jumin))
			//				MessageBox.Show("성공");
		}
		#endregion

		#region 주민등록번호 중복체크
		public static int JuminNOChk(string juminno)
		{//주민등록번호 중복체크
			int Msg = 0;
			string Jumin = juminno.Replace("-","");

			string Query = "Select Count(*) from MemberList Where Replace(Jumin,'-','') = '"+ Jumin.ToString() +"'";
			DataTable dt = DbOpen.NoTranDataTable(Query);

			Msg = Convert.ToInt32(dt.Rows[0][0].ToString());
			return Msg;
		}
		#endregion

		#region 암호화EnCode
		public static string EnCode(string Str)
		{//암호화	EnCode(textBox2.Text);
			int[] NanSuArr = new int[]{5,7,0,6,1,8,3,4,9,2};
			string RtnStr = "";

			Random rnd = new Random();
			for(int i = 0; i < Str.Length; i++)
			{
				string Tmp = Str.Substring(i,1);
				int NanSu = rnd.Next(9);
				int TmpNanSu = NanSu + 65;
				string FirstStr = Convert.ToChar(TmpNanSu).ToString();

				int SecondMod = (Convert.ToInt32(Convert.ToChar(Tmp.ToString())) % 29) + 65 + NanSuArr[NanSu] ;
				string SecondStr = Convert.ToChar(SecondMod).ToString();

				int Thirdint = ((Convert.ToInt32(Convert.ToChar(Tmp.ToString())) - (Convert.ToInt32(Convert.ToChar(Tmp.ToString())) % 29)) / 29 ) + 76 + NanSuArr[NanSu];
				string ThirdStr = Convert.ToChar(Thirdint).ToString();

				RtnStr = RtnStr + FirstStr.ToString() + SecondStr.ToString() + ThirdStr.ToString();
			}
			return RtnStr;
		}
		#endregion
		
		#region 복호화 DeCode
		public static string DeCode(string Str)
		{//복호화	DeCode(textBox1.Text);
			int[] NanSuArr = new int[]{5,7,0,6,1,8,3,4,9,2};
			string RtnStr = "";

			//Random rnd = new Random();
			for(int i = 0; i < Str.Length / 3; i++)
			{
				string Tmp1 = Str.Substring(((i+1)*3)-3,1);
				int First = Convert.ToChar(Convert.ToInt32(Convert.ToChar(Tmp1))) - 65;

				string Tmp2 = Str.Substring(((i+1)*3)-2,1);
				int Secondint = Convert.ToChar(Convert.ToInt32(Convert.ToChar(Tmp2))) - 65 - NanSuArr[First] ;

				string Tmp3 = Str.Substring(((i+1)*3)-1,1);
				int Thirdint = Convert.ToChar(Convert.ToInt32(Convert.ToChar(Tmp3))) - 76 - NanSuArr[First];

				RtnStr = RtnStr + Convert.ToChar(Convert.ToInt32(Convert.ToChar( ((Thirdint * 29) + Secondint) ))).ToString();
			}
			return RtnStr;
		}
		#endregion

		#region 암호화(S) Encode
		public static string  Encode( string strEncode )
		{
			int i = 0;
			int lens = 0;
			int conv = 0;
			string temp = "";
			string temps;

			temp = strEncode;
			lens = strEncode.Length;
			temp = "";

			for( i = 1; i < lens+1; i++ )
			{
				conv = i % 3;
				
				temps = strEncode.Substring(lens-i, 1);

				temp = temp + Convert.ToChar(Convert.ToInt32(Convert.ToChar(temps)) + conv);
			}

			return temp;
		}
		#endregion

		#region 복호화(S) Decode
		public static string  Decode( string strEncode )
		{
			int i = 0;
			int lens = 0;
			int conv = 0;
			string temp = "";
			string temps;

			temp = strEncode;
			lens = strEncode.Length;
			temp = "";

			for( i = lens; i > 0; i-- )
			{
				conv = i % 3;
				
				temps = strEncode.Substring(i-1, 1);

				temp = temp + Convert.ToChar(Convert.ToInt32(Convert.ToChar(temps)) - conv);
			}

			return temp;
		}
		#endregion

		#region 공통 메세지
		/******************************************************************************************************
		 * 	MessageBox.Show(SystemBase.Base.MessageRtn("P0002")); 결과 : 성공적으로 처리되었습니다.
		 *  MessageBox.Show(SystemBase.Base.MessageRtn("P0002"), "제목",MessageBoxButtons.OK);
		 *  MessageBox.Show("메세지", "제목",MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
		******************************************************************************************************/
		public static string MessageRtn( string MSG_CODE )
		{
			string Msg = "Message Code not Found (CODE : " +  MSG_CODE.Replace("'","'''") + ")";//메세지 코드가 잘못 입력되었거나 등록되지 않았습니다.
			try
			{
				//Msg = "Message Code Fail (CODE : " + MSG_CODE + ")";//메세지 코드가 잘못 입력되었거나 등록되지 않았습니다.
				string Query = "Select MSG_NAME From CO_SYS_MSG(Nolock) Where MSG_CODE = '"+ MSG_CODE.Replace("'","''") +"' ";
				DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);

				if(dt.Rows.Count > 0)
					Msg = dt.Rows[0][0].ToString();
				else
					Msg = MSG_CODE;
			}
			catch//(Exception e)
			{
				MessageBox.Show("메세지 코드가 잘못 입력되었거나 등록되지 않았습니다.");
			}
			return Msg;
		}
		#endregion

		#region 공통 메세지
		/******************************************************************************************
		 *   - 호출 : MessageBox.Show(SystemBase.Base.MessageRtn("SY023", "5||7"));
		 *   - B0010에 저장된 메세지 예 : ||번째 Row ||번째 칼럼은 필수항목입니다.
		 *   - 결과 : 5번째 Row 7번째 칼럼은 필수항목입니다.
		 * ***************************************************************************************/
		public static string MessageRtn( string MSG_CODE, string ReplaceMsg )
		{
			string Msg = "Message Code Fail (CODE : " + MSG_CODE + ")";
			try
			{
				string Query = "Select MSG_NAME From CO_SYS_MSG(Nolock) Where MSG_CODE = '"+ MSG_CODE.Replace("'","''") +"' ";
				DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);

				if(dt.Rows.Count > 0)
				{
					Msg = dt.Rows[0][0].ToString();

					ReplaceMsg = ReplaceMsg.Replace("||","#");

					Regex r = new Regex("#");
					string[] s = r.Split(ReplaceMsg);

					for(int i = 0; i < s.Length; i++)
					{
						if(Msg.IndexOf("||", 0) > -1)
						{
							Msg = Msg.Substring(0, Msg.IndexOf("||", 0)) + s[i].ToString() + Msg.Substring(Msg.IndexOf("||", 0)+2, Msg.Length - Msg.IndexOf("||", 0)-2);
						}
					}
				}
				else
				{
					Msg = MSG_CODE;

					ReplaceMsg = ReplaceMsg.Replace("||","#");

					Regex r = new Regex("#");
					string[] s = r.Split(ReplaceMsg);

					for(int i = 0; i < s.Length; i++)
					{
						if(Msg.IndexOf("||", 0) > -1)
						{
							Msg = Msg.Substring(0, Msg.IndexOf("||", 0)) + s[i].ToString() + Msg.Substring(Msg.IndexOf("||", 0)+2, Msg.Length - Msg.IndexOf("||", 0)-2);
						}
					}

				}

			}
			catch(Exception f)
			{
				SystemBase.Loggers.Log("MessageRtn ", f.ToString());
				MessageBox.Show("메세지 코드가 잘못 입력되었거나 등록되지 않았습니다.");
			}
			return Msg;
		}
		#endregion

		#region NM호출
		public static string CodeName( string SCode, string Name, string Table, string Code, string AddQuery)
		{
			// 사용법 txtCust_Nm.Text = SystemBase.Base.CodeName("CUST_CD","CUST_NM", "B_CUST_INFO", txtCust_Cd.Text, "");

			string Query = "Select Isnull( " + Name + " ,'') From "+ Table +" (Nolock) Where "+ SCode +" = '"+ Code +"' "+ AddQuery +" ";
			DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);

			if(dt.Rows.Count > 0)
			{
				Name = dt.Rows[0][0].ToString();
			}
			else
			{
				Name = "";
			}

			return Name;
		}
		#endregion

		#region 데이터존재여부
		public static bool CodeCheck( string SCode, string Name, string Table, string Code, string AddQuery)
		{
			// 사용법 txtCust_Nm.Text = SystemBase.Base.CodeName("CUST_CD","CUST_NM", "B_CUST_INFO", txtCust_Cd.Text, "");

			string Query = "Select count(*) cnt From "+ Table +" (Nolock) Where "+ SCode +" = '"+ Code +"' "+ AddQuery +" ";
			int resultCnt = Convert.ToInt32(SystemBase.DbOpen.NoTranScalar(Query));

			if(resultCnt > 0)
				return true;
			else
				return false;
		}
		#endregion

		#region GroupBoxLang 라벨 다국어 적용 및 싱글모드 입력창 재정의

		public static void GroupBoxLang(GroupBox groupBox1, string Lang, string FormName)
		{
			string Query = "SELECT SEQ, LBL_SEQ, LBL_NM FROM B_LABEL_NM WHERE LANG_CD = '"+ Lang.ToString() +"' AND FORM_ID = '"+ FormName.ToString() +"' AND GROUP_NM = '"+ groupBox1.Name.ToString() +"' ";
			DataSet ds = SystemBase.DbOpen.NoTranDataSet(Query);

			GroupBoxLang2(ds, groupBox1, Lang, FormName);
		}

		private static void GroupBoxLang2(DataSet ds, Control ctls, string Lang, string FormName)
		{// 사용법 : SystemBase.Base.GroupBoxLang(groupBox1);
			try
			{
				if(ds.Tables[0].Rows.Count > 0)
				{
					foreach(System.Windows.Forms.Control c in ctls.Controls)                           
					{   
						if(c.GetType().Name == "Label")
						{
							Label lb = (Label)c;
							if(lb.Tag != null && lb.Tag.ToString().Length > 0)
							{
								if(ds.Tables[0].Select("LBL_SEQ="+lb.Tag.ToString()) != null)
								{
									lb.Text = ds.Tables[0].Select("LBL_SEQ="+lb.Tag.ToString())[0][2].ToString();
								}
							}
						}
						else if(c.GetType().Name == "CheckBox")
						{
							CheckBox cb = (CheckBox)c;
							if(cb.Tag != null && cb.Tag.ToString().Length > 0)
							{
								if(ds.Tables[0].Select("LBL_SEQ="+cb.Tag.ToString()) != null)
								{
									cb.Text = ds.Tables[0].Select("LBL_SEQ="+cb.Tag.ToString())[0][2].ToString();
								}
							}
						}
						else if(c.GetType().Name == "RadioButton")
						{
							RadioButton rb = (RadioButton)c;
							if(rb.Tag != null && rb.Tag.ToString().Length > 0)
							{
								if(ds.Tables[0].Select("LBL_SEQ="+rb.Tag.ToString()) != null)
								{
									rb.Text = ds.Tables[0].Select("LBL_SEQ="+rb.Tag.ToString())[0][2].ToString();
								}
							}
						}
						else if(c.GetType().Name == "Button")
						{
							Button bb = (Button)c;
							if(bb.Tag != null && bb.Tag.ToString().Length > 0)
							{
								if(ds.Tables[0].Select("LBL_SEQ="+bb.Tag.ToString()) != null)
								{
									bb.Text = ds.Tables[0].Select("LBL_SEQ="+bb.Tag.ToString())[0][2].ToString();
								}
							}
						}
						else if(c.GetType().Name == "TextBox")
						{
							TextBox tb = (TextBox)c;
							if(tb.Tag != null && tb.Tag.ToString().Length > 0)
							{
								if(tb.Tag.ToString() == "1")
								{	// 필수항목
									tb.BackColor = Color.LightCyan;
								}
								else if(tb.Tag.ToString() == "2")
								{	// 읽기전용
									tb.BackColor = Color.Gainsboro;
									tb.ReadOnly = true;
								}
								else if(tb.Tag.ToString() == "3")
								{	// 읽기전용, 필수
									tb.BackColor = Color.LightCyan;
									tb.ReadOnly = true;
								}
								else if(tb.Tag.ToString() == "4")
								{	// 읽기전용, 필수, 회색
									tb.BackColor = Color.Gainsboro;
									tb.ReadOnly = true;
								}
								else if(tb.Tag.ToString() == "5")
								{	// 필수항목 영문 대문자만 사용시
									tb.BackColor = Color.LightCyan;
									tb.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;//영문 대문자만 사용시
								}
							}
						}
						else if(c.GetType().Name == "C1TextBox" || c.GetType().Name == "C1NumericEdit")
						{
							TextBox tb = (TextBox)c;
							if(tb.Tag != null && tb.Tag.ToString().Length > 0)
							{
								if(tb.Tag.ToString() == "1")
								{	// 필수항목
									tb.BackColor = Color.LightCyan;
								}
								else if(tb.Tag.ToString() == "2")
								{	// 읽기전용
									tb.BackColor = Color.Gainsboro;
									tb.ReadOnly = true;
								}
								else if(tb.Tag.ToString() == "3")
								{	// 읽기전용, 필수
									tb.BackColor = Color.LightCyan;
									tb.ReadOnly = true;
								}
								else if(tb.Tag.ToString() == "4")
								{	// 읽기전용, 필수, 회색
									tb.BackColor = Color.Gainsboro;
									tb.ReadOnly = true;
								}
								else if(tb.Tag.ToString() == "5")
								{	// 필수항목 영문 대문자만 사용시
									tb.BackColor = Color.LightCyan;
									tb.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;//영문 대문자만 사용시
								}
							}
						}
						else if(c.GetType().Name == "ComboBox")
						{
							ComboBox cb = (ComboBox)c;
							if(cb.Tag != null && cb.Tag.ToString().Length > 0)
							{
								if(cb.Tag.ToString() == "1" || cb.Tag.ToString() == "3")
								{
									cb.BackColor = Color.LightCyan;
								}
								else if(cb.Tag.ToString() == "2")
								{
									cb.BackColor = Color.Gainsboro;
									cb.Enabled = false;
								}
							}
						}
						else if(c.GetType().Name == "GroupBox")
						{
							GroupBox cb = (GroupBox)c;

							if(cb.Tag != null && cb.Tag.ToString().Length > 0)
							{
								if(ds.Tables[0].Select("LBL_SEQ="+cb.Tag.ToString()) != null)
								{
									cb.Text = ds.Tables[0].Select("LBL_SEQ="+cb.Tag.ToString())[0][2].ToString();
								}
							}
						}
						else if(c.GetType().Name == "C1DateEdit")
						{
							TextBox tb = (TextBox)c;
							if(tb.Tag != null && tb.Tag.ToString().Length > 0)
							{
								if(tb.Tag.ToString() == "1")
								{	// 필수항목
									tb.BackColor = Color.LightCyan;
								}
								else if(tb.Tag.ToString() == "2")
								{	// 읽기전용
									tb.BackColor = Color.Gainsboro;
									tb.ReadOnly = true;
								}
								else if(tb.Tag.ToString() == "3")
								{	// 읽기전용, 필수
									tb.BackColor = Color.LightCyan;
									tb.ReadOnly = true;
								}
								else if(tb.Tag.ToString() == "4")
								{	// 읽기전용, 필수, 회색
									tb.BackColor = Color.Gainsboro;
									tb.ReadOnly = true;
								}
							}
						}

						GroupBoxLang2(ds, c, Lang, FormName);

					}

				}
			}
			catch(Exception f)
			{
				SystemBase.Loggers.Log("GroupBoxLang (다국어 라벨 미등록 에러)", f.ToString());
				MessageBox.Show(SystemBase.Base.MessageRtn("B0020"));

			}
		}
		#endregion

		#region GroupBoxLang() GroupBox위 싱글모드 입력창 필수항목 재정의

		public static void GroupBoxLang(GroupBox groupBox1)
		{
			GroupBoxLang2(groupBox1);
		}

		private static void GroupBoxLang2(Control ctls)
		{// 사용법 : txtPlant_NM.Tag = "3";	SystemBase.Base.GroupBoxLang( groupBox1 );	
			foreach(System.Windows.Forms.Control c in ctls.Controls)                           
			{
				if(c.GetType().Name == "TextBox")
				{
					TextBox tb = (TextBox)c;
					if(tb.Tag != null && tb.Tag.ToString().Length > 0)
					{
						if(tb.Tag.ToString() == "1")
						{	// 필수항목
							tb.BackColor = Color.LightCyan;
							tb.ReadOnly = false;
							tb.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;//영문 대문자만 사용시
						}
						else if(tb.Tag.ToString() == "2")
						{	// 읽기전용
							tb.BackColor = Color.Gainsboro;
							tb.ReadOnly = true;
						}
						else if(tb.Tag.ToString() == "3")
						{	// 읽기전용, 필수
							tb.BackColor = Color.LightCyan;
							tb.ReadOnly = true;
						}
						else if(tb.Tag.ToString() == "4")
						{	// 읽기전용, 필수, 회색
							tb.BackColor = Color.Gainsboro;
							tb.ReadOnly = true;
						}
						else
						{
							tb.BackColor = Color.White;
							tb.ReadOnly = false;
						}

					}
				}
				else if(c.GetType().Name == "C1TextBox" || c.GetType().Name == "C1NumericEdit")
				{
					TextBox tb = (TextBox)c;
					if(tb.Tag != null && tb.Tag.ToString().Length > 0)
					{
						if(tb.Tag.ToString() == "1")
						{	// 필수항목
							tb.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;//영문 대문자만 사용시
							tb.BackColor = Color.LightCyan;
							tb.ReadOnly = false;
						}
						else if(tb.Tag.ToString() == "2")
						{	// 읽기전용
							tb.BackColor = Color.Gainsboro;
							tb.ReadOnly = true;
						}
						else if(tb.Tag.ToString() == "3")
						{	// 읽기전용, 필수
							tb.BackColor = Color.LightCyan;
							tb.ReadOnly = true;
						}
						else if(tb.Tag.ToString() == "4")
						{	// 읽기전용, 필수, 회색
							tb.BackColor = Color.Gainsboro;
							tb.ReadOnly = true;
						}
						else
						{
							tb.BackColor = Color.White;
							tb.ReadOnly = false;
						}

					}
				}
				else if(c.GetType().Name == "ComboBox")
				{
					ComboBox cb = (ComboBox)c;
					if(cb.Tag != null && cb.Tag.ToString().Length > 0)
					{
						if(cb.Tag.ToString() == "1")
						{	// 필수항목
							cb.BackColor = Color.LightCyan;
							cb.Enabled = true;
						}
						else if(cb.Tag.ToString() == "2")
						{	// 읽기전용
							cb.BackColor = Color.Gainsboro;
							cb.Enabled = false;
						}
						else if(cb.Tag.ToString() == "3")
						{	// 읽기전용, 필수
							cb.BackColor = Color.LightCyan;
							cb.Enabled = false;
						}
						else
						{
							cb.BackColor = Color.White;
							cb.Enabled = true;
						}
					}
				}
				else if(c.GetType().Name == "C1DateEdit")
				{
					TextBox tb = (TextBox)c;
					if(tb.Tag != null && tb.Tag.ToString().Length > 0)
					{
						if(tb.Tag.ToString() == "1")
						{	// 필수항목
							tb.BackColor = Color.LightCyan;
							tb.ReadOnly = false;
						}
						else if(tb.Tag.ToString() == "2")
						{	// 읽기전용
							tb.BackColor = Color.Gainsboro;
							tb.ReadOnly = true;
						}
						else if(tb.Tag.ToString() == "3")
						{	// 읽기전용, 필수
							tb.BackColor = Color.LightCyan;
							tb.ReadOnly = true;
						}
						else if(tb.Tag.ToString() == "4")
						{	// 읽기전용, 필수, 회색
							tb.BackColor = Color.Gainsboro;
							tb.ReadOnly = true;
						}
						else
						{
							tb.BackColor = Color.White;
							tb.ReadOnly = false;
						}
					}
				}
				else if(c.GetType().Name == "DateTimePicker")
				{
					DateTimePicker dt = (DateTimePicker)c;

					if(dt.Tag != null && dt.Tag.ToString().Length > 0)
					{
						if(dt.Tag.ToString() == "1")
						{	// 필수항목
							dt.BackColor = Color.LightCyan;
							dt.Enabled = true;
						}
						else if(dt.Tag.ToString() == "2")
						{	// 읽기전용
							dt.BackColor = Color.Gainsboro;
							dt.Enabled = false;
						}
						else if(dt.Tag.ToString() == "3")
						{	// 읽기전용, 필수
							dt.BackColor = Color.LightCyan;
							dt.Enabled = false;
						}
						else if(dt.Tag.ToString() == "4")
						{	// 읽기전용, 필수, 회색
							dt.BackColor = Color.Gainsboro;
							dt.Enabled = false;
						}
						else
						{
							dt.BackColor = Color.White;
							dt.Enabled = true;
						}

					}
				}
				else if(c.GetType().Name == "Button")
				{
					Button bt = (Button)c;
					if(bt.Tag != null && bt.Tag.ToString() == "2") bt.Enabled = false;
					else bt.Enabled = true;
				}

				//User Contorl 일경우엔 재귀호출
				//if(Convert.ToString(c.Tag) == "UC")
					GroupBoxLang2(c);

			}
		}
		#endregion

		#region PanelLang() Panel위 싱글모드 입력창 재정의
		public static void PanelLang(Panel panel1)
		{
			PanelLang2(panel1);
		}
		
		private static void PanelLang2(Control ctls)
		{// 사용법 : txtPlant_NM.Tag = "3";	SystemBase.Base.GroupBoxLang( groupBox1 );	
			foreach(System.Windows.Forms.Control c in ctls.Controls)                           
			{
				if(c.GetType().Name == "TextBox")
				{
					TextBox tb = (TextBox)c;
					if(tb.Tag != null && tb.Tag.ToString().Length > 0)
					{
						if(tb.Tag.ToString() == "1")
						{	// 필수항목
							tb.BackColor = Color.LightCyan;
							tb.ReadOnly = false;
						}
						else if(tb.Tag.ToString() == "2")
						{	// 읽기전용
							tb.BackColor = Color.Gainsboro;
							tb.ReadOnly = true;
						}
						else if(tb.Tag.ToString() == "3")
						{	// 읽기전용, 필수
							tb.BackColor = Color.LightCyan;
							tb.ReadOnly = true;
						}
						else if(tb.Tag.ToString() == "4")
						{	// 읽기전용, 필수, 회색
							tb.BackColor = Color.Gainsboro;
							tb.ReadOnly = true;
						}
						else
						{
							tb.BackColor = Color.White;
							tb.ReadOnly = false;
						}
						tb.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;//영문 대문자만 사용시
					}
				}
				else if(c.GetType().Name == "C1TextBox" || c.GetType().Name == "C1NumericEdit")
				{
					TextBox tb = (TextBox)c;
					if(tb.Tag != null && tb.Tag.ToString().Length > 0)
					{
						if(tb.Tag.ToString() == "1")
						{	// 필수항목
							tb.BackColor = Color.LightCyan;
							tb.ReadOnly = false;
						}
						else if(tb.Tag.ToString() == "2")
						{	// 읽기전용
							tb.BackColor = Color.Gainsboro;
							tb.ReadOnly = true;
						}
						else if(tb.Tag.ToString() == "3")
						{	// 읽기전용, 필수
							tb.BackColor = Color.LightCyan;
							tb.ReadOnly = true;
						}
						else if(tb.Tag.ToString() == "4")
						{	// 읽기전용, 필수, 회색
							tb.BackColor = Color.Gainsboro;
							tb.ReadOnly = true;
						}
						else
						{
							tb.BackColor = Color.White;
							tb.ReadOnly = false;
						}

						tb.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;//영문 대문자만 사용시
					}
				}
				else if(c.GetType().Name == "ComboBox")
				{
					ComboBox cb = (ComboBox)c;
					if(cb.Tag != null && cb.Tag.ToString().Length > 0)
					{
						if(cb.Tag.ToString() == "1")
						{
							cb.BackColor = Color.LightCyan;
							cb.Enabled = true;
						}
						else if(cb.Tag.ToString() == "2")
						{
							cb.BackColor = Color.Gainsboro;
							cb.Enabled = false;
						}
						else if(cb.Tag.ToString() == "3")
						{
							cb.BackColor = Color.LightCyan;
							cb.Enabled = false;
						}
						else if(cb.Tag.ToString() == "4")
						{	// 읽기전용, 필수, 회색
							cb.BackColor = Color.Gainsboro;
							cb.Enabled = false;
						}
						else
						{
							cb.BackColor = Color.White;
							cb.Enabled = true;
						}

					}
				}
				else if(c.GetType().Name == "C1DateEdit")
				{
					TextBox tb = (TextBox)c;
					if(tb.Tag != null && tb.Tag.ToString().Length > 0)
					{
						if(tb.Tag.ToString() == "1")
						{	// 필수항목
							tb.BackColor = Color.LightCyan;
							tb.ReadOnly = false;
						}
						else if(tb.Tag.ToString() == "2")
						{	// 읽기전용
							tb.BackColor = Color.Gainsboro;
							tb.ReadOnly = true;
						}
						else if(tb.Tag.ToString() == "3")
						{	// 읽기전용, 필수
							tb.BackColor = Color.LightCyan;
							tb.ReadOnly = true;
						}
						else if(tb.Tag.ToString() == "4")
						{	// 읽기전용, 필수, 회색
							tb.BackColor = Color.Gainsboro;
							tb.ReadOnly = true;
						}
						else
						{
							tb.BackColor = Color.White;
							tb.ReadOnly = false;
						}

					}
				}
				else if(c.GetType().Name == "DateTimePicker")
				{
					DateTimePicker dt = (DateTimePicker)c;

					if(dt.Tag != null && dt.Tag.ToString().Length > 0)
					{
						if(dt.Tag.ToString() == "1")
						{	// 필수항목
							dt.BackColor = Color.LightCyan;
							dt.Enabled = true;
						}
						else if(dt.Tag.ToString() == "2")
						{	// 읽기전용
							dt.BackColor = Color.Gainsboro;
							dt.Enabled = false;
						}
						else if(dt.Tag.ToString() == "3")
						{	// 읽기전용, 필수
							dt.BackColor = Color.LightCyan;
							dt.Enabled = false;
						}
						else if(dt.Tag.ToString() == "4")
						{	// 읽기전용, 필수, 회색
							dt.BackColor = Color.Gainsboro;
							dt.Enabled = false;
						}
						else
						{
							dt.BackColor = Color.White;
							dt.Enabled = true;
						}

					}
				}
				else if(c.GetType().Name == "Button")
				{
					Button bt = (Button)c;
					bt.Enabled = true;
				}

				//User Contorl 일경우엔 재귀호출
				//if(Convert.ToString(c.Tag) == "UC")
					PanelLang2(c);

			}
		}
		#endregion

		#region GroupBoxLock 싱글모드 입력창 전체 Lock

		public static void GroupBoxLock(GroupBox groupBox1)
		{
			GroupBoxLock2(groupBox1);
		}

		private static void GroupBoxLock2(Control ctls)
		{// 사용법 : SystemBase.Base.GroupBoxLock( groupBox1 );	
			foreach(System.Windows.Forms.Control c in ctls.Controls)                           
			{
				if(c.GetType().Name == "TextBox")
				{
					TextBox tb = (TextBox)c;
					tb.BackColor = Color.Gainsboro;
					tb.ReadOnly = true;
				}
				else if(c.GetType().Name == "C1TextBox" || c.GetType().Name == "C1NumericEdit")
				{
					TextBox tb = (TextBox)c;
					tb.BackColor = Color.Gainsboro;
					tb.ReadOnly = true;
				}
				else if(c.GetType().Name == "ComboBox")
				{
					ComboBox cb = (ComboBox)c;
					cb.BackColor = Color.Gainsboro;
					cb.Enabled = false;
				}
				else if(c.GetType().Name == "C1DateEdit")
				{
					TextBox tb = (TextBox)c;
					tb.BackColor = Color.Gainsboro;
					tb.ReadOnly = true;
				}
				else if(c.GetType().Name == "DateTimePicker")
				{
					DateTimePicker dt = (DateTimePicker)c;
					dt.BackColor = Color.Gainsboro;
					dt.Enabled = false;
				}
				else if(c.GetType().Name == "Button")
				{
					Button bt = (Button)c;
					bt.Enabled = false;
				}
				else if(c.GetType().Name == "RadioButton")
				{
					RadioButton rb = (RadioButton)c;
					rb.BackColor = Color.Gainsboro;
					rb.Enabled = false;
				}
				else if(c.GetType().Name == "CheckBox")
				{
					CheckBox rb = (CheckBox)c;
					rb.BackColor = Color.Gainsboro;
					rb.Enabled = false;
				}
				else if(c.GetType().Name == "Panel")
				{
					Panel pl = (Panel)c;
					pl.Enabled = false;
				}

				//User Contorl 일경우엔 재귀호출
				//if(Convert.ToString(c.Tag) == "UC")
					GroupBoxLock2(c);

			}
		}
		#endregion

		#region 그룹박스 상단 컨트롤만 락, 해제

		public static void GroupBoxLock(GroupBox groupBox1, bool Lock)
		{
			GroupBoxLock2(groupBox1, Lock);
		}

		private static void GroupBoxLock2(Control ctls, bool Lock)
		{// 사용법 : SystemBase.Base.GroupBoxLock( groupBox1 , true);	

			if(Lock == true)
			{
				foreach(System.Windows.Forms.Control c in ctls.Controls)                           
				{
					if(c.GetType().Name == "TextBox")
					{
						TextBox tb = (TextBox)c;
						tb.BackColor = Color.Gainsboro;
						tb.ReadOnly = true;
					}
					else if(c.GetType().Name == "C1TextBox" || c.GetType().Name == "C1NumericEdit")
					{
						TextBox tb = (TextBox)c;
						tb.BackColor = Color.Gainsboro;
						tb.ReadOnly = true;
					}
					else if(c.GetType().Name == "ComboBox")
					{
						ComboBox cb = (ComboBox)c;
						cb.BackColor = Color.Gainsboro;
						cb.Enabled = false;
					}
					else if(c.GetType().Name == "C1DateEdit")
					{
						TextBox tb = (TextBox)c;
						tb.BackColor = Color.Gainsboro;
						tb.ReadOnly = true;
					}
					else if(c.GetType().Name == "DateTimePicker")
					{
						DateTimePicker dt = (DateTimePicker)c;
						dt.BackColor = Color.Gainsboro;
						dt.Enabled = false;
					}
					else if(c.GetType().Name == "Button")
					{
						Button bt = (Button)c;
						bt.Enabled = false;
					}
					else if(c.GetType().Name == "RadioButton")
					{
						RadioButton rb = (RadioButton)c;
						rb.BackColor = Color.Gainsboro;
						rb.Enabled = false;
					}
					else if(c.GetType().Name == "CheckBox")
					{
						CheckBox rb = (CheckBox)c;
						rb.BackColor = Color.Gainsboro;
						rb.Enabled = false;
					}
					else if(c.GetType().Name == "Panel")
					{
						Panel pl = (Panel)c;
						pl.Enabled = false;
					}

					GroupBoxLock2(c, Lock);

				}
			}
			else if(Lock == false)
			{	// 싱글모드 재정의
				foreach(System.Windows.Forms.Control c in ctls.Controls)                           
				{
					if(c.GetType().Name == "TextBox")
					{
						TextBox tb = (TextBox)c;
						if(tb.Tag != null && tb.Tag.ToString().Length > 0)
						{
							if(tb.Tag.ToString() == "1")
							{	// 필수항목
								tb.BackColor = Color.LightCyan;
								tb.ReadOnly = false;
							}
							else if(tb.Tag.ToString() == "2")
							{	// 읽기전용
								tb.BackColor = Color.Gainsboro;
								tb.ReadOnly = true;
							}
							else if(tb.Tag.ToString() == "3")
							{	// 읽기전용, 필수
								tb.BackColor = Color.LightCyan;
								tb.ReadOnly = true;
							}
							else if(tb.Tag.ToString() == "4")
							{	// 읽기전용, 필수, 회색
								tb.BackColor = Color.Gainsboro;
								tb.ReadOnly = true;
							}
							else
							{
								tb.BackColor = Color.White;
								tb.ReadOnly = false;
							}

							//tb.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;//영문 대문자만 사용시
						}
						else
						{
							tb.BackColor = Color.White;
							tb.ReadOnly = false;
						}
					}
					else if(c.GetType().Name == "C1TextBox" || c.GetType().Name == "C1NumericEdit")
					{
						TextBox tb = (TextBox)c;
						if(tb.Tag != null && tb.Tag.ToString().Length > 0)
						{
							if(tb.Tag.ToString() == "1")
							{	// 필수항목
								tb.BackColor = Color.LightCyan;
								tb.ReadOnly = false;
							}
							else if(tb.Tag.ToString() == "2")
							{	// 읽기전용
								tb.BackColor = Color.Gainsboro;
								tb.ReadOnly = true;
							}
							else if(tb.Tag.ToString() == "3")
							{	// 읽기전용, 필수
								tb.BackColor = Color.LightCyan;
								tb.ReadOnly = true;
							}
							else if(tb.Tag.ToString() == "4")
							{	// 읽기전용, 필수, 회색
								tb.BackColor = Color.Gainsboro;
								tb.ReadOnly = true;
							}
							else
							{
								tb.BackColor = Color.White;
								tb.ReadOnly = false;
							}

							//tb.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;//영문 대문자만 사용시
						}
						else
						{
							tb.BackColor = Color.White;
							tb.ReadOnly = false;
						}
					}
					else if(c.GetType().Name == "ComboBox")
					{
						ComboBox cb = (ComboBox)c;
						if(cb.Tag != null && cb.Tag.ToString().Length > 0)
						{
							if(cb.Tag.ToString() == "1")
							{	// 필수항목
								cb.BackColor = Color.LightCyan;
								cb.Enabled = true;
							}
							else if(cb.Tag.ToString() == "2")
							{	// 읽기전용
								cb.BackColor = Color.Gainsboro;
								cb.Enabled = false;
							}
							else if(cb.Tag.ToString() == "3")
							{	// 읽기전용, 필수
								cb.BackColor = Color.LightCyan;
								cb.Enabled = false;
							}
							else
							{
								cb.BackColor = Color.White;
								cb.Enabled = true;
							}
						}
						else
						{
							cb.BackColor = Color.White;
							cb.Enabled = true;
						}
					}
					else if(c.GetType().Name == "RadioButton")
					{
						RadioButton rb = (RadioButton)c;
						if(rb.Tag != null && rb.Tag.ToString().Length > 0)
						{
							if(rb.Tag.ToString() == "1")
							{	// 필수항목
								rb.BackColor = Color.LightCyan;
								rb.Enabled = true;
							}
							else if(rb.Tag.ToString() == "2")
							{	// 읽기전용
								rb.BackColor = Color.Gainsboro;
								rb.Enabled = false;
							}
							else if(rb.Tag.ToString() == "3")
							{	// 읽기전용, 필수
								rb.BackColor = Color.LightCyan;
								rb.Enabled = false;
							}
							else
							{
								rb.BackColor = Color.White;
								rb.Enabled = true;
							}
						}
						else
						{
							rb.BackColor = Color.White;
							rb.Enabled = true;
						}
					}
					else if(c.GetType().Name == "CheckBox")
					{
						CheckBox rb = (CheckBox)c;
						if(rb.Tag != null && rb.Tag.ToString().Length > 0)
						{
							if(rb.Tag.ToString() == "1")
							{	// 필수항목
								rb.BackColor = Color.LightCyan;
								rb.Enabled = true;
							}
							else if(rb.Tag.ToString() == "2")
							{	// 읽기전용
								rb.BackColor = Color.Gainsboro;
								rb.Enabled = false;
							}
							else if(rb.Tag.ToString() == "3")
							{	// 읽기전용, 필수
								rb.BackColor = Color.LightCyan;
								rb.Enabled = false;
							}
							else
							{
								rb.BackColor = Color.White;
								rb.Enabled = true;
							}
						}
						else
						{
							rb.BackColor = Color.White;
							rb.Enabled = true;
						}
					}
					else if(c.GetType().Name == "C1DateEdit")
					{
						TextBox tb = (TextBox)c;
						if(tb.Tag != null && tb.Tag.ToString().Length > 0)
						{
							if(tb.Tag.ToString() == "1")
							{	// 필수항목
								tb.BackColor = Color.LightCyan;
								tb.ReadOnly = false;
							}
							else if(tb.Tag.ToString() == "2")
							{	// 읽기전용
								tb.BackColor = Color.Gainsboro;
								tb.ReadOnly = true;
							}
							else if(tb.Tag.ToString() == "3")
							{	// 읽기전용, 필수
								tb.BackColor = Color.LightCyan;
								tb.ReadOnly = true;
							}
							else if(tb.Tag.ToString() == "4")
							{	// 읽기전용, 필수, 회색
								tb.BackColor = Color.Gainsboro;
								tb.ReadOnly = true;
							}
							else
							{
								tb.BackColor = Color.White;
								tb.ReadOnly = false;
							}
						}
						else
						{
							tb.BackColor = Color.White;
							tb.ReadOnly = false;
						}
					}
					else if(c.GetType().Name == "DateTimePicker")
					{
						DateTimePicker dt = (DateTimePicker)c;

						if(dt.Tag != null && dt.Tag.ToString().Length > 0)
						{
							if(dt.Tag.ToString() == "1")
							{	// 필수항목
								dt.BackColor = Color.LightCyan;
								dt.Enabled = true;
							}
							else if(dt.Tag.ToString() == "2")
							{	// 읽기전용
								dt.BackColor = Color.Gainsboro;
								dt.Enabled = false;
							}
							else if(dt.Tag.ToString() == "3")
							{	// 읽기전용, 필수
								dt.BackColor = Color.LightCyan;
								dt.Enabled = false;
							}
							else if(dt.Tag.ToString() == "4")
							{	// 읽기전용, 필수, 회색
								dt.BackColor = Color.Gainsboro;
								dt.Enabled = false;
							}
							else
							{
								dt.BackColor = Color.White;
								dt.Enabled = true;
							}
						}
						else
						{
							dt.BackColor = Color.White;
							dt.Enabled = true;
						}
					}
					else if(c.GetType().Name == "Button")
					{
						Button bt = (Button)c;
						bt.Enabled = true;
					}
					else if(c.GetType().Name == "Panel")
					{
						Panel pl = (Panel)c;
						pl.Enabled = true;
					}

					GroupBoxLock2(c, Lock);

				}
			}
			
		}
		#endregion

		#region 그룹박스 상단 컨트롤 T:락  F:락 해제 R:초기화 RD:컨트롤 상단 데이타까지 초기화
 
		public static void GroupBoxLock(GroupBox groupBox1, string Lock)
		{
			GroupBoxLock2(groupBox1, Lock);
		}

		private static void GroupBoxLock2(Control ctls, string Lock)
		{// 사용법 : SystemBase.Base.GroupBoxLock( groupBox1 , "T"); // T:락  F:락 해제 R:"초기화" RD:컨트롤 상단 데이타까지 초기화 
			if(Lock == "T")
			{
				foreach(System.Windows.Forms.Control c in ctls.Controls)                           
				{
					if(c.GetType().Name == "TextBox")
					{
						TextBox tb = (TextBox)c;
						tb.BackColor = Color.Gainsboro;
						tb.ReadOnly = true;
					}
					else if(c.GetType().Name == "C1TextBox" || c.GetType().Name == "C1NumericEdit")
					{
						TextBox tb = (TextBox)c;
						tb.BackColor = Color.Gainsboro;
						tb.ReadOnly = true;
					}
					else if(c.GetType().Name == "ComboBox")
					{
						ComboBox cb = (ComboBox)c;
						cb.BackColor = Color.Gainsboro;
						cb.Enabled = false;
					}
					else if(c.GetType().Name == "C1DateEdit")
					{
						TextBox tb = (TextBox)c;
						tb.BackColor = Color.Gainsboro;
						tb.ReadOnly = true;
					}
					else if(c.GetType().Name == "DateTimePicker")
					{
						DateTimePicker dt = (DateTimePicker)c;
						dt.BackColor = Color.Gainsboro;
						dt.Enabled = false;
					}
					else if(c.GetType().Name == "Button")
					{
						Button bt = (Button)c;
						bt.Enabled = false;
					}
					else if(c.GetType().Name == "RadioButton")
					{
						RadioButton rb = (RadioButton)c;
						rb.BackColor = Color.Gainsboro;
						rb.Enabled = false;
					}
					else if(c.GetType().Name == "CheckBox")
					{
						CheckBox rb = (CheckBox)c;
						rb.BackColor = Color.Gainsboro;
						rb.Enabled = false;
					}

					GroupBoxLock2(c, Lock);
			
				}
			}
			else if(Lock == "F")
			{	// 모두 락 해제
				foreach(System.Windows.Forms.Control c in ctls.Controls)                           
				{
					if(c.GetType().Name == "TextBox")
					{
						TextBox tb = (TextBox)c;
						tb.BackColor = Color.White;
						tb.ReadOnly = false;
					}
					else if(c.GetType().Name == "C1TextBox" || c.GetType().Name == "C1NumericEdit")
					{
						TextBox tb = (TextBox)c;
						tb.BackColor = Color.White;
						tb.ReadOnly = false;
					}
					else if(c.GetType().Name == "ComboBox")
					{
						ComboBox cb = (ComboBox)c;
						cb.BackColor = Color.White;
						cb.Enabled = true;
					}
					else if(c.GetType().Name == "C1DateEdit")
					{
						TextBox tb = (TextBox)c;
						tb.BackColor = Color.White;
						tb.ReadOnly = false;
					}
					else if(c.GetType().Name == "DateTimePicker")
					{
						DateTimePicker dt = (DateTimePicker)c;
						dt.BackColor = Color.White;
						dt.Enabled = true;
					}
					else if(c.GetType().Name == "Button")
					{
						Button bt = (Button)c;
						bt.Enabled = true;
					}
					else if(c.GetType().Name == "RadioButton")
					{
						RadioButton rb = (RadioButton)c;
						rb.Enabled = true;
					}
					else if(c.GetType().Name == "CheckBox")
					{
						CheckBox rb = (CheckBox)c;
						rb.Enabled = true;
					}

					GroupBoxLock2(c, Lock);


				}
			}
			else if(Lock == "R")
			{	// 초기화 싱글모드 재정의
				foreach(System.Windows.Forms.Control c in ctls.Controls)                           
				{
					if(c.GetType().Name == "TextBox")
					{
						TextBox tb = (TextBox)c;
						if(tb.Tag != null && tb.Tag.ToString().Length > 0)
						{
							if(tb.Tag.ToString() == "1")
							{	// 필수항목
								tb.BackColor = Color.LightCyan;
								tb.ReadOnly = false;
							}
							else if(tb.Tag.ToString() == "2")
							{	// 읽기전용
								tb.BackColor = Color.Gainsboro;
								tb.ReadOnly = true;
							}
							else if(tb.Tag.ToString() == "3")
							{	// 읽기전용, 필수
								tb.BackColor = Color.LightCyan;
								tb.ReadOnly = true;
							}
							else if(tb.Tag.ToString() == "4")
							{	// 읽기전용, 필수, 회색
								tb.BackColor = Color.Gainsboro;
								tb.ReadOnly = true;
							}
							else
							{
								tb.BackColor = Color.White;
								tb.ReadOnly = false;
							}

							//tb.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;//영문 대문자만 사용시
						}
						else
						{
							tb.BackColor = Color.White;
							tb.ReadOnly = false;
						}
					}
					else if(c.GetType().Name == "C1TextBox" || c.GetType().Name == "C1NumericEdit")
					{
						TextBox tb = (TextBox)c;
						if(tb.Tag != null && tb.Tag.ToString().Length > 0)
						{
							if(tb.Tag.ToString() == "1")
							{	// 필수항목
								tb.BackColor = Color.LightCyan;
								tb.ReadOnly = false;
							}
							else if(tb.Tag.ToString() == "2")
							{	// 읽기전용
								tb.BackColor = Color.Gainsboro;
								tb.ReadOnly = true;
							}
							else if(tb.Tag.ToString() == "3")
							{	// 읽기전용, 필수
								tb.BackColor = Color.LightCyan;
								tb.ReadOnly = true;
							}
							else if(tb.Tag.ToString() == "4")
							{	// 읽기전용, 필수, 회색
								tb.BackColor = Color.Gainsboro;
								tb.ReadOnly = true;
							}
							else
							{
								tb.BackColor = Color.White;
								tb.ReadOnly = false;
							}

							//tb.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;//영문 대문자만 사용시
						}
						else
						{
							tb.BackColor = Color.White;
							tb.ReadOnly = false;
						}
					}
					else if(c.GetType().Name == "ComboBox")
					{
						ComboBox cb = (ComboBox)c;
						if(cb.Tag != null && cb.Tag.ToString().Length > 0)
						{
							if(cb.Tag.ToString() == "1")
							{	// 필수항목
								cb.BackColor = Color.LightCyan;
								cb.Enabled = true;
							}
							else if(cb.Tag.ToString() == "2")
							{	// 읽기전용
								cb.BackColor = Color.Gainsboro;
								cb.Enabled = false;
							}
							else if(cb.Tag.ToString() == "3")
							{	// 읽기전용, 필수
								cb.BackColor = Color.LightCyan;
								cb.Enabled = false;
							}
							else
							{
								cb.BackColor = Color.White;
								cb.Enabled = true;
							}
						}
						else
						{
							cb.BackColor = Color.White;
							cb.Enabled = true;
						}
					}
					else if(c.GetType().Name == "RadioButton")
					{
						RadioButton rb = (RadioButton)c;
						if(rb.Tag != null && rb.Tag.ToString().Length > 0)
						{
							if(rb.Tag.ToString() == "1")
							{	// 필수항목
								rb.BackColor = Color.LightCyan;
								rb.Enabled = true;
							}
							else if(rb.Tag.ToString() == "2")
							{	// 읽기전용
								rb.BackColor = Color.Gainsboro;
								rb.Enabled = false;
							}
							else if(rb.Tag.ToString() == "3")
							{	// 읽기전용, 필수
								rb.BackColor = Color.LightCyan;
								rb.Enabled = false;
							}
							else
							{
								rb.BackColor = Color.White;
								rb.Enabled = true;
							}
						}
						else
						{
							rb.BackColor = Color.White;
							rb.Enabled = true;
						}
					}
					else if(c.GetType().Name == "CheckBox")
					{
						CheckBox rb = (CheckBox)c;
						if(rb.Tag != null && rb.Tag.ToString().Length > 0)
						{
							if(rb.Tag.ToString() == "1")
							{	// 필수항목
								rb.BackColor = Color.LightCyan;
								rb.Enabled = true;
							}
							else if(rb.Tag.ToString() == "2")
							{	// 읽기전용
								rb.BackColor = Color.Gainsboro;
								rb.Enabled = false;
							}
							else if(rb.Tag.ToString() == "3")
							{	// 읽기전용, 필수
								rb.BackColor = Color.LightCyan;
								rb.Enabled = false;
							}
							else
							{
								rb.BackColor = Color.White;
								rb.Enabled = true;
							}
						}
						else
						{
							rb.BackColor = Color.White;
							rb.Enabled = true;
						}
					}
					else if(c.GetType().Name == "C1DateEdit")
					{
						TextBox tb = (TextBox)c;
						if(tb.Tag != null && tb.Tag.ToString().Length > 0)
						{
							if(tb.Tag.ToString() == "1")
							{	// 필수항목
								tb.BackColor = Color.LightCyan;
								tb.ReadOnly = false;
							}
							else if(tb.Tag.ToString() == "2")
							{	// 읽기전용
								tb.BackColor = Color.Gainsboro;
								tb.ReadOnly = true;
							}
							else if(tb.Tag.ToString() == "3")
							{	// 읽기전용, 필수
								tb.BackColor = Color.LightCyan;
								tb.ReadOnly = true;
							}
							else if(tb.Tag.ToString() == "4")
							{	// 읽기전용, 필수, 회색
								tb.BackColor = Color.Gainsboro;
								tb.ReadOnly = true;
							}
							else
							{
								tb.BackColor = Color.White;
								tb.ReadOnly = false;
							}
						}
						else
						{
							tb.BackColor = Color.White;
							tb.ReadOnly = false;
						}
					}
					else if(c.GetType().Name == "DateTimePicker")
					{
						DateTimePicker dt = (DateTimePicker)c;

						if(dt.Tag != null && dt.Tag.ToString().Length > 0)
						{
							if(dt.Tag.ToString() == "1")
							{	// 필수항목
								dt.BackColor = Color.LightCyan;
								dt.Enabled = true;
							}
							else if(dt.Tag.ToString() == "2")
							{	// 읽기전용
								dt.BackColor = Color.Gainsboro;
								dt.Enabled = false;
							}
							else if(dt.Tag.ToString() == "3")
							{	// 읽기전용, 필수
								dt.BackColor = Color.LightCyan;
								dt.Enabled = false;
							}
							else if(dt.Tag.ToString() == "4")
							{	// 읽기전용, 필수, 회색
								dt.BackColor = Color.Gainsboro;
								dt.Enabled = false;
							}
							else
							{
								dt.BackColor = Color.White;
								dt.Enabled = true;
							}
						}
						else
						{
							dt.BackColor = Color.White;
							dt.Enabled = true;
						}
					}
					else if(c.GetType().Name == "Button")
					{
						Button bt = (Button)c;
						bt.Enabled = true;
					}

					GroupBoxLock2(c, Lock);

				}
			}
			else if(Lock == "RD")
			{	// 초기화 (데이타, 컨트롤)
				foreach(System.Windows.Forms.Control c in ctls.Controls)                           
				{
					if(c.GetType().Name == "TextBox")
					{
						TextBox tb = (TextBox)c;
						if(tb.Tag != null && tb.Tag.ToString().Length > 0)
						{
							if(tb.Tag.ToString() == "1")
							{	// 필수항목
								tb.BackColor = Color.LightCyan;
								tb.ReadOnly = false;
							}
							else if(tb.Tag.ToString() == "2")
							{	// 읽기전용
								tb.BackColor = Color.Gainsboro;
								tb.ReadOnly = true;
							}
							else if(tb.Tag.ToString() == "3")
							{	// 읽기전용, 필수
								tb.BackColor = Color.LightCyan;
								tb.ReadOnly = true;
							}
							else if(tb.Tag.ToString() == "4")
							{	// 읽기전용, 필수, 회색
								tb.BackColor = Color.Gainsboro;
								tb.ReadOnly = true;
							}
							else
							{
								tb.BackColor = Color.White;
								tb.ReadOnly = false;
							}

							//tb.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;//영문 대문자만 사용시
						}
						else
						{
							tb.BackColor = Color.White;
							tb.ReadOnly = false;
						}
						tb.Text = "";
					}
					else if(c.GetType().Name == "C1TextBox" || c.GetType().Name == "C1NumericEdit")
					{
						TextBox tb = (TextBox)c;
						if(tb.Tag != null && tb.Tag.ToString().Length > 0)
						{
							if(tb.Tag.ToString() == "1")
							{	// 필수항목
								tb.BackColor = Color.LightCyan;
								tb.ReadOnly = false;
								tb.Text = "0";

							}
							else if(tb.Tag.ToString() == "2")
							{	// 읽기전용
								tb.BackColor = Color.Gainsboro;
								tb.ReadOnly = false;
								tb.Text = "0";
								tb.ReadOnly = true;
							}
							else if(tb.Tag.ToString() == "3")
							{	// 읽기전용, 필수
								tb.BackColor = Color.LightCyan;
								tb.ReadOnly = false;
								tb.Text = "0";
								tb.ReadOnly = true;
							}
							else if(tb.Tag.ToString() == "4")
							{	// 읽기전용, 필수, 회색
								tb.BackColor = Color.Gainsboro;
								tb.ReadOnly = false;
								tb.Text = "0";
								tb.ReadOnly = true;
							}
							else
							{
								tb.BackColor = Color.White;
								tb.Text = "0";
								tb.ReadOnly = false;
							}


							//tb.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;//영문 대문자만 사용시
						}
						else
						{
							tb.BackColor = Color.White;
							tb.Text = "0";
							tb.ReadOnly = false;
						}

					}
					else if(c.GetType().Name == "ComboBox")
					{
						ComboBox cb = (ComboBox)c;
						if(cb.Tag != null && cb.Tag.ToString().Length > 0)
						{
							if(cb.Tag.ToString() == "1")
							{	// 필수항목
								cb.BackColor = Color.LightCyan;
								cb.Enabled = true;
							}
							else if(cb.Tag.ToString() == "2")
							{	// 읽기전용
								cb.BackColor = Color.Gainsboro;
								cb.Enabled = false;
							}
							else if(cb.Tag.ToString() == "3")
							{	// 읽기전용, 필수
								cb.BackColor = Color.LightCyan;
								cb.Enabled = false;
							}
							else
							{
								cb.BackColor = Color.White;
								cb.Enabled = true;
							}
						}
						else
						{
							cb.BackColor = Color.White;
							cb.Enabled = true;
						}
						cb.SelectedIndex = 0;
					}
					else if(c.GetType().Name == "RadioButton")
					{
						RadioButton rb = (RadioButton)c;
						if(rb.Tag != null && rb.Tag.ToString().Length > 0)
						{
							if(rb.Tag.ToString() == "1")
							{	// 필수항목
								rb.BackColor = Color.LightCyan;
								rb.Enabled = true;
							}
							else if(rb.Tag.ToString() == "2")
							{	// 읽기전용
								rb.BackColor = Color.Gainsboro;
								rb.Enabled = false;
							}
							else if(rb.Tag.ToString() == "3")
							{	// 읽기전용, 필수
								rb.BackColor = Color.LightCyan;
								rb.Enabled = false;
							}
							else
							{
								rb.BackColor = Color.White;
								rb.Enabled = true;
							}
						}
						else
						{
							rb.BackColor = Color.White;
							rb.Enabled = true;
						}
					}
					else if(c.GetType().Name == "CheckBox")
					{
						CheckBox rb = (CheckBox)c;
						if(rb.Tag != null && rb.Tag.ToString().Length > 0)
						{
							if(rb.Tag.ToString() == "1")
							{	// 필수항목
								rb.BackColor = Color.LightCyan;
								rb.Enabled = true;
							}
							else if(rb.Tag.ToString() == "2")
							{	// 읽기전용
								rb.BackColor = Color.Gainsboro;
								rb.Enabled = false;
							}
							else if(rb.Tag.ToString() == "3")
							{	// 읽기전용, 필수
								rb.BackColor = Color.LightCyan;
								rb.Enabled = false;
							}
							else
							{
								rb.BackColor = Color.White;
								rb.Enabled = true;
							}
						}
						else
						{
							rb.BackColor = Color.White;
							rb.Enabled = true;
						}
					}
					else if(c.GetType().Name == "C1DateEdit")
					{
						TextBox tb = (TextBox)c;
						if(tb.Tag != null && tb.Tag.ToString().Length > 0)
						{
							if(tb.Tag.ToString() == "1")
							{	// 필수항목
								tb.BackColor = Color.LightCyan;
								tb.ReadOnly = false;
							}
							else if(tb.Tag.ToString() == "2")
							{	// 읽기전용
								tb.BackColor = Color.Gainsboro;
								tb.ReadOnly = true;
							}
							else if(tb.Tag.ToString() == "3")
							{	// 읽기전용, 필수
								tb.BackColor = Color.LightCyan;
								tb.ReadOnly = true;
							}
							else if(tb.Tag.ToString() == "4")
							{	// 읽기전용, 필수, 회색
								tb.BackColor = Color.Gainsboro;
								tb.ReadOnly = true;
							}
							else
							{
								tb.BackColor = Color.White;
								tb.ReadOnly = false;
							}
						}
						else
						{
							tb.BackColor = Color.White;
							tb.ReadOnly = false;
						}
						tb.Text = "";
					}
					else if(c.GetType().Name == "DateTimePicker")
					{
						DateTimePicker dt = (DateTimePicker)c;

						if(dt.Tag != null && dt.Tag.ToString().Length > 0)
						{
							if(dt.Tag.ToString() == "1")
							{	// 필수항목
								dt.BackColor = Color.LightCyan;
								dt.Enabled = true;
							}
							else if(dt.Tag.ToString() == "2")
							{	// 읽기전용
								dt.BackColor = Color.Gainsboro;
								dt.Enabled = false;
							}
							else if(dt.Tag.ToString() == "3")
							{	// 읽기전용, 필수
								dt.BackColor = Color.LightCyan;
								dt.Enabled = false;
							}
							else if(dt.Tag.ToString() == "4")
							{	// 읽기전용, 필수, 회색
								dt.BackColor = Color.Gainsboro;
								dt.Enabled = false;
							}
							else
							{
								dt.BackColor = Color.White;
								dt.Enabled = true;
							}
						}
						else
						{
							dt.BackColor = Color.White;
							dt.Enabled = true;
						}
						dt.Text = "";
					}
					else if(c.GetType().Name == "Button")
					{
						Button bt = (Button)c;
						bt.Enabled = true;
					}

					GroupBoxLock2(c, Lock);

				}
			}
		}

		#endregion

		#region PanelLock 싱글모드 입력창 전체 Lock

		public static void PanelLock(Panel panel1)
		{
			PanelLock2(panel1);
		}

		private static void PanelLock2(Control ctls)
		{
			foreach(System.Windows.Forms.Control c in ctls.Controls)                           
			{
				if(c.GetType().Name == "TextBox")
				{
					TextBox tb = (TextBox)c;
					tb.BackColor = Color.Gainsboro;
					tb.ReadOnly = true;
				}
				else if(c.GetType().Name == "C1TextBox" || c.GetType().Name == "C1NumericEdit")
				{
					TextBox tb = (TextBox)c;
					tb.BackColor = Color.Gainsboro;
					tb.ReadOnly = true;
				}
				else if(c.GetType().Name == "ComboBox")
				{
					ComboBox cb = (ComboBox)c;
					cb.BackColor = Color.Gainsboro;
					cb.Enabled = false;
				}
				else if(c.GetType().Name == "C1DateEdit")
				{
					TextBox tb = (TextBox)c;
					tb.BackColor = Color.Gainsboro;
					tb.ReadOnly = true;
				}
				else if(c.GetType().Name == "DateTimePicker")
				{
					DateTimePicker dt = (DateTimePicker)c;
					dt.BackColor = Color.Gainsboro;
					dt.Enabled = false;
				}
				else if(c.GetType().Name == "Button")
				{
					Button bt = (Button)c;
					bt.Enabled = false;
				}
				else if(c.GetType().Name == "RadioButton")
				{
					RadioButton rb = (RadioButton)c;
					rb.BackColor = Color.Gainsboro;
					rb.Enabled = false;
				}
				else if(c.GetType().Name == "CheckBox")
				{
					CheckBox rb = (CheckBox)c;
					rb.BackColor = Color.Gainsboro;
					rb.Enabled = false;
				}

				//User Contorl 일경우엔 재귀호출
				//if(Convert.ToString(c.Tag) == "UC")
					PanelLock2(c);

			}
		}
		#endregion

		#region 재귀호출
		public static DataTable SelfCall(DataTable dt, string S_ID, string M_ID, string P_ID, string[] GROUP)
		{// 사용법 : SystemBase.Base.SelfCall(dt, "1", "MY_ID", "PARENT_ID", new string[]{});
			DataTable dtRtn = new DataTable();	//Return할 DataTable

			ArrayList keyColumns = new ArrayList();

			string[] ColumnName = new string[dt.Columns.Count+1];	//Column명 저장변수
			for(int i=0; i < dt.Columns.Count; i++)
			{
				ColumnName[i] = dt.Columns[i].ColumnName;	//Column명 배열에 저장
				if((ColumnName[i] == M_ID) || (ColumnName[i] == P_ID))
				{
					dtRtn.Columns.Add(dt.Columns[i].ColumnName, typeof(string));//Column 생성
				}
				else
				{
					dtRtn.Columns.Add(dt.Columns[i].ColumnName, dt.Columns[i].DataType);//Column 생성
				}
			}
			dtRtn.Columns.Add("TMP_YN", typeof(string));
			
			DataColumn key = dtRtn.Columns.Add("TMP_SEQ", typeof(string));//col.DataType
			keyColumns.Add(key);

			UniqueConstraint unique = new UniqueConstraint((DataColumn[])keyColumns.ToArray(typeof(DataColumn)), true);
			dtRtn.Constraints.Add(unique);

			string SEQ = "";

			int g = 0;
			string TMPSEQ1 = "";
			foreach (DataRow drTemp in dt.Select(M_ID +" = '"+S_ID + "' "))
			{
				g++;

				if(Convert.ToString(g).Length == 1){TMPSEQ1 = "00" + Convert.ToString(g);}
				else if(Convert.ToString(g).Length == 2){TMPSEQ1 = "0" + Convert.ToString(g);}
				else{TMPSEQ1 = Convert.ToString(g);}

				DataRow dr = dtRtn.NewRow();
				for(int i=0; i < dt.Columns.Count; i++)
				{
					dr[ColumnName[i]] = drTemp[ColumnName[i]];//최초 시작 데이타 저장
				}
				dr["TMP_YN"] = "Y";

				dr["TMP_SEQ"] = TMPSEQ1;
				SEQ = TMPSEQ1;
				dtRtn.Rows.Add(dr);
			}
			DataTable dtRtnTmp = SystemBase.Base.SelfCallDetail(dt, dtRtn, S_ID, SEQ, M_ID, P_ID, ColumnName);

			DataTable dtRtnOR = TMPSEQ(dtRtnTmp, keyColumns);

			return dtRtnOR;
		}


		public static DataTable SelfCallDetail(DataTable dt, DataTable dtRtn, string S_ID, string SEQ, string M_ID, string P_ID, string[] ColumnName)
		{
			int k = 0;
			foreach (DataRow drTemp in dt.Select(P_ID +" = '"+ S_ID +"'" ))
			{
				if(drTemp[M_ID].ToString() !=  S_ID.ToString() )
				{
					k++;
					DataRow dr = dtRtn.NewRow();
					for(int i=0; i < dt.Columns.Count; i++)
					{
						dr[ColumnName[i]]	= drTemp[ColumnName[i]];
					}
					dr["TMP_YN"] = "N";
				
					string TMPSEQ1, TMPSEQ2 = "";

					if(SEQ.Length == 1){TMPSEQ1 = "00" + SEQ;}
					else if(SEQ.Length == 2){TMPSEQ1 = "0" + SEQ;}
					else{TMPSEQ1 = SEQ;}

					if(Convert.ToString(k).Length == 1){TMPSEQ2 = "00" + Convert.ToString(k);}
					else if(Convert.ToString(k).Length == 2){TMPSEQ2 = "0" + Convert.ToString(k);}
					else{TMPSEQ2 = Convert.ToString(k);}

					dr["TMP_SEQ"] = TMPSEQ1 + TMPSEQ2;

					dtRtn.Rows.Add(dr);
				}
			}

			if(dtRtn.Select(" TMP_YN = 'N' ").Length > 0)
			{
				foreach (DataRow drTemp  in dtRtn.Select(" TMP_YN = 'N' "))
				{
					if( dtRtn.Select( M_ID+ " = '"+ drTemp[M_ID] +"' AND TMP_YN = 'Y' ").Length == 0 && S_ID.ToString() != drTemp[M_ID].ToString())
					{
						drTemp["TMP_YN"] = "Y";
						S_ID = drTemp[M_ID].ToString();
						SEQ = drTemp["TMP_SEQ"].ToString();
						SystemBase.Base.SelfCallDetail(dt, dtRtn, S_ID, SEQ, M_ID, P_ID, ColumnName);
					}
				}
			}

			return dtRtn;
		}


		public static DataTable SelfCall(DataTable dt, string S_ID, string M_ID, string P_ID)
		{	// 사용법 : SystemBase.Base.SelfCall(dt, "1", "MY_ID", "PARENT_ID");//재귀호출 부모ID로 재귀호출
			DataTable dtRtn = new DataTable();	//Return할 DataTable

			ArrayList keyColumns = new ArrayList();

			string[] ColumnName = new string[dt.Columns.Count+1];	//Column명 저장변수
			for(int i=0; i < dt.Columns.Count; i++)
			{
				ColumnName[i] = dt.Columns[i].ColumnName;	//Column명 배열에 저장
				if((ColumnName[i] == M_ID) || (ColumnName[i] == P_ID))
				{
					dtRtn.Columns.Add(dt.Columns[i].ColumnName, typeof(string));//Column 생성
				}
				else
				{
					dtRtn.Columns.Add(dt.Columns[i].ColumnName, dt.Columns[i].DataType);//Column 생성
				}
			}
			dtRtn.Columns.Add("TMP_YN", typeof(string));
			
			DataColumn key = dtRtn.Columns.Add("TMP_SEQ", typeof(string));//col.DataType
			keyColumns.Add(key);

			UniqueConstraint unique = new UniqueConstraint((DataColumn[])keyColumns.ToArray(typeof(DataColumn)), true);
			dtRtn.Constraints.Add(unique);

			string SEQ = "";

			int g = 0;
			string TMPSEQ1 = "";
			foreach (DataRow drTemp in dt.Select(P_ID +" = '"+S_ID + "' " ))
			{
				g++;

				if(Convert.ToString(g).Length == 1){TMPSEQ1 = "00" + Convert.ToString(g);}
				else if(Convert.ToString(g).Length == 2){TMPSEQ1 = "0" + Convert.ToString(g);}
				else{TMPSEQ1 = Convert.ToString(g);}

				DataRow dr = dtRtn.NewRow();
				for(int i=0; i < dt.Columns.Count; i++)
				{
					dr[ColumnName[i]] = drTemp[ColumnName[i]];//최초 시작 데이타 저장
				}
				dr["TMP_YN"] = "N";

				dr["TMP_SEQ"] = TMPSEQ1;
				SEQ = TMPSEQ1;
				dtRtn.Rows.Add(dr);
			}
			//S_ID = dtRtn.Rows[0][M_ID].ToString();
			DataTable dtRtnTmp = SystemBase.Base.SelfCallDetail2(dt, dtRtn, S_ID, SEQ, M_ID, P_ID, ColumnName);
			DataTable dtRtnOR = TMPSEQ(dtRtnTmp, keyColumns);

			return dtRtnOR;
		}

		public static DataTable SelfCallDetail2(DataTable dt, DataTable dtRtn, string S_ID, string SEQ, string M_ID, string P_ID, string[] ColumnName)
		{
			//int k = 0;
/*
			foreach (DataRow drTemp in dt.Select(P_ID +" = '"+ S_ID +"'" ))
			{
				if(drTemp[M_ID].ToString() !=  S_ID.ToString() )
				{
					k++;
					DataRow dr = dtRtn.NewRow();
					for(int i=0; i < dt.Columns.Count; i++)
					{
						dr[ColumnName[i]]	= drTemp[ColumnName[i]];
					}
					dr["TMP_YN"] = "N";
				
					string TMPSEQ1, TMPSEQ2 = "";

					if(SEQ.Length == 1){TMPSEQ1 = "00" + SEQ;}
					else if(SEQ.Length == 2){TMPSEQ1 = "0" + SEQ;}
					else{TMPSEQ1 = SEQ;}

					if(Convert.ToString(k).Length == 1){TMPSEQ2 = "00" + Convert.ToString(k);}
					else if(Convert.ToString(k).Length == 2){TMPSEQ2 = "0" + Convert.ToString(k);}
					else{TMPSEQ2 = Convert.ToString(k);}

					dr["TMP_SEQ"] = TMPSEQ1 + TMPSEQ2;

					dtRtn.Rows.Add(dr);
				}
			}
*/
			if(dtRtn.Select(" TMP_YN = 'N' ").Length > 0)
			{
				foreach (DataRow drTemp  in dtRtn.Select(" TMP_YN = 'N' "))
				{
					if( dtRtn.Select( P_ID+ " = '"+ drTemp[M_ID] +"' AND TMP_YN = 'Y' ").Length == 0 && S_ID.ToString() != drTemp[M_ID].ToString())
					{
						drTemp["TMP_YN"] = "Y";
						S_ID = drTemp[M_ID].ToString();
						SEQ = drTemp["TMP_SEQ"].ToString();
						SystemBase.Base.SelfCallDetail(dt, dtRtn, S_ID, SEQ, M_ID, P_ID, ColumnName);
					}
				}
			}

			return dtRtn;
		}


		public static DataTable TMPSEQ(DataTable dt, ArrayList keyColumn)
		{
			DataView dv = new DataView(dt);
			dv.Sort = " TMP_SEQ ASC ";

			DataTable tdt = new DataTable("TmpTable2");
			DataView tdv = new DataView(tdt);

			ArrayList keyColumns = new ArrayList();
			foreach (DataColumn col in dt.Columns)
			{
				string colName = col.ColumnName;
				bool KeyChk = true;
				for(int k = 0; k < keyColumn.Count; k++)
				{
					if(keyColumn[k].ToString() == colName)
					{
						DataColumn key = tdt.Columns.Add(colName, col.DataType);//col.DataType
						keyColumns.Add(key);

						KeyChk = false;
					}
				}
				if(KeyChk)
				{
					tdt.Columns.Add(colName, col.DataType);//col.DataType
				}
			}

			UniqueConstraint unique = new UniqueConstraint(
				(DataColumn[])keyColumns.ToArray(typeof(DataColumn)), true);
			tdt.Constraints.Add(unique);

			Hashtable pivotNames = new Hashtable();
			foreach (DataRowView drv in dv)
			{
				object[] keys = new object[keyColumns.Count];

				for (int i = 0; i < keyColumns.Count; i++)
				{
					string name = ((DataColumn)keyColumns[i]).ColumnName;
					keys[i] = drv[name];
				}
				DataRow drp = tdt.Rows.Find(keys);
				
				if (drp == null)
				{
					drp = tdt.NewRow();
					foreach (DataColumn col in dt.Columns)
					{
						string name = col.ColumnName;
						drp[name] = drv[name];	// 키의 칼럼에 Row 데이타 담기
					}
					tdt.Rows.Add(drp);
				}
			}
			/**************************************************************************/
			
			DataTable dtRtnTmp = new DataTable();	//Return할 DataTable

			string[] ColumnName = new string[tdt.Columns.Count-2];	//Column명 저장변수
			for(int i=0; i < tdt.Columns.Count-2; i++)
			{
				ColumnName[i] = tdt.Columns[i].ColumnName;	//Column명 배열에 저장
				dtRtnTmp.Columns.Add(tdt.Columns[i].ColumnName, tdt.Columns[i].DataType);//Column 생성
			}

			DataView dv2 = new DataView(tdt);
			foreach (DataRowView drv in dv2)
			{
				DataRow drp = dtRtnTmp.NewRow();

				for(int k = 0; k < ColumnName.Length; k++)	
				{
					drp[ColumnName[k]] = drv[ColumnName[k]];	// 키의 칼럼에 Row 데이타 담기
				}
				dtRtnTmp.Rows.Add(drp);

			}
			return dtRtnTmp;
		}

		#endregion

		#region SVTM 서버시간 호출  SVTM 서버시간 호출	YYMMDD : 2007-12-26  YMD : 20071226  "" : 2007-12-26 16:43:55
		public static string ServerTime(string Kind)
		{// 사용법 : SystemBase.Base.ServerTime("YMD");
			string RtnMsg = "";
			string Query = " usp_TIME '"+ Kind +"' ";
			DataTable DT = SystemBase.DbOpen.NoTranDataTable(Query);

			if(DT.Rows.Count > 0)
				RtnMsg = DT.Rows[0][0].ToString();

			return RtnMsg;
		}
		#endregion

		#region 사사오입
		public static double MyRound(double value, int digits) 
		{	//사용법 : MyRound(Convert.ToDouble(textBox1.Text), 1);
			int sign = Math.Sign(value);
			double scale = Math.Pow(10.0, digits);
			double round = Math.Floor(Math.Abs(value) * scale + 0.5);
			return (sign * round / scale);
		}
		#endregion

        #region GridHeadIndex - 그리드 Head Index Return
        public static int GridHeadIndex(string[,] GHIdx, string HeadName)
        {	// fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "TRV NO")].Text
            int RtnGridHeadIndex = 0;
            for (int i = 0; i < GHIdx.Length / 2; i++)
            {
                if (GHIdx[i, 0].Trim().ToUpper() == HeadName.Trim().ToUpper())
                {
                    RtnGridHeadIndex = Convert.ToInt32(GHIdx[i, 1].Trim());
                    return RtnGridHeadIndex;
                }
            }
            return RtnGridHeadIndex;
        }

        public static int GridHeadIndex2(FarPoint.Win.Spread.FpSpread Grid, string HeadName)
        {
            int RtnGridHeadIndex = 0;
            for (int i = 0; i < Grid.Sheets[0].Columns.Count; i++)
            {
                if (HeadName.Trim().ToUpper().LastIndexOf("_") > 0 && Grid.Sheets[0].ColumnHeader.Cells[1, i].Text.ToUpper() == HeadName.Trim().ToUpper().Substring(0, HeadName.Trim().ToUpper().LastIndexOf("_")))
                {
                    RtnGridHeadIndex = i + 1;
                    return RtnGridHeadIndex;
                }
                else if (Grid.Sheets[0].ColumnHeader.Cells[1, i].Text.ToUpper() == HeadName.Trim().ToUpper())
                {
                    RtnGridHeadIndex = i;
                    return RtnGridHeadIndex;
                }
            }

            return RtnGridHeadIndex;
        }

        public static int GridHeadIndex3(FarPoint.Win.Spread.FpSpread Grid, string HeadName)
        {
            int RtnGridHeadIndex = 0;
            for (int i = 0; i < Grid.Sheets[0].Columns.Count; i++)
            {
                if (HeadName.Trim().ToUpper().LastIndexOf("_") > 0 && Grid.Sheets[0].ColumnHeader.Cells[2, i].Text.ToUpper() == HeadName.Trim().ToUpper().Substring(0, HeadName.Trim().ToUpper().LastIndexOf("_")))
                {
                    RtnGridHeadIndex = i + 1;
                    return RtnGridHeadIndex;
                }
                else if (Grid.Sheets[0].ColumnHeader.Cells[2, i].Text.ToUpper() == HeadName.Trim().ToUpper())
                {

                    RtnGridHeadIndex = i;
                    return RtnGridHeadIndex;
                }
            }

            return RtnGridHeadIndex;
        }

        #endregion

		#region GTP 그리드 세팅
        //public static void GantGridSet(object BaseGantt, int MaxCol, string MinDt, string MaxDt, double StDt, double EdDt, string[] Title, string [] CellType,  int[] Width, string[] HorzAlign, string FontName, float FontSize)
        //{

        //}

		/// <summary>
		/// GTP그리드세팅
		/// </summary>
		/// <param name="BaseGantt">Gantt1</param>
		/// <param name="MaxCol">총Col수</param>
		/// <param name="MinDt">Dafault로 보여줄 최소일자</param>
		/// <param name="MaxDt">Dafault로 보여줄 최대일자</param>
		/// <param name="StDt">Dafault로 보여줄 현재날짜(+-) 최소일자, double형식 DateTime.Add() 괄호안의 값임</param>
		/// <param name="EdDt">Dafault로 보여줄 현재날짜(+-) 최대일자, double형식 DateTime.Add() 괄호안의 값임</param>
		/// <param name="Title">헤더제목, string 배열형식</param>
		/// <param name="CellType">Col속성, string 배열형식</param>
		/// <param name="Width">Col넓이, int 배열형식</param>
		/// <param name="HorzAlign">데이터정렬, string 배열형식(L,C,R)</param>
		/// <param name="FontName">폰트 string형식</param>
		/// <param name="FontSize">폰트사이즈 float형식</param>
		public static void GantGridSet(PlexityHide.GTP.Gantt BaseGantt, int MaxCol, string MinDt, string MaxDt, double StDt, double EdDt, string[] Title, string [] CellType,  int[] Width, string[] HorzAlign, string FontName, float FontSize)
		{
			//line, 날짜 셋팅
			BaseGantt.GridProperties.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;				// BorderStyle
			BaseGantt.DateScaler.CultureInfoDateTimeFormat = new CultureInfo("ko-KR", false ).DateTimeFormat;	// 날짜 포맷
			BaseGantt.DateScalerProperties.LowerBound = Convert.ToDateTime(MinDt);								// 최소일자
			BaseGantt.DateScalerProperties.UpperBound = Convert.ToDateTime(MaxDt);								// 최대일자
			BaseGantt.DateScalerProperties.ShowWeekNumbers = true;												
			BaseGantt.VerticalDayStripes = true;
			BaseGantt.TodayLine = true;
			BaseGantt.DateScalerProperties.StartTime = DateTime.Now.AddDays(StDt);								// 차트 헤드 디자인 시작일자
			BaseGantt.DateScalerProperties.StopTime = DateTime.Now.AddDays(EdDt);								// 차트 헤드 디자인 종료일자
			BaseGantt.Grid.GridStructure.RowSelect = true;														//Single Select
			
			//그리드 레이아웃 세팅
			//default center정렬
			PlexityHide.GTP.CellLayout BaseLayout = new PlexityHide.GTP.CellLayout();
			BaseLayout.Name = "New";
			BaseLayout.Font = new System.Drawing.Font(FontName,FontSize);
			BaseLayout.HeaderBackgroundColor = Color.WhiteSmoke;
			BaseLayout.HeaderBackgroundGradiantColor = Color.WhiteSmoke;
			BaseLayout.HorzAlign = System.Drawing.StringAlignment.Center;
			BaseLayout.VertAlign = System.Drawing.StringAlignment.Center;
			BaseLayout.HeaderBackgroundColor = Color.LightGoldenrodYellow;
			//left 정렬
			PlexityHide.GTP.CellLayout LeftLayout = new PlexityHide.GTP.CellLayout();
			LeftLayout.Name = "Left";
			LeftLayout.Font = new System.Drawing.Font(FontName, FontSize);
			LeftLayout.HeaderBackgroundColor = Color.WhiteSmoke;
			LeftLayout.HeaderBackgroundGradiantColor = Color.WhiteSmoke;
			LeftLayout.HorzAlign = System.Drawing.StringAlignment.Near;
			LeftLayout.VertAlign = System.Drawing.StringAlignment.Center;
			LeftLayout.HeaderBackgroundColor = Color.LightGoldenrodYellow;
			//right 정렬
			PlexityHide.GTP.CellLayout RightLayout = new PlexityHide.GTP.CellLayout();
			RightLayout.Name = "Right";
			RightLayout.Font = new System.Drawing.Font(FontName, FontSize);
			RightLayout.HeaderBackgroundColor = Color.WhiteSmoke;
			RightLayout.HeaderBackgroundGradiantColor = Color.WhiteSmoke;
			RightLayout.HorzAlign = System.Drawing.StringAlignment.Far;
			RightLayout.VertAlign = System.Drawing.StringAlignment.Center;
			RightLayout.HeaderBackgroundColor = Color.LightGoldenrodYellow;

			int iWidth = 30;	//그리드 여백

			for (int i = 0; i < MaxCol; i++)
			{	
				switch(CellType[i])
				{
					case "MT" :
						BaseGantt.GridProperties.Columns.InsertNew(i,PlexityHide.GTP.CellType.MultiText);
						break;
					case "BC" :
						BaseGantt.GridProperties.Columns.InsertNew(i,PlexityHide.GTP.CellType.BoolCheck);
						break;
					case "CT" :
						BaseGantt.GridProperties.Columns.InsertNew(i,PlexityHide.GTP.CellType.ComboText);
						break;
					case "TD" :
						BaseGantt.GridProperties.Columns.InsertNew(i,PlexityHide.GTP.CellType.TimeDate);
						break;
					case "TS" :
						BaseGantt.GridProperties.Columns.InsertNew(i,PlexityHide.GTP.CellType.TimeSpan);
						break;
					case "CC" :
						BaseGantt.GridProperties.Columns.InsertNew(i,PlexityHide.GTP.CellType.CustomCell);
						break;
					default :
						BaseGantt.GridProperties.Columns.InsertNew(i,PlexityHide.GTP.CellType.SingleText);
						break;
				}

				switch(HorzAlign[i])
				{
					case "C" :
						BaseGantt.GridProperties.Columns[i].LayoutName = "New";
						break;
					case "R" :
						BaseGantt.GridProperties.Columns[i].LayoutName = "Right";
						break;
					default :
						BaseGantt.GridProperties.Columns[i].LayoutName = "Left";
						break;
				}
				BaseGantt.GridProperties.Columns[i].Title = Title[i].ToString();
				BaseGantt.GridProperties.Columns[i].Width = Width[i];
				BaseGantt.GridProperties.Columns[i].ReadOnly = true;

				BaseGantt.GridProperties.CellLayouts.Add(BaseLayout);
				BaseGantt.GridProperties.CellLayouts.Add(LeftLayout);
				BaseGantt.GridProperties.CellLayouts.Add(RightLayout);

				iWidth += Width[i];
			}
				
			BaseGantt.GridWidth = iWidth;
        }
		#endregion

		#region 시수계산
		public static int TimeCheck(string StartTm, string EndTm, int DayCount)
		{
			int Time = 0;
			string Time_T = "";		//시
			string Time_M = "";		//분
			int Time_S = 0;			//시작시수
			int Time_E = 0;			//종료시수

			try
			{
				if(StartTm != "" && EndTm != "")
				{
					//시작시간, 종료시간이 같으면 0이다
					if(DayCount == 0 && StartTm == EndTm)
					{
						Time = 0;
					}
					else
					{
						Time_T = StartTm.Substring(0,2);
						Time_M = StartTm.Substring(2,2);
						Time_S = Convert.ToInt32(Time_T) * 60 + Convert.ToInt32(Time_M);

						Time_T = EndTm.Substring(0,2);
						Time_M = EndTm.Substring(2,2);
						Time_E = Convert.ToInt32(Time_T) * 60 + Convert.ToInt32(Time_M);

						Time = (DayCount * 1440) + Time_E - Time_S;
					}
				}
			}
			catch(Exception f)
			{
				Loggers.Log("TimeCheck", f.Message);
				MessageBox.Show(SystemBase.Base.MessageRtn("SY008","시수계산"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}

			return Time;
		}
		#endregion

		#region 라우팅번호 찾기
		public static string RoutingNo(string Type)
		{
			string RoutNo = "";

			string Query = "SELECT MINOR_CODE FROM B_COMM_CODE(NOLOCK) WHERE COMM_CODE = '"+ SystemBase.Base.gstrCOMCD.ToString() +"' AND MAJOR_CODE = 'P018' AND REL_CD1 = '"+ Type +"' ";
			DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);

			if(dt.Rows.Count > 0)
			{
				RoutNo = dt.Rows[0][0].ToString();
			}
			else
			{
				RoutNo = "";
			}

			return RoutNo;
		}
		#endregion

		#region 폼로드
		
		public static void MDIFORM(string MdiFM, string MenuName, string MenuID, string[] fieldName, string[] fieldValue)
		{	// 다른 프로젝트 폼 로딩
			try
			{
				if(MdiFM.Length > 2)
				{
					string TMdiFM = MdiFM.ToString();
					// 구분자 . 가 마지막뒤에있는것이 클래스명이며, 앞에것은 모두 네임스페이스로 간주한다.
					//						string NamespaceName	= TMdiFM.Substring(0, TMdiFM.IndexOf(".",0, TMdiFM.Length-1) );
					string NamespaceName	= TMdiFM.Substring(0, TMdiFM.LastIndexOf("."));
					//						string RodeFormName		= TMdiFM.Substring(TMdiFM.IndexOf(".",0, TMdiFM.Length)+1, TMdiFM.Length-TMdiFM.IndexOf(".",0, TMdiFM.Length)-1 );
					string RodeFormName		= TMdiFM.Substring(TMdiFM.LastIndexOf(".") + 1, TMdiFM.Length - TMdiFM.LastIndexOf(".") -1);

					SystemBase.Base.RodeFormName	= TMdiFM.ToString();
					SystemBase.Base.RodeFormID		= MenuID.ToString();
					SystemBase.Base.RodeFormText	= SystemBase.Base.CodeName ("MENU_ID","MENU_NAME","CO_SYS_MENU", MenuID.ToString(),"");
					BaseForm.GetType().GetProperty("StatusBarText").SetValue(BaseForm, MenuID.ToString(), null);

					bool mdiwin = false;
					Form tempForm = null;
					for(int i=0; i < BaseForm.MdiChildren.Length; i++)
					{	// 폼이 이미 열려있으면 열린폼을 앞쪽으로
						if(BaseForm.MdiChildren[i].Name == RodeFormName)
						{
							BaseForm.MdiChildren[i].BringToFront();
							tempForm = BaseForm.MdiChildren[i];
							mdiwin = true;
							break;
						}
					}

					if(mdiwin == false)
					{
						Assembly ServiceAssembly = Assembly.LoadFile( ProgramWhere.ToString() +"\\" + NamespaceName.ToString() + ".dll" );
						Form myForm = (Form)System.Activator.CreateInstance(ServiceAssembly.GetType(MdiFM.ToString()));

						for(int i=0; i<fieldName.Length; i++)
						{
							myForm.GetType().GetProperty(fieldName[i]).SetValue(myForm, fieldValue[i], null);
						}

						myForm.MdiParent = BaseForm;
						myForm.Show();
					}
					else
					{
						for(int i=0; i<fieldName.Length; i++)
						{
							tempForm.GetType().GetProperty(fieldName[i]).SetValue(tempForm, fieldValue[i], null);
						}

						tempForm.Activate();
					}

				}
			}
			catch(Exception f)
			{
				SystemBase.Loggers.Log(BaseForm.Name, f.ToString());
				MessageBox.Show(SystemBase.Base.MessageRtn("SY006"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		#endregion

		#region SHDBNULL

		public static object SH_DBNULL(object val, object defaultValue)
		{
			object objVal = null;

			if(val != null) 
			{
				if(val.Equals(System.DBNull.Value))
				{
					objVal = defaultValue;
				}
				else
				{
					objVal = val;
				}
			}

			return objVal;
		}

		public static object SH_DBNULL(object val, object defaultValue, Type type)
		{
			object objVal = null;

			if(val != null) 
			{
				if(val.Equals(System.DBNull.Value))
				{
					objVal = defaultValue;
				}
				else
				{
					objVal = val;
				}
			}

			if(type.Equals(typeof(Int32)))
				objVal =  Convert.ToInt32(objVal);
			else if(type.Equals(typeof(Int16)))
				objVal =  Convert.ToInt16(objVal);
			else if(type.Equals(typeof(Int64)))
				objVal =  Convert.ToInt64(objVal);
			else if(type.Equals(typeof(decimal)))
				objVal =  Convert.ToDecimal(objVal);
			else if(type.Equals(typeof(DateTime)))
				objVal =  Convert.ToDateTime(objVal);
			else if(type.Equals(typeof(String)))
				objVal =  Convert.ToString(objVal);
			else if(type.Equals(typeof(Double)))
				objVal =  Convert.ToDouble(objVal);

			return objVal;
		}

		#endregion

        #region 두 년월일 사이의 개월수 차이
        public static int GetMonthDiff(string date1, string date2)
        {
            int GetMonth = 0;
            int d1 = 0; int d2 = 0;
            // 무조건 1일 부터 계산
            DateTime Date1 = Convert.ToDateTime(date1.Substring(0, 7) + "-01");
            DateTime Date2 = Convert.ToDateTime(date2.Substring(0, 7) + "-01");

            d1 = Date1.Month;
            d2 = Date2.Month;

            GetMonth = (d2 - d1) + 1;
            return GetMonth;
        }
        #endregion

        #region 년,월 차감 일자 구하기
        public static string GetYearMonth(string date1, int InterVal, string Get_Flag)
        {
            string GetYearMonth = "";
            DateTime Date1 = Convert.ToDateTime(date1);

            if (Get_Flag == "Y")
            {
                GetYearMonth = Date1.AddYears(InterVal).ToString();
            }
            else if (Get_Flag == "M")
            {
                GetYearMonth = Convert.ToDateTime(Date1.AddYears(InterVal).ToString()).Year.ToString() + "-12-" + Convert.ToDateTime(Date1.AddYears(InterVal).ToString()).Day.ToString();
            }

            return GetYearMonth;
        }
        #endregion

        #region 현재프로그램의 최상위 메뉴명까지 표시..
        public static string GetMenuTree(string MenuId)
        {
            string GetMenu = "" ;

            string strSql = " usp_MENU_TREE ";
            strSql += "  @pMENU_ID = '" + MenuId + "' ";

            DataSet ds = SystemBase.DbOpen.NoTranDataSet(strSql);
            if (ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count ; i++)
                {
                    if (i == ds.Tables[0].Rows.Count - 1)
                    {
                        GetMenu += ds.Tables[0].Rows[i]["MENU_NAME"].ToString() ;
                    }
                    else
                    {
                        GetMenu += ds.Tables[0].Rows[i]["MENU_NAME"].ToString() + " > ";
                    }
                }
            }

            return GetMenu;
        }
        #endregion

        #region 툴바 버튼상태값 가져오기
        public static string GetToolbarButtonStatus(string UserID, string PGM_ID)
        {
            string status = "";

            //버튼설정
            string strSql = "usp_TOOLBARSET '" + UserID + "','" + PGM_ID + "'";
            DataTable Rowdt = SystemBase.DbOpen.NoTranDataTable(strSql);

            if (Rowdt.Rows.Count > 0)
            {
                for (int i = 0; i < Rowdt.Columns.Count; i++)
                {
                    status += Rowdt.Rows[0][i].ToString();
                }
            }

            return status;
        }
        #endregion

        #region 엑셀 Cell값 구하기
        public static string ExCol(int Col)
        {
            string strCol = "";

            strCol = Convert.ToChar(Col + 64).ToString();

            return strCol;
        }

        public static string ExcelCol(int Col)
        {
            string strCol = "";

            String columName = "";
            String columName2 = "";
 
            int tmpI = 0;
            int tmpJ = 0;

            if (Col > 26)
            {
                tmpJ = Col % 26;
                tmpI = Col / 26;
 
                if (tmpI > 26)
                {
                    if (tmpJ == 0)
                    {
                        tmpJ = 1;
                        tmpI -= 1;
                    }

                    columName = ExcelCol(tmpI).ToString();
                    columName2 = ExCol(tmpJ).ToString();
                }
                else
                {
                    if (tmpJ == 0)
                    {
                        tmpJ = 1;
                        tmpI -= 1;
                    }
                    columName = ExCol(tmpI).ToString();
                    columName2 = ExCol(tmpJ).ToString();
                }
            }
            else
            {
                columName = ExCol(Col);
            }

            strCol = columName + columName2;

            return strCol;
        }
        #endregion

        #region 잔업 특권 권한 조회(인사/근태)
        public static string OverRollSearch(string strType, string strDt, string strInternalCd)
        {
            string strRoll = "N";

            string Query = "usp_H_COMMON 'H015', , @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'@pCOM_CD = '" + strType + "', @pDATE = '" + strDt + "', @pCOM_NM = '" + strInternalCd + "' ";
            DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);

            if (dt.Rows.Count > 0)
            {
                strRoll = dt.Rows[0][0].ToString();
            }

            return strRoll;
        }
        #endregion

        #region 인사관리 권한
        public static string HumanRoll(string strEmpNo)
        {
            string strRoll = "N";

            string Query = "usp_H_COMMON 'H016', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "', @pCOM_CD = '" + strEmpNo + "' ";
            DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);

            if (dt.Rows.Count > 0)
            {
                strRoll = dt.Rows[0][0].ToString();
            }

            return strRoll;
        }
        #endregion

        #region INI 값 읽기 쓰기
        // INI 값 읽기 
        public String GetIniValue(String Section, String Key, String iniPath)
        {
            StringBuilder temp = new StringBuilder(255);
            int i = GetPrivateProfileString(Section, Key, "", temp, 255, iniPath);
            return temp.ToString();
        }

        [DllImport("kernel32")]
        public static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        [DllImport("kernel32")]
        public static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        #endregion

		#region 쿼리 결과로 사전을 생성 (2014.7.16, 이재광)
		/// <summary>
		/// 쿼리 결과로 사전을 생성합니다. 쿼리 결과는 필드가 2개 필요하며 첫 번째가 키, 두 번째가 값이 됩니다.
		/// </summary>
		/// <param name="query">쿼리</param>
		/// <returns>사전</returns>
		public static Dictionary<string, string> CreateDictionary(string query)
		{
			DataTable dt = SystemBase.DbOpen.NoTranDataTable(query);
			Dictionary<string, string> dic = new Dictionary<string, string>();

			for (int i = 0; i < dt.Rows.Count; i++)
			{
				if (dt.Columns.Count > 1)
					dic.Add(dt.Rows[i][0].ToString(), dt.Rows[i][1].ToString());
				else
					dic.Add(dt.Rows[i][0].ToString(), dt.Rows[i][0].ToString());
			}

			return dic;
		}
		#endregion


        #region 2015.08.21. hma 추가: 거래처이력정보의 적용시작일 체크하여 해당일 기준의 거래처명 SELECT 
        /// 특정일자 기준의 거래처명 가져오기. 첫번째 매개변수:기준일자, 두번째 매개변수:거래처코드
        public static String GetCustName(String BaseDate, String CustCd)
        {
            string strCustName = "";

            string Query = "usp_CO_COMM_CODE @pTYPE = 'CUST_NM', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "', @pDATE = '" + BaseDate + "', @pCODE = '" + CustCd + "' ";
            DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);

            if (dt.Rows.Count > 0)
            {
                strCustName = dt.Rows[0][0].ToString();
            }

            return strCustName;
        }
        #endregion
    }
}
