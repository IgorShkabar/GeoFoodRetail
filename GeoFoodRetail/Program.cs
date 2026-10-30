/** 
 * Copyright (c) 2024-26 Igor Shkabar. All rights reserved.
 * DI Container Configuration (Entry Point / Composition Root).
 * Configuration occurs once at application startup 
 * (in the Main method for Console/WPF/WinForms).
 */
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Configuration;
using System.Windows.Forms;
using DatabaseServiceLib;
using Microsoft.Extensions.DependencyInjection;
using GeoFoodRetail.EnterpriseForm;

namespace GeoFoodRetail
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Безопасная инициализация сервиса БД из appsettings.json
            var builder = OracleDbService.CreateBuilder(args);
            
            var dbService = builder
                .UseConfiguration() // Автоматически заберет строку из JSON
                .Build();

            dbService.Connect();

            // Сохраняем сервис глобально(или передаем в главную форму), 
            // чтобы все остальные формы и меню могли им пользоваться
            
            Application.Run(new MainForm(dbService));
        }
    }
}
/**
 * Это тоже работает:
 static void Main(string[] args)
        {            
            var builder = OracleDbService.CreateBuilder(args);
            
            var dbService = builder
                .UseConnectionString("User Id=myUsername;Password=myPassword;Data Source=myOracleDB;")
                .Build();

            dbService.Connect();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }

 * Error CS0234 The type or namespace name 'ConfigurationManager' does not exist in the 
 namespace 'System.Configuration' (are you missing an assembly reference?) 
 в строке 
 string connString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"]?.ConnectionString ?? "Server=myServer;Database=myDB;";
 
 Ошибка CS0234 возникает потому, что проект не видит класс ConfigurationManager: 
 в классическом .NET Framework к проекту не подключена сборка System.Configuration.dll, 
 а в современном .NET (Core / 5 / 6 / 8 / 9+) этот класс больше не входит в состав стандартной 
 библиотеки и вынесен в отдельный пакет. 

Если у вас классический .NET Framework (например, .NET 4.5 / 4.6 / 4.7.2)
1. В обозревателе решений (Solution Explorer) в Visual Studio найдите ваш проект.
2. Нажмите правой кнопкой мыши по папке References (Ссылки) и выберите Add Reference... (Добавить ссылку...).
3. В открывшемся окне перейдите на вкладку Assemblies -> Framework (или просто введите в поиске).
4. Найдите и отметьте галочкой System.Configuration, затем нажмите OK.
5. Убедитесь, что в файле кода сверху подключен директивой namespace: 
using System.Configuration;

Вариант 2. Если у вас современный .NET (Core, .NET 5, 6, 7, 8, 9+)
В новых версиях .NET класс ConfigurationManager находится в отдельном NuGet-пакете 
System.Configuration.ConfigurationManager.
Откройте Package Manager Console (Инструменты -> NuGet Package Manager -> Package Manager Console) 
в Visual Studio и выполните команду:  Install-Package System.Configuration.ConfigurationManager

Или установите через графический интерфейс NuGet Package Manager, найдя пакет 
System.Configuration.ConfigurationManager.

2.Альтернативный и рекомендуемый путь для современных приложений на .NET Core / .NET 6+: 
вместо ConfigurationManager использовать стандартный интерфейс IConfiguration и файл настроек 
appsettings.json
*/

