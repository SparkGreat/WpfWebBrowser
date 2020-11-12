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
using System.Windows.Input;
using WpfWebBrowser.CefSharpExtend.Handler;
using WpfWebBrowser.CefSharpExtend.Helper;
using WpfWebBrowser.ViewModel.Core;

namespace WpfWebBrowser.ViewModel.Base
{
    public class MainViewModel : ViewModelBase
    {
        
        List<ChromiumWebBrowser> ChromList = new List<ChromiumWebBrowser>();

        public StackPanel TabHeader { get; set; }

        /// <summary>
        /// 当前选择的浏览器
        /// </summary>
        public ChromiumWebBrowser Chrom { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title
        {
            get => Chrom.Title;
            set => SetProperty(ref _Title, value);
        }
        private string _Title = "轻悦浏览器";

        /// <summary>
        /// 地址
        /// </summary>
        public string Url
        {
            get { return FormatHelper.UrlDecode(Chrom?.Address); }
            set { }
        }
        private string _Url;

        /// <summary>
        /// 是否能后退
        /// </summary>
        public bool CanGoBack
        {
            get
            {
                if (Chrom == null || !Chrom.IsBrowserInitialized)
                {
                    return false;
                }
                else
                {
                    return Chrom.GetBrowser().CanGoBack;
                }
            }
        }

        /// <summary>
        /// 是否能前进
        /// </summary>
        public bool CanGoForward
        {
            get
            {
                if (Chrom == null || !Chrom.IsBrowserInitialized)
                {
                    return false;
                }
                else
                {
                    return Chrom.GetBrowser().CanGoForward;
                }
            }
        }

        /// <summary>
        /// 是否正在加载
        /// </summary>
        public bool IsReloading
        {
            get
            {
                if (Chrom == null || !Chrom.IsBrowserInitialized)
                {
                    return false;
                }
                else
                {
                    return Chrom.GetBrowser().IsLoading;
                }
            }
        }


        public ChromiumWebBrowser TabCreate(string address)
        {
            ChromiumWebBrowser chromium = new ChromiumWebBrowser(address);

            LifeSpanHandler lifeSpanHandler = new LifeSpanHandler();
            lifeSpanHandler.NewWindow += LifeSpanHandler_NewWindow;
            chromium.LifeSpanHandler = lifeSpanHandler;
            chromium.MenuHandler = new MenuHandler();
            chromium.DownloadHandler = new DownloadHandler();
            chromium.KeyboardHandler = new KeyboardHandler();
            //chromium.DragHandler = new DragHandler();
            chromium.LoadHandler = new LoadHandler();
            //chromium.RequestHandler = new RequestHandler();
            //chromium.DisplayHandler = new DisplayHandler();

            chromium.FrameLoadStart += Chrom_FrameLoadStart;
            chromium.FrameLoadEnd += Chrom_FrameLoadEnd;
            chromium.TitleChanged += Chrom_TitleChanged;
            chromium.AddressChanged += Chrom_AddressChanged;
            chromium.LoadingStateChanged += Chrom_LoadingStateChanged;

            ChromList.Add(chromium);

            return chromium;
        }


        #region 命令

        /// <summary>
        /// 后退
        /// </summary>
        public ICommand GoBackCommand { get; private set; }
        public void GoBack(object obj)
        {
            if (Chrom.GetBrowser().CanGoBack)
            {
                Chrom.GetBrowser().GoBack();
            }
        }

        /// <summary>
        /// 前进
        /// </summary>
        public ICommand GoForwardCommand { get; private set; }
        public void GoForward(object obj)
        {
            if (Chrom.GetBrowser().CanGoForward)
            {
                Chrom.GetBrowser().GoForward();
            }
        }

        /// <summary>
        /// 刷新
        /// </summary>
        public ICommand ReloadCommand { get; private set; }
        public void Reload(object obj)
        {
            bool.TryParse(obj?.ToString(), out bool igc);

            Chrom.GetBrowser().Reload(igc);
        }

        /// <summary>
        /// 打开调试器
        /// </summary>
        public ICommand ShowDevToolsCommand { get; private set; }
        public void ShowDevTools(object obj)
        {
            Chrom.ShowDevTools();
        }

        #endregion


        public MainViewModel()
        {
            GoBackCommand = new DelegateCommand(GoBack);
            GoForwardCommand = new DelegateCommand(GoForward);
            ReloadCommand = new DelegateCommand(Reload);
            ShowDevToolsCommand = new DelegateCommand(ShowDevTools);

            //TabHeader = new StackPanel() { Orientation = Orientation.Horizontal };

            //for (int i = 0; i < 5; i++)
            //{
            //    RadioButton rb = new RadioButton();
            //    rb.Content = i;

            //    TabHeader.Children.Add(rb);
            //}

        }



        private void LifeSpanHandler_NewWindow(object arg1, string arg2)
        {
            Application.Current.Dispatcher.BeginInvoke((Action)delegate ()
            {
                var v = arg1;
            });
        }

        public void ChromChange()
        {
            Application.Current.Dispatcher.BeginInvoke((Action)delegate ()
            {
                RaisePropertyChanged(nameof(Title));
                RaisePropertyChanged(nameof(Url));
                RaisePropertyChanged(nameof(CanGoBack));
                RaisePropertyChanged(nameof(CanGoForward));
                RaisePropertyChanged(nameof(IsReloading));
            });
        }


        public void Chrom_FrameLoadStart(object sender, FrameLoadStartEventArgs e)
        {
            ChromChange();
        }

        public void Chrom_FrameLoadEnd(object sender, FrameLoadEndEventArgs e)
        {
            ChromChange();
        }

        public void Chrom_AddressChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ChromChange();
        }

        public void Chrom_TitleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke((Action)delegate ()
            {
                RaisePropertyChanged(nameof(Title));
            });
        }

        public void Chrom_LoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        {
            ChromChange();
        }


    }
}
