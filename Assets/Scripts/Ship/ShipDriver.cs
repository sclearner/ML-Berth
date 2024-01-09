using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class ShipDriver : MonoBehaviour
{
    public GraphGenerator map;
    public float speed = 1f;

    [HideInInspector]
    public bool isPause = false;
    [HideInInspector]
    public int nextQuay;

    private Node nextNode;
    private Coroutine coroutine;

    private List<Node> schedule;
    private ShipState shipState;
    public bool HasSchedule
    {
        get
        {
            return schedule != null;
        }
    }


    bool NewNextNode
    {
        get
        {
            if (nextNode == null) return false;
            return (new Vector2(transform.position.x, transform.position.y) - nextNode.ToVector2).magnitude < 1e-3f;
        }
    }

    public void GetSchedule(Node finish)
    {
        Node start = new Node(transform.position.x, transform.position.y);
        schedule = new List<Node>() {finish, finish.Shift(-5f, 0)};
        nextNode = start;
    }

    public void GetToQuay(QuayController quay)
    {
        GetSchedule(quay.waterNode);
        nextQuay = quay.index;
        
    }

    public void GetAwayFromQuay()
    {
        Node finish = new(Random.Range(-100f, 100f), nextNode.Shift(0, -100f).y);
        GetSchedule(finish);
        nextQuay = 0;
    }

    private IEnumerator GoToAnimated(Vector3 target)
    {
        var relativePos = target - transform.position;
        var timeToGo = relativePos.magnitude / speed;
        var factor = 0.0f;

        while (factor < timeToGo && Vector3.Angle((target - transform.position).normalized, relativePos.normalized) < 89f)
        {
            while (isPause) yield return new WaitForSeconds(Time.deltaTime);
            transform.position += Time.smoothDeltaTime * speed * relativePos.normalized;
            transform.right = relativePos.normalized;
            factor += Time.smoothDeltaTime;
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
        shipState = GetComponent<ShipState>();
    }

    // Update is called once per frame
    void LateUpdate()
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
        if (that.CompareTag("Ship"))
        {
            Debug.LogWarning($"{name} collides {that.name} with angle {Mathf.Abs(Vector3.Angle(that.position - transform.position, transform.right))}.");
            isPause = Mathf.Abs(Vector3.Angle(that.position - transform.position, transform.right)) <= 90f || !(thatDriver.isPause);
            if (thatDriver.isPause) isPause = false;
        }
        else if (that.CompareTag("Quay"))
        {
            that.GetComponent<Animator>().SetBool("hasShip", true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        isPause = false;
        Transform that = collision.transform;
        if (that.CompareTag("Quay"))
        {
            that.GetComponent<Animator>().SetBool("hasShip", false);
        }
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
