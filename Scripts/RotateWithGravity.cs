using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateWithGravity : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Rotate towards correct gravity
        if (Vector3.Angle(-transform.up, Physics.gravity) > 0.01f)
        {
            Vector3 downVector = Vector3.RotateTowards(-transform.up, Physics.gravity, Mathf.PI * 2 * Time.deltaTime, 0);
            transform.rotation = Quaternion.LookRotation(Vector3.back, -downVector);
        }
    }
}
