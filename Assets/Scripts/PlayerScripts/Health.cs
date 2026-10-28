using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    public float maxHealth = 100f;
    float currentHealth;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log($"{name} took damage: {damage}\nCurrent Health: {currentHealth}");


        if (currentHealth <= 0)
        {
            Debug.Log("Dummy is dead");
            Destroy(gameObject);
        }
    }

}
