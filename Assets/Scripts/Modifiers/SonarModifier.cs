using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Unity.Netcode;
using UnityEngine;

public class SonarModifier : ModifierBase
{
    public GameObject GhostPrefab;
    public float Radius = 5;
    public int Pings = 3;
    public float Delay = 3.5f; // If you use Radar.wav this should be at least 2.7f

    private short _currentPings = 0;

    protected override void ApplyModifier()
    {
        SpawnGhostsServerRpc();
    }

    protected override void DisposeModifier()
    {

    }

    [Rpc(SendTo.Server)]
    private void SpawnGhostsServerRpc()
    {
        StartCoroutine(PingAndSpawnCoroutine());
    }

    private IEnumerator PingAndSpawnCoroutine()
    {
        while(_currentPings < Pings)
        {
            if (_currentPings > 0) PlayPingClientRpc();
            LayerMask[] masks = new LayerMask[] { LayerMask.GetMask("LitEnemy"), LayerMask.GetMask("UnlitEnemy") };
            List<Collider2D> colliders = VisionSystem.FindNearPosition(transform.position, Radius, masks);
            foreach (Collider2D collider in colliders)
            {
                if (collider.gameObject.name.Contains("Bullet")) continue;
                Vector3 pos = collider.transform.position;
                SpawnGhostClientRpc(pos, collider.GetComponent<Animator>().GetFloat("Horizontal"), collider.GetComponent<Animator>().GetFloat("Vertical"));
            }
            yield return new WaitForSeconds(Delay);
            _currentPings++;
        }
        
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void PlayPingClientRpc()
    {
        GetComponent<AudioSource>().Play();
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void SpawnGhostClientRpc(Vector3 pos, float horizontal, float vertical)
    {
        if (GhostPrefab == null) return;
        GameObject ghost = Instantiate(GhostPrefab, pos, Quaternion.identity);
        ghost.GetComponent<Animator>().SetFloat("Horizontal", horizontal);
        ghost.GetComponent<Animator>().SetFloat("Vertical", vertical);
        
    }
}
