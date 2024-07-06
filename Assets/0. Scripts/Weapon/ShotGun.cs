using UnityEngine;

public class ShotGun : Weapon
{
    [SerializeField]
    Vector2 gunRecoil;

    [SerializeField]
    float shakeTime, shakeSize = 1;

    public void Recoil()
    {
        Controler.KnockBack(gunRecoil);
        GameManager.instance.MainCamCtrl.ShakeCamera(shakeTime, shakeSize);
    }

    [SerializeField]
    ParticleSystem gunFirePrtc;

    public void Fire()
    {
        AudioPlay();
        gunFirePrtc.Play();
    }
}
