namespace KTBE.Models.ViewModel
{
    using System.ComponentModel.DataAnnotations;

    public class RegisterVM
    {
        [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Tên đăng nhập phải từ 3 - 50 ký tự")]
        [Display(Name = "Tên đăng nhập")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải từ 6 ký tự trở lên")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Vui lòng xác nhận mật khẩu")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp")]
        [Display(Name = "Xác nhận mật khẩu")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập họ và tên")]
        [StringLength(100, ErrorMessage = "Họ và tên tối đa 100 ký tự")]
        [Display(Name = "Họ và tên")]
        public string CustomerName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [StringLength(15, MinimumLength = 9, ErrorMessage = "Số điện thoại phải từ 9 - 15 số")]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Số điện thoại")]
        public string CustomerPhone { get; set; }

        [EmailAddress(ErrorMessage = "Địa chỉ email không đúng định dạng")]
        [Display(Name = "Email")]
        public string CustomerEmail { get; set; }

        [Display(Name = "Địa chỉ nhận hàng")]
        public string CustomerAddress { get; set; }
    }
}
