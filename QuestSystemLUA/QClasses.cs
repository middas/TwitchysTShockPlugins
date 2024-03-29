﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using TShockAPI;
using Terraria;
using TShockAPI.DB;

namespace QuestSystemLUA
{
    public class QPlayer
    {
        public int Index { get; set; }
        public TSPlayer TSPlayer { get { return TShock.Players[Index]; } }
        public bool AwaitingHitCoords;
        public List<string> KillNames = new List<string>();
        public bool AwaitingKill = false;
        public List<AwaitingItem> AwaitingItems = new List<AwaitingItem>();
        public List<string> RunningQuests = new List<string>();
        public Item[] Inventory { get { return TSPlayer.TPlayer.inventory; } }
        public StoredQPlayer MyDBPlayer;
        public bool IsLoggedIn = false;
        public Vector2 LastTilePos = Vector2.Zero;
        public string CurQuestRegion { get; set; }
        public bool InHouse { get; set; }
        public bool AwaitingQRName = false;
        public bool AwaitingChat = false;
        public bool HideChat = false;
        public string LastChatMessage = "";
        public List<RunQuestParameters> RunningQuestThreads = new List<RunQuestParameters>();

        public QPlayer(int index)
        {
            Index = index;
        }
        public bool NewQuest(Quest q, bool skipchecks = false)
        {
            if (skipchecks || !this.RunningQuests.Contains(q.Name))
            {
                Thread t = new Thread(QTools.RunQuest);
                object parameters = new RunQuestParameters(q, this, t);
                t.Start(parameters);
                RunningQuestThreads.Add((RunQuestParameters)parameters);
                return true;
            }
            return false;
        }
    }
    public class Quest
    {
        public string Name;
        public string FilePath;
        public int MinQuestsNeeded;
        public int MaxAttemps;
        public int AmountOfPlayersAtATime;
        public bool EndOnPlayerDeath;

        public Quest(string name, string filepath, int min, int max, int players, bool endondeath)
        {
            Name = name;
            FilePath = filepath;
            MinQuestsNeeded = min;
            MaxAttemps = max;
            AmountOfPlayersAtATime = players;
        }
    }
    public class RunQuestParameters
    {
        public Quest Quest;
        public QPlayer Player;
        public Thread QThread;

        public RunQuestParameters(Quest quest, QPlayer player, Thread thread)
        {
            Quest = quest;
            Player = player;
            QThread = thread;
        }
    }
    public class AwaitingItem
    {
        public string QuestName;
        public int AwaitingAmount;
        public string AwaitingItemName;

        public AwaitingItem(string qname, int amt, string name)
        {
            QuestName = qname;
            AwaitingAmount = amt;
            AwaitingItemName = name;
        }
    }
    public class QuestRegion : Region
    {
        public string MessageOnEntry;
        public string MessageOnExit;
        public List<Quest> Quests = new List<Quest>();

        public QuestRegion(string name, List<Quest> quests, int x1, int y1, int x2, int y2, string entry, string exit)
        {
            this.Name = name;
            Quests = quests;
            this.Area = new Rectangle(x1, y1, x2 - x1, y2 - y1);
            MessageOnEntry = entry;
            MessageOnExit = exit;
        }
    }
    public class StoredQPlayer
    {
        public string LoggedInName;
        public List<QuestPlayerData> QuestPlayerData = new List<QuestPlayerData>();

        public StoredQPlayer(string name, List<QuestPlayerData> playerdata)
        {
            LoggedInName = name;
            QuestPlayerData = playerdata;
        }
    }
    public class QuestPlayerData
    {
        public string QuestName;
        public bool Complete = false;
        public int Attempts = 0;

        public QuestPlayerData(string name, bool complete, int attempts)
        {
            QuestName = name;
            Complete = complete;
            Attempts = attempts;
        }
    }
}
