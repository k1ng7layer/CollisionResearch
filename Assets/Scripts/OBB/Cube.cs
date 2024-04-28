using System;
using UnityEngine;

namespace OBB
{
    [Serializable]
    public class Cube
    {
        public Vector3 Position;
        public Quaternion Rotation;
        
        private readonly Vector3 Size;

        public Cube(Vector3 size)
        {
            Size = size;
        }

        public Vector3[] Vertices
        {
            get
            {
                var max = Size / 2f;
                var min = -max;

                return new[]
                {
                    Position + Rotation * min,
                    Position + Rotation * new Vector3(max.x, min.y, min.z),
                    Position + Rotation * new Vector3(min.x, max.y, min.z),
                    Position + Rotation * new Vector3(max.x, max.y, min.z),
                    Position + Rotation * new Vector3(min.x, min.y, max.z),
                    Position + Rotation * new Vector3(max.x, min.y, max.z),
                    Position + Rotation * new Vector3(min.x, max.y, max.z),
                    Position + Rotation * max,
                };
            }
        }

        public Vector3 TransformPointFromLocalToWorldSpace(Vector3 point)
        {
            var pointLocalMatrix = new Matrix4x4(
                new Vector4(point.x, 0, 0),
                new Vector4(point.y, 0, 0),
                new Vector4(point.z, 0, 0),
                new Vector4(0, 0, 0)
            );

            var translationMatrix = new Matrix4x4(
                new Vector4(1f, 0f, 0, point.x), 
                new Vector4(0f, 1f, 0f, point.y),
                new Vector4(0f, 0f, 1f, point.z), 
                new Vector4(0f, 0f, 0, 1f));


            var result = translationMatrix.MultiplyVector(point);
            
            return result;
        }

        private float[,] GetTranslationMatrix(Vector3 point)
        {
            return new float[,]
            {
                {1f, 0f, 0, point.x},
                {0f, 1f, 1f, point.y},
                {0f, 0f, 1f, point.z},
                {0f, 0f, 0, 1f},
            };
        }
        
    }
}