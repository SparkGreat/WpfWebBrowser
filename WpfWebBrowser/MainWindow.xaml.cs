using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using CefSharp;
using CefSharp.DevTools;
using CefSharp.Wpf;
using MaterialDesignThemes.Wpf;
using WpfWebBrowser.CefSharpExtend;
using WpfWebBrowser.CefSharpExtend.Handler;
using WpfWebBrowser.CefSharpExtend.Helper;
using WpfWebBrowser.Setting;
using WpfWebBrowser.ViewModel.Base;

namespace WpfWebBrowser
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeCef();

            InitializeComponent();

            //MaxWidth = SystemParameters.WorkArea.Size.Width;
            MaxHeight = SystemParameters.WorkArea.Size.Height + 14;
        }

        MainViewModel VM_Main = new MainViewModel();
        

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = VM_Main;

            VM_Main.Chrom = VM_Main.TabCreate("https://www.baidu.com");
            
            ccMain.Content = VM_Main.Chrom;

            
        }

        private void InitializeCef()
        {

            CefSettings cefSettings = new CefSettings();

            cefSettings.CachePath = Paths.cache; // 设置缓存地址
            cefSettings.PersistSessionCookies = true;

            cefSettings.CefCommandLineArgs.Add("--process-per-tab");

            cefSettings.Locale = "zh-CN";
            cefSettings.CefCommandLineArgs.Add("disable-gpu");  //去掉gpu，否则chrome显示有问题
            //cefSettings.UserAgent += "Flame/1.0.0";
            //cefSettings.CefCommandLineArgs.Add("no-proxy-server", "1");       //去掉代理，增加加载网页速度

            cefSettings.CefCommandLineArgs.Add("--disable-web-security");  // 不要强制执行同源策略。允许跨域
            cefSettings.CefCommandLineArgs.Add("--enable-system-flash");   // 使用系统flash
            //cefSettings.CefCommandLineArgs.Add("ppapi-plugin-launcher", "1");  //启动flash
            //cefSettings.CefCommandLineArgs.Add("ppapi-flash-version", "33.0.0.401"); // 设置flash插件版本
            //cefSettings.CefCommandLineArgs.Add("proxy-auto-detect", "0");    //去掉代理，增加加载网页速度

            //插入地址
            //cefSettings.CefCommandLineArgs.Add("ppapi-flash-path", Paths.plugins_flash);

            Cef.Initialize(cefSettings);
        }

        #region 窗体功能

        private void BtnWinMin(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void BtnWinRes(object sender, RoutedEventArgs e)
        {
            WindowState ^= WindowState.Maximized;
        }

        private void BtnWinClose(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void BtnNewBrowser_Click(object sender, RoutedEventArgs e)
        {
            //VM_Main.TabCreate("https://www.baidu.com");
        }

        #endregion

        #region 头部功能
        
        private void TxtUrl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                VM_Main.Chrom.Load(txtUrl.Text);
            }
        }

        #endregion


        // 设置代理
        async private void SetProxy(ChromiumWebBrowser cwb, string Address)
        {
            await Cef.UIThreadTaskFactory.StartNew(delegate
            {
                var rc = cwb.GetBrowser().GetHost().RequestContext;

                var v = new Dictionary<string, string>();

                v["mode"] = "fixed_servers";

                v["server"] = Address;

                string error;

                bool success = rc.SetPreference("proxy", v, out error);
            });

        }



        #region 更多功能
        
        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        #endregion



        private void BtnTest_Click(object sender, RoutedEventArgs e)
        {
            TestAsync();
        }

        private async Task TestAsync()
        {
            

            CookieVisitor CV = new CookieVisitor();
            var vc = CV.GetCookieList("qq.com");

            string cookice = "";
            foreach (var item in vc)
            {
                cookice += $"{item.Name}={item.Value};";
            }

            var vrm = RequestHelper.Request("https://qun.qq.com/cgi-bin/qun_mgr/get_friend_list", CefSharpExtend.Enums.MethodType.Post, cookice, null, "bkn=1998726893");
        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void T_Thumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            Top += e.VerticalChange;
            Height -= e.VerticalChange;
        }
        private void B_Thumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            Height += e.VerticalChange;
        }
        private void L_Thumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            Left += e.HorizontalChange;
            Width -= e.HorizontalChange;
        }
        private void R_Thumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            Width += e.HorizontalChange;
        }

    }
}
