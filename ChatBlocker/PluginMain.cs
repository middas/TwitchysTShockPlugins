using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Linq;
using System.Threading;
using Terraria;
using Hooks;
using TShockAPI;
using TShockAPI.DB;
using System.ComponentModel;

namespace ChatBlocker
{
    [APIVersion(1, 8)]
    public class PluginTemplate : TerrariaPlugin
    {
        public static CConfigFile CConfig { get; set; }
        internal static string CConfigPath { get { return Path.Combine(TShock.SavePath, "cconfig.json"); } }
        List<string> StartsWith = new List<string>();
        List<string> Contains = new List<string>();
        List<CommandRedirect> Redirects = new List<CommandRedirect>();

        public override string Name
        {
            get { return "ChatBlocker"; }
        }
        public override string Author
        {
            get { return "Created by Twitchy"; }
        }
        public override string Description
        {
            get { return ""; }
        }
        public override Version Version
        {
            get { return Assembly.GetExecutingAssembly().GetName().Version; }
        }

        public override void Initialize()
        {
            GameHooks.Initialize += OnInitialize;
            ServerHooks.Chat += OnChat;
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                GameHooks.Initialize -= OnInitialize;
                ServerHooks.Chat -= OnChat;
            }
            base.Dispose(disposing);
        }
        public PluginTemplate(Main game)
            : base(game)
        {
            CConfig = new CConfigFile();
            Order = 0;
        }

        public void OnInitialize()
        {
            SetupConfig();

            StartsWith = CConfig.StartsWith.Split(',').ToList<string>();
            Contains = CConfig.Contains.Split(',').ToList<string>();

            List<string> mergedcmds = CConfig.CommandsRedirect.Split(',').ToList<string>();

            foreach (string merged in mergedcmds)
            {
                Redirects.Add(new CommandRedirect(merged.ToLower()));
            }

            for (int i = 0; i < StartsWith.Count; i++)
            {
                StartsWith[i] = StartsWith[i].ToLower();
            }

            for (int i = 0; i < Contains.Count; i++)
            {
                Contains[i] = Contains[i].ToLower();
            }

            bool chatunblocked = false;

            foreach (Group group in TShock.Groups.groups)
            {
                if (group.Name != "superadmin")
                {
                    if (group.HasPermission("chatunblocked"))
                        chatunblocked = true;
                }
            }

            List<string> permlist = new List<string>();
            if (!chatunblocked)
                permlist.Add("chatunblocked");

            TShock.Groups.AddPermissions("trustedadmin", permlist);
        }

        public static TSPlayer GetTSPlayerByIndex(int index)
        {
            foreach (TSPlayer player in TShock.Players)
            {
                if (player != null && player.Index == index)
                    return player;
            }
            return null;
        }

        public void OnChat(messageBuffer msg, int ply, string text, HandledEventArgs e)
        {
            string firstword = text.Split(' ')[0];
            TSPlayer player = GetTSPlayerByIndex(ply);

            if (firstword[0] == '/')
            {
                List<string> words = firstword.Split('/').ToList<string>();

                if (words.Count > 1)
                {
                    firstword = words[1];

                        foreach (CommandRedirect cmdr in Redirects)
                    {
                        if (cmdr.NewCmd == firstword)
                        {
                            List<string> parameters = text.Split(' ').ToList<string>();
                            parameters[0] = "/" + cmdr.TShockCmd;
                            string finalstring = "";
                            int count = 0;
                            foreach (string str in parameters)
                            {
                                if (count == 0)
                                    finalstring = str;
                                else
                                    finalstring = finalstring + " " + str;

                                count++;
                            }

                            Commands.HandleCommand(player, finalstring);

                            e.Handled = true;
                        }
                    }
                }
            }

            if (!TShock.Players[ply].Group.HasPermission("chatunblocked"))
            {

                if (StartsWith.Contains(text[0].ToString().ToLower()))
                {
                    foreach (Command cmd in Commands.ChatCommands)
                    {
                        if (cmd.Name.ToLower() == firstword.ToLower())
                            return;
                    }
                    e.Handled = true;
                }

                foreach (string word in text.Split(' '))
                {
                    if (Contains.Contains(word.ToLower()))
                    {
                        TShock.Players[ply].SendMessage(CConfig.BlockedWordMessage, Color.Red);
                        e.Handled = true;
                        return;
                    }
                }
            }
        }

        public static void SetupConfig()
        {
            try
            {
                if (File.Exists(CConfigPath))
                {
                    CConfig = CConfigFile.Read(CConfigPath);
                    // Add all the missing config properties in the json file
                }
                CConfig.Write(CConfigPath);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error in config file");
                Console.ForegroundColor = ConsoleColor.Gray;
                Log.Error("Config Exception");
                Log.Error(ex.ToString());
            }
        }
    }

    public class CommandRedirect
    {
        public string NewCmd;
        public string TShockCmd;

        public CommandRedirect(string mergedcmd)
        {
            NewCmd = mergedcmd.Split('=')[0];
            TShockCmd = mergedcmd.Split('=')[1];
        }
    }

}