using StoneToolkit.Common.ConfigureFile;
using StoneToolkit.WpfUserControl.LogViewer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace StoneToolkit.WpfUserControl.LogViewer.ConfigureFile
{
    public partial class LogFilterConfigureFile
    {
        private XmlNode SelectLogLevelFiltersNode(XmlDocument xmlDocument)
        {
            XmlNode ioChannelsNode = xmlDocument.SelectSingleNode("/LogFilters/LevelFilters");
            return ioChannelsNode;
        }

        #region CURD

        public LogLevelFilterList LoadLogLevelFilterList()
        {
            XmlDocument xmlDocument = GetXmlDocument();

            XmlNode logLevelFiltersNode = SelectLogLevelFiltersNode(xmlDocument);
            if (logLevelFiltersNode != null)
            {
                LogLevelFilterList logLevelFilterList = new LogLevelFilterList();

                foreach (XmlNode logLevelFilterNode in logLevelFiltersNode.ChildNodes)
                {
                    LogLevelFilter logLevelFilter1 = ReadLogLevelFilterList(logLevelFilterNode);

                    foreach (LogLevelFilter logLevelFilter in logLevelFilterList.LevelFilters)
                    {
                        if (logLevelFilter.Level == logLevelFilter1.Level)
                        {
                            logLevelFilter.Enabled = logLevelFilter1.Enabled;
                        }
                    }
                }

                return logLevelFilterList;
            }

            return null;
        }

        public void UpdateLogLevelFilters(LogLevelFilterList logLevelFilterList)
        {
            XmlDocument xmlDocument = GetXmlDocument();

            XmlNode logLevelFiltersNode = SelectLogLevelFiltersNode(xmlDocument);
            logLevelFiltersNode.RemoveAll();
            foreach (LogLevelFilter logLevelFilter in logLevelFilterList.LevelFilters)
            {
                WriteLogLevelFilterList(logLevelFilter, logLevelFiltersNode);
            }

            xmlDocument.Save(ConfigFileName);
        }

        #endregion

        #region ReadWriteObject

        public static void WriteLogLevelFilterList(LogLevelFilter logLevelFilter, XmlNode xmlNode)
        {
            XmlElement xmlElement = xmlNode.OwnerDocument.CreateElement("LogLevelFilter");

            SerializationHelper.WriteProperty(xmlElement, "Level", logLevelFilter.Level);
            SerializationHelper.WriteProperty(xmlElement, "Enabled", logLevelFilter.Enabled);

            xmlNode.AppendChild(xmlElement);
        }

        public static LogLevelFilter ReadLogLevelFilterList(XmlNode xmlNode)
        {
            LogLevelFilter logLevelFilter = new LogLevelFilter();
            logLevelFilter.Level = SerializationHelper.ReadProperty(xmlNode, "Level", string.Empty);
            logLevelFilter.Enabled = SerializationHelper.ReadProperty(xmlNode, "Enabled", true);

            return logLevelFilter;
        }

        #endregion
    }
}
