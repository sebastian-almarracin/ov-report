using OVVersion2.OV;
using OVVersion2.WebDriver;
using OVVersion2.WerknemerLoket;
using System;
using System.Globalization;

namespace OVVersion2
{
    class Program
    {

        static void Main(string[] args)
        {
            var previousMonthDutch = DateTime.Now.AddMonths(-1).ToString("MMMM", CultureInfo.GetCultureInfo("NL-nl"));
            Driver.InitializeChrome();
            var ovHandler = new OpenbaarVervoer(previousMonthDutch);
            var declaration = ovHandler.GenerateDeclarationReport();
            
            var loketHandler = new LoketDriver(declaration);
            loketHandler.SendDeclaration( );

            Console.WriteLine("ReportGenerated!");
        }
    }
}
