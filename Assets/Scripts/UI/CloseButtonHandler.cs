using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CloseButtonHandler : MonoBehaviour, IPointerClickHandler
{
    private UIManager uiManager;

    void Start()
    {
        uiManager = FindObjectOfType<UIManager>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        switch (gameObject.transform.parent.name)
        {
            case "ObjectInventory":
                uiManager.CloseObjectInventory();
                break;
            case "Inventory":
                CloseInfo();
                break;
            case "CharacterInfo":
                CloseInfo();
                break;
            default:
                uiManager.CloseInventory();
                break;
        }
    }

    private void CloseInfo()
    {
        if (gameObject.transform.parent.parent.name == "PlayerInfo")
        {
            uiManager.CloseInventory();
        }
        else if (gameObject.transform.parent.parent.name == "EnemyInfo")
        {
            uiManager.CloseEnemyInfo();
            PlayerController pc = FindObjectOfType<PlayerController>();
            if (pc != null && !pc.State.Exists(s => s == StateEnum.Dead))
            {
                pc.State.Remove(StateEnum.Inspecting);
            }

            uiManager.CloseEnemyInfo();
            uiManager.InspectedEnemy = null;
        }
    }
}