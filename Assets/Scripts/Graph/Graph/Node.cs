using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class Node
{
    public float x;
    public float y;
    public string label;

    public Node WithLabel(string newLabel)
    {
        return new Node(x, y, newLabel);
    } 

    public Node(float x, float y, string label = null) {
        this.x = x;
        this.y = y;
        this.label = label;
    }

    public Node Shift(float x, float y)
    {
        return new Node(this.x + x, this.y + y, this.label);
    }

    public static float Manhattan(Node a, Node b)
   {
        return math.abs(a.x - b.x) + math.abs(a.y - b.y);
   }

    public Vector3 ToVector3(float z)
    {
        return new Vector3(x, y, z);
    }

    public Vector2 ToVector2
    {
        get => new Vector2(x, y);
    }

    public override bool Equals(object obj)
    {
        if (obj is not Node) return false;
        else return x == ((Node)obj).x && y == ((Node)obj).y;
    }

    public override int GetHashCode()
    {
        return x.GetHashCode() ^ y.GetHashCode();
    }

    static public Node FromVector(Vector2 v, string label = null)
    {
        return new Node(v.x, v.y, label);
    }
}
