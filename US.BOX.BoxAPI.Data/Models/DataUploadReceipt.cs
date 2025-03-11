using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace US.BOX.BoxAPI.Data.Models
{
    public class DataUploadReceipt
    {
        public DataUploadReceipt()
        {
            _receiptNumber = string.Empty;
            _fileStatus = 0;
            _noOfInvoices = 0;
            _totalValueOfInvoices = 0;
            _errorList = new List<Error>();
            _nextRequestNo = 0;
        }

        private string _receiptNumber;
        public string ReceiptNumber
        {
            get
            {
                return _receiptNumber;
            }
            set
            {
                _receiptNumber = value;
            }
        }

        private string _userName;
        public string UserName
        {
            get
            {
                return _userName;
            }
            set
            {
                _userName = value;
            }
        }

        private int _fileStatus;
        public int FileStatus
        {
            get
            {
                return _fileStatus;
            }
            set
            {
                _fileStatus = value;
            }
        }

        private double _noOfInvoices;
        public double NoOfInvoices
        {
            get
            {
                return _noOfInvoices;
            }
            set
            {
                _noOfInvoices = value;
            }
        }


        private double _totalValueOfInvoices;
        public double TotalValueOfInvoices
        {
            get
            {
                return _totalValueOfInvoices;
            }
            set
            {
                _totalValueOfInvoices = value;
            }
        }

        private List<Error> _errorList;
        public List<Error> ErrorList
        {
            get
            {
                return _errorList;
            }
            set
            {
                _errorList = value;
            }
        }

        private int _noOfTransactions = 0;
        public int NoOfTransactions
        {
            get
            {
                return _noOfTransactions;
            }

            set
            {
                _noOfTransactions = value;
            }
        }

        private decimal _transactionSum = 0;
        public decimal TransactionSum
        {
            get
            {
                return _transactionSum;
            }

            set
            {
                _transactionSum = value;
            }
        }

        [NonSerialized]
        private int _nextRequestNo = 0;

        public int NextRequestNo
        {
            get { return _nextRequestNo; }
            set { _nextRequestNo = value; }
        }



        private int _noOfCreditNotes = 0;
        public int NoOfCreditNotes
        {
            get
            {
                return _noOfCreditNotes;
            }

            set
            {
                _noOfCreditNotes = value;
            }
        }

        private decimal _creditNoteSum = 0;
        public decimal CreditNoteSum
        {
            get
            {
                return _creditNoteSum;
            }

            set
            {
                _creditNoteSum = value;
            }
        }

        public int NoOfNotes
        {
            get
            {
                return _noOfNotes;
            }

            set
            {
                _noOfNotes = value;
            }
        }

        private int _noOfNotes = 0;


    }
}
