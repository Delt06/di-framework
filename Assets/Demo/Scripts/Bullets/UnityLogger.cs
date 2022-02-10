using UnityEngine;

namespace Demo.Scripts.Bullets
{
    public class UnityLogger : ILogger
    {
        public void Log(string message) => Debug.Log(message);
    }
}