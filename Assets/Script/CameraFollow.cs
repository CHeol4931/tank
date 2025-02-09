using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    public Vector3 offset = new Vector3(0, 5, -10);

    void Start()
    {
        
    }

    void Update()
    {
        this.transform.position = player.position + offset;
    }
}
