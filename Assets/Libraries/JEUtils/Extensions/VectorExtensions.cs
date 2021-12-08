using System;
using System.Runtime.Remoting.Messaging;
using UnityEngine;

public static class VectorExtensions
{

    public static Vector3 ToVector3(this Vector2 self)
    {
        return self;
    }

    /// <summary>
    /// Return a new vector with the argument as the new x and the y and z components unchanged
    /// </summary>
    public static Vector3 X(this Vector3 self, float newX)
    {
        return new Vector3(newX, self.y, self.z);
    }

    public static Vector2 X(this Vector2 self, float newX)
    {
        return new Vector2(newX, self.y);
    }

    /// <summary>
    /// Return a new vector with the argument as the new y and the x and z components unchanged
    /// </summary>
    public static Vector3 Y(this Vector3 self, float newY)
    {
        return new Vector3(self.x, newY, self.z);
    }

    public static Vector2 Y(this Vector2 self, float newY)
    {
        return new Vector2(self.x, newY);
    }

    /// <summary>
    /// Return a new vector with the argument as the new z and the x and y components unchanged
    /// </summary>
    public static Vector3 Z(this Vector3 self, float newZ)
    {
        return new Vector3(self.x, self.y, newZ);
    }

    /// <summary>
    /// Creates a new Discard the z component
    /// </summary>
    public static Vector2 ToVector2(this Vector3 self)
    {
        return self.Vxy();
    }

    /// <summary>
    /// Extract just the x and y components of the vector
    /// </summary>
    public static Vector2 Vxy(this Vector3 self)
    {
        return new Vector2(self.x, self.y);
    }

    /// <summary>
    /// Extract just the y component of the vector
    /// </summary>
    public static Vector2 Vy(this Vector3 self)
    {
        return new Vector2(0, self.y);
    }
    
    // todo: add other swizzle convenience functions
    /// <summary>
    ///     Rearrange the components of a vector
    ///     <code>
    ///         var newVector3 = myVector3.Swizzle((x, y, z, v) => v(z, x, y));
    ///     </code>
    /// </summary>
    public static Vector3 Swizzle(this Vector3 self, Func<float, float, float, Func<float, float, float, Vector3>, Vector3> fn)
    {
        return fn(self.x, self.y, self.z, (x, y, z) => new Vector3(x, y, z));
    }

    /// <summary>
    ///     Rearrange the components of a vector without specifying a z value
    ///     <code>                                            
    ///          var newVector3 = myVector3.Swizzle((x, y, z, v) => v(y, z))
    ///     </code>
    /// </summary>
    public static Vector3 Swizzle(this Vector3 self, Func<float, float, float, Func<float, float, Vector2>, Vector2> fn)
    {
        return fn(self.x, self.y, self.z, (x, y) => new Vector2(x, y));
    }

    public static Vector3 ViewToWorld(this Vector3 self)
    {
        return self.ViewToWorld(Camera.main);
    }

    public static Vector3 ViewToWorld(this Vector3 self, Camera camera)
    {
        return camera.ViewportToWorldPoint(self);
    }

    public static Vector3 ViewToWorld(this Vector2 self)
    {
        return self.ViewToWorld(Camera.main);
    }

    public static Vector3 ViewToWorld(this Vector2 self, Camera camera)
    {
        return camera.ViewportToWorldPoint(self);
    }


    public static Vector3 PixelToWorld(this Vector2 self)
    {
        return self.PixelToWorld(Camera.main);
    }

    public static Vector3 PixelToWorld(this Vector2 self, Camera camera)
    {
        return camera.ScreenToWorldPoint(self);
    }

    public static Vector3 PixelToWorld(this Vector3 self)
    {
        return self.PixelToWorld(Camera.main);
    }

    public static Vector3 PixelToWorld(this Vector3 self, Camera camera)
    {
        return camera.ScreenToWorldPoint(self);
    }

    public static Vector3 WorldToView(this Vector3 self)
    {
        return Camera.main.WorldToViewportPoint(self);
    }

    public static Vector3 WorldToScreen(this Vector3 self)
    {
        return Camera.main.WorldToScreenPoint(self);
    }

    public static Vector3 ScreenToWorld(this Vector2 self)
    {
        return Camera.main.ScreenToWorldPoint(self);
    }

    public static Vector3 ScreenToWorld(this Vector3 self)
    {
        return Camera.main.ScreenToWorldPoint(self);
    }

    // todo: add the rest of the coordinate transformation functions
}
