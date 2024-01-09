using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Windows;

public class Manager : MonoBehaviour
{
    private List<Vehicle> vehicles = new();
    private List<QuayController> quays;
    private List<DestinationNodes> destinations;
    public GraphGenerator graph;
    private List<List<Task>> tasks = new();
    private List<Node> nodes = new();
    public int TaskCount
    {
        get => tasks.Select((x) => x.Count).Sum();
    }
    public TextAsset schedule;
    public string label;

    public GameObject vehiclePrototype;

    private void Awake()
    {
        for (int i = 1; i <=12; i++)
        {
            GameObject gameObject = Instantiate(vehiclePrototype, new Vector3(-45.8333f + 8.3333f*(i-1), 3, 0), transform.rotation);
            gameObject.name = $"Vehicle{i}";
            gameObject.GetComponent<VehicleDriver>().map = graph;
            vehicles.Add(gameObject.GetComponent<Vehicle>());
        }
    }
    // Start is called before the first frame update
    IEnumerator Start()
    {
        quays = GameObject.FindGameObjectsWithTag("Quay").Select(x => x.GetComponent<QuayController>()).OrderBy(x => x.name).ToList();
        destinations = GameObject.FindGameObjectsWithTag("Container").Select(x => x.GetComponent<DestinationNodes>()).ToList();
        List<string> rawTask = schedule.text.Split("\r\n").ToList();
        for (int i = 0; i < vehicles.Count; i++) {
            tasks.Add(new());
        }
        foreach (string task in rawTask)
        {
            string[] element = task.Split(' ');
            int vehicleIndex = int.Parse(element[0]);
            int quay = int.Parse(element[1]);
            Node desti = new((float.Parse(element[2]) - 1600) / 36, 44.69191465f-float.Parse(element[3])*0.027778489f);
            nodes.Add(desti);
            DestinationNodes destinationNode = destinations.OrderBy((x) => Mathf.Min(x.nodes.Select((y) => Node.Manhattan(y, desti)).ToArray())).First();
            tasks[vehicleIndex].Add(new(quays[quay-1], destinationNode));
        }
        yield return new WaitForSecondsRealtime(5);
    }

    private void Update()
    {
        for (int i=0; i<vehicles.Count; i++)
        {
            Vehicle vehicle = vehicles[i];
            if (vehicle.State.isBreak) continue;
            if (vehicle.task != null)
            {
                if (vehicle.task.status == 2)
                {
                    tasks[i].RemoveAt(0);
                    vehicle.task = null;
                }
            }
            else if (vehicle.task == null && tasks[i].Count > 0)
            {
                Task task = tasks[i].First();
                if (task.controller.isBusy) continue;
                task.controller.isBusy = true;
                vehicle.task = task;                
                vehicle.task.vehicle = vehicle;
                vehicle.task.status = 0;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        foreach (Node node in nodes)
        {
            Gizmos.DrawSphere(node.ToVector3(transform.position.z), 0.1f);
        }
    }
}
