using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Temptation : MonoBehaviour
{
    private float moveSpeed;
    public int hitPoints; //1 for normal temptations, 5 for elite temptations

    private void Awake()
    {
        this.GetRandomMoveSpeed();   
    }

    private void GetRandomMoveSpeed()
    {
        moveSpeed = Random.Range(GameManager.Instance.difficulty.minTemptationSpeed, GameManager.Instance.difficulty.maxTemptationSpeed);
    }

    void FixedUpdate()
    {
        Vector3 moveDirection = (GameManager.Instance.finalPlayerPosition - this.transform.position).normalized; 
        this.gameObject.transform.Translate(moveDirection * (this.moveSpeed * Time.fixedDeltaTime));    
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.LogError("OnTriggerEnter");

        if (other.tag == "Player")
        {
            this.DamagePlayer();
        }
        else if (other.tag == "KiBlast")
        {
            this.DecrementHitPoints();
        }
    }

    private void DamagePlayer()
    {
        GameManager.Instance.playerHealth--;
        Destroy(this.gameObject);
    }

    private void DecrementHitPoints()
    {
        this.hitPoints -= 1;

        if (this.hitPoints <= 0)
        {
            this.DestroyTemptation();
        } 
    }

    private void DestroyTemptation()
    {
        //Do temptation destroy vfx/sounds here  
        Destroy(this.gameObject);
    }
}
