//這個好好玩
/// <image url="..\Comment_Images\未命名.png" scale="0.08"></image>

using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;


public class Main : MonoBehaviour
{
    public Vector2 v2(Vector3 v)
    {
        return new Vector2(v.x, v.z);
    }
    public Vector3 v3(Vector2 v)
    {
        return new Vector3(v.x,0, v.y);
    }

    public Material m;
    public float hue,dHueDT;
    readonly System.Type[] allSpeciesTypes = Assembly.GetAssembly(typeof(Species)).GetTypes().Where(t => t.IsSubclassOf(typeof(Species))).ToArray();
    public Dictionary<System.Type, List<Species>> allIndividuals { get; }  = new Dictionary<System.Type, List<Species>>();
    public Dictionary<System.Type, List<Species>> buffer_add    = new Dictionary<System.Type, List<Species>>();
    public Dictionary<System.Type, List<Species>> buffer_remove = new Dictionary<System.Type, List<Species>>();

    public int iterations = 1;
    public float dt = 1;

    [System.Serializable]
    public class Rules
    {
        public float collisionDist = 0.1f;
    }
    public Rules rules;


    void Start()
    {
        
        foreach (System.Type T in allSpeciesTypes)
        {
            allIndividuals.Add(T, new List<Species>());
            buffer_add.Add(T, new List<Species>());
            buffer_remove.Add(T, new List<Species>());
        }
            
        foreach (Species s in FindObjectsOfType<Species>())
        {
            allIndividuals[s.GetType()].Add(s);
        }
    }

    void Update()
    {
        for(int iter = 0; iter < iterations; iter++)
        {
            foreach (System.Type T in allSpeciesTypes)
                foreach (Species s in allIndividuals[T])
                    s.Simulate(dt);
            foreach (System.Type T in allSpeciesTypes)
            {
                foreach (Species s in buffer_remove[T])
                    allIndividuals[T].Remove(s);
                buffer_remove[T].Clear();
                foreach (Species s in buffer_add[T])
                    allIndividuals[T].Add(s);
                buffer_add[T].Clear();
            }
        }
        hue += dHueDT;
        m.color = Color.HSVToRGB(hue%1, 0.30f, 0.60f);
    }

    public float[] DetectSpecies(System.Type T, Vector2 selfPos, Vector2 selfDir, float sightAngle, int sightResolution)
    {
            /// <image url="..\Comment_Images\input_vision.png" scale=".3"></image>
        float[] result = new float[sightResolution];
        foreach (Species target in allIndividuals[T])
        {
            float targetDirection = -0.01745329251f*Vector2.SignedAngle(selfDir,v2(target.transform.position) - selfPos);
            float sqrTargetDistance = Vector2.SqrMagnitude(v2(target.transform.position) - selfPos);
            if (targetDirection > -sightAngle / 2 && targetDirection < sightAngle / 2)
            {
                result[(int)((targetDirection / sightAngle + .5f) * sightResolution)] += 1.0f / sqrTargetDistance;
            }
        }
        return result;
    }
    public float[] DetectObstacle( Vector2 selfPos, Vector2 selfDir, float sightAngle, int sightResolution)
    {
        float[] result = new float[sightResolution];
        RaycastHit hit;
        for(int i = 0; i < sightResolution; i++)
        {
            Ray ray = new Ray(v3(selfPos), v3(Rotate(selfDir, -sightAngle / sightResolution * (i - sightResolution / 2.0f + .5f))));
            if(Physics.Raycast(ray,out hit,7)){
                result[i] = 1.0f / (hit.distance * hit.distance);
            }
        }
        return result;
    }
    public Vector2 Rotate(Vector2 v, float angle)
    {
        return new Vector2(v.x * Mathf.Cos(angle) - v.y * Mathf.Sin(angle), v.x * Mathf.Sin(angle) + v.y * Mathf.Cos(angle));
    }
}
