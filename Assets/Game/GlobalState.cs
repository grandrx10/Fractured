using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class GlobalState: MonoBehaviour
{
    public static GlobalState instance;
    public bool saveOnExit = true;
    private void Awake()
    {
        if (instance) Destroy(gameObject);
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            Load("Assets/Game/save.txt");
        }
    }

    private void OnDestroy()
    {
        if (saveOnExit)
        {
            Save("Assets/Game/save.txt");
        }
    }

    // ============================================================
    // Storage (Efficient dictionaries)
    // ============================================================

    public HashSet<string> events = new HashSet<string>();
    public Dictionary<string, int> ints = new Dictionary<string, int>();
    public Dictionary<string, string> strs = new Dictionary<string, string>();
    public Dictionary<string, List<int>> tups = new Dictionary<string, List<int>>();

    // ============================================================
    // EVT
    // ============================================================
    public void AddEvent(string name) => events.Add(name);
    public void RemoveEvent(string name) => events.Remove(name);
    public bool HasEvent(string name) => events.Contains(name);

    // ============================================================
    // INT
    // ============================================================
    public void SetInt(string key, int value) => ints[key] = value;
    public bool TryGetInt(string key, out int value) => ints.TryGetValue(key, out value);
    public void RemoveInt(string key) => ints.Remove(key);

    // ============================================================
    // STR
    // ============================================================
    public void SetStr(string key, string value) => strs[key] = value;
    public bool TryGetStr(string key, out string value) => strs.TryGetValue(key, out value);
    public void RemoveStr(string key) => strs.Remove(key);

    // ============================================================
    // TUP (List<int>)
    // ============================================================
    public void SetTup(string key, List<int> values) => tups[key] = new List<int>(values);

    public bool TryGetTup(string key, out List<int> values)
    {
        if (tups.TryGetValue(key, out var list))
        {
            values = new List<int>(list);
            return true;
        }
        values = null;
        return false;
    }

    public void RemoveTup(string key) => tups.Remove(key);

    /// <summary>
    /// Adds an integer to a TUP list **only if it isn't already present**.
    /// (TUP behaves like a set but keeps list semantics.)
    /// </summary>
    public void AddToIntListAsSet(string key, int value)
    {
        if (!tups.TryGetValue(key, out var list))
        {
            list = new List<int>();
            tups[key] = list;
        }

        if (!list.Contains(value))   // set behavior
            list.Add(value);
    }

    // ============================================================
    // SAVE
    // ============================================================
    public void Save(string path)
    {
        using StreamWriter writer = new StreamWriter(path);

        // EVT (no colon)
        foreach (var evt in events)
            writer.WriteLine($"EVT -- {evt}");

        // INT
        foreach (var kv in ints)
            writer.WriteLine($"INT -- {kv.Key} : {kv.Value}");

        // STR
        foreach (var kv in strs)
            writer.WriteLine($"STR -- {kv.Key} : {kv.Value}");

        // TUP (space-separated list of ints)
        foreach (var kv in tups)
        {
            string joined = string.Join(" ", kv.Value);
            writer.WriteLine($"TUP -- {kv.Key} : {joined}");
        }
    }

    // ============================================================
    // LOAD
    // ============================================================
    public void Load(string path)
    {
        events.Clear();
        ints.Clear();
        strs.Clear();
        tups.Clear();

        if (!File.Exists(path))
        {
            Debug.LogWarning($"GlobalState: file does not exist: {path}");
            return;
        }

        foreach (var rawLine in File.ReadAllLines(path))
        {
            if (string.IsNullOrWhiteSpace(rawLine))
                continue;

            string line = rawLine.Trim();

            // EVT format: EVT -- eventName
            if (line.StartsWith("EVT"))
            {
                var parts = line.Split(new[] { "--" }, StringSplitOptions.None);
                if (parts.Length >= 2)
                {
                    string evtName = parts[1].Trim();
                    events.Add(evtName);
                }
                continue;
            }

            // Everything else has a colon
            var parts2 = line.Split(new[] { "--", ":" }, StringSplitOptions.None);
            if (parts2.Length < 3) continue;

            string type = parts2[0].Trim();
            string name = parts2[1].Trim();
            string data = parts2[2].Trim();

            switch (type)
            {
                case "INT":
                    if (int.TryParse(data, out int iv))
                        ints[name] = iv;
                    break;

                case "STR":
                    strs[name] = data;
                    break;

                case "TUP":
                    if (string.IsNullOrWhiteSpace(data))
                    {
                        tups[name] = new List<int>();
                    }
                    else
                    {
                        tups[name] = data.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                                         .Select(int.Parse)
                                         .ToList();
                    }
                    break;
            }
        }
    }
}
