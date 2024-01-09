using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public partial class Graph
{
    public List<Node> AstarFindPath(Node start, params Node[] ends)
    {
        if (ends.Contains(start)) return new() {start};
        var openSet = new PriorityQueue<Node, float>();

        var cameFrom = new Dictionary<Node, Node>();
        var gScore = new Dictionary<Node, float>();
        foreach (var node in instance.Keys)
        {
            gScore[node] = float.MaxValue;
        }
        gScore[start] = 0;
        openSet.Enqueue(start, 0);
        while (openSet.Count > 0)
        {
            var node = openSet.Dequeue();
            if (ends.Contains(node))
            {
                return ReconstructPath(cameFrom, node);
            }

            foreach (var neighbor in instance[node])
            {
                var tentativeGScore = gScore[node] + Node.Manhattan(node, neighbor);

                if (tentativeGScore < gScore[neighbor])
                {
                    cameFrom[neighbor] = node;
                    gScore[neighbor] = tentativeGScore;
                    var fScore = tentativeGScore + Mathf.Min(ends.Select(end => Node.Manhattan(neighbor, end)).ToArray());
                    openSet.Enqueue(neighbor, fScore);
                }
            }
        }
        return null;
    }

    private List<Node> ReconstructPath(Dictionary<Node, Node> cameFrom, Node current)
    {
        var path = new List<Node> {current};
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            path.Add(current);
        }
        return path;
    }

    public Node NearestNode(Vector2 location)
    {
        Node nearestNode = null;
        float minDistance = float.MaxValue;

        foreach (var node in instance.Keys) {
            float distance = (location - node.ToVector2).magnitude;
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestNode = node;
            }
        }
        return nearestNode;
    }
}
