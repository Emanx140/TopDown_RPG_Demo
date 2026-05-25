using UnityEngine;

namespace TopDownRPG.Infrastructure
{
    public class UnityGameLogger : IGameLogger
    {
        public void Log(string message)
        {
            Debug.Log(message);
        }

        public void Warning(string message)
        {
            Debug.LogWarning(message);
        }

        public void Error(string message)
        {
            Debug.LogError(message);
        }
    }
}
