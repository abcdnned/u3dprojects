using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityController : MonoBehaviour
{
    [SerializeField] float velocity = 1;

    new private Rigidbody rigidbody;
    // Start is called before the first frame update

    private void Awake() {
        rigidbody = GetComponent<Rigidbody>();
    }
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
        
    }

    private void FixedUpdate() {
        Vector3 force = transform.forward * velocity;
        // Debug.DrawRay(transform.position, force, Color.green, 5);
        // rigidbody.velocity = force;
        rigidbody.AddForce(force, ForceMode.Acceleration);
        
    }
}
