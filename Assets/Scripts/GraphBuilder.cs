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
    //public GraphBuilder(GameObject gameObject) :
    //    base(gameObject)
    //{
    //}

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


        Waypoint myStartNode = new Waypoint();
        myStartNode = GameObject.FindGameObjectWithTag("Start").GetComponent<Waypoint>();

        // add nodes (all waypoints, including start and end) to graph

        Waypoint myEndNode = new Waypoint();
        myEndNode = GameObject.FindGameObjectWithTag("End").GetComponent<Waypoint>();

        var wayPointsOther = GameObject.FindGameObjectsWithTag("Waypoint");


        graph.AddNode(myStartNode);

        foreach (var waypoint in wayPointsOther)
        {
            Waypoint myOtherFlag = waypoint.GetComponent<Waypoint>();
            graph.AddNode(myOtherFlag);
        }

        graph.AddNode(myEndNode);
        Debug.Log("My node list" + graph.Nodes + " count " + graph.Count);


        // add neighbors to each node in graph
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