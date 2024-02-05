using System;
using System.Xml;
using System.Collections;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;

namespace SystemBase
{
	/// <summary>
	/// Admin Program이 기동할때 필요한 각종 정보를 가져오는 Class이다.
	/// 생성자가 Static으로 만들어져 있기 때문에 프로그램 기동시 자동으로 모든 세팅값을
	/// 가져와 세팅을 한다.
	/// </summary>
	public class Settings
	{
		//prohibit instance creation
		private Settings()
		{
		}

		static Settings()
		{
			Init();
		}

		/* -----------------------------------------------
			* Attribute
		   ----------------------------------------------- */

		// connection string
		private static string strConnectionString;

		// Log Level;
		private static int intLogLevel;

		// AdminOnly
		private static string strAdminOnly;

		// WorkStartTime
		private static string strWorkStartTime;

		// VinNo 에서 차종구분코드 시작위치;
		private static int intCkndCdPosInVinNo;

		// VinNo 에서 차종구분코드 길이;
		private static int intCkndCdLenInVinNo;

		// 언어구분
		private static string strLanguage;

		// Company Code
		private static string strCompanyCode;

		/* -----------------------------------------------
			* Method
		   ----------------------------------------------- */

		public static string ConnectionString
		{
			get { return strConnectionString; }
		}

		public static int LogLevel
		{
			get { return intLogLevel; }
			set { intLogLevel = value; }
		}

		public static string AdminOnly
		{
			get { return strAdminOnly; }
			set { strAdminOnly = value; }
		}

		public static string WorkStartTime
		{
			get { return strWorkStartTime; }
			set { strWorkStartTime = value; }
		}

		public static int CkndCdPosInVinNo
		{
			get { return intCkndCdPosInVinNo; }
		}

		public static int CkndCdLenInVinNo
		{
			get { return intCkndCdLenInVinNo; }
		}
		
		public static string Language
		{
			get { return strLanguage; }
			set { strLanguage = value; }
		}
		
		public static string CompanyCode
		{
			get { return strCompanyCode; }
		}

		/* -----------------------------------------------
			* User Define Module
		   ----------------------------------------------- */

		private static void Init()
		{
			try
			{
				// 환경설정 XML File Load
				string configFile = new Settings().GetType().Assembly.Location + ".xml";
				XmlDocument doc = new XmlDocument();
				doc.Load(configFile);

				XmlElement elem = null;

				StringDictionary settings = new StringDictionary();

				// 값을 읽어 올 node부분을 지정한다. 즉 XML파일에서 Set으로 시작하는 부분의 Key, Value부분을 가져온다.
				XmlNodeList nodes = doc.SelectNodes("//set");

				for (int i = 0; i < nodes.Count; i++)
				{
					elem = (XmlElement)nodes[i];
					string key = elem.GetAttribute("key");
					string val = elem.GetAttribute("value");
					settings.Add(key, val);

					// Xml에서의 Key, Value값을 로그에 저장한다.
					Logger.StaticLog("CONFIG: KEY=" + key + ", VALUE=" + val);
				}
				
				// XML File에 저장된 내용을 Static 변수에 저장한다.
				strConnectionString		= settings["ConnectionString"];
				intLogLevel				= Convert.ToInt32(settings["LogLevel"]);
				strAdminOnly			= settings["AdminOnly"];
				strWorkStartTime		= settings["WorkStartTime"];
				intCkndCdPosInVinNo		= Convert.ToInt32(settings["CkndCdPosInVinNo"]) - 1; // String Index 는 0 부터 시작하므로 실제 위치는 -1 한 값
				intCkndCdLenInVinNo		= Convert.ToInt32(settings["CkndCdLenInVinNo"]);
				strLanguage				= settings["Language"];
				strCompanyCode			= settings["CompanyCode"];
			}
			catch (Exception ex)
			{
				Logger.StaticLog(ex.Message + " " + ex.StackTrace);
			}
		}

		public static bool SaveConfig()
		{
			try
			{
				// 환경설정 XML File Load
				string configFile = new Settings().GetType().Assembly.Location + ".xml";
				
				XmlDocument doc = new XmlDocument();
				
				doc.Load(configFile);

				XmlNodeList nodes = doc.SelectNodes("//set");

				XmlElement elem = null;

				for (int i = 0; i < nodes.Count; i++)
				{
					elem = (XmlElement)nodes[i];
					string key = elem.GetAttribute("key");

					switch(key) 
					{
						case "LogLevel":
							if (intLogLevel.ToString() != elem.GetAttribute("value").ToString())
							{
								elem.SetAttribute("value", intLogLevel.ToString());
								Logger.StaticLog("CHANGE CONFIG: KEY=" + key + ", VALUE=" + intLogLevel.ToString());
							}
							break;

						case "AdminOnly":
							if (strAdminOnly != elem.GetAttribute("value").ToString())
							{
								elem.SetAttribute("value", strAdminOnly);
								Logger.StaticLog("CHANGE CONFIG: KEY=" + key + ", VALUE=" + strAdminOnly);
							}
							break;

						case "Language":
							if (strLanguage != elem.GetAttribute("value").ToString())
							{
								elem.SetAttribute("value", strLanguage);
								Logger.StaticLog("CHANGE CONFIG: KEY=" + key + ", VALUE=" + strLanguage);
							}
							break;
					}

				}
				doc.Save(configFile);
				return true;

			}
			catch (Exception ex)
			{
				Logger.StaticLog(ex.Message + " " + ex.StackTrace);
				return false;
			}
		}


	}
}

