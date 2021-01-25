using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotate : MonoBehaviour
{
    public float speed;

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.Rotate(Time.deltaTime * 0, 0, speed);
    }
}
