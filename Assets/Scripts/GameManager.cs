

class GameManager : NetworkSingleton<GameManager>
{
    public bool IsHostClient = false;

    public void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void Start()
    {
        
    }
}