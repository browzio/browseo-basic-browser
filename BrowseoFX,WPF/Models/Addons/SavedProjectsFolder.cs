using Delimon.Win32.IO;
using Newtonsoft.Json;
using Organiser.Common.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BrowseoFX_WPF.Models.Addons
{
    public class SavedModuleData<T>
    {
        public string Module { get; set; }
        public string ProjectName { get; set; }

        public T SavedData { get; set; }

        public SavedModuleData(string module, string projectName)
        {
            Module = module;
            ProjectName = projectName;
        }

        public async Task Load(string filepath)
        {
            string json = null;
            await Task.Run(() =>
            {
                try
                {
                    json = File.ReadAllText(filepath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(Module + " Couldn't load saved data. " + ex.Message);
                }
            });

            if (null == json) return;

            SavedData = JsonConvert.DeserializeObject<T>(json.ToLower());
        }

        public async Task Save(string filepath)
        {
            var jsonToSave = JsonConvert.SerializeObject(SavedData);

            await Task.Run(() =>
            {
                try
                {
                    if (jsonToSave.IsNullOrEmpty()) return;

                    if (File.Exists(filepath)) File.Delete(filepath);
                    File.WriteAllText(filepath, jsonToSave);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(Module + " Couldn't save. " + ex.Message);
                }
            });
        }

        public async Task DeleteSavedDataFolder(string dirPath)
        {
            if (!Directory.Exists(dirPath)) return;

            await Task.Run(()=>
            {
                Directory.Delete(dirPath,true);
            });
        }
    }
}
