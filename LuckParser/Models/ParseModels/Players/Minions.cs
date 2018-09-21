﻿using LuckParser.Models.DataModels;
using System.Collections.Generic;
using System.Linq;

namespace LuckParser.Models.ParseModels
{
    public class Minions : List<Minion>
    {
        public readonly int InstID;
        private readonly List<DamageLog> _damageLogs = new List<DamageLog>();
        private Dictionary<ushort, List<DamageLog>> _damageLogsByDst = new Dictionary<ushort, List<DamageLog>>();
        private readonly List<CastLog> _castLogs = new List<CastLog>();
        public string Character
        {
            get
            {
                return Count > 0 ? this[0].Character : "";
            }
        }

        public Minions(int instid)
        {
            InstID = instid;
        }

        public List<DamageLog> GetDamageLogs(ushort dstFilter, ParsedLog log, long start, long end)
        {
            if (_damageLogs.Count == 0)
            {
                foreach (Minion minion in this)
                {
                    _damageLogs.AddRange(minion.GetDamageLogs(0, log, 0, log.FightData.FightDuration));
                }
                _damageLogsByDst = _damageLogs.GroupBy(x => x.DstInstId).ToDictionary(x => x.Key, x => x.ToList());
            }
            if (_damageLogsByDst.TryGetValue(dstFilter, out var list))
            {
                return list.Where(x => x.Time >= start && x.Time <= end).ToList();
            }
            return _damageLogs.Where(x => x.Time >= start && x.Time <= end).ToList();
        }

        public List<DamageLog> GetDamageLogs(List<AgentItem> redirection, ParsedLog log, long start, long end)
        {
            List<DamageLog> dls = GetDamageLogs(0, log, start, end);
            List<DamageLog> res = new List<DamageLog>();
            foreach (AgentItem a in redirection)
            {
                res.AddRange(dls.Where(x => x.DstInstId == a.InstID && x.Time >= a.FirstAware - log.FightData.FightStart && x.Time <= a.LastAware - log.FightData.FightStart));
            }
            res.Sort((x, y) => x.Time < y.Time ? -1 : 1);
            return res;
        }

        /*public List<DamageLog> getHealingLogs(ParsedLog log, long start, long end)
        {
            List<DamageLog> res = new List<DamageLog>();
            foreach (Minion minion in this)
            {
                res.AddRange(minion.getHealingLogs(log, start, end));
            }
            return res;
        }*/

        public List<CastLog> GetCastLogs(ParsedLog log, long start, long end)
        {
            if (_castLogs.Count == 0)
            {
                foreach (Minion minion in this)
                {
                    _castLogs.AddRange(minion.GetCastLogs(log, 0, log.FightData.FightDuration));
                }
            }
            return _castLogs.Where(x => x.Time >= start && x.Time <= end).ToList();
        }

    }
}
