using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Destination : Graph
{
    public Node center;

    const float shift = 2;

    public Node top { get => center.Shift(0, shift); }

    public Node bottom { get => center.Shift(0, -shift); }

    //public Node left { get => center.Shift(-shift, 0); }

    //public Node right { get => center.Shift(shift, 0); }

    public Destination(Node center)
    {
        this.center = center;
        instance.Add(top, new List<Node>());
        instance.Add(bottom, new List<Node>());
        //instance.Add(left, new List<Node>());
        //instance.Add(right, new List<Node>());
        instance.Add(center.WithLabel($"PD{center.label[2..]}"), new List<Node>());
    }
}
