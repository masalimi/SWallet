using System;
using System.Collections.Generic;
using System.Text;
using NUglify.JavaScript.Syntax;

namespace Nop.Plugin.Payments.Wallet.Helper
{
    public class ApiErrorList
    {
        public string ReturnError(string errorCode)
        {
            switch (errorCode)
            {
                case "02":
                    return "تلاش مجدد";
                case "03":
                    return " یستماقتصادییافتنشدیاوضعیتسیستماقتصادیمعتبر نیست";
                case "06":
                    return " خطا در عملیات";
                case "30":
                    return "فرمت درخواست نامعتبر";
                case "51":
                    return "موجودی ناکافی";
                case "52":
                    return " حساب یافت نشد یا وضعیت حساب معتبر نیست";
                case "57":
                    return "حساب مسدود است";
                case "58":
                    return "تراکنش غیرمجاز";
                case "61":
                    return "نوع تراکنش یافت نشد یا وضعیت نوع تراکنش معتبر نیست";

                case "62":
                    return "شماره مرجع تکراریست";

                case "80":
                    return "مهلت زمانی ارسال تاییدیه به پایان رسیده است";

                case "81":
                    return "مهلت زمانی ارسال معکوس به پایان رسیده است";

                case "91":
                    return "تراکنش اصلی یافت نشد";

                case "92":
                    return "تراکنش برگشت خوردهاست";

                case "93":
                    return "وضعیت تراکنش اصلی نامعتبر است";

                case "94":
                    return "مدت زمان مجاز ارسال تراکنش برگشتی پایان یافته است";

                case "95":
                    return "برگشت تراکنش در وضعیت انتظار اجراست";

                case "7051":
                    return "نوع تراکنش ارسالی صحیح نیست";
                case "7002":
                    return "مقدار فیلد نوع تراکنش صحیح نیست";
                case "7003":
                    return " طول فیلد نوع تراکنش صحیح نیست";
                case "7004":
                    return "نوع تراکنش یافت نشد";
                case "7005":
                    return "فرمت نام نوع تراکنش صحیح نیست";
                case "7006":
                    return "نوع تراکنش ارسالی در حال حاضر موجود است";
                case "7007":
                    return "مقدارفیلد شناسه حساب صحیح نیست";
                case "7008":
                    return "مقدار فیلد شناسه نوع حساب صحیح نیست";
                case "7101":
                    return "InvalidEconomicSystemId";
                case "7102":
                    return "InvalidEconomicSystemName";
                case "7103":
                    return "EconomicSystemNotFound";
                case "7151":
                    return "AccountNotFound";
                case "7152":
                    return "InvalidAccountStatus";
                case "7153":
                    return "InsufficientBalance";
                case "7154":
                    return "InvalidAccountId";
                case "7155":
                    return "TransactionCreditorAccountsSumAmountsMismatch";
                case "7156":
                    return "DuplicateCreditorAccounts";
                case "7157":
                    return "DuplicateDebtorAccounts";
                case "7158":
                    return "CreditorAccountTypeNotSupportedByRequestTransactionType";
                case "7159":
                    return "CreditorAccountNotSupportedByRequestTransactionType";
                case "7160":
                    return "DebtorAccountTypeNotSupportedByRequestTransactionType";
                case "7161":
                    return "DebtorAccountNotSupportedByRequestTransactionType";
                case "7162":
                    return "TransactionDebtorAccountsSumAmountsMismatch";
                case "7201":
                    return "EmptyTransactionReferenceNo";
                case "7202":
                    return "TransactionReferenceNoInvalidLength";
                case "7251":
                    return "DuplicateTransactionReferenceNo";
                case "7300":
                    return "MaxCreditorBalanceExceeded";
                case "7301":
                    return "MaxDebtorBalanceExceeded";
                case "7351":
                    return "EmptyAmountField";
                case "7352":
                    return "TransactionTypeMaxAmountExceeded";
                case "7353":
                    return "TransactionTypeMinAmountExceeded";
                case "7401":
                    return "EmptyRefundRequestEconomicSystemId";
                case "7402":
                    return "InvalidRefundRequestDescriptionLength";
                case "7403":
                    return "EmptyRefundRequestReferenceNo";
                case "7404":
                    return "OriginalTransactionNotFoundForRefund";
                case "7405":
                    return "EmptyRefundRequestTransactionId";
                case "7406":
                    return "TransactionAlreadyRefunded";
                case "7407":
                    return "OriginalTransactionNotAllowedForRefund";
                case "7408":
                    return "RefundTimeout";
                case "7409":
                    return "RefundTransactionIsPending";
                case "7410":
                    return "TransactionNotSupportedForRefund";
                case "7451":
                    return "InvalidReferenceAccountId";
                case "7501":
                    return "MaxTransactionCountForCheckpointNotExceeded";
                case "7601":
                    return "TransactionRequestNotFound";
                case "7602":
                    return "TransactionRequestInvalidStatus";
                case "7606":
                    return "InvalidTransactionTypeStatus";
                case "7651":
                    return "InvalidMaxTransactionAmountValue";
                case "7701":
                    return "TransactionNotSupportedForVerify";
                case "7702":
                    return "VerifyTimeout";
                case "7703":
                    return "EmptyVerifyRequestEconomicSystemId";
                case "7704":
                    return "EmptyVerifyRequestReferenceNo";
                case "7801":
                    return "EmptyReverseRequestEconomicSystemId";
                case "7803":
                    return "EmptyReverseRequestReferenceNo";
                case "7805":
                    return "EmptyReverseRequestTransactionId";
                case "7808":
                    return "ReverseTimeout";
                case "7900":
                    return "خطای داخلی سرویس تراکنش";
                case "7901":
                    return "خطای همزمانی تراکنش";
                default:
                    return "";
            }
        }
    }
}
