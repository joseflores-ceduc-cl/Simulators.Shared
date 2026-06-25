using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ComponentFinder
{
    /// <summary>
    /// Busca y devuelve el primer componente T cuyo GameObject tenga la tag indicada.
    /// </summary>
    public static T FindFirstOnTag<T>(string tag = "Player") where T : Component
    {
        var all = UnityEngine.Object.FindObjectsByType<T>(FindObjectsSortMode.None); // UnityEngine.Object.FindObjectsOfType<T>();
        for (int i = 0; i < all.Length; i++)
        {
            if (all[i].gameObject.CompareTag(tag))
                return all[i];
        }
        return null;
    }

    /// <summary>
    /// Devuelve todos los componentes T cuyos GameObjects tengan la tag indicada.
    /// </summary>
    public static List<T> FindAllOnTag<T>(string tag = "Player") where T : Component
    {
        var list = new List<T>();
        var all = UnityEngine.Object.FindObjectsByType<T>(FindObjectsSortMode.None); //UnityEngine.Object.FindObjectsOfType<T>();
        for (int i = 0; i < all.Length; i++)
        {
            if (all[i].gameObject.CompareTag(tag))
                list.Add(all[i]);
        }
        return list;
    }

    /// <summary>
    /// Devuelve el primer componente T que cumpla el predicado (puedes inspeccionar propiedades del componente).
    /// </summary>
    public static T FindFirstWhere<T>(Func<T, bool> predicate) where T : Component
    {
        var all = UnityEngine.Object.FindObjectsByType<T>(FindObjectsSortMode.None); // UnityEngine.Object.FindObjectsOfType<T>();
        for (int i = 0; i < all.Length; i++)
        {
            if (predicate(all[i]))
                return all[i];
        }
        return null;
    }

    /// <summary>
    /// Devuelve todas las instancias de MonoBehaviour que implementan la interfaz especificada
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static List<T> FindInterfacesOfType<T>() where T : class
    {
        return UnityEngine.Object.FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
            .OfType<T>()
            .ToList();
    }
}
