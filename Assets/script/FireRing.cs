using UnityEngine;

public class FireRing : MonoBehaviour
{
    [Header("FireRing Param")]
    [SerializeField] private float m_forceBump;
    [SerializeField] private float m_timeBeforeExit;
    private float m_timer; 

    private void Update()
    {
        m_timer += Time.deltaTime;
        
        if(m_timer > m_timeBeforeExit)
        {
            m_timer = 0f;
            this.gameObject.SetActive(false);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            PlayerMouvement playerMovement = collision.gameObject.GetComponent<PlayerMouvement>();
            playerMovement.ApplyBump(new Vector3(this.transform.position.x - collision.gameObject.transform.position.x, 0, this.transform.position.z - collision.gameObject.transform.position.z), m_forceBump);
            playerMovement.NbAlumette = 0;
            GameManager.Instance.ResetPoint(playerMovement.Player1);
        }
    }
}
