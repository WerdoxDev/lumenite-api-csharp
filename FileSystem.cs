using System.IO;
using System.Linq;

namespace FILE
{
	public class FileSystem
	{
		public static string defaultPath = "./";

		public static string[] ReadLines(string path)
		{
			return File.ReadLines(path).ToArray();
		}

		public static void WriteLines(string path, string[] lines)
		{
			File.WriteAllLines(path, lines);
		}

		public static bool Exists(string path)
		{
			return File.Exists(path);
		}

		public static void CreateFile(string path)
		{
			File.Create(path);
		}
	}
}
