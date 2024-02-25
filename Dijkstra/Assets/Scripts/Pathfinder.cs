using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Pathfinder : Kinematic
{
    public Node target;
    public UnityEvent<Path> OnCreatePath = new UnityEvent<Path>();
    Graph graph;

    FollowPath myMoveType;
    LookWhereGoing myRotateType;

    void Awake()
    {
        Node start = FindClosestNode();
        graph = new Graph();
        graph.Build(this.GetComponent<MeshRenderer>());
        Connection[] pathfinding = Dijkstra.Pathfind(graph, start, target);

        Transform[] pathPoints = new Transform[pathfinding.Length + 1];
        int i = 0;
        foreach (var pathPoint in pathfinding)
        {
            pathPoints[i] = pathPoint.GetFromNode().transform;
            i++;
        }
        pathPoints[i] = target.transform;
        Path path = new Path(pathPoints);
        OnCreatePath.Invoke(path);

        myMoveType = new FollowPath();
        myMoveType.character = this;
        myMoveType.path = path;
        myMoveType.threshold = 0.5f;
        myMoveType.reverse = true;

        myRotateType = new LookWhereGoing();
        myRotateType.character = this;
        myRotateType.target = myTarget;
    }

    private Node FindClosestNode()
    {
        Node[] nodes = FindObjectsByType<Node>(FindObjectsSortMode.None);
        Node closest = null;
        float closestDistance = float.MaxValue;
        foreach (Node node in nodes) {
            float distance = Vector3.Distance(transform.position, node.transform.position);
            if (distance < closestDistance && node.GetComponent<MeshRenderer>().material.name == this.GetComponent<MeshRenderer>().material.name)
            {
                closest = node;
                closestDistance = distance;
            }
        }
        return closest;
    }

    protected override void Update()
    {
        steeringUpdate = new SteeringOutput();
        if (myRotateType == null) Debug.LogError("Rotation is null");
        if (myMoveType == null) Debug.LogError("Move is null");
        steeringUpdate.angular = myRotateType.getSteering().angular;
        steeringUpdate.linear = myMoveType.getSteering().linear;
        base.Update();
    }
}
