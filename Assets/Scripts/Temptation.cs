using System;
using System.Collections;
using UnityEngine;

public class Temptation : MonoBehaviour
{
    public static Action OnBossDied;
    private float moveSpeed;
    private Vector3 moveTrajectory;
    public int hitPoints; //1 for normal temptations, 5 for elite temptations

    public float trajectoryAngleVariance = 10f;
    private bool isBoss = false;
    private float trajectoryOffsetAngle;
    [SerializeField] private int damageAmount = 1;
    public AudioSource poppingSound;

    private Camera mainCamera = null;
    [SerializeField] private ParticleSystem particlesToShowOnDestroy = null;

    private void Awake()
    {
        RankTimer.OnTimeInRankCompleted += OnTimeInRankCompleted;
        GameManager.OnGameOver += OnGameOver;
        UIManager.OnMainMenu += DestroyTemptation;
        this.GetRandomMoveSpeedAndTrajectory();
    }

    private void OnTimeInRankCompleted()
    {
        DestroyTemptation();
    }

    private void OnGameOver(bool wasGameWon)
    {
        DestroyTemptation();
    }

    private void Start()
    {
        mainCamera = Camera.main;
        Rank rank = GameManager.Instance.CurrentRank;
        isBoss = rank == null ? false : rank.isBoss;
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }

    private void UnsubscribeFromEvents()
    {
        RankTimer.OnTimeInRankCompleted -= OnTimeInRankCompleted;
        GameManager.OnGameOver -= OnGameOver;
        UIManager.OnMainMenu -= DestroyTemptation;
    }

    private void GetRandomMoveSpeedAndTrajectory()
    {
        this.moveSpeed = UnityEngine.Random.Range(GameManager.Instance.Difficulty.minTemptationSpeed, GameManager.Instance.Difficulty.maxTemptationSpeed);
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
        //Vector3 screenPoint = Camera.main.WorldToViewportPoint(transform.position);
        //bool onScreen = screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
        //if (onScreen == false || hitPoints <= 0)
        //{
        //    return;
        //}

        if (hitPoints <= 0)
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
        GameManager.Instance.DamagePlayer(damageAmount);
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
        UnsubscribeFromEvents();
        if (isBoss)
        {
            OnBossDied();
        }

        StartCoroutine(PlayEffectsAndDestroy());
    }

    private IEnumerator PlayEffectsAndDestroy()
    {
        // Do temptation destroy vfx/sounds here
        float pitch = UnityEngine.Random.Range(-0.1f, 0.1f);
        this.poppingSound.pitch = this.poppingSound.pitch + pitch;
        this.poppingSound.Play();
        particlesToShowOnDestroy.Play();
        yield return new WaitForSeconds(0.2F);
        Destroy(this.gameObject);
    }
}
