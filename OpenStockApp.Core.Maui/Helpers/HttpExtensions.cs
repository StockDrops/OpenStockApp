using Microsoft.Extensions.DependencyInjection;
//#if ANDROID
//using StockDrops.Core.Platforms.Android.HttpClientHandlers;
//#endif
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace OpenStockApp.Core.Maui.Helpers
{
    public static class HttpClientHandlerExtensions
    {
        public static HttpClientHandler AddClientCertificate(this HttpClientHandler handler, X509Certificate2 certificate)
        {
            handler.ClientCertificateOptions = ClientCertificateOption.Manual;
            handler.ClientCertificates.Add(certificate);

            return handler;
        }
        /// <summary>
        /// This method will work on Windows, and Android and on any other platform it will just be transparent, and do nothing. This will need to be updated once https://github.com/xamarin/xamarin-android/pull/6541 is merged.
        /// </summary>
        /// <param name="httpClientBuilder"></param>
        /// <param name="certificate"></param>
        /// <returns></returns>
        public static IHttpClientBuilder AddClientCertificate(this IHttpClientBuilder httpClientBuilder, X509Certificate2 certificate)
        {
#if ANDROID
            //httpClientBuilder.ConfigureHttpMessageHandlerBuilder(builder =>
            //{
            //    if (builder.PrimaryHandler is AndroidHttpsClientHandler handler)
            //    {
            //        handler.Initialize();
            //        handler.SetClientCertificate(certificate.Export(X509ContentType.Pkcs12, "pass"), "pass".ToCharArray());
            //    }
            //});
#elif WINDOWS
            httpClientBuilder.ConfigureHttpMessageHandlerBuilder(builder =>
            {
                if (builder.PrimaryHandler is HttpClientHandler handler)
                {
                    handler.AddClientCertificate(certificate);
                }
                else
                {
                    throw new InvalidOperationException($"Only {typeof(HttpClientHandler).FullName} handler type is supported. Actual type: {builder.PrimaryHandler.GetType().FullName}");
                }
            });
#endif
            return httpClientBuilder;
        }
    }
}
