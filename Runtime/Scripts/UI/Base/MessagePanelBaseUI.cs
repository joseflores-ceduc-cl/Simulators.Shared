using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Ceduc.SWMiningTruck
{

    public enum UIMessageSeverity
    {
        Info, // 10AAE7
        Warning, // F7E64A
        Error, // F74D4A
        Critical //  FF0500
    }

    public class MessagePanelBaseUI : MonoBehaviour
    {
        [SerializeField] GameObject panel;
        [SerializeField] TMP_Text messageText;
        [SerializeField] Image barImage;
        [SerializeField] Color importantTextColor = Color.red;
        [SerializeField] ColorByMessageSeverity[] barColorByMessageSeverities;

        Dictionary<UIMessageSeverity, Color> barColorByMessageSeverityDict = new();

        public void Init()
        {
            InitDictionaries();
        }

        private void InitDictionaries()
        {
            foreach (var colorBySeverity in barColorByMessageSeverities)
            {
                barColorByMessageSeverityDict[colorBySeverity.severity] = colorBySeverity.color;
            }
        }

        public void Show()
        {
            panel.SetActive(true);
        }

        public void Hide()
        {
            panel.SetActive(false);
        }

        public void SetMessageText(string text, UIMessageSeverity severity = UIMessageSeverity.Info, string data = null)
        {
            var barColor = barColorByMessageSeverityDict[severity];
            barImage.color = barColor;

            // Validar formato de texto
            string message = text;
            if (!string.IsNullOrEmpty(data))
            {
                // El texto que viene en data se marca con color "importante"
                string colorHex = ColorUtility.ToHtmlStringRGB(importantTextColor);
                string dataWithColor = $"<color=#{colorHex}>{data}</color>";
                message = string.Format(message, dataWithColor);
            }

            messageText.text = message;
        }

        [System.Serializable]
        public struct ColorByMessageSeverity
        {
            public UIMessageSeverity severity;
            public Color color;
        }

    }

}