using System;
using System.Collections.Generic;
using System.Linq;
using OBB;
using UnityEngine;

public class OrientedBB3D : MonoBehaviour
{
    [SerializeField] private MeshFilter _mesh1;
    [SerializeField] private MeshFilter _mesh2;

    [SerializeField] private Cube _cube;


    private void Awake()
    {
        _cube = new Cube(Vector3.one);
    }

    private void Update()
    {
        var mtv = Vector3.zero;

        if (TestCollision(ref mtv)){}
            _mesh1.transform.position += new Vector3(mtv.x, mtv.y, mtv.z);

            _cube.Position = _mesh1.transform.position;
            _cube.Rotation = _mesh1.transform.rotation;
    }

    private bool TestCollision(ref Vector3 mtv)
    {
        var vertices1 = GetMeshVertices3D(_mesh1.mesh, _mesh1.transform);
        var vertices2 = GetMeshVertices3D(_mesh2.mesh, _mesh2.transform);

        var axes = GetPerpendicularAxes(vertices1, vertices2);
        var minOverlap = float.MaxValue;

        foreach (var axis in axes)
        {
            if (axis == Vector3.zero)
                continue;
            
            var projection1 = GetVerticesProjectionOnAxis(vertices1, axis);
            var projection2 = GetVerticesProjectionOnAxis(vertices2, axis);

            var overlappingLength = GetOverlapLength(projection1, projection2);

            if (overlappingLength == 0)
            {
                mtv = Vector3.zero;
                return false;
            }
            
            if (overlappingLength < minOverlap)
            {
                minOverlap = overlappingLength;
                mtv = axis * minOverlap;
            }
        }

        Debug.Log($"mtv: {mtv}");
        
        var checkDir = Vector3.Dot(_mesh1.transform.position - _mesh2.transform.position, mtv) < 0;

        if (checkDir)
            mtv = -mtv;
        
        return true;
    }
    
    private Vector2 GetVerticesProjectionOnAxis(List<Vector3> vertices, Vector3 axis)
    {
        float min = Mathf.Infinity;
        float max = Mathf.NegativeInfinity;

        foreach (var vertex in vertices)
        {
            var projection = Vector3.Dot(vertex, axis);
            
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
        
        // if (v1.x < v2.x)
        // {
        //     if (v1.y < v2.x)
        //     {
        //         return 0f;
        //     }
        //
        //     return v1.y - v2.x;
        // }
        //
        // if (v2.y < v1.x)
        // {
        //     return 0f;
        // }
        //
        // return v2.y - v1.x;
    }

    private bool IsOverlapping(Vector2 v1, Vector2 v2)
    {
        return v1.x <= v2.y && v1.y >= v2.x;
    }

    // private Vector2 GetNormal3D(Vector3 edge)
    // {
    //     return Vector3.Cross()
    // }

    //WTF?
    private List<Vector3> GetPerpendicularAxes(List<Vector3> vertices1, List<Vector3> vertices2)
    {
        // for (var i = 0; i < vertices1.Count; i++)
        // {
        //     var vertex1 = vertices1[i];
        //     var vertex2 = vertices1[i + 1];
        // }

        // var axes = new List<Vector3>()
        // {
        //     GetPerpendicularAxis(vertices1, 0),
        //     GetPerpendicularAxis(vertices1, 1),
        //     GetPerpendicularAxis(vertices1, 2),
        //     GetPerpendicularAxis(vertices2, 0),
        //     GetPerpendicularAxis(vertices2, 1),
        //     GetPerpendicularAxis(vertices2, 2),
        // };

        
        var axes = new List<Vector3>()
        {
            _mesh1.transform.right,
            _mesh1.transform.up,
            _mesh1.transform.forward,
            _mesh2.transform.right,
            _mesh2.transform.up,
            _mesh2.transform.forward,
            Vector3.Cross( _mesh1.transform.right,   _mesh2.transform.right),
            Vector3.Cross( _mesh1.transform.right,   _mesh2.transform.up),
            Vector3.Cross( _mesh1.transform.right,   _mesh2.transform.forward),
            
            Vector3.Cross( _mesh1.transform.up,   _mesh2.transform.right),
            Vector3.Cross( _mesh1.transform.up,   _mesh2.transform.up),
            Vector3.Cross( _mesh1.transform.up,   _mesh2.transform.forward),
            
            Vector3.Cross( _mesh1.transform.forward,   _mesh2.transform.right),
            Vector3.Cross( _mesh1.transform.forward,   _mesh2.transform.up),
            Vector3.Cross( _mesh1.transform.forward,   _mesh2.transform.forward),
        };

        return axes;
    }

    private Vector3 GetPerpendicularAxis(List<Vector3> vertices, int index)
    {
        var v1 = vertices[index + 1];
        var v2 = vertices[index];

        var normal = Vector3.Cross(v1, v2);

        return normal;
        //return GetNormal3D((vertices[index + 1] - vertices[index]).normalized);
    }

    private List<Vector3> GetMeshVertices3D(Mesh mesh, Transform meshTransform)
    {
       // var vertices1 = new List<Vector3>();
        // mesh.GetVertices(vertices1);
        
        //return vertices1;
        var vert = mesh.vertices;
        vert = RemoveDuplicates(vert, meshTransform);
        
        return vert.ToList();
    }

    private Vector3 GetCenterOfBB(Mesh mesh)
    {
        return mesh.bounds.center;
    }
    
    private Vector3[] RemoveDuplicates(Vector3[] dupArray, Transform transformObj) {

        for (int j = 0; j < dupArray.Length; j++) {
            dupArray[j] = transformObj.TransformPoint(dupArray[j]);
            var test = _cube.TransformPointFromLocalToWorldSpace((dupArray[j]));
            
            Debug.Log($"RemoveDuplicates unity: {dupArray[j]}, me: {test}");
        }

        Vector3[] newArray = new Vector3[8];  //change 8 to a variable dependent on shape
        bool isDup = false;
        int newArrayIndex = 0;
        for (int i = 0; i < dupArray.Length; i++) {
            for (int j = 0; j < newArray.Length; j++) {
                if (dupArray[i] == newArray[j]) {
                    isDup = true;
                }
            }
            if (!isDup) {
                newArray[newArrayIndex] = dupArray[i];
                newArrayIndex++;
                isDup = false;
            }
        }
        return newArray;
    }
}