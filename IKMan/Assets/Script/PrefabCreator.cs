using UnityEngine;

public class PrefabCreator
{

    public static void SpawnDebugger(Vector3 position, string prefabName, float liveTime, float scale, Transform parent)
    {
        GameObject prefab = Resources.Load<GameObject>(prefabName);
        GameObject instance = GameObject.Instantiate(prefab, position, Quaternion.identity);
        instance.transform.localScale = new Vector3(scale, scale, scale);
        instance.transform.SetParent(parent);
        GameObject.Destroy(instance, liveTime);
    }
}