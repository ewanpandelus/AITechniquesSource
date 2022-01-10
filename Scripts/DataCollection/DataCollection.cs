using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public class DataCollection : MonoBehaviour
{
    [SerializeField] private GameObject colony;
    public bool isRBS;
    private float timeElapsed = 0.0f;
    private float totalTime = 0.0f;
    private int numberAlive = 0;
    private float averageColonyHydration;
    private float averageColonyEnergy;
    private List<float> averageHydrations = new List<float>();
    private List<float> averageEnergies = new List<float>();
    private List<float> queenHealth = new List<float>();
    private Queen queen;
    private void Start()
    {
        queen = GameObject.Find("Queen").GetComponent<Queen>();
    }
    private void Update()
    {
        timeElapsed += Time.deltaTime;
        totalTime += Time.deltaTime;
        numberAlive = colony.transform.childCount;
        if (timeElapsed > 10&&isRBS)
        {
            CalcRBS();
            timeElapsed = 0;
        }
        else if(timeElapsed>10&&!isRBS)
        {
            CalcFSM();
            timeElapsed = 0;
        }

        if (totalTime > 131)
        {
            SaveToFile();
            totalTime = 0;
        }
    }
    private void CalcFSM()
    {
        CalculateAverageColonyHydrationFSM();
        CalculateAverageColonyEnergyFSM();
        queenHealth.Add((float)System.Math.Round(queen.GetEnergy(), 2));
    }
    private void CalcRBS()
    {
        CalculateAverageColonyHydrationRBS();
        CalculateAverageColonyEnergyRBS();
        queenHealth.Add((float)System.Math.Round(queen.GetEnergy(),2));
    }
    private void CalculateAverageColonyHydrationRBS()
    {
        float totalHydration = 0f;
        for(int i = 0; i < colony.transform.childCount - 1; i++)
        {
            totalHydration += colony.transform.GetChild(i).GetComponent<RBSAnt>().GetHydration();
        }
        averageColonyHydration = (float)System.Math.Round(totalHydration /= colony.transform.childCount);
        averageHydrations.Add(averageColonyHydration);
    }
    private void CalculateAverageColonyEnergyRBS()
    {
        float totalEnergy = 0f;
        for (int i = 0; i < colony.transform.childCount - 1; i++)
        {
            totalEnergy += colony.transform.GetChild(i).GetComponent<RBSAnt>().GetHydration();
        }
        averageColonyEnergy = (float)System.Math.Round(totalEnergy /= colony.transform.childCount,2);
        averageEnergies.Add(averageColonyEnergy);
    }
    private void CalculateAverageColonyHydrationFSM()
    {
        float totalHydration = 0f;
        for (int i = 0; i < colony.transform.childCount - 1; i++)
        {
            totalHydration += colony.transform.GetChild(i).GetComponent<Ant>().GetHydration();
        }
        averageColonyHydration = (float)System.Math.Round(totalHydration /= colony.transform.childCount);
        averageHydrations.Add(averageColonyHydration);
    }
    private void CalculateAverageColonyEnergyFSM()
    {
        float totalEnergy = 0f;
        for (int i = 0; i < colony.transform.childCount - 1; i++)
        {
            totalEnergy += colony.transform.GetChild(i).GetComponent<Ant>().GetEnergy();
        }
        averageColonyEnergy = (float)System.Math.Round(totalEnergy /= colony.transform.childCount, 2);
        averageEnergies.Add(averageColonyEnergy);
    }
#if UNITY_EDITOR

#endif


    public string ToCSV()
    {
        var sb = new StringBuilder("Average Hydration, Average Energy, Queen Health, Total Alive");
        for (int i = 0; i<averageHydrations.Count-1;i++)
        {
            sb.Append('\n').Append(averageHydrations[i]).Append(',').Append(averageEnergies[i]).Append(',').Append(queenHealth[i]);
        }
        sb.Append(',').Append(colony.transform.childCount);       

        return sb.ToString();
    }
    public void SaveToFile()
{
    // Use the CSV generation from before
    var content = ToCSV();

    // The target file path e.g.
#if UNITY_EDITOR
    var folder = Application.streamingAssetsPath;

    if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
#else
    var folder = Application.persistentDataPath;
#endif

    var filePath = Path.Combine(folder, "export.csv");

    using (var writer = new StreamWriter(filePath, false))
    {
        writer.Write(content);
    }

    // Or just
    //File.WriteAllText(content);

    Debug.Log($"CSV file written to \"{filePath}\"");

#if UNITY_EDITOR
    AssetDatabase.Refresh();
#endif
}
}
