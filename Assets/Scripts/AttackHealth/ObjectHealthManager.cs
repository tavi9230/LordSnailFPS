using System.Collections.Generic;
using UnityEngine;

public class ObjectHealthManager : MonoBehaviour
{
    public float CurrentHealth = 4;
    public float MaxHealth = 4;

    private bool isFlashActive = false;
    [SerializeField]
    private float flashLength = 0.5f;
    private float flashCounter = 0f;
    private SpriteRenderer objectSprite;
    private float aHue = 0f;

    // Start is called before the first frame update
    void Start()
    {
        objectSprite = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (isFlashActive)
        {
            if (flashCounter > flashLength * .99f)
            {
                objectSprite.color = new Color(objectSprite.color.r, objectSprite.color.g, objectSprite.color.b, 0f);
            }
            else if (flashCounter > flashLength * .82f)
            {
                objectSprite.color = new Color(objectSprite.color.r, objectSprite.color.g, objectSprite.color.b, 1f);
            }
            else if (flashCounter > flashLength * .66f)
            {
                objectSprite.color = new Color(objectSprite.color.r, objectSprite.color.g, objectSprite.color.b, 0f);
            }
            else if (flashCounter > flashLength * .49)
            {
                objectSprite.color = new Color(objectSprite.color.r, objectSprite.color.g, objectSprite.color.b, 1f);
            }
            else if (flashCounter > flashLength * .33f)
            {
                objectSprite.color = new Color(objectSprite.color.r, objectSprite.color.g, objectSprite.color.b, 0f);
            }
            else if (flashCounter > flashLength * .16f)
            {
                objectSprite.color = new Color(objectSprite.color.r, objectSprite.color.g, objectSprite.color.b, 1f);
            }
            else if (flashCounter > 0)
            {
                objectSprite.color = new Color(objectSprite.color.r, objectSprite.color.g, objectSprite.color.b, 0f);
            }
            else
            {
                objectSprite.color = new Color(objectSprite.color.r, objectSprite.color.g, objectSprite.color.b, aHue);
                isFlashActive = false;
            }
            flashCounter -= Time.deltaTime;
        }
        else
        {
            aHue = objectSprite.color.a;
        }
    }

    public void HurtObject(Dictionary<DamageTypeEnum, Damage> damageList)
    {
        isFlashActive = true;
        flashCounter = flashLength;
        float damageToGive = 0;

        foreach (var dmgType in damageList.Keys)
        {
            damageToGive += Random.Range(damageList[dmgType].MinValue, damageList[dmgType].MaxValue);
        }

        CurrentHealth -= damageToGive;
        if (CurrentHealth <= 0)
        {
            var objectController = gameObject.GetComponent<ObjectController>();
            foreach(var content in objectController.EnemyContent)
            {
                var spriteRenderer = content.GetComponent<SpriteRenderer>();
                spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1f);
            }
            foreach(var content in objectController.Content)
            {
                var c = Instantiate(content.GameObject, gameObject.transform.position, gameObject.transform.rotation);
                var pickup = c.gameObject.GetComponent<Pickable>();
                pickup.SetInventoryItem(content);
            }
            var uiManager = FindObjectOfType<UIManager>();
            uiManager.CloseInventory();
            Destroy(gameObject);
        }
    }
}
