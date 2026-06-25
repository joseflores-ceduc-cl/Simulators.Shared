using UnityEngine;

namespace Ceduc.SWMiningTruck
{
    public class ActivateAllMonitors : MonoBehaviour
    {
        static bool displaysActivated = false;

        private void Start()
        {
            if (displaysActivated)
                return;

            ActivateMonitors();
        }

        void ActivateMonitors()
        {
            for (int i = 1; i < Display.displays.Length; i++)
            {
                Display.displays[i].Activate();
            }
            displaysActivated = true;
        }

    }

}