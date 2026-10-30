using BankSystemApp;
using Microsoft.AspNetCore.Mvc;

namespace BankSystemApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly TransactionRepository _transactionRepository;

        public TransactionsController()
        {
            _transactionRepository = new TransactionRepository();
        }

        // 1. GET: جلب حركات حساب معين بناءً على الـ AccountID
        [HttpGet("account/{accountId}")]
        public IActionResult GetTransactionsByAccountId(int accountId)
        {
            try
            {
                var transactions = _transactionRepository.GetTransactionsByAccountId(accountId);
                return Ok(transactions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // 2. POST: إجراء عملية تحويل آمنة بين حسابين
        [HttpPost("transfer")]
        public IActionResult Transfer([FromQuery] int sourceId, [FromQuery] int destId, [FromQuery] decimal amount)
        {
            try
            {
                bool success = _transactionRepository.Transfer(sourceId, destId, amount);
                if (success)
                    return Ok(new { message = "تمت عملية التحويل بنجاح تام وبشكل آمن!" });

                return BadRequest(new { message = "فشلت عملية التحويل (تأكد من رصيد الحساب أو صحة الأرقام)." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}