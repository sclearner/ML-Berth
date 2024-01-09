using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Task
{
    public QuayController controller;
    public DestinationNodes destination;
    public Vehicle vehicle;
    public int status = -1; //-1 for no sign, 0 for get_container, 1 for go_to_destination, 2 for done
    // Start is called before the first frame update
    public Task(QuayController controller, DestinationNodes destination)
    {
        this.controller = controller;
        this.destination = destination;
    }

    public void UpdateStatus()
    {
        if (!vehicle.State.isRunning && !vehicle.State.isWaiting)
        {
            if (vehicle.State.hasContainer)
            {
                vehicle.Driver.GetSchedule(destination.nodes.ToArray());
                vehicle.State.isRunning = true;
            }
            else
            {
                vehicle.Driver.GetSchedule(controller.vehicleNode);
                vehicle.State.isRunning = true;
            }
        }
    }
}
