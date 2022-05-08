using System;
using System.Collections.Generic;

namespace BSCCSL.Models
{
    public class AccountDetail
    {
        public ProductType ProductType { get; set; }
        public string AccountNo { get; set; }
        public decimal Balance { get; set; }
        public decimal? UnclearBalance { get; set; }
        public bool? IsFreeze { get; set; }
        public List<MiniStatement> MiniStatement { get; set; }
        public Guid CustomerProductId { get; set; }
        public decimal PendingEMIAmount { get; set; }
    }

    public class MiniStatement
    {
        public DateTime? Date { get; set; }
        public decimal? Amount { get; set; }
        public TypeCRDR? Type { get; set; }
        public string Remarks { get; set; }
    }
}