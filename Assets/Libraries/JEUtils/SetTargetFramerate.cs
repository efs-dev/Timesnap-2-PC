using UnityEngine;

public class SetTargetFramerate : MonoBehaviour
{
    public int TargetFrameRate = 60;
    
    private void Start()
    {
        Application.targetFrameRate = TargetFrameRate;
    }
}