using Newtonsoft.Json;
using System;
using System.Linq;
using UnityEngine;
public class GraphGenerator : MonoBehaviour
{
    private Vector2 topLeft = new(1.5f, 2f);
    public int junctionsOnRow = 9;
    public int junctionsOnColumn = 4;
    private float scale;
    private Vector3 position;

    public GameObject container;
    public GameObject quay;

    public Graph topDestinations = new();

    private float Scale
    {
        get => transform.localScale.x / (6 * junctionsOnRow - 6 + 3f);
    }

    private Graph rawGraph = new();
    public Graph graph;

    void Awake()
    {
        DeleteAllPrefabs();
        //topLeft += new Vector2(transform.position.x, -transform.position.y);
        Junction[,] junctions = new Junction[junctionsOnRow, junctionsOnColumn];
        //Generate junctions
        for (int i = 0; i < junctionsOnColumn; i++)
        {
            for (int j = 0; j < junctionsOnRow; j++)
            {
                junctions[j, i] = new Junction(new Node(6 * j + topLeft.x, -(6 * i + topLeft.y)), (i==0 && j > 0)? $"NS{j}" : null);
                rawGraph.Merge(junctions[j, i]); 
                if (j > 0)
                {
                    Junction left = junctions[j - 1, i], right = junctions[j, i];
                    rawGraph.instance[right.topLeftest].Add(left.topRightest);
                    rawGraph.instance[left.bottomRightest].Add(right.bottomLeftest);
                }
                if (i == 0 && j > 0)
                {
                    Node quayLocation = new(junctions[j, i].topestLeft.x - 1.25f, junctions[j, i].topestLeft.y, $"PS{j}");
                    Node shipImport = new(quayLocation.x, quayLocation.y + 1.25f, $"WS{j}");
                    rawGraph.instance.Add(quayLocation, new());
                    rawGraph.instance.Add(shipImport, new());
                }
            }
            if (i > 0)
            {
                for (int j = 0; j < junctionsOnRow; j++)
                {
                   Junction top = junctions[j, i - 1], bottom = junctions[j, i];
                   rawGraph.instance[top.bottomestLeft].Add(bottom.topestLeft);
                   rawGraph.instance[bottom.topestRight].Add(top.bottomestRight);
                }
            }
        }
        Destination[,] destinations = new Destination[junctionsOnRow - 1, junctionsOnColumn - 1];
        for (int i = 0; i < junctionsOnColumn - 1; i++)
        {
            for (int j = 0; j < junctionsOnRow - 1; j++)
            {
                destinations[j, i] = new Destination(new Node(3 + 6 * j + topLeft.x, -(3 + 6 * i + topLeft.y), $"ND{i},{j}"));
                rawGraph.Merge(destinations[j, i]);
                Destination current = destinations[j, i];
                Junction jTopLeft = junctions[j, i], topRight = junctions[j + 1, i], bottomLeft = junctions[j, i+1], bottomRight = junctions[j+1, i+1];
                rawGraph.instance[jTopLeft.bottomRightest].Add(current.top);
                rawGraph.instance[current.top].Add(topRight.bottomLeftest);
                //rawGraph.instance[topRight.bottomestLeft].Add(current.right);
                //rawGraph.instance[current.right].Add(bottomRight.topestLeft);
                rawGraph.instance[bottomRight.topLeftest].Add(current.bottom);
                rawGraph.instance[current.bottom].Add(bottomLeft.topRightest);
                topDestinations.instance.Add(current.top, new());
                //rawGraph.instance[bottomLeft.topestRight].Add(current.left);
                //rawGraph.instance[current.left].Add(jTopLeft.bottomestRight);
            }
        }
        rawGraph = rawGraph.Move(transform.position);
        graph = rawGraph.ZoomIn(Scale, transform.position);
        topDestinations = topDestinations.Move(transform.position);
        topDestinations = topDestinations.ZoomIn(36f*Scale, transform.position);
        scale = Scale;
        position = transform.position;
        DrawPrefabs();
        DownloadToJson();
    }

    void Start()
    {
        
    }

    void Update()
    { 
        if (scale != Scale)
        {
            graph = rawGraph.ZoomIn(Scale, transform.position);
            scale = Scale;
            DrawPrefabs();
        }
        if (position != transform.position)
        {
            rawGraph = rawGraph.Move(transform.position - position);
            position = transform.position;
            graph = rawGraph.ZoomIn(Scale, position);
            DrawPrefabs();
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        if (graph == null) return;
        foreach (var from in graph.instance.Keys)
        {
            Gizmos.DrawSphere(from.ToVector3(transform.position.z), 0.1f * Scale);
            foreach (var to in graph.instance[from])
            {
                Gizmos.DrawRay(from.ToVector3(transform.position.z), (to.ToVector3(transform.position.z) - from.ToVector3(transform.position.z)));
                
            }
        }
    }

    private void DrawPrefabs()
    {
        if (transform.childCount > 0)
        {
            if (!Application.isPlaying) return;
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);                
            }
        }
        foreach (Node node in graph.instance.Keys)
        {
            if (node.label?.Contains("PS") ?? false)
            {
                GameObject newObject = Instantiate(quay, transform, true);
                newObject.GetComponent<QuayController>().generator = this;
                newObject.name = $"Quay {node.label}";
                newObject.transform.position = node.ToVector3(transform.position.z - 1);
                newObject.transform.localScale = Vector2.one / transform.localScale.x;
                newObject.GetComponent<QuayController>().SetGenerator();
            }
            else if (node.label?.Contains("PD") ?? false)
            {
                GameObject newObject = Instantiate(container, transform, true);
                newObject.transform.position = node.ToVector3(transform.position.z - 1);
                newObject.name = $"Container {node.label}";
                newObject.GetComponent<DestinationNodes>().nodes = graph.FindLabel($"N{node.label[1..]}");
                newObject.transform.localScale = new Vector3(0.4f * Scale / transform.localScale.x, 0.4f * Scale / transform.localScale.y, 1);
                Debug.Log(newObject.transform.position);
            }
        }
    }

    private void DeleteAllPrefabs()
    {
        foreach (Transform child in transform)
        {
           DestroyImmediate(child.gameObject);
        }
    }

    public void DownloadToJson()
    {
        string json = String.Join('\n', topDestinations.instance.Keys.Select(x => $"{x.x} {x.y}").ToList());
        string filePath = Application.persistentDataPath + "/" + "Data.txt";
        Debug.Log(filePath);
        System.IO.File.WriteAllText(filePath, json);
    }
}
