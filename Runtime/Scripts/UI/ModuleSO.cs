using UnityEngine;
namespace Ceduc.SWMiningTruck
{
    [CreateAssetMenu(fileName = "Module_", menuName = "CEDUC/Module")]
    public class ModuleSO : ScriptableObject {
        public string title;
        [TextArea] public string description;
        public Sprite image;
        public string sceneName;
    }
}