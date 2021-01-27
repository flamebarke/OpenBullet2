﻿using OpenBullet2.Models.Debugger;
using RuriLib.Logging;
using RuriLib.Models.Blocks;
using RuriLib.Services;
using System.Collections.Generic;

namespace OpenBullet2.Services
{
    public class VolatileSettingsService
    {
        public DebuggerOptions DebuggerOptions { get; set; }
        public BotLogger DebuggerLog { get; set; }
        public List<BlockDescriptor> RecentDescriptors { get; set; }
        public bool ConfigsDetailedView { get; set; } = false;

        public VolatileSettingsService(RuriLibSettingsService ruriLibSettings)
        {
            DebuggerOptions = new(ruriLibSettings);
            DebuggerLog = new();
            RecentDescriptors = new();
        }

        public void AddRecentDescriptor(BlockDescriptor descriptor)
        {
            if (RecentDescriptors.Contains(descriptor))
            {
                RecentDescriptors.Remove(descriptor);
            }

            RecentDescriptors.Insert(0, descriptor);
        }
    }
}
