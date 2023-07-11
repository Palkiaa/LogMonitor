using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LogMonitor
{
    [CreateAssetMenu(fileName = "LogMonitorChannel", menuName = "LogMonitorChannel", order = 0)]
    public class LogMonitorChannel : ScriptableObject, ILogMonitor
    {
        private readonly Dictionary<string, LogMonitorMessage> _messages;

        private int _lastFrame;

        public LogMonitorChannel()
        {
            _messages = new Dictionary<string, LogMonitorMessage>();
        }

        public void Log(string key, object value, Component context = null)
        {
            Log(key, value, 0, context);
        }

        public void Log(string key, object value, double lifeTime, Component context = null)
        {
            _messages[key] = new LogMonitorMessage(Time.timeAsDouble)
            {
                Key = key,
                Value = value,
                Context = context,
                LifeTimeInSeconds = lifeTime
            };
        }

        public void Clear()
        {
            _messages.Clear();
        }

        public int MessageCount => _messages.Count;

        public bool AnyMessages => _messages.Any();

        public void Tick()
        {
            if (Time.frameCount == _lastFrame)
                return;
            _lastFrame = Time.frameCount;

            for (int i = 0; i < _messages.Count; i++)
            {
                var keyValuePair = _messages.ElementAt(i);
                var key = keyValuePair.Key;
                var message = keyValuePair.Value;

                if (message.Expired)
                {
                    _messages.Remove(key);
                    i--;
                }
            }
        }

        public IEnumerable<LogMonitorMessage> GetMessages()
        {
            return _messages.Values.ToList();
        }
    }
}