using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Alumette : MonoBehaviour
{
    public Material[] Materials;
    public enum AlumetteState
    {
        Nothing,
        BaseState,
        Dash,
        Bouteille,
        Savon, 
        FireRing
    }
    public AlumetteState AlumetteType { get; set; }

    private Renderer objectRenderer;

    private void Start()
    {
        // Récupérer le Renderer (MeshRenderer ou SpriteRenderer)
        objectRenderer = GetComponent<Renderer>();
        
        // Si pas de Renderer sur cet objet, chercher dans les enfants
        if (objectRenderer == null)
        {
            objectRenderer = GetComponentInChildren<Renderer>();
        }
        
    }

    private void ApplyMat()
    {
        if (objectRenderer == null)
        {
            Debug.LogWarning($"Aucun Renderer trouvé sur {gameObject.name}");
            return;
        }

        Material targetMat = GetMatForType(AlumetteType);
        
        objectRenderer.material = targetMat;
        
        
    }

    private Material GetMatForType(AlumetteState type)
    {
        switch (type)
        {
            case AlumetteState.BaseState:
                return Materials[0]; // Blanc pour l'état de base
                
            case AlumetteState.Dash:
                return Materials[1]; // Jaune pour le dash (vitesse)
                
            case AlumetteState.Bouteille:
                return Materials[2]; // Bleu pour la bouteille
                
            case AlumetteState.Savon:
                return Materials[3]; // Cyan pour le savon (glissant)
                
            case AlumetteState.FireRing:
                return Materials[4]; // Rouge pour le feu
                
            default:
                return Materials[0]; // Gris par défaut
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        ApplyMat();
    }
}