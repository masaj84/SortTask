using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltiumTask.Utils
{
    public class Util
    {
        public static void CreatePath(string filePath)
        {
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
                Console.WriteLine($"Dir {filePath} has been created");
            }
        }
    }
}
