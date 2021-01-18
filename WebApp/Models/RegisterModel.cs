using System.ComponentModel.DataAnnotations;

namespace network2.Models
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "Не указан никнейм")]
        public string Nickname { get; set; }

        //[Required(ErrorMessage = "Не указан Email")]
        public string Email { get; set; }

        //[Required(ErrorMessage = "Не указан пароль")]
        //[DataType(DataType.Password)]
        public string Password { get; set; }

        //[DataType(DataType.Password)]
        //[Compare("Password", ErrorMessage = "Пароли не совпадают")]
        //[Required(ErrorMessage = "Не введено подтверждение пароля")]
        public string ConfirmPassword { get; set; }
        //[Required(ErrorMessage = "Не указано имя")]
        public string Name { get; set; }
        //[Required(ErrorMessage = "Не указана фамилия")]
        public string Surname { get; set; }
        //[Required(ErrorMessage = "Не указана дата рождения")]
        public string Bday { get; set; }
    }
}