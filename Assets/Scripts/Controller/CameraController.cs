using UnityEngine;

public class CameraController : MonoBehaviour
{
    #region Variables

    public Transform target;
    [Range(0.01f, 1.0f)]
    public float smoothing;
    public float distance;

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
        //if(target)
        //{
        //    Vector3 newPos = target.position - (transform.rotation * Vector3.forward * distance);
        //    transform.position = Vector3.Slerp(transform.position, newPos, smoothing);
        //}
        Vector3 newPos = target.position + cameraOffset;
        transform.position = Vector3.Slerp(transform.position, newPos, smoothing);
    }
}
