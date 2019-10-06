using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PolarCoordinates;

public class Temptation : MonoBehaviour
{
    private float moveSpeed;
    private Vector3 moveTrajectory;
    public int hitPoints; //1 for normal temptations, 5 for elite temptations

    public float trajectoryAngleVariance = 10f;
    private float trajectoryOffsetAngle;

    private void Awake()
    {
        this.GetRandomMoveSpeedAndTrajectory();   
    }

    private void GetRandomMoveSpeedAndTrajectory()
    {
        float test = Random.Range(-trajectoryAngleVariance, trajectoryAngleVariance);

        this.moveSpeed = Random.Range(GameManager.Instance.Difficulty.minTemptationSpeed, GameManager.Instance.Difficulty.maxTemptationSpeed);
        this.trajectoryOffsetAngle = Mathf.Deg2Rad * test;
        this.moveTrajectory = (GameManager.Instance.finalPlayerPosition - this.transform.position).normalized;
        PolarCoordinate moveDirectionPolar = new PolarCoordinate(1.0f, this.moveTrajectory);
        moveDirectionPolar.angle += trajectoryOffsetAngle;
        this.moveTrajectory = moveDirectionPolar.PolarToCartesian();
    }

    void FixedUpdate()
    {
        this.gameObject.transform.Translate(this.moveTrajectory * (this.moveSpeed * Time.fixedDeltaTime));    
    }

    private void OnTriggerEnter(Collider other)
    {
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
