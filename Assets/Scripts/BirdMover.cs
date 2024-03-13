using System.Collections;
using UnityEngine;

public class BirdMover : MonoBehaviour
{
    [SerializeField] InputManagerBase _inputManager;
    [SerializeField] Animator _animator;
    [SerializeField] float _maxMoveMelocity = 5.0f,_flapForce = 1.5f, _gravity = 2f, _moveSpeed = 1.1f, _flapDelay = 0.25f, _raycastDistance = 0.52f, _skidTime = 1.1f, _thunkDelay = 0.25f;
    [SerializeField] LayerMask _groundedMask;
    [SerializeField] SpriteRenderer _spriteRenderer;

    float _flapTime,_thunkTime, _currentSkidTime, _speedFactor = 1f;
    float _flapVelocity, _moveVelocity;
    RaycastHit2D[] _raycastHit;
    bool _flapping, _skidding, _isInvulnerable;
    Vector3 _direction;
    static readonly int Flapping = Animator.StringToHash("Flap");
    static readonly int Flying = Animator.StringToHash("Flying");
    static readonly int Speed = Animator.StringToHash("Speed");
    static readonly int Skidding = Animator.StringToHash("Skidding");
    IInputManager InputManager;
    bool isPlayer => TryGetComponent<PlayerInputManager>(out var playerInputManager);

    bool ShouldSkid() { 

            if(_skidding || isFlying()) return false;
            if(Mathf.Abs(_moveVelocity) <= 0.75f) return false;
            if(Mathf.Approximately(InputManager.Movement.x,0f)) return false;
            if(_direction == Vector3.right && InputManager.Movement.x < 0f) return true;
            return _direction == Vector3.left && InputManager.Movement.x > 0f;  
    }

    bool IsMount(){
        return gameObject.layer == LayerMask.NameToLayer("Player Mount") || gameObject.layer == LayerMask.NameToLayer("Enemy Mount");
    }

    bool canFlap () {
        return !_skidding && Time.time > _flapTime;
    }
    bool canBounce(){
        return Time.time > _thunkTime;
    }
    bool IsGrounded (){
        if(_flapping) return false;
        return Physics2D.RaycastNonAlloc(transform.position,Vector2.down, _raycastHit, _raycastDistance,_groundedMask) > 0;
    }
    bool InputReceived(){
        return _flapping || !Mathf.Approximately(InputManager.Movement.x,0f);
    }
    bool isFlying(){
        return !IsGrounded();
    }

    public void Init(Vector3 direction, float speedFactor = 1f){
        SetInvulnerability(true);
        _direction =direction;
        _speedFactor = speedFactor;
    }
    public IEnumerator ShowSpawnEffect (float spawnLength){
        SetInvulnerability(true);
        var originalColor = _spriteRenderer.material.color;
        float time = spawnLength;
        while (time > 0f && !InputReceived()){
            time -= Time.deltaTime;
            _spriteRenderer.material.color = Random.ColorHSV();
            if(transform.localScale.x < 1f){
                transform.localScale = Vector3.Lerp(transform.localScale,Vector3.one,Time.deltaTime);
            }
            yield return null;
        }
        transform.localScale = Vector3.one;
        _spriteRenderer.material.color = originalColor;
        SetInvulnerability(false);
    }

    void SetInvulnerability(bool invulnerable){
        _isInvulnerable = invulnerable;
        _inputManager.enabled = !invulnerable;

        if(IsMount()){
            Physics2D.IgnoreLayerCollision(gameObject.layer,LayerMask.NameToLayer("Player"),true);
            Physics2D.IgnoreLayerCollision(gameObject.layer,LayerMask.NameToLayer("Enemy Bird"),true);
            return;
        }
        Physics2D.IgnoreLayerCollision(gameObject.layer, isPlayer ? LayerMask.NameToLayer("Enemy Bird") : LayerMask.NameToLayer("Player"),invulnerable);
    }

    void Awake(){
        InputManager = GetComponent<IInputManager>();
        InputManager.OnFlapPressed += OnFlapPressed;
        _raycastHit = new RaycastHit2D[10];
        Init(Vector3.right);
        SetInvulnerability(false);
    }

    void OnDisable(){
        InputManager.OnFlapPressed -= OnFlapPressed;
    }
    void Start(){
        _flapTime = Time.time;
        _thunkTime = Time.time;
    }
    
    void Update(){
        HandleVerticalMovement();
        HandleHorizontalMovement();
        if(isPlayer) Debug.Log($"MOVE VELOCITY: {_moveVelocity}");
    }

