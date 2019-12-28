using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//grass
public class B00 : Species
{
    public Main main;
    public float maxHealth=100, health;
    public Image healthBar;
    
    public override void HelloWorld(Main main)
    {
        this.main = main;
        main.B00s.Add(this);
        base.HelloWorld(main);
        health = maxHealth;
    }
    public override void Simulate()
    {
        if (health <= 0) Die();
    }
    public void Update()
    {
        healthBar.fillAmount = health / maxHealth;
    }
    public override void Die()
    {
        main.B00s.Remove(this);
        base.Die();
    }

}
