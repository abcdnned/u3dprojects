using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TY {
public class PlayerIventory : MonoBehaviour
{
    internal Item[] items;

    internal int limit = 9;

    private void Awake() {
        items = new Item[limit];
        for (int i = 0; i < limit; ++i) {
            items[i] = null;
        }
    }

    public Item AddItem(Item target, int slot) {
        if (items[slot] == null) {
            items[slot] = target;
            Sprite s = target.itemIcon;
            Debug.Log(this.GetType().Name + " add item ");
            Debug.Log(this.GetType().Name + " " + s == null);
            return null;
        }
        for (int i = 0; i < limit; ++i) {
            if (items[i] == null) {
                items[i] = target;
                Debug.Log(this.GetType().Name + " add item 2 ");
                return null;
            }
        }
        return target;
    }
}

}