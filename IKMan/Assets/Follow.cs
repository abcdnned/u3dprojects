using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    [SerializeField] Transform target;

    // Update is called once per frame
    void Update()
    {
        Vector3 tp = new Vector3(transform.position.x, transform.position.y, target.position.z);
        transform.position = tp;
    }
}
