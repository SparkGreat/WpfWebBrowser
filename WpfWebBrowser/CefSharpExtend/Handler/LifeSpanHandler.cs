using CefSharp;
using CefSharp.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfWebBrowser.CefSharpExtend.Handler
{
    /// <summary>
    /// 生命周期处理类
    /// </summary>
    class LifeSpanHandler : ILifeSpanHandler
    {
        public event Action<object, string> NewWindow;


        public bool DoClose(IWebBrowser chromiumWebBrowser, IBrowser browser)
        {
            return false;
        }

        public void OnAfterCreated(IWebBrowser chromiumWebBrowser, IBrowser browser)
        {
            //Application.Current.Dispatcher.BeginInvoke((Action)delegate ()
            //{
            //    var v1 = browser;
            //    var v2 = chromiumWebBrowser;

            //    Console.WriteLine("地址：" + chromiumWebBrowser.Address);
            //});
        }

        public void OnBeforeClose(IWebBrowser chromiumWebBrowser, IBrowser browser)
        {

        }

        public bool OnBeforePopup(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, string targetUrl, string targetFrameName, WindowOpenDisposition targetDisposition, bool userGesture, IPopupFeatures popupFeatures, IWindowInfo windowInfo, IBrowserSettings browserSettings, ref bool noJavascriptAccess, out IWebBrowser newBrowser)
        {
            newBrowser = null;

            chromiumWebBrowser.Load(targetUrl);

            Application.Current.Dispatcher.BeginInvoke((Action)delegate ()
            {
                //var v1 = browser;
                //var v2 = chromiumWebBrowser;

                NewWindow?.Invoke(chromiumWebBrowser, targetUrl);

                Console.WriteLine("地址：" + chromiumWebBrowser.Address);
            });

            return true;
        }

    }
}
