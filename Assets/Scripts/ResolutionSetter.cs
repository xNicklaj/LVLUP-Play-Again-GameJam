using UnityEngine;

public class ResolutionSetter : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Screen.SetResolution(800, 600, true);   
    }
}
