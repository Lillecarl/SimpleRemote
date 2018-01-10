using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using MSTSCLib;
using AxMSTSCLib;

namespace SimpleRemote.Connections
{
    public partial class MSTSCWrapper : UserControl, IConnection
    {
        public MSTSCWrapper()
        {
            InitializeComponent();
            mstsc = new AxMsRdpClientNotSafeForScripting();
            Controls.Add(mstsc);
        }

        private AxMsRdpClientNotSafeForScripting mstsc = null;
        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        public void Connect()
        {
            mstsc.Width = Width;
            mstsc.Height = Height;

            mstsc.CreateControl();
            mstsc.Server = "";
            mstsc.UserName = @"";
            IMsTscNonScriptable secured = (IMsTscNonScriptable)mstsc.GetOcx();
            secured.ClearTextPassword = @"";
            mstsc.AdvancedSettings2.SmartSizing = true;
            mstsc.DesktopHeight = Height;
            mstsc.DesktopWidth = Width;
            mstsc.OnConnected += Mstsc_OnConnected;
            mstsc.Connect();
        }

        private void MSTSCWrapper_Resize(object sender, EventArgs e)
        {
            cancellationTokenSource.Cancel();
            cancellationTokenSource = new CancellationTokenSource();

            mstsc.Height = Height;
            mstsc.Width = Width;

            Task.Run(async () =>
            {
                await Task.Delay(1000, cancellationTokenSource.Token);

                cancellationTokenSource.Token.ThrowIfCancellationRequested();

                mstsc.OnDisconnected += Mstsc_ResizeReconnect;
                mstsc.Disconnect();
            }, cancellationTokenSource.Token);
        }

        private void Mstsc_OnConnected(object sender, EventArgs e)
        {
            Invoke((Action)(() =>
            {
                Refresh();
            }));
        }

        private void Mstsc_ResizeReconnect(object sender, IMsTscAxEvents_OnDisconnectedEvent e)
        {
            Invoke((Action)(() =>
            {
                Controls.Remove(mstsc);
                mstsc.Dispose();
                mstsc = new AxMsRdpClientNotSafeForScripting();
                Controls.Add(mstsc);
                Connect();
            }));
        }
    }
}
