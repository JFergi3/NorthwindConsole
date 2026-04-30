using NLog;
using NorthwindConsole.Services;

string path = Directory.GetCurrentDirectory() + "//nlog.config";

var logger = LogManager.Setup()
    .LoadConfigurationFromFile(path)
    .GetCurrentClassLogger();

logger.Info("Program started");

bool keepRunning;

do
{
    MenuService.DisplayMenu();

    string? choice = Console.ReadLine();

    keepRunning = MenuService.HandleMenuChoice(choice, logger);

} while (keepRunning);

logger.Info("Program ended");