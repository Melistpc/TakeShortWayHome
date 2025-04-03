using System;
using System.Collections;
using System.Collections.Generic;
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
    private Rigidbody2D rigidbody2D;

    #endregion

    #region Properties

    public float PathLength => pathLength;

    #endregion

    #region Unity methods

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
            StartCoroutine(FollowPath(path));
        }
    }

    #endregion

    #region Public methods

    public void AddPathFoundListener(UnityAction<float> listener) => pathFoundEvent.AddListener(listener);
    public void AddPathTraversalCompleteListener(UnityAction listener) => pathTraversalCompleteEvent.AddListener(listener);

    public LinkedList<Waypoint> Search(Waypoint start, Waypoint end, Graph<Waypoint> graph)
    {
        SortedLinkedList<SearchNode<Waypoint>> searchList = new SortedLinkedList<SearchNode<Waypoint>>();
        Dictionary<GraphNode<Waypoint>, SearchNode<Waypoint>> nodeDictionary = new Dictionary<GraphNode<Waypoint>, SearchNode<Waypoint>>();

        GraphNode<Waypoint> startNode = graph.Find(start);
        GraphNode<Waypoint> endNode = graph.Find(end);

        if (startNode == null || endNode == null)
        {
            Debug.LogError("Start or End node not found in graph!");
            return null;
        }

        foreach (GraphNode<Waypoint> node in graph.Nodes)
        {
            SearchNode<Waypoint> searchNode = new SearchNode<Waypoint>(node);
            if (node == startNode)
            {
                searchNode.Distance = 0;
            }
            searchList.Add(searchNode);
            nodeDictionary.Add(node, searchNode);
        }

        while (searchList.Count > 0)
        {
            SearchNode<Waypoint> currentSearchNode = searchList.First.Value;
            searchList.RemoveFirst();
            GraphNode<Waypoint> currentGraphNode = currentSearchNode.GraphNode;
            nodeDictionary.Remove(currentGraphNode);

            if (currentGraphNode == endNode)
            {
                Debug.Log($"Path found! Distance: {currentSearchNode.Distance}");
                path = BuildWaypointPath(currentSearchNode);
                MoveToStart();
                return path;
            }

            foreach (GraphNode<Waypoint> neighborNode in currentGraphNode.Neighbors)
            {
                if (nodeDictionary.ContainsKey(neighborNode))
                {
                    float distance = currentSearchNode.Distance + currentGraphNode.GetEdgeWeight(neighborNode);
                    SearchNode<Waypoint> neighborSearchNode = nodeDictionary[neighborNode];

                    if (distance < neighborSearchNode.Distance)
                    {
                        neighborSearchNode.Distance = distance;
                        neighborSearchNode.Previous = currentSearchNode;
                        searchList.Reposition(neighborSearchNode);

                        Debug.Log($"Updated {neighborSearchNode.GraphNode.Value}, Previous: {currentSearchNode.GraphNode.Value}");
                        pathFoundEvent.Invoke(distance);
                    }
                }
            }
        }

        Debug.LogWarning("No path found!");
        return null;
    }

    #endregion

    #region Private methods

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
            float speed = 40f;

            while (Vector2.Distance(transform.position, targetPos) > 0.1f)
            {
                rigidbody2D.MovePosition(Vector2.MoveTowards(rigidbody2D.position, targetPos, speed * Time.deltaTime));
                yield return new WaitForFixedUpdate();
            }

            path.RemoveFirst();
            target.GetComponent<SpriteRenderer>().color = Color.green;
        }

        pathTraversalCompleteEvent.Invoke();
        IsMoving = false;
    }

    #endregion
}
