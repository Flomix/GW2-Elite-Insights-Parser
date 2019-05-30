﻿using LuckParser.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.ParseModels
{
    public class BuffApplyEvent : AbstractBuffEvent
    {
        public BuffApplyEvent(CombatItem evtcItem, AgentData agentData, long offset) : base(evtcItem, offset)
        {
            By = agentData.GetAgentByInstID(evtcItem.SrcMasterInstid > 0 ? evtcItem.SrcMasterInstid : evtcItem.SrcInstid, evtcItem.LogTime);
            To = agentData.GetAgentByInstID(evtcItem.DstInstid, evtcItem.LogTime);
        }

        public BuffApplyEvent(AgentItem by, AgentItem to, long time, int duration, long buffID) : base(duration, buffID, time)
        {
            By = by;
            To = to;
        }

        public override bool IsBoonSimulatorCompliant(long fightEnd)
        {
            return BuffID != BoonHelper.NoBuff;
        }

        public override void TryFindSrc(ParsedLog log)
        {
        }

        public override void UpdateSimulator(BoonSimulator simulator)
        {
            simulator.Add(Value, By, Time);
        }
    }
}
