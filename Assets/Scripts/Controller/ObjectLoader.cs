using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

public static class ObjectLoader
{
    private static Dictionary<string, InventoryItem> items;
    private static Dictionary<ItemTypeEnum, List<InventoryItem>> itemsByLocation;
    
    public static Dictionary<ItemTypeEnum, List<InventoryItem>> GetItemsByLocation()
    {
        return itemsByLocation;
    }

    public static Dictionary<string, InventoryItem> LoadItems()
    {
        items = new Dictionary<string, InventoryItem>();
        itemsByLocation = new Dictionary<ItemTypeEnum, List<InventoryItem>>();

        TextAsset xmlFile = (TextAsset)Resources.Load("Settings/items");
        MemoryStream assetStream = new MemoryStream(xmlFile.bytes);
        XmlReader reader = XmlReader.Create(assetStream);
        XmlDocument xmlDoc = new XmlDocument();
        try
        {
            xmlDoc.Load(reader);
            XmlNode allItems = xmlDoc.ChildNodes[1];
            for (var i = 0; i < allItems.ChildNodes.Count; i++)
            {
                switch (allItems.ChildNodes[i].Name)
                {
                    case "weapons":
                        LoadWeapons(allItems.ChildNodes[i]);
                        break;
                    case "armors":
                        LoadArmors(allItems.ChildNodes[i]);
                        break;
                    case "ammo":
                        LoadAmmo(allItems.ChildNodes[i]);
                        break;
                    case "consumables":
                        LoadConsumables(allItems.ChildNodes[i]);
                        break;
                    default:
                        break;
                }
            }
        }
        catch (Exception ex)
        {
            Debug.Log("Error loading " + xmlFile.name + ":\n" + ex);
        }

        return items;
    }

    private static void LoadWeapons(XmlNode weapons)
    {
        for (var i = 0; i < weapons.ChildNodes.Count; i++)
        {
            switch (weapons.ChildNodes[i].Name)
            {
                case "axes":
                    LoadItem(weapons.ChildNodes[i], ItemTypeEnum.Weapon, ItemCategoryEnum.Axe);
                    break;
                case "bows":
                    LoadItem(weapons.ChildNodes[i], ItemTypeEnum.Weapon, ItemCategoryEnum.Bow);
                    break;
                case "clubs":
                    LoadItem(weapons.ChildNodes[i], ItemTypeEnum.Weapon, ItemCategoryEnum.Club);
                    break;
                case "crossbows":
                    LoadItem(weapons.ChildNodes[i], ItemTypeEnum.Weapon, ItemCategoryEnum.Crossbow);
                    break;
                case "daggers":
                    LoadItem(weapons.ChildNodes[i], ItemTypeEnum.Weapon, ItemCategoryEnum.Dagger);
                    break;
                case "hammers":
                    LoadItem(weapons.ChildNodes[i], ItemTypeEnum.Weapon, ItemCategoryEnum.Hammer);
                    break;
                case "other":
                    LoadItem(weapons.ChildNodes[i], ItemTypeEnum.Weapon, ItemCategoryEnum.Other);
                    break;
                case "polearms":
                    LoadItem(weapons.ChildNodes[i], ItemTypeEnum.Weapon, ItemCategoryEnum.Spear);
                    break;
                case "swords":
                    LoadItem(weapons.ChildNodes[i], ItemTypeEnum.Weapon, ItemCategoryEnum.Sword);
                    break;
                default:
                    break;
            }
        }
    }

    private static void LoadArmors(XmlNode armors)
    {
        for (var i = 0; i < armors.ChildNodes.Count; i++)
        {
            switch (armors.ChildNodes[i].Name)
            {
                case "torso":
                    LoadItem(armors.ChildNodes[i], ItemTypeEnum.Armor, ItemCategoryEnum.Armor);
                    break;
                case "head":
                    LoadItem(armors.ChildNodes[i], ItemTypeEnum.Head, ItemCategoryEnum.Armor);
                    break;
                case "neck":
                    LoadItem(armors.ChildNodes[i], ItemTypeEnum.Neck, ItemCategoryEnum.Amulet);
                    break;
                case "ring":
                    LoadItem(armors.ChildNodes[i], ItemTypeEnum.Ring, ItemCategoryEnum.Ring);
                    break;
                case "shield":
                    LoadItem(armors.ChildNodes[i], ItemTypeEnum.Shield, ItemCategoryEnum.Shield);
                    break;
                case "hip":
                    LoadItem(armors.ChildNodes[i], ItemTypeEnum.Hip, ItemCategoryEnum.Armor);
                    break;
                case "feet":
                    LoadItem(armors.ChildNodes[i], ItemTypeEnum.Feet, ItemCategoryEnum.Armor);
                    break;
                case "hands":
                    LoadItem(armors.ChildNodes[i], ItemTypeEnum.Hands, ItemCategoryEnum.Armor);
                    break;
                default:
                    break;
            }
        }
    }

