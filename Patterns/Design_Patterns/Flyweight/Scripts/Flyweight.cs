#region Namespaces
using UnityEngine;
#endregion

/// <summary> Example of an object implementing the Flyweight Pattern </summary>
public class Flyweight
{
    //Data specific to each individual object
    float _health;

    //Data being shared among all objects -> You have to inject it in the constructor
    Data _data;

    // Constructor Method
    public Flyweight(Data data)
    {
        _health = Random.Range(10f, 100f);
        _data = data;
    }
}
