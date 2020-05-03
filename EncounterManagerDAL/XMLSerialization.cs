// Albin Karlsson 2019-01-12

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using EncounterManager;

namespace EncounterManagerDAL
{
    public class XMLSerialization
    {
        public Monster Monster { get; set; }

        /// <summary>
        /// Method taking a Monster and saving it as an XML-file
        /// </summary>
        /// <param name="adventuringGroups"></param>
        public void SaveMonster(Monster monsterToSave)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "XML File|*.xml";
            saveFileDialog.Title = "Save an XML File";
            bool? dialogResult = saveFileDialog.ShowDialog();

            if ((bool)dialogResult)
            {
                Monster = monsterToSave;

                XmlSerializer writer = new XmlSerializer(typeof(Monster));
                FileStream file = File.Create(saveFileDialog.FileName); // Filepath
                writer.Serialize(file, Monster);
                file.Close();
            }
        }

        /// <summary>
        /// Method opening an XML-file and assigning the contents (Monster) to a loadedMonster variable in AddParticipantWindow
        /// </summary>
        public void LoadMonster()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "XML File|*.xml";
            openFileDialog.Title = "Open an XML File";
            bool? dialogResult = openFileDialog.ShowDialog();

            if ((bool)dialogResult)
            {
                XmlSerializer reader = new XmlSerializer(typeof(Monster));
                StreamReader file = new StreamReader(openFileDialog.FileName);
                Monster = (Monster)reader.Deserialize(file);
                file.Close();
            }
        }
    }
}
