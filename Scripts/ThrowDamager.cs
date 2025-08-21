using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ThrowDamager : MonoBehaviour
{
    public enum ShotType
    {
        HIGHEST_SHOT,
        LOWEST_SPEED,
        MOST_DIRECT
    }

    private Transform player; // 플레이어 transform
    private Vector3 _targetPosition;
    
    [SerializeField] private ShotType shotType;
    [SerializeField] private float throwingSpeed = 20;   //투척 최대 속도
    [SerializeField] private int damageAmount = 3;    //데미지
    [SerializeField] private float explosionRadius = 1f;   //폭발범위(반지름)
    
    [Tooltip("폭발 시 데미지를 입힐 레이어를 선택하세요.")]
    [SerializeField] private LayerMask damageMask; 
    
    [Tooltip("n초후에 폭발하도록 지정하고 싶은 경우에만 설정하세요. (기본값:-1)")]
    [SerializeField] private float explosionTime = -1;   //폭발시간
    
    [Tooltip("투척물이 땅에 닿거나 폭발할 경우에 재생될 VFX를 연결하세요.")]
    [SerializeField] private ParticleSystem explosionVFX;
    
    [Tooltip("지면에 VFX를 재생하고 싶은 경우 활성화하세요.")]
    [SerializeField] private bool vfxOnGround = true;
    

    private float timeSinceFired = 0.0f;
    private Rigidbody _rigidbody;
    private ParticleSystem vfxInstance;
    private int environmentLayer = -1;
    private AudioSource _audioSource;
    public AudioClip _clip;
    
    //투척물에 맞은 객체 수를 최대 32개까지만 저장
    private Collider[] explosionHitCache = new Collider[32];

    void Awake()
    {
        environmentLayer = 1 << LayerMask.NameToLayer("groundMask"); //레이어 인덱스만큼 비트시프트하여 레이어의 비트마스크 값을 저장.
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.detectCollisions = false;

        vfxInstance = Instantiate(explosionVFX);
        vfxInstance.gameObject.SetActive(false);
        
        _audioSource = GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        _targetPosition = player.position; //플레이어를 향하도록 타겟 위치를 갱신 
        
        Throw(_targetPosition);
    }

    private void OnEnable()
    {
        timeSinceFired = 0.0f;
    }
    public void Throw(Vector3 target)
    {
        _rigidbody.velocity = GetVelocity(target);
        //_rigidbody.AddRelativeTorque(Vector3.right * -5500.0f);
        
        //충돌 감지를 잠시 꺼두는 이유 : 발사할때 닿아도 바로 반응할 수 있기 때문..
        _rigidbody.detectCollisions = false;
        
        transform.forward = target - transform.position;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        timeSinceFired+=Time.deltaTime;
        if (timeSinceFired > 0.2f)
        {
            //0.2초 후에야 충돌을 활성화하여 본체로부터 떨어져나올 수 있는 시간을 확보.
            _rigidbody.detectCollisions = true;
        }
        
        if (explosionTime > 0 && timeSinceFired > explosionTime)
        {
            Explosion();
        }
    }

    public void Explosion()
    {
        Debug.Log("Explode");
        if (_audioSource != null)
        {
            Debug.Log("Sound Play");
            _audioSource.Play();
        }

        int count = Physics.OverlapSphereNonAlloc(transform.position, explosionRadius, explosionHitCache,
            damageMask.value);
        
        

        Damageable.DamageMessage message = new Damageable.DamageMessage
        {
            amount = damageAmount,  //데미지양
            damageSource = transform.position,  //폭발 위치
            damager = this,
            stopCamera = false,
            throwing = true
        };

        for (int i = 0; i < count; ++i)
        {
            Damageable d = explosionHitCache[i].GetComponentInChildren<Damageable>();

            if (d != null)
                d.ApplyDamage(message);
            
            //UnderAttack
            if (explosionHitCache[i].gameObject.TryGetComponent(out IHealthpower hp))
            {
                hp.UnderAttack(damageAmount);
                _audioSource.PlayOneShot(_clip);
                //Debug.Log(explosionHitCache[i].name+" Damaged -"+damageAmount+" , 현재체력 : "+hp.CurrentHealthPower);
            }
        }
        
        Vector3 playPosition = transform.position;
        Vector3 playNormal = Vector3.up;
        if (vfxOnGround)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, 100.0f, environmentLayer))
            {
                playPosition = hit.point + hit.normal * 0.1f;
                playNormal = hit.normal;
            }
        }
        
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.enabled = false;
        Collider collider = GetComponent<Collider>();
        collider.enabled = false;
        
        vfxInstance.gameObject.transform.position = playPosition;
        vfxInstance.gameObject.transform.up = playNormal;
        vfxInstance.time = 0.0f;
        vfxInstance.gameObject.SetActive(true);
        vfxInstance.Play(true);
        
        Destroy(vfxInstance, 3f);
        Destroy(gameObject, 3f);  // 3초 후에 현재 오브젝트(self)를 삭제
    }

    void OnCollisionEnter(Collision other)
    {
        if(explosionTime < 0)
            Explosion();
    }

    //target지점에 포물선 경로로 도달하기 위한 발사속도 계산하기.
    private Vector3 GetVelocity(Vector3 target)
    {
        Vector3 velocity = Vector3.zero;
        Vector3 throwDirection = target - transform.position; //목표지점과 현위치의 벡터 측정
        
        float gSquared = Physics.gravity.sqrMagnitude;  //중력벡터 제곱
        
        //발사체의 최대 속도 제곱과 목표 지점까지의 거리 벡터와 중력 벡터의 내적을 합친 값.
        float b = throwingSpeed * throwingSpeed + Vector3.Dot(throwDirection, Physics.gravity);
        
        //2차방정식의 판별식. (discriminant가 0 이상일때만 목표에 도달하는 실근이 존재.)
        float discriminant = b * b - gSquared * throwDirection.sqrMagnitude;
        
        
        if (discriminant < 0)
        {
            //정해진 shootingSpeed로는 목표에 도달할 수 없다. 따라서 임의의 velocity를 리턴한다.
            // Debug.Log("Can't reach");
            velocity = throwDirection;
            velocity.y = 0;
            velocity.Normalize();
            velocity.y = 0.7f;

            velocity *= throwingSpeed;
            return velocity;
        }
        
        float discRoot = Mathf.Sqrt(discriminant);

        // 최대 속도로 목표 지점에 도달할 때의 시간. 즉, 가장 높은 발사각도로 쏠 때 걸리는 시간
        float T_max = Mathf.Sqrt((b + discRoot) * 2f / gSquared); 
        
        // 가장 직선 경로로 목표에 도달할 때의 시간. 즉, 가장 낮은 발사각도로 쏠 때 걸리는 시간
        float T_min = Mathf.Sqrt((b - discRoot) * 2f / gSquared);

        // 가장 낮은 속도로 목표에 도달할 때의 시간. 이 경로는 더 많은 에너지를 소모할 수 있습니다.
        float T_lowEnergy = Mathf.Sqrt(Mathf.Sqrt(throwDirection.sqrMagnitude * 4f / gSquared));

        float T = 0;
        // choose T_max, T_min, or some T in-between like T_lowEnergy
        
        //사용자가 설정한 발사 방식에 따라 발사 시간을 선택합니다.
        switch (shotType)
        {
            case ShotType.HIGHEST_SHOT:
                T = T_max;
                break;
            case ShotType.LOWEST_SPEED:
                T = T_lowEnergy;
                break;
            case ShotType.MOST_DIRECT:
                T = T_min;
                break;
            default:
                break;
        }
        
        //목표지점에 도달하기 위한 발사속도 계산.
        velocity = throwDirection / T - Physics.gravity * T / 2f;
        //목표까지의 거리를 주어진 시간(T)에 맞춰 나누어, 단위 시간당 이동해야 하는 속도를 구합니다.
        //추가적으로, 중력에 의한 영향을 고려하여 하강속도를 보정합니다.
        
        return velocity;
    }
}
