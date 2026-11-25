using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;
using System.Xml;
using System.IO.Pipes;
using System.Xml.Linq;

namespace StoneToolkit.Common.Utilities
{
    public static class XmlSerializerUtility
    {
        public static bool SaveObjectToFile(string fileName, object @object)
        {
            try
            {
                using (FileStream fileStream = new FileStream(fileName, FileMode.Create))
                {
                    XmlWriterSettings settings = new XmlWriterSettings();
                    settings.Indent = true;
                    //settings.IndentChars = ("\t");
                    settings.OmitXmlDeclaration = true;
                    settings.NewLineOnAttributes = false;

                    XmlSerializerNamespaces xsn = new XmlSerializerNamespaces();
                    xsn.Add(String.Empty, String.Empty);

                    using (XmlWriter writer = XmlWriter.Create(fileStream, settings))
                    {
                        XmlSerializer xmlSerializer = new XmlSerializer(@object.GetType());
                        // Serialize using the XmlTextWriter.
                        xmlSerializer.Serialize(writer, @object);
                    }
                }
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }

        public static T LoadObjectFromFile<T>(string fileName)
        {
            try
            {
                T result = default(T);
                using (Stream fs = new FileStream(fileName, FileMode.Open))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(T));
                    result = (T)serializer.Deserialize(fs);
                }

                return result;
            }
            catch (Exception e)
            {

            }

            return default(T);
        }

        public static string SerializeToXml<T>(T @object)
        {
            try
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    XmlWriterSettings settings = new XmlWriterSettings();
                    settings.Indent = true;
                    //settings.IndentChars = ("\t");
                    settings.OmitXmlDeclaration = true;
                    settings.NewLineOnAttributes = false;
                    settings.Encoding = new UTF8Encoding(false); // 禁用 BOM, BOM 的十六进制表示是 EF BB BF，它标识了文件的编码格式。

                    XmlSerializerNamespaces xsn = new XmlSerializerNamespaces();
                    xsn.Add(String.Empty, String.Empty);

                    using (XmlWriter writer = XmlWriter.Create(memoryStream, settings))
                    {
                        XmlSerializer xmlSerializer = new XmlSerializer(@object.GetType());
                        // Serialize using the XmlTextWriter.
                        //xmlSerializer.Serialize(writer, @object);
                        xmlSerializer.Serialize(writer, @object, xsn);
                    }

                    byte[] bytes = memoryStream.ToArray();
                    return System.Text.Encoding.UTF8.GetString(bytes);
                }
            }
            catch (Exception e)
            {
            }

            return "";

            //using (MemoryStream ms = new MemoryStream())
            //{
            //    DataContractSerializer serializer = new DataContractSerializer(typeof(T));
            //    serializer.WriteObject(ms, obj);
            //    return System.Text.Encoding.UTF8.GetString(ms.ToArray());
            //}
        }

        public static T DeserializeFromXml<T>(string xmlData)
        {
            using (MemoryStream ms = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(xmlData)))
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                return (T)xmlSerializer.Deserialize(ms);

                //DataContractSerializer serializer = new DataContractSerializer(typeof(T));
                //return (T)serializer.ReadObject(ms);
            }
        }

        public static void SerializeAndEncryptToFile<T>(string filePath, T obj)
        {
            // Step 1: Serialize the object to XML string
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            string xmlString;
            using (StringWriter stringWriter = new StringWriter())
            {
                serializer.Serialize(stringWriter, obj);
                xmlString = stringWriter.ToString();
            }

            // Step 2: Encrypt the XML string using AES
            string encrypteXml = EncryptionHelper.Encrypt(xmlString);

            // Step 3: Write the encrypted data to a file
            File.WriteAllText(filePath, encrypteXml);
        }

        public static T DecryptAndDeserializeFromFile<T>(string filePath)
        {
            // Step 1: Read the encrypted data from the file
            string encrypteXml = File.ReadAllText(filePath);

            // Step 2: Decrypt the data
            string xmlString = EncryptionHelper.Decrypt(encrypteXml);

            // Step 3: Deserialize the decrypted XML back into an object
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (StringReader stringReader = new StringReader(xmlString))
            {
                return (T)serializer.Deserialize(stringReader);
            }
        }
    }
}
