using CefSharp;
using CefSharp.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfWebBrowser.CefSharpExtend.Handler
{
    /// <summary>
    /// 鼠标拖动到网页处理类
    /// </summary>
    class DragHandler : IDragHandler
    {
        public bool OnDragEnter(IWebBrowser chromiumWebBrowser, IBrowser browser, IDragData dragData, DragOperationsMask mask)
        {
            throw new NotImplementedException();
        }

        public void OnDraggableRegionsChanged(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IList<DraggableRegion> regions)
        {
            throw new NotImplementedException();
        }
    }
}
