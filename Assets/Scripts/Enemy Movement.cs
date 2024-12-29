using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    Vector3 staticMoveVector = new Vector3(1, 0, -1);
    Vector3 moveDirection;

    [SerializeField]float moveSpeed = 3f;

    private void Update()
    {
        MoveEnemy();
    }

    void MoveEnemy()
    {
        transform.Translate(staticMoveVector * moveSpeed * Time.deltaTime);
    }
    public void SetMoveDirection(Vector3 direction)
    {
        moveDirection = direction;
    }
}
