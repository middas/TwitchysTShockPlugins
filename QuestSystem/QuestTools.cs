using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Terraria;
using Hooks;
using TShockAPI;
using TShockAPI.DB;

namespace QuestSystem
{
    public class QTools
    {
        public static bool ReadQuestFile(string file, out Quest Quest)
        {
            Quest = null;

            try
            {
                List<string> lines = File.ReadAllLines(file).ToList<string>();
                List<QuestTask> Tasks = new List<QuestTask>();
                string Name = "";
                int MaxTries = 0;
                string GroupRestriction = "";

                //Starting info
                foreach (string line in lines)
                {
                    if (line.StartsWith("Name"))
                        Name = line.Split(':')[1];
                    if (line.StartsWith("MaxTries"))
                        MaxTries = Int32.Parse(line.Split(':')[1]);
                    if (line.StartsWith("GroupRestriction"))
                        GroupRestriction = line.Split(':')[1];
                    if (line.StartsWith("###"))
                        break;
                }

                if (Name == "" || MaxTries == 0 || GroupRestriction == "")
                {
                    Log.Error("Incorrect quest format in file: " + file);
                    return false;
                }

                lines.RemoveAt(0);
                lines.RemoveAt(0);
                lines.RemoveAt(0);
                lines.RemoveAt(0);

                foreach (string line in lines)
                {
                    if (line.StartsWith("goto"))
                    {
                        List<string> args = line.Split(' ').ToList<string>();
                        args.RemoveAt(0); //Remove param

                        int x = 0;
                        int y = 0;
                        int radius = 0;

                        foreach (string arg in args)
                        {
                            if (arg.ToLower().StartsWith("x"))
                            {
                                x = Int32.Parse(arg.Split(':')[1]);
                            }
                            if (arg.ToLower().StartsWith("y"))
                            {
                                y = Int32.Parse(arg.Split(':')[1]);
                            }
                            if (arg.ToLower().StartsWith("radius"))
                            {
                                radius = Int32.Parse(arg.Split(':')[1]);
                            }
                        }

                        Tasks.Add(new QuestTask("goto", new object[] { x, y, radius }));
                    }

                    if (line.StartsWith("kill"))
                        Tasks.Add(new QuestTask("kill", new object[] { Tools.GetNPCById(Int32.Parse(line.Split(':')[1])) }));

                    if (line.StartsWith("private"))
                        Tasks.Add(new QuestTask("private", new object[] { line.Split(':')[1] }));

                    if (line.StartsWith("reward"))
                        Tasks.Add(new QuestTask("reward", new object[] { Int32.Parse(line.Split(':')[1]) }));

                    if (line.StartsWith("give"))
                        Tasks.Add(new QuestTask("give", new object[] { Tools.GetItemById(Int32.Parse(line.Split(':')[1])) }));

                    if (line.StartsWith("spawn"))
                    {
                        List<string> args = line.Split(' ').ToList<string>();
                        args.RemoveAt(0); //Remove param

                        int id = 0;
                        int x = 0;
                        int y = 0;

                        foreach (string arg in args)
                        {
                            if (arg.ToLower().StartsWith("id"))
                            {
                                id = Int32.Parse(arg.Split(':')[1]);
                            }
                            if (arg.ToLower().StartsWith("x"))
                            {
                                x = Int32.Parse(arg.Split(':')[1]);
                            }
                            if (arg.ToLower().StartsWith("y"))
                            {
                                y = Int32.Parse(arg.Split(':')[1]);
                            }
                        }

                        Tasks.Add(new QuestTask("spawn", new object[] { id, x, y }));
                    }

                    if (line.StartsWith("delwall"))
                    {
                        List<string> args = line.Split(' ').ToList<string>();
                        args.RemoveAt(0); //Remove param

                        int x = 0;
                        int y = 0;

                        foreach (string arg in args)
                        {
                            if (arg.ToLower().StartsWith("x"))
                            {
                                x = Int32.Parse(arg.Split(':')[1]);
                            }
                            if (arg.ToLower().StartsWith("y"))
                            {
                                y = Int32.Parse(arg.Split(':')[1]);
                            }
                        }

                        Tasks.Add(new QuestTask("delwall", new object[] { x, y }));
                    }

                    if (line.StartsWith("deltile"))
                    {
                        List<string> args = line.Split(' ').ToList<string>();
                        args.RemoveAt(0); //Remove param

                        int x = 0;
                        int y = 0;

                        foreach (string arg in args)
                        {
                            if (arg.ToLower().StartsWith("x"))
                            {
                                x = Int32.Parse(arg.Split(':')[1]);
                            }
                            if (arg.ToLower().StartsWith("y"))
                            {
                                y = Int32.Parse(arg.Split(':')[1]);
                            }
                        }

                        Tasks.Add(new QuestTask("deltile", new object[] { x, y }));
                    }

                    if (line.StartsWith("delboth"))
                    {
                        List<string> args = line.Split(' ').ToList<string>();
                        args.RemoveAt(0); //Remove param

                        int x = 0;
                        int y = 0;

                        foreach (string arg in args)
                        {
                            if (arg.ToLower().StartsWith("x"))
                            {
                                x = Int32.Parse(arg.Split(':')[1]);
                            }
                            if (arg.ToLower().StartsWith("y"))
                            {
                                y = Int32.Parse(arg.Split(':')[1]);
                            }
                        }

                        Tasks.Add(new QuestTask("delall", new object[] { x, y }));
                    }

                    if (line.StartsWith("changetile"))
                    {
                        List<string> args = line.Split(' ').ToList<string>();
                        args.RemoveAt(0); //Remove param

                        byte id = 0;
                        int x = 0;
                        int y = 0;

                        foreach (string arg in args)
                        {
                            if (arg.ToLower().StartsWith("id"))
                            {
                                id = byte.Parse(arg.Split(':')[1]);
                            }
                            if (arg.ToLower().StartsWith("x"))
                            {
                                x = Int32.Parse(arg.Split(':')[1]);
                            }
                            if (arg.ToLower().StartsWith("y"))
                            {
                                y = Int32.Parse(arg.Split(':')[1]);
                            }
                        }

                        Tasks.Add(new QuestTask("changetile", new object[] { id, x, y }));
                    }

                    if (line.StartsWith("changewall"))
                    {
                        List<string> args = line.Split(' ').ToList<string>();
                        args.RemoveAt(0); //Remove param

                        byte id = 0;
                        int x = 0;
                        int y = 0;

                        foreach (string arg in args)
                        {
                            if (arg.ToLower().StartsWith("id"))
                            {
                                id = byte.Parse(arg.Split(':')[1]);
                            }
                            if (arg.ToLower().StartsWith("x"))
                            {
                                x = Int32.Parse(arg.Split(':')[1]);
                            }
                            if (arg.ToLower().StartsWith("y"))
                            {
                                y = Int32.Parse(arg.Split(':')[1]);
                            }
                        }

                        Tasks.Add(new QuestTask("changewall", new object[] { id, x, y }));
                    }
                }

                Quest = new Quest(Name, MaxTries, GroupRestriction, Tasks, false);
                return true;
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return false;
            }
        }

