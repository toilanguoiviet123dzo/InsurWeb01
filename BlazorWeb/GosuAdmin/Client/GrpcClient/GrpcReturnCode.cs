using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gosu.GrpcWeb.Client
{
    public class GrpcReturnCode
    {
        public static int UnAuthorized = -1;
        public static int OK = 200;
        public static int Error_201 = 201;
        public static int Error_202 = 202;
        public static int Error_203 = 203;
        public static int Error_204 = 204;
        public static int Error_205 = 205;
        public static int Error_206 = 206;
        public static int Error_207 = 207;
        public static int Error_208 = 208;
        public static int Error_209 = 209;
        public static int Error_BadRequest = 400;
        public static int Error_ByServer = 500;
    }
}
