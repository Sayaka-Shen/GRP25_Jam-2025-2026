using UnityEngine;
using UnityEngine.InputSystem;

public class Alumette : MonoBehaviour
{
    public enum AlumetteState
    {
        BaseState,
        Dash,
        Bouteille,
        Savon, 
        FireRing
    }
    public AlumetteState AlumetteType { get; set; }

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