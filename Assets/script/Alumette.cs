using UnityEngine;

public class Alumette : MonoBehaviour
{
    public enum AlumetteState
    {
        baseState,
        PowerUp,
        Flag,
    }
    private AlumetteState _alumetteState;
    
    public AlumetteState alumetteState => _alumetteState;
    
    
}
