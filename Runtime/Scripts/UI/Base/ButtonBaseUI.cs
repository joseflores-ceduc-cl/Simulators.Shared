using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Ceduc.SWMiningTruck
{
    public class ButtonBaseUI : Button
    {
        [SerializeField] TMP_Text label;
        [SerializeField] Color disabledTextColor = new Color32(110, 119, 129, 255);
        [SerializeField] Color normalTextColor = Color.white;

        public Color NormalTextColor => normalTextColor;
        public Color DisabledTextColor => disabledTextColor;        

        public void Init()
        {
            if (label == null)
                label = GetComponentInChildren<TMP_Text>();
        }

        public void SetText(string text)
        { 
            label.text = text; 
        }

        // Esta función se ejecuta en modo edit, por eso nos vemos obligados a crear la var serializable normalTextColor (no se puede leer en runtime desde label.color).
        protected override void DoStateTransition(SelectionState state, bool instant)
        {
            base.DoStateTransition(state, instant);

            if (label == null)
                return;

            switch (state)
            {
                case SelectionState.Disabled:
                    label.color = disabledTextColor;
                    break;
                default:
                    label.color = normalTextColor;
                    break;
            }
        }

    }

}