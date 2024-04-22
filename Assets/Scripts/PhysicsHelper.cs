using UnityEngine;

public static class PhysicsHelper
{
    public static bool SphereVsAABB(
        Vector3 minBox, 
        Vector3 maxBox,
        Quaternion boxRotation,
        Vector3 spherePosition, 
        float sphereRadius, 
        Vector3 velocity, 
        out Vector3 sphereNewPos)
    {
        minBox = boxRotation * minBox;
        maxBox = boxRotation * maxBox;
        // get box closest point to sphere center by clamping
        var x = Mathf.Max(minBox.x, Mathf.Min(spherePosition.x, maxBox.x));
        var y = Mathf.Max(minBox.y, Mathf.Min(spherePosition.y, maxBox.y));
        var z = Mathf.Max(minBox.z, Mathf.Min(spherePosition.z, maxBox.z));

        // this is the same as isPointInsideSphere
        var distance = Mathf.Sqrt(
            (x - spherePosition.x) * (x - spherePosition.x) +
            (y - spherePosition.y) * (y - spherePosition.y) +
            (z - spherePosition.z) * (z - spherePosition.z)
        );

        var dir = new Vector3(x, y, z) - spherePosition;

        var diff = sphereRadius - distance;
        var shiftDelta = dir.normalized * diff;
        var speed = velocity.magnitude;
        Debug.Log($"speed: {speed}");
        
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

    public static float SweptAABB(Box b1, Box b2, ref float normalX, ref float normalY)
    {
        float xInvEntry, yInvEntry; 
        float xInvExit, yInvExit; 

// find the distance between the objects on the near and far sides for both x and y 
        if (b1.vx > 0.0f) 
        { 
            xInvEntry = b2.x - (b1.x + b1.w);  
            xInvExit = (b2.x + b2.w) - b1.x;
        }
        else 
        { 
            xInvEntry = (b2.x + b2.w) - b1.x;  
            xInvExit = b2.x - (b1.x + b1.w);  
        } 

        if (b1.vy > 0.0f) 
        { 
            yInvEntry = b2.y - (b1.y + b1.h);  
            yInvExit = (b2.y + b2.h) - b1.y;  
        }
        else 
        { 
            yInvEntry = (b2.y + b2.h) - b1.y;  
            yInvExit = b2.y - (b1.y + b1.h);  
        }
        
        float xEntry, yEntry; 
        float xExit, yExit; 

        if (b1.vx == 0.0f)
        {
            xEntry = -Mathf.Infinity;
            xExit = Mathf.Infinity;
        } 
        else 
        { 
            xEntry = xInvEntry / b1.vx; 
            xExit = xInvExit / b1.vx; 
        } 

        if (b1.vy == 0.0f)
        {
            yEntry = -Mathf.Infinity;
            yExit = Mathf.Infinity;
        } 
        else 
        { 
            yEntry = yInvEntry / b1.vy; 
            yExit = yInvExit / b1.vy; 
        }
        
        var entryTime = Mathf.Max(xEntry, yEntry); 
        float exitTime = Mathf.Min(xExit, yExit);
        
        if (entryTime > exitTime || xEntry < 0.0f && yEntry < 0.0f || xEntry > 1.0f || yEntry > 1.0f) 
        { 
            normalX = 0.0f; 
            normalY = 0.0f; 
            return 1.0f; 
        }

        // if there was a collision 
        // calculate normal of collided surface
        if (xEntry > yEntry) 
        { 
            if (xInvEntry < 0.0f) 
            { 
                normalX = 1.0f; 
                normalY = 0.0f; 
            } 
            else 
            { 
                normalX = -1.0f; 
                normalY = 0.0f;
            } 
        } 
        else 
        { 
            if (yInvEntry < 0.0f) 
            { 
                normalX = 0.0f; 
                normalY = 1.0f; 
            } 
            else 
            { 
                normalX = 0.0f; 
                normalY = -1.0f; 
            } 
        } // return the time of collisionreturn entryTime; 

        return entryTime;
    }
}