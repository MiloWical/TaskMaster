﻿using System.CommandLine;
// using Spectre.Console;

// AnsiConsole.Markup("[underline red]Hello[/] World!");

var fileOption = new Option<FileInfo?>(
            name: "--file",
            description: "The file to read and display on the console.");

var rootCommand = new RootCommand("Sample app for System.CommandLine");
rootCommand.AddOption(fileOption);

rootCommand.SetHandler((file) =>
    {
      ReadFile(file!);
    },
    fileOption);

return await rootCommand.InvokeAsync(args);

static void ReadFile(FileInfo file)
{
  File.ReadLines(file.FullName).ToList()
      .ForEach(line => Console.WriteLine(line));
}