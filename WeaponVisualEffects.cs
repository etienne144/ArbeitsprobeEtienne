using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponVisualEffects : MonoBehaviour
{
    private const float RESET_SHOOTING_EFFECT_MULTIBULLET_DISCREPANCY = 0.02f;
    private const float MUZZLEFLASH_DURATION = 1f;

    private bool _multiBulletFirstEffect = true;
    private int _currentMuzzleflashIndex;
    private WeaponSettings _weaponSettings;
    private Magazine _magazine;
    private WeaponSound _weaponSound;
    private Weapon _weapon;

    public GameObject[] Muzzleflashes;

    private void Start()
    {
        _weaponSettings = GetComponent<WeaponSettings>();
        _magazine = GetComponent<Magazine>();
        _weaponSound = GetComponent<WeaponSound>();
        _weapon = GetComponent<Weapon>();
    }
    public void ShootingEffectOtherPlayer()
    {
        MuzzleflashEffectMultiCheck(false);
    }
    private IEnumerator ShootingEffectLocal()
    {
        int firedBulletsCounter = 0;
        yield return new WaitForSeconds(0f);
        WaitForSeconds timeToWait = new WaitForSeconds(_weaponSettings.SalvenTick);
        while (firedBulletsCounter <= _magazine.SalvenCounter)
        {
            MuzzleflashEffectMultiCheck(true);
            if (_weaponSettings.IsMinion) RecoilLocalPlayer();
            firedBulletsCounter++;
            yield return timeToWait;
        }
    }
    private void MuzzleflashEffectMultiCheck(bool isLocal)
    {
        if (_weaponSettings.MultiBullet)
        {
            MuzzleflashEffect();
        }
        else if (_multiBulletFirstEffect) //MultiBulletWeapons aka shotguns should just have one muzzleflash even though it spawns more than one bullet.
        {
            _multiBulletFirstEffect = false;
            MuzzleflashEffect();
            if(!isLocal)RecoilOtherPlayer();
            Invoke("ResetMultiBulletFirstEffect", _weapon.FireRate - RESET_SHOOTING_EFFECT_MULTIBULLET_DISCREPANCY);
        }
    }
    public void MuzzleflashEffect()
    {
        _weaponSound.PlayShootingSoundEffects();
        Muzzleflashes[_currentMuzzleflashIndex].SetActive(true);
        _currentMuzzleflashIndex = (_currentMuzzleflashIndex + 1) % Muzzleflashes.Length;
        StartCoroutine(HideMuzzleflashEffectWithDelay(_currentMuzzleflashIndex));
    }
    private IEnumerator HideMuzzleflashEffectWithDelay(int muzzleflashIndex)
    {
        yield return new WaitForSeconds(MUZZLEFLASH_DURATION);
        Muzzleflashes[muzzleflashIndex].SetActive(false);
    }
    public void RecoilLocalPlayer()
    {
        _weapon.Player.cameraRecoil.Fire(_weaponSettings.CameraRecoilAmount);
        _weapon.Player.playerController.localAnimations.Fire(_weaponSettings.IsLeft, _weaponSettings.RecoilRotation, _weaponSettings.RecoilKickBack);
    }
    public void RecoilOtherPlayer()
    {
        _weapon.Recoil.Fire(_weaponSettings.RecoilMagnitude);
    }
    public void StartCoroutineShootingEffectLocal()
    {
        StartCoroutine(ShootingEffectLocal());
    }
    private void ResetMultiBulletFirstEffect()
    {
        _multiBulletFirstEffect = true;
    }
}
