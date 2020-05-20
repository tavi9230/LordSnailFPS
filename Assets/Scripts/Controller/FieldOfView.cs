using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    [SerializeField]
    private LayerMask layerMask;
    private Mesh mesh;
    private float fov;
    private float viewDistance;
    private Vector3 origin;
    private float startingAngle;
    private GameManager gameManager;

    public bool IsPlayerInSight;
    public bool IsDeadEnemyInSight;
    public GameObject DeadEnemy;

    public void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        origin = Vector3.zero;
        fov = 90f;
        viewDistance = 5f;
    }

    public void LateUpdate()
    {
        transform.position = Vector3.zero;

        int rayCount = 50;
        float angle = startingAngle;
        float angleIncease = fov / rayCount;

        Vector3[] vertices = new Vector3[rayCount + 1 + 1];
        Vector2[] uv = new Vector2[vertices.Length];
        int[] triangles = new int[rayCount * 3];

        vertices[0] = origin;

        int vertexIndex = 1;
        int triangleIndex = 0;

        IsPlayerInSight = false;
        IsDeadEnemyInSight = false;
        DeadEnemy = null;

        for (var i = 0; i <= rayCount; i++)
        {
            Vector3 vertex;
            RaycastHit2D raycastHit2D = Physics2D.Raycast(origin, GetVectorFromAngle(angle), viewDistance, layerMask);
            if (raycastHit2D.collider == null)
            {
                // No hit
                vertex = origin + GetVectorFromAngle(angle) * viewDistance;
            }
            else
            {
                // Hit object
                vertex = raycastHit2D.point;

                if (raycastHit2D.collider.CompareTag("HideableObject"))
                {
                    vertex = origin + GetVectorFromAngle(angle) * viewDistance;
                }
                if (raycastHit2D.collider.CompareTag("PlayerPositionCollider"))
                {
                    var player = raycastHit2D.collider.transform.parent.GetComponent<PlayerController>();
                    var ec = gameObject.transform.parent.GetComponent<EnemyController>();
                    if (player.Conditions.Exists(c => c == ConditionEnum.Visible)
                        || ec.State.Exists(s => s == StateEnum.InCombat) && ec.PlayerShadow != null)
                    {
                        IsPlayerInSight = true;
                    }
                    if (player.Conditions.Exists(c => c == ConditionEnum.Invisible))
                    {
                        vertex = origin + GetVectorFromAngle(angle) * viewDistance;
                    }
                }
                if (raycastHit2D.collider.CompareTag("Enemy"))
                {
                    var enemy = raycastHit2D.collider.gameObject;
                    var ec = enemy.GetComponent<EnemyController>();

                    if (ec.State.Exists(s => s == StateEnum.Dead)
                        && ec.Conditions.Exists(c => c == ConditionEnum.Visible)
                        && DistanceToDeadSpot(enemy) >= 1.2)
                    {
                        var pc = FindObjectOfType<PlayerController>();
                        if (!pc.State.Exists(s => s == StateEnum.Crouching))
                        {
                            GameObject.Find("Map").transform.GetChild(2).gameObject.layer = Constants.LAYER_SOLID_OBJECTS;
                            RaycastHit2D checkRay = Physics2D.Raycast(origin, GetVectorFromAngle(angle), viewDistance, layerMask);
                            if (checkRay.collider.CompareTag("Enemy"))
                            {
                                IsDeadEnemyInSight = true;
                                DeadEnemy = enemy;
                            }
                            GameObject.Find("Map").transform.GetChild(2).gameObject.layer = Constants.LAYER_DEFAULT;
                        }
                        else
                        {
                            IsDeadEnemyInSight = true;
                            DeadEnemy = enemy;
                        }
                    }
                    vertex = origin + GetVectorFromAngle(angle) * viewDistance;
                }
            }
            vertices[vertexIndex] = vertex;

            if (i > 0)
            {
                triangles[triangleIndex + 0] = 0;
                triangles[triangleIndex + 1] = vertexIndex - 1;
                triangles[triangleIndex + 2] = vertexIndex;

                triangleIndex += 3;
            }
            vertexIndex++;

            angle -= angleIncease;
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
        mesh.bounds = new Bounds(origin, Vector3.one * 1000f);
    }

    private Vector3 GetVectorFromAngle(float angle)
    {
        // angle = 0 -> 360
        float angleRad = angle * (Mathf.PI / 180f);
        return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
    }

    private float GetAngleFromVector(Vector3 dir)
    {
        dir = dir.normalized;
        float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (n < 0)
        {
            n += 360;
        }
        return n;
    }

    private float DistanceToDeadSpot(GameObject enemy)
    {
        return Vector3.Distance(enemy.transform.position, gameManager.MapManager.GetWorldPosition(gameManager.MapManager.DeadSpot));
    }

    public void SetOrigin(Vector3 origin)
    {
        this.origin = origin;
    }

    public void SetAimDirection(Vector3 aimDirection)
    {
        // If used for player + 90 is needed
        startingAngle = (GetAngleFromVector(aimDirection) - fov / 2f) + 90;
    }

    public void SetViewDistance(float viewDistance)
    {
        this.viewDistance = viewDistance;
    }

    public void SetFoV(float fov)
    {
        this.fov = fov;
    }
}
