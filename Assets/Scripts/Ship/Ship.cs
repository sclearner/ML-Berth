using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour
{
    private ShipDriver driver;
    private ShipState state;
    private Animator animator1;
    public ShipDriver Driver
    {
        get => driver;
    }
    public ShipState State
    {
        get => state;
    }
    // Start is called before the first frame update
    void Awake()
    {
        driver = GetComponent<ShipDriver>();
        state = GetComponent<ShipState>();
    }
}
