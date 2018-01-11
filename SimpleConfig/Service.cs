using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using SuperSocket.WebSocket;
using System.IO;

using SimpleShared.PacketHandling;

namespace SimpleConfig
{
    public class Service
    {
        private WebSocketServer appServer = new WebSocketServer();
        private IDbConnection dbConnection;

        public bool Start()
        {
            if (!Directory.Exists(Globals.AppData))
                Directory.CreateDirectory(Globals.AppData);

            appServer.NewSessionConnected += AppServer_NewSessionConnected;
            appServer.SessionClosed += AppServer_SessionClosed;
            appServer.NewMessageReceived += AppServer_NewMessageReceived;

            if (!appServer.Setup(2012))
                return false;

            if (!appServer.Start())
                return false;

            Console.WriteLine("WebSockets setup finished");

            bool newdatabase = false;
            string sqlitefile = Path.Combine(Globals.AppData, "Database.sqlite3");

            if (!File.Exists(sqlitefile))
                newdatabase = true;

            var stringbuilder = new SQLiteConnectionStringBuilder
            {
                DataSource = sqlitefile
            };

            dbConnection = new SQLiteConnection(stringbuilder.ConnectionString).OpenAndReturn();

            if (newdatabase)
                new DatabaseCreator().CreateDatabase(dbConnection);

            PacketDelegator.Init();

            return true;
        }

        public bool Stop()
        {
            appServer.Stop();

            return true;
        }

        private void AppServer_NewSessionConnected(WebSocketSession session)
        {
            Console.WriteLine("New session connected");
        }

        private void AppServer_SessionClosed(WebSocketSession session, SuperSocket.SocketBase.CloseReason value)
        {
            Console.WriteLine("Session disconnected");

            if (appServer.SessionCount == 0)
            {
                Console.WriteLine("No active sessions, exiting");
                System.Threading.Thread.Sleep(1000);
                Environment.Exit(0);
            }
        }

        private void AppServer_NewMessageReceived(WebSocketSession session, string value)
        {
            try
            {
                new PacketDelegator().DelegateMessage(value);
            }
            catch (DelegatorException)
            {
            }
        }
    }
}
