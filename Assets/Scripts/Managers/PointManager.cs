using System.Collections;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PointManager : NetworkSingleton<PointManager>
{
    public int InitialScore = 1000;

    public NetworkVariable<int> CurrentScore = new NetworkVariable<int>();

    public NetworkVariable<int> HighScore = new NetworkVariable<int>();
    public int SoundThreshold = 1000;
    private float Timer = 0;
    private int _localHighScore;
    private bool _isCounting = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void OnNetworkSpawn()
    {
        if (!IsHost) return;
        HighScore.Value = PlayerPrefs.GetInt("HighScore");
        GameManager_v2.Instance.OnGameStart.AddListener(() =>
        {
            ResetScore();
            StartCoroutine(DecreaseScore());
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResetScore()
    {
        SetScoreServerRpc(InitialScore);
    }

    [Rpc(SendTo.Server)]
    public void SetScoreServerRpc(int score)
    {
        if (!_isCounting) return;
        if (score < 0) score = 0;
        if (score > _localHighScore)
        {
            if(((int)score) / ((int)SoundThreshold) > ((int)_localHighScore) / ((int)SoundThreshold))
            {
                GetComponent<AudioSource>().time = 0;
                GetComponent<AudioSource>().Play();
            }
            _localHighScore = score;
        }
        CurrentScore.Value = score;
        RefreshScoreClientRpc(score);
    }

    [Rpc(SendTo.Server)]
    public void AddScoreServerRpc(int score)
    {
        SetScoreServerRpc(CurrentScore.Value + score);
    }

    [Rpc(SendTo.Server)]
    public void StopCountingServerRpc()
    {
        _isCounting = false;
    }

    private float GetNextDelay()
    {
        return Timer switch
        {
            <= 60 => 10f,
            <= 120 => 8f,
            <= 180 => 6f,
            _ => 10
        };
    }

    private int GetNextPointDecrease()
    {
        return Timer switch
        {
            <= 60 => 50,
            <= 90 => 70,
            <= 120 => 100,
            <= 180 => 120,
            > 180 => 150,
            _ => 100
        };
    }

    private IEnumerator DecreaseScore()
    {
        while (GameManager_v2.Instance.GameStarted)
        {
            float delay = GetNextDelay();
            yield return new WaitForSeconds(delay);
            Timer += delay;
            SetScoreServerRpc(CurrentScore.Value - GetNextPointDecrease());
        }
        
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void RefreshScoreClientRpc(int value)
    {
        GameManager_v2.Instance.OnPointsUpdated.Invoke(value);
    }
}
