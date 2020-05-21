using UnityEngine;

public class CameraController : MonoBehaviour
{
    #region Variables
    private const float CLAMP = 1f;

    public Transform target;
    public float smoothing;
    public Vector3 minPosition;
    public Vector3 maxPosition;

    public float zoomSpeed = 1;
    public float defaultTargetOrtho;
    public float smoothSpeed = 2.0f;
    public float minOrtho = 1.0f;
    public float maxOrtho = 20.0f;

    private GameManager gm;
    private float targetOrtho;
    private Camera cameraComponent;
    #endregion

    void Start()
    {
        gm = FindObjectOfType<GameManager>();
        cameraComponent = GetComponent<Camera>();
        defaultTargetOrtho = cameraComponent.orthographicSize;
        targetOrtho = defaultTargetOrtho;
    }

    void Update()
    {
        //if (gm.CombatEnemyList.Count > 0)
        //{
        //    if (targetOrtho >= defaultTargetOrtho - 1.2)
        //    {
        //        targetOrtho -= zoomSpeed;
        //    }
        //    targetOrtho = Mathf.Clamp(targetOrtho, minOrtho, maxOrtho);
        //}
        //else
        //{
        targetOrtho += zoomSpeed;
        if (targetOrtho >= defaultTargetOrtho)
        {
            targetOrtho = defaultTargetOrtho;
        }
        targetOrtho = Mathf.Clamp(targetOrtho, minOrtho, maxOrtho);
        //}
        cameraComponent.orthographicSize = Mathf.MoveTowards(cameraComponent.orthographicSize, targetOrtho, smoothSpeed * Time.deltaTime);
    }

    void LateUpdate()
    {
        if (transform.position != target.position)
        {
            Vector3 targetPosition = new Vector3(target.position.x, target.position.y, transform.position.z);

            targetPosition.x = Mathf.Clamp(targetPosition.x, minPosition.x, maxPosition.x);
            targetPosition.y = Mathf.Clamp(targetPosition.y, minPosition.y, maxPosition.y);

            transform.position = Vector3.Lerp(transform.position, targetPosition, smoothing);
        }
    }
}
