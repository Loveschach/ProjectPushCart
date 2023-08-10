using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Vector3 followOffset;
    public float lerpConstant;
    public Camera camera;

    public int averagingWindowFrames = 10;
    Vector3[] previousDirections;

    private void Start()
    {
        previousDirections = new Vector3[averagingWindowFrames];

        for(int i = 0; i < averagingWindowFrames; i++)
        {
            previousDirections[i] = transform.forward;
        }
    }

    // Update is called once per frame
    void Update()
    {
        previousDirections[Time.frameCount % averagingWindowFrames] = transform.forward;

        var averageDirection = Vector3.zero;
        for (int i = 0; i < averagingWindowFrames; i++)
        {
            averageDirection += previousDirections[i];
        }

        averageDirection.Normalize();

        camera.transform.forward = averageDirection;
        var offset = camera.transform.forward * followOffset.z + camera.transform.right * followOffset.x + Vector3.up * followOffset.y;


  

        camera.transform.position = Vector3.Lerp(camera.transform.position, transform.position + offset, lerpConstant * Time.deltaTime);
    }
}