        public static bool IsTaskComplete(Quest Quest, QuestTask Task, QPlayer Player)
        {
            bool taskcomplete = false;

            if (Task.Type == "give") //Complete it
            {
                Item item = (Item)Task.Data[0];
                Player.TSPlayer.GiveItem(item.type, item.name, item.width, item.height, 1);
                taskcomplete = true;
            }

            if (Task.Type == "goto")
            {
                int x = (int)Task.Data[0];
                int y = (int)Task.Data[1];
                int radius = (int)Task.Data[2];

                Rectangle rec = new Rectangle(x - radius, y - radius, radius * 2, radius * 2);
                Rectangle playerrec = new Rectangle((int)Player.TSPlayer.X / 16, (int)Player.TSPlayer.Y / 16, 1, 1);

                if (rec.Intersects(playerrec))
                {
                    taskcomplete = true;
                }
            }

            if (Task.Type == "private")
            {
                string priv = (string)Task.Data[0];

                Player.TSPlayer.SendMessage(priv);
                taskcomplete = true;
            }

            if (Task.Type == "broadcast")
            {
                string broadcast = (string)Task.Data[0];

                Tools.Broadcast(broadcast);
                taskcomplete = true;
            }

            if (Task.Type == "spawn")
            {
                int id = (int)Task.Data[0];
                int x = (int)Task.Data[1];
                int y = (int)Task.Data[2];

                NPC npc = Tools.GetNPCById(id);
                TSPlayer.Server.SpawnNPC(npc.type, npc.name, 1, x, y, 0, 0);

                taskcomplete = true;
            }

            if (Task.Type == "delwall")
            {
                int x = (int)Task.Data[0];
                int y = (int)Task.Data[1];

                TileDeleteWall(x, y);

                taskcomplete = true;
            }

            if (Task.Type == "deltile")
            {
                int x = (int)Task.Data[0];
                int y = (int)Task.Data[1];

                TileDeleteTile(x, y);

                taskcomplete = true;
            }

            if (Task.Type == "delboth")
            {
                int x = (int)Task.Data[0];
                int y = (int)Task.Data[1];

                TileDeleteBoth(x, y);

                taskcomplete = true;
            }

            if (Task.Type == "changetile")
            {
                byte id = (byte)Task.Data[0];
                int x = (int)Task.Data[1];
                int y = (int)Task.Data[2];

                TileEdit(x, y, id);

                taskcomplete = true;
            }

            if (Task.Type == "changewall")
            {
                byte id = (byte)Task.Data[0];
                int x = (int)Task.Data[1];
                int y = (int)Task.Data[2];

                WallEdit(x, y, id);

                taskcomplete = true;
            }

            if (taskcomplete)
            {
                UpdateQuestTask(Quest, Player);
                return true;
            }

            return false;
        }

