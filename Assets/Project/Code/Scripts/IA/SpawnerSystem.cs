﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DarkTonic.MasterAudio;

public enum WaveState { WaitUntilStartOfTheGame, Standby, IsSpawning }

public class SpawnerSystem : MonoBehaviour
{
    public delegate void WaveAlertHandler(float timer);
    public event WaveAlertHandler OnWaveStartinSoon;
    public event WaveAlertHandler OnWavePossibilityToSpawnState;

    [System.Serializable]
    public class Wave
    {
        public string waveName;
        public List<MinionsData> minionsData;
    }

    [System.Serializable]
    public class MinionsData
    {
        public string spawnLocationName;
        public Transform spawnLocation; //Set the spawner it can spawns
        [Range(0,8)] public int usedPathIndex; //Set the path followed
        public GameObject[] minionsUsedInTheWave;
    }

    public WaveState waveState; // public for debug purpose

    [Header("SPAWNER PARAMETERS")]
    [SerializeField] private float delayBeforeSpawningFirstWave = 45f;
    [SerializeField] private float spawnRate = 5f;
    [SerializeField] private float delayBetweenWaves = 30f;
    private float firstCountdown = 0f; // put in private after tests
    private float countdown = 0f;
    private int indexOfCurrentWave = 0; // to delete after tests
    [SerializeField] private List<Transform> paths;
    [SerializeField] private GameObject spawnObjectFeedback;

    [Header("WAVES")]
    [SerializeField] private List<Wave> waves;

    [Header("SOUNDS")]
    [SoundGroup] public string spawnCustomEvent;

    public bool spawnEventEndedHasBeenHandled = false;

    public List<Transform> Paths { get => paths; }
    public int IndexOfCurrentWave { get => indexOfCurrentWave; set => indexOfCurrentWave = value; }
    public List<Wave> Waves { get => waves; }
    public float FirstCountdown { get => firstCountdown; set => firstCountdown = value; }
    public float Countdown { get => countdown; set => countdown = value; }
    public float DelayBeforeSpawningFirstWave { get => delayBeforeSpawningFirstWave; set => delayBeforeSpawningFirstWave = value; }
    public float DelayBetweenWaves { get => delayBetweenWaves; set => delayBetweenWaves = value; }

    void Start()
    {
        //Waits for a certain time then change wave state to spawn (it's taking in count the delay before the game realy starts)
        //StartCoroutine(WaitingState(DelayBeforeSpawningFirstWave, SpawnWave()));

        FirstCountdown = DelayBeforeSpawningFirstWave;
        OnWaveStartinSoon?.Invoke(DelayBeforeSpawningFirstWave);

        if (HasEntityToSpawn())
            OnWavePossibilityToSpawnState?.Invoke(1);

        Debug.Log(HasEntityToSpawn(), transform);
           
        //Set Fill amount here for the first delay
        Countdown = DelayBetweenWaves;
    }

    void Update()
    {
        if (!GameManager.Instance.GameIsInPlayMod()) return;

        if (waveState == WaveState.Standby && Countdown > 0)
        {
            Countdown -= Time.deltaTime;

            if (Countdown <= 0 && waveState != WaveState.IsSpawning)
            {
                StartCoroutine(SpawnWave());
            }
        }
        else if (waveState == WaveState.WaitUntilStartOfTheGame && FirstCountdown > 0)
        {
            FirstCountdown -= Time.deltaTime;

            if (FirstCountdown <= 0 && waveState != WaveState.IsSpawning)
            {
                StartCoroutine(SpawnWave());
            }
        }
    }

