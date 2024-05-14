using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JavaExam
{
    public static class GlobalUser
    {
      public static Studenti LoggedInUser { get; set; }
        public static bool SpecialUser {get; set;}
    }
}
