﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaveSpawner : MonoBehaviour { // the class which controls the Enemies spawned each wave and their properties 

    public enum SpawnState {Spawning,Waiting,Counting }

    [System.Serializable]
    public class Wave // the properties of the wave 
    {
        public string name;
        public Transform enemy;
        public int count;
        public float rate;
    }
    public Wave[] waves;
    private int nextWave = 0;

    public Transform[] spawnPoints;

    public float timeBetweenWaves = 5f;
    public float waveCountdown;
    public int countMultiplier;
    public float rateMultiplier;
    //UI
    private int waveNumber = 0;
    public int WaveNumber{
        get{ return waveNumber;}
        set{waveNumber  = value;}
    }
    public Text waveText;


    private float searchCountdown = 1f;

    private SpawnState state = SpawnState.Counting;

    void Start()
    {
        waveCountdown = timeBetweenWaves;
        SetWaveText();

    }

    void Update() // updates and changes the properties of the enemies spawned in the wave 
    {
        if (state == SpawnState.Waiting) // checks the state of the weapon 
        {
            if (!EnemyIsAlive())
            {
                WaveCompleted();
                
            }
            else
            {
                return;
            }
        }

        if (waveCountdown <= 0)
        {
            if (state != SpawnState.Spawning)
            {
                StartCoroutine(SpawnWave(waves[nextWave]));
            }
        }
        else
        {
            waveCountdown -= Time.deltaTime;
        }

    }
    void WaveCompleted() // if the wave is completed it sets the properties for the next wave 
    {
        state = SpawnState.Counting;
        waveCountdown = timeBetweenWaves;
        waveNumber++;
        SetWaveText();
        if (nextWave + 1 > waves.Length - 1)
        {
            nextWave = 0;
            if (rateMultiplier != 0 && countMultiplier != 0)
            {
                for (int i = 0; i < waves.Length; i++)
                {
                    waves[i].count *= countMultiplier;
                    waves[i].rate *= rateMultiplier;

                }
            }
            Debug.Log("All Waves Completed ~ LOOPING");
        }
        else
        {
            nextWave++;
        }
        
    }
    bool EnemyIsAlive() // checks wether the enemy from the last wave is alive 
    {
        searchCountdown -= Time.deltaTime;

        if (searchCountdown <= 0f)
        {
            searchCountdown = 0f;
            if (GameObject.FindGameObjectWithTag("Enemy") == null)
            {
                return false;
            }
        }
       
        return true;
    }

    IEnumerator SpawnWave(Wave _wave) // spawns the wave with the current properties
    {
        Debug.Log("Spawn Wave" + _wave.name);
        state = SpawnState.Spawning;
        for (int i = 0; i < _wave.count; i++)
        {
            SpawnEnemy(_wave.enemy);
            yield return new WaitForSeconds(1f / _wave.rate); // spawns the wave at a ertain rate 
        }

        state = SpawnState.Waiting;
        yield break;
    }

    void SpawnEnemy(Transform _enemy)// spawns the enemy at a random spawn point from the exisiting ones 
    {
        if (spawnPoints.Length == 0)
        {
            Debug.LogError("No Spawn Points");
        }
        Transform _sp = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Instantiate(_enemy, _sp.position,_sp.rotation);
    }

    public void SetWaveText() // controls the UI of the wave number 
    {


        waveText.text = "WAVE" + waveNumber.ToString();
    }
}
