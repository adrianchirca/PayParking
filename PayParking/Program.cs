using ConsoleTables;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PayParking.DTO;
using PayParking.Repositories;
using PayParking.Services;
using System;
using System.Collections.Generic;
using System.IO;

namespace PayParking
{
    class Program
    {
        public static IConfiguration Configuration { get; private set; }

        static void Main(string[] args)
        {

            Configuration = BuildConfig().Build();
            var host = CreateHostBuilder(args).Build();
            var _parkingService = host.Services.GetRequiredService<IParkingService>();

            List<MenuItem> menuItems = new List<MenuItem>();
            InitializeMenu(menuItems);

            var menu = GenerateDisplayMenu(menuItems);
            Console.Write("Your option: ");
            string selectedMenuOption = Console.ReadLine();
            SwithMenuItem(menu, selectedMenuOption, _parkingService);
        }

        #region ConfigMethods
        private static IConfigurationBuilder BuildConfig() =>
            new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        private static IHostBuilder CreateHostBuilder(string[] args) =>
             Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    services.AddScoped<IParkingRepository, ParkingRepository>();
                    services.AddScoped<IParkingService, ParkingService>();
                });
        #endregion

        #region InternalMethods
        private static void DisplayAppTitle()
        {
            var appTitle = new ConsoleTable("Pay Parking App");
            appTitle.Write(Format.Alternative);
        }

        private static ConsoleTable GenerateDisplayMenu(List<MenuItem> menuItems)
        {
            Console.Clear();
            DisplayAppTitle();
            var displayMenuOptions = new ConsoleTable("Menu option", "Value");

            menuItems.ForEach(x =>
            {
                displayMenuOptions.AddRow(x.Caption, x.MenuItemId);
            });

            displayMenuOptions.Write(Format.Alternative);
            Console.WriteLine("Welcome! Please choose an option by typing its coresponding value from the menu options above.\n");
            return displayMenuOptions;
        }

        private static void SwithMenuItem(ConsoleTable menu, string selectedMenuOption, IParkingService _parkingService)
        {
            string registrationNumber;
            switch (selectedMenuOption)
            {
                case string value when value == Constants.MenuItems.AvaiableParkingSpots.ToString():
                    _parkingService.GetNumberOfFreeParkingSpots();
                    InitializeNewCommand(menu, _parkingService);
                    break;
                case string value when value == Constants.MenuItems.DisplayCosts.ToString():
                    _parkingService.DisplayCosts();
                    InitializeNewCommand(menu, _parkingService);
                    break;
                case string value when value == Constants.MenuItems.EnterTheParking.ToString():
                    registrationNumber = GetUserConsoleInput("Please enter the car registration number. Please use the format BV10ABC: ");
                    _parkingService.AddNewParkingEntry(registrationNumber);
                    InitializeNewCommand(menu, _parkingService);
                    break;
                case string value when value == Constants.MenuItems.GetParkedCars.ToString():
                    _parkingService.GetParkedCars();
                    InitializeNewCommand(menu, _parkingService);
                    break;
                case string value when value == Constants.MenuItems.ExitTheParkingAndPay.ToString():
                    registrationNumber = GetUserConsoleInput("Please enter the car registration number: ");
                    _parkingService.ExitTheParking(registrationNumber);
                    InitializeNewCommand(menu, _parkingService);
                    break;
                case string value when value == Constants.MenuItems.ExitTheApp.ToString():
                    Console.WriteLine("Thank you for visiting us!");
                    break;
                default:
                    Console.WriteLine("Looks like your selection is not valid. Please select one of the avaiable menu options!\n");
                    InitializeNewCommand(menu, _parkingService);
                    break;
            }
        }

        private static void InitializeNewCommand(ConsoleTable menu, IParkingService _parkingService)
        {
            Console.Write("Your option: ");
            var newOption = Console.ReadLine();
            SwithMenuItem(menu, newOption, _parkingService);
        }

        private static void InitializeMenu(List<MenuItem> menuItems)
        {
            menuItems.Add(new MenuItem() { MenuItemId = Constants.MenuItems.AvaiableParkingSpots, Caption = "Check the avaiable parking spot" });
            menuItems.Add(new MenuItem() { MenuItemId = Constants.MenuItems.DisplayCosts, Caption = "Check our prices" });
            menuItems.Add(new MenuItem() { MenuItemId = Constants.MenuItems.EnterTheParking, Caption = "Enter the parking" });
            menuItems.Add(new MenuItem() { MenuItemId = Constants.MenuItems.GetParkedCars, Caption = "Get list of the parked cars" });
            menuItems.Add(new MenuItem() { MenuItemId = Constants.MenuItems.ExitTheParkingAndPay, Caption = "Exit the parking and pay" });
            menuItems.Add(new MenuItem() { MenuItemId = Constants.MenuItems.ExitTheApp, Caption = "Exit the app" });
        }

        private static string GetUserConsoleInput(string messageToDisplay)
        {
            Console.Write(messageToDisplay);
            return Console.ReadLine();
        }
        #endregion
    }
}
