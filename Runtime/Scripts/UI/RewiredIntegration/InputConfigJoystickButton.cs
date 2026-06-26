using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Ceduc.SWMiningTruck
{
    public enum JoystickButtonUIMode { DisplayOnly, Remapping }

    public class InputConfigJoystickButton : MonoBehaviour
    {
        [SerializeField] Color pressedBackgroundColor = Color.green;
        [SerializeField] Color pressedTextColor = Color.cyan;
        [SerializeField] ButtonBaseUI buttonBaseUI;
        [SerializeField] TMP_Text nameText;

        public event System.Action<int> OnClicked;

        ColorBlock initialColorBlock;
        JoystickButtonUIMode currentMode;
        int buttonIndex;

        public void Init(int index)
        {
            buttonBaseUI.Init();
            buttonIndex = index;
            nameText.text = $"{buttonIndex + 1}";
            buttonBaseUI.onClick.AddListener(() => OnClicked?.Invoke(buttonIndex));

            initialColorBlock = buttonBaseUI.colors;
            SetMode(JoystickButtonUIMode.DisplayOnly);
            SetIdleColors();
        }

        public void SetMode(JoystickButtonUIMode mode)
        {
            currentMode = mode;
            buttonBaseUI.interactable = mode == JoystickButtonUIMode.Remapping;
        }

        public void SetIdleColors()
        {            
            buttonBaseUI.colors = initialColorBlock;

            switch (currentMode)
            {
                case JoystickButtonUIMode.DisplayOnly:
                    nameText.color = buttonBaseUI.DisabledTextColor;
                    break;
                case JoystickButtonUIMode.Remapping:
                    nameText.color = buttonBaseUI.NormalTextColor;
                    break;
                default:
                    break;
            }
        }

        public void SetPressedColors()
        {
            buttonBaseUI.colors = initialColorBlock;
            var colors = buttonBaseUI.colors;
            switch (currentMode)
            {
                case JoystickButtonUIMode.DisplayOnly:
                    colors.disabledColor = pressedBackgroundColor;
                    break;
                case JoystickButtonUIMode.Remapping:
                    colors.normalColor = pressedBackgroundColor;
                    break;
                default:
                    break;
            }

            buttonBaseUI.colors = colors;
            nameText.color = pressedTextColor;
        }        

    }

}