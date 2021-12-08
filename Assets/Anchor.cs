using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Anchor : MonoBehaviour
{
    public Vector2 ScreenPoint;
    public float Z = 1;
    // Start is called before the first frame update
    void Update()
    {
        transform.position = Camera.main.ViewportToWorldPoint(new Vector3(ScreenPoint.x, ScreenPoint.y, Z));
    }

}
