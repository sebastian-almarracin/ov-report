using OpenQA.Selenium;
using OVVersion2.OV;
using OVVersion2.WebDriver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace OVVersion2.WerknemerLoket
{

    public class LoketDriver
    {
        private const string Username = "";
        private const string Password = "";
        private static readonly string _downloadDir = @"downloadDir";

        private Declaration Declaration { get; set; }

        public LoketDriver(Declaration declaration)
        {
            Declaration = declaration;
        }
         
        public void SendDeclaration()
        {
            Login();
            GoToDeclarations(); 
            SetDate();
            SetDeclarationType();
            SetDeclarableValue(Declaration);
            UploadFile();
            SubmitDeclaration();
        } 

        private void SubmitDeclaration()
        {
            var btns = Driver.GetElementsByTags("button").ToList();
            btns.FirstOrDefault(x => x.GetAttribute("gui_id") != null && x.GetAttribute("gui_id").Equals("mutatie-indienen-btn")).Click();
        }

        private void UploadFile()
        {
            var di = new DirectoryInfo(_downloadDir);
            var firstFileName = di.EnumerateFiles()
                                     .Select(fi => fi.Name)
                                     .FirstOrDefault(name => name != "Thumbs.db");
            var path = Path.Combine(_downloadDir, firstFileName);

            var input = Driver.GetElementsByTags("input").FirstOrDefault(x => x.GetAttribute("gui_id") != null && x.GetAttribute("gui_id").Equals("fileupload-input-mutatie"));
            input.SendKeys(path);
        }

        private void SetDeclarableValue(Declaration declarable)
        {
            Driver.GetElementById("waarde").SendKeys(declarable.DeclarationValue.ToString());
        }

        private void SetDate()
        {
            var btns = Driver.GetElementsByTags("input");
            btns.Where(x => x.GetAttribute("gui_id").Equals("datepicker-input-mutatie-datum")).ToList().ForEach(x => { x.SendKeys(DateTime.Now.ToString("dd-MM-yyyy")); });
            Thread.Sleep(1000);
        }

        private void SetDeclarationType()
        {
            var btns = Driver.GetElementsByTags("button").ToList();

            btns.FirstOrDefault(x => x.GetAttribute("gui_id") != null && x.GetAttribute("gui_id").Equals("select-btn--")).Click();
            var options = Driver.GetElementsByTags("a");
            options.FirstOrDefault(x => x.GetAttribute("gui_id") == "dropdown-item--"
                                     && x.Text == "Netto betaling, OV declaratie (300)")
                   .Click();
             
            Thread.Sleep(1000);
        }


        private void Login()
        {
            Driver.GoToUrl("https://werknemer.loket.nl/");

            Driver.GetElementById("userName").SendKeys(Username);
            Driver.GetElementById("password").SendKeys(Password);
            Driver.GetElementById("loginFormF1")
                  .FindChildrenByTagName("button")
                  .FirstOrDefault(x => x.Text == "Inloggen")
                  .Click();
            Thread.Sleep(1000);
        }

        private void GoToDeclarations()
        {
            Driver.GetElementById("sidebar").FindChildrenByTagName("span").FirstOrDefault(x => x.Text == "Mutatie/declaratie").Click();
            Thread.Sleep(1000);
        } 
    }
}
