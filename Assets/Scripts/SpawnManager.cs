using System.Xml.Serialization;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance;
    [SerializeField] private bool canSpawn;

    [SerializeField] private GameObject[] entitiesPrefabs;
    [SerializeField] private Vector3 spawnPosition;

    [SerializeField] private float entitieSpeed = 7;
    [SerializeField] private float xMargin = 2;
    [SerializeField] private float spawnTimer;
    [SerializeField] private float spawnTimerMax = 0.5f;

    private void Awake()
    {
        Instance = this;
    }
    private void Update()
    {
        TrySpawn();
    }

    public void TrySpawn()
    {
        if (!canSpawn)
            return;

        if (spawnTimer > 0)
        {
            spawnTimer -= Time.deltaTime;
        }
        else
        {
            spawnTimer = spawnTimerMax;
            SpawnEntity();
        }
    }

    public void StartScript()
    {
        canSpawn = true;
        spawnTimer = spawnTimerMax;
    }

    private void SpawnEntity()
    {
        GameObject entityToSpawn = entitiesPrefabs[Random.Range(0, entitiesPrefabs.Length)];
        spawnPosition.x = Random.Range(-xMargin, xMargin);

        GameObject spawnEntity = Instantiate(entityToSpawn, spawnPosition, Quaternion.identity);
        spawnEntity.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(0, -entitieSpeed);

    }
}