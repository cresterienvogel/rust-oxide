using System.Collections.Generic;
using UnityEngine;

using System.Linq;
using System;
using System.IO;
using System.Text;
using System.IO.Compression;

using Oxide.Core.Plugins;
using Oxide.Core.Configuration;
using Oxide.Core.Libraries;

using Facepunch;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Oxide.Plugins
{
    [Info("Telegram Status", "crester", "1.0.0")]
    [Description("Telegram Status Plugin")]

    public class TelegramStatus : RustPlugin 
    {
        public static TelegramStatus _plugin;
        void Init()
        {
            _plugin = this;
        }

        string BotID = "<Bot ID>";
        string BotToken = "<Bot Token>";

        private void SendMessage(string id, string message)
        {
            webrequest.Enqueue("https://api.telegram.org/bot" + BotID + ":" + BotToken + "/sendMessage?chat_id=" + id + "&text=" + message, null, (code, response) =>
            {
                Puts("Done");
            }, this, RequestMethod.GET);            
        }

        string sleepers = "?";
        private void RefreshSleepers()
        {
            sleepers = BasePlayer.sleepingPlayerList.Count.ToString();
        }

        string online = "?/?";
        private void RefreshOnlinePlayers()
        {
            online = $"{BasePlayer.activePlayerList.Count}/{ConVar.Server.maxplayers}";
        }   
        
        void OnPlayerDisconnected(BasePlayer pl)
        {
            timer.Once(2, RefreshOnlinePlayers);
            timer.Once(2, RefreshSleepers);
        }             

        void OnPlayerConnected(BasePlayer player)
        {
            timer.Once(2, RefreshOnlinePlayers);
            timer.Once(2, RefreshSleepers);
        }
        
        string last = "0";
        void OnTick()
        {
            webrequest.Enqueue("https://api.telegram.org/bot" + BotID + ":" + BotToken + "/getUpdates?offset=-1", null, (code, response) =>
            {
                if (code == 200)
                {
                    JObject msgs = null;
                    try
                    {
                        msgs = JObject.Parse(response);
                        string msg_id = (string)msgs.SelectToken("result[0].message.message_id");
                        string chat_id = (string)msgs.SelectToken("result[0].message.chat.id");
                        string message = (string)msgs.SelectToken("result[0].message.text");

                        if (msg_id != last)
                        {
                            last = msg_id;
                            SendMessage(chat_id, "🌎 XenPare ► Rust (xenpare.net:28015): " + online + " players online and " + sleepers + " sleepers.");    
                        }              
                    }
                    catch (Exception ex)
                    {
                        Puts("Error: " + ex.Message);
                    }                    
                };        
            }, this, RequestMethod.GET);
        }
    }
}