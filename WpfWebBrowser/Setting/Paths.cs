using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfWebBrowser.Setting
{
    public class Paths
    {

        private static string BasePath = AppDomain.CurrentDomain.BaseDirectory;

        public static string cache = BasePath + "\\cache";

        public static string plugins_flash = @"C:\Windows\System32\Macromed\Flash\pepflashplayer64_33_0_0_401.dll";

    }
}
