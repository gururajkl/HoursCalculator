using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace HoursCalculator.Services
{
    public class FileService<T>
    {
        private XmlSerializer serializer = new(typeof(List<T>));

        public List<T> GetData(string fileName)
        {
            if (File.Exists(fileName))
            {
                using var stream = new FileStream(fileName, FileMode.Open);
                return serializer.Deserialize(stream) as List<T>;
            }
            return new List<T>();
        }

        public void SetData(string fileName, List<T> data)
        {
            using (var stream = new FileStream(fileName, FileMode.Create))
            {
                var serializer = new XmlSerializer(typeof(List<T>));
                serializer.Serialize(stream, data);
            }
        }
    }
}
