using System;
using System.Net;

namespace SharpVectors.Net
{
    public static class DataSecurityProtocols
    {
        private static bool _isIntialized = false;

        public static void Initialize()
        {
            if (_isIntialized)
            {
                return;
            }
#if DOTNET40
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                    | SecurityProtocolTypes.Tls11 | SecurityProtocolTypes.Tls12;
            }
            catch
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls;
                return;
            }
#else
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls 
                | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
#endif
            _isIntialized = true;
        }

#if DOTNET40
        private static class SecurityProtocolTypes
        {
            public const SecurityProtocolType Tls12 = (SecurityProtocolType)0x00000C00;
            public const SecurityProtocolType Tls11 = (SecurityProtocolType)0x00000300;
            public const SecurityProtocolType SystemDefault = (SecurityProtocolType)0;
        }
#endif
    }
}
