/*
    TO FIX: Server bans new admins because of "cheating"
*/

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

namespace Oxide.Plugins
{
    [Info("XPA", "crester", "1.0.0")]
    [Description("XenPare Admin Authorization")]

    public class XPA : RustPlugin 
    {
        public static XPA _plugin;
        void Init()
        {
            _plugin = this;
        }

        string groupID = "<Steam Group ID>";
        void OnPlayerConnected(BasePlayer pl)
        {
            string id = pl.UserIDString;
            string name = pl.displayName;
            webrequest.Enqueue("http://steamcommunity.com/gid/" + groupID + "/memberslistxml/?xml=1", null, (code, response) =>
            {
                bool isAdmin = response.Contains(id);
                if (isAdmin && (!pl.IsAdmin))
                {
                    pl.SetPlayerFlag(BasePlayer.PlayerFlags.IsAdmin, true);
                };
            }, this, RequestMethod.GET);
        }
    }
}
