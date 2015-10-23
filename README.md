Historical-Volatility-Calculator
================================

Winforms app written in C# to calculate a stock's historical volatility using Yahoo Finance .CSV file

This app can tell you the probability of a stock being above or below at a certain price at a chosen date
in the future.  In the image example below, I calculated the probabilty of Amazon's stock being over 320 by 11/14/14
which was 36.05%.

![GitHub Logo](https://github.com/ovnisoftware/Historical-Volatility-Calculator/blob/master/Calculator.png)

I got the mathematical formulas to calculate the historical volatility from C.B. Reehl's book
'The Mathematics of Options Trading' and converted them into C#.
http://www.amazon.com/Mathematics-Options-Trading-C-B-Reehl/dp/0071445285/ref=sr_1_1?ie=UTF8&qid=1414070824&sr=8-1&keywords=mathematics+of+options+trading

If you want to run the .exe without compiling the source code, it's here:
```
HistoricalVolatilityCalculator\bin\Debug\HistoricalVolatilityCalculator.exe
```
The logic is here:
```
HistoricalVolatilityCalculator/VolatilityCalculator.cs
HistoricalVolatilityCalculator/Form1.cs
```
To get the .csv stock information you can either go to yahoo finance (here's the link for Amazon's historical info)
http://finance.yahoo.com/q/hp?s=AMZN+Historical+Prices

or you can use another app I wrote called the Yahoo Finance Quote Downloader that is also on Github.

This was built with Visual Studio 2013, I included all the files in the solution folder.  This program targets .NET 4.5 so it will not work on XP, but will work on Vista, Windows 7, Windows 8 and Windows 10.
