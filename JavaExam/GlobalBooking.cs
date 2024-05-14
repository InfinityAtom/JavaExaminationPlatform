using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JavaExam
{
    public static class GlobalBooking
    {
        public static Booking CurrentBooking { get; private set; }

        public static void FetchBookingByStudentId(int studentId)
        {
            using (var context = new JavaExamContext())
            {
                // Assuming there's only one booking for a student. Adjust as needed.
                CurrentBooking = context.Bookings.FirstOrDefault(b => b.StudentId == studentId);
            }
        }
    }
}
