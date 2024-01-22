/// the script to get ice thickness, which i got from GPT. Must be checked... 
using UnityEngine;

public class TerrainIntersection : MonoBehaviour
{
//	public GameObject terrain; // Reference to the terrain object
	Vector3 sourcePoint; // Source point beneath the terrain

    private void Awake()
    {
    }
    void Update()
    {
        sourcePoint = transform.position - Vector3.down * 1.1f;
        // Perform raycasting from the source point vertically downwards
        Ray ray = new Ray(sourcePoint, Vector3.down);
		RaycastHit hit;
        float depth = -1;
		if (Physics.Raycast(ray, out hit))
        {
            Debug.DrawRay(sourcePoint, Vector3.down * hit.distance, Color.yellow);
            /*
            // Check if the ray intersects with the terrain object
            if (hit.collider.gameObject == terrain)
			{
				// Get the intersection point
				Vector3 intersectionPoint = hit.point;

				Debug.Log("Intersection Point: " + intersectionPoint);
			}
			else
			{
				Debug.Log("Ray does not intersect with the terrain!");
			}*/
            depth = hit.distance;
		}
        else
            Debug.DrawRay(sourcePoint, Vector3.down * 100f, Color.red);
//        Debug.Log($"Depth: {depth}");
    }
}