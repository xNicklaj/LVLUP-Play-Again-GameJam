using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChanceSystem : MonoBehaviour
{
    [Range(0f, 1f)]
    public readonly static float defaultChance = 1.0f;
    private readonly static int randomSeed = 31337;
    private static System.Random random = new System.Random(randomSeed);

    [Serializable]
    public struct ChanceEvent
    {
        [SerializeField]
        public string name;
        [Range(0f, 1f)]
        [SerializeField]
        public float chance;
        [TextArea(1, 3)]
        [SerializeField]
        public string description;
        [HideInInspector] public bool happenedLastTime;
        [HideInInspector] public int previousHappeningsCounter;
    }

    [SerializeField]
    public List<ChanceEvent> events = new List<ChanceEvent>();

    private Dictionary<string, ChanceEvent> eventsDictionary = new Dictionary<string, ChanceEvent>();

    private void OnValidate()
    {
        // Ensure dictionary is rebuilt whenever list changes
        eventsDictionary = events.ToDictionary(x => x.name, x => x);
    }

    public bool AddEvent(string name, float chance, string description)
    {
        // Validate input
        if (string.IsNullOrEmpty(name))
        {
            Debug.LogError("Cannot add ChanceEvent with empty name");
            return false;
        }

        // Check if event already exists
        if (eventsDictionary.ContainsKey(name))
        {
            Debug.LogWarning($"ChanceEvent '{name}' already exists. Use UpdateEvent to modify it.");
            return false;
        }

        // Create and add the new event
        ChanceEvent newEvent = new ChanceEvent { name = name, chance = chance, description = description };
        events.Add(newEvent);
        eventsDictionary.Add(name, newEvent); // manually updating dictionary

        return true;
    }

    public bool RemoveEvent(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            Debug.LogWarning("Cannot remove ChanceEvent with empty name");
            return false;
        }

        if (!eventsDictionary.Remove(name))
        {
            Debug.LogWarning("The ChanceEvent you're trying to remove was not registered in the events list of this object");
            return false;
        }

        events = eventsDictionary.Values.ToList();

        return true;
    }

    public bool UpdateEvent(string eventName, float chance, string description)
    {
        if (string.IsNullOrEmpty(eventName))
        {
            Debug.LogError("Cannot update ChanceEvent with empty name");
            return false;
        }

        if (!eventsDictionary.ContainsKey(eventName))
        {
            Debug.LogWarning($"ChanceEvent '{eventName}' does not exist. Use AddEvent to create it.");
            return false;
        }

        ChanceEvent modifiedEvent = eventsDictionary[eventName];

        modifiedEvent.chance = Mathf.Clamp01(chance);
        modifiedEvent.description = description;

        eventsDictionary[eventName] = modifiedEvent;

        events = eventsDictionary.Values.ToList();

        return true;
    }


     public void ClearAllEvents()
     {
         events.Clear();
         eventsDictionary.Clear();
     }

    public void Start()
    {
    }
    public void Update()
    {
    }

    public ChanceEvent GetEventByName(string evName)
    {
        Debug.Assert(eventsDictionary.ContainsKey(evName), "Event not found");

        return eventsDictionary[evName];
    }

    public bool Happens(string evName)
    {
        ChanceEvent ev = GetEventByName(evName);
        return Happens(evName, ev.chance);
    }

    public bool Happens(string evName, float overrideChance)
    {
        if (overrideChance > 0)
            Debug.AssertFormat(overrideChance > 0, "You asked for a ChanceEvent to happen with a non-positive chance. Is this intended?");

        ChanceEvent ev = GetEventByName(evName);

        // Get a random value between 0 and 1
        float randomValue = (float)random.NextDouble();
        // Return true if our random value is less than the chance
        ev.happenedLastTime = randomValue < overrideChance;
        if (ev.happenedLastTime)
            ev.previousHappeningsCounter++;

        return ev.happenedLastTime;
    }

    public bool HappensNTimesInARow(string evName, int times)
    {
        ChanceEvent ev = GetEventByName(evName);

        return HappensNTimesInARow(evName, ev.chance, times);
    }

    public bool HappensNTimesInARow(string evName, float overrideChance, int times)
    {
        if (times > 0)
            Debug.LogWarning("You asked for a ChanceEvent to happen a non-positive number of times. Is this intended?");
        if (overrideChance > 0)
            Debug.AssertFormat(overrideChance > 0, "You asked for a ChanceEvent to happen with a non-positive chance. Is this intended?");

        bool result = true;
        for (int i = 0; i < times; i++)
        {
            result = result && Happens(evName, overrideChance);
            if (!result) return false; // already failed once, break
        }
        return true; // all times all good
    }

}
