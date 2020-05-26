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
    private Rigidbody2D myRB;
    private float destroyCounter = 5f;
    private float range = 0;
    private Vector3 startPosition;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;
        myRB = GetComponent<Rigidbody2D>();
        if (Owner.GetComponent<PlayerController>() != null)
        {
            var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            // instead of 1 put y of target
            mousePosition = new Vector3(mousePosition.x, 1, mousePosition.z);
            location = mousePosition - transform.position;
        }
        else
        {
            location = GameObject.Find("Player").transform.position - transform.position;
        }
        
        StartCoroutine("DebugDrawLine", location);
        location.Normalize();
        Destroy(gameObject, destroyCounter);
    }

    private IEnumerator DebugDrawLine(Vector3 location)
    {
        while (gameObject != null)
        {
            yield return new WaitForSeconds(0);
            Debug.DrawLine(transform.position, location);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 currentPosition = transform.position;
        Vector3 newPosition = currentPosition + location * moveSpeed * Time.deltaTime;
        transform.position = newPosition;

        if (Vector3.Distance(startPosition, currentPosition) > range)
        {
            Destroy(gameObject);
        }

        if (Physics.Linecast(currentPosition, newPosition, Constants.LAYER_SOLID_OBJECTS))
        {
            Debug.Log("HIT SolidObjects");
        }
        else if (Physics.Linecast(currentPosition, newPosition, Constants.LAYER_DEFAULT))
        {
            Debug.Log("HIT Default");
        }
    }

    public void Setup(GameObject owner, Quaternion rotation, float range)
    {
        Owner = owner;
        this.range = range;
        transform.Rotate(rotation.eulerAngles);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col != null && col.gameObject != null)
        {
            if (col.gameObject.CompareTag("HidingObject")
            || (col.gameObject.CompareTag("Player") && col.gameObject.tag != Owner.tag)
            || (col.gameObject.CompareTag("Enemy") && col.gameObject.tag != Owner.tag && !col.gameObject.GetComponent<EnemyController>().State.Exists(s => s == StateEnum.Dead)))
            {
                Destroy(gameObject);
            }
        }
    }
}
