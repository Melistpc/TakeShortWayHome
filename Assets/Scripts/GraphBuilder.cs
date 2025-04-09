using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Builds the graph
/// </summary>
public class GraphBuilder : MonoBehaviour
{
    static Graph<Waypoint> graph;

    #region Constructor

    // Uncomment the code below after copying this class into the console
    // app for the automated grader. DON'T uncomment it now; it won't
    // compile in a Unity project

    /// <summary>
    /// Constructor
    /// 
    /// Note: The GraphBuilder class in the Unity solution doesn't 
    /// use a constructor; this constructor is to support automated grading
    /// </summary>
    /// <param name="gameObject">the game object the script is attached to</param>
  

    #endregion

    /// <summary>
    /// Awake is called before Start
    ///
    /// Note: Leave this method public to support automated grading
    /// </summary>
    public void Awake()
    {
        if (graph == null)
        {
            Debug.Log("Graph is null");
            graph = new Graph<Waypoint>();
        }


        Waypoint myStartNode;
        myStartNode = GameObject.FindGameObjectWithTag("Start").GetComponent<Waypoint>();


        // add nodes (all waypoints, including start and end) to graph

        Waypoint myEndNode;
        myEndNode = GameObject.FindGameObjectWithTag("End").GetComponent<Waypoint>();

        var wayPointsOther = GameObject.FindGameObjectsWithTag("Waypoint");


        graph.AddNode(myStartNode);

        foreach (var waypoint in wayPointsOther)
        {
            graph.AddNode(waypoint.GetComponent<Waypoint>());
        }

        graph.AddNode(myEndNode);
        

        
        // add neighbors to each node in graph
        if (graph.Nodes.Count == 2 && graph.Nodes[0].Value==myStartNode && graph.Nodes[1].Value==myEndNode)
        {
        
            float distance=Mathf.Abs(GetDistance(graph.Nodes[0], graph.Nodes[1]).magnitude);
            Debug.Log("only start and end" + distance);
            graph.Nodes[0].AddNeighbor(graph.Nodes[1],distance);
            graph.Nodes[1].AddNeighbor(graph.Nodes[0],distance);
            
        }
   
        else 
        {
            foreach (var node in graph.Nodes)
            {
           
                CheckNodesDistance(node);
            
            }

        }
     

      
        Debug.Log("awake finished graphbuilder");
    }

    private void CheckNodesDistance(GraphNode<Waypoint> firstNode)
    {
        
        foreach (GraphNode<Waypoint> secondNode in graph.Nodes)
        {
            if (secondNode != firstNode)
            {
                Vector2 distance = GetDistance(firstNode, secondNode);
                if (Mathf.Abs(distance.x) <= 3.5f && Mathf.Abs(distance.y) <= 3.0f)
                {
                    firstNode.AddNeighbor(secondNode, distance.magnitude);
                }
                //Diyelim ki sadece 2 node var biri star diğeri end.ikisi arasında distance daha büyük olsa
                //bunu almıycak bu case i ekle.
                
                
            }
        }
    }

    private Vector2 GetDistance(GraphNode<Waypoint> node, GraphNode<Waypoint> node2)
    {
        Vector2 currentNodePos = node.Value.Position;
        Vector2 otherNodePos = node2.Value.Position;
        return currentNodePos - otherNodePos;
    }


    /// <summary>
    /// Gets and sets the graph
    /// 
    /// CAUTION: Set should only be used by the autograder
    /// </summary>
    /// <value>graph</value>
    public static Graph<Waypoint> Graph
    {
        get { return graph; }
        set { graph = value; }
    }
}