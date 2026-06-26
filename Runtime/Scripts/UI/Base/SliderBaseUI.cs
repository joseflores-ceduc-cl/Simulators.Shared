using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Ceduc.SWMiningTruck
{
    public class SliderBaseUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Slider slider;
        [SerializeField] private TMP_Text valueText;

        [Header("Formatting")]
        [SerializeField] private int decimalPlaces = 2;
        [SerializeField] private string prefix = ""; // Ej: "Valor: ", "$"
        [SerializeField] private string suffix = ""; // Ej: " %", " m", " s"

        private void Awake()
        {
            slider.onValueChanged.AddListener(OnSliderValueChanged);
            UpdateValueText(slider.value); // Inicializar texto
        }

        private void OnSliderValueChanged(float value)
        {
            UpdateValueText(value);
        }

        private void UpdateValueText(float value)
        {
            if (valueText == null)
                return;

            string formattedValue;
            bool useWholeNumbers = slider.wholeNumbers;

            if (useWholeNumbers)
            {
                formattedValue = Mathf.RoundToInt(value).ToString();
            }
            else
            {
                formattedValue = value.ToString($"F{decimalPlaces}");
            }

            valueText.text = $"{prefix}{formattedValue}{suffix}";
        }

    }

}