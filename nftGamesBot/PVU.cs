using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using nftGamesBot.Models;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System.Linq;

namespace nftGamesBot
{
    [TestFixture]
    public class PVU
    {
        private IWebDriver driver;
        public bool comprarMotherThree = false;
        int minimoPlantasVendidas = 5;
        int minimoCondicoes = 50;
        private bool acceptNextAlert = true;
        private double percentualVenda = 0.95;
        private double percentualAnomalia = 0.1;
        public double fatorCorrecao = 0.75;
        int tempoEsperaInicial = 700;
        int tempoEspera = 700;
        List<string> listIdPlantasPassadas = new List<string>();
        public string mainUrl = "https://marketplace.plantvsundead.com/offering/bundle#/marketplace/plant?sort=latest&elements=electro,fire,metal,wind,water,ice,parasite,dark";
        public Dictionary<string, List<Planta>> PlantasVendidas { get; set; }
        List<Planta> MinhasPlantas { get; set; }
        public Dictionary<string, double> Condicoes { get; set; }

        [SetUp]
        public void Setup()
        {
            if (comprarMotherThree)
            {
                mainUrl = "https://marketplace.plantvsundead.com/offering/bundle#/marketplace/mother-tree?sort=latest";
                minimoPlantasVendidas = 5;
            }

            PlantasVendidas = new Dictionary<string, List<Planta>>();
            MinhasPlantas = new List<Planta>();
            Condicoes = new Dictionary<string, double>();
            ChromeOptions opt = new ChromeOptions();
            opt.DebuggerAddress = "127.0.0.1:9222";

            driver = new ChromeDriver(@$"{Environment.CurrentDirectory}", opt);

            GetCotacao();
            BuscarPlantas();
        }

