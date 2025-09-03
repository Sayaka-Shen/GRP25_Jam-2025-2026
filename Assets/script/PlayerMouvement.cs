using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody), typeof(PlayerInput))]
public class PlayerMouvement : MonoBehaviour
{
    [Header("Paramètres")]
    public float moveSpeed = 5f;
    public float dashForce = 15f;
    public float dashDuration = 0.3f;
    public float dashCooldown = 1f;
    
    [Header("Glissement (Effet Glace)")]
    public bool isSliding = false;
    public float iceFriction = 0.05f; // Très faible friction comme sur la glace
    public float iceAcceleration = 1.5f; // Accélération réduite pour moins de contrôle
    public float maxIceSpeed = 15f; // Vitesse maximale sur glace
    
    private int _nbAlumette = 0;
    private bool _haveFlag = false;
    private Alumette _alumette;
    [SerializeField] private float _forceBump = 5;
    
    // Variables pour le dash
    private bool _isDashing = false;
    private bool _canDash = true;
    private float _dashTimer = 0f;

    private Rigidbody rb;
    private Vector2 moveInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    private void Update()
    {
        // Gestion du timer de dash
        if (_isDashing)
        {
            _dashTimer -= Time.deltaTime;
            if (_dashTimer <= 0f)
            {
                EndDash();
            }
        }
    }

    private void FixedUpdate()
    {
        if (!_isDashing)
        {
            Vector3 inputDirection = new Vector3(moveInput.x, 0, moveInput.y);
            
            if (isSliding)
            {
                // Mode glace : très peu de contrôle, beaucoup d'inertie
                Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
                
                // Applique la force d'entrée (contrôle réduit)
                if (inputDirection != Vector3.zero)
                {
                    rb.AddForce(inputDirection * iceAcceleration, ForceMode.Acceleration);
                }
                
                // Limite la vitesse maximale
                if (horizontalVelocity.magnitude > maxIceSpeed)
                {
                    rb.linearVelocity = new Vector3(
                        horizontalVelocity.normalized.x * maxIceSpeed,
                        rb.linearVelocity.y,
                        horizontalVelocity.normalized.z * maxIceSpeed
                    );
                }
                
                // Friction très faible (comme sur la glace)
                rb.AddForce(-horizontalVelocity * iceFriction, ForceMode.Acceleration);
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
        if (!_canDash || _isDashing) return;
        
        Vector3 dashDirection = new Vector3(moveInput.x, 0, moveInput.y).normalized;
        
        // Si aucune direction n'est pressée, dash vers l'avant du joueur
        if (dashDirection == Vector3.zero)
        {
            dashDirection = transform.forward;
        }
        
        StartDash(dashDirection);
    }
    
    private void StartDash(Vector3 direction)
    {
        _isDashing = true;
        _canDash = false;
        _dashTimer = dashDuration;
        
        // Réinitialiser la vélocité et appliquer la force de dash
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

    // Méthodes publiques pour contrôler le glissement
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
            switch (alumette.alumetteState)
            {
                case Alumette.AlumetteState.baseState:
                    _nbAlumette++;
                    break;
                case Alumette.AlumetteState.Flag:
                    _haveFlag = true;
                    break;
                case Alumette.AlumetteState.PowerUp:
                    //determinate
                    break;
                    
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
            if(_haveFlag)
                _haveFlag = false;
                
            // Interrompre le dash si collision avec un autre joueur
            if (_isDashing)
            {
                EndDash();
            }
        }
    }
}