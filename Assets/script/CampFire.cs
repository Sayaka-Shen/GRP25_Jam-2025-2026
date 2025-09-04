using UnityEngine;
using UnityEngine.UI;
using static Alumette;

public class CampFire : MonoBehaviour
{
    [Header("Parameter")]
    [SerializeField] public Slider m_sliderAllumettes;
    private float m_pointSlider = 1;
    private float m_sliderOriginalValue = 50f;
    

    private void Start()
    {
        m_sliderAllumettes.value = m_sliderOriginalValue;
    }

    public void AddAllumettes(int allumettes, bool player1)
    {
            for (int i = 0; i < allumettes; i++)
            {
                if (!player1)
                    m_sliderAllumettes.value += m_pointSlider;
                else
                    m_sliderAllumettes.value -= m_pointSlider;
            }
    }
}
