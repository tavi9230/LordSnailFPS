using UnityEngine;

public class Point
{
    public float X { get; set; }
    public float Y { get; set; }

    public Point(float x, float y)
    {
        X = x;
        Y = y;
    }

    public Point(float distance, Transform transform)
    {
        MousePositionSpawnPoint(distance, transform);
    }

    private Vector3 DistanceFromMouse(Transform transform)
    {
        var mousePosition = Camera.main.ScreenToWorldPoint((Vector2)Input.mousePosition);
        var distanceToPlayer = -(transform.GetChild(0).transform.position - mousePosition);

        if (distanceToPlayer.x > 0.1f)
        {
            distanceToPlayer.x = 0.11f;
        }
        else if (distanceToPlayer.x < -0.1f)
        {
            distanceToPlayer.x = -0.11f;
        }
        else if (distanceToPlayer.x < 0.1f && distanceToPlayer.x > -0.1f)
        {
            distanceToPlayer.x = 0f;
        }
        if (distanceToPlayer.y > 0.1f)
        {
            distanceToPlayer.y = 0.11f;
        }
        else if (distanceToPlayer.y < -0.1f)
        {
            distanceToPlayer.y = -0.11f;
        }
        else if (distanceToPlayer.y < 0.1f && distanceToPlayer.y > -0.1f)
        {
            distanceToPlayer.y = 0f;
        }
        return distanceToPlayer;
    }

    public void MousePositionSpawnPoint(float distance, Transform transform)
    {
        var distanceToPlayer = Helpers.DistanceFromMouseCustom(transform);
        X = 0f;
        Y = 0f;
        if (distanceToPlayer.x == 0.11f && distanceToPlayer.y == 0.11f)
        {
            X = distance;
            Y = distance;
        }
        else if (distanceToPlayer.x == 0.11f && distanceToPlayer.y == -0.11f)
        {
            X = distance;
            Y = -distance;
        }
        else if (distanceToPlayer.x == -0.11f && distanceToPlayer.y == 0.11f)
        {
            X = -distance;
            Y = distance;
        }
        else if (distanceToPlayer.x == -0.11f && distanceToPlayer.y == -0.11f)
        {
            X = -distance;
            Y = -distance;
        }
        else if (distanceToPlayer.x == 0f && distanceToPlayer.y == 0.11f)
        {
            X = 0f;
            Y = distance;
        }
        else if (distanceToPlayer.x == 0f && distanceToPlayer.y == -0.11f)
        {
            X = 0f;
            Y = -distance;
        }
        else if (distanceToPlayer.x == 0.11f && distanceToPlayer.y == 0f)
        {
            X = distance;
            Y = 0f;
        }
        else if (distanceToPlayer.x == -0.11f && distanceToPlayer.y == 0f)
        {
            X = -distance;
            Y = 0f;
        }
    }
}