    private void HandleHorizontalMovement()
    {
        if(ShouldSkid()){
            StartSkid();
        }
        if(_skidding){
            HandleSkidMovement();
            return;
        }
        if(ShouldChangeDirection()){
            _direction = _direction == Vector3.right ? Vector3.left : Vector3.right;
            transform.localEulerAngles = new Vector3(0f,_direction == Vector3.right ? 0f : 180f, 0f);
            //transform.rotation = new Quaternion(transform.rotation.x,transform.rotation.y == 0f ? 180f:0f,transform.rotation.z,transform.rotation.w);
        }
        if(_isInvulnerable && !Mathf.Approximately(InputManager.Movement.x,0f)){
            SetInvulnerability(false);
        }
        //_moveVelocity += Mathf.Abs(InputManager.Movement.x) * (_moveSpeed * _speedFactor);

        GetComponent<Rigidbody2D>().velocity *= 0;
        _moveVelocity += isFlying() ? InputManager.Movement.x * Time.deltaTime*2.4f : InputManager.Movement.x * Time.deltaTime * 1.2f;
        _moveVelocity = _moveVelocity < 0 ? Mathf.Max(_moveVelocity,-_maxMoveMelocity) : Mathf.Min(_moveVelocity,_maxMoveMelocity);
        //if(isPlayer) Debug.Log(GetComponent<Rigidbody2D>().velocity);
        transform.position += new Vector3((_moveVelocity * Time.deltaTime),0,0);
        _animator.SetFloat(Speed,Mathf.Abs(_moveVelocity));
    }

    private bool ShouldChangeDirection()
    {
        if(Mathf.Approximately(InputManager.Movement.x,0f)) return false;
        if(_direction == Vector3.right && _moveVelocity < 0f) return true;
        return _direction == Vector3.left && _moveVelocity > 0f;
    }

    private void HandleSkidMovement()
    {
        transform.position += new Vector3((_moveVelocity*Time.deltaTime),0,0);
        _moveVelocity = _moveVelocity < 0 ?Mathf.Min(_moveVelocity - (_moveVelocity * Time.deltaTime * 1.2f),0f) :  Mathf.Max(_moveVelocity - (_moveVelocity * Time.deltaTime * 1.2f),0f);
       // _moveVelocity = Mathf.Min(_moveVelocity - (_moveVelocity * Time.deltaTime),0f);
        _currentSkidTime += Time.deltaTime;
        if(Mathf.Abs(_moveVelocity) <= 0.75f){
            _moveVelocity = 0;
            StopSkid();
        }
        _animator.SetFloat(Speed,Mathf.Abs(_moveVelocity));
    }

    private void StopSkid()
    {
        _moveVelocity = 0f;
        _skidding = false;
        _animator.SetBool(Skidding,false);
    }

    private void StartSkid()
    {
        SoundManager.Instance.PlaySkidSound();
        _skidding = true;
        _animator.SetFloat(Speed,0f);
        _animator.SetBool(Skidding,true);
        _currentSkidTime = 0f;
    }

    private void HandleVerticalMovement()
    {
        if(_flapping){
            Flap();
            transform.position += Vector3.up * _flapVelocity * Time.deltaTime;
            return;
        }

        if(!IsGrounded()){
            _animator.SetBool(Flying, true);
            ApplyGravity();
        }else{
            _animator.SetBool(Flying,false);
            _flapVelocity = 0f;
        }
        transform.position += Vector3.up*(_flapVelocity*Time.deltaTime);
    }

    private void ApplyGravity()
    {
        _flapVelocity -= _gravity*Time.deltaTime;
    }

    private void Flap()
    {
        if(_isInvulnerable) SetInvulnerability(false);
        _flapTime = Time.time + _flapDelay;
        _animator.SetBool(Flying, true);
        _animator.SetTrigger(Flapping);
        _flapVelocity = _flapForce;
        _flapping = false;
        _skidding = false;
    }

    void OnFlapPressed(){
        if(canFlap()) _flapping = true;
    }

    public void InvertDirection(){
        if(!canBounce()) return;
        _thunkTime = Time.deltaTime + _thunkDelay;
        _moveVelocity *= -1;
        _direction = _direction == Vector3.right ? Vector3.left : Vector3.right;
        transform.localEulerAngles = new Vector3(0f,_direction == Vector3.right ? 0f : 180f, 0f);
        _flapVelocity = 0;
    }
}
