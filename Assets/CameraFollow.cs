using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Vector3 followOffset;
    public float lerpConstant;
    public Camera camera;


    // Update is called once per frame
    void Update()
    {
        var forward = gameObject.transform.forward;
        camera.transform.forward = forward;
        camera.transform.position = Vector3.Lerp(camera.transform.position, transform.position + followOffset, lerpConstant * Time.deltaTime);
    }
}
