using UnityEngine;
using UnityEngine.Serialization;

public class AudioManager : SceneSingleton<AudioManager>
{
    [SerializeField] private AudioClip btnClip;
    [SerializeField] private AudioClip btnClickClip;
    [SerializeField] private AudioClip footStepClip;
    [SerializeField] private AudioClip jumpClip;
    [SerializeField] private AudioClip swingClip;
    [SerializeField] private AudioClip punchClip;
    [SerializeField] private AudioClip swipKnifeClip;
    [SerializeField] private AudioClip knifeClip;
    [SerializeField] private AudioClip axeClip;
    [SerializeField] private AudioClip zombieStepClip;
    [SerializeField] private AudioClip zombieAttackClip;

    [SerializeField] private AudioClip OnUiWindowClip;
    [SerializeField] private AudioClip OffUiWindowClip;
    [SerializeField] private AudioClip craftClip;

    [SerializeField] private AudioClip bossSpawnClip;
    [SerializeField] private AudioClip bossSmashClip;
    [SerializeField] private AudioClip bossFireClip;
    [SerializeField] private AudioClip bossPoisonClip;
    [SerializeField] private AudioClip bossDiedClip;

    [SerializeField] private AudioClip gunRifleFire;
    [SerializeField] private AudioClip gunRifleReload;
    [SerializeField] private AudioClip gunShotGunFire;
    [SerializeField] private AudioClip gunShotGunReload;
    
    private AudioSource _audioSource;

    protected override void Awake()
    {
        base.Awake();
        _audioSource = GetComponent<AudioSource>();
        if (!_audioSource)
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
            Debug.Log("New AudioSource Added");
        }
    }
    
    // 이하 클립재생
    public void PlayButtonSound()
    {
        _audioSource.PlayOneShot(btnClip);
    }
    public void PlayClickSound()
    {
        _audioSource.PlayOneShot(btnClickClip);
    }
    public void PlayFootStepSound()
    {
        _audioSource.PlayOneShot(footStepClip);
    }
    public void PlayJumpSound()
    {
        _audioSource.PlayOneShot(jumpClip);
    }
    public void PlaySwingSound()
    {
        _audioSource.PlayOneShot(swingClip);
    }
    public void PlayPunchSound()
    {
        _audioSource.PlayOneShot(punchClip);
    }
    public void PlayKnifeSound()
    {
        _audioSource.PlayOneShot(knifeClip);
    }
    public void PlaySwingKnifeSound()
    {
        _audioSource.PlayOneShot(swipKnifeClip);
    }
    public void PlayAxeSound()
    {
        _audioSource.PlayOneShot(axeClip);
    }
    public void PlayZombieStepSound()
    {
        _audioSource.PlayOneShot(zombieStepClip);
    }
    public void PlayZombieAttackSound()
    {
        _audioSource.PlayOneShot(zombieAttackClip);
    }
    public void ClickButtonOnWindow()
    {
        _audioSource.PlayOneShot(OnUiWindowClip);
    }
    public void ClickButtonOffWindow()
    {
        _audioSource.PlayOneShot(OffUiWindowClip);
    }

    public void ClickCraft()
    {
        _audioSource.PlayOneShot(craftClip);
    }

    public void BossSpawn()
    {
        _audioSource.PlayOneShot(bossSpawnClip);
    }
    
    public void BossSmash()
    {
        _audioSource.PlayOneShot(bossSmashClip);
    }
    
    public void BossFire()
    {
        _audioSource.PlayOneShot(bossFireClip);
    }
    
    public void BossPoison()
    {
        _audioSource.PlayOneShot(bossPoisonClip);
    }

    public void BossDied()
    {
        _audioSource.PlayOneShot(bossDiedClip);
    }

    public void GunRifleFire()
    {
        _audioSource.PlayOneShot(gunRifleFire);
    }
    
    public void GunRifleReload()
    {
        _audioSource.PlayOneShot(gunRifleReload);
    }
    
    public void GunShotGunFire()
    {
        _audioSource.PlayOneShot(gunShotGunFire);
    }
    
    public void GunShotGunReload()
    {
        _audioSource.PlayOneShot(gunShotGunReload);
    }
    
    
}
