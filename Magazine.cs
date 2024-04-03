using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Magazine : MonoBehaviour
{
    private WeaponSettings _weaponSettings;
    private Weapon _weapon;
    private WeaponSound _weaponSound;
    private Coroutine _reloadBulletsCoroutine;

    public float HoldTime;
    public int MultiBulletCounter;
    public int SalvenCounter;
    public int BulletsInMagazine;

    private void Start()
    {
        _weaponSettings = GetComponent<WeaponSettings>();
        _weapon = GetComponent<Weapon>();
        _weaponSound = GetComponent<WeaponSound>();
    }
    public void ShootOnRelease(bool mouseButton, bool enableWeapon, bool isLeft, bool isCalledAsClient)
    {
        if (mouseButton && CanFire(enableWeapon))
        {
            if (!NetworkManager.Singleton.IsServer || !isCalledAsClient) UpdateHoldTimer();//Make sure that the code on the server is executed only once.
            if (FireIfCharged())
                _weapon.FireRequest(isLeft, isCalledAsClient);
        }
        if (WeaponIsReleased(mouseButton, enableWeapon))//checks whether the _weapon has been charged long enough and the mousbutton has now been released
            _weapon.FireRequest(isLeft, isCalledAsClient);
        if (mouseButton == false || !CanFire(enableWeapon)) //resets the hold timer if the _weapon has been used, overheated or even deactivated
            if (!NetworkManager.Singleton.IsServer || !isCalledAsClient) HoldTime = 0;//Make sure that the code on the server is executed only once.
    }
    public void UpdateMagazine()
    {
        if (_weaponSettings.SlowReload)
        {
            Reload();
            BulletsInMagazine--;
        }
        if (_weaponSettings.ShootOnRelease && (_weaponSettings.Salven || _weaponSettings.MultiBullet))
            SalvenCounter = IncreaseBulletCountOverTime(); //For salven, shotguns that increase by holding
    }
    private void Reload()
    {
        if (_reloadBulletsCoroutine != null)
        {
            StopCoroutine(_reloadBulletsCoroutine);
            _reloadBulletsCoroutine = null;
        }
        _reloadBulletsCoroutine = StartCoroutine(ReloadBullets());
    }
    private IEnumerator ReloadBullets()
    {
        yield return new WaitForSeconds(_weaponSettings.ReloadTime);
        WaitForSeconds timeToWait = new WaitForSeconds(_weaponSettings.ReloadTime);
        while (BulletsInMagazine < _weaponSettings.MaxBullets)
        {
            BulletsInMagazine++;
            yield return timeToWait;
        }
        _reloadBulletsCoroutine = null;
    }
    private int IncreaseBulletCountOverTime()//with holdIncreaseWeapons, the bullet counter is increased the longer the weapon is charged
    {
        int bulletCounter = 0;
        for (int i = 0; i < _weaponSettings.BulletCountIncreaseTimes.Length; i++)
            if (_weaponSettings.BulletCountIncreaseTimes[i] < HoldTime)
                bulletCounter++;
        return bulletCounter;
    }
    private void UpdateHoldTimer()
    {
        HoldTime += Time.fixedDeltaTime;
        _weaponSound.PlayHoldSound();
    }
    private bool FireIfCharged()
    {
        if (_weaponSettings.FireWhenFullyCharged == true && HoldTime > _weaponSettings.MindHoldTime)
            return true;
        else return false;
    }
    private bool CanFire(bool enableWeapon)
    {
        if (enableWeapon && _weapon.CanFire && !_weapon.Heat.isOverheated)
            return true;
        else return false;
    }
    private bool WeaponIsReleased(bool mouseButton, bool enableWeapon)
    {
        if (mouseButton && enableWeapon && HoldTime > _weaponSettings.MindHoldTime)
            return true;
        else return false;
    }
}
