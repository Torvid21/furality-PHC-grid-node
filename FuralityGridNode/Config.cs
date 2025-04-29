using System.Diagnostics;
using System.IO;
using System.Text.Json;

namespace FuralityGridNode
{
    public class Config
    {
        public string filepath = "basic.frig";
        public enum ConfigStatus
        {
            NoConfig,
            FileNotFound,
            Error,
            Reading,
            Read,
            Closed,
            MalformedFile,
        }
        public ConfigStatus status;

        public Config()
        {
            status = ConfigStatus.NoConfig;
        }

        public void LoadConfiguration(string path)
        {
            try
            {
#if DEBUG
                Trace.WriteLine("Reading FRig File");
#endif
                filepath = path;
                string jsonString = File.ReadAllText(path);
                status = ConfigStatus.Reading;
                if (jsonString.Length > 0)
                {
                    status = ConfigStatus.Read;
                } else
                {
                    status = ConfigStatus.Error;
                }
            } catch (FileNotFoundException)
            {
#if DEBUG
                Trace.WriteLine("Oops file not found, how?");
#endif
                status = ConfigStatus.FileNotFound;
            } catch (JsonException)
            {
#if DEBUG
                Trace.WriteLine("Json Exception");
#endif
                status = ConfigStatus.MalformedFile;
            }
        }
    }
}
