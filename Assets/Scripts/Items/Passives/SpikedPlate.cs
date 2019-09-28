using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpikedPlate", menuName = "Items/Passives/SpikedPlate")]
public class SpikedPlate : BasePassiveItem {
    public override void ApplyEffect(PlayerManager player) {
        Debug.Log("Effect applied!");
    }

    public override void RemoveEffect(PlayerManager player) {
        Debug.Log("Effect removed!");
    }
}
