﻿using System.Linq;
using TS3AudioBot;
using TS3AudioBot.Plugins;
using TS3AudioBot.CommandSystem;
using TS3Client.Commands;
using TS3Client.Full;

namespace Tools
{
    public class Tools : ITabPlugin {

        public class PluginInfo {
            public static readonly string Name = typeof(PluginInfo).Namespace;
            public const string Description = "";
            public const string URL = "";
            public const string Author = "Bluscream <admin@timo.de.vc>";
            public const int Version = 1337;
        }
        private MainBot bot;
        private Ts3FullClient lib;

        public void PluginLog(Log.Level logLevel, string Message) { Log.Write(logLevel, PluginInfo.Name + ": " + Message); }

        public void Initialize(MainBot mainBot) {
            bot = mainBot;
            lib = bot.QueryConnection.GetLowLibrary<Ts3FullClient>();
            PluginLog(Log.Level.Debug, "Plugin " + PluginInfo.Name + " v" + PluginInfo.Version + " by " + PluginInfo.Author + " loaded.");

        }

        public void Dispose() {
            PluginLog(Log.Level.Debug, "Plugin " + PluginInfo.Name + " unloaded.");
        }

        [Command("isowner", "Check if you're owner")]
        public string CommandCheckOwner(ExecutionInformation info) {
            return info.HasRights("*").ToString();
        }

        [Command("tprequest", "Request Talk Power!")]
        [RequiredParameters(0)]
        public void CommandTPRequest(ExecutionInformation info, string message)
        {
            lib.Send("clientupdate", new CommandParameter("client_talk_request", 1), new CommandParameter("client_talk_request_msg", message = message ?? "") );
        }

        [Command("rawcmd")]
        [RequiredParameters(1)]
        public string CommandRawCmd(ExecutionInformation info, string cmd, params string[] cmdpara) {
            try {
                var result = lib.Send<TS3Client.Messages.ResponseDictionary>(cmd,
                    cmdpara.Select(x => x.Split(new[] { '=' }, 2)).Select(x => new CommandParameter(x[0], x[1])).Cast<ICommandPart>().ToList());
                return string.Join("\n", result.Select(x => string.Join(", ", x.Select(kvp => kvp.Key + "=" + kvp.Value))));
            } catch (TS3Client.Ts3CommandException ex) { throw new CommandException(ex.Message, CommandExceptionReason.CommandError); }
        }

        [Command("hashpassword")]
        public string CommandHashPassword(ExecutionInformation info, string pw)
        {
            return Ts3Crypt.HashPassword(pw);
        }

        [Command("ownchannel")]
        public string CommandGetOwnChannelID(ExecutionInformation info) {
            return lib.WhoAmI().ChannelId.ToString();
        }
    }
}
