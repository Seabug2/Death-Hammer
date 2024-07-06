using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainSaw : Weapon
{
    [SerializeField]
    ParticleSystem blood;

    protected override void AttackEvent(Actor _actor)
    {
        base.AttackEvent(_actor);
        GameManager.instance.MainCamCtrl.ShakeCamera(.2f, 1);
        AudioPlay();
        blood.Play();
    }
}
