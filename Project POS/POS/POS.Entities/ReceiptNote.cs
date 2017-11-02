// <auto-generated>
// ReSharper disable ConvertPropertyToExpressionBody
// ReSharper disable DoNotCallOverridableMethodsInConstructor
// ReSharper disable EmptyNamespace
// ReSharper disable InconsistentNaming
// ReSharper disable PartialMethodWithSinglePart
// ReSharper disable PartialTypeWithSinglePart
// ReSharper disable RedundantNameQualifier
// ReSharper disable RedundantOverridenMember
// ReSharper disable UseNameofExpression
// TargetFrameworkVersion = 4.6
#pragma warning disable 1591    //  Ignore "Missing XML Comment" warning


namespace POS.Entities
{

    // ReceiptNote
    [System.CodeDom.Compiler.GeneratedCode("EF.Reverse.POCO.Generator", "2.33.0.0")]
    public partial class ReceiptNote
    {
        public string RnId { get; set; } // rn_id (Primary key) (length: 10)
        public string EmpId { get; set; } // emp_id (length: 10)
        public System.DateTime Inday { get; set; } // inday
        public decimal TotalAmount { get; set; } // total_amount

        // Reverse navigation

        /// <summary>
        /// Child ReceiptNoteDetails where [ReceiptNoteDetails].[rn_id] point to this entity (fk_receivenoteid)
        /// </summary>
        public virtual System.Collections.Generic.ICollection<ReceiptNoteDetail> ReceiptNoteDetails { get; set; } // ReceiptNoteDetails.fk_receivenoteid

        // Foreign keys

        /// <summary>
        /// Parent Employee pointed by [ReceiptNote].([EmpId]) (fk_employee_buy)
        /// </summary>
        public virtual Employee Employee { get; set; } // fk_employee_buy

        public ReceiptNote()
        {
            ReceiptNoteDetails = new System.Collections.Generic.List<ReceiptNoteDetail>();
            InitializePartial();
        }

        partial void InitializePartial();
    }

}
// </auto-generated>