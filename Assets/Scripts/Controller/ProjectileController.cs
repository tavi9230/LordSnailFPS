using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    #region Variables
    public Vector2 Offset = Vector2.zero;
    public GameObject Owner;

    [SerializeField]
    private float moveSpeed = 3f;
    private Vector2 location;
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
            var mousePosition = Camera.main.ScreenToWorldPoint((Vector2)Input.mousePosition);
            location = mousePosition - transform.position;
        }
        else
        {
            location = GameObject.Find("Player").transform.position - transform.position;
        }
        location.Normalize();
        Destroy(gameObject, destroyCounter);
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 currentPosition = new Vector2(transform.position.x, transform.position.y);
        Vector2 newPosition = currentPosition + location * moveSpeed * Time.deltaTime;
        transform.position = newPosition;
        
        RaycastHit2D[] hits = Physics2D.LinecastAll(currentPosition + Offset, newPosition + Offset);

        if(Vector3.Distance(startPosition, currentPosition)> range)
        {
            Destroy(gameObject);
        }

        foreach(RaycastHit2D hit in hits)
        {
            if (hit.collider.gameObject.CompareTag("SolidObject"))
            {
                Debug.Log("HIT WALL");
                Destroy(gameObject);
            }
        }

        Debug.DrawLine(currentPosition + Offset, newPosition + Offset, Color.green);
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
