using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSCCSL.Services;
namespace BSCCSL.Console
{
    class Program
    {
        
        static void Main(string[] args)
        {
            SchedulerService schedulerService = new SchedulerService();
            schedulerService.RunSchedular();   
        }
    }
}
