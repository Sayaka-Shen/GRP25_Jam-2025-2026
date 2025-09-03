using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody), typeof(PlayerInput))]
public class PlayerMouvement : MonoBehaviour
{
    [Header("Paramètres")]
    public float moveSpeed = 5f;
    private int _nbAlumette = 0;
    private bool _haveFlag = false;
    private Alumette _alumette;
    [SerializeField] private float _forceBump = 5;
    

    private Rigidbody rb;
    private Vector2 moveInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Sera assigné automatiquement via PlayerInput (Unity Events)
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y);
        rb.MovePosition(rb.position + move * moveSpeed * Time.fixedDeltaTime);
    }
    
    public void ApplyBump(Vector3 direction, float force)
    {
        Vector3 bumpDirection = new Vector3(direction.x, 0, direction.z).normalized;
        rb.linearVelocity = Vector3.zero;
        rb.AddForce(bumpDirection * force, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision other)
    {
        print(other.gameObject.tag);
        Alumette alumette = other.gameObject.GetComponent<Alumette>();
        if (alumette != null)
        {
            //switch (alumette.alumetteState)
            //{
            //    case Alumette.AlumetteState.baseState:
            //        _nbAlumette++;
            //        break;
            //    case Alumette.AlumetteState.Flag:
            //        _haveFlag = true;
            //        break;
            //    case Alumette.AlumetteState.PowerUp:
            //        //determinate
            //        break;
                    
            //}
            Destroy(other.gameObject);
        }
        if (other.gameObject.tag == "Player")
        {
            Vector3 dir;
            dir = new Vector3(other.gameObject.transform.position.x - this.transform.position.x, 0,other.gameObject.transform.position.z - this.transform.position.z);
            ApplyBump(dir, _forceBump);
            if(_haveFlag)
                _haveFlag = false;
        }
            
    }
}
