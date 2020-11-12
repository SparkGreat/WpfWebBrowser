using CefSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WpfWebBrowser.CefSharpExtend.Handler
{
    class CookieVisitor : ICookieVisitor
    {
        public List<Cookie> cookies = new List<Cookie>();
        readonly ManualResetEvent gotAllCookies = new ManualResetEvent(false);


        /// <summary>
        /// 获取cookie
        /// </summary>
        /// <param name="DomainStr">根据域名获取，如果DomainStr为空则获取所有的cookie</param>
        /// <returns></returns>
        public List<Cookie> GetCookieList(string DomainStr = "")
        {
            var visitor = new CookieVisitor();
            if (DomainStr.Length > 0)
            {
                var cookieManager = Cef.GetGlobalCookieManager();
                if (cookieManager.VisitAllCookies(visitor))
                {
                    visitor.WaitForAllCookies();
                    return visitor.cookies.Where(p => p.Domain.Contains(DomainStr)).ToList();
                }
                else
                {
                    return visitor.cookies;
                }
            }
            else
            {
                var cookieManager = Cef.GetGlobalCookieManager();
                if (cookieManager.VisitAllCookies(visitor))
                    visitor.WaitForAllCookies();
                return visitor.cookies;
            }
        }

        /// <summary>
        /// 给浏览器设置cookie
        /// </summary>
        /// <returns></returns>
        private async Task<bool> SetCookie(string domainStr, string nameStr, string valueStr, bool ishttps)
        {
            string httpStr = "http";
            if (ishttps)
            {
                httpStr = "https";
            }
            var cookieManager = Cef.GetGlobalCookieManager();
            var bol = await cookieManager.SetCookieAsync(httpStr + "://" + domainStr, new Cookie()
            {
                Domain = domainStr,
                Name = nameStr,
                Value = valueStr,
                Path = "/",
                HttpOnly = true,
                Expires = DateTime.Now.AddMinutes(1)
            });
            return bol;
        }

        public bool Visit(Cookie cookie, int count, int total, ref bool deleteCookie)
        {

            cookies.Add(cookie);
            if (count == total - 1)
                gotAllCookies.Set();
            return true;
        }

        public void WaitForAllCookies()
        {
            gotAllCookies.WaitOne();
        }

        public void Dispose()
        {
            //cookies.Clear();
        }
    }
}
