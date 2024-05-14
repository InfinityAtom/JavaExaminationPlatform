using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JavaExam
{
    public static class GlobalProctor
    {
        public static Proctor LoggedInProctor { get; set; }
        public static void FetchProctorByEmailAndPassword(string email, string password)
        {
            using (var context = new JavaExamContext())
            {
                LoggedInProctor = context.Proctors.FirstOrDefault(p => p.Email == email && p.Password == password);
            }
        }
    }
}
