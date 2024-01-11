using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Axishot : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject temp;
    void Start()
    {
        temp = FindObjectOfType<PlayerController>().gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(transform.position, temp.transform.position - transform.position, Color.green);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            Physics.gravity = transform.forward * Physics.gravity.magnitude;
        }
    }
}
