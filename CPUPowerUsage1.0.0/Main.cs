using OpenHardwareMonitor.Hardware;
using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Threading;
using Microsoft.Win32;

namespace SystemMonitor
{
    internal sealed class NotificationIcon : IDisposable
    {
        private static Mutex mutex = null;

        private readonly NotifyIcon notifyIcon;
        private readonly Computer computer;
        private readonly Font font;
        private readonly string appName;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool DestroyIcon(IntPtr handle);

        public NotificationIcon()
        {
            string mutexName = "SystemMonitorAppMutex";
            mutex = new Mutex(true, mutexName, out bool isOnlyInstance);
            if (!isOnlyInstance)
            {
                MessageBox.Show("Another instance is already running.", "Instance Check", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                Environment.Exit(0);
            }

            notifyIcon = new NotifyIcon
            {
                Icon = new Icon(CPUPowerUsage1._0._0.Properties.Resources.processor_16, 40, 40),
                Visible = true,
                ContextMenuStrip = new ContextMenuStrip()
            };
            notifyIcon.ContextMenuStrip.Items.Add("Start with Windows", null, OnStartWithWindowsClicked);
            notifyIcon.ContextMenuStrip.Items.Add("Exit", null, OnExitClicked);

            computer = new Computer { CPUEnabled = true };
            computer.Open();

            font = new Font("Consolas", 9f, FontStyle.Bold);

            appName = "SystemMonitor";

            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer { Interval = 1500 };
            timer.Tick += OnTimerTick;
            timer.Start();

            UpdateStartWithWindowsMenuItem();
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            float cpuUsage = 0, cpuWattage = 0;

            try
            {
                foreach (var hardware in computer.Hardware)
                {
                    if (hardware.HardwareType == HardwareType.CPU)
                    {
                        hardware.Update();

                        foreach (var sensor in hardware.Sensors)
                        {
                            if (sensor.SensorType == SensorType.Load && sensor.Name.Contains("CPU Total"))
                                cpuUsage = sensor.Value ?? 0;
                            else if (sensor.SensorType == SensorType.Power && sensor.Name.Contains("CPU Package"))
                                cpuWattage = sensor.Value ?? 0;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while updating hardware: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            try
            {
                using (var iconBitmap = new Bitmap(16, 16))
                using (var iconGraphics = Graphics.FromImage(iconBitmap))
                {
                    iconGraphics.Clear(Color.Black);
                    iconGraphics.FillRectangle(Brushes.Black, 0, 0, iconBitmap.Width, iconBitmap.Height);

                    SizeF textSize = iconGraphics.MeasureString($"{cpuWattage:F0}", font);
                    float x = (iconBitmap.Width - textSize.Width) / 2;
                    float y = (iconBitmap.Height - textSize.Height) / 2;

                    iconGraphics.DrawString($"{cpuWattage:F0}", font, Brushes.White, x, y);

                    IntPtr hIcon = iconBitmap.GetHicon();
                    notifyIcon.Icon = Icon.FromHandle(hIcon);
                    DestroyIcon(hIcon);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while updating the icon: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            notifyIcon.Text = $"CPU: {cpuUsage:F0}% - {cpuWattage:F1}W";
        }


        private void OnStartWithWindowsClicked(object sender, EventArgs e)
        {
            bool isStartWithWindows = IsStartWithWindows();
            if (isStartWithWindows)
            {
                RemoveStartWithWindows();
            }
            else
            {
                AddStartWithWindows();
            }
            UpdateStartWithWindowsMenuItem();
        }

        private void OnExitClicked(object sender, EventArgs e)
        {
            Dispose();
            Application.Exit();
        }

        private bool IsStartWithWindows()
        {
            try
            {
                RegistryKey startupKey = Registry.CurrentUser.OpenSubKey(
                    "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run",
                    true
                );
                bool enabled = startupKey.GetValue(appName) != null;
                startupKey.Close();
                return enabled;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while checking startup status: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void AddStartWithWindows()
        {
            try
            {
                RegistryKey startupKey = Registry.CurrentUser.OpenSubKey(
                    "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run",
                    true
                );
                startupKey.SetValue(appName, Application.ExecutablePath);
                startupKey.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while adding startup: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RemoveStartWithWindows()
        {
            try
            {
                RegistryKey startupKey = Registry.CurrentUser.OpenSubKey(
                    "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run",
                    true
                );
                startupKey.DeleteValue(appName, false);
                startupKey.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while removing startup: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateStartWithWindowsMenuItem()
        {
            bool isStartWithWindows = IsStartWithWindows();
            notifyIcon.ContextMenuStrip.Items[0].Text = isStartWithWindows ? "Don't start with Windows" : "Start with Windows";
        }

        public void Dispose()
        {
            if (mutex != null)
            {
                mutex.ReleaseMutex();
                mutex = null;
            }
            computer.Close();
            notifyIcon.Dispose();
            font.Dispose();
        }
    }
}
