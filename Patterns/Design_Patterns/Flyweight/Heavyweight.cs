#region Namespaces
using UnityEngine;
#endregion

/// <summary> Example of an object NOT implementing the Flyweight Pattern </summary>
public class Heavyweight
{
    //Data specific to each individual object
    float _health;
    Data _data;

    public Heavyweight()
    {
        _health = Random.Range(10f, 100f);
        _data = new Data();
    }
}
