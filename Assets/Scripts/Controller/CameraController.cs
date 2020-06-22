using UnityEngine;

public class CameraController2 : MonoBehaviour
{
    #region Variables

    public Transform target;
    [Range(0.01f, 1.0f)]
    public float smoothing;

    private GameManager gm;
    private Camera cameraComponent;

    private Vector3 cameraOffset;
    #endregion

    void Start()
    {
        gm = FindObjectOfType<GameManager>();
        cameraComponent = transform.GetChild(0).GetComponent<Camera>();
        cameraOffset = transform.position - target.position;
    }

    void LateUpdate()
    {
        Vector3 newPos = target.position + cameraOffset;
        transform.position = Vector3.Slerp(transform.position, newPos, smoothing);

        //if (transform.position != target.position)
        //{
        //    Vector3 targetPosition = new Vector3(target.position.x - 9.5f, transform.position.y, target.position.z - 31);
        //    transform.position = Vector3.Lerp(transform.position, targetPosition, smoothing);
        //}
    }
}
