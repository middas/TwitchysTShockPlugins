using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Terraria;
using TShockAPI;
using TShockAPI.Net;
using System.IO.Streams;

namespace QuestSystemLUA
{
    public delegate bool QGetDataHandlerDelegate(QGetDataHandlerArgs args);
    public class QGetDataHandlerArgs : EventArgs
    {
        public TSPlayer Player { get; private set; }
        public MemoryStream Data { get; private set; }

        public Player TPlayer
        {
            get { return Player.TPlayer; }
        }

        public QGetDataHandlerArgs(TSPlayer player, MemoryStream data)
        {
            Player = player;
            Data = data;
        }
    }
    public static class QGetDataHandlers
    {
        private static Dictionary<PacketTypes, QGetDataHandlerDelegate> GetDataHandlerDelegates;

        public static void InitGetDataHandler()
        {
            GetDataHandlerDelegates = new Dictionary<PacketTypes, QGetDataHandlerDelegate>
            {
                {PacketTypes.NpcStrike, OnNpcStrike},
                {PacketTypes.Tile, HandleTile},
                {PacketTypes.TileSendSquare, HandleSendTileSquare},
                {PacketTypes.TileKill, HandleTileKill},
                {PacketTypes.ItemDrop, HandleDropItem}
            };
        }

        public static bool HandlerGetData(PacketTypes type, TSPlayer player, MemoryStream data)
        {
            QGetDataHandlerDelegate handler;
            if (GetDataHandlerDelegates.TryGetValue(type, out handler))
            {
                try
                {
                    return handler(new QGetDataHandlerArgs(player, data));
                }
                catch (Exception ex)
                {
                    Log.Error(ex.ToString());
                }
            }
            return false;
        }

        public static bool OnNpcStrike(QGetDataHandlerArgs args)
        {
            short npcid = args.Data.ReadInt16();
            int damage = args.Data.ReadByte();
            NPC npc = Main.npc[(int)npcid];
            if (npc.life - damage <= 0)
            {
                var player = QTools.GetPlayerByID(args.Player.Index);
                if (player.AwaitingKill && !player.KillNames.Contains(npc.name))
                    player.KillNames.Add(npc.name);
            }
            return false;
        }

        private static bool HandleTile(QGetDataHandlerArgs args)
        {
            byte type = args.Data.ReadInt8();
            int x = args.Data.ReadInt32();
            int y = args.Data.ReadInt32();
            byte tiletype = args.Data.ReadInt8();
            var player = QTools.GetPlayerByID(args.Player.Index);
            if (player.AwaitingQRName)
            {
                player.AwaitingQRName = false;
                if (QTools.InQuestRegion(x, y) == null)
                    args.Player.SendMessage("Tile is not in any Quest Region", Color.Yellow);
                else
                    args.Player.SendMessage("Quest Region Name: " + QTools.InQuestRegion(x, y), Color.Yellow);
            }
            if (player.AwaitingHitCoords)
            {
                player.TSPlayer.SendMessage("X:" + x + ", Y:" + y);
                args.Player.SendTileSquare(x, y);
                player.AwaitingHitCoords = false;
                return true;
            }
            return false;
        }

        private static bool HandleTileKill(QGetDataHandlerArgs args)
        {
            int x = args.Data.ReadInt32();
            int y = args.Data.ReadInt32();
            var player = QTools.GetPlayerByID(args.Player.Index);
            if (player.AwaitingQRName)
            {
                player.AwaitingQRName = false;
                if (QTools.InQuestRegion(x, y) == null)
                    args.Player.SendMessage("Tile is not in any Quest Region", Color.Yellow);
                else
                    args.Player.SendMessage("Quest Region Name: " + QTools.InQuestRegion(x, y), Color.Yellow);
            }
            if (player.AwaitingHitCoords)
            {
                player.TSPlayer.SendMessage("X:" + x + ", Y:" + y);
                args.Player.SendTileSquare(x, y);
                player.AwaitingHitCoords = false;
                return true;
            }
            return false;
        }

        private static bool HandleSendTileSquare(QGetDataHandlerArgs args)
        {
            short size = args.Data.ReadInt16();
            int x = args.Data.ReadInt32();
            int y = args.Data.ReadInt32();
            var player = QTools.GetPlayerByID(args.Player.Index);
            if (player.AwaitingHitCoords)
            {
                player.TSPlayer.SendMessage("X:" + x + ", Y:" + y);
                args.Player.SendTileSquare(x, y);
                player.AwaitingHitCoords = false;
                return true;
            }
            return false;
        }

        private static bool HandleLiquidSet(QGetDataHandlerArgs args)
        {
            int x = args.Data.ReadInt32();
            int y = args.Data.ReadInt32();
            var player = QTools.GetPlayerByID(args.Player.Index);
            if (player.AwaitingHitCoords)
            {
                player.TSPlayer.SendMessage("X:" + x + ", Y:" + y);
                args.Player.SendTileSquare(x, y);
                player.AwaitingHitCoords = false;
                return true;
            }
            return false;
        }

        private static bool HandleDropItem(QGetDataHandlerArgs args)
        {
            var player = QTools.GetPlayerByID(args.Player.Index);
            var reader = new BinaryReader(args.Data);
            var id = reader.ReadInt16();
            var posx = reader.ReadSingle();
            var posy = reader.ReadSingle();
            var velx = reader.ReadSingle();
            var vely = reader.ReadSingle();
            var stack = reader.ReadByte();

            var itemnamebytes = new byte[args.Data.Length];
            reader.Read(itemnamebytes, 0, (int)(args.Data.Length));
            reader.Close();
            List<byte> finalbytelist = new List<byte>();

            foreach (byte by in itemnamebytes)
            {
                if (by != 0)
                    finalbytelist.Add(by);
            }

            var itemname = System.Text.Encoding.ASCII.GetString(finalbytelist.ToArray());
            var item = new Item();
            item.SetDefaults(itemname);

            foreach (AwaitingItem aitem in player.AwaitingItems)
            {
                if (aitem.AwaitingItemName == itemname)
                {
                    aitem.AwaitingAmount -= stack;

                    if (aitem.AwaitingAmount < 0)
                    {
                        if (Math.Abs(aitem.AwaitingAmount) > 1)
                            player.TSPlayer.SendMessage(string.Format("Returning {0} {1}'s", Math.Abs(aitem.AwaitingAmount), itemname));
                        else
                            player.TSPlayer.SendMessage(string.Format("Returning {0} {1}", Math.Abs(aitem.AwaitingAmount), itemname));

                        player.TSPlayer.GiveItem(item.type, item.name, item.width, item.width, Math.Abs(aitem.AwaitingAmount));
                        player.AwaitingItems.Remove(aitem);
                        return true;
                    }
                    else if (aitem.AwaitingAmount > 0)
                    {
                        if (Math.Abs(aitem.AwaitingAmount) > 1)
                            player.TSPlayer.SendMessage(string.Format("Drop another {0} {1}'s, to continue", Math.Abs(aitem.AwaitingAmount), itemname));
                        else
                            player.TSPlayer.SendMessage(string.Format("Drop {0} {1}, to continue", Math.Abs(aitem.AwaitingAmount), itemname));
                        return true;
                    }
                    else
                    {
                        if (stack > 1)
                            player.TSPlayer.SendMessage(string.Format("You dropped {0} {1}'s", stack, itemname));
                        else
                            player.TSPlayer.SendMessage(string.Format("You dropped {0} {1}", stack, itemname));

                        player.AwaitingItems.Remove(aitem);
                        return true;
                    }
                }
            }

            return false;
        }
    }
}