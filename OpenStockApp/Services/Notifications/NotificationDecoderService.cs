using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Result = OpenStockApi.Core.Models.Searches.Result;

namespace OpenStockApp.Services.Notifications
{
    public class NotificationDecoderService
    {
        public static Result? DecodeNotification(Dictionary<string, string> userInfo)
        {
            if (userInfo.Any())
            {
                if (userInfo.TryGetValue("json", out string? rawBytes) && userInfo.TryGetValue("type", out string? type) && userInfo.TryGetValue("encoding", out var encoding))
                {
                    var rawJson = UncompressMessage(rawBytes, encoding);

                    if (rawJson is null)
                        return null;

                    switch (type)
                    {
                        case nameof(Result):
                            var result = JsonSerializer.Deserialize<Result>(rawJson);
                            if (result != null)
                                return result;
                            break;
                        default:
                            break;

                    }
                }
            }
            return null;
        }
        private static string? UncompressMessage(string base64string, string encoding)
        {
            var bytes = Convert.FromBase64String(base64string);
            switch (encoding)
            {
                case "br":
                    return UncompressBrotli(bytes);
                default:
                    throw new ArgumentException("Unkwnown encoding");
            }
        }

        private static string? UncompressBrotli(Span<byte> bytes)
        {
            var span = new Span<byte>(new byte[4 * bytes.Length]);
            if (BrotliDecoder.TryDecompress(bytes, span, out int bytesWritten))
            {
                return Encoding.UTF8.GetString(span.Slice(0, bytesWritten));
            }
            return null;
        }

    }
}
