using CopyUnityDependApp;


namespace UnityCopyApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine($"参数为空,请输入一个路径.");
                return;
            }
            string sourcePath = args[0];
            string destinationPath = sourcePath + "_Copy";

            if (!Directory.Exists(sourcePath))
            {
                Console.WriteLine($"{sourcePath}不是一个目录");
                return;
            }

            var guid = System.Guid.NewGuid().ToString().Replace("-","");
            //var guid = GUID.Generate();
            Console.WriteLine($"Generated GUID: {guid}");

            //new UnityCopy();
            UnityCopy.CopyDirectoryDeep(sourcePath, destinationPath);
            Thread.Sleep(-1);
            Console.WriteLine("结束");

        }
    }
}
