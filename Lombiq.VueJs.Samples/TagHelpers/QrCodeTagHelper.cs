using Microsoft.AspNetCore.Razor.TagHelpers;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.IO;
using ZXing;
using ZXing.Common;
using BarcodeWriter = ZXing.ImageSharp.BarcodeWriter<SixLabors.ImageSharp.PixelFormats.Rgb24>;

namespace Lombiq.VueJs.Samples.TagHelpers;

[HtmlTargetElement("qr-code")]
public class QrCodeTagHelper : TagHelper
{
    [HtmlAttributeName("content")]
    public string Content { get; set; }

    [HtmlAttributeName("size")]
    public int Size { get; set; } = 400;

    [HtmlAttributeName("margin")]
    public int Margin { get; set; } = 5;

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        var barcodeWriter = new BarcodeWriter
        {
            Format = BarcodeFormat.QR_CODE,
            Options = new EncodingOptions
            {
                Width = Size,
                Height = Size,
                Margin = Margin,
            },
        };

        using var qrImage = barcodeWriter.WriteAsImageSharp<Rgb24>(Content);
        using var qrImageStream = new MemoryStream();

        qrImage.SaveAsPng(qrImageStream);

        qrImageStream.Seek(0, SeekOrigin.Begin);

        output.TagName = "img";
        output.Attributes.Add("width", Size);
        output.Attributes.Add("height", Size);
        output.Attributes.Add("src", $"data:image/png;base64,{Convert.ToBase64String(qrImageStream.ToArray())}");
    }
}
