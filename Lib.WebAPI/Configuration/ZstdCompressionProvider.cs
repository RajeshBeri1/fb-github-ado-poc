using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Options;
//using ZstdNet; not installed

namespace Lib.WebAPI.Configuration
{
    //public class ZstdCompressionProvider : ICompressionProvider
    //{
    //    private readonly ZstdCompressionProviderOptions _options;

    //    public ZstdCompressionProvider(IOptions<ZstdCompressionProviderOptions> options)
    //    {
    //        _options = options.Value;
    //    }

    //    public string EncodingName => "zstd"; // Standard Content-Encoding for Zstandard
    //    public bool SupportsFlush => true; // Zstd streams generally support flushing

    //    public Stream CreateStream(Stream outputStream)
    //    {
    //        var compressionOptions = new CompressionOptions(_options.Level);
    //        return new CompressionStream(outputStream, compressionOptions);
    //    }
    //}
}
