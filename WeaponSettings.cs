using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSettings : MonoBehaviour
{
    [Header("BasicInfos")]
    public bool IsLeft;
    public bool IsMinion;
    public int BulletIndex;
    public float FireRate;
    public float HeatProduction;
    public float BulletSpeed;
    public WeaponSoundSO WeaponSoundSO;

    [Header("Spray")]
    public bool SprayOnlyX;
    public float SprayAmount;

    [Header("Recoil")]
    public float CameraRecoilAmount;
    public Vector3 RecoilRotation;
    public Vector3 RecoilKickBack;
    public float RecoilMagnitude;

    [Header("Weapon modification Salven")]
    public bool Salven;
    public float SalvenTick;

    [Header("Weapon modification multiBullet")]
    public bool MultiBullet; //aka Shotguns

    [Header("Weapon modification slowReload")]
    public bool SlowReload;
    public float ReloadTime = 3;
    public int MaxBullets;

    [Header("Weapon modification shootOnRelease")]
    public bool ShootOnRelease;
    public float MindHoldTime;

    [Header("Weapon modification bulletsIncreaseWithHold")]
    public bool BulletCounterIncreaseWithHold;
    public float[] BulletCountIncreaseTimes;

    [Header("Weapon modification shootAfterMaxIncreaseTime")]
    public bool FireWhenFullyCharged;
}
