using System;
using UnityEngine;

public class Runner2 : MonoBehaviour
{
    [SerializeField] private Collider box1;
    [SerializeField] private Collider box2;
    [SerializeField] private float speed = 3f;

    private Vector3 _prevPos;
    [SerializeField] private Box b1;
    [SerializeField] private Box b2;

    [SerializeField] private Vector3 _prev;


    private void Awake()
    {
        b1 = new Box
        {
            x = box1.bounds.min.x,
            y = box1.bounds.min.z,
            w = 1f,
            h = 1f,
        };
        
        b2 = new Box()
        {
            x = box2.bounds.min.x,
            y = box2.bounds.min.z,
            w = 1f,
            h = 1f,
        };
    }

    private void Update()
    {
        var x = Input.GetAxisRaw("Horizontal");
        var z = Input.GetAxisRaw("Vertical");
        var dir = new Vector3(x, 0f, z);

        var velocity = (box1.bounds.min - _prev) / Time.deltaTime;
            
        box1.transform.position += dir * 3f * Time.deltaTime;
        b1.vx = velocity.x;
        b1.vy = velocity.z;
        
        b1.x = box1.transform.position.x;
        b1.y = box1.transform.position.z;

        var nx = 0f;
        var ny = 0f;
        var collision = PhysicsHelper.SweptAABB(b1, b2, ref nx, ref ny);
        
        Debug.Log($"SweptAABB: {collision}");

        _prev = box1.bounds.min;
    }
}