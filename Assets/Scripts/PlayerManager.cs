using Unity.Mathematics;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private Grid ground;
    [SerializeField] private float meteorRadius;

    [Header("Particles Effects")]
    [SerializeField] private GameObject impact;
    [SerializeField] private GameObject meteor;
    [SerializeField] private GameObject fire;

    [Header("Attacks Damage")] 
    [SerializeField] private int impactDamage;
    [SerializeField] private int meteorDamage;
    [SerializeField] private int fireDamage;
    
    private Camera _mainCamera;

    private const string DinosaurTag = "Dinosaur";

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
    }

    private void DetectMouseClick()
    {
        if (!Input.GetMouseButtonDown(0)) return;

        if (_selectedAttack == PlayerActions.Basic) BasicAttack();
        else if (_selectedAttack == PlayerActions.Meteor) MeteorAttack();
        else FireAttack();
    }

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