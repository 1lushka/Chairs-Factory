using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;
using DG.Tweening;

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

[Serializable]
public class GameProgress
{
    public int money;
    public int waveIndex;
}

public class GameManager : MonoBehaviour
{
    public List<Wave> waves;
    public GameState currentState = GameState.Build;

    [SerializeField] private GameObject buildingManager;
    [SerializeField] private List<GameObject> buildModeObjects;
    [SerializeField] private int startMoney = 100;
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private GameObject[] startObjsActive;
    [SerializeField] private GameObject[] startObjsNotActive;

    public int Money { get; private set; }
    private int currentWaveIndex = 0;
    private int enemiesAlive = 0;
    private Coroutine waveCoroutine;

    private string savePath;

    private void Awake()
    {
        savePath = Path.Combine(Application.persistentDataPath, "progress.json");
        LoadProgress();
    }

    private void Start()
    {
        StartGame();
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            StartWave();
        }
    }

    public void StartGame()
    {
        foreach (GameObject obj in startObjsActive) obj.SetActive(true);
        foreach (GameObject obj in startObjsNotActive) obj.SetActive(false);
        if (Money <= 0) Money = startMoney;
        UpdateMoneyUI();
        SetState(GameState.Build);
    }

    public void StartWave()
    {
        if (currentState != GameState.Build || currentWaveIndex >= waves.Count) return;
        SetState(GameState.Wave);
        Wave wave = waves[currentWaveIndex];
        enemiesAlive = 0;
        foreach (var group in wave.enemyGroups) enemiesAlive += group.count;
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
                Vector3 originalScale = enemyObj.transform.localScale;
                enemyObj.transform.localScale = Vector3.zero;
                enemyObj.transform.DOScale(originalScale, 0.3f).SetEase(Ease.OutBack);
                Enemy enemy = enemyObj.GetComponent<Enemy>();
                if (enemy != null) enemy.OnDeath += OnEnemyDeath;
                yield return new WaitForSeconds(wave.spawnInterval);
            }
        }
    }

    private void OnEnemyDeath(Enemy enemy)
    {
        enemiesAlive--;
        enemy.OnDeath -= OnEnemyDeath;
        AddMoney(enemy.Reward);
        if (enemiesAlive <= 0) EndWave();
    }

    private void EndWave()
    {
        currentWaveIndex++;
        SaveProgress();
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
        if (buildingManager != null) buildingManager.SetActive(currentState == GameState.Build);
        if (buildModeObjects != null)
        {
            foreach (var obj in buildModeObjects)
            {
                if (obj != null) obj.SetActive(currentState == GameState.Build);
            }
        }
    }

    public void ResetGame()
    {
        currentWaveIndex = 0;
        Money = startMoney;
        SaveProgress();
        UpdateMoneyUI();
        SetState(GameState.Build);
    }

    public bool SpendMoney(int amount)
    {
        if (Money >= amount)
        {
            Money -= amount;
            SaveProgress();
            UpdateMoneyUI();
            return true;
        }
        return false;
    }

    public void AddMoney(int amount)
    {
        Money += amount;
        SaveProgress();
        UpdateMoneyUI();
    }

    private void UpdateMoneyUI()
    {
        if (moneyText != null) moneyText.text = $"Деньги: {Money}";
    }

    private void SaveProgress()
    {
        GameProgress progress = new GameProgress
        {
            money = Money,
            waveIndex = currentWaveIndex
        };
        string json = JsonUtility.ToJson(progress, true);
        File.WriteAllText(savePath, json);
    }

    private void LoadProgress()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            GameProgress progress = JsonUtility.FromJson<GameProgress>(json);
            if (progress != null)
            {
                Money = progress.money;
                currentWaveIndex = progress.waveIndex;
                return;
            }
        }
        Money = startMoney;
        currentWaveIndex = 0;
    }

    private void OnApplicationQuit()
    {
        print("save");
        SaveProgress();
        FindAnyObjectByType<BuildingSaveManager>().SaveBuildings();
    }
}
