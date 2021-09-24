using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class AuthSenderOptions
    {
        private readonly string user = "Food Delivery"; // The name you want to show up on your email
                                                        // Make sure the string passed in below matches your API Key
        private readonly string key = "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";
        public string SendGridUser { get { return user; } }
        public string SendGridKey { get { return key; } }

    }

}
