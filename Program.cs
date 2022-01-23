﻿using System;
using System.IO;
using System.Text.Json;

namespace JsonArrayCopier

{
    class Program
    {
        static void Main(string[] args)
        {
            string infile1;
            string infile2;
            string outfile;
            if (!System.Diagnostics.Debugger.IsAttached)
            {
                if (args.Length != 3)
                {
                    Console.WriteLine("Invalid args");
                    return;
                }
                infile1 = args[0];
                infile2 = args[1];
                outfile = args[2];
            }
            else
            {
                var dir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                infile1 = Path.Combine(dir, "testfiles", "testfile1.json");
                infile2 = Path.Combine(dir, "testfiles", "testfile2.json");
                outfile = Path.Combine(dir, "testfiles", "testmerged.json");
            }
            var writer = new JsonArrayWriter(outfile);
            writer.Start();
            var totalItems = 0;
            totalItems += CopyArray(infile1, writer);
            totalItems += CopyArray(infile2, writer);
            var finalFile = writer.Finish();
            var finalFileInfo = new FileInfo(finalFile);
            Console.WriteLine($"Wrote {totalItems} items ({finalFileInfo.Length}) to {finalFile}");

        }

        // https://stackoverflow.com/questions/54983533/parsing-a-json-file-with-net-core-3-0-system-text-json
        private static int CopyArray(string inputFileName, JsonArrayWriter writer)
        {
            int itemscopied = 0;
            Console.WriteLine($"Copying array from {inputFileName} to {writer.fileName}");
            using var inStream = new FileStream(inputFileName, FileMode.Open, FileAccess.Read);
            using var jsonStreamReader = new Utf8JsonStreamReader(inStream, 32 * 1024);

            jsonStreamReader.Read(); // move to array start
            jsonStreamReader.Read(); // move to start of the object

            while (jsonStreamReader.TokenType != JsonTokenType.EndArray)
            {
                // deserialize object
                var obj = jsonStreamReader.Deserialize<object>();
                writer.Write(obj);
                itemscopied++;

                // JsonSerializer.Deserialize ends on last token of the object parsed,
                // move to the first token of next object
                jsonStreamReader.Read();
            }

            jsonStreamReader.Dispose();
            Console.WriteLine($"{itemscopied} items copied.");
            return itemscopied;
        }

    }
}