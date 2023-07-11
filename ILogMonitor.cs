using UnityEngine;

namespace LogMonitor
{
    public interface ILogMonitor
    {
        public void Log(string key, object value, Component context = null);

        public void Log(string key, object value, double lifeTime, Component context = null);
    }
}