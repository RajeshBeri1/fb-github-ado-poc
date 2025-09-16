using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Lib.WebAPI.Models.Flowchart;
using Lib.WebAPI.Models.Flowchart.Enumerations;

namespace Lib.WebAPI.Validation
{
    /// <summary>
    /// Image
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ImageValidator : AbstractValidator<Image>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImageValidator" /> class.
        /// </summary>
        public ImageValidator()
        {
            RuleFor(x => x.Content).NotEmpty().Must((image, bytes) =>
            {
                try
                {
                    var format = SixLabors.ImageSharp.Image.DetectFormat(bytes);

                    return format switch
                    {
                        SixLabors.ImageSharp.Formats.Gif.GifFormat _ => image.Type == ImageType.GIF,
                        SixLabors.ImageSharp.Formats.Tiff.TiffFormat _ => image.Type == ImageType.TIFF,
                        SixLabors.ImageSharp.Formats.Jpeg.JpegFormat _ => image.Type == ImageType.JPEG,
                        SixLabors.ImageSharp.Formats.Png.PngFormat _ => image.Type == ImageType.PNG,
                        SixLabors.ImageSharp.Formats.Bmp.BmpFormat _ => image.Type == ImageType.BMP,
                        _ => false,
                    };
                }
                catch (Exception)
                {
                    return false;
                }
            }).WithMessage($"Invalid image or image type mismatch. Accepted image types are: {string.Join(", ", Enum.GetValues<ImageType>())}.");

            RuleFor(x => x.Type).NotNull();
        }
    }
}