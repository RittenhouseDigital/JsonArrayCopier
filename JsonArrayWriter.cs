using System.IO;
using System.Text.Json;

namespace JsonArrayCopier
{
    public class JsonArrayWriter
    {
        public string fileName { get; set; }
        FileStream fileStream;
        Utf8JsonWriter writer;

        public JsonArrayWriter(string outputFileName)
        {
            this.fileName = outputFileName;
        }

        public void Write(object term)
        {
            JsonSerializer.Serialize(writer, term);
        }

        public void Start()
        {
            if (File.Exists(fileName)) File.Delete(fileName);
            fileStream = File.Create(fileName);
            var writerOptions = new JsonWriterOptions
            {
                Indented = false
            };
            writer = new Utf8JsonWriter(fileStream, options: writerOptions);
            writer.WriteStartArray();
        }

        public string Finish()
        {
            writer.WriteEndArray();
            writer.Flush();
            writer.Dispose();
            fileStream.Close();
            fileStream.Dispose();
            return fileName;
        }

    }
}