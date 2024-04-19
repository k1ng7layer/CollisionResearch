using UnityEngine;

public static class PhysicsHelper
{
    public static bool SphereVsAABB(
        Vector3 minBox, 
        Vector3 maxBox, 
        Vector3 spherePosition, 
        float sphereRadius, 
        Vector3 velocity, 
        out Vector3 sphereNewPos)
    {
        // get box closest point to sphere center by clamping
        var x = Mathf.Max(minBox.x, Mathf.Min(spherePosition.x, maxBox.x));
        var y = Mathf.Max(minBox.y, Mathf.Min(spherePosition.y, maxBox.y));
        var z = Mathf.Max(minBox.z, Mathf.Min(spherePosition.z, maxBox.x));

        // this is the same as isPointInsideSphere
        var distance = Mathf.Sqrt(
            (x - spherePosition.x) * (x - spherePosition.x) +
            (y - spherePosition.y) * (y - spherePosition.y) +
            (z - spherePosition.z) * (z - spherePosition.z)
        );

        var dir = new Vector3(x, y, z) - spherePosition;

        var diff = sphereRadius - distance;
        var shiftDelta = dir.normalized * diff;
        //var shiftDelta = dir;
        var speed = velocity.magnitude;
        Debug.Log($"speed: {speed}");
        // if (shiftDelta.magnitude > speed)
        // {
        //     shiftDelta = shiftDelta.normalized;
        //     shiftDelta *= speed;
        // }
        
        spherePosition -= shiftDelta;

        sphereNewPos = spherePosition;

        return distance < sphereRadius;
    }
    
    public static bool IntersectionAABB(ref BoundingBox a, ref BoundingBox b)
    {
        float lr = a.Left - b.Right;
        float rl = b.Left - a.Right;
        float bt = a.Bottom - b.Top;
        float tb = b.Bottom - a.Top;
        float fb = a.Front - b.Back;
        float bf = b.Front - a.Back;

        if (lr > 0 || rl > 0 || bt > 0 || tb > 0 || bf > 0 || fb > 0)
            return false;

        float max = Mathf.Max(lr, Mathf.Max(rl, Mathf.Max(bt, Mathf.Max(tb, Mathf.Max(bf, fb)))));

        // if (_complex)
        // {
        //     if (ComplexIntersection(ref box))
        //         return;
        // }

        if (max == lr)
            b.Center.x += max;
        else if (max == rl)
            b.Center.x -= max;
        else if (max == bt)
            b.Center.y += max;
        else if (max == tb)
            b.Center.y -= max;
        else if (max == fb)
            b.Center.z += max;
        else if (max == bf)
            b.Center.z -= max;

        return true;
    }
}