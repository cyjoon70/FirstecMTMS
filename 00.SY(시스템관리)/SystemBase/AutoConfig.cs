using System;
using System.Xml;
using System.Collections.Specialized;

namespace SystemBase
{
	/// <summary>
	/// AutoConfig에 대한 요약 설명입니다.
	/// </summary>
	public class AutoConfig
	{
		//prohibit instance creation
		private AutoConfig()
		{
		}

		static AutoConfig()
		{
			Init();
		}

		/* -----------------------------------------------
			* Attribute
		   ----------------------------------------------- */

        // DATABASE (Database Connection String)
		private static string strServer;
		private static string strDbNm;
		private static string strUid;
		private static string strPwd;
		private static string strDbConnString;

        // RFOLDER (Receive Infomaition Folder)
        private static string strRImportFolder;
        private static string strRExportFolder;

        // MFOLDER (Maps2_file Infomaition Folder)
        private static string strMFmtFolder;
        private static string strMBackupFolder;
        private static string strMLogFolder;

		// OTHSYS (Other System Information Folder)
		private static string strOImportFolder;
		private static string strOBackupFolder;
		
		/* -----------------------------------------------
			* Method
		   ----------------------------------------------- */

		// DATABASE (Database Connection String)
		public static string Server
		{
			get { return strServer; }
		}
		public static string DbNm
		{
			get { return strDbNm; }
		}
		public static string Uid
		{
			get { return strUid; }
		}
		public static string Pwd
		{
			get { return strPwd; }
		}
		public static string DbConnString
		{
			get { return strDbConnString; }
		}

        // RFOLDER (Receive Infomaition Folder)
        public static string RImportFolder
        {
            get { return strRImportFolder; }
        }
        public static string RExportFolder
        {
            get { return strRExportFolder; }
        }

        // MFOLDER (Maps2_file Infomaition Folder)
        public static string MFmtFolder
        {
            get { return strMFmtFolder; }
        }
        public static string MBackupFolder
        {
            get { return strMBackupFolder; }
        }
        public static string MLogFolder
        {
            get { return strMLogFolder; }
        }

		// OTHSYS (ERP Server Network Connection Information)
		public static string OImportFolder
		{
			get { return strOImportFolder; }
		}
		public static string OBackupFolder
		{
			get { return strOBackupFolder; }
		}
		
		/* -----------------------------------------------
			* User Define Module
		   ----------------------------------------------- */

		public static void Init()
		{
			try
			{
				// 환경설정 XML File Load
				string configFile = new AutoConfig().GetType().Assembly.Location + ".xml";
				XmlDocument docXml = new XmlDocument();
				docXml.Load(configFile);

				// DATABASE INFO
				XmlNode node = docXml.GetElementsByTagName("DATABASE")[0];

				strServer = node.Attributes["server"].Value;
				strDbNm   = node.Attributes["db"    ].Value;
				strUid    = node.Attributes["uid"   ].Value;
				strPwd    = node.Attributes["pwd"   ].Value;

				strDbConnString = "Data Source="     + strServer + 
								  ";Database="       + strDbNm   + 
								  ";user id="        + strUid    + 
								  ";password="       + strPwd;

                // RFOLDER INFO
                node = docXml.GetElementsByTagName("RFOLDER")[0];

                strRImportFolder = node.Attributes["import"].Value;
                strRExportFolder = node.Attributes["export"].Value;

                // MFOLDER INFO
                node = docXml.GetElementsByTagName("MFOLDER")[0];

                strMFmtFolder    = node.Attributes["fmt"   ].Value;
                strMBackupFolder = node.Attributes["backup"].Value;
                strMLogFolder    = node.Attributes["log"   ].Value;

				// OTHSYS INFO
				node = docXml.GetElementsByTagName("OTHSYS")[0];

				strOImportFolder = node.Attributes["import"].Value;
				strOBackupFolder = node.Attributes["backup"].Value;
			}
			catch (Exception ex)
			{
				Loggers.Log(ex.Message, ex);
			}
		}
	
	}
}
