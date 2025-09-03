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
                
                break;
            case AlumetteState.Bouteille:

                break;
            case AlumetteState.FireRing:

                break;
            case AlumetteState.Savon:

                break;
        }
    }
}