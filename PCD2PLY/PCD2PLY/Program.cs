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
        static void Main(string[] args)
        {
            string plyFilePath = args[0];
            string pcdFilePath = args[1];

            if (File.Exists(pcdFilePath))
            {
                string[] allPCDFileLines = File.ReadAllLines(pcdFilePath);

                List<string> header = new List<string>();

                // read PCD header. 
                int lineIndex = 0;
                for (; lineIndex < allPCDFileLines.Length; lineIndex++)
                {
                    string line = allPCDFileLines[lineIndex];
                    if (Char.IsDigit(line[0]) || line[0] == '-')
                    {
                        break;
                    }
                    header.Add(line);
                }



                List<string> linesToWrite = new List<string>();
                for (int c = lineIndex; c < allPCDFileLines.Length; c++)
                {
                    double[] pcdLineParts = allPCDFileLines[c].Split(' ').Select(n => double.Parse(n)).ToArray();
                    if (pcdLineParts.Any(n => Math.Abs(n) > 6))
                    {
                        continue;
                    }
                    else
                    {
                        linesToWrite.Add($"{pcdLineParts[0]} {pcdLineParts[1]} {pcdLineParts[2]} 255 255 255");
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

                using (StreamWriter sw = File.CreateText(plyFilePath))
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
