using UnityEngine;
using UnityEngine.UI;
public class A00 : Species
{
    public Image healthBar, saturationBar;
    public bool controledByPlayer = false;
    [System.Serializable]
    public struct Status
    {
        public float health;
        public float saturation;
        public Vector2 direction;
    }
    [SerializeField]
    protected Status stat;

    [System.Serializable]
    public class DNA
    {
        [System.Serializable]
        public struct StaticGene
        {
            public float maxHealth;
            public float maxSaturation;
            public float eatingSpeed;
            public float sightAngle;
            public int sightResolution;
            public float speed;
        }
        [SerializeField]
        public StaticGene staticGene;
        [System.Serializable]
        public struct Gene
        {
            public float m;
        }
        [SerializeField]
        public Gene gene;
        DNA Mutate()
        {

            DNA replicatedDNA=new DNA();
            replicatedDNA.staticGene = staticGene;
            replicatedDNA.gene = gene;//------
            return replicatedDNA;
        }
    }
    [SerializeField]
    DNA dna;

    public void HelloWorld(Main main, DNA dnaFromParent)
    {
        this.main = main;
        dna = dnaFromParent;
        main.allIndividuals[GetType()].Add(this);
        Status stat = new Status
        {
            health = dna.staticGene.maxHealth,
            saturation = 10
        };
    }

    public override void Simulate(float dt)
    {
        if (stat.health <= 0) Die();
        if (controledByPlayer)
        {

        }

        float[] detectB00 = main.DetectSpecies(typeof(B00),main.v2( transform.position),stat.direction,dna.staticGene.sightAngle,dna.staticGene.sightResolution);
        float[] detectObstacle = main.DetectObstacle( main.v2(transform.position), stat.direction, dna.staticGene.sightAngle, dna.staticGene.sightResolution);

        if (Eat(typeof(B00), dt))
        {
            Movement = 1;
        }
        else
        {
            Vector2 dir=new Vector2(0,0);
            for(int i = 0; i < dna.staticGene.sightResolution; i++)
            {
                dir += main.Rotate(stat.direction, -dna.staticGene.sightAngle / dna.staticGene.sightResolution * (i - dna.staticGene.sightResolution / 2.0f + .5f))*(-detectObstacle[i]+10*detectB00[i]);
            }
            dir +=stat.direction*dna.gene.m;
            dir.Normalize();


            stat.direction = main.Rotate(dir, Random.Range(0.3f, -0.3f) * dt);
            //stat.direction = dir;
            if (Move(dt))
                Movement = 0;
            else Movement = 2;
        }
    }
    
    public bool Eat(System.Type T,float dt)
    {
        foreach(Species s in main.allIndividuals[T])
            if(Vector3.SqrMagnitude(s.transform.position-transform.position)<(radius+s.radius)* (radius + s.radius))
            {
                stat.saturation += s.Eaten(dna.staticGene.eatingSpeed*dt);
                return true;
            }
        return false;
    }

    public bool Move(float dt)
    {
        if (Physics.Raycast(transform.position, main.v3(stat.direction),main.rules.collisionDist+radius))
            return false;
        if (Physics.Raycast(transform.position, main.v3(main.Rotate(stat.direction,1.0f)), main.rules.collisionDist + radius))
            return false;
        if (Physics.Raycast(transform.position, main.v3(main.Rotate(stat.direction, -1.0f)), main.rules.collisionDist + radius))
            return false;
        transform.position += main.v3(stat.direction * dna.staticGene.speed * dt);
        return true;
        
    }

    public float rotLerp = 0.01f;
    public Animator animator;
    int Movement=0;
    public void Update()
    {
        transform.LookAt(transform.position +main.v3(stat.direction)*rotLerp+transform.forward*(1-rotLerp));
        healthBar.fillAmount = stat.health / dna.staticGene.maxHealth;
        saturationBar.fillAmount = stat.saturation / dna.staticGene.maxSaturation;
        animator.SetInteger("movement id", Movement);
    }
}
