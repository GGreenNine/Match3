using System;
using System.IO;
using Data;
using Newtonsoft.Json;
using QFSW.QC;
using UnityEngine;

namespace Loaders
{
    public class StateSaveLoader
    {
        private string filePath = Application.dataPath + "/TilemapsData.json";
        
        public void SaveState(in State state)
        {
            var stateSerialized = JsonConvert.SerializeObject(state);
            File.WriteAllText(filePath, stateSerialized);
        }

        public State LoadState()
        {
            if (!File.Exists(filePath))
            {
                throw new Exception("File not found");
            }
            string json = File.ReadAllText(filePath);
            State state = JsonConvert.DeserializeObject<State>(json);
            return state;
        }
    }
}