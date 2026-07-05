using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform[] respawnPoints;
    [SerializeField] private float cooldown;
    [Space]
    [SerializeField] private float cdDecreaseRate = 0.05f;
    [SerializeField] private float cooldownCap = 0.7f;
    private float timer;
    private Transform player;



    private void Awake()
    {
        player = FindAnyObjectByType<Player>().transform;
    }

    private void Update()
    {
        timer -= Time.deltaTime;

        if (timer < 0)
        {
            timer = cooldown;
            spawnNewEnemy();

            cooldown = Mathf.Max(cooldownCap, cooldown - cdDecreaseRate);
        }
    }

    private void spawnNewEnemy()
    {
        int respawnPointIndex = Random.Range(0, respawnPoints.Length);
        Vector3 spawnPoint = respawnPoints[respawnPointIndex].position;

        GameObject newEnemy = Instantiate(enemyPrefab, spawnPoint, Quaternion.identity);

        Enemy enemy = newEnemy.GetComponent<Enemy>();

        bool createdOnTheRight = spawnPoint.x > player.position.x;

       if (createdOnTheRight)
       {
       enemy.SetDirection(-1);
       enemy.Flip();
       }
       else
       enemy.SetDirection(1);
    }
}
