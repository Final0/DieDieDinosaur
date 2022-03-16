using System.Collections.Generic;
using UnityEngine;

public class SpawnerManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> dinosaurs, waypoints;

    [SerializeField] private float spawnTime;

    private void Awake() => InvokeRepeating(nameof(SpawnDinosaur), spawnTime, spawnTime);

    private void SpawnDinosaur()
    {
        if (!UIManager.GameStarted) return;
        if(UIManager.GameFinished || UIManager.StopSpawn) CancelInvoke(nameof(SpawnDinosaur));

        var randomDinosaur = Random.Range(0, dinosaurs.Count);
        var randomWaypoint = Random.Range(0, waypoints.Count);

        var spawnPosition = new Vector3(waypoints[randomWaypoint].transform.position.x,
            waypoints[randomWaypoint].transform.position.y, dinosaurs[randomDinosaur].transform.position.z);
        
        Instantiate(dinosaurs[randomDinosaur], spawnPosition, transform.rotation, transform);
    }
}