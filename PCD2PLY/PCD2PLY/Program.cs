using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCD2PLY
{
    class Program
    {
        enum InputFileType
        {
            UNKNOWN,
            PCD,
            OBJ
        }

        static void Main(string[] args)
        {
            string inputFile = args[0];

            if (File.Exists(inputFile))
            {
                string outputFilePath = Path.Combine(Path.GetDirectoryName(inputFile), Path.GetFileNameWithoutExtension(inputFile) + ".ply"); 

                InputFileType inputType = InputFileType.UNKNOWN;

                if (inputFile.EndsWith(".pcd"))
                {
                    inputType = InputFileType.PCD;
                }
                else if (inputFile.EndsWith(".obj"))
                {
                    inputType = InputFileType.OBJ;
                }

                string[] allFileLines = File.ReadAllLines(inputFile);

                List<string> header = new List<string>();

                List<string> linesToWrite = new List<string>();

                if (inputType == InputFileType.PCD)
                {
                    // read PCD header. 
                    int lineIndex = 0;
                    for (; lineIndex < allFileLines.Length; lineIndex++)
                    {
                        string line = allFileLines[lineIndex];
                        if (Char.IsDigit(line[0]) || line[0] == '-')
                        {
                            break;
                        }
                        header.Add(line);
                    }


                    for (int c = lineIndex; c < allFileLines.Length; c++)
                    {
                        double[] pcdLineParts = allFileLines[c].Split(' ').Select(n => double.Parse(n)).ToArray();
                        if (pcdLineParts.Any(n => Math.Abs(n) > 6))
                        {
                            continue;
                        }
                        else
                        {
                            linesToWrite.Add($"{pcdLineParts[0]} {pcdLineParts[1]} {pcdLineParts[2]} 255 255 255");
                        }
                    }
                }
                else if (inputType == InputFileType.OBJ)
                {
                    foreach(string line in allFileLines)
                    {
                        if(line.StartsWith("v "))
                        {
                            string[] bits = line.Split(' ');

                            linesToWrite.Add($"{bits[1]} {bits[2]} {bits[3]} 255 255 255"); 
                        }
                    }
                }

                const string PLYHEADERTEMPLATE = @"ply
                    format ascii 1.0
                    element vertex REPLACEME
                    property float x
                    property float y
                    property float z
                    property uchar red
                    property uchar green
                    property uchar blue
                    element face 0
                    property list uchar uint vertex_indices
                    end_header";
                string plyHeader = PLYHEADERTEMPLATE.Replace("REPLACEME", linesToWrite.Count.ToString());

                using (StreamWriter sw = File.CreateText(outputFilePath))
                {
                    sw.WriteLine(plyHeader);

                    foreach (string lineToWrite in linesToWrite)
                    {
                        sw.WriteLine(lineToWrite);
                    }
                }

                return;
            }
        }
    }
}
