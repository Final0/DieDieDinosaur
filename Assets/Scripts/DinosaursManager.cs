using JetBrains.Annotations;
using UnityEngine;
using Random = UnityEngine.Random;

public class DinosaursManager : MonoBehaviour
{
    [SerializeField] private int health;
    [SerializeField] private float speed;

    public int Health
    {
        get => health;
        set
        {
            health = value;

            StopMoveAnimation();
            
            _animator.Play(health > 0 ? HitAnimation : DeathAnimation);
        }
    }

    private Vector2 _groundSize;

    private bool _hasDestination;

    private Vector3 _startPosition, _currentDestination;

    private const float DestinationRadius = 0.5f;

    private float _timeElapsed;

    private SpriteRenderer _spriteRenderer;

    private Animator _animator;

    private const string RunAnimation = "Run", DeathAnimation = "Death", HitAnimation = "Hit", IdleAnimation = "Idle";

    private bool _stopDinosaur;
    
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        
        _groundSize = new Vector2(8, 4);

        _startPosition = transform.position;
        _currentDestination = Vector3.zero;
        
        _animator.Play(RunAnimation);
    }

    private void Update()
    {
        if (_stopDinosaur) return;
        
        SetDestination();
        CheckDinosaurReachDestination();
        GoToDestination();
        FlipDinosaur();
    }

    private void SetDestination()
    {
        if (_hasDestination) return;
        
        var x = Random.Range(-_groundSize.x, _groundSize.x);
        var y = Random.Range(-_groundSize.y, _groundSize.y);

        _startPosition = transform.position;
        _currentDestination = new Vector3(x, y, -0.01f);

        _timeElapsed = 0f;
        
        _hasDestination = true;
    }

    private void CheckDinosaurReachDestination()
    {
        if(!_hasDestination) return;

        if (Vector3.Distance(transform.position, _currentDestination) < DestinationRadius) _hasDestination = false;
    }

    private void GoToDestination()
    {
        transform.position = Vector3.Lerp(_startPosition, _currentDestination, _timeElapsed / Vector3.Distance(_startPosition, _currentDestination));
        _timeElapsed += speed * Time.deltaTime;
    }

    private void FlipDinosaur()
    {
        var distanceX = _currentDestination.x - transform.position.x;

        _spriteRenderer.flipX = !(distanceX > 0);
    }
    
    private void StopMoveAnimation()
    {
        _stopDinosaur = true;
        _timeElapsed = 0f;
        _hasDestination = false;
        
        _animator.enabled = false;
        _animator.enabled = true;
    }

    [UsedImplicitly]
    public void StartMoveAnimation()
    {
        _stopDinosaur = false;
        _animator.Play(RunAnimation);
    }

    [UsedImplicitly] public void DestroyGameObject() => Destroy(gameObject);
}