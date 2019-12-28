using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    public List<A00> A00s;
    public List<B00> B00s;

    public int iterations = 1;

    void Start()
    { 
        
    }

    void Update()
    {
        for(int iter = 0; iter < iterations; iter++)
        {
            foreach(Species s in A00s)
                s.Simulate();
            foreach (Species s in B00s)
                s.Simulate();
        }
    }
}
