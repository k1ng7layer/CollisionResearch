using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OBB
{
    public static class OrientedBB
    {
        public static bool IsColliding(Cube a, Cube b, ref Vector3 mtv)
        {
            var vertices1 = GetMeshVertices3D(a);
            var vertices2 = GetMeshVertices3D(b);

            var axes = GetPerpendicularAxes(a, b);
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
        
            var checkDir = Vector3.Dot(a.Position - b.Position, mtv) < 0;

            if (checkDir)
                mtv = -mtv;
        
            return true;
        }
        
        private static List<Vector3> GetMeshVertices3D(Cube cube)
        {
            var vert = cube.Vertices;
            vert = RemoveDuplicates(vert, cube);
        
            return vert.ToList();
        }
        
        private static Vector3[] RemoveDuplicates(Vector3[] dupArray, Cube cube) {

            for (int j = 0; j < dupArray.Length; j++)
            { 
                dupArray[j] = cube.TransformPointFromLocalToWorldSpace(dupArray[j]);
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
        
        private static List<Vector3> GetPerpendicularAxes(Cube a, Cube b)
        {
            var axes = new List<Vector3>()
            {
                a.Right,
                a.Up,
                a.Forward,
                b.Right,
                b.Up,
                b.Forward,
                Vector3.Cross(a.Right,b.Right),
                Vector3.Cross(a.Right,b.Up),
                Vector3.Cross(a.Right,b.Forward),
                Vector3.Cross(a.Up,b.Right),
                Vector3.Cross(a.Up,b.Up),
                Vector3.Cross(a.Up,b.Forward),
                Vector3.Cross(a.Forward,b.Right),  
                Vector3.Cross(a.Forward,b.Up),     
                Vector3.Cross(a.Forward,b.Forward),
            };

            return axes;
        }
        
        private static Vector2 GetVerticesProjectionOnAxis(List<Vector3> vertices, Vector3 axis)
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
        
        private static float GetOverlapLength(Vector2 v1, Vector2 v2)
        {
            if (!IsOverlapping(v1, v2)) return 0;
        
            return Mathf.Min(v1.y, v2.y) - Mathf.Max(v1.x, v2.x);
        }
        
        private static bool IsOverlapping(Vector2 v1, Vector2 v2)
        {
            return v1.x <= v2.y && v1.y >= v2.x;
        }
    }
}