using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;
using Microsoft.Win32.SafeHandles;
using System.Data;
using System.ComponentModel;
using System.Threading;

namespace SystemBase
{
	public class RawPrinterHelper
	{
		// Structure and API declarions:
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		
		public class DOCINFOA
		{
			[MarshalAs(UnmanagedType.LPStr)]
			public string pDocName;
			[MarshalAs(UnmanagedType.LPStr)]
			public string pOutputFile;
			[MarshalAs(UnmanagedType.LPStr)]
			public string pDataType;
		}

		//PortType enum
		[Flags]
		public enum PortType : int
		{
			write = 0x1,
			read = 0x2,
			redirected = 0x4,
			net_attached = 0x8
		}


		//struct for PORT_INFO_2
		[StructLayout(LayoutKind.Sequential)]
		public struct PORT_INFO_2
		{
			public string pPortName;
			public string pMonitorName;
			public string pDescription;
			public PortType fPortType;
			internal int Reserved;
		}

		//Win32 API
		[DllImport("winspool.drv", EntryPoint = "EnumPortsA", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		public static extern int EnumPorts(string pName, int Level, IntPtr lpbPorts, int cbBuf, ref int pcbNeeded, ref int pcReturned);

		[DllImport("winspool.Drv", EntryPoint = "OpenPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
		public static extern bool OpenPrinter([MarshalAs(UnmanagedType.LPStr)] string szPrinter, out IntPtr hPrinter, IntPtr pd);

		[DllImport("winspool.Drv", EntryPoint = "ClosePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
		public static extern bool ClosePrinter(IntPtr hPrinter);

		[DllImport("winspool.Drv", EntryPoint = "StartDocPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
		public static extern bool StartDocPrinter(IntPtr hPrinter, Int32 level, [In, MarshalAs(UnmanagedType.LPStruct)] DOCINFOA di);

		[DllImport("winspool.Drv", EntryPoint = "EndDocPrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
		public static extern bool EndDocPrinter(IntPtr hPrinter);

		[DllImport("winspool.Drv", EntryPoint = "StartPagePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
		public static extern bool StartPagePrinter(IntPtr hPrinter);

		[DllImport("winspool.Drv", EntryPoint = "EndPagePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
		public static extern bool EndPagePrinter(IntPtr hPrinter);

		[DllImport("winspool.Drv", EntryPoint = "WritePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
		public static extern bool WritePrinter(IntPtr hPrinter, IntPtr pBytes, Int32 dwCount, out Int32 dwWritten);

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern SafeFileHandle CreateFile(string port, FileAccess dwDesiredAccess,
		uint dwShareMode, IntPtr lpSecurityAttributes, FileMode dwCreationDisposition,
		uint dwFlagsAndAttributes, IntPtr hTemplateFile);


		// SendBytesToPrinter()
		// When the function is given a printer name and an unmanaged array
		// of bytes, the function sends those bytes to the print queue.
		// Returns true on success, false on failure.
		public static bool SendBytesToPrinter(string szPrinterName, IntPtr pBytes, Int32 dwCount)
		{
			Int32 dwError = 0, dwWritten = 0;

			IntPtr hPrinter = new IntPtr(0);
			DOCINFOA di = new DOCINFOA();

			bool bSuccess = false; // Assume failure unless you specifically succeed.

			di.pDocName = "My C#.NET RAW Document";

			di.pDataType = "RAW";

			// Open the printer.
			if (OpenPrinter(szPrinterName.Normalize(), out hPrinter, IntPtr.Zero))
			{
				// Start a document.
				if (StartDocPrinter(hPrinter, 1, di))
				{
					// Start a page.
					if (StartPagePrinter(hPrinter))
					{
						// Write your bytes.
						bSuccess = WritePrinter(hPrinter, pBytes, dwCount, out dwWritten);

						EndPagePrinter(hPrinter);
					}
					EndDocPrinter(hPrinter);

				}
				ClosePrinter(hPrinter);
			}
			// If you did not succeed, GetLastError may give more information
			// about why not.

			if (bSuccess == false)
			{
				dwError = Marshal.GetLastWin32Error();

			}
			return bSuccess;
		}


		//public static bool SendStringToPrinterOld(string szPrinterName, string szString)
		//{
		//    IntPtr pBytes;
		//    Int32 dwCount;
		//    bool bReturn = true;

		//    // How many characters are in the string?
		//    dwCount = szString.Length;
		//    // Assume that the printer is expecting ANSI text, and then convert
		//    // the string to ANSI text.
		//    pBytes = Marshal.StringToCoTaskMemUni(szString);
		//    // Send the converted ANSI string to the printer.
		//    bReturn = SendBytesToPrinter(szPrinterName, pBytes, dwCount);
		//    Marshal.FreeCoTaskMem(pBytes);
		//    return bReturn;
		//}

		/// <summary>
		/// 바코드 출력
		/// </summary>
		/// <param name="port">"LPT1", "COM1" 등 PORT NAME</param>
		/// <param name="szString">출력할 문자열</param>
		/// <returns>출력 성공 여부</returns>
		public static bool SendStringToPrinter(string port, string szString)
		{

			Thread.Sleep(1000);

			bool bReturn = true;

			Byte[] buffer = new byte[szString.Length];
			buffer = System.Text.Encoding.UTF8.GetBytes(szString);
			
			// Use the CreateFile external func to connect to the port
			SafeFileHandle printer = CreateFile(port, FileAccess.ReadWrite, 0, IntPtr.Zero, FileMode.Open, 0, IntPtr.Zero);
			
			// Aqui verifico se a impressora é válida
			if (printer.IsInvalid == true)
			{
				bReturn = false;
				return bReturn;
			}

			// Open the filestream to the lpt1 port and send the command
			FileStream lpt1 = new FileStream(printer, FileAccess.ReadWrite);
			lpt1.Write(buffer, 0, buffer.Length);
			
			// Close the FileStream connection
			lpt1.Close();

			return bReturn;
			
		}

		/// <summary>
		/// method for retrieving all available printer ports
		/// </summary>
		/// <returns>generic list populated with post names (i.e; COM1, LTP1, etc)</returns>
		public static DataTable GetPortNames()
		{
			//variables needed for Win32 API calls
			int result; int needed = 0; int cnt = 0; IntPtr buffer = IntPtr.Zero; IntPtr port = IntPtr.Zero;

			//list to hold the returned port names
			DataTable ports = new DataTable();
			ports.Columns.Add("Key", typeof(string));
			ports.Columns.Add("Value", typeof(string));

			//new PORT_INFO_2 for holding the ports
			PORT_INFO_2[] portInfo = null;

			//enumerate through to get the size of the memory we need
			result = EnumPorts("", 2, buffer, 0, ref needed, ref cnt);
			try
			{


				//allocate memory
				buffer = Marshal.AllocHGlobal(Convert.ToInt32(needed + 1));

				//get list of port names
				result = EnumPorts("", 2, buffer, needed, ref needed, ref cnt);

				//check results, if 0 (zero) then we got an error
				if (result != 0)
				{
					//set port value
					port = buffer;

					//instantiate struct
					portInfo = new PORT_INFO_2[cnt];

					//now loop through the returned count populating our array of PORT_INFO_2 objects
					for (int i = 0; i < cnt; i++)
					{
						portInfo[i] = (PORT_INFO_2)Marshal.PtrToStructure(port, typeof(PORT_INFO_2));
						port = (IntPtr)(port.ToInt32() + Marshal.SizeOf(typeof(PORT_INFO_2)));
					}
					port = IntPtr.Zero;
				}
				else
					throw new Win32Exception(Marshal.GetLastWin32Error());

				//now get what we want. Loop through al the
				//items in the PORT_INFO_2 Array and populate our generic list
				for (int i = 0; i < cnt; i++)
				{
					ports.Rows.Add(portInfo[i].pPortName, portInfo[i].pDescription);
				}

				return ports;
			}
			catch (Exception ex)
			{
				SystemBase.Loggers.Log("PrinterHelperNew", ex.ToString());
				return null;
			}
			finally
			{
				if (buffer != IntPtr.Zero)
				{
					Marshal.FreeHGlobal(buffer);
					buffer = IntPtr.Zero;
					port = IntPtr.Zero;

				}
			}
		}

		/// <summary>
		/// 바코드 프린팅
		/// </summary>
		/// <param name="ipAddress">프린터 포트</param>
		/// <param name="strZPL">인쇄문자열</param>
		/// <returns></returns>
		public static bool PrintZPL(string ipAddress, string strZPL)
		{

			bool bReturn = true;
			int port = 9100;

			try
			{

				Thread.Sleep(1000);
				
				// Open connection
				System.Net.Sockets.TcpClient client = new System.Net.Sockets.TcpClient();
				client.Connect(ipAddress, port);

				// Write ZPL String to connection
				System.IO.StreamWriter writer = new System.IO.StreamWriter(client.GetStream());
				writer.Write(strZPL);
				writer.Flush();

				// Close Connection
				writer.Close();
				client.Close();
			}
			catch (Exception e)
			{
				bReturn = false;
			}

			return bReturn;
		}

		/// <summary>
		/// 프린터포트 ComboBox 설정
		/// </summary>
		/// <param name="cboPort">포트 ComboBox Control</param>
		public static void SetPortCombo(C1.Win.C1List.C1Combo cboPort)
		{
			DataTable dt = new DataTable();
			string strPort = string.Empty;
			
			if (GetPortNames() != null)
			{

				//dt = GetPortNames().Select("Key LIKE 'LPT%' OR Value = '표준 TCP/IP 포트'").CopyToDataTable<DataRow>();
				dt = GetPortNames();

				DataRow dr = dt.NewRow();
				dr[0] = "선택";
				dr[1] = "*";
				dt.Rows.InsertAt(dr, 0);
			}

			cboPort.Refresh();
			cboPort.ComboStyle = C1.Win.C1List.ComboStyleEnum.DropdownList;

			cboPort.ValueMember = dt.Columns[1].ColumnName.ToString();
			cboPort.DisplayMember = dt.Columns[0].ColumnName.ToString();
			cboPort.AllowColMove = false;
			cboPort.DataSource = dt;

			cboPort.AllowColMove = false;
			cboPort.Splits[0].DisplayColumns[0].Width = cboPort.Size.Width;
			cboPort.Splits[0].DisplayColumns[1].Width = 0;

			cboPort.HScrollBar.Style = C1.Win.C1List.ScrollBarStyleEnum.None;
			cboPort.VScrollBar.Style = C1.Win.C1List.ScrollBarStyleEnum.Automatic;
			cboPort.ColumnHeaders = false;

			strPort = GetPrinterPort();

			if (string.IsNullOrEmpty(strPort) == false)
			{

				for (int i = 0; i <= dt.Rows.Count - 1; i++)
				{
					if (string.Compare(dt.Rows[i]["Key"].ToString(), strPort, true) == 0)
					{
						cboPort.SelectedIndex = i;
						break;
					}
				}
			}
			else
			{
				cboPort.SelectedIndex = 0;
			}
		}

		/// <summary>
		/// 프린터 포트 조회
		/// </summary>
		/// <returns></returns>
		public static string GetPrinterPort()
		{
			string strReturn = string.Empty;
			string AppFolder = string.Empty;
			string sLine = string.Empty;

			AppFolder = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath);
			StreamReader objReader = new StreamReader(AppFolder + "\\E2MAX_FTP.ini");
			
			ArrayList arrText = new ArrayList();

			while (sLine != null)
			{
				sLine = objReader.ReadLine();
				if (sLine != null)
				{
					arrText.Add(sLine);

					if (sLine.Length > 11 && sLine.Substring(0, 11).ToString() == "PrinterPort")
					{
						string[] strTemp = sLine.Split('=');
						strReturn = strTemp[1].Trim();
					}
				}
			}
			objReader.Close();

			return strReturn;
		}

		/// <summary>
		/// 프린터 포트 저장
		/// </summary>
		/// <param name="port">포트</param>
		public static void SavePrinterPort(string port)
		{
			SystemBase.Base.WritePrivateProfileString("DATABASE", "PrinterPort", port, SystemBase.Base.ProgramWhere + "\\E2MAX_FTP.ini");
		}
	}
}
