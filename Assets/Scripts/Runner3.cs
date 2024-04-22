using System;
using UnityEngine;

public class Runner3 : MonoBehaviour
{
    [SerializeField] private SphereCollider sphere;
    [SerializeField] private MeshRenderer[] levelObjects;

    private void Update()
    {
        var spherePosition = sphere.transform.position;
        
        foreach (var levelObj in levelObjects)
        {
            var bounds1 = levelObj.bounds;
            var boxMin = bounds1.min;
            var boxMax = bounds1.max;
            
            if (PhysicsHelper.SphereVsAABB(boxMin, boxMax, levelObj.transform.rotation, spherePosition, sphere.radius, Vector3.zero, out var sphereNewPos))
            {
                // sphere.transform.position += sphereNewPos * Time.deltaTime * 10f;
                sphere.transform.position = sphereNewPos;
                
                Debug.Log($"Collides!, sphereNewPos: {sphereNewPos}");
            }
            
        }
        
        var x = Input.GetAxisRaw("Horizontal");
        var z = Input.GetAxisRaw("Vertical");
        var dir = new Vector3(x, 0f, z);

        sphere.transform.position += dir * 3f * Time.deltaTime;

    }
}