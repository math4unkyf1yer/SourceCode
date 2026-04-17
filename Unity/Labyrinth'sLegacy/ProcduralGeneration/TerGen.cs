using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

public class TerGen : MonoBehaviour
{
    public int width = 500; // x-axis of the terrain
    public int height = 500; // z-axis

    public int depth = 22; // y-axis

    public float scale = 6.6f;

    public float offsetX = 100f;
    public float offsetY = 100f;
     public int sceneInt;
    bool loaded = false;
    private SaveLoadManager saveLoadManager;
    private SaveLoad saveLoad;
    public GameObject[] objectPrefab;
    public int[] howManyForEach;
    public float[] extraHeightAdded;
    public Transform parent;
    public Transform parentForEnemyPosition;
    public string sceneName;
    public int MaxX;
    public int MaxZ;
    public int MinX;
    public int MinZ;

    
    private void Start()
    {
        saveLoadManager = SaveLoadManager.Intance;
        SaveLoad saveLoad = SaveLoad.Intance;
        GameObject player = GameObject.Find("Player");      
        GameData data = saveLoad.GetData();
        SetDataOffsets(data);
        if (loaded == false)
        {
            offsetX = Random.Range(0f, 9999f);
            offsetY = Random.Range(0f, 9999f);
            SaveOffsets(ref data);
        }

        Terrain terrain = GetComponent<Terrain>();
        if (terrain != null)
        {
            terrain.terrainData = GenerateTerrain(terrain.terrainData);
        }
        else
        {
            Debug.LogError("No Terrain component found on this GameObject.");
        }
        // Check if sceneObjects for this sceneInt are empty or not present
        if (!data.sceneObjects.ContainsKey(sceneInt) || data.sceneObjects[sceneInt].Count == 0)
        {
            InstantiateObjectsOnTerrain(ref data);
        }
        else
        {
            SetDataGameObject(data);
        }

        //set all edge to  the same height
        SetTerrainEdgeHeight(terrain, 4);
    }

    //set outer edge to be the same 
    private void SetTerrainEdgeHeight(Terrain terrain, float targetHeight)
    {
        TerrainData terrainData = terrain.terrainData;
        int width = terrainData.heightmapResolution;
        int height = terrainData.heightmapResolution;

        // Normalize target height to fit the heightmap range (0–1)
        float normalizedHeight = targetHeight / terrainData.size.y;

        // Get heightmap values
        float[,] heights = terrainData.GetHeights(0, 0, width, height);

        // Set heights for the top and bottom edges
        for (int x = 0; x < width; x++)
        {
            // Top edge
            heights[0, x] = normalizedHeight;

            // Bottom edge
            heights[height - 1, x] = normalizedHeight;
        }

        // Set heights for the left and right edges
        for (int y = 0; y < height; y++)
        {
            // Left edge
            heights[y, 0] = normalizedHeight;

            // Right edge
            heights[y, width - 1] = normalizedHeight;
        }

        // Apply the modified heights back to the terrain
        terrainData.SetHeights(0, 0, heights);

        Debug.Log("Edge heights set to: " + targetHeight);
    }

    //make the terrain 
    TerrainData GenerateTerrain(TerrainData terrainData)
    {
        if (terrainData == null)
        {
            terrainData = new TerrainData();
        }

        terrainData.heightmapResolution = width + 1;
        terrainData.size = new Vector3(width, depth, height);

        terrainData.SetHeights(0, 0, GenerateHeights());
        GameObject player = GameObject.Find("Player");
        Stats stats = Stats.instance;
      /*  if(stats.firstTime == false)
        {
            player.transform.position = new Vector3(player.transform.position.x, player.transform.position.y +3, player.transform.position.z);
            stats.firstTime = true;
        }*/
        return terrainData;
    }

    float[,] GenerateHeights()
    {
        float[,] heights = new float[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                heights[x, y] = CalculateHeight(x, y);
            }
        }

