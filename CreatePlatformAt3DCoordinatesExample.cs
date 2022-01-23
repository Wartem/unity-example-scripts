using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Creation of gameObjects at 3D coordinates. 
/// I've taken out some example code from a 3D project (prototype) of mine.
/// In it's context, connected to the Unity Editor, this code allows for 
/// creation of gameobjects in form of objects (the prefab is a cube/platform).
/// Created objects are set to a Vector3 pos (x,y,z).
/// </summary>

public class ExampleFunctions : MonoBehaviour
{

    private GameObject currentPlatform; // Set from Instantiated platformCube
    public GameObject platformCubePrefab; // Premade object set from the Unity Editor (public)

    void Start()
    {
        currentPlatform = Instantiate(platformCubePrefab);
        currentPlatform.transform.position = new Vector3(0, 10, 0);
        currentPlatform.SetActive(true);
        setPlatformState(currentPlatform, 1);

        // Example usage of the main function in this file: makeNewPlatform:
        makeNewPlatform(currentPlatform.transform.position.x - 
            (currentPlatform.GetComponent<Renderer>().bounds.size.x),
                    currentPlatform.transform.position.y,
                    currentPlatform.transform.position.z);
    }

    void deleteIntersects(GameObject doNotDestroy)
    {
        GameObject[] platforms = GameObject.FindGameObjectsWithTag("Platform");

        for (int e = 0; e < platforms.Length; e++)
        {
            for (int i = 0; i < platforms.Length; i++)
            {
                if (platforms[i] != null &&
                    platforms[i] != currentPlatform &&
                    platforms[i] != platforms[e] &&
                    platforms[i] != doNotDestroy &&
               platforms[e].GetComponent<BoxCollider>().bounds.Intersects(platforms[i].GetComponent<BoxCollider>().bounds)
               )
                {
                    Debug.Log("Destroy " + i);
                    Destroy(platforms[i]);
                }
            }
        }
    }

    public Color getBlackColor()
    {
        return new Color(0, 0, 0);
    }

    public Color getRandomBrightColor()
    {
        return new Color(
            UnityEngine.Random.Range(0.7f, 1f),
            UnityEngine.Random.Range(0.7f, 1f),
            UnityEngine.Random.Range(0.7f, 1f));
    }

    /// <summary>
    /// State can either be set to 0 or 1.
    /// 0 is the first state (black color). State 1 is set to objects when the player
    /// intersects with them for the first time (random color set).
    /// </summary>
    /// <param name="gameObjectToUse"></param>
    /// <param name="state">0 for not yet used. 1 for activated by player intersection.</param>
    public void setPlatformState(GameObject gameObjectToUse, int state)
    {
        if (gameObjectToUse.tag == "Platform" || gameObjectToUse.tag == "PlatformChild")
        {
            // PlatformData is pre-added to the prefab in the Unity Editor.
            // In which every object gets a "public int state = 0;"
            PlatformData platformData = gameObjectToUse.GetComponent<PlatformData>(); 
            platformData.state = state;

            Color colorToUse = getBlackColor();

            if (state == 0)
            {
                colorToUse = getBlackColor();
            }
            else if (state == 1)
            {
                colorToUse = getRandomBrightColor();
            }

            gameObjectToUse.GetComponent<Renderer>().material.color = colorToUse;
            int children = gameObjectToUse.transform.childCount;
            for (int i = 0; i < children; i++)
            {
                gameObjectToUse.transform.GetChild(i).gameObject.GetComponent<Renderer>().material.color = colorToUse;
            }
        }
        else
        {
            Debug.Log("setPlatformState failed");
        }
    }

    public bool destroyObjectsInsideVector(Vector3 newPos )
    {

        Collider[] hitColliders = Physics.OverlapSphere(newPos
            , 3f);

        foreach (var hitCollider in hitColliders)
        {
            Destroy(hitCollider.gameObject, 0);
        }

        return hitColliders.Length > 0;
    }

    private void makeNewPlatform(float x, float y, float z)
    {

        if (destroyObjectsInsideVector(new Vector3(x, y, z)))
        {
            Debug.Log("Vector inside new platform detected. Old object deleted.");
        }

        GameObject newGameObj = Instantiate(platformCubePrefab);
        newGameObj.transform.position = new Vector3(x, y, z);
        newGameObj.GetComponent<Renderer>().material.color = getBlackColor();
        setPlatformState(newGameObj, 0);
        setPlatformState(currentPlatform, 1);
        deleteIntersects(newGameObj);
    }
    }
