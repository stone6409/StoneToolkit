using StoneToolkit.Common.ConfigureFile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace StoneToolkit.WpfUserControl.LogViewer.ConfigureFile
{
    public partial class LogFilterConfigureFile : ConfigureFileBase
    {
        public LogFilterConfigureFile(String configFileName, bool ensureExist = false) : base(configFileName, ensureExist)
        {
        }

        protected override void OnCreateXmlDocument(XmlDocument xmlDocument)
        {
            XmlElement root = xmlDocument.CreateElement("LogFilters");
            xmlDocument.AppendChild(root);

            XmlElement levelFiltersNode = xmlDocument.CreateElement("LevelFilters");
            root.AppendChild(levelFiltersNode);

            XmlElement ruleFiltersNode = xmlDocument.CreateElement("RuleFilters");
            root.AppendChild(ruleFiltersNode);
        }
    }
}
