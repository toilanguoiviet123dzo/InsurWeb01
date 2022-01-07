using System.Net.Http;
using System.Threading.Tasks;
using System;
using Grpc.Core;
using Grpc.Net.Client;
using System.Linq;
using System.Collections.Generic;

namespace Gosu.Common
{
    public static class GrpcCallerService
    {
        public static async Task<TResponse> CallService<TResponse>(GrpcChannel channel, Func<GrpcChannel, Task<TResponse>> func)
        {
            //AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            //AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2Support", true);
            //
            try
            {
                return await func(channel);
            }
            catch
            {
                return default;
            }
        }



    }
}
