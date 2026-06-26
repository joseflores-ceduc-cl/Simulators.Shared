using Rewired;
using Rewired.UI.ControlMapper;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Rewired.UI.ControlMapper.ControlMapper;

namespace Ceduc.SWMiningTruck
{
    public class InputConfigPanelJoystickButtons : MonoBehaviour
    {
        [SerializeField] GameObject panel;
        [SerializeField] Transform buttonsPanel;
        [SerializeField] TMP_Text titleText;
        [SerializeField] TMP_Text subTitleText;
        [SerializeField] string titleBaseName = "Botones del control {0}";
        [SerializeField] string subTitleBaseName = "Seleccione un botón para asignarlo a {0}";
        [SerializeField] InputConfigJoystickButton joystickButtonPrefab;
        [SerializeField] int maxButtons = 32;
        [SerializeField] ButtonBaseUI cancelButton;

        ControlMapper controlMapper;
        List<InputConfigJoystickButton> inputConfigJoystickButton = new();
        System.Action<int> controlMapperCallback;
        System.Action controlMapperCancelCallback;

        private void Awake()
        {
            controlMapper = transform.GetComponentInParent<ControlMapper>(); // Se usa para obtener el nombre del joystick actual.
            CreateAllButtons(maxButtons);
            Hide();
            subTitleText.gameObject.SetActive(false);
            cancelButton.gameObject.SetActive(false);
            cancelButton.onClick.AddListener(() => CancelSelection(true));
        }

        private void OnEnable()
        {
            InputConfigButtonPressNotifier.OnButtonDown += OnButtonDown;
            InputConfigButtonPressNotifier.OnButtonUp += OnButtonUp;
            InputConfigButtonPressNotifier.OnResetButtons += OnResetButtons;
            controlMapper.OnInputFieldSelectionButtonStarted += OnInputFieldSelectionButtonStarted;

            InputConfigRenameController.OnJoystickNameChanged += OnJoystickNameChanged;
            InputConfigRenameController.OnJoystickNameReseted += OnJoystickNameReseted;
        }
        
        private void OnDisable()
        {
            InputConfigButtonPressNotifier.OnButtonDown -= OnButtonDown;
            InputConfigButtonPressNotifier.OnButtonUp -= OnButtonUp;
            InputConfigButtonPressNotifier.OnResetButtons -= OnResetButtons;
            controlMapper.OnInputFieldSelectionButtonStarted -= OnInputFieldSelectionButtonStarted;
            InputConfigRenameController.OnJoystickNameChanged -= OnJoystickNameChanged;
            InputConfigRenameController.OnJoystickNameReseted -= OnJoystickNameReseted;
        }

        private void OnButtonDown(ButtonData buttonData)
        {
            if (buttonData.buttonIndex > inputConfigJoystickButton.Count - 1)
                return;

            if (buttonData.controllerId != controlMapper.CurrentJoystick.id)
                return;

            inputConfigJoystickButton[buttonData.buttonIndex].SetPressedColors();
        }

        private void OnButtonUp(ButtonData buttonData)
        {
            if (buttonData.buttonIndex > inputConfigJoystickButton.Count - 1)
                return;

            if (buttonData.controllerId != controlMapper.CurrentJoystick.id)
                return;

            inputConfigJoystickButton[buttonData.buttonIndex].SetIdleColors();
        }

        private void OnResetButtons()
        {
            ResetPanel();
            Show();
        }

        private void OnInputFieldSelectionButtonStarted(InputMapping inputMapping, System.Action<int> callback, System.Action cancelCallback)
        {
            // Activar los botones del panel: dejarlos interactivos (cambiar su modo)
            foreach (var button in inputConfigJoystickButton)
            {
                button.SetMode(JoystickButtonUIMode.Remapping);
            }

            subTitleText.gameObject.SetActive(true);
            subTitleText.text = string.Format(subTitleBaseName, inputMapping.actionName);
            cancelButton.gameObject.SetActive(true);
            controlMapperCallback = callback;
            controlMapperCancelCallback = cancelCallback;
        }

        private void OnJoystickNameChanged(Joystick joystick, string name)
        {
            if (joystick.id != controlMapper.CurrentJoystick.id)
                return;

            titleText.text = string.Format(titleBaseName, controlMapper.GetCombinedJoystickName(joystick));
        }

        private void OnJoystickNameReseted(Joystick joystick)
        {
            if (joystick.id != controlMapper.CurrentJoystick.id)
                return;
            SetJoystickNameInTitleText();
        }

        void CancelSelection(bool callCallback = true)
        {
            foreach (var button in inputConfigJoystickButton)
            {
                button.SetMode(JoystickButtonUIMode.DisplayOnly);
            }

            subTitleText.gameObject.SetActive(false);
            cancelButton.gameObject.SetActive(false);

            if (callCallback)
            {
                controlMapperCancelCallback?.Invoke();
            }
        }

        void ResetPanel()
        {
            SetJoystickNameInTitleText();
            int buttonCount = Mathf.Min(maxButtons, controlMapper.CurrentJoystick.buttonCount);

            DeactivateAllButtons();

            for (int i = 0; i < buttonCount; i++)
            {
                inputConfigJoystickButton[i].gameObject.SetActive(true);
            }

            foreach (var button in inputConfigJoystickButton)
            {
                button.SetIdleColors();
            }
        }

        void SetJoystickNameInTitleText()
        {
            string joystickName = controlMapper.GetCombinedJoystickName(controlMapper.CurrentJoystick);
            titleText.text = string.Format(titleBaseName, joystickName);
        }

        void CreateAllButtons(int buttonCount)
        {
            for (int i = 0; i < buttonCount; i++)
            {
                var button = Instantiate(joystickButtonPrefab, buttonsPanel);
                button.Init(i);
                button.OnClicked += OnJoystickButtonClicked;
                inputConfigJoystickButton.Add(button);
            }
        }

        private void OnJoystickButtonClicked(int index)
        {
            controlMapperCallback(index);
            // Al final volvemos a desactivar los botones pero no llamamos al callback de Cancelar
            CancelSelection(callCallback: false);
        }

        void DeactivateAllButtons()
        {
            foreach (var button in inputConfigJoystickButton)
            {
                button.gameObject.SetActive(false);
            }
        }

        void Show()
        {
            panel.SetActive(true);
        }

        void Hide()
        {
            panel.SetActive(false);
        }

    }

}