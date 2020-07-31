using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Helper
{
    public class FileHelper
    {
        public static List<string> GetFolderAllChilden(string folderPath)
        {
            if (!Directory.Exists(folderPath)) return null;

            var data = Directory.GetFiles(folderPath).ToList();
            foreach (var directory in Directory.GetDirectories(folderPath))
            {
                var temp = GetFolderAllChilden(directory);
                if (temp == null) continue;

                data.AddRange(temp);
            }

            return data;
        }
    }
}