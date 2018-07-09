using LY.WMSCloud.Users.Dto;

namespace LY.WMSCloud.Models.TokenAuth
{
    public class AuthenticateResultModel
    {
        public string AccessToken { get; set; }

        public string EncryptedAccessToken { get; set; }

        public int ExpireInSeconds { get; set; }

        public UserDto UserInfo { get; set; }
    }
}
