using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float dmg = 20f;
    [SerializeField]float EnemyHealth = 20f;

    public float GetEnemyCurrentHealth()
    {
        return EnemyHealth;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision!=null)
        {
            Debug.Log("collision detected" + " " + collision.gameObject.name);
            Block contactBlock;
            contactBlock = collision.gameObject.GetComponent<Block>();
            if (contactBlock != null)
            {
                float contactBlockHP = contactBlock.GetCurrentHeath();
                TakeDmg(contactBlockHP);
                contactBlock.TakeDmg(dmg);
            }
            else
            {
                Debug.Log("could not get the refrence to contact block");
            }
        } 
    }

    void TakeDmg(float blockHP)
    {
        EnemyHealth = EnemyHealth - blockHP;
        if (EnemyHealth <= 0)
        {
            DestroyEnemy();
        }
        
    }

    void DestroyEnemy()
    { 
        Destroy(gameObject);
    }

    


}
