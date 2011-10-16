using System;
using System.Collections.Generic;
using System.Drawing;
using Terraria;
using Hooks;
using TShockAPI;
using TShockAPI.DB;
using System.ComponentModel;
using System.IO;
using MySql.Data.MySqlClient;

namespace QuestSystem
{
    [APIVersion(1, 8)]
    public class QuestMain : TerrariaPlugin
    {
        public static List<QPlayer> Players = new List<QPlayer>();
        public static List<Quest> QuestPool = new List<Quest>();
        public static SqlTableEditor SQLEditor;
        public static SqlTableCreator SQLWriter;

        public override string Name
        {
            get { return "QuestSystem"; }
        }

        public override string Author
        {
            get { return "Created by Twitchy"; }
        }

        public override string Description
        {
            get { return ""; }
        }

        public override void Initialize()
        {
            NetHooks.GetData += GetData;
            GameHooks.Update += OnUpdate;
            NetHooks.GreetPlayer += OnGreetPlayer;
            ServerHooks.Leave += OnLeave;
            GameHooks.Initialize += OnInitialize;
            GameHooks.Update += OnUpdate;

            Commands.ChatCommands.Add(new Command(QCommands.GetCoords, "getcoords"));
            Commands.ChatCommands.Add(new Command(QCommands.HitCoords, "hitcoords"));
            Commands.ChatCommands.Add(new Command(QCommands.GiveQuest, "giveq"));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                GameHooks.Update -= OnUpdate;
                NetHooks.GetData -= GetData;
                NetHooks.GreetPlayer -= OnGreetPlayer;
                ServerHooks.Leave -= OnLeave;
                GameHooks.Initialize -= OnInitialize;
                GameHooks.Update -= OnUpdate;
            }
            base.Dispose(disposing);
        }

        public QuestMain(Main game)
            : base(game)
        {
            Order = -10; //We want all the stuff.. :P :P
        }
        public void OnInitialize()
        {
            GetDataHandlers.InitGetDataHandler();

            if (!Directory.Exists("Quests"))
                Directory.CreateDirectory("Quests");                

            string[] filePaths = Directory.GetFiles("Quests", "*.txt");

            foreach (string path in filePaths)
            {
                Quest q = null;
                QTools.ReadQuestFile(path, out q);

                if (q != null)
                    QuestPool.Add(q);
            }

            SQLEditor = new SqlTableEditor(TShock.DB, TShock.DB.GetSqlType() == SqlType.Sqlite ? (IQueryBuilder)new SqliteQueryCreator() : new MysqlQueryCreator());
            SQLWriter = new SqlTableCreator(TShock.DB, TShock.DB.GetSqlType() == SqlType.Sqlite ? (IQueryBuilder)new SqliteQueryCreator() : new MysqlQueryCreator());

            var table = new SqlTable("QuestSystem",
                new SqlColumn("QuestName", MySqlDbType.Text) { Unique = true },
                new SqlColumn("PlayerDatabase", MySqlDbType.Text)
            );
            SQLWriter.EnsureExists(table);

            for (int i = 0; i < SQLEditor.ReadColumn("QuestSystem", "QuestName", new List<SqlValue>()).Count; i++)
            {
                string name = SQLEditor.ReadColumn("QuestSystem", "QuestName", new List<SqlValue>())[i].ToString();
                string[] list = SQLEditor.ReadColumn("QuestSystem", "PlayerDatabase", new List<SqlValue>())[i].ToString().Split(',');
                List<StoredQPlayer> items = new List<StoredQPlayer>();

                foreach (string playerdat in list)
                {
                    if(playerdat!="")
                        items.Add(new StoredQPlayer(playerdat.Split(':')[0], int.Parse(playerdat.Split(':')[1])));
                }

                foreach (Quest q in QuestPool)
                {
                    if (q.Name == name)
                    {
                        q.StoredPlayers = items;
                        q.ExistsInDB = true;
                        break;
                    }
                }
            }

            foreach (Quest q in QuestPool)
            {
                if (!q.ExistsInDB)
                {
                    List<SqlValue> values = new List<SqlValue>();
                    values.Add(new SqlValue("QuestName", "'" + q.Name + "'"));
                    values.Add(new SqlValue("PlayerDatabase", "''"));
                    SQLEditor.InsertValues("QuestSystem", values);
                    q.ExistsInDB = true;
                }
            }
        }

        public void OnUpdate()
        {
            foreach (QPlayer player in Players)
            {
                if (!player.IsLoggedIn && player.TSPlayer.IsLoggedIn)
                {
                    QTools.GetMyQuests(player);
                    player.IsLoggedIn = true;
                }

                lock (player.CurrentQuests)
                {
                    foreach (Quest quest in player.CurrentQuests)
                    {
                        int storedid = -1;

                        storedid = QTools.GetStoredPlayerByIdentification(quest, player);

                        if (QTools.IsTaskComplete(quest, quest.Tasks[quest.StoredPlayers[storedid].CurrentTask], player)) //This quest is complete, wait for next update to check again.
                            break;
                    }
                }
            }
        }

        public void OnGreetPlayer(int who, HandledEventArgs e)
        {
            lock (Players)
                Players.Add(new QPlayer(who, new List<Quest>()));

            QTools.GetMyQuests(QTools.GetPlayerByID(who));
        }

        public void OnLeave(int ply)
        {
            lock (Players)
            {
                for (int i = 0; i < Players.Count; i++)
                {
                    if (Players[i].Index == ply)
                    {
                        Players.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        private void GetData(GetDataEventArgs e)
        {
            PacketTypes type = e.MsgID;
            var player = TShock.Players[e.Msg.whoAmI];

            if (player == null)
            {
                e.Handled = true;
                return;
            }

            if (!player.ConnectionAlive)
            {
                e.Handled = true;
                return;
            }

            using (var data = new MemoryStream(e.Msg.readBuffer, e.Index, e.Length))
            {
                try
                {
                    if (GetDataHandlers.HandlerGetData(type, player, data))
                        e.Handled = true;
                }
                catch (Exception ex)
                {
                    Log.Error(ex.ToString());
                }
            }
        }
    }
}