using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class VehicleDriver : MonoBehaviour
{
    public GraphGenerator map;
    public float speed = 1f;

    [HideInInspector]
    public bool isPause = false;
    private Node nextNode;
    private Coroutine coroutine;

    private List<Node> schedule;
    public bool HasSchedule { get {
            return schedule != null;
        } }   
    

    bool NewNextNode
    {
        get { 
            if (nextNode == null) return false;
            return (new Vector2(transform.position.x, transform.position.y) - nextNode.ToVector2).magnitude < 1e-3f;
        } 
    }

    public void GetSchedule(params Node[] finish)
    {
        Node start = map.graph.NearestNode(transform.position);
        schedule = map.graph.AstarFindPath(start, finish);
        nextNode = start;
    }

    private IEnumerator GoToAnimated(Vector3 target)
    {
        var relativePos = target - transform.position;
        var timeToGo = relativePos.magnitude / speed;
        var factor = 0.0f;
        
        while (factor < timeToGo && Vector3.Angle((target - transform.position).normalized, relativePos.normalized) < 89f)
        {
            while (isPause) yield return new WaitForSeconds(Time.deltaTime);
            transform.position += Time.deltaTime * speed * relativePos.normalized;
            transform.right = relativePos.normalized;
            factor += Time.deltaTime;
            yield return 1;
        }
        transform.position = target;
        coroutine = null;
        yield return 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        nextNode = new Node(transform.position.x, transform.position.y);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (schedule == null) return;
        if (NewNextNode)
        {
            coroutine = null;
            if (schedule.Count > 0)
            {
                nextNode = schedule[^1];
                schedule.RemoveAt(schedule.Count - 1);
            }
            else schedule = null;
        }
        else if (coroutine == null && nextNode != null) coroutine = StartCoroutine(GoToAnimated(nextNode.ToVector3(transform.position.z)));
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        Transform that = collision.transform;
        VehicleDriver thatDriver = that.GetComponent<VehicleDriver>();
        Debug.LogWarning($"{name} collides {that.name} with angle {Mathf.Abs(Vector3.Angle(that.position - transform.position, transform.right))}.");
        //if (that.CompareTag("Vehicle"))
        //{
        //    isPause = Mathf.Abs(Vector3.Angle(that.position - transform.position, transform.right)) <= 90f || !(thatDriver.isPause);
        //    if (thatDriver.isPause) isPause = false;
        //}
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        isPause = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        if (schedule == null) return;
        Gizmos.DrawLine(transform.position, nextNode.ToVector3(transform.position.z));
        if (schedule.Count > 0)
        {
            Gizmos.DrawLine(nextNode.ToVector3(transform.position.z), schedule.Last().ToVector3(transform.position.z));
            for (int i = schedule.Count - 1; i >= 1; i--)
            {
                Gizmos.DrawLine(schedule[i].ToVector3(transform.position.z), schedule[i - 1].ToVector3(transform.position.z));
            }
        }
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, (nextNode.ToVector3(transform.position.z) - transform.position).normalized);
    }
}
