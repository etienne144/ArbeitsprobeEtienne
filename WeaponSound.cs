using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSound : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSourcePlayOneShot;
    [SerializeField] private AudioSource _audioSourceHoldSoundEffect;
    private WeaponSettings _weaponSettings;

    private void Start()
    {
        _weaponSettings = GetComponent<WeaponSettings>();
    }
    private IEnumerator WeaponSoundEffects()
    {
        _audioSourcePlayOneShot.PlayOneShot(_weaponSettings.WeaponSoundSO.fireClip, _weaponSettings.WeaponSoundSO.fireVolume);
        if (_weaponSettings.WeaponSoundSO.reloadClip != null)
        {
            yield return new WaitForSeconds(_weaponSettings.WeaponSoundSO.reloadDelay);
            _audioSourcePlayOneShot.PlayOneShot(_weaponSettings.WeaponSoundSO.reloadClip, _weaponSettings.WeaponSoundSO.reloadVolume);
        }
    }
    public void PlayHoldSound()
    {
        if (!_audioSourceHoldSoundEffect.isPlaying)
        {
            _audioSourceHoldSoundEffect.time = 0f;
            _audioSourceHoldSoundEffect.clip = _weaponSettings.WeaponSoundSO.holdClip;
            _audioSourceHoldSoundEffect.volume = _weaponSettings.WeaponSoundSO.holdVolume;
            _audioSourceHoldSoundEffect.Play();
        }
    }
    public void StopHoldSound()
    {
        if (_weaponSettings.ShootOnRelease)
            _audioSourceHoldSoundEffect.Stop();
    }
    public void PlayShootingSoundEffects()
    {
        StartCoroutine(WeaponSoundEffects());
    }
}
