using System.Collections;
using UnityEngine;

public class Temptation : MonoBehaviour
{
    private float moveSpeed;
    private Vector3 moveTrajectory;
    public int hitPoints; //1 for normal temptations, 5 for elite temptations

    public float trajectoryAngleVariance = 10f;
    private float trajectoryOffsetAngle;

    private Camera mainCamera = null;
    [SerializeField] private ParticleSystem particlesToShowOnDestroy;

    private void Awake()
    {
        RankTimer.OnTimeInRankCompleted += DestroyTemptation;
        GameManager.OnGameOver += DestroyTemptation;
        this.GetRandomMoveSpeedAndTrajectory();
    }

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void OnDestroy()
    {
        RankTimer.OnTimeInRankCompleted -= DestroyTemptation;
        GameManager.OnGameOver -= DestroyTemptation;
    }

    private void GetRandomMoveSpeedAndTrajectory()
    {
        this.moveSpeed = Random.Range(GameManager.Instance.Difficulty.minTemptationSpeed, GameManager.Instance.Difficulty.maxTemptationSpeed);
        this.moveTrajectory = (GameManager.Instance.finalPlayerPosition - this.transform.position).normalized;
        this.moveTrajectory.z = 0.0F;
    }

    void FixedUpdate()
    {
        //this.gameObject.transform.Translate(this.moveTrajectory * (this.moveSpeed * Time.fixedDeltaTime));
        Vector3 newPosition = transform.position;
        Vector3 moveAmount = moveTrajectory * moveSpeed * Time.fixedDeltaTime;
        newPosition.x += moveAmount.x;
        newPosition.y += moveAmount.y;
        //newPosition.z += moveAmount.z;
        transform.position = newPosition;
    }

    private void OnTriggerEnter(Collider other)
    {
        Vector3 screenPoint = Camera.main.WorldToViewportPoint(transform.position);
        bool onScreen = screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
        if (onScreen == false || hitPoints <= 0)
        {
            return;
        }

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
        GameManager.Instance.DamagePlayer(1);
        Destroy(this.gameObject);
    }

    private void DecrementHitPoints()
    {
        this.hitPoints -= 1;

        if (this.hitPoints <= 0)
        {
            DestroyTemptation();
        } 
    }

    private void DestroyTemptation()
    {
        this.moveSpeed = 0;
        StartCoroutine(PlayEffectsAndDestroy());
    }

    private IEnumerator PlayEffectsAndDestroy()
    {
        // Do temptation destroy vfx/sounds here
        particlesToShowOnDestroy.Play();
        yield return new WaitForSeconds(0.2F);
        Destroy(this.gameObject);
    }
}
