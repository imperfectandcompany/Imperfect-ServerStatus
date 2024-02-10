﻿using CounterStrikeSharp.API.Core;
using IGDiscord.Models.MessageInfo;

namespace IGDiscord.Models
{
    public class Config : BasePluginConfig
    {
        /// <summary>
        /// Config version number
        /// </summary>
        public override int Version { get; set; } = 1;

        /// <summary>
        /// Server status message information
        /// </summary>
        public required ServerStatusMessageInfo ServerStatusMessage { get; set; }
    }
}