using System.Collections;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    #region Variables
    public Vector2 Offset = Vector2.zero;
    public GameObject Owner;

    [SerializeField]
    private float moveSpeed = 3f;
    private Vector3 location;
    private Rigidbody myRB;
    private float destroyCounter = 5f;
    private float range = 0;
    private Vector3 startPosition;

    private Vector3 direction;
    private Ray ray;
    #endregion
    
    void Start()
    {
        startPosition = Owner.gameObject.transform.GetChild(2).position;
        transform.position = startPosition;
        myRB = GetComponent<Rigidbody>();
        if (Owner.GetComponent<PlayerController>() != null)
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100))
            {
                direction = hit.point;
            }
            else
            {
                direction = Input.mousePosition;
                direction.z = range * 5;
                direction = Camera.main.ScreenToWorldPoint(direction);
            }
            transform.LookAt(direction);
            location = direction;
        }
        else
        {
            // TODO: Check when enemy shoots
            direction = location;
        }
        
        Destroy(gameObject, destroyCounter);
    }
    
    void Update()
    {
        Debug.DrawLine(startPosition, location, Color.blue);
        
        myRB.velocity = transform.forward * moveSpeed;

        if (Vector3.Distance(startPosition, transform.position) > range * 5)
        {
            Destroy(gameObject);
        }
    }

    public void Setup(GameObject owner, Quaternion rotation, float range)
    {
        Owner = owner;
        this.range = range;
        transform.Rotate(rotation.eulerAngles);
    }

    void OnTriggerEnter(Collider col)
    {
        if (col != null && col.gameObject != null)
        {
            if (col.gameObject.CompareTag("HidingObject")
                || col.gameObject.CompareTag("SolidObject")
                || (col.gameObject.transform.CompareTag("Player") && col.gameObject.transform.tag != Owner.tag)
                || (col.gameObject.transform.CompareTag("Enemy") && col.gameObject.transform.tag != Owner.tag && !col.gameObject.transform.GetComponentInParent<EnemyController>().State.Exists(s => s == StateEnum.Dead)))
            {
                Destroy(gameObject);
            }
        }
    }
}
