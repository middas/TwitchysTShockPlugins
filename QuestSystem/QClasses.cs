using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TShockAPI;

namespace QuestSystem
{
    public class QPlayer
    {
        public int Index { get; set; }
        public TSPlayer TSPlayer { get { return TShock.Players[Index]; } }
        public List<Quest> CurrentQuests = new List<Quest>();
        public bool IsLoggedIn = false;
        public bool AwaitingHitCoords;

        public QPlayer(int index, List<Quest> currentquests)
        {
            Index = index;
        }
    }

    public class StoredQPlayer
    {
        public string LoggedInName;
        public int CurrentTask;

        public StoredQPlayer(string name,int task)
        {
            LoggedInName = name;
            CurrentTask = task;
        }
    }

    public class Quest
    {
        public string Name;
        public int MaxTries;
        public string GroupRestriction;
        public List<Branch> Branches = new List<Branch>();
        public List<QuestTask> Tasks = new List<QuestTask>();
        public List<StoredQPlayer> StoredPlayers = new List<StoredQPlayer>();
        public bool ExistsInDB = false;

        public Quest(string name, int max, string group, List<QuestTask> tasks, bool ExistsInDB)
        {
            Name = name;
            MaxTries = max;
            GroupRestriction = group;
            Tasks = tasks;
        }
    }

    public class QuestTask
    {
        public string Type;
        public object[] Data;

        public QuestTask(string type, object[] data)
        {
            Type = type;
            Data = data;
        }
    }

    public class Branch
    {
        public string Name { get; set; }
        public List<QuestTask> Tasks = new List<QuestTask>();
    }

    public class Ticker
    {
        public string Name { get; set; }
        public List<QuestTask> Tasks = new List<QuestTask>();
        public int TickInterval = 1;
        public int MaxTicks = 30;
    }
}
