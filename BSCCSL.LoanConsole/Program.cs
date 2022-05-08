using BSCCSL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSCCSL.LoanConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            LoanSchedulerService schedulerService = new LoanSchedulerService();
            schedulerService.RunSchedular();
        }
    }
}