    private static void LoadAmmo(XmlNode ammo)
    {
        for (var i = 0; i < ammo.ChildNodes.Count; i++)
        {
            switch (ammo.ChildNodes[i].Name)
            {
                case "bows":
                    LoadItem(ammo.ChildNodes[i], ItemTypeEnum.Ammo, ItemCategoryEnum.BowAmmo);
                    break;
                case "crossbows":
                    LoadItem(ammo.ChildNodes[i], ItemTypeEnum.Ammo, ItemCategoryEnum.CrossbowAmmo);
                    break;
            }
        }
    }

    private static void LoadConsumables(XmlNode consumables)
    {
        for (var i = 0; i < consumables.ChildNodes.Count; i++)
        {
            switch (consumables.ChildNodes[i].Name)
            {
                case "potions":
                    LoadItem(consumables.ChildNodes[i], ItemTypeEnum.Consumable, ItemCategoryEnum.Potion);
                    break;
                case "food":
                    LoadItem(consumables.ChildNodes[i], ItemTypeEnum.Consumable, ItemCategoryEnum.Food);
                    break;
            }
        }
    }

    private static void LoadItem(XmlNode itemNode, ItemTypeEnum type, ItemCategoryEnum category)
    {
        for (var i = 0; i < itemNode.ChildNodes.Count; i++)
        {
            InventoryItem item = new InventoryItem();
            foreach (XmlNode node in itemNode.ChildNodes[i].ChildNodes)
            {
                switch (node.Name)
                {
                    case "id":
                        item.Id = node.InnerText;
                        break;
                    case "tier":
                        item.Tier = int.Parse(node.InnerText);
                        break;
                    case "name":
                        item.Name = node.InnerText;
                        break;
                    case "description":
                        item.Description = node.InnerText;
                        break;
                    case "weight":
                        item.Weight = (WeightEnum)int.Parse(node.InnerText);
                        break;
                    case "gameobject":
                        item.GameObject = (GameObject)Resources.Load(node.InnerText);
                        break;
                    case "armor":
                        item.Resistance = GetResistances(node);
                        break;
                    case "durability":
                        item.Durability = new MinMax()
                        {
                            DefaultMinValue = float.Parse(node.ChildNodes[0].InnerText),
                            DefaultMaxValue = float.Parse(node.ChildNodes[1].InnerText)
                        };
                        break;
                    case "inventorySlots":
                        item.InventorySlots = new MinMax()
                        {
                            DefaultMinValue = int.Parse(node.ChildNodes[0].InnerText),
                            DefaultMaxValue = int.Parse(node.ChildNodes[1].InnerText)
                        };
                        break;
                    case "damage":
                        item.Damage = GetResistances(node);
                        break;
                    case "range":
                        item.Range = new MinMax()
                        {
                            DefaultMinValue = float.Parse(node.ChildNodes[0].InnerText),
                            DefaultMaxValue = float.Parse(node.ChildNodes[1].InnerText)
                        };
                        break;
                    case "properties":
                        List<PropertiesEnum> properties = new List<PropertiesEnum>();
                        foreach (XmlNode property in node.ChildNodes)
                        {
                            properties.Add((PropertiesEnum)int.Parse(property.InnerText));
                        }
                        item.Properties = properties;
                        break;
                    case "maxQuantity":
                        item.Quantity = new MinMax()
                        {
                            Value = 0,
                            MaxValue = int.Parse(node.InnerText)
                        };
                        break;
                    case "projectile":
                        item.Projectile = (GameObject)Resources.Load(node.InnerText);
                        break;
                    case "attackRecoveryTime":
                        item.AttackRecoveryTime = new MinMax()
                        {
                            DefaultMinValue = float.Parse(node.ChildNodes[0].InnerText),
                            DefaultMaxValue = float.Parse(node.ChildNodes[1].InnerText)
                        };
                        break;
                    case "staminaConsumption":
                        item.StaminaConsumption = new MinMax()
                        {
                            DefaultMinValue = float.Parse(node.ChildNodes[0].InnerText),
                            DefaultMaxValue = float.Parse(node.ChildNodes[1].InnerText)
                        };
                        break;
                    case "speed":
                        item.Speed = new MinMax()
                        {
                            DefaultMinValue = float.Parse(node.ChildNodes[0].InnerText),
                            DefaultMaxValue = float.Parse(node.ChildNodes[1].InnerText)
                        };
                        break;
                    case "criticalChance":
                        item.CriticalChance = new MinMax()
                        {
                            DefaultMinValue = float.Parse(node.ChildNodes[0].InnerText),
                            DefaultMaxValue = float.Parse(node.ChildNodes[1].InnerText)
                        };
                        break;
                    case "regain":
                        item.Regain = new MinMax()
                        {
                            DefaultMinValue = float.Parse(node.ChildNodes[0].InnerText),
                            DefaultMaxValue = float.Parse(node.ChildNodes[1].InnerText)
                        };
                        break;
                    case "chargeSpeed":
                        item.ChargeSpeed = new MinMax()
                        {
                            DefaultMinValue = float.Parse(node.ChildNodes[0].InnerText),
                            DefaultMaxValue = float.Parse(node.ChildNodes[1].InnerText)
                        };
                        break;
                    default:
                        break;
                }
            }

            item.ItemCategory = category;
            item.Type = type;

            if (itemsByLocation.ContainsKey(type))
            {
                itemsByLocation[type].Add(item);
            }
            else
            {
                itemsByLocation.Add(type, new List<InventoryItem>() { item });
            }

            items.Add(item.Id, item);
        }
    }

