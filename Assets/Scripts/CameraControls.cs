using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CameraController : MonoBehaviour
{
    public Transform cube1;
    public Transform cube2;
    public Transform cube3;
    public Transform cube4;
    public float offset;

    void Update()
    {
        // Calculate the center point between the four cubes
        Vector3 center = (cube1.position + cube2.position + cube3.position + cube4.position) / 4;

        // Set the camera to the calculated center, and move it backward to fit all cubes in view
        Camera.main.transform.position = center + new Vector3(30, transform.position.y, -10);

        // Ensure the camera looks at the center point
        Camera.main.transform.LookAt(center);
        transform.rotation = Quaternion.Euler(90,90,0);
        // Optionally, adjust camera field of view if needed to fit all cubes
        Camera.main.fieldOfView = 60;        
    }
}
