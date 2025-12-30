using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KikiMinigameManager : MonoBehaviour
{
    /* ===================== POWER ===================== */

    [Header("Power Settings")]
    public float maxPower = 100f;
    public float currentPower;

    [Header("Doors to Monitor")]
    public List<Door> doors = new List<Door>();

    [Header("Power Cylinder")]
    public Transform powerCylinder;
    public float scaleLerpSpeed = 5f;

    private float initialLocalHeight;
    private Vector3 initialLocalPosition;

    private bool powerOutTriggered = false;

    /* ===================== GHOST GIRL ===================== */

    [Header("Ghost Girl Settings")]
    public GameObject ghostGirlPrefab;
    public float ghostSpeed = 2f;
    public float ghostDisappearChance = 0.25f;

    [HideInInspector] public bool ghostGirlActive = false;

    /* ===================== PAPERS ===================== */

    [Header("Paper Settings")]
    public GameObject paperPrefab;
    public List<Transform> paperSpawnPoints = new List<Transform>();

    private Dictionary<Transform, Paper> papersBySpawn = new Dictionary<Transform, Paper>();
    [Header("Rolling Rocks")]
    public List<RollingRock> rollingRocks = new List<RollingRock>();


    /* ===================== UNITY ===================== */

    private void Start()
    {
        currentPower = maxPower;

        if (powerCylinder != null)
        {
            initialLocalHeight = powerCylinder.localScale.y;
            initialLocalPosition = powerCylinder.localPosition;
        }

        StartCoroutine(PowerDrainRoutine());
        StartCoroutine(GhostSpawnCheckRoutine());
        StartCoroutine(PaperSpawnRoutine());
        StartCoroutine(RockRollCheckRoutine());

    }

    private void Update()
    {
        UpdatePowerCylinder();

        if (!powerOutTriggered && currentPower <= 0f)
        {
            HandlePowerOut();
        }
    }

    /* ===================== POWER ===================== */

    private IEnumerator PowerDrainRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);

            if (powerOutTriggered)
                yield break;

            float drain = 0.25f;

            foreach (var door in doors)
            {
                if (door != null && !door.IsOpen())
                    drain += 0.25f;
            }

            currentPower = Mathf.Max(0f, currentPower - drain);
        }
    }

    private void UpdatePowerCylinder()
    {
        if (powerCylinder == null) return;

        float normalized = currentPower / maxPower;
        float targetHeight = initialLocalHeight * normalized;

        Vector3 scale = powerCylinder.localScale;
        scale.y = Mathf.Lerp(scale.y, targetHeight, Time.deltaTime * scaleLerpSpeed);
        powerCylinder.localScale = scale;

        // Anchor bottom while allowing parent movement
        Vector3 localPos = initialLocalPosition;
        localPos.y = initialLocalPosition.y - (initialLocalHeight - scale.y) * 0.5f;
        powerCylinder.localPosition = localPos;
    }

    /* ===================== POWER OUT ===================== */

    private void HandlePowerOut()
    {
        powerOutTriggered = true;

        Debug.Log("POWER OUT — doors forced open");

        foreach (Door door in doors)
        {
            if (door == null) continue;

            if (!door.IsOpen())
                door.Open();

            door.enabled = false;
        }
    }

    /* ===================== GHOST GIRL ===================== */

    private IEnumerator GhostSpawnCheckRoutine()
    {
        float cooldown = 10f;
        const float minCooldown = 3f;

        while (true)
        {
            yield return new WaitForSeconds(cooldown);

            if (powerOutTriggered)
                yield break;

            if (!ghostGirlActive)
            {
                List<Door> openDoors = doors.FindAll(d => d != null && d.IsOpen());

                if (openDoors.Count > 0 && Random.value <= 1f / 3f)
                {
                    Door door = openDoors[Random.Range(0, openDoors.Count)];
                    SpawnGhostGirl(door);
                }
            }

            cooldown = Mathf.Max(minCooldown, cooldown - 0.5f);
        }
    }

    private void SpawnGhostGirl(Door door)
    {
        if (ghostGirlActive || ghostGirlPrefab == null || door == null) return;

        Vector3 spawnPos = door.transform.position + door.transform.right * 10f;
        GameObject ghost = Instantiate(ghostGirlPrefab, spawnPos, Quaternion.identity);

        ghostGirlActive = true;
        StartCoroutine(GhostBehaviorRoutine(ghost, door));
    }

    private IEnumerator GhostBehaviorRoutine(GameObject ghost, Door door)
    {
        while (ghost != null)
        {
            if (!door.IsOpen())
            {
                float timer = 0f;

                while (!door.IsOpen())
                {
                    yield return null;
                    timer += Time.deltaTime;

                    if (timer >= 1f)
                    {
                        timer = 0f;

                        if (Random.value <= ghostDisappearChance)
                        {
                            Destroy(ghost);
                            ghostGirlActive = false;
                            yield break;
                        }
                    }
                }
            }

            ghost.transform.position = Vector3.MoveTowards(
                ghost.transform.position,
                door.transform.position,
                ghostSpeed * Time.deltaTime
            );

            if (Vector3.Distance(ghost.transform.position, door.transform.position) < 0.1f)
            {
                Debug.Log("Ghost girl reached the door!");
                Destroy(ghost);
                ghostGirlActive = false;
                yield break;
            }

            yield return null;
        }
    }

    /* ===================== PAPERS ===================== */

    private IEnumerator PaperSpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f);

            if (powerOutTriggered)
                yield break;

            if (Random.value > 0.5f)
                continue;

            List<Transform> freeSpawns = new List<Transform>();

            foreach (var spawn in paperSpawnPoints)
            {
                if (!papersBySpawn.ContainsKey(spawn))
                    freeSpawns.Add(spawn);
            }

            if (freeSpawns.Count == 0)
                continue;

            Transform chosenSpawn = freeSpawns[Random.Range(0, freeSpawns.Count)];

            GameObject paperObj = Instantiate(paperPrefab, chosenSpawn.position, chosenSpawn.rotation);
            Paper paper = paperObj.GetComponent<Paper>();

            if (paper != null)
            {
                paper.manager = this;
                paper.spawnPoint = chosenSpawn;
                papersBySpawn.Add(chosenSpawn, paper);
            }
        }
    }


    public void OnPaperCollected(Paper paper)
    {
        if (paper != null && paper.spawnPoint != null)
        {
            papersBySpawn.Remove(paper.spawnPoint);
        }
    }

    private IEnumerator RockRollCheckRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(8f);

            if (powerOutTriggered)
                yield break;

            if (rollingRocks.Count == 0 || doors.Count == 0)
                continue;

            if (Random.value > (1f / 3f))
                continue;

            RollingRock rock = rollingRocks[Random.Range(0, rollingRocks.Count)];
            Door door = GetNearestDoor(rock.transform.position);


            if (rock != null && door != null)
            {
                rock.RollThroughDoor(door.transform);
            }
        }
    }

    private Door GetNearestDoor(Vector3 fromPosition)
    {
        Door nearest = null;
        float bestDist = float.MaxValue;

        foreach (Door door in doors)
        {
            if (door == null)
                continue;

            float d = Vector3.Distance(fromPosition, door.transform.position);
            if (d < bestDist)
            {
                bestDist = d;
                nearest = door;
            }
        }

        return nearest;
    }


}
