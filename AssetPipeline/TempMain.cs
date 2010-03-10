using System.Xml;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;

namespace AssetPipeline
{
    public static class TempMain
    {
        public static void Main()
        {
            object testValue = new AssetSettingsContent();

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;

            using (XmlWriter xmlWriter = XmlWriter.Create("assetList.xml", settings))
            {
                IntermediateSerializer.Serialize(xmlWriter, testValue, null);
            }

        }
    }
}
