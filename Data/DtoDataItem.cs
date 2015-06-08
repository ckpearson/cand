using System;
using System.Collections.Generic;

namespace CandidateAssessment.Data
{
    public class DtoDataItem
    {
        private readonly string _account;
        private readonly string _description;
        private readonly string _currency;
        private readonly decimal _value;

        public const string AccountDbColName = "Account";
        public const string DescriptionDbColName = "Description";
        public const string CurrencyDbColName = "Currency";
        public const string ValueDbColName = "Value";

        public static IEnumerable<string> ColumnNames
        {
            get
            {
                return new[]
                {
                    AccountDbColName,
                    DescriptionDbColName,
                    CurrencyDbColName,
                    ValueDbColName,
                };
            }
        }

        public string Account
        {
            get { return _account; }
        }

        public string Description
        {
            get { return _description; }
        }

        public string Currency
        {
            get { return _currency; }
        }

        public decimal Value
        {
            get { return _value; }
        }

        private DtoDataItem(string account, string description, string currency, decimal value)
        {
            if (string.IsNullOrEmpty(account)) throw new ArgumentNullException("account");
            if (string.IsNullOrEmpty(description)) throw new ArgumentNullException("description");
            if (string.IsNullOrEmpty(currency)) throw new ArgumentNullException("currency");

            _account = account;
            _description = description;
            _currency = currency;
            _value = value;
        }

        public static DataItemOrValidationError FromData(Dictionary<string, object> columnarData)
        {
            if (columnarData == null) throw new ArgumentNullException("columnarData");

            try
            {
                var account = Convert.ToString(columnarData[AccountDbColName]);
                var desc = Convert.ToString(columnarData[DescriptionDbColName]);
                var curr = Convert.ToString(columnarData[CurrencyDbColName]);
                var val = columnarData[ValueDbColName];

                if (val == null)
                {
                    return new DataItemOrValidationError(null, "Value missing");
                }

                var decVal = Convert.ToDecimal(val);

                if (string.IsNullOrEmpty(account))
                {
                    return new DataItemOrValidationError(null, "Account missing");
                }

                if (string.IsNullOrEmpty(desc))
                {
                    return new DataItemOrValidationError(null, "Description missing");
                }

                if (string.IsNullOrEmpty(curr))
                {
                    return new DataItemOrValidationError(null, "Currency missing");
                }

                if (!CurrencyLookup.CurrencyExists(curr))
                {
                    return new DataItemOrValidationError(null, "Currency not a valid ISO 4217 value");
                }

                return new DataItemOrValidationError(new DtoDataItem(account, desc, curr, decVal), null);
            }
            catch (Exception ex)
            {
                return new DataItemOrValidationError(null, "ERROR: " + ex.Message);
            }
        }
    }
}