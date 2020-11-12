using CefSharp;
using CefSharp.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using WpfWebBrowser.CefSharpExtend.Handler;
using WpfWebBrowser.ViewModel;

namespace WpfWebBrowser.View.Browser
{
    /// <summary>
    /// ChormBrowser.xaml 的交互逻辑
    /// </summary>
    public partial class ChormBrowser : UserControl
    {
        public ChormBrowser()
        {
            InitializeComponent();
        }
        
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Chrom.MenuHandler = new MenuHandler();
            Chrom.LifeSpanHandler = new LifeSpanHandler();
            Chrom.DownloadHandler = new DownloadHandler();
            Chrom.KeyboardHandler = new KeyboardHandler();
            //Chrom.DragHandler = new DragHandler();
            Chrom.LoadHandler = new LoadHandler();
            //Chrom.RequestHandler = new RequestHandler();
            //Chrom.DisplayHandler = new DisplayHandler();

            

        }
        

        public void GoBack()
        {
            if (Chrom.GetBrowser().CanGoBack)
            {
                Chrom.GetBrowser().GoBack();
            }
        }

        public void GoForward()
        {
            if (Chrom.GetBrowser().CanGoForward)
            {
                Chrom.GetBrowser().GoForward();
            }
        }


    }
}
