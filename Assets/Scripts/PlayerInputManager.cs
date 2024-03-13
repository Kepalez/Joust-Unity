using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : InputManagerBase, IInputManager
{
    public Vector2 Movement {get;private set;}

    public event Action<Vector2> OnMoveRecieved = delegate(Vector2 vector2){};
    public event Action OnFlapPressed = delegate{};

    Thunk _thunk;
    BirdMover bm;

    void OnEnable(){
        SubscribeToThunkEvent();
        bm = GetComponent<BirdMover>();
    }

    void OnDisable(){
        UnsubscribeFromThunkEvent();
    }

    void OnMove(InputValue value){
        Movement = value.Get<Vector2>();
        OnMoveRecieved(Movement);
    }

    void OnFlap(){
        OnFlapPressed();
    }

    private void UnsubscribeFromThunkEvent()
    {
        if(_thunk){
            _thunk.OnThunk -= OnThunk;
        }
    }

    private void OnThunk()
    {
        bm.InvertDirection();
    }

    private void SubscribeToThunkEvent()
    {
        TryGetComponent<Thunk>(out _thunk);
        if(_thunk){
            _thunk.OnThunk += OnThunk;
        }
    }
}
