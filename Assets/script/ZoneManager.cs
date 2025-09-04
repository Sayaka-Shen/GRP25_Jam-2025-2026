using UnityEngine;

public class ZoneManager : MonoBehaviour
{
    [Header("Parameter Zone")]
    public static ZoneManager Instance;

    [SerializeField] private int m_nbMaxObstacleSpawn;
    public int NbObstacleSpawn { get; set; }
    public bool MaxObstacleSpawnedReach { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if(NbObstacleSpawn >= m_nbMaxObstacleSpawn)
        {
            MaxObstacleSpawnedReach = true;
        }
        else
        {
            MaxObstacleSpawnedReach = false;
        }
    }
}
