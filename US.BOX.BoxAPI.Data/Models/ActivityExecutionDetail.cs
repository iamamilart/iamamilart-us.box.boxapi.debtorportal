using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace US.BOX.BoxAPI.Data.Models
{
    public class ActivityExecutionDetail
    {
        private int _id = -1;
        private string _name = string.Empty;
        private string _type = string.Empty;
        private string _settings = string.Empty;
        private string _reference = string.Empty;
        private string _entityType = string.Empty;
        private string _workFlowType = string.Empty;
        private int _caseNo = -1;
        private string _categoryName = string.Empty;
        private string _activityVersion = string.Empty;
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        public string Type
        {
            get { return _type; }
            set { _type = value; }
        }
        public string Settings
        {
            get { return _settings; }
            set { _settings = value; }
        }
        public string Reference
        {
            get { return _reference; }
            set { _reference = value; }
        }
        public string EntityType
        {
            get { return _entityType; }
            set { _entityType = value; }
        }

        public string WorkFlowType
        {
            get { return _workFlowType; }
            set { _workFlowType = value; }
        }

        public int CaseNo
        {
            get { return _caseNo; }
            set { _caseNo = value; }
        }

        public string CategoryName
        {
            set { _categoryName = value; }
            get { return _categoryName; }
        }

        public string MessageType { get; set; }
        public int? defaultTemplateId { get; set; }

        public string ActivityVersion
        {
            get { return _activityVersion; }
            set { _activityVersion = value; }
        }
    }
}
