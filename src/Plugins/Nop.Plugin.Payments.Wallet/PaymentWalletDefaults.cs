namespace Nop.Plugin.Payments.Wallet
{
    /// <summary>
    /// Represents Qualpay payment gateway constants
    /// </summary>
    public class PaymentWalletDefaults
    {
        /// <summary>
        /// Name of the view component to display payment info in public store
        /// </summary>
        public const string PAYMENT_INFO_VIEW_COMPONENT_NAME = "QualpayPaymentInfo";

        /// <summary>
        /// Name of the view component to disaply Qualpay Customer Vault block on the customer details page
        /// </summary>
        public const string CUSTOMER_VIEW_COMPONENT_NAME = "QualpayCustomer";

        /// <summary>
        /// Qualpay payment method system name
        /// </summary>
        public static string SystemName => "Payments.Wallet";

        /// <summary>
        /// User agent using for requesting Qualpay services
        /// </summary>
        public static string UserAgent => "nopCommerce-plugin";

        /// <summary>
        /// nopCommerce developer application ID
        /// </summary>
        public static string DeveloperId => "nopCommerce";

        /// <summary>
        /// Numeric ISO code of the USD currency
        /// </summary>
        public static int UsdNumericIsoCode => 840;

        

        /// <summary>
        /// Webhook route name
        /// </summary>
        public static string PaymentWalletRouteName => "Plugin.Payments.Wallet.PaymentWallet";

    }
}