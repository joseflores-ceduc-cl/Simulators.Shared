using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

namespace Ceduc.SWMiningTruck
{
    public class ModuleCard : MonoBehaviour {
        public TextMeshProUGUI titleText;
        public TextMeshProUGUI descriptionText;
        public Image icon;
        [SerializeField] Image multiplayerIcon;
        private string sceneToLoad;
        [SerializeField] Button startButton;
        

        public void Setup(ModuleSO module, Action onClick, bool multiplayer = false) {
            multiplayerIcon.gameObject.SetActive(multiplayer);
            titleText.text = module.title;
            descriptionText.text = module.description;
            icon.sprite = module.image;
            sceneToLoad = module.sceneName;
            startButton.onClick.RemoveAllListeners();
            startButton.onClick.AddListener(() => onClick?.Invoke());
        }

        //TODO: Validar esta función LoadModuleScene: no está siendo llamada.
        public void LoadModuleScene() {            
            if (!string.IsNullOrEmpty(sceneToLoad)) {
                Debug.Log($"Cargando escena: {sceneToLoad}");
                SceneManager.LoadScene(sceneToLoad);
            }
            else {
                Debug.LogWarning("No se ha asignado una escena al módulo.");
            }
        }
    }
}