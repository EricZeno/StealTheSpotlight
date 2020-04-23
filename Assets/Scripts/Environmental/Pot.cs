using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pot : MonoBehaviour {
    #region Editor Variables
    [SerializeField]
    private Sprite[] brokenshards;
    #endregion

    #region Pot
    public void Break() {
        gameObject.GetComponent<BoxCollider2D>().enabled = false;
        gameObject.GetComponent<SpriteRenderer>().sprite = brokenshards[Random.Range(0, brokenshards.Length)];
    }
    #endregion
}
