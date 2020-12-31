using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    [Header("Set in Inspector")]
    public bool startFaceUp = true;
    public Sprite suitClub;
    public Sprite suitDiamond;
    public Sprite suitHeart;
    public Sprite suitSpade;
    public Sprite[] faceSprites;
    public Sprite[] rankSprites;

    public Sprite cardBack;
    public Sprite cardBackLight;
    public Sprite cardBackDark;
    public Sprite cardFont;

    public GameObject prefabCard;
    public GameObject prefabSprite;

    [Header("Set Dynamically")]
    public List<string> cardNames;
    public List<Card> cards;
    public List<Decorator> decorators;
    public List<CardDefinition> cardDefs;
    public Transform deckAnchor;
    public Dictionary<string, Sprite> dictSuits;

    public void InitDeck (string deckJSONText) {
        if (GameObject.Find("_Deck") == null) {
            GameObject anchorGo = new GameObject("_Deck");
            deckAnchor = anchorGo.transform;
        }

        dictSuits = new Dictionary<string, Sprite> () {
            { "C", suitClub },
            { "D", suitDiamond },
            { "H", suitHeart },
            { "S", suitSpade }
        };

        ReadDeck(deckJSONText);

        MakeCards();
    }

    public void ReadDeck (string deckJSONText) {
        DeckItem items = JsonUtility.FromJson<DeckItem>(deckJSONText);

        decorators = new List<Decorator> ();
        for (int i = 0;i < items.decorator.Length; i++)
        {
            // items.decorator[i].loc = new Vector3(
            //     items.decorator[i].x,
            //     items.decorator[i].y,
            //     items.decorator[i].z
            // );
            items.decorator[i].loc.x = items.decorator[i].x;
            items.decorator[i].loc.y = items.decorator[i].y;
            items.decorator[i].loc.z = items.decorator[i].z;

            decorators.Add(items.decorator[i]);
        }

        cardDefs = new List<CardDefinition> ();
        for (int i = 0; i < items.card.Length; i++)
        {
            if (items.card[i].pip != null) {
                for (int j = 0; j < items.card[i].pip.Count; j++) {
                    items.card[i].pip[j].type = "pip";
                    // items.card[i].pip[j].loc = new Vector3(
                    //     items.card[i].pip[j].x,
                    //     items.card[i].pip[j].y,
                    //     items.card[i].pip[j].z
                    // );
                    items.card[i].pip[j].loc.x = items.card[i].pip[j].x;
                    items.card[i].pip[j].loc.y = items.card[i].pip[j].y;
                    items.card[i].pip[j].loc.z = items.card[i].pip[j].z;
                }
            }
            cardDefs.Add(items.card[i]);
        }
    }

    public CardDefinition GetCardDefinitionByRank (int rank) {
        foreach (CardDefinition cd in cardDefs)
        {
            if (cd.rank == rank) {
                return ( cd );
            }
        }
        return ( null );
    }

    public void MakeCards () {
        cardNames = new List<string> ();
        string[] letters = new string[] { "C", "D", "H", "S" };
        foreach (string s in letters)
        {
            for (int i = 0; i < 13; i++) {
                cardNames.Add( s + ( i + 1 ) );
            }
        }

        cards = new List<Card> ();

        for (int i = 0; i < cardNames.Count; i++) {
            cards.Add( MakeCard(i) );
        }
        // cards.Add(MakeCard(1));
    }

    private Card MakeCard (int cNum) {
        GameObject cgo = Instantiate(prefabCard) as GameObject;
        cgo.transform.parent = deckAnchor;

        Card card = cgo.GetComponent<Card> ();
        cgo.transform.localPosition = new Vector3 (
            (cNum%13) * 3,
            cNum/13 * 4,
            0
        );

        // Debug.Log(cgo.transform.localPosition);

        card.name = cardNames[cNum];
        card.suit = card.name[0].ToString();
        card.rank = int.Parse( card.name.Substring(1) );
        if (card.suit == "D" || card.suit == "H") {
            card.colS = "Red";
            card.color = new Color(120f/255f, 29f/255f, 79f/255f);
        }

        card.def = GetCardDefinitionByRank(card.rank);

        AddDecorators(card);
        AddPips(card);
        AddFace(card);
        AddBack(card);

        return card;
    }

    private Sprite _tSP = null;
    private GameObject _tGO = null;
    private SpriteRenderer _tSR = null;

    private void AddDecorators(Card card) {
        foreach (Decorator deco in decorators)
        {
            if (deco.type == "suit") {
                _tGO = Instantiate( prefabSprite ) as GameObject;
                _tSR = _tGO.GetComponent<SpriteRenderer> ();
                _tSR.sprite = dictSuits[card.suit];
            } else {
                _tGO = Instantiate( prefabSprite ) as GameObject;
                _tSR = _tGO.GetComponent<SpriteRenderer> ();
                _tSP = rankSprites[card.rank];
                _tSR.sprite = _tSP;
                _tSR.color = card.color;
            }

            _tSR.sortingOrder = 1;
            _tGO.transform.SetParent(card.transform);
            _tGO.transform.localPosition = deco.loc;

            if (deco.flip) {
                _tGO.transform.rotation = Quaternion.Euler(0, 0, 180);
            }

            if (deco.scale != 1) {
                _tGO.transform.localScale = Vector3.one * deco.scale;
            }

            _tGO.name = deco.type;

            card.decoGOs.Add(_tGO);
        }
    }

    private void AddPips (Card card) {
        if(card.def.pip == null) {
            return;
        }
        foreach (Decorator pip in card.def.pip)
        {
            _tGO = Instantiate(prefabSprite) as GameObject;
            _tGO.transform.SetParent(card.transform);
            _tGO.transform.localPosition = pip.loc;

            if (pip.flip) {
                _tGO.transform.rotation = Quaternion.Euler( 0, 0, 180);
            }

            _tGO.transform.localScale = Vector3.one;

            if (pip.scale != 1) {
                _tGO.transform.localScale = Vector3.one * pip.scale;
            }

            // Debug.Log(_tGO.transform.localScale);

            // _tGO.transform.localScale = Vector3.one * 1;

            _tGO.name = "pip";
            _tSR = _tGO.GetComponent<SpriteRenderer> ();

            _tSR.sprite = dictSuits[card.suit];

            _tSR.sortingOrder = 1;

            card.pipGOs.Add(_tGO);
        }
    }

    private void AddFace (Card card) {
        if (card.def.face == null) {
            return;
        }

        _tGO = Instantiate(prefabSprite) as GameObject;
        _tSR = _tGO.GetComponent<SpriteRenderer> ();
        _tSP = GetFace(card.def.face);
        _tSR.sprite= _tSP;
        _tSR.color = card.color;
        _tSR.sortingOrder = 1;
        _tGO.transform.SetParent(card.transform);
        _tGO.transform.localScale = Vector3.one * 1;
        _tGO.transform.localPosition = Vector3.zero;
        _tGO.name = "face";
    }

    private Sprite GetFace (string faceS) {
        foreach (Sprite _tSP in faceSprites)
        {
            if (_tSP.name == faceS) {
                return ( _tSP );
            }
        }
        return null;
    }

    private void AddBack (Card card) {
        _tGO = Instantiate(prefabSprite) as GameObject;
        _tSR = _tGO.GetComponent<SpriteRenderer> ();
        _tSR.sprite = cardBack;
        _tGO.transform.SetParent(card.transform);
        _tGO.transform.localScale = Vector3.one;
        _tGO.transform.localPosition = Vector3.zero;
        _tSR.sortingOrder = 2;
        _tGO.name ="back";
        card.back = _tGO;

        card.faceUp = startFaceUp;
    }

    static public void Shuffle(ref List<Card> oCards) {
        List<Card> tCards = new List<Card>();
        int ndx;
        tCards = new List<Card>();

        while (oCards.Count > 0) {
            ndx = Random.Range(0, oCards.Count);
            tCards.Add(oCards[ndx]);
            oCards.RemoveAt(ndx);
        }
        oCards = tCards;
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

[System.Serializable]
public class DeckItem {
  public Decorator[] decorator;
  public CardDefinition[] card;
}