using System;
using System.Collections.Generic;
using System.Reflection;
using System.Drawing;
using Terraria;
using Hooks;
using TShockAPI;
using TShockAPI.DB;
using System.ComponentModel;

namespace PluginTemplate
{
    [APIVersion(1, 8)]
    public class PluginTemplate : TerrariaPlugin
    {
        public override string Name
        {
            get { return "PluginTemplate"; }
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
            GameHooks.Update += OnUpdate;
            GameHooks.Initialize += OnInitialize;
            NetHooks.GreetPlayer += OnGreetPlayer;
            ServerHooks.Leave += OnLeave;
            ServerHooks.Chat += OnChat;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                GameHooks.Update -= OnUpdate;
                GameHooks.Initialize -= OnInitialize;
                NetHooks.GreetPlayer -= OnGreetPlayer;
                ServerHooks.Leave -= OnLeave;
                ServerHooks.Chat -= OnChat;
            }
            base.Dispose(disposing);
        }

        public PluginTemplate(Main game)
            : base(game)
        {
        }

        public void OnInitialize()
        {
            bool say = false;

            foreach (Group group in TShock.Groups.groups)
            {
                if (group.Name != "superadmin")
                {
                    if (group.HasPermission("say"))
                        say = true;
                }
            }

            List<string> permlist = new List<string>();
            if (!say)
                permlist.Add("say");
            TShock.Groups.AddPermissions("trustedadmin", permlist);

            Commands.ChatCommands.Add(new Command("say", Say, "say"));
        }

        public void OnUpdate()
        {
        }

        public void OnGreetPlayer(int who, HandledEventArgs e)
        {
            TShock.Players[who].SendMessage("Message", Color.Black);
        }

        public void OnLeave(int ply)
        {
        }

        public void OnChat(messageBuffer msg, int ply, string text, HandledEventArgs e)
        {
        }

        public static void Say(CommandArgs args)
        {
            if (args.Parameters.Count > 0)
            {
                string text = "";

                foreach (string word in args.Parameters)
                {
                    text = text + word + " ";
                }

                Tools.Broadcast("<Server>: " + text, Color.Yellow);
            }
        }
    }
}