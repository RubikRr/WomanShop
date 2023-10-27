using System.ComponentModel.DataAnnotations;

namespace WomanShop.Areas.Admin.Models
{
    public class EditUserByAdminViewModel
    {
        public string Id { get; set; }
        [Required(ErrorMessage = "Не указана почта")]
        [EmailAddress(ErrorMessage = "Введите валидный email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Введите ваше имя")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Впишите номер телефона")]
        public string Phone { get; set; }
        [Required(ErrorMessage = "Задайте права пользователя")]
        public string RoleName { get; set; }

    }
}
