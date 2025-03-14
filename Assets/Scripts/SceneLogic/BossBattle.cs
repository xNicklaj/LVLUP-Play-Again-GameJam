using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BossBattle : NetworkBehaviour
{
    [SerializeField] private Tilemap bossGates;
    [SerializeField] private bool isGatesOpen = false;
    [SerializeField] private int maxEnemy = 5;
    [SerializeField] private int enemySpawned = 0;
    [SerializeField] private int enemyKilled = 0;

    private void Start()
    {
        if (!NetworkManager.Singleton.IsHost) return;
        Debug.Log("Iscrizione ai listner");
        GameManager_v2.Instance.OnBossEnemySpawn.AddListener(AddedEnemy);
        GameManager_v2.Instance.OnBossEnemyKill.AddListener(RemoveEnemy);
        enemySpawned = 0;
    }

    private void AddedEnemy()
    {
        enemySpawned++;
    }

    private void RemoveEnemy()
    {
        enemyKilled++;
        CheckKill();
    }
    

    void Update()
    {
    }

    private void CheckKill()
    {
        if (enemySpawned >= maxEnemy && enemyKilled >= maxEnemy)
        {
            OpenGates();
        }
    }
    private void OpenGates()
    {
        bossGates.gameObject.SetActive(false);
        Debug.Log("BOSS SCONFITTO");
    }
}
