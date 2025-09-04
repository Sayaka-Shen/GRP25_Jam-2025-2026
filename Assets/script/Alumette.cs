using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Alumette : MonoBehaviour
{
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
        
        // Appliquer la couleur selon le type
        ApplyColor();
    }

    private void ApplyColor()
    {
        if (objectRenderer == null)
        {
            Debug.LogWarning($"Aucun Renderer trouvé sur {gameObject.name}");
            return;
        }

        Color targetColor = GetColorForType(AlumetteType);
        
        objectRenderer.material.color = targetColor;
        
        
    }

    private Color GetColorForType(AlumetteState type)
    {
        switch (type)
        {
            case AlumetteState.BaseState:
                return Color.white; // Blanc pour l'état de base
                
            case AlumetteState.Dash:
                return Color.yellow; // Jaune pour le dash (vitesse)
                
            case AlumetteState.Bouteille:
                return Color.blue; // Bleu pour la bouteille
                
            case AlumetteState.Savon:
                return Color.cyan; // Cyan pour le savon (glissant)
                
            case AlumetteState.FireRing:
                return Color.red; // Rouge pour le feu
                
            default:
                return Color.gray; // Gris par défaut
        }
    }

    public void ApplyEffect(PlayerMouvement player)
    {
        switch(AlumetteType)
        {
            case AlumetteState.Dash:
                player.Dash();
                break;
            case AlumetteState.Bouteille:

                break;
            case AlumetteState.FireRing:

                FireRing fireRing = player.gameObject.GetComponentInChildren<FireRing>();
                if (fireRing != null)
                {
                    fireRing.gameObject.SetActive(true);   
                }
                
                break;
            case AlumetteState.Savon:
                player.EnableSliding();
                break;
        }
    }
   
}