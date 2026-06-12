using UnityEngine;

namespace Ceduc.Simulators.Shared
{
    public static class TestSharedUtils
    {
        public static void LogMessage(string message)
        {
            Debug.Log(message);
        }

        public static void LogWarning(string message)
        {
            Debug.LogWarning(message);
        }

        public static void LogError(string message) 
        { 
            Debug.LogError(message); 
        }

    }

}