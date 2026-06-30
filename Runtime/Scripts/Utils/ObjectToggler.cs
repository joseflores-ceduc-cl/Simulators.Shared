using System.Collections.Generic;
using UnityEngine;

namespace Ceduc.SWMiningTruck
{

    public class ObjectToggler : MonoBehaviour
    {
        [Tooltip("Objetos que se activarán o desactivarán")]
        public List<GameObject> objectsToToggle;

        [Tooltip("Estado que quieres aplicar: true = activar, false = desactivar")]
        public bool activate = true;

        public void ToggleObjects()
        {
            foreach (GameObject obj in objectsToToggle)
            {
                if (obj != null)
                {
                    obj.SetActive(activate);
                }
            }
        }

        public void ToggleObjects(bool active)
        {
            foreach (GameObject obj in objectsToToggle)
            {
                if (obj != null)
                {
                    obj.SetActive(active);
                }
            }
        }

        /// <summary>
        /// Esta función no usa la variable activate, sino que intercambia el valor de activeSelf.
        /// </summary>
        public void InvertActiveState()
        {
            if (objectsToToggle == null || objectsToToggle.Count == 0) return;

            foreach (var obj in objectsToToggle)
            {
                if (obj != null)
                {
                    obj.SetActive(!obj.activeSelf);
                }
            }
        }
    }

}