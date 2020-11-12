using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfWebBrowser.ViewModel.Base;

namespace WpfWebBrowser
{
    public class MainDesignModel : MainViewModel
    {
        public static MainDesignModel Instance => _Instance ?? (_Instance = new MainDesignModel());

        private static MainDesignModel _Instance;

        public MainDesignModel() : base()
        {
            Title = "奇悦浏览器";
        }
    }
}
