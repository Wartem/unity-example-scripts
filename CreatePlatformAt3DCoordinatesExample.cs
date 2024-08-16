using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// This is example code from a 3D project (prototype) of mine.
/// Manages the creation, positioning, and state of platform GameObjects.
/// </summary>
public class PlatformManager : MonoBehaviour
{
    [SerializeField] private GameObject platformCubePrefab;
    [SerializeField] private float platformSpacing = 3f;
    [SerializeField] private float minBrightness = 0.7f;
    [SerializeField] private Color defaultColor = Color.black;

    private GameObject currentPlatform;
    private List<GameObject> allPlatforms = new List<GameObject>();

    private void Start()
    {
        if (platformCubePrefab == null)
        {
            Debug.LogError("Platform prefab not assigned!");
            return;
        }

        CreateInitialPlatform(new Vector3(0, 10, 0));
        CreateNewPlatformAdjacentToCurrent();
    }

    private void CreateInitialPlatform(Vector3 position)
    {
        currentPlatform = InstantiatePlatform(position);
        SetPlatformState(currentPlatform, 1);
    }

    private void CreateNewPlatformAdjacentToCurrent()
    {
        if (currentPlatform == null) return;

        Renderer renderer = currentPlatform.GetComponent<Renderer>();
        if (renderer == null) return;

        Vector3 newPosition = currentPlatform.transform.position;
        newPosition.x -= renderer.bounds.size.x + platformSpacing;

        CreateAndSetupPlatform(newPosition);
    }

    private void CreateAndSetupPlatform(Vector3 position)
    {
        if (DestroyObjectsInsideSphere(position, platformSpacing))
        {
            Debug.Log("Intersecting object(s) detected and removed.");
        }

        GameObject newPlatform = InstantiatePlatform(position);
        if (newPlatform != null)
        {
            SetPlatformState(newPlatform, 0);
            if (currentPlatform != null)
            {
                SetPlatformState(currentPlatform, 1);
            }
            RemoveIntersectingPlatforms(newPlatform);
            currentPlatform = newPlatform;
        }
    }

    private GameObject InstantiatePlatform(Vector3 position)
    {
        GameObject platform = Instantiate(platformCubePrefab, position, Quaternion.identity);
        if (platform != null)
        {
            Renderer renderer = platform.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = defaultColor;
            }
            platform.SetActive(true);
            allPlatforms.Add(platform);
        }
        return platform;
    }

    /// <summary>
    /// Sets the state of the platform and updates its color accordingly.
    /// </summary>
    /// <param name="platform">Platform to update.</param>
    /// <param name="state">State to set (0 for default, 1 for activated).</param>
    private void SetPlatformState(GameObject platform, int state)
    {
        if (platform == null) return;

        PlatformData platformData = platform.GetComponent<PlatformData>();
        if (platformData == null) return;

        platformData.state = state;

        Color color = state == 0 ? defaultColor : GetRandomBrightColor();
        UpdatePlatformColor(platform, color);
    }

    private void UpdatePlatformColor(GameObject platform, Color color)
    {
        if (platform == null) return;

        Renderer[] renderers = platform.GetComponentsInChildren<Renderer>();
        foreach (var renderer in renderers)
        {
            if (renderer != null && renderer.material != null)
            {
                renderer.material.color = color;
            }
        }
    }

    private void RemoveIntersectingPlatforms(GameObject newPlatform)
    {
        if (newPlatform == null) return;

        BoxCollider newCollider = newPlatform.GetComponent<BoxCollider>();
        if (newCollider == null) return;

        for (int i = allPlatforms.Count - 1; i >= 0; i--)
        {
            GameObject platform = allPlatforms[i];
            if (platform == null || platform == currentPlatform || platform == newPlatform) continue;

            BoxCollider platformCollider = platform.GetComponent<BoxCollider>();
            if (platformCollider != null && platformCollider.bounds.Intersects(newCollider.bounds))
            {
                Debug.Log($"Destroying intersecting platform: {platform.name}");
                allPlatforms.RemoveAt(i);
                Destroy(platform);
            }
        }
    }

    private bool DestroyObjectsInsideSphere(Vector3 position, float radius)
    {
        Collider[] hitColliders = Physics.OverlapSphere(position, radius);
        foreach (var collider in hitColliders)
        {
            if (collider != null && collider.gameObject != null)
            {
                Destroy(collider.gameObject);
            }
        }
        return hitColliders.Length > 0;
    }

    private Color GetRandomBrightColor()
    {
        return new Color(
            Random.Range(minBrightness, 1f),
            Random.Range(minBrightness, 1f),
            Random.Range(minBrightness, 1f)
        );
    }
}
