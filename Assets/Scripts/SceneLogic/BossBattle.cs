using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(AudioSource))]
public class BossBattle : NetworkBehaviour
{
    [SerializeField] private bool isGatesOpen = false;
    [SerializeField] private int maxEnemy = 5;
    [SerializeField] private int enemySpawned = 0;
    [SerializeField] private int enemyKilled = 0;

    private AudioSource _source;

    private void Start()
    {
        GameManager_v2.Instance.OnGameStart.AddListener(OnGameStart);
    }

    private void OnGameStart()
    {
        //Debug.Log($"Host: {NetworkManager.Singleton.IsHost} Server: {NetworkManager.Singleton.IsServer} Client: {NetworkManager.Singleton.IsClient}");
        if (!NetworkManager.Singleton.IsHost) return;
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
        if (enemySpawned >= maxEnemy && enemyKilled >= maxEnemy && !isGatesOpen)
        {
            OpenGatesClientRpc();
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void OpenGatesClientRpc()
    {
        this.gameObject.SetActive(false);
        isGatesOpen = true;
        Debug.Log("BOSS DEFEATED");
        _source.Play();
    }


}
