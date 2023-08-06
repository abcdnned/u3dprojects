using UnityEngine;

public class PrefabCreator
{
    internal static string prefabSavePath = "Prefabs/";

    public static string POSITION_HELPER = "PositionHelper";

    public static float DEFAULT_LIVE = 1.2f;
    public static GameObject SpawnDebugger(Vector3 position, string prefabName, float liveTime, float scale, Transform parent)
    {
        GameObject prefab = Resources.Load<GameObject>(prefabName);
        GameObject instance = GameObject.Instantiate(prefab, position, Quaternion.identity);
        instance.transform.localScale = new Vector3(scale, scale, scale);
        instance.transform.SetParent(parent);
        GameObject.Destroy(instance, liveTime);
        return instance;
    }

    public static GameObject CreatePrefab(Vector3 position, string prefabName, Transform parent = null)
    {

        GameObject prefab = Resources.Load<GameObject>(prefabSavePath + prefabName);
        GameObject instance = GameObject.Instantiate(prefab, position, Quaternion.identity);
        instance.transform.SetParent(parent);
        return instance;
    }


}