using System;
namespace Argilla.Core.Entities.Setting
{

    public class Node
    {
        public string BaseAddress { get; set; }
        public string ServiceName { get; set; }

        public string Return
        {
            get
            {
                if (String.IsNullOrWhiteSpace(BaseAddress))
                {
                    return "";
                }

                if (BaseAddress.EndsWith("/"))
                {
                    return BaseAddress + "return";
                }
                else
                {
                    return BaseAddress + "/return";
                }
            }
        }

        public string EndpointSync
        {
            get
            {
                if (String.IsNullOrWhiteSpace(BaseAddress))
                {
                    return "";
                }

                if (BaseAddress.EndsWith("/"))
                {
                    return BaseAddress + "callback";
                }
                else
                {
                    return BaseAddress + "/callback";
                }
            }
        }

        public string EndpointAsync
        {
            get
            {
                if (String.IsNullOrWhiteSpace(BaseAddress))
                {
                    return "";
                }

                if (BaseAddress.EndsWith("/"))
                {
                    return BaseAddress + "callbackasync";
                }
                else
                {
                    return BaseAddress + "/callbackasync";
                }
            }
        }
    }
}
