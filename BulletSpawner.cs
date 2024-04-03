using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BulletSpawner
{
    private const float SERVER_DELAY_CORRECTION = 0.03f;

    public static void BulletSpawnerController(Weapon weapon, bool isLeft)
    {
        Quaternion spawnRotation = CalculateSpawnRotation(weapon);
        Vector3 spawnPos = CalculateSpawnBulletPosition(spawnRotation, weapon);
        if (weapon.WeaponSettings.IsMinion == false) weapon.Player.FireClientRpc(isLeft, spawnPos, spawnRotation);
        else weapon.MinionBrain.FireClientRpc(spawnPos, spawnRotation);
        SpawnBullet(spawnPos, spawnRotation, weapon);
    }
    private static Quaternion CalculateSpawnRotation(Weapon weapon)
    {
        Quaternion spawnRotation = new Quaternion(0, 0, 0, 0);
        if (weapon.WeaponSettings.IsMinion == false)
        {
            Quaternion spawnRot = weapon.Player.animationManager.top.rotation;
            float newHeadRot = weapon.Player.headRot;
            spawnRotation = Quaternion.Euler(newHeadRot, spawnRot.eulerAngles.y, spawnRot.eulerAngles.z);
        }
        else
        {
            Vector3 forwardDirection = weapon.MinionBrain.top.forward;
            spawnRotation = Quaternion.LookRotation(forwardDirection);
        }
        spawnRotation *= CalculateRandomBulletOffset(weapon);
        return spawnRotation;
    }
    private static Quaternion CalculateRandomBulletOffset(Weapon weapon)
    {
        float randomX = 0;
        if (weapon.WeaponSettings.SprayOnlyX == false)
            randomX = Random.Range(-weapon.WeaponSettings.SprayAmount, weapon.WeaponSettings.SprayAmount);
        float randomY = Random.Range(-weapon.WeaponSettings.SprayAmount, weapon.WeaponSettings.SprayAmount);
        float randomZ = Random.Range(-weapon.WeaponSettings.SprayAmount, weapon.WeaponSettings.SprayAmount);
        Quaternion randomBulletOffset = Quaternion.Euler(randomX, randomY, randomZ);// Erzeuge ein zufälliges Euler-Winkel-Offset
        return randomBulletOffset;
    }
    private static Vector3 CalculateSpawnBulletPosition(Quaternion rotation, Weapon weapon)
    {
        Vector3 gunPoint = weapon.transform.position;
        if (weapon.WeaponSettings.IsMinion == true) 
            gunPoint = weapon.MinionBrain.gunPoint.transform.position;
        Vector3 velocity = rotation * Vector3.forward * weapon.WeaponSettings.BulletSpeed;
        Vector3 spawnPosition = gunPoint + velocity * SERVER_DELAY_CORRECTION;//calculates to the position of the bullet, which it reaches after 0.03 seconds.
        return spawnPosition; //This position is delivered to the clients and is intended to compensate for the delay
    }
    public static void SpawnBullet(Vector3 bulletPosition, Quaternion bulletRotation, Weapon weapon)
    {
        GameObject bulletGO = ObjectPooler.SharedInstance.GetPooledObject(weapon.WeaponSettings.BulletIndex);
        bulletGO.transform.position = bulletPosition;
        bulletGO.transform.rotation = bulletRotation;
        bulletGO.SetActive(true);
        var bullet = bulletGO.GetComponent<Bullet>();
        bullet.isLocal = IsLocalBulletCheck(weapon);
        bullet.StartPos();
    }
    private static bool IsLocalBulletCheck(Weapon weapon)
    {
        bool isLocal = false;
        if (weapon.WeaponSettings.IsMinion == false)
            isLocal = weapon.Player.localPlayerBool;
        return isLocal;
    }
}
