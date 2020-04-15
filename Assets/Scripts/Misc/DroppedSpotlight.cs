using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppedSpotlight : MonoBehaviour
{
    #region Events and Delegates
    public delegate void PickUpSpotlight(int player);
    public static event PickUpSpotlight PickUpSpotlightEvent;
    #endregion

    #region Collision
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag(Consts.PLAYER_TAG)) {
            PickUpSpotlightEvent(collision.GetComponent<PlayerManager>().GetID());
        }
    }
    #endregion
}
