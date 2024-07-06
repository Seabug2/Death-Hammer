using UnityEngine;
public class ToyHammer_Swing : Weapon
{

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