using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillDetails : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private UIManager uiManager;

    void Start()
    {
        uiManager = FindObjectOfType<UIManager>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        var size = uiManager.SkillDetailsUI.transform.GetComponent<RectTransform>().sizeDelta;
        ShowDetails(gameObject.GetComponent<SkillStatus>().skill);
        uiManager.SkillDetailsUI.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HideAllDetails();
    }

    public void HideAllDetails()
    {
        uiManager.SkillDetailsUI.SetActive(false);
    }

    private void ShowDetails(Skill skill)
    {
        var icon = uiManager.SkillDetailsUI.transform.GetChild(0);
        icon.GetComponent<Image>().sprite = skill.GameObject.GetComponent<Image>().sprite;

        var skillNameText = uiManager.SkillDetailsUI.transform.GetChild(1);
        skillNameText.GetComponent<Text>().text = skill.Name;

        var skillDescriptionText = uiManager.SkillDetailsUI.transform.GetChild(2);
        skillDescriptionText.GetComponent<Text>().text = skill.Description;

        var skillTypeText = uiManager.SkillDetailsUI.transform.GetChild(3);
        skillTypeText.GetComponent<Text>().text = skill.Type.ToString();

        var attackRecoveryText = uiManager.SkillDetailsUI.transform.GetChild(4).GetChild(1);
        attackRecoveryText.GetComponent<Text>().text = skill.AttackRecoveryTime.ToString();

        var manaCost = uiManager.SkillDetailsUI.transform.GetChild(5).GetChild(1);
        manaCost.GetComponent<Text>().text = skill.ManaConsumption.ToString();

        ShowDamageDetails(skill);
    }

    private void ShowDamageDetails(Skill skill)
    {
        Transform damageDetails = uiManager.SkillDetailsUI.transform.GetChild(uiManager.SkillDetailsUI.transform.childCount - 1);
        var index = 0;
        foreach (var dmgType in skill.Damage.Keys)
        {
            var dmgDetails = damageDetails.GetChild(index);
            var dmgIcon = dmgDetails.GetChild(0).GetComponent<Image>();
            switch (dmgType)
            {
                case DamageTypeEnum.Bludgeoning:
                    dmgIcon.sprite = uiManager.BludgeoningIcon.GetComponent<Image>().sprite;
                    break;
                case DamageTypeEnum.Piercing:
                    dmgIcon.sprite = uiManager.PiercingIcon.GetComponent<Image>().sprite;
                    break;
                case DamageTypeEnum.Slashing:
                    dmgIcon.sprite = uiManager.SlashingIcon.GetComponent<Image>().sprite;
                    break;
                case DamageTypeEnum.Fire:
                    dmgIcon.sprite = uiManager.FireIcon.GetComponent<Image>().sprite;
                    break;
                case DamageTypeEnum.Cold:
                    dmgIcon.sprite = uiManager.ColdIcon.GetComponent<Image>().sprite;
                    break;
                case DamageTypeEnum.Lightning:
                    dmgIcon.sprite = uiManager.LightningIcon.GetComponent<Image>().sprite;
                    break;
                case DamageTypeEnum.Poison:
                    dmgIcon.sprite = uiManager.PoisonIcon.GetComponent<Image>().sprite;
                    break;
            };
            var text = damageDetails.GetChild(index).GetChild(1).GetComponent<Text>();
            text.text = string.Format("{0} - {1}", skill.Damage[dmgType].MinValue, skill.Damage[dmgType].MaxValue);
            index++;
            dmgDetails.gameObject.SetActive(true);
        }
    }

    //private void ShowResistanceDetails(Skill skill)
    //{
    //    Transform resistanceDetails = uiManager.SpellDetailsUI.transform.GetChild(uiManager.SpellDetailsUI.transform.childCount - 1);
    //    var index = 0;
    //    foreach (var dmgType in skill.Resistance.Keys)
    //    {
    //        var resDetails = resistanceDetails.GetChild(index);
    //        var resIcon = resDetails.GetChild(0).GetComponent<Image>();
    //        switch (dmgType)
    //        {
    //            case DamageTypeEnum.Bludgeoning:
    //                resIcon.sprite = uiManager.BludgeoningIcon.GetComponent<Image>().sprite;
    //                break;
    //            case DamageTypeEnum.Piercing:
    //                resIcon.sprite = uiManager.PiercingIcon.GetComponent<Image>().sprite;
    //                break;
    //            case DamageTypeEnum.Slashing:
    //                resIcon.sprite = uiManager.SlashingIcon.GetComponent<Image>().sprite;
    //                break;
    //            case DamageTypeEnum.Fire:
    //                resIcon.sprite = uiManager.FireIcon.GetComponent<Image>().sprite;
    //                break;
    //            case DamageTypeEnum.Cold:
    //                resIcon.sprite = uiManager.ColdIcon.GetComponent<Image>().sprite;
    //                break;
    //            case DamageTypeEnum.Lightning:
    //                resIcon.sprite = uiManager.LightningIcon.GetComponent<Image>().sprite;
    //                break;
    //            case DamageTypeEnum.Poison:
    //                resIcon.sprite = uiManager.PoisonIcon.GetComponent<Image>().sprite;
    //                break;
    //        };
    //        var text = resistanceDetails.GetChild(index).GetChild(1).GetComponent<Text>();
    //        text.text = string.Format("{0}", skill.Resistance[dmgType].Value);
    //        index++;
    //        resDetails.gameObject.SetActive(true);
    //    }
    //}
}