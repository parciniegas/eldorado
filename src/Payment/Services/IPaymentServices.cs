using Pay = Payment.Model.Payment;

namespace Payment.Services;

public interface IPaymentServices
{
    bool PayOrder(Pay payment);
    bool RefundOrder(Pay payment);
}
