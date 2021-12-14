#region Namespaces
using System.Collections.Generic;
using UnityEngine;
#endregion

// Open the profiler and click on Memory to see how much memory is being used
// Switch between Heavy and Flyweight to compare

/// <summary> Illustrates the Flyweight Pattern </summary>
public class FlyweightController : MonoBehaviour
{
    [Tooltip("Set to true if the Flyweight Pattern should be used")]
    public bool useFlyweight = false;

    // Lists of the heavyweightObjects and flyweightObjects
    List<Heavyweight> heavyweightObjects = new List<Heavyweight>();
    List<Flyweight> flyweightObjects = new List<Flyweight>();

    void Start()
    {
        // Defines the number of objects to create
        int numberOfObjects = 1000000;

        if (!useFlyweight) { GenerateHeavyweight(numberOfObjects); }
        else { GenerateFlyweight(numberOfObjects); }
    }

    /// <summary> Generate Heavyweight objects (do not share any data) </summary>
    /// <param name="number">The number of objects to generate </param>
    void GenerateHeavyweight(int number)
    {
        for (int i = 0; i < number; i++)
        {
            Heavyweight newHeavyweight = new Heavyweight();

            heavyweightObjects.Add(newHeavyweight);
        }
    }

    /// <summary> Generates Flyweight objects (share data) </summary>
    /// <param name="number">The number of objects to generate </param>
    void GenerateFlyweight(int number)
    {
        // Generate the shared data
        Data data = new Data();

        for (int i = 0; i < number; i++)
        {
            Flyweight newFlyweight = new Flyweight(data);

            flyweightObjects.Add(newFlyweight);
        }
    }
}
