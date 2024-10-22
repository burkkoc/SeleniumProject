using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using HtmlAgilityPack;
using System.Text;
using Selenium2;
using OpenQA.Selenium.Interactions;


var options = new ChromeOptions();
options.AddArgument("user-data-dir=C:/Users/bkoc/AppData/Local/Google/Chrome/User Data");
options.SetLoggingPreference(LogType.Browser, LogLevel.Off);

IWebDriver driver = new ChromeDriver(@"C:\Users\bkoc\OneDrive\Masaüstü\AT\ANK15\MVC\Selenium2\Selenium2\bin\Debug\net8.0", options);
IJavaScriptExecutor js = (IJavaScriptExecutor)driver;

Random rnd = new Random();

Console.Write("Count: ");
int count = int.Parse(Console.ReadLine());
string gameName = string.Empty;
string navigateUrl = string.Empty;
string lowestUrl = string.Empty;
string manageOfferVariableXPATH = string.Empty;
List<Game> games = new List<Game>();
for (int i = 0; i < count-1; i++)
    js.ExecuteScript("window.open();");

var windows = driver.WindowHandles;

for (int i = 0; i < count; i++)
{
   
    Game game = new Game();
    Console.Write("Game: ");
    game.Name = Console.ReadLine();
    switch (game.Name)
    {
        case "dnd":
            game.NavigateUrl = "https://www.g2g.com/sell/manage?service=1&game=31849";
            game.LowestUrl = "https://www.g2g.com/offer/Main-Server?service_id=lgc_service_1&brand_id=lgc_game_31849&fa=lgc_31849_server%3Algc_31849_server_47703&sort=lowest_price";
            game.ManageOfferVariableXPATH = "//*[@id=\"c2c_83481935\"]/td[6]/div[1]/span[1]/span[2]/a";
            game.PriceNode = "//*[@id=\"pre_checkout_sls_offer\"]/div[1]/div/div[1]/div[6]/div/span[1]";
            game.SellerNode = "//*[@id=\"pre_checkout_sls_offer\"]/div[1]/div/div[1]/div[1]/div/a/div[2]/div[1]";


            break;
        case "diablo":
            game.NavigateUrl = "https://www.g2g.com/sell/manage?service=1&game=26891";
            game.LowestUrl = "https://www.g2g.com/offer/Season-3---Hardcore?service_id=lgc_service_1&brand_id=lgc_game_26891&fa=lgc_26891_platform%3Algc_26891_platform_49816&sort=lowest_price";
            game.ManageOfferVariableXPATH = "//*[@id=\"c2c_84416865\"]/td[6]/div[1]/span[1]/span[2]/a\r\n";
            break;
        case "diablo-mats":
            game.NavigateUrl = "https://www.g2g.com/sell/manage?service=1&game=26891";
            game.LowestUrl = "https://sls.g2g.com/offer/G1705659039568GJ?currency=USD&country=TR&include_out_of_stock=1";
            break;
    }
    games.Add(game);
    driver.SwitchTo().Window(windows[i]);
    driver.Navigate().GoToUrl(games[i].NavigateUrl);
}


HtmlNode sellerNameNode = null;
HtmlNode priceNode = null;




while (true)
{
    for (int i = 0; i < count; i++)
    {
        driver.SwitchTo().Window(windows[i]);
        await ManageOffer(games[i]);
            
    }
    Console.WriteLine("-------------------------------");
    Thread.Sleep(rnd.Next(90000, 120000));
}


async Task<string> GetValuesFromSite(Game currentGame)
{
    try
    {
        using (HttpClient client = new HttpClient())
        {
            string html = await client.GetStringAsync(currentGame.LowestUrl);

            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(html);
            #region GameCoins
            //sellerNameNode = document.DocumentNode.SelectSingleNode("//*[@id=\"pre_checkout_sls_offer\"]/div[1]/div/div[1]/div[1]/div/a/div[2]/div[1]");

            //priceNode = document.DocumentNode.SelectSingleNode("//*[@id=\"pre_checkout_sls_offer\"]/div[1]/div/div[1]/div[6]/div/span[1]");
            #endregion
            priceNode = document.DocumentNode.SelectSingleNode(currentGame.PriceNode);
            sellerNameNode = document.DocumentNode.SelectSingleNode(currentGame.SellerNode);

            if (sellerNameNode != null && priceNode != null)
            {
                StringBuilder sellerName = new StringBuilder();

                if (sellerName.Length > 0)
                    sellerName.Clear();
                sellerName.Append(sellerNameNode.InnerText.Trim());

                if (sellerName.ToString() != "burkkoc")
                {
                    StringBuilder minPriceAsString = new StringBuilder();

                    if (minPriceAsString.Length > 0)
                        minPriceAsString.Clear();
                    minPriceAsString.Append(priceNode.InnerText.Trim());
                    double minPrice = double.Parse(minPriceAsString.ToString());
                    double myPrice = (minPrice - 0.000001);
                    Console.WriteLine($"-----\nGame: {currentGame.Name}\nSeller: {sellerName}\nPrice: {minPrice}");
                    if (myPrice.ToString().Length > 7)
                        return myPrice.ToString().Substring(0, 8);
                    else
                        return myPrice.ToString().Substring(0, myPrice.ToString().Length);
                }
                Console.WriteLine("We're lowest on " + currentGame.Name +"!");
                return "";

            }
            else
            {
                return "";
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
        return "";
    }
}

async Task ManageOffer(Game currentGame)
{
    try
    {

        StringBuilder myPrice = new StringBuilder();
        if (myPrice.Length > 0)
            myPrice.Clear();
        myPrice.Append(await GetValuesFromSite(currentGame));
        Console.Write($"Our Price: {myPrice}\n-----");
        if (myPrice.Length > 0)
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(2));

            IWebElement manageOfferVariable = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.XPath(currentGame.ManageOfferVariableXPATH)));

            manageOfferVariable.Click();

            manageOfferVariable = driver.FindElement(By.XPath("/html/body/div[17]/div[1]/div/div/form/div/div[1]/div[1]/div[1]/input"));

            manageOfferVariable.Clear();
            manageOfferVariable.SendKeys(myPrice.ToString());

            manageOfferVariable = driver.FindElement(By.XPath("/html/body/div[17]/div[1]/div/div/form/div/div[1]/div[1]/div[2]/button[2]"));

            try
            {
            manageOfferVariable.Click();
            }
            catch
            {
                throw new Exception("Out of stock or min price is too low.");
            }

        }
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
    }

}

