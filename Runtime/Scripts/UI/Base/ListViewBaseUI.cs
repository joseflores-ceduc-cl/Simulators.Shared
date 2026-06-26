using UnityEngine;

namespace Ceduc.SWMiningTruck
{
    public class ListViewBaseUI : MonoBehaviour
    {
        [SerializeField] ListItemBaseUI listItemPrefab;
        [SerializeField] Color oddColor = Color.blue;
        [SerializeField] Color evenColor = Color.white;
        [SerializeField] Transform container;

        // Edge condition: para contar la cantidad de hijos en container no sirve usar container.childCount
        // porque el Destroy se hace al final del frame: si yo en un mismo frame llamo a ClearItems y luego
        // quiero validar que no tiene hijos, debo usar otra variable.
        int childCount = 0;

        public bool IsValid()
        {
            return container != null;
        }

        public void ClearItems()
        {
            CommonUtils.DestroyAllChildren(container);
            childCount = 0;
        }

        public ListItemBaseUI AddItem()
        {
            childCount++;

            // Edge condition: si el prefab se instancia activo, se puede ver en la UI el color original del prefab
            // antes de cambiarlo al color background, como un parpadeo. Para evitarlo, lo instanciamos desactivado.
            // Esto es así porque se supone que Unity UI (Canvas) NO renderiza solo al final del frame.
            var listItem = Instantiate(listItemPrefab, container);
            listItem.gameObject.SetActive(false); // Edge condition: primero lo desactivamos.
            bool isEven = childCount % 2 == 0;
            var color = isEven ? evenColor : oddColor;
            listItem.SetBackgroundColor(color);
            listItem.gameObject.SetActive(true); // Edge condition: ahora lo activamos.
            return listItem;
        }

    }

}