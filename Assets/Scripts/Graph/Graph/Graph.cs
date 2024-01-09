using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public partial class Graph { 

    public Dictionary<Node, List<Node>> instance = new();
    private List<Node> source = null;
    private List<Node> target = null;

    public List<Node> GetSource
    {
        get => source ??= FindLabel("NS");
    }

    public List<Node> GetTarget
    {
        get => target ??= FindLabel("ND");
    }

    public List<Node> FindLabel(string label)
    {
        List<Node> list = new();
        foreach (Node node in instance.Keys) { 
            if (node.label?.Contains(label) ?? false)
            {
                list.Add(node);
            }
        }
        return list;
    }
    
    public void Merge(Graph that)
    {
        foreach (var key in that.instance.Keys)
        {
            if (!instance.ContainsKey(key)) instance.Add(key, that.instance[key]);
            else instance[key].AddRange(that.instance[key]);
        }
    }

    public Graph ZoomIn(float x, Vector2 center)
    {
        if (x <= 0) throw new UnityException();
        Graph result = new();
        foreach (var node in instance.Keys)
        {
            Node newNode = new((node.x - center.x)* x + center.x, (node.y - center.y) * x + center.y, node.label);
            result.instance.Add(newNode, new());
            foreach (var toNode in instance[node])
            {
                Node newToNode = new((toNode.x - center.x) * x + center.x, (toNode.y - center.y) * x + center.y, toNode.label);
                result.instance[newNode].Add(newToNode);
            }
        }
        return result;
    }

    public Graph Move(Vector2 center)
    {
        Graph result = new();
        foreach (var node in instance.Keys)
        {
            Node newNode = new(node.x + center.x, node.y + center.y, node.label);
            result.instance.Add(newNode, new());
            foreach (var toNode in instance[node])
            {
                Node newToNode = new(toNode.x + center.x, toNode.y + center.y, toNode.label);
                result.instance[newNode].Add(newToNode);
            }
        }
        return result;
    }

    public void AddEdges(Node from,params Node[] to)
    {    
        if (to == null)
        {
            if (!instance.ContainsKey(from)) instance[from] = new();
        }
        if (instance.ContainsKey(from))
        {
            instance[from].AddRange(to);
        }
        else
        {
            instance[from] = to.ToList<Node>();
        }
    }
}
