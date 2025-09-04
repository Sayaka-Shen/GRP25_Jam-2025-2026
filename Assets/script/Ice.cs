using System;
using System.Collections;
using UnityEngine;

public class Ice : MonoBehaviour
{
    public float _delayDestroySavon = 5f;
    private void Start()
    {
        StartCoroutine(DestroyIce());
    }
    private IEnumerator DestroyIce()
    {
        yield return new WaitForSeconds(_delayDestroySavon);
        PlayerMouvement[] players = FindObjectsByType<PlayerMouvement>(sortMode:FindObjectsSortMode.None);
        foreach (PlayerMouvement player in players)
        {
            player.isSliding = false;
        }
        Destroy(this.gameObject);
    }
}
