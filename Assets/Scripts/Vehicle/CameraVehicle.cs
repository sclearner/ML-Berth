using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class CameraVehicle : MonoBehaviour
{
    public GameObject vehicle;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 position = vehicle.transform.position;
        transform.position = new(position.x, position.y, transform.position.z);  
    }
}
