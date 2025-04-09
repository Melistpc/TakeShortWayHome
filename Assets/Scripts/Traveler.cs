using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;


/// <summary>
/// A traveler
/// </summary>
public class Traveler : MonoBehaviour
{
    #region Fields

    float pathLength = 0;
    private LinkedList<Waypoint> path;
   
    PathFoundEvent pathFoundEvent = new PathFoundEvent();
    PathTraversalCompleteEvent pathTraversalCompleteEvent = new PathTraversalCompleteEvent();
    private bool IsMoving;
     public LinkedList<Waypoint> explodeWaypoints;
    private Rigidbody2D rigidbody2D;
    private Explosion explodeprefab;
    private GameObject explosion;
   

    #endregion

// Uncomment the code below after you copy this class into the console
// app for the automated grader. DON'T uncomment it now; it won't
// compile in a Unity project

    /// <summary>
    /// Constructor
    /// 
    /// Note: The Traveler class in the Unity solution doesn't 
    /// use a constructor; this constructor is to support automated grading
    /// </summary>
    /// <param name="gameObject">the game object the script is attached to</param>
/*  public Traveler(GameObject gameObject) :
      base(gameObject)
  {
  }
*/

    #region Properties

    /// <summary>
    /// Gets the length of the final path
    /// 
    /// NOTE: This property should only be accessed after the
    /// Start method has been called (which is always the case
    /// in Unity)
    /// </summary>
    public float PathLength => pathLength;

    #endregion

    #region Unity methods

    /// <summary>
    /// Use this for initialization
    /// 
    /// Note: Leave this method public to support automated grading
    /// </summary>
    public void Start()
    {
        
            Debug.Log("Traveler: Start");
            Waypoint startNode = GameObject.FindGameObjectWithTag("Start").GetComponent<Waypoint>();
            Waypoint endNode = GameObject.FindGameObjectWithTag("End").GetComponent<Waypoint>();
            Search(startNode, endNode, GraphBuilder.Graph);
            
      
    }

