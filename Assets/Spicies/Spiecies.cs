using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Species : MonoBehaviour
{
    public virtual void HelloWorld(Main main)
    {

    }
    public virtual void Die()
    {
        DestroyImmediate(gameObject);
    }
    public virtual void Simulate()
    {

    }
}
