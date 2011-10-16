using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Terraria;
using TShockAPI;
using TShockAPI.Net;
using System.IO.Streams;

namespace QuestSystem
{
    public delegate bool GetDataHandlerDelegate(GetDataHandlerArgs args);
    public class GetDataHandlerArgs : EventArgs
    {
        public TSPlayer Player { get; private set; }
        public MemoryStream Data { get; private set; }

        public Player TPlayer
        {
            get { return Player.TPlayer; }
        }

        public GetDataHandlerArgs(TSPlayer player, MemoryStream data)
        {
            Player = player;
            Data = data;
        }
    }
    public static class GetDataHandlers
    {
        private static Dictionary<PacketTypes, GetDataHandlerDelegate> GetDataHandlerDelegates;

        public static void InitGetDataHandler()
        {
            GetDataHandlerDelegates = new Dictionary<PacketTypes, GetDataHandlerDelegate>
            {
                {PacketTypes.NpcStrike, OnNpcStrike},
                {PacketTypes.Tile, HandleTile},
                {PacketTypes.TileSendSquare, HandleSendTileSquare},
                {PacketTypes.TileKill, HandleTileKill}
            };
        }

        public static bool HandlerGetData(PacketTypes type, TSPlayer player, MemoryStream data)
        {
            GetDataHandlerDelegate handler;
            if (GetDataHandlerDelegates.TryGetValue(type, out handler))
            {
                try
                {
                    return handler(new GetDataHandlerArgs(player, data));
                }
                catch (Exception ex)
                {
                    Log.Error(ex.ToString());
                }
            }
            return false;
        }

        public static bool OnNpcStrike(GetDataHandlerArgs args)
        {
            short npcid = args.Data.ReadInt16();
            int damage = args.Data.ReadByte();
            NPC npc = Main.npc[(int)npcid];

            if (npc.life - damage <= 0)
            {
                var player = QTools.GetPlayerByID(args.Player.Index);

                foreach (Quest q in player.CurrentQuests)
                {
                    int storedid = QTools.GetStoredPlayerByIdentification(q, player);
                    if (q.Tasks[q.StoredPlayers[storedid].CurrentTask].Type == "kill")
                    {
                        object ob = q.Tasks[q.StoredPlayers[storedid].CurrentTask].Data[0];
                        var qnpc = (NPC)ob;

                        if (npc.type == qnpc.type)
                        {
                            QTools.UpdateQuestTask(q, player);
                        }
                    }
                }
            }

            return false;
        }

        private static bool HandleTile(GetDataHandlerArgs args)
        {
            byte type = args.Data.ReadInt8();
            int x = args.Data.ReadInt32();
            int y = args.Data.ReadInt32();
            byte tiletype = args.Data.ReadInt8();
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

        private static bool HandleTileKill(GetDataHandlerArgs args)
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

        private static bool HandleSendTileSquare(GetDataHandlerArgs args)
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

        private static bool HandleLiquidSet(GetDataHandlerArgs args)
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
    }
}