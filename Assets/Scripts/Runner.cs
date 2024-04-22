using System;
using UnityEngine;

public class Runner : MonoBehaviour
{
    [SerializeField] private MeshRenderer box1;
    [SerializeField] private MeshRenderer box2;
    [SerializeField] private SphereCollider sphere;
    [SerializeField] private float speed = 3f;

    [SerializeField] private MeshRenderer[] levelObjects;

    private Vector3 _prevPos;
    
    
    private void Update()
    {
        var bounds1 = box1.bounds;
        var boxMin = bounds1.min;
        var boxMax = bounds1.max;
        
        var spherePosition = sphere.transform.position;
        var spherePos = spherePosition;
        var sphereVelocity = (spherePosition - _prevPos) / Time.deltaTime;

        var bb1 = new BoundingBox();
        bb1.Left = boxMin.x;
        bb1.Right = boxMax.x;
        bb1.Bottom = boxMin.y;
        bb1.Top = boxMax.y;
        bb1.Front = boxMin.z;
        bb1.Back = boxMax.z; 
        
        
        var bounds2 = box2.bounds;
        var boxMin2 = bounds2.min;
        var boxMax2 = bounds2.max;
        
        var bb2 = new BoundingBox();
        bb2.Left = boxMin2.x;
        bb2.Right = boxMax2.x;
        bb2.Bottom = boxMin2.y;
        bb2.Top = boxMax2.y;
        bb2.Front = boxMin2.z;
        bb2.Back = boxMax2.z;

        // if (PhysicsHelper.IntersectionAABB(ref bb1, ref bb2))
        // {
        //     box2.transform.position = bb2.Center;
        // }
       
        // if (PhysicsHelper.SphereVsAABB(boxMin, boxMax, spherePos, sphere.radius, sphereVelocity, out var sphereNewPos))
        // {
        //     // sphere.transform.position += sphereNewPos * Time.deltaTime * 10f;
        //     sphere.transform.position = sphereNewPos;
        //         
        //     Debug.Log($"Collides!, sphereNewPos: {sphereNewPos}");
        // }
        

   

        // foreach (var levelObject in levelObjects)
        // {
        //     var bounds = levelObject.bounds;
        //     var boxMin = bounds.min;
        //     var boxMax = bounds.max;
        //     
        //     if (PhysicsHelper.SphereVsAABB(boxMin, boxMax, spherePos, sphere.radius, sphereVelocity, out var sphereNewPos))
        //     {
        //         // sphere.transform.position += sphereNewPos * Time.deltaTime * 10f;
        //         sphere.transform.position = sphereNewPos;
        //         
        //         Debug.Log($"Collides!, sphereNewPos: {sphereNewPos}");
        //     }
        // }
        
        var x = Input.GetAxisRaw("Horizontal");
        var z = Input.GetAxisRaw("Vertical");

        var dir = new Vector3(x, 0f, z);

        sphere.transform.position += dir * Time.deltaTime * speed;
        
        _prevPos =  sphere.transform.position;
    }
}