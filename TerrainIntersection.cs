/// the script to get ice thickness, which i got from GPT. Must be checked... 
using UnityEngine;

public class TerrainIntersection : MonoBehaviour
{
	public GameObject terrain; // Reference to the terrain object
	public Vector3 sourcePoint; // Source point beneath the terrain

	void Update()
	{
		// Perform raycasting from the source point vertically downwards
		Ray ray = new Ray(sourcePoint + Vector3.up * 1000f, Vector3.down);
		RaycastHit hit;

		if (Physics.Raycast(ray, out hit))
		{
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
			}
		}
	}
}