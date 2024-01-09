using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class Junction : Graph
{
    // 1.5 1.5
    public Node center;
    public string labelForTopestLeft;
    public Node topestLeft { get => center.Shift(-0.5f, 1.5f).WithLabel(labelForTopestLeft); }
    public Node topLeftest { get => center.Shift(-1.5f, 0.5f); }
    public Node topestRight { get => center.Shift(0.5f, 1.5f); }
    public Node topRightest { get => center.Shift(1.5f, 0.5f); }
    public Node bottomestLeft { get => center.Shift(-0.5f, -1.5f); }
    public Node bottomLeftest { get => center.Shift(-1.5f, -0.5f); }
    public Node bottomestRight { get => center.Shift(0.5f, -1.5f); }
    public Node bottomRightest { get => center.Shift(1.5f, -0.5f); }

    public Junction(Node center, string labelForTopestLeft = null)
    {
        this.center = center;
        this.labelForTopestLeft = labelForTopestLeft;
        AddEdges(bottomestRight, bottomRightest, topestRight, topLeftest, bottomestLeft);
        AddEdges(topestLeft, topLeftest, bottomRightest, bottomestLeft, topestRight);
        AddEdges(bottomLeftest, topestRight, bottomRightest, bottomestLeft, topLeftest);
        AddEdges(topRightest, topestRight, topLeftest, bottomestLeft, bottomRightest);
        AddEdges(topLeftest, bottomLeftest);
        AddEdges(topestRight, topestLeft);
        AddEdges(bottomestLeft, bottomestRight);
        AddEdges(bottomRightest, topRightest);
    }
}
