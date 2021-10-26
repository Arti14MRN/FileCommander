using System;
using System.IO;
using System.IO.Compression;
using System.Xml.Serialization;
using System.Text.Json;

namespace FileManager
{
    class Program
    {
        static void Main(string[] args)
        {
            

            ///описание команд через switch и передача аргумента в методы
            #region switchs
            do
            {
            Console.WriteLine("Welcome to FileManager");
            Console.WriteLine("write |help| for FAQ");
            Console.Write("input command:");
            string command = Console.ReadLine().ToLower();
            string[] words = command.Split(new char[] { ' ' });
            foreach (string comm in words)
            {
                    switch (comm)
                    {
                        case "lastpos":
                            lastpos();
                            break;
                        case "help":
                            help();
                            break;
                        case "ls":
                             ls(words[1]);
                            SavePosition(words[1]);
                            break;
                        case "cpfile":
                            cpfile(words[1], words[2]);
                            break;
                        case "cpdir":
                            cpdir(words[1], words[2]);
                            Console.WriteLine("cpdir");
                            break;
                        case "delfile":
                            delfile(words[1]);
                            break;
                        case "deldir":
                            deldir(words[1]);
                            break;
                        case "createdir":
                            createdir(words[1]);
                            break;
                        case "movedir":
                            movedir(words[1], words[2]);
                            break;
                        case "fileinfo":
                            fileinfo(words[1]);
                            break;
                        case "foldersize":
                            try
                            {
                                DirectoryInfo di = new DirectoryInfo(words[1]);
                                DirectoryInfo[] dirs = di.GetDirectories();
                                foreach (DirectoryInfo dir in dirs)
                                {
                                    Console.WriteLine(dir.Name.PadRight(30, ' ') + foldersize(dir).ToString().PadLeft(12, ' '));
                                }
                                FileInfo[] files = di.GetFiles();
                                foreach (FileInfo file in files)
                                {
                                    Console.WriteLine(file.Name.PadRight(30, ' ') + file.Length.ToString().PadLeft(12, ' '));
                                }
                            }
                            catch (Exception e)
                            {
                                ErrorsLog(e.ToString());
                            };
                            break;
                        case "readfile":
                            readfile(words[1]);
                            break;
                        case "zip":
                            zip(words[1], words[2]);
                            break;
                        default:
                            Console.WriteLine("unknow command");
                            break;
                            
                    }
                }
                Console.ReadKey();
                Console.Clear();
            }
            
            while (true) ;
            #endregion
            ///вызов методов
            #region methods

            static void lastpos()
            {
                ///загрузка последнего состояния
                FileInfo fileinfo = new FileInfo("lastposition.txt");

                if (!fileinfo.Exists)
                {
                    Console.WriteLine("lastposition.txt not found");
                }
                else
                {
                    string line;
                    using (StreamReader sr = new StreamReader("lastposition.txt", System.Text.Encoding.Default))
                    {

                        while ((line = sr.ReadLine()) != null)
                        {
                            Console.WriteLine("last position " + line);
                        }
                    }
                    
                }
                    
                }
            }
            static void help ()
            {
            Console.Clear();
            Console.WriteLine(@"command...path...path2");
            Console.WriteLine(@"----------------------");
            Console.WriteLine(@"lastpos returen last folder");
                Console.WriteLine(@"ls c:\temp");
                Console.WriteLine(@"cpfile c:\temp\1.txt c:\temp\2.txt");
                Console.WriteLine(@"cpdir source destination");
                Console.WriteLine(@"delfile source");
                Console.WriteLine(@"deldir c:\temp");
                Console.WriteLine(@"createdir c:\temp");
                Console.WriteLine(@"movedir c:\temp c:\temp2");
                Console.WriteLine(@"fileinfo c:\temp\1.txt");
                Console.WriteLine(@"foldersize c:\temp");
                Console.WriteLine(@"readfile c:\temp\1.txt");
                Console.WriteLine(@"zip c\temp c:\temp.zip");

            }
            ///просмотр файлов в папке
            static void ls (string source)
            {
                SavePosition(source);
                int i;
                var counter = 0;
                Console.Write("input Number of items on page:");
                var choosenumber = Console.ReadLine();
                int num;
                bool isNumber = int.TryParse(choosenumber, out num);
                try
                {
                    foreach (var file in Directory.GetFileSystemEntries(source))

                        for (i = 0; i < file.Length; i++)
                        {
                            Console.WriteLine(file);
                            counter++;
                            if (counter % num == 0)
                            {
                                Console.WriteLine("press any key for next page");
                                var input = Console.ReadKey();
                                switch (input.Key)
                                {
                                    case ConsoleKey.Spacebar:
                                        continue;
                                    case ConsoleKey.Enter:
                                        help();
                                        break;
                                }
                                
                                
                            }
                        }

                }
                catch (DirectoryNotFoundException e)
                {
                    Console.WriteLine("directory not found");
                    ErrorsLog(e.ToString());
                    
                }
                
            }
            ///копирование файла
            static void cpfile(string source, string destination)
            {
                FileInfo fileinfo = new FileInfo(source);

                if (fileinfo != null)
                {
                    try
                    {
                        File.Copy(source, destination);
                    }
                    catch (FileNotFoundException e)
                    {
                        Console.WriteLine("file not found");
                        ErrorsLog(e.ToString());

                    }
                    catch (DirectoryNotFoundException e)
                    {
                        Console.WriteLine("directory not found");
                        ErrorsLog(e.ToString());
                    }
                }

            }
            ///копирование директории
            static void cpdir(string source, string destination)
            {
                DirectoryInfo dir = new DirectoryInfo(source);
                DirectoryInfo[] dirs = dir.GetDirectories();
                if (!Directory.Exists(destination))
                {
                    Directory.CreateDirectory(destination);
                }
                FileInfo[] files = dir.GetFiles();
                foreach (FileInfo file in files)
                {
                    string temppath = Path.Combine(destination, file.Name);
                    file.CopyTo(temppath, true);
                }

                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(source, subdir.Name);
                    cpdir(subdir.FullName, temppath);
                }
            }
            ///удаление файла
            static void delfile (string source)
            {
                if (File.Exists(source))
                {
                    File.Delete(source);
                }
                else
                Console.WriteLine(source + " not found");
            }
            ///удаление директории
            static void deldir(string source)
            {
                try
                {
                    Directory.Delete(source, true);
                }
                catch (DirectoryNotFoundException e)
                {
                    Console.WriteLine("directory not found");
                    ErrorsLog(e.ToString());
                }

            }
            ///создание директории
            static void createdir(string source)
            {
                try
                {
                    DirectoryInfo dir = new DirectoryInfo(source);
                    if (!dir.Exists)
                    {
                        Directory.CreateDirectory(source);
                    }
                    else
                    {
                        Console.WriteLine("directory already live");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    ErrorsLog(e.ToString());
                }
            }
            ///миграция директории
            static void movedir(string source, string destination)
            {
                try
                {
                    DirectoryInfo dir = new DirectoryInfo(source);
                    if (dir.Exists)
                    {
                        Directory.Move(source, destination);
                    }
                    else
                        Console.WriteLine("folder not found");

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    ErrorsLog(e.ToString());
                }
            }
            ///инфа о файле
            static void fileinfo(string source)
            {
                FileInfo fileinfo = new FileInfo(source);
                if (fileinfo.Exists)
                {
                    Console.WriteLine($"{fileinfo.Name} {fileinfo.Length} byte {fileinfo.CreationTime}");
                }
                else
                {
                    Console.WriteLine("file not found");
                }
                

            }
            ///размер директории
            static long foldersize(DirectoryInfo source)
            {
                    long folderSize = 0;
                    DirectoryInfo[] dirs = source.GetDirectories();
                    foreach (DirectoryInfo dir in dirs)
                    {
                        folderSize += foldersize(dir);
                    }

                    FileInfo[] files = source.GetFiles();
                    foreach (FileInfo file in files)
                    {
                        folderSize += file.Length;
                    }

                    return folderSize;
                }
            ///чтение файла
            static void readfile (string source)
            {

                string data;
                FileInfo fileinfo = new FileInfo(source);
                if (fileinfo.Exists)
                {
                    data = File.ReadAllText(source);
                    Console.WriteLine(data);
                }
                else
                {
                    Console.WriteLine("file not found");
                }
            }
            ///архиварование
            static void zip(string source, string destination)
            {
                FileInfo fileinfo = new FileInfo(source);
                if (fileinfo.Exists)
                {
                    ZipFile.CreateFromDirectory(source, destination);
                }
                else
                {
                    Console.WriteLine("file not found");
                }
            }
            ///запись лог ошибок
            static void ErrorsLog(string error)
            {
                DirectoryInfo dir = new DirectoryInfo("errors");
                if (dir.Exists)
                {
                    using (StreamWriter sw = File.AppendText(@".\errors/random_name_exception.txt"))
                    {
                        var time = DateTime.Now.ToString("HH:mm:ss tt");
                        sw.WriteLine($" {time} {error}");
                    }
                }
                else 
                {
                    Directory.CreateDirectory("errors");
                    ErrorsLog(error);
                }

            }
            ///запись последней команды просмотра каталога c автозагрузкой
            static void SavePosition(string pos)
            {
                using (StreamWriter sw = new StreamWriter("lastposition.txt", false, System.Text.Encoding.Default))
                {
                    sw.WriteLine(pos);
                }
            }
            #endregion

            ///class error
            ///class methods
            ///class logs
            ///json or xml
        }
    } 



