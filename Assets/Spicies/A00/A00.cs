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
        public float radius;
    }
    [SerializeField]
    protected Properties prop;

    [System.Serializable]
    public struct Status
    {
        public float health;
        public float saturation;
        public B00 target;
    }
    [SerializeField]
    protected Status stat;


    public void HelloWorld(Main main, Properties prop)
    {
        this.main = main;
        this.prop = prop;
        main.all[GetType()].Add(this);
        Status stat = new Status
        {
            health = prop.maxHealth
        };
    }

    protected override void Die()
    {
        base.Die();
    }

    public override void Simulate(float dt)
    {
        if (stat.health <= 0) Die();
        Eat(stat.target, prop.eatingSpeed*dt);
    }

    public void Eat(Species food, float amount)
    {
        stat.saturation +=food.Eaten(amount);
    }

    public void Update()
    {
        healthBar.fillAmount = stat.health / prop.maxHealth;
        saturationBar.fillAmount = stat.saturation / prop.maxSaturation;
    }
}
