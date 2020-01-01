using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class A00 : Species
{
    public Image healthBar, saturationBar;
    
    [System.Serializable]
    public struct Properties
    {
        public float maxHealth;
        public float maxSaturation;
        public float eatingSpeed;
        public float sightAngle;
        public int sightResolution;
        public float speed;
    }
    [SerializeField]
    protected Properties prop;

    [System.Serializable]
    public struct Status
    {
        public float health;
        public float saturation;
        public Vector2 direction;
    }
    [SerializeField]
    protected Status stat;


    public void HelloWorld(Main main, Properties prop)
    {
        this.main = main;
        this.prop = prop;
        main.allIndividuals[GetType()].Add(this);
        Status stat = new Status
        {
            health = prop.maxHealth
        };
    }

    public override void Simulate(float dt)
    {
        if (stat.health <= 0) Die();
        float[] detectB00 = main.DetectSpecies(typeof(B00),main.v2( transform.position),stat.direction,prop.sightAngle,prop.sightResolution);
        float[] detectObstacle = main.DetectObstacle( main.v2(transform.position), stat.direction, prop.sightAngle, prop.sightResolution);
        
        if (!Eat(typeof(B00), dt))
        {
            Vector2 dir=new Vector2(0.01f,0);
            for(int i = 0; i < prop.sightResolution; i++)
            {
                dir += main.Rotate(stat.direction, -prop.sightAngle / prop.sightResolution * (i - prop.sightResolution / 2.0f + .5f))/Mathf.Max( detectObstacle[i],0.01f);
            }
            dir.Normalize();
            
            stat.direction = main.Rotate(dir, Random.Range(0.5f, -0.5f) * dt);
            Move(dt);
        }
    }
    
    public bool Eat(System.Type T,float dt)
    {
        foreach(Species s in main.allIndividuals[T])
            if(Vector3.SqrMagnitude(s.transform.position-transform.position)<(bProp.radius+s.bProp.radius)* (bProp.radius + s.bProp.radius))
            {
                stat.saturation += s.Eaten(prop.eatingSpeed*dt);
                return true;
            }
        return false;
    }

    public bool Move(float dt)
    {
        if (Physics.Raycast(transform.position, main.v3(stat.direction),main.rules.collisionDist+bProp.radius))
            return false;
        if (Physics.Raycast(transform.position, main.v3(main.Rotate(stat.direction,1.0f)), main.rules.collisionDist + bProp.radius))
            return false;
        if (Physics.Raycast(transform.position, main.v3(main.Rotate(stat.direction, -1.0f)), main.rules.collisionDist + bProp.radius))
            return false;
        transform.position += main.v3(stat.direction * prop.speed * dt);
        return true;
        
    }

    public void Update()
    {
        transform.LookAt(transform.position +main.v3(stat.direction));
        healthBar.fillAmount = stat.health / prop.maxHealth;
        saturationBar.fillAmount = stat.saturation / prop.maxSaturation;
    }
}
