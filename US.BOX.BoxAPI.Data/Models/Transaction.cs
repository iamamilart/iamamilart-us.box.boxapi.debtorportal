using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace US.BOX.BoxAPI.Data.Models
{
    public class Transaction
    {
        private string _pid = string.Empty;

        private string _cid = string.Empty;

        private string transtype = string.Empty;

        private string _TNumber = string.Empty;

        private string _tCaseNumner;

        private DateTime _TDate = DateTime.Now;

        private DateTime _TDueDate = DateTime.Now;

        private double _TAmount = 0.0;

        private string _TInfo = string.Empty;

        private double _TInterestRate = 0.0;

        private string _TInterestDate = string.Empty;

        private string _TKid = string.Empty;

        private string _TCreditinvoice = string.Empty;

        private string _TCreditInvoice = string.Empty;

        private string _aIGBranchNumber = string.Empty;

        private string _aIGAccountno = string.Empty;

        private string _aIGContract = string.Empty;

        private string _aIGContractExpire = DateTime.MinValue.ToShortDateString();

        private string _aIGLastVisit = DateTime.MinValue.ToShortDateString();

        private string _aIGLastContact = string.Empty;

        private string _aIGCKID = string.Empty;

        private string _aIGCStatus = string.Empty;

        private string _aIGInvoiceReference = string.Empty;

        private double _aIGBalance = 0.0;

        private double _aIGDueBalance = 0.0;

        private string _aIGPrintDate = string.Empty;

        private string _aIGOriginDueDate = string.Empty;

        private string _aIGPaidDate = string.Empty;

        private string _aIGLastReminder = string.Empty;

        private double _aIGRAmount = 0.0;

        private string _aIGDDStatus = string.Empty;

        private string _aIGDDText = string.Empty;

        private string _tStatus = string.Empty;

        private string _errorMsg = string.Empty;

        private List<CustomSetting> _customSettings = new List<CustomSetting>();

        [DataMember]
        public string PID
        {
            get
            {
                return _pid;
            }
            set
            {
                _pid = value;
            }
        }

        [DataMember]
        public string CID
        {
            get
            {
                return _cid;
            }
            set
            {
                _cid = value;
            }
        }

        [DataMember]
        public string TType
        {
            get
            {
                return transtype;
            }
            set
            {
                transtype = value;
            }
        }

        [DataMember]
        public string TNumber
        {
            get
            {
                return _TNumber;
            }
            set
            {
                _TNumber = value;
            }
        }

        public string TCaseNumner
        {
            get
            {
                return _tCaseNumner;
            }
            set
            {
                _tCaseNumner = value;
            }
        }

        [DataMember]
        public DateTime TDate
        {
            get
            {
                return _TDate;
            }
            set
            {
                _TDate = value;
            }
        }

        [DataMember]
        public DateTime TDueDate
        {
            get
            {
                return _TDueDate;
            }
            set
            {
                _TDueDate = value;
            }
        }

        [DataMember]
        public double TAmount
        {
            get
            {
                return _TAmount;
            }
            set
            {
                _TAmount = value;
            }
        }

        [DataMember]
        public string TInfo
        {
            get
            {
                return _TInfo;
            }
            set
            {
                _TInfo = value;
            }
        }

        [DataMember]
        public double TInterestRate
        {
            get
            {
                return _TInterestRate;
            }
            set
            {
                _TInterestRate = value;
            }
        }

        [DataMember]
        public string TInterestDate
        {
            get
            {
                return _TInterestDate;
            }
            set
            {
                _TInterestDate = value;
            }
        }

        [DataMember]
        public string TKid
        {
            get
            {
                return _TKid;
            }
            set
            {
                _TKid = value;
            }
        }

        public string TCreditinvoice
        {
            get
            {
                return _TCreditinvoice;
            }
            set
            {
                _TCreditinvoice = value;
            }
        }

        [DataMember]
        public string TCreditInvoice
        {
            get
            {
                return _TCreditInvoice;
            }
            set
            {
                _TCreditInvoice = value;
            }
        }

        public string AIGBranchNumber
        {
            get
            {
                return _aIGBranchNumber;
            }
            set
            {
                _aIGBranchNumber = value;
            }
        }

        public string AIGAccountno
        {
            get
            {
                return _aIGAccountno;
            }
            set
            {
                _aIGAccountno = value;
            }
        }

        public string AIGContract
        {
            get
            {
                return _aIGContract;
            }
            set
            {
                _aIGContract = value;
            }
        }

        public string AIGContractExpire
        {
            get
            {
                return _aIGContractExpire;
            }
            set
            {
                _aIGContractExpire = value;
            }
        }

        public string AIGLastVisit
        {
            get
            {
                return _aIGLastVisit;
            }
            set
            {
                _aIGLastVisit = value;
            }
        }

        public string AIGLastContact
        {
            get
            {
                return _aIGLastContact;
            }
            set
            {
                _aIGLastContact = value;
            }
        }

        public string AIGCKID
        {
            get
            {
                return _aIGCKID;
            }
            set
            {
                _aIGCKID = value;
            }
        }

        public string AIGCStatus
        {
            get
            {
                return _aIGCStatus;
            }
            set
            {
                _aIGCStatus = value;
            }
        }

        public string AIGInvoiceReference
        {
            get
            {
                return _aIGInvoiceReference;
            }
            set
            {
                _aIGInvoiceReference = value;
            }
        }

        public double AIGBalance
        {
            get
            {
                return _aIGBalance;
            }
            set
            {
                _aIGBalance = value;
            }
        }

        public double AIGDueBalance
        {
            get
            {
                return _aIGDueBalance;
            }
            set
            {
                _aIGDueBalance = value;
            }
        }

        public string AIGPrintDate
        {
            get
            {
                return _aIGPrintDate;
            }
            set
            {
                _aIGPrintDate = value;
            }
        }

        public string AIGOriginDueDate
        {
            get
            {
                return _aIGOriginDueDate;
            }
            set
            {
                _aIGOriginDueDate = value;
            }
        }

        public string AIGPaidDate
        {
            get
            {
                return _aIGPaidDate;
            }
            set
            {
                _aIGPaidDate = value;
            }
        }

        public string AIGLastReminder
        {
            get
            {
                return _aIGLastReminder;
            }
            set
            {
                _aIGLastReminder = value;
            }
        }

        public double AIGRAmount
        {
            get
            {
                return _aIGRAmount;
            }
            set
            {
                _aIGRAmount = value;
            }
        }

        public string AIGDDStatus
        {
            get
            {
                return _aIGDDStatus;
            }
            set
            {
                _aIGDDStatus = value;
            }
        }

        public string AIGDDText
        {
            get
            {
                return _aIGDDText;
            }
            set
            {
                _aIGDDText = value;
            }
        }

        [DataMember]
        public string TStatus
        {
            get
            {
                return _tStatus;
            }
            set
            {
                _tStatus = value;
            }
        }

        public string ErrorMsg
        {
            get
            {
                return _errorMsg;
            }
            set
            {
                _errorMsg = value;
            }
        }

        [DataMember]
        public List<CustomSetting> CustomSettings
        {
            get
            {
                return _customSettings;
            }
            set
            {
                _customSettings = value;
            }
        }
    }
    }
