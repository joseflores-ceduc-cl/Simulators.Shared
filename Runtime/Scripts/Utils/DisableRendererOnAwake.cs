using UnityEngine;

namespace Ceduc.SWMiningTruck
{
    public class DisableRendererOnAwake : MonoBehaviour
    {
        Renderer rend;

        private void Awake()
        {
            rend = GetComponent<Renderer>();
            rend.enabled = false;
        }

        public void SetActiveRenderer(bool value)
        {
            rend.enabled = value;
        }

    }

}