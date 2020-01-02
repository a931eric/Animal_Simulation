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
        return new Vector3(v.x, 0, v.y);
    }

    public Material m;
    public float hue, dHueDT;
    readonly System.Type[] allSpeciesTypes = Assembly.GetAssembly(typeof(Species)).GetTypes().Where(t => t.IsSubclassOf(typeof(Species))).ToArray();
    public Dictionary<System.Type, List<Species>> allIndividuals { get; } = new Dictionary<System.Type, List<Species>>();
    public Dictionary<System.Type, List<Species>> buffer_add = new Dictionary<System.Type, List<Species>>();
    public Dictionary<System.Type, List<Species>> buffer_remove = new Dictionary<System.Type, List<Species>>();

    public int iterations = 1;
    public float dt = 1;

    public B00 B00Prefab;

    [System.Serializable]
    public class Rules
    {
        public float collisionDist = 0.1f;

    }
    public Rules rules;
    Data2D isGround = new Data2D(64, 64, -64, 64, -64, 64);
    public float foodRate = 1;
    public float foodAmount = 0;
    public float obstacleHeight = 5;
    public float rayLength = 2;
    public GameObject ground;

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
        for (int y = -64; y < 64; y += 1)
        {
            for (int x = -64; x < 64; x += 1)
            {
                isGround.Set(x, y, !Physics.Raycast(new Vector3(x, obstacleHeight, y), new Vector3(0, -1, 0), rayLength));
            }
        }

    }

    void Update()
    {
        for (int iter = 0; iter < iterations; iter++)
        {
            foodAmount += foodRate;
            while (foodAmount > 1)
            {
                Vector3 rand = new Vector3(Random.Range(63, -63), 0, Random.Range(63, -63));
                if (isGround.Get(rand.x, rand.z))
                {
                    Instantiate(B00Prefab, rand, Quaternion.AngleAxis(Random.Range(0, 6.28f), new Vector3(0, 1, 0))).HelloWorld(this, B00Prefab.prop);
                    foodAmount--;
                }
            }


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
        m.color = Color.HSVToRGB(hue % 1, 0.30f, 0.60f);
    }

    public float[] DetectSpecies(System.Type T, Vector2 selfPos, Vector2 selfDir, float sightAngle, int sightResolution)
    {
        /// <image url="..\Comment_Images\input_vision.png" scale=".3"></image>
        float[] result = new float[sightResolution];
        foreach (Species target in allIndividuals[T])
        {
            float targetDirection = -0.01745329251f * Vector2.SignedAngle(selfDir, v2(target.transform.position) - selfPos);
            float sqrTargetDistance = Vector2.SqrMagnitude(v2(target.transform.position) - selfPos);
            if (targetDirection > -sightAngle / 2 && targetDirection < sightAngle / 2)
            {
                result[(int)((targetDirection / sightAngle + .5f) * sightResolution)] += 1.0f / sqrTargetDistance;
            }
        }
        return result;
    }
    public float[] DetectObstacle(Vector2 selfPos, Vector2 selfDir, float sightAngle, int sightResolution)
    {
        float[] result = new float[sightResolution];
        RaycastHit hit;
        for (int i = 0; i < sightResolution; i++)
        {
            Ray ray = new Ray(v3(selfPos), v3(Rotate(selfDir, -sightAngle / sightResolution * (i - sightResolution / 2.0f + .5f))));
            if (Physics.Raycast(ray, out hit, 10))
            {
                result[i] = 1.0f / (hit.distance * hit.distance);
            }
            else
            {
                result[i] = 1.0f / 100.0f;
            }
        }
        return result;
    }
    public Vector2 Rotate(Vector2 v, float angle)
    {
        return new Vector2(v.x * Mathf.Cos(angle) - v.y * Mathf.Sin(angle), v.x * Mathf.Sin(angle) + v.y * Mathf.Cos(angle));
    }
}
class Data2D
{
    public bool[,] data;
    float xRes, yRes, x0, y0, xk, yk;
    public Data2D(int xRes, int yRes, float x1, float x2, float y1, float y2)
    {
        data = new bool[xRes, yRes];
        x0 = -x1;
        y0 = -y1;
        xk = xRes / (x2 - x1);
        yk = xRes / (x2 - x1);
        this.xRes = xRes;
        this.yRes = yRes;
    }
    public bool Get(float x, float y)
    {
        return (data[(int)((x0 + x) * xk), (int)((y0 + y) * yk)]);
    }
    public void Set(float x, float y, bool value)
    {
        data[(int)((x0 + x) * xk), (int)((y0 + y) * yk)] = value;
        Debug.Log(((x0 + x) * xk) + "," + ((y0 + y) * yk));
    }
}