        return heights;
    }

    float CalculateHeight(int x, int y)
    {
        float xCoord = (float)x / width * scale + offsetX;
        float yCoord = (float)y / height * scale + offsetY;

        return Mathf.PerlinNoise(xCoord, yCoord);
    }
    void InstantiateObjectsOnTerrain(ref GameData data)
    {
        Scene loadedScene = SceneManager.GetSceneByName(sceneName);
        Terrain terrain = GetComponent<Terrain>();
        TerrainData terrainData = terrain.terrainData;
        for (int i = 0; i < howManyForEach.Length; i++)
        {
            for (int j = 0; j < howManyForEach[i]; j++)
            {
                float x = Random.Range(MinX, MaxX);
                float z = Random.Range(MinZ, MaxZ);
                float y = terrain.SampleHeight(new Vector3(x, 0, z));
                Vector3 position;
                position = new Vector3(x, y + extraHeightAdded[i], z);
                GameObject clone = Instantiate(objectPrefab[i], position, Quaternion.identity);
                SceneManager.MoveGameObjectToScene(clone, loadedScene);
                //find a way to seperate object that I want to be save

                clone.transform.SetParent(parent);
            }
        }
        
        SaveGameObjects(ref data);
    }

    //object and offset save 

    public void SaveGameObjects(ref GameData data)
    {
        List<SerializableGameObject> sceneObjectList;
        if (!data.sceneObjects.TryGetValue(sceneInt, out sceneObjectList))
        {
            sceneObjectList = new List<SerializableGameObject>();
            data.sceneObjects[sceneInt] = sceneObjectList;
        }

        sceneObjectList.Clear();  // Clear existing data to avoid duplication
        foreach (Transform child in parent)
        {
            SerializableGameObject sgo = new SerializableGameObject
            {
                prefabName = child.gameObject.name.Replace("(Clone)", "").Trim(),
                posX = child.position.x,
                posY = child.position.y,
                posZ = child.position.z,
                rotX = child.rotation.x,
                rotY = child.rotation.y,
                rotZ = child.rotation.z,
                rotW = child.rotation.w
            };
            sceneObjectList.Add(sgo);
        }
        saveLoadManager.SaveGame(data);
    }
    public void SetDataGameObject(GameData data)
    {
        // Load objects for this terrain
        if (data.sceneObjects.TryGetValue(sceneInt, out List<SerializableGameObject> sceneObjectList))
        {
            foreach (var sgo in sceneObjectList)
            {
                GameObject prefab = Resources.Load<GameObject>($"Prefab/Object/{sgo.prefabName}");
                if (prefab != null)
                {
                    Vector3 position = new Vector3(sgo.posX, sgo.posY, sgo.posZ);
                    Quaternion rotation = new Quaternion(sgo.rotX, sgo.rotY, sgo.rotZ, sgo.rotW);
                    GameObject obj = Instantiate(prefab, position, rotation);
                    obj.transform.SetParent(parent);
                    ObjectGroundCheck objeScript = obj.GetComponent<ObjectGroundCheck>();
                    if(objeScript != null)
                    {
                        if (objeScript.isWallOrFloor)//change layer 
                        {
                            string layerName = "WhatisGround";
                            obj.layer = LayerMask.NameToLayer(layerName);
                            foreach (Transform child in obj.transform)
                            {
                                child.gameObject.layer = LayerMask.NameToLayer(layerName);
                            }
                        }
                    }
                }
            }
        }
    }

    public void SaveOffsets(ref GameData data)
    {
        Debug.Log(data.offsetXX.Count);
        if (data == null)
        {
            data = new GameData();
        }
        data.offsetXX.Insert(sceneInt,offsetX);
        data.offsetXX[sceneInt] = offsetX;
        data.offsetYY.Insert(sceneInt, offsetY);
        data.offsetYY[sceneInt] = offsetY;
        Debug.Log(data.playerX);
        saveLoadManager.SaveGame(data);
    }
    public void SetDataOffsets(GameData data)
    {
        if (data != null && data.offsetXX != null && data.offsetXX.Count > sceneInt)
        {
            loaded = true;
            offsetX = data.offsetXX[sceneInt];
            offsetY = data.offsetYY[sceneInt];
        }
    }

    public void EnemySpawn(GameObject enemy,int height)
    {
        Scene loadedScene = SceneManager.GetSceneByName(sceneName);
        Terrain terrain = GetComponent<Terrain>();
        float x = Random.Range(MinX, MaxX);
        float z = Random.Range(MinZ, MaxZ);
        float y = terrain.SampleHeight(new Vector3(x, 0, z));
        Vector3 position;
        position = new Vector3(x, y + height, z);
        GameObject clone = Instantiate(enemy, position, Quaternion.identity);
        SceneManager.MoveGameObjectToScene(clone, loadedScene);
        clone.transform.SetParent(parent);
    }
}
