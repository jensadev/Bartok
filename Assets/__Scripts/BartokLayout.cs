using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SlotDef {
    public float xstagger;
    public int layer;

    public float x;
    public float y;
    public bool faceUp = false;
    public string layerName = "Default";
    public int layerID = 0;
    public int id;
    public string type = "slot";
    public Vector2 stagger;
    public float rot;
    public int player;
    public Vector3 pos;
}


[System.Serializable]
public class LayoutItem {
  public Vector2 multiplier;
  public SlotDef[] slot;
}

public class BartokLayout : MonoBehaviour
{
    [Header("Set Dynamically")]
    public Vector2 multiplier;
    public List<SlotDef> slotDefs;
    public SlotDef drawPile;
    public SlotDef discardPile;
    public SlotDef target;
    public void ReadLayout (string jsonText) {
        LayoutItem items = JsonUtility.FromJson<LayoutItem>(jsonText);
        
        multiplier.x = items.multiplier.x;
        multiplier.y = items.multiplier.y;

        for (int i = 0; i < items.slot.Length; i++) {
            if (items.slot[i].type == null) {
                items.slot[i].type = "slot";
            }

            items.slot[i].pos = new Vector3(
                items.slot[i].x * multiplier.x,
                items.slot[i].y * multiplier.y,
                0
            );

            items.slot[i].layerID = items.slot[i].layer;
            items.slot[i].layerName = items.slot[i].layerID.ToString();

            switch (items.slot[i].type)
            {
                case "slot":
                    break;
                case "drawpile":
                    items.slot[i].stagger.x = items.slot[i].xstagger;
                    drawPile = items.slot[i];
                    break;
                case "discardpile":
                    discardPile = items.slot[i];
                    break;
                case "target":
                    target = items.slot[i];
                    break;
                case "hand":
                    slotDefs.Add(items.slot[i]);
                    break;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}