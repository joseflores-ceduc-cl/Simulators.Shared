using System.Globalization;
using System.IO;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Collections;
using TMPro;

namespace Ceduc.SWMiningTruck
{
    public static class CommonUtils
    {
        public static float SignedAngleInRange180(float angle)
        {
            float newAngle = angle;

            if (newAngle > 180f)
            {
                newAngle -= 360f;
            }

            return newAngle;
        }

        public static string GetHierarchyPath(GameObject obj)
        {
            string path = obj.name;
            Transform current = obj.transform.parent;
            while (current != null)
            {
                path = current.name + "/" + path;
                current = current.parent;
            }
            return path;
        }

        public static string FormatFloat(float value, int decimals = 1)
        {
            string format = "F" + decimals;
            return value.ToString(format, CultureInfo.InvariantCulture);
        }

        public static bool TryParseFloat(string input, out float result)
        {
            return float.TryParse(input, NumberStyles.Float, CultureInfo.InvariantCulture, out result);
        }

        public static string FloatToStringInvariant(float value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        public static string FloatToStringInvariant(float value, string format)
        {
            return value.ToString(format, CultureInfo.InvariantCulture);
        }

        public static string[] GetFileList(string dir, string extension)
        {
            return Directory.GetFiles(dir, extension)
                .Select(Path.GetFileNameWithoutExtension)
                .ToArray();
        }

        public static T LoadFile<T>(string filePathWithExtension) where T : struct
        {
            if (File.Exists(filePathWithExtension))
            {
                string json = File.ReadAllText(filePathWithExtension);
                return JsonUtility.FromJson<T>(json);
            }

            Debug.LogWarning("Archivo de configuración no encontrado: " + filePathWithExtension);
            return default;

        }

        public static void SaveFile(object obj, string fullPath)
        {
            string json = JsonUtility.ToJson(obj, true);            
            File.WriteAllText(fullPath, json);            
        }

        public static void SaveData(string key, string value)
        {
            PlayerPrefs.SetString(key, value);
            PlayerPrefs.Save();
        }

        public static void DeleteData(string key)
        {
            PlayerPrefs.DeleteKey(key);
            PlayerPrefs.Save();
            Debug.Log($"Data deleted from PlayerPrefs: {key}");
        }

        public static string LoadData(string key, string defaultValue = "")
        {
            return PlayerPrefs.GetString(key, defaultValue);
        }

        public static void EnsureDirectoryExists(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);                
            }
        }

        public static List<string> GetFoldersByFolderPrefix(string path, string folderPattern)
        {

            string pattern = $@"^{Regex.Escape(folderPattern)}.*$";
            if (!Directory.Exists(path))
            {
                return null;
            }

            Regex regex = new Regex(pattern);

            return Directory.GetDirectories(path)
                .Where(folderPath =>
                {
                    string folderName = Path.GetFileName(folderPath);
                    return regex.IsMatch(folderName);
                })
                .OrderByDescending(folderPath => Directory.GetCreationTime(folderPath)).ToList();
        }

        public static Renderer[] GetRenderersWithFilters(GameObject target, bool filterParticles = true, bool filterTriggers = true)
        {
            Renderer[] renderers = target.GetComponentsInChildren<Renderer>()
                .Where(r => (!filterParticles || (filterParticles && r.GetComponent<ParticleSystem>() == null))
                             &&
                             (!filterTriggers || (filterTriggers && (r.GetComponent<Collider>() == null || !r.GetComponent<Collider>().isTrigger)))
                ).ToArray();

            return renderers;
        }

        public static Bounds GetCombinedRendererBounds(GameObject target, bool resetTransform = true)
        {
            var renderers = GetRenderersWithFilters(target);
            Quaternion initialRotation = target.transform.rotation;
            Vector3 initialPosition = target.transform.position;

            if (resetTransform)
            {
                // Nos interesa el bound del objeto en posición normal, sin considerar sus rotaciones actuales.
                target.transform.rotation = Quaternion.identity;
                target.transform.position = Vector3.zero;
            }            

            if (renderers.Length == 0)
            {
                // No hay renderers, devolvemos un bounds vacío en la posición del objeto.
                return new Bounds(target.transform.position, Vector3.zero);
            }

            Bounds combinedBounds = renderers[0].bounds;

            for (int i = 1; i < renderers.Length; i++)
            {                
                combinedBounds.Encapsulate(renderers[i].bounds);
            }

            if (resetTransform)
            {
                target.transform.rotation = initialRotation;
                target.transform.position = initialPosition;
            }
            
            return combinedBounds;
        }


