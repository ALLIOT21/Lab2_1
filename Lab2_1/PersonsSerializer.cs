using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Lab2_1
{
    public class PersonsSerializer
    {
        private const string _fileName = "persons.dat";

        public void SerializePersons(List<Person> persons) 
        {
            BinaryFormatter formatter = new BinaryFormatter();

            using (FileStream fs = new FileStream(_fileName, FileMode.OpenOrCreate))
            {
                formatter.Serialize(fs, persons);
            }
        }

        public List<Person> DeserializePersons()
        {
            BinaryFormatter formatter = new BinaryFormatter();

            var persons = new List<Person>();
            using (FileStream fs = new FileStream(_fileName, FileMode.OpenOrCreate))
            {
                if (fs.Length > 0)
                {
                    persons = (List<Person>)formatter.Deserialize(fs);
                }
            }

            return persons;
        }
    }
}
