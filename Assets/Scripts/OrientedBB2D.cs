using System;
using System.Collections.Generic;
using UnityEngine;

public class OrientedBB2D : MonoBehaviour
{
    [SerializeField] private MeshFilter _mesh1;
    [SerializeField] private MeshFilter _mesh2;


    private void Update()
    {
        var mtv = Vector2.zero;

        if (TestCollision(ref mtv))
            _mesh1.transform.position += new Vector3(mtv.x, mtv.y, 0f);
    }

    private bool TestCollision(ref Vector2 mtv)
    {
        var vertices1 = GetMeshVertices2D(_mesh1.mesh);
        var vertices2 = GetMeshVertices2D(_mesh2.mesh);

        var axes = GetPerpendicularAxes(vertices1, vertices2);

        var minOverlap = Mathf.Infinity;

        foreach (var axis in axes)
        {
            var projection1 = GetVerticesProjectionOnAxis(vertices1, axis);
            var projection2 = GetVerticesProjectionOnAxis(vertices2, axis);

            var overlappingLength = GetOverlapLength(projection1, projection2);

            if (overlappingLength == 0)
            {
                mtv = Vector2.zero;
                return false;
            }

            if (overlappingLength < minOverlap)
            {
                minOverlap = overlappingLength;
                mtv = axis * minOverlap;
            }
        }

        var checkDir = Vector2.Dot(GetCenterOfBB(_mesh1.mesh) - GetCenterOfBB(_mesh2.mesh), mtv) < 0;

        if (checkDir)
            mtv = -mtv;

        return true;
    }
    
    private Vector2 GetVerticesProjectionOnAxis(List<Vector2> vertices, Vector2 axis)
    {
        float min = Mathf.Infinity;
        float max = Mathf.NegativeInfinity;

        foreach (var vertex in vertices)
        {
            var projection = Vector2.Dot(vertex, axis);
            
            if (projection < min)
                min = projection;

            if (projection > max)
                max = projection;
        }

        return new Vector2(min, max);
    }

    //WTF
    private float GetOverlapLength(Vector2 v1, Vector2 v2)
    {
        if (!IsOverlapping(v1, v2)) return 0;

        return Mathf.Min(v1.y, v2.y) - Mathf.Max(v1.x, v2.x);
    }

    private bool IsOverlapping(Vector2 v1, Vector2 v2)
    {
        return v1.x <= v2.y && v1.y >= v2.x;
    }

    private Vector2 GetNormal2D(Vector2 edge)
    {
        return new Vector2(-edge.y, edge.x);
    }

    //WTF?
    private List<Vector2> GetPerpendicularAxes(List<Vector2> vertices1, List<Vector2> vertices2)
    {
        var axes = new List<Vector2>()
        {
            GetPerpendicularAxis(vertices1, 0),
            GetPerpendicularAxis(vertices1, 1),
            GetPerpendicularAxis(vertices2, 0),
            GetPerpendicularAxis(vertices2, 1)
        };

        return axes;
    }

    private Vector2 GetPerpendicularAxis(List<Vector2> vertices, int index)
    {
        return GetNormal2D((vertices[index + 1] - vertices[index]).normalized);
    }

    private List<Vector2> GetMeshVertices2D(Mesh mesh)
    {
        var vertices1 = new List<Vector3>();
        mesh.GetVertices(vertices1);

        var result = new List<Vector2>();

        foreach (var vertex in vertices1)
        {
            var v2d = new Vector2(vertex.x, vertex.y);
            result.Add(v2d);
        }

        return result;
    }

    private Vector2 GetCenterOfBB(Mesh mesh)
    {
        return mesh.bounds.center;
    }
}