using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading;
using TShockAPI;
using Terraria;

namespace QuestSystemLUA
{
    public class QFunctions
    {
        public static bool GotoXY(int x, int y, QPlayer Player, int radius = 1)
        {
            Rectangle rec, playerrec;
            rec = new Rectangle(x - radius, y - radius, radius * 2, radius * 2);
            playerrec = new Rectangle((int)Player.TSPlayer.X / 16, (int)Player.TSPlayer.Y / 16, 1, 1);
            return rec.Intersects(playerrec);
        }

        public static void Give(string name, QPlayer Player)
        {
            Item item = Tools.GetItemByName(name)[0];
            //Player.TSPlayer.GiveItem(item.type, item.name, item.width, item.height, 1);
        }

        public static void Private(string message, QPlayer Player)
        {
            Player.TSPlayer.SendMessage(message);
        }

        public static void Broadcast(string message)
        {
            Tools.Broadcast(message);
        }

        public static void SpawnMob(string name, int x, int y)
        {
            NPC npc = Tools.GetNPCByName(name)[0];
            TSPlayer.Server.SpawnNPC(npc.type, npc.name, 1, x, y + 3, 0, 0);
        }

        public static void TileEdit(int x, int y, string tile)
        {
            byte type;

            if (QTools.GetTileTypeFromName(tile, out type))
            {
                if (type < 253)
                {
                    Main.tile[x, y].type = (byte)type;
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
                QTools.UpdateTile(x, y);
            }
            else
                throw new Exception("Invalid Tile Name");
        }

        public static void WallEdit(int x, int y, string wall)
        {
            byte type;

            if (QTools.GetTileTypeFromName(wall, out type))
            {
                if (type < 255)
                {
                    Main.tile[x, y].wall = (byte)type;
                }
                QTools.UpdateTile(x, y);
            }
            else
                throw new Exception("Invalid Wall Name");
        }

        public static void DeleteBoth(int x, int y)
        {
            Main.tile[x, y].active = false;
            Main.tile[x, y].wall = 0;
            Main.tile[x, y].skipLiquid = true;
            Main.tile[x, y].liquid = 0;
            QTools.UpdateTile(x, y);
        }

        public static void DeleteWall(int x, int y)
        {
            Main.tile[x, y].wall = 0;
            QTools.UpdateTile(x, y);
        }

        public static void DeleteTile(int x, int y)
        {
            Main.tile[x, y].active = false;
            Main.tile[x, y].skipLiquid = true;
            Main.tile[x, y].liquid = 0;
            QTools.UpdateTile(x, y);
        }

        public static void Kill(string name, QPlayer Player)
        {
            Player.AwaitingKill = true;
            while (!Player.KillNames.Contains(name)) { Thread.Sleep(1); }
            Player.KillNames.Remove(name);
            Player.AwaitingKill = false;
        }

        public static void Sleep(int time)
        {
            Thread.Sleep(time);
        }

        public static void Teleport(int x, int y, QPlayer Player)
        {
            Player.TSPlayer.Teleport(x, y + 3);
        }

        public static void ClearKillList(QPlayer Player)
        {
            lock (Player.KillNames)
                Player.KillNames.Clear();
        }

        public static void GoCollectItem(string name, int amount, QPlayer Player)
        {
            int count;
            do
            {
                count = 0;
                try
                {
                    foreach (Item slot in Player.Inventory)
                    {
                        if (slot != null)
                            if (slot.name.ToLower() == name.ToLower())
                                count += slot.stack;
                    }
                }
                catch(Exception e)
                {
                    Log.Info(e.Message);
                }
                Thread.Sleep(1);
            }
            while (count < amount);
        }

        public static void TakeItem(string qname, string iname, int amt, QPlayer Player)
        {
            if (amt > 0)
            {
                var aitem = new AwaitingItem(qname, amt, iname);
                Player.AwaitingItems.Add(aitem);
                if (amt > 1)
                    Player.TSPlayer.SendMessage(string.Format("Please drop {0} {1}'s, The excess will be returned.", amt, iname));
                else
                    Player.TSPlayer.SendMessage(string.Format("Please drop {0} {1}, The excess will be returned.", amt, iname));
                while (Player.AwaitingItems.Contains(aitem)) { Thread.Sleep(1); }
            }
        }

        public static void PopUpSign(string text, QPlayer Player)
        {
            Main.sign[1] = new Sign();
            Main.sign[1].text = text;
            Main.sign[1].x = (int)Player.TSPlayer.X / 16;
            Main.sign[1].y = (int)Player.TSPlayer.Y / 16;
            Main.tile[Main.sign[1].x, Main.sign[1].y].type = 55;
            Main.tile[Main.sign[1].x, Main.sign[1].y].active = true;
            Player.TSPlayer.SendData(PacketTypes.Tile, "", 1, Main.sign[1].x, Main.sign[1].y, 55, 55);
            QTools.UpdateTile(Main.sign[1].x, Main.sign[1].y);
            NetMessage.SendData(47, Player.Index, -1, "", 1, 0f, 0f, 0f, 0);
        }
    }
}
