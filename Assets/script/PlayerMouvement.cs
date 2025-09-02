using UnityEngine;
using UnityEngine.InputSystem; // Nouveau Input System

[RequireComponent(typeof(Rigidbody))]
public class PlayerMouvement : MonoBehaviour
{
    [Header("Paramètres")]
    public float moveSpeed = 5f; // vitesse de déplacement

    private Rigidbody rb;
    private Vector2 moveInput; // valeur du stick gauche

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Cette fonction est appelée automatiquement si tu as créé une action "Move"
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        // Déplacement en fonction de la direction donnée par le stick
        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y);
        rb.MovePosition(rb.position + move * moveSpeed * Time.fixedDeltaTime);
    }
}