        public static Vector3[] GetVerticesFromAABB(Vector3 min, Vector3 max)
        {
            return new Vector3[]
            { 
                new Vector3(min.x, min.y, min.z), // 0
                new Vector3(min.x, min.y, max.z), // 1
                new Vector3(min.x, max.y, min.z), // 2
                new Vector3(min.x, max.y, max.z), // 3
                new Vector3(max.x, min.y, min.z), // 4
                new Vector3(max.x, min.y, max.z), // 5
                new Vector3(max.x, max.y, min.z), // 6
                new Vector3(max.x, max.y, max.z)  // 7
            };
        }

        public static bool IsPointInsideBoxCollider(BoxCollider box, Vector3 worldPoint)
        {
            // Convertir el punto al espacio local del collider
            Vector3 localPoint = box.transform.InverseTransformPoint(worldPoint);

            // Obtener centro y tamaño del box en espacio local
            Vector3 localCenter = box.center;
            Vector3 halfSize = box.size * 0.5f;

            // Comparar coordenadas locales contra los límites del box
            return Mathf.Abs(localPoint.x - localCenter.x) <= halfSize.x &&
                   Mathf.Abs(localPoint.y - localCenter.y) <= halfSize.y &&
                   Mathf.Abs(localPoint.z - localCenter.z) <= halfSize.z;
        }

        public static string ToIso8601DateTime(DateTime dateTime)
        {
            return dateTime.ToString("s");
        }

        public static int GetClosestIndex(float[] array, float value)
        {
            if (array == null || array.Length == 0)
            {
                Debug.LogError($"CommonUtils.GetClosestIndex: El array no puede ser nulo ni vacío: {nameof(array)}.");
                return -1;
            }

            int closestIndex = 0;
            float smallestDifference = Mathf.Abs(array[0] - value);

            for (int i = 1; i < array.Length; i++)
            {
                float difference = Mathf.Abs(array[i] - value);
                if (difference < smallestDifference)
                {
                    smallestDifference = difference;
                    closestIndex = i;
                }
            }

            return closestIndex;
        }

        public static float RemapRange(float value, float min, float max, float newMin, float newMax)
        {
            float t = Mathf.InverseLerp(min, max, value);
            float newValue = Mathf.Lerp(newMin, newMax, t);
            return newValue;
        }

        public static bool HasFocus(Selectable selectable)
        {
            return EventSystem.current.currentSelectedGameObject == selectable.gameObject;
        }

        public static void SetFocus(Selectable selectable)
        {
            EventSystem.current.SetSelectedGameObject(selectable.gameObject);
        }

        public static string TypeOfRigidbody(Rigidbody rb)
        {
            return rb.isKinematic ? "Kinematic" : "Dynamic";
        }

        public static void SetFirstLevelChildrenActive(GameObject parent, bool active)
        {
            if (parent == null)
                return;

            Transform parentTransform = parent.transform;

            int childCount = parentTransform.childCount;

            for (int i = 0; i < childCount; i++)
            {
                Transform child = parentTransform.GetChild(i);
                child.gameObject.SetActive(active);
            }
        }

        public static void ExitApplication()
        {
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false; // Solo para editor
#endif
        }

        public static void ResetInputField(TMP_InputField inputField)
        {
            inputField.SetTextWithoutNotify(string.Empty);
            inputField.caretPosition = 0;
            inputField.selectionAnchorPosition = 0;
            inputField.selectionFocusPosition = 0;
            inputField.ForceLabelUpdate();
        }

        public static void InvokeAfterFrames(MonoBehaviour owner, int framesToWait, Action callback)
        {
            owner.StartCoroutine(InvokeAfterFrames_CR(framesToWait, callback));
        }

        private static IEnumerator InvokeAfterFrames_CR(int framesToWait, Action callback)
        {
            framesToWait = Mathf.Max(1, framesToWait);
            for (int i = 0; i < framesToWait; i++)
            {
                yield return null;
            }

            callback?.Invoke();
        }

        public static string ToTitleCase(string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            // Convertir todo a minúsculas primero
            text = text.ToLower();

            // Aplicar formato Title Case
            TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
            string result = textInfo.ToTitleCase(text);
            return result;
        }

        public static void SetBackgroundColor(Selectable target, Color color)
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

        public static void DestroyAllChildren(Transform parent)
        {
            foreach (Transform child in parent)
            {
                GameObject.Destroy(child.gameObject);
            }
        }

        public static void SwapUI(GameObject toActivate, GameObject toDeactivate)
        {
            if (toActivate != null && !toActivate.activeSelf)
            {
                toActivate.SetActive(true);
            }

            if (toDeactivate != null && toDeactivate.activeSelf)
            {
                toDeactivate.SetActive(false);
            }
        }




    }

}