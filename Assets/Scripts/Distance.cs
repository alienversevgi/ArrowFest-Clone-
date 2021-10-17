using UnityEngine;

public enum Axis
{
    X, Y, Z
}

public enum AxisT
{
    XY, XZ, YZ
}

public static class Distance
{
    public static float DistanceAnAxis(this Transform transform, Transform other, Axis axis)
    {
        return DistanceAnAxis(transform, other.position, axis);
    }

    public static float DistanceAnAxis(this Transform transform, Vector3 other, Axis axis)
    {
        float distance = 0;
        switch (axis)
        {
            case Axis.X:
                distance = Mathf.Abs(transform.position.x - other.x);
                break;
            case Axis.Y:
                distance = Mathf.Abs(transform.position.y - other.y);
                break;
            case Axis.Z:
                distance = Mathf.Abs(transform.position.z - other.z);
                break;
            default:
                break;
        }

        return distance;
    }

    public static float DistanceAnAxisNoAbs(this Transform transform, Vector3 other, Axis axis)
    {
        float distance = 0;
        switch (axis)
        {
            case Axis.X:
                distance = transform.position.x - other.x;
                break;
            case Axis.Y:
                distance = transform.position.y - other.y;
                break;
            case Axis.Z:
                distance = transform.position.z - other.z;
                break;
            default:
                break;
        }

        return distance;
    }

    public static float DistanceAnAxis(this Vector3 vector, Vector3 other, Axis axis)
    {
        float distance = 0;
        switch (axis)
        {
            case Axis.X:
                distance = vector.x - other.x;
                break;
            case Axis.Y:
                distance = vector.y - other.y;
                break;
            case Axis.Z:
                distance = vector.z - other.z;
                break;
            default:
                break;
        }

        return distance;
    }

    public static float DistanceTwoAxis(this Vector3 vector, Vector3 other, AxisT axisT)
    {
        float distance = 0;
        switch (axisT)
        {
            case AxisT.XY:
                other.z = vector.z; distance = Vector3.Distance(vector, other);
                break;
            case AxisT.XZ:
                other.y = vector.y; distance = Vector3.Distance(vector, other);
                break;
            case AxisT.YZ:
                other.x = vector.x; distance = Vector3.Distance(vector, other);
                break;
            default:
                break;
        }

        return distance;
    }

    public static Vector3 VectorXZ(this Vector3 vector)
    {
        return new Vector3(vector.x, 0, vector.z);
    }

    public static Vector3 VectorXY(this Vector3 vector)
    {
        return new Vector3(vector.x, vector.y, 0);
    }

    public static Vector3 VectorYZ(this Vector3 vector)
    {
        return new Vector3(0, vector.y, vector.z);
    }

    public static Vector3 SetAxisT(this Vector3 vector, AxisT axis, Vector3 value)
    {
        Vector3 newValue = vector;
        switch (axis)
        {
            case AxisT.XY:
                newValue = new Vector3(value.x, value.y, vector.z);
                break;
            case AxisT.XZ:
                newValue = new Vector3(value.x, vector.y, value.z);
                break;
            case AxisT.YZ:
                newValue = new Vector3(vector.x, value.y, value.z);
                break;
        }

        return newValue;

    }

    public static Vector3 SetOnlyOneAxisVector(this Vector3 vector, Axis axis, float value)
    {
        switch (axis)
        {
            case Axis.X:
                return new Vector3(value, vector.y, vector.z);
            case Axis.Y:
                return new Vector3(vector.x, value, vector.z);
            case Axis.Z:
                return new Vector3(vector.x, vector.y, value);
            default:
                return vector;
        }
    }
}
