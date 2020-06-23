# ov-report
generate report from ov

The Ov folder contains files that allow you to connect to www.ov-chipkaart.nl

Ov requires a month to generate a report.

Ov will return a class with a decimal value which is the declarable value


WebDriver is the class which connects to a browser via a chrome driver (https://chromedriver.chromium.org/) and selenium (https://www.selenium.dev/)

It also contains some basic js functions to help navigating the web page


Loketdriver connects to the page where you can upload your travel cost declarations.

It will fetch the downloaded report from ov-chipkaart and upload it to the loket site.

