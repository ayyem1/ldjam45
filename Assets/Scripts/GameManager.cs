using UnityEngine;

public sealed class GameManager : MonoBehaviour
{
    private static GameManager instance = null;

    private float timestampToSwitchToNextLevel = 0.0F;

    [SerializeField] public Sentry sentry = null;
    [SerializeField] public GameLevel currentLevel = null;

    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameManager[] instances = FindObjectsOfType<GameManager>();
                if (instances.Length == 0)
                {
                    throw new System.Exception("ERROR: Could not find an instance of the GameManager in the current scene. Please attach one to a gameobject.");
                }
                else if (instances.Length > 1)
                {
                    throw new System.Exception("ERROR: Found multiple instances of the gamemanager in the current scene. Count=" + instances.Length);
                }

                instance = instances[0];
            }

            return instance;
        }
    }

    static GameManager() { }

    private GameManager() { }

    private void Start()
    {
        timestampToSwitchToNextLevel = Time.realtimeSinceStartup + currentLevel.levelDurationInSeconds;
    }

    private void Update()
    {
        
    }
}
