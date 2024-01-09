using System;
using System.Linq;
using UnityEngine;

public class QuayController : MonoBehaviour
{
    private Animator animator;
    private BoxCollider2D shipCollider;
    private BoxCollider2D vehicleCollider;
    public GraphGenerator generator;
    [HideInInspector]
    public int index;
    public Node waterNode;
    public Node vehicleNode;
    public Vehicle vehicle;
    public Ship ship;
    public bool isBusy = false;
    private void Start()
    {
        animator = GetComponent<Animator>();
        SetGenerator();
    }

    public void SetGenerator()
    {
        string waterLabel = $"W{name.Split(' ').Last()[1..]}";
        string vehicleLabel = $"N{name.Split(' ').Last()[1..]}";
        index = Int32.Parse(name.Split(' ').Last()[2..]);
        waterNode = generator.graph.FindLabel(waterLabel).First();
        vehicleNode = generator.graph.FindLabel(vehicleLabel).First();
        Debug.Log(waterLabel);
        shipCollider = GetComponentInParent<BoxCollider2D>();
        vehicleCollider = GetComponentInChildren<BoxCollider2D>();
        shipCollider.offset = waterNode.ToVector2 - (Vector2)transform.position;
        vehicleCollider.offset = vehicleNode.ToVector2 - (Vector2)transform.position;
    }

    private void Update()
    {
        animator.SetBool("vehicleHasContainer", vehicle?.State.hasContainer ?? false);
        animator.SetBool("shipHasContainer", (ship?.State.containerCount ?? 0) > 0);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Transform that = collision.transform;
        if (that.CompareTag("Ship"))
        {
            ship = that.GetComponent<Ship>();
        }
        else if (that.CompareTag("Vehicle"))
        {
            vehicle = that.GetComponent<Vehicle>();
            isBusy = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Transform that = collision.transform;
        if (that.CompareTag("Ship"))
        {
            ship = null;
        }
        else if (that.CompareTag("Vehicle"))
        {
            vehicle = null;
            isBusy = false;
        }
    }
}
