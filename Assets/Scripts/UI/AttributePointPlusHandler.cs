using UnityEngine;
using UnityEngine.EventSystems;

public class AttributePointPlusHandler : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        PlayerController playerController = FindObjectOfType<PlayerController>();
        UIManager uiManager = FindObjectOfType<UIManager>();
        var parentName = transform.parent.name;
        switch (parentName)
        {
            case "StrengthSlot":
                if (playerController.Stats.Attributes.Strength < Constants.ATTRIBUTES_MAX_VALUE)
                {
                    playerController.Stats.Attributes.Strength++;
                    playerController.Stats.AvailableAttributePoints--;
                    playerController.InventoryManager.UpdateStatsAndRefreshUI(playerController.Stats);
                }
                break;
            case "DexteritySlot":
                if (playerController.Stats.Attributes.Dexterity < Constants.ATTRIBUTES_MAX_VALUE)
                {
                    playerController.Stats.Attributes.Dexterity++;
                    playerController.Stats.AvailableAttributePoints--;
                    playerController.InventoryManager.UpdateStatsAndRefreshUI(playerController.Stats);
                }
                break;
            case "ConstitutionSlot":
                if (playerController.Stats.Attributes.Constitution < Constants.ATTRIBUTES_MAX_VALUE)
                {
                    playerController.Stats.Attributes.Constitution++;
                    playerController.Stats.AvailableAttributePoints--;
                    playerController.InventoryManager.UpdateStatsAndRefreshUI(playerController.Stats);
                }
                break;
            case "IntelligenceSlot":
                if (playerController.Stats.Attributes.Intelligence < Constants.ATTRIBUTES_MAX_VALUE)
                {
                    playerController.Stats.Attributes.Intelligence++;
                    playerController.Stats.AvailableAttributePoints--;
                    playerController.InventoryManager.UpdateStatsAndRefreshUI(playerController.Stats);
                }
                break;
            case "CharismaSlot":
                if (playerController.Stats.Attributes.Charisma < Constants.ATTRIBUTES_MAX_VALUE)
                {
                    playerController.Stats.Attributes.Charisma++;
                    playerController.Stats.AvailableAttributePoints--;
                    playerController.InventoryManager.UpdateStatsAndRefreshUI(playerController.Stats);
                }
                break;
            default:
                break;
        }
    }
}
