using System;

namespace Xpinn.SportsGo.Entities
{
    public class PayUConfirmationModel
    {
        public int merchantId { get; set; }
        public string state_pol { get; set; }
        public decimal risk { get; set; }
        public string response_code_pol { get; set; }
        public string reference_sale { get; set; }
        public string reference_pol { get; set; }
        public string sign { get; set; }
        public string extra1 { get; set; }
        public string extra2 { get; set; }
        public int payment_method { get;  set; }
        public int payment_method_type { get; set; }
        public int installments_number { get; set; }
        public int value { get; set; }
        public int tax { get; set; }
        public int additional_value { get; set; }
        public DateTime transaction_date { get; set; }
        public string currency { get; set; }
        public string email_buyer { get; set; }
        public string cus { get; set; }
        public string pse_bank { get; set; }
        public bool test { get; set; }
        public string description { get; set; }
        public string billing_address { get; set; }
        public string shipping_address { get; set; }
        public string phone { get; set; }
        public string office_phone { get; set; }
        public string account_number_ach { get; set; }
        public string account_type_ach { get; set; }
        public decimal administrative_fee { get; set; }
        public decimal administrative_fee_base { get; set; }
        public decimal administrative_fee_tax { get; set; }
        public string airline_code { get; set; }
        public int attempts { get; set; }
        public string authorization_code { get; set; }
        public string bank_id { get; set; }
        public string billing_city { get; set; }
        public string billing_country { get; set; }
        public decimal commision_pol { get; set; }
        public string commision_pol_currency { get; set; }
        public int customer_number { get; set; }
        public DateTime date { get; set; }
        public string error_code_bank { get; set; }
        public string error_message_bank { get; set; }
        public decimal exchange_rate { get; set; }
        public string ip { get; set; }
        public string nickname_buyer { get; set; }
        public string nickname_seller { get; set; }
        public int payment_method_id { get; set; }
        public string payment_request_state { get; set; }
        public string pseReference1 { get; set; }
        public string pseReference2 { get; set; }
        public string pseReference3 { get; set; }
        public string response_message_pol { get; set; }
        public string shipping_city { get; set; }
        public string shipping_country { get; set; }
        public string transaction_bank_id { get; set; }
        public string transaction_id { get; set; }
        public string payment_method_name { get; set; }

        public bool TransaccionFueAprobada
        {
            get
            {
                return state_pol == "4";
            }
        }
    }
}
