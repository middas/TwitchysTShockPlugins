using System;
using System.IO;
using Newtonsoft.Json;

namespace ChatBlocker
{
    public class CConfigFile
    {
        public string StartsWith = "/";
        public string Contains = "";
        public string BlockedWordMessage = "Blocked chat message due to blocked word being used";
        public string CommandsRedirect = "wp=warp,rg=region";

        public static CConfigFile Read(string path)
        {
            if (!File.Exists(path))
                return new CConfigFile();
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return Read(fs);
            }
        }

        public static CConfigFile Read(Stream stream)
        {
            using (var sr = new StreamReader(stream))
            {
                var cf = JsonConvert.DeserializeObject<CConfigFile>(sr.ReadToEnd());
                if (ConfigRead != null)
                    ConfigRead(cf);
                return cf;
            }
        }

        public void Write(string path)
        {
            using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Write))
            {
                Write(fs);
            }
        }

        public void Write(Stream stream)
        {
            var str = JsonConvert.SerializeObject(this, Formatting.Indented);
            using (var sw = new StreamWriter(stream))
            {
                sw.Write(str);
            }
        }

        public static Action<CConfigFile> ConfigRead;
    }
}