using UnityEngine;

namespace OBB
{
    public static class MatrixHelper
    {
        public static void PrintMatrix(Matrix4x4 matrix4X4, string message)
        {
            for (int i = 0; i < 4; i++)
            {
                var a = matrix4X4.GetRow(i);
                
                Debug.Log($"Matrix {message}: {a}");
            }
        }
    }
}