using System;
using System.Globalization;
using System.IO.Ports;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Windows.Forms;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.ApplicationServices;

namespace LgTvWakeup
{
	public partial class Form1 : Form
	{

		private SerialPort port;

		public Form1()
		{
			InitializeComponent();
			PowerManager.IsMonitorOnChanged += MonitorOnChanged;

			notifyIcon1.Icon = Icon = Properties.Resources.MonPower;
			foreach (var portName in SerialPort.GetPortNames())
			{
				comboBox1.Items.Add(portName);
			}
		}

		private void MonitorOnChanged(object sender, EventArgs e)
		{
			bool monOn = PowerManager.IsMonitorOn;
			using (var port = new SerialPort(comboBox1.Text, 9600, Parity.None, 8, StopBits.One))
			{
				port.Handshake = Handshake.XOnXOff;
				port.Open();
				port.WriteLine("ka 01 " + (monOn ? "01" : "00"));
				port.Close();
			}
		}

		//Another way, but always returned lParam = 2, so no way to find out is display turning on or off

		//const int WM_SYSCOMMAND = 0x0112;
		//const int SC_MONITORPOWER = 0xF170;
		//const int SC_SCREENSAVE = 0xF170;

		//[PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
		//protected override void WndProc(ref Message m)
		//{
		//	base.WndProc(ref m);

		//	if (m.Msg == WM_SYSCOMMAND)
		//	{
		//		var cmd = m.WParam.ToInt32() & 0xFFF0;
		//		if (cmd == SC_MONITORPOWER || cmd == SC_SCREENSAVE)
		//		{
		//			int state = m.LParam.ToInt32();
		//		}
		//	}
		//}

		private void OnResize(object sender, EventArgs e)
		{
			if (WindowState == FormWindowState.Minimized) Hide();
		}

		private void OnNotifyIconClick(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
				ShowAndBringToFront();
		}

		private void OnOpenSettings(object sender, EventArgs e)
		{
			ShowAndBringToFront();
		}

		private void ShowAndBringToFront()
		{
			Show();
			if (WindowState == FormWindowState.Minimized)
				WindowState = FormWindowState.Normal;
			BringToFront();
		}

		private void OnExit(object sender, EventArgs e)
		{
			Close();
		}
	}
}
