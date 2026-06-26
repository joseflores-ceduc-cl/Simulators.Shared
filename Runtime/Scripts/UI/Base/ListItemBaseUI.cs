using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Ceduc.SWMiningTruck
{
    public class ListItemBaseUI : MonoBehaviour
    {
        [SerializeField] Selectable target;
        [SerializeField] TMP_Text descriptionText;
        [SerializeField] TMP_Text valueText;
        [SerializeField] TextColors valueColors = new TextColors() {
            normal = Color.white,
            alert = Color.white,
        };

        public void SetBackgroundColor(Color color)
        {
            var colors = target.colors;

            if (target.interactable)
            {
                colors.normalColor = color;
            }
            else 
            { 
                colors.disabledColor = color;
            }

            target.colors = colors;
        }

        public void SetDescription(string description)
        {
            descriptionText.text = description;
        }

        public void SetValueText(string value, bool isAlert = false)
        {
            valueText.text = value;
            valueText.color = isAlert ? valueColors.alert : valueColors.normal;
        }

        [System.Serializable]
        public struct TextColors
        {
            public Color normal;
            public Color alert;
        }

    }

}