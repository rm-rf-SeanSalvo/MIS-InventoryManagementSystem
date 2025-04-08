﻿    using System.ComponentModel.DataAnnotations;

    namespace InventoryManagementSystem2.Models
    {
        public class LoginViewModel
        {
            [Required(ErrorMessage = "Username is required!!!")]
            [Display(Name = "Username")]
            public string Username { get; set; }

            [Required(ErrorMessage = "Password is required")]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

        }
    }
