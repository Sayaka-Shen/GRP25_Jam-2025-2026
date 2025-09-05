using UnityEngine;
using UnityEngine.UI;
using static Alumette;

public class CampFire : MonoBehaviour
{
    [Header("Parameter")]
    [SerializeField] public Slider m_sliderAllumettes;
    private float m_pointSlider = 1;
    private float m_sliderOriginalValue = 50f;
    [SerializeField] private GameObject m_baseFire;
    [SerializeField] private GameObject m_blueFire;
    [SerializeField] private GameObject m_redFire;
    

    private void Start()
    {
        m_sliderAllumettes.value = m_sliderOriginalValue;

        // Set the visual of fire at the start
        m_baseFire.SetActive(true);
        m_blueFire.SetActive(false);
        m_redFire.SetActive(false);
    }

    public void AddAllumettes(int allumettes, bool player1)
    {
        for (int i = 0; i < allumettes; i++)
        {
           if (!player1)
           {
                 m_sliderAllumettes.value += m_pointSlider;

                if(m_sliderAllumettes.value > m_sliderOriginalValue)
                {
                    m_baseFire.SetActive(false);
                    m_blueFire.SetActive(true);
                    m_redFire.SetActive(false);
                }
                
           }
           else
           {
               m_sliderAllumettes.value -= m_pointSlider;

                if (m_sliderAllumettes.value < m_sliderOriginalValue)
                {
                    m_baseFire.SetActive(false);
                    m_blueFire.SetActive(false);
                    m_redFire.SetActive(true);
                }
            }
        }
    }
}
