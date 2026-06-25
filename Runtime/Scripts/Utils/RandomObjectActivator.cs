using UnityEngine;

[System.Serializable]
public class RandomObjectEntry
{
    public GameObject target;
    [Range(0f, 100f)] public float probability = 1f;
}

public class RandomObjectActivator : MonoBehaviour
{
    [Header("Objects List and their probabilities")]
    public RandomObjectEntry[] objects;

    private void Awake()
    {
        if (objects == null || objects.Length == 0) return;

        foreach (var entry in objects)
        {
            if (entry.target != null)
                entry.target.SetActive(false);
        }


        float total = 0f;
        foreach (var entry in objects)
            total += Mathf.Max(0, entry.probability);

        if (total <= 0f) return;

        float randomPoint = Random.value * total;
        float cumulative = 0f;

        foreach (var entry in objects)
        {
            cumulative += Mathf.Max(0, entry.probability);
            if (randomPoint <= cumulative)
            {
                if (entry.target != null)
                    entry.target.SetActive(true);
                break;
            }
        }
    }
}
