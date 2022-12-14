using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Pathfinding : MonoBehaviour
{

	public Transform seeker, target;

	Map grid;
	PathProvider requestManager;


	void Awake()
	{
		grid = GetComponent<Map>();
		requestManager = GetComponent<PathProvider>();
	}

	public void StartFindPath(Vector3 startPos, Vector3 targetPos)
	{
		StartCoroutine(FindPath(startPos, targetPos));
	}

	IEnumerator FindPath(Vector3 startPos, Vector3 targetPos)
	{
		Vector3[] waypoints = new Vector3[0];
		bool pathSuccess = false;

		Node startNode = grid.nodeFromPos(startPos);
		Node targetNode = grid.nodeFromPos(targetPos);

		Heap<Node> openSet = new Heap<Node>(grid.w*grid.h);
		HashSet<Node> closedSet = new HashSet<Node>();
		openSet.Add(startNode);

		while (openSet.Count > 0)
		{
			Node currentNode = openSet.RemoveFirst();
			closedSet.Add(currentNode);

			if (currentNode == targetNode)
			{
				pathSuccess = true;
				break;
			}

			foreach (Node neighbour in grid.getNeighbours(currentNode))
			{
				if (!neighbour.valid || closedSet.Contains(neighbour))
				{
					continue;
				}

				int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
				if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
				{
					neighbour.gCost = newMovementCostToNeighbour;
					neighbour.hCost = GetDistance(neighbour, targetNode);
					neighbour.parent = currentNode;

					if (!openSet.Contains(neighbour))
						openSet.Add(neighbour);
					else
					{
						//openSet.UpdateItem(neighbour);
					}
				}
			}
		}
		yield return null;
		if (pathSuccess)
        {
			waypoints = RetracePath(startNode, targetNode);
		}
		if (waypoints.Length == 0) pathSuccess = false;
		requestManager.FinishedProcessingPath(waypoints, pathSuccess);
	}

	Vector3[] RetracePath(Node startNode, Node endNode)
	{
		List<Node> path = new List<Node>();
		Node currentNode = endNode;

		while (currentNode != startNode)
		{
			path.Add(currentNode);
			currentNode = currentNode.parent;
		}
		Vector3[] waypoints = SimplifyPath(path);
		Array.Reverse(waypoints);
		path.Reverse();

		grid.path = path;
		return waypoints;
	}

	Vector3[] SimplifyPath(List<Node> path)
	{
		List<Vector3> waypoints = new List<Vector3>();
		if(path.Count==1)
        {
			waypoints.Add(path[0].pos);
			return waypoints.ToArray();
        }
        else
        {
			if (path.Count == 0) return waypoints.ToArray(); 

		}
		Vector2 directionOld = Vector2.zero;

		for (int i = 1; i < path.Count; i++)
		{
			Vector2 directionNew = new Vector2(path[i - 1].gridX - path[i].gridX, path[i - 1].gridY - path[i].gridY);
			if (directionNew.x != directionOld.x || directionNew.y != directionOld.y || true)
			{
				waypoints.Add(path[i-1].pos);
			}
			directionOld = directionNew;
		}
		waypoints.Add(path[path.Count-1].pos);
		return waypoints.ToArray();
	}

	int GetDistance(Node nodeA, Node nodeB)
	{
		int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
		int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

		if (dstX > dstY)
			return 14 * dstY + 10 * (dstX - dstY);
		return 14 * dstX + 10 * (dstY - dstX);
	}


}