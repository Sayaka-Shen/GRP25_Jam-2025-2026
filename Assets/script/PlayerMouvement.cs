using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody), typeof(PlayerInput))]
public class PlayerMouvement : MonoBehaviour
{
    public bool Player1 = true;
    public PlayerMouvement _otherPlayer;
    
    [Header("Paramètres")]
    public float moveSpeed = 5f;
    public float dashForce = 15f;
    public float dashDuration = 0.3f;
    public float dashCooldown = 1f;
    [SerializeField] private GameObject fireRing;
    public bool _canMove = false;
    [SerializeField] private float _durationWet = 10f;     // 3 minutes = 180 secondes
    
    [Header("Glissement (Effet Glace)")]
    public bool isSliding = false;
    private Vector3 startDir;
    
    private int _nbAlumette = 0;
    public int NbAlumette { get { return _nbAlumette; } set {  _nbAlumette = value; } }

    private bool _haveFlag = false;
    private Alumette.AlumetteState _alumette;
    [SerializeField] private float _forceBump = 5;

    public GameObject _zoneSavon;
    // Variables pour le dash
    private bool _isDashing = false;
    private bool _canDash = false;
    private float _dashTimer = 0f;
    public bool _isWet = false;
    private float _timeRemaining;
    private bool _isWetRunning = false;
    private Rigidbody rb;
    private Vector2 moveInput;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (!_canMove) return;
        moveInput = context.ReadValue<Vector2>();
    }
    
    private void Update()
    {
        if (!_canMove) return;
        // Gestion du timer de dash
        if (_isDashing)
        {
            _dashTimer -= Time.deltaTime;
            if (_dashTimer <= 0f)
            {
                EndDash();
            }
        }
        if (_isWetRunning)
        {
            _timeRemaining -= Time.deltaTime;

            if (_timeRemaining <= 0f)
            {
                _timeRemaining = 0f;
                _isWetRunning = false;
                _isWet = false;
            }

        }
        
    }
    public void StartTimer()
    {
        _timeRemaining = _durationWet;
        _isWetRunning = true;
        _isWet = true;
        
    }

    private void FixedUpdate()
    {
        if (!_canMove) return;
        if (!_isDashing)
        {
            Vector3 inputDirection = new Vector3(moveInput.x, 0, moveInput.y);
            
            if (isSliding)
            {
                Vector3 move = startDir * moveSpeed * Time.fixedDeltaTime;
                rb.MovePosition(rb.position + move);
            }
            else
            {
                // Mouvement normal avec arrêt immédiat
                Vector3 move = inputDirection * moveSpeed * Time.fixedDeltaTime;
                rb.MovePosition(rb.position + move);
                
                // Arrêt immédiat quand pas d'input (mouvement normal)
                if (inputDirection == Vector3.zero)
                {
                    rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
                }
            }
        }

        // Pendant le dash, la physique continue normalement
    }
    
    public void ApplyBump(Vector3 direction, float force)
    {
        Vector3 bumpDirection = new Vector3(direction.x, 0, direction.z).normalized;
        rb.linearVelocity = Vector3.zero;
        rb.AddForce(bumpDirection * force, ForceMode.Impulse);
    }

    public void Dash()
    {
        
        Vector3 dashDirection = new Vector3(moveInput.x, 0, moveInput.y).normalized;
        
        // Si aucune direction n'est pressée, dash vers l'avant du joueur
        if (dashDirection == Vector3.zero)
        {
            dashDirection = transform.forward;
        }
        
        StartDash(dashDirection);
    }

    
    public void UseAlumette()
    {
        if (_alumette == null) return;
        
        switch (_alumette)
        {
            case Alumette.AlumetteState.Dash:
                Dash();
                print("dash");
                break;
            case Alumette.AlumetteState.Bouteille:
                _otherPlayer.StartTimer();
                print("bouteille");
                break;
            case Alumette.AlumetteState.Savon:
                Instantiate(_zoneSavon, _otherPlayer.transform.position, _zoneSavon.transform.rotation);
                break;
            case Alumette.AlumetteState.FireRing:
                    fireRing.gameObject.SetActive(true);
                break;
            default:
                break;
        }
        _alumette = Alumette.AlumetteState.Nothing;
        GameManager.Instance.OnAlumetteUse(Player1);
    }

    private void StartDash(Vector3 direction)
    {
        _isDashing = true;
        _canDash = false;
        _dashTimer = dashDuration;
        
        rb.linearVelocity = Vector3.zero;
        rb.AddForce(direction * dashForce, ForceMode.Impulse);
        
        // Démarrer le cooldown
        StartCoroutine(DashCooldownCoroutine());
    }
    
    private void EndDash()
    {
        _isDashing = false;
        // Optionnel : réduire la vélocité pour un arrêt plus contrôlé
        rb.linearVelocity *= 0.5f;
    }
    
    private IEnumerator DashCooldownCoroutine()
    {
        yield return new WaitForSeconds(dashCooldown);
        _canDash = true;
    }
    

    
    public void EnableSliding()
    {
        isSliding = true;
    }
    
    public void DisableSliding()
    {
        isSliding = false;
    }
    
    public void ToggleSliding()
    {
        isSliding = !isSliding;
    }

    private void OnCollisionEnter(Collision other)
    {
        Alumette alumette = other.gameObject.GetComponent<Alumette>();
        if (alumette != null)
        {

            if (alumette.AlumetteType == Alumette.AlumetteState.BaseState)
            {
                _nbAlumette++;
                GameManager.Instance.AddPoints(1, Player1);
            }
            else
            {
                _alumette = alumette.AlumetteType;
                GameManager.Instance.OnAlumette(_alumette, Player1);
            }
            Destroy(other.gameObject);
        }
        if (other.gameObject.tag == "Player")
        {
            Vector3 dir;
            moveInput = Vector2.zero;
            this.rb.linearVelocity = Vector3.zero;
            dir = new Vector3(this.transform.position.x - other.gameObject.transform.position.x, 0, this.transform.position.z - other.gameObject.transform.position.z);
            ApplyBump(dir, _forceBump);
            if (_haveFlag)
                _haveFlag = false;

            // Interrompre le dash si collision avec un autre joueur
            if (_isDashing)
            {
                EndDash();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Ice")
        {
            Vector3 inputDirection = new Vector3(moveInput.x, 0, moveInput.y);
            startDir =  inputDirection;
            isSliding = true;
        }
        
        if (other.gameObject.tag == "FireCamp" && !_isWet)
        {
            other.gameObject.GetComponent<CampFire>().AddAllumettes(_nbAlumette, Player1);
            GameManager.Instance.ResetPoint(Player1);
            _nbAlumette = 0;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Ice")
        {
            startDir = Vector3.zero;
            isSliding = false;
        }
    }
}