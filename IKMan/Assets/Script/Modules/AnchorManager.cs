using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnchorManager : MonoBehaviour
{
    public GameObject ragdoll;
    public GameObject footSpotPrefab;
    public float distanceFromCenter = 0.3f;

    internal Transform[] spots;

    public static float[] clockAngel = new float[13];

    // Start is called before the first frame update
    static AnchorManager() {
        for (int i = 1; i <= 12; ++i) {
            clockAngel[i] = i * 30f;
        }
    }

    void Start()
    {
        // Create the parent object at the current position
        GameObject footSpots = new GameObject("FootSpots");
        footSpots.transform.SetParent(ragdoll.transform);

        footSpots.transform.position = transform.position;

        spots = new Transform[12];

        // Create 12 prefabs around the parent object
        for (int i = 1; i <= 12; i++)
        {
            // Calculate the position of the prefab based on the clock position
            float angle = i * 30f * Mathf.Deg2Rad;
            float x = Mathf.Sin(angle) * distanceFromCenter;
            float z = Mathf.Cos(angle) * distanceFromCenter;
            Vector3 prefabPosition = footSpots.transform.position + new Vector3(x, 0f, z);

            // Instantiate the prefab at the calculated position
            GameObject prefab = Instantiate(footSpotPrefab, prefabPosition, Quaternion.identity);
            prefab.transform.SetParent(footSpots.transform);
            spots[i - 1] = prefab.transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
