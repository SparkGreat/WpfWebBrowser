using CefSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace WpfWebBrowser.CefSharpExtend.Handler
{
    /// <summary>
    /// 网页下载处理类
    /// </summary>
    class DownloadHandler : IDownloadHandler
    {
        public void OnBeforeDownload(IWebBrowser chromiumWebBrowser, IBrowser browser, DownloadItem downloadItem, IBeforeDownloadCallback callback)
        {
            //这里自己写自己下载文件的方式，downloadItem.Url是下载文件的地址，这里是采用ie的下载方式
            WebBrowser ie = new WebBrowser();
            ie.Navigate(downloadItem.Url);
            browser.CloseBrowser(false);
        }

        public void OnDownloadUpdated(IBrowser browser, DownloadItem downloadItem, IDownloadItemCallback callback)
        {
            downloadItem.IsCancelled = true;
        }

        public bool OnDownloadUpdated(DownloadItem downloadItem)
        {
            return false;
        }

        public void OnDownloadUpdated(IWebBrowser chromiumWebBrowser, IBrowser browser, DownloadItem downloadItem, IDownloadItemCallback callback)
        {

        }
    }
}
