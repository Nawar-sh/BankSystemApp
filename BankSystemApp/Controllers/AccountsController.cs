using BankSystemApp;
using Microsoft.AspNetCore.Mvc;

namespace BankSystemApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly AccountRepository _accountRepository;

        public AccountsController()
        {
            _accountRepository = new AccountRepository();
        }

        // 1. GET: جلب كل الحسابات من قاعدة البيانات
        [HttpGet]
        public IActionResult GetAllAccounts()
        {
            try
            {
                var accounts = _accountRepository.GetAllAccounts();
                return Ok(accounts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // 2. GET BY ID: جلب حساب برقم الـ ID
        [HttpGet("{id}")]
        public IActionResult GetAccountById(int id)
        {
            try
            {
                var account = _accountRepository.GetAccountById(id);
                if (account == null)
                    return NotFound(new { message = $"الحساب برقم {id} غير موجود." });

                return Ok(account);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // 3. POST: إنشاء حساب جديد في قاعدة البيانات
        [HttpPost("create")]
        public IActionResult CreateAccount([FromBody] Account account)
        {
            try
            {
                bool success = _accountRepository.CreateAccount(account);
                if (success)
                    return Ok(new { message = "تم إنشاء الحساب بنجاح في قاعدة البيانات!", data = account });

                return BadRequest(new { message = "فشل إنشاء الحساب." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // 4. PUT: إلغاء تفعيل حساب (Soft Delete)
        [HttpPut("deactivate/{id}")]
        public IActionResult DeactivateAccount(int id)
        {
            try
            {
                bool success = _accountRepository.DeactivateAccount(id);
                if (success)
                    return Ok(new { message = $"تم إغلاق الحساب #{id} بنجاح." });

                return BadRequest(new { message = "فشل إغلاق الحساب." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // 5. PUT: إيداع مبلغ في الحساب
        [HttpPut("deposit/{id}")]
        public IActionResult Deposit(int id, [FromQuery] decimal amount)
        {
            try
            {
                bool success = _accountRepository.Deposit(id, amount);
                if (success)
                    return Ok(new { message = $"تم إيداع مبلغ {amount} بنجاح للحساب #{id}." });

                return BadRequest(new { message = "فشل عملية الإيداع (تأكد أن الحساب نشط)." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}