using UnityEngine;

public class MathP
{
    public static Vector2 RadianToVector(float radian)
    {
        return new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
    }
    public static Vector2 DegreeToVector(float degree)
    {
        return RadianToVector((degree % 360) * Mathf.Deg2Rad);
    }
    public static Quaternion DegreeToQuaternion2D(float degree)
    {
        return Quaternion.Euler(0, 0, degree);
    }

    public static Quaternion RotationFromDirection(Vector2 dir)
    {
        float targetAngle = VectorToDegree(dir) + 180;
        if (dir.x < 0)
        {
            targetAngle += 180;
        }
        Quaternion rot = Quaternion.Euler(0, 0, targetAngle);
        return rot;
    }

    public static float VectorToDegree(Vector2 dir)
    {
        float z;
        if (Mathf.Approximately(0, dir.x))
        {
            if (dir.y >= 0) z = 90;
            else z = 270;
        }
        else
            z = Mathf.Atan(dir.y / dir.x) * Mathf.Rad2Deg;
        return z;
    }
}
