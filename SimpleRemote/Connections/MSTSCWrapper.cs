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
            mstsc.Connect();
        }

        private void MSTSCWrapper_Resize(object sender, EventArgs e)
        {
            mstsc.Height = Height;
            mstsc.Width = Width;
        }
    }
}
