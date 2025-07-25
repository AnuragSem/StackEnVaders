using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerAttributeManager : MonoBehaviour
{
    [SerializeField]int playerLives;

    public int getPlayerLives()
    { 
        return playerLives;
    }

    public void DecrementLifeCount()
    { 
        --playerLives;
        UIManager.Instance.UpdatePlayerLifeCount(playerLives);
    }
}
