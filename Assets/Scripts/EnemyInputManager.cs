using System;
using UnityEngine;
public class EnemyInputManager : InputManagerBase, IInputManager
{
    public Vector2 Movement {get; private set;}

    public event Action<Vector2> OnMoveRecieved = delegate {};
    public event Action OnFlapPressed = delegate {};
    [SerializeField] Vector2 _flapIntervalRange = new Vector2(0.5f, 3f);
    [SerializeField] Vector2 _directionChangeIntervalRange = new Vector2(5f,10f);
    [SerializeField] float _maxHeight = 3.75f, _kickbackBoundary = 5f;

    float _nextFlapTime;
    Vector2 _direction;
    float _nextDirectionChange;
    Thunk _thunk;
    readonly Vector2 _moveLeft = new Vector2(-1f,0f);
    readonly Vector2 _moveRight = new Vector2(1f,0f);

    void OnEnable(){
        SetNextDirectionChangeTime();
        SubscribeToThunkEvent();
        _direction = transform.localEulerAngles == Vector3.zero ? Vector3.right : Vector3.left;
    }

    void OnDisable(){
        UnsubscribeFromThunkEvent();
    }

    void Update(){
        if(ShouldFlap()){
            Flap();
        }
        if(ShouldChangeDirection()){
            ChangeDirection();
        }
        Move();
    }

    private void ChangeDirection()
    {
        SetNextDirectionChangeTime();
        _direction = _direction == Vector2.right ? Vector2.left : Vector2.right;
    }

    private void Move()
    {
        Movement = GetMoveDirection();
        OnMoveRecieved(Movement);
    }

    private Vector2 GetMoveDirection()
    {
        return _direction == Vector2.right ? _moveRight : _moveLeft;
    }

    private bool ShouldChangeDirection()
    {
        return Time.time >= _nextDirectionChange;
    }

    private bool ShouldFlap()
    {
        return transform.position.y < _maxHeight && Time.time >= _nextFlapTime;
    }

    private void Flap()
    {
        SetNextFlapTime();
        OnFlapPressed();
    }

    private void SetNextFlapTime()
    {
        _nextFlapTime = UnityEngine.Random.Range(_flapIntervalRange.x,_flapIntervalRange.y) + Time.time;
    }

    private void UnsubscribeFromThunkEvent()
    {
        if(_thunk){
            _thunk.OnThunk -= OnThunk;
        }
    }

    private void SubscribeToThunkEvent()
    {
        TryGetComponent<Thunk>(out _thunk);
        if(_thunk){
            _thunk.OnThunk += OnThunk;
        }
    }

    private void OnThunk()
    {
        ChangeDirection();
        ApplyKnockBack();
        GetComponent<BirdMover>().InvertDirection();
    }

    private void ApplyKnockBack()
    {
        var position = transform.position;
        if(Mathf.Abs(position.x) > _kickbackBoundary) return;
        position.x += GetMoveDirection().x *0.5f;
        transform.position = position;
    }

    private void SetNextDirectionChangeTime()
    {
        _nextDirectionChange = UnityEngine.Random.Range(_directionChangeIntervalRange.x,_directionChangeIntervalRange.y) + Time.time;
    }
}
