using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private float meteorRadius, meteorDelay;

    [SerializeField] private Image meteorImage;

    [Header("Particles Effects")]
    [SerializeField] private GameObject impact;
    [SerializeField] private GameObject meteor;
    [SerializeField] private GameObject fire;

    [Header("Attacks Damage")] 
    [SerializeField] private int impactDamage;
    [SerializeField] private int meteorDamage;
    [SerializeField] private int fireDamage;
    
    [Header("Attacks Cooldown")]
    public float meteorCooldown;
    public float fireCooldown;
    
    [Header("Sounds")]
    [SerializeField] private AudioSource meteorAudioSource;
    [SerializeField] private AudioSource flameAudioSource;

    [Header("Camera Shake")] 
    [SerializeField] private AnimationCurve curve;
    [SerializeField] private float duration;

    [Header("Selected Attack Images")] 
    [SerializeField] private GameObject selectedImpactImage;
    [SerializeField] private GameObject selectedMeteorImage;
    [SerializeField] private GameObject selectedFireImage;

    private Camera _mainCamera;

    private const string DinosaurTag = "Dinosaur";

    private float _meteorCooldownTimer, _fireCooldownTimer;

    private bool _canMeteor = true, _canFire = true;
    
    private Vector3 _mousePosition;

    private enum PlayerActions { Basic, Meteor, Fire }

    private PlayerActions _selectedAttack = PlayerActions.Basic;
    
    private void Awake() => _mainCamera = GetComponent<Camera>();

    private void Update()
    {
        if (UIManager.GameFinished || !UIManager.GameStarted)
        {
            flameAudioSource.Stop();
            return;
        }

        DetectMouseClick();
        ChooseAttack();
        
        Cooldown(ref _canMeteor ,ref _meteorCooldownTimer, meteorCooldown);
        Cooldown(ref _canFire ,ref _fireCooldownTimer, fireCooldown);
    }

    private void DetectMouseClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (_selectedAttack == PlayerActions.Basic)
                Attack(impact, impactDamage);
            else if (_selectedAttack == PlayerActions.Meteor && _canMeteor)
            {
                _canMeteor = false;
                meteorImage.fillAmount = 0f;
                
                meteorAudioSource.Play();
                
                _mousePosition = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
                Invoke(nameof(MeteorAttack), meteorDelay);
            }
        }
        else if (Input.GetMouseButton(0) && _selectedAttack == PlayerActions.Fire && _canFire)
        {
            _canFire = false;

            if(!flameAudioSource.isPlaying)
                flameAudioSource.Play();
            
            Attack(fire, fireDamage);
        }
        else if(_canFire)
        {
            flameAudioSource.Stop();
        }
    }
    
    private static void Cooldown(ref bool canAttack,ref float attackCooldownTimer, float attackCooldown)
    {
        if (canAttack) return;
        
        attackCooldownTimer += Time.deltaTime;

        if (attackCooldownTimer < attackCooldown) return;
        
        attackCooldownTimer = 0f;
        canAttack = true;
    }

    #region Attacks
    private void Attack(GameObject effect, int damage)
    {
        var mousePosition = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
        var hit = Physics2D.Raycast(mousePosition, Vector2.zero);

        var mousePosition2D = new Vector3(mousePosition.x, mousePosition.y, -6f);
        Instantiate(effect, mousePosition2D, quaternion.identity);
        
        if (hit.transform.CompareTag(DinosaurTag))
            hit.transform.GetComponent<DinosaursManager>().Damage -= damage;
    }
    
    private void MeteorAttack()
    {
        StartCoroutine(ShakingEffect());
        
        var hits = Physics2D.CircleCastAll(_mousePosition, meteorRadius, Vector2.zero);
        
        var mousePosition2D = new Vector3(_mousePosition.x, _mousePosition.y, -6f);
        Instantiate(meteor, mousePosition2D, quaternion.identity);

        foreach (var hit in hits)
        {
            if (hit.transform.CompareTag(DinosaurTag)) 
                hit.transform.GetComponent<DinosaursManager>().DamageMeteor -= meteorDamage;
        }
    }
    #endregion

    private void ChooseAttack()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            _selectedAttack = PlayerActions.Basic;
            SelectedAttackUI(true, false, false);
        }
        else if (Input.GetKeyDown(KeyCode.Z))
        {
            _selectedAttack = PlayerActions.Meteor;
            SelectedAttackUI(false, true, false);
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            _selectedAttack = PlayerActions.Fire;
            SelectedAttackUI(false, false, true);
        }
    }

    private void SelectedAttackUI(bool impactBool, bool meteorBool, bool fireBool)
    {
        selectedImpactImage.SetActive(impactBool);
        selectedMeteorImage.SetActive(meteorBool);
        selectedFireImage.SetActive(fireBool);
    }

    private IEnumerator ShakingEffect()
    {
        var startPosition = transform.position;
        var elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            var strength = curve.Evaluate(elapsedTime / duration);

            transform.position = startPosition + Random.insideUnitSphere * strength;
            yield return null;
        }

        transform.position = startPosition;
    }
}