using UnityEngine;

public class WoodHammer : Weapon
{
    [SerializeField]
    float shakeTime, shakeSize = 1;

    protected override void AttackEvent(Actor _actor)
    {
        base.AttackEvent(_actor);
    }

    [SerializeField]
    ParticleSystem prtc;

    public void Hitting()
    {
        GameManager.instance.MainCamCtrl.ShakeCamera(shakeTime, shakeSize);
        prtc.Play();
        AudioPlay();
    }
}
