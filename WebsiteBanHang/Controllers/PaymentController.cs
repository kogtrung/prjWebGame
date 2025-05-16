using Microsoft.AspNetCore.Mvc;
using WebGame.Utilities;
using Microsoft.AspNetCore.Identity;
using WebGame.Models;

namespace WebGame.Controllers
{
    public class PaymentController : Controller
    {
        public IActionResult PayWithVnPay(decimal amount)
        {
            string vnp_ReturnUrl = "https://localhost:5001/Payment/PaymentReturn"; // chỉnh đúng URL khi publish
            string vnp_Url = "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html";
            string vnp_TmnCode = "2QXUI4J4"; // mã test sandbox
            string vnp_HashSecret = "YGHMZDFYZLONYYIBNZDSSJXGJHBSQKIV"; // mã test sandbox

            VnPayLibrary vnpay = new VnPayLibrary();
            vnpay.AddRequestData("vnp_Version", "2.1.0");
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnpay.AddRequestData("vnp_Amount", ((int)(amount * 100)).ToString()); // nhân 100 theo VNPAY
            vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", HttpContext.Connection.RemoteIpAddress?.ToString());
            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toán đơn hàng");
            vnpay.AddRequestData("vnp_OrderType", "billpayment");
            vnpay.AddRequestData("vnp_ReturnUrl", vnp_ReturnUrl);
            vnpay.AddRequestData("vnp_TxnRef", DateTime.Now.Ticks.ToString());

            string paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
            return Redirect(paymentUrl);
        }

        public IActionResult PaymentReturn()
        {
            var vnp_ResponseCode = Request.Query["vnp_ResponseCode"];
            var vnp_TransactionNo = Request.Query["vnp_TransactionNo"];

            if (vnp_ResponseCode == "00")
            {
                ViewBag.Status = "✔️ Thanh toán thành công!";
                ViewBag.TransactionNo = vnp_TransactionNo;
                // TODO: cập nhật trạng thái đơn hàng trong DB
            }
            else
            {
                ViewBag.Status = "❌ Thanh toán thất bại!";
            }

            return View();
        }
    }
}
