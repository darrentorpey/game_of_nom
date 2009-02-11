using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Connecting.Utils
{
    class FileManager
    {
        public static string ReadLineFromFile(string fileName) {
            TextReader tr = new StreamReader(fileName);
            string line = tr.ReadLine();
            tr.Close();
            return line;
        }

        public static void WriteLineToFile(string fileName, string textToWrite)
        {
            TextWriter tw = new StreamWriter(fileName);
            tw.WriteLine(textToWrite);
            tw.Close();
        }
    }

//string directoryName = @"C:\TEMP\INTEGRA\";
//string fileName = directoryName + Integra.iUtilities.Text.SerialName(nameFile) + ".txt";

//if (!Directory.Exists(directoryName))
//{
//    Directory.CreateDirectory(directoryName);
//}

//if (!<b style="color:black;background-color:#99ff99">File</b>.Exists(fileName))
//{
//    // Create a <b style="color:black;background-color:#99ff99">file</b> to write to.
//    using (StreamWriter sw = <b style="color:black;background-color:#99ff99">File</b>.CreateText(fileName))
//    {
//        for (int i = 0; i < collectionLogMessages.Count; i++ )
//        {
//            sw.WriteLine(collectionLogMessages[i].Content);
//            Integra.iData.Operation.SystemAudit(collectionLogMessages[i].Content, Integra.iConfig.Audit.Types.Other.GetHashCode());
//        }
//    }
//}

}
