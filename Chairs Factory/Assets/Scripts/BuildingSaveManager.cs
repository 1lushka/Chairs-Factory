using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class SerializationWrapper<T>
{
    public List<T> items;
    public SerializationWrapper(List<T> items) { this.items = items; }
}

public class BuildingSaveManager : MonoBehaviour
{
    private string savePath;
    public List<GameObject> placedBuildings;

    public void Start()
    {
        savePath = Path.Combine(Application.persistentDataPath, "buildings.json");
        //ClearSave();
    }

    public void SaveBuildings()
    {
        List<BuildingSaveData> saveData = new List<BuildingSaveData>();

        foreach (var b in placedBuildings)
        {
            if (b == null || b.name == "Ghost") continue;

            Construction c = b.GetComponent<Construction>();
            BuildingSaveData data = new BuildingSaveData
            {
                prefabName = b.name.Replace("(Clone)", ""),
                posX = b.transform.position.x,
                posY = b.transform.position.y,
                posZ = b.transform.position.z,
                rotX = b.transform.eulerAngles.x,
                rotY = b.transform.eulerAngles.y,
                rotZ = b.transform.eulerAngles.z,
                currentLevel = c != null ? c.currentLevel : 0,
                currentHealth = c != null ? c.currentHealth : 0
            };

            saveData.Add(data);
        }

        string json = JsonUtility.ToJson(new SerializationWrapper<BuildingSaveData>(saveData), true);
        File.WriteAllText(savePath, json);
    }

    public List<BuildingSaveData> LoadBuildings()
    {
        if (!File.Exists(savePath)) return new List<BuildingSaveData>();

        string json = File.ReadAllText(savePath);
        var wrapper = JsonUtility.FromJson<SerializationWrapper<BuildingSaveData>>(json);
        return wrapper?.items ?? new List<BuildingSaveData>();
    }

    public void ClearSave()
    {
        if (File.Exists(savePath))
            File.Delete(savePath);
    }

}
