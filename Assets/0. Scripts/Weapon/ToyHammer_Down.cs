using UnityEngine;
public class ToyHammer_Down : Weapon
{
    protected override void AttackEvent(Actor _actor)
    {
        base.AttackEvent(_actor);
        GameManager.instance.MainCamCtrl.ShakeCamera(.2f, .4f);
    }

    [SerializeField]
    ParticleSystem prtc;

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
        AudioPlay();
        prtc.transform.position = collision.transform.position;
        prtc.Play();
    }
}
