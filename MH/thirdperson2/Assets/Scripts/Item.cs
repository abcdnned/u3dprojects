using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TY {

[CreateAssetMenu(menuName = "Items/Common Item")]
public class Item : ScriptableObject
{
    [Header("Item Information")]
    public Sprite itemIcon;
    public string itemName;

    public GameObject model;

    public GameObject missle;

    public string description;
}

}