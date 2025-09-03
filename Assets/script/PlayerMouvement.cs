using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody), typeof(PlayerInput))]
public class PlayerMouvement : MonoBehaviour
{
    [Header("Paramètres")]
    public float moveSpeed = 5f;

    [Header("Dash")]
    public float dashForce = 15f;
    public float dashDuration = 0.3f;
    public float dashCooldown = 1f;

    [Header("Glissement (Effet Glace)")]
    public bool isSliding = false;
    [Tooltip("Plus c'est petit, plus l'inertie est forte (glisse longtemps).")]
    public float iceFriction = 0.05f;      // pseudo-friction numérique
    [Tooltip("Contrôle du joueur sur glace (petite accélération conseillée).")]
    public float iceAcceleration = 6f;     // ~ forces d'entrée réduites
    public float maxIceSpeed = 15f;        // vitesse max horizontale sur glace

    [Header("Bump")]
    [SerializeField] private float _forceBump = 5f;

    // État ramassage/flag (laissez si vous l'utilisez ailleurs)
    private int _nbAlumette = 0;
    private bool _haveFlag = false;
    private Alumette _alumette;

    // Dash
    private bool _isDashing = false;
    private bool _canDash = true;
    private float _dashTimer = 0f;

    private Rigidbody rb;
    private Vector2 moveInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // Fortement conseillé pour un perso top-down :
        rb.freezeRotation = true;          // évite de basculer sur collisions
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
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
                EndDash();
        }
    }

    private void FixedUpdate()
    {
        if (_isDashing) return; // ne pas toucher aux vitesses pendant un dash

        Vector3 inputDir = new Vector3(moveInput.x, 0f, moveInput.y);

        if (isSliding)
        {
            // --- MODE GLACE ---
            // 0) Récup vitesse horizontale
            Vector3 v = rb.linearVelocity;
            Vector3 hv = new Vector3(v.x, 0f, v.z);

            // 1) Accélération proportionnelle à l'input (aucun gros seuil qui bloque)
            //    -> si tu veux un gain, multiplie moveInput par un facteur.
            if (moveInput.sqrMagnitude > 1e-12f) // epsilon ultra-faible, juste pour éviter NaN
            {
                // Accélération m/s² indépendante de la masse
                inputDir = new Vector3(moveInput.x, 0f, moveInput.y);
                rb.AddForce(inputDir * iceAcceleration, ForceMode.Acceleration);
            }

            // 1bis) Kickstart optionnel pour décoller si on est presque arrêté mais on appuie
            //       (évite l'impression de "coller" avec très faibles inputs)
            if (hv.sqrMagnitude < 0.0001f && moveInput.sqrMagnitude > 0.01f)
            {
                Vector3 n = new Vector3(moveInput.x, 0f, moveInput.y).normalized;
                rb.AddForce(n * (iceAcceleration * 0.5f), ForceMode.Acceleration);
            }

            // 2) Limitation vitesse horizontale (même si tu mets 3000, pas de souci)
            if (hv.magnitude > maxIceSpeed)
            {
                hv = hv.normalized * maxIceSpeed;
                rb.linearVelocity = new Vector3(hv.x, v.y, hv.z);
            }

            // 3) Pseudo-friction faible (freine très lentement -> longue inertie)
            if (hv.sqrMagnitude > 1e-12f)
            {
                rb.AddForce(-hv * iceFriction, ForceMode.Acceleration);
            }
        }
        else
        {
            // --- MODE NORMAL ---
            // On pilote directement la vitesse horizontale -> contrôle précis, arrêt net.
            Vector3 targetHV = inputDir.normalized * moveSpeed;
            Vector3 v = rb.linearVelocity;

            if (inputDir.sqrMagnitude > 0.0001f)
            {
                rb.linearVelocity = new Vector3(targetHV.x, v.y, targetHV.z);
            }
            else
            {
                // arrêt immédiat quand pas d'input
                rb.linearVelocity = new Vector3(0f, v.y, 0f);
            }
        }
    }

    public void ApplyBump(Vector3 direction, float force)
    {
        Vector3 bumpDirection = new Vector3(direction.x, 0, direction.z).normalized;
        rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
        rb.AddForce(bumpDirection * force, ForceMode.Impulse);

        // si un dash était en cours, on le stoppe pour éviter les états incohérents
        if (_isDashing) EndDash();
    }

    public void Dash()
    {
        if (!_canDash || _isDashing) return;

        Vector3 dashDirection = new Vector3(moveInput.x, 0, moveInput.y).normalized;
        if (dashDirection == Vector3.zero) dashDirection = transform.forward;

        StartDash(dashDirection);
    }

    private void StartDash(Vector3 direction)
    {
        _isDashing = true;
        _canDash = false;
        _dashTimer = dashDuration;

        // Réinitialiser la vitesse horizontale puis appliquer l'impulsion
        Vector3 v = rb.linearVelocity;
        rb.linearVelocity = new Vector3(0f, v.y, 0f);
        rb.AddForce(direction.normalized * dashForce, ForceMode.Impulse);

        StartCoroutine(DashCooldownCoroutine());
    }

    private void EndDash()
    {
        _isDashing = false;
        // Option : réduire un peu la vitesse pour un arrêt contrôlé du dash
        Vector3 v = rb.linearVelocity;
        rb.linearVelocity = new Vector3(v.x * 0.5f, v.y, v.z * 0.5f);
    }

    private IEnumerator DashCooldownCoroutine()
    {
        yield return new WaitForSeconds(dashCooldown);
        _canDash = true;
    }

    // Méthodes publiques pour contrôler le glissement
    public void EnableSliding()  => isSliding = true;
    public void DisableSliding() => isSliding = false;
    public void ToggleSliding()  => isSliding = !isSliding;

    private void OnCollisionEnter(Collision other)
    {
        var alumette = other.gameObject.GetComponent<Alumette>();
        if (alumette != null)
        {
            // Votre logique ramassage ici si besoin
            Destroy(other.gameObject);
        }

        if (other.gameObject.CompareTag("Player"))
        {
            moveInput = Vector2.zero;

            Vector3 dir = new Vector3(
                transform.position.x - other.transform.position.x,
                0f,
                transform.position.z - other.transform.position.z
            );

            ApplyBump(dir, _forceBump);

            if (_haveFlag) _haveFlag = false;
        }
    }
}
