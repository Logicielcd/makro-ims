using System;
using System.Collections.Generic;
//using TesseractOCR.Enums;
//using TesseractOCR;
using System.Drawing;

using System.Diagnostics;
using Tesseract;
using Image = System.Drawing.Image;
using Color = System.Drawing.Color;
using System.Drawing.Drawing2D;

class UrlShortener
{
    private readonly Dictionary<string, string> urlMappings = new Dictionary<string, string>();
    private int counter = 1;
    private const string BaseUrl = "https://bookingqa.siammakro.co.th/ims/";

    public string ShortenUrl(string longUrl)
    {
        string shortUrl = BaseUrl + Encode(counter);
        urlMappings[shortUrl] = longUrl;
        counter++;
        return shortUrl;
    }

    public string ExpandUrl(string shortUrl)
    {
        if (urlMappings.ContainsKey(shortUrl))
        {
            return urlMappings[shortUrl];
        }
        else
        {
            throw new ArgumentException("Short URL not found.");
        }
    }

    private string Encode(int number)
    {
        const string characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        int baseLength = characters.Length;
        string encoded = "";

        while (number > 0)
        {
            int remainder = number % baseLength;
            encoded = characters[remainder] + encoded;
            number /= baseLength;
        }

        return encoded;
    }
}

class Program
{
    static void Main()
    {
        try
        {
            string originalImage = @"C:\Temp\ocr\test1.jpg";
            string convertColorImage = @"c:\Temp\ocr\output1.png";
            string resizeImage = @"c:\Temp\ocr\output2.png";


            convertColor(originalImage, resizeImage);

            //cropAndResize(convertColorImage, resizeImage);
           

            string configurationFilePath = @"C:\Temp\ocr\tessdata";
            using (var engine = new TesseractEngine(configurationFilePath, "tha", EngineMode.Default))
            {
                using (var img = Tesseract.Pix.LoadFromFile(resizeImage))
                {
                    using (var page = engine.Process(img))
                    {
                        var text = page.GetText();                 
                        Console.WriteLine("Text (GetText): \r\n{0}", text);
                    }
                }
            }
        }
        catch (Exception e)
        {
            Trace.TraceError(e.ToString());
            Console.WriteLine("Unexpected Error: " + e.Message);
            Console.WriteLine("Details: ");
            Console.WriteLine(e.ToString());
        }
        Console.Write("Press any key to continue . . . ");
        Console.ReadKey(true);


        //string baseUrl = "https://localhost:4200/pages/driver?A=";

        //string para = "129483902";

        //string originalString = para;
        //byte[] bytes = System.Text.Encoding.UTF8.GetBytes(originalString);
        //string base64String = Convert.ToBase64String(bytes);

        //Console.WriteLine(baseUrl + base64String);

        //UrlShortener urlShortener = new UrlShortener();

        //string longUrl = "https://bookingqa.siammakro.co.th/ims/driver";
        //string shortenedUrl = urlShortener.ShortenUrl(longUrl);

        //Console.WriteLine("Shortened URL: " + shortenedUrl);

        //string expandedUrl = urlShortener.ExpandUrl(shortenedUrl);
        //Console.WriteLine("Expanded URL: " + expandedUrl);
    }

    static void cropAndResize(string imgPath,string outputPath)
    {
        Image originalImage = Image.FromFile(imgPath);

        System.Drawing.Rectangle cropArea = new System.Drawing.Rectangle(300, 200, 1100, 500);

        Bitmap croppedImage = new Bitmap(cropArea.Width, cropArea.Height);

        using (Graphics g = Graphics.FromImage(croppedImage))
        {
            g.DrawImage(originalImage, new System.Drawing.Rectangle(0, 0, 400, 200), cropArea, GraphicsUnit.Pixel);
        }

        croppedImage.Save(outputPath);
    }

    static void convertColor(string inputImagePath, string outputImagePath)
    {
        int colorThreshold = 200;

        // Load the input image
        using (Bitmap originalImage = new Bitmap(inputImagePath))
        {
            // Create a copy of the original image
            using (Bitmap modifiedImage = new Bitmap(originalImage.Width, originalImage.Height))
            {
                using (Graphics graphics = Graphics.FromImage(modifiedImage))
                {
                    // Set the drawing color to white
                    using (Brush whiteBrush = new SolidBrush(Color.White))
                    {
                        graphics.FillRectangle(whiteBrush, 0, 0, modifiedImage.Width, modifiedImage.Height);
                    }

                    // Iterate through the pixels of the original image
                    for (int x = 0; x < originalImage.Width; x++)
                    {
                        for (int y = 0; y < originalImage.Height; y++)
                        {
                            Color pixelColor = originalImage.GetPixel(x, y);

                            // Check if the pixel color is black
                            double distanceToBlack = CalculateColorDistance(pixelColor, Color.Black);

                            if (distanceToBlack <= colorThreshold)
                            {
                                // Set the corresponding pixel in the modified image to white
                                modifiedImage.SetPixel(x, y, Color.Black);
                            }
                            else
                            {
                                // Keep the original color for pixels not near black
                                modifiedImage.SetPixel(x, y, Color.White);
                            }
                        }
                    }
                }

                System.Drawing.Rectangle cropArea = new System.Drawing.Rectangle(300, 220, 1100, 500);
                
                Bitmap croppedImage = new Bitmap(400,200);

                using (Graphics g = Graphics.FromImage(croppedImage))
                {
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.DrawImage(modifiedImage, new System.Drawing.Rectangle(0, 0, 400, 200), cropArea, GraphicsUnit.Pixel);

                }

                // Save the modified image with black converted to white
                croppedImage.Save(outputImagePath, System.Drawing.Imaging.ImageFormat.Png);

            }
        }

        Console.WriteLine("Image processing complete.");
    }

    static double CalculateColorDistance(System.Drawing.Color c1, System.Drawing.Color c2)
    {
        int dr = c1.R - c2.R;
        int dg = c1.G - c2.G;
        int db = c1.B - c2.B;
        return Math.Sqrt(dr * dr + dg * dg + db * db);
    }

}
