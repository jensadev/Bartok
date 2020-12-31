using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    [Header("Set Dynamically")]
    public string suit;
    public int rank;

    // black = 41, 29, 43
    // red = 120, 29, 79
    // white = 245, 255, 232
    public Color color = new Color(41f/255f, 29f/255f, 43f/255f);
    public string colS = "Black";
    public List<GameObject> decoGOs = new List<GameObject>();
    public List<GameObject> pipGOs = new List<GameObject>();
    public GameObject back;
    public CardDefinition def;

    public SpriteRenderer[] spriteRenderers;

    public bool faceUp {
        get {
            return (!back.activeSelf);
        }
        set {
            back.SetActive(!value);
        }
    }

    virtual public void OnMouseUpAsButton() {
        print (name);
    }

    public override string ToString () {
        return "Card: "
            + suit + ", "
            + rank + ", "
            + colS + ", "
            + def.face;
    }

    public void PopulateSpriteRenderers () {
        if (spriteRenderers == null || spriteRenderers.Length == 0) {
            spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        }
    }

    public void SetSortingLayerName (string tSLN) {
        PopulateSpriteRenderers();

        foreach (SpriteRenderer tSR in spriteRenderers)
        {
            tSR.sortingLayerName = tSLN;
        }
    }

    public void SetSortOrder (int sOrd) {
        PopulateSpriteRenderers();
        foreach (SpriteRenderer tSR in spriteRenderers)
        {
            if (tSR.gameObject == this.gameObject) {
                tSR.sortingOrder = sOrd;
                continue;
            }

            switch (tSR.gameObject.name) {
                case "back":
                    tSR.sortingOrder = sOrd + 2;
                    break;
                case "face":
                default:
                    tSR.sortingOrder = sOrd + 1;
                    break;
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        SetSortOrder(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

[System.Serializable]
public class CardDefinition
{
    public int rank;
    public string face;
//    public Decorator[] pip;
    public List<Decorator> pip = new List<Decorator>();
}

[System.Serializable]
public class Decorator
{
    public string type;
    public float x;
    public float y;
    public float z;
    public Vector3 loc;
    public bool flip = false;
    public float scale = 1f;

    public override string ToString() {
        return "Deco: " 
            + type + ", "
            + x + ", " + y + ", "
            + flip + ", "
            + scale;
    }
}
