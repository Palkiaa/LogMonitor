using System;
using UnityEngine;

namespace LogMonitor
{
    [Serializable]
    public class LogMonitorMessage
    {
        public string Key;
        public object Value;
        public double? LifeTimeInSeconds;

        public Component Context;

        private readonly double _birthTime;

        public bool Expired => LifeTimeInSeconds.HasValue && LifeTimeInSeconds != 0 && _birthTime + LifeTimeInSeconds < Time.timeAsDouble;

        public LogMonitorMessage(double birthTime)
        {
            _birthTime = birthTime;
        }
    }
}