    private static Dictionary<DamageTypeEnum, Damage> GetResistances(XmlNode resistancesNode)
    {
        Dictionary<DamageTypeEnum, Damage> resistances = new Dictionary<DamageTypeEnum, Damage>()
        {
            {DamageTypeEnum.Piercing, new Damage() { Type=DamageTypeEnum.Piercing } },
            {DamageTypeEnum.Slashing, new Damage() { Type=DamageTypeEnum.Slashing }},
            {DamageTypeEnum.Bludgeoning, new Damage() { Type=DamageTypeEnum.Bludgeoning }},
            {DamageTypeEnum.Fire, new Damage() { Type=DamageTypeEnum.Fire }},
            {DamageTypeEnum.Cold, new Damage() { Type=DamageTypeEnum.Cold }},
            {DamageTypeEnum.Lightning, new Damage() { Type=DamageTypeEnum.Lightning }},
            {DamageTypeEnum.Poison, new Damage() { Type=DamageTypeEnum.Poison }},
        };
        foreach (XmlNode node in resistancesNode.ChildNodes)
        {
            switch (node.Name)
            {
                case "piercing":
                    resistances[DamageTypeEnum.Piercing].Type = DamageTypeEnum.Piercing;
                    resistances[DamageTypeEnum.Piercing].DefaultMinValue = int.Parse(node.ChildNodes[0].InnerText);
                    resistances[DamageTypeEnum.Piercing].DefaultMaxValue = int.Parse(node.ChildNodes[1].InnerText);
                    break;
                case "slashing":
                    resistances[DamageTypeEnum.Slashing].Type = DamageTypeEnum.Slashing;
                    resistances[DamageTypeEnum.Slashing].DefaultMinValue = int.Parse(node.ChildNodes[0].InnerText);
                    resistances[DamageTypeEnum.Slashing].DefaultMaxValue = int.Parse(node.ChildNodes[1].InnerText);
                    break;
                case "bludgeoning":
                    resistances[DamageTypeEnum.Bludgeoning].Type = DamageTypeEnum.Bludgeoning;
                    resistances[DamageTypeEnum.Bludgeoning].DefaultMinValue = int.Parse(node.ChildNodes[0].InnerText);
                    resistances[DamageTypeEnum.Bludgeoning].DefaultMaxValue = int.Parse(node.ChildNodes[1].InnerText);
                    break;
                case "fire":
                    resistances[DamageTypeEnum.Fire].Type = DamageTypeEnum.Fire;
                    resistances[DamageTypeEnum.Fire].DefaultMinValue = int.Parse(node.ChildNodes[0].InnerText);
                    resistances[DamageTypeEnum.Fire].DefaultMaxValue = int.Parse(node.ChildNodes[1].InnerText);
                    break;
                case "cold":
                    resistances[DamageTypeEnum.Cold].Type = DamageTypeEnum.Cold;
                    resistances[DamageTypeEnum.Cold].DefaultMinValue = int.Parse(node.ChildNodes[0].InnerText);
                    resistances[DamageTypeEnum.Cold].DefaultMaxValue = int.Parse(node.ChildNodes[1].InnerText);
                    break;
                case "lightning":
                    resistances[DamageTypeEnum.Lightning].Type = DamageTypeEnum.Lightning;
                    resistances[DamageTypeEnum.Lightning].DefaultMinValue = int.Parse(node.ChildNodes[0].InnerText);
                    resistances[DamageTypeEnum.Lightning].DefaultMaxValue = int.Parse(node.ChildNodes[1].InnerText);
                    break;
                case "poison":
                    resistances[DamageTypeEnum.Poison].Type = DamageTypeEnum.Poison;
                    resistances[DamageTypeEnum.Poison].DefaultMinValue = int.Parse(node.ChildNodes[0].InnerText);
                    resistances[DamageTypeEnum.Poison].DefaultMaxValue = int.Parse(node.ChildNodes[1].InnerText);
                    break;
                default:
                    break;
            }
        }
        return resistances;
    }
}
