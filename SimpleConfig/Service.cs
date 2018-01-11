using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using SuperSocket.WebSocket;
using System.IO;

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

            if (!appServer.Setup(2012))
                return false;

            if (!appServer.Start())
                return false;

            appServer.NewMessageReceived += AppServer_NewMessageReceived;

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
