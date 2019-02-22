using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace OSRS_Hiscores_Checker
{
    [DataContract]
    class Config
    {
        [DataMember(Order = 0)]
        public string InputFile { get; set; } = "";
        [DataMember(Order = 1)]
        public string ProxiesFile { get; set; } = "";
        [DataMember(Order = 2)]
        public int Threads { get; set; } = 1;
        [DataMember(Order = 3)]
        public string OutputFile { get; set; } = "";
        [DataMember(Order = 4)]
        public string ErrorsFile { get; set; } = "";

        public Config()
        {
        }

        public static Config ParseFromJson(String Json)
        {
            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(Json));
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(Config));
            Config r = (Config)serializer.ReadObject(stream);
            stream.Close();
            return r;
        }

        public string ToJson()
        {
            MemoryStream stream = new MemoryStream();
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(Config));
            serializer.WriteObject(stream, this);
            stream.Position = 0;
            StreamReader streamReader = new StreamReader(stream);
            string json = streamReader.ReadToEnd();
            streamReader.Close();
            stream.Close();
            return json;
        }

        public override string ToString()
        {
            return $"InputFile: {InputFile}, ProxiesFile: {ProxiesFile}, Threads: {Threads}, OutputFile: {OutputFile}, ErrorsFile: {ErrorsFile}.";
        }
    }
}