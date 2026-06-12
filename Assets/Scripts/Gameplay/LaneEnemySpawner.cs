using UnityEngine;

namespace ChineseZombieHunter
{
    public class LaneEnemySpawner : MonoBehaviour
    {
        [SerializeField] private GameObject enemyPrefab;
        [SerializeField] private Transform player;
        [SerializeField] private float spawnDistanceAhead = 28f;
        [SerializeField] private float spawnInterval = 1.5f;
        [SerializeField] private float laneOffset = 2.5f;
        [SerializeField] private int laneCount = 3;
        [SerializeField] private bool spawnContinuously = true;

        private float nextSpawnTime;

        private void Update()
        {
            if (!spawnContinuously || enemyPrefab == null || player == null)
            {
                return;
            }

            if (Time.time < nextSpawnTime)
            {
                return;
            }

            nextSpawnTime = Time.time + spawnInterval;
            SpawnEnemy();
        }

        public void SpawnEnemy()
        {
            if (enemyPrefab == null || player == null)
            {
                return;
            }

            int laneIndex = Random.Range(0, Mathf.Max(1, laneCount));
            float centeredIndex = laneIndex - ((laneCount - 1) * 0.5f);
            float x = centeredIndex * laneOffset;
            Vector3 spawnPosition = new Vector3(x, player.position.y, player.position.z + spawnDistanceAhead);

            Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        }
    }
}
