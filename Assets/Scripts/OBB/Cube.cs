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
                    // Vector3.zero + Rotation * min,
                    // Vector3.zero + Rotation * new Vector3(max.x, min.y, min.z),
                    // Vector3.zero + Rotation * new Vector3(min.x, max.y, min.z),
                    // Vector3.zero + Rotation * new Vector3(max.x, max.y, min.z),
                    // Vector3.zero + Rotation * new Vector3(min.x, min.y, max.z),
                    // Vector3.zero + Rotation * new Vector3(max.x, min.y, max.z),
                    // Vector3.zero + Rotation * new Vector3(min.x, max.y, max.z),
                    // Vector3.zero + Rotation * max,
                    
                    Vector3.zero + new Vector3(-Size.x, -Size.y, -Size.z) * 0.5f,
                    Vector3.zero + new Vector3(Size.x, -Size.y, -Size.z) * 0.5f,
                    Vector3.zero + new Vector3(Size.x, -Size.y, Size.z) * 0.5f,
                    Vector3.zero + new Vector3(-Size.x, -Size.y, Size.z) * 0.5f,
                    Vector3.zero + new Vector3(-Size.x, Size.y, -Size.z) * 0.5f,
                    Vector3.zero + new Vector3(Size.x, Size.y, -Size.z) * 0.5f,
                    Vector3.zero + new Vector3(Size.x, Size.y, Size.z) * 0.5f,
                    Vector3.zero + new Vector3(-Size.x, Size.y, Size.z) * 0.5f,
                };
            }
        }

        public Vector3 TransformPointFromLocalToWorldSpace(Vector3 point)
        {
            var rotEuler = Rotation.eulerAngles * Mathf.Deg2Rad;
            
            var translationMatrix = new Matrix4x4(
                new Vector4(1f, 0f, 0f, 0f), 
                new Vector4(0f, 1f, 0f, 0f),
                new Vector4(0f, 0f, 1f, 0f), 
                new Vector4(Position.x, Position.y, Position.z, 1f));
            
            var scaleMatrix = new Matrix4x4(
                new Vector4(1f, 0f, 0f, 0f), 
                new Vector4(0f, 1f, 0f, 0f),
                new Vector4(0f, 0f, 1f, 0f), 
                new Vector4(0f, 0f, 0f, 1f));
            
            var rotationMatrixX = new Matrix4x4(
                new Vector4(1f, 0f, 0f, 0f), 
                new Vector4(0f, Mathf.Cos(rotEuler.x), Mathf.Sin(rotEuler.x), 0f),
                new Vector4(0f, -Mathf.Sin(rotEuler.x), Mathf.Cos(rotEuler.x), 0f), 
                new Vector4(0f, 0f, 0f, 1f));
            
            var rotationMatrixY = new Matrix4x4(
                new Vector4(Mathf.Cos(rotEuler.y), 0f, -Mathf.Sin(rotEuler.y), 0f), 
                new Vector4(0f, 1f, 0f, 0f),
                new Vector4(Mathf.Sin(rotEuler.y), 0f, Mathf.Cos(rotEuler.y), 0f), 
                new Vector4(0f, 0f, 0f, 1f));
            
            var rotationMatrixZ = new Matrix4x4(
                new Vector4(Mathf.Cos(rotEuler.z), Mathf.Sin(rotEuler.z), 0f, 0f), 
                new Vector4(-Mathf.Sin(rotEuler.z), Mathf.Cos(rotEuler.z), 0f, 0f),
                new Vector4(0f, 0f, 1f, 0f), 
                new Vector4(0f, 0f, 0f, 1f));


            var transformation = translationMatrix * rotationMatrixY * rotationMatrixX * rotationMatrixZ * scaleMatrix; 
       
            var transformationArr = new float[4, 4];

            for (int i = 0; i < 4; i++)
            {
                var a = transformation.GetRow(i);

                transformationArr[i, 0] = a.x;
                transformationArr[i, 1] = a.y;
                transformationArr[i, 2] = a.z;
                transformationArr[i, 3] = a.w;
                //Debug.Log($"localToWorldMatrix: {a}");
            }


            var rows = transformationArr.GetUpperBound(0) + 1;
            var columns = transformationArr.Length / rows;
          
            // for (int i = 0; i < rows; i++)
            // {
            //     for (int j = 0; j < columns; j++)
            //     {
            //         Debug.Log($"localToWorldMatrix array {transformationArr[i, j]}");
            //     }
            // }
            
            
            //var result = MultiplyMatrixByVector(translationMatrix, new[] { point[0], point[1], point[2], 1 });
             var result = transformation.MultiplyPoint(point);

             // var result = MultiplyMatrix(transformationArr, new[] { point[0], point[1], point[2], 1 });
             // var result = MultiplyPoint(transformationArr, point);
            return result;
        }

        private float[,] GetTranslationMatrix()
        {
            return new float[,]
            {
                {1f, 0f, 0f, Position.x},
                {0f, 1f, 0f, Position.y},
                {0f, 0f, 1f, Position.z},
                {0f, 0f, 0f, 1f},
            };
        }


        private Vector4 MultiplyMatrixByVector(float[,] matrix4x4, float[] vector)
        {
            var rows = matrix4x4.GetUpperBound(0) + 1;
            var columns = matrix4x4.Length / rows;
            float[] result = new float[4];
            for (int i = 0; i < rows; i++)
            {
                result[i] = 0;
                
                for (int j = 0; j < columns; j++)
                {
                    result[i] += matrix4x4[i, j] * vector[j];
                }
            }

            return new Vector4(result[0], result[1], result[2], result[3]);
        }
        
        public float[,] MultiplyMatrix(float[,] A, float[,] B)
        {
            int rA = A.GetLength(0);
            int cA = A.GetLength(1);
            int rB = B.GetLength(0);
            int cB = B.GetLength(1);

            if (cA != rB)
            {
                Console.WriteLine("Matrixes can't be multiplied!!");
            }
            else
            {
                float temp = 0;
                float[,] kHasil = new float[rA, cB];

                for (int i = 0; i < rA; i++)
                {
                    for (int j = 0; j < cB; j++)
                    {
                        temp = 0;
                        for (int k = 0; k < cA; k++)
                        {
                            temp += A[i, k] * B[k, j];
                        }
                        kHasil[i, j] = temp;
                    }
                }

                return kHasil;
            }

            return default;
        }
        
        public Vector3 MultiplyPoint(float[, ] matrix, Vector3 point)
        {
            Vector3 res;
            float w;
            res.x =  matrix[0, 0] * point.x +  matrix[1, 0] * point.y +  matrix[2, 0] * point.z + matrix[3, 0];
            res.y = matrix[0, 1] * point.x +  matrix[1, 1] * point.y +  matrix[2, 1] * point.z +  matrix[3, 1];
            res.z =  matrix[0, 2] * point.x +  matrix[1, 2] * point.y +  matrix[2, 2] * point.z +  matrix[3, 2];
            w =  matrix[0, 3] * point.x +  matrix[1, 3] * point.y +  matrix[2, 3] * point.z +  matrix[3, 3];

            w = 1f / w;
            res.x *= w;
            res.y *= w;
            res.z *= w;
            return res;
        }
        
    }
}