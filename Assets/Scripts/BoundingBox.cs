using System;
using UnityEngine;

public struct BoundingBox
{
    public float Left;
    public float Right;
    public float Bottom;
    public float Top;
    public float Front;
    public float Back;
    public Vector3 Center;
}

[Serializable]
public class Box
{
    // position of top-left corner 
    public float x, y; 

    // dimensions 
    public float w, h; 

    // velocity 
    public float vx, vy;
}