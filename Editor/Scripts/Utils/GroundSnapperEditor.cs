using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Ceduc.SWMiningTruck
{
    [CustomEditor(typeof(GroundSnapper))]
    [CanEditMultipleObjects]
    public class GroundSnapperEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            // Dibuja el inspector por defecto
            DrawDefaultInspector();

            if (GUILayout.Button("Snap to Ground"))
            {
                Undo.IncrementCurrentGroup(); // opcional, agrupa la operación
                int group = Undo.GetCurrentGroup();

                foreach (var t in targets)
                {
                    GroundSnapper snapper = (GroundSnapper)t;
                    if (snapper == null) continue;

                    // Registrar el Transform (o el componente que se modifica) para Undo
                    Undo.RecordObject(snapper.transform, "Snap to Ground");

                    // Llamada que realiza el cambio (se asume que solo cambia transform.position)
                    snapper.TrySnapToGround();

                    // Marcar objeto y escena como modificados para que Unity lo guarde en la sesión
                    EditorUtility.SetDirty(snapper);
                    EditorSceneManager.MarkSceneDirty(snapper.gameObject.scene);
                }

                Undo.CollapseUndoOperations(group); // opcional, colapsa en una sola entrada Undo
            }
        }
 
    }

}