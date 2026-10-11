using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    public float MaxHealth = 100f;
    float CurrentHealth;

    void Awake()
    {
        CurrentHealth = MaxHealth;
    }

    public void TakeDamage(float damage)
    {
        CurrentHealth -= damage;
        Debug.Log($"Took damage: {damage}\nCurrent Health: {CurrentHealth}");


        if (CurrentHealth <= 0)
        {
            Debug.Log("Dummy is dead");
            Destroy(gameObject);
        }
    }

}
