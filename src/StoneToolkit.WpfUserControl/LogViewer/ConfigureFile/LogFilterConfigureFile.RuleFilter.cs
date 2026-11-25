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
        private XmlNode SelectLogRuleFiltersNode(XmlDocument xmlDocument)
        {
            XmlNode ioChannelsNode = xmlDocument.SelectSingleNode("/LogFilters/RuleFilters");
            return ioChannelsNode;
        }

        #region CURD

        public Dictionary<string, object> GetLogRuleFilterNameMap()
        {
            XmlDocument xmlDocument = GetXmlDocument();

            XmlNode logRuleFiltersNode = SelectLogRuleFiltersNode(xmlDocument);
            if (logRuleFiltersNode != null)
            {
                Dictionary<string, object> nameMap = new Dictionary<string, object>();
                foreach (XmlNode logRuleFilterNode in logRuleFiltersNode.ChildNodes)
                {
                    string childName = logRuleFilterNode.Attributes["Name"].Value;
                    nameMap.Add(childName, null);
                }

                return nameMap;
            }

            return null;
        }

        public LogRuleFilterCollection LoadLogRuleFilters()
        {
            XmlDocument xmlDocument = GetXmlDocument();

            XmlNode logRuleFiltersNode = SelectLogRuleFiltersNode(xmlDocument);
            if (logRuleFiltersNode != null)
            {
                LogRuleFilterCollection logRuleFilters = new LogRuleFilterCollection();
                foreach (XmlNode logRuleFilterNode in logRuleFiltersNode.ChildNodes)
                {
                    LogRuleFilter logRuleFilter = ReadLogRuleFilter(logRuleFilterNode);
                    logRuleFilters.Add(logRuleFilter);
                }

                return logRuleFilters;
            }

            return null;
        }

        public LogRuleFilter LoadLogRuleFilter(string name)
        {
            XmlDocument xmlDocument = GetXmlDocument();

            XmlNode logRuleFiltersNode = SelectLogRuleFiltersNode(xmlDocument);
            foreach (XmlNode logRuleFilterNode in logRuleFiltersNode.ChildNodes)
            {
                LogRuleFilter logRuleFilter = ReadLogRuleFilter(logRuleFilterNode);
                if (logRuleFilter.Name == name)
                {
                    return logRuleFilter;
                }
            }

            return null;
        }

        public void AddLogRuleFilter(LogRuleFilter logRuleFilter)
        {
            XmlDocument xmlDocument = GetXmlDocument();

            XmlNode logRuleFiltersNode = SelectLogRuleFiltersNode(xmlDocument);
            WriteLogRuleFilter(logRuleFilter, logRuleFiltersNode);

            xmlDocument.Save(ConfigFileName);
        }

        public void RemoveLogRuleFilter(string name)
        {
            XmlDocument xmlDocument = GetXmlDocument();

            XmlNode logRuleFiltersNode = SelectLogRuleFiltersNode(xmlDocument);
            foreach (XmlNode logRuleFilterNode in logRuleFiltersNode.ChildNodes)
            {
                LogRuleFilter logRuleFilter = ReadLogRuleFilter(logRuleFilterNode);
                if (logRuleFilter.Name == name)
                {
                    logRuleFilterNode.ParentNode.RemoveChild(logRuleFilterNode);
                }
            }

            xmlDocument.Save(ConfigFileName);
        }

        public void UpdateLogRuleFilter(LogRuleFilter logRuleFilter)
        {
            XmlDocument xmlDocument = GetXmlDocument();

            XmlNode logRuleFiltersNode = SelectLogRuleFiltersNode(xmlDocument);
            WriteLogRuleFilter(logRuleFilter, logRuleFiltersNode);

            XmlNode lastChildNode = logRuleFiltersNode.LastChild;
            foreach (XmlNode logRuleFilterNode in logRuleFiltersNode.ChildNodes)
            {
                LogRuleFilter logRuleFilter1 = ReadLogRuleFilter(logRuleFilterNode);
                if (logRuleFilterNode != lastChildNode && logRuleFilter1.Name == logRuleFilter.Name)
                {
                    logRuleFiltersNode.RemoveChild(lastChildNode);
                    logRuleFiltersNode.ReplaceChild(lastChildNode, logRuleFilterNode);
                    break;
                }
            }

            xmlDocument.Save(ConfigFileName);
        }

        public void UpdateLogRuleFilters(LogRuleFilterCollection logRuleFilters)
        {
            XmlDocument xmlDocument = GetXmlDocument();

            XmlNode logRuleFiltersNode = SelectLogRuleFiltersNode(xmlDocument);
            logRuleFiltersNode.RemoveAll();
            foreach (LogRuleFilter logRuleFilter in logRuleFilters)
            {
                WriteLogRuleFilter(logRuleFilter, logRuleFiltersNode);
            }

            xmlDocument.Save(ConfigFileName);
        }

        #endregion

        #region ReadWriteObject

        public static void WriteLogRuleFilter(LogRuleFilter logRuleFilter, XmlNode xmlNode)
        {
            XmlElement xmlElement = xmlNode.OwnerDocument.CreateElement("LogRuleFilter");

            SerializationHelper.WriteProperty(xmlElement, "Name", logRuleFilter.Name);
            SerializationHelper.WriteProperty(xmlElement, "Enabled", logRuleFilter.Enabled);
            SerializationHelper.WriteProperty(xmlElement, "Expression", logRuleFilter.Expression);
            SerializationHelper.WriteProperty(xmlElement, "Description", logRuleFilter.Description);

            xmlNode.AppendChild(xmlElement);
        }

        public static LogRuleFilter ReadLogRuleFilter(XmlNode xmlNode)
        {
            LogRuleFilter logRuleFilter = new LogRuleFilter();
            logRuleFilter.Name = SerializationHelper.ReadProperty(xmlNode, "Name", string.Empty);
            logRuleFilter.Enabled = SerializationHelper.ReadProperty(xmlNode, "Enabled", false);
            logRuleFilter.Expression = SerializationHelper.ReadProperty(xmlNode, "Expression", string.Empty);
            logRuleFilter.Description = SerializationHelper.ReadProperty(xmlNode, "Description", string.Empty);

            return logRuleFilter;
        }

        #endregion
    }
}
