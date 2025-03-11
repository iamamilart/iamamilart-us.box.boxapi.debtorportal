using PuppeteerSharp;
using System.IO;
using System.Threading.Tasks;

namespace US.BOX.BoxAPI.Service.Utils
{


    public class PdfService
    {
        public static async Task<byte[]> ConvertHtmlToPdfAsync(string htmlContent)
        {
           // await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultRevision);
            await using var browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true });
            await using var page = await browser.NewPageAsync();
            await page.SetContentAsync(htmlContent);

            byte[] pdfBytes = await page.PdfDataAsync(new PdfOptions
            {
                Format = PuppeteerSharp.Media.PaperFormat.A4
            });

            return pdfBytes;
        }
    }

}
