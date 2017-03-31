using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Node : MonoBehaviour
{
    public List<Node> neighbours = new List<Node>();
    public void ConnectTo(Node other)
    {
        if(!neighbours.Contains(other))
        {
            neighbours.Add(other);
        }
        if (!other.neighbours.Contains(this))
        {
            other.neighbours.Add(this);
        }
    }
}