        public static void UpdateQuestTask(Quest Quest, QPlayer Player)
        {
            int storedid = -1;
            storedid = QTools.GetStoredPlayerByIdentification(Quest, Player);

            Quest.StoredPlayers[storedid].CurrentTask++;

            if (Quest.StoredPlayers[storedid].CurrentTask == Quest.Tasks.Count)
            {
                Quest.StoredPlayers[storedid].CurrentTask = 0;
                Quest.StoredPlayers.RemoveAt(storedid);
                Player.CurrentQuests.Remove(Quest);
                Player.TSPlayer.SendMessage("Completed the Quest: " + Quest.Name);
            }

            UpdateQuestInDB(Quest);
        }

        public static void UpdateQuestStatus(Quest Quest, QPlayer Player)
        {
            int storedid = -1;

            storedid = QTools.GetStoredPlayerByIdentification(Quest, Player);

            Player.TSPlayer.SendMessage(Quest.StoredPlayers[storedid].CurrentTask.ToString());
        }

        public static void UpdateQuestInDB(Quest Quest)
        {
            List<SqlValue> wheres = new List<SqlValue>();
            wheres.Add(new SqlValue("QuestName", "'" + Quest.Name + "'"));
            List<string> plyitems = new List<string>();
            string plydb = "";

            foreach (StoredQPlayer ply in Quest.StoredPlayers)
            {
                plyitems.Add(string.Join(":", ply.LoggedInName, ply.CurrentTask));
            }

            for (int i = 0; i < plyitems.Count; i++)
            {
                if (i < plyitems.Count - 1)
                    plydb = plydb + plyitems[i] + ",";
                else
                    plydb = plydb + plyitems[i];
            }

            List<SqlValue> values = new List<SqlValue>();
            values.Add(new SqlValue("PlayerDatabase", "'" + plydb + "'"));
            QuestMain.SQLEditor.UpdateValues("QuestSystem", values, wheres);
        }

