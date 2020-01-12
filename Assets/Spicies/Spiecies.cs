using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Species : MonoBehaviour
{
    [SerializeField]
    protected Main main;
    public float radius;


    protected virtual void Die()
    {
        main.buffer_remove[GetType()].Add(this);
        Destroy(gameObject);
        enabled = false;
    }
    public virtual void Simulate(float dt)
    {

    }

    public virtual float Eaten(float amount)
    {
        return 0;
    }

}
