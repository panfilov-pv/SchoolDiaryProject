﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolDiary.Domain.Models.Authentication
{
    /// <summary>
    /// Represents base register model, this
    /// data is expected to come from frontend.
    /// </summary>
    public class BaseRegisterModel
    {
        [Required]
        [MinLength(4, ErrorMessage = "Длина для логина не меньше 4 символов!")]
        public string Login { get; set; }
        [Required]
        [MinLength(6, ErrorMessage = "Длина пароля не меньше 6 символов!")]
        public string Password { get; set; }
        [Required]
        [MinLength(2, ErrorMessage = "Минимальная длина имени - 2 символа!")]
        public string Firstname { get; set; }
        [Required]
        [MinLength(2, ErrorMessage = "Минимальная длина фамилии - 2 символа!")]
        public string Lastname { get; set; }
        [Required]
        [MinLength(2, ErrorMessage = "Минимальная длина отчества - 2 символа!")]
        public string Patronymic { get; set; }
        public string Phone { get; set; }
        [Required(ErrorMessage = "Не выбрана роль пользователя!")]
        public int RoleId { get; set; }
    }
}
