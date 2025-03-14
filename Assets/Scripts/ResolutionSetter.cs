using UnityEngine;

public class ResolutionSetter : MonoBehaviour
{
    public int Width = 740;
    public int Height = 600;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Screen.SetResolution(this.Width, this.Height, true);   
    }
}
