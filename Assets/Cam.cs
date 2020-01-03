using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cam : MonoBehaviour
{
    public GameObject target;
    public float posLerp=0.99f, rotLerp=0.99f;
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, target.transform.position, posLerp);
        transform.rotation = Quaternion.Lerp(transform.rotation, target.transform.rotation, rotLerp);
    }
}
