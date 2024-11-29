using Spectre.Console;
using TaskMaster.Core;

AnsiConsole.Clear();

var welcomeRule = new Rule("[blue]Task Master[/]");
welcomeRule.Justification = Justify.Left;
welcomeRule.RuleStyle("green4");
AnsiConsole.Write(welcomeRule);

AnsiConsole.WriteLine();

var inputFile = AnsiConsole.Prompt(
      new TextPrompt<string>("Scaffold filename [yellow underline]with path[/]:")
      //.Validate(path => File.Exists(path) ? ValidationResult.Success() : ValidationResult.Error("[red]File not found[/]"))
);

AnsiConsole.WriteLine();

var workItemSystem = AnsiConsole.Prompt(
    new SelectionPrompt<WorkItemSystem>()
        .Title("What's your [yellow]work item system[/]?")
        .AddChoices(Enum.GetValues<WorkItemSystem>()));

AnsiConsole.Clear();

var mainMenuRule = new Rule("[blue]Task Master - Main Menu[/]");
mainMenuRule.Justification = Justify.Left;
mainMenuRule.RuleStyle("green4");
AnsiConsole.Write(mainMenuRule);

AnsiConsole.WriteLine();

var mainMenuSelection = AnsiConsole.Prompt(
    new SelectionPrompt<string>()
        .Title("Options:")
        .AddChoices(["Exit"]));

AnsiConsole.Clear();