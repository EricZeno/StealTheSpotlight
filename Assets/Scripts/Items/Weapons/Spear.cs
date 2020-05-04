using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Spear", menuName = "Items/Weapons/Spear")]
public class Spear : BaseWeaponItem {
    public override string PlayAudio()
    {
        return "Light Hit 1";
    }
}
