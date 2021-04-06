using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] public Transform spawnLocation;
    [SerializeField] public Transform area;
    public bool isActive = false;
    public Enemy[] enemies;

    private void Start()
    {
        if (area != null)
            enemies = area.transform.GetComponentsInChildren<Enemy>();
        else enemies = transform.GetComponentsInChildren<Enemy>();
    }

    public void ResetCheckpoint(PlayerController player)
    {
        if (isActive)
        {
            DG.Tweening.DOTween.CompleteAll();
            DG.Tweening.DOTween.KillAll();
            player.ResetPlayer(spawnLocation.position);

            ResetEnemies();
        }
    }


    private void ResetEnemies()
    {
        foreach (Enemy enemy in enemies)
        {
            enemy.gameObject.SetActive(true);
            enemy.ResetEntity();
            enemy.ResetVelocityAndInput();
            enemy.ResetEnemy();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!isActive)
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            if (player != null)
            {
                isActive = true;
                player.SetActiveCheckpoint(this);
            }
        }
    }

}
