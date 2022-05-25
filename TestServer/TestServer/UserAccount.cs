using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace TestServer
{
    [System.Serializable]
    public class UserAccount
    {
        public string ID { get; set; } = "None";
        public string PW { get; set; } = "None";
    }

    public static class UserAccountExtensions
    {
        public static UserAccount GetValueOrDefault(this UserAccount? _userAccount) => _userAccount == null ? new UserAccount() : _userAccount;     
    }
}