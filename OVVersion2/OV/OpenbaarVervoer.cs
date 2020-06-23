using OpenQA.Selenium;
using OVVersion2.WebDriver;
using System;
using System.Globalization;
using System.IO;
using System.Linq;

namespace OVVersion2.OV
{
    public class OpenbaarVervoer
    {
        private readonly string usernameInp = "";
        private readonly string passwordInp = "";

        private readonly string homePage = "https://www.ov-chipkaart.nl/home.htm#/";
        private readonly string travelHistory = "https://www.ov-chipkaart.nl/mijn-ov-chip/mijn-ov-reishistorie.htm";

        private readonly string userMenuId = "menu-user";
        private readonly string loginMenuId = "menu-user-inloggen";
        private readonly string transactionFormId = "transaction-filter-form";

        private readonly string usernameId = "username";
        private readonly string passwordId = "password";

        private readonly string loginBtnId = "btn-login";
        private readonly string dateSelectId = "dateFilter";
        private readonly string periodSelectId = "select-period";

        private readonly string dateRangeId = "ranges";
        private readonly string listItemTag = "li";
        private readonly string optionsTag = "option";

        private readonly string transactionTypeId = "select-transaction-type";
        private readonly string travelTransactions = "Reistransacties";

        private readonly static string Home = "Arnhem";
        private readonly static string Work = "Duiven";
        private readonly static string CheckOut = "Check-uit";
        private readonly static string TrainProvider = "Arriva";

        private readonly string declarationsMonth = string.Empty;

        public OpenbaarVervoer(string monthToDeclare)
        {
            declarationsMonth = monthToDeclare;
        }

        public Declaration GenerateDeclarationReport()
        {
            GoToOvSite();
            LoginOv();
            GenerateTransactions();

            var result = ReadTransactions();

            DownloadDeclaration();

            return result;
        }

        private void GoToOvSite()
        {
            Driver.GoToUrl(homePage);
            Driver.ClickElementById(userMenuId);
            Driver.ClickElementById(loginMenuId);
        }

        private void LoginOv()
        {
            Driver.GetElementById(usernameId).SendKeys(usernameInp);
            Driver.GetElementById(passwordId).SendKeys(passwordInp);
            Driver.ClickElementById(loginBtnId);
        }

        private void GenerateTransactions()
        {
            Driver.GoToUrl(travelHistory);

            Driver.ScrollToElementById(transactionFormId);
            Driver.JsClickById(dateSelectId);
            Driver.JsClickById(periodSelectId);

            var listItems = Driver.GetElementByClass(dateRangeId).FindChildrenByTagName(listItemTag);

            var monthToSelect = listItems.FirstOrDefault(x => x.Text.ToLower().Contains(declarationsMonth.ToLower()));
            Driver.ScrollToElementByWebElement(monthToSelect);
            monthToSelect.Click();

            var transactionTypes = Driver.GetElementById(transactionTypeId);
            transactionTypes.Click();
            transactionTypes.FindChildrenByTagName(optionsTag).FirstOrDefault(x => x.Text == travelTransactions).Click();
        }

        private void DownloadDeclaration()
        {
            var downloadBtn = Driver.GetElementById("selected-card");
            Driver.ScrollToElementById("selected-card");
            if (downloadBtn != null)
            {
                downloadBtn.Click();
                var pdfBtnParent = Driver.GetElementByClass("export-transactions");
                var pdfBtn = pdfBtnParent.FindChildrenByClassName("tlsBtnSmall");
                pdfBtn.FirstOrDefault(x => x.Text == "Download pdf").Click();
            }
        }

        private IWebElement GetNextBtn()
        {
            var paginationBtnsDiv = Driver.GetElementByClass("transaction-pagination");
            var lastBtn = paginationBtnsDiv.FindChildrenByTagName("button").LastOrDefault();
            return lastBtn;
        }

        private readonly Func<string, bool> HasNextPage = btnText => btnText.Contains("Volgende");

        private Declaration ReadTransactions()
        {
            var result = new Declaration();

            while (GetNextBtn() != null)
            {
                var nextBtn = GetNextBtn();
                result.DeclarationValue += ReadTransactionPage();
                if (HasNextPage(nextBtn.Text))
                {
                    nextBtn.Click();
                }
                else
                {
                    break;
                }
            }
            return result;
        }

        private readonly Func<string, bool> IsWorkRoute = route => route.Contains(Home) && route.Contains(Work) && route.Contains(CheckOut) && route.Contains(TrainProvider);

        private decimal ReadTransactionPage()
        {
            var valueToDeclare = 0m;
            var rows = Driver.GetElementsByClass("known-transaction");
            foreach (var dataElem in rows)
            {
                if (IsWorkRoute(dataElem.Text))
                {
                    var filterString = "instaptarief";
                    var valueRow = dataElem.Text.IndexOf(filterString);
                    var valuePaid = $"{dataElem.Text.Substring(valueRow + filterString.Length)}";

                    var styleValue = NumberStyles.Number | NumberStyles.AllowCurrencySymbol;
                    var cultureValue = CultureInfo.CreateSpecificCulture("NL-nl");
                    if (decimal.TryParse(valuePaid, styleValue, cultureValue, out decimal tempPaid))
                    {
                        valueToDeclare += tempPaid;
                    }
                }
                else
                {
                    var checkbox = dataElem.FindElement(By.TagName("input"));
                    if (checkbox.Selected)
                    {
                        checkbox.Click();
                    }
                }
            }
            return valueToDeclare;
        }

    }
}
