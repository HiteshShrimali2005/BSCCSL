using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSCCSL.Models
{
    public class ErrorLog
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [StringLength(1000)]
        public string Message { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        public string UserId { get; set; }

        private LogLevel LogLevelId { get; set; }
        public LogLevel LogLevel
        {
            get { return LogLevelId; }
            set
            {
                LogLevelId = value;
                LogLevelName = Enum.GetName(typeof(LogLevel), value).Replace('_', ' ');

            }
        }

        public DateTime CreateDate { get; set; }
        [NotMapped]
        public string LogLevelName { get; set; }

        public ErrorLog()
        {
            CreateDate = DateTime.Now;
        }

    }

    public enum LogLevel
    {

        High = 1,
        Medium = 2,
        Low = 3
    }
}
