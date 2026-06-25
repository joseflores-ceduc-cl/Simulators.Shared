using UnityEngine;

namespace Ceduc.SWMiningTruck
{
    public class TimeScaleController : MonoBehaviour
    {
        [SerializeField] KeyCode accelerationKey = KeyCode.Home;
        [SerializeField] private float accelerationFactor = 5f;

#if UNITY_EDITOR        
        void Update()
        {
            if (Input.GetKey(accelerationKey))
            {
                Time.timeScale = accelerationFactor;
            }
            else
            {
                Time.timeScale = 1f;
            }
        }
#endif
    }

}