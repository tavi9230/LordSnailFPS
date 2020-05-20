using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    #region Variables
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private RectTransform ttt;
    private GameObject selectedItem;
    private ItemStatus itemStatus;
    private EnemyController enemyController;
    #endregion

    private void Awake()
    {
        if (canvas == null)
        {
            canvas = GameObject.Find("UI").GetComponent<Canvas>();
        }
        ttt = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        selectedItem = GameObject.Find("SelectedItemIcon");
    }

    // TODO: Maybe replace drag'n'drop with point and click?
    public void OnBeginDrag(PointerEventData eventData)
    {
        itemStatus = gameObject.GetComponent<ItemStatus>();
        enemyController = itemStatus.Owner.GetComponent<EnemyController>();
        if (itemStatus.location != InventoryLocationEnum.None
            && (enemyController == null || (enemyController != null && enemyController.State.Exists(s => s == StateEnum.Dead))))
        {
            canvasGroup.alpha = .6f;

            var selectedItemCanvasGroup = selectedItem.GetComponent<CanvasGroup>();
            selectedItemCanvasGroup.blocksRaycasts = false;

            selectedItem.transform.position = Input.mousePosition;


            var selectedItemImg = selectedItem.GetComponent<Image>();
            selectedItemImg.sprite = gameObject.GetComponent<Image>().sprite;
            selectedItemImg.enabled = true;

            var selectedItemUI = selectedItem.GetComponent<ItemStatus>();
            //selectedItemUI.location = InventoryLocationEnum.Feet;
            selectedItemUI.location = gameObject.GetComponent<ItemStatus>().location;
            //selectedItemUI.index = 0;
            selectedItemUI.index = gameObject.GetComponent<ItemStatus>().index;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        itemStatus = gameObject.GetComponent<ItemStatus>();
        enemyController = itemStatus.Owner.GetComponent<EnemyController>();
        if (itemStatus.location != InventoryLocationEnum.None
            && (enemyController == null || (enemyController != null && enemyController.State.Exists(s => s == StateEnum.Dead))))
        {
            var rectTransform = selectedItem.GetComponent<RectTransform>();
            rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        itemStatus = gameObject.GetComponent<ItemStatus>();
        enemyController = itemStatus.Owner.GetComponent<EnemyController>();
        if (itemStatus.location != InventoryLocationEnum.None
            && (enemyController == null || (enemyController != null && enemyController.State.Exists(s => s == StateEnum.Dead))))
        {
            canvasGroup.alpha = 1f;

            var selectedItemCanvasGroup = selectedItem.GetComponent<CanvasGroup>();
            selectedItemCanvasGroup.blocksRaycasts = true;

            selectedItem.GetComponent<Image>().enabled = false;

            var selectedItemUI = selectedItem.GetComponent<ItemStatus>();
            selectedItemUI.location = InventoryLocationEnum.None;
            selectedItemUI.index = 0;
        }
    }
}
