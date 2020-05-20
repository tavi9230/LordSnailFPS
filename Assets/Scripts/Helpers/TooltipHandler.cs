using UnityEngine.UI;

public static class TooltipHandler
{
    public static void DisplayTooltip(UIManager uiManager, string name, string quantityText = "")
    {
        uiManager.Tooltip.transform.GetChild(0).GetComponent<Text>().text = string.Format("{0}{1}", name, quantityText);
        uiManager.Tooltip.SetActive(true);
    }

    public static void DisplayEnemyTooltip(UIManager uiManager, string name, float health, float maxHealth)
    {
        var tooltipText = uiManager.Tooltip.transform.GetChild(0);
        tooltipText.GetComponent<Text>().text = string.Format("{0}", name);

        var healthBar = uiManager.Tooltip.transform.GetChild(1);
        var healthBarSlider = healthBar.GetComponent<Slider>();
        healthBarSlider.maxValue = maxHealth;
        healthBarSlider.minValue = 0;
        healthBarSlider.value = health;
        healthBarSlider.gameObject.SetActive(true);

        uiManager.Tooltip.SetActive(true);
    }

    public static void HideTooltip(UIManager uiManager)
    {
        var healthBar = uiManager.Tooltip.transform.GetChild(1);
        var healthBarSlider = healthBar.GetComponent<Slider>();
        healthBarSlider.gameObject.SetActive(false);
        uiManager.Tooltip.SetActive(false);
        uiManager.Tooltip.transform.GetChild(0).GetComponent<Text>().text = "";
    }
}