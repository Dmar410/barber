using Microsoft.AspNetCore.Mvc;
using AppointmentApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;

namespace AppointmentApp.Controllers
{
    public class AppointmentsController : Controller
    {
        private static List<Appointment> appointments = new List<Appointment>();

        public IActionResult MakeAppointment()
        {
            ViewBag.AvailableTimes = GetAvailableTimes(DateTime.Now.ToString("yyyy-MM-dd"));
            return View();
        }

        [HttpPost]
        public IActionResult ConfirmAppointment(Appointment appointment)
        {
            if (!DateTime.TryParse(appointment.Date, out DateTime selectedDate))
            {
                ModelState.AddModelError("Date", "Invalid date format.");
                ViewBag.AvailableTimes = GetAvailableTimes(appointment.Date);
                return View("MakeAppointment", appointment);
            }

            if (IsInvalidDate(selectedDate))
            {
                ModelState.AddModelError("Date", "Appointments are not available on Sundays and Mondays.");
                ViewBag.AvailableTimes = GetAvailableTimes(appointment.Date);
                return View("MakeAppointment", appointment);
            }

            if (!IsTimeAvailable(appointment.Date, appointment.Time))
            {
                ModelState.AddModelError("Time", "This time slot is already booked.");
                ViewBag.AvailableTimes = GetAvailableTimes(appointment.Date);
                return View("MakeAppointment", appointment);
            }

            appointments.Add(appointment);

            // Send Email Notifications
            SendEmailNotification(appointment); // Sends notification to the business
            SendConfirmationEmail(appointment); // Sends confirmation to the customer

            return RedirectToAction("Confirmation", new { date = appointment.Date, time = appointment.Time, name = appointment.Name });
        }

        public IActionResult Confirmation(string date, string time, string name)
        {
            ViewBag.Date = date;
            ViewBag.Time = time;
            ViewBag.Name = name;
            return View();
        }

        private List<string> GetAvailableTimes(string date)
        {
            if (!DateTime.TryParse(date, out DateTime parsedDate) || IsInvalidDate(parsedDate))
                return new List<string>();

            List<string> times = new List<string>();
            DateTime startTime = parsedDate.Date.AddHours(9);
            DateTime endTime = parsedDate.Date.AddHours(18);

            while (startTime < endTime)
            {
                if (!appointments.Any(a => a.Date == date && IsTimeBlocked(a.Time, startTime)))
                {
                    times.Add(startTime.ToString("hh:mm tt"));
                }
                startTime = startTime.AddMinutes(75); // 1 hour + 15 min break
            }

            return times;
        }

        private bool IsInvalidDate(DateTime date)
        {
            return date.DayOfWeek == DayOfWeek.Sunday || date.DayOfWeek == DayOfWeek.Monday;
        }

        private bool IsTimeBlocked(string bookedTime, DateTime checkTime)
        {
            if (!DateTime.TryParse(bookedTime, out DateTime bookedStart))
                return false;

            DateTime bookedEnd = bookedStart.AddMinutes(75);
            return checkTime >= bookedStart && checkTime < bookedEnd;
        }

        private bool IsTimeAvailable(string date, string time)
        {
            return !appointments.Any(a => a.Date == date && IsTimeBlocked(a.Time, DateTime.Parse(time)));
        }

        // Sends notification email to the business
        private void SendEmailNotification(Appointment appointment)
        {
            try
            {
                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress("youngdmar410@gmail.com"); // Your business email
                    mail.To.Add("youngdmar410@gmail.com"); // Email where you receive notifications
                    mail.Subject = "New Appointment Booked!";
                    mail.Body = $"A new appointment has been scheduled for {appointment.Date} at {appointment.Time} by {appointment.Name} ({appointment.Email}).";
                    mail.IsBodyHtml = false;

                    using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                    {
                        smtp.Credentials = new NetworkCredential("youngdmar410@gmail.com", "piyefprpyghqhpzh"); // Use App Password
                        smtp.EnableSsl = true;
                        smtp.Send(mail);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Email Error: {ex.Message}");
            }
        }

        // Sends confirmation email to the customer
        private void SendConfirmationEmail(Appointment appointment)
        {
            try
            {
                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress("youngdmar410@gmail.com");
                    mail.To.Add(appointment.Email); // Sends to the customer's email
                    mail.Subject = "Your Appointment Confirmation";
                    mail.Body = $"Hello {appointment.Name},\n\nYour appointment is confirmed for {appointment.Date} at {appointment.Time}.\n\nSee you soon!";
                    mail.IsBodyHtml = false;

                    using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                    {
                        smtp.Credentials = new NetworkCredential("youngdmar410@gmail.com", "piyefprpyghqhpzh");
                        smtp.EnableSsl = true;
                        smtp.Send(mail);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Email Error: {ex.Message}");
            }
        }
    }
}







