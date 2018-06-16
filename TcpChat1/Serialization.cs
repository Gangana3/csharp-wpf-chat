using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace TcpChat1
{
    /// <summary>
    /// Implements useful functions for working with binary serialization
    /// </summary>
    public class BinarySerialization
    {

        /// <summary>
        /// Writes an object to the given stream
        /// </summary>
        /// <typeparam name="T">Type of the object to write</typeparam>
        /// <param name="stream">Stream to write to</param>
        /// <param name="objectToWrite">Object that should be written</param>
        public static void WriteObjectToStream<T>(Stream stream, T objectToWrite)
        {
            var binaryFormatter = new BinaryFormatter();
            binaryFormatter.Serialize(stream, objectToWrite);
        }


        /// <summary>
        /// Saves an object to a file
        /// </summary>
        /// <typeparam name="T">Type of the object to save</typeparam>
        /// <param name="filePath">Path to the destination file</param>
        /// <param name="objectToSave">Object that should be saved</param>
        /// <param name="append">Whether should append or rewrite the file</param>
        public static void SaveObjectToFile<T>(string filePath, T objectToSave, bool append = false)
        {
            FileMode mode;
            if (!File.Exists(filePath))
                mode = FileMode.Create;  // Create file if does not exist
            else if (append)
                mode = FileMode.Append;
            else
                mode = FileMode.Open;

            using (Stream fileStream = File.Open(filePath, mode, FileAccess.Write))
                WriteObjectToStream<T>(fileStream, objectToSave);
        }


        /// <summary>
        /// Retreive an object from the given stream
        /// </summary>
        /// <typeparam name="T">Type of the object to be retrieved</typeparam>
        /// <param name="stream">Stream to retreive from</param>
        /// <returns>Object from the given stream</returns>
        public static T RetreiveObjectFromStream<T>(Stream stream)
        {
            var binaryFormatter = new BinaryFormatter();
            return (T)binaryFormatter.Deserialize(stream);
        }


        /// <summary>
        /// Retreives an object from the given file
        /// </summary>
        /// <typeparam name="T">Type of the object to retreive</typeparam>
        /// <param name="filePath">Path to the destination file</param>
        /// <returns>Object from the given file</returns>
        public static T RetreiveObjectFromFile<T>(string filePath)
        {
            using (Stream fileStream = File.Open(filePath, FileMode.Open, FileAccess.Read))
                return RetreiveObjectFromStream<T>(fileStream);
        }
    }
}
