using System.Collections;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;

public class Vehicle : MonoBehaviour
{
    private VehicleDriver driver;
    private VehicleState state;
    internal Task task;
    private Animator animator1;
    public VehicleDriver Driver
    {
        get => driver;
    }
    public VehicleState State
    {
        get => state;
    }
    // Start is called before the first frame update
    void Awake()
    {
        driver = GetComponent<VehicleDriver>();
        state = GetComponent<VehicleState>();
    }

    void Update()
    {
        animator1?.SetBool("vehicleHasContainer", state.hasContainer);
        if (state.isBreak)
        {
            animator1?.SetBool("hasVehicle", false);
            driver.StopAllCoroutines();
        }
        if (task != null)
        {
            switch (task.status)
            {
                case 0:
                    if (!driver.HasSchedule && !State.isRunning && !State.isWaiting)
                    {
                        driver.GetSchedule(task.controller.vehicleNode);
                        State.isRunning = true;
                    }
                    break;
                case 1:
                    if (!State.isRunning && State.isWaiting)
                    {
                        driver.GetSchedule(task.destination.nodes.ToArray());
                        State.isRunning = true;
                        State.isWaiting = false;
                    }
                    break;
                case 2:
                    break;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Transform that = collision.transform;
        if (that.CompareTag("QuayStop"))
        {
            state.isWaiting = true;
            state.isRunning = false;
            animator1 = that.parent.GetComponent<Animator>();
            animator1.SetBool("hasVehicle", true);
            StartCoroutine(TakeContainer());
        }
        if (that.CompareTag("Container"))
        {
            if (state.hasContainer)
            {
                state.hasContainer = false;
                task.status++;
                state.isRunning = false;
            }
        }
    }

    private IEnumerator TakeContainer()
    {
        yield return new WaitForSeconds(5);
        task.status = 1;
        state.hasContainer = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Transform that = collision.transform;
        if (that.CompareTag("QuayStop"))
        {
            animator1.SetBool("hasVehicle", false);
            animator1 = null;
        }
    }
}
