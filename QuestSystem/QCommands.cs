using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria;
using Hooks;
using TShockAPI;
using TShockAPI.DB;

namespace QuestSystem
{
    public class QCommands
    {
        public static void GetCoords(CommandArgs args)
        {
            int x = (int)args.Player.X / 16;
            int y = (int)args.Player.Y / 16;

            args.Player.SendMessage(string.Format("X: {0}, Y: {1}", x, y), Color.Tomato);
        }

        public static void HitCoords(CommandArgs args)
        {
            var player = QTools.GetPlayerByID(args.Player.Index);

            args.Player.SendMessage("Hit a Tile/Wall to get its Coords");
        }

        public static void GiveQuest(CommandArgs args)
        {
            int who = int.Parse(args.Parameters[0]);
            string name = args.Parameters[1];

            var q = QTools.GetQuestByName(name);
            var player = QTools.GetPlayerByID(who);
            player.CurrentQuests.Add(q);

            if (player.TSPlayer.IsLoggedIn)
            {
                q.StoredPlayers.Add(new StoredQPlayer(player.TSPlayer.UserAccountName, 0));
            }
            else
                q.StoredPlayers.Add(new StoredQPlayer(player.TSPlayer.IP, 0));

            QTools.UpdateQuestInDB(q);
        }
    }
}