        [Test]
        public void BuscarPlantas()
        {
            var url = "";

            int i = 0;
            tempoEspera = tempoEsperaInicial;

            while (true)
            {
                try
                {
                    driver.Navigate().GoToUrl(mainUrl);
                    Thread.Sleep(tempoEspera);
                    var mainResult = false;
                    while (!mainResult)
                    {
                        try
                        {
                            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;

                            var uls = driver.FindElements(By.TagName("ul"));
                            var lis = uls[3].FindElements(By.CssSelector(".tw-h-80.tw-rounded-lg.tw-bg-black.tw-bg-opacity-10.tw-border.tw-border-gray-900"));

                            foreach (var li in lis)
                            {
                                var result = false;

                                while (!result)
                                {
                                    var a = li.FindElement(By.TagName("a"));
                                    var id = li.FindElement(By.ClassName("id")).Text;
                                    var div = a.FindElement(By.CssSelector(".tw-flex.tw-justify-center"));
                                    var image = div.FindElement(By.CssSelector("img"));
                                    url = a.GetAttribute("href");
                                    var rodape = a.FindElement(By.CssSelector(".tw-flex.tw-justify-between.tw-items-end"));
                                    var twml4 = rodape.FindElement(By.CssSelector(".tw-ml-4"));
                                    var divPreco = twml4.FindElement(By.TagName("div"));
                                    var preco = Convert.ToDouble(divPreco.FindElement(By.TagName("p")).Text.Replace(".", ","));



                                    if (listIdPlantasPassadas.Count >= 15)
                                    {
                                        listIdPlantasPassadas.RemoveRange(0, 5);
                                    }

                                    if (MinhasPlantas.Count >= 15)
                                    {
                                        MinhasPlantas.RemoveRange(0, 5);
                                    }

                                    if (GetCondicao(image.GetAttribute("src"), preco, id) && !listIdPlantasPassadas.Contains(id))
                                    {
                                        ComprarPlanta(id, preco, url);
                                        GetMinhasPlantas();
                                    }

                                    result = true;
                                }
                            }

                            RemoverAnomalias();
                            mainResult = true;
                        }
                        catch (Exception ex)
                        {
                            tempoEspera += 100;
                            driver.Navigate().GoToUrl(mainUrl);
                            Thread.Sleep(tempoEspera);
                            Console.WriteLine(ex.Message);
                        }

                    }

                    if (i >= 15)
                    {
                        GetMinhasPlantas();
                    }

                    if (i++ >= 499)
                    {
                        i = 0;
                        GetCotacao();
                    }

                    Console.WriteLine("");
                    Console.WriteLine("Condições: ");

                    var condicoesToString = "";
                    foreach (var condicao in Condicoes)
                    {
                        condicoesToString = $"{condicao.Key} {condicao.Value}";
                        Console.WriteLine(condicoesToString);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public void GetCotacao()
        {
            PlantasVendidas.Clear();
            Condicoes.Clear();
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;

            string url = "https://marketplace.plantvsundead.com/offering/bundle#/";
            driver.Navigate().GoToUrl(url);

            Thread.Sleep(tempoEspera);

            if (comprarMotherThree)
            {
                js.ExecuteScript("document.getElementsByTagName('img')[86].click();");
                Thread.Sleep(2000);
            }

            while (Condicoes.Count < minimoCondicoes)
            {
                try
                {
                    Thread.Sleep(1000);

                    var uls = driver.FindElements(By.TagName("ul"));
                    var ulIndex = 0;

                    if (comprarMotherThree)
                        ulIndex = 5;
                    else
                        ulIndex = 4;

                    var lis = uls[ulIndex].FindElements(By.TagName("li"));

                    foreach (var li in lis)
                    {
                        var a = li.FindElement(By.TagName("a"));
                        var divs = a.FindElements(By.TagName("div"));
                        var buyer = a.FindElement(By.ClassName("username")).GetAttribute("innerHTML");
                        var divImagem = divs[0];
                        var image = divImagem.FindElement(By.CssSelector("img"));
                        var id = image.GetAttribute("src");
                        id = id.Substring(id.IndexOf("/plant/") + 7, (id.Length - 7) - id.IndexOf("/plant/")).Replace(".png", "").Replace(".jpg", "").Trim().ToUpper();
                        var divPreco = divs[2];
                        var p = divPreco.FindElement(By.TagName("p"));
                        var preco = Convert.ToDouble(p.GetAttribute("innerHTML").Replace(".", ","));

                        if (!PlantasVendidas.Any(x => x.Key == id))
                            PlantasVendidas.Add(id, new List<Planta> { new Planta { Id = id, Preco = preco, Buyer = buyer } });
                        else
                        {
                            var planta = PlantasVendidas.FirstOrDefault(x => x.Key == id);
                            if (!planta.Value.Any(x => x.Buyer == buyer))
                            {
                                planta.Value.Add(new Planta { Id = id, Buyer = buyer, Preco = preco });
                                //var result = planta.Value.Average(x => x.Preco) > preco * percentualAnomalia;
                                //while (result)
                                //{
                                //    planta.Value.RemoveAll(x => x.Preco == planta.Value.Max(x => x.Preco));
                                //    result = planta.Value.Count > 0 ? planta.Value.Average(x => x.Preco) > preco * percentualAnomalia : false;
                                //}
                                //if (planta.Value.Count > 0)
                                //{
                                //    if (preco < (planta.Value.Average(x => x.Preco) * percentualAnomalia))
                                //        planta.Value.Add(new Planta { Id = id, Preco = preco, Buyer = buyer });
                                //}
                                //else
                                //    planta.Value.Add(new Planta { Id = id, Preco = preco, Buyer = buyer });
                            }
                        }
                    }

                    RemoverAnomalias();

                    js = (IJavaScriptExecutor)driver;
                    if (comprarMotherThree)
                        js.ExecuteScript("document.getElementsByClassName('box tw-cursor-pointer')[3].click();");
                    else
                        js.ExecuteScript("document.getElementsByClassName('box tw-cursor-pointer')[1].click();");


                    var total = 0;
                    foreach (var planta in PlantasVendidas)
                    {
                        total += planta.Value.Count;
                    }

                    Thread.Sleep(1000);

                    foreach (var plantaVendida in PlantasVendidas)
                    {
                        AdicionarCondicao(plantaVendida);
                    }

                    var conditionsLoading = "";

                    for (int i = 0; i <= Condicoes.Count; i++)
                        conditionsLoading += "=";
                    conditionsLoading += $"> {Condicoes.Count}/{minimoCondicoes} Condicoes";
                    Console.Clear();
                    Console.WriteLine(conditionsLoading);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    driver.Navigate().GoToUrl(url);
                }
            }
        }


        private void RemoverAnomalias()
        {
            foreach (var plantaVendida in PlantasVendidas)
            {
                if (plantaVendida.Value.Count >= minimoPlantasVendidas)

                    while (plantaVendida.Value.Any(x => x.Preco >= plantaVendida.Value.Average(y => y.Preco) * (1 + percentualAnomalia)
                                                        //|| x.Preco <= plantaVendida.Value.Average(y => y.Preco) * (1 - percentualAnomalia)
                                                        ))
                    {
                        plantaVendida.Value.RemoveAll(x =>
                                                                 x.Preco >= plantaVendida.Value.Average(y => y.Preco) * (1 + percentualAnomalia)
                        //|| x.Preco <= plantaVendida.Value.Average(y => y.Preco) * (1 - percentualAnomalia)
                        );
                    }
            }
        }

        private bool GetCondicao(string src, double price, string id)
        {
            if (Condicoes.Count == 0)
                GetCotacao();

            var planta = PlantasVendidas.FirstOrDefault(x => src.IndexOf($"/{x.Key}") > -1);

            if (planta.Value == null)
            {
                var result = src.Split("/");
                var key = result[result.Count() - 1].Replace(".png", "");
                PlantasVendidas.Add(key, new List<Planta>());
                planta = PlantasVendidas.FirstOrDefault(x => x.Key == key);
            }

            if (planta.Value.Count > minimoPlantasVendidas)
                planta.Value.RemoveRange(0, planta.Value.Count - minimoPlantasVendidas);



            planta.Value.Add(new Planta { Id = id, Preco = price });
            RemoverAnomalias();

            AdicionarCondicao(planta);

            return Condicoes.Any(x => src.IndexOf($"/{x.Key}") > -1 && price <= ((x.Value * fatorCorrecao)));
        }

        private void AdicionarCondicao(KeyValuePair<string, List<Planta>> planta)
        {
            if (!Condicoes.Any(x => x.Key == planta.Key) && planta.Value.Count >= minimoPlantasVendidas)
                Condicoes.Add(planta.Key, planta.Value.Average(x => x.Preco));
        }

        public void GetMinhasPlantas()
        {
            var result = false;

            while (!result)
            {
                string id = null;
                string classe = null;
                var url = "https://marketplace.plantvsundead.com/farm#/profile/inventory";
                driver.Navigate().GoToUrl(url);

                Thread.Sleep(3000);

                try
                {
                    IJavaScriptExecutor js = (IJavaScriptExecutor)driver;


                    if (comprarMotherThree)
                    {
                        js.ExecuteScript("document.getElementsByTagName('img')[20].click();");
                        js.ExecuteScript("document.getElementsByTagName('img')[23].click();");
                    }
                    else
                    {
                        js.ExecuteScript("document.getElementsByTagName('img')[20].click();");
                        js.ExecuteScript("document.getElementsByTagName('img')[21].click();");
                    }




                    Thread.Sleep(3000);

                    var div1 = driver.FindElements(By.ClassName("v-card__text"));
                    var div2 = div1[1].FindElements(By.CssSelector(".wrap.tw-relative"));
                    var div3 = div2[0].FindElements(By.CssSelector(".grid__item.tw-px-1"));


                    foreach (var div in div3)
                    {
                        id = div.FindElement(By.CssSelector(".id")).GetAttribute("innerHTML");
                        var divImage = div.FindElement(By.ClassName("tw-justify-center"));
                        var imgSrc = divImage.FindElement(By.TagName("img")).GetAttribute("src").Split("/");
                        classe = imgSrc[imgSrc.Count() - 1].Replace(".jpg", "").Replace(".png", "");
                        var forSale = div.FindElement(By.CssSelector(".forSale")).GetAttribute("innerHTML").ToUpper().Trim();
                        //!= null ? div.FindElement(By.CssSelector(".forSale")).GetAttribute("innerHTML").ToUpper() : null;

                        if (forSale != "FARMING" && forSale != "FOR SALE")
                        {
                            VenderPlanta(id, classe);
                        }
                    }
                    BuscarPlantas();
                }
                catch (Exception ex)
                {
                    if (ex.Message.IndexOf("Unable to locate element") > -1 && ex.Message.IndexOf(".forSale") > -1)
                    {
                        VenderPlanta(id, classe);
                        Thread.Sleep(5000);
                    }

                    Console.WriteLine(ex.Message);
                }

                result = true;
            }
        }

        private void ComprarPlanta(string id, double preco, string url)
        {
            var result = false;
            while (!result)
            {
                try
                {
                    listIdPlantasPassadas.Add(id);
                    driver.Navigate().GoToUrl(url);
                    IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                    Thread.Sleep(tempoEspera);

                    js = (IJavaScriptExecutor)driver;
                    string title = (string)js.ExecuteScript("document.getElementsByClassName('btn__sell')[0].click();");
                    Thread.Sleep(tempoEspera);
                    MinhasPlantas.Add(new Planta { Id = id, Preco = preco });
                    result = true;
                }
                catch (Exception ex)
                {
                    tempoEspera += 100;
                    Console.WriteLine(ex.Message);

                    if (tempoEspera >= tempoEsperaInicial * 2)
                    {
                        tempoEspera = tempoEsperaInicial;
                        BuscarPlantas();
                    }
                    else
                        ComprarPlanta(id, preco, url);
                }
            }
        }

        private void VenderPlanta(string id, string classe)
        {
            var minhaPlanta = MinhasPlantas.FirstOrDefault(x => x.Id == id);
            if (planta != null)
            {
                try
                {
                    driver.Navigate().GoToUrl($"https://marketplace.plantvsundead.com/farm#/plant/{id}");
                    Thread.Sleep(tempoEspera);

                    var btnSell = driver.FindElement(By.CssSelector(".btn__sell"));

                    if (btnSell.GetAttribute("innerHTML").ToUpper().Trim() == "SELL NOW")
                    {
                        IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                        js.ExecuteScript("document.getElementsByClassName('btn__sell')[0].click();");
                        Thread.Sleep(tempoEspera);

                        driver.FindElement(By.CssSelector(".input.tw-ml-4")).Clear();
                        var precoMedio = PlantasVendidas.FirstOrDefault(x => x.Key == classe).Value.Average(y => y.Preco);

                        var precoVenda = precoMedio >= minhaPlanta.Preco ? precoMedio : minhaPlanta.Preco;

                        precoVenda = precoVenda <= precoMedio * 0.9 ? precoMedio * percentualVenda : precoVenda * percentualVenda;


                        driver.FindElement(By.CssSelector(".input.tw-ml-4")).SendKeys(Math.Round(precoMedio * percentualVenda, 2).ToString().Replace(".", "").Replace(",", "."));

                        js.ExecuteScript("document.getElementsByClassName('sell tw-mt-10 v-btn v-btn--is-elevated v-btn--has-bg theme--light v-size--default')[0].click();");
                        Thread.Sleep(tempoEspera * 40);
                    }
                }
            }
            catch (Exception ex)
            {
                if (tempoEspera <= 2000)
                    tempoEspera += 100;
                VenderPlanta(id, classe);
            }
            GetMinhasPlantas();
        }
    }
}
