using CefSharp;
using CefSharp.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SimpleRemote.Config
{
    class WWW : IConfigEntry
    {
        public FrameworkElement GetElement()
        {
            var browser = new ChromiumWebBrowser();
            browser.Address = "https://google.com";
            browser.LoadingStateChanged += LoadingStateChanged;

            //RenderOptions.SetBitmapScalingMode(browser, BitmapScalingMode.HighQuality);

            return browser;
        }

        private async void LoadingStateChanged(object sender, CefSharp.LoadingStateChangedEventArgs e)
        {
            var browser = sender as ChromiumWebBrowser;
            if (browser.CanExecuteJavascriptInMainFrame && !e.IsLoading)
            {
                try
                {
                    await Task.Delay(2500);
                    // HTML Form filling authentication
                    //await browser.EvaluateScriptAsync("document.getElementById(\"ctl00_TheContentPlaceHolder_UserNameTextBox\").setAttribute(\"value\", \"username\");");
                    //await browser.EvaluateScriptAsync("document.getElementById(\"ctl00_TheContentPlaceHolder_PasswordTextBox\").setAttribute(\"value\", \"password\");");
                    //await browser.EvaluateScriptAsync("document.getElementById(\"ctl00_TheContentPlaceHolder_LoginButton\").click();");
                }
                catch { }
            }
        }
    }
}
