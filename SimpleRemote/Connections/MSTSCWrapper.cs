using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
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

        Task reconnector = null;

        private AxMsRdpClientNotSafeForScripting mstsc;

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

        private void Mstsc_OnConnected(object sender, EventArgs e)
        {
            Invoke((Action)(() =>
            {
                Refresh();
            }));
        }

        private void MSTSCWrapper_Resize(object sender, EventArgs e)
        {
            mstsc.Height = Height;
            mstsc.Width = Width;

            if (reconnector == null)
            {
                reconnector = new Task(async () =>
                {
                    await Task.Delay(5000);
                    mstsc.OnDisconnected += Mstsc_OnDisconnected;
                    mstsc.Disconnect();
                    reconnector = null;
                });

                reconnector.Start();
            }
        }

        private void Mstsc_OnDisconnected(object sender, IMsTscAxEvents_OnDisconnectedEvent e)
        {
            Invoke((Action)(() =>
            {
                Controls.Remove(mstsc);
                mstsc = new AxMsRdpClientNotSafeForScripting();
                Controls.Add(mstsc);
                Connect();
            }));
            mstsc.OnDisconnected -= Mstsc_OnDisconnected;
            
        }
    }
}
