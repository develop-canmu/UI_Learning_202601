using System;
using Cysharp.Net.Http;
using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Net.Client.Configuration;

namespace Pjfb
{
    public static class ConnectionCommon
    {
        public static GrpcChannel GetGrpcChannelWithCertificate(string hostUrl)
        {
            var handler = new YetAnotherHttpHandler
            {
                SkipCertificateVerification = false,
                Http2Only = true,
                Http2KeepAliveInterval = TimeSpan.FromSeconds(3),
                Http2KeepAliveTimeout = TimeSpan.FromSeconds(5),
                Http2KeepAliveWhileIdle = true,
            };

            var config = new MethodConfig
            {
                Names = { MethodName.Default },
                RetryPolicy = new RetryPolicy
                {
                    MaxAttempts = 3,
                    InitialBackoff = TimeSpan.FromSeconds(1),
                    MaxBackoff = TimeSpan.FromSeconds(5),
                    BackoffMultiplier = 5,
                    RetryableStatusCodes =
                    {
                        StatusCode.Unavailable,
                        StatusCode.DataLoss,
                    }
                }
            };

            var options = new GrpcChannelOptions
            {
                HttpHandler = handler,
                UnsafeUseInsecureChannelCallCredentials = false,
                ServiceConfig = new ServiceConfig {MethodConfigs = { config }}
            };

            return GrpcChannel.ForAddress(hostUrl, options);
        }
    }
}
