using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class EnemyGroup
{
    public GameObject prefab;
    public int count;
}

[System.Serializable]
public class Wave
{
    public string waveName;
    public List<Transform> spawnPoints;
    public float spawnInterval = 1f;
    public List<EnemyGroup> enemyGroups;
}

public enum GameState
{
    Build,
    Wave
}

public class GameManager : MonoBehaviour
{
    public List<Wave> waves;
    public GameState currentState = GameState.Build;

    [SerializeField] private GameObject buildingManager;
    [SerializeField] private List<GameObject> buildModeObjects;

    [Header("Экономика")]
    [SerializeField] private int startMoney = 100;
    [SerializeField] private TextMeshProUGUI moneyText;
    public int Money { get; private set; }

    private int currentWaveIndex = 0;
    private int enemiesAlive = 0;
    private Coroutine waveCoroutine;

    void Start()
    {
        Money = startMoney;
        UpdateMoneyUI();
        SetState(GameState.Build);
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            StartWave();
        }
    }

    public void StartWave()
    {
        if (currentState != GameState.Build || currentWaveIndex >= waves.Count) return;

        SetState(GameState.Wave);

        Wave wave = waves[currentWaveIndex];
        enemiesAlive = 0;
        foreach (var group in wave.enemyGroups)
        {
            enemiesAlive += group.count;
        }

        waveCoroutine = StartCoroutine(SpawnWave(wave));
    }

    private IEnumerator SpawnWave(Wave wave)
    {
        foreach (var group in wave.enemyGroups)
        {
            for (int i = 0; i < group.count; i++)
            {
                Transform spawnPoint = wave.spawnPoints[UnityEngine.Random.Range(0, wave.spawnPoints.Count)];
                GameObject enemyObj = Instantiate(group.prefab, spawnPoint.position, spawnPoint.rotation);

                Enemy enemy = enemyObj.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.OnDeath += OnEnemyDeath;
                }

                yield return new WaitForSeconds(wave.spawnInterval);
            }
        }
    }

    private void OnEnemyDeath(Enemy enemy)
    {
        enemiesAlive--;
        enemy.OnDeath -= OnEnemyDeath;

        AddMoney(enemy.Reward);

        if (enemiesAlive <= 0)
        {
            EndWave();
        }
    }

    private void EndWave()
    {
        currentWaveIndex++;
        SetState(GameState.Build);

        if (waveCoroutine != null)
        {
            StopCoroutine(waveCoroutine);
            waveCoroutine = null;
        }
    }

    private void SetState(GameState newState)
    {
        currentState = newState;

        if (buildingManager != null)
            buildingManager.SetActive(currentState == GameState.Build);

        if (buildModeObjects != null)
        {
            foreach (var obj in buildModeObjects)
            {
                if (obj != null)
                    obj.SetActive(currentState == GameState.Build);
            }
        }
    }

    public void ResetGame()
    {
        currentWaveIndex = 0;
        Money = startMoney;
        UpdateMoneyUI();
        SetState(GameState.Build);
    }

    public bool SpendMoney(int amount)
    {
        if (Money >= amount)
        {
            Money -= amount;
            UpdateMoneyUI();
            return true;
        }
        return false;
    }

    public void AddMoney(int amount)
    {
        Money += amount;
        UpdateMoneyUI();
    }

    private void UpdateMoneyUI()
    {
        if (moneyText != null)
            moneyText.text = $"Деньги: {Money}";
    }
}
