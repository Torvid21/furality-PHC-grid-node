using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FuralityGridNode
{
    public abstract class Fixture
    {
        public int Channel { get; set; }
        public int GridColor { get; set; }

        [JsonIgnore]
        public int totalPixelCount;

        [JsonPropertyName("artnetUniverse")]
        public int ArtnetUniverse { get; set; }

        [JsonPropertyName("artnetChannel")]
        public int ArtnetChannel { get; set; }

        [JsonPropertyName("name")]
        public string Name;
        [JsonIgnore]
        internal string ArtnetChannelStr;
        [JsonIgnore]
        internal int GridChannel;
        [JsonIgnore]
        internal int UnityChannel;

        public abstract void Draw(byte[] data, int dataSizeX, int dataSizeY, int X, int Y, int sizeX, int sizeY, ref byte[] output);

        public void DrawSquare(int dataSizeX, int dataSizeY, int X, int Y, int sizeX, int sizeY, byte R, byte G, byte B, ref byte[] output, int selector)
        {
            for (int x = X; x < X + sizeX; x++)
            {
                for (int y = Y; y < Y + sizeY; y++)
                {
                    int index = (x + y * dataSizeX) * 3;
                    switch (selector)
                    {
                        case 1:
                            output[index + 2] = R;
                            break;
                        case 2:
                            output[index + 1] = G;
                            break;
                        case 3:
                            output[index] = B;
                            break;
                        default:
                            output[index + 0] = B;
                            output[index + 1] = G;
                            output[index + 2] = R;
                            break;
                    }
                }
            }
        }
    }

    public class Data : Fixture
    {
        [JsonConstructor]
        public Data(int channel, int artnetChannel) {
            this.Channel = channel;
            this.ArtnetChannel = artnetChannel;
            this.totalPixelCount = 1;
        }

        [JsonConstructor]
        public Data()
        {
            this.totalPixelCount = 1;
        }

        public override void Draw(byte[] data, int dataSizeX, int dataSizeY, int X, int Y, int sizeX, int sizeY, ref byte[] output)
        {
            DrawSquare(dataSizeX, dataSizeY, X, Y, sizeX, sizeY, data[Channel], data[Channel], data[Channel], ref output, 0);
        }
    }

    public class Red : Fixture
    {
        [JsonConstructor]
        public Red(int channel, int artnetChannel) {
            this.Channel = channel;
            this.ArtnetChannel = artnetChannel;
            this.totalPixelCount = 0;
        }

        [JsonConstructor]
        public Red() {
            this.totalPixelCount = 0;
        }

        public override void Draw(byte[] data, int dataSizeX, int dataSizeY, int X, int Y, int sizeX, int sizeY, ref byte[] output)
        {
            byte r = data[ArtnetChannel];
            DrawSquare(dataSizeX, dataSizeY, X, Y, sizeX, sizeY, r, 0, 0, ref output, 1);
        }
    }
    public class Green: Fixture
    {
        [JsonConstructor]
        public Green(int channel, int artnetChannel) {
            this.Channel = channel;
            this.ArtnetChannel = artnetChannel;
            this.totalPixelCount = 0;
        }

        [JsonConstructor]
        public Green() {
            totalPixelCount = 0;
        }

        public override void Draw(byte[] data, int dataSizeX, int dataSizeY, int X, int Y, int sizeX, int sizeY, ref byte[] output)
        {
            byte r = data[ArtnetChannel];
            DrawSquare(dataSizeX, dataSizeY, X, Y, sizeX, sizeY, 0, r, 0, ref output, 2);
        }
    }
    public class Blue: Fixture
    {
        [JsonConstructor]
        public Blue(int channel, int artnetChannel) {
            this.Channel = channel;
            this.ArtnetChannel = artnetChannel;
            this.totalPixelCount = 1;
        }

        [JsonConstructor]
        public Blue()
        {
            this.totalPixelCount = 1;
        }
        public override void Draw(byte[] data, int dataSizeX, int dataSizeY, int X, int Y, int sizeX, int sizeY, ref byte[] output)
        {
            byte r = data[ArtnetChannel];
            DrawSquare(dataSizeX, dataSizeY, X, Y, sizeX, sizeY, 0, 0, r, ref output, 3);
        }
    }

    public class FixtureMap
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        
        [JsonPropertyName("artnetUniverse")]
        public string ArtnetUniverse { get; set; }

        [JsonPropertyName("artnetChannel")]
        public string ArtnetChannel { get; set; }

        [JsonPropertyName("gridChannel")]
        public string GridChannel { get; set; }

        [JsonPropertyName("gridColor")]
        public string GridColor { get; set; }

        [JsonPropertyName("unityChannel")]
        public string UnityChannel { get; set; }

        [JsonIgnore]
        public int DmxOffset => int.TryParse(ArtnetChannel, out int result) ? result - 1 : -1;

        [JsonIgnore]
        public int GridChannelIndex => int.TryParse(GridChannel, out int channel) ? channel : -1;
    }

    public class FRigFile
    {
        [JsonPropertyName("version")]
        public string Version;

        [JsonPropertyName("date")]
        public string Date;

        [JsonPropertyName("fixtures")]
        public List<FixtureMap> Fixtures { get; set; }

        public FRigFile LoadFromFile(string filepath)
        {
            try
            {
                string json = File.ReadAllText(filepath);
                return LoadFromJsonString(json);
            } catch (Exception e)
            {
                Trace.WriteLine($"Error: loading frig file: {e.Message}");
                return null;
            }
        }

        public FRigFile LoadFromJsonString(string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                Trace.WriteLine("Error: Cannot deserialize empty or null JSON");
                return null;
            }
            
            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    AllowTrailingCommas = true,
                };
                FRigFile rigFile = JsonSerializer.Deserialize<FRigFile>(json);
                Trace.WriteLine($"Successfully deserialized FRig: {rigFile}");
                if (rigFile.Fixtures == null)
                    Trace.WriteLine($"FRig no fixtures :/");
                Fixtures = rigFile.Fixtures;
                return rigFile;
            } catch (Exception e)
            {
                Trace.WriteLine($"Error deserializing: {e.Message}");
                return null;
            }
        }
        public FRig ConvertToFRig()
        {
            if (this.Fixtures == null)
            {
                Trace.WriteLine("Error: fixtures are null");
                return null;
            }

            var drawableFixtures = new List<Fixture>();
            foreach (var mapEntry in this.Fixtures)
            {
                Fixture fixture = null;
                int channel = 0;
                int artnetChannel = 0;
                switch (mapEntry.GridColor)
                {
                    case "0":
                        fixture = new Data(int.TryParse(mapEntry.GridChannel, out channel) ? channel : -1, int.TryParse(mapEntry.ArtnetChannel, out artnetChannel) ? artnetChannel : -1);
                        break;
                    case "1":
                        fixture = new Red(int.TryParse(mapEntry.GridChannel, out channel) ? channel : -1, int.TryParse(mapEntry.ArtnetChannel, out artnetChannel) ? artnetChannel : -1);
                        break;
                    case "2":
                        fixture = new Green(int.TryParse(mapEntry.GridChannel, out channel) ? channel : -1, int.TryParse(mapEntry.ArtnetChannel, out artnetChannel) ? artnetChannel : -1);
                        break;
                    case "3":
                        fixture = new Blue(int.TryParse(mapEntry.GridChannel, out channel) ? channel : -1, int.TryParse(mapEntry.ArtnetChannel, out artnetChannel) ? artnetChannel : -1);
                        break;
                    default:
                        Trace.WriteLine($"Warning: Unknown gridColor '{mapEntry.GridColor}' encountered for fixture '{mapEntry.Name}'. Skipping");
                        continue;
                }
                //Trace.WriteLine($"Convert to FRig: {mapEntry.Name} {fixture.GetType()}");

                fixture.Name = mapEntry.Name;
                int result = 0;
                fixture.ArtnetUniverse = int.TryParse(mapEntry.ArtnetUniverse, out result) ? result : -1;
                fixture.ArtnetChannel = int.TryParse(mapEntry.ArtnetChannel, out result) ? result  : -1;
                fixture.GridChannel = int.TryParse(mapEntry.GridChannel, out result) ? result : -1;
                fixture.UnityChannel = int.TryParse(mapEntry.UnityChannel, out result) ? result : -1;
                fixture.GridColor = int.TryParse(mapEntry.GridColor, out result) ? result : -1;

                drawableFixtures.Add(fixture);

            }

            FRig frig = new FRig
            {
                Fixtures = drawableFixtures.ToArray()
            };
            return frig;
        }
    }

    public class FRig
    {
        [JsonPropertyName("version")]
        public string Version { get; set; }

        [JsonPropertyName("fixtures")]
        public Fixture[] Fixtures { get; set; }
    }
}