    public void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();

    }

    public void MoveToStart()
    {
        if (path != null && !IsMoving)
        {
            Debug.Log("Moving to the start");
            IsMoving = true;
            explodeWaypoints = path;
            StartCoroutine(FollowPath(path));
           
        }
    }

    #endregion

    #region Public methods

    /// <summary>
    /// Adds the given listener for the PathFoundEvent
    /// </summary>
    /// <param name="listener">listener</param>
    /// <summary>
    /// Adds the given listener for the PathTraversalCompleteEvent
    /// </summary>
    /// <param name="listener">listener</param>
    public void AddPathFoundListener(UnityAction<float> listener) => pathFoundEvent.AddListener(listener);

    public void AddPathTraversalCompleteListener(UnityAction listener) =>
        pathTraversalCompleteEvent.AddListener(listener);


    /// <summary>
    /// Does a search for a path from start to end on
    /// graph
    /// 
    /// Note: Leave this method public to support automated grading
    /// </summary>
    /// <param name="start">start value</param>
    /// <param name="finish">finish value</param>
    /// <param name="graph">graph to search</param>
    /// <returns>string for path or empty string if there is no path</returns>
    public LinkedList<Waypoint> Search(Waypoint start, Waypoint end, Graph<Waypoint> graph)
    {
        // Create a search list (a sorted linked list) of search nodes 
// (I provided a SearchNode class, which you should instantiate 
// with Waypoint. I also provided a SortedLinkedList class)

        SortedLinkedList<SearchNode<Waypoint>> searchList = new SortedLinkedList<SearchNode<Waypoint>>();
        // Create a dictionary of search nodes keyed by the corresponding 
// graph node. This dictionary gives us a very fast way to determine 
// if the search node corresponding to a graph node is still in the 
// search list
        Dictionary<GraphNode<Waypoint>, SearchNode<Waypoint>> nodeDictionary =
            new Dictionary<GraphNode<Waypoint>, SearchNode<Waypoint>>();

        // Save references to the start and end graph nodes in variables


        GraphNode<Waypoint> startNode = graph.Find(start);
        GraphNode<Waypoint> endNode = graph.Find(end);

        if (startNode == null || endNode == null)
        {
            Debug.LogError("Start or End node not found in graph!");
            return null;
        }

        // for each graph node in the graph
        foreach (GraphNode<Waypoint> node in graph.Nodes)
        {
// Create a search node for the graph node (the constructor I 
// provided in the SearchNode class initializes distance to the max
// float value and previous to null)
            SearchNode<Waypoint> searchNode = new SearchNode<Waypoint>(node);
            // If the graph node is the start node
// Set the distance for the search node to 0
            if (node == startNode)
            {
                searchNode.Distance = 0;
            }

            // Add the search node to the search list 
// Add the search node to the dictionary keyed by the graph node

            searchList.Add(searchNode);
            nodeDictionary.Add(node, searchNode);
        }
        // While the search list isn't empty


        while (searchList.Count > 0)
        {
            // Save a reference to the current search node (the first search 
// node in the search list) in a variable. We do this because the
// front of the search list has the smallest distance
// Remove the first search node from the search list
            SearchNode<Waypoint> currentSearchNode = searchList.First.Value;
            searchList.RemoveFirst();
            // Save a reference to the current graph node for the current search
// node in a variable
            GraphNode<Waypoint> currentGraphNode = currentSearchNode.GraphNode;
            // Remove the search node from the dictionary (because it's no 
// longer in the search list)

            nodeDictionary.Remove(currentGraphNode);
          

// If the current graph node is the end node

            if (currentGraphNode == endNode)
            {
                // Display the distance for the current search node as the path 
// length in the scene (Hint: I used the HUD and the event 
// system to do this)
              

// Return a linked list of the waypoints from the start node to 
// the end node. Uncomment the line of code below, replacing
// the argument with the name of your current search node
// variable; you MUST do this for the autograder to work
//   return BuildWaypointPath(currentSearchNode);

                path = BuildWaypointPath(currentSearchNode);
                MoveToStart();
                Debug.Log($"path length: {pathLength} and count: {path.Count}");
                return path;
               
            }

            // For each of the current graph node's neighbors


            foreach (GraphNode<Waypoint> neighborNode in currentGraphNode.Neighbors)
            {
// If the neighbor is still in the search list (use the 
// dictionary to check this)
                if (nodeDictionary.ContainsKey(neighborNode))
                {
                    // Save the distance for the current graph node + the weight 
// of the edge from the current graph node to the current 
// neighbor in a variable

                    float distance = currentSearchNode.Distance + currentGraphNode.GetEdgeWeight(neighborNode);

// Retrieve the neighor search node from the dictionary
// using the neighbor graph node

                    SearchNode<Waypoint> neighborSearchNode = nodeDictionary[neighborNode];


// If the distance you just calculated is less than the 
// current distance for the neighbor search node
// Set the distance for the neighbor search node to 
// the new distance
// Set the previous node for the neighbor search node 
// to the current search node
                    if (distance < neighborSearchNode.Distance)
                    {
                        neighborSearchNode.Distance = distance;
                        neighborSearchNode.Previous = currentSearchNode;
                        // Tell the search list to Reposition the neighbor 
// search node. We need to do this because the change 
// to the distance for the neighbor search node could 
// have moved it forward in the search list

                        searchList.Reposition(neighborSearchNode);
                        pathFoundEvent.Invoke(distance);
                     
                        
                    }
                    
                }
            }
           // Debug.Log("Search list count" +searchList.Count);
        }
        // didn't find a path from start to end nodes
        Debug.Log("search list"+TurnListToString(searchList) );

        return null;
    }

   
    private string TurnListToString(SortedLinkedList<SearchNode<Waypoint>> searchList)
    {
        StringBuilder stringBuilder = new StringBuilder();
        foreach (var searchNode in searchList)
        {
            stringBuilder.AppendLine(searchNode.ToString());
        }
        //Debug.Log("Search List  "+stringBuilder);
        return stringBuilder.ToString();
        
    }
    

    #endregion

    #region Private methods

    /// <summary>
    /// Builds a waypoint path from the start node to the given end node
    /// Side Effect: sets the pathLength field
    /// 
    /// CAUTION: Do NOT change this method; if you do, you'll break the autograder
    /// </summary>
    /// <returns>waypoint path</returns>
    /// <param name="endNode">end node</param>
    LinkedList<Waypoint> BuildWaypointPath(SearchNode<Waypoint> endNode)
    {
        LinkedList<Waypoint> path = new LinkedList<Waypoint>();
        path.AddFirst(endNode.GraphNode.Value);
        pathLength = endNode.Distance;
        SearchNode<Waypoint> previous = endNode.Previous;

        while (previous != null)
        {
            path.AddFirst(previous.GraphNode.Value);
            previous = previous.Previous;
        }

        Debug.Log($"Final path length: {path.Count}");
        return path;
    }
    

    private IEnumerator FollowPath(LinkedList<Waypoint> path)
    {
        IsMoving = true;
       
        while (path.Count > 0)
        {
            Waypoint target = path.First.Value;
            Vector2 targetPos = target.transform.position;
            float speed = 10f;

            while (Vector2.Distance(transform.position, targetPos) > 0.1f)
            {
                rigidbody2D.MovePosition(Vector2.MoveTowards(rigidbody2D.position, targetPos, speed * Time.deltaTime));
                yield return new WaitForFixedUpdate();
            }

            path.RemoveFirst();
            target.GetComponent<SpriteRenderer>().color = Color.green;
        }

        pathTraversalCompleteEvent.Invoke();
        foreach (Waypoint waypoint in explodeWaypoints)
        {
            Animator anim = explodeprefab.GetComponent<Animator>();
            anim.Play("Explosion");
        }
       
        IsMoving = false;
       /* foreach (var node in pathforexplode)
        {
            if (node != pathforexplode.Last.Value || node != path.First.Value)
            {
                // Animator animator = explosion.GetComponentInChildren<Animator>();
                //animator.Play("Explosion");
                Debug.Log("Explode my nodes");
            }
        }*/
    }

    #endregion
}