    public IEnumerator SpawnWave()
    {
        //yield return new WaitUntil(() => GameManager.Instance.GameIsInPlayMod());

        if (!HasEntityToSpawn()) yield break;

        waveState = WaveState.IsSpawning;

        //Hide Spawn State Displayer(s)
        OnWavePossibilityToSpawnState?.Invoke(0);

        //Spawn Sound Event : Oppening Portal
        UtilityClass.PlaySoundGroupImmediatly(spawnCustomEvent, transform);

        if (spawnObjectFeedback != null)
            spawnObjectFeedback.SetActive(true);
        else Debug.LogError("Add the spawnObjectFeedback in the empty field");

        //Waves[0,1,2...]
        Wave currentWave = Waves[IndexOfCurrentWave];
        Debug.Log(currentWave.waveName);

        //For each minions in it, loop spawning
        for (int i = 0; i < currentWave.minionsData.Count; i++)
        {
            for (int j = 0; j < currentWave.minionsData[i].minionsUsedInTheWave.Length; j++)
            {
                //Spawn Sound Event : Portal Loop
                //if(MasterAudio.SoundsReady)
                //     UtilityClass.PlaySoundGroupImmediatly(spawnCustomEvent, transform);

                MinionsData minionData = currentWave.minionsData[i];

                Debug.Log(minionData.minionsUsedInTheWave.Length);
                NPCController spawningMinionController = minionData.minionsUsedInTheWave[j].GetComponent<NPCController>();

                if (spawningMinionController.IsABoss && !GameManager.Instance.ItIsABossWave)
                {
                    GameManager.Instance.ItIsABossWave = true;
                }

                SpawnMinions(
                   minionData.minionsUsedInTheWave[j],
                    minionData.spawnLocation,
                    minionData.usedPathIndex);

                yield return new WaitForSeconds(spawnRate);

                //Update GameManager
                if(j == minionData.minionsUsedInTheWave.Length - 1 && !spawnEventEndedHasBeenHandled && !GameManager.Instance.WaveCountHasBeenSet)
                {
                    GameManager.Instance.UpdateWaveCount();
                    //Spawn Sound Event : Portal Closing
                    // UtilityClass.PlaySoundGroupImmediatly(spawnCustomEvent, transform);
                    //Spawn Sound Event : Stop Looping spawning
                    //MasterAudio.StopAllOfSound(spawnCustomEvent);
                }
            }
        }

        //Change spawner state
        waveState = WaveState.Standby;
    }

    public bool HasEntityToSpawn()
    {
        bool hasEntityToSpawn = false;

        if (Waves.Count < IndexOfCurrentWave
            || GameManager.Instance.ItIsABossWave
            || Waves[IndexOfCurrentWave].minionsData.Count == 0)
        {
            hasEntityToSpawn = false;
            OnWavePossibilityToSpawnState?.Invoke(0);
        }
        else if (Waves.Count >= IndexOfCurrentWave)
        {
            hasEntityToSpawn = true;
            OnWavePossibilityToSpawnState?.Invoke(1);
        }

        return hasEntityToSpawn;
    }

    void SpawnMinions(GameObject minion, Transform spawnLocation, int pathIndex)
    {
        Debug.Log(minion.name + " spawned !");

        GameObject currentMinion = Instantiate(minion, spawnLocation.position, spawnLocation.rotation);

        NPCController spawningMinionController = currentMinion.GetComponent<NPCController>();

        if (GameManager.Instance.ItIsABossWave)
        {
            GameManager.Instance.UpdateRemainingMonsterValue(1);
            spawningMinionController.IsABossWaveMember = true;

            Debug.Log("SET SPAWNED MINION AS A BOSS WAVE MEMBER !!!!!!!!!!!!");
        }

        Transform pathUsed = Paths[pathIndex];

        foreach (Transform waypoint in pathUsed)
        {
            currentMinion.GetComponent<NPCController>().waypoints.Add(waypoint);
        }
    }

    public void UpdateElementsOnSpawnFinished()
    {
        //Deactive Portal effect
        if (spawnObjectFeedback != null)
            spawnObjectFeedback.SetActive(false);

        //Display or Hide wave spawn state
        if (HasEntityToSpawn()) OnWavePossibilityToSpawnState?.Invoke(1);
        else OnWavePossibilityToSpawnState?.Invoke(0);

        OnWaveStartinSoon?.Invoke(DelayBetweenWaves);
        Countdown = DelayBetweenWaves;

        spawnEventEndedHasBeenHandled = false;
    }

    void OnValidate()
    {
        for (int i = 0; i < Waves.Count; i++)
        {
            for (int j = 0; j < Waves[i].minionsData.Count; j++)
            {
                if (Waves[i].minionsData[j].spawnLocation != null)
                    Waves[i].minionsData[j].spawnLocationName = Waves[i].minionsData[j].spawnLocation.name;
                else Waves[i].minionsData[j].spawnLocationName = string.Empty;
            }
        }
    }
}