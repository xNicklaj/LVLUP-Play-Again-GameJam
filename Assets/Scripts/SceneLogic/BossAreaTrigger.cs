using UnityEngine;
using UnityEngine.Tilemaps;

public class BossBattleTrigger : MonoBehaviour
{
    public int players=0;
    [SerializeField] private GameObject gateToClose;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Player entrato"+other.gameObject.name);
        players++;
        if (players >= 3) {
            gateToClose.GetComponent<TilemapRenderer>().enabled = true;
            gateToClose.GetComponent<TilemapCollider2D>().enabled = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        players--;
    }
    
}
