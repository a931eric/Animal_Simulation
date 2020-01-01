using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//grass
public class B00 : Species
{


    public Image healthBar;

    [System.Serializable]
    public struct Properties
    {
        public float maxHealth;
    }
    [SerializeField]
    protected Properties prop;

    [System.Serializable]
    public struct Status
    {
        public float health;
    }
    [SerializeField]
    protected Status stat;


    public void HelloWorld(Main main,Properties prop)
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
    }
    public void Update()
    {
        healthBar.fillAmount = stat.health / prop.maxHealth;
    }
    public override float Eaten(float amount)
    {
        if (amount > stat.health)
            amount = stat.health;

        stat.health -= amount;
        return amount;
    }
}
