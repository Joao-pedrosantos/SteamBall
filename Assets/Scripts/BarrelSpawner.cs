using System.Collections;
using UnityEngine;

public class BarrelSpawner : MonoBehaviour
{
    public GameObject barrelPrefab; // Drag your barrel prefab here in the inspector
    public float spawnRate = 2.0f; // Time between each spawn
    public Vector3 spawnPoint; // Position to spawn the barrels
    public Vector3 spawnForce; // Initial force applied to barrels

    private Rigidbody rb;

    private void Start()
    {
        StartCoroutine(SpawnBarrels());
    }

    private IEnumerator SpawnBarrels()
    {
        while (true)
        {

            
            GameObject newBarrel = Instantiate(barrelPrefab, spawnPoint, Quaternion.Euler(0, 0, 90)); // Rotate 90 degrees on the X-axis
            Rigidbody rb = newBarrel.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.AddForce(spawnForce, ForceMode.Impulse);
                rb.AddTorque(new Vector3(30, 0, 0), ForceMode.Impulse); // Adjust torque direction and magnitude
            }


            yield return new WaitForSeconds(spawnRate);
        }
    }
}
