using UnityEngine;
using UnityEngine.UI;
using static Alumette;

public class CampFire : MonoBehaviour
{
    [Header("Parameter")]
    [SerializeField] private Slider m_sliderAllumettes;
    [SerializeField] private float m_pointSlider;
    private float m_sliderOriginalValue = 5f;

    public int AllumettesCount { get; private set; }

    private void Start()
    {
        m_sliderAllumettes.value = m_sliderOriginalValue;
    }

    public void AddAllumettes(AlumetteState alumeteType, int allumettes, bool player1)
    {
       if(alumeteType == AlumetteState.BaseState)
       {
            AllumettesCount += allumettes;

            if (!player1)
            {
                m_sliderAllumettes.value += m_pointSlider;
            }

            m_sliderAllumettes.value -= m_pointSlider;
        }
    }
}
