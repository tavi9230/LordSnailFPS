using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectController : MonoBehaviour
{
    #region Variables

    public string Name = "";
    public WeightEnum Size;
    public List<GameObject> EnemyContent;
    public int MaxEnemyContent = 1;
    public List<InventoryItem> Content;
    public int MaxContent = 5;

    private Rigidbody2D rigidBody;
    private float defaultMass;
    private float defaultDrag;
    private GameManager gameManager;
    private Vector3 oldPosition;
    private bool hasBeenSearched = false;
    private bool isMenuOpen = false;
    private UIManager uiManager;
    private PlayerController playerController;

    #endregion

    void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
        uiManager = FindObjectOfType<UIManager>();
        EnemyContent = new List<GameObject>();
        Content = new List<InventoryItem>();
        Size = WeightEnum.Medium;
        rigidBody = gameObject.GetComponent<Rigidbody2D>();
        gameManager = FindObjectOfType<GameManager>();
        defaultMass = rigidBody.mass;
        defaultDrag = rigidBody.drag;
        oldPosition = gameObject.transform.GetChild(0).transform.position;
        SpawnLoot();
    }

    private void Update()
    {
        if (!isMenuOpen || isMenuOpen && Vector3.Distance(gameObject.transform.position, playerController.transform.position) > 1.2)
        {
            uiManager.ObjectInventoryUI.SetActive(false);
            isMenuOpen = false;
        }
    }

    private void OnMouseDown()
    {
        if (playerController.State.Exists(s => s == StateEnum.Inspecting)
            && Vector3.Distance(gameObject.transform.position, playerController.transform.position) <= 1.2)
        {
            hasBeenSearched = true;
            DisplayObjectInventory();
            isMenuOpen = true;
        }
    }

    public void DisplayObjectInventory()
    {
        uiManager.DisplayObjectInventory(Name, Content, EnemyContent, gameObject);
    }

    private void OnMouseEnter()
    {
        string quantityText = hasBeenSearched ? string.Format(" [{0}][{1}]", EnemyContent.Count, Content.Count) : "";
        TooltipHandler.DisplayTooltip(uiManager, Name, quantityText);
    }

    private void OnMouseExit()
    {
        TooltipHandler.HideTooltip(uiManager);
    }

    public void SpawnLoot()
    {
        if (gameManager.InventoryItems != null)
        {
            for (var i = 0; i < MaxContent; i++)
            {
                int r = Random.Range(1, 100);
                // 25% chance to spawn item
                if (r >= 75 && r <= 100)
                {
                    var rr = Random.Range(0, gameManager.InventoryItems.Count - 1);
                    var index = 0;
                    InventoryItem itemToSpawn = new InventoryItem();
                    foreach (var item in gameManager.InventoryItems)
                    {
                        if (index == rr)
                        {
                            itemToSpawn = item.Value;
                            break;
                        }
                        index++;
                    }
                    var itm = new InventoryItem(itemToSpawn, true);
                    if (itm.Type == ItemTypeEnum.Ammo)
                    {
                        itm.Quantity.Value = Random.Range(0, itm.Quantity.MaxValue);
                    }
                    Content.Add(itm);
                }
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player") && Input.GetKey(KeyCode.LeftShift))
        {
            PlayerController pc = other.gameObject.GetComponent<PlayerController>();
            if (pc.Stats.TotalAttributes.Strength >= (int)Size / 2)
            {
                // TODO: if >= Size + 10, increase or decrease things to make pushing faster
                rigidBody.mass = 8;
                rigidBody.drag = 5;
            }
        }
        RecalculateMapPosition();
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player") && Input.GetKey(KeyCode.LeftShift))
        {
            PlayerController pc = other.gameObject.GetComponent<PlayerController>();
            if (pc.Stats.TotalAttributes.Strength >= (int)Size / 2)
            {
                rigidBody.mass = 8;
                rigidBody.drag = 5;
            }
        }
        RecalculateMapPosition();
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            rigidBody.mass = defaultMass;
            rigidBody.drag = defaultDrag;
        }
        RecalculateMapPosition();
    }

    private void RecalculateMapPosition()
    {
        var currentPosition = gameObject.transform.GetChild(0).transform.position;
        var currentGridPosition = gameManager.MapManager.GetGridPosition(currentPosition);
        var oldGridPosition = gameManager.MapManager.GetGridPosition(oldPosition);

        if (oldGridPosition != currentGridPosition)
        {
            gameManager.MapManager.ToggleObjectOnMap(oldPosition, TileType.Floor);
            gameManager.MapManager.ToggleObjectOnMap(currentPosition, TileType.Wall);
            oldPosition = currentPosition;
        }
    }
}
