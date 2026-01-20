using NetDevPack.Domain;
using System;
using System.ComponentModel.DataAnnotations;

namespace Marko.IMS.Domain.Core.Models
{
    public abstract class BaseEntity : Entity
    {
        [MaxLength(50)]
        public string CreatedBy { get; set; }
        public DateTime? CreatedDateTime { get; set; }
        [MaxLength(50)]
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDateTime { get; set; }
        public bool IsActive { get; set; }
    }
}
