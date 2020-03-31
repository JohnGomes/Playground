using System;
using System.Net.Http;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Shopping.Gateway.Services
{
    public static class GrpcCallerService
    {
        public static async Task<TResponse> CallService<TResponse>(string urlGrpc, Func<GrpcChannel, Task<TResponse>> func, ILoggerFactory loggerFactory)
        {
            var channel = CreateInsecureChannel(urlGrpc, loggerFactory);

            Log.Information("Creating grpc client base address urlGrpc {@urlGrpc}, BaseAddress {@BaseAddress} ", urlGrpc, channel.Target);

            try
            {
                return await func(channel);
            }
            catch (RpcException e)
            {
                Log.Error("Error calling via grpc: {Status} - {Message}", e.Status, e.Message);
                return default;
            }
            finally
            {
                AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", false);
                AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2Support", false);
            }
        }

        private static GrpcChannel CreateInsecureChannel(string urlGrpc, ILoggerFactory loggerFactory)
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2Support", true);

            var httpClientHandler = new HttpClientHandler();
            httpClientHandler.ServerCertificateCustomValidationCallback = httpClientHandler.ServerCertificateCustomValidationCallback;
            httpClientHandler.ServerCertificateCustomValidationCallback = (o, c, ch, er) => true;

            return GrpcChannel.ForAddress(urlGrpc, new GrpcChannelOptions
            {
                HttpClient = new HttpClient(httpClientHandler), 
                LoggerFactory = loggerFactory
            });
        }
    }
}