        public static QPlayer GetPlayerByID(int id)
        {
            QPlayer player = null;
            foreach (QPlayer ply in QuestMain.Players)
            {
                if (ply.Index == id)
                    return ply;
            }
            return player;
        }

        public static Quest GetQuestByName(string name)
        {
            foreach (Quest q in QuestMain.QuestPool)
            {
                if (q.Name == name)
                    return q;
            }
            return null;
        }

        public static int GetStoredPlayerByIdentification(Quest Quest, QPlayer Player)
        {
            if (Player.TSPlayer.IsLoggedIn)
            {
                for (int i = 0; i < Quest.StoredPlayers.Count; i++)
                    if (Quest.StoredPlayers[i].LoggedInName == Player.TSPlayer.UserAccountName)
                        return i;
            }
            else
            {
                for (int i = 0; i < Quest.StoredPlayers.Count; i++)
                    if (Quest.StoredPlayers[i].LoggedInName == Player.TSPlayer.IP)
                        return i;
            }
            return -1;
        }

        public static List<Quest> GetMyQuests(QPlayer Player)
        {
            List<Quest> Quests = new List<Quest>();

            foreach (Quest quest in QuestMain.QuestPool)
            {
                foreach (StoredQPlayer stored in quest.StoredPlayers)
                {
                    if (Player.TSPlayer.IsLoggedIn)
                    {
                        if (stored.LoggedInName == Player.TSPlayer.UserAccountName)
                        {
                            Player.CurrentQuests.Add(quest);
                            break;
                        }
                    }
                    else
                    {
                        if (stored.LoggedInName == Player.TSPlayer.IP)
                        {
                            Player.CurrentQuests.Add(quest);
                            break;
                        }
                    }
                }
            }

            return Quests;
        }

        public static void TileEdit(int x, int y, byte type)
        {
            if (type < 253)
            {
                Main.tile[x, y].type = type;
                Main.tile[x, y].active = true;
                Main.tile[x, y].liquid = 0;
                Main.tile[x, y].skipLiquid = true;
                Main.tile[x, y].frameNumber = 0;
                Main.tile[x, y].frameX = -1;
                Main.tile[x, y].frameY = -1;
            }
            else if (type == 253)
            {
                Main.tile[x, y].active = false;
                Main.tile[x, y].skipLiquid = false;
                Main.tile[x, y].lava = false;
                Main.tile[x, y].liquid = 255;
                Main.tile[x, y].checkingLiquid = false;
            }
            else if (type == 254)
            {
                Main.tile[x, y].active = false;
                Main.tile[x, y].skipLiquid = false;
                Main.tile[x, y].lava = true;
                Main.tile[x, y].liquid = 255;
                Main.tile[x, y].checkingLiquid = false;
            }
            if ((Main.tile[x, y].type == 53) || (Main.tile[x, y].type == 253) || (Main.tile[x, y].type == 254))
                WorldGen.SquareTileFrame(x, y, false);
        }

        public static void WallEdit(int x, int y, byte wall)
        {
            if (wall < 255)
            {
                Main.tile[x, y].wall = wall;
            }
        }

        public static void TileDeleteBoth(int x, int y)
        {
            Main.tile[x, y].active = false;
            Main.tile[x, y].wall = 0;
            Main.tile[x, y].skipLiquid = true;
            Main.tile[x, y].liquid = 0;
        }

        public static void TileDeleteWall(int x, int y)
        {
            Main.tile[x, y].wall = 0;
        }

        public static void TileDeleteTile(int x, int y)
        {
            Main.tile[x, y].active = false;
            Main.tile[x, y].skipLiquid = true;
            Main.tile[x, y].liquid = 0;
        }
    }
}
