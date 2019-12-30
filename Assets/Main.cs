using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

public class Main : MonoBehaviour
{
    public Material m;
    public float hue,dHueDT;

    readonly System.Type[] allSpeciesTypes = Assembly.GetAssembly(typeof(Species)).GetTypes().Where(t => t.IsSubclassOf(typeof(Species))).ToArray();
    public Dictionary<System.Type, List<Species>> all { get; }  = new Dictionary<System.Type, List<Species>>();
    public Dictionary<System.Type, List<Species>> buffer_add    = new Dictionary<System.Type, List<Species>>();
    public Dictionary<System.Type, List<Species>> buffer_remove = new Dictionary<System.Type, List<Species>>();

    public int iterations = 1;
    public float dt = 1;

    void Start()
    {
        foreach (System.Type T in allSpeciesTypes)
        {
            all.Add(T, new List<Species>());
            buffer_add.Add(T, new List<Species>());
            buffer_remove.Add(T, new List<Species>());
        }
            
        foreach (Species s in FindObjectsOfType<Species>())
        {
            all[s.GetType()].Add(s);
        }
    }

    void Update()
    {
        for(int iter = 0; iter < iterations; iter++)
        {
            foreach (System.Type T in allSpeciesTypes)
                foreach (Species s in all[(T)])
                    s.Simulate(dt);
            foreach (System.Type T in allSpeciesTypes)
            {
                foreach (Species s in buffer_remove[(T)])
                    all[T].Remove(s);
                buffer_remove[T].Clear();
                foreach (Species s in buffer_add[(T)])
                    all[T].Add(s);
                buffer_add[T].Clear();
            }
        }
        hue += dHueDT;
        m.color = Color.HSVToRGB(hue%1, 0.30f, 0.60f);
    }
}
