using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stats {
    float maxHealth;
    float maxStamina;
    float staminaRecoveryRate;
    float healthRecoveryRate;
}

public class Player_Stats : MonoBehaviour, IDamagable<float>
{
    public float currentStamina;
    public float currentHealth;

    Player_Controller playerController;

    void Start()
    {
            
    }

    void Update() {

    }

    public void TakeDamage(float damageTaken) {
        currentHealth -= damageTaken;
    }
}
