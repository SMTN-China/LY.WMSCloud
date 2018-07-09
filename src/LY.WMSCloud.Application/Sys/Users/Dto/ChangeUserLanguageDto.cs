using System.ComponentModel.DataAnnotations;

namespace LY.WMSCloud.Users.Dto
{
    public class ChangeUserLanguageDto
    {
        [Required]
        public string LanguageName { get; set; }
    }
}