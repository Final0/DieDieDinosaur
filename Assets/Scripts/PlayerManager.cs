using Unity.Mathematics;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private float meteorRadius;
    [SerializeField] private float meteorDelay;

    [Header("Particles Effects")]
    [SerializeField] private GameObject impact;
    [SerializeField] private GameObject meteor;
    [SerializeField] private GameObject fire;

    [Header("Attacks Damage")] 
    [SerializeField] private int impactDamage;
    [SerializeField] private int meteorDamage;
    [SerializeField] private int fireDamage;
    
    [Header("Attacks Cooldown")]
    [SerializeField] private float meteorCooldown;
    [SerializeField] private float fireCooldown;
    
    private Camera _mainCamera;

    private const string DinosaurTag = "Dinosaur";

    private float _meteorCooldownTimer, _fireCooldownTimer;

    private bool _canMeteor = true, _canFire = true;

    private enum PlayerActions
    {
        Basic,
        Meteor,
        Fire
    }

    private PlayerActions _selectedAttack = PlayerActions.Basic;
    
    private void Awake() => _mainCamera = GetComponent<Camera>();

    private void Update()
    {
        DetectMouseClick();
        ChooseAttack();
        MeteorCooldown();
        FireCooldown();
    }

    private void DetectMouseClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (_selectedAttack == PlayerActions.Basic) BasicAttack();
            else if (_selectedAttack == PlayerActions.Meteor && _canMeteor)
            {
                _canMeteor = false;
                Invoke(nameof(MeteorAttack), meteorDelay);
            }
        }
        else if (Input.GetMouseButton(0) && _selectedAttack == PlayerActions.Fire && _canFire)
        {
            _canFire = false;
            FireAttack();
        }
    }

    #region Cooldowns
    private void MeteorCooldown()
    {
        if(_canMeteor) return;

        _meteorCooldownTimer += Time.deltaTime;
        
        if(meteorCooldown > _meteorCooldownTimer) return;

        _meteorCooldownTimer = 0f;
        _canMeteor = true;
    }
    
    private void FireCooldown()
    {
        if (_canFire) return;
        
        _fireCooldownTimer += Time.deltaTime;

        if (_fireCooldownTimer < fireCooldown) return;
        
        _fireCooldownTimer = 0f;
        _canFire = true;
    }
    #endregion
    
    #region Attacks
    private void BasicAttack()
    {
        var mousePosition = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
        var hit = Physics2D.Raycast(mousePosition, Vector2.zero);

        var mousePosition2D = new Vector3(mousePosition.x, mousePosition.y, -0.1f);
        Instantiate(impact, mousePosition2D, quaternion.identity);

        if (hit.transform.CompareTag(DinosaurTag))
            hit.transform.GetComponent<DinosaursManager>().Health -= impactDamage;
    }
    
    private void MeteorAttack()
    {
        var mousePosition = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
        var hits = Physics2D.CircleCastAll(mousePosition, meteorRadius, Vector2.zero);
        
        var mousePosition2D = new Vector3(mousePosition.x, mousePosition.y, -0.1f);
        Instantiate(meteor, mousePosition2D, quaternion.identity);

        foreach (var hit in hits)
        {
            if (hit.transform.CompareTag(DinosaurTag))
                hit.transform.GetComponent<DinosaursManager>().Health -= meteorDamage;
        }
    }
    
    private void FireAttack()
    {
        var mousePosition = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
        var hit = Physics2D.Raycast(mousePosition, Vector2.zero);

        var mousePosition2D = new Vector3(mousePosition.x, mousePosition.y, -0.1f);
        Instantiate(fire, mousePosition2D, quaternion.identity);
        
        if (hit.transform.CompareTag(DinosaurTag))
            hit.transform.GetComponent<DinosaursManager>().Health -= fireDamage;
    }
    #endregion

    private void ChooseAttack()
    {
        if(Input.GetKeyDown(KeyCode.A))
            _selectedAttack = PlayerActions.Basic;
        else if (Input.GetKeyDown(KeyCode.Z))
            _selectedAttack = PlayerActions.Meteor;
        else if (Input.GetKeyDown(KeyCode.E))
            _selectedAttack = PlayerActions.Fire;
    }
}