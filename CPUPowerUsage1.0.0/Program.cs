using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using SystemMonitor;

namespace CPUPowerUsage1._0._0
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            using (var notificationIcon = new NotificationIcon())
            {
                Application.Run();
            }
        }
    }
}
