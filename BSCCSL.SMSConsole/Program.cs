using BSCCSL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSCCSL.SMSConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            SchedulerService schedulerService = new SchedulerService();
            schedulerService.RunSMSSchedular();
        }
    }
}
