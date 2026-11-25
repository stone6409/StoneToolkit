using StoneToolkit.Common.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace StoneToolkit.Common.Configuration
{
    public class ExeConfiguration
    {
        private bool _isAutoSave;

        public ExeConfiguration()
        {
            Configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        }

        public ExeConfiguration(string configFile, bool isAutoSave = false)
        {
            ExeConfigurationFileMap configFileMap = new ExeConfigurationFileMap();
            configFileMap.ExeConfigFilename = configFile;

            _isAutoSave = isAutoSave;

            Configuration = ConfigurationManager.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None);
        }

        public System.Configuration.Configuration Configuration { get; protected set; }

        // example: C:\Users\evan.qin\AppData\Roaming\WindingTool\WindingTool.config
        public static string GetDefaultConfigFile()
        {
            string applicationName = ApplicationUtility.ApplicationName;

            string configFileName = applicationName + ".config";
            string configFolderPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);
            string configFile = System.IO.Path.Combine(configFolderPath, applicationName, configFileName);

            return configFile;
        }

        public void Save()
        {
            try
            {
                Configuration.Save(ConfigurationSaveMode.Modified);
            }
            catch (Exception ex)
            {

            }
        }

        public void SaveAs(string configFile)
        {
            Configuration.SaveAs(configFile, ConfigurationSaveMode.Full);
        }

        public T GetValue<T>(string key)
        {
            string value = this[key];
            if (!string.IsNullOrWhiteSpace(value))
            {
                TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
                return (T)converter.ConvertFromInvariantString(value);
            }

            return default(T);
        }

        public T GetValue<T>(string key, T defaultValue)
        {
            string value = this[key];
            if (!string.IsNullOrWhiteSpace(value))
            {
                TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
                return (T)converter.ConvertFromInvariantString(value);
            }

            return defaultValue;
        }

        public void SetValue<T>(string key, T value)
        {
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
            string valueString = converter.ConvertToInvariantString(value);
            this[key] = valueString;
        }

        protected string this[string key]
        {
            get
            {
                KeyValueConfigurationElement keyValue = Configuration.AppSettings.Settings[key];
                if (keyValue != null)
                {
                    return keyValue.Value;
                }

                return null;
            }
            set
            {
                KeyValueConfigurationElement keyValue = Configuration.AppSettings.Settings[key];
                if (keyValue != null)
                {
                    keyValue.Value = value;
                }
                else
                {
                    Configuration.AppSettings.Settings.Add(key, value);
                }

                if (_isAutoSave)
                {
                    Save();
                }
            }
        }

        public string Environment
        {
            get
            {
                return this["Environment"];
            }
            set
            {
                this["Environment"] = value;
            }
        }

        // 开发环境
        public bool IsDevelopment
        {
            get
            {
                return string.Compare(Environment, "Development", true) == 0;
            }
        }

        // 预发布环境
        public bool IsStaging
        {
            get
            {
                return string.Compare(Environment, "Staging", true) == 0;
            }
        }

        // 生产环境
        public bool IsProduction
        {
            get
            {
                return string.IsNullOrEmpty(Environment) || string.Compare(Environment, "Production", true) == 0;
            }
        }
    }
}
