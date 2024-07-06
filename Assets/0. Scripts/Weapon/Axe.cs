using UnityEngine;
public class Axe : Weapon
{
    [SerializeField]
    ParticleSystem prtc;

    protected override void AttackEvent(Actor _actor)
    {
        base.AttackEvent(_actor);
        GameManager.instance.MainCamCtrl.ShakeCamera(.3f, .6f);
    }
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
        AudioPlay();
        prtc.Play();
    }
}
