using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Overlayer.Utils
{
    public static class IOUtils
    {
        public static byte[] Zip(params RawFile[] rFiles)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (ZipArchive zipArchive = new ZipArchive(ms, ZipArchiveMode.Update, false, Encoding.UTF8))
                {
                    foreach (var rFile in rFiles)
                    {
                        var entry = zipArchive.CreateEntry(rFile.Name);
                        using (Stream entryStream = entry.Open())
                            entryStream.Write(rFile.Data, 0, rFile.Data.Length);
                    }
                }
                return ms.ToArray();
            }
        }
        public static List<RawFile> Unzip(byte[] zip)
        {
            List<RawFile> files = new List<RawFile>();
            using (MemoryStream ms = new MemoryStream(zip, 0, zip.Length, true, true))
            using (ZipArchive zipArchive = new ZipArchive(ms, ZipArchiveMode.Read, false, Encoding.UTF8))
            {
                foreach (var entry in zipArchive.Entries)
                {
                    using (Stream entryStream = entry.Open())
                    {
                        byte[] buffer = new byte[entryStream.Length];
                        entryStream.Read(buffer, 0, buffer.Length);
                        files.Add(new RawFile(entry.FullName, buffer));
                    }
                }
            }
            return files;
        }
    }
    public class RawFile
    {
        public string Name { get; }
        public byte[] Data { get; }
        public RawFile(string name, byte[] data)
        {
            Name = name;
            Data = data;
        }
    }
}
