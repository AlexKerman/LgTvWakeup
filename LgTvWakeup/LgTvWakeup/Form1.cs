﻿using System;
using System.Configuration;
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
			notifyIcon1.Icon = Icon;
			PowerManager.IsMonitorOnChanged += MonitorOnChanged;

			var settingsPort = Properties.Settings.Default["PortName"].ToString();
			
			foreach (var portName in SerialPort.GetPortNames())
			{
				comboBox1.Items.Add(portName);
				if (portName == settingsPort)
					comboBox1.SelectedItem = portName;
			}
		}

		private string PortName => comboBox1.Text;

		private void MonitorOnChanged(object sender, EventArgs e)
		{
			bool monOn = PowerManager.IsMonitorOn;
			SendMessage("ka 01 " + (monOn ? "01" : "00"));
		}

		private void SendMessage(string message)
		{
			if (PortName == "") return;
			try
			{
				using (var port = new SerialPort(PortName, 9600, Parity.None, 8, StopBits.One))
				{
					port.Handshake = Handshake.XOnXOff;
					port.Open();
					port.WriteLine(message);
					port.Close();
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				throw;
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

		private void OnSoundOn(object sender, EventArgs e)
		{
			SendMessage("ke 00 01");
		}

		private void OnSoundOff(object sender, EventArgs e)
		{
			SendMessage("ke 00 00");
		}

		private void comboBox1_SelectionChangeCommitted(object sender, EventArgs e)
		{
			if (PortName != "")
			{
				Properties.Settings.Default["PortName"] = PortName;
				Properties.Settings.Default.Save();
			}
		}
	}
}
