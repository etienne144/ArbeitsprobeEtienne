using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using RootMotion.FinalIK;

public class Weapon : MonoBehaviour
{   
    private WeaponSound _weaponSound;

    public float FireRate;
    public bool CanFire = true;

    public Player Player;
    public Heat Heat;
    public WeaponSettings WeaponSettings;
    public Magazine Magazine;
    public WeaponVisualEffects WeaponVisualEffects;   
    public Recoil Recoil;
    public MinionBrain MinionBrain;

    private void Start()
    {
        WeaponSettings = GetComponent<WeaponSettings>();
        Magazine = GetComponent<Magazine>();
        _weaponSound = GetComponent<WeaponSound>();
        WeaponVisualEffects = GetComponent<WeaponVisualEffects>();
    }
    public void FireRequest(bool isLeft, bool isCalledAsClient)
    {
        if (CantFire()) return;
        if (!NetworkManager.Singleton.IsServer || !isCalledAsClient)//Make sure that the code on the server is executed only once. Server is a Client aswell (Host)
        {
            UpdateWeaponState();
            Invoke("EnableFiring", FireRate);
        }
        if (isCalledAsClient)
        {
            _weaponSound.StopHoldSound();
            WeaponVisualEffects.StartCoroutineShootingEffectLocal();
        }
        else //isCalledAsServer
            StartCoroutine(FireServerCode(isLeft));
    }
    private void UpdateWeaponState()
    {
        CanFire = false;
        if (!WeaponSettings.IsMinion)
            Heat.UpdateHeat(WeaponSettings.HeatProduction);
        Magazine.UpdateMagazine();
    }
    private IEnumerator FireServerCode(bool isLeft)
    {
        int bulletCount = 0;
        WaitForSeconds timeToWait = new WaitForSeconds(WeaponSettings.SalvenTick);
        while (bulletCount <= Magazine.SalvenCounter) //salven
        {
            WeaponVisualEffects.RecoilOtherPlayer();
            for (int i = 0; i <= Magazine.MultiBulletCounter; i++) //multiShoot
            {
                BulletSpawner.BulletSpawnerController(this, isLeft);
            }
            bulletCount++;
            yield return timeToWait;
        }
    }
    //If a minion or another player has shot, the player receives this information and can start the visual representation of it
    public void FireClientReciverMinion(Vector3 BulletPos, Quaternion BulletRot)
    {
        BulletSpawner.SpawnBullet(BulletPos, BulletRot, this);
        WeaponVisualEffects.MuzzleflashEffect();
        WeaponVisualEffects.RecoilOtherPlayer();
    }
    public void FireClientReciver(Vector3 BulletPos, Quaternion BulletRot)
    {
        BulletSpawner.SpawnBullet(BulletPos, BulletRot, this);
        if (!Player.localPlayerBool)
            WeaponVisualEffects.ShootingEffectOtherPlayer();
    }
    private void EnableFiring()
    {
        CanFire = true;
    }
    private bool CantFire()
    {
        if (!CanFire || (WeaponSettings.SlowReload && Magazine.BulletsInMagazine == 0) || Heat.isOverheated)
            return true;
        else return false;
    }
}
